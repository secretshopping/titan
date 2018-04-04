<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BannerAuctionLink.ascx.cs" Inherits="Controls_BannerAuctionLink" %>

<asp:PlaceHolder ID="ControlPlaceHolder" runat="server">
    <div class="bannerlink" style="<%=Style%>">

        <b><%=Resources.U4000.ADVERTISEHERE %></b><br />
        <%=Resources.U4000.HOURLYAUCTIONS %><br />
        <i>
            <asp:Literal ID="PriceLiteral" runat="server"></asp:Literal>
        </i>
        <br />
        <a href="user/advert/bannersb.aspx" class="mybutton2"><%=Resources.U4000.PLACEBID %></a>

    </div>
</asp:PlaceHolder>
