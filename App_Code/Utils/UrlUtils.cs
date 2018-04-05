namespace Prem.PTC.Utils
{
    public static class UrlUtils
    {
        public static string ConvertTildePathToImgPath(string TildePath)
        {
            string ImgPath = TildePath.Replace("~/", AppSettings.Site.Url)
                .Replace("~\\", AppSettings.Site.Url);

            return ImgPath;
        }
    }
}