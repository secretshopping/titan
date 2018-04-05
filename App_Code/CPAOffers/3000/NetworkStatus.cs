using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

namespace Titan
{
    public enum NetworkStatus
    {
        Null = 0,

        Active = 1,
        Paused = 2,
        Deleted = 3
    }

    public static class NetworkStatusHelper
    {
        /// <summary>
        /// Returns list control source with all statuses' short descriptions as text 
        /// and int status ordinal as value
        /// </summary>
        public static ListItem[] ListItems
        {
            get
            {
                var query = from NetworkStatus status in Enum.GetValues(typeof(NetworkStatus))
                            where status != NetworkStatus.Null
                            orderby (int)status
                            select new ListItem(status.ToString(), (int)status + "");

                return query.ToArray();
            }
        }
    }
}