<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ebooks.aspx.cs" Inherits="user_ebooks" MasterPageFile="~/User.master" %>

<%@ Import Namespace="Prem.PTC.Utils" %>

<asp:Content runat="server" ContentPlaceHolderID="PageMainContent">

    <h1 class="page-header"><%= U6004.EBOOKS%></h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%= U6004.EBOOKSDESC %></p>
        </div>
    </div>
    <div class="tab-content">
        <div class="TitanViewElement">
            <div class="row">
                <div class="col-md-12">
                    <div id="NoEBooksPanelWrapper" visible="false" runat="server" class="row m-t-15">
                        <div class="col-md-12">
                            <p class="alert alert-danger">
                                <asp:Literal ID="NoEBookspanel" Visible="false" runat="server" />
                            </p>
                        </div>
                    </div>
                    <div class="row">
                        <asp:PlaceHolder ID="EBooksLiteral" runat="server"></asp:PlaceHolder>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
