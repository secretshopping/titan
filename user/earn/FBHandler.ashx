<%@ WebHandler Language="C#" Class="FBHandler" %>

using System;
using System.Web;

public class FBHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState {

    public void ProcessRequest(HttpContext context)
    {
        var accessToken = context.Request["accessToken"];
        context.Session["AccessToken"] = accessToken;

        context.Response.Cookies["fbcookie"].Value = "ok1";
        context.Response.Cookies["fbcookie"].Expires = DateTime.Now.AddMinutes(1);

        context.Response.Redirect("facebook.aspx");
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}