using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Titan.Publisher;

public partial class user_publish_offerwalls : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.PublishersRoleEnabled && 
                AppSettings.TitanFeatures.PublishOfferWallsEnabled && Member.CurrentInCache.IsPublisher);

            AddLang();
        }
    }

    private void AddLang()
    {
        RefreshIFrameButton.Text = "<span class='fa fa-refresh'></span>";           
        GetCodeMenuButton.Text = U6000.GETCODE;
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

    protected void WebsitesDDL_SelectedIndexChanged(object sender, EventArgs e)
    {
        ShowProperIframe();
    }

    private void ShowProperIframe()
    {
        PublishersWebsite publishersWebsite = null;

        if (WebsitesDDL.Items.Count > 0)
        {
            publishersWebsite = new PublishersWebsite(Convert.ToInt32(WebsitesDDL.SelectedValue));

            if (publishersWebsite.Status == PublishersWebsiteStatus.Accepted)
            {
                string iframeHtml = string.Format("<iframe src='{0}api/externalcpaoffer.aspx?{1}={2}&{3}={{username}}' width='{4}' height='{5}'></iframe>",
                    AppSettings.Site.Url, GlobalPostback.Parameters.PublishersWebsiteId, 
                    publishersWebsite.Id, GlobalPostback.Parameters.SubId, Convert.ToInt32(WidthTextBox.Text), 
                    Convert.ToInt32(HeightTextBox.Text));


                IframeLiteral.Text = HttpUtility.HtmlEncode(iframeHtml).Replace("{username}", "<span class='text-info'>{username}</span>");
                IframeLiteral.Text = HttpUtility.HtmlEncode(iframeHtml);
                IFramePlaceHolder.Visible = true;
                IFrameUnavailablePlaceHolder.Visible = false;
                PreviewPlaceHolder.Visible = true;
            }
            else
            {
                IFramePlaceHolder.Visible = false;
                IFrameUnavailablePlaceHolder.Visible = true;
                PreviewPlaceHolder.Visible = false;
            }
        }
    }

    protected void WebsitesDDL_Init(object sender, EventArgs e)
    {
        var websites = PublishersWebsite.GetActiveAndPendingWebsites(Member.CurrentId);
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

        ShowProperIframe();
    }

    protected void RefreshIFrameButton_Click(object sender, EventArgs e)
    {
        ShowProperIframe();
    }

    protected void GetCodeView_Activate(object sender, EventArgs e)
    {
        GetCodePlaceHolder.Visible = PublishersWebsite.ActiveOrPendingWebsiteExists(Member.CurrentId);

        if (!GetCodePlaceHolder.Visible)
        {
            GetCodeUnavailable.Visible = true;
            GetCodeUnavailable.Reason = U6000.NOWEBSITES;
        }
        else
            GetCodeUnavailable.Visible = false;
    }
}