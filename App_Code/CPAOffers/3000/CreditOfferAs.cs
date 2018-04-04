using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

public enum CreditOfferAs
{
    Null = 0,

    Cash = 1,
    NonCash = 2,

    NetworkDefault = 3 //Will take default value from the network
}

public class CreditOfferAsHelper
{
    /// <summary>
    /// Returns list control source with all statuses' short descriptions as text 
    /// and int status ordinal as value
    /// </summary>
    public static ListItem[] ListItems
    {
        get
        {
            var query = from CreditOfferAs status in Enum.GetValues(typeof(CreditOfferAs))
                        where (status != CreditOfferAs.Null)
                        orderby (int)status
                        select new ListItem(status.ToString(), (int)status + "");

            return query.ToArray();
        }
    }
}