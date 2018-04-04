using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Members;

public partial class Controls_Network_Friends : System.Web.UI.UserControl
{
    public Member Friend { get; set; }
    public bool RedirectToMessenger { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        LoadFriends();
    }

    private void LoadFriends()
    {
        FriendTitle.InnerText = Friend.Name;
        FriendUrlLinkButton.ImageUrl = Friend.AvatarUrl;     
        FriendUrlLinkButton.PostBackUrl = HtmlCreator.GetProfileURL(Friend);

        if (RedirectToMessenger)
            FriendUrlLinkButton.PostBackUrl = AppSettings.Site.Url + "user/network/messenger.aspx?recipientId=" + Friend.Id;
    }
}