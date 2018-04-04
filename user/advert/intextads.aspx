<%@ Page Language="C#" AutoEventWireup="true" CodeFile="intextads.aspx.cs" Inherits="user_advert_intextads" MasterPageFile="~/User.master" %>

<asp:Content ContentPlaceHolderID="PageHeadContent" runat="server">
    <link href="Scripts/default/assets/plugins/bootstrap-tagsinput/bootstrap-tagsinput.css" rel="stylesheet" />
    <script src="Scripts/default/assets/plugins/bootstrap-tagsinput/bootstrap-tagsinput.js"></script>
    <script>
        function pageLoad() {
            var tagField = $('#<%=TagsTextBox.ClientID %>');
            var maxNumberOfTags = tagField.data('max');
            var messageString = tagField.data('message');
            tagField.tagsinput({
                maxTags: maxNumberOfTags
            });
         
            <%=PageScriptGenerator.GetGridViewCode(MyInTextAdsGridView) %>

            tagField.on('itemRemoved', function(event) {
                var elem = $(this).parent().find('.bootstrap-tagsinput');
                if(elem.hasClass('bootstrap-tagsinput-max')) {
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
    <asp:UpdatePanel ID="MultiViewUpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>   
            <h1 class="page-header"><%=U6002.INTEXTADS%></h1>
            <div class="row">
                <div class="col-md-12">
                    <p class="lead"><%=U6002.INTEXTADSDESC%></p>
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
                                <asp:Button ID="MenuButtonMyAds" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                                <asp:Button ID="MenuButtonCreateAd" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </div>
            <div class="tab-content">
                <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
                    <asp:View runat="server" ID="CreateAdView" OnActivate="CreateAdView_Activate">
                        <div class="TitanViewElement">
                            <div class="row">
                                <div class="row">
                                    <div class="col-md-12">
                                        <asp:ValidationSummary runat="server" CssClass="alert alert-danger fade in m-b-15"
                                            ValidationGroup="CreateInTextAdValidationGroup" DisplayMode="List" />
                                    </div>
                                </div>
                                <div class="col-md-8">
                                    <div class="form-horizontal" runat="server" id="CreateAdvertisementPlaceHolder">
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=L1.PACK %>:</label>
                                            <div class="col-md-6">
                                                <asp:DropDownList runat="server" ID="PacksDDL" class="form-control" OnInit="PacksDDL_Init" AutoPostBack="true" OnSelectedIndexChanged="PacksDDL_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=L1.TITLE %>:</label>
                                            <div class="col-md-6">
                                                <asp:TextBox runat="server" ID="TitleTextBox" CssClass="form-control" MaxLength="40"></asp:TextBox>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="TitleTextBox" ID="TitleRequiredFieldValidator"
                                                    ValidationGroup="CreateInTextAdValidationGroup" CssClass="text-danger">*</asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=L1.DESCRIPTION %>:</label>
                                            <div class="col-md-6">
                                                <asp:TextBox runat="server" ID="DescriptionTextBox" CssClass="form-control" MaxLength="100"></asp:TextBox>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="DescriptionTextBox"
                                                    ID="DescriptionRequiredFieldValidator" ValidationGroup="CreateInTextAdValidationGroup" CssClass="text-danger">*</asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=U6002.TAGS %>:</label>
                                            <div class="col-md-6">
                                                <asp:TextBox runat="server" ID="TagsTextBox" CssClass="form-control" TextMode="MultiLine"></asp:TextBox>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="TagsTextBox"
                                                    ID="TagsRequiredFieldValidator" ValidationGroup="CreateInTextAdValidationGroup" CssClass="text-danger" Display="Dynamic">*</asp:RequiredFieldValidator>
                                                <span class="text-danger" id="tag-validator"></span>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="control-label col-md-2">URL:</label>
                                            <div class="col-md-6">
                                                <div class="input-group">
                                                    <div class="radio radio-button-list">
                                                        <asp:RadioButtonList runat="server" ID="UserUrlsRadioButtonList" OnInit="UserUrlsRadioButtonList_Init" RepeatLayout="Flow"></asp:RadioButtonList>
                                                    </div>
                                                </div>
                                                <asp:CustomValidator runat="server" ID="UrlRequiredValidator" OnServerValidate="UrlRequiredValidator_ServerValidate"
                                                    ValidationGroup="CreateInTextAdValidationGroup" CssClass="text-danger">*
                                                </asp:CustomValidator>
                                            </div>
                                        </div>
                                        <titan:TargetBalance runat="server" Feature="InTextAds" ID="TargetBalanceRadioButtonList"></titan:TargetBalance>
                                        <div class="form-group">
                                            <div class="col-md-2">
                                                <asp:Button ID="CreateInTextAdButton" runat="server" ValidationGroup="CreateInTextAdValidationGroup"
                                                    CssClass="btn btn-inverse btn-block" OnClick="CreateInTextAdButton_Click" />
                                            </div>
                                        </div>
                                    </div>
                                    <titan:FeatureUnavailable runat="server" ID="NewAdUnavailable"></titan:FeatureUnavailable>
                                </div>
                            </div>
                        </div>
                    </asp:View>
                    <asp:View runat="server" ID="MyAdsView" OnActivate="MyAdsView_Activate">
                        <div class="TitanViewElement">
                            <div class="row">
                                <div class="col-md-12">
                                
                                        <asp:GridView ID="MyInTextAdsGridView" runat="server" AllowPaging="True" AllowSorting="True"
                                            DataSourceID="MyInTextAdsGridView_DataSource" OnPreRender="BaseGridView_PreRender"
                                            OnRowDataBound="MyInTextAdsGridView_RowDataBound" PageSize="20" OnDataBound="MyInTextAdsGridView_DataBound"
                                            EmptyDataText="<%$ ResourceLookup : NODATA %>">
                                            <Columns>
                                                <asp:BoundField DataField='Id' HeaderText='Id' SortExpression='Id'  />
                                                <asp:BoundField DataField='Title' SortExpression='Title' />
                                                <asp:BoundField DataField='Description' SortExpression='Description' />
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <a href='<%#Eval("Url")%>'><%# Eval("Url").ToString() %></a>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField='ClicksReceived' SortExpression='ClicksReceived' />
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:GridView runat="server" ID="MyInTextAdsTagsGridView" AutoGenerateColumns="false" SkinID="-1" GridLines="None" HeaderStyle-CssClass="displaynone">
                                                            <Columns>
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        #<%#Eval("Tag") %>
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
                                  
                                    <asp:SqlDataSource ID="MyInTextAdsGridView_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
                                        OnInit="MyInTextAdsGridView_DataSource_Init"></asp:SqlDataSource>

                                </div>
                            </div>
                        </div>
                    </asp:View>
                </asp:MultiView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
