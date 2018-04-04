<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AdPackUserList.ascx.cs" Inherits="Controls_AdPackUserList" %>

<div class="panel-body">
    <div class="table-responsive">
        <asp:PlaceHolder ID="ContainerPlaceHolder" runat="server" Visible="false">
                <asp:Literal ID="TotalAdPacksLiteral" runat="server"></asp:Literal>
        </asp:PlaceHolder>
        <asp:Literal ID="NoDataLiteral" runat="server"></asp:Literal>
    </div>
</div>