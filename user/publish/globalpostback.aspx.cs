using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using Prem.PTC.Members;
using Prem.PTC;
using Titan.Publisher;

public partial class user_publish_globalpostback : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.PublishersRoleEnabled && 
                AppSettings.TitanFeatures.PublishGlobalPostbackEnabled && Member.CurrentInCache.IsPublisher);

            AddLang();
            UrlRegularExpressionValidator.ValidationExpression = RegexExpressions.AdWebsiteUrl;
        }
    }

    private void AddLang()
    {
        NewPostbackMenuButton.Text = U6000.CONFIGURE;
        DocumentationMenuButton.Text = U6000.DOCUMENTATION;
        TestPostbackMenuButton.Text = U6000.TESTPOSTBACK;
        LangAdder.Add(ChosenWebsiteCustomValidator, U6000.WEBSITEMUSTBEACCEPTED, true);
        LangAdder.Add(UrlRequiredFieldValidator, L1.REQ_URL, true);
        LangAdder.Add(UrlRegularExpressionValidator, L1.ER_BADURL, true);
        HintAdder.Add(PostbackUrlTextBox, L1.H_URL);
        AddPostbackUrlButton.Text = L1.SAVE;
        LangAdder.Add(TestPostback_Payout_RequiredFieldValidator, U6000.REQUIRED, true);
        LangAdder.Add(TestPostback_SubId_RequiredFieldValidator, U6000.REQUIRED, true);
        TestPostbackButton.Text = L1.SEND;
    }

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

    protected void WebsitesDDL_Init(object sender, EventArgs e)
    {
        var websites = PublishersWebsite.GetWebsites(Member.CurrentId).Where(w=>w.Status == PublishersWebsiteStatus.Accepted).ToList();
        var listItems = new List<ListItem>();

        WebsitesDDL.Controls.Clear();
        int? firstActiveId = null;

        for (int i = 0; i < websites.Count; i++)
        {
            if (firstActiveId == null && websites[i].Status == PublishersWebsiteStatus.Accepted)
            {
                firstActiveId = websites[i].Id;
            }

            listItems.Add(
                new ListItem
                {
                    Value = websites[i].Id.ToString(),
                    Text = websites[i].Host + " (" + websites[i].Status.ToString() + ")",
                });
        }

        WebsitesDDL.Items.AddRange(listItems.ToArray());

        if (websites.Count > 0 && firstActiveId != null)
        {
            WebsitesDDL.SelectedValue = firstActiveId.ToString();
        }
    }

    protected void AddPostbackUrlButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            try
            {
                ErrorMessagePanel.Visible = false;
                SuccessMessagePanel.Visible = false;
                var website = new PublishersWebsite(Convert.ToInt32(WebsitesDDL.SelectedValue));

                website.AddPostbackUrl(PostbackUrlTextBox.Text);

                SuccessMessagePanel.Visible = true;
                SuccessMessage.Text = U6000.SUCCESSADDINGPOSTBACKURL;
            }
            catch (Exception ex)
            {
                if (ex is MsgException)
                {
                    ErrorMessagePanel.Visible = true;
                    ErrorMessage.Text = ex.Message;
                }
                else
                    ErrorLogger.Log(ex.Message, LogType.Publisher);
            }
        }
    }

    protected void ChosenWebsiteCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = new PublishersWebsite(Convert.ToInt32(WebsitesDDL.SelectedValue)).Status == PublishersWebsiteStatus.Accepted;
    }

    protected void PostbackUrlsDDL_Init(object sender, EventArgs e)
    {
        var webistes = PublishersWebsite.GetActiveWithPostbackUrls(Member.CurrentId);
        foreach (var w in webistes)
        {
            PostbackUrlsDDL.Items.Add(new ListItem(w.PostbackUrl, w.PostbackUrl));
        }

    }

    protected void TestPostbackButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            try
            {
                ErrorMessagePanel.Visible = false;
                SuccessMessagePanel.Visible = false;

                var subId = InputChecker.HtmlEncode(TestPostback_SubIdTextBox.Text, TestPostback_SubIdTextBox.MaxLength, GlobalPostback.Parameters.SubId);
                var payout = Money.Parse(TestPostback_PayoutTextBox.Text);

                string response = GlobalPostback.GetTestResponse(PostbackUrlsDDL.SelectedValue, subId, payout);

                if (response == GlobalPostback.Parameters.SuccessfulResponse)
                {
                    SuccessMessagePanel.Visible = true;
                    SuccessMessage.Text = string.Format(U6000.THERESPONSEWAS, response) + " " + U6000.POSTBACKIMPLEMENTEDCORRECTLY;
                }
                else
                {
                    ErrorMessagePanel.Visible = true;
                    response = HttpUtility.HtmlEncode(response);
                    if (response.Length > 50)
                        response = response.Substring(0, 50) + "(...)";

                    ErrorMessage.Text = string.Format(U6000.THERESPONSEWAS, response) + " " + U6000.EXPECTEDOK;
                }
            }
            catch (Exception ex)
            {
                //if (ex is MsgException)
                //{
                    ErrorMessagePanel.Visible = true;
                    ErrorMessage.Text = ex.Message;
                //}
                //else
                //    ErrorLogger.Log(ex);
            }
        }
    }

    protected void TestPostbackView_Activate(object sender, EventArgs e)
    {

        TestPostbackPlaceHolder.Visible = PublishersWebsite.AreAnyActiveWithPostbackUrls(Member.CurrentId);

        if (!TestPostbackPlaceHolder.Visible)
        {
            TestPostbackUnavailable.Visible = true;
            TestPostbackUnavailable.HeaderText = U6000.TESTPOSTBACKUNAVAILABLEHEADER;
            TestPostbackUnavailable.Reason = U6000.TESTPOSTBACKUNAVAILABLEREASON;
        }
        else
            TestPostbackUnavailable.Visible = false;

    }

    protected void NewPostbackView_Activate(object sender, EventArgs e)
    {
        NewPostbackPlaceHolder.Visible = TableHelper.RowExists(PublishersWebsite.TableName, TableHelper.MakeDictionary("UserId", Member.CurrentId));

        if (!NewPostbackPlaceHolder.Visible)
        {
            AddPostbackUnavailable.Visible = true;
            AddPostbackUnavailable.Reason = U6000.NOWEBSITES;
        }
        else
            AddPostbackUnavailable.Visible = false;
    }
}