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
using Prem.PTC.Payments;
using Resources;
using Titan.Matrix;

public partial class About : System.Web.UI.Page
{
    public string jsSelectAllCode;

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.AdvertisersRoleEnabled && AppSettings.TitanFeatures.AdvertFacebookEnabled
             && (!Member.IsLogged || Member.CurrentInCache.IsAdvertiser));

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

        //JS changes
        chbProfilePicture.Attributes.Add("onclick", "updatePrice();");
        chbFriends.Attributes.Add("onclick", "updatePrice(); if ($(this).is(':checked')){$('#" +
            ddlFriends.ClientID + "').css('background-color','#fff').attr('disabled', false);} " +
            "else {$('#" + ddlFriends.ClientID + "').css('background-color','#f0f0ef').attr('disabled', true);}");
        ddlOptions.Attributes.Add("onchange", "updatePrice();");

        if (!Page.IsPostBack)
        {
            BindDataToDDL();
            BindDataToDDL2();
        }
        
        if (!FacebookManager.IsIntegratedCorretly())
        {
            FacebookPlaceHolder.Visible = false;
            FacebookUnavailable.Visible = true;
            FacebookUnavailable.HeaderText = U6002.NEWFACEBOOKUNAVAILABLEHEADER;
            FacebookUnavailable.Reason = U6002.NEWFACEBOOKUNAVAILABLEREASON;
        }
        else
        {
            FacebookPlaceHolder.Visible = true;
            FacebookUnavailable.Visible = false;
        }
    }

    private void BindDataToDDL()
    {
        var listPacks = TableHelper.SelectRows<FacebookAdvertPack>(TableHelper.MakeDictionary("IsVisibleByMembers", true));

        var list = new Dictionary<string, string>();
        foreach (FacebookAdvertPack pack in listPacks)
        {
            string ends = L1.NEVER;
            if (pack.Ends.EndMode == End.Mode.Days)
                ends = pack.Ends.Value.ToString() + " " + L1.DAYS;
            else if (pack.Ends.EndMode == End.Mode.Clicks)
                ends = pack.Ends.Value.ToString() + " " + L1.LIKES;

            list.Add(pack.Id.ToString(), ends + " - " + pack.Price.ToString());
        }
        ddlOptions.DataSource = list;
        ddlOptions.DataTextField = "Value";
        ddlOptions.DataValueField = "Key";
        ddlOptions.DataBind();
    }

    private void BindDataToDDL2()
    {
        var list = new Dictionary<string, string>();
        list.Add("10", "10");
        list.Add("25", "25");
        list.Add("50", "50");
        for (Int32 i = 100; i <= 500; i = i + 100)
        {
            list.Add(i.ToString(), i.ToString());
        }
        ddlFriends.DataSource = list;
        ddlFriends.DataTextField = "Value";
        ddlFriends.DataValueField = "Key";
        ddlFriends.DataBind();
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        SuccMessagePanel.Visible = false;

        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        if (viewIndex == 0)
            Response.Redirect("facebook.aspx");

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
            FacebookAdvert Ad = new FacebookAdvert(Convert.ToInt32(e.Row.Cells[1].Text));
            
            //[5] ProfilePic
            var check = (CheckBox)e.Row.Cells[5].Controls[0];
            if (check.Checked)
                e.Row.Cells[5].Text = HtmlCreator.GetCheckboxCheckedImage();
            else
                e.Row.Cells[5].Text = HtmlCreator.GetCheckboxUncheckedImage();

            //Progress [13]
            e.Row.Cells[13].Text = HtmlCreator.GenerateAdProgressHTML(Ad).Replace("clicks", L1.LIKESSMALL);

            // % [14]
            e.Row.Cells[14].Text = Ad.ProgressInPercent + "%";

            // Likes (Clicks) [15]
            e.Row.Cells[15].Text = Ad.Clicks.ToString();

            //Status [16]
            e.Row.Cells[16].Text = HtmlCreator.GetColoredStatus(Ad.Status);

            //start [19] stop [20] remove[21]
            if (Ad.Status != AdvertStatus.Paused)
                e.Row.Cells[19].Text = "&nbsp;";

            if (Ad.Status != AdvertStatus.Active)
                e.Row.Cells[20].Text = "&nbsp;";

            if(!Ad.Status.CanBeRemoved())
                e.Row.Cells[21].Text = "&nbsp;";
        }
    }

    protected void DirectRefsGridView_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {

    }

    protected void DirectRefsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {                
        //We want to obey OnSort and OnChart events
        string[] commands = new string[3] { "start", "stop", "remove" };

        if (commands.Contains(e.CommandName))
        {
            int index = e.GetSelectedRowIndex() % DirectRefsGridView.PageSize;
            GridViewRow row = DirectRefsGridView.Rows[index];
            var Ad = new FacebookAdvert(Convert.ToInt32(row.Cells[1].Text.Trim()));

            if (e.CommandName == "start")
            {
                Ad.Status = AdvertStatus.Active;
                Ad.SaveStatus();
                DirectRefsGridView.DataBind();
            }
            else if (e.CommandName == "stop")
            {
                Ad.Status = AdvertStatus.Paused;
                Ad.SaveStatus();
                DirectRefsGridView.DataBind();
            }
            else if (e.CommandName == "remove")
            {   
                Ad.Status = AdvertStatus.Deleted;
                Ad.SaveStatus();
                DirectRefsGridView.DataBind();
            }
        }
    }

    private void ShowSuccPanel()
    {
        SuccPanel2.Visible = true;
        SuccLiteral.Text = U3501.ADCREATED;
    }


    protected void CreateAdButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        SuccMessagePanel.Visible = false;

        if (Page.IsValid)
        {
            try
            {
                string InURL = URL.Text;

                //Check fanpage in database
                if (FacebookAdvert.IsFanpageInDatabase(InURL))
                    throw new MsgException(U6003.FANPAGEEXISTS);

                //Validate Facebook URL
                if (!FacebookManager.IsFanpageURLValid(InURL))
                    throw new MsgException(L1.ER_BADFBURL);

                AppSettings.DemoCheck();

                FacebookAdvertPack Pack = new FacebookAdvertPack(Int32.Parse(ddlOptions.SelectedValue));
                FacebookAdvert Ad = new FacebookAdvert();
                Ad.TargetUrl = InURL;

                Money TotalCost = Pack.Price;
                if (chbFriends.Checked)
                {
                    Ad.MinFriends = Convert.ToInt32(ddlFriends.SelectedValue);
                    TotalCost += AppSettings.Facebook.FriendsRestrictionsCost;
                }
                else
                    Ad.MinFriends = 0;

                if (chbProfilePicture.Checked)
                {
                    Ad.HasProfilePicRestrictions = true;
                    TotalCost += AppSettings.Facebook.ProfilePicRestrictionsCost;
                }
                else
                    Ad.HasProfilePicRestrictions = false;

                Member User = null;
                if (Member.IsLogged)
                {
                    User = Member.Current;

                    var targetBalance = TargetBalanceRadioButtonList.TargetBalance;
                    PurchaseOption.ChargeBalance(User, TotalCost, TargetBalanceRadioButtonList.Feature, targetBalance, "Facebook Ad credits");
                    Ad.TargetBalance = targetBalance;

                    Ad.Advertiser = Advertiser.AsMember(User.Name);
                    Ad.Status = AdvertStatusExtensions.GetStartingStatus();
                }
                else
                {
                    Ad.Advertiser = Advertiser.AsStranger(OutEmail.Text);
                    Ad.Status = AdvertStatus.Null;
                }

                Ad.Price = TotalCost;
                Ad.Pack = Pack;
                Ad.Save();

                if (Member.IsLogged)
                {
                    //Add history entry 1
                    History.AddPurchase(User.Name, Ad.Price, "Facebook campaign");
                    //Add history entry 2
                    History.AddPurchase(User.Name, Pack.Price, Pack.Ends.Value + " Facebook likes");

                    //Achievements trial
                    int UserCurrentCampaigns = TableHelper.CountOf<FacebookAdvert>(TableHelper.MakeDictionary("CreatorUsername", User.Name));
                    bool ShouldBeSaved = User.TryToAddAchievements(
                        Prem.PTC.Achievements.Achievement.GetProperAchievements(
                        Prem.PTC.Achievements.AchievementType.AfterAdvertisingFacebookCampaigns, UserCurrentCampaigns));

                    if (ShouldBeSaved)
                        User.Save();

                    //Show success panel
                    SuccMessagePanel.Visible = true;
                    SuccMessage.Text = Ad.Status == AdvertStatus.WaitingForAcceptance ? U4200.ADAWAITSAPPROVAL : U3501.ADCREATED;

                    MatrixBase.TryAddMemberAndCredit(User, Ad.Price, AdvertType.Facebook);
                }
                else
                {
                    //Show buttons
                    PaymentButtons.Visible = true;
                    CreateAdButton.Visible = false;

                    PaymentButtons.Text = GenerateHTMLButtons.GetPaymentButtons(new BuyAdvertButtonGenerator<IFacebookAdvertPack>(Ad));
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

}
