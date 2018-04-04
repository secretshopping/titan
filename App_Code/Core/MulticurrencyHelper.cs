using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using System.Globalization;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Net;
using System.Web.Caching;

public class MulticurrencyHelper
{
    #region Operations

    public static void SetCurrency(string regionName)
    {
        HttpCookie cookie = new HttpCookie("CurrencyInfo");
        cookie.Value = regionName;
        HttpContext.Current.Response.Cookies.Add(cookie);
    }

    public static string GetCurrency()
    {
        if (!ShowMulticurrency)
            return AppSettings.Site.CurrencyCode;

        return GetRegionInfo().ISOCurrencySymbol;
    }

    public static string GetCurrencySign()
    {
        if (!ShowMulticurrency)
            return AppSettings.Site.CurrencySign;

        return GetRegionInfo().CurrencySymbol;
    }

    public static bool ShowMulticurrency
    {
        get
        {
            if (!AppSettings.Site.IsMulticurrencyPricingEnabled || AppSettings.Side == ScriptSide.AdminPanel)
                return false;

            return true;
        }
    }

    #endregion

    public static RegionInfo GetRegionInfo()
    {
        HttpCookie cookie = HttpContext.Current.Request.Cookies["CurrencyInfo"];
        RegionInfo regionInfo = null;

        if (cookie == null)
        {
            string CountryCode = CountryManager.LookupCountryCode(IP.Current).ToUpper();

            try
            {
                regionInfo = new RegionInfo(CountryCode);
            }
            catch (Exception ex)
            {
                regionInfo = new RegionInfo("US");
            }
        }
        else
        {
            regionInfo = new RegionInfo(cookie.Value);
        }

        return regionInfo;
    }

    private static IEnumerable<RegionInfo> GetAllRegionInfos()
    {
        return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Select(x => new RegionInfo(x.Name))
            .Distinct();
    }

    public static List<string> AllCurrenciesList
    {
        get
        {
            var regions = GetAllRegionInfos();

            return regions
                .GroupBy(x => x.ISOCurrencySymbol)
                .Select(x => x.First())
                .OrderBy(x => x.ISOCurrencySymbol)
                .Select(x => x.Name)
                .ToList();
        }
    }

    public static ListItem[] AllCurrenciesListItem
    {
        get
        {
            var regions = GetAllRegionInfos();

            return regions
                .GroupBy(x => x.ISOCurrencySymbol)
                .Select(x => x.First())
                .OrderBy(x => x.ISOCurrencySymbol)
                .Select(x => new ListItem(String.Format("{0} ({1})", x.ISOCurrencySymbol, x.CurrencySymbol), x.Name))
                .ToArray();
        }
    }
}