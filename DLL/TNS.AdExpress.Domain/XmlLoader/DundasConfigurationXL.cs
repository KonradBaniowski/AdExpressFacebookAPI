using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using System.Xml;
using TNS.AdExpress.Domain.Results;

namespace TNS.AdExpress.Domain.XmlLoader {
    /// <summary>
    /// Initialize Dundas Configuration
    /// </summary>
    class DundasConfigurationXL {

        #region Parcours du fichier XML
        /// <summary>
        /// Load Dundas Configuration
        /// </summary>
        /// <param name="source">XML data source</param>
        /// <param name="dundas">Dundas Configuration</param>
        public static void Load(IDataSource source, DundasConfiguration dundas) {
            XmlTextReader Reader;

            try {
                Reader = (XmlTextReader)source.GetSource();

                while (Reader.Read()) {

                    if (Reader.NodeType == XmlNodeType.Element) {
                        switch (Reader.LocalName) {
                            case "VirtualPathFileTemporary":
                                if (Reader.GetAttribute("value") != null)
                                    dundas.InitImageURL(Reader.GetAttribute("value"));
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
