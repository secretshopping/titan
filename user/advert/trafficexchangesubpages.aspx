<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="trafficexchangesubpages.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script type="text/javascript">
        function CheckURL() {

            var temp = $('#<%=URLLabel.ClientID %>').val() + $('#<%=URLTextBox.ClientID %>').val();
            $('#__EVENTARGUMENT5').val(temp); //Set URL to validate

            //Send request
            $('#<%=Form.ClientID%>').attr('action', 'user/earn/surf.aspx?f=3');
            $('#<%=Form.ClientID%>').attr('target', '_blank');
            $('#<%=Form.ClientID%>').submit();
            $('#<%=Form.ClientID%>').attr('action', 'user/advert/trafficexchangesubpages.aspx');
            $('#<%=Form.ClientID%>').attr('target', '_self');

            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header">Add subpages</h1>

    <input type="hidden" name="__EVENTARGUMENT5" id="__EVENTARGUMENT5" value="" />

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

                    <asp:UpdatePanel runat="server" ID="AddNewAdWithURLCheck" OnLoad="AddNewAdWithURLCheck_Load" ClientIDMode="Static">
                        <ContentTemplate>
                            <div class="form-group">
                                <label class="control-label col-md-2">Subpage:</label>
                                <div class="col-md-6">
                                    <asp:TextBox ClientIDMode="Static" ID="URLTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                
                                <div class="col-md-6 col-md-offset-2">
                                    <div class="input-group">
                                        <asp:TextBox ClientIDMode="Static" ID="URLLabel" runat="server" Text="Label" CssClass="form-control"></asp:TextBox>
                                        <div class="input-group-btn">
                                            <asp:LinkButton ID="CheckURLButton" runat="server" OnClientClick="CheckURL()" CssClass="btn btn-primary"><%=U4200.CHECKURLTEXT %></asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                        
                    <div class="form-group">
                        <div class="col-md-3">
                            <asp:Button ID="CreateAdButton" runat="server" Text="Add"
                                ValidationGroup="RegisterUserValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="CreateAdButton_Click"
                                UseSubmitBehavior="false" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
