using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using MarchewkaOne.Titan.Balances;
using Titan;

//HOTKEYS (for chrome, ie, opera use alt + key; firefox - alt + shift + key):
//x = x2, c = /2, m = M, l = bet low, h = bet high
public partial class About : System.Web.UI.Page
{
    public Member user;
    Money siteInvestment;
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.MoneyDiceGameEnabled);
        user = Member.Current;

        if (!IsPostBack)
        {
            GetStatsData();
            btnBetLow.Text = U4200.LO;
            btnBetHigh.Text = U4200.HI;
            btnInvest.Text = U4200.INVEST;
            btnInvestAll.Text = U4200.INVESTALL;
            btnDivest.Text = U4200.DIVEST;
            btnDivestAll.Text = U4200.DIVESTALL;

            LangAdder.Add(MyBetsButton, U4200.MYBETS);
            LangAdder.Add(AllBetsButton, U4200.ALLBETS);
            LangAdder.Add(RandomizeButton, U4200.RANDOMIZE);
            LangAdder.Add(InvestButton, U4200.INVEST);
            LangAdder.Add(StatsButton, L1.STATISTICS);
            MyBetsGridView.EmptyDataText = L1.NODATA;
            AllBetsGridView.EmptyDataText = L1.NODATA;
            InvestmentsGridView.EmptyDataText = L1.NODATA;
            investTextBox.Attributes.Add("title", U4200.MAXDIGITSINFO);
            divestTextBox.Attributes.Add("title", U4200.MAXDIGITSINFO);
            kellyInvestTextBox.Attributes.Add("title", U4200.KELLYINFO + "<br />" + U4200.MAXKELLYINFO + ": " + AppSettings.DiceGame.MaxKellyLevelInt);
            kellyDivestTextBox.Attributes.Add("title", U4200.DIVESTKELLYINFO);
        }
        siteInvestment = SiteInvestmentManager.GetCurrentBankroll();
        sitesBankrollLabel.Text = siteInvestment.ToClearString();
        maxProfitLabel.Text = Money.MultiplyPercent(siteInvestment, AppSettings.DiceGame.MaxBitCoinProfitPercent).ToClearString();
        maxChanceLabel.Text = AppSettings.DiceGame.MaxChance.ToString() + "%";
        adBalanceLabel.Text = Member.Current.PurchaseBalance.ToClearString();

        ScriptManager.RegisterStartupScript(GamePanel, GetType(), "isBettingEnabled", "isBettingEnabled();", true);
        ScriptManager.RegisterStartupScript(MultiViewUpdatePanel, GetType(), "isBettingEnabled", "isBettingEnabled();", true); 
        ScriptManager.RegisterStartupScript(MultiViewUpdatePanel, GetType(), "setConfirmations", "setConfirmations();", true);
    }

    private void GetStatsData()
    {
        var userWins = DiceGameManager.GetTotalWinsLosses(isWin: true, userId: user.Id);
        var userLosses = DiceGameManager.GetTotalWinsLosses(isWin: false, userId: user.Id);
        var userBets = userWins + userLosses;
        UsersBetsLiteral.Text = userBets.ToString();

        var siteWins = DiceGameManager.GetTotalWinsLosses(isWin: true);
        var siteLosses = DiceGameManager.GetTotalWinsLosses(isWin: false);
        var siteBets = siteWins + siteLosses;
        SitesBetsLiteral.Text = siteBets.ToString();

        UsersWageredLiteral.Text = DiceGameManager.GetTotalWagered(user.Id).ToString();
        SitesWageredLiteral.Text = DiceGameManager.GetTotalWagered().ToString();
        UsersWinsLiteral.Text = userWins.ToString();
        SitesWinsLiteral.Text = siteWins.ToString();
        UsersLossesLiteral.Text = userLosses.ToString();
        SitesLossesLiteral.Text = siteLosses.ToString();
        UsersProfitLiteral.Text = DiceGameManager.GetTotalProfit(user.Id).ToString();
        SitesProfitLiteral.Text = DiceGameManager.GetTotalProfit().ToString();
    }
    #region BetGridViews
    protected void AllBetsGridViewSqlDataSource_Init(object sender, EventArgs e)
    {
        AllBetsGridViewSqlDataSource.SelectCommand = "SELECT TOP 40 ub.BetDate, ub.Chance, ub.Id, ub.UserId, ub.BetSize, ub.Low, ub.Roll, ub.Profit, us.UserName " +
                                                     "FROM UserBets ub " +
                                                     "INNER JOIN Users us ON ub.UserId = us.UserId " +
                                                     "ORDER BY BetDate DESC";
    }
    protected void UpdateTimer_Tick(object sender, EventArgs e)
    {
        AllBetsGridView.DataBind();
    }
    #endregion

    #region MyBetsGridView
    protected void MyBetsGridViewSqlDataSource_Init(object sender, EventArgs e)
    {
        MyBetsGridViewSqlDataSource.SelectCommand = "SELECT TOP 40 ub.BetDate, ub.Chance, ub.Id, ub.UserId, ub.BetSize, ub.Low, ub.Roll, ub.Profit, us.UserName " +
                                                    "FROM UserBets ub " +
                                                    "INNER JOIN Users us ON ub.UserId = us.UserId " +
                                                    "WHERE ub.UserId = " + Member.CurrentId + " ORDER BY BetDate DESC";

    }

    protected void MyBetsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            decimal houseEdge = AppSettings.DiceGame.HouseEdgePercent;
            decimal chance = Convert.ToDecimal(e.Row.Cells[5].Text);
            decimal multiplier = (100 - houseEdge) / chance;

            if (e.Row.Cells[7].Text.Contains("-"))
                e.Row.Cells[7].ForeColor = System.Drawing.Color.DarkRed;
            else
            {
                e.Row.Cells[7].Text = "+" + e.Row.Cells[7].Text;
                e.Row.Cells[7].ForeColor = System.Drawing.Color.Green;
            }
            if (e.Row.Cells[8].Text.ToLower() == "true")
                e.Row.Cells[5].Text = "< " + e.Row.Cells[4].Text;
            else
                e.Row.Cells[5].Text = "> " + (100 - Convert.ToDecimal(e.Row.Cells[4].Text)).ToString();
            e.Row.Cells[4].Text = multiplier.ToString("N8");
        }

    }
    #endregion
    public void btnBet_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        ErrorMessage.Text = "";

        try
        {
            AppSettings.Reload();
            var TheButton = (Button)sender;

            Money maxProfit = Money.MultiplyPercent(siteInvestment, AppSettings.DiceGame.MaxBitCoinProfitPercent);
            Money minBet = AppSettings.DiceGame.MinBitCoinBet;
            decimal maxChance = AppSettings.DiceGame.MaxChance;

            decimal formChance = Convert.ToDecimal(chanceTextBox.Text);
            Money formBetAmount =Money.Parse(betAmountTextBox.Text);
            int houseEdge = AppSettings.DiceGame.HouseEdgePercent;
            Money formProfit = Money.Parse(profitTextBox.Text);
            bool low = Convert.ToBoolean(TheButton.CommandArgument);

            DiceGameManager.TryToBet(maxProfit, minBet, formChance, formBetAmount, houseEdge, formProfit, low);

            MyBetsGridView.DataBind();
            siteInvestment = SiteInvestmentManager.GetCurrentBankroll();
            sitesBankrollLabel.Text = siteInvestment.ToClearString();
            maxProfitLabel.Text = Money.MultiplyPercent(siteInvestment, AppSettings.DiceGame.MaxBitCoinProfitPercent).ToClearString();
            adBalanceLabel.Text = Member.Current.PurchaseBalance.ToClearString();
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
    public void MenuButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";

        if (viewIndex == 3)
        {
            //enable betting
            currentWindow.Text = "3";
            InvestmentsGridView.DataBind();

        }
        else if (viewIndex == 2)
        {
            //Disable betting
            currentWindow.Text = "2";

            DiceGameHash CurrentDiceGameHash = DiceGameHash.Get(user);
            CurrentDiceGameHash.ArchiveServerSeedAndHash();
            string serverSeedPrevious = CurrentDiceGameHash.ServerSeedPrevious;
            string serverHashPrevious = CurrentDiceGameHash.ServerHashPrevious;
            string clientSeedPrevious = CurrentDiceGameHash.ClientSeedCurrent;
            string numberOfBets = DiceGameHashLogic.GetNumberOfBets(user.Id).ToString();
            CurrentDiceGameHash.GenerateServerSeedAndHash();
            CurrentDiceGameHash.Save();
            string serverHashCurrent = CurrentDiceGameHash.ServerHashCurrent;

            LastServerSeedLabel.Text = serverSeedPrevious;
            LastServerSeedHashLabel.Text = serverHashPrevious;
            LastClientSeedLabel.Text = clientSeedPrevious;
            NumberOfRollsLabel.Text = numberOfBets;
            NewServerSeedHashLabel.Text = serverHashCurrent;
            NewClientSeedTextBox.Text = DiceGameHashLogic.GenerateClientSeed();

        }
        else if (viewIndex == 1)
            //enable betting
            currentWindow.Text = "1";

        else if (viewIndex == 0)
            //enable betting
            currentWindow.Text = "0";
        else if (viewIndex == 4)
            //disable betting
            currentWindow.Text = "4";

    }

    public void btnRandomize_Click(object sender, EventArgs e)
    {
        try
        {
            string digitsOnly = NewClientSeedTextBox.Text;

            if (digitsOnly.Length != 24)
                throw new MsgException(U4200.CLIENTSEEDNOTVALID1);

            foreach (char c in digitsOnly)
            {
                if (!char.IsDigit(c))
                    throw new MsgException(U4200.CLIENTSEEDNOTVALID2);
            }

            DiceGameHash CurrentDiceGameHash = DiceGameHash.Get(user);
            CurrentDiceGameHash.UpdateClientSeed(NewClientSeedTextBox.Text);
            CurrentDiceGameHash.Save();

            Response.Redirect("dicegame.aspx");
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

    public void btnInvest_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        try
        {
            var TheButton = (Button)sender;
            int buttonArgument = Int32.Parse(TheButton.CommandArgument);
            Money adBalance = Member.Current.PurchaseBalance;
            int kelly;

            if (buttonArgument == 0)
            {
                Money amountToInvest = Money.Parse(investTextBox.Text);
                if (string.IsNullOrWhiteSpace(kellyInvestTextBox.Text))
                    throw new MsgException(U4200.KELLYERROR + ": " + AppSettings.DiceGame.MaxKellyLevelInt);
                kelly = Convert.ToInt32(kellyInvestTextBox.Text);
                SiteInvestmentManager.TryInvest(amountToInvest, kelly, user);
            }
            else if (buttonArgument == 1)
            {
                Money amountToInvest = adBalance;
                if (string.IsNullOrWhiteSpace(kellyInvestTextBox.Text))
                    throw new MsgException(U4200.KELLYERROR + ": " + AppSettings.DiceGame.MaxKellyLevelInt);
                kelly = Convert.ToInt32(kellyInvestTextBox.Text);
                SiteInvestmentManager.TryInvest(amountToInvest, kelly, user);
            }
            else if (buttonArgument == 2)
                
            {
                if (string.IsNullOrWhiteSpace(kellyDivestTextBox.Text))
                    throw new MsgException(U4200.KELLYERROR + ": " + AppSettings.DiceGame.MaxKellyLevelInt);
                Money amountToDivest = Money.Parse(divestTextBox.Text);
                kelly = Convert.ToInt32(kellyDivestTextBox.Text);

                SiteInvestmentManager.TryDivest(amountToDivest, kelly, user);
            }
            else if (buttonArgument == 3)
            {
                SiteInvestmentManager.TryDivestAll(user);
            }

            InvestmentsGridView.DataBind();
            siteInvestment = SiteInvestmentManager.GetCurrentBankroll();
            sitesBankrollLabel.Text = siteInvestment.ToClearString();
            maxProfitLabel.Text = Money.MultiplyPercent(siteInvestment, AppSettings.DiceGame.MaxBitCoinProfitPercent).ToClearString();
            adBalanceLabel.Text = Member.Current.PurchaseBalance.ToClearString();

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

    protected void InvestmentsGridViewSqlDataSource_Init(object sender, EventArgs e)
    {
        InvestmentsGridViewSqlDataSource.SelectCommand = "SELECT Amount, KellyInt FROM SiteInvestments si " +
                                                    "INNER JOIN Users us ON us.UserId = si.UserId " +
                                                    "WHERE si.UserId = " + Member.CurrentId +
                                                    " ORDER BY OperationDate DESC";
    }

    protected void StatsView_Activate(object sender, EventArgs e)
    {
        GetStatsData();
    }
}
