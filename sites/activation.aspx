<%@ Page Language="C#" MasterPageFile="~/Sites.master" AutoEventWireup="true" CodeFile="activation.aspx.cs" Inherits="sites_activation" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <asp:PlaceHolder runat="server" ID="SuccessInfoLiteralPlaceHolder" Visible="false">
        <div class="alert note note-info">
            <h4>
                <asp:Literal ID="SuccessInfoLiteral" runat="server" />
            </h4>
        </div>
    </asp:PlaceHolder>

    <div class="content" data-scrollview="true">
        <div class="container" data-animation="true" data-animation-type="fadeInDown">
            <h2 class="content-title"><%=U6011.ACCOUNTACTIVATION %></h2>
            <h5 class="text-center"><%=String.Format(U6011.ACTIVATIONINFO, AppSettings.Site.Name) %> </h5>

            <div class="row">
                <div class="col-md-8 col-md-offset-2">
                    <h4 class="p-b-20 text-center"><%=L1.PRICE %>:
                   
                       

                        <asp:Label ID="PriceLiteral" runat="server"></asp:Label></h4>
                </div>
            </div>

            <%--MAIN PAY BUTTON--%>
            <div class="col-md-4 col-md-offset-4">
                <asp:PlaceHolder ID="UpgradeViaPaymentProcessorPlaceHolder" runat="server">
                    <asp:Button ID="UpgradeViaPaymentProcessor" runat="server" OnClick="upgradeViaPaymentProcessor_Click" CssClass="btn btn-inverse btn-block btn-lg" />
                </asp:PlaceHolder>
            </div>


            <%--LIST OF AVAILABLE PP--%>
            <asp:PlaceHolder ID="PaymentProcessorsButtonPlaceholder" runat="server">
                <div class="p-t-20">
                    <div class="col-md-12 text-center">
                        <asp:Literal ID="PaymentButtons" runat="server"></asp:Literal>
                    </div>

                    <div class="col-md-4 col-md-offset-4" style="padding-right: 18px; padding-left: 18px;">
                        <asp:PlaceHolder runat="server" ID="PayViaCashBalancePlaceHolder" Visible="false">
                            <br />
                            <asp:Button ID="PayViaCashBalanceButton" runat="server" OnClick="payViaCashBalanceButton_Click" CssClass="btn btn-inverse btn-block btn-lg" />
                            <asp:Literal runat="server" ID="RedirectToDepositLiteral"></asp:Literal>
                        </asp:PlaceHolder>
                        <asp:LinkButton runat="server" Style="margin-top: 20px" class="btn btn-inverse btn-block btn-lg" href="logout.aspx"><%=L1.LOGOUT%></asp:LinkButton>
                    </div>
                </div>
            </asp:PlaceHolder>

            <%--NO WAY TO PAY INFO--%>
            <asp:PlaceHolder ID="NoPaymentProcessorsPlaceHolder" runat="server" Visible="false">
                <p class="p-b-20 text-center"><%=U6011.NOACTIVEPAYMENTPROCESSORS %></p>
            </asp:PlaceHolder>

        </div>
    </div>

</asp:Content>
