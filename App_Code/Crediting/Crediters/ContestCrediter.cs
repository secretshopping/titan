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
using MarchewkaOne.Titan.Balances;

namespace Titan
{
    public class ContestCrediter : Crediter
    {
        public ContestCrediter(Member User)
            : base(User)
        {
        }

        public void Credit(PrizeType Type, Money Value, int Place)
        {
            bool IsRentRefReward = false;
            int RefsRented = 0;

            switch (Type)
            {
                case PrizeType.PurchaseBalance:
                    User.AddToPurchaseBalance(Value, "Contest prize", BalanceLogType.Other);
                    break;

                case PrizeType.DirectRefLimit:
                    User.DirectReferralLimitEnlargedBy += Value.GetRealTotals();
                    break;

                case PrizeType.MainBalance:
                    base.CreditMainBalance(Value, "Contest prize", BalanceLogType.Other);
                    break;

                case PrizeType.Points:
                    base.CreditPoints(Value.GetRealTotals(), "Contest prize", BalanceLogType.Other);
                    break;

                case PrizeType.RentalBalance:
                    User.AddToTrafficBalance(Value, "Contest prize", BalanceLogType.Other);
                    break;

                case PrizeType.RentedReferral:
                    IsRentRefReward = true;
                    var rrm = new Prem.PTC.Referrals.RentReferralsSystem(User.Name, User.Membership);
                    int SpotsLeft = User.Membership.RentedReferralsLimit - rrm.GetUserRentedReferralsCount();

                    RefsRented = ContestManager.Minimum(SpotsLeft, Value.GetRealTotals());

                    if (RefsRented > 0)
                        rrm.RentReferrals(RefsRented);

                    break;
            }

            if (!IsRentRefReward)
            {
                User.Save();
                History.AddContestWin(User.Name, Place, ContestManager.GetPrizeProperObject(Type, Value).ToString() + " [%" + (int)Type + "%]");
            }
            else
                History.AddContestWin(User.Name, Place, RefsRented + " referrals");

            User.Save();
        }

        protected override Money CalculateRefEarnings(Member user, Money amount, int tier)
        {
            return Money.Zero;
        }
    }
}