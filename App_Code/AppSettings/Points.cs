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
        public static class Points
        {
            public static int TotalGeneratedUpToYesterday
            {
                get { return appSettings.TotalPointsGenerated; }
                set
                {
                    appSettings.TotalPointsGenerated = value;
                }
            }

            public static int TotalExchanged
            {
                get { return appSettings.TotalPointsExchangedForGiftCards; }
                set
                {
                    appSettings.TotalPointsExchangedForGiftCards = value;
                }
            }

            public static int TotalInSystem
            {
                get
                {
                    using (var bridge = ParserPool.Acquire(Database.Client))
                    {
                        string command = "SELECT SUM(Balance4) FROM Users";
                        return (int)bridge.Instance.ExecuteRawCommandScalar(command);
                    }
                }

            }

            public static int TotalGenerated
            {
                get
                {
                    using (var bridge = ParserPool.Acquire(Database.Client))
                    {
                        string command = "SELECT SUM(TotalPointsGenerated) FROM Users";
                        return (int)bridge.Instance.ExecuteRawCommandScalar(command);
                    }
                }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadPoints();
            }

            public static void Save()
            {
                appSettings.SavePoints();
            }

            public static int LockDays
            {
                get { return appSettings.LockDays; }
                set
                {
                    appSettings.LockDays = value;
                }
            }

            public static int LockPoints
            {
                get { return appSettings.LockPoints; }
                set
                {
                    appSettings.LockPoints = value;
                }
            }

            public static bool IsLock
            {
                get { return appSettings.IsLock; }
                set
                {
                    appSettings.IsLock = value;
                }
            }

            public static bool PointsEnabled
            {
                get { return appSettings.PointsEnabled; }
                set
                {
                    appSettings.PointsEnabled = value;
                }
            }

            public static bool LevelMembershipPolicyEnabled
            {
                get { return appSettings.LevelMembershipPolicyEnabled; }
                set
                {
                    appSettings.LevelMembershipPolicyEnabled = value;
                }
            }

            public static bool PointsDepositEnable
            {
                get { return appSettings.PointsDepositEnable; }
                set
                {
                    appSettings.PointsDepositEnable = value;
                }
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("TotalPointsExchangedForGiftCards")]
            internal int TotalPointsExchangedForGiftCards { get { return _TotalPointsExchangedForGiftCards; } set { _TotalPointsExchangedForGiftCards = value; SetUpToDateAsFalse(); } }

            [Column("TotalPointsGenerated")]
            internal int TotalPointsGenerated { get { return _TotalPointsGenerated; } set { _TotalPointsGenerated = value; SetUpToDateAsFalse(); } }

            [Column("LockDays")]
            internal int LockDays { get { return _lockDays; } set { _lockDays = value; SetUpToDateAsFalse(); } }

            [Column("LockPoints")]
            internal int LockPoints { get { return _lockPoints; } set { _lockPoints = value; SetUpToDateAsFalse(); } }

            [Column("IsLock")]
            internal bool IsLock { get { return _isLock; } set { _isLock = value; SetUpToDateAsFalse(); } }

            [Column("PointsEnabled")]
            internal bool PointsEnabled { get { return _PointsEnabled; } set { _PointsEnabled = value; SetUpToDateAsFalse(); } }

            [Column("LevelMembershipPolicyEnabled")]
            internal bool LevelMembershipPolicyEnabled { get { return _LevelMembershipPolicyEnabled; } set { _LevelMembershipPolicyEnabled = value; SetUpToDateAsFalse(); } }

            [Column("PointsDepositEnable")]
            internal bool PointsDepositEnable { get { return _PointsDepositEnable; } set { _PointsDepositEnable = value; SetUpToDateAsFalse(); } }


            private bool _isLock, _PointsEnabled, _LevelMembershipPolicyEnabled, _PointsDepositEnable;
            private int _TotalPointsExchangedForGiftCards, _TotalPointsGenerated, _lockDays, _lockPoints;

            //Save & reload section

            internal void ReloadPoints()
            {
                ReloadPartially(IsUpToDate, buildPointsProperties());
            }

            internal void SavePoints()
            {
                SavePartially(IsUpToDate, buildPointsProperties());
            }

            private PropertyInfo[] buildPointsProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.TotalPointsExchangedForGiftCards)
                    .Append(x => x.TotalPointsGenerated)
                    .Append(x => x.LockDays)
                    .Append(x => x.LockPoints)
                    .Append(x => x.IsLock)
                    .Append(x => x.LevelMembershipPolicyEnabled)
                    .Append(x => x.PointsDepositEnable)
                    .Append(x => x.PointsEnabled);
                return exValues.Build();
            }
        }

    }
}