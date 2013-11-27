using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMI.AdExpress.Aphrodite.Domain.Layers;
using TNS.FrameWork.DB.Common;
using System.Xml;

namespace KMI.AdExpress.Aphrodite.Domain.XmlLoader {
    /// <summary>
    /// Load Data Access Layer Information
    /// </summary>
    public class DataAccessLayerXL {

        #region Load Data Access Layer Information
        /// <summary>
        /// Load Data Access Layer Information
        /// </summary>
        /// <param name="source">DataSource</param>
        public static DataAccessLayer Load(IDataSource source) {

            DataAccessLayer dal = null;

            XmlTextReader reader = (XmlTextReader)source.GetSource();
            try {
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {
                        switch (reader.LocalName) {
                            case "dataAccessLayer":
                                if (reader.GetAttribute("name") != null && reader.GetAttribute("assemblyName") != null && reader.GetAttribute("class") != null &&
                                    reader.GetAttribute("name").Length > 0 && reader.GetAttribute("assemblyName").Length > 0 && reader.GetAttribute("class").Length > 0) {
                                        dal = new DataAccessLayer(reader.GetAttribute("name"), reader.GetAttribute("assemblyName"), reader.GetAttribute("class"));
                                }
                                break;
                        }
                    }
                }
            }
            catch (System.Exception et) {
                throw (new XmlException("Impossible to load vehicle description file" + et.Message));
            }

            source.Close();
            return (dal);
        }
        #endregion

    }
}
