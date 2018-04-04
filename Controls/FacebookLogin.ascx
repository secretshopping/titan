<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FacebookLogin.ascx.cs" Inherits="Controls_FacebookLogin" %>


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

        //this function is never used...
        function checkLoginState() {
            FB.getLoginStatus(function (response) {

                if (response.status === 'connected') {
                    var uid = response.authResponse.userID;
                    var accessToken = response.authResponse.accessToken;

                    // Do a post to the server to finish the logon
                    // This is a form post since we don't want to use AJAX
                    var form = document.getElementById("<%=((Page)(HttpContext.Current.Handler)).Form.ClientID%>");
                    form.setAttribute("method", 'post');
                    form.setAttribute("action", 'login.aspx?fb=1');

                    var field = document.createElement("input");
                    field.setAttribute("type", "hidden");
                    field.setAttribute("name", 'accessToken');
                    field.setAttribute("value", accessToken);
                    form.appendChild(field);

                    document.body.appendChild(form);
                    form.submit();
                }
            });
        }

   
        function fb_login() {
            FB.login(function (response) {

                if (response.status === 'connected') {
                    var uid = response.authResponse.userID;
                    var accessToken = response.authResponse.accessToken;

                    // Do a post to the server to finish the logon
                    // This is a form post since we don't want to use AJAX
                    var form = document.getElementById("<%=((Page)(HttpContext.Current.Handler)).Form.ClientID%>");
                    form.setAttribute("method", 'post');
                    form.setAttribute("action", 'login.aspx?fb=1');

                    var field = document.createElement("input");
                    field.setAttribute("type", "hidden");
                    field.setAttribute("name", 'accessToken');
                    field.setAttribute("value", accessToken);
                    form.appendChild(field);

                    document.body.appendChild(form);
                    form.submit();
                }

            }, {
                scope: 'public_profile,email'
            });
        }
    </script>

    <%=Prem.PTC.Members.FacebookMember.GetJSStartingCode() %>

    <asp:Literal ID="ControlLiteral" runat="server"></asp:Literal> 

</asp:Panel>

