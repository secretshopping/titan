<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CryptocurrencyBalancesInfo.ascx.cs" Inherits="Controls_CryptocurrencyBalancesInfo" %>

<asp:PlaceHolder ID="CryptocurrencyPanelControl" runat="server" Visible="false">
    <div class="alert alert-info fade in m-b-15 text-center">
        <span style="vertical-align: middle; margin-right:6px">
            <asp:Literal runat="server" ID="ControlTextLiteral" />
        </span>
        <a href="user/transfer.aspx" class='btn btn-primary btn-xs m-r-5'><i class='fa fa-plus'></i> <%=L1.TRANSFER %></a>
        <a href="user/cashout.aspx" class='btn btn-primary btn-xs m-r-5'><i class='fa fa-arrow-up'></i> <%=L1.CASHOUT %></a>
    </div>
</asp:PlaceHolder>
