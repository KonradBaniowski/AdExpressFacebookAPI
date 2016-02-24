/*!@license
* Infragistics.Web.ClientUI Editors localization resources 15.2.20152.2081
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
			    spinUpperTitle: 'Инкрементирай',
			    spinLowerTitle: 'Декрементирай',
			    buttonTitle: 'Покажи списъка',
			    clearTitle: 'Изчисти стойността',
			    datePickerButtonTitle: 'Покажи календара',
			    updateModeUnsupportedValue: 'Опцията updateMode поддържа две възможни стойности - "onChange" и "immediate"',
			    updateModeNotSupported: 'updateMode property supports only "onchange" mode for igMaskEditor, igDateEditor and igDatePicker extensions',
			    renderErrMsg: "Базовият редактор не може да бъде инстанцииран веднага. Опитайте с текстов, числов, едитор на дни или друг едитор.",
			    multilineErrMsg: 'textArea изисква различна конфигурация. textMode трябва да бъде зададен като "multiline".',
			    targetNotSupported: "Този целеви елемент не е поддържан.",
			    placeHolderNotSupported: "Този елемент контейнер не е поддържан от Вашия браузър.",
			    allowedValuesMsg: "Изберете стойност от падащото меню",
			    maxLengthErrMsg: "Входните данни са твърде дълги, за това бяха съкратени до {0} символа.",
			    maxLengthWarningMsg: "Входните данни достигнаха максимум дължина от {0} за това поле.",
			    minLengthErrMsg: "Трябва да бъдат въведени поне {0} знака.",
			    maxValErrMsg: "Входните данни достигнаха максималната стойност от {0} за това поле.",
			    minValErrMsg: "Входните данни достигнаха минималната стойност от {0} за това поле.",
			    maxValExceedRevertErrMsg: "Входните данни надхвърлиха максималната стойност от {0} и бяха върнат към предишната им стойност.",
			    minValExceedRevertErrMsg: "Входните данни надхвърлиха минималната стойност от {0} и бяха върнат към предишната им стойност.",
			    maxValExceededWrappedAroundErrMsg: "Входните данни надхвърлиха максималната стойност от {0} и бяха върнати към най-ниската им позволена такава.",
			    minValExceededWrappedAroundErrMsg: "Входните данни надхвърлиха максималната стойност от {0} и бяха върнати към най-ниската им позволена такава.",
			    btnValueNotSupported: 'Необходима е различна стойност на бутона. Изберете една от следните стойности: "dropdown", "clear" или "spin".',
			    scientificFormatErrMsg: 'Необходим е различен scientificFormat. Изберете една от следните стойности: "E", "e", "E+" или "e+".',
			    spinDeltaIsOfTypeNumber: "Необходим е различен тип на spinDelta. Трябва да бъде въведено положително число.",
			    spinDeltaCouldntBeNegative: "Опцията spinDelta не може да бъде негативна. Трябва да бъде въведено положително число.",
			    spinDeltaContainsExceedsMaxDecimals: "Максимъмът позволени части на spinDelta е {0}. Променете MaxDecimals или намалете стойността.",
			    spinDeltaIncorrectFloatingPoint: 'spinDelta с плаваща запетая изисква различна конфигурация. Настройте dataMode на редактора, на "double" или "float".',
			    notEditableOptionByInit: "Тази опция не може да бъде променяна след инициализиране. Стойността ѝ да бъде настроена по време на инициализацията.",
			    numericEditorNoSuchMethod: "Числовият едитор не поддържа този метод.",
			    displayFactorIsOfTypeNumber: "displayFactor изисква различна стойност. Стойността му трябва да бъде 1 или 100. ",
			    displayFactorAllowedValue: "displayFactor изисква различна стойност. Стойността му трябва да бъде 1 или 100. ",
			    instantiateCheckBoxErrMsg: "igCheckboxEditor изисква различен елемент. Използвайте  INPUT, SPAN или DIV елемент.",
			    cannotParseNonBoolValue: "igCheckboxEditor изисква различен елемент. Трябва да бъде подадена булева стойност.",
			    cannotSetNonBoolValue: "igCheckboxEditor изисква различен елемент. Трябва да бъде подадена булева стойност.",
			    maskEditorNoSuchMethod: "Редакторът на маски не поддържа този метод.",
			    datePickerEditorNoSuchMethod: "Редакторът на дати не поддържа този метод.",
			    datePickerNoSuchMethodDropDownContainer: "The date editor does not support this method. Use 'getCalendar' one instead.",
			    buttonTypeIsDropDownOnly: "Datepicker позволява само dropdown и чисти стойности за опцията buttonType.",
			    dateEditorMinValue: "Опцията MinValue не може да бъде настроена по време на изпълнение.",
			    dateEditorMaxValue: "Опцията MaxValue не може да бъде настроена по време на изпълнение.",
			    cannotSetRuntime: "Тази опцията не може да бъде настроена по време на изпълнение.",
			    invalidDate: "Невалидна дата",
			    maskMessage: 'Всички задължителни позиции трябва да бъдат попълнени.',
			    dateMessage: 'Трябва да бъде въведена валидна дата.',
			    centuryThresholdValidValues: "centuryThreshold property shoud be between 0 and 99. The value has been reverted to defaults.",
			    noListItemsNoButton: "No spin or dropdown button is rendered because there are no listitems."
		    }
	    };
    }
})(jQuery);