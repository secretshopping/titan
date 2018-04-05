using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Members;
using Resources;

public partial class sites_profiler : TitanPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Member.IsLogged && !ProfilingManager.IsProfiled(Member.Current))
        {
            LangAdder.Add(ProfileButton, L1.SEND);
            LangAdder.Add(SkipButton, U3900.SKIP);

            if (AppSettings.Authentication.CustomFieldsAsSurvey)
            {
                var panel = RegistrationFieldCreator.Generate(true);
                Member User = Member.CurrentInCache;

                foreach (var control in panel.Controls)
                {
                    if (control is TextBox || control is CheckBox)
                    {
                        WebControl target = (WebControl)control;
                        string id = target.ID.Replace("CustomField", "");

                        if (User.Custom.ContainsKey(id))
                        {
                            target.Enabled = false;

                            if (control is TextBox)
                                ((TextBox)target).Text = User.Custom.ToList().FirstOrDefault(x => x.Key == id).Value.ToString();

                            if (control is CheckBox)
                                ((CheckBox)target).Checked = Convert.ToBoolean(User.Custom.ToList().FirstOrDefault(x => x.Key == id).Value);
                        }
                    }
                }

                CustomFields.Controls.Add(panel);
            }
        }
    }


    protected void ProfileButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            Member User = Member.Current;
            RegistrationFieldCreator.Save(User, this.CustomFields, true);

            User.AddToPointsBalance(AppSettings.Authentication.ProfilingSurveyReward, "Profiling survey");
            User.Save();

            Response.Redirect("~/status.aspx?type=profilerok");
        }
    }

    protected void SkipButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/user/default.aspx?afterlogin=1&skippedprofiling=1");
    }
}