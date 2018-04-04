<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true" EnableEventValidation="false" ValidateRequest="false" 
    CodeFile="banners.aspx.cs" Inherits="BannersAndTools"  %>

<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script src="Scripts/default/assets/plugins/clipboard/clipboard.min.js"></script>
    <script>
        function pageLoad() {
            var clipboard = new Clipboard('.clipboard');
            $('.clipboard').tooltip({ trigger: 'focus' });
        }
    </script>
</asp:Content>


<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%= U5007.TOOLS %></h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=U5007.TOOLSINFO %></p>
        </div>
    </div>
    
    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <div class="TitanViewPage">
                    <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                        <asp:Button ID="Button4" runat="server" OnClick="MenuButton_Click" CommandArgument="3" />
                        <asp:Button ID="Button3" runat="server" OnClick="MenuButton_Click" CommandArgument="2" />
                        <asp:Button ID="Button2" runat="server" OnClick="MenuButton_Click" CommandArgument="1" />
                        <asp:Button ID="Button1" runat="server" OnClick="MenuButton_Click" CommandArgument="0" CssClass="ViewSelected" />
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>

    <div class="tab-content">
        <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">

            <asp:View runat="server" ID="links" OnActivate="links_Activate">
                <div class="TitanViewElement">
                    <%-- SUBPAGE START --%>
                        
                    <div class="row">
                        <div class="col-md-12">
                            <iframe src="../../register.aspx" style="width: 100%; height: 300px;" class="hidden-xs"></iframe>
                        </div>
                    </div>

                    <div class="row m-b-20">
                        <div class="col-md-12">

                            <%=L1.REFLINK %>:
                                <asp:Literal ID="RefLink" runat="server"></asp:Literal>
                            <asp:HiddenField runat="server" ID="RefLinkNoAnchor" />
                            <div class="clipboard-wrapper">
                                <pre id="refLinkNoAnchorPre"><%=RefCodeStrNoAnchor %></pre>
                                <button type="button" class="clipboard btn btn-inverse height-full" data-click="tooltip" data-placement="top" title="<%=U6000.COPIED %>!" data-clipboard-target="#refLinkNoAnchorPre"><%=U6000.COPY %></button>
                            </div>
                            <div class="clipboard-wrapper">
                                <pre id="<%=RefLink.ClientID %>"><%=System.Net.WebUtility.HtmlEncode(RefCodeStr) %></pre>
                                <button type="button" class="clipboard btn btn-inverse height-full" data-click="tooltip" data-placement="top" title="<%=U6000.COPIED %>!" data-clipboard-target="#<%=RefLink.ClientID %>"><%=U6000.COPY %></button>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            <iframe src="../../default.aspx" style="width: 100%; height: 300px;" class="hidden-xs"></iframe>
                        </div>
                    </div>

                    <div class="row m-b-20">
                        <div class="col-md-12">

                            <%=L1.REFLINK %>:
                                <asp:Literal ID="RefLink2" runat="server"></asp:Literal>
                            <div class="clipboard-wrapper">
                                <pre id="<%=RefLink2.ClientID %>"><%=System.Net.WebUtility.HtmlEncode(RefCodeStr2) %></pre>
                                <button type="button" class="clipboard btn btn-inverse height-full" data-click="tooltip" data-placement="top" title="<%=U6000.COPIED %>!" data-clipboard-target="#<%=RefLink2.ClientID %>"><%=U6000.COPY %></button>
                            </div>

                        </div>
    </div>
             
                </div>
            </asp:View>

            <asp:View runat="server" ID="banners" OnActivate="banners_Activate">
                <div class="TitanViewElement">
                    <%-- SUBPAGE START --%>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:PlaceHolder ID="ReferralsBannerList" runat="server" />
                        </div>
                    </div>
                </div>
            </asp:View>


            <asp:View runat="server" ID="splash" OnActivate="splash_Activate">
                <div class="TitanViewElement">
                    <%-- SUBPAGE START --%>
                    <div class="row">
                        <div class="col-md-12">
                            <iframe src="../../splash/welcome.aspx" style="width: 100%; height: 300px;" class="hidden-xs"></iframe>
                        </div>
                    </div>
                    <div class="row m-b-20">
                        <div class="col-md-12">

                            <%=L1.REFLINK %>:
                        <asp:Literal ID="SplashLink" runat="server"></asp:Literal>
                            <div class="clipboard-wrapper">
                                <pre id="<%=SplashLink.ClientID %>"><%=System.Net.WebUtility.HtmlEncode(SplashCodeStr) %></pre>
                                <button type="button" class="clipboard btn btn-inverse height-full" data-click="tooltip" data-placement="top" title="<%=U6000.COPIED %>!" data-clipboard-target="#<%=SplashLink.ClientID %>"><%=U6000.COPY %></button>
                            </div>

                        </div>
                    </div>

                </div>
            </asp:View>

            <asp:View runat="server" ID="customSplash" OnActivate="customSplash_Activate">
                <div class="TitanViewElement">
                    <%-- SUBPAGE START --%>
                    <div class="row">
                        <div class="col-md-12">
                            <p><b><%=U5007.CUSTOMLINK %>:</b></p>
                            <p class="text-wrap"><asp:Literal ID="CustomSplashPageLink" runat="server"></asp:Literal></p>
                        </div>
                        <div class="col-md-2">
                            <asp:Button ID="SaveButton" runat="server"
                                CssClass="btn btn-inverse btn-block m-b-15" OnClick="SaveButton_Click"
                                
                                UseSubmitBehavior="false" />
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            <CKEditor:CKEditorControl ID="SplashPageCKEditor" Height="600px" 
                                BasePath="../../Scripts/ckeditor/" runat="server"></CKEditor:CKEditorControl>

                        </div>
                    </div>


                </div>
            </asp:View>

        </asp:MultiView>

    </div>

</asp:Content>
