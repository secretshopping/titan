using System;
using Prem.PTC;
using Resources;
using Prem.PTC.Members;
using Prem.PTC.Memberships;

public partial class Controls_MemberInfo : System.Web.UI.UserControl
{

    public Member DisplayMember { get; set; }
    public bool ManualDatabind { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!ManualDatabind)
            DataBind();
    }

    public override void DataBind()
    {
        if (Member.IsLogged || DisplayMember != null)
        {
            if (!IsPostBack)
            {
                MembershipsPlaceholder.Visible = AppSettings.TitanFeatures.UpgradeEnabled && !AppSettings.Points.LevelMembershipPolicyEnabled;
                MembershipsLevelPlaceholder.Visible = AppSettings.Points.LevelMembershipPolicyEnabled;
                ExpirationPlaceHolder.Visible = !ManualDatabind;
            }

            Member member = DisplayMember != null ? DisplayMember : Member.CurrentInCache;
            UserProfileLinkLiteral.Text = string.Format("<a href={0}user/network/profile.aspx?userId={1}>{2}</a>", AppSettings.Site.Url, member.Id, member.Name);
            MembershipExpiresLiteral.Text = member.FormattedMembershipExpires;
            MembershipTypeLiteral1.Text = MembershipTypeLiteral2.Text = member.FormattedMembershipName;
            MainAvatarImage.ImageUrl = member.AvatarUrl;

            if (member.Membership.Id != Membership.Standard.Id && DateTime.Now.AddDays(3) > member.MembershipExpires)
            {
                //Membership expires in less than 3 days, display warning
                int howManyDaysLeftTillExpire = ((DateTime)member.MembershipExpires).Subtract(DateTime.Now).Days;
                MembershipWarningLiteral.Text = "<img src=\"Images/Misc/warning.png\" title=\"" + Resources.U3000.EXPIREWARNING2.Replace("%n%", howManyDaysLeftTillExpire.ToString()) + "\"/>";
            }

            if (AppSettings.Points.LevelMembershipPolicyEnabled)
            {
                LevelProgressLiteral.Text = HtmlCreator.GenerateProgressHTML(member.Membership.MinPointsToHaveThisLevel,
                    member.PointsBalance, LevelManager.NextLevelValue(member.PointsBalance), AppSettings.PointsName);
            }
        }
    }

}
