#region Informations
// Auteur : B.Masson
// Date de création : 21/04/2006
// Date de modification :
#endregion

using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Xml;

using TNS.FrameWork.DB.Common;
using GebExceptions=TNS.AdExpress.Anubis.Geb.Exceptions;
using GebCommon=TNS.AdExpress.Anubis.Geb.Common;

namespace TNS.AdExpress.Anubis.Geb.DataAccess {
    /// <summary>
    /// Classe pour le chargement de la configuration du plugin Geb
    /// </summary>
    public class GebConfigDataAccess {

        /// <summary>
        /// Méthode pour le chargement des éléments de configuration depuis le fichier XML
        /// </summary>
        /// <param name="source">Source de données</param>
        /// <param name="gebConfig">Objet common GebConfig</param>
        internal static void Load(IDataSource source,GebCommon.GebConfig gebConfig) {

            #region Variables
            XmlTextReader Reader;
            string valueXml="";
            #endregion

            try {
                Reader=(XmlTextReader)source.GetSource();
                // Parse XML file
                while(Reader.Read()) {
                    if(Reader.NodeType==XmlNodeType.Element) {
                        switch(Reader.LocalName) {
                            case "CustomerMail":
                                valueXml=Reader.GetAttribute("server");
                                if(valueXml!=null)
                                    gebConfig.CustomerMailServer = valueXml;
                                valueXml=Reader.GetAttribute("port");
                                if(valueXml!=null)
                                    gebConfig.CustomerMailPort = int.Parse(valueXml);
                                valueXml=Reader.GetAttribute("from");
                                if(valueXml!=null)
                                    gebConfig.CustomerMailFrom =valueXml;
                                break;
                            case "ExcelPath":
                                valueXml=Reader.GetAttribute("path");
                                if(valueXml!=null)
                                    gebConfig.ExcelPath = valueXml;
                                break;
                            case "Baal":
                                valueXml = Reader.GetAttribute("listId");
                                if(valueXml != null)
                                    gebConfig.BaalListId = int.Parse(valueXml);
                                break;
                        }
                    }
                }
            }
            catch(System.Exception err) {
                throw (new GebExceptions.GebConfigDataAccessException("Impossible de charger la configuration du plugin Geb",err));
            }
        }


    }
}
