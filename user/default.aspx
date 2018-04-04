<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <link href="Scripts/default/assets/plugins/jvectormap/jquery-jvectormap-1.2.2.css" rel="stylesheet" />
    <script src="Scripts/default/assets/plugins/jvectormap/jquery-jvectormap-1.2.2.min.js"></script>
    <script src="Scripts/default/assets/plugins/jvectormap/jquery-jvectormap-world-mill-en.js"></script>
    <script>

        function pageLoad() {
            try {
                $('#AdPacksToBuyTextBox').keyup(function () {
                    AdPacksTBChanged();
                });

                $('#MoneyToInvestTextBox').keyup(function () {
                    MoneyTBChanged();
                });

            } catch (err) { }

            var mainColor = $(".sidebar .nav>li.active>a").first().css('background-color');
            $(".LendingPacksPopUp").css("border-color", mainColor);
            $(".CustomAdPackType").css("background-color", mainColor);
        }

        $(function () {
            instantAccruals();
        });
    </script>
    <style>
        .trans {
            display: none !important;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=L1.SUMMARY %> </h1>
    <asp:PlaceHolder runat="server" ID="WarningMemberExpiresAndReferralsResolvedPlaceHolder" Visible="false">
        <div class="alert alert-danger">
            <button type="button" class="close" aria-label="Close" data-dismiss="alert">
                <span aria-hidden="true">&times;
                </span>
            </button>
            <h4>
                <asp:Literal runat="server" ID="ReferralsWillBeResolvedWarningLiteral"></asp:Literal>
            </h4>
            <p>
                <asp:Literal runat="server" ID="ReferralsWillBeResolveMessageLiteral"></asp:Literal>
            </p>
        </div>
    </asp:PlaceHolder>

    <titan:AwaitingPaymentConfirmationWindow runat="server" />
    <titan:UserBalances runat="server" />
    <asp:PlaceHolder runat="server" ID="ConverterPlaceholder" Visible="false">
        <script>
            $(function () {
                tokenPrice = $('#converterTokenRate').val();
                watchConverter();
                $('#converterWalletInput').keyup(function () {
                    watchConverter();
                });
            });

            function watchConverter() {

                var numberOfTokens = parseFloat($('#converterWalletInput').val()) || 0;
                numberOfTokens = isNaN(numberOfTokens) ? 0 : numberOfTokens;

                var tokenValue = numberOfTokens / tokenPrice;

                $("#converterTokenInput").val(tokenValue.toFixed(<%=CoreSettings.GetMaxDecimalPlaces()%>));
            }
        </script>
        <style>
            .converter .input-group .input-group-addon {
                border: 1px solid #ccd0d4 !important;
            }
        </style>
        <div class="col-md-6">
            <div class="panel panel-inverse">
                <div class="panel-heading">
                    <h4 class="panel-title">Converter</h4>
                </div>
                <div class="panel-body">
                    <div class="form-group">
                        <div class="input-group">
                            <input type="hidden" id="converterTokenRate" value="<%=AppSettings.Ethereum.ERC20TokenRate.ToDecimal() %>" />
                            <span class="bg-primary input-group-addon"><%=AppSettings.Site.CurrencySign %></span>
                            <input type="text" id="converterWalletInput" class="form-control">
                            <span class="bg-primary input-group-addon">>></span>
                            <input type="text" id="converterTokenInput" class="form-control">
                            <span class="bg-primary input-group-addon"></span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="LastJakpotsWinnersPlaceHolder" Visible="false">
        <div class="alert note note-info">
            <h4>
                <asp:Literal ID="JakpotsWinnersLiteral" runat="server" />
            </h4>
            <br />
            <h5>
                <asp:Literal ID="JakpotsWinnersDetailsLiteral" runat="server" />
            </h5>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="NoEarnAccessPlaceHolder" Visible="false">
        <div class="alert note" style="background-color: lightcoral; color: darkred">
            <h4><b><%=U6010.WARNINGNODAILYTASK %></b></h4>
            <br />
            <%=U6010.WARNINGNODAILYTASKINFO %>
        </div>
    </asp:PlaceHolder>

    <titan:MatrixInfo runat="server"></titan:MatrixInfo>

    <%--Instant Accruals Chart--%>
    <link href="Scripts/default/assets/plugins/morrisjs/morris.css" rel="stylesheet" />
    <script src="Scripts/default/assets/plugins/raphael/raphael.min.js"></script>
    <script src="Scripts/default/assets/plugins/morrisjs/morris.min.js"></script>
    <script>
        var inter;

        function instantAccruals() {
            if ('<%=AppSettings.RevShare.AdPack.InstantAccrualsEnabled %>' == 'True') {
                getAdPacksEarningChart();
            }
        }

        function getAdPacksEarningChart() {
            $.ajax({
                type: "POST",
                url: "<%= ResolveUrl("default.aspx/GetJsonWithInstantAccrualsValues") %>",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    updateAdPacksEarningChart(data);
                    inter = setInterval(function () {
                        updateAdPacksEarningChart(data);
                    }, <%=InstantAccrualsIntervalTime %>);
                }
            });
        }

        function updateAdPacksEarningChart(data) {
            $('#line_chart').empty();

            var jData = JSON.parse(data.d);
            var dataLength = jData.length - 1;
            var commissionPerSecond = jData[dataLength].earningPerSecond;
            if (commissionPerSecond == 0) 
                clearInterval(inter);
            jData.splice(dataLength, 1);
            dataLength = jData.length - 1;

            var tmpDate = new Date();
            var earningSeconds = tmpDate.getMinutes() * 60 + tmpDate.getSeconds();
            var currentMoney = jData[dataLength].money + earningSeconds * commissionPerSecond;
            var decimals = <%=CoreSettings.GetMaxDecimalPlaces() %>;
            var multiplier = Math.pow(10, decimals);
            var money = parseFloat(Math.round(currentMoney * multiplier) / multiplier).toFixed(decimals);
            var htmlCode = '<%=string.Format("{0}: <span class=\"text-primary\"><b>{1}", U6012.TODAYEARNINGS, AppSettings.Site.CurrencySign) %>' + money + '</b><span>';
            $("#<%=InstantAccrualLiteral.ClientID%>").html(htmlCode);

            jData[dataLength].money = money;
            new Morris.Line({
                element: 'line_chart',
                data: jData,
                xkey: 'date',
                ykeys: ['percentage'],
                labels: ['Percentage'],
                hoverCallback: function (index, options, content) {
                    var data = options.data[index];
                    content += 'Money: <%=AppSettings.Site.CurrencySign %>' + data.money;
                    return content;
                },
                hideHover: true,
                yLabelFormat: function (y) {
                    return parseFloat(Math.round(y * 100) / 100).toFixed(2) + '%';
                }
            });            
        }

        window.onresize = getAdPacksEarningChart;

    </script>
    <asp:PlaceHolder runat="server" ID="InstantAccrualChartPlaceHolder">
        <div class="row">
            <div class="col-md-12">
                <div class="panel panel-inverse">
                    <div class="panel-heading">
                        <h4 class="panel-title">
                            <%=string.Format(U6012.ADPACKEARNINGSFROMLASTDAYS, AppSettings.RevShare.AdPack.AdPackNamePlural) %>
                        </h4>
                    </div>
                    <div class="panel-body">
                        <p class="text-center">
                            <asp:Label runat="server" ID="InstantAccrualLiteral" ClientIDMode="Static" Width="200px" CssClass="form-control" />
                        </p>
                        <div id="line_chart"></div>
                    </div>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
    <%--!Instant Accruals Chart--%>

    <div class="row">
        <div class="col-md-6 ui-sortable">
            <div class="panel panel-inverse" data-sortable-id="table-basic-1" data-init="true">
                <div class="panel-heading">
                    <div class="panel-heading-btn">
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-default" data-click="panel-expand"><i class="fa fa-expand"></i></a>
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-warning" data-click="panel-collapse"><i class="fa fa-minus"></i></a>
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-danger" data-click="panel-remove"><i class="fa fa-times"></i></a>
                    </div>
                    <h4 class="panel-title">
                        <asp:Literal runat="server" Text="<%$ResourceLookup: ALLCREDITEDMONEY%>" />
                    </h4>
                </div>
                <div class="panel-body">
                    <titan:Statistics runat="server" StatType="User_AllCreditedMoney" Width="348px" Height="230px" IsDecimal="false"></titan:Statistics>
                </div>
            </div>
        </div>



        <div class="col-md-6 ui-sortable">
            <div class="panel panel-inverse">
                <div class="panel-heading">
                    <h4 class="panel-title">Balances </h4>
                </div>
                <div class="panel-body">
                    <titan:Statistics runat="server" StatType="UserBalancesPercents"></titan:Statistics>
                </div>
            </div>
        </div>

        <div class="col-md-12 ui-sortable">
            <div class="panel panel-inverse">
                <div class="panel-heading">
                    <h4 class="panel-title">Users Statistics </h4>
                </div>
                <div class="panel-body">
                    <titan:Statistics runat="server" StatType="CountriesWithMembers"></titan:Statistics>
                </div>
            </div>
        </div>

        <div class="col-md-6 ui-sortable" runat="server" id="pointsGraph">
            <div class="panel panel-inverse" data-sortable-id="table-basic-2" data-init="true">
                <div class="panel-heading">
                    <div class="panel-heading-btn">
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-default" data-click="panel-expand"><i class="fa fa-expand"></i></a>
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-warning" data-click="panel-collapse"><i class="fa fa-minus"></i></a>
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-danger" data-click="panel-remove"><i class="fa fa-times"></i></a>
                    </div>
                    <h4 class="panel-title"><%=U5001.TOTALPOINTSCREDITEDTOYOU.Replace("%n%", AppSettings.PointsName) %> </h4>
                </div>
                <div class="panel-body">
                    <titan:Statistics runat="server" StatType="User_AllPointsCredited" Width="348px" Height="230px" ID="UserStats" IsInt="true"></titan:Statistics>
                </div>
            </div>
        </div>

        <asp:UpdatePanel runat="server" ID="GridViewsUpdatepanel">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="BalanceLogslinkButtonRefresh" EventName="click" />
                <asp:AsyncPostBackTrigger ControlID="HistoryLinkButtonRefresh" EventName="click" />
            </Triggers>
            <ContentTemplate>

                <div class="col-md-6 ui-sortable">
                    <div class="panel panel-inverse" data-sortable-id="table-basic-3" data-init="true">
                        <div class="panel-heading">
                            <div class="panel-heading-btn">
                                <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-default" data-click="panel-expand"><i class="fa fa-expand"></i></a>
                                <asp:LinkButton ID="BalanceLogslinkButtonRefresh" runat="server" OnClick="BalanceLogslinkButtonRefresh_Click" CssClass="btn btn-xs btn-icon btn-circle btn-success" >
                                    <i class="fa fa-repeat"></i>
                                </asp:LinkButton>
                                <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-warning" data-click="panel-collapse"><i class="fa fa-minus"></i></a>
                                <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-danger" data-click="panel-remove"><i class="fa fa-times"></i></a>
                            </div>
                            <h4 class="panel-title"><%=U4000.LATEST %> <%=U4000.BALANCELOG %> </h4>
                        </div>
                        <div class="panel-body">
                            <asp:GridView ID="LogsGridView" runat="server" AllowSorting="False" AutoGenerateColumns="False" OnPreRender="BaseGridView_PreRender" DataSourceID="LogsSqlDataSource" OnRowDataBound="LogsGridView_RowDataBound">
                                <Columns>
                                    <asp:TemplateField HeaderText='<%$ ResourceLookup : DATE %>' SortExpression='DateOccured'>
                                        <ItemTemplate><%# ((DateTime)Eval("DateOccured")).ToShortDateString() %> </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField='Note' HeaderText='<%$ ResourceLookup : ENTRY %>' SortExpression='Note' />
                                    <asp:BoundField DataField='Amount' HeaderText='<%$ ResourceLookup : AMOUNT %>' SortExpression='Amount' />
                                    <asp:BoundField DataField='Balance' HeaderText='<%$ ResourceLookup : FROM %>' SortExpression='Balance' />
                                </Columns>
                            </asp:GridView>
                            <asp:SqlDataSource ID="LogsSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="LogsSqlDataSource_Init"></asp:SqlDataSource>
                        </div>
                    </div>
                </div>
                <div class="col-md-6 ui-sortable">
                    <div class="panel panel-inverse" data-sortable-id="table-basic-4" data-init="true" runat="server" id="LatestHistoryLogsPanel">
                        <div class="panel-heading">
                            <div class="panel-heading-btn">
                                <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-default" data-click="panel-expand"><i class="fa fa-expand"></i></a>
                                <asp:LinkButton ID="HistoryLinkButtonRefresh" runat="server" OnClick="HistoryLinkButtonRefresh_Click" CssClass="btn btn-xs btn-icon btn-circle btn-success" >
                                    <i class="fa fa-repeat"></i>
                                </asp:LinkButton>
                                <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-warning" data-click="panel-collapse"><i class="fa fa-minus"></i></a>
                                <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-danger" data-click="panel-remove"><i class="fa fa-times"></i></a>
                            </div>
                            <h4 class="panel-title"><%=U4000.LATEST %> <%=U4000.HISTORYLOG %> </h4>
                        </div>
                        <div class="panel-body">
                            <asp:GridView ID="HistoryGridView" runat="server" AllowSorting="False" AutoGenerateColumns="False" OnPreRender="BaseGridView_PreRender" DataSourceID="HistorySqlDataSource" OnRowDataBound="HistoryGridView_RowDataBound">
                                <Columns>
                                    <asp:TemplateField HeaderText='<%$ ResourceLookup : DATE %>' SortExpression='Date'>
                                        <ItemTemplate><%# ((DateTime)Eval("Date")).ToShortDateString() %> </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText='<%$ ResourceLookup : ENTRY %>' SortExpression='Id'>
                                        <ItemTemplate><%# (new Prem.PTC.Members.History((int)Eval("Id"))).GetFullText() %> </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:CheckBoxField DataField="IsRead" HeaderText="IsRead" SortExpression="IsRead" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                </Columns>
                            </asp:GridView>
                            <asp:SqlDataSource ID="HistorySqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="HistorySqlDataSource_Init"></asp:SqlDataSource>
                        </div>
                    </div>
                </div>

            </ContentTemplate>
        </asp:UpdatePanel>

        <titan:Shoutbox runat="server" MessagesInFeed="25" />
        <div class="col-md-6 ui-sortable" id="AdPacksPlaceHolder" runat="server">
            <div class="panel panel-inverse" data-sortable-id="table-basic-4" data-init="true">
                <div class="panel-heading">
                    <div class="panel-heading-btn">
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-default" data-click="panel-expand"><i class="fa fa-expand"></i></a>
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-warning" data-click="panel-collapse"><i class="fa fa-minus"></i></a>
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-danger" data-click="panel-remove"><i class="fa fa-times"></i></a>
                    </div>
                    <h4 class="panel-title"><%=AppSettings.RevShare.AdPack.AdPackNamePlural %> </h4>
                </div>
                <titan:AdPackUserList runat="server" />
            </div>
        </div>

    </div>
    <asp:PlaceHolder ID="WelcomeModalScript" Visible="false" runat="server">
        <script>
            $(function () {
                $('#welcomeModal').modal();
            });


        </script>
    </asp:PlaceHolder>

    <titan:AccountActivationPopUp runat="server" />

</asp:Content>

