<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="deposit.aspx.cs" Inherits="UserDeposit" %>

<%@ MasterType VirtualPath="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <link href="Scripts/default/assets/plugins/bootstrap-select/css/bootstrap-select.min.css" rel="stylesheet" type="text/css">
    <script src="Scripts/default/assets/plugins/bootstrap-select/js/bootstrap-select.min.js"></script>

    <script type="text/javascript">

        var pointValue = 0.0;
        var currencySign = '';

        function pageLoad() {

            if ($('#<%=Account.ClientID %>').length) {
                $('#accountInputCol').removeClass('col-md-4');
            } else {
                $('#accountInputCol').addClass('col-md-4');
            }

            if (typeof tryModal == 'function') {
                tryModal();
            }

            $('.selectpicker').each(function () {
                let itemCount = $(this).find('option').length;
                let selectClass = 'bg-silver';
                if (itemCount < 2) {
                    selectClass += ' no-caret no-dropdown';
                }
                $(this).selectpicker({
                    style: selectClass,
                    hideDisabled: false
                });
            });

            if ('<%=AppSettings.Points.PointsEnabled %>' == 'True') {
                pointValue = new Decimal(<%=Points.GetPointsPer1d() %>);
                currencySign = '<%=new Money(1).ToString() %>';
            }
        }

        function pointConversion() {

            if ('<%=AppSettings.Points.PointsEnabled %>' == 'True') {
                var money = $('#<%=MoneyConversionLabel.ClientID %>');
                var points = $('#<%=PointsConversionLabel.ClientID %>');
                var amount = new Decimal($('#<%=TransferFromTextBox.ClientID %>')[0].value);

                money.html(currencySign.replace('1', amount.toString()));
                points.html(parseInt(amount.times(pointValue)));
            }

            return false;
        }

    </script>

    <asp:PlaceHolder ID="SuccessModal" runat="server" Visible="false">
        <script>
            function tryModal() {
                var modal = $('#confirmationModal');
                modal.find('.modal-body').append('<div class="alert alert-success"><h4><i class="fa fa-check"></i> Payment successful!</h4><p>Click the button below to upgrade</p></div>');
                modal.find('.modal-footer').addClass('text-center').html('<a href="user/upgrade.aspx" class="btn btn-success">Upgrade</a>');
                modal.modal({ 'backdrop': true, 'show': true });
            }
        </script>
    </asp:PlaceHolder>


</asp:Content>






<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">



    <h1 class="page-header"><%=U4200.DEPOSIT %></h1>

    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%= U6012.DEPOSITMONEYINFO %></p>
        </div>
    </div>

    <titan:AwaitingPaymentConfirmationWindow runat="server" />
    <asp:PlaceHolder runat="server" ID="UserBalancesPlaceHolder">
        <titan:UserBalances runat="server" />
    </asp:PlaceHolder>


    <asp:UpdatePanel runat="server" ID="MessageUpdatePanel" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="row">
                <div class="col-md-12">
                    <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger">
                        <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                    </asp:Panel>
                    <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="alert alert-success">
                        <asp:Literal ID="SuccMessage" runat="server"></asp:Literal>
                    </asp:Panel>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>


    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <div class="TitanViewPage">
                    <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                        <%--DEPOSIT VIA REPRESENTATIVE--%>
                        <asp:Button ID="DepositButton" runat="server" OnClick="MenuButton_Click" CommandArgument="2" Visible="false" />
                        <%--BTC--%>
                        <asp:Button ID="BTCButton" runat="server" OnClick="MenuButton_Click" CommandArgument="1" Visible="false" />
                        <%--BALANCE--%>
                        <asp:Button ID="BalanceButton" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>

    <div class="tab-content">
        <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
            <asp:View runat="server" ID="BalanceView">
                <div class="TitanViewElement">
                    <%-- SUBPAGE START --%>

                    <div class="col-md-12" runat="server" id="CommissionTransferInfoDiv" visible="false">
                        <p class="alert alert-info">
                            <asp:Label runat="server" ID="CommissionTransferInfo"></asp:Label><br />
                        </p>
                    </div>
                    <asp:PlaceHolder ID="StandardTransferPlaceHolder" runat="server">

                        <asp:UpdatePanel runat="server">
                            <ContentTemplate>

                                <div class="row" runat="server" id="dropdownlistsRow">
                                    <div class="col-md-12">

                                        <asp:PlaceHolder ID="AdditionalInformationLiteralPlaceHolder" runat="server" Visible="false">
                                            <p class='alert alert-info text-center'>
                                                <asp:Literal ID="AdditionalInformationLiteral" runat="server"></asp:Literal>
                                            </p>
                                        </asp:PlaceHolder>

                                        <p runat="server" id="TransferSameCommissionToMainP" class="text-center">
                                            <asp:Literal runat="server" ID="TransferSameCommissionToMainLiteral" Visible="false"></asp:Literal>
                                        </p>

                                        <div class="row" id="transfertable" runat="server">
                                            <div class="col-md-4 text-center">
                                                <asp:DropDownList ID="RadioFrom" CssClass="selectpicker" runat="server" CellSpacing="10"
                                                    OnSelectedIndexChanged="RadioFrom_SelectedIndexChanged" AutoPostBack="true" RepeatLayout="Flow" />
                                            </div>
                                            <div class="col-md-4">
                                                <p class="text-center">
                                                    <span class="fa fa-arrow-right fa-5x hidden-sm hidden-xs"></span>
                                                    <span class="fa fa-arrow-down m-20 fa-5x visible-xs visible-sm hidden-md hidden-lg"></span>
                                                </p>
                                            </div>
                                            <div class="col-md-4 text-center">
                                                <asp:DropDownList ID="RadioTo" OnSelectedIndexChanged="RadioTo_SelectedIndexChanged" AutoPostBack="true"
                                                    CssClass="selectpicker" runat="server" CellSpacing="10" RepeatLayout="Flow">
                                                </asp:DropDownList>
                                            </div>
                                        </div>

                                    </div>
                                </div>

                                <div class="row" runat="server" id="transferInputRow">
                                    <div class="col-md-12 m-t-20">
                                        <asp:PlaceHolder ID="PaymentConversionPlaceHolder" runat="server" >
                                            <div class="row">
                                                <div class="col-md-6 col-md-offset-3">
                                                    <div class="text-center f-s-16">
                                                        <p>
                                                            <asp:Label ID="MoneyConversionLabel" runat="server" Font-Bold="true"/> =
                                                            <asp:Label ID="PointsConversionLabel" runat="server" Font-Bold="true"/>&nbsp;<%=AppSettings.PointsName %>
                                                        </p>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:PlaceHolder>
                                        <div class="row">
                                            <div class="col-md-6 col-md-offset-3">
                                                <div class="form-group" id="AccountInformationDiv" runat="server">
                                                    <div class="clearfix p-t-15" style="clear: both;">
                                                        <div id="accountInputCol" class="col-md-12 p-0">
                                                            <div id="accountInputGroup" class="input-group width-full">
                                                                <asp:Button ID="AddNewAccount" runat="server" CssClass="btn btn-primary btn-block" Visible="false" OnClick="ChangeAccountButton_Click" />
                                                                <asp:Label ID="InfoLabel" runat="server"></asp:Label>
                                                                <asp:TextBox ID="Account" runat="server" CssClass="form-control" autocomplete="off"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="Account" ValidationGroup="CashoutValidationGroup" Display="Dynamic" CssClass="text-danger" Text="Required" />
                                                                <asp:RegularExpressionValidator ID="REValidator" runat="server" ControlToValidate="Account" Display="Dynamic" ValidationGroup="CashoutValidationGroup" ValidationExpression="[a-zA-Z0-9.,!@#$%^&*()+-/?\\|:=_]{1,40}" CssClass="text-danger" />
                                                                <div class="input-group-btn">
                                                                    <asp:Button ID="ChangeAccountButton" runat="server" CssClass="btn btn-primary" Width="100px" Visible="false" OnClick="ChangeAccountButton_Click" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <div class="input-prepend input-group">
                                                        <span class="add-on input-group-addon">
                                                            <asp:Literal ID="CurrencyTransferSignLiteral" runat="server"></asp:Literal>
                                                        </span>
                                                        <asp:TextBox ID="TransferFromTextBox" CssClass="form-control" runat="server" type="number"
                                                            lang="en" step="0.1" min="0" value="100.0" ClientIDMode="Static" ></asp:TextBox>
                                                        <asp:PlaceHolder ID="CalculatePlaceHolder" runat="server" >
                                                            <div class="input-group-btn">
                                                                <asp:Button ID="CalculatePointsValueButton" runat="server" OnClientClick="pointConversion(); return false;" CssClass="form-control btn btn-default"
                                                                        UseSubmitBehavior="false" />
                                                            </div>
                                                        </asp:PlaceHolder>
                                                        <div class="input-group-btn">
                                                            <asp:Button ID="btnTransfer" runat="server" OnClick="btnTransfer_Click" CssClass="btn btn-primary"
                                                                UseSubmitBehavior="false" Width="100px" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                

                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="text-center">
                                            <p>
                                                <asp:Label ID="PaymentAmountLabel" runat="server" Visible="false"></asp:Label>
                                            </p>
                                            <p>
                                                <asp:Label ID="PaymentFeeLabel" runat="server" Visible="false"></asp:Label>
                                            </p>
                                            <p>
                                                <asp:Label ID="PaymentAmountWithFeeLabel" runat="server" Visible="false"></asp:Label>
                                            </p>
                                            <p>
                                                <asp:Literal ID="PaymentButtons" runat="server"></asp:Literal>
                                            </p>
                                        </div>
                                    </div>
                                </div>

                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="MPesaTransferPlaceHolder2" runat="server" Visible="false">
                        <div class="row">
                            <div class="col-md-12">
                                <asp:Panel ID="MPesaErrorPanel" runat="server" Visible="false" CssClass="alert alert-danger">
                                    <asp:Literal ID="MPesaErrorLiteral" runat="server"></asp:Literal>
                                </asp:Panel>
                                <div class="row">
                                    <div class="col-md-12">
                                        <img src="Images/Misc/MPesa.png" style="margin-bottom: 30px" />
                                        <p>
                                            <asp:Literal ID="MPesaAmount" runat="server"></asp:Literal>
                                            <br />
                                            <br />
                                        </p>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <label class="control-label col-md-3"><%=U4200.PHONE %>:</label>
                                                <div class="col-md-9 form-inline">
                                                    <asp:TextBox CssClass="form-control" runat="server" Width="46px" Enabled="false" Text="254"></asp:TextBox>
                                                    -
                                                    <asp:TextBox CssClass="form-control" ID="MPesaPhoneTextBox" runat="server"></asp:TextBox>
                                                    <span class="help-block"><%=U6005.MPESAENSURE %></span>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label class="control-label col-md-3"><%=U6005.CONFIRMATIONCODE %>:</label>
                                                <div class="col-md-9">
                                                    <asp:TextBox CssClass="form-control" ID="MPesaCodeTextBox" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <div class="col-md-4">
                                                    <asp:Button ID="MPesaConfirmButton" runat="server" OnClick="MPesaConfirmButton_Click" CssClass="btn btn-primary btn-block"
                                                        UseSubmitBehavior="false" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </asp:PlaceHolder>

                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>

            <asp:View ID="DepositBTCView" runat="server">
                <div class="TitanViewElement">
                    <%-- SUBPAGE START --%>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:UpdatePanel ID="CryptoCurrencyPanel" runat="server" UpdateMode="Conditional">
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="BTCPointsFrom" EventName="SelectedIndexChanged" />
                                </Triggers>
                                <ContentTemplate>
                                    <asp:Panel ID="ErrorMessagePanelBTC" runat="server" Visible="false" CssClass="alert alert-danger">
                                        <asp:Literal ID="ErrorMessageBTC" runat="server"></asp:Literal>
                                    </asp:Panel>

                                    <asp:PlaceHolder ID="AdditionalInfoPlaceHolder" runat="server" Visible="false">
                                        <p class='alert alert-info text-center'>
                                            <asp:Literal ID="AdditionalInfoLiteral" runat="server" />
                                        </p>
                                    </asp:PlaceHolder>
                                    <div class="row">
                                        <div class="col-md-4 text-center">
                                            <asp:DropDownList ID="BTCPointsFrom" CssClass="selectpicker" runat="server" CellSpacing="10" OnSelectedIndexChanged="BTCPointsFrom_SelectedIndexChanged" AutoPostBack="true" RepeatLayout="Flow">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-4">
                                            <p class="text-center">
                                                <span class="fa fa-arrow-right fa-5x hidden-sm hidden-xs"></span>
                                                <span class="fa fa-arrow-down m-20 fa-5x visible-xs visible-sm hidden-md hidden-lg"></span>
                                            </p>
                                        </div>
                                        <div class="col-md-4 text-center">
                                            <asp:DropDownList ID="BTCTo" CssClass="selectpicker" runat="server" CellSpacing="10" RepeatLayout="Flow">
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <asp:Panel ID="BTCButtonPanel" runat="server">
                                        <asp:Label runat="server" ID="BTCValueLabel"></asp:Label>

                                        <asp:Panel ID="BTCDepositInfopanel" runat="server">
                                            <div class="row">
                                                <div class="col-md-4 col-md-offset-4 m-t-15">
                                                    <div class="input-prepend input-group">
                                                        <span class="add-on input-group-addon"><%=AppSettings.Site.MulticurrencySign %></span>
                                                        <asp:TextBox ID="BTCAmountTextBox" runat="server" CssClass="form-control" />
                                                    </div>
                                                    <asp:RequiredFieldValidator ID="BTCAmountRequiredFieldValidator" runat="server" ControlToValidate="BTCAmountTextBox"
                                                        Display="Dynamic" ValidationGroup="BtcConfirmValidationGroup" CssClass="text-danger">*</asp:RequiredFieldValidator>
                                                </div>
                                            </div>

                                            <div class="row">
                                                <div class="col-md-2 col-md-offset-5 m-t-15">
                                                    <asp:Button ID="btnDepositBTC" runat="server" OnClick="btnDepositBTC_Click" CssClass="btn btn-primary btn-block"
                                                        UseSubmitBehavior="false" ValidationGroup="BtcConfirmValidationGroup" />
                                                </div>
                                            </div>
                                        </asp:Panel>

                                        <div class="row">
                                            <div class="col-md-12">
                                                <div class="text-center">
                                                    <p>
                                                        <asp:Literal ID="BTCPaymentButton" runat="server"></asp:Literal>
                                                    </p>
                                                </div>
                                            </div>
                                        </div>
                                    </asp:Panel>

                                    <asp:Panel runat="server" ID="ClassicBTCPanel">
                                        <asp:Panel ID="WalletToBTCPanel" runat="server" Visible="false">
                                            <asp:Panel runat="server" ID="BTCValuePanel" Visible="false">
                                                <asp:Label runat="server" ID="ClassicBTCValueLabel"></asp:Label>
                                            </asp:Panel>

                                            <asp:Panel ID="DepositBTCInfoPanel" runat="server" Visible="false">
                                                <asp:Label runat="server" ID="DepositBTCInfoLabel" /><br />
                                                <br />
                                            </asp:Panel>
                                            <asp:Literal runat="server" ID="multipleDepositWarningLiteral" Visible="false"></asp:Literal>

                                            <div class="text-center">
                                                <div>
                                                    <asp:Image ID="BitcoinQRCode" runat="server" />
                                                </div>
                                                <asp:Label runat="server" ID="depositBTCLabel"></asp:Label>
                                            </div>

                                        </asp:Panel>

                                        <div class="row">
                                            <div class="col-md-2 col-md-offset-5 m-t-15">
                                                <asp:Button ID="classicbtcDepositBTC" runat="server" OnClick="btnDepositBTC_Click" CssClass="btn btn-primary btn-block"
                                                    UseSubmitBehavior="false" ValidationGroup="BtcConfirmValidationGroup" />
                                            </div>
                                        </div>
                                    </asp:Panel>

                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>

                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>


            <asp:View runat="server" ID="TransferViaRepresentativeView" OnActivate="TransferViaRepresentativeView_Activate">

                <div class="TitanViewElement">
                    <div class="row">
                        <div class="col-md-12">
                            <asp:PlaceHolder runat="server" ID="NoRepresentativeInfoPlaceHolder" Visible="true">
                                <div class="text-center">
                                    <div class="form-group m-t-20">
                                        <h3>
                                            <%=U6010.NOREPRESENTATIVES %>
                                        </h3>
                                    </div>
                                </div>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder runat="server" ID="RepresentativeInfoContentPlaceHolder" Visible="false">
                                <div class="row" runat="server" id="Div1">
                                    <div class="col-md-12">
                                        <div class="row">
                                            <div class="col-md-8">
                                                <div class="form-horizontal">

                                                    <div class="form-group m-t-20">
                                                        <div class="col-md-3">
                                                            <label class="control-label">
                                                                <%=U6002.REPRESENTATIVES %> /
                                                        <asp:Image CssClass="UpperFlag" ID="flagImage" runat="server" />
                                                                <%=Member.CurrentInCache.Country %>:                                                                
                                                            </label>
                                                        </div>
                                                        <div class="col-md-6">
                                                            <div class="radio m-l-30">
                                                                <asp:RadioButtonList runat="server" ID="AvaibleRepresentativeList" CssClass="selectpicker" RepeatLayout="Flow" CellSpacing="10" OnSelectedIndexChanged="AvaibleRepresentativeList_SelectedIndexChanged" AutoPostBack="true" />
                                                            </div>
                                                            <br />
                                                            <a href="../sites/representatives.aspx" class="btn btn-primary"><%=U6010.SEEALL %> </a>
                                                        </div>
                                                    </div>

                                                    <div class="form-group m-t-20">
                                                        <div class="col-md-3">
                                                            <label class="control-label"><%=U6010.DEPOSITINFO %>:</label>
                                                        </div>
                                                        <div class="col-md-6" style="padding-top: 7px">
                                                            <asp:PlaceHolder runat="server" ID="DepositOptionsPlaceHolder" />
                                                        </div>
                                                    </div>

                                                    <div class="form-group m-t-20">
                                                        <div class="col-md-3">
                                                            <label class="control-label"><%=L1.AMOUNT %>:</label>
                                                        </div>
                                                        <div class="col-md-6">
                                                            <div class="input-prepend input-group">
                                                                <span class="add-on input-group-addon"><%=AppSettings.Site.MulticurrencySign %></span>
                                                                <input class="form-control" type="number" name="price" value="100" step=".1" min="0" />
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="form-group m-t-20">
                                                        <div class="col-md-3">
                                                            <label class="control-label"><%=U5004.MESSAGE %>:</label>
                                                        </div>
                                                        <div class="col-md-6" style="padding-top: 7px">
                                                            <asp:TextBox runat="server" ID="RepresentativeMessage" MaxLength="2500"
                                                                class="form-control" TextMode="MultiLine" Height="200px" />
                                                        </div>
                                                    </div>

                                                    <div class="form-group m-t-20">
                                                        <asp:Button ID="DepositViaRepresentativeButton" runat="server" OnClick="DepositViaRepresentativeButton_Click" CssClass="btn btn-inverse" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </asp:View>


        </asp:MultiView>
    </div>
</asp:Content>

