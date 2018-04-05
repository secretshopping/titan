<%@ WebHandler Language="C#" Class="Blockchain" %>

using Titan;
using System.Web;
using Prem.PTC;

public class Blockchain : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/css";
        context.Response.Write(CSS);
    }

    public bool IsReusable { get { return false; } }

    public string DarkColor = AppSettings.Site.DarkColor;
    public string MainColor = AppSettings.Site.MainColor;
    public string LightColor = AppSettings.Site.LightColor;

    public string CSS
    {
        get
        {
            return @"

                    .timer > #slice > .pie {
                        border-color: " + MainColor + @";
                    }

                    .timer.fill > #slice > .pie {
                        background-color: " + MainColor + @";
                    }

                    a, a:link, a:visited {
                        color: " + MainColor + @";
                    }

                    .Abox, .Aboxclicked, .HAbox, .HAboxclicked {
                        border: 1px solid " + MainColor + @";
                    }

                    .CashLinkTitle {
                        color: " + MainColor + @";
                    }

                    .surfThumbnailBeingWatched {
                        background-color: " + MainColor + @";
                        border: 1px solid " + DarkColor + @"
                    }

                     .Abox:hover, .HAbox, .Aboxclicked:hover {
                        border-color: " + DarkColor + @";
                    }

                    ul.menu a.selected, ul.menu a:hover,
                    ul.menu a:hover,
                    #links a:hover,
                    .news {
                        color: " + MainColor + @";
                    }

                    #flags input:hover {
                        background-color: " + MainColor + @";
                    }

                    .rednumber, .Frednumber {
                        background-color:" + LightColor + @";
                        border: 1px solid" + DarkColor + @";
                    }

                    .gridtable tr th {
                        border: 1px solid " + DarkColor + @";
                        background: " + MainColor + @";
                    }

                    a.replybutton, .rbutton, .replybutton {
                        background-color: " + MainColor + @";
                    }

                        .rbutton:hover, .replybutton:hover {
                            background-color: " + MainColor + @";
                        }

                    .whitebox {
                        border: 1px solid " + DarkColor + @";
                        color: " + MainColor + @";
                    }

                    .regular-checkbox + label,
                    .regular-checkbox:checked + label {
                        border: 1px solid " + MainColor + @";
                    }

                        .regular-checkbox:checked + label:after {
                            color: " + MainColor + @";
                        }

                    .traffic-grid .traffic-cell:hover {
                        background-color: " + MainColor + @";
                    }

                    .shoutbox_button {
                        border: 1px solid " + DarkColor + @";
                        background-color: " + MainColor + @";
                    }

                    .shoutbox_username {
                        color: " + MainColor + @";
                    }

                    .greenbutton {
                        border: 0px solid " + DarkColor + @";
                        background: " + MainColor + @";
                    }

                    .onoffswitch-inner:before,
                    .onoffswitch2-inner:before {
                        background-color: " + MainColor + @";
                    }

                    .ABtitle, .ABtitleclicked {
                        background: " + MainColor + @";
                    }

                    .ABpole {
                        background-color: " + MainColor + @";
                    }";
        }
    }
}

