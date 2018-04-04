using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prem.PTC.Members
{
    /// <summary>
    /// Summary description for MemberAuthenticationData
    /// </summary>
    public class MemberAuthenticationData
    {
        public string Username { get; set; }
        public string PrimaryPassword { get; set; }
        public string SecondaryPassword { get; set; }

        public MemberAuthenticationData(string username, string password1, string password2)
        {
            Username = username;
            PrimaryPassword = password1;
            SecondaryPassword = password2;
        }
    }
}