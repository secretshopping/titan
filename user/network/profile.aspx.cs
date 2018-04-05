using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using SocialNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberExtentionMethods;
using Titan.Leadership;

public partial class profile : System.Web.UI.Page
{
    protected Member profileOwner;
    private FriendshipRequest friendRequest;
    private List<Member> members { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        AssignUser();

        if (!AppSettings.TitanFeatures.SocialNetworkEnabled)
            Response.Redirect("~/sites/profile.aspx?u=" + profileOwner.Name);

        if (!IsPostBack)
        {
            AddLang();
        }

        if (Request.QueryString["addfriend"] != null && Request.QueryString["addfriend"] == "1" )
        {
            Befriend();
            Response.Redirect(Request.UrlReferrer.ToString());
        }

        LoadPosts();
        ShowHideContent();
        BindFriends();
    }

    private void ShowHideContent()
    {
        EPanel.Visible = false;
        NewPostPlaceHolder.Visible = profileOwner.Id == Member.CurrentId;

        bool areFriends = Member.CurrentInCache.IsFriendsWith(profileOwner);
        MessageButton.Visible = areFriends && Member.CurrentId != profileOwner.Id;

        friendRequest = FriendshipRequest.Get(Member.CurrentId, profileOwner.Id);

        if (friendRequest != null)
        {
            RejectRequestButton.Visible = AcceptRequestButton.Visible =
            friendRequest.RecipientId == Member.CurrentId
            && friendRequest.Status == FriendshipRequestStatus.Pending;

            RequestSent.Visible = !areFriends && friendRequest.SenderId == Member.CurrentId;
            RequestSent.Text = string.Format("Friend request {0}", friendRequest.Status);
            BefriendButton.Visible = false;
        }
        else
        {
            RejectRequestButton.Visible = AcceptRequestButton.Visible = RequestSent.Visible = false;
            BefriendButton.Visible = !areFriends;
        }
    }

    private void LoadPosts()
    {
        var posts = profileOwner.GetPosts();
        PostsPlaceHolder.Controls.Clear();

        for (int i = 0; i < posts.Count; i++)
        {
            UserControl postControl = (UserControl)Page.LoadControl("~/Controls/Network/Post.ascx");

            PropertyInfo myProp = postControl.GetType().GetProperty("Post");
            myProp.SetValue(postControl, posts[i], null);

            postControl.DataBind();
            PostsPlaceHolder.Controls.Add(postControl);
        }                         
    }

    private void AddLang()
    {
        NewPostButton.Text = U6000.POST;
        MessageButton.Text = L1.CONTACT;
        RejectRequestButton.Text = U6000.REJECTFRIENDREQUEST;
        AcceptRequestButton.Text = U6000.ACCEPFRIENDREQUEST;
        BefriendButton.Text = U6000.ADDFRIEND;
    }
    private void AssignUser()
    {
        if (Request.QueryString["userId"] != null)
            profileOwner = new Member(Convert.ToInt32(Request.QueryString["userId"]));
        else
            profileOwner = Member.CurrentInCache;

        MemberInfo.DisplayMember = profileOwner;
    }

    #region New Post
    Banner tempImage;

    public Banner TempImage
    {
        get
        {
            if (tempImage == null)
            {
                if (ViewState["TempImage"] == null)
                {
                    ViewState["TempImage"] = Banner.Empty;
                }

                tempImage = ViewState["TempImage"] as Banner;
            }
            return tempImage;
        }
        set
        {

            tempImage = value;
            ViewState["TempImage"] = tempImage;
        }
    }

    protected void ImageUploadButton_Click(object sender, EventArgs e)
    {
        try
        {
            EPanel.Visible = SPanel.Visible = false;

            DeleteOldImage();
            if (!Banner.TryFromStream(ImageUpload.PostedFile.InputStream, out tempImage))
                throw new MsgException(U6000.SELECTANIMAGEFIRST);

            Post.ValidateImage(tempImage.Width, tempImage.Height);
            TempImage = TempImage.Save(AppSettings.FolderPaths.PostImages);

            ImagePreview.ImageUrl = tempImage.Path;

            ClearUpload();
        }
        catch (Exception ex)
        {
            EPanel.Visible = true;
            ELabel.Text = ex.Message;
            DeleteOldImage();
        }
    }

    private void ClearUpload()
    {
        ImageUpload.Dispose();
    }

    private void DeleteOldImage()
    {
        if (TempImage != null && TempImage.IsSaved)
            TempImage.Delete();

    }

    private void ClearAll()
    {
        TempImage = null;
        ImageUpload.Visible = true;
        ImageUploadButton.Visible = true;
        ImagePreview.ImageUrl = string.Empty;
        NewPostTextBox.Text = string.Empty;
    }

    protected void NewPostButton_Click(object sender, EventArgs e)
    {
        try
        {
            EPanel.Visible = SPanel.Visible = false;
            if (profileOwner.Id != Member.CurrentId)
                throw new MsgException(U6000.CANNOTPOSTONSOMEONEELSEWALL);

            var text = InputChecker.HtmlEncode(NewPostTextBox.Text, NewPostTextBox.MaxLength, "Post");
            var post = Post.Create(profileOwner.Id, text, TempImage);

            post.Save();
            ClearAll();
            LoadPosts();
        }
        catch (Exception ex)
        {
            if (ex is MsgException)
            {
                EPanel.Visible = true;
                ELabel.Text = ex.Message;
                DeleteOldImage();
            }
        }
    }
    #endregion

    private void BindFriends()
    {
        members = Friendship
            .GetList(profileOwner.Id)
                .Select(x => new Member(profileOwner.Id == x.UserIdOne ? x.UserIdTwo : x.UserIdOne))
                .ToList();

        for (int i = 0; i < members.Count; i++)
        {
            UserControl friendInfoControl = (UserControl)Page.LoadControl("~/Controls/Network/FriendInfo.ascx");

            PropertyInfo member = friendInfoControl.GetType().GetProperty("Friend");
            member.SetValue(friendInfoControl, members[i], null);

            friendInfoControl.DataBind();
            FriendInfoPlaceHolder.Controls.Add(friendInfoControl);
        }
    }

    protected void BefriendButton_Click(object sender, EventArgs e)
    {
        Befriend();
    }

    protected void Befriend()
    {
        try
        {
            EPanel.Visible = SPanel.Visible = false;
            Member.CurrentInCache.AddFriend(profileOwner.Id, () => { SPanel.Visible = true; SLabel.Text = U6000.FRIENDREQUESTSENT; });
            ShowHideContent();
        }
        catch (Exception ex)
        {
            if (ex is MsgException)
            {
                EPanel.Visible = true;
                ELabel.Text = ex.Message;
            }
        }
    }

    protected void MessageButton_Click(object sender, EventArgs e)
    {
        if (profileOwner.Id != Member.CurrentId)
        {
            Response.Redirect("~/user/network/messenger.aspx?recipientId=" + profileOwner.Id);
        }
    }

    protected void AcceptRequestButton_Click(object sender, EventArgs e)
    {
        if (friendRequest != null && friendRequest.Status == FriendshipRequestStatus.Pending)
            friendRequest.Accept();
        ShowHideContent();
    }

    protected void RejectRequestButton_Click(object sender, EventArgs e)
    {
        if (friendRequest != null && friendRequest.Status == FriendshipRequestStatus.Pending)
            friendRequest.Reject();
        ShowHideContent();
    }

   
}