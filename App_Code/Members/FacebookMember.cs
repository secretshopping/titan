using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Facebook;
using Prem.PTC.Advertising;
using System.Net;
using System.Web.Security;

namespace Prem.PTC.Members
{
    public class FacebookMember
    {
        public bool HasProfilePicture { get; set; }
        public int Friends { get; set; }
        public string Name
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        private string LikesHash { get; set; }
        private FacebookClient client;

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FacebookId { get; set; }
        public Gender Gender { get; set; }
        public string Email { get; set; }

        public FacebookMember(string accessToken)
        {
            try
            {
                client = new FacebookClient(accessToken);
                dynamic result = client.Get("me", new { fields = "picture, friends, first_name, last_name, id, gender, email" });

                // Friends 
                if (result.friends != null)
                    Friends = Convert.ToInt32(result.friends.summary.total_count.ToString());
                else
                    Friends = 0;

                // Profile picture
                HasProfilePicture = Convert.ToBoolean(result.picture.is_silhouette);

                //Name
                FirstName = result.first_name;
                LastName = result.last_name;
                FacebookId = result.id;

                if (!string.IsNullOrWhiteSpace(result.gender))
                {
                    if (result.gender == "male")
                        Gender = Gender.Male;
                    else if (result.gender == "female")
                        Gender = Gender.Female;
                }
                else
                    Gender = Gender.Null;

                Email = result.email;
            }
            catch (FacebookOAuthException ex)
            {
            
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
        }


        /// <summary>
        /// Returns AccessToken or null if not logged
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string Logged()
        {
            if (HttpContext.Current.Session["AccessToken"] == null)
                return null;
            return HttpContext.Current.Session["AccessToken"].ToString();
        }

        /// <summary>
        /// Include this code right after the 'body' tag
        /// </summary>
        /// <returns></returns>
        public static string GetJSStartingCode()
        {
            string currentLocale = "en_US";

            HttpCookie cookie = HttpContext.Current.Request.Cookies["CultureInfo"];
            if (cookie != null && cookie.Value != null)
            {
                currentLocale = cookie.Value.Replace("-", "_");
            }

            return @"<div id=""fb-root""></div>
                            <script>(function(d, s, id) {
                                var js, fjs = d.getElementsByTagName(s)[0];
                                if (d.getElementById(id)) return;
                                js = d.createElement(s); js.id = id;
                                js.src = ""//connect.facebook.net/" + currentLocale + @"/all.js"";
                                fjs.parentNode.insertBefore(js, fjs);
                            }(document, 'script', 'facebook-jssdk'));</script>";
        }

        public static string GetLikeButtonCode(FacebookAdvert ad)
        {
            string code = "<div class=\"fb-like\" data-ad-id=\"" + ad.Id + "\" data-href=\"" + ad.TargetUrl + "\" data-action=\"like\" data-layout=\"standard\" data-width=\"450\" data-show-faces=\"false\"></div>";
            return code;
        }
    }
}