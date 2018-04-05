<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LeadershipSytemRow.ascx.cs" Inherits="Controls_LeadershipSystemRow" %>

<%@ Import Namespace="Titan.Leadership" %>

<div class="leadership-system-row col-md-4 col-lg-3" data-current="<%=IsCurrentRank %>" data-accurred="<%=IsRankAcquired %>">
    <div class="panel panel-inverse">
        <div class="panel-heading">
            <h4 class="panel-title"><span class="label label-success m-r-10 pull-left"><%= Rank.Rank %>.</span> <%= Rank.RankName %></h4>
        </div>
        <div class="panel-body">
            <asp:PlaceHolder ID="CustomPrizePlaceHolder" runat="server" Visible="false">
                <div class="text-center">
                    <div class="prize-image-wrapper">

                        <asp:Image CssClass="img-responsive prize-image" ID="PrizeImage" runat="server" />

                    </div>
                    <p class="rank-description"><%= Rank.Note %></p>
                </div>
            </asp:PlaceHolder>
            <div class="rank-table-wrapper">
                <strong><%= U6005.REQUIREMENTS %></strong>
                <br />
                <asp:Literal ID="RanksRequirementsLiteral" runat="server" />
                <br />
                <strong><%= L1.PRIZE %></strong>
                <table class="rank-details">
                    <tr>
                        <td>
                            <%= LeadershipSystem.ReturnName(Rank.PrizeKind) %>

                            <asp:PlaceHolder ID="PrizePlaceHolder" runat="server" Visible="false">
                                <asp:Literal ID="PrizeLiteral" runat="server" />
                            </asp:PlaceHolder>
                        </td>
                    </tr>
                </table>

            </div>
            <div class="progress-wrapper">
                <asp:Literal ID="ProgressBarLiteral" runat="server" Visible="false" />
            </div>
        </div>
    </div>
</div>
