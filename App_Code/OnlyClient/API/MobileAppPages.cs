using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;

namespace Titan.API
{
    public enum MobileAppPage
    {
        Dashboard = 0,

        Referrals = 1,
        Jackpots  = 2,
        Withdraw  = 3,
        Transfer  = 4,
        Upgrade   = 5,
    }

    public class MobileAppPagesHelper
    {
        public static bool IsEnabled(MobileAppPage mobileAppPage)
        {
            switch(mobileAppPage)
            {
                case MobileAppPage.Dashboard:
                    return true;
                case MobileAppPage.Referrals:
                    return AppSettings.TitanFeatures.ReferralsDirectEnabled;
                case MobileAppPage.Jackpots:
                    return AppSettings.TitanFeatures.MoneyJackpotEnabled;
                case MobileAppPage.Withdraw:
                    return AppSettings.TitanFeatures.MoneyPayoutEnabled;
                case MobileAppPage.Transfer:
                    return AppSettings.TitanFeatures.MoneyTransferEnabled;
                case MobileAppPage.Upgrade:
                    return AppSettings.TitanFeatures.UpgradeEnabled;
                default:
                    return false;
            }
        }

        public static List<MobileAppPage> GetList()
        {
            MobileAppPage[] array = (MobileAppPage[])Enum.GetValues(typeof(MobileAppPage));
            return new List<MobileAppPage>(array);
        }
    }
}