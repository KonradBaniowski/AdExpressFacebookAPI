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
using Navigation = TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpressI.ProductClassIndicators.Charts;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.ProductClassIndicators.DAL;
using System.Reflection;
using TNS.AdExpressI.ProductClassIndicators.Engines;


namespace TNS.AdExpressI.ProductClassIndicators
{
    /// <summary>
    /// Defines available indicators for Product Class Analysis
    /// </summary>
    public abstract class ProductClassIndicators:IProductClassIndicators
    {

        #region Attributes
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// Dal layer (should be ProductClassIndicatorsDAL)
        /// </summary>
        protected IProductClassIndicatorsDAL _dalLayer = null;
        /// <summary>
        /// Current running engine
        /// </summary>
        protected Engine _engine = null;
        /// <summary>
        /// Excel format?
        /// </summary>
        protected bool _excel = false;
        /// <summary>
        /// Chart Type
        /// </summary>
        protected ChartImageType _chartType = ChartImageType.Flash;
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
        /// Get / Set Excel format ?
        /// </summary>
        public bool Excel
        {
            get { return _excel; }
            set { _excel = value; }
        }
        /// <summary>
        /// Get / Set Chart Type
        /// </summary>
        public ChartImageType ChartType
        {
            get { return _chartType; }
            set { _chartType = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public ProductClassIndicators(WebSession session)
        {
            _session = session;
            Navigation.Module module =  Navigation.ModulesList.GetModule(session.CurrentModule);
            if (module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the product class indicators."));
            object[] parameters = new object[1];
            parameters[0] = _session;
            _dalLayer = (IProductClassIndicatorsDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryDataAccessLayer.AssemblyName, module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);

        }
        #endregion

        #region IProductClassIndicators Membres

        #region Tops
        /// <summary>
        /// Get indicator Tops as a chart on the selected year
        /// </summary>
        /// <param name="_classifLevel">Classification level to display (product or advertiser)</param>
        /// <returns>Chart control filled with top elements</returns>
        public virtual Chart GetTopsChart(CstResult.MotherRecap.ElementType _classifLevel)
        {
            ChartProductClassIndicator chart = new ChartTop(this._session, this._dalLayer, _classifLevel);
            chart.ChartType = _chartType;
            return chart;
        }
        /// <summary>
        /// Get Top indicator as a table
        /// </summary>
        /// <returns>Html code</returns>
        public virtual string GetTopsTable()
        {
            Engine engine = new EngineTop(this._session, this._dalLayer);
            engine.Excel = _excel;
            return engine.GetResult().ToString();
        }
        #endregion

        #region Novelty
        /// <summary>
        /// Get indicator novelties as a graph (product and advertiser)
        /// </summary>
        /// <returns>Chart control filled with top elements</returns>
        public virtual string GetNoveltyChart()
        {

            if (this._engine == null || !(this._engine is EngineNovelty))
            {
                this._engine = new EngineNovelty(this._session, this._dalLayer);
            }
            EngineNovelty e = (EngineNovelty)this._engine;
            e.Excel = _excel;
            return e.GetGraph().ToString();

        }
       /// <summary>
        /// Get indicator novelties as a Table (product and advertiser)
        /// </summary>
        /// <returns>Chart control filled with top elements</returns>
        public virtual string GetNoveltyTable()
        {

            if (this._engine == null || !(this._engine is EngineNovelty))
            {
                this._engine = new EngineNovelty(this._session, this._dalLayer);
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
        public virtual Chart GetEvolutionChart(CstResult.MotherRecap.ElementType _classifLevel)
        {
            ChartProductClassIndicator chart = new ChartEvolution(this._session, this._dalLayer, _classifLevel);
            chart.ChartType = _chartType;
            return chart;
        }
        /// <summary>
        /// Get Evolution indicator as a table
        /// </summary>
        /// <returns>Html code</returns>
        public virtual string GetEvolutionTable()
        {
            Engine engine = new EngineEvolution(this._session, this._dalLayer);
            engine.Excel = _excel;
            return engine.GetResult().ToString();
        }
        #endregion

        #region Summary
        /// <summary>
        /// Get Summary indicator
        /// </summary>
        /// <returns>Html code</returns>
        public virtual string GetSummary()
        {
            Engine engine = new EngineSummary(this._session, this._dalLayer);
            engine.Excel = _excel;
            return engine.GetResult().ToString();
        }        
        #endregion

        #region Media Strategy
        /// <summary>
        /// Get indicator Evolution as a chart on the selected year
        /// </summary>
        /// <returns>Chart control filled with evolution elements</returns>
        public virtual Chart GetMediaStrategyChart()
        {
            ChartProductClassIndicator chart = new ChartMediaStartegy(this._session, this._dalLayer);
            chart.ChartType = _chartType;
            return chart;
        }
        /// <summary>
        /// Get Evolution indicator as a table
        /// </summary>
        /// <returns>Html code</returns>
        public virtual string GetMediaStrategyTable()
        {
            Engine engine = new EngineMediaStrategy(this._session, this._dalLayer);
            engine.Excel = _excel;
            return engine.GetResult().ToString();
        }
        #endregion

        #region Seasonality
        /// <summary>
        /// Get indicator seasonality as a chart on the selected year
        /// </summary>
        /// <param name="bigSize">Big chart</param>
        /// <returns>Chart control filled with seasonality elements</returns>
        public virtual Chart GetSeasonalityChart(bool bigSize)
        {
            ChartProductClassIndicator chart = new ChartSeasonality(this._session, this._dalLayer, bigSize);
            chart.ChartType = _chartType;
            return chart;
        }
        /// <summary>
        /// Get seasonality indicator as a table
        /// </summary>
        /// <returns>Html code</returns>
        public virtual string GetSeasonalityTable()
        {
            Engine engine = new EngineSeasonality(this._session, this._dalLayer);
            engine.Excel = _excel;
            return engine.GetResult().ToString();
        }
        #endregion

        #endregion

    }
}
