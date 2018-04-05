<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true" CodeFile="profile.aspx.cs" Inherits="profile" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script>
        function pageLoad() {
            // temporary dummy loader
            if ($(".timeline li").length > 1) {
                $(".timeline").append('<li><div class="timeline-icon"><a href="javascript:;"><i class="fa fa-pulse fa-spinner"></i></a></div></li>');
            }
        }
    </script>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <asp:UpdatePanel runat="server" ID="ProfilePanel">
        <Triggers>
            <asp:PostBackTrigger ControlID="ImageUploadButton" />
        </Triggers>
        <ContentTemplate>

            <div class="row">
                <div class="col-md-12">
                    <asp:Panel ID="EPanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                        <asp:Label ID="ELabel" runat="server"></asp:Label>
                    </asp:Panel>
                    <asp:Panel ID="SPanel" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
                        <asp:Label ID="SLabel" runat="server"></asp:Label>
                    </asp:Panel>
                </div>
            </div>

            <div class="row">
                <div class="col-md-3 timeline-profile">

                    <div class="panel panel-inverse">
                        <div class="panel-heading m-b-20">
                            <div class="panel-heading-btn">
                                <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-default" data-click="panel-expand"><i class="fa fa-expand"></i></a>
                                <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-success" data-click="panel-reload"><i class="fa fa-repeat"></i></a>
                                <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-warning" data-click="panel-collapse"><i class="fa fa-minus"></i></a>
                                <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-danger" data-click="panel-remove"><i class="fa fa-times"></i></a>
                            </div>
                            <h4 class="panel-title">Profile</h4>
                        </div>
                        <div class="panel-body">
                            <titan:MemberInfo ID="MemberInfo" runat="server"></titan:MemberInfo>

                            <div class="row">
                                <div class="col-md-8 col-md-offset-2 p-20">
                                    <asp:Button runat="server" ID="BefriendButton" CssClass="btn btn-primary btn-block m-5 text-wrap" OnClick="BefriendButton_Click" />
                                    <asp:Button runat="server" ID="MessageButton" CssClass="btn btn-info btn-block m-5 text-wrap" OnClick="MessageButton_Click" />
                                    <asp:Button runat="server" ID="RejectRequestButton" CssClass="btn btn-danger btn-block m-5 text-wrap" OnClick="RejectRequestButton_Click" />
                                    <asp:Button runat="server" ID="AcceptRequestButton" CssClass="btn btn-success btn-block m-5 text-wrap" OnClick="AcceptRequestButton_Click" />
                                    <asp:Label runat="server" ID="RequestSent"></asp:Label>
                                </div>
                            </div>

                            <ul class="registered-users-list clearfix">
                                <asp:PlaceHolder runat="server" ID="FriendInfoPlaceHolder"></asp:PlaceHolder>
                            </ul>
                        </div>
                    </div>



                </div>
                <div class="col-md-8 col-md-offset-4">
                    <ul class="timeline">
                        <li class="post new-post" runat="server" id="NewPostPlaceHolder">
                            <div class="post-wrapper">
                                <!-- begin timeline-icon -->
                                <div class="timeline-icon">
                                    <a href="javascript:;" class="primary-color"><i class="fa fa-plus"></i></a>
                                </div>
                                <!-- end timeline-icon -->
                                <!-- begin timeline-body -->
                                <div class="timeline-body">
                                    <div class="timeline-content">
                                        <div class="form-group m-t-15">
                                            <div class="col-md-12">
                                                <asp:TextBox runat="server" ID="NewPostTextBox" CssClass="form-control" placeholder="What's on your mind?" MaxLength="1000" Rows="5" TextMode="MultiLine"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-12 m-b-15 m-t-10">
                                            <asp:Image runat="server" ID="ImagePreview" />
                                        </div>
                                        <div class="form-group">
                                            <div class="col-md-12">
                                                <span class="btn btn-success btn-xs fileinput-button">
                                                    <i class="fa fa-plus"></i>
                                                    <span><%=U6000.ADDFILE %></span>
                                                    <asp:FileUpload runat="server" ID="ImageUpload" />
                                                </span>
                                                <asp:Button runat="server" ID="ImageUploadButton" CssClass="btn btn-primary btn-xs" Text="Submit" OnClick="ImageUploadButton_Click" />
                                                <asp:Button runat="server" ID="NewPostButton" CssClass="btn btn-inverse pull-right" OnClick="NewPostButton_Click" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="timeline-footer primary-color"></div>
                                </div>
                                <!-- end timeline-body -->
                            </div>
                        </li>
                        <asp:PlaceHolder runat="server" ID="PostsPlaceHolder"></asp:PlaceHolder>

                    </ul>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
