using System;
using System.Configuration;
using Prem.PTC.Advertising;
using Prem.PTC.Utils;
using System.Reflection;
using System.Web;
using System.Diagnostics;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        #region ClientClassCreators
        private static AppSettingsTable appSettings;

        static AppSettings()
        {
            createAppSettingsInstanceIfNeeded();
        }

        private static bool createAppSettingsInstanceIfNeeded()
        {
            if (appSettings == null)
            {
                appSettings = new AppSettingsTable(); //_newAppSettingsTable();
                return true;
            }
            return false;
        }

        public static void Reload()
        {
            if (!createAppSettingsInstanceIfNeeded()) appSettings.Reload();
        }

        public static void Save()
        {
            if (appSettings != null) appSettings.Save();
        }

        public static bool IsNull()
        {
            return (appSettings == null);
        }

        #endregion ClientClassCreators
    }

}