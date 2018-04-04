<%@ Page Language="C#" AutoEventWireup="true" CodeFile="writing.aspx.cs" Inherits="user_news_writing" MasterPageFile="~/User.master" %>

<%@ MasterType VirtualPath="~/User.master" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <link href="Scripts/default/assets/plugins/bootstrap-tagsinput/bootstrap-tagsinput.css" rel="stylesheet" />
    <script src="Scripts/default/assets/plugins/bootstrap-tagsinput/bootstrap-tagsinput.js"></script>

    <script>
        function pageLoad() {
            var tagField = $('#<%=KeywordsTextBox.ClientID %>');
            var maxNumberOfTags = tagField.data('max');
            var messageString = tagField.data('message');
            var tagsCurrentTags = tagField.val().split(',');
      
            tagField.tagsinput({
                maxTags: maxNumberOfTags
            });

            for (var i = 0; i < tagsCurrentTags.length; i++) {
                tagField.tagsinput('add', tagsCurrentTags[i]);
            }


        <%=PageScriptGenerator.GetGridViewCode(ArticlesGridView) %>

            tagField.on('itemRemoved', function (event) {
                var elem = $(this).parent().find('.bootstrap-tagsinput');
                if (elem.hasClass('bootstrap-tagsinput-max')) {
                    $('#tag-validator').text(messageString).css('visibility', 'visible');
                } else {
                    $('#tag-validator').text('').css('visibility', 'hidden');
                }
            });
            tagField.on('beforeItemMax', function (event) {
                var elem = $(this).parent().find('.bootstrap-tagsinput');
                if (elem.hasClass('bootstrap-tagsinput-max')) {
                    $('#tag-validator').text(messageString).css('visibility', 'visible');
                } else {
                    $('#tag-validator').text('').css('visibility', 'hidden');
                }
            });
        }
    </script>
    <style>
        .bootstrap-tagsinput {
            width: 100%;
        }
    </style>
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


    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <asp:Panel ID="TitanViewPagePanel" runat="server" CssClass="TitanViewPage">
                    <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                        <asp:Button ID="ManageButton" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                        <asp:Button ID="AddNewButton" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                    </asp:PlaceHolder>
                </asp:Panel>
            </div>
        </div>
    </div>
    <div class="tab-content">
        <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">

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
                            <asp:GridView ID="ArticlesGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                                DataSourceID="ArticlesSqlDataSource" OnRowDataBound="ArticlesGridView_RowDataBound" DataKeyNames="Id" OnPreRender="BaseGridView_PreRender"
                                OnRowCommand="ArticlesGridView_RowCommand" EmptyDataText="">
                                <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="Title" HeaderText="<%$ ResourceLookup : TITLE %>" SortExpression="Title" />
                                    <asp:BoundField DataField="CategoryId" HeaderText="<%$ ResourceLookup : CATEGORY %>" SortExpression="CategoryId" />
                                    <asp:BoundField DataField="CreatedDate" HeaderText="<%$ ResourceLookup : DATEADDED %>" SortExpression="CreatedDate" />
                                    <asp:BoundField DataField="Geolocation" HeaderText="<%$ ResourceLookup : COUNTRY %>" SortExpression="Geolocation" />
                                    <asp:BoundField DataField="Clicks" SortExpression="Clicks" />
                                    <asp:BoundField DataField="CreatorMoneyEarned" SortExpression="CreatorMoneyEarned" />
                                    <asp:BoundField DataField="StatusInt" HeaderText="<%$ ResourceLookup : STATUS %>" SortExpression="StatusInt" />

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
            </asp:View>

            <asp:View ID="AddNewArticleView" runat="server" OnActivate="AddNewArticleView_Activate">
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

                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="CountriesList" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                    <ContentTemplate>
                                        <div class="form-group">
                                            <asp:Label ID="Label2" runat="server" CssClass="control-label col-md-2" AssociatedControlID="CountriesList"><%=L1.COUNTRY %>:</asp:Label>
                                            <div class="col-md-6">
                                                <asp:DropDownList ID="CountriesList" CssClass="form-control" runat="server" OnSelectedIndexChanged="CountriesList_SelectedIndexChanged" AutoPostBack="true" />
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <asp:Label ID="Label1" runat="server" CssClass="control-label col-md-2" AssociatedControlID="CategoriesList"><%=L1.CATEGORY %>:</asp:Label>
                                            <div class="col-md-6">
                                                <asp:DropDownList ID="CategoriesList" CssClass="form-control" runat="server" />
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>

                                <div class="form-group">
                                    <asp:Label ID="TitleLabel" runat="server" CssClass="control-label col-md-2" AssociatedControlID="TitleTextBox"><%=L1.TITLE %>:</asp:Label>
                                    <div class="col-md-6">
                                        <asp:TextBox ID="TitleTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="TitleRequired" runat="server" ControlToValidate="TitleTextBox"
                                            Display="Dynamic" CssClass="text-danger" Text=""
                                            ValidationGroup="MiniVideoValidationGroup">*</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="TitleRangeValidator" runat="server" Type="String"
                                            ControlToValidate="TitleTextBox" CssClass="text-danger" ValidationGroup="MiniVideoValidationGroup"></asp:RegularExpressionValidator>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <asp:Label ID="DescriptionLabel" runat="server" CssClass="control-label col-md-2" AssociatedControlID="DescriptionTextBox"><%=U6012.SUBTITLE %>:</asp:Label>
                                    <div class="col-md-6">
                                        <asp:TextBox ID="DescriptionTextBox" runat="server" CssClass="form-control" MaxLength="150"
                                            TextMode="multiline" Columns="50" Rows="2" />
                                        <asp:RequiredFieldValidator ID="DescriptionRequiredFieldValidator" runat="server" ControlToValidate="DescriptionTextBox"
                                            Display="Dynamic" CssClass="text-danger" Text=""
                                            ValidationGroup="MiniVideoValidationGroup">*</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="SubtitleRangeValidator" runat="server" Type="String" ValidationExpression="^.{70,150}$"
                                            ControlToValidate="DescriptionTextBox" CssClass="text-danger" ValidationGroup="MiniVideoValidationGroup"></asp:RegularExpressionValidator>
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
                                    </div>
                                </div>

                                <div class="form-group" style="display: none">
                                    <div class="col-md-6 col-md-offset-2">
                                        <asp:Label ID="VideoLabel" runat="server" />
                                        <asp:Label ID="VideoURLHiddenLabel" runat="server" Visible="false" />
                                    </div>
                                </div>

                                <div class="form-group" style="display: none">
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
                                    </div>
                                </div>

                                <div class="form-group">
                                    <asp:Label ID="Label3" runat="server" CssClass="control-label col-md-2" AssociatedControlID="DescriptionTextBox"><%=L1.TEXT %>:</asp:Label>
                                    <div class="col-md-6">
                                        <div class="ckeContainer">
                                            <CKEditor:CKEditorControl ID="TextCKEditor" ClientIDMode="Static" Height="600px" Toolbar="Link|Unlink|-|Bold|Italic|BulletedList|Format|Undo|Redo|-|youtube"
                                                BasePath="../../Scripts/ckeditor/" runat="server"></CKEditor:CKEditorControl>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <asp:Label ID="Label4" runat="server" CssClass="control-label col-md-2" AssociatedControlID="KeywordsTextBox"><%=U6012.KEYWORDS %>:</asp:Label>
                                    <div class="col-md-6">
                                        <asp:TextBox ID="KeywordsTextBox" runat="server" CssClass="form-control" MaxLength="5000" TextMode="MultiLine"></asp:TextBox>
                                        <span class="text-danger" id="tag-validator"></span>
                                        <span style="font-size: smaller"><%=String.Format(U6012.KEYWORDSINFO, MaxKeywords) %></span>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="control-label col-md-2"><%=L1.VERIFICATION %>: </label>
                                    <div class="col-md-6">
                                        <titan:Captcha runat="server" ID="TitanCaptcha" ValidationGroup="MiniVideoValidationGroup" />
                                        <asp:CustomValidator OnServerValidate="Validate_Captcha" runat="server" ValidationGroup="MiniVideoValidationGroup"
                                            Display="Dynamic" CssClass="text-danger" ID="CustomValidator1" EnableClientScript="False">*</asp:CustomValidator>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <div class="col-md-2">
                                        <asp:Button ID="CreateButton" runat="server"
                                            ValidationGroup="MiniVideoValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="CreateOrEditButton_Click" />
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


    <asp:SqlDataSource ID="ArticlesSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="ArticlesSqlDataSource_Init"></asp:SqlDataSource>


</asp:Content>
