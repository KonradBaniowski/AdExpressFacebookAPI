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
			    spinUpperTitle: 'Augmenter',
			    spinLowerTitle: 'Diminuer',
			    buttonTitle: 'Afficher la liste',
			    clearTitle: 'Effacer la valeur',
			    datePickerButtonTitle: 'Afficher le calendrier',
			    updateModeUnsupportedValue: 'L\'option updateMode prend en charge deux valeurs possibles - "onChange" et "immediate"',
			    updateModeNotSupported: 'updateMode property supports only "onchange" mode for igMaskEditor, igDateEditor and igDatePicker extensions',
			    renderErrMsg: "L’éditeur de base ne peut pas être directement instancié. Essayez avec du texte, une donnée numérique, une date ou un autre éditeur.",
			    multilineErrMsg: "textArea requiert une configuration différente. textMode doit être réglé sur « multiline ».",
			    targetNotSupported: "L’élément cible n’est pas pris en charge.",
			    placeHolderNotSupported: "L’attribut de l’espace réservé n’est pas pris en charge par votre navigateur.",
			    allowedValuesMsg: "Choisissez une valeur dans la liste déroulante",
			    maxLengthErrMsg: "La valeur entrée est trop longue et a été réduite à {0} symboles",
			    maxLengthWarningMsg: "Entry reached the maximum length of {0} for this field",
			    minLengthErrMsg: "La saisie doit comporter {0} caractères au minimum",
			    maxValErrMsg: "La valeur entrée a atteint la valeur maximum fixée à {0} pour ce champ",
			    minValErrMsg: "La valeur entrée a atteint la valeur minimum fixée à {0} pour ce champ",
			    maxValExceedRevertErrMsg: "La valeur entrée a atteint la valeur maximum fixée à {0} et est revenue à la valeur antérieure",
			    minValExceedRevertErrMsg: "La valeur entrée a atteint la valeur minimum fixée à {0} et est revenue à la valeur antérieure",
			    maxValExceededWrappedAroundErrMsg: "La valeur entrée a atteint la valeur maximum fixée à {0} et a été fixée à la valeur minimum autorisée",
			    minValExceededWrappedAroundErrMsg: "La valeur entrée a atteint la valeur maximum fixée à {0} et a été fixée à la valeur maximum autorisée",
			    btnValueNotSupported: "Une valeur de bouton différente est requise. Choisissez une valeur entre « dropdown », « clear » et « spin ».",
			    scientificFormatErrMsg: "Un scientificFormat différent est requis. Choisissez une valeur entre « E », « e », « E+ » et « e+ ».",
			    spinDeltaIsOfTypeNumber: "Un type de spinDelta différent est requis. Un nombre positif doit être saisi.",
			    spinDeltaCouldntBeNegative: "L’option spinDelta ne peut pas être négative. Un nombre positif doit être saisi.",
			    spinDeltaContainsExceedsMaxDecimals: "Le nombre maximum de fractions autorisées pour spinDelta est de {0}. Modifiez MaxDecimals ou diminuez la valeur.",
			    spinDeltaIncorrectFloatingPoint: "La virgule flottante spinDelta requiert une configuration différente. Réglez le dataMode de l’éditeur sur « double » ou « float ».",
			    notEditableOptionByInit: "Cette option ne peut pas être modifiée après l’initialisation. Ses valeurs doivent être définies pendant l’initialisation.",
			    numericEditorNoSuchMethod: "L’éditeur numérique ne prend pas cette méthode en charge.",
			    displayFactorIsOfTypeNumber: "displayFactor requiert une valeur différente. Sa valeur doit être un nombre compris entre 1 et 100.",
			    displayFactorAllowedValue: "displayFactor requiert une valeur différente. Sa valeur doit être un nombre compris entre 1 et 100.",
			    instantiateCheckBoxErrMsg: "igCheckboxEditor requiert un élément différent. Choisissez entre l’élément INPUT, SPAN ou DIV.",
			    cannotParseNonBoolValue: "igCheckboxEditor requiert une valeur différente. Une valeur booléenne doit être fournie.",
			    cannotSetNonBoolValue: "igCheckboxEditor requiert une valeur différente. Une valeur booléenne doit être fournie.",
			    maskEditorNoSuchMethod: "L’éditeur de masque ne prend pas cette méthode en charge.",
			    datePickerEditorNoSuchMethod: "L’éditeur de date ne prend pas cette méthode en charge.",
			    datePickerNoSuchMethodDropDownContainer: "The date editor does not support this method. Use 'getCalendar' one instead.",
			    buttonTypeIsDropDownOnly: "Le sélecteur de dates autorise uniquement les valeurs de la liste déroulante ou d’effacement pour l’option buttonType.",
			    dateEditorMinValue: "L’option MinValue ne peut pas être définie lors de l’exécution.",
			    dateEditorMaxValue: "L’option MaxValue ne peut pas être définie lors de l’exécution.",
			    cannotSetRuntime: "Cette option ne peut pas être définie lors de l’exécution",
			    invalidDate: "Date non valide",
			    maskMessage: 'Tous les postes requis doivent être remplis',
			    dateMessage: 'Une date valide doit être saisie',
			    centuryThresholdValidValues: "centuryThreshold property shoud be between 0 and 99. The value has been reverted to defaults."
		    }
	    };
    }
})(jQuery);
