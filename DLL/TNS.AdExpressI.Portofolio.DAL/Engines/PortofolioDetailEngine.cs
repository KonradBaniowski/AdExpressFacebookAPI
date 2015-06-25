#region Information
// Author: D. Mussuma
// Creation date: 11/08/2008
// Modification date:
#endregion

using System;
using System.Data;
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

using TNS.AdExpress.Domain.Units;

using TNS.AdExpress.Web.Exceptions;
using CustormerConstantes = TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.Core;

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

			#region Construction de la requ�te
			try {
				orderFieldName = _webSession.GenericProductDetailLevel.GetSqlOrderFields();
				orderFieldNameWithoutTablePrefix = _webSession.GenericProductDetailLevel.GetSqlOrderFieldsWithoutTablePrefix();
				groupByFieldNameWithoutTablePrefix = _webSession.GenericProductDetailLevel.GetSqlGroupByFieldsWithoutTablePrefix();

				if (customerPeriod.IsSliding4M) {
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
                        string unitSelect = WebFunctions.SQLGenerator.GetUnitFieldsNameUnionForPortofolio(_webSession);
                        if(unitSelect!=null && unitSelect.Length>0) unitSelect = ", " + unitSelect;
                        sql += " select id_media, " + productFieldNameWithoutTablePrefix + dataFieldsForGadWithoutTablePrefix + unitSelect;
                        if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.adnettrack || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.evaliantMobile)
                        {
                            sql += string.Format(", {0}", UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.versionNb).Id.ToString());
                        }
						sql += " from (";
						sql += sqlDataVehicle;
						sql += " UNION ";
						sql += sqlWebPlan;
						sql += " ) ";
						sql += " group by id_media, " + groupByFieldNameWithoutTablePrefix + dataFieldsForGadWithoutTablePrefix;
                        if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.adnettrack || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.evaliantMobile)
                        {
                            sql += string.Format(", {0}",UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.versionNb).Id.ToString());
                        }
					}

					sql += " order by " + orderFieldNameWithoutTablePrefix + ", id_media ";
				}
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to build detail media Portotofolio query " + sql, err));
			}
			#endregion

			#region Execution de la requ�te
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
            string groupByOptional = string.Empty;
            string fromOptional = string.Empty;
            CustomerPeriod customerPeriod = _webSession.CustomerPeriodSelected;
			#endregion

			#region Construction de la requ�te
			try {
				//Data table			
				switch (type) {
					case DBConstantes.TableType.Type.dataVehicle4M:
						dataTableName = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert, _webSession.IsSelectRetailerDisplay);
						break;
					case DBConstantes.TableType.Type.dataVehicle:
						dataTableName = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.analysis, _webSession.IsSelectRetailerDisplay);
						break;
					case DBConstantes.TableType.Type.webPlan:
						dataTableName = WebApplicationParameters.GetDataTable(TableIds.monthData, _webSession.IsSelectRetailerDisplay).SqlWithPrefix; //DBConstantes.Tables.WEB_PLAN_MEDIA_MONTH;
						break;
					default:
						throw (new CompetitorDataAccessException("Table type unknown"));
				}
				detailProductTablesNames = _webSession.GenericProductDetailLevel.GetSqlTables(WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label);
				detailProductFields = _webSession.GenericProductDetailLevel.GetSqlFields();
				detailProductJoints = _webSession.GenericProductDetailLevel.GetSqlJoins(_webSession.DataLanguage, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

                if (_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.adnettrack && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.evaliantMobile) {
                    unitFields = WebFunctions.SQLGenerator.GetUnitFieldsName(_webSession, type, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                }
                else
                {
                    unitFields = GetUnitFieldsName(_webSession, type, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    groupByOptional = GetBannerGroupByOptional(type);
                }

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
                productsRights = WebFunctions.SQLGenerator.GetClassificationCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
				mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
				listProductHap = GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
				//option inset (for veicle press)
				if (DBClassificationConstantes.Vehicles.names.press == _vehicleInformation.Id
                    || DBClassificationConstantes.Vehicles.names.newspaper == _vehicleInformation.Id
                    || DBClassificationConstantes.Vehicles.names.magazine == _vehicleInformation.Id
                    || DBClassificationConstantes.Vehicles.names.internationalPress == _vehicleInformation.Id)
					dataJointForInsert = WebFunctions.SQLGenerator.GetJointForInsertDetail(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
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
				throw (new PortofolioAnalysisDataAccessException("Impossible d'initialiser les param�tres de la requ�tes", err));
			}
			if (WebFunctions.CheckedText.IsStringEmpty(dataTableName.ToString().Trim())) {
				if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
					sql += " select " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media, " + detailProductFields + dataFieldsForGad + ", " + dateField + " as date_num, " + unitFields;
				else
					sql += " select " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media, " + detailProductFields + dataFieldsForGad + "," + unitFields;
                sql += " from " + mediaAgencyTable + dataTableName + fromOptional;
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
                // Autopromo
                string idMediaLabel = string.Empty;

                if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.adnettrack || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.evaliantMobile)
                    idMediaLabel = "id_media_evaliant";
                else if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.mms)
                    idMediaLabel = "id_media_mms";

                if (_vehicleInformation.Autopromo && (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.adnettrack
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.evaliantMobile
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.mms)) {

                    Table tblAutoPromo = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.autoPromo);

                    if (_webSession.AutoPromo == WebConstantes.CustomerSessions.AutoPromo.exceptAutoPromoAdvertiser)
                        sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".auto_promotion = 0 ";
                    else if (_webSession.AutoPromo == WebConstantes.CustomerSessions.AutoPromo.exceptAutoPromoHoldingCompany) {
                        sql += " and (" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media, " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_holding_company) not in ( ";
                        sql += " select distinct " + idMediaLabel + ", id_holding_company ";
                        sql += " from " + tblAutoPromo.Sql + " ";
                        sql += " where " + idMediaLabel + " is not null ";
                        sql += " ) ";
                    }
                }

                sql += GetFormatClause(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                sql += GetPurchaseModeClause(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

				// Joints Products
				sql += " " + detailProductJoints;
				sql += " " + dataJointForGad;
				sql += " " + mediaAgencyJoins;
				//Joints inset
				if (DBClassificationConstantes.Vehicles.names.press == _vehicleInformation.Id
                    || DBClassificationConstantes.Vehicles.names.newspaper == _vehicleInformation.Id
                    || DBClassificationConstantes.Vehicles.names.magazine == _vehicleInformation.Id
                    || DBClassificationConstantes.Vehicles.names.internationalPress == _vehicleInformation.Id)
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
				sql +=" " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
				// Rights media
				sql += mediaRights;

				// Group by
                if(customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
                    sql += " group by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media, " + detailProductFields + dataFieldsForGad + ", " + dateField;
                }
                else {
                    sql += " group by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media, " + detailProductFields + dataFieldsForGad;
                }
                sql += groupByOptional;
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

	    protected virtual string GetBannerGroupByOptional(DBConstantes.TableType.Type type)
	    {
            //string groupByOptional = string.Format(type == DBConstantes.TableType.Type.webPlan 
            //                                           ? ",{0}.list_banners " : ",{0}.id_banners ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
            string groupByOptional = string.Format(type == DBConstantes.TableType.Type.webPlan
                                                       ? ",{0}.list_banners " : ",{0}.id_banners ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
	        return groupByOptional;
	    }

	    #endregion


        public static string GetUnitFieldsName(WebSession webSession, DBConstantes.TableType.Type type, string dataTablePrefixe) {
            List<UnitInformation> unitsList = webSession.GetValidUnitForResult();
            var sqlUnit = new StringBuilder();
            if(!string.IsNullOrEmpty(dataTablePrefixe))
                dataTablePrefixe += ".";
            else
                dataTablePrefixe = "";
            for(int i = 0; i < unitsList.Count; i++) {
                if(i > 0) sqlUnit.Append(",");
                switch(type) {
                    case DBConstantes.TableType.Type.dataVehicle:
                    case DBConstantes.TableType.Type.dataVehicle4M:
                        if(unitsList[i].Id != TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.versionNb)
                            sqlUnit.AppendFormat("sum({0}{1}) as {2}", dataTablePrefixe, unitsList[i].DatabaseField, unitsList[i].Id.ToString());
                        else
                            sqlUnit.AppendFormat("  to_char({1}) as {2}", dataTablePrefixe, unitsList[i].DatabaseField, unitsList[i].Id.ToString());                                              
                        break;
                    case DBConstantes.TableType.Type.webPlan:
                        if(unitsList[i].Id != TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.versionNb)
                            sqlUnit.AppendFormat("sum({0}{1}) as {2}", dataTablePrefixe, unitsList[i].DatabaseMultimediaField, unitsList[i].Id.ToString());
                        else
                            sqlUnit.AppendFormat("{1} as {2}", dataTablePrefixe, unitsList[i].DatabaseMultimediaField, unitsList[i].Id.ToString());
                            //sqlUnit.AppendFormat("to_char(" + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label + ".stragg2(t2.column_value)) as {2}", dataTablePrefixe, unitsList[i].DatabaseMultimediaField, unitsList[i].Id.ToString());
                        break;
                }
            }
            return sqlUnit.ToString();
        }

	}
}
