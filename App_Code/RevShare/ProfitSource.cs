using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Titan.InternalExchange;

public static class ProfitSourcesHelper
{

    public static List<ProfitSource> GetActiveProfitSources()
    {
        List<ProfitSource> activeSources = new List<ProfitSource>();

        activeSources.Add(ProfitSource.AdPacks);
        activeSources.Add(ProfitSource.Banners);
        activeSources.Add(ProfitSource.TransferFees);
        activeSources.Add(ProfitSource.LoginAds);
        activeSources.Add(ProfitSource.Jackpots);
        activeSources.Add(ProfitSource.StartPage);
        activeSources.Add(ProfitSource.SurfAds);
        activeSources.Add(ProfitSource.Memberships);
        activeSources.Add(ProfitSource.RevenueAdminCommissions);
        activeSources.Add(ProfitSource.TrafficGrid);
        activeSources.Add(ProfitSource.AccountActivationFee);
        activeSources.Add(ProfitSource.JackpotPvp);
        //if (InternalExchangeManager.GetBalanceCode(false) == AppSettings.Site.CurrencyCode)
        //    activeSources.Add(ProfitSource.InternalExchangeFee);

        return activeSources;
    }

    public static bool IsActive(ProfitSource source)
    {
        return GetActiveProfitSources().Contains(source);
    }

    public static ListItem[] ListItems
    {
        get
        {
            var query = from ProfitSource source in Enum.GetValues(typeof(ProfitSource))
                        orderby (int)source
                        select new ListItem(GetDictProfitSources[source], (int)source + "", ProfitSourcesHelper.IsActive(source));

            return query.ToArray();
        }
    }

    public static ListItem[] ActiveListItems
    {
        get
        {
            var query = from ProfitSource source in Enum.GetValues(typeof(ProfitSource))
                        where ProfitSourcesHelper.IsActive(source)
                        orderby (int)source
                        select new ListItem(GetDictProfitSources[source], (int)source + "");

            return query.ToArray();
        }
    }

    public static Dictionary<ProfitSource, string> GetDictProfitSources
    {
        get
        {
            Dictionary<ProfitSource, string> sources = new Dictionary<ProfitSource, string>();

            sources.Add(ProfitSource.AdPacks, "AdPacks");
            sources.Add(ProfitSource.Banners, "Banners");
            sources.Add(ProfitSource.TransferFees, "Transfer Fees");
            sources.Add(ProfitSource.LoginAds, "Login Ads");
            sources.Add(ProfitSource.CPAGPT, "CPA/GPT");
            sources.Add(ProfitSource.Offerwalls, "Offer Walls");
            sources.Add(ProfitSource.FBLikes, "Facebook Likes");
            sources.Add(ProfitSource.TrafficGrid, "Traffic Grid");
            sources.Add(ProfitSource.Crowdflower, "Crowdflower");
            sources.Add(ProfitSource.CashLinks, "Cash Links");
            sources.Add(ProfitSource.Jackpots, "Jackpots");
            sources.Add(ProfitSource.StartPage, "Start Page");
            sources.Add(ProfitSource.SurfAds, "Surf Ads");
            sources.Add(ProfitSource.Memberships, "Memberships");
            sources.Add(ProfitSource.RevenueAdminCommissions, "Revenue Admin Commisions");
            sources.Add(ProfitSource.AccountActivationFee, "Account Activation Fee");
            sources.Add(ProfitSource.JackpotPvp, "Pvp Jackpot");
            sources.Add(ProfitSource.InternalExchangeFee, "Internal Exchange Fee");
            
            return sources;
        }
    }
}


public enum ProfitSource
{
    //Add new ProfitSource to GetDictProfitSources property!
    AdPacks = 1,
    Banners = 2,
    LoginAds = 3,
    TransferFees = 4,
    CPAGPT = 5,
    //Ads = 6, //not present in RevShare
    Offerwalls = 7,
    FBLikes = 8,
    TrafficGrid = 9,
    Crowdflower = 11,
    CashLinks = 12,
    Jackpots = 13,
    StartPage = 14,
    SurfAds = 15,
    Memberships = 16,
    RevenueAdminCommissions = 17,
    AccountActivationFee = 18,
    JackpotPvp = 19,
    InternalExchangeFee = 20
}

