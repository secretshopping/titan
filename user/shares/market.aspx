<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="market.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">

    <script type="text/javascript" src="Scripts/gridview.js"></script>
    <script type="text/javascript">
        function RefreshPrice() {   
            var price1 = parseFloat($('#<%=Price.ClientID%>').val());
            var price2 = parseInt($('#<%=UnitsBox.ClientID%>').val());

            $('#<%=TotalLiteral.ClientID%>').text(price1 * price2);
        }
    </script>
</asp:Content>



<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=U4000.MARKET %></h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=U4000.MARKETINFO %></p>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <asp:Panel ID="TitanViewPagePanel" runat="server" CssClass="TitanViewPage">
                    <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                        <asp:Button ID="Button2" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
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
                        <asp:Panel ID="SPanel" runat="server" Visible="false" CssClass="alert alert-success">
                            <asp:Literal ID="SText" runat="server"></asp:Literal>
                        </asp:Panel>

                        <asp:Panel ID="EPanel" runat="server" Visible="false" CssClass="alert alert-danger">
                            <asp:Literal ID="EText" runat="server"></asp:Literal>
                        </asp:Panel>
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-12">
                        <asp:Panel ID="AuctionsPanel" runat="server">

                            <asp:GridView ID="AuctionGrid1" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" OnRowCommand="AuctionGrid1_RowCommand"
                                DataSourceID="AuctionGrid1_DataSource" PageSize="25" OnRowDataBound="AuctionGrid1_RowDataBound">
                                <Columns>

                                    <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone"></asp:BoundField>
                                    <asp:BoundField DataField="DateStarted" HeaderText='<%$ ResourceLookup: DATE %>' SortExpression="DateStarted" DataFormatString="{0:d}"></asp:BoundField>
                                    <asp:BoundField DataField="Username" HeaderText='<%$ ResourceLookup : USERNAME %>' SortExpression="Username" />
                                    <asp:BoundField DataField="PortfolioProductId" HeaderText='<%$ ResourceLookup : PRODUCT %>' SortExpression="PortfolioProductId" />
                                    <asp:BoundField DataField="SharesToSell" HeaderText='<%$ ResourceLookup : UNITS %>' SortExpression="SharesToSell" />
                                    <asp:BoundField DataField="Price" HeaderText='<%$ ResourceLookup : PRICE %>' SortExpression="Price" />

                                    <asp:TemplateField HeaderText="#">
                                        <ItemStyle />
                                        <ItemTemplate>
                                            <asp:LinkButton ID="ImageButton1" runat="server"
                                                ToolTip='Buy'
                                                CommandName="buy"
                                                CommandArgument='<%# Container.DataItemIndex %>'>
                                                <span class="fa fa-play fa-lg text-success"></span>
                                            </asp:LinkButton>
                                            <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                                        </ItemTemplate>

                                    </asp:TemplateField>

                                </Columns>
                            </asp:GridView>


                        </asp:Panel>
                    </div>
                </div>

                <%-- SUBPAGE END   --%>
            </div>
        </asp:View>

        <asp:View ID="View1" runat="server">
            <div id="view1h" class="TitanViewElement">
                <%-- SUBPAGE START --%>

                <div class="row">
                    <div class="col-md-12">
                        <asp:Panel ID="SucPanel" runat="server" Visible="false" CssClass="alert alert-success">
                            <asp:Literal ID="SucMess" runat="server"></asp:Literal>
                        </asp:Panel>

                        <asp:Panel ID="ErrPanel" runat="server" Visible="false" CssClass="alert alert-danger">
                            <asp:Literal ID="ErrMess" runat="server"></asp:Literal>
                        </asp:Panel>
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-12">
                        <div class="form-horizontal">
                            <asp:Panel ID="SellingPanel" runat="server">
                                <div class="form-group">
                                    <label class="form-control col-md-2"><%=U4000.SHARES %>:</label>
                                    <div class="col-md-6">
                                        <asp:DropDownList ID="ddlOptions" runat="server" CssClass="styledselect form-control"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="form-control col-md-2"><%=U4000.UNITS %>:</label>
                                    <div class="col-md-6">
                                        <asp:TextBox ID="UnitsBox" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RegularExpressionValidator Display="Dynamic" CssClass="text-danger" ID="RegularExpressionValidator1" ValidationExpression="^[1-9][0-9]{0,}$" runat="server" ErrorMessage="RegularExpressionValidator" ControlToValidate="UnitsBox">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator Display="Dynamic" CssClass="text-danger" ID="RequiredFieldValidator1" runat="server" ControlToValidate="UnitsBox">*</asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="form-control col-md-2"><%=U4000.PRICEPERUNIT %>:</label>
                                    <div class="col-md-6">
                                        <%=Prem.PTC.AppSettings.Site.CurrencySign %><asp:TextBox ID="Price" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator Display="Dynamic" CssClass="text-danger" ID="RequiredFieldValidator2" runat="server" ControlToValidate="Price">*</asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="form-control col-md-2"><%=L1.PRICE %>:</label>
                                    <div class="col-md-6">
                                        <span class="form-control no-border"><%=Prem.PTC.AppSettings.Site.CurrencySign %><asp:Label ID="TotalLiteral" runat="server" Text="0"></asp:Label>
                                            <span class="fa fa-refresh fa-lg" onclick="RefreshPrice()"></span></span>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-md-3">
                                        <asp:Button ID="BidButton" runat="server"
                                            ValidationGroup="BidValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="BidButton_Click" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel ID="NoSharesPanel" runat="server" Visible="false">
                                <%=U4000.NOSHARES %>
                            </asp:Panel>
                        </div>
                    </div>
                </div>

                <%-- SUBPAGE END   --%>
            </div>
        </asp:View>
    </asp:MultiView>
    </div>


    <asp:SqlDataSource ID="AuctionGrid1_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
        OnInit="AuctionGrid1_DataSource_Init" />


    <asp:Label ID="UserName" runat="server" Visible="false"></asp:Label>

</asp:Content>
