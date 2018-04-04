<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="logs.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">

 <script>
     function pageLoad() {
        <%=PageScriptGenerator.GetGridViewCode(LogsGridView) %>
    }
</script>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <h1 class="page-header"><%= U4000.LOGS %></h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%= L1.HISTORYINFO %>.  <%=string.Format(U4000.LATESTHISTORYLOGS, "<b>" + AppSettings.DBArchiver.BalanceLogsKeptForDays + "</b>", MaxDisplayedLogs) %></p>
        </div>
    </div>
    

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="BalanceLogsUpdatePanel">
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="BalanceLogsUpdatePanel" runat="server">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="AdPacksCheckBox" EventName="CheckedChanged" />
            <asp:AsyncPostBackTrigger ControlID="OthersCheckBox" EventName="CheckedChanged" />
        </Triggers>
        <ContentTemplate>
            <asp:PlaceHolder ID="ShowBalanceLogsFilters" runat="server">
                <div class="row">
                    <div class="col-md-12">
                        <h3 runat="server" style="display: inline"><%=U5004.SHOW %>: </h3>
                        <asp:CheckBox runat="server" ID="AdPacksCheckBox" AutoPostBack="true" OnCheckedChanged="LogTypeCheckBox_CheckedChanged" Style="margin-right: 10px" />
                        <asp:CheckBox runat="server" ID="OthersCheckBox" AutoPostBack="true" OnCheckedChanged="LogTypeCheckBox_CheckedChanged" Style="margin-right: 10px" />
                    </div>
                </div>
            </asp:PlaceHolder>
            
            <div class="row">
                <div class="col-md-12"><asp:GridView ID="LogsGridView" runat="server" AllowSorting="False" AutoGenerateColumns="False" OnPreRender="BaseGridView_PreRender" DataSourceID="LogsSqlDataSource"
                        OnRowDataBound="LogsGridView_RowDataBound" PageSize="40" AllowPaging="true" OnPageIndexChanging="LogsGridView_PageIndexChanging" >
                        <Columns>
                            <asp:BoundField DataField='DateOccured' HeaderText='<%$ ResourceLookup : DATE %>' SortExpression='DateOccured' />
                            <asp:BoundField DataField='Note' HeaderText='<%$ ResourceLookup : ENTRY %>' SortExpression='Note' />
                            <asp:BoundField DataField='Amount' HeaderText='<%$ ResourceLookup : AMOUNT %>' SortExpression='Amount' />
                            <asp:BoundField DataField='Balance' HeaderText='<%$ ResourceLookup : FROM %>' SortExpression='Balance' />
                        </Columns>
                    </asp:GridView>
                    <asp:SqlDataSource ID="LogsSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
                        OnInit="LogsSqlDataSource_Init"></asp:SqlDataSource>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>


</asp:Content>
