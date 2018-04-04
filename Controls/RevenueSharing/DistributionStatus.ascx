<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DistributionStatus.ascx.cs" Inherits="Controls_DistributionStatus" %>

<asp:PlaceHolder ID="ContainerPlaceHolder" runat="server" Visible="false">

    <div class="alert alert-info">
        <asp:Literal ID="StatusLiteral" runat="server"></asp:Literal>
    </div>

</asp:PlaceHolder>
