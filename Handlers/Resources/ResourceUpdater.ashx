<%@ WebHandler Language="C#" Class="ResourceUpdater" %>

using System;
using System.Web;

public class ResourceUpdater : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {



        context.Response.ContentType = "text/plain";
        context.Response.Write("Hello World");



    }
 



    public bool IsReusable {
        get {
            return false;
        }
    }

}