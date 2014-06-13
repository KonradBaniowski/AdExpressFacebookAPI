#region Information
// Author: D. Mussuma
// Creation date: 11/08/2008
// Modification date:
#endregion

using System;
using System.Data;
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
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Units;

namespace TNS.AdExpressI.Portofolio.DAL.Engines {
	/// <summary>
	/// Get different data for portofolio synthesis
	/// </summary>
	public class SynthesisEngine : Engine {

		#region Constantes
		//préfixe table à utiliser
		const string LIST_ENCART = "85,108,999";
		#endregion

        #region Variables
        /// <summary>
        /// Synthesis data type
        /// <example>Data for Investment or Number product</example>
        /// </summary>
        protected PortofolioSynthesis.dataType _synthesisDataType;
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
		/// <param name="module">Module</param>
		public SynthesisEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string periodBeginning, string periodEnd)
			: base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd) {
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicle">Vehicle</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		/// <param name="module">Module</param>
		/// <param name="synthesisDataType">Synthesis data type</param>
        public SynthesisEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string periodBeginning, string periodEnd, PortofolioSynthesis.dataType synthesisDataType)
			: base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd) {
            _synthesisDataType = synthesisDataType;
		}

		#endregion

		#region Implementation abstract methods
		/// <summary>
		/// Get data for syntheis results
		/// </summary>
		/// <returns></returns>
		protected override DataSet ComputeData() {
            bool verif = false;
            switch (_synthesisDataType) {
                case PortofolioSynthesis.dataType.media:
                    return GetMediaData();
				case PortofolioSynthesis.dataType.category :
					return GetCategoryData();
                case PortofolioSynthesis.dataType.mediaSeller:
                    return GetMediaSellerData();
                case PortofolioSynthesis.dataType.interestCenter:
                    return GetInterestCenterData();
                case PortofolioSynthesis.dataType.periodicity:
					return GetPeriodicityData();
                case PortofolioSynthesis.dataType.investment:
                    return GetSynthsesisUnitsData();
                case PortofolioSynthesis.dataType.periodSelected:
                    return GetPeriod();
                case PortofolioSynthesis.dataType.pageNumber:
                    return GetPage();
                case PortofolioSynthesis.dataType.typeSale:
                    return GetTypeSale();
                case PortofolioSynthesis.dataType.numberProduct:
                    return GetNumberAdvertiserProduct(PortofolioSynthesis.dataType.numberProduct);
                case PortofolioSynthesis.dataType.numberNewProductInTracking:
                    return NumberProduct(PortofolioSynthesis.dataType.numberNewProductInTracking);
                case PortofolioSynthesis.dataType.numberNewProductInVehicle:
                    return NumberProduct(PortofolioSynthesis.dataType.numberNewProductInVehicle);
                case PortofolioSynthesis.dataType.numberAdvertiser:
                    return GetNumberAdvertiserProduct(PortofolioSynthesis.dataType.numberAdvertiser);
                case PortofolioSynthesis.dataType.adNumberIncludingInsets:
                    return NumberPageEncart(PortofolioSynthesis.dataType.adNumberIncludingInsets);
                case PortofolioSynthesis.dataType.adNumberExcludingInsets:
                    return NumberPageEncart(PortofolioSynthesis.dataType.adNumberExcludingInsets);
                case PortofolioSynthesis.dataType.numberAdBreaks:
                    return GetEcranData(); 
                case PortofolioSynthesis.dataType.numberBanners:
                    return GetNumberBanner();
                case PortofolioSynthesis.dataType.spotData:
                    return GetSpotData();
				default: throw new PortofolioDALException("The synthesis result type is not defined.");
			}
		}
		#endregion

		#region GetSynthsesisUnitsData
		///<summary>
		/// Get total investment, nb advet, spot duration
		/// </summary>		
		/// <returns>Query string</returns>
        protected virtual DataSet GetSynthsesisUnitsData() {

			#region Variables
			string sql = string.Empty, sql4M = string.Empty, sqlDataVehicle = string.Empty, sqlWebPlan = string.Empty;
			CustomerPeriod customerPeriod = _webSession.CustomerPeriodSelected;
			#endregion

			#region Construction de la requête
			try {
				if (customerPeriod.IsSliding4M) {
					sql4M = GetRequestForAnalysisPortofolio(DBConstantes.TableType.Type.dataVehicle4M);
					sql = sql4M;
				}
				else if (!customerPeriod.IsDataVehicle && !customerPeriod.IsWebPlan) {
					sql = GetRequestForAnalysisPortofolio(DBConstantes.TableType.Type.dataVehicle);
				}
				else {
					if (customerPeriod.IsDataVehicle) {
						sqlDataVehicle = GetRequestForAnalysisPortofolio(DBConstantes.TableType.Type.dataVehicle);
						sql = sqlDataVehicle;
					}

					if (customerPeriod.IsWebPlan) {
						sqlWebPlan = GetRequestForAnalysisPortofolio(DBConstantes.TableType.Type.webPlan);
						sql = sqlWebPlan;
					}

					if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
						sql = "";
						sql += " select " + WebFunctions.SQLGenerator.GetUnitFieldsNameUnionForPortofolio(_webSession);
						sql += " from (";
						sql += sqlDataVehicle;
						sql += " UNION ";
						sql += sqlWebPlan;
						sql += " ) ";
					}

				}
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to build query for total investment, nb ad, sport duration" + sql, err));
			}
			#endregion

			#region Execution de la requête
			try {
				return _webSession.Source.Fill(sql.ToString());
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to exectue query" + sql, err));
			}
			#endregion

		}
		#endregion

		#region GetRequestForAnalysisPortofolio
		/// <summary>
		/// Build query to get total investment, nd advert, spot duration
		/// </summary>	
		/// <param name="type">Type table </param>
		/// <returns>Query string</returns>
		protected virtual string GetRequestForAnalysisPortofolio(DBConstantes.TableType.Type type) {

			#region Build Sql query
			CustomerPeriod customerPeriod = _webSession.CustomerPeriodSelected;

			string table = string.Empty;
			//Data table			
			switch (type) {
				case DBConstantes.TableType.Type.dataVehicle4M:
					table = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert, _webSession.IsSelectRetailerDisplay);
					break;
				case DBConstantes.TableType.Type.dataVehicle:
                    table = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.analysis, _webSession.IsSelectRetailerDisplay);
					break;
				case DBConstantes.TableType.Type.webPlan:
					table = WebApplicationParameters.GetDataTable(TableIds.monthData, _webSession.IsSelectRetailerDisplay).SqlWithPrefix; //DBConstantes.Tables.WEB_PLAN_MEDIA_MONTH;
					break;
				default:
					throw (new PortofolioDALException("Table type unknown"));
			}
			string product = GetProductData();
			string mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
            string productsRights = WebFunctions.SQLGenerator.GetClassificationCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
			//list product hap
			string listProductHap = GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
			string date = string.Empty;

			switch (type) {
				case DBConstantes.TableType.Type.dataVehicle4M:
				case DBConstantes.TableType.Type.dataVehicle:
					date = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + DBConstantes.Fields.DATE_MEDIA_NUM;
					break;
				case DBConstantes.TableType.Type.webPlan:
					date = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + DBConstantes.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD;
					break;
			}

			StringBuilder sql = new StringBuilder();

            sql.AppendFormat("select {0}",WebFunctions.SQLGenerator.GetUnitFieldsNameForPortofolio(_webSession, type));         
            if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan && !customerPeriod.IsSliding4M) {
                sql.AppendFormat(", {0} as date_num ",date);
			}
           
            sql.AppendFormat(" from {0} where id_media={1}", table, _idMedia);           

            // Autopromo
            string idMediaLabel = string.Empty;

            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.adnettrack || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.evaliantMobile)
                idMediaLabel = "id_media_evaliant";
            else if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.mms)
                idMediaLabel = "id_media_mms";

            if ((_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.adnettrack
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.evaliantMobile
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.mms)) {

                Table tblAutoPromo = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.autoPromo);

                if (_webSession.AutoPromo == WebConstantes.CustomerSessions.AutoPromo.exceptAutoPromoAdvertiser)
                    sql.Append(" and auto_promotion = 0 ");
                else if (_webSession.AutoPromo == WebConstantes.CustomerSessions.AutoPromo.exceptAutoPromoHoldingCompany) {
                    sql.Append(" and (id_media, id_holding_company) not in ( ");
                    sql.Append(" select distinct " + idMediaLabel + ", id_holding_company ");
                    sql.Append(" from " + tblAutoPromo.Sql + " ");
                    sql.Append(" ) ");
                }
            }     

            sql.Append(GetFormatClause(null));

			// Period
			switch (type) {
				case DBConstantes.TableType.Type.dataVehicle4M:
                    sql.AppendFormat(" and {0}>={1} and {0}<={2}", date, customerPeriod.StartDate, customerPeriod.EndDate);
					break;
				case DBConstantes.TableType.Type.dataVehicle:
					if (_webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 0) {
						sql.AppendFormat(" and {0}>={1} and {0}<={2}", date, customerPeriod.StartDate, customerPeriod.EndDate);
					}
					else if (_webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 2) {
                        sql.AppendFormat(" and (({0}>={1} and {0}<={2}) or ({0}>={3} and {0}<={4}))"
                            , date
                            , customerPeriod.PeriodDayBegin[0]
                            , customerPeriod.PeriodDayEnd[0]
                            , customerPeriod.PeriodDayBegin[1]
                            , customerPeriod.PeriodDayEnd[1]);
					}
					else {
                        sql.AppendFormat(" and {0}>={1} and {0}<={2}", date, customerPeriod.PeriodDayBegin[0], customerPeriod.PeriodDayEnd[0]);
					}
					break;
				case DBConstantes.TableType.Type.webPlan:
                    sql.AppendFormat(" and {0}>={1} and {0}<={2}"
                        , date
                        , customerPeriod.PeriodMonthBegin[0].ToString().Substring(0, 6)
                        , customerPeriod.PeriodMonthEnd[0].ToString().Substring(0, 6));
					break;
			}
			
            sql.Append(product);
			sql.Append(productsRights);
			sql.Append(mediaRights);
			sql.Append(" " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
			sql.Append(listProductHap);
			if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan && !customerPeriod.IsSliding4M) {
				sql.AppendFormat(" group by {0}",date);
			}
			#endregion

			return sql.ToString();

		}
		#endregion

		#region GetRequestForNumberProductAdvertiser
		/// <summary>
		/// Get Nb product and advetiser for a one media
		/// </summary>
		/// <param name="type">Type table </param>
        /// <param name="dataType">Data type</param>
		/// <returns>object []	at index 0 the nb of product,  at index 1 the nb of advertiser</returns>
        protected virtual string GetRequestForNumberProductAdvertiser(DBConstantes.TableType.Type type, PortofolioSynthesis.dataType dataType) {

			#region Variables
			string sql = "";
			string tableName = "";
			string productsRights = "";
			string product = "";
			string mediaRights = "";
			string listProductHap = "";
			string date = "";
			CustomerPeriod customerPeriod = _webSession.CustomerPeriodSelected;
			#endregion

			try {
				// Table de données
				switch (type) {
					case DBConstantes.TableType.Type.dataVehicle4M:
                        tableName = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert, _webSession.IsSelectRetailerDisplay);
						break;
					case DBConstantes.TableType.Type.dataVehicle:
                        tableName = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.analysis, _webSession.IsSelectRetailerDisplay);
						break;
					case DBConstantes.TableType.Type.webPlan:
						tableName = WebApplicationParameters.GetDataTable(TableIds.monthData, _webSession.IsSelectRetailerDisplay).SqlWithPrefix;
						break;
					default:
						throw (new PortofolioDALException("Table type unknown"));
				}
				product = GetProductData();
				mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                productsRights = WebFunctions.SQLGenerator.GetClassificationCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
				listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
				switch (type) {
					case DBConstantes.TableType.Type.dataVehicle4M:
					case DBConstantes.TableType.Type.dataVehicle:
						date = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + DBConstantes.Fields.DATE_MEDIA_NUM;
						break;
					case DBConstantes.TableType.Type.webPlan:
						date = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + DBConstantes.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD;
						break;
				}
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to build sql query", err));
			}

			#region Construction de la requête
			if (dataType == PortofolioSynthesis.dataType.numberProduct) {
                sql = " select distinct id_product";
			}
            else if (dataType == PortofolioSynthesis.dataType.numberAdvertiser) {
                sql = " select distinct id_advertiser";
			}
			sql += " from " + tableName;
			sql += " where id_media=" + _idMedia + "";

            // Autopromo
            string idMediaLabel = string.Empty;

            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.adnettrack || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.evaliantMobile)
                idMediaLabel = "id_media_evaliant";
            else if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.mms)
                idMediaLabel = "id_media_mms";

            if ((_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.adnettrack
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.evaliantMobile
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.mms)) {

                Table tblAutoPromo = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.autoPromo);

                if (_webSession.AutoPromo == WebConstantes.CustomerSessions.AutoPromo.exceptAutoPromoAdvertiser)
                    sql += " and auto_promotion = 0 ";
                else if (_webSession.AutoPromo == WebConstantes.CustomerSessions.AutoPromo.exceptAutoPromoHoldingCompany) {
                    sql += " and (id_media, id_holding_company) not in ( ";
                    sql += " select distinct " + idMediaLabel + ", id_holding_company ";
                    sql += " from " + tblAutoPromo.Sql + " ";
                    sql += " ) ";
                }
            }

            sql += GetFormatClause(null);

			// Period
			switch (type) {
				case DBConstantes.TableType.Type.dataVehicle4M:
					sql += " and " + date + " >=" + customerPeriod.StartDate;
					sql += " and " + date + " <=" + customerPeriod.EndDate;
					break;
				case DBConstantes.TableType.Type.dataVehicle:
					if (_webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 0) {
						sql += " and " + date + " >=" + customerPeriod.StartDate;
						sql += " and " + date + " <=" + customerPeriod.EndDate;
					}
					else if (_webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 2) {
						sql += " and ((" + date + " >=" + customerPeriod.PeriodDayBegin[0];
						sql += " and " + date + " <=" + customerPeriod.PeriodDayEnd[0];
						sql += " ) or (" + date + " >=" + customerPeriod.PeriodDayBegin[1];
						sql += " and " + date + " <=" + customerPeriod.PeriodDayEnd[1];
						sql += "))";
					}
					else {
						sql += " and " + date + " >=" + customerPeriod.PeriodDayBegin[0];
						sql += " and " + date + " <=" + customerPeriod.PeriodDayEnd[0];
					}
					break;
				case DBConstantes.TableType.Type.webPlan:
					sql += " and " + date + " >=" + customerPeriod.PeriodMonthBegin[0].ToString().Substring(0, 6);
					sql += " and " + date + " <=" + customerPeriod.PeriodMonthEnd[0].ToString().Substring(0, 6);
					break;
			}
			sql += product;
			sql += productsRights;
			sql += mediaRights;
			sql += GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
			sql += listProductHap;
            if (dataType == PortofolioSynthesis.dataType.numberProduct) {
                sql += " group by id_product ";
			}
            else if (dataType == PortofolioSynthesis.dataType.numberAdvertiser) {
				sql += " group by id_advertiser ";
			}
			#endregion

			return sql;
		}
		#endregion

        #region Ecran
        /// <summary>
        /// récupère les écrans
        /// </summary>
        /// <returns>Ecrans</returns>
        public virtual DataSet GetEcranData() {

            #region Variables
            string select = "";
            string table = "";
            string product = "";
            string productsRights = "";
            string mediaRights = "";
            string listProductHap = "";
            StringBuilder sql = new StringBuilder();
            UnitInformation unitInformation = UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.insertion);
            #endregion

            #region Construction de la requête
            try {
                select = GetSelectDataEcran();
                table = GetTableData(true);
                product = GetProductData();
                productsRights = WebFunctions.SQLGenerator.GetClassificationCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
                mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to build the request for GetEcranData()", err));
            }

            sql.AppendFormat("select sum({0}) as {0},sum(ecran_duration) as ecran_duration ,sum(nbre_spot) as nbre_spot"
                , unitInformation.Id.ToString());

            sql.Append(" from ( ");

            sql.Append(select);
            sql.AppendFormat(" from {0}.{1} wp ",WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label,table);
            sql.AppendFormat(" where id_media={0} ",_idMedia);
            if (_beginingDate.Length > 0)
                sql.AppendFormat(" and date_media_num>={0} ",_beginingDate);
            if (_endDate.Length > 0)
                sql.AppendFormat(" and date_media_num<={0} ", _endDate);

            sql.AppendFormat(" and {0}={1}"
                            , unitInformation.DatabaseField
                            , this._cobrandindConditionValue);


            sql.Append(product);
            sql.Append(productsRights);
            sql.Append(mediaRights);
			sql.Append(" " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            sql.Append(listProductHap);
            sql.Append(" )");

            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for GetEcranData() : " + sql.ToString(), err));
            }
            #endregion

        }
        #endregion

        #region Get Select Data Ecran
        /// <summary>
        /// Get Select Data Ecran
        /// </summary>
        /// <returns>SQL</returns>
        protected virtual string GetSelectDataEcran() {
            UnitInformation unitInformation = UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.insertion);
            string sql = "";
            switch (_vehicleInformation.Id) {
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.czinternet:
                case DBClassificationConstantes.Vehicles.names.internet:
                    return "";
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                    sql += " select  distinct ID_COBRANDING_ADVERTISER";
                    sql += " ,duration_commercial_break as ecran_duration";
                    sql += " , NUMBER_spot_com_break nbre_spot";
                    sql += " ," + unitInformation.DatabaseField + " as " + unitInformation.Id.ToString() + " ";
                    return sql;
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                    sql += "select  distinct id_commercial_break ";
                    sql += " ,duration_commercial_break as ecran_duration";
                    sql += " ,NUMBER_MESSAGE_COMMERCIAL_BREA nbre_spot ";
                    sql += " ," + unitInformation.DatabaseField + " as " + unitInformation.Id.ToString() + " ";
                    return sql;
                default:
                    throw new PortofolioDALException("getTable(DBClassificationConstantes.Vehicles.value idMedia)-->Vehicle unknown.");
            }
        }
        #endregion

        #region Get Spot Data
        /// <summary>
        /// Get information about spots
        /// </summary>
        /// <returns>Spot data</returns>
        public virtual DataSet GetSpotData() {

            #region Variables
            string select = "";
            string table = "";
            string product = "";
            string productsRights = "";
            string mediaRights = "";
            string listProductHap = "";
            StringBuilder sql = new StringBuilder();
            UnitInformation unitInformation = UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.insertion);
            bool isAlertModule = _webSession.CustomerPeriodSelected.IsSliding4M;
            if (isAlertModule == false) {
                DateTime DateBegin = WebFunctions.Dates.getPeriodBeginningDate(_beginingDate, _webSession.PeriodType);
                if (DateBegin > DateTime.Now)
                    isAlertModule = true;
            }
            #endregion

            #region Construction de la requête
            try {
                select = GetSelectSpotData();
                table = GetTableData(isAlertModule);
                product = GetProductData();
                productsRights = WebFunctions.SQLGenerator.GetClassificationCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
                mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to build the request for GetSpotData()", err));
            }

            sql.AppendFormat("select round(AVG(NUMBER_SPOT_COM_BREAK),3) as avg_spot_nb, SUM(NUMBER_SPOT_COM_BREAK_WAP) as sum_spot_nb_wap, round(Avg(DURATION_COMMERCIAL_BREAK),3) as avg_dur_com_break, Sum(DURATION_COMMERCIAL_BREAK) as sum_dur_com_break, count(*) as com_break_nb ");

            sql.Append(" from ( ");

            sql.Append(select);
            sql.AppendFormat(" from {0}.{1} wp ", WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label, table);
            sql.AppendFormat(" where id_media={0} ", _idMedia);
            if (_beginingDate.Length > 0)
                sql.AppendFormat(" and date_media_num>={0} ", _beginingDate);
            if (_endDate.Length > 0)
                sql.AppendFormat(" and date_media_num<={0} ", _endDate);

            sql.AppendFormat(" and {0}={1}"
                            , unitInformation.DatabaseField
                            , this._cobrandindConditionValue);


            sql.Append(product);
            sql.Append(productsRights);
            sql.Append(mediaRights);
            sql.Append(" " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            sql.Append(listProductHap);
            sql.Append(" )");

            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for GetSpotData() : " + sql.ToString(), err));
            }
            #endregion

        }
        #endregion

        #region Get Select Spot Data
        /// <summary>
        /// Get Select Spot Data
        /// </summary>
        /// <returns>SQL</returns>
        protected virtual string GetSelectSpotData() {

            UnitInformation unitInformation = UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.insertion);
            string sql = "";
            switch (_vehicleInformation.Id) {
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.czinternet:
                case DBClassificationConstantes.Vehicles.names.internet:
                    return "";
                case DBClassificationConstantes.Vehicles.names.radio:
                    sql += " select  distinct date_media_num";
                    sql += " ,COMMERCIAL_BREAK";
                    sql += " , DURATION_COMMERCIAL_BREAK";
                    sql += " , DURATION_COM_BREAK_WAP";
                    sql += " , NUMBER_SPOT_COM_BREAK";
                    sql += " , NUMBER_SPOT_COM_BREAK_WAP ";
                    return sql;
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    sql += "select  distinct id_commercial_break ";
                    sql += " ,duration_commercial_break as ecran_duration";
                    sql += " ,NUMBER_MESSAGE_COMMERCIAL_BREA nbre_spot ";
                    sql += " ," + unitInformation.DatabaseField + " as " + unitInformation.Id.ToString() + " ";
                    return sql;
                default:
                    throw new PortofolioDALException("getTable(DBClassificationConstantes.Vehicles.value idMedia)-->Vehicle unknown.");
            }
        }
        #endregion

		#region GetPage
		/// <summary>
		/// Get pages number
		/// </summary>
		/// <returns>DataSet</returns>
		protected DataSet GetPage() {

			#region Construction de la requête

			string sql = "select sum(number_page_media) page";
			sql += " from " + WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.alarmMedia);
			sql += " where " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).Prefix + ".id_media=" + _idMedia + " ";
			if (_beginingDate.Length > 0)//date_cover_num
				sql += " and  " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).Prefix + ".DATE_ALARM>=" + _beginingDate + " ";
			if (_endDate.Length > 0)//date_cover_num
				sql += " and  " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).Prefix + ".DATE_ALARM<=" + _endDate + " ";
			#endregion

			#region Execution de la requête
			try {
				return _webSession.Source.Fill(sql.ToString());
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to get data for GetPage(): " + sql, err));
			}
			#endregion

		}
		#endregion

		#region Type Sale outdoor
		/// <summary>
		/// Get Type Sale
		/// </summary>
		/// <returns>DataSet</returns>
		protected DataSet GetTypeSale() {

			#region Construction de la requête

			string table = GetTableData(true);

			string sql = "select distinct type_sale";
			sql += " from  " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql + table;
			sql += " where id_media=" + _idMedia + " ";
			if (_beginingDate.Length > 0)
				sql += " and  date_media_num>=" + _beginingDate + " ";
			if (_endDate.Length > 0)
				sql += " and  date_media_num<=" + _endDate + " ";

			#endregion

			#region Execution de la requête
			try {
				return _webSession.Source.Fill(sql.ToString());
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to get data for GetTypeSale(): " + sql, err));
			}
			#endregion

		}
		#endregion

		#region Get Tables
		/// <summary>
		/// Get Tables 
		/// </summary>
		/// <returns>Sql From clause</returns>
		protected virtual string GetTable() {
			string sql = "";
			sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.media) + ", ";
			sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.basicMedia) + ", ";
			sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.mediaSeller) + ", ";
			sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.category) + ", ";
			sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.interestCenter);

			switch (_vehicleInformation.Id) {
				case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
				case DBClassificationConstantes.Vehicles.names.press:
					sql += "," + WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.periodicity);
					return sql;
				case DBClassificationConstantes.Vehicles.names.radio:
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
				case DBClassificationConstantes.Vehicles.names.outdoor:
                case DBClassificationConstantes.Vehicles.names.instore:
                case DBClassificationConstantes.Vehicles.names.indoor:
                case DBClassificationConstantes.Vehicles.names.czinternet:
                case DBClassificationConstantes.Vehicles.names.internet:
				case DBClassificationConstantes.Vehicles.names.directMarketing:
                case DBClassificationConstantes.Vehicles.names.mailValo:
					return sql;
				default:
					throw new PortofolioDALException("getTable()-->There is no table for this vehicle.");
			}
		}
		#endregion

		#region Get Table Data
		/// <summary>
		/// Get Table
		/// </summary>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
		/// Throw when the vehicle is unknown
		/// </exception>
		/// <returns>Table name</returns>
        protected virtual string GetTableData(bool isAlertModule) {
			switch (_vehicleInformation.Id) {
                case DBClassificationConstantes.Vehicles.names.newspaper:
                    return WebApplicationParameters.GetDataTable(isAlertModule ? TableIds.dataNewspaperAlert : TableIds.dataNewspaper, _webSession.IsSelectRetailerDisplay).Label;
                case DBClassificationConstantes.Vehicles.names.magazine:
                    return WebApplicationParameters.GetDataTable(isAlertModule ? TableIds.dataMagazineAlert : TableIds.dataMagazine, _webSession.IsSelectRetailerDisplay).Label;
				case DBClassificationConstantes.Vehicles.names.internationalPress:
                    return WebApplicationParameters.GetDataTable(isAlertModule ? TableIds.dataPressInterAlert : TableIds.dataPressInter, _webSession.IsSelectRetailerDisplay).Label;
				case DBClassificationConstantes.Vehicles.names.press:
                    return WebApplicationParameters.GetDataTable(isAlertModule ? TableIds.dataPressAlert : TableIds.dataPress, _webSession.IsSelectRetailerDisplay).Label;
				case DBClassificationConstantes.Vehicles.names.radio:
                    return WebApplicationParameters.GetDataTable(isAlertModule ? TableIds.dataRadioAlert : TableIds.dataRadio, _webSession.IsSelectRetailerDisplay).Label;
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
                    return WebApplicationParameters.GetDataTable(isAlertModule ? TableIds.dataTvAlert : TableIds.dataTv, _webSession.IsSelectRetailerDisplay).Label;
				case DBClassificationConstantes.Vehicles.names.outdoor:
                    return WebApplicationParameters.GetDataTable(isAlertModule ? TableIds.dataOutDoorAlert : TableIds.dataOutDoor, _webSession.IsSelectRetailerDisplay).Label;
                case DBClassificationConstantes.Vehicles.names.indoor:
                    return WebApplicationParameters.GetDataTable(isAlertModule ? TableIds.dataInDoorAlert : TableIds.dataInDoor, _webSession.IsSelectRetailerDisplay).Label;
                case DBClassificationConstantes.Vehicles.names.instore:
                    return WebApplicationParameters.GetDataTable(isAlertModule ? TableIds.dataInStoreAlert : TableIds.dataInStore, _webSession.IsSelectRetailerDisplay).Label;
				case DBClassificationConstantes.Vehicles.names.directMarketing:
                    return WebApplicationParameters.GetDataTable(isAlertModule ? TableIds.dataMarketingDirectAlert : TableIds.dataMarketingDirect, _webSession.IsSelectRetailerDisplay).Label;
                case DBClassificationConstantes.Vehicles.names.mailValo:
                    return WebApplicationParameters.GetDataTable(isAlertModule ? TableIds.dataMailAlert : TableIds.dataMail, _webSession.IsSelectRetailerDisplay).Label;
                case DBClassificationConstantes.Vehicles.names.czinternet:
                case DBClassificationConstantes.Vehicles.names.internet:
                    return WebApplicationParameters.GetDataTable(isAlertModule ? TableIds.dataInternetAlert : TableIds.dataInternet, _webSession.IsSelectRetailerDisplay).Label;
                case DBClassificationConstantes.Vehicles.names.cinema:
                    return WebApplicationParameters.GetDataTable(isAlertModule ? TableIds.dataCinemaAlert : TableIds.dataCinema, _webSession.IsSelectRetailerDisplay).Label;
                case DBClassificationConstantes.Vehicles.names.adnettrack:
                    return WebApplicationParameters.GetDataTable(isAlertModule ? TableIds.dataAdNetTrackAlert : TableIds.dataAdNetTrack, _webSession.IsSelectRetailerDisplay).Label;
                case DBClassificationConstantes.Vehicles.names.evaliantMobile:
                    return WebApplicationParameters.GetDataTable(isAlertModule ? TableIds.dataEvaliantMobileAlert : TableIds.dataEvaliantMobile, _webSession.IsSelectRetailerDisplay).Label;
                case DBClassificationConstantes.Vehicles.names.mms:
                    return WebApplicationParameters.GetDataTable(isAlertModule ? TableIds.dataMmsAlert : TableIds.dataMms, _webSession.IsSelectRetailerDisplay).Label;
                default:
					throw new PortofolioDALException("GetTableData()-->Vehicle unknown.");
			}
		}
		#endregion

        #region Get Media
        /// <summary>
        /// Get Media
        /// </summary>
        /// <returns>Media</returns>
        protected virtual DataSet GetMediaData() {

            #region Variables
            string sql = "";
            #endregion

            #region Construction de la requête
            try {
                sql += " select media";

                sql += " from ";
                sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.media);

                sql += " where " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media=" + _idMedia + "";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_language=" + _webSession.DataLanguage + " ";

            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to build the request", err));
            }
            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for GetMediaData() : " + sql, err));
            }
            #endregion

        }
        #endregion

        #region Get Category
        /// <summary>
        /// Get Category
        /// </summary>
        /// <returns>Category</returns>
        protected virtual DataSet GetCategoryData() {

            #region Variables
            string sql = "";
            #endregion

            #region Construction de la requête
            try {
                sql += " select category";

                sql += " from ";
                sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.media) + ", ";
                sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.basicMedia) + ", ";
                sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.category);

                sql += " where " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media=" + _idMedia + "";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_basic_media=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix + ".id_basic_media";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix + ".id_category=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_category ";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_language=" + _webSession.DataLanguage + " ";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix + ".id_language=" + _webSession.DataLanguage + " ";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_language=" + _webSession.DataLanguage + " ";
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to build the request", err));
            }
            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for GetCategoryData() : " + sql, err));
            }
            #endregion

        }
        #endregion

        #region Get Media Seller
        /// <summary>
        /// Get Media Seller
        /// </summary>
        /// <returns>Media Seller</returns>
        protected virtual DataSet GetMediaSellerData() {

            #region Variables
            string sql = "";
            #endregion

            #region Construction de la requête
            try {
                sql += " select media_seller";

                sql += " from ";
                sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.media) + ", ";
                sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.mediaSeller);

                sql += " where " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media=" + _idMedia + "";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media_seller=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.mediaSeller).Prefix + ".id_media_seller";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_language=" + _webSession.DataLanguage + " ";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.mediaSeller).Prefix + ".id_language=" + _webSession.DataLanguage + " ";

            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to build the request", err));
            }
            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for GetMediaSellerData() : " + sql, err));
            }
            #endregion

        }
        #endregion

        #region Get Interest Center
        /// <summary>
        /// Get Interest Center
        /// </summary>
        /// <returns>Interest Center</returns>
        protected virtual DataSet GetInterestCenterData() {

            #region Variables
            string sql = "";
            #endregion

            #region Construction de la requête
            try {
                sql += " select interest_center";

                sql += " from ";
                sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.media) + ", ";
                sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.interestCenter);

                sql += " where " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media=" + _idMedia + "";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_interest_center=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.interestCenter).Prefix + ".id_interest_center";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_language=" + _webSession.DataLanguage + " ";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.interestCenter).Prefix + ".id_language=" + _webSession.DataLanguage + " ";

            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to build the request", err));
            }
            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for GetInterestCenterData() : " + sql, err));
            }
            #endregion

        }
        #endregion

        #region Get Periodicity
        /// <summary>
        /// Get Periodicity
        /// </summary>
        /// <returns>Periodicity</returns>
        protected virtual DataSet GetPeriodicityData() {

            #region Variables
            string sql = "";
            #endregion

            #region Construction de la requête
            try {
                sql += " select periodicity";

                sql += " from ";
                sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.media) + ", ";
                sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.periodicity);

                sql += " where " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media=" + _idMedia + "";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.periodicity).Prefix + ".id_periodicity=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_periodicity";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_language=" + _webSession.DataLanguage + " ";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.periodicity).Prefix + ".id_language=" + _webSession.DataLanguage + " ";

            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to build the request", err));
            }
            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for GetPeriodicityData() : " + sql, err));
            }
            #endregion

        }
        #endregion

        #region GetInvestment
        /// <summary>
        /// Get total investment and date of issue
        /// </summary>
        /// <returns>DataSet</returns>
        protected virtual DataSet GetInvestment() {

            #region Construction de la requête
            string table = GetTableData(true);
            string product = GetProductData();
            string productsRights = WebFunctions.SQLGenerator.GetClassificationCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
            string mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
            //liste des produit hap
            string listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
            StringBuilder sql = new StringBuilder();

            UnitInformation unitInformation = UnitsInformation.Get(UnitsInformation.DefaultCurrency);
            sql.AppendFormat("select sum({0}) as {1}",unitInformation.DatabaseField,unitInformation.Id.ToString());

            sql.AppendFormat(" from {0}{1} {2}", WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql, table, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
            sql.AppendFormat(" where id_media = {0}",_idMedia);
            if (_beginingDate.Length > 0)
                sql.AppendFormat(" and date_media_num>={0} ",_beginingDate);
            if (_endDate.Length > 0)
                sql.AppendFormat(" and date_media_num<={0} ",_endDate);

            sql.Append(product);
            sql.Append(productsRights);
            sql.Append(mediaRights);
			sql.Append(" " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            sql.Append(listProductHap);
            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for GetInvestment(): " + sql.ToString(), err));
            }
            #endregion

        }
        #endregion

        #region Get Period
        /// <summary>
        /// Get date of issue
        /// </summary>
        /// <returns>DataSet</returns>
        protected virtual DataSet GetPeriod() {

            #region Construction de la requête
            string table = GetTableData(true);
            string product = GetProductData();
            string productsRights = WebFunctions.SQLGenerator.
                GetClassificationCustomerProductRight(_webSession,
                WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
            string mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession,
                WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
            //liste des produit hap
            string listProductHap = WebFunctions.SQLGenerator.
                GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,
                WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);

            string sql;
                
                sql = "select  min(DATE_MEDIA_NUM) first_date, max(DATE_MEDIA_NUM) last_date";

            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.outdoor
                 || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.indoor
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.instore)
                sql = "select min(DATE_CAMPAIGN_BEGINNING) first_date, max(DATE_CAMPAIGN_END) last_date";

            sql += " from " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql 
                + table + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + " ";
            sql += " where id_media = " + _idMedia + "";
            if (_beginingDate.Length > 0)
                sql += " and date_media_num>=" + _beginingDate + " ";
            if (_endDate.Length > 0)
                sql += " and date_media_num<=" + _endDate + "";

            sql += product;
            sql += productsRights;
			sql += GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
            sql += mediaRights;
            sql += listProductHap;
            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for GetPeriod(): " + sql, err));
            }
            #endregion

        }
        #endregion

        #region Get NumberProduct
        /// <summary>
        /// Get Number of products, Number of new products in the advert tracking selection
        /// or Number of new products in the vehicle
        /// </summary>
        /// <returns>Données</returns>
        protected DataSet NumberProduct(PortofolioSynthesis.dataType dataType) {

            #region Variables
            string sql = "";
            string tableName = "";
            string productsRights = null;
            string product = null;
            string mediaRights = null;
            string listProductHap = null;
            #endregion

            #region Construction de la requête
            try {
                tableName = GetTableData(true);
                productsRights = WebFunctions.SQLGenerator.GetClassificationCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
                product = GetProductData();
                mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);

                sql += " select count(id_product) as nbLines ";
                sql += " from( ";
                sql += " select  id_product ";
                sql += " from " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql + tableName + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
                sql += " where id_media=" + _idMedia + "";
                // Support
                if (dataType == PortofolioSynthesis.dataType.numberNewProductInVehicle)
                    sql += " and new_product=1 ";
                // Pige
                if (dataType == PortofolioSynthesis.dataType.numberNewProductInTracking)
                    sql += " and new_product=2 ";
                if (_beginingDate.Length > 0)
                    sql += " and  DATE_MEDIA_NUM>= " + _beginingDate + " ";
                if (_endDate.Length > 0)
                    sql += " and  DATE_MEDIA_NUM<= " + _endDate + " ";

                sql += product;
                sql += productsRights;
				sql += GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                sql += mediaRights;
                sql += listProductHap;

                sql += " group by id_product )";

            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to build request for : NumberProductAdvertiser()" + sql, err));
            }
            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for NumberProduct(PortofolioSynthesis.resultType): " + sql, err));
            }
            #endregion

        }
        #endregion

        #region Get NumberAdvertiser
        /// <summary>
        /// Get number of advertisers
        /// </summary>
        /// <returns>Number of advertisers</returns>
        protected DataSet NumberAdvertiser() {

            #region Variables
            string sql = "";
            string tableName = "";
            string productsRights = null;
            string product = null;
            string mediaRights = null;
            string listProductHap = null;
            #endregion

            #region Construction de la requête
            try {
                tableName = GetTableData(true);
                productsRights = WebFunctions.SQLGenerator.GetClassificationCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
                product = GetProductData();
                mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);

                sql += " select count(id_advertiser) as nbLines ";
                sql += " from( ";
                sql += " select  id_advertiser ";

                sql += " from " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql + tableName + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
                sql += " where id_media=" + _idMedia + "";

                if (_beginingDate.Length > 0)
                    sql += " and  DATE_MEDIA_NUM>= " + _beginingDate + " ";
                if (_endDate.Length > 0)
                    sql += " and  DATE_MEDIA_NUM<= " + _endDate + " ";

                sql += product;
                sql += productsRights;
                sql += mediaRights;
				sql += GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                sql += listProductHap;

                sql += " group by id_advertiser )";

            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to build request for : NumberProductAdvertiser()" + sql, err));
            }
            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for NumberAdvertiser(): " + sql, err));
            }
            #endregion

        }
        #endregion

        #region GetNumberAdvertiserProduct
        /// <summary>
        /// Get Nb product or nb advertiser for one media
        /// </summary>
        /// <param name="dataType">Data Type</param>
        /// <returns>nb product</returns>
        protected virtual DataSet GetNumberAdvertiserProduct(PortofolioSynthesis.dataType dataType) {

            #region Variables
            string sql = string.Empty, sqlGlobal = string.Empty, sql4M = string.Empty, sqlDataVehicle = string.Empty, sqlWebPlan = string.Empty;
            CustomerPeriod customerPeriod = _webSession.CustomerPeriodSelected;
            #endregion

            try {

                #region Request
                if (customerPeriod.IsSliding4M) {
                    sql4M = GetRequestForNumberProductAdvertiser(DBConstantes.TableType.Type.dataVehicle4M, dataType);
                    sql = sql4M;
                }
                else if (!customerPeriod.IsDataVehicle && !customerPeriod.IsWebPlan) {
                    sql = GetRequestForNumberProductAdvertiser(DBConstantes.TableType.Type.dataVehicle, dataType);
                }
                else {

                    if (customerPeriod.IsDataVehicle) {
                        sqlDataVehicle = GetRequestForNumberProductAdvertiser(DBConstantes.TableType.Type.dataVehicle, dataType);
                        sql = sqlDataVehicle;
                    }

                    if (customerPeriod.IsWebPlan) {
                        sqlWebPlan = GetRequestForNumberProductAdvertiser(DBConstantes.TableType.Type.webPlan, dataType);
                        sql = sqlWebPlan;
                    }

                    if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {

                        if (dataType == PortofolioSynthesis.dataType.numberProduct)
                            sql = " select distinct id_product";
                        else if (dataType == PortofolioSynthesis.dataType.numberAdvertiser)
                            sql = " select distinct id_advertiser";
                        sql += " from (";
                        sql += sqlDataVehicle;
                        sql += " UNION ";
                        sql += sqlWebPlan;
                        sql += " ) ";

                        if (dataType == PortofolioSynthesis.dataType.numberProduct)
                            sql += " group by id_product ";
                        else if (dataType == PortofolioSynthesis.dataType.numberAdvertiser)
                            sql += " group by id_advertiser ";
                    }
                }
                sqlGlobal = "select count(*) as nbLines from (";
                sqlGlobal += sql;
                sqlGlobal += " )";
                #endregion

                #region Execute query
                try {
                    return _webSession.Source.Fill(sqlGlobal);
                }
                catch (System.Exception err) {
                    throw (new PortofolioDALException("Impossible exectute query : " + sql, err));
                }
                #endregion

            }
            catch (System.Exception ex) {
                throw (new PortofolioDALException("Impossible to build table for nb product or nb advertiser : ", ex));
            }

        }
        #endregion

        #region NumberPageEncart
        /// <summary>
        /// Encart
        /// </summary>
        /// <param name="dataType">Data Type</param>
        /// <returns>Données</returns>
        protected virtual DataSet NumberPageEncart(PortofolioSynthesis.dataType dataType) {

            #region Variables
            StringBuilder sql = new StringBuilder();
            string tableName = "";
            string productsRights = null;
            string mediaRights = null;
            string product = null;
            string listProductHap = null;
            UnitInformation unitInformation = UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.pages);
            #endregion

            #region Build query
            try {
                tableName = GetTableData(true);
                productsRights = WebFunctions.SQLGenerator.GetClassificationCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
                mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                product = GetProductData();
                listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);

                sql.AppendFormat(" select sum({0}) as {1} "
                    , unitInformation.DatabaseField
                    , unitInformation.Id.ToString());
                sql.AppendFormat(" from {0}{1} {2}"
                    , WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql
                    , tableName
                    , WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                sql.AppendFormat(" where ID_MEDIA={0} ",_idMedia);
                // hors encart
                if (dataType == PortofolioSynthesis.dataType.adNumberExcludingInsets) {
                    sql.Append(" and id_inset=null ");
                }
                // Encart
                if (dataType == PortofolioSynthesis.dataType.adNumberIncludingInsets) {
                    sql.AppendFormat(" and id_inset in ({0}) ", _webSession.CustomerDataFilters.InsetTypesAsString);
                }
                if (_beginingDate.Length > 0)
                    sql.AppendFormat(" and  DATE_MEDIA_NUM>={0} ",_beginingDate);
                if (_endDate.Length > 0)
                    sql.AppendFormat(" and  DATE_MEDIA_NUM<={0} ", _endDate);

                sql.Append(product);
                sql.Append(productsRights);
                sql.Append(mediaRights);
				sql.Append(" " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
                sql.Append(listProductHap);

            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to build request for NumberPageEncart(): " + sql.ToString(), err));
            }
            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for NumberPageEncart(): " + sql.ToString(), err));
            }
            #endregion

        }
        #endregion

        #region Get NumberBanner (Evaliant)
        /// <summary>
        /// Get number of banners
        /// </summary>
        /// <returns>Number of banners</returns>
        protected virtual DataSet GetNumberBanner() {

            #region Variables
            string sql = string.Empty, sqlGlobal = string.Empty, sql4M = string.Empty, sqlDataVehicle = string.Empty, sqlWebPlan = string.Empty;
            CustomerPeriod customerPeriod = _webSession.CustomerPeriodSelected;
            #endregion

            #region Construction de la requête
            try {
                if(customerPeriod.IsSliding4M) {
                    sql4M = GetRequestForNumberBanner(DBConstantes.TableType.Type.dataVehicle4M);
                    sql = sql4M;
                }
                else if(!customerPeriod.IsDataVehicle && !customerPeriod.IsWebPlan) {
                    sql = GetRequestForNumberBanner(DBConstantes.TableType.Type.dataVehicle);
                }
                else {
                    if(customerPeriod.IsDataVehicle) {
                        sqlDataVehicle = GetRequestForNumberBanner(DBConstantes.TableType.Type.dataVehicle);
                        sql = sqlDataVehicle;
                    }

                    if(customerPeriod.IsWebPlan) {
                        sqlWebPlan = GetRequestForNumberBanner(DBConstantes.TableType.Type.webPlan);
                        sql = sqlWebPlan;
                    }

                    if(customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
                        sql = "";
                        sql += "select distinct hashcode";
                        sql += " from (";
                        sql += sqlDataVehicle;
                        sql += " UNION ";
                        sql += sqlWebPlan;
                        sql += " ) group by hashcode";
                    }
                }
                //sqlGlobal = "select count(hashcode) as nbLines from (";
                //sqlGlobal += sql;
                //sqlGlobal += " )";
            }
            catch(System.Exception err) {
                throw (new PortofolioDALException("Impossible to build query for total investment, nb ad, sport duration" + sql, err));
            }
            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch(System.Exception err) {
                throw (new PortofolioDALException("Impossible to exectue query" + sql, err));
            }
            #endregion
        }
        #endregion

        #region GetRequestForNumberBanner (Evaliant)
        /// <summary>
        /// Build query to get to number of banners
        /// </summary>
        /// <param name="type">Type table</param>
        /// <returns>Query string</returns>
        protected virtual string GetRequestForNumberBanner(DBConstantes.TableType.Type type) {

            #region Build Sql query
            CustomerPeriod customerPeriod = _webSession.CustomerPeriodSelected;

            string table = string.Empty;
            //Data table			
            switch(type) {
                case DBConstantes.TableType.Type.dataVehicle4M:
                    table = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert, _webSession.IsSelectRetailerDisplay);
                    break;
                case DBConstantes.TableType.Type.dataVehicle:
                    table = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.analysis, _webSession.IsSelectRetailerDisplay);
                    break;
                case DBConstantes.TableType.Type.webPlan:
                    table = WebApplicationParameters.GetDataTable(TableIds.monthData, _webSession.IsSelectRetailerDisplay).SqlWithPrefix; //DBConstantes.Tables.WEB_PLAN_MEDIA_MONTH;
                    break;
                default:
                    throw (new PortofolioDALException("Table type unknown"));
            }
            string product = GetProductData();
            string mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
            string productsRights = WebFunctions.SQLGenerator.GetClassificationCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
            //list product hap
            string listProductHap = GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
            string date = string.Empty;

            switch(type) {
                case DBConstantes.TableType.Type.dataVehicle4M:
                case DBConstantes.TableType.Type.dataVehicle:
                    date = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + DBConstantes.Fields.DATE_MEDIA_NUM;
                    break;
                case DBConstantes.TableType.Type.webPlan:
                    date = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + DBConstantes.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD;
                    break;
            }

            StringBuilder sql = new StringBuilder();

            if(type == DBConstantes.TableType.Type.webPlan) {
                sql.Append("select distinct list_banners as hashcode ");
            }
            else {

                sql.Append("select distinct to_char(id_banners) as hashcode ");
            }

            if(customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
                sql.AppendFormat(", {0} as date_num ", date);
            }
            sql.AppendFormat(" from {0} where id_media={1}", table, _idMedia);


            // Autopromo
            string idMediaLabel = string.Empty;

            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.adnettrack || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.evaliantMobile)
                idMediaLabel = "id_media_evaliant";
            else if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.mms)
                idMediaLabel = "id_media_mms";

            if ((_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.adnettrack
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.evaliantMobile
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.mms)) {

                Table tblAutoPromo = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.autoPromo);

                if (_webSession.AutoPromo == WebConstantes.CustomerSessions.AutoPromo.exceptAutoPromoAdvertiser)
                    sql.Append(" and auto_promotion = 0 ");
                else if (_webSession.AutoPromo == WebConstantes.CustomerSessions.AutoPromo.exceptAutoPromoHoldingCompany) {
                    sql.Append(" and (id_media, id_holding_company) not in ( ");
                    sql.Append(" select distinct " + idMediaLabel + ", id_holding_company ");
                    sql.Append(" from " + tblAutoPromo.Sql + " ");
                    sql.Append(" ) ");
                }
            }

            sql.Append(GetFormatClause(null));

            // Period
            switch(type) {
                case DBConstantes.TableType.Type.dataVehicle4M:
                    sql.AppendFormat(" and {0}>={1} and {0}<={2}", date, customerPeriod.StartDate, customerPeriod.EndDate);
                    break;
                case DBConstantes.TableType.Type.dataVehicle:
                    if(_webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 0) {
                        sql.AppendFormat(" and {0}>={1} and {0}<={2}", date, customerPeriod.StartDate, customerPeriod.EndDate);
                    }
                    else if(_webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 2) {
                        sql.AppendFormat(" and (({0}>={1} and {0}<={2}) or ({0}>={3} and {0}<={4}))"
                            , date
                            , customerPeriod.PeriodDayBegin[0]
                            , customerPeriod.PeriodDayEnd[0]
                            , customerPeriod.PeriodDayBegin[1]
                            , customerPeriod.PeriodDayEnd[1]);
                    }
                    else {
                        sql.AppendFormat(" and {0}>={1} and {0}<={2}", date, customerPeriod.PeriodDayBegin[0], customerPeriod.PeriodDayEnd[0]);
                    }
                    break;
                case DBConstantes.TableType.Type.webPlan:
                    sql.AppendFormat(" and {0}>={1} and {0}<={2}"
                        , date
                        , customerPeriod.PeriodMonthBegin[0].ToString().Substring(0, 6)
                        , customerPeriod.PeriodMonthEnd[0].ToString().Substring(0, 6));
                    break;
            }
            sql.Append(product);
            sql.Append(productsRights);
            sql.Append(mediaRights);
            sql.Append(" " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            sql.Append(listProductHap);
            sql.Append(" group by ");
            if(customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
                sql.AppendFormat(" {0},", date);
            }
            if (type == DBConstantes.TableType.Type.webPlan)
            {
                sql.Append("list_banners");
            }
            else
            {
                sql.Append("id_banners");
            }

            #endregion

            return sql.ToString();
        }
        #endregion

    }
}
