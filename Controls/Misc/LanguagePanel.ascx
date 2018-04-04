<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LanguagePanel.ascx.cs" Inherits="Controls_LanguagePanel" %>
<link href="Scripts/default/assets/plugins/bootstrap-select/css/bootstrap-select.min.css" rel="stylesheet" type="text/css">
<script src="Scripts/default/assets/plugins/bootstrap-select/js/bootstrap-select.min.js"></script>
<div id="flags" style="float: left">
    <asp:Panel ID="LangPanel" runat="server" CssClass="inline"></asp:Panel>
</div>

<asp:PlaceHolder ID="MulticurrencyPlaceHolder" runat="server" Visible="false">

    <div id="Global_CurrencyChange" class="currencyDropdownWrapper" style="display: inline-block; margin-top: 12px; margin-left: 15px;">

        <asp:DropDownList ID="CurrencyDropDownList" runat="server" AutoPostBack="true" CssClass="currencyDropDown show-tick"
            Style="width: 70px; border: 1px solid #cecece; border-radius: 15px; font-size: smaller; padding: 6px 0px 6px 8px; color: #a7a3a3;"
            OnSelectedIndexChanged="CurrencyDropDownList_SelectedIndexChanged">
        </asp:DropDownList>
    </div>
    <script>
            $('.currencyDropDown').selectpicker({
                style: 'currencyDropDownButton',
                size: 15,
                hideDisabled: false
            });
    </script>
</asp:PlaceHolder>
