<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="trafficexchange.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">

    <link rel="stylesheet" href="Styles/WatchAds.css" type="text/css" />

</asp:Content>





<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <input type="hidden" name="__EVENTARGUMENT5" id="__EVENTARGUMENT5" value="" />

    <h1 class="page-header"><%=L1.TRAFFICEXCHANGE %></h1>

    <div class="row">
        <div class="col-md-12">
            <p class="alert alert-info">
                All campaigns are also available from fixed links:<br />
                <i><%=Prem.PTC.AppSettings.Site.Url%>user/earn/auto1</i>, 
                <i><%=Prem.PTC.AppSettings.Site.Url%>user/earn/auto2</i>, 
                <i><%=Prem.PTC.AppSettings.Site.Url%>user/earn/auto3</i> ...
            </p>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12 text-center m-b-20 m-t-20">
            <span class="fa fa-refresh fa-5x"></span>
        </div>
    </div>
    <div class="row">
        <div class="col-sm-2 col-sm-offset-5">
            <asp:PlaceHolder runat="server" ID="AdsAvailablePlaceholder">
                <button onclick="window.open('user/earn/auto1')" class="btn btn-inverse btn-block"><%=U4200.STARTSURFING %></button>
            </asp:PlaceHolder>
        </div>
        <asp:PlaceHolder runat="server" ID="AdsUnavailablePlaceholder">
            <h3 class="text-center"><%=U4200.NOADSAVAILABLEFORUSER %></h3>
        </asp:PlaceHolder>
    </div>




</asp:Content>
