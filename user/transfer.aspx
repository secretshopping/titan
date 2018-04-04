<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="transfer.aspx.cs" Inherits="About" %>

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


            var seePointsAmount = $('#<%=SliderSinglePointsTextBox.ClientID %>')[0];

            if (seePointsAmount != null && '<%=AppSettings.Points.PointsEnabled %>' == 'True') {
                pointValue = new Decimal(<%=Points.GetCurrencyPer1Point() %>);
              currencySign = '<%=Money.CutEndingZeros(new Money(1).ToString()) %>';
                pointConversion();
            }
        }

        function pointConversion() {
            if ('<%=AppSettings.Points.PointsEnabled %>' == 'True') {
                var pointsNumber = $('#<%=PointsNumberLabel.ClientID %>');
                var moneyNumber = $('#<%=PointsValueLabel.ClientID %>');
                var amount = new Decimal($('#<%=SliderSinglePointsTextBox.ClientID %>')[0].value);

                pointsNumber.html(amount.toString());
                moneyNumber.html(currencySign.replace('1', amount.times(pointValue)));
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



    <h1 class="page-header"><%=L1.TRANSFER %></h1>

    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%= L1.TRANSFERMONEYINFO %></p>
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



    <input id="10PointsToDollarsRatio" value="<%=AppSettings.Memberships.TenPointsValue.ToClearString() %>" class="displaynone" />
    <input id="PointsToBTCRatio" value="<%=BtcCryptocurrency.GetValue() %>" class="displaynone" />


    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <div class="TitanViewPage">
                    <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                        <%--POINTS TO SPINS--%>
                        <asp:Button ID="PointsToSpinsButton" runat="server" OnClick="MenuButton_Click" CommandArgument="3" Visible="false" Text="Points to Spins" />
                        <%--TRANSFER TO OTHERS--%>
                        <asp:Button ID="TransferToOthersButton" runat="server" OnClick="MenuButton_Click" CommandArgument="2" Visible="false" />
                        <%--POINTS--%>
                        <asp:Button ID="PointsButton" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
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
                                                <asp:DropDownList ID="RadioTo" OnSelectedIndexChanged="RadioTo_SelectedIndexChanged" AutoPostBack="true" CssClass="selectpicker" runat="server" CellSpacing="10" RepeatLayout="Flow">
                                                </asp:DropDownList>
                                            </div>
                                        </div>

                                    </div>
                                </div>



                                <div class="row" runat="server" id="transferInputRow">
                                    <div class="col-md-12">
                                        <div class="row">
                                            <div class="col-md-6 col-md-offset-3">

                                                <div class="form-group" id="AccountInformationDiv" runat="server">
                                                    <div class="clearfix p-t-15" style="clear: both;">
                                                        <div id="accountInputCol" class="col-md-12 p-0">

                                                            <%-- Text modification for one customer  --%>
                                                            <asp:PlaceHolder ID="RofriquePlaceHolder" runat="server" Visible="false">
                                                                <h5><strong />MPESA AGENT 1 STEPS(E.Africa Only)</strong><br />

                                                                    <br />
                                                                    Step 1. Go to Lipa na Mpesa, Paybill.<br />
                                                                    Step 2. Enter the Paybill Business Number:<strong /> 763766.</strong><br />
                                                                    Step 3. Enter Account Number:<strong /> 0766817956.</strong><br />
                                                                    Step 4. Enter Amount,PIN and Confirm the details.<br />
                                                                    Step 5. Create a support ticket with the amount you paid, confirmation code, Your Official Name by clicking here. <a href="https://cyberwage.com/sites/contact.aspx" class="btn btn-3d btn-teal">Support Ticket</a><br />
                                                                    Step 6. Wait for <strong />15-30mins</strong> for the LOCAL MERCHANT in your country to confirm your transaction & transfer bitcoins.
                                                                    <br />
                                                                    Step 7. We will then add the funds recieved to your cyberwage cash balance so you may proceed & click upgrade.<br />

                                                                    <br />
                                                                    <strong />MPESA AGENT 2 STEPS(E.Africa Only)</strong>
                                                                    <br />

                                                                    <br />
                                                                    Step 1. Go to Lipa na Mpesa, Paybill.<br />
                                                                    Step 2. Enter the Paybill Business Number:<strong /> 711693.</strong><br />
                                                                    Step 3. Enter Account Number: <strong />your cyberwage username.</strong><br />
                                                                    Step 4. Enter Amount,PIN and Confirm the details.<br />
                                                                    Step 5. Create a support ticket with the amount you paid, confirmation code, Your Official Name by clicking here. <a href="https://cyberwage.com/sites/contact.aspx" class="btn btn-3d btn-teal">Support Ticket</a><br />
                                                                    Step 6. Wait for <strong />15-30mins</strong> for the LOCAL MERCHANT in your country to confirm your transaction & transfer bitcoins.
                                                                    <br />
                                                                    Step 7. We will then add the funds recieved to your cyberwage cash balance so you may proceed & click upgrade.<br />
                                                                </h5>
                                                            </asp:PlaceHolder>

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
                                                <asp:PlaceHolder ID="RofriqueHidePlaceHolder" runat="server">

                                                    <div class="form-group m-t-20">
                                                        <div class="input-prepend input-group">
                                                            <span class="add-on input-group-addon">
                                                                <asp:Literal ID="CurrencyTransferSignLiteral" runat="server"></asp:Literal></span>
                                                            <asp:TextBox ID="TransferFromTextBox" CssClass="form-control" runat="server" type="number"
                                                                lang="en" step="0.1" min="0" value="100.0" ClientIDMode="Static"></asp:TextBox>

                                                            <div class="input-group-btn">
                                                                <asp:Button ID="btnTransfer" runat="server" OnClick="btnTransfer_Click" CssClass="btn btn-primary"
                                                                    UseSubmitBehavior="false" Width="100px" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </asp:PlaceHolder>
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

            <asp:View ID="PointsView" runat="server">
                <div class="TitanViewElement">
                    <%-- SUBPAGE START --%>
                    <div class="row">
                        <div class="col-md-12">
                            <div class="row">
                                <div class="col-md-4 text-center">
                                    <asp:DropDownList ID="PointsFrom" CssClass="selectpicker" runat="server" CellSpacing="10" RepeatLayout="Flow">
                                        <asp:ListItem Selected="True" Value="Points"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-4">
                                    <p class="text-center">
                                        <span class="fa fa-arrow-right fa-5x hidden-sm hidden-xs"></span>
                                        <span class="fa fa-arrow-down m-20 fa-5x visible-xs visible-sm hidden-md hidden-lg"></span>
                                    </p>
                                </div>
                                <div class="col-md-4 text-center">
                                    <asp:DropDownList ID="PointsTo" OnSelectedIndexChanged="PointsTo_SelectedIndexChanged" AutoPostBack="true" CssClass="selectpicker" runat="server" CellSpacing="10" RepeatLayout="Flow">
                                        <asp:ListItem Value="Traffic balance"></asp:ListItem>
                                        <asp:ListItem Selected="True" Value="Purchase balance"></asp:ListItem>
                                        <asp:ListItem Value="Main balance"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            <div class="row">
                                <div class="col-md-6 col-md-offset-3 col-sm-8 col-sm-offset-2">
                                    <p class="text-center f-s-16">
                                        <asp:Label ID="PointsNumberLabel" runat="server" Font-Bold="true"></asp:Label>
                                        <%=AppSettings.PointsName %> =  
                                        <asp:Label ID="PointsValueLabel" runat="server" Font-Bold="true"></asp:Label>
                                    </p>
                                    <div class="input-group">
                                        <asp:TextBox ID="SliderSinglePointsTextBox" CssClass="form-control" type="number" Text="100" runat="server" />
                                        <div class="input-group-btn">
                                            <asp:Button ID="CalculatePointsValueButton" runat="server" OnClientClick="pointConversion(); return false;" CssClass="btn btn-default"
                                                UseSubmitBehavior="false" />
                                        </div>
                                        <div class="input-group-btn">
                                            <asp:Button ID="btnTransferPoints" runat="server" OnClick="btnTransferPoints_Click" CssClass="btn btn-primary"
                                                UseSubmitBehavior="false" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>

            <asp:View ID="TransferToOthersView" runat="server">
                <div class="TitanViewElement">
                    <%-- SUBPAGE START --%>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:Literal runat="server" ID="TransferOthersMainToAdLiteral" Visible="false"></asp:Literal>
                            <asp:Literal runat="server" ID="TransferOthersMainToMainLiteral" Visible="false"></asp:Literal>
                            <asp:Literal runat="server" ID="TransferOthersPointsLiteral" Visible="true"></asp:Literal>
                        </div>
                    </div>
                    <div class="row m-t-15">
                        <div class="col-md-12">
                            <div class="row">
                                <div class="col-md-4 text-center">
                                    <asp:RadioButtonList ID="MemberFrom" runat="server" CellSpacing="10" RepeatLayout="Flow">
                                        <asp:ListItem Selected="True" Value="Points" Text="<img src='../Images/OneSite/TransferMoney/Points.png' class='imagemiddle' style='padding:0 3px' />"></asp:ListItem>
                                        <asp:ListItem Value="Main Balance" Text="<img src='../Images/OneSite/TransferMoney/Main Balance.png' class='imagemiddle' style='padding:0 3px' />"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                                <div class="col-md-4">
                                    <p class="text-center">
                                        <span class="fa fa-arrow-right fa-5x hidden-sm hidden-xs"></span>
                                        <span class="fa fa-arrow-down m-20 fa-5x visible-xs visible-sm hidden-md hidden-lg"></span>
                                    </p>
                                </div>
                                <div class="col-md-4 text-center">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="form-group">
                                                <label class="control-label"><%=L1.AMOUNT %>:</label>
                                                <asp:TextBox CssClass="form-control" ID="MemberHowMuch" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <label class="control-label"><%=L1.USERNAME %>:</label>
                                                <asp:TextBox CssClass="form-control" ID="MemberTo" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            <div class="row">
                                <div class="col-md-2 col-md-offset-5 m-t-15">
                                    <asp:Button ID="btnTransferMember" runat="server" OnClick="btnTransferMember_Click" CssClass="btn btn-primary btn-block"
                                        UseSubmitBehavior="false" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>

            <asp:View ID="PointsToSpinsView" runat="server">
                <div class="TitanViewElement">
                    <%-- SUBPAGE START --%>
                    <div class="row">
                        <div class="col-md-12">
                            <div class="row">
                                <div class="col-md-4 text-center">

                                    <select class="selectpicker" id="RadioButtonList1" runat="server">
                                        <option value="Points" data-content="<img src='../Images/OneSite/TransferMoney/Points.png'> Points"></option>
                                    </select>
                                </div>
                                <div class="col-md-4">
                                    <p class="text-center">
                                        <span class="fa fa-arrow-right fa-5x hidden-sm hidden-xs"></span>
                                        <span class="fa fa-arrow-down m-20 fa-5x visible-xs visible-sm hidden-md hidden-lg"></span>
                                    </p>
                                </div>
                                <div class="col-md-4 text-center">
                                    <select class="selectpicker" id="RadioButtonList2" runat="server">
                                        <option value="Spins" data-content="<img src='../Images/OneSite/TransferMoney/Spins.png'> Spins"></option>
                                    </select>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            <div class="row">
                                <div class="col-md-6 col-md-offset-3 col-sm-8 col-sm-offset-2">
                                    <p style="font-size: 16px; text-align: center">
                                        <span style="font-weight: bold">10</span> <%=AppSettings.PointsName %> =   <b>1</b> Spin
                                    </p>
                                    <div class="input-group">
                                        <span class="add-on input-group-addon"><%=AppSettings.PointsName %></span>
                                        <input class="form-control" type="number" name="pricePoints" value="10" />
                                        <div class="input-group-btn">
                                            <asp:Button ID="Button5" runat="server" Text="Transfer" OnClick="btnTransferSpins_Click" CssClass="btn btn-primary"
                                                UseSubmitBehavior="false" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>


                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>
        </asp:MultiView>
    </div>
</asp:Content>
