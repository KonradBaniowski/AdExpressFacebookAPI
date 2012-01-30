using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.ProductClassIndicators.DAL;

using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpressI.ProductClassIndicators.Russia.Engines;
using System.Drawing;
using FctIndicators = TNS.AdExpressI.ProductClassIndicators.Russia.Functions;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Translation;
using Dundas.Charting.WebControl;
using TNS.AdExpress.Domain.Units;

namespace TNS.AdExpressI.ProductClassIndicators.Russia.Charts{

    [ToolboxData("<{0}:ChartTop runat=server></{0}:ChartTop>")]
    public class ChartEvolution : TNS.AdExpressI.ProductClassIndicators.Charts.ChartEvolution
    {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User sessions</param>
        /// <param name="dalLayer">Data Access Layer</param>
        public ChartEvolution(WebSession session, IProductClassIndicatorsDAL dalLayer, CstResult.MotherRecap.ElementType classifLevel)
            : base(session, dalLayer,classifLevel){}
        #endregion

        #region Init Engine
        /// <summary>
        /// Init Engine
        /// </summary>
        override protected void InitEngine()
        {
            _engine = new EngineEvolution(this._session, this._dalLayer);
        }
        #endregion

        #region SetSeriesData
        /// <summary>
        /// Set Series Data
        /// </summary>
        /// <param name="tab">tab</param>
        /// <param name="series">series</param>
        /// <param name="last">last</param>
        /// <param name="hasComp">has Competitor</param>
        /// <param name="hasRef">has Reference</param>
        /// <param name="hasMixed">has Mixed</param>
        protected override void SetSeriesData(object[,] tab, Series series, long last, ref bool hasComp, ref bool hasRef, ref   bool hasMixed)
        {
            double ecart = 0;
            string sEcart = string.Empty;
            int typeElt = 0;
            int compteur = 0;
            AdExpressCultureInfo fp = WebApplicationParameters.AllowedLanguages[_session.DataLanguage].CultureInfo;
            string u = "";
            for (int i = 0; i < tab.GetLongLength(0) && i < 10; i++)
            {
                if (!FctIndicators.IsNull(tab[i, EngineEvolution.ECART]))
                {
                    //Increase
                    ecart = Convert.ToDouble(tab[i, EngineEvolution.ECART], fp);
                    if (ecart > 0)
                    {
                        u = FctIndicators.ConvertUnitValueToString(tab[i, EngineEvolution.ECART], _session.Unit, fp);
                        if (!string.IsNullOrEmpty(u))
                        {
                            series.Points.AddXY(tab[i, EngineEvolution.PRODUCT].ToString(), Math.Round(FctIndicators.ConvertToDouble(u.Trim(), fp)));
                            series.Points[compteur].ShowInLegend = true;

                            #region Reference or competitor ?
                            if (tab[i, EngineEvolution.COMPETITOR] != null)
                            {
                                typeElt = Convert.ToInt32(tab[i, EngineEvolution.COMPETITOR]);
                                if (typeElt == 3)
                                {
                                    series.Points[compteur].Color = (Color)_colorConverter.ConvertFrom(_mixedSerieColor);
                                    hasMixed = true;
                                }
                                else if (typeElt == 2)
                                {
                                    series.Points[compteur].Color = (Color)_colorConverter.ConvertFrom(_competitorSerieColor);
                                    hasComp = true;
                                }
                                else if (typeElt == 1)
                                {
                                    series.Points[compteur].Color = (Color)_colorConverter.ConvertFrom(_referenceSerieColor);
                                    hasRef = true;
                                }
                            }
                            #endregion

                            compteur++;
                        }
                    }
                }
                //Decrease
                if (!FctIndicators.IsNull(tab[last, EngineEvolution.ECART]))
                {
                    ecart = Convert.ToDouble(tab[last, EngineEvolution.ECART], fp);
                    if (ecart < 0)
                    {
                        u = FctIndicators.ConvertUnitValueToString(tab[last, EngineEvolution.ECART], _session.Unit, fp);
                        if (!string.IsNullOrEmpty(u))
                        {
                            series.Points.AddXY(tab[last, EngineEvolution.PRODUCT].ToString(), Math.Round(FctIndicators.ConvertToDouble(u.Trim(), fp)));
                            series.Points[compteur].ShowInLegend = true;

                            #region Reference or competitor ?
                            if (tab[last, EngineEvolution.COMPETITOR] != null)
                            {
                                typeElt = Convert.ToInt32(tab[last, EngineEvolution.COMPETITOR]);
                                if (typeElt == 3)
                                {
                                    series.Points[compteur].Color = (Color)_colorConverter.ConvertFrom(_mixedSerieColor);
                                    hasMixed = true;
                                }
                                else if (typeElt == 2)
                                {
                                    series.Points[compteur].Color = (Color)_colorConverter.ConvertFrom(_competitorSerieColor);
                                    hasComp = true;
                                }
                                else if (typeElt == 1)
                                {
                                    series.Points[compteur].Color = (Color)_colorConverter.ConvertFrom(_referenceSerieColor);
                                    hasRef = true;
                                }
                            }
                            #endregion

                            compteur++;
                        }
                    }
                }
                last--;

            }
        }
        #endregion

        #region GetUnit
        /// <summary>
        /// Get unit
        /// </summary>
        /// <returns>unit</returns>
        protected override UnitInformation GetUnit()
        {
            return _session.GetSelectedUnit();

        }
        #endregion

    }
}
