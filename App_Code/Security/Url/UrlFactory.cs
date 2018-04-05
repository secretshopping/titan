using System;
public class UrlFactory
{
    Url _instance;
    string _url;
    public UrlFactory(string url)
    {
        _url = url;
    }
    public Url CreateInstance()
    {
        try
        {
            using (MyWebClient webclient = new MyWebClient())
            {
                webclient.DownloadString(_url);
                Uri uri = webclient.ResponseUri;

                if (uri.Host.StartsWith("www.youtube.com"))
                    _instance = new YouTubeUrl(uri);
                else if (uri.Host.StartsWith("www.dailymotion.com"))
                    _instance = new DailyMotionUrl(uri);
                else
                    _instance = new Url(_url);
            }

            return _instance;
        }
        catch (Exception ex)
        {
            //Unable to access url
            return new Url(_url);
        }
    }
}