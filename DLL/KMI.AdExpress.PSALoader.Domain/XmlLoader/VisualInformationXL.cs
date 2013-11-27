using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.FrameWork.DB.Common;
using System.Xml;
using TNS.FrameWork.Exceptions;

namespace KMI.AdExpress.PSALoader.Domain.XmlLoader {
    /// <summary>
    /// This class is used to load an XML file containing information concerning visual path by media
    /// </summary>
    public class VisualInformationXL {
        
        #region Load Visual Information List from XML
        /// <summary>
        /// Load Form Information List from XML
        /// </summary>
        /// <param name="source">DataSource</param>
        public static List<VisualInformation> Load(IDataSource source) {

            #region Variables
            List<VisualInformation> visualInformationList = new List<VisualInformation>();
            Constantes.Vehicles.names media = Constantes.Vehicles.names.DEFAULT;
            string path = string.Empty;
            #endregion

            XmlTextReader reader = (XmlTextReader)source.GetSource();
            try {
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {
                        switch (reader.LocalName) {
                            case "media":
                                if (reader.GetAttribute("name") == null || reader.GetAttribute("name").Length == 0) throw (new InvalidXmlValueException("Invalid media name parameter"));
                                media = (Constantes.Vehicles.names)Enum.Parse(typeof(Constantes.Vehicles.names), reader.GetAttribute("name"), true);
                                if (reader.GetAttribute("path") == null || reader.GetAttribute("path").Length == 0) throw (new InvalidXmlValueException("Invalid visual path parameter"));
                                path = reader.GetAttribute("path");
                                visualInformationList.Add(new VisualInformation(media, path));
                                break;
                        }
                    }
                }
            }
            catch (System.Exception et) {
                throw (new XmlException("Impossible to load visual informations file" + et.Message));
            }

            source.Close();
            return visualInformationList;
        }
        #endregion

    }
}
