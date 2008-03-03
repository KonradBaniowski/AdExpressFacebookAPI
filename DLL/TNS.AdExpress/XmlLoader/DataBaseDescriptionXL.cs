#region Information
//  Author : G. Facon
//  Creation  date: 02/03/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TNS.AdExpress.Exceptions;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.DB.Constantes;
using TNS.AdExpress.DataBaseDescription;



namespace TNS.AdExpress.XmlLoader{
    /// <summary>
    /// Load Database description
    /// </summary>
    public class DataBaseDescriptionXL {

        /// <summary>
        /// Load customer connections
        /// </summary>
        /// <example>
        /// <customerConnection enumId="0" description="AdExpress user connection" dataBaseEnumId=""  dataSource="adexpr03.pige" connectionTimeOut="120" pooling="true" decrPoolSize="20" maxPoolSize="150"/>
        /// </example>
        /// <param name="source">source</param>
        /// <returns>Customer connections list</returns>
        public static Dictionary<CustomerConnectionIds,CustomerConnection> LoadCustomerConnections(IDataSource dataSource) {
            #region Variables
            Dictionary<CustomerConnectionIds,CustomerConnection> themes=new Dictionary<CustomerConnectionIds,CustomerConnection>();
            XmlTextReader reader=null;
            
            CustomerConnectionIds customerConnection;
            DatabaseTypes dataSourceTypes;
            string dataSourceParameter="";
            int connectionTimeOut=-1;
            bool pooling;
            int decrPoolSize=-1;
            int maxPoolSize=-1;
            
            #endregion

            try {
                reader=(XmlTextReader)dataSource.GetSource();
                while(reader.Read()) {
                    if(reader.NodeType==XmlNodeType.Element) {

                        switch(reader.LocalName) {
                            case "customerConnection":
                                if(reader.GetAttribute("enumId")==null || reader.GetAttribute("enumId").Length==0) throw (new InvalidXmlValueException("Invalid enumId (connection parameter id) parameter"));
                                customerConnection=(CustomerConnectionIds)int.Parse(reader.GetAttribute("enumId"));
                                if(reader.GetAttribute("dataBaseEnumId")==null || reader.GetAttribute("dataBaseEnumId").Length==0) throw (new XmlNullValueException("Invalid dataBaseEnumId parameter"));
                                dataSourceTypes=(DatabaseTypes)int.Parse(reader.GetAttribute("dataBaseEnumId"));

                                //themes.Add(siteLanguage,new WebTheme(id,name,siteLanguage));
                                break;
                        }
                    }
                }
            }
            #region Error Management
            catch(System.Exception err) {

                #region Close the file
                if(dataSource.GetSource()!=null) dataSource.Close();
                #endregion

                throw (new Exception(" Error : ",err));
            }
            #endregion

            dataSource.Close();
            return (null);
        }
    }
}
