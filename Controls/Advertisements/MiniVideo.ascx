<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MiniVideo.ascx.cs" Inherits="Controls_Advertisements_MiniVideo" %>

<%@ Import Namespace="Prem.PTC.Utils" %>

<asp:PlaceHolder ID="MiniVideoPlaceHolder" runat="server">
    <div id="mainDiv" runat="server" class="miniVideoMain col-xs-12 col-sm-6 col-md-4 text-center p-40">
        <div class="miniVideoCard">
            <div class="row">
                <div class="col-md-12">
                    <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger">
                        <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                    </asp:Panel>

                    <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="alert alert-success">
                        <asp:Literal ID="SuccMessage" runat="server"></asp:Literal>
                    </asp:Panel>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <h3 class="desc text-center">
                        <%=Title %>
                    </h3>

                    <div class="miniVideoBg m-t-20 m-b-20"
                        style="background: url(<%=ResolveClientUrl(ImageURL)%>); background-size: cover; background-repeat: no-repeat; background-position: center; height: 200px; width: 100%;">

                        <div class="miniVideoDesc" style="height: 200px; width: 100%; display: none">
                            <p class="text-center m-t-15" style="font-size: 14px; color: black;">
                                <%=Description %>
                            </p>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-md-6">
                    <div class="desc text-left">
                        <b><%=VideoCategory %></b>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="desc text-right">
                        <b><%=Price %></b>
                    </div>
                </div>
            </div>

            <div class="row m-t-15">
                <div class="col-md-9">
                    <div class="form-horizontal">
                        <titan:TargetBalance runat="server" Feature="MiniVideo" ID="TargetBalanceRadioButtonList"></titan:TargetBalance>
                    </div>
                </div>
                <div class="col-md-3">
                    <asp:Button ID="BuyButton" CssClass="btn btn-inverse btn-block" runat="server" OnClick="BuyButton_Click" />
                </div>
            </div>

        </div>

    </div>
</asp:PlaceHolder>
