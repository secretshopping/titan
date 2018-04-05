<%@ Page Language="C#" MasterPageFile="~/Sites.master" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="_Default" %>

<%@ Register TagPrefix="YAF" Assembly="YAF" Namespace="YAF" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="ContentHeadContent">
    <!--This page uses Tipsy (http://onehackoranother.com/projects/jquery/tipsy) script
    Readme file is located in Plugins/Tipsy/README.txt
    License file is located in Plugins/Tipsy/LICENSE.txt-->
    <link href="Plugins/Tipsy/tipsy.css" rel="stylesheet">
    <script src="Plugins/Tipsy/jquery.tipsy.js"></script>
    <script type="text/javascript">
        jQuery(function ($) {
            $('.trofeum').tipsy({ trigger: 'hover', gravity: 'n', html: true });
        });
    </script>
    <script type="text/javascript">
        //This is 'hack' fixing the jQuery base tag problem
        jQuery(document).ready(function () {

            $.fn.__tabs = $.fn.tabs;
            $.fn.tabs = function (a, b, c, d, e, f) {
                var base = location.href.replace(/#.*$/, '');
                $('ul>li>a[href^="#"]', this).each(function () {
                    var href = $(this).attr('href');
                    $(this).attr('href', base + href);
                });
                $(this).__tabs(a, b, c, d, e, f);
            };
        });
    </script>
    <style type="text/css">
        ul {
            list-style-type: none;
        }
    </style>
</asp:Content>


<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div style="width: 900px; margin: 0 auto; margin-top: 30px">


        <a href="forum/">
            <h1><%= Resources.L1.FORUM %></h1>
        </a>
        <br />

        <YAF:Forum runat="server" ID="forum" BoardID="1" />
    </div>
</asp:Content>
