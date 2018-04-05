using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class TitanModules
        {
            public static string ModuleReqiuredMessage(int id)
            {
                return string.Format("Module #{0} required", id);
            }

            public static string ModuleErrorMessage(int id)
            {
                return string.Format("Purchase #{0} module first", id);

            }

            public static string ListOfModules
            {
                get
                {
                    return appSettings.TitanModules;
                }

                set
                {
                    appSettings.TitanModules = value;
                }
            }

            public static string ListOfProducts
            {
                get
                {
                    if (AppSettings.IsDemo && HttpContext.Current.Session["DemoProducts"] != null)
                        return HttpContext.Current.Session["DemoProducts"].ToString();

                    return appSettings.TitanProducts;
                }

                set
                {
                    appSettings.TitanProducts = value;
                }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadTitanModules();
            }

            public static void Save()
            {
                appSettings.SaveTitanModules();
            }
            public static bool HasModule(int moduleId)
            {
                return HasItem(ListOfModules, moduleId);
            }

            public static bool HasProduct(int productId)
            {
                return HasItem(ListOfProducts, productId);
            }

            private static bool HasItem(string list, int id)
            {
                if (string.IsNullOrEmpty(list))
                    return false;
                var items = list.Split(';');

                if (items.Contains(id.ToString()))
                    return true;
                return false;
            }
        }


        internal partial class AppSettingsTable : BaseTableObject
        {

            [Column("TitanModules")]
            internal string TitanModules { get { return _TitanModules; } set { _TitanModules = value; SetUpToDateAsFalse(); } }

            [Column("TitanProducts")]
            internal string TitanProducts { get { return _TitanProducts; } set { _TitanProducts = value; SetUpToDateAsFalse(); } }


            private string _TitanModules, _TitanProducts;


            //Save & reload section

            internal void ReloadTitanModules()
            {
                ReloadPartially(IsUpToDate, buildTitanModulesProperties());
            }

            internal void SaveTitanModules()
            {
                SavePartially(IsUpToDate, buildTitanModulesProperties());
            }

            private PropertyInfo[] buildTitanModulesProperties()
            {
                var paymentsValues = new PropertyBuilder<AppSettingsTable>();
                paymentsValues
                    .Append(x => x.TitanProducts)
                    .Append(x => x.TitanModules);

                return paymentsValues.Build();
            }
        }

        
    }
}