using System;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Utils;
using Prem.PTC.Members;
using Prem.PTC.Advertising;
using Resources;
using Titan.Matrix;

public partial class About : System.Web.UI.Page
{
    new Member User;
    string validURL;
    private const int LoginAdCreditsPrice = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        User = Member.Current;
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.AdvertisersRoleEnabled && AppSettings.TitanFeatures.AdvertLoginAdsEnabled && User.IsAdvertiser);

        RegularExpressionValidator2.ValidationExpression = RegexExpressions.AdWebsiteUrl;

        SubLiteral.Text = L1.ADVADSINFO;
        LangAdder.Add(CreateAdButton, L1.SEND);

        //Lang & Hint
        LangAdder.Add(Button1, L1.MANAGE);
        LangAdder.Add(Button2, L1.NEWCAMPAIGN);
        LangAdder.Add(RegularExpressionValidator2, L1.ER_BADURL, true);
        LangAdder.Add(UrlRequired, L1.ER_BADURL, true);
        HintAdder.Add(URL, L1.H_URL);
        HintAdder.Add(lblGeolocation, L1.GEOHINT);

        //JS changes
        URL.Attributes.Add("onfocus", "if (getElementById('" + URL.ClientID + "').value == 'http://') getElementById('" + URL.ClientID + "').value = ''");

        chbGeolocation.Attributes.Add("onclick", "ManageGeoEvent();");

        if (!Page.IsPostBack)
        {
            BindDataCountriesToDDL();

            if (AppSettings.LoginAds.IsGeolocationEnabled)
                GeolocationPlaceholder.Visible = true;

            var loginAdsCreditsEnabled = AppSettings.LoginAds.LoginAdsCreditsEnabled;

            LoginAdsCreditsPlaceHolder.Visible = loginAdsCreditsEnabled;
            string extraViews = string.Format(@"({0} {1})", User.LoginAdsCredits, U4200.AVAILABLE);
            AvailableLoginAdsCreditsLiteral.Text = extraViews;
            
            if(loginAdsCreditsEnabled)
                PriceLiteral.Text = string.Format("{0} / {1} {2}", AppSettings.LoginAds.Price.ToString(), LoginAdCreditsPrice, U5008.LOGINADSCREDITS);
            else
                PriceLiteral.Text = AppSettings.LoginAds.Price.ToString();
        }

        LoginAdsCreditsCheckBox_CheckedChanged(null, null);
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

        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";
    }

    protected void CreateAdButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        SuccMessagePanel.Visible = false;

        if (Page.IsValid)
        {
            try
            {
                User = Member.Current;

                AppSettings.DemoCheck();

                if (URL.Enabled)
                    throw new MsgException(U4200.CHECKURL);                

                var Ad = new LoginAd
                {
                    TargetUrl = URL.Text,
                };

                if (AppSettings.LoginAds.LoginAdsCreditsEnabled && LoginAdsCreditsCheckBox.Checked)                
                    //Login Ads Credits
                    Ad.PricePaid = new Money ((int)LoginAdCreditsPrice);                
                else
                    Ad.PricePaid = AppSettings.LoginAds.Price;                

                if (chbGeolocation.Checked && AppSettings.LoginAds.IsGeolocationEnabled)
                {
                    //Now get it from client-state
                    var countriesSelectedDelimited = Request.Form[GeoCountriesValues.Name].Substring(1);
                    GeolocationUnit unit = GeolocationUnit.ParseFromCountries(countriesSelectedDelimited);

                    //Cities
                    unit.Cities = GeoCities.Text;
                    unit.MinAge = Convert.ToInt32(GeoAgeMin.Text);
                    unit.MaxAge = Convert.ToInt32(GeoAgeMax.Text);
                    unit.Gender = (Gender)Convert.ToInt32(GeoGenderList.SelectedValue);

                    Ad.AddGeolocation(unit);
                }

                Ad.CreatorUserId = User.Id;
                Ad.Status = AdvertStatusExtensions.GetStartingStatus();

                var displayDate = AdDisplayDateCalendar.SelectedDate;

                if (displayDate <= DateTime.Now.Date)
                    throw new MsgException(U4200.SELECTDIFFERENTDISPLAYDATE);

                if (LoginManager.GetNumberOfAdsPurchasedForDay(displayDate) >= AppSettings.LoginAds.AdsPerDay)
                    throw new MsgException(U4200.SELECTDIFFERENTDISPLAYDATE);

                Ad.DisplayDate = displayDate;
                Ad.PurchaseDate = DateTime.Now;

                if (AppSettings.LoginAds.LoginAdsCreditsEnabled && LoginAdsCreditsCheckBox.Checked)
                {
                    Ad.TargetBalance = PurchaseBalances.LoginAdsCredits;
                    PurchaseOption.ChargeBalance(User, Ad.PricePaid, PurchaseBalances.LoginAdsCredits, U5001.LOGINAD);
                    
                    string extraViews = string.Format(@"({0} {1})", User.LoginAdsCredits, U4200.AVAILABLE);
                    AvailableLoginAdsCreditsLiteral.Text = extraViews;
                    History.AddPurchase(User.Name, Ad.PricePaid.AsPoints(), U5001.LOGINAD);
                }
                else
                {
                    Ad.TargetBalance = TargetBalanceRadioButtonList.TargetBalance;
                    PurchaseOption.ChargeBalance(User, Ad.PricePaid, TargetBalanceRadioButtonList.Feature, TargetBalanceRadioButtonList.TargetBalance, U5001.LOGINAD);

                    //Pools
                    PoolDistributionManager.AddProfit(ProfitSource.LoginAds, Ad.PricePaid);
                    //Add history entry 1
                    History.AddPurchase(User.Name, Ad.PricePaid, U5001.LOGINAD);
                    MatrixBase.TryAddMemberAndCredit(User, Ad.PricePaid, AdvertType.Login);
                }

                //Save advert
                Ad.Save();

                SuccMessagePanel.Visible = true;
                SuccMessage.Text = U4200.ADAWAITSAPPROVAL;

                URL.Text = "http://";
                URL.Enabled = true;
                CheckURLButton.Visible = true;
            }
            catch (MsgException ex)
            {
                SuccMessagePanel.Visible = false;
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

    protected void LoginAdsGridViewDataSource_Init(object sender, EventArgs e)
    {
        LoginAdsGridViewDataSource.SelectCommand = string.Format(@"SELECT * FROM LoginAds WHERE CreatorUserId = {0} AND Status != {1} ORDER BY Status",
            Member.CurrentId, (int)AdvertStatus.Deleted);
    }

    protected void LoginAdsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //Status [4]
            AdvertStatus Status = (AdvertStatus)Convert.ToInt32(e.Row.Cells[4].Text);
            e.Row.Cells[4].Text = HtmlCreator.GetColoredStatus(Status);

            //Display Date[2]
            e.Row.Cells[2].Text = DateTime.Parse(e.Row.Cells[2].Text).ToShortDateString();

            //start [5] stop [6] remove[7]
            if (Status != AdvertStatus.Paused)
                e.Row.Cells[5].Text = "&nbsp;";

            if (Status != AdvertStatus.Active)
                e.Row.Cells[6].Text = "&nbsp;";

            if (Status != AdvertStatus.Rejected && Status != AdvertStatus.Stopped)
                e.Row.Cells[7].Text = "&nbsp;";
        }
    }

    protected void AdDisplayDateCalendar_DayRender(object sender, DayRenderEventArgs e)
    {
        if (e.Day.Date <= DateTime.Now.Date)
        {
            e.Day.IsSelectable = false;
            e.Cell.CssClass = "not-available";
            return;
        }

        bool isFull = LoginManager.GetNumberOfAdsPurchasedForDay(e.Day.Date) >= AppSettings.LoginAds.AdsPerDay;
        if (isFull)
        {
            e.Cell.CssClass = "not-available alert alert-danger";
            e.Day.IsSelectable = false;
        }
        else        
            e.Cell.CssClass = "available";

        if (e.Day.IsSelected)
            e.Cell.CssClass = "active";
    }

    protected void AdDisplayDateCalendar_Load(object sender, EventArgs e)
    {
        //AdDisplayDateCalendar.TitleStyle.BackColor = Color.FromName(AppSettings.Site.MainColor);
    }

    protected void LoginAdsCreditsCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        TargetBalanceRadioButtonList.Visible = !LoginAdsCreditsCheckBox.Checked;
    }

    protected void LoginAdsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int index = e.GetSelectedRowIndex() % LoginAdsGridView.PageSize;
        GridViewRow row = LoginAdsGridView.Rows[index];
        var Ad = new LoginAd(Convert.ToInt32(row.Cells[0].Text.Trim()));

        if (e.CommandName == "start")
        {
            Ad.Status = AdvertStatus.Active;
            Ad.Save();
            LoginAdsGridView.DataBind();
        }
        else if (e.CommandName == "stop")
        {
            Ad.Status = AdvertStatus.Paused;
            Ad.Save();
            LoginAdsGridView.DataBind();
        }
        else if (e.CommandName == "remove")
        {
            Ad.Status = AdvertStatus.Deleted;
            Ad.Save();
            LoginAdsGridView.DataBind();
        }        
    }
}