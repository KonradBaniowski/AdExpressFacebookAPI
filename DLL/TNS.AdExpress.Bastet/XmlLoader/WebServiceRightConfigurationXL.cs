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
using TNS.AdExpress.Bastet.Web;


namespace TNS.AdExpress.Bastet.XmlLoader {
    public class WebServiceRightConfigurationXL {

		 /// <summary>
        /// Load core layers description list
        /// </summary>
        /// <param name="source">source</param>
        /// <returns>layers list</returns>
        public static WebServiceRightConfiguration Load(IDataSource source) {
            WebServiceRightConfiguration webServiceRightConfiguration = null;
			XmlTextReader reader = null;
			try {
                source.Open();
                reader = (XmlTextReader)source.GetSource();				
				while (reader.Read()) {
					if (reader.NodeType == XmlNodeType.Element) {
						switch (reader.LocalName) {
                            case "webServiceRightConfiguration":
								if (reader.GetAttribute("url") != null && reader.GetAttribute("tnsName") != null &&
								   reader.GetAttribute("url").Length > 0 && reader.GetAttribute("tnsName").Length > 0 ) {
                                    webServiceRightConfiguration = new WebServiceRightConfiguration(reader.GetAttribute("url"), reader.GetAttribute("tnsName")); 
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
             return (webServiceRightConfiguration);
		}
	}
}
