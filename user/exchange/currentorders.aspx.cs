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
using Titan.ICO;
using Titan.InternalExchange;

public partial class user_internalexchange_currentorders : System.Web.UI.Page
{
    private const string tradeDetailIndexString = "tradeDetailIndex";
    private const string tradeDetailGridString = "tradeDetailGrid";

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.InternalExchangeCurrentOrdersEnabled);

        if (!IsPostBack)
        {
            HideMessagges();
            LangAdders();

            ViewState[tradeDetailIndexString] = -1;
            ViewState[tradeDetailGridString] = string.Empty;
        }
    }

    private void LangAdders()
    {
        LangAdder.Add(MainTab, U6012.MYCURRENTORDERS);
        UserCurrentBidsGridView.EmptyDataText = L1.NODATA;
        UserCurrentAsksGridView.EmptyDataText = L1.NODATA;
    }

    private void HideMessagges()
    {
        SuccessMessagePanel.Visible = false;
        ErrorMessagePanel.Visible = false;
    }

    #region GridView RowBound

    protected void OfferHeaderRowBound(GridViewRow row)
    {
        row.Cells[2].Text += String.Format("({0})", InternalExchangeManager.GetBalanceSign(true));
        row.Cells[3].Text += String.Format("({0})", InternalExchangeManager.GetBalanceSign(false));
        row.Cells[4].Text += String.Format("({0})", InternalExchangeManager.GetBalanceSign(false));
        row.Cells[5].Text += String.Format("({0})", InternalExchangeManager.GetBalanceSign(true));
    }

    protected void OfferDataRowBound(GridViewRow row, GridView gridView)
    {
        decimal amount = Decimal.Parse(row.Cells[2].Text);
        decimal value = Decimal.Parse(row.Cells[3].Text);
        DateTime offerDateTime = DateTime.Parse(row.Cells[6].Text);
        ExchangeOfferStatus status = (ExchangeOfferStatus)Int32.Parse(row.Cells[7].Text);
        decimal originalAmount = Decimal.Parse(row.Cells[10].Text);

        row.Cells[2].Text = FormatDecimal(amount);
        row.Cells[3].Text = FormatDecimal(value);
        row.Cells[4].Controls.Add(GetFreezedValueOfOfferLabel(amount, value));
        row.Cells[5].Controls.Add(GetCurrentOfferVolumeLabel(amount, originalAmount));
        //row.Cells[8].Style.Add("width", "15%");

        //If offer lifetime is lower than limit, user can't withdraw it
        if (status == ExchangeOfferStatus.Open
            && AppSettings.InternalExchange.InternalExchangeLockWithdraw
            && offerDateTime > AppSettings.ServerTime.AddMinutes(-AppSettings.InternalExchange.InternalExchangeLockWithdrawTime))
        {
            row.Cells[8].Controls[1].Visible = false;
            row.Cells[8].Style.Add("color", "green");
            row.Cells[8].Style.Add("width", "13px");
            row.Cells[8].Controls.Add(GetOfferWithdrawalCounterLabel(offerDateTime));
        }
        else if (status != ExchangeOfferStatus.Open)
        {
            row.Cells[8].Controls[1].Visible = false;
        }

        if (amount == originalAmount)
        {
            row.Cells[9].Controls[1].Visible = false;
        }

        if (status == ExchangeOfferStatus.Open && amount != originalAmount)
        {
            status = ExchangeOfferStatus.Partial;
        }
        
        Label statusLabel = new Label();
        statusLabel.Text = status.ToString();
        statusLabel.CssClass = String.Format("badge {0}", ExchangeOfferStatusHelper.ColorDictionary[status]);
        row.Cells[7].Controls.Add(statusLabel);

        int tradeDetailIndex = (int)ViewState[tradeDetailIndexString];
        string tradeDetailGrid = ViewState[tradeDetailGridString].ToString();

        bool show =
            tradeDetailIndex == row.RowIndex
            && tradeDetailGrid == gridView.ID;

        var placeHolder = (PlaceHolder)row.Cells[11].Controls[1];
        
        if (show)
        {
            int offerId = Int32.Parse(row.Cells[0].Text);
            var childGridView = (GridView)placeHolder.Controls[1];

            List<InternalExchangeTransaction> date = tradeDetailGrid == UserCurrentAsksGridView.ID ?
                InternalExchangeTransaction.GetAskDetail(offerId) :
                InternalExchangeTransaction.GetBidDetail(offerId);

            childGridView.DataSource = date;
            childGridView.DataBind();
        }

        placeHolder.Visible = show;
    }

    protected void UserCurrentOfferGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            OfferHeaderRowBound(e.Row);
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            GridView gridView = (GridView)sender;

            OfferDataRowBound(e.Row, gridView);
        }
    }
    #endregion

    #region Detail GridView RowBound

    protected void DetailHeaderRowBound(GridViewRow row, bool isAsk)
    {
        row.Cells[0].Text += " #";
        row.Cells[2].Text += String.Format("({0})", InternalExchangeManager.GetBalanceSign(true));
        row.Cells[3].Text += String.Format("({0})", InternalExchangeManager.GetBalanceSign(false));
        row.Cells[4].Text += String.Format("({0})", InternalExchangeManager.GetBalanceSign(true));
        row.Cells[5].Text += String.Format("({0})", InternalExchangeManager.GetBalanceSign(false));
    }

    protected void DetailDataRowBound(GridViewRow row)
    {
        decimal amount = Decimal.Parse(row.Cells[2].Text);
        decimal value = Decimal.Parse(row.Cells[3].Text);
        
        row.Cells[2].Text = FormatDecimal(amount);
        row.Cells[3].Text = FormatDecimal(value);
        row.Cells[4].Text = FormatDecimal(amount * value);
        row.Cells[5].Text = row.Cells[5].Text.TrimEnd('0').TrimEnd('.', ',');
    }

    protected void TradeDetailGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            bool isAsk = ViewState[tradeDetailGridString].ToString() == UserCurrentAsksGridView.ID;

            DetailHeaderRowBound(e.Row, isAsk);
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DetailDataRowBound(e.Row);
        }
    }

    #endregion

    #region GridView Init

    private const string userCurrentBidSelectCommand = "SELECT * FROM InternalExchangeBids WHERE BidUserId={0} ORDER BY CASE BidStatus WHEN 1 THEN 0 ELSE 1 END, BidDate DESC";
    private const string userCurrentAskSelectCommand = "SELECT * FROM InternalExchangeAsks WHERE AskUserId={0} ORDER BY CASE AskStatus WHEN 1 THEN 0 ELSE 1 END, AskDate DESC";

    protected void UserCurrentBidsGridView_DataSource_Init(object sender, EventArgs e)
    {
        UserCurrentBidsGridView_DataSource.SelectCommand = String.Format(userCurrentBidSelectCommand, Member.CurrentId);
    }

    protected void UserCurrentAsksGridView_DataSource_Init(object sender, EventArgs e)
    {
        UserCurrentAsksGridView_DataSource.SelectCommand = String.Format(userCurrentAskSelectCommand, Member.CurrentId);
    }

    #endregion

    protected void UserCurrentAsksAndBids_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        var commands = new[] { "finishBid", "finishAsk", "tradeBid", "tradeAsk" };

        if (commands.Contains(e.CommandName))
        {
            GridView gridView = (GridView)sender;

            var index = e.GetSelectedRowIndex() % gridView.PageSize;
            var row = gridView.Rows[index];

            switch (e.CommandName)
            {
                case "finishAsk":
                    try
                    {
                        var askId = Int32.Parse(row.Cells[0].Text.Trim());

                        var currentAsk = new InternalExchangeAsk(askId);

                        if (currentAsk != null)
                        {
                            currentAsk.Withdraw();
                            SuccessMessageLiteral.Text = String.Format("{0}{1} {2}", InternalExchangeManager.GetBalanceSign(true), FormatDecimal(currentAsk.AskAmount), U6012.RETURNEDTOYOURBALANCE.ToLower());
                            SuccessMessagePanel.Visible = true;
                        }
                        else
                            throw new MsgException("Unexpected error: current offer not exist anymore");
                    }
                    catch (MsgException ex)
                    {
                        ErrorMessagePanel.Visible = true;
                        ErrorMessageLiteral.Text = ex.Message;
                    }
                    catch (DbException ex)
                    {
                        ErrorMessagePanel.Visible = true;
                        ErrorMessageLiteral.Text = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.Log(ex.Message);
                        throw ex;
                    }
                    finally
                    {
                        DataBind();
                    }

                    break;

                case "finishBid":
                    try
                    {
                        var bidId = Int32.Parse(row.Cells[0].Text.Trim());

                        var currentBid = new InternalExchangeBid(bidId);

                        if (currentBid != null)
                        {
                            currentBid.Withdraw();
                            SuccessMessageLiteral.Text = String.Format("{0}{1} {2}", InternalExchangeManager.GetBalanceSign(false), FormatDecimal(currentBid.BidAmount * currentBid.BidValue), U6012.RETURNEDTOYOURBALANCE.ToLower());
                            SuccessMessagePanel.Visible = true;
                        }
                        else
                            throw new MsgException("Unexpected error: current offer not exist anymore");
                    }
                    catch (MsgException ex)
                    {
                        ErrorMessagePanel.Visible = true;
                        ErrorMessageLiteral.Text = ex.Message;
                    }
                    catch (DbException ex)
                    {
                        ErrorMessagePanel.Visible = true;
                        ErrorMessageLiteral.Text = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.Log(ex.Message);
                        throw ex;
                    }
                    finally
                    {
                        DataBind();
                    }

                    break;
                case "tradeAsk":
                case "tradeBid":
                    int tradeDetailIndex = (int)ViewState[tradeDetailIndexString];
                    string tradeDetailGrid = ViewState[tradeDetailGridString].ToString();

                    if (tradeDetailIndex == index && tradeDetailGrid == gridView.ID)
                    {
                        ViewState[tradeDetailIndexString] = -1;
                        ViewState[tradeDetailGridString] = String.Empty;
                    }
                    else
                    {
                        ViewState[tradeDetailIndexString] = index;
                        ViewState[tradeDetailGridString] = gridView.ID;
                    }
                    DataBind();

                    break;
            }
        }
    }

    private static Label GetFreezedValueOfOfferLabel(decimal amount, decimal value)
    {
        Label FreezedValue = new Label();
        FreezedValue.Text = (amount * value).ToString("G0");
        return FreezedValue;
    }

    private static Label GetOfferWithdrawalCounterLabel(DateTime offerTime)
    {
        Label counter = new Label();
        TimeSpan span = AppSettings.ServerTime - offerTime;
        int Minutes = AppSettings.InternalExchange.InternalExchangeLockWithdrawTime - (int)span.TotalMinutes;
        counter.Text = String.Format("{0} {1} {2}", Minutes, U5005.MINUTES.ToLower(), U4200.TOWITHDRAW);

        return counter;
    }

    private static Label GetCurrentOfferVolumeLabel(decimal amount, decimal originalAmount)
    {
        Label Volume = new Label();

        Volume.Text = String.Format("{0} / {1}", (originalAmount - amount).ToString("N18").TrimEnd('0').TrimEnd('.'), originalAmount.ToString("N18").TrimEnd('0').TrimEnd('.'));

        return Volume;
    }

    private String FormatDecimal(decimal d)
    {
        return d.ToString("N18").TrimEnd('0').TrimEnd('.', ',');
    }
}