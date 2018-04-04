<%@ Page Language="C#" AutoEventWireup="true" CodeFile="plans.aspx.cs" Inherits="user_investmentplatform_plans" MasterPageFile="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script type="text/javascript">
        function pageLoad() {
            SetHeight();
            $(".preventClickBtn").click(function (e) { e.preventDefault(); });
            $('#confirmationModal').modal('hide');
        }

        function askForConfirmation(parameter) {
            var price = $(parameter).attr("data-moneyinsystem");
            var isBonus = $(parameter).attr("data-isbonusavaible");
            updateModal(price, isBonus);
            $('#confirmationModal').modal({ 'backdrop': true, 'show': true });
            let promise = new Promise(function (resolve, reject) {
                let confirmButton = document.getElementById('confirmButton');
                confirmButton.addEventListener("click", function () {
                    resolve();
                });
            });
            promise.then(function () {
                __doPostBack(parameter.name, '');
            });
        }

        function updateModal(price, isBonus) {
            var $modal = $('#confirmationModal');
            var priceWithSign = "";

            if ("<%=AppSettings.Site.IsCurrencySignBefore %>" == "True")
                priceWithSign = "<%=AppSettings.Site.CurrencySign %>" + price;
            else
                priceWithSign = price + "<%=AppSettings.Site.CurrencySign %>";

            if (isBonus == "True")
                $modal.find('.modal-body').html('<h2 class="text-center m-t-0"><%=string.Format(U6010.INVPLANTRANSFERCONFIRMATION, L1.MAINBALANCE) %><br /><%=U6010.INPLANNOTBONUS %> </h2><h3 class="text-success text-center">' + priceWithSign + '</h3>');
            else
                $modal.find('.modal-body').html('<h2 class="text-center m-t-0"><%=string.Format(U6010.INVPLANTRANSFERCONFIRMATION, L1.MAINBALANCE) %> </h2><h3 class="text-success text-center">' + priceWithSign + '</h3>');
        }

        var equalHeightKeeper = function (elem) {
            $(elem).css('height', 'auto');

            var maxHeight = $(elem).height();

            $(elem).each(function () {
                var height = $(this).outerHeight();
                if (height > maxHeight) {
                    maxHeight = height;
                }
            });

            $(elem).each(function () {
                $(this).css('height', maxHeight);
            });
        };

        $(function () {
            window.onresize = SetHeight;
        });

        function SetHeight() {
            equalHeightKeeper('.plan');
            equalHeightKeeper('.innerBorder');
        }

    </script>
    <style>
        .bordered .innerBorder {
            border: 1px solid #ccc;
            border-radius: 3px;
            padding: 15px;
            margin-bottom: 15px;
        }

        .bordered .plan {
            border: none !important;
        }

        .levelsTable tr td, .levelsTable tr th {
            text-align: center;
            border: 1px solid #e2e7eb !important;
        }

        .levelsTable .table-hover > tbody > tr:hover > td.levelsNameCol, .levelsTable .table-hover > tbody > tr:hover > th {
            background: #00acac !important;
        }

        .levelsNameCol {
            background-color: #00acac !important;
            color: white !important;
        }
    </style>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=AppSettings.InvestmentPlatform.LevelsEnabled ? U5007.LEVELS : U6006.PLANS %></h1>
    <div class="row">
        <div class="col-md-12">
            <p id="MainDescriptionP" runat="server" class="lead" />
        </div>
    </div>

    <asp:UpdatePanel ID="MultiViewUpdatePanel" runat="server" UpdateMode="Conditional">

        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ManageButton" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="PlansButton" EventName="Click" />
        </Triggers>

        <ContentTemplate>

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

            <div class="row">
                <div class="col-md-12">
                    <div class="nav nav-tabs custom text-right">
                        <div class="TitanViewPage">
                            <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                                <asp:Button ID="ManageButton" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                                <asp:Button ID="PlansButton" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </div>

            <div class="tab-content">
                <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
                    <asp:View runat="server" ID="BuyPlansView">
                        <div class="TitanViewElement">
                            <asp:PlaceHolder ID="BuyOptionsPlaceHolder" runat="server">
                                <div class="row">
                                    <div class="p-t-20">
                                        <div class="col-md-8 col-md-offset-2">
                                            <div class="form-group">
                                                <div class="col-md-8 col-md-offset-2">
                                                    <asp:PlaceHolder runat="server" ID="BuyControlsPlaceHolder">
                                                        <div class="row">
                                                            <div class="col-md-12">
                                                                <h4 class="p-b-20 text-center">
                                                                    <asp:Label ID="PurchaseDescriptionLabel" runat="server" />
                                                                </h4>
                                                            </div>
                                                        </div>

                                                        <asp:DropDownList ID="PlansDropDownList" runat="server" CssClass="form-control" OnSelectedIndexChanged="PlansDropDownList_SelectedIndexChanged" AutoPostBack="true" />

                                                        <h4 class="p-b-20 text-center">
                                                            <asp:PlaceHolder runat="server" ID="PurchaseBalanceInfoPlaceHolder">
                                                                <div class="row">
                                                                    <div class="col-md-12">
                                                                        <asp:Label ID="PurchaseBalanceLabel" runat="server" />
                                                                    </div>
                                                                </div>
                                                            </asp:PlaceHolder>
                                                            <asp:PlaceHolder runat="server" ID="CashBalanceInfoPlaceHolder">
                                                                <div class="row">
                                                                    <div class="col-md-12">
                                                                        <asp:Label ID="CashBalanceLabel" runat="server" />
                                                                    </div>
                                                                </div>
                                                                <br />
                                                            </asp:PlaceHolder>
                                                            <div class="row">
                                                                <div class="col-md-12 text-center m-b-10">
                                                                    <asp:Label ID="PriceLiteral" runat="server" />
                                                                </div>
                                                            </div>                                                            
                                                            <asp:PlaceHolder runat="server" ID="RangePricePlaceHolder">
                                                                <div class="row form-horizontal">                                                                    
                                                                    <div class="col-md-12 text-center">
                                                                        <div class="form-group">
                                                                            <label class="control-label col-md-6 col-lg-4"><%=U6012.TYPEPRICE %>:</label>
                                                                            <div class="col-md-6 col-lg-7">
                                                                                <div class="input-group">
                                                                                    <span class="add-on input-group-addon"><%=AppSettings.Site.CurrencySign %></span>
                                                                                    <asp:TextBox runat="server" ID="RangePriceTextBox" CssClass="form-control" />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </asp:PlaceHolder>
                                                        </h4>

                                                        <asp:Button ID="BuyFromPurchaseBalanceButton" runat="server" OnClick="BuyFromPurchaseBalanceButton_Click" CssClass="btn btn-inverse btn-block btn-lg" />
                                                        <asp:Button ID="BuyFromCashBalanceButton" runat="server" OnClick="BuyFromCashBalanceButton_Click" CssClass="btn btn-inverse btn-block btn-lg" />
                                                        <asp:Button ID="BuyViaPaymentProcessorButton" runat="server" OnClick="BuyViaPaymentProcessorButton_Click" CssClass="btn btn-inverse btn-block btn-lg" />
                                                    </asp:PlaceHolder>

                                                    <asp:PlaceHolder ID="PaymentProcessorsButtonPlaceHolder" runat="server">
                                                        <div class="row">
                                                            <div class="col-md-12 text-center">
                                                                <asp:Literal ID="PaymentButtons" runat="server" />
                                                            </div>
                                                        </div>
                                                    </asp:PlaceHolder>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </asp:PlaceHolder>

                            <div class="row">
                                <div class="col-md-12">
                                    <div class="row">
                                        <asp:PlaceHolder ID="InvestmentPlatformPlaceHolder" runat="server" Visible="false">
                                            <asp:PlaceHolder ID="InvestmentsPlansPlaceHolder" runat="server" />
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="LevelsPlaceHolder" runat="server" Visible="false">
                                            <br />
                                            <div class="levelsTable">
                                                <asp:GridView ID="LevelsGridView" runat="server" AllowSorting="False" AutoGenerateColumns="False" OnPreRender="BaseGridView_PreRender"
                                                    DataSourceID="LevelsSqlDataSource" OnRowDataBound="LevelsGridView_RowDataBound">
                                                    <Columns>
                                                        <asp:BoundField DataField='Id' ItemStyle-CssClass="levelsNameCol" HeaderStyle-CssClass="levelsNameCol" />
                                                        <asp:BoundField DataField='Price' />
                                                        <asp:BoundField DataField='Roi' />
                                                        <asp:BoundField DataField='LevelFee' />
                                                        <asp:BoundField DataField='LevelMaxPurchasePerDay' />
                                                        <asp:BoundField DataField='AvailableFromDate' />
                                                        <asp:BoundField DataField='PaymentProcessorInt' />
                                                    </Columns>
                                                </asp:GridView>
                                                <asp:SqlDataSource ID="LevelsSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="LevelsSqlDataSource_Init"></asp:SqlDataSource>
                                            </div>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="NoContetntPlaceHolder" runat="server" Visible="false">
                                            <asp:Literal ID="NoContentLiteral" runat="server" />
                                        </asp:PlaceHolder>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </asp:View>

                    <asp:View runat="server" ID="ManagePlansView">
                        <div class="TitanViewElement">
                            <div class="row">
                                <div class="col-md-12">
                                    <asp:Panel ID="InformationPanel" runat="server" CssClass="alert alert-success fade in m-b-15">
                                        <div class="row">
                                            <asp:Literal ID="InformationLiteral" runat="server" />
                                        </div>
                                    </asp:Panel>

                                    <div class="row">
                                        <asp:Label ID="MoneyInSystemLabel" runat="server" />
                                    </div>

                                    <asp:Button ID="WithdrawAllMoneyFromSystem" runat="server" Visible="false" OnClick="WithdrawAllMoneyFromSystem_Click" CssClass="btn btn-primary" />

                                    <asp:PlaceHolder ID="UsersPlanPlaceHolder" runat="server">
                                        <div class="row">
                                            <div class="col-md-12">
                                                <h3>
                                                    <asp:Label ID="ManageDescription" runat="server" /></h3>
                                            </div>
                                            <asp:PlaceHolder ID="UserPlanDetailsPlaceHolder" runat="server" />
                                        </div>
                                    </asp:PlaceHolder>

                                    <asp:PlaceHolder ID="NoPlansPlaceHolder" runat="server" Visible="false">
                                        <div class="row">
                                            <asp:Label ID="NoPlansLabel" runat="server" />
                                        </div>
                                    </asp:PlaceHolder>
                                </div>
                            </div>
                        </div>
                    </asp:View>

                </asp:MultiView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
