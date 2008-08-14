#region Information
/*
 * Author : G Ragneau
 * Created on : 31/07/2008
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Dundas.Charting.WebControl;

using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;

namespace TNS.AdExpressI.ProductClassIndicators
{
    /// <summary>
    /// Defines available indicators for Product Class Analysis
    /// </summary>
    public interface IProductClassIndicators
    {

        #region Tops
        /// <summary>
        /// Get indicator Tops as a chart on the selected year
        /// </summary>
        /// <param name="_classifLevel">Classification level to display (product or advertiser)</param>
        /// <returns>Chart control filled with top elements</returns>
        Chart GetTopsChart(CstResult.MotherRecap.ElementType _classifLevel);
        /// <summary>
        /// Get indicator Tops as a table
        /// </summary>
        /// <returns>Html code</returns>
        string GetTopsTable();
        #endregion

        #region Novelty
        ///// <summary>
        ///// Get indicator novelties as a graph
        ///// </summary>
        ///// <param name="_classifLevel">Classification level to display (product or advertiser)</param>
        ///// <returns>Chart control filled with top elements</returns>
        //string GetNoveltyChart(CstResult.MotherRecap.ElementType _classifLevel);
        /// <summary>
        /// Get indicator novelties as a graph (product and advertiser)
        /// </summary>
        /// <returns>Chart control filled with top elements</returns>
        string GetNoveltyChart();
        ///// <summary>
        ///// Get indicator novelties as a table
        ///// </summary>
        ///// <param name="_classifLevel">Classification level to display (product or advertiser)</param>
        ///// <returns>Chart control filled with top elements</returns>
        //string GetNoveltyTable(CstResult.MotherRecap.ElementType _classifLevel);
        /// <summary>
        /// Get indicator novelties as a Table (product and advertiser)
        /// </summary>
        /// <returns>Chart control filled with top elements</returns>
        string GetNoveltyTable();
        #endregion
    
        #region Evolution
        /// <summary>
        /// Get indicator Evolution as a chart on the selected year
        /// </summary>
        /// <param name="_classifLevel">Classification level to display (product or advertiser)</param>
        /// <returns>Chart control filled with evolution elements</returns>
        Chart GetEvolutionChart(CstResult.MotherRecap.ElementType _classifLevel);
        /// <summary>
        /// Get indicator Evolution as a table
        /// </summary>
        /// <returns>Html code</returns>
        string GetEvolutionTable();
        #endregion

        #region Summary
        /// <summary>
        /// Get Summary indicator
        /// </summary>
        /// <returns>Html code</returns>
        string GetSummary();
        #endregion

        #region Media Strategy
        /// <summary>
        /// Get indicator Media Strategy as a chart on the selected year
        /// </summary>
        /// <returns>Chart control filled with media strategy elements</returns>
        Chart GetMediaStrategyChart();
        /// <summary>
        /// Get media strategy indicator as a table
        /// </summary>
        /// <returns>Html code</returns>
        string GetMediaStrategyTable();
        #endregion

        #region Seasonality
        /// <summary>
        /// Get indicator seasonality as a chart on the selected year
        /// </summary>
        /// <param name="bigSize">Big chart</param>
        /// <returns>Chart control filled with seasonality elements</returns>
        Chart GetSeasonalityChart(bool bigSize);
        /// <summary>
        /// Get seasonality indicator as a table
        /// </summary>
        /// <returns>Html code</returns>
        string GetSeasonalityTable();
        #endregion

    }
}
