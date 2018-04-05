<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="ads.aspx.cs" Inherits="PTCClicks" %>

<%@ MasterType VirtualPath="~/User.master" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <h1 class="page-header"><%=U6003.PTC %> <%=L1.STATISTICS%></h1>

    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=U6010.STATSDESCRIPTION3 %></p>
        </div>
    </div>

    <asp:Placeholder runat="server" ID="PTC">
        <div class="row">
            <div class="col-md-12">
                <div class="nav nav-tabs custom text-right">
                    <asp:Panel ID="TitanViewPagePanel" runat="server" CssClass="TitanViewPage">
                        <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                            <asp:Button ID="Button2" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                            <asp:Button ID="Button1" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                        </asp:PlaceHolder>
                    </asp:Panel>
                </div>
            </div>
        </div>

        <div class="tab-content">
            <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
                <asp:View runat="server" ID="View1">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="TitanViewElement">
                                <h3><%=U5001.TOTAL %>: <b>
                                    <asp:Literal runat="server" ID="TotalLiteral"></asp:Literal></b></h3>
                                <titan:Statistics runat="server" StatType="User_Clicks" Width="700px" Height="400px" StatTitle="<%$ResourceLookup: USERCLICKSMADE %>" IsInt="true"></titan:Statistics>
                            </div>
                            
                            <div class="TitanViewElement">
                                <h3><%=U5001.TOTAL %>: <b>
                                    <asp:Literal runat="server" ID="TotalLiteralCashLink"></asp:Literal></b></h3>
                                <titan:Statistics runat="server" StatType="User_CashLinksMoney" Width="700px" Height="400px" StatTitle="<%$ResourceLookup: ALLCREDITEDMONEY %>"></titan:Statistics>
                            </div>
                        </div>
                    </div>
                </asp:View>
                <asp:View runat="server" ID="View2">
                    <div class="TitanViewElement">
                        <h3><%=U5001.TOTAL %>: <b>
                            <asp:Literal runat="server" ID="TotalLiteral1"></asp:Literal></b></h3>
                        <titan:Statistics runat="server" StatType="DRClicks" Width="700px" Height="400px" ID="ReferralsStatistics" IsInt="true"></titan:Statistics>
                    </div>
                    <div class="TitanViewElement">
                        <h3><%=U5001.TOTAL %>: <b>
                            <asp:Literal runat="server" ID="TotalRefLiteral"></asp:Literal></b></h3>
                        <titan:Statistics runat="server" StatType="Referrals_CashLinksMoney" Width="700px" Height="400px" StatTitle="<%$ResourceLookup: ALLCREDITEDMONEY %>"></titan:Statistics>
                    </div>
                </asp:View>
            </asp:MultiView>
        </div>
    </asp:Placeholder>
</asp:Content>
