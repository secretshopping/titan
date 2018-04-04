using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Utils;
using Resources;
using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Titan.Balances;
using Titan.Cryptocurrencies;
using Titan.CryptocurrencyPlatform;

public partial class user_btctrading_Sell : System.Web.UI.Page
{
    #region Control specifics
    public decimal MinCryptocurrencyAmount
    {
        set { _MinCryptocurrencyAmount = value; }

        get { return _MinCryptocurrencyAmount; }
    }

    public decimal MaxCryptocurrencyAmount
    {
        set { _MaxCryptocurrencyAmount = value; }

        get { return _MaxCryptocurrencyAmount; }
    }

    public decimal PricePerCryptocurrency
    {
        set { _PricePerCryptocurrency = value; }

        get { return _PricePerCryptocurrency; }
    }

    public decimal NumericUpDownStep
    {
        get
        {
            decimal DefaultStep = Convert.ToDecimal(0.1);
            if (_MaxCryptocurrencyAmount > DefaultStep)
                return DefaultStep;
            else
                return _MaxCryptocurrencyAmount;
        }       
    }

    private int SelectedOfferId
    {
        set { _SelectedOfferId = value; }

        get { return _SelectedOfferId; }
    }

    public String Description
    {
        set { _Description = value; }

        get { return _Description; }
    }

    public String CryptocurrencyAmount
    {
        set { _CryptocurrencyAmount = value; }

        get { return _CryptocurrencyAmount; }
    }

    private decimal _MinCryptocurrencyAmount, _MaxCryptocurrencyAmount, _PricePerCryptocurrency;
    private String _Description, _CryptocurrencyAmount;
    private int _SelectedOfferId;

    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {
        CryptocurrencyAmount = String.Empty;

        #region TabAimer
        int SelectedTab = (Request.Params["SelectedTab"] != null) ? Int32.Parse(Request.Params["SelectedTab"]) : -1;
        if (SelectedTab >= 0)
        {
            MenuMultiView.ActiveViewIndex = SelectedTab;
            int Counter = MenuButtonPlaceHolder.Controls.Count - 1;
            foreach (Button b in MenuButtonPlaceHolder.Controls)
            {
                if (Counter == SelectedTab)
                    b.CssClass = "ViewSelected";
                else
                    b.CssClass = "";
                Counter--;
            }
        }
        #endregion

        #region OpenDetailsInfoView
        if ((!IsPostBack) && Request.Params.Get("oid") != null) 
        {
            int ProductId = Convert.ToInt32(Request.Params.Get("oid"));

            CryptocurrencyTradeOffer SelectedOffer = new CryptocurrencyTradeOffer(ProductId);
            MenuMultiView.ActiveViewIndex = 4;
            SelectedOfferId = SelectedOffer.Id;

            //Counting min-max currency amount limits to sell
            CryptocurrencyMoney CountedMinCurrency, CountedMaxCurrency;
            CountedMinCurrency = CryptocurrencyMoney.Parse((SelectedOffer.MinOfferValue / SelectedOffer.MaxPrice).ToClearString(), CryptocurrencyType.BTC);
            CountedMaxCurrency = CryptocurrencyMoney.Parse((SelectedOffer.MaxOfferValue / SelectedOffer.MaxPrice).ToClearString(), CryptocurrencyType.BTC);
            
            if (SelectedOffer.AmountLeft < CountedMaxCurrency)
                CountedMaxCurrency = SelectedOffer.AmountLeft;

            //Creator info
            Member User = new Member(SelectedOffer.CreatorId);
            CreatorNameLabel.Text = String.Format("<span style=\"float:left; margin-right: 10px\">{0}</span>{1}",
                                                    HtmlCreator.CreateAvatarPlusUsername(User),
                                                    CryptocurrencyPlatformManager.GetHtmlRatingStringForUser(User.Id));

            //Other info
            MinOfferLabel.Text = SelectedOffer.MinOfferValue.ToString();
            MaxOfferLabel.Text = SelectedOffer.MaxOfferValue.ToString();
            CurrencyAvailableToBuyLabel.Text = SelectedOffer.AmountLeft.ToString();

            MinCurrencyLabel.Text = CountedMinCurrency.ToString();
            MaxCurrencyLabel.Text = CountedMaxCurrency.ToString();
            MinCryptocurrencyAmount = decimal.Parse(CountedMinCurrency.ToClearString());
            MaxCryptocurrencyAmount = decimal.Parse(CountedMaxCurrency.ToClearString());

            PricePerCurrencyLabel.Text = SelectedOffer.MaxPrice.ToClearString();
            PricePerCryptocurrency = decimal.Parse(SelectedOffer.MaxPrice.ToClearString());

            AmountToSellTextBox.Text = CountedMinCurrency.ToClearString();

            InfoForBuyerForOnClickSellTextBox.Text = String.Empty;
            Description = SelectedOffer.Description;

            //Updating Total Price label after every change of Cryptocurrency Amount
            AmountToSellTextBox.Attributes.Add("onkeyup", "updatePrice();");

            CashToPayLabel.Text = String.Format("{0}", decimal.Round(PricePerCryptocurrency * MinCryptocurrencyAmount, CoreSettings.GetMaxDecimalPlaces()).ToString());

        }
        #endregion

        #region OpenAddCommentView
        if ((!IsPostBack) && Request.Params.Get("foid") != null)
        {
            MenuMultiView.ActiveViewIndex = 5;
        }
        #endregion

        if (!IsPostBack)
            AddLang();

        //Constraint DataBind for second ManageTab load
        if (IsPostBack && MenuMultiView.ActiveViewIndex == 2)
            ManageTabDataBind();

        //Constraint DataBind for second HistoryTab load
        if (IsPostBack && MenuMultiView.ActiveViewIndex == 3)
            OfferHistoryGridView.DataBind();
    }
    private void AddLang()
    {
        MenuButtonFirstTab.Text = U6010.ACTIVEOFFERS;
        MenuButtonSecondTab.Text = L1.ADDNEW;
        MenuButtonThirdTab.Text = L1.MANAGE;
        MenuButtonFourthTab.Text = L1.HISTORY;
        AddSellOfferButton.Text = L1.ADDNEW;
        AddCommentButton.Text = U6010.ADDCOMMENT;
        SellOfferButton.Text = U6009.SELL;
        BackButton.Text = U6010.BACK;

        AllBuyOffersGridView.EmptyDataText = L1.NODATA;
        CurrentUserSellOfferGridView.EmptyDataText = L1.NODATA;
        OfferHistoryGridView.EmptyDataText = L1.NODATA;
        CurrentUserTransactionsGridView.EmptyDataText = U6010.NOOFFERS;
        OfferHistoryGridView.EmptyDataText = L1.NODATA;

        Amount_RangeValidator.ErrorMessage = U6010.BADRANGEORFORMAT;
        Amount_RequiredFieldValidator.ErrorMessage = L1.DETAILEDALLREQ;
        Price_RangeValidator.ErrorMessage = U6010.BADRANGEORFORMAT;
        Price_RequiredFieldValidator.ErrorMessage = L1.DETAILEDALLREQ;
        MinTransaction_RangeValidator.ErrorMessage = U6010.BADRANGEORFORMAT;
        MinTransaction_RequiredFieldValidator.ErrorMessage = L1.DETAILEDALLREQ;
        MaxTransaction_RangeValidator.ErrorMessage = U6010.BADRANGEORFORMAT;
        MaxTransaction_RequiredFieldValidator.ErrorMessage = L1.DETAILEDALLREQ;
        Escrow_RangeValidator1.ErrorMessage = U6010.BADRANGEORFORMAT;
        Escrow_RangeValidator2.ErrorMessage = U6010.BADESCROWRANGE;
        Escrow_RequiredFieldValidator.ErrorMessage = L1.DETAILEDALLREQ;
        Decsription_RequiredFieldValidator.ErrorMessage = L1.DETAILEDALLREQ;
        AmountToSell_RequiredFieldValidator.ErrorMessage = L1.DETAILEDALLREQ;
        AmountToSell_RangeValidator.ErrorMessage = U6010.BADRANGEORFORMAT;
        InfoForBuyerForOnClickSellTextBox_RequiredFieldValidator.ErrorMessage = L1.DETAILEDALLREQ;
    }

    #region Views Activation
    protected void BuyOffersView_Activate(object sender, EventArgs e)
    {
        AllBuyOffersGridView.DataBind();
    }

    protected void CreateSellOfferView_Activate(object sender, EventArgs e)
    {
        SellAmountTextBox.Text    = decimal.Zero.ToString();
        MinPriceTextBox.Text      = decimal.Zero.ToString();
        MinOfferValueTextBox.Text = decimal.Zero.ToString();
        MaxOfferValueTextBox.Text = decimal.Zero.ToString();
        EscrowDescriptionLiteral.Text = String.Format("<img src=\"Images/Misc/question_small.png\" title=\"{0}\"/>", U6010.ESCROWINFORMATOR);
        EscrowTimeTextBox.Text = "30";
    }

    protected void OfferManagementView_Activate(object sender, EventArgs e)
    {
        ManageTabDataBind();
    }

    protected void OfferHistoryView_Activate(object sender, EventArgs e)
    {
        OfferHistoryGridView.DataBind();
    }

    private void ManageTabDataBind()
    {
        CurrentUserTransactionsGridView.DataBind();

        if (CurrentUserTransactionsGridView.Rows.Count != 0)
            CurrentUserTransactionsPlaceHolder.Visible = true;

        CurrentUserSellOfferGridView.DataBind();
    }
    #endregion

    #region All Buy Offer GridView
    protected void AllBuyOffersGridView_DataSource_Init(object sender, EventArgs e)
    {
        AllBuyOffersGridView_DataSource.SelectCommand = CryptocurrencyTradeOffer.GetGridViewStringForAllActiveOffersForUser(Member.CurrentId, CryptocurrencyOfferType.Buy);
    }

    protected void AllBuyOffersGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var CurrentOffer = new CryptocurrencyTradeOffer(Int32.Parse(e.Row.Cells[0].Text));

            Member User = new Member(Int32.Parse(e.Row.Cells[1].Text));
            e.Row.Cells[1].Text = String.Format("<span style=\"float:left; margin-right: 10px\">{0}</span>{1}", 
                                    HtmlCreator.CreateAvatarPlusUsername(User),
                                    CryptocurrencyPlatformManager.GetHtmlRatingStringForUser(User.Id));

            e.Row.Cells[3].Text = String.Format("<b>{0}</b>/{1}", Money.Parse(e.Row.Cells[3].Text).ToString(), AppSettings.CryptocurrencyTrading.CryptocurrencyCode);
            e.Row.Cells[4].Text = String.Format("{0} - {1}", CurrentOffer.MinOfferValue, CurrentOffer.MaxOfferValue);
            e.Row.Cells[5].Text = CryptocurrencyMoney.Parse(e.Row.Cells[5].Text).ToString();
        }
    }

    protected void AllBuyOffersGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        var commands = new[] { "sell" };

        if (commands.Contains(e.CommandName))
        {
            var index = e.GetSelectedRowIndex() % AllBuyOffersGridView.PageSize;
            var row = AllBuyOffersGridView.Rows[index];
            var urlId = (row.Cells[0].Text.Trim());

            switch (e.CommandName)
            {
                case "sell":
                    HttpContext.Current.Response.Redirect("~/user/cctrading/sell.aspx?oid=" + urlId);
                    break;
            }
        }
    }
    #endregion

    #region User Sell Offers Management
    //Active offers
    protected void CurrentUserSellOfferGridView_DataSource_Init(object sender, EventArgs e)
    {
        CurrentUserSellOfferGridView_DataSource.SelectCommand = CryptocurrencyTradeOffer.GetGridViewStringForUserActualOffers(Member.CurrentId, CryptocurrencyOfferType.Sell);
    }

    protected void CurrentUserSellOfferGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {   
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[1].Text = L1.DATEADDED;
            e.Row.Cells[2].Text = L1.PRICE;
            e.Row.Cells[3].Text = U6010.TRANSACTIONVALUE;
            e.Row.Cells[4].Text = U6010.AMOUNTTOSELL;
            e.Row.Cells[5].Text = U6010.AMOUNTLEFT;
            e.Row.Cells[6].Text = L1.STATUS;
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var CurrentOffer = new CryptocurrencyTradeOffer(Int32.Parse(e.Row.Cells[0].Text));

            CryptocurrencyOfferStatus Status = (CryptocurrencyOfferStatus)Convert.ToInt32(e.Row.Cells[6].Text);

            e.Row.Cells[2].Text = String.Format("{0} / {1}", Money.Parse(e.Row.Cells[2].Text).ToString(), AppSettings.CryptocurrencyTrading.CryptocurrencyCode);
            e.Row.Cells[3].Text = String.Format("{0} - {1}", CurrentOffer.MinOfferValue, CurrentOffer.MaxOfferValue);
            e.Row.Cells[4].Text = CryptocurrencyMoney.Parse(e.Row.Cells[4].Text).ToString();
            e.Row.Cells[5].Text = CryptocurrencyMoney.Parse(e.Row.Cells[5].Text).ToString();
            e.Row.Cells[6].Text = HtmlCreator.GetColoredStatus(Status);

            if (Status != CryptocurrencyOfferStatus.Paused)
                e.Row.Cells[7].Text = "&nbsp;";

            if (Status != CryptocurrencyOfferStatus.Active)
                e.Row.Cells[8].Text = "&nbsp;";
        }
    }

    protected void CurrentUserSellOfferGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        var commands = new[] { "start", "stop", "remove" };

        if (commands.Contains(e.CommandName))
        {
            var index = e.GetSelectedRowIndex() % CurrentUserSellOfferGridView.PageSize;
            var row = CurrentUserSellOfferGridView.Rows[index];
            var OfferId = (row.Cells[0].Text.Trim());
            var Offer = new CryptocurrencyTradeOffer(Convert.ToInt32(OfferId));

            switch (e.CommandName)
            {
                case "start":
                    if (Offer.Status == CryptocurrencyOfferStatus.Paused)
                        Offer.Activate();
                    break;
                case "stop":
                    if (Offer.Status == CryptocurrencyOfferStatus.Active)
                        Offer.Pause();
                    break;
                case "remove":
                    Offer.Delete();
                    break;
            }

            CurrentUserSellOfferGridView.DataBind();
        }
    }

    //Actual transactions
    protected void CurrentUserTransactionsGridView_DataSource_Init(object sender, EventArgs e)
    {
        CurrentUserTransactionsGridView_DataSource.SelectCommand = CryptocurrencyTradeTransaction.GetGridViewStringForUserActualTransactions(Member.CurrentId, CryptocurrencyOfferType.Sell);
    }

    protected void CurrentUserTransactionsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[1].Text = L1.AMOUNT;
            e.Row.Cells[2].Text = U6010.EXECUTIONTIME;
            e.Row.Cells[3].Text = L1.STATUS;
            e.Row.Cells[5].Text = U4200.TIMELEFT;
            e.Row.Cells[6].Text = L1.DESCRIPTION;
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //Loading data for addiotional info
            var CurrentTransaction = new CryptocurrencyTradeTransaction(Int32.Parse(e.Row.Cells[0].Text));
            var CurrentOffer = new CryptocurrencyTradeOffer(CurrentTransaction.OfferId);

            e.Row.Cells[1].Text = CryptocurrencyMoney.Parse(e.Row.Cells[1].Text).ToString();

            if ((CryptocurrencyTransactionStatus)Int32.Parse(e.Row.Cells[3].Text) == CryptocurrencyTransactionStatus.AwaitingPaymentConfirmation)
                e.Row.Cells[6].Visible = true;
            else
                e.Row.Cells[6].Visible = false;
            e.Row.Cells[3].Text = HtmlCreator.GetColoredTransactionStatus((CryptocurrencyTransactionStatus)Int32.Parse(e.Row.Cells[3].Text));

            //Count Time Left in Escrow
            DateTime ExecutionTime = DateTime.Parse(e.Row.Cells[2].Text);
            TimeSpan TimeLeft = AppSettings.ServerTime - ExecutionTime;
            int TimeLeftMinutes = CurrentOffer.EscrowTime - (int)TimeLeft.TotalMinutes;
            if (TimeLeftMinutes < 0)
                TimeLeftMinutes = 0;

            Label OfferTimeLeftToPayTextBox = new Label();
            OfferTimeLeftToPayTextBox.Text = HtmlCreator.GetColoredTime(TimeLeftMinutes);
            e.Row.Cells[5].Controls.Add(OfferTimeLeftToPayTextBox);

            //Load Description for this offerAddToCryptocurrencyBalance
            Label OfferDescriptionTextBox = new Label();

            //If seller description have data, that means that it is 100% buy offer and tehre is no desc, we have to load seller's description
            if(String.IsNullOrEmpty(CurrentTransaction.SellerDescription) || String.IsNullOrWhiteSpace(CurrentTransaction.SellerDescription))
                OfferDescriptionTextBox.Text = CurrentOffer.Description;
            else
                OfferDescriptionTextBox.Text = CurrentTransaction.SellerDescription;

            OfferDescriptionTextBox.CssClass = "description-column displaynone";
            e.Row.Cells[4].Controls.Add(OfferDescriptionTextBox);
        }
    }

    protected void CurrentUserTransactionsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        var commands = new[] { "ConfirmReceived" };

        if (commands.Contains(e.CommandName))
        {
            var index = e.GetSelectedRowIndex() % CurrentUserTransactionsGridView.PageSize;
            var row = CurrentUserTransactionsGridView.Rows[index];
            var TransactionId = (row.Cells[0].Text.Trim());
            var Transaction = new CryptocurrencyTradeTransaction(Convert.ToInt32(TransactionId));
            var CurrentTradeOffer = new CryptocurrencyTradeOffer(Transaction.OfferId);

            switch (e.CommandName)
            {
                case "ConfirmReceived":
                    int ClientWithCash = -1;
                    if (Transaction.ClientId != Member.CurrentId)
                        ClientWithCash = Transaction.ClientId;
                    else
                        ClientWithCash = CurrentTradeOffer.CreatorId;

                    var BuyerWithCash = new Member(ClientWithCash);
                    BuyerWithCash.AddToCryptocurrencyBalance(CryptocurrencyType.BTC, Transaction.CCAmount.ToDecimal(), "Cryptocurrency trade", BalanceLogType.CryptocurrencyTrade);

                    Transaction.PaymentStatus = CryptocurrencyTransactionStatus.Finished;
                    Transaction.Save();

                    CryptocurrencyFinishedTradeOffer.CreateNewTemplate(Transaction.OfferId, ClientWithCash, Member.CurrentId, Transaction.CCAmount);
                    break;
            }

            CurrentUserTransactionsGridView.DataBind();
        }
    }
    #endregion

    #region User History GridView

    protected void OfferHistoryGridView_DataSource_Init(object sender, EventArgs e)
    {
        OfferHistoryGridView_DataSource.SelectCommand = CryptocurrencyFinishedTradeOffer.GetGridViewStringForUserHistory(Member.CurrentId, CryptocurrencyOfferType.Sell);
    }

    protected void OfferHistoryGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[1].Text = U6010.SOLDTO;
            e.Row.Cells[2].Text = U6010.CRYPTOCURRENCY;
            e.Row.Cells[7].Text = U6010.COMMENT_YOUR;
            e.Row.Cells[8].Text = U6010.COMMENT_CLIENT;
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Member User = new Member(Int32.Parse(e.Row.Cells[1].Text));
            e.Row.Cells[1].Text = HtmlCreator.CreateAvatarPlusUsername(User);
            e.Row.Cells[2].Text = String.Format("-{0} {1}", double.Parse(e.Row.Cells[2].Text).ToString(), AppSettings.CryptocurrencyTrading.CryptocurrencySign);
            e.Row.Cells[2].Style.Add("color", "red");
            e.Row.Cells[3].CssClass = "your-comment-column displaynone";
            e.Row.Cells[4].CssClass = "comment-column displaynone";

            if (!String.IsNullOrEmpty(e.Row.Cells[3].Text) && !(e.Row.Cells[3].Text == "&nbsp;"))
                e.Row.Cells[6].Text = String.Empty;
            else
                e.Row.Cells[3].Text = U6010.NOCOMMENTYET;

            if (String.IsNullOrEmpty(e.Row.Cells[4].Text) || (e.Row.Cells[4].Text == "&nbsp;"))
                e.Row.Cells[4].Text = U6010.CLIENTNOCOMMENT;
        }
    }

    protected void OfferHistoryGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        var commands = new[] { "AddComment" };

        if (commands.Contains(e.CommandName))
        {
            var index = e.GetSelectedRowIndex() % CurrentUserTransactionsGridView.PageSize;
            var row = OfferHistoryGridView.Rows[index];

            var FinishedTransactionId = (row.Cells[0].Text.Trim());

            switch (e.CommandName)
            {
                case "AddComment":
                    HttpContext.Current.Response.Redirect("~/user/cctrading/sell.aspx?foid=" + FinishedTransactionId);
                    break;
            }

            CurrentUserTransactionsGridView.DataBind();
        }
    }

    #endregion

    #region OnClicks
    public void MenuButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        SuccessMessagePanel.Visible = false;

        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        if(MenuMultiView.ActiveViewIndex != viewIndex)
        {
            MenuMultiView.ActiveViewIndex = viewIndex;

            foreach (Button b in MenuButtonPlaceHolder.Controls)
            {
                b.CssClass = "";
            }
            TheButton.CssClass = "ViewSelected";
        }

    }

    protected void AddSellOfferButton_Click(object sender, EventArgs e)
    {
        try
        { 
            if (String.IsNullOrEmpty(SellAmountTextBox.Text) || decimal.Parse(SellAmountTextBox.Text) == decimal.Zero)
                throw new MsgException(String.Format("{0}{1}{2}", U6010.AMOUNTTOBUY, U6010.NOZERO, "<br />"));
            if (decimal.Parse(MinPriceTextBox.Text) == decimal.Zero)
                throw new MsgException(String.Format("{0} {1}{2}{3}", U6010.PRICEPER, AppSettings.CryptocurrencyTrading.CryptocurrencyCode, U6010.NOZERO, "<br />"));
            if (decimal.Parse(MaxOfferValueTextBox.Text) == decimal.Zero)
                throw new MsgException(String.Format("{0}{1}{2}", U6010.CCMAXOFFERVALUE, U6010.NOZERO, "<br />"));
            if (decimal.Parse(MinOfferValueTextBox.Text) > decimal.Parse(MaxOfferValueTextBox.Text))
                throw new MsgException(String.Format("{0} {1}{2}{3}", U6010.CCMINOFFERVALUE, U6010.CANTBEHIGHER, U6010.CCMAXOFFERVALUE, "<br />"));

            var User = Member.CurrentInCache;
            var UserBalance = User.GetCryptocurrencyBalance(CryptocurrencyType.BTC);

            if (CryptocurrencyMoney.Parse(SellAmountTextBox.Text) > UserBalance)
                throw new MsgException(String.Format(U6010.NOCREDITS_CRYPTOCURRENCYBALANCE, UserBalance));

            CryptocurrencyTradeOffer.CreateNewOffer(CryptocurrencyOfferType.Sell,
                                                        Member.CurrentId,
                                                        Money.Parse(MinPriceTextBox.Text),
                                                        Money.Zero,
                                                        Money.Parse(MinOfferValueTextBox.Text),
                                                        Money.Parse(MaxOfferValueTextBox.Text),
                                                        Int32.Parse(EscrowTimeTextBox.Text),
                                                        Description = DescriptionTextBox.Text,
                                                        CryptocurrencyMoney.Parse(SellAmountTextBox.Text, CryptocurrencyType.BTC));

            Response.Redirect("~/user/cctrading/sell.aspx?SelectedTab=2");

        }
        catch (MsgException ex)
        {
            CreateSellErrorPanel.Visible = true;
            CreateSellErrorLiteral.Text = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex.Message);
            throw ex;
        }
    }

    protected void SellOfferButton_OnClick(object sender, EventArgs e)
    {
        try
        {
            if (Request.Params.Get("oid") != null)
            {
                var SelectedOffer = new CryptocurrencyTradeOffer(Convert.ToInt32(Request.Params.Get("oid")));

                //Because when creating buy offer you don't create description, seller have to provide necessary info for buyer
                String AdditionalDescription = InfoForBuyerForOnClickSellTextBox.Text;

                if (CryptocurrencyMoney.Parse(AmountToSellTextBox.Text) == CryptocurrencyMoney.Zero)
                    throw new MsgException(U6011.CANTSELLZEROCRYPTOCURRENCY);

                //Check if seller don't try sell more than expected
                if ( (CryptocurrencyMoney.Parse(AmountToSellTextBox.Text) > SelectedOffer.AmountLeft) || 
                     (CryptocurrencyMoney.Parse(AmountToSellTextBox.Text) * SelectedOffer.MaxPrice > SelectedOffer.MaxOfferValue) )
                    throw new MsgException(U6010.ERRORTRYSELLMORE);

                if (CryptocurrencyMoney.Parse(AmountToSellTextBox.Text) * SelectedOffer.MaxPrice < SelectedOffer.MinOfferValue)
                    throw new MsgException(U6010.ERRORTRYSELLLESS);

                CryptocurrencyPlatformManager.TryPlaceOrder(SelectedOffer.Id, CryptocurrencyMoney.Parse(AmountToSellTextBox.Text, CryptocurrencyType.BTC), AdditionalDescription);

                Response.Redirect("~/user/cctrading/sell.aspx?SelectedTab=2");
            }
        }
        catch (MsgException ex)
        {
            SellErrorPanel.Visible = true;
            SellError.Text = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex.Message);
            throw ex;
        }
    }

    protected void BackToActiveOffers_OnClick(object sender, EventArgs e)
    {
        Response.Redirect("~/user/cctrading/sell.aspx");
    }
    
    protected void AddCommentButton_OnClick(object sender, EventArgs e)
    {
        if (Request.Params.Get("foid") != null)
        {
            int FinishedTransactionId = Convert.ToInt32(Request.Params.Get("foid"));
            var FinishedTransaction = new CryptocurrencyFinishedTradeOffer(Convert.ToInt32(FinishedTransactionId));

            FinishedTransaction.SellerComment = AddCommentTextBox.Text;
            FinishedTransaction.Save();

            Response.Redirect("~/user/cctrading/sell.aspx?SelectedTab=3");
        }

    }

    #endregion

}