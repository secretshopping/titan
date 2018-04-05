<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="jackpot.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script type="text/javascript">
        $(document).ready(function () {
            $(".ticketList").hover(function () {
                if ($(this).hasClass("yourTickets")) {
                    $(this).removeClass("yourTickets");
                }
                else {
                    $(this).addClass("yourTickets");
                }
            });
        });
    </script>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=U5003.JACKPOTS %></h1>

    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=U5003.JACKPOTDESCRIPTION %></p>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                    <asp:Button ID="Button2" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                    <asp:Button ID="Button1" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                </asp:PlaceHolder>
            </div>
        </div>
    </div>

    <div class="tab-content">
        <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
            <asp:View runat="server" ID="View1">
                <div class="TitanViewElement">
                    <asp:PlaceHolder ID="JackpotPlaceHolder" runat="server"></asp:PlaceHolder>
                    <asp:Panel ID="NoDataPanel1" runat="server" CssClass="whitebox" Visible="false">
                        <asp:Literal ID="NoDataLiteral1" runat="server"></asp:Literal>
                    </asp:Panel>
                </div>
            </asp:View>
            <asp:View runat="server" ID="View2">
                <div class="TitanViewElement">
                    <asp:PlaceHolder ID="JackpotHistoryPlaceHolder" runat="server"></asp:PlaceHolder>
                    <asp:Panel ID="NoDataPanel2" runat="server" CssClass="whitebox" Visible="false">
                        <asp:Literal ID="NoDataLiteral2" runat="server"></asp:Literal>
                    </asp:Panel>
                </div>
            </asp:View>
        </asp:MultiView>
    </div>
</asp:Content>



