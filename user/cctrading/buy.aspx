<%@ Page Language="C#" AutoEventWireup="true" CodeFile="buy.aspx.cs" Inherits="user_btctrading_Buy" MasterPageFile="~/User.master" %>

<%@ MasterType VirtualPath="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script type="text/javascript">
        function pageLoad() {
            $("#<%=AmountToBuyTextBox.ClientID%>").spinner({
                step:<%=NumericUpDownStep%>,
                min: <%=MinCryptocurrencyAmount%>,
                max: <%=MaxCryptocurrencyAmount%>,
                spin: function (event, ui) {
                    var CCAmount = ui.value;
                    var PricePerCurrency = <%=PricePerCryptocurrency%>;           
                    var TotalPrice = CCAmount * PricePerCurrency;
                    $('#<%=CashToPayLabel.ClientID%>').val(TotalPrice.toFixed(<%=CoreSettings.GetMaxDecimalPlaces() %>).toString());
                }
            })
        };

        function updatePrice() {
            var CCAmount = $("#<%=AmountToBuyTextBox.ClientID%>").val();
            var PricePerCurrency = <%=PricePerCryptocurrency%>;           
            var TotalPrice = CCAmount * PricePerCurrency;
            $('#<%=CashToPayLabel.ClientID%>').val(TotalPrice.toFixed(<%=CoreSettings.GetMaxDecimalPlaces() %>).toString());
        }

        function getDescription(btn)
        {
            var row = $(btn).parent().parent();
            var desc = $(row).find('.description-column').text();
            var $modal = $('#welcomeModal');
            $modal.find('.modal-header').html('<h3 class="text-center m-t-0"><%=U6010.OFFERDESCRIPTION%></h3>');
            $modal.find('.modal-body').html('<h5 class="text-success" style="white-space:pre-wrap">'+desc+'</h5>');
            $('#welcomeModal').modal({ 'backdrop': true, 'show': true });
        }

        function getYourComment(btn)
        {
            var row = $(btn).parent().parent();
            var comm = $(row).find('.your-comment-column').text();
            var $modal = $('#welcomeModal');
            $modal.find('.modal-header').html('<h3 class="text-center m-t-0"> <%=U6010.COMMENT_YOUR%> </h3>');
            $modal.find('.modal-body').html('<h5 class="text-success" style="white-space:pre-wrap">' + comm + '</h5>');
            $('#welcomeModal').modal({ 'backdrop': true, 'show': true });
        }

        function getComment(btn)
        {
            var row = $(btn).parent().parent();
            var comm = $(row).find('.comment-column').text();
            var $modal = $('#welcomeModal');
            $modal.find('.modal-header').html('<h3 class="text-center m-t-0"> <%=U6010.COMMENT_CLIENT%> </h3>');
            $modal.find('.modal-body').html('<h5 class="text-success" style="white-space:pre-wrap">'+ comm+'</h5>');
            $('#welcomeModal').modal({ 'backdrop': true, 'show': true });
            console.log(comm);
        }

    </script>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=L1.BUY%> <%=AppSettings.CryptocurrencyTrading.CryptocurrencyCode %></h1>
    <div class="row">
        <div class="col-md-12" >
            <p class="lead"><%=U6010.CCPLATFORMDESC%></p>
        </div>
    </div>
    <asp:UpdatePanel ID="MultiViewUpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="row">
                <div class="col-md-12">
                    <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                        <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                    </asp:Panel>
                    <asp:Panel ID="SuccessMessagePanel" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
                        <asp:Literal ID="SuccessMessage" runat="server"></asp:Literal>
                    </asp:Panel>
                    <titan:CryptocurrencyBalancesInfo runat="server" />
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <div class="nav nav-tabs custom text-right">
                        <div class="TitanViewPage">
                            <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                                <asp:Button ID="MenuButtonFourthTab" runat="server" OnClick="MenuButton_Click" CommandArgument="3" />
                                <asp:Button ID="MenuButtonThirdTab" runat="server" OnClick="MenuButton_Click" CommandArgument="2" />
                                <asp:Button ID="MenuButtonSecondTab" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                                <asp:Button ID="MenuButtonFirstTab" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="tab-content">
                <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
                    <asp:View runat="server" ID="SellOffersView" OnActivate="SellOffersView_Activate">
                        <div class="TitanViewElement">
                            <br />
                            <asp:Panel ID="AllOffersErrorPanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                                <asp:Literal ID="AllOffersErrorLiteral" runat="server"></asp:Literal>
                            </asp:Panel>
                            <asp:GridView ID="AllSellOffersGridView" DataKeyNames='<%# new string[] { "Id", } %>'
                                AllowPaging="true" AllowSorting="true" DataSourceID="AllSellOffersGridView_DataSource"
                                OnRowCommand="AllSellOffersGridView_RowCommand" runat="server" PageSize="40"
                                OnRowDataBound="AllSellOffersGridView_RowDataBound">
                                <Columns>
                                    <asp:BoundField HeaderText="Id" SortExpression="Id" DataField="Id" ItemStyle-CssClass="displaynone" ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField HeaderText="<%$ ResourceLookup : USERNAME %>" DataField="CreatorId"/>
                                    <asp:BoundField HeaderText="<%$ ResourceLookup : DATEADDED %>" SortExpression="DateAdded" DataField="DateAdded"/>
                                    <asp:BoundField HeaderText="<%$ ResourceLookup : PRICE %>" SortExpression="MinPrice" DataField="MinPrice"/>
                                    <asp:TemplateField HeaderText="<%$ ResourceLookup : TRANSACTIONVALUE %>"></asp:TemplateField>
                                    <asp:BoundField HeaderText="<%$ ResourceLookup : AMOUNTLEFT %>" SortExpression="AmountLeft" DataField="AmountLeft"/>
                                    <asp:TemplateField HeaderText="">
                                        <ItemStyle Width="13px" />
                                        <ItemTemplate>
                                            <asp:LinkButton ID="StartImageButton" runat="server" CssClass="btn btn-inverse"
                                                CommandName="buy"
                                                CommandArgument='<%# Container.DataItemIndex %>'>
                                                <%=L1.BUY %>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:SqlDataSource ID="AllSellOffersGridView_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
                                OnInit="AllSellOffersGridView_DataSource_Init" />
                        </div>
                    </asp:View>
                    <asp:View runat="server" ID="CreateBuyOfferView" OnActivate="CreateBuyOfferView_Activate">
                        <div class="TitanViewElement">
                            <br />
                            <asp:Panel ID="CreateBuyErrorPanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                                <asp:Literal ID="CreateBuyErrorLiteral" runat="server"></asp:Literal>
                            </asp:Panel>

                            <div class="row">
                                <div class="col-md-12">
                                    <div class="form-horizontal">
                                        <div class="row">

                                            <div class="form-group">
                                                <div class="col-xs-5 col-sm-5 col-md-4 col-lg-4">
                                                    <label class="control-label pull-right">
                                                        <%=L1.AMOUNT %>
                                                    </label>
                                                </div>
                                                <div class="input-group">
                                                    <span class="add-on input-group-addon"><%=AppSettings.CryptocurrencyTrading.CryptocurrencySign %></span>
                                                    <asp:TextBox ID="BuyAmountTextBox" runat="server" CssClass="form-control" Width="200px"></asp:TextBox>
                                                </div>
                                                <div class="col col-xs-offset-5 col-sm-offset-5 col-md-offset-4 col-lg-offset-4">
                                                    <asp:RequiredFieldValidator ID="Amount_RequiredFieldValidator" ControlToValidate="BuyAmountTextBox" Display="Dynamic" CssClass="text-danger" runat="server" ValidationGroup="AddNewOffer_ValidationGroup"></asp:RequiredFieldValidator>
                                                    <asp:RangeValidator ID="Amount_RangeValidator" runat="server" ValidationGroup="AddNewOffer_ValidationGroup"
                                                        ControlToValidate="BuyAmountTextBox" MinimumValue="0" MaximumValue="999999999" Type="Double" Display="Dynamic" CssClass="text-danger"></asp:RangeValidator>
                                                </div>
                                            </div><br />

                                            <div class="form-group">
                                                <div class="col-xs-5 col-sm-5 col-md-4 col-lg-4" style="text-align: right">
                                                    <label class="control-label pull-right">
                                                        <%=L1.PRICE %>
                                                    </label>
                                                </div>
                                                <div class="input-group">
                                                    <span class="add-on input-group-addon"><%=AppSettings.Site.CurrencySign %></span>
                                                    <asp:TextBox ID="MaxPriceTextBox" runat="server" CssClass="form-control" Width="200px"></asp:TextBox>
                                                    <label style="padding-top: 8px;">&nbsp;/ <%=AppSettings.CryptocurrencyTrading.CryptocurrencyCode %></label>
                                                </div>
                                                <div class="col col-xs-offset-5 col-sm-offset-5 col-md-offset-4 col-lg-offset-4">
                                                    <asp:RequiredFieldValidator ID="Price_RequiredFieldValidator" ControlToValidate="MaxPriceTextBox" Display="Dynamic" CssClass="text-danger" runat="server" ValidationGroup="AddNewOffer_ValidationGroup"></asp:RequiredFieldValidator>
                                                    <asp:RangeValidator ID="Price_RangeValidator" runat="server" ValidationGroup="AddNewOffer_ValidationGroup"
                                                        ControlToValidate="MaxPriceTextBox" MinimumValue="0" MaximumValue="999999999" Type="Double" Display="Dynamic" CssClass="text-danger"></asp:RangeValidator>
                                                </div>
                                            </div><br />

                                            <div class="form-group">
                                                <div class="col-xs-5 col-sm-5 col-md-4 col-lg-4" style="text-align: right">
                                                    <label class="control-label pull-right">
                                                        <%=U6010.CCMINOFFERVALUE %>
                                                    </label>
                                                </div>
                                                <div class="input-group">
                                                    <span class="add-on input-group-addon"><%=AppSettings.Site.CurrencySign %></span>
                                                    <asp:TextBox ID="MinOfferValueTextBox" runat="server" CssClass="form-control" Width="200px"></asp:TextBox>
                                                </div>
                                                <div class="col col-xs-offset-5 col-sm-offset-5 col-md-offset-4 col-lg-offset-4">
                                                    <asp:RequiredFieldValidator ID="MinTransaction_RequiredFieldValidator" ControlToValidate="MinOfferValueTextBox" Display="Dynamic" CssClass="text-danger" runat="server"  ValidationGroup="AddNewOffer_ValidationGroup"></asp:RequiredFieldValidator>
                                                    <asp:RangeValidator ID="MinTransaction_RangeValidator" runat="server" ValidationGroup="AddNewOffer_ValidationGroup"
                                                        ControlToValidate="MinOfferValueTextBox" MinimumValue="0" MaximumValue="999999999" Type="Double" Display="Dynamic" CssClass="text-danger"></asp:RangeValidator>
                                                </div>
                                            </div><br />

                                            <div class="form-group">
                                                <div class="col-xs-5 col-sm-5 col-md-4 col-lg-4" style="text-align: right">
                                                    <label class="control-label pull-right">
                                                        <%=U6010.CCMAXOFFERVALUE %>
                                                    </label>
                                                </div>
                                                <div class="input-group">
                                                    <span class="add-on input-group-addon"><%=AppSettings.Site.CurrencySign %></span>
                                                    <asp:TextBox ID="MaxOfferValueTextBox" runat="server" CssClass="form-control" Width="200px"></asp:TextBox>
                                                </div>
                                                <div class="col col-xs-offset-5 col-sm-offset-5 col-md-offset-4 col-lg-offset-4">
                                                    <asp:RequiredFieldValidator ID="MaxTransaction_RequiredFieldValidator" ControlToValidate="MaxOfferValueTextBox" Display="Dynamic" CssClass="text-danger" runat="server" ValidationGroup="AddNewOffer_ValidationGroup"></asp:RequiredFieldValidator>
                                                    <asp:RangeValidator ID="MaxTransaction_RangeValidator" runat="server" ValidationGroup="AddNewOffer_ValidationGroup"
                                                        ControlToValidate="MaxOfferValueTextBox" MinimumValue="0" MaximumValue="999999999" Type="Double" Display="Dynamic" CssClass="text-danger"></asp:RangeValidator>
                                                </div>
                                            </div><br />

                                            <div class="form-group">
                                                <div class="col-xs-5 col-sm-5 col-md-4 col-lg-4" style="text-align: right">
                                                    <label class="control-label pull-right">
                                                        <%=U6010.ESCROWTIME %>
                                                        <asp:Literal ID="EscrowDescriptionLiteral" runat="server"></asp:Literal>
                                                    </label> 
                                                </div>
                                                <div class="input-group">
                                                    <span class="add-on input-group-addon" style="padding-right: 10px;"><i class="fa fa-clock-o" aria-hidden="true"></i></span>
                                                    <asp:TextBox ID="EscrowTimeTextBox" runat="server" CssClass="form-control" Width="200px"></asp:TextBox>
                                                    <label style="padding-top: 8px;"> &nbsp;<%=(U5005.MINUTES).ToLower() %></label>
                                                </div>
                                                <div class="col col-xs-offset-5 col-sm-offset-5 col-md-offset-4 col-lg-offset-4">
                                                    <asp:RangeValidator ID="Escrow_RangeValidator1" runat="server" ValidationGroup="AddNewOffer_ValidationGroup"
                                                        ControlToValidate="EscrowTimeTextBox" MinimumValue="0" MaximumValue="999999999" Type="Integer" Display="Dynamic" CssClass="text-danger"></asp:RangeValidator><br />
                                                    <asp:RangeValidator ID="Escrow_RangeValidator2" runat="server" ValidationGroup="AddNewOffer_ValidationGroup"
                                                        ControlToValidate="EscrowTimeTextBox" MinimumValue="30" MaximumValue="999999999" Type="Integer" Display="Dynamic" CssClass="text-danger"></asp:RangeValidator>
                                                    <asp:RequiredFieldValidator ID="Escrow_RequiredFieldValidator" ControlToValidate="EscrowTimeTextBox" Display="Dynamic" CssClass="text-danger" runat="server" ValidationGroup="AddNewOffer_ValidationGroup"></asp:RequiredFieldValidator>
                                                </div>
                                            </div><br />

                                            <br /><br />
                                            <div class="row">
                                                <div class="col-xs-5 col-sm-5 col-md-4 col-lg-4 col-xl-4
                                                            col-xs-offset-5 col-sm-offset-5 col-md-offset-4 col-lg-offset-4 col-xl-offset-4">
                                                    <asp:Button ID="AddBuyOfferButton" runat="server" CssClass="btn btn-inverse" ValidationGroup="AddNewOffer_ValidationGroup" CausesValidation="true" OnClick="AddBuyOfferButton_Click" />
                                                </div>
                                            </div><br /><br />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </asp:View>
                    <asp:View runat="server" ID="OfferManagementView" OnActivate="OfferManagementView_Activate">
                        <asp:PlaceHolder ID="CurrentUserTransactionsPlaceHolder" runat="server" Visible="false">
                            <h2><%=U6010.TRANSACTIONSINPROGRESS %></h2>
                            <asp:GridView ID="CurrentUserTransactionsGridView" DataKeyNames='<%# new string[] { "Id", } %>'
                                AllowPaging="true" AllowSorting="false" DataSourceID="CurrentUserTransactionsGridView_DataSource" runat="server" PageSize="20"
                                OnRowDataBound="CurrentUserTransactionsGridView_RowDataBound"
                                OnRowCommand="CurrentUserTransactionsGridView_RowCommand">
                                <Columns>
                                    <asp:BoundField SortExpression="Id" DataField="Id" ItemStyle-CssClass="displaynone" ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField SortExpression="CCAmount" DataField="CCAmount" />
                                    <asp:BoundField SortExpression="ExecutionTime" DataField="ExecutionTime" />
                                    <asp:BoundField SortExpression="PaymentStatus" DataField="PaymentStatus" />
                                    <asp:TemplateField ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone"></asp:TemplateField><%--Description--%>
                                    <asp:TemplateField></asp:TemplateField><%--TimeLeft--%>
                                    <asp:TemplateField HeaderText="">
                                        <ItemStyle Width="13px" />
                                        <ItemTemplate>
                                            <asp:LinkButton ID="ConfirmPaymentButton" runat="server" CssClass="btn btn-inverse"
                                                CommandName="Confirm"
                                                CommandArgument='<%# Container.DataItemIndex %>'>
                                                <%=U6010.CONFIRMPAYMENT %>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="">
                                        <ItemStyle Width="13px" />
                                        <ItemTemplate >          
                                            <asp:LinkButton ID="GetDescriptionButton" runat="server" CssClass="btn btn-inverse"
                                                OnClientClick="getDescription(this)"><span class="fa fa-info-circle fa-lg"></span>
                                            </asp:LinkButton>      
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:SqlDataSource ID="CurrentUserTransactionsGridView_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
                                OnInit="CurrentUserTransactionsGridView_DataSource_Init" />
                        </asp:PlaceHolder>

                        <br /><h2><%=U6010.ACTIVEOFFERS %></h2>
                        <asp:GridView ID="CurrentUserBuyOfferGridView" DataKeyNames='<%# new string[] { "Id", } %>'
                            AllowPaging="true" AllowSorting="false" DataSourceID="CurrentUserBuyOfferGridView_DataSource"
                            OnRowCommand="CurrentUserBuyOfferGridView_RowCommand" runat="server" PageSize="20"
                            OnRowDataBound="CurrentUserBuyOfferGridView_RowDataBound">
                            <Columns>
                                <asp:BoundField HeaderText="Id" SortExpression="Id" DataField="Id" ItemStyle-CssClass="displaynone" ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                <asp:BoundField SortExpression="DateAdded" DataField="DateAdded" />
                                <asp:BoundField SortExpression="MaxPrice" DataField="MaxPrice" />
                                <asp:TemplateField></asp:TemplateField><%--Transaction Limits--%>
                                <asp:BoundField SortExpression="Amount" DataField="Amount" />
                                <asp:BoundField SortExpression="AmountLeft" DataField="AmountLeft" />
                                <asp:BoundField SortExpression="Status" DataField="Status" />
                                <asp:TemplateField HeaderText="">
                                    <ItemStyle Width="13px" />
                                    <ItemTemplate>
                                        <asp:LinkButton ID="StartImageButton" runat="server"
                                            ToolTip='Start'
                                            CommandName="start"
                                            CommandArgument='<%# Container.DataItemIndex %>'><span class="fa fa-play fa-lg text-success"></span></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="">
                                    <ItemStyle Width="13px" />
                                    <ItemTemplate>
                                        <asp:LinkButton ID="PauseImageButton" runat="server"
                                            ToolTip='<%$ ResourceLookup : PAUSE %>'
                                            CommandName="stop"
                                            CommandArgument='<%# Container.DataItemIndex %>'><span class="fa fa-pause fa-lg text-warning"></span></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="">
                                    <ItemStyle Width="13px" />
                                    <ItemTemplate>
                                        <asp:LinkButton runat="server"
                                            ToolTip='<%$ ResourceLookup : REMOVE %>'
                                            CommandName="remove"
                                            CommandArgument='<%# Container.DataItemIndex %>'>
                                        <spin class="fa fa-times fa-lg text-danger"></spin>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="CurrentUserBuyOfferGridView_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
                            OnInit="CurrentUserBuyOfferGridView_DataSource_Init" />
                    </asp:View>
                    <asp:View runat="server" ID="OfferHistoryView" OnActivate="OfferHistoryView_Activate">
                        <asp:GridView ID="OfferHistoryGridView" DataKeyNames='<%# new string[] { "Id", } %>'
                            AllowPaging="true" AllowSorting="false" DataSourceID="OfferHistoryGridView_DataSource" runat="server" PageSize="20"
                            OnRowDataBound="OfferHistoryGridView_RowDataBound"
                            OnRowCommand="OfferHistoryGridView_RowCommand">
                            <Columns>
                                <asp:BoundField HeaderText="Id" SortExpression="Id" DataField="Id" ItemStyle-CssClass="displaynone" ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                <asp:BoundField SortExpression="SellerId" DataField="SellerId" />
                                <asp:BoundField SortExpression="CCAmount" DataField="CCAmount" />
                                <asp:BoundField SortExpression="BuyerComment" DataField="BuyerComment"  ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone"/>
                                <asp:BoundField SortExpression="SellerComment" DataField="SellerComment"  ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone"/>
                                <asp:BoundField SortExpression="Rating" DataField="Rating" ItemStyle-CssClass="displaynone" ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone"/>
                                <asp:TemplateField Visible="true">
                                    <ItemStyle Width="13px"/>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="AddYourCommentButton" runat="server" CssClass="btn btn-inverse"
                                            CommandName="AddComment"
                                            CommandArgument='<%# Container.DataItemIndex %>'>
                                                <%=U6010.ADDCOMMENT %>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <%--JS COMMENTS--%>
                                <asp:TemplateField>
                                    <ItemStyle Width="150px"/>
                                    <ItemTemplate>          
                                        <asp:LinkButton ID="GetYourCommentButton" runat="server" CssClass="btn btn-inverse"
                                            OnClientClick="getYourComment(this)"><span class="fa fa-id-card-o fa-lg"></span>
                                        </asp:LinkButton>      
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle Width="150px" />
                                    <ItemTemplate >          
                                        <asp:LinkButton ID="GetCommentButton" runat="server" CssClass="btn btn-inverse"
                                            OnClientClick="getComment(this)"><span class="fa fa-share-square-o fa-lg"></span>
                                        </asp:LinkButton>      
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="OfferHistoryGridView_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
                            OnInit="OfferHistoryGridView_DataSource_Init" />
                    </asp:View>
                    <asp:View ID="BuyOfferInfoWindow" runat="server">
                        <div class="TitanViewElement">
                            <asp:Panel ID="BuyErrorPanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                                <asp:Literal ID="BuyError" runat="server"></asp:Literal>
                            </asp:Panel>

                            <div class="row" style="font-size: 14px">
                                <div class="col-xs-5 col-sm-5 col-md-4 col-lg-4" style="text-align: right">
                                    <b><%=U4200.CREATOR %>:</b>
                                </div>
                                <div class="col-xs-6 col-sm-6 col-md-5 col-lg-5">
                                    <asp:Label ID="CreatorNameLabel" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="row" style="font-size: 14px">
                                <div class="col-xs-5 col-sm-5 col-md-4 col-lg-4" style="text-align: right">
                                    <b><%=String.Format("{0} {1}", U6010.PRICEPER, AppSettings.CryptocurrencyTrading.CryptocurrencyCode) %>:</b>
                                </div>
                                <div class="col-xs-6 col-sm-6 col-md-5 col-lg-5">
                                    <asp:Label ID="PricePerCurrencyLabel" runat="server"></asp:Label>
                                </div>
                            </div>

                            <br />
                            <br />
                            <div class="row">
                                <div class="col-xs-5 col-sm-5 col-md-4 col-lg-4" style="text-align: right">
                                    <b><%=U6010.CCMINOFFERVALUE%>:</b>
                                </div>
                                <div class="col-xs-6 col-sm-6 col-md-5 col-lg-5">
                                    <asp:Label ID="MinOfferLabel" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-5 col-sm-5 col-md-4 col-lg-4" style="text-align: right">
                                    <b><%=U6010.CCMAXOFFERVALUE%>:</b>
                                </div>
                                <div class="col-xs-6 col-sm-6 col-md-5 col-lg-5">
                                    <asp:Label ID="MaxOfferLabel" runat="server"></asp:Label>
                                </div>
                            </div>

                            <br />
                            <div class="row">
                                <div class="col-xs-5 col-sm-5 col-md-4 col-lg-4" style="text-align: right">
                                    <b><%=U6010.CCCURRENCYTOBUY%>:</b>
                                </div>
                                <div class="col-xs-6 col-sm-6 col-md-5 col-lg-5">
                                    <asp:Label ID="CurrencyAvailableToBuyLabel" runat="server"></asp:Label>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-xs-5 col-sm-5 col-md-4 col-lg-4" style="text-align: right">
                                    <b><%=U6010.HAVETOBUYMIN %>:</b>
                                </div>
                                <div class="col-xs-6 col-sm-6 col-md-5 col-lg-5">
                                    <asp:Label ID="MinCurrencyLabel" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-5 col-sm-5 col-md-4 col-lg-4" style="text-align: right">
                                    <b><%=U6010.CANBUYMAX %>:</b>
                                </div>
                                <div class="col-xs-6 col-sm-6 col-md-5 col-lg-5">
                                    <asp:Label ID="MaxCurrencyLabel" runat="server"></asp:Label>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-xs-5 col-sm-5 col-md-4 col-lg-4" style="text-align: right">
                                    <b><%=L1.DESCRIPTION %>:</b>
                                </div>
                                <div class="col-xs-6 col-sm-6 col-md-5 col-lg-5">
                                    <asp:TextBox Style="width: 100%; border-radius: 5px; border-left: solid 3px black; padding: 10px; background-color: lightgray;" ID="testerTB" runat="server" Wrap="true" TextMode="MultiLine" Rows="15" Enabled="false"></asp:TextBox>
                                </div>
                            </div>
                            <hr />

                            <div class="form-horizontal">
                                <div class="row">
                                    <div class="form-group" style="padding-left: 5px">
                                        <div class="col-xs-5 col-sm-5 col-md-4 col-lg-4">
                                            <label class="control-label pull-right">
                                                <%=L1.AMOUNT %>
                                            </label>
                                        </div>
                                        <div class="input-group">
                                            <span class="add-on input-group-addon"><%=AppSettings.CryptocurrencyTrading.CryptocurrencySign %></span>
                                            <asp:TextBox ID="AmountToBuyTextBox" runat="server" CssClass="form-control" Width="200px"></asp:TextBox>
                                        </div>
                                        <div class="col col-xs-offset-5 col-sm-offset-5 col-md-offset-4 col-lg-offset-4">
                                            <asp:RequiredFieldValidator ID="AmountToBuy_RequiredFieldValidator" ControlToValidate="AmountToBuyTextBox" Display="Dynamic" CssClass="text-danger" runat="server" ValidationGroup="ApplyOffer_ValidationGroup"></asp:RequiredFieldValidator>
                                            <asp:RangeValidator ID="AmountToBuy_RangeValidator" runat="server" ValidationGroup="ApplyOffer_ValidationGroup"
                                                ControlToValidate="AmountToBuyTextBox" MinimumValue="0" MaximumValue="999999999" Type="Double" Display="Dynamic" CssClass="text-danger"></asp:RangeValidator>
                                        </div>
                                    </div>
                                </div>
                                <div class="row form-group">
                                    <div class="col-xs-5 col-sm-5 col-md-4 col-lg-4" >
                                        <label class="control-label pull-right">
                                            <%=U6010.FORVALUEOF %>
                                        </label>
                                    </div>
                                    <div class="input-group">
                                        <span class="add-on input-group-addon"><%=AppSettings.Site.CurrencySign %></span>
                                        <asp:TextBox ID="CashToPayLabel" runat="server" CssClass="form-control" Width="200px" Enabled="false"></asp:TextBox>
                                    </div>
                                </div>
                            </div>         
                            <br /><br /><br />
                            <div class="row" style="font-size: 1vw">
                                <div class="col-xs-2 col-sm-2 col-md-1 col-lg-1 col-xs-offset-4 col-sm-offset-4 col-md-offset-4 col-lg-offset-4" style="text-align: center">
                                    <asp:Button ID="BackButton" runat="server" CssClass="btn btn-inverse"
                                        ToolTip='Back'
                                        OnClick="BackToActiveOffers_OnClick" />
                                </div>
                                <div class="col-xs-1 col-sm-1 col-md-1 col-lg-1" style="text-align: center">
                                    <asp:Button ID="BuyOfferButton" runat="server" CssClass="btn btn-inverse"
                                        ToolTip='Buy' ValidationGroup="ApplyOffer_ValidationGroup" CausesValidation="true"
                                        OnClick="BuyOfferButton_OnClick" />
                                </div>
                            </div>
                        </div>
                    </asp:View>
                    <asp:View ID="AddCommentView" runat="server">
                        <div class="TitanViewElement">
                            <div class="row">
                                <div class="col-xs-7 col-sm-7 col-md-4 col-lg-4">
                                    <label class="control-label pull-right">
                                        <%=L1.DESCRIPTION %>
                                    </label>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-5 col-lg-5">
                                    <asp:TextBox ID="AddCommentTextBox" runat="server" TextMode="MultiLine" Rows="15" CssClass="form-control input-h-40" />
                                </div>
                            </div>
                            <br />
                            <br />
                            <div class="row">
                                <div class="col-xs-6 col-sm-6 col-md-6 col-lg-6"></div>
                                <div class="col-xs-6 col-sm-6 col-md-6 col-lg-6">
                                    <asp:Button ID="AddCommentButton" runat="server" CssClass="btn btn-inverse" CausesValidation="true" OnClick="AddCommentButton_OnClick" />
                                </div>
                            </div>
                        </div>
                    </asp:View>
                </asp:MultiView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
