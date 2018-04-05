using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

public class Device
{
    public static DeviceType Current
    {
        get
        {
            if (Convert.ToBoolean(HttpContext.Current.Request.Browser["IsMobile"]))
                return DeviceType.Mobile;

            if (Convert.ToBoolean(HttpContext.Current.Request.Browser["IsTablet"]))
                return DeviceType.Mobile;

            return DeviceType.Desktop;
        }
    }

    public static ListItem[] ListItems
    {
        get
        {
            var query = from DeviceType status in Enum.GetValues(typeof(DeviceType))
                        select new ListItem(status.ToString(), (int)status + "");

            return query.ToArray();
        }
    }
}