<%@ Page Language="C#" AutoEventWireup="true" CodeFile="internalexchange.aspx.cs" Inherits="user_ico_internalexchange" MasterPageFile="~/User.master" %>

<%@ MasterType VirtualPath="~/User.master" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <style>
        .action-type {
            font-size: 15px;
        }
        .trans {
            display: none !important;
        }
    </style>
    <script type="text/javascript">
        function pageLoad() {

            $("#getLastStockValueButton").click(function () {
                getLastStockValue();
            });

            $("#<%=PlaceOrderButton.ClientID %>").click(function (e) { e.preventDefault(); });

            updatePrice();
            $('#confirmationModal').modal('hide');

            var currentMultiplierControl = document.getElementById('<%=currentMultiplier.ClientID%>');
            if (currentMultiplierControl != null)
            {//CHART CODE
                var graphWidth = document.getElementById("graph").clientWidth;
                var dim = {
                    width: graphWidth, height: 500,
                    margin: { top: 20, right: 100, bottom: 30, left: 100 },
                    ohlc: { height: 450 },
                    indicator: { height: 0, padding: 0 }
                };
                dim.plot = {
                    width: dim.width - dim.margin.left - dim.margin.right,
                    height: dim.height - dim.margin.top - dim.margin.bottom
                };
                dim.indicator.top = dim.ohlc.height + dim.indicator.padding;
                dim.indicator.bottom = dim.indicator.top + dim.indicator.height + dim.indicator.padding;

                var indicatorTop = d3.scaleLinear()
                        .range([dim.indicator.top, dim.indicator.bottom]);

                var parseDate = d3.timeParse("%d-%b-%yT%H:%M");

                var zoom = d3.zoom()
                        .on("zoom", zoomed);

                var x = techan.scale.financetime()
                        .range([0, dim.plot.width]);

                var y = d3.scaleLinear()
                        .range([dim.ohlc.height, 0]);


                var yPercent = y.copy();   // Same as y at this stage, will get a different domain later

                var yInit, yPercentInit, zoomableInit;

                var yVolume = d3.scaleLinear()
                        .range([y(0), y(0.2)]);

                var candlestick = techan.plot.candlestick()
                        .xScale(x)
                        .yScale(y);

                var tradearrow = techan.plot.tradearrow()
                        .xScale(x)
                        .yScale(y)
                        .y(function (d) {
                            // Display the buy and sell arrows a bit above and below the price, so the price is still visible
                            if (d.type === 'buy') return y(d.low) + 5;
                            if (d.type === 'sell') return y(d.high) - 5;
                            else return y(d.price);
                        });

                var sma0 = techan.plot.sma()
                        .xScale(x)
                        .yScale(y);

                var sma1 = techan.plot.sma()
                        .xScale(x)
                        .yScale(y);

                var ema2 = techan.plot.ema()
                        .xScale(x)
                        .yScale(y);

                var volume = techan.plot.volume()
                        .accessor(candlestick.accessor())   // Set the accessor to a ohlc accessor so we get highlighted bars
                        .xScale(x)
                        .yScale(yVolume);

                var trendline = techan.plot.trendline()
                        .xScale(x)
                        .yScale(y);

                var supstance = techan.plot.supstance()
                        .xScale(x)
                        .yScale(y);

                var xAxis = d3.axisBottom(x);

                var timeAnnotation = techan.plot.axisannotation()
                        .axis(xAxis)
                        .orient('bottom')
                        .format(d3.timeFormat('%Y-%m-%d %H:%M'))
                        .width(100)
                        .translate([0, dim.plot.height]);

                var yAxis = d3.axisRight(y);

                var ohlcAnnotation = techan.plot.axisannotation()
                        .axis(yAxis)
                        .orient('right')
                        .format(d3.format(',.4f'))
                        .translate([x(1), 0]);

                var closeAnnotation = techan.plot.axisannotation()
                        .axis(yAxis)
                        .orient('right')
                        .accessor(candlestick.accessor())
                        .format(d3.format(',.4f'))
                        .translate([x(1), 0]);

                var percentAxis = d3.axisLeft(yPercent)
                        .tickFormat(d3.format('+.1%'));

                var percentAnnotation = techan.plot.axisannotation()
                        .axis(percentAxis)
                        .orient('left');

                var volumeAxis = d3.axisRight(yVolume)
                        .ticks(3)
                        .tickFormat(d3.format(",.3s"));

                var volumeAnnotation = techan.plot.axisannotation()
                        .axis(volumeAxis)
                        .orient("right")
                        .width(35);


                var ohlcCrosshair = techan.plot.crosshair()
                        .xScale(timeAnnotation.axis().scale())
                        .yScale(ohlcAnnotation.axis().scale())
                        .xAnnotation(timeAnnotation)
                        .yAnnotation([ohlcAnnotation, percentAnnotation, volumeAnnotation])
                        .verticalWireRange([0, dim.plot.height]);

                var svg = d3.select("#graph").append("svg")
                        .attr("width", dim.width)
                        .attr("height", dim.height);

                var defs = svg.append("defs");

                defs.append("clipPath")
                        .attr("id", "ohlcClip")
                    .append("rect")
                        .attr("x", 0)
                        .attr("y", 0)
                        .attr("width", dim.plot.width)
                        .attr("height", dim.ohlc.height);

                defs.selectAll("indicatorClip").data([0, 1])
                    .enter()
                        .append("clipPath")
                        .attr("id", function (d, i) { return "indicatorClip-" + i; })
                    .append("rect")
                        .attr("x", 0)
                        .attr("y", function (d, i) { return indicatorTop(i); })
                        .attr("width", dim.plot.width)
                        .attr("height", dim.indicator.height);

                svg = svg.append("g")
                        .attr("transform", "translate(" + dim.margin.left + "," + dim.margin.top + ")");

                svg.append('text')
                        .attr("class", "symbol")
                        .attr("x", 20)
                        .text("<%=  ChartTitle  %>");

                        svg.append("g")
                                .attr("class", "x axis")
                                .attr("transform", "translate(0," + dim.plot.height + ")");

                        var ohlcSelection = svg.append("g")
                                .attr("class", "ohlc")
                                .attr("transform", "translate(0,0)");

                        ohlcSelection.append("g")
                                .attr("class", "axis")
                                .attr("transform", "translate(" + x(1) + ",0)")
                            .append("text")
                                .attr("transform", "rotate(-90)")
                                .attr("y", -12)
                                .attr("dy", ".71em")
                                .style("text-anchor", "end")
                                .text("<%=L1.PRICE%> (<%=SignOfPurchaseBalance%>)");

                                ohlcSelection.append("g")
                                        .attr("class", "close annotation up");

                                ohlcSelection.append("g")
                                        .attr("class", "volume")
                                        .attr("clip-path", "url(#ohlcClip)");

                                ohlcSelection.append("g")
                                        .attr("class", "candlestick")
                                        .attr("clip-path", "url(#ohlcClip)");

                                ohlcSelection.append("g")
                                        .attr("class", "indicator sma ma-0")
                                        .attr("clip-path", "url(#ohlcClip)");

                                ohlcSelection.append("g")
                                        .attr("class", "indicator sma ma-1")
                                        .attr("clip-path", "url(#ohlcClip)");

                                ohlcSelection.append("g")
                                        .attr("class", "indicator ema ma-2")
                                        .attr("clip-path", "url(#ohlcClip)");

                                ohlcSelection.append("g")
                                        .attr("class", "percent axis");

                                ohlcSelection.append("g")
                                        .attr("class", "volume axis");

                     
                // Add trendlines and other interactions last to be above zoom pane
                                svg.append('g')
                                        .attr("class", "crosshair ohlc");

                                svg.append("g")
                                        .attr("class", "tradearrow")
                                        .attr("clip-path", "url(#ohlcClip)");

                   
                                svg.append("g")
                                        .attr("class", "trendlines analysis")
                                        .attr("clip-path", "url(#ohlcClip)");
                                svg.append("g")
                                        .attr("class", "supstances analysis")
                                        .attr("clip-path", "url(#ohlcClip)");

                                d3.select("button").on("click", reset);


                //LOAD CHART DATA AND CREATE 
                                var accessor = candlestick.accessor(),
                                    indicatorPreRoll = 0;  // MINIMUM NUMER OF DATA DO DISPLAY CHART

                                var txtdata = qwe = String(document.getElementById('<%=NotVisibleChartData.ClientID%>').value);

                                //txtdata = qwe = txtdata.replaceAll(' ', '\n\r');
                                data = d3.csvParse(txtdata);
                                data = data.map(function (d) {
                                    return {
                                        date: parseDate(d.Date),
                                        open: +d.Open,
                                        high: +d.High,
                                        low: +d.Low,
                                        close: +d.Close,
                                        volume: +d.Volume
                                    };
                                }).sort(function (a, b) { return d3.ascending(accessor.d(a), accessor.d(b)); });

                                x.domain(techan.scale.plot.time(data).domain());
                                y.domain(techan.scale.plot.ohlc(data.slice(indicatorPreRoll)).domain());
                                yPercent.domain(techan.scale.plot.percent(y, accessor(data[indicatorPreRoll])).domain());
                                yVolume.domain(techan.scale.plot.volume(data).domain());
                
                                svg.select("g.candlestick").datum(data).call(candlestick);
                                svg.select("g.close.annotation").datum([data[data.length - 1]]).call(closeAnnotation);
                                svg.select("g.volume").datum(data).call(volume);
                                svg.select("g.sma.ma-0").datum(techan.indicator.sma().period(10)(data)).call(sma0); 
                                svg.select("g.sma.ma-1").datum(techan.indicator.sma().period(20)(data)).call(sma1); 
                                svg.select("g.ema.ma-2").datum(techan.indicator.ema().period(50)(data)).call(ema2); 
                   
                                svg.select("g.crosshair.ohlc").call(ohlcCrosshair).call(zoom);

                                // Stash for zooming
                                zoomableInit = x.zoomable().domain([indicatorPreRoll, data.length]).copy(); // Zoom in a little to hide indicator preroll
                                yInit = y.copy();
                                yPercentInit = yPercent.copy();
                                draw();


                                function reset() {
                                    zoom.scale(1);
                                    zoom.translate([0, 0]);
                                    draw();
                                }

                                function zoomed() {
                                    x.zoomable().domain(d3.event.transform.rescaleX(zoomableInit).domain());
                                    y.domain(d3.event.transform.rescaleY(yInit).domain());
                                    yPercent.domain(d3.event.transform.rescaleY(yPercentInit).domain());

                                    draw();
                                }

                                function draw() {
                                    svg.select("g.x.axis").call(xAxis);
                                    svg.select("g.ohlc .axis").call(yAxis);
                                    svg.select("g.volume.axis").call(volumeAxis);
                                    svg.select("g.percent.axis").call(percentAxis);
                

                                    // We know the data does not change, a simple refresh that does not perform data joins will suffice.
                                    svg.select("g.candlestick").call(candlestick.refresh);
                                    svg.select("g.close.annotation").call(closeAnnotation.refresh);
                                    svg.select("g.volume").call(volume.refresh);
                                    svg.select("g .sma.ma-0").call(sma0.refresh);
                                    svg.select("g .sma.ma-1").call(sma1.refresh);
                                    svg.select("g .ema.ma-2").call(ema2.refresh);
        
                                    svg.select("g.crosshair.ohlc").call(ohlcCrosshair.refresh);
          
                                    svg.select("g.trendlines").call(trendline.refresh);
                                    svg.select("g.supstances").call(supstance.refresh);
                                    svg.select("g.tradearrow").call(tradearrow.refresh);
                                }
                            }//CHART CODE END
                        };

                function updatePrice()
                {
                    var currentMultiplierControl = document.getElementById('<%=currentMultiplier.ClientID%>');
                
                    if(currentMultiplierControl != null)
                    {
                        var isBidOffer = $("#<%=BidButton.ClientID%>").hasClass("IeOfferSelected");
                        var GlobalDecimalPlaces = <%=PurchaseBalanceDecimalPlaces %>;

                        var currentMultiplier = new Decimal(currentMultiplierControl.innerText);
                        var StockAmount = new Decimal($("#<%=NumberOfStockTextBox.ClientID%>").val());
                        var StockValue = new Decimal($("#<%=ValueOfStockTextBox.ClientID%>").val());
                        var finalMultiplier = new Decimal(parseFloat(currentMultiplier));
                        currentMultiplier = currentMultiplier.times(new Decimal(100));
                        var StockGlobalValue = new Decimal(<%=Titan.Cryptocurrencies.CryptocurrencyFactory.Get(Titan.Cryptocurrencies.CryptocurrencyType.BTC).GetOriginalValue("USD").ToDecimal()%>);

                        var totalValue = new Decimal(StockAmount.times(StockValue));
                        var estimatedFee = totalValue.times(finalMultiplier).toFixed(GlobalDecimalPlaces);

                        if(isBidOffer)
                            finalMultiplier = new Decimal(1).sub(finalMultiplier);
                        else
                            finalMultiplier = finalMultiplier.add(1);

                        if(estimatedFee != 0)
                            estimatedFee = estimatedFee.toString().replace(/(\.?0+$)/, '')

                        $('#<%=TotalValueTextBox.ClientID%>').val(totalValue.toFixed(GlobalDecimalPlaces).toString().replace(/(\.?0+$)/, ''));
                        $('#<%=EstimatedFeeLabel.ClientID%>').text("<%=U6013.ESTIMATEDFEE%> ("+currentMultiplier+"%): <%=SignOfPurchaseBalance %>" + estimatedFee);
                        $('#<%=EstimatedValueLabel.ClientID%>').text("<%=U6013.VALUE%>: $" + totalValue.times(finalMultiplier).times(StockGlobalValue).toFixed(GlobalDecimalPlaces).toString().replace(/(\.?0+$)/, '')); 
                    }
                }

                function updatePriceFromTotal() 
                {
                    var currentMultiplierControl = document.getElementById('<%=currentMultiplier.ClientID%>');
                    var isBidOffer = $("#<%=BidButton.ClientID%>").hasClass("IeOfferSelected");
                    var GlobalDecimalPlaces = <%=PurchaseBalanceDecimalPlaces %>;
                
                    if(currentMultiplierControl != null)
                    {
                        var StockValue = new Decimal($("#<%=ValueOfStockTextBox.ClientID%>").val());
                        if (StockValue.eq(0))
                        {
                            StockValue = new Decimal(<%=Titan.ICO.InternalExchangeTransaction.GetLastStockValue() %>);
                            if (StockValue.eq(0)) {
                                StockValue = new Decimal(1);
                            }

                            $('#<%=ValueOfStockTextBox.ClientID%>').val(parseFloat(StockValue.toFixed(GlobalDecimalPlaces)).toString());
                        }

                        var currentMultiplier = new Decimal(document.getElementById('<%=currentMultiplier.ClientID%>').innerText);
                        var finalMultiplier = new Decimal(parseFloat(currentMultiplier).toFixed(GlobalDecimalPlaces));
                        currentMultiplier = currentMultiplier.times(new Decimal(100));
                        var totalValue = new Decimal($("#<%=TotalValueTextBox.ClientID%>").val());

                        var StockGlobalValue = new Decimal(<%=Titan.Cryptocurrencies.CryptocurrencyFactory.Get(Titan.Cryptocurrencies.CryptocurrencyType.BTC).GetOriginalValue("USD").ToDecimal()%>);

                        var estimatedFee = totalValue.times(finalMultiplier).toFixed(GlobalDecimalPlaces);

                        if(isBidOffer)
                            finalMultiplier = new Decimal(1) - finalMultiplier;
                        else
                            finalMultiplier = new Decimal(1) + finalMultiplier;

                        if(estimatedFee != 0)
                            estimatedFee = estimatedFee.toString().replace(/(\.?0+$)/, '')

                        var countedNumberOfStock = totalValue.div(StockValue).toFixed(GlobalDecimalPlaces);

                        $('#<%=NumberOfStockTextBox.ClientID%>').val(countedNumberOfStock.toString());
                        $('#<%=EstimatedFeeLabel.ClientID%>').text("<%=U6013.ESTIMATEDFEE%> ("+currentMultiplier+"%): <%=SignOfPurchaseBalance %>" + estimatedFee);
                        $('#<%=EstimatedValueLabel.ClientID%>').text("<%=U6013.VALUE%>: $" + totalValue.times(finalMultiplier).times(StockGlobalValue));
                    }
                }

            function askForConfirmation(parameter) {

                Page_ClientValidate();
                if (Page_IsValid) {

                    var isBidOffer = $("#<%=BidButton.ClientID%>").hasClass("IeOfferSelected");
                    var typeOfAction = "";
                    if (isBidOffer == true)
                        typeOfAction = '<%=L1.BUY.ToUpper() %>';
                    else
                        typeOfAction = ' <%=U6009.SELL.ToUpper() %>';

                    var StockAmount = $("#<%=NumberOfStockTextBox.ClientID%>").val();
                    var StockValue = $("#<%=ValueOfStockTextBox.ClientID%>").val();
                    var TotalValue = $("#<%=TotalValueTextBox.ClientID%>").val();

                    var StockAmountText = '<%=SignOfStock%>' + parseFloat(StockAmount).toString();
                    var StockValueText = '<%=SignOfPurchaseBalance%>' + parseFloat(StockValue).toString();
                    var TotalValueText = '<%=SignOfPurchaseBalance%>' + parseFloat(TotalValue).toString();

                    var $modal = $('#confirmationModal');
                    $modal.find('.modal-body').html('<h3 class="m-t-0 text-center"><%=U6012.CONFIRMORDER.ToUpper() %></h3><h4 class="text-center"><%=U6012.DOYOUWANTORDER %>:<br /> ' + typeOfAction + ' <span class="text-danger">' + StockAmountText + '</span> with stock value <span class="text-danger">' + StockValueText + '</span><br /><%=U6012.TOTALPRICEWITHFEE%> = <span class="text-danger">' + TotalValueText + '</span></h4>');

                    $('#confirmationModal').modal({ 'backdrop': true, 'show': true });

                    let promise = new Promise(function (resolve, reject) {
                        let confirmButton = document.getElementById('confirmButton');
                        confirmButton.addEventListener("click", function () {
                            $('#confirmationModal').modal('hide');
                            resolve();
                        });
                    });
                    promise.then(function () {
                        __doPostBack(parameter.name, '');
                    });
                }
            }

            function loadBidOffer(rowindex) {
                var row = rowindex.parentNode.parentNode;
                var NumberOfStock = $(row).find(".paramBidAmount").text().replace('<%=SignOfStock %>', '');
                var StockValue = $(row).find(".paramBidValue").text().replace('<%=SignOfPurchaseBalance %>', '');

                $('#<%=NumberOfStockTextBox.ClientID%>').val(parseFloat(NumberOfStock).toString());
                $('#<%=ValueOfStockTextBox.ClientID%>').val(parseFloat(StockValue).toString());
                updatePrice();
            }

            function loadAskOffer(rowindex) {
                var row = rowindex.parentNode.parentNode;
                var NumberOfStock = $(row).find(".paramAskAmount").text().replace('<%=SignOfStock %>', '');
                var StockValue = $(row).find(".paramAskValue").text().replace('<%=SignOfPurchaseBalance %>', '');

                $('#<%=NumberOfStockTextBox.ClientID%>').val(parseFloat(NumberOfStock).toString());
                $('#<%=ValueOfStockTextBox.ClientID%>').val(parseFloat(StockValue).toString());
                updatePrice();
            }

            
            function getLastStockValue() {
                var lastStockValue = <%=Titan.ICO.InternalExchangeTransaction.GetLastStockValue() %>;
                var $lastStockInput = $("#<%=ValueOfStockTextBox.ClientID %>");
                $lastStockInput.val(lastStockValue);
                updatePrice();
            }

    </script>

    <style>
        text {
            fill: #000;
        }

            text.symbol {
                fill: #BBBBBB;
            }

        path {
            fill: none;
            stroke-width: 1;
        }

            path.candle {
                stroke: #000000;
            }

                path.candle.body {
                    stroke-width: 0;
                }

                path.candle.up {
                    fill: #00AA00;
                    stroke: #00AA00;
                }

                path.candle.down {
                    fill: #FF0000;
                    stroke: #FF0000;
                }

        .close.annotation.up path {
            fill: #00AA00;
        }

        path.volume {
            fill: #DDDDDD;
        }

        .indicator-plot path.line {
            fill: none;
            stroke-width: 1;
        }

        .ma-0 path.line {
            stroke: #1f77b4;
        }

        .ma-1 path.line {
            stroke: #aec7e8;
        }

        .ma-2 path.line {
            stroke: #ff7f0e;
        }

        path.signal {
            stroke: #FF9999;
        }

        path.zero {
            stroke: #BBBBBB;
            stroke-dasharray: 0;
            stroke-opacity: 0.5;
        }

        path.difference {
            fill: #BBBBBB;
            opacity: 0.5;
        }

        path.overbought, path.oversold {
            stroke: #FF9999;
            stroke-dasharray: 5, 5;
        }

        path.middle, path.zero {
            stroke: #BBBBBB;
            stroke-dasharray: 5, 5;
        }

        .analysis path, .analysis circle {
            stroke: blue;
            stroke-width: 0.8;
        }

        .trendline circle {
            stroke-width: 0;
            display: none;
        }

        .mouseover .trendline path {
            stroke-width: 1.2;
        }

        .mouseover .trendline circle {
            stroke-width: 1;
            display: inline;
        }

        .dragging .trendline path, .dragging .trendline circle {
            stroke: darkblue;
        }

        .interaction path, .interaction circle {
            pointer-events: all;
        }

        .interaction .body {
            cursor: move;
        }

        .trendlines .interaction .start, .trendlines .interaction .end {
            cursor: nwse-resize;
        }

        .supstance path {
            stroke-dasharray: 2, 2;
        }

        .supstances .interaction path {
            pointer-events: all;
            cursor: ns-resize;
        }

        .mouseover .supstance path {
            stroke-width: 1.5;
        }

        .dragging .supstance path {
            stroke: darkblue;
        }

        .crosshair {
            cursor: crosshair;
        }

            .crosshair path.wire {
                stroke: #DDDDDD;
                stroke-dasharray: 1, 1;
            }

            .crosshair .axisannotation path {
                fill: #DDDDDD;
            }

        .tradearrow path.tradearrow {
            stroke: none;
        }

        .tradearrow path.buy {
            fill: #0000FF;
        }

        .tradearrow path.sell {
            fill: #9900FF;
        }

        .tradearrow path.highlight {
            fill: none;
            stroke-width: 2;
        }

            .tradearrow path.highlight.buy {
                stroke: #0000FF;
            }

            .tradearrow path.highlight.sell {
                stroke: #9900FF;
            }

        .ie-top-stat {
            font-size: 13px;
        }

        .descriptionUnder {
            color: gray;
            font-weight: 100;
        }

        .TabButton{
            width: 49%;
            background: #e2e7eb !important;
            color: #555 !important;
            margin-bottom: -1px;
            z-index: 999999;
            border-radius: 5px;
            font-size: 16px;
            font-weight: 500;
            border: 1px solid #e2e7eb !important;
        }

        .TabButton + .TabButton {
            margin-left: 5px;
        }

        .IeOfferSelected{
            color: #555 !important;
            background: #fff !important;
            border-bottom: 1px solid #fff !important;
        }

        .tab-content-small {
            padding: 25px 15px;
            border-radius: 5px;
            border-top-left-radius: 0;
            border: 1px solid #e2e7eb;
        }

        .nav-tabs-small {
            margin-bottom: -1px;
            width: 100%;
        }

        .left-title{
            float: left;
            padding-left: 10px;
        }

        .right-title{
            float: right;
            padding-right: 10px;
        }

        .ico-description label {
            display: block;
        }

        .low-height{
            line-height: 10px;
        }
    </style>

    <script src="Scripts/default/assets/plugins/d3v4/d3.v4.min.js"></script>
    <script src="Scripts/default/assets/plugins/techanjs/techan.min.js"></script>


    <asp:TextBox ID="NotVisibleChartData" runat="server" TextMode="MultiLine" CssClass="displaynone"></asp:TextBox>

    <h1 class="page-header"><%=U6012.INTERNALEXCHANGE %></h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=String.Format(U6012.INTERNALEXCHANGEDESCRIPTION, AppSettings.Site.Name , SignOfStock, SignOfPurchaseBalance) %></p>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <div class="TitanViewPage">
                    <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server">
                        <asp:Button ID="MainTab" runat="server" CommandArgument="0" CssClass="ViewSelected" />
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>
    <div class="tab-content">
        <asp:Timer ID="UpdateTimer" runat="server" Interval="60000" OnTick="UpdateTimer_Tick"></asp:Timer>
        <asp:UpdatePanel ID="MultiViewUpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <Triggers>
                
            </Triggers>
            <ContentTemplate>
                <asp:Panel ID="SuccessMessagePanel" runat="server" CssClass="alert alert-success fade in m-b-15">
                    <asp:Literal ID="SuccessMessageLiteral" runat="server" />
                </asp:Panel>
                <asp:Panel ID="ErrorMessagePanel" runat="server" CssClass="alert alert-danger fade in m-b-15">
                    <asp:Literal ID="ErrorMessageLiteral" runat="server" />
                </asp:Panel>

                <asp:Label runat="server" ID="currentMultiplier" CssClass="displaynone" />
                <div class="row text-center">

                    <div class="col-md-4 ie-top-stat">
                        <asp:Label runat="server" ID="LastStockValue" />
                    </div>
                    <div class="col-md-4 ie-top-stat">
                        <asp:Label runat="server" ID="LastBidValue" />
                    </div>
                    <div class="col-md-4 ie-top-stat">
                        <asp:Label runat="server" ID="Last24HighValue" />
                    </div>
                    <div class="col-md-4 ie-top-stat">
                        <asp:Label runat="server" ID="Last24Volume" />
                    </div>
                    <div class="col-md-4 ie-top-stat">
                        <asp:Label runat="server" ID="LastAskValue" />
                    </div>
                    <div class="col-md-4 ie-top-stat">
                        <asp:Label runat="server" ID="Last24LowValue" />
                    </div>
                </div>
                <hr />

                <div class="row">
                    <div class="col-sm-12 col-md-7 col-lg-9">
                        <div id="graph"></div>
                    </div>
                    <div class="col-sm-12 col-md-5 col-lg-3">
                        <div class="nav nav-tabs custom bg-white nav-tabs-small" style="border-bottom: 1px solid #e2e7eb !important; display: table;">
                            <asp:Button runat="server" ID="BidButton" CssClass="TabButton" OnClick="BidButton_Click" />
                            <asp:Button runat="server" ID="AskButton" CssClass="TabButton" OnClick="AskButton_Click" />
                        </div>
                        <div class="tab-content-small">
                            <div class="form-group ico-description">
                                <label><asp:label runat="server" ID="NumberOfStockLabel" /></label>
                                <div class="input-group p-b-5">
                                    <span class="add-on input-group-addon" style="min-width: 55px"><%=SignOfStock %></span>
                                    <asp:TextBox ID="NumberOfStockTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <label class="low-height"><asp:Label runat="server" ID="MinmumStockLabel" CssClass="descriptionUnder" /></label>
                                <label class="low-height"><asp:Label runat="server" ID="SellAmountLimitDescriptionTextBox" CssClass="descriptionUnder" /></label>
                                <label class="low-height"><asp:RequiredFieldValidator ID="Amount_RequiredFieldValidator" ControlToValidate="NumberOfStockTextBox" Display="Dynamic" CssClass="text-danger" runat="server" ValidationGroup="AddNewOffer_ValidationGroup" /></label>
                                <label class="low-height"><asp:RangeValidator ID="Amount_RangeValidator" runat="server" ValidationGroup="AddNewOffer_ValidationGroup" ControlToValidate="NumberOfStockTextBox" MinimumValue="0" MaximumValue="999999999" Type="Double" Display="Dynamic" CssClass="text-danger" /></label>
                            </div>
                            <div class="form-group ico-description">
                                <label><asp:label runat="server" ID="PricePerStockLabel" /></label>
                                <div class="input-group p-b-5">
                                    <span class="add-on input-group-addon" style="min-width: 55px"><%=SignOfPurchaseBalance %></span>
                                    <asp:TextBox ID="ValueOfStockTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                                    <a id="getLastStockValueButton" class="add-on input-group-addon btn btn-primary" ><%=U6012.LAST %></a>
                                </div>
                                <label class="low-height"><asp:Label runat="server" ID="MinimalPriceLabel" CssClass="descriptionUnder" /></label>
                                <label class="low-height"><asp:RequiredFieldValidator ID="ValueOfStockRequiredFieldValidator" ControlToValidate="ValueOfStockTextBox" Display="Dynamic" CssClass="text-danger" runat="server" ValidationGroup="AddNewOffer_ValidationGroup" /></label>
                                <label class="low-height"><asp:RangeValidator ID="PricePerStockRangeValidator" runat="server" ValidationGroup="AddNewOffer_ValidationGroup" ControlToValidate="ValueOfStockTextBox" MaximumValue="999999999" Type="Double" Display="Dynamic" CssClass="text-danger" /></label>
                            </div>
                            <div class="form-group ico-description">
                                <label><asp:Label runat="server" ID="TotalValueLabel"/></label>
                                <div class="input-group p-b-5">
                                    <span class="add-on input-group-addon" style="min-width: 55px"><%=SignOfPurchaseBalance %></span>
                                    <asp:TextBox ID="TotalValueTextBox" runat="server" CssClass="form-control" />
                                </div>
                                <label class="low-height"><asp:Label runat="server" ID="EstimatedFeeLabel" CssClass="descriptionUnder" /></label>
                                <label class="low-height"><asp:Label runat="server" ID="EstimatedValueLabel" CssClass="descriptionUnder" Visible="false"/></label>
                                <label class="low-height"><asp:RequiredFieldValidator ID="TotalValueRequiredFieldValidator" ControlToValidate="TotalValueTextBox" Display="Dynamic" CssClass="text-danger" runat="server" ValidationGroup="AddNewOffer_ValidationGroup" /></label>
                                <label class="low-height"><asp:RangeValidator ID="TotalValueRangeValidator" runat="server" ValidationGroup="AddNewOffer_ValidationGroup" ControlToValidate="TotalValueTextBox" MinimumValue="0.000000000000000001" MaximumValue="999999999" Type="Double" Display="Dynamic" CssClass="text-danger" /></label>
                            </div>
                            <div class="form-group">
                                <div class="action-type">
                                    <asp:Button runat="server" ID="PlaceOrderButton" CssClass="btn btn-inverse btn-lg btn-block m-t-30" ValidationGroup="AddNewOffer_ValidationGroup" CausesValidation="true"
                                        OnClientClick="askForConfirmation(this)" OnClick="PlaceOrderButton_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <br />
                <br />
                <div class="row">
                    <div class="col-md-6">
                        <h4 class="left-title"><asp:Label runat="server" ID="LeftTitleBidOffersLabel" /> </h4>
                        <h4 class="right-title"><asp:Label runat="server" ID="RightTitleBidOffersLabel" /></h4>
                        <asp:GridView ID="AllBidOffersGridView" DataKeyNames='<%# new string[] { "BidValue", } %>'
                            AllowPaging="true" AllowSorting="true" DataSourceID="AllBidOffersGridView_DataSource"
                            runat="server" PageSize="10" OnPreRender="BaseGridView_PreRender"
                            OnRowDataBound="AllBidOffersGridView_RowDataBound">
                            <Columns>
                                <asp:BoundField HeaderText="<%$ ResourceLookup : AMOUNT %>" DataField="Summum" />
                                <asp:BoundField HeaderText="<%$ ResourceLookup : PRICE %>" DataField="BidValue" />
                                <asp:BoundField HeaderText="<%$ ResourceLookup : SUM %>" DataField="Value" />
                                <asp:TemplateField HeaderText="">
                                    <ItemStyle Width="13px" />
                                    <ItemTemplate>
                                        <asp:LinkButton ID="LoadBidOfferButton" runat="server" CssClass="btn btn-xs btn-inverse"
                                            OnClientClick="return loadBidOffer(this)" OnClick="AskButton_Click"><span class="fa fa-plus"></span>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="AllBidOffersGridView_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
                            OnInit="AllBidOffersGridView_DataSource_Init" />

                    </div>
                    <div class="col-md-6">
                        <h4 class="left-title"><asp:Label runat="server" ID="LeftTitleAskOffersLabel" /></h4>
                        <h4 class="right-title"><asp:Label runat="server" ID="RightTitleAskOffersLabel" /></h4>
                        <asp:GridView ID="AllAskOffersGridView" DataKeyNames='<%# new string[] { "AskValue", } %>'
                            AllowPaging="true" AllowSorting="true" DataSourceID="AllAskOffersGridView_DataSource"
                            runat="server" PageSize="10" OnPreRender="BaseGridView_PreRender"
                            OnRowDataBound="AllAskOffersGridView_RowDataBound">
                            <Columns>
                                <asp:BoundField HeaderText="<%$ ResourceLookup : AMOUNT %>" DataField="Summum" />
                                <asp:BoundField HeaderText="<%$ ResourceLookup : VALUEOFSTOCK %>" DataField="AskValue" />
                                <asp:BoundField HeaderText="<%$ ResourceLookup : SUM %>" DataField="Value" />
                                <asp:TemplateField HeaderText="">
                                    <ItemStyle Width="13px" />
                                    <ItemTemplate>
                                        <asp:LinkButton ID="loadAskOfferButton" runat="server" CssClass="btn btn-xs btn-inverse"
                                            OnClientClick="return loadAskOffer(this)" OnClick="BidButton_Click"><span class="fa fa-plus"></span>
                                    </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="AllAskOffersGridView_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"
                            OnInit="AllAskOffersGridView_DataSource_Init" />
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
