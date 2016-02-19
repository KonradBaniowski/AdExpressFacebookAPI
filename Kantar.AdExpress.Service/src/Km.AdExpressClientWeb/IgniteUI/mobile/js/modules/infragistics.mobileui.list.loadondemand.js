﻿/*!@license
 * Infragistics.Web.MobileUI ListView LoadOnDemand 15.2.20152.1027
 *
 * Copyright (c) 2011-2015 Infragistics Inc.
 *
 * http://www.infragistics.com/
 *
 * Depends on:
 *  jquery-1.7.2.js
 *  jquery.mobile-1.2.0.js
 *  infragistics.mobileui.list.js
 *	infragistics.dataSource.js
 *  infragistics.ui.shared.js
 *	infragistics.util.js
 */
(function($){$.widget("mobile.igListViewLoadOnDemand",$.mobile.widget,{css:{loadMoreBtn:"ui-iglist-load-more"},options:{type:null,pageSize:10,recordCountKey:null,pageSizeUrlKey:null,pageIndexUrlKey:null,mode:"button",autoHideButtonAtEnd:true,loadButtonTheme:null},events:{itemsRequesting:"itemsRequesting",itemsRequested:"itemsRequested"},_create:function(){this._maxPosition=0;this._lastItemCount=0},_setOption:function(key,value){var o=this.options,css=this.css,btn,hidden;if(o[key]===value){return}$.Widget.prototype._setOption.apply(this,arguments);if(key==="type"||key==="mode"){throw new Error(this.list._locale("optionChangeNotSupported")+" "+key)}else if((key==="loadButtonTheme"||key==="loadMoreItemsLabel")&&o.mode==="button"){btn=this.list.container().find("."+css.loadMoreBtn).first();hidden=btn.css("display")==="none";btn.after(this._createAddMoreButton());btn.remove();if(hidden){this.list.container().find("."+css.loadMoreBtn).css("display","none")}}else if(key==="autoHideButtonAtEnd"&&o.mode==="button"){if(o.autoHideButtonAtEnd&&(this.list.dataSource.pageCount()<=1||this.list.dataSource.pageIndex()>=this.list.dataSource.pageCount()-1)){this.list.container().find("."+css.loadMoreBtn).hide()}else{this.list.container().find("."+css.loadMoreBtn).show()}}},_locale:function(key){var val=this.options[key];if(val===undefined||val===null){val=$.ig.mobileListViewLoadOnDemand&&$.ig.mobileListViewLoadOnDemand.locale?$.ig.mobileListViewLoadOnDemand.locale[key]:null}return val||""},_footerRendering:function(evnt,args){if(this.options.mode!=="button"){return}this._createAddMoreButton().appendTo(args.footerContents)},_createAddMoreButton:function(){var self=this,noCancel;return $("<div>").addClass(this.css.loadMoreBtn).text(this._locale("loadMoreItemsLabel")).bind("click",function(){noCancel=self._trigger(self.events.itemsRequesting,null,{owner:self,newPageIndex:self.list.dataSource.settings.paging.pageIndex+1});if(noCancel){self._shouldFireItemsRequested=true;if(self.list.dataSource.pageIndex()<self.list.dataSource.pageCount()-1){$.mobile.loading("show");self._oldIndex=self.list.dataSource.dataView().length}self.list.dataSource.nextPage();$(this).trigger("vmouseout").blur()}}).buttonMarkup({inline:false,theme:self.options.loadButtonTheme})},_itemsRendering:function(evnt,args){var dataSource=this.list.dataSource,filtering,oldPageIndex=null;if(this._shouldFireItemsRequested&&dataSource.settings.paging.type==="remote"&&dataSource.settings.filtering.type==="local"&&dataSource.settings.filtering.expressions&&dataSource.settings.filtering.expressions.length>0){filtering=this.list.element.data("igListViewFiltering");if(filtering){oldPageIndex=dataSource.settings.paging.pageIndex;filtering._refilter(false);delete this.list._requireRecordsClear}}if(dataSource.settings.paging.type==="remote"&&dataSource.settings.sorting.type==="local"&&dataSource.settings.sorting.expressions&&dataSource.settings.sorting.expressions.length>0){this.list._requireRecordsClear=true;return}args.index=dataSource.settings.paging.pageIndex*dataSource.settings.paging.pageSize;if(oldPageIndex!==null){dataSource.settings.paging.pageIndex=oldPageIndex;args.index=this._oldIndex;delete this._oldIndex}if(this._skippedDivider&&args.index>0){args.index--}if(args.listElement.attr("data-empty")){this.list._cleanupList().attr("data-empty",null)}else{if(this.list.options.inset){args.listElement.children().last().removeClass("ui-corner-bottom").find(".ui-btn-inner").not(".ui-li-link-alt span:first-child").removeClass("ui-corner-bottom").end().end().find(".ui-li-thumb").not(".ui-li-icon").removeClass("ui-corner-bl")}else{args.listElement.children().last().removeClass(this.list.css.lastNonInsetItem)}}},_itemsRendered:function(){var $doc,$win,docHeight,winHeight,self=this,val,options=this.options,css=this.css,list=this.list,listOptions=list.options,listLevel,hList=listOptions._hParent,hOpts=hList.options,listItem,parentOptions,parentListItem,pageIndex,i,childDS,result;if(listOptions.bindings&&listOptions.bindings.isDividerKey&&list.dataSource.dataView().length>0){val=list.dataSource.dataView()[list.dataSource.dataView().length-1];if(val[listOptions.bindings.isDividerKey]===true){this._skippedDivider=true}else{delete this._skippedDivider}}else{delete this._skippedDivider}if(this._shouldFireItemsRequested){if(options.autoHideButtonAtEnd&&list.dataSource.pageIndex()===list.dataSource.pageCount()-1){list.container().find("."+css.loadMoreBtn).hide();if(options.mode==="automatic"){$(document).unbind("scroll",this._docScrolledHandler);this._notBoundScroll=true}}listLevel=parseInt(list.element.attr("data-level"),10);if(hList&&(listLevel<hOpts.initialDataBindDepth||hOpts.initialDataBindDepth===-1)){pageIndex=list.dataSource.settings.paging.pageIndex;if(listLevel>0){listItem=listOptions._parent;parentOptions=listItem.closest(".ig-listview").data("igFlatList").options;parentListItem=parentOptions._parent;result=hList._getChildDataSource(parentOptions,listOptions,listItem,parentListItem);childDS=hList._hds.dataAt(result.itemId,result.keyspath+result.keyspathvar)}else{childDS=hList._hds.root()}if(childDS&&($.type(childDS)==="array"||listOptions.responseDataKey&&$.type(childDS[listOptions.responseDataKey])==="array")){for(i=pageIndex*list.dataSource.settings.paging.pageSize;i<list.dataSource._dataView.length;++i){if(listOptions.responseDataKey){childDS[listOptions.responseDataKey][i]=list.dataSource._dataView[i]}else{childDS[i]=list.dataSource._dataView[i]}}}}this._trigger(this.events.itemsRequested,null,{owner:this});this._shouldFireItemsRequested=false}else{if(options.autoHideButtonAtEnd&&(list.dataSource.pageCount()<=1||list.dataSource.pageIndex()>=list.dataSource.pageCount()-1)){list.container().find("."+css.loadMoreBtn).hide();if(options.mode==="automatic"){$(document).unbind("scroll",this._docScrolledHandler);this._notBoundScroll=true}}else{this.list.container().find("."+css.loadMoreBtn).show();if(this._notBoundScroll){$(document).bind("scroll",this._docScrolledHandler);delete this._notBoundScroll}}}$.mobile.loading("hide");if(options.mode==="automatic"&&this.list.dataSource.pageIndex()<this.list.dataSource.pageCount()-1&&(this._docScrolledFired||!this._loadedAfterRender)){$doc=$(document);docHeight=$doc.height();$win=$(window);winHeight=$win.height();if(docHeight<=winHeight){setTimeout(function(){var noCancel;noCancel=self._trigger(self.events.itemsRequesting,null,{owner:self,newPageIndex:self.list.dataSource.settings.paging.pageIndex+1});if(noCancel){if(!this._docScrolledFired){this._loadedAfterRender=true}self._shouldFireItemsRequested=true;if(self.list.dataSource.pageIndex()<self.list.dataSource.pageCount()-1){$.mobile.loading("show")}self.list.dataSource.nextPage();$.mobile.loading("hide")}})}}if(this.list.dataSource.dataView().length<this._lastItemCount){this._maxPosition=0}this._lastItemCount=this.list.dataSource.dataView().length},_docScrolled:function(){var noCancel=false,$win=$(window),winHeight=$win.height(),scrollTop=$win.scrollTop(),lastItem=this.list.listElement.children().last(),itemOffset=lastItem.offset(),activePage=$.mobile.activePage,listPage=this.list.element.closest(".ui-page");this._docScrolledFired=true;if(this._loadedAfterRender){delete this._loadedAfterRender;return}if(activePage&&activePage[0]===listPage[0]&&this._maxPosition<winHeight+scrollTop&&itemOffset&&itemOffset.top-lastItem.outerHeight()<=winHeight+scrollTop&&!this._shouldFireItemsRequested){noCancel=this._trigger(this.events.itemsRequesting,null,{owner:this,newPageIndex:this.list.dataSource.settings.paging.pageIndex+1});if(noCancel){if(this.list.dataSource.pageIndex()<this.list.dataSource.pageCount()-1){$.mobile.loading("show");this._shouldFireItemsRequested=true}this._maxPosition=winHeight+scrollTop;this.list.dataSource.nextPage()}}},_createHandlers:function(){this._footerRendering=$.proxy(this._footerRendering,this);this._itemsRenderingHandler=$.proxy(this._itemsRendering,this);this._itemsRenderedHandler=$.proxy(this._itemsRendered,this);if(this.options.mode==="automatic"){this._docScrolledHandler=$.proxy(this._docScrolled,this)}},_registerEvents:function(){var listElement=this.list.element;listElement.bind("iglistfooterrendering",this._footerRendering);listElement.bind("iglistitemsrendering",this._itemsRenderingHandler);listElement.bind("iglistitemsrendered",this._itemsRenderedHandler);if(this.options.mode==="automatic"){$(document).bind("scroll",this._docScrolledHandler)}},_unregisterEvents:function(){var listElement=this.list.element;listElement.unbind("iglistfooterrendering",this._footerRendering);listElement.unbind("iglistitemsrendering",this._itemsRenderingHandler);listElement.unbind("iglistitemsrendered",this._itemsRenderedHandler);if(this.options.mode==="automatic"){$(document).unbind("scroll",this._docScrolledHandler)}delete this.footerRendering;delete this._itemsRenderingHandler;delete this._itemsRenderedHandler;delete this._docScrolledHandler},_injectList:function(listInstance){var options=this.options,css=$.extend(true,{},$.mobile.igListViewLoadOnDemand.prototype.css),paging=listInstance.dataSource.settings.paging;this.list=listInstance;if(!options.loadButtonTheme){options.loadButtonTheme=$.mobile.getInheritedTheme(this.list.element,"c")}css.loadMoreBtn=css.loadMoreBtn.replace("{0}",options.loadButtonTheme);if(options.type===null){options.type=this.list._inferOpType()}if(options.type){paging.type=this.options.type}else{paging.type="remote"}paging.enabled=true;if(options.pageSizeUrlKey){paging.pageSizeUrlKey=options.pageSizeUrlKey}if(options.pageIndexUrlKey){paging.pageIndexUrlKey=options.pageIndexUrlKey}if(options.recordCountKey!==null){listInstance.dataSource.settings.responseTotalRecCountKey=options.recordCountKey}paging.pageSize=options.pageSize;paging.pageIndex=0;paging.appendPage=true;this.list._loadOnDemand=true;this._createHandlers();this._registerEvents()},destroy:function(){this._unregisterEvents();if(this.options.mode==="button"){this.list.container().find("."+this.css.loadMoreBtn).remove()}delete this.list._loadOnDemand;delete this._docScrolledFired;this.list._requireRecordsClear=true;delete this._notBoundScroll;this.list.dataSource.settings.paging.enabled=false;this.list.dataSource.settings.paging.pageIndex=0;if(this.options.type==="local"){this.list.dataSource.pageSize(this.list.dataSource._data.length)}else{this.list.dataSource.dataBind()}$.Widget.prototype.destroy.call(this);return this}});function _addDash(c){return"-"+c.toLowerCase()}$(document).bind("_igListViewHtmlOptions",function(evnt,args){var elem=args.element,lod,option,value;if(elem.jqmData("load-on-demand")===true){if(!args.options.features){args.options.features=[]}lod=args.options.features[args.options.features.length]={};lod.name="LoadOnDemand";for(option in $.mobile.igListViewLoadOnDemand.prototype.options){if($.mobile.igListViewLoadOnDemand.prototype.options.hasOwnProperty(option)){value=elem.jqmData("load-on-demand-"+option.replace(/[A-Z]/g,_addDash));if(value!==undefined){lod[option]=value}}}}});if(typeof define==="function"&&define.amd&&define.amd.jQuery){define("ig.mobileui.list.loadondemand",["ig.mobileui.list"],function(){return $.mobile.igListViewLoadOnDemand})}$.extend($.mobile.igListViewLoadOnDemand,{version:"15.2.20152.1027"})})(jQuery);