<%@ Page Language="C#" AutoEventWireup="true" CodeFile="asurf.aspx.cs" Inherits="About" EnableViewStateMac="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
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
    <link rel="stylesheet" href="Plugins/ProgressPieNoFrame/Style.css" type="text/css" />
    <script data-cfasync="false" src="Plugins/ProgressPieNoFrame/Script.js?v=4"></script>

    <script data-cfasync="false" src="Scripts/jquery.min.js"></script>
    <script data-cfasync="false">

        jQuery(function ($) {

            startTimer = function () {
                //Ad loaded, start progress bar
                var adTime = $('#adtime').val();
                $('#lblLoad').hide();
                startBar(adTime, '<%=Resources.U4200.TIMELEFT%>');

            }

        });

        function onCloseButtonClick() {
            window.opener.__doPostBack('AdRefreshUpdatePanel', '<%=HashedTrafficAd%>');
            window.close();
            return false;
        }

        window.onbeforeunload = function () {
            window.opener.__doPostBack('AdRefreshUpdatePanel', '<%=HashedTrafficAd%>');
        }

            function hideButton() {
                $('#<%=WatchAdButton.ClientID%>').hide();
            }
    </script>
    <title>
        <asp:Literal ID="SiteName" runat="server"></asp:Literal>
        | <%=Resources.U4000.ADVERTISEMENT %>
    </title>

    <asp:PlaceHolder runat="server" ID="TimerStylesPlaceHolder" Visible="false">
        <style>
            #RegisterUserValidationSummary {
                margin: 0 auto;
                color: rgb(205, 10, 10) !important;
            }

            .timer {
                position: relative;
                font-size: 60px;
                width: 2em;
                height: 2em;
                float: left;
            }

                .timer > .percent {
                    position: absolute;
                    top: 1em;
                    left: 0;
                    width: 3.33em;
                    font-size: 0.3em;
                    text-align: center;
                }

                .timer > #slice {
                    position: absolute;
                    width: 2em;
                    height: 2em;
                    clip: rect(0px,1em,1em,0.5em);
                }

                    .timer > #slice > .pie {
                        border: 0.1em solid #5cb85c;
                        osition: absolute;
                        width: 1em; /* 1 - (2 * border width) */
                        height: 1em; /* 1 - (2 * border width) */
                        clip: rect(0em,0.5em,1em,0em);
                        -moz-border-radius: 0.5em;
                        -webkit-border-radius: 0.5em;
                        border-radius: 0.5em;
                    }
        </style>
    </asp:PlaceHolder>

</head>
<body>
    <form id="form1" runat="server">
        <div style="width: 800px; margin: 0 auto; text-align: center; padding-top: 30vh;">
            <asp:PlaceHolder runat="server" ID="RedirectToBannersPlaceHolder" Visible="false">
                <p class="text-center"><a href="/user/advert/bannersoptions.aspx" target="_blank">Advertise Here</a></p>
            </asp:PlaceHolder>
            <asp:Literal ID="AdInfoContainer" runat="server"></asp:Literal>

            <div style="padding-left: 35px">
                <titan:Banner ID="BannerPanel" runat="server"></titan:Banner>
            </div>
            <div style="padding: 10px; height: <%=AnimatedHeight%>px">
                <img src="<%=AppSettings.Site.LogoImageURL %>" id="logo1" style="height: 50px" />


                <asp:Panel ID="BeforePanel" runat="server" Style="width: 200px; margin: 0 auto;">
                    <span id="lblLoad"><%=Resources.U4200.CLICKTOWATCH %>...</span>
                    <div style="width: 60px; margin: 0 auto;">
                        <div id="timerDiv" class="timer">
                            <br />
                        </div>
                    </div>
                    <br />
                    <br />
                    <div class="row">
                        <asp:Button ClientIDMode="Static" ID="WatchAdButton" runat="server" Text="Watch" CssClass="btn btn-lg btn-success btn-block" />
                    </div>
                </asp:Panel>

                <asp:Panel ID="CaptchaPanel" runat="server" Visible="false">

                    <%=Resources.U4000.CAPTCHAINFO %>:  
                    <asp:CustomValidator OnServerValidate="Validate_Captcha" runat="server" ValidationGroup="RegisterUserValidationGroup"
                        Display="None" ID="CustomValidator1" EnableClientScript="False" ForeColor="#b70d00">*</asp:CustomValidator>

                    <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="ui-state-error"
                        ValidationGroup="RegisterUserValidationGroup" DisplayMode="List" Width="400px" ForeColor="White" Visible="false" />

                    <titan:Captcha runat="server" ID="TitanCaptcha" ValidationGroup="RegisterUserValidationGroup" ValidatorDisplay="None" />
                    <div style="padding-top: 15px">
                        <asp:Button ID="CreditAfterCaptcha" ValidationGroup="RegisterUserValidationGroup" runat="server" CssClass="btn btn-lg btn-success btn-block" OnClick="CreditAfterCaptcha_Click" />
                    </div>

                </asp:Panel>

                <asp:Panel ID="AfterCounterPanel" runat="server" Visible="false">
                    <asp:Image ID="StatusImage" runat="server" CssClass="imagemiddle" />
                    <asp:Literal ID="InfoText" runat="server" />
                    <asp:Button ID="CloseRefreshButton" runat="server" CssClass="btn btn-lg btn-success btn-block" OnClientClick="onCloseButtonClick();" />
                    <br />
                    <br />
                    <%=Resources.L1.FEELFREE %>
                    <asp:Literal ID="FeelFreeLiteral" runat="server"></asp:Literal>
                </asp:Panel>


                <asp:Panel ID="MultiplePanel" runat="server" Visible="false">
                    <img src="Images/Misc/fail2mini.png" class="imagemiddle" />
                    <%=Resources.L1.YOUCANTMULTIPLE %>
                </asp:Panel>
            </div>
            <asp:PlaceHolder runat="server" ID="TokenAdsPlaceHolder" Visible="false">
                <div style="clear: both;"></div>
                <div id="SC_TBlock_441885" class="SC_TBlock" style="margin-top: 70px">loading...</div>
                <script type="text/javascript">var SC_CId = "441885", SC_Domain = "n.tckn-code.com"; SC_Start_441885 = (new Date).getTime();</script>
                <script type="text/javascript" src="//st-n.tckn-code.com/js/adv_out.js"></script>
            </asp:PlaceHolder>
        </div>

    </form>
</body>
</html>


