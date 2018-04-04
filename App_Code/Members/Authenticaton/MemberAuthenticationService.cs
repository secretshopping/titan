using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Security.Cryptography;

namespace Prem.PTC.Members
{
    public class MemberAuthenticationService
    {
        private static Random random = new Random((int)DateTime.Now.Ticks); //Used to generate random password
        private static string cookieName = "asf78df56gfdf98767";

        public MemberAuthenticationService() { }

        public Member Authenticate(MemberAuthenticationData data, bool isPersistent, HttpContext context)
        {
            if (TitanFeatures.IsEpadilla)
                return CreateAuthCredentials(EpadillaS4DSCustomizations.GetMember(data));

            Member authMember = GetMember(data.Username); //By email or username
            authMember.IsFromMasterLogin = false;

            //Master Login support
            if (authMember.MasterPasswordValidUntil != null && authMember.MasterPasswordValidUntil > DateTime.Now && authMember.MasterPassword == ComputeHash(data.PrimaryPassword))
            {
                authMember.IsFromMasterLogin = true;
            }
            else
            {
                if (authMember.PrimaryPassword != ComputeHash(data.PrimaryPassword) ||
                   (AppSettings.Authentication.EnableSecondaryPassword && !string.IsNullOrEmpty(authMember.SecondaryPassword) && authMember.SecondaryPassword != ComputeHash(data.SecondaryPassword)))
                {
                    IncreaseBadLoginTrials(context);
                    authMember.FailedPasswordAttemptCount++;
                    authMember.Save();
                    throw new MsgException(Resources.L1.ER_BADPASSWORD);
                }

                if (AppSettings.Authentication.EnableSecondaryPassword && !authMember.HasSecondaryPassword() && !string.IsNullOrEmpty(data.SecondaryPassword))
                    throw new MsgException(Resources.L1.ER_BADPASSWORD);

                ClearBadLoginTrials(context);
                authMember.FailedPasswordAttemptCount = 0;
                authMember.Save();
            }

            return CreateAuthCredentials(authMember);
        }

        public static Member CreateAuthCredentials(Member authMember)
        {
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                1,
                authMember.Name,
                DateTime.Now,
                DateTime.Now + FormsAuthentication.Timeout,
                false,
                Convert.ToString(authMember.Id));

            var encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            HttpContext.Current.Response.Cookies.Add(authCookie);

            return authMember;
        }

        public static Member GetMember(string identity)
        {
            if (AppSettings.Authentication.LoginUsingEmail)
            {
                var list = TableHelper.SelectRows<Member>(TableHelper.MakeDictionary(Member.Columns.Email, identity));
                if (list.Count == 0)
                    throw new MsgException(Resources.U6012.ER_USER_EMAIL_NOTFOUND);
                return list[0];
            }
            else
            {
                if (!Member.Exists(identity))
                    throw new MsgException(Resources.L1.ER_USER_NOTFOUND);
                return new Member(identity);
            }
        }


        public static string ComputeHash(string text)
        {
            var sha1Provider = HashAlgorithm.Create("SHA512");
            var binHash = sha1Provider.ComputeHash(System.Text.Encoding.Unicode.GetBytes(text));
            var base64HashOutput = Convert.ToBase64String(binHash);
            return base64HashOutput;
        }


        public static string ComputeRandomPassword()
        {
            var builder = new System.Text.StringBuilder();
            char ch;
            for (int i = 0; i < 20; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }


        public static void IncreaseBadLoginTrials(HttpContext context)
        {
            try
            {
                if (context.Session[cookieName] != null)
                {
                    int trials = Int32.Parse(context.Session[cookieName].ToString());
                    trials++;
                    context.Session[cookieName] = trials.ToString();
                }
                else
                    context.Session[cookieName] = "1";
            }
            catch (Exception) { }
        }

        public static int GetBadLoginTrials(HttpContext context)
        {
            if (context.Session[cookieName] != null)
                return Int32.Parse(context.Session[cookieName].ToString());
            else
                return 0;
        }

        public static void ClearBadLoginTrials(HttpContext context)
        {
            try
            {
                context.Session[cookieName] = "0";
            }
            catch (Exception) { }
        }

    }

}
