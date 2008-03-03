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

        #region Load customer connections
        /// <summary>
        /// Load customer connections
        /// </summary>
        /// <example>
        /// <customerConnection enumId="0" description="AdExpress user connection" dataBaseEnumId=""  dataSource="adexpr03.pige" connectionTimeOut="120" pooling="true" decrPoolSize="20" maxPoolSize="150"/>
        /// </example>
        /// <param name="source">source</param>
        /// <returns>Customer connections list</returns>
        public static Dictionary<CustomerConnectionIds,CustomerConnection> LoadCustomerConnections(IDataSource source) {
            #region Variables
            Dictionary<CustomerConnectionIds,CustomerConnection> list=new Dictionary<CustomerConnectionIds,CustomerConnection>();
            XmlTextReader reader=null;

            CustomerConnection customerConnection=null;
            CustomerConnectionIds customerConnectionId;
            DataSource.Type dataSourceTypes;
            string dataSourceParameter="";

            #endregion

            try {
                source.Open();
                reader=(XmlTextReader)source.GetSource();
                while(reader.Read()) {
                    if(reader.NodeType==XmlNodeType.Element) {

                        switch(reader.LocalName) {
                            case "customerConnection":
                                customerConnection=null;
                                dataSourceParameter="";
                                if(reader.GetAttribute("enumId")==null || reader.GetAttribute("enumId").Length==0) throw (new InvalidXmlValueException("Invalid enumId (connection parameter id) parameter"));
                                customerConnectionId=(CustomerConnectionIds)int.Parse(reader.GetAttribute("enumId"));
                                if(reader.GetAttribute("dataBaseEnumId")==null || reader.GetAttribute("dataBaseEnumId").Length==0) throw (new InvalidXmlValueException("Invalid dataBaseEnumId parameter"));
                                dataSourceTypes=(DataSource.Type)int.Parse(reader.GetAttribute("dataBaseEnumId"));
                                if(reader.GetAttribute("dataSource")==null || reader.GetAttribute("dataSource").Length==0) throw (new InvalidXmlValueException("Invalid dataSource parameter"));
                                dataSourceParameter=reader.GetAttribute("dataSource");
                                customerConnection=new CustomerConnection(customerConnectionId);
                                customerConnection.Type=dataSourceTypes;
                                customerConnection.DataSource=dataSourceParameter;
                                if(reader.GetAttribute("connectionTimeOut")!=null)
                                    customerConnection.ConnectionTimeOut=int.Parse(reader.GetAttribute("connectionTimeOut"));
                                if(reader.GetAttribute("decrPoolSize")!=null)
                                    customerConnection.DecrPoolSize=int.Parse(reader.GetAttribute("decrPoolSize"));
                                if(reader.GetAttribute("maxPoolSize")!=null)
                                    customerConnection.MaxPoolSize=int.Parse(reader.GetAttribute("maxPoolSize"));
                                if(reader.GetAttribute("pooling")!=null)
                                    customerConnection.Pooling=bool.Parse(reader.GetAttribute("pooling"));
                                list.Add(customerConnectionId,customerConnection);

                                break;
                        }
                    }
                }
            }
            #region Error Management
            catch(System.Exception err) {

                #region Close the file
                if(source.GetSource()!=null) source.Close();
                #endregion

                throw (new Exception(" Error : ",err));
            }
            #endregion

            source.Close();
            return (list);
        } 
        #endregion

        #region Load default connections
        /// <summary>
        /// Load default connections
        /// </summary>
        /// <example>
        /// <defaultConnection enumId="0" description="Session" dataBaseEnumId="2" userId="xxx" password="xxx" dataSource="adexpr03.pige" connectionTimeOut="120" pooling="true" decrPoolSize="20" maxPoolSize="150"/>
        /// </example>
        /// <param name="source">source</param>
        /// <returns>Customer connections list</returns>
        public static Dictionary<DefaultConnectionIds,DefaultConnection> LoadDefaultConnections(IDataSource source) {
            #region Variables
            Dictionary<DefaultConnectionIds,DefaultConnection> list=new Dictionary<DefaultConnectionIds,DefaultConnection>();
            XmlTextReader reader=null;

            DefaultConnection defaultConnection=null;
            DefaultConnectionIds defaultConnectionId;
            DataSource.Type dataSourceTypes;
            string dataSourceParameter="";
            string login="";
            string password="";

            #endregion

            try {
                source.Open();
                reader=(XmlTextReader)source.GetSource();
                
                while(reader.Read()) {
                    if(reader.NodeType==XmlNodeType.Element) {

                        switch(reader.LocalName) {
                            case "defaultConnection":
                                defaultConnection=null;
                                dataSourceParameter="";
                                login="";
                                password="";
                                if(reader.GetAttribute("enumId")==null || reader.GetAttribute("enumId").Length==0) throw (new InvalidXmlValueException("Invalid enumId (connection parameter id) parameter"));
                                defaultConnectionId=(DefaultConnectionIds)int.Parse(reader.GetAttribute("enumId"));
                                if(reader.GetAttribute("dataBaseEnumId")==null || reader.GetAttribute("dataBaseEnumId").Length==0) throw (new InvalidXmlValueException("Invalid dataBaseEnumId parameter"));
                                dataSourceTypes=(DataSource.Type)int.Parse(reader.GetAttribute("dataBaseEnumId"));
                                if(reader.GetAttribute("dataSource")==null || reader.GetAttribute("dataSource").Length==0) throw (new InvalidXmlValueException("Invalid dataSource parameter"));
                                dataSourceParameter=reader.GetAttribute("dataSource");
                                if(reader.GetAttribute("login")==null || reader.GetAttribute("login").Length==0) throw (new InvalidXmlValueException("Invalid login parameter"));
                                login=reader.GetAttribute("login");
                                if(reader.GetAttribute("password")==null || reader.GetAttribute("password").Length==0) throw (new InvalidXmlValueException("Invalid password parameter"));
                                password=reader.GetAttribute("password");
                                defaultConnection=new DefaultConnection(defaultConnectionId);
                                defaultConnection.Type=dataSourceTypes;
                                defaultConnection.DataSource=dataSourceParameter;
                                defaultConnection.Login=login;
                                defaultConnection.Password=password;
                                if(reader.GetAttribute("connectionTimeOut")!=null)
                                    defaultConnection.ConnectionTimeOut=int.Parse(reader.GetAttribute("connectionTimeOut"));
                                if(reader.GetAttribute("decrPoolSize")!=null)
                                    defaultConnection.DecrPoolSize=int.Parse(reader.GetAttribute("decrPoolSize"));
                                if(reader.GetAttribute("maxPoolSize")!=null)
                                    defaultConnection.MaxPoolSize=int.Parse(reader.GetAttribute("maxPoolSize"));
                                if(reader.GetAttribute("pooling")!=null)
                                    defaultConnection.Pooling=bool.Parse(reader.GetAttribute("pooling"));
                                list.Add(defaultConnectionId,defaultConnection);

                                break;
                        }
                    }
                }
            }
            #region Error Management
            catch(System.Exception err) {

                #region Close the file
                if(source.GetSource()!=null) source.Close();
                #endregion

                throw (new Exception(" Error : ",err));
            }
            #endregion

            source.Close();
            return (list);
        }
        #endregion
    }
}
