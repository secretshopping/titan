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
using Prem.PTC.Offers;
using Resources;
using Prem.PTC.Statistics;
using MarchewkaOne.Titan.Balances;

namespace Titan
{
    public class VideoCrediter : Crediter
    {
        public static readonly string Note = "GPT Video";


        public VideoCrediter(Member User)
            : base(User)
        {
        }

        /// <summary>
        /// Throws MsgException when feature is not available in memebrs country
        /// </summary>
        public bool CreditVideo(Video video = null)
        {
            FeatureManager Manager = new FeatureManager(base.User, GeolocatedFeatureType.Video);

            if (!Manager.IsAllowed)
                throw new MsgException(U4000.GEONOTAVAILABLE.Replace("%p%", AppSettings.PointsName));

            //Anti-Fraud
            bool IsEligibleForCredit = true;
            int reward = Manager.Reward;

            //Max daily limit
            if (User.PointsCreditedTodayForVideo + reward > AppSettings.SearchAndVideo.MaxPointsDailyForVideo)
                IsEligibleForCredit = false;

            if (IsEligibleForCredit)
            {
                base.CreditPoints(reward, Note, BalanceLogType.GPTVideo);
                base.CreditReferersPoints(reward, Note, BalanceLogType.GPTVideo);
                User.PointsCreditedTodayForVideo += reward;
            }

            User.TotalVideosWatched++;
            User.SaveSearchAndVideo();

            //General statistics
            StatisticsManager.Add(StatisticsType.VideosWatched, 1);

            return IsEligibleForCredit;
        }

        protected override Money CalculateRefEarnings(Member user, Money amount, int tier)
        {
            List<Commission> commissions = (List<Commission>)new CommissionsCache().Get();
            var commission = commissions.FirstOrDefault(c => c.MembershipId == user.Membership.Id && c.RefLevel == tier);

            if (commission == null)
                return Money.Zero;

            return Money.MultiplyPercent(amount, commission.VideoViewPercent);
        }
    }
}