<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SlotMachine.ascx.cs" Inherits="Controls_Misc_SlotMachine" %>

<asp:Panel ID="PageMainContent" runat="server">
  
    <div class="tab-content">
        <div class="row">
            <div class="col-md-12">
                <p class="lead"><%= L1.YOURCHANCES %>:
                    <%=UserChances %>
                </p>
            </div>
        </div>
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
            <div class="col-md-3">
                <asp:Image runat="server" CssClass="img-responsive" ImageUrl="~/Images/Controls/SlotMachine/jackpot.png" />
            </div>
            <div class="col-md-3 m-t-40 p-t-40 m-b-40 p-b-40 text-center">      
                <asp:Panel ID="MachinePanel" runat="server">
                    <asp:Button runat="server" ID="MainButton" OnClick="MainButton_Click" Text="Pull the Lever" CssClass="btn btn-inverse"/>
                </asp:Panel>
            </div>
        </div>
    </div>    

</asp:Panel>