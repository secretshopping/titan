using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Utils;
using Prem.PTC.Members;
using Resources;
using Titan.Shares;

public partial class About : System.Web.UI.Page
{
    public List<PortfolioShare> availableOptions;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (AppSettings.Shares.Policy == PorftolioSharesPolicy.Off)
            Response.Redirect("~/user/default.aspx");

        UserName.Text = Member.CurrentName;

        //Lang & Hint
        AuctionGrid1.EmptyDataText = L1.NODATA;

        LangAdder.Add(Button1, U4000.MARKET);
        LangAdder.Add(Button2, U4000.SELLUNITS);
        LangAdder.Add(RequiredFieldValidator2, U3500.CANTBEBLANK);
        LangAdder.Add(RequiredFieldValidator1, U3500.CANTBEBLANK);
        LangAdder.Add(RegularExpressionValidator1, U3500.ILLEGALCHARS);
        LangAdder.Add(BidButton, U4000.SELLUNITS);

        availableOptions = TableHelper.SelectRows<PortfolioShare>(TableHelper.MakeDictionary("OwnerUsername", Member.CurrentName));

        if (availableOptions.Count == 0)
        {
            SellingPanel.Visible = false;
            NoSharesPanel.Visible = true;
        }

        if (!Page.IsPostBack)
            BindDataToDDL();
    }


    public void MenuButton_Click(object sender, EventArgs e)
    {
        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        if (viewIndex == 0)
            Response.Redirect("market.aspx");

        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";

    }

    protected void AuctionGrid1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ShareOnMarket Share = new ShareOnMarket(Convert.ToInt32(e.Row.Cells[0].Text));

            // 3 - Portfolio Product
            // 5 - Price WITH FEE

            PortfolioProduct Product = new PortfolioProduct(Share.PortfolioProductId);

            e.Row.Cells[3].Text = "<a href=\"products.aspx?id=" + Product.Id + "\">" + Product.Name + "</a>";
            e.Row.Cells[5].Text = SharesMarketManager.CalculatePriceWithFee(Share.Price).ToString();

            ((LiteralControl)e.Row.Cells[6].Controls[2]).Text = " " + L1.BUY;

        }
    }

    protected void AuctionGrid1_DataSource_Init(object sender, EventArgs e)
    {
        AuctionGrid1_DataSource.SelectCommand = "SELECT * FROM SharesOnMarket ORDER BY DateStarted DESC";
    }


    protected void AuctionGrid1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        ParseRowCommand(AuctionGrid1, e);
    }

    private void ParseRowCommand(GridView view, GridViewCommandEventArgs e)
    {
        //We want to obey OnSort and OnChart events
        string[] commands = new string[1] { "buy" };

        if (commands.Contains(e.CommandName))
        {
            int index = e.GetSelectedRowIndex() % view.PageSize;
            GridViewRow row = view.Rows[index];
            string Id = (row.Cells[0].Text.Trim());

            var Auction = new ShareOnMarket(Convert.ToInt32(Id));
            Member User = Member.Current;

            if (e.CommandName == "buy")
            {
                SPanel.Visible = false;
                EPanel.Visible = false;
                try
                {
                    Money PriceWithFee = SharesMarketManager.CalculatePriceWithFee(Auction.Price);

                    //Balance check
                    if (PriceWithFee > User.MainBalance)
                        throw new MsgException(L1.NOTENOUGHFUNDS);

                    //Take money
                    User.SubtractFromMainBalance(PriceWithFee, "Sh. Market buy");
                    User.SaveBalances();

                    //Sell shares
                    SharesMarketManager.SellShare(Auction, User);

                    SPanel.Visible = true;
                    SText.Text = U4000.SHARESSUCC;
                }
                catch (MsgException ex)
                {
                    EPanel.Visible = true;
                    EText.Text = ex.Message;
                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex);
                    throw ex;
                }
            }
        }
    }

    private void BindDataToDDL()
    {
        var listPacks = availableOptions;

        var list = new Dictionary<string, string>();

        foreach (PortfolioShare share in listPacks)
        {
            PortfolioProduct Product = share.Product;
            list.Add(share.Id.ToString(), Product.Name + " (" + SharesMarketManager.GetSharesAvailableForSale(share) + " " + Resources.U4000.UNITS + ")");
        }
        ddlOptions.DataSource = list;
        ddlOptions.DataTextField = "Value";
        ddlOptions.DataValueField = "Key";
        ddlOptions.DataBind();
    }

    protected void BidButton_Click(object sender, EventArgs e)
    {
        ErrPanel.Visible = false;
        SucPanel.Visible = false;

        try
        {
            PortfolioShare share = new PortfolioShare(Convert.ToInt32(ddlOptions.SelectedValue));

            if (share.OwnerUsername == Member.CurrentName) //Anti-fraud check
            {
                Money Amount;
                
                if (!Money.TryParse(Price.Text, out Amount))
                    throw new MsgException(U3500.ILLEGALCHARS);

                int Units = Convert.ToInt32(UnitsBox.Text);

                Amount = Units * Amount;

                //Availability check
                if (Units > SharesMarketManager.GetSharesAvailableForSale(share)) 
                    throw new MsgException(U4000.NOTENOUGHUNITS);

                //Sell
                SharesMarketManager.AddShareToMarket(share, Units, Amount);

                SucPanel.Visible = true;
                SucMess.Text = L1.OP_SUCCESS;
            }
        }
        catch (MsgException ex)
        {
            ErrPanel.Visible = true;
            ErrMess.Text = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }
}
