<%@ Page Language="C#" AutoEventWireup="true" CodeFile="websites.aspx.cs" Inherits="user_publish_websites" MasterPageFile="~/User.master" %>

<asp:Content ContentPlaceHolderID="PageHeadContent" runat="server">
    <script>
        function CheckURL() {
            $('#__EVENTARGUMENT5').val($('#<%=WebsiteUrlTextBox.ClientID %>').val());

            //Send request
            $('#<%=Form.ClientID%>').attr('action', 'user/earn/surf.aspx?f=3');
            $('#<%=Form.ClientID%>').attr('target', '_blank');
            $('#<%=Form.ClientID%>').submit();
            $('#<%=Form.ClientID%>').attr('action', 'user/publish/websites.aspx');
            $('#<%=Form.ClientID%>').attr('target', '_self');
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <input type="hidden" name="__EVENTARGUMENT5" id="__EVENTARGUMENT5" value="" />
    <asp:UpdatePanel ID="MultiViewUpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <h1 class="page-header"><%= U6000.WEBSITES%></h1>
            <div class="row">
                <div class="col-md-12">
                    <p class="lead"><%=U6000.ADDANDMANAGEWEBSITES %></p>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <asp:Panel ID="ErrorMessagePanel" runat="server" CssClass="alert alert-danger fade in m-b-15">
                        <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                    </asp:Panel>
                    <asp:Panel ID="SuccessMessagePanel" runat="server" CssClass="alert alert-success fade in m-b-15">
                        <asp:Literal ID="SuccessMessage" runat="server"></asp:Literal>
                    </asp:Panel>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <div class="nav nav-tabs custom text-right">
                        <div class="TitanViewPage">
                            <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                                <asp:Button ID="MenuButtonMyWebsites" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                                <asp:Button ID="MenuButtonAddWebsite" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </div>
            <div class="tab-content">
                <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
                    <asp:View runat="server" ID="AddWebsitesView" OnActivate="AddWebsitesView_Activate">
                        <div class="TitanViewElement">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="form-horizontal" runat="server" id="AddWebsitePlaceHolder">
                                        <div class="form-group">
                                            <label class="control-label col-md-2">URL:</label>
                                            <div class="col-md-6">
                                                <asp:UpdatePanel runat="server" ID="AddNewAdWithURLCheck" OnLoad="UrlCheckerUpdatePanel_Load" ClientIDMode="Static" class="input-group">
                                                    <ContentTemplate>
                                                        <asp:TextBox runat="server" ID="WebsiteUrlTextBox" CssClass="form-control" MaxLength="800"></asp:TextBox>

                                                        <div class="input-group-btn">
                                                            <asp:LinkButton ID="CheckURLButton" runat="server" OnClientClick="CheckURL()" CssClass="btn btn-primary"><%=U4200.CHECKURLTEXT %></asp:LinkButton>
                                                        </div>

                                                    </ContentTemplate>
                                                </asp:UpdatePanel>

                                                <asp:RegularExpressionValidator Display="Dynamic" CssClass="text-danger" ValidationGroup="AddWebsiteValidationGroup"
                                                    ID="UrlRegularExpressionValidator" runat="server" ErrorMessage="*"
                                                    ControlToValidate="WebsiteUrlTextBox" Text="">*</asp:RegularExpressionValidator>
                                                <asp:RequiredFieldValidator ID="UrlRequiredFieldValidator" runat="server"
                                                    ControlToValidate="WebsiteUrlTextBox" Display="Dynamic" CssClass="text-danger"
                                                    ValidationGroup="AddWebsiteValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=U5006.CATEGORY %>:</label>
                                            <div class="col-md-6">
                                                <asp:DropDownList runat="server" ID="CategoriesDDL" class="form-control" OnInit="CategoriesDDL_Init"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="col-md-2">
                                                <asp:Button ID="AddWebsiteButton" runat="server"
                                                    ValidationGroup="AddWebsiteValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="AddWebsiteButton_Click" />
                                            </div>
                                        </div>
                                    </div>
                                    <titan:FeatureUnavailable runat="server" ID="NewWebsiteUnavailable"></titan:FeatureUnavailable>
                                </div>
                            </div>
                        </div>
                    </asp:View>
                    <asp:View runat="server" ID="MyWebsitesView" OnActivate="MyWebsitesView_Activate">
                        <div class="TitanViewElement">
                            <div class="row">
                                <div class="col-md-12">
                                    <asp:GridView ID="MyWebsitesGridView" runat="server" AllowPaging="True" AllowSorting="True"
                                        DataSourceID="MyWebsitesGridView_DataSource" OnPreRender="BaseGridView_PreRender"
                                        OnRowDataBound="MyWebsitesGridView_RowDataBound" PageSize="20" OnDataBound="MyWebsitesGridView_DataBound"
                                        EmptyDataText="<%$ ResourceLookup : NOWEBSITES %>">
                                        <Columns>
                                            <asp:BoundField DataField='Id' HeaderText='Id' SortExpression='Id' ControlStyle-CssClass="displaynone"
                                                HeaderStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" />
                                            <asp:BoundField DataField='Url' HeaderText='Url' SortExpression='Url' />
                                            <asp:BoundField DataField='PublishersWebsiteCategoryId' SortExpression='PublishersWebsiteCategoryId' />
                                            <asp:TemplateField SortExpression="Status">
                                                <ItemTemplate>
                                                    <%# HtmlCreator.GetColoredStatus((PublishersWebsiteStatus)(Eval("Status"))) %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="PostbackUrl" NullDisplayText="-" SortExpression="PostbackUrl" />
                                        </Columns>
                                    </asp:GridView>
                                    <asp:SqlDataSource ID="MyWebsitesGridView_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
                                        OnInit="MyWebsitesGridView_DataSource_Init"></asp:SqlDataSource>

                                </div>
                            </div>
                        </div>
                    </asp:View>
                </asp:MultiView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
