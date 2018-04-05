<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CustomAdPackPurchase.ascx.cs" Inherits="Controls_Advertisements_CustomAdPackPurchase" %>

<script type="text/javascript">

    function ShowInProgressAnimation() {
        var animationDiv = document.getElementById("InProgressAnimation");
        if (animationDiv != null)
            animationDiv.style.visibility = "visible";
    }

    function MoneyTBChanged() {
        var animationDiv = document.getElementById("InProgressAnimation");

        if (animationDiv != null) {
            
            var moneyToInvest = parseFloat($("#MoneyToInvestTextBox").val());
            var packPrice = parseFloat($("#CustomPackPriceLabel").text());

            var NumberOfAdPacksForInvestMoney = parseInt(moneyToInvest / packPrice);
            var ValueOfAdPacks = NumberOfAdPacksForInvestMoney * packPrice;


            //Set description
            var textBox = document.getElementById('AdPacksToBuyTextBox');
            if (textBox != null)
                textBox.value = NumberOfAdPacksForInvestMoney;

            $("#OnePackCost").text(packPrice + ' AYA');
            $("#NumberOfPacksToBuy").text(NumberOfAdPacksForInvestMoney);
            $("#MoneyValueOfPacksToBuy").text(ValueOfAdPacks.toFixed(4) + ' AYA');
        }

    }

    function TokenTBChanged() {
        var animationDiv = document.getElementById("InProgressAnimation");
        if (animationDiv != null) {
            var tokenToInvest = parseFloat($("#AyaToInvestTextBox").val());
            var packPrice = parseFloat($("#CustomPackPriceLabel").text());
            var ERC20Price = <%=(Titan.Cryptocurrencies.CryptocurrencyFactory.Get(Titan.Cryptocurrencies.CryptocurrencyType.BTC).GetValue().ToDecimal()).ToString("F8") %>;
            var moneyToInvest = tokenToInvest * ERC20Price;

            //Calculate
            var MoneyToTokenValue = moneyToInvest / ERC20Price;
            var NumberOfAdPacksForInvestMoney = parseInt(moneyToInvest / packPrice);
            var ValueOfAdPacks = NumberOfAdPacksForInvestMoney * packPrice;
            var TokenCostOfAdPacks = ValueOfAdPacks / ERC20Price;

            //Set description
            var textBox = document.getElementById('MoneyToInvestTextBox');
            if (textBox != null)
                textBox.value = parseInt(moneyToInvest);

            $("#OnePackCost").text(packPrice + ' AYA');
            $("#NumberOfPacksToBuy").text(NumberOfAdPacksForInvestMoney);
            $("#MoneyValueOfPacksToBuy").text(ValueOfAdPacks.toFixed(4) + ' AYA');
            $("#TokenValueOfPacksToBuy").text(TokenCostOfAdPacks.toFixed(8) + ' <%=TokenCryptocurrency.Code%>');
        }
    }

    function AdPacksTBChanged() {
        var animationDiv = document.getElementById("InProgressAnimation");
        if (animationDiv != null) {
            animationDiv.style.visibility = "hidden";
            var packPrice = parseFloat($("#CustomPackPriceLabel").text());
            var NumberOfAdPacksForInvestMoney = $("#AdPacksToBuyTextBox").val();
            var ValueOfAdPacks = NumberOfAdPacksForInvestMoney * packPrice;

            //Set description
            var textBox = document.getElementById('MoneyToInvestTextBox');
            if (textBox != null)
                textBox.value = parseInt(ValueOfAdPacks);

            $("#OnePackCost").text(packPrice + ' AYA');
            $("#NumberOfPacksToBuy").text(NumberOfAdPacksForInvestMoney);
            $("#MoneyValueOfPacksToBuy").text(ValueOfAdPacks.toFixed(4) + ' AYA');
        }
    }

</script>

<style>
    .trans {
        display: none !important;
    }

    .LendingSummary tr td {
        padding-top: 5px;
        padding-left: 17px;
        font-size: 12px;
    }

    #TOSCheckBox label, #ReferralCheckBox label {
        margin-left: 10px;
    }

    .LendingPacksPopUp {
        z-index: 9999999;
        box-shadow: 0 0 0 9999px rgba(0,0,0,.7) !important;
        border: solid 3px #ec3b57;
        position: absolute;
    }

    .CustomAdPackType {
        width: 100%;
        padding-left: 10px;
        margin: 10px 0px;
        color: white;
    }

    .CustomTitleLabel {
        font-weight: bold;
    }

    .mrgb {
        margin-bottom: 15px;
    }
</style>

<asp:UpdatePanel ID="CustomAdPackPurchaseUpdatePanel" runat="server" UpdateMode="Conditional">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="CustomTypesDropDown" EventName="SelectedIndexChanged" />
        <asp:AsyncPostBackTrigger ControlID="TOSAgreement" EventName="CheckedChanged" />
        <asp:AsyncPostBackTrigger ControlID="CustomBuyForReferralCheckBox" EventName="CheckedChanged" />
    </Triggers>
    <ContentTemplate>
        <asp:PlaceHolder runat="server" ID="MainPopUpContent" Visible="false">
            <div class="row test">
                <div class="col-md-6 col-md-offset-3 col-lg-4 col-lg-offset-4 LendingPacksPopUp" style="background-color: white">
                    <asp:PlaceHolder runat="server" ID="CustomMessagesPlaceHolder" Visible="false">
                        <br />
                        <asp:Panel ID="CustomErrorPanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                            <asp:Literal ID="CustomErrorLiteral" runat="server"></asp:Literal>
                        </asp:Panel>
                        <asp:Panel ID="CustomSuccessPanel" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
                            <asp:Literal ID="CustomSuccessLiteral" runat="server"></asp:Literal>
                        </asp:Panel>
                    </asp:PlaceHolder>

                    <div class="col-lg-12" style="text-align: center; padding: 15px;">
                        <h3 style="font-weight: bold; font-style: italic;">
                            <asp:Label runat="server" ID="CustomAdPacksBuyTitle"></asp:Label>
                        </h3>
                    </div>
                    <br />
                    <br />
                    <div class="form-group">
                        <div class="col-md-5">
                            <div class="input-group">
                                <span class="add-on input-group-addon"><%=AppSettings.Site.CurrencySign %></span>
                                <asp:TextBox runat="server" ClientIDMode="Static" ID="MoneyToInvestTextBox" MaxLength="10" onchange="MoneyTBChanged(); return false;" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="col-md-2" style="text-align: center; margin-top: 8px; font-size: 20px;">=</div>
                        <div class="col-md-5" style="display: none">
                            <div class="input-group">
                                <span class="add-on input-group-addon"><%=TokenCryptocurrency.Code %></span>
                                <asp:TextBox runat="server" ClientIDMode="Static" ID="AyaToInvestTextBox" MaxLength="10" onchange="TokenTBChanged(); return false;" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="col-md-5">
                            <div class="input-group">
                                <span class="add-on input-group-addon">#</span>
                                <asp:TextBox runat="server" ClientIDMode="Static" ID="AdPacksToBuyTextBox" MaxLength="10" onchange="AdPacksTBChanged(); return false;" CssClass="form-control" Text="1" />
                            </div>
                        </div>
                        <br />
                    </div>
                    <br />
                    <hr />
                    <asp:PlaceHolder runat="server" ID="CustomTypesDDLPlaceHolder">
                        <div class="col-md-5" style="margin-top: 15px">
                            <asp:Label runat="server" ID="CustomTypesLabel" class="CustomTitleLabel" /><br />
                            <asp:DropDownList ID="CustomTypesDropDown" runat="server" OnSelectedIndexChanged="CustomTypesDropDown_SelectedIndexChanged" class="CustomAdPackType" AutoPostBack="true" />
                        </div>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder runat="server" ID="SpaceDivPlaceHolder">
                        <div class="col-md-5" runat="server" id="SpaceDiv" />
                    </asp:PlaceHolder>

                    <asp:PlaceHolder runat="server" ID="CustomCampaignDDLPlaceHolder">
                        <div class="col-md-5 col-md-offset-2" style="margin-top: 15px">
                            <asp:Label runat="server" ID="CustomCampaignLabel" class="CustomTitleLabel" /><br />
                            <asp:DropDownList ID="CustomCampaignsDropDown" runat="server" class="CustomAdPackType" />
                        </div>
                    </asp:PlaceHolder>

                    <div class="col-md-12" style="margin-left: -15px; margin-bottom: 15px;">
                        <table class="LendingSummary">
                            <tr>
                                <td>1 <%=AppSettings.RevShare.AdPack.AdPackName %>&nbsp;<%=U6012.COST %>:</td>
                                <td style="font-weight: bold"><span id="OnePackCost"></td>
                            </tr>
                            <tr>
                                <td>Number of packages to buy: </td>
                                <td style="font-weight: bold"><span id="NumberOfPacksToBuy"></td>
                            </tr>
                            <tr style="display: none">
                                <td>Money value for packs: </td>
                                <td style="font-weight: bold"><span id="MoneyValueOfPacksToBuy"></td>
                            </tr>
                            <tr style="display: none">
                                <td>Bitcoin value for packs: </td>
                                <td style="font-weight: bold"><span id="TokenValueOfPacksToBuy"></td>
                            </tr>
                        </table>
                        <asp:Label runat="server" ID="testLabel"></asp:Label>
                    </div>
                    <br />
                    <br />

                    <asp:PlaceHolder runat="server" ID="CustomPurchaseForReferralPlaceHolder">
                        <div class="col-md-5" id="ReferralCheckBox">
                            <asp:CheckBox runat="server" ID="CustomBuyForReferralCheckBox" OnCheckedChanged="CustomBuyForReferralCheckBox_CheckedChanged" AutoPostBack="true" />
                            <asp:PlaceHolder runat="server" ID="CustomReferralsPlaceHolder" Visible="false">
                                <asp:DropDownList ID="CustomReferralsDropDownList" runat="server" class="CustomAdPackType" />
                            </asp:PlaceHolder>
                        </div>
                    </asp:PlaceHolder>

                    <br />
                    <br />
                    <div class="col-md-12" id="TOSCheckBox" style="margin-bottom: 15px">
                        <asp:CheckBox runat="server" ID="TOSAgreement" AutoPostBack="true"
                            onchange="MoneyTBChanged(); return false;" OnCheckedChanged="TOSAgreement_CheckedChanged" />
                        <a href="sites/tos.aspx" style="font-weight: bold">&nbsp;<%=L1.TERMSOFSERVICE %></a>
                    </div>
                    <br />
                    <br />
                    <div class="form-group">
                        <asp:PlaceHolder runat="server" ID="CustomPurchaseViaTokenPlaceHolder">
                            <div class="col-md-12">
                                <asp:Button ID="CustomPurchaseViaERC20TokensButton" runat="server"
                                    CssClass="btn btn-inverse btn-block mrgb"
                                    OnClick="PurchaseButtonViaERC20Tokens_Click"
                                    OnClientClick="ShowInProgressAnimation(); this.disabled = true;" UseSubmitBehavior="false" />
                            </div>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder runat="server" ID="CustomPurchaseViaPurchaseBalancePlaceHolder">
                            <div class="col-md-12">
                                <asp:Button ID="CustomPurchaseViaPurchaseBalanceButton" runat="server"
                                    CssClass="btn btn-inverse btn-block mrgb"
                                    OnClick="PurchaseButtonViaPurchaseBalance_Click"
                                    OnClientClick="ShowInProgressAnimation(); this.disabled = true;" UseSubmitBehavior="false" />
                            </div>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder runat="server" ID="CustomPurchaseViaCashBalancePlaceHolder">
                            <div class="col-md-12">
                                <asp:Button ID="CustomPurchaseViaCashBalanceButton" runat="server"
                                    CssClass="btn btn-inverse btn-block mrgb"
                                    OnClick="PurchaseButtonViaCashBalance_Click"
                                    OnClientClick="ShowInProgressAnimation(); this.disabled = true;" UseSubmitBehavior="false" />
                            </div>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder runat="server" ID="CustomPurchaseViaMainBalancePlaceHolder" Visible="false">
                            <div class="col-md-12">
                                <asp:Button ID="CustomPurchaseViaMainBalanceButton" runat="server"
                                    CssClass="btn btn-inverse btn-block mrgb" Text="PAY VIA MAIN BALANCE"
                                    OnClick="CustomPurchaseViaMainBalanceButton_Click"
                                    OnClientClick="ShowInProgressAnimation(); this.disabled = true;" UseSubmitBehavior="false" />
                            </div>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder runat="server" ID="CustomPurchaseViaCommissionBalancePlaceHolder" Visible="false">
                            <div class="col-md-12">
                                <asp:Button ID="CustomPurchaseViaCommissionBalanceButton" runat="server"
                                    CssClass="btn btn-inverse btn-block mrgb"
                                    OnClick="PurchaseButtonViaCommissionBalance_Click"
                                    OnClientClick="ShowInProgressAnimation(); this.disabled = true;" UseSubmitBehavior="false" />
                            </div>
                        </asp:PlaceHolder>

                        <div class="col-md-4">
                            <asp:Button ID="CancelLendingButton" runat="server" Text="Cancel"
                                CssClass="btn btn-inverse btn-block mrgb"
                                OnClick="CancelLendingButton_Click"
                                UseSubmitBehavior="false" />
                        </div>

                        <%--IN PROGRESS ANIMATION--%>
                        <div class="col-md-2 col-md-offset-6" clientidmode="Static" id="InProgressAnimation" runat="server">
                            <i class="fa fa-spinner fa-pulse fa-3x fa-fw"></i>
                        </div>
                        <br />
                        <br />
                    </div>
                </div>
            </div>
            <asp:Label runat="server" ID="CustomPackPriceLabel" ClientIDMode="Static" CssClass="displaynone" />
        </asp:PlaceHolder>
    </ContentTemplate>
</asp:UpdatePanel>



