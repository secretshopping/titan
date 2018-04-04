using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Titan;
using Prem.PTC;
using Resources;
using Prem.PTC.Members;
using System.Text;
using Prem.PTC.Advertising;
using Titan.Advertising;

public partial class Controls_BannerAuctionLink : System.Web.UI.UserControl
{
    /// <summary>
    /// Banner type: Normal or Constant
    /// </summary>
    public string BannerType { get; set; }
    public string Style { get; set; }

    private bool preventLoading = false;

    /// <summary>
    /// Set to false by default. If set to true, the control can only be loaded by explicitly calling Activate() method on the control.
    /// </summary>
    public bool PreventLoading
    {
        get
        {
            return preventLoading;
        }
        set
        {
            preventLoading = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!PreventLoading && !IsPostBack)
        {
            Activate();
        }
    }

    private void Activate()
    {
        if (AppSettings.BannerAdverts.AdvertisingPolicy == BannerPolicy.SellingPackages)
            ControlPlaceHolder.Visible = false;
        else
        {
            //BannerAdvert.Type type = BannerAdvert.Type.Normal;

            //if (!string.IsNullOrEmpty(BannerType) && BannerType.StartsWith("C"))
            //    type = BannerAdvert.Type.Constant;

            //BannerAuction result = BannerAuctionManager.GetFirstActiveAuction(type);

            //if (result != null)
            //{
            //    BannerBid highest = result.HighestBid;
            //    if (highest != null)
            //        PriceLiteral.Text = highest.ToString();
            //    else if (type == BannerAdvert.Type.Normal)
            //        PriceLiteral.Text = AppSettings.BannerAdverts.StartingAmount.ToString();
            //    else
            //        PriceLiteral.Text = AppSettings.BannerAdverts.StartingAmountConstant.ToString();
            //}
        }
    }
}
