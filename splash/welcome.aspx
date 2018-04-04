<%@ Page Language="C#" AutoEventWireup="true"
    CodeFile="welcome.aspx.cs" Inherits="About" %>



<!DOCTYPE html>
<!--[if IE 8]> <html lang="<%=System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName %>" class="ie8"> <![endif]-->
<!--[if !IE]><!-->
<html lang="<%=System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName %>">
<!--<![endif]-->
<head>
    <base href="<%=BaseUrl %>" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" name="viewport" />
    <link rel="shortcut icon" type="image/ico" href="<%=AppSettings.Site.FaviconImageURL %>">
    <title><%=AppSettings.Site.Name + " | " +  AppSettings.Site.Slogan %></title>
    <link href="https://fonts.googleapis.com/css?family=Open+Sans:300,400,600,700" rel="stylesheet" />
    <link href="Scripts/default/assets/plugins/jquery-ui/themes/base/minified/jquery-ui.min.css" rel="stylesheet" />
    <link href="Scripts/default/assets/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="Scripts/default/assets/plugins/font-awesome/css/font-awesome.min.css" rel="stylesheet" />
    <link href="Scripts/default/assets/plugins/bootstrap-social/bootstrap-social.css" rel="stylesheet" />
    <link href="Scripts/default/assets/css/animate.min.css" rel="stylesheet" />
    <link href="Scripts/assets/css/style.min.css" rel="stylesheet" />
    <link href="Scripts/assets/css/style-responsive.min.css" rel="stylesheet" />
    <link href="Scripts/default/assets/css/theme/default.css" id="theme" rel="stylesheet" />
    <link href="Scripts/login/assets/css/custom.css" rel="stylesheet" />
    <script src="Scripts/default/assets/plugins/jquery/jquery-1.9.1.min.js"></script>
    <script src="Scripts/default/assets/plugins/jquery/jquery-migrate-1.1.0.min.js"></script>

    <!-- Facebook wall link handlers-->
    <titan:FacebookOGraphInfo runat="server" />

    <titan:CustomHeader runat="server" />

</head>

<body class="pace-top <%=CssHelper.GetBodyCssClass() %>">

    <form runat="server" id="form2">

        <div class="login-cover">
            <div class="login-cover-image" style="background-image: url(Images/Login/bg-login.jpg)">
            </div>
            <div class="login-cover-bg"></div>
        </div>
        <!-- begin #page-container -->
        <div id="page-container" class="fade">
            <!-- begin login -->
            <div class="login login-v2">
                <!-- begin brand -->
                <div class="login-header">
                    <div class="brand">
                        <a href="default.aspx">
                            <p class="text-center"><span class="logo"></span><%=AppSettings.Site.Name %>
                            <small><%=AppSettings.Site.Slogan %></small></p>
                        </a>
                    </div>
                </div>
                <!-- end brand -->
                <div class="splash-content">
                    <div class="row">
                        <div class="col-md-6 text-center">
                            <div class="m-b-15">
                                <asp:Literal ID="SplashPageYoutubeUrl" runat="server"></asp:Literal>
                            </div>
                            <span class="p-b-15" style="word-wrap: break-word; line-height: 40px; font-size: 36px; color: <%=AppSettings.Site.MainColor %>;"><%=AppSettings.SplashPage.SplashPageSlogan %></span>

                        </div>
                        <div class="col-md-6">
                            <titan:Register runat="server"></titan:Register>
                        </div>
                    </div>

                </div>
            </div>
            <!-- end login -->

        </div>
        <!-- end page container -->



    </form>


    <script src="Scripts/default/assets/plugins/jquery-ui/ui/minified/jquery-ui.min.js"></script>
    <script src="Scripts/default/assets/plugins/bootstrap/js/bootstrap.min.js"></script>

    <script src="Scripts/login/assets/js/login-v2.demo.min.js"></script>
    <script src="Scripts/assets/js/apps.min.js"></script>

    <script>
        $(document).ready(function () {
            App.init();
            LoginV2.init();

        });
	</script>

    <titan:CustomFooter runat="server" />
</body>
</html>
