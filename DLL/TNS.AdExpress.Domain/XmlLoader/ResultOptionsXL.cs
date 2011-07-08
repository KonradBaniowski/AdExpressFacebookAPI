#region Informations
// Author: G. Facon
// Creation Date: 21/02/2008
// Modifications: 
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
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
            bool useBannersFormatFilter = false;
            bool useRetailer = false;
            Dictionary<TableIds, MatchingTable> matchingTableList = new Dictionary<TableIds, MatchingTable>();
            VpConfigurationDetail vpConfigurationDetail = null;
            List<VpDateConfiguration> vpDateConfigurationList = null;
            TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type defaultVpDateType = CustomerSessions.Period.Type.currentMonth;
            DetailLevelItemInformation.Levels defaultPersoLevel = DetailLevelItemInformation.Levels.vpBrand;
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
                            case "bannersFormat":
                                useBannersFormatFilter = bool.Parse(reader.GetAttribute("useBannersFormat"));
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
                                    List<ControlLayer> resultControlLayerList = new List<ControlLayer>();
                                    List<ControlLayer> selectionControlLayerList = new List<ControlLayer>();
                                    vpDateConfigurationList = new List<VpDateConfiguration>();
                                    #endregion

                                    subReader = reader.ReadSubtree();
                                    while (subReader.Read()) {
                                        if (subReader.NodeType == XmlNodeType.Element) {
                                            switch (subReader.LocalName) {
                                                case "dateSelections":
                                                    defaultVpDateType = (TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type)Enum.Parse(typeof(TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type), subReader.GetAttribute("defaultType"));

                                                    subSubReader = subReader.ReadSubtree();
                                                    while (subSubReader.Read()) {
                                                        if (subSubReader.NodeType == XmlNodeType.Element) {
                                                            switch (subSubReader.LocalName) {
                                                                case "dateSelection":
                                                                    vpDateConfigurationList.Add(new VpDateConfiguration((TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type)Enum.Parse(typeof(TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type), subSubReader.GetAttribute("type")), Int64.Parse(subSubReader.GetAttribute("textId"))));
                                                                    break;
                                                            }
                                                        }
                                                    }
                                                    break;
                                                case "results":
                                                    subSubReader = subReader.ReadSubtree();
                                                    while (subSubReader.Read()) {
                                                        if (subSubReader.NodeType == XmlNodeType.Element) {
                                                            switch (subSubReader.LocalName) {
                                                                case "result":
                                                                    resultControlLayerList.Add(new ControlLayer(subSubReader.GetAttribute("name"), subSubReader.GetAttribute("id"), subSubReader.GetAttribute("assemblyName"), subSubReader.GetAttribute("class"), (!string.IsNullOrEmpty(subSubReader.GetAttribute("skinId"))) ? subSubReader.GetAttribute("skinId") : string.Empty, (!string.IsNullOrEmpty(subSubReader.GetAttribute("validationMethod"))) ? subSubReader.GetAttribute("validationMethod") : string.Empty, (!string.IsNullOrEmpty(subSubReader.GetAttribute("display"))) ? bool.Parse(subSubReader.GetAttribute("display")) : true, (!string.IsNullOrEmpty(subSubReader.GetAttribute("textId"))) ? Int64.Parse(subSubReader.GetAttribute("textId")) : 0));
                                                                    break;
                                                            }
                                                        }
                                                    }
                                                    break;
                                                case "selections":
                                                    subSubReader = subReader.ReadSubtree();
                                                    while (subSubReader.Read()) {
                                                        if (subSubReader.NodeType == XmlNodeType.Element) {
                                                            switch (subSubReader.LocalName) {
                                                                case "selection":
                                                                    selectionControlLayerList.Add(new ControlLayer(subSubReader.GetAttribute("name"), subSubReader.GetAttribute("id"), subSubReader.GetAttribute("assemblyName"), subSubReader.GetAttribute("class"), (!string.IsNullOrEmpty(subSubReader.GetAttribute("skinId"))) ? subSubReader.GetAttribute("skinId") : string.Empty, (!string.IsNullOrEmpty(subSubReader.GetAttribute("validationMethod"))) ? subSubReader.GetAttribute("validationMethod") : string.Empty, (!string.IsNullOrEmpty(subSubReader.GetAttribute("display"))) ? bool.Parse(subSubReader.GetAttribute("display")) : true, (!string.IsNullOrEmpty(subSubReader.GetAttribute("textId"))) ? Int64.Parse(subSubReader.GetAttribute("textId")) : 0));
                                                                    break;
                                                            }
                                                        }
                                                    }
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
                        }
                    }
                }
                WebApplicationParameters.InsetTypeCollection = insetTypeCollection;
                WebApplicationParameters.UseComparativeMediaSchedule = useComparativeMediaSchedule;
                WebApplicationParameters.UseBannersFormatFilter = useBannersFormatFilter;
                WebApplicationParameters.UseRetailer = useRetailer;
                WebApplicationParameters.MatchingRetailerTableList = matchingTableList;
                WebApplicationParameters.VpConfigurationDetail = vpConfigurationDetail;
                WebApplicationParameters.VpDateConfigurations = new VpDateConfigurations(defaultVpDateType, vpDateConfigurationList);
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
    }
}
