using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using KMI.PromoPSA.Web.Domain.Configuration;
using KMI.PromoPSA.Web.Domain.Exceptions;
using TNS.FrameWork.DB.Common;

namespace KMI.PromoPSA.Web.Domain.XmlLoader {
    /// <summary>
    /// Promo PSA Web Services XL
    /// </summary>
    class PromoPSAWebServicesXL {

        /// <summary>
        /// Load WebService description
        /// </summary>
        /// <param name="source">Source</param>
        /// <returns>WebService list</returns>
        public static List<PromoPSAWebService> Load(IDataSource source) {

            #region Variables
            List<PromoPSAWebService> list = new List<PromoPSAWebService>();
            XmlTextReader reader = null;
            int defaultTimeout = 60;
            int timeout = 60;
            #endregion

            try {
                source.Open();
                reader = (XmlTextReader)source.GetSource();
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {
                        switch (reader.LocalName) {
                            case "WebServices":
                                if (reader.GetAttribute("timeout") != null
                                    && reader.GetAttribute("timeout").Length > 0) {
                                    defaultTimeout = Int32.Parse(reader.GetAttribute("timeout"));
                                }
                                break;
                            case "WebService":
                                if (reader.GetAttribute("name") != null
                                    && reader.GetAttribute("url") != null
                                    && reader.GetAttribute("name").Length > 0
                                    && reader.GetAttribute("url").Length > 0
                                    ) {
                                    if (reader.GetAttribute("timeout") != null
                                   && reader.GetAttribute("timeout").Length > 0) {
                                        timeout = Int32.Parse(reader.GetAttribute("timeout"));
                                    }
                                    else {
                                        timeout = defaultTimeout;
                                    }
                                    list.Add(new PromoPSAWebService((WebServices.Names)Enum.Parse(typeof(WebServices.Names), reader.GetAttribute("name")), reader.GetAttribute("url"), timeout));
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

                throw (new PromoPSAWebServicesXLException(" Error : ", err));
            }
            source.Close();
            return (list);
        }

    }
}
