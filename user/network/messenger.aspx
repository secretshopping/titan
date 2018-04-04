<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true" CodeFile="messenger.aspx.cs" Inherits="messenger" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <%--<link href="../../Styles/messenger.css?s=d" rel="stylesheet" />--%>
    <script>

        function pageLoad() {
            $('#inner-chat-box').slimScroll({
                height: '70vh',
                start: 'bottom'
            });
            $('#inner-conversation-box').slimScroll({
                height: '70vh',
                start: 'top'
            });
        }

    </script>

    <style>
        .panel.panel-forum {
            border: 2px solid #e2e7eb;
        }

        .forum-list {
            list-style-type: none;
            margin: 0;
            padding: 0;
        }

            .forum-list li {
                padding: 15px;
            }

            .forum-list a:hover,
            .forum-list a:focus,
            .forum-list a:visited {
                text-decoration: none;
            }

            .forum-list .media {
                font-size: 28px;
                float: left;
                width: 64px;
                text-align: center;
                color: rgba(0,0,0,.4);
                line-height: 40px;
                height: 40px;
            }

                .forum-list .media img {
                    max-width: 100%;
                    display: block;
                    border-radius: 50%;
                    height: 40px;
                }

            .forum-list.forum-topic-list .info-container {
                position: relative;
            }

            .forum-list .info-container {
                margin-left: 79px;
            }

            .forum-list.forum-topic-list .info-container .info {
                width: auto;
                float: none;
                padding-right: 100px;
            }

            .forum-list .info-container .info {
                width: 50%;
            }

                .forum-list .info-container .info .title {
                    font-size: 16px;
                    margin: 0 0 5px;
                    font-weight: 600;
                }

                    .forum-list .info-container .info .title a {
                        color: #242a30;
                    }

            .forum-list.forum-topic-list .info-start-end {
                list-style-type: none;
                margin: 0;
                padding: 0;
                line-height: 20px;
                width: 150px;
                white-space: nowrap;
                overflow: hidden;
                text-overflow: ellipsis;
            }

            .forum-list.forum-topic-list .info-container .date-replies {
                position: absolute;
                right: 0;
                top: 5px;
                text-align: center;
                width: 80px;
            }

                .forum-list.forum-topic-list .info-container .date-replies .time {
                    font-size: 11px;
                    line-height: 11px;
                    margin-bottom: 7px;
                }

                .forum-list.forum-topic-list .info-container .date-replies .replies {
                    background: #e2e7eb;
                    padding: 5px 10px;
                    border-radius: 4px;
                }

                    .forum-list.forum-topic-list .info-container .date-replies .replies .total {
                        font-size: 16px;
                        color: #242a30;
                        line-height: 18px;
                        margin-bottom: 2px;
                    }

                    .forum-list.forum-topic-list .info-container .date-replies .replies .text {
                        font-size: 10px;
                        line-height: 12px;
                        font-weight: 400;
                        color: #999;
                        margin-bottom: 2px;
                    }

        .chats .message {
            display: table !important;
            margin-right: 0 !important;
            margin-left: 0 !important;
            border-radius: 10px !important;
        }

        .chats .right .message, .chats .right .date-time {
            float: right !important;
        }

        .chats .left .message, .chats .left .date-time {
            float: left !important;
        }

        .chats .date-time {
            margin-top: 0 !important;
            margin-bottom: 5px !important;
        }

        .chats .name {
            margin-bottom: 0 !important;
        }

        .left .message.my-message {
            margin-left: 20px !important;
        }

        .right .message.my-message {
            margin-right: 20px !important;
        }

        .message.no-direction::after,
        .message.no-direction::before {
            display: none !important;
        }

        .chats .image img {
            border-radius: 50%;
        }
    </style>

</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <asp:PlaceHolder runat="server" ID="StatusPlaceHolder" Visible="false">
        <div class="row">
            <div class="col-md-12">
                <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger">
                    <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                </asp:Panel>
                <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="alert alert-success">
                    <asp:Literal ID="SuccMessage" runat="server"></asp:Literal>
                </asp:Panel>
            </div>
        </div>
    </asp:PlaceHolder>

    <asp:UpdatePanel runat="server" ID="MessagePanel">
        <ContentTemplate>
            <div class="row">
                <div class="col-md-3">
                    <div class="panel panel-inverse panel-conversations">
                        <div class="panel-heading">
                            <div class="panel-heading-btn">
                                <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-default" data-click="panel-expand"><i class="fa fa-expand"></i></a>
                                <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-success" data-click="panel-reload"><i class="fa fa-repeat"></i></a>
                                <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-warning" data-click="panel-collapse"><i class="fa fa-minus"></i></a>
                                <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-danger" data-click="panel-remove"><i class="fa fa-times"></i></a>
                            </div>
                            <h4 class="panel-title"><%=U6000.CONVERSATIONS %></h4>
                        </div>
                        <div class="panel-body bg-silver">
                            <div id="inner-conversation-box" class="panel panel-forum">
                                <!-- begin forum-list -->
                                <ul class="forum-list forum-topic-list">
                                    <asp:PlaceHolder runat="server" ID="ConversationsPlaceHolder"></asp:PlaceHolder>
                                </ul>

                                <!-- end forum-list -->
                            </div>
                        </div>
                        <div class="panel-footer text-center">
                            <a href="user/network/friends.aspx?conv=true" class="btn btn-block btn-primary btn-sm"><%=U6003.NEWCONVERSATION %></a>
                        </div>
                    </div>
                </div>
                <!-- begin col-4 -->
                <div class="col-md-9">
                    <!-- begin panel -->
                    <div class="panel panel-inverse panel-messages">
                        <div class="panel-heading">
                            <div class="panel-heading-btn">
                                <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-default" data-click="panel-expand"><i class="fa fa-expand"></i></a>
                                <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-success" data-click="panel-reload"><i class="fa fa-repeat"></i></a>
                                <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-warning" data-click="panel-collapse"><i class="fa fa-minus"></i></a>
                                <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-danger" data-click="panel-remove"><i class="fa fa-times"></i></a>
                            </div>
                            <h4 class="panel-title">
                                <asp:Literal runat="server" ID="OtherMemberNameLiteral"></asp:Literal></h4>
                        </div>
                        <div class="panel-body">
                            <div id="inner-chat-box">
                                <ul class="chats">
                                    <asp:Literal runat="server" ID="MessagesLiteral"></asp:Literal>
                                </ul>
                            </div>
                        </div>
                        <div class="panel-footer">
                            <form name="send_message_form" data-id="message-form">
                                <asp:Panel class="input-group" runat="server" DefaultButton="SendButton">
                                    <asp:TextBox runat="server" ID="MessageTextBox" MaxLength="1500" CssClass="form-control input-sm"></asp:TextBox>
                                    <span class="input-group-btn">
                                        <asp:Button runat="server" ID="SendButton" CssClass="btn btn-primary btn-sm" OnClick="SendButton_Click" />
                                    </span>
                                </asp:Panel>
                            </form>
                        </div>
                    </div>
                    <!-- end panel -->
                </div>
                <!-- end col-4 -->
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
