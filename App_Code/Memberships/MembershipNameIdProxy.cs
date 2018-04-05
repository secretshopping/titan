
using System;

namespace Prem.PTC.Memberships
{
    /// <summary>
    /// Proxy class for IMembership (Membership class) containing  Name and Id and lazy initializing other properties
    /// </summary>
    public class MembershipNameIdProxy : BaseIdProxyObject<IMembership>, IMembership
    {

        #region Columns
        public Money AdvertClickEarnings
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.AdvertClickEarnings;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.AdvertClickEarnings = value;
            }
        }

        public Money DirectReferralAdvertClickEarnings
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.DirectReferralAdvertClickEarnings;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.DirectReferralAdvertClickEarnings = value;
            }
        }

        public Money MaxDailyCashout
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.MaxDailyCashout;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.MaxDailyCashout = value;
            }
        }

        public Money MaxGlobalCashout
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.MaxGlobalCashout;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.MaxGlobalCashout = value;
            }
        }

        public decimal DirectReferralAdPackPurchaseEarnings
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.DirectReferralAdPackPurchaseEarnings;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.DirectReferralAdPackPurchaseEarnings = value;
            }
        }

        public int DirectReferralsLimit
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.DirectReferralsLimit;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.DirectReferralsLimit = value;
            }
        }

        public int DisplayOrder
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.DisplayOrder;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.DisplayOrder = value;
            }
        }

        public MembershipStatus Status
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.Status;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.Status = value;
            }
        }

        public string Name
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.Name;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.Name = value;
            }
        }

        public Money ReferralRentCost
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.ReferralRentCost;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.ReferralRentCost = value;
            }
        }

        public int RenewalDiscount
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.RenewalDiscount;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.RenewalDiscount = value;
            }
        }

        public Money RentedReferralAdvertClickEarnings
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.RentedReferralAdvertClickEarnings;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.RentedReferralAdvertClickEarnings = value;
            }
        }

        public int RentedReferralsLimit
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.RentedReferralsLimit;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.RentedReferralsLimit = value;
            }
        }

        public int AdvertPointsEarnings
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.AdvertPointsEarnings;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.AdvertPointsEarnings = value;
            }
        }

        public int TrafficGridShorterAd
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.TrafficGridShorterAd;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.TrafficGridShorterAd = value;
            }
        }

        public int TrafficGridChances
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.TrafficGridChances;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.TrafficGridChances = value;
            }
        }

        public int TrafficGridTrials
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.TrafficGridTrials;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.TrafficGridTrials = value;
            }
        }


        public int OfferwallsProfitPercent
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.OfferwallsProfitPercent;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.OfferwallsProfitPercent = value;
            }
        }

        public int CPAProfitPercent
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.CPAProfitPercent;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.CPAProfitPercent = value;
            }
        }
        public int RefPercentEarningsOfferwalls
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.RefPercentEarningsOfferwalls;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.RefPercentEarningsOfferwalls = value;
            }
        }

        public int RefPercentEarningsCPA
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.RefPercentEarningsCPA;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.RefPercentEarningsCPA = value;
            }
        }

        public bool HasInstantPayout
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.HasInstantPayout;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.HasInstantPayout = value;
            }
        }


        public int MaxRefPackageCount
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.MaxRefPackageCount;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.MaxRefPackageCount = value;
            }
        }


        public Money CashoutLimitIcreased
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.CashoutLimitIcreased;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.CashoutLimitIcreased = value;
            }
        }

        public Money CashoutLimit
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.CashoutLimit;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.CashoutLimit = value;
            }
        }

        public string Color
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.Color;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.Color = value;
            }
        }

        public Money RentedReferralRecycleCost
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.RentedReferralRecycleCost;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.RentedReferralRecycleCost = value;
            }
        }

        public bool CanAutoPay
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.CanAutoPay;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.CanAutoPay = value;
            }
        }

        public Money DailyAutoPayCost
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.DailyAutoPayCost;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.DailyAutoPayCost = value;
            }
        }

        public int ROIEnlargedByPercentage
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.ROIEnlargedByPercentage;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.ROIEnlargedByPercentage = value;
            }
        }

        public int AdPackDailyRequiredClicks
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.AdPackDailyRequiredClicks;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.AdPackDailyRequiredClicks = value;
            }
        }

        public int AdPackAdBalanceReturnPercentage
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.AdPackAdBalanceReturnPercentage;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.AdPackAdBalanceReturnPercentage = value;
            }
        }
        public System.TimeSpan MinRentingInterval
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.MinRentingInterval;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.MinRentingInterval = value;
            }
        }

        #region Transfer Fees
        public int SameUserCommissionToMainTransferFee
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.SameUserCommissionToMainTransferFee;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.SameUserCommissionToMainTransferFee = value;
            }
        }

        public int OtherUserMainToAdTransferFee
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.OtherUserMainToAdTransferFee;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.OtherUserMainToAdTransferFee = value;
            }
        }

        public int OtherUserMainToMainTransferFee
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.OtherUserMainToMainTransferFee;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.OtherUserMainToMainTransferFee = value;
            }
        }

        public int OtherUserPointsToPointsTransferFee
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.OtherUserPointsToPointsTransferFee;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.OtherUserPointsToPointsTransferFee = value;
            }
        }

        #endregion

        public decimal PTCCreditsPerView
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.PTCCreditsPerView;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.PTCCreditsPerView = value;
            }
        }

        public int PTCPurchaseCommissionPercent
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.PTCPurchaseCommissionPercent;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.PTCPurchaseCommissionPercent = value;
            }
        }

        public int PointsYourPTCAdBeingViewed
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.PointsYourPTCAdBeingViewed;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.PointsYourPTCAdBeingViewed = value;
            }
        }

        //

        public int PointsPer1000viewsDeliveredToPoolRotator
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.PointsPer1000viewsDeliveredToPoolRotator;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.PointsPer1000viewsDeliveredToPoolRotator = value;
            }
        }

        public int PointsPerNewReferral
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.PointsPerNewReferral;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.PointsPerNewReferral = value;
            }
        }

        public int AutosurfViewLimitMonth
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.AutosurfViewLimitMonth;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.AutosurfViewLimitMonth = value;
            }
        }

        public int MinAdsWatchedMonthlyToKeepYourLevel
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.MinAdsWatchedMonthlyToKeepYourLevel;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.MinAdsWatchedMonthlyToKeepYourLevel = value;
            }
        }

        public int MinPointsToHaveThisLevel
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.MinPointsToHaveThisLevel;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.MinPointsToHaveThisLevel = value;
            }
        }
        public int MaxActivePtcCampaignLimit
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.MaxActivePtcCampaignLimit;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.MaxActivePtcCampaignLimit = value;
            }
        }

        public int MaxExtraAdPackSecondsForClicks
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.MaxExtraAdPackSecondsForClicks;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.MaxExtraAdPackSecondsForClicks = value;
            }
        }

        public int LevelChanceToWinAnyAward
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.LevelChanceToWinAnyAward;
            }

            set
            {
                SetInstanceIfNeeded();
                proxyObject.LevelChanceToWinAnyAward = value;
            }
        }

        public int LevelPointsPrizeChance
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.LevelPointsPrizeChance;
            }

            set
            {
                SetInstanceIfNeeded();
                proxyObject.LevelPointsPrizeChance = value;
            }
        }

        public int LevelPointsPrizeMin
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.LevelPointsPrizeMin;
            }

            set
            {
                SetInstanceIfNeeded();
                proxyObject.LevelPointsPrizeMin = value;
            }
        }

        public int LevelPointsPrizeMax
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.LevelPointsPrizeMax;
            }

            set
            {
                SetInstanceIfNeeded();
                proxyObject.LevelPointsPrizeMax = value;
            }
        }

        public int LevelAdCreditsChance
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.LevelAdCreditsChance;
            }

            set
            {
                SetInstanceIfNeeded();
                proxyObject.LevelAdCreditsChance = value;
            }
        }

        public int LevelAdCreditsMin
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.LevelAdCreditsMin;
            }

            set
            {
                SetInstanceIfNeeded();
                proxyObject.LevelAdCreditsMin = value;
            }
        }

        public int LevelAdCreditsMax
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.LevelAdCreditsMax;
            }

            set
            {
                SetInstanceIfNeeded();
                proxyObject.LevelAdCreditsMax = value;
            }
        }

        public int LevelDRLimitIncreasedChance
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.LevelDRLimitIncreasedChance;
            }

            set
            {
                SetInstanceIfNeeded();
                proxyObject.LevelDRLimitIncreasedChance = value;
            }
        }

        public int LevelDRLimitIncreasedMin
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.LevelDRLimitIncreasedMin;
            }

            set
            {
                SetInstanceIfNeeded();
                proxyObject.LevelDRLimitIncreasedMin = value;
            }
        }

        public int LevelDRLimitIncreasedMax
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.LevelDRLimitIncreasedMax;
            }

            set
            {
                SetInstanceIfNeeded();
                proxyObject.LevelDRLimitIncreasedMax = value;
            }
        }

        public int MaxUpgradedDirectRefs
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.MaxUpgradedDirectRefs;
            }

            set
            {
                SetInstanceIfNeeded();
                proxyObject.MaxUpgradedDirectRefs = value;
            }
        }

        public int DirectReferralBannerPurchaseEarnings
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.DirectReferralBannerPurchaseEarnings;
            }

            set
            {
                SetInstanceIfNeeded();
                proxyObject.DirectReferralBannerPurchaseEarnings = value;
            }
        }

        public int DirectReferralMembershipPurchaseEarnings
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.DirectReferralMembershipPurchaseEarnings;
            }

            set
            {
                SetInstanceIfNeeded();
                proxyObject.DirectReferralMembershipPurchaseEarnings = value;
            }
        }

        public int DirectReferralAdvertClickEarningsPoints
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.DirectReferralAdvertClickEarningsPoints;
            }

            set
            {
                SetInstanceIfNeeded();
                proxyObject.DirectReferralAdvertClickEarningsPoints = value;
            }
        }

        public int DirectReferralTrafficGridPurchaseEarnings
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.DirectReferralTrafficGridPurchaseEarnings;
            }

            set
            {
                SetInstanceIfNeeded();
                proxyObject.DirectReferralTrafficGridPurchaseEarnings = value;
            }
        }

        public Money TrafficExchangeClickEarnings
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.TrafficExchangeClickEarnings;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.TrafficExchangeClickEarnings = value;
            }
        }

        public Money DRTrafficExchangeClickEarnings
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.DRTrafficExchangeClickEarnings;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.DRTrafficExchangeClickEarnings = value;
            }
        }

        public int MaxDailyPtcClicks
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.MaxDailyPtcClicks;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.MaxDailyPtcClicks = value;
            }
        }

        public int MaxDailyPayouts
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.MaxDailyPayouts;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.MaxDailyPayouts = value;
            }
        }

        public decimal PublishersBannerClickProfitPercentage
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.PublishersBannerClickProfitPercentage;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.PublishersBannerClickProfitPercentage = value;
            }
        }

        public decimal PublishersCpaOfferProfitPercentage
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.PublishersCpaOfferProfitPercentage;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.PublishersCpaOfferProfitPercentage = value;
            }
        }

        public decimal PublishersInTextAdClickProfitPercentage
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.PublishersInTextAdClickProfitPercentage;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.PublishersInTextAdClickProfitPercentage = value;
            }
        }
        public decimal PublishersPtcOfferWallProfitPercentage
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.PublishersPtcOfferWallProfitPercentage;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.PublishersPtcOfferWallProfitPercentage = value;
            }
        }

        public Money ExposureMiniClickEarnings
        {
            get
            { SetInstanceIfNeeded(); return proxyObject.ExposureMiniClickEarnings; }
            set { SetInstanceIfNeeded(); proxyObject.ExposureMiniClickEarnings = value; }
        }

        public Money ExposureMiniDirectClickEarnings
        {
            get { SetInstanceIfNeeded(); return proxyObject.ExposureMiniDirectClickEarnings; }
            set { SetInstanceIfNeeded(); proxyObject.ExposureMiniDirectClickEarnings = value; }
        }
        public Money ExposureMiniRentedClickEarnings
        {
            get { SetInstanceIfNeeded(); return proxyObject.ExposureMiniRentedClickEarnings; }
            set { SetInstanceIfNeeded(); proxyObject.ExposureMiniRentedClickEarnings = value; }
        }
        public Money ExposureMicroClickEarnings
        {
            get { SetInstanceIfNeeded(); return proxyObject.ExposureMicroClickEarnings; }
            set { SetInstanceIfNeeded(); proxyObject.ExposureMicroClickEarnings = value; }
        }
        public Money ExposureMicroDirectClickEarnings
        {
            get { SetInstanceIfNeeded(); return proxyObject.ExposureMicroDirectClickEarnings; }
            set { SetInstanceIfNeeded(); proxyObject.ExposureMicroDirectClickEarnings = value; }
        }
        public Money ExposureMicroRentedClickEarnings
        {
            get { SetInstanceIfNeeded(); return proxyObject.ExposureMicroRentedClickEarnings; }
            set { SetInstanceIfNeeded(); proxyObject.ExposureMicroRentedClickEarnings = value; }
        }
        public Money ExposureFixedClickEarnings
        {
            get { SetInstanceIfNeeded(); return proxyObject.ExposureFixedClickEarnings; }
            set { SetInstanceIfNeeded(); proxyObject.ExposureFixedClickEarnings = value; }
        }
        public Money ExposureFixedDirectClickEarnings
        {
            get { SetInstanceIfNeeded(); return proxyObject.ExposureFixedDirectClickEarnings; }
            set { SetInstanceIfNeeded(); proxyObject.ExposureFixedDirectClickEarnings = value; }
        }
        public Money ExposureFixedRentedClickEarnings
        {
            get { SetInstanceIfNeeded(); return proxyObject.ExposureFixedRentedClickEarnings; }
            set { SetInstanceIfNeeded(); proxyObject.ExposureFixedRentedClickEarnings = value; }
        }
        public Money ExposureStandardClickEarnings
        {
            get { SetInstanceIfNeeded(); return proxyObject.ExposureStandardClickEarnings; }
            set { SetInstanceIfNeeded(); proxyObject.ExposureStandardClickEarnings = value; }
        }
        public Money ExposureStandardDirectClickEarnings
        {
            get { SetInstanceIfNeeded(); return proxyObject.ExposureStandardDirectClickEarnings; }
            set { SetInstanceIfNeeded(); proxyObject.ExposureStandardDirectClickEarnings = value; }
        }
        public Money ExposureStandardRentedClickEarnings
        {
            get { SetInstanceIfNeeded(); return proxyObject.ExposureStandardRentedClickEarnings; }
            set { SetInstanceIfNeeded(); proxyObject.ExposureStandardRentedClickEarnings = value; }
        }
        public Money ExposureExtendedClickEarnings
        {
            get { SetInstanceIfNeeded(); return proxyObject.ExposureExtendedClickEarnings; }
            set { SetInstanceIfNeeded(); proxyObject.ExposureExtendedClickEarnings = value; }
        }
        public Money ExposureExtendedDirectClickEarnings
        {
            get { SetInstanceIfNeeded(); return proxyObject.ExposureExtendedDirectClickEarnings; }
            set { SetInstanceIfNeeded(); proxyObject.ExposureExtendedDirectClickEarnings = value; }
        }
        public Money ExposureExtendedRentedClickEarnings
        {
            get { SetInstanceIfNeeded(); return proxyObject.ExposureExtendedRentedClickEarnings; }
            set { SetInstanceIfNeeded(); proxyObject.ExposureExtendedRentedClickEarnings = value; }
        }

        public int MaxFacebookLikesPerDay
        {
            get { SetInstanceIfNeeded(); return proxyObject.MaxFacebookLikesPerDay; }
            set { SetInstanceIfNeeded(); proxyObject.MaxFacebookLikesPerDay = value; }
        }
        public Money NewReferralReward
        {
            get { SetInstanceIfNeeded(); return proxyObject.NewReferralReward; }
            set { SetInstanceIfNeeded(); proxyObject.NewReferralReward = value; }
        }

        public Money MinReferralEarningsToCreditReward
        {
            get { SetInstanceIfNeeded(); return proxyObject.MinReferralEarningsToCreditReward; }
            set { SetInstanceIfNeeded(); proxyObject.MinReferralEarningsToCreditReward = value; }
        }

        public decimal MaxWithdrawalAllowedPerInvestmentPercent
        {
            get { SetInstanceIfNeeded(); return proxyObject.MaxWithdrawalAllowedPerInvestmentPercent; }
            set { SetInstanceIfNeeded(); proxyObject.MaxWithdrawalAllowedPerInvestmentPercent = value; }
        }

        public int MaxCommissionPayoutsPerDay
        {
            get { SetInstanceIfNeeded(); return proxyObject.MaxCommissionPayoutsPerDay; }
            set { SetInstanceIfNeeded(); proxyObject.MaxCommissionPayoutsPerDay = value; }
        }

        public bool CanUseRefTools
        {
            get { SetInstanceIfNeeded(); return proxyObject.CanUseRefTools; }
            set { SetInstanceIfNeeded(); proxyObject.CanUseRefTools = value; }
        }

        public Money InvestmentPlatformMinAmountToCredited
        {
            get { SetInstanceIfNeeded(); return proxyObject.InvestmentPlatformMinAmountToCredited; }
            set { SetInstanceIfNeeded(); proxyObject.InvestmentPlatformMinAmountToCredited = value; }
        }

        public Money MaxCreditLineRequest
        {
            get { SetInstanceIfNeeded(); return proxyObject.MaxCreditLineRequest; }
            set { SetInstanceIfNeeded(); proxyObject.MaxCreditLineRequest = value; }
        }

        public Money AdBalanceBonusForUpgrade
        {
            get { SetInstanceIfNeeded(); return proxyObject.AdBalanceBonusForUpgrade; }
            set { SetInstanceIfNeeded(); proxyObject.AdBalanceBonusForUpgrade = value; }
        }

        public Money CashBalanceBonusForUpgrade
        {
            get { SetInstanceIfNeeded(); return proxyObject.CashBalanceBonusForUpgrade; }
            set { SetInstanceIfNeeded(); proxyObject.CashBalanceBonusForUpgrade = value; }
        }

        public Money TrafficBalanceBonusForUpgrade
        {
            get { SetInstanceIfNeeded(); return proxyObject.TrafficBalanceBonusForUpgrade; }
            set { SetInstanceIfNeeded(); proxyObject.TrafficBalanceBonusForUpgrade = value; }
        }

        public Money MiniVideoUploadPrice
        {
            get { SetInstanceIfNeeded(); return proxyObject.MiniVideoUploadPrice; }
            set { SetInstanceIfNeeded(); proxyObject.MiniVideoUploadPrice = value; }
        }

        public Money MiniVideoWatchPrice
        {
            get { SetInstanceIfNeeded(); return proxyObject.MiniVideoWatchPrice; }
            set { SetInstanceIfNeeded(); proxyObject.MiniVideoWatchPrice = value; }
        }

        public int AdPacksAdsPointsReward
        {
            get { SetInstanceIfNeeded(); return proxyObject.AdPacksAdsPointsReward; }
            set { SetInstanceIfNeeded(); proxyObject.AdPacksAdsPointsReward = value; }
        }

        public decimal ICOPurchasePercent
        {
            get { SetInstanceIfNeeded(); return proxyObject.ICOPurchasePercent; }
            set { SetInstanceIfNeeded(); proxyObject.ICOPurchasePercent = value; }
        }

        public Money ArticleCreatorCPM
        {
            get { SetInstanceIfNeeded(); return proxyObject.ArticleCreatorCPM; }
            set { SetInstanceIfNeeded(); proxyObject.ArticleCreatorCPM = value; }
        }

        public Money ArticleInfluencerCPM
        {
            get { SetInstanceIfNeeded(); return proxyObject.ArticleInfluencerCPM; }
            set { SetInstanceIfNeeded(); proxyObject.ArticleInfluencerCPM = value; }
        }

        public int BlockPayoutDays
        {
            get { SetInstanceIfNeeded(); return proxyObject.BlockPayoutDays; }
            set { SetInstanceIfNeeded(); proxyObject.BlockPayoutDays = value; }
        }

        #endregion Columns


        #region Constructors
        public MembershipNameIdProxy(int id) : base(x => new Membership(id), id) { }

        #endregion Constructors
    }
}