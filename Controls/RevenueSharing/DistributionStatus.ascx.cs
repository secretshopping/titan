using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Titan;
using Prem.PTC;
using Resources;
using Prem.PTC.Members;
using System.Text;
using Prem.PTC.Advertising;
using Prem.PTC.Texts;

public partial class Controls_DistributionStatus : System.Web.UI.UserControl
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Member.IsLogged && AppSettings.RevShare.IsRevShareEnabled)
        {
            Member member = Member.CurrentInCache;
            int dailyRequiredClicks = member.Membership.AdPackDailyRequiredClicks;
            if (dailyRequiredClicks > 0)
            {
                ContainerPlaceHolder.Visible = true;

                if (AppSettings.RevShare.DistributionTime != DistributionTimePolicy.EveryWeek)
                {
                    //Vacation Mode
                    if (member.Status == MemberStatus.VacationMode)
                        StatusLiteral.Text = U5003.YOUWILL2.Replace("%p%", AppSettings.RevShare.AdPack.AdPackName);

                    //Registered today
                    else if (member.Registered.Date == DateTime.Now.Date)
                        StatusLiteral.Text = U5003.YOUWILL3.Replace("%p%", AppSettings.RevShare.AdPack.AdPackName)
                            .Replace("%l%", String.Format("<b>{0}</b>", dailyRequiredClicks));

                    else if (member.RevenueShareAdsWatchedYesterday >= dailyRequiredClicks)
                        StatusLiteral.Text = U5003.YOUWILL1.Replace("%p%", AppSettings.RevShare.AdPack.AdPackName)
                            .Replace("%l%", String.Format("{0}", dailyRequiredClicks))
                            .Replace("%n%", String.Format("<b>{0}</b>", member.RevenueShareAdsWatchedYesterday));

                    else
                        StatusLiteral.Text = U5003.YOUWILLNOT1.Replace("%p%", AppSettings.RevShare.AdPack.AdPackName)
                            .Replace("%l%", String.Format("{0}", dailyRequiredClicks))
                            .Replace("%n%", String.Format("<b>{0}</b>", member.RevenueShareAdsWatchedYesterday));
                }
                else
                {
                    var adsViewed = member.RSAPTCAdsViewed.Count;
                    string coloredAdsViewed;

                    if (adsViewed < member.Membership.AdPackDailyRequiredClicks)
                        coloredAdsViewed = string.Format("<span style={0}>{1}</span>", "'color: #cb6f1b; font-weight: bold;'", adsViewed);
                    else
                        coloredAdsViewed = string.Format("<span style={0}>{1}</span>", "'color: #87a131; font-weight: bold;'", adsViewed);
                    string timeLeft = Prem.PTC.Utils.TimeSpanExtensions.ToFriendlyDisplay(AdPackManager.GetAdSurfDeadline(), 3);

                    StatusLiteral.Text = string.Format(U6003.ADPACKTIMEDESC, dailyRequiredClicks, coloredAdsViewed, dailyRequiredClicks, timeLeft);
                }

            }
        }
    }

}
