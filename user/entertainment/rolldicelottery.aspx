<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true" CodeFile="rolldicelottery.aspx.cs" Inherits="user_entertainment_rolldicelottery" %>

<%@ Import Namespace="Titan.RollDiceLottery" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script type="text/javascript">
        function pageLoad() {
            $(".preventClickBtn").click(function (e) { e.preventDefault(); });
            $('#confirmationModal').modal('hide');
        }

        function askForConfirmation(parameter) {
            updateModal();
            $('#confirmationModal').modal({ 'backdrop': true, 'show': true });
            let promise = new Promise(function (resolve, reject) {
                let confirmButton = document.getElementById('confirmButton');
                confirmButton.addEventListener("click", function () {
                    resolve();
                });
            });
            promise.then(function () {
                __doPostBack(parameter.name, '');
            });
        }

        function updateModal() {
            var $modal = $('#confirmationModal');

            $modal.find('.modal-body').html('<h2 class="text-center m-t-0"><%=string.Format("{0}({1}): {2}", L1.PRICE, U6012.PURCHASEBALANCE, RollDiceLotteryManager.GetParticipatePrice)%></h2><br /><h3 class="text-success text-center"><%=U6010.ROLLDICEWARNING %></h3>');
        }
        
    </script>
    <style>
        .trans {
            display: none !important;
        }

        img.img-responsive {
            margin: 0 auto;
        }
    </style>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=U6010.ROLLDICELOTTERY %></h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=U6010.ROLLDICELOTTERYDESCRIPTION %></p>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger">
                <asp:Literal ID="ErrorMessage" runat="server" />
            </asp:Panel>
            <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="alert alert-success">
                <asp:Literal ID="SuccMessage" runat="server" />
            </asp:Panel>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                    <asp:Button ID="ResultsButton" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                    <asp:Button ID="GameButton" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                </asp:PlaceHolder>
            </div>
        </div>
    </div>

    <div class="tab-content">
        <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
            <asp:View runat="server" ID="GameView">

                <asp:PlaceHolder runat="server" ID="EnterTheGamePlaceHolder">
                    <div class="row">
                        <div class="col-md-12">
                            <h3>
                                <asp:Literal runat="server" ID="PrizesDescriptionLiteral" /></h3>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-4 col-md-offset-4 m-t-30 m-b-30">
                            <asp:Button runat="server" ID="EnterGameButton" OnClick="EnterGameButton_Click" OnClientClick="askForConfirmation(this)" CssClass="preventClickBtn btn btn-block btn-lg btn-inverse" />
                        </div>
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder runat="server" ID="GamePlaceHolder" Visible="false">
                    <div class="TitanViewElement">
                        <div class="row">
                            <div class="col-md-12">

                                <asp:UpdatePanel ID="ResultsUpdatePanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>

                                        <asp:Timer OnTick="DiceTimer_Tick" runat="server" ID="DiceTimer" Interval="1000" />
                                        <div class="row">
                                            <div class="col-md-12">
                                                <asp:Label ID="TimeLiteral" runat="server" CssClass="text-danger" />
                                            </div>
                                        </div>
                                        
                                        <div class="row">
                                            <div class="col-sm-6">
                                                <p>
                                                    <asp:Literal runat="server" ID="ScoreLiteral" />
                                                    <asp:Literal runat="server" ID="RollsLiteral" />
                                                </p>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-sm-4 col-sm-offset-4">
                                                <asp:Button runat="server" ID="RollDiceButton" OnClick="RollDiceButton_Click" CssClass="btn btn-lg btn-block btn-success m-b-30" />
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-xs-4">
                                                <asp:Image runat="server" ID="diceOneImg" CssClass="img-responsive" />
                                            </div>
                                            <div class="col-xs-4">
                                                <asp:Image runat="server" ID="diceTwoImg" CssClass="img-responsive" />
                                            </div>
                                            <div class="col-xs-4">
                                                <asp:Image runat="server" ID="diceThreeImg" CssClass="img-responsive" />
                                            </div>
                                        </div>

                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="RollDiceButton" EventName="click" />
                                    </Triggers>
                                </asp:UpdatePanel>

                                <div class="row">
                                    <div class="col-xs-12 col-sm-4 col-sm-offset-4">
                                        <asp:Button runat="server" ID="SubmitScoreButton" OnClick="SubmitScoreButton_Click" CssClass="btn btn-block btn-inverse m-b-40 m-t-40" />
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder runat="server" ID="CurrentResultsPlaceHolder">

                    <h3>
                        <asp:Literal runat="server" ID="CurrentResultsGridViewTitleLiteral" />:</h3>
                    <asp:GridView ID="CurrentResultsGridView" runat="server" OnPreRender="BaseGridView_PreRender" AllowSorting="True" AutoGenerateColumns="False"
                        DataSourceID="CurrentResultsGridViewSqlDataSource" PageSize="15" OnRowDataBound="ResultsGridView_RowDataBound">
                        <Columns>
                            <asp:BoundField HeaderText="<%$ ResourceLookup : USERNAME %>" SortExpression="UserId" DataField="UserId" />
                            <asp:BoundField HeaderText="<%$ ResourceLookup : SCORE %>" SortExpression='Score' DataField="Score" />
                            <asp:BoundField HeaderText="<%$ ResourceLookup : ROLLSNUMBER %>" SortExpression='NumberOfRolls' DataField="NumberOfRolls" />
                            <asp:BoundField HeaderText="<%$ ResourceLookup : TIME %>" SortExpression='GameTime' DataField="GameTime" />
                        </Columns>
                    </asp:GridView>
                    <asp:SqlDataSource ID="CurrentResultsGridViewSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="CurrentResultsGridViewSqlDataSource_Init" />

                </asp:PlaceHolder>

            </asp:View>

            <asp:View runat="server" ID="LastResultsView">
                <div class="TitanViewElement">

                    <asp:PlaceHolder runat="server" ID="HistoryOnePlaceHolder">
                        <h3 class="text-center">
                            <asp:Literal runat="server" ID="HistoryOneLiteral" /></h3>
                        <asp:GridView ID="HistoryOneGridView" runat="server" OnPreRender="BaseGridView_PreRender" AllowSorting="True" AutoGenerateColumns="False"
                            DataSourceID="HistoryOneGridViewSqlDataSource" PageSize="15" OnRowDataBound="ResultsGridView_RowDataBound">
                            <Columns>
                                <asp:BoundField HeaderText="<%$ ResourceLookup : USERNAME %>" SortExpression="UserId" DataField="UserId" />
                                <asp:BoundField HeaderText="<%$ ResourceLookup : SCORE %>" SortExpression='Score' DataField="Score" />
                                <asp:BoundField HeaderText="<%$ ResourceLookup : ROLLSNUMBER %>" SortExpression='NumberOfRolls' DataField="NumberOfRolls" />
                                <asp:BoundField HeaderText="<%$ ResourceLookup : TIME %>" SortExpression='GameTime' DataField="GameTime" />
                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="HistoryOneGridViewSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="HistoryOneGridViewSqlDataSource_Init" />
                    </asp:PlaceHolder>

                    <asp:PlaceHolder runat="server" ID="HistoryTwoPlaceHolder">
                        <h3 class="text-center">
                            <asp:Literal runat="server" ID="HistoryTwoLiteral" /></h3>
                        <asp:GridView ID="HistoryTwoGridView" runat="server" OnPreRender="BaseGridView_PreRender" AllowSorting="True" AutoGenerateColumns="False"
                            DataSourceID="HistoryTwoGridViewSqlDataSource" PageSize="15" OnRowDataBound="ResultsGridView_RowDataBound">
                            <Columns>
                                <asp:BoundField HeaderText="<%$ ResourceLookup : USERNAME %>" SortExpression="UserId" DataField="UserId" />
                                <asp:BoundField HeaderText="<%$ ResourceLookup : SCORE %>" SortExpression='Score' DataField="Score" />
                                <asp:BoundField HeaderText="<%$ ResourceLookup : ROLLSNUMBER %>" SortExpression='NumberOfRolls' DataField="NumberOfRolls" />
                                <asp:BoundField HeaderText="<%$ ResourceLookup : TIME %>" SortExpression='GameTime' DataField="GameTime" />
                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="HistoryTwoGridViewSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="HistoryTwoGridViewSqlDataSource_Init" />
                    </asp:PlaceHolder>

                    <asp:PlaceHolder runat="server" ID="HistoryThreePlaceHolder">
                        <h3 class="text-center">
                            <asp:Literal runat="server" ID="HistoryThreeLiteral" /></h3>
                        <asp:GridView ID="HistoryThreeGridView" runat="server" OnPreRender="BaseGridView_PreRender" AllowSorting="True" AutoGenerateColumns="False"
                            DataSourceID="HistoryThreeGridViewSqlDataSource" PageSize="15" OnRowDataBound="ResultsGridView_RowDataBound">
                            <Columns>
                                <asp:BoundField HeaderText="<%$ ResourceLookup : USERNAME %>" SortExpression="UserId" DataField="UserId" />
                                <asp:BoundField HeaderText="<%$ ResourceLookup : SCORE %>" SortExpression='Score' DataField="Score" />
                                <asp:BoundField HeaderText="<%$ ResourceLookup : ROLLSNUMBER %>" SortExpression='NumberOfRolls' DataField="NumberOfRolls" />
                                <asp:BoundField HeaderText="<%$ ResourceLookup : TIME %>" SortExpression='GameTime' DataField="GameTime" />
                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="HistoryThreeGridViewSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="HistoryThreeGridViewSqlDataSource_Init" />
                    </asp:PlaceHolder>

                </div>
            </asp:View>
        </asp:MultiView>
    </div>
</asp:Content>



