<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PtcOfferWall.ascx.cs" Inherits="Controls_Advertisements_PtcOfferWall" %>

<div class="offer-wall" title="<%=HoverHintText %>">
    <div class="image-info" style="position: relative;">
        <input type="hidden" value="<%=OfferWall.Id %>" id="offer-id"/>
        <input type="hidden" value="<%=OfferWall.Title %>" id="offer-title"/>
        <p class="title"><strong><%=OfferWall.Title %></strong></p>

        <div class="desc">
            <p><small><%=OfferWall.Description %></small></p>
        </div>
    </div>

</div>
