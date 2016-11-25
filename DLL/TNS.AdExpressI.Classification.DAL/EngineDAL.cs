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
//using WebFunctions = TNS.AdExpress.Web.Functions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Web.Core.Utilities;

namespace TNS.AdExpressI.Classification.DAL
{
    /// <summary>
    /// Base class for the engines which going to compute the data of product or vehicle classification brand.
    /// Contains the filters methods of customer rights and working set.    
    /// </summary>
    public abstract class EngineDAL
    {

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
        /// <summary>
        /// Data Source
        /// </summary>
        protected TNS.FrameWork.DB.Common.IDataSource _dataSource = null;
        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public EngineDAL(WebSession session)
        {
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
        /// <summary>
        /// Get Data Source
        /// </summary>
        public TNS.FrameWork.DB.Common.IDataSource DataSource
        {
            set
            {
                _dataSource = value;
            }
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
            if (_session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR)
            {
                List<Int64> lst = ((Module)_session.CustomerLogin.GetModule(_session.CurrentModule)).ExcludedVehicles;
                if (lst != null && lst.Count > 0)
                {
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
        protected virtual string GetMediaRights(bool beginByAnd)
        {
            string sql = "";

            /*Get vehicle classification rights for modules  " Product class analysis: Graphic key reports "
            *  and "Product class analysis: Detailed reports"*/
            if (_session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
                || _session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE)
            {
                sql += GetRecapMediaConditions(vehicleTable, categoryTable, mediaTable, true);
            }
            else  /*Get vehicle classification rights for modules the others modules*/
                sql += GetMediaRights(vehicleTable, categoryTable, mediaTable, beginByAnd);

            if (_session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.NEW_CREATIVES)
            {
                //Get only media "adnettrack" or "telephony mobile" vehicles' for selection (for france)
                string ids = "";
                if (VehiclesInformation.Contains(VehicleClassificationCst.evaliantMobile))
                    ids = VehiclesInformation.Get(VehicleClassificationCst.evaliantMobile).DatabaseId.ToString();
                if (VehiclesInformation.Contains(VehicleClassificationCst.adnettrack))
                    ids += (ids != null && ids.Length > 0) ? "," + VehiclesInformation.Get(VehicleClassificationCst.adnettrack).DatabaseId.ToString()
                        : VehiclesInformation.Get(VehicleClassificationCst.adnettrack).DatabaseId.ToString();
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
        protected virtual string GetRecapMediaConditions(Table vehicleTable, Table categoryTable, Table mediaTable, bool beginByAnd)
        {
            string sql = "";

            //Obtains identifiers of media allowed : id_vehicle in ( 1,2,3)
            sql += TNS.AdExpress.Web.Core.Utilities.SQLGenerator.getAccessVehicleList(_session, vehicleTable.Prefix, beginByAnd);

            /*Exclude Sponsorship sub media if not allowed for current customer 
           * for Module modules  " Product class analysis: Graphic key reports "
         * and "Product class analysis: Detailed reports "*/
            if (_session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
                || _session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE
                && !_session.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SPONSORSHIP_TV_ACCESS_FLAG))
            {//FLAG rights for sub media sponsorship (for france)
                string idSponsorShipCategory = TNS.AdExpress.Domain.Lists.GetIdList(WebConstantes.GroupList.ID.category,
                    WebConstantes.GroupList.Type.productClassAnalysisSponsorShipTv);
                if (!string.IsNullOrEmpty(idSponsorShipCategory))
                {
                    if (beginByAnd || (sql != null && sql.Length > 0)) sql += " and ";
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
            if (_session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.VP
                || _session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ROLEX
                || _session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_PROGRAMMES)
            {
                if (beginByAnd) return string.Empty;
                else return " 1 = 1 ";
            }
            else
            {
                return GetMediaRights(prefix, prefix, prefix, beginByAnd);
            }
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
            if (_session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.VP
                 || _session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ROLEX)
            {
                if (beginByAnd) return string.Empty;
                else return " 1 = 1 ";
            }
            else
            {
                return GetMediaRights(vehicleTable.Prefix, categoryTable.Prefix, mediaTable.Prefix, beginByAnd);
            }
        }

        /// <summary>
        /// Get vehicle classification brand rights.
        /// </summary>
        /// <param name="categoryTable">sub media Table Prefix</param>
        /// <param name="mediaTable">vehicle Table Prefix</param>
        /// <param name="vehicleTable">media Table Prefix</param>
        /// <param name="beginByAnd">True if sql clause start with "AND"</param>
        /// <returns>string sql</returns>
        protected virtual string GetMediaRights(string vehicleTablePrefix, string categoryTablePrefix, string mediaTablePrefix, bool beginByAnd)
        {
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
                    sql += " ((" + SQLGenerator.GetInClauseMagicMethod((!string.IsNullOrEmpty(vehicleTablePrefix) ? vehicleTablePrefix + "." : "") + "id_vehicle", rights[CustomerRightConstante.type.vehicleAccess], true) + " ";
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
                    sql += " " + SQLGenerator.GetInClauseMagicMethod((!string.IsNullOrEmpty(categoryTablePrefix) ? categoryTablePrefix + "." : "") + "id_category", rights[CustomerRightConstante.type.categoryAccess], true) + " ";
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
                    sql += " " + SQLGenerator.GetInClauseMagicMethod((!string.IsNullOrEmpty(mediaTablePrefix) ? mediaTablePrefix + "." : "") + "id_media", rights[CustomerRightConstante.type.mediaAccess], true) + " ";
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
                    sql += " " + SQLGenerator.GetInClauseMagicMethod((!string.IsNullOrEmpty(vehicleTablePrefix) ? vehicleTablePrefix + "." : "") + "id_vehicle", rights[CustomerRightConstante.type.vehicleException], false) + " ";
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
                    sql += " " + SQLGenerator.GetInClauseMagicMethod((!string.IsNullOrEmpty(categoryTablePrefix) ? categoryTablePrefix + "." : "") + "id_category", rights[CustomerRightConstante.type.categoryException], false) + " ";
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
                    sql += " " + SQLGenerator.GetInClauseMagicMethod((!string.IsNullOrEmpty(mediaTablePrefix) ? mediaTablePrefix + "." : "") + "id_media", rights[CustomerRightConstante.type.mediaException], false) + " ";
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
        protected virtual string GetProductSelection(string dataTablePrefix, bool beginByAnd)
        {
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

        //protected virtual string GetVpBrandsRights(string tablePrefix, bool beginByAnd)
        //{
        //    return GetVpBrandsRights(tablePrefix, tablePrefix, beginByAnd);
        //}

        //protected virtual string GetVpBrandsRights(string circuitPrefix, string brandPrefix, bool beginByAnd)
        //{
        //    var sql = new StringBuilder();
        //    bool fisrt = true;

        //    // Get the circuit authorized for the current customer
        //    if (_session.CustomerLogin[CustomerRightConstante.type.circuitAccess].Length > 0)
        //    {

        //        if (beginByAnd) sql.Append(" and");
        //        sql.AppendFormat(" (({0}.id_circuit in ({1}) ", circuitPrefix, _session.CustomerLogin[CustomerRightConstante.type.circuitAccess]);
        //        fisrt = false;
        //    }
        //    // Get the brands authorized for the current customer
        //    if (_session.CustomerLogin[CustomerRightConstante.type.vpBrandAccess].Length > 0)
        //    {
        //        if (!fisrt) sql.Append(" or ");
        //        else
        //        {
        //            if (beginByAnd) sql.Append(" and");
        //            sql.Append(" ((");
        //        }
        //        sql.AppendFormat(" {0}.id_brand in ({1}) ", brandPrefix, _session.CustomerLogin[CustomerRightConstante.type.vpBrandAccess]);
        //        fisrt = false;
        //    }
        //    if (!fisrt) sql.Append(" )");

        //    // Get the circuit not authorized for the current customer
        //    if (_session.CustomerLogin[CustomerRightConstante.type.circuitException].Length > 0)
        //    {
        //        if (!fisrt) sql.Append(" and");
        //        else
        //        {
        //            if (beginByAnd) sql.Append(" and");
        //            sql.Append(" (");
        //        }
        //        sql.AppendFormat(" {0}.id_circuit not in ({1}) ", circuitPrefix, _session.CustomerLogin[CustomerRightConstante.type.circuitException]);
        //        fisrt = false;
        //    }
        //    // Get the brands not authorized for the current customer
        //    if (_session.CustomerLogin[CustomerRightConstante.type.vpBrandException].Length > 0)
        //    {
        //        if (!fisrt) sql.Append(" and");
        //        else
        //        {
        //            if (beginByAnd) sql.Append(" and");
        //            sql.Append(" (");
        //        }
        //        sql.AppendFormat(" {0}.id_brand not in ({1}) ", circuitPrefix, _session.CustomerLogin[CustomerRightConstante.type.vpBrandException]);
        //        fisrt = false;
        //    }
        //    if (!fisrt) sql.Append(" )");

        //    return sql.ToString();
        //}

        //protected virtual string GetVpProductsRights(string tablePrefix, bool beginByAnd)
        //{
        //    return GetVpProductsRights(tablePrefix, tablePrefix, tablePrefix, beginByAnd);
        //}

        //protected virtual string GetVpProductsRights(string segmentPrefix, string subSegmentPrefix, string productPrefix, bool beginByAnd)
        //{
        //    var sql = new StringBuilder();
        //    bool fisrt = true;

        //    // Get the segments authorized for the current customer
        //    if (_session.CustomerLogin[CustomerRightConstante.type.vpSegmentAccess].Length > 0)
        //    {

        //        if (beginByAnd) sql.Append(" and");
        //        sql.AppendFormat(" (({0}.id_segment in ({1}) ", segmentPrefix,
        //            _session.CustomerLogin[CustomerRightConstante.type.vpSegmentAccess]);
        //        fisrt = false;
        //    }
        //    // Get the sub segments authorized for the current customer
        //    if (_session.CustomerLogin[CustomerRightConstante.type.vpSubSegmentAccess].Length > 0)
        //    {
        //        if (!fisrt) sql.Append(" or ");
        //        else
        //        {
        //            if (beginByAnd) sql.Append(" and");
        //            sql.Append(" ((");
        //        }
        //        sql.AppendFormat(" {0}.id_category in ({1}) ", subSegmentPrefix,
        //            _session.CustomerLogin[CustomerRightConstante.type.vpSubSegmentAccess]);
        //        fisrt = false;
        //    }
        //    // Get the products authorized for the current customer
        //    if (_session.CustomerLogin[CustomerRightConstante.type.vpProductAccess].Length > 0)
        //    {
        //        if (!fisrt) sql.Append(" or ");
        //        else
        //        {
        //            if (beginByAnd) sql.Append(" and");
        //            sql.Append(" ((");
        //        }
        //        sql.AppendFormat(" {0}.id_product in ({1}) ", productPrefix,
        //            _session.CustomerLogin[CustomerRightConstante.type.vpProductAccess]);
        //        fisrt = false;
        //    }
        //    if (!fisrt) sql.Append(" )");

        //    // Get the segments not authorized for the current customer
        //    if (_session.CustomerLogin[CustomerRightConstante.type.segmentException].Length > 0)
        //    {
        //        if (!fisrt) sql.Append(" and");
        //        else
        //        {
        //            if (beginByAnd) sql.Append(" and");
        //            sql.Append(" (");
        //        }
        //        sql.AppendFormat(" {0}.id_segment not in ({1}) ", segmentPrefix,
        //            _session.CustomerLogin[CustomerRightConstante.type.segmentException]);
        //        fisrt = false;
        //    }
        //    // Get the brands not authorized for the current customer
        //    if (_session.CustomerLogin[CustomerRightConstante.type.vpSubSegmentException].Length > 0)
        //    {
        //        if (!fisrt) sql.Append(" and");
        //        else
        //        {
        //            if (beginByAnd) sql.Append(" and");
        //            sql.Append(" (");
        //        }
        //        sql.AppendFormat(" {0}.id_category not in ({1}) ", subSegmentPrefix,
        //            _session.CustomerLogin[CustomerRightConstante.type.vpSubSegmentException]);
        //        fisrt = false;
        //    }
        //    // Get the product not authorized for the current customer
        //    if (_session.CustomerLogin[CustomerRightConstante.type.vpProductException].Length > 0)
        //    {
        //        if (!fisrt) sql.Append(" and");
        //        else
        //        {
        //            if (beginByAnd) sql.Append(" and");
        //            sql.Append(" (");
        //        }
        //        sql.AppendFormat(" {0}.id_product not in ({1}) ", productPrefix,
        //            _session.CustomerLogin[CustomerRightConstante.type.vpProductException]);
        //        fisrt = false;
        //    }
        //    if (!fisrt) sql.Append(" )");

        //    return sql.ToString();
        //}
    }
}
