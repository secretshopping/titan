<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="settings.aspx.cs" Inherits="About" %>

<%@ MasterType VirtualPath="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">

    <link href="Scripts/default/assets/plugins/bootstrap-social/bootstrap-social.css" rel="stylesheet" />

    <link href="Scripts/default/assets/plugins/switchery/switchery.min.css" rel="stylesheet">
    <script src="Scripts/default/assets/plugins/switchery/switchery.min.js"></script>

    <link href="Styles/Switch.css" rel="stylesheet">
    <style>
        .img-check {
            width: 100%;
        }

        .check {
            opacity: 0.7;
        }

        .input-h-40 {
            height: 40px;
        }

        .text-gray {
            color: #CCC;
        }
    </style>
    <script type="text/javascript">
        var forceWhiteN;
        jQuery(function ($) {
            $('#myonoffswitch2').change(password2change);
            changeIt('1');
            changeIt('2');
            changeIt('3');
        });
        function changeIt(id) {
            $('.optional' + id).change(function () {
                if ($(this).val() != '') {
                    $('.optional' + id).css('background-color', '#ffffff');
                }
                else
                    $('.optional' + id).css('background-color', '#f0f0ef');
            });
        }
        function password2change(forceFast) {
            if (typeof (forceFast) === 'undefined') forceFast = false;
            if ($('#myonoffswitch2').is(':checked')) {
                $('.hiddable').show('slow');
                if ($('#<%=HadSecondaryPassword.ClientID%>').text() == 0) {
                    $('.optional2').css('background-color', '#ffffff');
                }
            }
            else {
                if (forceFast == true)
                    $('.hiddable').hide(0);
                else
                    $('.hiddable').hide('slow');

                $('#<%=Password2.ClientID%>').val('');
                $('#<%=ConfirmPassword2.ClientID%>').val('');
            }
        }

        function validateRegistrationType(source, args) {
            if ('<%=AppSettings.Registration.IsDefaultRegistrationStatusEnabled %>' == 'True') {
                args.IsValid = true;
            }
            else {
                var oneChecked = $(".registration-type-checkbox input[type=checkbox]").is(":checked") ? true : false;
                if (oneChecked == true) {
                    args.IsValid = true;
                } else {
                    args.IsValid = false;
                }
            }
        }

        function testRegistrationTypeCheckboxSection() {
            $(".registration-type-icon input[type=checkbox]").each(function () { console.log($(this).attr("id") + ": " + $(this).is(":checked")); });
        }

        $(document).ready(function () {
            //testRegistrationTypeCheckboxSection();
            $(".img-check").on("click", function () {
                $(this).toggleClass("check");
                //testRegistrationTypeCheckboxSection();
            });
            $(".registration-type-icon p").on("click", function () {
                $(this).parent().find("span.icon").toggleClass("check");
                //testRegistrationTypeCheckboxSection();
            });

        });
    </script>
</asp:Content>


<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <h1 class="page-header"><%= L1.SETTINGS %></h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=L1.SETTINGSINFO %></p>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
            </asp:Panel>
            <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
                <asp:Literal ID="SuccMessage" runat="server"></asp:Literal>
            </asp:Panel>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <div class="TitanViewPage">
                    <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                        <asp:Button ID="MenuButtonSecurity" runat="server" OnClick="MenuButton_Click" CommandArgument="5" />
                        <asp:Button ID="MenuButtonPreferences" runat="server" OnClick="MenuButton_Click" CommandArgument="4" />
                        <asp:Button ID="MenuButtonVerification" runat="server" OnClick="MenuButton_Click" CommandArgument="3" />
                        <asp:Button ID="MenuButtonVacationMode" runat="server" OnClick="MenuButton_Click" CommandArgument="2" />
                        <asp:Button ID="MenuButtonPayment" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                        <asp:Button ID="MenuButtonGeneral" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>
    <div class="tab-content">
        <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
            <asp:View runat="server" ID="asdasd">
                <div class="TitanViewElement">
                    <%-- SUBPAGE START --%>

                    <div class="row">
                        <div class="col-md-12">
                            <asp:ValidationSummary ID="ValidationSummary3" runat="server" CssClass="alert alert-danger fade in m-b-15"
                                ValidationGroup="createBannerAdvertisement_OnSubmitValidationGroup" DisplayMode="List" />

                            <asp:ValidationSummary ID="ChangeSettingsValidationSummary" runat="server" CssClass="alert alert-danger fade in m-b-15"
                                ValidationGroup="ChangeSettingsValidationGroup" DisplayMode="List" />
                        </div>
                    </div>
                    <div class="profile-section row">
                        <div class="profile-left-responsive col-lg-2 col-md-3 col-sm-4">
                            <div class="profile-image">
                                <asp:Image ID="AvatarImage" runat="server" />
                            </div>
                            <div class="m-b-10">
                                <span class="btn btn-warning btn-block btn-sm fileinput-button">
                                    <span><%=U6000.CHPICTURE %></span>
                                    <asp:FileUpload ID="changeSettings_AvatarUpload" runat="server" CssClass="upload-text" />
                                </span>
                                <asp:Button ID="changeSettings_AvatarUploadSubmit" Text="Submit" OnClick="changeSettings_AvatarUploadSubmit_Click"
                                    CausesValidation="true" runat="server" ValidationGroup="createBannerAdvertisement_OnSubmitValidationGroup"
                                    CssClass="btn btn-primary btn-block btn-sm upload-button" />
                            </div>

                            <div class="profile-highlight m-b-20">
                                <p class="text-warning text-center f-w-700"><%=string.Format(U6012.AVATARVALIDATOR, ImagesHelper.AvatarImage.MaxWidth, ImagesHelper.AvatarImage.MaxWidth) %></p>
                            </div>

                            <asp:CustomValidator ID="AvatarUploadValidCustomValidator"
                                ControlToValidate="changeSettings_AvatarUpload"
                                OnServerValidate="createBannerAdvertisement_BannerUploadValidCustomValidator_ServerValidate"
                                ValidationGroup="createBannerAdvertisement_OnSubmitValidationGroup" ValidateEmptyText="true"
                                runat="server" Display="Dynamic" CssClass="text-danger">*</asp:CustomValidator>

                        </div>
                        <div class="profile-right-responsive col-lg-10 col-md-9 col-sm-8">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="form-horizontal">
                                        <asp:PlaceHolder ID="ButtonsPlaceHolder" runat="server" Visible="true">
                                            <div class="form-group">
                                                <div class="col-md-8 col-md-offset-2">
                                                    <div class="row row-space-10">
                                                        <div class="col-xs-12 m-b-15">
                                                            <div class="col-xs-4" runat="server" id="EarnerCheckBoxPlaceHolder">
                                                                <label class="btn btn-primary btn-block p-0 registration-type-icon">
                                                                    <span runat="server" id="EarnerCheckBoxImage" class="icon fa fa-money fa-fw fa-2x img-thumbnail img-check text-primary p-10"></span>
                                                                    <p class="text-center m-0"><%=U6000.EARNER %></p>
                                                                    <asp:CheckBox runat="server" ID="EarnerCheckBox" ValidationGroup="ChangeSettingsValidationGroup" CssClass="registration-type-checkbox hidden" />
                                                                </label>
                                                            </div>
                                                            <div class="col-xs-4" runat="server" id="AdvertiserCheckBoxPlaceHolder">
                                                                <label class="btn btn-primary btn-block p-0 registration-type-icon">
                                                                    <span runat="server" id="AdvertiserCheckBoxImage" class="icon fa fa-bullhorn fa-fw fa-2x img-thumbnail img-check text-primary p-10"></span>
                                                                    <p class="text-center m-0"><%=L1.ADVERTISER %></p>
                                                                    <asp:CheckBox runat="server" ID="AdvertiserCheckBox" ValidationGroup="ChangeSettingsValidationGroup" CssClass="registration-type-checkbox hidden" />
                                                                </label>
                                                            </div>
                                                            <div class="col-xs-4" runat="server" id="PublisherCheckBoxPlaceHolder">
                                                                <label class="btn btn-primary btn-block p-0 registration-type-icon">
                                                                    <span runat="server" id="PublisherCheckBoxImage" class="icon fa fa-globe fa-fw fa-2x img-thumbnail img-check text-primary p-10"></span>
                                                                    <p class="text-center m-0"><%=U6000.PUBLISHER %></p>
                                                                    <asp:CheckBox runat="server" ID="PublisherCheckBox" ValidationGroup="ChangeSettingsValidationGroup" CssClass="registration-type-checkbox hidden" />
                                                                </label>
                                                            </div>
                                                            <asp:CustomValidator ID="AccountTypeValidator" ClientValidationFunction="validateRegistrationType" OnServerValidate="Validate_AccountType" runat="server" ValidationGroup="ChangeSettingsValidationGroup"
                                                                Display="Dynamic" CssClass="text-danger">*</asp:CustomValidator>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:PlaceHolder>

                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=U4200.EMAIL %>:</label>
                                            <div class="col-md-8">
                                                <asp:TextBox ID="Email" runat="server" CssClass="form-control tooltip-on"></asp:TextBox>

                                                <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                                                    Display="Dynamic" CssClass="text-danger"
                                                    ValidationGroup="ChangeSettingsValidationGroup" Text="">*</asp:RequiredFieldValidator>

                                                <asp:RegularExpressionValidator runat="server"
                                                    ValidationGroup="ChangeSettingsValidationGroup" Display="Dynamic" CssClass="text-danger"
                                                    Text="" ID="CorrectEmailRequired" ControlToValidate="Email" ValidationExpression="^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$">*</asp:RegularExpressionValidator>

                                            </div>
                                        </div>

                                        <%--Below is not implemented yet: hide it --%>
                                        <div class="form-group" runat="server" visible="false">
                                            <label class="control-label col-md-2"><%=L1.MESSAGESSYSTEM %>:</label>
                                            <div class="col-md-8">
                                                <asp:CheckBox ID="myonoffswitch" runat="server" CssClass="js-switch" />
                                            </div>
                                        </div>
                                        <%--Above is not implemented yet: hide it --%>
                                        <asp:PlaceHolder runat="server" ID="DetailedInfoPlaceHolder">
                                            <div class="form-group">
                                                <label class="control-label col-md-2"><%=L1.FIRSTNAME %>:</label>
                                                <div class="col-md-8">
                                                    <asp:TextBox ID="FirstNameTextBox" runat="server" MaxLength="100" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label class="control-label col-md-2"><%=L1.SECONDNAME %>:</label>
                                                <div class="col-md-8">
                                                    <asp:TextBox ID="SecondNameTextBox" runat="server" MaxLength="100" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label class="control-label col-md-2"><%=L1.ADDRESS %>:</label>
                                                <div class="col-md-8">
                                                    <asp:TextBox ID="AddressTextBox" runat="server" MaxLength="100" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label class="control-label col-md-2"><%=L1.CITY %>:</label>
                                                <div class="col-md-8">
                                                    <asp:TextBox ID="CityTextBox" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label class="control-label col-md-2"><%=L1.STATEPROVINCE %>:</label>
                                                <div class="col-md-8">
                                                    <asp:TextBox ID="StateProvinceTextBox" runat="server" MaxLength="30" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label class="control-label col-md-2"><%=L1.ZIPCODE %>:</label>
                                                <div class="col-md-8">
                                                    <asp:TextBox ID="ZipCodeTextBox" runat="server" MaxLength="10" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                        </asp:PlaceHolder>
                                        <%--<div class="form-group">
                                                            <asp:Label ID="BirthYearLabel" runat="server" CssClass="control-label col-md-3"><%=Resources.L1.BIRTHYEAR %>:</asp:Label>
                                                            <div class="col-md-7">
                                                                <asp:TextBox ID="BirthYear" runat="server" CssClass="form-control" MaxLength="4" autocomplete="off"></asp:TextBox>
                                                            </div>
                                                            <div class="col-md-2">
                                                                <asp:RegularExpressionValidator ValidationGroup="ChangeSettingsValidationGroup" ID="RegularExpressionValidator4" runat="server"
                                                                    ValidationExpression="(19[4-9][0-9]|200[0-9])" ControlToValidate="BirthYear">'</asp:RegularExpressionValidator>
                                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                                                    ControlToValidate="BirthYear" ValidationGroup="ChangeSettingsValidationGroup" Display="Dynamic">'</asp:RequiredFieldValidator>
                                                            </div>
                                                        </div>--%>
                                        <div class="form-group" runat="server" id="PINDiv1">
                                            <label class="control-label col-md-2"><%=L1.CURRENTPIN %>:</label>
                                            <div class="col-md-8">
                                                <asp:TextBox ID="CurrentPIN" runat="server" CssClass="form-control" MaxLength="4" autocomplete="off" type="password"></asp:TextBox>

                                                <asp:RegularExpressionValidator ValidationGroup="ChangeSettingsValidationGroup" ID="RegularExpressionValidator1" runat="server"
                                                    Display="Dynamic" CssClass="text-danger" ValidationExpression="[0-9]{4}" ControlToValidate="CurrentPIN">*</asp:RegularExpressionValidator>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="CurrentPIN"
                                                    Display="Dynamic" CssClass="text-danger" ValidationGroup="ChangeSettingsValidationGroup">*</asp:RequiredFieldValidator>
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <div class="col-md-2">
                                                <asp:Button ID="ChangeSettingsButton" runat="server"
                                                    ValidationGroup="ChangeSettingsValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="ChangeSettingsButton_Click" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>

            <asp:View ID="PaymentSettingsView" runat="server" OnActivate="PaymentSettingsView_Activate">
                <div class="TitanViewElement">
                    <%-- SUBPAGE START --%>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger fade in m-b-15"
                                ValidationGroup="SavePaymentPropertiesValidationGroup" DisplayMode="List" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <div class="payment-settings">
                                <%--PROCESSORS--%>
                                <asp:PlaceHolder ID="BasicPayoutProcessorsPlaceHolder" runat="server" />
                                <asp:PlaceHolder ID="CustomPayoutProcessorsPlaceHolder" runat="server" />

                                <div runat="server" id="btcSettings" class="form-group">

                                    <div class="row">
                                        <div class="col-sm-12 col-md-3 col-lg-1">
                                            <label class="control-label pull-right">
                                                <img src="Images/OneSite/TransferMoney/BTC.png" class="imagemiddle" />
                                            </label>
                                        </div>
                                        <div class="col-sm-12 col-md-9 col-lg-11">
                                            <asp:TextBox ID="BtcAddressTextBox" runat="server" CssClass="form-control input-h-40"></asp:TextBox>
                                            <p class="help-block" runat="server" id="BtcAddressWarning">
                                                <%= BtcCryptocurrency.ActivateUserAddressAfterDays > 0 ? string.Format(U6000.WALLETCHANGEWARNING, BtcCryptocurrency.ActivateUserAddressAfterDays) : "" %>
                                            </p>
                                        </div>
                                    </div>
                                </div>

                                <div runat="server" id="rippleSettings" class="form-group">
                                    <div class="row">
                                        <div class="col-sm-12 col-md-3 col-lg-1">
                                            <label class="control-label pull-right">
                                                <img src="Images/OneSite/TransferMoney/XRP.png" class="imagemiddle" />
                                            </label>
                                        </div>
                                        <div class="col-sm-12 col-md-9 col-lg-7 m-b-15">
                                            <asp:TextBox ID="XrpAddressTextBox" runat="server" CssClass="form-control input-h-40"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-12 col-md-9 col-md-offset-3 col-lg-4 col-lg-offset-0">
                                            <asp:TextBox ID="XrpDestTagTextBox" runat="server" CssClass="form-control input-h-40"></asp:TextBox>
                                            <p class="help-block" runat="server" id="XrpAddressWarning">
                                                <%= XrpCryptocurrency.ActivateUserAddressAfterDays> 0 ? string.Format(U6000.WALLETCHANGEWARNING, XrpCryptocurrency.ActivateUserAddressAfterDays) : "" %>
                                            </p>
                                        </div>
                                    </div>
                                </div>

                                <div runat="server" id="ethereumSettings" class="form-group">
                                    <div class="row">
                                        <div class="col-sm-12 col-md-3 col-lg-1">
                                            <label class="control-label pull-right">
                                                <img src="Images/OneSite/TransferMoney/ETH.png" class="imagemiddle" />
                                            </label>
                                        </div>
                                        <div class="col-sm-12 col-md-9 col-lg-11">
                                            <asp:TextBox ID="EthAddressTextBox" runat="server" CssClass="form-control input-h-40"></asp:TextBox>
                                            <p class="help-block" runat="server" id="EthAddressWarning">
                                                <%= EthCryptocurrency.ActivateUserAddressAfterDays> 0 ? string.Format(U6000.WALLETCHANGEWARNING, EthCryptocurrency.ActivateUserAddressAfterDays) : "" %>
                                            </p>
                                        </div>
                                    </div>
                                </div>

                                <div runat="server" id="tokenSettings" class="form-group">
                                    <div class="row">
                                        <div class="col-sm-12 col-md-3 col-lg-1">
                                            <label class="control-label pull-right">
                                                <asp:Image runat="server" ID="tokenImage" CssClass="imagemiddle" />
                                            </label>
                                        </div>
                                        <div class="col-sm-12 col-md-9 col-lg-11">
                                            <asp:TextBox ID="TokenAddressTextBox" runat="server" CssClass="form-control input-h-40"></asp:TextBox>
                                            <p class="help-block" runat="server" id="TokenAddressWarning">
                                                <%= TokenCryptocurrency.ActivateUserAddressAfterDays> 0 ? string.Format(U6000.WALLETCHANGEWARNING, TokenCryptocurrency.ActivateUserAddressAfterDays) : "" %>
                                            </p>
                                        </div>
                                    </div>
                                </div>

                                <div runat="server" id="CoinbaseEmailSettings" class="form-group" visible="false">
                                    <div class="row">
                                        <div class="col-sm-12 col-md-3 col-lg-1">
                                            <label class="control-label pull-right">
                                                <img src="Images/OneSite/TransferMoney/Coinbase.png" class="imagemiddle" />
                                            </label>
                                        </div>
                                        <div class="col-sm-12 col-md-9 col-lg-11">
                                            <asp:TextBox ID="CoinbaseEmailTextBox" runat="server" CssClass="form-control input-h-40" />
                                            <ajax:TextBoxWatermarkExtender ID="CoinbaseEmailTextBox_Watermark" TargetControlID="CoinbaseEmailTextBox" runat="server" WatermarkCssClass="form-control input-h-40 text-gray" />
                                            <p class="help-block" runat="server" id="CoinBaseEmailAddressWarning">
                                                <%= BtcCryptocurrency.ActivateUserAddressAfterDays> 0 ? string.Format(U6000.WALLETCHANGEWARNING, BtcCryptocurrency.ActivateUserAddressAfterDays) : "" %>
                                            </p>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-group" runat="server" id="PINDiv2">
                                    <div class="row">
                                        <div class="col-md-3 col-xs-6">
                                            <label class="control-label">
                                                <%=L1.CURRENTPIN %>:
                                            </label>

                                            <asp:TextBox ID="CurrentPIN2" runat="server" CssClass="form-control" MaxLength="4" autocomplete="off" type="password"></asp:TextBox>

                                            <asp:RegularExpressionValidator ValidationGroup="SavePaymentPropertiesValidationGroup" ID="RegularExpressionValidator7" runat="server"
                                                Display="Dynamic" CssClass="text-danger" ValidationExpression="[0-9]{4}" ControlToValidate="CurrentPIN2">*</asp:RegularExpressionValidator>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="CurrentPIN2"
                                                ValidationGroup="SavePaymentPropertiesValidationGroup" Display="Dynamic" CssClass="text-danger">*</asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="row">
                                        <div class="col-md-2">
                                            <asp:Button ID="ChangeSettingsButton2" runat="server"
                                                ValidationGroup="SavePaymentPropertiesValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="ChangeSettingsButton2_Click" />
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>

            <asp:View ID="View2" runat="server">
                <div class="TitanViewElement">
                    <%-- SUBPAGE START --%>
                    <div class="row">
                        <div class="col-md-12">
                            <p>
                                <asp:Literal ID="VacationLiteral" runat="server"></asp:Literal>
                            </p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-horizontal">
                                <asp:Panel ID="VacationPanel" runat="server">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <asp:ValidationSummary ID="ValidationSummary2" runat="server" CssClass="alert alert-danger fade in m-b-15"
                                                ValidationGroup="ChangeSettingsValidationGroup3" DisplayMode="List" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-8">
                                            <div class="form-group">
                                                <label class="control-label col-md-2"><%=L1.PRICE %>:</label>
                                                <div class="col-md-8">
                                                    <p class="form-control no-border m-b-0"><b><%=Prem.PTC.AppSettings.VacationAndInactivity.CostPerDay.ToString() %></b> <%=Resources.U4000.PERDAY %></p>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label class="control-label col-md-2"><%=U4000.DAYS %>:</label>
                                                <div class="col-md-8">
                                                    <asp:TextBox ID="VacationDays" runat="server" CssClass="form-control" MaxLength="4" autocomplete="off"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="YouAreInVacationPanel" runat="server" Visible="false">
                                    <p class="alert alert-warning">
                                        <%=U4000.YOUAREINVAC %>
                                        <asp:Literal ID="VacationEndsLiteral" runat="server"></asp:Literal>
                                    </p>
                                </asp:Panel>
                                <div class="row">
                                    <div class="col-md-8">
                                        <div class="form-group" runat="server" id="PINDiv3">
                                            <label class="control-label col-md-2"><%=L1.CURRENTPIN %>:</label>
                                            <div class="col-md-8">
                                                <asp:TextBox ID="CurrentPIN3" runat="server" CssClass="form-control" MaxLength="4" autocomplete="off" type="password"></asp:TextBox>

                                                <asp:RegularExpressionValidator ValidationGroup="ChangeSettingsValidationGroup3" ID="RegularExpressionValidator8" runat="server"
                                                    ValidationExpression="[0-9]{4}" ControlToValidate="CurrentPIN3" Display="Dynamic" CssClass="text-danger">*</asp:RegularExpressionValidator>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="CurrentPIN3"
                                                    ValidationGroup="ChangeSettingsValidationGroup3" Display="Dynamic" CssClass="text-danger">*</asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="col-md-3">
                                                <asp:Button ID="ChangeSettingsButton3" runat="server"
                                                    ValidationGroup="ChangeSettingsValidationGroup3" CssClass="btn btn-inverse btn-block" OnClick="ChangeSettingsButton3_Click" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>

            <!--VERIFICATION-->
            <asp:View ID="View3" runat="server">
                <div class="TitanViewElement">
                    <%-- SUBPAGE START --%>
                    <div class="row">
                        <div class="col-md-12">

                            <p><%=U5006.VERIFYINFO %> <%=String.Format(U5008.MAXSIZE,2500,1500) %></p>

                            <h4><%=L1.STATUS %>:</h4>
                            <p>
                                <asp:Label ID="AccountVerificationStatus" runat="server" Font-Bold="true"></asp:Label>
                            </p>

                            <p>
                                <asp:Label ID="VerificationLabel" Visible="false" runat="server" Text="Label"></asp:Label>
                            </p>

                            <div class="form-horizontal">
                                <div id="documentsUpload" runat="server">
                                    <div class="form-group">
                                        <label class="control-label col-md-2">
                                            <asp:Label ID="BannerImageLabel" runat="server" Text="Label"></asp:Label>*:</label>
                                        <div class="col-md-8">
                                            <span class="btn btn-success fileinput-button">
                                                <i class="fa fa-plus"></i>
                                                <span><%=U6000.ADDFILE %></span>
                                                <asp:FileUpload ID="Verification_BannerUpload" runat="server" />
                                            </span>
                                            <asp:Button ID="Verification_BannerUploadSubmit" OnClick="Verification_BannerUploadSubmit_Click"
                                                CausesValidation="true" runat="server" ValidationGroup="Verification_OnSubmitValidationGroup" CssClass="btn btn-primary" />
                                            <br />
                                            <asp:CustomValidator ID="Verification_BannerUploadValidCustomValidator"
                                                ControlToValidate="Verification_BannerUpload"
                                                OnServerValidate="Verification_BannerUploadValidCustomValidator_ServerValidate"
                                                ValidationGroup="Verification_OnSubmitValidationGroup" ValidateEmptyText="true"
                                                Display="Dynamic" CssClass="text-danger" runat="server">*</asp:CustomValidator>
                                        </div>
                                        <div class="col-md-12">
                                            <asp:Image ID="Verification_BannerImage" Visible="false" runat="server" BorderStyle="Solid" BorderWidth="1px" BorderColor="#e1e1e1" />
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-md-2">
                                        <asp:Button ID="VerificationButton" Visible="false" runat="server" CssClass="btn btn-primary" OnClick="VerificationButton_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>
            <asp:View runat="server" ID="View4">
                <div class="TitanViewElement">
                    <div class="row">
                        <div class="col-md-12">
                            <asp:ValidationSummary ID="ValidationSummary4" runat="server" CssClass="alert alert-danger fade in m-b-15"
                                ValidationGroup="PreferencesSettingsValidationGroup" DisplayMode="List" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="form-horizontal">
                            <div class="col-md-12">
                                <div id="captchaSettings" runat="server" class="form-group">
                                    <label class="control-label col-md-2">Captcha: </label>
                                    <div class="col-md-8">
                                        <div class="input-group">
                                            <div class="radio radio-button-list">
                                                <asp:RadioButtonList ID="CaptchaRB" runat="server" RepeatLayout="Flow">
                                                    <asp:ListItem Text="Google (reCAPTCHA)" Value="0" />
                                                    <asp:ListItem Text="SolveMedia" Value="1" />
                                                    <asp:ListItem Text="Titan" Value="2" />
                                                </asp:RadioButtonList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div id="shoutboxPrivacy" runat="server" class="form-group">
                                    <label class="control-label col-md-2"><%=U3500.SHOUTBOXPRIVACY %>:</label>
                                    <div class="col-md-8">
                                        <div class="input-group">
                                            <div class="radio radio-button-list">
                                                <asp:RadioButtonList ID="ShoutboxPrivacyList" runat="server" OnLoad="RadioButtonList1_Load" RepeatLayout="Flow"></asp:RadioButtonList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <%--Below is not implemented yet: hide it --%>
                                <div id="cpaNotifications" runat="server" class="form-group" visible="false">
                                    <label class="control-label col-md-2"><%=U3500.CPANOTIFICATIONS %>:</label>
                                    <div class="col-md-8">
                                        <div class="input-group">
                                            <div class="radio radio-button-list">
                                                <asp:RadioButtonList ID="CPACompletedPermissionsList" runat="server" OnLoad="CPACompletedPermissionsList_Load" RepeatLayout="Flow"></asp:RadioButtonList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <%--Above is not implemented yet: hide it --%>
                                <div class="form-group" runat="server" id="PINDiv4">
                                    <label class="control-label col-md-2"><%=L1.CURRENTPIN %>:</label>
                                    <div class="col-md-8">
                                        <asp:TextBox ID="CurrentPin4" runat="server" CssClass="form-control" MaxLength="4"
                                            autocomplete="off" type="password"></asp:TextBox>

                                        <asp:RegularExpressionValidator ValidationGroup="PreferencesSettingsValidationGroup" ID="RegularExpressionValidator5" runat="server"
                                            ValidationExpression="[0-9]{4}" ControlToValidate="CurrentPin4" Display="Dynamic" CssClass="text-danger">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="CurrentPin4"
                                            ValidationGroup="PreferencesSettingsValidationGroup" Display="Dynamic" CssClass="text-danger">*</asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-md-2">
                                        <asp:Button ID="PreferencesSettingsSaveButton" runat="server"
                                            ValidationGroup="PreferencesSettingsValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="PreferencesSettingsSaveButton_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:View>
            <asp:View runat="server" ID="View5">
                <div class="TitanViewElement">
                    <div class="row">
                        <div class="col-md-12">
                            <asp:ValidationSummary ID="SecuritySettingsValidationSummary" runat="server" CssClass="alert alert-danger fade in m-b-15"
                                ValidationGroup="SecuritySettingsValidationGroup" DisplayMode="List" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="form-horizontal">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <asp:Label ID="HadSecondaryPassword" runat="server" CssClass="displaynone"></asp:Label>
                                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" CssClass="control-label col-md-2"><%=L1.NEWPASS %>:</asp:Label>
                                    <div class="col-md-8">
                                        <asp:TextBox ID="Password" runat="server" CssClass="form-control tooltip-on" TextMode="Password"></asp:TextBox>

                                        <asp:RegularExpressionValidator Display="Dynamic" CssClass="text-danger" ValidationGroup="SecuritySettingsValidationGroup"
                                            ID="RegularExpressionValidator2" runat="server" ErrorMessage="*"
                                            ValidationExpression="[a-zA-Z0-9!A#\$%\^&\*\(\)=\+\.\,]{4,81}" ControlToValidate="Password" Text="">*</asp:RegularExpressionValidator>

                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword" CssClass="control-label col-md-2"><%=L1.NEWPASS2 %>:</asp:Label>
                                    <div class="col-md-8">
                                        <asp:TextBox ID="ConfirmPassword" runat="server" CssClass="form-control tooltip-on" TextMode="Password"></asp:TextBox>

                                        <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword"
                                            Display="Dynamic" CssClass="text-danger" ValidationGroup="SecuritySettingsValidationGroup" Text="">*</asp:CompareValidator>

                                    </div>
                                </div>
                                <div class="form-group" style="<%=Prem.PTC.AppSettings.Authentication.EnableSecondaryPassword? "": "display:none;"%>">
                                    <label class="control-label col-md-2"><%=L1.SECONDARYPASSWORD %></label>
                                    <div class="col-md-8">
                                        <asp:CheckBox ID="myonoffswitch2" runat="server" CssClass="js-switch" />
                                    </div>
                                </div>
                                <div class="form-group hiddable">
                                    <asp:Label ID="Label1" runat="server" AssociatedControlID="Password" CssClass="control-label col-md-2"><%=L1.NEWSECPASS %>:</asp:Label>
                                    <div class="col-md-8">
                                        <asp:TextBox ID="Password2" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>

                                        <asp:RegularExpressionValidator Display="Dynamic" CssClass="text-danger" ValidationGroup="SecuritySettingsValidationGroup" ID="RegularExpressionValidator6" runat="server" ErrorMessage="*"
                                            ValidationExpression="[a-zA-Z0-9!A#\$%\^&\*\(\)=\+\.\,]{4,81}" ControlToValidate="Password2" Text="">*</asp:RegularExpressionValidator>
                                    </div>
                                </div>
                                <div class="form-group hiddable">
                                    <asp:Label ID="Label2" runat="server" AssociatedControlID="ConfirmPassword" CssClass="control-label col-md-2"><%=L1.NEWPASS2 %>:</asp:Label>
                                    <div class="col-md-8">
                                        <asp:TextBox ID="ConfirmPassword2" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>

                                        <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="Password2" ControlToValidate="ConfirmPassword2"
                                            ValidationGroup="SecuritySettingsValidationGroup" Display="Dynamic" CssClass="text-danger" Text="">*</asp:CompareValidator>
                                    </div>
                                </div>
                                <div class="form-group" runat="server" id="PINDiv5">
                                    <label class="control-label col-md-2"><%=L1.NEWPIN %>:</label>
                                    <div class="col-md-8">
                                        <asp:TextBox ID="PIN" runat="server" CssClass="form-control" MaxLength="4" autocomplete="off" type="password"></asp:TextBox>

                                        <asp:RegularExpressionValidator ValidationGroup="SecuritySettingsValidationGroup" ID="RegularExpressionValidator3" runat="server"
                                            Display="Dynamic" CssClass="text-danger" ValidationExpression="[0-9]{4}" ControlToValidate="PIN">*</asp:RegularExpressionValidator>
                                        <asp:CustomValidator OnServerValidate="Validate_PIN" runat="server" ValidationGroup="SecuritySettingsValidationGroup"
                                            Display="Dynamic" CssClass="text-danger" ID="CustomValidator2" EnableClientScript="False" ControlToValidate="PIN">*</asp:CustomValidator>
                                    </div>
                                </div>
                                <div class="form-group" runat="server" id="PINDiv6">
                                    <label class="control-label col-md-2"><%=L1.CURRENTPIN %>:</label>
                                    <div class="col-md-8">
                                        <asp:TextBox ID="CurrentPin5" runat="server" CssClass="form-control" MaxLength="4"
                                            autocomplete="off" type="password"></asp:TextBox>

                                        <asp:RegularExpressionValidator ValidationGroup="SecuritySettingsValidationGroup" ID="RegularExpressionValidator4" runat="server"
                                            ValidationExpression="[0-9]{4}" ControlToValidate="CurrentPin5" Display="Dynamic" CssClass="text-danger">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="CurrentPin5"
                                            ValidationGroup="SecuritySettingsValidationGroup" Display="Dynamic" CssClass="text-danger">*</asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-md-2">
                                        <asp:Button ID="SecuritySettingsSaveButton" runat="server"
                                            ValidationGroup="SecuritySettingsValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="SecuritySettingsSaveButton_Click" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-sm-6 col-md-4">
                                        <titan:FacebookConnect runat="server" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:View>
        </asp:MultiView>
    </div>

</asp:Content>
