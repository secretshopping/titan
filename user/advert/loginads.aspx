<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="loginads.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script type="text/javascript" src="Scripts/gridview.js"></script>


    <script type="text/javascript">

        jQuery(function ($) {
            ManageGeoEvent();
        });

        function ManageGeoEvent() {
            if ($('#<%=chbGeolocation.ClientID%>').is(':checked')) {
                $('#geoPanels').show();
            }
            else {
                $('#geoPanels').hide();
            }
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

        function CheckURL() {
            $('#<%=RegisterUserValidationSummary.ClientID%>').addClass("displaynone");

            $('#__EVENTARGUMENT5').val($('#<%=URL.ClientID %>').val()); //Set URL to validate

            //Send request
            $('#<%=Form.ClientID%>').attr('action', 'user/earn/surf.aspx?f=3');
            $('#<%=Form.ClientID%>').attr('target', '_blank');
            $('#<%=Form.ClientID%>').submit();
            $('#<%=Form.ClientID%>').attr('action', 'user/advert/loginads.aspx');
            $('#<%=Form.ClientID%>').attr('target', '_self');

            return false;
        }

        function HideSuccess() {
            $('#<%=SuccMessagePanel.ClientID%>').addClass("displaynone");
            $('#<%=RegisterUserValidationSummary.ClientID%>').removeClass("displaynone");
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

            function pageLoad() {
            <%=PageScriptGenerator.GetGridViewCode(LoginAdsGridView) %>
        }

    </script>
    <link rel="stylesheet" href="Styles/WatchAds.css?s=1" type="text/css" />

</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=U4200.LOGINADS %></h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead">
                <asp:Literal ID="SubLiteral" runat="server"></asp:Literal></p>
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
            <asp:View runat="server" ID="View1">
                <div class="TitanViewElement">
                    <div class="row">
                        <div class="col-md-12">

                            <asp:GridView ID="LoginAdsGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                                DataSourceID="LoginAdsGridViewDataSource" OnPreRender="BaseGridView_PreRender" OnRowDataBound="LoginAdsGridView_RowDataBound" PageSize="20"
                                EmptyDataText="<%$ ResourceLookup : NOADCAMPAIGNS %>" OnRowCommand="LoginAdsGridView_RowCommand">
                                <Columns>
                                    <asp:BoundField DataField='Id' HeaderText='Id' SortExpression='Id' ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField='TargetUrl' HeaderText='Target Url' SortExpression='TargetUrl' />
                                    <asp:BoundField DataField='DisplayDate' HeaderText='Display Date' SortExpression='DisplayDate' />
                                    <asp:BoundField DataField='TotalViews' HeaderText='Total Views' SortExpression='TotalViews' />
                                    <asp:BoundField DataField='Status' HeaderText='Status' SortExpression='Status' />
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
                            <asp:SqlDataSource ID="LoginAdsGridViewDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="LoginAdsGridViewDataSource_Init"></asp:SqlDataSource>

                        </div>
                    </div>
                </div>
            </asp:View>

            <asp:View ID="View2" runat="server">

                <div class="TitanViewElement">
                    <%-- SUBPAGE START --%>

                    <div class="row">
                        <div class="col-md-12">
                            <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                                <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                            </asp:Panel>

                            <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15" ClientIDMode="Static">
                                <asp:Literal ID="SuccMessage" runat="server"></asp:Literal>
                            </asp:Panel>
                            <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="alert alert-danger fade in m-b-15"
                                ValidationGroup="RegisterUserValidationGroup" DisplayMode="List" ClientIDMode="Static" />
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            <div class="row">
                                <div class="col-md-12">
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
                                                <asp:RequiredFieldValidator ID="UrlRequired" runat="server" ControlToValidate="URL"
                                                    Display="Dynamic" CssClass="text-danger"
                                                    ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=L1.PRICE %>:</label>
                                            <div class="col-md-6">
                                                <asp:Label ID="PriceLiteral" CssClass="form-control no-border" runat="server" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=U5001.DISPLAYTIME %>:</label>
                                            <div class="col-md-6">
                                                <span class="form-control no-border"><%=AppSettings.LoginAds.DisplayTime%> <%=L1.SECONDS %></span>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=U5001.ADSPERDAY %>:</label>
                                            <div class="col-md-6">
                                                <span class="form-control no-border"><%=AppSettings.LoginAds.AdsPerDay %></span>
                                            </div>
                                        </div>

                                        <asp:PlaceHolder runat="server" ID="GeolocationPlaceholder" Visible="false">

                                            <div class="form-group">
                                                <label class="col-md-2 control-label">
                                                    <asp:Label ID="lblGeolocation" runat="server" Text="Geolocation*"></asp:Label>:</label>
                                                <div class="col-md-6">
                                                    <div class="checkbox">
                                                        <label>
                                                            <asp:CheckBox ID="chbGeolocation" runat="server" Checked="false" />
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

                                                <div class="form-group" style="display: none">
                                                    <label class="control-label col-md-2"><%=U3900.CITIES %>:</label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox ID="GeoCities" CssClass="form-control" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="form-group">
                                                    <label class="col-md-2 control-label"><%=U3900.AGE %>:</label>
                                                    <div class="col-md-6">
                                                        <div class="input-group">
                                                            <asp:TextBox ID="GeoAgeMin" CssClass="form-control" runat="server" MaxLength="2" Text="0"></asp:TextBox>
                                                            <span class="input-group-addon">-</span>
                                                            <asp:TextBox ID="GeoAgeMax" CssClass="form-control" runat="server" MaxLength="2" Text="0"></asp:TextBox>
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

                                        </asp:PlaceHolder>

                                        <asp:PlaceHolder runat="server" ID="LoginAdsCreditsPlaceHolder">

                                            <div class="form-group">
                                                <label class="col-md-2 control-label"><%=U5008.USELOGINADSCREDITS %>:</label>
                                                <div class="col-md-6">
                                                    <div class="checkbox">
                                                        <label>
                                                            <asp:CheckBox runat="server" ID="LoginAdsCreditsCheckBox" OnCheckedChanged="LoginAdsCreditsCheckBox_CheckedChanged" AutoPostBack="true" />
                                                            <asp:Literal runat="server" ID="AvailableLoginAdsCreditsLiteral"></asp:Literal>
                                                        </label>
                                                    </div>
                                                </div>
                                            </div>

                                        </asp:PlaceHolder>

                                        <div class="form-group">
                                            <div class="col-md-6 col-md-offset-2">
                                                <asp:UpdatePanel runat="server">
                                                    <ContentTemplate>
                                                        <asp:Calendar ID="AdDisplayDateCalendar" runat="server" OnDayRender="AdDisplayDateCalendar_DayRender" CssClass="table table-condensed table-borderless calendar" OnLoad="AdDisplayDateCalendar_Load"></asp:Calendar>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                        </div>
                                        <titan:TargetBalance runat="server" Feature="LoginAd" ID="TargetBalanceRadioButtonList"></titan:TargetBalance>

                                        <div class="form-group">
                                            <div class="col-md-2">
                                                <asp:Button ID="CreateAdButton" runat="server"
                                                    ValidationGroup="RegisterUserValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="CreateAdButton_Click"
                                                    OnClientClick="HideSuccess(); CheckGeo();"
                                                    UseSubmitBehavior="false" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>


                <%-- SUBPAGE END   --%>
            </asp:View>
        </asp:MultiView>
    </div>
</asp:Content>
