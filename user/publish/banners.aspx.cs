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

public partial class user_publish_banners : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.PublishersRoleEnabled && AppSettings.TitanFeatures.PublishBannersEnabled && Member.CurrentInCache.IsPublisher);

            ErrorMessagePanel.Visible = false;
            SuccessMessagePanel.Visible = false;

            GetCodePlaceHolder.Visible =
                PublishersWebsite.ActiveOrPendingWebsiteExists(Member.CurrentId)
                && TableHelper.RowExists(ExternalBannerAdvertDimensions.TableName, TableHelper.MakeDictionary("Status", (int)UniversalStatus.Active));

            if (!GetCodePlaceHolder.Visible)
            {
                GetCodeUnavailable.Visible = true;
                GetCodeUnavailable.HeaderText = U6003.PUBLISHBANNERSUNAVAILABLEHEADER;
                GetCodeUnavailable.Reason = U6003.PUBLISHBANNERSUNAVAILABLEREASON;
            }
            else
                GetCodeUnavailable.Visible = false;
        }
    }

    protected void DimensionsDDL_SelectedIndexChanged(object sender, EventArgs e)
    {
        ShowProperIframe();
    }

    private void ShowProperIframe()
    {
        try
        {
            ExternalBannerAdvertDimensions dimensions = null;

            PublishersWebsite publishersWebsite = null;

            if (WebsitesDDL.Items.Count > 0 && DimensionsDDL.Items.Count > 0)
            {
                dimensions = new ExternalBannerAdvertDimensions(Convert.ToInt32(DimensionsDDL.SelectedValue));
                publishersWebsite = new PublishersWebsite(Convert.ToInt32(WebsitesDDL.SelectedValue));

                if (publishersWebsite.Status == PublishersWebsiteStatus.Accepted)
                {
                    string iframeHtml = string.Format("<iframe src='{0}api/externalbanner.aspx?d={1}&{2}={3}' width='{4}px' height='{5}' scrolling='no'></iframe>",
                AppSettings.Site.Url, dimensions.Id, GlobalPostback.Parameters.PublishersWebsiteId, publishersWebsite.Id, dimensions.Width, dimensions.Height);
                    IframeLiteral.Text = HttpUtility.HtmlEncode(iframeHtml);
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
        catch(Exception ex)
        {
            ErrorLogger.Log(ex);
            ErrorLogger.Log(ex.Message, LogType.Publisher);
        }
    }

    protected void DimensionsDDL_Init(object sender, EventArgs e)
    {
        var dimensions = ExternalBannerAdvertDimensions.GetActive();
        var listItems = new List<ListItem>();

        foreach (var p in dimensions)
        {
            listItems.Add(
                new ListItem
                {
                    Value = p.Id.ToString(),
                    Text = string.Format("{0} x {1}px", p.Width, p.Height)
                });
        }

        DimensionsDDL.Controls.Clear();
        DimensionsDDL.Items.AddRange(listItems.ToArray());

        if (dimensions.Count > 0)
        {
            DimensionsDDL.SelectedValue = dimensions[0].Id.ToString();
            ShowProperIframe();
        }
    }

    protected void WebsitesDDL_SelectedIndexChanged(object sender, EventArgs e)
    {
        ShowProperIframe();
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
}