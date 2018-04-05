using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Advertising;
using System.Text;

public partial class Page_advert_SurfAds : System.Web.UI.Page
{
    new Member User;
    public string jsSelectAllCode;
    string validURL;
    SurfAdsPack surfAdsPack;

    protected void Page_Load(object sender, EventArgs e)
    {
        User = Member.CurrentInCache;
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.AdvertSurfAdsEnabled && User.IsAdvertiser);

        RegularExpressionValidator2.ValidationExpression = RegexExpressions.AdWebsiteUrl;
        Form.Action = Request.RawUrl;

        if (!Page.IsPostBack)
        {
            BindDataToCampaignsDDL();
            BindDataToPacksDDL();

            Button1.Text = L1.BUY + " " + U5004.SURFADS;
            Button2.Text = U4200.ADVERTISEMENTS;
            Button3.Text = L1.MANAGE;

            NewAdPackAdvert = null;
            StartPageDescriptionLiteral.Visible = StartPagePlaceHolder.Visible = AppSettings.RevShare.AdPack.IsStartPageEnabled;
            if (User.Name == AppSettings.RevShare.AdminUsername)
            {
                AdminLiteral.Visible = true;
                AdminLiteral.Text = @"<br/><br/><b>Administrator's advertisements are automatically approved and will appear 
                    in the surf queue if there are not enough ads created by users (no need to buy Surf Ads).</b>";
                StartPageDescriptionLiteral.Text = "<br/>Administrator's Start Pages will not be considered.";
            }
            else
            {
                AdminLiteral.Visible = false;
                StartPageDescriptionLiteral.Text = "<br/>" + U5001.STARTPAGEDESCRIPTION;
            }
            PurchaseStartPageCheckBox.Text = " " + L1.BUY + " " + U5001.STARTPAGE + "<span style='font-size:smaller'> (" + U5003.STARTPAGEINFO + ")</span>";

            surfAdsPack = new SurfAdsPack(Convert.ToInt32(PacksDropDown.SelectedValue));

            PackPriceLabel.Text = surfAdsPack.Price.ToClearString();
        }
        ScriptManager.RegisterStartupScript(MultiViewUpdatePanel, GetType(), "TBChanged", "TBChanged();", true);

        LangAdder.Add(RegularExpressionValidator1, L1.ER_INVALIDTITLE);
        LangAdder.Add(UserNameRequired, L1.REQ_TITLE);
        HintAdder.Add(Title, L1.H_TITLE);
        LangAdder.Add(CorrectEmailRequired, L1.ER_BADDESC);
        HintAdder.Add(Description, L1.H_DESCRIPTION);
        LangAdder.Add(PasswordRequired, L1.REQ_URL);
        LangAdder.Add(RegularExpressionValidator2, L1.ER_BADURL);
        HintAdder.Add(URL, L1.H_URL);

        ChangeAdvertInfoPlaceholder.Visible = AppSettings.RevShare.AdPack.EnableAdvertChange;

        //JS changes
        URL.Attributes.Add("onfocus", "if (getElementById('" + URL.ClientID + "').value == 'http://') getElementById('" + URL.ClientID + "').value = ''");

        bool hasAvailableAdverts = AdPackManager.HasAvailableAdverts(User.Id);
        DropDownAdsPlaceHolder.Visible = PurchaseButton.Visible = DropDownAdsPlaceHolder2.Visible = ChangeCampaignButton.Visible = hasAvailableAdverts;
        RedirectToNewAdsButton2.Visible = !hasAvailableAdverts;
        PurchaseButton.Text = L1.BUY;
        CreateAdButton.Text = U4200.CREATE;
        AdPacksAdGridView.EmptyDataText = U4200.NOADS;

        AdPacksAdGridView.Columns[2].HeaderText = L1.TITLE;
        AdPacksAdGridView.Columns[3].HeaderText = L1.DESCRIPTION;
        AdPacksAdGridView.Columns[4].HeaderText = U6008.ADDEDAS;
        AdPacksAdGridView.Columns[5].HeaderText = L1.STATUS;
        AdPacksAdGridView.Columns[6].HeaderText = U6008.TARGETURL;

        StartPageCalendarPanel.Visible = PurchaseStartPageCheckBox.Checked;
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        SPanel.Visible = false;
        EPanel.Visible = false;

        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        //Enables setting the style of MenuButton when redirected from another card
        if (TheButton.ID == "RedirectToNewAdsButton" || TheButton.ID == "RedirectToNewAdsButton2")
            TheButton = Button2;

        //Redirect to Groups list
        if (viewIndex == 4)
            Response.Redirect("~/user/advert/groups/list.aspx");

        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";
    }

    private void BindDataToCampaignsDDL()
    {
        var list = new Dictionary<string, string>();
        var campaigns = AdPackManager.GetUsersAdverts(User.Id);

        for (int i = 0; i < campaigns.Count; i++)
        {
            list.Add(campaigns[i].Id.ToString(), campaigns[i].Title);
        }

        CampaignsDropDown.DataSource = list;
        CampaignsDropDown.DataTextField = "Value";
        CampaignsDropDown.DataValueField = "Key";
        CampaignsDropDown.DataBind();

        ddlCampaigns2.DataSource = list;
        ddlCampaigns2.DataTextField = "Value";
        ddlCampaigns2.DataValueField = "Key";
        ddlCampaigns2.DataBind();
    }

    private void BindDataToPacksDDL()
    {
        PacksDropDown.Items.Clear();
        var availablePacks = SurfAdsPack.GetAllActivePacks();

        for (int i = 0; i < availablePacks.Count; i++)
        {
            string itemValue = availablePacks[i].Id.ToString();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}: {1}, {2}: {3}s", L1.VIEWSBIG, availablePacks[i].Clicks, U5001.DISPLAYTIME, availablePacks[i].DisplayTime);
            string itemString = sb.ToString();

            ListItem item = new ListItem(itemString, itemValue);
            PacksDropDown.Items.Insert(i, item);
            PacksDropDown.SelectedIndex = 0;
        }
    }

    protected void PurchaseButton_Click(object sender, EventArgs e)
    {
        SPanel.Visible = false;
        EPanel.Visible = false;

        try
        {
            User = Member.Current;

            int advertId = Convert.ToInt32(CampaignsDropDown.SelectedValue);
            surfAdsPack = new SurfAdsPack(Convert.ToInt32(PacksDropDown.SelectedValue));

            SurfAdsManager.BuySurfAds(advertId, User, surfAdsPack, TargetBalanceRadioButtonList.TargetBalance);

            SPanel.Visible = true;

            SText.Text = U5004.BUYSURFADSSUCCESS;

            SurfAdsStatsGridView.DataBind();
        }
        catch (MsgException ex)
        {
            EPanel.Visible = true;
            EText.Text = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }

    #region CREATE ADPACKADVERT
    protected void CreateAdButton_Click(object sender, EventArgs e)
    {
        SPanel.Visible = false;
        EPanel.Visible = false;

        try
        {
            if (URL.Enabled)
                throw new MsgException(U4200.CHECKURL);

            NewAdPackAdvert.Title = InputChecker.HtmlEncode(Title.Text, Title.MaxLength, L1.TITLE);
            NewAdPackAdvert.Description = InputChecker.HtmlEncode(Description.Text, Description.MaxLength, L1.DESCRIPTION);

            NewAdPackAdvert.TargetUrl = URL.Text;

            Member admin = new Member(AppSettings.RevShare.AdminUsername);

            NewAdPackAdvert.CreatorUserId = User.Id;

            //No need to approve admin's adverts
            if (NewAdPackAdvert.CreatorUserId == admin.Id)
                NewAdPackAdvert.Status = AdvertStatus.Active;
            else
                NewAdPackAdvert.Status = AdvertStatusExtensions.GetStartingStatus();

            NewAdPackAdvert.AddedAsType = PurchaseOption.Features.SurfAd;

            UrlVerifier.Check(UrlCreator.ParseUrl(URL.Text));

            if (AppSettings.RevShare.AdPack.IsStartPageEnabled && PurchaseStartPageCheckBox.Checked)
            {
                if (AdPackManager.GetNumberOfStartPagesPurchasedForDay(StartPageDateCalendar.SelectedDate) > 0)
                    throw new MsgException("You can't buy a Start Page for a selected date");

                User = Member.Current;
                Money startPagePrice = AppSettings.RevShare.AdPack.StartPagePrice;
                if (startPagePrice > User.PurchaseBalance)
                    throw new MsgException(L1.NOTENOUGHFUNDS);

                NewAdPackAdvert.StartPageDate = StartPageDateCalendar.SelectedDate;
                User.SubtractFromPurchaseBalance(startPagePrice, "Start Page");
                User.SaveBalances();

                //Add to profit sources
                PoolDistributionManager.AddProfit(ProfitSource.StartPage, startPagePrice);
            }

            NewAdPackAdvert.Save();

            //Clear all
            ClearAll();
            BindDataToCampaignsDDL();

            //Display info
            SPanel.Visible = true;
            if (User.Name == AppSettings.RevShare.AdminUsername)
                SText.Text = U3501.ADCREATED;
            else
                SText.Text = U4200.ADAWAITSAPPROVAL;

            AdPacksAdGridView.DataBind();

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

    protected void ClearAll()
    {
        //Clear all
        Title.Text = "";
        Description.Text = "";
        URL.Text = "";
    }

    //Creatning new campaign
    private AdPacksAdvert _newAdPackAdvert;
    protected AdPacksAdvert NewAdPackAdvert
    {
        get
        {
            if (_newAdPackAdvert == null)

                if (Session["NewAdvert"] is AdPacksAdvert)
                    _newAdPackAdvert = Session["NewAdvert"] as AdPacksAdvert;
                else
                {
                    NewAdPackAdvert = new AdPacksAdvert();
                    Session["NewAdvert"] = _newAdPackAdvert = NewAdPackAdvert;
                }
            return _newAdPackAdvert;
        }


        set { Session["NewAdvert"] = _newAdPackAdvert = value; }
    }
    #endregion

    #region ADPACKS GRIDVIEW
    protected void AdPacksAdGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //Shorten url [6]
            if (e.Row.Cells[6].Text.Length > 19)
                e.Row.Cells[6].Text = e.Row.Cells[6].Text.Substring(0, 16) + "...";

            //Status [5]
            AdvertStatus Status = (AdvertStatus)Convert.ToInt32(e.Row.Cells[5].Text);
            e.Row.Cells[5].Text = HtmlCreator.GetColoredStatus(Status);

            //Added as [4]
            PurchaseOption.Features AddedAsType = (PurchaseOption.Features)Convert.ToInt32(e.Row.Cells[4].Text);
            e.Row.Cells[4].Text = AddedAsType.ToString();
        }
    }

    protected void AdPacksAdGridViewDataSource_Init(object sender, EventArgs e)
    {
        AdPacksAdGridViewDataSource.SelectCommand = string.Format(@"SELECT * FROM AdPacksAdverts WHERE CreatorUserId = {0} ORDER BY Status", Member.CurrentId);
    }

    protected void AddNewAdWithURLCheck_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(Request.Params.Get("__EVENTARGUMENT")))
        {
            string argument = Request.Params.Get("__EVENTARGUMENT");

            validURL = Encryption.Decrypt(argument);

            if (validURL.StartsWith("http"))
            {
                URL.Text = validURL;
                URL.Enabled = false;
                CheckURLButton.Visible = false;
            }
        }
    }

    #endregion

    #region ADPACKS STATS GRIDVIEW
    protected void SurfAdsStatsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!AppSettings.RevShare.AdPack.EnableAdvertChange)
                e.Row.Cells[0].ControlStyle.CssClass = "displaynone";

            if (!AppSettings.RevShare.AdPack.IsStartPageEnabled)
                e.Row.Cells[4].ControlStyle.CssClass = "displaynone";

            AdPack adPack = new AdPack(Convert.ToInt32(e.Row.Cells[1].Text));

            e.Row.Cells[2].Text = HtmlCreator.GenerateCPAAdProgressHTML(Convert.ToInt32(e.Row.Cells[2].Text), Convert.ToInt32(adPack.ClicksBought), L1.CLICKS.ToLower());

            if ((e.Row.Cells[4].Text.ToLower() == "&nbsp;"))
                e.Row.Cells[4].Text = "-";
            else
            {
                try
                {
                    DateTime StartPageDate = Convert.ToDateTime(e.Row.Cells[4].Text);
                    if (StartPageDate < DateTime.Now.AddDays(-3))
                    {
                        //We do not show older then 3 days
                        e.Row.Cells[4].Text = "-";
                    }
                    else
                    {
                        e.Row.Cells[4].Text = StartPageDate.ToShortDateString();
                    }
                }
                catch { }
            }
        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            if (!AppSettings.RevShare.AdPack.EnableAdvertChange)
                SurfAdsStatsGridView.Columns[0].HeaderStyle.CssClass = "displaynone";

            if (!AppSettings.RevShare.AdPack.IsStartPageEnabled)
                SurfAdsStatsGridView.Columns[4].HeaderStyle.CssClass = "displaynone";

            SurfAdsStatsGridView.Columns[4].HeaderText = U5001.STARTPAGE;
        }
    }

    protected void SurfAdsStatsGridView_DataSource_Init(object sender, EventArgs e)
    {
        SurfAdsStatsGridView_DataSource.SelectCommand = string.Format(@"SELECT * FROM AdPacks ap LEFT JOIN AdPacksAdverts apa ON apa.Id = ap.AdPacksAdvertId WHERE ap.UserId = {0} AND AdPackTypeId = -1", Member.CurrentId);
    }

    #endregion

    protected void View3_Activate(object sender, EventArgs e)
    {
        SurfAdsStatsGridView.DataBind();
    }

    protected void ChangeCampaignButton_Click(object sender, EventArgs e)
    {
        if (AppSettings.RevShare.AdPack.EnableAdvertChange)
        {
            for (int i = 0; i < SurfAdsStatsGridView.Rows.Count; i++)
            {
                GridViewRow row = SurfAdsStatsGridView.Rows[i];
                bool isChecked = ((CheckBox)row.FindControl("chkSelect")).Checked;

                if (isChecked)
                {
                    int adpackId = Convert.ToInt32(row.Cells[1].Text);
                    AdPack adpack = new AdPack(adpackId);
                    adpack.AdPacksAdvertId = Convert.ToInt32(ddlCampaigns2.SelectedValue);

                    if (!AdPackManager.HasConstantBanner(adpack.AdPacksAdvertId))
                        adpack.TotalConstantBannerImpressions = adpack.ConstantBannerImpressionsBought;
                    if (!AdPackManager.HasNormalBanner(adpack.AdPacksAdvertId))
                        adpack.TotalNormalBannerImpressions = adpack.NormalBannerImpressionsBought;

                    adpack.Save();
                }
            }
            SurfAdsStatsGridView.DataBind();
        }
    }

    protected void StartPageDateCalendar_DayRender(object sender, DayRenderEventArgs e)
    {
        if (e.Day.Date <= DateTime.Now.Date)
        {
            e.Day.IsSelectable = false;
            e.Cell.CssClass = "not-available";
            return;
        }

        bool isFull = AdPackManager.GetNumberOfStartPagesPurchasedForDay(e.Day.Date) > 0;
        if (isFull)
        {
            e.Cell.CssClass = "not-available alert alert-danger";

            e.Day.IsSelectable = false;

        }
        else
        {
            e.Cell.CssClass = "available";
        }

        if (e.Day.IsSelected)
        {
            e.Cell.CssClass = "active";
        }
    }

    protected void PacksDropDown_SelectedIndexChanged(object sender, EventArgs e)
    {
        surfAdsPack = new SurfAdsPack(Convert.ToInt32(PacksDropDown.SelectedValue));
        PackPriceLabel.Text = surfAdsPack.Price.ToClearString();
    }
}
