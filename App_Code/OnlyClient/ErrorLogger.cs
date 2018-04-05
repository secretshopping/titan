using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Threading;
using ExtensionMethods;
using Prem.PTC.Payments;

namespace Prem.PTC
{
    public static class ErrorLogger
    {
        private const string CLIENT_FOLDER = "~/Logs/";

        public static void Log(Exception ex)
        {
            if (!(ex is ThreadAbortException))
            {
                try { ClientLog(ex); }
                catch (Exception innerEx) {  }
            }
        }

        public static void Log(string message, LogType Type = LogType.Other, bool IsOutgoing = false)
        {
            try { ClientLog(message, IsOutgoing, Type); }
            catch (Exception innerEx) {  }
        }

        private static void ClientLog(Exception ex)
        {
            string path = GetPath(LogType.Exceptions);
            string loggeduser = HttpContext.Current.User.Identity.IsAuthenticated ? HttpContext.Current.User.Identity.Name : "none";
            string message = DateTime.Now + Environment.NewLine + ex.Message + Environment.NewLine
                + ex.InnerException + Environment.NewLine + ex.StackTrace + Environment.NewLine
                + "LOGGED USER: " + loggeduser + Environment.NewLine + ex.StackTrace + Environment.NewLine +
                Environment.NewLine + Environment.NewLine;
            WriteTextToFile(path, message);
        }

        private static void ClientLog(string text, bool IsOutgoing, LogType Type = LogType.Other)
        {
            string path = GetPath(Type, IsOutgoing);
            string message = DateTime.Now + Environment.NewLine + text + Environment.NewLine + Environment.NewLine;
            WriteTextToFile(path, message);
        }

        private static void WriteTextToFile(string path, string text)
        {
            using (StreamWriter _testData = File.AppendText(path))
            {
                _testData.WriteLine(text); // Write the file.
                _testData.Flush();
            }
        }

        public static string GetPath(LogType Type, bool IsOutgoing = false)
        {
            string folderpath = HttpContext.Current.Server.MapPath(CLIENT_FOLDER);

            if (ErrorLoggerHelper.IsPayment(Type))
            { 
                    folderpath += "Payments\\";
                    if (IsOutgoing)
                        folderpath += "Outgoing\\";
                    else
                        folderpath += "Incoming\\";
                    if (!Directory.Exists(folderpath))
                        Directory.CreateDirectory(folderpath);
            }

            folderpath += Type.ToString() + "\\";

            if (!Directory.Exists(folderpath))
                Directory.CreateDirectory(folderpath);

            string filename = Type.ToString() + "_" + DateTime.Now.ToString("yyyy_MM_dd") + ".titanlog";
            string path = folderpath + filename;
            return path;
        }
    }
}