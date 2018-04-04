<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="leadershiprewards.aspx.cs" Inherits="LeadershipRewards" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">


    <!--Slider-->
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1>Leadership</h1>
    Prove that you are a leader and get your reward!


    <asp:UpdatePanel ID="MultiViewUpdatePanel" runat="server" UpdateMode="Conditional">

        <ContentTemplate>
            <asp:Panel ID="SPanel" runat="server" Visible="false" CssClass="greenbox">
                <asp:Literal ID="SText" runat="server"></asp:Literal>
            </asp:Panel>

            <asp:Panel ID="EPanel" runat="server" Visible="false" CssClass="ui-state-error">
                <asp:Literal ID="EText" runat="server"></asp:Literal>
            </asp:Panel>

            <div class="TitanViewPage">
                <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                    <asp:Button ID="Button2" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                    <asp:Button ID="Button1" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                </asp:PlaceHolder>
            </div>
            <div class="clear"></div>
            <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
                <asp:View runat="server" ID="View1">
                    <div class="TitanViewElement" style="margin-top: -1px">
                        <%-- SUBPAGE START --%>
                        <br /><br />
                            <img src="../../Images/OneSite/Referrals/diamond.png" alt="diamond"  style="display:block; margin:auto;" />
                            
                        <asp:PlaceHolder runat="server" ID="CurrentLevelPlaceHolder">The last Leadership Reward you received is
                            <b><asp:Literal ID="CurrentLevelLiteral" runat="server"></asp:Literal></b>.
                            
                        <br />
                        </asp:PlaceHolder>
                            <br />
                             You are now working towards receiving
                       <b><asp:Literal ID="NextLevelLiteral" runat="server"></asp:Literal></b>
                        Reward. Check <i>Leadership Levels</i> for details.

                        <br /><br />
                        <h2>Statistics</h2>
                        <br />
                        <table class="styledTable" style="text-align:left">
                            <tr>
                                <td width="90%">Direct Referrals</td>
                                <td>
                                    <asp:Literal runat="server" ID="DirectReferralsLiteral"></asp:Literal></td>
                            </tr>
                            <tr>
                                <td>Indirect Referrals</td>
                                <td>
                                    <asp:Literal runat="server" ID="IndirectReferralsLiteral"></asp:Literal></td>
                            </tr>
                            <tr>
                                <td>Team Partners</td>
                                <td>
                                    <asp:Literal runat="server" ID="TeamPartnersLiteral"></asp:Literal></td>
                            </tr>
                        </table>
                        <br />
                        <h3>You Team's Fresh Funds</h3>
                        <br />
                        <table class="styledTable" style="text-align:left">
                            <tr>
                                <td width="90%"><b>Total</b> (<asp:Literal ID="TotalDateSpanLiteral" runat="server"></asp:Literal>)</td>
                                <td><asp:Literal ID="TotalFreshFundsLiteral" runat="server"></asp:Literal></td>
                            </tr>

                            <asp:Literal runat="server" ID="SubTotalFunds"></asp:Literal>
                        </table>
                    </div>
                </asp:View>
                <asp:View runat="server" ID="View2" OnActivate="View2_Activate">
                    <div class="TitanViewElement" style="margin-top: -1px">
                        <%-- SUBPAGE START --%>
                        <br />
                        <h2>Leadership rewards and requirements</h2>
                        <br />
                        <asp:GridView ID="LeadershipLevelsGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                            DataSourceID="LeadershipLevelsGridViewDataSource" OnRowDataBound="LeadershipLevelsGridView_RowDataBound" PageSize="20">
                            <Columns>
                                <asp:BoundField DataField='Id' HeaderText='Id' SortExpression='Id' ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" />
                                <asp:BoundField DataField='Name' SortExpression='Name' />
                                <asp:TemplateField SortExpression="Reward">
                                    <ItemTemplate>
                                        <%#new Money(Convert.ToDecimal(Eval("Reward"))).ToString() %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField='IndirectReferrals' SortExpression='IndirectReferrals' />
                                <asp:BoundField DataField='DirectReferrals' SortExpression='DirectReferrals' />
                                <asp:BoundField DataField='TeamPartners' HeaderText='TeamPartners' SortExpression='TeamPartners' />
                                <asp:BoundField DataField='TotalTeamDeposits' HeaderText='TotalTeamDeposits' SortExpression='TotalTeamDeposits' />
                                <asp:BoundField DataField='TeamDepositsPerSubTime' HeaderText='TeamDepositsPerSubTime' SortExpression='TeamDepositsPerSubTime' />
                                <asp:BoundField DataField='TotalTimeConstraintDays' HeaderText='TotalTimeConstraintDays' SortExpression='TotalTimeConstraintDays' HeaderStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" />
                                <asp:BoundField DataField='NumberOfSubTimeConstraints' HeaderText='NumberOfSubTimeConstraints' SortExpression='NumberOfSubTimeConstraints' HeaderStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" />
                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="LeadershipLevelsGridViewDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="LeadershipLevelsGridViewDataSource_Init"></asp:SqlDataSource>
                    </div>
                </asp:View>
            </asp:MultiView>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
