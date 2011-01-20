#region Informations
// Author: G. Facon
// Creation Date: 21/02/2008
// Modifications: 
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TNS.FrameWork.Exceptions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Core;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Domain.XmlLoader {
    /// <summary>
    /// Load all the configuration parameter for theme management
    /// </summary>
    public class ResultOptionsXL{
        /// <summary>
        /// Load result options
        /// </summary>
        /// <param name="dataSource">Data Source</param>
        /// <summary>
        public static void Load(IDataSource dataSource) {

            #region Variables
            XmlTextReader reader=null;
			Dictionary<CustomerSessions.InsertType, long> insetTypeCollection = new Dictionary<CustomerSessions.InsertType, long>();
			string id;
			long dataBaseId;
            bool useComparativeMediaSchedule = false;
            #endregion

            try {
                reader=(XmlTextReader)dataSource.GetSource();
                while(reader.Read()) {
                    if(reader.NodeType==XmlNodeType.Element) {

                        switch(reader.LocalName) {
                            case "inset":
                                if (reader.GetAttribute("available") != null && reader.GetAttribute("available").Length > 0)
                                {
                                    WebApplicationParameters.AllowInsetOption = Convert.ToBoolean(reader.GetAttribute("available"));
                                }
                                break;
							case "insetItem":
								if (reader.GetAttribute("id") == null || reader.GetAttribute("id").Length == 0) throw (new InvalidXmlValueException("Invalid id parameter"));
								id = reader.GetAttribute("id");
								if (reader.GetAttribute("dataBaseId") == null || reader.GetAttribute("dataBaseId").Length == 0) throw (new InvalidXmlValueException("Invalid data base Id parameter")); ;
								dataBaseId = long.Parse(reader.GetAttribute("dataBaseId"));
								insetTypeCollection.Add((CustomerSessions.InsertType)Enum.Parse(typeof(CustomerSessions.InsertType), id, true), dataBaseId);
								break;
                            case "mediaSchedule":
                                useComparativeMediaSchedule = bool.Parse(reader.GetAttribute("useComparative"));
                                break;
                        }
                    }
                }
				WebApplicationParameters.InsetTypeCollection = insetTypeCollection;
                WebApplicationParameters.UseComparativeMediaSchedule = useComparativeMediaSchedule;
            }

            #region Error Management
            catch(System.Exception err) {

                #region Close the file
                if(dataSource.GetSource()!=null) dataSource.Close();
                #endregion

                throw (new XmlException(" Error while loading ResultOptions.xml : ",err));
            }
            #endregion

            dataSource.Close();

        }
    }
}
