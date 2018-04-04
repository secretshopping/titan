<%@ Page Language="C#" AutoEventWireup="true" CodeFile="defaultnews.aspx.cs" Inherits="sites_defaultnews" MasterPageFile="~/Sites.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="ContentHeadContent">
    <link href="Scripts/news/assets/css/style.min.css" rel="stylesheet" />
    <link href="Scripts/news/assets/css/style-responsive.min.css" rel="stylesheet" />
    <link href="Scripts/news/assets/css/default.css" rel="stylesheet" />
    <link href="Scripts/news/assets/css/custom.css" rel="stylesheet" />
    <script src="Scripts/news/assets/js/apps.min.js"> </script>
    <script src="Scripts/news/assets/js/clamp.min.js"> </script>
    <script>
        $(function () {
            var textToClamp = document.getElementsByClassName("ellipsis");
            $("#page-container").removeClass("fade");
            $clamp(textToClamp[0], { clamp: '200px' });
        });
    </script>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">

    <div class="language-menu-section">
        <div class="container">
            <div class="row">
                <div class="col-md-12">
                    <div class="language-menu-wrapper">
                        <nav class="language-menu">
                            <asp:PlaceHolder ID="LangPanel" runat="server"></asp:PlaceHolder>
                        </nav>
                    </div>
                </div>
            </div>

        </div>
    </div>

    <!-- begin #page-title -->
    <div id="page-title" class="page-title has-bg">
        <div class="bg-cover">
            <img src="Scripts/news/assets/img/cover.jpg" alt="" /></div>
        <div class="container">
            <h1><%=U6013.NEWSPORTALINTRO %></h1>
            <p><%=String.Format(U6013.NEWSPORTALSUBINFO, AppSettings.Site.Name) %></p>
        </div>
    </div>
    <!-- end #page-title -->

    <div id="content" class="content">
        <!-- begin container -->
        <div class="container">
            <div class="row row-space-30">
                <div class="col-md-12 HeadlinerArticle">
                    <h4 class="section-title"><span><%=U6013.FEATURED %></span></h4>
                    <asp:PlaceHolder ID="ArticleHeadlinerPlaceHolder" runat="server"></asp:PlaceHolder>
                </div>
            </div>
            <!-- begin row -->
            <div class="row row-space-30">
                <!-- begin col-9 -->
                <div class="col-md-6">
                    <h4 class="section-title"><span><%=U6002.NEWS %></span></h4>
                     
                    <!-- begin post-list -->
                    <ul class="post-list MainArticles">
                        <asp:PlaceHolder ID="ArticlesPlaceHolder" runat="server"></asp:PlaceHolder>
                    </ul>
                    <!-- end post-list -->
                </div>
                <!-- end col-9 -->
                <!-- begin col-3 -->
                <div class="col-md-4 bg-silver">
                    <!-- begin section-container -->
                    <div class="section-container">
                        <h4 class="section-title"><span class="bg-silver"><%=L1.CATEGORIES %></span></h4>
                        <ul class="sidebar-recent-post CategoryArticles">
                            <asp:PlaceHolder ID="ArticlesByCategoryPlaceHolder" runat="server"></asp:PlaceHolder>
                        </ul>
                    </div>
                    <!-- end section-container -->
                </div>
                <!-- end col-3 -->
                <!-- begin col-3 -->
                <div class="col-md-2">
                    <!-- begin section-container -->
                    <div class="section-container">
                        <h4 class="section-title"><span><%=U6013.WORLDNEWS %></span></h4>
                        <ul class="sidebar-recent-post WorldNewsArticles">
                            <asp:PlaceHolder ID="ArticlesWorldNewsPlaceHolder" runat="server"></asp:PlaceHolder>
                        </ul>
                    </div>
                    <!-- end section-container -->
                    <!-- begin section-container -->
                    <div class="section-container">
                        <h4 class="section-title"><span><%=U6013.FOLLOWUS %></span></h4>
                        <div class="sidebar-social-list">
                            <titan:SocialListFooter runat="server" />
                        </div>
                    </div>
                    <!-- end section-container -->
                </div>
                <!-- end col-3 -->
            </div>
            <!-- end row -->
        </div>
        <!-- end container -->
    </div>
    <!-- end #content -->


</asp:Content>

<asp:Content ID="FooterContent" runat="server" ContentPlaceHolderID="FooterContentPlaceHolder">
</asp:Content>
