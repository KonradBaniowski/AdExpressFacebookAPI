#region Information
//  Author : G. Facon
//  Creation  date: 05/08/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Exceptions;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Domain.XmlLoader {
    /// <summary>
    /// Load Units descriptions from XML files
    /// </summary>
    public class UnitsDescriptionXL {

        /// <summary>
        /// Load unit description list
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="defaultCurrency">Default currency</param>
        /// <param name="defaultKCurrency">Default k currency</param>
        /// <returns>flag list</returns>
        public static List<UnitInformation> Load(IDataSource source, out CustomerSessions.Unit defaultCurrency, out CustomerSessions.Unit defaultKCurrency)
        {

            #region Variables
            List<UnitInformation> list = new List<UnitInformation>();
            XmlTextReader reader = null;
            string id;
            Int64 webTextId;
            Int64 webTextSignId;
            string baseId="";          
            string databaseField="";
            string databaseMultimediaField="";
            string databaseTrendsField="";
            string cellType = "";
            string format = "";
            UnitInformation unit;
            string assembly = "";
            #endregion

            try {
                defaultCurrency = CustomerSessions.Unit.none;
                defaultKCurrency = CustomerSessions.Unit.none;
                source.Open();
                reader = (XmlTextReader)source.GetSource();
                while(reader.Read()) {
                    if(reader.NodeType == XmlNodeType.Element) {
                        switch(reader.LocalName) {
                            case "units":

                                if (reader.GetAttribute("defaultCurrency") == null || reader.GetAttribute("defaultCurrency").Length == 0)
                                    throw (new InvalidXmlValueException("Invalid default currency parameter"));
                                else
                                    defaultCurrency = (CustomerSessions.Unit)Enum.Parse(typeof(CustomerSessions.Unit), reader.GetAttribute("defaultCurrency"), true);

                                if (reader.GetAttribute("defaultKCurrency") == null || reader.GetAttribute("defaultKCurrency").Length == 0)
                                    throw (new InvalidXmlValueException("Invalid default KCurrency parameter"));
                                else
                                    defaultKCurrency = (CustomerSessions.Unit)Enum.Parse(typeof(CustomerSessions.Unit), reader.GetAttribute("defaultKCurrency"), true);

                                break;
                            case "unit":
                                if(reader.GetAttribute("id") == null || reader.GetAttribute("id").Length == 0) throw (new InvalidXmlValueException("Invalid id parameter"));
                                id = reader.GetAttribute("id");
                                if(reader.GetAttribute("baseId") == null || reader.GetAttribute("baseId").Length == 0)baseId="";
                                else baseId = reader.GetAttribute("baseId");
                                if(reader.GetAttribute("webTextId") == null || reader.GetAttribute("webTextId").Length == 0) throw (new InvalidXmlValueException("Invalid webTextId parameter"));
                                webTextId = Int64.Parse(reader.GetAttribute("webTextId"));
                                if (reader.GetAttribute("webTextSignId") == null || reader.GetAttribute("webTextSignId").Length == 0)
                                    webTextSignId = -1;
                                else
                                    webTextSignId = Int64.Parse(reader.GetAttribute("webTextSignId"));
                                if (reader.GetAttribute("cellType") == null || reader.GetAttribute("cellType").Length == 0) cellType = "";
                                else cellType = reader.GetAttribute("cellType");
                                if(reader.GetAttribute("field") == null || reader.GetAttribute("field").Length == 0) databaseField="";
                                else databaseField = reader.GetAttribute("field");
                                if(reader.GetAttribute("multimediaField") == null || reader.GetAttribute("multimediaField").Length == 0) databaseMultimediaField="";
                                else databaseMultimediaField = reader.GetAttribute("multimediaField");
                                if(reader.GetAttribute("trendsField") == null || reader.GetAttribute("trendsField").Length == 0) databaseTrendsField="";
                                else databaseTrendsField = reader.GetAttribute("trendsField");
                                if (reader.GetAttribute("format") == null || reader.GetAttribute("format").Length == 0) format = "";
                                else format = reader.GetAttribute("format");
                                if (reader.GetAttribute("assembly") == null || reader.GetAttribute("assembly").Length == 0) assembly = "";
                                else assembly = reader.GetAttribute("assembly");
                                unit = new UnitInformation(id, format, webTextId, webTextSignId, baseId, cellType, databaseField, databaseMultimediaField, databaseTrendsField, assembly);
                                list.Add(unit);
                                break;
                        }
                    }
                }
            }
            catch(System.Exception err) {

                #region Close the file
                if(source.GetSource() != null) source.Close();
                #endregion

                throw (new Exception(" Error : ",err));
            }
            source.Close();
            return (list);

        }
    }
}
