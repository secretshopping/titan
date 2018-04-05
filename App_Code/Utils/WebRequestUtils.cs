using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

public class WebRequestUtils
{
    /// <summary>
    /// Fixes SSL validation issues
    /// </summary>
	public static void SetUpSecurityProtocols()
    {
        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

        //Tls11 = 768, Tls12 = 3072
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
            (SecurityProtocolType)768 | (SecurityProtocolType)3072;
    }
}