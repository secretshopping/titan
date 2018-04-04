<%@ Page Language="C#" AutoEventWireup="true" CodeFile="intextads.aspx.cs" Inherits="user_publish_intextads" MasterPageFile="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script src="Scripts/default/assets/plugins/clipboard/clipboard.min.js"></script>
    <script>
        function pageLoad() {
            var clipboard = new Clipboard('.clipboard');
            $('.clipboard').tooltip({ trigger: 'focus' });
        }
    </script>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <asp:UpdatePanel ID="MultiViewUpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <h1 class="page-header"><%= U6002.INTEXTADS%></h1>
            <div class="row">
                <div class="col-md-12">
                    <p class="lead"><%=U6002.INTEXTADSDESCFORPUBLISHER %></p>
                </div>
            </div>
            <div class="tab-content">
                <div class="TitanViewElement">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-horizontal" runat="server" id="GetCodePlaceHolder">
                                <div class="row">
                                    <div class="form-group">
                                        <label class="control-label col-md-2"><%=U6000.WEBSITES %>:</label>
                                        <div class="col-md-3">
                                            <asp:DropDownList runat="server" ID="WebsitesDDL" class="form-control" OnInit="WebsitesDDL_Init"
                                                OnSelectedIndexChanged="WebsitesDDL_SelectedIndexChanged" AutoPostBack="true">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="form-group">
                                        <asp:PlaceHolder runat="server" ID="IFramePlaceHolder">
                                            <div class="col-md-12">
                                                <div class="clipboard-wrapper">
                                                    <pre id="<%=JSLiteral.ClientID %>"><asp:Literal runat="server" ID="JSLiteral"></asp:Literal></pre>
                                                    <button type="button" class="clipboard btn btn-inverse height-full" data-click="tooltip" data-placement="top" title="<%=U6000.COPIED %>!" data-clipboard-target="#<%=JSLiteral.ClientID %>"><%=U6000.COPY %></button>
                                                </div>
                                            </div>
                                        </asp:PlaceHolder>
                                        <div class="alert alert-warning" runat="server" id="IFrameUnavailablePlaceHolder">
                                            <%=U6000.WEBSITEMUSTBEACCEPTED %>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <titan:FeatureUnavailable runat="server" ID="GetCodeUnavailable"></titan:FeatureUnavailable>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
