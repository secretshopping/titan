using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prem.PTC.Achievements
{
    /// <summary>
    /// Corresponding AchievementType is always related to the Quantity
    /// </summary>
    public enum AchievementType
    {
        Null = 0,

        AfterClicks = 1,

        AfterTransferringOnceAmount = 2,

        AfterAdvertisingPtcCampaigns = 3,
        AfterAdvertisingBannerCampaigns = 4,
        AfterAdvertisingFacebookCampaigns = 5,

        AfterHavingDirectReferrals = 6,
        AfterHavingRentedReferrals = 7,

        AfterEarning = 8,

        AfterAdvertisingTrafficGridCampaigns = 9,
        AfterWinningInTrafficGrid = 10,

        AfterGettingPointsInOneDay = 11,
        AfterCPAOffersCompleted = 12


        // NOTE: When you add new type, make sure to add also a description handler to 
        // 'private string Achievement.GetAssociatedResource(AchievementType Type)'
        // otherwise default description will be provided
    };

}