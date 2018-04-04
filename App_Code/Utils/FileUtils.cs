using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Web;

namespace Prem.PTC.Utils
{
    /// <summary>
    /// Provides some functions unrelated to any other classes.
    /// </summary>
    public static class FileUtils
    {
        public static string MapPath(string path)
        {
            //if (path.Contains("http://") || path.Contains("https://"))
            //    return path;

            if (HttpContext.Current != null)
                return HttpContext.Current.Server.MapPath(path);

            return HttpRuntime.AppDomainAppPath + path.Replace("~", string.Empty).Replace('/', Path.DirectorySeparatorChar);
        }
    }
}