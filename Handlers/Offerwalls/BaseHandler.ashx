<%@ WebHandler Language="C#" Class="BaseHandler" %>

using Titan;
using System.Web;

public class BaseHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        OfferwallHandler.ProcessRequest(context);
    }

    public bool IsReusable { get { return false; } }
}