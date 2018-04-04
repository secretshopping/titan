<%@ Page Language="C#" AutoEventWireup="true" CodeFile="history.aspx.cs" Inherits="user_ico_history" MasterPageFile="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">

    <script>
        function pageLoad() {
        <%=PageScriptGenerator.GetGridViewCode(PurchasesGridView) %>
        }
    </script>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=L1.HISTORY %></h1>
    <div class="row">
        <div class="col-md-12">
            <p id="MainDescriptionP" runat="server" class="lead">
                <asp:Literal ID="MainDescriptionLiteral" runat="server" />
            </p>
        </div>
    </div>

    <div class="tab-content">


        <asp:UpdatePanel ID="UpdatePanel" runat="server">
            <ContentTemplate>

                <div class="row">
                    <div class="col-md-12">
                        <asp:GridView ID="PurchasesGridView" runat="server" AllowSorting="False" AutoGenerateColumns="False" OnPreRender="BaseGridView_PreRender" DataSourceID="PurchasesGridViewSqlDataSource"
                            OnRowDataBound="PurchasesGridView_RowDataBound" PageSize="40" AllowPaging="true">
                            <Columns>
                                <asp:BoundField DataField='Id' SortExpression='Id' ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" />
                                <asp:BoundField DataField='ICOStageId' HeaderText='Stage' SortExpression='ICOStageId' />
                                <asp:BoundField DataField='PurchaseTime' HeaderText='Date' SortExpression='PurchaseTime' />
                                <asp:BoundField DataField='NumberOfTokens' HeaderText='Volume' SortExpression='NumberOfTokens' />
                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="PurchasesGridViewSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
                            OnInit="PurchasesGridViewSqlDataSource_Init"></asp:SqlDataSource>
                    </div>
                </div>

            </ContentTemplate>
        </asp:UpdatePanel>

    </div>
</asp:Content>
