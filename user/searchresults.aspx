<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true" CodeFile="searchresults.aspx.cs" Inherits="searchresults" %>

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

    <h1 class="page-header"><%=U6000.USERS %></h1>

    <asp:PlaceHolder runat="server" ID="NoResultsPlaceHolder" Visible="false">    
        <div class="row">
            <div class="col-md-12">
                <p class="alert alert-danger">
                    <%=U6003.NORESULTS %>
                </p>
            </div>
        </div>
    </asp:PlaceHolder>

    <div class="">
        <div class="row">
            <div class="col-md-12">
                <ul class="user-search-results">
                    <asp:PlaceHolder runat="server" ID="FriendInfoPlaceHolder"></asp:PlaceHolder>
                </ul>
            </div>
        </div>
    </div>
    

</asp:Content>
