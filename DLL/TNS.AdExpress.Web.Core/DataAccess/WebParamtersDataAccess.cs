#region Information
// Author : G. Facon
// Creation Date: 28/02/2008
// Modifications :
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Web.Core.DataAccess {
    /// <summary>
    /// Load data from Paramters.xml
    /// </summary>
    public class WebParamtersDataAccess {

        #region Site Name
        /// <summary>
        /// Load Site Name
        /// </summary>
        /// <param name="source">Data Source</param>
        /// <returns>Site Name</returns>
        public static string LoadSiteName(IDataSource source) {

            XmlTextReader reader=null;
            string countryName="";
            try {
                reader=(XmlTextReader)source.GetSource();
                while(reader.Read()) {
                    if(reader.NodeType==XmlNodeType.Element) {
                        switch(reader.LocalName) {
                            case "configuration":
                                if(reader.GetAttribute("countryName")!=null && reader.GetAttribute("directoryName").Length>0 && reader.GetAttribute("countryName")!=null && reader.GetAttribute("countryName").Length>0) {
                                    countryName=reader.GetAttribute("countryName");
                                }
                                break;
                        }
                    }
                }

                #region Fermeture du fichier
                if(source.GetSource()!=null) source.Close();
                #endregion

            }
            #region Traitement des erreurs
            catch(System.Exception e) {

                #region Fermeture du fichier
                if(source.GetSource()!=null) source.Close();
                #endregion

                throw (new XmlException("Impossible load directory name",e));
            }
            #endregion

            return (countryName);
        }
        #endregion

        #region Directory name
        /// <summary>
        /// Load Directory Name
        /// </summary>
        /// <param name="source">Data Source</param>
        /// <returns>Directory Name</returns>
        public static string LoadDirectoryName(IDataSource source) {

            XmlTextReader reader=null;
            string directoryName="";
            try {
                reader=(XmlTextReader)source.GetSource();
                while(reader.Read()) {
                    if(reader.NodeType==XmlNodeType.Element) {
                        switch(reader.LocalName) {
                            case "configuration":
                                if(reader.GetAttribute("directoryName")!=null && reader.GetAttribute("directoryName").Length>0 && reader.GetAttribute("countryName")!=null && reader.GetAttribute("countryName").Length>0) {
                                    directoryName=reader.GetAttribute("directoryName");
                                }
                                break;
                        }
                    }
                }

                #region Fermeture du fichier
                if(source.GetSource()!=null) source.Close();
                #endregion

            }
            #region Traitement des erreurs
            catch(System.Exception e) {

                #region Fermeture du fichier
                if(source.GetSource()!=null) source.Close();
                #endregion

                throw (new XmlException("Impossible load directory name",e));
            }
            #endregion

            return (directoryName);
        }
        #endregion
    }
}
