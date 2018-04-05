using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

public static class JsonRequestExtension
{
    public static string DownloadJsonString(this WebClient client, string url)
    {
        string responseText;
        try
        {
            WebRequestUtils.SetUpSecurityProtocols();
            responseText = client.DownloadString(url);
        }
        catch (WebException exception)
        {
            using (var reader = new StreamReader(exception.Response.GetResponseStream()))
            {
                responseText = reader.ReadToEnd();
            }
        }
        return responseText;
    }

    public static string UploadJsonData(this WebClient client, string url, string data)
    {
        string responseText;
        try
        {
            WebRequestUtils.SetUpSecurityProtocols();
            responseText = client.UploadString(url, data);
        }
        catch (WebException exception)
        {
            using (var reader = new StreamReader(exception.Response.GetResponseStream()))
            {
                responseText = reader.ReadToEnd();
            }
        }
        return responseText;
    }

}