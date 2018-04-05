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

public partial class Controls_StartSurfingAdPack : System.Web.UI.UserControl, ICustomObjectControl
{
    public int ObjectID { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        ButtonPlaceholder.Visible = AppSettings.RevShare.AdPack.IsStartSurfingEnabled;
    }
}