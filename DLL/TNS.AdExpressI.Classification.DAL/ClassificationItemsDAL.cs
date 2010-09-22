#region Information
/*
 * Author : D. Mussuma
 * Created on : 21/07/2009
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web.Navigation;
using WebNavigation = TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.DataBaseDescription;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using CustomerRightConstante = TNS.AdExpress.Constantes.Customer.Right;
using VehicleClassificationCst = TNS.AdExpress.Constantes.Classification.DB.Vehicles.names;
using TNS.Classification.Universe;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core.Utilities;

namespace TNS.AdExpressI.Classification.DAL {
	/// <summary>
    /// This class generates search SQL queries on a classification brand. Allow customer to view and select the items 
    /// found in the different hierarchical levels of the chosen brand.
    /// The items displayed depend on user classification rights.  
	/// </summary>
	public class ClassificationItemsDAL : EngineDAL {

        /// <summary>
        /// Current classification brand (product or vehicle)
        /// </summary>
        protected TNS.Classification.Universe.Dimension _dimension;

		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="session">User session</param>
        public ClassificationItemsDAL(WebSession session, TNS.Classification.Universe.Dimension dimension)
			: base(session) {
                _dimension = dimension;
		}

		#endregion

		#region GetItems
        /// <summary>
        ///  Search function for Product or vehicle classification Items. The data will be filter 
        ///  with customer classification rights. It means product or media restriction.
        ///  <example>
        /// To search data for vehicle level :
        /// <code>
        /// Select distinct wp.id_media as id_item, wp.media  as item
        /// 
        /// //Query on View of vehicle classification
        /// from all_media_33 wp
        /// 
        /// //Search by label
        /// where wp.media like '%CNN%'
        /// 
        /// //media's rights
        /// and wp.id_vehicle in (1,2,3)
        /// 
        /// ...
        /// 
        /// //Order by vehicle label
        /// order by media
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="classificationLevelLabel">Label of classification level. For example, for the classification level
        /// product, the label will be "product". It corresponds also to the current classification level's table.</param>
        /// <param name="wordToSearch">Key word to search</param>
        /// <remarks>     
        /// - We use the parameter "classificationLevelLabel" to build the SQL query fields as follows :
        ///  <code>
        ///  public virtual DataSet GetItems(string classificationLevelLabel, ...){
        ///  
        /// ...
        /// //Set fields of the query with the current classification level
        /// sql.AppendFormat(" select distinct pr.id_{0} as id_item, pr.{0} as item ", classificationLevelLabel);
        ///  //Set Table of the query with the current classification level      
        /// sql.AppendFormat(" from {0} pr ",classificationLevelLabel)
        /// sql.AppendFormat(" order by  {0} ", classificationLevelLabel);
        /// ...
        /// }
        /// </code>
        /// 
        /// Then if parameter "classificationLevelLabel" is equal to "product " the query string will be as follows :
        /// <code>
        /// select distinct pr.id_product as id_item, pr.product as item
        /// from product pr 
        /// order by product
        /// </code>
        /// </remarks>        
        /// <returns>Data set with data table[id_item,item] : identifer and label of a level of brand classification</returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.ClassificationItemsDALException">Throw exception when error occurs during 
        /// execution or building of the query</exception>
        public virtual DataSet GetItems(string classificationLevelLabel, string wordToSearch)
        {

			#region Tables initilization
			View oView = null;
			string classificationRight = "";
			bool useView = true;

			try {
                /* The search of classification items is done on a View of product or vehicle classification.
                 * In fact, if the dimension (classification brand) is product, the search will be done on a view called "ADEXPR03.ALL_PRODUCT_33".
                 * ADEXPR03 corresponds to the scheme of the view and ALL_PRODUCT_33 the label of the view. 
                 * The number 33 corresponds to the identifier of the country, here France.
                 *  If the dimension is vehicle (classification brand), the search will be done on a view called "ADEXPR03.ALL_MEDIA_33".
                 * ALL_MEDIA_33 corresponds to the label of the view.*/
                oView = GetView(_dimension);

                /* Obtains customer's rights for product brand classification   */
				classificationRight = GetRights(_dimension);

                /*To improve the performance of research. The target table varies according to the rights 
                 * of clients. For example, for a given classification level, if no rights is applied, 
                 * the corresponding table  of classification will be targeted instead of view. */
                useView = CanUseView(_dimension, classificationRight);
			}
			catch (System.Exception err) {
				throw (new Exceptions.ClassificationItemsDALException("Impossible to get view names or rights", err));
			}
			#endregion

			#region Building sql query 
			StringBuilder sql = new StringBuilder(1000);

            /*SELECT Clause : 
            * The variable "classificationLevelLabel" correspond to the table of level brand classification.
             * The first column is the identifier of the classification level item.
             * Th seconde column is the label of trhe classification level item.*/
            sql.AppendFormat(" select distinct wp.id_{0} as id_item, wp.{0} as item ", classificationLevelLabel);
			
            /*FROM clause : Obtains the targets tables of the query*/
            GetFromClause(sql, oView, classificationLevelLabel, useView);

            //Key word allows to search for classication items via labels 			
			wordToSearch = wordToSearch.ToUpper().Replace("'", " ");
			wordToSearch = wordToSearch.ToUpper().Replace("*", "%");
			wordToSearch = wordToSearch.Trim() + "%";

            /*Query conditions */
            sql.AppendFormat(" where upper(wp.{0}) like upper('{1}')", classificationLevelLabel, wordToSearch);

            /*Query tables joins */
            GetJointClause(sql, oView, classificationLevelLabel, _dimension, classificationRight, useView);

            /*Query orders fields by items' labels */
            sql.AppendFormat(" order by  {0}", classificationLevelLabel);
			#endregion

			#region Excuting sql query
			try {
                //Excuting sql query
				return GetSource().Fill(sql.ToString());
			}
			catch (System.Exception e) {
				throw new Exceptions.ClassificationItemsDALException(e.Message, e);
			}
			#endregion
		}

        /// <summary>
        ///  Search function for Product or vehicle classification Items. The data will be filter 
        ///  with customer classification rights. It means product or media restriction.
        ///  
        /// The parameter "classificationLevelLabel" corresponds to classification level label
        /// where items must be included in SELECT clause.
        /// 
        /// The parameters "selectedClassificationLevelIds" and "selectedClassificationLevelLabel"
        /// correspond respectively to identifier list and the label of classification level  selected by the customer.
        /// It uses to filter data of the result level ( "classificationLevelLabel").
        /// 
        ///  To build the SQL quer, the preceding parameters are used as follows :
        ///  <example>              
        ///  <code>
        ///  public virtual DataSet GetItems(string classificationLevelLabel, string selectedClassificationLevelIds, string selectedClassificationLevelLabel){
        ///  
        /// View oVioew = ...//Get classification brand view : all_product_33 or all_media_44 (each one contains all items of its classification) 
        /// ...
        /// //Set fields of the query with the current classification level
        /// sql.AppendFormat(" select distinct pr.id_{0} as id_item, pr.{0} as item ", classificationLevelLabel);
        /// 
        ///   /*FROM clause : Obtains the targets tables of the query according to the web site data language*/
        ///  sql.AppendFormat(" from {0}{1} wp", oView.Sql,_session.DataLanguage.ToString());
        /// 
        ///  /*Query conditions */ 
        ///    * Restriction on the classification items selected by the customer*/
        ///	    sql.AppendFormat(" where wp.id_{0} in ( {1} ) ",selectedClassificationLevelLabel,selectedClassificationLevelIds);
        /// ...
        /// 
        ///  /*Query orders fields by items' labels */
        ///     sql.AppendFormat(" order by {0} ", classificationLevelLabel);
        /// }
        /// </code>
        /// 
        /// Then if parameters of the method are :
        ///  <code>
        ///  //Current classification level is product
        /// string classificationLevelLabel = "product";
        ///
        /// //If user has selected advertiser level
        /// string  selectedClassificationLevelLabel="advertiser";
        /// string selectedClassificationLevelIds ="1,35,56,35";
        ///  
        ///  </code>
        ///  
        /// The query string will be as follows :       
        /// <code>
        /// 
        /// //Get product ID and label
        /// Select distinct wp.id_product as id_item, wp.product  as item
        /// 
        /// //Query on product classification View
        /// from all_product_33 wp
        /// 
        /// //Filter by advertiser to obtain associated product
        /// where wp.advertiser in (1,35,56,35) 
        /// 
        /// //product's rights      
        /// ...
        /// 
        /// //Order by product label
        /// order by product
        /// </code>
        /// </summary>
        /// <param name="classificationLevelLabel">Label of classification level. For example, for the classification level
        /// product, the label will be "product". It corresponds also to the current classification level's table.</param>
        /// <param name="selectedClassificationLevelIds"> Selected (by user) classification level identifiers list (identifers separated by comma). These identifiers are used
        /// to filter the data.</param>
        /// <param name="selectedClassificationLevelLabel">Selected (by user) classification level label. This lable is used as field
        /// to filter the data as follows :         
        /// </param>
        /// <returns>Data set with data table[id_item,item] : identifer and label of a level of brand classification</returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.ClassificationItemsDALException">Throw exception when error occurs during 
        /// execution or building of the query</exception>
        public virtual DataSet GetItems(string classificationLevelLabel, string selectedClassificationLevelIds, string selectedItemTableName)
        {

			#region Tables initilization
			View oView = null;
			string classificationRight = "";
			StringBuilder sql = new StringBuilder(1000);

			try {
                /* The search of classification items is done on a View of product or vehicle classification.
                 * In fact, if the dimension (classification brand) is product, the search will be done on a view called "ADEXPR03.ALL_PRODUCT_33".
                 * ADEXPR03 corresponds to the scheme of the view and ALL_PRODUCT_33 the label of the view. 
                 * The number 33 corresponds to the identifier of the country, here France.
                 *  If the dimension is vehicle (classification brand), the search will be done on a view called "ADEXPR03.ALL_MEDIA_33".
                 * ALL_MEDIA_33 corresponds to the label of the view.*/
                oView = GetView(_dimension);

                /* Obtains customer's rights for product brand classification   */
				classificationRight = GetRights(_dimension);
			}
			catch (System.Exception err) {
				throw (new Exceptions.ClassificationItemsDALException("Impossible to get view names or rights", err));
			}
			#endregion

			#region Building sql query
            /*SELECT Clause : 
            * The variable "table" correspond to the table of level brand classification.
             * The first column is the identifier of the classification level item.
             * Th seconde column is the label of trhe classification level item.*/
            sql.AppendFormat(" select distinct wp.id_{0} as id_item, wp.{0} as item ", classificationLevelLabel);

             /*FROM clause : Obtains the targets tables of the query according to the web site data language*/
			 sql.AppendFormat(" from {0}{1} wp", oView.Sql,_session.DataLanguage.ToString());

             /*Query conditions */ 
            /* Restriction on the classification items selected by the customer*/
             sql.AppendFormat(" where wp.id_{0} in ( {1} ) ", selectedItemTableName, selectedClassificationLevelIds);

			//Defines customer classification's rihts
			if (classificationRight != null && classificationRight.Length > 0) {
				sql.Append(classificationRight);
			}
			
            /*If the current module is Media Schedule the data must be filtered 
             * on the medias selected by the customer*/
			switch (_module.Id) {
				case WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA:
					if (_dimension == Dimension.media)
						sql.AppendFormat(" and wp.id_vehicle in ({0}) ", _session.GetSelection(_session.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess));
					break;
			}
            
            /*Query orders fields by items' labels */
            sql.AppendFormat(" order by {0} ", classificationLevelLabel);
			#endregion

			#region Excution of query
			try {
                //Excuting sql query
				return GetSource().Fill(sql.ToString());
			}
			catch (System.Exception e) {
				throw new Exceptions.ClassificationItemsDALException(e.Message, e);
			}
			#endregion

		}


        /// <summary>
        ///  Search function for Product or vehicle classification Items. The data will be filter 
        ///  with customer classification rights (it means product or media restriction) and identiifer 
        ///  list of classification level's items.
        /// <remarks>     
        /// - We use the parameter "classificationLevelLabel" to build the SQL query fields as follows :
        ///  <code>
        ///  public virtual DataSet GetItems(string classificationLevelLabel,, string idList){
        ///  
        /// ...
        /// //Set fields of the query with the current classification level
        /// sql.AppendFormat(" select distinct pr.id_{0} as id_item, pr.{0} as item ", classificationLevelLabel);
        /// 
        ///  //Set Table of the query with the current classification level      
        /// sql.AppendFormat(" from {0} pr ",classificationLevelLabel)
        /// 
        /// /*Query conditions */
        ///   /* Restriction on the classification items selected by the customer*/
        /// sql.AppendFormat(" where wp.id_{0} in ({1})", classificationLevelLabel, idList);
        /// 
        /// //ordr fields
        /// sql.AppendFormat(" order by  {0} ", classificationLevelLabel);
        /// ...
        /// }
        /// </code>      
        /// </remarks>      
        ///  <example>
        /// To find vehicle level items with identifier list (5,56,59,56) the query string generated will be :
        /// <code>
        /// Select distinct wp.id_media as id_item, wp.media  as item
        /// 
        /// //Query on View of vehicle classification
        /// from all_media_33 wp
        /// 
        /// //Filtering with vehicles ID
        /// where wp.id_media in (5,56,59,56) 
        /// 
        /// //media's rights
        /// and wp.id_vehicle in (1,2,3)
        /// 
        /// ...
        /// 
        /// //Order by vehicle label
        /// order by media
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="classificationLevelLabel">Label of classification level. For example, for the classification level
        /// product, the label will be "product". It corresponds also to the current classification level's table.</param>
        ///<param name="selectedClassificationLevelIds"> Selected (by user) classification level identifiers list (identifers separated by comma). Identifiers are used
        /// to filter the data.</param>
        /// <returns>Data set with data table[id_item,item] : identifer and label of a level of brand classification</returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.ClassificationItemsDALException">Throw exception when error occurs during 
        /// execution or building of the query</exception>
        public virtual DataSet GetSelectedItems(string classificationLevelLabel, string idList)
        {

			#region Tables initilization
			View oView = null;
			string classificationRight = "";
			bool useView = true;

			try {
                /* The search of classification items is done on a View of product or vehicle classification.
               * In fact, if the dimension (classification brand) is product, the search will be done on a view called "ADEXPR03.ALL_PRODUCT_33".
               * ADEXPR03 corresponds to the scheme of the view and ALL_PRODUCT_33 the label of the view. 
               * The number 33 corresponds to the identifier of the country, here France.
               *  If the dimension is vehicle (classification brand), the search will be done on a view called "ADEXPR03.ALL_MEDIA_33".
               * ALL_MEDIA_33 corresponds to the label of the view.*/
                oView = GetView(_dimension);

                /* Obtains customer's rights for product brand classification   */
				classificationRight = GetRights(_dimension);

                /*To improve the performance of research. The target table varies according to the rights 
               * of clients. For example, for a given classification level, if no rights is applied, 
               * the corresponding table  of classification will be targeted instead of view. */
				useView = CanUseView(_dimension, classificationRight);
			}
			catch (System.Exception err) {
				throw (new Exceptions.ClassificationItemsDALException("Impossible to get view names or rights", err));
			}
			#endregion

			#region Buildind sql query
			StringBuilder sql = new StringBuilder();

            /*SELECT Clause : 
          * The variable "classificationLevelLabel" correspond to the table of level brand classification.
           * The first column is the identifier of the classification level item.
           * Th seconde column is the label of trhe classification level item.*/
            sql.AppendFormat("select distinct wp.id_{0} as id_item, wp.{0} as item ", classificationLevelLabel);
            
            /*FROM clause : Obtains the targets tables of the query*/
            GetFromClause(sql, oView, classificationLevelLabel, useView);

            /*Query conditions */
            /* Restriction on the classification items selected by the customer*/
            sql.AppendFormat(" where wp.id_{0} in ({1})", classificationLevelLabel, idList);

            /*Query tables joins */
            GetJointClause(sql, oView, classificationLevelLabel, _dimension, classificationRight, useView);

            /*Query orders fields by items' labels */
            sql.AppendFormat(" order by  {0}", classificationLevelLabel);
			#endregion

			#region Execute sql query
			try {
                //Excuting sql query
				return GetSource().Fill(sql.ToString());
			}
			catch (System.Exception e) {
				throw new Exceptions.ClassificationItemsDALException(e.Message, e);
			}
			#endregion

		}

        /// <summary>
        ///  Search function for Product or vehicle classification Items. The data will be filter 
        ///  with customer classification rights. It means product or media restriction.
        ///  <remarks>This method is use only for the modules  " Product class analysis: Graphic key reports "
        ///  and "Product class analysis: Detailed reports"
        /// 
        /// /// - We use the parameter "classificationLevelLabel" to build the SQL query fields as follows :
        ///  <code>
        ///  public virtual DataSet GetItems(string classificationLevelLabel, CustomerRightConstante.type customerRightType){
        ///  
        /// ...
        /// //Set fields of the query with the current classification level
        /// sql.AppendFormat(" select distinct pr.id_{0} as id_item, pr.{0} as item ", classificationLevelLabel);
        ///  //Set Table of the query with the current classification level 
        ///  
        ///   /*FROM clause : get the targets tables of the query*/
        /// sql.AppendFormat(" from {0} pr ",classificationLevelLabel)
        /// 
        /// sql.AppendFormat(" order by  {0} ", classificationLevelLabel);
        /// ...
        /// }
        /// </code>
        /// 
        /// </remarks>
        ///  <example>
        /// To search data for vehicle level :
        /// <code>
        /// Select distinct wp.id_media as id_item, wp.media  as item
        /// 
        /// //Query on table of vehicle classification
        /// from media wp
        ///                
        /// //vehicle's rights
        /// and wp.id_media in (1,2,3)
        /// 
        /// ...
        /// 
        /// //Order by vehicle label
        /// order by media
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="classificationLevelLabel">Label of classification level. For example, for the classification level
        /// product, the label will be "product". It corresponds also to the current classification level's table.</param>
        /// <param name="customerRightType">Enumerator allowing to get user rights of the current classification level.</param>          
        /// <returns>Data set with data table[id_item,item] : identifer and label of a level of brand classification</returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.ClassificationItemsDALException">Throw exception when error occurs during 
        /// execution or building of the query</exception>
        public virtual DataSet GetRecapItems(string classificationLevelLabel, CustomerRightConstante.type customerRightType)
        {

			#region Building sql query
			StringBuilder sql = new StringBuilder(1000);

            /*SELECT Clause : 
          * The variable "table" correspond to the table of level brand classification.
           * The first column is the identifier of the classification level item.
           * Th seconde column is the label of trhe classification level item.*/
            sql.AppendFormat(" select distinct wp.id_{0} as id_item, wp.{0} as item ", classificationLevelLabel);

            /*FROM clause : Obtains the targets tables of the query*/
            sql.AppendFormat(" from {0}.{1} wp ", _dBSchema, classificationLevelLabel);
           
            /*Query conditions */
            /* Restriction on the data language*/
			sql.AppendFormat( " where wp.id_language = {0} ",_session.DataLanguage);			
			
            /*Restriction on the cusomer's product classification rights*/
            SQLGenerator.GetInClauseMagicMethod("id_" + classificationLevelLabel, _session.CustomerLogin[customerRightType], true);

            /* Activation code to obtain only actives data */
			sql.AppendFormat(" and wp.activation < {0} ", TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);

            /*Query orders fields by items' labels */
            sql.AppendFormat(" order by {0} ", classificationLevelLabel);
			#endregion

			#region Excute sql query
			try {
                //Excuting sql query
				return GetSource().Fill(sql.ToString());
			}
			catch (System.Exception e) {
				throw new Exceptions.ClassificationItemsDALException(e.Message, e);
			}
			#endregion
		}

		#endregion

		#region GetRights
		/// <summary>
		/// Obtains customer rights according to brand classification.
		/// </summary>
        /// <param name="dimension">Determines the classification brand to use</param>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.ClassificationItemsDALException">
        /// Unknown classification brand</exception>
		protected virtual string GetRights( Dimension dimension) {
			string classificationRight = "";
			switch (dimension) {
                //Obtains rights of the product classification
				case Dimension.product:
					classificationRight = GetCustomerProductRight("wp", true);
					break;
                //Obtains rights of the vehicle classification
				case Dimension.media:
					classificationRight = GetMediaRights( "wp", true);
					break;
				default:
					throw (new Exceptions.ClassificationItemsDALException("Unknown classification brand"));
			}

			return classificationRight;
		}

		

		#endregion

		#region User Rights

        #region Product Rights
        /// <summary>
        //Obtains user rights of the product classification.
		/// </summary>
		/// <param name="tablePrefixe">table prefix</param>
		/// <param name="beginByAnd">True if the sql clause start with "AND"</param>
        /// <returns>SQL code string</returns>
		public virtual string GetCustomerProductRight( string tablePrefix, bool beginByAnd) {
			switch (_module.Id) {
                /* Obtains user rights of the product classification for the modules  
                 * " Product class analysis: Graphic key reports " and "Product class analysis: Detailed reports".*/
				case TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR:
				case TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE:
                    return (SQLGenerator.GetClassificationCustomerProductRight(_session, tablePrefix, tablePrefix, tablePrefix, tablePrefix, tablePrefix, beginByAnd));
				default:
                     /*Obtains user rights of the product classification for the modules for the others modules*/
					return (SQLGenerator.getClassificationCustomerProductRight(_session, tablePrefix, tablePrefix, tablePrefix, tablePrefix, beginByAnd));
			}
		}

		#endregion

		#endregion

		#region Protected Methods

		#region GetView
		/// <summary>
        /// Search of classification items is done on a View of product or vehicle classification.
        ///  In fact, if the dimension (classification brand) is product, the search will be done on a view called "ADEXPR03.ALL_PRODUCT_33".
        ///  ADEXPR03 corresponds to the scheme of the view and ALL_PRODUCT_33 the label of the view. 
        ///  The number 33 corresponds to the identifier of the country, here France.
        ///  If the dimension is vehicle (classification brand), the search will be done on a view called "ADEXPR03.ALL_MEDIA_33".
        ///  ALL_MEDIA_33 corresponds to the label of the view.
        ///  
        /// The View used for the queries depends also on type of module. 
		/// </summary>
        /// <param name="dimension">classification brand</param>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.ClassificationItemsDALException">
        /// Unknown classification brand</exception>
        protected virtual View GetView(Dimension dimension)
        {
            //Switch on module
            switch (_module.Id)
            {
                /* Obtains user View of the product or media classification for the modules  
                * " Product class analysis: Graphic key reports " and "Product class analysis: Detailed reports".*/
                case TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR:
                case TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE:
                    switch (dimension)
                    {   //View for product classification brand
                        case Dimension.product:
                            return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allRecapProduct);
                        
                        //View for vehicle classification brand
                        case Dimension.media:
                            return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allRecapMedia);
                        default:
                            throw (new Exceptions.ClassificationItemsDALException("Unknown classification brand"));
                    }
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_MANDATAIRES:
                    switch (dimension)
                    {
                        case Dimension.product:
                            return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allAdvAgency);
                        default:
                            throw (new Exceptions.ClassificationItemsDALException("Unknown nomenclature dimension"));
                    }
                default:
                    /* Obtains user View of the product or media classification for the others modules */
                    switch (dimension)
                    {
                        //View for product classification brand
                        case Dimension.product:
                            return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allProduct);

                        //View for vehicle classification brand
                        case Dimension.media:
                            return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allMedia);
                        default:
                            throw (new Exceptions.ClassificationItemsDALException("Unknown classification brand"));
                    }
            }

        }
		#endregion

		
		#region GetSource
		/// <summary>
        /// obtains the data source according to the module. Data source allows us 
        /// to execute the various SQL queries according to the database, module, site language.
		/// </summary>
		protected virtual IDataSource GetSource() {
			switch (_session.CurrentModule) {
                  /*obtains the data source for the modules  
                 * " Product class analysis: Graphic key reports " and "Product class analysis: Detailed reports".*/
				case TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR:
				case TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE:
					return WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis, WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].NlsSort);
				default:
                     /*obtains the data source for the others modules*/
					return _session.Source;
			}
		}
		#endregion

		#region Get From Clause
		/// <summary>
        /// Get sql FROM clause
		/// </summary>
		/// <param name="oView">View</param>
		/// <param name="table">Table label</param>
		/// <param name="useView">True if must use tables' View</param>
        /// <param name="dBSchema">Database scheme</param>
        /// <param name="sql">Sql string builder</param>
        /// <returns>sql FROM clause string</returns>
		protected virtual void GetFromClause(StringBuilder sql,  View oView, string table,  bool useView) {
			if (useView)
				sql.AppendFormat(" from {0}{1} wp ", oView.Sql,_session.DataLanguage.ToString());
			else sql.AppendFormat(" from {0}.{1} wp ",_dBSchema,table);
		}
		#endregion

		#region Get Joint Clause
		/// <summary>
		/// Obtains sql Joins clause
		/// </summary>
		/// <param name="oView">View</param>
		/// <param name="table">Table label</param>
		/// <param name="classificationRight">Classification rights</param>
		/// <param name="dimension">media or product classification brand</param>
		/// <param name="useView">True if must use tables' View</param>
        /// <param name="sql">Sql string builder</param>
		/// <returns>sql FROM clause string</returns>
		protected virtual void GetJointClause(StringBuilder sql, View oView, string table, Dimension dimension, string classificationRight, bool useView) {
		
            //If the queries will be done on classification brand view
			if (useView) {

				// Adding user classication rights
				if (classificationRight != null && classificationRight.Length > 0) {
					sql.Append(classificationRight);
				}				

				switch (_session.CurrentModule) {
                    //For module Media Schedule the data will be filter on selected media (ex. Identifer of media TELEVISION)
					case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA:
						if (dimension == Dimension.media)
							sql.AppendFormat(" and wp.id_vehicle in ({0}) ", _session.GetSelection(_session.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess));
						break;
				}
			}
			else {
                /*If the query is executed only on a classification table (ex. table adexpr03.Sector), only valid data (activation code equal to zero)
                 * must returned and for a specific language (ex. id_language 33 for French) */
				sql.AppendFormat( " and wp.activation<{0}", TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
				sql.AppendFormat(" and wp.id_language = {0}", _session.DataLanguage);
			}			
		}
		#endregion

		#region CanUseView
		/// <summary>
		/// Determine if tables' View must be used in SQL query.
		/// </summary>
		/// <param name="dimension">media or product classification brand</param>
		/// <param name="classificationRight">classification rights string</param>
        /// <returns>Ture if tables' View must be used in SQL query.</returns>
		protected virtual bool CanUseView(Dimension dimension, string classificationRight) {
            /*If rights are applied, the query will be executed on classification (produtc or  vehicle) view.
             * The view corresponding to a classification brand contains all fields (identifier and label) of brand.
             * For example for the product classification the View will be like this : id_produt,product, id_sector,sector, di_group_,
             * group_ etc */            
			if ((classificationRight != null && classificationRight.Length > 0)
			|| (dimension == Dimension.media && _module.Id == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA))
				return true;
			return false;
		}
		#endregion

		#endregion
	}
}
