using Prem.PTC;
using Prem.PTC.Memberships;
using Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

public class MembershipProperty : BaseTableObject
{
    #region Columns
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "MembershipProperties"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("Name")]
    public string Name { get { return _Name; } set { _Name = value; SetUpToDateAsFalse(); } }

    [Column("Hidden")]
    public bool Hidden { get { return _Hidden; } set { _Hidden = value; SetUpToDateAsFalse(); } }
    #endregion

    int _Id;
    string _Name;
    bool _Hidden;

    public MembershipProperty()
            : base()
    { }

    public MembershipProperty(int id) : base(id) { }

    public MembershipProperty(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
    { }

    public static List<MembershipProperty> GetAll()
    {
        return TableHelper.SelectAllRows<MembershipProperty>();
    }

    public static List<int> moneyRows = new List<int>()
            {
                4,5,6,9,13,15,21,22,29,46,63,64,87,92,93,94,95,96,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85
            };

    public static List<int> percentageRows = new List<int>()
            {
               10,18,19,20,23,24,26,28,30,32,33,37,35,36,39,48,49,52,55,67,69,70,90, 91
            };

    public static List<int> unlimitedTextRows = new List<int>()
            {
               7,90
            };

    public static List<int> pointsRows = new List<int>()
            {
               61
            };

    public static List<int> secondsRows = new List<int>()
            {
               
            };

    public static List<int> trueFalseRows = new List<int>()
            {
               14,27
            };

    public static List<int> secondsToDaysRows = new List<int>()
            {
               16
            };

    public static List<int> daysRows = new List<int>()
            {
                97
            };

    public static string GetResourceLabel(string propName)
    {
        string displayName = string.Empty;
        switch (propName)
        {
            case "AdvertClickEarnings":
                displayName = U6003.PTCCLICKEARNINGS;
                break;
            case "DirectReferralAdvertClickEarnings":
                displayName = U6003.DRPTCCLICKEARNINGS;
                break;
            case "RentedReferralAdvertClickEarnings":
                displayName = U6003.RRPTCCLICKEARNINGS;
                break;
            case "DirectReferralsLimit":
                displayName = L1.DIRECTREFLIMIT;
                break;
            case "RentedReferralsLimit":
                displayName = L1.RENTEDREFERRALLIMIT;
                break;
            case "ReferralRentCost":
                displayName = L1.PRICEPERREF;
                break;
            case "RenewalDiscount":
                displayName = L1.RENEWALDISCOUNT;
                break;
            case "AdvertPointsEarnings":
                displayName = String.Format(L1.ADPOINTSEARNINGS, AppSettings.PointsName);
                break;
            case "Color":
                displayName = L1.COLOR;
                break;
            case "RentedReferralRecycleCost":
                displayName = L1.RECYCLECOST;
                break;
            case "CanAutoPay":
                displayName = L1.AUTOPAY;
                break;
            case "DailyAutoPayCost":
                displayName = L1.AUTOPAYCOST;
                break;
            case "MinRentingIntervalSecs":
                displayName = L1.MINRENTINTERVAL;
                break;
            case "TrafficGridTrials":
                displayName = L1.TGHITS;
                break;
            case "TrafficGridChances":
                displayName = L1.TGCHANCES;
                break;
            case "TrafficGridShorterAd":
                displayName = L1.TGSHORTED;
                break;
            case "OfferwallsProfitPercent":
                displayName = U3000.OFFEARNINGS;
                break;
            case "CashoutLimit":
                displayName = L1.CASHOUTLIMIT;
                break;
            case "CashoutLimitIcreased":
                displayName = L1.CASHOUTLIMITNEXT;
                break;
            case "CPAProfitPercent":
                displayName = L1.CPAPROFIT;
                break;
            case "MaxRefPackageCount":
                displayName = U2502.MAXREFPACKAGE;
                break;
            case "RefPercentEarningsCPA":
                displayName = U3000.CPAREFEARNINGS;
                break;
            case "HasInstantPayout":
                displayName = U4000.INSTANTPAYOUT;
                break;
            case "MaxDailyCashout":
                displayName = U5002.MAXDAILYPAYOUT;
                break;
            case "AdPackAdBalanceReturnPercentage":
                displayName = U6012.ROICREDITEDTOPURCHASEBALANCE;
                break;
            case "AdPackDailyRequiredClicks":
                displayName = String.Format(U5004.DAILYREQIUREDCLICKS, AppSettings.RevShare.AdPack.AdPackName);
                break;
            case "ROIEnlargedByPercentage":
                displayName = String.Format(U5004.ROIINCREASEDBY, AppSettings.RevShare.AdPack.AdPackName);
                break;
            case "SameUserCommissionToMainTransferFee":
                displayName = U5004.TRANSFERCOMMISSIONTOMAINFEE;
                break;
            case "OtherUserMainToAdTransferFee":
                displayName = U6012.RECEIVEPURCHASEBALANCEFROMOTHERSFEE;
                break;
            case "OtherUserMainToMainTransferFee":
                displayName = U5004.RECEIVEMAINBALANCEFROMOTHERSFEE;
                break;
            case "OtherUserPointsToPointsTransferFee":
                displayName = U5004.RECEIVEPOINTSFROMOTHERSFEE.Replace("%n%", AppSettings.PointsName);
                break;
            case "PTCCreditsPerView":
                displayName = U5007.ADCREDITSPERADVIEW;
                break;
            case "PTCPurchaseCommissionPercent":
                displayName = U5007.PTCPURCHASECOMISSION;
                break;
            case "PointsYourPTCAdBeingViewed":
                displayName = string.Format(U5007.POINTSPERYOURADVIEW, AppSettings.PointsName);
                break;
            case "PointsPer1000viewsDeliveredToPoolRotator":
                displayName = string.Format(U5007.POINTSPER1000VIEWSROTATOR, AppSettings.PointsName);
                break;
            case "PointsPerNewReferral":
                displayName = string.Format(U5007.POINTSPERNEWREF, AppSettings.PointsName);
                break;
            case "AutosurfViewLimitMonth":
                displayName = U5007.PTCMONTHLYAUTOSURFLIMIT;
                break;
            case "MinAdsWatchedMonthlyToKeepYourLevel":
                displayName = U5007.MINADSWATCHEDMONTHLY;
                break;
            case "MinPointsToHaveThisLevel":
                displayName = L1.LIMIT + " " + AppSettings.PointsName;
                break;
            case "MaxActivePtcCampaignLimit":
                displayName = U5007.MAXACTIVEADCAMPAINS;
                break;
            case "MaxGlobalCashout":
                displayName = U5007.MAXGLOBALPAYOUT;
                break;
            case "MaxExtraAdPackSecondsForClicks":
                displayName = U5007.MAXADPACKSECSFORCLICKS;
                break;
            case "LevelChanceToWinAnyAward":
                displayName = U5007.CHANCETOWINLEVELAWARD;
                break;
            case "LevelPointsPrizeChance":
                displayName = string.Format(U5007.CHANCETOWINLEVELPOINTS, AppSettings.PointsName);
                break;
            case "LevelPointsPrizeMin":
                displayName = string.Format(U5007.MINLEVELPOINTSAWARD, AppSettings.PointsName);
                break;
            case "LevelPointsPrizeMax":
                displayName = string.Format(U5007.MAXLEVELPOINTSAWARD, AppSettings.PointsName);
                break;
            case "LevelAdCreditsChance":
                displayName = U5007.CHANCETOWINLEVELADCREDITS;
                break;
            case "LevelAdCreditsMin":
                displayName = U5007.MINLEVELADCREDITSAWARD;
                break;
            case "LevelAdCreditsMax":
                displayName = U5007.MAXLEVELADCREDITSAWARD;
                break;
            case "LevelDRLimitIncreasedChance":
                displayName = U5007.CHANCETOWINLEVELDR;
                break;
            case "LevelDRLimitIncreasedMin":
                displayName = U5007.MINLEVELDRAWARD;
                break;
            case "LevelDRLimitIncreasedMax":
                displayName = U5007.MAXLEVELDRAWARD;
                break;
            case "MaxUpgradedDirectRefs":
                displayName = U5009.UPGRADEDDRLIMIT;
                break;
            case "DirectReferralAdvertClickEarningsPoints":
                displayName = L1.DIRECTREFADCLICKEARNINGS;
                break;
            case "TrafficExchangeClickEarnings":
                displayName = U5009.TECLICKEARNINGS;
                break;
            case "DRTrafficExchangeClickEarnings":
                displayName = U5009.DRTECLICKEARNINGS;
                break;
            case "MaxDailyPtcClicks":
                displayName = U6000.MAXPTCCLICKSPERDAY;
                break;
            case "MaxDailyPayouts":
                displayName = U6000.WITHDRAWALSPERDAY;
                break;
            case "PublishersBannerClickProfitPercentage":
                displayName = U6003.PUBLISHERSBANNERCLICKPROFIT;
                break;
            case "PublishersCpaOfferProfitPercentage":
                displayName = U6003.PUBLISHERSCPAOFFERPROFIT;
                break;
            case "PublishersInTextAdClickProfitPercentage":
                displayName = U6003.PUBLISHERSINTEXTADCLICKPROFIT;
                break;
            case "PublishersPtcOfferWallProfitPercentage":
                displayName = U6003.PUBLISHERSPTCOFFERWALLPROFIT;
                break;
            case "ExposureMiniClickEarnings":
                displayName = "Mini " + U6003.PTCCLICKEARNINGS;
                break;
            case "ExposureMiniDirectClickEarnings":
                displayName = "Mini " + U6003.DRPTCCLICKEARNINGS;
                break;
            case "ExposureMiniRentedClickEarnings":
                displayName = "Mini " + U6003.RRPTCCLICKEARNINGS;
                break;
            case "ExposureMicroClickEarnings":
                displayName = "Micro " + U6003.PTCCLICKEARNINGS;
                break;
            case "ExposureMicroDirectClickEarnings":
                displayName = "Micro " + U6003.DRPTCCLICKEARNINGS;
                break;
            case "ExposureMicroRentedClickEarnings":
                displayName = "Micro " + U6003.RRPTCCLICKEARNINGS;
                break;
            case "ExposureFixedClickEarnings":
                displayName = "Fixed " + U6003.PTCCLICKEARNINGS;
                break;
            case "ExposureFixedDirectClickEarnings":
                displayName = "Fixed " + U6003.DRPTCCLICKEARNINGS;
                break;
            case "ExposureFixedRentedClickEarnings":
                displayName = "Fixed " + U6003.RRPTCCLICKEARNINGS;
                break;
            case "ExposureStandardClickEarnings":
                displayName = "Standard " + U6003.PTCCLICKEARNINGS;
                break;
            case "ExposureStandardDirectClickEarnings":
                displayName = "Standard " + U6003.DRPTCCLICKEARNINGS;
                break;
            case "ExposureStandardRentedClickEarnings":
                displayName = "Standard " + U6003.RRPTCCLICKEARNINGS;
                break;
            case "ExposureExtendedClickEarnings":
                displayName = "Extended " + U6003.PTCCLICKEARNINGS;
                break;
            case "ExposureExtendedDirectClickEarnings":
                displayName = "Extended " + U6003.DRPTCCLICKEARNINGS;
                break;
            case "ExposureExtendedRentedClickEarnings":
                displayName = "Extended " + U6003.RRPTCCLICKEARNINGS;
                break;
            case "MaxFacebookLikesPerDay":
                displayName = U6004.FACEBOOKLIKESPERDAY;
                break;
            case "NewReferralReward":
                displayName = U6004.NEWREFREWARD;
                break;
            case "MaxWithdrawalAllowedPerInvestmentPercent":
                displayName = U6005.MAXINVESTMENTALLOWEDTOWITHDRAW;
                break;
            case "MaxCommissionPayoutsPerDay":
                displayName = U6005.MAXCOMMISSIONPAYOUTSPERDAY;
                break;
            case "CanUseRefTools":
                displayName = U6005.CANUSEREFTOOLS;
                break;
            case "MaxCreditLineRequest":
                displayName = U6008.MAXCREDITLINEREQUEST;
                break;
            case "AdBalanceBonusForUpgrade":
                displayName = U6012.PURCHASEBALANCEBONUSFROMUPGRADE;
                break;
            case "CashBalanceBonusForUpgrade":
                displayName = U6008.CASHBALANCEBONUSFROMUPGRADE;
                break;
            case "TrafficBalanceBonusForUpgrade":
                displayName = U6008.TRAFFICBALANCEBONUSFROMUPGRADE;
                break;
            case "MiniVideoUploadPrice":
                displayName = U6008.MINIVIDEOUPLOADPRICE;
                break;
            case "MiniVideoWatchPrice":
                displayName = U6008.MINIVIDEOWATCHPRICE;
                break;
            case "ICOPurchasePercent":
                displayName = U6012.ICOPURCHASECOMMISSION;
                break;
            case "ArticleCreatorCPM":
                displayName = U6012.ARTICLECREATORCPM;
                break;
            case "ArticleInfluencerCPM":
                displayName = U6012.ARTICLEINFLUENCERCPM;
                break;
            case "BlockPayoutDays":
                displayName = U6013.MINIMUMTIMEBETWEENPAYOUT;
                break;
            default:
                displayName = propName;
                break;
        }


        return displayName;
    }

    public static List<string> GetPropsToHideBasedOnTitanFeatures()
    {
        var propsToHide = new List<string>();

        if (!AppSettings.TitanFeatures.AdvertAdsEnabled || !AppSettings.TitanFeatures.EarnAdsEnabled)
            propsToHide.AddRange(ptcProps);

        if (AppSettings.PtcAdverts.ExposureCategoriesEnabled)
            propsToHide.AddRange(ptcNoExposureProps);

        if (!AppSettings.TitanFeatures.AdvertAdsEnabled || !AppSettings.TitanFeatures.EarnAdsEnabled || !AppSettings.PtcAdverts.ExposureCategoriesEnabled)
            propsToHide.AddRange(ptcExposureProps);

        if (!AppSettings.PtcAdverts.PTCCreditsEnabled)
            propsToHide.AddRange(ptcCreditsProps);

        if (!AppSettings.TitanFeatures.ReferralPoolRotatorEnabled)
            propsToHide.AddRange(refPoolRotatorProps);

        if (!AppSettings.Points.PointsEnabled || !AppSettings.Points.LevelMembershipPolicyEnabled)
            propsToHide.AddRange(pointsLevelProps);

        if (!AppSettings.Points.PointsEnabled)
            propsToHide.AddRange(pointsProps);

        if (!AppSettings.TitanFeatures.ReferralsRentedEnabled)
            propsToHide.AddRange(rentedRefProps);

        if (!AppSettings.TitanFeatures.AdvertTrafficGridEnabled || !AppSettings.TitanFeatures.EarnTrafficGridEnabled)
            propsToHide.AddRange(trafficGridProps);

        if (!AppSettings.TitanFeatures.EarnOfferwallsEnabled && !AppSettings.TitanFeatures.EarnCrowdFlowerEnabled)
            propsToHide.AddRange(offerWallProps);

        if (!AppSettings.TitanFeatures.MoneyPayoutEnabled)
            propsToHide.AddRange(moneyPayoutProps);

        if (!AppSettings.TitanFeatures.EarnCPAGPTEnabled)
            propsToHide.AddRange(cpaProps);

        if (!AppSettings.Payments.IsInstantPayout)
            propsToHide.AddRange(instantPayoutProps);

        if (!AppSettings.RevShare.IsRevShareEnabled || !AppSettings.TitanFeatures.AdvertAdPacksEnabled || AppSettings.RevShare.AdPacksPolicy == AppSettings.AdPacksPolicy.HYIP)
            propsToHide.AddRange(revShareProps);

        if (!AppSettings.RevShare.AdPack.IsTimeClickExchangeEnabled)
            propsToHide.AddRange(timeClickExchangeProps);

        if (!AppSettings.Payments.CommissionToMainBalanceEnabled)
            propsToHide.AddRange(commissionBalanceProps);

        if (AppSettings.Payments.TransferMode != TransferFundsMode.AllowMainToPurchaseBalance)
            propsToHide.AddRange(maintoAdBalanceProps);

        if (!(AppSettings.Payments.TransferMode == TransferFundsMode.AllowMainBalanceOnly
               || AppSettings.Payments.TransferMode == TransferFundsMode.AllowPointsAndMainBalance))
            propsToHide.AddRange(mainAndPointAndMainProps);

        if (!(AppSettings.Payments.TransferMode == TransferFundsMode.AllowPointsOnly
                || AppSettings.Payments.TransferMode == TransferFundsMode.AllowPointsAndMainBalance))
            propsToHide.AddRange(pointsAndPointAndMainProps);

        if (!AppSettings.Misc.SpilloverEnabled)
            propsToHide.AddRange(spilloverProps);

        if (!AppSettings.TitanFeatures.EarnTrafficExchangeEnabled)
            propsToHide.AddRange(trafficExchangeProps);

        if (!(AppSettings.TitanFeatures.PublishersRoleEnabled &&
            (AppSettings.TitanFeatures.PublishBannersEnabled || AppSettings.TitanFeatures.PublishOfferWallsEnabled ||
            AppSettings.TitanFeatures.PublishInTextAdsEnabled || AppSettings.TitanFeatures.PublishPTCOfferWallsEnabled)))
            propsToHide.AddRange(publishersEarningProps);

        if (!AppSettings.TitanFeatures.PublishBannersEnabled)
            propsToHide.AddRange(publishersBannerClickProfitProps);

        if (!AppSettings.TitanFeatures.PublishOfferWallsEnabled)
            propsToHide.AddRange(publishersCpaOfferProfitProps);

        if (!AppSettings.TitanFeatures.PublishInTextAdsEnabled)
            propsToHide.AddRange(publishersInTextAdClickProfitProps);

        if (!AppSettings.TitanFeatures.PublishPTCOfferWallsEnabled)
            propsToHide.AddRange(publishersPtcOfferWallProfitProps);

        if (!AppSettings.TitanFeatures.EarnLikesEnabled)
            propsToHide.AddRange(fbProps);

        if (!AppSettings.Payments.CommissionBalanceWithdrawalEnabled)
            propsToHide.AddRange(commissionBalanceWithdrawProps);

        if (!AppSettings.TitanFeatures.MoneyCreditLineEnabled)
            propsToHide.Add("MaxCreditLineRequest");

        if (!AppSettings.Payments.CashBalanceEnabled)
            propsToHide.Add("CashBalanceBonusForUpgrade");

        if (!AppSettings.TitanFeatures.EarnTrafficExchangeEnabled)
            propsToHide.Add("TrafficBalanceBonusForUpgrade");

        if (!AppSettings.TitanFeatures.AdvertMiniVideoEnabled)
            propsToHide.Add("MiniVideoUploadPrice");

        if (!AppSettings.TitanFeatures.EntertainmentMiniVideoEnabled)
            propsToHide.Add("MiniVideoWatchPrice");

        if (!AppSettings.TitanFeatures.ICOBuyEnabled)
            propsToHide.Add("ICOPurchasePercent");

        if (!AppSettings.TitanModules.HasProduct(8))
            propsToHide.AddRange(titanNewsProps);

        return propsToHide;
    }

    public static List<string> GetPropsToHideForAdmin()
    {
        var hiddenProperties = GetPropsToHideBasedOnTitanFeatures();
        hiddenProperties.AddRange(irrelevantProps);
        hiddenProperties.AddRange(obsoleteProps);
        return hiddenProperties;
    }

    public static List<string> GetPropsToHideForClient()
    {
        var hiddenProperties = GetAll().Where(p => p.Hidden == true).Select(p => p.Name).ToList();
        hiddenProperties.AddRange(GetPropsToHideBasedOnTitanFeatures());
        hiddenProperties.AddRange(irrelevantProps);
        hiddenProperties.AddRange(obsoleteProps);
        return hiddenProperties;
    }

    public static string Format(int rowIndex, string value, bool formatCheckboxes = true)
    {
        //Add text "unlimited" for unlimited values
        if (unlimitedTextRows.Contains(rowIndex))
        {
            try
            {
                decimal limit = Convert.ToDecimal(value.Replace("%", ""));
                if (limit >= 1000000000)
                    return U5001.UNLIMITED;
            }
            catch (FormatException ex) { }
        }

        //Parse to money
        if (moneyRows.Contains(rowIndex))
            return Money.Parse(value).ToString();

        //Add percentage icon
        if (percentageRows.Contains(rowIndex))
            return NumberUtils.FormatPercents(value);

        //Add Points name
        if (pointsRows.Contains(rowIndex))
            return String.Format("{0} {1}", value, AppSettings.PointsName);

        //True-False check
        if (trueFalseRows.Contains(rowIndex))
        {
            var boolValue = Boolean.Parse(value);
            if (boolValue)
                return formatCheckboxes ? HtmlCreator.GetCheckboxCheckedImage() : boolValue.ToString().ToLower();
            else
                return formatCheckboxes ? HtmlCreator.GetCheckboxUncheckedImage() : boolValue.ToString().ToLower();
        }

        //Add seconds
        if (secondsRows.Contains(rowIndex))
            return String.Format("{0} {1}", value, L1.SECONDS);

        //Seconds to days
        if (secondsToDaysRows.Contains(rowIndex))
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(Convert.ToInt32(value));
            return timeSpan.TotalDays.ToString() + " " + L1.DAYS;
        }

        //Add days
        if (daysRows.Contains(rowIndex))
            return string.Format("{0} day(s)", value);

        return value;
    }

    private static List<string> obsoleteProps = new List<string>()
    {
        "RefPercentEarningsOfferwalls1",
        "DirectReferralAdPackPurchaseEarnings",
        "OtherUserMainToCommisionTransferFee",
        "DirectReferralBannerPurchaseEarnings",
        "DirectReferralMembershipPurchaseEarnings",
        "DirectReferralTrafficGridPurchaseEarnings",
        "ROIEnlargedByPercentage",
        "AdPackAdBalanceReturnPercentage",
        "MinReferralEarningsToCreditReward"
    };

    private static List<string> irrelevantProps = new List<string>()
    {
        "MembershipId",
        "Status",
        "DisplayOrder",
    };

    #region Titan Features Properties
    private static List<string> ptcProps = new List<string>()
    {
        "AdvertClickEarnings",
        "DirectReferralAdvertClickEarnings",
        "RentedReferralAdvertClickEarnings",
        "AdvertPointsEarnings",
        "PTCCreditsPerView",
        "PTCPurchaseCommissionPercent",
        "PointsYourPTCAdBeingViewed",
        "AutosurfViewLimitMonth",
        "MaxActivePtcCampaignLimit",
        "DirectReferralAdvertClickEarningsPoints",
        "MaxDailyPtcClicks"
    };

    private static List<string> ptcNoExposureProps = new List<string>()
    {
        "AdvertClickEarnings",
         "DirectReferralAdvertClickEarnings",
        "RentedReferralAdvertClickEarnings",
    };

    private static List<string> ptcExposureProps = new List<string>()
    {
        "ExposureMiniClickEarnings",
        "ExposureMiniDirectClickEarnings",
        "ExposureMiniRentedClickEarnings",
        "ExposureMicroClickEarnings",
        "ExposureMicroDirectClickEarnings",
        "ExposureMicroRentedClickEarnings",
        "ExposureFixedClickEarnings",
        "ExposureFixedDirectClickEarnings",
        "ExposureFixedRentedClickEarnings",
        "ExposureStandardClickEarnings",
        "ExposureStandardDirectClickEarnings",
        "ExposureStandardRentedClickEarnings",
        "ExposureExtendedClickEarnings",
        "ExposureExtendedDirectClickEarnings",
        "ExposureExtendedRentedClickEarnings",
    };

    private static List<string> ptcCreditsProps = new List<string>()
    {
        "PTCCreditsPerView",
    };

    private static List<string> refPoolRotatorProps = new List<string>()
    {
        "PointsPer1000viewsDeliveredToPoolRotator"
    };

    private static List<string> pointsLevelProps = new List<string>()
    {
        "AutosurfViewLimitMonth",
        "MinAdsWatchedMonthlyToKeepYourLevel",
        "MinPointsToHaveThisLevel",
        "LevelChanceToWinAnyAward",
        "LevelPointsPrizeChance",
        "LevelPointsPrizeMin",
        "LevelPointsPrizeMax",
        "LevelAdCreditsChance",
        "LevelAdCreditsMin",
        "LevelAdCreditsMax",
        "LevelDRLimitIncreasedChance",
        "LevelDRLimitIncreasedMin",
        "LevelDRLimitIncreasedMax"
    };

    private static List<string> pointsProps = new List<string>()
    {
        "AdvertPointsEarnings",
        "PointsYourPTCAdBeingViewed",
        "PointsPer1000viewsDeliveredToPoolRotator",
        "DirectReferralAdvertClickEarningsPoints"
    };

    private static List<string> rentedRefProps = new List<string>()
    {
        "RentedReferralAdvertClickEarnings",
        "RentedReferralsLimit",
        "ReferralRentCost",
        "RenewalDiscount",
        "RentedReferralRecycleCost",
        "CanAutoPay",
        "DailyAutoPayCost",
        "MinRentingIntervalSecs",
        "MaxRefPackageCount"
    };

    private static List<string> trafficGridProps = new List<string>()
    {
        "TrafficGridTrials",
        "TrafficGridChances",
        "TrafficGridShorterAd"
    };

    private static List<string> offerWallProps = new List<string>()
    {
        "OfferwallsProfitPercent"
    };

    private static List<string> moneyPayoutProps = new List<string>()
    {
        "CashoutLimit",
        "CashoutLimitIcreased",
        "MaxGlobalCashout",
        "MaxDailyPayouts",
        "BlockPayoutDays"
    };

    private static List<string> cpaProps = new List<string>()
    {
        "CPAProfitPercent",
        "RefPercentEarningsCPA",
        "MaxGlobalCashout"
    };

    private static List<string> instantPayoutProps = new List<string>()
    {
        "HasInstantPayout",
    };

    private static List<string> revShareProps = new List<string>()
    {
        "AdPackDailyRequiredClicks",
        "MaxExtraAdPackSecondsForClicks"
    };

    private static List<string> timeClickExchangeProps = new List<string>()
    {
        "MaxExtraAdPackSecondsForClicks",
    };

    private static List<string> commissionBalanceProps = new List<string>()
    {
        "SameUserCommissionToMainTransferFee",
    };

    private static List<string> commissionBalanceWithdrawProps = new List<string>()
    {
        "MaxCommissionPayoutsPerDay",
    };

    private static List<string> maintoAdBalanceProps = new List<string>()
    {
        "OtherUserMainToAdTransferFee",
    };

    private static List<string> mainAndPointAndMainProps = new List<string>()
    {
        "OtherUserMainToMainTransferFee",
    };
    private static List<string> pointsAndPointAndMainProps = new List<string>()
    {
        "OtherUserPointsToPointsTransferFee",
    };

    private static List<string> spilloverProps = new List<string>()
    {
        "MaxUpgradedDirectRefs",
    };

    private static List<string> trafficExchangeProps = new List<string>()
    {
        "TrafficExchangeClickEarnings",
        "DRTrafficExchangeClickEarnings"
    };

    private static List<string> publishersEarningProps = new List<string>()
    {
        "PublishersBannerClickProfitPercentage",
        "PublishersCpaOfferProfitPercentage",
        "PublishersInTextAdClickProfitPercentage",
        "PublishersPtcOfferWallProfitPercentage"
    };

    private static List<string> publishersBannerClickProfitProps = new List<string>()
    {
        "PublishersBannerClickProfitPercentage"
    };

    private static List<string> publishersCpaOfferProfitProps = new List<string>()
    {
        "PublishersCpaOfferProfitPercentage"
    };

    private static List<string> publishersInTextAdClickProfitProps = new List<string>()
    {
        "PublishersInTextAdClickProfitPercentage"
    };

    private static List<string> publishersPtcOfferWallProfitProps = new List<string>()
    {
        "PublishersPtcOfferWallProfitPercentage"
    };

    private static List<string> fbProps = new List<string>()
    {
        "MaxFacebookLikesPerDay"
    };

    private static List<string> titanNewsProps = new List<string>()
    {
        "ArticleCreatorCPM",
        "ArticleInfluencerCPM"
    };

    #endregion
}