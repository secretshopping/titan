using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Titan.InternalExchange;

public partial class user_internalexchange_history : System.Web.UI.Page
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

    private static String _signOfStock, _signOfPurchaseBalance;
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.InternalExchangeTradingHistoryEnabled);

        if (!IsPostBack)
        {
            SignOfStock = InternalExchangeManager.GetBalanceSign(true);
            SignOfPurchaseBalance = InternalExchangeManager.GetBalanceSign(false);

            LangAdders();
            DataBind();
        }
    }

    private void LangAdders()
    {
        LangAdder.Add(MainTab, L1.HISTORY);

        TransactionHistoryGridView.EmptyDataText = L1.NODATA;
    }

    #region GridView History
    private string GridViewHistoryCommand =
        @"SELECT
            TransactionId
	        , TransactionAmount
	        , TransactionValue
	        , TransactionDate
	        , 1 AS IsAsk
            , BidUserId
        FROM
            InternalExchangeTransactions
        WHERE
            AskUserId = {0}
        UNION ALL
        SELECT
            TransactionId
            , TransactionAmount
            , TransactionValue
            , TransactionDate
            , 0 AS IsAsk
            , BidUserId
        FROM
            InternalExchangeTransactions
        WHERE
            BidUserId = {0}
        ORDER BY
            TransactionDate DESC";

    protected void TransactionHistoryGridView_DataSource_Init(object sender, EventArgs e)
    {
        TransactionHistoryGridView_DataSource.SelectCommand = String.Format(GridViewHistoryCommand, Member.CurrentId);
    }

    protected void TransactionHistoryGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label TransactionValue = new Label();

            TransactionValue.Text = InternalExchangeManager.RecognizeCurrencyAndReturnString(false, (Decimal.Parse(e.Row.Cells[2].Text) * Decimal.Parse(e.Row.Cells[3].Text)));

            e.Row.Cells[2].Text = InternalExchangeManager.RecognizeCurrencyAndReturnString(true, e.Row.Cells[2].Text);
            e.Row.Cells[3].Text = InternalExchangeManager.RecognizeCurrencyAndReturnString(false, e.Row.Cells[3].Text);

            bool isAsk = !e.Row.Cells[6].Text.Equals(Member.CurrentId.ToString());
            String SignA = isAsk ? "-" : "+";
            String SignB = isAsk ? "+" : "-";
            String Color = isAsk ? "red" : "green";
            String OfferType = isAsk ? U6009.SELL : L1.BUY;

            e.Row.Style.Add("color", Color);
            TransactionValue.Text = String.Format("{0} {1}", SignB, TransactionValue.Text);

            e.Row.Cells[1].Text = OfferType.ToUpper();
            e.Row.Cells[2].Text = String.Format("{0} {1}", SignA, e.Row.Cells[2].Text);
            e.Row.Cells[4].Controls.Add(TransactionValue);
        }
    }
    #endregion
}