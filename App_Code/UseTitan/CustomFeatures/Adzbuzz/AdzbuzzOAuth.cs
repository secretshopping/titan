using Newtonsoft.Json.Linq;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Utils.NVP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.CustomFeatures
{
    public class AdzbuzzOAuth
    {
        public static readonly string SessionStateKey = "OAuthState";
        public static readonly string HandlerUrl = "https://titan.adzbuzz.com/Handlers/Utils/AdzbuzzOAuth.ashx";

        public static void CreateRedirect()
        {
            string key = HashingManager.SHA256(DateTime.Now + AppSettings.Offerwalls.UniversalHandlerPassword + SessionStateKey).ToLower();
            HttpContext.Current.Session[SessionStateKey] = key;

            var systemRedirectUri = GetRedirectUrl(key);  
            HttpContext.Current.Response.Redirect(systemRedirectUri);
        }

        public static string GetRedirectUrl(string key)
        {
            return OAuth2.CreateRedirect(GetProvider(key), HandlerUrl); 
        }

        public static void GetTokenAndLogin(string code, string state)
        {
            if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionStateKey] != null 
                && HttpContext.Current.Session[SessionStateKey].ToString() != state)
                throw new Exception("Invalid OAuth state.");

            string accessToken  = 
                OAuth2.AuthenticateByCode(GetProvider(state), HandlerUrl, code).AccessToken;

            string userInfoString = OAuth2.GetUserInfo(GetProvider(state), accessToken);

            JObject UserInfo = JObject.Parse(userInfoString);
            JToken UserObject = UserInfo["userObj"];

            string username = UserObject["id"].ToString();

            if (!Member.Exists(username))
            {
                //Register

                string email = UserObject["email"].ToString();
                DateTime birthYear = new DateTime((int)UserObject["birth"]["year"], 1, 1);

                TitanRegisterService.Register(username, email, 1234, birthYear,
                HashingManager.GenerateMD5(DateTime.Now + username), String.Empty, Gender.Male, null, String.Empty,
                String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, true, false, false, null, true);
            }

            Member member = new Member(username);

            member = MemberAuthenticationService.CreateAuthCredentials(member);

            member.BirthYear = new DateTime((int)UserObject["birth"]["year"], 1, 1);
            member.FirstName = UserObject["first_name"].ToString();
            member.SecondName = UserObject["last_name"].ToString();
            member.AvatarUrl = UserObject["avatar_url"].ToString();

            NotNullNameValuePairs nvp = new NotNullNameValuePairs();
            nvp.Add("adzbuzz_affid", UserObject["affid"].ToString());
            nvp.Add("adzbuzz_userid", UserObject["id"].ToString());

            member.Custom = nvp;

            member.Save();

            TitanAuthService.AuthenticateWithChecks(member, false, true);
        }

        private static OAuth2Provider GetProvider(string key)
        {
            return new OAuth2Provider
            {
                ClientId = "titan.adzbuzz.com",
                ClientSecret = "nosecret",
                Scope = "basic",
                AuthUri = "https://adzbuzz.com/oauth/authorize",
                AccessTokenUri = "https://adzbuzz.com/oauth/token",
                UserInfoUri = "https://adzbuzz.com/oauth/resource",
                State = key
            };
        } 

    }
}