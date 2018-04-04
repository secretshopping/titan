<%@ Page Language="C#" AutoEventWireup="true" CodeFile="externalcpaoffer.aspx.cs" Inherits="externalcpaoffer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../Scripts/default/assets/plugins/jquery/jquery-1.9.1.min.js"></script>
    <script src="../Scripts/default/assets/plugins/jquery/jquery-migrate-1.1.0.min.js"></script>
    <script src="../Scripts/default/assets/plugins/jquery-ui/ui/minified/jquery-ui.min.js"></script>
    <script src="../Scripts/default/assets/plugins/bootstrap/js/bootstrap.min.js"></script>
    <script src="../Scripts/default/assets/plugins/DataTables/media/js/jquery.dataTables.min.js"></script>
    <script src="../Scripts/default/assets/plugins/DataTables/extensions/Responsive/js/dataTables.responsive.min.js"></script>
    <link href="https://fonts.googleapis.com/css?family=Open+Sans:300,400,600,700" rel="stylesheet" />
    <link href="../Scripts/default/assets/plugins/jquery-ui/themes/base/minified/jquery-ui.min.css" rel="stylesheet" />
    <link href="../Scripts/default/assets/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../Scripts/default/assets/plugins/DataTables/media/css/jquery.dataTables.min.css" rel="stylesheet" />
    <link href="../Scripts/default/assets/plugins/DataTables/extensions/Responsive/css/responsive.dataTables.min.css" rel="stylesheet" />
    <link href="../Scripts/default/assets/plugins/font-awesome/css/font-awesome.min.css" rel="stylesheet" />
    <link href="../Scripts/default/assets/css/animate.min.css" rel="stylesheet" />
    <link href="../Scripts/assets/css/jquery.fileupload.css" rel="stylesheet" />
    <link href="../Scripts/assets/css/jquery.fileupload-ui.css" rel="stylesheet" />
    <link href="../Scripts/assets/css/theme/default.css" id="theme" rel="stylesheet" />
    <link href="../Scripts/default/assets/css/style-panel.min.css" rel="stylesheet" />
    <link href="../Scripts/assets/css/style-responsive.min.css" rel="stylesheet" />
    <link href="../Scripts/default/assets/css/custom.css" rel="stylesheet" />
    <style>
        body, form, input {
            margin: 0;
            padding: 0;
        }

        li.cpa-item {
            list-style: none;
        }

            li.cpa-item:after {
                content: '';
                width: 90%;
                border-bottom: 1px solid #f5f5f5;
                margin: 20px auto 0;
            }

        .tab-content {
            padding: 20px 20px !important;
            margin-top: 10px;
        }

        .offer-price .btn {
            margin-top: 15px;
        }

        .navbar-brand {
            color: #333;
            border-bottom: 1px solid #ccc;
        }
    </style>
</head>
<body>
    <form runat="server">
        <div id="page-container" class="page-content-full-height">

            <div class="container-fluid">
                <div class="tab-content">
                    <div class="text-center">
                        <a href="<%=AppSettings.Site.Url %>" class="navbar-brand width-full text-center">
                            <img src="<%=AppSettings.Site.LogoImageURL %>" alt="logo" class="height-full" />
                            <% if (AppSettings.Site.ShowSiteName)
                                { %>
                            <span><%=AppSettings.Site.Name %></span>
                            <% } %>
                        </a>
                    </div>
                    <div style="clear: both;"></div>
                    <p class="text-center m-t-15"><strong>Hi <%=externalUserName %>. Complete offers below and get paid.</strong></p>
                    <div style="clear: both;"></div>
                    <asp:PlaceHolder ID="PemissionErrorPlaceHolder" runat="server" Visible="false">
                        <div class="alert alert-warning">
                            <h3 class="text-warning"><%=U6011.UNAUTHORIZED %></h3>
                            <p><%=U6011.NOPERMISSIONS %></p>
                        </div>
                    </asp:PlaceHolder>
                    <ul class="result-list">
                        <asp:Panel runat="server" ID="OffersPlaceHolder">
                        </asp:Panel>
                    </ul>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
