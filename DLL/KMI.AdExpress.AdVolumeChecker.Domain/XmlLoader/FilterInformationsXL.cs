using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using TNS.FrameWork.DB.Common;

namespace KMI.AdExpress.AdVolumeChecker.Domain.XmlLoader {
    /// <summary>
    /// This class is used to load an XML file containing information concerning filters
    /// </summary>
    public class FilterInformationsXL {

        #region Load Filter Information List from XML
        /// <summary>
        /// Load Media Information List from XML
        /// </summary>
        /// <param name="source">DataSource</param>
        public static List<FilterInformation> Load(IDataSource source) {

            #region Variables
            List<FilterInformation> filterInformationList = new List<FilterInformation>();
            string field = string.Empty;
            string operator_ = string.Empty;
            string ids = string.Empty;
            #endregion

            XmlTextReader reader = (XmlTextReader)source.GetSource();
            try {
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {
                        switch (reader.LocalName) {
                            case "filter":
                                if (reader.GetAttribute("field") != null && reader.GetAttribute("field").Length != 0)
                                    field = reader.GetAttribute("field");
                                if (reader.GetAttribute("operator") != null && reader.GetAttribute("operator").Length != 0)
                                    operator_ = reader.GetAttribute("operator");
                                if (reader.GetAttribute("ids") != null && reader.GetAttribute("ids").Length != 0)
                                    ids = reader.GetAttribute("ids");
                                filterInformationList.Add(new FilterInformation(field, operator_, ids));
                                break;
                        }
                    }
                }
            }
            catch (System.Exception et) {
                throw (new XmlException("Impossible to load media informations file" + et.Message));
            }

            source.Close();
            return filterInformationList;
        }
        #endregion

    }
}
