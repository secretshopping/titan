<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Login.ascx.cs" Inherits="Controls_Login" %>
<%@ Register Src="~/Controls/Captcha.ascx" TagPrefix="titan" TagName="Captcha" %>
<%@ Register Src="~/Controls/FacebookLogin.ascx" TagPrefix="titan" TagName="FacebookLogin" %>

<asp:Panel ID="LoginPanel" runat="server" CssClass="loginbox" DefaultButton="LoginButton">
    <div class="login-content">

    <p class="alert alert-danger m-b-10" id="FailureP" runat="server" visible="false">
        <asp:Label ID="FailureText" runat="server"></asp:Label>
    </p>

    <p class="alert alert-success m-b-10" id="SuccesP" runat="server" visible="false">
        <asp:Label ID="SuccesText" runat="server"/>
    </p>

    <div class="form-group m-b-20">
        <asp:TextBox ID="Username" runat="server" CssClass="form-control input-lg" placeholder="Username" autocomplete="off"></asp:TextBox>

        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="Username"
            CssClass="" ErrorMessage="" ToolTip=""
            ValidationGroup="LoginUserValidationGroup" Text="" ForeColor="#CC0000"></asp:RequiredFieldValidator>
    </div>
    <div class="form-group m-b-20">
        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server"
            ControlToValidate="Password" ErrorMessage="" ToolTip=""
            ValidationGroup="LoginUserValidationGroup" Text=""></asp:RequiredFieldValidator>
        <asp:TextBox ID="Password" runat="server" CssClass="form-control input-lg"
            TextMode="Password" placeholder="Password"></asp:TextBox>
    </div>
    <div class="form-group m-b-20" style="<%=Prem.PTC.AppSettings.Authentication.EnableSecondaryPassword? "": "display:none;"%>">
        <asp:TextBox ID="Password2" runat="server" CssClass="form-control input-lg" placeholder="Secondary password"></asp:TextBox>
    </div>
    <%--<div class="checkbox m-b-20">
                <label>
                    <input type="checkbox" /> Remember Me
                </label>
            </div>--%>
    <asp:Panel ID="CaptchaPanel1" runat="server" Visible="false" CssClass="m-b-20">

        <label><%=L1.VERIFICATION %>:</label>
        <asp:CustomValidator OnServerValidate="Validate_Captcha" runat="server" ValidationGroup="LoginUserValidationGroup"
            Display="Dynamic" ID="CustomValidator1" EnableClientScript="False">*</asp:CustomValidator>

        <titan:Captcha runat="server" ID="TitanCaptcha" ValidationGroup="LoginUserValidationGroup" />

    </asp:Panel>
    <asp:ValidationSummary ID="LoginUserValidationSummary" runat="server"
        ValidationGroup="LoginUserValidationGroup"
        Font-Bold="False"
        DisplayMode="SingleParagraph" HeaderText=""
        CssClass="alert alert-danger m-t-20" ToolTip="" />
    <div class="login-buttons">

        <asp:Button ID="LoginButton" runat="server" OnClick="LoginButton_Click"
            ValidationGroup="LoginUserValidationGroup" CssClass="btn btn-success btn-block btn-lg" OnClientClick="window.setTimeout(TestValidation,0);" />

        <asp:Button ID="ResendEmailButton" runat="server" OnClick="ResendEmailButton_Click" CssClass="btn btn-success btn-block btn-lg" Visible="false"/>

        <asp:Button ID="ReactivateButton" runat="server" OnClick="ReactivateButton_Click" Visible="false"
            ValidationGroup="LoginUserValidationGroup" CssClass="btn btn-success btn-block btn-lg" Text="Reactivate"
            OnClientClick="return confirm('Are you sure you want to reactivate this account? All balances and referrals will be lost.');" />

        <div class="m-b-10">
            <titan:FacebookLogin ID="FacebookLogin1" runat="server" />
        </div>

    </div>
    <div class="m-t-20">
        <asp:HyperLink ID="ForgotMyPasswordHyperLink" NavigateUrl="~/resetpwd.aspx" runat="server" ></asp:HyperLink>
    </div>
    <div class="m-t-20">
        <%=String.Format(U6000.NOTAMEMBERYET, "<a href='register.aspx'>", "</a>") %>
    </div>

    </div>
</asp:Panel>