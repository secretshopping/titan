<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="giftcards.aspx.cs" Inherits="GiftCards" %>

<%@ Import Namespace="Prem.PTC" %>
<%@ Import Namespace="Resources" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">

    <script type="text/javascript">
        function showPopupList(giftcardId) {
            $('#popup').show();
            $('#cardPanel' + giftcardId).show();
        }

        function hidePopupList(giftcardId) {
            $('#popup').hide();
            $('#cardPanel' + giftcardId).hide();
        }

        function hideList(giftcardId) {
            $('#cardPanel' + giftcardId).hide();
            $('#loadingPanel').show();
        }
    </script>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <h1 class="page-header"><%=U4000.GIFTCARDS %></h1>

    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=U4000.GIFTCARDSINFO.Replace("%p%", AppSettings.PointsName) %></p>
        </div>
    </div>

    <div class="tab-content">
    <div class="row">
        <div class="col-md-12">
            <div class="table-responsive">
            <asp:GridView ID="GiftCardsGridView" runat="server" OnPreRender="BaseGridView_PreRender" AllowSorting="False" AutoGenerateColumns="False" AllowPaging="true" PageSize="10"
                OnRowDataBound="GiftCardsGridView_RowDataBound" OnPageIndexChanging="GiftCardsGridView_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField='Id' HeaderText='Id' SortExpression='Id' ControlStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" ItemStyle-CssClass="displaynone" />

                    <asp:TemplateField HeaderText="<%$ ResourceLookup : IMAGE %>">
                        <ItemTemplate>
                            <asp:Image AlternateText="Image not found"
                                ImageUrl='<%# Eval("ImageUrl") %>' runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField='Name' HeaderText='<%$ ResourceLookup : NAME %>' SortExpression='Name' />
                    <asp:BoundField DataField='Description' HeaderText='<%$ ResourceLookup : DESCRIPTION %>' SortExpression='Description' />

                    <asp:TemplateField HeaderText="#">
                        <ItemTemplate>
                            <asp:Button ID="Button1" runat="server"
                                CssClass="btn btn-success btn-xs"
                                CommandName="getlist"
                                CommandArgument='<%# Container.DataItemIndex %>' />
                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>
            </asp:GridView>
            </div>
        </div>
    </div>
    </div>

</asp:Content>


<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="PageMenuContent">
      <div id="popup" style="display: none;">
        <div class="popupPanel">
            <%--Popup start--%>

                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:PlaceHolder ID="ListPanelsPlaceHolder" runat="server"></asp:PlaceHolder>

                        <asp:Panel ID="CodeSubmitedPanel" runat="server" Visible="false">
                            <asp:Panel ID="SuccPanel" runat="server">
                                <div style="color: #87a131; font-size: larger; margin-bottom: 40px"><%=L1.OP_SUCCESS %></div>
                                <%=U4000.REQUESTSENTINFO %>: <b><%=Prem.PTC.Members.Member.CurrentInCache.Email %></b>
             
                                <a href="user/giftcards.aspx" class="btn btn-danger pull-right" style="color:#fff"><%=U4000.CLOSE %></a>
                            </asp:Panel>
                            <asp:Panel ID="ErrorPanel" runat="server" Visible="false">
                                <div style="color: #c31a1a; font-size: larger; margin-bottom: 40px">
                                    <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                                </div>
                                <a href="user/giftcards.aspx" class="btn btn-danger pull-right" style="color:#fff"><%=U4000.CLOSE %></a>
                            </asp:Panel>
                        </asp:Panel>

                        <div id="loadingPanel" style="display: none;">
                            <asp:UpdateProgress ID="UpdateProgress2" runat="server">
                            </asp:UpdateProgress>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

            <%--Popup end--%>
        </div>
        <div class="black_overlay"></div>
    </div>



    </asp:Content>