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
using Prem.PTC.Texts;
using Prem.PTC.Utils;
using ExtensionMethods;
using Titan.Marketplace;

public partial class Controls_MarketplaceProduct : System.Web.UI.UserControl, MarketplaceProductObjectControl
{
    public MarketplaceProduct Object { get; set; }
    public bool IsPreview { get; set; }

    public override void DataBind()
    {
        btn.Text = L1.BUY + " (" + Object.Price + ")";
        base.DataBind();
    }

    public string Title { get { return Object.Title; } }
    public string Description { get { return Object.Description; } }
    public string Contact { get { return Object.Contact; } }
    public string Quantity { get { return Object.Quantity.ToString(); } }
    public string ImageURL { get { return Object.ImagePath; } }

    protected void ProductInfo_Click(object sender, EventArgs e)
    {
        int ProductId = Convert.ToInt32(((Button)sender).CommandArgument);
        HttpContext.Current.Response.Redirect("~/user/advert/marketplace.aspx?pid=" + ProductId);
    }

    

    

}
