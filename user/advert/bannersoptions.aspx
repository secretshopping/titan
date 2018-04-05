<%@ Page Language="C#" AutoEventWireup="true" CodeFile="bannersoptions.aspx.cs" Inherits="user_advert_bannersoptions" MasterPageFile="~/User.master" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <div class="row m-t-20">
        <div class="col-md-12">
            <p class="lead text-center">
                <asp:Literal ID="SubLiteral" runat="server"></asp:Literal>
            </p>
        </div>
    </div>
    <div class="row m-t-20">
        <div class="col-md-6 text-center">
            <div class="form-group">
                <asp:LinkButton ID="CurrentWebsiteButton" runat="server" CssClass="btn btn-inverse btn-block" Width="140px" />
            </div>
        </div>
        <div class="col-md-6 text-center">
            <div class="form-group">
                <asp:LinkButton ID="OuterWebsiteButton" runat="server" CssClass="btn btn-inverse btn-block" Width="140px" />
            </div>
        </div>
    </div>
</asp:Content>
