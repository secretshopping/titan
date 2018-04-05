<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WelcomeTourWizard.ascx.cs" Inherits="Controls_WelcomeTourWizard" %>


<!-- Quick Start Guide Modal -->
<div class="modal fade" id="QuickStartGuideModal" tabindex="-1" role="dialog" aria-labelledby="welcomeModalLabel">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header" style="border: none;">
                <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-default" data-dismiss="modal"><%=U4000.CLOSE %></button>
                <button id="endForeverButton" type="button" class="btn btn-default" data-dismiss="modal"><%=U6013.NEVERSHOWAGAIN %></button>
                <button id="confirmQsButton" type="button" class="btn btn-success"><%=U6011.ENTER %></button>
            </div>
        </div>
    </div>
</div>

<script src="Plugins/BootstrapTour/bootstrap-tour.min.js"></script>


<asp:PlaceHolder runat="server" ID="QuickStartGuidePlaceHolder" Visible="false">
    <style>
        .tour .btn.disabled {
            display: none;
        }
    </style>
    <script>
        $(function () {

            $(".tour-tour-element *").click(function (e) { e.preventDefault(); })

            // Instance the tour
            tour = new Tour({
                onEnd: function () {
                    $.ajax({
                        url: 'user/default.aspx',
                        data: { 'QuickStartGuide': 'hide' }
                    });
                },
                storage: window.localStorage,
                backdrop: true,
                steps: <%=GetWelcomeTourJsonString()%>,
                template: "<div class='popover tour'><div class='arrow'></div><h3 class='popover-title'></h3><div class='popover-content'></div><div class='popover-navigation'><button class='btn btn-success m-r-10' data-role='prev'><i class='fa fa-angle-left'></i> Prev</button><button class='btn btn-success' data-role='next'>Next <i class='fa fa-angle-right'></i></button><button class='btn btn-danger' data-role='end'>End tour</button></div></div>"
            });
            // init tour

            if (sessionStorage.getItem('QuickStartGuideModal') != 'hide' && !tour._inited) 
                $('#QuickStartGuideModal').modal({ 'backdrop': true, 'show': true });

            tour.init();
            
            var $modal = $('#QuickStartGuideModal');
            $modal.find('.modal-body').html('<h2 class="text-center m-t-0"><%=String.Format(U6011.QUICKSTARTGUIDE,AppSettings.Site.Name) %>: </h2><h3 class="text-success text-center"><%=U6011.QUICKSTARTGUIDEINFO%></h3>');
            
            let endButton = document.getElementById('endForeverButton');
            endButton.addEventListener("click", function () {
                $('#QuickStartGuideModal').modal('hide');
                sessionStorage.setItem('QuickStartGuideModal', 'hide');
                $.ajax({
                    url: 'user/default.aspx',
                    data: { 'QuickStartGuide': 'hide' }
                });
            });
            
            let confirmButton = document.getElementById('confirmQsButton');
            confirmButton.addEventListener("click", function () {
                $('#QuickStartGuideModal').modal('hide');
                sessionStorage.setItem('QuickStartGuideModal', 'hide');
                tour.restart();
           });
        });
    </script>
</asp:PlaceHolder>
