<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="adpacks.aspx.cs" Inherits="Page_advert_Adpacks" %>

<%@ MasterType VirtualPath="~/User.master" %>

<asp:Content ContentPlaceHolderID="PageHeadContent" runat="server">
    <link rel="stylesheet" href="Styles/WatchAds.css" type="text/css" />
    <script type="text/javascript" src="Scripts/gridview.js"></script>

    <script type="text/javascript">

        //$.noConflict();

<%--        function askForConfirmation(parameter) {
            $('#confirmationModal').modal({ 'backdrop': true, 'show': true });

            var $modal = $('#confirmationModal');
            $modal.find('.modal-header').html('<h3 class="text-center m-t-0"><%=L1.SUMMARY%></h3>');

            var packPrice = parseFloat($("#<%=PackPriceLabel.ClientID%>").text());
            var numberOfPacks = parseFloat($("#<%=NumberOfPacksTextBox.ClientID%>").val());
            var result = packPrice * numberOfPacks;
            var token = parseFloat("<%=AppSettings.Ethereum.ERC20TokenRate.ToClearString() %>");
            var finalPrice = result / token;
            var addon = "";
            if (numberOfPacks > 1)
                addon = "s";

            $modal.find('.modal-body').html('<h5 class="text-success" style="white-space:pre-wrap"><span style="color: black; display: grid">' 
                + '<span style="text-align: center"><img src="<%=AppSettings.Ethereum.ERC20TokenImageUrl%>"/></span><br /><br />'
                + 'You are going to buy '+numberOfPacks+' <%=AppSettings.RevShare.AdPack.AdPackName%>'+addon+'. Please verify your exchange details below:<br /><br />'
                + numberOfPacks + ' <%=AppSettings.RevShare.AdPack.AdPackName%> x <%=AppSettings.Site.CurrencySign%>' + packPrice + ' = <%=AppSettings.Site.CurrencySign%>' + result + '<br />'
                + '1 <%=AppSettings.Ethereum.ERC20TokenTLA%> = <%=AppSettings.Ethereum.ERC20TokenRate%><br />'
                + '<%=AppSettings.Site.CurrencySign%>'+result+' / <%=AppSettings.Site.CurrencySign%>'+token+' = '+finalPrice+' <%=AppSettings.Ethereum.ERC20TokenTLA%><br /><br /></span>'
                + '<span style="font-size: 18px">' + '<%=L1.PRICE%>: '+finalPrice+' <%=AppSettings.Ethereum.ERC20TokenTLA%></span></h5>');

            
            let promise = new Promise(function (resolve, reject) {
                let confirmButton = document.getElementById('confirmButton');
                confirmButton.addEventListener("click", function () {
                    resolve();
                });
            });
            promise.then(function () {
                $('#confirmationModal').modal('hide');
                __doPostBack(parameter.name, '');
            });
        }--%>
        
        function TBChanged() {
            //Get values
            var packPrice = parseFloat($("#<%=PackPriceLabel.ClientID%>").text());
            var numberOfPacks = parseFloat($("#<%=NumberOfPacksTextBox.ClientID%>").val());
            var packROI = parseFloat($("#<%=PackROILabel.ClientID%>").text());

            var packClicks = parseFloat($("#<%=PackClicksLabel.ClientID%>").text());
            var packDisplayTime = parseFloat($("#<%=PackDisplayTimeLabel.ClientID%>").text());
            var packNormalBannerImpressions = parseFloat($("#<%=PackNormalBannerImpressionsLabel.ClientID%>").text());
            var packConstantBannerImpressions = parseFloat($("#<%=PackConstantBannerImpressionsLabel.ClientID%>").text());
            var packLoginAdsCredits = parseFloat($("#<%=packLoginAdsCreditsLabel.ClientID%>").text());
            var withdrawLimit = parseFloat($("#<%=packWithdrawLimitLabel.ClientID%>").text());
            var trafficExchange = parseFloat($("#<%=TrafficExchangeSurfCreditsLabel.ClientID%>").text());

            //Calculate
            var result = packPrice * numberOfPacks;

            //Display
            $("#DisplaySpanPacks").text(numberOfPacks);
            $("#DisplaySpanDollars").text(result.toFixed(4));
            $("#DisplaySpanROI").text(packROI);
            $("#DisplaySpanClicks").text(packClicks);
            $("#DisplaySpanDisplayTime").text(packDisplayTime);
            $("#DisplaySpanNormalBannerImpressions").text(packNormalBannerImpressions);
            $("#DisplaySpanConstantBannerImpressions").text(packConstantBannerImpressions);
            $("#DisplaySpanLoginAdsCredits").text(packLoginAdsCredits);
            $("#DisplaySpanWithdrawLimit").text(withdrawLimit);
            $("#DisplaySpanTrafficExchangeSurfCredits").text(trafficExchange);
        }

        function MoneyTBChanged() {
            var animationDiv = document.getElementById("ctl00_PageMainContent_InProgressAnimation");
            if(animationDiv != null){
                animationDiv.style.visibility = "hidden";

                var moneyToInvest = parseFloat($("#<%=MoneyToInvestTextBox.ClientID%>").val());
                var packPrice = parseFloat($("#<%=CustomPackPriceLabel.ClientID%>").text());
                var ERC20Price = <%=AppSettings.Ethereum.ERC20TokenRate.ToClearString() %>;

                //Calculate
                var MoneyToTokenValue = moneyToInvest / ERC20Price;
                var NumberOfAdPacksForInvestMoney = parseInt(moneyToInvest / packPrice);
                var ValueOfAdPacks = NumberOfAdPacksForInvestMoney * packPrice;
                var TokenCostOfAdPacks = ValueOfAdPacks / ERC20Price;
            
                //Set description
                var textBox = document.getElementById('<%=AyaToInvestTextBox.ClientID %>');
                if(textBox != null)
                    textBox.value = MoneyToTokenValue.toFixed(4);

                $("#OnePackCost").text(packPrice + ' $');
                $("#NumberOfPacksToBuy").text(NumberOfAdPacksForInvestMoney);
                $("#MoneyValueOfPacksToBuy").text(ValueOfAdPacks.toFixed(4) + ' $');
                $("#TokenValueOfPacksToBuy").text(TokenCostOfAdPacks.toFixed(8) + ' <%=TokenCryptocurrency.Code%>');
            }

        }

        function TokenTBChanged(){
            var animationDiv = document.getElementById("ctl00_PageMainContent_InProgressAnimation");
            if(animationDiv != null){
                var tokenToInvest = parseFloat($("#<%=AyaToInvestTextBox.ClientID%>").val());
                var packPrice = parseFloat($("#<%=CustomPackPriceLabel.ClientID%>").text());
                var ERC20Price = <%=AppSettings.Ethereum.ERC20TokenRate.ToClearString() %>;
                var moneyToInvest = tokenToInvest * ERC20Price;

                //Calculate
                var MoneyToTokenValue = moneyToInvest / ERC20Price;
                var NumberOfAdPacksForInvestMoney = parseInt(moneyToInvest / packPrice);
                var ValueOfAdPacks = NumberOfAdPacksForInvestMoney * packPrice;
                var TokenCostOfAdPacks = ValueOfAdPacks / ERC20Price;
            
                //Set description
                var textBox = document.getElementById('<%=MoneyToInvestTextBox.ClientID %>');
                if(textBox != null)
                    textBox.value = moneyToInvest.toFixed(4);
                $("#OnePackCost").text(packPrice + ' $');
                $("#NumberOfPacksToBuy").text(NumberOfAdPacksForInvestMoney);
                $("#MoneyValueOfPacksToBuy").text(ValueOfAdPacks.toFixed(4) + ' $');
                $("#TokenValueOfPacksToBuy").text(TokenCostOfAdPacks.toFixed(8) + ' <%=TokenCryptocurrency.Code%>');
            }
        }

        function ShowInProgressAnimation(){
            var animationDiv = document.getElementById("ctl00_PageMainContent_InProgressAnimation");
            if(animationDiv != null)
                animationDiv.style.visibility = "visible";
        }

        $(function() {
            $('#<%=MoneyToInvestTextBox.ClientID %>').keyup(function () {
                MoneyTBChanged();
            });

            $('#<%=AyaToInvestTextBox.ClientID %>').keyup(function () {
                TokenTBChanged();
            });
        });

        function CheckURL() {
            $('#__EVENTARGUMENT5').val($('#<%=URL.ClientID %>').val()); //Set URL to validate

            //Send request
            $('#<%=Form.ClientID%>').attr('action', 'user/earn/surf.aspx?f=3');
            $('#<%=Form.ClientID%>').attr('target', '_blank');
            $('#<%=Form.ClientID%>').submit();
            $('#<%=Form.ClientID%>').attr('action', 'user/advert/adpacks.aspx<%=editId%>');
            $('#<%=Form.ClientID%>').attr('target', '_self');
            return false;
        }

        function showURLBox(id) {
            $('#' + id).show();
            return false;
        }

        function hideURLBox(id) {
            $('#' + id).hide();
            $('#' + id).val('');
        }

        $(document).ready(function () {
            TBChanged();
        });

        function pageLoad() {
            //jQuery.noConflict();
            MoneyTBChanged(); 

            $('#<%=MoneyToInvestTextBox.ClientID %>').keyup(function () {
                MoneyTBChanged();
            });

            $('#<%=AyaToInvestTextBox.ClientID %>').keyup(function () {
                TokenTBChanged();
            });

            $('.selectableCheckbox input').on('click', function () {
                $('#<%=NewSelectedPanel.ClientID%>').show();
                hideIfUnchecked('<%=NewSelectedPanel.ClientID%>');
                hideIfUnchecked('<%=TimeClicksExchangePanel.ClientID%>');
                $(this).prop('checked', $('#checkAll').checked);
            });

            $('.allSelectableCheckbox').on('click', function () {
                $('.selectableCheckbox input').prop('checked', this.checked);
                hideIfUnchecked('<%=NewSelectedPanel.ClientID%>');
                hideIfUnchecked('<%=TimeClicksExchangePanel.ClientID%>');
                uncheckInvisible();
            });

           <%=PageScriptGenerator.GetGridViewCode(AdPacksStatsGridView) %>
           <%=PageScriptGenerator.GetGridViewCode(AdPacksAdGridView) %>
           <%=PageScriptGenerator.GetGridViewCode(MyGroupsGridView) %>

            $('#AdPackTypeMembershipTable').DataTable({
                responsive: true,
                paginate: false,
                info: false,
                searching: false,
                ordering: false,
                retrieve: true
            });

            $('.ToS').slimScroll({
                height: '500px',
                railVisible: true,
                alwaysVisible: true
            }).bind('slimscroll', function (e, pos) {
                if (pos == 'bottom') {
                    $("#<%=AgreeToSButton.ClientID %>").removeClass("disabled btn-default").addClass("btn-primary");
                    }
                });

            $("#<%=PurchaseViaCashBalanceButton.ClientID %>").click(function (e) { e.preventDefault(); });
            $('#confirmationModal').modal('hide');

            var mainColor = $(".sidebar .nav>li.active>a").first().css('background-color');
            $(".LendingPacksPopUp").css("border-color", mainColor);
            $(".CustomAdPackType").css("background-color", mainColor);
        }
    </script>
    <style>
        .trans {
            display: none !important;
        }

        .LendingSummary tr td{
            padding-top: 5px;
            padding-left: 17px;
            font-size: 12px;
        }

        #TOSCheckBox label, #ReferralCheckBox label{
            margin-left: 10px;
        }

        .LendingPacksPopUp{
            z-index: 9999999;
            box-shadow: 0 0 0 9999px rgba(0,0,0,.5) !important;
            border: solid 3px #ec3b57;
        }

        .CustomAdPackType{
            width: 100%;
            padding-left: 10px;
            margin: 10px 0px;
            color: white;
        }

        .CustomTitleLabel{
            font-weight: bold;
        }

        .mrgb{
            margin-bottom: 15px;
        }

    </style>

</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <h1 class="page-header"><%=AppSettings.RevShare.AdPack.AdPackNamePlural %></h1>
    <div class="row">
        <div class="col-md-12">
            <p id="MainDescriptionP" runat="server" class="lead" />
        </div>
    </div>
    <input type="hidden" name="__EVENTARGUMENT5" id="__EVENTARGUMENT5" value="" />

    <asp:UpdatePanel ID="MultiViewUpdatePanel" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="Button4" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="Button3" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="Button2" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="Button1" EventName="Click" />
            <asp:PostBackTrigger ControlID="NormalBannerUploadButton" />
            <asp:PostBackTrigger ControlID="createBannerAdvertisement_BannerUploadSubmit" />
            <asp:AsyncPostBackTrigger ControlID="CreateAdButton" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="RedirectToNewAdsButton" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="PurchaseButton" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="PurchaseViaCashBalanceButton" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="CancelLendingButton" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="CustomPurchaseViaERC20TokensButton" EventName="Click" />   
            <asp:AsyncPostBackTrigger ControlID="CustomTypesDropDown" EventName="SelectedIndexChanged" />    
            <asp:AsyncPostBackTrigger ControlID="CustomBuyForReferralCheckBox" EventName="CheckedChanged" /> 
                 
        </Triggers>

        <ContentTemplate>
            <div class="row">
                <div class="col-md-12">

                    <asp:Panel ID="SPanel" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
                        <asp:Literal ID="SText" runat="server"></asp:Literal>
                    </asp:Panel>

                    <asp:Panel ID="EPanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                        <asp:Literal ID="EText" runat="server"></asp:Literal>
                    </asp:Panel>

                    <asp:Panel ID="WPanel" runat="server" Visible="false" CssClass="alert alert-warning fade in m-b-15">
                        <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                    </asp:Panel>

                    <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="alert alert-danger fade in m-b-15"
                        ValidationGroup="RegisterUserValidationGroup" DisplayMode="List" ForeColor="#6b6b6b" />

                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger fade in m-b-15"
                        ValidationGroup="createBannerAdvertisement_OnSubmitValidationGroup" DisplayMode="List" ForeColor="White" />

                    <asp:ValidationSummary ID="ValidationSummary2" runat="server" CssClass="alert alert-danger fade in m-b-15"
                        ValidationGroup="NormalBannerUpload_ValidationGroup" DisplayMode="List" ForeColor="White" />

                    <asp:ValidationSummary ID="ValidationSummary3" runat="server" CssClass="alert alert-danger fade in m-b-15"
                        ValidationGroup="LinkCheckerGroup" DisplayMode="List" ForeColor="White" />

                    <asp:Label runat="server" ID="PackROILabel" ClientIDMode="Static" CssClass="displaynone" />
                    <asp:Label runat="server" ID="PackPriceLabel" ClientIDMode="Static" CssClass="displaynone" />

                    <asp:Label runat="server" ID="PackClicksLabel" ClientIDMode="Static" CssClass="displaynone" />
                    <asp:Label runat="server" ID="PackDisplayTimeLabel" ClientIDMode="Static" CssClass="displaynone" />
                    <asp:Label runat="server" ID="PackNormalBannerImpressionsLabel" ClientIDMode="Static" CssClass="displaynone" />
                    <asp:Label runat="server" ID="PackConstantBannerImpressionsLabel" ClientIDMode="Static" CssClass="displaynone" />
                    <asp:Label runat="server" ID="packLoginAdsCreditsLabel" ClientIDMode="Static" CssClass="displaynone" />
                    <asp:Label runat="server" ID="packWithdrawLimitLabel" ClientIDMode="Static" CssClass="displaynone" />
                    <asp:Label runat="server" ID="TrafficExchangeSurfCreditsLabel" ClientIDMode="Static" CssClass="displaynone" />

                    <div class="alert alert-info" id="AdpackCampaignsInfoDiv" runat="server">
                        <p><%= string.Format(U6000.CAMPAIGNSWITHOUTADPACKS,AppSettings.RevShare.AdPack.AdPackNamePlural)%></p>
                    </div>

                </div>
            </div>

            <%--BASiC VIEW--%>
            <asp:PlaceHolder runat="server" ID="BaseAdPacksView" Visible="true">
                <div class="row">
                    <div class="col-md-12">
                        <div class="nav nav-tabs custom text-right">
                            <div class="TitanViewPage">
                                <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                                    <asp:Button ID="Button4" runat="server" OnClick="MenuButton_Click" CommandArgument="3" />
                                    <asp:Button ID="Button3" runat="server" OnClick="MenuButton_Click" CommandArgument="2" />
                                    <asp:Button ID="Button2" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                                    <asp:Button ID="Button1" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                                </asp:PlaceHolder>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="tab-content">
                    <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
                        <asp:View runat="server" ID="View1">
                            <div class="TitanViewElement">
                                <%-- SUBPAGE START --%>
                                <div class="row">
                                    <div class="col-md-12">
                                        <p id="TitleDescriptionP" runat="server"><%=U4200.BUYADPACKDESCRIPTION.Replace("%n%", AppSettings.RevShare.AdPack.AdPackNamePlural) %></p>
                                        <asp:PlaceHolder runat="server" ID="ChangeAdvertInfoPlaceholder">
                                            <p><%=U5001.YOUCANCHANGECAMPAIGN.Replace("%n%", AppSettings.RevShare.AdPack.AdPackNamePlural) %></p>
                                        </asp:PlaceHolder>

                                        <asp:Literal ID="NoAdPackTypesForMemberLiteral" runat="server"></asp:Literal>
                                        <asp:PlaceHolder ID="AdPackPurchasePlaceHolder" runat="server">
                                            <div class="row p-t-20 p-b-20">
                                                <div class="col-md-4 text-center">
                                                    <span class="fa fa-cubes fa-5x"></span>
                                                    <p><b>1</b> <%=AppSettings.RevShare.AdPack.AdPackName %> =</p>
                                                </div>
                                                <div class="col-md-8">
                                                    <ul class="list-unstyled">
                                                        <li id="DescriptionAdPackListElement0" runat="server"><%=string.Format("<b>{0} </b>", "<span id='DisplaySpanClicks'>1</span>") %>
                                                            <%=U4200.ADPACKDESCRIPTION1 %>: <%=string.Format("<b>{0}</b>", "<span id='DisplaySpanDisplayTime'>1</span>") %>s</li>
                                                        <li id="DescriptionAdPackListElement1" runat="server"><%=string.Format("<b>{0} </b>", "<span id='DisplaySpanNormalBannerImpressions'>1</span>") %>
                                                            <%=U4200.NORMALBANNER.Replace("%n%", string.Format("({0}x{1})", AppSettings.RevShare.AdPack.PackNormalBannerWidth, AppSettings.RevShare.AdPack.PackNormalBannerHeight)) %></li>
                                                        <li id="DescriptionAdPackListElement2" runat="server"><%=string.Format("<b>{0} </b>", "<span id='DisplaySpanConstantBannerImpressions'>1</span>") %>
                                                            <%=U4200.CONSTANTBANNER.Replace("%n%", string.Format("({0}x{1})", AppSettings.RevShare.AdPack.PackConstantBannerWidth, AppSettings.RevShare.AdPack.PackConstantBannerHeight)) %></li>
                                                        <li><%=U4200.RETURNEDMONEY.Replace("%n%", string.Format("<b>{0}%</b>", "<span id='DisplaySpanROI'>1</span>")) %>
                                                        </li>
                                                        <asp:PlaceHolder runat="server" ID="WhiteBoxLoginAdsCreditsPlaceHolder">
                                                            <li><%=string.Format("<b>{0} </b>", "<span id='DisplaySpanLoginAdsCredits'>0</span>") %>
                                                                <%=U5008.LOGINADSCREDITS %></li>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder runat="server" ID="WhiteBoxWithdrawLimitPlaceHolder">
                                                            <li><%=U5008.WITHDRAWLIMITENLARGEDBY %>:
                                                     <b><span id='DisplaySpanWithdrawLimit'>0</span> <%=AppSettings.Site.CurrencySign %></b> </li>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder runat="server" ID="TrafficExchangeSurfCreditsPlaceHolder">
                                                            <li><%=U6000.TRAFFICEXCHANGESURFCREDITS %>:
                                                     <b><span id='DisplaySpanTrafficExchangeSurfCredits'>0</span> <%=AppSettings.Site.CurrencySign %></b> </li>
                                                        </asp:PlaceHolder>
                                                    </ul>
                                                </div>
                                            </div>

                                            <div class="row">
                                                <div class="col-md-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <label class="col-md-2 control-label"><%=L1.PRICE %>:</label>
                                                            <div class="col-md-6">
                                                                <p class="form-control no-border">
                                                                    <span id="DisplaySpanPacks" class="f-w-700">1</span>
                                                                    <%=AppSettings.RevShare.AdPack.AdPackNamePlural %> = 
                                                            <b><%=Prem.PTC.AppSettings.Site.CurrencySign %>
                                                                <span id="DisplaySpanDollars">?</span>
                                                            </b>
                                                                </p>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <label class="col-md-2 control-label"><%=U4200.NUMBEROFPACKS.Replace("%n%", AppSettings.RevShare.AdPack.AdPackNamePlural) %>:</label>
                                                            <div class="col-md-6">
                                                                <asp:TextBox runat="server" ClientIDMode="Static" ID="NumberOfPacksTextBox" MaxLength="4" onchange="TBChanged(); return false;" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div id="AddCampaignDiv" class="form-group" runat="server">
                                                            <label class="col-md-2 control-label"><%=L1.CAMPAIGN %>:</label>
                                                            <div class="col-md-6">
                                                                <div class="input-group">
                                                                    <asp:PlaceHolder runat="server" ID="DropDownAdsPlaceHolder">
                                                                        <asp:DropDownList ID="CampaignsDropDown" runat="server" AutoPostBack="true" CssClass="form-control"></asp:DropDownList>
                                                                    </asp:PlaceHolder>
                                                                    <div class="input-group-btn">
                                                                        <asp:Button runat="server" ID="RedirectToNewAdsButton" OnClick="MenuButton_Click" CommandArgument="2" Text="<%$ ResourceLookup:ADDNEW %>" CssClass="btn btn-primary" />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <asp:PlaceHolder runat="server" ID="AdPackTypePlaceHolder">
                                                            <div class="form-group">
                                                                <label class="col-md-2 control-label"><%=L1.TYPE %>:</label>
                                                                <div class="col-md-6">
                                                                    <asp:DropDownList ID="TypesDropDown" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="TypesDropDown_SelectedIndexChanged"></asp:DropDownList>
                                                                    <span class="helper-block">
                                                                        <asp:PlaceHolder ID="TypeAvailableForCustomGroups" runat="server">&nbsp; <%=U5006.AVAILABLEFORGROUPS %></asp:PlaceHolder>
                                                                    </span>
                                                                </div>
                                                            </div>
                                                        </asp:PlaceHolder>
                                                        <div class="form-group" runat="server" id="BuyForReferralPlaceHolder">
                                                            <label class="col-md-2 control-label"><%=U5008.BUYFORREFERRAL %>:</label>
                                                            <div class="col-md-6">
                                                                <div class="row">
                                                                    <div class="col-md-3 col-sm-3 col-xs-3">
                                                                        <div class="checkbox">
                                                                            <label>
                                                                                <asp:CheckBox runat="server" ID="BuyForReferralCheckBox" AutoPostBack="true" OnCheckedChanged="BuyForReferralCheckBox_CheckedChanged" />
                                                                            </label>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-md-9 col-sm-9 col-xs-9">
                                                                        <asp:DropDownList ID="ReferralsDropDown" runat="server" CssClass="form-control"></asp:DropDownList>
                                                                    </div>
                                                                </div>

                                                            </div>
                                                        </div>

                                                        <div id="AricustomisationPlaceHolderCheckbox" runat="server" class="form-group">
                                                            <div class="col-md-10 col-md-offset-2">
                                                                <div class="checkbox">
                                                                    <asp:CheckBox runat="server" CssClass="margin20-fix" ID="AgreeToSCheckBox" AutoPostBack="true" Text="I understand and agree Terms of Service" OnCheckedChanged="AgreeToSCheckBox_CheckedChanged" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <asp:PlaceHolder runat="server" ID="PurchaseViaPurchasePlaceHolder">
                                                                <div class="col-md-6 col-lg-4" style="margin-bottom:10px;">
                                                                    <asp:Button ID="PurchaseButton" runat="server"
                                                                        ValidationGroup="RegisterUserValidationGroup" CssClass="btn btn-inverse btn-block"
                                                                        OnClick="PurchaseButtonViaPurchaseBalance_Click"
                                                                        OnClientClick="this.disabled = true"
                                                                        UseSubmitBehavior="false" />
                                                                </div>
                                                            </asp:PlaceHolder>
                                                            <asp:PlaceHolder runat="server" ID="PurchaseViaCashPlaceHolder">
                                                                <div class="col-md-6 col-lg-4">
                                                                    <asp:Button ID="PurchaseViaCashBalanceButton" runat="server"
                                                                        ValidationGroup="RegisterUserValidationGroup" CssClass="btn btn-inverse btn-block"
                                                                        OnClick="PurchaseButtonViaCashBalance_Click"
                                                                        OnClientClick="this.disabled = true"
                                                                        UseSubmitBehavior="false" />
                                                                </div>
                                                            </asp:PlaceHolder>
                                                            <asp:PlaceHolder runat="server" ID="PurchaseViaCommissionPlaceHolder">
                                                                <div class="col-md-6 col-lg-4">
                                                                    <asp:Button ID="PurchaseViaCommissionBalanceButton" runat="server"
                                                                        ValidationGroup="RegisterUserValidationGroup" CssClass="btn btn-inverse btn-block"
                                                                        OnClick="PurchaseButtonViaCommissionBalance_Click"
                                                                        OnClientClick="this.disabled = true"
                                                                        UseSubmitBehavior="false" />
                                                                </div>
                                                            </asp:PlaceHolder>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:PlaceHolder>
                                        <div class="form-group" id="AricustomisationPlaceHolder" runat="server">

                                            <div id="AgreeToSPlaceHolder" runat="server" visible="false">
                                                <div class="row ToS p-20 bg-silver-lighter">
                                                    <div class="col-md-12">
                                                        <asp:Literal ID="AgreeToSLiteral" runat="server"></asp:Literal>
                                                    </div>
                                                </div>

                                                <div class="col-md-4 col-md-offset-4 m-t-15 col-sm-6 col-sm-offset-3">
                                                    <asp:LinkButton ID="AgreeToSButton" runat="server" OnCommand="AgreeToSButton_Command" CssClass="btn btn-default disabled btn-lg btn-block m-15">I AGREE WITH THE TERM OF SERVICE<br />
                                                CONTINUE TO BUY
                                                    </asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <%-- SUBPAGE END   --%>
                            </div>
                        </asp:View>

                        <asp:View runat="server" ID="TypesView" OnActivate="TypesView_Activate">
                            <div class="TitanViewElement">
                                <div class="row">
                                    <div class="col-md-12">
                                        <h2 runat="server" id="TypesTableTitle"></h2>
                                        <%--<%=string.Format(U6002.ADPACKTYPES, AppSettings.RevShare.AdPack.AdPackName)%>--%>
                                        <div class="table-responsive">
                                            <asp:GridView ID="AdPackTypesGridView" runat="server" OnPreRender="BaseGridView_PreRender" AutoGenerateColumns="true"
                                                DataSourceID="AdPackTypesGridViewDataSource" OnRowDataBound="AdPackTypesGridView_RowDataBound" >
                                            </asp:GridView>
                                            <asp:ObjectDataSource ID="AdPackTypesGridViewDataSource" runat="server" SelectMethod="GetAdPackActiveTypesDataSet" TypeName="AdPackManager"></asp:ObjectDataSource>
                                        </div>
                                    </div>
                                    <div class="col-md-12">
                                        <h2><%=string.Format(U6000.ADPACKROIANDREPURCHASE, AppSettings.RevShare.AdPack.AdPackName) %></h2>
                                        <asp:Literal runat="server" ID="TypesMembershipProperties"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                        </asp:View>

                        <asp:View runat="server" ID="View3">
                            <div class="TitanViewElement">
                                <%-- SUBPAGE START --%>
                                <div class="row">
                                    <div class="col-md-12">
                                        <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                                            <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                                        </asp:Panel>
                                        <asp:PlaceHolder ID="NewAdvertPlaceHolder" runat="server">
                                            <h2><%=U4200.CREATENEWADS %></h2>

                                            <p>
                                                <%=U4200.NEWADPACKSADDESCRIPTION.Replace("%n%", AppSettings.RevShare.AdPack.AdPackNamePlural) %>
                                                <asp:Literal runat="server" ID="AdminLiteral" Visible="false"></asp:Literal>
                                                <asp:Literal runat="server" ID="StartPageDescriptionLiteral"></asp:Literal>
                                            </p>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="EditEdvertPlaceHolder" runat="server"></asp:PlaceHolder>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <asp:Label ID="TitleLabel" runat="server" AssociatedControlID="Title" CssClass="control-label col-md-2"><%=L1.TITLE %>:</asp:Label>

                                                <div class="col-md-6">
                                                    <%--Max DB Length = 256--%>
                                                    <asp:TextBox ID="Title" runat="server" CssClass="form-control tooltip-on" MaxLength="30"></asp:TextBox>

                                                    <asp:RegularExpressionValidator ValidationGroup="RegisterUserValidationGroup" ID="RegularExpressionValidator1" runat="server"
                                                        ValidationExpression="[^'\n\r\t]{3,30}" Display="Dynamic" CssClass="text-danger" ControlToValidate="Title" Text="">*</asp:RegularExpressionValidator>
                                                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="Title"
                                                        Display="Dynamic" CssClass="text-danger" Text=""
                                                        ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Description" CssClass="control-label col-md-2"><%=L1.DESCRIPTION %>:</asp:Label>
                                                <div class="col-md-6">
                                                    <%--Max DB Length = 256--%>
                                                    <asp:TextBox ID="Description" runat="server" CssClass="form-control tooltip-on" TextMode="MultiLine" Rows="5" MaxLength="70"></asp:TextBox>

                                                    <asp:RegularExpressionValidator runat="server"
                                                        ValidationGroup="RegisterUserValidationGroup" Display="Dynamic" CssClass="text-danger"
                                                        Text="" ID="CorrectEmailRequired" ControlToValidate="Description" ValidationExpression="[^']{3,70}">*</asp:RegularExpressionValidator>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="URL" CssClass="control-label col-md-2">URL:</asp:Label>
                                                <div class="col-md-6">

                                                    <asp:UpdatePanel runat="server" ID="AddNewAdWithURLCheck" OnLoad="AddNewAdWithURLCheck_Load" ClientIDMode="Static" class="input-group">
                                                        <ContentTemplate>
                                                            <asp:TextBox ID="URL" runat="server" CssClass="form-control" Text="http://" MaxLength="800"></asp:TextBox>

                                                            <div class="input-group-btn">
                                                                <asp:LinkButton ID="CheckURLButton" runat="server" OnClientClick="CheckURL()" CssClass="btn btn-primary"><%=U4200.CHECKURLTEXT %></asp:LinkButton>
                                                            </div>

                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>


                                                    <asp:RegularExpressionValidator Display="Dynamic" CssClass="text-danger" ValidationGroup="RegisterUserValidationGroup" ID="RegularExpressionValidator2" runat="server" ErrorMessage="*"
                                                        ControlToValidate="URL" Text="">*</asp:RegularExpressionValidator>
                                                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="URL"
                                                        Display="Dynamic" CssClass="text-danger"
                                                        ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                                </div>
                                            </div>

                                            <asp:PlaceHolder runat="server" ID="BannerPlaceHolder">
                                                <div class="form-group">
                                                    <label class="col-md-2 control-label"><%=U4200.BANNER + string.Format("({0}x{1})", AppSettings.RevShare.AdPack.PackNormalBannerWidth, AppSettings.RevShare.AdPack.PackNormalBannerHeight) %>:</label>

                                                    <div class="col-md-6">
                                                        <asp:Image ID="createBannerAdvertisement_BannerImage2" runat="server" BorderStyle="Solid" BorderWidth="1px" BorderColor="#e1e1e1" />
                                                        <br />
                                                        <span class="btn btn-success fileinput-button">
                                                            <i class="fa fa-plus"></i>
                                                            <span><%=U6000.ADDFILE %></span>
                                                            <asp:FileUpload ID="NormalBannerUpload" runat="server" onclick="hideURLBox('BannerFileUrlTextBox1');" />
                                                        </span>
                                                        <asp:Button ID="BannerUploadByUrlButton1" Text="<%$ResourceLookup: ADDBANNERBYURL %>" runat="server" CssClass="btn btn-success fileinput-button"
                                                            OnClientClick="showURLBox('BannerFileUrlTextBox1'); return false;" />
                                                    </div>
                                                    <div class="col-md-6 col-md-offset-2 m-t-15">
                                                        <div class="input-group">
                                                            <asp:TextBox ID="BannerFileUrlTextBox1" runat="server" CssClass="form-control" Style="display: none" ClientIDMode="Static"></asp:TextBox>

                                                            <div class="input-group-btn">
                                                                <asp:Button ID="NormalBannerUploadButton" Text="<%$ResourceLookup: SUBMIT %>" OnClick="NormalBannerUploadButton_Click"
                                                                    CausesValidation="true" runat="server" ValidationGroup="NormalBannerUpload_ValidationGroup" CssClass="btn btn-primary" />
                                                            </div>
                                                        </div>
                                                        <br />
                                                        <asp:CustomValidator ID="NormalBannerUploadValidator"
                                                            ControlToValidate="NormalBannerUpload" Display="Dynamic" CssClass="text-danger"
                                                            OnServerValidate="NormalBannerUploadValidator_ServerValidate"
                                                            ValidationGroup="NormalBannerUpload_ValidationGroup" ValidateEmptyText="true"
                                                            runat="server">*</asp:CustomValidator>
                                                        <asp:CustomValidator ID="createBannerAdvertisement_BannerUploadSelectedCustomValidator2"
                                                            ControlToValidate="NormalBannerUpload" Display="Dynamic" CssClass="text-danger"
                                                            OnServerValidate="NormalBannerUploadSelectedValidator_ServerValidate"
                                                            ValidationGroup="RegisterUserValidationGroup" ValidateEmptyText="true"
                                                            runat="server">*</asp:CustomValidator>
                                                    </div>
                                                </div>

                                                <div class="form-group">
                                                    <label class="col-md-2 control-label"><%=U4200.BANNER + string.Format("({0}x{1})", AppSettings.RevShare.AdPack.PackConstantBannerWidth, AppSettings.RevShare.AdPack.PackConstantBannerHeight) %>:</label>

                                                    <div class="col-md-6">

                                                        <asp:Image ID="createBannerAdvertisement_BannerImage" runat="server" BorderStyle="Solid" BorderWidth="1px" BorderColor="#e1e1e1" />
                                                        <br />
                                                        <span class="btn btn-success fileinput-button">
                                                            <i class="fa fa-plus"></i>
                                                            <span><%=U6000.ADDFILE %></span>
                                                            <asp:FileUpload ID="createBannerAdvertisement_BannerUpload" runat="server" onclick="hideURLBox('BannerFileUrlTextBox2');" />
                                                        </span>
                                                        <asp:Button ID="BannerUploadByUrlButton2" Text="<%$ResourceLookup: ADDBANNERBYURL %>" runat="server" CssClass="btn btn-success fileinput-button"
                                                            OnClientClick="showURLBox('BannerFileUrlTextBox2'); return false;" />
                                                    </div>
                                                    <div class="col-md-6 col-md-offset-2 m-t-15">
                                                        <div class="input-group">
                                                            <asp:TextBox ID="BannerFileUrlTextBox2" runat="server" CssClass="form-control" Style="display: none" ClientIDMode="Static"></asp:TextBox>

                                                            <div class="input-group-btn">
                                                                <asp:Button ID="createBannerAdvertisement_BannerUploadSubmit" Text="<%$ResourceLookup: SUBMIT %>" OnClick="createBannerAdvertisement_BannerUploadSubmit_Click"
                                                                    CausesValidation="true" runat="server" ValidationGroup="createBannerAdvertisement_OnSubmitValidationGroup" CssClass="btn btn-primary" />
                                                            </div>
                                                        </div>
                                                        <br />
                                                        <asp:CustomValidator ID="createBannerAdvertisement_BannerUploadValidCustomValidator"
                                                            ControlToValidate="createBannerAdvertisement_BannerUpload" Display="Dynamic" CssClass="text-danger"
                                                            OnServerValidate="createBannerAdvertisement_BannerUploadValidCustomValidator_ServerValidate"
                                                            ValidationGroup="createBannerAdvertisement_OnSubmitValidationGroup" ValidateEmptyText="true"
                                                            runat="server">*</asp:CustomValidator>
                                                        <asp:CustomValidator ID="createBannerAdvertisement_BannerUploadSelectedCustomValidator"
                                                            ControlToValidate="createBannerAdvertisement_BannerUpload" Display="Dynamic" CssClass="text-danger"
                                                            OnServerValidate="createBannerAdvertisement_BannerUploadSelectedCustomValidator_ServerValidate"
                                                            ValidationGroup="RegisterUserValidationGroup" ValidateEmptyText="true"
                                                            runat="server">*</asp:CustomValidator>
                                                    </div>
                                                </div>
                                            </asp:PlaceHolder>

                                            <asp:PlaceHolder runat="server" ID="StartPagePlaceHolder">
                                                <div class="form-group">
                                                    <div class="col-md-6 col-md-offset-2">
                                                        <div class="checkbox">
                                                            <asp:CheckBox runat="server" ID="PurchaseStartPageCheckBox" AutoPostBack="true" CssClass="margin20-fix" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <div class="col-md-6 col-md-offset-2">
                                                        <asp:Panel runat="server" ID="StartPageCalendarPanel">
                                                            <h3 class="m-b-20">
                                                                <%=L1.PRICE%>: <b><%= AppSettings.RevShare.AdPack.StartPagePrice %></b>
                                                            </h3>
                                                            <asp:UpdatePanel runat="server">
                                                                <ContentTemplate>
                                                                    <asp:Calendar ID="StartPageDateCalendar" runat="server" OnDayRender="StartPageDateCalendar_DayRender" CssClass="table table-condensed table-borderless calendar"></asp:Calendar>
                                                                </ContentTemplate>
                                                            </asp:UpdatePanel>
                                                        </asp:Panel>
                                                    </div>
                                                </div>
                                            </asp:PlaceHolder>

                                            <div class="form-group">
                                                <div class="col-md-2">
                                                    <asp:Button ID="CreateAdButton" runat="server"
                                                        ValidationGroup="RegisterUserValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="CreateAdButton_Click"
                                                        UseSubmitBehavior="false" />
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-12">
                                        <h2><%=U4000.ADDEDCAMPAIGNS %></h2>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">

                                        <asp:GridView ID="AdPacksAdGridView" runat="server" OnPreRender="BaseGridView_PreRender" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                                            DataSourceID="AdPacksAdGridViewDataSource" OnRowDataBound="AdPacksAdGridView_RowDataBound" 
                                            PageSize="20" OnRowCommand="AdPacksAdGridView_RowCommand" DataKeyNames='<%# new string[] { "Id", } %>'>
                                            <Columns>
                                                <asp:BoundField DataField='Title' HeaderText='Title' SortExpression='Title' />
                                                <asp:BoundField DataField='Description' HeaderText='Description' SortExpression='Description' />
                                                <asp:BoundField DataField='Status' HeaderText='Status' SortExpression='Status' />
                                                <asp:BoundField DataField='TargetUrl' HeaderText='Target Url' SortExpression='TargetUrl' />
                                                <asp:BoundField DataField='ConstantImagePath' HeaderText='Constant banner' SortExpression='Constant banner' ConvertEmptyStringToNull="true" />
                                                <asp:BoundField DataField='NormalImagePath' HeaderText='Normal banner' SortExpression='Normal banner' />
                                                <asp:TemplateField HeaderText="">
                                                    <ItemStyle />
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="EditAdPackAdButton" runat="server"
                                                            CommandName="edit"
                                                            CommandArgument='<%# Container.DataItemIndex %>'>
                                                        <span class="fa fa-arrow-right fa-lg text-success"></span>
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <asp:SqlDataSource ID="AdPacksAdGridViewDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="AdPacksAdGridViewDataSource_Init"></asp:SqlDataSource>

                                    </div>
                                </div>

                            </div>
                        </asp:View>

                        <asp:View runat="server" ID="View4" OnActivate="View4_Activate">
                            <div class="TitanViewElement">

                                <asp:Button runat="server" ID="CustomBuyAdPacksButton" OnClick="StartLendingButton_Click" Visible="false"
                                    CssClass="btn btn-inverse btn-block" Width="200" />

                                <titan:DistributionStatus id="DistributionStatus" runat="server" />

                                <asp:PlaceHolder runat="server" ID="CurrentAutomaticGroupPlaceHolder">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <p>
                                                <asp:Literal runat="server" ID="CurrentAutomaticGroupLiteral"></asp:Literal>
                                            </p>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                                <div class="row">
                                    <div class="col-md-12">
                                        <h2><%=AppSettings.RevShare.AdPack.AdPackNamePlural %> <%=L1.STATISTICS %></h2>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-12">
                                        <asp:GridView ID="AdPacksStatsGridView" runat="server" OnPreRender="BaseGridView_PreRender" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" EmptyDataText="<%$ ResourceLookup:NOSTATS %>"
                                            DataSourceID="AdPacksStatsGridView_DataSource" OnRowDataBound="AdPacksStatsGridView_RowDataBound" PageSize="20">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Select">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkSelect" runat="server" CssClass="selectableCheckbox" />
                                                    </ItemTemplate>
                                                    <HeaderTemplate>
                                                        <input type="checkbox" id="checkAll" class="regular-checkbox mini-checkbox allSelectableCheckbox" runat="server" onclick="<%=this.jsSelectAllCode %>"><label for="checkAll"></label>
                                                    </HeaderTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField='Id' HeaderText=' ' SortExpression='Id' ControlStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                                <asp:BoundField DataField='TotalClicks' SortExpression='TotalClicks' HeaderText="<%$ ResourceLookup:CLICKS %>" />
                                                <asp:TemplateField SortExpression='DisplayTime'>
                                                    <ItemTemplate>
                                                        <%#Eval("DisplayTime") %>s
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField='TotalNormalBannerImpressions' />
                                                <asp:BoundField DataField='TotalConstantBannerImpressions' />
                                                <asp:BoundField DataField='MoneyReturned' SortExpression='MoneyReturned' HeaderText="<%$ ResourceLookup:MONEYRETURNED %>" />
                                                <asp:BoundField DataField="Title" HeaderText="<%$ ResourceLookup:CAMPAIGN %>" SortExpression="Title" />
                                                <asp:BoundField DataField="StartPageDate" SortExpression="StartPageDate" />
                                                <asp:BoundField DataField="PurchaseDate" SortExpression="PurchaseDate" HeaderText="<%$ ResourceLookup:CREATED %>" />
                                            </Columns>
                                        </asp:GridView>
                                        <asp:SqlDataSource ID="AdPacksStatsGridView_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="AdPacksStatsGridView_DataSource_Init"></asp:SqlDataSource>
                                    </div>
                                </div>


                                <asp:Panel ID="NewSelectedPanel" runat="server" CssClass="displaynone optionBox">
                                    <div class="row">
                                        <div class="col-md-12 form-horizontal">
                                            <div class="form-group">
                                                <label class="control-label col-md-3"><%=L1.CAMPAIGN %>:</label>
                                                <div class="input-group">
                                                    <asp:PlaceHolder runat="server" ID="DropDownAdsPlaceHolder2">
                                                        <asp:DropDownList ID="ddlCampaigns2" runat="server" CssClass="col-md-3 form-control"></asp:DropDownList>
                                                    </asp:PlaceHolder>
                                                    <asp:Button runat="server" ID="RedirectToNewAdsButton2" OnClick="MenuButton_Click" CommandArgument="1" Text="<%$ ResourceLookup:ADDNEW %>" CssClass="col-md-3 form-control btn btn-primary" />
                                                    <asp:Button runat="server" ID="ChangeCampaignButton" OnClick="ChangeCampaignButton_Click" Text="<%$ ResourceLookup:CHANGECAMPAIGN %>" CssClass="col-md-3 form-control btn btn-primary" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <asp:PlaceHolder runat="server" ID="AdvertChangeWarningPlaceholder">
                                        <div class="row">
                                            <div class="col-md-12">
                                                <p>
                                                    <%=U5001.ADPACKADCHANGEWARNING.Replace("%n%",  AppSettings.RevShare.AdPack.AdPackName) %>
                                                </p>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                </asp:Panel>


                                <asp:Panel ID="TimeClicksExchangePanel" runat="server" CssClass="displaynone optionBox" Style="margin-top: 10px">
                                    <div class="row">
                                        <div class="col-md-12 form-horizontal">
                                            <label class="control-label col-md-3"><%=L1.SECONDS %>:</label>
                                            <div class="form-group">
                                                <asp:TextBox runat="server" ID="ExchangeSecondsTextBox" CssClass="col-md-3 form-control"></asp:TextBox>
                                                <asp:Button runat="server" ID="AddSecondsButton" OnClick="AddSecondsButton_Click" CssClass="col-md-3 form-control btn btn-primary" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12">
                                            <p><%=U5006.TIMEFORCLICKSEXCHANGEDESC %></p>
                                        </div>
                                    </div>
                                </asp:Panel>

                                <asp:PlaceHolder runat="server" ID="MyCustomGroupsPlaceHolder">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <h2><%=U4200.MYGROUPS %> </h2>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="table-responsive">
                                                <asp:GridView ID="MyGroupsGridView" runat="server" AllowSorting="true" AutoGenerateColumns="false" AllowPaging="true" PageSize="20"
                                                    EmptyDataText="<%$ ResourceLookup:NOTMEMBEROFGROUP %>" OnRowDataBound="MyGroupsGridView_RowDataBound"
                                                    DataSourceID="MyGroupsGridViewDataSource" OnPreRender="BaseGridView_PreRender">
                                                    <Columns>
                                                        <asp:BoundField DataField="Creator" HeaderText="<%$ ResourceLookup:GROUPCREATOR %>" HeaderStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" />
                                                        <asp:BoundField DataField="Name" HeaderText="<%$ ResourceLookup:NAME %>" SortExpression="Name" />
                                                        <asp:BoundField DataField="Color" />
                                                        <asp:BoundField DataField="AdPacksAdded" HeaderText="<%$ ResourceLookup:PACKSADDED %>" SortExpression="Percentage" />
                                                        <asp:BoundField DataField="AdPacksLimit" HeaderStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" />
                                                        <asp:BoundField DataField="Percentage" />
                                                        <asp:BoundField DataField="Accelerated" SortExpression="Accelerated" />
                                                        <asp:BoundField DataField="UCGID" HeaderText="Number of participants" />
                                                        <asp:BoundField DataField="CreatorName" HeaderText="<%$ ResourceLookup:GROUPCREATOR %>" SortExpression="CreatorName" />
                                                    </Columns>
                                                </asp:GridView>
                                                <asp:SqlDataSource ID="MyGroupsGridViewDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="MyGroupsGridViewDataSource_Init"></asp:SqlDataSource>
                                            </div>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                            </div>
                        </asp:View>
                    </asp:MultiView>
                </div>
            </asp:PlaceHolder>

            <%--CUTOM VIEW WITH ADDITIONAL OPTION OF BUY VIA TOKEN WALLET--%>
            <asp:PlaceHolder runat="server" ID="CustomAdPacksView" Visible="false">
                <div class="row">
                    <div class="col-md-6 col-md-offset-3 col-lg-6 col-lg-offset-3 LendingPacksPopUp" style="background-color: white">
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
                            <div class="col-md-5">
                                <div class="input-group">
                                    <span class="add-on input-group-addon"><%=Titan.Cryptocurrencies.CryptocurrencyFactory.Get(Titan.Cryptocurrencies.CryptocurrencyType.ERC20Token).Code %></span>
                                    <asp:TextBox runat="server" ClientIDMode="Static" ID="AyaToInvestTextBox" MaxLength="10" onchange="TokenTBChanged(); return false;" CssClass="form-control" />
                                </div>
                            </div>
                            <br />
                        </div>
                        <br />
                        <hr />
                        <asp:PlaceHolder runat="server" ID="CustomTypesDDLPlaceHolder">
                            <div class="col-md-5"  style="margin-top: 15px">
                                <asp:Label runat="server" ID="CustomTypesLabel" class="CustomTitleLabel" /><br />
                                <asp:DropDownList ID="CustomTypesDropDown" runat="server"  OnSelectedIndexChanged="CustomTypesDropDown_SelectedIndexChanged" class="CustomAdPackType" AutoPostBack="true" />
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder runat="server" ID="CustomCampaignDDLPlaceHolder">
                            <div class="col-md-5" runat="server" ID="SpaceDiv" visible="false" />
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
                                <tr>
                                    <td>Money value for packs: </td>
                                    <td style="font-weight: bold"><span id="MoneyValueOfPacksToBuy"></td>
                                </tr>
                                <tr>
                                    <td>Tokens value for packs: </td>
                                    <td style="font-weight: bold"><span id="TokenValueOfPacksToBuy"></td>
                                </tr>
                            </table>
                            <asp:label runat="server" ID="testLabel"></asp:label>
                        </div>
                        <br /><br />

                        <asp:PlaceHolder runat="server" ID="CustomPurchaseForReferralPlaceHolder">
                            <div class="col-md-5" id="ReferralCheckBox">
                                <asp:CheckBox runat="server" ID="CustomBuyForReferralCheckBox" OnCheckedChanged="CustomBuyForReferralCheckBox_CheckedChanged"  AutoPostBack="true"
                                    />
                                <asp:PlaceHolder runat="server" ID="CustomReferralsPlaceHolder" Visible="false">
                                    <asp:DropDownList ID="CustomReferralsDropDownList" runat="server" class="CustomAdPackType" />
                                </asp:PlaceHolder>
                            </div>
                        </asp:PlaceHolder>

                        <br />
                        <br />
                        <div class="col-md-12" id="TOSCheckBox" style="margin-bottom:15px">
                            <asp:CheckBox runat="server" ID="TOSAgreement" AutoPostBack="true" onchange="MoneyTBChanged(); return false;" OnCheckedChanged="TOSAgreement_CheckedChanged" />
                            <a href="sites/tos.aspx" style="font-weight: bold">&nbsp;<%=L1.TERMSOFSERVICE %></a>
                        </div>
                        <br />
                        <br />
                        <div class="form-group">
                            <div class="col-md-12">
                                <asp:Button ID="CustomPurchaseViaERC20TokensButton" runat="server"
                                    CssClass="btn btn-inverse btn-block mrgb"
                                    OnClick="PurchaseButtonViaERC20Tokens_Click"
                                    OnClientClick="ShowInProgressAnimation(); this.disabled = true;" UseSubmitBehavior="false"/>
                            </div>

                            <asp:PlaceHolder runat="server" ID="CustomPurchaseViaPurchaseBalancePlaceHolder">
                                <div class="col-md-12">
                                    <asp:Button ID="CustomPurchaseViaPurchaseBalanceButton" runat="server"
                                        CssClass="btn btn-inverse btn-block mrgb"
                                        OnClick="PurchaseButtonViaPurchaseBalance_Click"
                                        OnClientClick="ShowInProgressAnimation(); this.disabled = true;" UseSubmitBehavior="false"/>
                                </div>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder runat="server" ID="CustomPurchaseViaCashBalancePlaceHolder">
                                <div class="col-md-12">
                                    <asp:Button ID="CustomPurchaseViaCashBalanceButton" runat="server"
                                        CssClass="btn btn-inverse btn-block mrgb"
                                        OnClick="PurchaseButtonViaCashBalance_Click"
                                        OnClientClick="ShowInProgressAnimation(); this.disabled = true;" UseSubmitBehavior="false"/>
                                </div>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder runat="server" ID="CustomPurchaseViaCommissionBalancePlaceHolder" Visible="false">
                                <div class="col-md-12">
                                    <asp:Button ID="CustomPurchaseViaCommissionBalanceButton" runat="server"
                                        CssClass="btn btn-inverse btn-block mrgb"
                                        OnClick="PurchaseButtonViaCommissionBalance_Click"
                                        OnClientClick="ShowInProgressAnimation(); this.disabled = true;" UseSubmitBehavior="false"/>
                                </div>
                            </asp:PlaceHolder>

                            <div class="col-md-4">
                                <asp:Button ID="CancelLendingButton" runat="server" Text="Cancel"
                                    CssClass="btn btn-inverse btn-block mrgb"
                                    OnClick="CancelLendingButton_Click"
                                    UseSubmitBehavior="false" />
                            </div>

                            <%--IN PROGRESS ANIMATION--%>
                            <div class="col-md-2 col-md-offset-6" id="InProgressAnimation" runat="server">
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
</asp:Content>
