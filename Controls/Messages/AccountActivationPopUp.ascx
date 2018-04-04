<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AccountActivationPopUp.ascx.cs" Inherits="Controls_AccountActivationPopUp" %>

<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/animate.css/3.5.2/animate.min.css" />
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/limonte-sweetalert2/7.0.3/sweetalert2.min.css" />
<script src="https://cdnjs.cloudflare.com/ajax/libs/limonte-sweetalert2/7.0.3/sweetalert2.min.js"></script>

<style>

    @media screen and (max-width: 768px) {
        .swal2-popup {
            width: 90% !important;
        }

        .swal2-buttonswrapper {
            display: block !important;
        }
    }
</style>

<script>
    $(function () {

        /* Black Friday PopUp */
        if (sessionStorage.getItem("showAccountActivationFeePopUp") != "false") {
            swal({
                title: '<%=PopUpTitle %>',
                html: '<%=PopUpHtmlBody %>',
                animation: false,
                showCloseButton: true,
                showCancelButton: true,
                customClass: 'animated tada',
                type: 'warning',
                width: '50%',
                confirmButtonText: '<%=PopUpButtonTextConfirm %>',
                cancelButtonText: '<%=PopUpButtonTextCancel %>',
                buttonsStyling: false,
                confirmButtonClass: 'btn btn-success',
                cancelButtonClass: 'btn btn-link',
            }).then(function (result) {
                if (result.value) {
                    window.location.href = "sites/activation.aspx";
                } 
            });
        }

    });
</script>

<asp:Panel runat="server" ID="PopUpMessage">
    <%--PopUp--%>
</asp:Panel>
