<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="receipts.aspx.cs" Inherits="Receipts" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
<script>
    function pageLoad() {
        <%=PageScriptGenerator.GetGridViewCode(HistoryGridView) %>
    }
</script>
</asp:Content>



<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <h1 class="page-header"><%= U5008.RECEIPTS %></h1>

    <div class="row">
        <div class="col-md-12">
            <p class="lead">
                <%=U5008.RECEIPTSINFO %>    
            </p>
        </div>
    </div>

    <div class="tab-content">
        <div class="row">
            <div class="col-md-12">
                <asp:GridView ID="HistoryGridView" runat="server" AllowSorting="False" AutoGenerateColumns="False" DataSourceID="HistorySqlDataSource"
                    PageSize="20" AllowPaging="true" OnRowCommand="HistoryGridView_RowCommand" 
                    OnPreRender="BaseGridView_PreRender" OnDataBound="HistoryGridView_DataBound">
                    <Columns>
                        <asp:BoundField SortExpression="Id" DataField="Id" />
                        <asp:TemplateField HeaderText='<%$ ResourceLookup : DATE %>' SortExpression='DateAdded'>
                            <ItemTemplate>
                                <%# ((DateTime)Eval("DateAdded")).ToString() %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField SortExpression="Description" DataField="Description" HeaderText='<%$ ResourceLookup : DESCRIPTION %>' />
                        <asp:BoundField SortExpression="Quantity" DataField="Quantity"/>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <%# Money.Parse(Eval("UnitPrice").ToString()) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField SortExpression='Tax'>
                            <ItemTemplate>
                                <%# Eval("Tax").ToString() %> %
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <%# new PurchasedItem((int)Eval("Id")).GetTotalValue() %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="">
                            <ItemStyle />
                            <ItemTemplate>
                                <asp:LinkButton runat="server"
                                    CommandName="download"
                                    CommandArgument='<%# Container.DataItemIndex %>'>
                                    <span class="fa fa-arrow-right fa-lg text-info"></span>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

                <asp:SqlDataSource ID="HistorySqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="HistorySqlDataSource_Init"></asp:SqlDataSource>


            </div>
        </div>
    </div>

    
</asp:Content>
