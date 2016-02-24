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
			    spinUpperTitle: 'Увеличение',
			    spinLowerTitle: 'Уменьшение',
			    buttonTitle: 'Показать список',
			    clearTitle: 'Удалить значение',
			    datePickerButtonTitle: 'Показать календарь',
			    updateModeUnsupportedValue: 'Опция updateMode поддерживает два возможных значения - "onChange" и "immediate"',
			    updateModeNotSupported: 'updateMode property supports only "onchange" mode for igMaskEditor, igDateEditor and igDatePicker extensions',
			    renderErrMsg: "В данный момент невозможно создать экземпляр основного редактора. Попробуйте текстовый, числовой редактор, редактор дат или другой редактор.",
			    multilineErrMsg: 'Требуется другая конфигурация textArea. Для textMode следует задать значение "multiline".',
			    targetNotSupported: "Данный элемент не поддерживается.",
			    placeHolderNotSupported: "Атрибут заполнителя не поддерживается вашим браузером.",
			    allowedValuesMsg: "Выберите значение из раскрывающегося списка",
			    maxLengthErrMsg: "Введенные данные превышают допустимую длину и были сокращены до {0} символов",
			    maxLengthWarningMsg: "Entry reached the maximum length of {0} for this field",
			    minLengthErrMsg: "Следует ввести, по меньшей мере, {0} символов",
			    maxValErrMsg: "Введенные данные достигли максимального значения {0} для этого поля",
			    minValErrMsg: "Введенные данные достигли минимального значения {0} для этого поля",
			    maxValExceedRevertErrMsg: "Введенные данные превысили максимальное значение {0}, было восстановлено предыдущее значение",
			    minValExceedRevertErrMsg: "Введенные данные меньше минимального значения {0}, было восстановлено предыдущее значение",
			    maxValExceededWrappedAroundErrMsg: "Введенные данные превысили максимальное значение {0}, было установлено минимально допустимое значение",
			    minValExceededWrappedAroundErrMsg: "Введенные данные меньше минимального значение {0}, было установлено максимально допустимое значение",
			    btnValueNotSupported: 'Требуется задать другое значение кнопки. Выберите значение из "dropdown", "clear" и "spin".',
			    scientificFormatErrMsg: 'Требуется другой scientificFormat. Выберите значение из "E", "e", "E+" и "e+".',
			    spinDeltaIsOfTypeNumber: "Требуется другой тип spinDelta. Следует ввести положительное число.",
			    spinDeltaCouldntBeNegative: "Параметр spinDelta не может принимать отрицательное значение. Следует ввести положительное число.",
			    spinDeltaContainsExceedsMaxDecimals: "Максимально допустимая дробная часть для spinDelta установлена в {0}. Измените значение параметра MaxDecimals или попробуйте сократить число.",
			    spinDeltaIncorrectFloatingPoint: 'Требуется другая конфигурация для работы с плавающей запятой spinDelta. Установите значение параметра редактора dataMode "double" или "float".',
			    notEditableOptionByInit: "Нельзя редактировать этот параметр после инициализации. Это значение следует задать в процессе инициализации.",
			    numericEditorNoSuchMethod: "Этот способ не поддерживается числовым редактором.",
			    displayFactorIsOfTypeNumber: "Требуется другое значение параметра displayFactor. Для этого параметра следует задать численное значение от 1 до 100.",
			    displayFactorAllowedValue: "Требуется другое значение параметра displayFactor. Для этого параметра следует задать численное значение от 1 до 100.",
			    instantiateCheckBoxErrMsg: "Требуется другой элемент параметра igCheckboxEditor. Используйте элементы INPUT, SPAN или DIV.",
			    cannotParseNonBoolValue: "Требуется другое значение параметра igCheckboxEditor. Следует указать логическое значение параметра.",
			    cannotSetNonBoolValue: "Требуется другое значение параметра igCheckboxEditor. Следует указать логическое значение параметра.",
			    maskEditorNoSuchMethod: "Этот способ не поддерживается редактором маски.",
			    datePickerEditorNoSuchMethod: "Этот способ не поддерживается редактором дат.",
			    datePickerNoSuchMethodDropDownContainer: "The date editor does not support this method. Use 'getCalendar' one instead.",
			    buttonTypeIsDropDownOnly: "Элемент выбора даты позволяет устанавливать значение параметра buttonType только dropdown и clear.",
			    dateEditorMinValue: "Параметр MinValue не может быть задан во время выполнения программы.",
			    dateEditorMaxValue: "Параметр MaxValue не может быть задан во время выполнения программы.",
			    cannotSetRuntime: "Этот параметр не может быть задан во время выполнения программы",
			    invalidDate: "Неверная дата",
			    maskMessage: 'Следует заполнить все обязательные поля',
			    dateMessage: 'Следует ввести правильную дату',
			    centuryThresholdValidValues: "centuryThreshold property shoud be between 0 and 99. The value has been reverted to defaults.",
			    noListItemsNoButton: "No spin or dropdown button is rendered because there are no listitems."
		    }
	    };
    }
})(jQuery);