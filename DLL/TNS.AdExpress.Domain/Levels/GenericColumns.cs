#region Informations
// Auteur: D. Mussuma, Y. Rkaina
// Création: 24/04/2006
// Modification:
#endregion

using System;
using System.Collections;
using System.Data;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.DB;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web;
using System.Collections.Generic;
using TNS.AdExpress.Domain.DataBaseDescription;

namespace TNS.AdExpress.Domain.Level
{
	/// <summary>
	/// Description  de colonnes génériques	.
	/// </summary>
	public class GenericColumns
	{
		#region variables
		/// <summary>
		/// Liste des colonnes
		/// </summary>
		/// <remarks>Contient des GenericColumnItemInformation</remarks>
		protected List<GenericColumnItemInformation> _columns;
		/// <summary>
		/// Définit le type d'emplacement d'où c'est la sélection des colonnes
		/// </summary>
		protected  WebConstantes.GenericColumn.SelectedFrom _selectedFrom;
		#endregion

		#region Constructeurs
		/// <summary>
		/// Contructeur avec une liste de colonnes
		/// </summary>
		/// <param name="columnIds">Liste des identifiant des colonnes</param>
		/// <remarks>columnIds doit contenir des int</remarks>
		/// <exception cref="System.ArgumentNullException">Si la liste des colonnes est null</exception>
		public GenericColumns(List<Int64> columnIds){
			_selectedFrom=WebConstantes.GenericColumn.SelectedFrom.unknown;
			if(columnIds==null)throw(new ArgumentNullException("columnIds list is null"));
            _columns = new List<GenericColumnItemInformation>();
			foreach(int currentId in columnIds){
				_columns.Add(WebApplicationParameters.GenericColumnItemsInformation.Get(currentId));
			}
		}

		/// <summary>
		/// Contructeur avec une liste de colonnes
		/// </summary>
		/// <param name="columnIds">Liste des identifiant des colonnes</param>
		/// <param name="selectedFrom">Niveau Sélectionné à partir de</param>
		/// <remarks>columnIds doit contenir des int</remarks>
		/// <exception cref="System.ArgumentNullException">Si la liste des colonnes est null</exception>
        public GenericColumns(List<Int64> columnIds, WebConstantes.GenericColumn.SelectedFrom selectedFrom)
            : this(columnIds)
        {
		_selectedFrom=selectedFrom;
	}


	#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient ou définit le type d'emplacement d'où c'est la sélection de colonnes
		/// </summary>
		public WebConstantes.GenericColumn.SelectedFrom FromControlItem{
			get{return(_selectedFrom);}
			set{_selectedFrom=value;}
		}
		/// <summary>
		/// Obtient la liste contenant les colonnes
		/// </summary>
        public List<GenericColumnItemInformation> Columns
        {
			get{return(_columns);}
		}
		/// <summary>
		/// Obtient la liste des identifiants des colonnes
		/// </summary>
        public List<Int64> ColumnIds
        {
			get{
                List<Int64> columnIds = new List<Int64>();
				foreach(GenericColumnItemInformation currentColumn in _columns){
					columnIds.Add((int)currentColumn.Id);
				}
				return(columnIds);}
		}

		/// <summary>
		/// Obtient le nombre de colonnes
		/// </summary>
		public int GetNbColumns{
			get{return(_columns.Count);}
		}
		#endregion

		#region Méthode publiques

		#region SQLGenerator

        #region select
        /// <summary>
		/// Obtient le code SQL des champs correspondant aux colonnes
		/// </summary>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
        public string GetSqlFields() {
            string sql = "";
            foreach (GenericColumnItemInformation currentColumn in _columns) {
                if (currentColumn.Constraints == null || currentColumn.Constraints.Count <= 0) {
                    if (currentColumn.GetSqlFieldId() != null && currentColumn.GetSqlFieldId().Length > 0)
                        sql += currentColumn.GetSqlFieldId() + ",";
                    if (currentColumn.GetSqlField() != null && currentColumn.GetSqlField().Length > 0)
                        sql += currentColumn.GetSqlField() + ",";
                }
                else {
                    if (currentColumn.GetSqlField() != null && currentColumn.GetSqlField().Length > 0)
                        sql += string.Format("to_char({0}.stragg2({1})) as {2},", WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label, currentColumn.DataBaseField, ((currentColumn.DataBaseAliasField != null && currentColumn.DataBaseAliasField.Length > 0) ? currentColumn.DataBaseAliasField : currentColumn.DataBaseField));
                }
            }
            if (sql.Length > 0) sql = sql.Substring(0, sql.Length - 1);
            return (sql);
        }
		/// <summary>
		/// Obtient le code SQL des champs correspondant aux colonnes exceptées celles qui ont une équivalence
		/// avec le niveau de détail présenté en ligne.  
		/// </summary>
		/// <param name="detailLevelList">Identifiants de niveau de détail</param>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlFields(ArrayList detailLevelList){
			string sql="";
			foreach(GenericColumnItemInformation currentColumn in _columns){
				if(detailLevelList==null ||  detailLevelList.Count==0 || !detailLevelList.Contains(currentColumn.IdDetailLevelMatching)){
                    if (currentColumn.Constraints == null || currentColumn.Constraints.Count <= 0) {
                        if (currentColumn.GetSqlFieldId() != null && currentColumn.GetSqlFieldId().Length > 0)
                            sql += currentColumn.GetSqlFieldId() + ",";
                        if (currentColumn.GetSqlField() != null && currentColumn.GetSqlField().Length > 0)
                            sql += currentColumn.GetSqlField() + ",";
                    }
                    else {
                        if (currentColumn.GetSqlField() != null && currentColumn.GetSqlField().Length > 0)
                            sql += string.Format("to_char({0}.stragg2({1})) as {2},", WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label, currentColumn.DataBaseField
                                ,((currentColumn.DataBaseAliasField != null && currentColumn.DataBaseAliasField.Length > 0) ? currentColumn.DataBaseAliasField : currentColumn.DataBaseField));
                    }
				}
			}
			if(sql.Length>0)sql=sql.Substring(0,sql.Length-1);
			return(sql);
		}

        /// <summary>
        /// Get the SQL code of the fields corresponding to columns (from the XML configuration files) except the ones that mutched with
        /// the detail level list
        /// </summary>
        /// <param name="columns">List of columns to include</param>
        /// <param name="detailLevelList">Detail level Ids list</param>
        /// <remarks>The SQL code doesn't end with a coma</remarks>
        /// <returns>SQL Code</returns>
        public static string GetSqlFields(List<GenericColumnItemInformation> columns, ArrayList detailLevelList)
        {
            string sql = "";
            foreach (GenericColumnItemInformation currentColumn in columns)
            {
                if (detailLevelList == null || detailLevelList.Count == 0 || !detailLevelList.Contains(currentColumn.IdDetailLevelMatching))
                {
                    if (currentColumn.Constraints == null || currentColumn.Constraints.Count <= 0)
                    {
                        if (currentColumn.GetSqlFieldId() != null && currentColumn.GetSqlFieldId().Length > 0)
                            sql += currentColumn.GetSqlFieldId() + ",";
                        if (currentColumn.GetSqlField() != null && currentColumn.GetSqlField().Length > 0)
                            sql += currentColumn.GetSqlField() + ",";
                    }
                    else
                    {
                        if (currentColumn.GetSqlField() != null && currentColumn.GetSqlField().Length > 0)
                            sql += string.Format("to_char({0}.stragg2(distinct {1})) as {2},"
                                , WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label
                                , currentColumn.DataBaseField
                                , ((currentColumn.DataBaseAliasField != null && currentColumn.DataBaseAliasField.Length > 0) ? currentColumn.DataBaseAliasField : currentColumn.DataBaseField)
                                );
                    }
                }
            }
            if (sql.Length > 0) sql = sql.Substring(0, sql.Length - 1);
            return (sql);
        }
		
		/// <summary>
		/// Obtient le code SQL des champs correspondant aux colonnes suivant les règles de contraintes
		/// </summary>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlConstraintFields(){
			string sql="";
			ArrayList constraintList=null;
			foreach(GenericColumnItemInformation currentColumn in _columns){
				if(currentColumn.Constraints!=null && currentColumn.Constraints.Count>0)
					if(currentColumn.Constraints.ContainsKey(Constraints.DB_FIELD_CONTRAINT_TYPE)){
						constraintList = (ArrayList)currentColumn.Constraints[Constraints.DB_FIELD_CONTRAINT_TYPE];
						for(int i=0; i<constraintList.Count; i++){
							sql+=constraintList[i].ToString()+",";
						}
					}					
			}
			if(sql.Length>0)sql=sql.Substring(0,sql.Length-1);
			return(sql);
		}

		/// <summary>
		/// Obtient le code SQL des champs correspondant aux colonnes suivant les règles de contraintes
		/// </summary>
        /// <param name="columns">Liust of columns to treat</param>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
		public static string GetSqlConstraintFields(List<GenericColumnItemInformation> columns){
			string sql="";
			ArrayList constraintList=null;
			foreach(GenericColumnItemInformation currentColumn in columns){
				if(currentColumn.Constraints!=null && currentColumn.Constraints.Count>0)
					if(currentColumn.Constraints.ContainsKey(Constraints.DB_FIELD_CONTRAINT_TYPE)){
						constraintList = (ArrayList)currentColumn.Constraints[Constraints.DB_FIELD_CONTRAINT_TYPE];
						for(int i=0; i<constraintList.Count; i++){
							sql+=constraintList[i].ToString()+",";
						}
					}					
			}
			if(sql.Length>0)sql=sql.Substring(0,sql.Length-1);
			return(sql);
		}
		/// <summary>
		/// Obtient le code SQL des champs correspondant aux colonnes pour les requêtes plurimedia.
		/// Cette fonction est utilisée pour obtenir les champs logique pour une requête avec union 
		/// (les champs sont sans spécification de la table)
		/// </summary>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlFieldsWithoutTablePrefix(){
			string sql="";
			foreach(GenericColumnItemInformation currentColumn in _columns){
				if(currentColumn.GetSqlFieldIdWithoutTablePrefix()!=null && currentColumn.GetSqlFieldIdWithoutTablePrefix().Length>0)
				sql+=currentColumn.GetSqlFieldIdWithoutTablePrefix()+",";
				if(currentColumn.GetSqlFieldWithoutTablePrefix()!=null && currentColumn.GetSqlFieldWithoutTablePrefix().Length>0)
				sql+=currentColumn.GetSqlFieldWithoutTablePrefix()+",";
			}
			if(sql.Length>0)sql=sql.Substring(0,sql.Length-1);
			return(sql);
        }
        #endregion

        #region Order by
        /// <summary>
		/// Obtient le code SQL de la clause order correspondant aux colonnes
		/// </summary>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
        public string GetSqlOrderFields() {
            string sql = "";
            foreach (GenericColumnItemInformation currentColumn in _columns) {
                if (currentColumn.Constraints == null || currentColumn.Constraints.Count <= 0) {
                    if (currentColumn.GetSqlFieldForOrder() != null && currentColumn.GetSqlFieldForOrder().Length > 0)
                        sql += currentColumn.GetSqlFieldForOrder() + ",";

                    if (currentColumn.GetSqlIdFieldForOrder() != null && currentColumn.GetSqlIdFieldForOrder().Length > 0)
                        sql += currentColumn.GetSqlIdFieldForOrder() + ",";
                }
            }
            if (sql.Length > 0) sql = sql.Substring(0, sql.Length - 1);
            return (sql);
        }

		/// <summary>
		/// Obtient le code SQL de la clause order correspondant aux colonnes exceptées celles qui ont une équivalence
		/// avec le niveau de détail présenté en ligne.  
		/// </summary>
		/// <param name="detailLevelList">Identifiants de niveau de détail</param>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlOrderFields(ArrayList detailLevelList){
			string sql="";
			foreach(GenericColumnItemInformation currentColumn in _columns){
                if (detailLevelList == null || detailLevelList.Count == 0 || !detailLevelList.Contains(currentColumn.IdDetailLevelMatching)) {
                    if (currentColumn.Constraints == null || currentColumn.Constraints.Count <= 0) {
                        if (currentColumn.GetSqlFieldForOrder() != null && currentColumn.GetSqlFieldForOrder().Length > 0)
                            sql += currentColumn.GetSqlFieldForOrder() + ",";
                        if (currentColumn.GetSqlIdFieldForOrder() != null && currentColumn.GetSqlIdFieldForOrder().Length > 0)
                            sql += currentColumn.GetSqlIdFieldForOrder() + ",";
                    }
                }
			}
			if(sql.Length>0)sql=sql.Substring(0,sql.Length-1);
			return(sql);
		}

        /// <summary>
        /// Obtient le code SQL de la clause order correspondant aux colonnes exceptées celles qui ont une équivalence
        /// avec le niveau de détail présenté en ligne.  
        /// </summary>
        /// <param name="columns">Columns to include in the clause</param>
        /// <param name="detailLevelList">Identifiants de niveau de détail</param>
        /// <remarks>Ne termine pas par une virgule</remarks>
        /// <returns>Code SQL</returns>
        public static string GetSqlOrderFields(List<GenericColumnItemInformation> columns, ArrayList detailLevelList)
        {
            string sql = "";
            foreach (GenericColumnItemInformation currentColumn in columns)            
            {
                if (currentColumn.Id != GenericColumnItemInformation.Columns.associatedFile && currentColumn.Id != GenericColumnItemInformation.Columns.associatedFileMax && currentColumn.Id != GenericColumnItemInformation.Columns.poster && currentColumn.Id != GenericColumnItemInformation.Columns.visual)
                {
                    if (detailLevelList == null || detailLevelList.Count == 0 || !detailLevelList.Contains(currentColumn.IdDetailLevelMatching))
                    {
                        if (currentColumn.Constraints == null || currentColumn.Constraints.Count <= 0)
                        {
                            if (currentColumn.GetSqlFieldForOrder() != null && currentColumn.GetSqlFieldForOrder().Length > 0)
                                sql += currentColumn.GetSqlFieldForOrder() + ",";
                            if (currentColumn.GetSqlIdFieldForOrder() != null && currentColumn.GetSqlIdFieldForOrder().Length > 0)
                                sql += currentColumn.GetSqlIdFieldForOrder() + ",";
                        }
                    }
                }
            }
            if (sql.Length > 0) sql = sql.Substring(0, sql.Length - 1);
            return (sql);
        }
		
		/// <summary>
		/// Obtient le code SQL de la clause order correspondant aux  colonnes contraigantes
		/// </summary>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlConstraintOrderFields(){
			string sql="";
			ArrayList constraintList=null;
			foreach(GenericColumnItemInformation currentColumn in _columns){
				if(currentColumn.Constraints!=null && currentColumn.Constraints.Count>0)
					if(currentColumn.Constraints.ContainsKey(Constraints.DB_ORDER_CONTRAINT_TYPE)){
						constraintList = (ArrayList)currentColumn.Constraints[Constraints.DB_ORDER_CONTRAINT_TYPE];
						for(int i=0; i<constraintList.Count; i++){
							sql+=constraintList[i].ToString()+",";
						}
					}					
			}
			if(sql.Length>0)sql=sql.Substring(0,sql.Length-1);
			return(sql);
		}

		/// <summary>
		/// Obtient le code SQL de la clause order correspondant aux  colonnes contraigantes
		/// </summary>
		/// <remarks>Ne termine pas par une virgule</remarks>
        /// <param name="columns">List columns to include</param>
		/// <returns>Code SQL</returns>
		public static string GetSqlConstraintOrderFields(List<GenericColumnItemInformation> columns){
			string sql="";
			ArrayList constraintList=null;
			foreach(GenericColumnItemInformation currentColumn in columns){
				if(currentColumn.Constraints!=null && currentColumn.Constraints.Count>0)
					if(currentColumn.Constraints.ContainsKey(Constraints.DB_ORDER_CONTRAINT_TYPE)){
						constraintList = (ArrayList)currentColumn.Constraints[Constraints.DB_ORDER_CONTRAINT_TYPE];
						for(int i=0; i<constraintList.Count; i++){
							sql+=constraintList[i].ToString()+",";
						}
					}					
			}
			if(sql.Length>0)sql=sql.Substring(0,sql.Length-1);
			return(sql);
		}
		/// <summary>
		/// Obtient le code SQL de la clause order correspondant aux colonnes
		/// </summary>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlOrderFieldsWithoutTablePrefix(){
			string sql="";
			foreach(GenericColumnItemInformation currentColumn in _columns){
				if(currentColumn.GetSqlFieldForOrderWithoutTablePrefix()!=null && currentColumn.GetSqlFieldForOrderWithoutTablePrefix().Length>0)
				sql+=currentColumn.GetSqlFieldForOrderWithoutTablePrefix()+",";
				if(currentColumn.GetSqlIdFieldForOrderWithoutTablePrefix()!=null && currentColumn.GetSqlIdFieldForOrderWithoutTablePrefix().Length>0)
				sql+=currentColumn.GetSqlIdFieldForOrderWithoutTablePrefix()+",";
			}
			if(sql.Length>0)sql=sql.Substring(0,sql.Length-1);
			return(sql);
        }
        #endregion

        #region Group by
        /// <summary>
		/// Obtient le code SQL de la clause group by correspondant aux colonnes
		/// </summary>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlGroupByFields(){
			string sql="";
			foreach(GenericColumnItemInformation currentColumn in _columns){
                if (!currentColumn.IsSum && !currentColumn.IsCountDistinct && !currentColumn.IsMax)
                {
                    if (currentColumn.GetSqlIdFieldForGroupBy() != null && currentColumn.GetSqlIdFieldForGroupBy().Length > 0)
                        sql += currentColumn.GetSqlIdFieldForGroupBy() + ",";
                    if (currentColumn.GetSqlFieldForGroupBy() != null && currentColumn.GetSqlFieldForGroupBy().Length > 0)
                        sql += currentColumn.GetSqlFieldForGroupBy() + ",";
                }
			}
			if(sql.Length>0)sql=sql.Substring(0,sql.Length-1);
			return(sql);
		}

		/// <summary>
		/// Obtient le code SQL de la clause group by correspondant aux colonnes exceptées celles qui ont une équivalence
		/// avec le niveau de détail présenté en ligne.  
		/// </summary>
		/// <param name="detailLevelList">Identifiants de niveau de détail</param>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlGroupByFields(ArrayList detailLevelList){
			string sql="";
			foreach(GenericColumnItemInformation currentColumn in _columns){
                if (detailLevelList == null || detailLevelList.Count == 0 || !detailLevelList.Contains(currentColumn.IdDetailLevelMatching))
                {
                    if (!currentColumn.IsSum && !currentColumn.IsCountDistinct && !currentColumn.IsMax)
                    {
                        if (currentColumn.GetSqlIdFieldForGroupBy() != null && currentColumn.GetSqlIdFieldForGroupBy().Length > 0)
                            sql += currentColumn.GetSqlIdFieldForGroupBy() + ",";
                        if (currentColumn.GetSqlFieldForGroupBy() != null && currentColumn.GetSqlFieldForGroupBy().Length > 0)
                            sql += currentColumn.GetSqlFieldForGroupBy() + ",";
                    }
                }
			}
			if(sql.Length>0)sql=sql.Substring(0,sql.Length-1);
			return(sql);
		}

        /// <summary>
        /// Get SQL group by clause matching columns in parameters excepted ones included in detail levels
        /// </summary>
        /// <param name="columns">List of columns to join</param>
        /// <param name="detailLevelList">Identifiants de niveau de détail</param>
        /// <remarks>Ne termine pas par une virgule</remarks>
        /// <returns>Code SQL</returns>
        public static string GetSqlGroupByFields(List<GenericColumnItemInformation> columns, ArrayList detailLevelList)
        {
            string sql = "";
            foreach (GenericColumnItemInformation currentColumn in columns)
            {
                if (!currentColumn.IsSum && !currentColumn.IsCountDistinct && !currentColumn.IsMax)
                {

                    if (currentColumn.Constraints == null || currentColumn.Constraints.Count <= 0)
                    {
                        if (detailLevelList == null || detailLevelList.Count == 0 || !detailLevelList.Contains(currentColumn.IdDetailLevelMatching))
                        {
                            if (currentColumn.GetSqlIdFieldForGroupBy() != null && currentColumn.GetSqlIdFieldForGroupBy().Length > 0)
                                sql += currentColumn.GetSqlIdFieldForGroupBy() + ",";
                            if (currentColumn.GetSqlFieldForGroupBy() != null && currentColumn.GetSqlFieldForGroupBy().Length > 0)
                                sql += currentColumn.GetSqlFieldForGroupBy() + ",";
                        }
                    }
                }
            }
            if (sql.Length > 0) sql = sql.Substring(0, sql.Length - 1);
            return (sql);
        }
		
		/// <summary>
		/// Obtient le code SQL de la clause group by correspondant aux  colonnes contraignantes
		/// </summary>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlConstraintGroupByFields(){
			string sql="";
			ArrayList constraintList=null;
			foreach(GenericColumnItemInformation currentColumn in _columns){
                if (!currentColumn.IsSum && !currentColumn.IsCountDistinct && !currentColumn.IsMax)
                {
                    if (currentColumn.Constraints != null && currentColumn.Constraints.Count > 0)
                        if (currentColumn.Constraints.ContainsKey(Constraints.GROUP_BY_CONTRAINT_TYPE))
                        {
                            constraintList = (ArrayList)currentColumn.Constraints[Constraints.GROUP_BY_CONTRAINT_TYPE];
                            for (int i = 0; i < constraintList.Count; i++)
                            {
                                sql += constraintList[i].ToString() + ",";
                            }
                        }
                }
			}
			if(sql.Length>0)sql=sql.Substring(0,sql.Length-1);
			return(sql);
		}

		/// <summary>
		/// Obtient le code SQL de la clause group by correspondant aux  colonnes contraignantes
		/// </summary>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
		public static string GetSqlConstraintGroupByFields(List<GenericColumnItemInformation> columns){
			string sql="";
			ArrayList constraintList=null;
			foreach(GenericColumnItemInformation currentColumn in columns){
                if (!currentColumn.IsSum && !currentColumn.IsCountDistinct && !currentColumn.IsMax)
                {

                    if (currentColumn.Constraints != null && currentColumn.Constraints.Count > 0)
                        if (currentColumn.Constraints.ContainsKey(Constraints.GROUP_BY_CONTRAINT_TYPE))
                        {
                            constraintList = (ArrayList)currentColumn.Constraints[Constraints.GROUP_BY_CONTRAINT_TYPE];
                            for (int i = 0; i < constraintList.Count; i++)
                            {
                                sql += constraintList[i].ToString() + ",";
                            }
                        }
                }
			}
			if(sql.Length>0)sql=sql.Substring(0,sql.Length-1);
			return(sql);
		}

		/// <summary>
		/// Obtient le code SQL de la clause group by correspondant aux colonnes
		/// </summary>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlGroupByFieldsWithoutTablePrefix(){
			string sql="";
			foreach(GenericColumnItemInformation currentColumn in _columns){
                if (!currentColumn.IsSum && !currentColumn.IsCountDistinct && !currentColumn.IsMax)
                {

                    if (currentColumn.GetSqlIdFieldForGroupByWithoutTablePrefix() != null && currentColumn.GetSqlIdFieldForGroupByWithoutTablePrefix().Length > 0)
                        sql += currentColumn.GetSqlIdFieldForGroupByWithoutTablePrefix() + ",";
                    if (currentColumn.GetSqlFieldForGroupByWithoutTablePrefix() != null && currentColumn.GetSqlFieldForGroupByWithoutTablePrefix().Length > 0)
                        sql += currentColumn.GetSqlFieldForGroupByWithoutTablePrefix() + ",";
                }
			}
			if(sql.Length>0)sql=sql.Substring(0,sql.Length-1);
			return(sql);
        }
        #endregion

        #region From
        /// <summary>
		/// Obtient le code SQL des tables correspondant aux colonnes
		/// </summary>
		/// <param name="dbSchemaName">Schema de la base de données à utiliser</param>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlTables(string dbSchemaName){
			if(dbSchemaName==null || dbSchemaName.Length==0)throw(new ArgumentNullException("Parameter dbSchemaName is invalid"));
			string sql="";
			foreach(GenericColumnItemInformation currentColumn in _columns){
				if(currentColumn.DataBaseTableName!=null) sql+=dbSchemaName+"."+currentColumn.DataBaseTableName+" "+currentColumn.DataBaseTableNamePrefix+",";
			}
			if(sql.Length>0)sql=sql.Substring(0,sql.Length-1);
			return(sql);
		}

		/// <summary>
		/// Obtient le code SQL des tables correspondant aux colonnes
		/// </summary>
		/// <param name="dbSchemaName">Schema de la base de données à utiliser</param>
		/// <param name="detailLevelList">Identifiants de niveau de détail</param>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlTables(string dbSchemaName,ArrayList detailLevelList){
			if(dbSchemaName==null || dbSchemaName.Length==0)throw(new ArgumentNullException("Parameter dbSchemaName is invalid"));
			string sql="";
			foreach(GenericColumnItemInformation currentColumn in _columns){
				if(detailLevelList==null ||  detailLevelList.Count==0 || !detailLevelList.Contains(currentColumn.IdDetailLevelMatching))
				if(currentColumn.DataBaseTableName!=null) sql+=dbSchemaName+"."+currentColumn.DataBaseTableName+" "+currentColumn.DataBaseTableNamePrefix+",";
			}
			if(sql.Length>0)sql=sql.Substring(0,sql.Length-1);
			return(sql);
		}

        /// <summary>
        /// Get SQL for "from" clause
        /// </summary>
        /// <param name="dbSchemaName">Data Base schema to use</param>
        /// <param name="detailLevelList">Id of included detail levels</param>
        /// <param name="columns">Columns to include</param>
        /// <remarks>Don't stop with a coma</remarks>
        /// <returns>SQL Code</returns>
        public static string GetSqlTables(string dbSchemaName, List<GenericColumnItemInformation> columns, ArrayList detailLevelList)
        {
            if (dbSchemaName == null || dbSchemaName.Length == 0) throw (new ArgumentNullException("Parameter dbSchemaName is invalid"));
            string sql = "";
            foreach (GenericColumnItemInformation currentColumn in columns)
            {
                if (detailLevelList == null || detailLevelList.Count == 0 || !detailLevelList.Contains(currentColumn.IdDetailLevelMatching))
                    if (currentColumn.DataBaseTableName != null) sql += dbSchemaName + "." + currentColumn.DataBaseTableName + " " + currentColumn.DataBaseTableNamePrefix + ",";
            }
            if (sql.Length > 0) sql = sql.Substring(0, sql.Length - 1);
            return (sql);
        }

		/// <summary>
		/// Obtient le code SQL des tables correspondant aux colonnes
		/// </summary>
		/// <param name="dbSchemaName">Schema de la base de données à utiliser</param>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlConstraintTables(string dbSchemaName){
			if(dbSchemaName==null || dbSchemaName.Length==0)throw(new ArgumentNullException("Parameter dbSchemaName is invalid"));
			string sql="";

			ArrayList constraintList=null;
			foreach(GenericColumnItemInformation currentColumn in _columns){
				if(currentColumn.Constraints!=null && currentColumn.Constraints.Count>0){
					if(currentColumn.Constraints.ContainsKey(Constraints.DB_TABLE_CONTRAINT_TYPE)){
						constraintList = (ArrayList)currentColumn.Constraints[Constraints.DB_TABLE_CONTRAINT_TYPE];
						for(int i=0; i<constraintList.Count; i++){
							sql+=constraintList[i].ToString()+",";
						}
					}	
				}
			}
			if(sql.Length>0)sql=sql.Substring(0,sql.Length-1);
			return(sql);
		}

        /// <summary>
        /// Obtient le code SQL des tables correspondant aux colonnes
        /// </summary>
        /// <param name="dbSchemaName">Schema de la base de données à utiliser</param>
        /// <param name="columns">List of columns to include</param>
        /// <remarks>Ne termine pas par une virgule</remarks>
        /// <returns>Code SQL</returns>
        public static string GetSqlConstraintTables(string dbSchemaName, List<GenericColumnItemInformation> columns)
        {
            if (dbSchemaName == null || dbSchemaName.Length == 0) throw (new ArgumentNullException("Parameter dbSchemaName is invalid"));
            string sql = "";

            ArrayList constraintList = null;
            foreach (GenericColumnItemInformation currentColumn in columns)
            {
                if (currentColumn.Constraints != null && currentColumn.Constraints.Count > 0)
                {
                    if (currentColumn.Constraints.ContainsKey(Constraints.DB_TABLE_CONTRAINT_TYPE))
                    {
                        constraintList = (ArrayList)currentColumn.Constraints[Constraints.DB_TABLE_CONTRAINT_TYPE];
                        for (int i = 0; i < constraintList.Count; i++)
                        {
                            sql += constraintList[i].ToString() + ",";
                        }
                    }
                }
            }
            if (sql.Length > 0) sql = sql.Substring(0, sql.Length - 1);
            return (sql);
        }
        #endregion

        #region Join
        /// <summary>
		/// Obtient le code SQL des jointures correspondant aux colonnes
		/// </summary>
		/// <remarks>Début par And</remarks>
		/// <param name="languageId">Langue à afficher</param>
		/// <param name="dataTablePrefix">Préfixe de la table de données sur laquelle on fait la jointure</param>
		/// <returns>Code SQL</returns>
		public string GetSqlJoins(int languageId,string dataTablePrefix){
			if(dataTablePrefix==null || dataTablePrefix.Length==0)throw(new ArgumentNullException("Parameter dataTablePrefix is invalid"));
			string sql="";
			string sqlOperation="";
			foreach(GenericColumnItemInformation currentColumn in _columns){
				if(currentColumn.DataBaseTableName!=null ){
					if(currentColumn.SqlOperation.Equals("leftOuterJoin"))sqlOperation = "(+)=";
					else if (currentColumn.SqlOperation.Equals("rightOuterJoin"))sqlOperation = "=(+)";
					else sqlOperation = "=";
					if(currentColumn.DbRelatedTablePrefixeForJoin!=null && currentColumn.DbRelatedTablePrefixeForJoin.Length>0)
						sql+=" and "+currentColumn.DataBaseTableNamePrefix+"."+currentColumn.DataBaseIdField+sqlOperation+currentColumn.DbRelatedTablePrefixeForJoin+"."+currentColumn.DataBaseIdField;
					else sql+=" and "+currentColumn.DataBaseTableNamePrefix+"."+currentColumn.DataBaseIdField+sqlOperation+dataTablePrefix+"."+currentColumn.DataBaseIdField;

                    if (currentColumn.UseLanguageRule)
                    {
                        sql += " and " + currentColumn.DataBaseTableNamePrefix + ".id_language" + sqlOperation + languageId.ToString();
                    }

                    if (currentColumn.UseActivationRule)
                    {
                        if (currentColumn.SqlOperation.Equals("leftOuterJoin"))
                            sql += " and " + currentColumn.DataBaseTableNamePrefix + ".activation(+)<" + ActivationValues.UNACTIVATED;
                        else
                            sql += " and " + currentColumn.DataBaseTableNamePrefix + ".activation<" + ActivationValues.UNACTIVATED;
                    }
				}
				sqlOperation="";
			}
			return(sql);
		}
		
		/// <summary>
		/// Obtient le code SQL des jointures correspondant aux colonnes exceptés celles qui ont une équivalence
		/// avec le niveau de détail présenté en ligne.  
		/// </summary>
		/// <remarks>Début par And</remarks>
		/// <param name="languageId">Langue à afficher</param>
		/// <param name="dataTablePrefix">Préfixe de la table de données sur laquelle on fait la jointure</param>
		/// <param name="detailLevelList">Identifiants de niveau de détail</param>
		/// <returns>Code SQL</returns>
		public string GetSqlJoins(int languageId,string dataTablePrefix,ArrayList detailLevelList){
			if(dataTablePrefix==null || dataTablePrefix.Length==0)throw(new ArgumentNullException("Parameter dataTablePrefix is invalid"));
			string sql="";
			string sqlOperation="";
			foreach(GenericColumnItemInformation currentColumn in _columns){
				if(currentColumn.DataBaseTableName!=null && (detailLevelList==null ||  detailLevelList.Count==0 || !detailLevelList.Contains(currentColumn.IdDetailLevelMatching))){
					if(currentColumn.SqlOperation.Equals("leftOuterJoin"))sqlOperation = "(+)=";
					else if (currentColumn.SqlOperation.Equals("rightOuterJoin"))sqlOperation = "=(+)";
					else sqlOperation = "=";
					if(currentColumn.DbRelatedTablePrefixeForJoin!=null && currentColumn.DbRelatedTablePrefixeForJoin.Length>0)
						sql+=" and "+currentColumn.DataBaseTableNamePrefix+"."+currentColumn.DataBaseIdField+sqlOperation+currentColumn.DbRelatedTablePrefixeForJoin+"."+currentColumn.DataBaseIdField;
					else sql+=" and "+currentColumn.DataBaseTableNamePrefix+"."+currentColumn.DataBaseIdField+sqlOperation+dataTablePrefix+"."+currentColumn.DataBaseIdField;
                    if (currentColumn.UseLanguageRule)
                    {
                        sql += " and " + currentColumn.DataBaseTableNamePrefix + ".id_language" + sqlOperation + languageId.ToString();
                    }
                    if (currentColumn.UseActivationRule)
                    {
                        if (currentColumn.SqlOperation.Equals("leftOuterJoin"))
                            sql += " and " + currentColumn.DataBaseTableNamePrefix + ".activation(+)<" + ActivationValues.UNACTIVATED;
                        else
                            sql += " and " + currentColumn.DataBaseTableNamePrefix + ".activation<" + ActivationValues.UNACTIVATED;
                    }
				}
			}
			return(sql);
		}

		/// <summary>
		/// Obtient le code SQL des jointures correspondant aux colonnes exceptés celles qui ont une équivalence
		/// avec le niveau de détail présenté en ligne.  
		/// </summary>
		/// <remarks>Début par And</remarks>
		/// <param name="languageId">Langue à afficher</param>
        /// <param name="columns">Columns to include</param>
		/// <param name="dataTablePrefix">Préfixe de la table de données sur laquelle on fait la jointure</param>
		/// <param name="detailLevelList">Identifiants de niveau de détail</param>
		/// <returns>Code SQL</returns>
        public static string GetSqlJoins(int languageId, string dataTablePrefix, List<GenericColumnItemInformation> columns, ArrayList detailLevelList)
        {
			if(dataTablePrefix==null || dataTablePrefix.Length==0)throw(new ArgumentNullException("Parameter dataTablePrefix is invalid"));
			string sql="";
			string sqlOperation="";
			foreach(GenericColumnItemInformation currentColumn in columns){
				if(currentColumn.DataBaseTableName!=null && (detailLevelList==null ||  detailLevelList.Count==0 || !detailLevelList.Contains(currentColumn.IdDetailLevelMatching))){
					if(currentColumn.SqlOperation.Equals("leftOuterJoin"))sqlOperation = "(+)=";
					else if (currentColumn.SqlOperation.Equals("rightOuterJoin"))sqlOperation = "=(+)";
					else sqlOperation = "=";
					if(currentColumn.DbRelatedTablePrefixeForJoin!=null && currentColumn.DbRelatedTablePrefixeForJoin.Length>0)
						sql+=" and "+currentColumn.DataBaseTableNamePrefix+"."+currentColumn.DataBaseIdField+sqlOperation+currentColumn.DbRelatedTablePrefixeForJoin+"."+currentColumn.DataBaseIdField;
					else sql+=" and "+currentColumn.DataBaseTableNamePrefix+"."+currentColumn.DataBaseIdField+sqlOperation+dataTablePrefix+"."+currentColumn.DataBaseIdField;
                    if (currentColumn.UseLanguageRule)
                    {
                        sql += " and " + currentColumn.DataBaseTableNamePrefix + ".id_language" + sqlOperation + languageId.ToString();
                    }
                    if (currentColumn.UseActivationRule)
                    {
                        if (currentColumn.SqlOperation.Equals("leftOuterJoin"))
                            sql += " and " + currentColumn.DataBaseTableNamePrefix + ".activation(+)<" + ActivationValues.UNACTIVATED;
                        else
                            sql += " and " + currentColumn.DataBaseTableNamePrefix + ".activation<" + ActivationValues.UNACTIVATED;
                    }
				}
			}
			return(sql);
		}
		/// <summary>
		/// Obtient le code SQL des jointures correspondant aux colonnes suivant les contraintes métiers
		/// </summary>
		/// <remarks>Début par And</remarks>	
		/// <returns>Code SQL</returns>
		public string GetSqlContraintJoins(){
			
			string sql="";
			ArrayList constraintList=null;
			foreach(GenericColumnItemInformation currentColumn in _columns){
				if(currentColumn.Constraints!=null && currentColumn.Constraints.Count>0){
					if(currentColumn.Constraints.ContainsKey(Constraints.DB_JOIN_CONTRAINT_TYPE)){
						constraintList = (ArrayList)currentColumn.Constraints[Constraints.DB_JOIN_CONTRAINT_TYPE];
						for(int i=0; i<constraintList.Count; i++){
							sql+=" and "+constraintList[i].ToString();
						}
					}	
				}
			}
			
			return(sql);
		}
		/// <summary>
		/// Obtient le code SQL des jointures correspondant aux colonnes suivant les contraintes métiers
		/// </summary>
        /// <param name="columns">List of columns to treat</param>
		/// <remarks>Début par And</remarks>	
		/// <returns>Code SQL</returns>
		public static string GetSqlContraintJoins(List<GenericColumnItemInformation> columns){
			
			string sql="";
			ArrayList constraintList=null;
			foreach(GenericColumnItemInformation currentColumn in columns){
				if(currentColumn.Constraints!=null && currentColumn.Constraints.Count>0){
					if(currentColumn.Constraints.ContainsKey(Constraints.DB_JOIN_CONTRAINT_TYPE)){
						constraintList = (ArrayList)currentColumn.Constraints[Constraints.DB_JOIN_CONTRAINT_TYPE];
						for(int i=0; i<constraintList.Count; i++){
							sql+=" and "+constraintList[i].ToString();
						}
					}	
				}
			}
			
			return(sql);
		}		
        #endregion			

		#region UI
		/// <summary>
		/// Obtient le nom traduit d'une colonne
		/// </summary>
		/// <param name="column">Niveau</param>
		/// <param name="languageId">Langue</param>
		/// <returns>Nom traduit</returns>
		public string GetColumnText(int column,int languageId){
			if(column<1)throw(new ArgumentException("invalid parameter column"));
			return(GestionWeb.GetWebWord(((GenericColumnItemInformation)_columns[column-1]).WebTextId,languageId));
		}

		/// <summary>
		/// Obtient le libellé de la colonne
		/// </summary>
		/// <param name="languageId">Langue</param>
		/// <returns>Libellé de la colonne</returns>
		public string GetLabel(int languageId){
			string ui="";
			for(int column=1;column<=_columns.Count;column++){
				ui+=GetColumnText(column,languageId)+@"\";
			}
			if(ui.Length>1)ui=ui.Substring(0,ui.Length-1);
			return(ui);
		}




		#endregion
		
		/// <summary>
		/// Indique si une colonne est contenu dans la liste des colonnes
		/// </summary>
		/// <param name="columnId">Identifiant de la colonne</param>
		/// <returns>True si oui, sinon false</returns>
		public bool ContainColumnItem(GenericColumnItemInformation.Columns columnId){
			foreach(GenericColumnItemInformation currentColumn in _columns){
				if(currentColumn.Id==columnId)return(true);
			}
			return(false);
		}

		
		/// <summary>
		/// Obtient la position de la colonne
		/// si la colonne n'existe pas la fonction retourne 0
		/// </summary>
		/// <param name="columnId">Identifiant de la colonne</param>
		/// <returns>la position de la colonne</returns>
		public int GetColumnRankColumnItem(GenericColumnItemInformation.Columns columnId){
			int rank=0;
			foreach(GenericColumnItemInformation currentColumn in _columns){
				rank++;
				if(currentColumn.Id==columnId)return(rank);
			}
			return(0);
		}

		/// <summary>
		/// Indique si la liste en paramètre à les mêmes niveaux de détail
		/// </summary>
		/// <param name="toTest">Liste à tester</param>
		/// <returns>True si elle contient les mêmes colonnes, false sinon</returns>
		public bool EqualColumnItems(GenericColumns toTest){
			if(this.GetNbColumns!=toTest.GetNbColumns)return(false);
			for(int i=0; i<toTest.GetNbColumns;i++){
				if(((GenericColumnItemInformation.Columns)this.ColumnIds[i])!=((GenericColumnItemInformation.Columns)toTest.ColumnIds[i]))return(false);
			}
			return(true);

		}

		/// <summary>
		/// Surcharge de la méthode ToString d'Object.
		/// Cette méthode est utilisée pour le debugage
		/// </summary>
		/// <remarks>La langue est le français</remarks>
		/// <returns>La chaîne représentant le niveau de détail</returns>
		public override string ToString() {
			return(GetLabel(TNS.AdExpress.Constantes.DB.Language.FRENCH));
		}
		#endregion

        #endregion
    }
}
