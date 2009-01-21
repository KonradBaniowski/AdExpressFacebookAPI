#region Informations
// Author: Y. R'kaina
// Creation date: 20/08/2007
// Modification date:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;

using TNS.AdExpress.Domain.Level;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Domain.XmlLoader {

    /// <summary>
    /// Cette classe est utilisée pour associer à chaque média un détail colonnes spécifique (utilisé pour la création du détail media dans le module portefeuille) 
    /// </summary>
    class PortofolioDetailMediaColumnsXL
    {

        #region Parcours du fichier XML
        public static void Load(IDataSource source, Dictionary<Int64, List<GenericColumnItemInformation>> defaultMediaDetailColumns) {
            XmlTextReader Reader;
            Int64 vehicleId = 0;
            Int64 idDetailColumn = 0;

            try {
                Reader = (XmlTextReader)source.GetSource();

                while (Reader.Read()) {

                    if (Reader.NodeType == XmlNodeType.Element) {
                        switch (Reader.LocalName) {
                            case "vehicle":
                                if(Reader.GetAttribute("id")!=null )
                                    vehicleId = Int64.Parse(Reader.GetAttribute("id"));
                                break;
                            case "detailColumn":
                                if (Reader.GetAttribute("id") != null) {
                                    idDetailColumn = Int64.Parse(Reader.GetAttribute("id"));
                                    defaultMediaDetailColumns.Add(vehicleId, WebApplicationParameters.GenericColumnsInformation.GetGenericColumnItemInformationList(idDetailColumn));
                                }
                                break;
                        }
                    }
                }

            }
            catch (System.Exception e) {
                throw (new System.Exception(" Erreur : " , e));
            }

        }
        #endregion

    }
}
