using Prem.PTC;
using Prem.PTC.Advertising;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;


public class PTCSercurityManager
{
    private readonly static string SessionName = "TIT_PTCSec";

    public static void Lock(int seconds)
    {
        HttpContext.Current.Session[SessionName] = DateTime.Now.AddSeconds(seconds);
    }

    public static void Release()
    {
        HttpContext.Current.Session[SessionName] = DateTime.Now.AddDays(-1);
    }

    public static bool IsLocked
    {
        get
        {
            if (HttpContext.Current.Session[SessionName] == null)
                return false;

            DateTime LockedUntil = (DateTime)HttpContext.Current.Session[SessionName];

            if (LockedUntil > DateTime.Now)
                return true;

            return false;
        }

    }
    public static string HashAd(string watchedId)
    {
        string stringToHash = watchedId + AppSettings.Offerwalls.UniversalHandlerPassword + DateTime.Now.ToShortDateString();

        return HashingManager.GenerateSHA256(stringToHash);
    }
    public static bool IsTrafficAdvertFound(string adHash)
    {
        List<string> trafficAdsHashes = HashAllTrafficAdsIds();
        foreach (string trafficAd in trafficAdsHashes)
        {
            if (CompareHashes(trafficAd, adHash))
                return true;
        }
        return false;
    }
    public static bool IsPTCAdvertFound(string adHash, List<PtcAdvert> listOfAds)
    {
        List<string> trafficAdsHashes = HashAllPTCAdsIds(listOfAds);
        foreach (string trafficAd in trafficAdsHashes)
        {
            if (CompareHashes(trafficAd, adHash))
                return true;
        }
        return false;
    }

    private static List<string> HashAllTrafficAdsIds()
    {
        var listOfAds = TrafficGridAdvert.GetAllActiveAds();
        var listOfAdsIds = new List<int>();
        var listOfAdsHashes = new List<string>();
        foreach (TrafficGridAdvert item in listOfAds)
        {
            listOfAdsIds.Add(item.Id);
        }
        foreach (int id in listOfAdsIds)
        {
            string stringToHash = id + AppSettings.Offerwalls.UniversalHandlerPassword + DateTime.Now.ToShortDateString();
            listOfAdsHashes.Add(HashingManager.GenerateSHA256(stringToHash));
        }
        return listOfAdsHashes;
    }
    private static List<string> HashAllPTCAdsIds(List<PtcAdvert> listOfAds)
    {
        var listOfAdsIds = new List<int>();
        var listOfAdsHashes = new List<string>();
        foreach (PtcAdvert item in listOfAds)
        {
            listOfAdsIds.Add(item.Id);
        }
        foreach (int id in listOfAdsIds)
        {
            string stringToHash = id + AppSettings.Offerwalls.UniversalHandlerPassword + DateTime.Now.ToShortDateString();
            listOfAdsHashes.Add(HashingManager.GenerateSHA256(stringToHash));
        }
        return listOfAdsHashes;
    }

    private static bool CompareHashes(string hash1, string hash2)
    {
        if (!String.IsNullOrWhiteSpace(hash1) && !String.IsNullOrWhiteSpace(hash2))
        {
            if (hash1 == hash2)
                return true;
            return false;
        }
        return false;
    }
}

