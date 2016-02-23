﻿/*!@license
* Infragistics.Web.ClientUI LinearGauge 15.2.20152.1027
*
* Copyright (c) 2011-2015 Infragistics Inc.
*
* http://www.infragistics.com/
*
* Depends on:
*     jquery-1.8.3.js
*     jquery.ui.core.js
*     jquery.ui.widget.js
*     infragistics.util.js
*     infragistics.dv.simple.core.js
*     infragistics.gauge_lineargauge.js
*/
if(typeof jQuery==="undefined"){throw new Error("jQuery is undefined")}(function($){$.widget("ui.igLinearGauge",{css:{unsupportedBrowserClass:"ui-html5-non-html5-supported-message ui-helper-clearfix ui-html5-non-html5",linearGauge:"ui-lineargauge",tooltip:"ui-lineargauge-tooltip ui-corner-all",linearGaugeNeedle:"ui-lineargauge-needle",linearGaugeBacking:"ui-lineargauge-backing",linearGaugeTick:"ui-lineargauge-tick",linearGaugeMinorTick:"ui-lineargauge-minortick",linearGaugeLabel:"ui-lineargauge-label",linearGaugePalette:"ui-lineargauge-range-palette-n",linearGaugeFillPalette:"ui-lineargauge-range-fill-palette-n",linearGaugeOutlinePalette:"ui-lineargauge-range-outline-palette-n"},events:{formatLabel:null,alignLabel:null},options:{width:null,height:null,ranges:[{name:null,brush:null,outline:null,startValue:0,endValue:0,innerStartExtent:0,innerEndExtent:0,outerStartExtent:0,outerEndExtent:0,strokeThickness:1}],rangeToolTipTemplate:null,needleToolTipTemplate:null,orientation:"horizontal",rangeBrushes:null,rangeOutlines:null,minimumValue:0,maximumValue:100,value:0,needleShape:null,needleName:null,rangeInnerExtent:.05,scaleInnerExtent:0,rangeOuterExtent:.65,scaleOuterExtent:0,needleInnerExtent:0,needleOuterExtent:0,needleInnerBaseWidth:0,needleOuterBaseWidth:0,needleInnerPointWidth:0,needleOuterPointWidth:0,needleInnerPointExtent:0,needleOuterPointExtent:0,interval:0,ticksPostInitial:0,ticksPreTerminal:0,labelInterval:0,labelExtent:0,labelsPostInitial:0,labelsPreTerminal:0,minorTickCount:3,tickStartExtent:.05,tickEndExtent:.65,tickStrokeThickness:2,tickBrush:null,fontBrush:null,needleBreadth:6,needleBrush:null,needleOutline:null,needleStrokeThickness:1,minorTickStartExtent:.05,minorTickEndExtent:.35,minorTickStrokeThickness:1,minorTickBrush:null,isScaleInverted:false,backingBrush:null,backingOutline:null,backingStrokeThickness:2,backingInnerExtent:0,backingOuterExtent:1,scaleStartExtent:.05,scaleEndExtent:.95,scaleBrush:null,scaleOutline:null,scaleStrokeThickness:0,transitionDuration:0,showToolTipTimeout:0,showToolTip:null,font:null},_setOption:function(key,value,checkPrev){var linearGauge=this._bulletGraph,o=this.options;if(checkPrev&&o[key]===value){return}$.Widget.prototype._setOption.apply(this,arguments);if(this._set_option(linearGauge,key,value)){return this}this._set_generated_option(linearGauge,key,value);return this},_set_generated_option:function(linearGauge,key,value){switch(key){case"orientation":switch(value){case"horizontal":linearGauge.orientation(0);break;case"vertical":linearGauge.orientation(1);break}return true;case"rangeBrushes":var isRGB=true,val=value?value[0]:null;if(typeof val=="string"&&val=="HSV"||val=="RGB"){if(value[0]=="HSV"){isRGB=false}value=value.slice(1)}var $tempBrushCollection=new $.ig.BrushCollection;for(var i=0;value&&i<value.length;i++){var $tempBrush=$.ig.Brush.prototype.create(value[i]);$tempBrushCollection.add($tempBrush)}linearGauge.rangeBrushes($tempBrushCollection);return true;case"rangeOutlines":var isRGB=true,val=value?value[0]:null;if(typeof val=="string"&&val=="HSV"||val=="RGB"){if(value[0]=="HSV"){isRGB=false}value=value.slice(1)}var $tempBrushCollection=new $.ig.BrushCollection;for(var i=0;value&&i<value.length;i++){var $tempBrush=$.ig.Brush.prototype.create(value[i]);$tempBrushCollection.add($tempBrush)}linearGauge.rangeOutlines($tempBrushCollection);return true;case"minimumValue":linearGauge.minimumValue(value);return true;case"maximumValue":linearGauge.maximumValue(value);return true;case"value":linearGauge.value(value);return true;case"needleShape":switch(value){case"custom":linearGauge.needleShape(0);break;case"rectangle":linearGauge.needleShape(1);break;case"triangle":linearGauge.needleShape(2);break;case"needle":linearGauge.needleShape(3);break;case"trapezoid":linearGauge.needleShape(4);break}return true;case"needleName":linearGauge.needleName(value);return true;case"rangeInnerExtent":linearGauge.rangeInnerExtent(value);return true;case"scaleInnerExtent":linearGauge.scaleInnerExtent(value);return true;case"rangeOuterExtent":linearGauge.rangeOuterExtent(value);return true;case"scaleOuterExtent":linearGauge.scaleOuterExtent(value);return true;case"needleInnerExtent":linearGauge.needleInnerExtent(value);return true;case"needleOuterExtent":linearGauge.needleOuterExtent(value);return true;case"needleInnerBaseWidth":linearGauge.needleInnerBaseWidth(value);return true;case"needleOuterBaseWidth":linearGauge.needleOuterBaseWidth(value);return true;case"needleInnerPointWidth":linearGauge.needleInnerPointWidth(value);return true;case"needleOuterPointWidth":linearGauge.needleOuterPointWidth(value);return true;case"needleInnerPointExtent":linearGauge.needleInnerPointExtent(value);return true;case"needleOuterPointExtent":linearGauge.needleOuterPointExtent(value);return true;case"interval":linearGauge.interval(value);return true;case"ticksPostInitial":linearGauge.ticksPostInitial(value);return true;case"ticksPreTerminal":linearGauge.ticksPreTerminal(value);return true;case"labelInterval":linearGauge.labelInterval(value);return true;case"labelExtent":linearGauge.labelExtent(value);return true;case"labelsPostInitial":linearGauge.labelsPostInitial(value);return true;case"labelsPreTerminal":linearGauge.labelsPreTerminal(value);return true;case"minorTickCount":linearGauge.minorTickCount(value);return true;case"tickStartExtent":linearGauge.tickStartExtent(value);return true;case"tickEndExtent":linearGauge.tickEndExtent(value);return true;case"tickStrokeThickness":linearGauge.tickStrokeThickness(value);return true;case"tickBrush":if(value==null){linearGauge.tickBrush(null)}else{var $tempBrush=$.ig.Brush.prototype.create(value);linearGauge.tickBrush($tempBrush)}return true;case"fontBrush":if(value==null){linearGauge.fontBrush(null)}else{var $tempBrush=$.ig.Brush.prototype.create(value);linearGauge.fontBrush($tempBrush)}return true;case"needleBreadth":linearGauge.needleBreadth(value);return true;case"needleBrush":if(value==null){linearGauge.needleBrush(null)}else{var $tempBrush=$.ig.Brush.prototype.create(value);linearGauge.needleBrush($tempBrush)}return true;case"needleOutline":if(value==null){linearGauge.needleOutline(null)}else{var $tempBrush=$.ig.Brush.prototype.create(value);linearGauge.needleOutline($tempBrush)}return true;case"needleStrokeThickness":linearGauge.needleStrokeThickness(value);return true;case"minorTickStartExtent":linearGauge.minorTickStartExtent(value);return true;case"minorTickEndExtent":linearGauge.minorTickEndExtent(value);return true;case"minorTickStrokeThickness":linearGauge.minorTickStrokeThickness(value);return true;case"minorTickBrush":if(value==null){linearGauge.minorTickBrush(null)}else{var $tempBrush=$.ig.Brush.prototype.create(value);linearGauge.minorTickBrush($tempBrush)}return true;case"isScaleInverted":linearGauge.isScaleInverted(value);return true;case"backingBrush":if(value==null){linearGauge.backingBrush(null)}else{var $tempBrush=$.ig.Brush.prototype.create(value);linearGauge.backingBrush($tempBrush)}return true;case"backingOutline":if(value==null){linearGauge.backingOutline(null)}else{var $tempBrush=$.ig.Brush.prototype.create(value);linearGauge.backingOutline($tempBrush)}return true;case"backingStrokeThickness":linearGauge.backingStrokeThickness(value);return true;case"backingInnerExtent":linearGauge.backingInnerExtent(value);return true;case"backingOuterExtent":linearGauge.backingOuterExtent(value);return true;case"scaleStartExtent":linearGauge.scaleStartExtent(value);return true;case"scaleEndExtent":linearGauge.scaleEndExtent(value);return true;case"scaleBrush":if(value==null){linearGauge.scaleBrush(null)}else{var $tempBrush=$.ig.Brush.prototype.create(value);linearGauge.scaleBrush($tempBrush)}return true;case"scaleOutline":if(value==null){linearGauge.scaleOutline(null)}else{var $tempBrush=$.ig.Brush.prototype.create(value);linearGauge.scaleOutline($tempBrush)}return true;case"scaleStrokeThickness":linearGauge.scaleStrokeThickness(value);return true;case"transitionDuration":linearGauge.transitionDuration(value);return true;case"showToolTipTimeout":linearGauge.showToolTipTimeout(value);return true;case"showToolTip":linearGauge.showToolTip(value);return true;case"font":linearGauge.font(value);return true}},_set_option:function(linearGauge,key,value){var currentKey,templ;switch(key){case"formatLabel":case"alignLabel":linearGauge.refresh();return true;case"ranges":var count=value.length;for(i=0;i<count;i++){if(!value[i].name){throw new Error("Range name is missing for range: "+i)}if(this._rangesColl.hasOwnProperty(value[i].name)){this._removeCollValue(linearGauge.ranges(),this._rangesColl,value[i]);this._updateCollValue(linearGauge.ranges(),this._rangesColl,value[i],this._setRangeOption)}else this._addCollValue(linearGauge.ranges(),this._rangesColl,value[i],this._setRangeOption,function(){return new $.ig.XamLinearGraphRange})}return true;case"width":this._setSize(linearGauge,"width",value);return true;case"height":this._setSize(linearGauge,"height",value);return true;case"rangeToolTipTemplate":this._tooltipTemplates["range"]=this._resolveTemplate(value);return true;case"actualValueTooltipTemplate":this._tooltipTemplates["actualValue"]=this._resolveTemplate(value);return true;case"needleToolTipTemplate":this._tooltipTemplates["needle"]=this._resolveTemplate(value);return true;case"showToolTip":if(value===true){var tooltip=$("<div class='"+this.css.tooltip+"' style='white-space: nowrap; z-index: 10000;'></div>");this._addTooltip(linearGauge,tooltip,"range")}if(value===false){this._removeTooltip(linearGauge)}linearGauge.showToolTip(value);return true}},_resolveTemplate:function(value){var templ;if($.ig.tmpl){if(this._htmlCheckExpr.test(value)){templ=value}else{if($("#"+value).length>0){templ=$("#"+value).text()}else if($(value).length>0){templ=$(value).text()}else{templ=value}}return templ}},_htmlCheckExpr:/^[^<]*(<[\w\W]+>)[^>]*$/,_addTooltip:function(linearGauge,tooltip,name){this._removeTooltipEvents(linearGauge,tooltip);this._bindTooltipEvents(linearGauge,tooltip);linearGauge.toolTip(tooltip)},_removeTooltip:function(linearGauge,name){this._removeTooltipEvents(linearGauge,linearGauge.toolTip());linearGauge.toolTip(null)},_bindTooltipEvents:function(chart,tooltip){tooltip.updateToolTip=$.ig.Delegate.prototype.combine(tooltip.updateToolTip,jQuery.proxy(this._fireToolTip_updateToolTip,this));tooltip.hideToolTip=$.ig.Delegate.prototype.combine(tooltip.hideToolTip,jQuery.proxy(this._fireToolTip_hideToolTip,this))},_removeTooltipEvents:function(chart,tooltip){delete tooltip.updateToolTip;delete tooltip.hideToolTip},_resolveTemplateContext:function(args,name){var e={};e.itemName=args.itemName();e.itemBrush=args.brush().fill();e.outline=args.outline().fill();e.thickness=args.thickness();e.label=args.label();switch(name){case"range":e.item={name:args.item().name(),startValue:args.item().startValue(),endValue:args.item().endValue()};break;case"needle":e.item={name:e.itemName,value:args.item()};break}return e},_fireToolTip_updateToolTip:function(args,name){var e,noCancel=true,template;template=this._tooltipTemplates[name];if(template===undefined){template=this._tooltipDefaultTemplates[name];this._tooltipTemplates[name]=template}this._bulletGraph.toolTip().children().remove();e=this._resolveTemplateContext(args,name);if(e.item===null){noCancel=false}if(noCancel){var templ=$.ig.tmpl(template,e);this._bulletGraph.toolTip().html(templ)}},getRangeNames:function(){var rangeNames="";for(var key in this._rangesColl){rangeNames+=key+"\n"}return rangeNames},addRange:function(value){this._addCollValue(this._bulletGraph.ranges(),this._rangesColl,value,this._setRangeOption,function(){return new $.ig.XamLinearGraphRange})},_addCollValue:function(target,source,value,setItemOptionCallback,createItemCallback){if(!value||value.remove==true||!source)return;if(!source.hasOwnProperty(value.name)){var range=createItemCallback();for(currentKey in value){if(value.hasOwnProperty(currentKey)){setItemOptionCallback(range,currentKey,value[currentKey])}}source[value.name]=range;target.add(range)}},removeRange:function(value){this._removeCollValue(this._bulletGraph.ranges(),this._rangesColl,value)},_removeCollValue:function(target,source,value){if(!value||!source)return;if(source.hasOwnProperty(value.name)){var range=source[value.name];if(range&&value.remove==true){delete source[value.name];if(target.contains(range))target.remove(range)}}},updateRange:function(value){this._updateCollValue(this._bulletGraph.ranges(),this._rangesColl,value,this._setRangeOption)},_updateCollValue:function(target,source,value,setItemOptionCallBack){if(!value||!source)return;if(source.hasOwnProperty(value.name)){var range=source[value.name];if(range&&!value.remove){for(currentKey in value){if(value.hasOwnProperty(currentKey)){setItemOptionCallBack(range,currentKey,value[currentKey])}}source[value.name]=range}}},_creationOptions:null,_bulletGraph:null,_bulletGraphId:null,_createWidget:function(options,element,widget){this._creationOptions=options;$.Widget.prototype._createWidget.apply(this,[options,element])},_create:function(){var key,v,size,linearGauge,width,height,i=-1,self=this,elem=self.element,style=elem[0].style,o=this._creationOptions;self._old_state={style:{position:style.position,width:style.width,height:style.height},css:elem[0].className,elems:elem.find("*")};if(!$.ig.util._isCanvasSupported()){$.ig.util._renderUnsupportedBrowser(this);return}linearGauge=this._createLinearGauge();self._bulletGraph=linearGauge;linearGauge.formatLabel=$.ig.Delegate.prototype.combine(linearGauge.formatLabel,jQuery.proxy(this._fireLinearGauge_formatLabel,this));linearGauge.alignLabel=$.ig.Delegate.prototype.combine(linearGauge.alignLabel,jQuery.proxy(this._fireLinearGauge_alignLabel,this));this._bulletGraphId=Date.now();this._tooltipTemplates={};this._rangesColl={};this._needlesColl={};this._tooltipDefaultTemplates={};this._tooltipDefaultTemplates["range"]="<div class='ui-lineargauge-range-tooltip' style='border-color: ${itemBrush};'><span>${label}</span></div>";this._tooltipDefaultTemplates["needle"]="<div class='ui-lineargauge-needle-tooltip' style='border-color: ${itemBrush};'><span>${label}</span></div>";if(o.hasOwnProperty("width"))elem[0].style.width=o["width"];if(o.hasOwnProperty("height"))elem[0].style.height=o["height"];linearGauge.provideContainer(elem[0]);elem.mousemove(function(eventData){var point=$.ig.APIFactory.prototype.createPoint(eventData.pageX,eventData.pageY);linearGauge.onMouseOver(point,true)});elem.mouseleave(function(){linearGauge.onMouseLeave()});for(key in o){if(o.hasOwnProperty(key)){v=o[key];if(v!==null){this._setOption(key,v,false)}}}while(i++<1){key=i===0?"width":"height";if(o[key]){size=key;v=o[key]}else{v=elem[0].style[key]}if(v&&typeof v==="string"&&v.indexOf("%")>0){self._setSize(linearGauge,size=key,v)}}if(!size){self._setSize(linearGauge,"width")}if(self.css&&self.css.linearGauge){elem.addClass(self.css.linearGauge)}},_createLinearGauge:function(){return new $.ig.XamLinearGauge},_fireLinearGauge_formatLabel:function(linearGauge,evt){var opts={};opts.actualMinimumValue=evt.actualMinimumValue;opts.actualMaximumValue=evt.actualMaximumValue;opts.value=evt.value;opts.label=evt.label;opts.owner=this;this._trigger("formatLabel",null,opts);evt.value=opts.value;evt.label=opts.label},_fireLinearGauge_alignLabel:function(linearGauge,evt){var opts={};opts.actualMinimumValue=evt.actualMinimumValue;opts.actualMaximumValue=evt.actualMaximumValue;opts.value=evt.value;opts.label=evt.label;opts.width=evt.width;opts.height=evt.height;opts.offsetX=evt.offsetX;opts.offsetY=evt.offsetY;opts.owner=this;this._trigger("alignLabel",null,opts);evt.value=opts.value;evt.label=opts.label;evt.offsetX=opts.offsetX;evt.offsetY=opts.offsetY;evt.width=opts.width;evt.height=opts.height},_setSize:function(linearGauge,key,val){$.ig.util.setSize(this.element,key,val,linearGauge,this._getNotifyResizeName())},_getNotifyResizeName:function(){return"containerResized"},_setRangeOption:function(range,key,value){switch(key){case"name":range.name(value);break;case"brush":range.brush($.ig.Brush.prototype.create(value));break;case"outline":range.outline($.ig.Brush.prototype.create(value));break;case"startValue":range.startValue(value);break;case"endValue":range.endValue(value);break;case"innerStartExtent":range.innerStartExtent(value);break;case"innerEndExtent":range.innerEndExtent(value);break;case"outerStartExtent":range.outerStartExtent(value);break;case"outerEndExtent":range.outerEndExtent(value);break;case"strokeThickness":range.strokeThickness(value);break}},getValueForPoint:function(x,y){if(this._bulletGraph){var point=$.ig.APIFactory.prototype.createPoint(x,y);return this._bulletGraph.getValueForPoint(point)}},needleContainsPoint:function(x,y){if(this._bulletGraph){var point=$.ig.APIFactory.prototype.createPoint(x,y);return this._bulletGraph.needleContainsPoint(point)}},exportVisualData:function(){if(this._bulletGraph)return this._bulletGraph.exportVisualData()},flush:function(){if(this._bulletGraph&&this._bulletGraph.view())this._bulletGraph.view().flush()},destroy:function(){var key,style,linearGauge=this._bulletGraph,old=this._old_state,elem=this.element;if(!old){return}elem.find("*").not(old.elems).remove();if(this.css.linearGauge){elem.removeClass(this.css.linearGauge)}old=old.style;style=elem[0].style;for(key in old){if(old.hasOwnProperty(key)){if(style[key]!==old[key]){style[key]=old[key]}}}if(linearGauge){this._setSize(linearGauge)}$.Widget.prototype.destroy.apply(this,arguments);if(linearGauge&&linearGauge.destroy){linearGauge.destroy()}delete this._bulletGraph;delete this._old_state},styleUpdated:function(){if(this._bulletGraph){this._bulletGraph.styleUpdated()}}});$.extend($.ui.igLinearGauge,{version:"15.2.20152.1027"})})(jQuery);