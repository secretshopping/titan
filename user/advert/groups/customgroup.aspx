<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="customgroup.aspx.cs" Inherits="About" %>

<%@ MasterType VirtualPath="~/User.master" %>


<asp:Content ContentPlaceHolderID="PageHeadContent" runat="server">
    <link rel="stylesheet" href="Styles/WatchAds.css" type="text/css" />
</asp:Content>


<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <asp:UpdatePanel ID="MultiViewUpdatePanel" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="Button2" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="Button1" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="JoinGroupButton" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="LeaveGroupButton" EventName="Click" />
        </Triggers>

        <ContentTemplate>

            <div class="row">
                <div class="col-md-12">
                    <asp:Panel ID="SPanel" runat="server" Visible="false" CssClass="alert alert-success">
                        <asp:Literal ID="SText" runat="server"></asp:Literal>
                    </asp:Panel>

                    <asp:Panel ID="EPanel" runat="server" Visible="false" CssClass="alert alert-danger">
                        <asp:Literal ID="EText" runat="server"></asp:Literal>
                    </asp:Panel>

                    <asp:Panel ID="WPanel" runat="server" Visible="false" CssClass="alert alert-warning">
                        <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                    </asp:Panel>
                </div>
            </div>
      
            <div class="row">
                <div class="col-md-12">
                    <div class="nav nav-tabs custom text-right">
                        <div class="TitanViewPage">
                            <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                                <asp:Button ID="Button2" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                                <asp:Button ID="Button1" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </div>
        
            <div class="tab-content">
                <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
                    <asp:View runat="server" ID="View1">
                        <div class="TitanViewElement">
                            <%-- SUBPAGE START --%>
                            <div class="row">
                                <div class="col-md-12">
                                    <h1>
                                        <asp:Literal runat="server" ID="GroupNameLiteral" Mode="Encode"></asp:Literal>
                                    </h1>
                                    <p>
                                        <asp:Literal ID="GroupURL" runat="server"></asp:Literal></p>
                                    <p>
                                        <asp:Literal runat="server" ID="PacksLeftLiteral"></asp:Literal></p>
                                </div>
                            </div> 

                            <div class="row">
                                <div class="col-md-12">
                                    <div class="groupDescription">
                                        <h2 runat="server"><%=L1.DESCRIPTION %></h2>
                                        <p>
                                            <asp:Literal runat="server" ID="GroupDescriptionLiteral" Mode="Encode"></asp:Literal>
                                        </p>
                                    </div>
                                    <div class="groupContact">
                                        <h2 runat="server"><%=U4200.CONTACTME %></h2>
                                        <span class="groupHeaderSpan"><%=L1.USERNAME%>:</span>
                                        <span class="groupHeaderSpan">
                                            <asp:Literal runat="server" ID="NameLiteral"></asp:Literal>
                                        </span>
                                        <p runat="server" id="EmailPlaceHolder">
                                            <span class="groupHeaderSpan"><%=U4200.EMAIL %>:</span>
                                            <span>
                                                <asp:Literal runat="server" ID="EmailLiteral" Mode="Encode"></asp:Literal>
                                            </span>
                                        </p>
                                        <p runat="server" id="SkypePlaceHolder">
                                            <span class="groupHeaderSpan"><%=U4200.SKYPE %>:</span>
                                            <span>
                                                <asp:Literal runat="server" ID="SkypeLiteral" Mode="Encode"></asp:Literal>
                                            </span>
                                        </p>
                                        <p runat="server" id="PhonePlaceHolder">
                                            <span class="groupHeaderSpan"><%=U4200.PHONE %>:</span>
                                            <span>
                                                <asp:Literal runat="server" ID="PhoneLiteral" Mode="Encode"></asp:Literal>
                                            </span>
                                        </p>
                                        <p runat="server" id="FacebookPlaceHolder">
                                            <span class="groupHeaderSpan">Facebook:</span>
                                            <span>
                                                <asp:Literal runat="server" ID="FacebookLiteral"></asp:Literal>
                                            </span>
                                        </p>
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-12">
                                    <div runat="server" id="VideoImagePlaceholder" class="groupVideoImage">
                                        <asp:Panel runat="server" ID="VideoPanel">
                                            <asp:Literal runat="server" ID="VideoLiteral"></asp:Literal>
                                        </asp:Panel>
                                        <asp:Panel runat="server" ID="ImagePanel">
                                            <asp:Literal runat="server" ID="ImageLiteral"></asp:Literal>
                                        </asp:Panel>
                                    </div>

                                    <h2><%=U5001.PEOPLEWHOJOINED%>:</h2>

                                    <asp:Literal runat="server" ID="ParticipantListLiteral"></asp:Literal>

                                    <asp:PlaceHolder ID="RewardInfoPlaceHolder" runat="server" Visible="false">
                                        <h2><%=U4000.REWARD%>:</h2>
                                        <asp:Literal runat="server" ID="RewardInfoLiteral"></asp:Literal>
                                    </asp:PlaceHolder>
                                </div>
                            </div> 
                        </div>
                    </asp:View>

                    <asp:View runat="server" ID="View2">
                        <div class="TitanViewElement">
                           
                            <asp:Literal runat="server" ID="PacksLeftLiteral2"></asp:Literal>
                           
                            <asp:PlaceHolder runat="server" ID="OpenGroupPlaceholder">
                                <div class="row">
                                    <div class="col-md-12">
                                        <div runat="server" id="AdPacksPlaceHolder" class="alert alert-info">
                                            <%= AppSettings.RevShare.AdPack.AdPackNamePlural %> <%=U4200.AVAILABLE.ToLower() %> <%=U4200.TOADD %>: 
                                            <b><asp:Label runat="server" ID="AvailableAdPacksLabel"></asp:Label></b>
                                            <br />
                                            <%= AppSettings.RevShare.AdPack.AdPackNamePlural %> <%=U4200.AVAILABLE.ToLower() %> <%=U4200.TOWITHDRAW %>: 
                                            <b><asp:Literal runat="server" ID="MyAdPacks"></asp:Literal></b>
                                        </div>    
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <label class="control-label col-md-2">
                                                    <%= AppSettings.RevShare.AdPack.AdPackNamePlural %>
                                                </label>
                                                <div class="col-md-6">
                                                    <asp:TextBox runat="server" ID="AdPacksTextBox" MaxLength="150" CssClass="form-control"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" Display="Dynamic"
                                                    ControlToValidate="AdPacksTextBox" CssClass="text-danger" runat="server" ValidationGroup="CustomGroup" ErrorMessage="<%=U4200.ENTERREQUIREDFIELDS %>">*</asp:RequiredFieldValidator>    
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <div class="col-md-3">
                                                    <asp:Button runat="server" ID="JoinGroupButton" CommandArgument="true" OnClick="JoinLeaveGroupButton_Click" CssClass="btn btn-success btn-block" CausesValidation="true" ValidationGroup="CustomGroup" />
                                                </div>
                                                <div class="col-md-3">
                                                    <asp:Button runat="server" ID="LeaveGroupButton" CommandArgument="false" OnClick="JoinLeaveGroupButton_Click" CssClass="btn btn-danger btn-block" CausesValidation="true" ValidationGroup="CustomGroup" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </asp:PlaceHolder>

                            <asp:PlaceHolder runat="server" ID="ClosedGroupPlaceHolder">
                                <div class="row">
                                    <div class="col-md-12">
                                        <p class="alert alert-warning text-center">
                                            <%=U4200.CUSTOMGROUPCLOSED %>
                                        </p>
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                        </div>
                    </asp:View>
                </asp:MultiView>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
