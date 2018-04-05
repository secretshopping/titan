<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ticket.aspx.cs" Inherits="sites_ticket" MasterPageFile="~/Sites.master" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <div class="content" data-scrollview="true">
        <div class="container" data-animation="true" data-animation-type="fadeInDown">

            <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger">
                <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
            </asp:Panel>

            <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="alert alert-success">
                <asp:Literal ID="SuccMessage" runat="server"></asp:Literal>
            </asp:Panel>

            <div class="row">
                <div class="col-md-12">
                    <table id="SupportTicketTable" class="supportTicket" cellspacing="5">
                        <asp:Literal ID="TicketLiteral" runat="server"></asp:Literal>
                    </table>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-md-8 col-md-offset-2">
                    <div class="form-group">
                        <asp:TextBox ID="SupportTicketTextBox" runat="server" TextMode="MultiLine" Rows="5" MaxLength="5000" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <div class="col-md-3">
                            <asp:Button ID="SupportTicketButton" runat="server" Text="Send" OnClick="SupportTicketButton_Click" CssClass="btn btn-primary btn-block" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
