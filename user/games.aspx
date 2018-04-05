<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="games.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">


    <script type="text/javascript">
        function showL(number) {
            var elem = document.getElementById('Le' + number);
            $('Le' + number).show();
        }
    </script>

</asp:Content>





<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <h1 class="page-header"><%= L1.GAMES %></h1>
    
    <div class="tab-content">
        <div class="row">
            <div class="col-md-12">
                <p class="lead"><%= U4001.GAMEINFO %></p>
            </div>
        </div>
    
        <div class="row">
            <div class="col-md-12">
                <p><asp:Literal ID="GamesLiteral" runat="server" Visible="true" /></p>

                <asp:Panel ID="OneGamePanel" runat="server" Visible="false">

                        <h3><%=GetTitlGame()%></h3>

                        <iframe scrolling="no" id="fg-frame-gamecontainer" width="640" height='<%=GetHeight()%>' data-aspect-ratio='<%=GetAscpetRatio()%>' src="" frameborder="0"></iframe>


                    <script type="text/javascript">
                        (function (d, url, fgJS, firstJS) {

                            fgJS = d.createElement('script'); firstJS = d.getElementsByTagName('script')[0];
                            fgJS.src = url;
                            fgJS.onload = function () {
                                var fg_frame = document.getElementById('fg-frame-gamecontainer');
                                var fg_url = '<%=GetUrl()%>';
                                var mobileRedirect = !0; var mobileTablet = !0;
                                famobi_embedFrame(fg_frame, fg_url, mobileRedirect, mobileTablet);
                            };
                            firstJS.parentNode.insertBefore(fgJS, firstJS);
                        })(document, '//games.cdn.famobi.com/html5games/plugins/embed.js?v=2.1');
                    </script>

                    <p><%=GetDescriptionGame()%></p>

                </asp:Panel>
            </div>
        </div>
    </div>
    

</asp:Content>



