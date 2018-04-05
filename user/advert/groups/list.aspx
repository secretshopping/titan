<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="list.aspx.cs" Inherits="About" %>

<%@ MasterType VirtualPath="~/User.master" %>


<asp:Content ContentPlaceHolderID="PageHeadContent" runat="server">
</asp:Content>


<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=U4200.GROUPS %></h1>

    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=U4200.GROUPSINFO %></p>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <div class="TitanViewPage">
                    <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                        <asp:Button ID="Button3" runat="server" OnClick="MenuButton_Click" CommandArgument="2" />
                        <asp:Button ID="Button2" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                        <asp:Button ID="Button1" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>


    <div class="tab-content">
    <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
        <asp:View runat="server" ID="View1" OnActivate="View1_Activate">
            <div class="TitanViewElement" style="margin-top: -1px">
                <br />
                <asp:UpdatePanel runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="OpenGroupsGridView" runat="server" AllowSorting="true" AutoGenerateColumns="false" AllowPaging="true" PageSize="5"
                            EmptyDataText="<%$ ResourceLookup:NOOPENGROUPS %>" OnRowDataBound="OpenGroupsGridView_RowDataBound" OnPreRender="OpenGroupsGridView_PreRender"
                            DataSourceID="OpenGridViewDataSource">
                            <Columns>
                                <asp:BoundField DataField="Creator" HeaderText="<%$ ResourceLookup:GROUPCREATOR %>" HeaderStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" />
                                <asp:BoundField DataField="Name" HeaderText="<%$ ResourceLookup:NAME %>" SortExpression="Name" />
                                <asp:BoundField DataField="Color" />
                                <asp:BoundField DataField="AdPacksAdded" HeaderText="AdPacks Added" SortExpression="Percentage" />
                                <asp:BoundField DataField="AdPacksLimit" />
                                <asp:BoundField DataField="Percentage" />
                                <asp:BoundField DataField="Accelerated" SortExpression="Accelerated" />
                                <asp:BoundField DataField="UCGID" HeaderText="Number of participants" />
                                <asp:BoundField SortExpression="RequiredLeft" HeaderText="<%$ ResourceLookup:PACKSREQUIREDTOCLOSE %>" />
                                <asp:BoundField DataField="CreatorName" HeaderText="<%$ ResourceLookup:GROUPCREATOR %>" SortExpression="CreatorName" />
                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="OpenGridViewDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="OpenGroupsGridView_Init"></asp:SqlDataSource>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </asp:View>
        <asp:View runat="server" ID="View2" OnActivate="View2_Activate">
            <div class="TitanViewElement" style="margin-top: -1px">
                <br />
                <asp:UpdatePanel runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="ClosedGroupsGridView" runat="server" AllowSorting="true" AutoGenerateColumns="false" AllowPaging="true" PageSize="5"
                            EmptyDataText="<%$ ResourceLookup:NOCLOSEDGROUPS %>" OnRowDataBound="ClosedGroupsGridView_RowDataBound" OnPreRender="ClosedGroupsGridView_PreRender"
                            DataSourceID="ClosedGroupsGridViewDataSource">
                            <Columns>
                                <asp:BoundField DataField="Creator" HeaderText="<%$ ResourceLookup:GROUPCREATOR %>" HeaderStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" />
                                <asp:BoundField DataField="Name" HeaderText="<%$ ResourceLookup:NAME %>" SortExpression="Name" />
                                <asp:BoundField DataField="Color" />
                                <asp:BoundField DataField="AdPacksAdded" HeaderText="<%$ ResourceLookup:PACKSADDED %>" SortExpression="Percentage" />
                                <asp:BoundField DataField="AdPacksLimit" />
                                <asp:BoundField DataField="Percentage" />
                                <asp:BoundField DataField="Accelerated" SortExpression="Accelerated" />
                                <asp:BoundField DataField="UCGID" HeaderText="Number of participants" />
                                <asp:BoundField DataField="CreatorName" HeaderText="<%$ ResourceLookup:GROUPCREATOR %>" SortExpression="CreatorName" />
                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="ClosedGroupsGridViewDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="ClosedGroupsGridViewDataSource_Init"></asp:SqlDataSource>

                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </asp:View>
        <asp:View runat="server" ID="View3" OnActivate="View3_Activate">
            <div class="TitanViewElement" style="margin-top: -1px">
                <br />
                <asp:UpdatePanel runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="ClosedGroupsGridView2" runat="server" AllowSorting="true" AutoGenerateColumns="false" AllowPaging="true" PageSize="5"
                            EmptyDataText="No groups" OnRowDataBound="ClosedGroupsGridView2_RowDataBound" OnPreRender="ClosedGroupsGridView2_PreRender"
                            DataSourceID="ClosedGroupsGridView2DataSource">
                            <Columns>
                                <asp:BoundField DataField="Creator" HeaderText="<%$ ResourceLookup:GROUPCREATOR %>" HeaderStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" />
                                <asp:BoundField DataField="Name" HeaderText="<%$ ResourceLookup:NAME %>" SortExpression="Name" />
                                <asp:BoundField DataField="Color" />
                                <asp:BoundField DataField="AdPacksAdded" HeaderText="<%$ ResourceLookup:PACKSADDED %>" SortExpression="Percentage" />
                                <asp:BoundField DataField="AdPacksLimit" />
                                <asp:BoundField DataField="Percentage" />
                                <asp:BoundField DataField="Accelerated" SortExpression="Accelerated" />
                                <asp:BoundField DataField="UCGID" HeaderText="Number of participants" />
                                <asp:BoundField DataField="CreatorName" HeaderText="<%$ ResourceLookup:GROUPCREATOR %>" SortExpression="CreatorName" />
                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="ClosedGroupsGridView2DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="ClosedGroupsGridView2DataSource_Init"></asp:SqlDataSource>

                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </asp:View>
    </asp:MultiView>
    <div class="row">
        <div class="col-md-2">
            <asp:Button runat="server" ID="CreateGroupButton" OnClick="CreateGroupButton_Click" CssClass="btn btn-inverse btn-block" />
        </div>
    </div>
    </div>

    
    <%--        </ContentTemplate>
    </asp:UpdatePanel>--%>
</asp:Content>
