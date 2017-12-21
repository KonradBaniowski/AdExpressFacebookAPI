#region Information
/*
 * Author : G Ragneau
 * Created on : 30/07/2008
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstRight = TNS.AdExpress.Constantes.Customer.Right;
using CstComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion;
using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using CstWeb = TNS.AdExpress.Constantes.Web;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Exceptions;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpressI.ProductClassIndicators.DAL.Exceptions;
using TNS.AdExpress.Domain.Exceptions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Classification;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Date.DAL;
using System.Reflection;


namespace TNS.AdExpressI.ProductClassIndicators.DAL.DALEngines
{
    /// <summary>
    /// Define default Database Access
    /// </summary>
    public class DALEngine
    {

        #region Constants

        #region Tables
        protected Table _dataRadio = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapRadio);
        protected Table _dataTV = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapTv);
        protected Table _dataPluri = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapPluri);
        protected Table _dataPress = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapPress);
        protected Table _dataOutdoor = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapOutDoor);
        protected Table _dataInternet = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapInternet);
        protected Table _dataCinema = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapCinema);
        protected Table _dataTactic = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapTactic);
        protected Table _recapAdvertiser = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapAdvertiser);
        protected Table _recapProduct = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapProduct);
        protected Table _recapSector = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapSector);
        protected Table _recapSegment = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapSegment);
        protected Table _recapVehicle = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapVehicle);
        protected Table _recapCategory = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapCategory);
        protected Table _recapMedia = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapMedia);
        /*
        protected Table _recapGroup = WebApplicationParameters.GetDataTable(TableIds.recapGroup);
        protected Table _recapBrand = WebApplicationParameters.GetDataTable(TableIds.recapBrand);
        protected Table _recapProduct = WebApplicationParameters.GetDataTable(TableIds.recapProduct);
        protected Table _recapAdvertiser = WebApplicationParameters.GetDataTable(TableIds.recapAdvertiser);
        protected Table _dataRadioSegment = WebApplicationParameters.GetDataTable(TableIds.recapRadioSegment);
        protected Table _dataTVSegment = WebApplicationParameters.GetDataTable(TableIds.recapTvSegment);
        protected Table _dataPluriSegment = WebApplicationParameters.GetDataTable(TableIds.recapPluriSegment);
        protected Table _dataPressSegment = WebApplicationParameters.GetDataTable(TableIds.recapPressSegment);
        protected Table _dataOutdoorSegment = WebApplicationParameters.GetDataTable(TableIds.recapOutDoorSegment);
        protected Table _dataInternetSegment = WebApplicationParameters.GetDataTable(TableIds.recapInternetSegment);
        protected Table _dataCinemaSegment = WebApplicationParameters.GetDataTable(TableIds.recapCinemaSegment);
        protected Table _dataTacticSegment = WebApplicationParameters.GetDataTable(TableIds.recapTacticSegment);*/
        #endregion

        #endregion

        #region Attributes
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// Vehicle to study
        /// </summary>
        protected CstDBClassif.Vehicles.names _vehicle;

        protected DALUtilities _utilities = null;

        #region Rules Engine attributes
        /// <summary>
        /// Specify if the table contains advertisers as references or competitors
        /// </summary>
        protected int _isPersonalized = 0;
        protected int _iYearId = 0;
        protected string _strYearId = string.Empty;
        protected int _iYearN1Id = 0;
        protected string _strYearN1Id = string.Empty;
        protected DateTime _periodBegin;
        protected DateTime _periodEnd;
        #endregion

        #endregion

        #region Accessors
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession Session
        {
            get { return _session; }
            set { _session = value; }
        }
        /// <summary>
        /// Period Begin
        /// </summary>
        public DateTime PeriodBegin
        {
            get { return _periodBegin; }          
        }
        /// </summary>
        /// Period end
        /// </summary>
        public DateTime PeriodEnd
        {
            get { return _periodEnd; }        
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public DALEngine(WebSession session, DALUtilities dalUtilities)
        {
            _session = session;
            _utilities = dalUtilities;
            _vehicle = VehiclesInformation.DatabaseIdToEnum(((LevelInformation)_session.CurrentUniversMedia.FirstNode.Tag).ID);

            #region Tables
            Table _dataRadio = WebApplicationParameters.GetDataTable(TableIds.recapRadio, _session.IsSelectRetailerDisplay);
            Table _dataTV = WebApplicationParameters.GetDataTable(TableIds.recapTv, _session.IsSelectRetailerDisplay);
            Table _dataPluri = WebApplicationParameters.GetDataTable(TableIds.recapPluri, _session.IsSelectRetailerDisplay);
            Table _dataPress = WebApplicationParameters.GetDataTable(TableIds.recapPress, _session.IsSelectRetailerDisplay);
            Table _dataOutdoor = WebApplicationParameters.GetDataTable(TableIds.recapOutDoor, _session.IsSelectRetailerDisplay);
            Table _dataInternet = WebApplicationParameters.GetDataTable(TableIds.recapInternet, _session.IsSelectRetailerDisplay);
            Table _dataCinema = WebApplicationParameters.GetDataTable(TableIds.recapCinema, _session.IsSelectRetailerDisplay);
            Table _dataTactic = WebApplicationParameters.GetDataTable(TableIds.recapTactic, _session.IsSelectRetailerDisplay);
            Table _recapAdvertiser = WebApplicationParameters.GetDataTable(TableIds.recapAdvertiser, _session.IsSelectRetailerDisplay);
            Table _recapProduct = WebApplicationParameters.GetDataTable(TableIds.recapProduct, _session.IsSelectRetailerDisplay);
            Table _recapSector = WebApplicationParameters.GetDataTable(TableIds.recapSector, _session.IsSelectRetailerDisplay);
            Table _recapSegment = WebApplicationParameters.GetDataTable(TableIds.recapSegment, _session.IsSelectRetailerDisplay);
            Table _recapVehicle = WebApplicationParameters.GetDataTable(TableIds.recapVehicle, _session.IsSelectRetailerDisplay);
            Table _recapCategory = WebApplicationParameters.GetDataTable(TableIds.recapCategory, _session.IsSelectRetailerDisplay);
            Table _recapMedia = WebApplicationParameters.GetDataTable(TableIds.recapMedia, _session.IsSelectRetailerDisplay);
            #endregion

            #region Dates
            //Get last available month depending on data delivering frequency
            CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
            object[] param = new object[1];
            param[0] = _session;
            IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                , AppDomain.CurrentDomain.BaseDirectory, cl.AssemblyName), cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            string absolutEndPeriod = dateDAL.CheckPeriodValidity(_session, _session.PeriodEndDate);

            if (int.Parse(absolutEndPeriod) < int.Parse(_session.PeriodBeginningDate))
                throw new NoDataException();

            _periodBegin = FctUtilities.Dates.GetPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType);
            _periodEnd = FctUtilities.Dates.GetPeriodEndDate(absolutEndPeriod, _session.PeriodType);

            FctUtilities.Dates.GetYearSelected(_session, ref _strYearId, ref _iYearId, _periodBegin);
            //_iYearN1Id = (_iYearId == 1) ? 2 : 1;
            _iYearN1Id = (_iYearId == (WebApplicationParameters.DataNumberOfYear - 1)) ? _iYearId : _iYearId + 1;
            if (_iYearN1Id > 0)
                _strYearN1Id = _iYearN1Id.ToString();
            #endregion

        }
        #endregion

        #region GetVehicleTableName
        /// <summary>
        /// Get data table to use depending on selected media
        /// </summary>
        /// <param name="productDetail">True if product (advertiser, brand) level is required </param>
        /// <exception cref="TNS.AdExpressI.ProductClassIndicators.DALEngines.Exceptions.ProductClassIndicatortsDALException">
        /// Thrown if vehicle is not known
        /// </exception>
        /// <returns>Data table object</returns>
        protected virtual Table GetDataTable(bool productDetail)
        {

            switch (_vehicle)
            {
                case CstDBClassif.Vehicles.names.cinema:
                    return (productDetail) ? WebApplicationParameters.GetDataTable(TableIds.recapCinema, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapCinemaSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.adnettrack:
                case CstDBClassif.Vehicles.names.czinternet:
                case CstDBClassif.Vehicles.names.internet:
                    return (productDetail) ? WebApplicationParameters.GetDataTable(TableIds.recapInternet, _session.IsSelectRetailerDisplay)
                        : WebApplicationParameters.GetDataTable(TableIds.recapInternetSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.outdoor:
                case CstDBClassif.Vehicles.names.dooh:
                    return (productDetail) ? WebApplicationParameters.GetDataTable(TableIds.recapOutDoor, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapOutDoorSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.indoor:
                    return (productDetail) ? WebApplicationParameters.GetDataTable(TableIds.recapInDoor, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapInDoorSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.instore:
                    return (productDetail) ? WebApplicationParameters.GetDataTable(TableIds.recapInStore, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapInStoreSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.radio:
                    return (productDetail) ? WebApplicationParameters.GetDataTable(TableIds.recapRadio, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapRadioSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.tv:
                    return (productDetail) ? WebApplicationParameters.GetDataTable(TableIds.recapTv, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapTvSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.press:
                    return (productDetail) ? WebApplicationParameters.GetDataTable(TableIds.recapPress, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapPressSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.newspaper:
                    return (productDetail) ? WebApplicationParameters.GetDataTable(TableIds.recapNewspaper, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapNewspaperSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.magazine:
                    return (productDetail) ? WebApplicationParameters.GetDataTable(TableIds.recapMagazine, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapMagazineSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.plurimedia:
                case CstDBClassif.Vehicles.names.plurimediaWithSearch:
                case CstDBClassif.Vehicles.names.plurimediaOffline:
                case CstDBClassif.Vehicles.names.plurimediaOnline:
                case CstDBClassif.Vehicles.names.PlurimediaWithoutMms:
                    return (productDetail) ? WebApplicationParameters.GetDataTable(TableIds.recapPluri, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapPluriSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.mediasTactics:
                    return (productDetail) ? WebApplicationParameters.GetDataTable(TableIds.recapTactic, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapTacticSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.mobileTelephony:
                    return (productDetail) ? WebApplicationParameters.GetDataTable(TableIds.recapMobileTel, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapMobileTelSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.emailing:
                    return (productDetail) ? WebApplicationParameters.GetDataTable(TableIds.recapEmailing, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapEmailingSegment, _session.IsSelectRetailerDisplay);
				case CstDBClassif.Vehicles.names.directMarketing:
					return (productDetail) ? WebApplicationParameters.GetDataTable(TableIds.recapDirectMarketing, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapDirectMarketingSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.mms:
                    return (productDetail) ? WebApplicationParameters.GetDataTable(TableIds.recapMms, _session.IsSelectRetailerDisplay)
                        : WebApplicationParameters.GetDataTable(TableIds.recapMmsSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.search:
                    return (productDetail) ? WebApplicationParameters.GetDataTable(TableIds.recapSearch, _session.IsSelectRetailerDisplay)
                        : WebApplicationParameters.GetDataTable(TableIds.recapSearchSegment, _session.IsSelectRetailerDisplay);

                case CstDBClassif.Vehicles.names.social:
                    return (productDetail) ? WebApplicationParameters.GetDataTable(TableIds.recapSocial, _session.IsSelectRetailerDisplay)
                        : WebApplicationParameters.GetDataTable(TableIds.recapSocialSegment, _session.IsSelectRetailerDisplay);
                default:
                    throw new ProductClassIndicatorsDALException(string.Format("Vehicle \"{0}\" is unknown. Unable to find a matching table.", _vehicle.GetHashCode()));
            }
        }
        #endregion

        #region Get Expenditure Fields
        /// <summary>
        /// Get expenditure clause
        /// </summary>
        /// <returns>Expenditure clause</returns>
        protected virtual string GetExpenditureClause()
        {
            return GetExpenditureClause(false);
        }

        /// <summary>
        /// Get expenditure clause
        /// </summary>
        /// <param name="onlyComparativeMonthString">True if previous year monthes only</param>
        /// <returns>Expenditure clause</returns>
        protected virtual string GetExpenditureClause(bool onlyComparativeMonthString)
        {

            StringBuilder sql = new StringBuilder();
            StringBuilder sqlComparative = new StringBuilder();

            #region Build clauses
            int StartMonth = _periodBegin.Month;
            int EndMonth = _periodEnd.Month;

            if (!_periodEnd.Equals(null) && !_periodBegin.Equals(null))
            {
                for (int i = StartMonth; i <= EndMonth; i++)
                {
                    if (!onlyComparativeMonthString)
                    {
                        if (i == EndMonth && StartMonth != EndMonth)
                        {
                            sql.AppendFormat("exp_euro_N{0}_{1}) as total_N", _strYearId, i);
                        }
                        else if (i == StartMonth && StartMonth != EndMonth)
                        {
                            sql.AppendFormat("sum(exp_euro_N{0}_{1} + ", _strYearId, i);
                        }
                        else if (StartMonth == EndMonth)
                        {
                            sql.AppendFormat("sum(exp_euro_N{0}_{1}) total_N ", _strYearId, i);
                        }
                        else
                        {
                            sql.AppendFormat("exp_euro_N{0}_{1} + ", _strYearId, i);
                        }
                    }

                    //Get period N-1 for comparative study
                    if (_session.ComparativeStudy && (_iYearId <=(WebApplicationParameters.DataNumberOfYear-2)))
                    {
                        if (i == EndMonth && StartMonth != EndMonth)
                        {
                            sqlComparative.AppendFormat("exp_euro_N{0}_{1}) as total_N1", _strYearN1Id, i);
                        }
                        else if (i == StartMonth && StartMonth != EndMonth)
                        {
                            sqlComparative.AppendFormat("sum(exp_euro_N{0}_{1} + ", _strYearN1Id, i);
                        }
                        else if (StartMonth == EndMonth)
                        {
                            sqlComparative.AppendFormat("sum(exp_euro_N{0}_{1}) as total_N1 ", _strYearN1Id, i);
                        }
                        else
                        {
                            sqlComparative.AppendFormat("exp_euro_N{0}_{1} + ", _strYearN1Id, i);
                        }
                    }
                }


                if (_session.ComparativeStudy)
                {
                    if (onlyComparativeMonthString) return sqlComparative.ToString();
                    else sql.AppendFormat(", {0}", sqlComparative); ;
                }

            }
            #endregion

            return sql.ToString();


        }
        
        /// <summary>
        /// Get expenditure clause about one year without any alias
        /// </summary>
        /// <param name="typeYear">Year to build</param>
        /// <returns>Expenditure clause</returns>
        protected virtual string GetExpenditureClause(CstResult.PalmaresRecap.typeYearSelected typeYear)
        {
            StringBuilder sql = new StringBuilder();


            #region Expenditure clause building
            int StartMonth = _periodBegin.Month;
            int EndMonth = _periodEnd.Month;

            string y = (typeYear != CstResult.PalmaresRecap.typeYearSelected.currentYear) ? _strYearN1Id : _strYearId;

            if (!_periodEnd.Equals(null) && !_periodBegin.Equals(null))
            {
                for (int i = StartMonth; i <= EndMonth; i++)
                {

                    if (i == EndMonth && StartMonth != EndMonth)
                    {
                        sql.AppendFormat("exp_euro_N{0}_{1} )", y, i);
                    }
                    else if (i == StartMonth && StartMonth != EndMonth)
                    {
                        sql.AppendFormat("sum(exp_euro_N{0}_{1} + ", y, i);
                    }
                    else if (StartMonth == EndMonth)
                    {
                        sql.AppendFormat("sum(exp_euro_N{0}_{1}) ", y, i);
                    }
                    else
                    {
                        sql.AppendFormat("exp_euro_N{0}_{1} + ", y, i);
                    }

                }
            }
            #endregion

            return sql.ToString(); ;
			
        }
        /// <summary>
        /// Get expenditure clause about the last studid month without any alias
        /// </summary>
        /// <param name="typeYear">Year to build</param>
        /// <returns>Expenditure clause</returns>
        protected virtual string GetMonthExpenditureClause(CstResult.PalmaresRecap.typeYearSelected typeYear)
        {
            StringBuilder sql = new StringBuilder();

            #region Current mont investments
            int EndMonth = _periodEnd.Month;

            if (!_periodEnd.Equals(null))
            {
                if (typeYear == CstResult.PalmaresRecap.typeYearSelected.currentYear)
                {
                    sql.AppendFormat("sum(exp_euro_N{0}_{1})", _strYearId, EndMonth);
                }
                else
                {
                    sql.AppendFormat("sum(exp_euro_N{0}_{1})", _strYearN1Id, EndMonth);
                }
            }
            #endregion

            return sql.ToString();

        }
        #endregion

        #region Get Market or Sector Total
        /// <summary>
        /// Get Total on the selected period or on the last studid month
        /// </summary>
        /// <param name="typeYear">Current year or previous one?</param>
        /// <param name="expenditureClause">Expenditure clause (sum of monthes or simple month)</param>
        /// <returns>Market orSector total</returns>
        protected double GetTotal(CstComparaisonCriterion totalType, CstResult.PalmaresRecap.typeYearSelected typeYear, string expenditureClause)
        {

            StringBuilder sql = new StringBuilder(5000);
            //Table dataTable = this.GetDataTable(false);
			Table dataTable = this.GetDataTable(true);
            double total = 0;

            #region Query Building
            sql.Append(" select sum(total) from (");
            sql.AppendFormat(" select {0}.id_sector, {1}.sector,", dataTable.Prefix, _recapSector.Prefix);
            sql.AppendFormat(" {0} as total", expenditureClause);
            sql.AppendFormat(" from {0}, ", dataTable.SqlWithPrefix);
            sql.AppendFormat(" {0}", _recapSector.SqlWithPrefix);
            sql.AppendFormat(" where {0}.id_sector={1}.id_sector", dataTable.Prefix, _recapSector.Prefix);
            sql.AppendFormat(" and {0}.id_language={1}", _recapSector.Prefix, _session.DataLanguage);

            if (totalType == CstComparaisonCriterion.sectorTotal)
            {
                sql.AppendFormat(" and {0}.id_sector in ( ", dataTable.Prefix);
                sql.Append(" select distinct id_sector ");
                sql.AppendFormat(" from {0} where", dataTable.SqlWithPrefix );
                // Product selection
                if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                    sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(dataTable.Prefix, false, TNS.Classification.Universe.AccessType.includes));

                sql.Append(" ) ");
            }
            else if (totalType == CstComparaisonCriterion.universTotal)
            {
                sql.Append(" ");
                // Product selection
                if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                    sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(dataTable.Prefix, true));
                // Product rights
                TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(_session.CurrentModule);
                string productRightsBranches = (module != null) ? module.ProductRightBranches : ""; 
                sql.Append(FctUtilities.SQLGenerator.GetClassificationCustomerProductRight(_session, dataTable.Prefix, true,productRightsBranches));
            }

            #region Media selection
            sql.AppendFormat(" and {0} ", _utilities.GetMediaSelection(dataTable.Prefix));
            #endregion

            sql.AppendFormat(" group by {0}.id_sector, {1}.sector ", dataTable.Prefix, _recapSector.Prefix);
            sql.Append(" ) ");

            #endregion

            #region Execute Query
            try
            {
                IDataSource source = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis, WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].NlsSort);
                DataSet ds = source.Fill(sql.ToString());
                if (ds.Tables[0].Rows[0][0] != System.DBNull.Value)
                {
                    total = Convert.ToDouble(ds.Tables[0].Rows[0][0]);
                }
                else
                    return 0;
            }
            catch (System.Exception err)
            {
                throw new ProductClassIndicatorsDALException("Impossible to sum the Total", err);
            }
            #endregion

            return (total);
        }
        /// <summary>
        /// Get Market or Sector Total on the selected period
        /// </summary>
        /// <param name="typeYear">Current year or previous one?</param>
        /// <returns>Market orSector total</returns>
        public double GetTotal(CstResult.PalmaresRecap.typeYearSelected typeYear){
            return GetTotal(this._session.ComparaisonCriterion, typeYear, GetExpenditureClause(typeYear));
        }
        /// <summary>
        /// Get Market or Sector Total on the last month of the studied period
        /// </summary>
        /// <param name="typeYear">Current year or previous one?</param>
        /// <returns>Market orSector total</returns>
        public double GetMonthTotal(CstResult.PalmaresRecap.typeYearSelected typeYear)
        {
            return GetTotal(this._session.ComparaisonCriterion,typeYear, GetMonthExpenditureClause(typeYear));
        }
        /// <summary>
        /// Get Total on the selected period
        /// </summary>
        /// <param name="totalType">Type of total</param>
        /// <param name="typeYear">Current year or previous one?</param>
        /// <returns>Market orSector total</returns>
        public double GetTotal(CstComparaisonCriterion totalType, CstResult.PalmaresRecap.typeYearSelected typeYear){
            return GetTotal(totalType, typeYear, GetExpenditureClause(typeYear));
        }
        /// <summary>
        /// Get Market or Sector Total on the last month of the studied period
        /// </summary>
        /// <param name="totalType">Type of total</param>
        /// <param name="typeYear">Current year or previous one?</param>
        /// <returns>Market orSector total</returns>
        public double GetMonthTotal(CstComparaisonCriterion totalType, CstResult.PalmaresRecap.typeYearSelected typeYear)
        {
            return GetTotal(totalType, typeYear, GetMonthExpenditureClause(typeYear));
        }
        #endregion

		#region Get Products Personnalisation type
		/// <summary>
		/// Get Products Personnalisation type
		/// </summary>
		/// <param name="idProductList">Id product list</param>
		/// <returns>DataTable[id_product,inref,incomp,inneutral]</returns>
		public virtual DataSet GetProductsPersonnalisationType(String idProductList, string strYearId) {
			#region Variables
			DataSet ds = null;
			Dictionary<long, List<long>> res = null;
			StringBuilder sql = new StringBuilder();
			string expenditure = "";
			#region dates (mensuels) des investissements
			int iStart = _periodBegin.Month;
			int iEnd = _periodEnd.Month;
			#endregion


			if (!_periodEnd.Equals(null) && !_periodBegin.Equals(null)) {
				
				//N or N-1
				for (int i = iStart; i <= iEnd; i++) {
					if (i == iEnd && iStart != iEnd) {
						sql.AppendFormat("exp_euro_N{0}_{1}) as total_N ", strYearId, i);
					}
					else if (i == iStart && iStart != iEnd) {
						sql.AppendFormat("sum(exp_euro_N{0}_{1} + ", strYearId, i);
					}
					else if (iStart == iEnd) {
						sql.AppendFormat("sum(exp_euro_N{0}_{1}) as total_N", strYearId, i);
					}
					else {
						sql.AppendFormat("exp_euro_N{0}_{1} + ", strYearId, i);
					}

				}				
				expenditure = sql.ToString();
			}
			#endregion

			#region Request building
			sql = new StringBuilder();
			Table dataTable = this.GetDataTable(true);
			
			sql.Append(" select distinct  id_product ");
			
			#region Notion de personnalisation des annonceurs?
			string annonceurPerso = "id_advertiser";
			if (annonceurPerso != null && annonceurPerso.Length > 0) {
				NomenclatureElementsGroup refElts = null;
				string refString = "";
				string[] refLst = null;
				if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(0)) {
					refElts = _session.SecondaryProductUniverses[0].GetGroup(0);
					if (refElts != null && refElts.Count() > 0 && refElts.Contains(TNSClassificationLevels.ADVERTISER)) {
						refString = refElts.GetAsString(TNSClassificationLevels.ADVERTISER);
						refLst = refString.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
						if (refLst != null && refLst.Length > 0) {
							sql.AppendFormat(", sum(decode({0}", annonceurPerso);
							foreach (string s in refLst) {
								sql.AppendFormat(",{0},1", s);
							}
							sql.Append(", 0)) as inref ");
						}
					}
				}
				if (refLst == null || refLst.Length == 0) {
					sql.Append(", 0 as inref ");
				}
				NomenclatureElementsGroup compElts = null;
				string compString = "";
				string[] compLst = null;
				if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(1)) {
					compElts = _session.SecondaryProductUniverses[1].GetGroup(0);
					if (compElts != null && compElts.Count() > 0 && compElts.Contains(TNSClassificationLevels.ADVERTISER)) {
						compString = compElts.GetAsString(TNSClassificationLevels.ADVERTISER);
						compLst = compString.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
						if (compLst != null && compLst.Length > 0) {
							sql.AppendFormat(", sum(decode({0}", annonceurPerso);
							foreach (string s in compLst) {
								sql.AppendFormat(",{0},1", s);
							}
							sql.Append(", 0)) as incomp ");
						}
					}
				}
				if (compLst == null || compLst.Length == 0) {
					sql.Append(", 0 as incomp ");
				}
				if (compString.Length > 0) {
					if (refString.Length > 0) {
						refString += ",";
					}
					refString += compString;
				}
				if (refString.Length > 0) {
					sql.AppendFormat(", sum(case when {0} not in ({1}) then 1 else 0 end) as inneutral ", annonceurPerso, refString);
				}
				else {
					sql.AppendFormat(", count(distinct {0}) as inneutral ", annonceurPerso);
				}
			}
			#endregion

			sql.Append("  from ( ");

			sql.AppendFormat(" select distinct  {0}.id_product, {0}.id_advertiser ", dataTable.Prefix);
			if (expenditure != null && expenditure.Length > 0) {
				sql.AppendFormat(" ,{0} ", expenditure);

			}
			sql.Append(" from ");
			sql.AppendFormat(" {0} ", dataTable.SqlWithPrefix);

			sql.Append(" where  0=0 ");

			//Media Selection
			sql.AppendFormat(" and {0}", _utilities.GetMediaSelection(dataTable.Prefix));

			if (!string.IsNullOrEmpty(idProductList)) {
				sql.Append(" and " + FctUtilities.SQLGenerator.GetInClauseMagicMethod(dataTable.Prefix + ".id_product", idProductList, true));
			}

			#region Product classification
			// Product selection
			if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
				sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(dataTable.Prefix, true));
			// Product rights
            TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(_session.CurrentModule);
            string productRightsBranches = (module != null) ? module.ProductRightBranches : "";
            sql.Append(FctUtilities.SQLGenerator.GetClassificationCustomerProductRight(_session, dataTable.Prefix,  true, productRightsBranches));
			#endregion

			sql.AppendFormat(" group by {0}.id_product,{0}.id_advertiser ", dataTable.Prefix);
			sql.AppendFormat(" order by {0}.id_product,{0}.id_advertiser ", dataTable.Prefix);
			sql.Append(" ) where total_N >0 ");
			sql.Append(" group by  id_product ");
			#endregion

			#region Execution de la requête
			IDataSource dataSource = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis, WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].NlsSort);
			try {
				ds = dataSource.Fill(sql.ToString());
			}
			catch (System.Exception err) {
				throw new ProductClassIndicatorsDALException(string.Format("Error while loading data: {0}", sql), err);
			}
			#endregion			

			return ds;
		}
		#endregion

    }
}
