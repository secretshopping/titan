/// <summary>
/// Retrieves CC and Country from given IP address
/// </summary>
public class CountryInformation
{
    /// <summary>
    /// ISO Standard Country Code (CC)
    /// </summary>
    public string CountryCode { get; set; }
    public string CountryName { get; set; }

    public CountryInformation(string ip)
    {
        CountryCode = CountryManager.LookupCountryCode(ip);
        CountryName = CountryManager.GetCountryName(CountryCode);
    }

    public CountryInformation() : this(IP.Current) { }
}