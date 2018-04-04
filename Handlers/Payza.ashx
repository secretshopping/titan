<%@ WebHandler Language="C#" Class="Prem.PTC.Payza" %>

using System;
using System.IO;
using System.Net;
using System.Web;
using System.Reflection;
using Prem.PTC.Payments;
using Prem.PTC.Utils.NVP;
using ExtensionMethods;

namespace Prem.PTC
{
    public class Payza : IHttpHandler
    {
        private const bool IsSandboxMode = false;
        private const string IpnHandlerUrl = "https://secure.payza.eu/ipn2.ashx";
        private const string IpnHandlerSandboxUrl = "https://sandbox.Payza.eu/sandbox/IPN2.ashx";

        private string IpnHandler { get { return IsSandboxMode ? IpnHandlerSandboxUrl : IpnHandlerUrl; } }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                ErrorLogger.Log(context.Request.ToRawString(), LogType.Payza);

                string strResponse;
                HandleConnection(context, out strResponse);

                if (string.Equals(strResponse, "INVALID TOKEN", StringComparison.OrdinalIgnoreCase))
                    throw new MsgException("Invalid TOKEN");

                ProcessPayment(strResponse);
            }
            catch (Exception ex) { ErrorLogger.Log(ex); }
        }

        private void HandleConnection(HttpContext context, out string strResponse)
        {
            // Disclaimer: Unlike PayPal, Payza doesn't expect HTTP 200 empty response before resending token

            string token = context.Request.Form["token"];
            token = "token=" + context.Server.UrlEncode(token);

            //Post back token
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(IpnHandler);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = token.Length;

            WebRequestUtils.SetUpSecurityProtocols();

            //Send the request to Payza and get the response
            StreamWriter streamOut = streamOut = new StreamWriter(req.GetRequestStream());
            streamOut.Write(token);
            streamOut.Close();

            WebRequestUtils.SetUpSecurityProtocols();

            StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
            strResponse = streamIn.ReadToEnd().Trim();
            streamIn.Close();
        }

        private void ProcessPayment(string strRequest)
        {
            var censoredRequest = censorRequest(strRequest);

            if (string.Equals(censoredRequest["ap_status"], "Success", StringComparison.OrdinalIgnoreCase) &&
            PayzaAccountDetails.Exists(censoredRequest["ap_merchant"]))
            {
                string commandName = censoredRequest["apc_1"];
                Assembly assembly = Assembly.GetAssembly(typeof(IIpnHandler));
                var type = assembly.GetType(commandName, true, true);
                IIpnHandler command = Activator.CreateInstance(type) as IIpnHandler;
                command.HandlePayza(censoredRequest.ToString());
            }
        }

        private NameValuePairs censorRequest(string strRequest)
        {
            var request = NameValuePairs.Parse(strRequest);

            request["ap_custfirstname"] =
            request["ap_custlastname"] =
            request["ap_custaddress"] =
            request["ap_custcity"] =
            request["ap_custstate"] =
            request["ap_custzip"] =
            request["ap_custemailaddress"] = "[TITAN_DELETED]";

            return request;
        }

        public bool IsReusable { get { return false; } }
    }
}