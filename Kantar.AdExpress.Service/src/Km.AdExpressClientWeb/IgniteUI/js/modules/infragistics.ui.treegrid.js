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
 *	infragistics.util.js
 *	infragistics.ui.grid.framework.js
 */
(function(factory){if(typeof define==="function"&&define.amd){define(["./infragistics.ui.grid.framework"],factory)}else{factory(jQuery)}})(function($){/*!@license
* Infragistics.Web.ClientUI Tree Grid localization resources 19.1.20
*
* Copyright (c) 2011-2019 Infragistics Inc.
*
* http://www.infragistics.com/
*
*/
(function($){$.ig=$.ig||{};$.ig.locale=$.ig.locale||{};$.ig.locale.en=$.ig.locale.en||{};$.ig.TreeGridHiding=$.ig.TreeGridHiding||{};$.ig.locale.en.TreeGridHiding=$.extend({},$.ig.locale.en.GridHiding);$.ig.TreeGridResizing=$.ig.TreeGridResizing||{};$.ig.locale.en.TreeGridResizing=$.extend({},$.ig.locale.en.GridResizing);$.ig.TreeGridSelection=$.ig.TreeGridSelection||{};$.ig.locale.en.TreeGridSelection=$.extend({},$.ig.locale.en.GridSelection);$.ig.TreeGridColumnMoving=$.ig.TreeGridColumnMoving||{};$.ig.locale.en.TreeGridColumnMoving=$.extend({},$.ig.locale.en.ColumnMoving);$.ig.TreeGridColumnFixing=$.ig.TreeGridColumnFixing||{};$.ig.locale.en.TreeGridColumnFixing=$.extend({},$.ig.locale.en.ColumnFixing);$.ig.TreeGridMultiColumnHeaders=$.ig.TreeGridMultiColumnHeaders||{};$.ig.locale.en.TreeGridMultiColumnHeaders=$.extend({},$.ig.locale.en.GridMultiColumnHeaders);$.ig.TreeGridSorting=$.ig.TreeGridSorting||{};$.ig.locale.en.TreeGridSorting=$.extend({},$.ig.locale.en.GridSorting);$.ig.TreeGridTooltips=$.ig.TreeGridTooltips||{};$.ig.locale.en.TreeGridTooltips=$.extend({},$.ig.locale.en.GridTooltips);$.ig.TreeGrid=$.ig.TreeGrid||{};$.ig.locale.en.TreeGrid=$.extend({},$.ig.locale.en.Grid,{fixedVirtualizationNotSupported:"Row Virtualization requires a different virtualizationMode setting. The virtualizationMode should be set to 'continuous'."});$.ig.TreeGridPaging=$.ig.TreeGridPaging||{};$.ig.locale.en.TreeGridPaging=$.extend({},$.ig.locale.en.GridPaging,{contextRowLoadingText:"Loading...",contextRowRootText:"Root",columnFixingWithContextRowNotSupported:"Column Fixing requires a different contextRowMode setting. contextRowMode should be set to 'none' in order to enable column fixing."});$.ig.TreeGridFiltering=$.ig.TreeGridFiltering||{};$.ig.locale.en.TreeGridFiltering=$.extend({},$.ig.locale.en.GridFiltering,{filterSummaryInPagerTemplate:"${currentPageMatches} of ${totalMatches} matching records"});$.ig.TreeGridRowSelectors=$.ig.TreeGridRowSelectors||{};$.ig.locale.en.TreeGridRowSelectors=$.extend({},$.ig.locale.en.GridRowSelectors,{multipleSelectionWithTriStateCheckboxesNotSupported:"Multiple selection requires a different checkBoxMode setting. checkBoxMode should be set to biState in order to enable multiple selection."});$.ig.TreeGridUpdating=$.ig.TreeGridUpdating||{};$.ig.locale.en.TreeGridUpdating=$.extend({},$.ig.locale.en.GridUpdating,{addChildButtonLabel:"Add a child row"});$.ig.TreeGrid.locale=$.ig.TreeGrid.locale||$.ig.locale.en.TreeGrid;$.ig.TreeGridHiding.locale=$.ig.TreeGridHiding.locale||$.ig.locale.en.TreeGridHiding;$.ig.TreeGridPaging.locale=$.ig.TreeGridPaging.locale||$.ig.locale.en.TreeGridPaging;$.ig.TreeGridResizing.locale=$.ig.TreeGridResizing.locale||$.ig.locale.en.TreeGridResizing;$.ig.TreeGridSelection.locale=$.ig.TreeGridSelection.locale||$.ig.locale.en.TreeGridSelection;$.ig.TreeGridRowSelectors.locale=$.ig.TreeGridRowSelectors.locale||$.ig.locale.en.TreeGridRowSelectors;$.ig.TreeGridSorting.locale=$.ig.TreeGridSorting.locale||$.ig.locale.en.TreeGridSorting;$.ig.TreeGridColumnMoving.locale=$.ig.TreeGridColumnMoving.locale||$.ig.locale.en.TreeGridColumnMoving;$.ig.TreeGridColumnFixing.locale=$.ig.TreeGridColumnFixing.locale||$.ig.locale.en.TreeGridColumnFixing;$.ig.TreeGridMultiColumnHeaders.locale=$.ig.TreeGridMultiColumnHeaders.locale||$.ig.locale.en.TreeGridMultiColumnHeaders;$.ig.TreeGridFiltering.locale=$.ig.TreeGridFiltering.locale||$.ig.locale.en.TreeGridFiltering;$.ig.TreeGridTooltips.locale=$.ig.TreeGridTooltips.locale||$.ig.locale.en.TreeGridTooltips;$.ig.TreeGridUpdating.locale=$.ig.TreeGridUpdating.locale||$.ig.locale.en.TreeGridUpdating;return $.ig.locale.en})($);var _aNull=function(val){return val===null||val===undefined};$.widget("ui.igTreeGrid",$.ui.igGrid,{css:{containerClasses:"ui-igtreegrid",expandCellExpanded:"ui-icon ui-igtreegrid-expansion-indicator ui-icon-minus",expandCellCollapsed:"ui-icon ui-igtreegrid-expansion-indicator ui-icon-plus",dataSkipCell:"ui-igtreegrid-non-data-column",expandColumn:"ui-igtreegrid-expansion-indicator-cell",dataColumnExpandContainer:"ui-igtreegrid-expansion-indicator-container",expandColumnContainer:"ui-igtreegrid-expansion-column-container",expandContainer:"ui-igtreegrid-expandcell",expandHeaderCell:"ui-igtreegrid-expansion-indicator-header-cell ui-iggrid-header ui-widget-header",rowLevel:"ui-igtreegrid-rowlevel"},options:{indentation:30,initialIndentationLevel:-1,showExpansionIndicator:true,expandTooltipText:null,collapseTooltipText:null,foreignKey:null,initialExpandDepth:-1,foreignKeyRootValue:-1,renderExpansionIndicatorColumn:false,renderFirstDataCellFunction:null,childDataKey:"childData",renderExpansionCellFunction:null,enableRemoteLoadOnDemand:false,dataSourceSettings:{propertyExpanded:null,propertyDataLevel:null,expandedKey:"__ig_options.expanded",dataLevelKey:"__ig_options.dataLevel",initialFlatDataView:false},locale:{expandTooltipText:undefined,collapseTooltipText:undefined},virtualizationMode:"continuous",avgColumnWidth:null,avgRowHeight:25,columnVirtualization:null,restSettings:{}},events:{rowExpanding:"rowExpanding",rowExpanded:"rowExpanded",rowCollapsing:"rowCollapsing",rowCollapsed:"rowCollapsed"},_isHierarchicalDataSource:true,_create:function(){this._checkForUnsoppertedScenarios();var func=this.options.renderFirstDataCellFunction;if(func&&$.type(func)!=="function"){if(window[func]&&typeof window[func]==="function"){func=window[func]}}if(func&&$.type(func)==="function"){this._renderFirstDataCellHandler=func}else{this._renderFirstDataCellHandler=$.proxy(this._renderFirstDataCell,this)}func=this.options.renderExpansionCellFunction;if(func&&$.type(func)!=="function"){if(window[func]&&typeof window[func]==="function"){func=window[func]}}if(func&&$.type(func)==="function"){this._renderExpandCellHandler=func}else{this._renderExpandCellHandler=$.proxy(this._renderExpandCell,this)}this._overrideFunctions();this._attachEvents();this.element.data($.ui.igGrid.prototype.widgetName,this.element.data($.ui.igTreeGrid.prototype.widgetName));this.options.dataSourceSettings.expandedKey=this.options.dataSourceSettings.propertyExpanded||this.options.dataSourceSettings.expandedKey;this.options.dataSourceSettings.dataLevelKey=this.options.dataSourceSettings.propertyDataLevel||this.options.dataSourceSettings.dataLevelKey;$.ui.igGrid.prototype._create.apply(this,arguments);this.element.attr("role","treegrid")},_changeLanguage:function(language){var self=this,i,f,features=this.options.features,featureInstance;this.options.language=language;for(i=0;i<features.length;i++){f=this.options.features[i];if(f&&f.name){featureInstance=this.element.data("igTreeGrid"+f.name)||this.element.data("igGrid"+f.name);if(featureInstance){featureInstance._setOptions({language:language})}}}this.container().find("[data-expand-button]").each(function(){self._changeLocaleForElement($(this))})},_checkForUnsoppertedScenarios:function(){if(this._rowVirtualizationEnabled()&&this.options.virtualizationMode==="fixed"){throw new Error(this._getLocaleValue("fixedVirtualizationNotSupported"))}},_wrapElementDiv:function(){$.ui.igGrid.prototype._wrapElementDiv.apply(this,arguments);this.element.data($.ui.igTreeGrid.prototype.widgetName,this);this._overrideFunctions()},_removeOverridenFunction:function(){if(!this._functionsOverriden){return}var f,funcs=this._functionsOverriden;for(f in funcs){if(funcs.hasOwnProperty(f)){this.element[f]=funcs[f]}}delete this._functionsOverriden},_overrideFunctions:function(){this._overrideFunction("bind");this._overrideFunction("unbind");this._overrideFunction("on");this._overrideFunction("off")},_overrideFunction:function(functionName){var e=this.element,func;if($.type(e[functionName])!=="function"){return}if(!this._functionsOverriden){this._functionsOverriden={}}func=e[functionName];if(!this._functionsOverriden[functionName]){this._functionsOverriden[functionName]=func}e[functionName]=function(name,arg1,arg2){var strIgGrid="iggrid",argsLen=arguments.length,evtName,oEvtName;if($.type(name)==="string"&&name.indexOf(strIgGrid)===0&&name.length>strIgGrid.length&&(argsLen===2||argsLen===3)){name="igtreegrid"+name.substr(strIgGrid.length);if(argsLen===2){return func.call(e,name,arg1)}return func.call(e,name,arg1,arg2)}else if($.type(name)==="object"){for(evtName in name){if(name.hasOwnProperty(evtName)){if($.type(evtName)==="string"&&evtName.indexOf(strIgGrid)===0){oEvtName=evtName;evtName="igtreegrid"+evtName.substr(strIgGrid.length);name[evtName]=name[oEvtName]}}}}return func.apply(e,arguments)}},_updateParentRowAfterDelete:function($pRow,dataLevel){var dl,found,rowId,children,ds=this.dataSource,primaryKeyCol,rec,identation=this.dataSource.getDataBoundDepth();if(isNaN(dataLevel)||dataLevel<=0){return}while($pRow.length===1){dl=parseInt($pRow.attr("aria-level"),10);if(isNaN(dl)){break}if(dl<dataLevel){found=true;break}$pRow=$pRow.prev("tr")}if(found){rowId=$pRow.attr("data-id");primaryKeyCol=this.columnByKey(this.options.primaryKey);if(primaryKeyCol.dataType==="number"||primaryKeyCol.dataType==="numeric"){rec=ds.findRecordByKey(parseInt(rowId,10))}else{rec=ds.findRecordByKey(rowId)}if(rec){children=rec[this.options.childDataKey];if(!children||children.length===0){if(this.hasFixedColumns()){$pRow=this.container().find('tr[data-id="'+rowId+'"]')}$pRow.find("span[data-expandcell-indicator]").empty();$pRow.find("td[data-expand-cell]").removeAttr("data-expand-cell")}}}this._rerenderDataSkipColumn(identation)},rollback:function(rowId,updateUI){var transactions=$.ui.igGrid.prototype.rollback.apply(this,arguments),transaction,i,tridx,tr,prevTr,dataLevel;if(updateUI===true){if(rowId!==null&&rowId!==undefined){if(transactions===null||transactions===undefined||transactions.length===0){return}i=transactions.length;while(i-- >0){transaction=transactions[i];tr=this.element.find("tr[data-id='"+transaction.rowId+"']");if(transaction.type==="insertnode"){tridx=this.element.children("tbody").children("tr:not([data-container],[data-grouprow])").index(tr);prevTr=tr.prev("tr");dataLevel=parseInt(tr.attr("aria-level"),10);tr.remove();this._reapplyZebraStyle(tridx);this._updateParentRowAfterDelete(prevTr,dataLevel)}}}else{this.dataBind()}}return transactions},dataBind:function(){if(!this._initialized){this._renderExtraHeaderCellHandler=$.proxy(this._renderExtraHeaderCells,this);this._renderExtraFooterCellHandler=$.proxy(this._renderExtraFooterCells,this);this._headerInitCallbacks.push({type:"TreeGrid",func:this._renderExtraHeaderCellHandler});this._footerInitCallbacks.push({type:"TreeGrid",func:this._renderExtraFooterCellHandler})}else{if(this._inferOpType()==="remote"&&this.dataSource&&this.dataSource.schema()){this.dataSource._data=[]}}$.ui.igGrid.prototype.dataBind.apply(this,arguments)},_generateDataSourceOptions:function(){var o=this.options,ds=o.dataSource,instanceOfDs,tds,opts=$.ui.igGrid.prototype._generateDataSourceOptions.apply(this,arguments);opts.treeDS={childDataKey:o.childDataKey,initialExpandDepth:o.initialExpandDepth,foreignKey:o.foreignKey,foreignKeyRootValue:o.foreignKeyRootValue};opts.treeDS=$.extend(opts.treeDS,o.dataSourceSettings);opts.treeDS.enableRemoteLoadOnDemand=o.enableRemoteLoadOnDemand;opts.treeDS.dataSourceUrl=o.dataSourceUrl;if(o.dataSourceUrl===null&&typeof ds==="string"){opts.treeDS.dataSourceUrl=ds}instanceOfDs=ds&&typeof ds._xmlToArray==="function"&&typeof ds._encodePkParams==="function";if(instanceOfDs){if(ds._isHierarchicalDataSource!==undefined){ds.settings.treeDS=ds.settings.treeDS||{};ds.settings.treeDS=$.extend(ds.settings.treeDS,opts.treeDS)}else{if($.type(ds.settings.dataSource)==="array"||$.type(ds.settings.dataSource)==="object"){tds=ds.settings.dataSource}else if($.type(ds.settings.dataSource)!=="string"){tds=ds.data()}else{tds=[]}ds.settings.dataSource=null;ds.settings.data=null;if(opts&&opts.dataSource){opts.dataSource=null}opts=$.extend(true,{},ds.settings,opts);opts.dataSource=tds;tds=null;o.dataSource=new $.ig.TreeHierarchicalDataSource(opts)}}else{opts.dataSource=ds}if(o.dataSourceType!==null){opts.type=o.dataSourceType}return opts},_getDataColumns:function(cols){var i,j,res=[],colsLength=cols.length,dCols;for(i=0;i<colsLength;i++){if(cols[i].group!==undefined&&cols[i].group!==null){dCols=this._getDataColumns(this._getDataColumns(cols[i].group));for(j=0;j<dCols.length;j++){res.push(dCols[j])}}else{res.push(cols[i])}}return res},_generateDataSourceSchema:function(){var schema,i,j,rec,prop,cols=this._getDataColumns(this.options.columns),k,c,ds=this.options.dataSource;if(cols.length>0){schema={};schema.fields=[];j=0;for(i=0;i<cols.length;i++){if(cols[i].unbound===true){continue}schema.fields[j]={};schema.fields[j].name=cols[i].key;schema.fields[j].type=cols[i].dataType;schema.fields[j].mapper=cols[i].mapper;j++}}else if(this.options.autoGenerateColumns){schema={};schema.fields=[];if($.type(ds)==="string"&&this.dataSource){ds=this.dataSource.data()}if(ds&&ds.length&&$.type(ds)==="array"){rec=ds[0];for(prop in rec){if(rec.hasOwnProperty(prop)){for(k=0;k<cols;k++){if(cols[k].key===prop){c=cols[k];break}}if(_aNull(c)){schema.fields.push({name:prop,type:$.ig.getColType(rec[prop])})}else{if(c.unbound===true){continue}schema.fields.push({name:prop,type:c.dataType})}}}}}if(schema){schema.searchField=this.options.responseDataKey}this._trigger(this.events.schemaGenerated,null,{owner:this,schema:schema,dataSource:ds});return schema},_createDataSource:function(dataOptions){var ds=this.options.dataSource,currentDataSource;if(!ds||typeof ds._xmlToArray!=="function"||typeof ds._encodePkParams!=="function"){currentDataSource=new $.ig.TreeHierarchicalDataSource(dataOptions)}else{currentDataSource=$.ui.igGrid.prototype._createDataSource.apply(this,arguments)}return currentDataSource},_containersRendered:function(){if(this.options.renderExpansionIndicatorColumn){this._addDataSkipColumn()}},_renderData:function(){var i=this._initialized;$.ui.igGrid.prototype._renderData.apply(this,arguments);if(!i){this._bindEvtsToExpIndicators()}},_bindEvtsToExpIndicators:function(){var $cont=this.container(),selector;if(!this.options.renderExpansionIndicatorColumn){selector="tbody>tr>td>span[data-expandcell-indicator]"}else{selector="td[data-expand-cell=1]"}$cont.off(".toggleTreegrid");$cont.on({"mouseup.toggleTreegrid":$.proxy(this._onMouseUpExpander,this),"keydown.toggleTreegrid":$.proxy(this._onKeyDownExpander,this),"click.toggleTreegrid":function(e){e.stopPropagation()}},selector)},_getTotalRowCount:function(){return this._getDataView().length},_getDataView:function(){return this.dataSource.flatDataView()},_renderVirtualRecordsContinuous:function(from){var to,$scrollContainer,scrllToBttm=false,$ae,$cell,$row,rowId,cellInd,$tr;if(_aNull(from)){$.ui.igGrid.prototype._renderVirtualRecordsContinuous.apply(this,arguments)}else{this._totalRowCount=this._getTotalRowCount();if(from>this._totalRowCount){return $.ui.igGrid.prototype._renderVirtualRecordsContinuous.apply(this,arguments)}this._virtualRowCount=this._determineVirtualRowCount();if(this._virtualRowCount>this._totalRowCount){this._virtualRowCount=this._totalRowCount}to=from+parseInt(this._virtualRowCount,0);if(to>this._totalRowCount){to=this._totalRowCount-1;from=to-this._virtualRowCount;scrllToBttm=true;if(from<0){from=0}}$ae=$(document.activeElement);$cell=$ae.closest("td");$row=$ae.closest("tr");cellInd=$cell.index();rowId=$row.attr("id");this._renderRecords(from,to);this._avgRowHeight=this._calculateAvgRowHeight();this._setScrollContainerHeight(this._totalRowCount*this._avgRowHeight);if(scrllToBttm){$scrollContainer=this._scrollContainer();$scrollContainer[0].scrollTop=this._totalRowCount*this._avgRowHeight+1}if(rowId){$tr=$("#"+rowId.replace(/(:|\.|\[|\]|,|\/)/g,"\\$1"));if($tr.length){if($ae.is("tr")){$ae=$tr;$tr.focus()}else if($ae.is("td")){$ae=$tr.children("td:nth-child("+(cellInd+1)+")");$ae.focus()}else if($ae.attr("data-expand-button")!==undefined){$ae=$tr.find("[data-expand-button]");$ae.focus()}this._fireInternalEvent("_virtualRecordsRendered",{row:$tr,activeElement:$ae,cellInd:cellInd})}}}},_rerenderDataSkipColumn:function(){var $cntnr=this.container();$cntnr.find("thead").find("th").find("[data-treegrid-th]").remove();this._rerenderColgroups();if(this.options.width===null){this._updateContainersWidthOnGridWidthNull()}else{this._adjustLastColumnWidth(true)}},_addDataSkipColumn:function(dbd){if(!this.options.renderExpansionIndicatorColumn){return}var optInd=this.options.indentation,indent,$thDataSkip,$gridColgroup,$headersTbl,$footersTbl,cf;if(_aNull(dbd)){dbd=this.options.initialIndentationLevel>0?this.options.initialIndentationLevel:this.dataSource.getDataBoundDepth()}dbd=dbd||1;if(dbd>=0){indent=parseInt(optInd,10)*dbd;if(optInd&&optInd.indexOf&&optInd.indexOf("%")>=0){indent=indent+"%"}cf=this.hasFixedColumns()&&this.fixingDirection()==="left";if(cf){$gridColgroup=this.fixedBodyContainer().find("colgroup:first")}else{$gridColgroup=this.element.find("colgroup:first")}this._addColHelper($gridColgroup,indent);if(cf){$headersTbl=this.fixedHeadersTable()}else{$headersTbl=this.headersTable()}if($headersTbl.attr("id")!==this.element.attr("id")){this._addColHelper($headersTbl.find("colgroup:first"),indent)}if(cf){$footersTbl=this.fixedFootersTable()}else{$footersTbl=this.footersTable()}if($footersTbl.attr("id")!==this.element.attr("id")){this._addColHelper($footersTbl.find("colgroup"),indent)}if(!$headersTbl.find("> thead th[data-treegrid-th]").length){if(!$headersTbl.find("> thead tr:nth-child(1) th.ui-iggrid-rowselector-header").length){$thDataSkip=$("<th></th>").prependTo($headersTbl.find("> thead tr:nth-child(1)"))}else{$thDataSkip=$("<th></th>").insertAfter($headersTbl.find("> thead tr:nth-child(1) th.ui-iggrid-rowselector-header"))}$thDataSkip.addClass(this.css.expandHeaderCell).attr("data-skip",true).attr("data-treegrid-th",true);if(this._isMultiColumnGrid){$thDataSkip.attr("rowspan",this._maxLevel+1)}}}},_addColHelper:function($colgroup,width){var $col=$colgroup.find("col[data-treegrid-col]");if($col.length===0){$col=$("<col />").prependTo($colgroup).attr("data-skip","true").attr("data-treegrid-col","true")}if(width){$col.width(width)}},_renderHeader:function(){$.ui.igGrid.prototype._renderHeader.apply(this,arguments);this.container().addClass(this.css.containerClasses)},_getHtmlForDataView:function(ds,isFixed,start,end){var i,dsLen,html="";ds=ds||this._getDataView();dsLen=ds.length;if(start===undefined){start=0;end=dsLen-1}if(start!==undefined&&end===undefined){end=start;start=0}start=start<0?0:start;end=end>dsLen-1?dsLen-1:end;for(i=start;i<=end;i++){html+=this._renderRecord(ds[i],i,isFixed)}return html},_renderRecord:function(data,rowIndex,isFixed,rowData){return this._renderRecordInternal(data,rowIndex,isFixed,rowData)},_renderRecordInternal:function(data,rowIndex,isFixed,rowData){var html="",markup,isContainerOnTheLeft,idxStart,children,hasChildren,o=this.options,key=this.options.primaryKey,dataBoundDepth,cssClass,id=this.id(),owns="";if(!rowData){children=data[o.childDataKey];hasChildren=children&&(children===true||$.type(children)==="array"&&(o.enableRemoteLoadOnDemand||children.length>0));rowData={dataBoundDepth:data[o.dataSourceSettings.dataLevelKey],parentCollapsed:false,hasExpandCell:hasChildren,children:children,expand:data[o.dataSourceSettings.expandedKey]}}dataBoundDepth=rowData.dataBoundDepth;cssClass=this.css.rowLevel+dataBoundDepth;html+="<tr";if(rowIndex%2!==0&&this.options.alternateRowStyles){cssClass+=" "+this.css.recordAltClass}if(this._transformCssCallback){cssClass=this._transformCssCallback(cssClass,data)}html+=' class="'+cssClass+'" data-row-idx="'+rowIndex+'" aria-level="'+dataBoundDepth+'"';if(!_aNull(key)){html+=' data-id="'+this._kval_from_key(key,data)+'" id="'+id+"_"+this._kval_from_key(key,data)+'"'}else if(!_aNull(data.ig_pk)){html+=' data-id="'+data.ig_pk+'" id="'+id+"_"+data.ig_pk+'"'}if(rowData.parentCollapsed&&dataBoundDepth>0){html+=' style="display: none;"'}if(rowData.hasExpandCell){$(rowData.children).each(function(){owns+=id+"_"+this[key]+" "});owns=owns.trimEnd();if(owns!==""){html+=' aria-owns="'+owns}html+='" aria-expanded="'+rowData.expand+'"'}html+=' role="row" tabIndex="'+this.options.tabIndex+'">';isContainerOnTheLeft=this._isDataContainerOnTheLeft(isFixed);if(this._shouldRenderDataSkipColumn(isFixed)){if(this._selection&&this._selection instanceof $.ig.SelectedRowsCollection){html+=this._editCellStyle(this._renderExpandCellHandler(rowData),data,key)}else{html+=this._renderExpandCellHandler(rowData)}}markup=$.ui.igGrid.prototype._renderRecord.call(this,data,rowIndex,isFixed);if(isContainerOnTheLeft){html+=this._renderFirstDataCellHandler(markup,rowData)}else{idxStart=markup.indexOf("<td");html+=markup.substring(idxStart)}return html},_shouldRenderDataSkipColumn:function(isFixed){var fdLeft;if(!this.options.renderExpansionIndicatorColumn){return false}if(!this.hasFixedColumns()){return true}fdLeft=this.fixingDirection()==="left";if(!isFixed&&fdLeft&&this._isFixedNonDataColumnsOnly()){return false}return this._isDataContainerOnTheLeft(isFixed)},_renderFirstDataCell:function(markup,rowData){var newTDSmarkup="",idxStart,TDSmarkup,tdContentFirstInd,otherTDSmarkup,classIdx;idxStart=markup.indexOf("<td");if(idxStart===-1){return""}TDSmarkup=markup.substring(idxStart);tdContentFirstInd=TDSmarkup.indexOf(">",2);otherTDSmarkup=TDSmarkup.substring(tdContentFirstInd+1);newTDSmarkup=TDSmarkup.substring(0,tdContentFirstInd);if(this.options.renderExpansionIndicatorColumn){newTDSmarkup+=' data-cell-shift-container="1">'}else{classIdx=newTDSmarkup.indexOf('class="');if(classIdx>-1){newTDSmarkup=newTDSmarkup.substring(0,classIdx+7)+this.css.expandColumn+" "+newTDSmarkup.substr(classIdx+7)}else{newTDSmarkup+=' class="'+this.css.expandColumn+'"'}newTDSmarkup+=' data-expand-cell="1">'}newTDSmarkup+=this._renderExpandCellContainer(rowData);newTDSmarkup+=otherTDSmarkup;return newTDSmarkup},_renderExpandCellContainer:function(rowData){var span="",margin,dataBoundDepth=rowData.dataBoundDepth;margin=dataBoundDepth>0?parseInt(this.options.indentation,10)*dataBoundDepth:0;if(this.options.renderExpansionIndicatorColumn){span='<span class="'+this.css.dataColumnExpandContainer+'" data-shift-container="1" style="display: inline-block; margin-left:'+margin+'px;"></span>'}else{span=this._renderExpandCellContainerHelper(rowData)}return span},_renderExpandCellContainerHelper:function(rowData){var span="",css,title,margin,dataBoundDepth=rowData.dataBoundDepth,cssEC=this.css.expandContainer,renderExpandButton=rowData.hasExpandCell;if(this.options.renderExpansionIndicatorColumn){cssEC=this.css.expandColumnContainer}margin=dataBoundDepth>0?parseInt(this.options.indentation,10)*dataBoundDepth:0;if($.type(this.options.indentation)==="string"&&this.options.indentation.indexOf("%")>0){margin+="%"}else{margin+="px"}span='<span data-expandcell-indicator="1" class="'+cssEC+'" style="padding-left:'+margin+';">';if(renderExpandButton&&this.options.showExpansionIndicator){if(rowData.expand){css=this.css.expandCellExpanded;title=this._getLocaleValue("collapseTooltip")}else{css=this.css.expandCellCollapsed;title=this._getLocaleValue("expandTooltip")}span+='<span data-expand-button class="'+css+'" title="'+title+'" tabIndex="'+this.options.tabIndex+'"'+" data-localeid='"+(rowData.expand?"collapseTooltip":"expandTooltip")+"'"+" data-localeattr='title'"+"></span>"}span+="</span>";return span},_renderExpandCell:function(rowData){if(!rowData.hasExpandCell){return this._renderDataSkipCell()}var html,css;css=(this.css.expandColumn+" "+this.css.dataSkipCell).trim();html='<td class="'+css+'" data-expand-cell="1" data-skip="true" tabIndex="'+this.options.tabIndex+'">';if(this.options.showExpansionIndicator){html+=this._renderExpandCellContainerHelper(rowData)}return html+"</td>"},_renderDataSkipCell:function(){return'<td class="'+this.css.dataSkipCell+'" data-skip="true" tabIndex="'+this.options.tabIndex+'"></td>'},_rerenderColgroups:function(){$.ui.igGrid.prototype._rerenderColgroups.apply(this,arguments);this._addDataSkipColumn()},_columnsGenerated:function(){var cols=this.options.columns,key,i,sDS=this.options.dataSourceSettings,arrSkipColumns=[this.options.childDataKey,sDS.expandedKey,sDS.dataLevelKey];for(i=0;i<cols.length;i++){key=cols[i].key;if($.inArray(key,arrSkipColumns)!==-1){$.ig.removeFromArray(cols,i);i--}}},_rowVirtualizationEnabled:function(){return this.options.rowVirtualization||this.options.virtualization},_rerenderGridRowsOnToggle:function(){var virtualizationEnabled=this._rowVirtualizationEnabled(),dsSettings=this.dataSource.settings;return dsSettings.paging.enabled&&dsSettings.treeDS.paging.mode!=="rootLevelOnly"||virtualizationEnabled},_onDataRecordToggled:function(rec,expand,res,args){if(!res||!args){return}var flatData,html,level,dataIdx,diff,triggerEvents,$row,callAsync=false,$firstRow,rowObj,$fRow,$ufRow,$curRow,offsetTop,customCallback,$this=this,fRows,ufRows,eArgs,$scrollContainer,owner=this,id=$(this.element).attr("id"),ariaOwns="";$row=args.row;triggerEvents=args.triggerEvents;customCallback=args.customCallback;rowObj=this._getRows($row);$fRow=rowObj.fixedRow;$ufRow=rowObj.unfixedRow;level=parseInt($row.attr("aria-level"),10);eArgs={owner:this,row:$ufRow,fixedRow:$fRow,dataLevel:level,dataRecord:rec};this._loadingIndicator.hide();if(!this._rerenderGridRowsOnToggle()){if(expand&&!$row.attr("data-populated")){if(rec){this._toggleRowSuccessors($ufRow);$ufRow.add($fRow).attr("data-populated","1");flatData=this.dataSource.getFlatDataForRecord(rec,level+1);$(rec[this.options.childDataKey]).each(function(){ariaOwns+=id+"_"+this[owner.options.primaryKey]+" "});ariaOwns=ariaOwns.trimEnd();if(flatData&&flatData.flatVisibleData){html=this._getHtmlForDataView(flatData.flatVisibleData,false);ufRows=$(html).insertAfter($ufRow);$ufRow.attr("aria-owns",ariaOwns);if($fRow){html=this._getHtmlForDataView(flatData.flatVisibleData,true);fRows=$(html).insertAfter($fRow);$fRow.attr("aria-owns",ariaOwns)}this._trigger("_buildDOMOnToggle",this,{fRows:fRows,ufRows:ufRows})}}}else{this._toggleRowSuccessors($row)}}else{if(this._rowVirtualizationEnabled()&&this.options.virtualizationMode==="continuous"){$scrollContainer=this._scrollContainer();if($row.length){$firstRow=$row.closest("tbody").children("tr[data-row-idx]:first");if(!$firstRow.length){return}dataIdx=$firstRow.attr("data-row-idx");offsetTop=$row.igOffset().top;this._renderVirtualRecordsContinuous(parseInt(dataIdx,10));this._updateVirtualScrollContainer();this._trigger("virtualrecordsrender",null,{owner:this,dom:this._virtualDom});if(!expand){$curRow=$("#"+this.id()+" > tbody > tr[data-row-idx="+$row.attr("data-row-idx")+"]");diff=$curRow.igOffset().top-offsetTop;if(Math.abs(diff)>1){callAsync=true;setTimeout(function(){$scrollContainer[0].scrollTop=$scrollContainer.scrollTop()+diff},50)}}}else{callAsync=true;this._updateVirtualScrollContainer()}}}if(callAsync){setTimeout(function(){$this._callDataToggledEventsAndCallbacks(customCallback,expand,eArgs,triggerEvents);$this._refreshIgScrollBars()},55)}else{this._callDataToggledEventsAndCallbacks(customCallback,expand,eArgs,triggerEvents);this._refreshIgScrollBars()}},_avgRowHeightChanged:function(){var top=this._getScrollContainerScrollTop(),h=this._getScrollContainerHeight();this._updateVirtualScrollContainer();this._correctVirtVertScrollTop(top,h)},_callDataToggledEventsAndCallbacks:function(customCallback,expand,eArgs,triggerEvents){var hasHeight=this.options.height&&parseInt(this.options.height,10)>0,isVirt=this.options.virtualization===true||this.options.columnVirtualization===true||this.options.rowVirtualization===true;if(customCallback){$.ig.util.invokeCallback(customCallback,[this,{unfixedRow:eArgs.row,fixedRow:eArgs.fixedRow},eArgs.dataRecord,expand])}if(hasHeight&&!isVirt&&this._hasVerticalScrollbar!==this.hasVerticalScrollbar()){this._adjustLastColumnWidth()}if(expand){this._fireInternalEvent("_rowExpanded",eArgs)}else{this._fireInternalEvent("_rowCollapsed",eArgs)}if(triggerEvents){if(expand){this._trigger(this.events.rowExpanded,null,eArgs)}else{this._trigger(this.events.rowCollapsed,null,eArgs)}}},toggleRow:function(row,callback){var $row,state,expand,ds=this.dataSource;if(row instanceof $){$row=row}else{$row=this.element.find("tr[data-id="+row+"]")}state=$row.attr("aria-expanded");if(!$row.length){expand=!ds.getExpandStateById(row)}else{expand=state==="false"}this._expandCollapseRow(row,expand,false,callback)},expandRow:function(row,callback){this._expandCollapseRow(row,true,false,callback)},collapseRow:function(row,callback){this._expandCollapseRow(row,false,false,callback)},_onMouseUpExpander:function(event){var $et=$(event.target);if($et.attr("data-expandcell-indicator")!==undefined){$et.find("[data-expand-button]").focus()}this._toggleRow($et.closest("tr"))},_onKeyDownExpander:function(event){if(event.keyCode===$.ui.keyCode.ENTER||event.keyCode===$.ui.keyCode.SPACE){this._toggleRow($(event.target).closest("tr"));event.preventDefault();event.stopPropagation()}},_toggleRow:function($row){if(!$row.length){return}var state=$row.attr("aria-expanded"),expand;if(state===undefined){return}expand=state==="false"||state==="0";this._expandCollapseRow($row,expand,true)},_expandCollapseRow:function(row,expand,triggerEvents,callback){var primaryKeyCol,noCancel=true,func,args,callbackArgs,rowId,$row,me=this,rows,$fRow,$ufRow,isExpanded=expand,$tr,$ae,$cell,cellInd;if(row instanceof $){$row=row;if(!_aNull(this.options.primaryKey)){primaryKeyCol=this.columnByKey(this.options.primaryKey);rowId=$row.attr("data-id");if(primaryKeyCol&&(primaryKeyCol.dataType==="number"||primaryKeyCol.dataType==="numeric")){rowId=parseInt(rowId,10)}}else{rowId=$row.index()}}else{rowId=row;$row=this.element.find("tr[data-id="+row+"]")}if($row.length===1&&$row.attr("aria-expanded")===isExpanded.toString()){return}func=$.proxy(this._onDataRecordToggled,this);callbackArgs={callback:func,args:{row:$row,triggerEvents:triggerEvents}};if(callback){callbackArgs.args.customCallback=callback}rows=this._getRows($row);$fRow=rows.fixedRow;$ufRow=rows.unfixedRow;args={owner:this,row:$ufRow,fixedRow:$fRow,dataLevel:parseInt($row.attr("aria-level"),10)};if(triggerEvents){if(expand){noCancel=this._trigger(this.events.rowExpanding,null,args)}else{noCancel=this._trigger(this.events.rowCollapsing,null,args)}}if(noCancel){this._loadingIndicator.show();$ae=$(document.activeElement);$cell=$ae.closest("td");cellInd=$cell.index();me.dataSource.setExpandedStateByPrimaryKey(rowId,expand,callbackArgs);if(me._rerenderGridRowsOnToggle()){$tr=me.element.find('[data-id="'+rowId+'"]');if($tr.length){if($ae.is("tr")){$ae=$tr;$tr.focus()}if($ae.is("td")){$ae=$tr.children("td:nth-child("+(cellInd+1)+")");$ae.focus()}else if($ae.attr("data-expand-button")!==undefined){$ae=$tr.find("[data-expand-button]");$ae.focus()}}}}},_toggleRowSuccessors:function($row){var $nextRow=$row,foundUpperLevel=false,dL,$fRow,$ufRow=$row,levelCollapsed=null,$container,dataBoundDepth=parseInt($row.attr("aria-level"),10),isExpanded=$row.attr("aria-expanded"),styleDisplay="",expanded=isExpanded==="true";if(expanded){styleDisplay="none"}if(this.hasFixedColumns()){if(this._isFixedElement($row)){$fRow=$row;if(this._rowVirtualizationEnabled()){$container=this._vdisplaycontainer()}else{$container=this.scrollContainer()}$ufRow=$container.find("tbody > tr").eq($row.index())}else{$fRow=this.fixedBodyContainer().find("tbody > tr").eq($row.index())}}while($nextRow.length===1&&!foundUpperLevel){$nextRow=$nextRow.next("tr[aria-level]");dL=parseInt($nextRow.attr("aria-level"),10);if(isNaN(dL)||dL<=dataBoundDepth){foundUpperLevel=true;break}if(!expanded){if(levelCollapsed!==null){if(dL<=levelCollapsed){levelCollapsed=null}else{continue}}if($nextRow.attr("aria-expanded")==="false"){levelCollapsed=dL}}this._showHideRow($nextRow,styleDisplay)}if(expanded){$ufRow.add($fRow).attr({"data-populated":"1","aria-expanded":false}).find("[data-expand-button]").attr({title:this._getLocaleValue("expandTooltip"),"data-localeid":"expandTooltip","data-localeattr":"title"}).removeClass(this.css.expandCellExpanded).addClass(this.css.expandCellCollapsed);
}else{$ufRow.add($fRow).attr("aria-expanded",true).find("[data-expand-button]").attr({title:this._getLocaleValue("collapseTooltip"),"data-localeid":"collapseTooltip","data-localeattr":"title"}).removeClass(this.css.expandCellCollapsed).addClass(this.css.expandCellExpanded)}},_showHideRow:function($row,styleDisplay){var rows,$ufRow=$row,$fRow,fixedColumns=this.hasFixedColumns();if(fixedColumns){rows=this._getRows($row);$fRow=rows.fixedRow;$ufRow=rows.unfixedRow}$ufRow.css("display",styleDisplay);if($fRow){$fRow.css("display",styleDisplay)}if(styleDisplay===""){this._trigger("_rowShown",this,{fRow:$fRow,ufRow:$ufRow})}else{this._trigger("_rowHidden",this,{fRow:$fRow,ufRow:$ufRow})}},_renderExtraHeaderCells:function(row,colgroup,prepend){if(!this.options.renderExpansionIndicatorColumn){return}if(prepend===true){$("<td></td>").prependTo(row).css("border-width",0).attr("data-skip",true);if(colgroup){$("<col />").prependTo(colgroup).attr("data-skip",true).css("width",this.options.indentation)}}else{$("<td></td>").appendTo(row).css("border-width",0).attr("data-skip",true);if(colgroup){$("<col />").appendTo(colgroup).attr("data-skip",true).css("width",this.options.indentation)}}},_isFixedNonDataColumnsOnly:function(){if(this.hasFixedColumns()&&(!this._fixedColumns||!this._fixedColumns.length)){return true}return false},_isDataContainerOnTheLeft:function(isFixed){var fdLeft=this.fixingDirection()==="left";if(!this.hasFixedColumns()){return true}if($.type(this._fixedColumns)==="array"&&!this._fixedColumns.length){return true}if(isFixed){return fdLeft}return!fdLeft},_getRows:function($row){var index,$fRow,$ufRow=$row;if(this.hasFixedColumns()){index=$row.index();if(this._isFixedElement($row)){$fRow=$row;$ufRow=$(this.rowAt(index))}else{$fRow=$(this.fixedRowAt(index))}}return{fixedRow:$fRow,unfixedRow:$ufRow}},_renderRow:function(rec,tr){var funcCallbak;funcCallbak=function(rec,tr){return $.ui.igGrid.prototype._renderRow.call(this,rec,tr)};return this._persistExpansionIndicator(rec,tr,funcCallbak,this)},renderNewChild:function(rec,parentId){var tbody=this.element.children("tbody"),tbodyFixed=this.fixedBodyContainer().find("tbody"),virt=this.options.virtualization===true||this.options.rowVirtualization===true,prevRow,dlprop=this.options.dataSourceSettings.dataLevelKey,dl,parent,rowData,fixing=this.hasFixedColumns(),unfixedRow,fixedRow,dataId,index=this._recordIndexInFlatView(rec[this.options.primaryKey]);if(parentId===undefined||parentId===null){this.renderNewRow(rec)}else{if(virt){this._renderVirtualRecordsContinuous();this._startRowIndex=0;this.virtualScrollTo(index)}else{parent=this.dataSource.findRecordByKey(parentId);if(parent===null){throw new Error(this._getLocaleValue("recordNotFound").replace("{id}",parentId))}parent[this.options.dataSourceSettings.expandedKey]=true;this.dataSource.generateFlatDataView();dl=parent[dlprop]+1;prevRow=this.rowById(parentId);while(prevRow.nextAll(":not(.ui-iggrid-addrow)").first().length>0){if(parseInt(prevRow.nextAll(":not(.ui-iggrid-addrow)").first().attr("aria-level"),10)>=dl){prevRow=prevRow.nextAll(":not(.ui-iggrid-addrow)").first()}else{break}}prevRow=prevRow?prevRow:this.rowById(parentId);rowData={dataBoundDepth:dl};dataId=prevRow.attr("data-id");this._fireInternalEvent("_childRowRendering",{id:rec[this.options.primaryKey],parentId:parentId});unfixedRow=this._renderRecord(rec,prevRow.index()+1,false,rowData);if(fixing){fixedRow=this._renderRecord(rec,prevRow.index()+1,true,rowData)}if(prevRow.length>0){$(prevRow).after(unfixedRow);if(fixing){tbodyFixed.find("tr[data-id="+dataId+"]").after(fixedRow)}}else{tbody.append(unfixedRow);if(fixing){tbodyFixed.append(fixedRow)}}}}},_recordIndexInFlatView:function(rowId){var dv=this.dataSource.flatDataView(),pk=this.options.primaryKey,index=-1;for(var i=0;i<dv.length;i++){if(dv[i][pk]===rowId){index=i;break}}return index},_persistExpansionIndicator:function(rec,tr,funcCallback,funcCallee){var $td,trRes=tr,$tr=$(tr),$span,renderEC=this.options.renderExpansionIndicatorColumn;if(renderEC){$span=$tr.find("span[data-shift-container]");$td=$span.closest("td")}else{$span=$tr.find("[data-expandcell-indicator]");$td=$span.closest("td");$span.detach()}if(funcCallback&&funcCallee){trRes=funcCallback.call(funcCallee,rec,tr);$tr=$(trRes)}if($span.length>0){$span.prependTo($td)}return trRes},_detachEvents:function(){if(this._columnsGeneratedHandler){this.element.unbind("igtreegrid_columnsgenerated",this._columnsGeneratedHandler)}if(this._containersRenderedHandler){this.element.unbind("iggrid_gridcontainersrendered",this._containersRenderedHandler)}if(this._avgRowHeightChangedHandler){this.element.unbind("iggridavgrowheightchanged",this._avgRowHeightChangedHandler)}$.ui.igGrid.prototype._detachEvents.apply(this,arguments)},_attachEvents:function(){this._columnsGeneratedHandler=$.proxy(this._columnsGenerated,this);this.element.bind("igtreegrid_columnsgenerated",this._columnsGeneratedHandler);this._containersRenderedHandler=$.proxy(this._containersRendered,this);this.element.bind("iggrid_gridcontainersrendered",this._containersRenderedHandler);if(this.options.virtualization||this.options.rowVirtualization){this._avgRowHeightChangedHandler=$.proxy(this._avgRowHeightChanged,this);this.element.bind("iggridavgrowheightchanged",this._avgRowHeightChangedHandler)}},_initFeature:function(featureObject){if(!featureObject){return}if(featureObject.name===undefined){return}var widgetTreeGrid="igTreeGrid"+featureObject.name;if($.type(this.element[widgetTreeGrid])==="function"){if(this.element.data(widgetTreeGrid)){this.element[widgetTreeGrid]("destroy")}featureObject.language=this.options.language;this.element[widgetTreeGrid](featureObject);this.element.data(widgetTreeGrid)._injectGrid(this)}else{return $.ui.igGrid.prototype._initFeature.apply(this,arguments)}},_destroyFeatures:function(){var i,features=this.options.features,e=this.element;for(i=0;i<features.length;i++){if(e.data("igTreeGrid"+features[i].name)){e["igTreeGrid"+features[i].name]("destroy")}else if(e.data("igGrid"+features[i].name)){e["igGrid"+features[i].name]("destroy")}}},destroy:function(){this._detachEvents();this._superApply(arguments);this.element.removeData($.ui.igGrid.prototype.widgetName);this._removeOverridenFunction();return this}});$.extend($.ui.igTreeGrid,{version:"19.1.20"});return $});