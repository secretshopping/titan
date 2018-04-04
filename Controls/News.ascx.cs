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

public partial class Controls_News : System.Web.UI.UserControl
{
    public int NewsInFeed { get; set; }

    List<WebsiteText> AllNews;

    protected void Page_Load(object sender, EventArgs e)
    {
        //Display news
        var dict = new Dictionary<string, object>();
        dict.Add("TextType", (int)WebsiteTextType.NewsNote);
        dict.Add("IsVisible", true);
        AllNews = TableHelper.SelectRows<WebsiteText>(dict);
        AllNews.Sort(Comparison);

        //Dsiplay only 4
        NewsLiteral.Text = "";
        string FirstOne = "";
        var newsText = new StringBuilder();
        for (int i = 0; i < NewsInFeed && i < AllNews.Count; ++i)
        {
            if (i == 0)
                FirstOne = "style=\"margin-top:5px\"";

            newsText.Append(string.Format("<div class=\"col-md-12\" {0}>", FirstOne));
            if(i == 0 || (i > 0 && AllNews[i].Created.Date != AllNews[i-1].Created.Date))
                newsText.Append(string.Format("<p>{0}</p>", AllNews[i].Created.ToShortDateString()));
            newsText.Append(string.Format("<p><a href=\"sites/news.aspx?news={0}\">{1}</a></p></div>", AllNews[i].Id, AllNews[i].Title));
        }
        NewsLiteral.Text += newsText;
    }

    public int Comparison(WebsiteText x, WebsiteText y)
    {
        if (x.Created < y.Created)
            return 1;
        else if (x.Created == y.Created)
            return 0;
        return -1;
    }
}
