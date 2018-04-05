using Prem.PTC.Members;
using Prem.PTC.Utils;
using Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using Titan.Leadership;

namespace Prem.PTC.Memberships
{
    [Serializable]
    public class Membership : BaseTableObject, IMembership
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return AppSettings.TableNames.Memberships; } }
        protected override string dbTable { get { return TableName; } }

        public static class Columns
        {
            public const string Id = "MembershipId";
            public const string Name = "Name";
            [Obsolete("Status is introduced instead.", true)]
            public const string IsActive = "IsActive";
            public const string MembershipStatus = "Status";
            public const string DisplayOrder = "DisplayOrder";
            public const string AdvertClickEarnings = "AdvertClickEarnings";
            public const string DirectReferralAdvertClickEarnings = "DirectReferralAdvertClickEarnings";
            public const string RentedReferralAdvertClickEarnings = "RentedReferralAdvertClickEarnings";
            public const string DirectReferralsLimit = "DirectReferralsLimit";
            public const string RentedReferralsLimit = "RentedReferralsLimit";
            public const string ReferralRentCost = "ReferralRentCost";
            public const string RenewalDiscount = "RenewalDiscount";
            public const string AdvertPointsEarnings = "AdvertPointsEarnings";
            public const string Color = "Color";
            public const string RentedReferralRecycleCost = "RentedReferralRecycleCost";
            public const string CanAutoPay = "CanAutoPay";
            public const string DailyAutoPayCost = "DailyAutoPayCost";
            public const string MinRentingIntervalSecs = "MinRentingIntervalSecs";

            public const string TrafficGridTrials = "TrafficGridTrials";
            public const string TrafficGridChances = "TrafficGridChances";
            public const string TrafficGridShorterAd = "TrafficGridShorterAd";

            public const string DirectReferralAdPackPurchaseEarnings = "DirectReferralAdPackPurchaseEarnings";
            public const string AdPackAdBalanceReturnPercentage = "AdPackAdBalanceReturnPercentage";
            public const string MaxDailyCashout = "MaxDailyCashout";
            public const string MaxGlobalCashout = "MaxGlobalCashout";

            public const string MaxExtraAdPackSecondsForClicks = "MaxExtraAdPackSecondsForClicks";
            public const string MaxUpgradedDirectRefs = "MaxUpgradedDirectRefs";
            public const string DirectReferralBannerPurchaseEarnings = "DirectReferralBannerPurchaseEarnings";
            public const string DirectReferralMembershipPurchaseEarnings = "DirectReferralMembershipPurchaseEarnings";
            public const string DirectReferralAdvertClickEarningsPoints = "DirectReferralAdvertClickEarningsPoints";
            public const string DirectReferralTrafficGridPurchaseEarnings = "DirectReferralTrafficGridPurchaseEarnings";
            public const string TrafficExchangeClickEarnings = "TrafficExchangeClickEarnings";

            public const string InvestmentPlatformMinAmountToCredited = "InvestmentPlatformMinAmountToCredited";

            public const string AdBalanceBonusForUpgrade = "AdBalanceBonusForUpgrade";
            public const string CashBalanceBonusForUpgrade = "CashBalanceBonusForUpgrade";
            public const string TrafficBalanceBonusForUpgrade = "TrafficBalanceBonusForUpgrade";

            public const string MiniVideoUploadPrice = "MiniVideoUploadPrice";
            public const string MiniVideoWatchPrice = "MiniVideoWatchPrice";

            public const string AdPacksAdsPointsReward = "AdPacksAdsPointsReward";
            public const string BlockPayoutDays = "BlockPayoutDays";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        /// <exception cref="ArgumentNullException" />
        [Column(Columns.Name)]
        public string Name
        {
            get { return _name; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException();

                _name = value; SetUpToDateAsFalse();
            }
        }

        [Obsolete("Status is introduced instead.", true)]
        public bool IsActive { get { return _isActive; } set { _isActive = value; } }

        [Column(Columns.MembershipStatus)]
        protected int MembershipStatus { get { return _membershipStatus; } set { _membershipStatus = value; SetUpToDateAsFalse(); } }

        [Column(Columns.DisplayOrder)]
        public int DisplayOrder { get { return _displayOrder; } set { _displayOrder = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AdvertClickEarnings)]
        public Money AdvertClickEarnings { get { return _advertClickEarnings; } set { _advertClickEarnings = value; SetUpToDateAsFalse(); } }

        [Column(Columns.DirectReferralAdvertClickEarnings)]
        public Money DirectReferralAdvertClickEarnings { get { return _directReferralAdvertClickEarnings; } set { _directReferralAdvertClickEarnings = value; SetUpToDateAsFalse(); } }

        [Column(Columns.RentedReferralAdvertClickEarnings)]
        public Money RentedReferralAdvertClickEarnings { get { return _rentedReferralAdvertClickEarnings; } set { _rentedReferralAdvertClickEarnings = value; SetUpToDateAsFalse(); } }

        [Column(Columns.DirectReferralsLimit)]
        public int DirectReferralsLimit { get { return _directReferralsLimit; } set { _directReferralsLimit = value; SetUpToDateAsFalse(); } }

        [Column(Columns.RentedReferralsLimit)]
        public int RentedReferralsLimit { get { return _rentedReferralsLimit; } set { _rentedReferralsLimit = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ReferralRentCost)]
        public Money ReferralRentCost { get { return _referralRentCost; } set { _referralRentCost = value; SetUpToDateAsFalse(); } }

        [Column(Columns.RenewalDiscount)]
        public int RenewalDiscount { get { return _renewalDiscount; } set { _renewalDiscount = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AdvertPointsEarnings)]
        public int AdvertPointsEarnings { get { return _advertPointsEarnings; } set { _advertPointsEarnings = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Color)]
        public string Color { get { return _color; } set { _color = value; SetUpToDateAsFalse(); } }

        [Column(Columns.RentedReferralRecycleCost)]
        public Money RentedReferralRecycleCost { get { return _rentedReferralRecycleCost; } set { _rentedReferralRecycleCost = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CanAutoPay)]
        public bool CanAutoPay { get { return _canAutoPay; } set { _canAutoPay = value; SetUpToDateAsFalse(); } }

        [Column(Columns.DailyAutoPayCost)]
        public Money DailyAutoPayCost { get { return _dailyAutoPayCost; } set { _dailyAutoPayCost = value; SetUpToDateAsFalse(); } }

        [Column(Columns.MinRentingIntervalSecs)]
        protected int MinRentingIntervalSecs { get { return _minRentingIntervalSecs; } set { _minRentingIntervalSecs = value; SetUpToDateAsFalse(); } }

        [Column(Columns.TrafficGridTrials)]
        public int TrafficGridTrials { get { return _TrafficGridTrials; } set { _TrafficGridTrials = value; SetUpToDateAsFalse(); } }

        [Column(Columns.TrafficGridChances)]
        public int TrafficGridChances { get { return _TrafficGridChances; } set { _TrafficGridChances = value; SetUpToDateAsFalse(); } }


        [Column(Columns.TrafficGridShorterAd)]
        public int TrafficGridShorterAd { get { return _TrafficGridShorterAd; } set { _TrafficGridShorterAd = value; SetUpToDateAsFalse(); } }

        [Obsolete]
        [Column(Columns.DirectReferralAdPackPurchaseEarnings)]
        public decimal DirectReferralAdPackPurchaseEarnings { get { return _DirectReferralAdPackPurchaseEarnings; } set { _DirectReferralAdPackPurchaseEarnings = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AdPackAdBalanceReturnPercentage)]
        public int AdPackAdBalanceReturnPercentage { get { return _AdPackAdBalanceReturnPercentage; } set { _AdPackAdBalanceReturnPercentage = value; SetUpToDateAsFalse(); } }

        [Column(Columns.MaxExtraAdPackSecondsForClicks)]
        public int MaxExtraAdPackSecondsForClicks { get { return _MaxExtraAdPackSecondsForClicks; } set { _MaxExtraAdPackSecondsForClicks = value; SetUpToDateAsFalse(); } }

        [Column("OfferwallsProfitPercent")]
        public int OfferwallsProfitPercent { get { return _OfferwallsProfitPercent; } set { _OfferwallsProfitPercent = value; SetUpToDateAsFalse(); } }

        [Column("CashoutLimit")]
        public Money CashoutLimit { get { return _CashoutLimit; } set { _CashoutLimit = value; SetUpToDateAsFalse(); } }

        [Column("MaxDailyCashout")]
        public Money MaxDailyCashout { get { return _MaxDailyCashout; } set { _MaxDailyCashout = value; SetUpToDateAsFalse(); } }

        [Column("MaxGlobalCashout")]
        public Money MaxGlobalCashout { get { return _MaxGlobalCashout; } set { _MaxGlobalCashout = value; SetUpToDateAsFalse(); } }

        [Column("CashoutLimitIcreased")]
        public Money CashoutLimitIcreased { get { return _CashoutLimitIcreased; } set { _CashoutLimitIcreased = value; SetUpToDateAsFalse(); } }

        [Column("CPAProfitPercent")]
        public int CPAProfitPercent { get { return _CPAProfitPercent; } set { _CPAProfitPercent = value; SetUpToDateAsFalse(); } }

        [Obsolete]
        [Column("RefPercentEarningsOfferwalls1")]
        public int RefPercentEarningsOfferwalls { get { return _RefPercentEarningsOfferwalls; } set { _RefPercentEarningsOfferwalls = value; SetUpToDateAsFalse(); } }


        //2502
        [Column("MaxRefPackageCount")]
        public int MaxRefPackageCount { get { return _MaxRefPackageCount; } set { _MaxRefPackageCount = value; SetUpToDateAsFalse(); } }

        [Obsolete]
        [Column("RefPercentEarningsCPA")]
        public int RefPercentEarningsCPA { get { return _RefPercentEarningsCPA; } set { _RefPercentEarningsCPA = value; SetUpToDateAsFalse(); } }

        [Column("HasInstantPayout")]
        public bool HasInstantPayout { get { return _HasInstantPayout; } set { _HasInstantPayout = value; SetUpToDateAsFalse(); } }

        [Column("AdPackDailyRequiredClicks")]
        public int AdPackDailyRequiredClicks { get { return _AdPackDailyRequiredClicks; } set { _AdPackDailyRequiredClicks = value; SetUpToDateAsFalse(); } }

        [Column("ROIEnlargedByPercentage")]
        public int ROIEnlargedByPercentage { get { return _ROIEnlargedByPercentage; } set { _ROIEnlargedByPercentage = value; SetUpToDateAsFalse(); } }

        #region Fee Percentages
        [Column("SameUserCommissionToMainTransferFee")]
        public int SameUserCommissionToMainTransferFee { get { return _SameUserCommissionToMainTransferFee; } set { _SameUserCommissionToMainTransferFee = value; SetUpToDateAsFalse(); } }

        [Obsolete]
        [Column("OtherUserMainToCommisionTransferFee")]
        public int OtherUserMainToCommisionTransferFee { get { return _OtherUserMainToCommisionTransferFee; } set { _OtherUserMainToCommisionTransferFee = value; SetUpToDateAsFalse(); } }

        [Column("OtherUserMainToAdTransferFee")]
        public int OtherUserMainToAdTransferFee { get { return _OtherUserMainToAdTransferFee; } set { _OtherUserMainToAdTransferFee = value; SetUpToDateAsFalse(); } }

        [Column("OtherUserMainToMainTransferFee")]
        public int OtherUserMainToMainTransferFee { get { return _OtherUserMainToMainTransferFee; } set { _OtherUserMainToMainTransferFee = value; SetUpToDateAsFalse(); } }

        [Column("OtherUserPointsToPointsTransferFee")]
        public int OtherUserPointsToPointsTransferFee { get { return _OtherUserPointsToPointsTransferFee; } set { _OtherUserPointsToPointsTransferFee = value; SetUpToDateAsFalse(); } }
        #endregion

        [Column("PTCCreditsPerView")]
        public decimal PTCCreditsPerView { get { return _PTCCreditsPerView; } set { _PTCCreditsPerView = value; SetUpToDateAsFalse(); } }

        [Column("PTCPurchaseCommissionPercent")]
        public int PTCPurchaseCommissionPercent { get { return _PTCPurchaseCommissionPercent; } set { _PTCPurchaseCommissionPercent = value; SetUpToDateAsFalse(); } }

        [Column("PointsYourPTCAdBeingViewed")]
        public int PointsYourPTCAdBeingViewed { get { return _PointsYourPTCAdBeingViewed; } set { _PointsYourPTCAdBeingViewed = value; SetUpToDateAsFalse(); } }

        [Column("PointsPer1000viewsDeliveredToPoolRotator")]
        public int PointsPer1000viewsDeliveredToPoolRotator { get { return _PointsPer1000viewsDeliveredToPoolRotator; } set { _PointsPer1000viewsDeliveredToPoolRotator = value; SetUpToDateAsFalse(); } }

        [Column("PointsPerNewReferral")]
        public int PointsPerNewReferral { get { return _PointsPerNewReferral; } set { _PointsPerNewReferral = value; SetUpToDateAsFalse(); } }

        [Column("AutosurfViewLimitMonth")]
        public int AutosurfViewLimitMonth { get { return _AutosurfViewLimitMonth; } set { _AutosurfViewLimitMonth = value; SetUpToDateAsFalse(); } }

        [Column("MinAdsWatchedMonthlyToKeepYourLevel")]
        public int MinAdsWatchedMonthlyToKeepYourLevel { get { return _MinAdsWatchedMonthlyToKeepYourLevel; } set { _MinAdsWatchedMonthlyToKeepYourLevel = value; SetUpToDateAsFalse(); } }

        [Column("MinPointsToHaveThisLevel")]
        public int MinPointsToHaveThisLevel { get { return _MinPointsToHaveThisLevel; } set { _MinPointsToHaveThisLevel = value; SetUpToDateAsFalse(); } }

        [Column("MaxActivePtcCampaignLimit")]
        public int MaxActivePtcCampaignLimit { get { return _MaxActivePtcCampaignLimit; } set { _MaxActivePtcCampaignLimit = value; SetUpToDateAsFalse(); } }

        [Column("MaxUpgradedDirectRefs")]
        public int MaxUpgradedDirectRefs { get { return _MaxUpgradedDirectRefs; } set { _MaxUpgradedDirectRefs = value; SetUpToDateAsFalse(); } }

        [Obsolete]
        [Column(Columns.DirectReferralBannerPurchaseEarnings)]
        public int DirectReferralBannerPurchaseEarnings { get { return _DirectReferralBannerPurchaseEarnings; } set { _DirectReferralBannerPurchaseEarnings = value; SetUpToDateAsFalse(); } }

        [Obsolete]
        [Column(Columns.DirectReferralMembershipPurchaseEarnings)]
        public int DirectReferralMembershipPurchaseEarnings { get { return _DirectReferralMembershipPurchaseEarnings; } set { _DirectReferralMembershipPurchaseEarnings = value; SetUpToDateAsFalse(); } }

        [Column(Columns.DirectReferralAdvertClickEarningsPoints)]
        public int DirectReferralAdvertClickEarningsPoints { get { return _DirectReferralAdvertClickEarningsPoints; } set { _DirectReferralAdvertClickEarningsPoints = value; SetUpToDateAsFalse(); } }

        [Obsolete]
        [Column(Columns.DirectReferralTrafficGridPurchaseEarnings)]
        public int DirectReferralTrafficGridPurchaseEarnings { get { return _DirectReferralTrafficGridPurchaseEarnings; } set { _DirectReferralTrafficGridPurchaseEarnings = value; SetUpToDateAsFalse(); } }

        [Column(Columns.TrafficExchangeClickEarnings)]
        public Money TrafficExchangeClickEarnings { get { return _TrafficExchangeClickEarnings; } set { _TrafficExchangeClickEarnings = value; SetUpToDateAsFalse(); } }

        [Column("DRTrafficExchangeClickEarnings")]
        public Money DRTrafficExchangeClickEarnings { get { return _DRTrafficExchangeClickEarnings; } set { _DRTrafficExchangeClickEarnings = value; SetUpToDateAsFalse(); } }

        [Column("MaxDailyPtcClicks")]
        public int MaxDailyPtcClicks { get { return _MaxDailyPtcClicks; } set { _MaxDailyPtcClicks = value; SetUpToDateAsFalse(); } }

        [Column("MaxDailyPayouts")]
        public int MaxDailyPayouts { get { return _MaxDailyPayouts; } set { _MaxDailyPayouts = value; SetUpToDateAsFalse(); } }

        [Column("PublishersBannerClickProfitPercentage")]
        public decimal PublishersBannerClickProfitPercentage
        {
            get { return _PublishersBannerClickProfitPercentage; }
            set
            {
                if (value < 0)
                    throw new MsgException("Banner click profit have to be bigger than 0");

                _PublishersBannerClickProfitPercentage = value;
                SetUpToDateAsFalse();
            }
        }

        [Column("PublishersCpaOfferProfitPercentage")]
        public decimal PublishersCpaOfferProfitPercentage { get { return _PublishersCpaOfferProfitPercentage; } set { _PublishersCpaOfferProfitPercentage = value; SetUpToDateAsFalse(); } }

        [Column("PublishersInTextAdClickProfitPercentage")]
        public decimal PublishersInTextAdClickProfitPercentage { get { return _PublishersInTextAdClickProfitPercentage; } set { _PublishersInTextAdClickProfitPercentage = value; SetUpToDateAsFalse(); } }

        [Column("PublishersPtcOfferWallProfitPercentage")]
        public decimal PublishersPtcOfferWallProfitPercentage { get { return _PublishersPtcOfferWallProfitPercentage; } set { _PublishersPtcOfferWallProfitPercentage = value; SetUpToDateAsFalse(); } }

        [Column("ExposureMiniClickEarnings")]
        public Money ExposureMiniClickEarnings  { get { return _ExposureMiniClickEarnings; } set { _ExposureMiniClickEarnings = value; SetUpToDateAsFalse(); } }

        [Column("ExposureMiniDirectClickEarnings")]
        public Money ExposureMiniDirectClickEarnings  { get { return _ExposureMiniDirectClickEarnings; } set { _ExposureMiniDirectClickEarnings = value; SetUpToDateAsFalse(); } }

        [Column("ExposureMiniRentedClickEarnings")]
        public Money ExposureMiniRentedClickEarnings  { get { return _ExposureMiniRentedClickEarnings; } set { _ExposureMiniRentedClickEarnings = value; SetUpToDateAsFalse(); } }

        [Column("ExposureMicroClickEarnings")]
        public Money ExposureMicroClickEarnings  { get { return _ExposureMicroClickEarnings; } set { _ExposureMicroClickEarnings = value; SetUpToDateAsFalse(); } }

        [Column("ExposureMicroDirectClickEarnings")]
        public Money ExposureMicroDirectClickEarnings  { get { return _ExposureMicroDirectClickEarnings; } set { _ExposureMicroDirectClickEarnings = value; SetUpToDateAsFalse(); } }

        [Column("ExposureMicroRentedClickEarnings")]
        public Money ExposureMicroRentedClickEarnings  { get { return _ExposureMicroRentedClickEarnings; } set { _ExposureMicroRentedClickEarnings = value; SetUpToDateAsFalse(); } }

        [Column("ExposureFixedClickEarnings")]
        public Money ExposureFixedClickEarnings  { get { return _ExposureFixedClickEarnings; } set { _ExposureFixedClickEarnings = value; SetUpToDateAsFalse(); } }

        [Column("ExposureFixedDirectClickEarnings")]
        public Money ExposureFixedDirectClickEarnings  { get { return _ExposureFixedDirectClickEarnings; } set { _ExposureFixedDirectClickEarnings = value; SetUpToDateAsFalse(); } }

        [Column("ExposureFixedRentedClickEarnings")]
        public Money ExposureFixedRentedClickEarnings  { get { return _ExposureFixedRentedClickEarnings; } set { _ExposureFixedRentedClickEarnings = value; SetUpToDateAsFalse(); } }

        [Column("ExposureStandardClickEarnings")]
        public Money ExposureStandardClickEarnings  { get { return _ExposureStandardClickEarnings; } set { _ExposureStandardClickEarnings = value; SetUpToDateAsFalse(); } }

        [Column("ExposureStandardDirectClickEarnings")]
        public Money ExposureStandardDirectClickEarnings  { get { return _ExposureStandardDirectClickEarnings; } set { _ExposureStandardDirectClickEarnings = value; SetUpToDateAsFalse(); } }

        [Column("ExposureStandardRentedClickEarnings")]
        public Money ExposureStandardRentedClickEarnings  { get { return _ExposureStandardRentedClickEarnings; } set { _ExposureStandardRentedClickEarnings = value; SetUpToDateAsFalse(); } }

        [Column("ExposureExtendedClickEarnings")]
        public Money ExposureExtendedClickEarnings  { get { return _ExposureExtendedClickEarnings; } set { _ExposureExtendedClickEarnings = value; SetUpToDateAsFalse(); } }

        [Column("ExposureExtendedDirectClickEarnings")]
        public Money ExposureExtendedDirectClickEarnings  { get { return _ExposureExtendedDirectClickEarnings; } set { _ExposureExtendedDirectClickEarnings = value; SetUpToDateAsFalse(); } }

        [Column("ExposureExtendedRentedClickEarnings")]
        public Money ExposureExtendedRentedClickEarnings  { get { return _ExposureExtendedRentedClickEarnings; } set { _ExposureExtendedRentedClickEarnings = value; SetUpToDateAsFalse(); } }

        [Column("MaxFacebookLikesPerDay")]
        public int MaxFacebookLikesPerDay { get { return _MaxFacebookLikesPerDay; } set { _MaxFacebookLikesPerDay = value; SetUpToDateAsFalse(); } }

        [Column("NewReferralReward")]
        public Money NewReferralReward { get { return _NewReferralReward; } set { _NewReferralReward = value; SetUpToDateAsFalse(); } }

        [Column("MinReferralEarningsToCreditReward")]
        public Money MinReferralEarningsToCreditReward { get { return _MinReferralEarningsToCreditReward; } set { _MinReferralEarningsToCreditReward = value; SetUpToDateAsFalse(); } }

        [Column("MaxWithdrawalAllowedPerInvestmentPercent")]
        public decimal MaxWithdrawalAllowedPerInvestmentPercent { get { return _MaxWithdrawalAllowedPerInvestmentPercent; } set { _MaxWithdrawalAllowedPerInvestmentPercent = value; SetUpToDateAsFalse(); } }

        [Column("MaxCommissionPayoutsPerDay")]
        public int MaxCommissionPayoutsPerDay { get { return _MaxCommissionPayoutsPerDay; } set { _MaxCommissionPayoutsPerDay = value; SetUpToDateAsFalse(); } }

        [Column("CanUseRefTools")]
        public bool CanUseRefTools { get { return _CanUseRefTools; } set { _CanUseRefTools = value; SetUpToDateAsFalse(); } }
        
        [Column(Columns.InvestmentPlatformMinAmountToCredited)]
        public Money InvestmentPlatformMinAmountToCredited { get { return _InvestmentPlatformMinAmountToCredited; } set { _InvestmentPlatformMinAmountToCredited = value; SetUpToDateAsFalse(); } }

        [Column("MaxCreditLineRequest")]
        public Money MaxCreditLineRequest { get { return _MaxCreditLineRequest; } set { _MaxCreditLineRequest = value; SetUpToDateAsFalse(); } }

        [Column(Columns.MiniVideoUploadPrice)]
        public Money MiniVideoUploadPrice { get { return _MiniVideoUploadPrice; } set { _MiniVideoUploadPrice = value; SetUpToDateAsFalse(); } }

        [Column(Columns.MiniVideoWatchPrice)]
        public Money MiniVideoWatchPrice { get { return _MiniVideoWatchPrice; } set { _MiniVideoWatchPrice = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AdPacksAdsPointsReward)]
        public int AdPacksAdsPointsReward { get { return _AdPacksAdsPointsReward; } set { _AdPacksAdsPointsReward = value; SetUpToDateAsFalse(); } }

        [Column("ICOPurchasePercent")]
        public decimal ICOPurchasePercent { get { return _ICOPurchasePercent; } set { _ICOPurchasePercent = value; SetUpToDateAsFalse(); } }

        [Column("ArticleCreatorCPM")]
        public Money ArticleCreatorCPM { get { return _ArticleCreatorCPM; } set { _ArticleCreatorCPM = value; SetUpToDateAsFalse(); } }

        [Column("ArticleInfluencerCPM")]
        public Money ArticleInfluencerCPM { get { return _ArticleInfluencerCPM; } set { _ArticleInfluencerCPM = value; SetUpToDateAsFalse(); } }

        [Column("BlockPayoutDays")]
        public int BlockPayoutDays { get { return _BlockPayoutDays; } set { _BlockPayoutDays = value; SetUpToDateAsFalse(); } }

        #region Balance Bonuses For Upgrade
        [Column(Columns.AdBalanceBonusForUpgrade)]
        public Money AdBalanceBonusForUpgrade { get { return _AdBalanceBonusForUpgrade; } set { _AdBalanceBonusForUpgrade = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CashBalanceBonusForUpgrade)]
        public Money CashBalanceBonusForUpgrade { get { return _CashBalanceBonusForUpgrade; } set { _CashBalanceBonusForUpgrade = value; SetUpToDateAsFalse(); } }

        [Column(Columns.TrafficBalanceBonusForUpgrade)]
        public Money TrafficBalanceBonusForUpgrade { get { return _TrafficBalanceBonusForUpgrade; } set { _TrafficBalanceBonusForUpgrade = value; SetUpToDateAsFalse(); } }
        #endregion

        #region Levels

        [Column("LevelChanceToWinAnyAward")]
        public int LevelChanceToWinAnyAward { get { return _LevelChanceToWinAnyAward; } set { _LevelChanceToWinAnyAward = value; SetUpToDateAsFalse(); } }

        [Column("LevelPointsPrizeChance")]
        public int LevelPointsPrizeChance { get { return _LevelPointsPrizeChance; } set { _LevelPointsPrizeChance = value; SetUpToDateAsFalse(); } }

        [Column("LevelPointsPrizeMin")]
        public int LevelPointsPrizeMin { get { return _LevelPointsPrizeMin; } set { _LevelPointsPrizeMin = value; SetUpToDateAsFalse(); } }

        [Column("LevelPointsPrizeMax")]
        public int LevelPointsPrizeMax { get { return _LevelPointsPrizeMax; } set { _LevelPointsPrizeMax = value; SetUpToDateAsFalse(); } }

        [Column("LevelAdCreditsChance")]
        public int LevelAdCreditsChance { get { return _LevelAdCreditsChance; } set { _LevelAdCreditsChance = value; SetUpToDateAsFalse(); } }

        [Column("LevelAdCreditsMin")]
        public int LevelAdCreditsMin { get { return _LevelAdCreditsMin; } set { _LevelAdCreditsMin = value; SetUpToDateAsFalse(); } }

        [Column("LevelAdCreditsMax")]
        public int LevelAdCreditsMax { get { return _LevelAdCreditsMax; } set { _LevelAdCreditsMax = value; SetUpToDateAsFalse(); } }

        [Column("LevelDRLimitIncreasedChance")]
        public int LevelDRLimitIncreasedChance { get { return _LevelDRLimitIncreasedChance; } set { _LevelDRLimitIncreasedChance = value; SetUpToDateAsFalse(); } }

        [Column("LevelDRLimitIncreasedMin")]
        public int LevelDRLimitIncreasedMin { get { return _LevelDRLimitIncreasedMin; } set { _LevelDRLimitIncreasedMin = value; SetUpToDateAsFalse(); } }

        [Column("LevelDRLimitIncreasedMax")]
        public int LevelDRLimitIncreasedMax { get { return _LevelDRLimitIncreasedMax; } set { _LevelDRLimitIncreasedMax = value; SetUpToDateAsFalse(); } }

        #endregion

        private int _id, _membershipStatus, _displayOrder, _directReferralsLimit, _rentedReferralsLimit, _renewalDiscount,
            _advertPointsEarnings, _minRentingIntervalSecs, _TrafficGridTrials, _TrafficGridChances, _TrafficGridShorterAd,
            _OfferwallsProfitPercent, _CPAProfitPercent, _RefPercentEarningsOfferwalls, _MaxRefPackageCount, _RefPercentEarningsCPA,
             _AdPackAdBalanceReturnPercentage, _AdPackDailyRequiredClicks, _ROIEnlargedByPercentage,
            _SameUserCommissionToMainTransferFee, _OtherUserMainToCommisionTransferFee, _OtherUserMainToMainTransferFee,
            _OtherUserPointsToPointsTransferFee, _OtherUserMainToAdTransferFee, _PTCPurchaseCommissionPercent, _PointsYourPTCAdBeingViewed,
            _MinPointsToHaveThisLevel, _MinAdsWatchedMonthlyToKeepYourLevel, _PointsPer1000viewsDeliveredToPoolRotator, _PointsPerNewReferral,
            _AutosurfViewLimitMonth, _MaxActivePtcCampaignLimit, _MaxExtraAdPackSecondsForClicks,
            _LevelChanceToWinAnyAward, _LevelPointsPrizeChance, _LevelPointsPrizeMin, _LevelPointsPrizeMax, _LevelAdCreditsChance, _LevelAdCreditsMin,
            _LevelAdCreditsMax, _LevelDRLimitIncreasedChance, _LevelDRLimitIncreasedMin, _LevelDRLimitIncreasedMax, _MaxUpgradedDirectRefs,
            _DirectReferralBannerPurchaseEarnings, _DirectReferralMembershipPurchaseEarnings, _DirectReferralAdvertClickEarningsPoints,
            _DirectReferralTrafficGridPurchaseEarnings, _MaxDailyPtcClicks, _MaxDailyPayouts, _MaxFacebookLikesPerDay,
            _MaxCommissionPayoutsPerDay, _AdPacksAdsPointsReward, _BlockPayoutDays;
        private string _name, _color;
        private bool _isActive, _canAutoPay, _HasInstantPayout, _CanUseRefTools;
        private Money _advertClickEarnings, _directReferralAdvertClickEarnings, _rentedReferralAdvertClickEarnings,
            _referralRentCost, _rentedReferralRecycleCost, _dailyAutoPayCost, _CashoutLimitIcreased, _CashoutLimit, _MaxDailyCashout, _MaxGlobalCashout,
            _TrafficExchangeClickEarnings, _DRTrafficExchangeClickEarnings, _ExposureMiniClickEarnings, _ExposureMiniDirectClickEarnings, _ExposureMiniRentedClickEarnings,
            _ExposureMicroClickEarnings, _ExposureMicroDirectClickEarnings, _ExposureMicroRentedClickEarnings, _ExposureFixedClickEarnings, _ExposureFixedDirectClickEarnings,
            _ExposureFixedRentedClickEarnings, _ExposureStandardClickEarnings, _ExposureStandardDirectClickEarnings, _ExposureStandardRentedClickEarnings,
            _ExposureExtendedClickEarnings, _ExposureExtendedDirectClickEarnings, _ExposureExtendedRentedClickEarnings, _NewReferralReward,
            _MinReferralEarningsToCreditReward, _InvestmentPlatformMinAmountToCredited, _MaxCreditLineRequest,
            _TrafficBalanceBonusForUpgrade, _CashBalanceBonusForUpgrade, _AdBalanceBonusForUpgrade, _MiniVideoUploadPrice, _MiniVideoWatchPrice,
            _ArticleCreatorCPM, _ArticleInfluencerCPM;
        decimal _PTCCreditsPerView, _PublishersBannerClickProfitPercentage, _PublishersCpaOfferProfitPercentage, _PublishersInTextAdClickProfitPercentage, 
            _PublishersPtcOfferWallProfitPercentage, _MaxWithdrawalAllowedPerInvestmentPercent, _ICOPurchasePercent;

        decimal _DirectReferralAdPackPurchaseEarnings;

        #endregion

        #region Constructors

        public Membership() : base() { }
        public Membership(int id) : base(id) { }
        public Membership(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion

        #region Helpers

        public MembershipStatus Status
        {
            get { return (MembershipStatus)MembershipStatus; }
            set { MembershipStatus = (int)value; }
        }

        public TimeSpan MinRentingInterval
        {
            get { return TimeSpan.FromSeconds(MinRentingIntervalSecs); }
            set { MinRentingIntervalSecs = (int)value.TotalSeconds; }
        }

        private static readonly int StandardMembershipId = 1;

        /// <summary>
        /// Returns new instance of default membership settings
        /// </summary>
        /// <exception cref="DbException"/>
        public static IMembership Standard { get { return new MembershipNameIdProxy(StandardMembershipId); } }

        /// <exception cref="DbException"/>
        public static ListItem[] ListControlSource
        {
            get { return SelectListControlSource(); }
        }

        /// <exception cref="DbException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static ListItem[] SelectListControlSource()
        {
            var columns = Parser.Columns(Membership.Columns.Id, Membership.Columns.Name, Membership.Columns.DisplayOrder);
            DataTable membershipsTable;
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                membershipsTable = bridge.Instance.Select(Membership.TableName, columns, null);
            }

            var sort = from DataRow membership in membershipsTable.Rows
                       orderby membership.Field<int>(Membership.Columns.DisplayOrder) ascending
                       select new ListItem(membership.Field<string>(Membership.Columns.Name),
                           Convert.ToString(membership.Field<int>(Membership.Columns.Id)));

            return sort.ToArray();

        }


        /// <exception cref="DbException" />
        public static ListItem[] ListItems
        {
            get
            {
                var columns = Parser.Columns(Membership.Columns.Id, Membership.Columns.Name, Membership.Columns.DisplayOrder);
                DataTable membershipsTable;
                using (var bridge = ParserPool.Acquire(Database.Client))
                {
                    membershipsTable = bridge.Instance.Select(Membership.TableName, columns, null);
                }

                var sort = from DataRow membership in membershipsTable.Rows
                           orderby membership.Field<int>(Membership.Columns.DisplayOrder) ascending
                           select new ListItem(membership.Field<string>(Membership.Columns.Name),
                               Convert.ToString(membership.Field<int>(Membership.Columns.Id)));

                return sort.ToArray();
            }
        }

        public static DataTable GetActiveMembershipsDataTables()
        {
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                return bridge.Instance.ExecuteRawCommandToDataTable("SELECT * FROM Memberships WHERE Status = 1");
            }
        }

        public static ListItem[] ActiveAndPausedListItems
        {
            get
            {
                var list = TableHelper.GetListFromRawQuery<Membership>("SELECT * FROM Memberships WHERE Status IN (1, 2)");

                var query = from Membership cat in list
                            orderby cat.DisplayOrder
                            select new ListItem(cat.Name, cat.Id.ToString());

                return query.ToArray();
            }
        }

        public static int GetMembershipId(string MembershipName)
        {
            var result = TableHelper.SelectRows<Membership>(TableHelper.MakeDictionary(Columns.Name, MembershipName));
            return result[0].Id;
        }

        /// <exception cref="DbException"/>
        /// <exception cref="ArgumentException"/>
        public static string SelectName(int membershipId)
        {
            string queryResult = null;

            var whereId = TableHelper.MakeDictionary(Membership.Columns.Id, membershipId);
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                queryResult = bridge.Instance.Select(Membership.TableName, Membership.Columns.Name, whereId) as string;
            }

            if (queryResult == null)
                throw new ArgumentException("Membership with specified Id does not exist");

            return queryResult;
        }

        public static Money GetPricePerDay(int packId)
        {
            var packs = MembershipPack.SelectPacksAssignedToMembership(packId);
            Money result = new Money(2000000000);

            if (packs.Count == 0)
                result = Money.Zero;

            foreach (var pack in packs)
            {
                Money value = pack.Price / pack.TimeDays;
                if (value < result)
                    result = value;
            }

            return result;
        }


        public static List<Membership> GetAll()
        {
            return TableHelper.GetListFromRawQuery<Membership>("SELECT * FROM Memberships");
        }

        /// <summary>
        /// get querry for list of membershipsId under current memberships (included current membershipId)
        /// </summary>
        public static string GetSqlQuerryForMembershipsIdListUnderCurrentMembership()
        {
            return string.Format(@"SELECT MembershipId FROM [Memberships] WHERE DisplayOrder <=
                                        (SELECT DisplayOrder FROM Memberships WHERE MembershipId = {0})", Member.CurrentInCache.MembershipId);
        }

#endregion

        public static void BuyPack(Member user, MembershipPack pack, PurchaseBalances targetBalance)
        {
            Money packPrice = pack.Price;

            PurchaseOption.ChargeBalance(user, pack.GetPrice(user), PurchaseOption.Features.Upgrade.ToString(), targetBalance, "Upgrade");

            AddPack(user, pack);
        }

        public static void AddPack(Member user, MembershipPack pack, Money paidAmountToValidate = null)
        {
            Money PackPrice = pack.GetPrice(user);

            if (paidAmountToValidate != null && paidAmountToValidate < PackPrice)
                throw new MsgException(string.Format("{0} is not enough to buy the chosen Membership ({1} - {2})", paidAmountToValidate, pack.Membership.Name, PackPrice));

            user.Upgrade(pack);

            //Add history entry
            History.AddUpgradePurchase(user.Name, pack.Membership.Name, PackPrice);

            //Update pack stats
            pack.CopiesBought++;
            pack.Save();

            //Credit Balances Bonuses
            var AdBalanceBonus = pack.Membership.AdBalanceBonusForUpgrade;
            var CashBalanceBonus = pack.Membership.CashBalanceBonusForUpgrade;
            var TrafficBalanceBonus = pack.Membership.TrafficBalanceBonusForUpgrade;

            if (AdBalanceBonus > Money.Zero)
                user.AddToPurchaseBalance(AdBalanceBonus, "Bonus from Upgrade");

            if (AppSettings.Payments.CashBalanceEnabled && CashBalanceBonus > Money.Zero)
                user.AddToCashBalance(CashBalanceBonus, "Bonus from Upgrade");

            if (AppSettings.TitanFeatures.EarnTrafficExchangeEnabled && TrafficBalanceBonus > Money.Zero)
                user.AddToTrafficBalance(TrafficBalanceBonus, "Bonus from Upgrade");

            var moneyLeftForPools = PackPrice;
            
            Titan.UpgradeCrediter Crediter = (Titan.UpgradeCrediter)Titan.CrediterFactory.Acquire(user, Titan.CreditType.Upgrade);
            moneyLeftForPools = Crediter.CreditReferer(PackPrice);           

            //LeadershipSystem
            var list = new List<RestrictionKind>();
            list.Add(RestrictionKind.RequiredMembership);
            LeadershipSystem.CheckSystem(list, user);

            //Pools
            PoolDistributionManager.AddProfit(ProfitSource.Memberships, moneyLeftForPools);
        }
    }
}