<%@ Page Language="C#" AutoEventWireup="true" CodeFile="surf.aspx.cs" Inherits="Api_Surf" EnableViewStateMac="false" %>

<!DOCTYPE html>
<html>
<head>
    <meta content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" name="viewport" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <base href="<%=BaseUrl %>" />

    <title>
        <%=AppSettings.Site.Name %>
        | <%=U4000.ADVERTISEMENT %></title>

    <link href="Scripts/default/assets/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="Scripts/default/assets/plugins/font-awesome/css/font-awesome.min.css" rel="stylesheet" />
    <link href="Scripts/assets/css/theme/default.css" id="theme" rel="stylesheet" />
    <link href="Scripts/default/assets/css/style-panel.min.css" rel="stylesheet" />
    <link href="Scripts/assets/css/style-responsive.min.css" rel="stylesheet" />
    <link href="Scripts/default/assets/css/custom.css" rel="stylesheet" />

    <style>
        iframe {
            width: 100%;
            height: 99vh;
            border: none;
        }

        .right-column {
            border-left: 1px solid #ddd;
        }

        .url-list-item-icon {
            text-shadow: 0px 0px 5px #00acac;
            color: #fff;
            -webkit-transition: all .5s;
            transition: all .5s;
        }

        .done .url-list-item-icon {
            text-shadow: none;
            color: #00acac;
        }

        .active .url-list-item-status {
            font-weight: 700;
        }

        .next-button.disabled:focus, 
        .next-button.disabled:hover {
            background: #ccc;

        }
    </style>
    <script>
        var isNotPostback = <%=(!Page.IsPostBack).ToString().ToLower()%>;

        if(isNotPostback){
            var jsonData = <%=JsonData%>;
            var autoSurfEnabled = jsonData.AutosurfEnabled;
            var displayTime = parseInt(jsonData.DisplayTime) * 1000;
            var iframeUrls = jsonData.urls;
        }

        successfulCaptcha =  <%=(SuccessfulCaptcha).ToString().ToLower()%>;

        if(successfulCaptcha) {
            window.opener.__doPostBack('PtcOfferWallsUpdatePanel', 'random');
            window.close();
        }
    </script>
    <script src="Scripts/default/assets/plugins/jquery/jquery-1.9.1.min.js"></script>
    <script src="Scripts/default/assets/plugins/jquery/jquery-migrate-1.1.0.min.js"></script>
    <script src="Scripts/default/assets/plugins/bootstrap/js/bootstrap.min.js"></script>
    <script src="Scripts/assets/js/jquery.timer.js"></script>
</head>
<body>
    <div class="container-fluid bg-white">
        
        
        <div class="row">
            <div class="col-md-3 left-column">
                <div class="row">
                    <div class="col-md-12">
                        <a href="<%=AppSettings.Site.Url%>" class="navbar-brand">
                            <span>
                                <img src="<%=AppSettings.Site.LogoImageURL %>" class="" style="height: 30px;" /></span>
                            <% if (AppSettings.Site.ShowSiteName)
                                { %>
                            <span><%=AppSettings.Site.Name %></span>
                            <% } %>
                        </a>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12">
                        <h4><strong class="text-center"><%=U6003.VIEWANDCOMPLETESLIDESHOW %></strong></h4>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12">
                        <span class="text-center"><%=U6003.SURFWARNING %></span>
                    </div>
                </div>
                <h4 class="text-center text-success">
                    <span class="total"></span>
                </h4>
                <h2 id="timer" class="text-center" style="height: 30px;"></h2>
                <p class="buttons text-right">
                    <button type="button" class="next-button btn btn-default disabled">Next <span></span></button>
                </p>
                <ul class="url-list list-unstyled m-l-20">
                </ul>
                <form runat="server" style="min-height: 150px">
                    <div id="CaptchaPanel" runat="server" style="display: none">
                        <div class="pull-left">
                            <asp:CustomValidator OnServerValidate="Validate_Captcha" runat="server" ValidationGroup="RegisterUserValidationGroup"
                                Display="None" ID="CustomValidator1" EnableClientScript="False" CssClass="text-danger">*</asp:CustomValidator>

                            <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="alert alert-danger"
                                ValidationGroup="RegisterUserValidationGroup" DisplayMode="List" Visible="false" />

                            <titan:Captcha runat="server" ID="TitanCaptcha" ValidationGroup="RegisterUserValidationGroup" ValidatorDisplay="None" />
                        </div>
                        <div class="pull-left captcha-banner-submit">
                            <asp:Button ID="CreditAfterCaptchaButton" ValidationGroup="RegisterUserValidationGroup" runat="server" CssClass="btn btn-default bg-green" OnClick="CreditAfterCaptcha_Click" />
                        </div>
                    </div>
                </form>
            </div>
            <div class="col-md-9 right-column">
                <iframe id="myFrame" src="#"></iframe>
            </div>
        </div>

    </div>

    <script>
        $(function() {
            var list = $(".url-list");
            iframeUrls.forEach(function(e, i) {
                list.append("<li id='url-" + i + "' class='url-list-item'><span class='url-list-item-icon fa fa-check fa-2x m-r-15'></span><span class='url-list-item-status'><%=U6002.UPNEXT %></span></li>");
            });
        });
    </script>
    <script defer>

        iframe = document.getElementById('myFrame');
        pagesLeft = iframeUrls.length;	

        timeUp = false;
        TimeLoader = new (function() {

            var $countdown,
            clickLog,
            incrementTime = 100,
            currentTime = displayTime,
            updateTimer = function() {
                $countdown.html(formatTime(currentTime));
                if (currentTime == 0) {
                    TimeLoader.Timer.stop();
                    TimeLoader.resetCountdown();
                    return;
                }
                currentTime -= incrementTime;
                if (currentTime < 0) currentTime = 0;
            },
            init = function() {
                iframe.src = iframeUrls[0];
                $countdown = $('#timer');
                clickLog = [];
                TimeLoader.Timer = $.timer(updateTimer, incrementTime, true);
                TimeLoader.Timer.stop();
                TimeLoader.resetCountdown();
            };
            this.resetCountdown = function() {
                var urlIndex = iframeUrls.length - pagesLeft;	
                if(pagesLeft > 0) {
                    if(autoSurfEnabled == true) {
                        iframe.src = jsonData.urls[urlIndex];
                        $("#timer").html('<span class="fa fa-spinner fa-pulse"></span>');
                        $(".next-button").remove();
                    } else {
                        clickLog[urlIndex] = Date.now();
                        if(urlIndex > 0) {
                            $(".next-button").removeClass("disabled").addClass("bg-green active").attr("data-next",urlIndex).attr("data-log", clickLog[urlIndex]);
                        }
                        $("span.total").text((urlIndex)  + '/' + iframeUrls.length);
                        $("#timer").html('<span class=""></span>');
                        timeUp = true;
                    }
                    
                    pagesLeft--;
                    
                } else {
                    $("#timer").html('<span class="fa fa-check-circle text-success"></span>');
                    $(".buttons .next-button").remove();
                    $("span.total").text(iframeUrls.length  + '/' + iframeUrls.length);
                    $("#<%=CaptchaPanel.ClientID %>").show();
                }
                $("#url-" + (urlIndex-1)).addClass('done').find(".url-list-item-status").text('<%=U6002.WATCHED %>');
                $(".url-list-item").removeClass('active');
                $("#url-" + urlIndex).addClass('active').find(".url-list-item-status").text('<%=U6002.WATCHING %>');

                currentTime = displayTime;
                //this.Timer.stop().once();

                $("body").on('click', '.buttons .next-button.disabled', function(e) {
                    e.preventDefault();
                });

                $("body").on('click', '.buttons .next-button.active', function(e) {
                    e.preventDefault();
                    $(this).addClass("disabled").removeClass("bg-green active");
                    var index = parseInt($(this).data('next'));
                    if(TimeLoader.getClickLog(parseInt($(this).data('next'))) == $(this).data('log')) {
                        iframe.src = jsonData.urls[urlIndex];
                    }   
                });
            };

            this.getClickLog = function(index) {
                return clickLog[index];
            };
            $(init);

            

        });
			
        iframe.onload = function() {
            timeUp = false;
            TimeLoader.Timer.play(true);
            $(".next-button").removeClass("bg-green active").addClass("disabled");
            
        };
			
        isActive = true;

        window.onfocus = function () { 
            isActive = true; 
            if(pagesLeft > 0 && timeUp == false) {
                TimeLoader.Timer.play();
            }
        }; 

        window.onblur = function () { 
            isActive = false; 
            if(pagesLeft > 0 && timeUp == false) {
                TimeLoader.Timer.pause();
            }
        }; 

        function pad(number, length) {
            var str = '' + number;
            while (str.length < length) {str = '0' + str;}
            return str;
        }
			
        function formatTime(time) {
            time = time / 10;
            var min = parseInt(time / 6000),
                sec = parseInt(time / 100) - (min * 60),
                hundredths = pad(time - (sec * 100) - (min * 6000), 2);
            return (min > 0 ? pad(min, 2) : "00") + ":" + pad(sec, 2) + ":" + hundredths;
        }

	</script>
</body>
</html>


