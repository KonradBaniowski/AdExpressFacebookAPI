/*!@license
 * Infragistics.Web.ClientUI Chart 19.1.20191.172
 *
 * Copyright (c) 2011-2019 Infragistics Inc.
 *
 * http://www.infragistics.com/
 *
 * Depends:
 * jquery.js
 * jquery-ui.js
 * jquery.ui.widget.js
 * infragistics.util.js
 * infragistics.datasource.js
 * infragistics.templating.js
 * infragistics.ext_core.js
 * infragistics.ext_collections.js
 * infragistics.ext_ui.js
 * infragistics.dv_core.js
 * infragistics.dv_geometry.js
 * infragistics.datachart_core.js
 * infragistics.dvcommonwidget.js
 * infragistics.datachart_scatter.js
 * infragistics.ext_text.js
 * infragistics.ext_io.js
 * infragistics.ext_threading.js
 * infragistics.ext_web.js
 * infragistics.dv_geo.js
 * infragistics.geographicmap_core.js
 */
(function(factory){if(typeof define==="function"&&define.amd){define(["jquery","jquery-ui","./infragistics.util","./infragistics.ext_collections","./infragistics.ext_web","./infragistics.dv_geo","./infragistics.datachart_scatter","./infragistics.geographicmap_core"],factory)}else{factory(jQuery)}})(function($){$.widget("ui.igMap",{options:{},css:{chart:"ui-corner-all ui-widget-content",unsupportedBrowserClass:"ui-chart-non-html5-supported-message ui-helper-clearfix"},events:{},_setMapOption:function(){return false},_seriesSetOption:function(){return false}});return $.ui.igMap});