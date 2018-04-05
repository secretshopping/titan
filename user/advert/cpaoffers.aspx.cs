using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Utils;
using Prem.PTC.Members;
using Prem.PTC.Referrals;
using Prem.PTC.Advertising;
using Prem.PTC.Offers;
using Prem.PTC.Payments;
using Resources;
using Titan;
using Prem.PTC.Memberships;

public partial class About : System.Web.UI.Page
{
    public string jsSelectAllCode;


    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.AdvertisersRoleEnabled && AppSettings.TitanFeatures.AdvertCPAGPTEnabled && Member.CurrentInCache.IsAdvertiser);

        RegularExpressionValidator2.ValidationExpression = RegexExpressions.AdWebsiteUrl;

        UserName.Text = Member.CurrentName;

        SubLiteral.Text = L1.ADVADSINFO;
        LangAdder.Add(CreateAdButton, L1.SEND);

        //Lang & Hint
        GridView1.EmptyDataText = L1.NODATA;
        LangAdder.Add(Button1, L1.MANAGE);
        LangAdder.Add(Button2, L1.NEWCAMPAIGN);

        LangAdder.Add(PasswordRequired, L1.REQ_URL, true);
        LangAdder.Add(RegularExpressionValidator2, L1.ER_BADURL, true);
        HintAdder.Add(URL, L1.H_URL);
        HintAdder.Add(GeoAgeMax, U3900.LEAVEZERO);
        HintAdder.Add(GeoAgeMin, U3900.LEAVEZERO);
        HintAdder.Add(GeoCities, U3900.CITIESHINT);

        BannerImageLabel.Text = L1.BURL;
        HintAdder.Add(BannerImageLabel, L1.H_BANNERURL);
        HintAdder.Add(lblGeolocation, L1.GEOHINT);
        LangAdder.Add(RequiredFieldValidator1, L1.REQ_TITLE, true);
        LangAdder.Add(RegularExpressionValidator1, L1.ER_INVALIDTITLE, true);
        LangAdder.Add(RequiredFieldValidator2, L1.REQ_DESC, true);
        HintAdder.Add(Title, U5006.CPAGPT1);
        HintAdder.Add(Description, U5006.CPAGPT2);
        LangAdder.Add(RequiredFieldValidator3, L1.DETAILEDALLREQ, true);
        LangAdder.Add(RequiredFieldValidator4, L1.DETAILEDALLREQ, true);
        LangAdder.Add(ExternalSubmissionsMenuButton, U6000.EXTERNALSUBMISSIONS);
        LangAdder.Add(RequiredFieldValidator4, L1.DETAILEDALLREQ, true);
        LangAdder.Add(ImageUploadedValidator, U6000.CHOOSEFILE, true);

        HintAdder.Add(Amount, U5006.CPAGPT4);

        chbGeolocation.Attributes.Add("onclick", "ManageGeoEvent();");

        //JS changes
        URL.Attributes.Add("onfocus", "if (getElementById('" + URL.ClientID + "').value == 'http://') getElementById('" + URL.ClientID + "').value = ''");

        //Get data
        if (!Page.IsPostBack)
        {
            AppSettings.Misc.Reload();
            ExternalSubmissionsMenuButton.Visible = AppSettings.AffiliateNetwork.AffiliateNetworkEnabled;
            BindDataCountriesToDDL();
            MakeCategoriesList();

            int submissions = 0;
            var adlist = TableHelper.SelectRows<CPAOffer>(TableHelper.MakeDictionary("AdvertiserUsername", Member.CurrentName));

            foreach (var ad in adlist)
            {

                //Just numbers
                foreach (var ent in ad.Entries)
                    if (ent.Status == OfferStatus.Pending || ent.Status == OfferStatus.UnderReview)
                        submissions++;
            }

            LangAdder.Add(Button3, U6000.SUBMISSIONS + " (" + submissions + ")");

            if (Request.QueryString["go"] != null && MenuMultiView.ActiveViewIndex == 0)
            {
                MenuMultiView.ActiveViewIndex = 2;

                //Change button style
                foreach (Button b in MenuButtonPlaceHolder.Controls)
                {
                    b.CssClass = "";
                }
                Button3.CssClass = "ViewSelected";
                MenuTitleLabel.Text = Button3.Text;
            }
            BannerUploadByUrlButton.Visible = AppSettings.Site.BannersAddByUrlEnabled;
        }

        ErrorMessagePanel.Visible = false;
    }

    public void MakeCategoriesList()
    {
        var list = new Dictionary<string, string>();
        var values = CPACategory.AllCategories;

        foreach (var elem in values)
        {
            list.Add((elem.Id).ToString(), CPAType.GetText(elem));
        }
        CategoriesList.DataSource = list;
        CategoriesList.DataTextField = "Value";
        CategoriesList.DataValueField = "Key";
        CategoriesList.DataBind();
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
            Response.Redirect("cpaoffers.aspx");

        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";


        Session["YourCampaigns_NewAAABannerAdvert"] = null;
        createBannerAdvertisement_BannerImage.ImageUrl = "";
        URL.Text = "http://";
    }


    protected void DirectRefsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int rowIndex = e.Row.RowIndex % DirectRefsGridView.PageSize;
            int ObjectId = (int)DirectRefsGridView.DataKeys[rowIndex].Value;

            CPAOffer BannerAd = new CPAOffer(ObjectId);

            //Title [0]
            e.Row.Cells[0].Text = (new HtmlString(BannerAd.Title)).ToHtmlString();

            //Progress [4]
            e.Row.Cells[3].Text = HtmlCreator.GenerateCPAAdProgressHTML(BannerAd.CompletedClicks, BannerAd.CreditsBought);

            //category [2]
            e.Row.Cells[1].Text = CPAType.GetText(BannerAd.Category);

            // daily [5]
            if (BannerAd.IsDaily)
                e.Row.Cells[4].Text = HtmlCreator.GetCheckboxCheckedImage();
            else
                e.Row.Cells[4].Text = HtmlCreator.GetCheckboxUncheckedImage();

            //geo [7]
            if (BannerAd.IsGeolocated)
                e.Row.Cells[5].Text = HtmlCreator.GetCheckboxCheckedImage();
            else
                e.Row.Cells[5].Text = HtmlCreator.GetCheckboxUncheckedImage();

            //start[7] stop[8] Remove[9]
            if (BannerAd.Status != AdvertStatus.Paused)
                e.Row.Cells[7].Text = "&nbsp;";

            if (BannerAd.Status != AdvertStatus.Active)
                e.Row.Cells[8].Text = "&nbsp;";

            if (!BannerAd.Status.CanBeRemoved())
                e.Row.Cells[9].Text = "&nbsp;";

            //[7] status
            e.Row.Cells[6].Text = HtmlCreator.GetColoredStatus(BannerAd.Status);
        }
    }

    protected void ImageButton1_Click(object sender, EventArgs e)
    {

    }

    //Creatning new campaign
    private CPAOffer _newBannerAdvert;
    protected CPAOffer NewCPAAdvert
    {
        get
        {
            if (_newBannerAdvert == null)

                if (Session["YourCampaigns_NewAAABannerAdvert"] is CPAOffer)
                    _newBannerAdvert = Session["YourCampaigns_NewAAABannerAdvert"] as CPAOffer;
                else
                {
                    NewCPAAdvert = new CPAOffer();
                    Session["YourCampaigns_NewAAABannerAdvert"] = NewCPAAdvert;
                }
            return _newBannerAdvert;
        }

        set { Session["YourCampaigns_NewAAABannerAdvert"] = _newBannerAdvert = value; }
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
                AppSettings.Misc.Reload();

                Member User = Member.Current;

                int HowManyNeeded = Convert.ToInt32(HowMany.Text);
                Money PPA, AdCost;

                try
                {
                    PPA = Money.Parse(Amount.Text);

                    Decimal CalculatedPercent = (Decimal)(AppSettings.CPAGPT.MoneyTakenFromCPAOffersPercent + 100) / (Decimal)100;
                    CalculatedPercent = CalculatedPercent * HowManyNeeded;

                    AdCost = CalculatedPercent * PPA;

                    if (PPA == new Money(0))
                        throw new MsgException(L1.ERCPA);

                }
                catch (Exception ex)
                {
                    throw new MsgException(L1.ERCPA);
                }

                if (PPA < AppSettings.CPAGPT.MinimalCPAPrice)
                    throw new MsgException("Your PPA can't be less than minimum: " + AppSettings.CPAGPT.MinimalCPAPrice.ToString());

                if (chbGeolocation.Checked)
                {
                    //AdCost += AppSettings.BannerAdverts.GeolocationCost;

                    //Now get it from client-state
                    var countriesSelectedDelimited = Request.Form[GeoCountriesValues.Name].Substring(1);
                    GeolocationUnit unit = GeolocationUnit.ParseFromCountries(countriesSelectedDelimited);

                    //Cities
                    unit.Cities = GeoCities.Text;
                    unit.MinAge = Convert.ToInt32(GeoAgeMin.Text);
                    unit.MaxAge = Convert.ToInt32(GeoAgeMax.Text);
                    unit.Gender = (Gender)Convert.ToInt32(GeoGenderList.SelectedValue);

                    NewCPAAdvert.AddGeolocation(unit);
                }

                NewCPAAdvert.Title = InputChecker.HtmlEncode(Title.Text, 50, L1.TITLE);
                NewCPAAdvert.Description = InputChecker.HtmlEncode(Description.Text, 3800, L1.DESCRIPTION);
                NewCPAAdvert.Category = new CPACategory(Convert.ToInt32(CategoriesList.SelectedValue));

                //Take money and save the user
                var targetBalance = TargetBalanceRadioButtonList.TargetBalance;
                PurchaseOption.ChargeBalance(User, AdCost, TargetBalanceRadioButtonList.Feature, targetBalance, "CPA/GPT advertising");

                NewCPAAdvert.TargetBalance = targetBalance;
                NewCPAAdvert.AdvertiserUsername = (User.Name);
                NewCPAAdvert.Status = AdvertStatusExtensions.GetStartingStatus();

                if (Member.CurrentName == "admin")
                    NewCPAAdvert.Status = AdvertStatus.Paused;

                //Set basics - add to db
                NewCPAAdvert.TargetURL = URL.Text;
                //NewBannerAdvert.Pack = Pack;
                NewCPAAdvert.LoginBoxRequired = LoginIDBox.Checked;
                NewCPAAdvert.EmailBoxRequired = EmailIDBox.Checked;
                NewCPAAdvert.IsDaily = IsDaily.Checked;
                NewCPAAdvert.MaxDailyCredits = 1;
                NewCPAAdvert.CreditsBought = HowManyNeeded;
                NewCPAAdvert.IsFromAutomaticNetwork = false;
                NewCPAAdvert.BaseValue = PPA;
                NewCPAAdvert.DateAdded = DateTime.Now;
                NewCPAAdvert.LastCredited = OffersManager.DateTimeZero;
                NewCPAAdvert.RequiredMembership = Membership.Standard.Id;

                if (AppSettings.Site.Mode == WebsiteMode.GPTWithPointsOnly)
                    NewCPAAdvert.CreditOfferAs = CreditOfferAs.NonCash;
                else
                    NewCPAAdvert.CreditOfferAs = CreditOfferAs.Cash;

                NewCPAAdvert.Save();

                //Add history entry 1
                History.AddPurchase(User.Name, AdCost, "CPA/GPT campaign");

                SuccMessagePanel.Visible = true;
                SuccMessage.Text = U3501.ADCREATED;

                Session["YourCampaigns_NewAAABannerAdvert"] = null;

                Response.Redirect("cpaoffers.aspx");
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
        if (!Page.IsValid) return;
        try
        {
            if (
                !((Banners.TryCreateBannerFromUrl(BannerFileUrlTextBox.Text, out newTemporaryBanner) ||
                   Banner.TryFromStream(createBannerAdvertisement_BannerUpload.PostedFile.InputStream,
                       out newTemporaryBanner)) &&
                  (newTemporaryBanner.Width <= 1200 && newTemporaryBanner.Height <= 300)))
                throw new MsgException(U6000.INVALIDBANNERIMAGEORDIMENSIONS);

            newTemporaryBanner.Save(AppSettings.FolderPaths.BannerAdvertImages);

            //deleteOldImageIfExists();

            NewCPAAdvert.BannerImage = newTemporaryBanner;
            NewCPAAdvert.ImageURL = NewCPAAdvert.BannerImage.Path;

            createBannerAdvertisement_BannerImage.ImageUrl = NewCPAAdvert.BannerImage.Path;

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

    Banner newTemporaryBanner;

    protected void DirectRefsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //We want to obey OnSort and OnChart events
        string[] commands = new string[3] { "start", "stop", "remove" };

        if (commands.Contains(e.CommandName))
        {
            int index = e.GetSelectedRowIndex() % DirectRefsGridView.PageSize;

            GridViewRow row = DirectRefsGridView.Rows[index];
            var Ad = new CPAOffer((int)DirectRefsGridView.DataKeys[index].Value);

            if (e.CommandName == "start")
            {
                Ad.Status = AdvertStatus.Active;
                Ad.Save();

                DirectRefsGridView.DataBind();
            }
            else if (e.CommandName == "stop")
            {
                Ad.Status = AdvertStatus.Paused;
                Ad.Save();

                DirectRefsGridView.DataBind();
            }
            else if (e.CommandName == "remove")
            {
                Ad.Status = AdvertStatus.Deleted;
                Ad.Save();

                DirectRefsGridView.DataBind();
            }
        }
    }

    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //We want to obey OnSort and OnChart events
        string[] commands = new string[3] { "deny", "accept", "under" };

        if (commands.Contains(e.CommandName))
        {
            int index = e.GetSelectedRowIndex() % GridView1.PageSize;
            GridViewRow row = GridView1.Rows[index];
            string AdId = (row.Cells[1].Text);
            var Ad = new OfferRegisterEntry(Convert.ToInt32(AdId));
            Member User = new Member(Ad.Username);

            if (Ad.Status == OfferStatus.Pending || Ad.Status == OfferStatus.UnderReview)
            {

                if (e.CommandName == "accept")
                {
                    CPAManager.AcceptEntryManually(Ad, User);
                    Response.Redirect("cpaoffers.aspx?go=1");
                }
                else if (e.CommandName == "deny")
                {
                    CPAManager.DenyEntryManually(Ad, User);
                    Response.Redirect("cpaoffers.aspx?go=1");
                }
                else if (e.CommandName == "under")
                {
                    CPAManager.PutEntryUnderReview(Ad, User);
                    Response.Redirect("cpaoffers.aspx?go=1");
                }
            }
        }
    }

    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var Entry = new OfferRegisterEntry(Convert.ToInt32(e.Row.Cells[1].Text));

            //title [2]
            e.Row.Cells[2].Text = Entry.Offer.Title;

            //Status [6]
            e.Row.Cells[6].Text = Entry.Status.ToString();
            if (Entry.Status == OfferStatus.Completed)
                e.Row.Cells[6].ForeColor = System.Drawing.Color.Green;
            else if (Entry.Status == OfferStatus.Denied)
                e.Row.Cells[6].ForeColor = System.Drawing.Color.Red;


            //hide under
            if (Entry.Status == OfferStatus.UnderReview)
                e.Row.Cells[8].Visible = false;

            if (Entry.Status == OfferStatus.Completed || Entry.Status == OfferStatus.Denied)
            {
                e.Row.Cells[7].Visible = false;
                e.Row.Cells[8].Visible = false;
                e.Row.Cells[9].Visible = false;
            }
        }
    }

    #region ExternalSubmissionsGridView
    protected void ExternalSubmissionsGridView_DataSource_Init(object sender, EventArgs e)
    {
        ExternalSubmissionsGridView_DataSource.SelectCommand =
            string.Format(@"SELECT * FROM ExternalCpaOfferSubmissions 
                            WHERE OfferId IN 
                                (SELECT Id FROM CPAOffers 
                                 WHERE AdvertiserUsername = '{0}') 
                                 AND (Status = {1} OR Status = {2} OR Status = {3} OR Status = {4}) 
                                 ORDER BY Status DESC",
                                 Member.CurrentName, (int)OfferStatus.Completed, (int)OfferStatus.Denied,
                                 (int)OfferStatus.Pending, (int)OfferStatus.UnderReview);
    }

    protected void ExternalSubmissionsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var submission = new ExternalCpaOfferSubmission(Convert.ToInt32(e.Row.Cells[1].Text));

            //title [2]
            e.Row.Cells[2].Text = submission.Offer.Title;

            //Status [6]
            e.Row.Cells[6].Text = submission.Status.ToString();
            if (submission.Status == OfferStatus.Completed)
                e.Row.Cells[6].ForeColor = System.Drawing.Color.Green;
            else if (submission.Status == OfferStatus.Denied)
                e.Row.Cells[6].ForeColor = System.Drawing.Color.Red;


            //hide under
            if (submission.Status == OfferStatus.UnderReview)
                e.Row.Cells[8].Visible = false;

            if (submission.Status == OfferStatus.Completed || submission.Status == OfferStatus.Denied)
            {
                e.Row.Cells[7].Visible = false;
                e.Row.Cells[8].Visible = false;
                e.Row.Cells[9].Visible = false;
            }
        }
    }

    protected void ImageSubmitValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = createBannerAdvertisement_BannerUpload.HasFile || !string.IsNullOrEmpty(BannerFileUrlTextBox.Text);
    }

    protected void ExternalSubmissionsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //We want to obey OnSort and OnChart events
        string[] commands = new string[3] { "deny", "accept", "under" };

        if (commands.Contains(e.CommandName))
        {
            int index = e.GetSelectedRowIndex() % ExternalSubmissionsGridView.PageSize;
            GridViewRow row = ExternalSubmissionsGridView.Rows[index];
            int id = Convert.ToInt32(row.Cells[1].Text);
            var submission = new ExternalCpaOfferSubmission(id);
            

            if (submission.Status == OfferStatus.Pending || submission.Status == OfferStatus.UnderReview)
            {

                if (e.CommandName == "accept")
                {
                    submission.Accept();


                }
                else if (e.CommandName == "deny")
                {
                    submission.Deny();
                }
                else if (e.CommandName == "under")
                {
                    submission.SetUnderReview();
                }

                ExternalSubmissionsGridView.DataBind();
            }
        }
    }

    #endregion
}
