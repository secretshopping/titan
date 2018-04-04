using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using MarchewkaOne.Titan.Balances;
using Prem.PTC.Statistics;
using Titan.Matrix;

namespace Titan
{
    /// <summary>
    /// Solid class for flexible and secure members crediting
    /// </summary>
    public abstract class Crediter
    {
        public Money MoneySpent { get; set; }

        #region Constructors

        protected Member User;
        protected Member OriginalUser;

        public Crediter(Member user)
        {
            this.User = user;
            //User.Membership.Reload(); performance boot
        }

        #endregion Constructors

        #region Credit functions

        protected void CreditPoints(int Input, String note, BalanceLogType balanceLogType)
        {
            CreditPoints(this.User, Input, note, balanceLogType);
        }

        protected void CreditMainBalance(Money Input, String note, BalanceLogType balanceLogType, bool forceSave = true)
        {
            CreditMainBalance(this.User, Input, note, balanceLogType, forceSave);
        }

        protected static void CreditPoints(Member User, int Input, String note, BalanceLogType balanceLogType)
        {
            //Credit
            CreditPointsHelper(ref User, Input, note, balanceLogType);

            //Save
            User.SaveStatisticsAndBalances();

            //Achievements
            User.TryToAddAchievements(
               Prem.PTC.Achievements.Achievement.GetProperAchievements(
               Prem.PTC.Achievements.AchievementType.AfterGettingPointsInOneDay, User.PointsToday));
        }

        protected static void CreditMainBalance(Member User, Money Input, String note, BalanceLogType balanceLogType, bool forceSave = true)
        {
            //Credit
            CreditMainBalanceHelper(ref User, Input, note, balanceLogType);

            if (forceSave)
            {
                //Save     
                User.SaveStatisticsAndBalances();
            }

            //Achievements
            User.TryToAddAchievements(
               Prem.PTC.Achievements.Achievement.GetProperAchievements(
               Prem.PTC.Achievements.AchievementType.AfterEarning, User.MainBalance.GetRealTotals()));
        }

        #endregion Credit functions

        #region CreditReferer functions

        protected abstract Money CalculateRefEarnings(Member user, Money amount, int tier);
        protected virtual void CreditReferersPoints(int input, String note, BalanceLogType balanceLogType, int level = 1, Member forcedUser = null)
        {
            if (AppSettings.Referrals.AreIndirectReferralsEnabled && level > AppSettings.Referrals.ReferralEarningsUpToTier)
                return;

            if (level == 1)
            {
                MoneySpent = Money.Zero;
                OriginalUser = User;
            }

            if (forcedUser != null)
                User = forcedUser;

            if (User.HasReferer)
            {
                Member referer = new Member(User.ReferrerId);

                if (referer.HasClickedEnoughToProfitFromReferrals())
                {
                    int calculated = PointsConverter.ToPoints(CalculateRefEarnings(referer, PointsConverter.ToMoney(input), level));

                    //Credit
                    CreditPointsHelper(ref referer, calculated, note, balanceLogType);

                    referer.IncreaseDirectRefPointsEarnings(calculated);
                    referer.SaveStatisticsAndBalances();

                    User.TotalPointsEarnedToDReferer += calculated;
                    User.SaveStatisticsAndBalances();

                    if (AppSettings.Referrals.AreIndirectReferralsEnabled)
                    {
                        CreditReferersPoints(input, note, balanceLogType, level + 1, referer);

                        //change to Original user when leaving recurrence
                        if (level == 1 && User.Id != OriginalUser.Id)
                            User = OriginalUser;
                    }
                }
            }
        }

        protected virtual Money CreditReferersMainBalance(Money input, String note, BalanceLogType balanceLogType, int level = 1,
            Member forcedUser = null, bool forceSave = true)
        {
            if (AppSettings.Referrals.AreIndirectReferralsEnabled && level > AppSettings.Referrals.ReferralEarningsUpToTier ||
                TitanFeatures.IsGambinos && User.MembershipId == 1)
                return Money.Zero;

            if (level == 1)
            {
                MoneySpent = Money.Zero;
                OriginalUser = User;
            }

            if (forcedUser != null)
                User = forcedUser;

            if (User.HasReferer)
            {
                Member referer = new Member(User.ReferrerId);

                if (referer.HasClickedEnoughToProfitFromReferrals())
                {
                    if (!MatrixBase.CanCreditReferer(User, referer))
                        return MoneySpent;

                    Money calculated = CalculateRefEarnings(referer, input, level);

                    //Representatives bonus
                    calculated += Representative.IncreaseAmountForRepresentative(input, User, referer.Id, level);

                    bool refererCredited = false;
                    bool adminCommissionPoolCredited = false;

                    if (AppSettings.RevShare.IsRevShareEnabled && referer.Id == AppSettings.RevShare.AdminUserId)
                        adminCommissionPoolCredited = PoolDistributionManager.AddProfit(ProfitSource.RevenueAdminCommissions, calculated);

                    if (!adminCommissionPoolCredited)
                    {
                        //Add commission to Revenue Pool instead of crediting member
                        if (AppSettings.Misc.SpilloverEnabled && referer.Membership.Id == Membership.Standard.Id)
                            SpilloverManager.AddMoneyToRevenuePool(calculated);
                        //Credit
                        else
                        {
                            if (!AppSettings.Payments.CommissionBalanceEnabled)
                                CreditMainBalanceHelper(ref referer, calculated, note, balanceLogType);
                            else
                                CreditCommissionBalanceHelper(ref referer, calculated, note, balanceLogType);
                                
                            refererCredited = true;
                            AddToCommissionsIncomeStatistics(referer, calculated);
                        }
                    }

                    if (refererCredited)
                    {
                        if (this is AdPackCrediter)
                        {
                            referer.IncreaseAdPackEarningsFromDR(calculated);
                            User.TotalAdPacksToDReferer += calculated;
                        }

                        if (this is CashLinkCrediter)
                        {
                            referer.IncreaseDRCashLinksEarnings(calculated);
                            User.TotalCashLinksToDReferer += calculated;
                        }

                        referer.IncreaseEarningsFromDirectReferral(calculated);
                        referer.SaveStatisticsAndBalances();

                        User.TotalEarnedToDReferer += calculated;
                    }

                    User.LastDRActivity = DateTime.Now;

                    if (forceSave)
                        User.SaveStatisticsAndBalances();


                    try
                    {
                        if (AppSettings.Emails.NewReferralCommisionEnabled && calculated != Money.Zero)
                            Mailer.SendNewReferralCommisionMessage(referer.Name, User.Name, calculated, referer.Email, note);
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.Log(ex);
                    }
                    //Mailer 

                    //increase Money spent
                    MoneySpent += calculated;

                    //Indirect referrals don't get paid for CashLinks
                    if (AppSettings.Referrals.AreIndirectReferralsEnabled && !(this is CashLinkCrediter))
                    {
                        var referralsMoneySpent = CreditReferersMainBalance(input, note, balanceLogType, level + 1, referer);

                        //change to Original user when leaving recurrence
                        if (level == 1 && User.Id != OriginalUser.Id)
                            User = OriginalUser;

                        return referralsMoneySpent;
                    }
                }
            }  
            return MoneySpent;
        }


        #endregion CreditReferer functions

        #region Helpers

        protected static void CreditPointsHelper(ref Member User, int Input, string note, BalanceLogType balanceLogType)
        {
            User.AddToPointsBalance(Input, note, balanceLogType);
            User.PointsToday += Input;
            User.TotalPointsGenerated += Input;
            User.IncreasePointsEarnings(Input);
        }

        protected static void CreditMainBalanceHelper(ref Member User, Money Input, string note, BalanceLogType balanceLogType)
        {
            User.AddToMainBalance(Input, note, balanceLogType);
            User.IncreaseEarnings(Input);
        }

        protected static void CreditCommissionBalanceHelper(ref Member User, Money Input, string note, BalanceLogType balanceLogType)
        {
            User.AddToCommissionBalance(Input, note, balanceLogType);
            User.IncreaseEarnings(Input);
        }

        protected static void AddToCommissionsIncomeStatistics(Member user, Money amount)
        {
            user.StatsCommissionsCurrentWeekIncome += amount;
            user.StatsCommissionsCurrentMonthIncome += amount;
            user.SaveStatistics();
        }
        #endregion
    }
}