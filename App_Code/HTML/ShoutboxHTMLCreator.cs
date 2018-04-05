using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using System.Text;
using Titan;

public class ShoutboxHTMLCreator
{
    public static string GetShoutboxMessageHTML(string Username, string Avatar, string CC, string Date, string Message, bool IsEvent, bool AvatarEnabled = true,
        bool ShowCountryFlag = true)
	{
        string MainCss = "sbentry media media-sm";

        if (IsEvent)
        {
            MainCss = "sbentryEvent media media-sm";
        }

        StringBuilder sb = new StringBuilder();

        sb.Append(@"<li class=""");
        sb.Append(MainCss);
        sb.Append(@""">");

        if (AvatarEnabled)
        {
            sb.Append(@"
                    <a class=""pull-left""><img src=""");
            sb.Append(Avatar);
            sb.Append(@""" class=""media-object rounded-corner shoutbox-image"" /></a>");
        }

        sb.Append(@"<div class=""media-body"">");
        sb.Append(@"<h5 class=""media-heading p-r-10"">");
        sb.Append(@"<span class=""small shoutbox-date"">");
        sb.Append(Date);
        sb.Append(@"</span>");
        sb.Append(Username);
        if (ShowCountryFlag)
        {
            sb.Append(@"
                    <img src=""");
            sb.Append("Images/Flags/");
            sb.Append(CC);
            sb.Append(@".png"" class=""pull-right shoutbox_flag"" title=""");
            sb.Append(CC.ToUpper());
            sb.Append(@"""/>");
        }
        sb.Append("</h5>");
        
        sb.Append(@"<p>");
        sb.Append(Message);
        sb.Append("</p>");
        sb.Append("</div>");
        sb.Append("</li>");
        

        

        return sb.ToString();
	}
}