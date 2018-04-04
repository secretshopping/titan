<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SidebarMenu.ascx.cs" Inherits="Controls_SidebarMenu" %>

<asp:PlaceHolder runat="server" ID="MainPanel">

    <%--

    Here is the USER Menu template. Feel free to customize it.
    You should keep <li> IDs and runat="server" tag in the <li> elements unchanged. They are both required to turn some menu elements ON and OFF.--%>

    <%--Customize below --%>
    <!-- begin sidebar scrollbar -->
    <div data-scrollbar="true" data-height="100%">
        <!-- begin sidebar user -->
        <ul class="nav">
            <li class="nav-profile">
                <div class="image">
                    <a href="javascript:;">
                        <img src="<%=ResolveClientUrl(Member.CurrentInCache.AvatarUrl) %>" alt="" /></a>
                </div>
                <div class="info">
                    <strong><a href="<%=HtmlCreator.GetProfileURL(Member.CurrentInCache) %>"><%=Member.CurrentInCache.Name %></a></strong><br />
                    <asp:PlaceHolder ID="LeadershipSystemPlaceHolder" runat="server" Visible="false">
                        <a href="/user/leadershipsystem.aspx" runat="server" style="margin-bottom: 10px">
                            <small>
                                <%= U6005.RANK %>:
                                <asp:Literal ID="LeadershipSystemRankDetailLiteral" runat="server" />
                            </small>
                        </a>
                    </asp:PlaceHolder>
                    <small>
                        <p style="padding-left: 49px; line-height: 12px">
                            <span style="display: inline-block; margin-top: 5px">
                                <%=Member.CurrentInCache.MembershipName %>
                            </span>
                            <br />
                            <asp:PlaceHolder runat="server" ID="ExpirationPlaceHolder">
                                <span runat="server" id="expireDateSpan" class="f-s-10" style="display: inline-block;">
                                    <%=L1.EXPIRES %>:
                                    <asp:Literal ID="MembershipExpiresLiteral" runat="server" />
                                </span>
                            </asp:PlaceHolder>
                        </p>
                        <asp:PlaceHolder runat="server" ID="AriPlaceHolder">
                            <p style="padding-left: 49px;">
                                Matching Bonus:
                                <asp:Label runat="server" ID="MatchingBonusLabel"></asp:Label>
                            </p>
                            <p style="padding-left: 49px;">
                                Direct Referrals:
                                <asp:Label runat="server" ID="DirectRefCountLabel"></asp:Label>/Unlimited
                            </p>
                        </asp:PlaceHolder>

                    </small>
                </div>
                <titan:MenuBalances runat="server" />
                <br />
                <asp:PlaceHolder runat="server" ID="AdPackInfoPlaceHolder" Visible="false">
                    <table class="menu-balance-table">
                        <tr>
                            <asp:Literal runat="server" ID="AdPackInfoActivePackages" />
                        </tr>
                        <tr>
                            <asp:Literal runat="server" ID="AdPackInfoExpiredPackages" />
                        </tr>
                        <tr>
                            <asp:Literal runat="server" ID="AdPackInfoTotalPackages" />
                        </tr>
                    </table>
                </asp:PlaceHolder>

            </li>
        </ul>
        <!-- end sidebar user -->
        <!-- begin sidebar nav -->
        <ul class="nav" id="Global_MainMenu">

            <li class="nav-header">Menu</li>

            <li id="Summary" runat="server">
                <a href="/user/default.aspx" runat="server">
                    <i class="fa fa-fw fa-th-large"></i>
                    <span><%=L1.SUMMARY %></span>
                </a>
            </li>

            <li id="S4DSPackages" runat="server" visible="false">
                <a href="/user/s4dspackages/s4dspackages.aspx" runat="server">
                    <i class="fa fa-fw fa-dollar"></i>
                    <span>Your Packages</span>
                </a>
            </li>

            <li class="has-sub" id="PublishMenu" runat="server">
                <a href="javascript:;">
                    <b class="caret pull-right"></b>
                    <i class="fa fa-fw fa-globe"></i>
                    <span><%=U6000.PUBLISH %></span>
                </a>
                <ul class="sub-menu">
                    <li id="Publish1" runat="server">
                        <a href="/user/publish/websites.aspx" runat="server"><%=U6000.WEBSITES %></a>
                    </li>
                    <li id="Publish2" runat="server">
                        <a href="/user/publish/banners.aspx" runat="server">
                            <%=L1.BANNERS %></a>
                    </li>
                    <li id="Publish3" runat="server">
                        <a href="/user/publish/offerwalls.aspx" runat="server">
                            <%=U5009.OFFERWALLS %></a>
                    </li>
                    <li id="Publish4" runat="server">
                        <a href="/user/publish/globalpostback.aspx" runat="server">
                            <%=U6000.GLOBALPOSTBACK%></a>
                    </li>
                    <li id="Publish5" runat="server">
                        <a href="/user/publish/ptcofferwalls.aspx" runat="server">
                            <%=U6002.PTCOFFERWALLS %>
                        </a>
                    </li>
                    <li id="Publish6" runat="server">
                        <a href="/user/publish/intextads.aspx" runat="server">
                            <%=U6002.INTEXTADS %>
                        </a>
                    </li>
                </ul>
            </li>

            <li class="has-sub" id="EarnMenu" runat="server">
                <a href="javascript:;">
                    <b class="caret pull-right"></b>
                    <i class="fa fa-fw fa-dollar"></i>
                    <span><%=L1.EARN %> <%=GetNotificationHTML(Earn) %></span>
                </a>
                <ul class="sub-menu">

                    <li id="Earn1" runat="server">
                        <a href="/user/earn/cpaoffers.aspx" runat="server"><%=U6013.CPAGPTOFFERS %> <%=GetNotificationHTML(CPAOffers) %></a>
                    </li>
                    <li id="Earn2" runat="server">
                        <a href="/user/earn/ads.aspx" runat="server">
                            <%=U6003.PTC %> <%=GetNotificationHTML(Ads) %></a>
                    </li>
                    <li id="Earn4" runat="server">
                        <a href="/user/earn/trafficexchange.aspx" runat="server">
                            <%=U4000.TE %> </a>
                    </li>
                    <li id="Earn9" runat="server">
                        <a href="/user/earn/offers.aspx" runat="server"><%=U5009.OFFERWALLS %></a>
                    </li>
                    <li id="Earn5" runat="server">
                        <a href="/user/earn/search.aspx" runat="server">
                            <%=L1.SEARCH %></a>
                    </li>
                    <li id="Earn6" runat="server">
                        <a href="/user/earn/video.aspx" runat="server">Video</a>
                    </li>
                    <li id="Earn7" runat="server">
                        <a href="/user/earn/facebook.aspx" runat="server">
                            <%=L1.LIKES %><%=GetNotificationHTML(Facebook) %></a>
                    </li>
                    <li id="Earn8" runat="server">
                        <a href="/user/earn/trafficgrid.aspx" runat="server">Traffic Grid</a>
                    </li>

                    <li id="Earn11" runat="server">
                        <a href="/user/earn/refback.aspx" runat="server">RefBack</a>
                    </li>
                    <li id="Earn12" runat="server">
                        <a href="/user/earn/crowdflower.aspx" runat="server">Crowdflower</a>
                    </li>
                    <li id="Earn14" runat="server">
                        <a href="/user/earn/paidtopromote.aspx" runat="server"><%=U6009.PAIDTOPROMOTE %></a>
                    </li>
                    <li id="Earn15" runat="server">
                        <a href="/user/earn/coinhiveclaim.aspx" runat="server">Coinhive <%=U4200.CLAIM.ToLower() %></a>
                    </li>
                </ul>
            </li>

            <li class="has-sub" id="AdvertiseMenu" runat="server">
                <a href="javascript:;">
                    <b class="caret pull-right"></b>
                    <i class="fa fa-fw fa-bullhorn"></i>
                    <span><%=L1.ADVERTISE %></span>
                </a>
                <ul class="sub-menu">
                    <li id="Advertise1" runat="server">
                        <a href="/user/advert/cpaoffers.aspx" runat="server"><%=U6013.CPAGPTOFFERS %></a>
                    </li>
                    <li id="Advertise2" runat="server">
                        <a href="/user/advert/bannersoptions.aspx" runat="server">
                            <%=L1.BANNERS %></a>
                    </li>
                    <li id="Advertise3" runat="server">
                        <a href="/user/advert/ads.aspx" runat="server">
                            <%=U6003.PTC %></a>
                    </li>

                    <li id="Advertise10" runat="server">
                        <a href="/user/advert/surfads.aspx" runat="server">Surf Ads</a>
                    </li>
                    <li id="Advertise11" runat="server">
                        <a href="/user/advert/myurls.aspx" runat="server">
                            <%=U6002.MYURLS %>
                        </a>
                    </li>
                    <li id="Advertise12" runat="server">
                        <a href="/user/advert/ptcofferwalls.aspx" runat="server">
                            <%=U6002.PTCOFFERWALLS %>
                        </a>
                    </li>
                    <li id="Advertise13" runat="server">
                        <a href="/user/advert/intextads.aspx" runat="server">
                            <%=U6002.INTEXTADS %>
                        </a>
                    </li>
                    <li id="Advertise9" runat="server">
                        <a href="/user/advert/loginads.aspx" runat="server">
                            <%=U4200.LOGINADS %></a>
                    </li>
                    <li id="Advertise5" runat="server">
                        <a href="/user/advert/trafficexchange.aspx" runat="server">
                            <%=U4000.TE%></a>
                    </li>
                    <li id="Advertise6" runat="server">
                        <a href="/user/advert/facebook.aspx" runat="server">Facebook</a>
                    </li>
                    <li id="Advertise7" runat="server">
                        <a href="/user/advert/trafficgrid.aspx" runat="server">Traffic Grid</a>
                    </li>
                    <li id="Advertise14" runat="server">
                        <a href="/user/advert/minivideo.aspx" runat="server"><%=U6008.MINIVIDEO %></a>
                    </li>
                    <li id="Advertise15" runat="server">
                        <a href="/user/advert/paidtopromote.aspx" runat="server"><%=U6009.PAIDTOPROMOTE %></a>
                    </li>
                </ul>
            </li>

            <li class="has-sub" id="InvestmentPlatformMenu" runat="server">
                <a href="javascript:;">
                    <b class="caret pull-right"></b>
                    <i class="fa fa-sitemap"></i>
                    <span><%=U4200.INVEST %></span>
                </a>
                <ul class="sub-menu">
                    <li>
                        <a href="/user/investmentplatform/plans.aspx" runat="server">
                            <%=AppSettings.InvestmentPlatform.LevelsEnabled ? U5007.LEVELS : U6006.PLANS %></a>
                    </li>
                    <li id="InvestmentQueueSystem" runat="server">
                        <a href="/user/investmentplatform/queuesystem.aspx" runat="server">
                            <%=U6012.QUEUESYSTEM %></a>
                    </li>
                    <li id="InvestmentPlanHistory" runat="server">
                        <a href="/user/investmentplatform/history.aspx" runat="server">
                            <%=L1.HISTORY %></a>
                    </li>
                    <li id="InvestmentPlatformCalculator" runat="server">
                        <a href="/user/investmentplatform/calculator.aspx" runat="server">
                            <%=U6007.CALCULATOR %></a>
                    </li>
                </ul>
            </li>

            <li class="has-sub" id="NewsMenu" runat="server">
                <a href="javascript:;">
                    <b class="caret pull-right"></b>
                    <i class="fa fa-list"></i>
                    <span><%=U6002.NEWS %></span>
                </a>
                <ul class="sub-menu">
                    <li id="NewsHomepage" runat="server">
                        <a href="/sites/defaultnews.aspx" runat="server">
                            <%=U6012.HOMEPAGE %></a>
                    </li>
                    <li id="NewsSharingArticles" runat="server">
                        <a href="/user/news/sharing.aspx" runat="server">
                            <%=U6012.SHARINGARTICLES %></a>
                    </li>
                    <li id="NewsWritingArticles" runat="server">
                        <a href="/user/news/writing.aspx" runat="server">
                            <%=U6012.WRITINGARTICLES %></a>
                    </li>
                </ul>
            </li>

            <li class="has-sub" id="ICOMenu" runat="server">
                <a href="javascript:;">
                    <b class="caret pull-right"></b>
                    <i class="fa fa-qrcode"></i>
                    <span><%=U6012.ICO %></span>
                </a>
                <ul class="sub-menu">
                    <li id="ICOInfo" runat="server">
                        <a href="/user/ico/info.aspx" runat="server">
                            <%=U6012.INFORMATIONS %></a>
                    </li>
                    <li id="ICOBuy" runat="server">
                        <a href="/user/ico/buy.aspx" runat="server">
                            <%=L1.BUY %> <%=Titan.Cryptocurrencies.CryptocurrencyFactory.Get(Titan.Cryptocurrencies.CryptocurrencyType.ERC20Token).Code %></a>
                    </li>
                    <li id="ICOStages" runat="server">
                        <a href="/user/ico/stages.aspx" runat="server">
                            <%=U6012.STAGES %></a>
                    </li>
                    <li id="ICOHistory" runat="server">
                        <a href="/user/ico/history.aspx" runat="server">
                            <%=L1.HISTORY %></a>
                    </li>
                </ul>
            </li>

            <li class="has-sub" id="InternalExchangeMenu" runat="server">
                <a href="javascript:;" runat="server">
                    <b class="caret pull-right"></b>
                    <i class="fa fa-fw fa-bar-chart-o"></i>
                    <span><%=U6012.INTERNALEXCHANGE %></span>
                </a>

                <ul class="sub-menu">
                    <li id="InternalExchangeMain" runat="server">
                        <a href="/user/exchange/internalexchange.aspx" runat="server">
                            <%=String.Format("{0} / {1} {2}",   Titan.InternalExchange.InternalExchangeManager.GetBalanceCode(true), 
                                                                Titan.InternalExchange.InternalExchangeManager.GetBalanceCode(false), 
                                                                U6012.EXCHANGE) %></a>
                    </li>
                    <li id="InternalExchangeOrders" runat="server">
                        <a href="/user/exchange/currentorders.aspx" runat="server">
                            <%=U6012.MYCURRENTORDERS %></a>
                    </li>

                    <li id="InternalExchangeHistory" runat="server">
                        <a href="/user/exchange/history.aspx" runat="server">
                            <%=U6012.MYTRADINGHISTORY %></a>
                    </li>

                </ul>

            </li>

            <li class="has-sub" id="RevenueSharingMenu" runat="server">
                <a href="javascript:;">
                    <b class="caret pull-right"></b>
                    <i class="fa fa-fw fa-exchange"></i>
                    <span><%=U6000.REVSHARE %> <%=GetNotificationHTML(AdPacksAds) %></span>
                </a>

                <ul class="sub-menu">

                    <li id="RevenueSharing1" runat="server">
                        <a href="/user/earn/adpacks.aspx" runat="server">
                            <%=U6000.SURF %><%=GetNotificationHTML(AdPacksAds) %></a>
                    </li>
                    <li id="RevenueSharing2" runat="server">
                        <a href="/user/advert/adpacks.aspx" runat="server">
                            <%=AppSettings.RevShare.AdPack.AdPackNamePlural %></a>
                    </li>

                    <li id="RevenueSharing4" runat="server">
                        <a href="/user/shares/calculator.aspx" runat="server">
                            <%=U6007.CALCULATOR %></a>
                    </li>

                    <li id="RevenueSharing3" runat="server">
                        <a href="/user/advert/groups/list.aspx" runat="server">
                            <%=U4200.GROUPS%></a>
                    </li>
                </ul>
            </li>

            <li class="has-sub" id="CryptocurrencyTradingPlatformMenu" runat="server">
                <a href="javascript:;">
                    <b class="caret pull-right"></b>
                    <i class="fa fa-btc fa-bar-chart-o"></i>
                    <span><%=U6010.CCPLATFORM %></span>
                </a>
                <ul class="sub-menu">
                    <li id="CryptocurrencyPlatformBuy" runat="server">
                        <a href="/user/cctrading/buy.aspx" runat="server">
                            <%=L1.BUY%> <%=AppSettings.CryptocurrencyTrading.CryptocurrencyCode %></a>
                    </li>
                    <li id="CryptocurrencyPlatformSell" runat="server">
                        <a href="/user/cctrading/sell.aspx" runat="server">
                            <%=U6009.SELL%> <%=AppSettings.CryptocurrencyTrading.CryptocurrencyCode %></a>
                    </li>
                </ul>
            </li>

            <li id="Marketplace" runat="server">
                <a href="/user/advert/marketplace.aspx" runat="server">
                    <i class="fa fa-fw fa-shopping-cart"></i>
                    <span><%=U5006.MARKETPLACE %></span>
                </a>
            </li>

            <li class="has-sub" id="ReferralsMenu" runat="server">
                <a href="javascript:;">
                    <b class="caret pull-right"></b>
                    <i class="fa fa-fw fa-users"></i>
                    <span><%=L1.REFERRALS %><%=GetNotificationHTML(Referrals) %></span>
                </a>
                <ul class="sub-menu">
                    <li id="Referrals1" runat="server">
                        <a href="/user/refs/direct.aspx" runat="server">
                            <%=L1.DIRECT %><%=GetNotificationHTML(DirectReferrals) %></a>
                    </li>
                    <li id="Referrals2" runat="server">
                        <a href="/user/refs/indirect.aspx" runat="server">
                            <%=U3000.INDIRECT %></a>
                    </li>
                    <li id="Referrals3" runat="server">
                        <a href="/user/refs/rented.aspx" runat="server">
                            <%=L1.RENTED %></a>
                    </li>
                    <li id="Referrals5" runat="server">
                        <a href="/user/refs/banners.aspx" runat="server">
                            <%=U5007.TOOLS %></a>
                    </li>
                    <li id="Referrals8" runat="server">
                        <a href="/user/refs/matrix.aspx" runat="server">
                            <%=U6003.MATRIX %><%=GetNotificationHTML(UnassignedMatrixMembers) %></a>
                    </li>
                    <li id="Referrals6" runat="server">
                        <a href="/user/refs/leadershiprewards.aspx" runat="server">
                            <%=U5006.LEADERSHIP %></a>
                    </li>
                    <li id="Referrals7" runat="server">
                        <a href="/user/refs/rotatorlink.aspx" runat="server">
                            <%=U5007.ROTATORLINK %></a>
                    </li>
                </ul>
            </li>

            <li class="has-sub" id="MoneyMenu" runat="server">
                <a href="javascript:;">
                    <b class="caret pull-right"></b>
                    <i class="fa fa-fw fa-money"></i>
                    <span><%=U4000.MONEY %></span>
                </a>
                <ul class="sub-menu">
                    <li id="Money1" runat="server">
                        <a href="/user/deposit.aspx" runat="server">
                            <%=U4200.DEPOSIT %></a>
                    </li>
                    <li id="Li1" runat="server">
                        <a href="/user/transfer.aspx" runat="server">
                            <%=L1.TRANSFER %></a>
                    </li>
                    <li id="Money2" runat="server">
                        <a href="/user/cashout.aspx" runat="server">
                            <%=L1.CASHOUT %></a>
                    </li>
                    <li id="Money5" runat="server">
                        <a href="/user/money/creditline.aspx" runat="server">
                            <%=U5006.CREDITLINE %></a>
                    </li>
                    <li id="Money3" runat="server">
                        <a href="/user/giftcards.aspx" runat="server">
                            <%=U4000.GIFTCARDS %></a>
                    </li>

                    <li id="Money6" runat="server">
                        <a href="/user/money/receipts.aspx" runat="server">
                            <%=U5008.RECEIPTS %></a>
                    </li>
                    <li id="Money4" runat="server">
                        <a href="/user/money/logs.aspx" runat="server">
                            <%=U4000.LOGS %></a>
                    </li>
                </ul>
            </li>

            <li class="has-sub" id="EntertainmentMenu" runat="server">
                <a href="javascript:;">
                    <b class="caret pull-right"></b>
                    <i class="fa fa-fw fa-trophy"></i>
                    <span><%=U6000.ENTERTAINMENT %></span>
                </a>
                <ul class="sub-menu">
                    <li id="Entertainment1" runat="server">
                        <a href="/user/earn/contests.aspx" runat="server">
                            <%=L1.CONTESTS %></a>
                    </li>

                    <li id="Entertainment2" runat="server">
                        <a href="/user/money/jackpot.aspx" runat="server">
                            <%=U5003.JACKPOTS %></a>
                    </li>

                    <li id="Entertainment11" runat="server">
                        <a href="/user/entertainment/pvpjackpot.aspx" runat="server">
                            <%=U6011.PVPJACKPOTS %></a>
                    </li>

                    <li id="Entertainment3" runat="server">
                        <a href="/user/games.aspx" runat="server">
                            <%=L1.GAMES %></a>
                    </li>
                    <li id="Entertainment4" runat="server">
                        <a href="/user/dicegame.aspx" runat="server">
                            <%=U4200.DICE %></a>
                    </li>
                    <li id="Entertainment5" runat="server">
                        <a href="/user/trophies.aspx" runat="server">
                            <%=L1.ACHIEVEMENTSMENU %></a>
                    </li>
                    <li id="Entertainment6" runat="server">
                        <a href="/user/entertainment/webinars.aspx" runat="server">
                            <%=U6003.WEBINARS %></a>
                    </li>
                    <li id="Entertainment7" runat="server">
                        <a href="/user/entertainment/ebooks.aspx" runat="server">
                            <%=U6004.EBOOKS %></a>
                    </li>
                    <li id="Entertainment8" runat="server">
                        <a href="/user/entertainment/slotMachine.aspx" runat="server">
                            <%=U6006.SLOTMACHINE %></a>
                    </li>
                    <li id="Entertainment9" runat="server">
                        <a href="/user/entertainment/minivideo.aspx" runat="server">
                            <%=U6008.MINIVIDEO %></a>
                    </li>
                    <li id="Entertainment10" runat="server">
                        <a href="/user/entertainment/rolldicelottery.aspx" runat="server">
                            <%=U6010.ROLLDICELOTTERY %></a>
                    </li>
                </ul>
            </li>


            <li class="has-sub" id="PeopleMenu" runat="server">
                <a href="javascript:;">
                    <b class="caret pull-right"></b>
                    <i class="fa fa-fw fa-comments"></i>
                    <span><%=L1.PEOPLE %> <%=GetNotificationHTML_Important(PendingRepresentativePaymentRequest) %> <%=GetNotificationHTML(Messages) %></span>
                </a>
                <ul class="sub-menu">
                    <li id="People3" runat="server">
                        <a id="PeopleProfileLink" runat="server">
                            <%=U6000.PROFILE %></a>
                    </li>
                    <li id="People1" runat="server">
                        <a href="/user/network/messenger.aspx" runat="server">
                            <%=L1.MESSAGES %> <%=GetNotificationHTML_Important(PendingRepresentativePaymentRequest) %> <%=GetNotificationHTML(Messages) %></a>
                    </li>
                    <li id="People5Representatives" runat="server">
                        <a href="/sites/representatives.aspx" runat="server">
                            <%=U6002.REPRESENTATIVES %></a>
                    </li>
                    <li id="People2" runat="server">
                        <a href="/user/network/friends.aspx" runat="server">
                            <%=L1.FRIENDS %></a>
                    </li>
                    <li id="People4" runat="server">
                        <a href="/user/testimonials.aspx" runat="server">
                            <%=U5008.TESTIMONIALS %></a>
                    </li>
                    <li id="People5LeadershipSystem" runat="server">
                        <a href="/user/leadershipsystem.aspx" runat="server">
                            <%=U6005.LEADERSHIPSYSTEM %></a>
                    </li>
                </ul>
            </li>



            <li class="has-sub" id="StatisticsMenu" runat="server">
                <a href="javascript:;">
                    <b class="caret pull-right"></b>
                    <i class="fa fa-fw fa-bar-chart-o"></i>
                    <span><%=L1.STATISTICS %></span>
                </a>
                <ul class="sub-menu">
                    <li id="Statistics6" runat="server">
                        <a href="/user/statistics/leaderboard.aspx" runat="server">
                            <%=U6000.LEADERBOARD %> </a>
                    </li>
                    <li id="Statistics1" runat="server">
                        <a href="/user/statistics/money.aspx" runat="server">
                            <%=U4000.MONEY%></a>
                    </li>
                    <li id="Statistics2" runat="server">
                        <a href="/user/statistics/points.aspx" runat="server">
                            <%= AppSettings.PointsName%></a>
                    </li>
                    <li id="Statistics4" runat="server">
                        <a href="/user/statistics/adpacks.aspx" runat="server">
                            <%=AppSettings.RevShare.AdPack.AdPackName%></a>
                    </li>
                    <li id="Statistics5" runat="server">
                        <a href="/user/statistics/ads.aspx" runat="server">
                            <%=U6003.PTC %> </a>
                    </li>
                </ul>
            </li>
            <!-- begin sidebar minify button -->
            <li><a href="javascript:;" class="sidebar-minify-btn" data-click="sidebar-minify"><i class="fa fa-angle-double-left"></i></a></li>
            <!-- end sidebar minify button -->
        </ul>
        <!-- end sidebar nav -->

    </div>
    <!-- end sidebar scrollbar -->

</asp:PlaceHolder>
