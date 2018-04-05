using Prem.PTC.Utils;
using System;
using System.Reflection;
using Titan.InvestmentPlatform;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class InvestmentPlatform
        {
            public static bool InvestmentPlatformEnabled
            {
                get { return appSettings.InvestmentPlatformEnabled; }
                set { appSettings.InvestmentPlatformEnabled = value; }
            }

            public static bool InvestmentBalanceEnabled
            {
                get { return appSettings.InvestmentBalanceEnabled; }
                set { appSettings.InvestmentBalanceEnabled = value; }
            }
            
            public static bool CreditToInvestmentBalanceEnabled
            {
                get { return appSettings.CreditToInvestmentBalanceEnabled; }
                set { appSettings.CreditToInvestmentBalanceEnabled = value; }
            }

            public static bool InvestmentPlatformDailyLimitsEnabled
            {
                get { return appSettings.InvestmentPlatformDailyLimitsEnabled; }
                set { appSettings.InvestmentPlatformDailyLimitsEnabled = value; }
            }

            public static SpeedUpOptions InvestmentPlatformSpeedUpOption
            {
                get { return (SpeedUpOptions)appSettings.InvestmentPlatformSpeedUpOption; }
                set { appSettings.InvestmentPlatformSpeedUpOption = (int)value; }
            }

            public static PlansPolicy InvestmentPlatformPlansPolicy
            {
                get { return (PlansPolicy)appSettings.InvestmentPlatformPlansPolicy; }
                set { appSettings.InvestmentPlatformPlansPolicy = (int)value; }
            }

            public static CreditingPolicy InvestmentPlatformCreditingPolicy
            {
                get { return (CreditingPolicy)appSettings.InvestmentPlatformCreditingPolicy; }
                set { appSettings.InvestmentPlatformCreditingPolicy = (int)value; }
            }

            public static bool LevelsEnabled
            {
                get { return appSettings.InvestmentPlatformLevelsEnabled; }
                set { appSettings.InvestmentPlatformLevelsEnabled = value; }
            }

            public static int MinimumTimeBetweenDeposits
            {
                get { return appSettings.InvestmentPlatformMinimumTimeBetweenDeposits; }
                set { appSettings.InvestmentPlatformMinimumTimeBetweenDeposits = value; }
            }

            public static DateTime? LevelsStartDate
            {
                get { return appSettings.InvestmentPlatformLevelsStartDate; }
                set { appSettings.InvestmentPlatformLevelsStartDate = value; }
            }

            #region Proofs
            public static bool ProofsEnabled
            {
                get { return appSettings.InvestmentProofsEnabled; }
                set { appSettings.InvestmentProofsEnabled = value; }
            }

            public static string ProofsNote
            {
                get { return appSettings.InvestmentProofsNote; }
                set { appSettings.InvestmentProofsNote = value; }
            }

            public static bool ProofsWatermarkEnabled
            {
                get { return appSettings.InvestmentProofsWatermarkEnabled; }
                set { appSettings.InvestmentProofsWatermarkEnabled = value; }
            }

            public static bool ProofsDownloadEnabled
            {
                get { return appSettings.InvestmentProofsDownloadEnabled; }
                set { appSettings.InvestmentProofsDownloadEnabled = value; }
            }

            #endregion

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadInvestmentPlatform();
            }

            public static void Save()
            {
                appSettings.SaveInvestmentPlatform();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("InvestmentPlatformEnabled")]
            internal bool InvestmentPlatformEnabled
            {
                get { return _InvestmentPlatformEnabled; }
                set { _InvestmentPlatformEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("InvestmentBalanceEnabled")]
            internal bool InvestmentBalanceEnabled
            {
                get { return _InvestmentBalanceEnabled; }
                set { _InvestmentBalanceEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("CreditToInvestmentBalanceEnabled")]
            internal bool CreditToInvestmentBalanceEnabled
            {
                get { return _CreditToInvestmentBalanceEnabled; }
                set { _CreditToInvestmentBalanceEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("InvestmentPlatformSpeedUpOption")]
            internal int InvestmentPlatformSpeedUpOption
            {
                get { return _InvestmentPlatformSpeedUpOption; }
                set { _InvestmentPlatformSpeedUpOption = value; SetUpToDateAsFalse(); }
            }

            [Column("InvestmentPlatformPlansPolicy")]
            internal int InvestmentPlatformPlansPolicy
            {
                get { return _InvestmentPlatformPlansPolicy; }
                set { _InvestmentPlatformPlansPolicy = value; SetUpToDateAsFalse(); }
            }                  

            [Column("InvestmentPlatformDailyLimitsEnabled")]
            internal bool InvestmentPlatformDailyLimitsEnabled
            {
                get { return _InvestmentPlatformDailyLimitsEnabled; }
                set { _InvestmentPlatformDailyLimitsEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("InvestmentPlatformCreditingPolicy")]
            internal int InvestmentPlatformCreditingPolicy
            {
                get { return _InvestmentPlatformCreditingPolicy; }
                set { _InvestmentPlatformCreditingPolicy = value; SetUpToDateAsFalse(); }
            }

            [Column("InvestmentPlatformLevelsEnabled")]
            internal bool InvestmentPlatformLevelsEnabled
            {
                get { return _InvestmentPlatformLevelsEnabled; }
                set { _InvestmentPlatformLevelsEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("InvestmentPlatformMinimumTimeBetweenDeposits")]
            internal int InvestmentPlatformMinimumTimeBetweenDeposits
            {
                get { return _InvestmentPlatformMinimumTimeBetweenDeposits; }
                set { _InvestmentPlatformMinimumTimeBetweenDeposits = value; SetUpToDateAsFalse(); }
            }

            [Column("InvestmentPlatformLevelsStartDate")]
            internal DateTime? InvestmentPlatformLevelsStartDate
            {
                get { return _InvestmentPlatformLevelsStartDate; }
                set { _InvestmentPlatformLevelsStartDate = value; SetUpToDateAsFalse(); }
            }

            #region Proofs
            [Column("InvestmentProofsEnabled")]
            internal bool InvestmentProofsEnabled
            {
                get { return _InvestmentProofsEnabled; }
                set { _InvestmentProofsEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("InvestmentProofsNote")]
            internal string InvestmentProofsNote
            {
                get { return _InvestmentProofsNote; }
                set { _InvestmentProofsNote = value; SetUpToDateAsFalse(); }
            }

            [Column("InvestmentProofsWatermarkEnabled")]
            internal bool InvestmentProofsWatermarkEnabled
            {
                get { return _InvestmentProofsWatermarkEnabled; }
                set { _InvestmentProofsWatermarkEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("InvestmentProofsDownloadEnabled")]
            internal bool InvestmentProofsDownloadEnabled
            {
                get { return _InvestmentProofsDownloadEnabled; }
                set { _InvestmentProofsDownloadEnabled = value; SetUpToDateAsFalse(); }
            }

            private bool _InvestmentProofsEnabled, _InvestmentProofsWatermarkEnabled, _InvestmentProofsDownloadEnabled;
            private string _InvestmentProofsNote;
            #endregion

            private bool _InvestmentBalanceEnabled, _CreditToInvestmentBalanceEnabled, _InvestmentPlatformDailyLimitsEnabled, _InvestmentPlatformLevelsEnabled,
                        _InvestmentPlatformEnabled;
            private int _InvestmentPlatformSpeedUpOption, _InvestmentPlatformPlansPolicy, _InvestmentPlatformCreditingPolicy,
                        _InvestmentPlatformMinimumTimeBetweenDeposits;
            private DateTime? _InvestmentPlatformLevelsStartDate;

            internal void ReloadInvestmentPlatform()
            {
                ReloadPartially(IsUpToDate, buildInvestmentPlatformProperties());
            }

            internal void SaveInvestmentPlatform()
            {
                SavePartially(IsUpToDate, buildInvestmentPlatformProperties());
            }

            private PropertyInfo[] buildInvestmentPlatformProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.InvestmentPlatformEnabled)
                    .Append(x => x.InvestmentBalanceEnabled)
                    .Append(x => x.CreditToInvestmentBalanceEnabled)
                    .Append(x => x.InvestmentPlatformSpeedUpOption)
                    .Append(x => x.InvestmentPlatformPlansPolicy)
                    .Append(x => x.InvestmentPlatformDailyLimitsEnabled)
                    .Append(x => x.InvestmentPlatformCreditingPolicy)
                    .Append(x => x.InvestmentPlatformLevelsEnabled)
                    .Append(x => x.InvestmentPlatformMinimumTimeBetweenDeposits)
                    .Append(x => x.InvestmentPlatformLevelsStartDate)
                    .Append(x => x.InvestmentProofsEnabled)
                    .Append(x => x.InvestmentProofsNote)
                    .Append(x => x.InvestmentProofsWatermarkEnabled)
                    .Append(x => x.InvestmentProofsDownloadEnabled);

                return exValues.Build();
            }
        }

    }
}