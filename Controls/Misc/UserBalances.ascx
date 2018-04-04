<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UserBalances.ascx.cs" Inherits="Controls_Misc_UserBalances" %>

<script data-cfasync="false" type="text/javascript">
    jQuery(function ($) {  
        $("div.stats-info p").each(function () {
            var $balance = $(this);
            var num = <%=CoreSettings.GetMaxDecimalPlaces().ToString() %>;
            if (num >= 1) {
                $balance.css('font-size', '22px');
            }
            if (num >= 3) {
                $balance.css('font-size', '20px');
            }
            if (num >= 4) {
                $balance.css('font-size', '16px');
            }
            if (num >= 6) {
                $balance.css('font-size', '14px');
            }
        });
    })
</script>

<div class="row" id="User_Balances">
    <!-- begin col-3 -->
    <div class="col-lg-2 col-md-3 col-sm-6" id="MainBalanceContainer" runat="server">
        <div class="widget widget-stats bg-green">
            <div class="stats-icon"><i class="fa fa-money"></i></div>
            <div class="stats-info">
                <h4><%=!TitanFeatures.IsRofriqueWorkMines ? U5004.MAIN : "Mined Bitcoins"%> </h4>
                <p><%=Member.CurrentInCache.MainBalance %><br /><br /></p>
                
            </div>
        </div>
    </div>
    <!-- end col-3 -->
    <!-- begin col-3 -->
    <asp:PlaceHolder ID="AdBalancePlaceHolder" runat="server" Visible="true">
        <div class="col-lg-2 col-md-3 col-sm-6">
            <div class="widget widget-stats bg-black-lighter">
                <div class="stats-icon"><i class="fa fa-bullhorn"></i></div>
                <div class="stats-info">
                    <h4><%=U6012.PURCHASE %> </h4>
                    <p><%=Member.CurrentInCache.PurchaseBalance %><br /><br /> </p>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>

    <!-- end col-3 -->
    <!-- begin col-3 -->
    <div class="col-lg-2 col-md-3 col-sm-6" id="CommissionBalanceContainer" runat="server">
        <div class="widget widget-stats bg-black-lighter">
            <div class="stats-icon"><i class="fa fa-user"></i></div>
            <div class="stats-info">
                <h4><%=U5004.COMMISSION %> </h4>
                <p><%=Member.CurrentInCache.CommissionBalance %> <br /><br /></p>
            </div>
        </div>
    </div>
    <!-- end col-3 -->
    <!-- begin col-3 -->
    <div class="col-lg-2 col-md-3 col-sm-6" id="TrafficBalanceContainer" runat="server">
        <div class="widget widget-stats bg-black-lighter">
            <div class="stats-icon"><i class="fa fa-exchange"></i></div>
            <div class="stats-info">
                <h4><%=U5004.TRAFFIC %> </h4>
                <p><%=Member.CurrentInCache.TrafficBalance %> <br /><br /></p>
            </div>
        </div>
    </div>
    <!-- end col-3 -->
    <!-- begin col-3 -->
    <div class="col-lg-2 col-md-3 col-sm-6" id="PointsBalanceContainer" runat="server">
        <div class="widget widget-stats bg-black-lighter">
            <div class="stats-icon"><i class="fa fa-trophy"></i></div>
            <div class="stats-info">
                <h4><%=AppSettings.PointsName %> </h4>
                <p><%=Member.CurrentInCache.PointsBalance %><br /><small style="font-size:0.7em"><asp:Literal runat="server" ID="EstimatedPointsValueLiteral"/></small> </p>
                
            </div>
        </div>
    </div>
    <!-- end col-3 -->

    <asp:PlaceHolder ID="DailyTaskPlaceHolder" Visible="false" runat="server">
        <!-- begin col-3 -->
        <div class="col-lg-4 col-md-6 col-sm-12" id="DailyTaskContainer" runat="server">
            <div class="widget widget-stats" style="background: #9E36A9 !important">
                <div class="stats-icon"><i class="fa fa-power-off"></i></div>
                <div class="stats-info">
                    <h4><%=U6010.DAILYTASK %> </h4>
                    <p><%=String.Format(U6010.DAILYTASKINFO, Member.Current.Address, Member.Current.NumberOfWatchedTrafficAdsToday, AppSettings.TrafficExchange.AmountOfAdsToWatchForcedByAdmin)%><br /><br /></p>
                </div>
            </div>
        </div>
        <!-- end col-3 -->
    </asp:PlaceHolder>

    <!-- begin col-3 -->
    <div class="col-lg-2 col-md-3 col-sm-6" id="CashBalanceContainer" runat="server">
        <div class="widget widget-stats bg-black-lighter">
            <div class="stats-icon"><i class="fa fa-balance-scale"></i></div>
            <div class="stats-info">
                <h4><%=!TitanFeatures.IsRofriqueWorkMines ? U6002.CASH : "Funds Deposited"%> </h4>
                <p><%=Member.CurrentInCache.CashBalance %> <br /><br /></p>
            </div>
        </div>
    </div>
    <!-- end col-3 -->
    <!-- begin col-3 -->
    <div class="col-lg-2 col-md-3 col-sm-6" id="InvestmentBalanceContainer" runat="server">
        <div class="widget widget-stats bg-black-lighter">
            <div class="stats-icon"><i class="fa fa-suitcase"></i></div>
            <div class="stats-info">
                <h4><%=U6006.INVESTMENT %> </h4>
                <p><%=Member.CurrentInCache.InvestmentBalance %> <br /><br /></p>
            </div>
        </div>
    </div>
    <!-- end col-3 -->
    <!-- begin col-3 -->
    <div class="col-lg-2 col-md-3 col-sm-6" id="MarketplaceBalanceContainer" runat="server">
        <div class="widget widget-stats bg-black-lighter">
            <div class="stats-icon"><i class="fa fa-suitcase"></i></div>
            <div class="stats-info">
                <h4><%=U5006.MARKETPLACE %> </h4>
                <p><%=Member.CurrentInCache.MarketplaceBalance %><br /><br /> </p>
            </div>
        </div>
    </div>
    <!-- end col-3 -->
    <!-- begin col-3 -->
    <div class="col-lg-2 col-md-3 col-sm-6" id="BTCWalletContainer" runat="server">
        <div class="widget widget-stats bg-orange">
            <div class="stats-icon"><i class="fa fa-btc"></i></div>
            <div class="stats-info">
                <h4><%=String.Format(U6012.WALLET, "BTC") %></h4>
                <p><%=Member.CurrentInCache.GetCryptocurrencyBalance(Titan.Cryptocurrencies.CryptocurrencyType.BTC) %><br /><small style="font-size:0.7em"><asp:Literal runat="server" ID="EstimatedBTCWalletValueLiteral"/> </small></p>
                
            </div>
        </div>
    </div>
    <!-- end col-3 -->
    <!-- begin col-3 -->
    <div class="col-lg-2 col-md-3 col-sm-6" id="ERC20TokenWalletContainer" runat="server">
        <div class="widget widget-stats bg-black-lighter">
            <div class="stats-icon"><i class="fa fa-th"></i></div>
            <div class="stats-info">
                <h4><%=String.Format(U6012.WALLET, TokenCryptocurrency.Code) %> </h4>
                <p><%=Member.CurrentInCache.GetCryptocurrencyBalance(Titan.Cryptocurrencies.CryptocurrencyType.ERC20Token)%> <br /><small style="font-size:0.7em"><asp:Literal runat="server" ID="EstimatedERC20WalletValueLiteral"/></small></p>
                
            </div>
        </div>
    </div>
    <!-- end col-3 -->
    <!-- begin col-3 -->
    <div class="col-lg-2 col-md-3 col-sm-6" id="ERC20FreezedTokensContainer" runat="server">
        <div class="widget widget-stats bg-black-lighter">
            <div class="stats-icon"><i class="fa fa-th"></i></div>
            <div class="stats-info">
                <h4><%=String.Format("{0} Wallet (Freezed)", TokenCryptocurrency.Code) %> </h4>
                <p><%=Member.CurrentInCache.GetCryptocurrencyBalance(Titan.Cryptocurrencies.CryptocurrencyType.ERCFreezed)%><br /> <small style="font-size:0.7em"><asp:Literal runat="server" ID="EstimatedERC20FreezedTokensValueLiteral"/> </small></p>
               
            </div>
        </div>
    </div>
    <!-- end col-3 -->
</div>
