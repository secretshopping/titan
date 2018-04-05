using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;


public enum MultiCurrencyProvider
{
    Null = 0,
    FixerIO = 1,
    OpenExchangeRates = 2
}


public class MultiCurrencyHelper
{

    public static ListItem[] ListItems
    {
        get
        {
            var query = from MultiCurrencyProvider status in Enum.GetValues(typeof(MultiCurrencyProvider))
                        where status != MultiCurrencyProvider.Null
                        orderby (int)status
                        select new ListItem(status.ToString(), (int)status + "");

            return query.ToArray();
        }
    }


}