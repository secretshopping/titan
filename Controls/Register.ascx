<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Register.ascx.cs" Inherits="Controls_Register" %>
<%@ Register Src="~/Controls/Captcha.ascx" TagPrefix="titan" TagName="Captcha" %>
<%@ Register Src="~/Controls/FacebookLogin.ascx" TagPrefix="titan" TagName="FacebookLogin" %>


<style>
    .img-check {
        width: 100%;
    }

    .check {
        opacity: 0.7;
    }
</style>

<script type="text/javascript">
    function checkBox(source, args) {
        var Achecked = $('#checkedTerms').prop('checked') ? true : false;
        if (Achecked == true) {
            args.IsValid = true;
        } else {
            args.IsValid = false;
        }
    }
    function validateRegistrationType(source, args) {
        //var oneChecked = $(".registration-type-checkbox input[type=checkbox]").prop("checked") ? true : false;
        var oneChecked = false;
        $(".registration-type-checkbox input[type=checkbox]").each(function () {
            if ($(this).prop('checked')) {
                oneChecked = true;
            }
        });

        if (oneChecked == true) {
            args.IsValid = true;
        } else {
            args.IsValid = false;
        }
    }

    $(function () {
        $(".img-check").on("click", function () {
            $(this).toggleClass("check");
        });
        $(".registration-type-icon p").on("click", function () {
            $(this).parent().find("span.icon:visible").toggleClass("check");
        });
        $('.btn-facebook').html('<i class="fa fa-facebook"></i>Sign Up with Facebook')
    });
        </script>

<div class="register-content">
    <div class="row">
        <div class="col-md-12">
            <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger">
                <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
            </asp:Panel>
            <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="alert alert-danger"
                ValidationGroup="RegisterUserValidationGroup" DisplayMode="List" />
        </div>
    </div>
    <asp:Panel ID="RegistrationPanel" runat="server">
        <asp:PlaceHolder runat="server" ID="AvailableRolesPlaceHolder">
            <label class="control-label"><%=U6000.REGISTERAS %>: <span class="text-danger">*</span></label>
            <div class="row row-space-10">
                <div class="col-xs-12 m-b-15">
                    <div class="col-xs-4" runat="server" id="EarnerCheckBoxPlaceHolder">
                        <label class="btn btn-primary btn-block p-0 registration-type-icon">
                            <span class="icon fa fa-money fa-fw fa-4x img-thumbnail img-check text-primary p-15 hidden-xs"></span>
                            <span class="icon fa fa-money fa-fw fa-2x img-thumbnail img-check text-primary p-10 visible-xs hidden-lg hidden-md hidden-lg"></span>
                            <p class="text-center m-0"><%=U6000.EARNER %></p>
                            <asp:CheckBox runat="server" ID="EarnerCheckBox" ValidationGroup="RegisterUserValidationGroup" CssClass="registration-type-checkbox hidden" />
                        </label>
                    </div>
                    <div class="col-xs-4" runat="server" id="AdvertiserCheckBoxPlaceHolder">
                        <label class="btn btn-primary btn-block p-0 registration-type-icon">
                            <span class="icon fa fa-bullhorn fa-fw fa-4x img-thumbnail img-check text-primary p-15 hidden-xs"></span>
                            <span class="icon fa fa-bullhorn fa-fw fa-2x img-thumbnail img-check text-primary p-10 visible-xs hidden-lg hidden-md hidden-lg"></span>
                            <p class="text-center m-0"><%=L1.ADVERTISER %></p>
                            <asp:CheckBox runat="server" ID="AdvertiserCheckBox" ValidationGroup="RegisterUserValidationGroup" CssClass="registration-type-checkbox hidden" />
                        </label>
                    </div>
                    <div class="col-xs-4" runat="server" id="PublisherCheckBoxPlaceHolder">
                        <label class="btn btn-primary btn-block p-0 registration-type-icon">
                            <span class="icon fa fa-globe fa-fw fa-4x img-thumbnail img-check text-primary p-15 hidden-xs"></span>
                            <span class="icon fa fa-globe fa-fw fa-2x img-thumbnail img-check text-primary p-10 visible-xs hidden-lg hidden-md hidden-lg"></span>
                            <p class="text-center m-0"><%=U6000.PUBLISHER %></p>
                            <asp:CheckBox runat="server" ID="PublisherCheckBox" ValidationGroup="RegisterUserValidationGroup" CssClass="registration-type-checkbox hidden" />
                        </label>
                    </div>



                    <asp:CustomValidator ID="AccountTypeValidator" ClientValidationFunction="validateRegistrationType" OnServerValidate="Validate_AccountType" runat="server" ValidationGroup="RegisterUserValidationGroup"
                        Display="Dynamic" CssClass="text-danger">*</asp:CustomValidator>
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="Username" CssClass="control-label"><%=L1.USERNAME %>: <span class="text-danger">*</span></asp:Label>
        <div class="row row-space-10">
            <div class="col-md-12 m-b-15">
                <asp:TextBox ID="Username" runat="server" CssClass="form-control"></asp:TextBox>
                <asp:RegularExpressionValidator ValidationGroup="RegisterUserValidationGroup" ID="RegularExpressionValidator1" runat="server"
                    ValidationExpression="[a-zA-Z][a-zA-Z0-9]{3,20}" ControlToValidate="Username" Text="" Display="Dynamic" CssClass="text-danger">*</asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="Username"
                    Text=""
                    ValidationGroup="RegisterUserValidationGroup" Display="Dynamic" CssClass="text-danger">*</asp:RequiredFieldValidator>
            </div>
        </div>
        <div <%=FacebookAuthHidden %>>
            <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email" CssClass="control-label">Email: <span class="text-danger">*</span></asp:Label>
            <div class="row m-b-15">
                <div class="col-md-12">
                    <asp:TextBox ID="Email" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                        Display="Dynamic" CssClass="text-danger"
                        ValidationGroup="RegisterUserValidationGroup" Text=""></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator runat="server"
                        ValidationGroup="RegisterUserValidationGroup" Display="Dynamic" CssClass="text-danger"
                        Text="" ID="CorrectEmailRequired" ControlToValidate="Email" ValidationExpression="^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$"></asp:RegularExpressionValidator>

                </div>
            </div>
        </div>

        <div <%=FacebookAuthHidden %>>
            <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" CssClass="control-label"><%=L1.PASSWORD %>: <span class="text-danger">*</span></asp:Label>
            <div class="row m-b-15">
                <div class="col-md-12">
                    <asp:TextBox ID="Password" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                    <asp:RegularExpressionValidator ForeColor="#b70d00" ValidationGroup="RegisterUserValidationGroup" ID="RegularExpressionValidator2" runat="server" ErrorMessage="*"
                        ValidationExpression="[a-zA-Z0-9!A#\$%\^&\*\(\)=\+\.\,\-@]{4,81}" ControlToValidate="Password" Text="" Display="Dynamic" CssClass="text-danger">*</asp:RegularExpressionValidator>
                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                        Display="Dynamic" CssClass="text-danger"
                        ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:RequiredFieldValidator>
                </div>
            </div>       
        </div>

        <div <%=FacebookAuthHidden %>>

            <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword" CssClass="control-label"><%=L1.CONFIRMPASSWORD %>: <span class="text-danger">*</span></asp:Label>

            <div class="row m-b-15">
                <div class="col-md-12 p-b-15">
                    <asp:TextBox ID="ConfirmPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="ConfirmPassword" Display="Dynamic" CssClass="text-danger"
                        ID="ConfirmPasswordRequired" runat="server"
                        ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword"
                        ValidationGroup="RegisterUserValidationGroup" Display="Dynamic" CssClass="text-danger" Text="">*</asp:CompareValidator>
                </div>
            </div>
        </div>

        <div class="row">
            <asp:PlaceHolder ID="PINSectionPlaceHolder" runat="server">
                <div class="col-md-6">
                    <asp:Label ID="Label1" AssociatedControlID="PIN" runat="server" CssClass="control-label"><%=L1.DESIREDPIN %>: <span class="text-danger">*</span></asp:Label>

                    <div class="row m-b-15">
                        <div class="col-md-12">
                            <asp:TextBox ID="PIN" runat="server" CssClass="form-control" MaxLength="4" autocomplete="off"></asp:TextBox>
                            <asp:RegularExpressionValidator ValidationGroup="RegisterUserValidationGroup" ID="RegularExpressionValidator3" runat="server"
                                ValidationExpression="[0-9]{4}" ControlToValidate="PIN" Display="Dynamic" CssClass="text-danger">*</asp:RegularExpressionValidator>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="PIN"
                                ValidationGroup="RegisterUserValidationGroup" Display="Dynamic" CssClass="text-danger">*</asp:RequiredFieldValidator>
                            <asp:CustomValidator OnServerValidate="Validate_PIN" runat="server" ValidationGroup="RegisterUserValidationGroup"
                                Display="Dynamic" CssClass="text-danger" ID="CustomValidator2" EnableClientScript="False" ControlToValidate="PIN" ErrorMessage="PIN can't have any zeros in the beginning">*</asp:CustomValidator>

                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="col-md-6">
                <asp:Label ID="BirthYearLabel" AssociatedControlID="BirthYear" runat="server" CssClass="control-label"><%=L1.BIRTHYEAR %>: <span class="text-danger">*</span></asp:Label>

                <div class="row m-b-15">
                    <div class="col-md-12">
                        <asp:TextBox ID="BirthYear" runat="server" CssClass="form-control" MaxLength="4" autocomplete="off"></asp:TextBox>
                        <asp:RegularExpressionValidator ValidationGroup="RegisterUserValidationGroup" ID="RegularExpressionValidator4" runat="server"
                            ValidationExpression="(19[0-9][0-9]|200[0-9])" ControlToValidate="BirthYear" Display="Dynamic" CssClass="text-danger">*</asp:RegularExpressionValidator>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                            ControlToValidate="BirthYear" ValidationGroup="RegisterUserValidationGroup" Display="Dynamic" CssClass="text-danger">*</asp:RequiredFieldValidator>
                    </div>
                </div>
            </div>
        </div>

        <asp:Panel ID="DetailedPanel" Visible="false" runat="server">

            <div class="row">
                <div class="col-md-6">
                    <div <%=FacebookAuthHidden %>>
                        <label class="control-label"><%=L1.FIRSTNAME %>: <span class="text-danger">*</span></label>

                        <div class="row m-b-15">
                            <div class="col-md-12">
                                <asp:TextBox ID="FirstName" MaxLength="20" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RegularExpressionValidator ValidationGroup="RegisterUserValidationGroup"
                                    ID="RE_1" runat="server"
                                    ValidationExpression="[a-zA-Z0-9\.\,\-\!\?\&\:\$\@\(\)\+\=\%\*|\\\t\\\n|\\\r|\\\s]{1,20}" Display="Dynamic" CssClass="text-danger" ControlToValidate="FirstName" Text="">*</asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="RF_1" runat="server" ControlToValidate="FirstName"
                                    Display="Dynamic" CssClass="text-danger" Text=""
                                    ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <div <%=FacebookAuthHidden %>>
                        <label class="control-label"><%=L1.SECONDNAME %>: <span class="text-danger">*</span></label>
                        <div class="row m-b-15">
                            <div class="col-md-12">
                                <asp:TextBox ID="SecondName" MaxLength="40" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RegularExpressionValidator ValidationGroup="RegisterUserValidationGroup"
                                    ID="RE_2" Display="Dynamic" CssClass="text-danger" runat="server"
                                    ValidationExpression="[a-zA-Z0-9\.\,\-\!\?\&\:\$\@\(\)\+\=\%\*|\\\t\\\n|\\\r|\\\s]{1,40}" ControlToValidate="SecondName" Text="">*</asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="RF_2" runat="server" ControlToValidate="SecondName"
                                    Display="Dynamic" CssClass="text-danger" Text=""
                                    ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-md-6">
                    <label class="control-label"><%=L1.ADDRESS %>: <span class="text-danger">*</span></label>
                    <div class="row m-b-15">
                        <div class="col-md-12">
                            <asp:TextBox ID="Address" MaxLength="80" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RegularExpressionValidator ValidationGroup="RegisterUserValidationGroup"
                                ID="RE_3" runat="server"
                                ValidationExpression="[a-zA-Z0-9\.\,\-\!\?\&\:\$\@\(\)\+\=\%\*|\\\t\\\n|\\\r|\\\s]{1,80}" Display="Dynamic" CssClass="text-danger"
                                ControlToValidate="Address" Text="">*</asp:RegularExpressionValidator>
                            <asp:RequiredFieldValidator ID="RF_3" runat="server" ControlToValidate="Address"
                                Display="Dynamic" CssClass="text-danger" Text=""
                                ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <label class="control-label"><%=L1.CITY %>: <span class="text-danger">*</span></label>
                    <div class="row m-b-15">
                        <div class="col-md-12">
                            <asp:TextBox ID="City" MaxLength="20" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RegularExpressionValidator ValidationGroup="RegisterUserValidationGroup"
                                ID="RE_4" runat="server" Display="Dynamic" CssClass="text-danger"
                                ValidationExpression="[a-zA-Z0-9\.\,\-\!\?\&\:\$\@\(\)\+\=\%\*|\\\t\\\n|\\\r|\\\s]{1,20}" ControlToValidate="City" Text="">*</asp:RegularExpressionValidator>

                            <asp:RequiredFieldValidator ID="RF_4" runat="server" ControlToValidate="City"
                                Display="Dynamic" CssClass="text-danger" Text=""
                                ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-md-6">
                    <label class="control-label"><%=L1.STATEPROVINCE %>: <span class="text-danger">*</span></label>
                    <div class="row m-b-15">
                        <div class="col-md-12">
                            <asp:TextBox ID="StateProvince" MaxLength="30" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RegularExpressionValidator ValidationGroup="RegisterUserValidationGroup"
                                ID="RE_5" runat="server"
                                ValidationExpression="[a-zA-Z0-9\.\,\-\!\?\&\:\$\@\(\)\+\=\%\*|\\\t\\\n|\\\r|\\\s]{1,30}" Display="Dynamic" CssClass="text-danger" ControlToValidate="StateProvince" Text="">*</asp:RegularExpressionValidator>

                            <asp:RequiredFieldValidator ID="RF_5" runat="server" ControlToValidate="StateProvince"
                                Display="Dynamic" CssClass="text-danger" Text=""
                                ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <label class="control-label"><%=L1.ZIPCODE %>: <span class="text-danger">*</span></label>
                    <div class="row m-b-15">
                        <div class="col-md-12">
                            <asp:TextBox ID="ZipCode" MaxLength="10" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RegularExpressionValidator ValidationGroup="RegisterUserValidationGroup"
                                ID="RE_6" runat="server"
                                ValidationExpression="[a-zA-Z0-9\.\,\-\!\?\&\:\$\@\(\)\+\=\%\*|\\\t\\\n|\\\r|\\\s]{1,10}" Display="Dynamic" CssClass="text-danger" ControlToValidate="ZipCode" Text="">*</asp:RegularExpressionValidator>

                            <asp:RequiredFieldValidator ID="RF_6" runat="server" ControlToValidate="ZipCode"
                                Display="Dynamic" CssClass="text-danger" Text=""
                                ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                        </div>
                    </div>
                </div>
            </div>



            <div <%=FacebookAuthHidden %>>
                <label class="control-label"><%=L1.GENDER %>:</label>
                <div class="row m-b-15">
                    <div class="col-md-12">
                        <asp:RadioButtonList ID="GenderList" runat="server" RepeatDirection="Horizontal" CellPadding="3" CellSpacing="8" RepeatLayout="flow">
                            <asp:ListItem Text=' <%$ ResourceLookup: MALE %>' Value="1" Selected="True" class="radio-inline"></asp:ListItem>
                            <asp:ListItem Text=' <%$ ResourceLookup: FEMALE %>' Value="0" class="radio-inline"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>

        </asp:Panel>

        <asp:Panel ID="CustomFields" runat="server"></asp:Panel>

        <div>
            <asp:Label ID="RefererLabel" runat="server" CssClass="control-label"><%=L1.REFERER %>:</asp:Label>
            <div class="row m-b-15">
                <div class="col-md-12">
                    <asp:TextBox ID="Referer" name="r" runat="server" CssClass="form-control" Text="" Enabled="false"></asp:TextBox>

                    <asp:CustomValidator OnServerValidate="Validate_Referer" runat="server" ValidationGroup="RegisterUserValidationGroup"
                        Display="Dynamic" CssClass="text-danger" ID="RefererValidator" EnableClientScript="False" ControlToValidate="Referer">*</asp:CustomValidator>
                </div>
            </div>
        </div>

        <div <%=FacebookAuthHidden %>>
            <asp:PlaceHolder ID="RegistrationCaptchaPlaceHolder" runat="server">
                <label class="control-label"><%=L1.VERIFICATION %>: <span class="text-danger">*</span></label>
                <div class="row m-b-15">
                    <div class="col-md-12">
                        <titan:Captcha runat="server" ID="TitanCaptcha" ValidationGroup="RegisterUserValidationGroup" />
                        <asp:CustomValidator OnServerValidate="Validate_Captcha" runat="server" ValidationGroup="RegisterUserValidationGroup"
                            Display="Dynamic" CssClass="text-danger" ID="CustomValidator1" EnableClientScript="False">*</asp:CustomValidator>
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>

        <div>
            <label class="control-label"><%=L1.DETECTEDCOUNTRY %>:</label>
            <div class="row m-b-15">
                <div class="col-md-12">
                    <asp:Image ID="Flag" runat="server" />
                    <asp:Label ID="CountryName" runat="server" Text="Label" Font-Size="Small"></asp:Label>
                </div>
            </div>
        </div>

        <div class="checkbox m-b-30">
            <label>
                <asp:CheckBox runat="server" ID="checkedTerms" CssClass="floatleft" ClientIDMode="Static" />
                <%=L1.REG_UNDERSTANDTOS %> <a href="sites/tos.aspx"><%=L1.TERMSOFSERVICE %></a> <span class="text-danger">*</span><br />
                <asp:CustomValidator ID="CustomValidator4" OnServerValidate="Validate_Tos" runat="server" ClientValidationFunction="checkBox" ValidationGroup="RegisterUserValidationGroup"
                    Display="Dynamic" CssClass="text-danger">*</asp:CustomValidator>
            </label>
        </div>

        <div class="row">
            <div class="col-md-12">
                <div class="register-buttons">
                    <asp:Button ID="CreateUserButton" runat="server" OnClientClick=""
                        ValidationGroup="RegisterUserValidationGroup" CssClass="btn btn-primary btn-block btn-lg" OnClick="CreateUserButton_Click" />
                    <div class="m-b-10">
                        <titan:FacebookLogin ID="FacebookLogin1" runat="server" />
                    </div>
                </div>
            </div>
        </div>

    </asp:Panel>
</div>
