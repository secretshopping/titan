using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC.Utils;
using System.Reflection;
/// <summary>
/// Summary description for BitCoin
/// </summary>

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class DiceGame
        {
            public static bool IsDiceGameEnabled { get { return appSettings.IsDiceGameEnabled; } set { appSettings.IsDiceGameEnabled = value; } }
            public static Decimal MaxBitCoinProfitPercent { get { return appSettings.MaxBitCoinProfitPercent; } set { appSettings.MaxBitCoinProfitPercent = value; } }
            public static Money MinBitCoinBet { get { return appSettings.MinBitCoinBet; } set { appSettings.MinBitCoinBet = value; } }
            public static Int32 HouseEdgePercent { get { return appSettings.HouseEdgePercent; } set { appSettings.HouseEdgePercent = value; } }
            public static Decimal MaxChance { get { return appSettings.MaxChance; } set { appSettings.MaxChance = value; } }

            public static Decimal MaxKellyLevel { get { return appSettings.MaxKellyLevel; } set { appSettings.MaxKellyLevel = value; } }

            public static Int32 MaxKellyLevelInt { get { return appSettings.MaxKellyLevelInt; } set { appSettings.MaxKellyLevelInt = value; } }

            public static Money AdminInvestment { get { return appSettings.AdminInvestment; } set { appSettings.AdminInvestment = value; } }

            public static Money HouseProfit { get { return appSettings.HouseProfit; } set { appSettings.HouseProfit = value; } }

            public static void Save()
            {
                appSettings.SaveDiceGame();
            }
            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadDiceGame();
            }
        }
        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("IsDiceGameEnabled")]
            internal bool IsDiceGameEnabled
            {
                get { return _IsDiceGameEnabled; }
                set { _IsDiceGameEnabled = value; SetUpToDateAsFalse(); }
            }
            [Column("MaxBitCoinProfitPercent")]
            internal Decimal MaxBitCoinProfitPercent
            {
                get { return _MaxBitCoinProfitPercent; }
                set { _MaxBitCoinProfitPercent = value; SetUpToDateAsFalse(); }
            }

            [Column("MinBitCoinBet")]
            internal Money MinBitCoinBet
            {
                get { return _MinBitCoinBet; }
                set { _MinBitCoinBet = value; SetUpToDateAsFalse(); }
            }

            [Column("MaxChance")]
            internal Decimal MaxChance
            {
                get { return _MaxChance; }
                set { _MaxChance = value; SetUpToDateAsFalse(); }
            }

            [Column("AdminInvestment")]
            internal Money AdminInvestment
            {
                get { return _AdminInvestment; }
                set { _AdminInvestment = value; SetUpToDateAsFalse(); }
            }

            [Column("HouseProfit")]
            internal Money HouseProfit
            {
                get { return _HouseProfit; }
                set { _HouseProfit = value; SetUpToDateAsFalse(); }
            }

            [Column("MaxKellyLevel")]
            internal Decimal MaxKellyLevel
            {
                get { return _MaxKellyLevel; }
                set { _MaxKellyLevel = value; SetUpToDateAsFalse(); }
            }

            [Column("MaxKellyLevelInt")]
            internal Int32 MaxKellyLevelInt
            {
                get { return _MaxKellyLevelInt; }
                set { _MaxKellyLevelInt = value; SetUpToDateAsFalse(); }
            }

            [Column("HouseEdgePercent")]
            internal Int32 HouseEdgePercent
            {
                get { return _HouseEdgePercent; }
                set { _HouseEdgePercent = value; SetUpToDateAsFalse(); }
            }

            bool _IsDiceGameEnabled;
            Decimal _MaxBitCoinProfitPercent, _MaxChance, _MaxKellyLevel;
            Int32 _HouseEdgePercent, _MaxKellyLevelInt;
            Money _AdminInvestment, _MinBitCoinBet, _HouseProfit;

            internal void SaveDiceGame()
            {
                SavePartially(IsUpToDate, buildDiceGameProperties());
            }
            internal void ReloadDiceGame()
            {
                ReloadPartially(IsUpToDate, buildDiceGameProperties());
            }

            private PropertyInfo[] buildDiceGameProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.IsDiceGameEnabled)
                    .Append(x => x.MaxBitCoinProfitPercent)
                    .Append(x => x.MinBitCoinBet)
                    .Append(x => x.HouseEdgePercent)
                    .Append(x => x.MaxKellyLevel)
                    .Append(x => x.MaxKellyLevelInt)
                    .Append(x => x.AdminInvestment)
                    .Append(x => x.HouseProfit)
                    .Append(x => x.MaxChance);

                return exValues.Build();
            }
        }
    }
}
