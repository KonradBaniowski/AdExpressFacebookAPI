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
using TNS.FrameWork.Exceptions;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Constantes.Classification.DB;

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
		public static void Load(IDataSource source,Hashtable htModuleGroup, Hashtable htModule){
			XmlTextReader reader=null;
            XmlReader subtree = null;
            XmlReader subtree2 = null;
			htModule.Clear();
			htModuleGroup.Clear();
			bool showLink;
			int moduleType = 0;
			OptionalPageInformation currentSelectionPage = null, currentSubSelectionPage = null; 
			try{

				reader=(XmlTextReader)source.GetSource();
				bool allowRecall=true;
				string helpUrlValue="";
				string optionalNextUrlValue="";
				string methodValue="";
				string rawExcelUrlValue="";
				string printExcelUrlValue="";
				string printBisExcelUrlValue="";
				string exportJpegUrlValue="";
                string remotePdfUrlValue = "", remoteCreativeExportUrlValue="";
				string remoteResultPdfUrlValue="";
				string remoteTextUrlValue="";
				string remoteExcelUrlValue="";
                string valueExcelUrlValue = "";
                string createAlertUrlValue = "";
                string functionName = "";
                string allowedUnitValue="";
                string allowedCampaignTypeValue = "";
                bool useBaalForModule=false;
                bool useBaalForResult=false;
                bool displayIncompleteDateInCalendar = false;
                Constante.CustomerSessions.Unit defaultUnit;
				Int64 module=0;
                Int32 resultLimitation = 0;
				ResultPageInformation currentResultPageInformation=null;
				while(reader.Read()){
					allowRecall=true;
					methodValue="";
					helpUrlValue="";
					optionalNextUrlValue="";
					rawExcelUrlValue="";
					printExcelUrlValue="";
					printBisExcelUrlValue="";
					exportJpegUrlValue="";
				    remotePdfUrlValue = ""; remoteCreativeExportUrlValue = "";
					remoteResultPdfUrlValue="";
					remoteTextUrlValue="";
					remoteExcelUrlValue="";
					valueExcelUrlValue="";
                    createAlertUrlValue="";
                    functionName="";
                    resultLimitation = 0;
					if(reader.NodeType==XmlNodeType.Element){
						switch(reader.LocalName){
                            case "ResultLimitation":
                                if (reader.GetAttribute("size") != null)
                                {
                                    ((Module)htModule[module]).ResultSize = int.Parse(reader.GetAttribute("size"));
                                }
                                break;
							case "moduleGroup":
								if(reader.GetAttribute("privilegecode")!=null && reader.GetAttribute("traductioncode")!=null && reader.GetAttribute("flashUrl")!=null && reader.GetAttribute("type")!=null) {
									htModuleGroup.Add(Int64.Parse(reader.GetAttribute("privilegecode")),new ModuleGroup(Int64.Parse(reader.GetAttribute("privilegecode")),Int64.Parse(reader.GetAttribute("traductioncode")),reader.GetAttribute("flashUrl"),reader.GetAttribute("missingFlashUrl"),Int64.Parse(reader.GetAttribute("descriptionWebTextId"))));
									moduleType=int.Parse(reader.GetAttribute("type"));
								}
								break;
							case "module":
                                if(reader.GetAttribute("privilegecode") != null && reader.GetAttribute("traductioncode") != null && reader.GetAttribute("urlNextPage") != null && reader.GetAttribute("descriptionWebTextId") != null && reader.GetAttribute("descriptionImageName") != null && reader.GetAttribute("moduleCategoryId") != null) {
                                    if (!string.IsNullOrEmpty(reader.GetAttribute("displayIncompleteDateInCalendar"))) displayIncompleteDateInCalendar = bool.Parse(reader.GetAttribute("displayIncompleteDateInCalendar"));
                                    else displayIncompleteDateInCalendar = false;
                                    htModule.Add(Int64.Parse(reader.GetAttribute("privilegecode")), new Module(Int64.Parse(reader.GetAttribute("privilegecode")), Int64.Parse(reader.GetAttribute("traductioncode")), Int64.Parse(reader.GetAttribute("descriptionWebTextId")), Int64.Parse(reader.GetAttribute("moduleCategoryId")), reader.GetAttribute("urlNextPage"), moduleType, reader.GetAttribute("descriptionImageName"), displayIncompleteDateInCalendar));
									module = Int64.Parse(reader.GetAttribute("privilegecode"));
								}
								break;
							case "selection":
								if(reader.GetAttribute("allowRecall")!=null)allowRecall=Boolean.Parse(reader.GetAttribute("allowRecall"));
								if(reader.GetAttribute("validationMethod")!=null)methodValue=reader.GetAttribute("validationMethod");
								if(reader.GetAttribute("helpUrl")!=null)helpUrlValue=reader.GetAttribute("helpUrl");
                                if (reader.GetAttribute("functionName") != null)functionName=reader.GetAttribute("functionName");
								if(reader.GetAttribute("showLink")==null)showLink=true;
								else{
									if(reader.GetAttribute("showLink")=="true")showLink=true;
									else showLink=false;
								}
								if(reader.GetAttribute("id")!=null && reader.GetAttribute("traductioncode")!=null && reader.GetAttribute("url")!=null && reader.GetAttribute("icone")!=null && reader.GetAttribute("menuTextId")!=null && module!=0 ){
									((Module)htModule[module]).SelectionsPages.Add(currentSelectionPage = new SelectionPageInformation(int.Parse(reader.GetAttribute("id")),showLink,reader.GetAttribute("url"),Int64.Parse(reader.GetAttribute("traductioncode")),reader.GetAttribute("icone"),helpUrlValue, methodValue,Int64.Parse(reader.GetAttribute("menuTextId")),allowRecall,functionName));
								}
								break;
							case "type":
								if (reader.GetAttribute("id")!=null)
									currentSelectionPage.LoadableUnivers.Add(int.Parse(reader.GetAttribute("id")));
								break;
							case "subSelection":
								if(reader.GetAttribute("allowRecall")!=null)allowRecall=Boolean.Parse(reader.GetAttribute("allowRecall"));
								if(reader.GetAttribute("validationMethod")!=null)methodValue=reader.GetAttribute("validationMethod");
								if(reader.GetAttribute("helpUrl")!=null)helpUrlValue=reader.GetAttribute("helpUrl");								
								if(reader.GetAttribute("showLink")==null)showLink=true;
								else {
									if(reader.GetAttribute("showLink")=="true")showLink=true;
									else showLink=false;
								}
								if(reader.GetAttribute("id")!=null && reader.GetAttribute("traductioncode")!=null && reader.GetAttribute("url")!=null && reader.GetAttribute("icone")!=null && reader.GetAttribute("menuTextId")!=null && module!=0 ) {
									((SelectionPageInformation)currentSelectionPage).HtSubSelectionPageInformation.Add(int.Parse(reader.GetAttribute("id")),currentSubSelectionPage = new SubSelectionPageInformation(int.Parse(reader.GetAttribute("id")),showLink,reader.GetAttribute("url"),Int64.Parse(reader.GetAttribute("traductioncode")),reader.GetAttribute("icone"),helpUrlValue, methodValue,Int64.Parse(reader.GetAttribute("menuTextId")),allowRecall));
								}
								break;
							case "subType":
								if (reader.GetAttribute("id")!=null)
									currentSubSelectionPage.LoadableUnivers.Add(int.Parse(reader.GetAttribute("id")));
								break;
							case "optionalSelection":
								if(reader.GetAttribute("helpUrl")!=null)helpUrlValue=reader.GetAttribute("helpUrl");
								if(reader.GetAttribute("optionalNextUrl")!=null)optionalNextUrlValue=reader.GetAttribute("optionalNextUrl");
								if(reader.GetAttribute("showLink")==null)showLink=true;
								else{
									if(reader.GetAttribute("showLink")=="true")showLink=true;
									else showLink=false;
								}
								if(reader.GetAttribute("id")!=null && reader.GetAttribute("traductioncode")!=null && reader.GetAttribute("url")!=null && reader.GetAttribute("icone")!=null && reader.GetAttribute("menuTextId")!=null && module!=0){
									((Module)htModule[module]).OptionalsPages.Add(currentSelectionPage = new OptionalPageInformation(int.Parse(reader.GetAttribute("id")), showLink, reader.GetAttribute("url"),Int64.Parse(reader.GetAttribute("traductioncode")),reader.GetAttribute("icone"),reader.GetAttribute("helpUrl"),Int64.Parse(reader.GetAttribute("menuTextId")),optionalNextUrlValue));
								}
								break;
							case "page":

                                #region Result Page
                                currentResultPageInformation =null;
								if(reader.GetAttribute("rawExcelUrl")!=null) rawExcelUrlValue=reader.GetAttribute("rawExcelUrl");
								if(reader.GetAttribute("printExcelUrl")!=null) printExcelUrlValue=reader.GetAttribute("printExcelUrl");
								if(reader.GetAttribute("printBisExcelUrl")!=null) printBisExcelUrlValue=reader.GetAttribute("printBisExcelUrl");
								if(reader.GetAttribute("exportJpegUrl")!=null) exportJpegUrlValue=reader.GetAttribute("exportJpegUrl");
								if(reader.GetAttribute("remotePdfUrl")!=null) remotePdfUrlValue=reader.GetAttribute("remotePdfUrl");
								if(reader.GetAttribute("remoteResultPdfUrl")!=null) remoteResultPdfUrlValue=reader.GetAttribute("remoteResultPdfUrl");
								if(reader.GetAttribute("remoteTextUrl")!=null) remoteTextUrlValue=reader.GetAttribute("remoteTextUrl");
								if(reader.GetAttribute("remoteExcelUrl")!=null) remoteExcelUrlValue=reader.GetAttribute("remoteExcelUrl");
								if(reader.GetAttribute("valueExcelUrl")!=null) valueExcelUrlValue=reader.GetAttribute("valueExcelUrl");
                                if (reader.GetAttribute("helpUrl") != null) helpUrlValue = reader.GetAttribute("helpUrl");
                                if (reader.GetAttribute("createAlertUrl") != null) createAlertUrlValue = reader.GetAttribute("createAlertUrl");
                                if (reader.GetAttribute("remoteCreativeExportUrl") != null) remoteCreativeExportUrlValue = reader.GetAttribute("remoteCreativeExportUrl");

								if(reader.GetAttribute("id")!=null && reader.GetAttribute("resultid")!=null && reader.GetAttribute("traductioncode")!=null && reader.GetAttribute("url")!=null && reader.GetAttribute("menuTextId")!=null && module!=0){
									currentResultPageInformation=new ResultPageInformation(int.Parse(reader.GetAttribute("id")),Int64.Parse(reader.GetAttribute("resultid")), reader.GetAttribute("url"),Int64.Parse(reader.GetAttribute("traductioncode")),rawExcelUrlValue,printExcelUrlValue,printBisExcelUrlValue,exportJpegUrlValue,remotePdfUrlValue,remoteResultPdfUrlValue,valueExcelUrlValue,remoteTextUrlValue,remoteExcelUrlValue,reader.GetAttribute("helpUrl"),Int64.Parse(reader.GetAttribute("menuTextId")), createAlertUrlValue,remoteCreativeExportUrlValue);
                                    currentResultPageInformation.ParentModule=(Module)htModule[module];
                                    subtree2 = (XmlReader)reader.ReadSubtree();

                                    while (subtree2.Read()) {
                                        if (subtree2.NodeType == XmlNodeType.Element) {
                                            switch (subtree2.LocalName) {
                                                case "detailSelection":
                                                    if (subtree2.GetAttribute("id") != null)
                                                        currentResultPageInformation.DetailSelectionItemsType.Add(int.Parse(subtree2.GetAttribute("id")));
                                                    break;
                                                case "allowedUnits":
                                                    if (subtree2.GetAttribute("forceDisplay") != null && subtree2.GetAttribute("forceDisplay").Length > 0)
                                                        currentResultPageInformation.CanDisplayUnitOption = bool.Parse(reader.GetAttribute("forceDisplay"));
                                                    break;
                                                case "allowedUnit":
                                                    allowedUnitValue = subtree2.ReadString();
                                                    if (allowedUnitValue != null && allowedUnitValue.Length > 0 && module != 0 && currentResultPageInformation != null)
                                                        currentResultPageInformation.AllowedUnitEnumList.Add((Constante.CustomerSessions.Unit)Enum.Parse(typeof(Constante.CustomerSessions.Unit), allowedUnitValue, true));
                                                    break;
                                                case "retailerOption":
                                                    if (subtree2.GetAttribute("use") != null)
                                                        currentResultPageInformation.UseRetailerOption = (bool.Parse(subtree2.GetAttribute("use")));
                                                    break;
                                                case "campaignTypes":
                                                    if (subtree2.GetAttribute("default") != null && subtree2.GetAttribute("default").Length > 0)                                                   
                                                        currentResultPageInformation.DefaultCampaignType = (Constante.CustomerSessions.CampaignType)Enum.Parse(typeof(Constante.CustomerSessions.CampaignType), reader.GetAttribute("default"));
                                                    if (subtree2.GetAttribute("forceDisplay") != null && subtree2.GetAttribute("forceDisplay").Length > 0)
                                                        currentResultPageInformation.CanDisplayCampaignType = bool.Parse(reader.GetAttribute("forceDisplay"));                                                  
                                                    break;
                                                case "campaignType":
                                                    allowedCampaignTypeValue = subtree2.ReadString();
                                                    if (allowedCampaignTypeValue != null && allowedCampaignTypeValue.Length > 0 && module != 0 && currentResultPageInformation != null)
                                                        currentResultPageInformation.AllowedCampaignTypeEnumList.Add((Constante.CustomerSessions.CampaignType)Enum.Parse(typeof(Constante.CustomerSessions.CampaignType), allowedCampaignTypeValue, true));
                                                    break;
                                                case "ResultAllowedMediaUniverse":
                                                    useBaalForResult = false;
                                                    if (subtree2.GetAttribute("initFromBaal") != null && subtree2.GetAttribute("initFromBaal").Length > 0) {
                                                        // Baal list must be use
                                                        if (Boolean.Parse(subtree2.GetAttribute("initFromBaal"))) {
                                                            if (subtree2.GetAttribute("baalId") == null || subtree2.GetAttribute("baalId").Length == 0)
                                                                throw (new ArgumentNullException("baalId must be declared if initFromBaal is true"));
                                                            currentResultPageInformation.AllowedMediaUniverse = Media.GetItemsList(int.Parse(subtree2.GetAttribute("baalId")));
                                                            useBaalForResult = true;
                                                        }
                                                    }
                                                    break;
                                                case "ResultVehicles":
                                                    if (!useBaalForResult && subtree2.GetAttribute("list") != null && subtree2.GetAttribute("list").Length > 0) {
                                                        if (currentResultPageInformation.IsNullAllowedMediaUniverse) currentResultPageInformation.InitAllowedMediaUniverse();
                                                        currentResultPageInformation.AllowedMediaUniverse.VehicleList = subtree2.GetAttribute("list");
                                                    }
                                                    break;
                                                case "ResultCategories":
                                                    if (!useBaalForResult && subtree2.GetAttribute("list") != null && subtree2.GetAttribute("list").Length > 0) {
                                                        if (currentResultPageInformation.IsNullAllowedMediaUniverse) currentResultPageInformation.InitAllowedMediaUniverse();
                                                        currentResultPageInformation.AllowedMediaUniverse.CategoryList = subtree2.GetAttribute("list");
                                                    }
                                                    break;
                                                case "ResultMedias":
                                                    if (!useBaalForResult && subtree2.GetAttribute("list") != null && subtree2.GetAttribute("list").Length > 0) {
                                                        if (currentResultPageInformation.IsNullAllowedMediaUniverse) currentResultPageInformation.InitAllowedMediaUniverse();
                                                        currentResultPageInformation.AllowedMediaUniverse.MediaList = subtree2.GetAttribute("list");
                                                    }
                                                    break;
                                                case "overrideDefaultUnits":
                                                    defaultUnit = Constante.CustomerSessions.Unit.none;
                                                    if (reader.GetAttribute("unit") != null && reader.GetAttribute("unit").Length > 0) {
                                                        if (!Enum.IsDefined(typeof(Constante.CustomerSessions.Unit), reader.GetAttribute("unit"))) throw new InvalidXmlValueException("Default unit '" + reader.GetAttribute("unit") + "' is not defined");
                                                        defaultUnit = (Constante.CustomerSessions.Unit)Enum.Parse(typeof(Constante.CustomerSessions.Unit), reader.GetAttribute("unit"));
                                                    }
                                                    currentResultPageInformation.OverrideDefaultUnits = new DefaultUnitList(GetDefaultUnit(subtree2), defaultUnit);
                                                    break;
                                            }
                                        }
                                    }
									((Module)htModule[module]).AddResultPageInformation(currentResultPageInformation);
								}
								
                                #endregion

                                break;
                            case "overrideDefaultUnits":
                                defaultUnit = Constante.CustomerSessions.Unit.none;
                                if (reader.GetAttribute("unit") != null && reader.GetAttribute("unit").Length > 0) {
                                    if (!Enum.IsDefined(typeof(Constante.CustomerSessions.Unit), reader.GetAttribute("unit"))) throw new InvalidXmlValueException("Default unit '" + reader.GetAttribute("unit") + "' is not defined");
                                    defaultUnit = (Constante.CustomerSessions.Unit)Enum.Parse(typeof(Constante.CustomerSessions.Unit), reader.GetAttribute("unit"));
                                }
                                ((Module)htModule[module]).OverrideDefaultUnits = new DefaultUnitList(GetDefaultUnit(reader),defaultUnit);
                                break;
                            case "retailerOption":
                                if (reader.GetAttribute("use") != null)
                                    ((Module)htModule[module]).UseRetailerOption = (bool.Parse(reader.GetAttribute("use")));
                                break;
                            case "allowedUnit":
                                allowedUnitValue = reader.ReadString();
                                if (allowedUnitValue != null && allowedUnitValue.Length > 0)
                                    ((Module)htModule[module]).AllowedUnitEnumList.Add((Constante.CustomerSessions.Unit)Enum.Parse(typeof(Constante.CustomerSessions.Unit), allowedUnitValue, true));
                                break;
                            case "link":
								if(reader.GetAttribute("privilegecode")!=null && module!=0){
									((Module)htModule[module]).Bridges.Add(Int64.Parse(reader.GetAttribute("privilegecode")));
								}
								break;
							case "defaultMediaDetailLevel":
								if(reader.GetAttribute("id")!=null && module!=0){
									((Module)htModule[module]).DefaultMediaDetailLevels.Add(DetailLevelsInformation.Get(int.Parse(reader.GetAttribute("id"))));
								}
								break;
							case "defaultProductDetailLevel":
								if(reader.GetAttribute("id")!=null && module!=0){
									((Module)htModule[module]).DefaultProductDetailLevels.Add(DetailLevelsInformation.Get(int.Parse(reader.GetAttribute("id"))));
								}
								break;
							case "allowedMediaLevelItem":
								if(reader.GetAttribute("id")!=null && module!=0){
									((Module)htModule[module]).AllowedMediaDetailLevelItems.Add(DetailLevelItemsInformation.Get(int.Parse(reader.GetAttribute("id"))));
								}
								break;
							case "allowedProductLevelItem":
								if(reader.GetAttribute("id")!=null && module!=0){
									((Module)htModule[module]).AllowedProductDetailLevelItems.Add(DetailLevelItemsInformation.Get(int.Parse(reader.GetAttribute("id"))));
								}
								break;
                            case "allowedColumnDetailLevelItem":
                                if (reader.GetAttribute("id") != null && module != 0) {
                                    ((Module)htModule[module]).AllowedColumnDetailLevelItems.Add(DetailLevelItemsInformation.Get(int.Parse(reader.GetAttribute("id"))));
                                }
                                break;  
                            case "allowedUniverseLevels" :
                                if (reader.GetAttribute("levelsIdToReadOnly") != null)
                                {
                                    currentSelectionPage.LevelsIdToReadOnly = reader.GetAttribute("levelsIdToReadOnly");
                                }
                                break;
							case "allowedUniverseLevel":
								if (reader.GetAttribute("id") != null) {
									currentSelectionPage.AllowedLevelsIds.Add(int.Parse(reader.GetAttribute("id")));
								}
								break;
                            case "allowedUniverseBranches":
                                if (reader.GetAttribute("defaultBranchId") != null)
                                {
                                    currentSelectionPage.DefaultBranchId = int.Parse(reader.GetAttribute("defaultBranchId"));
                                }
                                if (reader.GetAttribute("forceBranchId") != null)
                                {
                                    currentSelectionPage.ForceBranchId = int.Parse(reader.GetAttribute("forceBranchId"));
                                }                                
                                break;
							case "allowedUniverseBranch":
								if (reader.GetAttribute("id") != null) {
									currentSelectionPage.AllowedBranchesIds.Add(int.Parse(reader.GetAttribute("id")));
								}
								break;
                            case "RulesLayer":
                                if(reader.GetAttribute("name")!=null&&reader.GetAttribute("assemblyName")!=null&&reader.GetAttribute("class")!=null&&
                                    reader.GetAttribute("name").Length>0&&reader.GetAttribute("assemblyName").Length>0&&reader.GetAttribute("class").Length>0) {
                                   ((Module)htModule[module]).CountryRulesLayer=new RulesLayer(reader.GetAttribute("name"),reader.GetAttribute("assemblyName"),reader.GetAttribute("class"));
                                }
                                break;
                            case "DataAccessLayer":
                                if(reader.GetAttribute("name")!=null&&reader.GetAttribute("assemblyName")!=null&&reader.GetAttribute("class")!=null&&
                                    reader.GetAttribute("name").Length>0&&reader.GetAttribute("assemblyName").Length>0&&reader.GetAttribute("class").Length>0) {
                                    ((Module)htModule[module]).CountryDataAccessLayer=new DataAccessLayer(reader.GetAttribute("name"),reader.GetAttribute("assemblyName"),reader.GetAttribute("class"));
                                }
                                break;
                            case "ModuleAllowedMediaUniverse":
                                useBaalForModule=false;
                                if(reader.GetAttribute("initFromBaal")!=null && reader.GetAttribute("initFromBaal").Length>0){
                                    // Baal list must be use
                                    if(Boolean.Parse(reader.GetAttribute("initFromBaal"))){
                                        if(reader.GetAttribute("baalId")==null || reader.GetAttribute("baalId").Length==0)
                                            throw(new ArgumentNullException("baalId must be declared if initFromBaal is true"));
                                        ((Module)htModule[module]).AllowedMediaUniverse=Media.GetItemsList(int.Parse(reader.GetAttribute("baalId")));
                                        useBaalForModule=true;
                                    }
                                }
                                break;
                            case "ModuleVehicles":
                                if(!useBaalForModule && reader.GetAttribute("list")!=null && reader.GetAttribute("list").Length>0) {
                                    if(((Module)htModule[module]).IsNullAllowedMediaUniverse)((Module)htModule[module]).InitAllowedMediaUniverse();
                                    ((Module)htModule[module]).AllowedMediaUniverse.VehicleList=reader.GetAttribute("list");
                                }
                                break;
                            case "ModuleCategories":
                                if(!useBaalForModule && reader.GetAttribute("list")!=null && reader.GetAttribute("list").Length>0) {
                                    if(((Module)htModule[module]).IsNullAllowedMediaUniverse) ((Module)htModule[module]).InitAllowedMediaUniverse();
                                    ((Module)htModule[module]).AllowedMediaUniverse.CategoryList=reader.GetAttribute("list");
                                }
                                break;
                            case "ModuleMedias":
                                if(!useBaalForModule && reader.GetAttribute("list")!=null && reader.GetAttribute("list").Length>0) {
                                    if(((Module)htModule[module]).IsNullAllowedMediaUniverse) ((Module)htModule[module]).InitAllowedMediaUniverse();
                                    ((Module)htModule[module]).AllowedMediaUniverse.MediaList=reader.GetAttribute("list");
                                }
                                break;
							case "ExcludedVehicles":
								if (reader.GetAttribute("list") != null && reader.GetAttribute("list").Length>0) {
									((Module)htModule[module]).ExcludedVehicles = new List<Int64>(Array.ConvertAll<string, Int64>(reader.GetAttribute("list").Split(','), (Converter<string, long>)delegate(string s) { return Convert.ToInt64(s); })); 
								}
								break;
                            case"ProductRights":
                                if (reader.GetAttribute("branches") != null && reader.GetAttribute("branches").Length > 0)
                                {
                                    ((Module)htModule[module]).ProductRightBranches = reader.GetAttribute("branches").Trim();
                                }
                                break;
						}					
					}				
				}
                List<Constantes.Web.CustomerSessions.Unit> totalUnit = new List<Constantes.Web.CustomerSessions.Unit>();
                foreach(KeyValuePair<Constantes.Web.CustomerSessions.Unit,Domain.Units.UnitInformation> kvp in Units.UnitsInformation.List)
                    totalUnit.Add(kvp.Key);

                foreach (Module curentModule in htModule.Values) {
                    ArrayList tab = curentModule.GetResultPageInformationsList();
                    for (int i = 0; i < tab.Count; i++) {
                        ResultPageInformation currentPage = ((ResultPageInformation)tab[i]);
                        if (currentPage.AllowedUnitEnumList.Count == 0)
                            currentPage.AllowedUnitEnumList = totalUnit;
                    }
                    
                }
			}
			catch(System.Exception e){
                throw (new ModulesListXLException("Erreur : ", e)); 
			}
			finally{
				#region Close the file
				if(reader!=null)reader.Close();
				#endregion
			}
		}
		#endregion

        #region GetDefaultUnit
        /// <summary>
        /// Get Default Unit
        /// </summary>
        /// <param name="reader">reader</param>
        /// <returns>Allow Default Unit List</returns>
        private static Dictionary<Vehicles.names, DefaultUnit> GetDefaultUnit(XmlReader reader) {
            try {
                Dictionary<Vehicles.names, DefaultUnit> allowUnitList = new Dictionary<Vehicles.names, DefaultUnit>();
                DefaultUnit overrideDefaultUnit = null;
                Constante.CustomerSessions.Unit overrideDefaultUnitName;
                XmlReader subtree = reader.ReadSubtree();
                while (subtree.Read()) {
                    if (subtree.NodeType == XmlNodeType.Element) {
                        switch (subtree.LocalName) {
                            case "overrideDefaultUnit":
                                if (reader.GetAttribute("unit") == null && reader.GetAttribute("unit").Length <= 0) {
                                    throw new InvalidXmlValueException("Attribute 'unit' is not defined in overrideDefaultUnit");
                                }
                                else if (reader.GetAttribute("vehicle") == null && reader.GetAttribute("vehicle").Length <= 0) {
                                    throw new InvalidXmlValueException("Attribute 'vehicle' is not defined in overrideDefaultUnit");
                                }
                                else {
                                    if (!Enum.IsDefined(typeof(Constante.CustomerSessions.Unit), reader.GetAttribute("unit"))) {
                                        throw new InvalidXmlValueException("Value '" + reader.GetAttribute("unit") + "' of attribute 'unit' is incorrect in overrideDefaultUnit");
                                    }
                                    if (!Enum.IsDefined(typeof(Vehicles.names), reader.GetAttribute("vehicle"))) {
                                        throw new InvalidXmlValueException("Value '" + reader.GetAttribute("vehicle") + "' of attribute 'vehicle' is incorrect in overrideDefaultUnit");
                                    }
                                    overrideDefaultUnitName = (Constante.CustomerSessions.Unit)Enum.Parse(typeof(Constante.CustomerSessions.Unit), reader.GetAttribute("unit"));
                                    overrideDefaultUnit = new DefaultUnit(overrideDefaultUnitName, (Vehicles.names)Enum.Parse(typeof(Vehicles.names), reader.GetAttribute("vehicle")));

                                    if (allowUnitList.ContainsKey(overrideDefaultUnit.VehicleName))
                                        throw new InvalidXmlValueException("Attribute 'vehicle' is already defined in overrideDefaultUnit");
                                    else
                                        allowUnitList.Add(overrideDefaultUnit.VehicleName, overrideDefaultUnit);
                                }
                                break;
                        }
                    }
                }
                return allowUnitList;
            }
            catch (Exception e) {
                throw new ModulesListXLException("Impossible to Load default Unit", e);
            }
        }
        #endregion

    }
}
