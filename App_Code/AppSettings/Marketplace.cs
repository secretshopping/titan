using Prem.PTC.Utils;
using System.Reflection;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static partial class Marketplace
        {
            public static bool CanUsersAddMarketplaceProducts
            {
                get { return appSettings.CanUsersAddMarketplaceProducts; }
                set { appSettings.CanUsersAddMarketplaceProducts = value; }
            }

            public static bool MarketplaceAllowPurchaseFromAdBalance
            {
                get { return appSettings.MarketplaceAllowPurchaseFromAdBalance; }
                set { appSettings.MarketplaceAllowPurchaseFromAdBalance = value; }
            }

            public static bool MarketplaceAllowPurchaseFromPaymentProcessors
            {
                get { return appSettings.MarketplaceAllowPurchaseFromPaymentProcessors; }
                set { appSettings.MarketplaceAllowPurchaseFromPaymentProcessors = value; }
            }

            public static bool MarketplaceUsersPromoteByLinkEnabled
            {
                get { return appSettings.MarketplaceUsersPromoteByLinkEnabled; }
                set { appSettings.MarketplaceUsersPromoteByLinkEnabled = value; }
            }

            public static int MarketplacePromoteCommission
            {
                get { return appSettings.MarketplacePromoteCommission; }
                set { appSettings.MarketplacePromoteCommission = value; }
            }

            public static bool AllowPurchaseFromMarketplaceBalance
            {
                get { return appSettings.MarketplaceAllowPurchaseFromMarketplaceBalance; }
                set { appSettings.MarketplaceAllowPurchaseFromMarketplaceBalance = value; }
            }

            /// <summary>
            /// -1 = never
            /// </summary>
            public static int MarketplaceFundsExpireAfterDays
            {
                get { return appSettings.MarketplaceFundsExpireAfterDays; }
                set { appSettings.MarketplaceFundsExpireAfterDays = value; }
            }

            public static void Save()
            {
                appSettings.SaveMarketplace();
            }
            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) { appSettings.ReloadMarketplace(); }
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            #region Columns

            [Column("CanUsersAddMarketplaceProducts")]
            internal bool CanUsersAddMarketplaceProducts
            {
                get { return _CanUsersAddMarketplaceProducts; }
                set { _CanUsersAddMarketplaceProducts = value; SetUpToDateAsFalse(); }
            }

            [Column("MarketplaceAllowPurchaseFromAdBalance")]
            internal bool MarketplaceAllowPurchaseFromAdBalance
            {
                get { return _MarketplaceAllowPurchaseFromAdBalance; }
                set { _MarketplaceAllowPurchaseFromAdBalance = value; SetUpToDateAsFalse(); }
            }

            [Column("MarketplaceAllowPurchaseFromPaymentProcessors")]
            internal bool MarketplaceAllowPurchaseFromPaymentProcessors
            {
                get { return _MarketplaceAllowPurchaseFromPaymentProcessors; }
                set { _MarketplaceAllowPurchaseFromPaymentProcessors = value; SetUpToDateAsFalse(); }
            }

            [Column("MarketplaceUsersPromoteByLinkEnabled")]
            internal bool MarketplaceUsersPromoteByLinkEnabled
            {
                get { return _MarketplaceUsersPromoteByLinkEnabled; }
                set { _MarketplaceUsersPromoteByLinkEnabled = value;  SetUpToDateAsFalse(); }
            }

            [Column("MarketplacePromoteCommission")]
            internal int MarketplacePromoteCommission
            {
                get { return _MarketplacePromoteCommission; }
                set { _MarketplacePromoteCommission = value; SetUpToDateAsFalse(); }
            }

            [Column("MarketplaceAllowPurchaseFromMarketplaceBalance")]
            internal bool MarketplaceAllowPurchaseFromMarketplaceBalance
            {
                get { return _MarketplaceAllowPurchaseFromMarketplaceBalance; }
                set { _MarketplaceAllowPurchaseFromMarketplaceBalance = value; SetUpToDateAsFalse(); }
            }

            [Column("MarketplaceFundsExpireAfterDays")]
            internal int MarketplaceFundsExpireAfterDays
            {
                get { return _MarketplaceFundsExpireAfterDays; }
                set { _MarketplaceFundsExpireAfterDays = value; SetUpToDateAsFalse(); }
            }

            bool _CanUsersAddMarketplaceProducts, _MarketplaceAllowPurchaseFromAdBalance, _MarketplaceAllowPurchaseFromPaymentProcessors,
                 _MarketplaceUsersPromoteByLinkEnabled, _MarketplaceAllowPurchaseFromMarketplaceBalance;
            int _MarketplacePromoteCommission, _MarketplaceFundsExpireAfterDays;
            #endregion

            internal void SaveMarketplace()
            {
                SavePartially(IsUpToDate, buildMarketplaceProperties());
            }
            internal void ReloadMarketplace()
            {
                ReloadPartially(IsUpToDate, buildMarketplaceProperties());
            }

            private PropertyInfo[] buildMarketplaceProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.CanUsersAddMarketplaceProducts)
                    .Append(x => x.MarketplaceAllowPurchaseFromAdBalance)
                    .Append(x => x.MarketplaceAllowPurchaseFromPaymentProcessors)
                    .Append(x => x.MarketplaceUsersPromoteByLinkEnabled)
                    .Append(x => x.MarketplacePromoteCommission)
                    .Append(x => x.MarketplaceAllowPurchaseFromMarketplaceBalance)
                    .Append(x => x.MarketplaceFundsExpireAfterDays);
                
                return exValues.Build();
            }
        }
    }
}
