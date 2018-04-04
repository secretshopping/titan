using System;
using Newtonsoft.Json;

public class FreeGeoIpProvider : IpGeolocationProviderBase
{
    protected override string ApiURL { get { return @"https://freegeoip.net"; } }

    protected string ResponseFormat { get { return "json"; } }

    protected string NotFoundResponse { get { return "404 page not found"; } }

    protected override string GetRequestUrl(string ip)
    {
        return String.Format("{0}/{1}/{2}", ApiURL, ResponseFormat, ip);
    }

    protected override IpGeolocationInfo TranslateResponse(string responseJson)
    {
        FreeGeoIpResponse responseObject = JsonConvert.DeserializeObject<FreeGeoIpResponse>(responseJson);

        IpGeolocationInfo info = new IpGeolocationInfo()
        {
            Ip = responseObject.IP,
            Latitude = responseObject.Latitude,
            Longitude = responseObject.Longitude
        };

        return info;
    }

    protected override bool FoundResponse(string response)
    {
        return !response.Contains(NotFoundResponse);
    }
}

public class FreeGeoIpResponse
{
    [JsonProperty("ip")]
    public string IP { get; set; }

    [JsonProperty("country_code")]
    public string CountryCode { get; set; }

    [JsonProperty("country_name")]
    public string CountryName { get; set; }

    [JsonProperty("region_code")]
    public string RegionCode { get; set; }

    [JsonProperty("region_name")]
    public string RegionName { get; set; }

    [JsonProperty("city")]
    public string City { get; set; }

    [JsonProperty("zip_code")]
    public string ZipCode { get; set; }

    [JsonProperty("time_zone")]
    public string TimeZone { get; set; }

    [JsonProperty("latitude")]
    public decimal Latitude { get; set; }

    [JsonProperty("longitude")]
    public decimal Longitude { get; set; }

    [JsonProperty("metro_code")]
    public int MetroCode { get; set; }
}