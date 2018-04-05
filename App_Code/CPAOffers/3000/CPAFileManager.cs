using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using System.Net;
using System.Text;
using System.IO;

namespace Titan
{
    public class CPAFileManager
    {
        public static string HANDLER_LOCATION = "Handlers/CPACreator.ashx";
        public static string BASE_HANDLER_FILE = "Handlers/CPAGPT/BaseHandler.ashx";
        public static string HANDLER_FOLDER = "Handlers/CPAGPT/";
        public static string HANDLER_EXTENSION = ".ashx";
        public static string RESPONSE_OK_CODE = "OK";
        public static string RESPONSE_FRAUD_CODE = "FRAUD";
        public static string RESPONSE_ERROR_CODE = "ERROR";

        /// <summary>
        /// Hash must be unique. Creates new ASHX handler on client side
        /// RUN FROM ADMIN PANEL
        /// </summary>
        /// <param name="Hash"></param>
        public static void CreateHandler(string Hash)
        {
            //Send request to proper handler
            string url = AppSettings.Site.Url + HANDLER_LOCATION;
            string data = String.Format("?h={0}&p={1}", Hash, HashingManager.GenerateSHA256(AppSettings.Offerwalls.UniversalHandlerPassword));

            using (WebClient MyWebClient = new MyWebClient())
            {
                string resultStr = MyWebClient.DownloadString(url + data);

                //All OK
                if (resultStr == RESPONSE_OK_CODE)
                    return;

                //Errors
                if (resultStr == RESPONSE_ERROR_CODE)
                    throw new MsgException("There was a problem with creating ASHX handler. MAKE SURE that you enabled write permissions to "
                        + AppSettings.Site.Url + HANDLER_LOCATION + ", " + "~/" + HANDLER_FOLDER + " directory and all subdirectories and try again.");
                else
                    throw new MsgException("!" + resultStr);
            }
        }

        /// <summary>
        /// Hash must be unique. REMOVES ASHX handler on client side
        /// RUN FROM ADMIN PANEL
        /// </summary>
        /// <param name="Hash"></param>
        public static void RemoveHandler(string Hash)
        {
            //Send request to proper handler
            string url = AppSettings.Site.Url + HANDLER_LOCATION;
            string data = String.Format("?h={0}&p={1}&r=1", Hash, HashingManager.GenerateSHA256(AppSettings.Offerwalls.UniversalHandlerPassword));

            using (WebClient MyWebClient = new MyWebClient())
            {
                string resultStr = MyWebClient.DownloadString(url + data);

                //All OK
                if (resultStr == RESPONSE_OK_CODE)
                    return;

                //Errors
                if (resultStr == RESPONSE_ERROR_CODE)
                    throw new MsgException("There was a problem with removing ASHX handler. MAKE SURE that you enabled write permissions to "
                        + AppSettings.Site.Url + HANDLER_LOCATION + ", " + "~/" + HANDLER_FOLDER + " directory and all subdirectories and try again.");
                else
                    throw new MsgException("!" + resultStr);
            }
        }

        /// <summary>
        /// Hash must be unique. Creates new ASHX handler on client side
        /// RUN FROM CLIENT SIDE
        /// </summary>
        /// <param name="Hash"></param>
        public static void CreateHandlerIAmOnClientSideNow(HttpContext context, string Hash)
        {
            //Run from client side
            string from = context.Server.MapPath("~/" + BASE_HANDLER_FILE);
            string to = context.Server.MapPath("~/" + HANDLER_FOLDER + Hash + ".ashx");
            File.Copy(from, to);
        }

        /// <summary>
        /// Hash must be unique. REMOVES ASHX handler on client side
        /// RUN FROM CLIENT SIDE
        /// </summary>
        /// <param name="Hash"></param>
        public static void RemoveHandlerIAmOnClientSideNow(HttpContext context, string Hash)
        {
            //Run from client side
            string to = context.Server.MapPath("~/" + HANDLER_FOLDER + Hash + ".ashx");
            File.Delete(to);
        }

        public static string GetHashFromClientHandlerHit(HttpContext Context)
        {
            string path = Context.Request.Url.AbsolutePath;
            path = path.Substring(path.IndexOf(HANDLER_FOLDER) + HANDLER_FOLDER.Length);
            string hash = path.Replace(".ashx", "");

            return hash;
        }
    }
}