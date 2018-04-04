using Prem.PTC.Utils;
using System.Reflection;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class PaidToPromote
        {
            public static int RotationSlotsCount
            {
                get { return appSettings.PaidToPromoteRotationSlotsCount; }
                set { appSettings.PaidToPromoteRotationSlotsCount = value; }
            }

            public static int BannerDimensionId
            {
                get { return appSettings.PaitToPromoteBannerDimensionId; }
                set { appSettings.PaitToPromoteBannerDimensionId = value; }
            }            

            public static int AdDuration
            {
                get { return appSettings.PaidToPromoteAdDuration; }
                set { appSettings.PaidToPromoteAdDuration = value; }
            }

            public static Money CostPerMillePrice
            {
                get { return appSettings.PaidToPromoteCostPerMillePrice; }
                set { appSettings.PaidToPromoteCostPerMillePrice = value; }
            }

            public static Money GeolocationPrice
            {
                get { return appSettings.PaidToPromoteGeolocationPrice; }
                set { appSettings.PaidToPromoteGeolocationPrice = value; }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadPaidToPromote();
            }

            public static void Save()
            {
                appSettings.SavePaidToPromote();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("PaidToPromoteRotationSlotsCount")]
            internal int PaidToPromoteRotationSlotsCount
            {
                get { return _PaidToPromoteRotationSlotsCount; }
                set { _PaidToPromoteRotationSlotsCount = value; SetUpToDateAsFalse(); }
            }

            [Column("PaitToPromoteBannerDimensionId")]
            internal int PaitToPromoteBannerDimensionId
            {
                get { return _PaitToPromoteBannerDimensionId; }
                set { _PaitToPromoteBannerDimensionId = value; SetUpToDateAsFalse(); }
            }

            [Column("PaidToPromoteAdDuration")]
            internal int PaidToPromoteAdDuration
            {
                get { return _PaidToPromoteAdDuration; }
                set { _PaidToPromoteAdDuration = value; SetUpToDateAsFalse(); }
            }

            [Column("PaidToPromoteCostPerMillePrice")]
            internal Money PaidToPromoteCostPerMillePrice
            {
                get { return _PaidToPromoteCostPerMillePrice; }
                set { _PaidToPromoteCostPerMillePrice = value; SetUpToDateAsFalse(); }
            }

            [Column("PaidToPromoteGeolocationPrice")]
            internal Money PaidToPromoteGeolocationPrice
            {
                get { return _PaidToPromoteGeolocationPrice; }
                set { _PaidToPromoteGeolocationPrice = value; SetUpToDateAsFalse(); }
            }

            private int _PaidToPromoteRotationSlotsCount, _PaitToPromoteBannerDimensionId, _PaidToPromoteAdDuration;
            private Money _PaidToPromoteCostPerMillePrice, _PaidToPromoteGeolocationPrice;

            internal void ReloadPaidToPromote()
            {
                ReloadPartially(IsUpToDate, buildPaidToPromoteProperties());
            }

            internal void SavePaidToPromote()
            {
                SavePartially(IsUpToDate, buildPaidToPromoteProperties());
            }

            private PropertyInfo[] buildPaidToPromoteProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.PaidToPromoteRotationSlotsCount)
                    .Append(x => x.PaitToPromoteBannerDimensionId)
                    .Append(x => x.PaidToPromoteCostPerMillePrice)
                    .Append(x => x.PaidToPromoteAdDuration)
                    .Append(x => x.PaidToPromoteGeolocationPrice);

                return exValues.Build();
            }
        }
    }
}