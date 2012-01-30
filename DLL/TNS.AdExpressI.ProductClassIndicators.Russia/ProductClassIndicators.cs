#region Information
/*
 * Author : D. Mussuma
 * Created on : 18/07/2010
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
using Navigation = TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.ProductClassIndicators.DAL;
using System.Reflection;
using System.Data;
using TNS.AdExpressI.ProductClassIndicators.Russia.Charts;
using TNS.AdExpressI.ProductClassIndicators.Russia.Engines;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpressI.ProductClassIndicators.Exceptions;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpressI.ProductClassIndicators.Russia
{
    public class ProductClassIndicators : TNS.AdExpressI.ProductClassIndicators.ProductClassIndicators
    {

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public ProductClassIndicators(WebSession session) : base(session) { }
        #endregion

        #region IProductClassIndicators Membres

        #region Tops
        /// <summary>
        /// Get indicator Tops as a chart on the selected year
        /// </summary>
        /// <param name="_classifLevel">Classification level to display (product or advertiser)</param>
        /// <returns>Chart control filled with top elements</returns>
        public override Chart GetTopsChart(CstResult.MotherRecap.ElementType _classifLevel){
            TNS.AdExpressI.ProductClassIndicators.Charts.ChartProductClassIndicator chart = new ChartTop(this._webSession, this._dalLayer, _classifLevel);
            chart.ChartType = _chartType;
            return chart;
        }
        /// <summary>
        /// Get Top indicator as a table
        /// </summary>
        /// <returns>Html code</returns>
        public override string GetTopsTable(){
            TNS.AdExpressI.ProductClassIndicators.Engines.Engine engine = new EngineTop(this._webSession, this._dalLayer);
            engine.Excel = _excel;
            return engine.GetResult().ToString();
        }
        #endregion

        #region Novelty
        /// <summary>
        /// Get indicator novelties as a graph (product and advertiser)
        /// </summary>
        /// <returns>Chart control filled with top elements</returns>
        public override string GetNoveltyChart(){

            if (this._engine == null || !(this._engine is EngineNovelty)){
                this._engine = new EngineNovelty(this._webSession, this._dalLayer);
            }
            EngineNovelty e = (EngineNovelty)this._engine;
            e.Excel = _excel;
            return e.GetGraph().ToString();

        }
        /// <summary>
        /// Get indicator novelties as a Table (product and advertiser)
        /// </summary>
        /// <returns>Chart control filled with top elements</returns>
        public override string GetNoveltyTable(){

            if (this._engine == null || !(this._engine is EngineNovelty)){
                this._engine = new EngineNovelty(this._webSession, this._dalLayer);
            }
            EngineNovelty e = (EngineNovelty)this._engine;
            e.Excel = _excel;
            return e.GetResult().ToString();

        }
        #endregion

        #region Evolution
        /// <summary>
        /// Get indicator Evolution as a chart on the selected year
        /// </summary>
        /// <param name="_classifLevel">Classification level to display (product or advertiser)</param>
        /// <returns>Chart control filled with evolution elements</returns>
        public override Chart GetEvolutionChart(CstResult.MotherRecap.ElementType _classifLevel){

            TNS.AdExpressI.ProductClassIndicators.Charts.ChartProductClassIndicator chart = new ChartEvolution(this._webSession, this._dalLayer, _classifLevel);
            chart.ChartType = _chartType;
            return chart;
        }
        /// <summary>
        /// Get Evolution indicator as a table
        /// </summary>
        /// <returns>Html code</returns>
        public override string GetEvolutionTable(){

            TNS.AdExpressI.ProductClassIndicators.Engines.Engine engine = new EngineEvolution(this._webSession, this._dalLayer);
            engine.Excel = _excel;
            return engine.GetResult().ToString();
        }
        #endregion

        #region Summary
        /// <summary>
        /// Get Summary indicator
        /// </summary>
        /// <returns>Html code</returns>
        public override string GetSummary(){

            TNS.AdExpressI.ProductClassIndicators.Engines.Engine engine = new EngineSummary(this._webSession, this._dalLayer);
            engine.Excel = _excel;
            engine.Pdf = _pdf;
            return engine.GetResult().ToString();
        }
        #endregion

        #region Media Strategy
        /// <summary>
        /// Get indicator Evolution as a chart on the selected year
        /// </summary>
        /// <returns>Chart control filled with evolution elements</returns>
        public override Chart GetMediaStrategyChart()
        {
            TNS.AdExpressI.ProductClassIndicators.Charts.ChartProductClassIndicator chart = new ChartMediaStartegy(this._webSession, this._dalLayer);
            chart.ChartType = _chartType;
            return chart;
        }
        /// <summary>
        /// Get Evolution indicator as a table
        /// </summary>
        /// <returns>Html code</returns>
        public override string GetMediaStrategyTable()
        {    
            throw new NotImplementedException("This methods is not implemented");
        }
        #endregion

        #region Seasonality
        /// <summary>
        /// Get indicator seasonality as a chart on the selected year
        /// </summary>
        /// <param name="bigSize">Big chart</param>
        /// <returns>Chart control filled with seasonality elements</returns>
        public override Chart GetSeasonalityChart(bool bigSize)
        {
            TNS.AdExpressI.ProductClassIndicators.Charts.ChartProductClassIndicator chart = new Charts.ChartSeasonality(this._webSession, this._dalLayer, bigSize); 
            chart.ChartType = _chartType;
            return chart;
        }
        /// <summary>
        /// Get seasonality indicator as a table
        /// </summary>
        /// <returns>Html code</returns>
        public override string GetSeasonalityTable()
        {
            throw new NotImplementedException("This methods is not implemented");
        }
        #endregion

        #region Get Result Table
        /// <summary>
        /// Get Result Table
        /// </summary>
        /// <returns></returns>
        public override ResultTable GetResultTable() {
            TNS.AdExpressI.ProductClassIndicators.Engines.Engine result = null;
            try {
                switch (_webSession.CurrentTab) {
                    case MotherRecap.MEDIA_STRATEGY:
                        result = new Engines.EngineMediaStrategy(_webSession, this._dalLayer);
                        break;
                    case MotherRecap.PALMARES:
                        result = new Engines.EngineTop(_webSession, this._dalLayer);
                        break;
                    case MotherRecap.SEASONALITY:
                        result = new Engines.EngineSeasonality(_webSession, this._dalLayer);
                        break;
                    case EvolutionRecap.EVOLUTION:
                        result = new Engines.EngineEvolution(_webSession, this._dalLayer);
                        break;
                    case MotherRecap.NOVELTY:
                        result = new Engines.EngineNovelty(_webSession, this._dalLayer);
                        break;
                    default:
                        result = new Engines.EngineMediaStrategy(_webSession, this._dalLayer);
                        break;
                }
            }
            catch (System.Exception err) {
                throw (new ProductClassIndicatorsException("Impossible to compute Russia Product Class Indicator results", err));
            }

            return result.GetResultTable();
        }

        #endregion

        #endregion
    }
}
