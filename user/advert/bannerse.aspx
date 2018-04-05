<%@ Page Language="C#" AutoEventWireup="true" CodeFile="bannerse.aspx.cs" Inherits="user_advert_bannerse" MasterPageFile="~/User.master" %>

<asp:Content ContentPlaceHolderID="PageHeadContent" runat="server">
    <script>
        function CheckURL() {
            $('#__EVENTARGUMENT5').val($('#<%=BannerUrlTextBox.ClientID %>').val());

            //Send request
            $('#<%=Form.ClientID%>').attr('action', 'user/earn/surf.aspx?f=3');
            $('#<%=Form.ClientID%>').attr('target', '_blank');
            $('#<%=Form.ClientID%>').submit();
            $('#<%=Form.ClientID%>').attr('action', 'user/advert/bannerse.aspx');
            $('#<%=Form.ClientID%>').attr('target', '_self');
            return false;
        }

        function showURLBox() {
            $('#<%=BannerFileUrlTextBox.ClientID%>').show();
            return false;
        }

        function hideURLBox() {
            $('#<%=BannerFileUrlTextBox.ClientID%>').hide();
            $('#<%=BannerFileUrlTextBox.ClientID%>').val('');
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <input type="hidden" name="__EVENTARGUMENT5" id="__EVENTARGUMENT5" value="" />
    <asp:UpdatePanel ID="MultiViewUpdatePanel" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:PostBackTrigger ControlID="ImageUploadButton" />
        </Triggers>
        <ContentTemplate>

            <h1 class="page-header"><%=L1.BANNERS %></h1>
            <div class="row">
                <div class="col-md-12">
                    <p class="lead"><%=L1.ADVADSINFO %></p>
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
                                <asp:Button ID="MenuButtonMyBanners" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                                <asp:Button ID="MenuButtonBuyBanner" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </div>
            <div class="tab-content">
                <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
                    <asp:View runat="server" ID="BuyBannerView" OnActivate="BuyBannerView_Activate">
                        <div class="TitanViewElement">
                            <div class="row">
                                <div class="row">
                                    <div class="col-md-12">
                                        <asp:ValidationSummary runat="server" CssClass="alert alert-danger fade in m-b-15"
                                            ValidationGroup="BuyBannerValidationGroup" DisplayMode="List" />
                                    </div>
                                </div>
                                <div class="col-md-8">
                                    <div class="form-horizontal" runat="server" id="BuyBannersPlaceHolder">
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=L1.PACK %>:</label>
                                            <div class="col-md-6">
                                                <asp:DropDownList runat="server" ID="PacksDDL" class="form-control" OnInit="PacksDDL_Init"
                                                    OnSelectedIndexChanged="PacksDDL_SelectedIndexChanged" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=U5006.CATEGORY %>:</label>
                                            <div class="col-md-6">
                                                <asp:DropDownList runat="server" ID="CategoriesDDL" class="form-control" OnInit="CategoriesDDL_Init"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="text-center">
                                                <asp:Image ID="ImagePreview" runat="server" CssClass="img-responsive" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-md-2 control-label">
                                                <span><%=L1.BURL %>: </span>
                                            </label>
                                            <div class="col-md-6">
                                                <span class="btn btn-success fileinput-button">
                                                    <i class="fa fa-plus"></i>
                                                    <span><%=U6000.ADDFILE %></span>
                                                    <asp:FileUpload runat="server" ID="ImageUpload" onclick="hideURLBox();" />
                                                </span>
                                                <asp:Button ID="BannerUploadByUrlButton" Text="<%$ResourceLookup: ADDBANNERBYURL %>" runat="server" CssClass="btn btn-success fileinput-button" OnClientClick="showURLBox(); return false;" />

                                            </div>
                                            <div class="col-md-6 col-md-offset-2 m-t-15">
                                                <div class="input-group">
                                                    <asp:TextBox ID="BannerFileUrlTextBox" runat="server" CssClass="form-control" Style="display: none"></asp:TextBox>
                                                    <div class="input-group-btn">
                                                        <asp:Button runat="server" ID="ImageUploadButton" CssClass="btn btn-primary" Text="<%$ResourceLookup: SUBMIT %>" OnClick="ImageUploadButton_Click" ValidationGroup="ImageUploadValidationGroup" />
                                                    </div>
                                                </div>
                                                
                                                <asp:CustomValidator runat="server" ID="ImageUploadedValidator" OnServerValidate="ImageUploadedValidator_ServerValidate" Display="Dynamic" CssClass="text-danger"
                                                    ValidationGroup="BuyBannerValidationGroup">*</asp:CustomValidator>
                                                <asp:CustomValidator runat="server" ID="ImageSubmitValidator" OnServerValidate="ImageSubmitValidator_ServerValidate" Display="Dynamic" CssClass="text-danger"
                                                    ValidationGroup="ImageUploadValidationGroup">*</asp:CustomValidator>

                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="control-label col-md-2">URL:</label>
                                            <div class="col-md-6">
                                                <asp:UpdatePanel runat="server" ID="AddNewAdWithURLCheck" OnLoad="UrlCheckerUpdatePanel_Load" ClientIDMode="Static" class="input-group">
                                                    <ContentTemplate>
                                                        <asp:TextBox runat="server" ID="BannerUrlTextBox" CssClass="form-control" MaxLength="800"></asp:TextBox>

                                                        <div class="input-group-btn">
                                                            <asp:LinkButton ID="CheckURLButton" runat="server" OnClientClick="CheckURL()" CssClass="btn btn-primary"><%=U4200.CHECKURLTEXT %></asp:LinkButton>
                                                        </div>

                                                    </ContentTemplate>
                                                </asp:UpdatePanel>

                                                <asp:RegularExpressionValidator Display="Dynamic" CssClass="text-danger" ValidationGroup="BuyBannerValidationGroup"
                                                    ID="UrlRegularExpressionValidator" runat="server" ErrorMessage="*"
                                                    ControlToValidate="BannerUrlTextBox" Text="">*</asp:RegularExpressionValidator>
                                                <asp:RequiredFieldValidator ID="UrlRequiredFieldValidator" runat="server"
                                                    ControlToValidate="BannerUrlTextBox" Display="Dynamic" CssClass="text-danger"
                                                    ValidationGroup="BuyBannerValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <titan:TargetBalance runat="server" Feature="ExternalBanner" ID="TargetBalanceRadioButtonList"></titan:TargetBalance>
                                        <div class="form-group">
                                            <div class="col-md-2">
                                                <asp:Button ID="BuyBannerButton" runat="server"
                                                    ValidationGroup="BuyBannerValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="BuyBannerButton_Click" />
                                            </div>
                                        </div>
                                    </div>
                                    <titan:FeatureUnavailable runat="server" ID="NewBannerUnavailable"></titan:FeatureUnavailable>
                                </div>
                            </div>
                        </div>
                    </asp:View>
                    <asp:View runat="server" ID="MyBannersView" OnActivate="MyBannersView_Activate">
                        <div class="TitanViewElement">
                            <div class="row">
                                <div class="col-md-12">
                                    <asp:GridView ID="MyBannersGridView" runat="server" AllowPaging="True" AllowSorting="True"
                                        DataSourceID="MyBannersGridView_DataSource" OnPreRender="BaseGridView_PreRender"
                                        OnRowDataBound="MyBannersGridView_RowDataBound" PageSize="20" OnDataBound="MyBannersGridView_DataBound"
                                        EmptyDataText="<%$ ResourceLookup : NOBANNERCAMPS %>">
                                        <Columns>
                                            <asp:BoundField DataField='Id' HeaderText='Id' SortExpression='Id' ControlStyle-CssClass="displaynone"
                                                HeaderStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" />
                                            <asp:BoundField DataField='Url' HeaderText='URL' SortExpression='Url' />
                                            <asp:BoundField DataField='ImagePath' SortExpression='ImagePath' />
                                            <asp:BoundField DataField='Category' SortExpression='Category' />
                                            <asp:BoundField SortExpression="ExternalBannerAdvertPackId" />
                                            <asp:BoundField SortExpression="ClicksReceived" />
                                            <asp:TemplateField SortExpression="Status">
                                                <ItemTemplate>
                                                    <%# HtmlCreator.GetColoredStatus((Prem.PTC.Advertising.AdvertStatus)(Eval("Status"))) %>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                        </Columns>
                                    </asp:GridView>
                                    <asp:SqlDataSource ID="MyBannersGridView_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
                                        OnInit="MyBannersGridView_DataSource_Init"></asp:SqlDataSource>

                                </div>
                            </div>
                        </div>
                    </asp:View>
                </asp:MultiView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
