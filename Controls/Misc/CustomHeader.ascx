<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CustomHeader.ascx.cs" Inherits="Controls_Misc_CustomHeader" %>

<link href="Plugins/BootstrapTour/bootstrap-tour.min.css" rel="stylesheet">    

<asp:Literal runat="server" ID="ScriptsLiteral" OnInit="ScriptsLiteral_Init"></asp:Literal>
<%=Content %>