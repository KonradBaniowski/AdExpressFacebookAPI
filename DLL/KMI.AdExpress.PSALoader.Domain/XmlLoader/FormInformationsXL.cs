using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.FrameWork.DB.Common;
using System.Xml;
using TNS.FrameWork.Exceptions;
using System.Xml.Linq;

namespace KMI.AdExpress.PSALoader.Domain.XmlLoader {
    /// <summary>
    /// This class is used to load an XML file containing information concerning forms
    /// </summary>
    public class FormInformationsXL {

        #region Load Form Information List from XML
        /// <summary>
        /// Load Form Information List from XML
        /// </summary>
        /// <param name="source">DataSource</param>
        public static FormInformations Load(IDataSource source) {

            #region Variables
            FormInformation formInformation = null;
            List<FormInformation> formInformationList = new List<FormInformation>();
            string loadDate = string.Empty;
            Int64 formId = -1;
            Int64 sloganId = -1;
            string dateMediaNum = string.Empty;
            Constantes.Vehicles.names vehicle = Constantes.Vehicles.names.DEFAULT;
            string extension = string.Empty;
            string script = string.Empty;
            bool needToRead = true;
            #endregion

            XmlTextReader reader = (XmlTextReader)source.GetSource();
            try {
                while (!reader.EOF) {
                    if(needToRead) reader.Read();
                    needToRead = true;
                    if (reader.NodeType == XmlNodeType.Element) {
                        switch (reader.LocalName) {
                            case "DONNEES":
                                if (reader.GetAttribute("Date_export") == null || reader.GetAttribute("Date_export").Length == 0) throw (new InvalidXmlValueException("Invalid Date_export parameter"));
                                loadDate = reader.GetAttribute("Date_export");
                                break;
                            case "FICHE":
                                if (formId != -1) {
                                    formInformation = new FormInformation(formId, sloganId, dateMediaNum, vehicle, extension, script);
                                    formInformationList.Add(formInformation);
                                }
                                
                                formId = -1; sloganId = -1; dateMediaNum = string.Empty; vehicle = Constantes.Vehicles.names.DEFAULT; extension = string.Empty; script = string.Empty;  
                                if (reader.GetAttribute("ID_FORM") == null || reader.GetAttribute("ID_FORM").Length == 0) throw (new InvalidXmlValueException("Invalid ID_FORM parameter"));
                                formId = Int64.Parse(reader.GetAttribute("ID_FORM"));
                                break;
                            case "NO_ACCROCHE":
                                if (!reader.IsEmptyElement) {
                                    sloganId = reader.ReadElementContentAsLong();
                                    needToRead = false;
                                }
                                break;
                            case "DATE_SUPPORT":
                                if (!reader.IsEmptyElement) {
                                    dateMediaNum = reader.ReadElementString();
                                    needToRead = false;
                                }
                                break;
                            case "MEDIA":
                                if (reader.IsEmptyElement) throw (new InvalidXmlValueException("Invalid MEDIA parameter"));
                                string media = reader.ReadElementString().Replace(" ", "_");
                                if (media == Constantes.VehicleMapping.INTERNET_NEW_LABEL)
                                    vehicle = Constantes.Vehicles.names.INTERNET;
                                else
                                    vehicle = (Constantes.Vehicles.names)Enum.Parse(typeof(Constantes.Vehicles.names), media, true);
                                needToRead = false;
                                break;
                            case "EXTENSION":
                                if (reader.IsEmptyElement) throw (new InvalidXmlValueException("Invalid EXTENTION parameter"));
                                if (!reader.IsEmptyElement) {
                                    extension = reader.ReadElementString();
                                    needToRead = false;
                                }
                                break;
                            case "SCRIPT":
                                if (!reader.IsEmptyElement) {
                                    script = reader.ReadElementString();
                                    needToRead = false;
                                }
                                break;
                            
                        }
                    }
                }
                if (formId != -1) {
                    formInformation = new FormInformation(formId, sloganId, dateMediaNum, vehicle, extension, script);
                    formInformationList.Add(formInformation);
                }
            }
            catch (System.Exception et) {
                throw (new XmlException("Impossible to load form informations file" + et.Message));
            }

            source.Close();
            return (new FormInformations(formInformationList, loadDate));
        }
        #endregion

    }
}
