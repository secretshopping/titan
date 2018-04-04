using System.IO;
using System.Net;

namespace Prem.PTC.Utils.NVP
{
    public class NVPWebResponse
    {
        private HttpWebResponse webResponse;

        private NameValuePairs _content;
        private string _result;

        public NameValuePairs Content
        {
            get
            {
                if (_content == null && webResponse != null)
                {
                    using (var streamReader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        string result = streamReader.ReadToEnd();
                        _content = NameValuePairs.Parse(result);
                    }
                }

                return _content;
            }
        }

        public string RawContent
        {
            get
            {
                if (_result == null && webResponse != null)
                {
                    using (var streamReader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        _result = streamReader.ReadToEnd();
                    }
                }

                return _result;
            }
        }

        public NVPWebResponse(HttpWebResponse response)
        {
            this.webResponse = response;
        }
    }
}