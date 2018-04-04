using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ShoutboxMessageRestrictions
/// </summary>
static class ShoutboxMessageRestrictions
{
    public static bool AllowMessage(string message)
    {
        if (message.ToLower().Contains("http:/") || message.ToLower().Contains("https:/") || message.ToLower().Contains("www."))
            return false;
        else
            return true;
    }
}