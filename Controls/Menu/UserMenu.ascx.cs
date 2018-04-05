using System;
using System.Web;
using System.Web.UI;
using Prem.PTC;
using Resources;
using Prem.PTC.Members;

public partial class Controls_UserMenu : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Forum.Visible = AppSettings.Site.ForumEnabled;
        News.Visible = AppSettings.Site.LatestNewsEnabled;
        SearchPlaceHolder.Visible = AppSettings.TitanFeatures.SocialNetworkEnabled;
        SearchTextBox.Attributes.Add("placeholder", U6003.FINDPEOPLE);

        if (Member.IsLogged)
        {
            try
            {
                UpgradeMenu.Visible = AppSettings.TitanFeatures.UpgradeEnabled;
            }
            catch (Exception ex) { }
        }
        else
            MainPanel.Visible = false;

        if (TitanFeatures.IsTrafficThunder)
        {
            newsAnhor.HRef = "http://project-laya.com/blog/";
            newsAnhor.Target = "_blank";
        }
        else
            newsAnhor.HRef = ResolveURL("~/sites/news.aspx");
    }

    public string ResolveURL(string path)
    {
        return (HttpContext.Current.Handler as Page).ResolveUrl(path);
    }


    protected void SearchButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/user/searchresults.aspx?q=" + HttpUtility.UrlEncode(SearchTextBox.Text));
    }
}
