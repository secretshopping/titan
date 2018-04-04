using ExtensionMethods;
using Prem.PTC.Memberships;
using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Titan.Cryptocurrencies;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class Payments
        {
            /// <exception cref="ArgumentNullException" />
            public static string TransactionNote
            {
                get { return appSettings.TransactionNote; }
                set
                {
                    if (value == null) throw new ArgumentNullException("Payments");
                    appSettings.TransactionNote = value;
                }
            }

            public static Money MemberMaxCashoutLimit
            {
                get { return appSettings.MemberMaxCashoutLimit; }
                set { appSettings.MemberMaxCashoutLimit = value; }
            }

            public static Money GlobalCashoutLimitPerDay
            {
                get { return appSettings.GlobalCashoutLimitPerDay; }
                set { appSettings.GlobalCashoutLimitPerDay = value; }
            }

            /// <summary>
            /// Counts only INSTANT ones (without manual)
            /// </summary>
            public static Money GlobalCashoutsToday
            {
                get { return appSettings.GlobalCashoutsToday; }
                set { appSettings.GlobalCashoutsToday = value; }
            }

            public static bool IsInstantPayout
            {
                get { return appSettings.IsInstantPayout; }
                set { appSettings.IsInstantPayout = value; }
            }

            public static bool TransferSlidebarEnabled
            {
                get { return appSettings.TransferSlidebarEnabled; }
                set { appSettings.TransferSlidebarEnabled = value; }
            }

            public static string PointBalanceName
            {
                get { return appSettings.PointsBalanceName; }
                set { appSettings.PointsBalanceName = value; }
            }

            public static Money MinimumTransferAmount
            {
                get { return appSettings.MinimumTransferAmount; }
                set { appSettings.MinimumTransferAmount = value; }
            }

            public static TransferFundsMode TransferMode
            {
                get { return (TransferFundsMode)appSettings.TransferMode; }
                set { appSettings.TransferMode = (int)value; }
            }

            //4000

            public static int InstantPayoutMinOffersCompleted
            {
                get { return appSettings.InstantPayoutMinOffersCompleted; }
                set { appSettings.InstantPayoutMinOffersCompleted = value; }
            }

            public static int InstantPayoutMinRegisteredDays
            {
                get { return appSettings.InstantPayoutMinRegisteredDays; }
                set { appSettings.InstantPayoutMinRegisteredDays = value; }
            }

            public static int InstantPayoutMinCashoutsNumber
            {
                get { return appSettings.InstantPayoutMinCashoutsNumber; }
                set { appSettings.InstantPayoutMinCashoutsNumber = value; }
            }

            public static string CurrencyPointsName
            {
                get { return appSettings.CurrencyPointsName; }
                set { appSettings.CurrencyPointsName = value; }
            }

            //4200
            [Obsolete]
            public static int TransferFee
            {
                get { return appSettings.TransferFee; }
                set { appSettings.TransferFee = value; }
            }

            //5004
            public static MaximumPayoutPolicy MaximumPayoutPolicy
            {
                get { return (MaximumPayoutPolicy)appSettings.MaximumPayoutPolicyInt; }
                set { appSettings.MaximumPayoutPolicyInt = (int)value; }
            }

            public static int MaximumPayoutPercentage
            {
                get { return appSettings.MaximumPayoutPercentage; }
                set { appSettings.MaximumPayoutPercentage = value; }
            }

            public static Money MaximumPayoutConstant
            {
                get { return appSettings.MaximumPayoutConstant; }
                set { appSettings.MaximumPayoutConstant = value; }
            }

            public static bool CommissionBalanceEnabled
            {
                get { return appSettings.CommissionBalanceEnabled; }
                set { appSettings.CommissionBalanceEnabled = value; }
            }

            public static bool CommissionToMainBalanceEnabled
            {
                get { return appSettings.CommissionToMainBalanceEnabled; }
                set { appSettings.CommissionToMainBalanceEnabled = value; }
            }

            public static bool CommissionToAdBalanceEnabled
            {
                get { return appSettings.CommissionToAdBalanceEnabled; }
                set { appSettings.CommissionToAdBalanceEnabled = value; }
            }

            public static bool TransferMainInAdBalanceEnabled
            {
                get { return appSettings.TransferMainInAdBalanceEnabled; }
                set { appSettings.TransferMainInAdBalanceEnabled = value; }
            }

            public static bool ProportionalPayoutLimitsEnabled
            {
                get { return appSettings.ProportionalPayoutLimitsEnabled; }
                set { appSettings.ProportionalPayoutLimitsEnabled = value; }
            }

            public static bool AdPackTypeWithdrawLimitEnabled
            {
                get { return appSettings.AdPackTypeWithdrawLimitEnabled; }
                set { appSettings.AdPackTypeWithdrawLimitEnabled = value; }
            }
            public static bool RefTiersMaxWeeklyPayoutEnabled
            {
                get { return appSettings.RefTiersMaxWeeklyPayoutEnabled; }
                set { appSettings.RefTiersMaxWeeklyPayoutEnabled = value; }
            }

            public static string YourInvoiceName
            {
                get { return appSettings.YourInvoiceName; }
                set { appSettings.YourInvoiceName = value; }
            }

            public static bool PointsToAdBalanceEnabled
            {
                get { return appSettings.PointsToAdBalanceEnabled; }
                set { appSettings.PointsToAdBalanceEnabled = value; }
            }

            public static bool CashBalanceEnabled
            {
                get { return appSettings.CashBalanceEnabled; }
                set { appSettings.CashBalanceEnabled = value; }
            }

            public static bool CashToAdBalanceEnabled
            {
                get { return appSettings.CashToAdBalanceEnabled; }
                set { appSettings.CashToAdBalanceEnabled = value; }
            }

            public static CurrencyMode CurrencyMode
            {
                get { return (CurrencyMode)appSettings.CurrencyModeInt; }
                set { appSettings.CurrencyModeInt = (int)value; }
            }

            public static TokenCryptocurrencyValue TokenCryptocurrencyValueType
            {
                get { return (TokenCryptocurrencyValue)appSettings.TokenCryptocurrencyValueTypeInt; }
                set { appSettings.TokenCryptocurrencyValueTypeInt = (int)value; }
            }

            public static int WithdrawalVerificationCodeValidForMinutes
            {
                get { return appSettings.WithdrawalVerificationCodeValidForMinutes; }
                set { appSettings.WithdrawalVerificationCodeValidForMinutes = value; }
            }
            public static bool WithdrawalEmailEnabled { get { return appSettings.WithdrawalEmailEnabled; } set { appSettings.WithdrawalEmailEnabled = value; } }

            public static bool CommissionBalanceWithdrawalEnabled { get { return appSettings.CommissionBalanceWithdrawalEnabled; } set { appSettings.CommissionBalanceWithdrawalEnabled = value; } }

            public static bool MarketplaceBalanceEnabled
            {
                get { return appSettings.MarketplaceBalanceEnabled; }
                set { appSettings.MarketplaceBalanceEnabled = value; }
            }

            public static bool MarketplaceBalanceDepositEnabled
            {
                get { return appSettings.MarketplaceBalanceDepositEnabled; }
                set { appSettings.MarketplaceBalanceDepositEnabled = value; }
            }

            public static bool MainToMarketplaceBalanceEnabled
            {
                get { return appSettings.MainToMarketplaceBalanceEnabled; }
                set { appSettings.MainToMarketplaceBalanceEnabled = value; }
            }
            
            public static MultiCurrencyProvider MultiCurrencyProvider
            {
                get { return (MultiCurrencyProvider)appSettings.MultiCurrencyProviderInt; }
                set { appSettings.MultiCurrencyProviderInt = (int)value; }
            } 

            public static string ExchangeRateAppIDCode
            {
                get { return appSettings.ExchangeRateAppIDCode; }
                set { appSettings.ExchangeRateAppIDCode = value; }
            }

            public static bool TransferFromPaymentProcessorsToAdBalanceEnabled
            {
                get { return appSettings.TransferFromPaymentProcessorsToAdBalanceEnabled; }
                set { appSettings.TransferFromPaymentProcessorsToAdBalanceEnabled = value; }
            }

            public static bool TransferFromTokenWalletToPurchaseBalanceEnabled
            {
                get { return CryptocurrencyFactory.Get(CryptocurrencyType.ERC20Token).WalletEnabled && appSettings.TransferFromTokenWalletToPurchaseBalanceEnabled; }
                set { appSettings.TransferFromTokenWalletToPurchaseBalanceEnabled = value; }
            }

            public static bool TransferFromBTCWalletToPurchaseBalanceEnabled
            {
                get { return CryptocurrencyFactory.Get(CryptocurrencyType.BTC).WalletEnabled && appSettings.TransferFromBTCWalletToPurchaseBalanceEnabled; }
                set { appSettings.TransferFromBTCWalletToPurchaseBalanceEnabled = value; }
            }

            public static bool TransferFromMainBalanceToTokenWalletEnabled
            {
                get { return CryptocurrencyFactory.Get(CryptocurrencyType.ERC20Token).WalletEnabled && appSettings.TransferFromMainBalanceToTokenWalletEnabled; }
                set { appSettings.TransferFromMainBalanceToTokenWalletEnabled = value; }
            }
            public static bool BalancesVisibilityInUserMenuEnabled
            {
                get { return appSettings.BalancesVisibilityInUserMenuEnabled; }
                set { appSettings.BalancesVisibilityInUserMenuEnabled = value; }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadPayments();
            }

            public static void Save()
            {
                appSettings.SavePayments();
            }

            public static void CRON()
            {
                try
                {
                    HttpContext.Current.Application["GeoCountData"] = null;
                    AppSettings.Payments.GlobalCashoutsToday = new Money(0);
                    AppSettings.Payments.Save();
                    TableHelper.ExecuteRawCommandNonQuery(String.Format("DELETE FROM PaparaOrders WHERE DateAdded < '{0}'", AppSettings.ServerTime.AddDays(-30).ToDBString()));
                }
                catch(Exception e)
                {
                    ErrorLogger.Log(e);
                }
            }

            public static void SetAllowedPaymentDay(DayOfWeek day, bool activePayouts)
            {
                var tab = MakeDaysBoolArrayFromInt(EnabledPayoutDays);
                tab[(int)day] = activePayouts;
                EnabledPayoutDays = MakeDaysIntFromBoolArray(tab);
            }

            public static bool GetAllowedPaymentDay(DayOfWeek day)
            {
                var tab = MakeDaysBoolArrayFromInt(EnabledPayoutDays);
                return tab[(int)day];
            }

            private static bool[] MakeDaysBoolArrayFromInt(int n)
            {
                var digits = new List<bool>();

                for (; n != 0; n /= 10)
                    digits.Add((n % 10) == 2);

                var arr = digits.ToArray();
                Array.Reverse(arr);
                return arr;
            }

            private static int MakeDaysIntFromBoolArray(bool[] arr)
            {
                int r = 1111111;
                int counter = 1000000;
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i])
                        r += counter;
                    counter /= 10;
                }
                return r;
            }

            public static bool MaxWithdrawalAllowedPerInvestmentPercentEnabled
            {
                get
                {
                    var cache = new MembershipsCache();
                    var memberships = (List<Membership>)cache.Get();
                    foreach (var membership in memberships)
                        if (membership.MaxWithdrawalAllowedPerInvestmentPercent < 1000000000)
                            return true;
                    return false;
                }
            }

            private static int EnabledPayoutDays
            {
                get
                {
                    return appSettings.EnabledPayoutDays;
                }
                set
                {
                    appSettings.EnabledPayoutDays = value;
                }
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("GlobalCashoutLimitPerDay")]
            internal Money GlobalCashoutLimitPerDay { get { return _GlobalCashoutLimitPerDay; } set { _GlobalCashoutLimitPerDay = value; SetUpToDateAsFalse(); } }

            [Column("GlobalCashoutsToday")]
            internal Money GlobalCashoutsToday { get { return _GlobalCashoutsToday; } set { _GlobalCashoutsToday = value; SetUpToDateAsFalse(); } }

            [Column("MemberMaxCashoutLimit")]
            internal Money MemberMaxCashoutLimit { get { return _MemberMaxCashoutLimit; } set { _MemberMaxCashoutLimit = value; SetUpToDateAsFalse(); } }

            [Column("MaximumPayoutConstant")]
            internal Money MaximumPayoutConstant { get { return _MaximumPayoutConstant; } set { _MaximumPayoutConstant = value; SetUpToDateAsFalse(); } }

            [Column("MaximumPayoutPercentage")]
            internal int MaximumPayoutPercentage { get { return _MaximumPayoutPercentage; } set { _MaximumPayoutPercentage = value; SetUpToDateAsFalse(); } }

            [Column("TransactionNote")]
            internal string TransactionNote { get { return _transactionNote; } set { _transactionNote = value; SetUpToDateAsFalse(); } }

            [Column("IsInstantPayout")]
            internal bool IsInstantPayout { get { return _instantPayout; } set { _instantPayout = value; SetUpToDateAsFalse(); } }

            [Column("MinimumTransferAmount")]
            internal Money MinimumTransferAmount { get { return _MinimumTransferAmount; } set { _MinimumTransferAmount = value; SetUpToDateAsFalse(); } }

            [Column("TransferFundsMode")]
            internal int TransferMode { get { return _TransferMode; } set { _TransferMode = value; SetUpToDateAsFalse(); } }

            [Column("TransferFee")]
            internal int TransferFee { get { return _TransferFee; } set { _TransferFee = value; SetUpToDateAsFalse(); } }

            [Column("TransferSlidebarEnabled")]
            internal bool TransferSlidebarEnabled { get { return _TransferSlidebarEnabled; } set { _TransferSlidebarEnabled = value; SetUpToDateAsFalse(); } }

            [Column("InstantPayoutMinOffersCompleted")]
            internal int InstantPayoutMinOffersCompleted { get { return _InstantPayoutMinOffersCompleted; } set { _InstantPayoutMinOffersCompleted = value; SetUpToDateAsFalse(); } }

            [Column("InstantPayoutMinRegisteredDays")]
            internal int InstantPayoutMinRegisteredDays { get { return _InstantPayoutMinRegisteredDays; } set { _InstantPayoutMinRegisteredDays = value; SetUpToDateAsFalse(); } }

            [Column("InstantPayoutMinCashoutsNumber")]
            internal int InstantPayoutMinCashoutsNumber { get { return _InstantPayoutMinCashoutsNumber; } set { _InstantPayoutMinCashoutsNumber = value; SetUpToDateAsFalse(); } }

            [Column("CurrencyPointsName")]
            internal string CurrencyPointsName { get { return _CurrencyPointsName; } set { _CurrencyPointsName = value; SetUpToDateAsFalse(); } }

            [Column("MaximumPayoutPolicy")]
            internal int MaximumPayoutPolicyInt
            {
                get { return _MaximumPayoutPolicyInt; }
                set { _MaximumPayoutPolicyInt = value; SetUpToDateAsFalse(); }
            }

            [Column("CommissionBalanceEnabled")]
            internal bool CommissionBalanceEnabled { get { return _CommissionBalanceEnabled; } set { _CommissionBalanceEnabled = value; SetUpToDateAsFalse(); } }


            [Column("CommissionToAdBalanceEnabled")]
            internal bool CommissionToAdBalanceEnabled { get { return _CommissionToAdBalanceEnabled; } set { _CommissionToAdBalanceEnabled = value; SetUpToDateAsFalse(); } }

            [Column("CommissionToMainBalanceEnabled")]
            internal bool CommissionToMainBalanceEnabled { get { return _CommissionToMainBalanceEnabled; } set { _CommissionToMainBalanceEnabled = value; SetUpToDateAsFalse(); } }

            [Column("TransferMainInAdBalanceEnabled")]
            internal bool TransferMainInAdBalanceEnabled { get { return _TransferMainInAdBalanceEnabled; } set { _TransferMainInAdBalanceEnabled = value; SetUpToDateAsFalse(); } }

            [Column("ProportionalPayoutLimitsEnabled")]
            internal bool ProportionalPayoutLimitsEnabled { get { return _ProportionalPayoutLimitsEnabled; } set { _ProportionalPayoutLimitsEnabled = value; SetUpToDateAsFalse(); } }

            [Column("AdPackTypeWithdrawLimitEnabled")]
            internal bool AdPackTypeWithdrawLimitEnabled { get { return _AdPackTypeWithdrawLimitEnabled; } set { _AdPackTypeWithdrawLimitEnabled = value; SetUpToDateAsFalse(); } }

            [Column("RefTiersMaxWeeklyPayoutEnabled")]
            internal bool RefTiersMaxWeeklyPayoutEnabled { get { return _RefTiersMaxWeeklyPayoutEnabled; } set { _RefTiersMaxWeeklyPayoutEnabled = value; SetUpToDateAsFalse(); } }

            [Column("YourInvoiceName")]
            internal string YourInvoiceName { get { return _YourInvoiceName; } set { _YourInvoiceName = value; SetUpToDateAsFalse(); } }

            [Column("PointsToAdBalanceEnabled")]
            internal bool PointsToAdBalanceEnabled { get { return _PointsToAdBalanceEnabled; } set { _PointsToAdBalanceEnabled = value; SetUpToDateAsFalse(); } }

            [Column("CashBalanceEnabled")]
            internal bool CashBalanceEnabled { get { return _CashBalanceEnabled; } set { _CashBalanceEnabled = value; SetUpToDateAsFalse(); } }

            [Column("CashToAdBalanceEnabled")]
            internal bool CashToAdBalanceEnabled { get { return _CashToAdBalanceEnabled; } set { _CashToAdBalanceEnabled = value; SetUpToDateAsFalse(); } }

            [Column("CurrencyMode")]
            internal int CurrencyModeInt { get { return _CurrencyModeInt; } set { _CurrencyModeInt = value; SetUpToDateAsFalse(); } }

            [Column("TokenCryptocurrencyValueType")]
            internal int TokenCryptocurrencyValueTypeInt { get { return _TokenCryptocurrencyValueTypeInt; } set { _TokenCryptocurrencyValueTypeInt = value; SetUpToDateAsFalse(); } }

            [Column("MultiCurrencyProvider")]
            internal int MultiCurrencyProviderInt { get { return _MultiCurrencyProviderInt; } set { _MultiCurrencyProviderInt = value; SetUpToDateAsFalse(); } }

            [Column("WithdrawalVerificationCodeValidForMinutes")]
            internal int WithdrawalVerificationCodeValidForMinutes
            {
                get { return _WithdrawalVerificationCodeValidForMinutes; }
                set { _WithdrawalVerificationCodeValidForMinutes = value; SetUpToDateAsFalse(); }
            }

            [Column("WithdrawalEmailEnabled")]
            internal bool WithdrawalEmailEnabled
            {
                get { return _WithdrawalEmailEnabled; }
                set { _WithdrawalEmailEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("EnabledPayoutDays")]
            internal int EnabledPayoutDays { get { return _EnabledPayoutDays; } set { _EnabledPayoutDays = value; SetUpToDateAsFalse(); } }

            [Column("CommissionBalanceWithdrawalEnabled")]
            internal bool CommissionBalanceWithdrawalEnabled
            {
                get { return _CommissionBalanceWithdrawalEnabled; }
                set { _CommissionBalanceWithdrawalEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("MarketplaceBalanceEnabled")]
            internal bool MarketplaceBalanceEnabled
            {
                get { return _MarketplaceBalanceEnabled; }
                set { _MarketplaceBalanceEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("MarketplaceBalanceDepositEnabled")]
            internal bool MarketplaceBalanceDepositEnabled
            {
                get { return _MarketplaceBalanceDepositEnabled; }
                set { _MarketplaceBalanceDepositEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("MainToMarketplaceBalanceEnabled")]
            internal bool MainToMarketplaceBalanceEnabled
            {
                get { return _MainToMarketplaceBalanceEnabled; }
                set { _MainToMarketplaceBalanceEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("ExchangeRateAppIDCode")]
            internal string ExchangeRateAppIDCode
            {
                get { return _ExchangeRateAppIDCode; }
                set { _ExchangeRateAppIDCode = value;SetUpToDateAsFalse(); }
            }

            [Column("TransferFromPaymentProcessorsToAdBalanceEnabled")]
            internal bool TransferFromPaymentProcessorsToAdBalanceEnabled
            {
                get { return _TransferFromPaymentProcessorsToAdBalanceEnabled; }
                set { _TransferFromPaymentProcessorsToAdBalanceEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("BalancesVisibilityInUserMenuEnabled")]
            internal bool BalancesVisibilityInUserMenuEnabled
            {
                get { return _BalancesVisibilityInUserMenuEnabled; }
                set { _BalancesVisibilityInUserMenuEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("TransferFromTokenWalletToPurchaseBalanceEnabled")]
            internal bool TransferFromTokenWalletToPurchaseBalanceEnabled
            {
                get { return _TransferFromTokenWalletToPurchaseBalanceEnabled; }
                set { _TransferFromTokenWalletToPurchaseBalanceEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("TransferFromBTCWalletToPurchaseBalanceEnabled")]
            internal bool TransferFromBTCWalletToPurchaseBalanceEnabled
            {
                get { return _TransferFromBTCWalletToPurchaseBalanceEnabled; }
                set { _TransferFromBTCWalletToPurchaseBalanceEnabled = value; SetUpToDateAsFalse(); }
            }

            [Column("TransferFromMainBalanceToTokenWalletEnabled")]
            internal bool TransferFromMainBalanceToTokenWalletEnabled
            {
                get { return _TransferFromMainBalanceToTokenWalletEnabled; }
                set { _TransferFromMainBalanceToTokenWalletEnabled = value; SetUpToDateAsFalse(); }
            }

            private int _TransferMode, _InstantPayoutMinOffersCompleted, _InstantPayoutMinCashoutsNumber, _InstantPayoutMinRegisteredDays, _TransferFee, _MaximumPayoutPolicyInt,
                _MaximumPayoutPercentage, _CurrencyModeInt, _WithdrawalVerificationCodeValidForMinutes, _EnabledPayoutDays, _MultiCurrencyProviderInt, _TokenCryptocurrencyValueTypeInt;
            private string _transactionNote, _CurrencyPointsName, _YourInvoiceName, _ExchangeRateAppIDCode;
            private bool _instantPayout, _TransferSlidebarEnabled, _CommissionBalanceEnabled, _CommissionToMainBalanceEnabled, _TransferMainInAdBalanceEnabled, _ProportionalPayoutLimitsEnabled,
                _CommissionToAdBalanceEnabled, _AdPackTypeWithdrawLimitEnabled, _RefTiersMaxWeeklyPayoutEnabled, _PointsToAdBalanceEnabled,
                _CashBalanceEnabled, _CashToAdBalanceEnabled, _WithdrawalEmailEnabled, _CommissionBalanceWithdrawalEnabled, _MarketplaceBalanceEnabled,
                _MarketplaceBalanceDepositEnabled, _MainToMarketplaceBalanceEnabled, _TransferFromPaymentProcessorsToAdBalanceEnabled, _BalancesVisibilityInUserMenuEnabled,
                _TransferFromTokenWalletToPurchaseBalanceEnabled, _TransferFromMainBalanceToTokenWalletEnabled, _TransferFromBTCWalletToPurchaseBalanceEnabled;
            private Money _MemberMaxCashoutLimit, _GlobalCashoutLimitPerDay, _GlobalCashoutsToday, _MinimumTransferAmount, _MaximumPayoutConstant;

            //Save & reload section

            internal void ReloadPayments()
            {
                ReloadPartially(IsUpToDate, buildPaymentProperties());
            }

            internal void SavePayments()
            {
                SavePartially(IsUpToDate, buildPaymentProperties());
            }

            private PropertyInfo[] buildPaymentProperties()
            {
                var paymentsValues = new PropertyBuilder<AppSettingsTable>();
                paymentsValues
                    .Append(x => x.TransactionNote)
                    .Append(x => x.IsInstantPayout)
                    .Append(x => x.GlobalCashoutLimitPerDay)
                    .Append(x => x.GlobalCashoutsToday)
                    .Append(x => x.MinimumTransferAmount)
                    .Append(x => x.MemberMaxCashoutLimit)
                    .Append(x => x.TransferMode)
                    .Append(x => x.InstantPayoutMinOffersCompleted)
                    .Append(x => x.TransferFee)
                    .Append(x => x.InstantPayoutMinRegisteredDays)
                    .Append(x => x.MaximumPayoutPercentage)
                    .Append(x => x.MaximumPayoutConstant)
                    .Append(x => x.InstantPayoutMinCashoutsNumber)
                    .Append(x => x.CommissionBalanceEnabled)
                    .Append(x => x.MaximumPayoutPolicyInt)
                    .Append(x => x.CurrencyPointsName)
                    .Append(x => x.CommissionToMainBalanceEnabled)
                    .Append(x => x.TransferMainInAdBalanceEnabled)
                    .Append(x => x.ProportionalPayoutLimitsEnabled)
                    .Append(x => x.TransferSlidebarEnabled)
                    .Append(x => x.CommissionToAdBalanceEnabled)
                    .Append(x => x.AdPackTypeWithdrawLimitEnabled)
                    .Append(x => x.YourInvoiceName)
                    .Append(x => x.RefTiersMaxWeeklyPayoutEnabled)
                    .Append(x => x.PointsToAdBalanceEnabled)
                    .Append(x => x.CashBalanceEnabled)
                    .Append(x => x.CashToAdBalanceEnabled)
                    .Append(x => x.CurrencyModeInt)
                    .Append(x => x.MultiCurrencyProviderInt)
                    .Append(x => x.WithdrawalVerificationCodeValidForMinutes)
                    .Append(x => x.EnabledPayoutDays)
                    .Append(x => x.CommissionBalanceWithdrawalEnabled)
                    .Append(x => x.MarketplaceBalanceEnabled)
                    .Append(x => x.MarketplaceBalanceDepositEnabled)
                    .Append(x => x.MainToMarketplaceBalanceEnabled)
                    .Append(x => x.TransferFromPaymentProcessorsToAdBalanceEnabled)
                    .Append(x => x.ExchangeRateAppIDCode)
                    .Append(x => x.BalancesVisibilityInUserMenuEnabled)
                    .Append(x => x.TransferFromTokenWalletToPurchaseBalanceEnabled)
                    .Append(x => x.TransferFromBTCWalletToPurchaseBalanceEnabled)
                    .Append(x => x.TransferFromMainBalanceToTokenWalletEnabled)
                    .Append(x => x.TokenCryptocurrencyValueTypeInt);
                


                return paymentsValues.Build();
            }
        }

    }
}