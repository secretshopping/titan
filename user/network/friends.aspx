<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true" CodeFile="friends.aspx.cs" Inherits="Friends" %>

<%@ Import Namespace="SocialNetwork" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <asp:UpdatePanel runat="server" ID="ProfilePanel">
        <ContentTemplate>
            <asp:Literal runat="server" ID="ELiteral"></asp:Literal>
            <asp:Literal runat="server" ID="SLiteral"></asp:Literal>
            <h1 class="page-header"><%=L1.FRIENDS %></h1>
            <div class="row">
                <div class="col-md-12">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="nav nav-tabs custom text-right">
                                <asp:Panel ID="TitanViewPagePanel" runat="server" CssClass="TitanViewPage">
                                    <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                                        <asp:Button ID="FriendButton" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                                        <asp:Button ID="RequestButton" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                                    </asp:PlaceHolder>
                                </asp:Panel>
                            </div>
                        </div>
                    </div>
                    <div class="tab-content">
                        <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
                            <asp:View runat="server" ID="RequestsView" OnActivate="FriendsView_Activate">
                                <div class="TitanViewElement">
                                    <asp:Panel ID="RequestsPanel" runat="server">
                                        <div class="row">
                                            <div class="MenuContentPanel m-b-40">
                                                <ul class="registered-users-list clearfix">
                                                    <asp:PlaceHolder runat="server" ID="FriendInfoPlaceHolder"></asp:PlaceHolder>
                                                </ul>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </div>
                            </asp:View>
                            <asp:View runat="server" ID="FriendsView" OnActivate="RequestsView_Activate">
                                <div class="TitanViewElement">
                                    <asp:Panel ID="FriendsPanel" runat="server">
                                        <div class="row">
                                            <div class="MenuContentPanel m-b-40">
                                                <asp:GridView ID="RequestsGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                                                    DataSourceID="RequestsGridViewDataSource" OnRowDataBound="RequestsGridView_RowDataBound" DataKeyNames="Id"
                                                    PageSize="20" OnRowCommand="RequestsGridView_RowCommand" OnDataBound="RequestsGridView_DataBound" OnPreRender="BaseGridView_PreRender">
                                                    <Columns>
                                                        <asp:BoundField DataField='Id' HeaderText='Id' SortExpression='Id' ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" />
                                                        <asp:BoundField DataField='SenderId' SortExpression='SenderId' />
                                                        <asp:TemplateField SortExpression="Status" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone">
                                                            <ItemTemplate><%#((FriendshipRequestStatus)Eval("Status")).ToString()%></ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField SortExpression="Accept">
                                                            <ItemStyle Width="13px" />
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="AcceptLinkButton" runat="server"
                                                                    ToolTip='<%$ ResourceLookup : ACCEPT %>'
                                                                    CommandName="accept"
                                                                    CommandArgument='<%# Container.DataItemIndex %>'>
                                                                <span class="fa fa-plus text-success"></span>      
                                                                </asp:LinkButton>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField SortExpression="Reject">
                                                            <ItemStyle Width="13px" />
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="RejectLinkButton" runat="server"
                                                                    ToolTip='<%$ ResourceLookup : REJECTED %>'
                                                                    CommandName="reject"
                                                                    CommandArgument='<%# Container.DataItemIndex %>'>
                                                                <span class="fa fa-times text-success"></span>
                                                                </asp:LinkButton>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </div>
                            </asp:View>
                        </asp:MultiView>
                    </div>
                    <asp:SqlDataSource ID="RequestsGridViewDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="RequestsGridViewDataSource_Init"></asp:SqlDataSource>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
