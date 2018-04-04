<%@ Page Language="C#" AutoEventWireup="true" CodeFile="myurls.aspx.cs" Inherits="user_advert_myurls" MasterPageFile="~/User.master" %>

<asp:Content ContentPlaceHolderID="PageHeadContent" runat="server">
    <script>
        function CheckURL() {
            $('#__EVENTARGUMENT5').val($('#<%=UrlTextBox.ClientID %>').val());

            //Send request
            $('#<%=Form.ClientID%>').attr('action', 'user/earn/surf.aspx?f=3');
            $('#<%=Form.ClientID%>').attr('target', '_blank');
            $('#<%=Form.ClientID%>').submit();
            $('#<%=Form.ClientID%>').attr('action', 'user/advert/myurls.aspx');
            $('#<%=Form.ClientID%>').attr('target', '_self');
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <input type="hidden" name="__EVENTARGUMENT5" id="__EVENTARGUMENT5" value="" />
    <asp:UpdatePanel ID="MultiViewUpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <h1 class="page-header"><%=U6002.MYURLS %></h1>
            <div class="row">
                <div class="col-md-12">
                    <p class="lead"><%=U6002.YOURURLSDESCRIPTION %></p>
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
                                <asp:Button ID="MenuButtonMyUrls" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                                <asp:Button ID="MenuButtonAddUrls" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected"/>
                                
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </div>
            <div class="tab-content">
                <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
                    <asp:View runat="server" ID="AddUrlsView">
                        <div class="TitanViewElement">
                            <div class="row">
                                <div class="row">
                                    <div class="col-md-12">
                                        <asp:ValidationSummary ID="ChangeSettingsValidationSummary" runat="server" CssClass="alert alert-danger fade in m-b-15"
                                            ValidationGroup="AddUrlValidationGroup" DisplayMode="List" />
                                    </div>
                                </div>
                                <div class="col-md-8">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <label class="control-label col-md-2">URL:</label>
                                            <div class="col-md-6">
                                                <asp:UpdatePanel runat="server" ID="AddNewAdWithURLCheck" OnLoad="UrlCheckerUpdatePanel_Load" ClientIDMode="Static" class="input-group">
                                                    <ContentTemplate>
                                                        <asp:TextBox runat="server" ID="UrlTextBox" CssClass="form-control" MaxLength="800"></asp:TextBox>

                                                        <div class="input-group-btn">
                                                            <asp:LinkButton ID="CheckURLButton" runat="server" OnClientClick="CheckURL()" CssClass="btn btn-primary"><%=U4200.CHECKURLTEXT %></asp:LinkButton>
                                                        </div>

                                                    </ContentTemplate>
                                                </asp:UpdatePanel>

                                                <asp:RegularExpressionValidator Display="Dynamic" CssClass="text-danger" ValidationGroup="AddUrlValidationGroup"
                                                    ID="UrlRegularExpressionValidator" runat="server" ErrorMessage="*"
                                                    ControlToValidate="UrlTextBox" Text="">*</asp:RegularExpressionValidator>
                                                <asp:RequiredFieldValidator ID="UrlRequiredFieldValidator" runat="server"
                                                    ControlToValidate="UrlTextBox" Display="Dynamic" CssClass="text-danger"
                                                    ValidationGroup="AddUrlValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="col-md-2">
                                                <asp:Button ID="AddUrlButton" runat="server"
                                                    ValidationGroup="AddUrlValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="AddUrlButton_Click" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </asp:View>
                    <asp:View runat="server" ID="MyUrlsView" OnActivate="MyUrlsView_Activate">
                        <div class="TitanViewElement">
                            <div class="row">
                                <div class="col-md-12">
                                    <asp:GridView ID="MyUrlsGridView" runat="server" AllowPaging="True" AllowSorting="True"
                                        DataSourceID="MyUrlsGridView_DataSource" OnPreRender="BaseGridView_PreRender"
                                        OnRowDataBound="MyUrlsGridView_RowDataBound" PageSize="20" OnDataBound="MyUrlsGridView_DataBound"
                                        EmptyDataText="<%$ ResourceLookup : NODATA %>" OnRowCommand="MyUrlsGridView_RowCommand">
                                        <Columns>
                                            <asp:BoundField DataField='Id' HeaderText='Id' SortExpression='Id' ControlStyle-CssClass="displaynone"
                                                HeaderStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" />
                                            <asp:BoundField DataField='Url' HeaderText='URL' SortExpression='Url' />
                                            <asp:BoundField DataField="Status" HeaderText='<%$ ResourceLookup : STATUS %>' SortExpression="Status" />

                                            <asp:TemplateField HeaderText="">
                                                <ItemStyle Width="13px" />
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="ImageButton1" runat="server"
                                                        ToolTip='Start'
                                                        CommandName="start"
                                                        CommandArgument='<%# Container.DataItemIndex %>'><span class="fa fa-play fa-lg text-success"></span></asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="">
                                                <ItemStyle Width="13px" />
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="ImageButton2" runat="server"
                                                        ToolTip='<%$ ResourceLookup : PAUSE %>'
                                                        CommandName="stop"
                                                        CommandArgument='<%# Container.DataItemIndex %>'><span class="fa fa-pause fa-lg text-warning"></span></asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="">
                                                <ItemStyle Width="13px" />
                                                <ItemTemplate>
                                                    <asp:LinkButton runat="server"
                                                        ToolTip='<%$ ResourceLookup : REMOVE %>'
                                                        CommandName="remove"
                                                        CommandArgument='<%# Container.DataItemIndex %>'>
                                                <spin class="fa fa-times fa-lg text-danger"></spin>
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                        </Columns>
                                    </asp:GridView>
                                    <asp:SqlDataSource ID="MyUrlsGridView_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
                                        OnInit="MyUrlsGridView_DataSource_Init"></asp:SqlDataSource>

                                </div>
                            </div>
                        </div>
                    </asp:View>
                </asp:MultiView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
