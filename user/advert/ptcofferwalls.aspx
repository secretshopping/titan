<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ptcofferwalls.aspx.cs" Inherits="user_advert_ptcofferwalls" MasterPageFile="~/User.master" %>

<asp:Content ContentPlaceHolderID="PageHeadContent" runat="server">
    <script>
        function pageLoad() {
            ManageGeoEvent();
        }
        function ManageGeoEvent() {
            if ($('#<%=GeolocationCheckBox.ClientID%>').is(':checked')) {
                $('#geoPlaceHolder').show();
            }
            else {
                $('#geoPlaceHolder').hide();
            }
        }
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <asp:UpdatePanel ID="MultiViewUpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <h1 class="page-header"><%=U6002.PTCOFFERWALLS%></h1>
            <div class="row">
                <div class="col-md-12">
                    <p class="lead"><%=U6002.PTCOFFERWALLSDESC%></p>
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
                                <asp:Button ID="MenuButtonMyOfferWalls" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                                <asp:Button ID="MenuButtonCreateOfferWall" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </div>
            <div class="tab-content">
                <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
                    <asp:View runat="server" ID="CreateOfferWallView" OnActivate="CreateOfferWallView_Activate">
                        <div class="TitanViewElement">
                            <div class="row">
                                <div class="row">
                                    <div class="col-md-12">
                                        <asp:ValidationSummary runat="server" CssClass="alert alert-danger fade in m-b-15"
                                            ValidationGroup="CreateOfferWallValidationGroup" DisplayMode="List" />
                                    </div>
                                </div>
                                <div class="col-md-8">
                                    <div class="form-horizontal" runat="server" id="CreateOfferWallsPlaceHolder">
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=L1.TITLE %>:</label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TitleTextBox" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>

                                                <asp:RequiredFieldValidator ID="TitleRequiredFieldValidator" runat="server" ControlToValidate="TitleTextBox"
                                                    Display="Dynamic" CssClass="text-danger"
                                                    ValidationGroup="CreateOfferWallValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=L1.DESCRIPTION %>:</label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="DescriptionTextBox" runat="server" CssClass="form-control" MaxLength="500" TextMode="MultiLine" Rows="3"></asp:TextBox>

                                                <asp:RequiredFieldValidator ID="DescriptionRequiredFieldValidator" runat="server" ControlToValidate="DescriptionTextBox"
                                                    Display="Dynamic" CssClass="text-danger"
                                                    ValidationGroup="CreateOfferWallValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=L1.PACK %>:</label>
                                            <div class="col-md-6">
                                                <asp:DropDownList runat="server" ID="PacksDDL" class="form-control" OnInit="PacksDDL_Init">
                                                </asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <label class="control-label col-md-2">URLs:</label>
                                            <div class="col-md-6">
                                                <div class="checkbox">
                                                    <label>
                                                <asp:CheckBoxList runat="server" ID="UserUrlsCheckBoxList" OnInit="UserUrlsCheckBoxList_Init"></asp:CheckBoxList>
                                                 </label>
                                                     </div>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=U6003.ALLOWPCUSERS %>:</label>
                                            <div class="col-md-6">
                                                <div class="checkbox">
                                                    <label>
                                                        <asp:CheckBox runat="server" ID="PcAllowedCheckBox" Checked="true" />
                                                    </label>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=U6003.ALLOWMOBILEDEVICES %>:</label>
                                            <div class="col-md-6">
                                                <div class="checkbox">
                                                    <label>
                                                        <asp:CheckBox runat="server" ID="MobileAllowedCheckBox" Checked="true" />
                                                    </label>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=U6003.ALLOWAUTOSURF %>:</label>
                                            <div class="col-md-6">
                                                <div class="checkbox">
                                                    <label>
                                                        <asp:CheckBox runat="server" ID="AutosurfAllowedCheckBox" />
                                                    </label>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=U6003.MAXSINGLEUSERDAILYVIEWS %>:</label>
                                            <div class="col-md-6">
                                                <asp:TextBox runat="server" ID="MaxSingleUserDailyViewsTextBox" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="MaxSingleUserDailyViewsRequiredValidator" runat="server" ControlToValidate="MaxSingleUserDailyViewsTextBox"
                                                    Display="Dynamic" CssClass="text-danger"
                                                    ValidationGroup="CreateOfferWallValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                                <asp:CustomValidator runat="server" ID="MaxSingleUserDailyViewsCustomValidator" ControlToValidate="MaxSingleUserDailyViewsTextBox"
                                                    Display="Dynamic" CssClass="text-danger" ValidationGroup="CreateOfferWallValidationGroup" Text=""
                                                    OnServerValidate="MaxSingleUserDailyViewsCustomValidator_ServerValidate">*</asp:CustomValidator>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="GeolocationCheckBox" CssClass="control-label col-md-2"><%=L1.GEOLOCATION %>:</asp:Label>
                                            <div class="col-md-6">
                                                <div class="checkbox">
                                                    <label>
                                                        <asp:CheckBox ID="GeolocationCheckBox" runat="server" Checked="false" />
                                                    </label>
                                                </div>
                                            </div>
                                        </div>
                                        <div id="geoPlaceHolder" style="display: none">
                                            <div class="form-group">
                                                <div class="col-md-6 col-md-offset-2">
                                                    <asp:ListBox ID="AllCountriesListBox" runat="server" CssClass="form-control tooltip-on"></asp:ListBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <div class="col-md-6 col-md-offset-2">
                                                    <div class="input-group-btn">
                                                        <asp:Button ID="AddCountryButton" CssClass="titab titab1 btn" runat="server" Text="&#8595;" OnClick="AddCountryButton_Click" />
                                                        <asp:Button ID="RemoveCountryButton" CssClass="titab titab3 btn" runat="server" Text="&#8593;" OnClick="RemoveCountryButton_Click" />
                                                        <asp:Button ID="AddAllCountriesButton" CssClass="titab titab2 btn" runat="server"
                                                            Text="&#8595;&#8595;" OnClick="AddAllCountriesButton_Click" />
                                                        <asp:Button ID="RemoveAllCountriesButton" CssClass="titab titab4 btn" runat="server"
                                                            Text="&#8593;&#8593;" OnClick="RemoveAllCountriesButton_Click" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <div class="col-md-6 col-md-offset-2">
                                                    <asp:ListBox ID="GeoCountriesListBox" runat="server" CssClass="form-control tooltip-on"></asp:ListBox>
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
                                        <titan:TargetBalance runat="server" Feature="PtcOfferWall" ID="TargetBalanceRadioButtonList"></titan:TargetBalance>
                                        <div class="form-group">
                                            <div class="col-md-2">
                                                <asp:Button ID="CreateOfferWallButton" runat="server"
                                                    ValidationGroup="CreateOfferWallValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="CreateOfferWallButton_Click" />
                                            </div>
                                        </div>
                                    </div>
                                    <titan:FeatureUnavailable runat="server" ID="NewOfferWallUnavailable"></titan:FeatureUnavailable>
                                </div>
                            </div>
                        </div>
                    </asp:View>
                    <asp:View runat="server" ID="MyOfferWallsView" OnActivate="MyOfferWallsView_Activate">
                        <div class="TitanViewElement">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="table-responsive">
                                        <asp:GridView ID="MyOfferWallsGridView" runat="server" AllowPaging="True" AllowSorting="True"
                                            DataSourceID="MyOfferWallsGridView_DataSource" OnPreRender="BaseGridView_PreRender"
                                            OnRowDataBound="MyOfferWallsGridView_RowDataBound" PageSize="20" OnDataBound="MyOfferWallsGridView_DataBound"
                                            EmptyDataText="<%$ ResourceLookup : NODATA %>">
                                            <Columns>
                                                <asp:BoundField DataField='Id' HeaderText='Id' SortExpression='Id' ControlStyle-CssClass="displaynone"
                                                    HeaderStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" />
                                                <asp:BoundField DataField='Title' SortExpression='Title' />
                                                <asp:BoundField DataField='Description' SortExpression='Description' />
                                                <asp:BoundField DataField='CompletionTimes' SortExpression='CompletionTimes' />
                                                <asp:TemplateField SortExpression="DisplayTime">
                                                    <ItemTemplate>
                                                        <%# Eval("DisplayTime") %>s
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:GridView runat="server" ID="MyOfferWallsUrlsGridView" AutoGenerateColumns="false" SkinID="-1" GridLines="None" HeaderStyle-CssClass="displaynone">
                                                            <Columns>
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <a href='<%#Eval("Url")%>'><%# Eval("Url").ToString() %></a>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField SortExpression="Status">
                                                    <ItemTemplate>
                                                        <%# HtmlCreator.GetColoredStatus((Prem.PTC.Advertising.AdvertStatus)(Eval("Status"))) %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                    <asp:SqlDataSource ID="MyOfferWallsGridView_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
                                        OnInit="MyOfferWallsGridView_DataSource_Init"></asp:SqlDataSource>

                                </div>
                            </div>
                        </div>
                    </asp:View>
                </asp:MultiView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
