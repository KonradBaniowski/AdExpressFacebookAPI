using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Exceptions;

namespace KMI.AdExpress.AdVolumeChecker.Domain.XmlLoader {
    /// <summary>
    /// This class is used to load an XML file containing information concerning medias
    /// </summary>
    public class MediaInformationsXL {

        #region Load Media Information List from XML
        /// <summary>
        /// Load Media Information List from XML
        /// </summary>
        /// <param name="source">DataSource</param>
        public static List<MediaInformation> Load(IDataSource source) {

            #region Variables
            List<MediaInformation> mediaInformationList = new List<MediaInformation>();
            Int64 id = -1;
            string label = string.Empty;
            Int64 durationLimit = -1;
            Int64 averageDurationLimit = -1;
            bool encryptedMedia = false;
            string unencryptedSlotsFilePath = string.Empty;
            #endregion

            XmlTextReader reader = (XmlTextReader)source.GetSource();
            try {
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {
                        switch (reader.LocalName) {
                            case "media":
                                encryptedMedia = false;
                                unencryptedSlotsFilePath = string.Empty;
                                if (reader.GetAttribute("id") == null || reader.GetAttribute("id").Length == 0) throw (new InvalidXmlValueException("Invalid media id parameter"));
                                id = Int64.Parse(reader.GetAttribute("id"));
                                if (reader.GetAttribute("name") == null || reader.GetAttribute("name").Length == 0) throw (new InvalidXmlValueException("Invalid media name parameter"));
                                label = reader.GetAttribute("name");
                                if (reader.GetAttribute("durationLimit") == null || reader.GetAttribute("durationLimit").Length == 0) throw (new InvalidXmlValueException("Invalid duration Limit parameter"));
                                durationLimit = Int64.Parse(reader.GetAttribute("durationLimit"));
                                if (reader.GetAttribute("averageDurationLimit") == null || reader.GetAttribute("averageDurationLimit").Length == 0) throw (new InvalidXmlValueException("Invalid average duration Limit parameter"));
                                averageDurationLimit = Int64.Parse(reader.GetAttribute("averageDurationLimit"));
                                if (reader.GetAttribute("encryptedMedia") != null && reader.GetAttribute("encryptedMedia").Length > 0)
                                    encryptedMedia = Convert.ToBoolean(reader.GetAttribute("encryptedMedia"));
                                if (reader.GetAttribute("unencryptedSlotsFilePath") != null && reader.GetAttribute("unencryptedSlotsFilePath").Length > 0)
                                    unencryptedSlotsFilePath = reader.GetAttribute("unencryptedSlotsFilePath");
                                mediaInformationList.Add(new MediaInformation(id, label, durationLimit, averageDurationLimit, encryptedMedia, unencryptedSlotsFilePath));
                                break;
                        }
                    }
                }
            }
            catch (System.Exception et) {
                throw (new XmlException("Impossible to load media informations file" + et.Message));
            }

            source.Close();
            return mediaInformationList;
        }
        #endregion

    }
}
