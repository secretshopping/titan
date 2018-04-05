<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="surfads.aspx.cs" Inherits="Page_advert_SurfAds" %>

<%@ MasterType VirtualPath="~/User.master" %>


<asp:Content ContentPlaceHolderID="PageHeadContent" runat="server">
    <link rel="stylesheet" href="Styles/WatchAds.css" type="text/css" />
   
    <script type="text/javascript" src="Scripts/gridview.js"></script>

    <script type="text/javascript">
        function TBChanged() {
            //Get values
            var packPrice = parseFloat($("#<%=PackPriceLabel.ClientID%>").text());
            
            $("#DisplaySpanDollars").text(packPrice.toFixed(4));

        }

        function CheckURL() {
            $('#__EVENTARGUMENT5').val($('#<%=URL.ClientID %>').val()); //Set URL to validate

            //Send request
            $('#<%=Form.ClientID%>').attr('action', 'user/earn/surf.aspx?f=3');
            $('#<%=Form.ClientID%>').attr('target', '_blank');
            $('#<%=Form.ClientID%>').submit();
            $('#<%=Form.ClientID%>').attr('action', 'user/advert/surfads.aspx');
            $('#<%=Form.ClientID%>').attr('target', '_self');

            return false;
        }

        $(document).ready(function () {
            TBChanged();
        });

        function pageLoad() {
           // jQuery.noConflict();

            $('.selectableCheckbox input').on('click', function () {
                $('#<%=NewSelectedPanel.ClientID%>').show();
                hideIfUnchecked('<%=NewSelectedPanel.ClientID%>');
                $(this).prop('checked', $('#checkAll').checked);
            });

            $('.allSelectableCheckbox').on('click', function () {
                $('.selectableCheckbox input').prop('checked', this.checked);
                hideIfUnchecked('<%=NewSelectedPanel.ClientID%>');
                uncheckInvisible();
            });

        }
    </script>
</asp:Content>


<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <h1 class="page-header"><%=U5004.SURFADS %></h1>
    
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=U5004.SURFADDESCRIPTION.Replace("%n%", AppSettings.RevShare.AdPack.AdPackNamePlural) %></p>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
           <p class="text-danger m-t-15 f-w-700 f-s-14">
                <%=U6011.SURFADDETAILEDDESCRIPTION %>
           </p>
        </div>
    </div>

    <input type="hidden" name="__EVENTARGUMENT5" id="__EVENTARGUMENT5" value="" />
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="MultiViewUpdatePanel">
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="MultiViewUpdatePanel" runat="server" UpdateMode="Conditional">         
        <Triggers>    
            <asp:AsyncPostBackTrigger ControlID="Button3" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="Button2" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="Button1" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="CreateAdButton" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="RedirectToNewAdsButton" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="PurchaseButton" EventName="Click" />
        </Triggers>

        <ContentTemplate>

            <div class="row">
                <div class="col-md-12">
                    <asp:Panel ID="SPanel" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
                        <asp:Literal ID="SText" runat="server"></asp:Literal>
                    </asp:Panel>

                    <asp:Panel ID="EPanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                        <asp:Literal ID="EText" runat="server"></asp:Literal>
                    </asp:Panel>

                    <asp:Panel ID="WPanel" runat="server" Visible="false" CssClass="alert alert-warning fade in m-b-15">
                        <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                    </asp:Panel>

                    <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="alert alert-warning fade in m-b-15"
                        ValidationGroup="RegisterUserValidationGroup" DisplayMode="List" ForeColor="#6b6b6b" />

                    <asp:ValidationSummary ID="ValidationSummary3" runat="server" CssClass="alert alert-danger fade in m-b-15"
                        ValidationGroup="LinkCheckerGroup" DisplayMode="List" ForeColor="White" />
                </div>
            </div>

            <asp:Label runat="server" ID="PackROILabel" ClientIDMode="Static" CssClass="displaynone" />
            <asp:Label runat="server" ID="PackPriceLabel" ClientIDMode="Static" CssClass="displaynone" />

            <div class="row">
                <div class="col-md-12">
                    <div class="nav nav-tabs custom text-right">
                        <div class="TitanViewPage">       
                            <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                                <asp:Button ID="Button3" runat="server" OnClick="MenuButton_Click" CommandArgument="2" />
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
                                <%-- SUBPAGE START --%>                                   
                                <asp:PlaceHolder runat="server" ID="ChangeAdvertInfoPlaceholder">
                                    <p><%=U5001.YOUCANCHANGECAMPAIGN.Replace("%n%", U5004.SURFADS) %></p>
                                </asp:PlaceHolder>
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <label class="control-label col-md-2"><%=L1.PRICE %>:</label>
                                                <div class="col-md-6">
                                                    <span class="form-control no-border"><b><%=Prem.PTC.AppSettings.Site.CurrencySign %><span id="DisplaySpanDollars">?</span></b></span>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label class="control-label col-md-2"><%=L1.PACK %>:</label>
                                                <div class="col-md-6">
                                                    <asp:DropDownList ID="PacksDropDown" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="PacksDropDown_SelectedIndexChanged"></asp:DropDownList>    
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label class="control-label col-md-2"><%=L1.CAMPAIGN %>:</label>
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <asp:PlaceHolder runat="server" ID="DropDownAdsPlaceHolder">
                                                            <asp:DropDownList ID="CampaignsDropDown" runat="server" AutoPostBack="true" CssClass="form-control"></asp:DropDownList>
                                                        </asp:PlaceHolder>
                                                        <div class="input-group-btn">    
                                                            <asp:Button runat="server" ID="RedirectToNewAdsButton" OnClick="MenuButton_Click" CommandArgument="1" Text="<%$ ResourceLookup:ADDNEW %>" CssClass="btn btn-primary" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
											 <titan:TargetBalance runat="server" Feature="SurfAd" ID="TargetBalanceRadioButtonList"></titan:TargetBalance>
                                            <div class="form-group">
                                                <div class="col-md-3">
                                                    <asp:Button ID="PurchaseButton" runat="server"
                                                    ValidationGroup="RegisterUserValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="PurchaseButton_Click"
                                                    UseSubmitBehavior="false" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <%-- SUBPAGE END   --%>
                            </div>
                        </div>
                    </div>
                </asp:View>

                <asp:View runat="server" ID="View2">
                    <div class="TitanViewElement">
                        <%-- SUBPAGE START --%>
                        <div class="row">
                            <div class="col-md-12">                        
                                <h2><%=U4200.CREATENEWADS %></h2>
                
                                <p>
                                    <%=U5004.CREATESURFADSAD %>
                                    <asp:Literal runat="server" ID="AdminLiteral" Visible="false"></asp:Literal>
                                    <br />
                                    <asp:Literal runat="server" ID="StartPageDescriptionLiteral"></asp:Literal>
                                </p>

                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <asp:Label ID="TitleLabel" runat="server" AssociatedControlID="Title" CssClass="control-label col-md-2"><%=L1.TITLE %>:</asp:Label>
                                                <div class="col-md-6">
                                                    <%--Max DB Length = 256--%>
                                                    <asp:TextBox ID="Title" runat="server" CssClass="form-control" MaxLength="30"></asp:TextBox>
                                    
                                                    <asp:RegularExpressionValidator ValidationGroup="RegisterUserValidationGroup" ID="RegularExpressionValidator1" runat="server"
                                                ValidationExpression="[^'\n\r\t]{3,30}" Display="Dynamic" CssClass="text-danger" ControlToValidate="Title" Text="">*</asp:RegularExpressionValidator>
                                                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="Title"
                                                        Display="Dynamic" CssClass="text-danger" Text=""
                                                        ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>    
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Description" CssClass="control-label col-md-2"><%=L1.DESCRIPTION %>:</asp:Label>
                                                <div class="col-md-6">
                                                    <%--Max DB Length = 256--%>
                                                    <asp:TextBox ID="Description" runat="server" CssClass="form-control" MaxLength="70"></asp:TextBox>    
                     
                                                    <asp:RegularExpressionValidator runat="server"
                                                    ValidationGroup="RegisterUserValidationGroup" Display="Dynamic" CssClass="text-danger"
                                                    Text="" ID="CorrectEmailRequired" ControlToValidate="Description" ValidationExpression="[^']{3,70}">*</asp:RegularExpressionValidator>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="URL" CssClass="control-label col-md-2">URL:</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:UpdatePanel runat="server" ID="AddNewAdWithURLCheck" OnLoad="AddNewAdWithURLCheck_Load" ClientIDMode="Static">
                                                        <ContentTemplate>
                                                            <div class="input-group">
                                                                <asp:TextBox ID="URL" runat="server" CssClass="form-control" Text="http://" MaxLength="800"></asp:TextBox>
                                                                <div class="input-group-btn">    
                                                                    <asp:LinkButton ID="CheckURLButton" runat="server" OnClientClick="CheckURL()" CssClass="btn btn-primary"><%=U4200.CHECKURLTEXT %></asp:LinkButton>
                                                                </div>
                                                            </div>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                      
                                                    <asp:RegularExpressionValidator Display="Dynamic" CssClass="text-danger" ValidationGroup="RegisterUserValidationGroup" ID="RegularExpressionValidator2" runat="server" ErrorMessage="*"
                                                        ControlToValidate="URL" Text="">*</asp:RegularExpressionValidator>
                                                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="URL"
                                                        Display="Dynamic" CssClass="text-danger"
                                                        ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                            <asp:PlaceHolder runat="server" ID="StartPagePlaceHolder">
                                                <div class="form-group">
                                                    <div class="col-md-6 col-md-offset-2">
                                                        <div class="checkbox">
                                                            <asp:CheckBox runat="server" ID="PurchaseStartPageCheckBox" AutoPostBack="true" CssClass="margin20-fix"/>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <div class="col-md-6 col-md-offset-2">
                                                        <asp:Panel runat="server" ID="StartPageCalendarPanel">
                                                            <h3 class="m-b-20"><%=L1.PRICE%>: <b><%= AppSettings.RevShare.AdPack.StartPagePrice %></b></h3>
                                                            <asp:UpdatePanel runat="server">
                                                                <ContentTemplate>
                                                                    <asp:Calendar ID="StartPageDateCalendar" runat="server" OnDayRender="StartPageDateCalendar_DayRender" CssClass="table table-condensed table-borderless calendar"></asp:Calendar>
                                                                </ContentTemplate>
                                                            </asp:UpdatePanel>
                                                        </asp:Panel>
                                                    </div>
                                                </div>
                                            </asp:PlaceHolder>

                                            <div class="form-group">
                                                <div class="col-md-3">
                                                    <asp:Button ID="CreateAdButton" runat="server"
                                                        ValidationGroup="RegisterUserValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="CreateAdButton_Click"
                                                        UseSubmitBehavior="false" />
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>
                              
                                <div class="row">
                                    <div class="col-md-12">
                                        <h2><%=U4000.ADDEDCAMPAIGNS %></h2>    
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="table-responsive">
                                            <asp:GridView ID="AdPacksAdGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                                                DataSourceID="AdPacksAdGridViewDataSource" OnRowDataBound="AdPacksAdGridView_RowDataBound" PageSize="20">
                                                <Columns>
                                                    <asp:BoundField DataField='Id' HeaderText='Id' SortExpression='Id' ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" />
                                                    <asp:BoundField DataField='CreatorUserId' HeaderText='CreatorUserId' SortExpression='CreatorUserId' ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" />
                                                    <asp:BoundField DataField='Title' HeaderText='Title' SortExpression='Title' />
                                                    <asp:BoundField DataField='Description' HeaderText='Description' SortExpression='Description' />
                                                    <asp:BoundField DataField='AddedAsType' HeaderText='Added as' SortExpression='AddedAsType' />
                                                    <asp:BoundField DataField='Status' HeaderText='Status' SortExpression='Status' />
                                                    <asp:BoundField DataField='TargetUrl' HeaderText='Target Url' SortExpression='TargetUrl' />
                                                </Columns>
                                            </asp:GridView>
                                            <asp:SqlDataSource ID="AdPacksAdGridViewDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="AdPacksAdGridViewDataSource_Init"></asp:SqlDataSource>
                                        </div>    
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </asp:View>

                <asp:View runat="server" ID="View3" OnActivate="View3_Activate">
                    <div class="TitanViewElement">
                        <div class="row">
                            <div class="col-md-12">
                                <h2><%=U5004.SURFADS %> <%=L1.STATISTICS %></h2>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="table-responsive">
                                    <asp:GridView ID="SurfAdsStatsGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" EmptyDataText="<%$ ResourceLookup:NOSTATS %>"
                                        DataSourceID="SurfAdsStatsGridView_DataSource" OnRowDataBound="SurfAdsStatsGridView_RowDataBound" PageSize="20">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Select">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkSelect" runat="server" CssClass="selectableCheckbox" />
                                                </ItemTemplate>
                                                <HeaderTemplate>
                                                    <input type="checkbox" id="checkAll" class="regular-checkbox mini-checkbox allSelectableCheckbox" onclick="<%=this.jsSelectAllCode %>" /><label for="checkAll"></label>
                                                </HeaderTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField='Id' HeaderText='Id' SortExpression='Id' ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" />
                                            <asp:BoundField DataField='TotalClicks' SortExpression='TotalClicks' HeaderText="<%$ ResourceLookup:CLICKS %>" />
                                            <asp:BoundField DataField="Title" HeaderText="<%$ ResourceLookup:CAMPAIGN %>" SortExpression="Title" />
                                            <asp:BoundField DataField="StartPageDate" SortExpression="StartPageDate" />
                                        </Columns>
                                    </asp:GridView>
                                    <asp:SqlDataSource ID="SurfAdsStatsGridView_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="SurfAdsStatsGridView_DataSource_Init"></asp:SqlDataSource>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <asp:Panel ID="NewSelectedPanel" runat="server" CssClass="displaynone">
                                    <div class="form-group">
                                        <label class="col-md-2 control-label"><%=L1.CAMPAIGN %>:</label>
                                        <div class="col-md-6">
                                            <div class="input-group">
                                                <asp:PlaceHolder runat="server" ID="DropDownAdsPlaceHolder2">
                                                    <asp:DropDownList ID="ddlCampaigns2" runat="server" CssClass="form-control"></asp:DropDownList>
                                                </asp:PlaceHolder>
                                                <div class="input-group-btn">    
                                                    <asp:Button runat="server" ID="RedirectToNewAdsButton2" OnClick="MenuButton_Click" CommandArgument="1" Text="<%$ ResourceLookup:ADDNEW %>" CssClass="btn btn-default" />
                                                </div>
                                                <div class="input-group-btn">    
                                                    <asp:Button runat="server" ID="ChangeCampaignButton" OnClick="ChangeCampaignButton_Click" Text="<%$ ResourceLookup:CHANGECAMPAIGN %>" CssClass="btn btn-default" />                                                </div>
                                            </div>            
                                        </div>
                                    </div>
                                </asp:Panel>
                            </div>
                        </div>        
                    </div>
                </asp:View>
            </asp:MultiView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
