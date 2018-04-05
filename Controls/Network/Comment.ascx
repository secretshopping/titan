<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Comment.ascx.cs" Inherits="CommentControl" %>

<div class="info-container comment well p-10 m-t-5 m-b-5">
    <div runat="server" id="CommentAuthor" class="post-user pull-left m-r-20"></div>
    <asp:Panel ID="EPanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
        <asp:Label ID="ELabel" runat="server"></asp:Label>
    </asp:Panel>
    <asp:Panel ID="SPanel" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
        <asp:Label ID="SLabel" runat="server"></asp:Label>
    </asp:Panel>
    <div class="post-content p-b-5 m-b-0">
        <p runat="server" id="CommentText"></p>
    <asp:Image runat="server" CssClass="m-b-15" ID="CommentImage" />
    <asp:Button runat="server" CssClass="btn btn-danger btn-xs pull-right" ID="DeleteCommentButton" OnClick="DeleteCommentButton_Click" />
    </div>  
    <div runat="server" id="CommentTime" class="post-time"></div>
</div>

