using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class UrlCreator
{
    /// <summary>
    /// Pares the URL and add 'http' if it doesn't have.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns></returns>
    public static string ParseUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
            return url;

        if (url.StartsWith("http:"))
            return url;

        if (url.StartsWith("https:"))
            return url;

        return "http://" + url;
    }

    public static string ExtractDomainUrl(string url)
    {
        url = url.Replace("https://", "");
        url = url.Replace("http://", "");
        url = url.Replace("www.", "");
        return url;
    }
}