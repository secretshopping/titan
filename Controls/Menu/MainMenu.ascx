<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MainMenu.ascx.cs" Inherits="Controls_MainMenu" %>
<%@ Register Src="~/Controls/Misc/LanguagePanel.ascx" TagPrefix="titan" TagName="LanguagePanel" %>

<%--

Here is the MAIN Menu template. Feel free to customize it.
You should keep <li> IDs and runat="server" tag in the <li> elements unchanged. They are both required to turn some menu elements ON and OFF.--%>

<ul class="menu nav navbar-nav navbar-right">

    <asp:PlaceHolder ID="SearchPlaceHolder" runat="server">
        <li>
            <asp:Panel ID="SearchPanel" runat="server" DefaultButton="SearchButton" CssClass="navbar-form full-width">
                <div class="form-group">
                    <asp:TextBox ID="SearchTextBox" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                    <asp:LinkButton ID="SearchButton" runat="server" CssClass="btn btn-search" OnClick="SearchButton_Click">
                                    <i class="fa fa-search"></i>
                    </asp:LinkButton>
                </div>
            </asp:Panel>
        </li>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="ForLoggedInMembers">

        <%--Customize below --%>
        <li id="mm1" class="static" runat="server"><a href="<%=ResolveURL("~/default.aspx") %>"><%=L1.HOME %></a></li>
        <li id="mm2" class="static" runat="server"><a href="<%=ResolveURL("~/user/default.aspx") %>"><%=L1.ACCOUNT %><%=GetNotificationHTML(Account)%></a></li>
        <li id="mm3" class="static" runat="server"><a href="<%=ResolveURL("~/sites/help.aspx") %>"><%=U4000.SUPPORT %></a></li>
        <li id="mm4" class="static" runat="server"><a href="<%=ResolveURL("~/forum/") %>"><%=L1.FORUM %></a></li>
        <%--Customize above --%>

    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="ForNotLoggedInMembers">

        <%--Customize below --%>
        <li id="pmm1" class="static" runat="server"><a href="<%=ResolveURL("~/default.aspx") %>"><%=L1.HOME %></a></li>
        <li id="pmm2" class="static" runat="server"><a href="<%=ResolveURL("~/sites/advertise.aspx") %>"><%=L1.ADVERTISE %></a></li>
        <li id="pmm3" class="static" runat="server"><a href="<%=ResolveURL("~/register.aspx") %>"><%=L1.REGISTER %></a></li>
        <li id="pmm4" class="static" runat="server"><a href="<%=ResolveURL("~/sites/help.aspx") %>"><%=U4000.SUPPORT %></a></li>
        <li id="pmm5" class="static" runat="server"><a href="<%=ResolveURL("~/forum/") %>"><%=L1.FORUM %></a></li>
        <li id="pmm6" class="static" runat="server"><a href="<%=ResolveURL("~/login.aspx") %>"><%=L1.LOGIN %></a></li>
        <%--Customize above --%>

    </asp:PlaceHolder>

    <li class="static" style="min-width: 26px">
        <titan:LanguagePanel runat="server" />
    </li>

</ul>
