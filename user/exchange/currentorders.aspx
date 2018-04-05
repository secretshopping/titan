<%@ Page Language="C#" AutoEventWireup="true" CodeFile="currentorders.aspx.cs" Inherits="user_internalexchange_currentorders" MasterPageFile="~/User.master" %>

<%@ MasterType VirtualPath="~/User.master" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=U6012.INTERNALEXCHANGE %></h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=U6012.CHECKCURRENTORDERS %></p>
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
        <asp:Panel ID="SuccessMessagePanel" runat="server" CssClass="alert alert-success fade in m-b-15">
            <asp:Literal ID="SuccessMessageLiteral" runat="server" />
        </asp:Panel>
        <asp:Panel ID="ErrorMessagePanel" runat="server" CssClass="alert alert-danger fade in m-b-15">
            <asp:Literal ID="ErrorMessageLiteral" runat="server" />
        </asp:Panel>

        <h4><%=String.Format("{0} ({1})", U6012.YOURACTIVEOFFERS, L1.BUY) %> </h4>
        <asp:GridView ID="UserCurrentBidsGridView" DataKeyNames='<%# new string[] { "BidId", } %>'
            AllowPaging="true" AllowSorting="true" runat="server" PageSize="10"
            DataSourceID="UserCurrentBidsGridView_DataSource"
            OnRowDataBound="UserCurrentOfferGridView_RowDataBound"
            OnRowCommand="UserCurrentAsksAndBids_RowCommand">
            <Columns>
                <asp:BoundField HeaderText="Id" DataField="BidId" ItemStyle-CssClass="displaynone" ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                <asp:BoundField HeaderText="userID" DataField="BidUserId" ItemStyle-CssClass="displaynone" ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                <asp:BoundField HeaderText="<%$ ResourceLookup : AMOUNT %>" DataField="BidAmount" ItemStyle-CssClass="displaynone" ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                <asp:BoundField HeaderText="<%$ ResourceLookup : VALUEOFSTOCK %>" DataField="BidValue" />
                <asp:TemplateField HeaderText="<%$ ResourceLookup : FREEZEDVALUE %>" />
                <asp:TemplateField HeaderText="<%$ ResourceLookup : VOLUME %>" />
                <asp:BoundField HeaderText="<%$ ResourceLookup : CREATED %>" DataField="BidDate" />
                <asp:BoundField HeaderText="<%$ ResourceLookup : STATUS %>" DataField="BidStatus" />
                <asp:TemplateField HeaderText="">
                    <ItemStyle Width="13px" />
                    <ItemTemplate>
                        <asp:LinkButton ID="bidFinishOfferButton" runat="server" CssClass="btn btn-xs btn-inverse"
                            CommandName="finishBid"
                            CommandArgument='<%# Container.DataItemIndex %>'><span class="fa fa-remove"></span>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="">
                    <ItemStyle Width="13px" />
                    <ItemTemplate>
                        <asp:LinkButton ID="bidTradeOfferButton" runat="server" CssClass="btn btn-xs btn-inverse"
                            CommandName="tradeBid"
                            CommandArgument='<%# Container.DataItemIndex %>'><span class="fa fa-search"></span>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderText="<%$ ResourceLookup : AMOUNT %>" DataField="OriginalAmount" ItemStyle-CssClass="displaynone" ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:PlaceHolder ID="TradeDetailPlaceHolder" runat="server" Visible="false">
                        <tr><td colspan="999">
                            <asp:GridView ID="TradeDetailGridView" runat="server" AutoGenerateColumns="false" SkinID="-1" CssClass="table table-condensed"
                                OnRowDataBound="TradeDetailGridView_RowDataBound">
                                <Columns>
                                    <asp:BoundField HeaderText="<%$ ResourceLookup : TRANSACTION %>" DataField="Id" />
                                    <asp:BoundField HeaderText="<%$ ResourceLookup : DATE %>" DataField="TransactionDate" />
                                    <asp:BoundField HeaderText="<%$ ResourceLookup : AMOUNT %>" DataField="TransactionAmount"/>
                                    <asp:BoundField HeaderText="<%$ ResourceLookup : VALUEOFSTOCK %>" DataField="TransactionValue" />
                                    <asp:TemplateField HeaderText="<%$ ResourceLookup : VOLUME %>" />
                                    <asp:BoundField HeaderText="<%$ ResourceLookup : FEE %>" DataField="BidFee" />
                                </Columns>
                                <EmptyDataTemplate>
                                    <%=L1.NODATA %>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </td></tr>
                        </asp:PlaceHolder>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:SqlDataSource ID="UserCurrentBidsGridView_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
            OnInit="UserCurrentBidsGridView_DataSource_Init" />

        <br />
        <br />

        <h4><%=String.Format("{0} ({1})", U6012.YOURACTIVEOFFERS, U6009.SELL) %> </h4>
        <asp:GridView ID="UserCurrentAsksGridView" DataKeyNames='<%# new string[] { "AskId", } %>'
            AllowPaging="true" AllowSorting="true" runat="server" PageSize="10"
            DataSourceID="UserCurrentAsksGridView_DataSource"
            OnRowDataBound="UserCurrentOfferGridView_RowDataBound"
            OnRowCommand="UserCurrentAsksAndBids_RowCommand">
            <Columns>
                <asp:BoundField HeaderText="Id" DataField="AskId" ItemStyle-CssClass="displaynone" ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                <asp:BoundField HeaderText="userID" DataField="AskUserId" ItemStyle-CssClass="displaynone" ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                <asp:BoundField HeaderText="<%$ ResourceLookup : AMOUNT %>" DataField="AskAmount" ItemStyle-CssClass="displaynone" ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                <asp:BoundField HeaderText="<%$ ResourceLookup : VALUEOFSTOCK %>" DataField="AskValue" />
                <asp:TemplateField HeaderText="<%$ ResourceLookup : FREEZEDVALUE %>" />
                <asp:TemplateField HeaderText="<%$ ResourceLookup : VOLUME %>" />
                <asp:BoundField HeaderText="<%$ ResourceLookup : CREATED %>" DataField="AskDate" />
                <asp:BoundField HeaderText="<%$ ResourceLookup : STATUS %>" DataField="AskStatus" />
                <asp:TemplateField HeaderText="">
                    <ItemStyle Width="13px" />
                    <ItemTemplate>
                        <asp:LinkButton ID="askFinishOfferButton" runat="server" CssClass="btn btn-xs btn-inverse"
                            CommandName="finishAsk"
                            CommandArgument='<%# Container.DataItemIndex %>'><span class="fa fa-remove"></span>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="">
                    <ItemStyle Width="13px" />
                    <ItemTemplate>
                        <asp:LinkButton ID="askTradeOfferButton" runat="server" CssClass="btn btn-xs btn-inverse"
                            CommandName="tradeAsk"
                            CommandArgument='<%# Container.DataItemIndex %>'><span class="fa fa-search"></span>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderText="<%$ ResourceLookup : AMOUNT %>" DataField="OriginalAmount" ItemStyle-CssClass="displaynone" ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:PlaceHolder ID="TradeDetailPlaceHolder" runat="server" Visible="false">
                        <tr><td colspan="999">
                            <asp:GridView ID="TradeDetailGridView" runat="server" AutoGenerateColumns="false" SkinID="-1" CssClass="table table-condensed"
                                OnRowDataBound="TradeDetailGridView_RowDataBound">
                                <Columns>
                                    <asp:BoundField HeaderText="<%$ ResourceLookup : TRANSACTION %>" DataField="Id" />
                                    <asp:BoundField HeaderText="<%$ ResourceLookup : DATE %>" DataField="TransactionDate" />
                                    <asp:BoundField HeaderText="<%$ ResourceLookup : AMOUNT %>" DataField="TransactionAmount"/>
                                    <asp:BoundField HeaderText="<%$ ResourceLookup : VALUEOFSTOCK %>" DataField="TransactionValue" />
                                    <asp:TemplateField HeaderText="<%$ ResourceLookup : VOLUME %>" />
                                    <asp:BoundField HeaderText="<%$ ResourceLookup : FEE %>" DataField="AskFee" />
                                </Columns>
                                <EmptyDataTemplate>
                                    <%=L1.NODATA %>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </td></tr>
                        </asp:PlaceHolder>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:SqlDataSource ID="UserCurrentAsksGridView_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
            OnInit="UserCurrentAsksGridView_DataSource_Init" />

    </div>
</asp:Content>
