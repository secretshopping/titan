using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Titan
{
    public enum WhatIsSent
    {
        Null = 0,

        Points = 1,
        Money = 2
    }

    public static class WhatIsSentHelper
    {
        /// <summary>
        /// Returns list control source with all statuses' short descriptions as text 
        /// and int status ordinal as value
        /// </summary>
        public static ListItem[] ListItems
        {
            get
            {
                var query = from WhatIsSent status in Enum.GetValues(typeof(WhatIsSent))
                            where status != WhatIsSent.Null
                            orderby (int)status
                            select new ListItem(status.ToString(), (int)status + "");

                return query.ToArray();
            }
        }
    }
}