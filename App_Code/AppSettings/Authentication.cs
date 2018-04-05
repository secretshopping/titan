using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class Authentication
        {
            public static bool IsFacebookEnabled
            {
                get { return appSettings.IsFacebookEnabled; }

                set { appSettings.IsFacebookEnabled = value; }
            }

            public static bool IsInstantlyActivated
            {
                get { return appSettings.IsInstantlyActivated; }
                set
                {
                    appSettings.IsInstantlyActivated = value;
                }
            }

            public static bool DetailedRegisterFields
            {
                get { return appSettings.DetailedRegisterFields; }
                set
                {
                    appSettings.DetailedRegisterFields = value;
                }
            }

            public static bool LoginUsingEmail
            {
                get { return appSettings.LoginUsingEmail; }
                set
                {
                    appSettings.LoginUsingEmail = value;
                }
            }

            public static bool EnableSecondaryPassword
            {
                get { return appSettings.EnableSecondaryPassword; }
                set
                {
                    appSettings.EnableSecondaryPassword = value;
                }
            }

            public static bool CustomFieldsAsSurvey
            {
                get { return appSettings.CustomFieldsAsSurvey; }
                set
                {
                    appSettings.CustomFieldsAsSurvey = value;
                }
            }

            public static int ProfilingSurveyReward
            {
                get { return appSettings.ProfilingSurveyReward; }
                set
                {
                    appSettings.ProfilingSurveyReward = value;
                }
            }

            public static bool ShowDetailedBanMessage
            {
                get { return appSettings.ShowDetailedBanMessage; }
                set
                {
                    appSettings.ShowDetailedBanMessage = value;
                }
            }

            public static bool EnableKeyboard
            {
                get { return appSettings.EnableKeyboard; }
                set
                {
                    appSettings.EnableKeyboard = value;
                }
            }

            /// <summary>
            /// Enable preapproval if you want to approve messagaes sended between clients in messages system.
            /// </summary>
            /// <value>
            ///   <c>true</c> if [enable pre approval]; otherwise, <c>false</c>.
            /// </value>
            public static bool EnablePreApproval
            {
                get { return appSettings.EnablePreApproval; }
                set
                {
                    appSettings.EnablePreApproval = value;
                }
            }

            public static bool ResetPasswordIndirectLinkEnabled
            {
                get { return appSettings.ResetPasswordIndirectLinkEnabled; }
                set
                {
                    appSettings.ResetPasswordIndirectLinkEnabled = value;
                }
            }

            public static bool IsDocumentVerificationEnabled
            {
                get { return appSettings.IsDocumentVerificationEnabled; }
                set
                {
                    appSettings.IsDocumentVerificationEnabled = value;
                }
            }

            public static bool ResetPasswordAndPinTogether
            {
                get { return appSettings.ResetPasswordAndPinTogether; }
                set
                {
                    appSettings.ResetPasswordAndPinTogether = value;
                }
            }

            public static bool AnonymousMemberEnabled
            {
                get { return appSettings.AnonymousMemberEnabled; }
                set
                {
                    appSettings.AnonymousMemberEnabled = value;
                }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadAuthentication();
            }

            public static void Save()
            {
                appSettings.SaveAuthentication();
            }
        }


        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("IsInstantlyActivated")]
            internal bool IsInstantlyActivated { get { return _isInstantActivated; } set { _isInstantActivated = value; SetUpToDateAsFalse(); } }

            [Column("IsFacebookAuth")]
            internal bool IsFacebookEnabled { get { return _isFacebook; } set { _isFacebook = value; SetUpToDateAsFalse(); } }

            [Column("DetailedRegisterFields")]
            internal bool DetailedRegisterFields { get { return _DetailedRegisterFields; } set { _DetailedRegisterFields = value; SetUpToDateAsFalse(); } }

            [Column("CustomFieldsAsSurvey")]
            internal bool CustomFieldsAsSurvey { get { return _CustomFieldsAsSurvey; } set { _CustomFieldsAsSurvey = value; SetUpToDateAsFalse(); } }

            [Column("ProfilingSurveyReward")]
            internal int ProfilingSurveyReward { get { return _ProfilingSurveyReward; } set { _ProfilingSurveyReward = value; SetUpToDateAsFalse(); } }

            [Column("LoginUsingEmail")]
            internal bool LoginUsingEmail { get { return _LoginUsingEmail; } set { _LoginUsingEmail = value; SetUpToDateAsFalse(); } }

            [Column("EnableSecondaryPassword")]
            internal bool EnableSecondaryPassword { get { return _EnableSecondaryPassword; } set { _EnableSecondaryPassword = value; SetUpToDateAsFalse(); } }

            [Column("ShowDetailedBanInformation")]
            internal bool ShowDetailedBanMessage { get { return _ShowDetailedBanMessage; } set { _ShowDetailedBanMessage = value; SetUpToDateAsFalse(); } }

            [Column("EnableKeyboard")]
            internal bool EnableKeyboard { get { return _EnableKeyboard; } set { _EnableKeyboard = value; SetUpToDateAsFalse(); } }

            [Column("ResetPasswordIndirectLinkEnabled")]
            internal bool ResetPasswordIndirectLinkEnabled { get { return _ResetPasswordIndirectLinkEnabled; } set { _ResetPasswordIndirectLinkEnabled = value; SetUpToDateAsFalse(); } }

            [Column("EnablePreApproval")]
            internal bool EnablePreApproval { get { return _enablePreApproval; } set { _enablePreApproval = value; SetUpToDateAsFalse(); } }

            [Column("IsDocumentVerificationEnabled")]
            internal bool IsDocumentVerificationEnabled { get { return _IsDocumentVerificationEnabled; } set { _IsDocumentVerificationEnabled = value; SetUpToDateAsFalse(); } }

            [Column("ResetPasswordAndPinTogether")]
            internal bool ResetPasswordAndPinTogether { get { return _ResetPasswordAndPinTogether; } set { _ResetPasswordAndPinTogether = value; SetUpToDateAsFalse(); } }

            [Column("AnonymousMemberEnabled")]
            internal bool AnonymousMemberEnabled { get { return _AnonymousMemberEnabled; } set { _AnonymousMemberEnabled = value; SetUpToDateAsFalse(); } }

            private int _ProfilingSurveyReward;
            private bool _isInstantActivated, _isFacebook, _DetailedRegisterFields, _CustomFieldsAsSurvey, _LoginUsingEmail, _EnableSecondaryPassword,
                        _ShowDetailedBanMessage, _EnableKeyboard, _ResetPasswordIndirectLinkEnabled, _enablePreApproval, _IsDocumentVerificationEnabled,
                        _ResetPasswordAndPinTogether, _AnonymousMemberEnabled;

            //Save & reload section

            internal void ReloadAuthentication()
            {
                ReloadPartially(IsUpToDate, buildAuthenticationProperties());
            }

            internal void SaveAuthentication()
            {
                SavePartially(IsUpToDate, buildAuthenticationProperties());
            }

            private PropertyInfo[] buildAuthenticationProperties()
            {
                var paymentsValues = new PropertyBuilder<AppSettingsTable>();
                paymentsValues
                    .Append(x => x.IsInstantlyActivated)
                    .Append(x => x.IsFacebookEnabled)
                    .Append(x => x.DetailedRegisterFields)
                    .Append(x => x.CustomFieldsAsSurvey)
                    .Append(x => x.LoginUsingEmail)
                    .Append(x => x.ShowDetailedBanMessage)
                    .Append(x => x.EnableSecondaryPassword)
                    .Append(x => x.ResetPasswordIndirectLinkEnabled)
                    .Append(x => x.EnableKeyboard)
                    .Append(x => x.ProfilingSurveyReward)
                    .Append(x => x.IsDocumentVerificationEnabled)
                    .Append(x => x.EnablePreApproval)
                    .Append(x => x.ResetPasswordAndPinTogether)
                    .Append(x => x.AnonymousMemberEnabled);
                return paymentsValues.Build();
            }
        }

    }
}