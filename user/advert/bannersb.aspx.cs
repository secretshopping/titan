using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Utils;
using Prem.PTC.Members;
using Prem.PTC.Advertising;
using Resources;
using Titan.Advertising;

public partial class About : System.Web.UI.Page
{
    public string jsSelectAllCode;
    public List<BannerAdvert> availableOptions;

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.AdvertisersRoleEnabled && AppSettings.TitanFeatures.AdvertBannersEnabled && Member.CurrentInCache.IsAdvertiser);
        
        if (AppSettings.BannerAdverts.AdvertisingPolicy == BannerPolicy.SellingPackages)
            Response.Redirect("~/user/advert/banners.aspx");
        
        UserName.Text = Member.CurrentName;

        SubLiteral.Text = L1.ADVADSINFO;

        //Lang & Hint
        DirectRefsGridView.EmptyDataText = L1.NOBANNERCAMPS;
        LangAdder.Add(Button2, L1.ADDNEW);
        LangAdder.Add(Button1, U4000.ADDEDCAMPAIGNS);
        LangAdder.Add(Button3, U4000.AUCTIONS);
        LangAdder.Add(CreateAdButton, L1.SEND);
        LangAdder.Add(CustomValidator1, L1.ER_BADCAPTCHA, true);
        LangAdder.Add(PasswordRequired, L1.REQ_URL, true);
        LangAdder.Add(RegularExpressionValidator2, L1.ER_BADURL, true);
        HintAdder.Add(URL, L1.H_URL);
        BannerImageLabel.Text = L1.BURL;
        HintAdder.Add(BannerImageLabel, L1.H_BANNERURL);
        LangAdder.Add(createBannerAdvertisement_BannerUploadSelectedCustomValidator, L1.ER_BANNERNOTSELECTED, true);
        LangAdder.Add(createBannerAdvertisement_BannerUploadValidCustomValidator, U6000.CHOOSEFILE, true);
        AuctionsUnavailable.Visible = false;

        try
        {
            if (!Page.IsPostBack)
                CurrentSelectedDimemsions = new BannerAdvertDimensions(BannerAdvertDimensions.GetActive()[0].Id);
        }
        catch
        {
            AuctionsUnavailable.Visible = true;
            AuctionsUnavailable.HeaderText = U6000.NEWBANNERSUNAVAILABLEHEADER;
            AuctionsUnavailable.Reason = U6000.NEWBANNERSUNAVAILABLEREASON;
            AuctionsPanel.Visible = false;
            MenuButtonPlaceHolder.Visible = false;
            return;
        }
        if(MenuMultiView.ActiveViewIndex == 0)
            CreateDimensionsButtons();

        if (!Page.IsPostBack)
        {
            BannerTypeRadioButtonList.Items.Clear();
            BannerTypeRadioButtonList.Items.AddRange(BannerAdvertDimensions.GetActiveItems());

            if (Request.QueryString["red"] != null && Request.QueryString["red"] == "new")
            {
                DimensionsPlaceHolder.Controls.Clear();
                MenuMultiView.ActiveViewIndex = 2;
                Button3.CssClass = string.Empty;
                Button2.CssClass = "ViewSelected";
                Session["YourCampaigns_NewBannerAdvert"] = null;
                createBannerAdvertisement_BannerImage.ImageUrl = "";
                URL.Text = "http://";

                OptionRadioButtonDiv.Visible = false;

                if (Request.QueryString["view"] != null)
                {
                    var index = Request.QueryString["view"];
                    OptionBanerAddNewLiteral.Text = BannerTypeRadioButtonList.Items.FindByValue(index).Text;
                    OptionBanerAddNewLiteral.Visible = true;
                    BannerTypeRadioButtonList.Items.FindByValue(index).Selected = true;
                }
            }
            else if (Request.QueryString["red"] != null && Request.QueryString["red"] == "camp")
            {
                DimensionsPlaceHolder.Controls.Clear();
                MenuMultiView.ActiveViewIndex = 1;
                Button3.CssClass = string.Empty;
                Button1.CssClass = "ViewSelected";
                Session["YourCampaigns_NewBannerAdvert"] = null;
                createBannerAdvertisement_BannerImage.ImageUrl = "";
                URL.Text = "http://";
            }

            //JS changes
            URL.Attributes.Add("onfocus", "if (getElementById('" + URL.ClientID + "').value == 'http://') getElementById('" + URL.ClientID + "').value = ''");

            BannerUploadByUrlButton.Visible = AppSettings.Site.BannersAddByUrlEnabled;
        }
        ErrorMessagePanel.Visible = false;
    }

    private BannerAuction _target;
    public BannerAuction target
    {
        get
        {
            if (_target == null)
                _target = new BannerAuction((int)Session["BannerBidTarget"]);
            return _target;
        }
        set
        {
            _target = value;
            Session["BannerBidTarget"] = _target.Id;
        }
    }

    private BannerAdvertDimensions _currentSelectedDimemsions;
    public BannerAdvertDimensions CurrentSelectedDimemsions
    {
        get
        {
            if (_currentSelectedDimemsions == null)
            {
                if (Session["CurrentDimensionsId"] == null)
                {
                    if(BannerAdvertDimensions.GetActive().Count == 0)
                        _currentSelectedDimemsions = new BannerAdvertDimensions(1);
                    else
                        _currentSelectedDimemsions = new BannerAdvertDimensions(BannerAdvertDimensions.GetActive()[0].Id);
                }
                else
                    _currentSelectedDimemsions = new BannerAdvertDimensions((int)Session["CurrentDimensionsId"]);
            }
            return _currentSelectedDimemsions;
        }
        set
        {
            _currentSelectedDimemsions = value;
            Session["CurrentDimensionsId"] = _currentSelectedDimemsions.Id;
        }
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        SuccMessagePanel.Visible = false;

        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        if (viewIndex == 0)
            Response.Redirect("bannersb.aspx");
        else
            DimensionsPlaceHolder.Controls.Clear();

        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";

        if (viewIndex == 1)
            DirectRefsGridView.DataBind();

        Session["YourCampaigns_NewBannerAdvert"] = null;
        createBannerAdvertisement_BannerImage.ImageUrl = "";
        URL.Text = "http://";
    }

    private void DimensionsButton_Click(object sender, EventArgs e)
    {
        var myButton = (sender as Button);
        //Change button style
        foreach (Button b in DimensionsPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        myButton.CssClass = "ViewSelected";

        SucPanel.Visible = false;
        AuctionsPanel.Visible = true;
        BiddingPanel.Visible = false;
        BidsLiteral.Text = "";

        CurrentSelectedDimemsions = new BannerAdvertDimensions(Convert.ToInt32(myButton.ID));
        DimensionsHeaderLiteral.Text = CurrentSelectedDimemsions.ToString();
        AuctionGrid_DataSource_Init(null, null);
        AuctionGrid.DataBind();
    }

    private void CreateDimensionsButtons()
    {
        var dimensionsList = BannerAdvertDimensions.GetActive();
        foreach (var dimensions in dimensionsList)
        {
            var button = new Button();
            button.Text = dimensions.ToString();
            button.Click += new EventHandler(DimensionsButton_Click);
            button.ID = dimensions.Id.ToString();

            DimensionsPlaceHolder.Controls.Add(button);
        }

        var clickButton = DimensionsPlaceHolder.FindControl(CurrentSelectedDimemsions.Id.ToString());
        DimensionsButton_Click(clickButton, null);
    }

    protected void Validate_Captcha(object source, ServerValidateEventArgs args)
    {
        args.IsValid = TitanCaptcha.IsValid;       
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
            if (e.Row.Cells[3].Text.Length > 39)
                e.Row.Cells[3].Text = e.Row.Cells[3].Text.Substring(0, 36) + "...";

            //End mode [14]
            End.Mode Mode = (End.Mode)Convert.ToInt32(e.Row.Cells[14].Text);

            //Status [15]
            AdvertStatus Status = (AdvertStatus)Convert.ToInt32(e.Row.Cells[15].Text);
            e.Row.Cells[15].Text = HtmlCreator.GetColoredStatus(Status);

            //Remove [18]
            if (!Status.CanBeRemoved())
                e.Row.Cells[18].Text = "&nbsp;";

            //Paused = APPROVED
            if (Status == AdvertStatus.Paused)
                e.Row.Cells[15].Text = "<span style=\"color:#\">" + U4000.APPROVED + "</span>";

        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[12].Text = AppSettings.BannerAdverts.ImpressionsEnabled ? L1.IMPRESSIONS : L1.CLICKS;
        }
    }

    protected void AuctionGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            BannerAuction Auction = new BannerAuction(Convert.ToInt32(e.Row.Cells[0].Text));

            // 1 - Display Time
            // 2 - Highest Bid
            // 3 - Auction closes
            // 4 - Button

            e.Row.Cells[1].Text = Auction.DisplayTime;
            e.Row.Cells[2].Text = Auction.HighestBidString;
            e.Row.Cells[3].Text = Auction.Closes;


            if (Auction.Status != BannerAuctionStatus.Opened)
                e.Row.Cells[4].Controls[1].Visible = false;
            else
                ((LiteralControl)e.Row.Cells[4].Controls[2]).Text = " " + U4000.AUCTION;


        }
    }

    protected void DirectRefsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //We want to obey OnSort and OnChart events
        string[] commands = new string[1] { "remove" };

        if (commands.Contains(e.CommandName))
        {
            int index = e.GetSelectedRowIndex() % DirectRefsGridView.PageSize;
            GridViewRow row = DirectRefsGridView.Rows[index];
            string AdId = (row.Cells[1].Text.Trim());
            var Ad = new BannerAdvert(Convert.ToInt32(AdId));

            if (e.CommandName == "remove")
            {
                Ad.Status = AdvertStatus.Deleted;
                Ad.SaveStatus();
                RedirectToStart();
            }
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
                Member User = Member.Current;

                NewBannerAdvert.Advertiser = Advertiser.AsMember(User.Name);
                NewBannerAdvert.Status = AdvertStatusExtensions.GetStartingStatus();
                NewBannerAdvert.TargetUrl = URL.Text;
                NewBannerAdvert.Pack = new BannerAdvertPack();
                NewBannerAdvert.Price = new Money(0);
                NewBannerAdvert.Dimensions = new BannerAdvertDimensions(Convert.ToInt32(BannerTypeRadioButtonList.SelectedValue));
                NewBannerAdvert.Save();

                //Referesh notifications
                SuccMessagePanel.Visible = true;
                SuccMessage.Text = U3501.ADCREATED;

                Response.Redirect("bannersb.aspx?red=camp");

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

            if (!((Banners.TryCreateBannerFromUrl(BannerFileUrlTextBox.Text, out newTemporaryBanner) ||
                   Banner.TryFromStream(createBannerAdvertisement_BannerUpload.PostedFile.InputStream,
                       out newTemporaryBanner))
                && (
                BannerAdvertDimensions.Get(newTemporaryBanner.Width, newTemporaryBanner.Height) != null &&
                BannerAdvertDimensions.Get(newTemporaryBanner.Width, newTemporaryBanner.Height).Id.ToString() == BannerTypeRadioButtonList.SelectedValue)))
                throw new MsgException(U6000.INVALIDBANNERIMAGEORDIMENSIONS);


            newTemporaryBanner.Save(AppSettings.FolderPaths.BannerAdvertImages);

            //deleteOldImageIfExists();

            NewBannerAdvert.BannerImage = newTemporaryBanner;
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
    protected void ImageSubmitValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = createBannerAdvertisement_BannerUpload.HasFile || !string.IsNullOrEmpty(BannerFileUrlTextBox.Text);
    }


    Banner newTemporaryBanner;

    protected void createBannerAdvertisement_BannerUploadSelectedCustomValidator_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = NewBannerAdvert.BannerImage != null;
    }

    private void ShowSuccPanel()
    {
        SuccPanel2.Visible = true;
        SuccLiteral.Text = L1.OP_SUCCESS + ". " + L1.BANNERDURT.Replace("%n%", "3");
    }


    protected void AuctionGrid_DataSource_Init(object sender, EventArgs e)
    {
        AuctionGrid_DataSource.SelectCommand =
        String.Format(@"SELECT * FROM BannerAuctions WHERE DateStart > '{0}' AND BannerType = {1} ORDER BY DateStart ASC",
            DateTime.Now.Add(-AppSettings.BannerAdverts.AuctionTime).ToString("yyyy-MM-dd HH:mm:ss"), CurrentSelectedDimemsions.Id);
    }

    protected void AuctionGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        ParseRowCommand(AuctionGrid, e);
    }

    private void ParseRowCommand(GridView view, GridViewCommandEventArgs e)
    {
        //We want to obey OnSort and OnChart events
        string[] commands = new string[1] { "auction" };

        if (commands.Contains(e.CommandName))
        {
            int index = e.GetSelectedRowIndex() % view.PageSize;
            GridViewRow row = view.Rows[index];
            string Id = (row.Cells[0].Text.Trim());

            var Auction = new BannerAuction(Convert.ToInt32(Id));

            if (e.CommandName == "auction")
            {
                AuctionsPanel.Visible = false;
                BiddingPanel.Visible = true;
                target = Auction;
                GenerateBiddingPanel(view);
                DimensionsPlaceHolder.Controls.Clear();
            }
        }
    }

    private void BindDataToDDL()
    {
        var listPacks = availableOptions;

        var list = new Dictionary<string, string>();

        foreach (BannerAdvert pack in listPacks)
        {
            list.Add(pack.Id.ToString(), pack.TargetUrl);
        }
        ddlOptions.DataSource = list;
        ddlOptions.DataTextField = "Value";
        ddlOptions.DataValueField = "Key";
        ddlOptions.DataBind();
    }

    private void GenerateBiddingPanel(GridView view)
    {
        PlaceBidPanel.Visible = true;
        BannerAuction target = this.target;

        //Generate DDL
        var where = TableHelper.MakeDictionary("CreatorUsername", Member.CurrentName);
        where.Add("Status", (int)AdvertStatus.Active);
        where.Add("BannerAdvertDimensionId", (int)target.BannerType.Id);
        availableOptions = TableHelper.SelectRows<BannerAdvert>(where);

        //if 0, redirect
        if (availableOptions.Count == 0)        
            Response.Redirect(String.Format("~/user/advert/bannersb.aspx?red=new&view={0}", CurrentSelectedDimemsions.Id));

        //Generate Button
        BidButton.Text = U4000.PLACEBID + ": " + target.NextMinBidValue.ToString();
        BindDataToDDL();

        //Generate fields
        GenerateBidFields(target);

        //Check if we have the best bid
        if (target.HighestBid != null && target.HighestBid.Username == Member.CurrentName)
            PlaceBidPanel.Visible = false;
    }

    private void GenerateBidFields(BannerAuction target)
    {
        var bids = target.GetBids();
        for (int i = 0; i < bids.Count; i++)
        {
            bool IsPaid = i == 0 ? false : true;
            if (i < BannerAuctionManager.DisplayedNormalBannerNumber)
                IsPaid = false;

            BidsLiteral.Text += bids[i].ToString(i + 1, IsPaid);
        }
    }

    protected void BidButton_Click(object sender, EventArgs e)
    {
        BannerAuction target = this.target;
        ErrPanel.Visible = false;
        SucPanel.Visible = false;

        try
        {
            BannerAdvert advert = new BannerAdvert(Convert.ToInt32(ddlOptions.SelectedValue));
            Member User = Member.Current;
            Money bidValue = target.NextMinBidValue;

            //Status check
            if (target.Status != BannerAuctionStatus.Opened)
                throw new MsgException(U4000.AUCTIONCLOSED);

            //Balance check
            if (bidValue > User.PurchaseBalance)
                throw new MsgException(L1.NOTENOUGHFUNDS);

            //Take money
            User.SubtractFromPurchaseBalance(bidValue, "Banner bid");
            User.SaveBalances();

            //Add bid         
            BannerBid bid = new BannerBid();
            bid.BidValue = bidValue;
            bid.BannerAdvertId = advert.Id;
            bid.BannerAuctionId = target.Id;
            bid.Username = User.Name;
            BannerAuctionManager.AddBid(bid, target, User);

            //Refresh bids field
            GenerateBidFields(target);
            PlaceBidPanel.Visible = false;

            SucPanel.Visible = true;
            SucMess.Text = U4000.BIDPLACED;

            AuctionGrid.DataBind();
        }
        catch (MsgException ex)
        {
            ErrPanel.Visible = true;
            ErrMess.Text = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }

    private void RedirectToStart()
    {
        Response.Redirect("bannersb.aspx");
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
        createBannerAdvertisement_BannerImage.ImageUrl = null;
        createBannerAdvertisement_BannerUpload.Dispose();
        BannerFileUrlTextBox.Text = "";
        NewBannerAdvert = null;
        DimensionsPlaceHolder.Controls.Clear();
    }
}