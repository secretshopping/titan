using Prem.PTC;
using System;
using System.Web;

namespace Titan.MiniVideos
{
    public enum MiniVideoStatus
    {
        Null = 0,

        Active = 1,
        Paused = 2,
        Deleted = 3,
        Finished = 4
    }

    public class MiniVideoManager
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

        public static int VideoImageMaxWidth
        {
            get
            {
                return ImagesHelper.MiniVideo.VideoImageMaxWidth;
            }
        }

        public static int VideoImageImageMaxHeight
        {
            get
            {
                return ImagesHelper.MiniVideo.VideoImageMaxHeight;
            }
        }

        public static int MaxFileSize
        {
            get
            {
                return FileSizeInMB * 1024 * 1024;
            }
        }

        public static void CRON()
        {
            try
            {
                var selectVideosQuery = string.Format("SELECT * FROM {0} WHERE [AddedDate] < '{1}'",
                                          MiniVideoCampaign.TableName, DateTime.Now.AddDays(-AppSettings.MiniVideo.MiniVideoKeepDays));
                var deleteUsersVideoQuery = string.Format("DELETE FROM {0} WHERE [BoughtDate] < '{1}'",
                                          UsersMiniVideoCampaign.TableName, DateTime.Now.AddDays(-AppSettings.MiniVideo.MiniVideoKeepForUserDays));
                var deleteGeneralVideoQuery = string.Format("DELETE FROM {0} WHERE [AddedDate] < '{1}'",
                                          MiniVideoCampaign.TableName, DateTime.Now.AddDays(-AppSettings.MiniVideo.MiniVideoKeepDays));

                var filesList = TableHelper.GetListFromRawQuery<MiniVideoCampaign>(selectVideosQuery);

                foreach (var file in filesList)
                {
                    var videoFile = HttpContext.Current.Server.MapPath(file.VideoURL);
                    if (System.IO.File.Exists(videoFile))
                        System.IO.File.Delete(videoFile);

                    var imageFile = HttpContext.Current.Server.MapPath(file.ImageURL);
                    if (System.IO.File.Exists(imageFile))
                        System.IO.File.Delete(imageFile);
                }

                TableHelper.ExecuteRawCommandNonQuery(deleteUsersVideoQuery);
                TableHelper.ExecuteRawCommandNonQuery(deleteGeneralVideoQuery);
            }
            catch (Exception ex) { }
        }
    }
}