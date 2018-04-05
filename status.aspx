<%@ Page Language="C#" AutoEventWireup="true"
    CodeFile="status.aspx.cs" Inherits="About" %>

<!DOCTYPE html>
<!--[if IE 8]> <html lang="<%=System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName %>" class="ie8"> <![endif]-->
<!--[if !IE]><!-->
<html lang="<%=System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName %>">
<!--<![endif]-->
<head id="Head1" runat="server">

    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link rel="shortcut icon" type="image/ico" <%= "href='" + AppSettings.Site.FaviconImageURL +"'" %>>
    <title><%=AppSettings.Site.Name %></title>

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
    <form id="Form1" runat="server">

    <!-- begin #page-loader -->
	<div id="page-loader" class="fade in"><span class="spinner"></span></div>
	<!-- end #page-loader -->
	
	<!-- begin #page-container -->
	<div id="page-container" class="fade">
	    <!-- begin error -->
        <div class="error">
            <div class="error-code m-b-10"><i class="<%=IconClass %>"></i></div>
            <div class="error-content p-l-20 p-r-20">
                <div class="error-message"><asp:Label ID="Title" runat="server" Text="No operation specified"></asp:Label></div>
                
                <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                <asp:Literal ID="Literal2" runat="server"></asp:Literal>
                <div class="error-desc m-b-20">
                    <asp:Label ID="Description" runat="server" Text=""></asp:Label>
                </div>

                <a href="default.aspx" class="btn btn-success m-t-20">Return to homepage</a>
            </div>
        </div>
        <!-- end error -->    

		<!-- begin scroll to top btn -->
		<a href="javascript:;" class="btn btn-icon btn-circle btn-success btn-scroll-to-top fade" data-click="scroll-top"><i class="fa fa-angle-up"></i></a>
		<!-- end scroll to top btn -->
	</div>
	<!-- end page container -->

    </form>

    <script src="Scripts/default/assets/plugins/jquery/jquery-1.9.1.min.js"></script>
        <script src="Scripts/default/assets/plugins/jquery/jquery-migrate-1.1.0.min.js"></script>
        <script src="Scripts/default/assets/plugins/jquery-ui/ui/minified/jquery-ui.min.js"></script>
        <script src="Scripts/default/assets/plugins/bootstrap/js/bootstrap.min.js"></script>
        <script data-pace-options='{"ajax": false}' src="Scripts/assets/js/pace.min.js"></script>
	<script src="Scripts/assets/js/apps.min.js"></script>

    <script>
		$(document).ready(function() {
			App.init();
		});
	</script>

    <script type="text/javascript">

        $('document').ready(function () {
            if (parseGET('pp') == 'SolidTrustPay')
                submitformSolidTrustPay();

            if (parseGET('pp') == 'PerfectMoney')
                submitformPerfectMoney();
        });

        function submitformPerfectMoney() {
            // Get form object
            var myFormObj = document.getElementById("<%=Form.ClientID%>");

            // Change form action & submit
            myFormObj.action = "https://perfectmoney.is/api/step1.asp";
            myFormObj.submit();
        }

        function submitformSolidTrustPay() {
            // Get form object
            var myFormObj = document.getElementById("<%=Form.ClientID%>");

            // Change form action & submit
            myFormObj.action = "https://solidtrustpay.com/handle.php";
            myFormObj.submit();
        }
        
        function parseGET(val) {
            var result = "Not found",
                tmp = [];
            location.search
            //.replace ( "?", "" ) 
            // this is better, there might be a question mark inside
            .substr(1)
                .split("&")
                .forEach(function (item) {
                    tmp = item.split("=");
                    if (tmp[0] === val) result = decodeURIComponent(tmp[1]);
                });
            return result;
        }

    </script>
</body>
</html>


