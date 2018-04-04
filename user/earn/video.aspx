<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="video.aspx.cs" Inherits="EarnSearch" %>

<%@ MasterType VirtualPath="~/User.master" %>

<%@ Import Namespace="Prem.PTC" %>
<%@ Import Namespace="Resources" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <!--This page uses Tipsy (http://onehackoranother.com/projects/jquery/tipsy) script
    Readme file is located in Plugins/Tipsy/README.txt
    License file is located in Plugins/Tipsy/LICENSE.txt-->
    <link href="Plugins/Tipsy/tipsy.css" rel="stylesheet">
    <script src="Plugins/Tipsy/jquery.tipsy.js"></script>

    <script type="text/javascript">

        function EndRequestHandlerForVideo() {
            var newPoints = $('#<%=UpdatedPointsTextBox.ClientID%>').text();
            if (newPoints != 'N') {

                var oldPoints = $('#MemberBalancesControlPointsLabel').text();

                if (oldPoints != newPoints) {
                    $("#MemberBalancesControlPointsLabel").fadeOut(2000);
                    setTimeout(UpdatePrice, 1900);
                    $("#MemberBalancesControlPointsLabel").fadeIn(2000);
                }

            }
        }

        function UpdatePrice() {
            var newPoints = $('#<%=UpdatedPointsTextBox.ClientID%>').text();
            $('#MemberBalancesControlPointsLabel').text(newPoints); //Update
        }

        jQuery(function ($) {
            $('.video-tooltip').tipsy({ trigger: 'hover', gravity: 's', html: true, opacity: 1.0, live: true });

            $(document).on({
                mouseenter: function () {
                    $(this).parent().find(".video-link-hover").show();
                    $(this).parent().find(".video-link-hover-related").show();
                    $(this).parent().find(".playlist-link-hover").show();
                },
                mouseleave: function () {
                    $(this).parent().find(".video-link-hover").hide();
                    $(this).parent().find(".video-link-hover-related").hide();
                    $(this).parent().find(".playlist-link-hover").hide();
                }
            }, '.darken');

        });

    </script>
    <script type="text/javascript">
        var pageIndex = 1;
        var playlistLoadedPage = 1;
        var pageCount = 1000;
        var isLoadingNewVideos = false;

        $(window).scroll(function () {
            if (($(window).scrollTop() + 200 > $(document).height() - $(window).height() - 250) && isLoadingNewVideos == false) {
                console.log("IN");
                isLoadingNewVideos = true;
                GetRecords();
            }
        });

        function GetRecords() {

            pageIndex++;
            if ((pageIndex == 2 || pageIndex <= pageCount)) {

                $("#loader").show();
                $.ajax({
                    type: "POST",
                    url: "<%= ResolveUrl("~/user/earn/video.aspx/GetCustomers") %>",
                    data: '{pageIndex: ' + pageIndex + '}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: OnSuccess,
                    failure: function (response) {
                        //alert(response.d);
                    },
                    error: function (response) {
                        //alert(response.d);
                    }
                });
            }
        }

        function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }

        function showAllPlaylist() {
            playlistLoadedPage++;
            $("#showVideosButton").hide();
            $("#videosLoader").show();
            $.ajax({
                type: "POST",
                url: "<%= ResolveUrl("~/user/earn/video.aspx/GetAllVideos") %>",
                data: '{pid: "' + getParameterByName("p") + '", page: ' + playlistLoadedPage + ', index: ' + getParameterByName("o") + '}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnSuccessVideos,
                failure: function (response) {
                    //alert(response.d);
                },
                error: function (response) {
                    //alert(response.d);
                }
            });
        }

        function OnSuccess(response) {
            $("#<%=QueryResultAjax.ClientID%>").append(response.d);
            $("#loader").hide();
            isLoadingNewVideos = false;
            console.log("OUT");
        }

        function OnSuccessVideos(response) {
            $("#player-playlist-videos-id").append(response.d);
            $("#videosLoader").hide();
            $("#showVideosButton").show();

            if (strStartsWith(response.d, "<!--NOMORE-->"))
                $("#showVideosButton").hide();
        }

        function strStartsWith(str, prefix) {
            return str.indexOf(prefix) === 0;
        }
    </script>
    <link rel="stylesheet" href="Styles/SearchAndVideoClasses.css?s=1" type="text/css" />
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <h1 class="page-header">Video</h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><asp:Literal runat="server" ID="DescriptionLiteral" /></p>
        </div>
    </div>

    <%--AJAX PART--%>

    <div class="row">
        <div class="col-md-12">
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button ID="ajaxPostbackVideoTrigger" runat="server"
                        OnClick="ajaxPostbackVideoTrigger_Click" Style="display: none;" ClientIDMode="Static" />
                    <asp:Button ID="ajaxPostbackVideoTriggerEnded" runat="server"
                        OnClick="ajaxPostbackVideoTriggerEnded_Click" Style="display: none;" ClientIDMode="Static" />

                    <asp:Label ID="UpdatedPointsTextBox" runat="server" Text="N" Style="visibility: hidden"></asp:Label>

                    <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="alert alert-danger fade in m-b-15">
                        <asp:Literal ID="ErrorMessage" runat="server" Text=""></asp:Literal>
                    </asp:Panel>

                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ajaxPostbackVideoTrigger" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="ajaxPostbackVideoTriggerEnded" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>

    <div class="row">
        <div class="col-md-8">

            <asp:Panel ID="SearchPanel" runat="server" DefaultButton="SearchButton">
                <div class="input-group m-b-20">
                    <asp:TextBox ID="SearchBox" runat="server" CssClass="form-control"></asp:TextBox>
                    <span class="input-group-btn">
                        <asp:LinkButton ID="SearchButton" runat="server" CssClass="btn btn-inverse" OnClick="SearchButton_Click"><%=L1.SEARCH %></asp:LinkButton>
                    </span>
                </div>
            </asp:Panel>

        </div>
    </div>


    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                    <asp:Button ID="Button1" runat="server" Text="DailyMotion" CssClass="ViewSelected" OnClick="MenuButton_Click" CommandArgument="0" />
                    <asp:Button ID="Button2" runat="server" Text="Jun Group" Visible="false" OnClick="MenuButton_Click" CommandArgument="1" />
                </asp:PlaceHolder>
            </div>
        </div>
    </div>

    <div class="tab-content">
        <asp:MultiView ID="MenuMultiView" runat="server" ActiveViewIndex="0">
            <asp:View runat="server">
                <div class="row">

                    <asp:PlaceHolder runat="server" ID="DefaultViewPlaceHolder" Visible="false">
                        <asp:PlaceHolder ID="QueryResultsPlaceHolder" runat="server">
                            <div class="col-md-12">
                                <div class="video-container gallery">
                                    <asp:Literal ID="QueryResultsVideosLiteral" runat="server"></asp:Literal>
                                    <asp:Label ID="QueryResultAjax" runat="server" Text=""></asp:Label>
                                    <span id="loader" style="display: none"><i>Loading...</i></span>
                                </div>
                            </div>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="VideoPlayerPlaceHolder" runat="server">
                            <div class="col-md-8">
                                <asp:Literal ID="VideoPlayerLiteral" runat="server"></asp:Literal>
                            </div>

                            <div class="col-md-4">
                                <div class="row">
                                    <asp:Literal ID="RelatedVideosLiteral" runat="server"></asp:Literal>
                                </div>
                            </div>
                        </asp:PlaceHolder>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder runat="server" ID="WidgetPlaceHolder" Visible="false">
                        <asp:Literal runat="server" ID="WidgetCodeLiteral" />
                    </asp:PlaceHolder>

                </div>
            </asp:View>

            <asp:View ID="View1" runat="server">
                <div class="row">
                    <div class="col-md-12">
                        <script>
                            var testMode = true;
                            var distributorID = '4981090'; // 4981090 = Test, 4981091 = production
                            var userAge = <%=Member.IsLogged?Convert.ToInt32(DateTime.Now.Subtract(Member.CurrentInCache.BirthYear.Value).TotalDays/365):25%>;
                            var userDOB = '<%=Member.IsLogged?Member.CurrentInCache.BirthYear.Value.Year:1980%>-25-01';
                            var userGender = '[GENDER]';
                            if (userGender == 'F') userGender = 'f';
                            else if (userGender == 'M') userGender = 'm';
                            else userGender = '';
                            var chkUrl = "https://live.hyprmx.com/embedded_videos/videos_available?uid=<%=Member.IsLogged?Member.CurrentName:"" %>&dob=" + userDOB + "&gender=" + userGender + "&distributorid=" + distributorID;
                                var iframeUrl = "https://live.hyprmx.com/embedded_videos/player?uid=<%=Member.IsLogged?Member.CurrentName:"" %>&site=RewardStacker&distributorid=" + distributorID + "&rewards[][max_quantity]=1&rewards[][reward_id]=0&rewards[][title]=point&rewards[][value_in_dollars]=0.001";

                                $.get(chkUrl, function (result) {
                                    if (testMode || result == '1') {
                                        var videoIframe = document.getElementById('videoIframe');
                                        videoIframe.src = iframeUrl;
                                        videoIframe.style.display = 'block';
                                    } else {
                                        document.getElementById('noVideos').style.display = 'block';
                                    }
                                });
                        </script>

                        <iframe id="videoIframe" src="" style="width: 100%; height: 800px; display: none;"></iframe>

                        <div id="noVideos" style="display: none;" class="note note-warning">Sorry, there are currently no videos available. Please check back later!</div>
                    </div>
                </div>
            </asp:View>

        </asp:MultiView>
    </div>
</asp:Content>
