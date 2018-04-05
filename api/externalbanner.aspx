<%@ Page Language="C#" AutoEventWireup="true" CodeFile="externalbanner.aspx.cs" Inherits="externalbanner" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="Scripts/default/assets/plugins/jquery/jquery-1.9.1.min.js"></script>
    <script>
        function VisitWebsite() {
            var targetUrl = '<%=targetUrl%>';
            window.open(targetUrl, '_blank');
        }
    </script>
    <style>
        body, form, input {
            margin: 0;
            padding: 0;
        }
    </style>
</head>
<body class="banner">
    <form runat="server">
        <asp:ImageButton runat="server" OnClick="BannerLink_Click" ID="BannerLink" OnClientClick="VisitWebsite();" />
    </form>
</body>
</html>
