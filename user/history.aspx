<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="history.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">

    <!--Slider-->
    <link rel="stylesheet" href="Plugins/Slider/bin/jquery.slider.min.css" type="text/css">
    <script type="text/javascript" src="Plugins/Slider/bin/jquery.slider.min.js"></script>

</asp:Content>





<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <h1><%= L1.HISTORY %></h1>
    <%= L1.HISTORYINFO %>.
    <br />
    <br />


    <%=string.Format(U4000.LATESTHISTORYLOGS, "<b>" + AppSettings.DBArchiver.HistoryLogsKeptForDays + "</b>", MaxDisplayedLogs) %>
    <br />
    <br />
    <asp:GridView ID="HistoryGridView" runat="server" AllowSorting="False" AutoGenerateColumns="False" DataSourceID="HistorySqlDataSource"
         PageSize="40" AllowPaging="true">
        <Columns>
            <asp:TemplateField HeaderText='<%$ ResourceLookup : DATE %>' SortExpression='Date'>
                <ItemTemplate>
                    <%# ((DateTime)Eval("Date")).ToShortDateString() %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText='<%$ ResourceLookup : ENTRY %>' SortExpression='Id'>
                <ItemTemplate>
                    <%# (new Prem.PTC.Members.History((int)Eval("Id"))).GetFullText()  %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:CheckBoxField DataField="IsRead" HeaderText="IsRead" SortExpression="IsRead" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
        </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="HistorySqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="HistorySqlDataSource_Init"></asp:SqlDataSource>

</asp:Content>
