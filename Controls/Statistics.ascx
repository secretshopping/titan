<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Statistics.ascx.cs" Inherits="Controls_Statistics" %>

<link href="Scripts/default/assets/plugins/nvd3/build/nv.d3.css" rel="stylesheet" />
<script src="https://cdnjs.cloudflare.com/ajax/libs/d3/3.5.2/d3.min.js"></script>
<script src="Scripts/default/assets/plugins/nvd3/build/nv.d3.js"></script>

<script>

    if (typeof themeColors == 'undefined') {
        themeColors = ['#348fe2', '#5da5e8', '#1993E4',
            '#49b6d6', '#6dc5de', '#3a92ab',
            '#00acac', '#33bdbd', '#008a8a',
            '#f59c1a', '#f7b048', '#c47d15',
            '#2d353c', '#b6c2c9', '#727cb6',
            '#8e96c5', '#5b6392', '#ff5b57'];
    }

    function shuffleArray(array) {
        for (var i = array.length - 1; i > 0; i--) {
            var j = Math.floor(Math.random() * (i + 1));
            var temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
        return array;
    }

    themeColorPalette = shuffleArray(themeColors);

    Array.prototype.getRandom = function (cut) {
        var i = Math.floor(Math.random() * this.length);
        if (cut && i in this) {
            return this.splice(i, 1)[0];
        }
        return this[i];
    }

    var colors1 = ['#727cb6', '#348fe2', '#49b6d6', '#00acac', '#b6c2c9', '#2d353c', '#068DB3', '#006767'];

    function drawChart(div, data, xName, yName, isInt) {
        var barChartData = [{
            key: 'key',
            values:
            data
        }];

        if (typeof themeColorPalette == 'undefined') {
            themeColorPalette = [];
        }

        nv.addGraph(function () {

            var barChart = nv.models.discreteBarChart()
                .margin({ left: 100 })
                .x(function (d) { return d.label })
                .y(function (d) { return d.value })
                .showValues(true)
                .duration(1000).color(themeColorPalette);

            if (isInt == 'True') {
                barChart.valueFormat(d3.format('d'));
                barChart.yAxis.tickFormat(d3.format("d"));
            } else {
                barChart.tooltip.valueFormatter(function (d) { return d.toFixed(<%=CoreSettings.GetMaxDecimalPlaces()%>) });
                barChart.valueFormat(function (d) { return '' });
                barChart.yAxis.tickFormat(function (d) { return d.toFixed(<%=CoreSettings.GetMaxDecimalPlaces()%>) });
            }

            barChart.yAxis.axisLabel(yName).axisLabelDistance(40);
            barChart.xAxis.axisLabel(xName);

            d3.select('#' + div).append('svg').datum(barChartData).call(barChart);
            nv.utils.windowResize(barChart.update);

            return barChart;
        });
    }
</script>

<%--For ALL standard chart types --%>
<asp:PlaceHolder ID="StandardChartPlaceHolder" runat="server">
    <script>
        $(function () {
            drawChart('<%=StatisticsID%>', <%=GetChartData() %>, '<%=GetxAxisName() %>', '<%=GetyAxisName() %>', '<%=IsInt %>');
        });
    </script>
</asp:PlaceHolder>

<%--For types: UserBalancesPercents, CountriesWithMembers --%>
<asp:PlaceHolder ID="CustomChartPlaceHolder" runat="server" Visible="false">
    <link href="Scripts/default/assets/plugins/morrisjs/morris.css" rel="stylesheet" />
    <script src="Scripts/default/assets/plugins/morrisjs/morris.min.js"></script>
    <script>
        $(function () {
            var rawData = <%=GetChartData() %>;
            var donut = Morris.Donut({
                element: '<%=StatisticsID %>',
                data: rawData,
                colors: ['rgb(233, 30, 99)', 'rgb(0, 188, 212)', 'rgb(255, 152, 0)', 'rgb(0, 150, 136)', 'rgb(96, 125, 139)'],
                formatter: function (y) {
                    if ('<%=AppSettings.Site.IsCurrencySignBefore %>' == 'True')
                        return '<%=AppSettings.Site.CurrencySign %>' + y;
                    else
                        return y + '<%=AppSettings.Site.CurrencySign %>';
                }
            });

            $(window).on('resize', function () { donut.redraw() });
        });
    </script>
</asp:PlaceHolder>

<asp:PlaceHolder ID="MapChartPlaceholder" runat="server" Visible="false">
    <link href="Scripts/default/assets/plugins/jvectormap/jquery-jvectormap-1.2.2.css" rel="stylesheet" />
    <script src="Scripts/default/assets/plugins/jvectormap/jquery-jvectormap-1.2.2.min.js"></script>
    <script src="Scripts/default/assets/plugins/jvectormap/jquery-jvectormap-world-mill-en.js"></script>
     <script>
         $(function () {
             var markers = <%=VMemberGeolocation.GetRegisteredLastMapJson() %>;

             $('#<%=StatisticsID %>').vectorMap({
                 map: 'world_mill_en',
                 normalizeFunction: 'polynomial',
                 hoverOpacity: 0.7,
                 hoverColor: false,
                 backgroundColor: 'transparent',
                 regionStyle: {
                     initial: {
                         fill: '#343434',
                         stroke: 'rgba(93, 93, 93, 1)',
                         "stroke-width": 0.5,
                         "stroke-opacity": 0.5
                     },
                     hover: {
                         fill: '#242424',
                     },
                     selected: {
                         fill: 'yellow'
                     },
                     selectedHover: {}
                 },
                 markerStyle: {
                     initial: {
                         fill: '#00acac',
                         stroke: '#00acac'
                     },
                     hover: {
                         "fill-opacity": 0.7,
                         cursor: 'pointer',
                         stroke: '#fff'
                     }
                 },
                 markers: markers,
                 onMarkerLabelShow: labelLayout,
                 onRegionLabelShow: function (event, label, index) {
                     return false;
                 }
             });

             function labelLayout(event, label, index) {
                 label.html(
                     '<div style="padding:5px;"><img src="' + markers[index].avatar + '" style="height:40px; margin-right: 10px;float:left;">' +
                     '<div style="float:left;"><p style="margin: 0;"><b>' + markers[index].username + '</b></p>' +
                     '<p style="background-image:url(' + markers[index].flag + ');background-size:contain;background-repeat:no-repeat;line-height: 10px;padding: 0 0 0 20px;">' + markers[index].country + '</p></div><div>'
                 );
             }
         });
    </script>
</asp:PlaceHolder>

<asp:Placeholder runat="server" ID="DefaultLayoutPlaceholder" >
    <div class="row">
        <div class="col-md-12">
            <div id="<%=StatisticsID %>" class="height-sm"></div>
        </div>
    </div>
</asp:Placeholder>


<asp:Placeholder runat="server" ID="MapLayoutPlaceholder">
    <div class="row">
        <div class="col-md-8">
            <div id="<%=StatisticsID %>" class="jvector-map"></div>
        </div>
        <asp:PlaceHolder runat="server" ID="AdditionalMapStatsPlaceHolder">
            <div class="col-md-2">
                <asp:Literal runat="server" ID="membersStatsLiteral" />
            </div>
            <div class="col-md-2">
                <asp:Literal runat="server" ID="topEarnersStatsLiteral" />
            </div>
        </asp:PlaceHolder>
    </div>
</asp:Placeholder>
