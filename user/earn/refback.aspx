<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="refback.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <input type="hidden" name="__EVENTARGUMENT5" id="__EVENTARGUMENT5" value="" />


    <h1 class="page-header">RefBack System</h1>
    <div class="row">
        <div class="col-md-12">
            <p><%=L1.REFBACK1 %>   <%=L1.REFBACK3 %></p>
            <p><%=L1.REFBACK2 %></p>
            <p><%=L1.REFBACK4.Replace("%username1%","<b>" + Prem.PTC.Members.Member.CurrentName + "</b>").Replace("%username2%","<b>" + Prem.PTC.Members.Member.CurrentName + "refback</b>") %>
            </p>
            <p class="text-danger"><%=L1.REFBACK5 %></p>
        </div>
    </div>

    <div class="tab-content">
        <div class="row">
            <div class="col-md-12">
                <div class="table-responsive">
                    <asp:GridView ID="DirectRefsGridView" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                        DataSourceID="SqlDataSource1" DataKeyNames="Id" OnRowDataBound="DirectRefsGridView_RowDataBound" OnRowCommand="DirectRefsGridView_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" InsertVisible="False" ReadOnly="True" />
                            <asp:BoundField DataField="SiteName" HeaderText="<%$ ResourceLookup : ENTRY %>" SortExpression="SiteName" />
                            <asp:BoundField DataField="ReferralLink" HeaderText="<%$ ResourceLookup : REFLINK %>" SortExpression="ReferralLink" />
                            <asp:TemplateField HeaderText="<%$ ResourceLookup : SEND %>">
                                <ItemStyle />
                                <ItemTemplate>
                                    <asp:Button ID="ImageButton1" runat="server"
                                        CommandName="comSend" CssClass="rbuttongrey"
                                        CommandArgument='<%# Container.DataItemIndex %>'
                                        Text="<%$ ResourceLookup : REFDECLARATION %>" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
                <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" SelectCommand="SELECT [Id], [SiteName], [ReferralLink] FROM [RefbackSites] WHERE ([IsActive] = @IsActive)">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="true" Name="IsActive" Type="Boolean" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:Label ID="UserName" runat="server" Visible="false"></asp:Label>

            </div>
        </div>
    </div>

</asp:Content>
