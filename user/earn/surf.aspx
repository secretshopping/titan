<%@ Page Language="C#" AutoEventWireup="true" CodeFile="surf.aspx.cs" Inherits="About" EnableViewStateMac="false" %>

<!DOCTYPE html>
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <base href="<%=BaseUrl %>" />
    <link rel="shortcut icon" type="image/ico" href="<%=AppSettings.Site.FaviconImageURL %>">
    <link href="http://fonts.googleapis.com/css?family=Open+Sans:300,400,600,700" rel="stylesheet" />
    <link href="Scripts/default/assets/plugins/jquery-ui/themes/base/minified/jquery-ui.min.css" rel="stylesheet" />
    <link href="Scripts/default/assets/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="Scripts/default/assets/plugins/font-awesome/css/font-awesome.min.css" rel="stylesheet" />
    <link href="Scripts/default/assets/css/animate.min.css" rel="stylesheet" />
    <link href="Scripts/assets/css/style-responsive.min.css" rel="stylesheet" />
    <link href="Scripts/default/assets/css/theme/default.css" id="theme" rel="stylesheet" />

    <!--This page uses the script from http://blakek.us/labs/jquery/css3-pie-graph-timer/ -->
    <link rel="stylesheet" href="Plugins/ProgressPie/Style.css" type="text/css" />
    <link rel="stylesheet" href="Styles/NewClasses.css?s=1d" type="text/css" />
    <link rel="stylesheet" href="Styles/theme-colors.css.ashx" type="text/css" />

    <link href="Scripts/default/assets/css/style-panel.min.css" rel="stylesheet" />
    <link href="Scripts/default/assets/css/custom.css" rel="stylesheet" />

    <script data-cfasync="false" src="Plugins/ProgressPie/Script.js?x=p"></script>
    <script data-cfasync="false" src="Scripts/jquery.min.js"></script>

    <script data-cfasync="false">

        var loaded = false;
        var invalid = false;

        jQuery(function ($) {
            if ('<%=windowOpener%>' == 'URLChecker')
                setTimeout(doesntLoadInformation, 15000); //For URL Checker

            var totalHeight = $(window).innerHeight();
            $('#targetIframe1').height(totalHeight - $('#frameDivMain').height() - 25);
            $('#targetIframe1').load(function () {
                if (invalid == false) {
                    //Ad loaded, start progress bar
                    loaded = true;
                    var adTime = $('#adtime').val();
                    $('#lblLoad').hide();
                    if (!isNaN(adTime)) {
                        startBar(adTime, '<%=Resources.U4200.TIMELEFT%>','<%=AppSettings.GlobalAdvertsSettings.RequireFocusOnAdvertsSurf.ToString().ToLower() %>');
                    }
                }
                $('#targetIframe1').height(totalHeight - $('#frameDivMain').height() - 25);
            });
        });

        $(window).resize(function () {
            var totalHeight = $(window).innerHeight();
            $('#targetIframe1').height(totalHeight - $('#frameDivMain').height() - 25);
        });

        function onCloseButtonClick() {
            openerPostback();
            window.close();
            return false;
        }

        function onValidatorCloseButtonClick() {
            window.opener.__doPostBack('AddNewAdWithURLCheck', '<%=HashedURL%>');
            window.close();
            return false;
        }

        function openerPostback() {
            if ('<%=windowOpener%>' == 'TrafficGrid')
                window.opener.__doPostBack('TrafficRefreshUpdatePanel', '<%=HashedTrafficAd%>');
            else if ('<%=windowOpener%>' == 'PTC' )
                window.opener.__doPostBack('AdRefreshUpdatePanel', '<%=HashedTrafficAd%>');
            else if ('<%=windowOpener%>' == 'AdPack')
                window.opener.__doPostBack('AdPackRefreshUpdatePanel', '<%=HashedTrafficAd%>');
        }

        function doesntLoadInformation() {
            if (loaded == false) {
                $('#lblLoad').hide();
                $('#lblLoadInvalid').show();
                invalid = true;
            }
        }

        function pageLoad() {

            var autoUrl = $('#AutoUrl').val();
            if (autoUrl) {
                var delay = <%=AppSettings.RevShare.AdPack.TimeBetweenAdsRedirectInSeconds%> * 1000; //Your delay in milliseconds

        if ('<%=windowOpener%>' == 'TrafficExchange')
            delay = <%=AppSettings.TrafficExchange.TimeBetweenAdsRedirectInSeconds%> *1000;

        setTimeout(function () { window.location = autoUrl; }, delay);
        }
        }

    </script>
    <asp:Literal ID="ScriptLiteral" runat="server" Text="<script>function load() {} </script>"></asp:Literal>
    <title>
        <asp:Literal ID="SiteName" runat="server"></asp:Literal>
        | <%=U4000.ADVERTISEMENT %></title>

</head>
<body onload="load();" class="bg-white surf">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

        <asp:Button ID="CaptchaPostbackHackButton" runat="server" CssClass="displaynone" />
        <asp:Literal ID="AdInfoContainer" runat="server"></asp:Literal>
        <asp:HiddenField runat="server" ID="AutoUrl" ClientIDMode="Static" />
        <div class="container-fluid">
            <div id="frameDivMain" class="row navbar-default">

                <asp:Panel ID="AvatarPanel" runat="server" Visible="false" CssClass="col-md-2">
                    <titan:MemberInfo runat="server" ID="AvatarInfo" ManualDatabind="true" />
                </asp:Panel>
                <div class="col-md-4 col-sm-12 m-b-10 m-t-10">
                    <asp:Panel ID="BeforePanel" runat="server">
                        <span id="lblLoad" class="dot-loading"><%=L1.WAITFORAD %></span>
                        <div id="timerDiv" class="timer"></div>
                    </asp:Panel>

                    <asp:Panel ID="BeforePanelURLChecker" runat="server" Visible="false">
                        <span id="lblLoad" class="dot-loading"><%=L1.WAITFORAD %></span>
                        <span id="lblLoadInvalid" style="display: none">
                            <span class="fa fa-times fa-lg text-danger"></span>
                            <asp:Literal ID="ErrorLiteralChecker" runat="server"></asp:Literal></span>
                        <div id="timerDiv" class="timer"></div>
                    </asp:Panel>


                    <asp:Panel ID="CaptchaPanel" runat="server" Visible="false">

                        <div class="row">
                            <div class="col-xs-12" style="display: inline-block">
                                <div class="pull-left">
                                    <asp:CustomValidator OnServerValidate="Validate_Captcha" runat="server" ValidationGroup="RegisterUserValidationGroup"
                                        Display="None" ID="CustomValidator1" EnableClientScript="False" CssClass="text-danger">*</asp:CustomValidator>

                                    <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="alert alert-danger"
                                        ValidationGroup="RegisterUserValidationGroup" DisplayMode="List" Visible="false" />

                                    <titan:Captcha runat="server" ID="TitanCaptcha" ValidationGroup="RegisterUserValidationGroup" ValidatorDisplay="None" />
                                </div>
                                <div class="pull-left captcha-banner-submit">
                                    <asp:Button ID="CreditAfterCaptcha" ValidationGroup="RegisterUserValidationGroup" runat="server" CssClass="btn btn-default bg-green" OnClick="CreditAfterCaptcha_Click" />
                                </div>

                            </div>
                        </div>
                        <%--visible when from ads--%>
                    </asp:Panel>

                    <asp:UpdatePanel runat="server">
                        <ContentTemplate>

                            <asp:Panel ID="FeedbackCaptchaPanel" runat="server" Visible="false">

                                <%--visible when from ads--%>

                                <asp:Label ID="FeedbackCaptchaQuestionLabel" runat="server"></asp:Label>
                                <br />

                                <asp:ImageButton runat="server" ID="YesButton" Width="16px" Height="16px" ImageUrl="~/Images/Misc/yes.png" ImageAlign="Top"
                                    CommandArgument="1" OnClick="CreditAfterFeedbackCaptcha_Click" CssClass="captchaImage"></asp:ImageButton>
                                <asp:Label ID="YesCountLabel" runat="server" Font-Size="Smaller"></asp:Label>
                                &nbsp;
                                <asp:ImageButton runat="server" ID="NoButton" Width="16px" Height="16px" ImageUrl="~/Images/Misc/no.png" ImageAlign="AbsMiddle"
                                    CommandArgument="0" OnClick="CreditAfterFeedbackCaptcha_Click" CssClass="captchaImage"></asp:ImageButton>

                                <asp:Label ID="NoCountLabel" runat="server" Font-Size="Smaller"></asp:Label>

                            </asp:Panel>



                            <asp:Panel ID="AfterCounterPanel" runat="server" Visible="false">
                                <%--visible when from Ads--%>
                                <asp:Image ID="StatusImage" runat="server" CssClass="imagemiddle" />
                                <asp:Literal ID="InfoText" runat="server" />
                                <asp:Button ID="AdCloseRefreshButton" runat="server" CssClass="btn btn-danger m-l-10" OnClientClick="onCloseButtonClick();" />

                                <asp:Button ID="HomeButton" runat="server" CssClass="btn btn-primary m-l-10" OnClientClick="onCloseButtonClick();" Visible="false" />
                                <asp:Button ID="NextAddButton" ForeColor="White" runat="server" CssClass="btn btn-primary m-l-10" Visible="false" OnClick="NextAddButton_Click"></asp:Button>
                                <asp:Button ID="DashboardButton" ForeColor="White" runat="server" CssClass="btn btn-primary m-l-10" Visible="false"></asp:Button>

                                <br /><br />

                                <asp:PlaceHolder ID="FinalInfoPlaceHolder" runat="server" Visible="true">
                                    <%=L1.FEELFREE %>
                                    <asp:Literal ID="FeelFreeLiteral1" runat="server"></asp:Literal>
                                </asp:PlaceHolder>
                                
                                <asp:PlaceHolder ID="FavoriteAdsPanel" runat="server" Visible="false">
                                    <%--visible when from ads--%>


                                    <asp:LinkButton runat="server" ID="FavoriteAdsImageButton"
                                        OnClick="FavoriteAdsImageButton_Click" CssClass="captchaImage favoriteImage" OnClientClick="if (Page_ClientValidate()){this.disabled = true}">
                                        <span class="fa fa-heart fa-lg text-danger"></span>
                                    </asp:LinkButton>
                                    <asp:Label ID="FavoriteResult" runat="server" Font-Size="Smaller"></asp:Label>

                                </asp:PlaceHolder>
                            </asp:Panel>

                            <asp:Panel ID="CreditedPanel" runat="server" Visible="false">
                                <%--visible when from TrafficGrid--%>
                                <span class="fa fa-check fa-lg text-success"></span>
                                <asp:Label ID="TextInformation" runat="server" Font-Bold="true"></asp:Label>
                                <asp:Button ID="CloseRefreshButton" runat="server" CssClass="btn btn-success m-l-10" OnClientClick="onCloseButtonClick();" />
                                <br />
                                <br />
                                <%=L1.FEELFREE %>
                                <asp:Literal ID="FeelFreeLiteral2" runat="server"></asp:Literal>
                            </asp:Panel>

                            <asp:Panel ID="PaidToPromotePanel" runat="server" Visible="false">
                                <%--visible when from PaidToPromote--%>
                                <span class="fa fa-check fa-lg text-success"></span>
                                <asp:Label ID="PaidToPromoteLabel" runat="server" Font-Bold="true"></asp:Label>
                                <br />
                                <br />
                                <%=L1.FEELFREE %>
                                <asp:Literal ID="FeelFreePTPLiteral" runat="server"></asp:Literal>
                            </asp:Panel>

                            <asp:Panel ID="URLCheckerOK" runat="server" Visible="false">
                                <%--visible when from URL Checker--%>
                                <span class="fa fa-check fa-lg text-success"></span>
                                <span><%=U4200.URLCHECKEROK %></span>
                                <asp:Button ID="CheckerCloseRefreshButton" runat="server" CssClass="btn btn-success m-l-10" OnClientClick="onValidatorCloseButtonClick();" />
                            </asp:Panel>

                            <asp:Panel ID="Panel1" runat="server" Visible="false">
                                <span class="fa fa-check fa-lg text-success"></span>

                                <asp:Label ID="MoneyCredited" runat="server" Font-Bold="true"></asp:Label>

                                <asp:Label ID="PointsCredited" runat="server" Font-Bold="true"></asp:Label>
                                <asp:Label ID="PointsName" runat="server"></asp:Label>

                                <br />
                                <br />
                                <%=L1.FEELFREE %>
                                <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                                <asp:Button ID="Button1" Text="Close" runat="server" CssClass="btn btn-success m-l-10" OnClientClick="onCloseButtonClick();" />
                            </asp:Panel>

                            <asp:Panel ID="ErrorPanel" runat="server" Visible="false">
                                <span class="fa fa-times fa-lg text-danger"></span>
                                <%=L1.REACHEDHITS %>
                                <br />
                                <br />
                                <%=L1.FEELFREE %>
                                <asp:Literal ID="FeelFreeLiteral3" runat="server"></asp:Literal>
                            </asp:Panel>
                            <asp:Panel ID="MultiplePanel" runat="server" Visible="false" EnableViewState="true" ViewStateMode="Enabled">
                                <span class="fa fa-times fa-lg text-danger"></span>
                                <%=L1.YOUCANTMULTIPLE %>
                            </asp:Panel>

                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
                
                <div class="col-md-3 col-sm-6 ">                        
                    <div class="pull-left m-15 text-center" id="AdAdvertiserHolder" visible="false" runat="server">                            
                        <%=U6008.WEBSITEFROM %>:
                        <br />
                        <a runat="server" id="adAdvertiserLink" target="_blank">
                            <img id="adAdvertiserImage" runat="server" style="height: 100px;"/>
                        </a>
                        <br />                 
                        <asp:Literal ID="AdAdvertiserInfoLiteral" runat="server" />
                    </div>
                    <div class="pull-left m-15 text-center" id="BannerAdvertiserHolder" visible="false" runat="server">
                        <%=U6008.BANNERFROM %>:
                        <br />
                        <a runat="server" id="bannerAdvertiserLink" target="_blank">
                            <img id="bannerAdvertiserImage" runat="server" style="height: 100px;"/>
                        </a>
                        <br />
                        <asp:Literal ID="BannerAdvertiserInfoLiteral" runat="server" />
                    </div>                        
                </div>

                <div class="col-md-5 col-sm-12 pull-right text-right surf-banner">
                    <titan:Banner ID="BannerPanel" runat="server"></titan:Banner>
                </div>
                <div class="clear" style="clear: both;"></div>
                <asp:PlaceHolder runat="server" ID="ThumbnailsPlaceHolder" Visible="false">
                    <asp:Panel ID="ThumbnailsLiteral" runat="server" CssClass="surfThumbnails" Visible="false"></asp:Panel>
                </asp:PlaceHolder>
            </div>
        </div>

        <asp:Literal ID="AdFrame" runat="server"></asp:Literal>

    </form>
</body>
</html>


