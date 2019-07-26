/*!@license
 * Infragistics.Web.ClientUI Tree Grid 19.1.20
 *
 * Copyright (c) 2011-2019 Infragistics Inc.
 *
 * http://www.infragistics.com/
 *
 * Depends on:
 *	jquery-1.9.1.js
 *	jquery.ui.core.js
 *	jquery.ui.widget.js
 *	infragistics.dataSource.js
 *	infragistics.ui.shared.js
 *	infragistics.ui.treegrid.js
 *	infragistics.util.js
 *	infragistics.ui.grid.framework.js
 *	infragistics.ui.grid.tooltips.js
 */
(function(factory){if(typeof define==="function"&&define.amd){define(["./infragistics.ui.treegrid","./infragistics.ui.grid.tooltips"],factory)}else{return factory(jQuery)}})(function($){$.widget("ui.igTreeGridTooltips",$.ui.igGridTooltips,{options:{inherit:false},_create:function(){this.element.data($.ui.igGridTooltips.prototype.widgetName,this.element.data($.ui.igTreeGridTooltips.prototype.widgetName));$.ui.igGridTooltips.prototype._create.apply(this,arguments)},_getDataView:function(){return this.grid.dataSource.flatDataView()},_getRowIndex:function(element,row){return element.closest("tbody").children("tr:visible:not([data-container='true'],"+"[data-grouprow='true'],"+"[data-new-row='true'])").index(row)},destroy:function(){this._superApply(arguments);this.element.removeData($.ui.igGridTooltips.prototype.widgetName)}});$.extend($.ui.igTreeGridTooltips,{version:"19.1.20"});return $});