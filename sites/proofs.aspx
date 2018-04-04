<%@ Page Language="C#" AutoEventWireup="true" CodeFile="proofs.aspx.cs" Inherits="sites_proofs" MasterPageFile="~/Sites.master" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <div class="content" data-scrollview="true">
        <div class="container" data-animation="true" data-animation-type="fadeInDown">
            <h2 class="content-title"><%=U6000.RECENTPAYMENTS %></h2>
            <asp:PlaceHolder runat="server" ID="PaymentProofsAdditionalInfoPlaceHolder">
                <p class="text-center"><%=L1.PROOFSINFO %></p>
            </asp:PlaceHolder>
            <div class="row m-t-30">
                <div class="col-md-12">
                    <div class="table-responsive">
                    <asp:GridView ID="PayoutProofsGridView" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                        DataSourceID="PayoutProofsGridView_DataSource" OnRowDataBound="PayoutProofsGridView_RowDataBound" DataKeyNames="Id"
                        PageSize="50" AllowPaging="true">
                        <Columns>

                            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" ControlStyle-CssClass="displaynone" />
                            <asp:BoundField DataField="Date" HeaderText="<%$ ResourceLookup : DATE %>" SortExpression="Date" />

                            <asp:BoundField DataField="UserId" HeaderText="<%$ ResourceLookup : COUNTRY %>" SortExpression="UserId" />

                            <asp:BoundField DataField="Type" HeaderText="<%$ ResourceLookup : TYPE %>" SortExpression="Type" />
                            <asp:BoundField DataField="Amount" HeaderText="<%$ ResourceLookup : AMOUNT %>" SortExpression="Amount" />

                            <asp:BoundField DataField="Processor" HeaderText="<%$ ResourceLookup : ACCOUNT %>" SortExpression="Processor" />
                        </Columns>
                    </asp:GridView>
                    <asp:SqlDataSource ID="PayoutProofsGridView_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" SelectCommand="SELECT * FROM [PaymentProofs] ORDER BY [Date] DESC"></asp:SqlDataSource>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
