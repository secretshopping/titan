<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ArticleLink.ascx.cs" Inherits="Controls_News_ArticleLink" %>

<asp:Placeholder ID="ArticlePlaceHolder" runat="server">
    <li>
        <a href="<%=Object.GetLink()%>">
            <%=Object.Title %>
        </a>
    </li>
</asp:Placeholder>
