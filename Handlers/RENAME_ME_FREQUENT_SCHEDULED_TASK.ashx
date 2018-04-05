<%@ WebHandler Language="C#" Class="CRONNED" %>

using System.Web;

public class CRONNED : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        CRON.ProceedFrequentDistribution();
    }

    public bool IsReusable { get { return false; } }
}