﻿/*!@license
* Infragistics.Web.ClientUI Pivot Data Selector localization resources 15.2.20152.1027
*
* Copyright (c) 2011-2015 Infragistics Inc.
*
* http://www.infragistics.com/
*
*/
(function($){$.ig=$.ig||{};if(!$.ig.PivotDataSelector){$.ig.PivotDataSelector={};$.extend($.ig.PivotDataSelector,{locale:{invalidBaseElement:" is not supported as base element. Use DIV instead.",catalog:"Catalog",cube:"Cube",measureGroup:"Measure Group",measureGroupAll:"(All)",rows:"Rows",columns:"Columns",measures:"Measures",filters:"Filters",deferUpdate:"Defer Update",updateLayout:"Update Layout",selectAll:"Select All"}})}})(jQuery);/*!@license
* Infragistics.Web.ClientUI Pivot Data Selector 15.2.20152.1027
*
* Copyright (c) 2011-2012 Infragistics Inc.
*
* http://www.infragistics.com/
*
* Depends on: 
*	jquery-1.4.4.js
*	jquery.ui.core.js
*	jquery.ui.widget.js
*	jquery.ui.mouse.js
*	jquery.ui.draggable.js
*	jquery.ui.droppable.js
*	modernizr.js (Optional)
*	infragistics.util.js
*	infragistics.datasource.js
*	infragistics.olapxmladatasource.js
*	infragistics.olapflatdatasource.js
*	infragistics.templating.js
*	infragistics.ui.shared.js
*   infragistics.ui.scroll.js
*	infragistics.ui.combo.js
*	infragistics.ui.tree.js
*	infragistics.ui.pivot.shared.js
*/
(function($){var _droppable=$.ui.droppable.prototype.widgetFullName||$.ui.droppable.prototype.widgetName;$.widget("ui.igPivotDataSelector",{css:{dataSelector:"ui-igpivotdataselector",dataSelectorRoot:"ui-igpivotdataselector-root",catalog:"ui-igpivotdataselector-catalog",cube:"ui-igpivotdataselector-cube",measureGroup:"ui-igpivotdataselector-measuregroup",metadata:"ui-igpivotdataselector-metadata ui-widget-content",metadataItem:"ui-igpivot-metadataitem ui-widget ui-corner-all ui-state-default",dropAreasTable:"ui-igpivotdataselector-dropareas",dropArea:"ui-igpivot-droparea ui-widget-content",activeDropArea:"active",filtersIcon:"ui-icon ui-icon-pivot-filters",columnsIcon:"ui-icon ui-icon-pivot-columns",rowsIcon:"ui-icon ui-icon-pivot-rows",measuresIcon:"ui-icon ui-icon-pivot-measures",updateLayout:"ui-igpivotdataselector-updatelayout",dropIndicator:"ui-state-highlight",invalidDropIndicator:"ui-state-error",insertItem:"ui-igpivot-insertitem ui-state-highlight ui-corner-all",metadataItemDropDown:"ui-igpivot-metadatadropdown ui-widget ui-widget-content",filterIcon:"ui-icon ui-icon-pivot-smallfilter",filterDropDown:"ui-igpivot-filterdropdown ui-widget ui-widget-content",filterMembers:"ui-igpivot-filtermembers"},options:{width:250,height:null,dataSource:null,dataSourceOptions:{xmlaOptions:{serverUrl:null,catalog:null,cube:null,measureGroup:null,requestOptions:{withCredentials:false,beforeSend:null},enableResultCache:true,discoverProperties:null,executeProperties:null,mdxSettings:{nonEmptyOnRows:true,nonEmptyOnColumns:true,addCalculatedMembersOnRows:true,addCalculatedMembersOnColumns:true,dimensionPropertiesOnRows:[],dimensionPropertiesOnColumns:[]}},flatDataOptions:{dataSource:null,dataSourceUrl:null,dataSourceType:null,responseDataKey:null,responseDataType:null,metadata:{cube:{name:null,caption:null,measuresDimension:{name:null,caption:null,measures:[{name:null,caption:null,aggregator:null,displayFolder:null}]},dimensions:[{name:null,caption:null,hierarchies:[{name:null,caption:null,displayFolder:null,levels:[{name:null,caption:null,memberProvider:null}]}]}]}}},measures:null,filters:null,rows:null,columns:null},deferUpdate:false,dragAndDropSettings:{appendTo:"body",containment:false,zIndex:10},dropDownParent:"body",disableRowsDropArea:false,disableColumnsDropArea:false,disableMeasuresDropArea:false,disableFiltersDropArea:false,customMoveValidation:null},events:{dataSelectorRendered:null,dataSourceInitialized:null,dataSourceUpdated:null,deferUpdateChanged:null,dragStart:null,drag:null,dragStop:null,metadataDropping:null,metadataDropped:null,metadataRemoving:null,metadataRemoved:null,filterDropDownOpening:null,filterDropDownOpened:null,filterMembersLoaded:null,filterDropDownOk:null,filterDropDownClosing:null,filterDropDownClosed:null},_deferUpdate:false,_create:function(){var $this=this,elementName=this.element[0].nodeName.toUpperCase();if(elementName!=="DIV"){throw new Error(elementName+$.ig.PivotDataSelector.locale.invalidBaseElement)}this.element.addClass(this.css.dataSelector);this._onFiltersChanged=function(collection,collectionChangedArgs){var dropArea=$this.element.find(".ui-igpivot-droparea[data-role=filters]");$this._onDataSourceCollectionChanged(collection,collectionChangedArgs,dropArea,$this.options.disableFiltersDropArea)};this._onRowAxisChanged=function(collection,collectionChangedArgs){var dropArea=$this.element.find(".ui-igpivot-droparea[data-role=rows]");$this._onDataSourceCollectionChanged(collection,collectionChangedArgs,dropArea,$this.options.disableRowsDropArea)};this._onColumnAxisChanged=function(collection,collectionChangedArgs){var dropArea=$this.element.find(".ui-igpivot-droparea[data-role=columns]");$this._onDataSourceCollectionChanged(collection,collectionChangedArgs,dropArea,$this.options.disableColumnsDropArea)};this._onMeasuresChanged=function(collection,collectionChangedArgs){var dropArea=$this.element.find(".ui-igpivot-droparea[data-role=measures]");$this._onDataSourceCollectionChanged(collection,collectionChangedArgs,dropArea,$this.options.disableMeasuresDropArea)};this._setDataSource();this._makeDroppable(this.element)},_setOption:function(key,value){var dropArea,droppable;switch(key){case"dataSource":this._clearDataSource();this.options.dataSourceOptions=null;this.options.dataSource=this._createDataSource(value,null);this._setDataSource();break;case"dataSourceOptions":this._clearDataSource();this.options.dataSourceOptions=value;this.options.dataSource=this._createDataSource(null,value);this._setDataSource();break;case"deferUpdate":$.Widget.prototype._setOption.apply(this,arguments);this._deferUpdate=value;this.element.find(".ui-igpivotdataselector-deferupdate").attr("checked",value);this.element.find(".ui-igpivotdataselector-updatelayout").igButton(value?"enable":"disable");break;case"width":$.Widget.prototype._setOption.apply(this,arguments);this.element.width(value);this.element.find(".ui-igpivotdataselector-catalog, .ui-igpivotdataselector-cube, .ui-igpivotdataselector-measuregroup").igCombo("option","width",this.element.children(".ui-igpivotdataselector-root").width());break;case"height":$.Widget.prototype._setOption.apply(this,arguments);this.element.height(value);break;case"dragAndDropSettings":$.Widget.prototype._setOption.apply(this,arguments);this.element.find(":ui-draggable").each(function(ind,el){var draggable=$(el);draggable.draggable("option","appendTo",value.appendTo);draggable.draggable("option","containment",value.containment);draggable.draggable("option","zIndex",value.zIndex)});break;case"disableRowsDropArea":$.Widget.prototype._setOption.apply(this,arguments);dropArea=this.element.find(".ui-igpivot-droparea[data-role=rows]");if(value){droppable=dropArea.data(_droppable);if(droppable){droppable.destroy()}}else if(!$.ig.util.isTouchDevice()){dropArea.droppable(this._createDropAreaOptions())}this._fillDropArea(dropArea,this._ds.rowAxis(),this.options.disableRowsDropArea);break;case"disableColumnsDropArea":$.Widget.prototype._setOption.apply(this,arguments);dropArea=this.element.find(".ui-igpivot-droparea[data-role=columns]");if(value){droppable=dropArea.data(_droppable);if(droppable){droppable.destroy()}}else if(!$.ig.util.isTouchDevice()){dropArea.droppable(this._createDropAreaOptions())}this._fillDropArea(dropArea,this._ds.columnAxis(),this.options.disableColumnsDropArea);break;case"disableMeasuresDropArea":$.Widget.prototype._setOption.apply(this,arguments);dropArea=this.element.find(".ui-igpivot-droparea[data-role=measures]");if(value){droppable=dropArea.data(_droppable);if(droppable){droppable.destroy()}}else if(!$.ig.util.isTouchDevice()){dropArea.droppable(this._createDropAreaOptions())}this._fillDropArea(dropArea,this._ds.measures(),this.options.disableMeasuresDropArea);break;case"disableFiltersDropArea":$.Widget.prototype._setOption.apply(this,arguments);dropArea=this.element.find(".ui-igpivot-droparea[data-role=filters]");if(value){droppable=dropArea.data(_droppable);if(droppable){droppable.destroy()}}else if(!$.ig.util.isTouchDevice()){dropArea.droppable(this._createDropAreaOptions())}this._fillDropArea(dropArea,this._ds.filters(),this.options.disableFiltersDropArea);break}},_initUI:function(){var $this=this,dataSource=this._ds,rootDiv,comboOptions,droppableOptions,dropAreasTable,tableRow,tableHeader,tableColumn,dropArea,deferUpdateCheck,deferUpdateLabel;if(this.options.width){this.element.width(this.options.width)}if(this.options.height){this.element.height(this.options.height)}rootDiv=$("<div class='ui-widget "+this.css.dataSelectorRoot+"'></div>").appendTo(this.element);if(this._isInstance(dataSource,"OlapXmlaDataSource")){comboOptions={textKey:"_caption",valueKey:"_name",mode:"dropdown",enableClearButton:false,width:rootDiv.width()};$("<input class='"+this.css.catalog+"' />").appendTo(rootDiv).igCombo($.extend({placeHolder:$.ig.PivotDataSelector.locale.catalog,selectionChanged:function(evt,ui){$this._onCatalogSelected(ui.items[0].data.caption())}},comboOptions));$("<input class='"+this.css.cube+"' />").appendTo(rootDiv).igCombo($.extend({placeHolder:$.ig.PivotDataSelector.locale.cube,selectionChanged:function(evt,ui){$this._onCubeSelected(ui.items[0].data.caption())}},comboOptions));$("<input class='"+this.css.measureGroup+"' />").appendTo(rootDiv).igCombo($.extend({placeHolder:$.ig.PivotDataSelector.locale.measureGroup,selectionChanged:function(evt,ui){$this._onMeasureGroupSelected(ui.items[0].data.caption())}},comboOptions))}$("<div class='"+this.css.metadata+"'></div>").appendTo(rootDiv).addClass().igTree({initialExpandDepth:0,bindings:{nodeContentTemplate:"<span class='ui-igpivot-metadataitem' data-name='${name}' data-type='${type}'><span class='${image}'/>${caption}</span>",childDataProperty:"children"},rendered:function(evt,ui){ui.owner.element.removeAttr("data-scroll")}});droppableOptions=this._createDropAreaOptions();dropAreasTable=$("<table class='"+this.css.dropAreasTable+"'></table>").appendTo(rootDiv);tableRow=$("<tr></tr>").appendTo(dropAreasTable);tableHeader=$("<th></th>").appendTo(tableRow);$("<span class='"+this.css.filtersIcon+"'></span>").appendTo(tableHeader);$("<span></span>").text($.ig.PivotDataSelector.locale.filters).appendTo(tableHeader);tableHeader=$("<th></th>").appendTo(tableRow);$("<span class='"+this.css.columnsIcon+"'></span>").appendTo(tableHeader);$("<span></span>").text($.ig.PivotDataSelector.locale.columns).appendTo(tableHeader);tableRow=$("<tr></tr>").appendTo(dropAreasTable);tableColumn=$("<td></td>").appendTo(tableRow);dropArea=$("<ul class='"+this.css.dropArea+"' data-role='filters'></ul>").appendTo(tableColumn);if(!this.options.disableFiltersDropArea&&!$.ig.util.isTouchDevice()){dropArea.droppable(droppableOptions)}tableColumn=$("<td></td>").appendTo(tableRow);dropArea=$("<ul class='"+this.css.dropArea+"' data-role='columns'></ul>").appendTo(tableColumn);if(!this.options.disableColumnsDropArea&&!$.ig.util.isTouchDevice()){dropArea.droppable(droppableOptions)}tableRow=$("<tr></tr>").appendTo(dropAreasTable);tableHeader=$("<th></th>").appendTo(tableRow);$("<span class='"+this.css.rowsIcon+"'></span>").appendTo(tableHeader);$("<span></span>").text($.ig.PivotDataSelector.locale.rows).appendTo(tableHeader);tableHeader=$("<th></th>").appendTo(tableRow);$("<span class='"+this.css.measuresIcon+"'></span>").appendTo(tableHeader);$("<span></span>").text($.ig.PivotDataSelector.locale.measures).appendTo(tableHeader);tableRow=$("<tr></tr>").appendTo(dropAreasTable);tableColumn=$("<td></td>").appendTo(tableRow);dropArea=$("<ul class='"+this.css.dropArea+"' data-role='rows'></ul>").appendTo(tableColumn);if(!this.options.disableRowsDropArea&&!$.ig.util.isTouchDevice()){dropArea.droppable(droppableOptions)}tableColumn=$("<td></td>").appendTo(tableRow);dropArea=$("<ul class='"+this.css.dropArea+"' data-role='measures'></ul>").appendTo(tableColumn);if(!this.options.disableMeasuresDropArea&&!$.ig.util.isTouchDevice()){dropArea.droppable(droppableOptions)}if($.ig.util.isTouchDevice()){dropAreasTable.find(".ui-igpivot-droparea").igScroll()}this._deferUpdate=this.options.deferUpdate;deferUpdateLabel=$("<label></label>").appendTo(rootDiv).text($.ig.PivotDataSelector.locale.deferUpdate);deferUpdateCheck=$("<input class='ui-igpivotdataselector-deferupdate' type='checkbox' />").prependTo(deferUpdateLabel).change(function(event){$this._deferUpdate=$(event.target).is(":checked");if($this._deferUpdate){$this.element.find(".ui-igpivotdataselector-updatelayout").igButton("enable")}else{$this.element.find(".ui-igpivotdataselector-updatelayout").igButton("disable");$this._updateDataSource()}$this._triggerDeferUpdateChanged($this._deferUpdate)});$("<button class='"+this.css.updateLayout+"'></button>").attr("title",$.ig.PivotDataSelector.locale.updateLayout).appendTo(rootDiv).igButton({text:false,icons:{primary:"ui-icon-refresh"}}).igButton(this._deferUpdate?"enable":"disable").click(function(){$this._updateDataSource(true)});this._triggerDataSelectorRendered()},_clearUI:function(){this.element.find(".ui-igpivotdataselector-catalog, .ui-igpivotdataselector-cube, .ui-igpivotdataselector-measuregroup").igCombo("destroy");this.element.find(".ui-igpivotdataselector-metadata").igTree("destroy");this.element.find(".ui-igpivot-droparea .ui-igpivot-metadataitem.ui-draggable").draggable("destroy").remove();this.element.empty()},_setDataSource:function(){var $this=this,dataSource;this._ds=dataSource=this._createDataSource(this.options.dataSource,this.options.dataSourceOptions);this._initUI();if(!dataSource){return}this.timestamp=(new Date).getTime();$(dataSource).bind("initialized.dataselector"+this.timestamp,function(evt,args){$this._onDataSourceInitialized(evt,args)});$(dataSource).bind("updated.dataselector"+this.timestamp,function(evt,args){$this._onDataSourceUpdated(evt,args)});if(dataSource.isInitialized()){if(this._isInstance(dataSource,"OlapXmlaDataSource")){this._onCatalogSelected(this._getItemName(dataSource.catalog()),true)}else{this._fillMetadata(dataSource.metadataTree())}dataSource.bindCollectionChanged({filters:this._onFiltersChanged,rowAxis:this._onRowAxisChanged,columnAxis:this._onColumnAxisChanged,measures:this._onMeasuresChanged})}else{dataSource.initialize()}},_clearDataSource:function(){if(this._ds){$(this._ds).unbind("updated.dataselector");this._ds.unbindCollectionChanged({filters:this._onFiltersChanged,rowAxis:this._onRowAxisChanged,columnAxis:this._onColumnAxisChanged,measures:this._onMeasuresChanged})}this._filterMembersCache=[];this._clearUI()},_onDataSourceInitialized:function(evt,evtArgs){var dataSource=this._ds,args=$.extend({owner:this,dataSource:dataSource},evtArgs);dataSource.bindCollectionChanged({filters:this._onFiltersChanged,rowAxis:this._onRowAxisChanged,columnAxis:this._onColumnAxisChanged,measures:this._onMeasuresChanged});this._triggerDataSourceInitialized(evt,args);if(!evtArgs.error){if(this._isInstance(dataSource,"OlapXmlaDataSource")){this._onCatalogSelected(this._getItemName(dataSource.catalog()),true)}else{this._fillMetadata(dataSource.metadataTree())}}},_updateDataSource:function(deferUpdateOverride){var dataSource=this._ds,shouldUpdate=deferUpdateOverride||this._deferUpdate===false;if(shouldUpdate&&dataSource.cube()!==null){dataSource.update()}},_onDataSourceUpdated:function(evt,evtArgs){var dataSource=this._ds,args=$.extend({owner:this,dataSource:dataSource},evtArgs);this._triggerDataSourceUpdated(evt,args)},_getItemName:function(item){return item&&item.name()},_fillCombo:function(comboSelector,items,selectedItem){var t=typeof selectedItem;this.element.find(comboSelector).igCombo("option","dataSource",items);if(t==="number"&&selectedItem!==null){this.element.find(comboSelector).igCombo("index",selectedItem)}else if(t==="object"&&selectedItem!==null&&$.isFunction(selectedItem.name)){this.element.find(comboSelector).igCombo("value",selectedItem.name())}},_clearCombo:function(comboSelector){this.element.find(comboSelector).igCombo("deselectAll").igCombo("option","dataSource",null)},_fillMetadata:function(metadata){var $this=this,parseMetadata=function(m){var metadataItem={},hasItem=true,item,imgClass,children,i;switch(m.type()){case $.ig.OlapMetadataTreeItemType.prototype.cube:imgClass="cube";break;case $.ig.OlapMetadataTreeItemType.prototype.dimension:imgClass="dimension";break;case $.ig.OlapMetadataTreeItemType.prototype.group:imgClass="folder";hasItem=false;break;case $.ig.OlapMetadataTreeItemType.prototype.userDefinedHierarchy:imgClass="hierarchymultiple";break;case $.ig.OlapMetadataTreeItemType.prototype.systemEnabledHierarchy:imgClass="hierarchysingle";break;case $.ig.OlapMetadataTreeItemType.prototype.parentChildHierarchy:imgClass="hierarchydirect";break;case $.ig.OlapMetadataTreeItemType.prototype.measure:imgClass="measure";break;case $.ig.OlapMetadataTreeItemType.prototype.level1:imgClass="level1";break;case $.ig.OlapMetadataTreeItemType.prototype.level2:imgClass="level2";break;case $.ig.OlapMetadataTreeItemType.prototype.level3:imgClass="level3";break;case $.ig.OlapMetadataTreeItemType.prototype.level4:imgClass="level4";break;case $.ig.OlapMetadataTreeItemType.prototype.level5:imgClass="level5";break;case $.ig.OlapMetadataTreeItemType.prototype.kpiRoot:case $.ig.OlapMetadataTreeItemType.prototype.kpi:imgClass="kpi";break;case $.ig.OlapMetadataTreeItemType.prototype.kpiValue:imgClass="kpi value";break;case $.ig.OlapMetadataTreeItemType.prototype.kpiGoal:imgClass="kpi goal";break;case $.ig.OlapMetadataTreeItemType.prototype.kpiStatus:imgClass="kpi status";break;case $.ig.OlapMetadataTreeItemType.prototype.kpiTrend:imgClass="kpi trend";break;case $.ig.OlapMetadataTreeItemType.prototype.kpiWeight:imgClass="kpi weight";break;default:imgClass="folder";break}metadataItem.caption=m.caption();metadataItem.image=imgClass;if(hasItem){item=m.item();metadataItem.name=item.uniqueName();metadataItem.type=item.getType().typeName()}children=m.children();if(children){metadataItem.children=[];for(i=0;i<children.length;i++){metadataItem.children[i]=parseMetadata(children[i])}}return metadataItem},parsedMetadata=metadata===null?[]:[parseMetadata(metadata)],dragAndDropSettings=this.options.dragAndDropSettings,tree,items;tree=this.element.find(".ui-igpivotdataselector-metadata").igTree("option","dataSource",parsedMetadata);items=tree.find(".ui-igpivot-metadataitem[data-type='Kpi'],.ui-igpivot-metadataitem[data-type='Measure'],.ui-igpivot-metadataitem[data-type='Dimension'],.ui-igpivot-metadataitem[data-type='Hierarchy'],.ui-igpivot-metadataitem[data-type='KpiMeasure']");if(!$.ig.util.isTouchDevice()){items.draggable({appendTo:dragAndDropSettings.appendTo,containment:dragAndDropSettings.containment,zIndex:dragAndDropSettings.zIndex,cursorAt:this._const.dragCursorAt,revert:false,helper:function(){var markup;markup=$($this._const.dragHelperMarkup.replace("{0}",$(this).text()));markup.addClass($this.css.invalidDropIndicator).find("span").addClass("ui-icon  ui-icon-cancel");return markup},start:function(event,ui){return $this._triggerDragStart(event,ui,tree.igTree("nodeDataFor",$(this).attr("data-path")))},drag:function(event,ui){return $this._triggerDrag(event,ui,tree.igTree("nodeDataFor",$(this).attr("data-path")))},over:function(event,ui){$this._onDraggableOver(event,ui)},out:function(event,ui){$this._onDraggableOut(event,ui)},stop:function(event,ui){$this._triggerDragStop(event,ui)}})}items.click(function(event){var item=tree.igTree("nodeDataFor",$(this).closest("li").attr("data-path")),name=item.name,type=$.ig[item.type].prototype.getType(),metadataItem;metadataItem=$this._ds.getCoreElement(function(el){return el.uniqueName()===name},type);if(metadataItem){$this._createMetadataItemDropDown(event,this,metadataItem)}});this._fillDropArea(".ui-igpivot-droparea[data-role=filters]",this._ds.filters(),this.options.disableFiltersDropArea);this._fillDropArea(".ui-igpivot-droparea[data-role=rows]",this._ds.rowAxis(),this.options.disableRowsDropArea);this._fillDropArea(".ui-igpivot-droparea[data-role=columns]",this._ds.columnAxis(),this.options.disableColumnsDropArea);this._fillDropArea(".ui-igpivot-droparea[data-role=measures]",this._ds.measures(),this.options.disableMeasuresDropArea)},_fillDropArea:function(dropArea,items,isDisabled){var i,length;dropArea=dropArea.jquery?dropArea:this.element.find(dropArea);dropArea.empty();for(i=0,length=items.length;i<length;i++){this._createMetadataElement(items[i],isDisabled,"appendTo",dropArea)}},_clearMetadata:function(){this.element.find(".ui-igpivotdataselector-metadata").igTree("option","dataSource",[])},_onCatalogSelected:function(catalog,isInit){var $this=this,dataSource=this._ds,callback=function(){$this._fillCombo(".ui-igpivotdataselector-catalog",dataSource.catalogs(),dataSource.catalog());$this._fillCombo(".ui-igpivotdataselector-cube",dataSource.cubes(),dataSource.cube());$this._onCubeSelected($this._getItemName(dataSource.cube()),isInit)};this._clearCombo(".ui-igpivotdataselector-cube");if(catalog===null){callback()}else{dataSource.setCatalog(catalog).done(callback)}},_onCubeSelected:function(cube,isInit){var $this=this,dataSource=this._ds,callback=function(){var measureGroup=dataSource.cube()&&(dataSource.measureGroup()||0),measureGroups=dataSource.measureGroups().slice();measureGroups.splice(0,0,{_caption:$.ig.PivotDataSelector.locale.measureGroupAll,_name:$.ig.PivotDataSelector.locale.measureGroupAll});$this._fillCombo(".ui-igpivotdataselector-measuregroup",measureGroups,measureGroup);$this._onMeasureGroupSelected($this._getItemName(dataSource.measureGroup()),isInit)};this._clearCombo(".ui-igpivotdataselector-measuregroup");if(cube===null){callback()}else{dataSource.setCube(cube).done(callback)}},_onMeasureGroupSelected:function(measureGroup,isInit){var $this=this,dataSource=this._ds,callback=function(){$this._fillMetadata(dataSource.metadataTree());if(!isInit){$this._updateDataSource()}};this._clearMetadata();if(measureGroup===null){callback()}else{dataSource.setMeasureGroup(measureGroup).done(callback)}},_shouldAppendToTarget:function(target,dragged){return target.offset().top+target.height()/2<dragged.offset.top+this._const.dragCursorAt.top},update:function(){this._ds.update()},destroy:function(){this._clearDataSource();this.element.removeClass(this.css.dataSelector);$(this._ds).unbind("updated.dataselector"+this.timestamp);$(this._ds).unbind("initialized.dataselector"+this.timestamp);$.Widget.prototype.destroy.apply(this);return this},_triggerDataSelectorRendered:function(){var args={owner:this};this._trigger("dataSelectorRendered",null,args)},_triggerDeferUpdateChanged:function(deferUpdate){var args={owner:this,deferUpdate:deferUpdate};this._trigger("deferUpdateChanged",null,args)}});$.extend(true,$.ui.igPivotDataSelector.prototype,$.ig.Pivot._pivotShared);$.extend($.ui.igPivotDataSelector,{version:"15.2.20152.1027"})})(jQuery);