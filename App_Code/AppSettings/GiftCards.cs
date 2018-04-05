using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class GiftCards
        {
            public static GiftCardMode Mode
            {
                get { return (GiftCardMode)appSettings.GiftCardMode; }

                set { appSettings.GiftCardMode = (int)value; }
            }

            public static string EmailDraft
            {
                get { return appSettings.GiftCodeEmailDraft; }

                set { appSettings.GiftCodeEmailDraft = value; }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadGiftCards();
            }

            public static void Save()
            {
                appSettings.SaveGiftCards();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("GiftCardMode")]
            internal int GiftCardMode { get { return _GiftCardMode; } set { _GiftCardMode = value; SetUpToDateAsFalse(); } }

            [Column("GiftCodeEmailDraft")]
            internal string GiftCodeEmailDraft { get { return _GiftCodeEmailDraft; } set { _GiftCodeEmailDraft = value; SetUpToDateAsFalse(); } }


            private int _GiftCardMode;
            private string _GiftCodeEmailDraft;

            //Save & reload section

            internal void ReloadGiftCards()
            {
                ReloadPartially(IsUpToDate, buildGiftCardsProperties());
            }

            internal void SaveGiftCards()
            {
                SavePartially(IsUpToDate, buildGiftCardsProperties());
            }

            private PropertyInfo[] buildGiftCardsProperties()
            {
                var exValues = new PropertyBuilder<AppSettingsTable>();

                exValues
                    .Append(x => x.GiftCodeEmailDraft)
                    .Append(x => x.GiftCardMode);
                return exValues.Build();
            }
        }

    }
}