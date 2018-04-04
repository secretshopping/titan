<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="testimonials.aspx.cs" Inherits="Testimonials" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script type="text/javascript" src="Scripts/gridview.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    
    <h1 class="page-header"><%=U5008.TESTIMONIALS%></h1>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="MultiViewUpdatePanel">
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="MultiViewUpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

        <div class="row">
            <div class="col-md-12">
                <p class="lead">
                    <%=U5008.LEAVEFEEDBACK %>    
                </p>
            </div>
        </div>
        
        <div class="row">
            <div class="col-md-12">
                <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger">
                    <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                </asp:Panel>

                <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="alert alert-success" ClientIDMode="Static">
                    <asp:Literal ID="SuccMessage" runat="server"></asp:Literal>
                </asp:Panel>
            </div>
        </div>

        <div class="row">
            <div class="col-md-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <label class="control-label col-md-2"><%=L1.TEXT %></label>
                        <div class="col-md-6">
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="BodyTextBox" CssClass="form-control" MaxLength="3000" Rows="5"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="BodyTextBox"
                                CssClass="text-danger"
                                ValidationGroup="NewTestimonialValidationGroup" Text="">*</asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="control-label col-md-2"><%=U5008.SIGNATURE %></label>
                        <div class="col-md-6">
                            <asp:TextBox runat="server" ID="SignatureTextBox" CssClass="form-control" MaxLength="100"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="SignatureTextBox"
                            CssClass="text-danger"
                            ValidationGroup="NewTestimonialValidationGroup" Text="">*</asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-md-3">
                            <asp:Button ID="CreateTestimonialButton" runat="server"
                            ValidationGroup="NewTestimonialValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="CreateTestimonialButton_Click"
                            UseSubmitBehavior="false" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        </ContentTemplate>
   
    </asp:UpdatePanel>
</asp:Content>
