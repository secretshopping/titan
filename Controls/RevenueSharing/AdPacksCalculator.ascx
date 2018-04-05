<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AdPacksCalculator.ascx.cs" Inherits="Controls_Misc_AdPacksCalculator" %>
<link href="Scripts/default/assets/plugins/bootstrap.datepicker/css/bootstrap-datepicker.css" rel="stylesheet" />

<asp:PlaceHolder ID="CalculatorMainPlaceHolder" runat="server" Visible="false">
    <asp:UpdatePanel runat="server" ID="CalculatorupdatePanel">
        <ContentTemplate>
            <div class="panel panel-inverse" data-sortable-id="ui-widget-1" data-init="true">
                <div class="panel-heading">
                    <h4 class="panel-title">
                        <asp:Literal ID="TitleLiteral" runat="server" /></h4>
                </div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-md-12">
                            <p class="alert alert-danger m-b-10" id="FailureP" runat="server" visible="false">
                                <asp:Label ID="FailureText" runat="server"></asp:Label>
                            </p>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <label class="control-label col-md-2">
                                        <%=AppSettings.RevShare.AdPack.AdPackNamePlural %> <%=L1.TYPE %>:
                                    </label>
                                    <div class="col-md-3">
                                        <asp:DropDownList ID="AdPackDropDownList" runat="server" CssClass="form-control" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="control-label col-md-2">
                                        <%=U6005.COUNT %>:
                                    </label>
                                    <div class="col-md-3">
                                        <asp:TextBox ID="AdPacksCountTextBox" runat="server" MaxLength="4" CssClass="form-control" />
                                        <asp:RequiredFieldValidator ID="CountRequiredValidator" runat="server" Display="Dynamic" CssClass="text-danger"
                                            ControlToValidate="AdPacksCountTextBox" ErrorMessage="Required"
                                            ValidationGroup="CalculateGroup" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="control-label col-md-2"><%=U6007.STARTDATE %>:</label>
                                    <div class="col-md-3">
                                        <input type="text" id="startDateInput" runat="server" class="form-control date" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" CssClass="text-danger"
                                            ControlToValidate="startDateInput" ErrorMessage="Required"
                                            ValidationGroup="CalculateGroup" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-md-3 col-md-offset-2">
                                        <asp:Button ID="CalculateButton" runat="server" OnClick="CalculateButton_Click" ValidationGroup="CalculateGroup" CssClass="btn btn-inverse" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:PlaceHolder ID="ResultPlaceHolder" runat="server" Visible="false">
                                <p>
                                    <asp:Literal ID="FinishDateLiteral" runat="server" />
                                </p>
                                <p>
                                    <asp:Literal ID="EarnsLiteral" runat="server" />
                                </p>
                            </asp:PlaceHolder>
                        </div>
                    </div>

                    <script src="Scripts/default/assets/plugins/bootstrap.datepicker/js/bootstrap-datepicker.js"></script>
                    <script>
                        $(function () {
                            $(".date").datepicker({ autoclose: true });
                        });
                    </script>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:PlaceHolder>
<asp:PlaceHolder ID="ErrorMessagePlaceHolder" runat="server" Visible="false">
    <div class="row">
        <div class="col-md-12">
            <p class="alert alert-danger m-b-10" id="P1" runat="server">
                <asp:Label ID="ErrorMessageLabel" runat="server"></asp:Label>
            </p>
        </div>
    </div>
</asp:PlaceHolder>
