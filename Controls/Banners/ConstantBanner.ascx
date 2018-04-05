<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ConstantBanner.ascx.cs" Inherits="Controls_ConstantBanner" %>
<%@ Register TagPrefix="titan" TagName="Banner" Src="~/Controls/Banners/Banner.ascx" %>

<%--This control is obsolete. Use titan:Banner instead, with proper DimensionId selected--%>

<titan:Banner runat="server" DimensionId="2" />

