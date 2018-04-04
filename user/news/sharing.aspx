<%@ Page Language="C#" AutoEventWireup="true" CodeFile="sharing.aspx.cs" Inherits="user_news_sharing" MasterPageFile="~/User.master" %>

<%@ MasterType VirtualPath="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script src="Scripts/default/assets/plugins/clipboard/clipboard.min.js"></script>
    <script>
        function pageLoad() {
        <%=PageScriptGenerator.GetGridViewCode(ArticlesGridView) %>
            $(".sharelink").click(function (e) {
                e.preventDefault();
                var sharelink = $(this).parents(".isotope-item").attr("data-link") || "";
                $("#sharelink-target").text(sharelink);
                $modal = $("#confirmationModal");
                $modal.find(".modal-body").html('<p class="modal-text"><%=U6012.THISISSHARELINK %></p><div class="clipboard-wrapper"><pre id="sharelink-target">' + sharelink + '</pre><button type="button" class="clipboard btn btn-inverse height-full" data-click="tooltip" data-placement="top" title="<%=U6000.COPIED %>!" data-clipboard-target="#sharelink-target"><%=U6000.COPY %></button></div>').promise().then(function () {
                    var clipboard = new Clipboard('.clipboard');
                    $('.clipboard').tooltip({ trigger: 'focus' });
                });
                $modal.find("#confirmButton").hide();
                $modal.modal('show');
            });
        }
    </script>
    <link href="Scripts/default/assets/plugins/isotope/isotope.css" rel="stylesheet" />
                            
    <style>
        .modal-text {
            text-align: center;
            font-weight: 700;
            font-size: 18px;
            margin-bottom: 20px;
        }

        .gallery {
            margin-bottom: 50px;
        }

        .gallery,
        .isotope-item {
            z-index: unset;
            position: unset;
        }


        @media (max-width: 768px) {
            .gallery {
                margin: 0 !important;
            }

                .gallery .image.isotope-item {
                    width: 100%;
                }
        }

        @media (min-width: 768px) {
            .gallery .image.isotope-item {
                width: 50%;
            }
        }

        @media (min-width: 1200px) {
            .result-list .desc {
                min-width: 400px;
            }

            .gallery .image.isotope-item {
                width: 33.33333333%;
            }
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
                        <asp:Button ID="ArticlesButton" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                        <asp:Button ID="StatisticsButton" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                    </asp:PlaceHolder>
                </asp:Panel>
            </div>
        </div>
    </div>

    <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">

        <asp:View runat="server" ID="ArticlesView" OnActivate="ArticlesView_Activate">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>

                    <div class="tab-content">

                        <div class="row">
                            <div class="col-md-12">
                                <div class="form">
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <div class="form-group m-r-10">
                                                <label class="control-label"><%=L1.COUNTRY %></label>
                                                <asp:DropDownList ID="CountriesDropDownList" runat="server" AutoPostBack="true" CssClass="form-control"
                                                    OnSelectedIndexChanged="CountriesDropDownList_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="col-sm-6">
                                            <div class="form-group m-r-10">
                                                <label class="control-label"><%=L1.CATEGORY %></label>
                                                <asp:DropDownList ID="CategoriesDropDownList" runat="server" AutoPostBack="true" CssClass="form-control"
                                                    OnSelectedIndexChanged="CategoriesDropDownList_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="col-sm-12">
                                            <div class="form-group m-r-10" id="searchDiv" runat="server">
                                                <div class="input-group">
                                                    <asp:TextBox ID="SearchTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                                                    <div class="input-group-btn">
                                                        <asp:LinkButton ID="SearchButton" runat="server" CssClass="btn btn-inverse"
                                                            OnClick="SearchButton_Click"><span class="fa fa-search m-r-5"></span> <%=L1.SEARCH %></asp:LinkButton>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            <div class="row result-list gallery isotope">
                                <asp:PlaceHolder ID="ArticlesPlaceHolder" runat="server"></asp:PlaceHolder>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:View>

        <asp:View ID="StatisticsView" runat="server">
            <div class="tab-content">
                <div class="TitanViewElementSimple">
                    <div class="row">
                        <div class="col-md-6">
                            <h3><%=U6012.READS %></h3>
                            <titan:Statistics runat="server" StatType="User_ArticleSharesReads" Height="230px" IsInt="true"></titan:Statistics>
                        </div>
                        <div class="col-md-6">
                            <h3><%=L1.ALLCREDITEDMONEY %></h3>
                            <titan:Statistics runat="server" StatType="User_ArticleSharesMoney" Height="230px"></titan:Statistics>
                        </div>
                    </div>

                    <h3><%=U6012.TOPLIST %></h3>

                    <div class="row">
                        <div class="col-md-12">
                            <asp:GridView ID="ArticlesGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                                DataSourceID="ArticlesSqlDataSource" OnRowDataBound="ArticlesGridView_RowDataBound" OnPreRender="BaseGridView_PreRender">
                                <Columns>
                                    <asp:BoundField DataField="Title" HeaderText="<%$ ResourceLookup : TITLE %>" SortExpression="Title" />
                                    <asp:BoundField DataField="Category" HeaderText="<%$ ResourceLookup : CATEGORY %>" SortExpression="Category" />
                                    <asp:BoundField DataField="Geolocation" HeaderText="<%$ ResourceLookup : COUNTRY %>" SortExpression="Geolocation" />
                                    <asp:BoundField DataField="Reads" SortExpression="Reads" />
                                    <asp:BoundField DataField="Reads" SortExpression="Reads" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </asp:View>
    </asp:MultiView>

    <asp:SqlDataSource ID="ArticlesSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="ArticlesSqlDataSource_Init"></asp:SqlDataSource>


</asp:Content>

