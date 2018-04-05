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
                AppSettings.TitanFeatures.PublishPTCOfferWallsEnabled && Member.CurrentInCache.IsPublisher);

            AddLang();
        }
    }

    private void AddLang()
    {
        RefreshIFrameButton.Text = "<span class='fa fa-refresh'></span>";           
        GetCodeMenuButton.Text = U6000.GETCODE;
        StatisticsMenuButton.Text = L1.STATISTICS;
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
                string iframeHtml = string.Format(@"<iframe src='{0}api/ptcofferwall.aspx?{1}={2}&{3}={{username}}&{4}={{age}}&{5}={{gender}}' 
                                            width='{6}' height='{7}'></iframe>",
                    AppSettings.Site.Url, GlobalPostback.Parameters.PublishersWebsiteId, publishersWebsite.Id,
                    GlobalPostback.Parameters.SubId, GlobalPostback.Parameters.Age, GlobalPostback.Parameters.Gender,
                    Convert.ToInt32(WidthTextBox.Text), Convert.ToInt32(HeightTextBox.Text));


                IframeLiteral.Text = HttpUtility.HtmlEncode(iframeHtml)
                                    .Replace("{username}", "<span class='text-info'>{username}</span>")
                                    .Replace("{age}", "<span class='text-info'>{age}</span>")
                                    .Replace("{gender}", "<span class='text-info'>{gender}</span>");
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

    #region Statistics
    protected void StatisticsGridViewDataSource_Init(object sender, EventArgs e)
    {
        string query = string.Format(@"SELECT COUNT(v.Id) AS Views, w.Host, SUM(v.PublisherPayout) AS Payout, o.Title
                                        FROM PTCOfferWallViews v 
                                        JOIN PublishersWebsites w ON v.PublishersWebsiteId = w.Id
                                        JOIN PTCOfferWalls o ON v.PTCOfferWallId = o.Id
                                        WHERE w.UserId = {0}
                                        GROUP BY w.Id, w.Host, v.PublisherPayout, o.Title",
                                        Member.CurrentId);
        StatisticsGridViewDataSource.SelectCommand = query;
    }

    protected void StatisticsGridView_DataBound(object sender, EventArgs e)
    {
        StatisticsGridView.Columns[0].HeaderText = U6003.OFFERTITLE;
        StatisticsGridView.Columns[1].HeaderText = U6000.WEBSITE;
        StatisticsGridView.Columns[2].HeaderText = U6000.SUBMISSIONS;
        StatisticsGridView.Columns[3].HeaderText = U4000.PAYOUT;
    }
    #endregion

    protected void StatisticsView_Activate(object sender, EventArgs e)
    {
        StatisticsGridView.DataBind();
    }
}