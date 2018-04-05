<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ExternalCpaOffer.ascx.cs" Inherits="Controls_ExternalCpaOffer" %>


<li class="cpa-item p-t-30 p-b-30 row">
    <div class="offer-image col-md-3" style="background-image: url(<%=OfferImageHTML %>); height: 250px !important;">
    </div>

    <div class="col-md-7">
        <div class="offer-info">
            <div class="row">
                <div class="col-md-12">
                    <h4 class="title"><a href="<%=CpaOffer.GetTargetURL() %>" target="_blank"><%=CpaOffer.Title %></a></h4>
                    <p class="desc"><%=Mailer.ReplaceNewLines(CpaOffer.Description) %></p>
                </div>
            </div>
            <div class="row">
                <div class="<%=OfferInfoTableCssClass %>">
                    
                    <table class="cpa-table table table-condensed table-borderless">
                        <tr>
                            <td class="f-w-700"><%=L1.DATEADDED %></td>
                            <td><%=CpaOffer.DateAdded.ToString() %></td>
                        </tr>
                        <tr>
                            <td class="f-w-700"><%=L1.CATEGORY  %></td>
                            <td><%=Prem.PTC.Offers.CPAType.GetText(CpaOffer.Category) %></td>
                        </tr>
                        <tr>
                            <td class="f-w-700"><%=L1.AMOUNT %></td>
                            <td><%=Amount %></td>
                        </tr>
                        <tr>
                            <td class="f-w-700"><%=L1.OFFERRATING  %></td>
                            <td>
                                <img src="<%=AppSettings.Site.Url %>/Images/CPAOffers/Stars/<%=CpaOffer.Rating %>.png" />
                                <%=L1.COMPLETEDXTIMES.Replace("%n%", CpaOffer.CompletedTimes.ToString()) %></td>
                        </tr>
                        <tr>
                            <td class="f-w-700"><%=L1.CREDITINGTIME %></td>
                            <td>AVG: <%=CpaOffer.AVGCreditingTime + ", " + L1.LASTCREDITED + ": " + CpaOffer.GetLastCredited() %></td>
                        </tr>
                    </table>
                </div>
                <asp:PlaceHolder ID="FieldsGroupPlaceHolder" runat="server">
                    <div class="col-md-6">
                        <div id="boxes<%=CpaOffer.Id %>">
                            <%--Show Login ID field? --%>
                            <asp:PlaceHolder ID="LoginIDPlaceHolder" runat="server" Visible="false">
                                <div class="form-group">
                                    <label>Login ID:</label>
                                    <asp:TextBox ID="LoginIDTextBox" CssClass="form-control" runat="server" MaxLength="80" ClientIDMode="Static"></asp:TextBox>
                                    <span class="help-block"><%=L1.LOGINIDHINT %></span>
                                </div>
                            </asp:PlaceHolder>

                            <%--Show Email ID field? --%>
                            <asp:PlaceHolder ID="EmailIDPlaceHolder" runat="server" Visible="false">
                                <div class="form-group">
                                    <label>Email:</label>
                                    <asp:TextBox ID="EmailIDTextBox" CssClass="form-control" Rows="3" runat="server" MaxLength="400" ClientIDMode="Static" TextMode="MultiLine"></asp:TextBox>
                                    <span class="help-block"><%=L1.OFFEMAILHINT %> </span>
                                </div>
                            </asp:PlaceHolder>

                        </div>

                        <%=Text %>
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>
    </div>

    <div class="offer-price col-md-2">
        <%=Amount %>
        <asp:Button ID="SubmitButton" CssClass="btn btn-inverse btn-lg btn-block" runat="server" OnClick="SubmitButton_Click"/>
    </div>

</li>

