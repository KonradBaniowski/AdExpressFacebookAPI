#region Information
/*
 * Author : G Ragneau
 * Created on : 17/07/2008
 * Modification:
 *      Author - Date - Description
 * 
 * Origine:
 *      Auteur: G. Ragneau
 *      Date de création: 22/09/2004
 *      Date de modification: 27/09/2004
 *      18/02/2005	A.Obermeyer		rajout Marque en personnalisation
 *      23/08/2005	G. Facon		Solution temporaire pour les IDataSource
 */
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;


using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstComparisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion;
using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.ProductClassIndicators.DAL.DALEngines;




namespace TNS.AdExpressI.ProductClassIndicators.DAL
{

    /// <summary>
    /// Default behaviour of DAL layer
    /// </summary>
    public abstract class ProductClassIndicatorsDAL : IProductClassIndicatorsDAL
    {

        #region Attributes
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// ID media type
        /// </summary>
        protected CstDBClassif.Vehicles.names _vehicle;
        /// <summary>
        /// DAL utilities
        /// </summary>
        protected DALUtilities _dalUtilities = null;
        /// <summary>
        /// Period Beginning date
        /// </summary>
        protected DateTime _periodBegin;
        /// <summary>
        /// Period End date
        /// </summary>
        protected DateTime _periodEnd;
        #endregion

        #region Accessors
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
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession Session
        {
            get { return _session; }
            set { _session = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public ProductClassIndicatorsDAL(WebSession session)
        {
            _session = session;
            _dalUtilities = new DALUtilities(session);
            _vehicle = _dalUtilities.Vehicle;           
        }
        #endregion

        #region IProductClassIndicatorsDAL Membres
        /// <summary>
        /// Implements default data access layer for top indicator
        /// </summary>
        /// <param name="classifLevel">Classification detail (product or advertiser)</param>
        /// <param name="typeYear">Type of study</param>
        public virtual DataSet GetTops(CstResult.PalmaresRecap.typeYearSelected typeYear, CstResult.MotherRecap.ElementType classifLevel)
        {
            return (new DALEngineTop(this._session, typeYear, classifLevel,_dalUtilities)).GetData();
        }
		/// <summary>
		/// Implements method to get Products Personnalisation type
		/// </summary>
		/// <param name="idProductList">Id product list</param>
		/// <param name="strYearId">Year Id</param>
		/// <returns>DataTable[id_product,inref,incomp,inneutral]</returns>
        public virtual DataSet GetProductsPersonnalisationType(String idProductList, string strYearId)
        {
			return (new DALEngine(this._session, _dalUtilities)).GetProductsPersonnalisationType(idProductList,strYearId);
		}
        /// <summary>
        /// Implements default data access layer for evolution indicator
        /// </summary>
        /// <param name="classifLevel">Classification detail (product or advertiser)</param>
        /// <param name="typeYear">Type of study</param>
        public virtual DataSet GetEvolution(CstResult.MotherRecap.ElementType classifLevel)
        {
            return (new DALEngineEvolution(this._session, classifLevel, _dalUtilities)).GetData();
        }        /// <summary>
        /// Implements default data access layer for novelty indicator
        /// </summary>
        /// <param name="classifLevel">Classification detail (product or advertiser)</param>
        public virtual DataSet GetNovelty(CstResult.MotherRecap.ElementType classifLevel)
        {
            return (new DALEngineNovelty(this._session, classifLevel, _dalUtilities)).GetData();
        }
        /// <summary>
        /// Get Total of sector or advertising market on the Data from database for the report of the user session
        /// </summary>
        /// <param name="typeYear">Get total of year N or year N-1?</param>
        /// <returns>Total depending on User selection</returns>
        public virtual  double GetTotal(CstResult.PalmaresRecap.typeYearSelected typeYear)
        {
            return (new DALEngine(this._session,_dalUtilities)).GetTotal(typeYear);
        }
        /// <summary>
        /// Get Total of sector or advertising market on the Data from database for the report of the user session
        /// </summary>
        /// <param name="typeYear">Get total of year N or year N-1?</param>
        /// <returns>Total depending on User selection</returns>
        public virtual double GetMonthTotal(CstResult.PalmaresRecap.typeYearSelected typeYear)
        {
            return (new DALEngine(this._session,_dalUtilities)).GetMonthTotal(typeYear);
        }
        /// <summary>
        /// Get Total on the Data from database for the report of the user session
        /// </summary>
        /// <param name="totalType">Type of total</param>
        /// <param name="typeYear">Get total of year N or year N-1?</param>
        /// <returns>Total depending on User selection</returns>
        public virtual double GetTotal(CstComparisonCriterion totalType, CstResult.PalmaresRecap.typeYearSelected typeYear)
        {
            return (new DALEngine(this._session, _dalUtilities)).GetTotal(totalType, typeYear);
        }
        /// <summary>
        /// Get Total on the Data from database for the report of the user session
        /// </summary>
        /// <param name="totalType">Type of total</param>
        /// <param name="typeYear">Get total of year N or year N-1?</param>
        /// <returns>Total depending on User selection</returns>
        public virtual double GetMonthTotal(CstComparisonCriterion totalType, CstResult.PalmaresRecap.typeYearSelected typeYear)
        {
            return (new DALEngine(this._session, _dalUtilities)).GetMonthTotal(totalType, typeYear);
        }
        /// <summary>
        /// Get required data to build a seasonality table
        /// </summary>
        /// <param name="withAdvertisers">Include advertiser dimension</param>
        /// <param name="withRights">Aply product classification rights</param>
        public virtual DataSet GetSeasonalityTblData(bool withAdvertisers, bool withRights)
        {
            return (new DALEngineSeasonality(this._session, _dalUtilities)).GetTableData(withAdvertisers, withRights);
        }
        /// <summary>
        /// Get required data to build a seasonality graph
        /// </summary>
        /// <param name="withAdvertisers">Include advertiser dimension</param>
        /// <param name="withRights">Aply product classification rights</param>
        public virtual DataSet GetSeasonalityGraphData(bool withAdvertisers, bool withRights)
        {
            return (new DALEngineSeasonality(this._session, _dalUtilities)).GetChartData(withAdvertisers, withRights);
        }        /// <summary>
        /// Get Total of sector or advertising market on the Data from database for the seasonality table
        /// </summary>
        /// <returns>Total depending on User selection</returns>
        public virtual DataSet GetSeasonalityTotal()
        {
            return (new DALEngineSeasonality(this._session, _dalUtilities)).GetSeasonalityTotal();
        }
        /// <summary>
        /// Get number of elements on year N and N-1 if required
        /// </summary>
        /// <param name="totalType">Type of univers (selected univers, sector, market)</param>
        /// <param name="classifLevel">Level of classification to study</param>
        /// <returns>Number of distinct advertisers or products on N and N-1</returns>
        public virtual DataSet GetSummaryVolumes(CstComparisonCriterion totalType, CstResult.MotherRecap.ElementType classifLevel)
        {
            return (new DALEngineSummary(this._session, _dalUtilities)).GetVolumes(totalType, classifLevel);
        }
        /// <summary>
        /// Get investments on year N and N-1 if required
        /// </summary>
        /// <param name="totalType">Type of univers (selected univers, sector, market)</param>
        /// <returns>Investments on N and N-1</returns>
        public virtual DataSet GetSummaryInvestments(CstComparisonCriterion totalType)
        {
            return (new DALEngineSummary(this._session, _dalUtilities)).GetInvestments(totalType);
        }
        /// <summary>
        /// Define contract to access media strategy data when using a table
        /// </summary>
        /// <param name="totalType">Type of univers (selected univers, sector, market)</param>
        /// <param name="classifLevel">Level of classification to study</param>
        /// <returns>DataTable to build media Strategy Indicator</returns>
        public virtual DataSet GetMediaStrategyTblData(CstResult.MotherRecap.ElementType classifLevel, CstComparisonCriterion totalType, bool applyAdvFilter)
        {
            return (new DALEngineMediaStrategy(this._session, _dalUtilities)).GetTableData(classifLevel, totalType, applyAdvFilter);
        }
        /// <summary>
        /// Define contract to access media strategy tops data when using a table
        /// </summary>
        /// <param name="totalType">Type of univers (selected univers, sector, market)</param>
        /// <param name="classifLevel">Level of classification to study</param>
        /// <param name="isPluri">True if calcul requires tops of the pluri media, false if vehicles, categories or medias required</param>
        /// <param name="mediaLevel">Media Level to study</param>
        /// <returns>DataTable to build media Strategy Indicator</returns>
        public virtual DataSet GetMediaStrategyTopsData(CstResult.MotherRecap.ElementType classifLevel, CstComparisonCriterion totalType, CstResult.MediaStrategy.MediaLevel mediaLevel, bool isPluri)
        {
            return (new DALEngineMediaStrategy(this._session, _dalUtilities)).GetTopElements(classifLevel, totalType, mediaLevel, isPluri);
        }

        /// <summary>
        ///Get media strategy data 
        /// <remarks>Use specially in Russia</remarks>
        /// </summary>       
        /// <returns>DataTable to build media Strategy Indicator</returns>
        public virtual DataSet GetMediaStrategyData()
        {
            throw new NotImplementedException("Currently Implemented only in Russia"); 
        }
        /// <summary>
        /// Get summary data
        /// </summary>
        /// <remarks>Use specially in Russia</remarks>
        /// </summary>       
        /// <returns>DataTable to build Summary Indicator</returns>
        public virtual DataSet GetSummary()
        {
            throw new NotImplementedException("Currently Implemented only in Russia"); 
        }
        #endregion
    }
}
