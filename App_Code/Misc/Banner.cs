using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Titan;

namespace Prem.PTC
{
    /// <summary>
    /// New banner class.
    /// </summary>

    [Serializable]
    public class Banner
    {
        public string Path { get; private set; }
        public string MappedPath { get { return Utils.FileUtils.MapPath(Path); } }

        public int Width { get { return _image != null ? _image.Width : 0; } }
        public int Height { get { return _image != null ? _image.Height : 0; } }

        /// <summary>
        /// Whether image file is saved in filesystem.
        /// </summary>
        public bool IsSaved
        {
            get
            {
                return String.IsNullOrWhiteSpace(Path) ? false : File.Exists(MappedPath);
            }
        }

        public Image _image;

        private Banner(Image image, string path = "")
        {
            _image = image;
            Path = path;
        }

        /// <summary>
        /// Represents empty banner with no image set.
        /// </summary>
        public static Banner Empty
        {
            get { return new Banner(null); }
        }

        /// <exception cref="ArgumentNullException"/>
        public static Banner FromImage(Image image)
        {
            if (image == null)
                throw new ArgumentNullException();

            return new Banner(image.Clone() as Image);
        }

        /// <exception cref="ArgumentException"/>
        public static Banner FromStream(Stream stream)
        {
            Image image;

            if (Prem.PTC.Utils.Banners.TryParse(stream, out image) &&
                Prem.PTC.Utils.Banners.HasValidFormat(image))
                return new Banner(image);

            else throw new ArgumentException("Invalid stream");
        }

        /// <exception cref="FileNotFoundException" />
        /// <exception cref="ArgumentException" />
        /// <exception cref="OutOfMemoryException" />
        public static Banner FromFile(string directoryPath, string imageName)
        {
            if (!directoryPath.EndsWith(System.IO.Path.DirectorySeparatorChar + ""))
                directoryPath += System.IO.Path.DirectorySeparatorChar;

            return FromFile(directoryPath + imageName);
        }

        /// <exception cref="FileNotFoundException" />
        /// <exception cref="ArgumentException" />
        /// <exception cref="OutOfMemoryException" />
        public static Banner FromFile(string imagePath)
        {
            Image image = Image.FromFile(Utils.FileUtils.MapPath(imagePath));

            return new Banner(image, imagePath);
        }

        public static bool TryFromStream(Stream stream, out Banner banner)
        {
            try { banner = FromStream(stream); }
            catch (Exception)
            {
                banner = null;
                return false;
            }

            return true;
        }

        private byte[] GetImageBytes(ImageFormat ImageFormat)
        {
            using (var ms = new MemoryStream())
            {
                _image.Save(ms, ImageFormat);
                return ms.ToArray();
            }
        }

        private ImageFormat GetImageFormat()
        {
            return _image.RawFormat;
        }

        public static string GetTemporaryBannerPath(Banner TemporaryBanner)
        {
            return @String.Format("data:image/png;base64,{0}",
                Convert.ToBase64String(TemporaryBanner.GetImageBytes(TemporaryBanner.GetImageFormat())));
        }


        /// <summary>
        /// Saves image under specified directory path and with specified filename.
        /// </summary>
        /// <param name="directoryPath">Path to directory where image file should be saved.</param>
        /// <param name="imageName">Suggested image name. Remember to include valid file extension.</param>
        /// <param name="forceSave">When set as false and this banner IsSaved exception is thrown. 
        /// When set as true attempt to override file is made, however not guaranteed.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidStateException"/>
        /// <exception cref="InvalidOperationException"/>
        public void Save(string directoryPath, string imageName, bool forceSave = false)
        {
            if (!forceSave && IsSaved)
                throw new InvalidOperationException("Cannot save image.");

            directoryPath = directoryPath.Replace("/", System.IO.Path.DirectorySeparatorChar + "");

            string mappedDirectoryPath = "";

            if (!Directory.Exists(directoryPath))
                mappedDirectoryPath = Utils.FileUtils.MapPath(directoryPath);

            if (!Directory.Exists(mappedDirectoryPath))
                throw new ArgumentException("Not a directory: " + directoryPath);

            if (!directoryPath.EndsWith(System.IO.Path.DirectorySeparatorChar + ""))
                directoryPath += System.IO.Path.DirectorySeparatorChar;

            string _path = directoryPath + imageName;

            _image.Save(Utils.FileUtils.MapPath(_path));

            Path = _path;
        }

        public void SaveTildaPath(string directoryPath, bool forceSave = false)
        {
            string imageName =
                DateTime.UtcNow.Ticks +
                Prem.PTC.Utils.Banners.GetExtension(_image);

            if (!forceSave && IsSaved)
                throw new InvalidOperationException("Cannot save image.");

            //directoryPath = directoryPath.Replace("/", System.IO.Path.DirectorySeparatorChar + "");

            string mappedDirectoryPath = "";

            if (!Directory.Exists(directoryPath))
                mappedDirectoryPath = Utils.FileUtils.MapPath(directoryPath);

            if (!Directory.Exists(mappedDirectoryPath))
                throw new ArgumentException("Not a directory: " + directoryPath);

            //if (!directoryPath.EndsWith(System.IO.Path.DirectorySeparatorChar + ""))
            //    directoryPath += System.IO.Path.DirectorySeparatorChar;

            string _path = directoryPath + imageName;

            //_image.Save(Utils.FileUtils.MapPath(_path));

            Path = _path;
        }


        /// <summary>
        /// Saves image under specified directory path. File name is generated automatically.
        /// </summary>
        /// <param name="directoryPath">Path to directory where image file should be saved.</param>
        /// <param name="forceSave">When set as false and this banner IsSaved exception is thrown. 
        /// When set as true attempt to override file is made, however not guaranteed.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidStateException"/>
        /// <exception cref="InvalidOperationException"/>
        public Banner Save(string directoryPath, bool forceSave = false)
        {
            string imageName =
                DateTime.UtcNow.Ticks +
                Prem.PTC.Utils.Banners.GetExtension(_image);
 
            Save(directoryPath, imageName, forceSave);
            return this;
        }


        /// <summary>
        /// Saves image under specific path
        /// </summary>
        /// <param name="directoryPath">Path to save</param>
        /// <param name="imageName">Image name (no extension)</param>
        public void Save(string directoryPath, string imageName)
        {
            this.Save(directoryPath, imageName + Prem.PTC.Utils.Banners.GetExtension(_image), true);
        }

        /// <summary>
        /// Deletes image saved previously in Banner.ImagePath path
        /// If image doesn't exist before deleting: does nothing.
        /// Object shouln't be reused after this method call.
        /// </summary>
        /// <exception cref="IOException" />
        /// <exception cref="NotSupportedException" />
        /// <exception cref="UnauthorizesAccessException" />
        public void Delete()
        {
            if (IsSaved)
            {
                File.Delete(MappedPath);
                Path = string.Empty;
                _image.Dispose();
            }
        }

        private string Serialize()
        {
            string base64String;

            
            using (var ms = new MemoryStream())
            {
                try
                {
                    _image.Save(ms, _image.RawFormat);
                }
                catch
                {
                    var bmp = new Bitmap(_image);
                    bmp.Save(ms, _image.RawFormat);
                }
                byte[] imageBytes = ms.ToArray();
                base64String = Convert.ToBase64String(imageBytes);
            }

            return base64String;
        }

        public static Banner Deserialize(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            return new Banner(Image.FromStream(ms, true));
        }

        //Method return MsgException
        public void SaveOnClient(string fileName, string filePath = "~/Images/b_ads/")
        {
            if (_image == null)
                throw new ArgumentNullException();

            using (var client = new WebClient())
            {
                var values = new NameValueCollection();
                values["p"] = HashingManager.SHA256(AppSettings.Offerwalls.UniversalHandlerPassword);
                values["fn"] = HttpUtility.UrlEncode(fileName);
                values["fp"] = HttpUtility.UrlEncode(filePath);
                values["h"] = HttpUtility.UrlEncode(Serialize());

                //Send request to proper handler
                string urlPost = AppSettings.Site.Url + "Handlers/ImageCreator.ashx";

                try
                {
                    var response = client.UploadValues(urlPost, values);
                    string responseString = Encoding.Default.GetString(response);

                    if (responseString != OfferwallFileManager.RESPONSE_OK_CODE)
                        throw new MsgException(responseString);

                    Path = filePath + fileName;
                }
                catch (Exception ex)
                {
                    throw new MsgException(ex.Message);
                }
            }
        }

        public void SaveOnClient(string filePath = "~/Images/b_ads/")
        {
            string imageName =
                DateTime.UtcNow.Ticks +
                Prem.PTC.Utils.Banners.GetExtension(_image);

            SaveOnClient(imageName, filePath);
        }
        public bool HasValidDimensions(int maxWidth, int maxHeight)
        {
            return this.Width <= maxWidth && this.Height <= maxHeight;
        }
    }
}