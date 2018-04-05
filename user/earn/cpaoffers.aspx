<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="cpaoffers.aspx.cs" Inherits="About" Trace="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script src="Scripts/default/assets/plugins/underscore/underscore.min.js"> </script>
    <script type="text/javascript">
        function ShowReportTable(ElemId) {
            $('#boxes' + ElemId).hide();
            $('#buttons' + ElemId).hide();
            $('#report' + ElemId).show();
            $('#reportbutton' + ElemId).show();
        }

        function MakeReturn(ElemId, Sender) {
            $('#<%=Form.ClientID%>').attr('action', 'user/earn/cpaoffers.aspx?undo=' + ElemId + '&sender=' + Sender);
        }

        function MakeSubmit(ElemId) {
            $('#<%=Form.ClientID%>').attr('action', 'user/earn/cpaoffers.aspx?submit=' + ElemId);
        }

        function MakeIgnore(ElemId) {
            $('#<%=Form.ClientID%>').attr('action', 'user/earn/cpaoffers.aspx?ignore=' + ElemId);
        }

        function MakeReport(ElemId) {
            $('#<%=Form.ClientID%>').attr('action', 'user/earn/cpaoffers.aspx?report=' + ElemId);
        }

        function HideReportTable(ElemId) {
            $('#boxes' + ElemId).show();
            $('#buttons' + ElemId).show();
            $('#report' + ElemId).hide();
            $('#reportbutton' + ElemId).hide();
        }
    </script>

    <asp:PlaceHolder ID="GroupOfferLevelsPlaceHolder" runat="server">
        <script>
            function pageLoad() {
                groupByLevel();

            }

            function groupByLevel() {

                var levels = $('[data-offer-level]').map(function () {
                    return $(this).attr('data-offer-level');
                });


                var uniqueLevels = _.uniq(levels);


                var uniqueLevelsSorted = _.sortBy(uniqueLevels, function (n) { return parseInt(n); });


                $(".result-list").wrapAll('<div class="panel-group" id="accordion"></div>');

                var levelBlockColor = '#fff';
                var levelBlockClass = '';
                var levelBlockSize = 5;

                $(uniqueLevelsSorted).each(function (i, v) {
                    if (i % levelBlockSize == 0) {
                        //levelBlockColor = "#" + ((1 << 24) * Math.random() | 0).toString(16);
                        levelBlockColor = (i % 2 == 0) ? "#8ed65c" : "#2ce0d4";
                        levelBlockClass = "m-t-30";
                    } else {
                        levelBlockClass = "";
                    }
                    $('[data-offer-level="' + v + '"]').wrapAll('<div class="panel panel-default ' + levelBlockClass + '" data-level-order="' + i + '"><div id="accordion-' + v + '" class="panel-collapse collapse"><div class="panel-body"></div>').parent().parent().parent().prepend('<div class="panel-heading" style="background:' + levelBlockColor + ';"><h4 class="panel-title"><a data-toggle="collapse" data-parent="#accordion" href="#accordion-' + v + '"><b>Level ' + v + '</b><i class="fa fa-plus pull-right"></i></a></h4></div>');

                });

                $('.result-list .collapse').on('show.bs.collapse', function () {
                    $(this).parent().find(".fa-plus").removeClass("fa-plus").addClass("fa-minus");
                }).on('hide.bs.collapse', function () {
                    $(this).parent().find(".fa-minus").removeClass("fa-minus").addClass("fa-plus");
                });

            }
        </script>
    </asp:PlaceHolder>

</asp:Content>


<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <input type="hidden" name="__EVENTARGUMENT5" id="__EVENTARGUMENT5" value="" />

    <asp:PlaceHolder ID="StandardPageHeaderPlaceHolder" runat="server">
        <h1 class="page-header"><%=U6013.CPAGPTOFFERS %></h1>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="CustomPageHeaderPlaceHolder" runat="server" Visible="false">
        <h2 class="content-title"><%=U6013.CPAGPTOFFERS %></h2>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="InformationPlaceHolder" runat="server">
        <div class="row">
            <div class="col-md-12">
                <div class="note note-warning">
                    <ul>
                        <li><%=U4200.CPAGPT1 %></li>
                        <li><%=U4200.CPAGPT2 %></li>
                        <li><%=U4200.CPAGPT3 %></li>
                    </ul>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>

    <div class="row p-t-20">
        <div class="col-md-12">
            <div class="result-container">
                <div class="input-group">
                    <asp:TextBox ID="SearchBox1" runat="server" MaxLength="20" CssClass="form-control input-white"></asp:TextBox>
                    <div class="input-group-btn">
                        <asp:Button ID="SearchButton1" OnClick="SearchButton1_Click" CssClass="btn btn-inverse" runat="server" Text="<%$ ResourceLookup : SEARCH %>" />
                    </div>
                </div>
                <asp:RegularExpressionValidator ID="TextLenghtValidator" runat="server" ControlToValidate="SearchBox1"
                    ValidationExpression="[a-zA-Z0-9\.\-\,\!\?\&\:\$\@\(\)\+\=\%\*|\\\t\\\n|\\\r|\\\s]{1,20}"
                    ForeColor="#b70d00">*</asp:RegularExpressionValidator>
            </div>
        </div>
        <div class="col-md-3">
            <div class="form-group">
                <label class="control-label"><%=L1.SORTBY %>:</label>
                <asp:DropDownList ID="SortBy1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="SortBy1_SelectedIndexChanged" CssClass="form-control">
                    <asp:ListItem Value="NONE"></asp:ListItem>

                    <asp:ListItem Value="ARating"></asp:ListItem>
                    <asp:ListItem Value="ALast"></asp:ListItem>
                    <asp:ListItem Value="ADate"></asp:ListItem>
                    <asp:ListItem Value="ATimes"></asp:ListItem>
                    <asp:ListItem Value="AMoney"></asp:ListItem>

                    <asp:ListItem Value="DRating"></asp:ListItem>
                    <asp:ListItem Value="DLast"></asp:ListItem>
                    <asp:ListItem Value="DDate"></asp:ListItem>
                    <asp:ListItem Value="DTimes"></asp:ListItem>
                    <asp:ListItem Value="DMoney"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="col-md-2">
            <div class="form-group">
                <label class="control-label"><%=U5004.SHOW %>:</label>
                <asp:DropDownList ID="PagingDropDownList" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="PagingDropDownList_SelectedIndexChanged">
                    <asp:ListItem Value="20" Text="20" Selected="True"></asp:ListItem>
                    <asp:ListItem Value="50" Text="50"></asp:ListItem>
                    <asp:ListItem Value="100" Text="100"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
            </asp:Panel>

            <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
                <asp:Literal ID="SuccMessage" runat="server"></asp:Literal>
            </asp:Panel>
        </div>
    </div>


    <div class="row">
        <div class="col-md-12">

            <asp:PlaceHolder ID="DeviceTypeSelectionPlaceHolder" runat="server">
                <div class="nav nav-pills custom pull-left">
                    <asp:PlaceHolder ID="DeviceTypeMenuButtonPlaceHolder" runat="server">
                        <asp:Button ID="DesktopDeviceTypeButton" runat="server" OnClick="DeviceTypeMenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                        <asp:Button ID="MobileDeviceTypeButton" runat="server" OnClick="DeviceTypeMenuButton_Click" CommandArgument="1" />
                    </asp:PlaceHolder>
                </div>
            </asp:PlaceHolder>

            <div class="nav nav-pills custom text-right">
                <asp:PlaceHolder ID="FrequencySelectionPlaceHolder" runat="server">
                    <asp:PlaceHolder ID="UpperMenuButtonPlaceHolder" runat="server">
                        <asp:Button ID="OtherCategoriesButton" runat="server" OnClick="UpperMenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                        <asp:Button ID="DailyCategoriesButton" runat="server" OnClick="UpperMenuButton_Click" CommandArgument="1" />
                    </asp:PlaceHolder>
                </asp:PlaceHolder>
            </div>

            <asp:PlaceHolder ID="NavigationPlaceHolder" runat="server">
                <div class="nav nav-tabs custom text-right">
                    <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                        <asp:Button ID="Button7" runat="server" OnClick="MenuButton_Click" CommandArgument="6" />
                        <asp:Button ID="Button3" runat="server" OnClick="MenuButton_Click" CommandArgument="5" />
                        <asp:Button ID="Button4" runat="server" OnClick="MenuButton_Click" CommandArgument="4" />
                        <asp:Button ID="Button5" runat="server" OnClick="MenuButton_Click" CommandArgument="3" />
                        <asp:Button ID="Button6" runat="server" OnClick="MenuButton_Click" CommandArgument="2" />
                        <asp:Button ID="Button2" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                        <asp:Button ID="Button1" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                    </asp:PlaceHolder>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>

    <div class="tab-content">
        <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
            <asp:View runat="server" ID="FirstView">
                <div class="TitanViewElement" id="QSG_CPA_LIST">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="pull-left form-horizontal">
                                <div class="form-group">
                                    <label class="col-md-4 control-label"><%=L1.CATEGORIES %>:</label>
                                    <div class="col-md-8">
                                        <asp:DropDownList ID="CategoriesList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CategoriesList_SelectedIndexChanged" CssClass="form-control" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <ul class="result-list" >
                        <asp:Panel ID="OffersPanel0" runat="server"></asp:Panel>
                    </ul>
                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>

            <asp:View runat="server" ID="View1">
                <div class="TitanViewElement">
                    <ul class="result-list">
                        <asp:Panel ID="OffersPanel1" runat="server"></asp:Panel>
                    </ul>
                    <%--SUBPAGE END  --%>
                </div>
            </asp:View>

            <asp:View runat="server" ID="View2">
                <div class="TitanViewElement">
                    <ul class="result-list">
                        <asp:Panel ID="OffersPanel2" runat="server"></asp:Panel>
                    </ul>
                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>

            <asp:View runat="server" ID="View3">
                <div class="TitanViewElement">
                    <ul class="result-list">
                        <asp:Panel ID="OffersPanel3" runat="server"></asp:Panel>
                    </ul>
                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>

            <asp:View runat="server" ID="View4">
                <div class="TitanViewElement">
                    <ul class="result-list">
                        <asp:Panel ID="OffersPanel4" runat="server"></asp:Panel>
                    </ul>
                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>

            <asp:View runat="server" ID="View5">
                <div class="TitanViewElement">
                    <ul class="result-list">
                        <asp:Panel ID="OffersPanel5" runat="server"></asp:Panel>
                    </ul>
                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>

            <asp:View runat="server" ID="View6">
                <div class="TitanViewElement">
                    <ul class="result-list">
                        <asp:Panel ID="OffersPanel6" runat="server"></asp:Panel>
                    </ul>
                    <%-- SUBPAGE END   --%>
                </div>
            </asp:View>
        </asp:MultiView>

        <ul class="pager">
            <li class="previous">
                <asp:LinkButton ID="PreviousPageButton" runat="server" CssClass="m-b-5 btn btn-default" OnClick="PreviousPageButton_Click">Older</asp:LinkButton></li>
            <li><a class="m-b-5"><%=U6011.PAGE %> <b>
                <asp:Literal ID="CurrentPageLiteral" runat="server" Text="1"></asp:Literal></b></a></li>
            <li class="next">
                <asp:LinkButton ID="NextPageButton" runat="server" CssClass="m-b-5 btn btn-default" OnClick="NextPageButton_Click">Newer</asp:LinkButton></li>
        </ul>


    </div>

</asp:Content>
