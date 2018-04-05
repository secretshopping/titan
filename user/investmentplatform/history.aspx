<%@ Page Language="C#" AutoEventWireup="true" CodeFile="history.aspx.cs" Inherits="user_investmentplatform_history" MasterPageFile="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">

    <script>
        function pageLoad() {
        <%=PageScriptGenerator.GetGridViewCode(HistoryGridView) %>
        }
    </script>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <h1 class="page-header"><%= L1.HISTORY %></h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=U6006.INVESTMENTHISTORYDESCRIPTION %></p>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <asp:PlaceHolder runat="server" ID="HistoryGridViewPlaceHolder">
                <asp:GridView ID="HistoryGridView" runat="server" AllowSorting="False" AutoGenerateColumns="False" OnPreRender="BaseGridView_PreRender" DataSourceID="HistorySqlDataSource"
                    OnRowDataBound="HistoryGridView_RowDataBound" PageSize="40" AllowPaging="true" OnRowCommand="HistoryGridView_RowCommand">
                    <Columns>
                        <asp:BoundField DataField='Id' ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" />
                        <asp:BoundField DataField='PurchasedDate' HeaderText='<%$ ResourceLookup : DATE %>' SortExpression='PurchasedDate' />
                        <asp:BoundField DataField='PlanId' HeaderText='<%$ ResourceLookup : PLAN %>' SortExpression='PlanId' />
                        <asp:BoundField DataField='MoneyReturned' HeaderText='<%$ ResourceLookup : MONEYRETURNED %>' SortExpression='MoneyReturned' />
                        <asp:BoundField DataField='MoneyToReturn' HeaderText='<%$ ResourceLookup : MONEYTORETURN %>' SortExpression='MoneyToReturn' />
                        <asp:BoundField DataField='MoneyInSystem' HeaderText='<%$ ResourceLookup : MONEYINSYSTEM %>' SortExpression='MoneyInSystem' />
                        <asp:BoundField DataField='BalanceBoughtType' HeaderText='<%$ ResourceLookup : FROM %>' SortExpression='BalanceBoughtType' />
                        <asp:BoundField DataField='Status' HeaderText='<%$ ResourceLookup : STATUS %>' SortExpression='Status' />
                        <asp:TemplateField HeaderText="Proofs">
                            <ItemStyle />
                            <ItemTemplate>
                                <asp:LinkButton ID="DownloadButton" runat="server"
                                    ToolTip='Download Proof'
                                    CommandName="download"
                                    CommandArgument='<%# Container.DataItemIndex %>'>
                                        <span class="fa fa-download fa-lg text-success"></span>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:SqlDataSource ID="HistorySqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
                    OnInit="HistorySqlDataSource_Init"></asp:SqlDataSource>
            </asp:PlaceHolder>

            <asp:PlaceHolder runat="server" ID="TicketsGridViewPlaceHolder">
                <asp:GridView ID="TicketsGridView" runat="server" AllowSorting="False" AutoGenerateColumns="False" OnPreRender="BaseGridView_PreRender" DataSourceID="TicketsSqlDataSource"
                    OnRowDataBound="TicketsGridView_RowDataBound" PageSize="40" AllowPaging="true" OnRowCommand="TicketsGridView_RowCommand">
                    <Columns>
                        <asp:BoundField DataField='Id' ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" />
                        <asp:BoundField DataField='Date' HeaderText='<%$ ResourceLookup : DATE %>' SortExpression='Date' />
                        <asp:BoundField DataField='Level' HeaderText='<%$ ResourceLookup : LEVEL %>' SortExpression='PlanId' />
                        <asp:BoundField DataField='LevelPrice' HeaderText='<%$ ResourceLookup : PRICE %>' SortExpression='LevelPrice' />
                        <asp:BoundField DataField='LevelFee' HeaderText='<%$ ResourceLookup : FEE %>' SortExpression='LevelFee' />
                        <asp:BoundField DataField='LevelEarnings' HeaderText='<%$ ResourceLookup : EARNINGS %>' SortExpression='LevelEarnings' />
                        <asp:BoundField DataField='StatusInt' HeaderText='<%$ ResourceLookup : STATUS %>' SortExpression='StatusInt' />
                        <asp:TemplateField HeaderText="Proofs">
                            <ItemStyle />
                            <ItemTemplate>
                                <asp:LinkButton ID="DownloadButton" runat="server"
                                    ToolTip='Download Proof'
                                    CommandName="download"
                                    CommandArgument='<%# Container.DataItemIndex %>'>
                                        <span class="fa fa-download fa-lg text-success"></span>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:SqlDataSource ID="TicketsSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
                    OnInit="TicketsSqlDataSource_Init"></asp:SqlDataSource>
            </asp:PlaceHolder>
        </div>
    </div>

</asp:Content>
