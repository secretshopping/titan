using Prem.PTC.Utils;
using System.Reflection;

namespace Prem.PTC
{
    public enum ResultsView
    {
        Hide = 0,
        Last1 = 1,
        Last2 = 2,
        Last3 = 3
    }

    public static partial class AppSettings
    {
        public static class RollDiceLottery
        {
            public static Money RollDiceLotteryFeePrice
            {
                get { return appSettings.RollDiceLotteryFeePrice; }
                set { appSettings.RollDiceLotteryFeePrice = value; }
            }

            public static Money RollDiceLotteryPrizeMainBalance
            {
                get { return appSettings.RollDiceLotteryPrizeMainBalance; }
                set { appSettings.RollDiceLotteryPrizeMainBalance = value; }
            }

            public static Money RollDiceLotteryPrizeAdBalance
            {
                get { return appSettings.RollDiceLotteryPrizeAdBalance; }
                set { appSettings.RollDiceLotteryPrizeAdBalance = value; }
            }

            public static ResultsView RollDiceLotteryLastResultsViewCount
            {
                get { return (ResultsView)appSettings.RollDiceLotteryLastResultsViewCount; }
                set { appSettings.RollDiceLotteryLastResultsViewCount = (int)value; }
            }

            public static int RollDiceLotteryPrizePoints
            {
                get { return appSettings.RollDiceLotteryPrizePoints; }
                set { appSettings.RollDiceLotteryPrizePoints = value; }
            }

            public static int RollDiceLotteryGameTime
            {
                get { return appSettings.RollDiceLotteryGameTime; }
                set { appSettings.RollDiceLotteryGameTime = value; }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadRollDiceLottery();
            }

            public static void Save()
            {
                appSettings.SaveRollDiceLottery();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("RollDiceLotteryFeePrice")]
            internal Money RollDiceLotteryFeePrice
            {
                get { return _RollDiceLotteryFeePrice; }
                set { _RollDiceLotteryFeePrice = value; SetUpToDateAsFalse(); }
            }

            [Column("RollDiceLotteryPrizeMainBalance")]
            internal Money RollDiceLotteryPrizeMainBalance
            {
                get { return _RollDiceLotteryPrizeMainBalance; }
                set { _RollDiceLotteryPrizeMainBalance = value; SetUpToDateAsFalse(); }
            }

            [Column("RollDiceLotteryPrizeAdBalance")]
            internal Money RollDiceLotteryPrizeAdBalance
            {
                get { return _RollDiceLotteryPrizeAdBalance; }
                set { _RollDiceLotteryPrizeAdBalance = value; SetUpToDateAsFalse(); }
            }

            [Column("RollDiceLotteryLastResultsViewCount")]
            internal int RollDiceLotteryLastResultsViewCount
            {
                get { return _RollDiceLotteryLastResultsViewCount; }
                set { _RollDiceLotteryLastResultsViewCount = value; SetUpToDateAsFalse(); }
            }

            [Column("RollDiceLotteryPrizePoints")]
            internal int RollDiceLotteryPrizePoints
            {
                get { return _RollDiceLotteryPrizePoints; }
                set { _RollDiceLotteryPrizePoints = value; SetUpToDateAsFalse(); }
            }

            [Column("RollDiceLotteryGameTime")]
            internal int RollDiceLotteryGameTime
            {
                get { return _RollDiceLotteryGameTime; }
                set { _RollDiceLotteryGameTime = value; SetUpToDateAsFalse(); }
            }

            private Money _RollDiceLotteryFeePrice, _RollDiceLotteryPrizeMainBalance, _RollDiceLotteryPrizeAdBalance;
            private int _RollDiceLotteryLastResultsViewCount, _RollDiceLotteryPrizePoints, _RollDiceLotteryGameTime;

            internal void ReloadRollDiceLottery()
            {
                ReloadPartially(IsUpToDate, buildRollDiceLotteryProperties());
            }

            internal void SaveRollDiceLottery()
            {
                SavePartially(IsUpToDate, buildRollDiceLotteryProperties());
            }

            private PropertyInfo[] buildRollDiceLotteryProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.RollDiceLotteryFeePrice)
                    .Append(x => x.RollDiceLotteryLastResultsViewCount)
                    .Append(x => x.RollDiceLotteryGameTime)
                    .Append(x => x.RollDiceLotteryPrizeMainBalance)
                    .Append(x => x.RollDiceLotteryPrizeAdBalance)
                    .Append(x => x.RollDiceLotteryPrizePoints);

                return exValues.Build();
            }
        }

    }
}