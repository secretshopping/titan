<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UseTitanDemoHeader.ascx.cs" Inherits="Controls_UseTitanDemoHeader" %>
<link href="Scripts/default/assets/plugins/gritter/css/jquery.gritter.css" rel="stylesheet" />
<style>
    #gritter-notice-wrapper {
        /*top: unset;
        bottom: 20px;*/
        display: none;
    }

    .gritter-title {
        padding-bottom: 5px !important;
    }

    .theme-name {
        color: #fff;
    }

    .gritter-item p {
        line-height: 20px;
    }

    .gritter-item img {
        height: 18px;
        margin-top: -2px;
        width: auto !important;
    }
</style>
<script>
    $(function () {
        if (localStorage.getItem("showDemoInfo") != "false") {
            $("#gritter-notice-wrapper").fadeIn();
        }
        $(".gritter-close").click(function (event) {
            event.preventDefault();
            $("#gritter-notice-wrapper").fadeOut();
            localStorage.setItem("showDemoInfo", "false");
        });
    });
</script>
<asp:PlaceHolder ID="GlobalPlaceHolder" runat="server" Visible="false">
    <div id="gritter-notice-wrapper">

        <div class="gritter-item-wrapper" style="opacity: 0.92632;" role="alert">
            <div class="gritter-top"></div>
            <div class="gritter-item">
                <a class="gritter-close" href="#" tabindex="1" style="display: none;">Close Notification</a>
                <div class="gritter-without-image">
                    <span class="gritter-title">You are now watching demo of </span>
                    <p>
                        <asp:Literal ID="MainTextLiteral" runat="server"></asp:Literal></p>
                </div>
                <div style="clear: both"></div>
            </div>
            <div class="gritter-bottom"></div>
        </div>

    </div>
</asp:PlaceHolder>
