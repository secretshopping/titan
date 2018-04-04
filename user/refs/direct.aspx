<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="direct.aspx.cs" Inherits="DirectReferrals" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script type="text/javascript" src="Scripts/gridview.js"></script>

    <script type="text/javascript">

        function pageLoad() {
               var columnsToDetach = $(".table td.displaynone, .table th.displaynone");
                columnsToDetach.detach();
            <%=PageScriptGenerator.GetGridViewCode(DirectRefsGridView) %>
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%= L1.DIRECT + " " + L1.REFERRALS %></h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=L1.DIRECTINFO %></p>
        </div>
    </div>

    <asp:UpdatePanel ID="MultiViewUpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="row">
                <div class="col-md-12">
                    <asp:Panel ID="SPanel" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
                        <asp:Literal ID="SText" runat="server"></asp:Literal>
                    </asp:Panel>

                    <asp:Panel ID="EPanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                        <asp:Literal ID="EText" runat="server"></asp:Literal>
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
                        <div class="row">
                            <div class="col-md-12">
                                <asp:Panel ID="WarningPanel" runat="server" Visible="false" CssClass="alert alert-warning fade in m-b-15">
                                    <asp:Literal ID="WarningLiteral" runat="server"></asp:Literal>
                                </asp:Panel>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-12">
                                <h4><asp:Literal ID="ReferrerInfoLiteral" runat="server" /></h4>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-12">
                                <h4><titan:ReferralsCount runat="server"></titan:ReferralsCount></h4>

                                    <asp:Panel ID="ChangeAVGPanel" runat="server">

                                    </asp:Panel>
                                </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                        <asp:GridView ID="DirectRefsGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                                            DataSourceID="DirectRefsGridView_DataSource" OnPreRender="BaseGridView_PreRender" OnRowDataBound="DirectRefsGridView_RowDataBound"
                                            OnSelectedIndexChanging="DirectRefsGridView_SelectedIndexChanging" OnDataBound="DirectRefsGridView_DataBound" PageSize="30">
                                            <Columns>
                                                <asp:BoundField DataField="UserId" SortExpression="UserId" ItemStyle-Width="1px" />
                                                <asp:TemplateField HeaderText="Select">
                                                    <ItemTemplate>
                                                        <input type="checkbox" runat="server" id="chkSelect" class="regular-checkbox mini-checkbox" />
                                                    </ItemTemplate>
                                                    <HeaderTemplate>
                                                        <input type="checkbox" id="checkAll" class="regular-checkbox mini-checkbox" onclick="<%=this.jsSelectAllCode %>" /><label for="checkAll"></label>
                                                    </HeaderTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Username" HeaderText="<%$ ResourceLookup : USERNAME %>" SortExpression="Username" Visible="true">
                                                    <ItemStyle />
                                                </asp:BoundField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <%#Eval("FirstName") %>  <%#Eval("SecondName") %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <%#Eval("PhoneCountryCodeE") %>  <%#Eval("PhoneNumberE") %> 
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Email">
                                                    <ItemTemplate>
                                                        <span title="<%#Eval("Email") %>"><%#Eval("Email") %></span>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="CameFromUrl" HeaderText='<%$ ResourceLookup : CAMEFROMURL %>' SortExpression="CameFromUrl"></asp:BoundField>
                                                <asp:BoundField DataField="ReferralSince" HeaderText='<%$ ResourceLookup : REFERRALSINCE %>' SortExpression="ReferralSince" DataFormatString="{0:d}">
                                                    <ItemStyle />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="LastDRActivity" HeaderText='<%$ ResourceLookup : LASTACTIVITY %>' SortExpression="LastDRActivity"></asp:BoundField>
                                                <asp:TemplateField HeaderText="<%$ ResourceLookup : STATS %>" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone">
                                                    <ItemTemplate>
                                                        
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="TotalPTCClicksToDReferer" SortExpression="TotalPTCClicksToDReferer" />
                                                <asp:TemplateField ItemStyle-CssClass="AVG" />
                                                <asp:CommandField ShowSelectButton="True" HeaderText='<%$ ResourceLookup : CHART %>' SelectImageUrl="~/Images/OneSite/Referrals/chart.png" ButtonType="Image" HeaderStyle-Width="25px" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                                <asp:BoundField DataField="StatsClicks" HeaderText="Stats" SortExpression="StatsClicks" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />                                                
                                                <asp:BoundField DataField="TotalCashLinksToDReferer" HeaderText='<%$ ResourceLookup: CASHLINKS %>' SortExpression="TotalCashLinksToDReferer" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" ></asp:BoundField>
                                                <asp:BoundField DataField="TotalAdPacksToDReferer" SortExpression="TotalAdPacksToDReferer"></asp:BoundField>
                                                <asp:BoundField DataField="TotalPointsEarnedToDReferer" SortExpression="TotalPointsEarnedToDReferer"></asp:BoundField>
                                                <asp:BoundField DataField="TotalEarnedToDReferer" HeaderText='<%$ ResourceLookup: MONEY %>' SortExpression="TotalEarnedToDReferer"></asp:BoundField>
                                                <asp:BoundField DataField="TotalERC20TokensEarnedToDReferer" HeaderText="<%$ ResourceLookup : ICO %>" SortExpression="TotalERC20TokensEarnedToDReferer" />
                                                <asp:BoundField DataField="Username" HeaderText="<%$ ResourceLookup : USERNAME %>" SortExpression="Username" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                                <asp:BoundField DataField="ReferralSince" HeaderText="<%$ ResourceLookup : EXPIRES %>" SortExpression="ReferralSince" />
                                                <asp:BoundField DataField="UpgradeId" HeaderText="<%$ ResourceLookup : MEMBERSHIP %>" SortExpression="UpgradeId" />
                                                <%-- Column for status --%>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                         <%# HtmlCreator.GetColoredStatus((MemberStatus)Convert.ToInt32(Eval("AccountStatusInt"))) %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <%-- Column for Adpacks --%>
                                                <asp:TemplateField ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone">
                                                    <ItemTemplate>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <%-- Column for Referrals --%>
                                                <asp:TemplateField ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone">
                                                    <ItemTemplate> 
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <%-- Column for Amount spent on AdPacks from cash --%>
                                                <asp:TemplateField ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone">
                                                    <ItemTemplate>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <%-- Column for just getting the id of user in current row --%>
                                                <asp:BoundField DataField="UserId" SortExpression="UserId" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />   
                                            </Columns>
                                        </asp:GridView>

                                <asp:PlaceHolder runat="server" ID="AdditionalInfoPlaceHolder">
                                    <p>
                                        <%=U4000.MONEY %> - <%=U5001.TOTALMONEYBYREFERRAL %>
                                    </p>
                                </asp:PlaceHolder>

                                <asp:Panel ID="SelectedPanel" runat="server" CssClass="displaynone">
                                    <%=L1.SELECTED %>: 
                                    <asp:LinkButton ID="BtnDelete" runat="server" OnClick="ImageButton1_Click" ToolTip='<%$ ResourceLookup : REMOVE %>' >
                                        <span class="fa fa-times text-danger"></span>
                                    </asp:LinkButton>
                                </asp:Panel>


                                <asp:SqlDataSource ID="DirectRefsGridView_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="DirectRefsGridView_DataSource_Init"></asp:SqlDataSource>
                                <asp:Label ID="UserName" runat="server" Visible="false"></asp:Label>

                            </div>
                        </div>
                    </div>
                </asp:View>

                <asp:View runat="server" ID="View2" OnActivate="View2_Activate">
                    <div class="TitanViewElement">
                        <div class="row">
                            <div class="col-md-6 col-md-offset-3">
                                <h4 class="text-center"><titan:ReferralsCount runat="server" ID="ReferralsCount2"></titan:ReferralsCount></h4>

                                <div class="well">

                                    <p class="text-center"><span class="fa fa-user fa-5x"></span></p>

                                    <asp:PlaceHolder runat="server" ID="BuyDirectRefPackPlaceHolder">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="col-md-12">
                                                    <asp:DropDownList ID="DirectRefPackDDL" runat="server" CssClass="form-control"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <titan:TargetBalance runat="server" Feature="DirectReferral" ID="TargetBalanceRadioButtonList"></titan:TargetBalance>
                                        </div>
                                        <p class="text-center"><asp:Button runat="server" ID="BuyDirectRefBackButton" OnClick="BuyDirectRefBackButton_Click" CssClass="btn btn-inverse" /></p>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder runat="server" ID="NoDirectRefPacksPlaceHolder">
                                        <p class="text-center"><asp:Literal runat="server" ID="NoDirectRefPacksLiteral"></asp:Literal></p>
                                    </asp:PlaceHolder>
                                </div>

                            </div>
                        </div>
                    </div>
                </asp:View>

            </asp:MultiView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
