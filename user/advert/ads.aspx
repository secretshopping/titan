<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="ads.aspx.cs" Inherits="About" %>

<%@ MasterType VirtualPath="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script type="text/javascript" src="Scripts/gridview.js"></script>

    <script type="text/javascript" src="../../Plugins/ColorPicker/jquery.minicolors.js"></script>
    <link rel="stylesheet" href="../../Plugins/ColorPicker/jquery.minicolors.css?3s=d" />

    <script type="text/javascript">
        function pageLoad() {
            ManageGeoEvent();
        }

        function ManageGeoEvent() {
            if ($('#<%=chbGeolocation.ClientID%>').is(':checked')) {
                $('#geoPanels').show();
            }
            else {
                $('#geoPanels').hide();
            }
        }

        function fixCSS() {
            $('#view1h').removeClass('TitanViewElement');
            $('#view1h').addClass('TitanViewElementSimple');
        }

        function CheckURL() {
            $('#__EVENTARGUMENT5').val($('#<%=URL.ClientID %>').val()); //Set URL to validate

            //Send request
            $('#<%=Form.ClientID%>').attr('action', 'user/earn/surf.aspx?f=3');

            $('#<%=Form.ClientID%>').attr('target', '_blank');
            $('#<%=Form.ClientID%>').submit();

            $('#<%=Form.ClientID%>').attr('action', 'user/advert/ads.aspx<%=EditId %>');

            $('#<%=Form.ClientID%>').attr('target', '_self');
            return false;
        }

    </script>

    <link rel="stylesheet" href="Styles/WatchAds.css?sf=1" type="text/css" />

</asp:Content>


<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">


    <h1 class="page-header">
        <asp:Literal ID="TitleLiteral" runat="server"></asp:Literal>
    </h1>

    <div class="row">
        <div class="col-md-12">
            <p class="lead">
                <asp:Literal ID="SubLiteral" runat="server"></asp:Literal>
            </p>
        </div>
    </div>


    <input type="hidden" name="__EVENTARGUMENT5" id="__EVENTARGUMENT5" value="" />

    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <asp:Panel ID="TitanViewPagePanel" runat="server" CssClass="TitanViewPage">
                    <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                        <asp:Button ID="Button2" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                        <asp:Button ID="Button1" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                    </asp:PlaceHolder>
                </asp:Panel>
            </div>
        </div>
    </div>
    <div class="tab-content">
        <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
            <asp:View runat="server" ID="manageView" OnActivate="manageView_Activate">
                <div class="TitanViewElement">
                    <%-- SUBPAGE START --%>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:Panel ID="ErrorMessagePanel2" runat="server" Visible="false" CssClass="alert alert-danger">
                                <asp:Literal ID="ErrorMessage2" runat="server"></asp:Literal>
                            </asp:Panel>
                            <asp:PlaceHolder runat="server" ID="MaxActiveCampaignsPlaceHolder">
                                <div class="alert alert-warning">
                                    <asp:Literal runat="server" ID="MaxActiveCampaignsLiteral"></asp:Literal>
                                </div>
                            </asp:PlaceHolder>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            <div class="table-responsive">
                                <asp:GridView ID="DirectRefsGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                                    DataSourceID="SqlDataSource1" OnRowDataBound="DirectRefsGridView_RowDataBound"
                                    DataKeyNames="PtcAdvertId" OnDataBound="DirectRefsGridView_DataBound"
                                    OnRowCommand="DirectRefsGridView_RowCommand" EmptyDataText="<%$ ResourceLookup : NOADCAMPAIGNS %>" OnPreRender="BaseGridView_PreRender">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Select" HeaderStyle-Width="26px" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone">
                                            
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="PtcAdvertId" HeaderText="PtcAdvertId" SortExpression="PtcAdvertId" Visible="true" InsertVisible="False" ReadOnly="True" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone"></asp:BoundField>
                                        <asp:BoundField DataField="PtcCategoryId" HeaderText='PtcCategoryId' SortExpression="PtcCategoryId" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone"></asp:BoundField>
                                        <asp:BoundField DataField="PtcAdvertPackId" HeaderText='PtcAdvertPackId' SortExpression="PtcAdvertPackId" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone"></asp:BoundField>
                                        <asp:BoundField DataField="TargetUrl" HeaderText='URL' SortExpression="TargetUrl" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone"></asp:BoundField>
                                        <asp:BoundField DataField="Title" HeaderText='<%$ ResourceLookup : TITLE %>' SortExpression="Title" ItemStyle-Width="80px"></asp:BoundField>
                                        <asp:BoundField DataField="Description" HeaderText='<%$ ResourceLookup : DESC %>' SortExpression="Description" ItemStyle-Width="11px" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                        <asp:BoundField DataField="CreatedBy" HeaderText="CreatedBy" SortExpression="CreatedBy" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                        <asp:BoundField DataField="CreatorUsername" HeaderText="CreatorUsername" SortExpression="CreatorUsername" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                        <asp:BoundField DataField="CreatorEmail" HeaderText="CreatorEmail" SortExpression="CreatorEmail" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                        <asp:BoundField DataField="CreationDate" HeaderText='<%$ ResourceLookup : CREATED %>' SortExpression="CreationDate" DataFormatString="{0:d}" />
                                        <asp:BoundField DataField="StartDate" HeaderText='<%$ ResourceLookup : PROGRESS %>' SortExpression="StartDate" />
                                        <asp:BoundField DataField="PointsEarnedFromViews" HeaderText='PointsEarnedFromViews' SortExpression="PointsEarnedFromViews" />
                                        <asp:BoundField DataField="EndDate" HeaderText="%" SortExpression="EndDate" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                        <asp:BoundField DataField="TotalSecsActive" HeaderText='<%$ ResourceLookup : VIEWSBIG %>' SortExpression="TotalSecsActive" />
                                        <asp:BoundField DataField="Clicks" HeaderText="Clicks" SortExpression="Clicks" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                        <asp:BoundField DataField="EndValue" HeaderText="EndValue" SortExpression="EndValue" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                        <asp:BoundField DataField="EndMode" HeaderText="EndMode" SortExpression="EndMode" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                        <asp:BoundField DataField="DisplayTimeSeconds" HeaderText='<%$ ResourceLookup : TIME %>' SortExpression="DisplayTimeSeconds" />
                                        <asp:BoundField DataField="ClickValue" HeaderText="ClickValue" SortExpression="ClickValue" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                        <asp:BoundField DataField="DirectReferralClickValue" HeaderText="DirectReferralClickValue" SortExpression="DirectReferralClickValue" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                        <asp:BoundField DataField="RentedReferralClickValue" HeaderText="RentedReferralClickValue" SortExpression="RentedReferralClickValue" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />

                                        <asp:BoundField DataField="MinMembershipId" HeaderText="MinMembershipId" SortExpression="MinMembershipId" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                        <asp:BoundField DataField="Description" HeaderText='<%$ ResourceLookup : DESC %>' SortExpression="Description" ItemStyle-Width="13px" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                        <asp:CheckBoxField DataField="IsGeolocated" HeaderText="GEO" SortExpression="IsGeolocated" ItemStyle-Width="13px" />

                                        <asp:BoundField DataField="Status" HeaderText='<%$ ResourceLookup : STATUS %>' SortExpression="Status" />
                                        <asp:BoundField DataField="StatusLastChangedDate" HeaderText="StatusLastChangedDate" SortExpression="StatusLastChangedDate" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                        <asp:BoundField DataField="Price" HeaderText="Price" ItemStyle-Width="25px" SortExpression="Price" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />

                                        <asp:BoundField DataField="CaptchaQuestion" HeaderText="<%$ ResourceLookup : CAPTCHAQUESTION %>" ItemStyle-Width="80px" SortExpression="CaptchaQuestion" />


                                        <asp:TemplateField>
                                            <ItemStyle />
                                            <HeaderTemplate>
                                                <span class="fa fa-thumbs-up fa-lg text-success"></span>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <%# Eval("CaptchaYesAnswers") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField>
                                            <ItemStyle />
                                            <HeaderTemplate>
                                                <span class="fa fa-thumbs-down fa-lg text-danger"></span>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <%# Eval("CaptchaNoAnswers") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="">
                                            <ItemStyle />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="ImageButton1" runat="server"
                                                    ToolTip='Start'
                                                    CommandName="start"
                                                    CommandArgument='<%# Container.DataItemIndex %>'>
                                                <span class="fa fa-play fa-lg text-success"></span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="">
                                            <ItemStyle />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="ImageButton2" runat="server"
                                                    ToolTip='<%$ ResourceLookup : PAUSE %>'
                                                    CommandName="stop"
                                                    CommandArgument='<%# Container.DataItemIndex %>'>
                                                <span class="fa fa-pause fa-lg text-warning"></span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="">
                                            <ItemStyle />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="ImageButton3" runat="server"
                                                    ToolTip='<%$ ResourceLookup : BUYCREDITS %>'
                                                    CommandName="add"
                                                    CommandArgument='<%# Container.DataItemIndex %>'>
                                                <span class="fa fa-plus fa-lg text-success"></span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>


                                        <asp:TemplateField HeaderText="">
                                            <ItemStyle />
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server"
                                                    ToolTip='<%$ ResourceLookup : REMOVE %>'
                                                    CommandName="remove"
                                                    CommandArgument='<%# Container.DataItemIndex %>'>
                                                <spin class="fa fa-times fa-lg text-danger"></spin>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="">
                                            <ItemStyle />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="ImageButton4" runat="server"
                                                    CommandName="edit"
                                                    CommandArgument='<%# Container.DataItemIndex %>'>
                                                <span class="fa fa-arrow-right fa-lg text-info"></span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>


                    <asp:Panel ID="SelectedPanel" runat="server" CssClass="displaynone">
                        <%=L1.SELECTED %>: 
                    <asp:LinkButton runat="server" ToolTip='<%$ ResourceLookup : REMOVE %>'>
                        <span class="fa fa-times text-danger"></span>
                    </asp:LinkButton>
                    </asp:Panel>

                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>
            <asp:View ID="View1" runat="server" OnActivate="View1_Activate">
                <asp:UpdatePanel runat="server" ID="MultiViewUpdatePanel">
                    <Triggers>
                        <asp:PostBackTrigger ControlID="PTCImage_UploadSubmit" />
                    </Triggers>
                    <ContentTemplate>
                        <div id="view1h" class="TitanViewElement">
                            <%-- SUBPAGE START --%>
                            <div class="row">
                                <div class="col-md-12">
                                    <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger">
                                        <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                                    </asp:Panel>

                                    <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="alert alert-success">
                                        <asp:Literal ID="SuccMessage" runat="server"></asp:Literal>
                                    </asp:Panel>
                                    <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="alert alert-danger"
                                        ValidationGroup="RegisterUserValidationGroup" DisplayMode="List" />
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-12" runat="server" id="AdsPlaceHolder">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=L1.PREVIEW %>:</label>
                                            <div class="col-md-3 ptc-preview">
                                                <titan:PtcAdvert runat="server" IsPreview="true" ID="Abox1"></titan:PtcAdvert>
                                            </div>
                                        </div>
                                        <div id="titleTr" runat="server" class="form-group">
                                            <asp:Label ID="TitleLabel" runat="server" CssClass="control-label col-md-2" AssociatedControlID="Title"><%=L1.TITLE %>:</asp:Label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="Title" runat="server" CssClass="form-control" MaxLength="100" OnTextChanged="RefreshAdvertAndPrice" AutoPostBack="true"></asp:TextBox>
                                                <asp:RegularExpressionValidator ValidationGroup="RegisterUserValidationGroup" ID="RegularExpressionValidator1" runat="server"
                                                    ValidationExpression="[^\n\r\t]{3,30}" Display="Dynamic" CssClass="text-danger" ControlToValidate="Title" Text="">*</asp:RegularExpressionValidator>
                                                <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="Title"
                                                    Display="Dynamic" CssClass="text-danger" Text=""
                                                    ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label ID="PasswordLabel" runat="server" CssClass="control-label col-md-2" AssociatedControlID="URL">URL:</asp:Label>
                                            <div class="col-md-6">
                                                <asp:UpdatePanel runat="server" ID="AddNewAdWithURLCheck" OnLoad="AddNewAdWithURLCheck_Load" ClientIDMode="Static">
                                                    <ContentTemplate>
                                                        <div class="input-group" runat="server" id="UrlWrapper">
                                                            <asp:TextBox ID="URL" runat="server" CssClass="form-control" Text="http://" MaxLength="800"></asp:TextBox>
                                                            <div class="input-group-btn">
                                                                <asp:LinkButton ID="CheckURLButton" runat="server" OnClientClick="CheckURL()" CssClass="btn btn-primary"><%=U4200.CHECKURLTEXT %></asp:LinkButton>
                                                                <asp:Image ImageUrl="../../Images/Misc/youtube.png" runat="server" ID="YouTubeImage" Visible="false" Width="24px" Style="vertical-align: middle; margin-left: 10px;" />
                                                            </div>
                                                        </div>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                                <asp:RegularExpressionValidator Display="Dynamic" CssClass="text-danger" ValidationGroup="RegisterUserValidationGroup" ID="RegularExpressionValidator2" runat="server" ErrorMessage="*"
                                                    ControlToValidate="URL" Text="">*</asp:RegularExpressionValidator>
                                                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="URL"
                                                    Display="Dynamic" CssClass="text-danger"
                                                    ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <%--    Only for outside advertising--%>
                                        <asp:PlaceHolder ID="OutEmailPlaceHolder" runat="server" Visible="false">
                                            <div class="form-group">
                                                <label class="control-label col-md-2">Email:</label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="OutEmail" runat="server" CssClass="form-control"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ValidationGroup="RegisterUserValidationGroup" ID="OutEmailRegularExpressionValidator" runat="server"
                                                        ValidationExpression="^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$" Display="Dynamic" CssClass="text-danger" ControlToValidate="OutEmail" Text="">*</asp:RegularExpressionValidator>
                                                    <asp:RequiredFieldValidator ID="OutEmailRequiredFieldValidator" runat="server" ControlToValidate="OutEmail"
                                                        Display="Dynamic" CssClass="text-danger" Text=""
                                                        ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                        </asp:PlaceHolder>
                                        <%--    Only for outside advertising--%>
                                        <div runat="server" id="CategoriesDropDownPlaceHolder" visible="false" class="form-group">
                                            <label class="control-label col-md-2"><%=U5006.CATEGORY %>:</label>
                                            <div class="col-md-6">
                                                <asp:DropDownList ID="CategoriesDDL" runat="server" CssClass="form-control ddl"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div runat="server" id="BoxSizePlaceHolder" visible="false" class="form-group">
                                            <label class="control-label col-md-2"></label>
                                            <div class="col-md-6">
                                            </div>
                                        </div>                                       
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=L1.PACK %>:</label>
                                            <div class="col-md-6">
                                                <asp:DropDownList ID="ddlOptions" runat="server" CssClass="ddl form-control" OnTextChanged="RefreshAdvertAndPrice" AutoPostBack="true"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <asp:PlaceHolder ID="PTCImagePlaceHolder" runat="server" Visible="false">
                                            <div class="form-group">
                                                <div class="col-md-6 col-md-offset-2">
                                                    <asp:Image ID="PTCImage_Image" runat="server" CssClass="img-responsive" />
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label class="control-label col-md-2"><%=L1.IMAGE %>:</label>
                                                <div class="col-md-6">
                                                    <span class="btn btn-success fileinput-button">
                                                        <i class="fa fa-plus"></i>
                                                        <span><%=U6000.ADDFILE %></span>
                                                        <asp:FileUpload ID="PTCImage_Upload" runat="server" />
                                                    </span>
                                                    <asp:Button ID="PTCImage_UploadSubmit" Text="<%$ResourceLookup: SUBMIT %>" OnClick="createPTCImage_UploadSubmit_Click"
                                                        CausesValidation="true" runat="server" ValidationGroup="PTCImageSubmitValidationGroup" CssClass="btn btn-primary" />
                                                    <asp:CustomValidator ID="PTCImageSubmitValidator" ControlToValidate="PTCImage_Upload" Display="Dynamic" CssClass="text-danger"
                                                        OnServerValidate="PTCImageSubmitValidator_ServerValidate" ValidationGroup="PTCImageSubmitValidationGroup" ValidateEmptyText="true"
                                                        runat="server" />
                                                    <br />
                                                    <asp:CustomValidator ID="PTCImageValidator" Display="Dynamic" CssClass="text-danger"
                                                        OnServerValidate="PTCImageValidator_ServerValidate" ValidationGroup="RegisterUserValidationGroup" ValidateEmptyText="true"
                                                        runat="server" />
                                                </div>
                                            </div>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder runat="server" ID="BuyWithPTCCreditsPlaceHolder" Visible="false">
                                            <div class="form-group">
                                                <label class="control-label col-md-2"><%=U5006.USEEXTRAVIEWS %>:</label>
                                                <div class="col-md-6">
                                                    <div class="checkbox">
                                                        <label>
                                                            <asp:CheckBox runat="server" ID="UseExtraViewsCheckBox" AutoPostBack="true" OnCheckedChanged="UseExtraViewsCheckBox_CheckedChanged" />
                                                            <%=L1.YES %>
                                                        </label>
                                                    </div>
                                                </div>
                                            </div>
                                            <div runat="server" id="BuyWithPTCCreditsPlaceHolder2" class="form-group">
                                                <label class="control-label col-md-2"><%=L1.VIEWSBIG %>:</label>
                                                <div class="col-md-2">
                                                    <asp:TextBox runat="server" ID="PTCCreditsTextBox" CssClass="form-control" MaxLength="8"
                                                        OnTextChanged="PTCCreditsTextBox_TextChanged" AutoPostBack="true" Text="1"></asp:TextBox>
                                                    <span class="help-block">
                                                        <asp:Literal runat="server" ID="AvailablePTCCreditsLiteral"></asp:Literal>
                                                    </span>
                                                </div>
                                            </div>
                                        </asp:PlaceHolder>
                                        <div id="descriptionTr" runat="server" class="form-group">
                                            <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Description" CssClass="control-label col-md-2"><%=L1.DESCRIPTION %>:</asp:Label>
                                            <div class="col-md-1">
                                                <div class="checkbox">
                                                    <label>
                                                        <asp:CheckBox ID="chbDescription" runat="server" Checked="false" AutoPostBack="true" OnCheckedChanged="chbDescription_CheckedChanged" />
                                                        <asp:Literal ID="PriceDesc" runat="server"></asp:Literal>
                                                    </label>
                                                </div>
                                            </div>
                                            <div class="col-md-5">
                                                <asp:TextBox ID="Description" runat="server" TextMode="MultiLine" CssClass="form-control" MaxLength="70" Rows="5" OnTextChanged="RefreshAdvertAndPrice" AutoPostBack="true" Visible="false"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group" id="GeolocationPlaceHolder" runat="server">
                                            <asp:Label ID="lblGeolocation" runat="server" AssociatedControlID="chbGeolocation" CssClass="control-label col-md-2"><%=L1.GEOLOCATION %>*:</asp:Label>
                                            <div class="col-md-6">
                                                <div class="checkbox">
                                                    <label>
                                                        <asp:CheckBox ID="chbGeolocation" runat="server" Checked="false" OnCheckedChanged="RefreshAdvertAndPrice" AutoPostBack="true" />
                                                        <asp:Literal ID="PriceGeo" runat="server"></asp:Literal>
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
                                                    <input type="hidden" id="GeoCountriesValues" name="GeoCountriesValues" runat="server" clientidmode="Static">
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
                                        <div id="boldTr" runat="server" class="form-group">
                                            <label class="control-label col-md-2"><%=L1.FONTBOLD %>:</label>
                                            <div class="col-md-6">
                                                <div class="checkbox">
                                                    <label>
                                                        <asp:CheckBox ID="chbBold" runat="server" Checked="false" OnCheckedChanged="RefreshAdvertAndPrice" AutoPostBack="true" />
                                                        <asp:Literal ID="PriceBold" runat="server"></asp:Literal>
                                                    </label>
                                                </div>
                                            </div>
                                        </div>
                                        <div runat="server" id="StarredAdPlaceHolder" visible="false" class="form-group">
                                            <label class="control-label col-md-2">
                                                <asp:Label ID="lblStarredAd" runat="server"></asp:Label>*:</label>
                                            <div class="col-md-6">
                                                <div class="checkbox">
                                                    <label>
                                                        <asp:CheckBox ID="StarredAdCheckBox" runat="server" Checked="false" OnCheckedChanged="RefreshAdvertAndPrice" AutoPostBack="true" />
                                                        <%=AppSettings.PtcAdverts.StarredAdsPrice %>
                                                    </label>
                                                </div>
                                            </div>
                                        </div>
                                        <div runat="server" id="BackgroundColorPlaceHolder" visible="false" class="form-group">
                                            <label class="control-label col-md-2"><%=U5006.BACKGROUNDCOLOR %>:</label>
                                            <div class="col-md-1">
                                                <div class="checkbox">
                                                    <label>
                                                        <asp:CheckBox runat="server" ID="BackgroundColorCheckBox" AutoPostBack="true" OnCheckedChanged="BackgroundColorCheckBox_CheckedChanged" />
                                                    </label>
                                                </div>
                                            </div>
                                            <div class="col-md-2">
                                                <asp:DropDownList OnInit="BindDataBgColorsToDDL" OnDataBound="BgColorsDDL_DataBound" ID="BgColorsDDL" runat="server"
                                                    CssClass="form-control ddl" AutoPostBack="true" OnSelectedIndexChanged="BgColorsDDL_SelectedIndexChanged" Visible="false">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div runat="server" id="CustomCaptchaPlaceHolder" visible="false" class="form-group">
                                            <label class="control-label col-md-2"><%=U6013.YESNOCAPTCHAQUESTION %>:</label>
                                            <div class="col-md-1">
                                                <div class="checkbox">
                                                    <label>
                                                        <asp:CheckBox ID="CaptchaQuestionCheckBox" OnCheckedChanged="CaptchaQuestionCheckBox_CheckedChanged" runat="server" AutoPostBack="true" />
                                                        <%=L1.YES %>
                                                    </label>
                                                </div>
                                            </div>
                                            <div class="col-md-2">
                                                <asp:TextBox runat="server" ID="CaptchaQuestionTexbox" MaxLength="80" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <h4><%=L1.PRICE %>:
                                                <asp:Label ID="lblPrice" runat="server" Text="Label"></asp:Label>
                                            </h4>
                                        </div>
                                        
                                        <asp:PlaceHolder ID="CashbackInfoPlaceHolder" runat="server" Visible="false">
                                            <div class="form-group">
                                                <h4><%=U6010.CASHBACK %>: <asp:Label ID="CashBackLabel" runat="server" Text="Label"></asp:Label></h4><br />
                                                <%=U6010.CASHBACKINFO %><br /><br />
                                            </div>
                                        </asp:PlaceHolder>

                                        <div runat="server" id="TotalViewsPlaceHolder" class="form-group">
                                            <h4><%=L1.VIEWSBIG %>:
                                                <asp:Label ID="lblViews" runat="server" Text="?"></asp:Label>
                                            </h4>
                                            <h4><%=U5008.TOTALVIEWS %>:
                                                <asp:Label ID="lblTotalViews" runat="server" Text="?"></asp:Label>
                                            </h4>
                                        </div>
                                        <titan:TargetBalance runat="server" Feature="PTC" ID="TargetBalanceRadioButtonList"></titan:TargetBalance>
                                        <div class="form-group">
                                            <div class="col-md-2">
                                                <asp:Button ID="CreateAdButton" runat="server"
                                                    ValidationGroup="RegisterUserValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="CreateAdButton_Click"
                                                    UseSubmitBehavior="false" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="col-md-12">
                                                <asp:Literal ID="PaymentButtons" runat="server" Visible="false"></asp:Literal>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <titan:FeatureUnavailable runat="server" ID="NewAdsWebsiteUnavailable"></titan:FeatureUnavailable>
                            </div>
                            <%-- SUBPAGE END   --%>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:View>
        </asp:MultiView>
    </div>


    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" SelectCommand="SELECT * FROM [PtcAdverts] WHERE ([CreatorUsername] = @CreatorUsername) AND [Status] != 7 ORDER BY [StartDate] DESC">
        <SelectParameters>
            <asp:ControlParameter ControlID="UserName" Name="CreatorUsername" PropertyName="Text" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:Label ID="UserName" runat="server" Visible="false"></asp:Label>


</asp:Content>
