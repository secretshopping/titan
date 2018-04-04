<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="rotatorlink.aspx.cs" Inherits="RotatorLink" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <!--Slider-->
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=U5007.ROTATORLINK %></h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=U5007.ROTATORLINKINFO %></p>
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
                                <asp:Button ID="Button2" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
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
                                    <p class="text-center"><span class="fa fa-refresh fa-5x"></span></p>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-12">
                                    <h2><%=L1.STATUS %></h2>

                                    <asp:PlaceHolder ID="ParticipatePlaceHolder" runat="server" Visible="false">
                                        <h2><%=L1.REFLINK %>:</h2>
                                        <p><asp:Label ID="RotatorLinkLiteral" Font-Underline="true" runat="server"></asp:Label></p>
                                        <h2><%=L1.EXPIRES %>:</h2>
                                        <p><asp:Label ID="RotatorExpiresLiteral" runat="server" Font-Italic="true"></asp:Label></p>
                                    </asp:PlaceHolder>

                                    <asp:PlaceHolder ID="NoParticipatePlaceHolder" runat="server">
                                        <p><%=U5007.NOLINK %></p>
                                    </asp:PlaceHolder>


                                    <h2><%=L1.STATISTICS %></h2>

                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=U5007.LINKOPENS %>:</label>
                                            <div class="col-md-6">
                                                <p class="form-control no-border"><asp:Label runat="server" ID="LinkOpensLabel" Font-Bold="true" Text="-"></asp:Label></p>
                                            </div>
                                        </div>
                                        <%--<div class="form-group">
                                            <label class="control-label col-md-4">Referrals gained</label>
                                            <div class="col-md-8">
                                                <p class="form-control no-border"><asp:Label runat="server" ID="ReferralsGainedLabel" Font-Bold="true" Text="-"></asp:Label></p>
                                            </div>
                                        </div>--%>
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=U5007.MEMBERSINPOOL %>:</label>
                                            <div class="col-md-6">
                                                <p class="form-control no-border"><asp:Label runat="server" ID="MembersInPoolLabel"></asp:Label></p>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                               
                        </div>
                    </asp:View>

                    <asp:View runat="server" ID="View2">
                        <div class="TitanViewElement">
                            <%-- SUBPAGE START --%>
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=L1.PRICE %></b>:</label>
                                            <div class="col-md-6">
                                                <p class="form-control no-border"><asp:Literal runat="server" ID="PriceLiteral"></asp:Literal></p>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="col-md-2">
                                                <asp:Button ID="ParticipateButton" runat="server"
                                                CssClass="btn btn-inverse btn-block" OnClick="ParticipateButton_Click"
                                                OnClientClick=""
                                                UseSubmitBehavior="false" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>                             

                    </asp:View>

                </asp:MultiView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
