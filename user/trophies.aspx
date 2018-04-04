<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="trophies.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">

</asp:Content>


<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <h1 class="page-header"><%=L1.ACHIEVEMENTS %></h1>
    
    <div class="tab-content">
        <div class="row">
            <div class="col-md-12">
                <p class="lead"><%= L1.ACHIEVEMENTSINFO.Replace("%c%", Prem.PTC.AppSettings.PointsName) %>  (<%= L1.ACHIEVEMENTSMENU %>)</p>
            </div>
        </div> 

        <div class="row">
            <div class="col-md-12">
                <h3><%=L1.ACHIEVEMENTSMENU %></h3>
                <p><asp:Label ID="AchivCount" runat="server" Font-Bold="true"></asp:Label>/<asp:Label ID="AchivTotalCount" runat="server"></asp:Label></p>
            </div>
        </div> 

        <div class="row">
            <div class="col-md-12">
                <asp:Panel ID="WarningPanel" runat="server" Visible="false" CssClass="alert alert-warning fade in m-b-15">
                    <asp:Literal ID="WarningLiteral" runat="server"></asp:Literal>
                </asp:Panel>
            </div>
        </div>

        <div class="row">
            <div class="col-md-12">
                <div class="box">
                    <asp:Literal ID="UserAchivsLiteral" runat="server"></asp:Literal>
                </div>
            </div>
        </div>    
  
        <div class="row">
            <div class="col-md-12">
                <h3><%=L1.ALLAVAILABLEACHIVS %></h3>
                <p><asp:Literal ID="AllAchivsLiteral" runat="server"></asp:Literal></p>
            </div>
        </div>
    </div>     

</asp:Content>
