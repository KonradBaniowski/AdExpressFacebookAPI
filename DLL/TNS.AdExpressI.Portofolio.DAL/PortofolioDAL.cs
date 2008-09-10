#region Information
// Author: Y. Rkaina && D. Mussuma
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
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBConstantes=TNS.AdExpress.Constantes.DB;

using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Classification;

using TNS.AdExpress.Web.Exceptions;
using CustormerConstantes=TNS.AdExpress.Constantes.Customer;
using CstProject = TNS.AdExpress.Constantes.Project;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Exceptions;
using TNS.AdExpress.Domain.Units;

namespace TNS.AdExpressI.Portofolio.DAL {
    /// <summary>
    /// Portofolio Data Access Layer
    /// </summary>
    public abstract class PortofolioDAL:IPortofolioDAL {

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
        /// Vehicle name
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
		/// Screen code
		/// </summary>
		protected string _adBreak;
		/// <summary>
		/// Beginning hour interval list
		/// </summary>
		protected Dictionary<string, double> _hourBeginningList = null;
		/// <summary>
		/// End hour interval list
		/// </summary>
		protected Dictionary<string, double> _hourEndList = null;
		/// <summary>
		/// Ventilation type list for press result
		/// </summary>
		protected List<PortofolioStructure.Ventilation> _ventilationTypeList = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
		/// <param name="vehicleInformation">Vehicle name</param>
        /// <param name="idMedia">Media Id</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
		protected PortofolioDAL(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string beginingDate, string endDate) {
            if(webSession==null) throw (new ArgumentNullException("Customer session is null"));
            if(beginingDate==null || beginingDate.Length==0) throw (new ArgumentException("Begining Date is invalid"));
            if(endDate==null || endDate.Length==0) throw (new ArgumentException("End Date is invalid"));
			if (vehicleInformation == null) throw (new ArgumentNullException("vehicleInformation session is null"));
			_webSession=webSession;
            _beginingDate=beginingDate;
            _endDate=endDate;
			_vehicleInformation = vehicleInformation;
            _idMedia = idMedia;
            try {
                // Module
                _module=ModulesList.GetModule(webSession.CurrentModule);

            }
            catch(System.Exception err) {
                throw (new PortofolioDALException("Impossible to set parameters",err));
            }
        
        }
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="vehicleInformation">Vehicle name</param>
		/// <param name="idMedia">Media Id</param>
		/// <param name="beginingDate">begining Date</param>
		/// <param name="endDate">end Date</param>
		/// <param name="adBreak"></param>		
		protected PortofolioDAL(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string beginingDate, string endDate,string adBreak) :
		this(webSession,vehicleInformation,idMedia,beginingDate,endDate){			
			_adBreak = adBreak;			
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="vehicleInformation">Vehicle name</param>
		/// <param name="idMedia">Media Id</param>
		/// <param name="beginingDate">begining Date</param>
		/// <param name="endDate">end Date</param>
		/// <param name="hour Beginning List">hour Beginning List</param>
		/// <param name="hourEndList">hour EndList</param>		
		protected PortofolioDAL(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string beginingDate, string endDate, Dictionary<string, double> hourBeginningList, Dictionary<string, double> hourEndList)
		: this(webSession, vehicleInformation, idMedia, beginingDate, endDate) {
			_hourBeginningList = hourBeginningList;
			_hourEndList = hourEndList;
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="vehicleInformation">Vehicle name</param>
		/// <param name="idMedia">Media Id</param>
		/// <param name="beginingDate">begining Date</param>
		/// <param name="endDate">end Date</param>
		/// <param name="adBreak"></param>
		/// <param name="ventilationTypeList">ventilation Type List</param>
		protected PortofolioDAL(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string beginingDate, string endDate, List<PortofolioStructure.Ventilation> ventilationTypeList)
			:
		this(webSession, vehicleInformation, idMedia, beginingDate, endDate) {
			_ventilationTypeList = ventilationTypeList;
		}
        #endregion

        #region IPortofolioDAL Membres

		/// <summary>
		/// Get synthesis data
		/// </summary>
		/// <returns></returns>
		public virtual DataSet GetData() {
			Engines.Engine res = null;

			switch (_webSession.CurrentTab) {
				case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
					res = new Engines.PortofolioDetailEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate);
					break;
				case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.CALENDAR:
					res = new Engines.CalendarEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate);
					break;
				case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA :
					switch (_vehicleInformation.Id) {
						case DBClassificationConstantes.Vehicles.names.others :
						case DBClassificationConstantes.Vehicles.names.tv :
						case DBClassificationConstantes.Vehicles.names.radio:
						res = new Engines.MediaDetailEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate);
					break;
					default:
					throw (new PortofolioDALException("Impossible to identified current vehicle "));
					}
					break;
				case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.STRUCTURE :
					res = GetStructData();
					break;
				default:
					throw (new PortofolioDALException("Impossible to identified current tab "));
			}
			
			return res.GetData();
		}

		/// <summary>
		/// Get Inssertion data
		/// </summary>
		/// <returns></returns>
		public virtual DataSet GetInsertionData() {
			Engines.Engine res = new Engines.InsertionDetailEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate,_adBreak);
			return res.GetData();
		}

		#region Synthesis membres
		/// <summary>
		/// Get synthesis data
		/// </summary>
        /// <param name="synthesisDataType">Synthesis Data Type</param>
		/// <returns></returns>
        public virtual DataSet GetSynthisData(PortofolioSynthesis.dataType synthesisDataType) {
            Engines.SynthesisEngine res = new Engines.SynthesisEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate, synthesisDataType);
			return res.GetData();
		}		

        /// <summary>
        /// récupère les écrans
        /// </summary>
        /// <returns>Ecrans</returns>
        public DataSet GetEcranData() {
            Engines.SynthesisEngine res = new Engines.SynthesisEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate);
            return res.GetEcranData();

        }
        #endregion

        #region Get Struct Data
        /// <summary>
        /// Get structure data 
        /// </summary>
		/// <remarks>Used for tv or radio</remarks>
		/// <param name="hourBegin">Hour Begin</param>
		/// <param name="hourEnd">Hour End</param>
        /// <returns>DataSet</returns>
		protected virtual Engines.StructureEngine GetStructData() {
			Engines.StructureEngine res = null;

			switch (_vehicleInformation.Id) {
				case DBClassificationConstantes.Vehicles.names.others :
				case DBClassificationConstantes.Vehicles.names.tv :
				case DBClassificationConstantes.Vehicles.names.radio:
					res = new Engines.StructureEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate,_hourBeginningList,_hourEndList);
					break;
				case DBClassificationConstantes.Vehicles.names.press :
				case DBClassificationConstantes.Vehicles.names.internationalPress :					
					res = new Engines.StructureEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate, _ventilationTypeList);
					break;
				default:
					throw (new PortofolioDALException("Impossible to identified current vehicle "));
			}
			return res;
		}
		#endregion


		#region Investment By Media
		/// <summary>
		/// Get Investment By Media
		/// </summary>
		/// <returns>Data</returns>
		public Hashtable GetInvestmentByMedia() {

			#region Variables
			string sql = "";
			Hashtable htInvestment = new Hashtable();
			string product = null;
			string productsRights = null;
			string mediaRights = null;
			string listProductHap = null;
            string euroFieldNameSumWithAlias = null;
            string insertionFieldNameSumWithAlias = null;
			#endregion

			try {
				product = GetProductData();
				productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
				mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
				//listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
				listProductHap = GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                euroFieldNameSumWithAlias = "sum(" + UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.euro].DatabaseField + ") as " + UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.euro].Id.ToString();
                insertionFieldNameSumWithAlias = "sum(" + UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion].DatabaseField + ") as " + UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion].Id.ToString();
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to build the request for GetInvestmentByMedia() : " + sql, err));
			}

			#region Construction de la requête
            sql += " select " + insertionFieldNameSumWithAlias + "," + euroFieldNameSumWithAlias + ",date_cover_num date1";
			sql += "  from " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.ALERT_DATA_PRESS + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
			sql += " where id_media=" + _idMedia + " ";
			if (_beginingDate.Length > 0)
				sql += " and date_media_num>=" + _beginingDate + " ";
			if (_endDate.Length > 0)
				sql += "  and date_media_num<=" + _endDate + " ";
			sql += product;
			sql += productsRights;
			sql += mediaRights;
			sql += listProductHap;
			sql += " group by date_cover_num ";
			#endregion

			#region Execution de la requête
			try {
				DataSet ds = _webSession.Source.Fill(sql);
				if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0) {
					string[] value1 = null;
					foreach (DataRow current in ds.Tables[0].Rows) {
						value1 = new string[2];
                        value1[0] = current[UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.euro].Id.ToString()].ToString();
                        value1[1] = current[UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion].Id.ToString()].ToString();
						htInvestment.Add(current["date1"], value1);
					}
				}
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("error whene getting investment by media", err));
			}
			#endregion

			return (htInvestment);
		}
		#endregion

        #region Structure

        #endregion

        #region Get dates list
        /// <summary>
        /// Get dates list
        /// </summary>
        /// <param name="conditionEndDate">Add condition end date</param>
        /// <returns>DataSet</returns>
        public virtual DataSet GetListDate(bool conditionEndDate,DBConstantes.TableType.Type tableType) {

            string tableName = "";
            try {
				switch (tableType) {
					case DBConstantes.TableType.Type.dataVehicle4M:
						tableName = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert);
						break;
					case DBConstantes.TableType.Type.dataVehicle:
						tableName = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.analysis);
						break;				
					default:
						throw (new CompetitorDataAccessException("Table type unknown"));
				}
                //tableName = GetAlertTable();
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Error when getting table name", err));
            }

            #region Construction de la requête
            StringBuilder sql = new StringBuilder(500);

            sql.Append("select distinct date_media_num ");

            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress) {
                sql.Append(", disponibility_visual ");
                sql.Append(", number_page_media ");
                sql.Append(", date_cover_num ");
            }
            sql.Append(", media ");
            sql.Append(" from ");
            //sql.Append(WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label + "." + tableName + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
			sql.Append(tableName);

            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress) {
                sql.Append("," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.applicationMedia).SqlWithPrefix + " ");
                sql.Append("," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).SqlWithPrefix + " ");
            }
            sql.Append("," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).SqlWithPrefix + " ");
            sql.Append(" where " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media" + " ");
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media=" + _idMedia + " ");
			sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media=" + _webSession.DataLanguage + " ");
            // Période			

            if (_beginingDate.Length > 0)
                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num>=" + _beginingDate);
            if (_endDate.Length > 0 && conditionEndDate)
                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num<=" + _endDate);

            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress) {

                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.applicationMedia).Prefix + ".date_debut(+) = " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num ");

                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.applicationMedia).Prefix + ".id_project(+) = " + CstProject.ADEXPRESS_ID + " ");
                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.applicationMedia).Prefix + ".id_media(+) = " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media ");

                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media=al.id_media(+)");

                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num=al.DATE_ALARM(+)");

                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).Prefix + ".id_media(+)=" + _idMedia + " ");
                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).Prefix + ".ID_LANGUAGE_I(+)=" + _webSession.DataLanguage + " ");
                sql.Append(" and  " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).Prefix + ".DATE_ALARM(+)>=" + _beginingDate + " ");
                if (_endDate.Length > 0 && conditionEndDate)
                    sql.Append(" and  " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).Prefix + ".DATE_ALARM(+)<=" + _endDate + " ");

            }
            // Tri			
            sql.Append(" order by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num desc");

            #endregion

            #region Execution de la requête
            try {
                return (_webSession.Source.Fill(sql.ToString()));
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Erreur lors de la sélection de la table", err));
            }
            #endregion

        }
        #endregion        
        
		#region Is Media belong To Category
		/// <summary>
		/// Cheks if media belong to Category
		/// </summary>
		/// <param name="idMedia">Id media</param>
		/// <param name="idCategory">Id Category</param>
		/// <returns>True if media belong to Category</returns>
		public bool IsMediaBelongToCategory(Int64 idMedia, Int64 idCategory) {

			StringBuilder t = new StringBuilder(1000);
			DataTable dt = null;

			t.Append(" select  " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".media," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_category ");
			t.Append(" from  " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).SqlWithPrefix + "," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).SqlWithPrefix);
			t.Append(" " + "," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).SqlWithPrefix + " ");
			t.Append(" where ");
			t.Append(" " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_basic_media =" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix + ".id_basic_media ");
			t.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_category = " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix + ".id_category ");
			t.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_category = "+idCategory);
			t.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_language = " + _webSession.DataLanguage);
			t.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_language = " + _webSession.DataLanguage);
			t.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix + ".id_language = " + _webSession.DataLanguage);
			t.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media = " + idMedia);
			t.Append("  order by media ");

			#region Execution de la requête
			try {
				dt = _webSession.Source.Fill(t.ToString()).Tables[0];

				if (dt != null && !dt.Equals(System.DBNull.Value) && dt.Rows.Count > 0) return true;
				else return false;
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to know if the media belong to Category  : " + t, err));
			}
			#endregion

		}
		#endregion

        #endregion

        #region Methods

        #region Get Product Data
        /// <summary>
        /// Récupère la liste produit de référence
        /// </summary>
        /// <returns>la liste produit de référence</returns>
        protected virtual string GetProductData() {
            string sql="";
            if(_webSession.PrincipalProductUniverses != null && _webSession.PrincipalProductUniverses.Count > 0)
                sql= _webSession.PrincipalProductUniverses[0].GetSqlConditions(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,true);

            return sql;
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

        #region Get Select Data Ecran
        /// <summary>
        /// Get Select Data Ecran
        /// </summary>
        /// <returns>SQL</returns>
		protected virtual string GetSelectDataEcran() {
            string sql = "";
            switch (_vehicleInformation.Id) {
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.internet:
                    return "";
                case DBClassificationConstantes.Vehicles.names.radio:
                    sql += " select  distinct ID_COBRANDING_ADVERTISER";
                    sql += " ,duration_commercial_break as ecran_duration";
                    sql += " , NUMBER_spot_com_break nbre_spot";
                    sql += " ," + UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion].DatabaseField + " ";

                    return sql;
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    sql += "select  distinct id_commercial_break ";
                    sql += " ,duration_commercial_break as ecran_duration";
                    sql += " ,NUMBER_MESSAGE_COMMERCIAL_BREA nbre_spot ";
                    sql += " ," + UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion].DatabaseField + " ";
                    return sql;
                default:
                    throw new PortofolioDALException("getTable(DBClassificationConstantes.Vehicles.value idMedia)-->Vehicle unknown.");
            }
        }
        #endregion

        #region Get Table Data New Product
        /// <summary>
        /// Get Table Data New Product
        /// </summary>
        /// <returns>SQL</returns>
		protected virtual string getTableDataNewProduct() {
            string sql = "";
            switch (_vehicleInformation.Id) {
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    sql += WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql + DBConstantes.Tables.ALERT_DATA_PRESS_INTER + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
                    sql += " ," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).SqlWithPrefix;
                    sql += " ," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.color).SqlWithPrefix;
                    sql += " ," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.format).SqlWithPrefix;
                    return sql;
                case DBClassificationConstantes.Vehicles.names.press:
                    sql += WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql + DBConstantes.Tables.ALERT_DATA_PRESS + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
                    sql += " ," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).SqlWithPrefix;
                    sql += " ," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.color).SqlWithPrefix;
                    sql += " ," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.format).SqlWithPrefix;
                    return sql;
                case DBClassificationConstantes.Vehicles.names.radio:
                    sql += WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql + DBConstantes.Tables.ALERT_DATA_RADIO + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
                    sql += " ," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).SqlWithPrefix;
                    return sql;
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    sql += WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql + DBConstantes.Tables.ALERT_DATA_TV + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
                    sql += " ," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).SqlWithPrefix;
                    return sql;
                default:
                    throw new PortofolioDALException("getTable(DBClassificationConstantes.Vehicles.value idMedia)-->Vehicle unknown.");
            }
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
			ProductItemsList prList = Product.GetItemsList(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID);
			if (prList != null)
				sql = prList.GetExcludeItemsSql(true, prefix);
			return sql;
		}
		#endregion
    
		#endregion
	}
}
