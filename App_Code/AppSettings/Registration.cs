using System.Reflection;
using Prem.PTC.Utils;
using Titan.Registration;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class Registration
        {
            public static bool IsDefaultRegistrationStatusEnabled { get { return appSettings.IsDefaultRegistrationStatusEnabled; } set { appSettings.IsDefaultRegistrationStatusEnabled = value; } }
            public static bool IsDefaultEarnerStatus { get { return appSettings.IsDefaultEarnerStatus; } set { appSettings.IsDefaultEarnerStatus = value; } }
            public static bool IsDefaultAdvertiserStatus { get { return appSettings.IsDefaultAdvertiserStatus; } set { appSettings.IsDefaultAdvertiserStatus = value; } }
            public static bool IsDefaultPublisherStatus { get { return appSettings.IsDefaultPublisherStatus; } set { appSettings.IsDefaultPublisherStatus = value; } }
            public static bool IsRegistrationCaptchaEnabled { get { return appSettings.IsRegistrationCaptchaEnabled; } set { appSettings.IsRegistrationCaptchaEnabled = value; } }
            public static bool IsPINEnabled { get { return appSettings.IsPINEnabled; } set { appSettings.IsPINEnabled = value; } }
            public static bool IsAccountActivationFeeEnabled { get { return appSettings.IsAccountActivationFeeEnabled; } set { appSettings.IsAccountActivationFeeEnabled = value; } }
            public static Money AccountActivationFee { get { return appSettings.AccountActivationFee; } set { appSettings.AccountActivationFee = value; } }
            private static int IntAccountActivationFeeVia { get { return appSettings.IntAccountActivationFeeVia; } set { appSettings.IntAccountActivationFeeVia = value; } }
            public static bool AccountActivationFeeViaCashBalanceEnabled { get { return appSettings.AccountActivationFeeViaCashBalanceEnabled; } set { appSettings.AccountActivationFeeViaCashBalanceEnabled = value; } }

            public static AccountActivationFeeVia AccountActivationFeeVia
            {
                get { return (AccountActivationFeeVia)IntAccountActivationFeeVia; }
                set { IntAccountActivationFeeVia = (int)value; }
            }
            public static void Save()
            {
                appSettings.SaveRegistrattion();
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadRegistrattion();
            }
        }
        internal partial class AppSettingsTable : BaseTableObject
        {
            #region Columns
            [Column("IsDefaultRegistrationStatusEnabled")]
            internal bool IsDefaultRegistrationStatusEnabled
            {
                get { return _IsDefaultRegistrationStatusEnabled; }
                set { _IsDefaultRegistrationStatusEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("IsDefaultEarnerStatus")]
            internal bool IsDefaultEarnerStatus
            {
                get { return _IsDefaultEarnerStatus; }
                set { _IsDefaultEarnerStatus = value; SetUpToDateAsFalse(); }
            }

            [Column("IsDefaultAdvertiserStatus")]
            internal bool IsDefaultAdvertiserStatus
            {
                get { return _IsDefaultAdvertiserStatus; }
                set { _IsDefaultAdvertiserStatus = value; SetUpToDateAsFalse(); }
            }

            [Column("IsDefaultPublisherStatus")]
            internal bool IsDefaultPublisherStatus
            {
                get { return _IsDefaultPublisherStatus; }
                set { _IsDefaultPublisherStatus = value; SetUpToDateAsFalse(); }
            }

            [Column("RegistrationCaptchaEnabled")]
            internal bool IsRegistrationCaptchaEnabled
            {
                get { return _registrationCaptchaEnabled; }
                set { _registrationCaptchaEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("PINEnabled")]
            internal bool IsPINEnabled
            {
                get { return _IsPINEnabled; }
                set { _IsPINEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("AccountActivationFeeEnabled")]
            internal bool IsAccountActivationFeeEnabled
            {
                get { return _IsAccountActivationFeeEnabled; }
                set { _IsAccountActivationFeeEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("AccountActivationFee")]
            internal Money AccountActivationFee
            {
                get { return _AccountActivationFee; }
                set { _AccountActivationFee = value; SetUpToDateAsFalse(); }
            }

            [Column("AccountActivationFeeVia")]
            internal int IntAccountActivationFeeVia
            {
                get { return _IntAccountActivationFeeVia; }
                set { _IntAccountActivationFeeVia = value; SetUpToDateAsFalse(); }
            }

            [Column("AccountActivationFeeViaCashBalanceEnabled")]
            internal bool AccountActivationFeeViaCashBalanceEnabled
            {
                get { return _AccountActivationFeeViaCashBalanceEnabled; }
                set { _AccountActivationFeeViaCashBalanceEnabled = value; SetUpToDateAsFalse(); }
            }

            bool _IsDefaultRegistrationStatusEnabled, _IsDefaultEarnerStatus, _IsDefaultAdvertiserStatus, _IsDefaultPublisherStatus, _registrationCaptchaEnabled,
                _IsPINEnabled, _IsAccountActivationFeeEnabled, _AccountActivationFeeViaCashBalanceEnabled;
            Money _AccountActivationFee;
            int _IntAccountActivationFeeVia;
            #endregion

            internal void SaveRegistrattion()
            {
                SavePartially(IsUpToDate, buildRegistrattionProperties());
            }

            internal void ReloadRegistrattion()
            {
                ReloadPartially(IsUpToDate, buildRegistrattionProperties());
            }

            private PropertyInfo[] buildRegistrattionProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                .Append(x => x.IsDefaultRegistrationStatusEnabled)
                .Append(x => x.IsDefaultEarnerStatus)
                .Append(x => x.IsDefaultAdvertiserStatus)
                .Append(x => x.IsDefaultPublisherStatus)
                .Append(x => x.IsRegistrationCaptchaEnabled)
                .Append(x => x.IsPINEnabled)
                .Append(x => x.IsAccountActivationFeeEnabled)
                .Append(x => x.AccountActivationFee)
                .Append(x => x.IntAccountActivationFeeVia)
                .Append(x => x.AccountActivationFeeViaCashBalanceEnabled);

                return exValues.Build();
            }
        }
    }
}