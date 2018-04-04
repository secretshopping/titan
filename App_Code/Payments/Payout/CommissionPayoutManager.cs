using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// No support for Custom Processors
/// </summary>
public class CommissionPayoutManager
{
    private Member user;
    private readonly Money amountToPayout;
    private readonly string paymentProcessor;

    public CommissionPayoutManager(Member user, Money amountToPayout, string paymentProcessor)
    {
        this.user = user;
        this.amountToPayout = amountToPayout;
        this.paymentProcessor = paymentProcessor;
    }

    public string TryMakePayout()
    {
        ValidatePayout();

        string paymentAddress = PaymentAccountDetails.GetPaymentProcessorUserAccount(user, paymentProcessor);

        var request = new TransactionRequest(user.Name, paymentAddress, amountToPayout);
        Transaction transaction = TransactionFactory.CreateTransaction(request, paymentProcessor);
        var response = transaction.Commit();

        if (!response.IsSuccess)
        {
            if (request != null && response != null)
                PayoutManager.logPayout("Commission Balance Payout unsuccessful", request, response, paymentProcessor);
            throw new MsgException(response.Note);
        }

        PayoutRequest req = new PayoutRequest();

        req.Amount = amountToPayout;
        req.IsPaid = true;
        req.RequestDate = DateTime.Now;
        req.Username = user.Name;
        req.IsRequest = false;
        req.BalanceType = BalanceType.CommissionBalance;
        req.PaymentAddress = paymentAddress;
        req.PaymentProcessor = paymentProcessor;
        req.Save();

        user.SubtractFromCommissionBalance(amountToPayout, "Payout");
        user.SaveBalances();

        History.AddCashout(user.Name, amountToPayout);
        PayoutManager.logPayout("Commission Balance Payout successful", request, response, paymentProcessor);
        PaymentProof.Add(user, req);

        return U3501.AUTOMATICCASHOUTSUCC + ": " + response.Note;
    }

    private void ValidatePayout()
    {
        if (amountToPayout > user.CommissionBalance)
            throw new MsgException(L1.NOTENOUGHFUNDS);

        if (user.Status == MemberStatus.VacationMode)
            throw new MsgException(U4000.YOUAREINVACATIONMODE);

        //Validate Credit Loans
        if (CreditLineManager.UserHasUnpaidLoans(user.Id))
            throw new MsgException(U6008.REPAYCREDITLINETOWITHDRAW);

        if (user.NumberOfCommissionPayoutsToday + 1 > user.Membership.MaxCommissionPayoutsPerDay)
        {
            throw new MsgException(string.Format(U6000.TOOMANYWITHDRAWSTODAY, user.Membership.MaxCommissionPayoutsPerDay));
        }
    }
}