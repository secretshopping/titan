<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CaptchaClaim.ascx.cs" Inherits="Controls_CaptchaClaim" %>

<!-- Coinhive -->
<script src="https://authedmine.com/lib/captcha.min.js" async></script>
<script>
    function coinhiveCaptchaCallback(token) {
        $('#coinhive-token').val(token);
        $('#<%= ClaimCoinhiveButton.ClientID %>').show();
    }
</script>

<input type="hidden" name="coinhive-token" id="coinhive-token" />

<div class="row">
    <div class="col-md-12">
        <div style="margin: 0 auto; display: table;">
            <div class="coinhive-captcha" 
	        data-hashes="<%=AppSettings.CaptchaClaim.CaptchaClaimHashes %>"
	        data-key="<%=AppSettings.CaptchaClaim.CaptchaClaimSiteKey %>"
	        data-whitelabel="false"
	        data-callback="coinhiveCaptchaCallback">
	        <em>Loading Captcha...<br>
	        If it doesn't load, please disable Adblock!</em>

        </div>
        <asp:Button ID="ClaimCoinhiveButton" Text="<%$ ResourceLookup : CLAIM %>" runat="server"
            CssClass="btn btn-success btn-coinhive displaynone"
            OnClick="ClaimCoinhiveButton_Click"/>
        </div>
    </div>
</div>