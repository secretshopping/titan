<%@ Page Language="C#" AutoEventWireup="true" CodeFile="stages.aspx.cs" Inherits="user_ico_info" MasterPageFile="~/User.master" %>

<%@ MasterType VirtualPath="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script src="Plugins/jQueryKnob/jquery.knob.min.js"></script>
    <script src="Plugins/Countdown/jquery.countdown.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.20.1/moment.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/moment-timezone/0.5.14/moment-timezone.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/moment-timezone/0.5.14/moment-timezone-with-data.min.js"></script>
    <script>
        $(function() {
            $(".knob").knob({
                'format': function (value) {
                    return value + '%';
                }, 
                'readOnly': true
            });

            $(".ICOtimer .countdown").each(function (i, elem) {

                var date = $(elem).attr("data-timestamp");
                var datetz = moment.tz(date, "MM-DD-YYYY HH:mm:ss A", "Europe/Berlin");

                $(elem).countdown(datetz.toDate(), {}).on('update.countdown', function (event) {
                    if (event.offset.totalDays) {
                        $(this).find(".countdown-placeholder").text(event.strftime('%D days %H hours %M minutes %S seconds'));
                    } else {
                        if (event.offset.totalHours) {
                            $(this).find(".countdown-placeholder").text(event.strftime('%H hours %M minutes %S seconds'));
                        } else {
                            if (event.offset.totalMinutes) {
                                $(this).find(".countdown-placeholder").text(event.strftime('%M minutes %S seconds'));
                            } else {
                                $(this).find(".countdown-placeholder").text(event.strftime('%S seconds'));
                            }
                        }
                    }
                });
            });
        });
    </script>

    <style>
    .ICOStage {
        background-color: white;
        margin-bottom: 20px;
        border: 1px solid #ddd;
        color: black;
        padding: 15px 0;
    }
    .tokenImage {
        margin: 5px;
        height: 20px;
        float: left;
    }
    .stagePriceLabel {
        font-size: 16px;
        color: #bbb;
    }
    .availableTokensLabel {
        font-size: 16px;
        color: #bbb;
    }
    .stagePriceValue {
        font-size: 20px;
        color: #555;
        font-weight: bold;
    }
    .availableTokensValue {
        font-size: 20px;
        color: #555;
        font-weight: bold;
        float: left;
    }
    .ICOStage h5 {
        font-weight: bold;
        font-size: 20px;
    }
    .countdown>span {
        font-weight: 400;
        font-size: 14px;
        color: #aaa;
    }
</style>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=U6012.ICO %> <%=U6012.STAGES %></h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=U6012.ICOSTAGESDESCRIPTION %></p>
        </div>
    </div>

    <asp:PlaceHolder ID="NoStagePlaceHolder" runat="server" Visible="false">
        <p class="alert alert-warning">
            <asp:Literal ID="NoStageLiteral" runat="server" />
        </p>
    </asp:PlaceHolder>

    <div class="tab-content">

        <asp:PlaceHolder ID="AllStagesLiteral" runat="server" />

    </div>

    
</asp:Content>
