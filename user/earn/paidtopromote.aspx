<%@ Page Language="C#" AutoEventWireup="true" CodeFile="paidtopromote.aspx.cs" Inherits="user_earn_paidtopromote" MasterPageFile="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <!--Slider-->
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=U6009.PAIDTOPROMOTE %></h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=U6009.PAIDTOPROMOTEEARNDESCRIPTION %></p>
        </div>
    </div>

    <asp:UpdatePanel ID="MultiViewUpdatePanel" runat="server" UpdateMode="Conditional">

        <ContentTemplate>

            <div class="row">
                <div class="col-md-12">
                    <asp:Panel ID="SPanel" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
                        <asp:Literal ID="SText" runat="server"></asp:Literal>
                    </asp:Panel>

                    <asp:Panel ID="EPanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                        <asp:Literal ID="EText" runat="server"></asp:Literal>
                    </asp:Panel>
                </div>
            </div>

            <div class="row">
                <div class="col-md-12">
                    <div class="nav nav-tabs custom text-right">
                        <div class="TitanViewPage">
                            <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                                <asp:Button ID="Button1" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </div>

            <div class="tab-content">
                <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
                    <asp:View runat="server" ID="View1">
                        <div class="TitanViewElement">
                            <%-- SUBPAGE START --%>
                            <div class="row">
                                <div class="col-md-12">
                                    <h2><%=L1.STATUS %>:</h2>

                                    <asp:PlaceHolder ID="InactiveUserPlaceHolder" runat="server" Visible="false">
                                        <asp:Button runat="server" ID="GetLinkButton" OnClick="GetLinkButton_Click" />
                                    </asp:PlaceHolder>

                                    <asp:PlaceHolder ID="ActiveUserPlaceHolder" runat="server" Visible="false">
                                        <h2><%=L1.REFLINK %>:</h2>
                                        <p>
                                            <asp:Label ID="RotatorLinkLiteral" Font-Underline="true" runat="server"></asp:Label></p>

                                        <h2><%=L1.STATISTICS %>:</h2>
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <label class="control-label col-md-2"><%=U5007.LINKOPENS %>:</label>
                                                <div class="col-md-6">
                                                    <p class="form-control no-border">
                                                        <asp:Label runat="server" ID="LinkOpensLabel" Font-Bold="true" Text="-"></asp:Label></p>
                                                </div>
                                            </div>
                                            <%--<div class="form-group">
                                                <label class="control-label col-md-2">Referrals gained</label>
                                                <div class="col-md-8">
                                                    <p class="form-control no-border">
                                                        <asp:Label runat="server" ID="ReferralsGainedLabel" Font-Bold="true" Text="-"></asp:Label></p>
                                                </div>
                                            </div>--%>
                                        </div>
                                    </asp:PlaceHolder>
                                </div>
                            </div>

                        </div>
                    </asp:View>
                </asp:MultiView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
