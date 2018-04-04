using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using System.Web.Security;
using System.Web.UI;
using Resources;
using System.Text;
using Titan.Registration;

public class PageMasterHelper
{
    public bool HasNotifications;
    public bool DisplayNotifications;

    public void PageLoad()
    {
        HttpContext context = HttpContext.Current;
        Page page = (Page)context.Handler;

        if (Member.IsLogged)
        {
            if (!page.IsPostBack)
            {
                Member User = Member.CurrentInCache;                

                //Member status check
                if (User.LastLogged != null && User.LastLogged.Value < DateTime.Now.AddDays(-3))
                {
                    //Member was logged but the data hasn't been updated
                    //E.g. long cookie time expiration
                    User.Login();
                    User.Save();
                }

                //Redeirect if account activation fee is turned on and is not paid for current user
                if (AppSettings.Registration.IsAccountActivationFeeEnabled && !User.IsAccountActivationFeePaid && !AppSettings.Authentication.AnonymousMemberEnabled)
                {
                    if (AppSettings.Registration.AccountActivationFeeVia == AccountActivationFeeVia.MainSite)
                        HttpContext.Current.Response.Redirect("~/sites/activation.aspx");

                    if (!HttpContext.Current.Request.Url.AbsolutePath.StartsWith("/user/transfer.aspx") && !HttpContext.Current.Request.Url.AbsolutePath.StartsWith("/user/default.aspx"))
                        HttpContext.Current.Response.Redirect("~/user/default.aspx");
                }

                //Notifications
                HasNotifications = (bool)context.Application["HasNotifications"];

                if (HasNotifications && (User.CPAOfferCompletedBehavior == CPACompletedBehavior.PopupOnScreen ||
                        User.CPAOfferCompletedBehavior == CPACompletedBehavior.PopupAndSendEmail))
                    DisplayNotifications = true;

                if (LastLastActivityRefreshTime < DateTime.Now.AddMinutes(-5))
                {
                    LastLastActivityRefreshTime = DateTime.Now;
                    User.UpdateLastActivityTime();
                }

                if (AppSettings.Points.LevelMembershipPolicyEnabled)
                {
                    var levelNotifications = LevelNotification.Get(User.Id, false);
                    foreach (var n in levelNotifications)
                        LevelNotification.TryDisplay(n);
                }
            }

            //Disable cache for all User.master inner pages
            //So when you logout, you can't click Browser Back button and see all private content
            HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
            HttpContext.Current.Response.AddHeader("Expires", "0");
        }
    }

    private const string LastLastActivity_Name = "LastLastActivity";

    public static DateTime LastLastActivityRefreshTime
    {
        get
        {
            if (HttpContext.Current.Session[LastLastActivity_Name] == null)
                HttpContext.Current.Session[LastLastActivity_Name] = DateTime.Now.AddDays(-10); //Never

            return (DateTime)HttpContext.Current.Session[LastLastActivity_Name];
        }
        set
        {
            HttpContext.Current.Session[LastLastActivity_Name] = value;
        }
    }
}