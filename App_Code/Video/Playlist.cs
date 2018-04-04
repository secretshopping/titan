using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Resources;
using System.Text;

public abstract class Playlist
{
    public static int MaxDownloadedVideos = 6;

    public abstract string ID { get; }

    public abstract string Title { get; }
    public abstract string Description { get; }

    public abstract string SmallThumbnailURL { get; }
    public abstract string BigThumbnailURL { get; }

    public abstract int TotalVideosCount { get; }
    public abstract IEnumerable<Video> Videos { get; set; }

    #region ToString

    public string ToListHTML(bool IsRelated = false)
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
        string linkCss = "video-link";
        string thumbnail = BigThumbnailURL;
        if (IsRelated)
        {
            linkCss = "video-link-related";
            thumbnail = SmallThumbnailURL;
        }

        StringBuilder sb = new StringBuilder();
        sb.Append("<div class=\"")
          .Append(linkCss)
          .Append("\"><a href=\"user/earn/video.aspx?p=")
          .Append(ID)
          .Append("&o=1")
          .Append("\"><div class=\"playlist-label\">")
          .Append(TotalVideosCount)
          .Append(" ")
          .Append(U4000.VIDEOS)
          .Append("</div><div class=\"playlist-link-hover\"><img src=\"Images/play_all.png\" /></div><div class=\"darken\"><img src=\"")
          .Append(thumbnail)
          .Append("\" /></div><div class=\"video-tooltip\" title=\"")
          .Append(Title.Replace("\"", "'"))
          .Append("<br/><span style='color:#868686'>")
          .Append("")
          .Append("</span>\"><div class=\"video-link-title\">")
          .Append(Title.Replace("\"", "'"))
          .Append("</div><span class=\"video-link-time\">")
          .Append("")
          .Append("</span></div><span class=\"video-link-views\">")
          .Append(U4000.PLAYLIST)
          .Append("</span></a></div>");
        return sb.ToString();
    }

    public string ToRelatedHTML()
    {
        return ToListHTML(true);
    }

    public static int GetPage(int position)
    {
        position = position - 1;
        return Convert.ToInt32(Decimal.Floor((Decimal)position / (Decimal)MaxDownloadedVideos)) + 1;
    }

    #endregion
}