<%@ WebHandler Language="C#" Class="LocalBitcoinsAshx" %>

using System.Web;
using Prem.PTC.Payments;

public class LocalBitcoinsAshx : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        LocalBitcoinsHandler handler = new LocalBitcoinsHandler(context, PaymentProcessor.LocalBitcoins);
        handler.Process();
    }

    public bool IsReusable { get { return false; } }
}