<%@ Page Language="C#" AutoEventWireup="true" CodeFile="minivideo.aspx.cs" Inherits="user_entertainment_minivideo" MasterPageFile="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <style>
        .miniVideoBg:hover .miniVideoDesc {
            display: block !important;
            background: rgba(255,255,255,.9);
        }

        .miniVideoDesc {
            background: rgba(255,255,255,0);
            position: absolute;
            z-index: 999999;
            left: 0;
        }

        .miniVideoMain .form-horizontal .control-label {
            width: auto !important;
        }

        .miniVideoMain .form-horizontal .radio span {
            display: -webkit-box;
        }

        .miniVideoMain .form-horizontal .radio span label:last-child {
            padding-left: 28px !important;
        }

        .miniVideoCard {
            padding: 15px;
            border: 1px solid #f5f5f5;
            border-radius: 5px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="PageMainContent">

    <h1 class="page-header">
        <asp:Literal ID="TitleLiteral" runat="server" />
    </h1>

    <div class="row">
        <div class="col-md-12">
            <p class="lead">
                <asp:Literal ID="SubLiteral" runat="server"></asp:Literal>
            </p>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <asp:Panel ID="TitanViewPagePanel" runat="server" CssClass="TitanViewPage">
                    <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                        <asp:Button ID="YourVideosButton" runat="server" OnClick="MenuButton_Click" CommandArgument="1" OnClientClick="aspnetForm.target ='_self';" />
                        <asp:Button ID="AvaibleVideosButton" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" OnClientClick="aspnetForm.target ='_self';" />
                    </asp:PlaceHolder>
                </asp:Panel>
            </div>
        </div>
    </div>

    <div class="tab-content">
        <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
            <asp:View ID="AvaibleVideosView" runat="server" OnActivate="AvaibleVideosView_Activate">
                <div class="TitanViewElement">

                    <div class="row">
                        <div class="col-md-12">
                            <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger">
                                <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                            </asp:Panel>

                            <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="alert alert-success">
                                <asp:Literal ID="SuccMessage" runat="server"></asp:Literal>
                            </asp:Panel>
                        </div>
                    </div>

                    <asp:PlaceHolder runat="server" ID="SearchPlaceHolder">
                        <div class="row">
                            <div class="col-md-12" runat="server">
                                <div class="form-group" id="Div1" runat="server">
                                    <label class="control-label"><%=L1.SEARCH %>:</label>
                                    <div class="" style="width: 200px;">
                                        <asp:DropDownList ID="SearchCategoryDropDownList" CssClass="form-control" runat="server" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </asp:PlaceHolder>

                    <div class="row">
                        <div class="col-md-12">
                            <div id="NoVideosPanelWrapper" visible="false" runat="server" class="row m-t-15">
                                <div class="col-md-12">
                                    <p class="alert alert-danger">
                                        <asp:Literal ID="NoVideosPanel" Visible="false" runat="server" />
                                    </p>
                                </div>
                            </div>
                            <div class="row">
                                <asp:PlaceHolder ID="AvaibleVideosPlaceHolder" runat="server"></asp:PlaceHolder>
                            </div>
                        </div>
                    </div>
                    
                </div>
            </asp:View>

            <asp:View runat="server" ID="YourVideosView">
                <div class="TitanViewElementSimple">
                    <div class="row">
                        <div class="col-md-12">
                            <asp:Panel ID="ErrorMessagePanel2" runat="server" Visible="false" CssClass="alert alert-danger">
                                <asp:Literal ID="ErrorMessage2" runat="server"></asp:Literal>
                            </asp:Panel>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <div class="table-responsive">
                                <asp:GridView ID="UsersBoughtMiniVideosGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                                    DataSourceID="SqlDataSource1" OnRowDataBound="UsersBoughtMiniVideosGridView_RowDataBound" DataKeyNames="Id"
                                    OnRowCommand="UsersBoughtMiniVideosGridView_RowCommand" EmptyDataText="<%$ ResourceLookup : YOUDONTHAVEANYVIDEOBOUGHT %>">
                                    <Columns>
                                        <asp:BoundField DataField="VideoId" SortExpression="VideoId" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                                        <asp:BoundField DataField="VideoId" HeaderText="<%$ ResourceLookup : TITLE %>" SortExpression="VideoId" />
                                        <asp:BoundField DataField="BoughtDate" HeaderText="<%$ ResourceLookup : DATEADDED %>" SortExpression="BoughtDate" />

                                        <asp:TemplateField HeaderText="">
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" CssClass="pull-right"
                                                    ToolTip='<%$ ResourceLookup : GO %>'
                                                    CommandName="goToVideo" OnClientClick="aspnetForm.target ='_blank';"
                                                    CommandArgument='<%# Container.DataItemIndex %>'>
                                                <spin class="fa fa-play fa-lg text-success"></spin>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:View>

        </asp:MultiView>
    </div>

    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" SelectCommand="SELECT * FROM [UsersMiniVideoCampaigns] WHERE ([Username] = @Username) ORDER BY [BoughtDate] DESC">
        <SelectParameters>
            <asp:ControlParameter ControlID="UserName" Name="Username" PropertyName="Text" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:Label ID="UserName" runat="server" Visible="false"></asp:Label>

</asp:Content>