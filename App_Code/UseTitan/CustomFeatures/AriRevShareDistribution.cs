using MarchewkaOne.Titan.Balances;
using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Titan;

public class AriRevShareDistribution
{
    public static void CreditAriRevShareDistribution()
    {
        try
        {
            var balanceLogsList = TableHelper.GetListFromRawQuery<BalanceLog>("SELECT * FROM BalanceLogs WHERE balancelogtype = 6 AND DateOccured IN (SELECT MAX(DateOccured) FROM BalanceLogs WHERE balancelogtype = 6) AND balance = 1");

            foreach (var log in balanceLogsList)
            {
                var referrerId = GerReferrerID(log.UserId);
                if (CheckReferalCondition(referrerId))
                {
                    var member = new Member(referrerId);
                    var memberWhoMatched = new Member(log.UserId);

                    member.AddToMainBalance(new Money(log.Amount), "Matching bonus from " + memberWhoMatched.Name);
                    member.AddToPurchaseBalance(new Money(log.Amount), "Matching bonus from " + memberWhoMatched.Name);

                    member.IncreaseAdPackEarningsFromDR(2 * new Money(log.Amount));
                    member.IncreaseEarningsFromDirectReferral(2 * new Money(log.Amount));

                    member.Save();
                }
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }

    private static int GerReferrerID(int id)
    {
        return (int)TableHelper.SelectScalar(string.Format("SELECT ReferrerId FROM Users WHERE UserId = {0}", id));
    }

    public static bool CheckReferalCondition(int refId)
    {
        var inRef = false;
        var inAdp = false;
        var inExtra = false;

        var referalsIdRestriction = TableHelper.GetListFromRawQuery<Member>(@"SELECT ReferrerId FROM Users WHERE UserId IN 
                (SELECT DISTINCT UserId FROM AdPacks WHERE BalanceBoughtType = 2)
                AND IsRented = 0 GROUP BY ReferrerId HAVING COUNT (ReferrerId) >= 3");

        var userAdPackRestriction = TableHelper.GetListFromRawQuery<AdPack>("SELECT DISTINCT UserId FROM AdPacks WHERE PurchaseDate > GETDATE() - 30");
        var extraRestriction = TableHelper.GetListFromRawQuery<AdPack>("SELECT UserId FROM AdPacks GROUP BY UserId HAVING COUNT (UserId) >= 25");

        foreach (var refe in referalsIdRestriction)
        {
            if (refe.ReferrerId == refId)
            {
                inRef = true;
                break;
            }
        }

        foreach (var user in userAdPackRestriction)
        {
            if (user.UserId == refId)
            {
                inAdp = true;
                break;
            }
        }

        foreach (var user in extraRestriction)
        {
            if (user.UserId == refId)
            {
                inExtra = true;
                break;
            }
        }

        return inRef && inAdp && inExtra;
    }

    public class AdPackAriCrediter : Crediter
    {
        private static bool TargetAdBalance;
        public AdPackAriCrediter(Member User) : base(User) { }

        public Money CreditReferer(Money money, PurchaseBalances targetBalance)
        {
            if (targetBalance == PurchaseBalances.Purchase)
                TargetAdBalance = true;
            else
                TargetAdBalance = false;

            Money moneySpent = CreditReferersMainBalance(money, "AdPack /ref/ " + User.Name, BalanceLogType.AdPackRefPurchase);
            Money moneyLeftForPools = money - moneySpent;

            return money - moneySpent;
        }

        protected override Money CalculateRefEarnings(Member user, Money amount, int tier)
        {
            List<Commission> commissions = (List<Commission>)new CommissionsCache().Get();
            var commission = commissions.FirstOrDefault(c => c.MembershipId == user.Membership.Id && c.RefLevel == tier);

            if (commission == null)
                return Money.Zero;

            if (TargetAdBalance)
                return Money.MultiplyPercent(amount * 0.5M, commission.AdPackPurchasePercent);
            else
                return Money.MultiplyPercent(amount, commission.AdPackPurchasePercent);
        }

    }
}