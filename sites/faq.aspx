<%@ Page Language="C#" AutoEventWireup="true" CodeFile="faq.aspx.cs" Inherits="sites_faq" MasterPageFile="~/Sites.master" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <div class="content" data-scrollview="true">
        <div class="container" data-animation="true" data-animation-type="fadeInDown">
            <h2 class="content-title"><%=L1.FAQ %></h2>
            <div class="row">
                <div class="col-md-12">
                    <asp:Literal ID="Literal3" runat="server"></asp:Literal>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
