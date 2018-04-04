<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MenuBalances.ascx.cs" Inherits="Controls_Misc_MenuBalances" %>

<table class="menu-balance-table">
    <tr>
        <asp:Literal runat="server" ID="MainBalanceLiteral" />
    </tr>
    <asp:PlaceHolder runat="server" ID="AdBalancePlaceHolder">
        <tr>
            <asp:Literal runat="server" ID="AdBalanceLiteral" />
        </tr>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="PointsPlaceHolder">
        <tr>
            <asp:Literal runat="server" ID="PointsLiteral" /></tr>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="CashBalancePlaceHolder">
        <tr>
            <asp:Literal runat="server" ID="CashBalanceLiteral" /></tr>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="CommissionBalancePlaceHolder">
        <tr>
            <asp:Literal runat="server" ID="CommissionBalanceLiteral" /></tr>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="TrafficBalancePlaceHolder">
        <tr>
            <asp:Literal runat="server" ID="TrafficBalanceLiteral" /></tr>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="MarketPlaceBalancePlaceHolder">
        <tr>
            <asp:Literal runat="server" ID="MarketPlaceBalanceLiteral" /></tr>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="InvestmentBalancePlaceHolder">
        <tr>
            <asp:Literal runat="server" ID="InvestmentBalanceLiteral" /></tr>
    </asp:PlaceHolder>

        <asp:PlaceHolder runat="server" ID="BTCWalletPlaceHolder">
        <tr>
            <asp:Literal runat="server" ID="BTCWalletLiteral" /></tr>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="ERC20TokenPlaceHolder">
        <tr>
            <asp:Literal runat="server" ID="ERC20WalletLiteral" /></tr>
        <asp:PlaceHolder runat="server" ID="ERC20FreezedWalletPlaceHolder">
        <tr>
            <asp:Literal runat="server" ID="ERC20FreezedTokensWalletLiteral" /></tr>
        </asp:PlaceHolder>
    </asp:PlaceHolder>
</table>
