using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberExtentionMethods;
using Prem.PTC.Members;
using Resources;
using Prem.PTC;

public partial class sites_profile : TitanPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (TitanFeatures.IsClickmyad)
            Response.Redirect("~/user/default.aspx");

        if (TitanFeatures.IsJ5WalterOffersFromHome)
        {
            smallInfoPlaceHolder.Visible = false;
            AchievementsPlaceHolder.Visible = false;
            ReferralsPlaceHolder.Visible = false;
        }

        if (Member.IsLogged)
        {
            string memberUsername = Request.QueryString["u"];

            if (!string.IsNullOrEmpty(memberUsername))
            {
                try
                {
                    Member Target = new Member(memberUsername);
                    if (AppSettings.TitanFeatures.SocialNetworkEnabled)
                        Response.Redirect("~/user/network/profile.aspx?userId=" + Target.Id);
                    MainAvatarImage.ImageUrl = Target.AvatarUrl;
                    UsernameLabel.Text = Target.Name;
                    MembershipTypeLiteral.Text = Target.FormattedMembershipName;

                    TotalEarned.Text = Target.TotalEarned.ToString();
                    AccStatus.Text = Target.Status.ToString();

                    var rrm = new Prem.PTC.Referrals.RentReferralsSystem(Target.Name, Target.Membership);

                    Referrals.Text = (Target.GetDirectReferralsCount() + rrm.GetUserRentedReferralsCount()).ToString();

                    MemberAchievementsList1.TargetUsername = Target.Name;
                    MemberAchievementsList1.Visible = true;

                    if (Member.IsLogged)
                    {
                        bool areFriends = Member.CurrentInCache.IsFriendsWith(Target);

                        if (TitanFeatures.IsJ5WalterOffersFromHome)
                            BefriendButton.Visible = false;
                        else
                            BefriendButton.Visible = !areFriends;

                        MessageButton.Visible = areFriends && Member.CurrentId != Target.Id;
                    }
                    else
                    {
                        BefriendButton.Visible = MessageButton.Visible = false;
                    }
                }
                catch (Exception ex)
                {
                    UsernameLabel.Text = "User not found";
                }
            }
            else
                UsernameLabel.Text = "User not found";
        }
        else
            UsernameLabel.Text = Resources.U3501.YOUMUSTBEL;

        LangAdder.Add(MessageButton, L1.CONTACT);
    }

    protected void BefriendButton_Click(object sender, EventArgs e)
    {
        try
        {
            EPanelProfile.Visible = SPanelProfile.Visible = false;
            Member profileOwner = Member.CurrentInCache;
            if (Request.QueryString["u"] != null)
                profileOwner = new Member(Request.QueryString["u"]);
            Member.CurrentInCache.AddFriend(profileOwner.Id, () =>
            {
                SPanelProfile.Visible = true; SLiteralProfile.Text = "Friend request sent";
            });
        }
        catch (Exception ex)
        {
            if (ex is MsgException)
            {
                EPanelProfile.Visible = true;
                ELiteralProfile.Text = ex.Message;
            }
        }
    }

    protected void MessageButton_Click(object sender, EventArgs e)
    {
        if (Request.QueryString["u"] != null)
        {
            Member profileOwner = new Member(Request.QueryString["u"]);
            Response.Redirect("~/user/network/messenger.aspx?recipientId=" + profileOwner.Id);
        }

    }
}