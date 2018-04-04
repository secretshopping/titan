using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;

public partial class Controls_Countdown : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppSettings.Addons.Reload();
        CountDownPlaceHolder.Visible = AppSettings.Addons.IsCustomCounterEnabled;
        DeadLineHiddenField.Value = AppSettings.Addons.CustomCounterDeadLine.ToString();
    }
}