using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Utils;
using Prem.PTC.Members;
using Prem.PTC.Referrals;
using Prem.PTC.Advertising;
using Prem.PTC.Payments;
using Resources;
using Titan;
using Titan.Matrix;

public partial class About : System.Web.UI.Page
{
    public string jsSelectAllCode;

    //This page has 2 versions: for logged and not-logged member

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.AdvertisersRoleEnabled && AppSettings.TitanFeatures.AdvertBannersEnabled
                                         && (!Member.IsLogged || Member.CurrentInCache.IsAdvertiser));

        RegularExpressionValidator2.ValidationExpression = RegexExpressions.AdWebsiteUrl;

        if (Member.IsLogged)
        {
            if (AppSettings.BannerAdverts.AdvertisingPolicy == BannerPolicy.BannerBidding)
                Response.Redirect("~/user/advert/bannersb.aspx");

            UserName.Text = Member.CurrentName;

            SubLiteral.Text = L1.ADVADSINFO;
            LangAdder.Add(CreateAdButton, L1.SEND);
        }
        else
        {
            UserName.Text = "somerandomstring123456";

            //Disable menu & proper page
            TitanViewPagePanel.Visible = false;
            MenuMultiView.ActiveViewIndex = 1;

            //Fix CSS
            ClientScript.RegisterStartupScript(this.GetType(), "key1", "<script>fixCSS();</script>");

            SubLiteral.Text = L1.OUTADSINFO;
            LangAdder.Add(CreateAdButton, U3000.PAY);

            //Email field
            OutEmailPlaceHolder.Visible = true;
            LangAdder.Add(OutEmailRegularExpressionValidator, L1.ER_BADEMAILFORMAT, true);
            LangAdder.Add(OutEmailRequiredFieldValidator, L1.REG_REQ_EMAIL, true);
            HintAdder.Add(OutEmail, L1.OUTEMAILINFO);

            Master.HideSidebars();
        }

        //Lang & Hint
        LangAdder.Add(Button1, L1.MANAGE);
        LangAdder.Add(Button2, L1.NEWCAMPAIGN);
        LangAdder.Add(PasswordRequired, L1.REQ_URL, true);
        LangAdder.Add(RegularExpressionValidator2, L1.ER_BADURL, true);
        HintAdder.Add(URL, L1.H_URL);
        BannerImageLabel.Text = L1.BURL;
        HintAdder.Add(lblGeolocation, L1.GEOHINT);
        LangAdder.Add(createBannerAdvertisement_BannerUploadValidCustomValidator, U6000.CHOOSEFILE, true);
        LangAdder.Add(createBannerAdvertisement_BannerUploadSelectedCustomValidator, L1.ER_BANNERNOTSELECTED, true);

        GeolocationPlaceHolder.Visible = AppSettings.BannerAdverts.GeoloactionEnabled;
        PriceGeo.Text = AppSettings.BannerAdverts.GeolocationCost.ToString();
        chbGeolocation.Attributes.Add("onclick", "updatePrice(); ManageGeoEvent();");
        ddlOptions.Attributes.Add("onchange", "updatePrice();");

        //JS changes
        URL.Attributes.Add("onfocus",
            "if (getElementById('" + URL.ClientID + "').value == 'http://') getElementById('" + URL.ClientID +
            "').value = ''");

        //Get data
        if (!Page.IsPostBack)
        {
            BannerTypeRadioButtonList.Items.Clear();
            BannerTypeRadioButtonList.Items.AddRange(BannerAdvertDimensions.GetActiveItems());
            if (BannerAdvertDimensions.GetActive().Count > 0)
                BindDataToDDL(BannerAdvertDimensions.GetActive()[0]);
            BindDataCountriesToDDL();
            BannerUploadByUrlButton.Visible = AppSettings.Site.BannersAddByUrlEnabled;
        }
        ErrorMessagePanel.Visible = false;
    }

    private void BindDataToDDL(BannerAdvertDimensions dimensions)
    {
        var whereDict = TableHelper.MakeDictionary("IsVisibleByMembers", true);
        whereDict.Add("Type", dimensions.Id);
        var listPacks = TableHelper.SelectRows<BannerAdvertPack>(whereDict);

        var list = new Dictionary<string, string>();
        foreach (BannerAdvertPack pack in listPacks)
        {
            if (pack.Status == UniversalStatus.Active)
            {
                string ends = L1.NEVER;
                if (pack.Ends.EndMode == End.Mode.Days)
                    ends = pack.Ends.Value.ToString() + " " + L1.DAYS;
                else if (pack.Ends.EndMode == End.Mode.Clicks)
                {
                    if (AppSettings.BannerAdverts.ImpressionsEnabled)
                        ends = pack.Ends.Value.ToString() + " " + L1.IMPRESSIONSSMALL;
                    else
                        ends = pack.Ends.Value.ToString() + " " + L1.CLICKSSMALL;
                }

                list.Add(pack.Id.ToString(), ends + " - " + pack.Price.ToString());
            }
        }
        ddlOptions.DataSource = list;
        ddlOptions.DataTextField = "Value";
        ddlOptions.DataValueField = "Key";
        ddlOptions.DataBind();
    }

    private void BindDataCountriesToDDL()
    {
        AllCountries.DataSource = GeolocationUtils.GetCountriesData();
        AllCountries.DataTextField = "Value";
        AllCountries.DataValueField = "Key";
        AllCountries.DataBind();
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        SuccMessagePanel.Visible = false;

        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        if (viewIndex == 0)
            Response.Redirect("banners.aspx");

        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";


        Session["YourCampaigns_NewBannerAdvert"] = null;
        createBannerAdvertisement_BannerImage.ImageUrl = "";
        URL.Text = "http://";
    }


    protected void DirectRefsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            BannerAdvert BannerAd = new BannerAdvert(Convert.ToInt32(e.Row.Cells[1].Text));

            //Image [4]
            var Imag = new Image();
            Imag.ImageUrl = e.Row.Cells[4].Text;
            Imag.Height = Unit.Percentage(33);

            e.Row.Cells[4].Text = "";
            e.Row.Cells[4].Controls.Add(Imag);

            //Shorten url [3]
            if (e.Row.Cells[3].Text.Length > 19)
                e.Row.Cells[3].Text = e.Row.Cells[3].Text.Substring(0, 16) + "...";

            //End mode [15]
            End.Mode Mode = (End.Mode)Convert.ToInt32(e.Row.Cells[15].Text);

            //Pack [2]
            BannerAdvertPack Pack = new BannerAdvertPack(Convert.ToInt32(e.Row.Cells[2].Text));

            //Status [16]
            AdvertStatus Status = (AdvertStatus)Convert.ToInt32(e.Row.Cells[16].Text);
            e.Row.Cells[16].Text = HtmlCreator.GetColoredStatus(Status);

            //Progress [10]
            e.Row.Cells[10].Text =
                HtmlCreator.GenerateAdProgressHTML(BannerAd).Replace("Clicks", L1.CLICKSSMALL).Replace("days", L1.DAYS);

            if (AppSettings.BannerAdverts.ImpressionsEnabled)
                e.Row.Cells[10].Text = e.Row.Cells[10].Text.Replace(L1.CLICKSSMALL, L1.IMPRESSIONSSMALL);

            //% [11]
            e.Row.Cells[11].Text = BannerAd.ProgressInPercent.ToString() + "%";

            //start [20] stop [21] remove[22]
            if (Status != AdvertStatus.Paused)
                e.Row.Cells[20].Text = "&nbsp;";

            if (Status != AdvertStatus.Active)
                e.Row.Cells[21].Text = "&nbsp;";

            if (!Status.CanBeRemoved())
                e.Row.Cells[22].Text = "&nbsp;";

            //geo [19]

            if (BannerAd.IsGeolocated)
                e.Row.Cells[19].Text = HtmlCreator.GetCheckboxCheckedImage();
            else
                e.Row.Cells[19].Text = HtmlCreator.GetCheckboxUncheckedImage();
        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[13].Text = AppSettings.BannerAdverts.ImpressionsEnabled ? L1.IMPRESSIONS : L1.CLICKS;
        }
    }

    //Creatning new campaign
    private BannerAdvert _newBannerAdvert;
    protected BannerAdvert NewBannerAdvert
    {
        get
        {
            if (_newBannerAdvert == null)

                if (Session["YourCampaigns_NewBannerAdvert"] is BannerAdvert)
                    _newBannerAdvert = Session["YourCampaigns_NewBannerAdvert"] as BannerAdvert;
                else
                {
                    NewBannerAdvert = new BannerAdvert();
                    Session["YourCampaigns_NewBannerAdvert"] = NewBannerAdvert;
                }
            return _newBannerAdvert;
        }

        set { Session["YourCampaigns_NewBannerAdvert"] = _newBannerAdvert = value; }
    }

    protected void CreateAdButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        SuccMessagePanel.Visible = false;

        if (Page.IsValid)
        {
            try
            {
                AppSettings.DemoCheck();
                Member User = null;

                if (Member.IsLogged)
                    User = Member.Logged(Context);

                if (ddlOptions.SelectedItem == null)
                    throw new MsgException(U6002.BANNERPACKERROR);

                BannerAdvertPack Pack = new BannerAdvertPack(Int32.Parse(ddlOptions.SelectedValue));
                Money AdCost = Pack.Price;

                if (chbGeolocation.Checked)
                {
                    AdCost += AppSettings.BannerAdverts.GeolocationCost;

                    //Now get it from client-state
                    var CTable = Request.Form[GeoCountriesValues.Name].Substring(1).Split('#');
                    var geoUList = GeolocationUtils.GeoCountData.Keys;

                    foreach (string s in CTable)
                    {
                        if (geoUList.Contains<string>(s))
                            NewBannerAdvert.BannedCountries.Add(s);
                    }
                }

                if (Member.IsLogged)
                {
                    var targetBalance = TargetBalanceRadioButtonList.TargetBalance;
                    PurchaseOption.ChargeBalance(User, AdCost, TargetBalanceRadioButtonList.Feature, targetBalance, "Banner campaign");

                    NewBannerAdvert.Advertiser = Advertiser.AsMember(User.Name);
                    NewBannerAdvert.Status = AdvertStatusExtensions.GetStartingStatus();
                    NewBannerAdvert.TargetBalance = targetBalance;
                }
                else
                {
                    NewBannerAdvert.Advertiser = Advertiser.AsStranger(OutEmail.Text);
                    NewBannerAdvert.Status = AdvertStatus.Null;
                }                

                //Set basics - add to db
                NewBannerAdvert.TargetUrl = URL.Text;
                NewBannerAdvert.Pack = Pack;
                NewBannerAdvert.Price = AdCost;
                NewBannerAdvert.Save();

                if (Member.IsLogged)
                {
                    if (AppSettings.BannerAdverts.AdvertisingPolicy != BannerPolicy.BannerBidding &&
                        (NewBannerAdvert.Status == AdvertStatus.Active || NewBannerAdvert.Status == AdvertStatus.Paused))
                    {
                        BannerCrediter crediter = new BannerCrediter(User);
                        Money moneyLeftForPools = crediter.CreditReferer(NewBannerAdvert.Price);

                        PoolDistributionManager.AddProfit(ProfitSource.Banners, moneyLeftForPools);
                    }

                    //Achievements trial
                    int UserCurrentCampaigns =
                        TableHelper.CountOf<BannerAdvert>(TableHelper.MakeDictionary("CreatorUsername", User.Name));
                    bool ShouldBeSaved = User.TryToAddAchievements(
                        Prem.PTC.Achievements.Achievement.GetProperAchievements(
                            Prem.PTC.Achievements.AchievementType.AfterAdvertisingBannerCampaigns, UserCurrentCampaigns));

                    if (ShouldBeSaved)
                        User.Save();

                    //Add history entry 1
                    History.AddPurchase(User.Name, AdCost, "Banner campaign");

                    SuccMessagePanel.Visible = true;
                    SuccMessage.Text = U3501.ADCREATED;

                    //Earning stats
                    EarningsStatsManager.Add(EarningsStatsType.Banner, AdCost);

                    MatrixBase.TryAddMemberAndCredit(User, AdCost, AdvertType.Banner);

                    Response.Redirect("banners.aspx");
                }
                else
                {
                    //Show buttons
                    PaymentButtons.Visible = true;
                    CreateAdButton.Visible = false;

                    PaymentButtons.Text =
                        GenerateHTMLButtons.GetPaymentButtons(
                            new BuyAdvertButtonGenerator<IBannerAdvertPack>(NewBannerAdvert));
                }

                Session["YourCampaigns_NewBannerAdvert"] = null;
            }
            catch (MsgException ex)
            {
                ErrorMessagePanel.Visible = true;
                ErrorMessage.Text = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                throw ex;
            }
        }
    }


    protected void createBannerAdvertisement_BannerUploadSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            if (!Page.IsValid) return;
            
            if (!((Banners.TryCreateBannerFromUrl(BannerFileUrlTextBox.Text, out _newTemporaryBanner) ||
                 Banner.TryFromStream(createBannerAdvertisement_BannerUpload.PostedFile.InputStream,
                     out _newTemporaryBanner))
                && (
                BannerAdvertDimensions.Get(_newTemporaryBanner.Width, _newTemporaryBanner.Height) != null &&
                BannerAdvertDimensions.Get(_newTemporaryBanner.Width, _newTemporaryBanner.Height).Id.ToString() == BannerTypeRadioButtonList.SelectedValue)))
                throw new MsgException(U6000.INVALIDBANNERIMAGEORDIMENSIONS);

            _newTemporaryBanner.Save(AppSettings.FolderPaths.BannerAdvertImages);

            //DeleteOldImageIfExists();

            NewBannerAdvert.BannerImage = _newTemporaryBanner;
            createBannerAdvertisement_BannerImage.ImageUrl = NewBannerAdvert.BannerImage.Path;

            NewBannerAdvert.Dimensions = BannerAdvertDimensions.Get(NewBannerAdvert.BannerImage.Width, NewBannerAdvert.BannerImage.Height);

            if (createBannerAdvertisement_BannerUpload.HasFile)
                createBannerAdvertisement_BannerUpload.Dispose();
            if (!string.IsNullOrEmpty(BannerFileUrlTextBox.Text))
                BannerFileUrlTextBox.Text = "";
        }
        catch (Exception ex)
        {
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = ex.Message;
        }
    }

    private Banner _newTemporaryBanner;

    protected void createBannerAdvertisement_BannerUploadValidCustomValidator_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = createBannerAdvertisement_BannerUpload.HasFile || !string.IsNullOrEmpty(BannerFileUrlTextBox.Text);
    }

    protected void createBannerAdvertisement_BannerUploadSelectedCustomValidator_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = NewBannerAdvert.BannerImage != null;
    }

    protected void DirectRefsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //We want to obey OnSort and OnChart events
        string[] commands = new string[3] { "start", "stop", "remove" };

        if (commands.Contains(e.CommandName))
        {
            int index = e.GetSelectedRowIndex() % DirectRefsGridView.PageSize;
            GridViewRow row = DirectRefsGridView.Rows[index];
            string AdId = (row.Cells[1].Text.Trim());
            var Ad = new BannerAdvert(Convert.ToInt32(AdId));

            switch (e.CommandName)
            {
                case "start":
                    Ad.Status = AdvertStatus.Active;
                    break;
                case "stop":
                    Ad.Status = AdvertStatus.Paused;
                    break;
                case "remove":
                    Ad.Status = AdvertStatus.Deleted;
                    break;
            }
            Ad.SaveStatus();
            ShowSuccPanel();
            DirectRefsGridView.DataBind();
            Response.Redirect("~/user/advert/banners.aspx");
        }
    }

    private void ShowSuccPanel()
    {
        SuccPanel2.Visible = true;
        SuccLiteral.Text = L1.OP_SUCCESS + ". " + L1.BANNERDURT.Replace("%n%", "3");
    }
    protected void View1_Activate(object sender, EventArgs e)
    {
        var where = TableHelper.MakeDictionary("Status", 1);
        var listPacks = TableHelper.SelectRows<BannerAdvertDimensions>(where);

        if (listPacks.Count == 0)
        {
            BannersPlaceHolder.Visible = false;
            BannersUnavailable.Visible = true;
            BannersUnavailable.HeaderText = U6000.NEWADVERTUNAVAILABLEHEADER;
            BannersUnavailable.Reason = U6002.NEWBANNERUNAVAILABLEREASON;
        }
        else
        {
            BannersPlaceHolder.Visible = true;
            BannersUnavailable.Visible = false;
        }
    }

    protected void BannerTypeRadioButtonList_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindDataToDDL(new BannerAdvertDimensions(Convert.ToInt32(BannerTypeRadioButtonList.SelectedValue)));
        createBannerAdvertisement_BannerImage.ImageUrl = null;
        createBannerAdvertisement_BannerUpload.Dispose();
        BannerFileUrlTextBox.Text = "";
        NewBannerAdvert = null;
    }
}