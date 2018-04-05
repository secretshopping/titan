<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="facebook.aspx.cs" Inherits="About" %>

<%@ MasterType VirtualPath="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">

    <script type="text/javascript" src="Scripts/gridview.js"></script>

    <script type="text/javascript">
        jQuery(function ($) {
            updatePrice();
        });

        function updatePrice() {
            //Get values
            var price1 = 0.0;
            var price2 = 0.0;
            var price3 = 0.0;

            if ($('#<%=chbFriends.ClientID%>').is(':checked'))
                price1 = parseFloat($('#prajs1').text());

            if ($('#<%=chbProfilePicture.ClientID%>').is(':checked'))
                price2 = parseFloat($('#prajs2').text());

            var selectStringIndex = $('#<%=ddlOptions.ClientID%> option:selected').text().indexOf('-') + 1;
            price3 = parseFloat($('#<%=ddlOptions.ClientID%> option:selected').text().substring(selectStringIndex).replace('<%=Prem.PTC.AppSettings.Site.CurrencySign%>',''))

            var totalPrice = price1 + price2 + price3;

            $('#<%=lblPrice.ClientID%>').text('<%=Prem.PTC.AppSettings.Site.CurrencySign %>' + totalPrice);
        }

        function pageLoad() {
            <%=PageScriptGenerator.GetGridViewCode(DirectRefsGridView) %>
        }

    </script>
    <link rel="stylesheet" href="Styles/WatchAds.css" type="text/css" />

</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header">Facebook</h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead">
                <asp:Literal ID="SubLiteral" runat="server"></asp:Literal></p>
        </div>
    </div>

    <span id="prajs1" class="displaynone"><%=Prem.PTC.AppSettings.Facebook.FriendsRestrictionsCost.ToClearString() %></span>
    <span id="prajs2" class="displaynone"><%=Prem.PTC.AppSettings.Facebook.ProfilePicRestrictionsCost.ToClearString() %></span>

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
                                DataSourceID="SqlDataSource1" OnPreRender="BaseGridView_PreRender" OnRowDataBound="DirectRefsGridView_RowDataBound" OnRowCommand="DirectRefsGridView_RowCommand"
                                OnSelectedIndexChanging="DirectRefsGridView_SelectedIndexChanging" DataKeyNames="FbAdvertId" EmptyDataText="<%$ ResourceLookup : NOFBCAMPS %>">

                                <Columns>
                                    <asp:TemplateField HeaderText="Select" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkSelect" runat="server" />
                                        </ItemTemplate>
                                        <HeaderTemplate>
                                            <input type="checkbox" id="checkAll" class="regular-checkbox mini-checkbox" onclick="<%=this.jsSelectAllCode %>" /><label for="checkAll"></label>
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="FbAdvertId" HeaderText="FbAdvertId" InsertVisible="False" ReadOnly="True" SortExpression="FbAdvertId" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="FbAdvertPackId" HeaderText="FbAdvertPackId" SortExpression="FbAdvertPackId" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:TemplateField HeaderText="URL" SortExpression="TargetUrl">
                                        <ItemTemplate>
                                            <a href='<%# Eval("TargetUrl") %>' target="_blank">
                                                <%# Eval("TargetUrl").ToString() %>
                                            </a>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="MinFriends" HeaderText="<%$ ResourceLookup : FRIENDS %>" SortExpression="MinFriends" />
                                    <asp:CheckBoxField DataField="HasProfilePicRestrictions" HeaderText="<%$ ResourceLookup : PICTURE %>" SortExpression="HasProfilePicRestrictions" />
                                    <asp:BoundField DataField="CreatedBy" HeaderText="CreatedBy" SortExpression="CreatedBy" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="CreatorUsername" HeaderText="CreatorUsername" SortExpression="CreatorUsername" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="CreatorEmail" HeaderText="CreatorEmail" SortExpression="CreatorEmail" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="CreationDate" HeaderText="<%$ ResourceLookup : CREATED %>" SortExpression="CreationDate" DataFormatString="{0:d}" />
                                    <asp:BoundField DataField="StartDate" HeaderText="StartDate" SortExpression="StartDate" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="EndDate" HeaderText="EndDate" SortExpression="EndDate" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="TotalSecsActive" HeaderText="TotalSecsActive" SortExpression="TotalSecsActive" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="Clicks" HeaderText="<%$ ResourceLookup : PROGRESS %>" SortExpression="Clicks" />
                                    <asp:BoundField DataField="EndValue" HeaderText="%" SortExpression="EndValue" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="Clicks" HeaderText="<%$ ResourceLookup : LIKES %>" SortExpression="Clicks" />
                                    <asp:BoundField DataField="Status" HeaderText="<%$ ResourceLookup : STATUS %>" SortExpression="Status" />
                                    <asp:BoundField DataField="StatusLastChangedDate" HeaderText="StatusLastChangedDate" SortExpression="StatusLastChangedDate" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="Price" HeaderText="Price" SortExpression="Price" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
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
                        </div>
                    </div>
                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>

            <asp:View ID="View1" runat="server">
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
                            <titan:FeatureUnavailable runat="server" ID="FacebookUnavailable"></titan:FeatureUnavailable>
                            <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="alert alert-danger fade in m-b-15"
                                ValidationGroup="RegisterUserValidationGroup" DisplayMode="List" ForeColor="White" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12" runat="server" id="FacebookPlaceHolder">

                            <div class="form-horizontal">

                                <div class="form-group">
                                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="URL" CssClass="control-label col-md-2"><%=U6000.FANPAGEURL %>:</asp:Label>
                                    <div class="col-md-6">
                                        <asp:TextBox ID="URL" runat="server" CssClass="form-control"></asp:TextBox>

                                        <asp:RegularExpressionValidator ForeColor="#b70d00" ValidationGroup="RegisterUserValidationGroup" ID="RegularExpressionValidator2" runat="server" ErrorMessage="*"
                                            Display="Dynamic" CssClass="text-danger" ValidationExpression="(^([a-zA-Z0-9]+(\.[a-zA-Z0-9]+)+.*)$)|(http(s)?:\/\/([\w-]+\.)+[\w-]+(\/[\w- .\/?%&amp;=]*)?)" ControlToValidate="URL" Text="">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="URL"
                                            Display="Dynamic" CssClass="text-danger" ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="control-label col-md-2"><%=L1.PACK %>:</label>
                                    <div class="col-md-6">
                                        <asp:DropDownList ID="ddlOptions" runat="server" CssClass="ddl form-control"></asp:DropDownList>
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

                                <div class="form-group">
                                    <label class="control-label col-md-2"><%=L1.NUMBEROFREQFRENDS %>:</label>
                                    <div class="col-md-6">
                                        <div class="row">
                                            <div class="col-md-3 col-sm-3 col-xs-3">
                                                <div class="checkbox">
                                                    <label>
                                                        <asp:CheckBox ID="chbFriends" runat="server" Checked="false" />
                                                        <%=Prem.PTC.AppSettings.Facebook.FriendsRestrictionsCost.ToString() %>
                                                    </label>
                                                </div>
                                            </div>
                                            <div class="col-md-9 col-sm-9 col-xs-9">
                                                <asp:DropDownList ID="ddlFriends" runat="server" CssClass="ddl form-control" Enabled="false"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="control-label col-md-2"><%=L1.PROFILEPICREQ %>:</label>
                                    <div class="col-md-6">
                                        <div class="checkbox">
                                            <label>
                                                <asp:CheckBox ID="chbProfilePicture" runat="server" Checked="false" />
                                                <%=Prem.PTC.AppSettings.Facebook.ProfilePicRestrictionsCost.ToString() %>
                                            </label>
                                        </div>
                                    </div>
                                </div>

                                <titan:TargetBalance runat="server" Feature="Facebook" ID="TargetBalanceRadioButtonList"></titan:TargetBalance>

                                <div class="form-group">
                                    <div class="col-md-12">
                                        <h4><%=L1.PRICE %>:
                                        <asp:Label ID="lblPrice" runat="server" Text="Label"></asp:Label></h4>
                                    </div>
                                </div>

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
                    </div>
                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>
        </asp:MultiView>
    </div>

    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" SelectCommand="SELECT * FROM [FacebookAdverts] WHERE ([CreatorUsername] = @CreatorUsername) AND [Status] != 7 ORDER BY [CreationDate] DESC">
        <SelectParameters>
            <asp:ControlParameter ControlID="UserName" Name="CreatorUsername" PropertyName="Text" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:Label ID="UserName" runat="server" Visible="false"></asp:Label>


</asp:Content>
