using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using Titan.Advertising;

public class BannerDisplayer
{
    private static HyperLink _workingBanner;
    private static List<BannerAdvert> allBanners;
    private static Random random = new Random();

    public static Control GetBanner(BannerAdvertDimensions dimensions, Page page, int position = 1)
    {
        allBanners = TryGetFromCache(dimensions, page, position);

        List<BannerAdvert> allBannersFiltered = new List<BannerAdvert>();

        if (AppSettings.BannerAdverts.AdvertisingPolicy == BannerPolicy.SellingPackages)
        {
            Member User = null;
            if (Member.IsLogged)
                User = Member.CurrentInCache;

            foreach (var elem in allBanners)
            {
                if (!elem.IsGeolocated)
                    allBannersFiltered.Add(elem);
                else if (elem.IsGeolocated && Member.IsLogged && elem.BannedCountries.Contains(User.Country))
                    allBannersFiltered.Add(elem);
            }
        }
        else
            allBannersFiltered = allBanners; //No geolocation in Banner Bidding       

        _workingBanner = new HyperLink();
        int bannerIndex = random.Next(0, allBannersFiltered.Count);

        _workingBanner = (HyperLink)GetPanelWithDimensionsAndBorder(dimensions, _workingBanner);        

        if (allBannersFiltered.Count > 0)
        {
            BannerAdvert ChoosenBanner = allBannersFiltered[bannerIndex];

            var te = new Encryption();
            HttpContext.Current.Session["SurfBannerAdvertID"] = ChoosenBanner.Id;            

            if (ChoosenBanner.ShouldBeFinished)
            {
                ChoosenBanner.Status = AdvertStatus.Finished;
                ChoosenBanner.SaveStatus();
            }

            if (AppSettings.BannerAdverts.ImpressionsEnabled)
            {
                //Count impression
                ChoosenBanner.Click();
            }

            //Dynamic banner (HTML code instead of image)
            if (ChoosenBanner.IsDynamic)
            {
                Panel panel = new Panel();
                panel = (Panel)GetPanelWithDimensionsAndBorder(dimensions, panel);

                Literal literal = new Literal();
                literal.Text = ChoosenBanner.BannerHTML;

                panel.Controls.Add(literal);
                return panel;
            }

            dimensions = ChoosenBanner.Dimensions;
            var Helper = new BannerDisplayerHelper(dimensions);

            //Now lets create Image Web Control and return it           
            
            _workingBanner.Target = "_blank";
            _workingBanner.NavigateUrl = AppSettings.Site.Url + "link.aspx?id=" + te.EncryptBannerId(ChoosenBanner.Id);

            _workingBanner.Controls.Clear();
            _workingBanner.Controls.Add(Helper.GetBannerImage(ChoosenBanner));
        }
        return _workingBanner;
    }

    private static WebControl GetPanelWithDimensionsAndBorder(BannerAdvertDimensions dimensions, WebControl control)
    {
        /* unnecessary border causing errors with displaying
        try
        {
            //Weird exception is sometimes thrown from there
            control.BorderColor = System.Drawing.Color.FromArgb(220, 220, 220); //Grey
            control.BorderStyle = BorderStyle.Solid;
            control.BorderWidth = Unit.Pixel(1);
        }
        catch (Exception ex)
        { }
        */
        return control;
    }


    private static List<BannerAdvert> TryGetFromCache(BannerAdvertDimensions dimensions, Page page, int position = 1)
    {
        if (AppSettings.BannerAdverts.AdvertisingPolicy == BannerPolicy.SellingPackages)
        {
            var cache = new AdvertBannersCache();
            var dictionary = (BannerDimensionsDictionary)cache.Get();
            return dictionary.GetAdverts(dimensions.Id);
        }

        if (AppSettings.BannerAdverts.AdvertisingPolicy == BannerPolicy.BannerBidding)
        {
            var cache = new AdvertBannersBiddingCache();
            var dictionary = (BannerBiddingDimensionsDictionary)cache.Get();

            //Position is not yet supported (defaults to 1)
            return dictionary.GetAdverts(dimensions.Id, 1);
        }

        return null;
    }
}