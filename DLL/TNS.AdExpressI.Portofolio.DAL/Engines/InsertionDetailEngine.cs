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
	/// Get different data for portofolio insertion detail
	/// </summary>
	public class InsertionDetailEngine : Engine {
		#region Variables
		/// <summary>
		/// Screen code
		/// </summary>
		protected string _adBreak;
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicleInformation">Vehicle information</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		public InsertionDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string periodBeginning, string periodEnd, string adBreak)
			: base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd) {
			_adBreak = adBreak;	
		}
		#endregion

		#region ComputeData

		/// <summary>
		/// Get data for media detail insertion
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant média</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="code_ecran">code ecran</param>
		/// <param name="allPeriod">vrai si le détail des insertions concerne toute la période sélectionnée</param>
		/// <returns>liste des publicités pour un média donné</returns>
		protected override DataSet ComputeData() {

			#region Variables
			StringBuilder sql = new StringBuilder(5000);
			string sqlFields = "";
            string sqlGroupBy = "";
			string sqlConstraintFields = "";
			string sqlTables = "";
			string sqlConstraintTables = "";
			string listProductHap = "";
			string product = "";
			string productsRights = "";
			string mediaRights = "";
			string orderby = "";
			bool allPeriod = (_adBreak != null && _adBreak.Length > 0) ? false : true;
			#endregion

			try {
				sqlFields = _webSession.GenericInsertionColumns.GetSqlFields(null);
                sqlGroupBy = _webSession.GenericInsertionColumns.GetSqlGroupByFields(null);
                sqlConstraintFields = _webSession.GenericInsertionColumns.GetSqlConstraintFields();
				string tableName = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert); //WebFunctions.SQLGenerator.GetVehicleTableNameForAlertDetailResult(_vehicleName);
				sqlTables = _webSession.GenericInsertionColumns.GetSqlTables(WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label, null);
				sqlConstraintTables = _webSession.GenericInsertionColumns.GetSqlConstraintTables(DBConstantes.Schema.ADEXPRESS_SCHEMA);

				//Select
				sql.Append(" select distinct");
				if (sqlFields.Length > 0) sql.Append(" " + sqlFields);

                if (_webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.agenceMedia))
                {
                    sql.Append(" , advertising_agency");
                    sqlGroupBy += " , advertising_agency";
                }

                if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress)
                {
                    sql.Append(" , date_cover_num");
                    sqlGroupBy += " , date_cover_num";
                }

                if (sqlConstraintFields.Length > 0)
                {
                    sql.Append(" , " + sqlConstraintFields);//Fields for constraint management
                    sqlGroupBy += " , " + _webSession.GenericInsertionColumns.GetSqlConstraintGroupByFields();
                }

				sql.Append(" from ");
				sql.Append(" " + tableName + " ");

				if (_webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.agenceMedia))
					sql.Append(", " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).SqlWithPrefix);

				if (sqlTables.Length > 0) sql.Append(" ," + sqlTables);

				if (sqlConstraintTables.Length > 0)
					sql.Append(" , " + sqlConstraintTables);//Tables pour la gestion des contraintes métiers

				// Joints conditions
				sql.Append(" Where ");

				sql.Append(" " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media=" + _idMedia + " ");
				sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num>=" + _beginingDate + " ");
				sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num<=" + _endDate + " ");

				if (_webSession.GenericInsertionColumns.GetSqlJoins(_webSession.DataLanguage, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, null).Length > 0)
					sql.Append("  " + _webSession.GenericInsertionColumns.GetSqlJoins(_webSession.DataLanguage, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, null));
				sql.Append("  " + _webSession.GenericInsertionColumns.GetSqlContraintJoins());

				if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio && _adBreak != null && _adBreak.Length > 0)
					sql.Append("  and commercial_break = " + _adBreak + "");


				if ((_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.others )  && _adBreak != null && _adBreak.Length > 0) {
					sql.Append(" and id_commercial_break = " + _adBreak + "");
				}


				//listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
				listProductHap = GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
				product = GetProductData();
				productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
				mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
				orderby = GetOrderByDetailMedia(allPeriod);

				if (_webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.agenceMedia)) {
					sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).Prefix + ".id_advertising_agency(+)=" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_advertising_agency ");
					sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).Prefix + ".id_language(+)=" + _webSession.DataLanguage + " ");
					sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).Prefix + ".activation(+)<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ");
				}

				#region Droits
				//liste des produit hap
				sql.Append(listProductHap);
				sql.Append(product);
				sql.Append(productsRights);
				sql.Append(mediaRights);

				//Media Universe
				sql.Append( " " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));

				//Rights detail spot to spot TNT
				if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv
					&& !_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_DETAIL_DIGITAL_TV_ACCESS_FLAG)) {					
					string idTNTCategory = TNS.AdExpress.Domain.Lists.GetIdList(WebConstantes.GroupList.ID.category, WebConstantes.GroupList.Type.digitalTv);
					if (idTNTCategory != null && idTNTCategory.Length > 0) {
						sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_category not in (" + idTNTCategory + ")  ");
					}		
				}

				#endregion

                //Group by
                sql.Append(" group by ");
                if (sqlGroupBy.Length > 0) sql.Append(sqlGroupBy);


				// Order by
				sql.Append("  " +orderby);

			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to build the query " + sql.ToString(), err));
			}

			#region Query execution
			try {
				return _webSession.Source.Fill(sql.ToString());
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to exectute query of  media detail : " + sql.ToString(), err));
			}
			#endregion

		}
		#endregion

		#region GetOrderByDetailMedia
		/// <summary>
		/// Get order for media detail insertion
		/// </summary>
		/// <param name="allPeriod">True if is for all period</param>
		/// <returns>String SQL</returns>
		protected virtual string GetOrderByDetailMedia(bool allPeriod) {

			switch (_vehicleInformation.Id) {
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					if (allPeriod) return " order by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num," + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".Id_type_page,media_paging,id_product," + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_advertisement";
					else
						return " order by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".Id_type_page,media_paging, id_product," + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_advertisement";
				case DBClassificationConstantes.Vehicles.names.radio:
					if (allPeriod) return "order by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num," + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_top_diffusion";
					else return " order by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_top_diffusion";
				case DBClassificationConstantes.Vehicles.names.tv:
					// Top diffusion
					if (allPeriod)
						return " order by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num," + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".top_diffusion ";
					else return " order by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".top_diffusion ";
				case DBClassificationConstantes.Vehicles.names.others:
					// order by date, scrreen code 
					if (allPeriod)
						return " order by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num," + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_commercial_break ";
					else return " order by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_commercial_break";
				default:
					throw new PortofolioDALException("GetOrderByDetailMedia : This media is not treated. None related table.");
			}
		}
		#endregion



	}
}
