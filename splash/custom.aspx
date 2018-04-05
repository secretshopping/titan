<%@ Page Language="C#" AutoEventWireup="true"
    CodeFile="custom.aspx.cs" Inherits="About" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title><%=AppSettings.Site.Name %> | <%=AppSettings.Site.Slogan %></title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link rel="shortcut icon" type="image/ico" href="<%=AppSettings.Site.FaviconImageURL %>" />
    <link rel="stylesheet" href="/Styles/Global.css" type="text/css" />
    <!--This page uses the script from http://blakek.us/labs/jquery/css3-pie-graph-timer/ -->
</head>

<body>
 <asp:Literal ID="PageLiteral" runat="server"></asp:Literal>
</body>
</html>
