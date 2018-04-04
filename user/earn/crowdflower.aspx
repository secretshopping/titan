<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="crowdflower.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    
</asp:Content>


<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <input type="hidden" name="__EVENTARGUMENT5" id="__EVENTARGUMENT5" value="" />


    <h1 class="page-header">Crowdflower</h1>

    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <%--<h3><asp:Label ID="MenuTitleLabel" runat="server"></asp:Label></h3>--%>
                <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                    <asp:Button ID="Button2" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                    <asp:Button ID="Button1" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" Text="Crowdflower" />
                </asp:PlaceHolder>
            </div>
        </div>
    </div>

    <div class="tab-content">
        <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
            <asp:View runat="server" ID="asdasd">
                <div class="TitanViewElement">
                    <asp:PlaceHolder runat="server" ID="CrowdFlowerPlaceHolder" Visible="true">
                        <%=L1.OFFERSHORTINFO %>
                        <asp:Panel ID="cfOn" runat="server" Visible="false">
                            <iframe src="http://crowdflower.com/judgments/<%=Prem.PTC.AppSettings.Offerwalls.CrowdFlowerName %>?uid=<%=Prem.PTC.Members.Member.CurrentName %>" height="880" frameborder="0" width="900"></iframe>
                        </asp:Panel>
                        <asp:Panel ID="cfOff" runat="server" Visible="false">
                            <%=L1.FEATUREDISABLED %>
                        </asp:Panel>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="DemoCrowdFlowerPlaceHolder" Visible="false">
                        <div style="text-align: center; font-size: 15px;">
                            <asp:Image runat="server" ID="CrowdFlowerLogo"/><br />
                            <asp:Label runat="server" ID="DemoCrowdFlowerLabel" /><br />
                        </div>
                    </asp:PlaceHolder>


                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>
            <asp:View runat="server" ID="View1">
                <div class="TitanViewElement">
                    <div class="table-responsive">    
                        <asp:GridView ID="DirectRefsGridView" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                            DataSourceID="SqlDataSource1">
                            <Columns>
                                <asp:BoundField DataField="Date" HeaderText="<%$ ResourceLookup : DATE %>" SortExpression="Date" />
                                <asp:BoundField DataField="Title" HeaderText="<%$ ResourceLookup : ENTRY %>" SortExpression="Title" />
                                <asp:BoundField DataField="Points" HeaderText="Points" SortExpression="Points" />
                            </Columns>
                        </asp:GridView>

                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" SelectCommand="SELECT [Date], [Title], [Points] FROM [CrowdflowerTasks] WHERE ([Username] = @Referer) ORDER BY [Date]">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="UserName" Name="Referer" PropertyName="Text" Type="String" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                        <asp:Label ID="UserName" runat="server" Visible="false"></asp:Label>
                    </div>
                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>

        </asp:MultiView>
    </div>

</asp:Content>
