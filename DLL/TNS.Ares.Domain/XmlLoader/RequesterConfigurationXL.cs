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
    public class RequesterConfigurationXL
    {
        /// <summary>
        /// Load the configuration from an XmlTextReader
        /// </summary>
        /// <param name="nyxConfiguration">Configuration source</param>
        /// <param name="defaultFilePath">Out parameter where default file path should be stored</param>
        /// <param name="defaultLongevity">Out parameter where the default longevity should be stored</param>
        /// <param name="dataAccessLayers">DataAccessLayer dictionary</param>
        /// <param name="plugins">Plugin dictionary</param>
        public static Dictionary<LsClientName, RequesterConfiguration> Load(IDataSource source) {
            // Default values
            Dictionary<LsClientName, RequesterConfiguration> aresConfigurationList = new Dictionary<LsClientName, RequesterConfiguration>();

            // Checking if configuration source is defined
            try {
                source.Open();
                XmlTextReader reader = (XmlTextReader)source.GetSource();
                XmlReader subReader = null;
                List<ModuleDescription> moduleDescriptionList = null;
                LsClientName requesterName;
                Int32 familyId;
                Int32 monitorPort;
                string productName;
                string directoryName;
                Int32 maxAvailableSlots;

                // Reading configuration
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {
                        switch (reader.LocalName) {
                            case "lsClientConfiguration":

                                requesterName = (LsClientName)Enum.Parse(typeof(LsClientName), reader.GetAttribute("name"));
                                familyId = Int32.Parse(reader.GetAttribute("familyId"));
                                monitorPort = Int32.Parse(reader.GetAttribute("monitorPort"));
                                productName = reader.GetAttribute("productName");
                                directoryName = reader.GetAttribute("directoryName");
                                if (reader.GetAttribute("maxAvailableSlots") != null && reader.GetAttribute("maxAvailableSlots").ToString().Length > 0)
                                    maxAvailableSlots = Int32.Parse(reader.GetAttribute("maxAvailableSlots"));
                                else
                                    maxAvailableSlots = 1;

                                moduleDescriptionList = new List<ModuleDescription>();
                                subReader = reader.ReadSubtree();
                                while (subReader.Read()) {
                                    if (subReader.NodeType == XmlNodeType.Element) {
                                        switch (subReader.LocalName) {
                                            case "moduleDescription":
                                                moduleDescriptionList.Add(new ModuleDescription(subReader.GetAttribute("description"), Int32.Parse(subReader.GetAttribute("moduleId"))));
                                                break;
                                        }
                                    }
                                }

                                aresConfigurationList.Add(requesterName, new RequesterConfiguration(familyId, monitorPort, productName, directoryName, requesterName, moduleDescriptionList, maxAvailableSlots));
                                break;
                        }
                    }
                }
            }
            catch(System.Exception err) {

                #region Close the file
                if(source.GetSource()!=null) source.Close();
                #endregion

                throw (new Exception(" Error : ",err));
            }

            source.Close();
            return (aresConfigurationList);
        }
    }
}
