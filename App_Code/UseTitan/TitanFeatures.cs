using Prem.PTC;
using System.Web;

public static class TitanFeatures
{
    public static bool PurchaseBalanceDisabled
    {
        get
        {
            return IsRofriqueWorkMines || IsJ5WalterOffersFromHome || IsDucenzoPerez || IsNightwolf || IsTrafficThunder;
        }
    }

    /// <summary>
    /// Include balance range in Cash Link Packs
    /// </summary>
    public static bool CashLinksBalanceRangeEnabled
    {
        get
        {
            return IsCopiousassets && AppSettings.RevShare.IsRevShareEnabled;
        }
    }

    /// <summary>
    /// Enable Transfer of Commission to Main Balance for each user instead of globally
    /// </summary>
    public static bool UserCommissionToMainBalanceEnabled
    {
        get
        {
            return IsCopiousassets;
        }
    }

    /// <summary>
    /// copiouassets.com
    /// </summary>
    public static bool IsCopiousassets
    {
        get
        {
            return (HttpContext.Current.User.Identity.Name == "copiousassets.com" || AppSettings.Site.Url == "http://copiousassets.com/");
        }
    }

    public static bool isSatvetErturkmen
    {
        get
        {
            if (AppSettings.Side == ScriptSide.Client &&
                HttpContext.Current.Application["CoHits"] != null && ((bool)HttpContext.Current.Application["CoHits"]))
                return true;

            if (AppSettings.Side == ScriptSide.AdminPanel && HttpContext.Current.User.Identity.Name == "www.cohits.com")
                return true;

            return false;
        }
    }

    public static bool isBoazor
    {
        get
        {
            return (AppSettings.Side == ScriptSide.AdminPanel && HttpContext.Current.User.Identity.Name == "www.vc2shop.com");
        }
    }

    public static bool isBoazorSite
    {
        get
        {
            return HttpContext.Current.Application["isBoazorSite"] != null;
        }
    }

    public static decimal? StringSawSundayPool
    {
        get
        {
            return HttpContext.Current.Application["StringSawSundayPool"] as int?;
        }
    }

    public static bool isAri
    {
        get
        {
            return (HttpContext.Current.Application["AriRevShareDistribution"] != null);
        }
    }

    public static bool IsRevolca
    {
        get
        {
            return HttpContext.Current.Application["RevolcaMatrix"] != null;
        }
    }

    public static bool IsSardaryify
    {
        get
        {
            return HttpContext.Current.Application["IsSardaryify"] != null;
        }
    }

    public static bool IsGambinos
    {
        get
        {
            return HttpContext.Current.Application["IsGambinos"] != null;
        }
    }

    public static bool IsRewardStacker
    {
        get
        {
            return HttpContext.Current.Application["IsRewardStacker"] != null;
        }
    }

    public static bool IsEpadilla
    {
        get
        {
            if (AppSettings.Side == ScriptSide.AdminPanel && HttpContext.Current.User.Identity.Name == "my1fashiondigital.com")
                return true;

            return HttpContext.Current.Application["IsEpadilla"] != null;
        }
    }

    public static bool IsTami9191
    {
        get
        {
            if (AppSettings.Side == ScriptSide.AdminPanel && HttpContext.Current.User.Identity.Name == "makeculous.com")
                return true;

            return HttpContext.Current.Application["IsTami9191"] != null;
        }
    }

    public static bool IsParras2k
    {
        get
        {
            return HttpContext.Current.Application["IsParras2k"] != null;
        }
    }

    public static bool IsAdzbuzz
    {
        get
        {
            return HttpContext.Current.Application["IsAdzbuzz"] != null;
        }
    }

    public static bool IsBobbyDonev
    {
        get
        {
            if (AppSettings.Side == ScriptSide.AdminPanel)
                return HttpContext.Current.User.Identity.Name == "moneybuzz.eu";
            else
                return HttpContext.Current.Application["IsBobbyDonev"] != null;
        }
    }

    public static bool IsAhmed
    {
        get
        {
            if (AppSettings.Side == ScriptSide.AdminPanel)
                return HttpContext.Current.User.Identity.Name == "www.qoinad.com";
            else
                return HttpContext.Current.Application["IsAhmed"] != null;
        }
    }

    public static bool IsRofrique
    {
        get
        {
            return HttpContext.Current.Application["IsRofrique"] != null;
        }
    }

    public static bool IsRofriqueWorkMines
    {
        get
        {
            if (AppSettings.Side == ScriptSide.AdminPanel)
                return HttpContext.Current.User.Identity.Name == "workmines.com";
            else
                return HttpContext.Current.Application["IsRofrique"] != null;
        }
    }

    public static bool IsClickmyad
    {
        get
        {
            if(AppSettings.Side == ScriptSide.AdminPanel)
                return HttpContext.Current.User.Identity.Name == "clickmyad.net";

            return HttpContext.Current.Application["IsClickmyad"] != null;
        }
    }

    public static bool IsTradeOwnSystem
    {
        get
        {
            if (AppSettings.Side == ScriptSide.AdminPanel)
                return HttpContext.Current.User.Identity.Name == "cashbackadverts.com";
            else
                return HttpContext.Current.Application["IsTradeOwnSystem"] != null;
        }
    }

    public static bool IsFlotrading
    {
        get
        {
            return HttpContext.Current.Application["IsFlotrading"] != null;
        }
    }

    public static bool IsRetireYoung
    {
        get
        {
            if (AppSettings.Side == ScriptSide.AdminPanel)
                return HttpContext.Current.User.Identity.Name == "retireyoung.co";

            return HttpContext.Current.Application["IsRetireYoung"] != null;
        }
    }

    public static bool IsJ5WalterFreebiesFromHome
    {
        get
        {
            if (AppSettings.Side == ScriptSide.AdminPanel)
                return HttpContext.Current.User.Identity.Name == "freebiesfromhome.com" || HttpContext.Current.User.Identity.Name == "adultfreebiesfromhome.com";

            return HttpContext.Current.Application["IsJ5WalterFreebiesFromHome"] != null;
        }
    }

    public static bool IsJ5WalterOffersFromHome
    {
        get
        {
            return HttpContext.Current.Application["IsJ5WalterOffersFromHome"] != null;
        }
    }

    public static bool IsJ5Walter
    {
        get
        {
            if (AppSettings.Side == ScriptSide.AdminPanel)
                return  HttpContext.Current.User.Identity.Name == "freebiesfromhome.com" || 
                        HttpContext.Current.User.Identity.Name == "adultfreebiesfromhome.com" || 
                        HttpContext.Current.User.Identity.Name == "offersfromhome.com";

            return HttpContext.Current.Application["IsJ5Walter"] != null;
        }
    }

    public static bool IsNightwolf
    {
        get
        {
            if (AppSettings.Side == ScriptSide.AdminPanel)
                return HttpContext.Current.User.Identity.Name == "kingofjackpot.com";
            else
                return HttpContext.Current.Application["IsNightwolf"] != null;
        }
    }

    public static bool IsDucenzoPerez
    {
        get
        {
            if (AppSettings.Side == ScriptSide.AdminPanel)
                return HttpContext.Current.User.Identity.Name == "mycryptogrind.com";
            else
                return HttpContext.Current.Application["IsDucenzoPerez"] != null;
        }
    }

    public static bool IsTrafficThunder
    {
        get
        {
            if (AppSettings.Side == ScriptSide.AdminPanel)
                return HttpContext.Current.User.Identity.Name == "network.project-laya.com" || 
                    HttpContext.Current.User.Identity.Name == "testnetwork.project-laya.com";
            else
                return HttpContext.Current.Application["IsTrafficThunder"] != null;
        }
    }

    public static bool IsMarkWheelz85
    {
        get
        {
            if (AppSettings.Side == ScriptSide.AdminPanel)
                return HttpContext.Current.User.Identity.Name == "millionairefox.com" ||
                    HttpContext.Current.User.Identity.Name == "test.millionairefox.com";
            else
                return HttpContext.Current.Application["IsMarkWheelz85"] != null;
        }
    }
    
    public static bool IsMuhdnasir
    {
        get
        {
            if (AppSettings.Side == ScriptSide.AdminPanel)
                return HttpContext.Current.User.Identity.Name == "www.lateepay.com";
            else
                return HttpContext.Current.Application["IsMuhdnasir"] != null;
        }
    }
}