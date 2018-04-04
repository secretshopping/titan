using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using Prem.PTC;
using Prem.PTC.Members;
using System.Text;
using Titan;

public partial class EarnSearch : System.Web.UI.Page
{
    //Settings
    protected static int LimitPerPage = 20;
    protected static int LimitPerRelated = 5;

    protected static int FirstPlaylistAfterVideos = 4; //To keep DailyMotion style
    protected static int SecondPlaylistAfterVideos = 12; //To keep DailyMotion style

    //Viewstate
    protected static string PageViewState = "Viewstate_CurrentPage";
    protected static string QueryViewState = "Viewstate_Query1";
    protected static string CurrentVideoViewState = "Viewstate_CurrentVideo";

    protected VideoAPIManager Manager;

    protected void Page_Load(object sender, EventArgs e)
    {
        AppSettings.SearchAndVideo.Reload();

        //Turned off
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.EarnersRoleEnabled && 
            AppSettings.TitanFeatures.EarnVideoEnabled
            && (Member.IsLogged || Member.CurrentInCache.IsEarner));

        if (AppSettings.SearchAndVideo.VideoWidgetEnabled)
        {
            WidgetPlaceHolder.Visible = true;
            SearchPanel.Visible = false;
            WidgetCodeLiteral.Text = AppSettings.SearchAndVideo.VideoWidgetCode;
            DescriptionLiteral.Text = U6013.WATCHVIDEOSANDHAVEFUN;
        }
        else
        {
            DefaultViewPlaceHolder.Visible = true;
            DescriptionLiteral.Text = U4000.VIDEOINFO;

            //Only one manager (DailyMotion) is currently available
            Manager = new DailyMotionAPIManager();

            if (!Page.IsPostBack)
            {
                if (Request.QueryString["q"] != null)
                {
                    Query = HttpUtility.UrlDecode(Request.QueryString["q"]);
                    SearchBox.Text = Query;
                }
                else
                    Query = String.Empty;

                try
                {
                    if (Request.QueryString["v"] != null)
                    {
                        string id = Request.QueryString["v"].Replace("'", "''");
                        try
                        {
                            //Display VideoPlayer
                            var video = Manager.GetVideo(id);
                            var related = Manager.GetRelatedVideos(id, LimitPerRelated, CurrentCulture);
                            DisplayPlayerPage(video, related);
                        }
                        catch (Exception ex)
                        {
                            ErrorLogger.Log(ex);
                        }
                    }
                    else if (Request.QueryString["p"] != null && Request.QueryString["o"] != null)
                    {
                        string id = Request.QueryString["p"].Replace("'", "''");
                        int index = Convert.ToInt32(Request.QueryString["o"]);

                        try
                        {
                            //Display VideoPlayer with PlayList buttons & videos
                            var playlist = Manager.GetPlaylist(id, Playlist.GetPage(index), Playlist.MaxDownloadedVideos);
                            var video = playlist.Videos.ElementAt<Video>((index - 1) % Playlist.MaxDownloadedVideos);
                            var related = Manager.GetRelatedVideos(video.ID, LimitPerRelated, CurrentCulture);
                            DisplayPlayerPage(video, related, index, playlist.Videos, playlist.ID, playlist.TotalVideosCount);
                        }
                        catch (Exception ex)
                        {
                            ErrorLogger.Log(ex);
                        }
                    }
                    else
                    {
                        DisplayResultPage();
                    }
                }
                catch (MsgException ex)
                {
                    ErrorLogger.Log(ex);
                    QueryResultsPlaceHolder.Visible = true;
                    VideoPlayerPlaceHolder.Visible = false;
                    QueryResultsVideosLiteral.Text = "<i>" + U4000.ERRORAPI + " DailyMotion API. " + U4000.TRYAGAIN + "...</i>";
                }
            }
        }
        Button2.Visible = TitanFeatures.IsRewardStacker;
        ScriptManager.RegisterStartupScript(this.UpdatePanel1, this.GetType(), "EndRequestHandlerForVideo", "EndRequestHandlerForVideo();", true);
    }

    #region Getters & setters

    public Int32 CurrentPage
    {
        get
        {
            if (ViewState[PageViewState] == null)
                CurrentPage = 1;

            return (int)ViewState[PageViewState];
        }
        set
        {
            ViewState[PageViewState] = value;
        }
    }

    public static String Query
    {
        get
        {
            if (HttpContext.Current.Session[QueryViewState] == null)
                Query = "";
            return (string)HttpContext.Current.Session[QueryViewState];
        }
        set
        {
            HttpContext.Current.Session[QueryViewState] = value;
        }
    }

    public static string CurrentCulture
    {
        get
        {
            string cc = CountryManager.LookupCountryCode(IP.Current).ToLower();
            if (cc == "-")
                return "US";
            return cc;
        }
    }

    public string CurrentVideoID
    {
        get
        {
            return (string)HttpContext.Current.Session[CurrentVideoViewState];
        }
        set
        {
            HttpContext.Current.Session[CurrentVideoViewState] = value;
        }
    }

    #endregion

    #region Playlist videos

    [System.Web.Services.WebMethod]
    public static string GetAllVideos(int page, string pid, int index)
    {
        return GetAllVideosData(page, pid, index);
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        if (viewIndex == 0)
            Response.Redirect("video.aspx");

        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
            b.CssClass = "";

        TheButton.CssClass = "ViewSelected";
    }

    public static string GetAllVideosData(int page, string id, int index)
    {
        bool HasMoreVideos = true;
        StringBuilder sb = new StringBuilder();
        DailyMotionAPIManager Manager = new DailyMotionAPIManager();

        page = Playlist.GetPage(index) + page - 1;

        try
        {
            var playlist = Manager.GetPlaylist(id, page, Playlist.MaxDownloadedVideos);

            for (int i = 0; i < playlist.Videos.Count(); i++)
            {
                if (playlist.TotalVideosCount <= (Playlist.MaxDownloadedVideos * (page - 1)) + 1 + i)
                    HasMoreVideos = false;

                sb.Append(playlist.Videos.ElementAt<Video>(i).ToPlaylistHTML(id, (Playlist.MaxDownloadedVideos * (page - 1)) + 1 + i, false));
            }

        }
        catch (Exception ex) { ErrorLogger.Log(ex); }

        if (HasMoreVideos == false)
            return "<!--NOMORE-->" + sb.ToString();

        return sb.ToString();
    }

    #endregion

    #region Infinite browse

    public static string GetCustomersData(int pageIndex)
    {
        ResultPage result = null;
        DailyMotionAPIManager Manager = new DailyMotionAPIManager();

        if (string.IsNullOrWhiteSpace(Query))
            result = Manager.GetDefaultPage(pageIndex, LimitPerPage, CurrentCulture); //No query, display default page
        else
            result = Manager.Search(Query, pageIndex, LimitPerPage, CurrentCulture);

        StringBuilder sb = new StringBuilder();
        foreach (var video in result.Videos)
            sb.Append(video.ToListHTML());

        return sb.ToString();
    }

    [System.Web.Services.WebMethod]
    public static string GetCustomers(int pageIndex)
    {
        return GetCustomersData(pageIndex);
    }


    #endregion

    #region Displayers

    protected void DisplayResultPage()
    {
        ResultPage result;
        PlaylistResultPage PlaylistResult = null;

        if (string.IsNullOrWhiteSpace(Query))
            result = Manager.GetDefaultPage(CurrentPage, LimitPerPage, CurrentCulture); //No query, display default page
        else
        {
            result = Manager.Search(Query, CurrentPage, LimitPerPage, CurrentCulture);
            PlaylistResult = Manager.SearchPlaylist(Query, 1, 2, CurrentCulture);
        }

        DisplayResultPage(result, PlaylistResult);
    }

    protected void DisplayResultPage(ResultPage page, PlaylistResultPage playlists)
    {
        //Placeholders
        QueryResultsPlaceHolder.Visible = true;
        VideoPlayerPlaceHolder.Visible = false;
        StringBuilder sb = new StringBuilder();
        sb.Append("<div class='row'>");
        for (int i = 0; i < page.Videos.Count<Video>(); i++)
        {
            //Append playlist 1
            if (i == FirstPlaylistAfterVideos && playlists != null && playlists.Playlists.Count<Playlist>() > 0)
                sb.Append(playlists.Playlists.ElementAt<Playlist>(0).ToListHTML());

            //Append playlist 2
            if (i == SecondPlaylistAfterVideos && playlists != null && playlists.Playlists.Count<Playlist>() > 1)
                sb.Append(playlists.Playlists.ElementAt<Playlist>(1).ToListHTML());

            sb.Append(page.Videos.ElementAt<Video>(i).ToListHTML());
            
            if((i+1)%4==0)
            {
                sb.Append("</div><div class='row'>");
            }
        }
        sb.Append("</div>");

        QueryResultsVideosLiteral.Text = sb.ToString();
    }

    protected void DisplayPlayerPage(Video video, IEnumerable<Video> relatedVideos, int playlistIndex = -1, IEnumerable<Video> playlistDownloadedVideos = null
        , string playlistId = "", int playlistVideosCount = 0)
    {
        //Placeholders
        QueryResultsPlaceHolder.Visible = false;
        VideoPlayerPlaceHolder.Visible = true;

        //Display player
        if (playlistIndex != -1)
            VideoPlayerLiteral.Text = video.Player.ToHTMLWithInformation(510, 315, playlistIndex, playlistDownloadedVideos, playlistId, playlistVideosCount); //Playlist
        else
            VideoPlayerLiteral.Text = video.Player.ToHTMLWithInformation(510, 315);

        //Display related videos
        StringBuilder sb = new StringBuilder();
        foreach (var relatedVideo in relatedVideos)
        {
            sb.Append(relatedVideo.ToRelatedHTML());
        }
        RelatedVideosLiteral.Text = sb.ToString();
    }

    #endregion

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        string text = SearchBox.Text.Trim();
        if (!string.IsNullOrWhiteSpace(text))
        {
            text = HttpUtility.UrlEncode(text);
            Response.Redirect("~/user/earn/video.aspx?q=" + text);
        }
    }

    #region AJAX handlers

    protected void ajaxPostbackVideoTrigger_Click(object sender, EventArgs e)
    {
        //AJAX triggered
        //We should credit the member and refresh balance
        try
        {
            if (!Member.IsLogged)
                throw new MsgException(U4000.YOUMUSTBELOGGED.Replace("%p%", AppSettings.PointsName));

            Member User = Member.Current;
            VideoCrediter Crediter = new VideoCrediter(User);
            Crediter.CreditVideo();

            //Refresh Points panel
            UpdatedPointsTextBox.Text = User.PointsBalance.ToString();

        }
        catch (MsgException ex)
        {
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = ex.Message;
        }
    }

    protected void ajaxPostbackVideoTriggerEnded_Click(object sender, EventArgs e)
    {
        //AJAX triggered
        //We should move to the next video of playlist (IF playlist)
        try
        {
            if (Request.QueryString["p"] != null && Request.QueryString["o"] != null)
            {
                string id = Request.QueryString["p"].Replace("'", "''");
                int index = Convert.ToInt32(Request.QueryString["o"]);
                var playlist = Manager.GetPlaylist(id, Playlist.GetPage(index), Playlist.MaxDownloadedVideos);

                if (index < playlist.TotalVideosCount)
                    Response.Redirect("~/user/earn/video.aspx?p=" + id + "&o=" + (index + 1));
            }

        }
        catch (Exception ex)
        { }
    }

    #endregion

}
