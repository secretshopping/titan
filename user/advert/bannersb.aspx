<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="bannersb.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">

    <script type="text/javascript" src="Scripts/gridview.js"></script>

    <script type="text/javascript">

        function fixCSS() {
            $('#view1h').removeClass('TitanViewElement');
            $('#view1h').addClass('TitanViewElementSimple');
        }

        function showURLBox() {
            $('#<%=BannerFileUrlTextBox.ClientID%>').show();
            return false;
        }

        function hideURLBox() {
            $('#<%=BannerFileUrlTextBox.ClientID%>').hide();
            $('#<%=BannerFileUrlTextBox.ClientID%>').val('');
        }

        function pageLoad() {
            <%=PageScriptGenerator.GetGridViewCode(AuctionGrid) %>            
            <%=PageScriptGenerator.GetGridViewCode(DirectRefsGridView) %>
        }

    </script>

    <link rel="stylesheet" href="Styles/WatchAds.css" type="text/css" />
    <style>
        .custom.nav-pills input[type=submit] {
            background: #d9e0e7 !important;
        }
        .custom.nav-pills input[type=submit].ViewSelected {
            background: #242a30 !important;
        }
    </style>

</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=L1.BANNERS %></h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead">
                <asp:Literal ID="SubLiteral" runat="server"></asp:Literal>
            </p>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                    <asp:Button ID="Button2" runat="server" OnClick="MenuButton_Click" CommandArgument="2" />
                    <asp:Button ID="Button1" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                    <asp:Button ID="Button3" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                </asp:PlaceHolder>
            </div>
            
        </div>
    </div>

    <div class="tab-content">
        <div class="nav nav-pills custom text-right">                
            <asp:PlaceHolder ID="DimensionsPlaceHolder" runat="server" />
        </div>
        <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
            <asp:View runat="server" ID="View2">
                <div class="TitanViewElement">
                    <div class="row">
                    <%-- SUBPAGE START --%>
                    <asp:Panel ID="AuctionsPanel" runat="server">
                        <div class="row">
                            <div class="col-md-12">
                                <div class="MenuPanel">
                                    <h2><%=L1.BANNERS %>:
                                        <asp:Literal runat="server" ID="DimensionsHeaderLiteral" /></h2>
                                </div>
                                <div class="MenuContentPanel m-b-40">
                                    <asp:GridView ID="AuctionGrid" runat="server" OnPreRender="BaseGridView_PreRender" AllowPaging="true" AllowSorting="True" AutoGenerateColumns="False" OnRowCommand="AuctionGrid_RowCommand" PagerSettings-PageButtonCount="7"
                                        DataSourceID="AuctionGrid_DataSource" PageSize="25" OnRowDataBound="AuctionGrid_RowDataBound">
                                        <Columns>
                                            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone"></asp:BoundField>
                                            <asp:BoundField DataField="DateStart" HeaderText='<%$ ResourceLookup: DISPLAYTIME %>' SortExpression="DateStart"></asp:BoundField>
                                            <asp:BoundField DataField="DateStart" HeaderText='<%$ ResourceLookup : HIGHESTBID %>' SortExpression="DateStart" />
                                            <asp:BoundField DataField="DateStart" HeaderText='<%$ ResourceLookup : AUCTIONCLOSES %>' SortExpression="DateStart" />
                                            <asp:TemplateField HeaderText="#">
                                                <ItemStyle />
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="ImageButton1" runat="server"
                                                        ToolTip='Go to'
                                                        CommandName="auction"
                                                        CommandArgument='<%# Container.DataItemIndex %>'>
                                                    <span class="fa fa-play fa-lg m-r-10"></span>                                                      
                                                    </asp:LinkButton>
                                                    <asp:Literal runat="server"></asp:Literal>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="BiddingPanel" runat="server" Visible="false">
                        <div class="row">
                            <div class="col-md-12">
                                <asp:Panel ID="SucPanel" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
                                    <asp:Literal ID="SucMess" runat="server"></asp:Literal>
                                </asp:Panel>

                                <asp:Panel ID="ErrPanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                                    <asp:Literal ID="ErrMess" runat="server"></asp:Literal>
                                </asp:Panel>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <p><%=U4000.AUCTIONFOR %>: <%=target.DateStart.ToString() %></p>
                                <p><%=U4000.AUCTIONCLOSES %>: <b><%=target.Closes %></b></p>
                                <asp:Panel ID="PlaceBidPanel" runat="server">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=L1.CAMPAIGN %>:</label>
                                            <div class="col-md-6">
                                                <asp:DropDownList ID="ddlOptions" runat="server" CssClass="form-control"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <%--<div class="form-group">
                                        <div class="col-md-12">
                                            <asp:Image ID="CampaignImage" runat="server" />
                                        </div>
                                    </div>--%>
                                        <div class="form-group">
                                            <div class="col-md-2">
                                                <asp:Button ID="BidButton" runat="server"
                                                    ValidationGroup="BidValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="BidButton_Click" />
                                            </div>
                                        </div>
                                    </div>
                                </asp:Panel>
                                <asp:Literal ID="BidsLiteral" runat="server"></asp:Literal>
                            </div>
                            
                        </div>
                    </asp:Panel>
                    <%-- SUBPAGE END   --%>
                </div>
                    <titan:FeatureUnavailable runat="server" ID="AuctionsUnavailable"></titan:FeatureUnavailable>
                    </div>
            </asp:View>

            <asp:View runat="server" ID="asdasd">

                <div class="TitanViewElement">
                    <%-- SUBPAGE START --%>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:Panel ID="SuccPanel2" runat="server" Visible="false" CssClass="greenbox">
                                <asp:Literal ID="SuccLiteral" runat="server"></asp:Literal>
                            </asp:Panel>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:GridView ID="DirectRefsGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                                DataSourceID="SqlDataSource1" OnPreRender="BaseGridView_PreRender" OnRowDataBound="DirectRefsGridView_RowDataBound" 
                                DataKeyNames="BannerAdvertId" OnRowCommand="DirectRefsGridView_RowCommand">
                                <Columns>
                                    <asp:TemplateField HeaderText="Select" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkSelect" runat="server" />
                                        </ItemTemplate>
                                        <HeaderTemplate>
                                            <input type="checkbox" id="checkAll" class="regular-checkbox mini-checkbox" onclick="<%=this.jsSelectAllCode %>" /><label for="checkAll"></label>
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="BannerAdvertId" HeaderText="BannerAdvertId" SortExpression="BannerAdvertId" Visible="true" InsertVisible="False" ReadOnly="True" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone"></asp:BoundField>
                                    <asp:BoundField DataField="BannerAdvertPackId" HeaderText='BannerAdvertPackId' SortExpression="BannerAdvertPackId" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone"></asp:BoundField>
                                    <asp:BoundField DataField="TargetUrl" HeaderText='URL' SortExpression="TargetUrl"></asp:BoundField>
                                    <asp:BoundField DataField="ImagePath" HeaderText='<%$ ResourceLookup:IMAGE %>' SortExpression="ImagePath"></asp:BoundField>
                                    <asp:BoundField DataField="CreatedBy" HeaderText="CreatedBy" SortExpression="CreatedBy" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="CreatorUsername" HeaderText="CreatorUsername" SortExpression="CreatorUsername" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="CreatorEmail" HeaderText="CreatorEmail" SortExpression="CreatorEmail" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="CreationDate" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" HeaderText='<%$ ResourceLookup : CREATED %>' SortExpression="CreationDate" DataFormatString="{0:d}" />
                                    <asp:BoundField DataField="StartDate" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" HeaderText='<%$ ResourceLookup : PROGRESS %>' SortExpression="StartDate" ItemStyle-Width="150px" ItemStyle-Height="34px" />
                                    <asp:BoundField DataField="EndDate" HeaderText="%" SortExpression="EndDate" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="TotalSecsActive" HeaderText="TotalSecsActive" SortExpression="TotalSecsActive" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="Clicks" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" HeaderText='<%$ ResourceLookup : CLICKS %>' SortExpression="Clicks" />
                                    <asp:BoundField DataField="EndValue" HeaderText="EndValue" SortExpression="EndValue" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="EndMode" HeaderText="EndMode" SortExpression="EndMode" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="Status" HeaderStyle-Width="100px" HeaderText="Status" SortExpression="Status" />
                                    <asp:BoundField DataField="StatusLastChangedDate" HeaderText="StatusLastChangedDate" SortExpression="StatusLastChangedDate" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="Price" HeaderText="Price" SortExpression="Price" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />

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
                        </div>
                    </div>
                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>

            <asp:View ID="View1" runat="server" OnActivate="View1_Activate">
                <div id="view1h" class="TitanViewElement">
                    <%-- SUBPAGE START --%>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                                <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                            </asp:Panel>

                            <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
                                <asp:Literal ID="SuccMessage" runat="server"></asp:Literal>
                            </asp:Panel>
                            <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="alert alert-danger fade in m-b-15"
                                ValidationGroup="RegisterUserValidationGroup" DisplayMode="List" />
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger fade in m-b-15"
                                ValidationGroup="createBannerAdvertisement_OnSubmitValidationGroup" DisplayMode="List" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12" runat="server" id="BannersPlaceHolder">
                            <div class="form-horizontal">

                                <div class="form-group">
                                    <label class="control-label col-md-2"><%=L1.OPTION %>:</label>
                                    <div class="col-md-6">

                                        <div id="OptionRadioButtonDiv" runat="server">
                                            <asp:DropDownList ID="BannerTypeRadioButtonList" runat="server" CssClass="form-control" RepeatDirection="Vertical" RepeatLayout="Flow"
                                                AutoPostBack="true" OnSelectedIndexChanged="BannerTypeRadioButtonList_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </div>
                                        <asp:Label ID="OptionBanerAddNewLiteral" runat="server" CssClass="form-control no-border" Visible="false"></asp:Label>

                                    </div>
                                </div>

                                <div class="form-group">
                                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="URL" CssClass="control-label col-md-2">URL:</asp:Label>
                                    <div class="col-md-6">
                                        <asp:TextBox ID="URL" runat="server" CssClass="form-control" Text="http://"></asp:TextBox>

                                        <asp:RegularExpressionValidator Display="Dynamic" CssClass="text-danger" ValidationGroup="RegisterUserValidationGroup" ID="RegularExpressionValidator2" runat="server" ErrorMessage="*"
                                            ValidationExpression="http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&amp;=]*)?" ControlToValidate="URL" Text="">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="URL"
                                            ValidationGroup="RegisterUserValidationGroup" Display="Dynamic" CssClass="text-danger" Text="">*</asp:RequiredFieldValidator>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="control-label col-md-2"></label>
                                    <div class="col-md-6">
                                        <asp:Image ID="createBannerAdvertisement_BannerImage" runat="server" CssClass="img-responsive" />
                                    </div>
                                </div>


                                <div class="form-group">
                                    <label class="control-label col-md-2">
                                        <asp:Label ID="BannerImageLabel" runat="server" Text="Label"></asp:Label>:</label>

                                    <div class="col-md-6">
                                        <span class="btn btn-success fileinput-button">
                                            <i class="fa fa-plus"></i>
                                            <span><%=U6000.ADDFILE %></span>
                                            <asp:FileUpload ID="createBannerAdvertisement_BannerUpload" runat="server" onclick="hideURLBox();" />
                                        </span>
                                        <asp:Button ID="BannerUploadByUrlButton" Text="<%$ResourceLookup: ADDBANNERBYURL %>" runat="server" CssClass="btn btn-success fileinput-button" OnClientClick="showURLBox(); return false;" />

                                    </div>
                                    <div class="col-md-6 col-md-offset-2 m-t-15">
                                        <div class="input-group">
                                            <asp:TextBox ID="BannerFileUrlTextBox" runat="server" CssClass="form-control" Style="display: none"></asp:TextBox>
                                            <div class="input-group-btn">
                                                <asp:Button ID="createBannerAdvertisement_BannerUploadSubmit" Text="<%$ResourceLookup: SUBMIT %>" OnClick="createBannerAdvertisement_BannerUploadSubmit_Click"
                                                    CausesValidation="true" runat="server" ValidationGroup="createBannerAdvertisement_OnSubmitValidationGroup" CssClass="btn btn-primary" />
                                            </div>
                                        </div>

                                        <asp:CustomValidator runat="server" ID="createBannerAdvertisement_BannerUploadValidCustomValidator" OnServerValidate="ImageSubmitValidator_ServerValidate" Display="Dynamic" CssClass="text-danger"
                                            ValidationGroup="createBannerAdvertisement_OnSubmitValidationGroup">*</asp:CustomValidator>
                                        <asp:CustomValidator ID="createBannerAdvertisement_BannerUploadSelectedCustomValidator"
                                            ControlToValidate="createBannerAdvertisement_BannerUpload" Display="Dynamic" CssClass="text-danger"
                                            OnServerValidate="createBannerAdvertisement_BannerUploadSelectedCustomValidator_ServerValidate"
                                            ValidationGroup="RegisterUserValidationGroup" ValidateEmptyText="true"
                                            runat="server">*</asp:CustomValidator>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="control-label col-md-2"><%=L1.VERIFICATION %>:</label>
                                    <div class="col-md-6">
                                        <titan:Captcha runat="server" ID="TitanCaptcha" ValidationGroup="RegisterUserValidationGroup" class="form-control" />

                                        <asp:CustomValidator OnServerValidate="Validate_Captcha" runat="server" ValidationGroup="RegisterUserValidationGroup"
                                            Display="Dynamic" CssClass="text-danger" ID="CustomValidator1" EnableClientScript="False">*</asp:CustomValidator>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <div class="col-md-2">
                                        <asp:Button ID="CreateAdButton" runat="server"
                                            ValidationGroup="RegisterUserValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="CreateAdButton_Click"
                                            OnClientClick="if (Page_ClientValidate()){this.disabled = true; this.className = 'rbutton-loading'; this.value='';}"
                                            UseSubmitBehavior="false" />
                                    </div>

                                </div>

                                <%-- SUBPAGE END   --%>
                            </div>
                        </div>
                        <titan:FeatureUnavailable runat="server" ID="BannersUnavailable"></titan:FeatureUnavailable>
                    </div>
                </div>
            </asp:View>
        </asp:MultiView>
    </div>

    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" SelectCommand="SELECT * FROM [BannerAdverts] WHERE ([CreatorUsername] = @CreatorUsername) AND [Status] != 7 AND [BannerAdvertDimensionId] IN (SELECT [Id] FROM [BannerAdvertDimensions] WHERE [Status] = 1) ORDER BY [CreationDate] DESC">
        <SelectParameters>
            <asp:ControlParameter ControlID="UserName" Name="CreatorUsername" PropertyName="Text" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="AuctionGrid_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
        OnInit="AuctionGrid_DataSource_Init" />

    <asp:Label ID="UserName" runat="server" Visible="false"></asp:Label>
</asp:Content>
