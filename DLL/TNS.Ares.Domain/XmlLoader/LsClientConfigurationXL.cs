using System;
using System.Collections.Generic;
using System.Text;
using TNS.Ares.Domain.LS;
using TNS.FrameWork.DB.Common;
using TNS.LinkSystem.LinkKernel;
using System.Xml;

namespace TNS.Ares.Domain.XmlLoader {
    /// <summary>
    /// XML loader for the ls client configuration object
    /// </summary>
    public class LsClientConfigurationXL {

        #region Load
        /// <summary>
        /// Load the configuration from an XmlTextReader
        /// </summary>
        /// <param name="source">Data Source</param>
        /// <returns>Link Server Client configuration object</returns>
        public static LsClientConfiguration Load(IDataSource source) {
            
            LsClientConfiguration lsClientConfiguration = null;

            // Checking if configuration source is defined
            try {

                source.Open();
                XmlTextReader reader = (XmlTextReader)source.GetSource();
                XmlReader subReader = null;
                List<ModuleDescription> moduleDescriptionList = null;
                Int32 familyId;
                string familyName;
                Int32 monitorPort;
                string productName;
                string directoryName;
                Int32 maxAvailableSlots;
                bool isSelectable = true;

                // Reading configuration
                while (reader.Read()) {

                    if (reader.NodeType == XmlNodeType.Element) {

                        switch (reader.LocalName) {

                            case "lsClientConfiguration":
                                
                                familyId = Int32.Parse(reader.GetAttribute("familyId"));
                                familyName = reader.GetAttribute("familyName");
                                monitorPort = Int32.Parse(reader.GetAttribute("monitorPort"));
                                productName = reader.GetAttribute("productName");
                                directoryName = reader.GetAttribute("directoryName");
                                moduleDescriptionList = new List<ModuleDescription>();
                                if (reader.GetAttribute("maxAvailableSlots") != null && reader.GetAttribute("maxAvailableSlots").ToString().Length > 0)
                                    maxAvailableSlots = Int32.Parse(reader.GetAttribute("maxAvailableSlots"));
                                else
                                    maxAvailableSlots = 1;
                                
                                subReader = reader.ReadSubtree();

                                while (subReader.Read()) {
                                
                                    if (subReader.NodeType == XmlNodeType.Element) {
                                        switch (subReader.LocalName) {
                                            case "moduleDescription":
                                                if (!bool.TryParse(subReader.GetAttribute("isSelectable"), out isSelectable)) isSelectable = true;
                                                moduleDescriptionList.Add(new ModuleDescription(subReader.GetAttribute("description"), Int32.Parse(subReader.GetAttribute("moduleId")), isSelectable));
                                                break;
                                        }
                                    }
                                }

                                lsClientConfiguration = new LsClientConfiguration(familyId, familyName, monitorPort, productName, directoryName, moduleDescriptionList, maxAvailableSlots);
                                break;
                        }
                    }
                }
            }
            catch (System.Exception err) {

                #region Close the file
                if (source.GetSource() != null) source.Close();
                #endregion

                throw (new Exception(" Error when loadingxml data for the LsClientConfiguration object : ", err));
            }

            source.Close();
            return (lsClientConfiguration);
        }
        #endregion

    }
}
