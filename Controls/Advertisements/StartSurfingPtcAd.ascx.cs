using System;
using Prem.PTC;
using Resources;
using Prem.PTC.Members;

public partial class Controls_StartSurfingPtcAd : System.Web.UI.UserControl, ICustomObjectControl
{
    public int ObjectID { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Member.IsLogged || AppSettings.PtcAdverts.IsExternalIFrameEnabled ||
            (AppSettings.Points.LevelMembershipPolicyEnabled && Member.IsLogged && Member.CurrentInCache.PtcAutoSurfClicksThisMonth >= Member.CurrentInCache.Membership.AutosurfViewLimitMonth)            )
            ButtonPlaceholder.Visible = false;

        if (AppSettings.Points.LevelMembershipPolicyEnabled && Member.IsLogged)
        {
            int autosurfLeft = Member.CurrentInCache.Membership.AutosurfViewLimitMonth - Member.CurrentInCache.PtcAutoSurfClicksThisMonth;
            AutosurfLeftThisMonthLiteral.Text = autosurfLeft > 0 ? string.Format(U5007.AUTOSURFLEFTDESC, autosurfLeft) : U5007.NOAUTOSURFLEFT;
        }
        else
            AutosurfLeftThisMonthPlaceHolder.Visible = false;

    }
}