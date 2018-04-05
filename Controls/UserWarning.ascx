<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UserWarning.ascx.cs" Inherits="Controls_UserWarning" %>

<asp:PlaceHolder ID="UserWarningControl" runat="server" Visible="false">
    <div class="alert alert-warning m-b-0">
        <asp:Literal runat="server" ID="UserWarningControlText" />
    </div>
</asp:PlaceHolder>
