<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="facebook.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <style>
        .fb-like.fb_iframe_widget.disabled:before {
            content: "";
            background: rgba(255,255,255,.8);
            bottom: 0;
            top: 0;
            left: 0;
            right: 0;
            position: absolute;
            z-index: 999;
        }

        .table tr td:last-child, .table tr th:last-child{
            text-align: center;
        }

    </style>
    <script type="text/javascript">

        function pageLoad() {
            window.focus();
            window.addEventListener('blur', function () {
                if (document.activeElement.tagName == 'IFRAME') {
                    $(".fb-like.fb_iframe_widget").addClass("disabled");
                    setTimeout(function () {
                        $(".fb-like.fb_iframe_widget").removeClass("disabled");
                        window.focus();
                    }, 3000);
                }
                window.focus();
            });


            $(".fb-like").click(function () {
                console.log(this);
            });
        }


        window.fbAsyncInit = function () {
            FB.init({
                appId: '<%=FacebookAppId%>', // App ID
                status: true, // check login status
                cookie: true, // enable cookies to allow the server to access the session
                xfbml: true  // parse XFBML
            });

            FB.Event.subscribe('edge.create', page_like_callback);
            FB.Event.subscribe('edge.remove', page_unlike_callback);

            // Additional initialization code here
            FB.Event.subscribe('auth.authResponseChange', function (response) {
                if (response.status === 'connected') {
                    // the user is logged in and has authenticated your
                    // app, and response.authResponse supplies
                    // the user's ID, a valid access token, a signed
                    // request, and the time the access token 
                    // and signed request each expire

                    if (getCookie('fbcookie') != 'ok1') {

                        var uid = response.authResponse.userID;
                        var accessToken = response.authResponse.accessToken;

                        // Do a post to the server to finish the logon
                        // This is a form post since we don't want to use AJAX
                        var form = document.getElementById("<%=Form.ClientID%>");
                        form.setAttribute("method", 'post');
                        form.setAttribute("action", 'User/earn/FBHandler.ashx');

                        var field = document.createElement("input");
                        field.setAttribute("type", "hidden");
                        field.setAttribute("name", 'accessToken');
                        field.setAttribute("value", accessToken);
                        form.appendChild(field);

                        document.body.appendChild(form);
                        form.submit();
                    }

                } else if (response.status === 'not_authorized') {
                    // the user is logged in to Facebook, 
                    // but has not authenticated your app
                } else {
                    // the user isn't logged in to Facebook.
                }
            });
        };

        var page_like_callback = function (url, html_element) {
            console.log('like');
            like_unlike(true, $(html_element).attr('data-ad-id'));
        }

        var page_unlike_callback = function (url, html_element) {
            console.log('unlike');
            like_unlike(false, $(html_element).attr('data-ad-id'));
        }

        var like_unlike = function (credit, adId) {
            $(".fb-like.fb_iframe_widget").addClass("disabled");
            $.ajax({
                type: "POST",
                url: 'user/earn/facebook.aspx/LikeUnlike',
                data: "{adId:'" + adId + "', credit: '" + credit + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (m) {
                    $(".fb-like.fb_iframe_widget").removeClass("disabled");
                    __doPostBack("Page", m.d);
                },
                error: function (e) {

                }
            });
        }

        function getCookie(c_name) {
            var c_value = document.cookie;
            var c_start = c_value.indexOf(" " + c_name + "=");
            if (c_start == -1) {
                c_start = c_value.indexOf(c_name + "=");
            }
            if (c_start == -1) {
                c_value = null;
            }
            else {
                c_start = c_value.indexOf("=", c_start) + 1;
                var c_end = c_value.indexOf(";", c_start);
                if (c_end == -1) {
                    c_end = c_value.length;
                }
                c_value = unescape(c_value.substring(c_start, c_end));
            }
            return c_value;
        }
    </script>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <%=BodyCode %>
    <h1 class="page-header"><%=L1.LIKES %></h1>

    <div class="tab-content">
        <div class="row">
            <div class="col-md-12">
                <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                    <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                </asp:Panel>

                <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="alert alert-success fade in m-b-15">
                    <asp:Literal ID="SuccMessage" runat="server"></asp:Literal>
                </asp:Panel>
            </div>
        </div>


        <div class="row">
            <div class="col-md-12 text-center m-b-20">
                <p class="alert alert-danger fade in m-b-15">
                    <asp:Label ID="CustomLikesInfoLabel" runat="server" />
                </p>
                <asp:Label ID="LabelInfo" runat="server" Text="Label"></asp:Label>
                <asp:Panel CssClass="box" Style="width: 400px" runat="server" ID="FacebookLoginPanel">
                    <div class="fb-login-button" data-show-faces="true" data-width="300" data-max-rows="1" scope="user_friends, user_likes"></div>
                </asp:Panel>
            </div>
        </div>

        <div class="row">
            <div class="col-md-12">
                <div class="table-responsive">
                    <asp:GridView ID="FacebookLikesGridView" runat="server" AllowSorting="True" AutoGenerateColumns="False" PageSize="50"
                        DataSourceID="FacebookLikesGridView_DataSource" OnRowDataBound="FacebookLikesGridView_RowDataBound"
                        DataKeyNames="FbAdvertId" Width="100%" OnRowCommand="FacebookLikesGridView_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="FbAdvertId" HeaderText="FbAdvertId" InsertVisible="False" ReadOnly="True" SortExpression="FbAdvertId" ItemStyle-CssClass="displaynone" HeaderStyle-CssClass="displaynone" />
                            <asp:BoundField DataField="MinFriends" HeaderText="<%$ ResourceLookup: FRIENDS %>" SortExpression="MinFriends" />
                            <asp:CheckBoxField DataField="HasProfilePicRestrictions" HeaderText="<%$ ResourceLookup: PICTURE %>" SortExpression="HasProfilePicRestrictions" />
                            <asp:BoundField DataField="FbAdvertId" HeaderText="" InsertVisible="False" ReadOnly="True" SortExpression="FbAdvertId" />
                            <asp:TemplateField ControlStyle-CssClass="text-center">                                
                                <ItemTemplate>
                                    <asp:LinkButton ID="LikeButton" runat="server"                                       
                                        CommandArgument='<%# Container.DataItemIndex %>'>                                        
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-md-12">
                <asp:SqlDataSource ID="FacebookLikesGridView_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>" OnInit="FacebookLikesGridView_DataSource_Init"></asp:SqlDataSource>

                <p>
                    <b><%=L1.WHATSREWARD %></b><br />
                    <%=L1.YOUEARNPOINTS.Replace("%n%","<b>" + AppSettings.Facebook.PointsPerLike + "</b>") %>
                </p>

                <asp:PlaceHolder ID="MyFriendsPlaceHolder" runat="server">
                    <p>
                        <b><%=L1.MYFRIENDSNMBERLOW %></b><br />
                        <%=L1.MYFRIENDSNMBERLOWANSW %>
                    </p>
                </asp:PlaceHolder>
            </div>
        </div>
    </div>

</asp:Content>
