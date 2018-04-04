<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShareableArticle.ascx.cs" Inherits="Controls_News_ShareableArticle" %>

<asp:PlaceHolder ID="ArticlePlaceHolder" runat="server">

    <div class="image isotope-item col-md-4" data-link="<%=Object.GetSharingLink(Member.CurrentId) %>">
        <div class="image-inner">
            <a href="<%=Object.GetLink() %>">
                <img src="<%=Object.GetImageURL() %>" alt="">
            </a>
            <p class="image-caption">
                <%=Object.Reads %>&nbsp;<%=U6012.READS.ToLower() %>
            </p>
        </div>
        <div class="image-info">
            <h5 class="title"><a href="<%=Object.GetLink() %>"><%=Object.Title %></a></h5>
            <div class="pull-right">
                <small><%=U6013.POSTEDBY %></small> <a href="javascript:;"><%=Author.FirstName %> <%=Author.SecondName %> (<%=Author.Name %>)</a>
            </div>
            <div class="rating">
                <small class="text-muted"><%=Object.CreatedDate %></small><br />
                <a href="javascript:;"><%=Category.Text %></a>
            </div>
            <div class="desc">
                <%=Object.ShortDescription %>
            </div>
            <asp:Button ID="GetShareLinkButton" CssClass="btn btn-inverse btn-block sharelink" runat="server" />
        </div>
    </div>

</asp:PlaceHolder>
