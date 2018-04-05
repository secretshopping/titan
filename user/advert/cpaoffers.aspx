<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="cpaoffers.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">


    <script type="text/javascript" src="Scripts/gridview.js"></script>

    <script type="text/javascript">
        jQuery(function ($) {
            ManageGeoEvent();
            RecalculatePrice();

            $('#Amount').bind("change", RecalculatePrice);
            $('#HowMany').bind("change", RecalculatePrice);

        });

        function RecalculatePrice() {
            //Get values
            var price1 = 0.0;
            var price2 = 0;
            var percents = 5;
            var percentsDec = 5.0;
            var totalP = 0.0;
            var ptext = "";

            percents = parseInt($('#AdvCost').val());
            price1 = parseFloat($('#Amount').val());
            price2 = parseInt($('#HowMany').val());

            percentsDec = (percents + 100) / 100;

            var totalP = percentsDec * (price1 * price2);

            ptext = "(" + price2 + " x <%=Prem.PTC.AppSettings.Site.CurrencySign%>" + price1 + ") + " + percents + "% " + "<%=L1.ADVCOSTS %>";

            totalP = totalP.toFixed(<%=CoreSettings.GetMaxDecimalPlaces()%>);

            $('#<%=lblPrice.ClientID%>').text(totalP);
            $('#<%=lblPriceText.ClientID%>').text(ptext);
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
            $('#<%=DirectRefsGridView.ClientID %>').DataTable({
                responsive: true,
                paginate: false,
                info: false,
                searching: false,
                ordering: false
            });
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
    <link rel="stylesheet" href="Styles/WatchAds.css" type="text/css" />

</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=U6013.CPAGPTOFFERS %></h1>

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
                    <!--<h3><asp:Label ID="MenuTitleLabel" runat="server"></asp:Label></h3>-->
                    <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                        <asp:Button ID="ExternalSubmissionsMenuButton" runat="server" OnClick="MenuButton_Click" CommandArgument="3" />
                        <asp:Button ID="Button3" runat="server" OnClick="MenuButton_Click" CommandArgument="2" />
                        <asp:Button ID="Button2" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                        <asp:Button ID="Button1" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                    </asp:PlaceHolder>
                </asp:Panel>
            </div>
        </div>
    </div>

    <input type="hidden" id="AdvCost" value="<%=Prem.PTC.AppSettings.CPAGPT.MoneyTakenFromCPAOffersPercent %>" />

    <div class="tab-content">
        <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
            <asp:View runat="server" ID="asdasd">
                <div class="row">
                    <div class="col-md-12">
                        <%-- SUBPAGE START --%>

                        <asp:Panel ID="SuccPanel2" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
                            <asp:Literal ID="SuccLiteral" runat="server"></asp:Literal>
                        </asp:Panel>

                        <div class="alert alert-warning">
                            <b>Why my Campaign is Stopped?</b><br />
                            Your campaign will remain Stopped until you resolve some submissions (Complete/Deny). 
                                It happens because it's possible that all submissions are correct and your campaign should be Finished now.
          
                        </div>

                        <asp:GridView ID="DirectRefsGridView" runat="server" OnPreRender="BaseGridView_PreRender" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                            DataSourceID="SqlDataSource1" OnRowDataBound="DirectRefsGridView_RowDataBound"
                            DataKeyNames="Id" OnRowCommand="DirectRefsGridView_RowCommand" EmptyDataText="<%$ ResourceLookup : NODATA %>">
                            <Columns>
                                <asp:BoundField DataField="Title" HeaderText='<%$ ResourceLookup : TITLE %>' SortExpression="Title"></asp:BoundField>
                                <asp:BoundField DataField="Title" HeaderText='<%$ ResourceLookup : CATEGORY %>' SortExpression="Title"></asp:BoundField>
                                <asp:BoundField DataField="DateAdded" HeaderText='<%$ ResourceLookup : CREATED %>' SortExpression="DateAdded" DataFormatString="{0:d}" />
                                <asp:BoundField DataField="Title" HeaderText='<%$ ResourceLookup : PROGRESS %>' SortExpression="Title"></asp:BoundField>
                                <asp:BoundField DataField="IsDaily" HeaderText='<%$ ResourceLookup : DAILY %>' SortExpression="IsDaily" />
                                <asp:BoundField DataField="IsDaily" HeaderText="Geo." SortExpression="IsDaily" />
                                <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />

                                <asp:TemplateField HeaderText="">
                                    <ItemStyle />
                                    <ItemTemplate>
                                        <asp:LinkButton ID="ImageButton1" runat="server"
                                            ToolTip='Start'
                                            CommandName="start"
                                            CommandArgument='<%# Container.DataItemIndex %>'><span class="fa fa-play fa-lg text-success"></span></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="">
                                    <ItemStyle />
                                    <ItemTemplate>
                                        <asp:LinkButton ID="ImageButton2" runat="server"
                                            ToolTip='<%$ ResourceLookup : PAUSE %>'
                                            CommandName="stop"
                                            CommandArgument='<%# Container.DataItemIndex %>'><span class="fa fa-pause fa-lg text-warning"></span></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="">
                                    <ItemStyle />
                                    <ItemTemplate>
                                        <asp:LinkButton runat="server"
                                            ToolTip='<%$ ResourceLookup : REMOVE %>'
                                            CommandName="remove"
                                            CommandArgument='<%# Container.DataItemIndex %>'><spin class="fa fa-times fa-lg text-danger"></spin>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                        </asp:GridView>

                        <%-- SUBPAGE END   --%>
                    </div>
                </div>
            </asp:View>
            <asp:View ID="View1" runat="server">
                <div class="row">
                    <div class="col-md-12">

                        <div class="alert alert-warning">
                            <ul>
                                <li>You decide what action you pay for, use clear and detailed instructions to members</li>
                                <li>You decide what is the price per one action and how many actions do you want to have</li>
                                <li>If the price for action is too low, members may not do it</li>
                                <li>Please Approve/Deny all submissions as fast as possible</li>
                                <li>If you want to upload banner image, it must be <b>max 1200x300</b></li>
                            </ul>
                        </div>

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
                    <div class="col-md-12">
                        <div id="view1h" class="form-horizontal">
                            <%-- SUBPAGE START --%>
                            <div class="form-group">
                                <label class="control-label col-md-2"><%=L1.CATEGORY %>:</label>
                                <div class="col-md-6">
                                    <asp:DropDownList ID="CategoriesList" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="URL" CssClass="control-label col-md-2">URL:</asp:Label>

                                <div class="col-md-6">
                                    <asp:TextBox ID="URL" runat="server" CssClass="form-control tooltip-on" Text="http://" MaxLength="200"></asp:TextBox>
                                    <asp:RegularExpressionValidator Display="Dynamic" CssClass="text-danger" ValidationGroup="RegisterUserValidationGroup" ID="RegularExpressionValidator2" runat="server" ErrorMessage="*"
                                        ControlToValidate="URL" Text="">*</asp:RegularExpressionValidator>
                                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="URL"
                                        Display="Dynamic" CssClass="text-danger"
                                        ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label ID="Label1" runat="server" AssociatedControlID="URL" CssClass="control-label col-md-2"><%=L1.TITLE %>:</asp:Label>

                                <div class="col-md-6">
                                    <asp:TextBox ID="Title" runat="server" CssClass="form-control tooltip-on" MaxLength="50"></asp:TextBox>
                                    <asp:RegularExpressionValidator Display="Dynamic" CssClass="text-danger" ValidationGroup="RegisterUserValidationGroup" ID="RegularExpressionValidator1" runat="server" ErrorMessage="*"
                                        ValidationExpression="[^\n\r\t]{1,50}" ControlToValidate="Title" Text="">*</asp:RegularExpressionValidator>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="Title"
                                        Display="Dynamic" CssClass="text-danger"
                                        ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                </div>



                            </div>
                            <div class="form-group">
                                <asp:Label ID="Label2" runat="server" AssociatedControlID="URL" CssClass="control-label col-md-2"><%=L1.DESCRIPTION %>:</asp:Label>

                                <div class="col-md-6">
                                    <asp:TextBox ID="Description" runat="server" CssClass="form-control tooltip-on" MaxLength="3000" Rows="5" TextMode="MultiLine"></asp:TextBox>

                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="Description"
                                        Display="Dynamic" CssClass="text-danger"
                                        ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label ID="Label3" runat="server" AssociatedControlID="URL" CssClass="control-label col-md-2"><%=L1.HOWMANY %>:</asp:Label>

                                <div class="col-md-6">
                                    <asp:TextBox autocomplete="off" ID="HowMany" runat="server" Text="10" ClientIDMode="Static" CssClass="form-control tooltip-on" MaxLength="6"></asp:TextBox>

                                    <asp:RangeValidator ID="RangeValidator1" Display="Dynamic" CssClass="text-danger"
                                        ValidationGroup="RegisterUserValidationGroup" runat="server" ErrorMessage="Invalid number"
                                        ControlToValidate="HowMany" Text="" Type="Integer" MinimumValue="1" MaximumValue="999999">*</asp:RangeValidator>

                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="HowMany"
                                        Display="Dynamic" CssClass="text-danger"
                                        ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label ID="Label4" runat="server" AssociatedControlID="URL" CssClass="control-label col-md-2"><%=L1.AMOUNTPERONEACTION %>:</asp:Label>

                                <div class="col-md-6">
                                    <div class="input-prepend input-group">
                                        <span class="add-on input-group-addon">
                                            <%=AppSettings.Site.CurrencySign %>
                                        </span>
                                        <asp:TextBox autocomplete="off" ID="Amount" runat="server" ClientIDMode="Static" CssClass="form-control tooltip-on" Text="0.5"></asp:TextBox>
                                    </div>
                                    <span class="help-block">(Minimum: <%=Prem.PTC.AppSettings.CPAGPT.MinimalCPAPrice.ToString() %>)</span>

                                    <asp:RegularExpressionValidator Display="Dynamic" CssClass="text-danger" ValidationGroup="RegisterUserValidationGroup" ID="RegularExpressionValidator5" runat="server"
                                        ErrorMessage="Invalid money format"
                                        ValidationExpression="[0-9]{1,5}[\.\,]{0,1}[0-9]{0,8}" ControlToValidate="Amount" Text="">*</asp:RegularExpressionValidator>

                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="Amount"
                                        Display="Dynamic" CssClass="text-danger"
                                        ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="text-center">
                                    <asp:Image ID="createBannerAdvertisement_BannerImage" runat="server" CssClass="img-responsive" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label ID="Label5" runat="server" AssociatedControlID="URL" CssClass="control-label col-md-2">
                                    <asp:Label ID="BannerImageLabel" runat="server" Text="Label"></asp:Label>
                                    *:</asp:Label>

                                <div class="col-md-6">
                                    <span class="btn btn-success fileinput-button">
                                        <i class="fa fa-plus"></i>
                                        <span><%=U6000.ADDFILE %></span>
                                        <asp:FileUpload ID="createBannerAdvertisement_BannerUpload" runat="server" />
                                    </span>
                                    <asp:Button ID="BannerUploadByUrlButton" Text="<%$ResourceLookup: ADDBANNERBYURL %>" runat="server" CssClass="btn btn-success fileinput-button" OnClientClick="showURLBox(); return false;" />

                                </div>
                                <div class="col-md-6 col-md-offset-2 m-t-15">
                                    <div class="input-group">
                                        <asp:TextBox ID="BannerFileUrlTextBox" runat="server" CssClass="form-control" Style="display: none"></asp:TextBox>
                                        <div class="input-group-btn">
                                            <asp:Button ID="createBannerAdvertisement_BannerUploadSubmit" Text="<%$ResourceLookup: SUBMIT %>" OnClick="createBannerAdvertisement_BannerUploadSubmit_Click"
                                                CausesValidation="true" runat="server" ValidationGroup="createBannerAdvertisement_OnSubmitValidationGroup" CssClass="btn btn-primary start" />
                                            <br />
                                        </div>
                                    </div>
                                    <asp:CustomValidator runat="server" ID="ImageUploadedValidator" OnServerValidate="ImageSubmitValidator_ServerValidate" Display="Dynamic" CssClass="text-danger"
                                                    ValidationGroup="createBannerAdvertisement_OnSubmitValidationGroup">*</asp:CustomValidator>

                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-md-2 control-label"><%=L1.LBR %>:</label>
                                <div class="col-md-6">
                                    <div class="checkbox">
                                        <label>
                                            <asp:CheckBox ID="LoginIDBox" runat="server" Checked="false" />
                                            <%=L1.LBR1 %>
                                        </label>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-md-2 control-label"><%=L1.EBR %>:</label>
                                <div class="col-md-6">
                                    <div class="checkbox">
                                        <label>
                                            <asp:CheckBox ID="EmailIDBox" runat="server" Checked="false" />
                                            <%=L1.EBR1 %>
                                        </label>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-md-2 control-label"><%=L1.REFDAILY %>:</label>
                                <div class="col-md-6">
                                    <div class="checkbox">
                                        <label>
                                            <asp:CheckBox ID="IsDaily" runat="server" Checked="false" />
                                            <%=L1.DAILY1 %>
                                        </label>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-md-2 control-label">
                                    <asp:Label ID="lblGeolocation" runat="server" Text="Geolocation *"></asp:Label>:</label>
                                <div class="col-md-6">
                                    <div class="checkbox">
                                        <label>
                                            <asp:CheckBox ID="chbGeolocation" runat="server" CssClass="" Checked="false" />
                                        </label>
                                    </div>
                                </div>
                            </div>






                            <%--    Only for outside advertising--%>
                            <%-- <asp:PlaceHolder ID="OutEmailPlaceHolder" runat="server" Visible="false">

                                                    <tr>
                                                        <td>Email:
                                                            <asp:RegularExpressionValidator ValidationGroup="RegisterUserValidationGroup" ID="OutEmailRegularExpressionValidator" runat="server"
                                                                ValidationExpression="^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$" ForeColor="#b70d00" ControlToValidate="OutEmail" Text="">'</asp:RegularExpressionValidator>

                                                            <asp:RequiredFieldValidator ID="OutEmailRequiredFieldValidator" runat="server" ControlToValidate="OutEmail"
                                                                ForeColor="#b70d00" Text=""
                                                                ValidationGroup="RegisterUserValidationGroup">'</asp:RequiredFieldValidator>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="OutEmail" runat="server" CssClass="rtextbox" Width="384px"></asp:TextBox>
                                                        </td>
                                                    </tr>

                                                </asp:PlaceHolder>--%>
                            <%--    Only for outside advertising--%>




                            <div id="geoPanels" style="display: none">

                                <div class="form-group">
                                    <div class="col-md-6 col-md-offset-2">
                                        <asp:ListBox ID="AllCountries" runat="server" CssClass="form-control tooltip-on"></asp:ListBox>
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
                                        <asp:ListBox ID="GeoCountries" runat="server" CssClass="form-control tooltip-on"></asp:ListBox>
                                        <input type="hidden" id="GeoCountriesValues" name="GeoCountriesValues" runat="server" clientidmode="Static">
                                    </div>
                                </div>

                                                            <div class="form-group displaynone">
                                                                <label class="control-label col-md-2"><%=U3900.CITIES %>:</label>
                                                                <div class="col-md-6">
                                                                    <asp:TextBox ID="GeoCities" CssClass="form-control tooltip-on" runat="server" ></asp:TextBox> 
                                                                </div>
                                                            </div>

                                <div class="form-group">
                                    <label class="col-md-2 control-label"><%=U3900.AGE %>:</label>
                                    <div class="col-md-6">
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



                            <h3 class="m-t-30 m-b-30">
                                <span style="color: #000"><%=L1.PRICE %>: <%=Prem.PTC.AppSettings.Site.CurrencySign %><asp:Label ID="lblPrice" runat="server" Text="Label"></asp:Label></span>
                                =
                                    <asp:Label ID="lblPriceText" runat="server" Text="Label"></asp:Label>
                            </h3>
                            <titan:TargetBalance runat="server" Feature="CPA" ID="TargetBalanceRadioButtonList"></titan:TargetBalance>

                                <div class="form-group">
                                    <div class="col-md-3">
                                        <asp:Button ID="CreateAdButton" runat="server"
                                                ValidationGroup="RegisterUserValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="CreateAdButton_Click" 
                                               OnClientClick="CheckGeo(); "
                                                    UseSubmitBehavior="false" />
                                    </div>
                                </div>

                            <%-- SUBPAGE END   --%>
                        </div>
                    </div>
                </div>
            </asp:View>
            <asp:View runat="server" ID="View2">
                <div class="row">
                    <div class="col-md-12">
                        <div class="TitanViewElement">
                            <%-- SUBPAGE START --%>

                            <asp:Panel ID="Panel1" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
                                <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                            </asp:Panel>
                            <div class="alert alert-warning">
                                <p>
                                    If everything is OK, click <b>Complete</b> and the member will be credited for this submission.
                                <br />
                                    If it's not, you can either <b>Under Review</b> it (if you found something suspicious and need clarification) or <b>Deny</b> it (close forever)
                                </p>

                            </div>
                            <%--  <asp:DropDownList ID="OfferList" runat="server"></asp:DropDownList>--%>

                            <div class="table-responsive">

                                <asp:GridView ID="GridView1" runat="server" AllowPaging="True" OnPreRender="BaseGridView_PreRender" AllowSorting="True" AutoGenerateColumns="False"
                                    DataSourceID="SqlDataSource2" OnRowDataBound="GridView1_RowDataBound" DataKeyNames="Id" OnRowCommand="GridView1_RowCommand">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Select" HeaderStyle-Width="26px" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkSelect" runat="server" />
                                            </ItemTemplate>
                                            <HeaderTemplate>
                                                <input type="checkbox" id="checkAll" class="regular-checkbox mini-checkbox" onclick="<%=this.jsSelectAllCode %>" /><label for="checkAll"></label>
                                            </HeaderTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" Visible="true" InsertVisible="False" ReadOnly="True" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone"></asp:BoundField>
                                        <asp:BoundField DataField="OfferId" HeaderText='<%$ ResourceLookup : CAMPAIGN %>' SortExpression="OfferId"></asp:BoundField>
                                        <asp:BoundField DataField="PostDate" HeaderText='<%$ ResourceLookup : SUBMITED %>' SortExpression="PostDate" DataFormatString="{0:d}" />
                                        <asp:BoundField DataField="LoginID" HeaderText='LoginID' SortExpression="LoginID"></asp:BoundField>
                                        <asp:BoundField DataField="EmailID" HeaderText='EmailID' SortExpression="EmailID" />
                                        <asp:BoundField DataField="OfferStatus" HeaderText="Status" SortExpression="OfferStatus" />

                                        <asp:TemplateField HeaderText="">
                                            <ItemStyle />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="ImageButton11" runat="server"
                                                    ToolTip='Complete'
                                                    CommandName="accept"
                                                    CommandArgument='<%# Container.DataItemIndex %>'>
                                                    <span class="fa fa-plus fa-lg text-success"></span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="">
                                            <ItemStyle />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="ImageButton33" runat="server"
                                                    ToolTip='Under Review'
                                                    CommandName="under"
                                                    CommandArgument='<%# Container.DataItemIndex %>'>
                                                    <span class="fa fa-clock-o fa-lg text-info"></span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="">
                                            <ItemStyle />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="ImageButton22" runat="server"
                                                    ToolTip='Deny'
                                                    CommandName="deny"
                                                    CommandArgument='<%# Container.DataItemIndex %>'>
                                                    <span class="fa fa-times fa-lg text-danger"></span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                            <asp:Panel ID="Panel2" runat="server" CssClass="displaynone">
                                <%=L1.SELECTED %>: 
                            <asp:LinkButton ID="ImageButton3" runat="server" OnClick="ImageButton1_Click" ToolTip='<%$ ResourceLookup : REMOVE %>'><span class="fa fa-times fa-lg text-danger"></span></asp:LinkButton>
                            </asp:Panel>
                            <%-- SUBPAGE END   --%>
                        </div>
                    </div>
                </div>

            </asp:View>
            <asp:View runat="server" ID="ExternalSubmissionsView">
                <div class="row">
                    <div class="col-md-12">
                        <div class="TitanViewElement">
                            <%-- SUBPAGE START --%>

                            <asp:Panel ID="ExternalSubmissionsSuccessPanel" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
                                <asp:Literal ID="ExternalSubmissionsSuccessMessage" runat="server"></asp:Literal>
                            </asp:Panel>
                            <div class="alert alert-warning">
                                <p>If everything is OK, click <b>Complete</b> and the member will be credited for this submission.
                                <br />
                                If it's not, you can either <b>Under Review</b> it (if you found something suspicious and need clarification) or <b>Deny</b> it (close forever)</p>

                            </div>                            
                            <div class="table-responsive">
                                <asp:GridView ID="ExternalSubmissionsGridView" runat="server" AllowPaging="True" OnPreRender="BaseGridView_PreRender" AllowSorting="True"
                                    DataSourceID="ExternalSubmissionsGridView_DataSource" OnRowDataBound="ExternalSubmissionsGridView_RowDataBound"
                                    DataKeyNames="Id" OnRowCommand="ExternalSubmissionsGridView_RowCommand">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Select" HeaderStyle-Width="26px" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkSelect" runat="server" />
                                            </ItemTemplate>
                                            <HeaderTemplate>
                                                <input type="checkbox" id="checkAll" class="regular-checkbox mini-checkbox" onclick="<%=this.jsSelectAllCode %>" /><label for="checkAll"></label>
                                            </HeaderTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" Visible="true" InsertVisible="False" ReadOnly="True" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone"></asp:BoundField>
                                        <asp:BoundField DataField="OfferId" HeaderText='<%$ ResourceLookup : CAMPAIGN %>' SortExpression="OfferId"></asp:BoundField>
                                        <asp:BoundField DataField="DateAdded" HeaderText='<%$ ResourceLookup : SUBMITED %>' SortExpression="DateAdded" DataFormatString="{0:d}" />
                                        <asp:BoundField DataField="LoginID" HeaderText='LoginID' SortExpression="LoginID"></asp:BoundField>
                                        <asp:BoundField DataField="EmailID" HeaderText='EmailID' SortExpression="EmailID" />
                                        <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />

                                        <asp:TemplateField HeaderText="">
                                            <ItemStyle />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="ImageButton11" runat="server"
                                                    ToolTip='Complete'
                                                    CommandName="accept"
                                                    CommandArgument='<%# Container.DataItemIndex %>'>
                                                    <span class="fa fa-plus fa-lg text-success"></span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="">
                                            <ItemStyle />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="ImageButton33" runat="server"
                                                    ToolTip='Under Review'
                                                    CommandName="under"
                                                    CommandArgument='<%# Container.DataItemIndex %>'>
                                                    <span class="fa fa-clock-o fa-lg text-info"></span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="">
                                            <ItemStyle />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="ImageButton22" runat="server"
                                                    ToolTip='Deny'
                                                    CommandName="deny"
                                                    CommandArgument='<%# Container.DataItemIndex %>'>
                                                    <span class="fa fa-times fa-lg text-danger"></span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                <asp:SqlDataSource runat="server" ID="ExternalSubmissionsGridView_DataSource" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="ExternalSubmissionsGridView_DataSource_Init"></asp:SqlDataSource>
                            </div>
                            <asp:Panel ID="Panel3" runat="server" CssClass="displaynone">
                                <%=L1.SELECTED %>: 
                            <asp:LinkButton ID="LinkButton1" runat="server" OnClick="ImageButton1_Click" ToolTip='<%$ ResourceLookup : REMOVE %>'><span class="fa fa-times fa-lg text-danger"></span></asp:LinkButton>
                            </asp:Panel>
                            <%-- SUBPAGE END   --%>
                        </div>
                    </div>
                </div>

            </asp:View>
        </asp:MultiView>
    </div>

    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" SelectCommand="SELECT * FROM [CPAOffers] WHERE ([AdvertiserUsername] = @CreatorUsername) AND [Status] != 7 ORDER BY DateAdded DESC">
        <SelectParameters>
            <asp:ControlParameter ControlID="UserName" Name="CreatorUsername" PropertyName="Text" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" SelectCommand="SELECT * FROM [OfferRegisterEntries] WHERE ([OfferId] IN (SELECT Id FROM CPAOffers WHERE AdvertiserUsername = @CreatorUsername)) AND (OfferStatus = 5 OR OfferStatus = 2 OR OfferStatus = 3 OR OfferStatus = 7) ORDER BY OfferStatus DESC">
        <SelectParameters>
            <asp:ControlParameter ControlID="UserName" Name="CreatorUsername" PropertyName="Text" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>

    <asp:Label ID="UserName" runat="server" Visible="false"></asp:Label>

</asp:Content>
