using ExtensionMethods;
using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Summary description for CreditLineManager
/// </summary>
public static class CreditLineManager
{
    public static void CRON()
    {
        try
        {
            if (!AppSettings.TitanFeatures.MoneyCreditLineEnabled)
                return;
            var now = AppSettings.ServerTime;

            string query = string.Format(@"SELECT * FROM Users u WHERE u.UserId IN (SELECT UserId FROM CreditLineLoans WHERE Loaned > Repaid);");
            List<Member> users = TableHelper.GetListFromRawQuery<Member>(query);

            foreach (var user in users)            
                AutomaticRepay(user, now);            
        }
        catch (Exception e)
        {
            ErrorLogger.Log(e);
        }
    }

    private static void SendReminder(CreditLineLoan loan, DateTime now, string userName)
    {
        var amountToPay = Money.Zero;
        var deadline = DateTime.MinValue;

        if (loan.FinalDeadline.Date == now.AddDays(-2).Date && loan.Loaned > loan.Repaid)
        {
            amountToPay = loan.Loaned - loan.Repaid;
            deadline = loan.FinalDeadline;
        }
        else if (loan.SecondDeadline.Date == now.AddDays(-2).Date && loan.AmounBeforeSecondDeadline > loan.Repaid)
        {
            amountToPay = loan.AmounBeforeSecondDeadline - loan.Repaid;
            deadline = loan.SecondDeadline;
        }
        else if (loan.FirstDeadline.Date == now.AddDays(-2).Date && loan.AmounBeforeFirstDeadline > loan.Repaid)
        {
            amountToPay = loan.AmounBeforeFirstDeadline - loan.Repaid;
            deadline = loan.FirstDeadline;
        }
        if (amountToPay > Money.Zero && deadline > DateTime.MinValue)
        {
            string email = string.Format(U5006.CREDITLINEREMINDEREMAIL, userName, amountToPay.ToString(), deadline.Date.ToShortDateString(), AppSettings.Site.Name);
            Mailer.SendCreditLineRemainderEmail(email, userName);
        }
    }
    public static List<CreditLineLoan> GetUsersLoans(int userId, bool paid)
    {
        string query = string.Format("SELECT * FROM CreditLineLoans WHERE UserId = {0} AND Repaid {1} Loaned;", userId, paid ? ">=" : "<");
        return TableHelper.GetListFromRawQuery<CreditLineLoan>(query);
    }

    public static bool UserHasUnpaidLoans(int userId)
    {
        string query = string.Format("SELECT COUNT(*) FROM CreditLineLoans WHERE UserId = {0} AND Repaid < Loaned;", userId);

        return (int)TableHelper.SelectScalar(query) > 0;
    }

    public static void TrySendRequest(Member user, Money amount)
    {
        if ((AppSettings.ServerTime - user.Registered).TotalDays < AppSettings.CreditLine.MinimumRegisterDays)
            throw new MsgException(string.Format(U6008.NOTENOUGHDAYS, AppSettings.CreditLine.MinimumRegisterDays));

        var numberOfPendingRequests = GetNumberOfUsersRequests(user.Id, CreditLineRequestStatus.Pending);
        if (numberOfPendingRequests > 0)
            throw new MsgException(U5007.CANONLYSENDONEREQUEST);

        var loans = GetUsersLoans(user.Id, false);
        if (loans.Count > 0)
            throw new MsgException("You must repay your debt first.");

        var maxAmount = GetMaxPossibleRequest(user);

        if (amount > maxAmount)
            throw new MsgException(string.Format("The maximum amount you can borrow is {0}", maxAmount));

        CreditLineRequest request = new CreditLineRequest();
        request.UserId = user.Id;
        request.LoanRequested = amount;
        request.RequestDate = AppSettings.ServerTime;
        request.Status = CreditLineRequestStatus.Pending;
        request.Save();
    }

    public static void AcceptRequest(int creditLineRequestId)
    {
        Money maxLoan = GlobalPool.Get(PoolsHelper.GetBuiltInProfitPoolId(Pools.CreditLine)).SumAmount;

        CreditLineRequest request = new CreditLineRequest(creditLineRequestId);

        if (request.LoanRequested > maxLoan)
        {
            request.Status = CreditLineRequestStatus.Rejected;
            request.Save();
            throw new MsgException("There is not enough money in the pool. The request has been rejected.");
        }

        request.Status = CreditLineRequestStatus.Accepted;
        request.Save();

        Member user = new Member(request.UserId);
        DateTime now = AppSettings.ServerTime;
        CreditLineLoan loan = new CreditLineLoan();
        loan.UserId = user.Id;
        loan.Loaned = request.LoanRequested;
        loan.FirstDeadline = now.AddDays(AppSettings.CreditLine.FirstDeadlineDays);
        loan.SecondDeadline = now.AddDays(AppSettings.CreditLine.SecondDeadlineDays);
        loan.FinalDeadline = now.AddDays(AppSettings.CreditLine.FinalDeadlineDays);
        loan.AmounBeforeFirstDeadline = Money.MultiplyPercent(request.LoanRequested, AppSettings.CreditLine.FirstRepayPercent);
        loan.AmounBeforeSecondDeadline = Money.MultiplyPercent(request.LoanRequested, AppSettings.CreditLine.SecondRepayPercent);
        loan.BorrowDate = now;
        loan.Repaid = Money.Zero;
        loan.Save();

        var moneyWithoutFee = Money.MultiplyPercent(request.LoanRequested, 100 - AppSettings.CreditLine.Fee);

        GlobalPoolManager.SubtractFromPool(PoolsHelper.GetBuiltInProfitPoolId(Pools.CreditLine), moneyWithoutFee);

        user.AddToPurchaseBalance(moneyWithoutFee, "Credit Line", BalanceLogType.CreditLine);
        user.SaveBalances();

        Mailer.SendNewCreditLineMessage(user.Email, request.LoanRequested);
    }

    public static void RejectRequest(int creditLineRequestId)
    {
        CreditLineRequest request = new CreditLineRequest(creditLineRequestId);
        request.Status = CreditLineRequestStatus.Rejected;
        request.Save();
    }

    public static Money GetMaxPossibleRequest(Member user)
    {
        var amountInPool = GlobalPool.Get(PoolsHelper.GetBuiltInProfitPoolId(Pools.CreditLine)).SumAmount;
        var pendingSum = GetPendingRequestsSumAmount();
        var left = amountInPool - pendingSum;

        var amounts = new List<Money>();
        amounts.Add(left);
        amounts.Add(user.Membership.MaxCreditLineRequest);

        if (TitanFeatures.IsCopiousassets)
            amounts.Add(Money.MultiplyPercent(GetThisWeeksMarketplaceSpendings(user.Id), 10));

        return amounts.Min();
    }

    private static Money GetThisWeeksMarketplaceSpendings(int userId)
    {
        var query = string.Format(@"SELECT SUM(p.Price * i.ProductQuantity) 
                                    FROM MarketplaceIPNs i JOIN MarketplaceProducts p ON i.ProductId = p.Id 
                                    WHERE DateAdded >= '{0}' AND BuyerId = {1}", AppSettings.ServerTime.AddDays(-7).ToDBString(), userId);

        object spendingsObj = TableHelper.SelectScalar(query);

        if (spendingsObj is DBNull)
            return Money.Zero;

        return new Money((decimal)spendingsObj);
    }

    public static void AutomaticRepay(Member user, DateTime now)
    {
        List<CreditLineLoan> loans = GetUsersLoans(user.Id, false);
        Money debt = GetUsersDebt(loans, now, user.Name, true);

        if (debt <= Money.Zero)
            return;

        var availableMoney = Money.Zero;
        var adBalance = user.PurchaseBalance;

        if (adBalance <= debt)        
            availableMoney = adBalance;        
        else 
            availableMoney = debt;        
        user.SubtractFromPurchaseBalance(availableMoney, "Credit Line", BalanceLogType.CreditLine);

        //Subtract rest of the debt from Main Balance, balance can go to negative
        var debtLeft = debt - availableMoney;
        if (debtLeft > Money.Zero)
        {
            user.SubtractFromMainBalance(debtLeft, "Credit Line", BalanceLogType.CreditLine);
            availableMoney += debtLeft;
        }
        user.SaveBalances();

        Money moneyForPool = Money.Zero;
        
        foreach (var loan in loans)
        {
            if (availableMoney <= Money.Zero)
                return;

            var toPay = Money.Zero;

            if (loan.FinalDeadline < now)            
                toPay = loan.Loaned - loan.Repaid;            
            else if (loan.SecondDeadline < now)            
                toPay = loan.AmounBeforeSecondDeadline - loan.Repaid;            
            else if (loan.FirstDeadline < now)            
                toPay = loan.AmounBeforeFirstDeadline - loan.Repaid;            

            if (toPay > Money.Zero)
            {
                if (toPay > availableMoney)
                {
                    loan.Repaid += availableMoney;
                    moneyForPool += availableMoney;
                    availableMoney -= availableMoney;
                }
                else
                {
                    loan.Repaid += toPay;
                    moneyForPool += toPay;
                    availableMoney -= toPay;
                }
            }
            loan.Save();
        }
        if (moneyForPool > Money.Zero)
            GlobalPoolManager.AddToPool(PoolsHelper.GetBuiltInProfitPoolId(Pools.CreditLine), moneyForPool);
    }

    /// <summary>
    /// Returns total debt that should have been be paid until now
    /// </summary>
    /// <param name="loans"></param>
    /// <returns></returns>
    private static Money GetUsersDebt(List<CreditLineLoan> loans, DateTime now, string userName = null, bool sendReminder = false)
    {
        Money debt = Money.Zero;

        foreach (var loan in loans)
        {
            var toPay = Money.Zero;

            if (loan.FinalDeadline < now)            
                toPay = loan.Loaned - loan.Repaid;            
            else if (loan.SecondDeadline < now)            
                toPay = loan.AmounBeforeSecondDeadline - loan.Repaid;            
            else if (loan.FirstDeadline < now)            
                toPay = loan.AmounBeforeFirstDeadline - loan.Repaid;            

            debt += toPay > new Money(0) ? toPay : Money.Zero;
            if (sendReminder && !string.IsNullOrWhiteSpace(userName))
                SendReminder(loan, now, userName);
        }
        return debt;
    }

    public static void TryRepayDebt(Money amount, Member user, out Money repaidAmound)
    {
        repaidAmound = Money.Zero;
        var loan = GetUsersLoans(user.Id, false).FirstOrDefault();
        if (loan != null)
        {
            if (amount <= Money.Zero)
                throw new MsgException("Amount must be higher than 0");

            if (amount > user.PurchaseBalance)
                throw new MsgException(U6012.CREDITLINEPURCHASEBALANCEERROR);

            repaidAmound = loan.Loaned - loan.Repaid;
            if (amount > repaidAmound)
                amount = repaidAmound;
            else
                repaidAmound = amount;

            loan.Repaid += amount;
            loan.Save();
            GlobalPoolManager.AddToPool(PoolsHelper.GetBuiltInProfitPoolId(Pools.CreditLine), amount);
            user.SubtractFromPurchaseBalance(amount, "Credit Line", BalanceLogType.CreditLine);
            user.SaveBalances();
        }
    }

    private static int GetNumberOfUsersRequests(int userId, CreditLineRequestStatus status)
    {
        return (int)TableHelper.SelectScalar(string.Format("SELECT COUNT(*) FROM CreditLineRequests WHERE UserId = {0} AND Status = {1}", userId, (int)status));
    }

    private static Money GetPendingRequestsSumAmount()
    {
        object helper = TableHelper.SelectScalar(string.Format("SELECT SUM(LoanRequested) FROM CreditLineRequests WHERE Status = {0}", (int)CreditLineRequestStatus.Pending));

        if (helper is DBNull)
            return Money.Zero;

        return new Money((decimal)helper);
    }
}