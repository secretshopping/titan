<%@ Page Language="C#" AutoEventWireup="true" CodeFile="profile.aspx.cs" Inherits="sites_profile" MasterPageFile="~/Sites.master" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <div class="content" data-scrollview="true">
        <div class="container" data-animation="true" data-animation-type="fadeInDown">
            <h2 class="content-title"><%=U3501.PROFILEVIEWER %></h2>
            <asp:PlaceHolder runat="server" ID="smallInfoPlaceHolder">
                <p class="text-center"><%=U3501.PROFILEVIEWERINFO %></p>
            </asp:PlaceHolder>


            <asp:Panel ID="EPanelProfile" runat="server" Visible="false" CssClass="alert alert-danger">
                <asp:Literal ID="ELiteralProfile" runat="server"></asp:Literal>
            </asp:Panel>
            <asp:Panel ID="SPanelProfile" runat="server" Visible="false" CssClass="alert alert-success">
                <asp:Literal ID="SLiteralProfile" runat="server"></asp:Literal>
            </asp:Panel>

            <div class="row">
                <div class="col-md-12">
                    <asp:Image ID="MainAvatarImage" runat="server" Width="50px" Height="50px" />

                    <h3>
                        <asp:Label ID="UsernameLabel" runat="server"></asp:Label></h3>
                </div>
                <div class="col-md-12">
                    <p>
                        <%=L1.MEMBERSHIP %>:
                                       
                        <asp:Literal ID="MembershipTypeLiteral" runat="server"></asp:Literal>
                    </p>

                    <asp:PlaceHolder runat="server" ID="AchievementsPlaceHolder" Visible="true">
                        <%=L1.ACHIEVEMENTS %>:      
                        <titan:MemberAchievementsList ID="MemberAchievementsList1" runat="server" MaxMiniaturesShown="8" Visible="false" />
                        <br />
                    </asp:PlaceHolder>

                    <p>
                        <%=L1.STATUS %>:
                            
                        <asp:Label ID="AccStatus" runat="server" Font-Bold="true"></asp:Label>
                    </p>

                    <asp:PlaceHolder runat="server" ID="ReferralsPlaceHolder" Visible="true">
                        <p>
                            <%=L1.REFERRALS %>:
                            
                            <asp:Label ID="Referrals" runat="server" Font-Bold="true"></asp:Label>
                        </p>
                    </asp:PlaceHolder>

                    <p>
                        <%=U3501.TOTALEARNED %>:
                            
                        <asp:Label ID="TotalEarned" runat="server" Font-Bold="true"></asp:Label>
                    </p>
                </div>
                <div class="row">
                    <div class="col-sm-3">
                        <asp:Button runat="server" ID="BefriendButton" OnClick="BefriendButton_Click" Text="Add Friend" CssClass="btn btn-primary btn-block" />
                    </div>
                    <div class="col-sm-3">
                        <asp:Button runat="server" ID="MessageButton" OnClick="MessageButton_Click" CssClass="btn btn-success btn-block" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>



