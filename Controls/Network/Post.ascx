<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Post.ascx.cs" Inherits="PostControl" %>
<asp:UpdatePanel runat="server">
    <Triggers>
        <asp:PostBackTrigger ControlID="ImageUploadButton" />
    </Triggers>
    <ContentTemplate>

        <li runat="server" id="PostPlaceHolder" class="post">
            <div class="post-wrapper">
            <!-- begin timeline-time -->
            <div class="timeline-time">
                <span runat="server" id="PostDate" class="date"></span>
                <span runat="server" id="PostTime" class="time"></span>
            </div>
            <!-- end timeline-time -->
            <!-- begin timeline-icon -->
            <div class="timeline-icon">
                <a href="javascript:;"><i class="fa fa-circle"></i></a>
            </div>
            <!-- end timeline-icon -->
            <!-- begin timeline-body -->
            <div class="timeline-body">
                <div class="timeline-header">
                    <span class="userimage">
                        <asp:Image runat="server" ID="PostAuthorImage" />
                    </span>
                    <span runat="server" id="PostAuthor" class="username"></span>
                </div>
                <div class="timeline-content">
                    <p class="m-t-20">
                        <asp:Image runat="server" ID="PostImage" />
                    </p>

                    <p runat="server" id="PostText"></p>

                </div>
                <div class="timeline-footer">
                
                    <asp:LinkButton runat="server" ID="AddCommentButton" OnClick="AddCommentButton_Click" ></asp:LinkButton>
                    <asp:PlaceHolder runat="server" ID="NewCommentPlaceHolder">
                        <div class="form-horizontal">
                            <asp:LinkButton runat="server" ID="CancelCommentButton" OnClick="CancelCommentButton_Click" Visible="false"></asp:LinkButton>
                            <asp:Panel ID="EPanel" runat="server" Visible="false" CssClass="alert alert-danger m-t-15">
                                <asp:Label ID="ELabel" runat="server"></asp:Label>
                            </asp:Panel>
                            <asp:Panel ID="SPanel" runat="server" Visible="false" CssClass="alert alert-success m-t-15">
                                <asp:Label ID="SLabel" runat="server"></asp:Label>
                            </asp:Panel>
                            <div class="form-group m-t-15">
                                <div class="col-md-12">
                                    <asp:TextBox runat="server" ID="NewCommentTextBox" CssClass="form-control" MaxLength="1000" Rows="3" TextMode="MultiLine"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-12 m-b-15">
                                <asp:Image runat="server" ID="ImagePreview" />
                            </div>
                            <div class="form-group">
                                <div class="col-md-12">
                                    <span class="btn btn-success btn-xs fileinput-button">
                                        <i class="fa fa-plus"></i>
                                        <span><%=U6000.ADDFILE %></span>
                                        <asp:FileUpload ID="ImageUpload" runat="server" />
                                    </span>
                                    <asp:Button ID="ImageUploadButton" Text="Submit" OnClick="ImageUploadButton_Click"
                                        CausesValidation="true" runat="server" ValidationGroup="MarketplaceAdd_OnSubmitValidationGroup" CssClass="btn btn-primary btn-xs" />
                                    <asp:Button runat="server" ID="ConfirmCommentButton" OnClick="ConfirmCommentButton_Click" CssClass="btn btn-inverse pull-right" />
                                </div>
                            </div>                        
                        
                        
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="CommentsPlaceHolder">
                        <div runat="server" id="CommentsDiv"></div>
                    </asp:PlaceHolder>
                </div>
            </div>
            <!-- end timeline-body -->
        </div>
    </li>
    </ContentTemplate>
</asp:UpdatePanel>
