using ExtensionMethods;
using System;
using System.Text;

/// <summary>
/// Summary description for Referrals
/// </summary>
/// 

namespace Prem.PTC.Members
{
    public partial class Member : BaseTableObject
    {
        public static void DeleteReferralsCRON()
        {
            StringBuilder whereSb = new StringBuilder();

            if (AppSettings.DirectReferrals.DirectReferralExpirationEnabled)
            {
                whereSb.AppendFormat(" WHERE ((ReferralSince < '{0}' AND IsRented = 0)", AppSettings.ServerTime.AddDays(-AppSettings.DirectReferrals.DirectReferralExpiration).ToDBString());
                if (AppSettings.DirectReferrals.DirectReferralBuyingEnabled)
                    whereSb.AppendFormat(" OR (ReferrerExpirationDate < '{0}' AND IsRented = 1)", AppSettings.ServerTime.ToDBString());
                whereSb.Append(")");
            }
            else
            {
                if (AppSettings.DirectReferrals.DirectReferralBuyingEnabled)
                    whereSb.AppendFormat(" WHERE (ReferrerExpirationDate < '{0}' AND IsRented = 1)", AppSettings.ServerTime.ToDBString());
                else
                    return;
            }

            whereSb.AppendFormat(" AND Username != 'admin' AND UserId != 1005");            

            StringBuilder updateSb = new StringBuilder();
            updateSb.Append("UPDATE Users SET ");
            if (AppSettings.DirectReferrals.DefaultReferrerEnabled)
            {
                var defaultReferrerId = AppSettings.DirectReferrals.DefaultReferrerId;
                var defaultReferrerName = GetMemberUsername((int)defaultReferrerId);
                whereSb.AppendFormat(" AND Username != '{0}' AND UserId != {1}", defaultReferrerName, defaultReferrerId);
                updateSb.AppendFormat("Referer = '{0}', ReferrerId = {1}", defaultReferrerName, defaultReferrerId);
            }
            else
                updateSb.AppendFormat("Referer = {0}, ReferrerId = {1}", "''", -1);

            string whereQuery = whereSb.ToString();

            updateSb.AppendFormat(", ReferralSince = '{0}', IsRented = 0", DateTime.Now.Zero().ToDBString());
            updateSb.Append(", PointsEarnedToReferer = 0, TotalPTCClicksToDReferer = 0, TotalEarnedToDReferer = 0, TotalPointsEarnedToDReferer = 0, CreditedRefererReward = 0");
            updateSb.AppendFormat(", TotalAdPacksToDReferer = 0, TotalCashLinksToDReferer = 0, LastPointableActivity = null, LastDRActivity = '{0}', ReferrerExpirationDate = NULL", DateTime.Now.Zero().ToDBString());

            string updateQuery = updateSb.ToString();

            string finalQuery = updateQuery + whereQuery;

            TableHelper.ExecuteRawCommandNonQuery(finalQuery);
        }

        /// <summary>
        /// Needs to be saved
        /// </summary>
        public void RemoveReferer()
        {
            //This method is repeated in MemberManager. Number 7
            if (!AppSettings.DirectReferrals.DefaultReferrerEnabled)
            {
                int referrerId = AppSettings.DirectReferrals.DefaultReferrerId.HasValue ? AppSettings.DirectReferrals.DefaultReferrerId.Value : -1;

                Referer = Member.GetMemberUsername(referrerId);
                ReferrerId = referrerId;
                ReferralSince = DateTime.Now;
            }
            else
            {
                Referer = String.Empty;
                ReferrerId = -1;
                ReferralSince = DateTime.Now.Zero();
            }
            PointsEarnedToReferer = 0;
            TotalPTCClicksToDReferer = 0;
            TotalEarnedToDReferer = new Money(0);
            TotalPointsEarnedToDReferer = 0;
            TotalAdPacksToDReferer = new Money(0);
            TotalCashLinksToDReferer = new Money(0);
            LastPointableActivity = null;
            LastDRActivity = DateTime.Now.Zero();
            CreditedRefererReward = false;
        }

        public void TryAddReferer(string username, bool isRented = false)
        {
            this.RemoveReferer();

            if (!String.IsNullOrWhiteSpace(username))
                TryAddRefererRaw(new Member(username), isRented);
        }

        public void TryAddReferer(Member upline, bool isRented = false, DateTime? expirationDate = null)
        {
            this.RemoveReferer();
            TryAddRefererRaw(upline, isRented, expirationDate);
        }

        private void TryAddRefererRaw(Member upline, bool isRented = false, DateTime? expirationDate = null)
        {
            this.Referer = upline.Name;
            this.ReferralSince = DateTime.Now;
            this.IsRented = isRented;

            this.ReferrerId = upline.Id;
            this.ReferrerExpirationDate = expirationDate;

            //HandleAchievement trial - ref connected
            int DirectRefsCount = upline.GetDirectReferralsCount();
            bool ShouldBeSaved = upline.TryToAddAchievements(
                PTC.Achievements.Achievement.GetProperAchievements(
                PTC.Achievements.AchievementType.AfterHavingDirectReferrals, DirectRefsCount));

            if (ShouldBeSaved)
                upline.Save();


            string loggerMsg = " gained a new direct referral - ";

            if (!isRented)
            {
                //Bonus
                if (upline.Membership.PointsPerNewReferral > 0)
                {
                    upline.AddToPointsBalance(upline.Membership.PointsPerNewReferral, "New referral", BalanceLogType.Other, true);
                    upline.PointsToday += upline.Membership.PointsPerNewReferral;
                    upline.TotalPointsGenerated += upline.Membership.PointsPerNewReferral;
                    upline.IncreasePointsEarnings(upline.Membership.PointsPerNewReferral);
                    upline.SaveBalances();
                }

                //Check the contests[if activated instantly] If not activated instantly, wait for email verification
                if (AppSettings.Authentication.IsInstantlyActivated)
                    Contests.ContestManager.IMadeAnAction(Contests.ContestType.Direct, upline.Name, null, 1);

                //Mailer
                if (AppSettings.Emails.NewReferralEnabled)
                    Mailer.SendNewReferralMessage(upline.Name, this.Name, upline.Email);
            }
            else
                loggerMsg = loggerMsg.Replace("gained", "rented");

            ErrorLogger.Log(upline.Name + loggerMsg + this.Name, LogType.RefTrack);
        }
    }
}