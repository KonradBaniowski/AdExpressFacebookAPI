#region Information
/*
 * Author : Y Rkaina && D. Mussuma
 * Created on : 15/07/2009
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
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web.Navigation;
using WebNavigation = TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.DataBaseDescription;
using DBClassification = TNS.AdExpress.Constantes.Classification.DB;
using VehicleClassificationCst = TNS.AdExpress.Constantes.Classification.DB.Vehicles.names;
using CustomerRightConstante = TNS.AdExpress.Constantes.Customer.Right;
using WebFunctions = TNS.AdExpress.Web.Functions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpressI.Classification.DAL {
	/// <summary>
	/// Base class for the engines which going to compute the data of product or vehicle classification brand.
    /// Contains the filters methods of customer rights and working set.    
	/// </summary>
	public abstract class EngineDAL {

		#region Attributes
		/// <summary>
		/// User session
		/// </summary>
		protected WebSession _session = null;	
        /// <summary>
        /// Current web site's module
        /// </summary>
		protected Module _module = null;
        /// <summary>
        /// vehicle classification brand tables' descriptions
        /// <remarks> Objet Table contains label,prefix,scheme of the table. Uses to build SQl query string</remarks>
        /// </summary>
		protected Table vehicleTable = null, categoryTable = null, basicMediaTable = null, mediaTable = null;

        /// <summary>
        /// Data base scheme
        /// </summary>
        protected string _dBSchema = null;
        /// <summary>
        /// Filters (that we can apply for a specific level)
        /// we can add severals filters
        /// The key represents the level filter
        /// The value represents the list of ids to exclude (example of a list : 9999,999,2541)
        /// </summary>
        protected Dictionary<long, string> _filters = new Dictionary<long, string>();
		#endregion

		#region Constructor		

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="session">User session</param>
		public EngineDAL(WebSession session) {
			_session = session;
			_module = (Module)_session.CustomerLogin.GetModule(_session.CurrentModule);
		}
		
		#endregion


        #region Properties
        /// <summary>
        /// Database schema
        /// <remarks>Can be null for others country excpted for france</remarks>
        /// </summary>
        public string DBSchema
        {
            set
            {
                _dBSchema = value;
            }
        }
        /// <summary>
        /// Get/Set Control filters
        /// </summary>
        public Dictionary<long, string> Filters
        {
            get { return _filters; }
            set { _filters = value; }
        }
        #endregion

        /// <summary>
        /// Defines the customer's vehicle classification brand working set.
        /// <example>
        /// The data will be restricted on :       
        /// <code>
        /// and id_vehcile in (1,2,3,4,5,6)
        /// </code>
        /// </example>
        /// </summary>
        /// <returns>SQl conditions string</returns>
        protected virtual string GetAllowedMediaUniverse()
        {
			string sql = "";
			//obtains customer vehicle universe conditions.
			if (_module != null)
				sql += _module.GetAllowedMediaUniverseSql(vehicleTable.Prefix, categoryTable.Prefix, mediaTable.Prefix, true);

            //Exclude media "MARKETING DIRECT" for  module " Product class analysis: Graphic key reports " (for France)
			if (_session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR) {
				List<Int64> lst = ((Module)_session.CustomerLogin.GetModule(_session.CurrentModule)).ExcludedVehicles;
				if (lst != null && lst.Count > 0) {
					string inCondition = String.Join(", ", Array.ConvertAll<long, string>(lst.ToArray(), i => i.ToString()));
					sql += " and " + vehicleTable.Prefix + ".id_vehicle not in ( " + inCondition + ") ";
				}
			}
			return sql;
		}

        /// <summary>
        ///  Obtains customer vehicle classification brand rights. 
        /// </summary>
        /// <example>
        /// The data will be restricted on :       
        /// <code>
        /// string sql ="";
        /// TNS.AdExpress.Web.Core.CustomerDataFilters dataFilters = new TNS.AdExpress.Web.Core.CustomerDataFilters(_session);            
        ///  Dictionary[TNS.AdExpress.Constantes.Customer.Right.type, string] rights = null;
        ///    if (dataFilters != null) rights = dataFilters.GetMediaRights();
        /// ...
        /// 
        ///  //Get allowed medias
        ///     if (rights[CustomerRightConstante.type.vehicleAccess].Length > 0)
        ///      {
		///  		sql += " and";
        ///  		sql += " ((" + vehicleTable.Prefix + ".id_vehicle in (" + rights[CustomerRightConstante.type.vehicleAccess] + ") ";
        ///        
        ///  	}
        ///  	 //Get not allowed medias
        ///   if (rights[CustomerRightConstante.type.vehicleException].Length > 0)
        ///    {
        ///        sql += " and";
        ///		 sql += " " + vehicleTable.Prefix + ".id_vehicle not in (" + rights[CustomerRightConstante.type.vehicleException] + ") ";
        ///      
		///	}
        ///	
        ///	...
        /// </code>
        /// </example>
        /// <param name="beginByAnd">True if SQL clause strat with "AND"</param>
        /// <returns>SQl rights string</returns>
		protected virtual string GetMediaRights(bool beginByAnd) {
			string sql = "";

            /*Get vehicle classification rights for modules  " Product class analysis: Graphic key reports "
            *  and "Product class analysis: Detailed reports"*/
			if (_session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
				|| _session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE) {
				sql += GetRecapMediaConditions(vehicleTable, categoryTable, mediaTable,true);
			}
            else  /*Get vehicle classification rights for modules the others modules*/
				sql += GetMediaRights( vehicleTable, categoryTable, mediaTable,beginByAnd);

			if (_session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.NEW_CREATIVES) {
				//Get only media "adnettrack" or "telephony mobile" vehicles' for selection (for france)
				string ids = "";
				if (VehiclesInformation.Contains(VehicleClassificationCst.evaliantMobile))
					ids = VehiclesInformation.Get(VehicleClassificationCst.evaliantMobile).DatabaseId.ToString();
				if (VehiclesInformation.Contains(VehicleClassificationCst.adnettrack))
					ids += (ids != null && ids.Length > 0) ? "," + VehiclesInformation.Get(VehicleClassificationCst.adnettrack).DatabaseId.ToString() : VehiclesInformation.Get(VehicleClassificationCst.adnettrack).DatabaseId.ToString();
				if (ids != null && ids.Length > 0) sql += " and " + vehicleTable.Prefix + ".id_vehicle  in ( " + ids + ") ";
				else throw (new Exception("Impossible to execute query no adnettrack and mobileTelephony vehciles availabe "));
			}
			
			return sql;
		}

		/// <summary>
        /// Obtains modules  " Product class analysis: Graphic key reports "
        /// and "Product class analysis: Detailed reports" media conditions.
		/// </summary>
        /// <returns>sql conditions string </returns>
		protected virtual string GetRecapMediaConditions(Table vehicleTable, Table categoryTable, Table mediaTable,bool beginByAnd) {
			string sql = "";

		    //Obtains identifiers of media allowed : id_vehicle in ( 1,2,3)
            sql += WebFunctions.SQLGenerator.getAccessVehicleList(_session, vehicleTable.Prefix, beginByAnd);
			
            /*Exclude Sponsorship sub media if not allowed for current customer 
           * for Module modules  " Product class analysis: Graphic key reports "
         * and "Product class analysis: Detailed reports "*/
            if (_session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
				|| _session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE
				&& !_session.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SPONSORSHIP_TV_ACCESS_FLAG)) {//FLAG rights for sub media sponsorship (for france)
				string idSponsorShipCategory = TNS.AdExpress.Domain.Lists.GetIdList(WebConstantes.GroupList.ID.category, WebConstantes.GroupList.Type.productClassAnalysisSponsorShipTv);
				if (idSponsorShipCategory != null && idSponsorShipCategory.Length > 0) {
                    if (beginByAnd || (sql!=null && sql.Length > 0)) sql += " and ";
					sql += "  " + categoryTable.Prefix + ".id_category not in (" + idSponsorShipCategory + ") ";
				}

			}
		

			return sql;
		}

		#region GetMediaRights

        /// <summary>
        /// Get vehicle classification brand rights
        /// </summary>
        /// <param name="prefix">prefix</param> 
        /// <param name="beginByAnd">True if sql clause start with "AND"</param>
        /// <returns>string sql</returns>
		protected virtual string GetMediaRights(string prefix, bool beginByAnd)
        {
            return GetMediaRights(prefix, prefix, prefix,beginByAnd);
        }

        /// <summary>
        /// Get vehicle classification brand rights
        /// </summary>
        /// <param name="categoryTable">Sub media Table description</param>
        /// <param name="mediaTable">vehicle Table description</param>
        /// <param name="vehicleTable">media Table description</param>
        /// <param name="beginByAnd">True if sql clause start with "AND"</param>
        /// <returns>sql rights string</returns>
        protected virtual string GetMediaRights(Table vehicleTable, Table categoryTable, Table mediaTable, bool beginByAnd)
        {
            return GetMediaRights(vehicleTable.Prefix, categoryTable.Prefix, mediaTable.Prefix, beginByAnd);
        }

		/// <summary>
        /// Get vehicle classification brand rights.
		/// </summary>
        /// <param name="categoryTable">sub media Table Prefix</param>
        /// <param name="mediaTable">vehicle Table Prefix</param>
        /// <param name="vehicleTable">media Table Prefix</param>
        /// <param name="beginByAnd">True if sql clause start with "AND"</param>
		/// <returns>string sql</returns>
		protected virtual string GetMediaRights(string vehicleTablePrefix, string categoryTablePrefix , string mediaTablePrefix,bool beginByAnd ) {
			string sql = "";
           
			bool fisrt = true;
            //Get Media rights filter data
            Dictionary<TNS.AdExpress.Constantes.Customer.Right.type, string> rights = _session.CustomerDataFilters.MediaRights;

            if (rights != null)
            {
                
                // Get the medias authorized for the current customer
                if (rights.ContainsKey(CustomerRightConstante.type.vehicleAccess) && rights[CustomerRightConstante.type.vehicleAccess].Length > 0)
                {
                    if (beginByAnd) sql += " and";
					sql += " ((" + ((vehicleTablePrefix != null && vehicleTablePrefix.Length>0) ? vehicleTablePrefix+"." : "" ) + " id_vehicle in (" + rights[CustomerRightConstante.type.vehicleAccess] + ") ";
                    fisrt = false;
                }
                // Get the sub medias authorized for the current customer
                if (rights.ContainsKey(CustomerRightConstante.type.categoryAccess) && rights[CustomerRightConstante.type.categoryAccess].Length > 0)
                {
                    if (!fisrt) sql += " or";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " ((";
                    }
					sql += " " + ((categoryTablePrefix != null && categoryTablePrefix.Length > 0) ? categoryTablePrefix + "." : "") + " id_category in (" + rights[CustomerRightConstante.type.categoryAccess] + ") ";
                    fisrt = false;
                }
                // Get the vehicles authorized for the current customer                
                    if (rights.ContainsKey(CustomerRightConstante.type.mediaAccess) && rights[CustomerRightConstante.type.mediaAccess].Length > 0)

                {
                    if (!fisrt) sql += " or";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " ((";
                    }
					sql += " " + ((mediaTablePrefix != null && mediaTablePrefix.Length > 0) ? mediaTablePrefix + "." : "") + " id_media in (" + rights[CustomerRightConstante.type.mediaAccess] + ") ";
                    fisrt = false;
                }
                if (!fisrt) sql += " )";

                // Get the medias not authorized for the current customer
                if (rights.ContainsKey(CustomerRightConstante.type.vehicleException) && rights[CustomerRightConstante.type.vehicleException].Length > 0)
                {
                    if (!fisrt) sql += " and";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " (";
                    }
					sql += " " + ((vehicleTablePrefix != null && vehicleTablePrefix.Length > 0) ? vehicleTablePrefix + "." : "") + " id_vehicle not in (" + rights[CustomerRightConstante.type.vehicleException] + ") ";
                    fisrt = false;
                }
                // Get the sub medias not authorized for the current customer
                    if (rights.ContainsKey(CustomerRightConstante.type.categoryException) && rights[CustomerRightConstante.type.categoryException].Length > 0)
                {
                    if (!fisrt) sql += " and";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " (";
                    }
					sql += " " + ((categoryTablePrefix != null && categoryTablePrefix.Length > 0) ? categoryTablePrefix + "." : "") + " id_category not in (" + rights[CustomerRightConstante.type.categoryException] + ") ";
                    fisrt = false;
                }
                // Get the vehicles not authorized for the current customer
                    if (rights.ContainsKey(CustomerRightConstante.type.mediaException) && rights[CustomerRightConstante.type.mediaException].Length > 0)
                {
                    if (!fisrt) sql += " and";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " (";
                    }
					sql += " " + ((mediaTablePrefix != null && mediaTablePrefix.Length > 0) ? mediaTablePrefix + "." : "") + " id_media not in (" + rights[CustomerRightConstante.type.mediaException] + ") ";
                    fisrt = false;
                }
                if (!fisrt) sql += " )";
            }
			return sql;
		}
		#endregion

        /// <summary>
        /// Get Product Selection
        /// </summary>
        /// <param name="dataTablePrefix"></param>
        /// <param name="beginByAnd"></param>
        /// <returns></returns>
        protected virtual string GetProductSelection(string dataTablePrefix,bool beginByAnd){
            // Sélection de Produits
            if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
            {
                switch (_session.CurrentModule)
                {
                    case TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR:
                    case TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE:
                        return _session.PrincipalProductUniverses[0].GetSqlConditions(dataTablePrefix, beginByAnd);
                }
            }
            return "";
        }
	}
}
