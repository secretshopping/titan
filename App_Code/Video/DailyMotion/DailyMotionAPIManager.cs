using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

/// <summary>
/// Returns non-parsed JSON Response from DailyMotion API
/// </summary>
public class DailyMotionAPIManager : VideoAPIManager
{
    protected string APIUrl = "https://api.dailymotion.com";

    #region Query

    protected string SearchForVideosJSON(string text, int page, int limit, string culture)
    {
        string query = "/videos?fields=id,ads,description,title,views_total,duration,thumbnail_120_url,thumbnail_360_url,&localization=" + culture + "&search=" + text + "&page=" + page + "&limit=" + limit;
        return base.Get(APIUrl, query);
    }

    protected string SearchForPlaylistsJSON(string text, int page, int limit, string culture)
    {
        string query = "/playlists?fields=id,name,description,videos_total,thumbnail_120_url,thumbnail_360_url,&localization=" + culture + "&search=" + text + "&page=" + page + "&sort=relevance&limit=" + limit;
        return base.Get(APIUrl, query);
    }

    protected string GetDefaultPageJSON(int page, int limit, string culture)
    {
        string query = "/videos?fields=id,ads,description,title,views_total,duration,thumbnail_120_url,thumbnail_360_url,&localization=" + culture + "&flags=featured&page=" + page + "&limit=" + limit;
        return base.Get(APIUrl, query);
    }

    protected string GetVideoJSON(string id)
    {
        string query = "/video/" + id + "?fields=id,ads,description,title,views_total,duration,thumbnail_120_url,thumbnail_360_url,";
        return base.Get(APIUrl, query);
    }

    protected string GetPlaylistInfoJSON(string id)
    {
        string query = "/playlist/" + id + "?fields=id,name,description,videos_total,thumbnail_120_url,thumbnail_360_url,";
        return base.Get(APIUrl, query);
    }

    protected string GetPlaylistVideosJSON(string id, int limit, int page)
    {
        string query = "/playlist/" + id + "/videos?fields=id,ads,description,title,views_total,duration,thumbnail_120_url,thumbnail_360_url,&page=" + page + "&limit=" + limit;
        return base.Get(APIUrl, query);
    }

    protected string GetRelatedVideosJSON(string id, int limit, string culture)
    {
        string query = "/video/" + id + "/related?fields=id,ads,description,title,views_total,duration,thumbnail_120_url,thumbnail_360_url,&localization=" + culture + "&limit=" + limit;
        return base.Get(APIUrl, query);
    }

    #endregion

    public override ResultPage Search(string text, int page, int limit, string culture)
    {
        return JsonConvert.DeserializeObject<DailyMotionResultPage>(SearchForVideosJSON(HttpUtility.UrlEncode(text), page, limit, culture));
    }

    public override PlaylistResultPage SearchPlaylist(string text, int page, int limit, string culture)
    {
        return JsonConvert.DeserializeObject<DailyMotionPlaylistResultPage>(SearchForPlaylistsJSON(HttpUtility.UrlEncode(text), page, limit, culture));
    }

    public override ResultPage GetDefaultPage(int page, int limit, string culture)
    {
        return JsonConvert.DeserializeObject<DailyMotionResultPage>(GetDefaultPageJSON(page, limit, culture));
    }

    public override Video GetVideo(string id)
    {
        return JsonConvert.DeserializeObject<DailyMotionVideo>(GetVideoJSON(id));
    }

    public override Playlist GetPlaylist(string id, int page, int limit)
    {
        var playlist = JsonConvert.DeserializeObject<DailyMotionPlaylist>(GetPlaylistInfoJSON(id));
        DailyMotionResultPage pageResult = JsonConvert.DeserializeObject<DailyMotionResultPage>(GetPlaylistVideosJSON(id, limit, page));
        playlist.Videos = pageResult.Videos;

        return playlist;
    }

    public override IEnumerable<Video> GetRelatedVideos(string id, int limit, string culture)
    {
        DailyMotionResultPage page = JsonConvert.DeserializeObject<DailyMotionResultPage>(GetRelatedVideosJSON(id, limit, culture));
        return page.Videos;
    }
}