<%@ Page Language="C#" AutoEventWireup="true" CodeFile="info.aspx.cs" Inherits="user_ico_info" MasterPageFile="~/User.master" %>

<%@ MasterType VirtualPath="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=U6012.INFORMATIONS %></h1>


    <div class="tab-content">

            <div class="row">
                <div class="col-md-12">
                    <asp:Literal ID="DescriptionLiteral" runat="server" />
                </div>
            </div>
   </div>
</asp:Content>
