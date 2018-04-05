<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProcessorSettings.ascx.cs" Inherits="Controls_ProcessorSettings" %>

<div runat="server" class="form-group">
    <div class="row">
    <div class="col-sm-12 col-md-3 col-lg-1">
        <label class="control-label pull-right">
            <img src="<%=ImageURL%>" class="imagemiddle " />
        </label>
    </div>
    <div class="col-sm-12 col-md-9 col-lg-11">
        <asp:TextBox ID="ValueTextBox" runat="server" CssClass="form-control input-h-40"></asp:TextBox>
        <asp:RegularExpressionValidator ID="REValidator" runat="server" ControlToValidate="ValueTextBox" Display="Dynamic" ValidationGroup="SavePaymentPropertiesValidationGroup"
            ValidationExpression="[a-zA-Z0-9.,!@#$%^&*()+-/?\\|:=_]{1,200}" CssClass="text-danger" />
        <p class="help-block" runat="server" id="Processor_Helper" />
    </div>
        </div>

</div>
