<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ICOStage.ascx.cs" Inherits="Controls_ICO_ICOStage" %>

<asp:PlaceHolder ID="StagePlaceHolder" runat="server">
    <div class="ICOStage" runat="server" ID="ICOStageItem">
        <div class="row">
            <div class="col-md-12" style="text-align: center">
                <h5>
                    <asp:Literal runat="server" ID="ICOStageName" />
                    <br />
                    <span class="countdown" runat="server" id="ICOStageTimestamp">
                        <span>
                            <asp:Literal runat="server" ID="ICOStageEventLiteral" />
                        </span>
                        <br />
                    </span>
                </h5>
            </div>

            <div class="col-md-3" style="text-align: center">
                <span class="stagePriceLabel"><%=L1.PRICE %></span>
                <br />
                <span class="stagePriceValue">
                    <asp:Literal runat="server" ID="ICOStagePriceLiteral" /></span>
            </div>

            <div class="col-md-6" style="text-align: center">
                <asp:Literal runat="server" ID="ICOStageAvailableTokensPercentLiteral" />
            </div>

            <div class="col-md-3" style="text-align: center">
                <span style="display: inline-block;">
                    <span class="availableTokensValue">
                        <asp:Literal runat="server" ID="ICOStageAvailableTokensLiteral" /></span>
                    <asp:Image runat="server" ID="TokenImage" CssClass="tokenImage" />
                </span>
                <br />
                <span class="availableTokensLabel"><%=U4200.AVAILABLE %></span>
            </div>
        </div>
    </div>
</asp:PlaceHolder>
