using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Text;
using System.Web.UI.WebControls;
using Titan.RollDiceLottery;

public partial class user_entertainment_rolldicelottery : System.Web.UI.Page
{
    private int timeCounter;
    private Member User;
    private RollDiceLotteryManager _rollDiceManager;
    private RollDiceLotteryManager RollDiceManager
    {
        get
        {
            if (_rollDiceManager == null)
                _rollDiceManager = (RollDiceLotteryManager)ViewState["RollDiceLotteryManager"];

            return _rollDiceManager;
        }
        set { ViewState["RollDiceLotteryManager"] = value; }
    }

    private bool IsLotteryRunning
    {
        get { return (bool)Session["DiceLotteryRun"]; }
        set { Session["DiceLotteryRun"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.RollDiceLotteryEnabled);

        InitControls();

        if (!Page.IsPostBack)
            IsLotteryRunning = false;
    }

    private void InitControls()
    {
        User = Member.CurrentInCache;
       
        ErrorMessagePanel.Visible = false;
        SuccMessagePanel.Visible = false;

        //Game View
        EnterGameButton.Text = U6010.ENTERTHEGAME;
        ResultsButton.Text = L1.HISTORY;
        GameButton.Text = U6010.ROLLDICE;
        RollDiceButton.Text = U6010.ROLLDICE;
        SubmitScoreButton.Text = L1.SUBMIT;
        CurrentResultsGridViewTitleLiteral.Text = U6010.CURRENTRESULTS;
        CurrentResultsGridView.EmptyDataText = L1.NODATA;

        var prizesText = new StringBuilder();
        prizesText.Append(string.Format("{0}:", L1.PRIZES));

        if (RollDiceLotteryManager.MainBalancePrize > Money.Zero)
            prizesText.Append(string.Format(" {0} - {1},", L1.MAINBALANCE, RollDiceLotteryManager.MainBalancePrize));
        if (RollDiceLotteryManager.AdBalancePrize > Money.Zero)
            prizesText.Append(string.Format(" {0} - {1},", U6012.PURCHASEBALANCE, RollDiceLotteryManager.AdBalancePrize));
        if (AppSettings.Points.PointsEnabled && RollDiceLotteryManager.PointsPrize > 0)
            prizesText.Append(string.Format(" {0} - {1}", AppSettings.PointsName, RollDiceLotteryManager.PointsPrize));
        if (prizesText[prizesText.Length - 1] == ',')
            prizesText.Length--;

        PrizesDescriptionLiteral.Text = prizesText.ToString();

        //History View
        var viewCount = RollDiceLotteryManager.LastResultsCount;
        if (viewCount == 0)
            ResultsButton.Visible = false;
        else if (viewCount == 1)
        {
            HistoryTwoPlaceHolder.Visible = false;
            HistoryThreePlaceHolder.Visible = false;
        }
        else if(viewCount == 2)
            HistoryThreePlaceHolder.Visible = false;

        HistoryOneLiteral.Text = AppSettings.ServerTime.Date.AddDays(-1).ToShortDateString();
        HistoryTwoLiteral.Text = AppSettings.ServerTime.Date.AddDays(-2).ToShortDateString();
        HistoryThreeLiteral.Text = AppSettings.ServerTime.Date.AddDays(-3).ToShortDateString();
    }

    private void UpdateRollControls()
    {
        if (RollDiceManager.LastResult != null)
        {
            var lastResult = RollDiceManager.LastResult;
            diceOneImg.ImageUrl = GetDiceImgUrl(lastResult[0]);
            diceTwoImg.ImageUrl = GetDiceImgUrl(lastResult[1]);
            diceThreeImg.ImageUrl = GetDiceImgUrl(lastResult[2]);
        }
        else        
            diceOneImg.ImageUrl = diceTwoImg.ImageUrl = diceThreeImg.ImageUrl = GetDiceImgUrl(1);        

        ScoreLiteral.Text = string.Format("{0}: <b>{1}</b>", U6010.SCORE, RollDiceManager.Score.ToString());
        RollsLiteral.Text = string.Format("{0}: <b>{1}</b>", U6010.ROLLSNUMBER, RollDiceManager.RollsCount.ToString());
    }

    private string GetDiceImgUrl(int number)
    {
        return string.Format("~/Images/Misc/dice{0}.svg", number);
    }

    protected void MenuButton_Click(object sender, EventArgs e)
    {
        var TheButton = (Button)sender;
        var viewIndex = Int32.Parse(TheButton.CommandArgument);

        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
            b.CssClass = "";

        TheButton.CssClass = "ViewSelected";
    }

    protected void EnterGameButton_Click(object sender, EventArgs e)
    {
        if (IsLotteryRunning)        
            Response.Redirect("rolldicelottery.aspx");        

        ErrorMessagePanel.Visible = false;
        ErrorMessage.Text = "";

        try
        {
            var price = RollDiceLotteryManager.GetParticipatePrice;
            var note = U6010.ROLLDICEPARTICIPATE;

            if (User.PurchaseBalance < price)
                throw new MsgException(L1.NOTENOUGHFUNDS);

            User.SubtractFromPurchaseBalance(price, note);
            User.SaveBalances();

            RollDiceManager = new RollDiceLotteryManager(User.Id);
            EnterTheGamePlaceHolder.Visible = false;
            GamePlaceHolder.Visible = true;
            CurrentResultsPlaceHolder.Visible = false;
            RollDiceButton.Visible = true;
            UpdateRollControls();
            TimeLiteral.Text = string.Format("{0}: {1}", U4200.TIMELEFT, RollDiceLotteryManager.GetGameTime.ToString());
            IsLotteryRunning = true;
        }
        catch (MsgException ex)
        {
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            throw ex;
        }
    }

    protected void RollDiceButton_Click(object sender, EventArgs e)
    {        
        CheckTime();
        if (RollDiceManager.IsActive)
        {
            RollDiceManager.Roll();
            UpdateRollControls();
        }
    }

    protected void SubmitScoreButton_Click(object sender, EventArgs e)
    {
        try
        {
            RollDiceManager.SendResult();

            GamePlaceHolder.Visible = false;
            EnterTheGamePlaceHolder.Visible = true;
            CurrentResultsPlaceHolder.Visible = true;
            IsLotteryRunning = false;

            CurrentResultsGridView.DataBind();

            if (RollDiceManager.Score == 0)
            {
                ErrorMessagePanel.Visible = true;
                ErrorMessage.Text = U6010.RESULTNOTRECORDER;
            }
            else
            {
                SuccMessagePanel.Visible = true;
                SuccMessage.Text = U6010.RESULTRECORDER;
            }
        }
        catch(MsgException ex)
        {
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = ex.Message;
        }

    }

    protected void DiceTimer_Tick(object sender, EventArgs e)
    {
        CheckTime();
    }

    private void CheckTime()
    {
        if (!RollDiceManager.IsActive)
        {
            RollDiceButton.Visible = false;
            DiceTimer.Tick -= DiceTimer_Tick;
        }
        else
        {
            RollDiceManager.CheckTime();
            TimeLiteral.Text = string.Format("{0}: {1}", U4200.TIMELEFT, (RollDiceLotteryManager.GetGameTime - RollDiceManager.Time).ToString());
        }
    }

    protected void ResultsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var userId = Convert.ToInt32(e.Row.Cells[0].Text);
            e.Row.Cells[0].Text = new Member(userId).Name;

            var timeText = new StringBuilder();
            var timeInSeconds = Convert.ToInt32(e.Row.Cells[3].Text);
            var timeInMinutes = timeInSeconds / 60;

            timeInSeconds %= 60;
            timeText.Append(string.Format("{0}:", timeInMinutes));
            if (timeInSeconds == 0)
                timeText.Append("00");
            else if (timeInSeconds < 10)
                timeText.Append(string.Format("0{0}", timeInSeconds));
            else
                timeText.Append(timeInSeconds);

            e.Row.Cells[3].Text = timeText.ToString();
        }
    }

    protected void CurrentResultsGridViewSqlDataSource_Init(object sender, EventArgs e)
    {
        CurrentResultsGridViewSqlDataSource.SelectCommand = string.Format("SELECT * FROM RollDiceLotteryParticipants WHERE [StatusInt] = {0} ORDER BY [Score] DESC, [NumberOfRolls] ASC, [GameTime] ASC",
            (int)ParticipantStatus.Active);
    }

    protected void HistoryOneGridViewSqlDataSource_Init(object sender, EventArgs e)
    {
        var date = AppSettings.ServerTime.Date.AddDays(-1);
        var date2 = AppSettings.ServerTime.Date;
        HistoryOneGridViewSqlDataSource.SelectCommand = string.Format("SELECT * FROM RollDiceLotteryParticipants WHERE [StatusInt] = {0} AND [DateOccured] > '{1}' AND [DateOccured] < '{2}' ORDER BY [Score] DESC, [NumberOfRolls] ASC, [GameTime] ASC",
            (int)ParticipantStatus.Recorded, date, date2);
    }

    protected void HistoryTwoGridViewSqlDataSource_Init(object sender, EventArgs e)
    {
        var date = AppSettings.ServerTime.Date.AddDays(-2);
        var date2 = AppSettings.ServerTime.Date.AddDays(-1);
        HistoryTwoGridViewSqlDataSource.SelectCommand = string.Format("SELECT * FROM RollDiceLotteryParticipants WHERE [StatusInt] = {0} AND [DateOccured] > '{1}' AND [DateOccured] < '{2}' ORDER BY [Score] DESC, [NumberOfRolls] ASC, [GameTime] ASC",
            (int)ParticipantStatus.Recorded, date, date2);
    }

    protected void HistoryThreeGridViewSqlDataSource_Init(object sender, EventArgs e)
    {
        var date = AppSettings.ServerTime.Date.AddDays(-3);
        var date2 = AppSettings.ServerTime.Date.AddDays(-2);
        HistoryThreeGridViewSqlDataSource.SelectCommand = string.Format("SELECT * FROM RollDiceLotteryParticipants WHERE [StatusInt] = {0} AND [DateOccured] > '{1}' AND [DateOccured] < '{2}' ORDER BY [Score] DESC, [NumberOfRolls] ASC, [GameTime] ASC",
            (int)ParticipantStatus.Recorded, date, date2);
    }    
}