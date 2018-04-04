using Prem.PTC.Advertising;
using Prem.PTC.Members;
using Prem.PTC.Offers;
using Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Prem.PTC {

    public enum PurchaseBalances
    {
        Purchase = 1,
        Cash = 2,
        PaymentProcessor = 3,
        LoginAdsCredits = 4,
        Traffic = 5
    }

    [Serializable]
    public class PurchaseOption : BaseTableObject
    {
        public enum PaymentProcessorStatusEnum
        {
            Disabled = 0,
            Enabled = 1,
            Unavailable = 2
        }

        #region Columns
        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "PurchaseOptions"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column("Feature")]
        public int FeatureInt { get { return _FeatureInt; } set { _FeatureInt = value; SetUpToDateAsFalse(); } }

        [Column("AdBalanceEnabled")]
        public bool PurchaseBalanceEnabled { get { return _AdBalanceEnabled; } set { _AdBalanceEnabled = value; SetUpToDateAsFalse(); } }

        [Column("CashBalanceEnabled")]
        public bool CashBalanceEnabled { get { return _CashBalanceEnabled; } set { _CashBalanceEnabled = value; SetUpToDateAsFalse(); } }

        [Column("PaymentProcessorEnabled")]
        private int PaymentProcessorEnabledInt { get { return _PaymentProcessorEnabled; } set { _PaymentProcessorEnabled = value; SetUpToDateAsFalse(); } }

        int _Id, _FeatureInt, _PaymentProcessorEnabled;
        bool _AdBalanceEnabled, _CashBalanceEnabled;

        public PaymentProcessorStatusEnum PaymentProcessorStatus
        {
            get { return (PaymentProcessorStatusEnum)Enum.Parse(typeof(PaymentProcessorStatusEnum), PaymentProcessorEnabledInt.ToString()); }
            set { PaymentProcessorEnabledInt = (int)value; }
        }

        public bool PaymentProcessorEnabled
        {
            get { return PaymentProcessorStatus == PaymentProcessorStatusEnum.Enabled; }
        }

        public Features Feature
        {
            get { return (Features)FeatureInt; }
            set { FeatureInt = (int)value; }        
        }
        #endregion

        public enum Features
        {
            NULL = 0,
            CPA = 1,
            PTC = 2,
            AdPack = 4,
            SurfAd = 5,
            Facebook = 6,
            TrafficGrid = 7,
            Banner = 8,
            LoginAd = 9,
            //Marketplace = 10, advertising in marketplace is free
            DirectReferral = 11,
            //RotatorLink = 12,
            Upgrade = 13,
            ExternalBanner = 14,
            PtcOfferWall = 15,
            InTextAds = 16,
            Jackpot = 17,
            InvestmentPlatform = 18,
            MiniVideo = 19,
            PaidToPromote = 20,
        }        

        public static BalanceType GetBalanceType(PurchaseBalances balance)
        {
            if (balance == PurchaseBalances.Purchase)
                return BalanceType.PurchaseBalance;
            if (balance == PurchaseBalances.Cash)
                return BalanceType.CashBalance;

            return BalanceType.PurchaseBalance;
        }

        public PurchaseOption() : base() { }
        public PurchaseOption(int id) : base(id) { }
        public PurchaseOption(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        public static List<int> DisplayedFeatures
        {
            get
            {
                var features = new List<int>();

                if (AppSettings.TitanFeatures.AdvertCPAGPTEnabled)
                    features.Add((int)Features.CPA);
                if (AppSettings.TitanFeatures.AdvertAdsEnabled)
                    features.Add((int)Features.PTC);
                if (AppSettings.TitanFeatures.AdvertAdPacksEnabled)
                    features.Add((int)Features.AdPack);
                if (AppSettings.TitanFeatures.AdvertSurfAdsEnabled)
                    features.Add((int)Features.SurfAd);
                if (AppSettings.TitanFeatures.AdvertFacebookEnabled)
                    features.Add((int)Features.Facebook);
                if (AppSettings.TitanFeatures.AdvertTrafficGridEnabled)
                    features.Add((int)Features.TrafficGrid);
                if (AppSettings.TitanFeatures.AdvertBannersEnabled && AppSettings.BannerAdverts.AdvertisingPolicy == BannerPolicy.SellingPackages)
                    features.Add((int)Features.Banner);
                if (AppSettings.TitanFeatures.AdvertLoginAdsEnabled)
                    features.Add((int)Features.LoginAd);
                //if (AppSettings.TitanFeatures.AdvertMarketplaceEnabled)
                //    features.Add((int)Features.Marketplace);
                if (AppSettings.TitanFeatures.ReferralsDirectEnabled && AppSettings.DirectReferrals.DirectReferralBuyingEnabled)
                    features.Add((int)Features.DirectReferral);
                //if (AppSettings.TitanFeatures.ReferralPoolRotatorEnabled)
                //    features.Add((int)Features.RotatorLink);
                if (AppSettings.TitanFeatures.UpgradeEnabled && !AppSettings.Points.LevelMembershipPolicyEnabled)
                    features.Add((int)Features.Upgrade);
                if (AppSettings.TitanFeatures.PublishBannersEnabled)
                    features.Add((int)Features.ExternalBanner);
                if (AppSettings.TitanFeatures.AdvertPtcOfferWallEnabled)
                    features.Add((int)Features.PtcOfferWall);
                if (AppSettings.TitanFeatures.AdvertInTextAdsEnabled)
                    features.Add((int)Features.InTextAds);
                if (AppSettings.TitanFeatures.MoneyJackpotEnabled)
                    features.Add((int)Features.Jackpot);
                if (AppSettings.InvestmentPlatform.InvestmentPlatformEnabled && !AppSettings.InvestmentPlatform.LevelsEnabled)
                    features.Add((int)Features.InvestmentPlatform);
                if (AppSettings.TitanFeatures.AdvertMiniVideoEnabled || AppSettings.TitanFeatures.EntertainmentMiniVideoEnabled)
                    features.Add((int)Features.MiniVideo);
                if (AppSettings.TitanFeatures.AdvertPaidToPromoteEnabled)
                    features.Add((int)Features.PaidToPromote);

                return features;
            }
        }

        private static Features GetFeatureFromString(string feature)
        {
            return (Features)Enum.Parse(typeof(Features), feature, true);
        }

        public static PurchaseBalances GetTargetBalanceFromString(string balance)
        {
            return (PurchaseBalances)Enum.Parse(typeof(PurchaseBalances), balance, true);
        }

        private static void ValidateChosenBalance(Features feature, PurchaseBalances balance)
        {
            try
            {
                var purchaseOption = Get(feature);

                switch (balance)
                {
                    case PurchaseBalances.Purchase:
                        if (!purchaseOption.PurchaseBalanceEnabled)
                            throw new ArgumentException("Chosen balance is invalid.", balance.ToString());
                        break;
                    case PurchaseBalances.Cash:
                        if (!purchaseOption.CashBalanceEnabled || !AppSettings.Payments.CashBalanceEnabled)
                            throw new ArgumentException("Chosen balance is invalid.", balance.ToString());
                        break;
                    default: throw new ArgumentException("Chosen balance is invalid.", balance.ToString());
                }
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException)
                    throw new MsgException(ex.Message);
                else
                {
                    ErrorLogger.Log(ex);
                    throw new MsgException("Chosen balance is invalid.");
                }
            }
        }
      
        public static void CreditAfterCampaignRejection(ExternalBannerAdvert advert)
        {
            if (AppSettings.TitanFeatures.ReferralMatrixEnabled)
                return;

            var user = new Member(advert.UserId);
            CreditBalance(user, advert.TargetBalance, advert.PricePaid, "External Banner Advert rejected by administrator");
        }

        public static void CreditAfterCampaignRejection(Advert advert)
        {
            if (AppSettings.TitanFeatures.ReferralMatrixEnabled)
                return;

            var user = new Member(advert.Advertiser.MemberUsername);
            var objName = string.Join(" ", Regex.Matches(advert.GetType().Name, @"([A-Z][a-z]+)").Cast<Match>().Select(m => m.Value));
            CreditBalance(user, advert.TargetBalance, advert.Price, string.Format("{0} rejected by administrator", objName));
        }

        public static void CreditAfterCampaignRejection(LoginAd advert)
        {
            if (AppSettings.TitanFeatures.ReferralMatrixEnabled)
                return;

            var user = new Member(advert.CreatorUserId);
            CreditBalance(user, advert.TargetBalance, advert.PricePaid, "Login Ad rejected by administrator");
        }
        
        public static void CreditAfterCampaignRejection(CPAOffer advert)
        {
            if (AppSettings.TitanFeatures.ReferralMatrixEnabled)
                return;

            Decimal CalculatedPercent = (Decimal)(AppSettings.CPAGPT.MoneyTakenFromCPAOffersPercent + 100) / (Decimal)100;
            CalculatedPercent = CalculatedPercent * advert.CreditsBought;

            var adCost = CalculatedPercent * advert.BaseValue;

            var user = new Member(advert.AdvertiserUsername);
            CreditBalance(user, advert.TargetBalance, adCost, "CPA offer rejected by administrator");
        }

        /// <summary>
        /// Saves balances.
        /// </summary>
        private static void CreditBalance(Member user, PurchaseBalances balance, Money amount, string note)
        {
            switch (balance)
            {
                case PurchaseBalances.Purchase:
                    user.AddToPurchaseBalance(amount, note);
                    break;
                case PurchaseBalances.Cash:
                    user.AddToCashBalance(amount, note);
                    break;
                case PurchaseBalances.LoginAdsCredits:
                    user.AddToLoginAdsCredits(amount.AsPoints(), note);
                    break;
                case PurchaseBalances.Traffic:
                    user.AddToTrafficBalance(amount, note);
                    break;
            }

            user.SaveBalances();
        }

        public static void ChargeBalance(Member user, Money amount, string feature, PurchaseBalances balance, string note, BalanceLogType balanceLogType = BalanceLogType.Other)
        {
            ChargeBalance(user, amount, balance, note, feature, balanceLogType);
        }

        public static void ChargeBalance(Member user, Money amount, PurchaseBalances balance, string note, BalanceLogType balanceLogType = BalanceLogType.Other)
        {
            ChargeBalance(user, amount, balance, note, null, balanceLogType);
        }

        private static void ChargeBalance(Member user, Money amount, PurchaseBalances balance, string note, string feature = null, BalanceLogType balanceLogType = BalanceLogType.Other)
        {
            if (feature != null)
            {
                Features targetFeature = GetFeatureFromString(feature);
                ValidateChosenBalance(targetFeature, balance);
            }

            switch (balance) {
                case PurchaseBalances.Cash:
                    if (amount > user.CashBalance)
                        throw new MsgException(L1.NOTENOUGHFUNDS);
                    user.SubtractFromCashBalance(amount, note, balanceLogType);
                    break;
                case PurchaseBalances.Purchase:
                    if (amount > user.PurchaseBalance)
                        throw new MsgException(L1.NOTENOUGHFUNDS);
                    user.SubtractFromPurchaseBalance(amount, note, balanceLogType);
                    break;
                case PurchaseBalances.Traffic:
                    if (amount > user.TrafficBalance)
                        throw new MsgException(L1.NOTENOUGHFUNDS);
                    user.SubtractFromTrafficBalance(amount, note, balanceLogType);
                    break;
                case PurchaseBalances.LoginAdsCredits:
                    if(amount.AsPoints() > user.LoginAdsCredits)
                        throw new MsgException(L1.NOTENOUGHFUNDS);
                    user.SubstractFromLoginAdsCredits(amount.AsPoints(), note, balanceLogType);
                    break;
            }

            user.SaveBalances();
        }

        public static PurchaseOption Get(Features feature)
        {
            return TableHelper.GetListFromRawQuery<PurchaseOption>
                (string.Format("SELECT * FROM PurchaseOptions WHERE Feature = {0}", (int)feature)).Single();
        }
    }
}