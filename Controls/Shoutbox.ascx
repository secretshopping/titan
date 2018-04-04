<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Shoutbox.ascx.cs" Inherits="Controls_Shoutbox" %>
<%@ Register Src="~/Controls/Messages/ErrorPanel.ascx" TagPrefix="titan" TagName="ErrorPanel" %>
<%@ Register Src="~/Controls/Messages/SuccessPanel.ascx" TagPrefix="titan" TagName="SuccessPanel" %>

<asp:PlaceHolder ID="ShoutboxPanel" runat="server">

    <script type="text/javascript">


        var activeCart = 0;

        function StartRequestHandler() {
            var obj = document.getElementById("<%=TextLabel.ClientID %>");
        if (obj != null)
            obj.innerHTML = "<%=Resources.U3501.SENDING %>";

        var from = document.getElementById("<%=Message.ClientID %>");
        var to = document.getElementById("<%=Message2.ClientID %>");

        if (from != null && to != null) {
            to.value = from.value;
            from.value = '';
        }
    }

    function ShoutboxEndRequestHandler() {
        var obj = document.getElementById("<%=TextLabel.ClientID %>");
        if (obj != null)
            obj.innerHTML = "";

        var to = document.getElementById("<%=Message2.ClientID %>");
        if (to != null) {
            to.value = '';
        }

        <%=(Prem.PTC.AppSettings.Shoutbox.DisplayContent == ShoutboxDisplayContent.ChatAndEventsInSeparateTabs)? "changeCart(activeCart);" : "" %>

        $(function () {
            $('#customScroll').slimScroll({
                height: 'auto'
            });
        });
    }

    function ChangeDisplayState(newState) {
        document.getElementById("<%=ShoutboxInputPanel.ClientID %>").style.display = newState;
    }

    function changeCart(newCart) {
        if (newCart == 0) {
            $(".sbentry").show();
            $(".sbentryEvent").hide();
            $("#shoutbox_Cart_0").addClass("btn-inverse");
            $("#shoutbox_Cart_1").removeClass("btn-inverse");
            activeCart = 0;
        }
        else {
            $(".sbentry").hide();
            $(".sbentryEvent").show();
            $("#shoutbox_Cart_0").removeClass("btn-inverse");
            $("#shoutbox_Cart_1").addClass("btn-inverse");
            activeCart = 1;
        }
    }

    </script>

    

    <div class="col-md-6 ui-sortable">
            <div class="panel panel-inverse shoutbox-panel" data-sortable-id="table-basic-4" data-init="true">
                <div class="panel-heading">
                    <asp:PlaceHolder ID="PanelButtonsPlaceHolder" runat="server">
                        <div class="panel-heading-btn">
                            <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-default" data-click="panel-expand"><i class="fa fa-expand"></i></a>
                            <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-success" data-click="panel-reload"><i class="fa fa-repeat"></i></a>
                            <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-warning" data-click="panel-collapse"><i class="fa fa-minus"></i></a>
                            <a href="javascript:;" class="btn btn-xs btn-icon btn-circle btn-danger" data-click="panel-remove"><i class="fa fa-times"></i></a>
                        </div>
                    </asp:PlaceHolder>
                    <h4 class="panel-title"><asp:Literal Id="TitleLiteral" runat="server" /></h4>
                </div>

                <div class="panel-body">
                    <asp:PlaceHolder ID="CartPlaceholder" runat="server" Visible="false">
                        <button type="button" id="shoutbox_Cart_0" class="shoutbox_cart btn btn-xs btn-default btn-inverse m-b-15" onclick="changeCart(0);"><%=U4000.CHAT %></button>
                        <button type="button" id="shoutbox_Cart_1" class="shoutbox_cart btn btn-xs btn-default m-b-15" onclick="changeCart(1);" runat="server"><%=U4000.EVENTS %></button>
                    </asp:PlaceHolder>
                <div class="height-sm" data-scrollbar="true">            
                    <asp:Timer ID="UpdateTimer" runat="server" OnTick="UpdateTimer_Tick"></asp:Timer>
                    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                        <ProgressTemplate>
                            <div class="shoutbox_loading">
                                <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/Controls/Shoutbox/loading.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" OnLoad="UpdatePanel1_Load" ChildrenAsTriggers="true">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
                            <asp:AsyncPostBackTrigger ControlID="SendMessageButton" EventName="Click" />
                        </Triggers>
                        <ContentTemplate>
                            <ul class="media-list media-list-with-divider media-messaging">

                                <titan:ErrorPanel runat="server" ID="ErrorPanelCtrl" Visible="false"></titan:ErrorPanel>
                                <titan:SuccessPanel runat="server" ID="SuccessPanelCtrl" Visible="false"></titan:SuccessPanel>

                                <asp:Literal runat="server" ID="ContentLiteral" />

                                <asp:Panel runat="server" ID="EmptyPanel" Visible="false" CssClass="shoutbox_empty">
                                    <asp:Label ID="EmptyLabel" runat="server"></asp:Label>

                                </asp:Panel>

                            </ul>

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
            <div class="panel-footer">
                <asp:Panel ID="ShoutboxInputPanel" runat="server" DefaultButton="SendMessageButton" CssClass="input-group">
                    <asp:TextBox ID="Message" runat="server" MaxLength="1000" CssClass="form-control bg-silver"></asp:TextBox>
                    <asp:TextBox ID="Message2" runat="server" CssClass="displaynone"></asp:TextBox>
                    <span class="input-group-btn">
                        <asp:LinkButton ID="SendMessageButton" CssClass="btn btn-primary" runat="server" OnClick="SendMessageButton_Click" OnClientClick="StartRequestHandler()">
                            <span class="fa fa-pencil"></span>
                        </asp:LinkButton>
                    </span>
                </asp:Panel>
                <asp:Label ID="TextLabel" runat="server" CssClass="smalltextlabel"></asp:Label>
            </div>

            </div>
        </div>

</asp:PlaceHolder>

