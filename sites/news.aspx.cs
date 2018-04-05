using System;
using System.Reflection;
using System.Web.UI;
using Prem.PTC;
using Prem.PTC.Texts;

public partial class sites_news : TitanPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!AppSettings.Site.LatestNewsEnabled)
            Response.Redirect("~/user/default.aspx");

        int newsId;

        if(!int.TryParse(Request.QueryString["news"], out newsId) && TableHelper.SelectScalar(string.Format("SELECT * FROM Texts WHERE TextType = {0}", (int)WebsiteTextType.NewsNote)) != null)
            newsId = (int)TableHelper.SelectScalar(string.Format("SELECT TOP 1 TextId FROM Texts WHERE TextType = {0} ORDER BY LastModifiedDate DESC", (int)WebsiteTextType.NewsNote));

        var whereNewsNotes = TableHelper.MakeDictionary("TextId", newsId);
        var news = TableHelper.SelectRows<WebsiteText>(whereNewsNotes);

        if (news.Count == 0)
            NoNewsPlaceHolder.Visible = true;
        else
        {
            NewsPlaceHolder.Visible = true;

            if (news.Count == 1)
            {
                //Display the news
                TitleLiteral.Text = news[0].Title;
                DateLiteral.Text = news[0].Created.ToString();
                TextLiteral.Text = news[0].Content;
            }

            UserControl newsControl = (UserControl)Page.LoadControl("~/Controls/News.ascx");

            PropertyInfo newsProperty = newsControl.GetType().GetProperty("NewsInFeed");
            newsProperty.SetValue(newsControl, 4, null);

            newsControl.DataBind();
            LatestNewsPlaceHolder.Controls.Add(newsControl);
        }
    }
}


