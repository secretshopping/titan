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
using Titan.Shares;

public partial class Controls_RevShareNormalBanner : System.Web.UI.UserControl
{
    /// <summary>
    /// Start with 0 and increment for each new control of the same type.
    /// Prevents loading the same banner more than once if there is more banners on the page then in the database.
    /// </summary>
    public int IndexOnPage { get; set; }

    //Set to false by default. If set to true, the control can only be loaded by explicitly calling Activate() method on the control.
    private bool preventLoading = false;
    private readonly string sessionContainerName = "Controls_RevShareNormalBanner";

    private Control GetBanner()
    {
        return AdPackBannerDisplayer.GetBanner(BannerAdvert.Type.Normal, IndexOnPage);
    }

    #region Control setup

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!PreventLoading && !IsPostBack)
        {
            Activate();
        }

        MainPanel.Controls.Add(LoadedBanner);
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
