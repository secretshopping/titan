using System;
using System.Web;
using System.Web.UI.WebControls;
using Prem.PTC.Members;
using Prem.PTC;
using Prem.PTC.Advertising;
using Resources;
using Prem.PTC.Security;
using Titan;
using System.Web.Services;
using Prem.PTC.Utils;

public partial class About : System.Web.UI.Page
{
    public string BodyCode = "";
    public string FacebookAppId = AppSettings.Facebook.ApplicationId;
    Member User;
    FacebookMember FbUser;

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.EarnersRoleEnabled && AppSettings.TitanFeatures.EarnLikesEnabled && Member.CurrentInCache.IsEarner);
        DisplayMessage();

        LabelInfo.Text = L1.CONNECTWITHFACEBOOK;
        User = Member.CurrentInCache;

        //Get codes
        BodyCode = FacebookMember.GetJSStartingCode();

        if (FacebookMember.Logged() != null)
        {
            LabelInfo.Text = L1.YOUCONNECTEDWITHFACEBOOK;
            FbUser = new FacebookMember(FacebookMember.Logged());

            //Facebook security (banning multiple accounts instantly)
            User.FacebookName = FbUser.Name;
            User.Save();

            AntiCheatSystem.AfterFacebookLogin(User);
            FacebookLikesGridView.Visible = MyFriendsPlaceHolder.Visible = true;
            FacebookLoginPanel.Visible = false;
        }
        else
            FacebookLikesGridView.Visible = MyFriendsPlaceHolder.Visible = false;

        FacebookLikesGridView.DataBind();

        if (AppSettings.Facebook.CustomFacebookLikesEnabled)
        {
            FacebookLikesGridView.Columns[4].HeaderText = U6012.REWARDFORLIKES;
            CustomLikesInfoLabel.Text = U6012.CUSTOMLIKESINFO;
        }
    }

    void DisplayMessage()
    {
        SuccMessagePanel.Visible = false;
        ErrorMessagePanel.Visible = false;

        string message = Request["__EVENTARGUMENT"] != null ? Request["__EVENTARGUMENT"] : string.Empty;

        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        if (message.Contains(U3501.YOUHAVEBEENCREDITED))
        {
            SuccMessagePanel.Visible = true;
            SuccMessage.Text = message;
        }
        else
        {
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = message;
        }
    }

    protected void FacebookLikesGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            FacebookAdvert Ad = new FacebookAdvert(Convert.ToInt32(e.Row.Cells[0].Text));

            //[2] Profile picture
            var check = (CheckBox)e.Row.Cells[2].Controls[0];
            if (check.Checked)
                e.Row.Cells[2].Text = HtmlCreator.GetCheckboxCheckedImage();
            else
                e.Row.Cells[2].Text = HtmlCreator.GetCheckboxUncheckedImage();

            //[3] Facebook like button
            e.Row.Cells[3].Text = FacebookMember.GetLikeButtonCode(Ad);

            if (FbUser != null && !User.AdsLiked.Contains(Ad.Id))
            {
                e.Row.Cells[1].Text += String.Format(" ({0} {1})", L1.YOUHAVE, FbUser.Friends);

                if (FbUser.Friends >= Ad.MinFriends)
                    e.Row.Cells[1].ForeColor = System.Drawing.Color.Green;
                else
                    e.Row.Cells[1].ForeColor = System.Drawing.Color.DarkRed;

                if (Ad.HasProfilePicRestrictions && !FbUser.HasProfilePicture)
                {
                    e.Row.Cells[2].Text = L1.NEEDED.ToUpper();
                    e.Row.Cells[2].ForeColor = System.Drawing.Color.DarkRed;
                }
                else
                {
                    e.Row.Cells[2].Text = "OK";
                    e.Row.Cells[2].ForeColor = System.Drawing.Color.Green;
                }
            }

            if (AppSettings.Facebook.CustomFacebookLikesEnabled)
            {
                //[4] Titan like button
                LinkButton btn = e.Row.FindControl("LikeButton") as LinkButton;
                if (FbUser != null && !User.AdsLiked.Contains(Ad.Id) && FbUser.Friends >= Ad.MinFriends && (!Ad.HasProfilePicRestrictions || Ad.HasProfilePicRestrictions && FbUser.HasProfilePicture))
                {
                    btn.ToolTip = "Like";
                    btn.CommandName = "like";
                    btn.Text = "<span class=\"fa fa-plus fa-lg text-success\"></span>";
                }
                else
                {
                    btn.ToolTip = "Forbidden";
                    btn.Text = "<span class=\"fa fa-times fa-lg text-danger\"></span>";
                    btn.Enabled = false;
                }
            }
        }
    }


    protected void FacebookLikesGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "like")
        {
            var index = e.GetSelectedRowIndex() % FacebookLikesGridView.PageSize;
            var row = FacebookLikesGridView.Rows[index];
            var AdId = (row.Cells[0].Text);            
            LikeUnlike(AdId, true);

            FacebookLikesGridView_DataSource_Init(null, null);
        }
    }

    private static bool HasUserMeetRequirements(FacebookAdvert Ad, FacebookMember FbMember)
    {
        bool IsOKWithRequirements = true;

        //Profile pic
        if (Ad.HasProfilePicRestrictions && !FbMember.HasProfilePicture)
            IsOKWithRequirements = false;

        //Friends
        if (Ad.MinFriends > 0 && FbMember.Friends < Ad.MinFriends)
            IsOKWithRequirements = false;

        return IsOKWithRequirements;
    }

    [WebMethod]
    public static string LikeUnlike(string adId, bool credit)
    {
        string succMsg = string.Empty;

        if (FacebookMember.Logged() != null)
        {
            Member User = Member.Current;
            FacebookMember FbUser;
            FacebookAdvert Ad = new FacebookAdvert(Convert.ToInt32(adId));

            FbUser = new FacebookMember(FacebookMember.Logged());

            //Facebook security (banning multiple accounts instantly)
            User.FacebookName = FbUser.Name;
            User.Save();

            AntiCheatSystem.AfterFacebookLogin(User);

            if (FbUser == null)
            {
                return U5008.MUSTCONNECTFB;
            }

            if (!HasUserMeetRequirements(Ad, FbUser))
                return L1.NOTMEETREQUIREMENTS;

            var userLikes = User.FbLikesToday;
            var maxLikes = User.Membership.MaxFacebookLikesPerDay;
            if (userLikes >= maxLikes)
                return string.Format(U6004.REACHEDMAXLIKESPERDAY, maxLikes);

            HttpContext.Current.Session["fbcookie"] = "notok";

            if (credit)
            {
                Ad.Click();
                succMsg = U3501.YOUHAVEBEENCREDITED + " (" + AppSettings.Facebook.PointsPerLike + " " + AppSettings.PointsName + ")";
            }
            else
            {
                Ad.Unclick();
                succMsg = string.Format(U5008.POINTSREVERSED, AppSettings.Facebook.PointsPerLike + " " + AppSettings.PointsName);
            }

            if (Ad.ShouldBeFinished)
            {
                Ad.Status = AdvertStatus.Finished;
            }

            Ad.SaveClicks();
            Ad.Save();

            //Use Crediter
            FacebookCrediter Crediter = (FacebookCrediter)CrediterFactory.Acquire(User, CreditType.FacebookLike);
            Crediter.CreditMember(Ad, credit);

            NotificationManager.RefreshWithMember(NotificationType.NewFacebookAds, User);
            return succMsg;
        }
        else
        {
            return U5008.MUSTCONNECTFB;
        }
    }

    protected void FacebookLikesGridView_DataSource_Init(object sender, EventArgs e)
    {
        User = Member.CurrentInCache;
        var userLikes = User.FbLikesToday;
        var maxLikes = User.Membership.MaxFacebookLikesPerDay;
        if (userLikes >= maxLikes)
        {
            FacebookLikesGridView.EmptyDataText = string.Format(U6004.REACHEDMAXLIKESPERDAY, maxLikes);
            return;
        }
        else       
            FacebookLikesGridView.EmptyDataText = L1.NORECORDS;

        var likedPages = string.Join(",", User.AdsLiked);

        FacebookLikesGridView_DataSource.SelectCommand =
            string.Format("SELECT * FROM FacebookAdverts WHERE Status = {0} AND FbAdvertId NOT IN ({1})", (int)AdvertStatus.Active, likedPages);
    }

}
