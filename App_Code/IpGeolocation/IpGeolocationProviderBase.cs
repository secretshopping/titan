using System;
using System.Net;
using Prem.PTC;

public abstract class IpGeolocationProviderBase
{
    protected abstract string ApiURL { get; }

    public IpGeolocationInfo GetGeoLocationInfo(string ip)
    {
        IpGeolocationInfo response = null;

        try
        {
            string apiRequestUrl = GetRequestUrl(ip);

            using (WebClient client = new WebClient())
            {
                string json = client.DownloadJsonString(apiRequestUrl);
                if (FoundResponse(json))
                    response = TranslateResponse(json);
            }
        }
        catch(Exception ex)
        {
            ErrorLogger.Log(ex);
        }

        return response;
    }
    
    protected abstract string GetRequestUrl(string ip);
    protected abstract IpGeolocationInfo TranslateResponse(string responseJson);
    protected abstract bool FoundResponse(string response);
}