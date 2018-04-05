using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;


public enum MultiCurrencyHelperProviders
{
    Null = 0,
    FixerIO = 1,
    OpenExchangeRates = 2
}


public class MultiCurrencyHelperManager
{
    //public MultiCurrencyHelperManager()
    //{
    //}

    public static ListItem[] ListItems
    {
        get
        {
            var query = from MultiCurrencyHelperProviders status in Enum.GetValues(typeof(MultiCurrencyHelperProviders))
                        where status != MultiCurrencyHelperProviders.Null
                        orderby (int)status
                        select new ListItem(status.ToString(), (int)status + "");

            return query.ToArray();
        }
    }
}