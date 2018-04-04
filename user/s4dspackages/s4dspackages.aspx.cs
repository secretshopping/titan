using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using System.Data;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using Titan.Shares;
using Prem.PTC.Advertising;
using Prem.PTC.Memberships;
using System.Text;
using Prem.PTC.Utils;
using Prem.PTC.Texts;

public partial class Page_advert_Adpacks : System.Web.UI.Page
{
    new Member User;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            ActivePacksTab.Text = "List";
            User = Member.CurrentInCache;

            AccessManager.RedirectIfDisabled(TitanFeatures.IsEpadilla);

            Form.Action = Request.RawUrl;
            bool hasAvailableAdverts = AdPackManager.HasAvailableAdverts(User.Id);
        }
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";
    }

    #region S4DS PACKS GRIDVIEW
    protected void UserS4DSPacksGridView_DataSource_Init(object sender, EventArgs e)
    {
        UserS4DSPacksGridView_DataSource.SelectCommand = String.Format("SELECT * FROM AdPacks WHERE UserId={0} ORDER BY PurchaseDate DESC", Member.CurrentId);
    }

    protected void UserS4DSPacksGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            String Addon = Convert.ToDecimal(e.Row.Cells[1].Text) >= 120 ? "Finished: " : "Purchased: ";

            AdPack adPack = new AdPack(Convert.ToInt32(e.Row.Cells[0].Text));
            AdPackTypesCache cache = new AdPackTypesCache();
            var adPackTypes = (Dictionary<int, AdPackType>)cache.Get();

            e.Row.Cells[1].Text = HtmlCreator.GenerateCPAAdProgressHTML(
                new Money(Convert.ToDecimal(e.Row.Cells[1].Text)).ToDecimal(), adPack.MoneyToReturn.ToDecimal(), AppSettings.Site.CurrencySign);
            e.Row.Cells[2].Text = Addon + e.Row.Cells[2].Text;
        }
    }

    #endregion

}
