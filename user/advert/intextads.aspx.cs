using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC.Utils;
using Titan.Publisher.InTextAds;

public partial class user_advert_intextads : System.Web.UI.Page
{
    protected int? maxNumberOfTags;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.AdvertisersRoleEnabled && AppSettings.TitanFeatures.AdvertInTextAdsEnabled && Member.CurrentInCache.IsAdvertiser);

            AddLang();
        }

        ErrorMessagePanel.Visible = false;
        TagsTextBox.Attributes.Add("data-max", maxNumberOfTags.ToString());
        TagsTextBox.Attributes.Add("data-message", string.Format(U6002.TOOMANYTAGS, maxNumberOfTags));
    }

    private void AddLang()
    {
        MenuButtonCreateAd.Text = L1.ADDNEW;
        MenuButtonMyAds.Text = L1.MANAGE;

        LangAdder.Add(TitleRequiredFieldValidator, L1.REQ_TITLE, true);
        LangAdder.Add(DescriptionRequiredFieldValidator, L1.REQ_DESC, true);
        LangAdder.Add(UrlRequiredValidator, L1.REQ_URL, true);
        LangAdder.Add(TagsRequiredFieldValidator, U6002.MUSTADDTAGS, true);
        CreateInTextAdButton.Text = L1.BUY; 
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



    protected void CreateInTextAdButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (Page.IsValid)
            {
                AppSettings.DemoCheck();

                ErrorMessagePanel.Visible = SuccessMessagePanel.Visible = false;
                var pack = new InTextAdvertPack(Convert.ToInt32(PacksDDL.SelectedValue));
                string url = UserUrlsRadioButtonList.SelectedItem.Text.Trim();
                string title = InputChecker.HtmlEncode(TitleTextBox.Text, TitleTextBox.MaxLength, L1.TITLE);
                string description = InputChecker.HtmlEncode(DescriptionTextBox.Text, DescriptionTextBox.MaxLength, L1.DESCRIPTION);
                List<string> tags = TagsTextBox.Text.Replace(" ", string.Empty)
                                                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                    .Select(tag => InputChecker.HtmlEncode(tag, 20, U6002.TAG))
                                                    .ToList();

                InTextAdvert.Buy(Member.Current, pack, title, description, url, TargetBalanceRadioButtonList.TargetBalance, tags);

                SuccessMessagePanel.Visible = true;
                SuccessMessage.Text = U6003.CAMPAIGNCREATED;
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
        }
    }

    protected void PacksDDL_Init(object sender, EventArgs e)
    {
        var packs = InTextAdvertPack.GetActive();
        if (packs.Count > 0)
        {
            var listItems = new List<ListItem>();
            for (int i = 0; i < packs.Count; i++)
            {
                listItems.Add(
                   new ListItem
                   {
                       Value = packs[i].Id.ToString(),
                       Text = string.Format("{0} {1} ({2} {3}) - {4}", packs[i].Clicks, L1.CLICKS, packs[i].MaxNumberOfTags, U6002.TAGS, packs[i].Price)
                   });
            }

            PacksDDL.Controls.Clear();
            PacksDDL.Items.AddRange(listItems.ToArray());
            PacksDDL.SelectedIndex = 0;
            maxNumberOfTags = packs[0].MaxNumberOfTags;
        }
    }

    protected void MyInTextAdsGridView_DataBound(object sender, EventArgs e)
    {
        MyInTextAdsGridView.Columns[1].HeaderText = L1.TITLE;
        MyInTextAdsGridView.Columns[2].HeaderText = L1.DESCRIPTION;
        MyInTextAdsGridView.Columns[3].HeaderText = "URL";
        MyInTextAdsGridView.Columns[4].HeaderText = L1.PROGRESS;
        MyInTextAdsGridView.Columns[5].HeaderText = U6002.TAGS;
        MyInTextAdsGridView.Columns[6].HeaderText = L1.STATUS;
    }

    protected void MyInTextAdsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            var inTextAd = new InTextAdvert(Convert.ToInt32(e.Row.Cells[0].Text));

            e.Row.Cells[4].Text = HtmlCreator.GenerateCPAAdProgressHTML(inTextAd.ClicksReceived, inTextAd.ClicksBought, L1.CLICKS);

            GridView tagsGridView = e.Row.FindControl("MyInTextAdsTagsGridView") as GridView;

            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                tagsGridView.DataSource = bridge.Instance.ExecuteRawCommandToDataTable(
                    @"SELECT Tag 
                    FROM InTextAdvertsTags 
                    WHERE InTextAdvertId = " + inTextAd.Id);
            }
            tagsGridView.DataBind();
        }
    }

    protected void MyInTextAdsGridView_DataSource_Init(object sender, EventArgs e)
    {
        MyInTextAdsGridView_DataSource.SelectCommand =
            string.Format(@"SELECT * FROM InTextAdverts WHERE UserId = {0}", Member.CurrentId);
    }

    protected void MyAdsView_Activate(object sender, EventArgs e)
    {
        MyInTextAdsGridView.DataBind();
    }

    protected void CreateAdView_Activate(object sender, EventArgs e)
    {
        bool packsConfigured = InTextAdvertPack.AreAnyActive();
        bool userHasActiveUrls = UserUrl.UserHasActive(Member.CurrentId);
        CreateAdvertisementPlaceHolder.Visible = packsConfigured && userHasActiveUrls;

        if (!CreateAdvertisementPlaceHolder.Visible)
        {
            NewAdUnavailable.Visible = true;
            NewAdUnavailable.HeaderText = U6000.NEWADVERTUNAVAILABLEHEADER;
            string reason = !packsConfigured ? U6000.NEWADVERTUNAVAILABLEREASON : U6002.PLEASEADDURLSFIRST;

            NewAdUnavailable.Reason = reason;
        }
        else
            NewAdUnavailable.Visible = false;
    }

    protected void UserUrlsRadioButtonList_Init(object sender, EventArgs e)
    {
        var urls = UserUrl.GetActive(Member.CurrentId);

        foreach (var url in urls)
        {
            UserUrlsRadioButtonList.Items.Add(new ListItem(url.Url, url.Id.ToString()));
        }
        UserUrlsRadioButtonList.DataBind();
    }

    protected void UrlRequiredValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = UserUrlsRadioButtonList.Items.Count > 0 && UserUrlsRadioButtonList.SelectedIndex != -1;
    }

    protected void PacksDDL_SelectedIndexChanged(object sender, EventArgs e)
    {
        var pack = new InTextAdvertPack(Convert.ToInt32(PacksDDL.SelectedValue));
        maxNumberOfTags = pack.MaxNumberOfTags;
        TagsTextBox.Attributes.Add("data-max", maxNumberOfTags.ToString());
        TagsTextBox.Attributes.Add("data-message", string.Format(U6002.TOOMANYTAGS, maxNumberOfTags));
    }
}