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

public partial class Controls_Banner : System.Web.UI.UserControl
{
    /// <summary>
    /// Only when BannerBidding is enabled. If you want to display more than 1 banner 
    /// on particular page (e.g. display 1st, 2nd & 3rd) Auction winner,
    /// you can select here the position (1, 2, or 3). Set to 1 by default
    /// </summary>
    public string BannerBidPosition { get; set; }
    public int? DimensionId { get; set; }

    //Set to false by default. If set to true, the control can only be loaded by explicitly calling Activate() method on the control.
    private bool preventLoading = false;
    private string sessionContainerName
    {
        get
        {
            return "Controls_Banner" + this.UniqueID;
        }
    }

    private Control GetBanner()
    {
        if (!DimensionId.HasValue)
            DimensionId = AppSettings.BannerAdverts.SurfBannerDimensionsID;

        if (AppSettings.BannerAdverts.AdvertisingPolicy == BannerPolicy.BannerBidding
           && !string.IsNullOrWhiteSpace(BannerBidPosition))
            return BannerDisplayer.GetBanner(new BannerAdvertDimensions(DimensionId.Value), this.Page, Convert.ToInt32(BannerBidPosition));

        return BannerDisplayer.GetBanner(new BannerAdvertDimensions(DimensionId.Value), this.Page, 1);
    }

    #region Control setup

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!AppSettings.BannerAdverts.HideAllBannersEnabled)
        {
            if (!PreventLoading && !IsPostBack)
            {
                Activate();
            }

            MainPanel.Controls.Clear();
            MainPanel.Controls.Add(LoadedBanner);
        }
    }

    private Control LoadedBanner
    {
        get
        {
            if (Session[sessionContainerName] == null)
                LoadedBanner = GetBanner();
            return (Control)Session[sessionContainerName];
        }
        set
        {
            Session[sessionContainerName] = value;
        }
    }

    public void Activate()
    {
        Session[sessionContainerName] = null;
    }

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

    #endregion
}
