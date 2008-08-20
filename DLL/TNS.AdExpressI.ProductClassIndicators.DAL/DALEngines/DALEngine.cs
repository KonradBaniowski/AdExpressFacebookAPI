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
        protected Table _recapGroup = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapGroup);
        protected Table _recapBrand = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapBrand);
        protected Table _recapProduct = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapProduct);
        protected Table _recapAdvertiser = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapAdvertiser);
        protected Table _dataRadioSegment = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapRadioSegment);
        protected Table _dataTVSegment = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapTvSegment);
        protected Table _dataPluriSegment = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapPluriSegment);
        protected Table _dataPressSegment = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapPressSegment);
        protected Table _dataOutdoorSegment = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapOutDoorSegment);
        protected Table _dataInternetSegment = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapInternetSegment);
        protected Table _dataCinemaSegment = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapCinemaSegment);
        protected Table _dataTacticSegment = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapTacticSegment);*/
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
        /// Vehicle to study
        /// </summary>
        protected CstDBClassif.Vehicles.names Vehicle
        {
            get { return _vehicle; }
            set { _vehicle = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public DALEngine(WebSession session)
        {
            _session = session;
            _vehicle = (CstDBClassif.Vehicles.names)((LevelInformation)_session.CurrentUniversMedia.FirstNode.Tag).ID;

            #region Dates
            //Get last available month depending on data delivering frequency
            string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(_session, _session.PeriodEndDate);

            if (int.Parse(absolutEndPeriod) < int.Parse(_session.PeriodBeginningDate))
                throw new NoDataException();

            _periodBegin = FctUtilities.Dates.getPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType);
            _periodEnd = FctUtilities.Dates.getPeriodEndDate(absolutEndPeriod, _session.PeriodType);

            FctUtilities.Dates.GetYearSelected(_session, ref _strYearId, ref _iYearId, _periodBegin);
            _iYearN1Id = (_iYearId == 1) ? 2 : 1;
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
                    return (productDetail) ? WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapCinema) : WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapCinemaSegment);
                case CstDBClassif.Vehicles.names.internet:
                    return (productDetail) ? WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapInternet) : WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapInternetSegment);
                case CstDBClassif.Vehicles.names.outdoor:
                    return (productDetail) ? WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapOutDoor) : WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapOutDoorSegment);
                case CstDBClassif.Vehicles.names.radio:
                    return (productDetail) ? WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapRadio) : WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapRadioSegment);
                case CstDBClassif.Vehicles.names.tv:
                    return (productDetail) ? WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapTv) : WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapTvSegment);
                case CstDBClassif.Vehicles.names.press:
                    return (productDetail) ? WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapPress) : WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapPressSegment);
                case CstDBClassif.Vehicles.names.plurimedia:
                    return (productDetail) ? WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapPluri) : WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapPluriSegment);
                case CstDBClassif.Vehicles.names.mediasTactics:
                    return (productDetail) ? WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapTactic) : WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapTacticSegment);
                case CstDBClassif.Vehicles.names.mobileTelephony:
                    return (productDetail) ? WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapMobileTel) : WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapMobileTelSegment);
                case CstDBClassif.Vehicles.names.emailing:
                    return (productDetail) ? WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapEmailing) : WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapEmailingSegment);
                case CstDBClassif.Vehicles.names.internationalPress:
                case CstDBClassif.Vehicles.names.others:
                case CstDBClassif.Vehicles.names.adnettrack:
                default:
                    throw new ProductClassIndicatorsDALException(string.Format("Vehicle \"{0}\" is unknown. Unable to find a matching table.", _vehicle.GetHashCode()));
            }

            return null;

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
                    if (_session.ComparativeStudy && (_iYearId == 0 || _iYearId == 1))
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
            Table dataTable = this.GetDataTable(false);
            double total = 0;

            #region Query Building
            sql.Append(" select sum(total) from (");
            sql.AppendFormat(" select {0}.id_sector, {1}.sector,", dataTable.Prefix, _recapSector.Prefix);
            sql.AppendFormat(" {0} as total", expenditureClause);
            sql.AppendFormat(" from {0}, ", dataTable.SqlWithPrefix);
            sql.AppendFormat(" {0}", _recapSector.SqlWithPrefix);
            sql.AppendFormat(" where {0}.id_sector={1}.id_sector", dataTable.Prefix, _recapSector.Prefix);
            sql.AppendFormat(" and {0}.id_language={1}", _recapSector.Prefix, _session.SiteLanguage);

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
                sql.Append(FctUtilities.SQLGenerator.getClassificationCustomerProductRight(_session, dataTable.Prefix, dataTable.Prefix, dataTable.Prefix, dataTable.Prefix, true));
            }

            #region Media selection
            sql.AppendFormat(" and {0} ", this.GetMediaSelection(dataTable.Prefix));
            #endregion

            sql.AppendFormat(" group by {0}.id_sector, {1}.sector ", dataTable.Prefix, _recapSector.Prefix);
            sql.Append(" ) ");

            #endregion

            #region Execute Query
            try
            {
                IDataSource source = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis);
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

        #region Get Media Selection
        /// <summary>
        /// Get Media Selection Clause (plurimedia?, Sponsorship?)
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        protected virtual string GetMediaSelection(string prefix){

            StringBuilder sql = new StringBuilder();
            // Multimedia
            if (_vehicle == CstDBClassif.Vehicles.names.plurimedia)
            {
                sql.Append(FctUtilities.SQLGenerator.getAdExpressUniverseCondition(_session, CstWeb.AdExpressUniverse.RECAP_MEDIA_LIST_ID, prefix, false));
            }
            else
            {
                sql.Append(FctUtilities.SQLGenerator.getAccessVehicleList(_session, prefix, false));
            }
            //TV Sponsorship rights
            if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_SPONSORSHIP_TV_ACCESS_FLAG))
            {
                sql.AppendFormat("  and  {0}.id_category not in (68) ", prefix);
            }
            sql.Append(FctUtilities.SQLGenerator.GetRecapMediaSelection(_session.GetSelection(_session.CurrentUniversMedia, CstRight.type.categoryAccess), _session.GetSelection(_session.CurrentUniversMedia, CstRight.type.mediaAccess), true));

            return sql.ToString();
        }
        #endregion
    }
}
