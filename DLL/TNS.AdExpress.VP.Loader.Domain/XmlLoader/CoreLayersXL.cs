#region Information
//  Author : Y Rkaina && D. Mussuma
//  Creation  date: 15/07/2009
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
using WebConstantes = TNS.AdExpress.Constantes.Web;


namespace TNS.AdExpress.VP.Loader.Domain.XmlLoader {
    public class CoreLayersXL {

        /// <summary>
        /// Load core layers description list
        /// </summary>
        /// <param name="source">source</param>
        /// <returns>layers list</returns>
        public static Dictionary<TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id, TNS.AdExpress.Domain.Layers.CoreLayer> Load(IDataSource source) {
            Dictionary<TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id, TNS.AdExpress.Domain.Layers.CoreLayer> list = new Dictionary<TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id, TNS.AdExpress.Domain.Layers.CoreLayer>();
            XmlTextReader reader = null;
            try {
                source.Open();
                reader = (XmlTextReader)source.GetSource();
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {
                        switch (reader.LocalName) {
                            case "layer":
                                if (reader.GetAttribute("id") != null && reader.GetAttribute("name") != null && reader.GetAttribute("assemblyName") != null && reader.GetAttribute("class") != null &&
                                   reader.GetAttribute("id").Length > 0 && reader.GetAttribute("name").Length > 0 && reader.GetAttribute("assemblyName").Length > 0 && reader.GetAttribute("class").Length > 0) {
                                    list.Add((TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id)Enum.Parse(typeof(TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id), reader.GetAttribute("id"), true), new TNS.AdExpress.Domain.Layers.CoreLayer(reader.GetAttribute("name"), reader.GetAttribute("assemblyName"), reader.GetAttribute("class")));
                                }
                                break;
                        }
                    }
                }
            }
            catch (System.Exception err) {

                #region Close the file
                if (source.GetSource() != null) source.Close();
                #endregion

                throw (new Exception(" Error : ", err));
            }
            source.Close();
            return (list);
        }
    }
}
