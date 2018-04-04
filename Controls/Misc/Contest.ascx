<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Contest.ascx.cs" Inherits="Controls_Contest" %>

<div class="contest-item row">
    <div class="col-md-3 text-center">
        <img src="Images/Misc/<%=IsActive? "cupactive" : "cup" %>.png" />
        <p><%=ParticipateButtonHTML %></p>
         <h4><%=Contest.Name %></h4>

        <table class="table table-condensed table-borderless">
                            <tr>
                                <td><%=L1.FROM %></td>
                                <td><%=Contest.DateStart.ToShortDateString() %></td>
                            </tr>
                            <tr>
                                <td><%=L1.TO %></td>
                                <td><%=Contest.DateEnd.ToShortDateString()  %></td>
                            </tr>
                        </table>
    </div>
    <div class="col-md-2 col-md-offset-1">
        <table class="table table-condensed table-borderless">
                            <caption><h5><%=L1.REQUIREMENTS %></h5></caption>
                            <tr>
                                <td><%=L1.CLICKSSTRAIGHT %></td>
                                <td><%=Contest.ClicksRestriction %></td>
                            </tr>
                            <tr>
                                <td><%=L1.ACCOUNTDAYS %></td>
                                <td><%=Contest.RegisteredDaysRestriction %></td>
                            </tr>
                            <tr>
                                <td><%=L1.DIRECT %></td>
                                <td><%=Contest.DirectRefRestriction %></td>
                            </tr>
                            <tr>
                                <td><%=L1.RENTED %></td>
                                <td><%=Contest.RentedRefRestriction %></td>
                            </tr>
                            <tr>
                                <td>New Referrals <%=AppSettings.RevShare.AdPack.AdPackNamePlural %></td>
                                <td><%=Contest.AdPackReferrallsPurchaseRestriction %></td>
                            </tr>
                            <tr id="MinTransferPlaceHolder" runat="server">
                                <td><%=U6003.MINTRANSFER %></td>
                                <td><%=Contest.MinMembersDeposit %></td>
                            </tr>
                        </table>
    </div>
    <div class="col-md-2 col-md-offset-1">
         <% if (IsImageRewards) %>
                        <% { %>
                        <table class="table table-condensed table-borderless">
                            <caption><h5><%=L1.PRIZES %></h5></caption>
                            <tr>
                                <td class="text-center">
                                    <p>1.</p>
                                    <%=PrizesList[0] %></td>
                                <td class="text-center">
                                    <p>2.</p>
                                    <%=PrizesList[1] %></td>
                                <td class="text-center">
                                    <p>3.</p>
                                    <%=PrizesList[2] %></td>
                            </tr>
                        </table>
                        <% }
                        else
                        { %>
                        <table class="table table-condensed table-borderless">
                            <caption><h5><%=L1.PRIZES %></h5></caption>
                            <tr>
                                <td>1.</td>
                                <td><%=PrizesList[0] %></td>
                            </tr>
                            <tr>
                                <td>2.</td>
                                <td><%=PrizesList[1] %></td>
                            </tr>
                            <tr>
                                <td>3.</td>
                                <td><%=PrizesList[2] %></td>
                            </tr>
                        </table>
                        <% } %>
    </div>
    <div class="col-md-2 col-md-offset-1">
        <table class="table table-condensed table-borderless">
                    <caption><h5><%=U6012.TOPLIST %></h5></caption>
                    <tr>
                        <td>1.</td>
                        <td><%=TopList[0] %></td>
                    </tr>
                    <tr>
                        <td>2.</td>
                        <td><%=TopList[1] %></td>
                    </tr>
                    <tr>
                        <td>3.</td>
                        <td><%=TopList[2] %></td>
                    </tr>
                    <tr>
                        <td>4.</td>
                        <td><%=TopList[3] %></td>
                    </tr>
                    <tr>
                        <td>5.</td>
                        <td><%=TopList[4] %></td>
                    </tr>
                    <tr>
                        <td>6.</td>
                        <td><%=TopList[5] %></td>
                    </tr>
                    <tr>
                        <td>7.</td>
                        <td><%=TopList[6] %></td>
                    </tr>
                </table>
    </div>

</div>
<hr />
