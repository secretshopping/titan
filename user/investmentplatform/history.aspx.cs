using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Utils;
using Resources;
using System;
using System.Web.UI.WebControls;
using Titan.InvestmentPlatform;

public partial class user_investmentplatform_history : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.InvestmentPlatformHistoryEnabled);

        if (!IsPostBack)
        {
            HistoryGridView.EmptyDataText = L1.NODATA;
            HistoryGridView.DataBind();

            TicketsGridView.EmptyDataText = L1.NODATA;
            TicketsGridView.DataBind();

            if (!AppSettings.InvestmentPlatform.ProofsEnabled)
            {
                HistoryGridView.Columns.RemoveAt(8);
                TicketsGridView.Columns.RemoveAt(7);
            }

            HistoryGridViewPlaceHolder.Visible = !AppSettings.InvestmentPlatform.LevelsEnabled;
            TicketsGridViewPlaceHolder.Visible = AppSettings.InvestmentPlatform.LevelsEnabled;
        }
    }

    protected void HistoryGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var plan = new InvestmentPlatformPlan(int.Parse(e.Row.Cells[2].Text));

            e.Row.Cells[2].Text = plan.Name;

            //Money Returned
            var returnedMoney = Money.Parse(e.Row.Cells[3].Text);
            e.Row.Cells[3].Text = returnedMoney.ToString();

            //Money to Return
            var moneyToReturn = Money.Parse(e.Row.Cells[4].Text);
            e.Row.Cells[4].Text = moneyToReturn.ToString();

            //Money in System
            e.Row.Cells[5].Text = Money.Parse(e.Row.Cells[5].Text).ToString();

            if (returnedMoney >= moneyToReturn)
                e.Row.Cells[3].ForeColor = System.Drawing.Color.Green;
            else
                e.Row.Cells[3].ForeColor = System.Drawing.Color.DarkRed;

            var Type = (PurchaseBalances)Convert.ToInt32(e.Row.Cells[6].Text);
            switch (Type)
            {
                case PurchaseBalances.Purchase:
                    e.Row.Cells[6].Text = U6012.PURCHASEBALANCE;
                    break;
                case PurchaseBalances.Cash:
                    e.Row.Cells[6].Text = U5008.CASHBALANCE;
                    break;
                default:
                    break;
            }

            PlanStatus status = (PlanStatus)Convert.ToInt32(e.Row.Cells[7].Text);
            e.Row.Cells[7].Text = HtmlCreator.GetColoredStatus(status);
        }
    }

    protected void HistorySqlDataSource_Init(object sender, EventArgs e)
    {
        string command = string.Format("SELECT * FROM {0} WHERE UserId = {1} AND [Status] != {2} ORDER BY {3} DESC",
            InvestmentUsersPlans.TableName, Member.CurrentId, (int)PlanStatus.Removed, InvestmentUsersPlans.Columns.PurchaseDate);

        HistorySqlDataSource.SelectCommand = command;
    }

    protected void HistoryGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {        
        if (e.CommandName == "download")
        {
            int index = e.GetSelectedRowIndex() % HistoryGridView.PageSize;
            GridViewRow row = HistoryGridView.Rows[index];
            var plan = new InvestmentUsersPlans(Convert.ToInt32(row.Cells[0].Text.Trim()));

            var proof = new HtmlInvestmentProofGenerator(plan);
            proof.DownloadPdf();
        }
    }

    protected void TicketsSqlDataSource_Init(object sender, EventArgs e)
    {
        string command = string.Format("SELECT * FROM {0} WHERE UserId = {1} ORDER BY {2} DESC",
                    InvestmentTicket.TableName, Member.CurrentId, InvestmentTicket.Columns.Date);

        TicketsSqlDataSource.SelectCommand = command;
    }

    protected void TicketsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var level = Convert.ToInt32(e.Row.Cells[2].Text);
            e.Row.Cells[2].Text = InvestmentPlatformPlan.GetNameByLevel(level);

            var levelPrice = Money.Parse(e.Row.Cells[3].Text);
            e.Row.Cells[3].Text = levelPrice.ToString();

            var levelFee = Money.Parse(e.Row.Cells[4].Text);
            e.Row.Cells[4].Text = levelFee.ToString();

            var levelEarnings = Money.Parse(e.Row.Cells[5].Text);
            e.Row.Cells[5].Text = levelEarnings.ToString();

            TicketStatus status = (TicketStatus)Convert.ToInt32(e.Row.Cells[6].Text);
            e.Row.Cells[6].Text = HtmlCreator.GetColoredStatus(status);
        }
    }

    protected void TicketsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "download")
        {
            int index = e.GetSelectedRowIndex() % TicketsGridView.PageSize;
            GridViewRow row = TicketsGridView.Rows[index];
            var ticket = new InvestmentTicket(Convert.ToInt32(row.Cells[0].Text.Trim()));

            var proof = new HtmlInvestmentProofGenerator(ticket);
            proof.DownloadPdf();
        }
    }
}