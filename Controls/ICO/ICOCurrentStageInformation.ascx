<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ICOCurrentStageInformation.ascx.cs" Inherits="Controls_ICO_ICOStageInformation" %>

<style>
    table.ico-stage-table {
        margin: 15px 0;
    }

        table.ico-stage-table tr td {
            padding: 2px 10px;
            text-align: left;
            font-size: 13px;
        }

            table.ico-stage-table tr td:last-child {
                font-weight: 600;
            }
</style>

<asp:PlaceHolder ID="NoStagePlaceHolder" runat="server">
    <p class="alert alert-warning">
        <asp:Literal ID="NoStageLiteral" runat="server" />. 
        <asp:Literal ID="NextStageLiteral" runat="server" />
    </p>
</asp:PlaceHolder>

<asp:PlaceHolder ID="StagePlaceHolder" runat="server">
    <div class="row">
        <div class="col-md-12">
            <p class="alert alert-success">
                <asp:Literal ID="NameTextBox" runat="server" />
            </p>
            <div class="row">
                <div class="col-md-12">
                    <div class="progress">
                        <asp:Literal ID="ProgressBarLiteral" runat="server" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:PlaceHolder>
