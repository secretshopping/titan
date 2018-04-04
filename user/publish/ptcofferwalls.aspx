﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ptcofferwalls.aspx.cs" Inherits="user_publish_offerwalls" MasterPageFile="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script src="Scripts/default/assets/plugins/clipboard/clipboard.min.js"></script>
    <script>
        function pageLoad() {
            var clipboard = new Clipboard('.clipboard');
            $('.clipboard').tooltip({ trigger: 'focus' });
        }
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <asp:UpdatePanel ID="MultiViewUpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <h1 class="page-header"><%= U5009.OFFERWALLS%></h1>
            <div class="row">
                <div class="col-md-12">
                    <p class="lead"><%=U6000.PUBLISHOFFERWALLSONYOURWEBSITES %></p>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                        <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                    </asp:Panel>
                    <asp:Panel ID="SuccessMessagePanel" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
                        <asp:Literal ID="SuccessMessage" runat="server"></asp:Literal>
                    </asp:Panel>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <div class="nav nav-tabs custom text-right">
                        <div class="TitanViewPage">
                            <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                                <asp:Button ID="StatisticsMenuButton" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                                <asp:Button ID="GetCodeMenuButton" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </div>
            <div class="tab-content">
                <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
                    <asp:View runat="server" ID="GetCodeView" OnActivate="GetCodeView_Activate">
                        <div class="TitanViewElement">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="form-horizontal" runat="server" id="GetCodePlaceHolder">
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=U6000.WEBSITES %>:</label>
                                            <div class="col-md-3">
                                                <asp:DropDownList runat="server" ID="WebsitesDDL" class="form-control" OnInit="WebsitesDDL_Init"
                                                    OnSelectedIndexChanged="WebsitesDDL_SelectedIndexChanged" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=U6000.DIMENSIONS %>:</label>
                                            <div class="col-md-3">
                                                <div class="input-group">
                                                    <asp:TextBox runat="server" ID="WidthTextBox" class="form-control" Text="300">
                                                    </asp:TextBox>
                                                    <span class="input-group-addon">x</span>
                                                    <asp:TextBox runat="server" ID="HeightTextBox" class="form-control" Text="1000">
                                                    </asp:TextBox>
                                                    <span class="input-group-addon">px</span>
                                                </div>
                                            </div>
                                            <div class="col-md-2">
                                                <asp:LinkButton runat="server" ID="RefreshIFrameButton" OnClick="RefreshIFrameButton_Click" CssClass="btn btn-inverse">
                                                </asp:LinkButton>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="col-md-12">
                                                <asp:PlaceHolder runat="server" ID="IFramePlaceHolder">
                                                    <div class="alert alert-info">
                                                        <%=U6002.PUBLISHERREPLACEVARIABLES %>*
                                                       
                                                        <br />
                                                        <span class="small">*<%=U6002.GENDEROPTIONS %></span>
                                                    </div>
                                                    <div class="clipboard-wrapper">
                                                        <pre id="<%=IframeLiteral.ClientID %>"><asp:Literal runat="server" ID="IframeLiteral"></asp:Literal></pre>
                                                        <button type="button" class="clipboard btn btn-inverse height-full" data-click="tooltip" data-placement="top" title="<%=U6000.COPIED %>!" data-clipboard-target="#<%=IframeLiteral.ClientID %>"><%=U6000.COPY %></button>
                                                    </div>
                                                </asp:PlaceHolder>
                                                <div class="alert alert-warning" runat="server" id="IFrameUnavailablePlaceHolder">
                                                    <%=U6000.WEBSITEMUSTBEACCEPTED %>
                                                </div>
                                            </div>
                                        </div>
                                        <%--                                        <div class="form-group">
                                            <div class="col-md-6">
                                                <div class="banner-iframe">
                                                    <%=HttpUtility.HtmlDecode(IframeLiteral.Text) %>
                                                </div>
                                            </div>
                                        </div>--%>
                                    </div>

                                    <titan:FeatureUnavailable runat="server" ID="GetCodeUnavailable"></titan:FeatureUnavailable>
                                </div>
                            </div>
                        </div>
                    </asp:View>
                    <asp:View runat="server" ID="StatisticsView" OnActivate="StatisticsView_Activate">
                        <div class="TitanViewElement">
                            <asp:GridView runat="server" ID="StatisticsGridView" AllowPaging="true" AllowSorting="true"
                                DataSourceID="StatisticsGridViewDataSource" EmptyDataText="<%$ ResourceLookup:NOSTATS %>"
                                OnPreRender="BaseGridView_PreRender" PageSize="20" OnDataBound="StatisticsGridView_DataBound">
                                <Columns>
                                    <asp:BoundField DataField="Title" SortExpression="Title" />
                                    <asp:BoundField DataField="Host" SortExpression="Host" />
                                    <asp:BoundField DataField="Views" SortExpression="Views" />
                                    <asp:TemplateField SortExpression="Payout">
                                        <ItemTemplate>
                                            <%#Money.Parse(Eval("Payout").ToString()) %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:SqlDataSource runat="server" ID="StatisticsGridViewDataSource"
                                OnInit="StatisticsGridViewDataSource_Init" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"></asp:SqlDataSource>
                        </div>
                    </asp:View>
                </asp:MultiView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>