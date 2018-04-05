using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Titan;

public enum BypassSecurityCheck
{
    Null = 0,

    No = 1,
    Yes = 5
}

public class BypassSecurityCheckHelper
{
    public static ListItem[] ListItems
    {
        get
        {

            var query = from BypassSecurityCheck status in Enum.GetValues(typeof(BypassSecurityCheck))
                        where status != BypassSecurityCheck.Null
                        orderby (int)status
                        select new ListItem(status.ToString(), (int)status + "");

            return query.ToArray();
        }
    }
}