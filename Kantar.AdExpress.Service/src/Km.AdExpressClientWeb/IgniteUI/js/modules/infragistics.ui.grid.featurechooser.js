﻿/*!@license
 * Infragistics.Web.ClientUI Grid Feature Chooser 15.2.20152.1027
 *
 * Copyright (c) 2011-2015 Infragistics Inc.
 *
 * http://www.infragistics.com/
 *
 * Depends on:
 *	jquery-1.4.4.js
 *	jquery.ui.core.js
 *	jquery.ui.widget.js
 *	jquery.ui.mouse.js
 *	jquery.ui.draggable.js
 *	jquery.ui.resizable.js
 *	modernizr.js (Optional)
 *	infragistics.ui.popover.js
 *	infragistics.ui.grid.framework.js
 *	infragistics.ui.grid.shared.js
 *	infragistics.ui.shared.js
 *	infragistics.dataSource.js
 *	infragistics.util.js
 */
if(typeof jQuery!=="function"){throw new Error("jQuery is undefined")}(function($){$.widget("ui.igGridFeatureChooserPopover",$.ui.igPopover,{options:{gridId:"",targetButton:null,closeOnBlur:true,containment:null},_create:function(){$.extend($.ui.igGridFeatureChooserPopover.prototype.options,$.ui.igPopover.prototype.options);$.extend($.ui.igGridFeatureChooserPopover.prototype.css,$.ui.igPopover.prototype.css);$.ui.igPopover.prototype._create.apply(this)},isShown:function(){return this.container().is(":visible")},_setFCElementFocus:function(focus,$target){if($target===undefined||$target===null){$target=this.options.targetButton}this.options.targetButton.data("onFocus",focus);if(focus){$target.focus()}},registerElements:function(elements){var events,$targetButton=this.options.targetButton,self=this;events={focus:function(){setTimeout(function(){$targetButton.data("onFocus",true)},1)},blur:function(){$targetButton.data("onFocus",false);setTimeout(function(){if(!$targetButton.data("onFocus")&&self.isShown()){self._closePopover()}},10)}};elements.bind(events)},_createWidget:function(){var self=this,$targetButton;this._attachedToBody=true;this.options.content="";$.Widget.prototype._createWidget.apply(this,arguments);$.ui.igPopover.prototype._createWidget.apply(this,arguments);this._detachEventsFromTarget();$targetButton=this.options.targetButton;if(this.options.closeOnBlur){$targetButton.attr("tabindex","0");this._eventsFC={iggridfeaturechooserpopovershown:function(){self._setFCElementFocus(true)},iggridfeaturechooserpopoverhidden:function(){self._setFCElementFocus(false);self.popover.width("");self.popover.css({left:""})},mousedown:function(){setTimeout(function(){self._setFCElementFocus(true)},1)},touchstart:function(){setTimeout(function(){self._setFCElementFocus(true)},1)}};this.element.bind(this._eventsFC);this.registerElements($targetButton)}},_removeOriginalTitle:function(){},_positionPopover:function(){var $popover,availableWidth,mw=this.options.maxWidth,$containment=this.options.containment,$window=$(window);if(mw&&$.type(mw)==="string"){mw=parseInt(mw,10)}$popover=this.popover;if($popover){if($containment.offset().left>=$window.scrollLeft()){availableWidth=$window.width()+$window.scrollLeft()-$containment.offset().left}else{availableWidth=$window.width()}if(mw&&$.type(mw)==="number"&&mw<availableWidth){availableWidth=mw}this.popover.css("max-width",availableWidth)}$.ui.igPopover.prototype._positionPopover.apply(this,arguments)},destroy:function(){var $targetButton;if(this._eventsFCTargetButton){$targetButton=this.options.targetButton;$targetButton.unbind(this._eventsFCTargetButton)}if(this._eventsFC){this.element.unbind(this._eventsFC)}$.ui.igPopover.prototype.destroy.apply(this,arguments)}});$.extend($.ui.igGridFeatureChooserPopover,{version:"15.2.20152.1027"});$.ig=$.ig||{};$.ig.GridFeatureChooserSections=$.ig.GridFeatureChooserSections||{click:"click",toggle:"toggle",modalDialog:"modalDialog"};$.widget("ui.igGridFeatureChooser",{css:{featureChooserDropDown:"",headerButtonIcon:"ui-iggrid-featurechooserbutton ui-icon ui-icon-gear",headerButtonIconMouseOver:"ui-iggrid-header-icon-mouseover",headerButtonIconSelected:"ui-iggrid-header-icon-selected",listClass:"ui-iggrid-featurechooser-list ui-menu ui-widget ui-widget-content ui-corner-all",listItemClass:"ui-iggrid-featurechooserddlistitemicons ui-state-default",listItemContainer:"ui-iggrid-featurechooser-li-container",listItemIconContainer:"ui-iggrid-featurechooser-li-iconcontainer",itemSecondaryIconContainer:"ui-iggrid-featurechooser-secondaryicon-container",separator:"ui-iggrid-featurechooser-separator",listItemText:"ui-iggrid-featurechooserddlistitemtext",itemSelected:"item-selected",dropDownListItemHover:"ui-iggrid-featurechooser-listitem-hover ui-state-active ui-state-hover",dropDownButtonClasses:"ui-igbutton",dropDownButtonHoverClasses:"",dropDownButtonActiveClasses:"",dropDownButtonFocusClasses:"",dropDownButtonLabelClass:"",containerDelimiter:"ui-iggrid-featurechooser-container-delimiter",containerSection:"ui-iggrid-featurechooser-container-section",itemNoIcon:"ui-iggrid-featurechooserbutton ui-icon ui-icon-close",submenu:"ui-iggrid-featurechooser-submenu ui-widget-content ui-corner-all",submenuIcon:"ui-iggrid-featurechooser-submenuicon ui-icon ui-icon-triangle-1-s"},options:{dropDownWidth:null,animationDuration:400},events:{featureChooserRendering:"featureChooserRendering",featureChooserRendered:"featureChooserRendered",featureChooserDropDownOpening:"featureChooserDropDownOpening",featureChooserDropDownOpened:"featureChooserDropDownOpened",menuToggling:"menuToggling",featureToggling:"featureToggling",featureToggled:"featureToggled"},_createWidget:function(){$.Widget.prototype._createWidget.apply(this,arguments)},_create:function(){this.analyzedData={};this.data={};this._features=[];this._isFeaturesAnalyzed=false;this.isInitialized=false;this.grid=this.element.data("igGrid");this.gridId=this.element[0].id;this._analyzeGridFeaturesOptions();this._countRenderedFeatures=0;if(this._headerRenderedHandler){this.grid.element.unbind("iggridheaderrendered",this._headerRenderedHandler)}this._headerRenderedHandler=$.proxy(this._headerRendered,this);this.grid.element.bind("iggridheaderrendered",this._headerRenderedHandler);if(this._gridDestroyedHandler){this.grid.element.unbind("igcontroldestroyed",this._gridDestroyedHandler)}this._gridDestroyedHandler=$.proxy(this.destroy,this);this.grid.element.bind("igcontroldestroyed",this._gridDestroyedHandler);if(this._virtualHorizontalScrollHandler){this.grid.element.unbind("iggridvirtualhorizontalscroll",this._virtualHorizontalScrollHandler)}this._virtualHorizontalScrollHandler=$.proxy(this._virtualHorizontalScroll,this);this.grid.element.bind("iggridvirtualhorizontalscroll",this._virtualHorizontalScrollHandler)},_analyzeGridFeaturesOptions:function(){var i,features=this.grid.options.features,featuresLength=features.length;for(i=0;i<featuresLength;i++){this._features.push(features[i].name)}},shouldShowFeatureIcon:function(key){if(this.analyzedData[key]!==undefined&&this.analyzedData[key]!==null){return this.analyzedData[key].length<=1}return true},_setOption:function(){$.Widget.prototype._setOption.apply(this,arguments)},_id:function(name,suffix){var res=this.gridId+"_"+name;if(suffix!==undefined&&suffix!==null&&$.type(suffix)==="string"){res+="_"+suffix}return res},_isMetroIE:function(){return!$.ig.util.isIEOld&&$.ig.util.isIE&&(window.hasOwnProperty("ontouchstart")||navigator.MaxTouchPoints>0||navigator.msMaxTouchPoints>0)},_isTouchDevice:function(){var fcIconDisplay;if(this._isTouch===undefined){fcIconDisplay=this.grid.options.featureChooserIconDisplay;if(fcIconDisplay==="none"){this._isTouch=true}else if(fcIconDisplay==="always"){this._isTouch=false}else{this._isTouch=typeof Modernizr==="object"&&Modernizr.touch===true||this._isMetroIE()}}return this._isTouch},_callFeatureMethod:function(feature,isSelected,columnKey,event){var method,returnType=true,methodType=$.type(feature.method),featureData=this.grid.element.data("igGrid"+feature.name);if(methodType==="string"){if(featureData===undefined||featureData===null){return false}method=featureData[feature.method]}else if(methodType==="function"){method=feature.method}if(method===undefined||method===null){return false}if($.type(feature.method)==="string"){returnType=featureData[feature.method](event,columnKey,isSelected,feature.methodData)}else{returnType=feature.method(event,columnKey,isSelected,feature.methodData)}return returnType},_getFeatureByKeyName:function(columnKey,featureName){var i,feature=null,columnData=this.data[columnKey],columnDataLength;if(!columnData){return feature}columnDataLength=columnData.length;for(i=0;i<columnDataLength;i++){if(columnData[i].name===featureName){feature=columnData[i];break}}return feature},_clickFeature:function(event){var i,j,$target=$(event.target).data("data")!==undefined?$(event.target):$(event.currentTarget),d=$target.data("data"),self=this,isSelected=null,type=d.type,columnKey=d.columnKey,featureName=d.featureName,columnData=this.data[columnKey],columnDataLength=columnData.length,feature=null;feature=this._getFeatureByKeyName(columnKey,featureName);if(feature===null||feature===undefined){return}if(type==="toggle"){isSelected=!feature.isSelected}if(this._trigger(this.events.featureToggling,null,{target:$target,columnKey:columnKey,isToSelect:isSelected,featureName:"igGrid"+feature.name})===false){return}if(this._callFeatureMethod(feature,isSelected,columnKey,event)===false){return}if(type==="toggle"){feature.isSelected=isSelected;self._setSelectedItem(columnKey,isSelected,$("#"+self._id("featurechooser_dd_li_"+columnKey,featureName)));if(feature.updateOnClickAll===true){$.each(self.data,function(index,value){if(index!==columnKey){for(j=0;j<value.length;j++){if(value[j].name===featureName){break}}if(j===value.length){return true}columnData=self.data[index];if(columnData===null||columnData===undefined){return true}columnDataLength=columnData.length;for(i=0;i<columnDataLength;i++){if(columnData[i].name===featureName){columnData[i].isSelected=isSelected;break}}self._setSelectedItem(index,isSelected,$("#"+self._id("featurechooser_dd_li_"+index,value[j].name)))}})}}},_setListItemText:function(columnKey,featureName,text){$("#"+this._id("featurechooser_dd_li_"+columnKey,featureName)+" span.ui-iggrid-featurechooserddlistitemtext").text(text)},_analyzeFeatures:function(){if(this._isFeaturesAnalyzed===true){return}var i,k,cs=this.grid.options.columns,csLength=cs.length,featureName,featuresLength=this._features.length,features=this._features,newFeatures=[],columnsFeature,columnsFeatureLength,featureInstance,columnKey;for(i=0;i<featuresLength;i++){featureName=features[i];featureInstance=this.grid.element.data("igGrid"+featureName);if(featureInstance===undefined||featureInstance===null){continue}if(featureInstance.renderInFeatureChooser!==true){continue}if(featureInstance._columnMap===undefined||featureInstance._columnMap===null){continue}columnsFeature=featureInstance._columnMap();if(columnsFeature===false||columnsFeature===null||columnsFeature===undefined){continue}columnsFeatureLength=columnsFeature.length;if(columnsFeatureLength===0){for(k=0;k<csLength;k++){columnKey=cs[k].key;if(this.analyzedData[columnKey]===undefined||this.analyzedData[columnKey]===null){this.analyzedData[columnKey]=[]}if(this._isFeatureExistByColumnKey(featureName,columnKey)===true){continue}this.analyzedData[columnKey].push({name:featureName,method:null,text:null,iconClass:null,isSelected:false,columnCell:null,isActive:false})}}else{for(k=0;k<columnsFeatureLength;k++){columnKey=columnsFeature[k].columnKey;if(this.analyzedData[columnKey]===undefined||this.analyzedData[columnKey]===null){this.analyzedData[columnKey]=[]}if(columnsFeature[k].enabled!==true||this._isFeatureExistByColumnKey(featureName,columnKey)===true){continue}this.analyzedData[columnKey].push({name:featureName,method:null,text:null,iconClass:null,isSelected:false,columnCell:null,isActive:false})}}newFeatures.push({name:featureName,instance:featureInstance})}this._features=newFeatures;this._isFeaturesAnalyzed=true},_isFeatureExistByColumnKey:function(featureName,columnKey){var aData=this.analyzedData[columnKey],ret=false;if(aData===null||aData===undefined){return true}$.each(aData,function(index,value){if(value.name===featureName){ret=true;return false}});return ret},_shouldRenderInFeatureChooser:function(columnKey){this._analyzeFeatures();if(this.analyzedData[columnKey]!==undefined&&this.analyzedData[columnKey]!==null){return this.analyzedData[columnKey].length>1}return false},_headerRendered:function(sender,args){if(args.owner.element.attr("id")!==this.grid.element.attr("id")){return}if(sender.target.id!==this.grid.element[0].id){return}this._renderFCForAllColumns()},_renderFCForAllColumns:function(){var self=this;$.each(this.data,function(columnKey,elem){self._initFC(columnKey,elem)})},_touchStart:function(e,columnKey){if($(e.target).attr("data-skip-event")){return e}this.toggleDropDown(columnKey);e.preventDefault();e.stopPropagation()},_initFC:function(columnKey,elem){var self=this,thead=this.grid.container().find("thead"),$columnCell,$theadCell,buttonId,cancelFunc,$button,$headerContainer;$columnCell=$("#"+self.grid.element[0].id+"_"+columnKey);buttonId=self._id("featureChooser_headerButton",columnKey);if(!$columnCell.length){return}$columnCell.find("[data-fc-button]").remove();$("#"+buttonId).remove();cancelFunc=function(e){e.preventDefault();e.stopPropagation()};if(elem.isCancelledRendering!==false&&self._trigger(self.events.featureChooserRendering,null,{owner:self.grid,columnKey:columnKey,columnCell:$columnCell})===false){elem.isCancelledRendering=true;return}elem.isCancelledRendering=false;if(this._isTouchDevice()){$columnCell.bind({click:function(e){self._touchStart(e,columnKey)}})}else{$theadCell=thead.find("#"+self.grid.element[0].id+"_"+columnKey);this.grid._enableHeaderCellFeature($theadCell);$headerContainer=$theadCell.find(".ui-iggrid-indicatorcontainer");if($headerContainer.length===0){$headerContainer=$("<div></div>").appendTo($theadCell).addClass("ui-iggrid-indicatorcontainer")}$button=$("<span></span>").prependTo($headerContainer).addClass(self.css.headerButtonIcon);$button.wrap('<a id="'+buttonId+'" role="button" tabindex="'+self.grid.options.tabIndex+'" data-fc-button="'+columnKey+'" href="#" title="'+$.ig.Grid.locale.featureChooserTooltip+'"></a>');$button.parent().attr("th-remove-focus","").bind({keydown:function(event){var $item,$nextPrevItem,$dialog=$("#"+self._id("featureChooser_dd",columnKey));if(event.keyCode===$.ui.keyCode.ENTER||event.keyCode===$.ui.keyCode.SPACE){$item=$dialog.find("ul li.ui-iggrid-featurechooser-listitem-hover");if($dialog.is(":visible")&&$item.length>0){if($item.find('*[role="button"]').length){event.target=$item.find('*[role="button"]:eq(0)')}else{event.target=$item[0]}self._clickFeature(event)}self.toggleDropDown(columnKey);cancelFunc(event)}else if(event.keyCode===$.ui.keyCode.DOWN||event.keyCode===$.ui.keyCode.UP){if($dialog.is(":visible")){$item=$dialog.find("ul li.ui-iggrid-featurechooser-listitem-hover");if($item.length>1){$.each($item,function(index,value){self._removeCssSelectionListItem($(value))})}$nextPrevItem=event.keyCode===$.ui.keyCode.DOWN?$item.next():$item.prev();if($nextPrevItem.attr("data-fc-separator")){$nextPrevItem=event.keyCode===$.ui.keyCode.DOWN?$nextPrevItem.next():$nextPrevItem.prev()}if($item.length===0){self._addCssSelectionListItem($dialog.find("ul li:eq(0)"))}else if($nextPrevItem.length>0&&$nextPrevItem.is("li")){self._removeCssSelectionListItem($item);self._addCssSelectionListItem($nextPrevItem)}cancelFunc(event)}}},mousedown:function(event){self.toggleDropDown(columnKey);cancelFunc(event)},mouseover:function(){if($button.hasClass(self.css.headerButtonIconSelected)===false){$button.addClass(self.css.headerButtonIconMouseOver)}},mouseout:function(){$button.removeClass(self.css.headerButtonIconMouseOver)},mouseup:cancelFunc,click:cancelFunc});$("#"+this._id("featureChooser_dd",columnKey)).remove()}self._trigger(self.events.featureChooserRendered,null,{owner:self.grid,columnKey:columnKey,columnCell:$columnCell})},_renderInFeatureChooser:function(columnKey,data){var i,columnData=this.analyzedData[columnKey],columnDataLength;if(columnData===undefined||columnData===null){return}columnDataLength=columnData.length;if(columnData.isCancelledRendering===true){return}for(i=0;i<columnDataLength;i++){if(columnData[i].name.toLowerCase()===data.name.toLowerCase()){this.analyzedData[columnKey][i]=data;break}}if(i===columnDataLength){this.analyzedData[columnKey].push(data)}if(this.data[columnKey]===null||this.data[columnKey]===undefined){this.data[columnKey]=[]}if($.type(this.data[columnKey].order)!=="number"){this.data[columnKey].order=0}for(i=0;i<this.data[columnKey].length;i++){if(this.data[columnKey][i].name===data.name&&data.name!==undefined){return}}this.data[columnKey].push(data)},_renderMenu:function(columnKey){var i,data,self=this,listItems,popoverInstance,$targetButton,$headerCell=$("#"+this.grid.id()+"_"+columnKey),dropDownId=this._id("featureChooser_dd",columnKey),$dropDown=$("#"+dropDownId),rootContainer=this.grid._rootContainer();if($dropDown.length>0){return}$dropDown=$('<div tabindex="0"></div>').attr("id",dropDownId).appendTo(rootContainer);$targetButton=$("#"+this._id("featureChooser_headerButton",columnKey)).find("span");if($targetButton.length===0){$targetButton=$headerCell}$dropDown.igGridFeatureChooserPopover({position:"auto",targetButton:$targetButton,maxWidth:this.grid.container().width(),direction:"bottom",maxHeight:null,containment:rootContainer,appendTo:rootContainer});popoverInstance=$dropDown.data("igGridFeatureChooserPopover");$dropDown.bind("iggridfeaturechooserpopovershown",function(){self._visiblePopover=dropDownId});$dropDown.bind("iggridfeaturechooserpopoverhiding",function(){if(self._activeSubmenuId){$("#"+self._activeSubmenuId).hide();self._activeSubmenuId=null}});data=this.data[columnKey];if(data){for(i=0;i<data.length;i++){this._renderDropDownItem(columnKey,data[i])}listItems=popoverInstance.container().find("li[data-fc-order]");$dropDown.igGridFeatureChooserPopover("registerElements",listItems);$targetButton.bind({keydown:function(e){if(e.keyCode===$.ui.keyCode.ESCAPE){popoverInstance._closePopover();e.stopPropagation()}else if(e.keyCode===$.ui.keyCode.TAB&&!e.shiftKey&&popoverInstance.isShown()){popoverInstance.popover.find("li[data-fc-order]:first").focus();e.stopPropagation();e.preventDefault()}}});listItems.bind({keydown:function(e){var $elem,keyCode=e.keyCode;switch(keyCode){case $.ui.keyCode.ESCAPE:$targetButton.data("onFocus",false).focus();popoverInstance._closePopover();break;case $.ui.keyCode.TAB:$elem=$(this);if(!e.shiftKey){if($elem.is(":last-child")){$elem.closest("ul").find("li:first-child").focus();e.preventDefault()}}else{if($elem.is(":first-child")){$elem.closest("ul").find("li:last-child").focus();e.preventDefault()}}break;case $.ui.keyCode.RIGHT:$elem=$(this);$elem=$elem.nextAll("li[data-fc-order]").eq(0);if($elem.length===0){$elem=$(this).closest("ul").find("li:first-child")}$elem.focus();e.preventDefault();break;case $.ui.keyCode.LEFT:$elem=$(this);$elem=$elem.prevAll("li[data-fc-order]").eq(0);if($elem.length===0){$elem=$(this).closest("ul").find("li:last-child")}$elem.focus();e.preventDefault();break}}})}$dropDown.bind({keydown:function(event){if(event.keyCode===$.ui.keyCode.ESCAPE&&popoverInstance.container().is(":visible")){self.toggleDropDown(columnKey);popoverInstance.container().blur()}}})},_removeFeature:function(featureName,submenu){var self=this,cols=this.grid.options.columns;$.each(cols,function(index,col){self._removeDropDownItem(col.key,featureName);if(submenu){$("#"+self._id("featurechooser_submenu_"+col.key,featureName)).remove()}})},_removeDropDownItem:function(columnKey,featureName){var index=-1,data,$dropDown=$("#"+this._id("featureChooser_dd",columnKey)),$dropDownList=$("#"+this._id("featurechooser_dd_list",columnKey)),$item=$("#"+this._id("featurechooser_dd_li_"+columnKey,featureName));if($dropDown.length===0){data=this.data[columnKey];if(data){$.each(data,function(i,dataItem){if(dataItem.name===featureName){index=i;return false}});if(index>-1){data.splice(index,1);if(data.length===0){this._removeFC(columnKey)}}}}else if($item.length>0){$item.remove();if($dropDownList.find("li:not([data-fc-separator])").length===0){this._removeFC(columnKey)}else{this._removeSeparatorItem(columnKey)}}},_removeFC:function(columnKey){var $dropDown=$("#"+this._id("featureChooser_dd",columnKey));$dropDown.igGridFeatureChooserPopover("destroy");$dropDown.remove();$("#"+this._id("featureChooser_headerButton",columnKey)).remove()},_removeSeparatorItem:function(columnKey){var listLiSeparator=$("#"+this._id("featurechooser_dd_list",columnKey)).find("[data-fc-separator]");listLiSeparator.each(function(){var $li=$(this),$prevLi=$li.prev(),$nextLi=$li.next();if($prevLi.length===0||$prevLi.attr("data-fc-separator")!==undefined||$nextLi.length===0||$nextLi.attr("data-fc-separator")!==undefined){$li.remove();return false}})},_renderDropDownItem:function(columnKey,data){var dropDownId=this._id("featureChooser_dd",columnKey),$dropDownContainer=$("#"+dropDownId).igGridFeatureChooserPopover("container"),$li,listId,$list,funcClickOnFeature,$iconContainer,$span,cssClassIcon="",self=this,isSelected=data.isSelected,featureName=data.name,innerData,liId=this._id("featurechooser_dd_li_"+columnKey,featureName),groupName=data.groupName,groupOrder=data.groupOrder,listItemsGroup,insertElementObj,labelText;if($("#"+liId).length>0){return}if(groupName===undefined||groupName===null){groupName="click"}listId=this._id("featurechooser_dd_list",columnKey);$list=$("#"+listId);if($list.length===0){$dropDownContainer.html("");$list=$("<ul></ul>").attr("id",listId).addClass(this.css.listClass).appendTo($dropDownContainer)}if(isSelected===undefined||isSelected===null){isSelected=false}$li=$('<li tabindex="0"></li>').addClass(this.css.listItemClass).attr("id",liId).attr("data-fc-order",data.order);if(groupName!==undefined&&groupName!==null){$li.attr("data-fc-groupname",groupName)}if(groupOrder!==undefined&&groupOrder!==null){$li.attr("data-fc-grouporder",groupOrder)}innerData={columnKey:columnKey,featureName:featureName,updateOnClickAll:data.updateOnClickAll,iconClass:data.iconClass,iconClassOff:data.iconClassOff,groupName:groupName,groupOrder:groupOrder,type:data.type,textHide:data.textHide,state:data.state,text:data.text};$li.data("data",innerData);funcClickOnFeature=function(event){var submenuId=self._id("featurechooser_submenu_"+columnKey,featureName),e=event,$submenu,keyCode=event.keyCode;if(data.type!=="dropdown"){if(!keyCode||keyCode===$.ui.keyCode.ENTER||keyCode===$.ui.keyCode.SPACE){if(event.target===undefined){e.target=event.srcElement}self._clickFeature(event);self.hideDropDown(columnKey)}}else{if(self._visiblePopover){if(!keyCode){self._toggleSubmenu(columnKey,featureName,data);setTimeout(function(){$("#"+self._visiblePopover).data("igGridFeatureChooserPopover")._setFCElementFocus(true)},5)}else{if(keyCode===$.ui.keyCode.DOWN){$submenu=$("#"+submenuId);if($submenu.is(":visible")){$submenu.find("[data-fc-item]:first").focus()}else{self._toggleSubmenu(columnKey,featureName,data)}}else if(keyCode===$.ui.keyCode.ENTER||keyCode===$.ui.keyCode.SPACE){self._toggleSubmenu(columnKey,featureName,data)}else if(keyCode===$.ui.keyCode.UP){$submenu=$("#"+submenuId);if($submenu.is(":visible")){self._toggleSubmenu(columnKey,featureName,data)}}}}if(event.preventDefault){event.preventDefault()}}};$li.bind({keydown:funcClickOnFeature,mousedown:funcClickOnFeature});if(data.iconClass!==null&&data.iconClass!==undefined&&data.iconClass!==""){if(data.iconClassOff&&data.isSelected===false){cssClassIcon=data.iconClassOff}else{cssClassIcon=data.iconClass}}$iconContainer=$("<div></div>").addClass(this.css.listItemContainer).appendTo($li);$span=$("<span></span>").addClass(this.css.listItemIconContainer).appendTo($iconContainer);if(cssClassIcon!==""){$span.addClass(cssClassIcon)}else{$span.addClass(this.css.itemNoIcon)}labelText=data.text;$li.attr("title",labelText);$("<span></span>").text(data.text).addClass(this.css.listItemText).appendTo($iconContainer);listItemsGroup=$("#"+listId+" li[data-fc-groupName = "+groupName+"]");if(listItemsGroup.length===0){listItemsGroup=$("#"+listId+" li[data-fc-groupName]");insertElementObj=this._getInsertElement(listItemsGroup,groupOrder,"data-fc-grouporder");if(insertElementObj.item!==null){$list=listItemsGroup}this._insertElement(insertElementObj,$list,$li,groupName)}else{insertElementObj=this._getInsertElement(listItemsGroup,data.order,"data-fc-order");this._insertElement(insertElementObj,listItemsGroup,$li,groupName)}if(data.type==="toggle"){this._setSelectedItem(columnKey,data.isSelected,$li)}if(data.type==="dropdown"){$('<span class="'+this.css.submenuIcon+'" data-submenu-arrow="true"></span>').appendTo($iconContainer);self._renderSubmenu(columnKey,featureName,data)}},_renderSubmenu:function(columnKey,featureName,data){var submenuId=this._id("featurechooser_submenu_"+columnKey,featureName),$submenu;if($("#"+submenuId).length>0){$("#"+submenuId).remove()}if(this._submenus===null||this._submenus===undefined){this._submenus=[]}this._submenus.push(submenuId);$submenu=$('<div id="'+submenuId+'" style="position:absolute" class="'+this.css.submenu+'"></div>').appendTo(this.grid._rootContainer());$submenu.data("buttonId",this._id("featurechooser_dd_li_"+columnKey,featureName));$submenu.hide();if(data.methodRenderSubmenu){data.methodRenderSubmenu(columnKey,$submenu);$("#"+this._id("featureChooser_dd",columnKey)).igGridFeatureChooserPopover("registerElements",$submenu.find("[data-fc-item]"))}},_toggleSubmenu:function(columnKey,featureName,data){var rOffset,$li=$("#"+this._id("featurechooser_dd_li_"+columnKey,featureName)),$innerDiv=$li.find("div:nth-child(1)"),targetWidth=$innerDiv.outerWidth(),targetLeft=$innerDiv.offset().left,submenuId=this._id("featurechooser_submenu_"+columnKey,featureName),$submenu=$("#"+submenuId),left=targetLeft-Math.abs($submenu.outerWidth()-targetWidth)/2,top=$innerDiv.offset().top+$innerDiv.outerHeight();rOffset=$.ig.util.getRelativeOffset($submenu);$submenu.css({left:left-rOffset.left,top:top-rOffset.top});if(data.methodToggleSubmenu){data.methodToggleSubmenu(columnKey,!$submenu.is(":visible"),$submenu)}if(this._activeSubmenuId&&this._activeSubmenuId!==submenuId){$("#"+this._activeSubmenuId).hide()}this._activeSubmenuId=submenuId;$submenu.toggle("slide",{duration:150,direction:"up"})},_getSeparatorItem:function(){return $('<li data-fc-separator="1" class="'+this.css.separator+'"></li>')},_insertElement:function(insertElementObj,$list,$li,groupName){var $insertElement=insertElementObj.item,insertElementPosition=insertElementObj.position,$prevElement;if($insertElement===null){$li.appendTo($list);$prevElement=$li.prev();if($prevElement!==undefined&&$prevElement.length>0&&$prevElement.attr("data-fc-separator")!=="1"&&$prevElement.attr("data-fc-groupname")!==undefined&&$prevElement.attr("data-fc-groupname")!==groupName){this._getSeparatorItem().insertBefore($li)}}else{if(insertElementPosition==="after"){$li.insertAfter($insertElement);if($insertElement!==undefined&&$insertElement.length>0&&$insertElement.attr("data-fc-separator")!=="1"&&$insertElement.attr("data-fc-groupname")!==undefined&&$insertElement.attr("data-fc-groupname")!==groupName){this._getSeparatorItem().insertBefore($li)}}else{$li.insertBefore($insertElement);$prevElement=$li.prev();if($prevElement!==undefined&&$prevElement.length>0&&$prevElement.attr("data-fc-separator")!=="1"&&$prevElement.attr("data-fc-groupname")!==undefined&&$prevElement.attr("data-fc-groupname")!==groupName){this._getSeparatorItem().insertBefore($li)}else if($insertElement!==undefined&&$insertElement.length>0&&$insertElement.attr("data-fc-separator")!=="1"&&$insertElement.attr("data-fc-groupname")!==undefined&&$insertElement.attr("data-fc-groupname")!==groupName){this._getSeparatorItem().insertAfter($li)}}}},_getInsertElement:function(listItems,targetOrder,attr){var $item=null,itemOrder,at,i;for(i=0;i<listItems.length;i++){$item=$(listItems[i]);at=$item.attr(attr);if(at===undefined){continue}itemOrder=parseInt(at,10);if(itemOrder>targetOrder){return{item:$item,position:"before"}}}return{item:$item,position:"after"}},_setSelectedState:function(featureName,columnKey,isSelected,executeCallback){var data=this.data[columnKey],$listItem,self=this;if(data===undefined||data===null){return false}$.each(data,function(index,value){if(value.name.toLowerCase()===featureName.toLowerCase()){$listItem=$("#"+self._id("featurechooser_dd_li_"+columnKey,value.name));self._setSelectedItem(columnKey,isSelected,$listItem);self.data[columnKey][index].isSelected=!isSelected;if(executeCallback===true){self._callFeatureMethod(value,isSelected,columnKey,null)}value.isSelected=isSelected;return false}})},_setSelectedItem:function(columnKey,isSelected,$listItem){if($listItem===null||$listItem===undefined||$listItem.length===0){return}var innerData=$listItem.data("data"),textShow,textHide,labelText,$label=null;if(innerData!==undefined&&innerData!==null){textShow=innerData.text;textHide=innerData.textHide;$label=$listItem.find("span.ui-iggrid-featurechooserddlistitemtext")}$listItem.attr("data-fc-selected",isSelected);if(isSelected===true){$listItem.addClass(this.css.itemSelected);labelText=textShow;if($label!==null&&textShow!==null&&textShow!==undefined){$label.text(labelText)}if(innerData.iconClassOff){$listItem.find("span.ui-iggrid-featurechooser-li-iconcontainer").removeClass(innerData.iconClassOff).addClass(innerData.iconClass)}}else{$listItem.removeClass(this.css.itemSelected);labelText=textHide;if($label!==null&&textHide!==null&&textHide!==undefined){$label.text(labelText)}if(innerData.iconClassOff){$listItem.find("span.ui-iggrid-featurechooser-li-iconcontainer").removeClass(innerData.iconClass).addClass(innerData.iconClassOff)}}$listItem.attr("title",labelText)},_toggleSelectedItems:function(featureName,selected){var i,$li,columnKey,cols=this.grid.options.columns,colsLength=cols.length,isSelected,feature;if(selected!==undefined){isSelected=selected}for(i=0;i<colsLength;i++){columnKey=cols[i].key;if($("#"+this._id("featureChooser_dd",columnKey)).length===0){feature=this._getFeatureByKeyName(columnKey,featureName);if(feature===null||feature===undefined){continue}if(isSelected===undefined){isSelected=feature.isSelected}}else{$li=$("#"+this._id("featurechooser_dd_li_"+columnKey,featureName));if($li.data("data")===null||$li.data("data")===undefined){continue}if(selected===undefined){isSelected=$li.attr("data-fc-selected")==="true"}}this._setSelectedState(featureName,columnKey,!isSelected)}},showDropDown:function(columnKey){var $targetButton,dropDownId=this._id("featureChooser_dd",columnKey),$dropDown=this.getDropDownByColumnKey(columnKey),fcp;this.grid._focusedElement=$("#"+this._id("featureChooser_headerButton",columnKey)).find("span");$targetButton=$("#"+this._id("featureChooser_headerButton",columnKey)).find("span");if($targetButton.length===0){$targetButton=$("#"+this.grid.id()+"_"+columnKey)}if(this._visiblePopover&&dropDownId!==this._visiblePopover){fcp=$("#"+this._visiblePopover).data("igGridFeatureChooserPopover");if(fcp){fcp._closePopover()}setTimeout(function(){$dropDown.igGridFeatureChooserPopover("show",$targetButton)},100)}else{$dropDown.igGridFeatureChooserPopover("show",$targetButton)}},hideDropDown:function(columnKey){var $dropDown=this.getDropDownByColumnKey(columnKey);$dropDown.igGridFeatureChooserPopover("hide");this.grid._focusedElement=null},getDropDownByColumnKey:function(columnKey){var dropDownId=this._id("featureChooser_dd",columnKey),$dropDown=$("#"+dropDownId);if($dropDown.length===0){this._renderMenu(columnKey);$dropDown=$("#"+dropDownId)}return $dropDown},toggleDropDown:function(columnKey){var $dropDown=this.getDropDownByColumnKey(columnKey),isVisible=$dropDown.igGridFeatureChooserPopover("isShown");this._trigger(this.events.menuToggling,null,{isVisible:isVisible,columnKey:columnKey,owner:this});if(isVisible){this.hideDropDown(columnKey)}else{this.showDropDown(columnKey)}},_mouseOverDropDownItem:function(event){this._addCssSelectionListItem($(event.currentTarget))},_mouseOutDropDownItem:function(event){this._removeCssSelectionListItem($(event.currentTarget))},_addCssSelectionListItem:function($listItem){$listItem.addClass(this.css.dropDownListItemHover)},_removeCssSelectionListItem:function($listItem){$listItem.removeClass(this.css.dropDownListItemHover)},_virtualHorizontalScroll:function(){this._renderFCForAllColumns()},destroy:function(e,args){var i,l,self=this,sbm=this._submenus,grid;if(args&&args.owner){grid=args.owner;if(grid&&grid.id()!==this.gridId){return}}this.data=null;$.each(this.grid.options.columns,function(ind,column){var $dd=$("#"+self._id("featureChooser_dd",column.key));if($dd.length>0){$dd.igGridFeatureChooserPopover("destroy");
$dd.remove()}});if(sbm){l=sbm.length;for(i=0;i<l;i++){$("#"+sbm[i]).remove()}}if(this._headerRenderedHandler){this.grid.element.unbind("iggridheaderrendered",this._headerRenderedHandler)}if(this._gridDestroyedHandler){this.grid.element.unbind("igcontroldestroyed",this._gridDestroyedHandler)}if(this._gridRenderedHandler){this.grid.element.unbind("iggridrendered",this._gridRenderedHandler)}if(this._virtualHorizontalScrollHandler){this.grid.element.unbind("iggridvirtualhorizontalscroll",this._virtualHorizontalScrollHandler)}delete this._gridRenderedHandler;delete this._headerRenderedHandler;delete this._gridDestroyedHandler;delete this._virtualHorizontalScrollHandler;$.Widget.prototype.destroy.apply(this,arguments);return this}});$.extend($.ui.igGridFeatureChooser,{version:"15.2.20152.1027"})})(jQuery);