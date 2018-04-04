<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="units.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">

    <script type="text/javascript" src="Scripts/gridview.js"></script>

</asp:Content>





<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=U4000.PORTFOLIOUNITS %></h1>

    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <asp:Panel ID="TitanViewPagePanel" runat="server" CssClass="TitanViewPage">
                    <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                        <asp:Button ID="Button1" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                    </asp:PlaceHolder>
                </asp:Panel>
            </div>
        </div>
    </div>

    <div class="tab-content">
    <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
        <asp:View runat="server" ID="View2">
            <div class="TitanViewElement">
                <%-- SUBPAGE START --%>

                <div class="row">
                    <div class="col-md-12">
                        <asp:GridView ID="AuctionGrid1" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                            DataSourceID="AuctionGrid1_DataSource" Width="280px" PageSize="25" OnRowDataBound="AuctionGrid1_RowDataBound">
                            <Columns>

                                <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone"></asp:BoundField>
                                <asp:BoundField DataField="PortfolioProductId" HeaderText='Product' SortExpression="PortfolioProductId"></asp:BoundField>
                                <asp:BoundField DataField="Shares" HeaderText='Units' SortExpression="Shares" />
                                <asp:BoundField DataField="PortfolioProductId" HeaderText='%' SortExpression="PortfolioProductId" />

                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
    
                <div class="row">
                    <div class="col-md-12">
                        <a href="user/shares/market.aspx" class="mybutton"><span class="fa fa-bar-chart fa-lg m-r-10"></span><%=U5006.MARKETPLACE %></a>
                    </div>
                </div>    


                

                <%-- SUBPAGE END   --%>
            </div>
        </asp:View>

    </asp:MultiView>


    <asp:SqlDataSource ID="AuctionGrid1_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
        OnInit="AuctionGrid1_DataSource_Init" />

    <asp:Label ID="UserName" runat="server" Visible="false"></asp:Label>

    </div>

</asp:Content>
