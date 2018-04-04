using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using System.Web.Security;
using Resources;
using Titan.Registration;

namespace Titan
{

    public class TitanAuthService
    {
        /// <summary>
        /// Throws MsgException with proper error message
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="password2"></param>
        public static void Login(string username, string password, string password2, bool redirect = true)
        {
            MemberAuthenticationService authservice = new MemberAuthenticationService();
            MemberAuthenticationData data = new MemberAuthenticationData(username, password, password2);
            Member member = authservice.Authenticate(data, false, HttpContext.Current);

            TitanAuthService.AuthenticateWithChecks(member, false, redirect);
        }

        public static void LoginOrRegister(string FacebookOAuthId, bool redirect = true)
        {
            var list = TableHelper.SelectRows<Member>(TableHelper.MakeDictionary("FacebookOAuthId", FacebookOAuthId));
            if (list.Count == 1)
            {
                //Already registered, login
                Member member = list[0];
                MemberAuthenticationService.CreateAuthCredentials(member);

                member.IsFromMasterLogin = false;
                TitanAuthService.AuthenticateWithChecks(member, false, redirect);
            }
            else if (list.Count == 0)
            {
                //Not registered yet

                if (!redirect)
                    throw new MsgException(L1.ER_USER_NOTFOUND);

                //Redirect to Register page with Facebook
                HttpContext.Current.Response.Redirect("register.aspx?fb=1");
            }
        }

        public static void LoginOrRegister(FacebookMember fbMember, bool redirect = true)
        {
            LoginOrRegister(fbMember.FacebookId, redirect);
        }

        public static void Reactivate(string username, string password, string password2)
        {
            MemberAuthenticationService authservice = new MemberAuthenticationService();
            MemberAuthenticationData data = new MemberAuthenticationData(username, password, password2);
            Member member = authservice.Authenticate(data, false, HttpContext.Current);

            TitanAuthService.AuthenticateWithChecks(member, true);
        }

        public static void AuthenticateWithChecks(Member member, bool reactivating = false, bool redirect = true)
        {

            // 2. Check user status (active, banned)
            if (member.Status == MemberStatus.Inactive)
                throw new MsgException(Resources.L1.ACCNOTACTIVATED);

            if (member.Status == MemberStatus.Cancelled)
                throw new MsgException(Resources.L1.ACCCANCELLED);

            if (member.Status == MemberStatus.Expired && reactivating == false)
            {
                //Account expired, but maybe reactivation is enabled
                throw new SpecialException(Resources.L1.ACCINACTIVE);
            }

            if (reactivating)
            {
                member.Reactivate();
                member.Save();
            }

            try
            {
                if (member.Status == MemberStatus.Locked)
                {
                    string reason = "";
                    if (!string.IsNullOrEmpty(member.DetailedBanReason))
                        reason = " (" + member.DetailedBanReason + ")";

                    throw new MsgException(Resources.L1.ACCLOCKED + reason);
                }

                if (member.Status == MemberStatus.BannedOfTos)
                    throw new MsgException(Resources.L1.ACCTOS + ": " + member.DetailedBanReason);

                if (member.Status == MemberStatus.BannedOfCheating)
                    throw new MsgException(Resources.L1.ACCCHEAT + ": " + member.DetailedBanReason);

                if (member.Status == MemberStatus.BannedBlacklisted)
                    throw new MsgException(Resources.L1.ACCBLACK + ": " + member.DetailedBanReason);

                // 3. Run Security Manager to ban cheaters instantly
                if (!member.IsFromMasterLogin)
                {
                    Prem.PTC.Security.SecurityManager.AfterLogin(member);
                    member.SaveStatus();

                    Prem.PTC.Security.AntiCheatSystem.AfterLogin(member);
                }

                // 4. After this run member can be banned so check banned conditions again
                if (member.Status == MemberStatus.BannedOfCheating)
                    throw new MsgException(Resources.L1.ACCCHEAT + ": " + member.DetailedBanReason);

                if (member.Status == MemberStatus.BannedBlacklisted)
                    throw new MsgException(Resources.L1.ACCBLACK + ": " + member.DetailedBanReason);
            }
            catch (MsgException message)
            {
                if (AppSettings.Authentication.ShowDetailedBanMessage)
                    throw message;

                throw new MsgException(Resources.U4000.FRAUDINFO.Replace("%a%", "<u><a href='sites/contact.aspx'>").Replace("%b%", "</a></u>")); //General info about potential fraud
            }

            if (member.Status == MemberStatus.AwaitingSMSPIN)
            {
                FormsAuthentication.SignOut();
                HttpContext.Current.Session.Abandon();

                if (redirect)
                    HttpContext.Current.Response.Redirect("~/sites/phone.aspx?username=" + member.Name + "&code=" + member.Id);
                
            }

            // If not
            // 5. Login with data
            member.Login(member.IsFromMasterLogin);
            member.Save();

            //Display login add if enabled and not watched yet
            if (redirect)
            {
                //Redirect to Account Activation page if activation fee is enabled and account is not activated yet and type of activation is via main site
                if (AppSettings.Registration.IsAccountActivationFeeEnabled && !member.IsAccountActivationFeePaid && AppSettings.Registration.AccountActivationFeeVia == AccountActivationFeeVia.MainSite)
                    HttpContext.Current.Response.Redirect("~/sites/activation.aspx");

                LoginManager.TryDisplayLoginAd(member);
                HttpContext.Current.Response.Redirect("~/user/default.aspx?afterlogin=1");
            }
        }
    }
}