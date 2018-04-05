using System;
using System.Web.UI.WebControls;
using Prem.PTC.Members;
using Resources;
using Prem.PTC;
using System.Text;
using System.Web.Services;

public partial class About : System.Web.UI.Page
{
    //in ms
    public const int InstantAccrualsIntervalTime = 1000;

    protected void Page_Load(object sender, EventArgs e)
    {      
        if (!Member.IsLogged)
            Response.Redirect(AppSettings.Site.Url);

        var User = Member.CurrentInCache;

        if ((Member.Current.NumberOfWatchedTrafficAdsToday < AppSettings.TrafficExchange.AmountOfAdsToWatchForcedByAdmin))
            NoEarnAccessPlaceHolder.Visible = true;
        else
            NoEarnAccessPlaceHolder.Visible = false;

        #region Customizations
        if (TitanFeatures.IsRetireYoung)
        {
            var balanceField = new BoundField { HeaderText = U5009.BALANCE, DataField = "Id" };
            var aggregateField = new BoundField { HeaderText = U6010.AGGREGATE, DataField = "Id" };

            HistoryGridView.Columns.Add(balanceField);
            HistoryGridView.Columns.Add(aggregateField);

            HistoryGridView.Columns[1].HeaderText = U6010.CONCEPT;
        }

        if (TitanFeatures.IsTrafficThunder)
            AdPacksPlaceHolder.Visible = false;
        else
            AdPacksPlaceHolder.Visible = AppSettings.RevShare.IsRevShareEnabled && AppSettings.TitanFeatures.AdvertAdPacksEnabled && User.IsEarner;

        #endregion

        LogsGridView.EmptyDataText = L1.NODATA;
        HistoryGridView.EmptyDataText = L1.NODATA;

        //Profiling survey
        if (this.CameStraightAfterLogin && !SkippedProfiling && !ProfilingManager.IsProfiled(User))
            Response.Redirect("~/sites/profiler.aspx");

        if ((this.CameStraightAfterLogin && AppSettings.Site.EnableAfterLoginPopup) && (!AppSettings.Site.ShowQuickStartGuideEnabled
            || (AppSettings.Site.ShowQuickStartGuideEnabled && Member.CurrentInCache.IsQuickGuideViewed)))
            DisplayAfterLoginPopup();

        UserStats.StatTitle = U5001.TOTALPOINTSCREDITEDTOYOU.Replace("%n%", AppSettings.PointsName);


        if (User.ResolveReferralsLimitDate != null && User.MembershipExpires == null)
        {
            WarningMemberExpiresAndReferralsResolvedPlaceHolder.Visible = true;
            ReferralsWillBeResolvedWarningLiteral.Text = U6006.WARNING;
            ReferralsWillBeResolveMessageLiteral.Text = string.Format(U6006.MEMBERSHIPEXPIREDREFERRALSWILLBERESOLVED, User.ResolveReferralsLimitDate.Value.Day - DateTime.Now.Day);
        }

        var jackpotWinners = new StringBuilder();
        if (AppSettings.Addons.ShowLastJackpotsWinnersOnUserDashboard && JackpotManager.GetLastJackpotsWinners(out jackpotWinners))
        {
            LastJakpotsWinnersPlaceHolder.Visible = true;
            JakpotsWinnersLiteral.Text = U6008.LASTJACKPOTWINNERSCONGRATULATIONS;
            JakpotsWinnersDetailsLiteral.Text = jackpotWinners.ToString();
        }

        pointsGraph.Visible = AppSettings.Points.PointsEnabled;
        InstantAccrualChartPlaceHolder.Visible = AppSettings.RevShare.AdPack.InstantAccrualsEnabled;
    }

    [WebMethod]
    public static string GetJsonWithInstantAccrualsValues()
    {
        return RevShareManager.GetJsonWithInstantAccrualsValues(Member.CurrentId);       
    }

    protected void LogsSqlDataSource_Init(object sender, EventArgs e)
    {
        String additionalWhere = "";
        if (TitanFeatures.IsTrafficThunder) additionalWhere = "AND Note<>'Purchase transfer'";
        LogsSqlDataSource.SelectCommand = String.Format("SELECT TOP 5 * FROM BalanceLogs WHERE UserId={0} {1} ORDER BY DateOccured DESC", Member.CurrentId, additionalWhere);
    }

    protected void HistorySqlDataSource_Init(object sender, EventArgs e)
    {
        if (TitanFeatures.IsRetireYoung)
            HistorySqlDataSource.SelectCommand = "SELECT TOP 5 * FROM History WHERE AssignedUsername = '" + Member.CurrentName + "' AND Type IN (27, 28, 29, 30) ORDER BY Date DESC";
        else
            HistorySqlDataSource.SelectCommand = "SELECT TOP 5 * FROM History WHERE AssignedUsername = '" + Member.CurrentName + "' ORDER BY Date DESC";
    }

    private bool CameStraightAfterLogin
    {
        get
        {
            if (Request.QueryString["afterlogin"] != null && Request.QueryString["afterlogin"] == "1")
                return true;
            return false;
        }
    }

    private bool SkippedProfiling
    {
        get
        {
            if (Request.QueryString["skippedprofiling"] != null && Request.QueryString["skippedprofiling"] == "1")
                return true;
            return false;
        }
    }

    protected void LogsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var amount = Convert.ToDecimal(e.Row.Cells[2].Text);
            var balanceType = (BalanceType)Convert.ToInt32(e.Row.Cells[3].Text);

            e.Row.Cells[2].ForeColor = amount < 0 ? System.Drawing.Color.DarkRed : System.Drawing.Color.Green;
            e.Row.Cells[2].Text = BalanceTypeHelper.GetDisplayValue(balanceType, amount);
            e.Row.Cells[3].Text = BalanceTypeHelper.GetName(balanceType);
        }
    }

    private void DisplayAfterLoginPopup()
    {
        if (Session["WelcomeModalShowed"] == null)
        {
            Session["WelcomeModalShowed"] = true;
            WelcomeModalScript.Visible = true;
        }
        else
            WelcomeModalScript.Visible = false;
    }

    protected void HistoryGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (!TitanFeatures.IsRetireYoung)
            return;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var historyId = Convert.ToInt32(e.Row.Cells[3].Text);
            var history = new History(historyId);
            var fulltext = history.GetFullText();
            var splittedText = fulltext.Split('(', '/', ')');

            e.Row.Cells[2].Text = splittedText[0];
            e.Row.Cells[3].Text = splittedText[1];
            e.Row.Cells[4].Text = splittedText[2] ?? "-";
        }
    }

    protected void HistoryLinkButtonRefresh_Click(object sender, EventArgs e)
    {
        HistoryGridView.DataBind();
    }

    protected void BalanceLogslinkButtonRefresh_Click(object sender, EventArgs e)
    {
        LogsGridView.DataBind();
    }
}
