using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Utils;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Titan.Cryptocurrencies;
using Titan.ICO;
using Titan.InternalExchange;

public partial class user_ico_internalexchange : System.Web.UI.Page
{
    #region Properties
    public static String SignOfStock
    {
        get { return _signOfStock; }
        set { _signOfStock = value; }
    }

    public static String SignOfPurchaseBalance
    {
        get { return _signOfPurchaseBalance; }
        set { _signOfPurchaseBalance = value; }
    }

    public static String ChartTitle
    {
        get { return _chartTitle; }
        set { _chartTitle = value; }
    }

    public static int PurchaseBalanceDecimalPlaces
    {
        get
        {
            if (_purchaseBalanceDecimalPlaces == 0)
                _purchaseBalanceDecimalPlaces = GetPurchaseBalanceDecimalPlaces();
            return _purchaseBalanceDecimalPlaces;
        }
        set { _purchaseBalanceDecimalPlaces = value; }
    }

    private static String _signOfStock, _signOfPurchaseBalance, _chartTitle;
    private static int _purchaseBalanceDecimalPlaces = 0;
    #endregion

    #region Hidden Statistics Data
    private bool FirstIncome = true;
    //LAST STOCK VALUE
    private decimal tmpLastStockValue;
    private decimal tmpCurrentStockValue;

    //LAST ASK VALUE
    private decimal tmpLastAskValue;
    private decimal tmpCurrentLastAskValue;

    //LAST BID VALUE
    private decimal tmpLastBidValue;
    private decimal tmpCurrentLastBidValue;

    //LAST 24H HIGH
    private decimal tmpLast24HighValue;
    private decimal tmpCurrentLast24HighValue;

    //LAST 24H LOW
    private decimal tmpLast24LowValue;
    private decimal tmpCurrentLast24LowValue;

    //LAST 24H VOLUME
    private decimal tmpLast24Volume;
    private decimal tmpCurrentLast24Volume;
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.InternalExchangeEnabled);

        SignOfStock = InternalExchangeManager.GetBalanceSign(true);
        SignOfPurchaseBalance = InternalExchangeManager.GetBalanceSign(false);

        if (!IsPostBack)
        {
            NotVisibleChartData.Text = AppSettings.IsDemo ? InternalExchangeManager.GetTestChartData() : new InternalExchangeCache().Get().ToString();

            HideMessagges();
            FillData();
            LangAdders();

            NumberOfStockTextBox.Attributes.Add("onkeyup", "updatePrice();");
            ValueOfStockTextBox.Attributes.Add("onkeyup", "updatePrice();");
            TotalValueTextBox.Attributes.Add("onkeyup", "updatePriceFromTotal();");

            MinimalPriceLabel.Visible = AppSettings.InternalExchange.ICOInternalExchangeMinimalStockPrice == Decimal.Zero ? false : true;
        }
    }

    private void LangAdders()
    {
        LangAdder.Add(MainTab, U6012.INTERNALEXCHANGE);
        LangAdder.Add(PlaceOrderButton, U6012.PLACEORDER);

        LangAdder.Add(BidButton, U6012.BID);
        LangAdder.Add(AskButton, U6012.ASK);

        LangAdder.Add(MinimalPriceLabel, String.Format("{0} 1{1} = {2}{3}", U6010.MINPRICE, SignOfStock, FormatDecimal(AppSettings.InternalExchange.ICOInternalExchangeMinimalStockPrice), SignOfPurchaseBalance));
        LangAdder.Add(MinmumStockLabel, String.Format("{0} {1}{2}", L1.LIMIT, FormatDecimal(AppSettings.InternalExchange.InternalExchangeMinimalSellAmount), SignOfStock));

        NumberOfStockLabel.Text = String.Format("{0} {1}:", U6013.SIZE, String.Format("({0} {1})", Member.CurrentInCache.GetClearStringBalance(AppSettings.InternalExchange.InternalExchangeStockType), U4200.AVAILABLE.ToLower()));
        PricePerStockLabel.Text = String.Format("{0} {1}:", U6012.BID, String.Format("({0} {1})", Member.CurrentInCache.GetClearStringBalance(AppSettings.InternalExchange.InternalExchangePurchaseVia), U4200.AVAILABLE.ToLower()));
        TotalValueLabel.Text = U6013.TOTALVALUE + ":";


        AllBidOffersGridView.EmptyDataText = L1.NODATA;
        AllAskOffersGridView.EmptyDataText = L1.NODATA;

        Amount_RangeValidator.ErrorMessage = String.Format("{0}", U6010.BADRANGEORFORMAT);
        PricePerStockRangeValidator.ErrorMessage = String.Format("{0}", U6010.BADRANGEORFORMAT);
        TotalValueRangeValidator.ErrorMessage = String.Format("{0}", U6010.BADRANGEORFORMAT);
        Amount_RequiredFieldValidator.ErrorMessage = L1.DETAILEDALLREQ;
        ValueOfStockRequiredFieldValidator.ErrorMessage = L1.DETAILEDALLREQ;
        TotalValueRequiredFieldValidator.ErrorMessage = L1.DETAILEDALLREQ;

        LeftTitleBidOffersLabel.Text = String.Format("{0} {1}", L1.BUY, U3000.OFFERS);
        LeftTitleAskOffersLabel.Text = String.Format("{0} {1}", U6009.SELL, U3000.OFFERS);
    }

    private void FillData()
    {
        BidButton.CssClass = "TabButton IeOfferSelected";
        AskButton.CssClass = "TabButton IeOfferNotSelected";

        if (AppSettings.InternalExchange.ICOInternalExchangeMinimalStockPrice <= Decimal.Zero)
        {
            if (!BalanceTypeHelper.IsCryptoBalance(AppSettings.InternalExchange.InternalExchangePurchaseVia) || AppSettings.InternalExchange.InternalExchangePurchaseVia == BalanceType.BTC)
                PricePerStockRangeValidator.MinimumValue = "0.00000001"; // 1/8
            else
                PricePerStockRangeValidator.MinimumValue = "0.000000000000000001"; // 1/18
        }
        else
            PricePerStockRangeValidator.MinimumValue = AppSettings.InternalExchange.ICOInternalExchangeMinimalStockPrice.ToString();

        Amount_RangeValidator.MinimumValue = AppSettings.InternalExchange.InternalExchangeMinimalSellAmount.ToString("N18");
        Amount_RangeValidator.Text = U6013.STOCKBUYERROR;

        NumberOfStockTextBox.Text = "0";
        ValueOfStockTextBox.Text = "0";
        TotalValueTextBox.Text = "0";

        currentMultiplier.Text = (AppSettings.InternalExchange.InternalExchangeBidCommissionPercent / 100m).ToString();

        UpdateStatistics();
        UpdateGridViewTitles();

        
        if(TitanFeatures.IsTrafficThunder)
            EstimatedValueLabel.Visible = true;

        ChartTitle = TitanFeatures.IsTrafficThunder ? string.Empty : String.Format("{0} ({1})", U6012.VALUEOFSTOCK, SignOfStock);
    }


    #region Actions
    private void HideMessagges()
    {
        SuccessMessagePanel.Visible = false;
        ErrorMessagePanel.Visible = false;
    }

    protected void PlaceOrderButton_Click(object sender, EventArgs e)
    {
        HideMessagges();

        if (Member.CurrentInCache.IsAnyBalanceIsNegative())
        {
            ErrorMessagePanel.Visible = true;
            ErrorMessageLiteral.Text = U6013.YOUCANTPROCEED;
        }
        else
        {
            if (Page.IsValid)
            {
                try
                {
                    bool isAsk = IsCurrentlyAsk();
                    decimal numberOfStockToBuy = Decimal.Parse(NumberOfStockTextBox.Text);
                    decimal valueOfStock = Decimal.Parse(ValueOfStockTextBox.Text);
                    decimal totalValue = Decimal.Parse(TotalValueTextBox.Text);

                    if (valueOfStock < AppSettings.InternalExchange.ICOInternalExchangeMinimalStockPrice)
                        throw new MsgException("Value of stock lower than allowed.");

                    if (numberOfStockToBuy <= Decimal.Zero || valueOfStock <= Decimal.Zero || totalValue <= Decimal.Zero)
                    {
                        ErrorLogger.Log(String.Format("INTERNAL EXCHANGE | Unexpected error | User: {0}({1}) | Size({2}) | Price({3}) | Total({4}) | IsAsk({5})", Member.CurrentName, Member.CurrentId, numberOfStockToBuy, valueOfStock, totalValue, isAsk));
                        throw new MsgException("Unexpected error. Value less than or equals to 0.");
                    }

                    InternalExchangeOfferResponse response = InternalExchangeManager.TryPlaceOrder(Member.CurrentId, numberOfStockToBuy, valueOfStock, isAsk);

                    if (response != null)
                    {
                        SuccessMessageLiteral.Text = GetPlaceOfferResponse(response, isAsk);
                        SuccessMessagePanel.Visible = true;
                    }
                    else
                    {
                        ErrorMessagePanel.Visible = true;
                        ErrorMessageLiteral.Text = L1.ERROR_INFO;
                    }

                    DataBind();
                }
                catch (MsgException ex)
                {
                    ErrorMessagePanel.Visible = true;
                    ErrorMessageLiteral.Text = ex.Message;
                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex.Message);
                    throw ex;
                }
            }
            else
            {
                ErrorMessagePanel.Visible = true;
                ErrorMessageLiteral.Text = "Unexpected Error - page is not valid";
            }
        }
    }

    private string GetPlaceOfferResponse(InternalExchangeOfferResponse response, bool isAsk)
    {
        string message = string.Empty;

        if (response.AmountTransferd != decimal.Zero)
        {
            
            string amountTransferd = InternalExchangeManager.RecognizeCurrencyAndReturnString(true, FormatDecimal(response.AmountTransferd));
            string valueTransferd = InternalExchangeManager.RecognizeCurrencyAndReturnString(false, FormatDecimal(response.ValueTransferd));

            if (isAsk)
                message += String.Format(U6012.SOLDSTOCKS, amountTransferd, valueTransferd);
            else
                message += String.Format(U6012.BOUGHTSTOCKS, amountTransferd, valueTransferd);

            message += "<br />";
        }

        if (response.AmountPlaced != decimal.Zero)
        {
            string amountPlaced = InternalExchangeManager.RecognizeCurrencyAndReturnString(true, FormatDecimal(response.AmountPlaced));
            string valuePlaced = InternalExchangeManager.RecognizeCurrencyAndReturnString(false, FormatDecimal(response.ValuePlaced));

            message += String.Format(U6012.NEWEXCHANGEOFFER, amountPlaced, valuePlaced);
        }

        return message;
    }
    #endregion

    #region GridView Bids
    protected void AllBidOffersGridView_DataSource_Init(object sender, EventArgs e)
    {
        AllBidOffersGridView_DataSource.SelectCommand =
            @"SELECT DISTINCT TOP(50)
                BidValue
                , SUM(BidAmount) AS [Summum]
                , SUM(BidAmount*BidValue) AS [Value]
            FROM InternalExchangeBids
            WHERE BidStatus = 1
            GROUP BY BidValue ORDER BY BidValue DESC";
    }

    protected void AllBidOffersGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[0].Text += String.Format("({0})", InternalExchangeManager.GetBalanceCode(true));
            e.Row.Cells[1].Text += String.Format("({0})", InternalExchangeManager.GetBalanceCode(false));
            e.Row.Cells[2].Text += String.Format("({0})", InternalExchangeManager.GetBalanceCode(false));
        }

        if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
        {
            if (e.Row.RowIndex == 0)
                e.Row.Style.Add("font-weight", "bold");

            e.Row.Cells[0].Style.Add("text-align", "right");
            e.Row.Cells[1].Style.Add("text-align", "right");
            e.Row.Cells[2].Style.Add("text-align", "right");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (BalanceTypeHelper.IsCryptoBalance(AppSettings.InternalExchange.InternalExchangeStockType))
                    e.Row.Cells[0].Text = new CryptocurrencyMoney(CryptocurrencyType.BTC, Decimal.Parse(e.Row.Cells[0].Text)).ToClearString();
                else
                    e.Row.Cells[0].Text = new Money(Decimal.Parse(e.Row.Cells[0].Text)).ToClearString();

                if (BalanceTypeHelper.IsCryptoBalance(AppSettings.InternalExchange.InternalExchangePurchaseVia))
                {
                    e.Row.Cells[2].Text = new CryptocurrencyMoney(CryptocurrencyType.BTC, Decimal.Parse(e.Row.Cells[2].Text)).ToClearString();
                    e.Row.Cells[1].Text = new CryptocurrencyMoney(CryptocurrencyType.BTC, Decimal.Parse(e.Row.Cells[1].Text)).ToClearString();
                }
                else
                {
                    e.Row.Cells[2].Text = new Money(Decimal.Parse(e.Row.Cells[2].Text)).ToClearString();
                    e.Row.Cells[1].Text = new Money(Decimal.Parse(e.Row.Cells[1].Text)).ToClearString();
                }

                e.Row.Cells[0].CssClass = "paramBidAmount";
                e.Row.Cells[1].CssClass = "paramBidValue";
            }
        }
    }
    #endregion

    #region GridView Asks
    protected void AllAskOffersGridView_DataSource_Init(object sender, EventArgs e)
    {
        AllAskOffersGridView_DataSource.SelectCommand =
            @"SELECT DISTINCT TOP(50)
                AskValue
                , SUM(AskAmount) AS [Summum]
                , SUM(AskAmount*AskValue) AS [Value]
            FROM InternalExchangeAsks
            WHERE AskStatus = 1
            GROUP BY AskValue ORDER BY AskValue";
    }

    protected void AllAskOffersGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[0].Text += String.Format("({0})", InternalExchangeManager.GetBalanceCode(true));
            e.Row.Cells[1].Text += String.Format("({0})", InternalExchangeManager.GetBalanceCode(false));
            e.Row.Cells[2].Text += String.Format("({0})", InternalExchangeManager.GetBalanceCode(false));
        }

        if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
        {
            if (e.Row.RowIndex == 0)
                e.Row.Style.Add("font-weight", "bold");

            e.Row.Cells[0].Style.Add("text-align", "right");
            e.Row.Cells[1].Style.Add("text-align", "right");
            e.Row.Cells[2].Style.Add("text-align", "right");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (BalanceTypeHelper.IsCryptoBalance(AppSettings.InternalExchange.InternalExchangeStockType))
                    e.Row.Cells[0].Text = new CryptocurrencyMoney(CryptocurrencyType.BTC, Decimal.Parse(e.Row.Cells[0].Text)).ToClearString();
                else
                    e.Row.Cells[0].Text = new Money(Decimal.Parse(e.Row.Cells[0].Text)).ToClearString();

                if (BalanceTypeHelper.IsCryptoBalance(AppSettings.InternalExchange.InternalExchangePurchaseVia))
                {
                    e.Row.Cells[2].Text = new CryptocurrencyMoney(CryptocurrencyType.BTC, Decimal.Parse(e.Row.Cells[2].Text)).ToClearString();
                    e.Row.Cells[1].Text = new CryptocurrencyMoney(CryptocurrencyType.BTC, Decimal.Parse(e.Row.Cells[1].Text)).ToClearString();
                }
                else
                {
                    e.Row.Cells[2].Text = new Money(Decimal.Parse(e.Row.Cells[2].Text)).ToClearString();
                    e.Row.Cells[1].Text = new Money(Decimal.Parse(e.Row.Cells[1].Text)).ToClearString();
                }

                e.Row.Cells[0].CssClass = "paramAskAmount";
                e.Row.Cells[1].CssClass = "paramAskValue";
            }
        }
    }
    #endregion

    protected void UpdateTimer_Tick(object sender, EventArgs e)
    {
        UpdateStatistics();
        UpdateGridViewTitles();
        AllBidOffersGridView.DataBind();
        AllAskOffersGridView.DataBind();
    }

    private void UpdateStatistics()
    {
        UpdateStatisticsData();

        SetStatisticForLabel(LastStockValue, tmpLastStockValue, tmpCurrentStockValue, String.Format("<b>{0}:</b> {1}", U6012.LAST.ToUpper(), InternalExchangeManager.RecognizeCurrencyAndReturnString(false, tmpLastStockValue)));
        SetStatisticForLabel(LastAskValue, tmpLastAskValue, tmpCurrentLastAskValue, String.Format("<b>{0} {1}:</b> {2}", U6012.LAST.ToUpper(), U6012.ASK.ToUpper(), InternalExchangeManager.RecognizeCurrencyAndReturnString(false, tmpLastStockValue)));
        SetStatisticForLabel(LastBidValue, tmpLastBidValue, tmpCurrentLastBidValue, String.Format("<b>{0} {1}:</b> {2}", U6012.LAST.ToUpper(), U6012.BID.ToUpper(), InternalExchangeManager.RecognizeCurrencyAndReturnString(false, tmpLastBidValue)));
        SetStatisticForLabel(Last24HighValue, tmpLast24HighValue, tmpCurrentLast24HighValue, String.Format("<b>24H {0}:</b> {1}", U6013.HIGH, InternalExchangeManager.RecognizeCurrencyAndReturnString(false, tmpLast24HighValue)));
        SetStatisticForLabel(Last24LowValue, tmpLast24LowValue, tmpCurrentLast24LowValue, String.Format("<b>24H {0}:</b> {1}", U6013.LOW, InternalExchangeManager.RecognizeCurrencyAndReturnString(false, tmpLast24LowValue)));
        SetStatisticForLabel(Last24Volume, tmpLast24Volume, tmpCurrentLast24Volume, String.Format("<b>24H</b> {0} %", Math.Round(tmpLast24Volume, 2)));
    }

    private void UpdateGridViewTitles()
    {
        RightTitleBidOffersLabel.Text = String.Format("{0}: <b>{1}</b>", U5001.TOTAL, InternalExchangeManager.RecognizeCurrencyAndReturnString(true, InternalExchangeTransaction.GetSumOfStockForOffers(true)));
        RightTitleAskOffersLabel.Text = String.Format("{0}: <b>{1}</b>", U5001.TOTAL, InternalExchangeManager.RecognizeCurrencyAndReturnString(false, InternalExchangeTransaction.GetValueOfOffers(false)));
    }

    protected void BidButton_Click(object sender, EventArgs e)
    {
        SellAmountLimitDescriptionTextBox.Visible = false;

        decimal CurrentMultiplier = AppSettings.InternalExchange.InternalExchangeBidCommissionPercent/100m;
        currentMultiplier.Text = CurrentMultiplier.ToString();

        Amount_RangeValidator.MaximumValue = "999999999";
        Amount_RangeValidator.Text = "<br/>" + U6013.STOCKBUYERROR;

        BidButton.CssClass = "TabButton IeOfferSelected";
        AskButton.CssClass = "TabButton IeOfferNotSelected";

        PricePerStockLabel.Text = String.Format("{0} {1}:", U6012.BID, String.Format("({0} {1})", Member.CurrentInCache.GetClearStringBalance(AppSettings.InternalExchange.InternalExchangePurchaseVia), U4200.AVAILABLE.ToLower()));
    }

    protected void AskButton_Click(object sender, EventArgs e)
    {
        decimal CurrentMultiplier = AppSettings.InternalExchange.InternalExchangeAskCommissionPercent / 100m;
        currentMultiplier.Text = CurrentMultiplier.ToString();

        decimal dailySellLimit = AppSettings.InternalExchange.InternalExchangePeriodMaxSellAmount;
        dailySellLimit -= Member.Current.GetInternalExchangeDayAskAmount();

        if (dailySellLimit <= 0)
        {
            dailySellLimit = 0.0m;
            SellAmountLimitDescriptionTextBox.Text = U6013.STOCKSELLLIMITREACHED;
            Amount_RangeValidator.MinimumValue = FormatDecimal(dailySellLimit);
        }
        else
        {
            SellAmountLimitDescriptionTextBox.Text = U6013.STOCKSELLLIMIT + " " + FormatDecimal(dailySellLimit) + InternalExchangeManager.GetBalanceSign(true);
            SellAmountLimitDescriptionTextBox.Visible = true;
        }

        Decimal CurrentStockWallet = Decimal.Parse(Member.CurrentInCache.GetClearStringBalance(AppSettings.InternalExchange.InternalExchangeStockType));

        if (CurrentStockWallet > AppSettings.InternalExchange.InternalExchangeMinimalSellAmount)
            Amount_RangeValidator.MaximumValue = CurrentStockWallet < dailySellLimit ? FormatDecimal(CurrentStockWallet) : FormatDecimal(dailySellLimit);
        else
            Amount_RangeValidator.MaximumValue = FormatDecimal(dailySellLimit);
        Amount_RangeValidator.Text = "<br/>" + U6013.STOCKSELLERROR;

        BidButton.CssClass = "TabButton IeOfferNotSelected";
        AskButton.CssClass = "TabButton IeOfferSelected";

        PricePerStockLabel.Text = String.Format("{0} {1}:", U6012.ASK, String.Format("({0} {1})", Member.CurrentInCache.GetClearStringBalance(AppSettings.InternalExchange.InternalExchangePurchaseVia), U4200.AVAILABLE.ToLower()));
    }

    private bool IsCurrentlyAsk()
    {
        return AskButton.CssClass.Contains("IeOfferSelected");
    }

    private String FormatDecimal(decimal deci)
    {
        return deci.ToString("N18").TrimEnd('0').TrimEnd('.').Replace(",", "");
    }

    #region Statistics

    private void SetStatisticForLabel(Label currentLabel, decimal currentValue, decimal oldValue, String nameOfStat)
    {
        if (currentValue == decimal.Zero)
        {
            currentLabel.Style.Clear();
            currentLabel.CssClass = "";
            currentLabel.Text = nameOfStat;
        }


        if (currentValue > oldValue)
            SetStatisticAsIncrease(currentLabel, nameOfStat);
        else if (currentValue < oldValue)
            SetStatisticAsDecrease(currentLabel, nameOfStat);
        else
        {
            //VALUES ARE THE SAME
            if (currentLabel.CssClass.Equals("INCREASE"))
                SetStatisticAsIncrease(currentLabel, nameOfStat);
            else if (currentLabel.CssClass.Equals("DECREASE"))
                SetStatisticAsDecrease(currentLabel, nameOfStat);

            currentLabel.Style.Clear();
            currentLabel.CssClass = "";
            currentLabel.Text = nameOfStat;
        }
    }

    private void SetStatisticAsIncrease(Label currentLabel, String nameOfStat)
    {
        currentLabel.CssClass = "INCREASE";
        currentLabel.Style.Add("color", "green");
        currentLabel.Text = String.Format("{0} {1}", nameOfStat, "<i class=\"fa fa-arrow-up\" aria-hidden=\"true\"></i>");
    }

    private void SetStatisticAsDecrease(Label currentLabel, String nameOfStat)
    {
        currentLabel.CssClass = "DECREASE";
        currentLabel.Style.Add("color", "red");
        currentLabel.Text = String.Format("{0} {1}", nameOfStat, "<i class=\"fa fa-arrow-down\" aria-hidden=\"true\"></i>");
    }

    private void UpdateStatisticsData()
    {
        Regex rgx = new Regex("[0-9]+([.,][0-9]+)?");

        var test = rgx.Match(Last24LowValue.Text).NextMatch();

        //LAST STOCK VALUE
        tmpLastStockValue = InternalExchangeTransaction.GetLastStockValue(FirstIncome);
        if (FirstIncome)
            tmpCurrentStockValue = InternalExchangeTransaction.GetLastStockValue(!FirstIncome);
        else
            tmpCurrentStockValue = rgx.IsMatch(LastStockValue.Text) ? Decimal.Parse(rgx.Match(LastStockValue.Text).Value) : 0.0m;

        //LAST ASK VALUE
        tmpLastAskValue = InternalExchangeTransaction.GetLastAskValue(FirstIncome);
        if (FirstIncome)
            tmpCurrentLastAskValue = InternalExchangeTransaction.GetLastAskValue(!FirstIncome);
        else
            tmpCurrentLastAskValue = rgx.IsMatch(LastAskValue.Text) ? Decimal.Parse(rgx.Match(LastAskValue.Text).Value) : 0.0m;

        //LAST BID VALUE
        tmpLastBidValue = InternalExchangeTransaction.GetLastBidValue(FirstIncome);
        if (FirstIncome)
            tmpCurrentLastBidValue = InternalExchangeTransaction.GetLastBidValue(!FirstIncome);
        else
            tmpCurrentLastBidValue = rgx.IsMatch(LastBidValue.Text) ? Decimal.Parse(rgx.Match(LastBidValue.Text).Value) : 0.0m;

        //LAST 24H HIGH
        tmpLast24HighValue = InternalExchangeTransaction.GetLast24hHigh(FirstIncome);
        if (FirstIncome)
            tmpCurrentLast24HighValue = InternalExchangeTransaction.GetLast24hHigh(!FirstIncome);
        else
            tmpCurrentLast24HighValue = rgx.IsMatch(Last24HighValue.Text) ? Decimal.Parse(rgx.Match(Last24HighValue.Text).NextMatch().Value) : 0.0m;

        //LAST 24H LOW
        tmpLast24LowValue = InternalExchangeTransaction.GetLast24hLow(FirstIncome);
        if (FirstIncome)
            tmpCurrentLast24LowValue = InternalExchangeTransaction.GetLast24hLow(!FirstIncome);
        else
            tmpCurrentLast24LowValue = rgx.IsMatch(Last24LowValue.Text) ? Decimal.Parse(rgx.Match(Last24LowValue.Text).NextMatch().Value) : 0.0m;

        //LAST 24H VOLUME
        tmpLast24Volume = InternalExchangeTransaction.GetLast24hVolume(FirstIncome);
        if (FirstIncome)
            tmpCurrentLast24Volume = InternalExchangeTransaction.GetLast24hVolume(!FirstIncome);
        else
            tmpCurrentLast24Volume = rgx.IsMatch(Last24Volume.Text) ? Decimal.Parse(rgx.Match(Last24Volume.Text).NextMatch().Value) : 0.0m;

        FirstIncome = false;
    }

    #endregion

    private static int GetPurchaseBalanceDecimalPlaces()
    {
        if (BalanceTypeHelper.IsCryptoBalance(AppSettings.InternalExchange.InternalExchangePurchaseVia) && AppSettings.InternalExchange.InternalExchangePurchaseVia != BalanceType.BTC)
            return 18;
        return 8;
    }
}