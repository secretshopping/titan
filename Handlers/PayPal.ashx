<%@ WebHandler Language="C#" Class="PayPal" %>

using System;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Reflection;
using Prem.PTC.Utils.NVP;
using Prem.PTC.Payments;
using Prem.PTC.Members;
using Prem.PTC;
using ExtensionMethods;

public class PayPal : IHttpHandler
{
    private const bool IsSandboxMode = false;
    private const string IpnHandlerUrl = "https://www.paypal.com/cgi-bin/webscr";
    private const string IpnHandlerSandboxUrl = "https://www.sandbox.paypal.com/cgi-bin/webscr";

    private string IpnHandler { get { return IsSandboxMode ? IpnHandlerSandboxUrl : IpnHandlerUrl; } }

    public void ProcessRequest(HttpContext context)
    {
        try
        {
            string LogMessage = context.Request.Params.ToRawString();
            ErrorLogger.Log(LogMessage, LogType.PayPal);

            string strRequest;
            string strResponse;

            HandleConnection(context, out strRequest, out strResponse);

            if (strResponse == "VERIFIED")
                ProcessPayment(strRequest);
        }
        catch (Exception ex) { ErrorLogger.Log(ex); }

    }

    private void HandleConnection(HttpContext context, out string strRequest, out string strResponse)
    {
        var Response = context.Response;
        Response.Clear();
        Response.ContentType = "application/x-www-form-urlencoded";
        Response.StatusCode = 200;
        Response.Write("");
        Response.Flush();

        //Post back to either sandbox or live
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(IpnHandler);

        //Set values for the request back
        byte[] param = context.Request.BinaryRead(context.Request.ContentLength);
        strRequest = Encoding.ASCII.GetString(param).Trim();
        strRequest = "cmd=_notify-validate&" + strRequest;
        req.Method = "POST";
        req.ContentType = "application/x-www-form-urlencoded";
        req.ContentLength = strRequest.Length;

        //Send the request to PayPal and get the response
        StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
        streamOut.Write(strRequest);
        streamOut.Close();
        StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
        strResponse = streamIn.ReadToEnd().Trim();
        streamIn.Close();
    }

    private void ProcessPayment(string strRequest)
    {
        var censoredRequest = censorRequest(strRequest);

        if (string.Equals(censoredRequest["payment_status"], "completed", StringComparison.OrdinalIgnoreCase) &&
            PayPalAccountDetails.Exists(censoredRequest["receiver_email"]))
        {
            string commandName = censoredRequest["custom"];
            Assembly assembly = Assembly.GetAssembly(typeof(IIpnHandler));
            var type = assembly.GetType(commandName, true, true);
            IIpnHandler command = Activator.CreateInstance(type) as IIpnHandler;
            command.HandlePayPal(censoredRequest.ToString());
        }
    }

    private NameValuePairs censorRequest(string strRequest)
    {
        return NameValuePairs.Parse(strRequest);
    }

    public bool IsReusable { get { return false; } }
}