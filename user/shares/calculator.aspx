<%@ Page Language="C#" AutoEventWireup="true" CodeFile="calculator.aspx.cs" Inherits="user_calculator" MasterPageFile="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=U6007.CALCULATOR %></h1>
    <div class="row">
        <div class="col-md-12">
            <p id="MainDescriptionP" runat="server" class="lead" />
        </div>
    </div>
    <titan:AdPacksCalculator runat="server" />
</asp:Content>
