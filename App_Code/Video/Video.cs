using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Resources;
using System.Text;

public abstract class Video
{
    public abstract string ID { get; }

    public abstract string Title { get; }
    public abstract string Description { get; }

    public abstract int DurationInSeconds { get; }
    public abstract int Views { get; }

    public abstract bool HasAds { get; }

    public abstract string SmallThumbnailURL { get; }
    public abstract string BigThumbnailURL { get; }

    public abstract VideoPlayer Player { get; }

    #region ToString

    public string DurationToString()
    {
        int Minutes = Convert.ToInt32(Decimal.Floor((Decimal)DurationInSeconds / (Decimal)60));
        int SecondsLeft = DurationInSeconds - (60 * Minutes);

        string SecondsLeftString = SecondsLeft.ToString();
        if (SecondsLeft < 10)
            SecondsLeftString = "0" + SecondsLeft; // 4:4 -> 4:04, etc. (adding zero at beginning)

        return Minutes + ":" + SecondsLeftString;
    }

    public string ViewsToString()
    {
        return Views + " " + L1.VIEWS;
    }

    public string ToListHTML(bool IsRelated = false, string url = "", bool nowPlaying = false)
    {
         //<div class="video-link">
         //               <a href="#">
         //                   <img src="" />
         //                   <div class="video-link-title">Macklemore -Ryan</div>
         //                   <span class="video-link-time">4:23</span>
         //                   <br />
         //                   <span class="video-link-views">423423 views</span>
         //               </a>
         //</div>
        string linkCss = "col-md-3 image";
        //string hoverCss = "video-link-hover";

        string thumbnail = BigThumbnailURL;
        string description = Description.Replace("<br />", "");
        if (description.Length > 100)
            description = description.Substring(0, 99) + "...";

        if (IsRelated)
        {
            linkCss = "image video-link-related col-md-6 col-sm-6";
            //hoverCss = "video-link-hover-related";
            thumbnail = SmallThumbnailURL;
        }

        string videoUrl = "user/earn/video.aspx?v=" + ID;
        if (!string.IsNullOrEmpty(url))
            videoUrl = url;

        StringBuilder sb = new StringBuilder();
        sb.Append("<div class=\"")
          .Append(linkCss)
          .Append("\"><div class=\"image-inner\"><a href=\"")
          .Append(videoUrl)
          .Append("\">")
          .Append("<img src=\"")
          .Append(thumbnail)
          .Append("\" /></a><p class=\"image-caption\">")
          .Append(DurationToString())
          .Append("</p>");

        if (nowPlaying)
        {
            //Append "Now playing" for playlist videos
            sb.Append("<div class=\"playlist-nowplaying-label\">")
              .Append(U4000.NOWPLAYING)
              .Append("</div>");
        }

        sb.Append("</div><div class=\"image-info\" title=\"")
          .Append(Title.Replace("\"", "'"))
          .Append("\"><a href=\"")
          .Append(videoUrl)
          .Append("\"><h5 class=\"title\">")
          .Append(Title.Replace("\"", "'"))
          .Append("</h5></a><div class=\"desc\">")
          .Append(description.Replace("\"", "'"))
          .Append("</div><p class=\"views text-right\">")
          .Append(ViewsToString())
          .Append("</p></div></div>"); 

        return sb.ToString();
    }

    public string ToRelatedHTML()
    {
        return ToListHTML(true);
    }

    public string ToPlaylistHTML(string playlistId, int position, bool nowPlaying)
    {
        return ToListHTML(true, "user/earn/video.aspx?p=" + playlistId + "&o=" + position, nowPlaying);
    }

    #endregion
}