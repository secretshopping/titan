<%@ Page Language="C#" AutoEventWireup="true" CodeFile="tickets.aspx.cs" Inherits="sites_tickets" MasterPageFile="~/Sites.master" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <div class="content" data-scrollview="true">
        <div class="container" data-animation="true" data-animation-type="fadeInDown">
            <h2 class="content-title"><%=U3501.VIEWTICKETS %></h2>
            <div class="row">
                <div class="col-md-12">
                    <p class="text-center"><%=U3501.VIEWTICKETSINFO %></p>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <asp:Panel ID="Panel1" runat="server" CssClass="TitanViewPage">
                        <asp:PlaceHolder ID="PlaceHolder1" runat="server">
                            <div class="row">
                                <div class="col-md-2">
                                    <asp:Button ID="Button4" runat="server" OnClick="MenuButton_Click" CommandArgument="1" CssClass="btn btn-inverse btn-block m-t-15 ViewSelected" />
                                </div>
                                <div class="col-md-2">
                                    <asp:Button ID="Button3" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="btn btn-inverse btn-block m-t-15" />
                                </div>
                            </div>
                        </asp:PlaceHolder>
                    </asp:Panel>

                    <div class="TitanViewElement">
                        <div class="row m-t-30">
                            <div class="col-md-12">
                                <asp:GridView ID="MessageGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataKeyNames='<%# new string[] { "SupportTicketId", } %>'
                                    DataSourceID="TicketSqlDataSource1" OnRowDataBound="MessageGridView_RowDataBound" OnRowCommand="MessageGridView_RowCommand"
                                    OnDataBound="MessageGridView_DataBound" PageSize="25">
                                    <Columns>

                                        <asp:BoundField DataField="SupportTicketId" HeaderText="Id" ReadOnly="True" SortExpression="SupportTicketId" Visible="true" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                        <asp:BoundField DataField="Date" HeaderText='<%$ ResourceLookup : DATE %>' SortExpression="Date" DataFormatString="{0:d}">
                                            <ItemStyle />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="TicketDepartmentId" HeaderText='Department' SortExpression="TicketDepartmentId">
                                            <ItemStyle />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Subject" HeaderText='Subject' SortExpression="Subject">
                                            <ItemStyle />
                                        </asp:BoundField>

                                        <asp:TemplateField HeaderStyle-Width="16" ItemStyle-Width="16" HeaderText='<%$ ResourceLookup : READ %>'>
                                            <ItemTemplate>
                                                <asp:LinkButton ToolTip='<%$ ResourceLookup : READ %>'
                                                    CommandName='Read' CommandArgument='<%# Container.DataItemIndex %>'
                                                    runat="server" >
                                                    <span class="fa fa-envelope fa-lg"></span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:CheckBoxField HeaderStyle-Width="16px" DataField="IsSolved" HeaderText="Solved" SortExpression="IsSolved" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                    
                    <asp:SqlDataSource ID="TicketSqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" SelectCommand="SELECT * FROM [SupportTickets] WHERE ([FromUsername] = @ToUsername) ORDER BY [Date] DESC">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="TicketUserName" Name="ToUsername" PropertyName="Text" Type="String" />
                        </SelectParameters>
                    </asp:SqlDataSource>

                    <asp:Label ID="TicketUserName" runat="server" Visible="false"></asp:Label>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
