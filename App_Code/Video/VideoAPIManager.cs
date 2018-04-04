using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

/// <summary>
/// Returns non-parsed JSON Response from Video API
/// </summary>
public abstract class VideoAPIManager
{
    public abstract ResultPage Search(string text, int page, int limit, string culture);
    public abstract PlaylistResultPage SearchPlaylist(string text, int page, int limit, string culture);
    public abstract Video GetVideo(string id);
    public abstract Playlist GetPlaylist(string id, int page, int limit);
    public abstract IEnumerable<Video> GetRelatedVideos(string id, int limit, string culture);
    public abstract ResultPage GetDefaultPage(int page, int limit, string culture);

    protected string Get(string APIUrl, string query)
    {
        try
        {
            using (WebClient client = new MyWebClient())
            {
                return client.DownloadString(APIUrl + query);
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            return String.Empty;
        }
    }
}