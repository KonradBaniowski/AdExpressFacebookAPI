#region Informations
// Author: Y. R'kaina
// Creation date: 17/10/2008
// Modification date:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using TNS.AdExpress.Domain.Level;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Domain.XmlLoader {

    /// <summary>
    /// Class used to match for every vehicle a column detail list (Used in the media schedule result and PDF export)
    /// </summary>
    public class MSCreativesDetailsXL {

        #region Parcours du fichier XML
        /// <summary>
        /// Load the XML file
        /// </summary>
        /// <param name="source">Data source</param>
        /// <param name="detailColumnsList">Detail column list</param>
        public static void Load(IDataSource source, Dictionary<Int64, List<GenericColumnItemInformation>> detailColumnsList, Dictionary<Int64, Int64> detailColumnsByVehicle) {
            XmlTextReader Reader;
            Int64 vehicleId = 0;
            Int64 idDetailColumn = 0;

            try {
                Reader = (XmlTextReader)source.GetSource();

                while (Reader.Read()) {

                    if (Reader.NodeType == XmlNodeType.Element) {
                        switch (Reader.LocalName) {
                            case "vehicle":
                                if (Reader.GetAttribute("id") != null)
                                    vehicleId = Int64.Parse(Reader.GetAttribute("id"));
                                break;
                            case "detailColumn":
                                if (Reader.GetAttribute("id") != null) {
                                    idDetailColumn = Int64.Parse(Reader.GetAttribute("id"));
                                    detailColumnsList.Add(vehicleId, WebApplicationParameters.GenericColumnsInformation.GetGenericColumnItemInformationList(idDetailColumn));
                                    detailColumnsByVehicle.Add(vehicleId, idDetailColumn);
                                }
                                break;
                        }
                    }
                }

            }
            catch (System.Exception e) {
                throw (new System.Exception(" Erreur : " + e.Message));
            }

        }
        #endregion

    }
}
