#region Information
// Author: Y. Rkaina && D. Mussuma
// Creation date: 08/08/2008
// Modification date:
#endregion

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.Portofolio.DAL.Exceptions;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBConstantes=TNS.AdExpress.Constantes.DB;

using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Classification;

using TNS.AdExpress.Web.Exceptions;
using CstProject = TNS.AdExpress.Constantes.Project;
using TNS.AdExpress.Constantes.FrameWork.Results;
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
                        case DBClassificationConstantes.Vehicles.names.tvGeneral:
                        case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                        case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                        case DBClassificationConstantes.Vehicles.names.tvAnnounces:
						case DBClassificationConstantes.Vehicles.names.radio:
                        case DBClassificationConstantes.Vehicles.names.radioGeneral:
                        case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                        case DBClassificationConstantes.Vehicles.names.radioMusic:
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
			var res = new Engines.InsertionDetailEngine(_webSession, 
                _vehicleInformation, _module, _idMedia, _beginingDate, _endDate,_adBreak);
			return res.GetData();
		}

		#region Synthesis membres
		/// <summary>
		/// Get synthesis data
		/// </summary>
        /// <param name="synthesisDataType">Synthesis Data Type</param>
		/// <returns></returns>
        public virtual DataSet GetSynthisData(PortofolioSynthesis.dataType synthesisDataType) {
            var res = new Engines.SynthesisEngine(_webSession, 
                _vehicleInformation, _module, _idMedia, _beginingDate, _endDate, synthesisDataType);
			return res.GetData();
		}		

        /// <summary>
        /// récupère les écrans
        /// </summary>
        /// <returns>Ecrans</returns>
        public virtual DataSet GetEcranData() {
            var res = new Engines.SynthesisEngine(_webSession, 
                _vehicleInformation, _module, _idMedia, _beginingDate, _endDate);
            return res.GetEcranData();
        }

        #region TableOfIssue
        /// <summary>
        /// Implements  data access layer for table of issue. 
        /// </summary>
        /// <returns>Data Set with Data Table Result</returns>
        public virtual DataSet TableOfIssue(){
            throw new NotImplementedException("This methods is not implemented");
        }
        #endregion

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
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
				case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
					res = new Engines.StructureEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate,_hourBeginningList,_hourEndList);
					break;
				case DBClassificationConstantes.Vehicles.names.press :
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
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
		public virtual Hashtable GetInvestmentByMedia() {

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
                productsRights = SQLGenerator.GetClassificationCustomerProductRight(_webSession,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
				mediaRights = SQLGenerator.getAnalyseCustomerMediaRight(_webSession,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
				listProductHap = GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                euroFieldNameSumWithAlias = string.Format("sum({0}) as {1}", UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField,
                    UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString());
                insertionFieldNameSumWithAlias = string.Format("sum({0}) as {1}",
                    UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion].DatabaseField,
                    UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion].Id.ToString());
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to build the request for GetInvestmentByMedia() : " + sql, err));
			}

			#region Construction de la requête

            sql += string.Format(" select {0},{1},date_cover_num date1", insertionFieldNameSumWithAlias, euroFieldNameSumWithAlias);
            sql += string.Format("  from {0}  {1}", WebApplicationParameters.GetDataTable(TableIds.dataPress, _webSession.IsSelectRetailerDisplay).Sql,
                WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
			sql += string.Format(" where id_media={0} ", _idMedia);
			if (_beginingDate.Length > 0)
				sql += string.Format(" and date_media_num>={0} ", _beginingDate);
			if (_endDate.Length > 0)
				sql += string.Format("  and date_media_num<={0} ", _endDate);
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
				    foreach (DataRow current in ds.Tables[0].Rows) {
						var value1 = new string[2];
                        value1[0] = current[UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()].ToString();
                        value1[1] = current[UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion].Id.ToString()].ToString();
						htInvestment.Add(current["date1"], value1);
					}
				}
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("error when getting investment by media", err));
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

            string tableName;
            try {
				switch (tableType) {
					case DBConstantes.TableType.Type.dataVehicle4M:
						tableName = SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id,
                            WebConstantes.Module.Type.alert, _webSession.IsSelectRetailerDisplay);
						break;
					case DBConstantes.TableType.Type.dataVehicle:
                        tableName = SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id,
                            WebConstantes.Module.Type.analysis, _webSession.IsSelectRetailerDisplay);
						break;				
					default:
                        throw (new PortofolioDALException("Table type unknown"));
				}
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Error when getting table name", err));
            }

            #region Construction de la requête
            var sql = new StringBuilder(500);

            sql.Append(" select distinct date_media_num ");

            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress) {
                sql.Append(", disponibility_visual, number_page_media ");
                sql.Append(", date_cover_num ");
            }
            else if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine) {
                sql.Append(", disponibility_visual , number_page_media ");
                sql.Append(", date_media_num as date_cover_num");
            }
            sql.Append(", media ");
            sql.Append(" from ");
			sql.Append(tableName);

            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress) {
                sql.AppendFormat(",{0} ", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.applicationMedia).SqlWithPrefix);
                sql.AppendFormat(",{0} ", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).SqlWithPrefix);
            }
            sql.AppendFormat(",{0} ", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).SqlWithPrefix);
            sql.AppendFormat(" where {0}.id_media={1}.id_media" + " ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,
                WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix);
            sql.AppendFormat(" and {0}.id_media={1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _idMedia);
			sql.AppendFormat(" and {0}.id_language={1} ", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix,
                _webSession.DataLanguage);

            // Période			

            if (_beginingDate.Length > 0)
                sql.AppendFormat(" and {0}.date_media_num>={1}",
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _beginingDate);
            if (_endDate.Length > 0 && conditionEndDate)
                sql.AppendFormat(" and {0}.date_media_num<={1}",
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _endDate);

            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress) {

                sql.AppendFormat(" and {0}.date_debut(+) = {1}.date_media_num ",
                    WebApplicationParameters.DataBaseDescription.GetTable(TableIds.applicationMedia).Prefix,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

                sql.AppendFormat(" and {0}.id_project(+) = {1} ", 
                    WebApplicationParameters.DataBaseDescription.GetTable(TableIds.applicationMedia).Prefix, CstProject.ADEXPRESS_ID);
                sql.AppendFormat(" and {0}.id_media(+) = {1}.id_media ", 
                    WebApplicationParameters.DataBaseDescription.GetTable(TableIds.applicationMedia).Prefix,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

                sql.AppendFormat(" and {0}.id_media=al.id_media(+)",
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

                sql.AppendFormat(" and {0}.date_media_num=al.DATE_ALARM(+)",
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

                sql.AppendFormat(" and {0}.id_media(+)={1} ", 
                    WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).Prefix, _idMedia);
                sql.AppendFormat(" and {0}.ID_LANGUAGE_I(+)={1} ",
                    WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).Prefix, _webSession.DataLanguage);
                sql.AppendFormat(" and  {0}.DATE_ALARM(+)>={1} ",
                    WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).Prefix, _beginingDate);
                if (_endDate.Length > 0 && conditionEndDate)
                    sql.AppendFormat(" and  {0}.DATE_ALARM(+)<={1} ", 
                        WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).Prefix, _endDate);

            }
            // Tri			
            sql.AppendFormat(" order by {0}.date_media_num desc",
                WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

            #endregion

            #region Execution de la requête
            try {
                return (_webSession.Source.Fill(sql.ToString()));
            }
            catch (Exception err) {
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
		public bool IsMediaBelongToCategory(Int64 idMedia, string idCategory) {

			StringBuilder t = new StringBuilder(1000);
			DataTable dt = null;

			t.AppendFormat(" select  {0}.id_media,{0}.media,{1}.id_category ",
                WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix,
                WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix);
			t.AppendFormat(" from  {0},{1}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).SqlWithPrefix,
                WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).SqlWithPrefix);
			t.AppendFormat(" " + ",{0} ", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).SqlWithPrefix);
			t.Append(" where ");
			t.AppendFormat(" {0}.id_basic_media ={1}.id_basic_media ", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix,
                WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix);
			t.AppendFormat(" and {0}.id_category = {1}.id_category ", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix,
                WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix);
			t.AppendFormat(" and {0}.id_category in ({1}) ",
                WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix, idCategory);
			t.AppendFormat(" and {0}.id_language = {1}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix, _webSession.DataLanguage);
			t.AppendFormat(" and {0}.id_language = {1}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix,
                _webSession.DataLanguage);
			t.AppendFormat(" and {0}.id_language = {1}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix,
                _webSession.DataLanguage);
			t.AppendFormat(" and {0}.id_media in ( {1})  ",
                WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix, idMedia);
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
                sql= _webSession.PrincipalProductUniverses[0].
                    GetSqlConditions(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,true);

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
        //protected virtual string GetTableData() {
        //    switch (_vehicleInformation.Id) {
        //        case DBClassificationConstantes.Vehicles.names.internationalPress:
        //            return DBConstantes.Tables.ALERT_DATA_PRESS_INTER;
        //        case DBClassificationConstantes.Vehicles.names.press:
        //            return DBConstantes.Tables.ALERT_DATA_PRESS;
        //        case DBClassificationConstantes.Vehicles.names.radio:
        //            return DBConstantes.Tables.ALERT_DATA_RADIO;
        //        case DBClassificationConstantes.Vehicles.names.tv:
        //        case DBClassificationConstantes.Vehicles.names.others:
        //            return DBConstantes.Tables.ALERT_DATA_TV;
        //        case DBClassificationConstantes.Vehicles.names.outdoor:
        //            return DBConstantes.Tables.ALERT_DATA_OUTDOOR;
        //        case DBClassificationConstantes.Vehicles.names.directMarketing:
        //            return DBConstantes.Tables.ALERT_DATA_MARKETING_DIRECT;
        //        default:
        //            throw new PortofolioDALException("GetTableData()-->Vehicle unknown.");
        //    }
		//}
		#endregion

        #region Get Select Data Ecran
        /// <summary>
        /// Get Select Data Ecran
        /// </summary>
        /// <returns>SQL</returns>
		protected virtual string GetSelectDataEcran() {
            var sql = string.Empty;
            switch (_vehicleInformation.Id) {
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.czinternet:
                case DBClassificationConstantes.Vehicles.names.internet:
                   break;
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                    sql += " select  distinct ID_COBRANDING_ADVERTISER";
                    sql += " ,duration_commercial_break as ecran_duration";
                    sql += " , NUMBER_spot_com_break nbre_spot";
                    sql += string.Format(" ,{0} ",
                        UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion].DatabaseField);
                    break;
                   
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                    sql += "select  distinct id_commercial_break ";
                    sql += " ,duration_commercial_break as ecran_duration";
                    sql += " ,NUMBER_MESSAGE_COMMERCIAL_BREA nbre_spot ";
                    sql += string.Format(" ,{0} ",
                         UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion].DatabaseField);
                    break;
                default:
                    throw new PortofolioDALException("getTable(DBClassificationConstantes.Vehicles.value idMedia)-->Vehicle unknown.");
            }
            return sql;
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
                case DBClassificationConstantes.Vehicles.names.press:
                    sql += string.Format("{0} {1}", 
                        WebApplicationParameters.GetDataTable(TableIds.dataPressAlert, _webSession.IsSelectRetailerDisplay).Sql,
                        WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    sql += string.Format(" ,{0}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).SqlWithPrefix);
                    sql += string.Format(" ,{0}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.color).SqlWithPrefix);
                    sql += string.Format(" ,{0}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.format).SqlWithPrefix);
                    return sql;
                case DBClassificationConstantes.Vehicles.names.newspaper:
                    sql += string.Format("{0} {1}",
                        WebApplicationParameters.GetDataTable(TableIds.dataNewspaperAlert, _webSession.IsSelectRetailerDisplay).Sql, 
                        WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    sql += string.Format(" ,{0}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).SqlWithPrefix);
                    sql += string.Format(" ,{0}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.color).SqlWithPrefix);
                    sql += string.Format(" ,{0}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.format).SqlWithPrefix);
                    return sql;
                case DBClassificationConstantes.Vehicles.names.magazine:
                    sql += string.Format("{0} {1}",
                        WebApplicationParameters.GetDataTable(TableIds.dataMagazineAlert, _webSession.IsSelectRetailerDisplay).Sql,
                        WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    sql += string.Format(" ,{0}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).SqlWithPrefix);
                    sql += string.Format(" ,{0}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.color).SqlWithPrefix);
                    sql += string.Format(" ,{0}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.format).SqlWithPrefix);
                    return sql;
                case DBClassificationConstantes.Vehicles.names.radio:
                    sql += string.Format("{0} {1}", WebApplicationParameters.GetDataTable(TableIds.dataRadioAlert, _webSession.IsSelectRetailerDisplay).Sql,
                        WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    sql += " ," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).SqlWithPrefix;
                    return sql;
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    sql += string.Format("{0} {1}",
                        WebApplicationParameters.GetDataTable(TableIds.dataTvAlert, _webSession.IsSelectRetailerDisplay).Sql,
                        WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    sql += string.Format(" ,{0}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).SqlWithPrefix);
                    return sql;
                default:
                    throw new PortofolioDALException(" Vehicle unknown.");
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
			string sql = string.Empty;
			ProductItemsList prList = null; ;
			if (Product.Contains(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID) 
                && (prList = Product.GetItemsList(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID)) != null)
				sql = prList.GetExcludeItemsSql(true, prefix);
			return sql;
		}
		#endregion
    
		#endregion
	}
}
