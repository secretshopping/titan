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

public partial class About : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Append Data
        HistoryGridView.DataSource = GetGridViewData();
        HistoryGridView.DataBind();
    }

    private DataTable GetGridViewData()
    {
        var dt = new DataTable();
        dt.Columns.Add("Date", "a".GetType());
        dt.Columns.Add("Banner", "a".GetType());
        dt.Columns.Add("Portfolio units", "a".GetType());
        dt.Columns.Add("Promotion", "a".GetType());
        dt.Columns.Add("Revenue Share Ads", "a".GetType());
        dt.Columns.Add("Charity", "a".GetType());

        for (int j = 0; j < 14; j++)
        {
            DateTime date = DateTime.Now.Date.AddDays(-j);

            var dr = dt.NewRow();
            dr["Date"] = date.ToShortDateString();
            dr["Banner"] = EarningsStatsManager.GetEarnings(EarningsStatsType.Banner, date).ToString();
            dr["Portfolio units"] = EarningsStatsManager.GetEarnings(EarningsStatsType.PortfolioUnits, date).ToString();
            dr["Promotion"] = EarningsStatsManager.GetEarnings(EarningsStatsType.Promotion, date).ToString();
            dr["Revenue Share Ads"] = EarningsStatsManager.GetEarnings(EarningsStatsType.RevenueShareAds, date).ToString();
            dr["Charity"] = EarningsStatsManager.GetEarnings(EarningsStatsType.Charity, date).ToString();
            dt.Rows.Add(dr);
        }

        return dt;
    }
}
