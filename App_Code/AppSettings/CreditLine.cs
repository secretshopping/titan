using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class CreditLine
        {
            [Obsolete]
            public static Money MaxAmount
            {
                get { return appSettings.MaxAmount; }
                set { appSettings.MaxAmount = value; }
            }

            public static int FinalDeadlineDays
            {
                get { return appSettings.FinalDeadlineDays; }
                set { appSettings.FinalDeadlineDays = value; }
            }

            public static int FirstDeadlineDays
            {
                get { return appSettings.FirstDeadlineDays; }
                set { appSettings.FirstDeadlineDays = value; }
            }

            public static int SecondDeadlineDays
            {
                get { return appSettings.SecondDeadlineDays; }
                set { appSettings.SecondDeadlineDays = value; }
            }

            public static int FirstRepayPercent
            {
                get { return appSettings.FirstRepayPercent; }
                set { appSettings.FirstRepayPercent = value; }
            }

            public static int SecondRepayPercent
            {
                get { return appSettings.SecondRepayPercent; }
                set { appSettings.SecondRepayPercent = value; }
            }

            public static decimal Fee
            {
                get { return appSettings.CreditLineFee; }
                set { appSettings.CreditLineFee = value; }
            }

            /// <summary>
            /// User have to be registered this number of days before he can borrow money.
            /// </summary>
            public static int MinimumRegisterDays
            {
                get { return appSettings.CreditLineMinimumRegisterDays; }
                set { appSettings.CreditLineMinimumRegisterDays = value; }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadCreditLine();
            }

            public static void Save()
            {
                appSettings.SaveCreditLine();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {

            [Column("CreditLineMaxAmountToBorrow")]
            internal Money MaxAmount { get { return _MaxAmount; } set { _MaxAmount = value; SetUpToDateAsFalse(); } }

            [Column("CreditLineFinalDeadlineDays")]
            internal int FinalDeadlineDays { get { return _FinalDeadlineDays; } set { _FinalDeadlineDays = value; SetUpToDateAsFalse(); } }

            [Column("CreditLineFirstDeadlineDays")]
            internal int FirstDeadlineDays { get { return _FirstDeadlineDays; } set { _FirstDeadlineDays = value; SetUpToDateAsFalse(); } }

            [Column("CreditLineSecondDeadlineDays")]
            internal int SecondDeadlineDays { get { return _SecondDeadlineDays; } set { _SecondDeadlineDays = value; SetUpToDateAsFalse(); } }

            [Column("CreditLineFirstRepayPercent")]
            internal int FirstRepayPercent { get { return _FirstRepayPercent; } set { _FirstRepayPercent = value; SetUpToDateAsFalse(); } }

            [Column("CreditLineSecondRepayPercent")]
            internal int SecondRepayPercent { get { return _SecondRepayPercent; } set { _SecondRepayPercent = value; SetUpToDateAsFalse(); } }

            [Column("CreditLineFee")]
            internal decimal CreditLineFee { get { return _CreditLineFee; } set { _CreditLineFee = value; SetUpToDateAsFalse(); } }

            [Column("CreditLineMinimumRegisterDays")]
            internal int CreditLineMinimumRegisterDays { get { return _CreditLineMinimumRegisterDays; } set { _CreditLineMinimumRegisterDays = value; SetUpToDateAsFalse(); } }
            
            int _FinalDeadlineDays, _FirstDeadlineDays, _SecondDeadlineDays, _FirstRepayPercent, _SecondRepayPercent, _CreditLineMinimumRegisterDays;
            Money _MaxAmount;
            decimal _CreditLineFee;

            //Save & reload section

            internal void ReloadCreditLine()
            {
                ReloadPartially(IsUpToDate, buildCreditLineProperties());
            }

            internal void SaveCreditLine()
            {
                SavePartially(IsUpToDate, buildCreditLineProperties());
            }

            private PropertyInfo[] buildCreditLineProperties()
            {
                var CreditLineProperties = new PropertyBuilder<AppSettingsTable>();
                CreditLineProperties
                    .Append(x => x.MaxAmount)
                    .Append(x => x.FinalDeadlineDays)
                    .Append(x => x.FirstDeadlineDays)
                    .Append(x => x.SecondDeadlineDays)
                    .Append(x => x.FirstRepayPercent)
                    .Append(x => x.SecondRepayPercent)
                    .Append(x => x.CreditLineFee)
                    .Append(x => x.CreditLineMinimumRegisterDays);
                
                return CreditLineProperties.Build();
            }
        }

    }
}