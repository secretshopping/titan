using System;
using System.Web.UI.WebControls;
using Prem.PTC.Members;
using Resources;
using Titan.Shares;


public partial class About : System.Web.UI.Page
{
    public string jsSelectAllCode;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Member.IsLogged)
        {
            UserName.Text = Member.CurrentName;
        }

        AuctionGrid1.EmptyDataText = U4000.YOUDONTHAVEANYSHARES;

        LangAdder.Add(Button1, U4000.MYUNITS);
    }
   
    public void MenuButton_Click(object sender, EventArgs e)
    {
        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        if (viewIndex == 0)
            Response.Redirect("units.aspx");

        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";
    }

    protected void AuctionGrid1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            PortfolioShare share = new PortfolioShare(Convert.ToInt32(e.Row.Cells[0].Text));
            e.Row.Cells[1].Text = share.Product.Name;
            e.Row.Cells[2].Text = share.Shares.ToString();
            e.Row.Cells[3].Text = ((Decimal)share.Shares * 100 / (Decimal)share.Product.TotalShares).ToString("F4") + "%";
        }
    }

    protected void AuctionGrid1_DataSource_Init(object sender, EventArgs e)
    {
        AuctionGrid1_DataSource.SelectCommand =
        String.Format(@"SELECT * FROM PortfolioShares WHERE OwnerUsername = '" + Member.CurrentName + "'");
    }
}
