<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Sites.master"
    CodeFile="resetpwd.aspx.cs" Inherits="About" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContentPlaceHolder">

    <asp:Panel ID="ResetPanel1" runat="server">

        <div class="container">
            <!-- begin #page-container -->
            <div id="page-container" class="row">
                <!-- begin login -->
                <div class="col-md-4 col-md-offset-4">
                    <div class="login-panel">
                        <h1 class="header text-center"><asp:Label ID="HeaderLabel1" runat="server" Text=""></asp:Label></h1>

                        <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger">
                            <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                        </asp:Panel>
                        <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="alert alert-success">
                            <asp:Literal ID="SuccMessage" runat="server"></asp:Literal>
                        </asp:Panel>

                        <asp:ValidationSummary ID="ResetValidationSummary" runat="server" CssClass="alert alert-danger"
                            ValidationGroup="ResetValidationGroup" DisplayMode="List" />

                        <div id="usernameTr" runat="server" class="form-group m-b-20">
                            <asp:TextBox ID="Username" runat="server" CssClass="form-control input-lg"></asp:TextBox>

                            <asp:RegularExpressionValidator ValidationGroup="ResetValidationGroup" ID="RegularExpressionValidator1" runat="server"
                                ValidationExpression="[a-zA-Z][a-zA-Z0-9]{3,20}" Display="Dynamic" CssClass="text-danger" ControlToValidate="Username">*</asp:RegularExpressionValidator>
                            <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="Username"
                                Display="Dynamic" CssClass="text-danger"
                                ValidationGroup="ResetValidationGroup">*</asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group m-b-20">
                            <asp:TextBox ID="Email" runat="server" CssClass="form-control input-lg"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                                Display="Dynamic" CssClass="text-danger"
                                ValidationGroup="ResetValidationGroup">*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator runat="server"
                                ValidationGroup="ResetValidationGroup" Display="Dynamic" CssClass="text-danger"
                                Text="*" ID="CorrectEmailRequired" ControlToValidate="Email"
                                ValidationExpression="^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$">*</asp:RegularExpressionValidator>

                        </div>

                        <div id="pinTr" runat="server" class="form-group m-b-20">
                            <div class="row">
                                <div class="col-md-6">
                                    <asp:TextBox ID="PIN" runat="server" CssClass="form-control input-lg" MaxLength="4" autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <asp:RegularExpressionValidator ValidationGroup="ResetValidationGroup" ID="RegularExpressionValidator3" runat="server"
                                ValidationExpression="[0-9]{4}" Display="Dynamic" CssClass="text-danger" ControlToValidate="PIN">*</asp:RegularExpressionValidator>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="PIN"
                                ValidationGroup="ResetValidationGroup" Display="Dynamic" CssClass="text-danger">*</asp:RequiredFieldValidator>

                        </div>

                        <div class="m-b-20">

                            <label><%=L1.VERIFICATION %>:</label>
                            <asp:CustomValidator OnServerValidate="Validate_Captcha1" runat="server" ValidationGroup="ResetValidationGroup"
                                Display="Dynamic" CssClass="text-danger" ID="CustomValidator1" EnableClientScript="False">*</asp:CustomValidator>


                            <titan:Captcha runat="server" ID="TitanCaptcha1" ValidationGroup="ResetValidationGroup" />

                        </div>


                        <div class="login-buttons">
                            <asp:Button ID="ResetButton" runat="server"
                                ValidationGroup="ResetValidationGroup" CssClass="btn btn-success btn-block btn-lg" OnClick="ResetButton_Click" />

                        </div>


                    </div>
                </div>
            </div>
        </div>


    </asp:Panel>

    <asp:Panel ID="ResetPanel2" runat="server" Visible="false">

        <div class="container">
            <!-- begin #page-container -->
            <div id="page-container" class="row">
                <!-- begin login -->
                <div class="col-md-4 col-md-offset-4">
                    <div class="login-panel">
                        <h1 class="header text-center"><asp:Label ID="HeaderLabel2" runat="server" Text=""></asp:Label></h1>

                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger"
                            ValidationGroup="RegisterUserValidationGroup" DisplayMode="List" />

                        <div class="form-group m-b-20">
                            <asp:TextBox ID="Password" runat="server" CssClass="form-control input-lg" TextMode="Password"></asp:TextBox>

                            <asp:RegularExpressionValidator Display="Dynamic" CssClass="text-danger" ValidationGroup="RegisterUserValidationGroup" ID="RegularExpressionValidator2" runat="server" ErrorMessage="*"
                                ValidationExpression="[a-zA-Z0-9!A#\$%\^&\*\(\)=\+\.\,]{4,81}" ControlToValidate="Password" Text="">*</asp:RegularExpressionValidator>
                            <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                Display="Dynamic" CssClass="text-danger"
                                ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group m-b-20">
                            <asp:TextBox ID="ConfirmPassword" runat="server" CssClass="form-control input-lg" TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="ConfirmPassword" Display="Dynamic" CssClass="text-danger"
                                ID="ConfirmPasswordRequired" runat="server"
                                ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword"
                                Display="Dynamic" CssClass="text-danger"
                                ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:CompareValidator>

                        </div>

                        <asp:PlaceHolder ID="ResetPINPlaceHolder" runat="server">
                            <div class="form-group m-b-20">
                                <asp:TextBox ID="PINTextBox" runat="server" CssClass="form-control input-lg"></asp:TextBox>

                                <asp:RegularExpressionValidator Display="Dynamic" CssClass="text-danger" ValidationGroup="RegisterUserValidationGroup" ID="RegularExpressionValidator4" runat="server" ErrorMessage="*"
                                    ValidationExpression="[0-9]{4}" ControlToValidate="PINTextBox" Text="">*</asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="PINTextBox"
                                    Display="Dynamic" CssClass="text-danger"
                                    ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group m-b-20">
                                <asp:TextBox ID="ConfirmPINTextBox" runat="server" CssClass="form-control input-lg"></asp:TextBox>
                                <asp:RequiredFieldValidator ControlToValidate="ConfirmPINTextBox" Display="Dynamic" CssClass="text-danger"
                                    ID="RequiredFieldValidator3" runat="server"
                                    ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="PINTextBox" ControlToValidate="ConfirmPINTextBox"
                                    Display="Dynamic" CssClass="text-danger"
                                    ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:CompareValidator>

                            </div>
                        </asp:PlaceHolder>

                        <div class="m-b-20">

                            <label><%=L1.VERIFICATION %>:</label>
                            <asp:CustomValidator OnServerValidate="Validate_Captcha2" runat="server" ValidationGroup="RegisterUserValidationGroup"
                                Display="Dynamic" CssClass="text-danger" ID="CustomValidator2" EnableClientScript="False">*</asp:CustomValidator>

                            <titan:Captcha runat="server" ID="TitanCaptcha2" ValidationGroup="RegisterUserValidationGroup" />

                        </div>

                        <div class="login-buttons">
                            <asp:Button ID="Button1" runat="server"
                                ValidationGroup="RegisterUserValidationGroup" CssClass="btn btn-success btn-block btn-lg" OnClick="ResetButton2_Click" />

                        </div>


                    </div>
                </div>
            </div>
        </div>


    </asp:Panel>

</asp:Content>
