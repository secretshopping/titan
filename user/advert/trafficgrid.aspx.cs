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
using Titan;
using Titan.Matrix;

public partial class About : System.Web.UI.Page
{
    public List<TrafficGridAdvertPack> availableOptions;

    //This page has 2 versions: for logged and not-logged member

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.AdvertisersRoleEnabled && AppSettings.TitanFeatures.AdvertTrafficGridEnabled
             && (!Member.IsLogged || Member.CurrentInCache.IsAdvertiser));

        RegularExpressionValidator2.ValidationExpression = RegexExpressions.AdWebsiteUrl;

        if (Member.IsLogged)
        {
            UserName.Text = Member.CurrentName;            

            SubLiteral.Text = L1.TRAFFICGRIDADINFO1;
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

            SubLiteral.Text = L1.TRAFFICGRIDADINFO1 + "<br/><br/>" + L1.OUTADSINFO;
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
        LangAdder.Add(PasswordRequired, L1.REQ_URL, true);
        LangAdder.Add(RegularExpressionValidator2, L1.ER_BADURL, true);
        HintAdder.Add(URL, L1.H_URL);

        //JS changes
        URL.Attributes.Add("onfocus", "if (getElementById('" + URL.ClientID + "').value == 'http://') getElementById('" + URL.ClientID + "').value = ''");
        Title.Attributes.Add("onchange", "$('#Abox1 .ABtitle').html($(this).val());");
        ddlOptions.Attributes.Add("onchange", "updatePrice();");


        //Get Packs
        if (!Page.IsPostBack)
        {
            availableOptions = TableHelper.SelectRows<TrafficGridAdvertPack>(TableHelper.MakeDictionary("IsVisibleByMembers", true));
            BindDataToDDL();
        }

    }

    private void BindDataToDDL()
    {
        var listPacks = availableOptions;

        var list = new Dictionary<string, string>();
        foreach (TrafficGridAdvertPack pack in listPacks)
        {
            string ends = L1.NEVER;
            if (pack.Ends.EndMode == End.Mode.Days)
                ends = pack.Ends.Value.ToString() + " " + L1.DAYS;
            else if (pack.Ends.EndMode == End.Mode.Clicks)
                ends = pack.Ends.Value.ToString() + " " + L1.VIEWS;

            list.Add(pack.Id.ToString(), ends + " (" + pack.DisplayTime.TotalSeconds.ToString() + "s) - " + pack.Price.ToString());
        }
        ddlOptions.DataSource = list;
        ddlOptions.DataTextField = "Value";
        ddlOptions.DataValueField = "Key";
        ddlOptions.DataBind();
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        SuccMessagePanel.Visible = false;

        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        if (viewIndex == 0)
            Response.Redirect("trafficgrid.aspx");

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
            TrafficGridAdvert Ad = new TrafficGridAdvert(Convert.ToInt32(e.Row.Cells[1].Text));

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

            //Pack [3]
            AdvertPack Pack = new TrafficGridAdvertPack(Convert.ToInt32(e.Row.Cells[3].Text));

            //Displaytime [17]
            e.Row.Cells[17].Text += "s";

            //Shorten url [4]
            if (e.Row.Cells[4].Text.Length > 18)
                e.Row.Cells[4].Text = e.Row.Cells[4].Text.Substring(0, 15) + "...";

            //TItle [5]
            e.Row.Cells[5].Text = e.Row.Cells[5].Text.Replace("&lt;", "<");
            e.Row.Cells[5].Text = e.Row.Cells[5].Text.Replace("&gt;", ">");

            //Progress [11]
            string ProgressBarStatus = "";
            if (Status == AdvertStatus.Active)
                ProgressBarStatus = "active";
            e.Row.Cells[11].Text = HtmlCreator.GenerateAdProgressHTML(Ad, ProgressBarStatus).Replace("clicks", L1.CLICKSSMALL).Replace("days", L1.DAYS);

            //Geolocation check [23]
            var check = (CheckBox)e.Row.Cells[23].Controls[0];
            if (check.Checked)
                e.Row.Cells[23].Text = HtmlCreator.GetCheckboxCheckedImage();
            else
                e.Row.Cells[23].Text = HtmlCreator.GetCheckboxUncheckedImage();

            //Add % progress [12]
            e.Row.Cells[12].Text = Ad.ProgressInPercent.ToString() + "%";

            //Add total views [13]
            e.Row.Cells[13].Text = e.Row.Cells[14].Text;

            // Start[27] Pause[28] Add [29] Remove[30]
            if (Status != AdvertStatus.Paused)
                e.Row.Cells[27].Text = "&nbsp;";

            if (Status != AdvertStatus.Active)
                e.Row.Cells[28].Text = "&nbsp;";

            e.Row.Cells[29].Text = "&nbsp;";

            if (!Status.CanBeRemoved())
                e.Row.Cells[30].Text = "&nbsp;";
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

                if (URL.Enabled)
                    throw new MsgException(U4200.CHECKURL);

                TrafficGridAdvert Ad = new TrafficGridAdvert();
                TrafficGridAdvertPack Pack = new TrafficGridAdvertPack(Int32.Parse(ddlOptions.SelectedValue));
                Money AdCost = Pack.Price;

                //Set basics
                Ad.Title = InputChecker.HtmlEncode(Title.Text, Title.MaxLength, L1.TITLE);
                Ad.TargetUrl = URL.Text;

                if (Member.IsLogged)
                {
                    //Take money and save the user
                    var targetBalance = TargetBalanceRadioButtonList.TargetBalance;
                    PurchaseOption.ChargeBalance(User, AdCost, TargetBalanceRadioButtonList.Feature, targetBalance, "TrafficGrid advertising");

                    Ad.TargetBalance = targetBalance;
                    Ad.Advertiser = Advertiser.AsMember(User.Name);
                    Ad.Status = AdvertStatusExtensions.GetStartingStatus();
                }
                else
                {
                    Ad.Advertiser = Advertiser.AsStranger(OutEmail.Text);
                    Ad.Status = AdvertStatus.Null;
                }

                //Add the ad to the db (for approval)
                Ad.Price = AdCost;
                Ad.Pack = Pack;
                Ad.Save();

                if (Member.IsLogged)
                {
                    if (Ad.Status == AdvertStatus.Active || Ad.Status == AdvertStatus.Paused)
                    {
                        var Crediter = (TrafficGridCrediter)CrediterFactory.Acquire(User, Titan.CreditType.TrafficGrid);
                        Crediter.CreditReferer(AdCost);
                    }

                    //Achievements trial
                    int UserCurrentCampaigns = TableHelper.CountOf<TrafficGridAdvert>(TableHelper.MakeDictionary("CreatorUsername", User.Name));
                    bool ShouldBeSaved = User.TryToAddAchievements(
                        Prem.PTC.Achievements.Achievement.GetProperAchievements(
                        Prem.PTC.Achievements.AchievementType.AfterAdvertisingTrafficGridCampaigns, UserCurrentCampaigns));

                    if (ShouldBeSaved)
                        User.Save();

                    //Add history entry 1
                    History.AddPurchase(User.Name, AdCost, "TrafficGrid campaign");

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

                    MatrixBase.TryAddMemberAndCredit(User, Ad.Price, AdvertType.TrafficGrid);
                }
                else
                {
                    //Show buttons
                    CreateAdButton.Visible = false;

                    PaymentButtons.Visible = true;
                    PaymentButtons.Text = GenerateHTMLButtons.GetPaymentButtons(new BuyAdvertButtonGenerator<ITrafficGridAdvertPack>(Ad));
                }
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
        string[] commands = new string[4] { "start", "stop", "add", "remove" };

        if (commands.Contains(e.CommandName))
        {
            int index = e.GetSelectedRowIndex() % DirectRefsGridView.PageSize;
            GridViewRow row = DirectRefsGridView.Rows[index];
            string AdId = (row.Cells[1].Text.Trim());
            var Ad = new TrafficGridAdvert(Convert.ToInt32(AdId));

            if (e.CommandName == "start")
            {
                Ad.Status = AdvertStatus.Active;
                Ad.SaveStatus();
                Response.Redirect("trafficgrid.aspx");
            }
            else if (e.CommandName == "stop")
            {
                Ad.Status = AdvertStatus.Paused;
                Ad.SaveStatus();
                Response.Redirect("trafficgrid.aspx");
            }
            else if (e.CommandName == "remove")
            {
                Ad.Status = AdvertStatus.Deleted;
                Ad.SaveStatus();
                Response.Redirect("trafficgrid.aspx");
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
