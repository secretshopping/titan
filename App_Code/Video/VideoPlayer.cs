using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Resources;

public abstract class VideoPlayer
{
    protected string id, title, description, views;

    public VideoPlayer(string id, string title, string description, string views)
    {
        this.id = id;
        this.title = title;
        this.description = description;
        this.views = views;
    }

    public abstract string ToHTML(int width, int height);

    public string ToHTMLWithInformation(int width, int height, int playlistPosition = 0, IEnumerable<Video> playlistDownloadedVideos = null, string playlistId = "",
        int playlistVideosCount = 0)
    {
        //<div class="player-video-title">
        //    Title 
        //</div>
        //<div class="player-video-description">
        //    Maskd asid aksdk asodk asdlas da sda s 
        //</div>
        //<div class="player-video-views">
        //    12 views 
        //</div>

        StringBuilder sb = new StringBuilder();
        sb.Append("<div class=\"player-video-title\">")
          .Append(title)
          .Append("</div><div class=\"player-video-description\">")
          .Append(description)
          .Append("</div>");



        sb.Append("<div class=\"player-video-views\">");

        if (playlistPosition != 0 && playlistDownloadedVideos != null)
        {
            //Playlist buttons
            string previousUrl = "user/earn/video.aspx?p=" + playlistId + "&o=" + (playlistPosition - 1);
            string nextUrl = "user/earn/video.aspx?p=" + playlistId + "&o=" + (playlistPosition + 1);
            string previousCss = "player-video-plbutton";
            string nextCss = "player-video-plbutton";
            string previousCode = "";
            string nextCode = "";

            if (playlistPosition == 1)
            {
                previousUrl = "#";
                previousCss = "player-video-grey";
                previousCode = " onclick=\"return false;\"";
            }

            if (playlistPosition == playlistVideosCount)
            {
                nextUrl = "#";
                nextCss = "player-video-grey";
                nextCode = " onclick=\"return false;\"";
            }

            sb.Append("<a href=\"" + previousUrl + "\" class=\"" + previousCss + "\" " + previousCode + " style=\"color:#fff\">")
              .Append(U4000.PREVIOUSVIDEO)
              .Append("</a><a href=\"" + nextUrl + "\" class=\"" + nextCss + "\" " + nextCode + " style=\"color:#fff\">")
              .Append(U4000.NEXTVIDEO)
              .Append("</a><br/>");
        }

        sb.Append(views)
        .Append("</div>");

        if (playlistPosition != 0 && playlistDownloadedVideos != null)
        {
            //Playlist videos
            sb.Append("<div id=\"player-playlist-videos-id\" class=\"player-playlist-videos\">");

            int ShowCount = 0;
            int FirstPosition = ((Playlist.GetPage(playlistPosition)-1) * Playlist.MaxDownloadedVideos) + 1;

            for (int i = FirstPosition; ShowCount < playlistDownloadedVideos.Count() && i <= playlistVideosCount;)
            {
                sb.Append(playlistDownloadedVideos.ElementAt<Video>(ShowCount).ToPlaylistHTML(playlistId, i, (playlistPosition-1) % Playlist.MaxDownloadedVideos == ShowCount));
                //sb.Append(playlistDownloadedVideos.ElementAt<Video>(ShowCount).ToPlaylistHTML(playlistId, i, false));
                i++;
                ShowCount++;
            }

            sb.Append("</div>");

            //

            if (playlistDownloadedVideos.Count<Video>() >= Playlist.MaxDownloadedVideos)
            sb.Append("<br/><br/><a href=\"#\" id=\"showVideosButton\" onclick=\"showAllPlaylist(); return false;\">Show more videos</a><span id=\"videosLoader\" style=\"display: none\"><i>Loading...</i></span>");
        }

        return this.ToHTML(width, height) + sb.ToString();
    }
}