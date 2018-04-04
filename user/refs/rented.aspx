<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="rented.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">

    <script type="text/javascript" src="Scripts/gridview.js"></script>

    <link rel="stylesheet" href="Styles/WatchAds.css" type="text/css" />
    <script type="text/javascript">
        function buyPack(count) {
            $('#__PACKID').val(count);
            $('#<%=Form.ClientID %>').submit();
        }
    </script>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%= L1.RENTED + " " + L1.REFERRALS %></h1>

    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=L1.RENTEDINFO %></p>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <asp:PlaceHolder ID="InfoPlaceHolder" runat="server">
                <div class="alert alert-info fade in m-b-15">
                    <%=L1.REFERRALS %>: 
                    <asp:Label ID="RefCount" runat="server" Font-Bold="true"></asp:Label>/<asp:Label ID="RefLimit" runat="server"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:Panel ID="ErrorMessagePanel2" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                <asp:Literal ID="ErrorMessage2" runat="server"></asp:Literal>
            </asp:Panel>

            <asp:Panel ID="SuccMessagePanel2" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
                <asp:Literal ID="SuccMessage2" runat="server"></asp:Literal>
            </asp:Panel>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <div class="TitanViewPage">
                    <h3>
                        <asp:Label ID="MenuTitleLabel" runat="server"></asp:Label></h3>
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
            <asp:View runat="server" ID="asdasd">
                <div class="TitanViewElement">
                    <%-- SUBPAGE START --%>
                    <br />

                    <asp:Panel ID="WarningPanel" runat="server" Visible="false" CssClass="alert alert-warning fade in m-b-15">
                        <asp:Literal ID="WarningLiteral" runat="server"></asp:Literal>
                    </asp:Panel>

                    <div class="table-responsive">
                        <asp:GridView ID="DirectRefsGridView" runat="server" AllowPaging="False" AllowSorting="True" AutoGenerateColumns="False"
                            DataSourceID="SqlDataSource1" OnRowDataBound="DirectRefsGridView_RowDataBound" 
                            DataKeyNames="RefId" OnRowCommand="DirectRefsGridView_RowCommand">
                            <Columns>
                                <asp:TemplateField HeaderText="Select" HeaderStyle-Width="26px">
                                    <ItemTemplate>
                                        <input type="checkbox" runat="server" id="chkSelect" class="regular-checkbox mini-checkbox" />
                                    </ItemTemplate>
                                    <HeaderTemplate>
                                        <input type="checkbox" id="checkAll" class="regular-checkbox mini-checkbox" onclick="<%=this.jsSelectAllCode %>" /><label for="checkAll"></label>
                                    </HeaderTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="RefId" HeaderText='<%$ ResourceLookup : ID %>' SortExpression="RefId" Visible="true" InsertVisible="False" ReadOnly="True"></asp:BoundField>


                                <asp:TemplateField HeaderText="<%$ ResourceLookup : STATS %>" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone">
                                    <ItemTemplate>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="ReferralSince" HeaderText='<%$ ResourceLookup : REFERRALSINCE %>' SortExpression="ReferralSince" DataFormatString="{0:d}"></asp:BoundField>
                                <asp:BoundField DataField="ExpireDate" HeaderText='<%$ ResourceLookup : EXPIRESIN %>' SortExpression="ExpireDate"></asp:BoundField>

                                <asp:BoundField DataField="LastClick" HeaderText='<%$ ResourceLookup : LASTCLICK %>' SortExpression="LastClick"></asp:BoundField>
                                <asp:BoundField DataField="TotalClicks" HeaderText="<%$ ResourceLookup : CLICKSSTRAIGHT %>" SortExpression="TotalClicks" />
                                <asp:CheckBoxField DataField="HasAutoPay" HeaderText="AutoPay" SortExpression="HasAutoPay" ItemStyle-Width="9px" />
                                <asp:BoundField DataField="PointsEarnedToReferer" HeaderText='PE' SortExpression="PointsEarnedToReferer"></asp:BoundField>

                            </Columns>
                        </asp:GridView>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <div class="small">
                                <%=U4200.LPAINFO %>
                                <br />
                                <%=U4200.PEINFO %>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:PlaceHolder ID="AutopayAllPlaceHolder" runat="server">
                                <div class="row m-t-10 m-b-20">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <%=U6011.ALLREFERRALS %>:
                                        </div>
                                    </div>
                                    <div class="col-lg-12 m-t-10 col-md-12">
                                        <asp:Button ID="AutopayAllOnButton" runat="server" CssClass="btn btn-inverse pull-left m-b-15 m-r-15 p-l-40 p-r-40" OnClick="AutopayAllOnButton_Click" />                                    
                                        <asp:Button ID="AutopayAllOffButton" runat="server" CssClass="btn btn-inverse pull-left m-b-15 m-r-15 p-l-40 p-r-40" OnClick="AutopayAllOffButton_Click" />
                                    </div>
                                </div>
                            </asp:PlaceHolder>                            
                            
                            <asp:Panel ID="SelectedPanel" runat="server" CssClass="displaynone m-t-15">
                                <p><%=L1.SELECTED %>:</p> 
                                <div class="row">                                    
                                    <div class="col-lg-12 m-t-10 col-md-12">
                                        <asp:Button ID="RemoveButton" runat="server" CssClass="btn btn-danger pull-left m-b-15 m-r-15 p-l-40 p-r-40" OnClick="RemoveButton_Click" />                                   
                                        <asp:Button ID="RecycleButton" runat="server" CssClass="btn btn-warning pull-left m-b-15 p-l-40 p-r-40" OnClick="RecycleButton_Click" />
                                    </div>
                                    </div>
                                <div class="row m-b-20">
                                    <div class="col-lg-12 m-t-10 col-md-12">
                                        <asp:Button ID="AutopayOnButton" runat="server" CssClass="btn btn-inverse pull-left m-b-15 m-r-15 p-l-40 p-r-40" OnClick="AutopayOnButton_Click" />                                    
                                        <asp:Button ID="AutopayOffButton" runat="server" CssClass="btn btn-inverse pull-left m-b-15 m-r-15 p-l-40 p-r-40" OnClick="AutopayOffButton_Click" />
                                    </div>
                                </div>
                                <hr />
                                <div class="row m-t-30 m-b-30">
                                    <div class="col-sm-6 col-md-6 col-lg-4 m-t-10">
                                        <asp:DropDownList runat="server" ID="RenewDropDownList" OnInit="RenewDropDownList_Init" class="form-control"></asp:DropDownList>
                                    </div>
                                    <div class="col-sm-6 col-md-3 col-lg-2 m-t-10">
                                        <asp:Button ID="RenewButton" runat="server" CssClass="btn btn-info btn-block" OnClick="RenewButton_Click" />
                                    </div>
                                </div>
                                
                                
                            </asp:Panel>
                        </div>
                    </div>
                                

                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>
            <asp:View ID="View1" runat="server">
                <div class="TitanViewElement">
                    <%-- SUBPAGE START --%>
                    <input type="hidden" name="__PACKID" id="__PACKID" value="AA.KZ" />
                    <br />
                    <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="ui-state-error">
                        <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                    </asp:Panel>

                    <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="greenbox">
                        <asp:Literal ID="SuccMessage" runat="server"></asp:Literal>
                    </asp:Panel>
                    <br />
                    <table style="font-size: 11px; padding-left: 8px;">
                        <tr>
                            <td style="width: 170px;"><%=Resources.L1.RENTALTIME %> :
                            </td>
                            <td>
                                <asp:Label ID="RentalTimeLabel" runat="server"></asp:Label>
                                <%=Resources.L1.DAYS %>
                            </td>
                        </tr>
                        <tr>
                            <td><%=Resources.L1.RENTALGUARANTEE %>:
                            </td>
                            <td>
                                <asp:Label ID="RefGauranteeLabel" runat="server"></asp:Label>
                            </td>
                        </tr>


                    </table>
                    <br />
                    <h4><%=Resources.L1.REQUIREMENTS %></h4>
                    <table style="font-size: 11px; padding-left: 8px;">
                        <tr>
                            <td style="width: 170px;"><%=Resources.L1.MINCLICKS %> :
                            </td>
                            <td>
                                <asp:Label ID="MinClicksLabel" runat="server"></asp:Label>
                                (<%=Resources.L1.YOUHAVE %> <asp:Literal ID="TotalClicksLiteral" runat="server"></asp:Literal>)
                            </td>
                        </tr>
                        <tr>
                            <td><%=Resources.L1.LASTRENTED %>:
                            </td>
                            <td>
                                <asp:Literal ID="LastRentedLiteral" runat="server"></asp:Literal>
                            </td>
                        </tr>
                        <tr>
                            <td><%=Resources.L1.MINRENTINTERVAL %>:
                            </td>
                            <td>
                                <asp:Label ID="MinRentInterval" runat="server"></asp:Label>
                                <%=Resources.L1.DAYS %>
                            </td>
                        </tr>
                        <tr>
                            <td><%=Resources.U2502.MAXREFPACKAGE %>:
                            </td>
                            <td>
                                <asp:Label ID="MaxRefPack" runat="server"></asp:Label>

                            </td>
                        </tr>

                    </table>
                    <br />
                    <br />
                    <div class="row">
                        <div class="col-md-12">
                            <div class="row">
                                <asp:Literal ID="RefBoxesLiteral" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </div>

                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>
        </asp:MultiView>


        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" SelectCommand="SELECT RefId, ReferralSince, HasAutoPay,ExpireDate, LastClick, TotalClicks, ClicksStats, LastPointableActivity, PointsEarnedToReferer FROM [RentedReferrals] WHERE ([OwnerUsername] = @OwnerUsername) ORDER BY [ReferralSince]">
            <SelectParameters>
                <asp:ControlParameter ControlID="UserName" Name="OwnerUsername" PropertyName="Text" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:Label ID="UserName" runat="server" Visible="false"></asp:Label>

    </div>


</asp:Content>
