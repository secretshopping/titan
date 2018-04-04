<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="adpacks.aspx.cs" Inherits="About" %>

<%@ MasterType VirtualPath="~/User.master" %>


<asp:Content ContentPlaceHolderID="PageHeadContent" runat="server">
    
    <script type="text/javascript">
        function doSubmit(eventArgument, isSurfable) {
            $('#__EVENTARGUMENT5').val(eventArgument);

            if (isSurfable) {
                $('#<%=Form.ClientID%>').attr('action', 'user/earn/surf.aspx?f=2');
                $('#<%=Form.ClientID%>').attr('target', '_blank');
                $('.tipsy').hide();
            }
            $('#<%=Form.ClientID%>').submit();
            $('#<%=Form.ClientID%>').attr('action', 'user/earn/adpacks.aspx');
            $('#<%=Form.ClientID%>').attr('target', '_self');
        }
        jQuery(function ($) {
            $(document).on('click', '.AboxActive', function () {
                var AdId = $(this).find('input').val();
                doSubmit(AdId, true);
            });
        });
    </script>
</asp:Content>


<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <h1 class="page-header"><%=AppSettings.RevShare.AdPack.AdPackName %> <%=U4200.ADVERTISEMENTS.ToLower() %></h1>

    <div class="row">
        <div class="col-md-12">
            <p class="lead">
                <%=U4200.ADPACKINFO.Replace("%n%", AppSettings.RevShare.AdPack.AdPackNamePlural) %>
            </p>
        </div>
    </div>

    <%-- SUBPAGE START --%>

    <div class="row">
        <div class="col-md-3 text-center p-20">
            <titan:RevShareConstantBanner ID="ConstantBanner1" IndexOnPage="0" runat="server" />
        </div>
        <div class="col-md-3 text-center p-20">
            <titan:RevShareConstantBanner ID="ConstantBanner2" IndexOnPage="1" runat="server" />
        </div>
        <div class="col-md-3 text-center p-20">
            <titan:RevShareConstantBanner ID="ConstantBanner3" IndexOnPage="2" runat="server" />
        </div>
        <div class="col-md-3 text-center p-20">
            <titan:RevShareConstantBanner ID="ConstantBanner4" IndexOnPage="3" runat="server" />
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <titan:RevShareNormalBanner runat="server" IndexOnPage="0" />
        </div>
        <div class="col-md-12">
            <p class="alert alert-info">
                <asp:Literal runat="server" ID="AdsWatchedInfo"></asp:Literal><br />              
            </p>
        </div>
        <div class="col-md-12" runat="server" id="CommissionTransferInfoDiv">
            <p class="alert alert-info">
                <asp:Label runat="server" ID="CommissionTransferInfo"></asp:Label><br />
            </p>
        </div>
        <asp:PlaceHolder runat="server">
            <div class="col-md-12">
                <p class="alert alert-info">
                    <asp:Label runat="server"><%=U6000.RESETTIME %><%=Prem.PTC.Utils.TimeSpanExtensions.ToFriendlyDisplay(AppSettings.Site.TimeToNextCRONRun, 3) %></asp:Label>
                </p>
            </div>
        </asp:PlaceHolder>
    </div>

    <div class="row">
        <div class="col-md-12">
            <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="AdPackRefreshUpdatePanel"></asp:UpdateProgress>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <asp:UpdatePanel runat="server" ID="AdPackRefreshUpdatePanel" OnLoad="AdPackRefreshUpdatePanel_Load" class="gallery" ClientIDMode="Static">
                <ContentTemplate>

                    <input type="hidden" name="__EVENTARGUMENT5" id="__EVENTARGUMENT5" value="" />
                    <titan:StartSurfingAdPack runat="server"></titan:StartSurfingAdPack>

                    <%--Generate advertisements--%>
                    <asp:PlaceHolder ID="AdsLiteral" runat="server"></asp:PlaceHolder>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>


    <%-- SUBPAGE END   --%>
</asp:Content>
