using ExtensionMethods;
using Prem.PTC;
using Prem.PTC.Advertising;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.News
{
    public class ArticlesManager
    {
        public static List<Article> Get(ArticleFilter filter)
        {
            var categoryWhere = filter.CategoryId >= 0 ? String.Format(" AND CategoryId = {0}", filter.CategoryId) : String.Empty;
            var searchText = !String.IsNullOrEmpty(filter.SearchText) ? String.Format(
                " AND (Title LIKE '%{0}%' OR Keywords LIKE '%{0}%' OR ShortDescription LIKE '%{0}%')", filter.SearchText)
                : String.Empty;


            var query = String.Format(
                "SELECT * FROM Articles WHERE StatusInt = {0} AND Geolocation = '{3}'{4}{5} AND CreatedDate >= '{1}' ORDER BY Clicks DESC, CreatedDate DESC "
              + "OFFSET {6} ROWS FETCH NEXT {2} ROWS ONLY" ,
            (int)AdvertStatus.Active, AppSettings.ServerTime.AddDays(-filter.LastDays).ToDBString(), filter.FirstResults,
            filter.CountryCode, categoryWhere, searchText, filter.SkipFirst);

            return TableHelper.GetListFromRawQuery<Article>(query);
        }


    }
}