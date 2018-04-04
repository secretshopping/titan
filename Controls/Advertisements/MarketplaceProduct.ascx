<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MarketplaceProduct.ascx.cs" Inherits="Controls_MarketplaceProduct" %>

<div class="col-md-3 col-sm-4 col-xs-12">
    <div class="item p-20">

        <div class="item-image text-center">
            <asp:Image ID="Image" ImageUrl="<%#ImageURL %>" Style="height: 125px; max-width: 125px;" runat="server" BorderStyle="Solid" />
        </div>

        <h5><%=Title %></h5>

     
        <div class="item-description">
            <p><%=Description %></p>
        </div>

        <%--<input type="number" min="1" max="<%=Quantity %>" value="1" step="1" style="width: 35px;"> / <span style="font-weight: bold;"><%=Quantity %></span> 
        <br />--%>
        <asp:Button ID="btn" runat="server" OnClick="ProductInfo_Click" CssClass="btn btn-inverse btn-block"
            OnClientClick="this.disabled = true; this.className = 'rbutton-loading'; this.value='';"
            UseSubmitBehavior="false"
            CommandArgument="<%#Object.Id%>" />
    </div>
</div>
