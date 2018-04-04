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
using System.Text;
using System.Threading;
using MarchewkaOne.Titan.Balances;

public partial class About : System.Web.UI.Page
{
    public static int MaxDisplayedLogs = 200;
    
    readonly bool AdPacksEnabled = AppSettings.TitanFeatures.AdvertAdPacksEnabled || AppSettings.TitanFeatures.EarnAdPacksEnabled;

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.MoneyLogsEnabled);

        if (!IsPostBack)
        {
            LogsGridView.EmptyDataText = L1.NODATA;
            AdPacksCheckBox.Text = AppSettings.RevShare.AdPack.AdPackNamePlural;
            OthersCheckBox.Text = U2600.CPACAT16;
            
            AdPacksCheckBox.Visible = AdPacksEnabled;
            OthersCheckBox.Visible = ShowBalanceLogsFilters.Visible = AdPacksEnabled;
            
            AdPacksCheckBox.Checked = true;
            OthersCheckBox.Checked = true;

            BindToLogsGridView();
            LogsGridView.DataBind();

        }
    }

    private void BindToLogsGridView()
    {
        //string andCommand = GetSelectedLogCommand();
        string command = string.Format("SELECT TOP {0} * FROM BalanceLogs WHERE UserId = {1} ORDER BY DateOccured DESC", MaxDisplayedLogs, Member.CurrentId);
        LogsSqlDataSource.SelectCommand = command;
    }

    private string GetSelectedLogCommand()
    {
        StringBuilder andQuery = new StringBuilder();

        andQuery.AppendFormat("1=1 ");

        if (!OthersCheckBox.Checked)
        {
            andQuery.AppendFormat("AND BalanceLogType <> {0} ", (int)BalanceLogType.Other);
        }
        
        if (!AdPacksCheckBox.Checked || !AdPacksEnabled)
        {
            andQuery.AppendFormat("AND BalanceLogType <> {0} ", (int)BalanceLogType.AdPackPurchase);
            andQuery.AppendFormat("AND BalanceLogType <> {0} ", (int)BalanceLogType.AdPackRefPurchase);
            andQuery.AppendFormat("AND BalanceLogType <> {0} ", (int)BalanceLogType.AdPackROI);
        }

        return andQuery.ToString();
    }

    protected void Filter()
    {
        StringBuilder filterBuilder = new StringBuilder();
        filterBuilder.Append(GetSelectedLogCommand());

        LogsSqlDataSource.FilterExpression = filterBuilder.ToString();
        DataBind();

        FilteredDataSource = LogsSqlDataSource.FilterExpression;
    }

    protected void LogsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[1].Text = e.Row.Cells[1].Text.Replace("<b>", "<i>").Replace("</b>", "</i>"); ;

            var amount = Convert.ToDecimal(e.Row.Cells[2].Text);
            var balanceType = (BalanceType)Convert.ToInt32(e.Row.Cells[3].Text);

            e.Row.Cells[2].ForeColor = amount < 0 ? System.Drawing.Color.DarkRed : System.Drawing.Color.Green;
            e.Row.Cells[2].Text = BalanceTypeHelper.GetDisplayValue(balanceType, amount);
            e.Row.Cells[3].Text = BalanceTypeHelper.GetName(balanceType);
        }
    }
  
    protected void LogTypeCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        BindToLogsGridView();
        Filter();
        LogsGridView.DataBind();
    }

    protected void LogsSqlDataSource_Init(object sender, EventArgs e)
    {
        BindToLogsGridView();
    }

    private string _FilteredDataSource;
    protected string FilteredDataSource
    {
        get
        {
            if (_FilteredDataSource == null)
            {
                _FilteredDataSource = Session["_FilteredDataSource"] as string;
            }

            return _FilteredDataSource;
        }
        set { Session["_FilteredDataSource"] = _FilteredDataSource = value; }
    }
    protected void LogsGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        if (FilteredDataSource == null)
            FilteredDataSource = LogsSqlDataSource.FilterExpression;
        LogsSqlDataSource.FilterExpression = FilteredDataSource;
        LogsGridView.DataBind();
    }
}
