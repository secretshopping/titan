using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC.Texts;

public partial class sites_privacy : TitanPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var dict = TableHelper.MakeDictionary("TextType", (int)WebsiteTextType.PrivacyPolicy);
        var list = TableHelper.SelectRows<WebsiteText>(dict);
        Literal2.Text = list[0].Content;
    }
}