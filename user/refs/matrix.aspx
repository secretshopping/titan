<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="matrix.aspx.cs" Inherits="Matrix" %>

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
            height: 500px;
            display: block;
        }

        .nodeLabel {
            margin-top: -15px;
            font-size: 10px;
        }
    </style>
    <asp:PlaceHolder runat="server" ID="MatrixTreeScriptPlaceHolder" >
    <script>
        $(function () {
            var cy = window.cy = cytoscape({
                container: document.getElementById('cy'),

                boxSelectionEnabled: false,
                autounselectify: true,

                layout: {
                    name: 'dagre',
                    spacingFactor: 0.3,
                    rankSep: 120
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

            //cy.nodeHtmlLabel(
            //    [
            //        {
            //            query: 'node',
            //            halign: 'center',
            //            valign: 'top',
            //            halignBox: 'center',
            //            valignBox: 'center',
            //            cssClass: 'nodeLabel',
            //            tpl: function (data) { return '<div>' + 'info' + '</div>' }
            //        }
            //    ]
            //);

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
            
            //console.log(data);
            
            cy.on('mouseover', 'node', function (evt) {
                $('html,body').css('cursor', 'pointer');                
            });

            cy.on('mouseout', 'node', function (evt) {
                $('html,body').css('cursor', 'pointer');
            });

            cy.$("node[status='occupied']").qtip({
                content: {
                    text: function ()
                    {
                        var info = '<b>' + this._private.data.name + '</b><br/>';
                        info += '<div style="font-size:smaller"><img src="' + this._private.data.flag + '"/> ' + this._private.data.countryName + '<br/>';
                        if ('<%= AppSettings.TitanFeatures.LeaderShipSystemEnabled.ToString() %>' == 'True')
                        {
                            info += '<%= U6005.RANK %>:' + this._private.data.rank + '<br/>';
                        }
                      
                        info += '<%= U6012.SPONSOR %>: ' + this._private.data.sponsor + '<br/>';

                        if ('<%= (ShowPoints && !PointsOnEdges).ToString() %>' == 'True')
                        {
                            info += '<%= String.Format(U6012.LEFT_POINTS, AppSettings.PointsName) %>: ' + this._private.data.leftPoints + '<br/>';
                            info += '<%= String.Format(U6012.RIGHT_POINTS, AppSettings.PointsName) %>: ' + this._private.data.rightPoints + '<br/>';
                        }

                        if (this._private.data.areNotFriends)
                        {
                            info += '<br/><a href="/user/network/profile.aspx?userId=' + this._private.data.userId + '&addfriend=1" class="btn btn-primary"><%= U6000.ADDFRIEND %></a>';
                        }
                        info += '</div>';

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

    <asp:PlaceHolder ID="AllocateReferralScriptsPlaceHolder" runat="server">
        <script>
        $(function () {
  

            var data = <%=UsersToAssign.ToString() %>;
            var activeNode = {};
            var activeUser = data[0];

            var $selectBox = $("<select class='form-control' id=\"selectId\" name=\"selectName\" />");
            for (var val in data) {
                $("<option />", { value: data[val], text: data[val] }).appendTo($selectBox);
            }

            
            $selectBox.on('change', function () {
                activeUser = this.value;
            })

            var $selectBoxWrapper = $("<div class='input-group selectBoxWrapper'></div>");
            $selectBoxWrapper.append($selectBox);

            var $confirmButton = $('<button class="btn btn-inverse m-t-10" id="matrixConfirmButton" type="button"><%=U6007.ASSIGN %> <i id="spinnerIcon" style="display: none;" class="fa fa-spinner fa-pulse fa-fw"></i></button>');
            $selectBoxWrapper.append($confirmButton);
            $confirmButton.click(function () {
                var nodeData = {};
                nodeData.nodeId = activeNode.id();
                nodeData.user = activeUser;
                //console.log(nodeData);
                $.ajax({
                    type: "POST",
                    url: "<%= ResolveUrl("~/user/refs/matrix.aspx/AssignUser") %>",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify(nodeData),
                    beforeSend: function () {
                        $('body').find("#spinnerIcon").show();
                    },
                    cache: false,
                    success: function (html) {
                        window.location.reload(true)
                    }
                });
            });
        
           cy.$("node[status = 'free']").qtip({
                content: {
                    text: $selectBoxWrapper
                },
                position: {
                    my: 'top center',
                    at: 'bottom center'
                },
                style: {
                    classes: 'qtip-bootstrap',
                    tip: {
                        width: 16,
                        height: 8
                    }
                },
                hide: {
                    event: 'unfocus'
                }
            });

            cy.on('click', 'node[status = "free"]', function (evt) {
                activeNode = this;
            });
        });
        </script>
    </asp:PlaceHolder>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">
    <h1 class="page-header"><%=U6003.MATRIX %></h1>

    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=U6007.MATRIXINFO %></p>
        </div>
    </div>

    <asp:PlaceHolder runat="server" ID="NotInMatrixPlaceHolder" >
        <div class="tab-content">
            <br />
            <br />
            <br />
            <br />
            <br />
            <div class="text-center">
                <div style="font-size: 72px">:(</div>
                <h2>You are not in the matrix</h2>
            </div>
            <br />
            <br />
            <br />
            <br />
            <br />
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="MatrixTreePlaceHolder" >
        <asp:PlaceHolder ID="UnassignedReferralsPlaceHolder" runat="server">
            <div class="row">
                <div class="col-md-12">
                    <div class="alert alert-info">
                        <asp:Literal ID="UnassignedMatrixReferralsLiteral" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>

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
            <asp:PlaceHolder runat="server" ID="AssignButtonsPlaceHolder">
                <div class="row">
                    <div class="col-md-3 ">
                        <div class="form-group">
                            <asp:DropDownList runat="server" ID="AssignLeftDropList" CssClass="form-control m-20"
                                DataTextField="Name" DataValueField="Id"></asp:DropDownList>
                        </div>
                        <asp:LinkButton runat="server" class="btn btn-primary btn-block m-20" OnClick="AssignFirstLeft_Click">Assigne user to first left</asp:LinkButton>
               
                    </div>
                    <div class="col-md-offset-6 col-md-3">
                        <div class="form-group">
                            <asp:DropDownList runat="server" ID="AssignRightDropList" CssClass="form-control m-20"
                                DataTextField="Name" DataValueField="Id"></asp:DropDownList>
                        </div>
                        <asp:LinkButton runat="server" class="btn btn-primary btn-block m-20" OnClick="AssignFirstRight_Click">Assigne user to first right</asp:LinkButton>
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </asp:PlaceHolder>

</asp:Content>
