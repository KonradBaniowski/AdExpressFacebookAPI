#region Information
// Author: D. Mussuma
// Creation date: 11/08/2008
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
	/// Get different data for portofolio detail
	/// </summary>
	public class PortofolioDetailEngine : Engine {

		#region PortofolioDetailEngine
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicleInformation">Vehicle information</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		public PortofolioDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string periodBeginning, string periodEnd)
			: base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd) {
		}
		#endregion

		#region GetGenericData
		/// <summary>
		/// Get data for media detail
		/// </summary>	
		/// <returns>Data set</returns>
		protected override DataSet ComputeData() {

			#region Variables
			string sql = string.Empty, sql4M = string.Empty, sqlDataVehicle = string.Empty, sqlWebPlan = string.Empty;
			string groupByFieldNameWithoutTablePrefix = string.Empty;
			string orderFieldName = string.Empty, orderFieldNameWithoutTablePrefix = string.Empty;
			string productFieldNameWithoutTablePrefix = string.Empty, unitField = string.Empty, dataFieldsForGadWithoutTablePrefix = string.Empty;
			CustomerPeriod customerPeriod = _webSession.CustomerPeriodSelected;
			#endregion

			#region Construction de la requête
			try {
				orderFieldName = _webSession.GenericProductDetailLevel.GetSqlOrderFields();
				orderFieldNameWithoutTablePrefix = _webSession.GenericProductDetailLevel.GetSqlOrderFieldsWithoutTablePrefix();
				groupByFieldNameWithoutTablePrefix = _webSession.GenericProductDetailLevel.GetSqlGroupByFieldsWithoutTablePrefix();

				if (customerPeriod.Is4M) {
					sql4M = GetRequest(DBConstantes.TableType.Type.dataVehicle4M);
					sql4M += " order by " + orderFieldName + "," + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media ";
					sql = sql4M;
				}
				else if (!customerPeriod.IsDataVehicle && !customerPeriod.IsWebPlan) {
					sql = GetRequest(DBConstantes.TableType.Type.dataVehicle);
					sql += " order by " + orderFieldName + "," + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media ";
				}
				else {

					if (customerPeriod.IsDataVehicle) {
						sqlDataVehicle = GetRequest(DBConstantes.TableType.Type.dataVehicle);
						sql = sqlDataVehicle;
					}

					if (customerPeriod.IsWebPlan) {
						sqlWebPlan = GetRequest(DBConstantes.TableType.Type.webPlan);
						sql = sqlWebPlan;
					}

					if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
						productFieldNameWithoutTablePrefix = _webSession.GenericProductDetailLevel.GetSqlFieldsWithoutTablePrefix();
						if (_webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser))
							dataFieldsForGadWithoutTablePrefix = ", " + WebFunctions.SQLGenerator.GetFieldsAddressForGad("");
						sql = "";
						sql += " select id_media, " + productFieldNameWithoutTablePrefix + dataFieldsForGadWithoutTablePrefix + ", " + WebFunctions.SQLGenerator.getUnitFieldsNameForMediaDetailResult(_vehicleInformation.Id, TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO);
						sql += " from (";
						sql += sqlDataVehicle;
						sql += " UNION ";
						sql += sqlWebPlan;
						sql += " ) ";
						sql += " group by id_media, " + groupByFieldNameWithoutTablePrefix + dataFieldsForGadWithoutTablePrefix;
					}

					sql += " order by " + orderFieldNameWithoutTablePrefix + ", id_media ";
				}
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to build detail media Portotofolio query " + sql, err));
			}
			#endregion

			#region Execution de la requête
			try {
				return _webSession.Source.Fill(sql.ToString());
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to exectute detail media Portotofolio query" + sql, err));
			}

			#endregion

		}
		#endregion

		#region GetRequest
		/// <summary>
		/// Build detail media portofolio sql query
		/// </summary>		
		/// <param name="type">Table Type  </param>
		/// <returns>Sql query</returns>
		protected virtual string GetRequest(DBConstantes.TableType.Type type) {
			#region Variables
			string dateField = "";
			string dataTableName = "";
			string dataTableNameForGad = "";
			string dataFieldsForGad = "";
			string dataJointForGad = "";
			string dataJointForInsert = "";
			string detailProductTablesNames = "";
			string detailProductFields = "";
			string detailProductJoints = "";
			string detailProductOrderBy = "";
			string unitFields = "";
			string productsRights = "";
			string sql = "";
			string list = "";
			string mediaList = "";
			bool premier;
			string mediaRights = "";
			string listProductHap = "";
			string mediaAgencyTable = string.Empty;
			string mediaAgencyJoins = string.Empty;
			CustomerPeriod customerPeriod = _webSession.CustomerPeriodSelected;
			#endregion

			#region Construction de la requête
			try {
				//Data table			
				switch (type) {
					case DBConstantes.TableType.Type.dataVehicle4M:
						dataTableName = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert);
						break;
					case DBConstantes.TableType.Type.dataVehicle:
						dataTableName = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.analysis);
						break;
					case DBConstantes.TableType.Type.webPlan:
						dataTableName = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.monthData).SqlWithPrefix; //DBConstantes.Tables.WEB_PLAN_MEDIA_MONTH;
						break;
					default:
						throw (new CompetitorDataAccessException("Table type unknown"));
				}
				detailProductTablesNames = _webSession.GenericProductDetailLevel.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA);
				detailProductFields = _webSession.GenericProductDetailLevel.GetSqlFields();
				detailProductJoints = _webSession.GenericProductDetailLevel.GetSqlJoins(_webSession.DataLanguage, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
				unitFields = WebFunctions.SQLGenerator.getUnitFieldsNameForMediaDetailResult(_vehicleInformation.Id, type, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
				detailProductOrderBy = _webSession.GenericProductDetailLevel.GetSqlOrderFields();

				switch (type) {
					case DBConstantes.TableType.Type.dataVehicle4M:
					case DBConstantes.TableType.Type.dataVehicle:
						dateField = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + DBConstantes.Fields.DATE_MEDIA_NUM;
						break;
					case DBConstantes.TableType.Type.webPlan:
						dateField = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + DBConstantes.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD;
						break;
				}
				productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
				mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
				listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
				//option inset (for veicle press)
				if (DBClassificationConstantes.Vehicles.names.press == _vehicleInformation.Id || DBClassificationConstantes.Vehicles.names.internationalPress == _vehicleInformation.Id)
					dataJointForInsert = WebFunctions.SQLGenerator.GetJointForInsertDetail(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, type);
				if (_webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser)) {
					try {
						dataTableNameForGad = ", " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.gad).SqlWithPrefix;
						dataFieldsForGad = ", " + WebFunctions.SQLGenerator.GetFieldsAddressForGad();
						dataJointForGad = "and " + WebFunctions.SQLGenerator.GetJointForGad(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
					}
					catch (PortofolioDALException) { ;}
				}
			}
			catch (System.Exception err) {
				throw (new PortofolioAnalysisDataAccessException("Impossible d'initialiser les paramètres de la requêtes", err));
			}
			if (WebFunctions.CheckedText.IsStringEmpty(dataTableName.ToString().Trim())) {
				if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
					sql += " select " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media, " + detailProductFields + dataFieldsForGad + ", " + dateField + " as date_num, " + unitFields;
				else
					sql += " select " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media, " + detailProductFields + dataFieldsForGad + "," + unitFields;
				sql += " from " + mediaAgencyTable + dataTableName;
				if (detailProductTablesNames.Length > 0)
					sql += ", " + detailProductTablesNames;
				sql += " " + dataTableNameForGad;
				// Period
				switch (type) {
					case DBConstantes.TableType.Type.dataVehicle4M:
						sql += " where " + dateField + " >=" + customerPeriod.StartDate;
						sql += " and " + dateField + " <=" + customerPeriod.EndDate;
						break;
					case DBConstantes.TableType.Type.dataVehicle:
						if (_webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 0) {
							sql += " where " + dateField + " >=" + customerPeriod.StartDate;
							sql += " and " + dateField + " <=" + customerPeriod.EndDate;
						}
						else if (_webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 2) {
							sql += " where ((" + dateField + " >=" + customerPeriod.PeriodDayBegin[0];
							sql += " and " + dateField + " <=" + customerPeriod.PeriodDayEnd[0];
							sql += " ) or (" + dateField + " >=" + customerPeriod.PeriodDayBegin[1];
							sql += " and " + dateField + " <=" + customerPeriod.PeriodDayEnd[1];
							sql += "))";
						}
						else {
							sql += " where " + dateField + " >=" + customerPeriod.PeriodDayBegin[0];
							sql += " and " + dateField + " <=" + customerPeriod.PeriodDayEnd[0];
						}
						break;
					case DBConstantes.TableType.Type.webPlan:
						sql += " where " + dateField + " >=" + customerPeriod.PeriodMonthBegin[0].ToString().Substring(0, 6);
						sql += " and " + dateField + " <=" + customerPeriod.PeriodMonthEnd[0].ToString().Substring(0, 6);
						break;
				}
				// Joints Products
				sql += " " + detailProductJoints;
				sql += " " + dataJointForGad;
				sql += " " + mediaAgencyJoins;
				//Joints inset
				if (DBClassificationConstantes.Vehicles.names.press == _vehicleInformation.Id || DBClassificationConstantes.Vehicles.names.internationalPress == _vehicleInformation.Id)
					sql += " " + dataJointForInsert;

				#region Media selection
				mediaList += _webSession.GetSelection((TreeNode)_webSession.ReferenceUniversMedia, CustormerConstantes.Right.type.mediaAccess);
				if (mediaList.Length > 0) sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media in (" + mediaList + ")";
				#endregion

				#region Products Selection
				sql += GetProductData();
				#endregion

				// Rights products
				sql += " " + productsRights;
				sql += listProductHap;
				// Rights media
				sql += mediaRights;

				// Group by
				if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
					sql += " group by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media, " + detailProductFields + dataFieldsForGad + ", " + dateField;
				else
					sql += " group by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media, " + detailProductFields + dataFieldsForGad;
			}
			#endregion

			#region Execute query
			try {
				return sql.ToString();
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to build detail media query : " + sql, err));
			}
			#endregion

		}
		#endregion



	}
}
