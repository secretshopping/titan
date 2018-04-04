using System;
using System.Configuration;
using Prem.PTC.Advertising;
using Prem.PTC.Utils;
using System.Reflection;
using System.Web;
using System.Diagnostics;

namespace Prem.PTC
{
    public static partial class AppSettings
    {

        #region AllTheData

        /// <summary>
        /// Forbids operations to be performed in script demo version
        /// Throws MsgException with proper info (if demo script)
        /// </summary>
        public static void DemoCheck()
        {
            if (IsDemo)
                throw new MsgException(Resources.L1.DEMOCHECK);
        }


        public static string PointsName
        {
            get
            {
                return AppSettings.Payments.CurrencyPointsName;
            }
        }

        /// <summary>
        /// Decides wheather this is demo script or not
        /// </summary>
        /// <returns></returns>
        public static bool IsDemo
        {
            get
            {
                if (HttpContext.Current == null)
                    return false;

                if (Side == ScriptSide.AdminPanel && HttpContext.Current.User.Identity.Name != null
                    && HttpContext.Current.User.Identity.Name == "demo.usetitan.com")
                {
                    return true;
                }
                else if (Side == ScriptSide.Client && HttpContext.Current.Application["IsDemoScript"] != null)
                {
                    return (bool)HttpContext.Current.Application["IsDemoScript"];
                }

                return false;
            }
        }

        /// <summary>
        /// Decides wheather this is AdminPanel or client script
        /// </summary>
        /// <returns></returns>
        public static ScriptSide Side
        {
            get
            {
                if (HttpContext.Current.Application["ScriptSide"] != null)
                    return ((ScriptSide)(int)HttpContext.Current.Application["ScriptSide"]);

                return ScriptSide.Client;
            }
        }

        /// <summary>
        /// Check wheather App is offline or not
        /// If app is set offline it means that CRON is running
        /// </summary>
        public static bool IsOffline
        {
            get
            {
                if (HttpContext.Current.Application["IsOffline"] != null)
                {
                    return (bool)HttpContext.Current.Application["IsOffline"];
                }
                return false;
            }

            set
            {
                HttpContext.Current.Application["IsOffline"] = value;
            }
        }


        public static class SSL
        {
            public static SSLType Type
            {
                get { return (SSLType)appSettings.SSLType; }

                set { appSettings.SSLType = (int)value; }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadSSL();
            }

            public static void Save()
            {
                appSettings.SaveSSL();
            }
        }

        public static class TrafficGrid
        {
            public static Money Limit
            {
                get { return appSettings.TrafficGridLimit; }

                set { appSettings.TrafficGridLimit = value; }
            }

            public static int P_MainBalance
            {
                get { return appSettings.TrafficGridPrize1; }

                set { appSettings.TrafficGridPrize1 = value; }
            }

            public static int P_RentalBalance
            {
                get { return appSettings.TrafficGridPrize2; }

                set { appSettings.TrafficGridPrize2 = value; }
            }

            public static int P_AdBalance
            {
                get { return appSettings.TrafficGridPrize3; }

                set { appSettings.TrafficGridPrize3 = value; }
            }

            public static int P_Points
            {
                get { return appSettings.TrafficGridPrize4; }

                set { appSettings.TrafficGridPrize4 = value; }
            }

            public static int P_RentedReferral
            {
                get { return appSettings.TrafficGridPrize5; }

                set { appSettings.TrafficGridPrize5 = value; }
            }

            public static int P_DirectReferralLimit
            {
                get { return appSettings.TrafficGridPrize6; }

                set { appSettings.TrafficGridPrize6 = value; }
            }

            public static string Image1
            {
                get { return appSettings.TrafficGridImage1; }

                set { appSettings.TrafficGridImage1 = value; }
            }

            public static string Image2
            {
                get { return appSettings.TrafficGridImage2; }

                set { appSettings.TrafficGridImage2 = value; }
            }

            public static string Image3
            {
                get { return appSettings.TrafficGridImage3; }

                set { appSettings.TrafficGridImage3 = value; }
            }

            public static string Image4
            {
                get { return appSettings.TrafficGridImage4; }

                set { appSettings.TrafficGridImage4 = value; }
            }

            public static Money TrafficGridDailyMoneyLeft
            {
                get { return appSettings.TrafficGridDailyMoneyLeft; }

                set { appSettings.TrafficGridDailyMoneyLeft = value; }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadTrafficGrid();
            }

            public static void Save()
            {
                appSettings.SaveTrafficGrid();
            }
        }


        public static class Misc
        {
            public static string DefaultAvatarUrl
            {
                get { return appSettings.DefaultAvatarUrl; }
                set
                {
                    if (value == null) throw new ArgumentNullException("DefaultAvatarUrl");

                    appSettings.DefaultAvatarUrl = value;

                }
            }

            public static string BannerLocation1
            {
                get { return appSettings.BannerLocation1; }
                set
                {
                    if (value == null) throw new ArgumentNullException("BannerLocation1");

                    appSettings.BannerLocation1 = value;

                }
            }

            public static string BannerLocation2
            {
                get { return appSettings.BannerLocation2; }
                set
                {
                    if (value == null) throw new ArgumentNullException("BannerLocation2");

                    appSettings.BannerLocation2 = value;

                }
            }

            public static DateTime LastCRONRun
            {
                get { return appSettings.LastCRONRun; }
                set
                {
                    if (value == null) throw new ArgumentNullException("LastCRONRun");

                    appSettings.LastCRONRun = value;

                }
            }

            public static DateTime PlannedCronRun
            {
                get { return appSettings.PlannedCronRun; }
                set
                {
                    if (value == null) throw new ArgumentNullException("PlannedCronRun");

                    appSettings.PlannedCronRun = value;

                }
            }

            public static int DaysToInactivity
            {
                get { return appSettings.DaysToInactivity; }
                set
                {
                    if (value == null) throw new ArgumentNullException("DaysToInactivity");

                    appSettings.DaysToInactivity = value;

                }
            }

            public static int ServerTimeDifference
            {
                get { return appSettings.ServerTimeDifference; }
                set
                {
                    if (value == null) throw new ArgumentNullException("ServerTimeDifference");

                    appSettings.ServerTimeDifference = value;

                }
            }


            public static bool IsTransferPointsToMainBalanceEnabled
            {
                get { return appSettings.IsTransferPointsToMainBalanceEnabled; }
                set
                {
                    if (value == null) throw new ArgumentNullException("IsTransferPointsToMainBalanceEnabled");

                    appSettings.IsTransferPointsToMainBalanceEnabled = value;

                }
            }

            [Obsolete]
            public static bool AreContestsEnabled
            {
                get { return true; }
                set
                {
                    if (value == null) throw new ArgumentNullException("AreContestsEnabled");

                    appSettings.AreContestsEnabled = value;

                }
            }

            /// <summary>
            /// Deafult = false. If set to true than user is only earning money from referrals from Standard and Extended ad types. 
            /// And user is required to view X Standard ads in order to get money from referrals tomorrow
            /// </summary>
            public static bool ExposureRefEarningsEnabled
            {
                get { return appSettings.ExposureRefEarningsEnabled; }
                set
                {
                    if (value == null) throw new ArgumentNullException("ExposureRefEarningsEnabled");

                    appSettings.ExposureRefEarningsEnabled = value;

                }
            }

            [Obsolete]
            public static bool SpilloverEnabled
            {
                get { return appSettings.SpilloverEnabled; }
                set
                {
                    appSettings.SpilloverEnabled = value;

                }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadMisc();
            }

            public static void Save()
            {
                appSettings.SaveMisc();
            }
        }


        public static class Email
        {
            /// <exception cref="ArgumentNullException" />
            public static string NoReply
            {
                get { return appSettings.NoReplyEmail; }
                set
                {
                    if (value == null) throw new ArgumentNullException("NoReplyEmail");

                    appSettings.NoReplyEmail = value;
                }

            }

            /// <exception cref="ArgumentNullException" />
            public static string Forward
            {
                get { return appSettings.ForwardEmail; }
                set
                {
                    if (value == null) throw new ArgumentNullException("ForwardEmail");
                    appSettings.ForwardEmail = value;
                }
            }

            /// <exception cref="ArgumentNullException" />
            public static string Host
            {
                get { return appSettings.EmailHost; }
                set
                {
                    if (value == null) throw new ArgumentNullException("EmailHost");

                    appSettings.EmailHost = value;
                }

            }

            /// <exception cref="ArgumentNullException" />
            public static string Username
            {
                get { return appSettings.EmailUsername; }
                set
                {
                    if (value == null) throw new ArgumentNullException("EmailUsername");

                    appSettings.EmailUsername = value;
                }

            }

            /// <exception cref="ArgumentNullException" />
            public static string Password
            {
                get { return appSettings.EmailPassword; }
                set
                {
                    if (value == null) throw new ArgumentNullException("EmailPassword");

                    appSettings.EmailPassword = value;
                }

            }

            /// <exception cref="ArgumentNullException" />
            public static int Port
            {
                get { return appSettings.EmailPort; }
                set
                {
                    if (value == null) throw new ArgumentNullException("EmailPort");

                    appSettings.EmailPort = value;
                }

            }

            public static bool IsSecureMail
            {
                get { return appSettings.IsSecureMail; }
                set
                {
                    if (value == null) throw new ArgumentNullException("IsSecureMail");

                    appSettings.IsSecureMail = value;
                }

            }


            public static void Save()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.SaveEmail();
            }

            public static void Reload()
            {
                appSettings.ReloadEmail();
            }
        }


        public static class Admin
        {
            /// <exception cref="ArgumentNullException" />
            public static string Name
            {
                get { return appSettings.AdminName; }
                set
                {
                    if (value == null) throw new ArgumentNullException("AdminName");

                    appSettings.AdminName = value;

                }
            }
        }


    

        public static class RentedReferrals
        {
            public static bool IsDeletingEnabled
            {
                get { return appSettings.IsRentedReferralsDeletingEnabled; }
                set { appSettings.IsRentedReferralsDeletingEnabled = value; }
            }

            public static bool IsRecyclingEnabled
            {
                get { return appSettings.IsRentedReferralsRecyclingEnabled; }
                set { appSettings.IsRentedReferralsRecyclingEnabled = value; }
            }

            /// <exception cref="ArgumentOutOfRangeException">
            public static int MinPackageCount
            {
                get { return appSettings.RentedReferralsMinPackage; }
                set
                {
                    if (value < 0) throw new ArgumentOutOfRangeException("MinPackageCount");
                    appSettings.RentedReferralsMinPackage = value;
                }
            }

            /// <exception cref="ArgumentOutOfRangeException">
            public static int PackagesCount
            {
                get { return appSettings.RentedReferralsPackagesCount; }
                set
                {
                    if (value < 0) throw new ArgumentOutOfRangeException("PackagesCount");
                    appSettings.RentedReferralsPackagesCount = value;
                }
            }

            /// <exception cref="ArgumentOutOfRangeException">
            public static int PackageMultipler
            {
                get { return appSettings.RentedReferralsPackageMultipler; }
                set
                {
                    if (value < 1) throw new ArgumentOutOfRangeException("PackageMultipler");
                    appSettings.RentedReferralsPackageMultipler = value;
                }
            }

            public static TimeSpan CanBeRentedFor
            {
                get { return TimeSpan.FromDays(appSettings.RentedReferralsRentalTimeDays); }
                set { appSettings.RentedReferralsRentalTimeDays = (int)value.TotalDays; }
            }

            public static TimeSpan MinLastClickingActivity
            {
                get { return TimeSpan.FromDays(appSettings.RentedReferralsMinLastDaysClickingActivity); }
                set { appSettings.RentedReferralsMinLastDaysClickingActivity = (int)value.TotalDays; }
            }

            /// <summary>
            /// "Minimalna liczba dni które musi mieć rented referral, żeby móc na nim uruchomić AutoPay."
            /// </summary>
            public static int MinDaysToStartAutoPay
            {
                get { return appSettings.RentedReferralsMinDaysToAutoPay; }
                set { appSettings.RentedReferralsMinDaysToAutoPay = value; }
            }

            public static void Save()
            {
                appSettings.SaveRentedReferrals();
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadRentedReferrals();
            }
        }


        public static class BotReferrals
        {
            /// <exception cref="ArgumentOutOfRangeException">
            public static int InactiveBotPercentage
            {
                get { return appSettings.BotReferralsInactiveBotPercentage; }
                set
                {
                    if (value < 0 && value > 100) throw new ArgumentOutOfRangeException("InactiveBotPercentage");
                    appSettings.BotReferralsInactiveBotPercentage = value;
                }
            }

            /// <exception cref="ArgumentOutOfRangeException">
            public static int BotQualityIndex
            {
                get { return appSettings.BotReferralsBotQualityIndex; }
                set
                {
                    if (value < 10) throw new ArgumentOutOfRangeException("BotQualityIndex");
                    appSettings.BotReferralsBotQualityIndex = value;
                }
            }

            public static void Save()
            {
                appSettings.SaveBotReferrals();
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadBotReferrals();
            }
        }

        public static class Memberships
        {
            /// <exception cref="ArgumentNullException" />
            public static Money TenPointsValue
            {
                get { return appSettings.MembershipTenPointsValue; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("TenPointsValue");

                    appSettings.MembershipTenPointsValue = value;
                }
            }

            /// <exception cref="ArgumentOutOfRangeException" />
            public static int UpgradePointsDiscount
            {
                get { return appSettings.MembershipUpgradePointsDiscount; }
                set
                {
                    if (value < 0 || value > 100)
                        throw new ArgumentOutOfRangeException("UpgradePointsDiscount");

                    appSettings.MembershipUpgradePointsDiscount = value;
                }
            }

            public static void Save()
            {
                appSettings.SaveMemberships();
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadMemberships();
            }
        }

        public static class FolderPaths
        {
            public static string BannerAdvertImages
            {
                get { return ConfigurationManager.AppSettings["BannerAdvertImagesFolderPath"] as string; }
            }

            public static string PostImages
            {
                get { return ConfigurationManager.AppSettings["PostImagesFolderPath"] as string; }
            }

            public static string LogoImages
            {
                get { return ConfigurationManager.AppSettings["LogoImagesFolderPath"] as string; }
            }

            public static string BannerImages
            {
                get { return ConfigurationManager.AppSettings["BannerImagesFolderPath"] as string; }
            }

            public static string AchievementImages
            {
                get { return ConfigurationManager.AppSettings["AchievmentImagesFolderPath"] as string; }
            }
        }


        public static class TableNames
        {
            public static string AppSettings
            {
                get { return ConfigurationManager.AppSettings["AppSettingsTableName"] as string; }
            }

            public static string Memberships
            {
                get { return ConfigurationManager.AppSettings["MembershipsTableName"] as string; }
            }

            public static string MembershipPacks
            {
                get { return ConfigurationManager.AppSettings["MembershipPacksTableName"] as string; }
            }

            public static string PtcAdverts
            {
                get { return ConfigurationManager.AppSettings["PtcAdvertsTableName"] as string; }
            }

            public static string PtcAdvertCategories
            {
                get { return ConfigurationManager.AppSettings["PtcAdvertCategoriesTableName"] as string; }
            }

            public static string PtcAdvertGeolocations
            {
                get { return ConfigurationManager.AppSettings["PtcAdvertGeolocationsTableName"] as string; }
            }

            public static string PtcAdvertPacks
            {
                get { return ConfigurationManager.AppSettings["PtcAdvertPacksTableName"] as string; }
            }

            public static string BannerAdverts
            {
                get { return ConfigurationManager.AppSettings["BannerAdvertsTableName"] as string; }
            }

            public static string BannerAdvertGeolocations
            {
                get { return ConfigurationManager.AppSettings["BannerAdvertGeolocations"] as string; }
            }

            public static string BannerAdvertPacks
            {
                get { return ConfigurationManager.AppSettings["BannerAdvertPacksTableName"] as string; }
            }

            public static string SupportTickets
            {
                get { return ConfigurationManager.AppSettings["SupportTicketsTableName"] as string; }
            }

            public static string Members
            {
                get { return ConfigurationManager.AppSettings["UsersTableName"] as string; }
            }

            public static string PayPalGateways
            {
                get { return ConfigurationManager.AppSettings["PayPalGatewaysTableName"] as string; }
            }

            public static string PayzaGateways
            {
                get { return ConfigurationManager.AppSettings["PayzaGatewaysTableName"] as string; }
            }

            public static string LibertyReserveGateways
            {
                get { return ConfigurationManager.AppSettings["LibertyReserveGatewaysTableName"] as string; }
            }

            public static string Texts
            {
                get { return ConfigurationManager.AppSettings["TextsTableName"] as string; }
            }

            public static string TextFragments
            {
                get { return ConfigurationManager.AppSettings["TextFragmentsTableName"] as string; }
            }

            public static string Messages
            {
                get { return ConfigurationManager.AppSettings["MessagesTableName"] as string; }
            }
        }


        public static class HandlersPaths
        {
            public static string Payza
            {
                get { return ConfigurationManager.AppSettings["PayzaHandlerPath"] as string; }
            }
        }


        internal partial class AppSettingsTable : BaseTableObject
        {
            public override Database Database { get { return Database.Client; } }
            public static new string TableName { get { return TableNames.AppSettings; } }
            protected override string dbTable { get { return TableName; } }

            #region Columns

            [Column("ApplicationSettingsId", IsPrimaryKey = true)]
            public override int Id { get { return _id; } protected set { _id = value; } }

            [Column("AdminName")]
            internal string AdminName { get { return _adminName; } set { _adminName = value; SetUpToDateAsFalse(); } }

            [Column("NoReplyEmail")]
            internal string NoReplyEmail { get { return _noReplyEmail; } set { _noReplyEmail = value; SetUpToDateAsFalse(); } }

            [Column("ForwardEmail")]
            internal string ForwardEmail { get { return _forwardEmail; } set { _forwardEmail = value; SetUpToDateAsFalse(); } }

            [Column("PointsBalanceName")]
            internal string PointsBalanceName { get { return _pointsBalanceName; } set { _pointsBalanceName = value; SetUpToDateAsFalse(); } }

            [Column("RentedRefsDeleting")]
            internal bool IsRentedReferralsDeletingEnabled { get { return _rentedReferralsDeletingEnabled; } set { _rentedReferralsDeletingEnabled = value; SetUpToDateAsFalse(); } }

            [Column("RentedRefsRecycling")]
            internal bool IsRentedReferralsRecyclingEnabled { get { return _rentedReferralsRecyclingEnabled; } set { _rentedReferralsRecyclingEnabled = value; SetUpToDateAsFalse(); } }

            [Column("RentedRefsMinPackage")]
            internal int RentedReferralsMinPackage { get { return _rentedReferralsMinPackage; } set { _rentedReferralsMinPackage = value; SetUpToDateAsFalse(); } }

            [Column("RentedRefsPackagesCount")]
            internal int RentedReferralsPackagesCount { get { return _rentedReferralsPackagesCount; } set { _rentedReferralsPackagesCount = value; SetUpToDateAsFalse(); } }

            [Column("RentedRefsPackageMultipler")]
            internal int RentedReferralsPackageMultipler { get { return _rentedReferralsPackageMultipler; } set { _rentedReferralsPackageMultipler = value; SetUpToDateAsFalse(); } }

            [Column("RentedRefsRentalTimeDays")]
            internal int RentedReferralsRentalTimeDays { get { return _rentedReferralsRentalTimeDays; } set { _rentedReferralsRentalTimeDays = value; SetUpToDateAsFalse(); } }

            [Column("RentedRefsMinLastDaysClickingActivity")]
            internal int RentedReferralsMinLastDaysClickingActivity { get { return _rentedReferralsMinLastDaysClickingActivity; } set { _rentedReferralsMinLastDaysClickingActivity = value; SetUpToDateAsFalse(); } }

            [Column("RentedRefsMinDaysToAutoPay")]
            internal int RentedReferralsMinDaysToAutoPay { get { return _rentedReferralsMinDaysToAutoPay; } set { _rentedReferralsMinDaysToAutoPay = value; SetUpToDateAsFalse(); } }

            [Column("DefaultAvatarUrl")]
            internal string DefaultAvatarUrl { get { return _dau; } set { _dau = value; SetUpToDateAsFalse(); } }

            [Column("BotRefsInactiveBotPercentage")]
            internal int BotReferralsInactiveBotPercentage { get { return _botReferralsInactiveBotPercentage; } set { _botReferralsInactiveBotPercentage = value; SetUpToDateAsFalse(); } }

            [Column("BotRefsBotQualityIndex")]
            internal int BotReferralsBotQualityIndex { get { return _botReferralsBotQualityIndex; } set { _botReferralsBotQualityIndex = value; SetUpToDateAsFalse(); } }

            [Column("MembershipTenPointsValue")]
            internal Money MembershipTenPointsValue { get { return _membershipTenPointsValue; } set { _membershipTenPointsValue = value; SetUpToDateAsFalse(); } }

            [Column("MembershipUpgradePointsDiscount")]
            internal int MembershipUpgradePointsDiscount { get { return _membershipUpgradePointsDiscount; } set { _membershipUpgradePointsDiscount = value; SetUpToDateAsFalse(); } }

            [Column("DaysToInactivity")]
            internal int DaysToInactivity { get { return _DaysToInactivity; } set { _DaysToInactivity = value; SetUpToDateAsFalse(); } }

            [Column("ServerTimeDifference")]
            internal int ServerTimeDifference { get { return _ServerTimeDifference; } set { _ServerTimeDifference = value; SetUpToDateAsFalse(); } }

            [Column("EmailHost")]
            internal string EmailHost { get { return _EmailHost; } set { _EmailHost = value; SetUpToDateAsFalse(); } }

            [Column("EmailUsername")]
            internal string EmailUsername { get { return _EmailUsername; } set { _EmailUsername = value; SetUpToDateAsFalse(); } }

            [Column("EmailPassword")]
            internal string EmailPassword { get { return _EmailPassword; } set { _EmailPassword = value; SetUpToDateAsFalse(); } }

            [Column("EmailPort")]
            internal int EmailPort { get { return _EmailPort; } set { _EmailPort = value; SetUpToDateAsFalse(); } }

            [Column("LastCRONRun")]
            internal DateTime LastCRONRun { get { return _LastCRONRun; } set { _LastCRONRun = value; SetUpToDateAsFalse(); } }

            [Column("PlannedCronRun")]
            internal DateTime PlannedCronRun { get { return _PlannedCronRun; } set { _PlannedCronRun = value; SetUpToDateAsFalse(); } }

            [Column("SSLType")]
            internal int SSLType { get { return _SSLType; } set { _SSLType = value; SetUpToDateAsFalse(); } }


            //This column can be deleted. See the ReferralBanners table.
            [Column("BannerLoc1")]
            internal string BannerLocation1 { get { return _BannerLocation1; } set { _BannerLocation1 = value; SetUpToDateAsFalse(); } }

            [Column("BannerLoc2")]
            internal string BannerLocation2 { get { return _BannerLocation2; } set { _BannerLocation2 = value; SetUpToDateAsFalse(); } }

            //TrafficGrid

            [Column("TrafficGridLimit")]
            internal Money TrafficGridLimit { get { return _TrafficGridLimit; } set { _TrafficGridLimit = value; SetUpToDateAsFalse(); } }

            [Column("TrafficGridDailyMoneyLeft")]
            internal Money TrafficGridDailyMoneyLeft { get { return _TrafficGridDailyMoneyLeft; } set { _TrafficGridDailyMoneyLeft = value; SetUpToDateAsFalse(); } }

            [Column("TrafficGridPrize1")]
            internal int TrafficGridPrize1 { get { return _TrafficGridPrize1; } set { _TrafficGridPrize1 = value; SetUpToDateAsFalse(); } }

            [Column("TrafficGridPrize2")]
            internal int TrafficGridPrize2 { get { return _TrafficGridPrize2; } set { _TrafficGridPrize2 = value; SetUpToDateAsFalse(); } }

            [Column("TrafficGridPrize3")]
            internal int TrafficGridPrize3 { get { return _TrafficGridPrize3; } set { _TrafficGridPrize3 = value; SetUpToDateAsFalse(); } }

            [Column("TrafficGridPrize4")]
            internal int TrafficGridPrize4 { get { return _TrafficGridPrize4; } set { _TrafficGridPrize4 = value; SetUpToDateAsFalse(); } }

            [Column("TrafficGridPrize5")]
            internal int TrafficGridPrize5 { get { return _TrafficGridPrize5; } set { _TrafficGridPrize5 = value; SetUpToDateAsFalse(); } }

            [Column("TrafficGridPrize6")]
            internal int TrafficGridPrize6 { get { return _TrafficGridPrize6; } set { _TrafficGridPrize6 = value; SetUpToDateAsFalse(); } }

            [Column("TrafficGridImage1")]
            internal string TrafficGridImage1 { get { return _TrafficGridImage1; } set { _TrafficGridImage1 = value; SetUpToDateAsFalse(); } }

            [Column("TrafficGridImage2")]
            internal string TrafficGridImage2 { get { return _TrafficGridImage2; } set { _TrafficGridImage2 = value; SetUpToDateAsFalse(); } }

            [Column("TrafficGridImage3")]
            internal string TrafficGridImage3 { get { return _TrafficGridImage3; } set { _TrafficGridImage3 = value; SetUpToDateAsFalse(); } }

            [Column("TrafficGridImage4")]
            internal string TrafficGridImage4 { get { return _TrafficGridImage4; } set { _TrafficGridImage4 = value; SetUpToDateAsFalse(); } }
            
            [Column("IsTransferPointsToMainBalanceEnabled")]
            internal bool IsTransferPointsToMainBalanceEnabled { get { return _IsTransferPointsToMainBalanceEnabled; } set { _IsTransferPointsToMainBalanceEnabled = value; SetUpToDateAsFalse(); } }

            [Column("ExposureRefEarningsEnabled")]
            internal bool ExposureRefEarningsEnabled { get { return _ExposureRefEarningsEnabled; } set { _ExposureRefEarningsEnabled = value; SetUpToDateAsFalse(); } }

            [Column("SpilloverEnabled")]
            internal bool SpilloverEnabled { get { return _SpilloverEnabled; } set { _SpilloverEnabled = value; SetUpToDateAsFalse(); } }
           
            [Column("AreContestsEnabled")]
            internal bool AreContestsEnabled { get { return _AreContestsEnabled; } set { _AreContestsEnabled = value; SetUpToDateAsFalse(); } }

            [Column("IsSecureMail")]
            internal bool IsSecureMail { get { return _IsSecureMail; } set { _IsSecureMail = value; SetUpToDateAsFalse(); } }

            private int _id, _rentedReferralsMinPackage, _rentedReferralsPackagesCount, _rentedReferralsPackageMultipler, _rentedReferralsRentalTimeDays,
                _rentedReferralsMinLastDaysClickingActivity, _rentedReferralsMinDaysToAutoPay, _botReferralsInactiveBotPercentage, _botReferralsBotQualityIndex,
                _membershipUpgradePointsDiscount, _EmailPort, _DaysToInactivity, _SSLType, _ServerTimeDifference,
                _TrafficGridPrize1, _TrafficGridPrize2, _TrafficGridPrize3, _TrafficGridPrize4, _TrafficGridPrize5, _TrafficGridPrize6;

            private string _adminName, _noReplyEmail, _forwardEmail, _pointsBalanceName, _dau, _EmailHost, _EmailUsername, _EmailPassword,

                _BannerLocation1, _BannerLocation2,
                _TrafficGridImage1, _TrafficGridImage2, _TrafficGridImage3, _TrafficGridImage4;

            private bool _rentedReferralsDeletingEnabled,
                _rentedReferralsRecyclingEnabled,
                _IsTransferPointsToMainBalanceEnabled, _ExposureRefEarningsEnabled, _AreContestsEnabled,
                _IsSecureMail, _SpilloverEnabled;

            private Money _cashoutLimit1, _cashoutLimit2,
                _cashoutLimit3, _cashoutLimitRest, _membershipTenPointsValue, _TrafficGridLimit, _TrafficGridDailyMoneyLeft;

            private DateTime _LastCRONRun, _PlannedCronRun;

            #endregion Columns


            #region Constructors

            private static readonly int APP_SETTINGS_ID = 1;


            internal AppSettingsTable() : base(APP_SETTINGS_ID) { }

            #endregion Constructors


            #region SSL

            internal void ReloadSSL()
            {
                ReloadPartially(IsUpToDate, buildSSLProperties());
            }

            internal void SaveSSL()
            {
                SavePartially(IsUpToDate, buildSSLProperties());
            }

            private PropertyInfo[] buildSSLProperties()
            {
                var paymentsValues = new PropertyBuilder<AppSettingsTable>();
                paymentsValues
                    .Append(x => x.SSLType);

                return paymentsValues.Build();
            }

            #endregion SSL


            #region TrafficGrid

            internal void ReloadTrafficGrid()
            {
                ReloadPartially(IsUpToDate, buildTrafficGridProperties());
            }

            internal void SaveTrafficGrid()
            {
                SavePartially(IsUpToDate, buildTrafficGridProperties());
            }

            private PropertyInfo[] buildTrafficGridProperties()
            {
                var paymentsValues = new PropertyBuilder<AppSettingsTable>();
                paymentsValues
                    .Append(x => x.TrafficGridImage1)
                    .Append(x => x.TrafficGridImage2)
                    .Append(x => x.TrafficGridImage3)
                    .Append(x => x.TrafficGridImage4)
                    .Append(x => x.TrafficGridLimit)
                    .Append(x => x.TrafficGridPrize1)
                    .Append(x => x.TrafficGridPrize2)
                    .Append(x => x.TrafficGridPrize3)
                    .Append(x => x.TrafficGridPrize4)
                    .Append(x => x.TrafficGridPrize5)
                    .Append(x => x.TrafficGridDailyMoneyLeft)
                    .Append(x => x.TrafficGridPrize6);

                return paymentsValues.Build();
            }

            #endregion TrafficGrid


            #region Misc

            internal void ReloadMisc()
            {
                ReloadPartially(IsUpToDate, buildMiscProperties());
            }

            internal void SaveMisc()
            {
                SavePartially(IsUpToDate, buildMiscProperties());
            }

            private PropertyInfo[] buildMiscProperties()
            {
                var paymentsValues = new PropertyBuilder<AppSettingsTable>();
                paymentsValues
                    .Append(x => x.DefaultAvatarUrl)
                    .Append(x => x.DaysToInactivity)
                    .Append(x => x.PlannedCronRun)
                    .Append(x => x.LastCRONRun)
                    .Append(x => x.IsTransferPointsToMainBalanceEnabled)
                    .Append(x => x.BannerLocation1)
                    .Append(x => x.ExposureRefEarningsEnabled)
                    .Append(x => x.AreContestsEnabled)
                    .Append(x => x.ServerTimeDifference)
                    .Append(x => x.BannerLocation2)
                    .Append(x => x.SpilloverEnabled);
                

                return paymentsValues.Build();
            }

            #endregion Misc


            #region Rented Referrals

            internal void SaveRentedReferrals()
            {
                SavePartially(IsUpToDate, buildRentedReferralsProperties());
            }

            internal void ReloadRentedReferrals()
            {
                ReloadPartially(IsUpToDate, buildRentedReferralsProperties());
            }

            private PropertyInfo[] buildRentedReferralsProperties()
            {
                return new PropertyBuilder<AppSettingsTable>()
                    .Append(x => x.IsRentedReferralsDeletingEnabled)
                    .Append(x => x.IsRentedReferralsRecyclingEnabled)
                    .Append(x => x.RentedReferralsMinPackage)
                    .Append(x => x.RentedReferralsPackagesCount)
                    .Append(x => x.RentedReferralsPackageMultipler)
                    .Append(x => x.RentedReferralsRentalTimeDays)
                    .Append(x => x.RentedReferralsMinLastDaysClickingActivity)
                    .Append(x => x.RentedReferralsMinDaysToAutoPay)
                    .Build();
            }

            #endregion Rented Referrals


            #region Bot referrals

            internal void SaveBotReferrals()
            {
                SavePartially(IsUpToDate, buildBotReferralsProperties());
            }

            internal void ReloadBotReferrals()
            {
                ReloadPartially(IsUpToDate, buildBotReferralsProperties());
            }

            private PropertyInfo[] buildBotReferralsProperties()
            {
                return new PropertyBuilder<AppSettingsTable>()
                    .Append(x => x.BotReferralsInactiveBotPercentage)
                    .Append(x => x.BotReferralsBotQualityIndex)
                    .Build();
            }

            #endregion Bot referrals


            #region Email

            internal void SaveEmail()
            {
                SavePartially(IsUpToDate, buildEmailProperties());
            }

            internal void ReloadEmail()
            {
                ReloadPartially(IsUpToDate, buildEmailProperties());
            }

            private PropertyInfo[] buildEmailProperties()
            {
                return new PropertyBuilder<AppSettingsTable>()
                    .Append(x => x.NoReplyEmail)
                    .Append(x => x.ForwardEmail)
                    .Append(x => x.EmailHost)
                    .Append(x => x.EmailUsername)
                    .Append(x => x.EmailPassword)
                    .Append(x => x.EmailPort)
                    .Append(x => x.IsSecureMail)
                    .Build();
            }

            #endregion Email


            #region Memberships

            internal void SaveMemberships()
            {
                SavePartially(IsUpToDate, buildMembershipsProperties());
            }

            internal void ReloadMemberships()
            {
                ReloadPartially(IsUpToDate, buildMembershipsProperties());
            }

            private PropertyInfo[] buildMembershipsProperties()
            {
                return new PropertyBuilder<AppSettingsTable>()
                    .Append(x => x.MembershipTenPointsValue)
                    .Append(x => x.MembershipUpgradePointsDiscount)
                    .Build();
            }

            #endregion Memberships

        }

        #endregion AllTheData

    }
}