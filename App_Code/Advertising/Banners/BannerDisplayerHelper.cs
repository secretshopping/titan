using Prem.PTC;
using Prem.PTC.Advertising;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;


public class BannerDisplayerHelper
{
    protected BannerAdvertDimensions dimensions;

    public BannerDisplayerHelper(BannerAdvertDimensions dimensions)
    {
        this.dimensions = dimensions;
    }

    public HyperLink GetBannerHyperLink()
    {
        HyperLink _workingBanner = new HyperLink();

        //Border
        _workingBanner.BorderColor = System.Drawing.Color.FromArgb(220, 220, 220); //Grey
        _workingBanner.BorderStyle = BorderStyle.Solid;
        _workingBanner.BorderWidth = Unit.Pixel(1);
        _workingBanner.Target = "_blank";

        return _workingBanner;
    }

    public Image GetBannerImage(BannerAdvert advert)
    {
        Image banner = new Image();

        banner.ImageUrl = advert.ImagePath;
        banner.CssClass = "img-responsive";

        return banner;
    }
}