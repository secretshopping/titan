using Newtonsoft.Json.Linq;
using Prem.PTC;
using Prem.PTC.Utils.NVP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Titan.CustomFeatures
{
    public class S4DSAPI
    {
        public static string APIEndpoint = "https://app.s4ds.com/1fashionglobal-core/";

        public static JObject SendRequest(string service, string values)
        {
            try
            {
                var request = new NVPWebRequest(APIEndpoint + service);
                request.SendRequest(values, Encoding.ASCII);

                var rawResponse = new NVPWebResponse(request.Response).RawContent;
                return JObject.Parse(rawResponse);
            }
            catch(Exception ex)
            {
                ErrorLogger.Log(ex);
                throw new MsgException("Unable to connect to S4DS API");
            }
        }
    }

    public class S4DSAuthInfo
    {
        public string personId { get; set; }
        public string token { get; set; }

        public bool IsOK
        {
            get
            {
                return !String.IsNullOrEmpty(personId) && !String.IsNullOrEmpty(token);
            }
        }

        public S4DSAuthInfo(JObject reply)
        {
            if (reply["result"] != null)
                {
                if (reply["result"]["token"] != null)
                    token = reply["result"]["token"].ToString();

                if (reply["result"]["personId"] != null)
                    personId = reply["result"]["personId"].ToString();
            }
        }
    }
}