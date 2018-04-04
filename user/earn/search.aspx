<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="search.aspx.cs" Inherits="EarnSearch" %>

<%@ MasterType VirtualPath="~/User.master" %>

<%@ Import Namespace="Prem.PTC" %>
<%@ Import Namespace="Resources" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script type="text/javascript">

        function EndRequestHandlerForSearch() {
            var newPoints = $('#<%=UpdatedPointsTextBox.ClientID%>').text();
            if (newPoints != 'N') {

                var oldPoints = $('#MemberBalancesControlPointsLabel').text();

                if (oldPoints != newPoints) {
                    $("#MemberBalancesControlPointsLabel").fadeOut(2000);
                    setTimeout(UpdatePrice, 1900);
                    $("#MemberBalancesControlPointsLabel").fadeIn(2000);
                }

            }
        }

        function UpdatePrice() {
            var newPoints = $('#<%=UpdatedPointsTextBox.ClientID%>').text();
            $('#MemberBalancesControlPointsLabel').text(newPoints); //Update
        }

        function animatePrice() {
            $("#MemberBalancesControlPointsLabel").fadeOut(1);
            $("#MemberBalancesControlPointsLabel").fadeIn(2000);
        }


    </script>
    <link rel="stylesheet" href="Styles/SearchAndVideoClasses.css" type="text/css" />
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <h1 class="page-header"><%=U4000.SEARCH %></h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=U4000.SEARCHINFO.Replace("%p%", AppSettings.PointsName) %></p>
        </div>
    </div>

   <%--AJAX PART--%>
    <div class="row">
        <div class="col-md-12">
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Label ID="UpdatedPointsTextBox" runat="server" Text="N" Style="visibility: hidden"></asp:Label>
                </ContentTemplate>
            </asp:UpdatePanel>

            <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                <asp:Literal ID="ErrorMessage" runat="server" Text=""></asp:Literal>
            </asp:Panel>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                    <asp:Button ID="Button1" runat="server" OnClientClick="return false;" Text="Yahoo" CssClass="ViewSelected" />
                </asp:PlaceHolder>
            </div> 
        </div>
    </div>

    <div class="tab-content">
        <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
            <asp:View ID="View1" runat="server">
                <div class="row">
                    <div class="col-md-8">
                        <asp:Panel ID="Panel1" runat="server" DefaultButton="SearchButton">
                            <div class="input-group">
                                <asp:TextBox ID="SearchBox" runat="server" CssClass="form-control"></asp:TextBox>
                                <span class="input-group-btn">
                                    <asp:Button ID="SearchButton" runat="server" Text="<%$ ResourceLookup: SEARCH %>" CssClass="btn btn-inverse" OnClick="SearchButton_Click" />
                                </span>
                            </div>
                        </asp:Panel>

                        <asp:Literal ID="QueryResultsVideosLiteral" runat="server"></asp:Literal>

                        <script>
                            oli_var_<%=Prem.PTC.AppSettings.SearchAndVideo.YahooSearchAPI%> = {
                                "keywords": [
                                "<%=Query%>"
                                ],
                                "pagenumber" : <%=CurrentPage %>
                                }
                        </script>
                        <div id="adcreative"></div>
                        <script src="http://cdn.xdirectx.com/js/creative/<%=Prem.PTC.AppSettings.SearchAndVideo.YahooSearchAPI%>.js"></script>

                        <ul class="pagination pull-right">
                            <li class="paginate_button previous">
                                <asp:LinkButton ID="PreviousPageButton" runat="server" OnClick="PreviousPageButton_Click"><%=U4000.PREVIOUSPAGE %></asp:LinkButton>
                            </li>
                            <li class="paginate_button active">
                                <a href="#" aria-controls="data-table" tabindex="0">
                                    <asp:Literal ID="PageNumberLiteral" runat="server"></asp:Literal>
                                </a>
                            </li>
                            <li class="paginate_button next">
                                <asp:LinkButton ID="NextPageButton" runat="server" OnClick="NextPageButton_Click"><%=U4000.NEXTPAGE %></asp:LinkButton>
                            </li>
                        </ul>
                    </div>
                </div>
            </asp:View>
        </asp:MultiView>
    </div>
</asp:Content>
