/*!@license
 * Infragistics.Web.ClientUI Tree Grid 15.2.20152.2081
 *
 * Copyright (c) 2011-2015 Infragistics Inc.
 *
 * http://www.infragistics.com/
 *
 * Depends on:
 *	jquery-1.4.4.js
 *	jquery.ui.core.js
 *	jquery.ui.widget.js
 *	infragistics.dataSource.js
 *	infragistics.ui.shared.js
 *	infragistics.ui.treegrid.js
 *	infragistics.util.js
 *	infragistics.ui.grid.framework.js
 *	infragistics.ui.grid.updating.js
 */
if(typeof jQuery!=="function"){throw new Error("jQuery is undefined")}(function($){$.widget("ui.igTreeGridUpdating",$.ui.igGridUpdating,{options:{enableAddRow:false},_removeChildrenFromUI:function($row,dataLevel){if(!$row||!$row.length){return}var dl,$tmp,rowId,$fRow,fixedCols=this.grid.hasFixedColumns();if(isNaN(dataLevel)){return}while($row.length===1){dl=parseInt($row.attr("aria-level"),10);if(isNaN(dl)){break}if(dl<=dataLevel){break}$tmp=$row;rowId=$row.attr("data-id");$row=$row.next("tr");if(fixedCols){$fRow=this.container().find("tr[data-id="+rowId+"]");$fRow.remove()}$tmp.remove()}},_updateParentRowAfterDelete:function($pRow,dataLevel){var dl,found,rowId,children,ds=this.grid.dataSource,primaryKeyCol,rec;if(isNaN(dataLevel)||dataLevel<=0){return}while($pRow.length===1){dl=parseInt($pRow.attr("aria-level"),10);if(isNaN(dl)){break}if(dl<dataLevel){found=true;break}$pRow=$pRow.prev("tr")}if(found){rowId=$pRow.attr("data-id");primaryKeyCol=this.grid.columnByKey(this.grid.options.primaryKey);if(primaryKeyCol.dataType==="number"||primaryKeyCol.dataType==="numeric"){rec=ds.findRecordByKey(parseInt(rowId,10))}else{rec=ds.findRecordByKey(rowId)}if(rec){children=rec[this.grid.options.childDataKey];if(children&&children.length===0){if(this.grid.hasFixedColumns()){$pRow=this.container().find("tr[data-id="+rowId+"]")}$pRow.find("span[data-expandcell-indicator]").empty();$pRow.find("td[data-expand-cell]").removeAttr("data-expand-cell")}}}},_generatePrimaryKeyValue:function(e,col){var value,ds=this.grid.dataSource;if(col){value=Math.max(this._recCount||1,ds._totalRecordsCount||1,ds._data.length);if(this.element.find("tr[data-id="+value+"]").length){value++}this._recCount=value;col.value=value}},_attachEvents:function(){this._generatePrimaryKeyValueHandler=$.proxy(this._generatePrimaryKeyValue,this);this.element.bind("igtreegridupdatinggenerateprimarykeyvalue",this._generatePrimaryKeyValueHandler)},_detachEvents:function(){if(this._generatePrimaryKeyValueHandler){this.element.unbind("igtreegridupdatinggenerateprimarykeyvalue",this._generatePrimaryKeyValueHandler);delete this._generatePrimaryKeyValueHandler}},_create:function(){this.element.data($.ui.igGridUpdating.prototype.widgetName,this.element.data($.ui.igTreeGridUpdating.prototype.widgetName));$.ui.igGridUpdating.prototype._create.apply(this,arguments)},_deleteRow:function(evt,rowId,suppress){var res,$prevRow,$nextRow,$row=this.grid.rowById(rowId),dataLevel;if($row&&$row.length){$nextRow=$row.next("tr");$prevRow=$row.prev("tr");dataLevel=parseInt($row.attr("aria-level"),10)}res=this._super(evt,rowId,suppress);if(res&&!isNaN(dataLevel)){this._removeChildrenFromUI($nextRow,dataLevel);this._updateParentRowAfterDelete($prevRow,dataLevel)}return res},destroy:function(){this._detachEvents();$.ui.igGridUpdating.prototype.destroy.apply(this,arguments);this.element.removeData($.ui.igGridUpdating.prototype.widgetName)},_injectGrid:function(grid,isRebind){$.ui.igGridUpdating.prototype._injectGrid.apply(this,arguments);if(!isRebind){this._detachEvents();this._attachEvents()}}});$.extend($.ui.igTreeGridUpdating,{version:"15.2.20152.2081"})})(jQuery);