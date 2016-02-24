/*!@license
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
	        seriesName: "doit spécifier l'option de nom de série lors de la définition des options.",
	        axisName: "doit spécifier l'option de nom d'axe lors de la définition des options.",
	        invalidLabelBinding: "Il n'y a aucune valeur pour les étiquettes à associer.",
			close: "Fermer",
			overview: "Aperçu",
			zoomOut: "Zoom arrière",
			zoomIn: "Zoom avant",
			resetZoom: "Réinitialiser zoom",
			seriesUnsupportedOption: "the current series type does not support the option: ",
			seriesTypeNotLoaded: "the JavaScript file containing the requested series type has not been loaded or the series type is invalid: ",
			axisTypeNotLoaded: "the JavaScript file containing the requested axis type has not been loaded or the axis type is invalid: ",
			axisUnsupportedOption: "the current axis type does not support the option: "
		}
	});

}
})(jQuery);