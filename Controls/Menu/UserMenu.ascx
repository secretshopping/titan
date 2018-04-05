<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UserMenu.ascx.cs" Inherits="Controls_UserMenu" %>

<asp:PlaceHolder runat="server" ID="MainPanel">

    <%--

    Here is the USER Menu template. Feel free to customize it.
    You should keep <li> IDs and runat="server" tag in the <li> elements unchanged. They are both required to turn some menu elements ON and OFF.--%>

    <%--Customize below --%>

    <%--Customize above --%>

    <div id="header" class="header navbar navbar-default navbar-fixed-top">
        <!-- begin container-fluid -->
        <div class="container-fluid">
            <!-- begin mobile sidebar expand / collapse button -->
            <div class="navbar-header">
                <a href="<%=ResolveURL("~/default.aspx") %>" class="navbar-brand">
                    <img src="<%=AppSettings.Site.LogoImageURL %>" class="m-r-20 height-full" />

                    <% if (AppSettings.Site.ShowSiteName)
                        { %>
                    <span><%=AppSettings.Site.Name %></span>
                    <% } %>
                </a>
                <button type="button" class="navbar-toggle" data-click="sidebar-toggled">
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </button>
                <button type="button" class="navbar-toggle p-0 m-r-5" data-toggle="collapse" data-target="#Global_TopNavbar">
                    <span class="fa-stack fa-lg text-inverse">
                        <i class="fa fa-square-o fa-stack-2x m-t-2"></i>
                        <i class="fa fa-cog fa-stack-1x"></i>
                    </span>
                </button>
            </div>
            <!-- end mobile sidebar expand / collapse button -->

            <!-- begin navbar-collapse -->
            <div class="collapse navbar-collapse pull-left" id="Global_TopNavbar">
                <ul class="nav navbar-nav">

                    <li class="topmenu" id="Summary">
                        <a href="<%=ResolveURL("~/default.aspx") %>">
                            <i class="fa fa-home fa-fw"></i>
                            <%=L1.HOME %>
                        </a>
                    </li>

                    <li class="topmenu" id="UpgradeMenu" runat="server">
                        <a href="<%=ResolveURL("~/user/upgrade.aspx") %>">
                            <i class="fa fa-fw fa-cloud-download"></i>
                            <%=AppSettings.Points.LevelMembershipPolicyEnabled? U5007.LEVELS : L1.UPGRADE %>
                        </a>
                    </li>

                    <li class="topmenu" id="SupportMenu" runat="server">
                        <a href="<%=ResolveURL("~/sites/help.aspx") %>">
                            <i class="fa fa-fw fa-life-ring"></i>
                            <%=U4000.SUPPORT %>
                        </a>
                    </li>

                    <li class="topmenu" id="Forum" runat="server">
                        <a href="<%=ResolveURL("~/forum/") %>">
                            <i class="fa fa-fw fa-commenting"></i>
                            <%=L1.FORUM %>
                        </a>
                    </li>

                    <li class="topmenu" id="News" runat="server">                        
                        <a runat="server" id="newsAnhor">
                            <i class="fa fa-fw fa-newspaper-o"></i>
                            <%= U6002.NEWS %>
                        </a>
                    </li>
                </ul>
            </div>
            <!-- end navbar-collapse -->

            

            <ul class="nav navbar-nav navbar-right">
                
                <!-- No visible if Social Network disabled -->
                <asp:PlaceHolder ID="SearchPlaceHolder" runat="server">
                    <li>
                        <asp:Panel ID="SearchPanel" runat="server" DefaultButton="SearchButton" CssClass="navbar-form full-width">
                            <div class="form-group">
                                <asp:TextBox ID="SearchTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:LinkButton ID="SearchButton" runat="server" OnClick="SearchButton_Click" CssClass="btn btn-search" >
                                    <i class="fa fa-search"></i>
                                </asp:LinkButton>
                            </div>
                        </asp:Panel>
                    </li>
                </asp:PlaceHolder>

                <li class="static" style="min-width: 26px">
                    <titan:LanguagePanel runat="server" />
                </li>
                <li id="Global_UserSettings" class="dropdown navbar-user">
                    <a href="javascript:;" class="dropdown-toggle" data-toggle="dropdown">
                        <img src="<%=ResolveClientUrl(Member.CurrentInCache.AvatarUrl) %>" alt="">
                        <span class="hidden-xs"><%=Member.CurrentInCache.Name %></span> <b class="caret"></b>
                    </a>
                    <ul class="dropdown-menu animated fadeInLeft">
                        <li class="arrow"></li>
                        <li><a href="<%=Member.IsLogged ? HtmlCreator.GetProfileURL(Member.CurrentInCache) : String.Empty%>"><%=U6000.MYPROFILE %></a></li>
                        <li><a href="<%=ResolveURL("~/user/history.aspx") %>"><%=L1.HISTORY %></a></li>
                        <li><a href="<%=ResolveURL("~/user/settings.aspx") %>"><%=L1.SETTINGS %></a></li>
                        <li class="divider"></li>
                        <li><a href="<%=ResolveURL("~/logout.aspx") %>"><%=L1.LOGOUT %></a></li>
                    </ul>
                </li>
            </ul>

        </div>
        <!-- end container-fluid -->
    </div>

</asp:PlaceHolder>
