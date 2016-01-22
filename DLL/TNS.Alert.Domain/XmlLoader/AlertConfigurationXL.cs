using System;
using System.Collections.Generic;
using System.Text;

/* TNS */
using TNS.Ares.Domain.LS;
using System.Xml;
using TNS.Ares.Domain.Layers;
using TNS.FrameWork.DB.Common;
using TNS.Ares.Domain.DataBaseDescription;

namespace TNS.Alert.Domain.XmlLoader
{
    public class AlertConfigurationXL
    {
        /// <summary>
        /// Load the configuration from an XmlTextReader
        /// </summary>
        /// <param name="dataSource">dataSource</param>
        /// <param name="occurrenceInformation">occurrenceInformation</param>
        /// <param name="mailInformation">mailInformation</param>
        /// <param name="isActivated">isActivated</param>
        public static void Load(IDataSource dataSource, 
            out AlertInformation alertInformation,
            out OccurrenceInformation occurrenceInformation, 
            out MailInformation mailInformation, 
            out bool isActivated) {
            alertInformation = null;
            occurrenceInformation = null;
            mailInformation = null;
            isActivated = false;
            // Checking if configuration source is defined
            try {
                dataSource.Open();
                XmlTextReader reader = (XmlTextReader)dataSource.GetSource();
                XmlReader subReader = null;
                // Reading configuration
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {
                        switch (reader.LocalName) {
                            case "pluginAlerteConfiguration":
                                if (reader.GetAttribute("isAlertsActivated") != null)
                                    bool.TryParse(reader.GetAttribute("isAlertsActivated"), out isActivated);

                                subReader = reader.ReadSubtree();
                                while (subReader.Read()) {
                                    if (subReader.NodeType == XmlNodeType.Element) {
                                        switch (subReader.LocalName) {
                                            case "alert":
                                                alertInformation = new AlertInformation(bool.Parse(reader.GetAttribute("delete")));
                                                break;
                                            case "occurrence":
                                                occurrenceInformation = new OccurrenceInformation(Int32.Parse(reader.GetAttribute("dayExpiration")), bool.Parse(reader.GetAttribute("delete")));
                                                break;
                                            case "mailConfiguration":
                                                mailInformation = new MailInformation(reader.GetAttribute("targetHost"));
                                                break;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            catch(System.Exception err) {

                #region Close the file
                if (dataSource.GetSource() != null) dataSource.Close();
                #endregion

                throw (new Exception(" Error : ",err));
            }

            dataSource.Close();
        }
    }
}
