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

public partial class Controls_UserWarning : System.Web.UI.UserControl
{

    protected void Page_Load(object sender, EventArgs e)
    {
        //Display VacationMode warning
        if (Member.IsLogged && Member.CurrentInCache.Status == MemberStatus.VacationMode)
        {
            UserWarningControl.Visible = true;
            UserWarningControlText.Text = U4000.YOUAREINVACATIONMODE;
        }
    }

}
