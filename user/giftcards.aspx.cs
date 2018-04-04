using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using System.Data;
using Prem.PTC;
using Prem.PTC.Members;
using System.Web.Security;

public partial class GiftCards : System.Web.UI.Page
{
    List<GiftCard> cards;

    protected void Page_Load(object sender, EventArgs e)
    {
        //Turned off
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.MoneyGiftCardsEnabled);

        cards = GiftCard.GetActiveCards();

        if (!Page.IsPostBack)
        {
            //Setup
            GiftCardsGridView.EmptyDataText = U4000.NOGIFTCARDS;

            //Append Data
            GiftCardsGridView.DataSource = GetGridViewData();
            GiftCardsGridView.DataBind();
        }

        //ListPanelsPlaceHolder
        if (ListPanelsPlaceHolder.Controls.Count == 0)
            foreach (var card in cards)
                ListPanelsPlaceHolder.Controls.Add(GeneratePanel(card));
    }

    protected void GiftCardsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //Get HTML to work
            e.Row.Cells[2].Text = HttpUtility.HtmlDecode(e.Row.Cells[2].Text);
            e.Row.Cells[3].Text = HttpUtility.HtmlDecode(e.Row.Cells[3].Text);

            //Button text
            var button = (Button)e.Row.Cells[4].FindControl("Button1");
            //button.Text = U4000.EXCHANGEPOINTS.Replace("%p%", AppSettings.PointsName);

            button.Text = U4000.EXCHANGEPOINTS.Replace("%p%", "");
            button.OnClientClick = "showPopupList(" + e.Row.Cells[0].Text + "); return false;";
        }
    }

    private DataTable GetGridViewData()
    {
        var dt = new DataTable();
        dt.Columns.Add("Id", 1.GetType());
        dt.Columns.Add("ImageUrl", "a".GetType());
        dt.Columns.Add("Name", "a".GetType());
        dt.Columns.Add("Description", "a".GetType());

        for (int i = 0; i < cards.Count; ++i)
        {
            var card = cards[i];

            var dr = dt.NewRow();
            dr["Id"] = card.Id;
            dr["ImageUrl"] = card.ImageUrl;
            dr["Name"] = card.Title;
            dr["Description"] = card.Description;
            dt.Rows.Add(dr);
        }
        return dt;
    }

    protected Panel GeneratePanel(GiftCard giftcard)
    {
        Panel cardPanel = new Panel();
        cardPanel.ClientIDMode = System.Web.UI.ClientIDMode.Static;
        cardPanel.ID = "cardPanel" + giftcard.Id;
        cardPanel.Attributes.Add("style", "display:none;");

        Image image = new Image();
        image.ImageUrl = giftcard.ImageUrl;
        cardPanel.Controls.Add(image); //Append it

        Literal lit1 = new Literal();
        lit1.Text = "<br/><h3>" + giftcard.Title + "</h3><br/>";

        var codes = GiftCode.GetActiveCodesForMember(Member.CurrentInCache, giftcard.Id);

        if (codes.Count > 0)
        {
            //Generate table
            lit1.Text += "<table class=\"exchangeTable\">";
            cardPanel.Controls.Add(lit1);

            foreach (var code in codes)
            {
                Literal lit2 = new Literal();
                lit2.Text = "<tr><td>" + U4000.EXCHANGENPFOR.Replace("%n%", "<b>" + code.Value + "</b>")
                            .Replace("%p%", AppSettings.PointsName).Replace("%w%", code.Key.Value) + "</td><td>";
                cardPanel.Controls.Add(lit2);

                Button button = new Button();
                button.CssClass = "btn btn-inverse btn-xs";
                button.Text = L1.SUBMIT;
                button.OnClientClick = "hideList(" + giftcard.Id + ");";
                button.CommandArgument = code.Key.Id.ToString();
                button.Command += button_Click;
                cardPanel.Controls.Add(button);

                //Register for AJAX
                ScriptManager scriptMan = ScriptManager.GetCurrent(this);
                scriptMan.RegisterAsyncPostBackControl(button);

                Literal lit3 = new Literal();
                lit3.Text = "</td></tr>";
                cardPanel.Controls.Add(lit3);
            }

            Literal lit4 = new Literal();
            lit4.Text = "</table><br /><br /><a href=\"#\" class=\"btn btn-danger\" onclick=\"hidePopupList(" + giftcard.Id + "); return false;\">"
                      + U4000.CANCEL + "</a>";
            cardPanel.Controls.Add(lit4);
        }
        else
        {
            lit1.Text += "<br/><i>" + U4000.THEREARENOCODES + "</i>";
            lit1.Text += "<br /><br /><a href=\"#\" class=\"btn btn-danger\" onclick=\"hidePopupList(" + giftcard.Id + "); return false;\">"
                      + U4000.CANCEL + "</a>";
            cardPanel.Controls.Add(lit1);
        }

        return cardPanel;
    }

    void button_Click(object sender, CommandEventArgs e)
    {
        CodeSubmitedPanel.Visible = true;
        GiftCode Code = new GiftCode(Convert.ToInt32(e.CommandArgument));

        try
        {
            Member User = Member.Current;

            Int32 Price = GiftCodeExchangeRate.GetPrice(User, Code.Id);
            if (Price < 0)
                throw new MsgException("You are not eligible for this code");

            //Activity check
            if (!Code.IsActiveAtTheMoment(User))
                throw new MsgException("This card/code is disabled");

            //Check if member is not banned meantime
            if (User.IsBanned)
            {
                Member.Current.Logout(Response);
                FormsAuthentication.SignOut();
                Session.Abandon();
                Response.Redirect("~/status.aspx?type=logoutsuspended&id=logoutsus");
            }

            //Balance check
            if (Price > User.PointsBalance)
                throw new MsgException(L1.NOTENOUGHFUNDS);

            //Send the request & take the money
            GiftCodeRequest.Add(User, Code.Id, Price);
        }
        catch (MsgException ex)
        {
            SuccPanel.Visible = false;
            ErrorPanel.Visible = true;
            ErrorMessage.Text = ex.Message;
        }
    }

    protected void GiftCardsGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GiftCardsGridView.PageIndex = e.NewPageIndex;
        GiftCardsGridView.DataSource = GetGridViewData();
        GiftCardsGridView.DataBind();
    }
}
