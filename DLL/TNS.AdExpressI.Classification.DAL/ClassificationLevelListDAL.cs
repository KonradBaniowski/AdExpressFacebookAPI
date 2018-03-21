#region Informations
// Auteur: D. Mussuma
// Date de création: 17/08/2009
// Date de modification: 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpressI.Classification.DAL.Exceptions;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpressI.Classification.DAL
{

    /// <summary>
    /// Contains the list of labels of a classification's level 
    /// </summary>
    /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.ClassificationDALException">
    /// Impossible to load classification's items
    /// </exception>
    public class ClassificationLevelListDAL
    {

        #region Variables
        /// <summary>
        ///Detail level information
        /// </summary>
        protected DetailLevelItemInformation _detailLevelItemInformation = null;
        /// <summary>
        /// Database schema
        /// </summary>
        protected string _dbSchema = "";
        /// <summary>
        /// Classification items list
        /// </summary>
        protected Dictionary<long, string> _list;
        /// <summary>
        /// Identifiers' list sorted by labels
        /// </summary>
        protected List<long> _idListOrderByClassificationItem = new List<long>();
        /// <summary>
        /// Data Table
        /// </summary>
        protected DataTable _dataTable;
        /// <summary>
        /// Items' Language
        /// </summary>
        protected int _language = TNS.AdExpress.Constantes.DB.Language.FRENCH;

        #endregion

        #region Constructors
        /// <summary>	
        /// Constructor of items list of classification's level
        /// </summary>
        /// <remarks>Use only in TNS AdExpress website</remarks>
        /// <param name="detailLevelItemInformation">Detail level information to build the list</param>
        /// <param name="language">Data language identifier</param>
        /// <param name="source">Data source</param>
        public ClassificationLevelListDAL(DetailLevelItemInformation detailLevelItemInformation, int language, IDataSource source)
        {
            this._language = language;
            if (detailLevelItemInformation == null) throw (new NullReferenceException("detailLevelItemInformation parameter is null"));
            this._detailLevelItemInformation = detailLevelItemInformation;
            DataTable dt;

            _list = new Dictionary<long, string>();
            SetSchema();

            // Building query
            string sql = BuildSQLQuery(detailLevelItemInformation.DataBaseIdField, detailLevelItemInformation.DataBaseField, detailLevelItemInformation.DataBaseTableName, language);

            //Execute query
            try
            {
                dt = source.Fill(sql).Tables[0];
                dt.TableName = detailLevelItemInformation.DataBaseTableName;
                _dataTable = dt;
            }
            catch (System.Exception ex)
            {
                throw (new ClassificationDALException("Impossible to load classification's items", ex));
            }

            #region Transformation of DataTable to dictionary
            try
            {
                foreach (DataRow currentRow in dt.Rows)
                {
                    _list.Add(long.Parse(currentRow[0].ToString()), currentRow[1].ToString());
                    _idListOrderByClassificationItem.Add(long.Parse(currentRow[0].ToString()));
                }
            }
            catch (System.Exception ext)
            {
                throw (new ClassificationDALException("Impossible to transfer data in the list", ext));
            }
            #endregion

        }

        /// <summary>	
        /// Constructor of items list of classification's level
        /// </summary>
        /// <remarks>Use only in TNS AdExpress website</remarks>
        /// <param name="detailLevelItemInformation">Detail level information to build the list</param>
        /// <param name="language">Data language identifier</param>
        /// <param name="source">Data source</param>
        public ClassificationLevelListDAL(DetailLevelItemInformation detailLevelItemInformation, int language, IDataSource source, string dbSchema)
        {
            this._language = language;
            if (detailLevelItemInformation == null) throw (new NullReferenceException("detailLevelItemInformation parameter is null"));
            this._detailLevelItemInformation = detailLevelItemInformation;
            DataTable dt;
            _dbSchema = dbSchema;

            _list = new Dictionary<long, string>();
            SetSchema();

            // Building query
            string sql = BuildSQLQuery(detailLevelItemInformation.DataBaseIdField, detailLevelItemInformation.DataBaseField, detailLevelItemInformation.DataBaseTableName, language);

            //Execute query
            try
            {
                dt = source.Fill(sql).Tables[0];
                dt.TableName = detailLevelItemInformation.DataBaseTableName;
                _dataTable = dt;
            }
            catch (System.Exception ex)
            {
                throw (new ClassificationDALException("Impossible to load classification's items", ex));
            }

            #region Transformation of DataTable to dictionary
            try
            {
                foreach (DataRow currentRow in dt.Rows)
                {
                    _list.Add(long.Parse(currentRow[0].ToString()), currentRow[1].ToString());
                    _idListOrderByClassificationItem.Add(long.Parse(currentRow[0].ToString()));
                }
            }
            catch (System.Exception ext)
            {
                throw (new ClassificationDALException("Impossible to transfer data in the list", ext));
            }
            #endregion

        }


        /// Constructor of items list of classification's level
        /// </summary>
        /// <remarks>Use only in TNS AdExpress website</remarks>
        /// <param name="detailLevelItemInformation">Detail level information to build the list</param>
        /// <param name="idList">classification items' identifier list</param>
        /// <param name="language">Data language identifier</param>
        /// <param name="source">Data source</param>
        public ClassificationLevelListDAL(DetailLevelItemInformation detailLevelItemInformation, string idList, int language, IDataSource source, string dbSchema)
        {
            this._language = language;
            if (detailLevelItemInformation == null) throw (new NullReferenceException("detailLevelItemInformation parameter is null"));
            this._detailLevelItemInformation = detailLevelItemInformation;
            DataTable dt;
            _list = new Dictionary<long, string>();
            _dbSchema = dbSchema;

            SetSchema();

            // Building query
            string sql = BuildSQLQuery(detailLevelItemInformation.DataBaseIdField, detailLevelItemInformation.DataBaseField, detailLevelItemInformation.DataBaseTableName, idList, language);

            //Execute query
            try
            {
                dt = source.Fill(sql).Tables[0];
                dt.TableName = detailLevelItemInformation.DataBaseTableName;
                _dataTable = dt;
            }
            catch (System.Exception ex)
            {
                throw (new ClassificationDALException("Impossible to load classification's items", ex));
            }

            #region Transformation of DataTable to dictionary
            try
            {
                foreach (DataRow currentRow in dt.Rows)
                {
                    _list.Add(long.Parse(currentRow[0].ToString()), currentRow[1].ToString());
                    _idListOrderByClassificationItem.Add(long.Parse(currentRow[0].ToString()));
                }
            }
            catch (System.Exception ext)
            {
                throw (new ClassificationDALException("Impossible to transfer data in the list", ext));
            }
            #endregion

        }

        /// Constructor of items list of classification's level
        /// </summary>
        /// <remarks>Use only in TNS AdExpress website</remarks>
        /// <param name="detailLevelItemInformation">Detail level information to build the list</param>
        /// <param name="idList">classification items' identifier list</param>
        /// <param name="language">Data language identifier</param>
        /// <param name="source">Data source</param>
        public ClassificationLevelListDAL(DetailLevelItemInformation detailLevelItemInformation, string idList, int language, IDataSource source)
        {
            this._language = language;
            if (detailLevelItemInformation == null) throw (new NullReferenceException("detailLevelItemInformation parameter is null"));
            this._detailLevelItemInformation = detailLevelItemInformation;
            DataTable dt;
            _list = new Dictionary<long, string>();

            SetSchema();

            // Building query
            string sql = BuildSQLQuery(detailLevelItemInformation.DataBaseIdField, detailLevelItemInformation.DataBaseField, detailLevelItemInformation.DataBaseTableName, idList, language);

            //Execute query
            try
            {
                dt = source.Fill(sql).Tables[0];
                dt.TableName = detailLevelItemInformation.DataBaseTableName;
                _dataTable = dt;
            }
            catch (System.Exception ex)
            {
                throw (new ClassificationDALException("Impossible to load classification's items", ex));
            }

            #region Transformation of DataTable to dictionary
            try
            {
                foreach (DataRow currentRow in dt.Rows)
                {
                    _list.Add(long.Parse(currentRow[0].ToString()), currentRow[1].ToString());
                    _idListOrderByClassificationItem.Add(long.Parse(currentRow[0].ToString()));
                }
            }
            catch (System.Exception ext)
            {
                throw (new ClassificationDALException("Impossible to transfer data in the list", ext));
            }
            #endregion

        }

        /// Constructor of items list of classification's level
        /// </summary>
        /// <remarks>Use only in TNS AdExpress website</remarks>
        /// <param name="table">Target table used to build the list</param>
        /// <param name="idList">classification items' identifier list</param>
        /// <param name="language">Data language identifier</param>
        /// <param name="source">Data source</param>
        public ClassificationLevelListDAL(string table, string idList, int language, IDataSource source, string dbSchema)
        {
            this._language = language;
            _list = new Dictionary<long, string>();
            DataTable dt;

            _dbSchema = dbSchema;

            SetSchema();

            //Building query
            string sql = BuildSQLQuery("id_" + table, table, table, idList, language);

            //Execute query
            try
            {
                dt = source.Fill(sql).Tables[0];
                dt.TableName = table.ToString();
                _dataTable = dt;
            }
            catch (System.Exception ex)
            {
                throw (new ClassificationDALException("Impossible to load classification's items", ex));
            }


            #region Transformation of DataTable to dictionary
            try
            {
                foreach (DataRow currentRow in dt.Rows)
                {
                    _list.Add(long.Parse(currentRow[0].ToString()), currentRow[1].ToString());
                    _idListOrderByClassificationItem.Add(long.Parse(currentRow[0].ToString()));
                }
            }
            catch (System.Exception ext)
            {
                throw (new ClassificationDALException("Impossible to transfer data in the list", ext));
            }
            #endregion
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public ClassificationLevelListDAL()
        {
        }
		
        #endregion

		
       
		
		#region Properties
		/// <summary>
		/// Returns Label of corresponding identifier
		/// </summary>
		public string this [Int64 id]{
			get{
				try{
					return(_list[id].ToString());
				}
				catch(System.Exception e){
					throw (new ClassificationDALException("There is not label for identifying it " + id.ToString() + ". The vehicle is it maybe UNACTIVATED, in that case will be deleted during the next maintenance of vehicle", e));
				}
			}
		}

        /// <summary>
        /// Returns the list of identifiers sorted by labels
        /// </summary>
        public List<long> IdListOrderByClassificationItem
        {
            get { return (this._idListOrderByClassificationItem); }
        }

        /// <summary>
        /// Get Data table
        /// </summary>
        public DataTable GetDataTable
        {
            get { return (_dataTable); }
        }

        /// <summary>
        /// Data base schema 
        /// </summary>
        public string DbSchema
        {
            get { return (_dbSchema); }
            set { _dbSchema = value; }
        }
        #endregion

        /// <summary>
        /// Build SQL query to select list of classsification level items
        /// </summary>
        /// <param name="dataBaseIdField">Data Base Id Field</param>
        /// <param name="dataBaseField">Data Base Field</param>
        /// <param name="dataBaseTableName">Data BaseTable Name</param>
        /// <param name="idList">filter classsification level's identifier List</param>
        /// <param name="language">Data language</param>
        /// <returns>SQL Query string to select list of classsification level items</returns>
        protected virtual string BuildSQLQuery(string dataBaseIdField, string dataBaseField, string dataBaseTableName, string idList, int language)
        {
            #region Building query
            string sql = " select " + dataBaseIdField + ", " + dataBaseField;
            sql += " from " + (!string.IsNullOrEmpty(_dbSchema) ? _dbSchema + "." : "") + dataBaseTableName;
            sql += " where " + dataBaseIdField + " in (" + idList + ")";
          
                sql += " and activation<" + DBConstantes.ActivationValues.UNACTIVATED;
                sql += " and id_language = " + language;
            
            sql += " order by " + dataBaseField;
            #endregion

            return sql;
        }

        /// <summary>
        /// Build SQL query to select list of classsification level items
        /// </summary>
        /// <param name="dataBaseIdField">Data Base Id Field</param>
        /// <param name="dataBaseField">Data Base Field</param>
        /// <param name="dataBaseTableName">Data BaseTable Name</param>
        /// <param name="language">Data language</param>
        /// <returns>SQL Query string to select list of classsification level items</returns>
        protected virtual string BuildSQLQuery(string dataBaseIdField, string dataBaseField, string dataBaseTableName, int language)
        {
            #region Building query
            string sql = " select " + dataBaseIdField + ", " + dataBaseField;
            sql += " from " + ((_dbSchema != null && _dbSchema.Length > 0) ? _dbSchema + "." : "") + dataBaseTableName;
            sql += " where id_language = " + language + " and activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
            sql += " order by " + dataBaseField;
            #endregion

            return sql;
        }

        /// <summary>
        /// Set Schema
        /// </summary>
        protected virtual void SetSchema()
        {
            if (string.IsNullOrEmpty(_dbSchema )) _dbSchema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label;

        }
    }
}
