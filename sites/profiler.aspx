<%@ Page Language="C#" AutoEventWireup="true" CodeFile="profiler.aspx.cs" Inherits="sites_profiler" MasterPageFile="~/Sites.master" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <div class="content" data-scrollview="true">
        <div class="container" data-animation="true" data-animation-type="fadeInDown">
            <h2 class="content-title"><%=U3900.PROFILERSURVEY %></h2>
            <p class="text-center"><%=U3900.PROFILERSURVEYINFO %></p>
            <p class="text-center"><%=Prem.PTC.AppSettings.Authentication.ProfilingSurveyReward > 0 ? U3900.PROFILERREWARD.Replace("%n%", "<b>" + Prem.PTC.AppSettings.Authentication.ProfilingSurveyReward + " " + Prem.PTC.AppSettings.PointsName + "</b>") : "" %></p>
            <div class="row">
                <div class="col-md-12">
                    <asp:Panel ID="Panel2" runat="server" Visible="false" CssClass="alert alert-danger">
                        <asp:Literal ID="Literal4" runat="server"></asp:Literal>
                    </asp:Panel>
                    <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="alert alert-danger"
                        ValidationGroup="RegisterUserValidationGroup" DisplayMode="List" />
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <asp:Panel ID="CustomFields" runat="server"></asp:Panel>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-3">
                    <asp:Button ID="ProfileButton" runat="server"
                        CssClass="btn btn-inverse btn-block" OnClick="ProfileButton_Click" ValidationGroup="RegisterUserValidationGroup" />
                </div>
                <div class="col-sm-3">
                    <asp:Button ID="SkipButton" runat="server"
                        CssClass="btn btn-default btn-block" OnClick="SkipButton_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
