<%@ Page Language="C#" AutoEventWireup="true" CodeFile="banners.aspx.cs" Inherits="user_publish_banners" MasterPageFile="~/User.master" %>

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

            <h1 class="page-header"><%= L1.BANNERS%></h1>
            <div class="row">
                <div class="col-md-12">
                    <p class="lead"><%=U6000.PUBLISHERBANNERINFO %></p>
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
            <div class="tab-content">
                <div class="TitanViewElement">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-horizontal" runat="server" id="GetCodePlaceHolder">
                                <div class="form-group">
                                    <label class="control-label col-md-2"><%=U6000.WEBSITES %>:</label>
                                    <div class="col-md-3">
                                        <asp:DropDownList runat="server" ID="WebsitesDDL" class="form-control" OnInit="WebsitesDDL_Init"
                                            OnSelectedIndexChanged="WebsitesDDL_SelectedIndexChanged" AutoPostBack="true">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="control-label col-md-2"><%=U6000.DIMENSIONS %>:</label>
                                    <div class="col-md-3">
                                        <asp:DropDownList runat="server" ID="DimensionsDDL" class="form-control" OnInit="DimensionsDDL_Init"
                                            OnSelectedIndexChanged="DimensionsDDL_SelectedIndexChanged" AutoPostBack="true">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-md-12">
                                        <asp:PlaceHolder runat="server" ID="IFramePlaceHolder">
                                            <div class="clipboard-wrapper">
                                                <pre id="<%=IframeLiteral.ClientID %>"><asp:Literal runat="server" ID="IframeLiteral"></asp:Literal></pre>
                                                <button type="button" class="clipboard btn btn-inverse height-full" data-click="tooltip" data-placement="top" title="<%=U6000.COPIED %>!" data-clipboard-target="#<%=IframeLiteral.ClientID %>"><%=U6000.COPY %></button>
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
