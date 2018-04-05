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
using Titan.Matrix;

namespace Titan
{
    public class AdPackCrediter : Crediter
    {
        public AdPackCrediter(Member User)
            : base(User)
        {
        }

        public Money CreditReferer(Money money)
        {
            Money moneySpent = CreditReferersMainAndPointBalance(money, "AdPack /ref/ " + User.Name, BalanceLogType.AdPackRefPurchase);
            Money moneyLeftForPools = money - moneySpent;

            return moneyLeftForPools;
        }

        protected Money CreditReferersMainAndPointBalance(Money input, String note, BalanceLogType balanceLogType, int level = 1,
            Member forcedUser = null, bool forceSave = true)
        {
            if (level == 1)
            {
                MoneySpent = Money.Zero;
            }

            if (AppSettings.Referrals.AreIndirectReferralsEnabled && level > AppSettings.Referrals.ReferralEarningsUpToTier)
                return MoneySpent;

            if (forcedUser != null)
                User = forcedUser;

            if (User.HasReferer)
            {
                Member referer = new Member(User.ReferrerId);

                if (!MatrixBase.CanCreditReferer(User, referer))
                    return MoneySpent;

                Money calculated = CalculateRefEarnings(referer, input, level);
                var points = CalculateRefPointsEarnings(referer, input, level);
                Money calculatedPoints = PointsConverter.ToMoney(points);

                //Representatives bonus
                calculated += Representative.IncreaseAmountForRepresentative(input, User, referer.Id, level);

                bool refererCredited = false;
                bool adminCommissionPoolCredited = false;

                if (AppSettings.RevShare.IsRevShareEnabled && referer.Id == AppSettings.RevShare.AdminUserId)
                    adminCommissionPoolCredited = PoolDistributionManager.AddProfit(ProfitSource.RevenueAdminCommissions, calculated + calculatedPoints);

                if (!adminCommissionPoolCredited)
                {
                    if (AppSettings.Misc.SpilloverEnabled && referer.Membership.Id == Membership.Standard.Id)
                        //Add commission to Revenue Pool instead of crediting member
                        SpilloverManager.AddMoneyToRevenuePool(calculated);
                    //Credit
                    else if (!AppSettings.Payments.CommissionBalanceEnabled)
                    {
                        CreditMainBalanceHelper(ref referer, calculated, note, balanceLogType);
                        refererCredited = true;
                    }
                    else
                    {
                        CreditCommissionBalanceHelper(ref referer, calculated, note, balanceLogType);
                        refererCredited = true;
                    }
                    if (calculatedPoints > Money.Zero)
                    {
                        CreditPointsHelper(ref referer, PointsConverter.ToPoints(calculatedPoints), note, BalanceLogType.AdPackPurchase);
                    }
                }


                if (refererCredited)
                {
                    referer.IncreaseAdPackEarningsFromDR(calculated + calculatedPoints);
                    referer.IncreaseDirectRefPointsEarnings(points);
                    referer.IncreaseEarningsFromDirectReferral(calculated);
                    User.TotalAdPacksToDReferer += calculated + calculatedPoints;

                    referer.SaveStatisticsAndBalances();

                    User.TotalPointsEarnedToDReferer += points;
                    User.TotalEarnedToDReferer += calculated;
                }

                User.LastDRActivity = DateTime.Now;

                if (forceSave)
                    User.SaveStatisticsAndBalances();


                try
                {
                    if (AppSettings.Emails.NewReferralCommisionEnabled && calculated + calculatedPoints != Money.Zero)
                        Mailer.SendNewReferralCommisionMessage(referer.Name, User.Name, calculated, referer.Email, note, points);
                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex);
                }
                //Mailer 


                //increase Money spent
                MoneySpent += calculated + calculatedPoints;

                //Indirect referrals don't get paid for CashLinks
                if (AppSettings.Referrals.AreIndirectReferralsEnabled)
                    return CreditReferersMainAndPointBalance(input, note, balanceLogType, level + 1, referer);

            }
            return MoneySpent;
        }

        protected override Money CalculateRefEarnings(Member user, Money amount, int tier)
        {
            List<Commission> commissions = (List<Commission>)new CommissionsCache().Get();
            var commission = commissions.FirstOrDefault(c => c.MembershipId == user.Membership.Id && c.RefLevel == tier);

            if (commission == null)
                return Money.Zero;

            return Money.MultiplyPercent(amount, commission.AdPackPurchasePercent);
        }

        private int CalculateRefPointsEarnings(Member user, Money amount, int tier)
        {
            List<Commission> commissions = (List<Commission>)new CommissionsCache().Get();
            var commission = commissions.FirstOrDefault(c => c.MembershipId == user.Membership.Id && c.RefLevel == tier);

            if (commission == null)
                return 0;

            var moneyAmount = Money.MultiplyPercent(amount, commission.PointsForAdPackPurchasePercent);
            var points = PointsConverter.ToPoints(moneyAmount);

            return points;
        }

        protected override void CreditReferersPoints(int input, string note, BalanceLogType balanceLogType, int level = 1, Member forcedUser = null)
        {
            if (AppSettings.Referrals.AreIndirectReferralsEnabled && level > AppSettings.Referrals.ReferralEarningsUpToTier)
                return;

            if (forcedUser != null)
                User = forcedUser;

            if (User.HasReferer)
            {
                Member referer = new Member(User.ReferrerId);

                //Credit
                CreditPointsHelper(ref referer, input, note, balanceLogType);

                referer.IncreaseDirectRefPointsEarnings(input);
                referer.SaveStatisticsAndBalances();

                User.TotalPointsEarnedToDReferer += input;
                User.SaveStatisticsAndBalances();

                if (AppSettings.Referrals.AreIndirectReferralsEnabled)
                    CreditReferersPoints(input, note, balanceLogType, level + 1, referer);
            }
        }
    }
}