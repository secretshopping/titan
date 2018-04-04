<%@ WebHandler Language="C#" Class="FacebookConnect" %>

using System;
using System.Web;
using Prem.PTC.Members;
public class FacebookConnect : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        string accessToken = context.Request.Params["accessToken"] as string;
        if (!string.IsNullOrEmpty(accessToken))
        {
            FacebookMember fbUser = new FacebookMember(accessToken);
            var user = Member.CurrentInCache;
                user.ConnectWithFacebook(fbUser);
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}