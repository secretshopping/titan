<%@ Page Language="C#" AutoEventWireup="true" CodeFile="~/sites/phone.aspx.cs" Inherits="sites_phone" MasterPageFile="~/Sites.master" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <div class="content" data-scrollview="true">
        <div class="container" data-animation="true" data-animation-type="fadeInDown">
            <h2 class="content-title"><%=L1.SMSVERIFICATION %></h2>

            <p class="text-center"><%=L1.SMSINFO %></p>


            <div class="row">
                <div class="col-md-12">
                    <asp:Panel ID="ErrorMessagePanel2" runat="server" Visible="false" CssClass="alert alert-danger">
                        <asp:Literal ID="ErrorMessage2" runat="server"></asp:Literal>
                    </asp:Panel>
                    <asp:Panel ID="SuccMessagePanel2" runat="server" Visible="false" CssClass="alert alert-success">
                        <asp:Literal ID="SuccMessage2" runat="server"></asp:Literal>
                    </asp:Panel>
                </div>
            </div>

            <div class="row">
                <div class="col-md-12">
                    <asp:Panel ID="SMSPanel1" runat="server" class="text-center">
                        <div class="col-md-4 col-md-offset-4">
                            <p class="text-center"><span class="fa fa-mobile fa-5x"></span></p>
                            <div class="form-inline">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon">+</span>
                                        <asp:TextBox ID="CC" runat="server" MaxLength="3" CssClass="form-control" Width="50px"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:TextBox ID="PHONE" runat="server" MaxLength="12" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="m-t-30">
                                    <asp:Button ID="SendSMS" runat="server" CssClass="btn btn-inverse btn-block" OnClick="SendSMS_Click" />
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="SMSPanel2" runat="server" Visible="false" class="text-center">
                        <div class="col-md-4 col-md-offset-4">
                            <p class="text-center"><span class="fa fa-mobile fa-5x"></span></p>
                            <div class="form-group">
                                <label class="control-label"><%=L1.RECPIN %>:</label>
                                <asp:TextBox ID="TheCode" runat="server" CssClass="form-control" Width="100px"></asp:TextBox>
                            </div>
                            <div class="m-t-30">
                                <asp:Button ID="ConfirmTheCode" runat="server" CssClass="btn btn-inverse btn-block" OnClick="ConfirmTheCode_Click" />
                            </div>
                        </div>
                    </asp:Panel>

                </div>
            </div>
        </div>
    </div>
</asp:Content>

