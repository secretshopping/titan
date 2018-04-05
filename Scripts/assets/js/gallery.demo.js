/*
Template Name: Color Admin - Responsive Admin Dashboard Template build with Twitter Bootstrap 3.3.7
Version: 2.1.0
Author: Sean Ngu
Website: http://www.seantheme.com/color-admin-v2.1/admin/html/
*/

function calculateDivider() {
    var dividerValue = 4;
    if ($(this).width() <= 480) {
        dividerValue = 1;
    } else if ($(this).width() <= 767) {
        dividerValue = 2;
    } else if ($(this).width() <= 980) {
        dividerValue = 3;
    }
    return dividerValue;
}
var handleIsotopesGallery = function(id) {
    //"use strict";
    filters = {};

	    var container = $(id);
	    
		var dividerValue = calculateDivider();
		var containerWidth = $(container).width() - 20;
		var columnWidth = containerWidth / dividerValue;
		$(container).isotope({
			resizable: true,
			masonry: {
				columnWidth: columnWidth
			}
		});
		
		$(window).smartresize(function() {
			var dividerValue = calculateDivider();
            var containerWidth = $(container).width() - 20;
            var columnWidth = containerWidth / dividerValue;
			$(container).isotope({
				masonry: { 
				    columnWidth: columnWidth 
				}
			});
		});
		
		var $optionSets = $('.options .gallery-option-set'),
		$optionLinks = $optionSets.find('a');
		
		//$optionLinks.click( function(){
		//	var $this = $(this);
		//	if ($this.hasClass('active')) {
		//		return false;
		//	}
		//	var $optionSet = $this.parents('.gallery-option-set');
		//	$optionSet.find('.active').removeClass('active');
		//	$this.addClass('active');
		
		//	var options = {};
		//	var key = $optionSet.attr('data-option-key');
		//	var value = $this.attr('data-option-value');
		//		value = value === 'false' ? false : value;
		//		options[ key ] = value;
		//		$(container).isotope(options);
		//		console.log(options);
		//	return false;
	    //});

		$('.options a').click(function (event) {
		    event.preventDefault();
		    var $this = $(this);
		    // don't proceed if already selected
		    if ($this.hasClass('active')) {
		        return;
		    }

		    var $optionSet = $this.parents('.gallery-option-set');
		    // change selected class
		    $optionSet.find('.active').removeClass('active');
		    $this.addClass('active');

		    // store filter value in object
		    // i.e. filters.color = 'red'
		    var group = $optionSet.attr('data-option-group');
		    filters[group] = $this.attr('data-option-value');
		    // convert object into array
		    var isoFilters = [];
		    for (var prop in filters) {
		        if(filters[prop] != "*") {
		            isoFilters.push(filters[prop]);
		        }
            }
		
		    var selector = isoFilters.join('');
		    container.isotope({ filter: selector });

		    return false;
		});



};


var Gallery = function () {
    //"use strict";
    var state = false;
    return {
        //main function
        init: function (id) {
            handleIsotopesGallery(id);
            Gallery.state = true;
        }
    };
}();