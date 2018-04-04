<%@ WebHandler Language="C#" Class="CPABaseHandler" %>

using Titan;
using System.Web;

public class CPABaseHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        CPAHandler.ProcessRequest(context);
    }

    public bool IsReusable { get { return false; } }
}