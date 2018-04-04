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
using Prem.PTC.Achievements;

public partial class Controls_MemberAchievementsList : System.Web.UI.UserControl
{
    public int MaxMiniaturesShown { get; set; }
    public string TargetUsername { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        //Display achievements
        if (!Page.IsPostBack)
        {
            Member Target = null;
            try
            {
                if (!string.IsNullOrEmpty(TargetUsername))
                    Target = new Member(TargetUsername);
                else if (Member.IsLogged)
                    Target = Member.CurrentInCache;
            }
            catch (Exception ex) { }

            if (Target != null)
            {

                int counter = 0;
                foreach (int achivId in Target.Achievements)
                {
                    if (achivId != -1)
                    {
                        if (counter == MaxMiniaturesShown)
                        {
                            //We dont display more, just '...'
                            AchievementsLiteral.Text += "...";
                            break;
                        }
                        Achievement achiv = new Achievement(achivId);
                        AchievementsLiteral.Text += HtmlCreator.GenerateSmallAcheivementHTML(achiv);
                        counter++;
                    }
                }
            }
        }
    }

}
