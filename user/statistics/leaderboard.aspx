<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="leaderboard.aspx.cs" Inherits="Leaderboard" %>

<%@ MasterType VirtualPath="~/User.master" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <div class="row">
         <div class="col-lg-6 col-md-6 col-sm-6 leaderboard">
            <div class="panel panel-success">
                <div class="panel-heading">
                    <div class="panel-heading-btn">
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-default" data-click="panel-expand"><i class="fa fa-expand"></i></a>
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-success" data-click="panel-reload"><i class="fa fa-repeat"></i></a>
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-warning" data-click="panel-collapse"><i class="fa fa-minus"></i></a>
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-danger" data-click="panel-remove"><i class="fa fa-times"></i></a>
                    </div>
                    <h4 class="panel-title"><%=string.Format(U6000.TOPEARNERS, RecordsShown) %> </h4>
                </div>
                <div class="panel-body">
                    <asp:Panel runat="server" ID="TotalEarnedPanel" ClientIDMode="Static">
                        <div class="table-responsive">
                            <asp:GridView runat="server" ID="TotalEarnedGridView" DataSourceID="TotalEarnedGridView_DataSource"
                                OnDataBound="TotalEarnedGridView_DataBound" OnPreRender="BaseGridView_PreRender">
                                <Columns>
                                    <asp:BoundField DataField="Username" SortExpression="Username" />
                                    <asp:TemplateField SortExpression="TotalEarned">
                                        <ItemTemplate>
                                            <%#new Money((decimal)Eval("TotalEarned")).ToString() %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:SqlDataSource runat="server" ID="TotalEarnedGridView_DataSource" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
                                OnInit="TotalEarnedGridView_DataSource_Init"></asp:SqlDataSource>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </div>
        <div class="col-lg-6 col-md-6 col-sm-6 leaderboard">
            <div class="panel panel-warning">
                <div class="panel-heading">
                    <div class="panel-heading-btn">
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-default" data-click="panel-expand"><i class="fa fa-expand"></i></a>
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-success" data-click="panel-reload"><i class="fa fa-repeat"></i></a>
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-warning" data-click="panel-collapse"><i class="fa fa-minus"></i></a>
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-danger" data-click="panel-remove"><i class="fa fa-times"></i></a>
                    </div>
                    <h4 class="panel-title"><%=string.Format(U6000.TOPCOUNTRIES, RecordsShown) %> </h4>
                </div>
                <div class="panel-body">
                    <asp:Panel runat="server" ID="CountryPanel" ClientIDMode="Static">
                        <div class="table-responsive">
                            <asp:GridView runat="server" ID="CountryGridView" DataSourceID="CountryGridView_DataSource"
                                OnDataBound="CountryGridView_DataBound" OnPreRender="BaseGridView_PreRender">
                                <Columns>
                                    <asp:BoundField DataField="Country" SortExpression="Country" />
                                    <asp:BoundField DataField="CountryCount" SortExpression="CountryCount" />
                                </Columns>
                            </asp:GridView>
                            <asp:SqlDataSource runat="server" ID="CountryGridView_DataSource" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
                                OnInit="CountryGridView_DataSource_Init"></asp:SqlDataSource>
                        </div>
                    </asp:Panel>
                </div>
        </div>
    </div>
        <div class="col-lg-6 col-md-6 col-sm-6 leaderboard">
            <div class="panel panel-info">
                <div class="panel-heading">
                    <div class="panel-heading-btn">
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-default" data-click="panel-expand"><i class="fa fa-expand"></i></a>
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-success" data-click="panel-reload"><i class="fa fa-repeat"></i></a>
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-warning" data-click="panel-collapse"><i class="fa fa-minus"></i></a>
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-danger" data-click="panel-remove"><i class="fa fa-times"></i></a>
                    </div>
                    <h4 class="panel-title"><%=string.Format(U6000.TOPREFERERS, RecordsShown) %></h4>
                </div>
                <div class="panel-body">
                    <asp:Panel runat="server" ID="ReferralsPanel" ClientIDMode="Static">
                        <div class="table-responsive">
                            <asp:GridView runat="server" ID="ReferralGridView" DataSourceID="ReferralGridView_DataSource"
                                OnDataBound="ReferralGridView_DataBound" OnPreRender="BaseGridView_PreRender">
                                <Columns>
                                    <asp:BoundField DataField="Username" SortExpression="Username" />
                                    <asp:BoundField DataField="RefCount" SortExpression="RefCount" />
                                </Columns>
                            </asp:GridView>
                            <asp:SqlDataSource runat="server" ID="ReferralGridView_DataSource" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
                                OnInit="ReferralGridView_DataSource_Init"></asp:SqlDataSource>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </div>
        <div class="col-lg-6 col-md-6 col-sm-6 leaderboard">
            <div class="panel panel-danger">
                <div class="panel-heading">
                    <div class="panel-heading-btn">
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-default" data-click="panel-expand"><i class="fa fa-expand"></i></a>
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-success" data-click="panel-reload"><i class="fa fa-repeat"></i></a>
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-warning" data-click="panel-collapse"><i class="fa fa-minus"></i></a>
                        <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-danger" data-click="panel-remove"><i class="fa fa-times"></i></a>
                    </div>
                    <h4 class="panel-title"><%=string.Format(U6000.TOPCOMMISSIONEARNERS, RecordsShown) %></h4>
                </div>
                <div class="panel-body">
                    <asp:Panel runat="server" ID="CommissionsPanel" ClientIDMode="Static">
                        <div class="table-responsive">
                            <asp:GridView runat="server" ID="CommissionsGridView" DataSourceID="CommissionsGridView_DataSource"
                                OnDataBound="CommissionsGridView_DataBound" OnPreRender="BaseGridView_PreRender">
                                <Columns>
                                    <asp:BoundField DataField="Username" SortExpression="Username" />
                                    <asp:TemplateField SortExpression="TotalDirectReferralsEarned">
                                        <ItemTemplate>
                                            <%#new Money((decimal)Eval("TotalDirectReferralsEarned")).ToString() %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:SqlDataSource runat="server" ID="CommissionsGridView_DataSource" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
                                OnInit="CommissionsGridView_DataSource_Init"></asp:SqlDataSource>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </div>
       
    </div>
</asp:Content>
