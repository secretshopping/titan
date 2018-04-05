<%@ Page Language="C#" AutoEventWireup="true" CodeFile="searchnews.aspx.cs" Inherits="sites_searchnews" MasterPageFile="~/Sites.master" %>

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
    <script type="text/javascript">
        function setSearchTextBox(value)
        {
            $('#SearchTextBox').val(value);
        }
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


    <div id="content" class="content">
        <!-- begin container -->
        <div class="container">
            <div class="row row-space-30">
                <!-- begin col-9 -->
                <div class="col-md-12">                     
                    <!-- begin post-list -->
                    <h2>Search results for "":</h2>
                    <ul class="post-list search-results">
                        <asp:PlaceHolder ID="ArticlesPlaceHolder" runat="server"></asp:PlaceHolder>
                    </ul>
                    <!-- end post-list -->
                </div>
                <!-- end col-9 -->

            </div>
            <!-- end row -->
        </div>
        <!-- end container -->
    </div>
    <!-- end #content -->

</asp:Content>
