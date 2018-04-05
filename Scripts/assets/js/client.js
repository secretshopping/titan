$.noConflict();
jQuery(document).ready(function ($) {

    (function () {
        var css = '.tag{background: #ffff00;}.popover{position:absolute;top:0;left:0;z-index:1060;display:none;max-width:276px;padding:1px;font-family:"Helvetica Neue",Helvetica,Arial,sans-serif;font-size:14px;font-weight:400;line-height:1.42857143;text-align:left;white-space:normal;background-color:#fff;-webkit-background-clip:padding-box;background-clip:padding-box;border:1px solid #ccc;border:1px solid rgba(0,0,0,.2);border-radius:6px;-webkit-box-shadow:0 5px 10px rgba(0,0,0,.2);box-shadow:0 5px 10px rgba(0,0,0,.2)}.popover.top{margin-top:-10px}.popover.right{margin-left:10px}.popover.bottom{margin-top:10px}.popover.left{margin-left:-10px}.popover-title{padding:8px 14px;margin:0;font-size:14px;background-color:#f7f7f7;border-bottom:1px solid #ebebeb;border-radius:5px 5px 0 0}.popover-content{padding:9px 14px}.popover>.arrow,.popover>.arrow:after{position:absolute;display:block;width:0;height:0;border-color:transparent;border-style:solid}.popover>.arrow{border-width:11px}.popover>.arrow:after{content:"";border-width:10px}.popover.bottom>.arrow:after,.popover.left>.arrow:after,.popover.right>.arrow:after,.popover.top>.arrow:after{content:" "}.popover.top>.arrow{bottom:-11px;left:50%;margin-left:-11px;border-top-color:#999;border-top-color:rgba(0,0,0,.25);border-bottom-width:0}.popover.top>.arrow:after{bottom:1px;margin-left:-10px;border-top-color:#fff;border-bottom-width:0}.popover.right>.arrow{top:50%;left:-11px;margin-top:-11px;border-right-color:#999;border-right-color:rgba(0,0,0,.25);border-left-width:0}.popover.right>.arrow:after{bottom:-10px;left:1px;border-right-color:#fff;border-left-width:0}.popover.bottom>.arrow{top:-11px;left:50%;margin-left:-11px;border-top-width:0;border-bottom-color:#999;border-bottom-color:rgba(0,0,0,.25)}.popover.bottom>.arrow:after{top:1px;margin-left:-10px;border-top-width:0;border-bottom-color:#fff}.popover.left>.arrow{top:50%;right:-11px;margin-top:-11px;border-right-width:0;border-left-color:#999;border-left-color:rgba(0,0,0,.25)}.popover.left>.arrow:after{right:1px;bottom:-10px;border-right-width:0;border-left-color:#fff}',
            head = document.head || document.getElementsByTagName('head')[0],
            style = document.createElement('style');

        style.type = 'text/css';
        if (style.styleSheet) {
            style.styleSheet.cssText = css;
        } else {
            style.appendChild(document.createTextNode(css));
        }

        head.appendChild(style);
    })();

    (function (inTextAd) {

        var tagArr = [];

        inTextAd.forEach(function (item) {
            tagArr.push(item.Tag);
        });

        var options = {
            element: 'a',
            className: 'tag',
            "separateWordSearch": false,
            accuracy: { value: 'exacly', limiters: [",", ".", "?", "!", ";", ":", "'", '"', "/"] },
            done: function done() {
                prepareAdds();
            }
        };

        var context = document.querySelector('body');
        var instance = new Mark(context);
        instance.mark(tagArr, options);
    })(inTextAds);

    function prepareAdds() {
        var popoverTemplate = '<div class=\'popover\' role=\'tooltip\'><div class=\'arrow\'></div><h3 class=\'popover-title\'></h3><div class=\'popover-content\'></div></div>';
        $('a.tag').each(function () {
            var elem = $(this);
            var options = {
                template: popoverTemplate,
                html: true,
                trigger: 'hover',
                placement: 'top',
                container: elem
            };

            var keyword = elem.text();
            inTextAds.forEach(function (item) {
                if (keyword.toUpperCase() === item.Tag.toUpperCase()) {
                    elem.data('id', item.Id);
                    elem.prop('href', adUrl + item.Id);
                    elem.attr('target', '_blank');
                    elem.data('toggle', 'popover');
                    elem.attr('title', item.Title);
                    elem.data('content', item.Description);
                }
            });

            elem.popover(options);
        });
    }
});

