using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using Prem.PTC.Advertising;
using Prem.PTC.Contests;
using Prem.PTC.Referrals;
using Resources;
using MarchewkaOne.Titan.Balances;

namespace Titan
{
    public class TrafficGridCrediter : Crediter
    {
        public TrafficGridCrediter(Member User)
            : base(User)
        {
        }

        public void Credit(ContestCrediter Ad)
        {
            //Not set up yet
        }

        public void CreditUser(Money amount, string what, TrafficGridPrizeType type, int? points = null)
        {
            switch (type)
            {
                case TrafficGridPrizeType.PurchaseBalance:
                    User.AddToPurchaseBalance(amount, "TrafficGrid win", BalanceLogType.Other);
                    User.SaveBalances();
                    History.AddTrafficGridWin(User.Name, what);
                    break;
                case TrafficGridPrizeType.MainBalance:
                    CreditMainBalance(amount, "TrafficGrid win", BalanceLogType.Other);
                    History.AddTrafficGridWin(User.Name, what);
                    break;
                case TrafficGridPrizeType.TrafficBalance:
                    User.AddToTrafficBalance(amount, "TrafficGrid win", BalanceLogType.Other);
                    User.SaveBalances();
                    History.AddTrafficGridWin(User.Name, what);
                    break;
                case TrafficGridPrizeType.DirectReferralLimit:
                    User.DirectReferralLimitEnlargedBy++;
                    User.Save();
                    History.AddTrafficGridWin(User.Name, what);
                    break;
                case TrafficGridPrizeType.RentedReferrals:
                    var rrm = new RentReferralsSystem(User.Name, User.Membership);
                    if (rrm.GetUserRentedReferralsCount() >= User.Membership.RentedReferralsLimit)
                        throw new MsgException(L1.WONERROR);
                    rrm.RentReferrals(1);
                    History.AddTrafficGridWin(User.Name, what);
                    break;
                case TrafficGridPrizeType.Points:
                    CreditPoints(points.Value, "TrafficGrid win", BalanceLogType.Other);
                    History.AddTrafficGridWin(User.Name, what);
                    break;
            }
        }

        public Money CreditReferer(Money money)
        {
            Money moneySpent = CreditReferersMainBalance(money, "TrafficGrid purchase /ref/" + User.Name, BalanceLogType.TrafficGridRefPurchase);
            Money moneyLeftForPools = money - moneySpent;
            return money - moneySpent;
        }
        protected override Money CalculateRefEarnings(Member user, Money amount, int tier)
        {
            List<Commission> commissions = (List<Commission>)new CommissionsCache().Get();
            var commission = commissions.FirstOrDefault(c => c.MembershipId == user.Membership.Id && c.RefLevel == tier);

            if (commission == null)
                return Money.Zero;

            return Money.MultiplyPercent(amount, commission.TrafficGridPurchasePercent);
        }
    }
}