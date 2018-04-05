using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for RegexExpressions
/// </summary>
public class RegexExpressions
{
    public static string AdWebsiteUrl
    {
        get
        {
            return @"^(https?|ftp)://[^\s/$.?#].[^\s]*$";
        }
    }

    public static string GetWatchedAdsSQLText(int minNumberOfAds)
    {
        string baseText = "%#%";
        string addText = "#%";

        if (minNumberOfAds < 1)
            throw new InvalidOperationException("MinNumberOfAds must be >= 1");

        for (int i = 1; i < minNumberOfAds; i++)
            baseText += addText;

        return baseText;
    }

}