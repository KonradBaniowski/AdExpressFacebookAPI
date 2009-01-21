#region Information
//	Author : 
//	Creation Date:
//	Modification Date:
//		01/08/2006	G Ragneau	new attribute in "selection" balises : validatrionMethod
#endregion

using System;
using System.Data;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TNS.AdExpress.Constantes;
using ClassificationUniverse = TNS.Classification.Universe;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Layers;
using Except = TNS.FrameWork.Exceptions;
using Constante = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpress.Domain.XmlLoader{
	///<summary>
	///Class used to know all the informations of the modules and modules groups
	/// </summary>
	///  <stereotype>utility</stereotype>
    public class ModulesListXL {

		#region Load Xml file
		/// <summary>
		/// Load modules and modules groups data informations from xml file
		/// </summary>
		/// <param name="pathXMLFile">Xml file path</param>
		/// <param name="HtModuleGroup">Modules groups list</param>
		/// <param name="HtModule">Modules List</param>
		public static void Load(IDataSource source,Hashtable HtModuleGroup, Hashtable HtModule){
			XmlTextReader Reader=null;
			HtModule.Clear();
			HtModuleGroup.Clear();
			bool showLink;
			int moduleType = 0;
			OptionalPageInformation currentSelectionPage = null, currentSubSelectionPage = null; 
			try{

				Reader=(XmlTextReader)source.GetSource();
				bool allowRecall=true;
				string helpUrlValue="";
				string optionalNextUrlValue="";
				string methodValue="";
				string rawExcelUrlValue="";
				string printExcelUrlValue="";
				string printBisExcelUrlValue="";
				string exportJpegUrlValue="";
				string remotePdfUrlValue="";
				string remoteResultPdfUrlValue="";
				string remoteTextUrlValue="";
				string remoteExcelUrlValue="";
				string valueExcelUrlValue="";
                string functionName="";
                string allowedUnitValue="";
                bool useBaalForModule=false;
                bool useBaalForResult=false;
				Int64 module=0;
				ResultPageInformation currentResultPageInformation=null;
				while(Reader.Read()){
					allowRecall=true;
					methodValue="";
					helpUrlValue="";
					optionalNextUrlValue="";
					rawExcelUrlValue="";
					printExcelUrlValue="";
					printBisExcelUrlValue="";
					exportJpegUrlValue="";
					remotePdfUrlValue="";
					remoteResultPdfUrlValue="";
					remoteTextUrlValue="";
					remoteExcelUrlValue="";
					valueExcelUrlValue="";
                    functionName="";
					if(Reader.NodeType==XmlNodeType.Element){
						switch(Reader.LocalName){
							case "moduleGroup":
								if(Reader.GetAttribute("privilegecode")!=null && Reader.GetAttribute("traductioncode")!=null && Reader.GetAttribute("flashUrl")!=null && Reader.GetAttribute("type")!=null) {
									HtModuleGroup.Add(Int64.Parse(Reader.GetAttribute("privilegecode")),new ModuleGroup(Int64.Parse(Reader.GetAttribute("privilegecode")),Int64.Parse(Reader.GetAttribute("traductioncode")),Reader.GetAttribute("flashUrl"),Reader.GetAttribute("missingFlashUrl"),Int64.Parse(Reader.GetAttribute("descriptionWebTextId"))));
									moduleType=int.Parse(Reader.GetAttribute("type"));
								}
								break;
							case "module":
                                if(Reader.GetAttribute("privilegecode") != null && Reader.GetAttribute("traductioncode") != null && Reader.GetAttribute("urlNextPage") != null && Reader.GetAttribute("descriptionWebTextId") != null && Reader.GetAttribute("descriptionImageName") != null && Reader.GetAttribute("moduleCategoryId") != null) {
                                    HtModule.Add(Int64.Parse(Reader.GetAttribute("privilegecode")), new Module(Int64.Parse(Reader.GetAttribute("privilegecode")), Int64.Parse(Reader.GetAttribute("traductioncode")), Int64.Parse(Reader.GetAttribute("descriptionWebTextId")), Int64.Parse(Reader.GetAttribute("moduleCategoryId")), Reader.GetAttribute("urlNextPage"), moduleType, Reader.GetAttribute("descriptionImageName")));
									module = Int64.Parse(Reader.GetAttribute("privilegecode"));
								}
								break;
							case "selection":
								if(Reader.GetAttribute("allowRecall")!=null)allowRecall=Boolean.Parse(Reader.GetAttribute("allowRecall"));
								if(Reader.GetAttribute("validationMethod")!=null)methodValue=Reader.GetAttribute("validationMethod");
								if(Reader.GetAttribute("helpUrl")!=null)helpUrlValue=Reader.GetAttribute("helpUrl");
                                if (Reader.GetAttribute("functionName") != null)functionName=Reader.GetAttribute("functionName");
								if(Reader.GetAttribute("showLink")==null)showLink=true;
								else{
									if(Reader.GetAttribute("showLink")=="true")showLink=true;
									else showLink=false;
								}
								if(Reader.GetAttribute("id")!=null && Reader.GetAttribute("traductioncode")!=null && Reader.GetAttribute("url")!=null && Reader.GetAttribute("icone")!=null && Reader.GetAttribute("menuTextId")!=null && module!=0 ){
									((Module)HtModule[module]).SelectionsPages.Add(currentSelectionPage = new SelectionPageInformation(int.Parse(Reader.GetAttribute("id")),showLink,Reader.GetAttribute("url"),Int64.Parse(Reader.GetAttribute("traductioncode")),Reader.GetAttribute("icone"),helpUrlValue, methodValue,Int64.Parse(Reader.GetAttribute("menuTextId")),allowRecall,functionName));
								}
								break;
							case "type":
								if (Reader.GetAttribute("id")!=null)
									currentSelectionPage.LoadableUnivers.Add(int.Parse(Reader.GetAttribute("id")));
								break;
							case "subSelection":
								if(Reader.GetAttribute("allowRecall")!=null)allowRecall=Boolean.Parse(Reader.GetAttribute("allowRecall"));
								if(Reader.GetAttribute("validationMethod")!=null)methodValue=Reader.GetAttribute("validationMethod");
								if(Reader.GetAttribute("helpUrl")!=null)helpUrlValue=Reader.GetAttribute("helpUrl");								
								if(Reader.GetAttribute("showLink")==null)showLink=true;
								else {
									if(Reader.GetAttribute("showLink")=="true")showLink=true;
									else showLink=false;
								}
								if(Reader.GetAttribute("id")!=null && Reader.GetAttribute("traductioncode")!=null && Reader.GetAttribute("url")!=null && Reader.GetAttribute("icone")!=null && Reader.GetAttribute("menuTextId")!=null && module!=0 ) {
									((SelectionPageInformation)currentSelectionPage).HtSubSelectionPageInformation.Add(int.Parse(Reader.GetAttribute("id")),currentSubSelectionPage = new SubSelectionPageInformation(int.Parse(Reader.GetAttribute("id")),showLink,Reader.GetAttribute("url"),Int64.Parse(Reader.GetAttribute("traductioncode")),Reader.GetAttribute("icone"),helpUrlValue, methodValue,Int64.Parse(Reader.GetAttribute("menuTextId")),allowRecall));
								}
								break;
							case "subType":
								if (Reader.GetAttribute("id")!=null)
									currentSubSelectionPage.LoadableUnivers.Add(int.Parse(Reader.GetAttribute("id")));
								break;
							case "optionalSelection":
								if(Reader.GetAttribute("helpUrl")!=null)helpUrlValue=Reader.GetAttribute("helpUrl");
								if(Reader.GetAttribute("optionalNextUrl")!=null)optionalNextUrlValue=Reader.GetAttribute("optionalNextUrl");
								if(Reader.GetAttribute("showLink")==null)showLink=true;
								else{
									if(Reader.GetAttribute("showLink")=="true")showLink=true;
									else showLink=false;
								}
								if(Reader.GetAttribute("id")!=null && Reader.GetAttribute("traductioncode")!=null && Reader.GetAttribute("url")!=null && Reader.GetAttribute("icone")!=null && Reader.GetAttribute("menuTextId")!=null && module!=0){
									((Module)HtModule[module]).OptionalsPages.Add(currentSelectionPage = new OptionalPageInformation(int.Parse(Reader.GetAttribute("id")), showLink, Reader.GetAttribute("url"),Int64.Parse(Reader.GetAttribute("traductioncode")),Reader.GetAttribute("icone"),Reader.GetAttribute("helpUrl"),Int64.Parse(Reader.GetAttribute("menuTextId")),optionalNextUrlValue));
								}
								break;
							case "page":
								currentResultPageInformation=null;
								if(Reader.GetAttribute("rawExcelUrl")!=null) rawExcelUrlValue=Reader.GetAttribute("rawExcelUrl");
								if(Reader.GetAttribute("printExcelUrl")!=null) printExcelUrlValue=Reader.GetAttribute("printExcelUrl");
								if(Reader.GetAttribute("printBisExcelUrl")!=null) printBisExcelUrlValue=Reader.GetAttribute("printBisExcelUrl");
								if(Reader.GetAttribute("exportJpegUrl")!=null) exportJpegUrlValue=Reader.GetAttribute("exportJpegUrl");
								if(Reader.GetAttribute("remotePdfUrl")!=null) remotePdfUrlValue=Reader.GetAttribute("remotePdfUrl");
								if(Reader.GetAttribute("remoteResultPdfUrl")!=null) remoteResultPdfUrlValue=Reader.GetAttribute("remoteResultPdfUrl");
								if(Reader.GetAttribute("remoteTextUrl")!=null) remoteTextUrlValue=Reader.GetAttribute("remoteTextUrl");
								if(Reader.GetAttribute("remoteExcelUrl")!=null) remoteExcelUrlValue=Reader.GetAttribute("remoteExcelUrl");
								if(Reader.GetAttribute("valueExcelUrl")!=null) valueExcelUrlValue=Reader.GetAttribute("valueExcelUrl");
								if(Reader.GetAttribute("helpUrl")!=null)helpUrlValue=Reader.GetAttribute("helpUrl");

								if(Reader.GetAttribute("id")!=null && Reader.GetAttribute("resultid")!=null && Reader.GetAttribute("traductioncode")!=null && Reader.GetAttribute("url")!=null && Reader.GetAttribute("menuTextId")!=null && module!=0){
									currentResultPageInformation=new ResultPageInformation(int.Parse(Reader.GetAttribute("id")),Int64.Parse(Reader.GetAttribute("resultid")), Reader.GetAttribute("url"),Int64.Parse(Reader.GetAttribute("traductioncode")),rawExcelUrlValue,printExcelUrlValue,printBisExcelUrlValue,exportJpegUrlValue,remotePdfUrlValue,remoteResultPdfUrlValue,valueExcelUrlValue,remoteTextUrlValue,remoteExcelUrlValue,Reader.GetAttribute("helpUrl"),Int64.Parse(Reader.GetAttribute("menuTextId")));
                                    currentResultPageInformation.ParentModule=(Module)HtModule[module];
									((Module)HtModule[module]).AddResultPageInformation(currentResultPageInformation);
								}
								break;
							case "detailSelection":
								if (Reader.GetAttribute("id")!=null)
									currentResultPageInformation.DetailSelectionItemsType.Add(int.Parse(Reader.GetAttribute("id")));
								break;
                            case "allowedUnit":
                                allowedUnitValue = Reader.ReadString();
                                if (allowedUnitValue != null && allowedUnitValue.Length > 0 && module != 0 && currentResultPageInformation!=null)
                                    currentResultPageInformation.AllowedUnitEnumList.Add((Constante.CustomerSessions.Unit)Enum.Parse(typeof(Constante.CustomerSessions.Unit), allowedUnitValue, true));
                                break;
                            case "link":
								if(Reader.GetAttribute("privilegecode")!=null && module!=0){
									((Module)HtModule[module]).Bridges.Add(Int64.Parse(Reader.GetAttribute("privilegecode")));
								}
								break;
							case "defaultMediaDetailLevel":
								if(Reader.GetAttribute("id")!=null && module!=0){
									((Module)HtModule[module]).DefaultMediaDetailLevels.Add(DetailLevelsInformation.Get(int.Parse(Reader.GetAttribute("id"))));
								}
								break;
							case "defaultProductDetailLevel":
								if(Reader.GetAttribute("id")!=null && module!=0){
									((Module)HtModule[module]).DefaultProductDetailLevels.Add(DetailLevelsInformation.Get(int.Parse(Reader.GetAttribute("id"))));
								}
								break;
							case "allowedMediaLevelItem":
								if(Reader.GetAttribute("id")!=null && module!=0){
									((Module)HtModule[module]).AllowedMediaDetailLevelItems.Add(DetailLevelItemsInformation.Get(int.Parse(Reader.GetAttribute("id"))));
								}
								break;
							case "allowedProductLevelItem":
								if(Reader.GetAttribute("id")!=null && module!=0){
									((Module)HtModule[module]).AllowedProductDetailLevelItems.Add(DetailLevelItemsInformation.Get(int.Parse(Reader.GetAttribute("id"))));
								}
								break;
                            case "allowedColumnDetailLevelItem":
                                if (Reader.GetAttribute("id") != null && module != 0) {
                                    ((Module)HtModule[module]).AllowedColumnDetailLevelItems.Add(DetailLevelItemsInformation.Get(int.Parse(Reader.GetAttribute("id"))));
                                }
                                break;
							case "allowedUniverseLevel":
								if (Reader.GetAttribute("id") != null) {
									currentSelectionPage.AllowedLevelsIds.Add(int.Parse(Reader.GetAttribute("id")));
								}
								break;
							case "allowedUniverseBranch":
								if (Reader.GetAttribute("id") != null) {
									currentSelectionPage.AllowedBranchesIds.Add(int.Parse(Reader.GetAttribute("id")));
								}
								break;
                            case "RulesLayer":
                                if(Reader.GetAttribute("name")!=null&&Reader.GetAttribute("assemblyName")!=null&&Reader.GetAttribute("class")!=null&&
                                    Reader.GetAttribute("name").Length>0&&Reader.GetAttribute("assemblyName").Length>0&&Reader.GetAttribute("class").Length>0) {
                                   ((Module)HtModule[module]).CountryRulesLayer=new RulesLayer(Reader.GetAttribute("name"),Reader.GetAttribute("assemblyName"),Reader.GetAttribute("class"));
                                }
                                break;
                            case "DataAccessLayer":
                                if(Reader.GetAttribute("name")!=null&&Reader.GetAttribute("assemblyName")!=null&&Reader.GetAttribute("class")!=null&&
                                    Reader.GetAttribute("name").Length>0&&Reader.GetAttribute("assemblyName").Length>0&&Reader.GetAttribute("class").Length>0) {
                                    ((Module)HtModule[module]).CountryDataAccessLayer=new DataAccessLayer(Reader.GetAttribute("name"),Reader.GetAttribute("assemblyName"),Reader.GetAttribute("class"));
                                }
                                break;
                            case "ModuleAllowedMediaUniverse":
                                useBaalForModule=false;
                                if(Reader.GetAttribute("initFromBaal")!=null && Reader.GetAttribute("initFromBaal").Length>0){
                                    // Baal list must be use
                                    if(Boolean.Parse(Reader.GetAttribute("initFromBaal"))){
                                        if(Reader.GetAttribute("baalId")==null || Reader.GetAttribute("baalId").Length==0)
                                            throw(new ArgumentNullException("baalId must be declared if initFromBaal is true"));
                                        ((Module)HtModule[module]).AllowedMediaUniverse=Media.GetItemsList(int.Parse(Reader.GetAttribute("baalId")));
                                        useBaalForModule=true;
                                    }
                                }
                                break;
                            case "ModuleVehicles":
                                if(!useBaalForModule && Reader.GetAttribute("list")!=null && Reader.GetAttribute("list").Length>0) {
                                    if(((Module)HtModule[module]).IsNullAllowedMediaUniverse)((Module)HtModule[module]).InitAllowedMediaUniverse();
                                    ((Module)HtModule[module]).AllowedMediaUniverse.VehicleList=Reader.GetAttribute("list");
                                }
                                break;
                            case "ModuleCategories":
                                if(!useBaalForModule && Reader.GetAttribute("list")!=null && Reader.GetAttribute("list").Length>0) {
                                    if(((Module)HtModule[module]).IsNullAllowedMediaUniverse) ((Module)HtModule[module]).InitAllowedMediaUniverse();
                                    ((Module)HtModule[module]).AllowedMediaUniverse.CategoryList=Reader.GetAttribute("list");
                                }
                                break;
                            case "ModuleMedias":
                                if(!useBaalForModule && Reader.GetAttribute("list")!=null && Reader.GetAttribute("list").Length>0) {
                                    if(((Module)HtModule[module]).IsNullAllowedMediaUniverse) ((Module)HtModule[module]).InitAllowedMediaUniverse();
                                    ((Module)HtModule[module]).AllowedMediaUniverse.MediaList=Reader.GetAttribute("list");
                                }
                                break;
                            case "ResultAllowedMediaUniverse":
                                useBaalForResult=false;
                                if(Reader.GetAttribute("initFromBaal")!=null && Reader.GetAttribute("initFromBaal").Length>0) {
                                    // Baal list must be use
                                    if(Boolean.Parse(Reader.GetAttribute("initFromBaal"))) {
                                        if(Reader.GetAttribute("baalId")==null || Reader.GetAttribute("baalId").Length==0)
                                            throw (new ArgumentNullException("baalId must be declared if initFromBaal is true"));
                                        currentResultPageInformation.AllowedMediaUniverse=Media.GetItemsList(int.Parse(Reader.GetAttribute("baalId")));
                                        useBaalForResult=true;
                                    }
                                }
                                break;
                            case "ResultVehicles":
                                if(!useBaalForResult && Reader.GetAttribute("list")!=null && Reader.GetAttribute("list").Length>0) {
                                    if(currentResultPageInformation.IsNullAllowedMediaUniverse) currentResultPageInformation.InitAllowedMediaUniverse();
                                    currentResultPageInformation.AllowedMediaUniverse.VehicleList=Reader.GetAttribute("list");
                                }
                                break;
                            case "ResultCategories":
                                if(!useBaalForResult && Reader.GetAttribute("list")!=null && Reader.GetAttribute("list").Length>0) {
                                    if(currentResultPageInformation.IsNullAllowedMediaUniverse) currentResultPageInformation.InitAllowedMediaUniverse();
                                    currentResultPageInformation.AllowedMediaUniverse.CategoryList=Reader.GetAttribute("list");
                                }
                                break;
                            case "ResultMedias":
                                if(!useBaalForResult && Reader.GetAttribute("list")!=null && Reader.GetAttribute("list").Length>0) {
                                    if(currentResultPageInformation.IsNullAllowedMediaUniverse) currentResultPageInformation.InitAllowedMediaUniverse();
                                    currentResultPageInformation.AllowedMediaUniverse.MediaList=Reader.GetAttribute("list");
                                }
                                break;
						}					
					}				
				}
                List<Constantes.Web.CustomerSessions.Unit> totalUnit = new List<Constantes.Web.CustomerSessions.Unit>();
                foreach(KeyValuePair<Constantes.Web.CustomerSessions.Unit,Domain.Units.UnitInformation> kvp in Units.UnitsInformation.List)
                    totalUnit.Add(kvp.Key);

                foreach (Module curentModule in HtModule.Values) {
                    ArrayList tab = curentModule.GetResultPageInformationsList();
                    for (int i = 0; i < tab.Count; i++) {
                        ResultPageInformation currentPage = ((ResultPageInformation)tab[i]);
                        if (currentPage.AllowedUnitEnumList.Count == 0)
                            currentPage.AllowedUnitEnumList = totalUnit;
                    }
                    
                }
			}
			catch(System.Exception e){
				throw(new ModuleException("Erreur : ",e)); 
			}
			finally{
				#region Close the file
				if(Reader!=null)Reader.Close();
				#endregion
			}
		}
		#endregion

	}
}
