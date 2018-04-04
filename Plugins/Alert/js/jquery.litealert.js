/*!
 * jQuery Lite Alert Plugin v1.0
 * https://github.com/denikeweb/jquery-LiteAlert
 *
 * Copyright 2015 Denis Dragomirik
 * Released under the MIT license
 */
(function( $ ) {
	config = {
		classes : {
			box: 'lite-alert-box',
			item: 'lite-alert-item',
			close: 'lite-alert-item-close',
			header: 'lite-alert-item-header',
			content: 'lite-alert-item-content',
			footer: 'lite-alert-item-footer'
		},
		speed: 300
	};
	hide = function () {
		var jQueryObjItem = $(this).parent ();
		jQueryObjItem.slideUp (config.speed, function () {jQueryObjItem.remove()});
	};
	show = function (headerText, contentText, footerText) {
		if (headerText  == undefined) headerText  = '';
		if (contentText == undefined) contentText = '';
		if (footerText  == undefined) footerText  = '';
		jQueryObjBox = $('.' + config.classes.box);
		if (jQueryObjBox.length == 0) {
			$('body').append ('<div class="' + config.classes.box + '"></div>');
			jQueryObjBox = $('.' + config.classes.box);
		}
		jQueryObjBox.prepend (
			'<div class="' + config.classes.item + '" style="display:none">' +
				'<div class="' + config.classes.close + '"></div>' +
				'<div class="' + config.classes.header + '">'  + headerText  + '</div>' +
				'<div class="' + config.classes.content + '">' + contentText + '</div>' +
				'<div class="' + config.classes.footer + '">'  + footerText  + '</div>' +
			'</div>'
		); //create
		var jQueryObjItem = $('.' + config.classes.item).first();
		jQueryObjItem.slideDown (config.speed);
			jQueryObjItem.children('.' + config.classes.close)
				.on('click', hide);
	};
	var plugin = $.LiteAlert = $.la = function(headerText, contentText, footerText) {
		show (headerText, contentText, footerText);
	};
	var conifig = $.LiteAlertConfig = $.laConfig = function (configObj) {
		config = configObj;
	};
})(jQuery);