using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC.Members;
using Prem.PTC.Utils.NVP;
using Prem.PTC.Utils;
using System.Text;
using Prem.PTC.Offers;

/// <summary>
/// Handles geolocation profiling 
/// </summary>
public class GeolocationUnit
{
    public string CountryCodes { get; set; }
    public string Cities { get; set; }
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
    public Gender Gender { get; set; }
    public NotNullNameValuePairs Profile { get; set; }

    public GeolocationUnit()
    {
        Profile = new NotNullNameValuePairs();
        MinAge = 0; MaxAge = 0; Gender = Gender.Null;
    }

    public GeolocationUnit(string cc) : this()
    {
        CountryCodes = cc;
    }

    public GeolocationUnit(string cc, string cities, int minAge, int maxAge, Gender gender)
        : this()
    {
        CountryCodes = cc; Cities = cities; MinAge = minAge; MaxAge = maxAge; Gender = gender;
    }

    /// <summary>
    /// Returns valid GeolocationUnit object from countries string (delimited with delimiter)
    /// </summary>
    /// <param name="input">country1#country2 etc.</param>
    /// <returns></returns>
    public static GeolocationUnit ParseFromCountries(string input, char delimiter = '#')
    {
        return new GeolocationUnit(ParseFromCountriesString(input, delimiter));
    }

    /// <summary>
    /// Returns valid string object from countries string (delimited with delimiter)
    /// </summary>
    /// <param name="input">country1#country2 etc.</param>
    /// <returns></returns>
    public static string ParseFromCountriesString(string input, char delimiter = '#')
    {
        var countries = input.Split(delimiter);
        StringBuilder sb = new StringBuilder();

        bool isFirst = true;

        foreach (var country in countries)
        {
            string cc = CountryManager.GetCountryCode(country);
            if (!String.IsNullOrEmpty(cc))
            {
                //We found match
                if (!isFirst)
                    sb.Append(GeolocationBase.Delimiter);

                sb.Append(cc);
                isFirst = false;
            }
        }

        return sb.ToString();
    }

}