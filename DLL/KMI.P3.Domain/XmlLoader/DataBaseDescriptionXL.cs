using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TNS.FrameWork.Exceptions;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.DB.Constantes;
using KMI.P3.Domain.DataBaseDescription;
namespace KMI.P3.Domain.Exceptions
{
    /// <summary>
    /// Load Database description
    /// </summary>
    public class DataBaseDescriptionXL
    {

        #region Load customer connections
        /// <summary>
        /// Load customer connections
        /// </summary>
        /// <example>
        /// <customerConnection enumId="0" description="EasyMusic user connection" dataBaseEnumId=""  dataSource="adexpr03.pige" connectionTimeOut="120" pooling="true" decrPoolSize="20" maxPoolSize="150"/>
        /// </example>
        /// <param name="source">source</param>
        /// <returns>Customer connections list</returns>
        public static Dictionary<CustomerConnectionIds, CustomerConnection> LoadCustomerConnections(IDataSource source)
        {

            #region Variables
            Dictionary<CustomerConnectionIds, CustomerConnection> list = new Dictionary<CustomerConnectionIds, CustomerConnection>();
            XmlTextReader reader = null;

            CustomerConnection customerConnection = null;
            CustomerConnectionIds customerConnectionId;
            DataSource.Type dataSourceTypes;
            string dataSourceParameter = "";

            #endregion

            try
            {
                source.Open();
                reader = (XmlTextReader)source.GetSource();
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {

                        switch (reader.LocalName)
                        {
                            case "customerConnection":
                                customerConnection = null;
                                dataSourceParameter = "";
                                if (reader.GetAttribute("enumId") == null || reader.GetAttribute("enumId").Length == 0) throw (new InvalidXmlValueException("Invalid enumId (connection parameter id) parameter"));
                                customerConnectionId = (CustomerConnectionIds)int.Parse(reader.GetAttribute("enumId"));
                                if (reader.GetAttribute("dataBaseEnumId") == null || reader.GetAttribute("dataBaseEnumId").Length == 0) throw (new InvalidXmlValueException("Invalid dataBaseEnumId parameter"));
                                dataSourceTypes = (DataSource.Type)int.Parse(reader.GetAttribute("dataBaseEnumId"));
                                if (reader.GetAttribute("dataSource") == null || reader.GetAttribute("dataSource").Length == 0) throw (new InvalidXmlValueException("Invalid dataSource parameter"));
                                dataSourceParameter = reader.GetAttribute("dataSource");
                                customerConnection = new CustomerConnection(customerConnectionId);
                                customerConnection.Type = dataSourceTypes;
                                customerConnection.DataSource = dataSourceParameter;
                                if (reader.GetAttribute("connectionTimeOut") != null)
                                    customerConnection.ConnectionTimeOut = int.Parse(reader.GetAttribute("connectionTimeOut"));
                                if (reader.GetAttribute("decrPoolSize") != null)
                                    customerConnection.DecrPoolSize = int.Parse(reader.GetAttribute("decrPoolSize"));
                                if (reader.GetAttribute("maxPoolSize") != null)
                                    customerConnection.MaxPoolSize = int.Parse(reader.GetAttribute("maxPoolSize"));
                                if (reader.GetAttribute("pooling") != null)
                                    customerConnection.Pooling = bool.Parse(reader.GetAttribute("pooling"));
                                list.Add(customerConnectionId, customerConnection);
                                if (reader.GetAttribute("isUtf8") != null)
                                    customerConnection.IsUTF8 = bool.Parse(reader.GetAttribute("isUtf8"));
                                break;
                        }
                    }
                }
            }
            #region Error Management
            catch (System.Exception err)
            {

                #region Close the file
                if (source.GetSource() != null) source.Close();
                #endregion

                throw (new Exception(" Error : ", err));
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
        public static Dictionary<DefaultConnectionIds, DefaultConnection> LoadDefaultConnections(IDataSource source)
        {
            #region Variables
            Dictionary<DefaultConnectionIds, DefaultConnection> list = new Dictionary<DefaultConnectionIds, DefaultConnection>();
            XmlTextReader reader = null;

            DefaultConnection defaultConnection = null;
            DefaultConnectionIds defaultConnectionId;
            DataSource.Type dataSourceTypes;
            string dataSourceParameter = "";
            string login = "";
            string password = "";

            #endregion

            try
            {
                source.Open();
                reader = (XmlTextReader)source.GetSource();

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {

                        switch (reader.LocalName)
                        {
                            case "defaultConnection":
                                defaultConnection = null;
                                dataSourceParameter = "";
                                login = "";
                                password = "";
                                if (reader.GetAttribute("enumId") == null || reader.GetAttribute("enumId").Length == 0) throw (new InvalidXmlValueException("Invalid enumId (connection parameter id) parameter"));
                                defaultConnectionId = (DefaultConnectionIds)int.Parse(reader.GetAttribute("enumId"));
                                if (reader.GetAttribute("dataBaseEnumId") == null || reader.GetAttribute("dataBaseEnumId").Length == 0) throw (new InvalidXmlValueException("Invalid dataBaseEnumId parameter"));
                                dataSourceTypes = (DataSource.Type)int.Parse(reader.GetAttribute("dataBaseEnumId"));
                                if (reader.GetAttribute("dataSource") == null || reader.GetAttribute("dataSource").Length == 0) throw (new InvalidXmlValueException("Invalid dataSource parameter"));
                                dataSourceParameter = reader.GetAttribute("dataSource");
                                if (reader.GetAttribute("login") == null || reader.GetAttribute("login").Length == 0) throw (new InvalidXmlValueException("Invalid login parameter"));
                                login = reader.GetAttribute("login");
                                if (reader.GetAttribute("password") == null || reader.GetAttribute("password").Length == 0) throw (new InvalidXmlValueException("Invalid password parameter"));
                                password = reader.GetAttribute("password");
                                defaultConnection = new DefaultConnection(defaultConnectionId);
                                defaultConnection.Type = dataSourceTypes;
                                defaultConnection.DataSource = dataSourceParameter;
                                defaultConnection.Login = login;
                                defaultConnection.Password = password;
                                if (reader.GetAttribute("connectionTimeOut") != null)
                                    defaultConnection.ConnectionTimeOut = int.Parse(reader.GetAttribute("connectionTimeOut"));
                                if (reader.GetAttribute("decrPoolSize") != null)
                                    defaultConnection.DecrPoolSize = int.Parse(reader.GetAttribute("decrPoolSize"));
                                if (reader.GetAttribute("maxPoolSize") != null)
                                    defaultConnection.MaxPoolSize = int.Parse(reader.GetAttribute("maxPoolSize"));
                                if (reader.GetAttribute("pooling") != null)
                                    defaultConnection.Pooling = bool.Parse(reader.GetAttribute("pooling"));
                                if (reader.GetAttribute("isUtf8") != null)
                                    defaultConnection.IsUTF8 = bool.Parse(reader.GetAttribute("isUtf8"));
                                list.Add(defaultConnectionId, defaultConnection);

                                break;
                        }
                    }
                }
            }
            #region Error Management
            catch (System.Exception err)
            {

                #region Close the file
                if (source.GetSource() != null) source.Close();
                #endregion

                throw (new Exception(" Error : ", err));
            }
            #endregion

            source.Close();
            return (list);
        }
        #endregion

        #region Load schemas
        /// <summary>
        /// Load schemas
        /// </summary>
        /// <example>
        /// <schema enumId="0" description="EasyMusic Data (AdExpr03)" label="adexpr"/>
        /// </example>
        /// <param name="source">source</param>
        /// <returns>Schema list</returns>
        public static Dictionary<SchemaIds, Schema> LoadSchemas(IDataSource source)
        {

            #region Variables
            Dictionary<SchemaIds, Schema> list = new Dictionary<SchemaIds, Schema>();
            XmlTextReader reader = null;
            SchemaIds schemaId;
            string label = "";

            #endregion

            try
            {
                source.Open();
                reader = (XmlTextReader)source.GetSource();
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {

                        switch (reader.LocalName)
                        {
                            case "schema":
                                if (reader.GetAttribute("enumId") == null || reader.GetAttribute("enumId").Length == 0) throw (new InvalidXmlValueException("Invalid enumId (schema id) parameter"));
                                schemaId = (SchemaIds)int.Parse(reader.GetAttribute("enumId"));
                                if (reader.GetAttribute("label") == null || reader.GetAttribute("label").Length == 0) throw (new InvalidXmlValueException("Invalid Schema label parameter"));
                                label = reader.GetAttribute("label");
                                list.Add(schemaId, new Schema(schemaId, label));
                                break;
                        }
                    }
                }
            }
            #region Error Management
            catch (System.Exception err)
            {

                #region Close the file
                if (source.GetSource() != null) source.Close();
                #endregion

                throw (new Exception(" Error : ", err));
            }
            #endregion

            source.Close();
            return (list);
        }
        #endregion

        #region Load Tables
        /// <summary>
        /// Load tables
        /// </summary>
        /// <example>
        /// <table enumId="91" description="Right: Login" schemaEnumId="2" label="login" prefix="lo"/>
        /// </example>
        /// <param name="source">source</param>
        /// <returns>Tables list</returns>
        public static Dictionary<TableIds, Table> LoadTables(IDataSource source, Dictionary<SchemaIds, Schema> schemas)
        {

            #region Variables
            Dictionary<TableIds, Table> list = new Dictionary<TableIds, Table>();
            XmlTextReader reader = null;

            TableIds tableId;
            SchemaIds schemaId;
            string label = "";
            string prefix = "";

            #endregion

            try
            {
                source.Open();
                reader = (XmlTextReader)source.GetSource();
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {

                        switch (reader.LocalName)
                        {
                            case "table":
                                if (reader.GetAttribute("enumId") == null || reader.GetAttribute("enumId").Length == 0) throw (new InvalidXmlValueException("Invalid enumId (table id) parameter"));
                                tableId = (TableIds)int.Parse(reader.GetAttribute("enumId"));
                                if (reader.GetAttribute("schemaEnumId") == null || reader.GetAttribute("schemaEnumId").Length == 0) throw (new InvalidXmlValueException("Invalid schemaEnumId (schema id) parameter"));
                                schemaId = (SchemaIds)int.Parse(reader.GetAttribute("schemaEnumId"));
                                if (reader.GetAttribute("label") == null || reader.GetAttribute("label").Length == 0) throw (new InvalidXmlValueException("Invalid table label parameter"));
                                label = reader.GetAttribute("label");
                                if (reader.GetAttribute("prefix") == null || reader.GetAttribute("prefix").Length == 0) throw (new InvalidXmlValueException("Invalid table prefix parameter"));
                                prefix = reader.GetAttribute("prefix");
                                list.Add(tableId, new Table(tableId, label, prefix, schemas[schemaId]));
                                break;
                        }
                    }
                }
            }
            #region Error Management
            catch (System.Exception err)
            {

                #region Close the file
                if (source.GetSource() != null) source.Close();
                #endregion

                throw (new Exception(" Error : ", err));
            }
            #endregion

            source.Close();
            return (list);
        }
        #endregion

        #region Load Views
        /// <summary>
        /// Load views
        /// </summary>
        /// <example>
        /// <view enumId="91" description="all media" schemaEnumId="0" label="all_media" prefix="am"/>
        /// </example>
        /// <param name="source">source</param>
        /// <returns>Views list</returns>
        public static Dictionary<ViewIds, View> LoadViews(IDataSource source, Dictionary<SchemaIds, Schema> schemas)
        {

            #region Variables
            Dictionary<ViewIds, View> list = new Dictionary<ViewIds, View>();
            XmlTextReader reader = null;

            ViewIds viewId;
            SchemaIds schemaId;
            string label = "";
            string prefix = "";
            #endregion

            try
            {
                source.Open();
                reader = (XmlTextReader)source.GetSource();
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {

                        switch (reader.LocalName)
                        {
                            case "view":
                                if (reader.GetAttribute("enumId") == null || reader.GetAttribute("enumId").Length == 0) throw (new InvalidXmlValueException("Invalid enumId (view id) parameter"));
                                viewId = (ViewIds)int.Parse(reader.GetAttribute("enumId"));
                                if (reader.GetAttribute("schemaEnumId") == null || reader.GetAttribute("schemaEnumId").Length == 0) throw (new InvalidXmlValueException("Invalid schemaEnumId (schema id) parameter"));
                                schemaId = (SchemaIds)int.Parse(reader.GetAttribute("schemaEnumId"));
                                if (reader.GetAttribute("label") == null || reader.GetAttribute("label").Length == 0) throw (new InvalidXmlValueException("Invalid view label parameter"));
                                label = reader.GetAttribute("label");
                                if (reader.GetAttribute("prefix") == null || reader.GetAttribute("prefix").Length == 0) throw (new InvalidXmlValueException("Invalid view prefix parameter"));
                                prefix = reader.GetAttribute("prefix");
                                list.Add(viewId, new View(viewId, label, prefix, schemas[schemaId]));
                                break;
                        }
                    }
                }
            }
            #region Error Management
            catch (System.Exception err)
            {

                #region Close the file
                if (source.GetSource() != null) source.Close();
                #endregion

                throw (new Exception(" Error : ", err));
            }
            #endregion

            source.Close();
            return (list);
        }
        #endregion

        #region Default result table prefix
        /// <summary>
        /// Get Default Result Table Prefix
        /// </summary>
        /// <param name="source">Data source</param>
        /// <returns>Default Result Table Prefix</returns>
        public static string LoadDefaultResultTablePrefix(IDataSource source)
        {
            #region Variables
            string defaultResultTablePrefix = "";
            XmlTextReader reader = null;

            #endregion

            try
            {
                source.Open();
                reader = (XmlTextReader)source.GetSource();
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {

                        switch (reader.LocalName)
                        {
                            case "tables":
                                if (reader.GetAttribute("defaultResultTablesPrefix") == null || reader.GetAttribute("defaultResultTablesPrefix").Length == 0) throw (new InvalidXmlValueException("Invalid default result tables prefix parameter"));
                                defaultResultTablePrefix = reader.GetAttribute("defaultResultTablesPrefix");
                                break;
                        }
                    }
                }
            }
            #region Error Management
            catch (System.Exception err)
            {

                #region Close the file
                if (source.GetSource() != null) source.Close();
                #endregion

                throw (new Exception(" Error : ", err));
            }
            #endregion

            source.Close();
            return (defaultResultTablePrefix);
        }
        #endregion
    }
}
