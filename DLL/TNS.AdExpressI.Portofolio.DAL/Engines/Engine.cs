#region Information
// Author: D. Mussuma
// Creation date: 08/08/2008
// Modification date:
#endregion

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Portofolio.DAL.Exceptions;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
//using WebFunctions = TNS.AdExpress.Web.Functions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Classification;
using WebNavigation = TNS.AdExpress.Domain.Web.Navigation;

//using TNS.AdExpress.Web.Exceptions;
using CustormerConstantes = TNS.AdExpress.Constantes.Customer;
using CstProject = TNS.AdExpress.Constantes.Project;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Exceptions;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.Classification.Universe;

namespace TNS.AdExpressI.Portofolio.DAL.Engines {
	/// <summary>
	/// Compute data for different results of portofolio
	/// </summary>
	public abstract class Engine {

		#region Variables
		/// <summary>
		/// Customer session
		/// </summary>
		protected WebSession _webSession;
		/// <summary>
		/// Current Module
		/// </summary>
		protected Module _module;
		/// <summary>
		/// Vehicle information
		/// </summary>
		protected VehicleInformation _vehicleInformation;
		/// <summary>
		/// Media Id
		/// </summary>
		protected Int64 _idMedia;
		/// <summary>
		/// Begining Date
		/// </summary>
		protected string _beginingDate;
		/// <summary>
		/// End Date
		/// </summary>
		protected string _endDate;
		/// <summary>
		/// Cobranding condition
		/// </summary>
		protected int _cobrandindConditionValue = 1;
		#endregion

		#region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
		/// <param name="vehicleInformation">Vehicle information</param>
		/// <param name="idMedia">Media Id</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
		protected Engine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string beginingDate, string endDate) {
            if(webSession==null) throw (new ArgumentNullException("Customer session is null"));
            if(beginingDate==null || beginingDate.Length==0) throw (new ArgumentException("Begining Date is invalid"));
            if(endDate==null || endDate.Length==0) throw (new ArgumentException("End Date is invalid"));
			if (module == null) throw (new ArgumentNullException("Module is null"));
			if (vehicleInformation == null) throw (new ArgumentNullException("vehicleInformation session is null"));
            _webSession=webSession;
            _beginingDate=beginingDate;
            _endDate=endDate;
			_vehicleInformation = vehicleInformation;
            _idMedia = idMedia;                          
             _module=module;          
        
        }
		
        #endregion


		/// <summary>
		/// Get data
		/// </summary>
		/// <returns></returns>
		public virtual DataSet GetData() {
			return ComputeData();
		}

		/// <summary>
		/// Get result table
		/// </summary>
		/// <returns></returns>
		protected abstract DataSet ComputeData();

		#region Get Product Data
		/// <summary>
		/// Récupère la liste produit de référence
		/// </summary>
		/// <returns>la liste produit de référence</returns>
		protected virtual string GetProductData() {
			string sql = "";
			if (_webSession.PrincipalProductUniverses != null && _webSession.PrincipalProductUniverses.Count > 0)
				sql = _webSession.PrincipalProductUniverses[0].GetSqlConditions(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);

			return sql;
		}
		#endregion

		#region Get media universe
		/// <summary>
		/// Get media Universe
		/// </summary>
		/// <param name="webSession">Web Session</param>
		/// <returns>string sql</returns>
		protected virtual string GetMediaUniverse( string prefix) {
			string sql = "";
			ResultPageInformation resPageInfo = _module.GetResultPageInformation(_webSession.CurrentTab);
			if(resPageInfo != null)
				sql = resPageInfo.GetAllowedMediaUniverseSql(prefix, true);
			return sql;
		}
		#endregion

		#region Get excluded products
		/// <summary>
		/// Get excluded products
		/// </summary>
		/// <param name="sql">String builder</param>
		/// <returns></returns>
		protected virtual string GetExcludeProducts(string prefix) {
			// Exclude product 
			string sql = "";
            ProductItemsList prList = null;
            if( Product.Contains(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID) && (prList = Product.GetItemsList(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID)) != null){
				sql = prList.GetExcludeItemsSql(true, prefix);
            }
			return sql;
		}
		#endregion

        #region GetFormatClause
        /// <summary>
        /// Get Format Clause
        /// </summary>
        /// <param name="prefix">Prefix</param>
        /// <returns>Sql Format selected Clause</returns>
        protected virtual string GetFormatClause(string prefix) {
            var sql = new StringBuilder();
            var formatIdList = _webSession.GetValidFormatSelectedList(new List<VehicleInformation>(new[] { _vehicleInformation }));
            if (formatIdList.Count > 0)
                sql.AppendFormat(" and {0}ID_{1} in ({2}) "
                    , ((!string.IsNullOrEmpty(prefix)) ? prefix + "." : string.Empty)
                           , WebApplicationParameters.DataBaseDescription.GetTable(WebApplicationParameters.VehiclesFormatInformation.VehicleFormatInformationList[_vehicleInformation.DatabaseId].FormatTableName).Label
                           , string.Join(",", formatIdList.ConvertAll(p => p.ToString()).ToArray()));
            return sql.ToString();
        }
        #endregion

        #region Get Purchase Mode Clause
        /// <summary>
        /// Get Purchase Mode Clause
        /// </summary>
        /// <param name="prefix">Prefix</param>
        /// <returns>Sql Purchase Mode selected Clause</returns>
        protected virtual string GetPurchaseModeClause(string prefix) {
            var sql = new StringBuilder();
            if (WebApplicationParameters.UsePurchaseMode && _webSession.CustomerLogin.CustormerFlagAccess(Flags.ID_PURCHASE_MODE_DISPLAY_FLAG))
            {
                var vehicleInfoList = _webSession.GetVehiclesSelected();
                if (vehicleInfoList.ContainsKey(VehiclesInformation.Get(Vehicles.names.mms).DatabaseId)) {
                    string purchaseModeIdList = _webSession.SelectedPurchaseModeList;
                    if (purchaseModeIdList.Length > 0)
                        sql.AppendFormat(" and {0}ID_{1} in ({2}) "
                            , ((!string.IsNullOrEmpty(prefix)) ? prefix + "." : string.Empty)
                                   , WebApplicationParameters.DataBaseDescription.GetTable(TableIds.purchaseModeMMS).Label
                                   , purchaseModeIdList);
                }
            }
            return sql.ToString();
        }
        #endregion

        #region GetMediatSelection
        /// <summary>
        /// Get media selection
        /// </summary>
        /// <remarks>
        /// Must beginning by AND
        /// </remarks>
        /// <param name="dataTablePrefixe">data table prefixe</param>
        /// <returns>media selection to add as condition into a sql query</returns>
        protected virtual string GetMediaSelection(string dataTablePrefixe)
        {
            StringBuilder sql = new StringBuilder();

            #region Old

            //if (_webSession.SecondaryMediaUniverses != null && _webSession.SecondaryMediaUniverses.Count > 0)
            //    sql.Append(_webSession.SecondaryMediaUniverses[0].GetSqlConditions(dataTablePrefixe, true)); 
            #endregion
          
            if (_webSession.PrincipalMediaUniverses != null && _webSession.PrincipalMediaUniverses.Count > 0)
                sql.Append(_webSession.PrincipalMediaUniverses[0].GetSqlConditions(dataTablePrefixe, true));

            return sql.ToString();
        }
        #endregion

        protected virtual void GetGad(ref string dataTableNameForGad, ref string dataFieldsForGad, ref string dataJointForGad)
        {
            
                dataTableNameForGad = ", " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.gad).SqlWithPrefix;
            dataFieldsForGad = ", " + SQLGenerator.GetFieldsAddressForGad();
            dataJointForGad = "and " +
                              SQLGenerator.GetJointForGad(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

        }

        protected virtual string GetBannerGroupby()
        {
            return string.Format(",{0}.id_banners", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

        }

     

    }
}
