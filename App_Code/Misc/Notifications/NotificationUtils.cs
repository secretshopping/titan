using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

/// <summary>
/// Summary description for NotificationUtils
/// </summary>
public static class NotificationUtils
{
    public static void DisplayCenteredNotification(string title, string message)
    {
        string script = "<script type=\"text/javascript\">$.laConfig ({classes: { box: 'lite-alert-boxcenter', item: 'lite-alert-itemcenter', close: 'lite-alert-item-close', "
            + "header: 'lite-alert-item-headerbig', content: 'lite-alert-item-content',  footer: 'lite-alert-item-footer'} });$.la('"
            + title + "', '" + message
            + "');</script>";
        ScriptManager.RegisterClientScriptBlock((HttpContext.Current.Handler as Page), typeof(string), Guid.NewGuid().ToString(), script, false);
    }
}