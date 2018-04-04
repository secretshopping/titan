<%@ Application Language="C#" %>

<%@ Import Namespace="System.Web.Http" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="System.Net.Http.Formatting" %>
<%@ Import Namespace="Microsoft.AspNet.FriendlyUrls" %>

<script RunAt="server">

    private static WebSocket4Net.WebSocket websocket;

    private static SSLType SSLSettings
    {
        get
        {
            try
            {
                return AppSettings.SSL.Type;
            }
            catch (Exception ex) { }
            return SSLType.No;
        }
    }

    private static string DefaultLanguage
    {
        get
        {
            try
            {
                return AppSettings.Site.DefaultLanguage;
            }
            catch(Exception ex)
            {
                return "en-US";
            }
        }
    }

    void Application_BeginRequest(Object sender, EventArgs e)
    {
        // CHANGE LANGUAGE SECTION 
        HttpCookie cookie = Request.Cookies["CultureInfo"];

        // if there is some value in cookie
        if (cookie != null && cookie.Value != null)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(cookie.Value);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cookie.Value);
        }
        else // if none value has been sent by cookie, set default language
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(DefaultLanguage);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(DefaultLanguage);
        }

        System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";

        //URL REWRITING 
        string shortPath = Request.Path.Substring(Request.ApplicationPath.Length);

        bool IsHandlerPage = shortPath.Contains("Handlers/") || shortPath.Contains("VisualCaptcha/Handler.ashx") || shortPath.StartsWith("Scripts/"); //Not rewriting handlers

        if (!IsHandlerPage)
        {
            //Full SSL 
            if (SSLSettings == SSLType.Yes)
            {
                bool IsSurfPage = shortPath.Contains("surf.aspx") || shortPath.Contains("ads.aspx") || shortPath.Contains("traffic")
                     || shortPath.Contains("cashlinks.aspx") || shortPath.Contains("adpacks.aspx") || shortPath.Contains("user/earn/auto")
                     || shortPath.Contains("paidtopromote.aspx");

                bool IsSecureConnection = HttpContext.Current.Request.IsSecureConnection.Equals(true);
                // || string.Equals(HttpContext.Current.Request.Headers["X-Forwarded-Proto"], "https");

                if (!IsSecureConnection && HttpContext.Current.Request.IsLocal.Equals(false)
                    && IsSurfPage == false)
                {
                    //Rewrite 
                    Response.Redirect("https://" + Request.ServerVariables["HTTP_HOST"]
                    + HttpContext.Current.Request.RawUrl);
                }
                if (IsSecureConnection && HttpContext.Current.Request.IsLocal.Equals(false)
                    && IsSurfPage == true)
                {
                    //Rewrite OFF for surf
                    Response.Redirect("http://" + Request.ServerVariables["HTTP_HOST"]
                    + HttpContext.Current.Request.RawUrl);
                }
            }

            if (shortPath.StartsWith("user/earn/auto"))
            {
                //Traffic Exchange mode
                //BECAUSE WE WILL HAVE user/earn/auto1
                try
                {
                    int id = Convert.ToInt32(shortPath.Substring(14));
                    Context.RewritePath("~/user/earn/surf.aspx?f=6&auto=" + id);

                }
                catch (Exception ex) { Context.RewritePath("~/user/earn/trafficexchange.aspx"); }
            }
        }
    }

    void Application_Start(object sender, EventArgs e)
    {
        Application["IsDemoScript"] = false;
        Application["ScriptSide"] = (int)ScriptSide.Client;

        #region API
        RouteTable.Routes.MapHttpRoute(
            name: "ClassicApi",
            routeTemplate: "xapi/{controller}"
        );
        #endregion

        #region TITAN News Friendly URLs
        RouteTable.Routes.MapPageRoute("", "article/{id}/{title}", "~/sites/article.aspx");
        RouteTable.Routes.EnableFriendlyUrls();
        #endregion

        #region Obsolete
        //Obsolete
        Application["HasMF"] = "";
        Application["HasNotifications"] = true;
        Application["ResetAdEachMonth"] = false;
        Application["SwedishGeo"] = false;
        Application["CLP"] = false;
        Application[DatabaseConnectionPersister.DBConnectionErrorName] = DateTime.Now.AddYears(10); //Initialize = no error
        Application["CoHits"] = false;
        #endregion
    }

    void Application_End(object sender, EventArgs e)
    {

    }

    void Application_Error(object sender, EventArgs e)
    {

    }

    void Session_Start(object sender, EventArgs e)
    {
    }

    void Session_End(object sender, EventArgs e)
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }

</script>
