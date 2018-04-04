using System;
using Prem.PTC.Texts;

public partial class sites_tos : TitanPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var dict = TableHelper.MakeDictionary("TextType", (int)WebsiteTextType.ToS);
        var list = TableHelper.SelectRows<WebsiteText>(dict);
        Literal1.Text = list[0].Content;
    }
}