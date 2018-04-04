using System;
using System.Reflection;
using Prem.PTC.Utils;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class Referrals
        {
            /// <summary>
            /// Normal referrals: users who left field Referer blank
            /// Bot: artifical users
            /// All: Normal & Bot
            /// </summary>
            public enum RentingOption
            {
                Null = 0,
                Normal = 1,
                Bot = 2,
                All = 3,
                DirectReferrals = 4
            }

            public enum AutopayPolicy
            {
                UserChooses = 1,
                AllReferrals = 2
            }

            public static RentingOption Renting
            {
                get { return (RentingOption)appSettings.ReferralsRentingOption; }
                set { appSettings.ReferralsRentingOption = (int)value; }
            }


            public static int MinTotalClicksToRentRefs
            {
                get { return appSettings.MinTotalClicksToRentRefs; }
                set { appSettings.MinTotalClicksToRentRefs = value; }
            }

            public static AutopayPolicy RentedRefAutopayPolicy
            {
                get { return (AutopayPolicy)appSettings.RentedRefAutopayPolicy; }
                set { appSettings.RentedRefAutopayPolicy = (int)value; }
            }

            /// <summary>
            /// How much adverts must be clicked to get profit 
            /// from direct and rented referrals on next day
            /// </summary>
            public static int MinDailyClicksToEarnFromRefs
            {
                get { return appSettings.MinDailyClicksToEarnFromRefs; }
                set { appSettings.MinDailyClicksToEarnFromRefs = value; }
            }           

            public static int ReferralEarningsUpToTier
            {
                get { return appSettings.ReferralEarningsUpToTier; }
                set { appSettings.ReferralEarningsUpToTier = value; }
            }

            public static bool AreIndirectReferralsEnabled
            {
                get
                {
                    if (ReferralEarningsUpToTier > 1)
                        return true;
                    return false;
                }
            }

            public static Money ReferralPoolRotatorPricePerMonth
            {
                get { return appSettings.ReferralPoolRotatorPricePerMonth; }
                set { appSettings.ReferralPoolRotatorPricePerMonth = value; }
            }

            public static Decimal ReferralPoolRotatorPricePerMonthInAdCredits
            {
                get { return appSettings.ReferralPoolRotatorPricePerMonthInAdCredits; }
                set { appSettings.ReferralPoolRotatorPricePerMonthInAdCredits = value; }
            }

            public static int ResolveReferralsAfterSpecifiedDays
            {
                get { return appSettings.ResolveReferralsAfterSpecifiedDays; }
                set { appSettings.ResolveReferralsAfterSpecifiedDays = value; }
            }

            public static void Save()
            {
                appSettings.SaveReferrals();
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadReferrals();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("RefferalsRentingOption")]
            internal int ReferralsRentingOption { get { return _referralsRentingOption; } set { _referralsRentingOption = value; SetUpToDateAsFalse(); } }

            [Column("MinTotalClicksToRentRefs")]
            internal int MinTotalClicksToRentRefs { get { return _minTotalClicksToRentRefs; } set { _minTotalClicksToRentRefs = value; SetUpToDateAsFalse(); } }

            [Column("MinDailyClicksToEarnFromRefs")]
            internal int MinDailyClicksToEarnFromRefs { get { return _minDailyClicksToEarnFromRefs; } set { _minDailyClicksToEarnFromRefs = value; SetUpToDateAsFalse(); } }

            //Ref tirers
            [Column("ReferralEarningsUpToTier")]
            internal int ReferralEarningsUpToTier { get { return _ReferralEarningsUpToTier; } set { _ReferralEarningsUpToTier = value; SetUpToDateAsFalse(); } }

            [Column("ReferralEarningsFromTier2")]
            internal int ReferralEarningsFromTier2 { get { return _REFT2; } set { _REFT2 = value; SetUpToDateAsFalse(); } }

            [Column("ReferralEarningsFromTier3")]
            internal int ReferralEarningsFromTier3 { get { return _REFT3; } set { _REFT3 = value; SetUpToDateAsFalse(); } }

            [Column("ReferralEarningsFromTier4")]
            internal int ReferralEarningsFromTier4 { get { return _REFT4; } set { _REFT4 = value; SetUpToDateAsFalse(); } }

            [Column("ReferralEarningsFromTier5")]
            internal int ReferralEarningsFromTier5 { get { return _REFT5; } set { _REFT5 = value; SetUpToDateAsFalse(); } }

            [Column("ReferralEarningsFromTier6")]
            internal int ReferralEarningsFromTier6 { get { return _REFT6; } set { _REFT6 = value; SetUpToDateAsFalse(); } }

            [Column("ReferralEarningsFromTier7")]
            internal int ReferralEarningsFromTier7 { get { return _REFT7; } set { _REFT7 = value; SetUpToDateAsFalse(); } }

            [Column("ReferralEarningsFromTier8")]
            internal int ReferralEarningsFromTier8 { get { return _REFT8; } set { _REFT8 = value; SetUpToDateAsFalse(); } }

            [Column("ReferralEarningsFromTier9")]
            internal int ReferralEarningsFromTier9 { get { return _REFT9; } set { _REFT9 = value; SetUpToDateAsFalse(); } }

            [Column("ReferralEarningsFromTier10")]
            internal int ReferralEarningsFromTier10 { get { return _REFT10; } set { _REFT10 = value; SetUpToDateAsFalse(); } }
            
            [Column("ReferralPoolRotatorPricePerMonth")]
            internal Money ReferralPoolRotatorPricePerMonth { get { return _ReferralPoolRotatorPricePerMonth; } set { _ReferralPoolRotatorPricePerMonth = value; SetUpToDateAsFalse(); } }

            [Column("ReferralPoolRotatorPricePerMonthInAdCredits")]
            internal Decimal ReferralPoolRotatorPricePerMonthInAdCredits { get { return _ReferralPoolRotatorPricePerMonthInAdCredits; } set { _ReferralPoolRotatorPricePerMonthInAdCredits = value; SetUpToDateAsFalse(); } }

            [Column("RentedRefAutopayPolicy")]
            internal int RentedRefAutopayPolicy { get { return _RentedRefAutopayPolicy; } set { _RentedRefAutopayPolicy = value; SetUpToDateAsFalse(); } }

            [Column("ResolveReferralsAfterSpecifiedDays")]
            internal int ResolveReferralsAfterSpecifiedDays { get { return _ResolveReferralsAfterSpecifiedDays; } set { _ResolveReferralsAfterSpecifiedDays = value; SetUpToDateAsFalse(); } }

            int _referralsRentingOption, _minTotalClicksToRentRefs,
                    _minDailyClicksToEarnFromRefs, _ReferralEarningsUpToTier, _REFT2, _REFT3, _REFT4, _REFT5, _REFT6, _REFT7, _REFT8, _REFT9, _REFT10, _RentedRefAutopayPolicy,
                    _ResolveReferralsAfterSpecifiedDays;            
            Money _ReferralPoolRotatorPricePerMonth;
            Decimal _ReferralPoolRotatorPricePerMonthInAdCredits;

            internal void SaveReferrals()
            {
                SavePartially(IsUpToDate, buildReferralsProperties());
            }

            internal void ReloadReferrals()
            {
                ReloadPartially(IsUpToDate, buildReferralsProperties());
            }

            private PropertyInfo[] buildReferralsProperties()
            {
                return new PropertyBuilder<AppSettingsTable>()
                    .Append(x => x.ReferralsRentingOption)
                    .Append(x => x.MinTotalClicksToRentRefs)
                    .Append(x => x.MinDailyClicksToEarnFromRefs)
                    .Append(x => x.ReferralEarningsFromTier10)
                    .Append(x => x.ReferralEarningsFromTier9)
                    .Append(x => x.ReferralEarningsFromTier8)
                    .Append(x => x.ReferralEarningsFromTier7)
                    .Append(x => x.ReferralEarningsFromTier6)
                    .Append(x => x.ReferralEarningsFromTier5)
                    .Append(x => x.ReferralEarningsFromTier4)
                    .Append(x => x.ReferralEarningsFromTier3)
                    .Append(x => x.ReferralEarningsFromTier2)
                    .Append(x => x.ReferralEarningsUpToTier)
                    .Append(x => x.ReferralPoolRotatorPricePerMonth)
                    .Append(x => x.ReferralPoolRotatorPricePerMonthInAdCredits)
                    .Append(x => x.RentedRefAutopayPolicy)
                    .Append(x => x.ResolveReferralsAfterSpecifiedDays)
                    .Build(); 
            }
        }
    }
}