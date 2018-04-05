using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Titan.Shares;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class Shares
        {
            public static PorftolioSharesPolicy Policy
            {
                get { return (PorftolioSharesPolicy)appSettings.PorftolioSharesPolicyInt; }

                set { appSettings.PorftolioSharesPolicyInt = (int)value; }
            }

            public static int SharesMarketSaleCommission
            {
                get { return appSettings.SharesMarketSaleCommission; }
                set
                {
                    appSettings.SharesMarketSaleCommission = value;
                }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadShares();
            }

            public static void Save()
            {
                appSettings.SaveShares();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("PorftolioSharesPolicy")]
            internal int PorftolioSharesPolicyInt { get { return _PorftolioSharesPolicyInt; } set { _PorftolioSharesPolicyInt = value; SetUpToDateAsFalse(); } }

            [Column("SharesMarketSaleCommission")]
            internal int SharesMarketSaleCommission { get { return _SharesMarketSaleCommission; } set { _SharesMarketSaleCommission = value; SetUpToDateAsFalse(); } }

            private int _PorftolioSharesPolicyInt, _SharesMarketSaleCommission;

            //Save & reload section

            internal void ReloadShares()
            {
                ReloadPartially(IsUpToDate, buildSharesProperties());
            }

            internal void SaveShares()
            {
                SavePartially(IsUpToDate, buildSharesProperties());
            }

            private PropertyInfo[] buildSharesProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.PorftolioSharesPolicyInt)
                    .Append(x => x.SharesMarketSaleCommission);
                return exValues.Build();
            }
        }

    }
}