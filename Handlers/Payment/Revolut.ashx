<%@ WebHandler Language="C#" Class="Revolut" %>

using System.Web;
using Prem.PTC.Payments;

public class Revolut : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        PaymentHandler handler = new RevolutHandler(context, PaymentProcessor.AdvCash);
        handler.Process();
    }

    public bool IsReusable { get { return false; } }
}