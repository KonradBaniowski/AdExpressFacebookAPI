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

namespace TNS.AdExpress.Bastet.XmlLoader {
    /// <summary>
    /// Load Units descriptions from XML files
    /// </summary>
    public class GroupContactFilterXL {

        /// <summary>
        /// Load unit description list
        /// </summary>
        /// <param name="source">source</param>
        /// <returns>flag list</returns>
        public static List<Int64> Load(IDataSource source) {

            #region Variables
            List<Int64> list = new List<Int64>();
            XmlTextReader reader = null;
            #endregion

            try {
                source.Open();
                reader = (XmlTextReader)source.GetSource();
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {
                        switch (reader.LocalName) {
                            case "groupContactFilter":
                                if (reader.GetAttribute("id") == null) throw (new InvalidXmlValueException("Invalid id parameter"));
                                Int64 id = Int64.Parse(reader.GetAttribute("id"));
                                if (!list.Contains(id))
                                    list.Add(id);
                                break;
                        }
                    }
                }
            }
            catch (System.Exception err) {
                throw (new Exception(" Error : ", err));
            }
            finally {
                if (source != null) source.Close();
                source = null;
                if (reader != null) reader.Close();
                reader = null;
            }
            return (list);

        }
    }
}
