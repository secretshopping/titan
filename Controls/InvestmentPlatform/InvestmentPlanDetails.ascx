<%@ Control Language="C#" AutoEventWireup="true" CodeFile="InvestmentPlanDetails.ascx.cs" Inherits="Controls_InvestmentPlatform_InvestmentPlanDetails" %>

<asp:Panel ID="InvPlanPlaceHolder" runat="server">
    <div class="col-md-6 col-lg-6">
        <div class="innerBorder">

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

            <div class="plan p-15 m-15">
                <div class="header" style="background: <%=Color%>">
                    <p class="text-center m-0" runat="server" id="HeaderPrice"><span class="price"><%=Price %></span></p>
                    <h3><%=Name %></h3>
                </div>

                <div runat="server" id="DescriptionTable">
                    <table class="table table-condensed table-borderless">
                        <tr runat="server" id="InvestmentTab">
                            <td><%=U6006.INVESTMENT %>:</td>
                            <td><%=Price %></td>
                        </tr>
                        <tr>
                            <td>ROI:</td>
                            <td><%=ROI %></td>
                        </tr>
                        <tr>
                            <td><%=L1.TIME %>:</td>
                            <td><%=RepurchaseTime %>&nbsp;<%=L1.DAYS %></td>
                        </tr>
                        <tr runat="server" id="CreditingEarningTab">
                            <td>
                                <asp:Label runat="server" ID="CreditingTextLabel" />
                                :</td>
                            <td><%=BinaryEarning %></td>
                        </tr>
                        <asp:PlaceHolder runat="server" ID="LimitsPlaceHolder">
                            <tr>
                                <td><%=U6006.DAILYLIMIT %>:</td>
                                <td><%=DailyLimit %></td>
                            </tr>
                            <tr>
                                <td><%=U6006.MONTHLYLIMIT %>:</td>
                                <td><%=MonthlyLimit %></td>
                            </tr>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder runat="server" ID="EarningsPlaceHolder">
                            <tr>
                                <td><%=U6010.DAILYPAY %>:</td>
                                <td><%=DailyEarning %></td>
                            </tr>
                            <tr>
                                <td><%=U6010.MONTHLYPAY %>:</td>
                                <td><%=MonthlyEarning %></td>
                            </tr>
                            <tr>
                                <td><%=U5001.TOTAL %>:</td>
                                <td><%=TotalEarning %></td>
                            </tr>
                        </asp:PlaceHolder>
                        <tr runat="server" id="EarningDelayTab">
                            <td><%=U6008.EARNINGDELAY %>:</td>
                            <td><%=EarningDelay %>&nbsp;<%=L1.DAYS %></td>
                        </tr>
                        <tr runat="server" id="BonusTab">
                            <td><%=U6010.BONUSFORTENURE %>:</td>
                            <td><%=PlatformPlan.EndBonus %></td>
                        </tr>
                    </table>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12 text-center">
                    <asp:Panel ID="ProgressBarContainer" runat="server" Visible="false">
                        <p>
                            <asp:Label ID="LabelProgressBarDescription" runat="server" />
                            <br />
                            <asp:Label ID="LabelProgressBarValues" runat="server" />                            
                            <asp:Label ID="LabelProgressBar" runat="server" />
                            <br />
                            <asp:Label ID="MoneyInSystemLabel" runat="server" />
                        </p>
                        <asp:Button ID="WithdrawMoneyFromSystem" runat="server" OnClick="WithdrawMoneyFromSystem_Click" OnClientClick="askForConfirmation(this)" CssClass="btn btn-primary preventClickBtn" />                        
                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>
</asp:Panel>
