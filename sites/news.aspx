<%@ Page Language="C#" AutoEventWireup="true" CodeFile="news.aspx.cs" Inherits="sites_news" MasterPageFile="~/Sites.master" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <div class="content" data-scrollview="true">
        <div class="container" data-animation="true" data-animation-type="fadeInDown">
            <h2 class="content-title"><%=U6002.NEWS %></h2>

            <asp:PlaceHolder ID="NoNewsPlaceHolder" runat="server" Visible="false">

                <div class="row">
                    <div class="col-md-12">
                        <p class="text-center f-s-15">
                            <%=U6012.NONEWS %>
                        </p>
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="NewsPlaceHolder" runat="server" Visible="false">
                <h3>
                    <asp:Literal ID="TitleLiteral" runat="server" />
                    <small>
                        <asp:Literal ID="DateLiteral" runat="server" />
                    </small>
                </h3>

                <div class="row">
                    <div class="col-md-9">
                        <p>
                            <asp:Literal ID="TextLiteral" runat="server" />
                        </p>
                        <br />
                        <br />
                    </div>
                    <div class="col-md-3">
                        <asp:PlaceHolder runat="server" ID="LatestNewsPlaceHolder"></asp:PlaceHolder>
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>
