<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AdPacksAdvert.ascx.cs" Inherits="Controls_AdPacksAdvert" %>

<%--

    This is template for AdPacksAdvert. Feel free to modify it, but do NOT change:
    1. Any data between <% %> tags
    2. Any ASP.NET tags (e.g. <asp:PlaceHolder></asp:PlaceHolder>)

--%>




<%--Active advertisement--%>

<asp:PlaceHolder ID="ActiveAdvertPlaceHolder" runat="server" Visible="false">

    <div class="image AboxActive">
        <div class="image-info" style="border-left-color: <%=AdColor %>"> 
            <h5 class="title"><%=Title %></h5>
            <input type="hidden" value="<%=Object.Id %>" />
            <div class="desc">
                <%=Description %>    
            </div>
        </div>
    </div>

</asp:PlaceHolder>




<%--Inactive (grey) advertisement--%>

<asp:PlaceHolder ID="InactiveAdvertPlaceHolder" runat="server" Visible="false">

    <div class="image AboxClicked">
        <div class="image-info"> 
            <h5 class="title"><%=Title %></h5>
            <div class="desc">
                <%=Description %>    
            </div>
        </div>
    </div>

</asp:PlaceHolder>

