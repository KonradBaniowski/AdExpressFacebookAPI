﻿/*!@license
 * Infragistics.Web.MobileUI ListView Filtering 15.2.20152.1027
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
(function($){$.widget("mobile.igListViewFiltering",$.mobile.widget,{css:{filterKeywordArea:"ui-iglist-filter-key-word-area",filterPresets:"ui-iglist-filter-presets",filterScopeOptions:"ui-iglist-keyword-scope-options",preset:"ui-btn-inline ui-iglist-preset",presetSelected:"ui-iglist-preset-selected",presetHidden:"ui-iglist-preset-hidden",keywordFooterText:"ui-iglist-tray-footer-item ig-tray-keyword",presetFooterText:"ui-iglist-tray-footer-item ig-tray-filter-preset",separatorFooterText:"ui-iglist-tray-footer-sep",keywordSearchScopeHidden:"ui-iglist-search-scope-hidden",keywordInputContainer:"ui-iglist-input-search-container",keywordInputContainerWithCancel:"ui-igelastic-block",cancelButtonContainer:"ui-igelastic-block ui-iglist-button-cancel-container"},options:{type:null,caseSensitive:false,filterExprUrlKey:null,searchBarEnabled:true,searchBarFields:[{text:"",fieldName:"",condition:""}],searchBarFieldName:"",searchBarPlaceHolder:null,searchBarCondition:"contains",searchBarAllFieldsCondition:"contains",filteredFields:[{fieldName:"",searchValue:"",condition:"",logic:"AND"}],filterPresetsLabel:null,filterState:"default",presets:[{text:"",filteredFields:[{fieldName:"",searchValue:"",condition:"",logic:"AND"}]}]},events:{presetChanging:"presetChanging",presetChanged:"presetChanged",keywordChanging:"keywordChanging",keywordChanged:"keywordChanged",scopeChanging:"scopeChanging",scopeChanged:"scopeChanged"},_createWidget:function(){var options=this.options;options.filteredFields=[];options.presets=[];options.searchBarFields=[];$.Widget.prototype._createWidget.apply(this,arguments)},_create:function(){var self=this;this._searchScopeTapHandler=function(evnt){var target=self.list._itemFromTarget(evnt.target,"li","idx"),$li,liLeft,liWidth,$div,divWidth,divScrollLeft,newLeft,noCancel,newName,index,fields,field,condition,x,args;if(target&&target.getAttribute("scoping")===null){$li=$(target);index=$li.attr("idx");if(index!=="all"){newName=self.options.searchBarFields[index].fieldName;condition=self.options.searchBarFields[index].condition;if(!condition){fields=self.list.dataSource.schema().fields();if(!fields||fields.length===0){fields=self.list.options.schema.fields}condition="contains";for(x=0;x<fields.length;++x){field=fields[x];if(field.name===newName){if(field.type==="number"||field.type==="date"){condition="equals"}break}}}}else{newName="";condition=self.options.searchBarAllFieldsCondition||"contains"}args={scopeField:newName,condition:condition,owner:self};noCancel=self._trigger(self.events.scopeChanging,null,args);if(noCancel){liLeft=$li.offset().left;liWidth=$li.outerWidth();$div=$li.parent().parent();divWidth=$div.outerWidth();divScrollLeft=$div.scrollLeft();$li.attr("scoping",1).addClass(self.css.presetSelected).siblings().attr("scoping",null).removeClass(self.css.presetSelected);self.options.searchBarFieldName=args.scopeField;self.options.searchBarCondition=args.condition;if(liLeft+liWidth>divWidth){newLeft=divScrollLeft+liWidth;$div.animate({scrollLeft:newLeft},function(){self._refreshPresets($div)})}else if(liLeft<0){newLeft=divScrollLeft-liWidth;$div.animate({scrollLeft:newLeft},function(){self._refreshPresets($div)})}if(self._filterInput.val()!==""){$.mobile.loading("show");self._shouldFireScopeChanged=true;self._refilter()}else{self._trigger(self.events.scopeChanged,null,{scopeField:newName,owner:self})}}}};this._presetTapHandler=function(evnt){var target=self.list._itemFromTarget(evnt.target,"li","idx"),$li,state,noCancel;if(target&&target.getAttribute("filtered")===null){$li=$(target);state=$li.attr("idx");noCancel=self._trigger(self.events.presetChanging,null,{state:state,owner:self});if(noCancel){self._adjustFilterState(state,$li);$.mobile.loading("show");self._shouldFirePresetChanged=true;self._refilter()}}};this.css.preset=this.css.preset.replace("{0}","c")},_setOption:function(key,value){var o=this.options,css=this.css;if(o[key]===value){return}$.Widget.prototype._setOption.apply(this,arguments);if(key==="type"||key==="presets"||key==="searchBarEnabled"||key==="searchBarFields"){throw new Error(this.list._locale("optionChangeNotSupported")+" "+key)}else if(key==="caseSensitive"){this.list.dataSource.settings.filtering.caseSensitive=this.options.caseSensitive}else if(key==="filterState"){this._adjustFilterState(value);this._refilter()}else if(key==="filterPresetsLabel"){this.list.container().find("."+css.filterPresets).children("span").html(this._locale("filterPresetsLabel"))}else if(key==="keywordSearchLabel"){this.list.container().find("."+css.filterKeywordArea).children("span").html(this._locale("keywordSearchLabel"))}else if(key==="filteredFields"||(key==="searchBarFieldName"||key==="searchBarCondition")&&o.searchBarEnabled){this._refilter()}else if(key==="searchBarPlaceHolder"&&o.searchBarEnabled){this._filterInput.attr("placeholder",this._locale("searchBarPlaceHolder"))}},_locale:function(key){var val=this.options[key];if(val===undefined||val===null){val=$.ig.mobileListViewFiltering&&$.ig.mobileListViewFiltering.locale?$.ig.mobileListViewFiltering.locale[key]:null}return val||""},_adjustFilterState:function(filterState,li){var self=this,liLeft,liWidth,$div,divWidth,divScrollLeft,newLeft,options=this.options,css=this.css;if(li===null||li===undefined){li=this.list.container().find("."+css.filterPresets).find('[idx="'+filterState+'"]')}options.filterState=filterState;if(li&&li.length>0){liLeft=li.offset().left;liWidth=li.outerWidth();$div=li.parent().parent();divWidth=$div.outerWidth();divScrollLeft=$div.scrollLeft();li.attr("filtered",1).addClass(css.presetSelected).siblings().attr("filtered",null).removeClass(css.presetSelected);if(liLeft+liWidth>divWidth){newLeft=divScrollLeft+liWidth;$div.animate({scrollLeft:newLeft},function(){self._refreshPresets($div)})}else if(liLeft<0){newLeft=divScrollLeft-liWidth;$div.animate({scrollLeft:newLeft},function(){self._refreshPresets($div)})}}this._adjustTrayText(options.presets[options.filterState])},_adjustTrayText:function(preset){var showing;if(this._presetSpan){if(preset){showing=this._presetSpan.text()==="";this._presetSpan.text(preset.text).prev().show();if(showing){if(this._presetSpan.prevAll('[data-trayRole="keyword"]').text()){this._presetSpan.prevAll('[data-trayRole="bullet"]').show()}if(this._presetSpan.nextAll('[data-trayRole="keyword"]').text()){this._presetSpan.nextAll('[data-trayRole="bullet"]').show()}this._presetSpan.nextAll('[data-trayRole="empty"]').hide().next().hide()}}else{this._presetSpan.text("").prev().hide().siblings('[data-trayRole="bullet"]').hide();if(this._presetSpan.prevAll('[data-trayRole="keyword"]').text()&&this._presetSpan.nextAll('[data-trayRole="keyword"]').text()){this._presetSpan.prevAll('[data-trayRole="bullet"]').show()}else{this._presetSpan.nextAll('[data-trayRole="empty"]').show().next().show()}}}},_adjustKeywordTrayText:function(val){var showing,prevShown;if(this._keywordSpan){if(val){showing=this._keywordSpan.text()==="";prevShown=showing;this._keywordSpan.text('"'+val+'"');if(showing){this._keywordSpan.nextAll('[data-trayRole="keyword"]').each(function(){if($(this).text()&&prevShown){$(this).prev().prev().show()}if(!prevShown){prevShown=$(this).text().length>0}});if(prevShown){this._keywordSpan.prev().prev().show()}this._keywordSpan.nextAll('[data-trayRole="empty"]').hide().next().hide()}}else{this._keywordSpan.text("").next().hide();prevShown=false;this._keywordSpan.nextAll('[data-trayRole="keyword"]').each(function(i){if(i>0&&!$(this).text()&&prevShown){$(this).prev().prev().hide();prevShown=false}if(!prevShown){prevShown=$(this).text().length>0}});if(!this._keywordSpan.nextAll('[data-trayRole="keyword"]').text()){this._keywordSpan.nextAll('[data-trayRole="empty"]').show().next().show()}}}},filter:function(fieldExpressions,trayText){var options=this.options,css=this.css;options.filterState="custom";if(options.presets&&options.presets.length>0){this.list.container().find("."+css.filterPresets).find("."+css.presetSelected).removeClass(css.presetSelected).attr("filtered",null);this._adjustTrayText(trayText?{text:trayText}:null)}this._refilter(true,null,fieldExpressions)},_refilter:function(renderData,sortedFields,filteredFields){var x,options=this.options,filtering=options.filteredFields,filters=[],index=0,searchBarFields,val,allFilters=[],schema,fields,condition,needsDate,dataSource=this.list.dataSource,listOptions=this.list.options,fieldName;if(renderData===undefined){renderData=true}for(x=0;x<filtering.length;++x){filters[index++]={fieldName:filtering[x].fieldName,expr:filtering[x].searchValue,cond:filtering[x].condition,logic:filtering[x].logic}}filtering=options.filterState;if(filtering!==null&&filtering!==""&&filtering!=="default"&&filtering!=="custom"){filtering=options.presets[parseInt(filtering,10)].filteredFields}else if(filteredFields&&filtering==="custom"){filtering=filteredFields}else{filtering=[]}for(x=0;x<filtering.length;++x){filters[index++]={fieldName:filtering[x].fieldName,expr:filtering[x].searchValue,cond:filtering[x].condition,logic:filtering[x].logic}}if(options.searchBarEnabled){val=this._filterInput?this._filterInput.val():null;if(options.searchBarFieldName===null||options.searchBarFieldName.length===0){schema=dataSource.schema();fields=schema?schema.fields():null;if(!fields||fields.length===0){fields=this.list.options.schema}if(!(options.type==="remote"&&options.filterExprUrlKey)){for(x=0;x<fields.length&&val;++x){fieldName=fields[x].name;if(!(schema&&schema.schema&&schema.schema.childDataProperty&&schema.schema.childDataProperty.name===fieldName)&&fieldName!==listOptions.bindings.isDividerKey&&fieldName!=="ig_pk"&&fieldName!=="ig_hpk"&&!(listOptions.childLayout&&listOptions.childLayout.key===fieldName)){allFilters[allFilters.length]={fieldName:fieldName,expr:val,cond:options.searchBarCondition,logic:"or"}}}}else if(val){this._needsAll=true}else{delete this._needsAll}}else{searchBarFields=options.searchBarFieldName.split(",");condition=options.searchBarCondition.toLowerCase();needsDate=condition==="on"||condition==="noton"||condition==="after"||condition==="before";if(val&&needsDate){val=new Date(val);if(val.toString()==="Invalid Date"){val=null}else if(options.type==="local"&&this.list.options.enableUtcDates&&dataSource.schema()&&dataSource.schema()._serverOffset!==undefined){val=val.getTime()+dataSource.schema()._serverOffset}}for(x=0;x<searchBarFields.length&&val;++x){filters[index++]={fieldName:searchBarFields[x],expr:val,cond:options.searchBarCondition,logic:"or"}}delete this._needsAll}}delete this._dividers;if(dataSource.settings.sorting.enabled&&(sortedFields&&sortedFields.length>0||!sortedFields&&dataSource.settings.sorting.expressions.length>0)&&this._filterDividers){if(options.type==="local"||!options.filterExprUrlKey||filters.length===0&&allFilters.length===0){filters=this._filterDividers.concat(filters)}else{this._dividers="out"}}else{if(filters.length>0&&this._keepDividers){if(options.type==="local"||!options.filterExprUrlKey){filters=this._keepDividers.concat(filters)}else{this._dividers="keep"}}if(allFilters.length>0&&this._keepDividersAll&&(options.type==="local"||filters.length===0)){if(options.type==="local"||!options.filterExprUrlKey){allFilters=this._keepDividersAll.concat(allFilters)}else{this._dividers="keep"}}}dataSource.settings.filtering.expressions=filters.concat(allFilters);dataSource.settings.paging.appendPage=false;if(dataSource.settings.paging.type==="remote"&&"local"===dataSource.settings.filtering.type){this._oldPagingIndex=dataSource.settings.paging.pageIndex}dataSource.settings.paging.pageIndex=0;this.list._requireRecordsClear=true;if(options.type==="local"){if(filters.length>0||allFilters.length>0){dataSource.filter(filters,"AND",true,allFilters)}else{dataSource.settings.sorting.expressions=[];dataSource.clearLocalFilter()}if(dataSource.settings.sorting.type==="local"&&dataSource.settings.sorting.expressions.length>0&&!sortedFields){dataSource.sort(dataSource.settings.sorting.expressions,"asc")}if(renderData){this.list._renderData()}dataSource.settings.paging.appendPage=true}else if(renderData){dataSource.dataBind()}},_renderingTray:function(e,ui){var self=this,wrapper,changeFunc,x,index,searchScope,insertedSearch,search,$this,val,noCancel,options=this.options,css=this.css,inputSearchContainer;if(e.target.id!==this.list.element[0].id){return}if(options.searchBarEnabled){if(!ui.needTray){ui.needTray=1}ui.css=(ui.css+"-keyword").replace("-one-keyword","-keyword-one");wrapper=$("<div />").addClass(css.filterKeywordArea).append("<span>"+this._locale("keywordSearchLabel")+"</span>");inputSearchContainer=$("<div class='"+css.keywordInputContainer+"' />").appendTo(wrapper);search=this._filterInput=$("<input>",{placeholder:this._locale("searchBarPlaceHolder")}).jqmData("lastval","").attr("data-"+$.mobile.ns+"type","search").bind("keyup change",function(e){$this=$(this);changeFunc=function(){val=$this.val();if(val===$this.jqmData("lastVal")){return}noCancel=self._trigger(self.events.keywordChanging,null,{value:val,owner:self});if(noCancel){$.mobile.loading("show");self._shouldFireKeywordChanged=true;self._adjustKeywordTrayText(val);if(self._expr&&self.options.type==="remote"){self._filterNew=val}else{self._expr=val;self._refilter()}}x=searchScope?searchScope.parent().parent():null;x=x&&x[0]?x.parent()[0]:null;if(x){if(val.length){noCancel=searchScope.hasClass(css.keywordSearchScopeHidden);searchScope.removeClass(css.keywordSearchScopeHidden);x.className=x.className.replace("-keyword","-keyword-scope");if(noCancel){self._refreshPresets(searchScope)}}else{searchScope.addClass(css.keywordSearchScopeHidden);x.className=x.className.replace("-scope","")}if(!$this[0]._setScopeWidths){$this[0]._setScopeWidths=true;searchScope.find("li").each(function(){var li=$(this);li.width(li.outerWidth())})}}$this.jqmData("lastVal",val)};val=$this.val();if(e&&e.type==="change"||val!==$this.jqmData("lastVal")){e=null}e=e?e.keyCode||e.which:null;if(!e||e===8&&!val||e>8&&e<46&&e!==32){changeFunc()}else{setTimeout(changeFunc,50)}}).appendTo(inputSearchContainer).textinput({clearSearchButtonText:this._locale("clearSearchButtonText")});if(options.searchBarFields.length>0){wrapper.append("<div class='"+css.filterScopeOptions+"'><ul class='"+css.keywordSearchScopeHidden+"'></ul></div>").find("ul").bind("tap",this._searchScopeTapHandler).append("<li class='"+css.preset+" "+(!options.searchBarFieldName?css.presetSelected:"")+"' idx='all'>"+this._locale("keywordAllStateText")+"</li>");searchScope=wrapper=wrapper.find("ul");for(x=0;x<options.searchBarFields.length;++x){wrapper.append($("<li class='"+css.preset+"' idx='"+x+"'>"+options.searchBarFields[x].text+"</li>"));if(options.searchBarFields[x].fieldName===options.searchBarFieldName&&(options.searchBarFields[x].condition===options.searchBarCondition||options.searchBarCondition==="contains"&&!options.searchBarFields[x].condition)){wrapper.children().removeClass(css.presetSelected).last().addClass(css.presetSelected)}}wrapper=wrapper.parent().bind({scroll:function(){self._refreshPresets()}});if(wrapper.scrollview){wrapper.scrollview()}else if(wrapper.igScroll){wrapper.igScroll({cancelStart:true})}wrapper=wrapper.parent()}if(!ui.tray){ui.tray=$("<div />");ui.tray.append(wrapper)}else{wrapper.prependTo(ui.tray)}insertedSearch=true}if(options.presets&&options.presets.length>0){if(ui.needTray!==2){ui.needTray=2}if(ui.css.indexOf("one")>-1){ui.css=ui.css.replace("one","two")}else{ui.css=ui.css+"-one"}wrapper=$("<div />").addClass(css.filterPresets).append("<span>"+this._locale("filterPresetsLabel")+"</span>").append("<div><ul></ul></div>").find("ul").bind("click",this._presetTapHandler);wrapper.append("<li class='"+css.preset+"' idx='default'>"+this._locale("filterAllStateText")+"</li>");for(x=0;x<options.presets.length;++x){wrapper.append($("<li class='"+css.preset+"' idx='"+x+"'>"+options.presets[x].text+"</li>"))}index=parseInt(options.filterState,10)+1;if(isNaN(index)){index=0}wrapper.children().eq(index).addClass(css.presetSelected);wrapper=wrapper.parent().bind({scroll:function(){self._refreshPresets()}});if(wrapper.scrollview){wrapper.scrollview()}else if(wrapper.igScroll){wrapper.igScroll({cancelStart:true})}wrapper=wrapper.parent();if(!ui.tray){ui.tray=$("<div />");wrapper.appendTo(ui.tray)}else if(insertedSearch){wrapper.insertAfter(ui.tray.children().eq(0))}else{wrapper.prependTo(ui.tray)}}},_renderedTray:function(e,ui){var self=this,$win=$(window),noCancel,needRendering=false,options=this.options,css=this.css,presets=options.presets,inputSearchContainer;if(presets&&presets.length>0||options.searchBarEnabled&&options.searchBarFields&&options.searchBarFields.length>0){this._refreshPresets();$win.bind("resize",function(){self._refreshPresets()});$win.bind("orientationchange",function(){self._refreshPresets()})}if(options.searchBarEnabled&&ui.needTray===2){inputSearchContainer=this._filterInput.parent().parent().addClass(css.keywordInputContainerWithCancel);$("<div>").addClass(css.cancelButtonContainer).append($("<div>").text(this._locale("cancelButtonLabel")).buttonMarkup({inline:true}).bind("click",function(){var needToBind;if(self._filterInput.val()!==""){noCancel=self._trigger(self.events.keywordChanging,null,{value:"",owner:self});if(noCancel){self._filterInput.val("");self._filterInput.jqmData("lastVal","");self._adjustKeywordTrayText("");self._shouldFireKeywordChanged=true;needRendering=true}}if(options.filterState!=="default"){noCancel=self._trigger(self.events.presetChanging,null,{state:"default",owner:self});if(noCancel){self._adjustFilterState("default");self._shouldFirePresetChanged=true;needRendering=true}}if(needRendering){$.mobile.loading("show");self._refilter(false)}needToBind=self._trigger("_cancelButtonClicked");self.list.toggleSearchArea();if(needToBind&&needRendering){if(options.type==="remote"){self.list.dataSource.dataBind()}else{self.list._renderData()}}})).insertAfter(inputSearchContainer)}},_refreshPresets:function(parent){var activePage=$.mobile.activePage,listPage=this.list.element.closest(".ui-page"),css=this.css;if(activePage&&activePage[0]===listPage[0]){if(!parent){parent=this.list.container().find("."+css.filterPresets).parent()}$(parent).find("li").removeClass(css.presetHidden).each(function(i,val){var $div=$(val.parentNode.parentNode),$li=$(val),liLeft=$li.offset().left,liWidth=$li.width(),divWidth=$div.outerWidth(),divLeft=Math.min(0,$div.offset().left);if(liLeft+liWidth>divWidth+divLeft||liLeft<divLeft){$li.addClass(css.presetHidden)}})}},_renderingTrayFooterBar:function(e,ui){var options=this.options,css=this.css,wrapper=$("<div>");if(options.searchBarEnabled){wrapper.append(this._keywordSpan=$("<span data-trayRole='keyword' class='"+css.keywordFooterText+"'></span>").text(this._filterInput.val())).append("<span style='display:none;' class='"+css.separatorFooterText+"' data-trayRole='bullet'> &bull; </span>")}if(options.presets&&options.presets.length>0){wrapper.append("<span class='"+css.separatorFooterText+"' style='display:none;'> "+this._locale("showLabel")+" </span>").append(this._presetSpan=$("<span data-trayRole='keyword' class='"+css.presetFooterText+"'></span>"));if(options.filterState!=="default"){this._presetSpan.text(options.presets[options.filterState].text).prev().show();if(this._keywordSpan&&this._keywordSpan.text()){this._keywordSpan.next().show()}}}if(ui.trayFooter.children().length>0){wrapper.children().prependTo(ui.trayFooter)}else{ui.trayFooter.append(wrapper.children())}if(this.options.filterState!=="default"&&this._presetSpan.nextAll('[data-trayRole="keyword"]').text()){this._presetSpan.next().show()}},_dataRendering:function(){if(this.list._dataBinding&&this.options.type==="local"){this._refilter(false)}if(this._filterNew&&this._expr!==this._filterNew){this._expr=this._filterNew;this._refilter();return false}},_itemsRendering:function(){delete this._expr;delete this._filterNew;if(this._oldPagingIndex!==undefined){this.list.dataSource.settings.paging.pageIndex=this._oldPagingIndex;delete this._oldPagingIndex}},_itemsRendered:function(){if(this._shouldFirePresetChanged){this._trigger(this.events.presetChanged,null,{state:this.options.filterState,owner:this});this._shouldFirePresetChanged=false}if(this._shouldFireKeywordChanged){this._trigger(this.events.keywordChanged,null,{value:this._filterInput.val(),owner:this});this._shouldFireKeywordChanged=false}if(this._shouldFireScopeChanged){this._trigger(this.events.scopeChanged,null,{scopeField:this.options.searchBarFieldName,owner:this});this._shouldFireScopeChanged=false}$.mobile.loading("hide")},_sortChanging:function(e,ui){if(this._keepDividers&&this._filterDividers){this._refilter(false,ui.sortedFields);ui.handled=true}},_encodeExtraParams:function(dataSource,params){if(this._dividers){params.filteringParams.dividers=this._dividers}if(this._needsAll){params.filteringParams.allCondition=this.options.searchBarCondition;params.filteringParams.allValue=this._filterInput.val()}},_createHandlers:function(){this._renderingTrayHandler=$.proxy(this._renderingTray,this);this._renderedTrayHandler=$.proxy(this._renderedTray,this);this._renderingTrayFooterBarHandler=$.proxy(this._renderingTrayFooterBar,this);this._dataRenderingHandler=$.proxy(this._dataRendering,this);this._itemsRenderingHandler=$.proxy(this._itemsRendering,this);this._itemsRenderedHandler=$.proxy(this._itemsRendered,this);this._sortChangingHandler=$.proxy(this._sortChanging,this)},_registerEvents:function(){var listElement=this.list.element;listElement.bind("iglistrenderingtray",this._renderingTrayHandler);listElement.bind("iglistrenderedtray",this._renderedTrayHandler);listElement.bind("iglistrenderingtrayfooterbar",this._renderingTrayFooterBarHandler);listElement.bind("iglistdatarendering",this._dataRenderingHandler);listElement.bind("iglistitemsrendering",this._itemsRenderingHandler);listElement.bind("iglistitemsrendered",this._itemsRenderedHandler);listElement.bind("iglistviewsorting_sortchanging",this._sortChangingHandler)},_unregisterEvents:function(){var listElement=this.list.element;listElement.unbind("iglistrenderingtray",this._renderingTrayHandler);listElement.unbind("iglistrenderedtray",this._renderedTrayHandler);listElement.unbind("iglistrenderingtrayfooterbar",this._renderingTrayFooterBarHandler);listElement.unbind("iglistdatarendering",this._dataRenderingHandler);listElement.unbind("iglistitemsrendering",this._itemsRenderingHandler);listElement.unbind("iglistitemsrendered",this._itemsRenderedHandler);listElement.unbind("iglistviewsorting_sortchanging",this._sortChangingHandler);delete this._renderingTrayHandler;delete this._renderedTrayHandler;delete this._renderingTrayFooterBarHandler;delete this._dataRenderingHandler;delete this._itemsRenderedHandler;delete this._sortChangingHandler},_injectList:function(listInstance){var x,presets,filteredFields=[],options=this.options,listOptions=listInstance.options,settings=listInstance.dataSource.settings,filtering=settings.filtering;this.list=listInstance;if(options.type===null){options.type=listInstance._inferOpType()}if(options.type){filtering.type=options.type}else{filtering.type="remote"}if(options.filterExprUrlKey){filtering.filterExprUrlKey=options.filterExprUrlKey;this._encodeExtraParamsHandler=$.proxy(this._encodeExtraParams,this);settings.urlParamsEncoded=this._encodeExtraParamsHandler}filtering.caseSensitive=options.caseSensitive;presets=options.filteredFields;if(listOptions.bindings&&listOptions.bindings.isDividerKey){this._keepDividers=[];this._keepDividers[0]={fieldName:listOptions.bindings.isDividerKey,cond:"true",logic:"Or"};this._keepDividersAll=[];this._keepDividersAll[0]={fieldName:listOptions.bindings.isDividerKey,cond:"equals",expr:"true",logic:"Or"};this._filterDividers=[];this._filterDividers[0]={fieldName:listOptions.bindings.isDividerKey,cond:"false",logic:"And"}}if(presets&&presets.length>0){for(x=0;x<presets.length;++x){filteredFields[filteredFields.length]={fieldName:presets[x].fieldName,cond:presets[x].condition,expr:presets[x].searchValue}}}if(options.filterState!=="default"&&options.presets){presets=options.presets[options.filterState].filteredFields}if(presets&&presets.length>0){for(x=0;x<presets.length;++x){filteredFields[filteredFields.length]={fieldName:presets[x].fieldName,cond:presets[x].condition,expr:presets[x].searchValue}}}if(settings.sorting.enabled&&settings.sorting.defaultFields.length>0&&this._filterDividers){filteredFields=this._filterDividers.concat(filteredFields)}else if(filteredFields.length>0&&this._keepDividers){filteredFields=this._keepDividers.concat(filteredFields)}if(filteredFields.length>0){filtering.expressions=filteredFields}this._createHandlers();this._registerEvents()},destroy:function(){if(this._presetSpan){this._presetSpan.prev().remove();this._presetSpan.prev().remove();this._presetSpan.remove();delete this._presetSpan}if(this._keywordSpan){this._keywordSpan.remove();delete this._keywordSpan}if(this._filterInput){if(this._filterInput.data("textinput")){this._filterInput.data("textinput").destroy()}this._filterInput.parents(".ui-iglist-filter-key-word-area").remove();delete this._filterInput}this.list.container().find("."+this.css.filterPresets).remove();this._unregisterEvents();$.Widget.prototype.destroy.call(this);return this}});function _addDash(c){return"-"+c.toLowerCase()}function _fixSearchValue(searchValue){var temp=searchValue;try{if(searchValue!=="jQuery"&&searchValue!=="$"){searchValue=eval(searchValue)}}catch(ex){searchValue=temp}if(searchValue===undefined&&temp){searchValue=temp}return searchValue}$(document).bind("_igListViewHtmlOptions",function(evnt,args){var elem=args.element,filtering,option,value,x,i;if(elem.jqmData("filtering")===true){if(!args.options.features){args.options.features=[]}filtering=args.options.features[args.options.features.length]={};filtering.name="Filtering";for(option in $.mobile.igListViewFiltering.prototype.options){if($.mobile.igListViewFiltering.prototype.options.hasOwnProperty(option)){value=elem.jqmData("filtering-"+option.replace(/[A-Z]/g,_addDash));if(value!==undefined){if(option==="presets"){for(x=0;x<value.length;++x){if(value[x]&&value[x].filteredFields){for(i=0;i<value[x].filteredFields.length;++i){value[x].filteredFields[i].searchValue=_fixSearchValue(value[x].filteredFields[i].searchValue)}}}}else if(option==="filteredFields"){for(x=0;x<value.length;++x){if(value[x]){value[x].searchValue=_fixSearchValue(value[x].searchValue)}}}filtering[option]=value}}}}});if(typeof define==="function"&&define.amd&&define.amd.jQuery){define("ig.mobileui.list.filtering",["ig.mobileui.list"],function(){return $.mobile.igListViewFiltering})}$.extend($.mobile.igListViewFiltering,{version:"15.2.20152.1027"})})(jQuery);