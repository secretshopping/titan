<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="money.aspx.cs" Inherits="Money" %>

<%@ MasterType VirtualPath="~/User.master" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=U4000.MONEY%> <%=L1.STATISTICS%></h1>
    
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=U5001.STATSDESCRIPTION %></p>
        </div>
    </div>
 
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
                            <h3><%=U5001.TOTAL %>: <b><asp:Literal runat="server" ID="TotalLiteral"></asp:Literal></b></h3>
                            <titan:Statistics runat="server" StatType="User_AllCreditedMoney" Width="700px" Height="400px" StatTitle="<%$ResourceLookup: ALLCREDITEDMONEY %>"></titan:Statistics>
                        </div>
                    </div>
                </div>
            </asp:View>
            <asp:View runat="server" ID="View2">
                <div class="TitanViewElement">
                    <h3><%=U5001.TOTAL %>: <b><asp:Literal runat="server" ID="TotalRefLiteral"></asp:Literal></b></h3>
                    <titan:Statistics runat="server" StatType="Referrals_AllCreditedMoney" Width="700px" Height="400px" StatTitle="<%$ResourceLookup: ALLCREDITEDMONEY %>"></titan:Statistics>
                </div>
            </asp:View>
        </asp:MultiView>
    </div>

</asp:Content>