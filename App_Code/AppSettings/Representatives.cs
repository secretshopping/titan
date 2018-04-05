using System;
using System.Reflection;
using Prem.PTC.Utils;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class Representatives
        {
            public enum RepresentativesPolicy
            {
                Null = 0,
                Users = 1,
                Automatic = 2,
            }

            public static RepresentativesPolicy Policy
            {
                get { return (RepresentativesPolicy)appSettings.RepresentativesPolicy; }
                set { appSettings.RepresentativesPolicy = (int)value; }
            }

            public static int NoOfRepresentatives
            {
                get { return appSettings.NoOfRepresentatives; }
                set { appSettings.NoOfRepresentatives = value; }
            }

            public static Decimal ProfitForRepresentantFromReferral
            {
                get { return appSettings.ProfitForRepresentantFromReferral; }
                set { appSettings.ProfitForRepresentantFromReferral = value; }
            }

            public static bool RepresentativesHelpWithdrawalEnabled
            {
                get { return appSettings.RepresentativesHelpWithdrawalEnabled; }
                set { appSettings.RepresentativesHelpWithdrawalEnabled = value; }
            }

            public static bool RepresentativesHelpDepositEnabled
            {
                get { return appSettings.RepresentativesHelpDepositEnabled; }
                set { appSettings.RepresentativesHelpDepositEnabled = value; }
            }

            public static decimal RepresentativesHelpWithdrawalFee
            {
                get { return appSettings.RepresentativesHelpWithdrawalFee; }
                set { appSettings.RepresentativesHelpWithdrawalFee = value; }
            }

            public static bool RepresentativesIgnoreWitdrawalRules
            {
                get { return appSettings.RepresentativesIgnoreWitdrawalRules; }
                set { appSettings.RepresentativesIgnoreWitdrawalRules = value; }
            }

            public static int RepresentativesEscrowTime
            {
                get { return appSettings.RepresentativesEscrowTime; }
                set { appSettings.RepresentativesEscrowTime = value; }
            }

            public static void Save()
            {
                appSettings.SaveRepresentatives();
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadRepresentatives();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("RepresentativesPolicy")]
            internal int RepresentativesPolicy { get { return _RepresentativesPolicy; } set { _RepresentativesPolicy = value; SetUpToDateAsFalse(); } }

            [Column("NoOfRepresentatives")]
            internal int NoOfRepresentatives { get { return _NoOfRepresentatives; } set { _NoOfRepresentatives = value; SetUpToDateAsFalse(); } }

            [Column("ProfitForRepresentantFromReferral")]
            internal Decimal ProfitForRepresentantFromReferral { get { return _ProfitForRepresentantFromReferral; } set { _ProfitForRepresentantFromReferral = value; SetUpToDateAsFalse(); } }

            [Column("RepresentativesHelpWithdrawalEnabled")]
            internal bool RepresentativesHelpWithdrawalEnabled { get { return _RepresentativesHelpWithdrawalEnabled; } set { _RepresentativesHelpWithdrawalEnabled = value; SetUpToDateAsFalse(); } }

            [Column("RepresentativesHelpDepositEnabled")]
            internal bool RepresentativesHelpDepositEnabled { get { return _RepresentativesHelpDepositEnabled; } set { _RepresentativesHelpDepositEnabled = value; SetUpToDateAsFalse(); } }

            [Column("RepresentativesHelpWithdrawalFee")]
            internal decimal RepresentativesHelpWithdrawalFee { get { return _RepresentativesHelpWithdrawalFee; } set { _RepresentativesHelpWithdrawalFee = value; SetUpToDateAsFalse(); } }

            [Column("RepresentativesIgnoreWitdrawalRules")]
            internal bool RepresentativesIgnoreWitdrawalRules { get { return _RepresentativesIgnoreWitdrawalRules; } set { _RepresentativesIgnoreWitdrawalRules = value; SetUpToDateAsFalse(); } }

            [Column("RepresentativesEscrowTime")]
            internal int RepresentativesEscrowTime { get { return _RepresentativesEscrowTime; } set { _RepresentativesEscrowTime = value; SetUpToDateAsFalse(); } }

            bool _RepresentativesHelpWithdrawalEnabled, _RepresentativesHelpDepositEnabled, _RepresentativesIgnoreWitdrawalRules;
            int _RepresentativesPolicy, _NoOfRepresentatives, _RepresentativesEscrowTime;
            decimal _ProfitForRepresentantFromReferral, _RepresentativesHelpWithdrawalFee;

            internal void SaveRepresentatives()
            {
                SavePartially(IsUpToDate, buildRepresentativesProperties());
            }

            internal void ReloadRepresentatives()
            {
                ReloadPartially(IsUpToDate, buildRepresentativesProperties());
            }

            private PropertyInfo[] buildRepresentativesProperties()
            {
                return new PropertyBuilder<AppSettingsTable>()
                    .Append(x => x.RepresentativesPolicy)
                    .Append(x => x.NoOfRepresentatives)
                    .Append(x => x.ProfitForRepresentantFromReferral)
                    .Append(x => x.RepresentativesHelpWithdrawalEnabled)
                    .Append(x => x.RepresentativesHelpDepositEnabled)
                    .Append(x => x.RepresentativesHelpWithdrawalFee)
                    .Append(x => x.RepresentativesIgnoreWitdrawalRules)
                    .Append(x => x.RepresentativesEscrowTime)
                    .Build();
            }
        }
    }
}