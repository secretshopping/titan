<%@ Page Language="C#" MasterPageFile="~/Sites.master" AutoEventWireup="true"
    CodeFile="adblock.aspx.cs" Inherits="About" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="content" data-scrollview="true">
        <div class="container" data-animation="true" data-animation-type="fadeInDown">
            <h2 class="content-title"><%=U4200.DISABLEADBLOCK %></h2>
            <p class="text-center"><%=U4200.DISABLEADBLOCKINFO.Replace("%n%", AppSettings.Site.Name) %></p>
        </div>
    </div>

</asp:Content>
