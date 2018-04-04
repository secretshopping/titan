<%@ Page Language="C#" AutoEventWireup="true" CodeFile="history.aspx.cs" Inherits="user_internalexchange_history" MasterPageFile="~/User.master" %>

<%@ MasterType VirtualPath="~/User.master" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=U6012.INTERNALEXCHANGE %></h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=U6012.INTERNALEXCHANGEHISTORYDESCRIPTION %></p>
        </div>
    </div>
    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <div class="TitanViewPage">
                    <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                        <asp:Button ID="MainTab" runat="server" CommandArgument="0" CssClass="ViewSelected" />
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>
    <div class="tab-content">
        <h4><%=String.Format("{0} {1}", L1.HISTORY, U6012.TRANSACTIONS)  %> </h4>
        <asp:GridView ID="TransactionHistoryGridView" DataKeyNames='<%# new string[] { "TransactionId", } %>'
            AllowPaging="true" AllowSorting="true" runat="server" PageSize="10"
            DataSourceID="TransactionHistoryGridView_DataSource"
            OnRowDataBound="TransactionHistoryGridView_RowDataBound">
            <Columns>
                <asp:BoundField HeaderText="Id" DataField="TransactionId" ItemStyle-CssClass="displaynone" ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                <asp:BoundField HeaderText="Type" DataField="IsAsk" />
                <asp:BoundField HeaderText="Stock Amount" DataField="TransactionAmount" />
                <asp:BoundField HeaderText="Stock Value" DataField="TransactionValue" />
                <asp:TemplateField HeaderText="Value of transaction (w/o fee)" />
                <asp:BoundField HeaderText="TransactionDate" DataField="TransactionDate" />
                <asp:BoundField HeaderText="BidUserId" DataField="BidUserId" ItemStyle-CssClass="displaynone" ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
            </Columns>
        </asp:GridView>
        <asp:SqlDataSource ID="TransactionHistoryGridView_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
            OnInit="TransactionHistoryGridView_DataSource_Init" />
    </div>
    <br />

</asp:Content>
