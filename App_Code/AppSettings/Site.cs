using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Windows.Forms;
using Titan.Cryptocurrencies;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class Site
        {
            /// <exception cref="ArgumentNullException" />
            public static string Name
            {
                get { return appSettings.SiteName; }
                set
                {
                    if (value == null) throw new ArgumentNullException("SiteName");

                    appSettings.SiteName = value;

                }
            }

            /// <summary>
            /// Returns full website URL with "http://" and "/". For example:
            /// http://example.com/
            /// </summary>
            public static string Url
            {
                get { return appSettings.SiteUrl; }
                set
                {
                    Uri uri = new Uri(value); // uri validation

                    appSettings.SiteUrl = value;

                }
            }

            public static string Slogan
            {
                get { return appSettings.SiteSlogan; }
                set
                {
                    if (value == null) throw new ArgumentNullException("Slogan");

                    appSettings.SiteSlogan = value;

                }
            }

            public static bool TrafficExchangeMod
            {
                get { return Mode == WebsiteMode.TrafficExchange; }
                set
                {
                    if (value == true)
                        Mode = WebsiteMode.TrafficExchange;
                }
            }

            public static bool AllowOnlyOneRegisteredIP
            {
                get { return appSettings.AllowOnlyOneRegisteredIP; }
                set
                {
                    appSettings.AllowOnlyOneRegisteredIP = value;
                }
            }

            public static bool PureGPTMode
            {
                get { return Mode == WebsiteMode.GPT; }
                set
                {
                    if (value == true)
                        Mode = WebsiteMode.GPT;
                }
            }

            public static WebsiteMode Mode
            {
                get { return (WebsiteMode)appSettings.websiteMode; }
                set
                {
                    appSettings.websiteMode = (int)value;
                }
            }
            public static bool DeveloperModeEnabled
            {
                get { return appSettings.DeveloperModeEnabled; }
                set
                {
                    appSettings.DeveloperModeEnabled = value;

                }
            }

            public static string Description
            {
                get { return appSettings.SiteDescription; }
                set
                {
                    appSettings.SiteDescription = value;

                }
            }

            public static bool ShowSiteName
            {
                get { return appSettings.ShowSiteName; }
                set
                {
                    appSettings.ShowSiteName = value;

                }
            }

            public static bool ForumEnabled
            {
                get { return appSettings.ForumEnabled; }
                set
                {
                    appSettings.ForumEnabled = value;

                }
            }

            public static bool LatestNewsEnabled
            {
                get { return appSettings.LatestNewsEnabled; }
                set
                {
                    appSettings.LatestNewsEnabled = value;

                }
            }

            public static string CurrencySign
            {
                get { return appSettings.CurrencySign; }
                set
                {
                    appSettings.CurrencySign = value;
                }
            }

            public static string MulticurrencySign
            {
                get
                {
                    if (!MulticurrencyHelper.ShowMulticurrency)
                        return appSettings.CurrencySign;

                    return MulticurrencyHelper.GetCurrencySign();
                }
            }

            public static string CurrencyCode
            {
                get { return appSettings.CurrencyCode; }
                set
                {
                    appSettings.CurrencyCode = value;

                }
            }

            public static bool CurrencyIsTokenCryptocurrency
            {
                get
                {
                    return (AppSettings.Payments.CurrencyMode == CurrencyMode.Cryptocurrency &&
                        CryptocurrencyFactory.GetCryptocurrencyType(AppSettings.Site.CurrencyCode) == CryptocurrencyType.ERC20Token);
                }
            }

            public static bool IsEUCookiePolicyEnabled
            {
                get { return appSettings.IsEUCookieEnabled; }
                set
                {
                    appSettings.IsEUCookieEnabled = value;

                }
            }

            public static bool AutoApprovalCampaignsEnabled
            {
                get { return appSettings.IsAutoApprovalEnabled; }
                set
                {
                    appSettings.IsAutoApprovalEnabled = value;

                }
            }

            public static bool AutoActiveCampaignsEnabled
            {
                get { return appSettings.IsAutoAactiveEnabled; }
                set
                {
                    appSettings.IsAutoAactiveEnabled = value;

                }
            }

            public static bool IsCurrencySignBefore
            {
                get { return appSettings.IsCurrencySignBefore; }
                set
                {
                    appSettings.IsCurrencySignBefore = value;

                }
            }

            public static AdBlockPolicy AdBlockPolicy
            {
                get { return (AdBlockPolicy)appSettings.AdBlockPolicy; }
                set
                {
                    appSettings.AdBlockPolicy = (int)value;

                }
            }

            public static string MainColor
            {
                get { return appSettings.MainColor; }
                set
                {
                    appSettings.MainColor = value;

                }
            }

            public static string LightColor
            {
                get { return appSettings.LightColor; }
                set
                {
                    appSettings.LightColor = value;

                }
            }

            public static string DarkColor
            {
                get { return appSettings.DarkColor; }
                set
                {
                    appSettings.DarkColor = value;

                }
            }

            public static string Theme
            {
                get
                {
                    if (AppSettings.IsDemo && UseTitanDemoHelper.IsThemeSet()) // For script demo purposes
                        return UseTitanDemoHelper.GetTheme();

                    if (string.IsNullOrWhiteSpace(appSettings.Theme)) // Default theme
                        return "titan";

                    return appSettings.Theme;
                }
                set
                {
                    appSettings.Theme = value;
                }
            }

            public static string LogoImageURL
            {
                get
                {
                    if (String.IsNullOrEmpty(appSettings.LogoImageURL))
                    {
                        if (AppSettings.Side == ScriptSide.Client)
                            return Resources.DEFAULT.LOGOURL;
                        else
                            return AppSettings.Site.Url + Resources.DEFAULT.LOGOURL;
                    }

                    return UrlUtils.ConvertTildePathToImgPath(appSettings.LogoImageURL);
                }
                set
                {
                    appSettings.LogoImageURL = value;

                }
            }

            public static string FaviconImageURL
            {
                get
                {
                    if (String.IsNullOrEmpty(appSettings.FaviconImageURL))
                    {
                        string faviconPath = "Images/Global/favicon.png";

                        if (AppSettings.Side == ScriptSide.Client)
                            return faviconPath;
                        else
                            return AppSettings.Site.Url + faviconPath;
                    }

                    return UrlUtils.ConvertTildePathToImgPath(appSettings.FaviconImageURL);
                }
                set
                {
                    appSettings.FaviconImageURL = value;

                }
            }

            public static TimeSpan TimeToNextCRONRun
            {
                get { return Misc.LastCRONRun.AddHours(24) - ServerTime; }
            }

            public static DateTime MaintenanceModeEndsDate
            {
                get { return appSettings.MaintenanceModeEndsDate; }
                set
                {
                    appSettings.MaintenanceModeEndsDate = value;

                }
            }

            public static bool URLCheckerAllowRedirect { get { return appSettings.URLCheckerAllowRedirect; } set { appSettings.URLCheckerAllowRedirect = value; } }

            public static bool ShowRemainingZeros { get { return appSettings.ShowRemainingZeros; } set { appSettings.ShowRemainingZeros = value; } }

            public static bool CommasInNumbersEnabled { get { return appSettings.CommasInNumbersEnabled; } set { appSettings.CommasInNumbersEnabled = value; } }

            public static bool FooterRemoved
            {
                get { return appSettings.FooterRemoved; }
                set { appSettings.FooterRemoved = value; }
            }

            public static bool BannersAddByUrlEnabled
            {
                get { return appSettings.BannersAddByUrlEnabled; }
                set { appSettings.BannersAddByUrlEnabled = value; }
            }

            public static bool ShowQuickStartGuideEnabled
            {
                get { return appSettings.ShowQuickStartGuideEnabled; }
                set { appSettings.ShowQuickStartGuideEnabled = value; }
            }

            public static string CustomFooterContent
            {
                get { return appSettings.CustomFooterContent; }
                set { appSettings.CustomFooterContent = value; }
            }

            public static string CustomHeaderContent
            {
                get { return appSettings.CustomHeaderContent; }
                set { appSettings.CustomHeaderContent = value; }
            }

            public static bool IsMulticurrencyPricingEnabled
            {
                get { return appSettings.IsMulticurrencyPricingEnabled; }
                set { appSettings.IsMulticurrencyPricingEnabled = value; }
            }

            #region SocialLinks
            public static string FacebookLink
            {
                get { return appSettings.FacebookLink; }
                set { appSettings.FacebookLink = value; }
            }

            public static string InstagramLink
            {
                get { return appSettings.InstagramLink; }
                set { appSettings.InstagramLink = value; }
            }

            public static string TwitterLink
            {
                get { return appSettings.TwitterLink; }
                set { appSettings.TwitterLink = value; }
            }

            public static string GooglePlusLink
            {
                get { return appSettings.GooglePlusLink; }
                set { appSettings.GooglePlusLink = value; }
            }

            public static string DribbbleLink
            {
                get { return appSettings.DribbbleLink; }
                set { appSettings.DribbbleLink = value; }
            }
            #endregion            

            public static bool EnableAfterLoginPopup
            {
                get { return appSettings.EnableAfterLoginPopup; }
                set { appSettings.EnableAfterLoginPopup = value; }
            }

            public static string SetAfterLoginPopupTitle
            {
                get { return appSettings.AfterLoginPopupTitle; }
                set { appSettings.AfterLoginPopupTitle = value; }
            }

            public static string SetAfterLoginPopupContent
            {
                get { return appSettings.AfterLoginPopupContent; }
                set { appSettings.AfterLoginPopupContent = value; }
            }

            public static string ChoosedLanguages
            {
                get { return appSettings.ChoosedLanguages; }
                set { appSettings.ChoosedLanguages = value; }
            }

            public static string DefaultLanguage
            {
                get { return appSettings.DefaultLanguage; }
                set { appSettings.DefaultLanguage = value; }
            }

            public static void Save()
            {
                appSettings.SaveSite();
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadSite();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {

            [Column("SiteName")]
            internal string SiteName { get { return _siteName; } set { _siteName = value; SetUpToDateAsFalse(); } }

            [Column("SiteUrl")]
            internal string SiteUrl { get { return _siteUrl; } set { _siteUrl = value; SetUpToDateAsFalse(); } }

            [Column("SiteSlogan")]
            internal string SiteSlogan { get { return _siteSlogan; } set { _siteSlogan = value; SetUpToDateAsFalse(); } }

            [Column("WebsiteMode")]
            internal int websiteMode { get { return _websiteMode; } set { _websiteMode = value; SetUpToDateAsFalse(); } }

            [Column("SiteDescription1")]
            internal string SiteDescription { get { return _SiteDescription; } set { _SiteDescription = value; SetUpToDateAsFalse(); } }

            [Column("ShowSiteName")]
            internal bool ShowSiteName { get { return _ShowSiteName; } set { _ShowSiteName = value; SetUpToDateAsFalse(); } }

            [Column("CurrencySign")]
            internal string CurrencySign { get { return _CurrencySign; } set { _CurrencySign = value; SetUpToDateAsFalse(); } }

            [Column("CurrencyCode")]
            internal string CurrencyCode { get { return _CurrencyCode; } set { _CurrencyCode = value; SetUpToDateAsFalse(); } }

            [Column("IsCurrencySignBefore")]
            internal bool IsCurrencySignBefore { get { return _IsCurrencySignBefore; } set { _IsCurrencySignBefore = value; SetUpToDateAsFalse(); } }

            [Column("IsEUCookieEnabled1")]
            internal bool IsEUCookieEnabled { get { return _IsEUCookieEnabled; } set { _IsEUCookieEnabled = value; SetUpToDateAsFalse(); } }

            [Column("AdBlockPolicy")]
            internal int AdBlockPolicy { get { return _AdBlockPolicy; } set { _AdBlockPolicy = value; SetUpToDateAsFalse(); } }

            [Column("MainColor")]
            internal string MainColor { get { return _MainColor; } set { _MainColor = value; SetUpToDateAsFalse(); } }

            [Column("LightColor")]
            internal string LightColor { get { return _LightColor; } set { _LightColor = value; SetUpToDateAsFalse(); } }

            [Column("DarkColor")]
            internal string DarkColor { get { return _DarkColor; } set { _DarkColor = value; SetUpToDateAsFalse(); } }

            [Column("LogoImageURL")]
            internal string LogoImageURL { get { return _LogoImageURL; } set { _LogoImageURL = value; SetUpToDateAsFalse(); } }

            [Column("FaviconImageURL")]
            internal string FaviconImageURL { get { return _FaviconImageURL; } set { _FaviconImageURL = value; SetUpToDateAsFalse(); } }

            [Column("AllowOnlyOneRegisteredIP")]
            internal bool AllowOnlyOneRegisteredIP { get { return _AllowOnlyOneRegisteredIP; } set { _AllowOnlyOneRegisteredIP = value; SetUpToDateAsFalse(); } }

            [Column("MaintenanceModeeEndsDate")]
            internal DateTime MaintenanceModeEndsDate { get { return _MaintenanceModeEndsDate; } set { _MaintenanceModeEndsDate = value; SetUpToDateAsFalse(); } }

            [Column("DeveloperModeEnabled")]
            internal bool DeveloperModeEnabled { get { return _DeveloperModeEnabled; } set { _DeveloperModeEnabled = value; SetUpToDateAsFalse(); } }

            [Column("ForumEnabled")]
            internal bool ForumEnabled { get { return _ForumEnabled; } set { _ForumEnabled = value; SetUpToDateAsFalse(); } }

            [Column("LatestNewsEnabled")]
            internal bool LatestNewsEnabled { get { return _LatestNewsEnabled; } set { _LatestNewsEnabled = value; SetUpToDateAsFalse(); } }

            [Column("URLCheckerAllowRedirect")]
            internal bool URLCheckerAllowRedirect { get { return _URLCheckerAllowRedirect; } set { _URLCheckerAllowRedirect = value; SetUpToDateAsFalse(); } }

            [Column("CommasInNumbersEnabled")]
            internal bool CommasInNumbersEnabled { get { return _CommasInNumbersEnabled; } set { _CommasInNumbersEnabled = value; SetUpToDateAsFalse(); } }

            [Column("ShowRemainingZeros")]
            internal bool ShowRemainingZeros { get { return _ShowRemainingZeros; } set { _ShowRemainingZeros = value; SetUpToDateAsFalse(); } }

            [Column("FooterRemoved")]
            internal bool FooterRemoved { get { return _FooterRemoved; } set { _FooterRemoved = value; SetUpToDateAsFalse(); } }

            [Column("IsAutoApprovalEnabled")]
            internal bool IsAutoApprovalEnabled { get { return _IsAutoApprovalEnabled; } set { _IsAutoApprovalEnabled = value; SetUpToDateAsFalse(); } }

            [Column("IsAutoAactiveEnabled")]
            internal bool IsAutoAactiveEnabled { get { return _IsAutoAactiveEnabled; } set { _IsAutoAactiveEnabled = value; SetUpToDateAsFalse(); } }

            [Column("BannersAddByUrlEnabled")]
            internal bool BannersAddByUrlEnabled { get { return _BannersAddByUrlEnabled; } set { _BannersAddByUrlEnabled = value; SetUpToDateAsFalse(); } }

            [Column("ShowQuickStartGuideEnabled")]
            internal bool ShowQuickStartGuideEnabled { get { return _ShowQuickStartGuide; } set { _ShowQuickStartGuide = value; SetUpToDateAsFalse(); } }

            [Column("CustomFooterContent")]
            internal string CustomFooterContent { get { return _CustomFooterContent; } set { _CustomFooterContent = value; SetUpToDateAsFalse(); } }

            [Column("CustomHeaderContent")]
            internal string CustomHeaderContent { get { return _CustomHeaderContent; } set { _CustomHeaderContent = value; SetUpToDateAsFalse(); } }

            [Column("Theme")]
            internal string Theme { get { return _Theme; } set { _Theme = value; SetUpToDateAsFalse(); } }

            [Column("AfterLoginPopupEnabled")]
            internal bool EnableAfterLoginPopup { get { return _afterLoginPopupEnabled; } set { _afterLoginPopupEnabled = value; SetUpToDateAsFalse(); } }

            [Column("AfterLoginPopupTitle")]
            internal string AfterLoginPopupTitle { get { return _afterLoginPopupTitle; } set { _afterLoginPopupTitle = value; SetUpToDateAsFalse(); } }

            [Column("AfterLoginPopupContent")]
            internal string AfterLoginPopupContent { get { return _afterLoginPopupContent; } set { _afterLoginPopupContent = value; SetUpToDateAsFalse(); } }

            [Column("IsMulticurrencyPricingEnabled")]
            internal bool IsMulticurrencyPricingEnabled { get { return _IsMulticurrencyPricingEnabled; } set { _IsMulticurrencyPricingEnabled = value; SetUpToDateAsFalse(); } }

            [Column("ChoosedLanguages")]
            internal string ChoosedLanguages { get { return _ChoosedLanguages; } set { _ChoosedLanguages = value; SetUpToDateAsFalse(); } }

            [Column("DefaultLanguage")]
            internal string DefaultLanguage { get { return _DefaultLanguage; } set { _DefaultLanguage = value; SetUpToDateAsFalse(); } }

            #region SocialLinks
            [Column("FacebookLink")]
            internal string FacebookLink { get { return _facebookLink; } set { _facebookLink = value; SetUpToDateAsFalse(); } }
            [Column("InstagramLink")]
            internal string InstagramLink { get { return _instagramLink; } set { _instagramLink = value; SetUpToDateAsFalse(); } }
            [Column("TwitterLink")]
            internal string TwitterLink { get { return _twitterLink; } set { _twitterLink = value; SetUpToDateAsFalse(); } }
            [Column("GooglePlusLink")]
            internal string GooglePlusLink { get { return _googlePlusLink; } set { _googlePlusLink = value; SetUpToDateAsFalse(); } }
            [Column("DribbbleLink")]
            internal string DribbbleLink { get { return _dribbbleLink; } set { _dribbbleLink = value; SetUpToDateAsFalse(); } }
            #endregion

            private int _websiteMode, _AdBlockPolicy;
            private bool _IsEUCookieEnabled, _IsCurrencySignBefore, _AllowOnlyOneRegisteredIP, _DeveloperModeEnabled, _ForumEnabled, _LatestNewsEnabled,
                _URLCheckerAllowRedirect, _ShowRemainingZeros, _FooterRemoved, _IsAutoApprovalEnabled, _IsAutoAactiveEnabled, _BannersAddByUrlEnabled, _ShowSiteName,
                _ShowQuickStartGuide, _afterLoginPopupEnabled, _IsMulticurrencyPricingEnabled, _CommasInNumbersEnabled;
            private string _SiteDescription, _CurrencySign, _CurrencyCode, _siteSlogan, _siteName, _siteUrl,
                           _MainColor, _LightColor, _DarkColor, _LogoImageURL, _FaviconImageURL, _facebookLink, _instagramLink, _twitterLink, _googlePlusLink, _dribbbleLink,
                           _CustomFooterContent, _CustomHeaderContent, _Theme, _afterLoginPopupTitle, _afterLoginPopupContent, _ChoosedLanguages, _DefaultLanguage;
            private DateTime _MaintenanceModeEndsDate;

            //Save & reload section

            internal void ReloadSite()
            {
                ReloadPartially(IsUpToDate, buildSiteProperties());
            }

            internal void SaveSite()
            {
                SavePartially(IsUpToDate, buildSiteProperties());
            }

            private PropertyInfo[] buildSiteProperties()
            {
                var paymentsValues = new PropertyBuilder<AppSettingsTable>();
                paymentsValues
                    .Append(x => x.SiteName)
                    .Append(x => x.SiteUrl)
                    .Append(x => x.SiteSlogan)
                    .Append(x => x.SiteDescription)
                    .Append(x => x.ShowSiteName)
                    .Append(x => x.IsEUCookieEnabled)
                    .Append(x => x.CurrencySign)
                    .Append(x => x.CurrencyCode)
                    .Append(x => x.IsCurrencySignBefore)
                    .Append(x => x.AdBlockPolicy)
                    .Append(x => x.MainColor)
                    .Append(x => x.LightColor)
                    .Append(x => x.DarkColor)
                    .Append(x => x.LogoImageURL)
                    .Append(x => x.FaviconImageURL)
                    .Append(x => x.AllowOnlyOneRegisteredIP)
                    .Append(x => x.MaintenanceModeEndsDate)
                    .Append(x => x.ForumEnabled)
                    .Append(x => x.LatestNewsEnabled)
                    .Append(x => x.DeveloperModeEnabled)
                    .Append(x => x.websiteMode)
                    .Append(x => x.URLCheckerAllowRedirect)
                    .Append(x => x.ShowRemainingZeros)
                    .Append(x => x.FooterRemoved)
                    .Append(x => x.IsAutoApprovalEnabled)
                    .Append(x => x.IsAutoAactiveEnabled)
                    .Append(x => x.FacebookLink)
                    .Append(x => x.InstagramLink)
                    .Append(x => x.TwitterLink)
                    .Append(x => x.GooglePlusLink)
                    .Append(x => x.DribbbleLink)
                    .Append(x => x.ShowQuickStartGuideEnabled)
                    .Append(x => x.CustomFooterContent)
                    .Append(x => x.CustomHeaderContent)
                    .Append(x => x.Theme)
                    .Append(x => x.EnableAfterLoginPopup)
                    .Append(x => x.AfterLoginPopupTitle)
                    .Append(x => x.AfterLoginPopupContent)
                    .Append(x => x.IsMulticurrencyPricingEnabled)
                    .Append(x => x.CommasInNumbersEnabled)
                    .Append(x => x.ChoosedLanguages)
                    .Append(x => x.DefaultLanguage)
                    ;

                return paymentsValues.Build();
            }
        }

    }
}