<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="products.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">

    <script type="text/javascript" src="Scripts/gridview.js"></script>

</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header">Porfolio products</h1>

    <div class="tab-content">
        <%-- SUBPAGE START --%>

        <div class="row">
            <div class="col-md-12">
                <asp:GridView ID="AuctionGrid1" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" PagerSettings-PageButtonCount="7"
                    DataSourceID="AuctionGrid1_DataSource" PageSize="25" OnRowDataBound="AuctionGrid1_RowDataBound">
                    <Columns>

                        <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone"></asp:BoundField>
                        <asp:BoundField DataField="Name" HeaderText='Name' SortExpression="Name"></asp:BoundField>
                        <asp:BoundField DataField="Description" HeaderText='Description' SortExpression="Description" />
                        <asp:BoundField DataField="CostToBuild" HeaderText='Cost' SortExpression="CostToBuild" />
                        <asp:BoundField DataField="TotalShares" HeaderText='Total shares' SortExpression="TotalShares" />
                    </Columns>
                </asp:GridView>



                <asp:SqlDataSource ID="AuctionGrid1_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
                    OnInit="AuctionGrid1_DataSource_Init" />


                <asp:Label ID="UserName" runat="server" Visible="false"></asp:Label>
            </div>
        </div>

    </div>

</asp:Content>
