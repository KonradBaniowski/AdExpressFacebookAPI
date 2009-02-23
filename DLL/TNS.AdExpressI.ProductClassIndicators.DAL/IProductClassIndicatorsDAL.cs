#region Information
/*
 * Author : G Ragneau
 * Created on : 30/06/2008
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

using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using CstComparisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion;

using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.ProductClassIndicators.DAL
{
    /// <summary>
    /// Contract to respect to provide access to the DAL Layer
    /// </summary>
    public interface IProductClassIndicatorsDAL
    {
        /// <summary>
        /// Get Total of sector or advertising market on the Data from database for the report of the user session
        /// </summary>
        /// <param name="typeYear">Get total of year N or year N-1?</param>
        /// <returns>Total depending on User selection</returns>
        double GetTotal(CstResult.PalmaresRecap.typeYearSelected typeYear);
        /// <summary>
        /// Get Total of sector or advertising market on the Data from database for the report of the user session
        /// </summary>
        /// <param name="typeYear">Get total of year N or year N-1?</param>
        /// <returns>Total of the last month of the selected period depending on User selection</returns>
        double GetMonthTotal(CstResult.PalmaresRecap.typeYearSelected typeYear);
        /// <summary>
        /// Get Total on the Data from database for the report of the user session
        /// </summary>
        /// <param name="totalType">Type of total</param>
        /// <param name="typeYear">Get total of year N or year N-1?</param>
        /// <returns>Total depending on User selection</returns>
        double GetTotal(CstComparisonCriterion totalType, CstResult.PalmaresRecap.typeYearSelected typeYear);
        /// <summary>
        /// Get Total on the Data from database for the report of the user session
        /// </summary>
        /// <param name="totalType">Type of total</param>
        /// <param name="typeYear">Get total of year N or year N-1?</param>
        /// <returns>Total depending on User selection</returns>
        double GetMonthTotal(CstComparisonCriterion totalType, CstResult.PalmaresRecap.typeYearSelected typeYear);
        /// <summary>
        /// Get data for Indocator Top (Table or graph)
        /// </summary>
        /// <param name="typeYear">Type of study</param>
        /// <param name="classifLevel">Detail of the indicator</param>        
        /// <returns>DataSet filled with data required to compute result</returns>
        DataSet GetTops(CstResult.PalmaresRecap.typeYearSelected typeYear, CstResult.MotherRecap.ElementType classifLevel);
		/// <summary>
		/// Get Products Personnalisation type
		/// </summary>
		/// <param name="idProductList">Id product list</param>
		///<param name="strYearId">Year Id</param>
		/// <returns>DataTable[id_product,inref,incomp,inneutral]</returns>
		DataSet GetProductsPersonnalisationType(String idProductList, string strYearId);
        /// <summary>
        /// Get data for Indocator Evolution (Table or graph)
        /// </summary>
        /// <param name="classifLevel">Detail of the indicator</param>        
        /// <returns>DataSet filled with data required to compute result</returns>
        DataSet GetEvolution(CstResult.MotherRecap.ElementType classifLevel);
        /// <summary>
        /// Define contract to access novelty data
        /// </summary>
        /// <param name="classifLevel">Classification detail (product or advertiser)</param>
        DataSet GetNovelty(CstResult.MotherRecap.ElementType classifLevel);
        /// <summary>
        /// Define contract to access seasonality data when using a table
        /// </summary>
        DataSet GetSeasonalityTblData(bool withAdvertisers, bool withRights);
        /// <summary>
        /// Define contract to access seasonality data when using a graph
        /// </summary>
        DataSet GetSeasonalityGraphData(bool withAdvertisers, bool withRights);
        /// <summary>
        /// Get Total of sector or advertising market on the Data from database for the seasonality table
        /// </summary>
        /// <returns>Total depending on User selection</returns>
        DataSet GetSeasonalityTotal();
        /// <summary>
        /// Get number of elements on year N and N-1 if required
        /// </summary>
        /// <param name="totalType">Type of univers (selected univers, sector, market)</param>
        /// <param name="classifLevel">Level of classification to study</param>
        /// <returns>Number of distinct advertisers or products on N and N-1</returns>
        DataSet GetSummaryVolumes(CstComparisonCriterion totalType, CstResult.MotherRecap.ElementType classifLevel);
        /// <summary>
        /// Get investments on year N and N-1 if required
        /// </summary>
        /// <param name="totalType">Type of univers (selected univers, sector, market)</param>
        /// <returns>Investments on N and N-1</returns>
        DataSet GetSummaryInvestments(CstComparisonCriterion totalType);
        /// <summary>
        /// Define contract to access media strategy data when using a table
        /// </summary>
        /// <param name="totalType">Type of univers (selected univers, sector, market)</param>
        /// <param name="classifLevel">Level of classification to study</param>
        /// <returns>DataTable to build media Strategy Indicator</returns>
        DataSet GetMediaStrategyTblData(CstResult.MotherRecap.ElementType classifLevel, CstComparisonCriterion totalType, bool applyAdvFilter);
        /// <summary>
        /// Define contract to access media strategy tops data when using a table
        /// </summary>
        /// <param name="totalType">Type of univers (selected univers, sector, market)</param>
        /// <param name="classifLevel">Level of classification to study</param>
        /// <param name="isPluri">True if calcul requires tops of the pluri media, false if vehicles, categories or medias required</param>
        /// <param name="mediaLevel">Media Level to study</param>
        /// <returns>DataTable to build media Strategy Indicator</returns>
        DataSet GetMediaStrategyTopsData(CstResult.MotherRecap.ElementType classifLevel, CstComparisonCriterion totalType, CstResult.MediaStrategy.MediaLevel mediaLevel, bool isPluri);
    }
}
