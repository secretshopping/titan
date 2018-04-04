<%@ Page Language="C#" AutoEventWireup="true" CodeFile="representatives.aspx.cs" Inherits="sites_representatives" MasterPageFile="~/Sites.master" %>

<asp:Content runat="server" ContentPlaceHolderID="ContentHeadContent">
    <style>
        .UpperFlag {
            margin-bottom: 3px;
        }

        .customButton {
            padding: 6px 12px;
            border-radius: 3px;
        }
    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>

    <div class="content" data-scrollview="true">
        <div id="TitlePlaceHolder" runat="server" class="container" data-animation="true" data-animation-type="fadeInDown">
            <h2 class="content-title">
                <asp:Literal runat="server" ID="titleLiteral" /></h2>
            <p class="text-center">
                <asp:PlaceHolder runat="server" ID="DefaultSubTitlePlaceHolder">
                    <%=String.Format(U6002.REPRESENT, AppSettings.Site.Name) %>
                    <asp:LinkButton ID="ContactPanelLinkButton" runat="server" OnClick="ContactPanelLinkButton_Click">&nbsp;<%=L1.CONTACTFORM %></asp:LinkButton>
                </asp:PlaceHolder>

                <asp:PlaceHolder runat="server" ID="ForRepSubTitlePlaceHolder" Visible="false">
                    <%=String.Format(U6010.YOUAREREPRESENTATIVE, AppSettings.Site.Name, Member.CurrentInCache.Country) %><br /><br />
                    <asp:LinkButton ID="EditPaymentMethodsLinkButton" runat="server" CssClass="btn btn-primary customButton" OnClick="EditPaymentMethodsLinkButton_Click">&nbsp;<%=U6010.EDITPAYMENTMETHODS %></asp:LinkButton>
                </asp:PlaceHolder>
            </p>
        </div>
        <div class="container">
            <div class="row-fluid">
                <div class="col-md-6 col-md-offset-3">
                    <div class="row-fluid">
                        <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="alert alert-success">
                            <asp:Literal ID="SuccMessage" runat="server"></asp:Literal>
                        </asp:Panel>
                        <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger">
                            <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                        </asp:Panel>
                    </div>
                    <div class="row-fluid">
                        <asp:PlaceHolder ID="ContactPanelPlaceHolder" runat="server">
                            <h2 class="content-title"><%=U6002.REPRESENTATIVES %></h2>
                            <p class="text-center"><%=U6002.REPRESENTINFO %></p>
                            <div class="row">
                                <div class="col-md-12">

                                    <asp:ValidationSummary ID="RepresentativeValidationSummary" runat="server" CssClass="alert alert-danger"
                                        ValidationGroup="RepresentativeValidationGroup" DisplayMode="List" />
                                </div>
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <label class="control-label col-md-2"><%=L1.NAME %>:</label>
                                        <div class="col-md-10">
                                            <asp:TextBox ID="NameTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="NameTextBoxRequiredFieldValidator" runat="server" ControlToValidate="NameTextBox" Text=""
                                                ValidationGroup="RepresentativeValidationGroup" Display="Dynamic" CssClass="text-danger">*</asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="control-label col-md-2">Email:</label>
                                        <div class="col-md-10">
                                            <asp:TextBox ID="EmailTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                                            <asp:RegularExpressionValidator runat="server" ValidationGroup="RepresentativeValidationGroup"
                                                Display="Dynamic" CssClass="text-danger" Text="" ID="CorrectEmailRequired" ControlToValidate="EmailTextBox"
                                                ValidationExpression="^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$"></asp:RegularExpressionValidator>
                                            <asp:RequiredFieldValidator ID="EmailTextBoxRequiredFieldValidator" runat="server" ControlToValidate="EmailTextBox" Display="Dynamic"
                                                CssClass="text-danger" Text="" ValidationGroup="RepresentativeValidationGroup">*</asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="control-label col-md-2"><%=U6002.WHY %>?</label>
                                        <div class="col-md-10">
                                            <asp:TextBox ID="WhyTextBox" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="WhyRequiredFieldValidator" runat="server" ControlToValidate="WhyTextBox" Text=""
                                                ValidationGroup="RepresentativeValidationGroup" Display="Dynamic" CssClass="text-danger">*</asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="control-label col-md-2"><%=L1.CITY %>:</label>
                                        <div class="col-md-10">
                                            <asp:TextBox ID="CityTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="CityTextBoxRequiredFieldValidator" runat="server" ControlToValidate="CityTextBox" Text=""
                                                ValidationGroup="RepresentativeValidationGroup" Display="Dynamic" CssClass="text-danger">*</asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="control-label col-md-2"><%=L1.COUNTRY %>:</label>
                                        <div class="col-md-10">
                                            <asp:TextBox ID="CountryTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                                            <asp:DropDownList ID="CountryDropDownList" runat="server" CssClass="form-control"></asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="CountryTextBoxRequiredFieldValidator" runat="server" ControlToValidate="CountryTextBox" Text=""
                                                ValidationGroup="RepresentativeValidationGroup" Display="Dynamic" CssClass="text-danger">*</asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="control-label col-md-2"><%=U4200.SKYPE %>:</label>
                                        <div class="col-md-10">
                                            <asp:TextBox ID="SkypeTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="SkypeTextBoxRequiredFieldValidator" runat="server" ControlToValidate="SkypeTextBox" Text=""
                                                ValidationGroup="RepresentativeValidationGroup" Display="Dynamic" CssClass="text-danger">*</asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="control-label col-md-2">Facebook:</label>
                                        <div class="col-md-10">
                                            <asp:TextBox ID="FacebookTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="FacebookTextBoxRequiredFieldValidator" runat="server" ControlToValidate="FacebookTextBox" Text=""
                                                ValidationGroup="RepresentativeValidationGroup" Display="Dynamic" CssClass="text-danger">*</asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="control-label col-md-2"><%=U4200.PHONE %>:</label>
                                        <div class="col-md-10">
                                            <asp:TextBox ID="PhoneTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                                            <%--                                            <asp:RegularExpressionValidator ValidationGroup="RepresentativeValidationGroup" ID="PhoneTextBoxRegularExpressionValidator" runat="server"
                                                ValidationExpression="[a-zA-Z0-9\.\,\-\!\?\&\:\$\@\(\)\+\=\%\*|\\\t\\\n|\\\r|\\\s]{1,10}" Display="Dynamic"
                                                CssClass="text-danger" ControlToValidate="PhoneTextBox" Text=""></asp:RegularExpressionValidator>--%>
                                            <asp:CustomValidator runat="server" ID="PhoneNumberCustomValidator" ValidationGroup="RepresentativeValidationGroup" ControlToValidate="PhoneTextBox" OnServerValidate="PhoneNumberCustomValidator_ServerValidate"></asp:CustomValidator>
                                            <asp:RequiredFieldValidator ID="PhoneTextBoxRequiredFieldValidator" runat="server" ControlToValidate="PhoneTextBox" Display="Dynamic"
                                                CssClass="text-danger" Text="" ValidationGroup="RepresentativeValidationGroup">*</asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="control-label col-md-2"><%=L1.LANGUAGE %>:</label>
                                        <div class="col-md-10">
                                            <asp:TextBox ID="LanguagesTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="LanguagesTextBoxRequiredFieldValidator" runat="server" ControlToValidate="LanguagesTextBox" Text=""
                                                ValidationGroup="RepresentativeValidationGroup" Display="Dynamic" CssClass="text-danger">*</asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="control-label col-md-2"><%=L1.USERNAME %>:</label>
                                        <div class="col-md-10">
                                            <asp:TextBox ID="UsernameTextBox" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <titan:RepresentativePaymentMethod ID="RepresentativePaymentMethod" runat="server" />

                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="col-md-4 text-center col-md-offset-4">
                                            <asp:Button ID="SendButton" runat="server"
                                                ValidationGroup="RepresentativeValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="SendButton_Click" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </asp:PlaceHolder>
                    </div>
                </div>

                <div class="col-md-12">
                    <div class="row-fluid">
                        <asp:PlaceHolder runat="server" ID="PaymentMethodsPlaceHolder" Visible="false">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="col-md-12 text-center m-b-30">
                                        <asp:Button runat="server" CssClass="btn btn-primary customButton" ID="MenuAddNewPaymentMethodButton" OnClick="MenuAddNewPaymentMethodButton_Click" />
                                        &nbsp;&nbsp;&nbsp;
                                        <asp:Button runat="server" CssClass="btn btn-primary customButton" ID="MenuEditPaymentMethodsButton" OnClick="MenuEditPaymentMethodsButton_Click" />
                                    </div>
                                </div>
                            </div>

                            <asp:PlaceHolder runat="server" ID="AddNewPaymentMethodPlaceHolder">
                                <titan:RepresentativePaymentMethod ID="RepresentativePaymentMethodForCurrentRep" runat="server" />

                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="col-md-4 text-center col-md-offset-4">
                                            <asp:Button ID="SavePaymentMethodButton" runat="server" CssClass="btn btn-inverse customButton" OnClick="SavePaymentMethodButton_Click" />
                                        </div>
                                    </div>
                                </div>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder runat="server" ID="ManagePaymentMethodsPlaceHolder" Visible="false">
                                <div class="TitanViewElement">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="table-responsive">
                                                <asp:GridView ID="RepPaymentProcessorsGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                                                    DataSourceID="RepPaymentProcessorsGridView_SqlDataSource" OnPreRender="BaseGridView_PreRender" OnRowDataBound="RepPaymentProcessorsGridView_RowDataBound"
                                                    OnRowCommand="RepPaymentProcessorsGridView_RowCommand">
                                                    <Columns>
                                                        <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                                        <asp:BoundField DataField="Name" HeaderText="<%$ ResourceLookup : PAYMENTPROCESSOR %>" SortExpression="Name" />
                                                        <asp:BoundField DataField="LogoPath" HeaderText="<%$ ResourceLookup : LOGO %>" SortExpression="LogoPath" />
                                                        <asp:BoundField DataField="DepositInfo" HeaderText="<%$ ResourceLookup : DEPOSITINFO %>" SortExpression="DepositInfo" />
                                                        <asp:BoundField DataField="WithdrawalInfo" HeaderText="<%$ ResourceLookup : WITHDRAWALINFO %>" SortExpression="WithdrawalInfo" />

                                                        <asp:TemplateField HeaderText="">
                                                            <ItemStyle />
                                                            <ItemTemplate>
                                                                <asp:LinkButton runat="server"
                                                                    ToolTip='<%$ ResourceLookup : REMOVE %>'
                                                                    CommandName="remove"
                                                                    CommandArgument='<%# Container.DataItemIndex %>'>
                                                        <spin class="fa fa-times fa-lg text-danger"/>
                                                                </asp:LinkButton>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="">
                                                            <ItemStyle />
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="ImageButton4" runat="server"
                                                                    ToolTip='<%$ ResourceLookup : EDIT %>'
                                                                    CommandName="edit"
                                                                    CommandArgument='<%# Container.DataItemIndex %>'>
                                                        <span class="fa fa-pencil-square-o fa-lg text-info"/>
                                                                </asp:LinkButton>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                                <asp:SqlDataSource ID="RepPaymentProcessorsGridView_SqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="RepPaymentProcessorsGridView_SqlDataSource_Init" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                        </asp:PlaceHolder>
                    </div>
                </div>

                <div class="col-md-12">
                    <div class="row-fluid">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <div class="panel-group m-t-20" aria-multiselectable="true" id="accordion">
                                    <asp:PlaceHolder ID="CountriesPlaceHolder" runat="server"></asp:PlaceHolder>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
