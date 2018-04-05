using System;
using System.Web.UI.WebControls;
using Prem.PTC.Members;

public partial class About : System.Web.UI.Page
{
    public string jsSelectAllCode;

    protected void Page_Load(object sender, EventArgs e)
    {   
        if (Member.IsLogged)        
            UserName.Text = Member.CurrentName;        
    }

    protected void AuctionGrid1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {           
            // 4 - Total shares
            if (e.Row.Cells[4].Text == "0")
                e.Row.Cells[4].Text = "-";
        }
    }

    protected void AuctionGrid1_DataSource_Init(object sender, EventArgs e)
    {
        AuctionGrid1_DataSource.SelectCommand =
        String.Format(@"SELECT * FROM PortfolioProducts");
    } 
}
