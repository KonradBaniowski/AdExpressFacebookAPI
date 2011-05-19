#region Informations
/*
 * Author: G. Ragneau
 * Creation Date: 09/04/2008
 * Modifications:
 *      Author - Date - Description
 * 
 * 
*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.WebService.Domain.Configuration;

namespace TNS.AdExpress.WebService.Domain.XmlLoader
{
    /// <summary>
    /// Load all the configuration parameter for creatives pathes
    /// </summary>
    public class VehiclesCreativesInformationDataAccess
    {
        /// <summary>
        /// Load creatives pathes config
        /// </summary>
        /// <param name="dataSource">Data Source</param>
        public static Dictionary<Int64, VehicleCreativesInformation> Load(IDataSource dataSource)
        {

            #region Variables
            XmlTextReader reader = null;
            XmlReader subReader = null;
            XmlReader subSubReader = null;
            Dictionary<Int64, VehicleCreativesInformation> vehicleCreativesList = new Dictionary<long, VehicleCreativesInformation>();
            Int64 cVehicleId = 0;
            #endregion

            try {

                reader = (XmlTextReader)dataSource.GetSource();
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {

                        switch (reader.LocalName) {
                            case "vehicleCreativesAccess":
                                subReader = reader.ReadSubtree();
                                while (subReader.Read()) {
                                    if (subReader.NodeType == XmlNodeType.Element) {
                                        switch (subReader.LocalName) {
                                            case "vehicle":
                                                cVehicleId = Int64.Parse(subReader.GetAttribute("dataBaseId"));
                                                subSubReader = subReader.ReadSubtree();
                                                while (subSubReader.Read()) {
                                                    if (subSubReader.NodeType == XmlNodeType.Element) {
                                                        switch (subSubReader.LocalName) {
                                                            case "creativeAccess":
                                                                if (subSubReader.GetAttribute("user") != null && subSubReader.GetAttribute("domain") != null && subSubReader.GetAttribute("password") != null)
                                                                    vehicleCreativesList.Add(cVehicleId, new VehicleCreativesInformation(cVehicleId, new CreativeInformation(subSubReader.GetAttribute("path"), new ImpersonateInformation(subSubReader.GetAttribute("user"), subSubReader.GetAttribute("domain"), subSubReader.GetAttribute("password")))));
                                                                else
                                                                    vehicleCreativesList.Add(cVehicleId, new VehicleCreativesInformation(cVehicleId, new CreativeInformation(subSubReader.GetAttribute("path"))));
                                                                break;
                                                        }
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
                return vehicleCreativesList;
            }
            finally {
                if(dataSource!=null)dataSource.Close();
            }          
        }
    }
}