<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ptcofferwall.aspx.cs" Inherits="ptcofferwall" %>

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
    <script src="../Scripts/assets/js/custom.js"></script>
    <link href="http://fonts.googleapis.com/css?family=Open+Sans:300,400,600,700" rel="stylesheet" />
    <link href="../Scripts/default/assets/plugins/jquery-ui/themes/base/minified/jquery-ui.min.css" rel="stylesheet" />
    <link href="../Scripts/default/assets/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../Scripts/default/assets/plugins/DataTables/media/css/jquery.dataTables.min.css" rel="stylesheet" />
    <link href="../Scripts/default/assets/plugins/DataTables/extensions/Responsive/css/responsive.dataTables.min.css" rel="stylesheet" />
    <link href="../Scripts/default/assets/plugins/font-awesome/css/font-awesome.min.css" rel="stylesheet" />
    <link href="../Scripts/default/assets/css/animate.min.css" rel="stylesheet" />
    <link href="../Scripts/assets/css/jquery.fileupload.css" rel="stylesheet" />
    <link href="../Scripts/assets/css/jquery.fileupload-ui.css" rel="stylesheet" />
    <style>
        body, form, input {
            margin: 0;
            padding: 0;
        }

        .container {
            padding-top: 15px;
            cursor: pointer;
        }

        .image-info {
            padding: 15px;
            box-shadow: 1px 1px 1px 1px #ccc;
            margin-top: 10px;
            border: 1px solid #fff;
            border-radius: 5px;
        }

            .image-info:hover {
                background: #f5f5f5;
                box-shadow: none;
                border-color: #eee;
            }
    </style>
    <script type="text/javascript">
        $(document).on('click', '.offer-wall', function () {
            var offerId = $(this).find('#offer-id').val();
            var offerTitle = $(this).find('#offer-title').val();

            doSubmit(offerId, offerTitle);
        });

        function doSubmit(offerId, offerTitle) {
            $('#__OfferWallId').val(offerId);
            $('#__OfferWallTitle').val(offerTitle);
            $('#__SubId').val('<%=ExternalUserName%>');
            $('#__SubId2').val('<%=SubId2%>');
            $('#__SubId3').val('<%=SubId3%>');
            $('#__Age').val('<%=Age%>');
            $('#__Gender').val('<%=Gender%>');
            $('#__PublishersWebsiteId').val('<%=PublishersWebsiteId%>');
            $('#__CountryCode').val('<%=CountryCode%>');

            $('#<%=Form.ClientID%>').attr('action', '<%=AppSettings.Site.Url%>api/surf.aspx');
            $('#<%=Form.ClientID%>').attr('target', '_blank');

            $('#<%=Form.ClientID%>').submit();
            $('#<%=Form.ClientID%>').attr('action', '<%=AppSettings.Site.Url%>api/ptcofferwall.aspx');
            $('#<%=Form.ClientID%>').attr('target', '_self');

        }


    </script>
</head>
<body>
    <div class="container">
        <form runat="server">
            <asp:ScriptManager runat="server"></asp:ScriptManager>
            <asp:UpdatePanel runat="server" ID="PtcOfferWallsUpdatePanel" OnLoad="PtcOfferWallsUpdatePanel_Load" ClientIDMode="Static">
                <ContentTemplate>
                    <div class="row">
                        <div class="col-md-10">
                            <a href="<%=AppSettings.Site.Url%>" class="navbar-brand">
                                <span>
                                    <img src="<%=AppSettings.Site.Url + AppSettings.Site.LogoImageURL %>" class="" style="height: 30px;" /></span>
                                <% if (AppSettings.Site.ShowSiteName)
                                    { %>
                                <span><%=AppSettings.Site.Name %></span>
                                <% } %>
                            </a>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-2">
                            <asp:LinkButton runat="server" ID="ReloadOfferWallsButton" OnClick="Page_Load" CssClass="btn">
                            </asp:LinkButton>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <input type="hidden" name="__OfferWallId" id="__OfferWallId" value="" />
                            <input type="hidden" name="__OfferWallTitle" id="__OfferWallTitle" value="" />
                            <input type="hidden" name="__PublishersWebsiteId" id="__PublishersWebsiteId" value="" />
                            <input type="hidden" name="__SubId" id="__SubId" value="" />
                            <input type="hidden" name="__SubId2" id="__SubId2" value="" />
                            <input type="hidden" name="__SubId3" id="__SubId3" value="" />
                            <input type="hidden" name="__Age" id="__Age" value="" />
                            <input type="hidden" name="__Gender" id="__Gender" value="" />
                            <input type="hidden" name="__CountryCode" id="__CountryCode" value="" />
                            <asp:Panel runat="server" ID="OffersPlaceHolder">
                            </asp:Panel>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </form>
    </div>
</body>
</html>
