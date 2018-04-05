using System;

namespace Prem.PTC.Memberships
{
    public interface IMembership : ITableObject
    {
        Money AdvertClickEarnings { get; set; }
        Money DirectReferralAdvertClickEarnings { get; set; }
        int DirectReferralsLimit { get; set; }
        int DisplayOrder { get; set; }
        MembershipStatus Status { get; set; }
        string Name { get; set; }
        Money ReferralRentCost { get; set; }
        int RenewalDiscount { get; set; }
        Money RentedReferralAdvertClickEarnings { get; set; }
        int RentedReferralsLimit { get; set; }
        /// <summary>
        /// Cost of replacing rented referral to another one at any moment.
        /// </summary>
        Money RentedReferralRecycleCost { get; set; }
        /// <summary>
        /// Whether this membership has possibility to have autopay enabled
        /// </summary>
        bool CanAutoPay { get; set; }
        /// <summary>
        /// Daily cost of auto pay service per one rented referral
        /// </summary>
        Money DailyAutoPayCost { get; set; }
        /// <summary>
        /// Minimal timespan between acquisition of two rented referral packs
        /// </summary>
        TimeSpan MinRentingInterval { get; set; }
        int AdvertPointsEarnings { get; set; }
        string Color { get; set; }

        int TrafficGridTrials { get; set; }
        int TrafficGridChances { get; set; }
        int TrafficGridShorterAd { get; set; }

        int OfferwallsProfitPercent { get; set; }
        Money CashoutLimitIcreased { get; set; }
        Money CashoutLimit { get; set; }
        Money MaxDailyCashout { get; set; }
        Money MaxGlobalCashout { get; set; }

        int CPAProfitPercent { get; set; }
        int RefPercentEarningsOfferwalls { get; set; }
        int RefPercentEarningsCPA { get; set; }
        bool HasInstantPayout { get; set; }

        int MaxRefPackageCount { get; set; }
        decimal DirectReferralAdPackPurchaseEarnings { get; set; }
        int ROIEnlargedByPercentage { get; set; }
        int AdPackDailyRequiredClicks { get; set; }
        int AdPackAdBalanceReturnPercentage { get; set; }

        int SameUserCommissionToMainTransferFee { get; set; }
        int OtherUserMainToAdTransferFee { get; set; }
        int OtherUserMainToMainTransferFee { get; set; }
        int OtherUserPointsToPointsTransferFee { get; set; }

        decimal PTCCreditsPerView { get; set; }
        int PTCPurchaseCommissionPercent { get; set; }

        int PointsYourPTCAdBeingViewed { get; set; }
        int PointsPer1000viewsDeliveredToPoolRotator { get; set; }
        int PointsPerNewReferral { get; set; }
        int AutosurfViewLimitMonth { get; set; }
        int MinAdsWatchedMonthlyToKeepYourLevel { get; set; }
        int MinPointsToHaveThisLevel { get; set; }
        int MaxActivePtcCampaignLimit { get; set; }      
        int MaxExtraAdPackSecondsForClicks { get; set; }
        int MaxUpgradedDirectRefs { get; set; }
        int DirectReferralBannerPurchaseEarnings { get; set; }
        int DirectReferralMembershipPurchaseEarnings { get; set; }
        int DirectReferralAdvertClickEarningsPoints { get; set; }
        int DirectReferralTrafficGridPurchaseEarnings { get; set; }
        Money TrafficExchangeClickEarnings { get; set; }
        Money DRTrafficExchangeClickEarnings { get; set; }
        int MaxDailyPtcClicks { get; set; }
        int MaxDailyPayouts { get; set; }
        decimal PublishersBannerClickProfitPercentage { get; set; }
        decimal PublishersCpaOfferProfitPercentage { get; set; }
        decimal PublishersInTextAdClickProfitPercentage { get; set; }
        decimal PublishersPtcOfferWallProfitPercentage { get; set; }

        Money ExposureMiniClickEarnings { get; set; }
        Money ExposureMiniDirectClickEarnings { get; set; }
        Money ExposureMiniRentedClickEarnings { get; set; }
        Money ExposureMicroClickEarnings { get; set; }
        Money ExposureMicroDirectClickEarnings { get; set; }
        Money ExposureMicroRentedClickEarnings { get; set; }
        Money ExposureFixedClickEarnings { get; set; }
        Money ExposureFixedDirectClickEarnings { get; set; }
        Money ExposureFixedRentedClickEarnings { get; set; }
        Money ExposureStandardClickEarnings { get; set; }
        Money ExposureStandardDirectClickEarnings { get; set; }
        Money ExposureStandardRentedClickEarnings { get; set; }
        Money ExposureExtendedClickEarnings { get; set; }
        Money ExposureExtendedDirectClickEarnings { get; set; }
        Money ExposureExtendedRentedClickEarnings { get; set; }
        int MaxFacebookLikesPerDay { get; set; }
        Money NewReferralReward { get; set; }

        Money MinReferralEarningsToCreditReward { get; set; }
        decimal MaxWithdrawalAllowedPerInvestmentPercent { get; set; }
        int MaxCommissionPayoutsPerDay { get; set; }
        bool CanUseRefTools { get; set; }

        Money InvestmentPlatformMinAmountToCredited { get; set; }
        Money MaxCreditLineRequest { get; set; }
        Money MiniVideoUploadPrice { get; set; }
        Money MiniVideoWatchPrice { get; set; }

        int AdPacksAdsPointsReward { get; set; }
        decimal ICOPurchasePercent { get; set; }

        Money ArticleCreatorCPM { get; set; }
        Money ArticleInfluencerCPM { get; set; }
        int BlockPayoutDays { get; set; }

        #region Balances Bonuses
        Money AdBalanceBonusForUpgrade { get; set; }
        Money CashBalanceBonusForUpgrade { get; set; }
        Money TrafficBalanceBonusForUpgrade { get; set; }
        #endregion

        #region Levels
        int LevelChanceToWinAnyAward { get; set; }
        int LevelPointsPrizeChance { get; set; }
        int LevelPointsPrizeMin { get; set; }
        int LevelPointsPrizeMax { get; set; }
        int LevelAdCreditsChance { get; set; }
        int LevelAdCreditsMin { get; set; }
        int LevelAdCreditsMax { get; set; }
        int LevelDRLimitIncreasedChance { get; set; }
        int LevelDRLimitIncreasedMin { get; set; }
        int LevelDRLimitIncreasedMax { get; set; }
        #endregion
    }
}