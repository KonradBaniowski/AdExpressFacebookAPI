/*
* Infragistics.Web.ClientUI common DV widget localization resources 15.2.20152.2081
*
* Copyright (c) 2011-2015 Infragistics Inc.
*
* http://www.infragistics.com/
*
*/

/*global jQuery */
(function ($) {
    $.ig = $.ig || {};

    if (!$.ig.Chart) {
	    $.ig.Chart = {};

	    $.extend($.ig.Chart, {

		    locale: {
			    seriesName: "трябва да попълните series name в зададените от вас опции.",
			    axisName: "трябва да попълните axis name в зададените от вас опции.",
			    invalidLabelBinding: "Не съществува такава стойност, с която да се обвържат етикетите.",
			    close: "Затвори",
			    overview: "Преглед",
			    zoomOut: "Отдалечи",
			    zoomIn: "Увеличи",
			    resetZoom: "Рестартирай увеличението",
			    seriesUnsupportedOption: "the current series type does not support the option: ",
			    seriesTypeNotLoaded: "the JavaScript file containing the requested series type has not been loaded or the series type is invalid: ",
			    axisTypeNotLoaded: "the JavaScript file containing the requested axis type has not been loaded or the axis type is invalid: ",
			    axisUnsupportedOption: "the current axis type does not support the option: "
		    }
	    });

    }
})(jQuery);