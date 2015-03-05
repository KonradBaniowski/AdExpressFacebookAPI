#region Information
// Author: D. Mussuma
// Creation date: 11/08/2008
// Modification date:
#endregion

using System;
using System.Data;
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
using CustormerConstantes = TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.Core.Exceptions;

namespace TNS.AdExpressI.Portofolio.DAL.Engines {
	/// <summary>
	/// Get different data for calendar of advertising activity
	/// </summary>
	public class CalendarEngine : Engine {

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
		public CalendarEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string periodBeginning, string periodEnd)
			: base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd) {
		}
		#endregion

		#region Calendar
		/// <summary>
		/// Get Portofolio calendar
		/// </summary>
		/// <returns>Calendar data set</returns>
		protected override DataSet ComputeData() {

			#region Variables
			string dataTableName = "";
			string dataTableNameForGad = "";
			string dataFieldsForGad = "";
			string dataJointForGad = "";
			string detailProductTablesNames = "";
			string detailProductFields = "";
			string detailProductJoints = "";
			string detailProductOrderBy = "";
            string unitFieldNameSumWithAlias = "";
			string productsRights = "";
			string sql = "";
			string list = "";
			//int positionUnivers=1;
			string mediaList = "";
			bool premier;
			string dataJointForInsert = "";
			string listProductHap = "";
			string mediaRights = "";
			string mediaAgencyTable = string.Empty;
			string mediaAgencyJoins = string.Empty;

            string dataGroupby = "";
			#endregion
            
			#region Construction de la requête

			dataTableName = GetQueryFields(dataTableName, ref detailProductTablesNames, ref detailProductFields, ref detailProductJoints,
                ref unitFieldNameSumWithAlias, ref dataGroupby, ref mediaRights, ref productsRights, ref detailProductOrderBy, 
                ref dataJointForInsert, ref listProductHap, ref dataTableNameForGad, ref dataFieldsForGad, ref dataJointForGad);

			sql += " select " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media, " + detailProductFields + dataFieldsForGad + ","+unitFieldNameSumWithAlias;
			sql += ", " + DBConstantes.Fields.DATE_MEDIA_NUM + "";
			sql += " from " + mediaAgencyTable + dataTableName;
			if (detailProductTablesNames.Length > 0)
				sql += ", " + detailProductTablesNames;
			sql += " " + dataTableNameForGad;
			// Période
			sql += " where " + DBConstantes.Fields.DATE_MEDIA_NUM +" >=" + _beginingDate;
			sql += " and " + DBConstantes.Fields.DATE_MEDIA_NUM + " <=" + _endDate;

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
                    sql += " and (" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media, " 
                        + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_holding_company) not in ( ";
                    sql += " select distinct " + idMediaLabel + ", id_holding_company ";
                    sql += " from " + tblAutoPromo.Sql + " ";
                    sql += " where " + idMediaLabel + " is not null ";
                    sql += " ) ";
                }
            }

            sql += GetFormatClause(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
            sql += GetPurchaseModeClause(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

			// Jointures Produits
			sql += " " + detailProductJoints;
			sql += " " + dataJointForGad;
			sql += " " + mediaAgencyJoins;
			//Jointures encart
			if (DBClassificationConstantes.Vehicles.names.press == _vehicleInformation.Id 
                || DBClassificationConstantes.Vehicles.names.internationalPress == _vehicleInformation.Id
                || DBClassificationConstantes.Vehicles.names.newspaper == _vehicleInformation.Id
                || DBClassificationConstantes.Vehicles.names.magazine == _vehicleInformation.Id
                )
				sql += " " + dataJointForInsert;

			#region Sélection de Médias
			mediaList += _webSession.GetSelection((TreeNode)_webSession.ReferenceUniversMedia, CustormerConstantes.Right.type.mediaAccess);
			if (mediaList.Length > 0) sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media in (" + mediaList + ")";
			#endregion

			#region Sélection de Produits
			sql += " " + GetProductData();
			#endregion

			//Media Universe
			sql += " " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

			// Media rights
			sql += " " + productsRights;
			// Products rights
			sql += mediaRights;
			sql += listProductHap;
			// Group by
			sql += " group by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media, " + detailProductFields + dataFieldsForGad;
			sql += "," + DBConstantes.Fields.DATE_MEDIA_NUM + "";
            
            sql += dataGroupby; // hashcode pour Evaliant
            
			// Order by
			sql += " order by " + detailProductOrderBy + "," + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media";
			sql += "," + DBConstantes.Fields.DATE_MEDIA_NUM + "";
			#endregion

			#region Execution de la requête
			try {
				return _webSession.Source.Fill(sql.ToString());
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to load data for  calendar of advertising activity : " + sql, err));
			}
			#endregion

		}

	    protected virtual string GetQueryFields(string dataTableName, ref string detailProductTablesNames, ref string detailProductFields, ref string detailProductJoints,
            ref string unitFieldNameSumWithAlias, ref string dataGroupby, ref string mediaRights, ref string productsRights, ref string detailProductOrderBy, ref string dataJointForInsert,
            ref string listProductHap, ref string dataTableNameForGad, ref string dataFieldsForGad, ref string dataJointForGad)
	    {
	        try
	        {
	            dataTableName = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert, _webSession.IsSelectRetailerDisplay);
	            detailProductTablesNames = _webSession.GenericProductDetailLevel.GetSqlTables(WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label);
	            detailProductFields = _webSession.GenericProductDetailLevel.GetSqlFields();
	            detailProductJoints = _webSession.GenericProductDetailLevel.GetSqlJoins(_webSession.DataLanguage, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

	            if (_webSession.GetSelectedUnit().Id != TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.versionNb)
	                unitFieldNameSumWithAlias = WebFunctions.SQLGenerator.GetUnitFieldNameSumWithAlias(_webSession, DBConstantes.TableType.Type.dataVehicle4M); //WebFunctions.SQLGenerator.GetUnitFieldName(_webSession);
	            else
	            {
	                unitFieldNameSumWithAlias = GetUnitFieldName(_webSession, DBConstantes.TableType.Type.dataVehicle4M);
	                dataGroupby = string.Format(",{0}.id_banners", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
	            }

	            mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
	            productsRights = WebFunctions.SQLGenerator.GetClassificationCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
	            detailProductOrderBy = _webSession.GenericProductDetailLevel.GetSqlOrderFields();
	            //option encarts (pour la presse)
	            if (DBClassificationConstantes.Vehicles.names.press == _vehicleInformation.Id || DBClassificationConstantes.Vehicles.names.internationalPress == _vehicleInformation.Id 
                    || DBClassificationConstantes.Vehicles.names.newspaper == _vehicleInformation.Id || DBClassificationConstantes.Vehicles.names.magazine == _vehicleInformation.Id)
	                dataJointForInsert = WebFunctions.SQLGenerator.GetJointForInsertDetail(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
	            listProductHap = GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
	            if (_webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser))
	            {
	                try
	                {
	                    dataTableNameForGad = dataTableNameForGad = ", " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.gad).SqlWithPrefix;
	                    dataFieldsForGad = ", " + WebFunctions.SQLGenerator.GetFieldsAddressForGad();
	                    dataJointForGad = "and " + WebFunctions.SQLGenerator.GetJointForGad(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
	                }
	                catch (SQLGeneratorException)
	                {
	                    ;
	                }
	            }
	        }
	        catch (Exception e)
	        {
	            throw (new PortofolioDALException("Impossible to init query parameters" + e.Message));
	        }
	        return dataTableName;
	    }


	    protected virtual string GetUnitFieldName(WebSession webSession, DBConstantes.TableType.Type type) {
            StringBuilder sql = new StringBuilder();
            switch(type) {
                case DBConstantes.TableType.Type.dataVehicle4M:
                case DBConstantes.TableType.Type.dataVehicle:
                    try {
                        UnitInformation unitInformation = _webSession.GetSelectedUnit();
                        if(unitInformation.Id != TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.versionNb)
                            sql.AppendFormat("sum({0}) as {1}", unitInformation.DatabaseField, unitInformation.Id.ToString());
                        else
                            sql.AppendFormat("{0} as {1}", unitInformation.DatabaseField, unitInformation.Id.ToString());
                        return sql.ToString();
                    }
                    catch {
                        throw new SQLGeneratorException("Not managed unit (Alert Module)");
                    }
                case DBConstantes.TableType.Type.webPlan:
                    try {
                        UnitInformation unitInformation = _webSession.GetSelectedUnit();
                        if(unitInformation.Id != TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.versionNb)
                            sql.AppendFormat("sum({0}) as {1}", unitInformation.DatabaseMultimediaField, unitInformation.Id.ToString());
                        else
                            sql.AppendFormat("{0} as {1}", unitInformation.DatabaseMultimediaField, unitInformation.Id.ToString());
                        return sql.ToString();
                    }
                    catch {
                        throw (new SQLGeneratorException("Not managed unit (Analysis Module)"));
                    }
                default:
                    throw (new SQLGeneratorException("The type of module is not managed for the selection of unit"));
            }
        }
		#endregion        
	}
}
