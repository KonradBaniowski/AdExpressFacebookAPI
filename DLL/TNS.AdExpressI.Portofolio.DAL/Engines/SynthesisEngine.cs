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
	/// Get different data for portofolio synthesis
	/// </summary>
	public class SynthesisEngine : Engine {

		#region Constantes
		//préfixe table à utiliser
		const string LIST_ENCART = "85,108,999";
		#endregion

		/// <summary>
		/// Synthesis result data type
		/// <example>dta for Investment or Number product</example>
		/// </summary>
		protected int _synthesisDataType = -1;

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
		public SynthesisEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string periodBeginning, string periodEnd, int synthesisDataType)
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
			switch (_synthesisDataType) {
				case PortofolioSynthesis.CATEGORY_MEDIA_SELLER_DATA:
					return GetCategoryMediaSellerData();
				case PortofolioSynthesis.INSERTION_NUMBER_DATA:
					return GetInsertionNumber();
				case PortofolioSynthesis.INVESTMENT_DATA:
					return (_webSession.CustomerPeriodSelected.Is4M && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.internet) ? GetInvestment() : GetSynthsesisUnitsData();
				case PortofolioSynthesis.TYPE_SALE_NUMBER_DATA:
					return GetTypeSale();
				case PortofolioSynthesis.NUMBER_PAGE_DATA:
					return GetPage();

				default: throw new PortofolioDALException("The synthesis result type is not defined.");
			}
		}
		#endregion

		/// <summary>
		/// Get Number
		/// </summary>
		/// <example>Get number advertiser, product or inset</example>
		/// <returns></returns>
		public virtual object[] GetNumber() {
			switch (_synthesisDataType) {

				case PortofolioSynthesis.NUMBER_PRODUCT_ADVERTISER_DATA:
					return (_webSession.CustomerPeriodSelected.Is4M && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.internet) ? NumberProductAdvertiser() : GetNumberProductAdvertiser();
				case PortofolioSynthesis.NUMBER_INSET_DATA :
					return NumberPageEncart();
				default: throw new PortofolioDALException("The synthesis result type is not defined.");
			}
		}		

		#region Category, Media Owner, Interest Center and Periodicity
		/// <summary>
		/// Get the following fields : Category, Media Owner, Interest Center and Periodicity for press
		/// </summary>
		/// <returns>Category, Media Owner, Interest Center and Periodicity for press</returns>
		protected virtual DataSet GetCategoryMediaSellerData() {

			#region Variables
			string sql = "";
			string tableName = "";
			string fields = "";
			string joint = "";
			string mediaRights = "";
			#endregion

			#region Construction de la requête
			try {
				tableName = GetTable();
				fields = GetFields();
				joint = GetJoint();
				mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix, true);
				sql += " select " + fields;
				sql += " from " + tableName;
				sql += " where " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media=" + _idMedia + "";
				sql += joint;
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
				throw (new PortofolioDALException("Impossible to get data for GetCategoryMediaSellerData() : " + sql, err));
			}
			#endregion

		}
		#endregion

		#region GetInsertionNumber
		/// <summary>
		/// Get insertions number
		/// </summary>
		/// <returns>DataSet</returns>
		protected virtual DataSet GetInsertionNumber() {

			#region Construction de la requête

			string table = GetTableData();
			string product = GetProductData();
			string productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
			string mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
			//liste des produit hap
			string listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);


			string insertionField = "insertion";
			if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.outdoor)
				insertionField = "NUMBER_BOARD";

			string sql = " select sum(" + insertionField + ") as insertion ";

			sql += " from " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql + table + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + " ";
			sql += " where id_media=" + _idMedia + "";
			if (_beginingDate.Length > 0)
				sql += " and date_media_num>=" + _beginingDate + " ";
			if (_endDate.Length > 0)
				sql += " and date_media_num<=" + _endDate + "";

			if (_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.outdoor) sql += " and insertion=1";
			sql += product;
			sql += productsRights;
			sql += mediaRights;
			sql += listProductHap;

			#endregion

			#region Execution de la requête
			try {
				return _webSession.Source.Fill(sql.ToString());
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to get data for GetInsertionNumber(): " + sql, err));
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
			string select = GetSelectData();
			string table = GetTableData();
			string product = GetProductData();
			string productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
			string mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
			//liste des produit hap
			string listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);

			string sql = select;

			sql += " from " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql + table + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + " ";
			sql += " where id_media = " + _idMedia + "";
			if (_beginingDate.Length > 0)
				sql += " and date_media_num>=" + _beginingDate + " ";
			if (_endDate.Length > 0)
				sql += " and date_media_num<=" + _endDate + "";

			sql += product;
			sql += productsRights;
			sql += mediaRights;
			sql += listProductHap;
			#endregion

			#region Execution de la requête
			try {
				return _webSession.Source.Fill(sql.ToString());
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to get data for GetInvestment(): " + sql, err));
			}
			#endregion

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
				if (customerPeriod.Is4M) {
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
					table = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert);
					break;
				case DBConstantes.TableType.Type.dataVehicle:
					table = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.analysis);
					break;
				case DBConstantes.TableType.Type.webPlan:
					table = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.monthData).SqlWithPrefix; //DBConstantes.Tables.WEB_PLAN_MEDIA_MONTH;
					break;
				default:
					throw (new PortofolioDALException("Table type unknown"));
			}
			string product = GetProductData();
			string mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
			string productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
			//list product hap
			string listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
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

			string sql = null;

			switch (type) {
				case DBConstantes.TableType.Type.dataVehicle4M:
				case DBConstantes.TableType.Type.dataVehicle:
					sql += "select " + WebFunctions.SQLGenerator.GetUnitFieldsNameForPortofolio(_webSession, DBConstantes.TableType.Type.dataVehicle);
					break;
				case DBConstantes.TableType.Type.webPlan:
                    sql += "select " + WebFunctions.SQLGenerator.GetUnitFieldsNameForPortofolio(_webSession, DBConstantes.TableType.Type.webPlan);
					break;
			}
			if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
				sql += ", " + date + " as date_num ";
			}
			sql += " from " + table;
			sql += " where id_media=" + _idMedia + "";
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
			sql += listProductHap;
			if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
				sql += " group by " + date;
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
		/// <param name="index">Iteration</param>
		/// <returns>object []	at index 0 the nb of product,  at index 1 the nb of advertiser</returns>
		protected virtual string GetRequestForNumberProductAdvertiser(DBConstantes.TableType.Type type, int index) {

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
						tableName = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert);
						break;
					case DBConstantes.TableType.Type.dataVehicle:
						tableName = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.analysis);
						break;
					case DBConstantes.TableType.Type.webPlan:
						tableName = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.monthData).SqlWithPrefix;
						break;
					default:
						throw (new PortofolioDALException("Table type unknown"));
				}
				product = GetProductData();
				mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
				productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
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
			if (index == 0) {
				if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
					sql = " select id_product, " + date + " as date_num, count(rowid) as nbLines";
				else
					sql = " select id_product, count(rowid) as nbLines";
			}
			else {
				if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
					sql = " select id_advertiser, " + date + " as date_num, count(rowid) as nbLines";
				else
					sql = " select id_advertiser, count(rowid) as nbLines";
			}
			sql += " from " + tableName;
			sql += " where id_media=" + _idMedia + "";
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
			sql += listProductHap;
			if (index == 0) {
				if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
					sql += " group by id_product, " + date;
				else
					sql += " group by id_product ";
			}
			else {
				if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
					sql += " group by id_advertiser, " + date;
				else
					sql += " group by id_advertiser ";
			}
			#endregion

			return sql;
		}
		#endregion

		#region GetNumberProductAdvertiser
		/// <summary>
		/// Get Nb product and Nb advertiser for one media
		/// </summary>		
		/// <returns>object [] at 0 the nb product,at 1 the nb advertiser</returns>
		protected virtual object[] GetNumberProductAdvertiser() {

			#region Variables
			string sql = string.Empty, sql4M = string.Empty, sqlDataVehicle = string.Empty, sqlWebPlan = string.Empty;
			CustomerPeriod customerPeriod = _webSession.CustomerPeriodSelected;
			object[] tab = new object[4];
			int compteur = 0;
			DataSet ds = null;
			#endregion

			try {

				for (int i = 0; i <= 1; i++) {

					if (customerPeriod.Is4M) {
						sql4M = GetRequestForNumberProductAdvertiser(DBConstantes.TableType.Type.dataVehicle4M, i);
						sql = sql4M;
					}
					else if (!customerPeriod.IsDataVehicle && !customerPeriod.IsWebPlan) {
						sql = GetRequestForNumberProductAdvertiser(DBConstantes.TableType.Type.dataVehicle, i);
					}
					else {

						if (customerPeriod.IsDataVehicle) {
							sqlDataVehicle = GetRequestForNumberProductAdvertiser(DBConstantes.TableType.Type.dataVehicle, i);
							sql = sqlDataVehicle;
						}

						if (customerPeriod.IsWebPlan) {
							sqlWebPlan = GetRequestForNumberProductAdvertiser(DBConstantes.TableType.Type.webPlan, i);
							sql = sqlWebPlan;
						}

						if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
							sql = "";
							if (i == 0)
								sql += " select id_product, sum(nbLines) as nbLines";
							else
								sql += " select id_advertiser, sum(nbLines) as nbLines";
							sql += " from (";
							sql += sqlDataVehicle;
							sql += " UNION ";
							sql += sqlWebPlan;
							sql += " ) ";
							if (i == 0) {
								sql += " group by id_product ";
							}
							else {
								sql += " group by id_advertiser ";
							}
						}
					}

					#region Execute query
					try {
						ds = _webSession.Source.Fill(sql);
					}
					catch (System.Exception err) {
						throw (new PortofolioDALException("Impossible exectute query : " + sql, err));
					}
					#endregion
					if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0) {
						compteur = ds.Tables[0].Rows.Count;
					}
					tab[i] = compteur;
					compteur = 0;
				}
			}
			catch (System.Exception ex) {
				throw (new PortofolioDALException("Impossible to build table for nb advertiser and product : ", ex));
			}


			return tab;
		}
		#endregion

		#region NumberProductAdvertiser
		/// <summary>
		/// Tableau contenant le nombre de produits, 
		/// le nombre de nouveau produit dans le support,
		/// le nombre de nouveau produit dans la pige,
		/// le nombre d'annonceurs		
		/// </summary>
		/// <returns>Données</returns>
		protected object[] NumberProductAdvertiser() {

			#region Variables
			object[] tab = new object[4];
			DataSet ds = null;
			string sql = "";
			string tableName = "";
			string productsRights = null;
			string product = null;
			string mediaRights = null;
			string listProductHap = null;
			int index = 0;
			#endregion

			#region Construction de la requête
			try {
				tableName = GetTableData();
				productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
				product = GetProductData();
				mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
				listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);

				for (int i = 0; i <= 3; i++) {
					if ((_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.outdoor) || (i != 2 && i != 1)) {
						if (i <= 2) {
							sql += " select count(id_product) as total ";
							sql += " from( ";
							sql += " select  id_product ";
						}
						else {
							sql += " select count(id_advertiser) as total ";
							sql += " from( ";
							sql += " select  id_advertiser ";
						}
						sql += " from " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql + tableName + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
						sql += " where id_media=" + _idMedia + "";
						// Support
						if (i == 1)
							sql += " and new_product=1 ";
						// Pige
						if (i == 2)
							sql += " and new_product=2 ";

						if (_beginingDate.Length > 0)
							sql += " and  DATE_MEDIA_NUM>= " + _beginingDate + " ";
						if (_endDate.Length > 0)
							sql += " and  DATE_MEDIA_NUM<= " + _endDate + " ";

						sql += product;
						sql += productsRights;
						sql += mediaRights;
						sql += listProductHap;

						if (i <= 2) {
							sql += " group by id_product )";
						}
						else {
							sql += " group by id_advertiser )";
						}

						if (i <= 2)
							sql += " UNION ALL ";
					}
				}

			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to build request for : NumberProductAdvertiser()" + sql, err));
			}
			#endregion

			#region Execution de la requête
			try {
				ds = _webSession.Source.Fill(sql.ToString());
				if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
					foreach (DataRow row in ds.Tables[0].Rows) {
						tab[index] = row["total"].ToString();
						if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.outdoor) {
							index = index + 3;
						}
						else
							index++;
					}
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to get data for NumberProductAdvertiser(): " + sql, err));
			}
			#endregion

			return tab;
		}
		#endregion

		#region NumberPageEncart
		/// <summary>
		/// Encart
		/// </summary>
		/// <returns>Données</returns>
		protected virtual object[] NumberPageEncart() {
			
			#region Variables
			object[] tab = new object[4];
			DataSet ds = null;
			string sql = "";
			string tableName = "";
			string productsRights = null;
			string mediaRights = null;
			string product = null;
			string listProductHap = null;
			int index = 0;
			#endregion

			#region Build query
			try {
				tableName = GetTableData();
				productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
				mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
				product = GetProductData();
				listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);

				for (int i = 0; i <= 2; i++) {

					sql += " select sum(area_page) as page ";
					sql += " from " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql + tableName + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
					sql += " where ID_MEDIA=" + _idMedia + " ";
					// hors encart
					if (i == 1) {
						sql += " and id_inset=null ";
					}
					// Encart
					if (i == 2) {
						sql += " and id_inset in (" + LIST_ENCART + ") ";
					}
					if (_beginingDate.Length > 0)
						sql += " and  DATE_MEDIA_NUM>=" + _beginingDate + " ";
					if (_endDate.Length > 0)
						sql += " and  DATE_MEDIA_NUM<=" + _endDate + " ";

					sql += product;
					sql += productsRights;
					sql += mediaRights;
					sql += listProductHap;

					if (i < 2)
						sql += " UNION ALL ";

				}
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to build request for NumberPageEncart(): " + sql, err));
			}
			#endregion

			#region Execution de la requête
			try {
				ds = _webSession.Source.Fill(sql.ToString());
				if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
					foreach (DataRow row in ds.Tables[0].Rows) {
						tab[index] = row["page"].ToString();
						index++;
					}

			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to get data for NumberPageEncart(): " + sql, err));
			}
			#endregion

			return tab;

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

			string table = GetTableData();

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

		#region Get Select Data
		/// <summary>
		/// Get Select Data
		/// </summary>
		/// <returns>SQL</returns>
		protected string GetSelectData() {
			switch (_vehicleInformation.Id) {
				case DBClassificationConstantes.Vehicles.names.internationalPress:
				case DBClassificationConstantes.Vehicles.names.press:
					return " select sum(EXPENDITURE_EURO) as euro, min(DATE_MEDIA_NUM) first_date, max(DATE_MEDIA_NUM) last_date ";
				case DBClassificationConstantes.Vehicles.names.radio:
                    return " select sum(EXPENDITURE_EURO) as euro, min(DATE_MEDIA_NUM) first_date, max(DATE_MEDIA_NUM) last_date, "
						+ " sum(insertion) as insertion, sum(duration) as duration";
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
                    return " select sum(EXPENDITURE_EURO) as euro, min(DATE_MEDIA_NUM) first_date, max(DATE_MEDIA_NUM) last_date, "
						+ " sum(insertion) as insertion, sum(duration) as duration";
				case DBClassificationConstantes.Vehicles.names.outdoor:
                    return " select sum(EXPENDITURE_EURO) as euro, min(DATE_CAMPAIGN_BEGINNING) first_date, max(DATE_CAMPAIGN_END) last_date, "
						+ " sum(NUMBER_BOARD) as number_board ";
				default:
					throw new PortofolioDALException("GetSelectData()-->Vehicle unknown.");
			}
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
				case DBClassificationConstantes.Vehicles.names.press:
					sql += "," + WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.periodicity);
					return sql;
				case DBClassificationConstantes.Vehicles.names.radio:
					return sql;
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
				case DBClassificationConstantes.Vehicles.names.outdoor:
				case DBClassificationConstantes.Vehicles.names.internet:
					return sql;
				case DBClassificationConstantes.Vehicles.names.directMarketing:
					return sql;
				default:
					throw new PortofolioDALException("getTable()-->There is no table for this vehicle.");
			}
		}
		#endregion

		#region Get Fields
		/// <summary>
		/// Get Fields
		/// </summary>
		/// <returns>SQL</returns>
		protected virtual string GetFields() {
			string sql = "category, media_seller, interest_center, media as support ";
			switch (_vehicleInformation.Id) {
				case DBClassificationConstantes.Vehicles.names.internationalPress:
				case DBClassificationConstantes.Vehicles.names.press:
					sql += ", periodicity ";
					return sql;
				case DBClassificationConstantes.Vehicles.names.radio:
					return sql;
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					return sql;
				case DBClassificationConstantes.Vehicles.names.outdoor:
				case DBClassificationConstantes.Vehicles.names.internet:
					return sql;
				case DBClassificationConstantes.Vehicles.names.directMarketing:
					return sql;
				default:
					throw new PortofolioDALException("GetFields()-->There are no fields for this vehicle.");
			}
		}
		#endregion

		#region Get joint
		/// <summary>
		/// Get sql joints
		/// </summary>
		/// <returns>SQL</returns>
		protected virtual string GetJoint() {
			string sql = "and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_basic_media=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix + ".id_basic_media";
			sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media_seller=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.mediaSeller).Prefix + ".id_media_seller";
			sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix + ".id_category=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_category ";
			sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_interest_center=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.interestCenter).Prefix + ".id_interest_center";
			sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_language=" + _webSession.DataLanguage + " ";
			sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix + ".id_language=" + _webSession.DataLanguage + " ";
			sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.mediaSeller).Prefix + ".id_language=" + _webSession.DataLanguage + " ";
			sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_language=" + _webSession.DataLanguage + " ";
			sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.interestCenter).Prefix + ".id_language=" + _webSession.DataLanguage + " ";

			switch (_vehicleInformation.Id) {
				case DBClassificationConstantes.Vehicles.names.internationalPress:
				case DBClassificationConstantes.Vehicles.names.press:
					sql += "and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.periodicity).Prefix + ".id_periodicity=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_periodicity";
					sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.periodicity).Prefix + ".id_language=" + _webSession.DataLanguage + " ";
					return sql;
				case DBClassificationConstantes.Vehicles.names.radio:
					return sql;
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
				case DBClassificationConstantes.Vehicles.names.outdoor:
				case DBClassificationConstantes.Vehicles.names.internet:
					return sql;
				case DBClassificationConstantes.Vehicles.names.directMarketing:
					return sql;
				default:
					throw new PortofolioDALException("GetJoint()-->Vehicle unknown");
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
		protected virtual string GetTableData() {
			switch (_vehicleInformation.Id) {
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					return DBConstantes.Tables.ALERT_DATA_PRESS_INTER;
				case DBClassificationConstantes.Vehicles.names.press:
					return DBConstantes.Tables.ALERT_DATA_PRESS;
				case DBClassificationConstantes.Vehicles.names.radio:
					return DBConstantes.Tables.ALERT_DATA_RADIO;
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					return DBConstantes.Tables.ALERT_DATA_TV;
				case DBClassificationConstantes.Vehicles.names.outdoor:
					return DBConstantes.Tables.ALERT_DATA_OUTDOOR;
				case DBClassificationConstantes.Vehicles.names.directMarketing:
					return DBConstantes.Tables.ALERT_DATA_MARKETING_DIRECT;
				default:
					throw new PortofolioDALException("GetTableData()-->Vehicle unknown.");
			}
		}
		#endregion

		

	}
}
