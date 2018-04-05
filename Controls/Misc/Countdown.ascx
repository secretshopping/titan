<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Countdown.ascx.cs" Inherits="Controls_Countdown" %>

<%@ Import Namespace="ExtensionMethods" %>

<asp:PlaceHolder runat="server" ID="CountDownPlaceHolder" ClientIDMode="Static">
    <style>
        #CountDownPlaceHolder {
            text-align: center;
            font-family: sans-serif;
            font-weight: 100;
        }

        .clockdiv {
            z-index: 100001;
            font-family: sans-serif;
            color: #fff;
            display: inline-block;
            font-weight: 100;
            text-align: center;
            font-size: 30px;
            position: fixed;
            left: 0;
            bottom:0;
            right:0;
            padding: 50px;
            background-color: rgba(82, 90, 88, 0.80);
        }

            .clockdiv > div {
                padding: 30px 10px;
                border-radius: 3px;
                display: inline-block;
            }

        .smalltext {
            padding-top: 5px;
            font-size: 16px;
        }

        #description {
            vertical-align: bottom;
        }

        #dismissButton {
            position: absolute;
            right: 15px;
            top: 15px;
            cursor: pointer;
            width: 24px;
            margin-right: 15px;
        }
    </style>
    <asp:HiddenField ID="DeadLineHiddenField" runat="server" ClientIDMode="Static" />
    <div class="clockdiv row" id="clockdiv">
        <div id="description" class="col-md-8 col-md-offset-2">
            <div style="margin-top: 10px;">
                <span><%=AppSettings.Addons.CustomCounterTitle %></span>
            </div>
            <div class="row" style="margin-top: 20px;">
                <div class="col-md-3">
                    <div>
                        <span class="days"></span>
                        <div class="smalltext"><%=L1.DAYS %></div>
                    </div>
                </div>
                <div class="col-md-3">
                    <span class="hours"></span>
                    <div class="smalltext"><%=U5005.HOURS %></div>
                </div>
                <div class="col-md-3">
                    <span class="minutes"></span>
                    <div class="smalltext"><%=U5005.MINUTES %></div>
                </div>
                <div class="col-md-3">
                    <span class="seconds"></span>
                    <div class="smalltext"><%=L1.SECONDS %></div>
                </div>
            </div>
        </div>
        
        <img src="../../Images/Icons/close.png" id="dismissButton" onclick="dismiss(); return false;" />
    </div>

    <script type="text/javascript">
        
        serverTime = Date.parse(new Date("<%=AppSettings.ServerTime %>"));

        function getTimeRemaining(endtime) {
                //var t = Date.parse(endtime) - Date.parse(new Date('<%=DateTime.Now.ToDBString()%>'));
                var t = Date.parse(endtime) - serverTime;
                var seconds = Math.floor((t / 1000) % 60);
                var minutes = Math.floor((t / 1000 / 60) % 60);
                var hours = Math.floor((t / (1000 * 60 * 60)) % 24);
                var days = Math.floor(t / (1000 * 60 * 60 * 24));
                if (days < 0) {
                    days = 0;
                    hours = 0;
                    minutes = 0;
                    seconds = 0;
                }
                return {
                    'total': t,
                    'days': days,
                    'hours': hours,
                    'minutes': minutes,
                    'seconds': seconds
                };
            }

            function initializeClock(id, endtime) {
                var clock = document.getElementById(id);
                var daysSpan = clock.querySelector('.days');
                var hoursSpan = clock.querySelector('.hours');
                var minutesSpan = clock.querySelector('.minutes');
                var secondsSpan = clock.querySelector('.seconds');

                function updateClock() {
                    var t = getTimeRemaining(endtime);

                    daysSpan.innerHTML = t.days;
                    hoursSpan.innerHTML = ('0' + t.hours).slice(-2);
                    minutesSpan.innerHTML = ('0' + t.minutes).slice(-2);
                    secondsSpan.innerHTML = ('0' + t.seconds).slice(-2);
                    if (t.total <= 0) {
                        clearInterval(timeinterval);
                    }
                    serverTime += 1000;
                }

                updateClock();
                var timeinterval = setInterval(updateClock, 1000);
            }

       
                //var deadline = new Date(Date.parse(new Date()) + 15 * 24 * 60 * 60 * 1000);
                var deadline = new Date($("#DeadLineHiddenField").val());

                initializeClock('clockdiv', deadline);

                var dismiss = function () {
                    $("#clockdiv").remove();
                }

            
       
</script>

</asp:PlaceHolder>
