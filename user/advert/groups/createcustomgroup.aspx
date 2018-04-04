<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="createcustomgroup.aspx.cs" Inherits="CreateCustomGroup" %>

<%@ MasterType VirtualPath="~/User.master" %>


<asp:Content ContentPlaceHolderID="PageHeadContent" runat="server">
    <link rel="stylesheet" href="Styles/WatchAds.css" type="text/css" />

    <script type="text/javascript">

        
    </script>
</asp:Content>


<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <asp:UpdatePanel ID="MultiViewUpdatePanel" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="CreateGroupButton" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="GroupsAvailableToOpen" EventName="SelectedIndexChanged" />
            <asp:PostBackTrigger ControlID="PictureUploadSubmit" />
        </Triggers>

        <ContentTemplate>
        <div class="tab-content">

            <div class="row">
                <div class="col-md-12">
                    <asp:Panel ID="SPanel" runat="server" Visible="false" CssClass="alert alert-success">
                        <asp:Literal ID="SText" runat="server"></asp:Literal>
                    </asp:Panel>

                    <asp:Panel ID="EPanel" runat="server" Visible="false" CssClass="alert alert-danger">
                        <asp:Literal ID="EText" runat="server"></asp:Literal>
                    </asp:Panel>

                    <asp:Panel ID="WPanel" runat="server" Visible="false" CssClass="alert alert-warning">
                        <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                    </asp:Panel>
                    <asp:ValidationSummary ID="SubmitValidationSummary2" runat="server" CssClass="alert alert-danger"
                        ValidationGroup="CustomGroup" DisplayMode="List" />

                    <asp:ValidationSummary ID="SubmitValidationSummary" runat="server" CssClass="alert alert-danger"
                        ValidationGroup="OnSubmitValidationGroup" DisplayMode="List" />    
                </div>
            </div>
            


            <div class="TitanViewElement">
                <%-- SUBPAGE START --%>
                <div class="row">
                    <div class="col-md-12">
                        <div runat="server" id="AdPacksPlaceHolder" >
                        <h4><%=U4200.AVAILABLE %> <%= AppSettings.RevShare.AdPack.AdPackNamePlural %>: 
                            <asp:Label runat="server" ID="AvailableAdPacksLabel"></asp:Label>,
                            <%=U4200.ALL %> <%= AppSettings.RevShare.AdPack.AdPackNamePlural %>: 
                            <asp:Label runat="server" ID="AllAdPacksLabel"></asp:Label></h4>
                            
                        </div>    
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-12">
                        <div class="form-horizontal">
                            <asp:PlaceHolder runat="server" ID="ControlsPlaceHolder">
                            <div class="form-group">
                                <label class="control-label col-md-2"><%=U4200.GROUPS %></label>
                                <div class="col-md-6">
                                    <div class="input-group">
                                        <div class="radio radio-button-list">
                                            <asp:RadioButtonList runat="server" ID="GroupsAvailableToOpen" AutoPostBack="true" RepeatLayout="Flow" OnSelectedIndexChanged="GroupsAvailableToOpen_SelectedIndexChanged"></asp:RadioButtonList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-md-2"><%= AppSettings.RevShare.AdPack.AdPackNamePlural %></label>
                                <div class="col-md-6">
                                    <asp:TextBox runat="server" CssClass="form-control" ID="AdPacksTextBox" MaxLength="150"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" Display="Dynamic" CssClass="text-danger"
                                    ControlToValidate="AdPacksTextBox" runat="server" ValidationGroup="CustomGroup" ErrorMessage="<%$ ResourceLookup:ENTERREQUIREDFIELDS %>">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-md-2"><%=L1.NAME %></label>
                                <div class="col-md-6">
                                    <asp:TextBox runat="server" CssClass="form-control" ID="GroupNameTextBox" MaxLength="150"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" Display="Dynamic" CssClass="text-danger"
                                    ControlToValidate="GroupNameTextBox" runat="server" ValidationGroup="CustomGroup" ErrorMessage="<%$ ResourceLookup:ENTERREQUIREDFIELDS %>">*</asp:RequiredFieldValidator>
                                </div> 
                            </div>
                            <div class="form-group">
                                <label class="control-label col-md-2"><%=L1.DESCRIPTION %></label>
                                <div class="col-md-6">
                                    <asp:TextBox runat="server" CssClass="form-control" ID="GroupDescriptionTextBox" TextMode="MultiLine" MaxLength="2500" Rows="5"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" Display="Dynamic" CssClass="text-danger"
                                        ControlToValidate="GroupDescriptionTextBox" runat="server" ValidationGroup="CustomGroup" ErrorMessage="<%$ ResourceLookup:ENTERREQUIREDFIELDS %>">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-md-2">
                                    <div class="input-group">
                                        <div class="radio radio-button-list">
                                            <asp:RadioButtonList runat="server" ID="PhotoVideo" AutoPostBack="true" OnSelectedIndexChanged="PhotoVideo_SelectedIndexChanged" RepeatLayout="Flow">
                                            </asp:RadioButtonList>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div runat="server" id="VideoUploadPlaceHolder">
                                        <asp:TextBox runat="server" ID="PromoUrlTextBox" CssClass="form-control" MaxLength="1000" TextMode="MultiLine" Row="5"></asp:TextBox>
                                    </div>
                                    <div runat="server" id="PictureUploadPlaceHolder">
                                       
                                        <span class="btn btn-success fileinput-button">
                                            <i class="fa fa-plus"></i>
                                            <span><%=U6000.ADDFILE %></span>
                                            <asp:FileUpload ID="PictureUpload" runat="server" Width="240px" Style="display: inline;" CssClass="fileupload-control" />
                                        </span>
                                                        
                                        <asp:Button ID="PictureUploadSubmit" Text="Submit" OnClick="PictureUploadSubmit_Click"
                                            CausesValidation="true" runat="server" ValidationGroup="OnSubmitValidationGroup" CssClass="btn btn-primary start" />
                                        <br />
                                        <asp:Image ID="createBannerAdvertisement_BannerImage" runat="server" BorderStyle="Solid" BorderWidth="1px" BorderColor="#e1e1e1" CssClass="m-t-15"/>
                                    </div>
                                    <div runat="server" id="PictureUploadValidatorsPlaceHolder">
                                        <asp:CustomValidator ID="PictureUploadValidCustomValidator"
                                            ControlToValidate="PictureUpload" Display="Dynamic" CssClass="text-danger"
                                            OnServerValidate="PictureUploadValidCustomValidator_ServerValidate"
                                            ValidationGroup="OnSubmitValidationGroup" ValidateEmptyText="true"
                                            runat="server">*</asp:CustomValidator>
                                        <asp:CustomValidator ID="PictureUploadSelectedCustomValidator"
                                            ControlToValidate="PictureUpload" Display="Dynamic" CssClass="text-danger"
                                            OnServerValidate="PictureUploadSelectedCustomValidator_ServerValidate"
                                            ValidationGroup="CustomGroup" ValidateEmptyText="true"
                                            runat="server">*</asp:CustomValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-md-2"><%=U4200.EMAIL %></label>
                                <div class="col-md-6">
                                    <asp:TextBox runat="server" CssClass="form-control" ID="EmailTextBox" MaxLength="100"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-md-2" ><%=U4200.SKYPE %></label>
                                <div class="col-md-6">
                                    <asp:TextBox runat="server" CssClass="form-control" ID="SkypeTextBox" MaxLength="100"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-md-2"><%=U4200.PHONE %></label>
                                <div class="col-md-6">
                                    <asp:TextBox runat="server" CssClass="form-control" ID="PhoneNumberTextBox" MaxLength="50"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-md-2"><%=U4200.FACEBOOKPROFILE %></label>
                                <div class="col-md-6">
                                    <asp:TextBox runat="server" CssClass="form-control" ID="FacebookUrlTextBox" MaxLength="800"></asp:TextBox>
                                    <asp:RegularExpressionValidator ValidationGroup="CustomGroup" ID="RegularExpressionValidator2" runat="server" ErrorMessage="*"
                                        ControlToValidate="FacebookUrlTextBox" Display="Dynamic" CssClass="text-danger" Text="">*</asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-md-3 col-sm-4">
                                    <asp:Button runat="server" ID="CreateGroupButton" OnClick="CreateGroupButton_Click" CssClass="btn btn-inverse btn-block m-t-15" CausesValidation="true" ValidationGroup="CustomGroup" />
                                </div>
                                <div class="col-md-3 col-sm-4 col-sm-offset-4">
                                    <a runat="server" id="CancelButton" class="btn btn-default btn-block m-t-15"><%=U4000.CANCEL %></a>
                                </div>
                            </div>
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
