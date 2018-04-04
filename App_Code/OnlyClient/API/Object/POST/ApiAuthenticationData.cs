using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.API
{
    public class ApiAuthenticationData
    {
        public string username { get; set; }
        public string password { get; set; }
        public string secondaryPassword { get; set; }

        public bool isFacebook { get; set; }
    }
}