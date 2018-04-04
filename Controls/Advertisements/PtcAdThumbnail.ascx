<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PtcAdThumbnail.ascx.cs" Inherits="Controls_PtcAdThumbnail" %>

<%--

    This is template for AdPacksAdvert. Feel free to modify it, but do NOT change:
    1. Any data between <% %> tags
    2. Any ASP.NET tags (e.g. <asp:PlaceHolder></asp:PlaceHolder>)

--%>




<%--CurrentlyWatched advertisement--%>

<asp:PlaceHolder ID="CurrentlyWatchedPlaceHolder" runat="server" Visible="false">
    <div class="surfThumbnail surfThumbnailBeingWatched">
        <span style="color:white;"><%=Title %></span><br />
        <span style="color:#E6E6E6;"><i><%=L1.TIME %>: <%=Time %></i></span><br />

        <input type="hidden" value="<%=Object.Id %>" />
    </div>
</asp:PlaceHolder>




<%--NotCurrentlyWatched advertisement--%>

<asp:PlaceHolder ID="NotCurrentlyWatchedPlaceHolder" runat="server" Visible="false">

    <div class="surfThumbnailArrow">
        <img src="Images\Misc\arrow2.png" style="width:16px; height:18px" />
    </div>
    <div class="surfThumbnail">
         <span style="color:#5D5D5D;"><%=Title %></span><br />
        <i><%=L1.TIME %>: <%=Time %></i><br />

        <input type="hidden" value="<%=Object.Id %>" />
    </div>


</asp:PlaceHolder>

