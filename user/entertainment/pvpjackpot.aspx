<%@ Page Language="C#" AutoEventWireup="true" CodeFile="pvpjackpot.aspx.cs" Inherits="user_entertainment_pvpjackpot" MasterPageFile="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <style>
        .trans {
            display: none !important;
        }
    </style>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header">PvP <%=U5003.JACKPOTS %></h1>

    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=U6011.PVPJACKPOTDESCRIPTION %></p>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                    <asp:Button ID="PlayGame" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                    <asp:Button ID="BuyStage" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                </asp:PlaceHolder>
            </div>
        </div>
    </div>

    <div class="tab-content">
        <asp:Panel ID="SuccessPanel" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
            <asp:Literal ID="SuccessMessage" runat="server"></asp:Literal>
        </asp:Panel>
        <asp:Panel ID="ErrorPanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
            <asp:Literal ID="ErrorMessage" runat="server" />
        </asp:Panel>

        <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
            <asp:View runat="server" ID="View1">
                <div class="TitanViewElement">
                    <asp:GridView ID="PvpJackpotsStagesGridView" DataKeyNames='<%# new string[] { "Id", } %>'
                        AllowPaging="false" AllowSorting="false" DataSourceID="PvpJackpotsStagesDataSource" EmptyDataText="" runat="server" PageSize="20"
                        OnRowCommand="PvpJackpotsStages_RowCommand"
                        OnRowDataBound="PvpJackpotsStages_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="Id" ItemStyle-CssClass="displaynone" ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                            <asp:BoundField DataField="Name" HeaderText="Name" />
                            <asp:BoundField DataField="Cost" HeaderText="Cost" />
                            <asp:TemplateField HeaderText="Battles Amount">
                                <ItemTemplate>
                                    <asp:Literal runat="server" ID="BattlesAmountLiteral"></asp:Literal>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="WinPercent" HeaderText="Cash for every win" />
                            <asp:TemplateField HeaderText="">
                                <ItemStyle Width="13px" />
                                <ItemTemplate>
                                    <asp:LinkButton ID="StartImageButton" runat="server" CssClass="btn btn-inverse"
                                        CommandName="buy"
                                        CommandArgument='<%# Container.DataItemIndex %>'>
                                            
                                            <i class="fa fa-money" aria-hidden="true"></i>&nbsp;<%=L1.BUY %>
                                        </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <asp:SqlDataSource ID="PvpJackpotsStagesDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
                        OnInit="PvpJackpotsStagesDataSource_Init" />
                </div>
            </asp:View>

            <asp:View runat="server" ID="View2">
                <div class="TitanViewElement">
                    <asp:PlaceHolder runat="server" ID="UserStagesPlaceHolder">
                        <asp:GridView ID="UserStagesGridView" DataKeyNames='<%# new string[] { "Id", } %>'
                            AllowPaging="false" AllowSorting="false" DataSourceID="UserStagesGridViewDataSource" EmptyDataText="" runat="server" PageSize="20"
                            OnRowCommand="UserStagesGridView_RowCommand"
                            OnRowDataBound="UserStagesGridView_RowDataBound">
                            <Columns>
                                <asp:BoundField DataField="Id" ItemStyle-CssClass="displaynone" ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                <asp:BoundField DataField="StageId" HeaderText="Stage" />
                                <asp:BoundField DataField="BattlesDone" HeaderText="Battles left" />
                                <asp:TemplateField HeaderText="">
                                    <ItemStyle Width="13px" />
                                    <ItemTemplate>
                                        <asp:LinkButton ID="StartImageButton" runat="server" CssClass="btn btn-inverse"
                                            CommandName="play"
                                            CommandArgument='<%# Container.DataItemIndex %>'>
                                        <i class="fa fa-play" aria-hidden="true"></i>&nbsp;<%=U6011.PLAY %>
                                    </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="UserStagesGridViewDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
                            OnInit="UserStagesGridViewDataSource_Init" />
                    </asp:PlaceHolder>

                    <asp:PlaceHolder runat="server" ID="SearchOpponentPlaceHolder" Visible="false">
                        
                        <asp:UpdatePanel ID="TimeLeftUpdatePanel" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Timer OnTick="TimeLeftTimer_Tick" runat="server" ID="TimeLeftTimer" Interval="1000" />
                                <div class="row">
                                    <div class="col-md-12" style="text-align:center; font-size: 15px; padding-bottom: 50px;">
                                        <%=U6011.SEARCHINGOPPONENT %>
                                        <br /><br />
                                        <asp:Label ID="TimeLiteral" runat="server" CssClass="text-success" />
                                        <br /><br />
                                        <i class="fa fa-spinner fa-pulse fa-5x fa-fw"></i>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder runat="server" ID="BattleResultPlaceHolder" Visible="false">
                        <div style="text-align: center; font-size: 18px">
                            <asp:Literal runat="server" ID="WhoWonLiteral"/>
                            <asp:PlaceHolder runat="server" ID="SmilePlaceHolder" Visible="false"><br /><br /><i class="fa fa-smile-o fa-4x" aria-hidden="true" style="color:#1ada1a"></i></asp:PlaceHolder>
                            <asp:PlaceHolder runat="server" ID="SadPlaceHolder" Visible="false"><br /><br /><i class="fa fa-frown-o fa-4x" aria-hidden="true" style="color:#ff3333"></i></asp:PlaceHolder>
                            
                            <br /><br />
                            <asp:Button runat="server" ID="GoBackToStagesButton" OnClick="GoBackToStagesButton_Click" CssClass="btn btn-inverse"/>
                        </div>
                    </asp:PlaceHolder>

                </div>
            </asp:View>
        </asp:MultiView>
    </div>

</asp:Content>

