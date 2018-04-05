using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Titan.News
{
    public class NewsStatistics
    {
        public static int Articles
        {
            get
            {
                var query = String.Format("SELECT COUNT(Id) FROM Articles WHERE StatusInt != {0}", (int)UniversalStatus.Deleted);
                return (int)TableHelper.SelectScalar(query);
            }
        }

        public static int ArticleReads
        {
            get
            {
                var query = String.Format("SELECT COUNT(Id) FROM ArticleViews");
                return (int)TableHelper.SelectScalar(query);
            }
        }

        public static int UniqueVisitors
        {
            get
            {
                var query = String.Format("SELECT COUNT(DISTINCT(IP)) FROM ArticleViews;");
                return (int)TableHelper.SelectScalar(query);
            }
        }
    }
}