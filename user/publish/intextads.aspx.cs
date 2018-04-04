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

public partial class user_publish_intextads : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.PublishersRoleEnabled && 
                AppSettings.TitanFeatures.PublishInTextAdsEnabled && Member.CurrentInCache.IsPublisher);

            
            ShowHideContent();
        }
    }

    private void ShowHideContent()
    {
        GetCodePlaceHolder.Visible =
                PublishersWebsite.ActiveOrPendingWebsiteExists(Member.CurrentId);

        if (!GetCodePlaceHolder.Visible)
        {
            GetCodeUnavailable.Visible = true;
            GetCodeUnavailable.Reason = U6000.NOWEBSITES;
        }
        else
            GetCodeUnavailable.Visible = false;
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
                string iframeHtml = string.Format("<script src='{0}Handlers/InText/GetAds.ashx?{1}={2}'></script>",
               AppSettings.Site.Url, GlobalPostback.Parameters.PublishersWebsiteId, publishersWebsite.Id);

                JSLiteral.Text = HttpUtility.HtmlEncode(iframeHtml);

                IFramePlaceHolder.Visible = true;
                IFrameUnavailablePlaceHolder.Visible = false;
            }
            else
            {
                IFramePlaceHolder.Visible = false;
                IFrameUnavailablePlaceHolder.Visible = true;
            }
        }
    }
}