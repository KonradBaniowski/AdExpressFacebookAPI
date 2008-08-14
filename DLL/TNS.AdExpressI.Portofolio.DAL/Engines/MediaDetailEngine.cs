#region Information
// Author: D. Mussuma
// Creation date: 12/08/2008
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
using WebFunctions = TNS.AdExpress.Web.Functions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Classification;

using TNS.AdExpress.Web.Exceptions;
using CustormerConstantes = TNS.AdExpress.Constantes.Customer;
using CstProject = TNS.AdExpress.Constantes.Project;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Exceptions;

namespace TNS.AdExpressI.Portofolio.DAL.Engines {
	/// <summary>
	/// Get different data for portofolio media detail
	/// </summary>
	public class MediaDetailEngine : Engine {
		
		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicleInformation">Vehicle information</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		public MediaDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string periodBeginning, string periodEnd)
			: base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd) {
		}
		#endregion

		#region Implementation abstract methods
		/// <summary>
		/// Get data for media detail
		/// </summary>	
		/// <returns>Data set</returns>
		protected override DataSet ComputeData() {
			switch (_vehicleInformation.Id) {
				case DBClassificationConstantes.Vehicles.names.radio:					
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					return GetCommercialBreak();
				default: throw new PortofolioDALException("The method to get data is not defined for this vehicle.");
			}
		}
		#endregion

		#region GetCommercialBreak
		/// <summary>
		/// Get Commercial Break For Tv & Radio
		/// </summary>
		/// <returns>Liste des codes ecrans</returns>
		protected virtual DataSet GetCommercialBreak() {

			#region Variables
			string selectFields = "";
			string tableName = "";
			string groupByFields = "";
			string listProductHap = "";
			string product = "";
			string productsRights = "";
			string mediaRights = "";
			string sql = "";
			#endregion

			#region Build query

			try {
				selectFields = GetFieldsDetailMedia();
				tableName = WebFunctions.SQLGenerator.GetVehicleTableNameForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert);
				groupByFields = GetGroupByDetailMedia();
				listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
				product = GetProductData();
				productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
				mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to init query parameters", err));
			}

			sql += "select " + selectFields;
			sql += " from " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label + "." + tableName + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + " ";
			sql += " where id_media=" + _idMedia + "  ";
			sql += " and date_media_num>=" + _beginingDate + " ";
			sql += " and date_media_num<=" + _endDate + " ";
			sql += " and insertion=1 ";
			sql += listProductHap;
			sql += product;
			sql += productsRights;
			sql += mediaRights;
			sql += groupByFields;


			#endregion

			#region Execution query
			try {
				return _webSession.Source.Fill(sql.ToString());
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to load data for detail media portoflolio : " + sql.ToString(), err));
			}
			#endregion

		}
		#endregion

		#region Get Fields Detail Media For Tv & Radio
		/// <summary>
		/// Get Fields Detail Media For Tv & Radio
		/// </summary>
		/// <returns>SQL</returns>
		protected virtual string GetFieldsDetailMedia() {
			string sql = "";
			switch (_vehicleInformation.Id) {
				case DBClassificationConstantes.Vehicles.names.radio:
					sql += " sum(insertion) as insertion ";
					sql += ",commercial_break as code_ecran";
					sql += ",sum(expenditure_euro) value ";
					sql += " , date_media_num  ";
					return sql;
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					sql += " sum(insertion) as insertion ";
					sql += ",id_commercial_break as code_ecran";
					sql += ",sum(expenditure_euro) value ";
					sql += ",date_media_num  ";
					return sql;
				default:
					throw new PortofolioDALException("GetFieldsDetailMedia : Vehicle unknown.");
			}
		}
		#endregion

		#region Get Group By Detail Media For Tv & Radio
        /// <summary>
        /// Get Group By Detail Media For Tv & Radio
        /// </summary>
        /// <returns>SQL</returns>
        protected virtual string GetGroupByDetailMedia() {
            switch (_vehicleInformation.Id) {
                case DBClassificationConstantes.Vehicles.names.radio:
                    return "group by date_media_num ,commercial_break order by commercial_break";
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    return "group by date_media_num ,id_commercial_break order by id_commercial_break";
                default:
                    throw new PortofolioDALException("GetGroupByDetailMediaForTvRadio()-->Vehicle unknown.");
            }
        }
        #endregion		

	}
}
