<%@ Page Language="C#" AutoEventWireup="true" CodeFile="article.aspx.cs" Inherits="sites_article" MasterPageFile="~/Sites.master" %>

<asp:Content runat="server" ContentPlaceHolderID="ContentHeadContent">
    <link href="Scripts/news/assets/css/style.min.css" rel="stylesheet" />
    <link href="Scripts/news/assets/css/style-responsive.min.css" rel="stylesheet" />
    <link href="Scripts/news/assets/css/default.css" rel="stylesheet" />
    <link href="Scripts/news/assets/css/custom.css" rel="stylesheet" />
    <script src="Scripts/news/assets/js/apps.min.js"> </script>
    <script>
        $(function () {
            $("#page-container").removeClass("fade");
        });
    </script>
    <style>
        
    </style>

    <script data-cfasync="false" src="Plugins/NewsTimer/Script.js?x=xp"></script>

    <script data-cfasync="false">

        var loaded = false;
        var invalid = false;

        jQuery(function ($) {
            loaded = true;
            startCounter(<%=CreditReadAfterSeconds%>);
        });

    </script>

    <meta property="og:type" content="article" />
    <meta property="og:title" <%="content=\"" + InputChecker.HtmlPartialDecode(Article.Title) + " - " + InputChecker.HtmlPartialDecode(AppSettings.Site.Name) + "\"" %> />
    <meta property="og:description" <%="content=\"" + InputChecker.HtmlPartialDecode(Article.ShortDescription) + "\""%> />
    <meta property="og:image" <%="content=\"" +  Article.GetCoverImageURLAbsolutePath() + "\""%> />

</asp:Content>


<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <asp:ScriptManager runat="server"></asp:ScriptManager>

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



    <div id="content" class="content">
        <!-- begin container -->
        <div class="container">
            <div class="row row-space-30">
                <div class="col-md-9">
                    <div class="single-post">
                        <h1 class="post-title">
                            <%=Article.Title %>
                        </h1>
                        <div class="post-by">
                            Posted by 
                        <a href="#"><%=Author.FirstName %> <%=Author.SecondName %> (<%=Author.Name %>)</a> <span class="divider">|</span> <%=Article.CreatedDate.ToShortDateString() %> <span class="divider">|</span> <a href="#"><%=Article.GetCategory().Text %></a>
                        </div>
                        <!-- begin post-image -->
                        <div class="post-image">
                            <img src="<%=Article.GetCovertImageURL() %>" alt="" />
                        </div>
                        <!-- end post-image -->
                        <div class="post-desc">
                            <p>
                                <strong>
                                    <%=Article.ShortDescription %>
                                </strong>
                            </p>
                        </div>
                        <!-- begin post-desc -->
                        <div class="post-desc">
                            <asp:Literal ID="TextLiteral" runat="server"></asp:Literal>
                        </div>
                    </div>
                </div>

                <div class="col-md-3">
                    <!-- begin section-container -->
                    <div class="section-container">
                        <h4 class="section-title"><span><%=U6012.WHATSTRENDING %></span></h4>
                        <ul class="sidebar-list TrendingArticles">
                            <asp:PlaceHolder ID="TreningArticlesPlaceHolder" runat="server"></asp:PlaceHolder>
                        </ul>
                    </div>
                    <!-- end section-container -->
                    <!-- begin section-container -->
                    <div class="section-container">
                        <h4 class="section-title"><span><%=U6013.SUGGESTEDFORYOU %></span></h4>
                        <ul class="sidebar-recent-post SuggestedArticles">
                            <asp:PlaceHolder ID="SuggestedArticlesPlaceHolder" runat="server"></asp:PlaceHolder>

                        </ul>
                    </div>
                    <!-- end section-container -->
                    <!-- begin section-container -->
                    <div class="section-container">
                        <h4 class="section-title"><span>Follow Us</span></h4>
                        <div class="sidebar-social-list">
                            <titan:SocialListFooter runat="server" />
                        </div>
                        
                    </div>
                    <!-- end section-container -->
                </div>
            </div>
        </div>
    </div>

    <asp:UpdatePanel runat="server">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="CreditPostback" EventName="Click" />
        </Triggers>
        <ContentTemplate>
            <asp:Button ID="CreditPostback" ClientIDMode="Static" runat="server" OnClick="CreditPostback_Click" CssClass="displaynone" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
