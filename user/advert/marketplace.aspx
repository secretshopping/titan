<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="marketplace.aspx.cs" Inherits="About" EnableEventValidation="false" ValidateRequest="false" EnableViewStateMac="false" %>

    <%@ MasterType VirtualPath="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script src="Scripts/default/assets/plugins/clipboard/clipboard.min.js"></script>
    <script type="text/javascript" src="Scripts/gridview.js"></script>
    <script>
        function pageLoad()
        {
            ManageGeoEvent();
            var clipboard = new Clipboard('.clipboard');
            $('.clipboard').tooltip({ trigger: 'focus' });
        }
        function ManageGeoEvent() {
            if ($('#<%=GeolocationCheckBox.ClientID%>').is(':checked')) {
                $('#geoPanels').show();
            }
            else {
                $('#geoPanels').hide();
            }
        }
    </script>
    <link rel="stylesheet" href="Styles/WatchAds.css" type="text/css" />
</asp:Content>


<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header">Marketplace</h1>

    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=U5006.MARKETPLACEDESC %></p>
        </div>
    </div>

    <asp:UpdatePanel ID="MultiViewUpdatePanel" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:PostBackTrigger ControlID="MarketplaceAdd_BannerUploadSubmit" />
            <asp:PostBackTrigger ControlID="SubMenuButton_MarketplaceAddProduct" />
        </Triggers>
        <ContentTemplate>
            <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
            </asp:Panel>

            <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
                <asp:Literal ID="SuccMessage" runat="server"></asp:Literal>
            </asp:Panel>

            <div class="row">
                <div class="col-md-12">
                    <div class="nav nav-tabs custom text-right">
                        <asp:Panel ID="TitanViewPagePanel" runat="server" CssClass="TitanViewPage">

                            <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                                <asp:Button ID="SubSubMenuButton_MarketplaceMyProducts" runat="server" OnClick="MenuButton_Click" CommandArgument="3" />
                                <asp:Button ID="SubMenuButton_MarketplaceConfirmBuying" runat="server" OnClick="MenuButton_Click" CommandArgument="2" />
                                <asp:Button ID="SubMenuButton_MarketplaceAddProduct" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                                <asp:Button ID="SubMenuButton_MarketplaceProducts" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                            </asp:PlaceHolder>
                        </asp:Panel>
                    </div>
                </div>
            </div>

            <div class="tab-content">
                <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
                    <!--Marketplace products-->
                    <asp:View ID="View1" runat="server">
                        <div class="TitanViewElement">
                            <div class="row">
                                <asp:PlaceHolder ID="MarketplaceProductsPlaceholder" runat="server"></asp:PlaceHolder>
                                <asp:PlaceHolder ID="MarketEmptyPlaceHolder" runat="server" Visible="false">
                                    <h3><%=U5006.NOPRODUCTSTOBUY %></h3>
                                </asp:PlaceHolder>
                            </div>
                        </div>
                    </asp:View>

                    <!--Marketplace add new-->
                    <asp:View ID="View2" runat="server">
                        <div class="TitanViewElement">
                            <div class="row">
                                <div class="col-md-12">
                                    <h3 runat="server" id="AdvertisingUnavailable" visible="false"><%=U5009.ADVERTISINGUNAVAILABLE %></h3>

                                    <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="alert alert-danger fade in m-b-15"
                                        ValidationGroup="RegisterUserValidationGroup" DisplayMode="List" />
                                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger fade in m-b-15"
                                        ValidationGroup="MarketplaceAdd_OnSubmitValidationGroup" DisplayMode="List" />

                                </div>
                            </div>
                            <asp:PlaceHolder runat="server" ID="AdvertisePlaceholder">
                                <div class="row">
                                    <div class="col-md-12">

                                        <div class="form-horizontal">

                                            <div class="form-group">
                                                <label class="control-label col-md-2"><%=L1.TITLE %>:</label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="MarketplaceAddTitle" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>

                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="MarketplaceAddTitle"
                                                        Display="Dynamic" CssClass="text-danger"
                                                        ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                                </div>
                                            </div>

                                            <div class="form-group">
                                                <label class="control-label col-md-2"><%=L1.DESCRIPTION %>:</label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="MarketplaceAddDescription" runat="server" CssClass="form-control" MaxLength="500" TextMode="MultiLine" Rows="3"></asp:TextBox>

                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="MarketplaceAddDescription"
                                                        Display="Dynamic" CssClass="text-danger"
                                                        ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                                </div>
                                            </div>

                                            <div class="form-group">
                                                <label class="control-label col-md-2"><%=L1.CATEGORIES %>:</label>
                                                <div class="col-md-6">
                                                    <asp:DropDownList ID="MarketplaceAddCategoriesList" runat="server" CssClass="form-control"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="form-group">
                                                <label class="control-label col-md-2"><%=L1.CONTACT %>:</label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="MarketplaceAddContact" runat="server" CssClass="form-control" MaxLength="100" TextMode="MultiLine" Rows="3"></asp:TextBox>

                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="MarketplaceAddContact"
                                                        Display="Dynamic" CssClass="text-danger"
                                                        ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                                </div>
                                            </div>

                                            <div class="form-group">
                                                <label class="col-md-2 control-label"><%=L1.IMAGE +" (max "+MaxImageWidth+"x"+MaxImageHeight +"px)"%>:</label>

                                                <div class="col-md-6 col-xs-12">

                                                    <asp:Image ID="MarketplaceAdd_BannerImage" runat="server" />

                                                    <span class="btn btn-success fileinput-button">
                                                        <i class="fa fa-plus"></i>
                                                        <span><%=U6000.ADDFILE %></span>
                                                        <asp:FileUpload ID="MarketplaceAdd_BannerUpload" runat="server" />
                                                    </span>
                                                    <asp:Button ID="MarketplaceAdd_BannerUploadSubmit" Text="Submit" OnClick="MarketplaceAdd_BannerUploadSubmit_Click"
                                                        CausesValidation="true" runat="server" ValidationGroup="MarketplaceAdd_OnSubmitValidationGroup" CssClass="btn btn-primary" />
                                                    <br />
                                                    <asp:CustomValidator ID="MarketplaceAdd_BannerUploadValidCustomValidator"
                                                        ControlToValidate="MarketplaceAdd_BannerUpload" Display="Dynamic" CssClass="text-danger"
                                                        OnServerValidate="MarketplaceAdd_BannerUploadValidCustomValidator_ServerValidate"
                                                        ValidationGroup="MarketplaceAdd_OnSubmitValidationGroup" ValidateEmptyText="true"
                                                        runat="server">*</asp:CustomValidator>
                                                    <asp:CustomValidator ID="MarketplaceAdd_BannerUploadSelectedCustomValidator"
                                                        ControlToValidate="MarketplaceAdd_BannerUpload" Display="Dynamic" CssClass="text-danger"
                                                        OnServerValidate="MarketplaceAdd_BannerUploadSelectedCustomValidator_ServerValidate"
                                                        ValidationGroup="RegisterUserValidationGroup" ValidateEmptyText="true"
                                                        runat="server">*</asp:CustomValidator>

                                                </div>
                                            </div>

                                            <div class="form-group">
                                                <label class="control-label col-md-2"><%=L1.HOWMANY %>:</label>
                                                <div class="col-md-6">
                                                    <asp:TextBox autocomplete="off" ID="MarketplaceAddQuantity" runat="server" Text="10" ClientIDMode="Static" CssClass="form-control" MaxLength="6"></asp:TextBox>

                                                    <asp:RangeValidator ID="RangeValidator1" Display="Dynamic" CssClass="text-danger"
                                                        ValidationGroup="RegisterUserValidationGroup" runat="server" ErrorMessage="Invalid number"
                                                        ControlToValidate="MarketplaceAddQuantity" Text="" Type="Integer" MinimumValue="1" MaximumValue="999999">*</asp:RangeValidator>

                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="MarketplaceAddQuantity"
                                                        Display="Dynamic" CssClass="text-danger"
                                                        ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                                </div>
                                            </div>

                                            <div class="form-group">
                                                <label class="control-label col-md-2"><%=U5006.UNITPRICE %>:</label>

                                                <div class="col-md-6">
                                                    <div class="input-prepend input-group">
                                                        <span class="add-on input-group-addon">
                                                            <%=AppSettings.Site.CurrencySign %>
                                                        </span>
                                                        <asp:TextBox autocomplete="off" ID="MarketplaceAddPrice" runat="server" ClientIDMode="Static" CssClass="form-control" Text="0.5"></asp:TextBox>
                                                    </div>

                                                    <asp:RegularExpressionValidator ForeColor="#b70d00" ValidationGroup="RegisterUserValidationGroup" ID="RegularExpressionValidator5" runat="server"
                                                        ErrorMessage="Invalid money format" Display="Dynamic" CssClass="text-danger"
                                                        ValidationExpression="[0-9]{1,5}[\.\,]{0,1}[0-9]{0,3}" ControlToValidate="MarketplaceAddPrice" Text="">*</asp:RegularExpressionValidator>

                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="MarketplaceAddPrice"
                                                        Display="Dynamic" CssClass="text-danger"
                                                        ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label ID="lblGeolocation" runat="server" AssociatedControlID="GeolocationCheckBox" CssClass="control-label col-md-2"><%=L1.GEOLOCATION %>:</asp:Label>
                                                <div class="col-md-6">
                                                    <div class="checkbox">
                                                        <label>
                                                            <asp:CheckBox ID="GeolocationCheckBox" runat="server" Checked="false" />
                                                        </label>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="geoPanels" style="display: none">
                                                <div class="form-group">
                                                    <div class="col-md-6 col-md-offset-2">
                                                        <asp:ListBox ID="AllCountries" runat="server" CssClass="form-control tooltip-on"></asp:ListBox>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <div class="col-md-6 col-md-offset-2">
                                                        <div class="input-group-btn">
                                                            <asp:Button ID="btnAdd" CssClass="titab titab1 btn" runat="server" Text="&#8595;" OnClick="btnAdd_Click" />
                                                            <asp:Button ID="btnRemove" CssClass="titab titab3 btn" runat="server" Text="&#8593;" OnClick="btnRemove_Click" />
                                                            <asp:Button ID="btnAddAll" CssClass="titab titab2 btn" runat="server" Text="&#8595;&#8595;" OnClick="btnAddAll_Click" />
                                                            <asp:Button ID="btnRemoveAll" CssClass="titab titab4 btn" runat="server" Text="&#8593;&#8593;" OnClick="btnRemoveAll_Click" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <div class="col-md-6 col-md-offset-2">
                                                        <asp:ListBox ID="GeoCountries" runat="server" CssClass="form-control tooltip-on"></asp:ListBox>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-md-2 control-label"><%=U3900.AGE %>:</label>
                                                    <div class="col-md-3">
                                                        <div class="input-group">
                                                            <asp:TextBox ID="GeoAgeMin" CssClass="form-control tooltip-on" runat="server" MaxLength="2" Text="0"></asp:TextBox>
                                                            <span class="input-group-addon">-</span>
                                                            <asp:TextBox ID="GeoAgeMax" CssClass="form-control tooltip-on" runat="server" MaxLength="2" Text="0"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-md-2 control-label"><%=L1.GENDER %>:</label>
                                                    <div class="col-md-6">
                                                        <div class="input-group">
                                                            <div class="radio radio-button-list">
                                                                <asp:RadioButtonList ID="GeoGenderList" runat="server" RepeatLayout="Flow">
                                                                    <asp:ListItem Text="All" Value="0" Selected="True"></asp:ListItem>
                                                                    <asp:ListItem Text="Male" Value="1"></asp:ListItem>
                                                                    <asp:ListItem Text="Female" Value="2"></asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <div class="col-md-2">
                                                    <asp:Button ID="MarketplaceAddButton" runat="server"
                                                        CssClass="btn btn-inverse btn-block" OnClick="MarketplaceAddButton_Click" ValidationGroup="RegisterUserValidationGroup"
                                                        OnClientClick="if (Page_ClientValidate()){this.disabled = true; this.className = 'rbutton-loading'; this.value='';}"
                                                        UseSubmitBehavior="false" />

                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </asp:PlaceHolder>
                        </div>
                    </asp:View>

                    <!--Marketplace purchased products-->
                    <asp:View ID="View3" runat="server" OnActivate="View3_Activate">
                        <div class="TitanViewElement">
                            <div class="row">
                                <div class="col-md-12">

                                    <asp:GridView ID="PurchasedProductsGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                                        DataSourceID="PurchasedProductsGridViewDataSource" OnPreRender="BaseGridView_PreRender" OnRowDataBound="PurchasedProductsGridView_RowDataBound" OnRowCommand="PurchasedProductsGridView_RowCommand"
                                        PageSize="20">
                                        <Columns>
                                            <asp:BoundField DataField='Id' HeaderText='Id' SortExpression='Id' ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" />
                                            <asp:BoundField SortExpression="ImagePath" DataField='ImagePath'></asp:BoundField>
                                            <asp:BoundField DataField='Title' SortExpression='Title' />
                                            <asp:TemplateField SortExpression="Price">
                                                <ItemTemplate>
                                                    <%#Money.Parse(Eval("Price").ToString()) %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField='ProductQuantity' HeaderText='ProductQuantity' SortExpression='ProductQuantity' />
                                            <asp:TemplateField SortExpression="SellerId">
                                                <ItemTemplate>
                                                    <%#  MemberManager.getUsersProfileURL(new Member((int)Eval("SellerId")).Name)%>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField='Status' SortExpression='Status' />
                                            <asp:TemplateField HeaderText="">
                                                <ItemStyle />
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="PurchasedProductsConfirmButton" runat="server"
                                                        ToolTip='Confirm'
                                                        CommandName="ConfirmIPNCommand"
                                                        CommandArgument='<%# Container.DataItemIndex %>'>
                                                    <span class="fa fa-play fa-lg text-warning"></span>                                                    
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    <asp:SqlDataSource ID="PurchasedProductsGridViewDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="PurchasedProductsGridViewDataSource_Init"></asp:SqlDataSource>

                                </div>
                            </div>
                        </div>
                    </asp:View>

                    <asp:View ID="View4" runat="server" OnActivate="View4_Activate">
                        <div class="TitanViewElement">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="table-responsive">
                                        <asp:GridView ID="MyProductsGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                                            DataSourceID="MyProductsGridViewDataSource" OnRowDataBound="MyProductsGridView_RowDataBound"
                                            PageSize="20">
                                            <Columns>
                                                <asp:BoundField DataField='Id' HeaderText='Id' SortExpression='Id' ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" />
                                                <asp:BoundField SortExpression="ImagePath" DataField='ImagePath'></asp:BoundField>
                                                <asp:BoundField DataField='Title' HeaderText='Title' SortExpression='Title' />
                                                <asp:TemplateField SortExpression="Price">
                                                    <ItemTemplate>
                                                        <%#Money.Parse(Eval("Price").ToString()) %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField SortExpression="SellerId">
                                                    <ItemTemplate>
                                                        <%# (int)Eval("Sold")+"/"+ ((int)Eval("Quantity") + (int)Eval("Sold")).ToString()%>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <asp:SqlDataSource ID="MyProductsGridViewDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="MyProductsGridViewDataSource_Init"></asp:SqlDataSource>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </asp:View>

                    <asp:View ID="MarketplaceProductInfoWindow" runat="server">
                        <div class="TitanViewElement">
                            <div class="product">
                                <div class="row">
                                    <div class="product-detail">
                                        <div class="col-md-6">
                                            <div class="product-image">
                                                <div class="product-main-image">
                                                    <asp:Image ID="ProductInfoImage" runat="server" BorderStyle="Solid" BorderWidth="1px" BorderColor="#e1e1e1" Style="display: block; margin: auto;" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="product-info">
                                                <div class="product-info-header">
                                                    <h1 class="product-title">
                                                        <asp:Literal ID="ProductInfoTitle" runat="server"></asp:Literal></h1>
                                                </div>
                                                <div class="product-warranty">
                                                    <div>
                                                        <b><%=L1.CONTACT %>: </b>
                                                        <asp:Literal ID="ProductInfoContact" runat="server"></asp:Literal>
                                                    </div>
                                                </div>
                                                <div class="product-info-list">
                                                    <asp:Literal ID="ProductInfoDescription" runat="server"></asp:Literal>
                                                </div>
                                                <asp:PlaceHolder runat="server" ID="AffiliateLinkPlaceHolder" Visible="false">
                                                    <div>
                                                        <b><%=U6005.AFFILIATELINK %>:</b>
                                                        <asp:HiddenField runat="server" ID="AffiliateLink" />
                                                        <div class="clipboard-wrapper">
                                                            <pre id="AffiliateLinkPre"><%=ItemAffiliateLink %></pre>
                                                            <button type="button" class="clipboard btn btn-inverse height-full" data-click="tooltip" data-placement="top" title="<%=U6000.COPIED %>!" data-clipboard-target="#AffiliateLinkPre"><%=U6000.COPY %></button>
                                                        </div>
                                                    </div>
                                                </asp:PlaceHolder>
                                                <div class="product-purchase-container form-horizontal">
                                                    <div class="product-price ">
                                                        <div class="price">
                                                            <div class="form-group">
                                                                <label class="control-label col-md-9"><%=L1.BUY %></label>
                                                                <div class="col-md-3">
                                                                    <div class="input-group">
                                                                        <asp:TextBox autocomplete="off" ID="ProductInfoBuyCount" runat="server" Text="1" ClientIDMode="Static" CssClass="form-control width-50" Min="1" MaxLength="6"></asp:TextBox>
                                                                        <p class="input-group-addon text-left width-50">
                                                                            /
                                                                            <asp:Literal ID="ProductInfoQuantity" runat="server"></asp:Literal>
                                                                        </p>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="control-label col-md-2"><%=U6005.DELIVERYADDRESS %>:</label>
                                                                <div class="col-md-8">
                                                                    <asp:TextBox autocomplete="off" runat="server" ID="DeliveryAddressTextBox" TextMode="MultiLine" CssClass="form-control" MaxLength="1000"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator runat="server" ID="DeliveryAddressRequired" ControlToValidate="DeliveryAddressTextBox"
                                                                        ValidationGroup="BuyProductValidationGroup" Display="Dynamic" CssClass="text-danger">*</asp:RequiredFieldValidator>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="control-label col-md-2"><%=U4200.EMAIL %>:</label>
                                                                <div class="col-md-8">
                                                                    <asp:TextBox runat="server" ID="EmailTextBox" CssClass="form-control"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator runat="server" ID="EmailRequired" ControlToValidate="EmailTextBox"
                                                                        ValidationGroup="BuyProductValidationGroup" Display="Dynamic" CssClass="text-danger">*</asp:RequiredFieldValidator>
                                                                    <asp:RegularExpressionValidator runat="server" ID="EmailRegex" ControlToValidate="EmailTextBox"
                                                                        ValidationExpression="^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$"
                                                                        ValidationGroup="BuyProductValidationGroup" Display="Dynamic" CssClass="text-danger">*</asp:RegularExpressionValidator>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="form-group">
                                                        <div class="col-md-6">
                                                            <asp:Button ID="BuyProductButton" runat="server"
                                                                CssClass="btn btn-inverse btn-block btn-lg" OnClick="BuyProduct_Click"
                                                                UseSubmitBehavior="false" ValidationGroup="BuyProductValidationGroup" />
                                                            <asp:Button ID="BuyProductFromAdBalance" runat="server"
                                                                CssClass="btn btn-inverse btn-block btn-lg" OnClick="BuyProductFromAdOrMarketplaceBalance_Click"
                                                                UseSubmitBehavior="false" Visible="false" />
                                                            <asp:Button ID="BuyProductFromMarketplaceBalance" runat="server"
                                                                CssClass="btn btn-inverse btn-block btn-lg" OnClick="BuyProductFromAdOrMarketplaceBalance_Click"
                                                                UseSubmitBehavior="false" Visible="false" />
                                                        </div>
                                                    </div>
                                                    <div class="form-group" runat="server" id="PaymentProcessorButtonsPlaceHolder" visible="false">
                                                        <div class="col-md-12">
                                                            <asp:Literal ID="PaymentButtons" runat="server"></asp:Literal>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>

                        </div>
                    </asp:View>

                    <asp:View ID="ConfirmSingleProduct" runat="server">
                        <div class="TitanViewElement">
                            <div class="row">
                                <div class="col-md-6 text-center">
                                    <asp:Image ID="ConfirmProductImage" Style="height: 250px; max-width: 250px;" runat="server" BorderStyle="Solid" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6">
                                    <b>
                                        <asp:Literal runat="server" ID="ConfirmProductTitle"></asp:Literal></b>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6">
                                    <asp:Literal runat="server" ID="ConfirmProductDescription"></asp:Literal>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-3">
                                    <asp:Button ID="ConfirmProductButton" runat="server" OnClick="ConfirmProductButton_Click" CssClass="btn btn-inverse btn-block"
                                        UseSubmitBehavior="false" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6">
                                    <p>
                                        <asp:Literal ID="AlreadyConfirmedLiteral" runat="server" Visible="false"></asp:Literal><br />
                                        <b><%=L1.CONTACT %>: </b>
                                        <asp:Literal ID="ConfirmProductContact" runat="server"></asp:Literal>
                                    </p>
                                </div>
                            </div>
                        </div>
                    </asp:View>
                </asp:MultiView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
