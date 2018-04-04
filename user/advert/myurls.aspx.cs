using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC.Advertising;
using Prem.PTC.Utils;
using Titan.Publisher.Security;

public partial class user_advert_myurls : System.Web.UI.Page
{
    string validURL;
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.AdvertisersRoleEnabled && AppSettings.TitanFeatures.AdvertMyUrlsEnabled && Member.CurrentInCache.IsAdvertiser);

        if (!IsPostBack)
        {
            AddLang();
        }
        UrlRegularExpressionValidator.ValidationExpression = RegexExpressions.AdWebsiteUrl;

        Form.Action = Request.RawUrl;
        ErrorMessagePanel.Visible = false;
    }

    private void AddLang()
    {
        MenuButtonAddUrls.Text = L1.ADDNEW;
        MenuButtonMyUrls.Text = L1.MANAGE;        
        LangAdder.Add(UrlRequiredFieldValidator, L1.REQ_URL, true);
        LangAdder.Add(UrlRegularExpressionValidator, L1.ER_BADURL, true);
        HintAdder.Add(UrlTextBox, L1.H_URL);

        AddUrlButton.Text = L1.ADDNEW;
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

    protected void UrlCheckerUpdatePanel_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(Request.Params.Get("__EVENTARGUMENT")))
        {
            string argument = Request.Params.Get("__EVENTARGUMENT");

            validURL = Encryption.Decrypt(argument);

            if (validURL.StartsWith("http"))
            {
                UrlTextBox.Text = validURL;
                UrlTextBox.Enabled = false;
                CheckURLButton.Visible = false;
            }
        }
    }

    protected void MyUrlsGridView_DataBound(object sender, EventArgs e)
    {
        MyUrlsGridView.Columns[1].HeaderText = "URL";
        MyUrlsGridView.Columns[2].HeaderText = L1.STATUS;
    }

    protected void MyUrlsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var url = e.Row.Cells[1].Text;
            e.Row.Cells[1].Text = url.Length > 30 ? e.Row.Cells[1].Text = url.Substring(0, 30) : url;

            var status = (AdvertStatus)Convert.ToInt32(e.Row.Cells[2].Text);
            e.Row.Cells[2].Text = HtmlCreator.GetColoredStatus(status);

            // Start[3] Pause[4] Remove[5]
            if (status != AdvertStatus.Paused)
                e.Row.Cells[3].Text = "&nbsp;";

            if (status != AdvertStatus.Active)
                e.Row.Cells[4].Text = "&nbsp;";

            if (!status.CanBeRemoved())
                e.Row.Cells[5].Text = "&nbsp;";
        }

    }

    protected void MyUrlsGridView_DataSource_Init(object sender, EventArgs e)
    {
        MyUrlsGridView_DataSource.SelectCommand =
            string.Format(@"SELECT * FROM UserUrls
                            WHERE UserId = {0} AND Status != {1}", Member.CurrentId, (int)AdvertStatus.Deleted);
    }


    protected void MyUrlsView_Activate(object sender, EventArgs e)
    {
        MyUrlsGridView.DataBind();
    }

    protected void AddUrlButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            try
            {

                AppSettings.DemoCheck();
                ErrorMessagePanel.Visible = SuccessMessagePanel.Visible = false;

                UserUrl.Create(Member.CurrentId, UrlTextBox.Text);

                SuccessMessagePanel.Visible = true;
                SuccessMessage.Text = U6000.REQUESTSENTPLEASEWAIT;

            }
            catch (Exception ex)
            {
                if (ex is Exception)
                {
                    ErrorMessagePanel.Visible = true;
                    ErrorMessage.Text = ex.Message;
                }
                else
                    ErrorLogger.Log(ex);
            }
            finally
            {
                ClearAll();
            }
        }
    }

    private void ClearAll()
    {
        UrlTextBox.Text = string.Empty;
        UrlTextBox.Enabled = true;
        CheckURLButton.Visible = true;
    }

    protected void MyUrlsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        var commands = new[] { "start", "stop", "remove" };

        if (commands.Contains(e.CommandName))
        {
            var index = e.GetSelectedRowIndex() % MyUrlsGridView.PageSize;
            var row = MyUrlsGridView.Rows[index];
            var urlId = (row.Cells[0].Text.Trim());
            var url = new UserUrl(Convert.ToInt32(urlId));

            switch (e.CommandName)
            {
                case "start":
                    if (url.Status == AdvertStatus.Paused)
                        url.Activate();
                    break;
                case "stop":
                    if (url.Status == AdvertStatus.Active)
                        url.Pause();
                    break;
                case "remove":
                    url.Delete();
                    break;
            }

            MyUrlsGridView.DataBind();
        }
    }
}