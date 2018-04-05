<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="adpacks.aspx.cs" Inherits="AdPacks" %>

<%@ MasterType VirtualPath="~/User.master" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=AppSettings.RevShare.AdPack.AdPackNamePlural%> <%=L1.STATISTICS%></h1>
    
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=U5001.STATSDESCRIPTION %></p>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <asp:Panel ID="TitanViewPagePanel" runat="server" CssClass="TitanViewPage">
                    <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                        <asp:Button ID="Button1" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                        <asp:Button ID="Button2" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                    </asp:PlaceHolder>
                </asp:Panel>
            </div>
        </div>
    </div>

    <div class="tab-content">
        <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">\
            <asp:View runat="server" ID="View1">
                <div class="row">
                    <div class="col-md-12">
                        <div class="TitanViewElement">
                            <asp:GridView ID="AdPacksStatsGridView" runat="server" OnPreRender="BaseGridView_PreRender" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" EmptyDataText="<%$ ResourceLookup:NOSTATS %>"
                            DataSourceID="AdPacksStatsGridView_DataSource" OnRowDataBound="AdPacksStatsGridView_RowDataBound" PageSize="30">
                                <Columns>

                                    <%-- Index 0 --%>
                                    <asp:BoundField DataField='Id' SortExpression="Id" HeaderText="Id" />

                                    <%-- Index 1 --%>
                                    <asp:BoundField DataField='PurchaseDate' SortExpression="PurchaseDate" HeaderText="PurchaseDate" />

                                    <%-- Index 2, This is for the "position" as requested by SARDARYIFY --%>
                                    <asp:BoundField DataField='AdPackTypeId' HeaderText="AdPackTypeId" />

                                     <%-- Index 3 --%>
                                    <asp:BoundField DataField="Price" SortExpression="Price" HeaderText="Price" />

                                     <%-- Index 4 --%>
                                    <asp:BoundField DataField="MoneyReturned" SortExpression="MoneyReturned" HeaderText="MoneyReturned" />

                                    <%--  Index 5, This is the status of the adpack --%>
                                    <asp:BoundField DataField="Id" HeaderText="Id"  />

                                    <%--  Index 6, This is for calculating the status of the adpack together with MoneyReturned --%>
                                    <asp:BoundField DataField="MoneyToReturn" HeaderText="MoneyToReturn" />

                                </Columns>
                            </asp:GridView>
                            <asp:SqlDataSource ID="AdPacksStatsGridView_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="AdPacksStatsGridView_DataSource_Init"></asp:SqlDataSource>   
                        </div>
                    </div>
                </div>   
            </asp:View> 
            <asp:View runat="server" ID="View2">
                <div class="row">
                    <div class="col-md-12">
                        <div class="TitanViewElement">
                            <h3><%=U5001.TOTAL %>: <b><asp:Literal runat="server" ID="TotalLiteral"></asp:Literal></b></h3>
                            <titan:Statistics runat="server" StatType="Referrals_AdPacks" Width="700px" Height="400px" StatTitle="<%$ResourceLookup: ALLCREDITEDMONEY %>"></titan:Statistics>
                        </div>
                    </div>
                </div>   
            </asp:View>
        </asp:MultiView>
    </div>

</asp:Content>
