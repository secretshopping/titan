<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TargetBalance.ascx.cs" Inherits="Controls_TargetBalance" %>

<div class="form-group">
    <label class="control-label col-md-2"><%=U5009.BALANCE %>: </label>
    <div class="col-md-6">
        <div class="input-group">
            <div class="radio radio-button-list">
                <asp:RadioButtonList runat="server" ID="TargetBalances" RepeatLayout="Flow" OnInit="TargetBalances_Init"></asp:RadioButtonList>
            </div>
        </div>
    </div>
</div>