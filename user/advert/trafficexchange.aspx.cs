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
using Prem.PTC.Payments;
using Prem.PTC.Advertising;
using Resources;

public partial class About : System.Web.UI.Page
{
    public string jsSelectAllCode;
    public List<TrafficExchangeAdvertPack> availableOptions;

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.AdvertisersRoleEnabled && AppSettings.TitanFeatures.AdvertTrafficExchangeEnabled 
            && Member.CurrentInCache.IsAdvertiser);

        RegularExpressionValidator2.ValidationExpression = RegexExpressions.AdWebsiteUrl;
        if (Member.IsLogged)
        {
            UserName.Text = Member.CurrentName;            

            SubLiteral.Text = L1.ADVADSINFO;
            LangAdder.Add(CreateAdButton, L1.SEND);

            TrafficSourceList.Items[0].Text = " " + U4000.THISWEBSITE + ": " + AppSettings.Site.Url.Replace("http://", "").Replace("/", "");
            TrafficSourceList.Items[1].Text = " " + U4000.ANONDIRECT;

            //Lang & Hint
            LangAdder.Add(Button1, L1.MANAGE);
            LangAdder.Add(Button2, L1.NEWCAMPAIGN);
            LangAdder.Add(RegularExpressionValidator1, L1.ER_INVALIDTITLE, true);
            LangAdder.Add(UserNameRequired, L1.REQ_TITLE, true);
            HintAdder.Add(Title, L1.H_TITLE);

            LangAdder.Add(PasswordRequired, L1.REQ_URL, true);
            LangAdder.Add(RegularExpressionValidator2, L1.ER_BADURL, true);
            HintAdder.Add(URL, L1.H_URL);
            HintAdder.Add(lblGeolocation, L1.GEOHINT);

            //JS changes
            URL.Attributes.Add("onfocus", "if (getElementById('" + URL.ClientID + "').value == 'http://') getElementById('" + URL.ClientID + "').value = ''");
            Title.Attributes.Add("onchange", "$('#Abox1 .ABtitle').html($(this).val());");


            chbGeolocation.Attributes.Add("onclick", "updatePrice(); ManageGeoEvent();");
            ddlOptions.Attributes.Add("onchange", "updatePrice();");

            //Read prices
            PriceGeo.Text = AppSettings.PtcAdverts.GeolocationCost.ToString();

            UsersTrafficBalanceLabel.Text = string.Format(U6008.YOURTRAFFICBALANCE, Member.CurrentInCache.TrafficBalance);

            //Get Packs
            if (!Page.IsPostBack)
            {
                availableOptions = TableHelper.SelectRows<TrafficExchangeAdvertPack>(TableHelper.MakeDictionary("IsVisibleByMembers", true));
                BindDataToDDL();
                BindDataCountriesToDDL();
            }
        }
    }

    private void BindDataToDDL()
    {
        var listPacks = availableOptions;

        var list = new Dictionary<string, string>();
        foreach (TrafficExchangeAdvertPack pack in listPacks)
        {
            string ends = L1.NEVER;
            if (pack.Ends.EndMode == End.Mode.Days)
                ends = pack.Ends.Value.ToString() + " " + L1.DAYS;
            else if (pack.Ends.EndMode == End.Mode.Clicks)
                ends = pack.Ends.Value.ToString() + " " + L1.VIEWS;

            string timeBetweenViews = pack.TimeBetweenViewsInMinutes.ToString() + U4200.MINUTES;
            list.Add(pack.Id.ToString(), ends + " (" + pack.DisplayTime.TotalSeconds.ToString() + "s) - " + pack.Price.ToString() + 
                ", " + U4200.CANBEVIEWEDEVERY + " " + timeBetweenViews);
        }
        ddlOptions.DataSource = list;
        ddlOptions.DataTextField = "Value";
        ddlOptions.DataValueField = "Key";
        ddlOptions.DataBind();
    }

    private void BindDataCountriesToDDL()
    {

        var dictlist = new Dictionary<string, string>();

        foreach (string s in GeolocationUtils.GeoCountData.Keys)
        {
            dictlist.Add(s, s + " (" + GeolocationUtils.GeoCountData[s] + ")");
        }

        AllCountries.DataSource = dictlist;
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
            Response.Redirect("trafficexchange.aspx");

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
            TrafficExchangeAdvert Ad = new TrafficExchangeAdvert(Convert.ToInt32(e.Row.Cells[1].Text));

            //Description [22]
            if (string.IsNullOrEmpty(e.Row.Cells[22].Text))
                e.Row.Cells[22].Text = HtmlCreator.GetCheckboxUncheckedImage();
            else
                e.Row.Cells[22].Text = HtmlCreator.GetCheckboxCheckedImage();

            //End mode [16]
            End.Mode Mode = (End.Mode)Convert.ToInt32(e.Row.Cells[16].Text);

            //Status [24]
            AdvertStatus Status = (AdvertStatus)Convert.ToInt32(e.Row.Cells[24].Text);
            e.Row.Cells[24].Text = HtmlCreator.GetColoredStatus(Status);

            //Subpages [25]
            e.Row.Cells[25].Text = "0";
            var splist = TableHelper.SelectRows<TrafficExchangeSubpage>(TableHelper.MakeDictionary("PtcAdId", Ad.Id));
            if (splist.Count > 0)
            {
                e.Row.Cells[25].Text = splist.Count + " (hover)";
                foreach (var elem in splist)
                {
                    e.Row.Cells[25].ToolTip += Ad.TargetUrl + elem.SubPage + "\n";
                }
            }

            //Pack [3]
            AdvertPack Pack = new TrafficExchangeAdvertPack(Convert.ToInt32(e.Row.Cells[3].Text));

            //Displaytime [17]
            e.Row.Cells[17].Text += "s";

            //Shorten url [4]
            if (e.Row.Cells[4].Text.Length > 18)
                e.Row.Cells[4].Text = e.Row.Cells[4].Text.Substring(0, 15) + "...";

            //TItle [5]
            e.Row.Cells[5].Text = e.Row.Cells[5].Text.Replace("&lt;", "<");
            e.Row.Cells[5].Text = e.Row.Cells[5].Text.Replace("&gt;", ">");

            //Progress [11]
            e.Row.Cells[11].Text = HtmlCreator.GenerateAdProgressHTML(Ad).Replace("clicks", L1.CLICKSSMALL).Replace("days", L1.DAYS);

            //Geolocation check [23]
            if (Ad.IsGeolocated)
                e.Row.Cells[23].Text = HtmlCreator.GetCheckboxCheckedImage();
            else
                e.Row.Cells[23].Text = HtmlCreator.GetCheckboxUncheckedImage();

            //Add % progress [12]
            e.Row.Cells[12].Text = Ad.ProgressInPercent.ToString() + "%";

            //Add total views [13]
            e.Row.Cells[13].Text = e.Row.Cells[14].Text;

            // Start[27] Pause[28] Add [29] Remove[31]
            if (Status != AdvertStatus.Paused)
                e.Row.Cells[27].Text = "&nbsp;";

            if (Status != AdvertStatus.Active)
                e.Row.Cells[28].Text = "&nbsp;";

            if (Status != AdvertStatus.Finished)
                e.Row.Cells[29].Text = "&nbsp;";

            if (!Status.CanBeRemoved())
                e.Row.Cells[31].Text = "&nbsp;";
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

                if (Member.IsLogged)
                    User = Member.Logged(Context);
                else
                    Response.Redirect("~/default.aspx");

                if (URL.Enabled)
                    throw new MsgException(U4200.CHECKURL);

                TrafficExchangeAdvert Ad = new TrafficExchangeAdvert();
                TrafficExchangeAdvertPack Pack = new TrafficExchangeAdvertPack(Int32.Parse(ddlOptions.SelectedValue));
                Money AdCost = Pack.Price;

                //Set basics
                Ad.Title = InputChecker.HtmlEncode(Title.Text, Title.MaxLength, L1.TITLE);
                Ad.TargetUrl = URL.Text;

                if (TrafficSourceList.SelectedValue == "Anon")
                    Ad.Description = TrafficSource.Anonymous;
                else
                    Ad.Description = TrafficSource.ThisWebsite;

                if (chbGeolocation.Checked)
                {
                    AdCost += AppSettings.PtcAdverts.GeolocationCost;

                    //Now get it from client-state
                    var CTable = Request.Form[GeoCountriesValues.Name].Substring(1).Split('#');
                    var geoUList = GeolocationUtils.GeoCountData.Keys;

                    foreach (string s in CTable)
                    {
                        if (geoUList.Contains<string>(s))
                            Ad.GeolocatedCountries +=  CountryManager.GetCountryCode(s) + ",";
                    }
                }

                if (Member.IsLogged)
                {
                    //Take money and save the user
                    var note = TitanFeatures.IsAhmed ? "Flash Traffic advertising" : "Traffic Exchange advertising";
                    PurchaseOption.ChargeBalance(User, AdCost, PurchaseBalances.Traffic, note);
                    
                    Ad.Advertiser = Advertiser.AsMember(User.Name);
                    Ad.Status = AdvertStatusExtensions.GetStartingStatus();
                }

                //Add the ad to the db (for approval)
                Ad.Price = AdCost;
                Ad.Pack = Pack;
                Ad.Save();

                if (Member.IsLogged)
                {
                    //Add history entry 1
                    if (TitanFeatures.IsAhmed)
                        History.AddPurchase(User.Name, AdCost, "Flash Traffic advertising");
                    else
                        History.AddPurchase(User.Name, AdCost, "Traffic Exchange advertising");

                    //Add history entry 2
                    string entryText = "";
                    if (Pack.Ends.EndMode == End.Mode.Clicks)
                        entryText = (Convert.ToInt32(Pack.Ends.Value)).ToString() + " ad clicks";
                    else if (Pack.Ends.EndMode == End.Mode.Days)
                        entryText = (Convert.ToInt32(Pack.Ends.Value)).ToString() + " ad days";

                    History.AddPurchase(User.Name, Pack.Price, entryText);

                    Title.Text = "";
                    URL.Text = "";
                    URL.Enabled = true;
                    CheckURLButton.Visible = true;

                    SuccMessagePanel.Visible = true;
                    SuccMessage.Text = Ad.Status == AdvertStatus.WaitingForAcceptance ? U4200.ADAWAITSAPPROVAL : U3501.ADCREATED;
                }
                UsersTrafficBalanceLabel.Text = string.Format(U6008.YOURTRAFFICBALANCE, Member.CurrentInCache.TrafficBalance);
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

    protected void DirectRefsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //We want to obey OnSort and OnChart events
        string[] commands = new string[5] { "start", "stop", "add", "addsubpage", "remove" };

        if (commands.Contains(e.CommandName))
        {
            int index = e.GetSelectedRowIndex() % DirectRefsGridView.PageSize;
            GridViewRow row = DirectRefsGridView.Rows[index];
            string AdId = (row.Cells[1].Text.Trim());
            var Ad = new TrafficExchangeAdvert(Convert.ToInt32(AdId));

            if (e.CommandName == "start")
            {
                Ad.Status = AdvertStatus.Active;
                Ad.SaveStatus();
                Response.Redirect("trafficexchange.aspx");
            }
            else if (e.CommandName == "stop")
            {
                Ad.Status = AdvertStatus.Paused;
                Ad.SaveStatus();
                Response.Redirect("trafficexchange.aspx");
            }
            else if (e.CommandName == "add")
                Response.Redirect("trafficexchangecredits.aspx?id=" + AdId);
            else if (e.CommandName == "addsubpage")
                Response.Redirect("trafficexchangesubpages.aspx?id=" + AdId);
            else if (e.CommandName == "remove")
            {
                Ad.Status = AdvertStatus.Deleted;
                Ad.SaveStatus();
                Response.Redirect("trafficexchange.aspx");
            }
        }
    }

    protected void AddNewAdWithURLCheck_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(Request.Params.Get("__EVENTARGUMENT")))
        {
            string argument = Request.Params.Get("__EVENTARGUMENT");
            string validURL = Encryption.Decrypt(argument);

            URL.Text = validURL;
            URL.Enabled = false;
            CheckURLButton.Visible = false;
        }
    }
}
