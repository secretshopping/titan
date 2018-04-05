using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Utils;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using Prem.PTC.Advertising;
using Resources;
using System.Text;
using Titan.Matrix;
using Titan;

public partial class About : System.Web.UI.Page
{
    public string EditId = string.Empty;

    //This page has 2 versions: for logged and not-logged member
    string validURL;

    public enum RequestType
    {
        Create = 0,
        Edit = 1
    }

    public RequestType PageRequest { get; set; }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (AppSettings.PtcAdverts.AdvertPTCPackCashbackEnabled)
            CashbackInfoPlaceHolder.Visible = true;

        if (ViewState["editid"] == null)
            ViewState["editid"] = Request.QueryString["editid"];
        if (ViewState["editid"] != null)
        {
            PageRequest = RequestType.Edit;
        }
        else
            PageRequest = RequestType.Create;

        //Get Packs
        if (!Page.IsPostBack)
        {
            BindDataToDDL();
            BindDataCountriesToDDL();
        }

        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.AdvertisersRoleEnabled && AppSettings.TitanFeatures.AdvertAdsEnabled
            && (!Member.IsLogged || Member.CurrentInCache.IsAdvertiser));
        TitleLiteral.Text = U6003.PTC;
        if (!IsPostBack)
        {
            BindDataCategoriesToDDL();

            if (Member.IsLogged)
                BuyWithPTCCreditsPlaceHolder.Visible = AppSettings.PtcAdverts.PTCCreditsEnabled;

            CaptchaQuestionTexbox.Text = AppSettings.PtcAdverts.PTCDefaultCaptchaQuestion;
            BackgroundColorPlaceHolder.Visible = true;

            if (AppSettings.PtcAdverts.FeedbackCaptchaEnabled)
            {
                CustomCaptchaPlaceHolder.Visible = true;
                CaptchaQuestionTexbox.Visible = CaptchaQuestionCheckBox.Checked;
            }

            StarredAdPlaceHolder.Visible = AppSettings.PtcAdverts.StarredAdsEnabled;
            CategoriesDropDownPlaceHolder.Visible = AppSettings.PtcAdverts.PTCCategoryPolicy == AppSettings.PTCCategoryPolicy.Custom;
            BoxSizePlaceHolder.Visible = true;

            if (PageRequest == RequestType.Edit)
            {
                BindEditWindow();
            }



        }
        RegularExpressionValidator2.ValidationExpression = RegexExpressions.AdWebsiteUrl;

        if (Member.IsLogged)
        {
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
        LangAdder.Add(RegularExpressionValidator1, L1.ER_INVALIDTITLE, true);
        LangAdder.Add(UserNameRequired, L1.REQ_TITLE, true);
        HintAdder.Add(Title, L1.H_TITLE);
        HintAdder.Add(Description, L1.H_DESCRIPTION);
        LangAdder.Add(PasswordRequired, L1.REQ_URL, true);
        LangAdder.Add(RegularExpressionValidator2, L1.ER_BADURL, true);
        HintAdder.Add(URL, L1.H_URL);
        HintAdder.Add(lblGeolocation, L1.GEOHINT);
        LangAdder.Add(lblStarredAd, U5006.STARREDAD);
        HintAdder.Add(lblStarredAd, U5006.STARREDADINFO);
        HintAdder.Add(GeoAgeMax, U3900.LEAVEZERO);
        HintAdder.Add(GeoAgeMin, U3900.LEAVEZERO);
        LangAdder.Add(PTCImageSubmitValidator, U6000.CHOOSEFILE, true);
        LangAdder.Add(PTCImageValidator, U5006.MUSTUPLOADIMAGE, true);

        URL.Attributes.Add("onfocus", "if (getElementById('" + URL.ClientID + "').value == 'http://') getElementById('" + URL.ClientID + "').value = ''");
        chbGeolocation.Attributes.Add("onclick", "ManageGeoEvent();");

        //Read prices
        PriceGeo.Text = AppSettings.PtcAdverts.GeolocationCost.ToString();
        PriceBold.Text = AppSettings.PtcAdverts.FontBoldCost.ToString();
        PriceDesc.Text = AppSettings.PtcAdverts.DescriptionCost.ToString();

        var where = TableHelper.MakeDictionary("IsVisibleByMembers", true);
        var listPacks = TableHelper.SelectRows<PtcAdvertPack>(where);

        if (listPacks.Count != 0)
            RefreshAdvertAndPrice(this, null);

        if (AppSettings.PtcAdverts.PTCImagesEnabled)
            PTCImagePlaceHolder.Visible = true;

        GeolocationPlaceHolder.Visible = AppSettings.PtcAdverts.GeolocationStatus;
    }

    private void BindEditWindow()
    {
        if (!Member.IsLogged)
            return;

        MenuMultiView.SetActiveView(View1);
        int adId = Convert.ToInt32(ViewState["editid"]);
        EditId = "?editid=" + adId.ToString();

        PtcAdvert ad = new PtcAdvert(adId);
        if (ad.AdvertiserUserId != Member.CurrentId || ad.Status == AdvertStatus.Rejected)
            Response.Redirect("~/default.aspx");
        Title.Text = ad.Title;
        URL.Text = ad.TargetUrl;

        if (AppSettings.PtcAdverts.PTCCategoryPolicy == AppSettings.PTCCategoryPolicy.Custom)
        {
            if (PtcAdvertCategory.GetActiveCategories().Any(x => x.Id == ad.CategoryId))
                CategoriesDDL.SelectedValue = ad.CategoryId.ToString();
        }

        var where = TableHelper.MakeDictionary("IsVisibleByMembers", true);

        if (AppSettings.PtcAdverts.PTCCreditsEnabled)
            where["EndMode"] = (int)End.Mode.Clicks;

        var listPacks = TableHelper.SelectRows<PtcAdvertPack>(where);
        if (listPacks.Any(x => x.Id == ad.Pack.Id))
            ddlOptions.SelectedValue = ad.Pack.Id.ToString();

        chbDescription.Checked = !string.IsNullOrEmpty(ad.Description);
        Description.Visible = chbDescription.Checked;

        if (chbDescription.Checked)
            Description.Text = ad.Description;
        else
            Description.Text = string.Empty;

        chbGeolocation.Checked = ad.IsGeolocated;

        if (ad.IsGeolocatedByAge)
        {
            GeoAgeMin.Text = ad.GeolocatedAgeMin.ToString();
            GeoAgeMax.Text = ad.GeolocatedAgeMax.ToString();
        }

        if (ad.IsGeolocatedByGender)
            GeoGenderList.SelectedValue = ((int)ad.GeolocatedGender).ToString();

        chbBold.Checked = ad.HasBoldTitle;
        StarredAdCheckBox.Checked = ad.IsStarredAd;

        var activeColors = PtcAdvertBgColor.GetActiveBgColors();
        if (activeColors.Any(x => x.BgColor == ad.BackgroundColor))
            BgColorsDDL.SelectedValue = activeColors.Where(x => x.BgColor == ad.BackgroundColor).ToList()[0].Id.ToString();

        var freeBgColors = activeColors.Where(x => x.Price == Money.Zero);
        BackgroundColorCheckBox.Checked = !freeBgColors.Any(x => x.BgColor == ad.BackgroundColor);

        CaptchaQuestionCheckBox.Checked = ad.CaptchaQuestion != AppSettings.PtcAdverts.PTCDefaultCaptchaQuestion;
        CaptchaQuestionTexbox.Visible = CaptchaQuestionCheckBox.Checked;
        CaptchaQuestionTexbox.Text = ad.CaptchaQuestion;

        if (ad.ExtraViews != -1)
        {
            BuyWithPTCCreditsPlaceHolder2.Visible = UseExtraViewsCheckBox.Checked = true;
            PTCCreditsTextBox.Text = ad.ExtraViews.ToString();
        }

        if (ad.ImagePath != null)
            PTCImage_Image.ImageUrl = ad.ImagePath;
    }

    private void BindDataToDDL()
    {
        var where = TableHelper.MakeDictionary("IsVisibleByMembers", true);

        if (AppSettings.PtcAdverts.PTCCreditsEnabled)
            where["EndMode"] = (int)End.Mode.Clicks;

        var listPacks = TableHelper.SelectRows<PtcAdvertPack>(where);

        listPacks.Sort(PtcAdvertManager.PackComparision);
        var list = new Dictionary<string, string>();

        foreach (PtcAdvertPack pack in listPacks)
        {
            string ends = L1.NEVER;
            if (pack.Ends.EndMode == End.Mode.Days)
                ends = pack.Ends.Value.ToString() + " " + L1.DAYS;
            else if (pack.Ends.EndMode == End.Mode.Clicks)
                ends = pack.Ends.Value.ToString() + " " + L1.VIEWS;

            Money price;
            PtcAdvert ad = null;

            try
            {
                ad = new PtcAdvert(Convert.ToInt32(ViewState["editid"]));
            }
            catch { }

            if (ad != null && ad.Pack.Id == pack.Id)
            {
                price = ad.PackPrice;
            }
            else
                price = PtcAdvertManager.GetDiscountedPTCPackPrice(pack);

            string value = ends + " (" + pack.DisplayTime.TotalSeconds.ToString() + "s) - " + price.ToString();

            if (AppSettings.PtcAdverts.AdvertPTCPackCashbackEnabled && pack.PTCPackCashBackPercent != 0)
                value += String.Format(" - {0}% cashback", pack.PTCPackCashBackPercent);

            list.Add(pack.Id.ToString(), value);
        }
        ddlOptions.DataSource = list;
        ddlOptions.DataTextField = "Value";
        ddlOptions.DataValueField = "Key";

        ddlOptions.DataBind();
    }


    private void BindDataCountriesToDDL()
    {
        var dictlist = GeolocationUtils.GetCountriesData();

        if (PageRequest == RequestType.Edit)
        {
            var geolocated = new Dictionary<string, string>();

            var ad = new PtcAdvert(Convert.ToInt32(ViewState["editid"]));
            if (ad.IsGeolocatedByCountry)
            {
                var countries = ad.GeolocatedCC.Split(',');
                foreach (var country in countries)
                {
                    if (string.IsNullOrEmpty(country))
                        continue;

                    var countryName = CountryManager.GetCountryName(country);
                    dictlist.Remove(countryName);

                    var geolocatedCountry = GeolocationUtils.GetCountryData(countryName);
                    geolocated.Add(geolocatedCountry.Item1, geolocatedCountry.Item2);
                }
                GeoCountries.DataSource = geolocated;
                GeoCountries.DataTextField = "Value";
                GeoCountries.DataValueField = "Key";
                GeoCountries.DataBind();
            }
        }
        AllCountries.DataSource = dictlist;
        AllCountries.DataTextField = "Value";
        AllCountries.DataValueField = "Key";
        AllCountries.DataBind();
    }

    private void BindDataCategoriesToDDL()
    {
        var activeCategories = PtcAdvertCategory.GetActiveCategories();
        var dictlist = new Dictionary<string, string>();

        foreach (var category in activeCategories)
        {
            dictlist[category.Id.ToString()] = category.Name;
        }
        CategoriesDDL.DataSource = dictlist;
        CategoriesDDL.DataTextField = "Value";
        CategoriesDDL.DataValueField = "Key";
        CategoriesDDL.DataBind();
    }

    protected void BindDataBgColorsToDDL(Object sender, EventArgs e)
    {
        var activeColors = PtcAdvertBgColor.GetActiveBgColors();

        var dictlist = new Dictionary<string, string>();

        foreach (var color in activeColors)
        {
            dictlist[color.Id.ToString()] = color.Price.ToString();
        }
        BgColorsDDL.DataSource = dictlist;
        BgColorsDDL.DataTextField = "Value";
        BgColorsDDL.DataValueField = "Key";
        BgColorsDDL.DataBind();
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        SuccMessagePanel.Visible = false;

        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        if (viewIndex == 0 || PageRequest == RequestType.Edit)
            Response.Redirect("ads.aspx");

        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";

    }

    protected void DirectRefsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //The ad id [1]
            PtcAdvert Ad = new PtcAdvert(Convert.ToInt32(e.Row.Cells[1].Text));

            //Description [23]
            if (string.IsNullOrEmpty(e.Row.Cells[23].Text))
                e.Row.Cells[23].Text = HtmlCreator.GetCheckboxUncheckedImage();
            else
                e.Row.Cells[23].Text = HtmlCreator.GetCheckboxCheckedImage();

            //End mode [17]
            End.Mode Mode = (End.Mode)Convert.ToInt32(e.Row.Cells[17].Text);

            //Status [25]
            AdvertStatus Status = (AdvertStatus)Convert.ToInt32(e.Row.Cells[25].Text);
            e.Row.Cells[25].Text = HtmlCreator.GetColoredStatus(Status);

            //Pack [3]
            AdvertPack Pack = new PtcAdvertPack(Convert.ToInt32(e.Row.Cells[3].Text));

            //Displaytime [18]
            e.Row.Cells[18].Text += "s";

            //Shorten url [4]
            if (e.Row.Cells[4].Text.Length > 18)
                e.Row.Cells[4].Text = e.Row.Cells[4].Text.Substring(0, 15) + "...";

            //TItle [5]
            e.Row.Cells[5].Text = e.Row.Cells[5].Text.Replace("&lt;", "<");
            e.Row.Cells[5].Text = e.Row.Cells[5].Text.Replace("&gt;", ">");

            e.Row.Cells[5].Text = Ad.TargetUrl;

            string starredAd = Ad.IsStarredAd ? "<img src='Images/Misc/star.png' style='width:16px;height:16px;'> " : "";
            string targetUrl = Ad.TargetUrl.Length > 12 ? Ad.TargetUrl.Substring(0, 12) + "..." : Ad.TargetUrl;
            e.Row.Cells[5].Text = starredAd + Ad.Title + "<br/><i>" + targetUrl + "</i>";

            //Progress [11]
            e.Row.Cells[11].Text = HtmlCreator.GenerateAdProgressHTML(Ad).Replace("clicks", L1.CLICKSSMALL).Replace("days", L1.DAYS);

            //PointsEarnedFromViews [12]
            if (AppSettings.PtcAdverts.PTCCreditsEnabled)
                e.Row.Cells[12].Text = string.Format("{0}/~{1}", Ad.PointsEarnedFromViews, Member.CurrentInCache.Membership.PointsYourPTCAdBeingViewed * Ad.Ends.Value);
            else
                e.Row.Cells[12].CssClass = "displaynone";


            //Add % progress [13]
            e.Row.Cells[13].Text = Ad.ProgressInPercent.ToString() + "%";

            //Add total views [14]
            e.Row.Cells[14].Text = e.Row.Cells[15].Text;

            //Geolocation check [24]
            var check = (CheckBox)e.Row.Cells[24].Controls[0];
            if (check.Checked)
                e.Row.Cells[24].Text = HtmlCreator.GetCheckboxCheckedImage();
            else
                e.Row.Cells[24].Text = HtmlCreator.GetCheckboxUncheckedImage();

            // Start[31] Pause[32] Add [33] Remove [34]
            if (Status != AdvertStatus.Paused)
                e.Row.Cells[31].Text = "&nbsp;";

            if (Status != AdvertStatus.Active)
                e.Row.Cells[32].Text = "&nbsp;";

            if (Status != AdvertStatus.Finished)
                e.Row.Cells[33].Text = "&nbsp;";

            if (!Status.CanBeRemoved())
                e.Row.Cells[34].Text = "&nbsp;";

            //Edit button [35]
            ((LinkButton)e.Row.Cells[35].FindControl("ImageButton4")).ToolTip = U5007.EDIT;
            if (Ad.Status == AdvertStatus.Rejected)
                e.Row.Cells[35].Controls.Clear();
        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            //PointsEarnedFromViews [12]
            DirectRefsGridView.Columns[12].HeaderText = string.Format(U5007.EARNED, AppSettings.PointsName);
        }

        if (!AppSettings.PtcAdverts.FeedbackCaptchaEnabled)
        {
            DirectRefsGridView.Columns[28].HeaderStyle.CssClass = "displaynone";
            DirectRefsGridView.Columns[29].HeaderStyle.CssClass = "displaynone";
            DirectRefsGridView.Columns[30].HeaderStyle.CssClass = "displaynone";
            DirectRefsGridView.Columns[28].ItemStyle.CssClass = "displaynone";
            DirectRefsGridView.Columns[29].ItemStyle.CssClass = "displaynone";
            DirectRefsGridView.Columns[30].ItemStyle.CssClass = "displaynone";
        }


        if (!AppSettings.PtcAdverts.PTCCreditsEnabled)
            DirectRefsGridView.Columns[12].HeaderStyle.CssClass = "displaynone";

    }

    protected void GetAdWithPrice(ref PtcAdvert Ad, ref Money AdCost, ref PtcAdvertPack Pack, bool throwExceptions = true)
    {
        try
        {
            ErrorMessagePanel.Visible = false;
            int packId = int.Parse(ddlOptions.SelectedValue);

            //Use Ad Credits only
            if (packId == -1)
            {
                Pack = null;
                if (!UseExtraViewsCheckBox.Checked && throwExceptions)
                    throw new MsgException(U5006.SELECTUSEXTRAVIEWS);
            }
            else
                Pack = new PtcAdvertPack(packId);

            AdCost = Money.Zero;

            if (Pack != null && Ad.Pack.Id != Pack.Id)
            {
                AdCost = PtcAdvertManager.GetDiscountedPTCPackPrice(Pack);
                Ad.PackPrice = AdCost;
            }

            //Set basics
            if (AppSettings.PtcAdverts.CashLinkViewEnabled == AppSettings.PTCViewMode.CashLink)
                Ad.Title = "Cash Link"; //Fixed title for cash links
            else
                Ad.Title = InputChecker.HtmlEncode(Title.Text, Title.MaxLength, L1.TITLE);

            Ad.TargetUrl = URL.Text;

            int categoryId = -1;

            if (AppSettings.PtcAdverts.PTCCategoryPolicy == AppSettings.PTCCategoryPolicy.Custom)            
                int.TryParse(CategoriesDDL.SelectedValue, out categoryId);
            
            Ad.CategoryId = categoryId;

            if (chbDescription.Checked)
            {
                if (Ad.Description == null || Ad.Description == string.Empty)
                    AdCost += AppSettings.PtcAdverts.DescriptionCost;

                Ad.Description = InputChecker.HtmlEncode(Description.Text, Description.MaxLength, L1.DESCRIPTION);
            }
            else
                Ad.Description = string.Empty;

            if (chbBold.Checked)
            {
                if (!Ad.HasBoldTitle)
                    AdCost += AppSettings.PtcAdverts.FontBoldCost;

                Ad.HasBoldTitle = true;
            }

            if (chbGeolocation.Checked)
            {
                if ((Ad.GeolocatedCC == null || Ad.GeolocatedCC == string.Empty) && Ad.GeolocatedAgeMin == 0 && Ad.GeolocatedAgeMax == 0 && Ad.GeolocatedGender == Gender.Null)
                    AdCost += AppSettings.PtcAdverts.GeolocationCost;

                //Now get it from client-state
                var CTable = GeoCountries.Items;
                var geoUList = GeolocationUtils.GeoCountData.Keys;

                StringBuilder sb = new StringBuilder();

                foreach (ListItem item in CTable)
                {
                    if (geoUList.Contains<string>(item.Value))
                    {
                        var countryCode = CountryManager.GetCountryCode(item.Value);
                        if (!string.IsNullOrWhiteSpace(countryCode))
                        {
                            sb.Append(CountryManager.GetCountryCode(item.Value));
                            sb.Append(",");
                        }
                    }
                }

                Ad.GeolocatedCC = sb.ToString().Trim(',');
                Ad.GeolocatedAgeMin = Convert.ToInt32(GeoAgeMin.Text);
                Ad.GeolocatedAgeMax = Convert.ToInt32(GeoAgeMax.Text);
                Ad.GeolocatedGender = (Gender)Convert.ToInt32(GeoGenderList.SelectedValue);
            }
            else
            {
                Ad.GeolocatedCC = string.Empty;
                Ad.GeolocatedAgeMin = Ad.GeolocatedAgeMax = 0;
                Ad.GeolocatedGender = Gender.Null;
            }

            if (BackgroundColorCheckBox.Checked)
            {
                BgColorsDDL.Visible = true;
                var bgColor = new PtcAdvertBgColor(Convert.ToInt32(BgColorsDDL.SelectedValue));
                BgColorsDDL.Attributes.Add("style", string.Format("background-color:{0};color:white;", new PtcAdvertBgColor(Convert.ToInt32(BgColorsDDL.SelectedValue)).BgColor));

                if (Ad.BackgroundColor != bgColor.BgColor)
                {
                    AdCost += bgColor.Price;
                    Ad.BackgroundColor = bgColor.BgColor;
                }
            }

            if (AppSettings.PtcAdverts.FeedbackCaptchaEnabled)            
                Ad.CaptchaQuestion = InputChecker.HtmlEncode(CaptchaQuestionTexbox.Text, CaptchaQuestionTexbox.MaxLength, U6013.YESNOCAPTCHAQUESTION);            

            if (AppSettings.PtcAdverts.StarredAdsEnabled && StarredAdCheckBox.Checked)
            {
                if(!Ad.IsStarredAd)
                    AdCost += AppSettings.PtcAdverts.StarredAdsPrice;

                Ad.IsStarredAd = true;
            }
            
            if (AppSettings.PtcAdverts.PTCImagesEnabled)            
                Ad.ImagePath = PTCImage_Image.DescriptionUrl;
            else
                Ad.ImagePath = null;
        }
        catch (MsgException ex)
        {
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = ex.Message;
        }
    }


    protected void RefreshAdvertAndPrice(object sender, EventArgs e)
    {
        PtcAdvert Ad;
        if (PageRequest == RequestType.Edit)
        {
            int adId = Convert.ToInt32(ViewState["editid"]);
            Ad = new PtcAdvert(adId);
        }
        else
            Ad = new PtcAdvert();

        Money AdCost = Money.Zero;
        PtcAdvertPack Pack = new PtcAdvertPack();

        GetAdWithPrice(ref Ad, ref AdCost, ref Pack, false);

        if (AppSettings.PtcAdverts.CashLinkViewEnabled == AppSettings.PTCViewMode.CashLink)
        {   //CashLink
            titleTr.Visible = false;
            descriptionTr.Visible = false;
            boldTr.Visible = false;
            BuyWithPTCCreditsPlaceHolder.Visible = false;
            Abox1.Visible = true;
        }

        Abox1.IsActive = true;
        Abox1.Object = Ad;
        Abox1.DataBind();

        Money totalCost = AdCost;// - Ad.Price;
        lblPrice.Text = (totalCost < Money.Zero ? Money.Zero : totalCost).ToString();

        if (AppSettings.PtcAdverts.AdvertPTCPackCashbackEnabled)
        {
            if (Pack.PTCPackCashBackPercent != 0)
            {
                Double CashBack = Double.Parse(totalCost.ToClearString()) * ((Double)Pack.PTCPackCashBackPercent / 100.0);
                CashBackLabel.Text = new Money(CashBack).ToString();
            }
            else
                CashBackLabel.Text = U6010.NOCASHBACK;
        }

        if (Pack != null && Pack.Ends.EndMode == End.Mode.Days)
            TotalViewsPlaceHolder.Visible = false;
        else
        {
            TotalViewsPlaceHolder.Visible = true;

            var totalViews = string.Empty;
            var totalViewsInt = 0;

            if (Pack != null && Pack.Ends.EndMode == End.Mode.Clicks)
            {
                totalViews += Pack.Ends.Value;
                totalViewsInt += (int)Pack.Ends.Value;
            }

            if (AppSettings.PtcAdverts.PTCCreditsEnabled && UseExtraViewsCheckBox.Checked)
            {
                int ptcCreditViews = 0;
                if (int.TryParse(PTCCreditsTextBox.Text, out ptcCreditViews))
                {
                    totalViews += " + " + PTCCreditsTextBox.Text;
                    totalViewsInt += ptcCreditViews;
                }
            }

            lblViews.Text = totalViews;
            totalViewsInt = totalViewsInt - Ad.Clicks;

            if (totalViewsInt < 0)
                totalViewsInt = 0;

            lblTotalViews.Text = string.Format("{0} = {1}", U5008.TOTALVIEWSDESC.ToLower(), totalViewsInt);
        }
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
                int oldPackId = 0;

                if (Member.IsLogged)
                    User = Member.Logged(Context);

                if (URL.Enabled)
                    throw new MsgException(U4200.CHECKURL);

                PtcAdvert Ad;
                if (PageRequest == RequestType.Edit)
                {
                    int adId = Convert.ToInt32(ViewState["editid"]);
                    Ad = new PtcAdvert(adId);
                    oldPackId = Ad.Pack.Id;
                }
                else
                    Ad = new PtcAdvert();

                Money AdCost = Money.Zero;
                PtcAdvertPack Pack = new PtcAdvertPack();

                GetAdWithPrice(ref Ad, ref AdCost, ref Pack);

                //Add the ad to the db (for approval)
                Ad.Price = AdCost;
                if (Pack != null)
                    Ad.Pack = Pack;

                if (Member.IsLogged)
                {
                    //Take money and save the user
                    int adViews = 0;
                    if (AppSettings.PtcAdverts.PTCCreditsEnabled && UseExtraViewsCheckBox.Checked)
                    {
                        adViews = Convert.ToInt32(PTCCreditsTextBox.Text);
                        if (adViews < 0)
                            throw new MsgException("Input cannot be negative.");

                        decimal adCreditsSpent = 0;
                        var creditsPerDayOrClick = 0m;
                        if (Pack == null)
                        {
                            if (TitanFeatures.isSatvetErturkmen)
                            {
                                PtcAdvertPack advertPack = TableHelper.GetListFromRawQuery<PtcAdvertPack>("SELECT TOP 1 * FROM PtcAdvertPacks")[0];
                                Ad.Pack = advertPack;
                                Ad.Ends = End.FromClicks(adViews);
                                creditsPerDayOrClick = advertPack.PTCCreditsPerDayOrClick;
                            }
                        }
                        else
                        {
                            Ad.Ends = Ad.Ends.AddValue(adViews);
                            creditsPerDayOrClick = Pack.PTCCreditsPerDayOrClick;
                        }

                        if (PageRequest == RequestType.Edit && Ad.ExtraViews != -1)
                        {
                            adCreditsSpent = (adViews - Ad.ExtraViews) * creditsPerDayOrClick;
                            if (adCreditsSpent < 0)
                                adCreditsSpent = 0;
                        }
                        else
                            adCreditsSpent = adViews * creditsPerDayOrClick;

                        Ad.ExtraViews = adViews;

                        if (adCreditsSpent > Member.Current.PTCCredits)
                            throw new MsgException(U5006.NOTENOUGHADCREDITS);
                        else
                            User.SubstractFromPTCCredits(adCreditsSpent, "PTC advert");
                    }
                    else
                        Ad.ExtraViews = -1;

                    if (PageRequest == RequestType.Edit)                    
                        Ad.IsEdited = true;
                    
                    Ad.TargetBalance = TargetBalanceRadioButtonList.TargetBalance;
                    PurchaseOption.ChargeBalance(User, AdCost, TargetBalanceRadioButtonList.Feature, Ad.TargetBalance, PageRequest == RequestType.Edit ? "PTC update" : "PTC advert");

                    Ad.Advertiser = Advertiser.AsMember(User.Name);
                    Ad.AdvertiserUserId = User.Id;
                    Ad.Status = AdvertStatusExtensions.GetStartingStatus();

                    if (AppSettings.PtcAdverts.AdvertPTCPackCashbackEnabled && Pack.PTCPackCashBackPercent != 0)
                    {
                        Money CashBack = new Money(Double.Parse(AdCost.ToClearString()) * ((Double)Pack.PTCPackCashBackPercent / 100.0));
                        User.AddToTrafficBalance(CashBack, U6010.PTCCASHBACK);
                        User.Save();
                    }
                }
                else
                {
                    Ad.Advertiser = Advertiser.AsStranger(OutEmail.Text);
                    Ad.TargetBalance = PurchaseBalances.PaymentProcessor;
                    Ad.Status = AdvertStatus.Null;
                }

                if (oldPackId != 0 && oldPackId != Ad.Pack.Id)
                    Ad.ResetClicks();

                Ad.Save();

                if (Member.IsLogged)
                {
                    //Achievements trial
                    int UserCurrentCampaigns = TableHelper.CountOf<PtcAdvert>(TableHelper.MakeDictionary("CreatorUsername", User.Name));
                    bool ShouldBeSaved = User.TryToAddAchievements(
                        Prem.PTC.Achievements.Achievement.GetProperAchievements(
                        Prem.PTC.Achievements.AchievementType.AfterAdvertisingPtcCampaigns, UserCurrentCampaigns));

                    if (ShouldBeSaved)
                        User.Save();

                    if (PageRequest == RequestType.Edit)
                        History.AddEdit(User.Name, AdCost, "Advert campaign");
                    else
                    {
                        History.AddPurchase(User.Name, AdCost, "Advert campaign");

                        MatrixBase.TryAddMemberAndCredit(User, AdCost, AdvertType.PTC);
                    }

                    if (Ad.Status != AdvertStatus.WaitingForAcceptance)
                        PtcCrediter.TryToCreditReferrerAfterPurchase(User, AdCost, PageRequest == RequestType.Edit ? "PTC update" : "PTC advert");

                    Title.Text = "";
                    URL.Text = "";
                    chbDescription.Checked = false;
                    chbGeolocation.Checked = false;
                    chbBold.Checked = false;
                    BackgroundColorCheckBox.Checked = false;
                    StarredAdCheckBox.Checked = false;

                    SuccMessagePanel.Visible = true;

                    if (PageRequest == RequestType.Edit)
                        SuccMessage.Text = Ad.Status == AdvertStatus.WaitingForAcceptance ? U6012.EDITEDADAWAITSAPPROVAL : U6012.ADUPDATED;
                    else
                        SuccMessage.Text = Ad.Status == AdvertStatus.WaitingForAcceptance ? U4200.ADAWAITSAPPROVAL : U3501.ADCREATED;
                }
                else
                {
                    //Show buttons
                    PaymentButtons.Visible = true;
                    CreateAdButton.Visible = false;

                    PaymentButtons.Text = GenerateHTMLButtons.GetPaymentButtons(new BuyAdvertButtonGenerator<IPtcAdvertPack>(Ad));
                }

                ViewState["editid"] = null;
            }
            catch (MsgException ex)
            {
                ErrorMessagePanel.Visible = true;
                ErrorMessage.Text = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
        }
    }

    protected void DirectRefsGridView_DataBound(object sender, EventArgs e)
    {
        DirectRefsGridView.Columns[5].HeaderText = string.Format("{0}/URL", L1.TITLE);
    }

    protected void DirectRefsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //We want to obey OnSort and OnChart events
        string[] commands = new string[5] { "start", "stop", "add", "remove", "edit" };

        if (commands.Contains(e.CommandName))
        {
            ErrorMessagePanel2.Visible = false;

            int index = e.GetSelectedRowIndex() % DirectRefsGridView.PageSize;
            GridViewRow row = DirectRefsGridView.Rows[index];
            string AdId = (row.Cells[1].Text.Trim());
            var Ad = new PtcAdvert(Convert.ToInt32(AdId));

            if (e.CommandName == "start")
            {
                //Check Max Campaign limit
                int currentCamapigns = PtcAdvert.GetUserActiveCampaignsCount(Member.CurrentId);

                if (currentCamapigns + 1 > Member.CurrentInCache.Membership.MaxActivePtcCampaignLimit)
                {
                    ErrorMessagePanel2.Visible = true;
                    ErrorMessage2.Text = U5006.CANNOTEXCEED.Replace("%n%", Member.CurrentInCache.Membership.MaxActivePtcCampaignLimit.ToString());
                }
                else
                {
                    Ad.Status = AdvertStatus.Active;
                    Ad.SaveStatus();
                }
            }
            else if (e.CommandName == "stop")
            {
                Ad.Status = AdvertStatus.Paused;
                Ad.SaveStatus();
            }
            else if (e.CommandName == "add")
                Response.Redirect("credits.aspx?id=" + AdId);
            else if (e.CommandName == "remove")
            {
                Ad.Status = AdvertStatus.Deleted;
                Ad.SaveStatus();
            }
            else if (e.CommandName == "edit")
                Response.Redirect("ads.aspx?editid=" + AdId);

            UpdateMaxActiveCampaignsLiteralText();
            DirectRefsGridView.DataBind();
        }
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
                UrlWrapper.Attributes.Add("class", "input-group");
                if (validURL.Contains("youtube.com"))
                    YouTubeImage.Visible = true;
            }
        }
    }

    protected void PTCCreditsTextBox_TextChanged(object sender, EventArgs e)
    {
        decimal pTCCreditsPerDayOrClick;

        if (TitanFeatures.isSatvetErturkmen)
            pTCCreditsPerDayOrClick = (decimal)TableHelper.SelectScalar("SELECT TOP 1 PTCCreditsPerDayOrClick FROM PtcAdvertPacks");
        else
            pTCCreditsPerDayOrClick = new PtcAdvertPack(Convert.ToInt32(ddlOptions.SelectedValue)).PTCCreditsPerDayOrClick;

        try
        {
            int inputViews = Convert.ToInt32(PTCCreditsTextBox.Text);

            decimal PTCCreditsPrice = Convert.ToInt32(PTCCreditsTextBox.Text) * pTCCreditsPerDayOrClick;

            var availablePTCCredits = Member.Current.PTCCredits;
            var availablePTCCreditsString = string.Empty;

            if (availablePTCCredits >= PTCCreditsPrice)
                availablePTCCreditsString = string.Format("<span style={0}>{1}</span>", "'color: #87a131; font-weight: bold;'", availablePTCCredits);

            else
                availablePTCCreditsString = string.Format("<span style={0}>{1}</span>", "'color: red; font-weight: bold;'", availablePTCCredits);

            string extraViews = string.Format(@"<b>{0}</b> {1} = <b>{2}</b> {3} ({4} {5})", inputViews, inputViews == 1 ? U5006.VIEW.ToLower() : L1.VIEWS.ToLower(), PTCCreditsPrice, U5006.ADCREDITS, availablePTCCreditsString, U4200.AVAILABLE);
            AvailablePTCCreditsLiteral.Text = extraViews;

        }
        catch (Exception ex)
        {
            AvailablePTCCreditsLiteral.Text = string.Format("<span style='color: red;'>{0}</span>", U5006.INVALIDINTEGER);
        }
    }

    protected void UseExtraViewsCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        BuyWithPTCCreditsPlaceHolder2.Visible = UseExtraViewsCheckBox.Checked;
        RefreshAdvertAndPrice(this, null);
    }

    protected void StartPageDateCalendar_DayRender(object sender, DayRenderEventArgs e)
    {
        if (e.Day.Date <= DateTime.Now.Date)
        {
            e.Day.IsSelectable = false;
            e.Cell.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFBBBBBB");
            return;
        }

        bool isFull = PtcAdvert.GetNumberOfStartPagesPurchasedForDay(e.Day.Date) > 0;
        if (isFull)
        {
            e.Cell.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFE7D5D5");
            e.Cell.ForeColor = System.Drawing.ColorTranslator.FromHtml("White");

            e.Day.IsSelectable = false;

        }
        else
        {
            e.Cell.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFD0DECE");
            e.Cell.ForeColor = System.Drawing.ColorTranslator.FromHtml("White");
        }

        if (e.Day.IsSelected)
        {
            e.Cell.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFE2E499");
        }
    }

    protected void View1_Activate(object sender, EventArgs e)
    {
        var where = TableHelper.MakeDictionary("IsVisibleByMembers", true);
        var listPacks = TableHelper.SelectRows<PtcAdvertPack>(where);

        if (listPacks.Count == 0)
        {
            AdsPlaceHolder.Visible = false;
            NewAdsWebsiteUnavailable.Visible = true;
            NewAdsWebsiteUnavailable.HeaderText = U6000.NEWADVERTUNAVAILABLEHEADER;
            NewAdsWebsiteUnavailable.Reason = U6000.NEWADVERTUNAVAILABLEREASON;
        }
        else
        {
            AdsPlaceHolder.Visible = true;
            NewAdsWebsiteUnavailable.Visible = false;
            PTCCreditsTextBox_TextChanged(null, null);
        }
    }

    protected void CaptchaQuestionCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        CaptchaQuestionTexbox.Visible = CaptchaQuestionCheckBox.Checked;
        RefreshAdvertAndPrice(this, null);
    }

    protected void PurchaseStartPageCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        RefreshAdvertAndPrice(this, null);
    }

    protected void chbDescription_CheckedChanged(object sender, EventArgs e)
    {
        Description.Visible = false;
        if (chbDescription.Checked)
            Description.Visible = true;

        RefreshAdvertAndPrice(this, null);
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        try
        {
            GeoCountries.Items.Add(new ListItem(AllCountries.SelectedItem.Text, AllCountries.SelectedItem.Value));
            OrderItems(GeoCountries);
            AllCountries.Items.Remove(AllCountries.SelectedItem);
            RefreshAdvertAndPrice(this, null);
        }
        catch (Exception ex) { }
    }

    protected void btnRemove_Click(object sender, EventArgs e)
    {
        try
        {
            AllCountries.Items.Add(new ListItem(GeoCountries.SelectedItem.Text, GeoCountries.SelectedItem.Value));
            OrderItems(AllCountries);
            GeoCountries.Items.Remove(GeoCountries.SelectedItem);
            RefreshAdvertAndPrice(this, null);
        }
        catch (Exception ex) { }
    }

    protected void btnAddAll_Click(object sender, EventArgs e)
    {
        try
        {
            GeoCountries.Items.AddRange(AllCountries.Items.Cast<ListItem>().ToArray());
            OrderItems(GeoCountries);
            AllCountries.Items.Clear();
            RefreshAdvertAndPrice(this, null);
        }
        catch (Exception ex) { }
    }

    protected void btnRemoveAll_Click(object sender, EventArgs e)
    {
        try
        {
            AllCountries.Items.AddRange(GeoCountries.Items.Cast<ListItem>().ToArray());
            OrderItems(AllCountries);
            GeoCountries.Items.Clear();
            RefreshAdvertAndPrice(this, null);
        }
        catch (Exception ex) { }
    }

    private void OrderItems(ListBox list)
    {
        var items = list.Items.Cast<ListItem>().Select(x => x).OrderBy(x => x.Value).ToList();
        list.Items.Clear();
        list.Items.AddRange(items.ToArray());
    }

    protected void BgColorsDDL_DataBound(object sender, EventArgs e)
    {
        for (int i = 0; i < BgColorsDDL.Items.Count; i++)
        {
            var color = new PtcAdvertBgColor(Convert.ToInt32(BgColorsDDL.Items[i].Value));
            BgColorsDDL.Items[i].Attributes.Add("style", string.Format("background-color:{0}", color.BgColor));
        }
        if (BgColorsDDL.Items.Count > 0)
        {
            BgColorsDDL.Attributes.Remove("style");
            BgColorsDDL.Attributes.Add("style", string.Format("background-color:{0};color:white;", new PtcAdvertBgColor(Convert.ToInt32(BgColorsDDL.SelectedValue)).BgColor));
        }
    }

    protected void BgColorsDDL_SelectedIndexChanged(object sender, EventArgs e)
    {
        RefreshAdvertAndPrice(sender, e);
        var color = new PtcAdvertBgColor(Convert.ToInt32(BgColorsDDL.SelectedValue));
        BgColorsDDL.Attributes.Add("style", string.Format("background-color:{0};color:white;", color.BgColor));

    }

    protected void BackgroundColorCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        RefreshAdvertAndPrice(sender, e);
        BgColorsDDL.Visible = BackgroundColorCheckBox.Checked;
    }

    private void UpdateMaxActiveCampaignsLiteralText()
    {
        int activeCampaigns = PtcAdvert.GetUserActiveCampaignsCount(Member.CurrentId);
        MaxActiveCampaignsLiteral.Text = U5007.MAXACTIVEADCAMPAINS + ": " + activeCampaigns
                + "/" + Member.CurrentInCache.Membership.MaxActivePtcCampaignLimit;
    }

    protected void manageView_Activate(object sender, EventArgs e)
    {
        if (!Member.IsLogged)
            MaxActiveCampaignsPlaceHolder.Visible = false;
        else
        {
            MaxActiveCampaignsPlaceHolder.Visible = true;
            UpdateMaxActiveCampaignsLiteralText();
        }
    }

    protected void createPTCImage_UploadSubmit_Click(object sender, EventArgs e)
    {
        var fileName = HashingManager.GenerateMD5(DateTime.Now + "ptcImg");
        const string filePath = "~/Images/b_ads/";

        try
        {
            Banner ptcImage;
            var inputStream = PTCImage_Upload.PostedFile.InputStream;

            if (!Banner.TryFromStream(inputStream, out ptcImage) || ptcImage.Width > ImagesHelper.PTC.ImageWidth ||
                ptcImage.Height > ImagesHelper.PTC.ImageHeight)
                throw new MsgException(string.Format(U6003.INVALIDIMAGEORDIMENSION, ImagesHelper.PTC.ImageWidth, ImagesHelper.PTC.ImageHeight));

            ptcImage.Save(filePath, fileName);
            PTCImage_Image.ImageUrl = Banner.GetTemporaryBannerPath(ptcImage);
            PTCImage_Image.DescriptionUrl = ptcImage.Path;
            PTCImage_Upload.Dispose();
        }
        catch (Exception ex)
        {
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = ex.Message;
        }
    }

    protected void PTCImageSubmitValidator_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = PTCImage_Upload.HasFile;
    }

    protected void PTCImageValidator_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = PTCImage_Image.ImageUrl != "";
    }
}
