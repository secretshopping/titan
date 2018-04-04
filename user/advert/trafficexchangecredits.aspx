<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="trafficexchangecredits.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=L1.BUYCREDITS %></h1>

    <div class="tab-content">
        <div class="row">
            <div class="col-md-12">
                <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger">
                    <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                </asp:Panel>
                <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="alert alert-success">
                    <asp:Literal ID="SuccMessage" runat="server"></asp:Literal>
                </asp:Panel>
                <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="alert alert-danger"
                    ValidationGroup="RegisterUserValidationGroup" DisplayMode="List" />
            </div>
        </div>

        <div class="row">
            <div class="col-md-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <label class="control-label col-md-2"><%=L1.TITLE %>:</label>
                        <div class="col-md-6">
                            <asp:Label ID="TitleLabel" runat="server" Text="Label" CssClass="form-control no-border"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="control-label col-md-2">URL:</label>
                        <div class="col-md-6">
                            <asp:Label ID="URLLabel" runat="server" Text="Label" CssClass="form-control no-border"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="control-label col-md-2"><%=L1.PACK %>:</label>
                        <div class="col-md-6">
                            <asp:DropDownList ID="ddlOptions" CssClass="form-control" runat="server"></asp:DropDownList>
                        </div>
                    </div>
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
    </div>

</asp:Content>
