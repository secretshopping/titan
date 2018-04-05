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
using System.Text;
using Titan.Publish.PTCOfferWalls;

public partial class user_advert_ptcofferwalls : System.Web.UI.Page
{
    string validURL;
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.AdvertisersRoleEnabled && AppSettings.TitanFeatures.AdvertPtcOfferWallEnabled && Member.CurrentInCache.IsAdvertiser);

        if (!IsPostBack)
        {
            AddLang();
            BindDataCountriesToDDL();
            GeolocationCheckBox.Attributes.Add("onclick", "ManageGeoEvent();");
        }

        ErrorMessagePanel.Visible = false;
    }

    private void AddLang()
    {
        MenuButtonCreateOfferWall.Text = L1.ADDNEW;
        MenuButtonMyOfferWalls.Text = L1.MANAGE;
        CreateOfferWallButton.Text = L1.BUY;
        LangAdder.Add(TitleRequiredFieldValidator, L1.REQ_TITLE, true);
        LangAdder.Add(DescriptionRequiredFieldValidator, L1.REQ_DESC, true);
        LangAdder.Add(MaxSingleUserDailyViewsRequiredValidator, U6003.REQ_MAXDAILYVIEWS, true);
        LangAdder.Add(MaxSingleUserDailyViewsCustomValidator, U6003.ERR_MAXDAILYVIEWS, true);
        
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


    #region Geolocation
    private void BindDataCountriesToDDL()
    {
        AllCountriesListBox.DataSource = GeolocationUtils.GetCountriesData();
        AllCountriesListBox.DataTextField = "Value";
        AllCountriesListBox.DataValueField = "Key";
        AllCountriesListBox.DataBind();
    }
    protected void AddCountryButton_Click(object sender, EventArgs e)
    {
        try
        {
            GeoCountriesListBox.Items.Add(new ListItem(AllCountriesListBox.SelectedItem.Text, AllCountriesListBox.SelectedItem.Value));
            OrderItems(GeoCountriesListBox);
            AllCountriesListBox.Items.Remove(AllCountriesListBox.SelectedItem);
        }
        catch (Exception ex) { }
    }

    protected void RemoveCountryButton_Click(object sender, EventArgs e)
    {
        try
        {
            AllCountriesListBox.Items.Add(new ListItem(GeoCountriesListBox.SelectedItem.Text, GeoCountriesListBox.SelectedItem.Value));
            OrderItems(AllCountriesListBox);
            GeoCountriesListBox.Items.Remove(GeoCountriesListBox.SelectedItem);
        }
        catch (Exception ex) { }
    }

    protected void AddAllCountriesButton_Click(object sender, EventArgs e)
    {
        try
        {
            GeoCountriesListBox.Items.AddRange(AllCountriesListBox.Items.Cast<ListItem>().ToArray());
            OrderItems(GeoCountriesListBox);
            AllCountriesListBox.Items.Clear();
        }
        catch (Exception ex) { }
    }

    protected void RemoveAllCountriesButton_Click(object sender, EventArgs e)
    {
        try
        {
            AllCountriesListBox.Items.AddRange(GeoCountriesListBox.Items.Cast<ListItem>().ToArray());
            OrderItems(AllCountriesListBox);
            GeoCountriesListBox.Items.Clear();

        }
        catch (Exception ex) { }
    }

    private void OrderItems(ListBox list)
    {
        var items = list.Items.Cast<ListItem>().Select(x => x).OrderBy(x => x.Value).ToList();
        list.Items.Clear();
        list.Items.AddRange(items.ToArray());
    }
    #endregion
    protected void CreateOfferWallButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (Page.IsValid)
            {
                AppSettings.DemoCheck();

                ErrorMessagePanel.Visible = SuccessMessagePanel.Visible = false;

                string title = InputChecker.HtmlEncode(TitleTextBox.Text, TitleTextBox.MaxLength, L1.TITLE);
                string description = InputChecker.HtmlEncode(DescriptionTextBox.Text, DescriptionTextBox.MaxLength, L1.DESCRIPTION);
                var pack = new PTCOfferWallPack(Convert.ToInt32(PacksDDL.SelectedValue));
                
                List<UserUrl> urlIds = UserUrlsCheckBoxList.Items.Cast<ListItem>()
                                        .Where(x => x.Selected)
                                        .Select(x => new UserUrl(Convert.ToInt32(x.Value)))
                                        .ToList();

                GeolocationUnit geolocationUnit = null;

                if (GeolocationCheckBox.Checked)
                {
                    var validCountries = GeolocationUtils.GeoCountData.Keys;
                    var countryNames = new StringBuilder();

                    foreach (ListItem item in GeoCountriesListBox.Items)
                    {
                        if (validCountries.Contains<string>(item.Value))
                        {
                            countryNames.Append(item.Value);
                            countryNames.Append("#");
                        }
                    }

                    var minAge = Convert.ToInt32(GeoAgeMin.Text);
                    var maxAge = Convert.ToInt32(GeoAgeMax.Text);
                    var gender = (Gender)Convert.ToInt32(GeoGenderList.SelectedValue);
                    var countryCodes = GeolocationUnit.ParseFromCountriesString(countryNames.ToString());
                    var cities = string.Empty;
                    geolocationUnit = new GeolocationUnit(countryCodes, cities, minAge, maxAge, gender);
                }

                var pcAllowed = PcAllowedCheckBox.Checked;
                var mobileAllowed = MobileAllowedCheckBox.Checked;
                var autosurfEnabled = AutosurfAllowedCheckBox.Checked;
                var maxSingleUserDailyViews = Convert.ToInt32(MaxSingleUserDailyViewsTextBox.Text);
                PTCOfferWall.Buy(Member.Current, pack, TargetBalanceRadioButtonList.TargetBalance, urlIds, title,
                    description, geolocationUnit, pcAllowed, mobileAllowed, autosurfEnabled, maxSingleUserDailyViews);

                SuccessMessagePanel.Visible = true;
                SuccessMessage.Text = U3501.ADCREATED;

                ClearAll();
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

    private void ClearAll()
    {
        TitleTextBox.Text = string.Empty;
        DescriptionTextBox.Text = string.Empty;
        AllCountriesListBox.Items.AddRange(GeoCountriesListBox.Items.Cast<ListItem>().ToArray());
        OrderItems(AllCountriesListBox);
        GeolocationCheckBox.Checked = false;
        GeoCountriesListBox.Items.Clear();
        GeoAgeMin.Text = "0";
        GeoAgeMax.Text = "0";
        MaxSingleUserDailyViewsTextBox.Text = string.Empty;
    }

    protected void PacksDDL_Init(object sender, EventArgs e)
    {
        var packs = PTCOfferWallPack.GetActive();
        var listItems = new List<ListItem>();
        foreach (var p in packs)
        {
            listItems.Add(
                new ListItem
                {
                    Value = p.Id.ToString(),
                    Text = string.Format("{0} {1} ({2} URL(s) x {3}s) - {4}", p.CompletionTimes, U6000.SUBMISSIONS, p.Adverts, p.DisplayTime, p.Price)
                });
        }

        PacksDDL.Controls.Clear();
        PacksDDL.Items.AddRange(listItems.ToArray());
    }

    protected void MyOfferWallsGridView_DataBound(object sender, EventArgs e)
    {
        MyOfferWallsGridView.Columns[1].HeaderText = L1.TITLE;
        MyOfferWallsGridView.Columns[2].HeaderText = L1.DESCRIPTION;
        MyOfferWallsGridView.Columns[3].HeaderText = L1.PROGRESS;
        MyOfferWallsGridView.Columns[4].HeaderText = U4000.DISPLAYTIME;
        MyOfferWallsGridView.Columns[5].HeaderText = "URLs";
        MyOfferWallsGridView.Columns[6].HeaderText = L1.STATUS;
    }

    protected void MyOfferWallsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            var offerWall = new PTCOfferWall(Convert.ToInt32(e.Row.Cells[0].Text));

            e.Row.Cells[3].Text = HtmlCreator.GenerateCPAAdProgressHTML(offerWall.CompletionTimes, offerWall.CompletionTimesBought, U6000.SUBMISSIONS);

            GridView urlsGridView = e.Row.FindControl("MyOfferWallsUrlsGridView") as GridView;

            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                urlsGridView.DataSource = bridge.Instance.ExecuteRawCommandToDataTable(
                    @"SELECT urls.Url 
                    FROM PTCOfferWallsUserUrls mappings 
                    JOIN UserUrls urls 
                    ON mappings.UserUrlId = urls.Id 
                    WHERE mappings.PTCOfferWallId = " + offerWall.Id);
            }
            urlsGridView.DataBind();
        }
    }

    protected void MyOfferWallsGridView_DataSource_Init(object sender, EventArgs e)
    {
        MyOfferWallsGridView_DataSource.SelectCommand =
            string.Format(@"SELECT * FROM PTCOfferWalls WHERE UserId = {0}", Member.CurrentId);
    }

    protected void MyOfferWallsView_Activate(object sender, EventArgs e)
    {
        MyOfferWallsGridView.DataBind();
    }

    protected void UserUrlsCheckBoxList_Init(object sender, EventArgs e)
    {
        var urls = UserUrl.GetActive(Member.CurrentId);

        foreach (var url in urls)
        {
            UserUrlsCheckBoxList.Items.Add(new ListItem(url.Url, url.Id.ToString()));
        }
        UserUrlsCheckBoxList.DataBind();
    }

    protected void CreateOfferWallView_Activate(object sender, EventArgs e)
    {
        bool packsConfigured = PTCOfferWall.ArePacksConfigured();
        bool userHasActiveUrls = UserUrl.UserHasActive(Member.CurrentId);

        CreateOfferWallsPlaceHolder.Visible = packsConfigured && userHasActiveUrls;

        if (!CreateOfferWallsPlaceHolder.Visible)
        {
            NewOfferWallUnavailable.Visible = true;
            NewOfferWallUnavailable.HeaderText = U6000.NEWADVERTUNAVAILABLEHEADER;
            string reason = !packsConfigured ? U6000.NEWADVERTUNAVAILABLEREASON : U6002.PLEASEADDURLSFIRST;

            NewOfferWallUnavailable.Reason = reason;
        }
        else
            NewOfferWallUnavailable.Visible = false;
    }

    protected void MaxSingleUserDailyViewsCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        int maxDailyViews = 0;
        args.IsValid = int.TryParse(MaxSingleUserDailyViewsTextBox.Text, out maxDailyViews) && maxDailyViews > 0;
    }
}