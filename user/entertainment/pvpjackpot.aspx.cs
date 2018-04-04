using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Utils;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class user_entertainment_pvpjackpot : System.Web.UI.Page
{
    private static int maxTime, randTime, stageToPlayId;
    private static DateTime gameStart;

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.JackpotPvpEnabled);
        LangAdder();

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

        if(MenuMultiView.ActiveViewIndex == 0 && TimeLeftTimer.Enabled)
        {
            TimeLeftTimer.Enabled = false;
            UserStagesPlaceHolder.Visible = true;
            SearchOpponentPlaceHolder.Visible = false;
        }


        this.DataBind();
        UserStagesPlaceHolder.Visible = true;
        BattleResultPlaceHolder.Visible = false;
        SuccessPanel.Visible = false;
        ErrorPanel.Visible = false;
    }

    private void LangAdder()
    {
        PvpJackpotsStagesGridView.EmptyDataText = L1.NODATA;
        UserStagesGridView.EmptyDataText = U6011.NOSTAGESTOPLAY;

        BuyStage.Text = U6011.BUYSTAGE;
        PlayGame.Text = U6011.PLAYGAME;

        GoBackToStagesButton.Text = U6010.BACK;
    }

    #region StagesGridView
    protected void PvpJackpotsStagesDataSource_Init(object sender, EventArgs e)
    {
        PvpJackpotsStagesDataSource.SelectCommand = String.Format(@"SELECT * FROM JackpotPvpStages WHERE Status={0}", (int)UniversalStatus.Active);
    }

    protected void PvpJackpotsStages_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[1].Style.Add("font-weight", "bold");

            Money Cost = Money.Parse(e.Row.Cells[2].Text);
            decimal Percent = Convert.ToDecimal(e.Row.Cells[4].Text);

            e.Row.Cells[2].Text = Cost.ToString();
            e.Row.Cells[4].Text += String.Format("% = {0}", (Cost/JackpotPvpManager.BattlesAmountPerStage)*(Percent/100));

            Literal battlesAmount = (Literal)e.Row.FindControl("BattlesAmountLiteral");
            battlesAmount.Text = JackpotPvpManager.BattlesAmountPerStage.ToString();
        }
    }

    protected void PvpJackpotsStages_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName != "Sort" && e.CommandName != "Page")
        {
            int rowIndex = e.GetSelectedRowIndex() % PvpJackpotsStagesGridView.PageSize;
            int JackpotPvpStageId = (int)PvpJackpotsStagesGridView.DataKeys[rowIndex].Value;

            switch (e.CommandName)
            {
                case "buy":
                    try
                    {
                        JackpotPvpManager.AddStageForUser(Member.Current, JackpotPvpStageId);
                        SuccessMessage.Text = L1.OP_SUCCESS;
                        SuccessPanel.Visible = true;
                    }
                    catch (MsgException ex)
                    {
                        ErrorPanel.Visible = true;
                        ErrorMessage.Text = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.Log(ex.Message);
                        throw ex;
                    }

                    break;

                default:
                    break;
            }
        }
    }
    #endregion


    #region Current User Stages GridView
    protected void UserStagesGridViewDataSource_Init(object sender, EventArgs e)
    {
        UserStagesGridViewDataSource.SelectCommand = String.Format(@"SELECT * FROM JackpotPvpStagesBought WHERE UserId={0} AND Active=1", Member.CurrentId);
    }

    protected void UserStagesGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[1].Text = new JackpotPvpStage(Convert.ToInt32(e.Row.Cells[1].Text)).Name;
            e.Row.Cells[1].Style.Add("font-weight", "bold");

            e.Row.Cells[2].Text = String.Format("{0}/{1}", JackpotPvpManager.BattlesAmountPerStage - Convert.ToInt32(e.Row.Cells[2].Text), JackpotPvpManager.BattlesAmountPerStage); 
        }
    }

    protected void UserStagesGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName != "Sort" && e.CommandName != "Page")
        {
            int rowIndex = e.GetSelectedRowIndex() % PvpJackpotsStagesGridView.PageSize;
            int selectedStageToPlayId = (int)UserStagesGridView.DataKeys[rowIndex].Value;
            var BoughtStage = new JackpotPvpStageBought(selectedStageToPlayId);

            switch (e.CommandName)
            {
                case "play":
                    ErrorPanel.Visible = false;
                    SuccessPanel.Visible = false;

                    try
                    {
                        stageToPlayId = BoughtStage.StageId;
                        JackpotPvpManager.TryCheckSystemPoolsCash(stageToPlayId);

                        Random Rnd = new Random();
                        maxTime = AppSettings.Addons.PvpJackpotOpponentSearchTime;
                        randTime = Rnd.Next(5, AppSettings.Addons.PvpJackpotOpponentSearchTime);


                        gameStart = AppSettings.ServerTime;

                        UserStagesPlaceHolder.Visible = false;
                        SearchOpponentPlaceHolder.Visible = true;
                        TimeLeftTimer.Enabled = true;

                        TimeLiteral.Text = string.Format("{0}: {1} {2}", U4200.TIMELEFT, maxTime.ToString(), L1.SECONDS.ToLower());
                    }
                    catch (MsgException ex)
                    {
                        ErrorPanel.Visible = true;
                        ErrorMessage.Text = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.Log(ex.Message);
                    }

                    break;

                default:
                    break;
            }
        }
    }
    #endregion

    protected void TimeLeftTimer_Tick(object sender, EventArgs e)
    {
        CheckTime();
    }

    private void CheckTime()
    {
        if (!AppSettings.TitanFeatures.JackpotPvpEnabled)
        {
            SearchOpponentPlaceHolder.Visible = false;
            TimeLeftTimer.Tick -= TimeLeftTimer_Tick;
        }
        else
        {
            TimeSpan totalTime = AppSettings.ServerTime - gameStart;
            TimeLiteral.Text = string.Format("{0}: {1} {2}", U4200.TIMELEFT, (maxTime - totalTime.Seconds) > 0 ? (maxTime-totalTime.Seconds).ToString() : "0", L1.SECONDS.ToLower());

            if (totalTime.Seconds > randTime)
            {
                TimeLeftTimer.Enabled = false;

                UserStagesPlaceHolder.Visible     = false;
                SearchOpponentPlaceHolder.Visible = false;
                ErrorPanel.Visible                = false;
                SuccessPanel.Visible              = false;

                try
                {
                    bool UserWon = JackpotPvpManager.PlayBattleWithBot(Member.CurrentId, stageToPlayId);

                    BattleResultPlaceHolder.Visible = true;
                    if (UserWon)
                    {
                        WhoWonLiteral.Text = U6011.BATTLEWON;
                        SmilePlaceHolder.Visible = true;
                        SadPlaceHolder.Visible = false;
                    }
                    else
                    {
                        WhoWonLiteral.Text = U6011.BATTLELOST;
                        SmilePlaceHolder.Visible = false;
                        SadPlaceHolder.Visible = true;
                    }
                }
                catch (MsgException ex)
                {
                    ErrorPanel.Visible = true;
                    ErrorMessage.Text = ex.Message;
                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex.Message);
                }

                return;
            }
        }
    }

    protected void GoBackToStagesButton_Click(object sender, EventArgs e)
    {
        BattleResultPlaceHolder.Visible = false;
        UserStagesPlaceHolder.Visible = true;
        this.DataBind();
    }
}