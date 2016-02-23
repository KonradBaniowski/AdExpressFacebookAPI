﻿/*!@license Infragistics.Mobile.Loader 15.2.20152.1027
*
* Copyright (c) 2011-2015 Infragistics Inc.
*
* http://www.infragistics.com/
*
*/
if(typeof jQuery!=="function"){throw new Error("jQuery is undefined")}$.ig=$.ig||{};$.ig.loaderClass=$.ig.loaderClass||{};$.ig.loaderClass.locale={listGroup:"Lists",editorsGroup:"Editors",frameworkGroup:"Framework",miscGroup:"Miscellaneous"};$.ig.dependencies=[{widget:"theme",scripts:[],internal:true,css:["$path$/themes/$theme$/infragistics.mobile.theme.css"]},{widget:"igDataSource",dependency:[{name:"igUtil"}],scripts:["$path$/modules/infragistics.datasource.js"],locale:["$localePath$/infragistics.datasource-$locale$.js"],group:$.ig.loaderClass.locale.frameworkGroup,css:[]},{widget:"igTemplating",dependency:[{name:"igUtil"}],scripts:["$path$/modules/infragistics.templating.js"],group:$.ig.loaderClass.locale.miscGroup,css:[]},{widget:"igUtil",priority:true,scripts:["$path$/modules/infragistics.util.js"],group:$.ig.loaderClass.locale.miscGroup,css:[]},{widget:"igScroll",scripts:["$path$/modules/infragistics.ui.scroll.js"],group:$.ig.loaderClass.locale.miscGroup,css:[]},{widget:"igmList",dependency:[{name:"igUtil"},{name:"igTemplating"},{name:"igDataSource"}],scripts:["$path$/modules/infragistics.mobileui.list.js"],css:["$path$/structure/modules/infragistics.mobile.list.css"],locale:["$localePath$/infragistics.mobileui.list-$locale$.js"],group:$.ig.loaderClass.locale.listGroup,regional:["$localePath$/regional/infragistics.ui.regional-$regional$.js"]},{widget:"Filtering",parentWidget:"igmList",dependency:[{name:"igmList"}],scripts:["$path$/modules/infragistics.mobileui.list.filtering.js"]},{widget:"LoadOnDemand",parentWidget:"igmList",dependency:[{name:"igmList"}],scripts:["$path$/modules/infragistics.mobileui.list.loadondemand.js"]},{widget:"Sorting",parentWidget:"igmList",dependency:[{name:"igmList"}],scripts:["$path$/modules/infragistics.mobileui.list.sorting.js"]},{widget:"igmList.*",dependency:[{name:"igmList"},{name:"Filtering"},{name:"LoadOnDemand"},{name:"Sorting"}]},{widget:"igmRating",dependency:[{name:"igUtil"}],scripts:["$path$/modules/infragistics.mobileui.rating.js"],group:$.ig.loaderClass.locale.editorsGroup,css:["$path$/structure/modules/infragistics.mobile.rating.css"]}];$.ig.theme="ios";if(window.navigator.userAgent.indexOf("Android")>=0){$.ig.theme="android/holodark"}else if(window.navigator.userAgent.indexOf("Windows Phone")>=0){$.ig.theme="windowsphone/dark"}if($.ig.isDocumentReady===undefined){$.ig.isDocumentReady=false;$(document).bind("pagebeforeload",function(){$.ig.isDocumentReady=false});$(document).bind("pageinit",function(){$.ig.isDocumentReady=true;if($.ig._loader){$.ig.loader()._notifyLoaded()}})}$.extend($.ig,{loader:function(param1,param2,param3){var options,callback,resources;function assignParameter(p){if(typeof p==="object"){options=p}if(typeof p==="function"){callback=p}if(typeof p==="string"){resources=p}}if(arguments.length>0){assignParameter(param1)}if(arguments.length>1){assignParameter(param2)}if(arguments.length>2){assignParameter(param3)}if(options){if(resources){options.resources=resources}if(callback){options.ready=callback}}$.ig._loader=$.ig.loaderClass;$.ig._loader._init(options);if(!options){$.ig._loader.load(resources,callback)}return $.ig._loader}});$.extend($.ig.loaderClass,{settings:{resources:null,scriptPath:"js",cssPath:"css",theme:$.ig.theme,localePath:"$path$/modules/i18n",autoDetectLocale:false,locale:null,regional:null,preinit:null,ready:null},load:function(resources,callback,preinit){if(!resources&&(this._themeLoaded||!this.settings.theme)){if(!callback&&!preinit){return this}if(callback){this._callbackArray.push(callback)}if(preinit){this._preinitArray.push(preinit)}this._waitBatches(this._proxy(this,this._notifyLoaded,[]))}else{if(!this._themeLoaded&&this.settings.theme){this._themeLoaded=true;resources="theme"+(resources?","+resources:"")}this.settings.ready=callback;var res=resources.split(","),loadBatch={},i;loadBatch.callback=this._proxy(loadBatch,this._callback,[]);loadBatch.waitFireCallback=this._proxy(loadBatch,this._waitFireCallback,[]);loadBatch._noWdgtLoaded=res.length;loadBatch.loader=this;loadBatch.ready=this._proxy(this,this._notifyLoaded,[]);if(callback){this._callbackArray.push(callback)}if(preinit){this._preinitArray.push(preinit)}this._loadBatches.push(loadBatch);for(i=0;i<res.length;i++){new $.ig._loadWorkItem(this).loadWidgetResources($.trim(res[i]),loadBatch.callback)}}return this},preinit:function(callback){if(callback){this._preinitArray.push(callback)}this._notifyLoaded();return this},_themeLoaded:false,_dataLog:"",_loadBatches:[],_resources:$.ig.dependencies,_init:function(options){if(options){var basePath=options.scriptPath,localePath=options.localePath,cssPath=options.cssPath,regional=options.regional,locale=options.locale,userLang;if(basePath&&basePath.length>0){if(basePath.lastIndexOf("/")===basePath.length-1){basePath=basePath.slice(0,basePath.length-1)}this.settings.scriptPath=basePath}if(cssPath&&cssPath.length>0){if(cssPath.lastIndexOf("/")===cssPath.length-1){cssPath=cssPath.slice(0,cssPath.length-1)}this.settings.cssPath=cssPath}if(localePath&&localePath.length>0){if(localePath.lastIndexOf("/")===localePath.length-1){localePath=localePath.slice(0,localePath.length-1)}this.settings.localePath=localePath}if(options.theme!==undefined){this.settings.theme=options.theme}if(options.resources){this.settings.resources=options.resources}if(options.ready){this.settings.ready=options.ready}if(options.preinit){this.settings.preinit=options.preinit}if(options.autoDetectLocale!==undefined){this.settings.autoDetectLocale=options.autoDetectLocale}if(!locale&&this.settings.autoDetectLocale){userLang=navigator.language||navigator.userLanguage;locale=userLang.split("-")[0];if(!regional){regional=locale}}if(locale===this._defaultLocale){locale=""}if(!locale&&this._defaultLocale){this.settings.locale=""}else{if(locale){this.settings.locale=locale}else{this.settings.locale="en"}}if(!regional){regional=options.locale}if(!regional&&this._defaultLocale==="ja"){regional="ja"}if(regional==="en"){regional=null}this.settings.regional=regional;this._initializePaths("script");this._initializePaths("css");if(this.settings.resources){this.load(this.settings.resources,this.settings.ready,this.settings.preinit)}}},_defaultLocale:"en",_proxy:function(instance,method,args){return function(){return method.apply(instance,args)}},_initializePaths:function(type){var i,j,len=this._resources.length,path=type==="script"?this.settings.scriptPath:this.settings.cssPath,localePath=this.settings.localePath,theme=this.settings.theme,scriptData,useLocale=type==="script"&&this.settings.locale,useRegional=type==="script"&&this.settings.regional,localeScripts,regionalScripts;for(i=0;i<len;i++){scriptData=type==="script"?this._resources[i].scripts:this._resources[i].css;scriptData=scriptData||[];if(useLocale){if(this._resources[i].locale){localeScripts=this._resources[i].locale.slice(0);while(localeScripts.length>0){scriptData.unshift(localeScripts.pop().replace("$locale$",this.settings.locale))}}}if(useRegional){if(this._resources[i].regional){regionalScripts=this._resources[i].regional.slice(0);while(regionalScripts.length>0){scriptData.unshift(regionalScripts.pop().replace("$regional$",this.settings.regional))}}}for(j=0;j<scriptData.length;j++){scriptData[j]=scriptData[j].replace("$localePath$",localePath).replace("$path$",path).replace("$theme$",theme)}}},_log:function(data){this._dataLog+=data.toString()+"<br/>"},log:function(){return this._dataLog},_callback:function(){this._noWdgtLoaded--;if(this._noWdgtLoaded<=0){if(this.ready){this.waitFireCallback()}else{this.loader._loadBatches.pop(this)}}},_waitFireCallback:function(){if($.ig.isDocumentReady){this.loader._loadBatches.pop(this);this.ready()}else{window.setTimeout(this.loader._proxy(this,this.waitFireCallback,[]),25)}},_waitBatches:function(callback){if(this._loadBatches.length===0&&$.ig.isDocumentReady){callback()}else{window.setTimeout(this._proxy(this,this._waitBatches,[callback]),25)}},_preinitArray:[],_callbackArray:[],_notifyLoaded:function(){if(this._loadBatches.length>0){return}var pre=this._preinitArray,call=this._callbackArray;if(this._preinitArray.length){this._preinitArray=[]}if(this._callbackArray.length){this._callbackArray=[]}if(this.settings.regional&&typeof $.ig.setRegionalDefault==="function"){$.ig.setRegionalDefault(this.settings.regional)}while(pre.length>0){pre.shift()()}while(call.length>0){call.shift()()}},_findWidgetScriptData:function(widgetName){var i,len=this._resources.length,scriptData;if(widgetName.length>3&&widgetName.indexOf(".js")===widgetName.length-3){return{widget:widgetName,scripts:[this.settings.scriptPath+"/"+widgetName],css:[]}}if(widgetName.length>4&&widgetName.indexOf(".css")===widgetName.length-4){return{widget:widgetName,scripts:[],css:[this.settings.cssPath+"/"+widgetName]}}for(i=0;i<len;i++){scriptData=this._resources[i];if(scriptData.widget===widgetName){return scriptData}}return null}});$.ig._loadWorkItem=function(loader){this._loader=loader;this._loadedCssDependencies=[];this._loadedScriptDependencies=[]};$.ig._loadWorkItem.prototype={_loadingEntities:[],_inArray:function(elem,array){var i,len=array&&elem?array.length:0;for(i=0;i<len;i++){if(elem===array[i]){return true}}return false},_loadDependency:function(scriptData,loadingEntity,type){var j,len=scriptData.dependency?scriptData.dependency.length:0,name;for(j=0;j<len;j++){name=scriptData.dependency[j].name;if(!this._inArray(name,type==="script"?this._loadedScriptDependencies:this._loadedCssDependencies)){this._loadFeatureItem(name,type,loadingEntity);if(type==="script"){this._loadedScriptDependencies.push(name)}else{this._loadedCssDependencies.push(name)}}}},_loadFeatureItem:function(widgetName,type,loadingEntity){var scriptData=this._loader._findWidgetScriptData(widgetName),i,res,len,item;if(!scriptData){throw"Resource '{0}' cannot be found. Please check that the resource name is correct.".replace("{0}",widgetName)}res=type==="script"?scriptData.scripts:scriptData.css;res=res||[];len=res.length;this._loadDependency(scriptData,loadingEntity,type);for(i=0;i<len;i++){item=res[i];this._queueItem(item,loadingEntity,type,scriptData.priority)}},_loadFeatures:function(type,widgetName){var features,i,len;if(widgetName.indexOf("*")===widgetName.length-1||widgetName.length>3&&widgetName.indexOf(".js")===widgetName.length-3||widgetName.length>4&&widgetName.indexOf(".css")===widgetName.length-4){features=[widgetName]}else{features=widgetName.split(".")}len=features.length;for(i=0;i<len;i++){this._loadFeatureItem(features[i],type,widgetName)}},_loadAllFeatures:function(type){var i,j,len=this._loader._resources.length,jlen=0,scriptData,item,loadingEntity="ALL",res;for(i=0;i<len;i++){scriptData=this._loader._resources[i];this._loadDependency(scriptData,loadingEntity,type);res=type==="script"?scriptData.scripts:scriptData.css;res=res||[];jlen=res.length;for(j=0;j<jlen;j++){item=res[j];this._queueItem(item,loadingEntity,type,scriptData.priority)}}},loadWidgetResources:function(widgetName,callback){if(this._loadingEntities[widgetName]!==undefined){this._loadingEntities[widgetName].call.push(callback)}else{this._loadingEntities[widgetName]={name:widgetName,call:[callback],queue:[]}}this._loadFeatures("css",widgetName,callback);this._loadFeatures("script",widgetName,callback);this._loadMonitor()},loadWidgetCss:function(widgetName,callback){this._loadFeatures("css",widgetName,callback)},loadWidgetScripts:function(widgetName,callback){this._loadFeatures("script",widgetName,callback)},loadAllScripts:function(callback){this._loadAllFeatures("script",callback)},loadAllCss:function(callback){this._loadAllFeatures("css",callback)},loadAllResources:function(callback){var loadingEntity="ALL";this._loadingEntities[loadingEntity]={name:loadingEntity,call:[callback],queue:[]};this._loadAllFeatures("css",callback);this._loadAllFeatures("script",callback);this._loadMonitor()},_queueItem:function(fileName,loadingEntity,itemType,itemPriority){if(!this._loadingEntities[loadingEntity].queue[fileName]){this._loadingEntities[loadingEntity].queue[fileName]={type:itemType,loaded:false,priority:itemPriority};this._loader._log("Enqueue: "+fileName)}},_loadScript:function(fileName,loadingEntity){var scriptLoad=this._loadingEntities[loadingEntity].queue[fileName],s,head,i,len,self=this,k,isIE10Plus=false;if(!this.isLoadedScript(fileName)){scriptLoad.loading=true;s=document.createElement("script");head=document.documentElement;len=head.childNodes.length;s.type="text/javascript";s.src=fileName;k=window.navigator.userAgent.indexOf("MSIE ");if(k>=0){isIE10Plus=parseInt(window.navigator.userAgent.substr(k+4),10)>=10}s.async=isIE10Plus;s.loadingEntity=loadingEntity;s.fileName=fileName;s.onreadystatechange=s.onload=function(){if(s.readyState===undefined||s.readyState==="complete"||!isIE10Plus&&s.readyState==="loaded"){scriptLoad.loading=false;self._scriptLoaded(this)}};if(head.nodeName!=="HEAD"){for(i=0;i<len;i++){if(head.childNodes[i].nodeName==="HEAD"){head=head.childNodes[i];break}}}head.appendChild(s)}else{if(!scriptLoad.loading){scriptLoad.loaded=true;this._loadMonitor()}}},_loadCss:function(fileName,loadingEntity){if(!this.isLoadedCss(fileName)){var l=document.createElement("link"),head=document.documentElement,i,len=head.childNodes.length;l.type="text/css";l.rel="stylesheet";l.href=fileName;l.media="screen";l.loadingEntity=loadingEntity;l.fileName=fileName;if(head.nodeName!=="HEAD"){for(i=0;i<len;i++){if(head.childNodes[i].nodeName==="HEAD"){head=head.childNodes[i];break}}}head.appendChild(l)}this._loadingEntities[loadingEntity].queue[fileName].loaded=true;this._loadMonitor()},_scriptLoaded:function(scriptObj){var rs=scriptObj.readyState,entity=this._loadingEntities[scriptObj.loadingEntity],item;if(entity){item=entity.queue[scriptObj.fileName];if(item&&!item.loaded&&(!rs||/loaded|complete/.test(rs))){item.loaded=true;this._loader._log("Loaded: "+scriptObj.fileName);this._loadMonitor()}}},_isLoadedHeadElem:function(src,type){var head=document.documentElement,i,len=head.childNodes.length,elem,nodeSrc;if(head.nodeName!=="HEAD"){for(i=0;i<len;i++){if(head.childNodes[i].nodeName==="HEAD"){head=head.childNodes[i];break}}len=head.childNodes.length}for(i=0;i<len;i++){elem=head.childNodes[i];if(type==="LINK"){nodeSrc=elem.href}else if(type==="SCRIPT"){nodeSrc=elem.src}if(nodeSrc&&elem.nodeName===type&&nodeSrc.lastIndexOf(src)!==-1){return true}}return false},isLoadedScript:function(src){return src&&src.length>0?this._isLoadedHeadElem(src.substring(src.lastIndexOf("/")),"SCRIPT"):false},isLoadedCss:function(src){return src&&src.length>0?this._isLoadedHeadElem(src.substring(src.lastIndexOf("/")),"LINK"):false},_loadMonitor:function(){var i,scriptName,entity,loaded,item,passed,priority,c;for(i in this._loadingEntities){if(this._loadingEntities.hasOwnProperty(i)){entity=this._loadingEntities[i];loaded=true;passed=false;priority=false;for(scriptName in entity.queue){if(entity.queue.hasOwnProperty(scriptName)){passed=true;item=entity.queue[scriptName];loaded=loaded&&item.loaded;if(!loaded){if(item.type==="script"){this._loadScript(scriptName,i)}else{this._loadCss(scriptName,i)}priority=item.priority;break}}}if(loaded&&passed&&entity.call){for(c=0;c<entity.call.length;c++){if(entity.call[c]){entity.call[c]()}}delete this._loadingEntities[entity.name]}else if(passed&&priority){break}}}}};