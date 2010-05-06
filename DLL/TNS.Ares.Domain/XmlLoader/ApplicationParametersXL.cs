using System;
using System.Collections.Generic;
using System.Text;

/* TNS */
using TNS.Ares.Domain.LS;
using System.Xml;
using TNS.Ares.Domain.Layers;
using TNS.FrameWork.DB.Common;
using TNS.Ares.Domain.DataBaseDescription;
using TNS.LinkSystem.LinkKernel;

namespace TNS.Ares.Domain.XmlLoader
{
    public class ApplicationParametersXL
    {
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
