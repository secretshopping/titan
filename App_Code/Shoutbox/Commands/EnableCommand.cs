using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan;

/// <summary>
/// Summary description for ShoutboxMessageRestrictions
/// </summary>
public static class EnableCommand
{
    public static void TryEnableCommands()
    {
        HttpContext.Current.Response.Cookies[ShoutboxCommands.cookieName].Value = "true";
        HttpContext.Current.Response.Cookies[ShoutboxCommands.cookieName].Expires = DateTime.Now.AddYears(5);

        throw new SuccessMsgException(U4200.COMMANDSENABLED);
    }
}