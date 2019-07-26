/*!@license
* Infragistics.Web.ClientUI Data Binding Plugin 19.1.20
*
* Copyright (c) 2011-2019 Infragistics Inc.
*
* igOlapXmlaDataSource is an OLAP data source that connects to Xmla Analysis Services over HTTP.
*
* http://www.infragistics.com/
*
* Depends on:
*	jquery-1.9.1.js
*	infragistics.util.js
*
*/
(function(factory){if(typeof define==="function"&&define.amd){define(["jquery","./infragistics.util"],factory)}else{return factory(jQuery)}})(function($){$.ig=$.ig||{};$.ig.OlapXmlaDataSource=$.ig.OlapXmlaDataSource||Class.extend({options:{serverUrl:null,catalog:null,cube:null,measureGroup:null,measures:null,filters:null,rows:null,columns:null,requestOptions:{withCredentials:false,beforeSend:null},enableResultCache:true,discoverProperties:null,executeProperties:null,mdxSettings:{nonEmptyOnRows:true,nonEmptyOnColumns:true,addCalculatedMembersOnRows:true,addCalculatedMembersOnColumns:true,dimensionPropertiesOnRows:[],dimensionPropertiesOnColumns:[]},isRemote:false},initialize:function(){},isInitialized:function(){return this.dataSource().isInitialized()},isModified:function(){return this.dataSource().isInitialized()},isUpdating:function(){return this.dataSource().isInitialized()},catalogs:function(){},catalog:function(){},setCatalog:function(catalogName){},cubes:function(){},cube:function(){},setCube:function(cubeName){},measureGroups:function(){},measureGroup:function(){},setMeasureGroup:function(measureGroupName){},metadataTree:function(){},addRowItem:function(rowItem){},removeRowItem:function(rowItem){},addColumnItem:function(columnItem){},removeColumnItem:function(columnItem){},addFilterItem:function(filterItem){},removeFilterItem:function(filterItem){},addMeasureItem:function(measureItem){},removeMeasureItem:function(measureItem){},setMeasureListIndex:function(index){},setMeasureListLocation:function(location){},expandTupleMember:function(axisName,tupleIndex,memberIndex){},collapseTupleMember:function(axisName,tupleIndex,memberIndex){},rowAxis:function(){},columnAxis:function(){},filters:function(){},measures:function(){},result:function(){},clearPendingChanges:function(){},update:function(){},getCoreElement:function(predicate,elementType){},getCoreElements:function(predicate,elementType){},getDimension:function(dimensionUniqueName){},getHierarchy:function(hierarchyUniqueName){},getLevel:function(levelUniqueName){},getMeasure:function(measureUniqueName){},getMeasureList:function(){},getFilterMemberNames:function(hierarchyUniqueName){},addFilterMember:function(hierarchyUniqueName,memberUniqueName){},removeFilterMember:function(hierarchyUniqueName,memberUniqueName){},removeAllFilterMembers:function(hierarchyUniqueName){},getMembersOfLevel:function(levelUniqueName){},getMembersOfHierarchy:function(hierarchyUniqueName){},getMembersOfMember:function(memberUniqueName){}});$.ig.OlapFlatDataSource=$.ig.OlapFlatDataSource||Class.extend({options:{dataSource:null,dataSourceUrl:null,dataSourceType:null,responseDataKey:null,responseDataType:null,measures:null,filters:null,rows:null,columns:null,metadata:{cube:{name:null,caption:null,measuresDimension:{name:null,caption:null,measures:[{name:null,caption:null,aggregator:null,displayFolder:null}]},dimensions:[{name:null,caption:null,hierarchies:[{name:null,caption:null,displayFolder:null,levels:[{name:null,caption:null,memberProvider:null}]}]}]}}},initialize:function(){},isInitialized:function(){return this.dataSource().isInitialized()},isModified:function(){return this.dataSource().isInitialized()},isUpdating:function(){return this.dataSource().isInitialized()},cubes:function(){},cube:function(){},setCube:function(cubeName){},metadataTree:function(){},addRowItem:function(rowItem){},removeRowItem:function(rowItem){},addColumnItem:function(columnItem){},removeColumnItem:function(columnItem){},addFilterItem:function(filterItem){},removeFilterItem:function(filterItem){},addMeasureItem:function(measureItem){},removeMeasureItem:function(measureItem){},setMeasureListIndex:function(index){},setMeasureListLocation:function(location){},expandTupleMember:function(axisName,tupleIndex,memberIndex){},collapseTupleMember:function(axisName,tupleIndex,memberIndex){},rowAxis:function(){},columnAxis:function(){},filters:function(){},measures:function(){},result:function(){},clearPendingChanges:function(){},update:function(){},getCoreElement:function(predicate,elementType){},getCoreElements:function(predicate,elementType){},getDimension:function(dimensionUniqueName){},getHierarchy:function(hierarchyUniqueName){},getLevel:function(levelUniqueName){},getMeasure:function(measureUniqueName){},getMeasureList:function(){},getFilterMemberNames:function(hierarchyUniqueName){},addFilterMember:function(hierarchyUniqueName,memberUniqueName){},removeFilterMember:function(hierarchyUniqueName,memberUniqueName){},removeAllFilterMembers:function(hierarchyUniqueName){},getMembersOfLevel:function(levelUniqueName){},getMembersOfHierarchy:function(hierarchyUniqueName){},getMembersOfMember:function(memberUniqueName){}});$.ig.OlapMetadataTreeItem=$.ig.OlapMetadataTreeItem||Class.extend({item:function(){},type:function(){},caption:function(){},children:function(){}});$.ig.OlapResultView=$.ig.OlapResultView||Class.extend({options:{result:null,visibleResult:null,hasColumns:null,hasRows:null},collapseTupleMember:function(axisName,tupleIndex,memberIndex){},expandTupleMember:function(axisName,tupleIndex,memberIndex){},extend:function(partialResult,axisName){}});$.ig.OlapTableView=$.ig.OlapTableView||Class.extend({options:{result:null,hasColumns:null,hasRows:null,viewSettings:{isParentInFrontForColumns:false,isParentInFrontForRows:true,compactColumnHeaders:false,compactRowHeaders:true}},initialize:function(){},viewSettings:function(){},columnSortDirections:function(columnSortDirections){},appliedColumnSortDirections:function(){},levelSortDirections:function(levelSortDirections){},appliedLevelSortDirections:function(){},appliedSortDirectionsMap:function(){},rowHeaders:function(){},columnHeaders:function(){},resultCells:function(){},result:function(){}});$.ig.OlapTableViewHeaderCell=$.ig.OlapTableViewHeaderCell||Class.extend({caption:function(){},isExpanded:function(){},isExpanable:function(){},rowIndex:function(){},rowSpan:function(){},columnIndex:function(){},columnSpan:function(){},axisName:function(){},tupleIndex:function(){},memberIndex:function(){}});$.ig.OlapTableViewResultCell=$.ig.OlapTableViewResultCell||Class.extend({value:function(){},formattedValue:function(){},cellOrdinal:function(){},resultCellIndex:function(){}});$.ig.Catalog=$.ig.Catalog||Class.extend({name:function(value){},uniqueName:function(value){},caption:function(value){},description:function(value){}});$.ig.Cube=$.ig.Cube||Class.extend({name:function(value){},uniqueName:function(value){},caption:function(value){},description:function(value){},cubeType:function(value){},lastProcessed:function(value){},lastUpdated:function(value){}});$.ig.Dimension=$.ig.Dimension||Class.extend({name:function(value){},uniqueName:function(value){},caption:function(value){},description:function(value){},dimensionType:function(value){}});$.ig.Hierarchy=$.ig.Hierarchy||Class.extend({name:function(value){},uniqueName:function(value){},caption:function(value){},description:function(value){},defaultMember:function(value){},allMember:function(value){},dimensionUniqueName:function(value){},hierarchyOrigin:function(value){},hierarchyDisplayFolder:function(value){}});$.ig.Measure=$.ig.Measure||Class.extend({name:function(value){},uniqueName:function(value){},caption:function(value){},description:function(value){},measureGroupName:function(value){},aggregatorType:function(value){},defaultFormatString:function(value){},measureDisplayFolder:function(value){}});$.ig.Level=$.ig.Level||Class.extend({name:function(value){},uniqueName:function(value){},caption:function(value){},description:function(value){},depth:function(value){},hierarchyUniqueName:function(value){},dimensionUniqueName:function(value){},membersCount:function(value){},levelOrigin:function(value){},levelOrderingProperty:function(value){}});$.ig.MeasureGroup=$.ig.MeasureGroup||Class.extend({name:function(value){},caption:function(value){},description:function(value){},catalogName:function(value){},cubeName:function(value){}});$.ig.MeasureList=$.ig.MeasureList||Class.extend({caption:function(value){},measures:function(value){}});$.ig.OlapResult=$.ig.OlapResult||Class.extend({isEmpty:function(value){},axes:function(value){},cells:function(value){}});$.ig.OlapResultAxis=$.ig.OlapResultAxis||Class.extend({options:{tuples:null,tupleSize:null},tuples:function(){},tupleSize:function(){}});$.ig.OlapResultTuple=$.ig.OlapResultTuple||Class.extend({options:{members:null},members:function(){}});$.ig.OlapResultAxisMember=$.ig.OlapResultAxisMember||Class.extend({uniqueName:function(value){},caption:function(value){},levelUniqueName:function(value){},hierarchyUniqueName:function(value){},levelNumber:function(value){},displayInfo:function(value){},childCount:function(value){},drilledDown:function(value){},parentSameAsPrev:function(value){},properties:function(value){}});$.ig.OlapResultCell=$.ig.OlapResultCell||Class.extend({cellOrdinal:function(value){},properties:function(value){}})});