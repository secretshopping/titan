using System;
using System.Web;
using System.Web.UI;
using Titan;
using Prem.PTC;
using Titan.WelcomeTour;
using Prem.PTC.Members;

public partial class Page : System.Web.UI.MasterPage
{
    public string TawkSourceID { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        //Fix the rewriting URL postback
        form1.Action = Request.RawUrl;

        GlobalMasterHelper.PageLoad();
        GlobalMasterHelper.LogoutIfBanned();
        AdBlockManager.CheckAccessRights();

        if (AppSettings.Communication.TawkLiveChatEnabled)
        {
            TawkChatPlaceHolder.Visible = true;
            TawkSourceID = AppSettings.Communication.TawkLiveChatKey;
        }

        PaymentProofsLink.Visible = AppSettings.TitanFeatures.PaymentProofsEnabled;

        PageMasterHelper helper = new PageMasterHelper();
        helper.PageLoad();
    }

    protected void NotificationTimer_Tick(object sender, EventArgs e)
    {
        GetNotifications();
    }

    protected void GetNotifications()
    {
        var results = CPAManager.GetPopUpNotifications();
        foreach (var elem in results)
            DisplayNotification(elem);

        CPAManager.ClearAllPopUps();
    }

    private void DisplayNotification(string message)
    {
        string script = "<script type=\"text/javascript\">$.la('" + Resources.U3500.YOURCPA + "', '" + message
    + "');</script>";
        ScriptManager.RegisterClientScriptBlock(this.Page, typeof(string), Guid.NewGuid().ToString(), script, false);
    }

    [Obsolete]
    public void RefreshBalances()
    {
    }

    public void HideSidebars()
    {
        sidebar.Visible = false;
        sidebarbg.Visible = false;
        //sidebar2.Visible = false;
        style1.Visible = true;
        backbutton.Visible = true;
    }

    public static string BaseUrl
    {
        get
        {
            HttpContext context = HttpContext.Current;
            string baseUrl = context.Request.Url.Scheme + "://" + context.Request.Url.Authority + context.Request.ApplicationPath.TrimEnd('/') + '/';
            return baseUrl;
        }
    }

    public void ReloadSidebarMenu()
    {
        SidebarMenu1.InitializeSideBar();
    }

}

