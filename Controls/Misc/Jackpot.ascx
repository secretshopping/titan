<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Jackpot.ascx.cs" Inherits="Controls_Jackpot" %>



<div class="jackpot-item row">
    <div class="col-md-12">
        <asp:Panel ID="SPanel" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
            <asp:Literal ID="SText" runat="server"></asp:Literal>
        </asp:Panel>
        <asp:Panel ID="EPanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
            <asp:Literal ID="EText" runat="server"></asp:Literal>
        </asp:Panel>
    </div>
    <div class="general">
        <div class="col-md-3 text-center">
            <h5><%=Jackpot.Name %></h5>
            <img src="Images/Misc/jackpot.png" id="toggleDetails" />
            <h5><%=L1.PRIZE %></h5>
            <div runat="server" id="mainBalancePrize">
                <%=L1.MAINBALANCE%>

                <%= Jackpot.MainBalancePrize.ToString() %>
            </div>
            <div style="padding-bottom: 10px" runat="server" id="adBalancePrize">
                <%=U6012.PURCHASEBALANCE%>

                <%= Jackpot.AdBalancePrize.ToString() %>
            </div>
            <div style="padding-bottom: 10px" runat="server" id="loginAdsCreditsPrize">
                <%=U5008.LOGINADSCREDITS%>

                <%= Jackpot.LoginAdsCreditsPrize.ToString() %>
            </div>
            <div style="padding-bottom: 10px" runat="server" id="upgradePrize">
                <%=L1.UPGRADE%>

                <asp:Literal runat="server" ID="UpgradeDetails"></asp:Literal>
            </div>
        </div>
        <div class="col-md-2">
            <h5><%=L1.DURATION  %></h5>

            <p>
                <%=Jackpot.StartDate.ToString() %><br />
                <%=L1.TO %><br />
                <%=Jackpot.EndDate.ToString() %>
            </p>
        </div>
        <div class="col-md-4">
            <table class="table table-condensed table-striped m-t-20">
                <tr>
                    <td><%=U5003.SINGLETICKET %></td>
                    <td><%=Jackpot.TicketPrice %></td>
                </tr>
                <tr>
                    <td><%=U5003.PARTICIPANTS %></td>
                    <td>
                        <asp:Literal runat="server" ID="ParticipantsLiteral"></asp:Literal></td>
                </tr>
                <tr>
                    <td><%=U5003.ALLTICKETS %></td>
                    <td>
                        <asp:Literal runat="server" ID="TicketsLiteral"></asp:Literal></td>
                </tr>
                <asp:PlaceHolder runat="server" ID="UsersTicketsPlaceholder">
                    <tr>
                        <td><%=U5003.YOURTICKETS %> (<asp:Literal runat="server" ID="NumberOfTicketsLiteral"></asp:Literal>)</td>
                        <td>
                            <asp:Literal runat="server" ID="UsersTicketsLiteral"></asp:Literal></td>
                    </tr>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="HistoryPlaceholder">
                    <tr>
                        <td><%=U6006.WINNERS %></td>
                        <td>
                            <asp:Literal runat="server" ID="WinnerLiteral"></asp:Literal></td>
                    </tr>
                    <tr>
                        <td><%=U6006.WINNINGTICKETS %></td>
                        <td>
                            <asp:Literal runat="server" ID="WinningTicketLiteral"></asp:Literal></td>
                    </tr>
                </asp:PlaceHolder>
            </table>
        </div>
        <asp:PlaceHolder ID="BuyTicketsPlaceholder" runat="server">
            <div class="col-md-3 m-b-20">
                <h5><%=U5003.BUYTICKETS %></h5>
                <div class="input-group">
                    <p>
                        <asp:TextBox runat="server" ID="NumberOfTicketsTextBox" CssClass="form-control m-b-5" ClientIDMode="Static"
                            type="number" min="1" step="1" value="1"
                            autocomplete="off" />
                    </p>
                    <asp:PlaceHolder ID="BuyTicketsFromAdBalancePlaceHolder" runat="server" Visible="false">
                        <asp:Button runat="server" ID="BuyTicketsFromAdBalanceButton" CssClass="btn btn-inverse btn-block m-b-5" OnClick="BuyTicketsViaAdBalance_Click" 
                            OnClientClick="this.disabled = true; this.className = 'rbutton-loading'; this.value='';"
                            UseSubmitBehavior="false" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="BuyTicketsFromCashBalancePlaceHolder" runat="server" Visible="false">
                        <asp:Button runat="server" ID="BuyTicketsFromCashBalanceButton" CssClass="btn btn-inverse btn-block m-b-5" OnClick="BuyTicketsViaCashBalance_Click" 
                            OnClientClick="this.disabled = true; this.className = 'rbutton-loading'; this.value='';"
                            UseSubmitBehavior="false" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="BuyTicketsViaPaymentProcessorPlaceHolder" runat="server">
                        <asp:Button ID="BuyTicketsViaPaymentProcessor" runat="server" CssClass="btn btn-inverse btn-block m-b-5"
                            OnClick="BuyTicketsViaPaymentProcessor_Click" />
                    </asp:PlaceHolder>
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="PaymentProcessorsButtonPlaceholder" runat="server">
            <div class="col-md-3 m-b-20">
                <div class="form-group">
                    <div class="payment-processors">
                        <asp:Literal ID="PaymentButtons" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>
    </div>
</div>






