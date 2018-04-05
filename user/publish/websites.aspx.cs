using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class user_publish_websites : System.Web.UI.Page
{
    string validURL;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.PublishersRoleEnabled && 
                AppSettings.TitanFeatures.PublishWebsitesEnabled && Member.CurrentInCache.IsPublisher);

            ErrorMessagePanel.Visible = SuccessMessagePanel.Visible = false;
            AddLang();
        }
        UrlRegularExpressionValidator.ValidationExpression = RegexExpressions.AdWebsiteUrl;
        Form.Action = Request.RawUrl;

    }

    private void AddLang()
    {
        MenuButtonMyWebsites.Text = U6000.MYWEBSITES;
        MenuButtonAddWebsite.Text = L1.ADDNEW;
        LangAdder.Add(UrlRequiredFieldValidator, L1.REQ_URL, true);
        LangAdder.Add(UrlRegularExpressionValidator, L1.ER_BADURL, true);
        HintAdder.Add(WebsiteUrlTextBox, L1.H_URL);
        AddWebsiteButton.Text = L1.ADDNEW;
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

    protected void AddWebsitesView_Activate(object sender, EventArgs e)
    {
        AddWebsitePlaceHolder.Visible = PublishersWebsiteCategory.AreAnyActive();

        if (!AddWebsitePlaceHolder.Visible)
        {
            NewWebsiteUnavailable.Visible = true;
            NewWebsiteUnavailable.HeaderText = U6000.NEWWEBSITEUNAVAILABLEHEADER;
            NewWebsiteUnavailable.Reason = U6000.NEWWEBSITEUNAVAILABLEREASON;
        }
        else
            NewWebsiteUnavailable.Visible = false;
    }

    protected void MyWebsitesView_Activate(object sender, EventArgs e)
    {
        MyWebsitesGridView.DataBind();
    }

    protected void UrlCheckerUpdatePanel_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(Request.Params.Get("__EVENTARGUMENT")))
        {
            string argument = Request.Params.Get("__EVENTARGUMENT");

            validURL = Encryption.Decrypt(argument);

            if (validURL.StartsWith("http"))
            {
                WebsiteUrlTextBox.Text = validURL;
                WebsiteUrlTextBox.Enabled = false;
                CheckURLButton.Visible = false;
            }
        }
    }

    protected void CategoriesDDL_Init(object sender, EventArgs e)
    {
        var categories = PublishersWebsiteCategory.GetActive();
        var listItems = new List<ListItem>();
        foreach (var c in categories)
        {
            listItems.Add(
                new ListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
        }

        CategoriesDDL.Controls.Clear();
        CategoriesDDL.Items.AddRange(listItems.ToArray());
    }

    protected void AddWebsiteButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        SuccessMessagePanel.Visible = false;
        try
        {
            if (Page.IsValid)
            {
                AppSettings.DemoCheck();

                if (WebsiteUrlTextBox.Enabled)
                    throw new MsgException(U4200.CHECKURL);

                PublishersWebsite.Create(Member.CurrentId, WebsiteUrlTextBox.Text, Convert.ToInt32(CategoriesDDL.SelectedValue));

                SuccessMessagePanel.Visible = true;
                SuccessMessage.Text = U6000.REQUESTSENTPLEASEWAIT;
                ClearNewWebsiteFields(true);
            }
        }
        catch (Exception ex)
        {
            if (ex is MsgException)
            {
                ErrorMessagePanel.Visible = true;
                ErrorMessage.Text = ex.Message;
            }
            else
                ErrorLogger.Log(ex);
            ClearNewWebsiteFields(false);
        }
    }

    private void ClearNewWebsiteFields(bool successful)
    {
        WebsiteUrlTextBox.Enabled = true;
        CheckURLButton.Visible = true;

        if (successful)
            WebsiteUrlTextBox.Text = string.Empty;
    }

    protected void MyWebsitesGridView_DataSource_Init(object sender, EventArgs e)
    {
        MyWebsitesGridView_DataSource.SelectCommand = string.Format(@"SELECT * FROM PublishersWebsites WHERE UserId = {0} AND Status != {1}", 
            Member.CurrentId, (int)PublishersWebsiteStatus.Deleted);
    }

    protected void MyWebsitesGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[2].Text = new PublishersWebsiteCategory(Convert.ToInt32(e.Row.Cells[2].Text)).Name;
        }
    }

    protected void MyWebsitesGridView_DataBound(object sender, EventArgs e)
    {
        MyWebsitesGridView.Columns[2].HeaderText = U5006.CATEGORY;
        MyWebsitesGridView.Columns[3].HeaderText = L1.STATUS;
        MyWebsitesGridView.Columns[4].HeaderText = U6000.POSTBACKURL;
    }
}