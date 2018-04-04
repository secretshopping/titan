using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using System.Data;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using Titan.Shares;
using Prem.PTC.Advertising;
using System.Text;

public partial class About : System.Web.UI.Page
{
    new Member User;
    int _userCustomGroupId;
    public int userCustomGroupId { get { return _userCustomGroupId; } }
    public UserCustomGroup userCustomGroup;
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.AdvertAdPacksEnabled &&
            (AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.CustomGroups || AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.AutomaticAndCustomGroups));

        if (Request.Params.Get("g") == null)
            Response.Redirect("~/default.aspx");

        if (!Int32.TryParse(Request.Params.Get("g"), out _userCustomGroupId))
            Response.Redirect("~/default.aspx");

        try
        {
            userCustomGroup = new UserCustomGroup(userCustomGroupId);
        }
        catch (IndexOutOfRangeException ex)
        {
            Response.Redirect("~/default.aspx");
        }

        if (Member.IsLogged)
            User = Member.Current;
        else
            Button2.Visible = false;

        if (Member.IsLogged)
            UpdateAvailableAdPacks();

        DataBind();

    }

    public override void DataBind()
    {
        base.DataBind();

        CustomGroup customGroup = new CustomGroup(userCustomGroup.CustomGroupId);
        JoinGroupButton.Text = U4200.JOINGROUP;
        LeaveGroupButton.Text = U4200.LEAVEGROUP;
        Button1.Text = userCustomGroup.Name;
        Button2.Text = U4200.JOINLEAVE;

        string groupURL = AppSettings.Site.Url + "user/advert/groups/customgroup.aspx?g=" + userCustomGroup.Id;
        GroupURL.Text = String.Format("<a href='{0}'>{0}</a>", groupURL);

        GroupNameLiteral.Text = userCustomGroup.Name;

        PacksLeftLiteral.Text = PacksLeftLiteral2.Text = HtmlCreator.GenerateCPAAdProgressHTML(userCustomGroup.AdPacksAdded, customGroup.AdPacksLimit,
                AppSettings.RevShare.AdPack.AdPackNamePlural);

        if (userCustomGroup.AdPacksAdded < customGroup.AdPacksLimit)
        {
            OpenGroupPlaceholder.Visible = true;
            ClosedGroupPlaceHolder.Visible = false;
        }
        else
        {
            OpenGroupPlaceholder.Visible = false;
            ClosedGroupPlaceHolder.Visible = true;
        }



        GroupDescriptionLiteral.Text = userCustomGroup.Description;

        VideoImagePlaceholder.Style.Add("max-height", GroupStyles.maxPictureHeight.ToString() + "px");
        VideoImagePlaceholder.Style.Add("max-width", GroupStyles.maxPictureWidth.ToString() + "px");

        if (!string.IsNullOrWhiteSpace(userCustomGroup.PromoUrl))
        {
            VideoPanel.Visible = true;
            VideoLiteral.Text = userCustomGroup.PromoUrl;


            ImagePanel.Visible = false;
        }
        if (!string.IsNullOrWhiteSpace(userCustomGroup.ImagePath))
        {
            ImagePanel.Visible = true;
            ImageLiteral.Text = "<img src='" + Page.ResolveUrl(userCustomGroup.ImagePath) + "'></img>";

            VideoPanel.Visible = false;
        }

        if (!string.IsNullOrWhiteSpace(userCustomGroup.Email))
        {
            EmailPlaceHolder.Visible = true;
            EmailLiteral.Text = userCustomGroup.Email;
        }

        if (!string.IsNullOrWhiteSpace(userCustomGroup.Skype))
        {
            SkypePlaceHolder.Visible = true;
            SkypeLiteral.Text = userCustomGroup.Skype;
        }

        if (!string.IsNullOrWhiteSpace(userCustomGroup.PhoneNumber))
        {
            PhonePlaceHolder.Visible = true;
            PhoneLiteral.Text = userCustomGroup.PhoneNumber;
        }

        if (!string.IsNullOrWhiteSpace(userCustomGroup.FacebookUrl))
        {
            FacebookPlaceHolder.Visible = true;
            FacebookLiteral.Text = string.Format("<a href='{0}'>{1}</a>", userCustomGroup.FacebookUrl, userCustomGroup.Name);
        }

        if (AppSettings.RevShare.AdPack.RevShareCustomGroupRewardsEnabled && 
            (userCustomGroup.Status == CustomGroupStatus.Active || userCustomGroup.Status == CustomGroupStatus.Expired))
        {
            //Show rewards info
            RewardInfoPlaceHolder.Visible = true;
            RewardInfoLiteral.Text = userCustomGroup.BonusExtraInformation;
        }

        NameLiteral.Text = MemberManager.getUsersProfileURL(new Member(userCustomGroup.CreatorUserId).Name);
        ParticipantListLiteral.Text = GetParticipants();
    }
    private string GetParticipants()
    {
        StringBuilder sb = new StringBuilder();

        var participants = CustomGroupManager.GetParticipantNamesAndAdPackCount(userCustomGroup);
        foreach (var participant in participants)
        {
            sb.Append(MemberManager.getUsersProfileURL(participant.Key) + "(" + participant.Value + "), ");
        }

        string final = sb.ToString();

        if (!string.IsNullOrWhiteSpace(final))
            final = final.Remove(final.Length - 2);

        return final;
    }
    public void MenuButton_Click(object sender, EventArgs e)
    {
        SPanel.Visible = false;
        EPanel.Visible = false;

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

    private void UpdateAvailableAdPacks()
    {
        var customGroup = new CustomGroup(userCustomGroup.CustomGroupId);
        AvailableAdPacksLabel.Text = AdPackManager.GetUsersActiveAdPacksForGroups(User.Id, customGroup).Count.ToString();
        if (User.Id == userCustomGroup.CreatorUserId)
            MyAdPacks.Text = (AdPackManager.GetUsersActiveAdPacksForGroups(User.Id, customGroup, userCustomGroupId).Count - customGroup.CreatorsMinNumberOfAdPacks).ToString();
        else
            MyAdPacks.Text = AdPackManager.GetUsersActiveAdPacksForGroups(User.Id, customGroup, userCustomGroupId).Count.ToString();
    }


    protected void JoinLeaveGroupButton_Click(object sender, EventArgs e)
    {
        SPanel.Visible = false;
        EPanel.Visible = false;

        try
        {
            int numberOfAdPacks = Convert.ToInt32(AdPacksTextBox.Text);

            if (numberOfAdPacks <= 0)
                throw new MsgException(U4200.CANTBUYZEROADPACKS.Replace("%n%", AppSettings.RevShare.AdPack.AdPackName));

            var TheButton = (Button)sender;

            if (TheButton.CommandArgument == "true")
                CustomGroupManager.AddRemoveUsersAdPacksToCustomGroup(User.Id, numberOfAdPacks, userCustomGroup, true);
            else if (TheButton.CommandArgument == "false")
                CustomGroupManager.AddRemoveUsersAdPacksToCustomGroup(User.Id, numberOfAdPacks, userCustomGroup, false);

            SPanel.Visible = true;
            if (TheButton.CommandArgument == "true")
                SText.Text = U4200.JOINGROUPSUCCESS.Replace("%n%", userCustomGroup.Name).Replace("%p%", numberOfAdPacks + " " + AppSettings.RevShare.AdPack.AdPackNamePlural);
            else if (TheButton.CommandArgument == "false")
                SText.Text = U4200.LEAVEGROUPSUCCESS.Replace("%n%", numberOfAdPacks.ToString()).Replace("%p%", AppSettings.RevShare.AdPack.AdPackNamePlural);

            CustomGroup customGroup = new CustomGroup(userCustomGroup.CustomGroupId);
            if (userCustomGroup.AdPacksAdded >= customGroup.AdPacksLimit)
            {
                if (AppSettings.RevShare.AdPack.CustomReturnOption == CustomReturnOption.Increase)
                    CustomGroupManager.IncreaseAdPacksReturnAmountInClosedGroup(customGroup, userCustomGroup);
                userCustomGroup.Status = CustomGroupStatus.Active;

                if (AppSettings.RevShare.AdPack.RevShareCustomGroupRewardsEnabled)
                    GroupRewardManager.Apply(customGroup, userCustomGroup);
            }
            else
                userCustomGroup.Status = CustomGroupStatus.InProgress;

            userCustomGroup.Save();

            UpdateAvailableAdPacks();
            DataBind();
        }
        catch (MsgException ex)
        {
            EPanel.Visible = true;
            EText.Text = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }

    }
}
