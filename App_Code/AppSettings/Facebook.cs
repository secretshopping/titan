using Prem.PTC.Utils;
using System.Reflection;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class Facebook
        {
            public static int PointsPerLike
            {
                get { return appSettings.FacebookPointsPerLike; }
                set { appSettings.FacebookPointsPerLike = value; }
            }

            public static int DirectReferralPointsPerLike
            {
                get { return appSettings.DirectReferralFbPointsPerLike; }
                set { appSettings.DirectReferralFbPointsPerLike = value; }
            }

            public static Money FriendsRestrictionsCost
            {
                get { return appSettings.FacebookFriendsRestrictionsCost; }
                set
                {
                    appSettings.FacebookFriendsRestrictionsCost = value;
                }
            }

            public static Money ProfilePicRestrictionsCost
            {
                get { return appSettings.FacebookProfilePicRestrictionsCost; }
                set
                {
                    appSettings.FacebookProfilePicRestrictionsCost = value;
                }
            }

            public static string ApplicationId
            {
                get { return appSettings.FacebookAppId; }
                set
                {
                    appSettings.FacebookAppId = value;
                }
            }

            public static string AppSecret
            {
                get { return appSettings.AppSecret; }
                set
                {
                    appSettings.AppSecret = value;
                }
            }

            public static bool CustomFacebookLikesEnabled
            {
                get { return appSettings.CustomFacebookLikesEnabled; }
                set { appSettings.CustomFacebookLikesEnabled = value; }
            }

            public static void Save()
            {
                appSettings.SaveFacebook();
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadFacebook();
            }

            public static bool CreditOnlyUsersWhoPuchasedFacebookMembership
            {
                get { return appSettings.CreditOnlyUsersWhoPuchasedFacebookMembership; }
                set { appSettings.CreditOnlyUsersWhoPuchasedFacebookMembership = value; }
            }

        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("FacebookAppId")]
            internal string FacebookAppId { get { return _FacebookAppId; } set { _FacebookAppId = value; SetUpToDateAsFalse(); } }

            [Column("FbFriendsRestrictionsCost")]
            internal Money FacebookFriendsRestrictionsCost { get { return _fbFriendsRestrictionCost; } set { _fbFriendsRestrictionCost = value; SetUpToDateAsFalse(); } }

            [Column("FbProfilePicRestrictionsCost")]
            internal Money FacebookProfilePicRestrictionsCost { get { return _fbProfilePicRestrictionsCost; } set { _fbProfilePicRestrictionsCost = value; SetUpToDateAsFalse(); } }

            [Column("FbPointsPerLike")]
            internal int FacebookPointsPerLike { get { return _fbPointsPerLike; } set { _fbPointsPerLike = value; SetUpToDateAsFalse(); } }

            [Column("DRFbPointsPerLike")]
            internal int DirectReferralFbPointsPerLike { get { return _DRfbPointsPerLike; } set { _DRfbPointsPerLike = value; SetUpToDateAsFalse(); } }

            [Column("FacebookSecretKey")]
            internal string AppSecret { get { return _AppSecret; } set { _AppSecret = value; SetUpToDateAsFalse(); } }

            [Column("CreditOnlyUsersWhoPuchasedFacebookMembership")]
            internal bool CreditOnlyUsersWhoPuchasedFacebookMembership { get { return _CreditOnlyUsersWhoPuchasedFacebookMembership; } set { _CreditOnlyUsersWhoPuchasedFacebookMembership = value; SetUpToDateAsFalse(); } }

            [Column("CustomFacebookLikesEnabled")]
            internal bool CustomFacebookLikesEnabled { get { return _CustomFacebookLikesEnabled; } set { _CustomFacebookLikesEnabled = value; SetUpToDateAsFalse(); } }

            private string _FacebookAppId, _AppSecret;
            private Money _fbFriendsRestrictionCost, _fbProfilePicRestrictionsCost;
            private int _fbPointsPerLike, _DRfbPointsPerLike;
            private bool _CreditOnlyUsersWhoPuchasedFacebookMembership, _CustomFacebookLikesEnabled;

            //Save & reload section

            internal void SaveFacebook()
            {
                SavePartially(IsUpToDate, buildFacebookProperties());
            }

            internal void ReloadFacebook()
            {
                ReloadPartially(IsUpToDate, buildFacebookProperties());
            }

            private PropertyInfo[] buildFacebookProperties()
            {
                return new PropertyBuilder<AppSettingsTable>()
                    .Append(x => x.FacebookPointsPerLike)
                    .Append(x => x.FacebookProfilePicRestrictionsCost)
                    .Append(x => x.FacebookFriendsRestrictionsCost)
                    .Append(x => x.FacebookAppId)
                    .Append(x => x.AppSecret)
                    .Append(x => x.DirectReferralFbPointsPerLike)
                    .Append(x => x.CreditOnlyUsersWhoPuchasedFacebookMembership)
                    .Append(x => x.CustomFacebookLikesEnabled)
                    .Build();
            }
        }

    }
}