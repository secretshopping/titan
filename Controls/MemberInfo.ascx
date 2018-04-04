<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MemberInfo.ascx.cs" Inherits="Controls_MemberInfo" %>
<%@ Register Src="~/Controls/MemberAchievementsList.ascx" TagPrefix="titan" TagName="MemberAchievementsList" %>

<div class="row">
    <div class="col-md-6 text-center">
        <a href="user/default.aspx">
            <asp:Image ID="MainAvatarImage" runat="server" Style="border-radius: 50%;" />
        </a>
    </div>
    <div class="col-md-6 profile-data">
        <div class="row">
            <h3 class="m-t-10"><asp:Literal runat="server" ID="UserProfileLinkLiteral"></asp:Literal></h3>
        </div>
        <div class="row">
            <asp:PlaceHolder runat="server" ID="MembershipsPlaceholder">
                <p><%=L1.MEMBERSHIP %>: 
                    <asp:Literal ID="MembershipTypeLiteral1" runat="server"></asp:Literal>
                </p>
                <p runat="server" id="ExpirationPlaceHolder"><%=L1.EXPIRES %>: 
                    <asp:Literal ID="MembershipExpiresLiteral" runat="server"></asp:Literal>
                    <asp:Literal ID="MembershipWarningLiteral" runat="server"></asp:Literal>
                </p>
            </asp:PlaceHolder>

            <asp:PlaceHolder runat="server" ID="MembershipsLevelPlaceholder">
                <p class="smalltext">
                    <asp:Literal ID="MembershipTypeLiteral2" runat="server"></asp:Literal>
                </p>
                <asp:Literal ID="LevelProgressLiteral" runat="server"></asp:Literal>
            </asp:PlaceHolder>
        </div>
    </div>
</div>

<titan:MemberAchievementsList ID="MemberAchievementsList1" runat="server" MaxMiniaturesShown="8" />
