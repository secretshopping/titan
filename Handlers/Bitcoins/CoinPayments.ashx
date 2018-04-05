<%@ WebHandler Language="C#" Class="CoinPayments" %>

using Prem.PTC.Payments;
using System.Web;

public class CoinPayments : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        CoinPaymentsHandler handler = new CoinPaymentsHandler(context, PaymentProcessor.CoinPayments);
        handler.Process();
    }

    public bool IsReusable { get { return false; } }
}