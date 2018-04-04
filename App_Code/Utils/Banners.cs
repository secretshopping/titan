using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Prem.PTC.Utils
{
    /// <summary>
    /// Class with simple utilities for banners.
    /// </summary>
    public static class Banners
    {
        /// <summary>
        /// Checks if stream is a valid image and if so, returns image.
        /// </summary>
        /// <param name="stream">Stream (for example from uploadfile control)</param>
        /// <returns></returns>
        public static bool TryParse(Stream stream, out Image image)
        {
            try { image = Image.FromStream(stream); }
            catch (ArgumentException)
            {
                image = null;
                return false;
            }

            return true;
        }

        public static bool HasValidFormat(this Image image)
        {
            try { GetExtension(image); }
            catch (InvalidDataException) { return false; }
            return true;
        }

        public static string GetExtension(this Image image)
        {
            if (image.RawFormat.Guid == ImageFormat.Png.Guid) return ".png";
            if (image.RawFormat.Guid == ImageFormat.Gif.Guid) return ".gif";
            if (image.RawFormat.Guid == ImageFormat.Jpeg.Guid) return ".jpg";

            throw new InvalidDataException();
        }

        public static bool TryCreateBannerFromUrl(string url, out Banner banner)
        {
            banner = null;
            if (string.IsNullOrEmpty(url)) return false;
            try
            {
                byte[] imageData;
                using (var wc = new WebClient())
                    imageData = wc.DownloadData(url);

                Banner.TryFromStream(new MemoryStream(imageData), out banner);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}