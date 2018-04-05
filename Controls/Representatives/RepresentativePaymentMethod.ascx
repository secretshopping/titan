<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RepresentativePaymentMethod.ascx.cs" Inherits="Controls_Representatives_RepresentativePaymentMethod" %>

<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<div class="form-horizontal">
    <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger">
        <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
    </asp:Panel>

    <div class="form-group">
        <label class="control-label col-md-2"><%=U5004.PAYMENTPROCESSOR %>:</label>
        <div class="col-md-10">
            <asp:TextBox ID="ProcessorNameTextBox" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="ProcessorNameRequiredFieldValidator" runat="server" ControlToValidate="ProcessorNameTextBox" Text=""
                ValidationGroup="RepresentativeValidationGroup" Display="Dynamic" CssClass="text-danger">*</asp:RequiredFieldValidator>
        </div>
    </div>

    <div class="form-group">
        <div class="col-md-6 col-md-offset-2">
            <asp:Image ID="ProcessorLogoImage" runat="server" CssClass="img-responsive" />
        </div>
    </div>
    <div class="form-group">
        <label class="control-label col-md-2"><%=U6010.LOGO %>(Max <%=ImagesHelper.RepresentativesPaymentsLogo.LogoImageMaxWidth %>px x <%=ImagesHelper.RepresentativesPaymentsLogo.LogoImageMaxHeight %>px):</label>
        <div class="col-md-6">
            <span class="btn btn-success fileinput-button customButton">
                <i class="fa fa-plus"></i>
                <span><%=U6000.ADDFILE %></span>
                <asp:FileUpload ID="ProcessorLogoImage_Upload" runat="server" />
            </span>
            <asp:Button ID="ProcessorLogoImage_UploadSubmit" Text="<%$ResourceLookup: SUBMIT %>" OnClick="ProcessorLogoImage_UploadSubmit_Click"
                CausesValidation="true" runat="server" ValidationGroup="RepresentativeSubmitValidationGroup" CssClass="btn btn-primary customButton" />
            <asp:CustomValidator ID="PTCImageSubmitValidator" ControlToValidate="ProcessorLogoImage_Upload" Display="Dynamic" CssClass="text-danger"
                OnServerValidate="PTCImageSubmitValidator_ServerValidate" ValidationGroup="RepresentativeSubmitValidationGroup" ValidateEmptyText="true"
                runat="server" />
            <br />
            <asp:CustomValidator ID="ProcessorLogoImageValidator" Display="Dynamic" CssClass="text-danger"
                OnServerValidate="ProcessorLogoImageValidator_ServerValidate" ValidationGroup="RegisterUserValidationGroup" ValidateEmptyText="true"
                runat="server" />
        </div>
    </div>

    <asp:PlaceHolder runat="server" ID="DepositInformationPlaceHolder">
        <div class="form-group">
            <label class="control-label col-md-2"><%=U6010.DEPOSITINFO %>:</label>
            <div class="col-md-10">
                <CKEditor:CKEditorControl ID="DepositInformationTextBox" Height="300px"
                                BasePath="../../Scripts/ckeditor/" runat="server"></CKEditor:CKEditorControl>
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="WithdrawalInformationPlaceHolder">
        <div class="form-group">
            <label class="control-label col-md-2"><%=U6010.WITHDRAWALINFO %>:</label>
            <div class="col-md-10">
                <CKEditor:CKEditorControl ID="WithdrawalInformationTextBox" Height="300px"
                                BasePath="../../Scripts/ckeditor/" runat="server"></CKEditor:CKEditorControl>
            </div>
        </div>
    </asp:PlaceHolder>
</div>
