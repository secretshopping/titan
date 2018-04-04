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
    public class SearchCrediter : Crediter
    {
        public static readonly string Note = "GPT Search";

        public SearchCrediter(Member User)
            : base(User)
        {
        }

        /// <summary>
        /// Throws MsgException when feature is not available in memebrs country
        /// </summary>
        public bool CreditSearch()
        {
            FeatureManager Manager = new FeatureManager(base.User, GeolocatedFeatureType.Search);

            if (!Manager.IsAllowed)
                throw new MsgException(U4000.GEONOTAVAILABLE.Replace("%p%", AppSettings.PointsName));

            //Anti-Fraud
            bool IsEligibleForCredit = true;
            int reward = Manager.Reward;

            //Max daily limit
            if (User.PointsCreditedTodayForSearch + reward > AppSettings.SearchAndVideo.MaxPointsDailyForSearch)
                IsEligibleForCredit = false;

            //Security timer
            if (User.LastCreditedSearch.AddSeconds(AppSettings.SearchAndVideo.SearchCreditingBlockedInMinutes) > DateTime.Now)
                IsEligibleForCredit = false;

            if (IsEligibleForCredit)
            {
                base.CreditPoints(reward, Note, BalanceLogType.GPTSearch);
                User.LastCreditedSearch = DateTime.Now;
                User.PointsCreditedTodayForSearch += reward;
            }

            User.TotalSearchesDone++;
            User.SaveSearchAndVideo();

            //General statistics
            StatisticsManager.Add(StatisticsType.SearchesMade, 1);

            return IsEligibleForCredit;
        }
        protected override Money CalculateRefEarnings(Member user, Money amount, int tier)
        {
            return Money.Zero;
        }
    }
}