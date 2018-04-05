using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Utils;
using System.Web.UI.WebControls;
using Resources;
using System.Text;
using System.Net;
using System.Collections.Specialized;

public partial class About : TitanPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AdBlockManager.RunOnAdBlockRedirectPage();
    }
}
