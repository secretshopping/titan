<%@ Control Language="C#" AutoEventWireup="true" CodeFile="InvestmentCalculator.ascx.cs" Inherits="Controls_Misc_InvestmentCalculator" %>
<link href="Scripts/default/assets/plugins/bootstrap.datepicker/css/bootstrap-datepicker.css" rel="stylesheet" />

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
                        <div class="form-horizontal">
                            <div class="form-group">
                                <label class="control-label col-md-2">
                                    <%=U6008.INVESTMENTPLAN %>
                                </label>
                                <div class="col-md-3">
                                    <asp:DropDownList ID="InvestmentPlanDropDownList" runat="server" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-md-3 col-md-offset-2">
                                    <asp:Button ID="CalculateButton" runat="server" OnClick="CalculateButton_Click" ValidationGroup="CalculateGroup" CssClass="btn btn-inverse" />
                                </div>
                            </div>

                        </div>
                    </div>
                    <div class="col-md-6">
                        <asp:PlaceHolder ID="ResultPlaceHolder" runat="server" Visible="false">
                            <table class="table table-hover table-striped">
                                <thead>
                                    <tr>
                                        <th>
                                            <%=L1.DURATION %>
                                        </th>
                                        <th>
                                            <%=U4000.EARNINGS %> 
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>
                                            <%=L1.DAILY %>
                                        </td>
                                        <td>
                                            <asp:Literal runat="server" ID="DailyEarningsLiteral" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <%=U6007.WEEKLY %>
                                        </td>
                                        <td>
                                            <asp:Literal runat="server" ID="WeeklyEarningsLiteral" />
                                        </td>
                                    </tr>
                                    <tr runat="server" id="MonthlyTab">
                                        <td>
                                            <%=U6008.MONTHLY %>
                                        </td>
                                        <td>
                                            <asp:Literal runat="server" ID="MonthlyEarningsLiteral" />
                                        </td>
                                    </tr>
                                    <tr runat="server" id="HalfYearTab">
                                        <td>
                                            6 <%=U6008.MONTHS %>
                                        </td>
                                        <td>
                                            <asp:Literal runat="server" ID="HalfYearlyEarningsLiteral" />
                                        </td>
                                    </tr>
                                    <tr runat="server" id="TotalTab">
                                        <td>
                                            <%=U5001.TOTAL %>
                                        </td>
                                        <td>
                                            <asp:Literal runat="server" ID="TotalEarningsLitereal" />
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
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
