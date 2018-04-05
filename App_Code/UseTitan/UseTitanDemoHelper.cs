using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class UseTitanDemoHelper
{
    private static readonly string SessionNameProducts = "DemoProducts";
    private static readonly string SessionNameTheme = "DemoTheme";

    public static void SetProducts(string products)
    {
        HttpContext.Current.Session[SessionNameProducts] = products;
    }

    public static void SetTheme(string theme)
    {
        HttpContext.Current.Session[SessionNameTheme] = theme;
    }

    public static string GetProducts()
    {
        if (IsProductsSet())
            return HttpContext.Current.Session[SessionNameProducts].ToString();
        return String.Empty;
    }

    public static string GetTheme()
    {
        if (IsThemeSet())
            return HttpContext.Current.Session[SessionNameTheme].ToString();
        return String.Empty;
    }

    public static bool IsProductsSet()
    {
        var checkSession = HttpContext.Current.Session == null ? null : HttpContext.Current.Session[SessionNameProducts];
        return checkSession != null;
    }

    public static bool IsThemeSet()
    {
        return HttpContext.Current.Session[SessionNameTheme] != null;
    }
}
