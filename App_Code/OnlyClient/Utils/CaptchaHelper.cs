using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Security.Cryptography;
using System.Text;
using VisualCaptcha;

namespace Prem.PTC.Utils
{
    public static class CaptchaHelper
    {
        private static string GetSessionKey(HttpContext context)
        {
            return context.Request.Params["namespace"];
        }

        public static bool Verify(string value)
        {
            string sessionKey = GetSessionKey(HttpContext.Current);

            if (HttpContext.Current.Session[sessionKey] == null)
                return false;

            var captcha = (Captcha)HttpContext.Current.Session[sessionKey];
            var success = captcha.ValidateAnswer(value);
            return success;
        }
    }
}