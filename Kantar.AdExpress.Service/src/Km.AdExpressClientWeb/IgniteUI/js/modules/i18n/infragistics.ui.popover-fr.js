﻿/*!@license
* Infragistics.Web.ClientUI Popover localization resources 15.2.20152.2081
*
* Copyright (c) 2011-2015 Infragistics Inc.
*
* http://www.infragistics.com/
*
*/

/*global jQuery */
(function ($) {
$.ig = $.ig || {};

if (!$.ig.Popover) {
	$.ig.Popover = {};

	$.extend( $.ig.Popover, {
		locale: {
			popoverOptionChangeNotSupported: "La modification de l'option suivante après l'initialisation de FinPopig n'est pas prise en charge :",
			popoverShowMethodWithoutTarget: "Le paramètre target de la fonction show est obligatoire lorsque l'option selectors est utilisée"
		}
	});

}
})(jQuery);