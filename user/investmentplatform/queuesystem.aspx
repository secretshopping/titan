<%@ Page Language="C#" AutoEventWireup="true" CodeFile="queuesystem.aspx.cs" Inherits="user_investmentplatform_queuesystem" MasterPageFile="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header">TMP TITLE</h1>
    <div class="row">
        <div class="col-md-12">
            <p id="MainDescriptionP" runat="server" class="lead" />
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <asp:Panel ID="SuccessPanel" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
                <asp:Literal ID="SuccessTextLiteral" runat="server"></asp:Literal>
            </asp:Panel>

            <asp:Panel ID="ErrorPanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                <asp:Literal ID="ErrorTextLiteral" runat="server"></asp:Literal>
            </asp:Panel>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <div class="TitanViewPage">
                    <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                        <asp:Button ID="Button1" runat="server" CommandArgument="1" />
                        <asp:Button ID="Button2" runat="server" CommandArgument="0" CssClass="ViewSelected" />
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>

    <div class="tab-content">
        <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">

            <asp:View runat="server" ID="BuyPlansView">
                <div class="TitanViewElement">
                    <div class="row">
                        <div class="col-md-12">
                            content
                        </div>
                    </div>
                </div>
            </asp:View>

            <asp:View runat="server" ID="ManagePlansView">
                <div class="TitanViewElement">
                    <div class="row">
                        <div class="col-md-12">
                            content2
                        </div>
                    </div>
                </div>
            </asp:View>

        </asp:MultiView>
    </div>

</asp:Content>
