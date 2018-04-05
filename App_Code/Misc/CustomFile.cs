using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Titan;

namespace Prem.PTC
{
    public class CustomFile
    {
        public string Path { get; private set; }
        public string MappedPath { get { return Utils.FileUtils.MapPath(Path); } }

        private Stream _Stream;

        /// <summary>
        /// Whether file is saved in filesystem.
        /// </summary>
        public bool IsSaved
        {
            get
            {
                return String.IsNullOrWhiteSpace(Path) ? false : File.Exists(MappedPath);
            }
        }

        public CustomFile(Stream stream, string path = "")
        {
            _Stream = stream;
            Path = path;
        }

        public CustomFile(string content, string path = "")
        {
            _Stream = GenerateStreamFromString(content);
            Path = path;
        }

        public static CustomFile Empty
        {
            get { return new CustomFile(Stream.Null); }
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
                _Stream = null;
            }
        }

        private string Serialize()
        {
            string base64String;

            using (var ms = new MemoryStream())
            {
                _Stream.CopyTo(ms);
                byte[] imageBytes = ms.ToArray();
                base64String = Convert.ToBase64String(imageBytes);
            }

            return base64String;
        }

        public static CustomFile Deserialize(string base64String)
        {
            byte[] pdfBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(pdfBytes, 0, pdfBytes.Length);

            ms.Write(pdfBytes, 0, pdfBytes.Length);
            return new CustomFile(ms);
        }

        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Saves file under specified directory path and with specified filename.
        /// </summary>
        /// <param name="directoryPath">Path to directory where image file should be saved.</param>
        /// <param name="name">Suggested name. Remember to include valid file extension.</param>
        /// <param name="forceSave">When set as false and this banner IsSaved exception is thrown. 
        /// When set as true attempt to override file is made, however not guaranteed.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidStateException"/>
        /// <exception cref="InvalidOperationException"/>
        public void Save(string directoryPath, string name, bool forceSave = false)
        {
            if (!forceSave && IsSaved)
                throw new InvalidOperationException("Can not save file.");

            directoryPath = directoryPath.Replace("/", System.IO.Path.DirectorySeparatorChar + "");

            string mappedDirectoryPath = "";

            if (!Directory.Exists(directoryPath))
                mappedDirectoryPath = Utils.FileUtils.MapPath(directoryPath);

            if (!Directory.Exists(mappedDirectoryPath))
                throw new ArgumentException("Not a directory: " + directoryPath);

            if (!directoryPath.EndsWith(System.IO.Path.DirectorySeparatorChar + ""))
                directoryPath += System.IO.Path.DirectorySeparatorChar;

            string _path = directoryPath + name;

            using (var fileStream = File.Create(Utils.FileUtils.MapPath(_path)))
            {
                _Stream.Seek(0, SeekOrigin.Begin);
                _Stream.CopyTo(fileStream);
            }

            Path = _path;
        }

        //Method return MsgException
        public void SaveOnClient(string fileName, string filePath)

        {
            if (_Stream == null)
                throw new ArgumentNullException();

            using (var client = new WebClient())
            {
                var values = new NameValueCollection();
                values["p"] = HashingManager.SHA256(AppSettings.Offerwalls.UniversalHandlerPassword);
                values["fn"] = HttpUtility.UrlEncode(fileName);
                values["fp"] = HttpUtility.UrlEncode(filePath);
                values["h"] = HttpUtility.UrlEncode(Serialize());

                //Send request to proper handler
                string urlPost = AppSettings.Site.Url + "Handlers/CustomFileCreator.ashx";

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
    }
}