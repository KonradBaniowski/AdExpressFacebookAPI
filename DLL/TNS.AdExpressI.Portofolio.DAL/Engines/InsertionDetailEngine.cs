#region Information
// Author: D. Mussuma
// Creation date: 12/08/2008
// Modification date:
#endregion

using System;
using System.Data;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
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
        public InsertionDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string periodBeginning, string periodEnd)
            : base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd) { }
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
		/// <returns>liste des publicités pour un média donné</returns>
		protected override DataSet ComputeData() {

			#region Variables
			var sql = new StringBuilder(5000);
		    bool allPeriod = string.IsNullOrEmpty(_adBreak);
			#endregion

			try {
				string sqlFields = _webSession.GenericInsertionColumns.GetSqlFields(null);
                string sqlGroupBy = _webSession.GenericInsertionColumns.GetSqlGroupByFields(null);
                string sqlConstraintFields = _webSession.GenericInsertionColumns.GetSqlConstraintFields();
				string tableName = SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert,
                    _webSession.IsSelectRetailerDisplay);
				string sqlTables = _webSession.GenericInsertionColumns.GetSqlTables(WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label, null);
				string sqlConstraintTables = _webSession.GenericInsertionColumns.GetSqlConstraintTables(DBConstantes.Schema.ADEXPRESS_SCHEMA);

				//Select
				sql.Append(" select distinct");
				if (sqlFields.Length > 0) sql.AppendFormat(" {0}", sqlFields);

                if (_webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.agenceMedia))
                {
                    sql.Append(" , advertising_agency");
                    sqlGroupBy += " , advertising_agency";
                }

                 GetDateCoverField(sql, ref sqlGroupBy);

                if (sqlConstraintFields.Length > 0)
                {
                    sql.AppendFormat(" , {0}", sqlConstraintFields);//Fields for constraint management
                    sqlGroupBy += string.Format(" , {0}", _webSession.GenericInsertionColumns.GetSqlConstraintGroupByFields());
                }

				sql.Append(" from ");
				sql.AppendFormat(" {0} ", tableName);

				if (_webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.agenceMedia))
					sql.Append(", " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).SqlWithPrefix);

				if (sqlTables.Length > 0) sql.AppendFormat(" ,{0}", sqlTables);

				if (sqlConstraintTables.Length > 0)
					sql.Append(" , " + sqlConstraintTables);//Tables pour la gestion des contraintes métiers

				// Joints conditions
				sql.Append(" Where ");

				sql.AppendFormat(" {0}.id_media={1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _idMedia);
				sql.AppendFormat(" and {0}.date_media_num>={1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _beginingDate);
				sql.AppendFormat(" and {0}.date_media_num<={1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _endDate);

				if (_webSession.GenericInsertionColumns.GetSqlJoins(_webSession.DataLanguage,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, null).Length > 0)
					sql.Append("  " + _webSession.GenericInsertionColumns.GetSqlJoins(_webSession.DataLanguage,
                        WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, null));
				sql.Append("  " + _webSession.GenericInsertionColumns.GetSqlContraintJoins());

				if ((_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioGeneral
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioSponsorship
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioMusic)
                    && !string.IsNullOrEmpty(_adBreak))
					sql.AppendFormat("  and commercial_break = {0}", _adBreak);


				if ((_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv 
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.others
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvGeneral
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvSponsorship
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvNonTerrestrials
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvAnnounces)  
                    && !string.IsNullOrEmpty(_adBreak)) {
					sql.AppendFormat(" and id_commercial_break = {0}", _adBreak);
				}


				string listProductHap = GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
				string product = GetProductData();
                string productsRights = SQLGenerator.GetClassificationCustomerProductRight(_webSession,
                 WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
				string mediaRights = SQLGenerator.getAnalyseCustomerMediaRight(_webSession,
				WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
				string orderby = GetOrderByDetailMedia(allPeriod);

				if (_webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.agenceMedia)) {
					sql.AppendFormat(" and {0}.id_advertising_agency(+)={1}.id_advertising_agency ",
                        WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).Prefix,
                        WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
					sql.AppendFormat(" and {0}.id_language(+)={1} ",
                        WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).Prefix, _webSession.DataLanguage);
					sql.AppendFormat(" and {0}.activation(+)<{1} ", 
                        WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).Prefix,
                        TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
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
              && !_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_DETAIL_DIGITAL_TV_ACCESS_FLAG))
                {
                    string idTntCategory = TNS.AdExpress.Domain.Lists
                        .GetIdList(WebConstantes.GroupList.ID.category, WebConstantes.GroupList.Type.digitalTv);
                    if (!string.IsNullOrEmpty(idTntCategory))
                    {
                        sql.AppendFormat(" and {0}.id_category not in ({1})  ", WebApplicationParameters
                       .DataBaseDescription.DefaultResultTablePrefix, idTntCategory);
                    }
                }
               
				#endregion

                //Group by
                sql.Append(" group by ");
                if (sqlGroupBy.Length > 0) sql.Append(sqlGroupBy);


				// Order by
				sql.AppendFormat("  {0}", orderby);

			}
			catch (System.Exception err) {
				throw (new PortofolioDALException(string.Format("Impossible to build the query {0}", sql.ToString()), err));
			}

			#region Query execution
			try {
				return _webSession.Source.Fill(sql.ToString());
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException(string.Format("Impossible to exectute query of  media detail : {0}",
                    sql.ToString()), err));
			}
			#endregion

		}

	    protected virtual void GetDateCoverField(StringBuilder sql, ref string sqlGroupBy)
	    {
	        if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
	            || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress)
	        {
	            sql.Append(" , date_cover_num");
	            sqlGroupBy += " , date_cover_num";
	        }
	      
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
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
                    string res = " order by ";
                    if (allPeriod)                    
                        res += WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num,";
                    
                    res += WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".Id_type_page";
                    if (_webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.location)
                        || _webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.locationMagazine)
                        || _webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.locationNewsPaper))
                        res += "," + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_advertisement";
                    if (_webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.mediaPaging)) res += ",media_paging";
                    res += ",id_product";
               
                return res;
				case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
					if (allPeriod) return "order by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num," + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_top_diffusion";
					else return " order by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_top_diffusion";
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
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

	    protected override long CountDataRows()
	    {
	        throw new NotImplementedException();
	    }

	    #endregion



	}
}
