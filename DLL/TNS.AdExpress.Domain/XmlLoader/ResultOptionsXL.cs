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
                                    ControlLayer resultControlLayer = null;
                                    List<ControlLayer> selectionControlLayerList = new List<ControlLayer>();
                                    #endregion

                                    subReader = reader.ReadSubtree();
                                    while (subReader.Read()) {
                                        if (subReader.NodeType == XmlNodeType.Element) {
                                            switch (subReader.LocalName) {
                                                case "result":
                                                    resultControlLayer = new ControlLayer(subReader.GetAttribute("name"), subReader.GetAttribute("assemblyName"), subReader.GetAttribute("class"), (!string.IsNullOrEmpty(subReader.GetAttribute("skinId"))) ? subReader.GetAttribute("skinId") : string.Empty, (!string.IsNullOrEmpty(subReader.GetAttribute("validationMethod"))) ? subReader.GetAttribute("validationMethod") : string.Empty);
                                                    break;
                                                case "selections":
                                                    subSubReader = subReader.ReadSubtree();
                                                    while (subSubReader.Read()) {
                                                        if (subSubReader.NodeType == XmlNodeType.Element) {
                                                            switch (subSubReader.LocalName) {
                                                                case "selection":
                                                                    selectionControlLayerList.Add(new ControlLayer(subSubReader.GetAttribute("name"), subSubReader.GetAttribute("assemblyName"), subSubReader.GetAttribute("class"), (!string.IsNullOrEmpty(subSubReader.GetAttribute("skinId"))) ? subSubReader.GetAttribute("skinId") : string.Empty, (!string.IsNullOrEmpty(subSubReader.GetAttribute("validationMethod"))) ? subSubReader.GetAttribute("validationMethod") : string.Empty));
                                                                    break;
                                                            }
                                                        }
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                    vpConfigurationDetail = new VpConfigurationDetail(resultControlLayer, selectionControlLayerList);
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
