using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using TNS.FrameWork.DB.Common;

namespace KMI.PromoPSA.Web.Domain.XmlLoader {
    /// <summary>
    /// Load data from Paramters.xml
    /// </summary>
    public class WebParamtersXL {

        #region Site Name
        /// <summary>
        /// Load Site Name
        /// </summary>
        /// <param name="source">Data Source</param>
        /// <returns>Site Name</returns>
        public static string LoadSiteName(IDataSource source) {

            XmlTextReader reader = null;
            string siteName = "";
            try {
                reader = (XmlTextReader)source.GetSource();
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {
                        switch (reader.LocalName) {
                            case "configuration":
                                if (reader.GetAttribute("siteName") != null && reader.GetAttribute("siteName").Length > 0) {
                                    siteName = reader.GetAttribute("siteName");
                                }
                                break;
                        }
                    }
                }

                #region Fermeture du fichier
                if (source.GetSource() != null) source.Close();
                #endregion

            }
            #region Traitement des erreurs
            catch (System.Exception e) {

                #region Fermeture du fichier
                if (source.GetSource() != null) source.Close();
                #endregion

                throw (new XmlException("Impossible load directory name", e));
            }
            #endregion

            return (siteName);
        }
        #endregion

        #region Country Short Name
        /// <summary>
        /// Load Site Name
        /// </summary>
        /// <param name="source">Data Source</param>
        /// <returns>Site Name</returns>
        public static string LoadCountryShortName(IDataSource source) {

            XmlTextReader reader = null;
            string countryShortName = "";
            try {
                reader = (XmlTextReader)source.GetSource();
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {
                        switch (reader.LocalName) {
                            case "configuration":
                                if (reader.GetAttribute("countryShortName") != null && reader.GetAttribute("countryShortName").Length > 0) {
                                    countryShortName = reader.GetAttribute("countryShortName");
                                }
                                break;
                        }
                    }
                }

                #region Fermeture du fichier
                if (source.GetSource() != null) source.Close();
                #endregion

            }
            #region Traitement des erreurs
            catch (System.Exception e) {

                #region Fermeture du fichier
                if (source.GetSource() != null) source.Close();
                #endregion

                throw (new XmlException("Impossible load directory name", e));
            }
            #endregion

            return (countryShortName);
        }
        #endregion

        #region Directory name
        /// <summary>
        /// Load Directory Name
        /// </summary>
        /// <param name="source">Data Source</param>
        /// <returns>Directory Name</returns>
        public static string LoadDirectoryName(IDataSource source) {

            XmlTextReader reader = null;
            string directoryName = "";
            try {
                reader = (XmlTextReader)source.GetSource();
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {
                        switch (reader.LocalName) {
                            case "configuration":
                                if (reader.GetAttribute("directoryName") != null && reader.GetAttribute("directoryName").Length > 0 && reader.GetAttribute("countryName") != null && reader.GetAttribute("countryName").Length > 0) {
                                    directoryName = reader.GetAttribute("directoryName");
                                }
                                break;
                        }
                    }
                }

                #region Fermeture du fichier
                if (source.GetSource() != null) source.Close();
                #endregion

            }
            #region Traitement des erreurs
            catch (System.Exception e) {

                #region Fermeture du fichier
                if (source.GetSource() != null) source.Close();
                #endregion

                throw (new XmlException("Impossible load directory name", e));
            }
            #endregion

            return (directoryName);
        }
        #endregion
    }
}
