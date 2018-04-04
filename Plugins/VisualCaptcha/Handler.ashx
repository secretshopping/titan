<%@ WebHandler Language="C#" Class="CaptchaHandler" %>

using System;
using System.Web;
using VisualCaptcha;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

public class CaptchaHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    private static string GetSessionKey(HttpContext context)
    {
        return context.Request.Params["namespace"];
    }

    public void ProcessRequest(HttpContext context)
    {
        var args = context.Request.Path.Substring(context.Request.Path.IndexOf("ashx/") + 5).Split('/');
        string methodName = args[0];

        if (methodName == "start")
        {
            context.Response.Write(Start(Convert.ToInt32(args[1]), context));
        }
        else if (methodName == "image")
        {
            Image(Convert.ToInt32(args[1]), context, args.Length > 2 ? Convert.ToInt32(args[2]) : 0);
        }
        else if (methodName == "audio")
        {
            Audio(context, args.Length > 1 ? args[1] : "mp3");
        }
        else if (methodName == "try")
        {
            context.Response.Write(Try(args[1], context));
        }
    }

    public static string Start(int numberOfImages, HttpContext context)
    {
        var captcha = new Captcha(numberOfImages);
        context.Session[GetSessionKey(context)] = captcha;

        var frontEndData = captcha.GetFrontEndData();

        // Client side library requires lowercase property names
        return new JObject(
            new JProperty("values", frontEndData.Values),
            new JProperty("imageName", frontEndData.ImageName),
            new JProperty("imageFieldName", frontEndData.ImageFieldName),
            new JProperty("audioFieldName", frontEndData.AudioFieldName)
        ).ToString();
    }

    public void Image(int imageIndex, HttpContext context, int retina = 0)
    {
        var captcha = (Captcha)context.Session[GetSessionKey(context)];

        context.Response.ContentType = "image/png";
        //Write the generated file directly to the response stream
        context.Response.BinaryWrite(captcha.GetImage(imageIndex, retina == 1));
        context.Response.End();
    }

    public void Audio(HttpContext context, string type = "mp3")
    {
        var captcha = (Captcha)context.Session[GetSessionKey(context)];
        var content = captcha.GetAudio(type);

        var contentType = type == "mp3" ? "audio/mpeg" : "audio/ogg";

        context.Response.ContentType = contentType;
        //Write the generated file directly to the response stream
        context.Response.BinaryWrite(captcha.GetAudio(type));
        context.Response.End();
    }

    public string Try(string value, HttpContext context)
    {
        var captcha = (Captcha)context.Session[GetSessionKey(context)];
        var success = captcha.ValidateAnswer(value);
        var message = "Your answer was " + (success ? "valid." : "invalid.");

        return new JObject(
            new JProperty(success.ToString(), message)
        ).ToString();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}