<%@ Page Language="C#" AutoEventWireup="true" CodeFile="maintenance.aspx.cs" Inherits="Maintenance" %>

<!DOCTYPE html>
<!--[if IE 8]> <html lang="<%=System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName %>" class="ie8"> <![endif]-->
<!--[if !IE]><!-->
<html lang="<%=System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName %>">
<!--<![endif]-->
<head>

    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link rel="shortcut icon" type="image/ico" <%= "href='" + AppSettings.Site.FaviconImageURL +"'" %>>
    <title>Maintenance</title>

    <link href="https://fonts.googleapis.com/css?family=Open+Sans:300,400,600,700" rel="stylesheet" />
    <link href="Scripts/default/assets/plugins/jquery-ui/themes/base/minified/jquery-ui.min.css" rel="stylesheet" />
    <link href="Scripts/default/assets/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="Scripts/default/assets/plugins/font-awesome/css/font-awesome.min.css" rel="stylesheet" />
    <link href="Scripts/default/assets/css/animate.min.css" rel="stylesheet" />
    <link href="Scripts/assets/css/style.min.css" rel="stylesheet" />
    <link href="Scripts/assets/css/style-responsive.min.css" rel="stylesheet" />
    <link href="Scripts/default/assets/css/theme/default.css" id="theme" rel="stylesheet" />
    <link href="Scripts/login/assets/css/custom.css" rel="stylesheet" />

</head>

<body class="pace-top">
    <form runat="server">
        <!-- begin #page-loader -->
        <div id="page-loader" class="fade in"><span class="spinner"></span></div>
        <!-- end #page-loader -->
        <!-- begin #page-container -->
        <div id="page-container" class="fade">
            <!-- begin error -->
            <div class="error">
                <div class="error-code m-b-10"><i class="fa fa-clock-o"></i></div>
                <div class="error-content p-l-20 p-r-20">
                    <div class="error-message">
                        The website is under maintenance.
                    </div>
                    <div class="error-desc m-b-20">
                        We will be back on <%=AppSettings.Site.MaintenanceModeEndsDate.ToString() %>
                    </div>
                </div>
            </div>
            <!-- end error -->
            <!-- begin scroll to top btn -->
            <a href="javascript:;" class="btn btn-icon btn-circle btn-success btn-scroll-to-top fade" data-click="scroll-top"><i class="fa fa-angle-up"></i></a>
            <!-- end scroll to top btn -->
        </div>
        <!-- end page container -->

        <script src="Scripts/default/assets/plugins/jquery/jquery-1.9.1.min.js"></script>
        <script src="Scripts/default/assets/plugins/jquery/jquery-migrate-1.1.0.min.js"></script>
        <script src="Scripts/default/assets/plugins/jquery-ui/ui/minified/jquery-ui.min.js"></script>
        <script src="Scripts/default/assets/plugins/bootstrap/js/bootstrap.min.js"></script>
        <script data-pace-options='{"ajax": false}' src="Scripts/assets/js/pace.min.js"></script>
        <script src="Scripts/assets/js/apps.min.js"></script>

        <script>
            $(document).ready(function () {
                App.init();
            });
        </script>
    </form>
</body>
</html>




