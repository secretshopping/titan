using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Titan.News
{
    public class NewsCountriesHelper
    {
        public static List<string> GetAvailableCountryCodes()
        {
            string[] languages = AppSettings.Site.ChoosedLanguages.Split('#');
            List<string> list = new List<string>();

            foreach(var language in languages)
            {
                RegionInfo regionInfo = new RegionInfo(language);
                list.Add(regionInfo.TwoLetterISORegionName.ToUpper());
            }

            return list;
        }

        public static ListItem[] ListItems
        {
            get
            {
                var list = GetAvailableCountryCodes();
                var query = from string cc in list
                            select new ListItem(CountryManager.GetCountryName(cc), cc);

                return query.ToArray();
            }
        }
    }
}