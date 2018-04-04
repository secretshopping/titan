<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Captcha.ascx.cs" Inherits="Controls_Captcha" %>


<!-- Google Captcha -->
<asp:Literal ID="CaptchaLiteral" runat="server"></asp:Literal>


<!-- Titan Captcha -->
<asp:PlaceHolder ID="TitanCaptchaPlaceHolder" runat="server" Visible="false">

    <style>
        .captcha-status {
            -webkit-border-radius: 7px;
            -moz-border-radius: 7px;
            -ms-border-radius: 7px;
            -o-border-radius: 7px;
            border-radius: 7px;
            background-color: #fbb6a9;
            padding: 10px;
            text-align: center;
            font-family: 'Oxygen', sans-serif;
            font-size: 18px;
            color: #b41c1c;
            font-weight: 100;
            margin: 20px 0 24px;
        }

        @media only screen and (max-width: 360px) {
            .captcha-status {
                font-size: 16px;
            }
        }

        .captcha-status p {
            margin: 0;
        }
    </style>

    <link rel="stylesheet" href="Plugins/VisualCaptcha/visualcaptcha.css" media="all" />
    <link rel="stylesheet" href="Scripts/default/assets/css/captcha-customize.css" media="all" />
    <link rel="stylesheet" <%="href='Themes/" + AppSettings.Site.Theme + "/css/captcha/captcha.css'"%> />
    <script src="Plugins/VisualCaptcha/visualcaptcha.jquery.js?v=2"></script>

    <input type="hidden" name="captcha-value" id="captcha-value" />

    <div id="captcha-status-message" style="display: none">
        <div id="captcha-status" class="captcha-status">
            <div id="captcha-status-icon"></div>
            <p id="captcha-status-text"></p>
        </div>
    </div>

    <div id="sample-captcha"></div>

    <asp:CustomValidator ID="CaptchaCheckedCustomValidator" runat="server" ClientIDMode="Static"
        ClientValidationFunction="checkCaptchaCompleted"></asp:CustomValidator>

    <asp:PlaceHolder ID="TitanCaptchaJavascriptPlaceHolder" runat="server">

        <script>

            var Main = (function () {
                function Main() {
                    this.$captchaContainer = $('#sample-captcha');
                    this.$form = $("form");
                    this.$statusContainer = $("#captcha-status");
                    this.$statusIcon = $("#captcha-status-icon");
                    this.$statusText = $("#captcha-status-text");
                    this.$statusMessage = $("#captcha-status-message");
                    this.initializeCaptcha();
                    this.bindHandlers();
                }
                Main.prototype.initializeCaptcha = function () {
                    this.captcha = this.$captchaContainer.visualCaptcha({
                        imgPath: '/<%=Page.ResolveClientUrl("~/Plugins/VisualCaptcha/")%>img/',
                        captcha: {
                            numberOfImages: 4,
                            routes: {
                                start: '/<%=Page.ResolveClientUrl("~/Plugins/VisualCaptcha/")%>Handler.ashx/start',
                                image: '/<%=Page.ResolveClientUrl("~/Plugins/VisualCaptcha/")%>Handler.ashx/image',
                                audio: '/<%=Page.ResolveClientUrl("~/Plugins/VisualCaptcha/")%>Handler.ashx/audio'
                            },
                            namespace: '<%=HashingManager.GenerateMD5(HttpContext.Current.Request.Url.AbsolutePath)%>'
                        },
                        language: {
                            accessibilityAlt: '<%=U6002.SOUNDICON%>',
                            accessibilityTitle: '<%=U6002.ACC1%>',
                            accessibilityDescription: '<%=U6002.TYPEBELOW %>',
                            explanation: '<%=String.Format(U6002.CLICKORTOUCH, "<strong>ANSWER</strong>") %>',
                            refreshAlt: '<%=U6002.REFRESHICON %>',
                            refreshTitle: '<%=U6002.REFRESHIMAGE %>'
                        }

                    }).data("captcha");
                };

                Main.prototype.bindHandlers = function () {
                    var _this = this;
                    // Bind form submission behavior
                    this.$form.submit(function () {
                        if (_this.captcha.getCaptchaData().valid) {
                            $('#captcha-value').val(_this.captcha.getCaptchaData().value);
                        }
                    });


                    WebForm_OnSubmit = function () {
                        if (typeof (ValidatorOnSubmit) == "function" && ValidatorOnSubmit() == false) return false;

                        try {
                            if (_this.captcha.getCaptchaData().valid) {
                                $('#captcha-value').val(_this.captcha.getCaptchaData().value);
                            }
                        } catch (Exception) { }

                        return true;
                    }

                };

                Main.prototype.setStatus = function (result) {
                    if (result.success) {
                        this.$statusContainer.addClass("valid");
                    } else {
                        this.$statusContainer.removeClass("valid");
                    }

                    this.$statusIcon.removeClass().addClass(result.success ? "icon-yes" : "icon-no");
                    this.$statusText.text(result.message);
                    this.$statusMessage.show();
                };

                return Main;
            })();


            var captchaObject = new Main();

            function checkCaptchaCompleted(source, arguments) {
                ShowValidationSummaries();
                arguments.IsValid = true;
                if (captchaObject.captcha.getCaptchaData().valid == false) {
                    captchaObject.setStatus({ success: false, message: "<%=U6000.REQCAPTCHA%>" });
                    arguments.IsValid = false;
                    HideValidationSummaries();
                }
            }

            function HideValidationSummaries() {
                if (typeof (Page_ValidationSummaries) != "undefined") {
                    for (sums = 0; sums < Page_ValidationSummaries.length; sums++) {
                        summary = Page_ValidationSummaries[sums];
                        summary.className += " displaynone";
                    }
                }
            }

            function ShowValidationSummaries() {
                if (typeof (Page_ValidationSummaries) != "undefined") {
                    for (sums = 0; sums < Page_ValidationSummaries.length; sums++) {
                        summary = Page_ValidationSummaries[sums];
                        summary.className = summary.className.replace(/(?:^|\s)displaynone(?!\S)/g, '');
                    }
                }
            }

        </script>
    </asp:PlaceHolder>



</asp:PlaceHolder>


<!-- SolveMedia -->
<asp:PlaceHolder ID="SolveMediaPlaceHolder" runat="server" Visible="false">

    <script type="text/javascript"
        src="<%=SolveMediaURL %>/papi/challenge.script?k=<%=AppSettings.Captcha.SolveMediaCKey %>">
    </script>

    <noscript>
        <iframe src="<%=SolveMediaURL %>/papi/challenge.noscript?k=<%=AppSettings.Captcha.SolveMediaCKey %>"
            height="300" width="500" frameborder="0"></iframe>
        <br />
        <textarea name="adcopy_challenge" rows="3" cols="40">     </textarea>
        <input type="hidden" name="adcopy_response"
            value="manual_challenge" />
    </noscript>

</asp:PlaceHolder>

<!-- Coinhive -->
<asp:PlaceHolder ID="CoinhivePlaceHolder" runat="server" Visible="false">

    <input type="hidden" name="coinhive-token" id="coinhive-token" />

	<script src="https://authedmine.com/lib/captcha.min.js" async></script>
	<script>
            function coinhiveCaptchaCallback(token) {
                $('#coinhive-token').val(token);
            }
	</script>
    
	<div class="coinhive-captcha" 
		data-hashes="<%=AppSettings.Captcha.CoinhiveHashes %>"
		data-key="<%=AppSettings.Captcha.CoinhiveSiteKey %>"
		data-whitelabel="false"
		data-callback="coinhiveCaptchaCallback">
		<em>Loading Captcha...<br>
		If it doesn't load, please disable Adblock!</em>
	</div>

</asp:PlaceHolder>
