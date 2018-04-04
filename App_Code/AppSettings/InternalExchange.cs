using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Prem.PTC.Utils;
using System.Reflection;
using Titan.InternalExchange;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class InternalExchange
        {
            #region Global Settings
            private static int InternalExchangeStockTypeInt
            {
                get { return appSettings.InternalExchangeStockType; }
                set { appSettings.InternalExchangeStockType = value; }
            }

            public static BalanceType InternalExchangeStockType
            {
                get { return (BalanceType)InternalExchangeStockTypeInt; }
                set { InternalExchangeStockTypeInt = (int)value; }
            }

            private static int InternalExchangePurchaseViaInt
            {
                get { return appSettings.InternalExchangePurchaseVia; }
                set { appSettings.InternalExchangePurchaseVia = value; }
            }

            public static BalanceType InternalExchangePurchaseVia
            {
                get { return (BalanceType)InternalExchangePurchaseViaInt; }
                set { InternalExchangePurchaseViaInt = (int)value; }
            }

            public static int InternalExchangeBuforTimeMinutes
            {
                get { return appSettings.InternalExchangeBuforTimeMinutes; }
                set { appSettings.InternalExchangeBuforTimeMinutes = value; }
            }

            public static decimal InternalExchangeBidCommissionPercent
            {
                get { return appSettings.InternalExchangeBidCommissionPercent; }
                set { appSettings.InternalExchangeBidCommissionPercent = value; }
            }

            public static decimal InternalExchangeAskCommissionPercent
            {
                get { return appSettings.InternalExchangeAskCommissionPercent; }
                set { appSettings.InternalExchangeAskCommissionPercent = value; }
            }

            public static bool InternalExchangeLockWithdraw
            {
                get { return appSettings.InternalExchangeLockWithdraw; }
                set { appSettings.InternalExchangeLockWithdraw = value; }
            }

            public static int InternalExchangeLockWithdrawTime
            {
                get { return appSettings.InternalExchangeLockWithdrawTime; }
                set { appSettings.InternalExchangeLockWithdrawTime = value; }
            }

            public static decimal ICOInternalExchangeMinimalStockPrice
            {
                get { return appSettings.ICOInternalExchangeMinimalStockPrice; }
                set { appSettings.ICOInternalExchangeMinimalStockPrice = value; }
            }

            public static int InternalExchangePeriodSellAmountTime
            {
                get { return appSettings.InternalExchangePeriodSellAmountTime; }
                set { appSettings.InternalExchangePeriodSellAmountTime = value; }
            }

            public static decimal InternalExchangeMinimalSellAmount
            {
                get { return appSettings.InternalExchangeMinimalSellAmount; }
                set { appSettings.InternalExchangeMinimalSellAmount = value; }
            }

            public static decimal InternalExchangePeriodMaxSellAmount
            {
                get { return appSettings.InternalExchangePeriodMaxSellAmount; }
                set { appSettings.InternalExchangePeriodMaxSellAmount = value; }
            }
            #endregion

            public static void Reload()
            {
                appSettings.ReloadInternalExchangeSettings();
            }

            public static void Save()
            {
                appSettings.SaveInternalExchangeSettings();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            #region Columns
            [Column("InternalExchangeStockType")]
            internal int InternalExchangeStockType
            {
                get { return _InternalExchangeStockType; }
                set { _InternalExchangeStockType = value; SetUpToDateAsFalse(); }
            }

            [Column("InternalExchangePurchaseVia")]
            internal int InternalExchangePurchaseVia
            {
                get { return _InternalExchangePurchaseVia; }
                set { _InternalExchangePurchaseVia = value; SetUpToDateAsFalse(); }
            }

            [Column("InternalExchangeBuforTimeMinutes")]
            internal int InternalExchangeBuforTimeMinutes
            {
                get { return _InternalExchangeBuforTimeMinutes; }
                set { _InternalExchangeBuforTimeMinutes = value; SetUpToDateAsFalse(); }
            }

            [Column("InternalExchangeBidCommissionPercent")]
            internal decimal InternalExchangeBidCommissionPercent
            {
                get { return _InternalExchangeBidCommissionPercent; }
                set { _InternalExchangeBidCommissionPercent = value; SetUpToDateAsFalse(); }
            }

            [Column("InternalExchangeAskCommissionPercent")]
            internal decimal InternalExchangeAskCommissionPercent
            {
                get { return _InternalExchangeAskCommissionPercent; }
                set { _InternalExchangeAskCommissionPercent = value; SetUpToDateAsFalse(); }
            }

            [Column("InternalExchangeLockWithdraw")]
            internal bool InternalExchangeLockWithdraw
            {
                get { return _InternalExchangeLockWithdraw; }
                set { _InternalExchangeLockWithdraw = value; SetUpToDateAsFalse(); }
            }

            [Column("InternalExchangeLockWithdrawTime")]
            internal int InternalExchangeLockWithdrawTime
            {
                get { return _InternalExchangeLockWithdrawTime; }
                set { _InternalExchangeLockWithdrawTime = value; SetUpToDateAsFalse(); }
            }

            [Column("ICOInternalExchangeMinimalStockPrice")]
            internal decimal ICOInternalExchangeMinimalStockPrice
            {
                get { return _ICOInternalExchangeMinimalStockPrice; }
                set { _ICOInternalExchangeMinimalStockPrice = value; SetUpToDateAsFalse(); }
            }

            [Column("InternalExchangePeriodSellAmountTime")]
            internal int InternalExchangePeriodSellAmountTime
            {
                get { return _InternalExchangePeriodSellAmountTime; }
                set { _InternalExchangePeriodSellAmountTime = value; SetUpToDateAsFalse(); }
            }

            [Column("InternalExchangeMinimalSellAmount")]
            internal decimal InternalExchangeMinimalSellAmount
            {
                get { return _InternalExchangeMinimalSellAmount; }
                set { _InternalExchangeMinimalSellAmount = value; SetUpToDateAsFalse(); }
            }

            [Column("InternalExchangePeriodMaxSellAmount")]
            internal decimal InternalExchangePeriodMaxSellAmount
            {
                get { return _InternalExchangePeriodMaxSellAmount; }
                set { _InternalExchangePeriodMaxSellAmount = value; SetUpToDateAsFalse(); }
            }

            private bool _InternalExchangeLockWithdraw;
            private int _InternalExchangeStockType, _InternalExchangePurchaseVia, _InternalExchangeBuforTimeMinutes,
                _InternalExchangeLockWithdrawTime, _InternalExchangePeriodSellAmountTime;
            private decimal _InternalExchangeBidCommissionPercent, _InternalExchangeAskCommissionPercent, _ICOInternalExchangeMinimalStockPrice
                , _InternalExchangeMinimalSellAmount, _InternalExchangePeriodMaxSellAmount;

            #endregion

            internal void SaveInternalExchangeSettings()
            {
                SavePartially(IsUpToDate, buildInternalExchangeProperties());
            }

            internal void ReloadInternalExchangeSettings()
            {
                ReloadPartially(IsUpToDate, buildInternalExchangeProperties());
            }

            private PropertyInfo[] buildInternalExchangeProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.InternalExchangeStockType)
                    .Append(x => x.InternalExchangePurchaseVia)
                    .Append(x => x.InternalExchangeBuforTimeMinutes)
                    .Append(x => x.InternalExchangeAskCommissionPercent)
                    .Append(x => x.InternalExchangeBidCommissionPercent)
                    .Append(x => x.InternalExchangeLockWithdraw)
                    .Append(x => x.InternalExchangeLockWithdrawTime)
                    .Append(x => x.ICOInternalExchangeMinimalStockPrice)
                    .Append(x => x.InternalExchangePeriodSellAmountTime)
                    .Append(x => x.InternalExchangeMinimalSellAmount)
                    .Append(x => x.InternalExchangePeriodMaxSellAmount)
                    ;

                return exValues.Build();
            }

        }
    }
}





