using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Exceptions;

namespace KMI.AdExpress.AdVolumeChecker.Domain.XmlLoader {
    /// <summary>
    /// This class is used to load an XML file containing information concerning data base
    /// </summary>
    public class DataBaseInformationXL {

        #region Load Media Information List from XML
        /// <summary>
        /// Load Media Information List from XML
        /// </summary>
        /// <param name="source">DataSource</param>
        public static string Load(IDataSource source) {

            #region Variables
            string connectionString = string.Empty;
            #endregion

            XmlTextReader reader = (XmlTextReader)source.GetSource();
            try {
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {
                        switch (reader.LocalName) {
                            case "dataBase":
                                if (reader.GetAttribute("connectionString") == null || reader.GetAttribute("connectionString").Length == 0) throw (new InvalidXmlValueException("Invalid connectionString  parameter"));
                                connectionString = reader.GetAttribute("connectionString");
                                break;
                        }
                    }
                }
            }
            catch (System.Exception et) {
                throw (new XmlException("Impossible to load database file" + et.Message));
            }

            source.Close();
            return connectionString;
        }
        #endregion

    }
}
