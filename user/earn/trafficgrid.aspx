<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="trafficgrid.aspx.cs" Inherits="About" EnableViewStateMac="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">

    <link rel="stylesheet" href="Styles/WatchAds.css" type="text/css" />

    <script type="text/javascript">
        function doSubmit() {
            $('#__EVENTARGUMENT5').val('YES');
            $('#<%=Form.ClientID%>').attr('action', 'user/earn/surf.aspx?f=0');
            $('#<%=Form.ClientID%>').attr('target', '_blank');
            $('#<%=Form.ClientID%>').submit();

            $('#<%=Form.ClientID%>').attr('action', 'user/earn/trafficgrid.aspx');
            $('#<%=Form.ClientID%>').attr('target', '_self');
        }
        function resetFormAction() {
            $('#<%=Form.ClientID%>').attr('action', 'user/earn/trafficgrid.aspx');
            $('#<%=Form.ClientID%>').attr('target', '_self');
        }
    </script>

    <style type="text/css">
        .traffic-grid {
            background: url(<%=ImageUrl%>);
        }
    </style>

    <script>
        function pageLoad() {
            <%=PageScriptGenerator.GetGridViewCode(GridView1) %>
            <%=PageScriptGenerator.GetGridViewCode(GridView2) %>
        }
    </script>

</asp:Content>





<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <input type="hidden" name="__EVENTARGUMENT5" id="__EVENTARGUMENT5" value="" />


    <h1 class="page-header">Traffic Grid</h1>
    
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=L1.TGINFO %></p>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">

            <div class="TitanViewPage">
                <%--<h3><asp:Label ID="MenuTitleLabel" runat="server"></asp:Label></h3>--%>
                <div class="nav nav-tabs custom text-right">
                    <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                        <asp:Button ID="Button3" runat="server" OnClick="MenuButton_Click" OnClientClick="resetFormAction();" CommandArgument="2" />
                        <asp:Button ID="Button2" runat="server" OnClick="MenuButton_Click" OnClientClick="resetFormAction();" CommandArgument="1" />
                        <asp:Button ID="Button1" runat="server" OnClick="MenuButton_Click" OnClientClick="resetFormAction();" CommandArgument="0" CssClass="ViewSelected" Text="TrafficGrid" />
                    </asp:PlaceHolder>
                </div>
            </div>

        </div>
    </div>

    <div class="tab-content">
        <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
            <asp:View runat="server" ID="asdasd">
                <div class="TitanViewElement">
                    <div class="row">
                        <div class="col-md-12">
                    
                            <%-- SUBPAGE START --%>
                    
                            <asp:UpdatePanel runat="server" ID="TrafficRefreshUpdatePanel" OnLoad="TrafficRefreshUpdatePanel_Load" ClientIDMode="Static">
                                <ContentTemplate>
                                    <asp:Panel ID="GridOn" runat="server">
                                        <div class="row">
                                            <div class="col-md-6">
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <h3><%=L1.YOURACCOUNT %></h3>
                                                            <%=L1.TGHITS %>: <b>
                                                                <asp:Literal ID="L_Clicked" runat="server"></asp:Literal></b>/<asp:Literal ID="L_Max" runat="server"></asp:Literal>
                                                            <br />
                                                            <%=L1.YOURCHANCES %>: <b>
                                                                <asp:Literal ID="L_Chances" runat="server"></asp:Literal>%</b>
                                                            <br />
                                                            <%=L1.TGSHORTED %>: <b>
                                                                <asp:Literal ID="L_Duration" runat="server"></asp:Literal>%</b>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                            <div class="col-md-6">
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <h3><%=L1.PRIZES %></h3>
                                                            <span id="mainBalance" runat="server"><%=L1.MAINBALANCE %>: <b>$0.01-<%=Prem.PTC.AppSettings.TrafficGrid.Limit.ToString()%></b></span>
                                                            <span id="trafficBalance" runat="server"><%=U4200.TRAFFICBALANCE %>: <b>$0.01-<%=Prem.PTC.AppSettings.TrafficGrid.Limit.ToString()%></b></span>
                                                            <span id="adBalance" runat="server"><%=U6012.PURCHASEBALANCE %>: <b>$0.01-<%=Prem.PTC.AppSettings.TrafficGrid.Limit.ToString()%></b></span>                                    
                                                            <span id="points" runat="server"><%=AppSettings.PointsName %>: <b><%=TrafficGridManager.GetMinPointsReward().ToString() %>-<%=TrafficGridManager.GetMaxPointsReward().ToString() %></b></span>
                                                            <span id="drLimit" runat="server"><%=L1.DRRLIMITPLUS %>: <b>+1</b>
                                                            </span>
                                                            <span id="rentedReferrals" runat="server"><%=L1.RENTED %> <%=L1.REFERRALS %>: <b>+1</b></span>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </div>

                                            <asp:Literal ID="TheGrid" runat="server"></asp:Literal>


                                    </asp:Panel>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:Panel ID="GridOff" runat="server" Visible="false">
                                <div><%=U5006.ATLEASTONETRAFFICGRID %></div>
                            </asp:Panel>

                            <%-- SUBPAGE END   --%>

                        </div>
                    </div>
                </div>
            </asp:View>

            <asp:View runat="server" ID="View1">
                <div class="TitanViewElement">
                    <div class="row">
                        <div class="col-md-12">

                            <%-- SUBPAGE START --%>
                                <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                                    DataSourceID="SqlDataSource2" OnPreRender="BaseGridView_PreRender" OnRowDataBound="GridView1_RowDataBound" >
                                    <Columns>

                                        <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                        <asp:BoundField DataField="What" HeaderText="<%$ ResourceLookup : PRIZE %>" SortExpression="What" />
                                        <asp:BoundField DataField="Username" HeaderText="<%$ ResourceLookup : USERNAME %>" SortExpression="Username" />
                                        <asp:BoundField DataField="Date" HeaderText="<%$ ResourceLookup : DATE %>" SortExpression="Date" DataFormatString="{0:d}" />
                                        <asp:BoundField DataField="Date" HeaderText=" " SortExpression="Date"  />
                                    </Columns>
                                </asp:GridView>
                                <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" SelectCommand="SELECT TOP 10 * FROM [TrafficGridLatestWinners] ORDER BY [Date] DESC"></asp:SqlDataSource>
                        
                            <%-- SUBPAGE END   --%>    

                        </div>
                    </div>
                </div>
            </asp:View>


            <asp:View runat="server" ID="View2">
                <div class="TitanViewElement">
                    <div class="row">
                        <div class="col-md-12">

                            <%-- SUBPAGE START --%>
                
                                <asp:GridView ID="GridView2" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                                    DataSourceID="SqlDataSource3" OnPreRender="BaseGridView_PreRender" OnRowDataBound="GridView1_RowDataBound">
                                    <Columns>

                                        <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />


                                        <asp:BoundField DataField="What" HeaderText="<%$ ResourceLookup : PRIZE %>" SortExpression="What" />
                                        <asp:BoundField DataField="Username" HeaderText="<%$ ResourceLookup : USERNAME %>" SortExpression="Username" />
                                        <asp:BoundField DataField="Date" HeaderText="<%$ ResourceLookup : DATE %>" SortExpression="Date" DataFormatString="{0:d}" />
                                        <asp:BoundField DataField="Date" HeaderText="" SortExpression="Date" />
                                    </Columns>
                                </asp:GridView>
                                <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" SelectCommand="SELECT TOP 10 * FROM [TrafficGridTopWinners] ORDER BY [Value] DESC"></asp:SqlDataSource>
                 
                            <%-- SUBPAGE END   --%>

                        </div>
                    </div>
                </div>
            </asp:View>
        </asp:MultiView>
    </div>

</asp:Content>
