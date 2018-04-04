using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC.Utils;
using System.Reflection;


namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class DirectReferrals
        {
            public static bool AreUsernamesEncrypted
            {
                get { return appSettings.AreDirectReferralsUsernamesEncrypted; }
                set { appSettings.AreDirectReferralsUsernamesEncrypted = value; }
            }

            public static bool IsDeletingEnabled
            {
                get { return appSettings.IsDirectReferralsDeletingEnabled; }
                set { appSettings.IsDirectReferralsDeletingEnabled = value; }
            }

            public static bool ShowDirectReferralEmail
            {
                get { return appSettings.ShowDirectReferralEmail; }
                set { appSettings.ShowDirectReferralEmail = value; }
            }


            public static int DirectReferralExpiration
            {
                get { return appSettings.DirectReferralExpiration; }
                set { appSettings.DirectReferralExpiration = value; }
            }

            public static bool DirectReferralExpirationEnabled
            {
                get { return appSettings.DirectReferralExpirationEnabled; }
                set { appSettings.DirectReferralExpirationEnabled = value; }
            }

            public static bool DirectReferralBuyingEnabled
            {
                get { return appSettings.DirectReferralBuyingEnabled; }
                set { appSettings.DirectReferralBuyingEnabled = value; }
            }

            public static bool ShowDirectReferralsFullName
            {
                get { return appSettings.ShowDirectReferralsFullName; }
                set { appSettings.ShowDirectReferralsFullName = value; }
            }

            public static bool ShowDirectReferralsPhoneNumber
            {
                get { return appSettings.ShowDirectReferralsPhoneNumber; }
                set { appSettings.ShowDirectReferralsPhoneNumber = value; }
            }

            public static bool ShowDirectReferralsStatus
            {
                get { return appSettings.ShowDirectReferralsStatus; }
                set { appSettings.ShowDirectReferralsStatus = value; }
            }

            public static bool DirectReferralMembershipPacksEnabled
            {
                get { return appSettings.DirectReferralMembershipPacksEnabled; }
                set { appSettings.DirectReferralMembershipPacksEnabled = value; }
            }

            public static bool DefaultReferrerEnabled
            {
                get { return appSettings.IsDefaultReferrreEnabled; }
                set { appSettings.IsDefaultReferrreEnabled = value; }
            }

            public static int? DefaultReferrerId
            {
                get { return appSettings.DefaultReferrerId; }
                set { appSettings.DefaultReferrerId = value; }
            }

            public static void Save()
            {
                appSettings.SaveDirectReferrals();
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadDirectReferrals();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            #region Columns

            [Column("DirectRefsUsernamesEncrypted")]
            internal bool AreDirectReferralsUsernamesEncrypted { get { return _directReferralsUsernamesEncrypted; } set { _directReferralsUsernamesEncrypted = value; SetUpToDateAsFalse(); } }

            [Column("DirectRefsDeleting")]
            internal bool IsDirectReferralsDeletingEnabled { get { return _directReferralsDeletingEnabled; } set { _directReferralsDeletingEnabled = value; SetUpToDateAsFalse(); } }

            [Column("ShowDirectReferralEmail")]
            internal bool ShowDirectReferralEmail { get { return _ShowDirectReferralEmail; } set { _ShowDirectReferralEmail = value; SetUpToDateAsFalse(); } }

            [Column("DirectReferralExpiration")]
            internal int DirectReferralExpiration { get { return _DirectReferralExpiration; } set { _DirectReferralExpiration = value; SetUpToDateAsFalse(); } }

            [Column("DirectReferralExpirationEnabled")]
            internal bool DirectReferralExpirationEnabled { get { return _DirectReferralExpirationEnabled; } set { _DirectReferralExpirationEnabled = value; SetUpToDateAsFalse(); } }

            [Column("DirectReferralBuyingEnabled")]
            internal bool DirectReferralBuyingEnabled { get { return _DirectReferralBuyingEnabled; } set { _DirectReferralBuyingEnabled = value; SetUpToDateAsFalse(); } }

            [Column("ShowDirectReferralsFullName")]
            internal bool ShowDirectReferralsFullName { get { return _showDirectReferralsFullName; } set { _showDirectReferralsFullName = value; SetUpToDateAsFalse(); } }

            [Column("ShowDirectReferralsPhoneNumber")]
            internal bool ShowDirectReferralsPhoneNumber { get { return _showDirectReferralsPhoneNumber; } set { _showDirectReferralsPhoneNumber = value; SetUpToDateAsFalse(); } }

            [Column("ShowDirectReferralsStatus")]
            internal bool ShowDirectReferralsStatus { get { return _showDirectReferralsStatus; } set { _showDirectReferralsStatus = value; SetUpToDateAsFalse(); } }

            [Column("DirectReferralMembershipPacksEnabled")]
            internal bool DirectReferralMembershipPacksEnabled { get { return _DirectReferralMembershipPacksEnabled; } set { _DirectReferralMembershipPacksEnabled = value; SetUpToDateAsFalse(); } }

            [Column("IsDefaultReferrerEnabled")]
            internal bool IsDefaultReferrreEnabled { get { return _isDefaultReferrerEnabled; } set { _isDefaultReferrerEnabled = value; SetUpToDateAsFalse(); } }

            [Column("DefaultReferrerId")]
            internal int? DefaultReferrerId { get { return _defaultReferrerId; } set { _defaultReferrerId = value; SetUpToDateAsFalse(); } }


            private bool _directReferralsUsernamesEncrypted, _directReferralsDeletingEnabled, _ShowDirectReferralEmail, _DirectReferralExpirationEnabled, _DirectReferralBuyingEnabled,
                         _showDirectReferralsFullName, _showDirectReferralsPhoneNumber, _showDirectReferralsStatus, _DirectReferralMembershipPacksEnabled, _isDefaultReferrerEnabled;

            private int _DirectReferralExpiration;
            private int? _defaultReferrerId;

            #endregion



            #region DirectReferrals
            internal void SaveDirectReferrals()
            {
                SavePartially(IsUpToDate, buildDirectReferralsProperties());
            }

            internal void ReloadDirectReferrals()
            {
                ReloadPartially(IsUpToDate, buildDirectReferralsProperties());
            }

            private PropertyInfo[] buildDirectReferralsProperties()
            {
                return new PropertyBuilder<AppSettingsTable>()
                    .Append(x => x.AreDirectReferralsUsernamesEncrypted)
                    .Append(x => x.ShowDirectReferralEmail)
                    .Append(x => x.IsDirectReferralsDeletingEnabled)
                    .Append(x => x.DirectReferralExpiration)
                    .Append(x => x.DirectReferralExpirationEnabled)
                    .Append(x => x.DirectReferralBuyingEnabled)
                    .Append(x => x.ShowDirectReferralsFullName)
                    .Append(x => x.ShowDirectReferralsPhoneNumber)
                    .Append(x => x.ShowDirectReferralsStatus)
                    .Append(x => x.DirectReferralMembershipPacksEnabled)
                    .Append(x => x.IsDefaultReferrreEnabled)
                    .Append(x => x.DefaultReferrerId)
                    .Build();
            }

            #endregion
        }

    }
}
