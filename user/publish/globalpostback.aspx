<%@ Page Language="C#" AutoEventWireup="true" CodeFile="globalpostback.aspx.cs" Inherits="user_publish_globalpostback" MasterPageFile="~/User.master" %>
<%@ Import Namespace="Prem.PTC.Utils" %>
<%@ Import Namespace="Titan.Publisher" %>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <asp:UpdatePanel ID="MultiViewUpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <h1 class="page-header"><%= U6000.GLOBALPOSTBACK%></h1>
            <div class="row">
                <div class="col-md-12">
                    <p class="lead"><%=U6000.GLOBALPOSTBACKDESC %></p>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                        <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                    </asp:Panel>
                    <asp:Panel ID="SuccessMessagePanel" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
                        <asp:Literal ID="SuccessMessage" runat="server"></asp:Literal>
                    </asp:Panel>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <div class="nav nav-tabs custom text-right">
                        <div class="TitanViewPage">
                            <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                                <asp:Button ID="TestPostbackMenuButton" runat="server" OnClick="MenuButton_Click" CommandArgument="2" />
                                <asp:Button ID="DocumentationMenuButton" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                                <asp:Button ID="NewPostbackMenuButton" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </div>
            <div class="tab-content">
                <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
                    <asp:View runat="server" ID="NewPostbackView" OnActivate="NewPostbackView_Activate">
                        <div class="TitanViewElement">
                            <div class="row">
                                <div class="col-md-12">
                                    <asp:ValidationSummary ID="AddPostbackUrlValidationSummary" runat="server" CssClass="alert alert-danger fade in m-b-15"
                                        ValidationGroup="AddPostbackUrlValidationGroup" DisplayMode="List" />
                                </div>
                                <div class="col-md-12">
                                    <div class="form-horizontal" runat="server" id="NewPostbackPlaceHolder">
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=U6000.WEBSITE %>:</label>
                                            <div class="col-md-6">
                                                <asp:DropDownList runat="server" ID="WebsitesDDL" class="form-control" OnInit="WebsitesDDL_Init">
                                                </asp:DropDownList>
                                                <asp:CustomValidator runat="server" ID="ChosenWebsiteCustomValidator" ValidationGroup="AddPostbackUrlValidationGroup" OnServerValidate="ChosenWebsiteCustomValidator_ServerValidate"></asp:CustomValidator>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=U6000.POSTBACKURL %>:</label>
                                            <div class="col-md-6">
                                                <asp:TextBox runat="server" ID="PostbackUrlTextBox" placeholder="http://mywebsite.com/mypostbackhandler" MaxLength="800" CssClass="form-control"></asp:TextBox>
                                                <asp:RegularExpressionValidator Display="Dynamic" CssClass="text-danger" ValidationGroup="AddPostbackUrlValidationGroup"
                                                    ID="UrlRegularExpressionValidator" runat="server" ErrorMessage="*"
                                                    ControlToValidate="PostbackUrlTextBox" Text="">*</asp:RegularExpressionValidator>
                                                <asp:RequiredFieldValidator ID="UrlRequiredFieldValidator" runat="server"
                                                    ControlToValidate="PostbackUrlTextBox" Display="Dynamic" CssClass="text-danger"
                                                    ValidationGroup="AddPostbackUrlValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="col-md-2">
                                                <asp:Button ID="AddPostbackUrlButton" runat="server"
                                                    ValidationGroup="AddPostbackUrlValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="AddPostbackUrlButton_Click" />
                                            </div>
                                        </div>
                                    </div>
                                    <titan:FeatureUnavailable runat="server" ID="AddPostbackUnavailable"></titan:FeatureUnavailable>
                                </div>
                            </div>
                        </div>
                    </asp:View>
                    <asp:View runat="server" ID="DocumentationView">
                        <div class="TitanViewElement">

                            <h3><%=U6000.WHITELISTIP %></h3>
                            <p><%=string.Format(U6000.WHITELISTIPINFO, AppSettings.Site.Name, AppSettings.ServerIP) %></p>

                            <h3><%=U6000.SUCCESSFULRESPONSE %></h3>
                            <p><%=string.Format(U6000.SUCCESSFULRESPONSEDESC, GlobalPostback.Parameters.SuccessfulResponse) %></p>

                            <h3><%=U6000.POSTBACKPARAMS %></h3>
                            <table class="table table-striped table-hover">
                                <thead>
                                    <tr>
                                        <th><%=U6000.PARAMETER %></th>
                                        <th><%=L1.DESCRIPTION %></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>{<%=GlobalPostback.Parameters.OfferId %>}
                                        </td>
                                        <td><%=U6000.PBCAMPAIGNID %>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>{<%=GlobalPostback.Parameters.OfferName %>}
                                        </td>
                                        <td><%=U6000.PBCAMPAIGNNAME %>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>{<%=GlobalPostback.Parameters.IpAddress%>}
                                        </td>
                                        <td><%=U6000.PBIPADDRESS %>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>{<%=GlobalPostback.Parameters.CountryCode%>}
                                        </td>
                                        <td><%=U6000.PBCOUNTRYCODE%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>{<%=GlobalPostback.Parameters.Payout %>}
                                        </td>
                                        <td><%=U6000.PBPAYOUT%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>{<%=GlobalPostback.Parameters.CurrencyCode %>}
                                        </td>
                                        <td><%=U6000.PBCURRENCYCODE %>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>{<%=GlobalPostback.Parameters.SubId %>}
                                        </td>
                                        <td><%=U6000.PBSUBID %>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>{<%=GlobalPostback.Parameters.SubId2 %>}
                                        </td>
                                        <td><%=U6000.PBSUBID2 %>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>{<%=GlobalPostback.Parameters.SubId3 %>}
                                        </td>
                                        <td><%=U6000.PBSUBID2 %>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>{<%=GlobalPostback.Parameters.Age %>}
                                        </td>
                                        <td><%=U6002.PBAGE %>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>{<%=GlobalPostback.Parameters.Gender %>}*
                                        </td>
                                        <td><%=U6002.PBGENDER %>
                                        </td>
                                    </tr>
                                </tbody>
                                <tfoot>
                                    <tr>
                                        <td><span class="small">*<%=U6002.GENDEROPTIONS %></span></td>
                                    </tr>
                                </tfoot>
                            </table>
                        </div>
                    </asp:View>
                    <asp:View runat="server" ID="TestPostbackView" OnActivate="TestPostbackView_Activate">
                        <div class="TitanViewElement">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="form-horizontal" runat="server" id="TestPostbackPlaceHolder">
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=U6000.POSTBACKURL %>:</label>
                                            <div class="col-md-6">
                                                <asp:DropDownList runat="server" ID="PostbackUrlsDDL" class="form-control" OnInit="PostbackUrlsDDL_Init">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=GlobalPostback.Parameters.SubId %>:</label>
                                            <div class="col-md-6">
                                                <asp:TextBox runat="server" ID="TestPostback_SubIdTextBox" CssClass="form-control" MaxLength="500"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="TestPostback_SubId_RequiredFieldValidator" runat="server"
                                                    ControlToValidate="TestPostback_SubIdTextBox" Display="Dynamic" CssClass="text-danger"
                                                    ValidationGroup="TextPostbackValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="control-label col-md-2"><%=GlobalPostback.Parameters.Payout %>:</label>
                                            <div class="col-md-6">
                                                <asp:TextBox runat="server" ID="TestPostback_PayoutTextBox" CssClass="form-control" MaxLength="500"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="TestPostback_Payout_RequiredFieldValidator" runat="server"
                                                    ControlToValidate="TestPostback_PayoutTextBox" Display="Dynamic" CssClass="text-danger"
                                                    ValidationGroup="TextPostbackValidationGroup" Text="">*</asp:RequiredFieldValidator>
                                                <asp:CustomValidator runat="server" ControlToValidate="TestPostback_PayoutTextBox" OnServerValidate="ValidateMoneyWithNegative" 
                                                    ValidationGroup="TextPostbackValidationGroup" ErrorMessage="Invalid money format" Display="Dynamic" CssClass="text-danger"></asp:CustomValidator>
                                            </div>
                                            <div class="col-md-1">
                                               <span class="form-control no-border"> <%=AppSettings.Site.CurrencySign %></span>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="col-md-2">
                                                <asp:Button ID="TestPostbackButton" runat="server"
                                                    ValidationGroup="TextPostbackValidationGroup" CssClass="btn btn-inverse btn-block" OnClick="TestPostbackButton_Click" />
                                            </div>
                                        </div>
                                    </div>
                                     <titan:FeatureUnavailable runat="server" ID="TestPostbackUnavailable"></titan:FeatureUnavailable>
                                </div>
                            </div>
                        </div>
                    </asp:View>
                </asp:MultiView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
