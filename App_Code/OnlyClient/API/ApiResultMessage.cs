using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.API
{
    public class ApiResultMessage
    {
        public bool success { get; set; }

        public string message { get; set; }
        public bool messageIsHtml { get; set; }

        public object data { get; set; }
    }
}