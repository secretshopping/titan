<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="dicegame.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <link href="Plugins/Tipsy/tipsy.css" rel="stylesheet">
    <script type="text/javascript" src="Plugins/Tipsy/jquery.tipsy.js"></script>
    <script type="text/javascript">

        jQuery(function ($) {

            $('.tipsyTextbox').tipsy({ trigger: 'hover', gravity: 's', html: true, live: true });

        });
        var maxDecimalPlaces = <%=CoreSettings.GetMaxDecimalPlaces()%>;

        function truncateDecimals(num, digits) {
            var numS = parseFloat(num).toFixed(9),
                decPos = numS.indexOf('.'),
                substrLength = decPos == -1 ? numS.length : 1 + decPos + digits,
                trimmedResult = numS.substr(0, substrLength),
                finalResult = isNaN(trimmedResult) ? 0 : trimmedResult;

            return parseFloat(finalResult);
        }

        function isBettingEnabled() {
            var winChance = parseFloat($("#<%=chanceTextBox.ClientID%>").val());
            var multiplier = parseFloat($("#<%=multiplierTextBox.ClientID%>").val());
            var betAmount = parseFloat($("#<%=betAmountTextBox.ClientID%>").val());
            var profit = parseFloat($("#<%=profitTextBox.ClientID%>").val());
            var window = parseFloat($("#<%=windowCurrent.ClientID%>").text());

            if (winChance > 0 && multiplier > 0 && betAmount > 0 && profit > 0 && window != 2 && window != 3 && window != 4) {
                $("#<%=btnBetLow.ClientID%>").prop('disabled', false);
                $("#<%=btnBetHigh.ClientID%>").prop('disabled', false);
                $("#<%=btnBetHigh.ClientID%>").removeClass('disabled');
                $("#<%=btnBetHigh.ClientID%>").addClass('');
                $("#<%=btnBetLow.ClientID%>").removeClass('disabled');
                $("#<%=btnBetLow.ClientID%>").addClass('');
            }
            else {
                $("#<%=btnBetLow.ClientID%>").prop('disabled', true);
                $("#<%=btnBetHigh.ClientID%>").prop('disabled', true);
                $("#<%=btnBetHigh.ClientID%>").removeClass('');
                $("#<%=btnBetHigh.ClientID%>").addClass('disabled');
                $("#<%=btnBetLow.ClientID%>").removeClass('');
                $("#<%=btnBetLow.ClientID%>").addClass('disabled');
            }
        }

        function getMaxBet() {
            var houseEdge = $("#houseEdge").val();
            var winChance = parseFloat($("#<%=chanceTextBox.ClientID%>").val());
            var multiplier = parseFloat($("#<%=multiplierTextBox.ClientID%>").val());
            var maxProfit = parseFloat($("#<%=maxProfitLabel.ClientID%>").text());
            var btcBalance = parseFloat($("#<%=adBalanceLabel.ClientID%>").text()).toFixed(maxDecimalPlaces);
            if (winChance > 0) {
                if (isNaN(multiplier)) {
                    var setMultiplier = true;
                }
                multiplier = (100 - houseEdge) / winChance;
                if (multiplier > 1) {
                    var maxBetAmount = (maxProfit / (multiplier - 1)).toFixed(maxDecimalPlaces);

                    if (parseFloat(maxBetAmount, maxDecimalPlaces) > parseFloat(btcBalance, maxDecimalPlaces)) {
                        $("#<%=betAmountTextBox.ClientID%>").val(parseFloat(btcBalance));
                        calculateProfit();
                    }
                    else {
                        $("#<%=betAmountTextBox.ClientID%>").val(parseFloat(maxBetAmount));
                        $("#<%=profitTextBox.ClientID%>").val(maxProfit);
                    }
                }
                if (setMultiplier)
                    $("#<%=multiplierTextBox.ClientID%>").val(parseFloat(multiplier.toFixed(maxDecimalPlaces)));

            }
            else if (multiplier > 0) {
                if (isNaN(winChance)) {
                    var setWinChance = true;
                }
                var maxBetAmount = (maxProfit / (multiplier - 1)).toFixed(maxDecimalPlaces);
                if (maxBetAmount > btcBalance) {
                    $("#<%=betAmountTextBox.ClientID%>").val(parseFloat(btcBalance));
                    calculateProfit();
                }
                else {
                    $("#<%=betAmountTextBox.ClientID%>").val(parseFloat(maxBetAmount));
                    $("#<%=profitTextBox.ClientID%>").val(maxProfit);
                }
                if (setWinChance && multiplier > 0) {
                    winChance = (100 - houseEdge) / multiplier;
                    $("#<%=chanceTextBox.ClientID%>").val(parseFloat(winChance.toFixed(maxDecimalPlaces)));
                }
            }
        isBettingEnabled();
    }

    function multiply(multiplier) {
        var betAmountValue = parseFloat($("#<%=betAmountTextBox.ClientID%>").val());
        if (isNaN(betAmountValue) || betAmountValue <= 0) {
            $("#<%=betAmountTextBox.ClientID%>").val(0);
            isBettingEnabled();
            return;
        }
        var newAmount = betAmountValue * multiplier;
        $("#<%=betAmountTextBox.ClientID%>").val(parseFloat(newAmount.toFixed(maxDecimalPlaces)));
        calculateProfit();
    }

        // Displayed PROFIT value is cut to maxDecimalPlaces decimal places (without rounding)
    function calculateProfit() {

        var houseEdge = $("#houseEdge").val();
        var betAmount = parseFloat($("#<%=betAmountTextBox.ClientID%>").val());
        var winChance = parseFloat($("#<%=chanceTextBox.ClientID%>").val());
        var multiplier = parseFloat($("#<%=multiplierTextBox.ClientID%>").val());

        if (betAmount > 0 && winChance > betAmount) {
            var profit = (betAmount * (100 - houseEdge) / winChance - betAmount).toPrecision(maxDecimalPlaces);
            var cutProfit = truncateDecimals(profit, maxDecimalPlaces);
            $("#<%=profitTextBox.ClientID%>").val(parseFloat(cutProfit));
        }
        else if (multiplier > 0 && betAmount > 0) {
            var profit = (betAmount * multiplier - betAmount);
            var cutProfit = truncateDecimals(profit, maxDecimalPlaces);
            $("#<%=profitTextBox.ClientID%>").val(parseFloat(cutProfit));
        }
    calculateMultiplier();
    isBettingEnabled();
}

function calculateMultiplier() {
    var houseEdge = $("#houseEdge").val();
    var winChance = parseFloat($("#<%=chanceTextBox.ClientID%>").val());
    if (winChance > 0) {
        var multiplier = ((100 - houseEdge) / winChance).toFixed(maxDecimalPlaces);
        $("#<%=multiplierTextBox.ClientID%>").val(parseFloat(multiplier));
    }
}
// Displayed BET AMOUNT value is rounded
function calculateBetAmount(id) {

    var houseEdge = $("#houseEdge").val();

    if (id != "multiplierTextBox") {
        var winChance = parseFloat($("#<%=chanceTextBox.ClientID%>").val());
        var profit = parseFloat($("#<%=profitTextBox.ClientID%>").val());

        if (winChance > 1 && profit > 0) {
            var betAmount = (profit / ((100 - houseEdge) / winChance - 1));
            var cutBetAmount = truncateDecimals(betAmount, maxDecimalPlaces);
            var multiplier = ((100 - houseEdge) / winChance).toFixed(maxDecimalPlaces);

            $("#<%=betAmountTextBox.ClientID%>").val(parseFloat(cutBetAmount));
            $("#<%=multiplierTextBox.ClientID%>").val(parseFloat(multiplier));
        }
    }
    if (id != "chanceTextBox") {
        var multiplier = parseFloat($("#<%=multiplierTextBox.ClientID%>").val());
        var profit = parseFloat($("#<%=profitTextBox.ClientID%>").val());

        if (multiplier > 1 && profit > 0) {
            var betAmount = (profit / (multiplier - 1));
            var cutBetAmount = truncateDecimals(betAmount, maxDecimalPlaces);
            var winChance = ((100 - houseEdge) / multiplier).toFixed(maxDecimalPlaces);

            $("#<%=betAmountTextBox.ClientID%>").val(parseFloat(cutBetAmount));
            $("#<%=chanceTextBox.ClientID%>").val(parseFloat(winChance));
        }
    }
    isBettingEnabled();
}

function checkKelly(sender) {
    var kellyLevel;
    if (sender == 0) {
        kellyLevel = parseFloat($("#<%=kellyInvestTextBox.ClientID%>").val());
    }
    else if (sender == 1) {
        kellyLevel = parseFloat($("#<%=kellyDivestTextBox.ClientID%>").val());
    }
    var maxKelly = parseFloat($("#maxKelly").val());
    kellyShort = truncateDecimals(kellyLevel, 0);
    if (kellyShort <= 0 || kellyShort > maxKelly) {
        if (sender == 0) {
            $("#<%=kellyInvestTextBox.ClientID%>").val("");
        }
        else if (sender == 1) {
            $("#<%=kellyDivestTextBox.ClientID%>").val("");
        }
}
else {
    if (sender == 0) {
        $("#<%=kellyInvestTextBox.ClientID%>").val(kellyShort);
    }
    else if (sender == 1) {
        $("#<%=kellyDivestTextBox.ClientID%>").val(kellyShort);
    }
}
}
function parseAmount(sender) {
    var valueToParse;
    var valueParsed;
    if (sender == 0) {
        valueToParse = truncateDecimals(parseFloat($("#<%=investTextBox.ClientID%>").val()), maxDecimalPlaces);
        $("#<%=investTextBox.ClientID%>").val(valueToParse);
    }
    else if (sender == 1) {
        valueToParse = truncateDecimals(parseFloat($("#<%=divestTextBox.ClientID%>").val()), maxDecimalPlaces);
        $("#<%=divestTextBox.ClientID%>").val(valueToParse);
    }
}
function setConfirmations() {
    $("#<%=btnInvest.ClientID%>").click(function () {
        if (isNaN($("#<%=investTextBox.ClientID%>").val()) || isNaN($("#<%=kellyInvestTextBox.ClientID%>").val()))
            return;
        return confirm('<%= Resources.U4200.INVESTCONFIRMATION %>' + ' ' + $("#<%=investTextBox.ClientID%>").val() + '<%=AppSettings.Site.CurrencySign %>' + ' ' + '<%= Resources.U4200.WITHKELLY %>' + ' ' + $("#<%=kellyInvestTextBox.ClientID%>").val() + '?')
    });
    $("#<%=btnInvestAll.ClientID%>").click(function () { return confirm('<%= string.Format(U4200.INVESTAllCONFIRMATION, U6012.PURCHASEBALANCE) %>') });
    $("#<%=btnDivest.ClientID%>").click(function () {
        if (isNaN($("#<%=divestTextBox.ClientID%>").val()) || isNaN($("#<%=kellyDivestTextBox.ClientID%>").val()))
            return;
        return confirm('<%= Resources.U4200.DIVESTCONFIRMATION %>' + ' ' + $("#<%=divestTextBox.ClientID%>").val() + '<%=AppSettings.Site.CurrencySign %>' + ' ' + '<%= Resources.U4200.WITHKELLY %>' + ' ' + $("#<%=kellyDivestTextBox.ClientID%>").val() + '?')
    });
    $("#<%=btnDivestAll.ClientID%>").click(function () { return confirm('<%= Resources.U4200.DIVESTALLCONFIRMATION %>') });
}

$(document).ready(function () {
    isBettingEnabled();
    calculateProfit();
    setConfirmations();
});

    </script>

    <script>
        function pageLoad() {
            <%=PageScriptGenerator.GetGridViewCode(AllBetsGridView) %>
            <%=PageScriptGenerator.GetGridViewCode(MyBetsGridView) %>
            <%=PageScriptGenerator.GetGridViewCode(InvestmentsGridView) %>
        }
    </script>

    <style>
        .trans {
            display: none !important;
        }
        .dice-game-stats {
            font-weight: 700;
            margin-top: 10px;
        }
    </style>

</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%= U4200.DICE %></h1>
    
    <div class="row">
        <div class="col-md-12">
            <p class="lead">
                <%= U6004.DICEGAMEDESCRIPTION %>
            </p>
        </div>
    </div>
    <input id="houseEdge" value="<%=Prem.PTC.AppSettings.DiceGame.HouseEdgePercent.ToString() %>" class="displaynone" />
    <input id="maxKelly" value="<%=Prem.PTC.AppSettings.DiceGame.MaxKellyLevelInt.ToString() %>" class="displaynone" />

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="GamePanel">
        <ProgressTemplate>
            <div class="blackRefresh" id="blackOverlayLoading">
                <img class="refreshBlack" alt="" src="Images/Misc/loading.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="GamePanel" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnBetHigh" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnBetLow" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnDivest" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnDivestAll" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnInvest" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnInvestAll" EventName="Click" />
        </Triggers>
        <ContentTemplate>
            <div class="tab-content">
                <div class="row">
                    <div class="col-md-12">
                        <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger m-t-15">
                            <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                        </asp:Panel>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="row">
                                    <div class="form-group col-sm-12">
                                        <label class="control-label"><%= U4200.BETAMOUNT %></label>
                                        <div class="input-group">
                                            <asp:TextBox ClientIDMode="Static" Text="0.00004" ID="betAmountTextBox" onchange="calculateProfit(this.id); return false;" runat="server" CssClass="form-control"></asp:TextBox>
                                            <div class="input-group-btn">
                                                <button id="btnDouble" onclick="multiply(2); return false;" accesskey="x" class="btn btn-default">X2</button>
                                                <button id="btnHalf" onclick="multiply(0.5); return false;" accesskey="c" class="btn btn-default">/2</button>
                                                <button id="btnMax" onclick="getMaxBet(); return false;" accesskey="m" class="btn btn-default">M</button>
                                            </div>    
                                        </div>
                                    </div>
                                    <div class="form-group col-sm-6">
                                        <label class="control-label"><%= U4200.PROFIT %></label>
                                        <asp:TextBox ClientIDMode="Static" ID="profitTextBox" onchange="calculateBetAmount(this.id); return false;" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="form-group col-sm-6">
                                        <label class="control-label"><%= U4200.WINCHANCE %></label>
                                        <asp:TextBox ClientIDMode="Static" Text="49.5" ID="chanceTextBox" onchange="calculateProfit(this.id); calculateBetAmount(this.id); return false;" runat="server" CssClass="form-control"></asp:TextBox></td>
                                    </div>
                                    <div class="form-group col-sm-6">
                                        <label class="control-label"><%= U4200.MULTIPLIER %></label>
                                        <asp:TextBox ClientIDMode="Static" ID="multiplierTextBox" onchange="calculateBetAmount(this.id); calculateMultiplier(); return false;" runat="server" CssClass="form-control"></asp:TextBox></td>
                                    </div>
                                    <div class="form-group col-sm-6 m-t-10 text-right">
                                        <div class="input-group">
                                            <div class="input-group-btn">
                                                <asp:Button ClientIDMode="Static" ID="btnBetHigh" runat="server" OnClick="btnBet_Click" AccessKey="h"
                                                OnClientClick="isBettingEnabled();"
                                                UseSubmitBehavior="false" CommandArgument="false" CssClass="btn btn-lg btn-primary"/>  
                                                <asp:Button ClientIDMode="Static" ID="btnBetLow" AccessKey="l" runat="server" OnClick="btnBet_Click" 
                                                 OnClientClick="isBettingEnabled();"
                                                UseSubmitBehavior="false" CommandArgument="true" CssClass="btn btn-lg btn-primary"/>  
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-4 col-md-offset-1">
                                <table class="table table-borderless dice-game-stats">
                                    <tr>
                                        <td><%= U4200.SITESBANKROLL %></td>
                                        <td><asp:Label runat="server" ID="sitesBankrollLabel"></asp:Label>&nbsp;<%=AppSettings.Site.CurrencySign %></td>
                                    </tr>    
                                    <tr>
                                        <td><%= U4200.MAXPROFIT %></td>
                                        <td><asp:Label runat="server" ID="maxProfitLabel"></asp:Label>&nbsp;<%=AppSettings.Site.CurrencySign %></td>
                                    </tr>  
                                    <tr>
                                        <td><%= U4200.MAXWINCHANCE %></td>
                                        <td><asp:Label runat="server" ID="maxChanceLabel"></asp:Label></td>
                                    </tr>  
                                    <tr>
                                        <td><%= U6012.PURCHASEBALANCE%></td>
                                        <td><asp:Label runat="server" ID="adBalanceLabel"></asp:Label>&nbsp;<%=AppSettings.Site.CurrencySign %></td>
                                    </tr>  
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    
    <asp:UpdatePanel ID="MultiViewUpdatePanel" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="StatsButton" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="InvestButton" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="MyBetsButton" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="RandomizeButton" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="InvestButton" EventName="Click" />
        </Triggers>

        <ContentTemplate>
            <asp:Panel ID="windowCurrent" ClientIDMode="Static" runat="server" CssClass="displaynone">
                <asp:Literal ClientIDMode="Static" ID="currentWindow" runat="server"></asp:Literal>
            </asp:Panel>
            <div class="row">
                <div class="col-md-12">
                    <div class="nav nav-tabs custom text-right">
                        <div class="TitanViewPage">
                            <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                                <asp:Button ID="StatsButton" runat="server" OnClick="MenuButton_Click" CommandArgument="4" />
                                <asp:Button ID="InvestButton" runat="server" OnClick="MenuButton_Click" CommandArgument="3" />
                                <asp:Button ID="RandomizeButton" runat="server" OnClick="MenuButton_Click" CommandArgument="2" />
                                <asp:Button ID="MyBetsButton" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                                <asp:Button ID="AllBetsButton" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                            </asp:PlaceHolder>
                        </div>
                     </div>
                </div>
            </div>
            <div class="tab-content">
            <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">

                <asp:View runat="server" ID="AllBetsView">
                    <div class="TitanViewElement" style="margin-top: -1px">
                        <asp:Timer ID="UpdateTimer" runat="server" Interval="5000" OnTick="UpdateTimer_Tick"></asp:Timer>
                        <br />
                        <asp:UpdatePanel ID="AllBetsUpdatePanel" runat="server" UpdateMode="Conditional">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
                            </Triggers>
                            <ContentTemplate>
                                <asp:GridView ID="AllBetsGridView" runat="server" OnPreRender="BaseGridView_PreRender" AllowSorting="True" AutoGenerateColumns="False"
                                    DataSourceID="AllBetsGridViewSqlDataSource" OnRowDataBound="MyBetsGridView_RowDataBound" PageSize="30">
                                    <Columns>
                                        <asp:BoundField HeaderText="Bet Id" SortExpression='Id' DataField="Id" />
                                        <asp:BoundField HeaderText="User" SortExpression='UserName' DataField="UserName" />
                                        <asp:TemplateField HeaderText='Time' SortExpression='BetDate'>
                                            <ItemTemplate>
                                                <%# ((DateTime)Eval("BetDate")).ToString()  %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText='<%$ ResourceLookup : BET %>' SortExpression='BetSize' DataField="BetSize" />
                                        <asp:BoundField HeaderText='<%$ ResourceLookup : MULTIPLIER %>' SortExpression='Multiplier' DataField="Chance" />
                                        <asp:BoundField HeaderText='<%$ ResourceLookup : TARGET %>' SortExpression='Chance' DataField="Chance" />
                                        <asp:BoundField HeaderText='<%$ ResourceLookup : ROLL %>' SortExpression='Roll' DataField="Roll" />
                                        <asp:BoundField HeaderText='<%$ ResourceLookup : PROFIT %>'  SortExpression='Profit' DataField="Profit" />
                                        <asp:BoundField HeaderText="Low" SortExpression='Low' DataField="Low" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    </Columns>
                                </asp:GridView>
                                <asp:SqlDataSource ID="AllBetsGridViewSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="AllBetsGridViewSqlDataSource_Init"></asp:SqlDataSource>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </asp:View>
                <asp:View runat="server" ID="MyBetsView">
                    <div class="TitanViewElement" style="margin-top: -1px">
                        <br />
                        <asp:UpdatePanel ID="MyBetsUpdatePanel" runat="server" UpdateMode="Conditional">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btnBetHigh" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btnBetLow" EventName="Click" />
                            </Triggers>
                            <ContentTemplate>
                                <asp:GridView ID="MyBetsGridView" runat="server" AllowSorting="True" OnPreRender="BaseGridView_PreRender" AutoGenerateColumns="False"
                                    DataSourceID="MyBetsGridViewSqlDataSource" OnRowDataBound="MyBetsGridView_RowDataBound" PageSize="30">
                                    <Columns>
                                        <asp:BoundField HeaderText='<%$ ResourceLookup : BETID %>' SortExpression='Id' DataField="Id" />
                                        <asp:BoundField HeaderText='<%$ ResourceLookup : USER %>' SortExpression='UserName' DataField="UserName" />
                                        <asp:TemplateField HeaderText='<%$ ResourceLookup : TIME %>' SortExpression='BetDate'>
                                            <ItemTemplate>
                                                <%# ((DateTime)Eval("BetDate")).ToString()  %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText='<%$ ResourceLookup : BET %>' SortExpression='BetSize' DataField="BetSize" />
                                        <asp:BoundField HeaderText='<%$ ResourceLookup : MULTIPLIER %>' SortExpression='Multiplier' DataField="Chance" />
                                        <asp:BoundField HeaderText='<%$ ResourceLookup : TARGET %>' SortExpression='Chance' DataField="Chance" />
                                        <asp:BoundField HeaderText='<%$ ResourceLookup : ROLL %>' SortExpression='Roll' DataField="Roll" />
                                        <asp:BoundField HeaderText='<%$ ResourceLookup : PROFIT %>' SortExpression='Profit' DataField="Profit" />
                                        <asp:BoundField HeaderText="Low" SortExpression='Low' DataField="Low" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    </Columns>
                                </asp:GridView>
                                <asp:SqlDataSource ID="MyBetsGridViewSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="MyBetsGridViewSqlDataSource_Init"></asp:SqlDataSource>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </asp:View>
                <asp:View runat="server" ID="RandomizeView">
                    <div class="TitanViewElement">
                        
                        <div class="row m-b-15">
                            <div class="col-md-12">
                                <h3><%= U4200.RANDOMIZEROLLS %></h3>
                            </div>
                        </div>
                        <div class="row m-b-15">
                            <div class="col-md-12">
                                <h5><%= U4200.LASTSERVERSEED %></h5>
                            </div>
                            <div class="col-md-12 m-t-15">
                                <pre><asp:Label runat="server" ID="LastServerSeedLabel"></asp:Label></pre>
                            </div>
                        </div>
                        <div class="row m-b-15">
                            <div class="col-md-12">
                                <h5><%= U4200.LASTSERVERHASH %></h5>
                            </div>
                            <div class="col-md-12 m-t-15">
                                <pre><asp:Label runat="server" ID="LastServerSeedHashLabel"></asp:Label></pre>
                            </div>
                        </div>
                        <div class="row m-b-15">
                            <div class="col-md-12">
                                <h5><%= U4200.LASTCLIENTSEED %></h5>
                            </div>
                            <div class="col-md-12 m-t-15">
                                <pre><asp:Label runat="server" ID="LastClientSeedLabel"></asp:Label></pre>
                            </div>
                        </div>
                        <div class="row m-b-15">
                            <div class="col-md-12">
                                <h5><%= U4200.NEWSERVERHASH %></h5>
                            </div>
                            <div class="col-md-12 m-t-15">
                                <pre><asp:Label runat="server" ID="NewServerSeedHashLabel"></asp:Label></pre>
                            </div>
                        </div>
                        <div class="row m-b-15">
                            <div class="col-md-12">
                                <h5><%= U4200.NUMBEROFROLLS %></h5>
                            </div>
                            <div class="col-md-12 m-t-15">
                                <pre><asp:Label runat="server" ID="NumberOfRollsLabel"></asp:Label></pre>    
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <h5><%= U4200.RERANDOMIZESEED %></h5>
                            </div>
                            <div class="col-lg-6 col-md-8 col-sm-12 m-t-15">
                                <div class="form-group">
                                    <div class="input-group">                                    
                                        <asp:TextBox ClientIDMode="Static" ID="NewClientSeedTextBox" onchange="" runat="server" Font-Size="13px" CssClass="form-control" MaxLength="24"></asp:TextBox>
                                        <div class="input-group-btn">
                                            <asp:Button ID="btnRandomize" runat="server" OnClick="btnRandomize_Click" Text="<%$ ResourceLookup : RANDOMIZE %>" CssClass="btn btn-primary" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:View>
                <asp:View runat="server" ID="InvestView">
                    <div class="TitanViewElement">
                        
        
                        <div class="row">
                            <div class="col-md-12">
                                <h3><%= U4200.INVEST %></h3>
                                <h5><%= U4200.LENDSOME %></h5>
                                <p><%= U4200.INVESTDESCRITPION %></p>
                                <div class="form-inline">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <label class="control-label"><%= U4200.INVESTAMOUNT %></label>
                                            <asp:TextBox ClientIDMode="Static" ID="investTextBox" onchange="parseAmount(0); return false;" runat="server" CssClass="form-control tipsyTextbox"></asp:TextBox>
                                        </div>
                                        <div class="input-group">
                                            <label class="control-label"><%= U4200.KELLYLEVEL %></label>
                                            <asp:TextBox ClientIDMode="Static" ID="kellyInvestTextBox" runat="server" CssClass="form-control tipsyTextbox" onchange="checkKelly(0); return false;"></asp:TextBox>
                                        </div>
                                        <div class="input-group">
                                            <label class="control-label">&nbsp;</label>
                                            <div class="">
                                                <asp:Button ClientIDMode="Static" ID="btnInvest" runat="server" OnClick="btnInvest_Click" CommandArgument="0" CssClass="btn btn-primary" />
                                                <asp:Button ClientIDMode="Static" ID="btnInvestAll" runat="server" OnClick="btnInvest_Click" CommandArgument="1" CssClass="btn btn-primary" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-12">
                                <h3><%= U4200.DIVEST %></h3>
                        
                                <h5><%= U4200.DIVESTDESCRIPTION %></h5>
                                <p><asp:Label runat="server" ID="Label4"></asp:Label></p>

                                <div class="form-inline">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <label class="control-label"><%= U4200.DIVESTAMOUNT %></label>
                                            <asp:TextBox ClientIDMode="Static" ID="divestTextBox" onchange="parseAmount(1); return false;" runat="server" CssClass="form-control tipsyTextBox"></asp:TextBox>
                                        </div>
                                        <div class="input-group">
                                            <label class="control-label"><%= U4200.KELLYLEVEL %></label>
                                            <asp:TextBox ClientIDMode="Static" ID="kellyDivestTextBox" runat="server" CssClass="form-control tipsyTextbox" onchange="checkKelly(1); return false;"></asp:TextBox>
                                        </div>
                                        <div class="input-group">
                                            <label class="control-label">&nbsp;</label>
                                            <div class="">
                                                <asp:Button ClientIDMode="Static" ID="btnDivest" runat="server" OnClick="btnInvest_Click" CommandArgument="2" CssClass="btn btn-primary" />
                                                <asp:Button ClientIDMode="Static" ID="btnDivestAll" runat="server" OnClick="btnInvest_Click" CommandArgument="3" CssClass="btn btn-primary" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-12">
                                <h3><%= U4200.YOURINVESTMENTS %></h3>
                        
                                <asp:UpdatePanel ID="InvestmentsUpdatePanel" runat="server" UpdateMode="Conditional">
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="btnDivest" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="btnDivestAll" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="btnInvest" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="btnInvestAll" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="btnBetLow" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="btnBetHigh" EventName="Click" />
                                    </Triggers>
                                    <ContentTemplate>
                                        <asp:GridView ID="InvestmentsGridView" runat="server" AllowSorting="true" OnPreRender="BaseGridView_PreRender"
                                            AutoGenerateColumns="False" DataSourceID="InvestmentsGridViewSqlDataSource" PageSize="30">
                                            <Columns>
                                                <asp:BoundField HeaderText='<%$ ResourceLookup : AMOUNT %>' SortExpression='Amount' DataField="Amount" />
                                                <asp:BoundField HeaderText='<%$ ResourceLookup : KELLYLEVEL %>' SortExpression='KellyInt' DataField="KellyInt" />
                                            </Columns>
                                        </asp:GridView>
                                        <asp:SqlDataSource ID="InvestmentsGridViewSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="InvestmentsGridViewSqlDataSource_Init"></asp:SqlDataSource>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>
                </asp:View>
                <asp:View runat="server" ID="StatsView" OnActivate="StatsView_Activate">
                    <div class="TitanViewElement">
                        <div class="row">
                            <div class="col-md-6">
                                <table class="table">
                                    <thead>
                                        <tr>
                                            <th></th>
                                            <th><%=user.Name %>
                                            </th>
                                            <th><%=AppSettings.Site.Name %>
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <th>
                                                <%=U5003.BETS %>
                                            </th>
                                            <td>
                                                <asp:Literal runat="server" ID="UsersBetsLiteral"></asp:Literal>
                                            </td>
                                            <td>
                                                <asp:Literal runat="server" ID="SitesBetsLiteral"></asp:Literal>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>
                                                <%=U5003.WAGERED %>
                                            </th>
                                            <td>
                                                <asp:Literal runat="server" ID="UsersWageredLiteral"></asp:Literal>
                                            </td>
                                            <td>
                                                <asp:Literal runat="server" ID="SitesWageredLiteral"></asp:Literal>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>
                                                <%=U5003.WINS %>
                                            </th>
                                            <td>
                                                <asp:Literal runat="server" ID="UsersWinsLiteral"></asp:Literal>
                                            </td>
                                            <td>
                                                <asp:Literal runat="server" ID="SitesWinsLiteral"></asp:Literal>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>
                                                <%=U5003.LOSSES %>
                                            </th>
                                            <td>
                                                <asp:Literal runat="server" ID="UsersLossesLiteral"></asp:Literal>
                                            </td>
                                            <td>
                                                <asp:Literal runat="server" ID="SitesLossesLiteral"></asp:Literal>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>
                                                <%=U4200.PROFIT %>
                                            </th>
                                            <td>
                                                <asp:Literal runat="server" ID="UsersProfitLiteral"></asp:Literal>
                                            </td>
                                            <td>
                                                <asp:Literal runat="server" ID="SitesProfitLiteral"></asp:Literal>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </asp:View>
            </asp:MultiView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    
</asp:Content>



