using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Titan.News
{
    public class NewsManager
    {
        public const int FileSizeInMB = 25;

        public static string[] FileExtensions = new string[6] { ".mp4", ".flv", ".avi", ".mov", ".mpg", ".wmv" };

        public static bool CheckVideoExtension(string extension)
        {
            foreach (var FileExtension in FileExtensions)
                if (extension.Equals(FileExtension))
                    return true;

            return false;
        }

        public static int ImageMaxWidth { get { return ImagesHelper.News.ImageMaxWidth; } }
        public static int ImageImageMaxHeight { get { return ImagesHelper.News.ImageMaxHeight; } }
        public static int MaxFileSize { get { return FileSizeInMB * 1024 * 1024; } }

        /// <summary>
        /// Handles crediting for article reads for writers and influencers
        /// </summary>
        public static void CRON()
        {
            try
            {
                NewsSQLHelper.CreditWriters();
                NewsSQLHelper.CreditInfluencers();
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
        }


    }
}