<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="contests.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <input type="hidden" name="__EVENTARGUMENT5" id="__EVENTARGUMENT5" value="" />


    <h1 class="page-header"><%=Resources.L1.CONTESTS %></h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead">
                <%=Resources.L1.CONTESTINFO %>
            </p>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <%--<h3><asp:Label ID="MenuTitleLabel" runat="server"></asp:Label></h3>--%>
                <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                    <asp:Button ID="Button8" runat="server" OnClick="MenuButton_Click" CommandArgument="7" />
                    <asp:Button ID="Button7" runat="server" OnClick="MenuButton_Click" CommandArgument="6" Text="<%$ ResourceLookup : FORUMCONTEST %>" />
                    <asp:Button ID="Button3" runat="server" OnClick="MenuButton_Click" CommandArgument="5" Text="<%$ ResourceLookup : CONTEST6 %>" />
                    <asp:Button ID="Button4" runat="server" OnClick="MenuButton_Click" CommandArgument="4" Text="<%$ ResourceLookup : CONTEST5 %>" />
                    <asp:Button ID="Button5" runat="server" OnClick="MenuButton_Click" CommandArgument="3" Text="<%$ ResourceLookup : CONTEST4 %>" />
                    <asp:Button ID="Button6" runat="server" OnClick="MenuButton_Click" CommandArgument="2" Text="<%$ ResourceLookup : CONTEST3 %>" />
                    <asp:Button ID="Button2" runat="server" OnClick="MenuButton_Click" CommandArgument="1" Text="<%$ ResourceLookup : CONTEST2 %>" />
                    <asp:Button ID="Button1" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" Text="<%$ ResourceLookup : CONTEST1 %>" />
                </asp:PlaceHolder>
            </div>
        </div>
    </div>

    <div class="tab-content">
        <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
            <asp:View runat="server" ID="VDR">
                <div class="TitanViewElement">
                    
                    <asp:PlaceHolder ID="Contests_DR" runat="server"></asp:PlaceHolder>
                    <asp:Literal ID="Contests_Info" runat="server"></asp:Literal>
                    
                    <h5><asp:Literal ID="LatestLiteralDR" runat="server"></asp:Literal></h5>
                    <ul>
                        <asp:Literal ID="Latest_DR" runat="server"></asp:Literal>
                    </ul>
                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>
            <asp:View runat="server" ID="VRR">
                <div class="TitanViewElement">

                    <asp:PlaceHolder ID="Contests_RR" runat="server"></asp:PlaceHolder>

                    <h5><%=L1.LATESTKINDWINNERS %>:</h5>
                    <ul>
                        <asp:Literal ID="Latest_RR" runat="server"></asp:Literal>
                    </ul>
                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>
            <asp:View runat="server" ID="VC">
                <div class="TitanViewElement">
                    <h5><asp:Label runat="server" ID="OfferWallsLabel" Visible="false"></asp:Label></h5>
                    <asp:PlaceHolder ID="Contests_Click" runat="server"></asp:PlaceHolder> 

                    <h5><%=L1.LATESTKINDWINNERS %>:</h5>
                    <ul>
                        <asp:Literal ID="Latest_Click" runat="server"></asp:Literal>
                    </ul>
                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>
            <asp:View runat="server" ID="VT">
                <div class="TitanViewElement">

                    <asp:PlaceHolder ID="Contests_Transfer" runat="server"></asp:PlaceHolder>
                    
                    <h5><%=L1.LATESTKINDWINNERS %>:</h5>
                    <ul>
                        <asp:Literal ID="Latest_Transfer" runat="server"></asp:Literal>
                    </ul>
                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>
            <asp:View runat="server" ID="VO">
                <div class="TitanViewElement">
                    <asp:PlaceHolder ID="Contests_Offerwalls" runat="server"></asp:PlaceHolder>
                    
                    <h5><%=L1.LATESTKINDWINNERS %>:</h5>
                    <ul>
                        <asp:Literal ID="Latest_Offerwalls" runat="server"></asp:Literal>
                    </ul>
                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>
            <asp:View runat="server" ID="VCL">
                <div class="TitanViewElement">
                    <asp:PlaceHolder ID="Contests_Crowdflower" runat="server"></asp:PlaceHolder>
                    <h5><%=L1.LATESTKINDWINNERS %>:</h5>
                    <ul>
                        <asp:Literal ID="Latest_Crowdflower" runat="server"></asp:Literal>
                    </ul>
                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>
            <asp:View runat="server" ID="FC">
                <div class="TitanViewElement">
                    <asp:PlaceHolder ID="Contests_Forum" runat="server"></asp:PlaceHolder>
                    <h5><%=L1.LATESTKINDWINNERS %>:</h5>
                    <ul>
                        <asp:Literal ID="Latest_Forum" runat="server"></asp:Literal>
                    </ul>
                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>
            <asp:View runat="server" ID="OC">
                <div class="TitanViewElement">
                    <asp:PlaceHolder ID="Contests_Offer" runat="server"></asp:PlaceHolder>
                    <h5><%=L1.LATESTKINDWINNERS %>:</h5>
                    <ul>
                        <asp:Literal ID="Latest_Offer" runat="server"></asp:Literal>
                    </ul>
                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>
        </asp:MultiView>
    </div>
</asp:Content>
