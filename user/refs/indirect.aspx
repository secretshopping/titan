<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="indirect.aspx.cs" Inherits="IndirectReferrals" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    <script src="Scripts/default/assets/plugins/cytoscape/cytoscape.js"></script>
    <script src="Scripts/default/assets/plugins/cytoscape-dagre/dagre.min.js"></script>
    <script src="Scripts/default/assets/plugins/cytoscape-dagre/cytoscape-dagre.js"></script>
    <script src="Scripts/default/assets/plugins/jquery-qtip/jquery.qtip.min.js"></script>
    <link href="Scripts/default/assets/plugins/jquery-qtip/jquery.qtip.min.css" rel="stylesheet" type="text/css">
    <script src="Scripts/default/assets/plugins/cytoscape-qtip/cytoscape-qtip.js"></script>
    <script src="Scripts/default/assets/plugins/cytoscape-html/cytoscape-node-html-label.min.js"></script>
    <style>
        #cy {
            width: 100%;
            height: 60vh;
            display: block;
        }
    </style>

    <asp:PlaceHolder ID="MatrixJavascriptPlaceHolder" runat="server">

    <script>

        $(function () {
            var cy = window.cy = cytoscape({
                container: document.getElementById('cy'),

                boxSelectionEnabled: false,
                autounselectify: true,

                layout: {
                    name: 'dagre',
                    spacingFactor: 0.3, 
                    rankSep: 350
                },

                style: [
                    {
                        selector: 'node',
                        style: {
                            'content': 'data(name)',
                            'text-opacity': 0.5,
                            'text-halign': 'left',
                            'text-valign': 'bottom',
                            'background-color': '#fff',
                            'width': 20,
                            'height': 20,
                            'font-size': 7,
                            'text-rotation': -45,
                            'background-image': 'data(image)',
                            'background-fit': 'cover cover'
                        }
                    },
                    {
                        selector: 'node[name="<%=Member.CurrentInCache.Name %>"]',
                        style: {
                            'text-halign': 'center',
                            'text-valign': 'top',
                            'text-rotation': 0

                        }
                    },
                    {
                        selector: 'edge',
                        style: {
                            'width': 0.5,
                            'target-arrow-shape': 'triangle',
                            'line-color': '#e9e9e9',
                            'target-arrow-color': '#00acac',
                            'label': 'data(label)',
                            'font-size': 7
                        }
                    },
                    {
                        selector: "node[status = 'free']",
                        css: {
                            'border-color': '#C8C8C8',
                            'border-width': 1,
                            'background-color': '#fff',
                            'background-image-opacity': 0.3
                        }
                    },
                    {
                        selector: "edge[status='free']",
                        style: {
                            'line-style': 'dashed'
                        }
                    }
                ],

                elements: {
                    nodes: 
                        <%=Nodes.ToString() %>
                    ,
                    edges: 
                        <%=Edges.ToString() %>
                    },

            });

            cy.fit();

            cy.userPanningEnabled(true);
            cy.userZoomingEnabled(false);

            $('#cyZoomIn').click(function () {
                cy.zoom({
                    level: 1.2 * cy.zoom(),
                    renderedPosition: { x: cy.width() / 2, y: cy.height() / 2 }
                }); 
            });
            
            $('#cyZoomOut').click(function () {
                cy.zoom({
                    level: 0.8 * cy.zoom(),
                    renderedPosition: { x: cy.width() / 2, y: cy.height() / 2 }
            });
            });

            $('#cyFit').click(function () {
                cy.fit();
            });


            cy.on('mouseover', 'node', function (evt) {
                $('html,body').css('cursor', 'pointer');
            });

            cy.on('mouseout', 'node', function (evt) {
                $('html,body').css('cursor', 'pointer');
            });

            cy.$("node[status='occupied']").qtip({
                content: {
                    text: function () {
                        var info = '<b>' + this._private.data.name + '</b><br/><div style="font-size:smaller">' +
                            '<img src="' + this._private.data.flag + '"/> ' + this._private.data.countryName + '<br/>';
                        if ('<%= AppSettings.TitanFeatures.LeaderShipSystemEnabled.ToString() %>' == 'True') {
                            info = info + '<%= U6005.RANK %>' + ': ' + this._private.data.rank + '</div>'
                        }

                        return info;
                    }
                },
                position: {
                    my: 'bottom center',
                    at: 'top center'
                },
                style: {
                    classes: 'qtip-bootstrap',
                    tip: {
                        width: 16,
                        height: 8
                    }
                },
                show: {
                    event: 'mouseover'
                },
                hide: {
                    event: 'mouseleave mouseout'
                }
            });

            cy.on('click', 'node', function (evt) {

            });
        });

    </script>

    </asp:PlaceHolder>

</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=U3000.INDIRECTREFERRALS %></h1>

    <asp:PlaceHolder ID="NoReferralsAvailablePlaceHolder" runat="server" Visible="false">
        <div class="row">
            <div class="col-md-12">
                <div class="alert alert-info">
                    <%=U6008.NOREFS %>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="ReferralsAvailablePlaceHolder" runat="server">
        <div class="row">
            <div class="col-md-12">
                <div class="alert alert-info">
                <p>
                    <%=L1.DIRECT %> <%=L1.REFERRALS %>: 
                <asp:Label ID="RefCount" runat="server" Font-Bold="true"></asp:Label>
                </p>

                <p>
                    <%=U3000.INDIRECTREFERRALS %>: 
                <asp:Label ID="RefCount2" runat="server" Font-Bold="true"></asp:Label>
                </p>
            </div>
        </div>
        </div>
        <div class="tab-content">

        <div class="row">
            <div class="col-md-3 col-md-offset-9">
                <div class="form-group col-sm-6 m-t-10">
                    <div class="input-group">
                        <div class="input-group-btn">
                            <button type="button" id="cyZoomIn" class="btn btn-lg btn-primary" title="Zoom in"><i class="fa fa-plus" aria-hidden="true"></i></button>
                            <button type="button" id="cyZoomOut" class="btn btn-lg btn-primary" title="Zoom out"><i class="fa fa-minus" aria-hidden="true"></i></button>
                            <button type="button" id="cyFit" class="btn btn-lg btn-default m-l-10" title="Center view"><i class="fa fa-crosshairs" aria-hidden="true"></i></button>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-12">
                <div class="tree" id="cy"></div>
            </div>
        </div>
    </div>
    </asp:PlaceHolder>

</asp:Content>
