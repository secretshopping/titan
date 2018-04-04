<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CPAOffer.ascx.cs" Inherits="Controls_CPAOffer" %>

<%--

    This is template for CPA/GPT Offer. Feel free to modify it, but do NOT change:
    1. Any data between <% %> tags
    2. Any ASP.NET tags (e.g. <asp:PlaceHolder></asp:PlaceHolder>)

--%>

<li class="cpa-item p-t-30 p-b-30 row" data-offer-level="<%=Object.OfferLevel %>">
    <div class="offer-image col-md-3" style="background-image: url(<%=OfferImageHTML %>);">
    </div>
    <div class="col-md-9" runat="server" id="amountDiv">
        <h3 class="text-right"><%=Amount %></h3>
    </div>
    <div class="col-md-7">
        <div class="offer-info">
            <div class="row">
                <div class="col-md-12">
                    <h4 class="title"><a href="<%=Object.GetTargetURL() %>" target="_blank"><%=Object.Title %></a></h4>
                    <p class="desc"><%=Mailer.ReplaceNewLines(Object.Description) %></p>
                </div>
            </div>
            <div class="row">
                <div class="<%=OfferInfoTableClass %>">
                    <table class="cpa-table table table-condensed table-borderless">
                        <tr runat="server" id="DateAddTr">
                            <td class="f-w-700"><%=L1.DATEADDED %></td>
                            <td><%=Object.DateAdded.ToString() %></td>
                        </tr>
                        <tr runat="server" id="CategoryTr">
                            <td class="f-w-700"><%=L1.CATEGORY  %></td>
                            <td><%=Prem.PTC.Offers.CPAType.GetText(Object.Category) %></td>
                        </tr>
                        <tr runat="server" id="amountTr">
                            <td class="f-w-700"><%=L1.AMOUNT %></td>
                            <td><%=Amount %></td>
                        </tr>
                        <asp:PlaceHolder ID="RatingAndCreditingTimePlaceHolder" runat="server">
                            <tr runat="server" id="OfferRatingTr">
                                <td class="f-w-700"><%=L1.OFFERRATING  %></td>
                                <td>
                                    <img src="Images/CPAOffers/Stars/<%=Object.Rating %>.png" />
                                    <%=L1.COMPLETEDXTIMES.Replace("%n%", Object.CompletedTimes.ToString()) %></td>
                            </tr>
                            <tr>
                                <td class="f-w-700"><%=L1.CREDITINGTIME %></td>
                                <td>AVG: <%=Object.AVGCreditingTime + ", " + L1.LASTCREDITED + ": " + Object.GetLastCredited() %></td>
                            </tr>
                        </asp:PlaceHolder>
                    </table>
                </div>
                <asp:PlaceHolder ID="FieldsGroupPlaceHolder" runat="server">
                    <div class="col-md-6">
                        <div id="boxes<%=Object.Id %>">
                            <%--Show Login ID field? --%>
                            <asp:PlaceHolder ID="LoginIDPlaceHolder" runat="server" Visible="false">
                                <div class="form-group">
                                    <label>Login ID:</label>
                                    <asp:TextBox ID="LoginID" CssClass="form-control" runat="server" MaxLength="80" ClientIDMode="Static"></asp:TextBox>
                                    <span class="help-block"><%=L1.LOGINIDHINT %></span>
                                </div>
                            </asp:PlaceHolder>

                            <%--Show Email ID field? --%>
                            <asp:PlaceHolder ID="EmailIDPlaceHolder" runat="server" Visible="false">
                                <div class="form-group">
                                    <label>Email:</label>
                                    <asp:TextBox ID="EmailID" CssClass="form-control" Rows="3" runat="server" MaxLength="400" ClientIDMode="Static" TextMode="MultiLine"></asp:TextBox>
                                    <span class="help-block"><%=L1.OFFEMAILHINT %> </span>
                                </div>
                            </asp:PlaceHolder>

                        </div>

                        <%=Text %>
                    </div>
                </asp:PlaceHolder>
            </div>

            <div class="row">
                <div class="col-md-12">
                    <%--Show buttons? --%>

                    <asp:PlaceHolder ID="ButtonsPlaceHolder" runat="server" Visible="false">

                        <div id="report<%=Object.Id %>" style="display: none;">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="form-group">
                                        <asp:TextBox ID="ReportMessage" MaxLength="600" CssClass="form-control" ClientIDMode="Static" TextMode="MultiLine" Rows="3" runat="server"></asp:TextBox>
                                    </div>
                                    <div id="reportbutton<%=Object.Id %>" style="display: none;">
                                        <div class="form-group">
                                            <div class="row">
                                                <div class="col-md-3 col-sm-6 col-xs-6">
                                                    <asp:Button ID="MakeReportButton" CssClass="btn btn-inverse btn-sm btn-block" runat="server" />
                                                </div>
                                                <div class="col-md-3 col-sm-6 col-xs-6">
                                                    <asp:Button ID="CancelReportButton" CssClass="btn btn-default btn-sm btn-block" runat="server" />
                                                </div>
                                            </div>


                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div id="buttons<%=Object.Id %>">
                            <div class="row">
                                <div class="col-md-3 col-sm-6 col-xs-6">
                                    <asp:Button ID="ReportButton" CssClass="btn btn-inverse btn-sm btn-block" runat="server" />
                                </div>
                                <div class="col-md-3 col-sm-6 col-xs-6">
                                    <asp:Button ID="IgnoreButton" CssClass="btn btn-inverse btn-sm btn-block" runat="server" />
                                </div>
                            </div>
                        </div>

                    </asp:PlaceHolder>

                    <asp:Button ID="ReturnButton" CssClass="btn btn-inverse" runat="server" Visible="false" />
                </div>
            </div>
        </div>
    </div>
    <div class="offer-price col-md-2">
        <asp:LinkButton ID="GoToOfferButton" CssClass="btn btn-inverse btn-lg btn-block" runat="server" />
        <asp:Button ID="SubmitButton" CssClass="btn btn-inverse btn-lg btn-block" runat="server" />
    </div>
</li>
