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
using TNS.AdExpress.Bastet.Units;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Bastet.Periods;

namespace TNS.AdExpress.Bastet.XmlLoader {
    /// <summary>
    /// Load Units descriptions from XML files
    /// </summary>
    public class PeriodsDescriptionXL {

        /// <summary>
        /// Load unit description list
        /// </summary>
        /// <param name="source">source</param>
        /// <returns>flag list</returns>
        public static List<PeriodInformation> Load(IDataSource source) {

            #region Variables
            List<PeriodInformation> list = new List<PeriodInformation>();
            XmlTextReader reader = null;
            Int64 webTextId;
            Int64 dataBaseId;
            PeriodInformation period;
            #endregion

            try {
                source.Open();
                reader = (XmlTextReader)source.GetSource();
                while(reader.Read()) {
                    if(reader.NodeType == XmlNodeType.Element) {
                        switch(reader.LocalName) {
                            case "period":
                                if(reader.GetAttribute("webTextId") == null || reader.GetAttribute("webTextId").Length == 0) throw (new InvalidXmlValueException("Invalid webTextId parameter"));
                                webTextId = Int64.Parse(reader.GetAttribute("webTextId"));
                                if (reader.GetAttribute("dataBaseId") == null || reader.GetAttribute("dataBaseId").Length == 0) throw (new InvalidXmlValueException("Invalid dataBaseId parameter"));
                                dataBaseId = Int64.Parse(reader.GetAttribute("dataBaseId"));

                                period = new PeriodInformation(webTextId, dataBaseId);
                                list.Add(period);
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
