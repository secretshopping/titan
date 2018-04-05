﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text;

namespace Prem.PTC.Utils.NVP
{
    /// <summary>
    /// Sends request via HTTP protocol using NVP pairs
    /// </summary>
    public class NVPWebRequest
    {
        private HttpWebRequest webRequest;

        public HttpWebResponse Response
        {
            get { return webRequest.GetResponse() as HttpWebResponse; }
        }

        public string URL { get; private set; }

        public NVPWebRequest(string url)
        {
            URL = url;
            webRequest = WebRequest.Create(URL) as HttpWebRequest;
        }

        public void SendRequest(string nameValuePairs)
        {
            SendRequest(nameValuePairs, Encoding.ASCII);
        }

        public void SendRequest(string nameValuePairs, Encoding encoding, string contentType = "application/x-www-form-urlencoded")
        {
            webRequest.Method = "POST";
            webRequest.ContentLength = nameValuePairs.Length;
            webRequest.ContentType = contentType;

            //Fixes SSL validation issues
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // SecurityProtocolType.Tls12
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            var streamWriter = new StreamWriter(webRequest.GetRequestStream(), encoding);
            streamWriter.Write(nameValuePairs);
            streamWriter.Close();

        }

        public void SendRequest(NameValuePairs nameValuePairs)
        {
            SendRequest(nameValuePairs.ToString());
        }

        public void SendRequest(NameValuePair nameValuePair)
        {
            SendRequest(nameValuePair.ToString());
        }

        public void SendRequest(NameValuePairs nameValuePairs, Encoding encoding)
        {
            SendRequest(nameValuePairs.ToString(), encoding);
        }

        public void SendRequest(NameValuePair nameValuePair, Encoding encoding)
        {
            SendRequest(nameValuePair.ToString(), encoding);
        }
    }
}