<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SearchResultArticle.ascx.cs" Inherits="Controls_SearchResultNews_Article" %>

<asp:Placeholder ID="ArticlePlaceHolder" runat="server">

    <div class="post-detail section-container">
        <div class="row">
            <div class="col-md-3 col-sm-4">
                <div class="post-image">
                    <a href="<%=Object.GetLink() %>">
                        <img src="<%=Object.ImageURL %>" alt="">
                    </a>
                </div>
            </div>
            <div class="col-md-9 col-sm-8">
                <h4 class="post-title">
                    <a href="<%=Object.GetLink() %>"><%=Object.Title %></a>
                </h4>
                <div class="post-by">
                    <%=U6013.POSTEDBY %> <a href="sites/profile.aspx?u=<%=Author.Name %>"><%=Author.FirstName %> <%=Author.SecondName %> (<%=Author.Name %>)</a> <span class="divider">|</span> <%=Object.CreatedDate.ToShortDateString() %> <span class="divider">|</span> <a href="#"><%=Category.Text %></a>
                </div>
                <div class="post-desc">
                    <p>
                        <%=Object.ShortDescription %> [...]
                    </p>
                </div>
            </div>
        </div>
        <div class="read-btn-container">
            <a href="<%=Object.GetLink() %>"><%=U6013.READMORE %> <i class="fa fa-angle-double-right"></i></a>
        </div>
    </div>
</asp:Placeholder>
