<%@ Page Language="C#" AutoEventWireup="true" CodeFile="slotMachine.aspx.cs" Inherits="user_entertainment_slotMachine" MasterPageFile="~/User.master" %>

<asp:Content runat="server" ContentPlaceHolderID="PageMainContent">
    
    <h1 class="page-header">Spin for Points</h1>

        <div class="row">
            <div class="col-md-12">
                <p class="lead">Spin the STACKER SLOT MACHINE to earn redeemable points!</p>
            </div>
        </div>
    <titan:SlotMachine runat="server" />
</asp:Content>
