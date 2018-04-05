using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Utils;
using Resources;
using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Titan.CryptocurrencyPlatform;
using Titan.Cryptocurrencies;

public partial class user_btctrading_Buy : System.Web.UI.Page
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
        
        #region TabAimer
        int SelectedTab = (Request.Params["SelectedTab"] != null) ? Int32.Parse(Request.Params["SelectedTab"]) : -1;
        if(SelectedTab >= 0)
        {
            MenuMultiView.ActiveViewIndex = SelectedTab;
            int Counter = MenuButtonPlaceHolder.Controls.Count-1;
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

        #region OpenOfferDetails
        if ((!IsPostBack) && Request.Params.Get("oid") != null) 
        {
            try
            {
                MenuMultiView.ActiveViewIndex = 4;

                CryptocurrencyAmount = String.Empty;
                int ProductId = Convert.ToInt32(Request.Params.Get("oid"));

                CryptocurrencyTradeOffer SelectedOffer = new CryptocurrencyTradeOffer(ProductId);
                               
                var OfferCreator = new Member(SelectedOffer.CreatorId);
                var OfferCreatorBalance = OfferCreator.GetCryptocurrencyBalance(CryptocurrencyType.BTC);

                //If creator of offer don't have enaugh currency in balance 
                if (OfferCreatorBalance < SelectedOffer.AmountLeft)
                {
                    //If Offer AmountLeft is bigger than creator's balance, set max limit of buy to creator's balacne value
                    SelectedOffer.AmountLeft = OfferCreatorBalance;

                    if (OfferCreatorBalance == CryptocurrencyMoney.Zero)
                    {
                        SelectedOffer.Status = CryptocurrencyOfferStatus.Paused;
                        SelectedOffer.Save();
                        throw new MsgException(U6010.NOCREATORBALANCETOBUYOFFER);
                    }
                    SelectedOffer.Save();
                }

                SelectedOfferId = SelectedOffer.Id;

                decimal CountedMinCurrency, CountedMaxCurrency;

                CountedMinCurrency = decimal.Parse((SelectedOffer.MinOfferValue / SelectedOffer.MinPrice).ToClearString());
                CountedMaxCurrency = decimal.Parse((SelectedOffer.MaxOfferValue / SelectedOffer.MinPrice).ToClearString());
                if (SelectedOffer.AmountLeft < CryptocurrencyMoney.Parse(CountedMaxCurrency.ToString()))
                    CountedMaxCurrency = decimal.Parse(SelectedOffer.AmountLeft.ToClearString());


                Member User = new Member(SelectedOffer.CreatorId);
                CreatorNameLabel.Text = String.Format("<span style=\"float:left; margin-right: 10px\">{0}</span>{1}",
                                                    HtmlCreator.CreateAvatarPlusUsername(User),
                                                    CryptocurrencyPlatformManager.GetHtmlRatingStringForUser(User.Id));

                MinOfferLabel.Text = SelectedOffer.MinOfferValue.ToString();
                MaxOfferLabel.Text = SelectedOffer.MaxOfferValue.ToString();
                CurrencyAvailableToBuyLabel.Text = SelectedOffer.AmountLeft.ToString();

                MinCryptocurrencyAmount = CountedMinCurrency;
                MaxCryptocurrencyAmount = CountedMaxCurrency;
                MinCurrencyLabel.Text = CryptocurrencyMoney.Parse(CountedMinCurrency.ToString()).ToString();
                MaxCurrencyLabel.Text = CryptocurrencyMoney.Parse(CountedMaxCurrency.ToString()).ToString();


                PricePerCurrencyLabel.Text = SelectedOffer.MinPrice.ToString();
                PricePerCryptocurrency = decimal.Parse(SelectedOffer.MinPrice.ToClearString());

                AmountToBuyTextBox.Text = CountedMinCurrency.ToString();

                //Updating Total Price label after every change of Cryptocurrency Amount
                AmountToBuyTextBox.Attributes.Add("onkeyup", "updatePrice();");

                CashToPayLabel.Text = decimal.Round(PricePerCryptocurrency * MinCryptocurrencyAmount, CoreSettings.GetMaxDecimalPlaces()).ToString();

                testerTB.Text = SelectedOffer.Description;
                Description = SelectedOffer.Description;
                BuyOfferButton.Text = L1.BUY;
            }
            catch (MsgException ex)
            {
                AllOffersErrorPanel.Visible = true;
                AllOffersErrorLiteral.Text = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex.Message);
                throw ex;
            }
        }
        #endregion

        #region OpenAddCommentView
        if ((!IsPostBack) && Request.Params.Get("foid") != null)
        {
            MenuMultiView.ActiveViewIndex = 5;
        }
        #endregion

        if (!IsPostBack)
        {
            AddLang();
        }

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
        AddBuyOfferButton.Text = L1.ADDNEW;
        AddCommentButton.Text = U6010.ADDCOMMENT;
        BackButton.Text = U6010.BACK;

        AllSellOffersGridView.EmptyDataText = L1.NODATA;
        CurrentUserTransactionsGridView.EmptyDataText = L1.NODATA;
        CurrentUserBuyOfferGridView.EmptyDataText = L1.NODATA;
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
        AmountToBuy_RequiredFieldValidator.ErrorMessage = L1.DETAILEDALLREQ;
        AmountToBuy_RangeValidator.ErrorMessage = U6010.BADRANGEORFORMAT;
    }


    #region Views Activation
    protected void SellOffersView_Activate(object sender, EventArgs e)
    {
        AllSellOffersGridView.DataBind();
    }

    protected void CreateBuyOfferView_Activate(object sender, EventArgs e)
    {
        BuyAmountTextBox.Text     = decimal.Zero.ToString();
        MaxPriceTextBox.Text      = decimal.Zero.ToString();
        MinOfferValueTextBox.Text = decimal.Zero.ToString();
        MaxOfferValueTextBox.Text = decimal.Zero.ToString();
        EscrowDescriptionLiteral.Text = String.Format("<img src=\"Images/Misc/question_small.png\" title=\"{0}\"/>", U6010.ESCROWINFORMATOR );
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

        CurrentUserBuyOfferGridView.DataBind();
    }

    #endregion

    #region All Sell Offers GridView
    protected void AllSellOffersGridView_DataSource_Init(object sender, EventArgs e)
    {
        AllSellOffersGridView_DataSource.SelectCommand = CryptocurrencyTradeOffer.GetGridViewStringForAllActiveOffersForUser(Member.CurrentId, CryptocurrencyOfferType.Sell);
    }

    protected void AllSellOffersGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var CurrentOffer = new CryptocurrencyTradeOffer(Int32.Parse(e.Row.Cells[0].Text));
            Member User = new Member(Int32.Parse(e.Row.Cells[1].Text));

            e.Row.Cells[1].Text = String.Format("<span style=\"float:left; margin-right: 10px\">{0}</span>{1}", 
                                                    HtmlCreator.CreateAvatarPlusUsername(User),
                                                    CryptocurrencyPlatformManager.GetHtmlRatingStringForUser(User.Id));

            e.Row.Cells[3].Text = String.Format("<b>{0}</b>/{1}", Money.Parse(e.Row.Cells[3].Text).ToString(), AppSettings.CryptocurrencyTrading.CryptocurrencyCode);
            e.Row.Cells[4].Text = CurrentOffer.MinOfferValue + " - " + CurrentOffer.MaxOfferValue;
            e.Row.Cells[5].Text = CryptocurrencyMoney.Parse(e.Row.Cells[5].Text).ToString();
        }
    }

    protected void AllSellOffersGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        var commands = new[] { "buy" };

        if (commands.Contains(e.CommandName))
        {
            var index = e.GetSelectedRowIndex() % AllSellOffersGridView.PageSize;
            var row = AllSellOffersGridView.Rows[index];
            var urlId = (row.Cells[0].Text.Trim());

            switch (e.CommandName)
            {
                case "buy":
                    HttpContext.Current.Response.Redirect("~/user/cctrading/buy.aspx?oid=" + urlId);
                    break;

            } 
        }
    }
    #endregion

    #region User Buy Offers Management
    //Active offers
    protected void CurrentUserBuyOfferGridView_DataSource_Init(object sender, EventArgs e)
    {
        CurrentUserBuyOfferGridView_DataSource.SelectCommand = CryptocurrencyTradeOffer.GetGridViewStringForUserActualOffers(Member.CurrentId, CryptocurrencyOfferType.Buy);
    }

    protected void CurrentUserBuyOfferGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[1].Text = L1.DATEADDED;
            e.Row.Cells[2].Text = L1.PRICE;
            e.Row.Cells[3].Text = U6010.TRANSACTIONVALUE;
            e.Row.Cells[4].Text = U6010.AMOUNTTOBUY;
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

    protected void CurrentUserBuyOfferGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        var commands = new[] { "start", "stop", "remove" };

        if (commands.Contains(e.CommandName))
        {
            var index = e.GetSelectedRowIndex() % CurrentUserBuyOfferGridView.PageSize;
            var row = CurrentUserBuyOfferGridView.Rows[index];
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

            CurrentUserBuyOfferGridView.DataBind();
        }
    }

    //Actual transactions
    protected void CurrentUserTransactionsGridView_DataSource_Init(object sender, EventArgs e)
    {
        CurrentUserTransactionsGridView_DataSource.SelectCommand = CryptocurrencyTradeTransaction.GetGridViewStringForUserActualTransactions(Member.CurrentId, CryptocurrencyOfferType.Buy);
    }

    protected void CurrentUserTransactionsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[1].Text = L1.AMOUNT;
            e.Row.Cells[2].Text = U6010.EXECUTIONTIME;
            e.Row.Cells[3].Text = L1.STATUS;
            e.Row.Cells[4].Text = L1.DESCRIPTION;
            e.Row.Cells[5].Text = U4200.TIMELEFT;
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //Loading data for addiotional info
            var CurrentTransaction = new CryptocurrencyTradeTransaction(Int32.Parse(e.Row.Cells[0].Text));
            var CurrentOffer = new CryptocurrencyTradeOffer(CurrentTransaction.OfferId);

            e.Row.Cells[1].Text = CryptocurrencyMoney.Parse(e.Row.Cells[1].Text).ToString();

            //Buttons management
            if ((CryptocurrencyTransactionStatus)Int32.Parse(e.Row.Cells[3].Text) == CryptocurrencyTransactionStatus.AwaitingPayment)
                e.Row.Cells[6].Visible = true;
            else
                e.Row.Cells[6].Visible = false;            

            //Count Time Left in Escrow
            DateTime ExecutionTime = DateTime.Parse(e.Row.Cells[2].Text);
            TimeSpan TimeLeft = AppSettings.ServerTime - ExecutionTime;
            int TimeLeftMinutes = CurrentOffer.EscrowTime - (int)TimeLeft.TotalMinutes;

            if (TimeLeftMinutes < 0)
            {
                TimeLeftMinutes = 0;
                if ((CryptocurrencyTransactionStatus)CurrentTransaction.PaymentStatus == CryptocurrencyTransactionStatus.AwaitingPayment)
                    CurrentTransaction.PaymentStatus = CryptocurrencyTransactionStatus.NotPaid;
                else if((CryptocurrencyTransactionStatus)CurrentTransaction.PaymentStatus == CryptocurrencyTransactionStatus.AwaitingPaymentConfirmation)
                    CurrentTransaction.PaymentStatus = CryptocurrencyTransactionStatus.PaymentNotConfirmed;
                CurrentTransaction.Save();

                e.Row.Cells[6].Visible = false;
            }
            e.Row.Cells[3].Text = HtmlCreator.GetColoredTransactionStatus((CryptocurrencyTransactionStatus)Int32.Parse(e.Row.Cells[3].Text));

            Label OfferTimeLeftToPayTextBox = new Label();
                OfferTimeLeftToPayTextBox.Text = HtmlCreator.GetColoredTime(TimeLeftMinutes);
            e.Row.Cells[5].Controls.Add(OfferTimeLeftToPayTextBox);

            //Load Description for this offer
            Label OfferDescriptionLabel = new Label();

            //If seller description have data, that means that it is 100% buy offer and tehre is no desc, we have to load seller's description
            if (String.IsNullOrEmpty(CurrentTransaction.SellerDescription) || String.IsNullOrWhiteSpace(CurrentTransaction.SellerDescription))
                OfferDescriptionLabel.Text = CurrentOffer.Description;
            else
                OfferDescriptionLabel.Text = CurrentTransaction.SellerDescription;

            OfferDescriptionLabel.CssClass = "description-column displaynone";
            e.Row.Cells[4].Controls.Add(OfferDescriptionLabel);
        }
    }

    protected void CurrentUserTransactionsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        var commands = new[] { "Confirm", "ConfirmReceived" };

        if (commands.Contains(e.CommandName))
        {
            var index = e.GetSelectedRowIndex() % CurrentUserTransactionsGridView.PageSize;
            var row = CurrentUserTransactionsGridView.Rows[index];
            var TransactionId = (row.Cells[0].Text.Trim());
            var Transaction = new CryptocurrencyTradeTransaction(Convert.ToInt32(TransactionId));

            switch (e.CommandName)
            {
                case "Confirm":
                    Transaction.PaymentStatus = CryptocurrencyTransactionStatus.AwaitingPaymentConfirmation;
                    Transaction.Save();
                    break;
                case "ConfirmReceived":
                    break;
            }

            CurrentUserTransactionsGridView.DataBind();
        }
    }
    
    #endregion

    #region User History GridView
    protected void OfferHistoryGridView_DataSource_Init(object sender, EventArgs e)
    {
        OfferHistoryGridView_DataSource.SelectCommand = CryptocurrencyFinishedTradeOffer.GetGridViewStringForUserHistory(Member.CurrentId, CryptocurrencyOfferType.Buy);
    }

    protected void OfferHistoryGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[1].Text = U6010.BOUGHTFROM;
            e.Row.Cells[2].Text = U6010.CRYPTOCURRENCY;
            e.Row.Cells[5].Text = U6010.RATING;
            e.Row.Cells[7].Text = U6010.COMMENT_YOUR;
            e.Row.Cells[8].Text = U6010.COMMENT_CLIENT;
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Member User = new Member(Int32.Parse(e.Row.Cells[1].Text));
            e.Row.Cells[1].Text = HtmlCreator.CreateAvatarPlusUsername(User);
            e.Row.Cells[2].Text = String.Format("+{0} {1}", double.Parse(e.Row.Cells[2].Text).ToString(), AppSettings.CryptocurrencyTrading.CryptocurrencySign); 
            e.Row.Cells[2].Style.Add("color", "green");
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
                    HttpContext.Current.Response.Redirect("~/user/cctrading/buy.aspx?foid=" + FinishedTransactionId);
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

        MenuMultiView.ActiveViewIndex = viewIndex;

        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";
    }

    protected void AddBuyOfferButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (decimal.Parse(BuyAmountTextBox.Text) == decimal.Zero)
                throw new MsgException(String.Format("{0}{1}{2}", U6010.AMOUNTTOBUY, U6010.NOZERO, "<br />"));
            if (decimal.Parse(MaxPriceTextBox.Text) == decimal.Zero)
                throw new MsgException(String.Format("{0} {1}{2}{3}", U6010.PRICEPER, AppSettings.CryptocurrencyTrading.CryptocurrencyCode, U6010.NOZERO, "<br />"));
            if (decimal.Parse(MaxOfferValueTextBox.Text) == decimal.Zero)
                throw new MsgException(String.Format("{0}{1}{2}", U6010.CCMAXOFFERVALUE, U6010.NOZERO, "<br />"));
            if (decimal.Parse(MinOfferValueTextBox.Text) > decimal.Parse(MaxOfferValueTextBox.Text))
                throw new MsgException(String.Format("{0} {1}{2}{3}", U6010.CCMINOFFERVALUE, U6010.CANTBEHIGHER, U6010.CCMAXOFFERVALUE, "<br />"));

            CryptocurrencyTradeOffer.CreateNewOffer(
                        CryptocurrencyOfferType.Buy,
                        Member.CurrentId,
                        Money.Zero,
                        Money.Parse(MaxPriceTextBox.Text),
                        Money.Parse(MinOfferValueTextBox.Text),
                        Money.Parse(MaxOfferValueTextBox.Text),
                        Int32.Parse(EscrowTimeTextBox.Text),
                        Description = String.Empty,
                        CryptocurrencyMoney.Parse(BuyAmountTextBox.Text, CryptocurrencyType.BTC));

            Response.Redirect("~/user/cctrading/buy.aspx?SelectedTab=2");
        }
        catch(MsgException ex)
        {
            CreateBuyErrorPanel.Visible = true;
            CreateBuyErrorLiteral.Text = ex.Message;
        }
        catch(Exception ex)
        {
            ErrorLogger.Log(ex.Message);
            throw ex;
        }
    }

    protected void BuyOfferButton_OnClick(object sender, EventArgs e)
    {
        try
        {
            if (Request.Params.Get("oid") != null)
            {
                int ProductId = Convert.ToInt32(Request.Params.Get("oid"));
                var SelectedOffer = new CryptocurrencyTradeOffer(ProductId);

                if (CryptocurrencyMoney.Parse(AmountToBuyTextBox.Text) == CryptocurrencyMoney.Zero)
                    throw new MsgException(U6011.CANTBUYZEROCRYPTOCURRENCY);

                if ( (CryptocurrencyMoney.Parse(AmountToBuyTextBox.Text) > SelectedOffer.AmountLeft) ||
                     (CryptocurrencyMoney.Parse(AmountToBuyTextBox.Text) * SelectedOffer.MinPrice > SelectedOffer.MaxOfferValue) )
                    throw new MsgException(U6010.ERRORTRYBUYMORE);

                 if (CryptocurrencyMoney.Parse(AmountToBuyTextBox.Text) * SelectedOffer.MinPrice < SelectedOffer.MinOfferValue)
                    throw new MsgException(U6010.ERRORTRYBUYLESS);

                CryptocurrencyPlatformManager.TryPlaceOrder(ProductId, CryptocurrencyMoney.Parse(AmountToBuyTextBox.Text, CryptocurrencyType.BTC), String.Empty);

                Response.Redirect("~/user/cctrading/buy.aspx?SelectedTab=2");
            }
        }
        catch (MsgException ex)
        {
            BuyErrorPanel.Visible = true;
            BuyError.Text = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex.Message);
            throw ex;
        }
    }

    protected void BackToActiveOffers_OnClick(object sender, EventArgs e)
    {
        Response.Redirect("~/user/cctrading/buy.aspx");
    }

    protected void AddCommentButton_OnClick(object sender, EventArgs e)
    {
        if (Request.Params.Get("foid") != null)
        {
            int FinishedTransactionId = Convert.ToInt32(Request.Params.Get("foid"));
            var FinishedTransaction = new CryptocurrencyFinishedTradeOffer(Convert.ToInt32(FinishedTransactionId));

            FinishedTransaction.BuyerComment = AddCommentTextBox.Text;
            FinishedTransaction.Save();

            Response.Redirect("~/user/cctrading/buy.aspx?SelectedTab=3");
        }

    }

    #endregion
}