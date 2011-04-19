using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using KMI.P3.Domain.Exceptions;
using KMI.P3.Domain.XmlLoader;

namespace KMI.P3.Domain.DataBaseDescription
{
    #region Enums

    #region Customer Connection Ids
    /// <summary>
    /// Customer Connection Ids
    /// </summary>
    public enum CustomerConnectionIds
    {
        /// <summary>
        /// P3 ~Charmed~
        /// </summary>
        p3=0,
        /// <summary>
        /// AdExpress
        /// </summary>
        adexpr03 = 1
    }
    #endregion

    #region Default Connection Ids
    /// <summary>
    /// Default Connection Ids
    /// </summary>
    public enum DefaultConnectionIds
    {
        /// <summary>
        /// Session
        /// </summary>
        session = 0,
        /// <summary>
        /// Translation
        /// </summary>
        translation = 1,       
        /// <summary>
        /// Web Administration
        /// </summary>
        webAdministration = 2
       
    }
    #endregion

    #region Schema Ids
    /// <summary>
    /// Schema Ids
    /// </summary>
    public enum SchemaIds
    {
        /// <summary>
        /// AdExpr03
        /// </summary>
        adexpr03 = 0,        
        /// <summary>
        /// Right (Mau)
        /// </summary>
        mau01 = 1,
        /// <summary>
        /// Mou
        /// </summary>
        mou01 = 2,      
        /// <summary>
        /// Web
        /// </summary>
        webnav01 = 3       
    }
    #endregion

    #region Table Ids
    /// <summary>
    /// Table Ids
    /// </summary>
    public enum TableIds
    {
        /// <summary>
        /// Customer login
        /// </summary>
        rightLogin = 1,
        /// <summary>
        /// Customer contact
        /// </summary>
        rightContact = 2,
        /// <summary>
        /// Customer contact group
        /// </summary>
        rightContactGroup = 3,
        /// <summary>
        /// Customer Address
        /// </summary>
        rightAddress = 4,
        /// <summary>
        /// Customer Company
        /// </summary>
        rightCompany = 5,
        /// <summary>
        /// Modules
        /// </summary>
        rightModule = 6,
        /// <summary>
        /// Module groups
        /// </summary>
        rightModuleGroup = 7,
        /// <summary>
        /// Module assignment
        /// </summary>
        rightModuleAssignment = 8,
        /// <summary>
        /// Right assignment
        /// </summary>
        rightAssignment = 9,
        /// <summary>
        /// My login
        /// </summary>
        rightMyLogin = 10,
        /// <summary>
        /// Module right frequency
        /// </summary>
        rightFrequency = 11,
        /// <summary>
        /// Module Category
        /// </summary>
        rightModuleCategory = 12,
        /// <summary>
        /// Flags 
        /// </summary>
        rightFlag = 13,
        /// <summary>
        /// Flags assignment
        /// </summary>
        rightProjectFlagAssignment = 14,
        /// <summary>
        /// customer Sessions
        /// </summary>
        customerSession = 15

    }
    #endregion

    #region View Ids
    /// <summary>
    /// View Ids
    /// </summary>
    public enum ViewIds
    {
        /// <summary>
        /// All Media
        /// </summary>
        allMedia = 0,
        /// <summary>
        /// All Product
        /// </summary>
        allProduct = 1
    }
    #endregion

    #endregion

    /// <summary>
    /// Database description
    /// </summary>
    public class DataBase
    {

        #region Variables
        /// <summary>
        /// Default connections list
        /// </summary>
        private Dictionary<DefaultConnectionIds, DefaultConnection> _defaultConnections;
        /// <summary>
        /// Customer Connections List
        /// </summary>
        private Dictionary<CustomerConnectionIds, CustomerConnection> _customerConnections;
        /// <summary>
        /// Schemas List
        /// </summary>
        private Dictionary<SchemaIds, Schema> _schemas;
        /// <summary>
        /// Tables List
        /// </summary>
        private Dictionary<TableIds, Table> _tables;
        /// <summary>
        /// Views List
        /// </summary>
        private Dictionary<ViewIds, View> _views;
        /// <summary>
        /// Default Result Table Prefix
        /// </summary>
        private string _defaultResultTablePrefix;

        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="source">Data source</param>
        public DataBase(IDataSource source)
        {
            _customerConnections = DataBaseDescriptionXL.LoadCustomerConnections(source);
            _defaultConnections = DataBaseDescriptionXL.LoadDefaultConnections(source);
            _schemas = DataBaseDescriptionXL.LoadSchemas(source);
            _tables = DataBaseDescriptionXL.LoadTables(source, _schemas);
            _views = DataBaseDescriptionXL.LoadViews(source, _schemas);
            _defaultResultTablePrefix = DataBaseDescriptionXL.LoadDefaultResultTablePrefix(source);
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get default result table prefix 
        /// </summary>
        public string DefaultResultTablePrefix
        {
            get { return (_defaultResultTablePrefix); }
        }
        #endregion

        #region Public Methodes

        /// <summary>
        /// Get default database connection
        /// </summary>
        /// <param name="defaultConnectionId">Connection Id</param>
        public IDataSource GetDefaultConnection(DefaultConnectionIds defaultConnectionId)
        {
            return (_defaultConnections[defaultConnectionId].GetDataSource());
        }
        /// <summary>
        /// Get default database connection
        /// </summary>
        /// <param name="defaultConnectionId">Connection Id</param>
        /// <param name="nlsSort">NLS sort code</param>
        public IDataSource GetDefaultConnection(DefaultConnectionIds defaultConnectionId, string nlsSort)
        {
            return (_defaultConnections[defaultConnectionId].GetDataSource(nlsSort));
        }
        /// <summary>
        /// Get schema label
        /// </summary>
        /// <param name="schema">Schema Id</param>
        /// <returns>schema label</returns>
        public Schema GetSchema(SchemaIds schemaId)
        {
            try
            {
                return (_schemas[schemaId]);
            }
            catch (System.Exception err)
            {
                throw (new DataBaseException("Impossible to retreive schema label", err));
            }
        }

        /// <summary>
        /// Get customer database connection
        /// </summary>
        /// <param name="login">Customer login</param>
        /// <param name="password">Customer password</param>
        /// <param name="customerConnectionId">Connection Id</param>
        /// <returns>Data Source</returns>
        public IDataSource GetCustomerConnection(string login, string password, CustomerConnectionIds customerConnectionId)
        {
            if (login == null || login.Length == 0) throw (new ArgumentException("Invalid login parameter"));
            if (password == null || password.Length == 0) throw (new ArgumentException("Invalid password parameter"));
            return (_customerConnections[customerConnectionId].GetDataSource(login, password));
        }
        /// <summary>
        /// Get customer database connection
        /// </summary>
        /// <param name="login">Customer login</param>
        /// <param name="password">Customer password</param>
        /// <param name="customerConnectionId">Connection Id</param>
        /// <param name="isUTf8">True if is utf-8 encoding</param>
        /// <param name="nlsSort">NLS SORT code</param>
        /// <returns>Data Source</returns>
        public IDataSource GetCustomerConnection(string login, string password, string nlsSort, CustomerConnectionIds customerConnectionId)
        {
            if (login == null || login.Length == 0) throw (new ArgumentException("Invalid login parameter"));
            if (password == null || password.Length == 0) throw (new ArgumentException("Invalid password parameter"));
            return (_customerConnections[customerConnectionId].GetDataSource(login, password, nlsSort));
        }
        /// <summary>
        /// Get customer database connection
        /// </summary>
        /// <param name="login">Customer login</param>
        /// <param name="password">Customer password</param>
        /// <param name="customerConnectionId">Connection Id</param>
        /// <returns>Data Source</returns>
        public IDataSource GetAdExpr03CustomerConnection(string login, string password)
        {
            return (GetCustomerConnection(login, password, CustomerConnectionIds.adexpr03));

        }

        /// <summary>
        /// Get table label with schema label and prefix
        /// Schema.Table prefix
        /// </summary>
        /// <remarks>
        /// A space is put before the string
        /// </remarks>
        /// <example> adexpr03.data_press_4M wp</example>
        /// <param name="tableId">Table Id</param>
        /// <returns>SQL Table code</returns>
        public string GetSqlTableLabelWithPrefix(TableIds tableId)
        {
            try
            {
                return (_tables[tableId].SqlWithPrefix);
            }
            catch (System.Exception err)
            {
                throw (new DataBaseException("Impossible to retreive sql table code", err));
            }
        }

        /// <summary>
        /// Get table object
        /// </summary>
        /// <param name="tableId">Table Id</param>
        /// <returns>Table Object</returns>
        public Table GetTable(TableIds tableId)
        {
            try
            {
                Table table = _tables[tableId];
                if (table == null) throw (new DataBaseException("Table Object is null"));
                return (table);
            }
            catch (System.Exception err)
            {
                throw (new DataBaseException("Impossible to retreive table object", err));
            }
        }

        /// <summary>
        /// Get view label with schema label and prefix
        /// Schema.View prefix
        /// </summary>
        /// <remarks>
        /// A space is put before the string
        /// </remarks>
        /// <example>adexpr03.all_media am</example>
        /// <param name="viewId">View Id</param>
        /// <returns>SQL View code</returns>
        public string GetSqlViewLabelWithPrefix(ViewIds viewId)
        {
            try
            {
                return (_views[viewId].SqlWithPrefix);
            }
            catch (System.Exception err)
            {
                throw (new DataBaseException("Impossible to retreive sql view code", err));
            }
        }

        /// <summary>
        /// Get view object
        /// </summary>
        /// <param name="viewId">View Id</param>
        /// <returns>View Object</returns>
        public View GetView(ViewIds viewId)
        {
            try
            {
                View view = _views[viewId];
                if (view == null) throw (new DataBaseException("View Object is null"));
                return (view);
            }
            catch (System.Exception err)
            {
                throw (new DataBaseException("Impossible to retreive view object", err));
            }
        }
        #endregion
    }
}
