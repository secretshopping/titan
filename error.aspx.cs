using Prem.PTC.Members;
using System;
using System.Security.Principal;
using System.Web.Security;
using System.Web.UI.WebControls;
using Prem.PTC;
using System.Web;
using System.Web.Caching;
using System.Security.Cryptography;

public partial class _Default : TitanPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        Literal1.Text = Resources.L1.ERROR;
        Literal2.Text = Resources.L1.ERROR_INFO;
    }

}
