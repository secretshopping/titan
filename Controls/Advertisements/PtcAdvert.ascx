<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PtcAdvert.ascx.cs" Inherits="Controls_PtcAdvert" %>

<%--

    This is template for PtcAdvert. Feel free to modify it, but do NOT change:
    1. Any data between <% %> tags
    2. Any ASP.NET tags (e.g. <asp:PlaceHolder></asp:PlaceHolder>)

--%>

<%--Active advertisement--%>

<asp:PlaceHolder ID="ActiveAdvertPlaceHolder" runat="server" Visible="false">

    <div class="AboxActive Ad image <%=ImageClass %> gallery-group-<%=CategoryId %> <%=FavoriteCssClass %> <%=ExposureType %>" title="PTC">
        <%--<asp:Image ID="PtcImage" runat="server" />--%>
        <div class="ad-image-background" style="<%=ImageBackgroundStyle %>"></div>

        <div class="image-info" style="position: relative; border-left-color: <%=AdColor %>">
            <div style="position: absolute; right: 5px; top: 5px;">
                <span class="fa fa-star text-warning p-r-5" runat="server" id="starImg" visible="false"></span>
                <asp:LinkButton CssClass="starredAdImg" runat="server" ID="favoriteImg" Visible="false" OnClick="FavoriteAdsImageButton_Click">
                    <span class="fa fa-heart text-danger"></span>    
                </asp:LinkButton>
                <input type="hidden" value="<%=Object.Id %>" />
            </div>
            <h5 class="title"><%=Title %></h5>

            <div class="desc">
                <%=Description %>
            </div>
            <div class="ABinfo" id="info1" runat="server">
                 <%=Info %>
            </div>
        </div>

    </div>

</asp:PlaceHolder>




<%--Inactive (grey) advertisement--%>

<asp:PlaceHolder ID="InactiveAdvertPlaceHolder" runat="server" Visible="false">

    <div class="AboxClicked Ad image <%=ImageClass %> gallery-group-<%=CategoryId %> <%=FavoriteCssClass %>" data-target-url="<%=Object.TargetUrl %>">
        <%--<asp:Image ID="PtcImage2" runat="server" />--%>
        <div class="ad-image-background" style="<%=ImageBackgroundStyle %>"></div>
        <div class="image-info" style="position: relative; border-left-color: <%=AdColor %>">
            <div style="position: absolute; right: 5px; top: 5px;">
                <span class="fa fa-star text-warning p-r-5" runat="server" id="starImg2" visible="false"></span>
                <asp:LinkButton CssClass="starredAdImg" runat="server" ID="favoriteImg2" Visible="false" OnClick="FavoriteAdsImageButton_Click">
                    <span class="fa fa-heart text-danger"></span>    
                </asp:LinkButton>
            </div>
            <h5 class="title"><%=Title %></h5>

            <div class="desc">
                <%=Description %>
            </div>
            <div class="ABinfo" id="info2" runat="server">
                 <%=Info %>
            </div>
        </div>

    </div>

</asp:PlaceHolder>

<%-- Cash Links mode  --%>

<asp:PlaceHolder ID="CashLinkPlaceHolder" runat="server" Visible="false">

    <div class="Abox CashLinkBox col-md-6 col-lg-2" runat="server" id="CashLinkDiv">
        <div class="CashLinkTitle m-t-0" style="background-color:<%=AdColor %>; padding: 20px">
            <p>
                <%=Info %>
            </p>
            <p style="clear: both">
                <button type="button" onclick="doSubmit('<%=IsPreview? 0 : Object.Id %>', true); return false;" class="btn btn-success"><%=U4200.CLAIM %></button>
            </p>
        </div>
    </div>

</asp:PlaceHolder>

<asp:PlaceHolder ID="CashLinkPlaceHolderInActive" runat="server" Visible="false">

    <div class="Abox CashLinkBox col-md-6 col-lg-2 bg-silver">
        <div class="CashLinkTitle bg-silver m-t-0" style="padding: 20px">
            <p style="visibility: hidden">
                <%=Info %>
            </p>
            <p style="clear: both">
                <button type="button" class="AboxClicked btn btn-default" data-target-url="<%=Object.TargetUrl %>">Visit</button>
            </p>
        </div>
    </div>

</asp:PlaceHolder>