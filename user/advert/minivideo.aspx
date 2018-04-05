<%@ Page Language="C#" AutoEventWireup="true" CodeFile="minivideo.aspx.cs" Inherits="user_advert_minivideo" MasterPageFile="~/User.master" %>

<%@ MasterType VirtualPath="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script>
        function pageLoad() {
             <%=PageScriptGenerator.GetGridViewCode(UserMiniVideosGridView) %>
        }
    </script>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header">
        <asp:Literal ID="TitleLiteral" runat="server"></asp:Literal></h1>

    <div class="row">
        <div class="col-md-12">
            <p class="lead">
                <asp:Literal ID="SubLiteral" runat="server"></asp:Literal></p>
        </div>
    </div>


    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <asp:Panel ID="TitanViewPagePanel" runat="server" CssClass="TitanViewPage">
                    <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                        <asp:Button ID="ManageButton" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                        <asp:Button ID="AddNewButton" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />                        
                    </asp:PlaceHolder>
                </asp:Panel>
            </div>
        </div>
    </div>
    <div class="tab-content">
        <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">   
            <asp:View ID="AddVideoView" runat="server" OnActivate="AddVideoView_Activate">
                <div id="view1h" class="TitanViewElementSimple">
                    <%-- SUBPAGE START --%>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger">
                                <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                            </asp:Panel>

                            <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="alert alert-success">
                                <asp:Literal ID="SuccMessage" runat="server"></asp:Literal>
                            </asp:Panel>
                            <asp:ValidationSummary ID="MiniVideoValidationGroupSummary" runat="server" CssClass="alert alert-danger"
                                ValidationGroup="MiniVideoValidationGroup" DisplayMode="List" />
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12" runat="server">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <asp:Label ID="TitleLabel" runat="server" CssClass="control-label col-md-2" AssociatedControlID="TitleTextBox"><%=L1.TITLE %>:</asp:Label>
                                    <div class="col-md-6">
                                        <asp:TextBox ID="TitleTextBox" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="TitleRequired" runat="server" ControlToValidate="TitleTextBox"
                                            Display="Dynamic" CssClass="text-danger" Text=""
                                            ValidationGroup="MiniVideoValidationGroup">*</asp:RequiredFieldValidator>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <asp:Label ID="DescriptionLabel" runat="server" CssClass="control-label col-md-2" AssociatedControlID="DescriptionTextBox"><%=L1.DESCRIPTION %>:</asp:Label>
                                    <div class="col-md-6">
                                        <asp:TextBox ID="DescriptionTextBox" runat="server" CssClass="form-control" MaxLength="100"
                                            TextMode="multiline" Columns="50" Rows="5" />
                                        <asp:RequiredFieldValidator ID="DescriptionRequiredFieldValidator" runat="server" ControlToValidate="DescriptionTextBox"
                                            Display="Dynamic" CssClass="text-danger" Text=""
                                            ValidationGroup="MiniVideoValidationGroup">*</asp:RequiredFieldValidator>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <asp:Label ID="Label1" runat="server" CssClass="control-label col-md-2"><%=L1.CATEGORY %>:</asp:Label>
                                    <div class="col-md-6">
                                        <asp:DropDownList ID="VideoCategoriesList" CssClass="form-control"  runat="server" />
                                    </div>
                                </div>

                                <div class="form-group">
                                    <div class="col-md-6 col-md-offset-2">
                                        <asp:Image ID="VideoImage" runat="server" CssClass="img-responsive" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="control-label col-md-2"><%=L1.IMAGE %>:</label>
                                    <div class="col-md-6">
                                        <span class="btn btn-success fileinput-button">
                                            <i class="fa fa-plus"></i>
                                            <span><%=U6000.ADDFILE %></span>
                                            <asp:FileUpload ID="ImageFileUpload" runat="server" />
                                        </span>
                                        <asp:Button ID="VideoImageButton" Text="<%$ResourceLookup: SUBMIT %>" OnClick="VideoImageButton_Click"
                                            CausesValidation="true" runat="server" ValidationGroup="MiniVideoUploadValidationGroup" CssClass="btn btn-primary" />
                                        <asp:CustomValidator ID="ImageSubmitCustomValidator" ControlToValidate="ImageFileUpload" Display="Dynamic" CssClass="text-danger"
                                            OnServerValidate="ImageSubmitValidator_ServerValidate" ValidationGroup="MiniVideoUploadValidationGroup" ValidateEmptyText="true"
                                            runat="server" />
                                        <br />
                                        <asp:CustomValidator ID="ImageServerCustomValidator" Display="Dynamic" CssClass="text-danger"
                                            OnServerValidate="ImageValidator_ServerValidate" ValidationGroup="MiniVideoValidationGroup" ValidateEmptyText="true"
                                            runat="server" />
                                    </div>
                                </div>

                                <div class="form-group">
                                    <div class="col-md-6 col-md-offset-2">
                                        <asp:Label ID="VideoLabel" runat="server" />
                                        <asp:Label ID="VideoURLHiddenLabel" runat="server" Visible="false" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="control-label col-md-2"><%=U6008.VIDEO %>:</label>
                                    <div class="col-md-6">
                                        <span class="btn btn-success fileinput-button">
                                            <i class="fa fa-plus"></i>
                                            <span><%=U6000.ADDFILE %></span>
                                            <asp:FileUpload ID="VideoFileUpload" runat="server" />
                                        </span>
                                        <asp:Button ID="VideoSourceButton" Text="<%$ResourceLookup: SUBMIT %>" OnClick="VideoSourceButton_Click"
                                            CausesValidation="true" runat="server" ValidationGroup="MiniVideoSourceUploadValidationGroup" CssClass="btn btn-primary" />
                                        <asp:CustomValidator ID="VideoSubminCustomValidator" ControlToValidate="VideoFileUpload" Display="Dynamic" CssClass="text-danger"
                                            OnServerValidate="VideoSubmitValidator_ServerValidate" ValidationGroup="MiniVideoSourceUploadValidationGroup" ValidateEmptyText="true"
                                            runat="server" />
                                        <br />
                                        <asp:CustomValidator ID="VideoServerCustomValidator" Display="Dynamic" CssClass="text-danger"
                                            OnServerValidate="VideoValidator_ServerValidate" ValidationGroup="MiniVideoValidationGroup" ValidateEmptyText="true"
                                            runat="server" />
                                    </div>
                                </div>

                                <div class="form-group">
                                    <h4><%=L1.PRICE %>:
                                        <asp:Label ID="lblPrice" runat="server"></asp:Label></h4>
                                </div>

                                <titan:TargetBalance runat="server" Feature="MiniVideo" ID="TargetBalanceRadioButtonList"></titan:TargetBalance>

                                <div class="form-group">
                                    <div class="col-md-2">
                                        <asp:Button ID="CreateVideoButton" runat="server"
                                            ValidationGroup="MiniVideoValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="CreateVideoButton_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>

             <asp:View runat="server" ID="manageView" OnActivate="manageView_Activate">
                <div class="TitanViewElementSimple">                   
                    <div class="row">
                        <div class="col-md-12">
                            <asp:Panel ID="ErrorMessagePanel2" runat="server" Visible="false" CssClass="alert alert-danger">
                                <asp:Literal ID="ErrorMessage2" runat="server"></asp:Literal>
                            </asp:Panel>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                                <asp:GridView ID="UserMiniVideosGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                                    DataSourceID="SqlDataSource1" OnRowDataBound="UserMiniVideosGridView_RowDataBound" DataKeyNames="Id" OnPreRender="BaseGridView_PreRender"
                                    OnRowCommand="UserMiniVideosGridView_RowCommand" EmptyDataText="<%$ ResourceLookup : YOUDONTHAVEANYVIDEOUPLOADED %>">
                                    <Columns>
                                        <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                        <asp:BoundField DataField="Title" HeaderText="<%$ ResourceLookup : TITLE %>" SortExpression="Title" />
                                        <asp:BoundField DataField="VideoCategory" HeaderText="<%$ ResourceLookup : CATEGORY %>" SortExpression="VideoCategory" />
                                        <asp:BoundField DataField="Status" HeaderText="<%$ ResourceLookup : STATUS %>" SortExpression="Status" />
                                        <asp:BoundField DataField="AddedDate" HeaderText="<%$ ResourceLookup : DATEADDED %>" SortExpression="AddedDate" />

                                        <asp:TemplateField HeaderText="">
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" CssClass="pull-right"
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
            </asp:View>
        </asp:MultiView>
    </div>


    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" SelectCommand="SELECT * FROM [MiniVideoCampaigns] WHERE ([Username] = @Username) AND [Status] != 4 ORDER BY [AddedDate] DESC">
        <SelectParameters>
            <asp:ControlParameter ControlID="UserName" Name="Username" PropertyName="Text" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:Label ID="UserName" runat="server" Visible="false"></asp:Label>


</asp:Content>
