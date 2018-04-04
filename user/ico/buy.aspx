<%@ Page Language="C#" AutoEventWireup="true" CodeFile="buy.aspx.cs" Inherits="user_ico_buy" MasterPageFile="~/User.master" %>

<%@ Import Namespace="Titan.Cryptocurrencies" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script>
        $(function () {
            watchTokenNumber();
            $('#<%=NumberOfTokensTextBox.ClientID %>').keyup(function () {
                watchTokenNumber();
            });
        });

        function watchTokenNumber() {

            var numberOfTokens = $('#<%=NumberOfTokensTextBox.ClientID %>').val() || 0;
            numberOfTokens = isNaN(numberOfTokens) ? 0 : numberOfTokens;

            var tokenPrice = $('#tokenPrice').text() || 0;
            var totalCurrencyValue = numberOfTokens * tokenPrice;
            $('#totalCurrencyValue').text(totalCurrencyValue.toFixed(<%=CoreSettings.GetMaxDecimalPlaces()%>));

            var BTCPrice = parseFloat($('#BTCPrice').text()) || 0;
            var totalBTCValue = numberOfTokens * BTCPrice;
            $('#totalBTCValue').text(totalBTCValue.toFixed(8));
        }
    </script>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=L1.BUY %> <%=TokenCryptocurrency.Code %></h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=String.Format(U6012.ENTERAMOUNT, TokenCryptocurrency.Code) %></p>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <asp:Panel ID="SuccessPanel" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
                <asp:Literal ID="SuccessTextLiteral" runat="server"></asp:Literal>
            </asp:Panel>

            <asp:Panel ID="ErrorPanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                <asp:Literal ID="ErrorTextLiteral" runat="server"></asp:Literal>
            </asp:Panel>
        </div>
    </div>

    <div class="tab-content">
        <div class="row">
            <div class="col-md-12">
                <titan:ICOCurrentStageInformation runat="server" />
            </div>
        </div>


        <div class="row">
            <div class="col-md-12">
                <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger m-b-15">
                    <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                </asp:Panel>

                <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="alert alert-success m-b-15">
                    <asp:Literal ID="SuccMessage" runat="server"></asp:Literal>
                </asp:Panel>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="alert alert-danger m-b-15"
                    ValidationGroup="RegisterUserValidationGroup" DisplayMode="List" />

                <div class="form-horizontal">

                    <div class="form-group">
                        <div class="col-md-6">
                            <div class="input-prepend input-group">
                                <span class="add-on input-group-addon">
                                    <%=TokenCryptocurrency.Code %>
                                </span>
                                <asp:TextBox autocomplete="off" ID="NumberOfTokensTextBox"
                                    runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                            </div>

                            <asp:RegularExpressionValidator ValidationGroup="RegisterUserValidationGroup" ID="RegularExpressionValidator5" runat="server"
                                ErrorMessage="Invalid format" Display="Dynamic" CssClass="text-danger"
                                ValidationExpression="[0-9]{1,10}" ControlToValidate="NumberOfTokensTextBox" Text="">*</asp:RegularExpressionValidator>

                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="NumberOfTokensTextBox"
                                Display="Dynamic" CssClass="text-danger"
                                ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:RequiredFieldValidator>
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="col-md-6">
                            <div class="row">
                                <label class="control-label col-md-12 text-left">
                                    <asp:Literal ID="USDValueLiteral" runat="server"></asp:Literal> (<%=U5001.TOTAL %>: <strong><%=AppSettings.Site.CurrencySign %><span id="totalCurrencyValue"></span></strong>)
                                </label>
                                <label class="control-label col-md-12 text-left" runat="server" id="BTCValueLabel">
                                    <asp:Literal ID="BTCValueLiteral" runat="server"></asp:Literal> (<%=U5001.TOTAL %>: <strong><span id="totalBTCValue"></span>BTC</strong>)
                                </label>
                                <label class="control-label col-md-12 text-left">
                                    <br />
                                    <asp:Literal ID="MaxVolumeLiteral" runat="server"></asp:Literal>
                                </label>
                            </div>
                        </div>
                    </div>



                    <div class="form-group">

                        <div class="col-md-6">
                            <label class="control-label"><%=L1.VERIFICATION %>: <span class="text-danger">*</span></label>
                            <titan:Captcha runat="server" ID="TitanCaptcha" ValidationGroup="RegisterUserValidationGroup" />
                            <asp:CustomValidator OnServerValidate="Validate_Captcha" runat="server" ValidationGroup="RegisterUserValidationGroup"
                                Display="Dynamic" CssClass="text-danger" ID="CustomValidator1" EnableClientScript="False">*</asp:CustomValidator>
                        </div>
                    </div>


                    <div class="form-group">
                        <div class="col-md-4">

                            <asp:Button ID="BuyFromPurchaseBalanceButton" OnClientClick="if (Page_ClientValidate()){this.disabled = true; this.className = 'rbutton-loading'; this.value='';}"
                                UseSubmitBehavior="false" OnClick="BuyFromPurchaseBalanceButton_Click" ValidationGroup="RegisterUserValidationGroup" runat="server" CssClass="btn btn-inverse btn-block btn-lg" />

                            <asp:Button ID="BuyFromBTCWalletButton" OnClientClick="if (Page_ClientValidate()){this.disabled = true; this.className = 'rbutton-loading'; this.value='';}"
                                UseSubmitBehavior="false" OnClick="BuyFromBTCWalletButton_Click" ValidationGroup="RegisterUserValidationGroup" runat="server" CssClass="btn btn-inverse btn-block btn-lg" />

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
