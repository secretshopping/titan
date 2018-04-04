using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Web;
using Prem.PTC;

public class UseTitan
{
    /// <summary>
    /// Gets the list of IP addresses used by Official UseTitan Admin Panel
    /// located on UseTitan server: admin.usetitan.com
    /// Can be helpful with verifying handler requests
    /// </summary>
    public static List<string> AdminPanelIPAddresses
    {
        get
        {
            var cache = new UseTitanIPCache();
            return (List<string>)cache.Get();
        }
    }

    public static string AdminPanelIPAddressesString
    {
        get
        {
            string temp = "";
            foreach (var elem in AdminPanelIPAddresses)
            {
                temp += elem;
                temp += ";";
            }
            return temp.Substring(0, temp.Length - 2);
        }
    }

    public static string AdminPanelIP
    {
        get
        {
            if (AppSettings.Side == ScriptSide.Client)
                return AdminPanelIPAddresses[0];
            else
                return System.Web.HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"];
        }
    }
}