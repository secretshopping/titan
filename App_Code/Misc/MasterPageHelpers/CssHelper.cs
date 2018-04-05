using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class CssHelper
{
    public static string GetBodyCssClass()
    {
        string path = HttpContext.Current.Request.Url.AbsolutePath.TrimStart('/');
        return "bodyclass-" + path.Replace("/", "-").Replace(".aspx", "");
    }
}