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

public partial class AdPacks : System.Web.UI.Page
{
    Member User;
    Dictionary<int, int> adPackTypesPositions = new Dictionary<int, int>();

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.StatisticsAdPacksEnabled);
        User = Member.CurrentInCache;
        if (!IsPostBack)
        {
            Button1.Text = L1.REFERRALS;
            Button2.Text = U6006.EARNINGHISTORY;
            TotalLiteral.Text = User.TotalDREarnedFromAdPacks.ToString();

            SetColumns();
        }
        PrepareAdPackPositions();
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

    protected void SetColumns()
    {
        AdPacksStatsGridView.Columns[0].HeaderText = L1.ID;
        AdPacksStatsGridView.Columns[1].HeaderText = L1.DATE;
        AdPacksStatsGridView.Columns[2].HeaderText = U6006.POSITION;
        AdPacksStatsGridView.Columns[3].HeaderText = L1.AMOUNT;
        AdPacksStatsGridView.Columns[4].HeaderText = U6006.EARNING;
        AdPacksStatsGridView.Columns[5].HeaderText = L1.STATUS;
        AdPacksStatsGridView.Columns[6].HeaderStyle.CssClass = "displaynone";
        AdPacksStatsGridView.Columns[6].ItemStyle.CssClass = "displaynone";
    }

    private void PrepareAdPackPositions()
    {
        var adpackTypesList = TableHelper.GetListFromRawQuery(string.Format("SELECT AdPackTypeId FROM AdPacks WHERE UserId = {0}", User.Id));

        foreach (var type in adpackTypesList)
        {
            if (adPackTypesPositions.ContainsKey(type))
                adPackTypesPositions[type]++;
            else
                adPackTypesPositions.Add(type, 1);
        }
    }

    protected void AdPacksStatsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var moneyReturned = Convert.ToDecimal(e.Row.Cells[4].Text);
            var moneyToReturn = Convert.ToDecimal(e.Row.Cells[6].Text);

            if (moneyToReturn > moneyReturned)
            {
                e.Row.Cells[5].ForeColor = System.Drawing.Color.Green;
                e.Row.Cells[5].Text = L1.ACTIVE;
            }
            else
            {
                e.Row.Cells[5].ForeColor = System.Drawing.Color.Purple;
                e.Row.Cells[5].Text = L1.COMPLETED;
            }

            var type = Convert.ToInt32(e.Row.Cells[2].Text);
            e.Row.Cells[2].Text = adPackTypesPositions[type].ToString();

            e.Row.Cells[3].Text = Prem.PTC.Money.Parse(e.Row.Cells[3].Text).ToString();
            e.Row.Cells[4].Text = Prem.PTC.Money.Parse(e.Row.Cells[4].Text).ToString();
        }
    }

    protected void AdPacksStatsGridView_DataSource_Init(object sender, EventArgs e)
    {
        AdPacksStatsGridView_DataSource.SelectCommand = string.Format(@"
            SELECT A.Id, A.PurchaseDate,  A.MoneyToReturn, A.MoneyReturned, A.AdPackTypeId, A.MoneyToReturn, B.Price
            FROM Adpacks A
            JOIN AdPackTypes B
            ON A.AdPackTypeId = B.Id
            WHERE A.UserId = {0}",
            Member.CurrentId);
    }
}
