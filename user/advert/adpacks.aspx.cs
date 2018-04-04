using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Advertising;
using Prem.PTC.Memberships;
using Prem.PTC.Utils;
using Prem.PTC.Texts;
using Titan.Cryptocurrencies;

public partial class Page_advert_Adpacks : System.Web.UI.Page
{
    new Member User;
    public string jsSelectAllCode;
    string validURL;
    AdPackType adPackType;
    public string editId = string.Empty;
    static bool agreeToS = false;
    private readonly string IsAdPackTypesHiddenViewStateName = "IsAdPackTypesHidden";
    public Cryptocurrency TokenCryptocurrency { get; set; }

    protected bool IsAdPackTypesHidden
    {
        get
        {
            if (ViewState[IsAdPackTypesHiddenViewStateName] != null)
                return (bool)ViewState[IsAdPackTypesHiddenViewStateName];
            return false;
        }
        set
        {
            ViewState[IsAdPackTypesHiddenViewStateName] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        User = Member.CurrentInCache;
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.AdvertAdPacksEnabled && User.IsEarner);

        RegularExpressionValidator2.ValidationExpression = RegexExpressions.AdWebsiteUrl;
        Form.Action = Request.RawUrl;
        TokenCryptocurrency = CryptocurrencyFactory.Get<RippleCryptocurrency>();

        if (!Page.IsPostBack)
        {
            if (Request != null && Request.UrlReferrer != null)
                ViewState["PreviousPageUrl"] = Request.UrlReferrer.ToString();

            var PurchaseOptions = PurchaseOption.Get(PurchaseOption.Features.AdPack);

            CheckIfDirectBuy();

            BindDataToCampaignsDDL();
            BindDataToTypesDDL();

            IsAdPackTypesHidden = AppSettings.RevShare.AdPack.HideAdPackTypesWhenOneEnabled && AdPackManager.GetAdPackActiveTypesCount() <= 1;

            BuyForReferralPlaceHolder.Visible = AppSettings.RevShare.AdPack.BuyAdPacksForReferralsEnabled && User.GetDirectReferralsCount() > 0;
            ReferralsDropDown.Visible = BuyForReferralCheckBox.Checked;

            NewAdPackAdvert = null;
            StartPageDescriptionLiteral.Visible = StartPagePlaceHolder.Visible = AppSettings.RevShare.AdPack.IsStartPageEnabled;

            AdminLiteral.Visible = User.Name == AppSettings.RevShare.AdminUsername ? true : false;

            UpdateWhiteBoxInfo();

            HideContent();
            BannerUploadByUrlButton1.Visible = AppSettings.Site.BannersAddByUrlEnabled;
            BannerUploadByUrlButton2.Visible = AppSettings.Site.BannersAddByUrlEnabled;

            PurchaseViaPurchasePlaceHolder.Visible = 
                (PurchaseOptions.PurchaseBalanceEnabled && !TitanFeatures.PurchaseBalanceDisabled) || 
                (PurchaseOptions.CashBalanceEnabled && AppSettings.Payments.CashBalanceEnabled && !TitanFeatures.PurchaseBalanceDisabled);

            PurchaseButton.Visible = PurchaseOptions.PurchaseBalanceEnabled;
            PurchaseViaCashPlaceHolder.Visible = PurchaseOptions.CashBalanceEnabled;
            PurchaseViaCommissionPlaceHolder.Visible = AppSettings.RevShare.AdPackPurchasesViaCommissionBalanceEnabled;

            //Preparing custom view of purchases with Token Wallet
            if (TokenCryptocurrency.WalletEnabled)
            {
                AdPackTypePlaceHolder.Visible = false;

                CustomPurchaseViaCommissionBalancePlaceHolder.Visible =  AppSettings.RevShare.AdPackPurchasesViaCommissionBalanceEnabled;
                CustomPurchaseViaPurchaseBalancePlaceHolder.Visible = PurchaseOptions.PurchaseBalanceEnabled && !TitanFeatures.PurchaseBalanceDisabled;
                CustomPurchaseViaCashBalancePlaceHolder.Visible = PurchaseOptions.CashBalanceEnabled;
                CustomPurchaseForReferralPlaceHolder.Visible = AppSettings.RevShare.AdPack.BuyAdPacksForReferralsEnabled;

                CustomPurchaseViaERC20TokensButton.Enabled = false;
                CustomPurchaseViaCommissionBalanceButton.Enabled = false;
                CustomPurchaseViaCashBalanceButton.Enabled = false;
                CustomPurchaseViaPurchaseBalanceButton.Enabled = false;

                TOSAgreement.Checked = false;
                Button1.Visible = false;

                if (TitanFeatures.IsTrafficThunder)
                    Button2.Visible = false;
                    
                MenuMultiView.ActiveViewIndex = 3;
                Button4.CssClass = "ViewSelected";
            }

            LangAdders();
        }

        if(TokenCryptocurrency.WalletEnabled)
            TypeAvailableForCustomGroups.Visible = (AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.AutomaticAndCustomGroups
                || AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.CustomGroups) && new AdPackType(Convert.ToInt32(CustomTypesDropDown.SelectedValue)).CustomGroupsEnabled;
        else
        TypeAvailableForCustomGroups.Visible = (AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.AutomaticAndCustomGroups
                || AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.CustomGroups) && new AdPackType(Convert.ToInt32(TypesDropDown.SelectedValue)).CustomGroupsEnabled;

        ScriptManager.RegisterStartupScript(MultiViewUpdatePanel, GetType(), "TBChanged", "TBChanged();", true);
        ScriptManager.RegisterStartupScript(MultiViewUpdatePanel, GetType(), "MoneyTBChanged", "MoneyTBChanged();", true);

        if (AppSettings.RevShare.AdPacksPolicy != AppSettings.AdPacksPolicy.HYIP)
            AdvertChangeWarningPlaceholder.Visible = ChangeAdvertInfoPlaceholder.Visible = AppSettings.RevShare.AdPack.EnableAdvertChange;
        TimeClicksExchangePanel.Visible = AppSettings.RevShare.AdPack.IsTimeClickExchangeEnabled;

        //JS changes
        URL.Attributes.Add("onfocus", "if (getElementById('" + URL.ClientID + "').value == 'http://') getElementById('" + URL.ClientID + "').value = ''");

        bool hasAvailableAdverts = AdPackManager.HasAvailableAdverts(User.Id);
        DropDownAdsPlaceHolder.Visible = DropDownAdsPlaceHolder2.Visible = ChangeCampaignButton.Visible = hasAvailableAdverts;
        RedirectToNewAdsButton2.Visible = !hasAvailableAdverts;


        AdPacksAdGridView.EmptyDataText = U4200.NOADS;

        MyCustomGroupsPlaceHolder.Visible = (AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.CustomGroups
            || AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.AutomaticAndCustomGroups);

        CurrentAutomaticGroupPlaceHolder.Visible = (AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.AutomaticGroups
              || AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.AutomaticAndCustomGroups);

        if (Request.QueryString["editid"] != null)
        {
            BindEditWindow();
            CreateAdButton.Text = L1.SAVE;
            NewAdvertPlaceHolder.Visible = false;
        }
        else
            CreateAdButton.Text = U4200.CREATE;

        StartPageCalendarPanel.Visible = PurchaseStartPageCheckBox.Checked;
        ErrorMessagePanel.Visible = false;
    }

    private void LangAdders()
    {
        LangAdder.Add(Button1, String.Format("{0} {1}", L1.BUY, AppSettings.RevShare.AdPack.AdPackNamePlural));
        LangAdder.Add(Button2, IsAdPackTypesHidden ? U6012.INFORMATIONS : U5004.TYPES);
        LangAdder.Add(Button3, U4200.ADVERTISEMENTS);
        LangAdder.Add(Button4, L1.MANAGE);

        NumberOfPacksTextBox.Text = 1.ToString();

        if (User.Name == AppSettings.RevShare.AdminUsername)
        {
            LangAdder.Add(AdminLiteral, @"<br/><br/><b>Administrator's advertisements are automatically approved and will appear 
                    in the surf queue if there are not enough ads created by users (no need to buy AdPacks).</b>");
            LangAdder.Add(StartPageDescriptionLiteral, "<br/>Administrator's Start Pages will not be considered.");
        }
        else
            LangAdder.Add(StartPageDescriptionLiteral, String.Format("<br/>{0}", U5001.STARTPAGEDESCRIPTION));

        LangAdder.Add(RegularExpressionValidator1, L1.ER_INVALIDTITLE, true);
        LangAdder.Add(UserNameRequired, L1.REQ_TITLE, true);
        HintAdder.Add(Title, L1.H_TITLE);
        LangAdder.Add(CorrectEmailRequired, L1.ER_BADDESC, true);
        HintAdder.Add(Description, L1.H_DESCRIPTION);
        LangAdder.Add(PasswordRequired, L1.REQ_URL, true);
        LangAdder.Add(RegularExpressionValidator2, L1.ER_BADURL, true);
        HintAdder.Add(URL, L1.H_URL);
        LangAdder.Add(createBannerAdvertisement_BannerUploadValidCustomValidator, U6000.CHOOSEFILE, true);
        LangAdder.Add(NormalBannerUploadValidator, U6000.CHOOSEFILE, true);
        LangAdder.Add(createBannerAdvertisement_BannerUploadSelectedCustomValidator2, U4200.NORMALBANNERNOTUPLOADED.Replace("%n%", string.Format("{0}/{1}", AppSettings.RevShare.AdPack.PackNormalBannerWidth, AppSettings.RevShare.AdPack.PackNormalBannerHeight)), true);
        LangAdder.Add(createBannerAdvertisement_BannerUploadSelectedCustomValidator, U4200.CONSTANTBANNERNOTUPLOADED.Replace("%n%", string.Format("{0}/{1}", AppSettings.RevShare.AdPack.PackConstantBannerWidth, AppSettings.RevShare.AdPack.PackConstantBannerHeight)), true);
        LangAdder.Add(AddSecondsButton, U5006.ADDDISPLAYTIME);
        LangAdder.Add(PurchaseStartPageCheckBox, String.Format(" {0} {1}<span style='font-size:smaller'>({2})</span>", L1.BUY, U5001.STARTPAGE, U5003.STARTPAGEINFO));
        LangAdder.Add(PurchaseButton, U6012.PAYVIAPURCHASEBALANCE);
        LangAdder.Add(CustomPurchaseViaPurchaseBalanceButton, U6012.PAYVIAPURCHASEBALANCE);
        LangAdder.Add(CustomPurchaseViaERC20TokensButton, String.Format(U6012.PAYVIAWALLET, TokenCryptocurrency.Code));
        LangAdder.Add(PurchaseViaCashBalanceButton, U6005.PAYVIACASHBALANCE);
        LangAdder.Add(PurchaseViaCommissionBalanceButton, U6012.PAYVIACOMMISSIONBALANCE);
        LangAdder.Add(CustomBuyForReferralCheckBox, U5008.BUYFORREFERRAL);
        
        if (TokenCryptocurrency.WalletEnabled)
        {
            MoneyToInvestTextBox.Text = "1";

            LangAdder.Add(TOSAgreement, U6012.UNDERSTANDANDAGREE);
            LangAdder.Add(CustomAdPacksBuyTitle, String.Format("{0} {1}", L1.BUY, AppSettings.RevShare.AdPack.AdPackName).ToUpper());
            LangAdder.Add(CustomPurchaseViaCommissionBalanceButton, U6012.PAYVIACOMMISSIONBALANCE);
            LangAdder.Add(CustomPurchaseViaCashBalanceButton, U6005.PAYVIACASHBALANCE);

            if (TitanFeatures.IsTrafficThunder)
                LangAdder.Add(CustomAdPacksBuyTitle, "LENDING LAYA PACKAGES");
        }
    }

    #region QueryStringCheckers
    private void CheckIfDirectBuy()
    {
        if (Request.QueryString["DirectBuy"] != null)
            HidePacksListShowBuyPageV2();
    }

    private void CheckIfRedirectAfterAction()
    {
        if (Request.QueryString["BackAfter"] != null)
            Response.Redirect(ViewState["PreviousPageUrl"].ToString());
    }
    #endregion

    private void HideContent()
    {
        AdpackCampaignsInfoDiv.Visible = AdPackManager.HasAdPackWithoutCampaigns(User.Id);
        AricustomisationPlaceHolder.Visible = TitanFeatures.isAri;
        AricustomisationPlaceHolderCheckbox.Visible = TitanFeatures.isAri;
        PurchaseButton.Enabled = !TitanFeatures.isAri;

        BannerPlaceHolder.Visible = DescriptionAdPackListElement1.Visible = DescriptionAdPackListElement2.Visible = !AppSettings.BannerAdverts.HideAllBannersEnabled;

        if (AppSettings.RevShare.AdPacksPolicy == AppSettings.AdPacksPolicy.HYIP)
        {
            Button3.Visible = false;
            AdPacksStatsGridView.Columns[2].Visible = false;
            AdPacksStatsGridView.Columns[3].Visible = false;
            AdPacksStatsGridView.Columns[4].Visible = false;
            AdPacksStatsGridView.Columns[5].Visible = false;
            AdPacksStatsGridView.Columns[7].Visible = false;
            AdPacksStatsGridView.Columns[8].Visible = false;
            AddCampaignDiv.Visible = false;
            DescriptionAdPackListElement0.Visible = false;
            DescriptionAdPackListElement1.Visible = false;
            DescriptionAdPackListElement2.Visible = false;
            ChangeAdvertInfoPlaceholder.Visible = false;
            MainDescriptionP.InnerText = U4200.BUYADPACKDESCRIPTION.Replace("%n%", AppSettings.RevShare.AdPack.AdPackNamePlural);
            TitleDescriptionP.Visible = false;
            DistributionStatus.Visible = false;
        }
        else
        {
            MainDescriptionP.InnerText = U4200.ADPACKMAINDESCRIPTION;
            AdPacksStatsGridView.Columns[9].Visible = false;
        }
    }

    private void BindEditWindow()
    {
        if (!IsPostBack)
        {
            if (!Member.IsLogged)
                return;

            MenuMultiView.SetActiveView(View3);

            //Change button style
            foreach (Button b in MenuButtonPlaceHolder.Controls)
            {
                b.CssClass = "";
            }
            Button3.CssClass = "ViewSelected";
            Button3.Text = U5007.EDIT;
            int adId = Convert.ToInt32(Request.QueryString["editid"]);
            editId = "?editid=" + adId.ToString();

            AdPacksAdvert ad = new AdPacksAdvert(adId);
            AccessManager.RedirectIfDisabled(ad.CreatorUserId == Member.CurrentId);
            NewAdPackAdvert = ad;
            Title.Text = ad.Title;
            Description.Text = ad.Description;
            URL.Text = ad.TargetUrl;
            if (ad.StartPageDate.HasValue && ad.StartPageDate > AppSettings.ServerTime)
            {
                PurchaseStartPageCheckBox.Checked = true;
                StartPageDateCalendar.SelectedDate = ad.StartPageDate.Value;
            }

        }
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        SPanel.Visible = false;
        EPanel.Visible = false;

        var TheButton = (Button)sender;

        //Enables setting the style of MenuButton when redirected from another card
        if (TheButton.ID == "RedirectToNewAdsButton" || TheButton.ID == "RedirectToNewAdsButton2")
            TheButton = Button3;

        int viewIndex = Int32.Parse(TheButton.CommandArgument);

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

    private void BindDataToTypesDDL()
    {
        if(TokenCryptocurrency.WalletEnabled)
            CustomTypesDropDown.Items.Clear();
        else
            TypesDropDown.Items.Clear();

        var availableTypes = AdPackTypeManager.GetAllActiveTypesForUser(User);

        for (int i = 0; i < availableTypes.Count; i++)
        {
            string itemValue = availableTypes[i].Id.ToString();
            string itemString = availableTypes[i].Name;

            ListItem item = new ListItem(itemString, itemValue);

            if (TokenCryptocurrency.WalletEnabled)
            {
                CustomTypesDropDown.Items.Insert(i, item);
                CustomTypesDropDown.SelectedIndex = 0;
            }
            else
            {
                TypesDropDown.Items.Insert(i, item);
                TypesDropDown.SelectedIndex = 0;
            }
        }

        if (availableTypes.Count > 0)
        {
            if (TokenCryptocurrency.WalletEnabled)
                CustomTypesDropDown.Attributes.Add("style", string.Format("background-color:{0};color:white", availableTypes[0].Color));
            else
                TypesDropDown.Attributes.Add("style", string.Format("background-color:{0};color:white", availableTypes[0].Color));
        }
            

        AdPackTypePlaceHolder.Visible = !IsAdPackTypesHidden;
    }

    private void BindDataToReferralsDDL()
    {
        ReferralsDropDown.Items.Clear();
        CustomReferralsDropDownList.Items.Clear();

        var referrals = User.GetDirectReferralsList();

        var boughtAdPacks = AdPacksForOtherUsers.GetListByBuyer(User.Id);

        for (int i = 0; i < referrals.Count; i++)
        {
            string itemValue = referrals[i].Id.ToString();

            int boughtForUser = 0;
            var adPacksForUser = boughtAdPacks.FirstOrDefault(x => x.OwnerId == referrals[i].Id);
            if (adPacksForUser != null)
                boughtForUser = adPacksForUser.Count;
            string itemString = string.Format("{0} (max {1})", referrals[i].Name, AppSettings.RevShare.AdPack.MaxAdPacksForOtherUser - boughtForUser);

            ListItem item = new ListItem(itemString, itemValue);

            ReferralsDropDown.Items.Insert(i, item);
            CustomReferralsDropDownList.Items.Insert(i, item);
        }
        ReferralsDropDown.DataBind();
        CustomReferralsDropDownList.DataBind();
    }

    private void ValidateAdPacksForReferrals()
    {
        var boughtAdPacks = AdPacksForOtherUsers.GetListByBuyer(User.Id);
        var targetUserId = Convert.ToInt32(ReferralsDropDown.SelectedValue);
        var adPacksForUser = boughtAdPacks.FirstOrDefault(x => x.OwnerId == targetUserId);
        var maxAdPacks = AppSettings.RevShare.AdPack.MaxAdPacksForOtherUser;

        if (adPacksForUser != null)
            maxAdPacks = maxAdPacks - adPacksForUser.Count;

        if (NumberOfPacks > maxAdPacks)
            throw new MsgException(string.Format(U5008.CANTBUYADPACKFORTHISUSER, maxAdPacks, AppSettings.RevShare.AdPack.AdPackNamePlural));
    }


    private void BindToCashLinksDropDownList()
    {
    }

    #region AdPack Purchase

    private int AdvertId;
    private int NumberOfPacks;
    private AdPackType AdPackType;
    private Member AdPackOwner;
    private int? AdPackOwnerId;
    private void TryPrepareDataForAdPackPurchase()
    {
        bool isErc20 = TokenCryptocurrency.WalletEnabled;
        AdvertId = -1;
        NumberOfPacks = 0;
        AdPackType = null;
        AdPackOwner = null;
        AdPackOwnerId = null;

    //Get Advert Id
        try
        {
            if (!isErc20)
                AdvertId = Convert.ToInt32(CampaignsDropDown.SelectedValue);
            else
                AdvertId = Convert.ToInt32(CustomCampaignsDropDown.SelectedValue);
        }
        catch (Exception ex) { }

    //Get number of AdPacks 
        try
        {
            if (isErc20)
            {
                Money CostOfPack = Money.Parse(CustomPackPriceLabel.Text);
                NumberOfPacks = (int)(Decimal.Parse((MoneyToInvestTextBox.Text)) / CostOfPack.ToDecimal());
            }
            else
                NumberOfPacks = Convert.ToInt32(NumberOfPacksTextBox.Text);
        }
        catch (Exception ex)
        {
            throw new MsgException(U4000.BADFORMAT);
        }

    //Get AdPack type
        if (isErc20)
            AdPackType = new AdPackType(Convert.ToInt32(CustomTypesDropDown.SelectedValue));
        else
            AdPackType = new AdPackType(Convert.ToInt32(TypesDropDown.SelectedValue));

        //Get referral for AdPack
        if (AppSettings.RevShare.AdPack.BuyAdPacksForReferralsEnabled)
        {
            if (BuyForReferralCheckBox.Checked && ReferralsDropDown.Items.Count > 0)
                AdPackOwnerId = Convert.ToInt32(ReferralsDropDown.SelectedValue);

            if (CustomBuyForReferralCheckBox.Checked && CustomReferralsDropDownList.Items.Count > 0)
                AdPackOwnerId = Convert.ToInt32(CustomReferralsDropDownList.SelectedValue);
        }
            
        if (AdPackOwnerId.HasValue)
        {
            ValidateAdPacksForReferrals();
            AdPackOwner = new Member(AdPackOwnerId.Value);
        }
    }

    protected void PurchaseButtonViaPurchaseBalance_Click(object sender, EventArgs e)
    {
        ProceedAdPackPurchase(BalanceType.PurchaseBalance);
    }

    protected void PurchaseButtonViaERC20Tokens_Click(object sender, EventArgs e)
    {
        ProceedAdPackPurchase(BalanceType.Token);
    }

    protected void PurchaseButtonViaCommissionBalance_Click(object sender, EventArgs e)
    {
        ProceedAdPackPurchase(BalanceType.CommissionBalance);
    }

    protected void PurchaseButtonViaCashBalance_Click(object sender, EventArgs e)
    {
        ProceedAdPackPurchase(BalanceType.CashBalance);
    }

    protected void ProceedAdPackPurchase(BalanceType fromBalance)
    {
        HideMessages();

        try
        {
            if ((!agreeToS && TitanFeatures.isAri) || (TokenCryptocurrency.WalletEnabled && TOSAgreement.Checked == false))
                throw new MsgException("Terms of Service are not agreed");
            
            TryPrepareDataForAdPackPurchase();
            AdPackManager.TryBuyAdPack(fromBalance, AdvertId, AdPackType, NumberOfPacks, adPackOwner: AdPackOwner);

            if (NumberOfPacks == 1)
                SText.Text = CustomSuccessLiteral.Text = U4200.BUYADPACKSUCCESS.Replace("%n%", NumberOfPacks.ToString()).Replace("%p%", AppSettings.RevShare.AdPack.AdPackName) + ".";
            else
                SText.Text = CustomSuccessLiteral.Text = U4200.BUYADPACKSUCCESS.Replace("%n%", NumberOfPacks.ToString()).Replace("%p%", AppSettings.RevShare.AdPack.AdPackNamePlural) + ".";

            SPanel.Visible = true;
            CustomSuccessPanel.Visible = true;

            //Set up date when buy adpack to count number of active adpack (Customization)
            User.FirstActiveDayOfAdPacks = DateTime.Now;

            AdPacksStatsGridView.DataBind();

            BindDataToTypesDDL();
            BindDataToReferralsDDL();
        }
        catch (MsgException ex)
        {
            if (TokenCryptocurrency.WalletEnabled)
            {
                CustomErrorLiteral.Text = ex.Message;
                CustomErrorPanel.Visible = true;
            }
            else
            {
                EPanel.Visible = true;
                EText.Text = ex.Message;
            }
        }
        catch (Exception ex){
            ErrorLogger.Log(ex);}
    }
    #endregion

    #region CREATE ADPACKADVERT
    protected void CreateAdButton_Click(object sender, EventArgs e)
    {
        SPanel.Visible = false;
        EPanel.Visible = false;

        try
        {
            if (URL.Enabled && NewAdPackAdvert.TargetUrl != URL.Text)
                throw new MsgException(U4200.CHECKURL);

            NewAdPackAdvert.Title = InputChecker.HtmlEncode(Title.Text, Title.MaxLength, L1.TITLE);
            NewAdPackAdvert.Description = InputChecker.HtmlEncode(Description.Text, Description.MaxLength, L1.DESCRIPTION);
            NewAdPackAdvert.TargetUrl = URL.Text;

            NewAdPackAdvert.CreatorUserId = User.Id;

            //No need to approve admin's adverts
            if (NewAdPackAdvert.CreatorUserId == AppSettings.RevShare.AdminUserId || AppSettings.RevShare.AdPack.EnableAdvertAutoApproval)
                NewAdPackAdvert.Status = AdvertStatus.Active;
            else
                NewAdPackAdvert.Status = AdvertStatusExtensions.GetStartingStatus();

            NewAdPackAdvert.AddedAsType = PurchaseOption.Features.AdPack;

            UrlVerifier.Check(UrlCreator.ParseUrl(URL.Text));

            if (AppSettings.RevShare.AdPack.IsStartPageEnabled && PurchaseStartPageCheckBox.Checked && StartPageDateCalendar.SelectedDate != NewAdPackAdvert.StartPageDate)
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
            createBannerAdvertisement_BannerImage.ImageUrl = null;
            createBannerAdvertisement_BannerImage2.ImageUrl = null;

            //Display info
            SPanel.Visible = true;
            if (User.Name == AppSettings.RevShare.AdminUsername)
                SText.Text = U3501.ADCREATED;
            else
                SText.Text = U4200.ADAWAITSAPPROVAL;

            AdPacksAdGridView.DataBind();
            if (Request.QueryString["editid"] != null)
                Response.Redirect("adpacks.aspx");

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
        URL.Enabled = true;
        CheckURLButton.Visible = true;
        deleteOldImageIfExists();
        deleteOldImageIfExists2();
        createBannerAdvertisement_BannerUpload.Visible = true;
        createBannerAdvertisement_BannerUploadSubmit.Visible = true;
        NormalBannerUpload.Visible = true;
        NormalBannerUploadButton.Visible = true;
        NewAdPackAdvert = null;
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

    protected void createBannerAdvertisement_BannerUploadSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            if (!Page.IsValid) return;

            if (!((Banners.TryCreateBannerFromUrl(BannerFileUrlTextBox2.Text, out newTemporaryBanner) ||
                   Banner.TryFromStream(createBannerAdvertisement_BannerUpload.PostedFile.InputStream,
                       out newTemporaryBanner))
                  &&
                  (newTemporaryBanner.Width == AppSettings.RevShare.AdPack.PackConstantBannerWidth &&
                   newTemporaryBanner.Height == AppSettings.RevShare.AdPack.PackConstantBannerHeight)))
                throw new MsgException(U6000.INVALIDBANNERIMAGEORDIMENSIONS);


            newTemporaryBanner.Save(AppSettings.FolderPaths.BannerAdvertImages);

            //deleteOldImageIfExists();
            NewAdPackAdvert.ConstantBannerImage = newTemporaryBanner;
            createBannerAdvertisement_BannerImage.ImageUrl = NewAdPackAdvert.ConstantBannerImage.Path;

            //Hide upload
            //createBannerAdvertisement_BannerUpload.Visible = false;
            //createBannerAdvertisement_BannerUploadSubmit.Visible = false;

            if (createBannerAdvertisement_BannerUpload.HasFile)
                createBannerAdvertisement_BannerUpload.Dispose();
            if (!string.IsNullOrEmpty(BannerFileUrlTextBox2.Text))
                BannerFileUrlTextBox2.Text = "";
        }
        catch (Exception ex)
        {
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = ex.Message;
        }
    }

    protected void NormalBannerUploadButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (!Page.IsValid) return;

            if (!((Banners.TryCreateBannerFromUrl(BannerFileUrlTextBox1.Text, out newTemporaryBanner2) ||
                   Banner.TryFromStream(NormalBannerUpload.PostedFile.InputStream, out newTemporaryBanner2))
                  &&
                  (newTemporaryBanner2.Width == AppSettings.RevShare.AdPack.PackNormalBannerWidth &&
                   newTemporaryBanner2.Height == AppSettings.RevShare.AdPack.PackNormalBannerHeight)))
                throw new MsgException(U6000.INVALIDBANNERIMAGEORDIMENSIONS);


            newTemporaryBanner2.Save(AppSettings.FolderPaths.BannerAdvertImages);

            //deleteOldImageIfExists2();
            NewAdPackAdvert.NormalBannerImage = newTemporaryBanner2;
            createBannerAdvertisement_BannerImage2.ImageUrl = NewAdPackAdvert.NormalBannerImage.Path;

            //Hide upload
            //NormalBannerUpload.Visible = false;
            //NormalBannerUploadButton.Visible = false;

            if (NormalBannerUpload.HasFile)
                NormalBannerUpload.Dispose();
            if (!string.IsNullOrEmpty(BannerFileUrlTextBox1.Text))
                BannerFileUrlTextBox1.Text = "";
        }
        catch (Exception ex)
        {
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = ex.Message;
        }
    }

    private void deleteOldImageIfExists()
    {
        if (NewAdPackAdvert != null && NewAdPackAdvert.ConstantBannerImage != null && NewAdPackAdvert.ConstantBannerImage.IsSaved
            && newTemporaryBanner != null)
        {
            newTemporaryBanner.Delete();
            NewAdPackAdvert.ConstantBannerImage = null;
        }
    }

    private void deleteOldImageIfExists2()
    {
        if (NewAdPackAdvert != null && NewAdPackAdvert.NormalBannerImage != null && NewAdPackAdvert.NormalBannerImage.IsSaved
             && newTemporaryBanner2 != null)
        {
            newTemporaryBanner2.Delete();
            NewAdPackAdvert.NormalBannerImage = null;
        }
    }

    Banner newTemporaryBanner;
    Banner newTemporaryBanner2;

    protected void createBannerAdvertisement_BannerUploadValidCustomValidator_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = createBannerAdvertisement_BannerUpload.HasFile || !string.IsNullOrEmpty(BannerFileUrlTextBox2.Text);
    }

    protected void NormalBannerUploadValidator_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = NormalBannerUpload.HasFile || !string.IsNullOrEmpty(BannerFileUrlTextBox1.Text);
    }


    protected void createBannerAdvertisement_BannerUploadSelectedCustomValidator_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = NewAdPackAdvert.ConstantBannerImage != null;
    }

    protected void NormalBannerUploadSelectedValidator_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = NewAdPackAdvert.NormalBannerImage != null;
    }
    #endregion

    #region ADPACKS GRIDVIEW

    protected void AdPacksAdGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!AppSettings.RevShare.AdPack.EnableAdvertChange)
                AdPacksAdGridView.Columns[6].ItemStyle.CssClass = "displaynone";

            if (AppSettings.BannerAdverts.HideAllBannersEnabled)
            {
                e.Row.Cells[4].ControlStyle.CssClass = "displaynone";
                e.Row.Cells[5].ControlStyle.CssClass = "displaynone";
            }
            else
            {
                var ConstantImg = new Image();
                var NormalImg = new Image();
                if (e.Row.Cells[4].Text != "&nbsp;")
                {
                    ConstantImg.ImageUrl = e.Row.Cells[4].Text;
                    ConstantImg.Width = Unit.Pixel(AppSettings.RevShare.AdPack.PackConstantBannerWidth / 10);
                    ConstantImg.Height = Unit.Pixel(AppSettings.RevShare.AdPack.PackConstantBannerHeight / 10);
                    e.Row.Cells[4].Text = "";
                    e.Row.Cells[4].Controls.Add(ConstantImg);
                }
                if (e.Row.Cells[5].Text != "&nbsp;")
                {
                    NormalImg.ImageUrl = e.Row.Cells[5].Text;
                    NormalImg.Width = Unit.Pixel(AppSettings.RevShare.AdPack.PackNormalBannerWidth / 10);
                    NormalImg.Height = Unit.Pixel(AppSettings.RevShare.AdPack.PackNormalBannerHeight / 10);
                    e.Row.Cells[5].Text = "";
                    e.Row.Cells[5].Controls.Add(NormalImg);
                }
            }

            if (e.Row.Cells[3].Text.Length > 19)
                e.Row.Cells[3].Text = e.Row.Cells[3].Text.Substring(0, 16) + "...";

            AdvertStatus Status = (AdvertStatus)Convert.ToInt32(e.Row.Cells[2].Text);
            e.Row.Cells[2].Text = HtmlCreator.GetColoredStatus(Status);
        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            if (AppSettings.BannerAdverts.HideAllBannersEnabled)
            {
                AdPacksAdGridView.Columns[4].HeaderStyle.CssClass = "displaynone";
                AdPacksAdGridView.Columns[5].HeaderStyle.CssClass = "displaynone";
            }

            if (!AppSettings.RevShare.AdPack.EnableAdvertChange)
                AdPacksAdGridView.Columns[6].HeaderStyle.CssClass = "displaynone";

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
    protected void AdPacksStatsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!AppSettings.RevShare.AdPack.IsStartPageEnabled)
                e.Row.Cells[8].ControlStyle.CssClass = "displaynone";

            if (AppSettings.BannerAdverts.HideAllBannersEnabled)
            {
                e.Row.Cells[4].ControlStyle.CssClass = "displaynone";
                e.Row.Cells[5].ControlStyle.CssClass = "displaynone";
            }

            AdPack adPack = new AdPack(Convert.ToInt32(e.Row.Cells[1].Text));

            AdPackTypesCache cache = new AdPackTypesCache();
            var adPackTypes = (Dictionary<int, AdPackType>)cache.Get();
            e.Row.Cells[0].Style.Add("background-color", adPackTypes[adPack.AdPackTypeId].Color);

            if (AppSettings.RevShare.AdPacksPolicy != AppSettings.AdPacksPolicy.HYIP)
            {
                e.Row.Cells[2].Text = HtmlCreator.GenerateCPAAdProgressHTML(Convert.ToInt32(e.Row.Cells[2].Text), Convert.ToInt32(adPack.ClicksBought), L1.CLICKS.ToLower());
                e.Row.Cells[4].Text = HtmlCreator.GenerateCPAAdProgressHTML(Convert.ToInt32(e.Row.Cells[4].Text), Convert.ToInt32(adPack.NormalBannerImpressionsBought), L1.IMPRESSIONS.ToLower());
                e.Row.Cells[5].Text = HtmlCreator.GenerateCPAAdProgressHTML(Convert.ToInt32(e.Row.Cells[5].Text), Convert.ToInt32(adPack.ConstantBannerImpressionsBought), L1.IMPRESSIONS.ToLower());
                if ((e.Row.Cells[8].Text.ToLower() == "&nbsp;"))
                    e.Row.Cells[8].Text = "-";
                else
                {
                    try
                    {
                        DateTime StartPageDate = Convert.ToDateTime(e.Row.Cells[8].Text);
                        if (StartPageDate < DateTime.Now.AddDays(-3))
                        {
                            //We do not show older then 3 days
                            e.Row.Cells[8].Text = "-";
                        }
                        else
                        {
                            e.Row.Cells[8].Text = StartPageDate.ToShortDateString();
                        }
                    }
                    catch { }
                }
            }
            else
            {
                //Hide checkbox
                e.Row.Cells[0].Controls[1].Visible = false;
            }
            e.Row.Cells[6].Text = HtmlCreator.GenerateCPAAdProgressHTML(new Money(Convert.ToDecimal(e.Row.Cells[6].Text)).ToDecimal(), adPack.MoneyToReturn.ToDecimal(), AppSettings.Site.CurrencySign);

            if ((e.Row.Cells[7].Text.ToLower() == "&nbsp;"))
                e.Row.Cells[7].Text = "-";
        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            if (AppSettings.RevShare.AdPacksPolicy == AppSettings.AdPacksPolicy.HYIP)
                ((System.Web.UI.HtmlControls.HtmlInputCheckBox)e.Row.FindControl("checkAll")).Visible = false;

            if (AppSettings.BannerAdverts.HideAllBannersEnabled)
            {
                AdPacksStatsGridView.Columns[4].HeaderStyle.CssClass = "displaynone";
                AdPacksStatsGridView.Columns[5].HeaderStyle.CssClass = "displaynone";
            }
            else
            {
                e.Row.Cells[4].Text = U4200.BANNER + " " + string.Format("({0}x{1})", AppSettings.RevShare.AdPack.PackNormalBannerWidth, AppSettings.RevShare.AdPack.PackNormalBannerHeight);
                e.Row.Cells[5].Text = U4200.BANNER + " " + string.Format("({0}x{1})", AppSettings.RevShare.AdPack.PackConstantBannerWidth, AppSettings.RevShare.AdPack.PackConstantBannerHeight);
            }

            if (!AppSettings.RevShare.AdPack.EnableAdvertChange)
            {
                AdPacksStatsGridView.Columns[0].ShowHeader = false;
                AdPacksStatsGridView.Columns[0].Visible = false;
                AdPacksStatsGridView.Columns[1].ShowHeader = false;
                AdPacksStatsGridView.Columns[1].HeaderStyle.CssClass = "displaynone";
                ((System.Web.UI.HtmlControls.HtmlInputCheckBox)e.Row.FindControl("checkAll")).Visible = false;
            }


            if (!AppSettings.RevShare.AdPack.IsStartPageEnabled)
                AdPacksStatsGridView.Columns[8].HeaderStyle.CssClass = "displaynone";

            AdPacksStatsGridView.Columns[8].HeaderText = U5001.STARTPAGE;
            AdPacksStatsGridView.Columns[3].HeaderText = U5001.DISPLAYTIME;
        }
    }

    protected void AdPacksStatsGridView_DataSource_Init(object sender, EventArgs e)
    {
        AdPacksStatsGridView_DataSource.SelectCommand = string.Format(@"SELECT * FROM AdPacks ap LEFT JOIN AdPacksAdverts apa ON apa.Id = ap.AdPacksAdvertId WHERE ap.UserId = {0} AND AdPackTypeId != -1 ORDER BY PurchaseDate DESC, UserCustomGroupId, Status", Member.CurrentId);
    }

    #endregion

    #region MY GROUPS GRIDVIEW

    protected void MyGroupsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int userId = Convert.ToInt32(e.Row.Cells[0].Text);
            int userCustomGroupId = Convert.ToInt32(e.Row.Cells[7].Text);

            //CreatorUserId -> UserName
            e.Row.Cells[0].Text = new Member(userId).Name;

            //Name
            e.Row.Cells[1].BackColor = System.Drawing.Color.FromName(e.Row.Cells[2].Text);

            //insert link
            e.Row.Cells[1].Text = string.Format("<span style='font-weight: bold;'><a style='color:white;' href='{0}user/advert/groups/customgroup.aspx?g={1}'>{2}<a/><span/>", AppSettings.Site.Url, userCustomGroupId, e.Row.Cells[1].Text);

            //Color
            e.Row.Cells[2].Visible = false;

            //AdPacks progress
            e.Row.Cells[3].Text = HtmlCreator.GenerateCPAAdProgressHTML(Convert.ToInt32(e.Row.Cells[3].Text), Convert.ToInt32(e.Row.Cells[4].Text),
                AppSettings.RevShare.AdPack.AdPackNamePlural);

            //My AdPacks
            e.Row.Cells[4].Text = AdPackManager.GetNumberOfUsersAdPacksInCustomGroup(userId, userCustomGroupId).ToString();

            //Percentage
            e.Row.Cells[5].Visible = false;

            //Daily profit
            if (AppSettings.RevShare.AdPack.CustomReturnOption == CustomReturnOption.Increase)
                e.Row.Cells[6].Text = "+" + Convert.ToInt32(e.Row.Cells[6].Text).ToString();
            e.Row.Cells[6].Text += "%";

            //UserGroupId - > Number of participants
            e.Row.Cells[7].Text = AdPackManager.GetNumberOfParticipantsInGroup(userCustomGroupId).ToString();

        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[2].Visible = false;

            e.Row.Cells[5].Visible = false;

            if (AppSettings.RevShare.AdPack.CustomReturnOption == CustomReturnOption.Accelerate)
                MyGroupsGridView.Columns[6].HeaderText = U4200.DAILYPROFITRAISEDBY;
            else if (AppSettings.RevShare.AdPack.CustomReturnOption == CustomReturnOption.Increase)
                MyGroupsGridView.Columns[6].HeaderText = U5001.PROFITRAISEDBY;
        }
    }

    protected void MyGroupsGridViewDataSource_Init(object sender, EventArgs e)
    {

        MyGroupsGridViewDataSource.SelectCommand = CustomGroupManager.GetGroupsThatUserParticipatesInSQL(Member.CurrentId);
    }

    #endregion

    #region ADPACK TYPES

    protected void AdPackTypesGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LangAdder.AddUpgrade(e.Row, 0, L1.NAME);
            LangAdder.AddUpgrade(e.Row, 2, L1.PRICE);
            LangAdder.AddUpgrade(e.Row, 3, U5004.MINOFPREVIOUSTYPE);
            LangAdder.AddUpgrade(e.Row, 4, U5004.MAXINSTANCES);
            LangAdder.AddUpgrade(e.Row, 5, U6003.MAXACTIVEINSTANCES);
            LangAdder.AddUpgrade(e.Row, 7, U5001.DISPLAYTIME);
            LangAdder.AddUpgrade(e.Row, 10, U5006.REQUIREDMEMBERSHIP);
            LangAdder.AddUpgrade(e.Row, 11, U5006.VALUEOF1SECINCLICKS);
            LangAdder.AddUpgrade(e.Row, 12, U5006.AVAILABLEFORCUSTOMGROUPS);
            LangAdder.AddUpgrade(e.Row, 14, U5008.WITHDRAWLIMITENLARGEDBY);
            LangAdder.AddUpgrade(e.Row, 15, U6000.TRAFFICEXCHANGESURFCREDITS);

            if (IsAdPackTypesHidden)
                if (e.Row.RowIndex == 0 || e.Row.RowIndex == 3)
                    e.Row.CssClass = "displaynone";

            if (!AppSettings.TitanFeatures.UpgradeEnabled)
                if (e.Row.RowIndex == 10)
                    e.Row.CssClass = "displaynone";

            if (!AppSettings.RevShare.AdPack.IsTimeClickExchangeEnabled)
                if (e.Row.RowIndex == 11)
                    e.Row.CssClass = "displaynone";

            if (AppSettings.RevShare.AdPack.GroupPolicy != GroupPolicy.CustomGroups && AppSettings.RevShare.AdPack.GroupPolicy != GroupPolicy.AutomaticAndCustomGroups)
                if (e.Row.RowIndex == 12)
                    e.Row.CssClass = "displaynone";

            if (!AppSettings.LoginAds.LoginAdsCreditsEnabled)
                if (e.Row.RowIndex == 13)
                    e.Row.CssClass = "displaynone";

            if (!AppSettings.Payments.AdPackTypeWithdrawLimitEnabled)
                if (e.Row.RowIndex == 14)
                    e.Row.CssClass = "displaynone";

            if (!AppSettings.TitanFeatures.AdvertTrafficExchangeEnabled)
                if (e.Row.RowIndex == 15)
                    e.Row.CssClass = "displaynone";

            if (AppSettings.BannerAdverts.HideAllBannersEnabled)
                if (e.Row.RowIndex == 8 || e.Row.RowIndex == 9)
                    e.Row.CssClass = "displaynone";

            //true -> check | false -> &nbsp
            if (e.Row.RowIndex == 12)
                foreach (TableCell tc in e.Row.Cells)
                    if (tc != e.Row.Cells[0])
                    {
                        if (tc.Text.Contains("alse"))
                        {
                            tc.Text = HtmlCreator.GetCheckboxUncheckedImage();
                        }
                        else
                        {
                            tc.Text = HtmlCreator.GetCheckboxCheckedImage();
                        }
                    }

            //Add color
            if (e.Row.RowIndex == 1)
                foreach (TableCell tc in e.Row.Cells)
                    if (tc != e.Row.Cells[0])
                        tc.Text = "<div style=\"background-color:" + tc.Text + ";height:12px; width:25px\">&nbsp;</div>";

            if (e.Row.RowIndex == 2)
                foreach (TableCell tc in e.Row.Cells)
                    if (tc != e.Row.Cells[0])
                        tc.Text = Money.Parse(tc.Text).ToString();

            if (e.Row.RowIndex == 4)
                foreach (TableCell tc in e.Row.Cells)
                    if (tc != e.Row.Cells[0] && Convert.ToInt32(tc.Text) >= int.MaxValue)
                        tc.Text = U4200.UNLIMITED;

            if (e.Row.RowIndex == 5)
                foreach (TableCell tc in e.Row.Cells)
                    if (tc != e.Row.Cells[0] && Convert.ToInt32(tc.Text) >= int.MaxValue)
                        tc.Text = U4200.UNLIMITED;

            if (e.Row.RowIndex == 7)
                foreach (TableCell tc in e.Row.Cells)
                    if (tc != e.Row.Cells[0])
                        tc.Text += "s";

            if (e.Row.RowIndex == 10)
                foreach (TableCell tc in e.Row.Cells)
                    if (tc != e.Row.Cells[0])
                        tc.Text = new Membership(Convert.ToInt32(tc.Text)).Name;

            if (e.Row.RowIndex == 11)
                foreach (TableCell tc in e.Row.Cells)
                    if (tc != e.Row.Cells[0])
                        tc.Text += " " + L1.CLICKSSMALL;

            if (e.Row.RowIndex == 14)
                foreach (TableCell tc in e.Row.Cells)
                    if (tc != e.Row.Cells[0])
                        tc.Text += "% " + U5008.OFPRICE;

            if (e.Row.RowIndex == 15)
                foreach (TableCell tc in e.Row.Cells)
                    if (tc != e.Row.Cells[0])
                        tc.Text = Money.Parse(tc.Text).ToString();


            if (AppSettings.RevShare.AdPacksPolicy != AppSettings.AdPacksPolicy.HYIP)
            {
                LangAdder.AddUpgrade(e.Row, 6, L1.CLICKS);
                LangAdder.AddUpgrade(e.Row, 8, U4200.NORMALBANNER.Replace("%n%", string.Format("({0}x{1})", AppSettings.RevShare.AdPack.PackNormalBannerWidth, AppSettings.RevShare.AdPack.PackNormalBannerHeight)));
                LangAdder.AddUpgrade(e.Row, 9, U4200.CONSTANTBANNER.Replace("%n%", string.Format("({0}x{1})", AppSettings.RevShare.AdPack.PackConstantBannerWidth, AppSettings.RevShare.AdPack.PackConstantBannerHeight)));
                LangAdder.AddUpgrade(e.Row, 13, U5008.LOGINADSCREDITS);
            }
            else
            {
                if (e.Row.RowIndex == 6 || e.Row.RowIndex == 8 || e.Row.RowIndex == 9 || e.Row.RowIndex == 13)
                    e.Row.CssClass = "displaynone";
            }
        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            foreach (TableCell tc in e.Row.Cells)
                tc.Text = "&nbsp;";
        }
    }

    #endregion

    protected void View4_Activate(object sender, EventArgs e)
    {
        if (TokenCryptocurrency.WalletEnabled)
        {
            CustomBuyAdPacksButton.Visible = true;

            if (TitanFeatures.IsTrafficThunder)
                CustomBuyAdPacksButton.Text = "START LENDING";
            else
                CustomBuyAdPacksButton.Text = String.Format("{0} {1}", L1.BUY, AppSettings.RevShare.AdPack.AdPackName).ToUpper();
        }

        AdPacksStatsGridView.DataBind();

        int numberOfAdPacksToNextGroup = 0;
        AutomaticGroup nextGroup = null;

        if (AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.AutomaticGroups || AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.AutomaticAndCustomGroups)
        {
            var currentGroup = AutomaticGroupManager.GetUsersAutomaticGroup(User, ref numberOfAdPacksToNextGroup, ref nextGroup);

            if (currentGroup != null)
            {
                CurrentAutomaticGroupLiteral.Text = U4200.CURRENTAUTOMATICGROUPDESC1.Replace("%a%", "<b>" + currentGroup.Number.ToString() + "</b>")
                    .Replace("%b%", "<b>" + (currentGroup.AcceleratedProfitPercentage - 100).ToString() + "</b>");

                if (nextGroup != null)
                {
                    CurrentAutomaticGroupLiteral.Text += " " + U4200.CURRENTAUTOMATICGROUPDESC2.Replace("%a%", "<b>" + numberOfAdPacksToNextGroup.ToString() + "</b>")
                        .Replace("%b%", AppSettings.RevShare.AdPack.AdPackNamePlural).Replace("%c%", "<b>" + (nextGroup.AcceleratedProfitPercentage - 100).ToString() + "</b>");
                }
            }
            else
                CurrentAutomaticGroupPlaceHolder.Visible = false;
        }
    }

    protected void ChangeCampaignButton_Click(object sender, EventArgs e)
    {
        if (AppSettings.RevShare.AdPack.EnableAdvertChange)
        {
            for (int i = 0; i < AdPacksStatsGridView.Rows.Count; i++)
            {
                GridViewRow row = AdPacksStatsGridView.Rows[i];
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
            AdpackCampaignsInfoDiv.Visible = AdPackManager.HasAdPackWithoutCampaigns(User.Id);
            AdPacksStatsGridView.DataBind();
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

    protected void TypesDropDown_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (TypesDropDown.Items.Count > 0)
        {
            UpdateWhiteBoxInfo();
            TypesDropDown.Attributes.Add("style", string.Format("background-color:{0};color:white", adPackType.Color));

            if (AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.AutomaticAndCustomGroups || AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.CustomGroups)
                TypeAvailableForCustomGroups.Visible = adPackType.CustomGroupsEnabled;
        }
    }

    private void UpdateWhiteBoxInfo()
    {
        if (TypesDropDown.Items.Count > 0)
        {
            adPackType = new AdPackType(Convert.ToInt32(TypesDropDown.SelectedValue));
            PackPriceLabel.Text = adPackType.Price.ToClearString();
            PackROILabel.Text = adPackType.PackReturnValuePercentage.ToString();
            PackClicksLabel.Text = adPackType.Clicks.ToString();
            PackDisplayTimeLabel.Text = adPackType.DisplayTime.ToString();
            PackNormalBannerImpressionsLabel.Text = adPackType.NormalBannerImpressions.ToString();
            PackConstantBannerImpressionsLabel.Text = adPackType.ConstantBannerImpressions.ToString();
            packLoginAdsCreditsLabel.Text = adPackType.LoginAdsCredits.ToString();
            packWithdrawLimitLabel.Text = Money.MultiplyPercent(adPackType.Price, adPackType.WithdrawLimitPercentage).ToClearString();
            WhiteBoxLoginAdsCreditsPlaceHolder.Visible = AppSettings.LoginAds.LoginAdsCreditsEnabled && adPackType.LoginAdsCredits > 0;
            WhiteBoxWithdrawLimitPlaceHolder.Visible = AppSettings.Payments.AdPackTypeWithdrawLimitEnabled;
            TrafficExchangeSurfCreditsPlaceHolder.Visible = AppSettings.TitanFeatures.AdvertTrafficExchangeEnabled;
            TrafficExchangeSurfCreditsLabel.Text = adPackType.TrafficExchangeSurfCredits.ToClearString();
            TypesDropDown.Attributes.Add("style", string.Format("background-color:{0};color:white", adPackType.Color));
        }
        else
        {
            AdPackPurchasePlaceHolder.Visible = false;
            NoAdPackTypesForMemberLiteral.Text = String.Format(U5008.NOADPACKTYPES, AppSettings.RevShare.AdPack.AdPackName);
        }

        if (CustomTypesDropDown.Items.Count > 0)
        {
            adPackType = new AdPackType(Convert.ToInt32(CustomTypesDropDown.SelectedValue));
            CustomPackPriceLabel.Text = CustomPackPriceLabel.Text = adPackType.Price.ToClearString();
        }
        else
            CustomTypesDDLPlaceHolder.Visible = false;
    }
    protected void AddSecondsButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (!AppSettings.RevShare.AdPack.IsTimeClickExchangeEnabled)
                return;

            SPanel.Visible = false;
            EPanel.Visible = false;

            int numberOfSeconds = Convert.ToInt32(ExchangeSecondsTextBox.Text);
            if (numberOfSeconds < 1)
                throw new MsgException("Number of seconds must be higher than 0");

            int count = 0;
            for (int i = 0; i < AdPacksStatsGridView.Rows.Count; i++)
            {
                GridViewRow row = AdPacksStatsGridView.Rows[i];
                bool isChecked = ((CheckBox)row.FindControl("chkSelect")).Checked;

                if (isChecked)
                    count++;
            }
            if (count > 1)
                throw new MsgException(U5006.SELECTSINGLECHECKBOX);
            else if (count <= 0)
                return;

            for (int i = 0; i < AdPacksStatsGridView.Rows.Count; i++)
            {
                GridViewRow row = AdPacksStatsGridView.Rows[i];
                bool isChecked = ((CheckBox)row.FindControl("chkSelect")).Checked;

                if (isChecked)
                {
                    int adpackId = Convert.ToInt32(row.Cells[1].Text);

                    AdPack adpack = new AdPack(adpackId);
                    AdPackManager.ExchangeClicksForSeconds(numberOfSeconds, adpack.Id, User);
                }
            }

            SPanel.Visible = true;
            SText.Text = "You have succesfully extended advertisement's display time";
            AdPacksStatsGridView.DataBind();
        }
        catch (Exception ex)
        {
            EPanel.Visible = true;
            EText.Text = ex.Message;
            AdPacksStatsGridView.DataBind();
        }
    }

    protected void AdPacksAdGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "edit" && AppSettings.RevShare.AdPack.EnableAdvertChange)
        {
            int index = e.GetSelectedRowIndex() % AdPacksAdGridView.PageSize;
            string adId = AdPacksAdGridView.DataKeys[index].Values["Id"].ToString();
            Response.Redirect("adpacks.aspx?editid=" + adId);
        }
    }

    protected void BuyForReferralCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        if (BuyForReferralCheckBox.Checked)
            BindDataToReferralsDDL();

        ReferralsDropDown.Visible = BuyForReferralCheckBox.Checked;
    }

    protected void TypesView_Activate(object sender, EventArgs e)
    {
        TypesMembershipProperties.Text = AdPackTypeMembershipMapper.Mapper.GetHtmlFromCache();

        SetTypesViewLabels();
    }

    private void SetTypesViewLabels()
    {
        if (IsAdPackTypesHidden)
        {
            TypesTableTitle.InnerHtml = string.Format(U6012.INFORMATIONOF, AppSettings.RevShare.AdPack.AdPackName);
        }
        else
        {
            TypesTableTitle.InnerHtml = string.Format(U6002.ADPACKTYPES, AppSettings.RevShare.AdPack.AdPackName);
        }
    }

    protected void AgreeToSCheckBox_CheckedChanged(object sender, EventArgs e)
    {
            AgreeToSPlaceHolder.Visible = AgreeToSCheckBox.Checked;
            AgreeToSCheckBox.Visible = !AgreeToSCheckBox.Checked;
            AdPackPurchasePlaceHolder.Visible = !AgreeToSCheckBox.Checked;
            TitleDescriptionP.Visible = !AgreeToSCheckBox.Checked;

            PurchaseButton.Enabled = AgreeToSCheckBox.Checked && agreeToS;

            var dict = TableHelper.MakeDictionary("TextType", (int)WebsiteTextType.ToS);
            var list = TableHelper.SelectRows<WebsiteText>(dict);
            AgreeToSLiteral.Text = list[0].Content;
    }

    protected void AgreeToSButton_Command(object sender, CommandEventArgs e)
    {
        AgreeToSPlaceHolder.Visible = false;
        AgreeToSCheckBox.Visible = true;
        PurchaseButton.Enabled = true;
        AdPackPurchasePlaceHolder.Visible = true;
        TitleDescriptionP.Visible = true;
        agreeToS = true;
    }

    
    protected void TOSAgreement_CheckedChanged(object sender, EventArgs e)
    {
        CustomPurchaseViaERC20TokensButton.Enabled = TOSAgreement.Checked;
        CustomPurchaseViaPurchaseBalanceButton.Enabled = TOSAgreement.Checked;
        CustomPurchaseViaCommissionBalanceButton.Enabled = TOSAgreement.Checked;
        CustomPurchaseViaCashBalanceButton.Enabled = TOSAgreement.Checked;
    }

    protected void StartLendingButton_Click(object sender, EventArgs e)
    {
        HidePacksListShowBuyPageV2();
    }

    private void HidePacksListShowBuyPageV2()
    {
        BaseAdPacksView.Visible = false;
        CustomAdPacksView.Visible = true;

        CustomCampaignsDropDownManagement();
        CustomTypesLabel.Text = "AdPack types";

        if (TitanFeatures.IsTrafficThunder)
        {
            CustomTypesDDLPlaceHolder.Visible = false;
            CustomPurchaseViaPurchaseBalancePlaceHolder.Visible = false;
        }

        HideMessages();
    }

    protected void CancelLendingButton_Click(object sender, EventArgs e)
    {
        CheckIfRedirectAfterAction();

        BaseAdPacksView.Visible = true;
        CustomAdPacksView.Visible = false;
        HideMessages();
    }

    protected void HideMessages()
    {
        SPanel.Visible = false;
        EPanel.Visible = false;
        CustomSuccessPanel.Visible = false;
        CustomErrorPanel.Visible = false;
        CustomMessagesPlaceHolder.Visible = false;
    }

    protected void CustomTypesDropDown_SelectedIndexChanged(object sender, EventArgs e)
    {
        CustomPackPriceLabel.Text = new AdPackType(Convert.ToInt32(CustomTypesDropDown.SelectedValue)).Price.ToClearString();
    }

    protected void CustomCampaignsDropDownManagement()
    {
        CustomCampaignDDLPlaceHolder.Visible = AdPackManager.HasAvailableAdverts(Member.CurrentId);
        if (CustomCampaignDDLPlaceHolder.Visible && !CustomTypesDDLPlaceHolder.Visible)
            SpaceDiv.Visible = true;

        if (CustomCampaignDDLPlaceHolder.Visible == true)
        {
            CustomCampaignLabel.Text = "Campaigns";
            var list = new Dictionary<string, string>();
            var campaigns = AdPackManager.GetUsersAdverts(User.Id);

            for (int i = 0; i < campaigns.Count; i++)
            {
                list.Add(campaigns[i].Id.ToString(), campaigns[i].Title);
            }

            CustomCampaignsDropDown.DataSource = list;
            CustomCampaignsDropDown.DataTextField = "Value";
            CustomCampaignsDropDown.DataValueField = "Key";
            CustomCampaignsDropDown.DataBind();
        }
    }

    protected void CustomBuyForReferralCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        if (CustomBuyForReferralCheckBox.Checked)
            BindDataToReferralsDDL();

        CustomReferralsPlaceHolder.Visible = CustomBuyForReferralCheckBox.Checked;
    }
}
