<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="s4dspackages.aspx.cs" Inherits="Page_advert_Adpacks" %>

<%@ MasterType VirtualPath="~/User.master" %>

<asp:Content ContentPlaceHolderID="PageHeadContent" runat="server">
    <link rel="stylesheet" href="Styles/WatchAds.css" type="text/css" />
    <script type="text/javascript" src="Scripts/gridview.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <h1 class="page-header">S4DS Packages</h1>
    <div class="row">
        <div class="col-md-12">
            <p id="MainDescriptionP" runat="server" class="lead" />
        </div>
    </div>

    <input type="hidden" name="__EVENTARGUMENT5" id="__EVENTARGUMENT5" value="" />

        <ContentTemplate>
            <div class="row">
                <div class="col-md-12">
                    <div class="nav nav-tabs custom text-right">
                        <div class="TitanViewPage">
                            <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                                <asp:Button ID="ActivePacksTab" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </div>

            <div class="tab-content">
                <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
                    <asp:View runat="server" ID="View4">
                        <div class="TitanViewElement">
                            <div class="row">
                                <div class="col-md-12">
                                    <asp:GridView ID="UserS4DSPacksGridView" runat="server" OnPreRender="BaseGridView_PreRender" AllowPaging="True" AllowSorting="True" 
                                        AutoGenerateColumns="False" EmptyDataText="No Packages to display"
                                        DataSourceID="UserS4DSPacksGridView_DataSource" 
                                        OnRowDataBound="UserS4DSPacksGridView_RowDataBound" 
                                        PageSize="20">
                                        <Columns>
                                            <asp:BoundField DataField='Id' HeaderText=' ' SortExpression='Id' ControlStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone"/>
                                            <asp:BoundField DataField='MoneyReturned' SortExpression='MoneyReturned' HeaderText="<%$ ResourceLookup:MONEYRETURNED %>" />
                                            <asp:BoundField DataField="PurchaseDate" SortExpression="PurchaseDate" HeaderText="Date" />
                                        </Columns>
                                    </asp:GridView>
                                    <asp:SqlDataSource ID="UserS4DSPacksGridView_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" 
                                        OnInit="UserS4DSPacksGridView_DataSource_Init"></asp:SqlDataSource>
                                </div>
                            </div>
                        </div>
                    </asp:View>
                </asp:MultiView>
            </div>
        </ContentTemplate>
</asp:Content>
