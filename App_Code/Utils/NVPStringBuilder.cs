using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Prem.PTC.Utils.NVP
{
    public class NVPStringBuilder
    {
        private StringBuilder builder;

        public NVPStringBuilder()
        {
            builder = new StringBuilder();
        }

        public NVPStringBuilder Append(string name, string value)
        {
            name = HttpUtility.UrlEncode(name);
            value = HttpUtility.UrlEncode(value);

            builder.Append(name).Append('=')
                .Append(value).Append('&');

            return this;
        }

        public NVPStringBuilder Append(string name, int value)
        {
            builder
                .Append(HttpUtility.UrlEncode(name)).Append('=')
                .Append(value).Append('&');

            return this;
        }

        public NVPStringBuilder Append(NameValuePair nameValuePair)
        {
            Append(nameValuePair.Name, nameValuePair.Value);

            return this;
        }

        public string Build()
        {
            string render = builder.ToString();

            // delete the last char (ampersand)
            return string.IsNullOrEmpty(render) ? string.Empty : render.Remove(render.Length - 1);
        }

        public override string ToString()
        {
            return Build();
        }
    }
}