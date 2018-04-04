<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="banners.aspx.cs" Inherits="About" %>

<%@ MasterType VirtualPath="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">


    <script type="text/javascript" src="Scripts/gridview.js"></script>

    <script type="text/javascript">
        jQuery(function ($) {
            ManageGeoEvent();
            updatePrice();
        });

        function updatePrice() {
            //Get values
            var price1 = 0.0;
            var price2 = 0.0;
            var price3 = 0.0;
            var price4 = 0.0;

            if ($('#<%=chbGeolocation.ClientID%>').is(':checked'))
                price3 = parseFloat($('#<%=chbGeolocation.ClientID%>').parent().parent().text().trim().replace('<%=Prem.PTC.AppSettings.Site.CurrencySign%>',''));

            var selectStringIndex = $('#<%=ddlOptions.ClientID%> option:selected').text().indexOf('-') + 1;
            price4 = parseFloat($('#<%=ddlOptions.ClientID%> option:selected').text().substring(selectStringIndex).replace('<%=Prem.PTC.AppSettings.Site.CurrencySign%>',''))

            var totalPrice = price1 + price2 + price3 + price4;

            $('#<%=lblPrice.ClientID%>').text(totalPrice);
        }

        function fixCSS() {
            $('#view1h').removeClass('TitanViewElement');
            $('#view1h').addClass('TitanViewElementSimple');
        }

        function CheckGeo() {
            if ($('#<%=chbGeolocation.ClientID%>').is(':checked')) {

                        var list = "#";

                        $('#<%=GeoCountries.ClientID%> > option').each(function () {
                            // add $(this).val() to your list
                            list = list + $(this).val() + "#";
                        });

                        $('#<%=GeoCountriesValues.ClientID%>').val(list);
            }
        }

        function ManageGeoEvent() {
            if ($('#<%=chbGeolocation.ClientID%>').is(':checked')) {
                $('#geoPanels').show();
            }
            else {
                $('#geoPanels').hide();
            }
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
    <script type="text/javascript">

        function pageLoad() {

            ManageGeoEvent();
            updatePrice();

                <%=PageScriptGenerator.GetGridViewCode(DirectRefsGridView) %>

                                $('#<%=btnAdd.ClientID%>').click(
                    function (e) {
                        $('#<%=AllCountries.ClientID%> > option:selected').appendTo('#<%=GeoCountries.ClientID%>');
                        e.preventDefault();
                    });

                $('#<%=btnAddAll.ClientID%>').click(
                function (e) {
                    $('#<%=AllCountries.ClientID%> > option').appendTo('#<%=GeoCountries.ClientID%>');
                    e.preventDefault();
                });

                $('#<%=btnRemove.ClientID%>').click(
                function (e) {
                    $('#<%=GeoCountries.ClientID%> > option:selected').appendTo('#<%=AllCountries.ClientID%>');
                    e.preventDefault();
                });

                $('#<%=btnRemoveAll.ClientID%>').click(
                function (e) {
                    $('#<%=GeoCountries.ClientID%> > option').appendTo('#<%=AllCountries.ClientID%>');
                    e.preventDefault();
                });
            }
    </script>
    <link rel="stylesheet" href="Styles/WatchAds.css" type="text/css" />

</asp:Content>





<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <asp:UpdateProgress runat="server" AssociatedUpdatePanelID="BannersUpdatePanel"></asp:UpdateProgress>
    <asp:UpdatePanel runat="server" ID="BannersUpdatePanel">
        <Triggers>
            <asp:PostBackTrigger ControlID="createBannerAdvertisement_BannerUploadSubmit" />
        </Triggers>
        <ContentTemplate>


            <h1 class="page-header"><%=  L1.BANNERS %></h1>

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
                    <asp:View runat="server" ID="asdasd">

                        <div class="TitanViewElement">
                            <%-- SUBPAGE START --%>

                            <div class="row">
                                <div class="col-md-12">
                                    <asp:Panel ID="SuccPanel2" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
                                        <asp:Literal ID="SuccLiteral" runat="server"></asp:Literal>
                                    </asp:Panel>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-12">
                                    <asp:GridView ID="DirectRefsGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                                        DataSourceID="SqlDataSource1" OnRowDataBound="DirectRefsGridView_RowDataBound" 
                                        DataKeyNames="BannerAdvertId" OnPreRender="BaseGridView_PreRender" OnRowCommand="DirectRefsGridView_RowCommand" EmptyDataText="<%$ ResourceLookup : NOBANNERCAMPS %>">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Select" HeaderStyle-Width="26px" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone">
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
                                            <asp:BoundField DataField="TargetUrl" HeaderText="TargetUrl" SortExpression="TargetUrl" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                            <asp:BoundField DataField="CreatedBy" HeaderText="CreatedBy" SortExpression="CreatedBy" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                            <asp:BoundField DataField="CreatorUsername" HeaderText="CreatorUsername" SortExpression="CreatorUsername" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                            <asp:BoundField DataField="CreatorEmail" HeaderText="CreatorEmail" SortExpression="CreatorEmail" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                            <asp:BoundField DataField="CreationDate" HeaderText='<%$ ResourceLookup : CREATED %>' SortExpression="CreationDate" DataFormatString="{0:d}" />
                                            <asp:BoundField DataField="StartDate" HeaderText='<%$ ResourceLookup : PROGRESS %>' SortExpression="StartDate" ItemStyle-Width="150px" ItemStyle-Height="34px" />
                                            <asp:BoundField DataField="EndDate" HeaderText="%" SortExpression="EndDate" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                            <asp:BoundField DataField="TotalSecsActive" HeaderText="TotalSecsActive" SortExpression="TotalSecsActive" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                            <asp:BoundField DataField="Clicks" HeaderText='<%$ ResourceLookup : CLICKS %>' SortExpression="Clicks" />
                                            <asp:BoundField DataField="EndValue" HeaderText="EndValue" SortExpression="EndValue" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                            <asp:BoundField DataField="EndMode" HeaderText="EndMode" SortExpression="EndMode" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                            <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                                            <asp:BoundField DataField="StatusLastChangedDate" HeaderText="StatusLastChangedDate" SortExpression="StatusLastChangedDate" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                            <asp:BoundField DataField="Price" HeaderText="Price" SortExpression="Price" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                            <asp:BoundField DataField="Price" HeaderText="GEO" SortExpression="Price" />
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
                                        ValidationGroup="RegisterUserValidationGroup" DisplayMode="List" ForeColor="White" />
                                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger fade in m-b-15"
                                        ValidationGroup="createBannerAdvertisement_OnSubmitValidationGroup" DisplayMode="List" ForeColor="White" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12 form-horizontal" runat="server" id="BannersPlaceHolder">

                                    <div class="form-group">
                                        <label class="col-md-2 control-label"><%=U6000.DIMENSIONS %>:</label>
                                        <div class="col-md-6">
                                            <div class="input-group">
                                                <div class="radio radio-button-list">
                                                    <asp:DropDownList ID="BannerTypeRadioButtonList" runat="server" CssClass="form-control"
                                                        RepeatLayout="Flow" AutoPostBack="true" OnSelectedIndexChanged="BannerTypeRadioButtonList_SelectedIndexChanged">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="URL" CssClass="control-label col-md-2">URL:</asp:Label>
                                        <div class="col-md-6">
                                            <asp:TextBox ID="URL" runat="server" CssClass="form-control" Text="http://"></asp:TextBox>

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
                                            <asp:Label ID="Label1" runat="server" AssociatedControlID="URL" CssClass="control-label col-md-2">Email:</asp:Label>
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

                                    <div class="form-group">
                                        <label class="control-label col-md-2"></label>
                                        <div class="col-md-6">
                                            <asp:Image ID="createBannerAdvertisement_BannerImage" runat="server" CssClass="img-responsive" />
                                        </div>
                                    </div>

                                    <div class="form-group" runat="server">
                                        <label class="col-md-2 control-label">
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
                                            <asp:CustomValidator ID="createBannerAdvertisement_BannerUploadValidCustomValidator"
                                                ControlToValidate="createBannerAdvertisement_BannerUpload" Display="Dynamic" CssClass="text-danger"
                                                OnServerValidate="createBannerAdvertisement_BannerUploadValidCustomValidator_ServerValidate"
                                                ValidationGroup="createBannerAdvertisement_OnSubmitValidationGroup" ValidateEmptyText="true"
                                                runat="server">*</asp:CustomValidator>
                                            <asp:CustomValidator ID="createBannerAdvertisement_BannerUploadSelectedCustomValidator"
                                                ControlToValidate="createBannerAdvertisement_BannerUpload" Display="Dynamic" CssClass="text-danger"
                                                OnServerValidate="createBannerAdvertisement_BannerUploadSelectedCustomValidator_ServerValidate"
                                                ValidationGroup="RegisterUserValidationGroup" ValidateEmptyText="true"
                                                runat="server">*</asp:CustomValidator>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label class="control-label col-md-2"><%=L1.PACK %>:</label>
                                        <div class="col-md-6">
                                            <asp:DropDownList  ID="ddlOptions" runat="server" CssClass="form-control"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="form-group" id="GeolocationPlaceHolder" runat="server">
                                        <label class="col-md-2 control-label">
                                            <asp:Label ID="lblGeolocation" runat="server" Text="Geolocation *"></asp:Label>:</label>
                                        <div class="col-md-6">
                                            <div class="checkbox">
                                                <label>
                                                    <asp:CheckBox ID="chbGeolocation" runat="server" Checked="false" />
                                                    <asp:Literal ID="PriceGeo" runat="server"></asp:Literal>
                                                </label>
                                            </div>
                                        </div>
                                    </div>

                                    <div id="geoPanels" style="display: none">

                                        <div class="form-group">
                                            <div class="col-md-6 col-md-offset-2">
                                                <asp:ListBox ID="AllCountries" runat="server" CssClass="form-control"></asp:ListBox>
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <div class="col-md-6 col-md-offset-2">
                                                <div class="input-group-btn">
                                                    <asp:Button ID="btnAdd" CssClass="titab titab1 btn" runat="server" Text="&#8595;" />
                                                    <asp:Button ID="btnRemove" CssClass="titab titab3 btn" runat="server" Text="&#8593;" />
                                                    <asp:Button ID="btnAddAll" CssClass="titab titab2 btn" runat="server" Text="&#8595;&#8595;" />
                                                    <asp:Button ID="btnRemoveAll" CssClass="titab titab4 btn" runat="server" Text="&#8593;&#8593;" />
                                                </div>
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <div class="col-md-6 col-md-offset-2">
                                                <asp:ListBox ID="GeoCountries" runat="server" CssClass="form-control"></asp:ListBox>
                                                <input type="hidden" id="GeoCountriesValues" name="GeoCountriesValues" runat="server" clientidmode="Static">
                                            </div>
                                        </div>

                                    </div>
                                    <div class="form-group">
                                        <div class="col-md-12">
                                            <h4><%=L1.PRICE %>: <%=Prem.PTC.AppSettings.Site.CurrencySign %><asp:Label ID="lblPrice" runat="server" ></asp:Label></h4>
                                        </div>
                                    </div>
                                    <titan:TargetBalance runat="server" Feature="Banner" ID="TargetBalanceRadioButtonList"></titan:TargetBalance>
                                    <div class="form-group">
                                        <div class="col-md-2">
                                            <asp:Button ID="CreateAdButton" runat="server"
                                                ValidationGroup="RegisterUserValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="CreateAdButton_Click"
                                                OnClientClick="CheckGeo(); if (Page_ClientValidate()){this.disabled = true; this.className = 'rbutton-loading'; this.value='';}"
                                                UseSubmitBehavior="false" />
                                            
                                        </div>
                                        <div class="col-md-12">
                                            <asp:Literal ID="PaymentButtons" runat="server" Visible="false"></asp:Literal>
                                        </div>
                                    </div>

                                </div>
                                <titan:FeatureUnavailable runat="server" ID="BannersUnavailable"></titan:FeatureUnavailable>
                            </div>
                            <%-- SUBPAGE END   --%>
                        </div>
                    </asp:View>
                </asp:MultiView>
            </div>

            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" SelectCommand="SELECT * FROM [BannerAdverts] WHERE ([CreatorUsername] = @CreatorUsername) AND [Status] != 7 ORDER BY [CreationDate] DESC">
                <SelectParameters>
                    <asp:ControlParameter ControlID="UserName" Name="CreatorUsername" PropertyName="Text" Type="String" />
                </SelectParameters>
            </asp:SqlDataSource>
            <asp:Label ID="UserName" runat="server" Visible="false"></asp:Label>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
