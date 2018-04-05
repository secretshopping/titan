<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MemberBalances.ascx.cs" Inherits="Controls_MemberBalances" %>
<%--<h3>Balances</h3>
<table style="margin-left: -1px">
    <tr>
        <td style="width: 100px; font-weight: bold"><%=U5004.MAIN%></td>
        <td class="greenText" style="font-weight: bold">
            <asp:Label ID="MainBalanceLabel" runat="server"></asp:Label></td>
    </tr>
    <tr>
        <td><%=U6012.PURCHASE %></td>
        <td class="greenText">
            <asp:Label ID="AdBalanceLabel" runat="server"></asp:Label></td>
    </tr>
    <tr id="CommissionBalancePlaceHolder" runat="server" visible="false">
        <td><%=U5004.COMMISSION %></td>
        <td class="greenText">
            <asp:Label ID="CommissionBalanceLabel" ClientIDMode="Static" runat="server"></asp:Label>
        </td>
    </tr>
    <tr id="CashBalancePlaceHolder" runat="server" visible="false">
        <td><%=U5008.CASHBALANCE %></td>
        <td class="greenText">
            <asp:Label ID="CashBalanceLabel" ClientIDMode="Static" runat="server"></asp:Label>
        </td>
    </tr>
    <tr id="trafficBalanceTrId" runat="server">
        <td><%=U5004.TRAFFIC %></td>
        <td class="greenText">
            <asp:Label ID="TrafficBalanceLabel" runat="server"></asp:Label></td>
    </tr>
    <tr id="PointsBalanceRow" runat="server">
        <td><%=AppSettings.PointsName %></td>
        <td class="greenText">
            <asp:Label ID="MemberBalancesControlPointsLabel" ClientIDMode="Static" runat="server"></asp:Label>
        </td>
    </tr>
    <tr id="BTCBalanceTableRow" runat="server" visible="false">
        <td>BTC</td>
        <td class="greenText">
            <asp:Label ID="BTCBalance" ClientIDMode="Static" runat="server"></asp:Label>
        </td>
    </tr>
    <tr id="AdCreditsBalanceTrId" runat="server" visible="false">
        <td><%=U5006.ADCREDITS %></td>
        <td class="greenText">
            <asp:Label ID="AdCreditsBalanceLabel" runat="server"></asp:Label></td>
    </tr>
</table>--%>

<ul class="nav navbar-nav navbar-right">
    <li class="dropdown navbar-user">
    <table>
	    <a href="javascript:;" class="dropdown-toggle" data-toggle="dropdown">
			<span class="hidden-xs">
                <tr>
                    <td><%=U5004.MAIN%></td>
                    <td>
                        <asp:Label ID="MainBalanceLabel" runat="server"></asp:Label>
                    </td>
                </tr>
			</span> <b class="caret"></b>
		</a>
		<ul class="dropdown-menu animated fadeInLeft">
			<li class="arrow"></li>
			<li>
                <tr>
                    <td><%=U6012.PURCHASE %></td>
                    <td >
                        <asp:Label ID="AdBalanceLabel" runat="server"></asp:Label>
                    </td>
                </tr>
			</li>
            <li>
                <tr id="CommissionBalancePlaceHolder" runat="server" visible="false">
                    <td><%=U5004.COMMISSION %></td>
                    <td>
                        <asp:Label ID="CommissionBalanceLabel" ClientIDMode="Static" runat="server"></asp:Label>
                    </td>
                </tr>
            </li>
            <li>
                <tr id="CashBalancePlaceHolder" runat="server" visible="false">
                    <td><%=U5008.CASHBALANCE %></td>
                    <td>
                        <asp:Label ID="CashBalanceLabel" ClientIDMode="Static" runat="server"></asp:Label>
                    </td>
                </tr>
            </li>
            <li>
                <tr id="trafficBalanceTrId" runat="server">
                    <td><%=U5004.TRAFFIC %></td>
                    <td>
                        <asp:Label ID="TrafficBalanceLabel" runat="server"></asp:Label></td>
                </tr>
            </li>
            <li>
                <tr id="PointsBalanceRow" runat="server">
                    <td><%=AppSettings.PointsName %></td>
                    <td>
                        <asp:Label ID="MemberBalancesControlPointsLabel" ClientIDMode="Static" runat="server"></asp:Label>
                    </td>
                </tr>
            </li>
            <li>
                <tr id="AdCreditsBalanceTrId" runat="server" visible="false">
                    <td><%=U5006.ADCREDITS %></td>
                    <td>
                        <asp:Label ID="AdCreditsBalanceLabel" runat="server"></asp:Label></td>
                </tr>
            </li>
			<li>Inbox</li>
			<li>Calendar</li>
			<li>Setting</li>
			<li class="divider"></li>
			<li>Log Out</li>
		</ul>
    </table>
	</li>
</ul>
