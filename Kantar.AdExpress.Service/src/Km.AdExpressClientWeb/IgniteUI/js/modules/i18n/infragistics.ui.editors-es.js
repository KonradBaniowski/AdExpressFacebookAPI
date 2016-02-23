/*!@license
* Infragistics.Web.ClientUI Editors localization resources 15.2.20152.1027
*
* Copyright (c) 2011-2015 Infragistics Inc.
*
* http://www.infragistics.com/
*
*/

/*global jQuery */
(function ($) {
    $.ig = $.ig || {};

    if (!$.ig.Editor) {
	    $.ig.Editor = {
		    locale: {
			    spinUpperTitle: 'Incrementar',
			    spinLowerTitle: 'Reducir',
			    buttonTitle: 'Mostrar lista',
			    clearTitle: 'Borrar valor',
			    datePickerButtonTitle: 'Mostrar calendario',
			    updateModeUnsupportedValue: 'La opción updateMode admite dos valores posibles: "onChange" e "immediate"',
			    updateModeNotSupported: 'updateMode property supports only "onchange" mode for igMaskEditor, igDateEditor and igDatePicker extensions',
			    renderErrMsg: "No se puede instalar un editor de base directamente. Inténtelo con un editor de texto, numérico, de fecha u otro editor.",
			    multilineErrMsg: 'textArea requiere una configuración diferente. textMode debería ajustarse a "multiline".',
			    targetNotSupported: "Este elemento de origen no se admite.",
			    placeHolderNotSupported: "Su navegador no admite el atributo de campo de comodín.",
			    allowedValuesMsg: "Elija un valor de la lista desplegable.",
			    maxLengthErrMsg: "La entrada es demasiado larga y se ha acortado en {0} símbolos.",
			    maxLengthWarningMsg: "Entry reached the maximum length of {0} for this field",
			    minLengthErrMsg: "Deben introducirse al menos {0} caracteres.",
			    maxValErrMsg: "La entrada ha alcanzado el valor máximo de {0} para este campo.",
			    minValErrMsg: "La entrada ha alcanzado el valor mínimo de {0} para este campo.",
			    maxValExceedRevertErrMsg: "La entrada ha superado el valor máximo de {0} y se ha vuelto a la anterior.",
			    minValExceedRevertErrMsg: "La entrada ha superado el valor mínimo de {0} y se ha vuelto a la anterior.",
			    maxValExceededWrappedAroundErrMsg: "La entrada ha superado el valor máximo de {0} y se ha ajustado al mínimo permitido.",
			    minValExceededWrappedAroundErrMsg: "La entrada ha superado el valor mínimo de {0} y se ha ajustado al máximo permitido.",
			    btnValueNotSupported: 'Se requiere un valor de botón diferente. Elija un valor entre "dropdown", "clear" y "spin".',
			    scientificFormatErrMsg: 'Se requiere un scientificFormat diferente. Elija un valor entre "E", "e", "E+" y "e+".',
			    spinDeltaIsOfTypeNumber: "Se requiere un tipo de spinDelta diferente. Debe introducirse un número positivo.",
			    spinDeltaCouldntBeNegative: "La opción spinDelta no puede ser negativa. Debe introducirse un número positivo.",
			    spinDeltaContainsExceedsMaxDecimals: "El número de fracciones máximo permitido para spinDelta está establecido en {0}. Cambie MaxDecimals o bien intente reducir su valor.",
			    spinDeltaIncorrectFloatingPoint: 'Un punto flotante spinDelta requiere una configuración diferente. Ajuste dataMode del editor a "double" o "float".',
			    notEditableOptionByInit: "Esta opción no puede editarse tras la inicialización. Su valor debe establecerse durante la inicialización.",
			    numericEditorNoSuchMethod: "El editor numérico no admite este método.",
			    displayFactorIsOfTypeNumber: "displayFactor requiere un valor diferente. Su valor debe establecerse con un número entre 1 o 100.",
			    displayFactorAllowedValue: "displayFactor requiere un valor diferente. Su valor debe establecerse con un número entre 1 o 100.",
			    instantiateCheckBoxErrMsg: "igCheckboxEditor requiere un elemento diferente. Utilice los elementos INPUT, SPAN o DIV.",
			    cannotParseNonBoolValue: "igCheckboxEditor requiere un valor diferente. Debe proporcionarse un valor booleano.",
			    cannotSetNonBoolValue: "igCheckboxEditor requiere un valor diferente. Debe proporcionarse un valor booleano.",
			    maskEditorNoSuchMethod: "El editor de máscaras no admite este método.",
			    datePickerEditorNoSuchMethod: "El editor de fechas no admite este método.",
			    datePickerNoSuchMethodDropDownContainer: "The date editor does not support this method. Use 'getCalendar' one instead.",
			    buttonTypeIsDropDownOnly: "Datepicker sólo admite valores de desplegar menú y de borrar para la opción buttonType.",
			    dateEditorMinValue: "La opción MinValue no puede establecer un tiempo de ejecución.",
			    dateEditorMaxValue: "La opción MaxValue no puede establecer un tiempo de ejecución.",
			    cannotSetRuntime: "Esta opción no puede establecer un tiempo de ejecución",
			    invalidDate: "Fecha no válida",
			    maskMessage: 'Deben rellenarse todas las posiciones requeridas.',
			    dateMessage: 'Debe introducirse una fecha válida',
			    centuryThresholdValidValues: "centuryThreshold property shoud be between 0 and 99. The value has been reverted to defaults."
		    }
	    };
    }
})(jQuery);
