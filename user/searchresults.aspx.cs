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

public partial class searchresults : System.Web.UI.Page
{
    private List<Member> members { get; set; }
    private string query { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!AppSettings.TitanFeatures.SocialNetworkEnabled)
            Response.Redirect("~/user/default.aspx");

        if (!IsPostBack && Request.QueryString["q"] != null)
        {
            query = HttpUtility.UrlDecode(Request.QueryString["q"]).Trim().Replace("'","");
            BindFriends();
        }
    }

    private void BindFriends()
    {
        members = new List<Member>();

        try
        {
            members = TableHelper.GetListFromRawQuery<Member>(
                String.Format("SELECT * FROM Users WHERE Username LIKE '%{0}%'", query));
        }
        catch (Exception ex) { }

        NoResultsPlaceHolder.Visible = (members.Count == 0);

        for (int i = 0; i < members.Count; i++)
        {
            UserControl friendInfoControl = (UserControl)Page.LoadControl("~/Controls/Network/FriendInfo.ascx");

            PropertyInfo member = friendInfoControl.GetType().GetProperty("Friend");
            member.SetValue(friendInfoControl, members[i], null);

            friendInfoControl.DataBind();
            FriendInfoPlaceHolder.Controls.Add(friendInfoControl);
        }
    }
}