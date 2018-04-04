<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StartSurfingPtcAd.ascx.cs" Inherits="Controls_StartSurfingPtcAd" %>

<asp:PlaceHolder runat="server" ID="AutosurfLeftThisMonthPlaceHolder">
    
    <p class="text-center">
        <asp:Literal runat="server" ID="AutosurfLeftThisMonthLiteral"></asp:Literal>
    </p>
</asp:PlaceHolder>
<asp:PlaceHolder ID="ButtonPlaceholder" runat="server">
    <div class="text-center">
        <button onclick="window.open('user/earn/surf.aspx?f=1&auto=1')" class="btn btn-inverse btn-lg"><%=U4200.STARTSURFING %></button>
    </div>
</asp:PlaceHolder>
