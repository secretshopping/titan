<%@ Page Language="C#" AutoEventWireup="true" CodeFile="advertise.aspx.cs" Inherits="sites_advertise" MasterPageFile="~/Sites.master" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <div class="content" data-scrollview="true">
        <div class="container" data-animation="true" data-animation-type="fadeInDown">
            <h2 class="content-title"><%=L1.ADVERTISE %></h2>
            <div class="row">
                <div class="col-md-12">
                    <p class="text-center"><%=L1.OUT1 %></p>
                </div>
            </div>

            <div class="row m-t-20">
                <div class="col-md-6">
                    <div id="div01" class="logo-selected">
                        <h2><%=L1.OUT2 %></h2>
                        <p><%=L1.OUT3 %></p>
                        <ul class="personalized-ul">
                            <li><%=U4200.CREATENEWCAMPAIGN %></li>
                            <li><%=L1.OUT5 %></li>
                            <li><%=L1.OUT6 %></li>
                            <li><%=L1.OUT7 %></li>
                            <li><%=L1.OUT8 %></li>
                            <li><%=L1.STATISTICS %></li>
                            <li><%=L1.OUT9 %></li>
                        </ul>

                        <div>
                            <a href="register.aspx" class="btn btn-link text-success"><%=L1.REGISTER %></a>
                            <a href="login.aspx" class="btn btn-link text-success"><%=L1.LOGIN %></a>
                        </div>

                    </div>
                </div>
                <div class="col-md-6">
                    <div runat="server" id="div02" class="logo">
                        <h2><%=L1.OUT10 %></h2>
                        <p><%=L1.OUT11 %></p>

                        <ul class="personalized-ul">
                            <li><%=U4200.CREATENEWCAMPAIGN %></li>
                            <li><%=L1.OUT5 %></li>
                            <li><%=L1.OUT6 %></li>
                            <li><%=L1.OUT12 %></li>
                            <li><%=L1.OUT13 %></li>
                        </ul>

                        <div>
                            <a runat="server" id="ptcLink" href="user/advert/ads.aspx" class="btn btn-link text-success">PTC</a>
                            <a runat="server" id="bannerLink" href="user/advert/banners.aspx" class="btn btn-link text-success">Banner</a>
                            <a runat="server" id="facebookLink" href="user/advert/facebook.aspx" class="btn btn-link text-success">Facebook</a>
                            <a runat="server" id="trafficGridLink" href="user/advert/trafficgrid.aspx" class="btn btn-link text-success">TrafficGrid</a>
                            <a runat="server" id="marketplaceLink" href="user/advert/marketplace.aspx" class="btn btn-link text-success"><%=U5006.MARKETPLACE %></a>
                            <br />
                            <a runat="server" id="investmentPlatformCalculatorLink" href="user/investmentplatform/calculator.aspx" class="btn btn-link text-success"><%=U6006.INVESTMENTPLATFORM %></a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
