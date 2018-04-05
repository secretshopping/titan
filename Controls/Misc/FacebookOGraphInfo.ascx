<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FacebookOGraphInfo.ascx.cs" Inherits="Controls_Misc_FacebookHandlers" %>

<meta property="og:type" content="article" />
<meta property="og:title" <%="content='" + HttpUtility.HtmlEncode(AppSettings.Site.Name) + "'" %> />
<meta property="og:description" <%="content='" + HttpUtility.HtmlEncode(AppSettings.Site.Description) + "'"%> />
<meta property="og:image" <%="content='" +  FacebookOgraphImageURL + "'"%> />
<script src="Plugins/Alert/js/jquery.litealert.js"></script>
