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
using Prem.PTC.Utils;

public partial class Friends : System.Web.UI.Page
{
    protected Member user;
    private List<Member> members { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.PeopleFriendsEnabled);
        user = Member.CurrentInCache;

        if (!IsPostBack)
        {
            AddLang();
        }
    }

    private void AddLang()
    {
        LangAdder.Add(FriendButton, U6000.LIST);
        LangAdder.Add(RequestButton, U6000.INVITATIONS);
    }

    private void BindFriends()
    {
        members = Friendship
            .GetList(Member.CurrentId)
                .Select(x => new Member(Member.CurrentId == x.UserIdOne ? x.UserIdTwo : x.UserIdOne))
                .ToList();

        for (int i = 0; i < members.Count; i++)
        {
            UserControl friendInfoControl = (UserControl)Page.LoadControl("~/Controls/Network/FriendInfo.ascx");

            PropertyInfo member = friendInfoControl.GetType().GetProperty("Friend");
            member.SetValue(friendInfoControl, members[i], null);
            
            if (Request.QueryString["conv"] != null)
                friendInfoControl.GetType().GetProperty("RedirectToMessenger").SetValue(friendInfoControl, true, null);

            friendInfoControl.DataBind();
            FriendInfoPlaceHolder.Controls.Add(friendInfoControl);
        }
    }

    protected void RequestsGridViewDataSource_Init(object sender, EventArgs e)
    {
        RequestsGridViewDataSource.SelectCommand =
            string.Format("SELECT * FROM FriendshipRequests WHERE RecipientId = {0} AND Status = {1}",
            Member.CurrentId, (int)FriendshipRequestStatus.Pending);
    }

    protected void RequestsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var member = new Member(Convert.ToInt32(e.Row.Cells[1].Text));

            Panel userNameDivPanel = new Panel();
            LinkButton linkUserNameButton = new LinkButton();

            linkUserNameButton.PostBackUrl = HtmlCreator.GetProfileURL(member);

            linkUserNameButton.Text = member.Name;
            userNameDivPanel.Controls.Add(linkUserNameButton);

            e.Row.Cells[1].Controls.Add(userNameDivPanel);

            var userAvatar = new Image();
            userAvatar.ImageUrl = member.AvatarUrl;

            e.Row.Cells[1].Controls.Add(userAvatar);
        }
    }

    protected void RequestsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int index = e.GetSelectedRowIndex() % RequestsGridView.PageSize;
        GridViewRow row = RequestsGridView.Rows[index];
        int id = Convert.ToInt32(row.Cells[0].Text.Trim());
        var request = new FriendshipRequest(id);

        if (e.CommandName == "accept")
            request.Accept();
        if (e.CommandName == "reject")
            request.Reject();

        RequestsGridView.DataBind();
    }

    protected void RequestsGridView_DataBound(object sender, EventArgs e)
    {
        RequestsGridView.Columns[1].HeaderText = L1.USERNAME;
        RequestsGridView.Columns[2].HeaderText = L1.STATUS;
        RequestsGridView.Columns[3].HeaderText = string.Empty;
        RequestsGridView.Columns[4].HeaderText = string.Empty;

        RequestsGridView.EmptyDataText = L1.NODATA;
    }

    protected void MenuButton_Click(object sender, EventArgs e)
    {
        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        if (viewIndex == 0)
            Response.Redirect("friends.aspx");

        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";
    }

    protected void RequestsView_Activate(object sender, EventArgs e)
    {
        RequestsGridView.DataBind();
    }

    protected void FriendsView_Activate(object sender, EventArgs e)
    {
        BindFriends();
    }
}