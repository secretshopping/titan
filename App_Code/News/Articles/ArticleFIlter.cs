using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.News
{
    public class ArticleFilter
    {
        public string CountryCode { get; set; }
        public int CategoryId { get; set; }

        public string SearchText { get; set; }
        public int FirstResults { get; set; }
        public int LastDays { get; set; }
        public int SkipFirst { get; set; }

        public ArticleFilter(string countryCode)
        {
            CountryCode = countryCode;
            CategoryId = -1;
            SearchText = String.Empty;
            FirstResults = 30;
            LastDays = 10;
        }

        public ArticleFilter(string countryCode, int categoryId)
        {
            CountryCode = countryCode;
            CategoryId = categoryId;
            SearchText = String.Empty;
            FirstResults = 30;
            LastDays = 10;
        }
    }
}