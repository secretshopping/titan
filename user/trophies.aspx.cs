using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using Prem.PTC.Achievements;

public partial class About : System.Web.UI.Page
{
    List<Achievement> achivsList;
    List<Achievement> allAchievementsEverPossible;

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.TrophiesEnabled);

        achivsList = TableHelper.SelectRows<Achievement>(TableHelper.MakeDictionary("AchievementStatus", (int)AchievmentStatus.Visible));
        allAchievementsEverPossible = TableHelper.SelectAllRows<Achievement>();

        Member User = Member.Logged(Context);

        //Write data
        AchivCount.Text = (User.Achievements.Count - 1).ToString();
        AchivTotalCount.Text = achivsList.Count.ToString();

        //New achievements
        if (User.UnspottedAchievements > 0)
        {
            WarningPanel.Visible = true;
            WarningLiteral.Text = L1.NEWACHIEVEMENTS;
            User.UnspottedAchievements = 0;
            User.Save();

            //Refresh notification
            NotificationManager.RefreshWithMember(NotificationType.NewAchievements, User);
        } 

        //Display user achievements
        foreach (Achievement achiv in allAchievementsEverPossible)
        {
            if (achiv.AchievmentStatus == AchievmentStatus.Visible)
                AllAchivsLiteral.Text += HtmlCreator.GenerateAllAcheivementHTML(achiv);

            if (User.Achievements.Contains(achiv.Id))
                UserAchivsLiteral.Text += HtmlCreator.GenerateUserAcheivementHTML(achiv);
        }

        //If no achivs write message
        if (User.Achievements.Count == 1)
            UserAchivsLiteral.Text = L1.NOACHIVS;
    }

}
