<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FacebookConnect.ascx.cs" Inherits="Controls_FacebookLogin" %>


<asp:Panel ID="FacebookLoginPanel" runat="server" Visible="false">

    <script>

        window.fbAsyncInit = function () {
            FB.init({
                appId: '<%=AppSettings.Facebook.ApplicationId%>', // App ID
                status: true, // check login status
                cookie: true, // enable cookies to allow the server to access the session
                xfbml: true  // parse XFBML
            });

        };

        function fb_login() {
            FB.login(function (response) {

                if (response.status === 'connected') {
                    var uid = response.authResponse.userID;
                    var accessToken = response.authResponse.accessToken;

                    $.post("Handlers/FacebookConnect.ashx",
                    {
                        accessToken: accessToken,
                    },
                    function (data, status) {
                        if(status=='success')
                            window.location.reload(true);
                    });
                }

            }, {
                scope: 'public_profile,email'
            });
        }
    </script>

    <%=Prem.PTC.Members.FacebookMember.GetJSStartingCode() %>
</asp:Panel>
<asp:Literal ID="ControlLiteral" runat="server" ViewStateMode="Disabled"></asp:Literal>


