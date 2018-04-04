using Prem.PTC.Utils;
using System;
using System.Reflection;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class Emails
        {
            /// <exception cref="ArgumentNullException" />
            public static string AdvertRejection
            {
                get { return appSettings.AdvertRejectionMessage; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("AdvertRejection");

                    appSettings.AdvertRejectionMessage = value;
                }
            }

            /// <exception cref="ArgumentNullException" />
            public static string AdvertStart
            {
                get { return appSettings.AdvertStartMessage; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("AdvertStart");

                    appSettings.AdvertStartMessage = value;
                }
            }

            /// <exception cref="ArgumentNullException" />
            public static string AccountActivation
            {
                get { return appSettings.AccountActivationMessage; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("AccountActivation");

                    appSettings.AccountActivationMessage = value;
                }
            }

            /// <exception cref="ArgumentNullException" />
            public static string ResetPassword
            {
                get { return appSettings.ResetPasswordMessage; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("ResetPassword");

                    appSettings.ResetPasswordMessage = value;
                }
            }

            public static string ResetPasswordLink
            {
                get { return appSettings.ResetPasswordLink; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("ResetPassword");

                    appSettings.ResetPasswordLink = value;
                }
            }

            public static string NewReferralCommision
            {
                get { return appSettings.NewReferralCommisionEmailMessage; }
                set
                {
                    appSettings.NewReferralCommisionEmailMessage = value;
                }
            }

            public static string NewReferral
            {
                get { return appSettings.NewReferralEmailMessage; }
                set
                {
                    appSettings.NewReferralEmailMessage = value;
                }
            }

            public static bool NewReferralCommisionEnabled
            {
                get { return appSettings.NewReferralCommisionEnabled; }
                set
                {
                    appSettings.NewReferralCommisionEnabled = value;
                }
            }

            public static bool NewReferralEnabled
            {
                get { return appSettings.NewReferralEnabled; }
                set
                {
                    appSettings.NewReferralEnabled = value;
                }
            }

            public static bool PayoutEmailMessageEnabled
            {
                get { return appSettings.PayoutEmailMessageEnabled; }
                set { appSettings.PayoutEmailMessageEnabled = value; }
            }

            public static string PayoutEmailMessage
            {
                get { return appSettings.PayoutEmailMessage; }
                set { appSettings.PayoutEmailMessage = value; }
            }

            public static void Save()
            {
                appSettings.SaveEmails();
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadEmails();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {

            [Column("AdvertRejectionMessage")]
            internal string AdvertRejectionMessage { get { return _advertRejectionMessage; } set { _advertRejectionMessage = value; SetUpToDateAsFalse(); } }

            [Column("AdvertStartMessage")]
            internal string AdvertStartMessage { get { return _advertStartMessage; } set { _advertStartMessage = value; SetUpToDateAsFalse(); } }

            [Column("AccountActivationMessage")]
            internal string AccountActivationMessage { get { return _accountActivationMessage; } set { _accountActivationMessage = value; SetUpToDateAsFalse(); } }

            [Column("ResetPasswordMessage")]
            internal string ResetPasswordMessage { get { return _resetPasswordMessage; } set { _resetPasswordMessage = value; SetUpToDateAsFalse(); } }

            [Column("ResetPasswordLink")]
            internal string ResetPasswordLink { get { return _ResetPasswordLink; } set { _ResetPasswordLink = value; SetUpToDateAsFalse(); } }

            [Column("NewReferralEmailMessage")]
            internal string NewReferralEmailMessage { get { return _NewReferralEmailMessage; } set { _NewReferralEmailMessage = value; SetUpToDateAsFalse(); } }

            [Column("NewReferralCommisionEmailMessage")]
            internal string NewReferralCommisionEmailMessage { get { return _NewReferralCommisionEmailMessage; } set { _NewReferralCommisionEmailMessage = value; SetUpToDateAsFalse(); } }

            [Column("NewReferralCommisionEnabled")]
            internal bool NewReferralCommisionEnabled { get { return _NewReferralCommisionEnabled; } set { _NewReferralCommisionEnabled = value; SetUpToDateAsFalse(); } }

            [Column("NewReferralEnabled")]
            internal bool NewReferralEnabled { get { return _NewReferralEnabled; } set { _NewReferralEnabled = value; SetUpToDateAsFalse(); } }

            [Column("PayoutEmailMessage")]
            internal string PayoutEmailMessage { get { return _PayoutEmailMessage; } set { _PayoutEmailMessage = value; SetUpToDateAsFalse(); } }

            [Column("PayoutEmailMessageEnabled")]
            internal bool PayoutEmailMessageEnabled { get { return _PayoutEmailMessageEnabled; } set { _PayoutEmailMessageEnabled = value; SetUpToDateAsFalse(); } }

            private string _advertRejectionMessage, _advertStartMessage, _accountActivationMessage, _resetPasswordMessage, _ResetPasswordLink,
                _NewReferralEmailMessage, _NewReferralCommisionEmailMessage, _PayoutEmailMessage;

            private bool _NewReferralCommisionEnabled, _NewReferralEnabled, _PayoutEmailMessageEnabled;

            #region Emails

            internal void SaveEmails()
            {
                SavePartially(IsUpToDate, buildEmailsProperties());
            }

            internal void ReloadEmails()
            {
                ReloadPartially(IsUpToDate, buildEmailsProperties());
            }

            private PropertyInfo[] buildEmailsProperties()
            {
                return new PropertyBuilder<AppSettingsTable>()
                    .Append(x => x.AdvertRejectionMessage)
                    .Append(x => x.AdvertStartMessage)
                    .Append(x => x.AccountActivationMessage)
                    .Append(x => x.ResetPasswordMessage)
                    .Append(x => x.ResetPasswordLink)
                    .Append(x => x.NewReferralEmailMessage)
                    .Append(x => x.NewReferralCommisionEmailMessage)
                    .Append(x => x.NewReferralCommisionEnabled)
                    .Append(x => x.NewReferralEnabled)
                    .Append(x => x.PayoutEmailMessage)
                    .Append(x => x.PayoutEmailMessageEnabled)
                    .Build();
            }

            #endregion Emails
        }
    }
}