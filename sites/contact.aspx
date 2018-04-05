<%@ Page Language="C#" AutoEventWireup="true" CodeFile="contact.aspx.cs" Inherits="sites_contact" MasterPageFile="~/Sites.master" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <div class="content" data-scrollview="true">
        <div class="container" data-animation="true" data-animation-type="fadeInDown">
            <h2 class="content-title"><%=L1.CONTACT %></h2>

            <asp:PlaceHolder ID="TicketPlaceHolder" runat="server">
                <p class="text-center"><%=L1.CONTACTINFO %></p>
                <div class="row">
                    <div class="col-md-12">
                        <asp:Panel ID="SupportTicketsMenu" runat="server" Visible="false" CssClass="TitanViewPage">
                            <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                                <div class="row">
                                    <div class="col-md-2">
                                        <asp:Button ID="Button2" runat="server" OnClick="MenuButton_Click" CommandArgument="1" CssClass="btn btn-inverse btn-block m-t-15" />
                                    </div>

                                    <div class="col-md-2">
                                        <asp:Button ID="Button1" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="btn btn-inverse btn-block m-t-15 ViewSelected" />
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                        </asp:Panel>
                        <br />
                        <div class="TitanViewElement">
                            <div class="row">
                                <div class="col-md-12">
                                    <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger">
                                        <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                                    </asp:Panel>

                                    <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="alert alert-success">
                                        <asp:Literal ID="SuccMessage" runat="server"></asp:Literal>
                                    </asp:Panel>
                                    <asp:ValidationSummary ID="SendMessageValidationSummary" runat="server" CssClass="alert alert-danger"
                                        ValidationGroup="SendMessageValidationGroup" DisplayMode="List" />
                                </div>
                            </div>

                            <asp:PlaceHolder runat="server" ID="SendTicketPlaceHolder">
                                <div class="row m-t-30">
                                    <div class="col-md-12">
                                        <h3><%=L1.DIRECTCONTACT %></h3>
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <label class="control-label col-md-2">
                                                    Email:
                                                </label>
                                                <div class="col-md-6">
                                                    <asp:Label ID="LabelEmail" runat="server" CssClass="form-control no-border"></asp:Label>
                                                </div>
                                            </div>
                                        </div>

                                        <h3>
                                            <asp:Literal ID="FormLiteral" runat="server"></asp:Literal></h3>
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <label class="control-label col-md-2">
                                                    <asp:Panel ID="Option1SecondPanel" runat="server" Visible="false">
                                                        Email:
                                                    <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                                                        CssClass="text-danger" Display="Dynamic" 
                                                        ValidationGroup="SendMessageValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator runat="server"
                                                            ValidationGroup="SendMessageValidationGroup" CssClass="text-danger"
                                                            Display="Dynamic" Text="" ID="CorrectEmailRequired" ControlToValidate="Email" ValidationExpression="^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$">*</asp:RegularExpressionValidator>
                                                    </asp:Panel>
                                                    <asp:Label ID="FromUsernameLabel" runat="server" Visible="false"></asp:Label>
                                                </label>
                                                <div class="col-md-6">
                                                    <asp:Panel ID="Option1Panel" runat="server" Visible="false">
                                                        <asp:TextBox ID="Email" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </asp:Panel>
                                                    <asp:Panel ID="Option2Panel" runat="server" Visible="false">
                                                        <asp:Image ID="AvatarImage" Width="20px" Height="20px" CssClass="messageavatar" runat="server" />
                                                        <asp:Literal ID="UserNameLiteral" runat="server"></asp:Literal>
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                            <asp:PlaceHolder ID="InsertNamePlaceHolder" runat="server">
                                                <div class="form-group">
                                                    <label class="control-label col-md-2">
                                                        <%=L1.NAME %>:
                                                    </label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox ID="FullNameTextBox" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="InsertPhoneNumberPlaceHolder" runat="server">
                                                <div class="form-group">
                                                    <label class="control-label col-md-2">
                                                        <%=U4200.PHONE %>:
                                                    </label>
                                                    <div class="col-md-6">
                                                        <div class="form-inline row">
                                                            <div class="form-group col-md-4 col-sm-6 col-xs-12" style="margin-left: auto; width: 115px;">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon">+</span>
                                                                    <asp:TextBox ID="CountryCodeTextBox" runat="server" CssClass="form-control m-r-10" MaxLength="5"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="form-group col-md-8 col-sm-6 col-xs-12" style="margin-left: auto">
                                                                <asp:TextBox ID="PhoneNumberTextBox" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="TicketDepartmentsPlaceHolder" runat="server">
                                                <div class="form-group">
                                                    <label class="control-label col-md-2">
                                                        <%=U6008.DEPARTMENT %>:
                                                    </label>
                                                    <div class="col-md-6">
                                                        <div class="row">
                                                            <div class="form-group col-md-4 col-sm-6 col-xs-12" style="margin-left: auto; width: 300px;">
                                                                <div class="input-group">
                                                                    <div class="radio radio-button-list">
                                                                        <asp:RadioButtonList ID="TicketDepartmentsButtonList" runat="server" RepeatLayout="Flow" /> 
                                                                    </div>
                                                                    
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </asp:PlaceHolder>

                                            <div class="form-group">
                                                <label class="control-label col-md-2">
                                                    <%=L1.TEXT %>:
                                                    <asp:RequiredFieldValidator ID="TextRequired" runat="server" ControlToValidate="MessageText"
                                                        CssClass="text-danger" Display="Dynamic"
                                                        ValidationGroup="SendMessageValidationGroup">*</asp:RequiredFieldValidator>
                                                </label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="MessageText" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" MaxLength="5000"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label class="control-label col-md-2">
                                                    <%=L1.VERIFICATION %>: 
                                                </label>
                                                <div class="col-md-6">
                                                    <titan:Captcha runat="server" ID="TitanCaptcha" ValidationGroup="SendMessageValidationGroup" />
                                                    <asp:CustomValidator OnServerValidate="Validate_Captcha" runat="server" ValidationGroup="SendMessageValidationGroup"
                                                        Display="Dynamic" ID="CustomValidator1" EnableClientScript="False" CssClass="text-danger">*</asp:CustomValidator>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <div class="col-md-3">
                                                    <asp:Button ID="SendMessageButton" runat="server"
                                                        ValidationGroup="SendMessageValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="SendMessageButton_Click" />
                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="EmailPlaceHolder" runat="server">
                <p class="text-center"><%=U6005.CONTACTINSTRUCTIONS %></p>
                <p class="text-center">
                    Email: 
                    <asp:Label ID="EmailLabelWhenTicketsDisabled" runat="server" CssClass="form-control no-border"></asp:Label>
                </p>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>
