/*!@license
* Infragistics.Web.ClientUI Grid localization resources 15.2.20152.2081
*
* Copyright (c) 2011-2015 Infragistics Inc.
*
* http://www.infragistics.com/
*
*/

/*global jQuery */
(function ($) {
$.ig = $.ig || {};

if (!$.ig.Grid) {
	$.ig.Grid = {};

	$.extend($.ig.Grid, {

		locale: {
		    noSuchWidget: "{featureName} was not recognized. Verify that such a feature exists and the spelling is correct.",
		    autoGenerateColumnsNoRecords: "autoGenerateColumns is enabled, but there are no records in the data source. Load a data source with records to be able to determine the columns.",
		    optionChangeNotSupported: "{optionName} cannot be edited after initialization. Its value should be set during initialization.",
		    optionChangeNotScrollingGrid: "{optionName} cannot be edited after initialization because your grid initially does not scroll and full re-rendering will be required. This option should be set during initialization.",
		    noPrimaryKeyDefined: "There is no primary key defined for the grid. Define a primary key in order to use features such as Grid Editing.",
		    indexOutOfRange: "The specified row index is out of range. A row index between 0 and {max} should be provided.",
		    noSuchColumnDefined: "The specified column key is not valid. A column key that matches the key of one of the defined grid columns should be provided.",
		    columnIndexOutOfRange: "The specified column index is out of range. A column index between 0 and {max} should be provided.",
		    recordNotFound: "A record with id {id} could not be found in the data view. Verify the id used for the search and adjust it if necessary.",
		    columnNotFound: "A column with key {key} could not be found. Verify the key used for the search and adjust it if necessary.",
		    colPrefix: "Column ",
		    columnVirtualizationRequiresWidth: "Virtualization and columnVirtualization require the width of the grid or its columns to be set. Provide a value for the grid width, defaultColumnWidth or the width of each column.",
		    virtualizationRequiresHeight: "Virtualization requires the height of the grid to be set. A value for the grid height should be provided.",
		    colVirtualizationDenied: "columnVirtualization requires a different virtualizationMode setting. The virtualizationMode should be set to 'fixed'.",
		    noColumnsButAutoGenerateTrue: "autoGenerateColumns is disabled and no columns are defined for the grid. Either enable autoGenerateColumns or manually specify the columns.",
		    noPrimaryKey: "igHierarchicalGrid requires a primary key to be defined. A primary key should be provided.",
		    expandTooltip: "Expand Row",
		    collapseTooltip: "Collapse Row",
		    featureChooserTooltip: "Feature chooser",
		    movingNotAllowedOrIncompatible: "The specified column could not be moved. Verify that such a column exists and its end position would not break the column layout.",
		    allColumnsHiddenOnInitialization: "All columns cannot be hidden during initialization. At least one column should be configured as visible.",
		    virtualizationNotSupportedWithAutoSizeCols: "Virtualization requires a different column width configuration than '*'. The column width should be set as a number in pixels.",
		    columnVirtualizationNotSupportedWithPercentageWidth: "Column virtualization requires a different grid width configuration. The column width should be set as a number in pixels.",
		    mixedWidthsNotSupported: "All columns are required to have their width set the same way. Set all column widths either as percentages or as number in pixels."
		}
	});

	$.ig.GridFiltering = $.ig.GridFiltering || {};

	$.extend($.ig.GridFiltering, {
		locale: {
		    startsWithNullText: "Starts with...",
		    endsWithNullText: "Ends with...",
		    containsNullText: "Contains...",
		    doesNotContainNullText: "Does not contain...",
		    equalsNullText: "Equals...",
		    doesNotEqualNullText: "Does not equal...",
		    greaterThanNullText: "Greater than...",
		    lessThanNullText: "Less than...",
		    greaterThanOrEqualToNullText: "Greater than or equal to...",
		    lessThanOrEqualToNullText: "Less than or equal to...",
		    onNullText: "On...",
		    notOnNullText: "Not on...",
		    afterNullText: "After",
		    beforeNullText: "Before",
		    emptyNullText: "Empty",
		    notEmptyNullText: "Not empty",
		    nullNullText: "Null",
		    notNullNullText: "Not null",
		    startsWithLabel: "Starts with",
		    endsWithLabel: "Ends with",
		    containsLabel: "Contains",
		    doesNotContainLabel: "Does not contain",
		    equalsLabel: "Equals",
		    doesNotEqualLabel: "Does not equal",
		    greaterThanLabel: "Greater than",
		    lessThanLabel: "Less than",
		    greaterThanOrEqualToLabel: "Greater than or equal to",
		    lessThanOrEqualToLabel: "Less than or equal to",
		    trueLabel: "True",
		    falseLabel: "False",
		    afterLabel: "After",
		    beforeLabel: "Before",
		    todayLabel: "Today",
		    yesterdayLabel: "Yesterday",
		    thisMonthLabel: "This month",
		    lastMonthLabel: "Last month",
		    nextMonthLabel: "Next month",
		    thisYearLabel: "This year",
		    lastYearLabel: "Last year",
		    nextYearLabel: "Next year",
		    clearLabel: "Clear Filter",
		    noFilterLabel: "No",
		    onLabel: "On",
		    notOnLabel: "Not on",
		    advancedButtonLabel: "Advanced",
		    filterDialogCaptionLabel: "ADVANCED FILTER",
		    filterDialogConditionLabel1: "Show records matching ",
		    filterDialogConditionLabel2: " of the following criteria",
		    filterDialogConditionDropDownLabel: "Filtering condition",
		    filterDialogOkLabel: "Search",
		    filterDialogCancelLabel: "Cancel",
		    filterDialogAnyLabel: "ANY",
		    filterDialogAllLabel: "ALL",
		    filterDialogAddLabel: "Add",
		    filterDialogErrorLabel: "You reached the maximum number of filters supported.",
		    filterDialogCloseLabel: "Close filtering dialog",
		    filterSummaryTitleLabel: "Search results",
		    filterSummaryTemplate: "${matches} matching records",
		    filterDialogClearAllLabel: "Clear ALL",
		    tooltipTemplate: "${condition} filter applied",
		    featureChooserText: "Hide Filter",
		    featureChooserTextHide: "Show Filter",
		    featureChooserTextAdvancedFilter: "Advanced Filter",
		    virtualizationSimpleFilteringNotAllowed: "Column virtualization requires a different type of filtering. Set filtering mode to 'advanced' or disable advancedModeEditorsVisible",
		    featureChooserNotReferenced: "A reference to Feature Chooser is missing. Include infragistics.ui.grid.featurechooser.js in your project, use a loader or one of the combined script files.",
		    conditionListLengthCannotBeZero: "The conditionList array in columnSettings is empty. A suitable array for the conditionList should be provided.",
		    conditionNotValidForColumnType: "The condition '{0}' is not valid for the current configuration. It should be replaced with a condition suitable for {1} column type.",
		    defaultConditionContainsInvalidCondition: "defaultExpression for the '{0}' column contains a condition that is not allowed. It should be replaced it with a condition suitable for {0} column type."
		}
	});

	$.ig.GridGroupBy = $.ig.GridGroupBy || {};

	$.extend($.ig.GridGroupBy, {
		locale: {
		    emptyGroupByAreaContent: "Drag a column here or {0} to Group By",
		    emptyGroupByAreaContentSelectColumns: "select columns",
		    emptyGroupByAreaContentSelectColumnsCaption: "select columns",
		    expandTooltip: "Expand Grouped Row",
		    collapseTooltip: "Collapse Grouped Row",
		    removeButtonTooltip: "Remove Grouped Column",
		    featureChooserText: "Ungroup By",
		    featureChooserTextHide: "Group By",
		    modalDialogCaptionButtonDesc: "Sort ascending",
		    modalDialogCaptionButtonAsc: "Sort descending",
		    modalDialogCaptionButtonUngroup: "Ungroup",
		    modalDialogGroupByButtonText: "Group By",
		    modalDialogCaptionText: "Add to Group By",
		    modalDialogDropDownLabel: "Showing:",
		    modalDialogClearAllButtonLabel: "Clear ALL",
		    modalDialogRootLevelHierarchicalGrid: "Root",
		    modalDialogDropDownButtonCaption: "Show/Hide",
		    modalDialogButtonApplyText: "Apply",
		    modalDialogButtonCancelText: "Cancel",
		    fixedVirualizationNotSupported: "Group By requires another virtualization setting. The virtualizationMode should be set to 'continuous'.",
		    summaryRowTitle: "Grouping summary row"
		}
	});

	$.ig.GridHiding = $.ig.GridHiding || {};

	$.extend($.ig.GridHiding, {
		locale: {
		    columnChooserDisplayText: "Column Chooser",
		    hiddenColumnIndicatorTooltipText: "Hidden Column(s)",
		    columnHideText: "Hide",
		    columnChooserCaptionLabel: "Column Chooser",
		    columnChooserCloseButtonTooltip: "Close",
		    hideColumnIconTooltip: "Hide",
		    featureChooserNotReferenced: "A reference to Feature Chooser is missing. Include infragistics.ui.grid.featurechooser.js in your project or use one of the combined script files.",
		    columnChooserShowText: "Show",
		    columnChooserHideText: "Hide",
		    columnChooserResetButtonLabel: "Reset",
		    columnChooserButtonApplyText: "Apply",
		    columnChooserButtonCancelText: "Cancel"
		}
	});

		$.ig.GridResizing = $.ig.GridResizing || {};

		$.extend($.ig.GridResizing, {
			locale: {
			    noSuchVisibleColumn: "There is no visible column for the specified key. The showColumn() method should be used on the column before trying to resize it.",
			    resizingAndFixedVirtualizationNotSupported: "Resizing columns requires a different virtualization setting. Use rowVirtualization and set the virtualizationMode to 'continuous'."
			}
		});

	$.ig.GridPaging = $.ig.GridPaging || {};

	$.extend($.ig.GridPaging, {

		locale: {
		    pageSizeDropDownLabel: "Show ",
		    pageSizeDropDownTrailingLabel: "records",
		    nextPageLabelText: "Sonraki",
		    prevPageLabelText: "Önceki",
		    firstPageLabelText: "",
		    lastPageLabelText: "",
		    currentPageDropDownLeadingLabel: "Pg",
		    currentPageDropDownTrailingLabel: "of ${count}",
		    currentPageDropDownTooltip: "Choose page index",
		    pageSizeDropDownTooltip: "Choose number of records per page",
		    pagerRecordsLabelTooltip: "Current records range",
		    prevPageTooltip: "Previous page",
		    nextPageTooltip: "Next page",
		    firstPageTooltip: "First page",
		    lastPageTooltip: "Son Sayfa",
		    pageTooltipFormat: "Page ${index}",
		    pagerRecordsLabelTemplate: "${startRecord} - ${endRecord} of ${recordCount} records",
		    invalidPageIndex: "The specified page index is not valid. Provide a page index that is greater than or equal to 0 and less than the total number of pages."
		}
	});

    $.ig.GridSelection = $.ig.GridSelection || {};

    $.extend($.ig.GridSelection, {
        locale: {
            persistenceImpossible: "Persisting selection requires a different configuration. The primary key option of the grid should be configured."
        }
    });

	$.ig.GridRowSelectors = $.ig.GridRowSelectors || {};

	$.extend($.ig.GridRowSelectors, {

		locale: {
		    selectionNotLoaded: "igGridSelection has not been initialized. Selection should be enabled for the grid.",
		    columnVirtualizationEnabled: "Row Selectors require a different virtualization setting. Use rowVirtualization and set the virtualizationMode to 'continuous'.",
		    selectedRecordsText: "You have selected ${checked} records.",
		    deselectedRecordsText: "You have deselected ${unchecked} records.",
		    selectAllText: "Select all ${totalRecordsCount} records",
		    deselectAllText: "Deselect all ${totalRecordsCount} records",
		    requireSelectionWithCheckboxes: "Selection is required when there are checkboxes enabled"
		}
	});

	$.ig.GridSorting = $.ig.GridSorting || {};

	$.extend($.ig.GridSorting, {
		locale: {
		    sortedColumnTooltipFormat: "Sorted ${direction}",
		    unsortedColumnTooltip: "Sütun Sırala",
		    ascending: "ascending",
		    descending: "descending",
		    modalDialogSortByButtonText: "Sort by",
		    modalDialogResetButton: "Reset",
		    modalDialogCaptionButtonDesc: "Click to sort descending",
		    modalDialogCaptionButtonAsc: "Click to sort ascending",
		    modalDialogCaptionButtonUnsort: "Click to remove sorting",
		    featureChooserText: "Sort on Multiple",
		    modalDialogCaptionText: "Sort on Multiple",
		    modalDialogButtonApplyText: "Apply",
		    modalDialogButtonCancelText: "Cancel",
		    sortingHiddenColumnNotSupport: "The specified column could not be sorted because it is hidden. Use the showColumn() method on it before trying to sort it.",
		    featureChooserSortAsc: "Sort ascending",
		    featureChooserSortDesc: "Sort descending"
		}
	});

	$.ig.GridSummaries = $.ig.GridSummaries || {};

	$.extend($.ig.GridSummaries, {
		locale: {
		    featureChooserText: "Hide Summaries",
		    featureChooserTextHide: "Show Summaries",
		    dialogButtonOKText: "OK",
		    dialogButtonCancelText: "Cancel",
		    emptyCellText: "",
		    summariesHeaderButtonTooltip: "Show/Hide summaries",
		    defaultSummaryRowDisplayLabelCount: "Count",
		    defaultSummaryRowDisplayLabelMin: "Min",
		    defaultSummaryRowDisplayLabelMax: "Max",
		    defaultSummaryRowDisplayLabelSum: "Sum",
		    defaultSummaryRowDisplayLabelAvg: "Avg",
		    defaultSummaryRowDisplayLabelCustom: "Custom",
		    calculateSummaryColumnKeyNotSpecified: "Column key is missing. A column key should be specified to calculate summaries.",
		    featureChooserNotReferenced: "A reference to Feature Chooser is missing. Include infragistics.ui.grid.featurechooser.js in your project or use one of the combined script files."
		}
	});

	$.ig.GridUpdating = $.ig.GridUpdating || {};

	$.extend($.ig.GridUpdating, {
		locale: {
		    doneLabel: "Done",
		    doneTooltip: "Stop editing and update",
		    cancelLabel: "Cancel",
		    cancelTooltip: "Stop editing without updating",
		    addRowLabel: "Add new row",
		    addRowTooltip: "Start adding a new row",
		    deleteRowLabel: "Delete row",
		    deleteRowTooltip: "Delete row",
		    igTextEditorException: "It is currently not possible to update string columns in the grid. ui.igTextEditor should be loaded first.",
		    igNumericEditorException: "It is currently not possible to update numeric columns in the grid. ui.igNumericEditor should be loaded first.",
		    igCheckboxEditorException: "It is currently not possible to update checkbox columns in the grid. ui.igCheckboxEditor should be loaded first.",
		    igCurrencyEditorException: "It is currently not possible to update numeric columns with currency format in the grid. ui.igCurrencyEditor should be loaded first.",
		    igPercentEditorException: "It is currently not possible to update numeric columns with percent format in the grid. ui.igPercentEditor should be loaded first.",
		    igDateEditorException: "It is currently not possible to update date columns in the grid. ui.igDateEditor should be loaded first.",
		    igDatePickerException: "It is currently not possible to update date columns in the grid. ui.igDatePicker should be loaded first.",
		    igComboException: "It is currently not possible to use a combo in the grid. ui.igCombo should be loaded first.",
		    igRatingException: "It is currently not possible to use igRating as an editor in the grid. ui.igRating should be loaded first.",
		    igValidatorException: "It is currently not possible to support validation with the options defined in igGridUpdating. ui.igValidator should be loaded first.",
		    editorTypeCannotBeDetermined: "Updating did not have enough information to properly determine the type of editor to use for column: ",
		    noPrimaryKeyException: "In order to support update operations after a row was deleted, application should define primaryKey in options of igGrid.",
		    hiddenColumnValidationException: "Cannot edit row which has a hidden column with enabled validation.",
		    dataDirtyException: "Grid has pending transactions which may affect rendering of data. To prevent exception, application may enable autoCommit option of igGrid, or it should process dataDirty event of igGridUpdating and return false. While processing that event, application also may do commit() data in igGrid.",
		    recordOrPropertyNotFoundException: "The specified record or property was not found. Verify the criteria for your search and adjust them if necessary.",
		    rowEditDialogCaptionLabel: "Edit row data",
		    excelNavigationNotSupportedWithCurrentEditMode: "Excel Navigation requires a different configuration. editMode should be set to 'cell' or 'row'",
		    columnNotFound: "The specified column key was not found in the visible columns' collection or the specified index was out of range.",
		    rowOrColumnSpecifiedOutOfView: "Editing the specified row or column is currently not possible. It should be in view on the current page and virtualization frame.",
		    editingInProgress: "A row or cell is currently being edited. Another updating procedure cannot start before the current editing is finished.",
		    undefinedCellValue: "Undefined cannot be set as a cell value."
		}
    });

    $.ig.ColumnMoving = $.ig.ColumnMoving || {};

    $.extend($.ig.ColumnMoving, {
        locale: {
            movingDialogButtonApplyText: "Apply",
            movingDialogButtonCancelText: "Cancel",
            movingDialogCaptionButtonDesc: "Move down",
            movingDialogCaptionButtonAsc: "Move up",
            movingDialogCaptionText: "Move Columns",
            movingDialogDisplayText: "Move Columns",
            movingDialogDropTooltipText: "Move here",
            movingDialogCloseButtonTitle: "Close moving dialog",
            dropDownMoveLeftText: "Move left",
            dropDownMoveRightText: "Move right",
            dropDownMoveFirstText: "Move first",
            dropDownMoveLastText: "Move last",
            featureChooserNotReferenced: "A reference to Feature Chooser is missing. Include infragistics.ui.grid.featurechooser.js in your project or use one of the combined script files.",
            movingToolTipMove: "Move",
            featureChooserSubmenuText: "Move To"
        }
    });

    $.ig.ColumnFixing = $.ig.ColumnFixing || {};

    $.extend($.ig.ColumnFixing, {
        locale: {
            headerFixButtonText: "Fix this column",
            headerUnfixButtonText: "Unfix this column",
            featureChooserTextFixedColumn: "Fix column",
            featureChooserTextUnfixedColumn: "Unfix column",
            groupByNotSupported: "Column Fixing requires a different configuration. The Group By functionality should be disabled.",
            virtualizationNotSupported: "Column Fixing requires a different virtualization setting. rowVirtualization should be used instead.",
            columnVirtualizationNotSupported: "Column Fixing requires a different virtualization setting. columnVirtualization should be disabled.",
            columnMovingNotSupported: "Column Fixing requires a different configuration. Column Moving should be disabled.",
            hidingNotSupported: "Column Fixing requires a different configuration. The Hiding functionality should be disabled.",
            hierarchicalGridNotSupported: "igHierarchicalGrid does not support Column Fixing. Column Fixing should be disabled.",
            responsiveNotSupported: "Column Fixing requires a different configuration. The Responsive functionality should be disabled.",
            noGridWidthNotSupported: "Column Fixing requires a different configuration. The grid width should be set in pixels.",
            defaultColumnWidthInPercentageNotSupported: "Column Fixing requires a different configuration. The default column width should be set as a number in pixels.",
            columnsWidthShouldBeSetInPixels: "Column Fixing requires a different column width setting. The width of column with key {key} should be set in pixels.",
            unboundColumnsNotSupported: "Column Fixing requires a different configuration. Unbound Columns should be disabled.",
            excelNavigationNotSupportedWithCurrentEditMode: "Excel Navigation requires a different configuration. editMode should be set to 'cell' or 'row'.",
            initialFixingNotApplied: "Initial fixing could not be applied for column with key: {0}. Reason: {1}",
            setOptionGridWidthException: "Incorrect value for option grid width. When there are fixed columns width of the visible area of unfixed column(s) should be greater that or equal to value of minimalVisibleAreaWidth.",
            internalErrors: {
                none: "Your grid configuration is successful!",
                notValidIdentifier: "The specified column key is not valid. Provide a column key that matches the key of one of the defined grid columns.",
                fixingRefused: "Fixing this column is not currently supported. Unfix another visible column or use the showColumn() method on any hidden unfixed column first.",
                fixingRefusedMinVisibleAreaWidth: "This column cannot be fixed. Its width exceeds the available space for fixing a column in the grid.",
                alreadyHidden: "Fixing/Unfixing this column is currently not possible. The showColumn() method should be used on the column first.",
                alreadyUnfixed: "This column is already unfixed.",
                alreadyFixed: "This column is already fixed.",
                unfixingRefused: "Unfixing this column is currently not possible. The showColumn() method should be used on any hidden fixed column first.",
                targetNotFound: "Target column with key {key} could not be found. Verify the key used for the search and adjust it if necessary."
            }
        }
    });

    $.ig.GridAppendRowsOnDemand = $.ig.GridAppendRowsOnDemand || {};

    $.extend($.ig.GridAppendRowsOnDemand, {
    	locale: {
    	    loadMoreDataButtonText: "Load more data",
    	    appendRowsOnDemandRequiresHeight: "Append Rows On Demand requires a different configuration. The grid height should be set.",
    	    groupByNotSupported: "Append Rows On Demand requires a different configuration. Group By should be disabled.",
    	    pagingNotSupported: "Append Rows On Demand requires a different configuration. Paging should be disabled.",
    	    cellMergingNotSupported: "Append Rows On Demand requires a different configuration. Cell Merging should be disabled.",
    	    virtualizationNotSupported: "Append Rows On Demand requires a different configuration. Virtualization should be disabled."
    	}
    });


    $.ig.igGridResponsive = $.ig.igGridResponsive || {};

    $.extend($.ig.igGridResponsive, {
    	locale: {
    	    fixedVirualizationNotSupported: 'The Responsive functionality requires a different virtualization setting. virtualizationMode should be set to "continuous".'
    	}
    });

    $.ig.igGridMultiColumnHeaders = $.ig.igGridMultiColumnHeaders || {};

    $.extend($.ig.igGridMultiColumnHeaders, {
    	locale: {
    	    multiColumnHeadersNotSupportedWithColumnVirtualization: "Multi-column headers require a different configuration. columnVirtualization should be disabled."
    	}
    });

}
})(jQuery);
