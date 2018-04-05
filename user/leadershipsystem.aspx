<%@ Page Language="C#" AutoEventWireup="true" CodeFile="leadershipsystem.aspx.cs" Inherits="user_leadershipsystem" MasterPageFile="~/User.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script type="text/javascript" src="Scripts/gridview.js"></script>
    <style>
        .leadership-system-row[data-current=True] {
            position: relative;
        }

            .leadership-system-row[data-current=True]:before {
                /*border-style: solid;
    border-width: 50px 0 50px 15px;
    border-color: transparent transparent transparent #2d353c;
    position: absolute;
    top: 50%;
    left: 25px;
    margin-top: -50px;
    z-index: 999;*/
            }

        .leadership-system-row[data-accurred=True] .row {
            border-left: 20px solid #00acac;
        }

        .leadership-system-row[data-current=True] .row {
            /*background: #4AC1C1 !important;*/
            color: #fff !important;
        }

        .leadership-system-row[data-current=True] h1,
        .leadership-system-row[data-current=True] h2,
        .leadership-system-row[data-current=True] h3 {
            color: #fff !important;
        }

        .prize-image-wrapper {
            height: 150px;
            padding: 15px;
        }

        .prize-image {
            margin: 0 auto;
            max-height: 120px;
        }

        table.rank-details {
            width: 100%;
        }

            table.rank-details td {
                padding: 5px;
                clear: both;
            }

            table.rank-details b {
                float: right;
            }

        @media (min-width: 992px) {
            .current-rank .leadership-system-row {
                width: 50% !important;
                /*margin-left: 50% !important;*/
            }
        }

        .current-rank .panel-heading {
        }

        .current-rank *:not(.note-info) {
            height: auto !important;
        }

        .current-rank .rank-description,
        .current-rank .prize-image {
            float: left;
        }

        .current-rank .rank-table-wrapper {
            float: left;
            padding: 0 15px;
        }

        .current-rank .progress-wrapper {
            clear: both;
        }

        .current-rank .text-center {
            float: left;
        }

        .current-rank .panel-heading {
            background: #fff !important;
        }

        .current-rank .panel-title {
            color: #242a30;
        }
    </style>

    <script>
        var equalHeightKeeper = function () {
            $('.leadership-system-row .rank-description').css('height', 'auto');
            $('.leadership-system-row .rank-table-wrapper').css('height', 'auto');
            $('.leadership-system-row .progress-wrapper').css('height', 'auto');

            var maxHeightDesc = $('.leadership-system-row .rank-description').height();
            var maxHeightTable = $('.leadership-system-row .rank-table-wrapper').height();
            var maxHeightBody = $('.leadership-system-row .progress-wrapper').height();

            $('.leadership-system-row').each(function () {
                var DescHeight = $(this).find('.rank-description').height();
                var TableHeight = $(this).find('.rank-table-wrapper').height();
                var BodyHeight = $(this).find('.progress-wrapper').height();
                if (DescHeight > maxHeightDesc) {
                    maxHeightDesc = DescHeight;
                }
                if (TableHeight > maxHeightTable) {
                    maxHeightTable = TableHeight;
                }
                if (BodyHeight > maxHeightBody) {
                    maxHeightBody = BodyHeight;
                }
            });

            $('div:not(.current-rank) .leadership-system-row').each(function () {
                $(this).find('.rank-description').css('height', maxHeightDesc);
                $(this).find('.rank-table-wrapper').css('height', maxHeightTable);
                $(this).find('.progress-wrapper').css('height', maxHeightBody);
            });
        };

        var equalHeightKeeperCurrentRank = function () {
            $('.current-rank .note-info').css('height', 'auto');
            $('.current-rank .panel').css('height', 'auto');

            var maxHeight = $('.current-rank .note-info').height();

            if (maxHeight < $('.current-rank .panel').height()) {
                maxHeight = $('.current-rank .panel').height();
            }

            $('.current-rank').each(function () {
                $(this).find('.note-info').css('height', maxHeight);
                $(this).find('.panel').css('height', maxHeight);
            });
        };

        $(function () {
            equalHeightKeeper();
            equalHeightKeeperCurrentRank();
            window.onresize = function () {
                equalHeightKeeper();
                equalHeightKeeperCurrentRank();
            };
        });

    </script>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <h1 class="page-header"><%= AppSettings.LeadershipSystem.Name %></h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%= U6005.LEADERSHIPSYSTEMDESCRIPTION %></p>
        </div>
    </div>
    <div class="row">
        <div class="col-md-12">
            <p>
                <%= AppSettings.LeadershipSystem.Description %>
            </p>
        </div>
    </div>
    <div class="row current-rank">
        <div class="col-md-6">
            <div class="alert note note-info">
                <h2 class="page-header"><%= U6006.YOURSTATISTIC %></h2>
                <asp:Panel ID="AccountDetailsPanel" runat="server">
                    <asp:Literal ID="AccountDetailsLiteral" runat="server" />
                </asp:Panel>
                <br />
                <br />
                <h4>
                    <asp:Label ID="CurrentRankNameLabel" runat="server" /></h4>
            </div>

        </div>

        <titan:LeadershipSytemRow runat="server" ID="CurrentRankControl" />
    </div>
    <div class="row">
    </div>
    <div class="row">
        <asp:Panel ID="RanksPanel" runat="server" />
    </div>

</asp:Content>
