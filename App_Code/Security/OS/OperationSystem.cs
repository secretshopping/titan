using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class OperationSystem
{
    public static OperationSystemType Current
    {
        get
        {
            string userAgent = HttpContext.Current.Request.UserAgent;

            if (userAgent.Contains("Windows NT"))
                return OperationSystemType.Windows;

            if (userAgent.Contains("Mac OS") || userAgent.Contains("Macintosh") || userAgent.Contains("Mac_PowerPC"))
                return OperationSystemType.Mac;

            if (userAgent.Contains("Windows Phone"))
                return OperationSystemType.WindowsPhone;

            if (userAgent.Contains("iPad"))
                return OperationSystemType.iPad;

            if (userAgent.Contains("iPhone"))
                return OperationSystemType.iPhone;

            if (userAgent.Contains("Linux"))
                return OperationSystemType.Linux;

            return OperationSystemType.Unknown;
        }
    }

}