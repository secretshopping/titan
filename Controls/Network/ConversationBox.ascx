<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ConversationBox.ascx.cs" Inherits="ConversationBox" %>

<li>
    <!-- begin media -->
    <div class="media">
        <img src="<%=ResolveUrl(OtherUser.AvatarUrl) %>" alt="">
    </div>
    <!-- end media -->
    <!-- begin info-container -->
    <div class="info-container">
        <div class="info">
            <h4 class="title"><%=OtherUser.Name %> <asp:Literal ID="UnreadMessagesLiteral" runat="server"></asp:Literal></h4>
            <p class="info-start-end">
                <%=MessageText %>
            </p>
        </div>
    </div>
    <!-- end info-container -->
</li>

