using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

public enum UniversalStatus
{
    Null = 0,

    Active = 1,
    Paused = 2,
    Deleted = 3
}

public static class UniversalStatusHelper
{
    /// <summary>
    /// Returns list control source with all statuses' short descriptions as text 
    /// and int status ordinal as value
    /// </summary>
    public static ListItem[] ListItems
    {
        get
        {
            var query = from UniversalStatus status in Enum.GetValues(typeof(UniversalStatus))
                        where status != UniversalStatus.Null
                        orderby (int)status
                        select new ListItem(status.ToString(), (int)status + "");

            return query.ToArray();
        }
    }

    public static Dictionary<UniversalStatus, System.Drawing.Color> ColorDictionary = new Dictionary<UniversalStatus, System.Drawing.Color>
    {
        { UniversalStatus.Active, System.Drawing.Color.Green },
        { UniversalStatus.Paused, System.Drawing.Color.Orange },
        { UniversalStatus.Deleted, System.Drawing.Color.Red }
    };
}