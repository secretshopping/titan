$(function () {

    $('.tooltip-on').tooltip({
        placement: 'right',
        trigger: 'focus'
    });

    var elems = Array.prototype.slice.call(document.querySelectorAll('.js-switch input'));

    elems.forEach(function (html) {
        var switchery = new Switchery(html);
    });

    /* Extract pagination */
    var paginationList = $('.table').find(".pgr");
    if (paginationList.length) {
        paginationList.each(function () {
            var paginationTable = $(this).find("table");
            var paginationParent = paginationTable.closest(".table");
            var paginationParentId = paginationParent.attr("id");
            paginationParent.parent().append("<div data-parent=\"" + paginationParentId + "\" class=\"pagination\">" + paginationTable.html() + "</div>");
            $(this).remove();
        });
    }
    /* --- */

    /* Detach hidden columns */
    var columnsToDetach = $(".table td.displaynone, .table th.displaynone");
    columnsToDetach.detach();
    /* --- */

    /* AJAX loader */
    Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
    function BeginRequestHandler(sender, args) {
        $(".page-loader").fadeIn();
    }
    function EndRequestHandler(sender, args) {
        $(".page-loader").fadeOut();
    }
    /* --- */

});
