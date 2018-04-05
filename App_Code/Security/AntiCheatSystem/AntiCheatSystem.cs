using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using System.Data;
using System.Text;
using Prem.PTC.Members;
using System.Collections.Specialized;

namespace Prem.PTC.Security
{
    public class AntiCheatSystem
    {
        public static AntiCheatSystemRuleManager RuleManager = null;
        public static bool IsApprovalEnabled = false;

        public static void CRON()
        {
            RuleManager = new AntiCheatSystemRuleManager();

            DuplicatedPaymentAddresses();
            OtherDuplicateColumns();
            DuplicateIPs();
        }

        public static void AfterLogin(Member User)
        {
            if (User.Status == MemberStatus.Cancelled)
                return;

            //Proxy check
            if (User.BypassSecurityCheck == BypassSecurityCheck.No && AppSettings.Proxy.IPPolicy == IPVerificationPolicy.EveryLogin && ProxyManager.IsProxy(IP.Current))
                User.BanCheater("Proxy IP (" + IP.Current + ")");
        }

        public static void AfterFacebookLogin(Member User)
        {
            if (User.Status == MemberStatus.Cancelled)
                return;

            RuleManager = new AntiCheatSystemRuleManager();
            MultipleFacebook(User);
        }

        #region Rules Logic

        private static void DuplicatedPaymentAddresses()
        {
            if (!RuleManager.CheckIfRuleEnabled(AntiCheatRuleType.DuplicatedPaymentAddresses))
                return;

            string command = string.Empty;

            //Check all payment processor duplications
            command = AntiCheatSystemSQLHelper.BanAllExceptTheOldestSQL();
            TableHelper.ExecuteRawCommandNonQuery(command);

        }

        private static void OtherDuplicateColumns()
        {
            string command = string.Empty;
            string reason = string.Empty;

            if (RuleManager.CheckIfRuleEnabled(AntiCheatRuleType.SameRegisteredIPSameDay))
            {
                reason = RuleManager.GetRuleText(AntiCheatRuleType.SameRegisteredIPSameDay);
                command = AntiCheatSystemSQLHelper.BanAllSQL("RegisteredWithIP, CONVERT(date, RegisterDate)", reason,
                    "RegisteredWithIP IS NOT NULL AND RegisteredWithIP <> ''");
                TableHelper.ExecuteRawCommandNonQuery(command);
            }

            if (RuleManager.CheckIfRuleEnabled(AntiCheatRuleType.SameRegisteredIP))
            {
                reason = RuleManager.GetRuleText(AntiCheatRuleType.SameRegisteredIP);
                command = AntiCheatSystemSQLHelper.BanAllSQL("RegisteredWithIP", reason,
                    "RegisteredWithIP IS NOT NULL AND RegisteredWithIP <> ''");
                TableHelper.ExecuteRawCommandNonQuery(command);
            }
        }

        private static void DuplicateIPs()
        {    
            string command = string.Empty;
            string reason = string.Empty;

            if (RuleManager.CheckIfRuleEnabled(AntiCheatRuleType.SameIPAddressSameDay))
            {
                reason = RuleManager.GetRuleText(AntiCheatRuleType.SameIPAddressSameDay);
                command = AntiCheatSystemSQLHelper.BanAllIPSQL("IP, CONVERT(date, LoginDate)", reason,
                    "IsMasterLogin = 0");
                TableHelper.ExecuteRawCommandNonQuery(command);
            }

            if (RuleManager.CheckIfRuleEnabled(AntiCheatRuleType.SameIPAddress))
            {
                reason = RuleManager.GetRuleText(AntiCheatRuleType.SameIPAddress);
                command = AntiCheatSystemSQLHelper.BanAllIPSQL("IP", reason,
                    "IsMasterLogin = 0");
                TableHelper.ExecuteRawCommandNonQuery(command);
            }
        }


        private static void MultipleFacebook(Member User)
        {
            if (!RuleManager.CheckIfRuleEnabled(AntiCheatRuleType.MultipleFacebook))
                return;

            string command = string.Empty;
            string reason = RuleManager.GetRuleText(AntiCheatRuleType.MultipleFacebook);

            command = AntiCheatSystemSQLHelper.BanAllSQL("FacebookName", reason,
                "FacebookName IS NOT NULL AND FacebookName <> ''");
            TableHelper.ExecuteRawCommandNonQuery(command);
        }

        #endregion 

       
        public static string GetUserInformation()
        {
            HttpRequest request = HttpContext.Current.Request;

            var sb = new StringBuilder();
            sb.Append(request.Browser.Browser);
            sb.Append(request.Browser.Version);
            sb.Append(request.Browser.ScreenPixelsWidth);
            sb.Append("x");
            sb.Append(request.Browser.ScreenPixelsHeight);
            sb.Append(request.Browser.Platform);
            sb.Append(request.Browser.MSDomVersion);
            sb.Append(request.Browser.JScriptVersion.ToString());
            sb.Append(request.Browser.ClrVersion);
            sb.Append(request.Browser.W3CDomVersion);
            return sb.ToString();
        }

    }
}