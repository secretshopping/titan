<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="trafficgrid.aspx.cs" Inherits="About" %>

    <%@ MasterType VirtualPath="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">


    <script type="text/javascript" src="Scripts/gridview.js"></script>

    <script type="text/javascript">
        jQuery(function ($) {
            updatePrice();
        });

        function ManageGeoEvent(thebox, ddlId, trId) {
            if ($(thebox).is(':checked')) {
                $('#' + ddlId).css('background-color', '#fff');
                $('#' + trId).css('display', '');
            }
            else {
                $('#' + ddlId).css('background-color', '#f0f0ef');
                $('#' + trId).css('display', 'none');
                $('#' + trId + ' td span input:checkbox').prop('checked', false);

                if (trId == 'trGeo1') {
                    $('#trGeo2').css('display', 'none');
                    $('#trGeo2 td span input:checkbox').prop('checked', false);
                    $('#trGeo2 td select').css('background-color', '#f0f0ef');
                    $('#trGeo3').css('display', 'none');
                    $('#trGeo3 td span input:checkbox').prop('checked', false);
                    $('#trGeo3 td select').css('background-color', '#f0f0ef');
                }
                if (trId == 'trGeo2') {
                    $('#trGeo3').css('display', 'none');
                    $('#trGeo3 td span input:checkbox').prop('checked', false);
                    $('#trGeo3 td select').css('background-color', '#f0f0ef');
                }
            }
        }

        function updatePrice() {
            //Get values
            var price1 = 0.0;
            var price2 = 0.0;
            var price3 = 0.0;
            var price4 = 0.0;

            var selectStringIndex = $('#<%=ddlOptions.ClientID%> option:selected').text().indexOf('-') + 1;
            price4 = parseFloat($('#<%=ddlOptions.ClientID%> option:selected').text().substring(selectStringIndex).replace('<%=Prem.PTC.AppSettings.Site.CurrencySign%>',''))

            var totalPrice = price1 + price2 + price3 + price4;

            $('#<%=lblPrice.ClientID%>').text(totalPrice);
        }

        function fixCSS() {
            $('#view1h').removeClass('TitanViewElement');
            $('#view1h').addClass('TitanViewElementSimple');
        }

        function CheckURL() {
            $('#__EVENTARGUMENT5').val($('#<%=URL.ClientID %>').val()); //Set URL to validate

            //Send request
            $('#<%=Form.ClientID%>').attr('action', 'user/earn/surf.aspx?f=3');
            $('#<%=Form.ClientID%>').attr('target', '_blank');   
            $('#<%=Form.ClientID%>').submit();
            $('#<%=Form.ClientID%>').attr('action', 'user/advert/trafficgrid.aspx');
            $('#<%=Form.ClientID%>').attr('target', '_self');

            return false;
        }

        function pageLoad() {
            <%=PageScriptGenerator.GetGridViewCode(DirectRefsGridView) %>
        }

    </script>
    <link rel="stylesheet" href="Styles/WatchAds.css" type="text/css" />

</asp:Content>






<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header">Traffic Grid</h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><asp:Literal ID="SubLiteral" runat="server"></asp:Literal></p>
        </div>
    </div>
    
    <input type="hidden" name="__EVENTARGUMENT5" id="__EVENTARGUMENT5" value="" />

    <div class="row">
                <div class="col-md-12">
                    <div class="nav nav-tabs custom text-right">
                        <asp:Panel ID="TitanViewPagePanel" runat="server" CssClass="TitanViewPage">
                            <%--<h3><asp:Label ID="MenuTitleLabel" runat="server"></asp:Label></h3>--%>
                            <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                                <asp:Button ID="Button2" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                                <asp:Button ID="Button1" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                            </asp:PlaceHolder>
                        </asp:Panel>
                    </div>
                </div>
    </div>


    <div class="tab-content">
    <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
        <asp:View runat="server" ID="asdasd">
            <div class="TitanViewElement">
                <%-- SUBPAGE START --%>
                
                <div class="row">
                    
                    <div class="col-md-12">

                            <asp:GridView ID="DirectRefsGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                                DataSourceID="SqlDataSource1" OnPreRender="BaseGridView_PreRender" OnRowDataBound="DirectRefsGridView_RowDataBound"
                                DataKeyNames="PtcAdvertId" EmptyDataText="<%$ ResourceLookup : NOADCAMPAIGNS %>"
                                OnRowCommand="DirectRefsGridView_RowCommand">
                                <Columns>
                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="26px" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone">                                        
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="PtcAdvertId" HeaderText="PtcAdvertId" SortExpression="PtcAdvertId" Visible="true" InsertVisible="False" ReadOnly="True" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone"></asp:BoundField>
                                    <asp:BoundField DataField="PtcCategoryId" HeaderText='PtcCategoryId' SortExpression="PtcCategoryId" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone"></asp:BoundField>
                                    <asp:BoundField DataField="PtcAdvertPackId" HeaderText='PtcAdvertPackId' SortExpression="PtcAdvertPackId" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone"></asp:BoundField>
                                    <asp:BoundField DataField="TargetUrl" HeaderText='URL' SortExpression="TargetUrl" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone"></asp:BoundField>
                                    <asp:BoundField DataField="Title" HeaderText='<%$ ResourceLookup : TITLE %>' SortExpression="Title"></asp:BoundField>
                                    <asp:BoundField DataField="Description" HeaderText='<%$ ResourceLookup : DESC %>' SortExpression="Description" ItemStyle-Width="11px" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="CreatedBy" HeaderText="CreatedBy" SortExpression="CreatedBy" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="CreatorUsername" HeaderText="CreatorUsername" SortExpression="CreatorUsername" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="CreatorEmail" HeaderText="CreatorEmail" SortExpression="CreatorEmail" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="CreationDate" HeaderText='<%$ ResourceLookup : CREATED %>' SortExpression="CreationDate" DataFormatString="{0:d}" />
                                    <asp:BoundField DataField="StartDate" HeaderText='<%$ ResourceLookup : PROGRESS %>' SortExpression="StartDate" />
                                    <asp:BoundField DataField="EndDate" HeaderText="%" SortExpression="EndDate" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="TotalSecsActive" HeaderText='<%$ ResourceLookup : VIEWSBIG %>' SortExpression="TotalSecsActive" />
                                    <asp:BoundField DataField="Clicks" HeaderText="Clicks" SortExpression="Clicks" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="EndValue" HeaderText="EndValue" SortExpression="EndValue" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="EndMode" HeaderText="EndMode" SortExpression="EndMode" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="DisplayTimeSeconds" HeaderText='<%$ ResourceLookup : TIME %>' SortExpression="DisplayTimeSeconds" />
                                    <asp:BoundField DataField="ClickValue" HeaderText="ClickValue" SortExpression="ClickValue" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="DirectReferralClickValue" HeaderText="DirectReferralClickValue" SortExpression="DirectReferralClickValue" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="RentedReferralClickValue" HeaderText="RentedReferralClickValue" SortExpression="RentedReferralClickValue" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />

                                    <asp:BoundField DataField="MinMembershipId" HeaderText="MinMembershipId" SortExpression="MinMembershipId" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="Description" HeaderText='<%$ ResourceLookup : DESC %>' SortExpression="Description" ItemStyle-Width="13px" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:CheckBoxField DataField="IsGeolocated" HeaderText="Geo." SortExpression="IsGeolocated" ItemStyle-Width="13px" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />

                                    <asp:BoundField DataField="Status" HeaderText='<%$ ResourceLookup : STATUS %>' SortExpression="Status" />
                                    <asp:BoundField DataField="StatusLastChangedDate" HeaderText="StatusLastChangedDate" SortExpression="StatusLastChangedDate" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                    <asp:BoundField DataField="Price" HeaderText="Price" ItemStyle-Width="25px" SortExpression="Price" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />

                                    <asp:TemplateField HeaderText="">
                                        <ItemStyle Width="13px" />
                                        <ItemTemplate>
                                            <asp:LinkButton ID="ImageButton1" runat="server"
                                                ToolTip='Start'
                                                CommandName="start"
                                                CommandArgument='<%# Container.DataItemIndex %>'><span class="fa fa-play fa-lg text-success"></span></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="">
                                        <ItemStyle Width="13px" />
                                        <ItemTemplate>
                                            <asp:LinkButton ID="ImageButton2" runat="server"
                                                ToolTip='<%$ ResourceLookup : PAUSE %>'
                                                CommandName="stop"
                                                CommandArgument='<%# Container.DataItemIndex %>'><span class="fa fa-pause fa-lg text-warning"></span></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone">
                                        <ItemStyle Width="13px" />
                                        <ItemTemplate>
                                            <asp:LinkButton ID="ImageButton3" runat="server"
                                                ToolTip='<%$ ResourceLookup : BUYCREDITS %>'
                                                CommandName="add"
                                                CommandArgument='<%# Container.DataItemIndex %>'>
                                                <span class="fa fa-plus fa-lg text-info"></span>
                                            </asp:LinkButton>
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

                    </div>
                </div>
                <%-- SUBPAGE END   --%>
            </div>
        </asp:View>

        <asp:View ID="View1" runat="server">
            <div id="view1h" class="TitanViewElement">
                <%-- SUBPAGE START --%>

                <div class="row">
                    <div class="col-md-12">
                        <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                            <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                        </asp:Panel>

                        <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
                            <asp:Literal ID="SuccMessage" runat="server"></asp:Literal>
                        </asp:Panel>
                        <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="alert alert-danger fade in m-b-15"
                            ValidationGroup="RegisterUserValidationGroup" DisplayMode="List"  />
                    </div>
                </div>
                
                <div class="row">
                    <div class="col-md-12">
                        <div class="form-horizontal">
                            
                            <div class="form-group">
                                <asp:Label ID="TitleLabel" runat="server" AssociatedControlID="Title" CssClass="control-label col-md-2"><%=L1.TITLE %>:</asp:Label>
                                <div class="col-md-6">
                                    <asp:TextBox ID="Title" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>    
                               
                                    <asp:RegularExpressionValidator ValidationGroup="RegisterUserValidationGroup" ID="RegularExpressionValidator1" runat="server"
                                        ValidationExpression="[^']{3,30}" Display="Dynamic" CssClass="text-danger" ControlToValidate="Title" Text="">*</asp:RegularExpressionValidator>
                                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="Title"
                                        Display="Dynamic" CssClass="text-danger" Text=""
                                        ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
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
                                    <asp:RegularExpressionValidator ControlToValidate="URL" Display="Dynamic" CssClass="text-danger" ValidationGroup="RegisterUserValidationGroup" ID="RegularExpressionValidator2" runat="server" ErrorMessage="*"
                                        Text="">*</asp:RegularExpressionValidator>
                                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="URL"
                                        Display="Dynamic" CssClass="text-danger"
                                        ValidationGroup="RegisterUserValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                </div>
                            </div>

                            <%--    Only for outside advertising--%>
                            <asp:PlaceHolder ID="OutEmailPlaceHolder" runat="server" Visible="false">
                                <div class="form-group">
                                    <label class="control-label col-md-2">Email:</label>
                                    <div class="col-md-6">
                                        <asp:TextBox ID="OutEmail" runat="server" CssClass="form-control"></asp:TextBox>
                        
                                        <asp:RegularExpressionValidator ValidationGroup="RegisterUserValidationGroup" ID="OutEmailRegularExpressionValidator" runat="server"
                                           Display="Dynamic" CssClass="text-danger" ValidationExpression="^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$" ControlToValidate="OutEmail" Text="">*</asp:RegularExpressionValidator>

                                        <asp:RequiredFieldValidator ID="OutEmailRequiredFieldValidator" runat="server" ControlToValidate="OutEmail"
                                            Text="" Display="Dynamic" CssClass="text-danger"
                                            ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                            <%--    Only for outside advertising--%>

                            <div class="form-group">
                                <label class="control-label col-md-2"><%=L1.PACK %>:</label>
                                <div class="col-md-6">
                                    <asp:DropDownList ID="ddlOptions" runat="server" CssClass="form-control ddl"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="form-group">
                                <div class="col-md-12">
                                    <h4><%=L1.PRICE %>: <%=Prem.PTC.AppSettings.Site.CurrencySign %><asp:Label ID="lblPrice" runat="server" Text="Label"></asp:Label></h4> 
                                </div>   
                            </div>

                            <titan:TargetBalance runat="server" Feature="TrafficGrid" ID="TargetBalanceRadioButtonList"></titan:TargetBalance>

                            <div class="form-group">
                                <div class="col-md-2">
                                    <asp:Button ID="CreateAdButton" runat="server"
                                            ValidationGroup="RegisterUserValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="CreateAdButton_Click"
                                            UseSubmitBehavior="false" /> 
                                </div>   
                            </div>

                            <asp:Literal ID="PaymentButtons" runat="server" Visible="false"></asp:Literal>

                        </div>
                    </div>
                </div>
                <%-- SUBPAGE END   --%>
            </div>
        </asp:View>
    </asp:MultiView>
    </div>


    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" SelectCommand="SELECT * FROM [TrafficGridAdverts] WHERE ([CreatorUsername] = @CreatorUsername) AND [Status] != 7 ORDER BY [StartDate] DESC">
        <SelectParameters>
            <asp:ControlParameter ControlID="UserName" Name="CreatorUsername" PropertyName="Text" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:Label ID="UserName" runat="server" Visible="false"></asp:Label>



</asp:Content>
