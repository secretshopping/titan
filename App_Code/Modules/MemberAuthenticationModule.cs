using Prem.PTC.Members;
using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace Prem.PTC.Modules
{
    /// <summary>
    /// Authenticates members basing on cookies
    /// and storing userdata (IMember) in cache. 
    /// Cache object is configured in the same
    /// way as cookie (expiration date and sliding
    /// expiration).<br/>
    /// IMember is saved in cache during authentication 
    /// under Cache[memberSessionKey].<br/>
    /// memberSessionKey is randomly generated string
    /// stored inside cookie
    /// </summary>
    public class MemberAuthenticationModule : IHttpModule
    {
        public MemberAuthenticationModule() { }

        public void Init(HttpApplication application)
        {
            application.AuthenticateRequest +=
                new EventHandler(application_AuthenticateRequest);
        }

        private void application_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpContext econtext = HttpContext.Current;
            string baseUrl = econtext.Request.Url.Scheme + "://" + econtext.Request.Url.Authority + econtext.Request.ApplicationPath.TrimEnd('/') + '/';



                var context = (sender as HttpApplication).Context;

                var authCookie = getAuthorizationCookie(context);
                if (authCookie == null) 
                    return;

                if (baseUrl.Contains("admin.usetitan.com"))
                {
                    var authTicket1 = FormsAuthentication.Decrypt(authCookie.Value);

                    var identity1 = new GenericIdentity(authTicket1.Name);
                    var principal1 = new GenericPrincipal(identity1, new string[0]);
                    context.User = principal1;

                    return;
                }

                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                int memberId = Convert.ToInt32(authTicket.UserData);
                string memberUsername = authTicket.Name;


                var identity = new GenericIdentity(memberUsername);
                var principal = new GenericPrincipal(identity, new string[0]);
                context.User = principal;
        }

        private HttpCookie getAuthorizationCookie(HttpContext context)
        {
            string cookieName = FormsAuthentication.FormsCookieName;
            return context.Request.Cookies[cookieName];
        }

        public void Dispose() { }
    }
}