<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StartSurfingAdPack.ascx.cs" Inherits="Controls_StartSurfingAdPack" %>

<asp:PlaceHolder ID="ButtonPlaceholder" runat="server">

        <div class="text-center">
            <button onclick="window.open('user/earn/surf.aspx?f=2&auto=1')" class="btn btn-inverse"><%=Resources.U4200.STARTSURFING %></button>
        </div>

</asp:PlaceHolder>
