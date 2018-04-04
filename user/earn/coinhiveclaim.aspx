<%@ Page Title="" Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true" CodeFile="coinhiveclaim.aspx.cs" Inherits="user_earn_coinhiveclaim" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="PageHeadContent" runat="Server">
    <style>
        .banner a {
            text-align: center;
        }

            .banner a img {
                margin: 0 auto;
            }

        .captcha .coinhive-captcha {
            text-align: center;
                float: left;
        }

        .btn-coinhive {
            line-height: 24px;
            margin: 20px;
        }
    </style>
</asp:Content>

<asp:Content ID="MenuContent" ContentPlaceHolderID="PageMenuContent" runat="Server">
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="PageMainContent" runat="Server">
    <h1 class="page-header">Coinhive <%=U4200.CLAIM.ToLower() %></h1>

    <div class="row">
        <div class="col-md-12">
            <p class="lead">
                <asp:Label ID="CoinhiveClaimDescription" runat="server" />
            </p>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <div class="TitanViewPage">
                    <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                        <asp:Button ID="MainTab" runat="server" CommandArgument="0" CssClass="ViewSelected" />
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>

    <div class="tab-content">
        <asp:Panel ID="SuccessMessagePanel" runat="server" CssClass="alert alert-success fade in m-b-15">
            <asp:Label ID="SuccessMessageLabel" runat="server" />
        </asp:Panel>
        <asp:Panel ID="ErrorMessagePanel" runat="server" CssClass="alert alert-danger fade in m-b-15">
            <asp:Label ID="ErrorMessageLabel" runat="server" />
        </asp:Panel>

        <div class="row">
            <div class="col-lg-2">
            </div>
            <div class="col-lg-8">
                <div class="row">
                    <div class="col-md-12 m-b-30">
                        <div class="banner">
                            <titan:Banner runat="server" BannerBidPosition="1" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12 m-b-30">
                        <div class="banner">
                            <titan:Banner runat="server" BannerBidPosition="2" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12 m-b-30">
                        <div class="captcha">
                            <titan:CaptchaClaim ID="CoinhiveClaim" runat="server" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-2">
            </div>
        </div>
    </div>
</asp:Content>

