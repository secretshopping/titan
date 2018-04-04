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

public partial class CreateCustomGroup : System.Web.UI.Page
{
    new Member User;

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.AdvertAdPacksEnabled &&
            (AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.CustomGroups || AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.AutomaticAndCustomGroups)
            && Member.CurrentInCache.IsEarner);


        User = Member.Current;

        if (!Page.IsPostBack)
        {
            BindToPhotoVideoRadio();
            LangAdder.Add(PictureUploadValidCustomValidator, U4200.INVALIDPICTURE, true);
            LangAdder.Add(PictureUploadSelectedCustomValidator, U4200.CONSTANTBANNERNOTUPLOADED.Replace("%n%", string.Format("{0}/{1}", AppSettings.RevShare.AdPack.PackConstantBannerWidth, AppSettings.RevShare.AdPack.PackConstantBannerHeight)), true);
            CreateGroupButton.Text = U4200.OPENGROUP;
            BindToGroupsAvailableToOpenRadio();
            UpdateAvailableAdPacks();
        }

        if (PhotoVideo.SelectedIndex == 0)
        {
            VideoUploadPlaceHolder.Visible = true;
            PictureUploadPlaceHolder.Visible = false;
        }
        else if (PhotoVideo.SelectedIndex == 1)
        {
            VideoUploadPlaceHolder.Visible = false;
            PictureUploadPlaceHolder.Visible = true;
        }


        RegularExpressionValidator2.ValidationExpression = RegexExpressions.AdWebsiteUrl;
        LangAdder.Add(RegularExpressionValidator2, L1.ER_BADURL, true);

        LangAdder.Add(RequiredFieldValidator2, AppSettings.RevShare.AdPack.AdPackNamePlural + " " + U3900.FIELDISREQUIRED, true);
        LangAdder.Add(RequiredFieldValidator1, L1.NAME + " " + U3900.FIELDISREQUIRED, true);
        LangAdder.Add(RequiredFieldValidator3, L1.REQ_DESC, true);

        CancelButton.Attributes.Add("href", ResolveUrl("~/user/advert/groups/list.aspx"));
    }

    private void UpdateAvailableAdPacks()
    {
        int customGroupId = -1;
        Int32.TryParse(GroupsAvailableToOpen.SelectedValue, out customGroupId);

        if (customGroupId != -1)
        {
            AvailableAdPacksLabel.Text = AdPackManager.GetUsersActiveAdPacksForGroups(User.Id, new CustomGroup(customGroupId)).Count.ToString();
            AllAdPacksLabel.Text = AdPackManager.GetUsersActiveAdPacksWithoutGroup(User.Id).Count().ToString();
        }
        else
            AdPacksPlaceHolder.Visible = false;

    }

    private void BindToGroupsAvailableToOpenRadio()
    {
        GroupsAvailableToOpen.Items.Clear();

        var groups = CustomGroupManager.GetAvailableGroups(User.Id);
        var usersHighestClosedGroup = CustomGroupManager.GetUsersHighestClosedGroup(User);

        for (int i = 0; i < groups.Count; i++)
        {
            var itemKey = groups[i].Id.ToString();
            string profitString = "";

            if (AppSettings.RevShare.AdPack.CustomReturnOption == CustomReturnOption.Accelerate)
                profitString = U4200.DAILYPROFITRAISEDBY;

            else if (AppSettings.RevShare.AdPack.CustomReturnOption == CustomReturnOption.Increase)
                profitString = U5001.PROFITRAISEDBY;

            var itemString = " " + profitString + ": " + "<b>" 
                + ((AppSettings.RevShare.AdPack.CustomReturnOption == CustomReturnOption.Increase ? 
                ("+" + (groups[i].AcceleratedProfitPercentage).ToString()) : groups[i].AcceleratedProfitPercentage.ToString()))
                + "%</b>, "
                + U5001.PACKSREQUIREDTOOPENCLOSE.Replace("%n%", AppSettings.RevShare.AdPack.AdPackNamePlural) + ": <b>"
                + groups[i].CreatorsMinNumberOfAdPacks.ToString() + "/" + groups[i].AdPacksLimit.ToString() + "</b> "
                + ", "
                + U5001.OPENINSTANCES + ": <b>" + CustomGroupManager.GetNumberOfUsersCustomGroupInstances(User.Id, groups[i].Id) + "/" + groups[i].NumberOfGroupsLimit + "</b>";
            var isEnabled = true;

            if (CustomGroupManager.GetNumberOfUsersCustomGroupInstances(User.Id, groups[i].Id) >= groups[i].NumberOfGroupsLimit)
                isEnabled = false;

            if (groups[i].Number > usersHighestClosedGroup + 1)
                isEnabled = false;

            ListItem item = new ListItem(itemString, itemKey, isEnabled);

            GroupsAvailableToOpen.Items.Add(item);
        }
        if (GroupsAvailableToOpen.Items.Count <= 0)
        {
            ListItem item = new ListItem(U4200.YOUMUSTCLOSEGROUPS, "-1", false);
            GroupsAvailableToOpen.Items.Add(item);
        }
        GroupsAvailableToOpen.SelectedIndex = 0;
    }

    private void BindToPhotoVideoRadio()
    {
        PhotoVideo.Items.Clear();

        var list = new Dictionary<string, string>();
        var videoString = " " + U4200.VIDEOURL;
        var photoString = string.Format(" {0} max({1}/{2})", U4200.PICTURE, GroupStyles.maxPictureWidth, GroupStyles.maxPictureHeight);

        var videoItem = new ListItem(videoString);
        var photoItem = new ListItem(photoString);

        PhotoVideo.Items.Add(videoItem);
        PhotoVideo.Items.Add(photoItem);

        PhotoVideo.SelectedIndex = 0;
    }
    protected void CreateGroupButton_Click(object sender, EventArgs e)
    {
        SPanel.Visible = false;
        EPanel.Visible = false;
        try
        {
            if (!AdPacksPlaceHolder.Visible)
                throw new MsgException(U4200.YOUMUSTCLOSEGROUPS);
            int userGroupId = Convert.ToInt32(GroupsAvailableToOpen.SelectedValue);
            int numberOfAdPacks = 0;
            try
            {
                numberOfAdPacks = Convert.ToInt32(AdPacksTextBox.Text);
            }
            catch (Exception ex)
            {
                throw new MsgException(U4000.BADFORMAT);
            }
            if (numberOfAdPacks <= 0)
                throw new MsgException(U4200.CANTBUYZEROADPACKS.Replace("%n%", AppSettings.RevShare.AdPack.AdPackName));

            var customGroup = new CustomGroup(userGroupId);
            var usersHighestClosedNumber = CustomGroupManager.GetUsersHighestClosedGroup(User);

            var allAdPacksList = AdPackManager.GetUsersActiveAdPacksForGroups(User.Id, customGroup);
            IEnumerable<AdPack> adPacksList = (from a in allAdPacksList select a).Take(numberOfAdPacks);

            if (adPacksList.Count() < numberOfAdPacks)
                throw new MsgException(U4200.NOTENOUGHADPACKSAVAILABLEFORGROUPS.Replace("%n%", AppSettings.RevShare.AdPack.AdPackNamePlural));

            //HAS USER CLOSED A GROUP WITH A LOWER NUMBER?
            if (customGroup.Number > usersHighestClosedNumber + 1)
                throw new MsgException(U4200.CANTCREATEGROUPOFTHISTYPE);

            //MIN NUMBER OF PACKS TO CREATE
            if (customGroup.CreatorsMinNumberOfAdPacks > numberOfAdPacks)
                throw new MsgException(U4200.MOREADPACKSREQUIRED.Replace("%n%", customGroup.CreatorsMinNumberOfAdPacks.ToString()).Replace("%p%", AppSettings.RevShare.AdPack.AdPackNamePlural));

            //NUMBER OF PACKS TO CLOSE
            if (customGroup.AdPacksLimit < numberOfAdPacks)
                throw new MsgException(U4200.TOOMANYPACKS.Replace("%n%", AppSettings.RevShare.AdPack.AdPackNamePlural) + " " + customGroup.AdPacksLimit);

            //MAX OPEN INSTANCES
            if (CustomGroupManager.GetNumberOfUsersCustomGroupInstances(User.Id, customGroup.Id) >= customGroup.NumberOfGroupsLimit)
                throw new MsgException("You cannot exceed your group open instances limit");

            //MAX ADMIN ADPACKS
            if (customGroup.CreatorsMaxNumberOfAdPacks < numberOfAdPacks)
                throw new MsgException(U4200.TOOMANYPACKS.Replace("%n%", AppSettings.RevShare.AdPack.AdPackNamePlural) + " " + customGroup.CreatorsMaxNumberOfAdPacks);

            string name = GroupNameTextBox.Text;
            string description = GroupDescriptionTextBox.Text;
            string videoURL = PromoUrlTextBox.Text;
            string email = EmailTextBox.Text;
            string skype = SkypeTextBox.Text;
            string phoneNumber = PhoneNumberTextBox.Text;
            string facebookURL = FacebookUrlTextBox.Text;

            if (!string.IsNullOrWhiteSpace(facebookURL) && !facebookURL.Contains("facebook.com"))
                throw new MsgException(L1.ER_BADURL);

            //IFRAME VALID?
            if (!string.IsNullOrWhiteSpace(videoURL) && !UrlVerifier.IsIframeValid(videoURL))
                throw new MsgException(U4200.INVALIDIFRAME);

            CustomGroupManager.CreateUserCustomGroup(NewCustomGroup, customGroup, adPacksList, User, name, description, videoURL, email, skype, phoneNumber, facebookURL);

            SPanel.Visible = true;

            string groupUrl = string.Format("{0}user/advert/groups/customgroup.aspx?g={1}", AppSettings.Site.Url, NewCustomGroup.Id);
            SText.Text = U4200.CREATEGROUPSUCCESS.Replace("%n%", name).Replace("%p%", numberOfAdPacks + " " + AppSettings.RevShare.AdPack.AdPackNamePlural).Replace("%u%", "<br/>" + groupUrl);

            if (NewCustomGroup.AdPacksAdded >= customGroup.AdPacksLimit)
            {
                if (AppSettings.RevShare.AdPack.CustomReturnOption == CustomReturnOption.Increase)
                    CustomGroupManager.IncreaseAdPacksReturnAmountInClosedGroup(customGroup, NewCustomGroup);

                SText.Text += "<br/>" + U5001.CREATORCLOSECUSTOMGROUPSUCCESS.Replace("%a%", AppSettings.RevShare.AdPack.AdPackName)
                        .Replace("%b%", (customGroup.AcceleratedProfitPercentage).ToString());

                NewCustomGroup.Status = CustomGroupStatus.Active;
            }
            else
                NewCustomGroup.Status = CustomGroupStatus.InProgress;

            NewCustomGroup.Save();

            BindToGroupsAvailableToOpenRadio();
            UpdateAvailableAdPacks();
            ClearControls();
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

    private void ClearControls()
    {
        foreach (Control c in ControlsPlaceHolder.Controls)
        {
            if (c is TextBox)
            {
                ((TextBox)c).Text = string.Empty;
            }
        }

        NewCustomGroup.BannerImage = null;
        createBannerAdvertisement_BannerImage.ImageUrl = "";

        NewCustomGroup = null;
    }

    protected void GroupsAvailableToOpen_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateAvailableAdPacks();
    }

    #region PICTURE UPLOAD
    private UserCustomGroup _newCustomGroup;
    protected UserCustomGroup NewCustomGroup
    {
        get
        {
            if (_newCustomGroup == null)

                if (Session["UserCustomGroup"] is UserCustomGroup)
                    _newCustomGroup = Session["UserCustomGroup"] as UserCustomGroup;
                else
                {
                    NewCustomGroup = new UserCustomGroup();
                    Session["UserCustomGroup"] = _newCustomGroup = NewCustomGroup;
                }
            return _newCustomGroup;
        }


        set { Session["UserCustomGroup"] = _newCustomGroup = value; }
    }


    Banner newTemporaryPicture;
    protected void PictureUploadValidCustomValidator_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        if (PhotoVideo.SelectedIndex == 1)
        {

            bool stream = Banner.TryFromStream(PictureUpload.PostedFile.InputStream, out newTemporaryPicture);
            bool width = newTemporaryPicture.Width <= GroupStyles.maxPictureWidth;
            bool height = newTemporaryPicture.Height <= GroupStyles.maxPictureHeight;
            e.IsValid = stream && (width && height);
        }
    }

    protected void PictureUploadSelectedCustomValidator_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        if (PhotoVideo.SelectedIndex == 1)
        {
            e.IsValid = NewCustomGroup.BannerImage != null;
        }
    }

    protected void PictureUploadSubmit_Click(object sender, EventArgs e)
    {
        if (Page.IsValid && PictureUpload.HasFile)
        {
            newTemporaryPicture.Save(AppSettings.FolderPaths.BannerAdvertImages);

            NewCustomGroup.BannerImage = newTemporaryPicture;
            createBannerAdvertisement_BannerImage.ImageUrl = NewCustomGroup.BannerImage.Path;

            //Hide upload
            PictureUpload.Visible = false;
            PictureUploadSubmit.Visible = false;

            PictureUpload.Dispose();
        }
    }

    protected void PhotoVideo_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (PhotoVideo.SelectedIndex == 0)
        {
            VideoUploadPlaceHolder.Visible = true;
            PictureUploadPlaceHolder.Visible = false;
        }
        else if (PhotoVideo.SelectedIndex == 1)
        {
            VideoUploadPlaceHolder.Visible = false;
            PictureUploadPlaceHolder.Visible = true;
        }
    }
    #endregion
}
