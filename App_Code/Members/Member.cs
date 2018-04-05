using Prem.PTC.Memberships;
using Prem.PTC.Utils;
using System;
using System.Data;
using System.Reflection;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using Prem.PTC.Utils.NVP;
using System.Web.Security;
using Titan;
using System.Web.Caching;
using Prem.PTC.Security;
using Titan.Cryptocurrencies;

namespace Prem.PTC.Members
{
    [Serializable]
    public partial class Member : BaseTableObject
    {
        #region Columns
        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return AppSettings.TableNames.Members; } }
        protected override string dbTable { get { return TableName; } }

        public static class Columns
        {
            public const string Id = "UserId";
            public const string Username = "Username";
            public const string PrimaryPassword = "Password1";
            public const string SecondaryPassword = "Password2";
            public const string PIN = "PIN";
            public const string AccountStatus = "AccountStatus";
            public const string IsLocked = "IsLocked";
            public const string FailedPasswordAttemptCount = "FailedPasswordAttemptCount";
            public const string FailedPasswordAttemptWindow = "FailedPasswordAttemptWindow";
            public const string FailedPINAttemptCount = "FailedPINAttemptCount";
            public const string Email = "Email";
            public const string MainBalance = "Balance1";
            public const string TrafficBalance = "Balance2";
            public const string RentalBalance = "Balance2";
            public const string AdvertisingBalance = "Balance3";
            public const string PointsBalance = "Balance4";
            public const string CommissionBalance = "CommissionBalance";
            public const string CashBalance = "CashBalance";
            public const string InvestmentBalance = "InvestmentBalance";
            public const string MarketplaceBalance = "MarketplaceBalance";
            public const string Country = "Country";
            public const string CountryCode = "Code";
            public const string BirthYear = "BirthYear";
            public const string IsMale = "IsMale";
            public const string RegisterDate = "RegisterDate";
            public const string LastActivityDate = "LastActivityDate2";
            public const string LastLoginDate = "LastLoginDate";
            public const string MembershipId = "UpgradeId";
            public const string MembershipName = "UpgradeName";
            public const string MembershipExpires = "UpgradeExpires";
            public const string MembershipWhen = "UpgradeWhen";
            public const string ViewedAds = "ViewedAds";
            public const string Referer = "Referer";
            public const string DetailedBanReason = "DetailedBanReason";
            public const string MessageSystemTurnedOn = "MessageSystemTurnedOn";
            public const string AvatarUrl = "AvatarUrl";
            public const string TotalClicks = "TotalClicks";
            public const string CameFromUrl = "CameFromUrl";
            public const string StatsClicks = "StatsClicks";
            public const string StatsEarned = "StatsEarned";
            public const string LastRentDate = "LastRentDate";
            public const string IsRented = "IsRented";
            public const string TotalEarned = "TotalEarned";
            public const string IsSpotted = "IsSpotted";
            public const string Achievements = "Achievements";
            public const string UnspottedAchievements = "UnspottedAchievements";
            public const string CashoutsProceed = "CashoutsProceed";
            public const string IsForumModerator = "IsForumModerator";
            public const string LikedAds = "LikedAds";
            public const string UserClicksStats = "UserClicksStats";
            public const string UserInfo = "UserInfo";
            public const string LastUsedIP = "LastUsedIP";
            public const string RegisteredWithIP = "RegisteredWithIP";
            public const string TrafficGridHitsToday = "TrafficGridHitsToday";
            public const string DirectReferralLimitEnlargedBy = "DirectReferralLimitEnlargedBy";
            public const string TrafficGridTotalWons = "TrafficGridTotalWons";

            public const string PhoneCountryCode = "PhoneCountryCodeE";
            public const string PhoneNumber = "PhoneNumberE";
            public const string IsRegisterIPVerified = "IsRegisterIPVerified";
            public const string IsPhoneVerified = "IsPhoneVerified";
            public const string UnconfirmedSMSSent = "UnconfirmedSMSSent";
            public const string IsPhoneVerifiedBeforeCashout = "IsPhoneVerifiedBeforeCashout";

            public const string FacebookName = "FacebookName";
            public const string PerfectMoneyAccountNo = "PerfectMoneyAccountNo";
            public const string IsExcludedFromInstantPayment = "IsExcludedFromInstantPayment";
            public const string IsForumAdministrator = "IsForumAdministrator";
            public const string CommissionToMainBalanceEnabled = "CommissionToMainBalanceEnabled";
            public const string ReferrerId = "ReferrerId";
            public const string RepresentativeId = "RepresentativeId";
            public const string ReferrerExpirationDate = "ReferrerExpirationDate";
            public const string HasEverUpgraded = "HasEverUpgraded";
            public const string FbLikesToday = "FbLikesToday";
            public const string CreditedRefererReward = "CreditedRefererReward";
            public const string SlotMachineChances = "SlotMachineChances";

            public const string ResolveReferralsLimitDate = "ResolveReferralsLimitDate";

            public const string AmountOfWatchedTrafficAdsToday = "AmountOfWatchedTrafficAdsToday";

            public const string InvestedIntoPlans = "InvestedIntoPlans";
            public const string InvestedIntoPlansStructure = "InvestedIntoPlansStructure";

        }


        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Username)]
        public string Name { get { return _username; } set { _username = value; SetUpToDateAsFalse(); } }

        [Column(Columns.PrimaryPassword)]
        public string PrimaryPassword { get { return _password1; } set { _password1 = value; SetUpToDateAsFalse(); } }

        [Column(Columns.SecondaryPassword)]
        public string SecondaryPassword { get { return _password2; } set { _password2 = value; SetUpToDateAsFalse(); } }

        /// <summary>
        /// 4-digit-long code
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When given argument is not correct PIN</exception>
        [Column(Columns.PIN)]
        public int? PIN
        {
            get { return _pin; }
            set
            {
                if (value != null)
                    if (value < 0 || value > 9999)
                        throw new ArgumentOutOfRangeException("Given PIN is incorrect (" + Convert.ToString(value) + ")");

                _pin = value; SetUpToDateAsFalse();
            }
        }

        [Column(Columns.FailedPasswordAttemptCount)]
        public int FailedPasswordAttemptCount { get { return _failedPasswordAttemptCount; } set { _failedPasswordAttemptCount = value; SetUpToDateAsFalse(); } }

        [Column(Columns.IsExcludedFromInstantPayment)]
        public bool IsExcludedFromInstantPayment { get { return _IsExcludedFromInstantPayment; } set { _IsExcludedFromInstantPayment = value; SetUpToDateAsFalse(); } }

        [Column(Columns.FailedPasswordAttemptWindow)]
        public DateTime? FailedPasswordAttemptWindow { get { return _failedPasswordAttemptWindow; } set { _failedPasswordAttemptWindow = value; SetUpToDateAsFalse(); } }

        [Column(Columns.FailedPINAttemptCount)]
        public int FailedPINAttemptCount { get { return _failedPINAttemptCount; } set { _failedPINAttemptCount = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Email)]
        public string Email { get { return _email; } set { _email = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Country)]
        public string Country { get { return _country; } set { _country = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CountryCode)]
        public string CountryCode { get { return _countryCode; } set { _countryCode = value; SetUpToDateAsFalse(); } }

        [Column(Columns.BirthYear)]
        public DateTime? BirthYear { get { return _birth; } set { _birth = value; SetUpToDateAsFalse(); } }

        [Column(Columns.IsMale)]
        protected bool? IsMale { get { return _isMale; } set { _isMale = value; SetUpToDateAsFalse(); } }

        [Column(Columns.RegisterDate)]
        public DateTime Registered { get { return _registered; } set { _registered = value; SetUpToDateAsFalse(); } }

        [Column(Columns.LastActivityDate)]
        public DateTime? LastActive { get { return _lastActive; } set { _lastActive = value; SetUpToDateAsFalse(); } }

        [Column(Columns.LastLoginDate)]
        public DateTime? LastLogged { get { return _lastLogged; } protected set { _lastLogged = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ViewedAds)]
        protected string ViewedAds { get { return _viewedAds; } set { _viewedAds = value; SetUpToDateAsFalse(); } }

        //[Column("TEViewedAds")]
        //protected string TEViewedAds { get { return _TEViewedAds; } set { _TEViewedAds = value; SetUpToDateAsFalse(); } }

        [Column(Columns.LikedAds)]
        protected string LikedAds { get { return _LikedAds; } set { _LikedAds = value; SetUpToDateAsFalse(); } }

        [Obsolete]
        [Column(Columns.Referer)]
        public string Referer { get { return _referer; } set { _referer = value; SetUpToDateAsFalse(); } }

        [Column(Columns.MessageSystemTurnedOn)]
        public bool MessageSystemTurnedOn { get { return _msto; } set { _msto = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AvatarUrl)]
        public string AvatarUrl { get { return _avatarUrl; } set { _avatarUrl = value; SetUpToDateAsFalse(); } }

        [Column(Columns.TotalClicks)]
        public int TotalClicks { get { return _totalClicks; } set { _totalClicks = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CameFromUrl)]
        public string CameFromUrl { get { return _cameFromUrl; } set { _cameFromUrl = value; SetUpToDateAsFalse(); } }

        [Column(Columns.LastRentDate)]
        public DateTime? LastRentDate { get { return _lrd; } set { _lrd = value; SetUpToDateAsFalse(); } }

        [Column(Columns.IsRented)]
        public bool IsRented { get { return _isRent; } set { _isRent = value; SetUpToDateAsFalse(); } }

        [Column(Columns.IsForumModerator)]
        public bool IsForumModerator { get { return _isFM; } set { _isFM = value; SetUpToDateAsFalse(); } }

        [Column(Columns.IsForumAdministrator)]
        public bool IsForumAdministrator { get { return _isFA; } set { _isFA = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Achievements)]
        protected string AchievementsString { get { return _AchievementsString; } set { _AchievementsString = value; SetUpToDateAsFalse(); } }

        [Column(Columns.UnspottedAchievements)]
        public int UnspottedAchievements { get { return ua; } set { ua = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CashoutsProceed)]
        public int CashoutsProceed
        {
            get { return _CashoutsProceed; }
            set
            {
                if (value < 0)
                    value = 0;
                _CashoutsProceed = value; SetUpToDateAsFalse();
            }
        }

        /// <summary>
        /// Indicates wheather the Referer noticed that he has the referral or not
        /// False - not
        /// Used in Notification System
        /// </summary>
        [Column(Columns.IsSpotted)]
        public bool IsSpotted { get { return _isSpot; } set { _isSpot = value; SetUpToDateAsFalse(); } }

        [Column(Columns.UserInfo)]
        public string UserAgentInformation { get { return _UserInfo; } set { _UserInfo = value; SetUpToDateAsFalse(); } }

        [Column(Columns.LastUsedIP)]
        public string LastUsedIP { get { return _LastUsedIP; } set { _LastUsedIP = value; SetUpToDateAsFalse(); } }

        [Column(Columns.RegisteredWithIP)]
        public string RegisteredWithIP { get { return _RegisteredWithIP; } set { _RegisteredWithIP = value; SetUpToDateAsFalse(); } }

        [Column(Columns.TrafficGridHitsToday)]
        public int TrafficGridHitsToday { get { return _TrafficGridHitsToday; } set { _TrafficGridHitsToday = value; SetUpToDateAsFalse(); } }

        [Column(Columns.DirectReferralLimitEnlargedBy)]
        public int DirectReferralLimitEnlargedBy { get { return _DirectReferralLimitEnlargedBy; } set { _DirectReferralLimitEnlargedBy = value; SetUpToDateAsFalse(); } }

        [Column(Columns.TrafficGridTotalWons)]
        public int TrafficGridTotalWons { get { return _TrafficGridTotalWons; } set { _TrafficGridTotalWons = value; SetUpToDateAsFalse(); } }

        [Column(Columns.PhoneCountryCode)]
        public string PhoneCountryCode { get { return _PhoneCountryCode; } set { _PhoneCountryCode = value; SetUpToDateAsFalse(); } }

        [Column(Columns.PhoneNumber)]
        public string PhoneNumber { get { return _PhoneNumber; } set { _PhoneNumber = value; SetUpToDateAsFalse(); } }

        [Column(Columns.IsRegisterIPVerified)]
        public bool IsRegisterIPVerified { get { return _IsRegisterIPVerified; } set { _IsRegisterIPVerified = value; SetUpToDateAsFalse(); } }

        [Column(Columns.IsPhoneVerified)]
        public bool IsPhoneVerified { get { return _IsPhoneVerified; } set { _IsPhoneVerified = value; SetUpToDateAsFalse(); } }

        [Column(Columns.IsPhoneVerifiedBeforeCashout)]
        public bool IsPhoneVerifiedBeforeCashout { get { return _IsPhoneVerifiedBeforeCashout; } set { _IsPhoneVerifiedBeforeCashout = value; SetUpToDateAsFalse(); } }

        [Column("BypassSecurityCheck")]
        protected int BypassSecurityCheckInt { get { return _BypassSecurityCheck; } set { _BypassSecurityCheck = value; SetUpToDateAsFalse(); } }

        public BypassSecurityCheck BypassSecurityCheck
        {
            get { return (BypassSecurityCheck)BypassSecurityCheckInt; }
            set { BypassSecurityCheckInt = (int)value; }
        }

        [Column(Columns.UnconfirmedSMSSent)]
        public int UnconfirmedSMSSent { get { return _UnconfirmedSMSSent; }
            set
            {
                if (value < 0)
                    value = 0;
                _UnconfirmedSMSSent = value; SetUpToDateAsFalse(); } }

        [Column(Columns.FacebookName)]
        public string FacebookName { get { return _FacebookName; } set { _FacebookName = value; SetUpToDateAsFalse(); } }

        [Column("MoneyTransferred")]
        public Money MoneyTransferredFromMainBalance { get { return _mt; } set { _mt = value; SetUpToDateAsFalse(); } }

        [Column("MoneyCashout")]
        public Money MoneyCashout
        {
            get { return _mc; }
            set
            {
                if (value < Money.Zero)
                    value = Money.Zero;
                _mc = value; SetUpToDateAsFalse();
            }
        }

        [Column("ZipCode")]
        public string ZipCode { get { return w1; } set { w1 = value; SetUpToDateAsFalse(); } }

        [Column("StateProvince")]
        public string StateProvince { get { return w2; } set { w2 = value; SetUpToDateAsFalse(); } }

        [Column("City")]
        public string City { get { return w3; } set { w3 = value; SetUpToDateAsFalse(); } }

        [Column("Address")]
        public string Address { get { return w4; } set { w4 = value; SetUpToDateAsFalse(); } }

        [Column("SecondName")]
        public string SecondName { get { return w5; } set { w5 = value; SetUpToDateAsFalse(); } }

        [Column("FirstName")]
        public string FirstName { get { return w6; } set { w6 = value; SetUpToDateAsFalse(); } }

        [Column("TotalCPACompleted")]
        public int TotalCPACompleted { get { return i1; } set { i1 = value; SetUpToDateAsFalse(); } }

        [Column("PointsToday")]
        public int PointsToday { get { return i2; } set { i2 = value; SetUpToDateAsFalse(); } }

        //2001

        [Column("PointsEarnedToReferer")]
        public int PointsEarnedToReferer { get { return Ei2; } set { Ei2 = value; SetUpToDateAsFalse(); } }

        [Column("LastPointableActivity")]
        public DateTime? LastPointableActivity { get { return _lrdQQ; } set { _lrdQQ = value; SetUpToDateAsFalse(); } }

        [Column("CustomFields")]
        private string _CustomFields { get { return u3500f; } set { u3500f = value; SetUpToDateAsFalse(); } }

        [Column("AdminNotes")]
        public string AdminNotes { get { return _AdminNotes; } set { _AdminNotes = value; SetUpToDateAsFalse(); } }

        [Column("MasterPassword")]
        public string MasterPassword { get { return _MasterPassword; } set { _MasterPassword = value; SetUpToDateAsFalse(); } }

        [Column("MasterPasswordValidUntil")]
        public DateTime? MasterPasswordValidUntil { get { return mppvu; } set { mppvu = value; SetUpToDateAsFalse(); } }

        [Column("TransferFundsPermission")]
        private int TransferFundsPermission { get { return _TransferFundsPermission; } set { _TransferFundsPermission = value; SetUpToDateAsFalse(); } }

        [Column("ShoutboxPermission")]
        private int ShoutboxPermissionField { get { return _ShoutboxPermission; } set { _ShoutboxPermission = value; SetUpToDateAsFalse(); } }

        [Column("CPACompletedBehavior")]
        private int CPACompletedBehaviorField { get { return _CPACompletedBehavior; } set { _CPACompletedBehavior = value; SetUpToDateAsFalse(); } }

        [Column("LastActivityTime")]
        public DateTime? LastActivityTime { get { return mppvu2; } set { mppvu2 = value; SetUpToDateAsFalse(); } }

        [Column("FacebookOAuthId")]
        public string FacebookOAuthId { get { return _FacebookOAuthId; } set { _FacebookOAuthId = value; SetUpToDateAsFalse(); } }

        [Column("CPACompletedToday")]
        public int CPACompletedToday { get { return _CPACompletedToday; } set { _CPACompletedToday = value; SetUpToDateAsFalse(); } }

        [Column("Temp")]
        public string Temp { get { return _Temp; } set { _Temp = value; SetUpToDateAsFalse(); } }

        [Column("VacationModeEnds")]
        public DateTime? VacationModeEnds { get { return _VacationModeEnds; } set { _VacationModeEnds = value; SetUpToDateAsFalse(); } }

        [Column("CaptchaType")]
        internal int IntCaptchaType { get { return _CaptchaType; } set { _CaptchaType = value; SetUpToDateAsFalse(); } }

        [Column("RSAAdsViewed")]
        public string RSAAdsViewed { get { return _RSAAdsViewed; } set { _RSAAdsViewed = value; SetUpToDateAsFalse(); } }

        [Column("TotalPointsExchanged")]
        public int TotalPointsExchanged { get { return _TotalPointsExchanged; } set { _TotalPointsExchanged = value; SetUpToDateAsFalse(); } }

        [Column("TotalPointsGenerated")]
        public int TotalPointsGenerated { get { return _TotalPointsGenerated; } set { _TotalPointsGenerated = value; SetUpToDateAsFalse(); } }

        [Column("CompletedOffersFromOfferwallsToday")]
        public int CompletedOffersFromOfferwallsToday { get { return _CompletedOffersFromOfferwallsToday; } set { _CompletedOffersFromOfferwallsToday = value; SetUpToDateAsFalse(); } }

        [Column("CompletedOffersMoreThan100pFromOfferwallsToday")]
        public int CompletedOffersMoreThan100pFromOfferwallsToday { get { return _CompletedOffersMoreThan100pFromOfferwallsToday; } set { _CompletedOffersMoreThan100pFromOfferwallsToday = value; SetUpToDateAsFalse(); } }

        [Column("CompletedDailyOffersFromOfferwallsToday")]
        public int CompletedDailyOffersFromOfferwallsToday { get { return _CompletedDailyOffersFromOfferwallsToday; } set { _CompletedDailyOffersFromOfferwallsToday = value; SetUpToDateAsFalse(); } }

        [Column("RevenueShareAdsWatchedYesterday")]
        public int RevenueShareAdsWatchedYesterday { get { return _RevenueShareAdsWatchedYesterday; } set { _RevenueShareAdsWatchedYesterday = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CommissionToMainBalanceEnabled)]
        public bool CommissionToMainBalanceEnabled { get { return _CommissionToMainBalanceEnabled; } set { _CommissionToMainBalanceEnabled = value; SetUpToDateAsFalse(); } }

        [Column("HasEverUpgraded")]
        public bool HasEverUpgraded { get { return _HasEverUpgraded; } set { _HasEverUpgraded = value; SetUpToDateAsFalse(); } }

        /// <summary>
        /// Returns -1 if no referrer. Recommended to make .HasReferrer() to check if referrer exists.
        /// </summary>
        [Column(Columns.ReferrerId)]
        public int ReferrerId { get { return _ReferrerId; } set { _ReferrerId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.RepresentativeId)]
        public int? RepresentativeId { get { return _RepresentativeId; } set { _RepresentativeId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ReferrerExpirationDate)]
        public DateTime? ReferrerExpirationDate { get { return _ReferrerExpirationDate; } set { _ReferrerExpirationDate = value; SetUpToDateAsFalse(); } }

        [Column("IsEarner")]
        public bool IsEarner { get { return _IsEarner; } set { _IsEarner = value; SetUpToDateAsFalse(); } }

        [Column("IsAdvertiser")]
        public bool IsAdvertiser { get { return _IsAdvertiser; } set { _IsAdvertiser = value; SetUpToDateAsFalse(); } }

        [Column("IsPublisher")]
        public bool IsPublisher { get { return _IsPublisher; } set { _IsPublisher = value; SetUpToDateAsFalse(); } }

        [Column(Columns.FbLikesToday)]
        public int FbLikesToday { get { return _FbLikesToday; } set { _FbLikesToday = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CreditedRefererReward)]
        public bool CreditedRefererReward { get { return _CreditedRefererReward; } set { _CreditedRefererReward = value; SetUpToDateAsFalse(); } }

        [Column(Columns.SlotMachineChances)]
        public int SlotMachineChances { get { return _SlotMachineChances; } set { _SlotMachineChances = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AmountOfWatchedTrafficAdsToday)]
        public int NumberOfWatchedTrafficAdsToday { get { return _NumberOfWatchedTrafficAdsToday; } set { _NumberOfWatchedTrafficAdsToday = value; SetUpToDateAsFalse(); } }

        [Column("RegisteredLatitude")]
        public Decimal? RegisteredLatitude { get { return _RegisteredLatitude; } set { _RegisteredLatitude = value; SetUpToDateAsFalse(); } }

        [Column("RegisteredLongitude")]
        public Decimal? RegisteredLongitude { get { return _RegisteredLongitude; } set { _RegisteredLongitude = value; SetUpToDateAsFalse(); } }


        private int _id, _failedPasswordAttemptCount, _failedPINAttemptCount, Ei2, _SlotMachineChances,
            _totalClicks, ua, _CashoutsProceed, _UnconfirmedSMSSent, i1, i2, _BypassSecurityCheck, _CPACompletedToday, _ReferrerId, _FbLikesToday;
        private string _username, _password1, _password2, _accountStatus, hh1, _email, _country, _countryCode, _referer, _detailedBanReason, _RSAAdsViewed;
        private DateTime? _failedPasswordAttemptWindow, _birth, _lastActive, _lastLogged, _lrd, _lrdQQ, mppvu, mppvu2, _VacationModeEnds, _ReferrerExpirationDate;
        private Money _mainBalance, _trafficBalance, _advertisingBalance, _mt, _mc, _CommissionBalance,
            _CashBalance, _InvestmentBalance, _MarketplaceBalance;
        private Decimal _cryptoCurrencyBalance, _PTCCredits;
        private int _pointsBalance, _CaptchaType, _TotalPointsGenerated, _TotalPointsExchanged, _CompletedOffersFromOfferwallsToday, _CompletedOffersMoreThan100pFromOfferwallsToday,
            _CompletedDailyOffersFromOfferwallsToday, _LoginAdsCredits;
        private DateTime _registered;
        private bool _isLocked, _msto, _isSpot, _isRent, _isFM, _IsRegisterIPVerified, _isFA,
             _IsPhoneVerified, _IsPhoneVerifiedBeforeCashout, _IsExcludedFromInstantPayment, _CommissionToMainBalanceEnabled, _HasEverUpgraded,
            _IsEarner, _IsAdvertiser, _IsPublisher, _CreditedRefererReward;
        private int? _pin, _RepresentativeId;
        private string _viewedAds, _TEViewedAds, _avatarUrl, _cameFromUrl, _AchievementsString, _LikedAds,
            _FacebookName, _PhoneCountryCode, _PhoneNumber, w1, w2, w3, w4, w5, w6;

        private Decimal? _RegisteredLatitude, _RegisteredLongitude;

        private int _RevenueShareAdsWatchedYesterday, _TrafficGridHitsToday, _DirectReferralLimitEnlargedBy, _TrafficGridTotalWons, _TransferFundsPermission,
            _ShoutboxPermission, _CPACompletedBehavior, _NumberOfWatchedTrafficAdsToday;
        private string _UserInfo, _LastUsedIP, _RegisteredWithIP, u3500f, _AdminNotes, _MasterPassword, _FacebookOAuthId, _Temp;

        private bool? _isMale;

        public bool IsFromMasterLogin { get; set; } //It's really bad XD

        #endregion Columns

        public TransferFundsPermission TransferPermission
        {
            get
            {
                return (TransferFundsPermission)(TransferFundsPermission);
            }
            set
            {
                TransferFundsPermission = (int)value;
            }
        }

        public CaptchaType SelectedCaptchaType
        {
            get { return (CaptchaType)IntCaptchaType; }

            set { IntCaptchaType = (int)value; }
        }

        public ShoutboxPermission ShoutboxPrivacyPermission
        {
            get
            {
                return (ShoutboxPermission)(ShoutboxPermissionField);
            }
            set
            {
                ShoutboxPermissionField = (int)value;
            }
        }

        public CPACompletedBehavior CPAOfferCompletedBehavior
        {
            get
            {
                return (CPACompletedBehavior)(CPACompletedBehaviorField);
            }
            set
            {
                CPACompletedBehaviorField = (int)value;
            }
        }

        public NotNullNameValuePairs Custom
        {
            get
            {
                return NotNullNameValuePairs.Parse(_CustomFields);
            }
            set
            {
                _CustomFields = value.ToString();
            }
        }

        /// <summary>
        /// Username colored (based on membership)
        /// Use with Literal control (contains HTML)
        /// </summary>
        public string FormattedName
        {
            get
            {
                if (IsForumAdministrator)
                    return "<span style='color:" + AppSettings.Shoutbox.ShoutboxAdminColor + "'>" + Name + "</span>";
                if (IsForumModerator)
                    return "<span style='color:" + AppSettings.Shoutbox.ShoutboxMorderatorColor + "'>" + Name + "</span>";

                return "<span style='color:" + Membership.Color + "'>" + Name + "</span>";
            }
        }

        /// <summary>
        /// Direct Referral Limit FULL (taken from Membership + new slots won in the TrafficGrid)
        /// </summary>
        public int DirectReferralLimit
        {
            get
            {
                if (Membership.DirectReferralsLimit >= 1000000000)
                    return Membership.DirectReferralsLimit;
                else
                    return Membership.DirectReferralsLimit + DirectReferralLimitEnlargedBy;
            }
        }

        public int Age
        {
            get
            {
                if (this.BirthYear != null)
                    return DateTime.Now.Year - ((DateTime)this.BirthYear).Year;
                return 0;
            }
        }


        /// <summary>
        /// Use with Literal control (contains HTML)
        /// </summary>
        public string FormattedMembershipExpires
        {
            get
            {
                if (MembershipExpires == null)
                    return "<i>" + Resources.L1.NEVER + "</i>";
                else
                    return Convert.ToDateTime(MembershipExpires).ToShortDateString();
            }
        }

        /// <summary>
        /// AdsViewed have at least 1 element by default
        /// </summary>
        public int AdsViewedCount
        {
            get
            {
                return AdsViewed.Count - 1;
            }
        }

        public List<int> AdsViewed
        {
            get
            {
                List<int> adslist = new List<int>();
                adslist = TableHelper.GetIntListFromString(ViewedAds);
                return adslist;
            }
            set
            {
                List<int> distinct = value.Distinct<int>().ToList(); //We dont want duplicate ad id's
                ViewedAds = TableHelper.GetStringFromIntList(distinct);
            }
        }

        public List<int> RSAPTCAdsViewed
        {
            get
            {
                List<int> adslist = new List<int>();
                adslist = TableHelper.GetIntListFromString(RSAAdsViewed);
                adslist.Remove(-1);
                return adslist;
            }
            set
            {
                List<int> distinct = value.Distinct<int>().ToList(); //We dont want duplicate ad id's
                RSAAdsViewed = TableHelper.GetStringFromIntList(distinct);
            }
        }

        public List<int> AdsLiked
        {
            get
            {
                List<int> adslist = new List<int>();
                adslist = TableHelper.GetIntListFromString(LikedAds);
                return adslist;
            }
            set
            {
                List<int> distinct = value.Distinct<int>().ToList(); //We dont want duplicate ad id's
                LikedAds = TableHelper.GetStringFromIntList(distinct);
            }
        }

        /// <summary>
        /// Empty list contains only '-1' !!
        /// </summary>
        public List<int> Achievements
        {
            get
            {
                List<int> achlist = new List<int>();
                achlist = TableHelper.GetIntListFromString(AchievementsString);
                return achlist;
            }
            set
            {
                List<int> distinct = value.Distinct<int>().ToList(); //We dont want duplicate ad id's
                AchievementsString = TableHelper.GetStringFromIntList(distinct);
            }
        }


        /// <summary>
        /// Tries to add achievements id to the current user collection. If the list is empty or contains only achievements already obtained, returns false
        /// Also adds proper Points to PointsBalance and mark new achievements as Unspotted
        /// You SHOULD force notification refres after this changes something
        /// </summary>
        /// <param name="AchievementsIds"></param>
        public bool TryToAddAchievements(List<Prem.PTC.Achievements.Achievement> NewAchievements)
        {
            List<int> currentAchievements = Achievements;
            bool isModified = false;

            foreach (Prem.PTC.Achievements.Achievement achiv in NewAchievements)
            {
                if (!currentAchievements.Contains(achiv.Id))
                {
                    isModified = true;
                    currentAchievements.Add(achiv.Id);
                    Achievements = currentAchievements;

                    //Unspotted
                    UnspottedAchievements++;

                    //Credit Points
                    AchievmentCrediter crediter = new AchievmentCrediter(this);
                    crediter.CreditPoints(achiv.Points);
                }
            }

            return isModified;
        }

        public bool HasClickedEnoughToProfitFromReferrals()
        {
            if (Registered.Date == DateTime.Now.Date)
                return true;

            if (UserStatisticsClicks[1] >= AppSettings.Referrals.MinDailyClicksToEarnFromRefs)
                return true;

            return false;
        }

        /// <summary>
        /// Returns user color based on the Membership
        /// NULL if Standard 
        /// </summary>
        public string Color
        {
            get
            {
                if (Membership.Name == Memberships.Membership.Standard.Name)
                    return null;
                return Membership.Color;
            }
        }


        [Obsolete]
        public static string GetCurrentIP(System.Web.HttpRequest request)
        {
            return IP.Current;
        }

        public List<Member> GetDirectReferralsList()
        {
            var dict = TableHelper.MakeDictionary(Member.Columns.ReferrerId, Id);
            dict.Add("IsRented", false);
            return TableHelper.SelectRows<Member>(dict);
        }

        public int GetDirectReferralsCount()
        {
            return GetDirectReferralsCount(Id);
        }

        public static int GetDirectReferralsCount(int userId)
        {
            return (int)TableHelper.SelectScalar(string.Format("SELECT COUNT(*) FROM Users WHERE ReferrerId = {0}", userId));
        }

        public int GetActiveDirectReferralsCount()
        {
            return (int)TableHelper.SelectScalar(string.Format(@"
                                    SELECT COUNT(UserId) FROM Users 
                                    WHERE ReferrerId = {0} AND AccountStatusInt = {1}", Id, (int)MemberStatus.Active));
        }

        /// <exception cref="ArgumentOutOfRangeException" />
        public Gender Gender
        {
            get { return IsMale.MapFromDatabase(); }
            set { IsMale = value.MapToDatabase(); }
        }

        public IMembership Membership
        {
            get
            {
                var cache = (List<Memberships.Membership>)(new MembershipsAllStatusCache()).Get();
                return cache.Where(membership => membership.Id == MembershipId).First();
            }

            set
            {
                MembershipId = value.Id;
                MembershipName = value.Name;
            }
        }

        public bool HasSecondaryPassword()
        {
            if (String.IsNullOrEmpty(SecondaryPassword))
                return false;
            return true;
        }

        #region Constructors

        public Member()
            : base()
        { }
        public Member(int id)
            : base(id)
        { }
        public Member(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        { }

        public Member(string username) : this(GetMemberId(username))
        {

        }

        public static int GetMemberId(string username)
        {
            var result = TableHelper.SelectRows<Member>(TableHelper.MakeDictionary(Columns.Username, username));

            if (result.Count == 0)
                throw new MsgException(Resources.L1.ER_USER_NOTFOUND + ": " + username);

            return result[0].Id;
        }

        public static string GetMemberUsername(int userId)
        {
            if (userId == -1)
                return string.Empty;

            var result = TableHelper.SelectRows<Member>(TableHelper.MakeDictionary(Columns.Id, userId));

            if (result.Count == 0)
                throw new MsgException(Resources.L1.ER_USER_NOTFOUND + ": " + userId);

            return result[0].Name;
        }

        #endregion Constructors


        /// <summary>
        /// Check if there is a user in a database with specified username
        /// </summary>
        /// <exception cref="DbException" />
        public static bool Exists(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;

            return TableHelper.RowExists(Member.TableName, Member.Columns.Username, username);
        }

        /// <summary>
        /// Check if there is a user in a database with specified Email address
        /// </summary>
        /// <exception cref="DbException" />
        public static bool ExistsWithEmail(string Email)
        {
            if (string.IsNullOrWhiteSpace(Email)) return false;

            return TableHelper.RowExists(Member.TableName, Member.Columns.Email, Email);
        }

        #region Status

        public bool IsRegistered { get { return this.IsInDatabase && Status != MemberStatus.Null; } }

        /// <summary>
        /// Registers this member instance. All conditions are already checked
        /// </summary>
        /// <param name="status"></param>
        public void Register(MemberStatus status)
        {
            //Add to stats first
            var stat = new Prem.PTC.Statistics.Statistics(Prem.PTC.Statistics.StatisticsType.NewMembers);
            stat.AddToData1(1);
            stat.Save();
            
            bool isUpToDate = IsUpToDate;

            Registered = DateTime.Now;
            FirstActiveDayOfAdPacks = DateTime.Now;
            Membership = Memberships.Membership.Standard;
            Status = status;
            AvatarUrl = AppSettings.Misc.DefaultAvatarUrl;
            RegisteredWithIP = IP.Current;
            BypassSecurityCheck = global::BypassSecurityCheck.No;
            SelectedCaptchaType = AppSettings.Captcha.Type;

            //IpGeoLocation
            IpGeolocationInfo geolocationInfo = IpGeolocation.GetInfo(IP.Current);
            if (geolocationInfo != null)
            {
                RegisteredLatitude = geolocationInfo.Latitude;
                RegisteredLongitude = geolocationInfo.Longitude;
            }

            //Default fields
            this.TransferPermission = global::TransferFundsPermission.AllowAll;
            this.CPAOfferCompletedBehavior = CPACompletedBehavior.PopupOnScreen;
            this.ShoutboxPrivacyPermission = ShoutboxPermission.NoRestrictions;

            this.StatsDirectReferralsPointsEarned = "0#0#0#0#0#0#0";
            this.StatsDirectReferralsEarned = "0#0#0#0#0#0#0";
            this.StatsPointsEarned = "0#0#0#0#0#0#0";
            this.StatsDRAdPacksEarned = "0#0#0#0#0#0#0";
            this.StatsCashLinksEarned = "0#0#0#0#0#0#0";
            this.StatsDRCashLinksEarned = "0#0#0#0#0#0#0";

            LeadershipResetTime = DateTime.Now;
            LeadershipLevelId = -1;
            Save();
        }

        public bool HasReferer
        {
            get
            {
                if (ReferrerId == -1)
                    return false;
                return true;
            }
        }

        public void SaveTrafficGridData()
        {
            SaveTrafficGridData(this.IsUpToDate);
        }

        private void SaveTrafficGridData(bool isUpToDate)
        {
            PropertyBuilder<Member> builder = new PropertyBuilder<Member>();
            builder
                .Append(x => x.TrafficGridHitsToday)
                .Append(x => x.TrafficGridTotalWons);

            SavePartially(isUpToDate, builder.Build());
        }

        public void Login(bool IsMasterLogin = false)
        {
            if (!IsMasterLogin)
            {
                LastLogged = DateTime.Now;
                if (Name != "admin")
                    LastUsedIP = IP.Current;
                UserAgentInformation = AntiCheatSystem.GetUserInformation();

                //Country update
                CountryInformation CIService = new CountryInformation(IP.Current);
                this.Country = CIService.CountryName;
                this.CountryCode = CIService.CountryCode;

                //IPHistoryLog
                IPHistoryLog.RecordLogin(this, IsMasterLogin);
            }
        }

        public void Logout(HttpResponse response)
        {

        }

        #region AuthHelpers

        [Obsolete]
        public static bool IsSomebodyLoggedIn(System.Web.HttpContext context)
        {
            if (AppSettings.Authentication.AnonymousMemberEnabled)
                return true;

            if (string.IsNullOrEmpty(context.User.Identity.Name) || CurrentId == 0)
                return false;
            return true;
        }

        /// <summary>
        /// Very fast (no DB connection)
        /// </summary>
        public static bool IsLogged
        {
            get
            {
                return Member.IsSomebodyLoggedIn(HttpContext.Current);
            }
        }

        [Obsolete]
        public static Member GetLoggedMember(HttpContext context)
        {
            int currentId = Member.CurrentId;
            Member target = new Member(currentId);

            context.Cache.Insert(currentId.ToString(), target, null, DateTime.Now.AddSeconds(10), Cache.NoSlidingExpiration,
                CacheItemPriority.AboveNormal, null);

            return target;
        }

        [Obsolete]
        public static Member Logged(HttpContext context)
        {
            return GetLoggedMember(context);
        }

        /// <summary>
        /// Returns currently logged in member (refresh)
        /// </summary>
        public static Member Current
        {
            get
            {
                return Member.GetLoggedMember(HttpContext.Current);
            }
        }

        /// <summary>
        /// Returns currently logged in member (from cache)
        /// </summary>
        public static Member CurrentInCache
        {
            get
            {
                if (HttpContext.Current.Cache[Member.CurrentId.ToString()] != null)
                    return (Member)HttpContext.Current.Cache[Member.CurrentId.ToString()];

                return Member.Current;
            }
        }

        [Obsolete]
        public static void RefreshCurrentCache()
        {
        }

        /// <summary>
        /// Very fast (no DB connection)
        /// </summary>
        public static string CurrentName
        {
            get
            {
                if (AppSettings.Authentication.AnonymousMemberEnabled)
                    return "Anonymous";

                string userName = string.Empty;

                if (HttpContext.Current.Request.IsAuthenticated)
                {
                    userName = HttpContext.Current.User.Identity.Name;
                }

                return userName;
            }
        }

        /// <summary>
        /// Very fast (no DB connection)
        /// </summary>
        public static int CurrentId
        {
            get
            {
                if (AppSettings.Authentication.AnonymousMemberEnabled)
                    return Convert.ToInt32(HttpContext.Current.Application["AnonymousMemberId"]);

                int userId = 0;

                if (HttpContext.Current.Request.IsAuthenticated)
                {
                    var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                    if (authCookie != null)
                    {
                        var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                        userId = Convert.ToInt32(authTicket.UserData);
                    }
                }

                return userId;
            }
        }

        #endregion AuthHelpers

        public void ReloadBalances()
        {
            PropertyInfo[] balancePropertiesToReload = buildBalances();

            ReloadPartially(IsUpToDate, balancePropertiesToReload);
        }

        public void SaveBalances()
        {
            PropertyInfo[] balancePropertiesToSave = buildBalances();
            //
            SavePartially(IsUpToDate, balancePropertiesToSave);
        }

        public void SaveStatClicks()
        {
            PropertyBuilder<Member> builder = new PropertyBuilder<Member>();

            builder.Append(x => x.StatsClicks);
            PropertyInfo[] balancePropertiesToSave = builder.Build();
            //
            SavePartially(IsUpToDate, balancePropertiesToSave);
        }

        public void UpdateLastActivityTime()
        {
            LastActivityTime = DateTime.Now;
            SaveLastActivityTime();
        }

        private void SaveLastActivityTime()
        {
            PropertyInfo[] balancePropertiesToSave = buildLastActivityTime();
            //
            SavePartially(IsUpToDate, balancePropertiesToSave);
        }

        private PropertyInfo[] buildLastActivityTime()
        {
            PropertyBuilder<Member> builder = new PropertyBuilder<Member>();

            builder.Append(x => x.LastActivityTime);

            return builder.Build();
        }

        private PropertyInfo[] buildBalances()
        {
            PropertyBuilder<Member> builder = new PropertyBuilder<Member>();

            builder.Append(x => x.MainBalance)
                   .Append(x => x.TrafficBalance)
                   .Append(x => x.PurchaseBalance)
                   .Append(x => x.CommissionBalance)
                   .Append(x => x.PointsToday)
                   .Append(x => x.PointsBalance)
                   .Append(x => x.StatsClicks)
                   .Append(x => x.StatsEarned)
                   .Append(x => x.UserClicksStats)
                   .Append(x => x.TotalEarned)
                   .Append(x => x.TotalPointsGenerated)
                   .Append(x => x.TotalPointsExchanged)
                   .Append(x => x.RawDirectReferralsClicks)
                   .Append(x => x.RawRentedReferralsClicks)
                   .Append(x => x.TotalClicks)
                   .Append(x => x.PTCCredits)
                   .Append(x => x.CashBalance)
                   .Append(x => x.LoginAdsCredits)
                   .Append(x => x.InvestmentBalance)
                   .Append(x => x.MarketplaceBalance);

            return builder.Build();
        }

        private PropertyInfo[] BuildCRONInformation()
        {
            PropertyBuilder<Member> builder = new PropertyBuilder<Member>();

            builder.Append(x => x.StatsClicks)
                   .Append(x => x.StatsEarned)
                   .Append(x => x.AccountStatus)
                   .Append(x => x.AccountStatusInt)
                   .Append(x => x.UserClicksStats)
                   .Append(x => x.MembershipId)
                   .Append(x => x.MembershipName)
                   .Append(x => x.MembershipWhen)
                   .Append(x => x.MembershipExpires);
            return builder.Build();
        }

        #endregion Status

        #region PIN

        public static string PINRegexPattern
        {
            get { return "^\\d{4}$"; }
        }

        #endregion PIN

        public static string PasswordRegexPattern
        {
            get { return "[a-zA-Z0-9]{4,81}"; }
        }

        /// <summary>
        /// Not connected to Status (Status may be Active)
        /// </summary>
        public bool IsInactive
        {
            get
            {
                if (this.LastActivityTime.HasValue && ((DateTime)this.LastActivityTime).AddDays(AppSettings.VacationAndInactivity.DaysToInactivityCharge) < DateTime.Now)
                    return true;
                return false;
            }
        }

        public static string GetAdminUsernameLink(string username, bool IsFromMembersFolder = false)
        {
            string rep = IsFromMembersFolder? String.Empty : "../Members/";
            return string.Format("<a href=\"{0}MemberList.aspx?un={1}\">{1}</a>", rep, username);
        }

        public static string GetAdminUsernameLink(int userId, bool IsFromMembersFolder = false)
        {
            var username = TableHelper.SelectScalar(String.Format("SELECT Username FROM Users WHERE UserId = {0}", userId)).ToString();
            return GetAdminUsernameLink(username, IsFromMembersFolder);
        }

        public bool IsRepresentative()
        {
            var activeRepresentatives = Representative.GetAllActive();

            foreach (var activeRepresentative in activeRepresentatives)
                if (activeRepresentative.UserId == this.Id)
                    return true;

            return false;
        }

        /// <summary>
        /// Handle offerwall referral crediting
        /// Exception-proof
        /// REQUIRE FULL MEMBER SAVE
        /// </summary>
        /// <param name="BasePoints"></param>
        public void TryCreditTheReferer(int BasePoints)
        {
            try
            {
                if (this.HasReferer)
                {
                    //Calculate the points amount
                    Member PointReferer = new Member(this.ReferrerId);
                    double PointsToCreditDouble = ((double)PointReferer.Membership.RefPercentEarningsOfferwalls / 100.0) * BasePoints;
                    int PointsToCredit = Convert.ToInt32(PointsToCreditDouble);

                    if (this.IsRented)
                    {
                        //Rented
                        PointReferer.AddToPointsBalance(PointsToCredit, "Offerwall /ref/", BalanceLogType.Other);
                        PointReferer.SaveBalances();

                        this.PointsEarnedToReferer += PointsToCredit;
                        this.LastPointableActivity = DateTime.Now;
                    }
                    else
                    {
                        //Direct
                        PointReferer.AddToPointsBalance(PointsToCredit, "Offerwall /ref/", BalanceLogType.Other);
                        PointReferer.SaveBalances();

                        this.PointsEarnedToReferer += PointsToCredit;
                        this.LastPointableActivity = DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
        }

        /// <summary>
        /// Allow user to log in with Facebook.
        /// User will be banned if another user is connected with the same Facebook account.
        /// </summary>
        /// <param name="fbUser"></param>
        public void ConnectWithFacebook(FacebookMember fbUser)
        {
            FacebookOAuthId = fbUser.FacebookId;
            FacebookName = fbUser.Name;
            Save();
            
            AntiCheatSystem.AfterFacebookLogin(this);
        }
        
        public void IncreaseFailedPINAttemptCount()
        {
            FailedPINAttemptCount++;
            if (FailedPINAttemptCount == 15)
                Lock("Too many failed PIN attempts (15+)");
        }

        public bool IsAnyBalanceIsNegative()
        {
            var zero = Money.Zero; 
            var cryptoList = CryptocurrencyFactory.GetAllAvailable();

            foreach(var crypto in cryptoList)            
                if (crypto.WalletEnabled && GetCryptocurrencyBalance(crypto.Type) < zero)
                    return true;            

            if (MainBalance < zero || PurchaseBalance < zero || CommissionBalance < zero || CashBalance < zero || MarketplaceBalance < zero || PointsBalance < 0)
                return true;

            return false;
        }

        public static void UpdateMembershipName(int membershipId, string name)
        {
            var query = string.Format("UPDATE {0} SET {1} = '{2}' WHERE {3} = {4}",
                TableName, Columns.MembershipName, name, Columns.MembershipId, membershipId);

            TableHelper.ExecuteRawCommandNonQuery(query);
        }
    }
}