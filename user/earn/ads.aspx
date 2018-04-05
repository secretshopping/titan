<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true" CodeFile="ads.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <link rel="stylesheet" href="Styles/WatchAds.css" type="text/css" />
    <link rel="stylesheet" href="Scripts/default/assets/plugins/isotope/isotope.css" type="text/css" />
    <script src="Scripts/default/assets/plugins/isotope/jquery.isotope.min.js"></script>
    <script src="Scripts/assets/js/gallery.demo.js"></script>

    <script type="text/javascript">
        function doSubmit(eventArgument, isSurfable) {
            $('#__EVENTARGUMENT5').val(eventArgument);

            if (isSurfable) {
                if ($('#<%=ExternalIFrameLabel.ClientID%>').text() == 'True') {
                    $('#<%=Form.ClientID%>').attr('action', 'user/earn/asurf/asurf.aspx');
                    $('#<%=Form.ClientID%>').attr('target', '_blank');
                }
                else {
                    $('#<%=Form.ClientID%>').attr('action', 'user/earn/surf.aspx?f=1');
                    $('#<%=Form.ClientID%>').attr('target', '_blank');
                }
            }
            $('#<%=Form.ClientID%>').submit();
            $('#<%=Form.ClientID%>').attr('action', 'user/earn/ads.aspx');
            $('#<%=Form.ClientID%>').attr('target', '_self');
        }

        function pageLoad() {
            $('.AboxActive').on('click', function () {
                var AdId = $(this).find('input').val();
                doSubmit(AdId, true);
            });

            $('.AboxActive').on('click', function (e) {
                if ($(e.target).hasClass('starredAdImg')) {
                    return true;
                }
            });

            $('.AboxClicked').on('click', function (e) {
                window.open($(this).attr('data-target-url'));
            });

            //Gallery.init("#<%=CustomCategoriesPlaceHolder.ClientID %>");
            <% if (AppSettings.PtcAdverts.CashLinkViewEnabled == AppSettings.PTCViewMode.PTC)
        { %>
            Gallery.init(".gallery");
            <% } %>
        }

    </script>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <input type="hidden" name="__EVENTARGUMENT5" id="__EVENTARGUMENT5" value="" />
    <asp:Label runat="server" ClientIDMode="Static" ID="ExternalIFrameLabel" CssClass="displaynone"></asp:Label>

    <h1 class="page-header"><%=U6003.PTC %></h1>

    <div class="row">
        <div class="col-md-12">
            <asp:PlaceHolder runat="server" ID="ResetTimeLeftPlaceHolder" Visible="true">
                <div class="alert alert-info">
                    <asp:Label runat="server"><%=U6000.RESETTIME %><%=Prem.PTC.Utils.TimeSpanExtensions.ToFriendlyDisplay(AppSettings.Site.TimeToNextCRONRun, 3) %></asp:Label>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="ClicksToKeepLevelPlaceHolder" Visible="false">
                <div class="whitebox">
                    <asp:Literal runat="server" ID="ClicksToKeepLevelLiteral"></asp:Literal>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>

    <asp:UpdatePanel runat="server" ID="AdRefreshUpdatePanel" OnLoad="AdRefreshUpdatePanel_Load" ClientIDMode="Static">
        <ContentTemplate>
            <div class="tab-content" style="background-color: #d9e0e7;">
                <div id="NoPTCpanelWrapper" visible="false" runat="server" class="row m-t-15">
                    <div class="col-md-12">
                        <p class="alert alert-danger">
                            <asp:Literal ID="NoPTCpanel" Visible="false" runat="server" />
                        </p>
                    </div>
                </div>
                <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
                    <asp:View runat="server" ID="ViewAds">
                        <div class="TitanViewElement" style="background-color: #d9e0e7;">
                            <asp:Panel CssClass="row" ID="GotoMineBTCPanel" runat="server" Visible="false">
                                <div class="text-center"><br />
                                    <button onclick="window.open('user/earn/surf.aspx?f=1&auto=1')" class="btn btn-inverse btn-lg" style="background: #2d353c !important">Start Mining</button>
                                </div>
                            </asp:Panel>
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="form-inline">
                                        <div class="row">

                                            <asp:PlaceHolder ID="searchPlaceHolder" runat="server">
                                                <div class="col-md-4">
                                                    <div class="form-group" id="searchDiv" runat="server">
                                                        <label class="control-label"><%=L1.SEARCH %>:</label>
                                                        <div class="input-group">
                                                            <asp:TextBox ID="SearchTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                                                            <div class="input-group-btn">
                                                                <asp:LinkButton ID="SearchButton" runat="server" CssClass="btn btn-inverse"
                                                                    OnClick="SearchButton_Click"><span class="fa fa-search"></span></asp:LinkButton>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </asp:PlaceHolder>

                                            <asp:PlaceHolder runat="server" ID="SortPlaceHolder">
                                                <div class="col-md-4">
                                                    <div class="form-group" id="sortbydiv" runat="server">
                                                        <label class="control-label"><%=L1.SORTBY %>:</label>

                                                        <asp:DropDownList ID="SortBy1" runat="server" AutoPostBack="true" CssClass="form-control"
                                                            OnInit="SortBy1_Init">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </asp:PlaceHolder>

                                            <div class="col-md-4">
                                                <titan:StartSurfingPtcAd runat="server" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-12 p-t-15">


                                    <div runat="server" id="FilterSection" class="row m-t-30 m-b-30">
                                        <div class="col-md-4">
                                            <div id="options" class="options m-b-10">
                                                <label><%=U5006.CATEGORY %></label>
                                                <span class="gallery-option-set" id="filter" data-option-key="filter" data-option-group="group2">
                                                    <a class="btn btn-default btn-xs" data-option-value="*"><%=U4200.ALL %></a>
                                                    <asp:PlaceHolder ID="CategoryButtonsPlaceHolder" runat="server"></asp:PlaceHolder>
                                                </span>
                                            </div>
                                        </div>
                                        <div class="col-md-4">
                                            <label><%=U5006.FAVORITE %></label>
                                            <div id="favoriteOptions" class="options m-b-10">
                                                <span class="gallery-option-set" id="favoriteFilter" data-option-key="filter" data-option-group="group1">
                                                    <a class="btn btn-default btn-xs" data-option-value="*"><%=U4200.ALL %></a>
                                                    <a class="btn btn-default btn-xs" data-option-value=".fav" runat="server" id="favouriteAnhor"><%=U5006.FAVORITE %></a>
                                                </span>
                                            </div>
                                        </div>
                                        <div class="col-md-4" runat="server" id="ExposureCategoriesPlaceHolder">
                                            <label>Exposure</label>
                                            <div id="exposureOptions" class="options m-b-10">
                                                <span class="gallery-option-set" id="exposureFilter" data-option-key="filter" data-option-group="group3">
                                                    <a class="btn btn-default btn-xs" data-option-value="*"><%=U4200.ALL %></a>
                                                    <a class="btn btn-default btn-xs" data-option-value=".micro">Micro</a>
                                                    <a class="btn btn-default btn-xs" data-option-value=".mini">Mini</a>
                                                    <a class="btn btn-default btn-xs" data-option-value=".fixed">Fixed</a>
                                                    <a class="btn btn-default btn-xs" data-option-value=".standard">Standard</a>
                                                    <a class="btn btn-default btn-xs" data-option-value=".extended">Extended</a>
                                                </span>
                                            </div>
                                        </div>
                                    </div>

                                    <asp:Panel ID="CustomCategoriesPlaceHolder" CssClass="gallery" Visible="false" runat="server">
                                        <%--Content dynamically generated--%>
                                    </asp:Panel>

                                    <asp:PlaceHolder runat="server" ID="AdPacksRequiredPlaceHolder">
                                        <%=String.Format(U5008.ADPACKSREQUIRED, AppSettings.RevShare.AdPack.AdPackNamePlural) %>
                                    </asp:PlaceHolder>

                                    <div class="gallery">
                                        <%--Generate advertisements--%>
                                        <asp:PlaceHolder ID="AdsLiteral" runat="server"></asp:PlaceHolder>
                                        <%--Generate advertisements with no description--%>
                                        <asp:PlaceHolder ID="SmallAdsLiteral" runat="server"></asp:PlaceHolder>
                                    </div>

                                    <asp:PlaceHolder runat="server" ID="AdditionalAdsInfoPlaceHolder" Visible="false">
                                        <div class="alert alert-info">
                                            <asp:Label ID="AdditionalAdsInfoLabel" runat="server">CHANGE_ME_INFO</asp:Label>
                                        </div>
                                    </asp:PlaceHolder>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <asp:Literal ID="ExpiredLiteral" Visible="false" runat="server"></asp:Literal>
                                    <asp:GridView ID="ExpiredFavoriteGridView" Visible="false" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                                        DataSourceID="ExpiredFavoriteGridView_DataSource" OnPreRender="BaseGridView_PreRender" OnRowDataBound="ExpiredFavoriteGridView_RowDataBound">
                                        <Columns>
                                            <asp:BoundField DataField="Title" HeaderText='<%$ ResourceLookup: TITLE %>' SortExpression="Title"></asp:BoundField>
                                            <asp:BoundField DataField="Description" HeaderText='<%$ ResourceLookup: DESCRIPTION %>' SortExpression="Description"></asp:BoundField>
                                            <asp:BoundField DataField="TargetUrl" HeaderText="URL" SortExpression="TargetUrl" />
                                        </Columns>
                                    </asp:GridView>
                                    <asp:SqlDataSource ID="ExpiredFavoriteGridView_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="ExpiredFavoriteGridView_DataSource_Init"></asp:SqlDataSource>

                                </div>
                            </div>
                        </div>
                    </asp:View>

                </asp:MultiView>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
