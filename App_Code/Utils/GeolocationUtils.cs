using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Collections.Specialized;
using System.Linq;
using System.Data;
using System.Resources;
using System.Web;
using Prem.PTC.Members;
using System.Globalization;

namespace Prem.PTC.Utils
{
    public static class GeolocationUtils
    {
        public static SortedDictionary<string, int> CountriesDictionary
        {
            get
            {
                //if (AppSettings.Side == ScriptSide.AdminPanel)
                //{
                //    // No members count for Admin Panel
                //    ResourceSet resourceSet = Resources.Countries.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
                //    SortedDictionary<string, int> data = new SortedDictionary<string, int>();

                //    foreach (DictionaryEntry entry in resourceSet)
                //    {
                //        data.Add(entry.Value.ToString(), 0);
                //    }

                //    return data;
                //}
                //else
                //{
                    var cache = new MembersCountryListCache();
                    return (SortedDictionary<string, int>)cache.Get();
                //}            
            }
        }

        public static SortedDictionary<string, int> GeoCountData
        {
            get 
            {
                return CountriesDictionary;
            }
        }   

        public static SortedDictionary<string, string> GetCountriesData()
        {
            SortedDictionary<string, string> dic = new SortedDictionary<string, string>();

            foreach (var item in CountriesDictionary)
            {
                if (AppSettings.GlobalAdvertsSettings.CountOfMembersFromCountryInGeoLocation)
                    dic.Add(item.Key, item.Key + " (" + item.Value + ")");
                else
                    dic.Add(item.Key, item.Key);
            }

            return dic;
        }

        public static Tuple<string, string> GetCountryData(string countryName)
        {
            if (AppSettings.GlobalAdvertsSettings.CountOfMembersFromCountryInGeoLocation)
                return new Tuple<string, string>(countryName, string.Format("{0} ({1})", countryName, CountriesDictionary[countryName]));            
            else
                return new Tuple<string, string>(countryName, countryName);
        }
    }
}