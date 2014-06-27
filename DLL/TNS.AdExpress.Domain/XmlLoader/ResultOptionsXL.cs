#region Informations
// Author: G. Facon
// Creation Date: 21/02/2008
// Modifications: 
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TNS.AdExpress.Domain.Classification;
using TNS.FrameWork.Exceptions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Core;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Domain.XmlLoader {
    /// <summary>
    /// Load all the configuration parameter for theme management
    /// </summary>
    public class ResultOptionsXL{
        /// <summary>
        /// Load result options
        /// </summary>
        /// <param name="dataSource">Data Source</param>
        /// <summary>
        public static void Load(IDataSource dataSource) {

            #region Variables
            XmlTextReader reader=null;
            XmlReader subReader = null;
            XmlReader subSubReader = null;
			Dictionary<CustomerSessions.InsertType, long> insetTypeCollection = new Dictionary<CustomerSessions.InsertType, long>();
			string id;
			long dataBaseId;
            bool useComparativeMediaSchedule = false;
            bool useTendencyComparativeWeekType = false;
            bool useBannersFormatFilter = false;
            bool useRetailer = false;
            Dictionary<TableIds, MatchingTable> matchingTableList = new Dictionary<TableIds, MatchingTable>();
            VpConfigurationDetail vpConfigurationDetail = null;
            RolexConfigurationDetail rolexConfigurationDetail = null;
            List<DateConfiguration> vpDateConfigurationList = null, rolexDateConfigurationList = null;
            TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type defaultVpDateType = CustomerSessions.Period.Type.currentMonth;
            TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type defaultRolexDateType = CustomerSessions.Period.Type.currentMonth;
            DetailLevelItemInformation.Levels defaultPersoLevel = DetailLevelItemInformation.Levels.vpBrand;
            bool useComparativeLostWon = false;
            bool useDiponibilityLostWon = true;
            bool useTypeLostWon = true;
            bool isAllPeriodIsRestrictTo4Month = true;
            bool canSaveLevels = false;
            bool displayNews = false;
            #endregion

            try {
                reader = (XmlTextReader)dataSource.GetSource();
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {

                        switch (reader.LocalName) {
                            case "inset":
                                if (reader.GetAttribute("available") != null && reader.GetAttribute("available").Length > 0) {
                                    WebApplicationParameters.AllowInsetOption = Convert.ToBoolean(reader.GetAttribute("available"));
                                }
                                break;
                            case "insetItem":
                                if (reader.GetAttribute("id") == null || reader.GetAttribute("id").Length == 0) throw (new InvalidXmlValueException("Invalid id parameter"));
                                id = reader.GetAttribute("id");
                                if (reader.GetAttribute("dataBaseId") == null || reader.GetAttribute("dataBaseId").Length == 0) throw (new InvalidXmlValueException("Invalid data base Id parameter")); ;
                                dataBaseId = long.Parse(reader.GetAttribute("dataBaseId"));
                                insetTypeCollection.Add((CustomerSessions.InsertType)Enum.Parse(typeof(CustomerSessions.InsertType), id, true), dataBaseId);
                                break;
                            case "mediaSchedule":
                                useComparativeMediaSchedule = bool.Parse(reader.GetAttribute("useComparative"));
                                break;
                            case "tendency":
                                useTendencyComparativeWeekType = bool.Parse(reader.GetAttribute("useComparativeWeekType"));
                                break;
                            case "bannersFormat":
                                useBannersFormatFilter = bool.Parse(reader.GetAttribute("use"));
                                if (useBannersFormatFilter)
                                {
                                    Dictionary<Int64, VehicleFormatInformation> vehicleFormatInformationList = new Dictionary<Int64, VehicleFormatInformation>(); 
                                    subReader = reader.ReadSubtree();
                                    while (subReader.Read()) {
                                        if (subReader.NodeType == XmlNodeType.Element) {
                                            switch (subReader.LocalName) {
                                                case "allowVehicle":
                                                    string excludedFormats = string.Empty;
                                                    if (subReader.GetAttribute("excludedFormats") != null && subReader.GetAttribute("excludedFormats").Length > 0)
                                                        excludedFormats = reader.GetAttribute("excludedFormats").ToString();
                                                    vehicleFormatInformationList.Add(VehiclesInformation.Get(Int64.Parse(subReader.GetAttribute("id"))).DatabaseId
                                                    , new VehicleFormatInformation(
                                                        VehiclesInformation.Get(Int64.Parse(subReader.GetAttribute("id"))).DatabaseId,
                                                            (Constantes.Customer.RightBanners.Type)Enum.Parse(typeof(Constantes.Customer.RightBanners.Type), subReader.GetAttribute("rightBannersType")),
                                                            (TableIds)Enum.Parse(typeof(TableIds), subReader.GetAttribute("dataTableName")),
                                                            (TableIds)Enum.Parse(typeof(TableIds), subReader.GetAttribute("formatTableName")),
                                                            excludedFormats
                                                        )
                                                    );
                                                    break;
                                            }
                                        }
                                    }
                                    WebApplicationParameters.VehiclesFormatInformation = new VehiclesFormatInformation(useBannersFormatFilter, vehicleFormatInformationList);
                                }
                                break;
                            case "retailer":
                                useRetailer = bool.Parse(reader.GetAttribute("use"));
                                if (useRetailer) {
                                    subReader = reader.ReadSubtree();
                                    while (subReader.Read()) {
                                        if (subReader.NodeType == XmlNodeType.Element) {
                                            switch (subReader.LocalName) {
                                                case "matchingTable":
                                                    matchingTableList.Add((TableIds)Enum.Parse(typeof(TableIds), subReader.GetAttribute("defaultEnumId"))
                                                        , new MatchingTable(
                                                            (TableIds)Enum.Parse(typeof(TableIds), subReader.GetAttribute("defaultEnumId")),
                                                            (TableIds)Enum.Parse(typeof(TableIds), subReader.GetAttribute("retailerEnumId"))
                                                        )
                                                    );
                                                    break;
                                            }
                                        }
                                    }
                                }
                                break;
                            case "vpConfiguration":

                                if (ModulesList.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.VP) != null) {

                                    #region Variables
                                    var resultControlLayerList = new List<ControlLayer>();
                                    var selectionControlLayerList = new List<ControlLayer>();
                                    vpDateConfigurationList = new List<DateConfiguration>();
                                    #endregion

                                    subReader = reader.ReadSubtree();
                                    while (subReader.Read()) {
                                        if (subReader.NodeType == XmlNodeType.Element) {
                                            switch (subReader.LocalName) {
                                                case "dateSelections":                                                  
                                                    defaultVpDateType = GetDateConfigurations(vpDateConfigurationList, subReader);
                                                    break;
                                                case "results":
                                                    GetResultControlLayers(resultControlLayerList, subReader);
                                                    break;
                                                case "selections":
                                                    GetSelectionControlLayers(selectionControlLayerList, subReader);
                                                    break;
                                                case "defaulPersonnalizedLevel":
                                                    defaultPersoLevel = (DetailLevelItemInformation.Levels)Enum.Parse(typeof(DetailLevelItemInformation.Levels), subReader.GetAttribute("level"));
                                                    break;

                                            }
                                        }
                                    }
                                    vpConfigurationDetail = new VpConfigurationDetail(resultControlLayerList, selectionControlLayerList, defaultPersoLevel);
                                }
                                break;
							case "rolexConfiguration":
                                if (ModulesList.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.ROLEX) != null)
                                {   
                                    #region Variables
                                    var resultControlLayerList = new List<ControlLayer>();
                                    var selectionControlLayerList = new List<ControlLayer>();
                                    rolexDateConfigurationList = new List<DateConfiguration>();
                                    #endregion

                                    subReader = reader.ReadSubtree();
                                    while (subReader.Read())
                                    {
                                        if (subReader.NodeType == XmlNodeType.Element)
                                        {
                                            switch (subReader.LocalName)
                                            {
                                                case "dateSelections":
                                                    defaultRolexDateType = GetDateConfigurations(rolexDateConfigurationList, subReader);
                                                    break;
                                                case "results":
                                                    GetResultControlLayers(resultControlLayerList, subReader);
                                                    break;
                                                case "selections":
                                                    GetSelectionControlLayers(selectionControlLayerList, subReader);
                                                    break;                                               
                                            }
                                        }
                                    }
                                    rolexConfigurationDetail = new RolexConfigurationDetail(resultControlLayerList, selectionControlLayerList);
                                }
                                break;
                            case "campaignType":
                                if (reader.GetAttribute("available") != null && reader.GetAttribute("available").Length > 0)
                                {
                                    WebApplicationParameters.AllowCampaignTypeOption = Convert.ToBoolean(reader.GetAttribute("available"));
                                }
                                break;
                            case "refineUniverse":
                                if (reader.GetAttribute("keepSelection") != null && reader.GetAttribute("keepSelection").Length > 0)
                                {
                                    WebApplicationParameters.KeepRefineUniverseSelection = Convert.ToBoolean(reader.GetAttribute("keepSelection"));
                                }
                                break;
                            case "lostWon":
                                useComparativeLostWon = bool.Parse(reader.GetAttribute("useComparative"));
                                useDiponibilityLostWon = bool.Parse(reader.GetAttribute("useDiponibility"));
                                useTypeLostWon = bool.Parse(reader.GetAttribute("useType"));
                                break;
                            case "insertionReport":
                                isAllPeriodIsRestrictTo4Month = bool.Parse(reader.GetAttribute("isAllPeriodIsRestrictTo4Month"));
                                canSaveLevels = bool.Parse(reader.GetAttribute("canSaveLevels"));
                                break;
                            case "news":
                                displayNews = bool.Parse(reader.GetAttribute("displayNews"));
                                break;
                        }
                    }
                }
                WebApplicationParameters.InsetTypeCollection = insetTypeCollection;
                WebApplicationParameters.UseComparativeMediaSchedule = useComparativeMediaSchedule;
                WebApplicationParameters.UseTendencyComparativeWeekType = useTendencyComparativeWeekType;
                WebApplicationParameters.UseRetailer = useRetailer;
                WebApplicationParameters.MatchingRetailerTableList = matchingTableList;
                WebApplicationParameters.VpConfigurationDetail = vpConfigurationDetail;
                WebApplicationParameters.RolexConfigurationDetail = rolexConfigurationDetail;
                WebApplicationParameters.VpDateConfigurations = new DateConfigurations(defaultVpDateType, vpDateConfigurationList);
                WebApplicationParameters.RolexDateConfigurations = new DateConfigurations(defaultRolexDateType, rolexDateConfigurationList);	
                WebApplicationParameters.UseComparativeLostWon = useComparativeLostWon;
                WebApplicationParameters.UseDiponibilityOptionPeriodLostWon = useDiponibilityLostWon;
                WebApplicationParameters.UseTypeOptionPeriodLostWon = useTypeLostWon;
                //WebApplicationParameters.IsAllPeriodIsRestrictTo4MonthInInsertionReport = isAllPeriodIsRestrictTo4Month;
                WebApplicationParameters.InsertionOptions = new InsertionOptions(isAllPeriodIsRestrictTo4Month, canSaveLevels);
                WebApplicationParameters.DisplayNews = displayNews;
            }

            #region Error Management
            catch (System.Exception err) {
                throw (new XmlException(" Error while loading ResultOptions.xml : ", err));
            }
            #endregion

            #region Close the file
            finally {
                if (dataSource != null && dataSource.GetSource() != null)
                    dataSource.Close();
            }
            #endregion

        }

        private static CustomerSessions.Period.Type GetDateConfigurations(List<DateConfiguration> vpDateConfigurationList, XmlReader subReader)
        {
            CustomerSessions.Period.Type defaultVpDateType;
            XmlReader subSubReader;
            defaultVpDateType =
                (CustomerSessions.Period.Type)
                Enum.Parse(typeof (CustomerSessions.Period.Type), subReader.GetAttribute("defaultType"));

            subSubReader = subReader.ReadSubtree();
            while (subSubReader.Read())
            {
                if (subSubReader.NodeType == XmlNodeType.Element)
                {
                    switch (subSubReader.LocalName)
                    {
                        case "dateSelection":
                            vpDateConfigurationList.Add(
                                new DateConfiguration(
                                    (CustomerSessions.Period.Type)
                                    Enum.Parse(typeof (CustomerSessions.Period.Type), subSubReader.GetAttribute("type")),
                                    Int64.Parse(subSubReader.GetAttribute("textId"))));
                            break;
                    }
                }
            }
            return defaultVpDateType;
        }

        private static void GetSelectionControlLayers(List<ControlLayer> selectionControlLayerList, XmlReader subReader)
        {
            XmlReader subSubReader;
            subSubReader = subReader.ReadSubtree();
            while (subSubReader.Read())
            {
                if (subSubReader.NodeType == XmlNodeType.Element)
                {
                    switch (subSubReader.LocalName)
                    {
                        case "selection":
                            selectionControlLayerList.Add(new ControlLayer(subSubReader.GetAttribute("name"),
                                                                           subSubReader.GetAttribute("id"),
                                                                           subSubReader.GetAttribute("assemblyName"),
                                                                           subSubReader.GetAttribute("class"),
                                                                           (!string.IsNullOrEmpty(
                                                                               subSubReader.GetAttribute("skinId")))
                                                                               ? subSubReader.GetAttribute("skinId")
                                                                               : string.Empty,
                                                                           (!string.IsNullOrEmpty(
                                                                               subSubReader.GetAttribute("validationMethod")))
                                                                               ? subSubReader.GetAttribute("validationMethod")
                                                                               : string.Empty,
                                                                           (!string.IsNullOrEmpty(
                                                                               subSubReader.GetAttribute("display")))
                                                                               ? bool.Parse(subSubReader.GetAttribute("display"))
                                                                               : true,
                                                                           (!string.IsNullOrEmpty(
                                                                               subSubReader.GetAttribute("textId")))
                                                                               ? Int64.Parse(subSubReader.GetAttribute("textId"))
                                                                               : 0));
                            break;
                    }
                }
            }
        }

        private static void GetResultControlLayers(List<ControlLayer> resultControlLayerList, XmlReader subReader)
        {
            XmlReader subSubReader = subReader.ReadSubtree();
            while (subSubReader.Read())
            {
                if (subSubReader.NodeType == XmlNodeType.Element)
                {
                    switch (subSubReader.LocalName)
                    {
                        case "result":
                            resultControlLayerList.Add(new ControlLayer(subSubReader.GetAttribute("name"),
                                                                        subSubReader.GetAttribute("id"),
                                                                        subSubReader.GetAttribute("assemblyName"),
                                                                        subSubReader.GetAttribute("class"),
                                                                        (!string.IsNullOrEmpty(
                                                                            subSubReader.GetAttribute("skinId")))
                                                                            ? subSubReader.GetAttribute("skinId")
                                                                            : string.Empty,
                                                                        (!string.IsNullOrEmpty(
                                                                            subSubReader.GetAttribute("validationMethod")))
                                                                            ? subSubReader.GetAttribute("validationMethod")
                                                                            : string.Empty,
                                                                        (!string.IsNullOrEmpty(
                                                                            subSubReader.GetAttribute("display")))
                                                                            ? bool.Parse(subSubReader.GetAttribute("display"))
                                                                            : true,
                                                                        (!string.IsNullOrEmpty(
                                                                            subSubReader.GetAttribute("textId")))
                                                                            ? Int64.Parse(subSubReader.GetAttribute("textId"))
                                                                            : 0));
                            break;
                    }
                }
            }
        }
    }
}
