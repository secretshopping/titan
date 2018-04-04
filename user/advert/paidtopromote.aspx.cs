using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Utils;
using Prem.PTC.Members;
using Prem.PTC.Advertising;
using Resources;
using System.Text;
using Titan.PaidToPromote;
using System.Globalization;

public partial class user_advert_paidtopromote : System.Web.UI.Page
{
    private string validURL;

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.AdvertPaidToPromoteEnabled);

        //Get Packs
        if (!Page.IsPostBack)
        {
            BindDataToDDL();
            BindDataCountriesToDDL();
        }

        LangAdder.Add(CreateAdButton, L1.BUY);
        LangAdder.Add(Button1, L1.MANAGE);
        LangAdder.Add(Button2, L1.NEWCAMPAIGN);
        LangAdder.Add(PasswordRequired, L1.REQ_URL, true);
        LangAdder.Add(RegularExpressionValidator2, L1.ER_BADURL, true);
        HintAdder.Add(URL, L1.H_URL);
        HintAdder.Add(lblGeolocation, L1.GEOHINT);
        
        PriceGeo.Text = AppSettings.PaidToPromote.GeolocationPrice.ToString();
        TitleLiteral.Text = U6009.PAIDTOPROMOTE;
        SubLiteral.Text = L1.ADVADSINFO; //TO CHANGE
        UserName.Text = Member.CurrentId.ToString();
        
        URL.Attributes.Add("onfocus", "if (getElementById('" + URL.ClientID + "').value == 'http://') getElementById('" + URL.ClientID + "').value = ''");
        chbGeolocation.Attributes.Add("onclick", "ManageGeoEvent();");
        RegularExpressionValidator2.ValidationExpression = RegexExpressions.AdWebsiteUrl;
    }

    protected void MenuButton_Click(object sender, EventArgs e)
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
                AppSettings.DemoCheck();

                if (URL.Enabled)
                    throw new MsgException(U4200.CHECKURL);

                var user = Member.CurrentInCache;
                var selectedPack = new PaidToPromotePack(Convert.ToInt32(ddlOptions.SelectedValue));
                var advert = new PaidToPromoteAdvert();
                var adCost = selectedPack.Price;

                advert.CreatorId = user.Id;
                advert.PackId = selectedPack.Id;
                //TODO ??
                //advert.Status = AdvertStatusExtensions.GetStartingStatus();                
                advert.TargetUrl = URL.Text;               

                if (chbGeolocation.Checked)
                {
                    adCost += AppSettings.PaidToPromote.GeolocationPrice;
                    
                    var CTable = GeoCountries.Items;
                    var geoUList = GeolocationUtils.GeoCountData.Keys;
                    var sb = new StringBuilder();

                    foreach (ListItem item in CTable)
                    {
                        if (geoUList.Contains(item.Value))
                        {
                            var countryCode = CountryManager.GetCountryCode(item.Value);
                            if (!string.IsNullOrWhiteSpace(countryCode))
                            {
                                sb.Append(CountryManager.GetCountryCode(item.Value));
                                sb.Append(",");
                            }
                        }
                    }

                    advert.GeolocatedCC = sb.ToString().Trim(',');
                }
                else                
                    advert.GeolocatedCC = string.Empty;

                if (!PaidToPromoteManager.IsEmptySlotInRotation)
                    throw new MsgException(U6009.NOEMPTYSLOTINROTATION);

                PurchaseOption.ChargeBalance(user, adCost, TargetBalanceRadioButtonList.Feature, TargetBalanceRadioButtonList.TargetBalance, "Paid To Promote Ad Purchase");

                advert.Save();

                History.AddPurchase(user.Name, adCost, "Paid To Promote Campaign");
                //MatrixBase matrix = MatrixFactory.GetMatrix();
                //if (matrix != null)
                //{
                //    matrix.TryAddMember(User, AdvertType.PTC);
                //    matrix.Credit(User, AdCost);
                //}

                URL.Text = "";
                chbGeolocation.Checked = false;

                SuccMessagePanel.Visible = true;
                SuccMessage.Text = U3501.ADCREATED;
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

    protected void View1_Activate(object sender, EventArgs e)
    {
        if (!PaidToPromoteManager.IsEmptySlotInRotation)
        {
            CantAddAdPlaceHolder.Visible = true;
            CantAddAdLabel.Text = U6009.NOEMPTYSLOTINROTATION;
        }
        else
        {
            AddAdPlaceHolder.Visible = true;

            var listPacks = PaidToPromoteManager.GetAllActivePacks;

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
            }
        }
    }

    protected void BtnAdd_Click(object sender, EventArgs e)
    {
        try
        {
            GeoCountries.Items.Add(new ListItem(AllCountries.SelectedItem.Text, AllCountries.SelectedItem.Value));
            OrderItems(GeoCountries);
            AllCountries.Items.Remove(AllCountries.SelectedItem);
        }
        catch (Exception ex) { }
    }

    protected void BtnRemove_Click(object sender, EventArgs e)
    {
        try
        {
            AllCountries.Items.Add(new ListItem(GeoCountries.SelectedItem.Text, GeoCountries.SelectedItem.Value));
            OrderItems(AllCountries);
            GeoCountries.Items.Remove(GeoCountries.SelectedItem);
        }
        catch (Exception ex) { }
    }

    protected void BtnAddAll_Click(object sender, EventArgs e)
    {
        try
        {
            GeoCountries.Items.AddRange(AllCountries.Items.Cast<ListItem>().ToArray());
            OrderItems(GeoCountries);
            AllCountries.Items.Clear();
        }
        catch (Exception ex) { }
    }

    protected void BtnRemoveAll_Click(object sender, EventArgs e)
    {
        try
        {
            AllCountries.Items.AddRange(GeoCountries.Items.Cast<ListItem>().ToArray());
            OrderItems(AllCountries);
            GeoCountries.Items.Clear();
        }
        catch (Exception ex) { }
    }

    protected void AdvertsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //[0] Id - hidden
            var ptpAd = new PaidToPromoteAdvert(Convert.ToInt32(e.Row.Cells[0].Text));

            //[1] PackId - hidden
            var ptpPack = new PaidToPromotePack(Convert.ToInt32(e.Row.Cells[1].Text));

            //[2] TargetUrl
            var link = e.Row.Cells[2].Text;
            e.Row.Cells[2].Text = string.Format("<a href='{0}' target='_blank'>{0}</a>", link);

            //[3] CreationDate
            //OK

            //[4] FinishDate - can be null
            //OK

            //[5] Creation Date - change it to progress
            e.Row.Cells[5].Text = HtmlCreator.GenerateProgressHTML(0m, ptpAd.EndValue, (decimal)ptpPack.Ends.Value);
            
            //[6] EndValue - change to End Mode
            var Mode = ptpPack.Ends.EndMode;
            e.Row.Cells[6].Text = Mode.ToString();

            //[7] EndValue(title: Click/Days) - Depend on Pack change it to click/days
            if (Mode == End.Mode.Clicks)
                e.Row.Cells[7].Text += string.Format(" {0}", L1.CLICKS);
            else
                e.Row.Cells[7].Text += string.Format(" {0}", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(L1.DAYS.ToLower()));

            //[8] GeolocatedCC - check all geo and add checkBox
            if (ptpAd.IsGeo())
                e.Row.Cells[8].Text = HtmlCreator.GetCheckboxCheckedImage();
            else
                e.Row.Cells[8].Text = HtmlCreator.GetCheckboxUncheckedImage();

            //[9] Status
            var status = (AdvertStatus)Convert.ToInt32(e.Row.Cells[9].Text);
            e.Row.Cells[9].Text = HtmlCreator.GetColoredStatus(status);

            //[10] Remove button           
            //OK  
        }
        else if (e.Row.RowType == DataControlRowType.Header)
        { 
            AdvertsGridView.Columns[2].HeaderText = U6008.TARGETURL;
            AdvertsGridView.Columns[3].HeaderText = L1.CREATED;
            AdvertsGridView.Columns[4].HeaderText = L1.FINISHED;
            AdvertsGridView.Columns[5].HeaderText = L1.PROGRESS;
            AdvertsGridView.Columns[7].HeaderText = string.Format("{0}/{1}", L1.CLICKS, CultureInfo.CurrentCulture.TextInfo.ToTitleCase(L1.DAYS.ToLower()));
            AdvertsGridView.Columns[9].HeaderText = L1.STATUS;
        }
    }

    protected void AdvertsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "remove")
        {
            ErrorMessagePanel2.Visible = false;

            var index = e.GetSelectedRowIndex() % AdvertsGridView.PageSize;
            var row = AdvertsGridView.Rows[index];
            var AdId = (row.Cells[0].Text.Trim());
            var Ad = new PaidToPromoteAdvert(Convert.ToInt32(AdId))
            {
                Status = AdvertStatus.Deleted
            };
            Ad.Save();

            AdvertsGridView.DataBind();
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

    private void OrderItems(ListBox list)
    {
        var items = list.Items.Cast<ListItem>().Select(x => x).OrderBy(x => x.Value).ToList();
        list.Items.Clear();
        list.Items.AddRange(items.ToArray());
    }

    private void BindDataToDDL()
    {
        var listPacks = PaidToPromoteManager.GetAllActivePacks;
        listPacks.Sort(PaidToPromoteManager.PackComparision);

        var list = new Dictionary<string, string>();

        foreach (PaidToPromotePack pack in listPacks)
        {
            var ends = L1.NEVER;
            if (pack.Ends.EndMode == End.Mode.Days)
                ends = pack.Ends.Value.ToString() + " " + L1.DAYS;
            else if (pack.Ends.EndMode == End.Mode.Clicks)
                ends = pack.Ends.Value.ToString() + " " + L1.VIEWS;

            var value = string.Format("{0} ({1})s - {2}", ends, AppSettings.PaidToPromote.AdDuration.ToString(), pack.Price.ToString());


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

        AllCountries.DataSource = dictlist;
        AllCountries.DataTextField = "Value";
        AllCountries.DataValueField = "Key";
        AllCountries.DataBind();
    }
}