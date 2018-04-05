<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="earnings.aspx.cs" Inherits="About" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <script>
        function pageLoad() {
            <%=PageScriptGenerator.GetGridViewCode(HistoryGridView) %>
        }
    </script>

    <h1 class="page-header">Daily earnings of <%=Prem.PTC.AppSettings.Site.Name %></h1>
    
    <div class="row">
        <div class="col-md-12">
            <p class="lead">
                See all of our earnings from the last 14 days
            </p>
        </div>
    </div>

    <div class="tab-content">
        <div class="row">
            <div class="col-md-12">
                <asp:GridView ID="HistoryGridView" runat="server" AllowSorting="False" AutoGenerateColumns="True" AllowPaging="true" PageSize="30" OnPreRender="BaseGridView_PreRender">
                </asp:GridView>
            </div>
        </div>
    </div>
    


</asp:Content>
