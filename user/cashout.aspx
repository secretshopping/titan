<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="cashout.aspx.cs" Inherits="About" %>

<%@ MasterType VirtualPath="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <link href="Scripts/default/assets/plugins/bootstrap-select/css/bootstrap-select.min.css" rel="stylesheet" type="text/css">
    <script src="Scripts/default/assets/plugins/bootstrap-select/js/bootstrap-select.min.js"></script>

    <script type="text/javascript">

        data = {};

        function pageLoad() {
            $("#<%=CashoutButton.ClientID %>").click(function (e) { e.preventDefault(); });
            $("#<%=SendWithdrawViaRepresentativeButton.ClientID %>").click(function (e) { e.preventDefault(); });
            data.email = $("body").find("#<%=Account.ClientID %>").text();
            data.amount = $("#<%=AmountToCashoutTextBox.ClientID %>").val();
            data.noPP = false;
            updateListener(updateModal);
            $('#confirmationModal').modal('hide');

            if ($('#<%=Account.ClientID %>').length) {
                $('#accountInputCol').removeClass('col-md-4');
            } else {
                $('#accountInputCol').addClass('col-md-4');
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
        }


        function askForConfirmation(parameter) {
            Page_ClientValidate();
            if (Page_IsValid) {
                updateListener(updateModal);
                $('#confirmationModal').modal({ 'backdrop': true, 'show': true });
                let promise = new Promise(function (resolve, reject) {
                    let confirmButton = document.getElementById('confirmButton');
                    confirmButton.addEventListener("click", function () {
                        //console.log('confirm button clicked');
                        resolve();
                    });
                });
                promise.then(function () {
                    __doPostBack(parameter.name, '');
                });
            }
        }

        function askForConfirmationViaRepresentative(parameter) {
            Page_ClientValidate();
            if (Page_IsValid) {
                updateListenerForRep(updateModalForRep);
            }
            $('#confirmationModal').modal({ 'backdrop': true, 'show': true });
            let promise = new Promise(function (resolve, reject) {
                let confirmButton = document.getElementById('confirmButton');
                confirmButton.addEventListener("click", function () {
                    console.log('confirm button clicked');
                    resolve();
                });
            });
            promise.then(function () {
                __doPostBack(parameter.name, '');

            });
        }

        function updateListener(callback) {
            data.paymentProcessorImagePath = $("select.selectpicker option:selected").attr('data-content');
            data.account = $('#ctl00_PageMainContent_Account').val();
            data.amount = $("#<%=AmountToCashoutTextBox.ClientID %>").val();
            data.noPP = Number.isInteger(parseInt($('input[type=radio]:checked').val())) ? true : false;
            callback(data);
            $('.selectpicker').on('changed.bs.select', function () {
                data.paymentProcessorImagePath = $("select.selectpicker option:selected").val();
                data.noPP = Number.isInteger(parseInt($('input[type=radio]:checked').val())) ? true : false;
                callback(data);
            });
            $('#ctl00_PageMainContent_Account').keyup(function () {
                data.account = $('#ctl00_PageMainContent_Account').val();
                callback(data);
            });
            $("#<%=AmountToCashoutTextBox.ClientID %>").keyup(function () {
                data.amount = $("#<%=AmountToCashoutTextBox.ClientID %>").val();
                callback(data);
            });
        }

        function updateListenerForRep(callback) {
            try {
                data.amount = document.getElementsByName("price")[0].value;
                data.feepercent = document.getElementById('ctl00_PageMainContent_RepFeeLabel').innerText;
                data.feepercent = data.feepercent.replace('%', '');
                data.fee = (data.feepercent / 100) * data.amount;
            }
            catch (err)
            { }
            callback(data);
        }

        function updateModal(data) {
            var $modal = $('#confirmationModal');
            $modal.find('.modal-body').html('<h3 class="m-t-0 text-center"><%=U6006.SURETOWITHDRAW %> <%=AppSettings.Site.MulticurrencySign %>' + data.amount + '?</h3><p class="text-center p-10">' + data.paymentProcessorImagePath + '</p><h4 class="text-center">Account: <span class="text-danger">' + data.account + '</span></h4>');
        }

        function updateModalForRep(data) {
            var $modal = $('#confirmationModal');
            $modal.find('.modal-body').html('<h3 class="m-t-0 text-center"><%=U6006.SURETOWITHDRAW %> ' + data.amount + '<%=AppSettings.Site.CurrencySign%>?</h3><h4 class="text-center"><span class="text-danger">Withdrawal fee: ' + data.fee + '<%=AppSettings.Site.CurrencySign%> </span></h4>');
        }
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <h1 class="page-header"><%= L1.CASHOUT %></h1>

    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%= L1.CASHOUTINFO %></p>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <div class="TitanViewPage">
                    <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                        <asp:Button ID="MaxWithdrawalsButton" runat="server" OnClick="MenuButton_Click" CommandArgument="2" Visible="false" />
                        <asp:Button ID="WithdrawHistoryButton" runat="server" OnClick="MenuButton_Click" CommandArgument="4" />
                        <asp:Button ID="WithdrawViaRepresentativeButton" runat="server" OnClick="MenuButton_Click" CommandArgument="5" Visible="false" />
                        <asp:Button ID="CommissionButton" runat="server" OnClick="MenuButton_Click" CommandArgument="3" Visible="false" />
                        <asp:Button ID="ERC20TokenButton" runat="server" OnClick="MenuButton_Click" CommandArgument="1" Visible="false" />
                        <asp:Button ID="XRPWithdrawalButton" runat="server" OnClick="MenuButton_Click" CommandArgument="1" Visible="false"/>
                        <asp:Button ID="BTCWithdrawalButton" runat="server" OnClick="MenuButton_Click" CommandArgument="1" Visible="false" />
                        <asp:Button ID="MainBalanceButton" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>

    <div class="tab-content">
        <asp:Panel runat="server" ID="UnpaidCreditLineInfo" CssClass="alert alert-warning" Visible="false">
            <%=U6008.REPAYCREDITLINETOWITHDRAW %>
        </asp:Panel>
        <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
            <asp:View runat="server" ID="MoneyPayout">
                <div class="TitanViewElement">
                    <asp:UpdatePanel runat="server" ID="MoneyPayoutUpdatePanel">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="RadioFrom" EventName="SelectedIndexChanged" />
                        </Triggers>
                        <ContentTemplate>
                            <div class="row">
                                <div class="col-md-12">

                                    <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger">
                                        <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                                    </asp:Panel>
                                    <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="alert alert-success">
                                        <asp:Literal ID="SuccMessage" runat="server"></asp:Literal>
                                    </asp:Panel>
                                    <asp:Literal ID="GenerateLiteral" runat="server"></asp:Literal>

                                    <asp:ValidationSummary ID="CashoutValidationSummary" runat="server" CssClass="alert alert-danger"
                                        ValidationGroup="CashoutValidationGroup" DisplayMode="List" />
                                    <asp:Panel ID="WarningPanel" runat="server" Visible="false" CssClass="alert alert-warning">
                                        <asp:Literal ID="WarningLiteral" runat="server"></asp:Literal>
                                    </asp:Panel>
                                </div>
                            </div>


                            <div class="row">
                                <div class="col-md-12">
                                    <div class="form-horizontal">
                                        <div class="row" runat="server" id="PayoutPlaceHolder">
                                            <div class="col-lg-4 col-md-6 m-t-20" id="transfertable">


                                                <asp:DropDownList ID="RadioFrom" CssClass="selectpicker" runat="server" CellSpacing="10" RepeatLayout="Flow" OnSelectedIndexChanged="RadioFrom_SelectedIndexChanged" AutoPostBack="true">
                                                </asp:DropDownList>

                                                <br />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="RadioFrom"
                                                    ValidationGroup="CashoutValidationGroup" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                            </div>
                                            <div class="col-lg-6 col-md-6">
                                                <h3><%=L1.MAINBALANCE %>:
                                            <asp:Literal ID="MainBalanceLiteral" runat="server"></asp:Literal></h3>
                                                <div class="form-group">
                                                    <label class="control-label col-md-2"><%=U6006.MINIMUM %>:</label>
                                                    <div class="col-md-4">
                                                        <b>
                                                            <asp:Label ID="MinLimitLabel" CssClass="form-control no-border" runat="server"></asp:Label></b>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="control-label col-md-2"><%=U5004.MAXIMUM %>:</label>
                                                    <div class="col-md-4">
                                                        <b>
                                                            <asp:Label ID="MaxLimitLabel" CssClass="form-control no-border" runat="server"></asp:Label></b>
                                                    </div>
                                                </div>
                                                <div class="form-group" id="feesAndInfoDiv">
                                                    <label class="control-label col-md-2"><%=U3500.CASHOUT_FEES %>:</label>
                                                    <div class="col-md-4">
                                                        <asp:Label ID="FeesLabel" runat="server" CssClass="form-control no-border"></asp:Label>
                                                    </div>
                                                    <div class="clearfix p-t-15" style="clear: both;">
                                                        <label class="control-label col-md-2"><%=U3500.CASHOUT_ACCOUNT %>:</label>
                                                        <div id="accountInputCol" class="col-md-6">
                                                            <div id="accountInputGroup" class="input-group width-full">
                                                                <asp:Button ID="AddNewAccount" runat="server" CssClass="btn btn-primary btn-block" Visible="false" OnClick="ChangeAccountButton_Click" />
                                                                <asp:Label ID="InfoLabel" runat="server"></asp:Label>
                                                                <asp:TextBox ID="Account" runat="server" CssClass="form-control" autocomplete="off"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="Account" ValidationGroup="CashoutValidationGroup" Display="Dynamic" CssClass="text-danger" Text="Required" />
                                                                <asp:RegularExpressionValidator ID="REValidator" runat="server" ControlToValidate="Account" Display="Dynamic" ValidationGroup="CashoutValidationGroup" ValidationExpression="[a-zA-Z0-9.,!@#$%^&*()+-/?\\|:=_]{1,120}" CssClass="text-danger" />
                                                                <div class="input-group-btn">
                                                                    <asp:Button ID="ChangeAccountButton" runat="server" CssClass="btn btn-primary" Width="115px" Visible="false" OnClick="ChangeAccountButton_Click" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="form-group">
                                                    <label class="control-label col-md-2"><%=L1.AMOUNT %>:</label>
                                                    <div class="col-md-4">
                                                        <div class="input-group">
                                                            <span class="add-on input-group-addon"><%=AppSettings.Site.MulticurrencySign %></span>
                                                            <asp:TextBox ID="AmountToCashoutTextBox" runat="server" CssClass="form-control" autocomplete="off"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ValidationGroup="CashoutValidationGroup" ID="AmountToCashoutRegularExpressionValidator" runat="server"
                                                            ValidationExpression="[0-9]+(\.[0-9]{1,2})?" ControlToValidate="AmountToCashoutTextBox" Display="Dynamic" CssClass="alert-danger">*</asp:RegularExpressionValidator>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="form-group" runat="server" id="PINDiv1">
                                                    <label class="control-label col-md-2">PIN: </label>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="PIN" runat="server" CssClass="form-control" MaxLength="4" autocomplete="off"></asp:TextBox>
                                                        <asp:RegularExpressionValidator ValidationGroup="CashoutValidationGroup" ID="RegularExpressionValidator3" runat="server"
                                                            ValidationExpression="[0-9]{4}" ControlToValidate="PIN" Display="Dynamic" CssClass="alert-danger">*</asp:RegularExpressionValidator>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="PIN"
                                                            ValidationGroup="CashoutValidationGroup" Display="Dynamic" CssClass="text-danger">*</asp:RequiredFieldValidator>
                                                    </div>
                                                </div>
                                                <div class="form-group" runat="server" id="ConfirmationCodePlaceHolder2" visible="false">
                                                    <label class="control-label col-md-2"><%=U6000.SECURITYCODE %></label>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="FiatConfirmationCodeTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="FiatConfirmationCodeTextBox"
                                                            Display="Dynamic" ValidationGroup="FiatConfirmValidationGroup" CssClass="text-danger">*</asp:RequiredFieldValidator>
                                                        <asp:CustomValidator runat="server" ControlToValidate="FiatConfirmationCodeTextBox" OnServerValidate="FiatCodeValidator_ServerValidate"
                                                            ValidateEmptyText="true" Display="Dynamic" ID="FiatCodeValidator" ValidationGroup="FiatConfirmValidationGroup" CssClass="text-danger"></asp:CustomValidator>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <div class="col-md-3">
                                                        <asp:Button ID="CashoutButton" runat="server"
                                                            ValidationGroup="CashoutValidationGroup" CssClass="btn btn-inverse btn-block" OnClientClick="if (Page_IsValid) { askForConfirmation(this) } else { return false; }" OnClick="CashoutButton_Click" Width="115px" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <div class="col-md-3">
                                                        <asp:Button ID="CashoutButtonConfirm" OnClick="CashoutButtonConfirm_Click" runat="server" CssClass="btn btn-inverse btn-block" Visible="false"
                                                            ValidationGroup="FiatConfirmValidationGroup" Width="115px" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>

                    </asp:UpdatePanel>
                </div>
            </asp:View>
            <asp:View runat="server" ID="CryptocurrencyPayout" OnActivate="CryptocurrencyPayout_Activate">
                <div class="TitanViewElement">
                    <asp:UpdatePanel runat="server">
                        <ContentTemplate>
                            <div class="row">
                                <div class="col-md-12">
                                    <asp:ValidationSummary runat="server" CssClass="alert alert-danger"
                                        ValidationGroup="CryptocurrencyConfirmValidationGroup" DisplayMode="List" />

                                    <asp:Panel ID="CryptocurrencyErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger">
                                        <asp:Literal ID="CryptocurrencyErrorMessageLiteral" runat="server"></asp:Literal>
                                    </asp:Panel>

                                    <asp:Panel ID="CryptocurrencySuccessMessagePanel" runat="server" Visible="false" CssClass="alert alert-success">
                                        <asp:Literal ID="CryptocurrencySuccessMessageLiteral" runat="server"></asp:Literal>
                                    </asp:Panel>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-12">
                                    <div class="form-horizontal">
                                        <div class="row">
                                            <div class="col-md-12">
                                                <asp:PlaceHolder runat="server" ID="AhmedInfoPlaceHolder" Visible="false">
                                                    <p class='alert alert-info'>
                                                        <asp:Literal ID="AhmedInfoLiteral" runat="server"/>
                                                    </p>
                                                </asp:PlaceHolder>
                                                <p class='alert alert-info'>
                                                    <asp:Literal ID="CryptocurrencyValueLiteral" runat="server" />
                                                    <br />
                                                    <asp:Literal ID="CryptocurrencyWithdrawalSourceLiteral" runat="server" />
                                                </p>
                                                <div class="form-group">

                                                    <label class="control-label col-md-2 text-left">
                                                        <asp:Image ID="CryptocurrencyImage" runat="server" />
                                                    </label>
                                                    <div class="col-md-4">
                                                        <asp:PlaceHolder runat="server" ID="WithdrawalPacksPlaceHolder" Visible="false">
                                                            <p>
                                                                <asp:Literal runat="server" ID="WithdrawalPacksLiteral" />
                                                            </p>
                                                        </asp:PlaceHolder>

                                                        <p class="no-border">
                                                            <asp:Literal runat="server" ID="MinimumCryptocurrencyAmountLiteral"></asp:Literal>
                                                            <asp:Literal runat="server" ID="MaximumCryptocurrencyAmountLiteral"></asp:Literal>
                                                            <asp:Literal runat="server" ID="CryptocurrencyFeeLiteral"></asp:Literal>
                                                            <asp:Literal runat="server" ID="WithdrawTotalCryptocurrencyLiteral"></asp:Literal>
                                                        </p>

                                                    </div>
                                                </div>

                                                <div class="form-group">
                                                    <label class="control-label col-md-2"><%=L1.AMOUNT %>:</label>
                                                    <div class="col-md-4">
                                                        <div class="input-prepend input-group">
                                                            <span class="add-on input-group-addon">
                                                                <asp:Literal ID="CurrencySignLiteral" runat="server"></asp:Literal></span>
                                                            <asp:TextBox ID="WithdrawCryptocurrencyAmountTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                                                        </div>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="WithdrawCryptocurrencyAmountTextBox"
                                                            Display="Dynamic" ValidationGroup="CryptocurrencyConfirmValidationGroup" CssClass="text-danger">*</asp:RequiredFieldValidator>
                                                    </div>
                                                </div>

                                                <div class="form-group">
                                                    <label class="control-label col-md-2">
                                                        <asp:Label runat="server" ID="CryptocurrencyAddressLabel" />:
                                                    </label>
                                                    <div class="col-md-6">
                                                        <div class="input-group">
                                                            <asp:TextBox ID="WithdrawCryptocurrencyAddressTextBox" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                            <asp:DropDownList ID="CoinbaseAddressesDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="CoinbaseAddressesDDL_SelectedIndexChanged" Visible="false" AutoPostBack="true" />
                                                            <div class="input-group-btn">
                                                                <asp:Button ID="ChangeCryptocurrencyAddressButton" runat="server" CssClass="btn btn-primary" Width="115px" OnClick="ChangeAccountButton_Click" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="form-group" runat="server" id="BtcPinDiv">
                                                    <label class="control-label col-md-2">PIN: </label>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="CryptoPINTextBox" runat="server" CssClass="form-control" MaxLength="4" autocomplete="off"></asp:TextBox>
                                                        <asp:RegularExpressionValidator ValidationGroup="CryptocurrencyConfirmValidationGroup" ID="RegularExpressionValidator2" runat="server"
                                                            ValidationExpression="[0-9]{4}" ControlToValidate="CryptoPINTextBox" Display="Dynamic" CssClass="alert-danger">*</asp:RegularExpressionValidator>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="CryptoPINTextBox"
                                                            ValidationGroup="CryptocurrencyConfirmValidationGroup" Display="Dynamic" CssClass="text-danger">*</asp:RequiredFieldValidator>
                                                    </div>
                                                </div>

                                                <div class="form-group" runat="server" id="ConfirmationCodePlaceHolder" visible="false">
                                                    <label class="control-label col-md-2"><%=U6000.SECURITYCODE %></label>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="BtcConfirmationCodeTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="BtcConfirmationCodeTextBox"
                                                            Display="Dynamic" ValidationGroup="CryptocurrencyConfirmValidationGroup" CssClass="text-danger">*</asp:RequiredFieldValidator>
                                                        <asp:CustomValidator runat="server" ControlToValidate="BtcConfirmationCodeTextBox" OnServerValidate="BtcCodeValidator_ServerValidate"
                                                            ValidateEmptyText="true" Display="Dynamic" ID="BtcCodeValidator" ValidationGroup="CryptocurrencyConfirmValidationGroup" CssClass="text-danger"></asp:CustomValidator>
                                                    </div>
                                                </div>

                                                <div class="form-group">
                                                    <div class="col-md-3 col-lg-2">
                                                        <asp:Button ID="WithdrawCryptocurrencyButton" OnClick="WithdrawCryptocurrencyButton_Click" runat="server" CssClass="btn btn-block btn-inverse" ValidationGroup="CryptocurrencyConfirmValidationGroup" />
                                                    </div>
                                                </div>

                                                <div class="form-group">
                                                    <div class="col-md-3  col-lg-2">
                                                        <asp:Button ID="WithdrawCryptocurrencyConfirmButton" OnClick="WithdrawCryptocurrencyConfirmButton_Click" runat="server" CssClass="btn btn-inverse btn-block" Visible="false"
                                                            ValidationGroup="CryptocurrencyConfirmValidationGroup" />
                                                    </div>
                                                </div>


                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </asp:View>

            <asp:View runat="server" ID="PayoutProportions">
                <div class="TitanViewElement">


                    <asp:GridView ID="ProportionsGridView" runat="server" AllowSorting="True" AutoGenerateColumns="False" DataSourceID="ProportionsGridViewSqlDataSource"
                        OnRowDataBound="ProportionsGridView_RowDataBound" PageSize="30">
                        <Columns>
                            <asp:BoundField HeaderText="Processor" SortExpression='Processor' DataField="Processor" />
                            <asp:BoundField HeaderText="TotalIn" SortExpression='TotalIn' DataField="TotalIn" />
                            <asp:BoundField HeaderText="TotalIn" SortExpression='TotalIn' DataField="TotalIn" />
                            <asp:BoundField HeaderText="TotalOut" SortExpression='TotalOut' DataField="TotalOut" />
                            <asp:BoundField HeaderText="Id" SortExpression='Id' DataField="Id" />
                        </Columns>
                    </asp:GridView>
                    <asp:SqlDataSource ID="ProportionsGridViewSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="ProportionsGridViewSqlDataSource_Init"></asp:SqlDataSource>
                    <br />
                    <span style="font-size: smaller">* <%=U5004.MAXWITHDRAWDESC %></span>
                    <asp:PlaceHolder ID="MaxWithdrawalAllowedPerInvestmentPercentPlaceHolder" runat="server" Visible="false">
                        <br />
                        <br />
                        <h5><%=U5004.PAIDIN %>:
                            <asp:Literal ID="TotalPaidInLiteral" runat="server"></asp:Literal></h5>
                        <h5><%=DEFAULT.TOTALCASHOUT %>:
                            <asp:Literal ID="TotalCashoutLiteral" runat="server"></asp:Literal>
                        </h5>
                        <h5>
                            <asp:Literal ID="HowmuchMoreCanBeWithdrawnLiteral" runat="server"></asp:Literal></h5>
                    </asp:PlaceHolder>
                </div>
            </asp:View>
            <asp:View runat="server" ID="CommissionPayout">
                <div class="TitanViewElement">

                    <div class="row">
                        <div class="col-md-12">
                            <asp:Panel ID="CommissionErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger">
                                <asp:Literal ID="CommissionErrorMessage" runat="server"></asp:Literal>
                            </asp:Panel>
                            <asp:Panel ID="CommissionSuccMessagePanel" runat="server" Visible="false" CssClass="alert alert-success">
                                <asp:Literal ID="CommissionSuccMessage" runat="server"></asp:Literal>
                            </asp:Panel>

                            <asp:ValidationSummary ID="CommissionCashoutValidationSummary" runat="server" CssClass="alert alert-danger"
                                ValidationGroup="CommissionCommissionCashoutValidationGroup" DisplayMode="List" />
                            <asp:Panel ID="CommissionWarningPanel" runat="server" Visible="false" CssClass="alert alert-warning">
                                <asp:Literal ID="CommissionWarningLiteral" runat="server"></asp:Literal>
                            </asp:Panel>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-horizontal">
                                <div class="row" runat="server" id="CommissionPayoutPlaceHolder">
                                    <div class="col-md-3 m-t-20">
                                        <asp:RadioButtonList ID="CommissionRadioFrom" runat="server" CellSpacing="10" RepeatLayout="Flow">
                                        </asp:RadioButtonList>
                                        <br />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="CommissionRadioFrom"
                                            ValidationGroup="CommissionCommissionCashoutValidationGroup" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                    </div>
                                    <div class="col-md-6">
                                        <h3><%=U5004.COMMISSIONBALANCE %>:
                                            <asp:Literal ID="CommissionBalanceLiteral" runat="server"></asp:Literal></h3>

                                        <div class="form-group" id="commissionEmailDiv">
                                            <label class="control-label col-md-2">Email:</label>
                                            <div class="col-md-4">
                                                <asp:Label ID="CommissionEmailLabel" runat="server" CssClass="form-control no-border" ForeColor="Red"></asp:Label>
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=L1.AMOUNT %>:</label>
                                            <div class="col-md-4">
                                                <div class="input-group">
                                                    <span class="add-on input-group-addon"><%=AppSettings.Site.MulticurrencySign %></span>
                                                    <asp:TextBox ID="CommissionAmountToCashout" runat="server" CssClass="form-control" autocomplete="off"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-group" runat="server" id="PINDiv2">
                                            <label class="control-label col-md-2">PIN: </label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="CommissionPIN" runat="server" CssClass="form-control" MaxLength="4" autocomplete="off"></asp:TextBox>
                                                <asp:RegularExpressionValidator ValidationGroup="CommissionCashoutValidationGroup" ID="RegularExpressionValidator1" runat="server"
                                                    ValidationExpression="[0-9]{4}" ControlToValidate="CommissionPIN" Display="Dynamic" CssClass="alert-danger">*</asp:RegularExpressionValidator>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="CommissionPIN"
                                                    ValidationGroup="CommissionCashoutValidationGroup" Display="Dynamic" CssClass="text-danger">*</asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="col-md-3">
                                                <asp:Button ID="CommissionCashoutButton" runat="server"
                                                    ValidationGroup="CommissionCashoutValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="CommissionCashoutButton_Click" Width="115px" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            </asp:View>
            <asp:View runat="server" ID="WithdrawHistoryView">
                <div class="TitanViewElement">
                    <div class="row">
                        <div class="col-md-12">
                            <asp:GridView ID="WithdrawHistoryGridView" runat="server" AllowSorting="True" AutoGenerateColumns="False" DataSourceID="WithdrawHistorySqlDataSource" DataKeyNames="PayoutRequestId"
                                OnRowDataBound="WithdrawHistoryGridView_RowDataBound" OnRowEditing="WithdrawHistoryGridView_RowEditing" PageSize="30" AllowPaging="true" OnRowUpdating="WithdrawHistoryGridView_RowUpdating">
                                <Columns>
                                    <asp:TemplateField HeaderText="RequestDate" SortExpression="RequestDate">
                                        <ItemTemplate>
                                            <%# Eval("RequestDate").ToString() %>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <%# Eval("RequestDate").ToString() %>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Amount" SortExpression="Amount">
                                        <ItemTemplate>
                                            <%# Eval("Amount").ToString() %>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <%# Eval("Amount").ToString() %>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="PaymentProcessor" SortExpression="PaymentProcessor">
                                        <ItemTemplate>
                                            <%# Eval("PaymentProcessor").ToString() %>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <%# Eval("PaymentProcessor").ToString() %>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="PaymentAddress" SortExpression="PaymentAddress">
                                        <ItemTemplate>
                                            <%# Eval("PaymentAddress").ToString() %>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="EditPaymentAddressTextBox" Text=' <%# Eval("PaymentAddress").ToString() %>' runat="server" />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="IsPaid" SortExpression="IsPaid">
                                        <ItemTemplate>
                                            <%# Eval("IsPaid").ToString() %>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <%# Eval("IsPaid").ToString() %>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="BalanceType" SortExpression="BalanceType">
                                        <ItemTemplate>
                                            <%# Eval("BalanceType").ToString() %>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <%# Eval("BalanceType").ToString() %>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:CommandField HeaderStyle-Width="20px" ButtonType="Image" ShowEditButton="true" ShowCancelButton="true"
                                        UpdateImageUrl="~/Images/GridViewIcons/tick_black_12.png" EditImageUrl="~/Images/GridViewIcons/edit_black_16.png"
                                        CancelImageUrl="~/Images/GridViewIcons/larrow_black_12.png" UpdateText="" EditText="" CancelText=""
                                        CausesValidation="true">
                                        <ControlStyle CssClass="button imageButton" />
                                    </asp:CommandField>
                                </Columns>
                            </asp:GridView>
                            <asp:SqlDataSource ID="WithdrawHistorySqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="WithdrawHistoryGridViewDataSource_Init"
                                UpdateCommand=";"></asp:SqlDataSource>
                        </div>
                    </div>
                </div>
            </asp:View>

            <asp:View runat="server" ID="WithdrawViaRepresentativeView" OnActivate="WithdrawViaRepresentativeView_Activate">
                <div class="TitanViewElement">
                    <asp:UpdatePanel runat="server" ID="WithdrawViaRepresentativeUpdatePanel">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="SendWithdrawViaRepresentativeButton" EventName="Click" />
                        </Triggers>
                        <ContentTemplate>
                            <div class="row">
                                <div class="col-md-12">

                                    <asp:Panel ID="RepresentativeErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger">
                                        <asp:Literal ID="RepresentativeErrorMessage" runat="server"></asp:Literal>
                                    </asp:Panel>

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
                                                                    <label class="control-label"><%=U6010.WITHDRAWALINFO %>:</label>
                                                                </div>
                                                                <div class="col-md-6" style="padding-top: 7px">
                                                                    <asp:PlaceHolder runat="server" ID="WithdrawalOptionsPlaceHolder" />
                                                                </div>
                                                            </div>

                                                            <div class="form-group m-t-20">
                                                                <div class="col-md-3" style="margin-left: 10px">
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
                                                                    <label class="control-label"><%=U6010.WITHDRAWALFEE %>:</label>
                                                                </div>
                                                                <div class="col-md-6">
                                                                    <asp:Label runat="server" CssClass="form-control no-border" ID="RepFeeLabel" />
                                                                </div>
                                                            </div>

                                                            <div class="form-group m-t-20">
                                                                <div class="col-md-3">
                                                                    <label class="control-label"><%=U5004.MESSAGE %>:</label>
                                                                </div>
                                                                <div class="col-md-6" style="padding-top: 7px; margin-left: 10px">
                                                                    <asp:TextBox runat="server" ID="RepresentativeMessage" MaxLength="2500"
                                                                        class="form-control" TextMode="MultiLine" Height="200px" />
                                                                </div>
                                                            </div>
                                                            <asp:RequiredFieldValidator ID="ForConfirmationRequiredFieldValidator" ControlToValidate="RepresentativeMessage" runat="server" ErrorMessage="RequiredFieldValidator" ValidationGroup="forConfirmation"></asp:RequiredFieldValidator>

                                                            <div class="form-group m-t-20">
                                                                <asp:Button ID="SendWithdrawViaRepresentativeButton" runat="server" ValidationGroup="forConfirmation" OnClientClick="askForConfirmationViaRepresentative(this)" OnClick="SendWithdrawViaRepresentativeButtonConfirm_Click" CssClass="btn btn-primary" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>

                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </asp:View>


        </asp:MultiView>
    </div>
</asp:Content>
