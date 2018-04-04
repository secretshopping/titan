<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Sites.master"
    CodeFile="login.aspx.cs" Inherits="About" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContentPlaceHolder">

    <asp:HiddenField ID="LoginAd" runat="server" ClientIDMode="Static" />
    <input type="hidden" name="__EVENTARGUMENT5" id="__EVENTARGUMENT5" value="" />

    <asp:Panel ID="LoginPanel" runat="server" CssClass="loginbox">
        <div class="container">
            <!-- begin #page-container -->
            <div id="page-container" class="row">
                <!-- begin login -->
                <div class="col-md-4 col-md-offset-4">
                    <div class="login-panel">
                        <h1 class="header text-center"><%=L1.LOGIN %></h1>
                        <titan:Login runat="server"></titan:Login>
                    </div>
                </div>
                <!-- end login -->

            </div>
            <!-- end page container -->
        </div>
    </asp:Panel>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="FooterContentPlaceHolder">
    <script type="text/javascript">
        function TestValidation() {

            //This is executed after your Validation

            if (Page_IsValid) {
                //Validation is successful
                $('#Panel1').hide();
                $('#loadingPanel').show();
            }
            else {
                //Validation Failed
            }
        }

    </script>
    <asp:PlaceHolder ID="DemoInfoPlaceholder" runat="server" Visible="false">
        <script>
            $(function () {
                localStorage.setItem("showDemoInfo", "true");
            });
        </script>
    </asp:PlaceHolder>
</asp:Content>
