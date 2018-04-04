<%@ Page Language="C#" AutoEventWireup="true" CodeFile="paidtopromote.aspx.cs" Inherits="user_advert_paidtopromote" MasterPageFile="~/User.master" %>

<%@ MasterType VirtualPath="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script type="text/javascript" src="Scripts/gridview.js"></script>

    <script type="text/javascript">

        function pageLoad() {
            ManageGeoEvent();
            updatePrice();
        };        

        function ManageGeoEvent() {
            if ($('#<%=chbGeolocation.ClientID%>').is(':checked')) {
                $('#geoPanels').show();
            }
            else {
                $('#geoPanels').hide();
            }
        }

        function updatePrice() {
            //Get values
            var price1 = 0.0;
            var price2 = 0.0;

            if ($('#<%=chbGeolocation.ClientID%>').is(':checked'))
                price1 = parseFloat($('#<%=chbGeolocation.ClientID%>').parent().parent().text().trim().replace('<%=Prem.PTC.AppSettings.Site.CurrencySign%>', ''));

            var selectStringIndex = $('#<%=ddlOptions.ClientID%> option:selected').text().indexOf('-') + 1;
            price2 = parseFloat($('#<%=ddlOptions.ClientID%> option:selected').text().substring(selectStringIndex).replace('<%=Prem.PTC.AppSettings.Site.CurrencySign%>', ''))

            var totalPrice = price1 + price2;

            $('#<%=lblPrice.ClientID%>').text(totalPrice);
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
            $('#<%=Form.ClientID%>').attr('action', 'user/advert/paidtopromote.aspx');
            $('#<%=Form.ClientID%>').attr('target', '_self');
            return false;
        }

    </script>

    <script type="text/javascript">
        $(document).ready(

            function () {

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

            });

    </script>
    <link rel="stylesheet" href="Styles/WatchAds.css?sf=1" type="text/css" />
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <h1 class="page-header">
        <asp:Literal ID="TitleLiteral" runat="server"></asp:Literal></h1>
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
                        <asp:Button ID="Button1" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                        <asp:Button ID="Button2" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                    </asp:PlaceHolder>
                </asp:Panel>
            </div>
        </div>
    </div>
    <div class="tab-content">
        <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
            <asp:View ID="View1" runat="server" OnActivate="View1_Activate">

                <asp:PlaceHolder runat="server" ID="CantAddAdPlaceHolder" Visible="false">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <asp:Label ID="CantAddAdLabel" runat="server" />
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder runat="server" ID="AddAdPlaceHolder" Visible="false">
                    <asp:UpdatePanel runat="server" ID="MultiViewUpdatePanel">

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
                                                <asp:Label ID="PasswordLabel" runat="server" CssClass="control-label col-md-2" AssociatedControlID="URL">URL:</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:UpdatePanel runat="server" ID="AddNewAdWithURLCheck" OnLoad="AddNewAdWithURLCheck_Load" ClientIDMode="Static">
                                                        <ContentTemplate>
                                                            <div class="input-group">
                                                                <asp:TextBox ID="URL" runat="server" CssClass="form-control" Text="http://" MaxLength="800"></asp:TextBox>
                                                                <div class="input-group-btn">
                                                                    <asp:LinkButton ID="CheckURLButton" runat="server" OnClientClick="CheckURL()" CssClass="btn btn-primary"><%=U4200.CHECKURLTEXT %></asp:LinkButton>
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
                                            <div class="form-group">
                                                <label class="control-label col-md-2"><%=L1.PACK %>:</label>
                                                <div class="col-md-6">
                                                    <asp:DropDownList ID="ddlOptions" runat="server" CssClass="ddl form-control" AutoPostBack="true" />
                                                </div>
                                            </div>
                                            <div class="form-group" runat="server">
                                                <asp:Label ID="lblGeolocation" runat="server" AssociatedControlID="chbGeolocation" CssClass="control-label col-md-2"><%=L1.GEOLOCATION %>*:</asp:Label>
                                                <div class="col-md-6">
                                                    <div class="checkbox">
                                                        <label>
                                                            <asp:CheckBox ID="chbGeolocation" runat="server" Checked="false" AutoPostBack="true" />
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
                                                            <asp:Button ID="btnAdd" CssClass="titab titab1 btn" runat="server" Text="&#8595;" OnClick="BtnAdd_Click" />
                                                            <asp:Button ID="btnRemove" CssClass="titab titab3 btn" runat="server" Text="&#8593;" OnClick="BtnRemove_Click" />
                                                            <asp:Button ID="btnAddAll" CssClass="titab titab2 btn" runat="server" Text="&#8595;&#8595;" OnClick="BtnAddAll_Click" />
                                                            <asp:Button ID="btnRemoveAll" CssClass="titab titab4 btn" runat="server" Text="&#8593;&#8593;" OnClick="BtnRemoveAll_Click" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <div class="col-md-6 col-md-offset-2">
                                                        <asp:ListBox ID="GeoCountries" runat="server" CssClass="form-control tooltip-on"></asp:ListBox>
                                                        <input type="hidden" id="GeoCountriesValues" name="GeoCountriesValues" runat="server" clientidmode="Static">
                                                    </div>
                                                </div>  
                                            </div>

                                            <div class="form-group">
                                                <h4><%=L1.PRICE %>: <%=Prem.PTC.AppSettings.Site.CurrencySign %>
                                                    <asp:Label ID="lblPrice" runat="server" Text="Label"></asp:Label></h4>
                                            </div>

                                            <titan:TargetBalance runat="server" Feature="PaidToPromote" ID="TargetBalanceRadioButtonList"></titan:TargetBalance>
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
                </asp:PlaceHolder>
            </asp:View>

            <asp:View runat="server" ID="manageView">
                <div class="TitanViewElement">
                    <%-- SUBPAGE START --%>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:Panel ID="ErrorMessagePanel2" runat="server" Visible="false" CssClass="alert alert-danger">
                                <asp:Literal ID="ErrorMessage2" runat="server"></asp:Literal>
                            </asp:Panel>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            <div class="table-responsive">
                                <asp:GridView ID="AdvertsGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                                    DataSourceID="SqlDataSource" OnRowDataBound="AdvertsGridView_RowDataBound" DataKeyNames="Id"
                                    OnRowCommand="AdvertsGridView_RowCommand" EmptyDataText="<%$ ResourceLookup : NOADCAMPAIGNS %>" OnPreRender="BaseGridView_PreRender">
                                    <Columns>
                                        <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                        <asp:BoundField DataField="PackId" SortExpression="PackId" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                        <asp:BoundField DataField="TargetUrl" SortExpression="TargetUrl" />
                                        <asp:BoundField DataField="CreationDate" SortExpression="CreationDate" />
                                        <asp:BoundField DataField="FinishDate" SortExpression="FinishDate" />
                                        <asp:BoundField DataField="CreationDate" SortExpression="CreationDate" />
                                        <asp:BoundField DataField="EndValue" HeaderText="End Mode" SortExpression="EndValue" />
                                        <asp:BoundField DataField="EndValue" SortExpression="EndValue" />
                                        <asp:BoundField DataField="GeolocatedCC" HeaderText="GEO" SortExpression="GeolocatedCC" ItemStyle-Width="13px" />
                                        <asp:BoundField DataField="Status" SortExpression="Status" />
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
        </asp:MultiView>
    </div>


    <asp:SqlDataSource ID="SqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" SelectCommand="SELECT * FROM [PaidToPromoteAdverts] WHERE ([CreatorId] = @CreatorUsername) AND [Status] != 7 ORDER BY [Status], [CreationDate] DESC">
        <SelectParameters>
            <asp:ControlParameter ControlID="UserName" Name="CreatorUsername" PropertyName="Text" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:Label ID="UserName" runat="server" Visible="false"></asp:Label>


</asp:Content>
