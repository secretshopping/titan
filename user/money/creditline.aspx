<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="creditline.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script type="text/javascript" src="Scripts/gridview.js"></script>

</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=U5006.CREDITLINE %></h1>
    
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=U5006.CREDITLINEDESC %></p>
        </div>
    </div>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="MultiViewUpdatePanel">
        <ProgressTemplate>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="MultiViewUpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            
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

            <div class="row">
                <div class="col-md-12">
                    <div class="nav nav-tabs custom text-right">
                        <asp:Panel ID="TitanViewPagePanel" runat="server" CssClass="TitanViewPage">
                            <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                                <%--<asp:Button ID="Button2" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />--%>
                                <asp:Button ID="Button1" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                            </asp:PlaceHolder>
                        </asp:Panel>
                    </div>
                </div>
            </div>
            
            <div class="tab-content">
            <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
                <asp:View runat="server" ID="View1">
                    <div class="TitanViewElement">
                        
                        <div class="row">
                            <div class="col-md-12">
                                <p class="alert alert-info">
                                    <%=string.Format(U6012.CREDITLINEDESC, AppSettings.CreditLine.Fee) %><br /><asp:Literal ID="RepayBorrowDescriptionLiteral" runat="server"></asp:Literal>
                                </p>    
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-6 col-md-offset-3">
                                <div class="text-center">
                                    <img src="../../Images/Misc/creditLine.png" width="50%" />
                                    <div class="row">
                                        <div class="col-md-6 col-md-offset-3">
                                            <div class="form-group">
                                                <label class="control-label">
                                                    <asp:Literal runat="server" ID="BorrowPayLiteral"></asp:Literal></label>
                                                <asp:TextBox runat="server" ID="RepayBorrowAmountTextBox" CssClass="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="form-group">
                                                <div class="col-md-6 col-md-offset-3">
                                                    <asp:Button runat="server" ID="BorrowButton" OnClick="RepayBorrow" CommandArgument="1" Visible="false" CssClass="btn btn-block btn-inverse"/>
                                                </div>
                                                <div class="col-md-6 col-md-offset-3">
                                                    <asp:Button runat="server" ID="RepayButton" OnClick="RepayBorrow" CommandArgument="2" Visible="false" CssClass="btn btn-block btn-inverse" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <asp:PlaceHolder runat="server" ID="RepayPlaceHolder">
                            <br />
                            <div class="row">
                                <div class="col-md-6 col-md-offset-3">
                                    <table class="table table-striped ">
                                        <tr>
                                            <td>
                                                <b><%=U5007.DEADLINE %></b>
                                            </td>
                                            <td><b><%=L1.AMOUNT %></b></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Literal runat="server" ID="FirstDateLiteral"></asp:Literal></td>
                                            <td>
                                                <asp:Literal runat="server" ID="FirstRepayLiteral"></asp:Literal></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Literal runat="server" ID="SecondDateLiteral"></asp:Literal></td>
                                            <td>
                                                <asp:Literal runat="server" ID="SecondRepayLiteral"></asp:Literal></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Literal runat="server" ID="FinalDateLiteral"></asp:Literal></td>
                                            <td>
                                                <asp:Literal runat="server" ID="FinalRepayLiteral"></asp:Literal></td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </asp:PlaceHolder>

                    </div>
                </asp:View>
                <asp:View ID="View2" runat="server">
                    <div class="TitanViewElement">
                        <%-- SUBPAGE START --%>

                        <%-- SUBPAGE END   --%>
                </asp:View>
            </asp:MultiView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
