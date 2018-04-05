<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="upgrade.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script type="text/javascript">
        function updatePrice() {
            //Normal price
            var price1 = 0.0;
            var selectStringIndex = $('#<%=ddlOptions.ClientID%> option:selected').text().indexOf('-') + 1;
            price1 = parseFloat($('#<%=ddlOptions.ClientID%> option:selected').text().substring(selectStringIndex).replace('<%=AppSettings.Site.CurrencySign%>', '')).toFixed(<%=CoreSettings.GetMaxDecimalPlaces() %>);

            var totalPrice;
            totalPrice = '<%=AppSettings.Site.CurrencySign%>' + price1;

            $('#<%=PriceLiteral.ClientID%>').text(totalPrice);
        }

        function pageLoad() {
            $('#<%=CommissionsGridView.ClientID %>').DataTable({
                responsive: true,
                paginate: false,
                info: false,
                searching: false,
                ordering: false
            });
            $("#<%=UpgradeFromAdBalance.ClientID %>").click(function (e) { e.preventDefault(); });
            $("#<%=UpgradeFromCashBalance.ClientID %>").click(function (e) { e.preventDefault(); });
            $("#<%=UpgradeViaPaymentProcessor.ClientID %>").click(function (e) { e.preventDefault(); });
            updateListener(updateModal);
            $('#confirmationModal').modal('hide');
        }

        function askForConfirmation(parameter) {
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

        function updateListener(callback) {
            var price = $('#<%=PriceLiteral.ClientID %>').text();
            var upgrade = $('#<%=ddlOptions.ClientID %> option:selected').text();

            callback(price, upgrade);
            $('#<%=ddlOptions.ClientID %>').on('change', function () {
                price = $('#<%=PriceLiteral.ClientID %>').text();
                upgrade = $('#<%=ddlOptions.ClientID %> option:selected').text();

                callback(price, upgrade);
            });

        }

        function updateModal(price, upgrade) {
            var $modal = $('#confirmationModal');
            $modal.find('.modal-body').html('<h2 class="text-center m-t-0"><%=U6006.SURETOUPGRADE %>: </h2><h3 class="text-success text-center">' + upgrade + '</h3>');
        }


    </script>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <h1 class="page-header"><%= AppSettings.Points.LevelMembershipPolicyEnabled? U5007.LEVELS : L1.UPGRADE %></h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%= AppSettings.Points.LevelMembershipPolicyEnabled? String.Format(U5007.LEVELSINFO,AppSettings.PointsName) : L1.UPGRADEINFO %></p>
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

    <asp:Label ID="Label10" runat="server" CssClass="displaynone"></asp:Label>
    <asp:Label ID="LabelIle" runat="server" CssClass="displaynone"></asp:Label>


    <asp:Panel ID="WarningPanel" runat="server" Visible="false" CssClass="alert alert-warning fade in m-b-15 text-center">
        <asp:Literal ID="WarningLiteral" runat="server"></asp:Literal>
    </asp:Panel>



    <div class="page-section">


        <asp:PlaceHolder ID="BuyUpgradePlaceHolder" runat="server">
            <div class="row p-t-20">
                <div class="col-md-8 col-md-offset-2">
                    <div class="form-horizontal">
                        <h4 class="text-center m-b-20"><%=U6006.SELECTMEMBERSHIPUPGRADE %></h4>

                        <div class="form-group">
                            <div class="col-md-6 col-md-offset-3">
                                <asp:DropDownList ID="ddlOptions" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="form-group m-t-40 m-b-0">
                            <div class="col-lg-4 col-lg-offset-4 col-md-6 col-md-offset-3 col-sm-12">
                                <table class="table table-borderless table-condensed">
                                    <tr runat="server" id="adBalanceInfo">
                                        <td class="p-0"><%=U6012.PURCHASEBALANCE %>:</td>
                                        <td class="p-0">
                                            <asp:Literal ID="AdBalanceLiteral" runat="server"></asp:Literal></td>
                                    </tr>
                                    <tr runat="server" id="cashBalanceInfo">
                                        <td class="p-0"><%=U5008.CASHBALANCE %>:</td>
                                        <td class="p-0">
                                            <asp:Literal ID="CashBalanceLiteral" runat="server"></asp:Literal></td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-md-4">
                                <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8 col-md-offset-2">
                    <h4 class="p-b-20 text-center"><%=L1.PRICE %>:
                            <asp:Label ID="PriceLiteral" runat="server"></asp:Label></h4>
                </div>
            </div>
            <div class="row p-t-20">
                <div class="col-md-8 col-md-offset-2">
                    <div class="form-group">
                        <div class="col-md-4 col-md-offset-4">
                            <asp:PlaceHolder ID="UpgradeFromAdBalancePlaceHolder" runat="server">
                                <asp:Button ID="UpgradeFromAdBalance" runat="server" OnClientClick="askForConfirmation(this)" OnClick="upgradeFromAdBalance_Click" CssClass="btn btn-inverse btn-block btn-lg" />
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="UpgradeFromCashBalancePlaceHolder" runat="server">
                                <asp:Button ID="UpgradeFromCashBalance" runat="server" OnClientClick="askForConfirmation(this)" OnClick="upgradeFromCashBalance_Click" CssClass="btn btn-inverse btn-block btn-lg" />
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="UpgradeViaPaymentProcessorPlaceHolder" runat="server">
                                <asp:Button ID="UpgradeViaPaymentProcessor" runat="server" OnClientClick="askForConfirmation(this)" OnClick="upgradeViaPaymentProcessor_Click" CssClass="btn btn-inverse btn-block btn-lg" />
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </div>


        </asp:PlaceHolder>
        <asp:PlaceHolder ID="PaymentProcessorsButtonPlaceholder" runat="server">
            <div class="row p-t-20">
                <div class="col-md-8 col-md-offset-2">
                    <div class="form-group">
                        <div class="col-md-4 col-md-offset-4 payment-processors">
                            <asp:Literal ID="PaymentButtons" runat="server"></asp:Literal>
                        </div>
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>
    </div>


    <div class="page-section m-t-20">
        <div class="row">
            <div class="col-md-12">
                <div class="table-responsive">
                    <asp:GridView ID="UpgradeGridView" runat="server" Width="100%" OnPreRender="BaseGridView_PreRender"
                        OnRowDataBound="UpgradeGridView_RowDataBound" DataSourceID="ObjectDataSource1" AutoGenerateColumns="true">
                    </asp:GridView>
                    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetUpgradeDataSet" TypeName="Prem.PTC.HTML.DataCreator"></asp:ObjectDataSource>
                </div>
            </div>
        </div>
    </div>




    <asp:UpdatePanel runat="server">
        <ContentTemplate>

            <h2><%=U5009.COMMISSIONS %></h2>

            <div class="row">
                <div class="col-md-2">
                    <div class="form-group">
                        <div class="m-b-15">
                            <asp:DropDownList runat="server" ID="MembershipDDL" OnInit="MembershipDDL_Init" AutoPostBack="true"
                                OnSelectedIndexChanged="MembershipDDL_SelectedIndexChanged" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
            </div>

            <div class="page-section">
                <div class="row">
                    <div class="col-md-12">
                        <asp:GridView ID="CommissionsGridView" DataKeyNames='<%# new string[] { "Id", } %>' Width="100%"
                            AllowPaging="true" OnPreRender="BaseGridView_PreRender" AllowSorting="true" DataSourceID="CommissionsGridView_DataSource"
                            OnRowDataBound="CommissionsGridView_RowDataBound" AutoGenerateColumns="false"
                            runat="server" PageSize="20">
                            <Columns>
                                <asp:BoundField HeaderText="RefLevel" SortExpression="RefLevel" DataField="RefLevel" />
                                <asp:BoundField HeaderText="MembershipPurchasePercent" SortExpression="MembershipPurchasePercent" DataField="MembershipPurchasePercent" />
                                <asp:BoundField HeaderText="BannerPurchasePercent" SortExpression="BannerPurchasePercent" DataField="BannerPurchasePercent" />
                                <asp:BoundField HeaderText="AdPackPurchasePercent" SortExpression="AdPackPurchasePercent" DataField="AdPackPurchasePercent" />
                                <asp:BoundField HeaderText="OfferwallPercent" SortExpression="OfferwallPercent" DataField="OfferwallPercent" />
                                <asp:BoundField HeaderText="CPAOfferPercent" SortExpression="CPAOfferPercent" DataField="CPAOfferPercent" />
                                <asp:BoundField HeaderText="TrafficGridPurchasePercent" SortExpression="TrafficGridPurchasePercent" DataField="TrafficGridPurchasePercent" />
                                <asp:BoundField HeaderText="CashBalanceDepositPercent" SortExpression="CashBalanceDepositPercent" DataField="CashBalanceDepositPercent" />
                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="CommissionsGridView_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
                            OnLoad="CommissionsGridView_DataSource_Load" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:PlaceHolder runat="server" ID="AdPackPropsPlaceHolder">
        <div class="row">
            <div class="col-md-12">
                <h2><%=string.Format(U6000.ADPACKROIANDREPURCHASE, AppSettings.RevShare.AdPack.AdPackName) %></h2>
                <asp:Literal runat="server" ID="TypesMembershipProperties"></asp:Literal>
            </div>
        </div>
    </asp:PlaceHolder>



</asp:Content>
