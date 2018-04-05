using System;
using System.Web;
using System.Web.UI;
using Prem.PTC;
using Prem.PTC.Members;

public partial class Controls_MainMenu : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Member.IsLogged)
        {
            ForLoggedInMembers.Visible = true;
            ForNotLoggedInMembers.Visible = false;
        }
        else
        {
            ForLoggedInMembers.Visible = false;
            ForNotLoggedInMembers.Visible = true;
        }

        //We want to display searchbar for TITAN News
        SearchPlaceHolder.Visible = AppSettings.TitanFeatures.NewsHomepageEnabled && 
            ((HttpContext.Current.Handler as Page).Request.Url.AbsolutePath == "/sites/defaultnews.aspx" ||
            ( HttpContext.Current.Handler as Page).Request.Url.AbsolutePath == "/sites/searchnews.aspx");
        SearchTextBox.Attributes["placeholder"] = String.Format(Resources.U6013.SEARCHON, AppSettings.Site.Name);

        //Turn some features on and off
        try
        {
            mm4.Visible = pmm5.Visible = AppSettings.Site.ForumEnabled;
            //pmm7.Visible = mm7.Visible = AppSettings.TitanFeatures.PaymentProofsEnabled;
        }
        catch (Exception ex) { }
    }

    public string ResolveURL(string path)
    {
        return (HttpContext.Current.Handler as Page).ResolveUrl(path);
    }

    public string GetNotificationHTML(int number)
    {
        return HtmlCreator.GenerateMenuNotificationNumber(number);
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        string text = SearchTextBox.Text.Trim();
        if (!string.IsNullOrWhiteSpace(text))
        {
            text = HttpUtility.UrlEncode(text);
            Response.Redirect("~/sites/searchnews.aspx?q=" + text);
        }
    }

    #region Notifications
    public int Account
    {
        get
        {
            return NotificationManager.GetAccountSum();
        }
    }

    public int Ads
    {
        get
        {
            return NotificationManager.Get(NotificationType.NewAds);
        }
    }
        
    public int Offers
    {
        get
        {
            return NotificationManager.Get(NotificationType.NewCPAOffers);
        }
    }
    #endregion
}