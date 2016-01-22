using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Anubis.Hotep.Common;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpressI.ProductClassIndicators.Russia.Engines;
using FctIndicators = TNS.AdExpress.Anubis.Hotep.Russia.Functions;
using System.Drawing;
using Dundas.Charting.WinControl;

namespace TNS.AdExpress.Anubis.Hotep.Russia.UI
{
    public  class UIEvolutionGraph : TNS.AdExpress.Anubis.Hotep.UI.UIEvolutionGraph
    {
        #region Constructeur
        public UIEvolutionGraph(WebSession webSession, IDataSource dataSource, HotepConfig config, object[,] tab, TNS.FrameWork.WebTheme.Style style)
            : base(webSession,  dataSource, config,tab, style)
        {            
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
        protected override void SetSeriesData(Series series, long last, ref bool hasComp, ref bool hasRef, ref   bool hasMixed, ref Color colorTemp)
        {
            double ecart = 0;
            string sEcart = string.Empty;
            int typeElt = 0;
            int compteur = 0;
            AdExpressCultureInfo fp = WebApplicationParameters.AllowedLanguages[_webSession.DataLanguage].CultureInfo;
            string u = "";
            for (int i = 0; i < _tab.GetLongLength(0) && i < 10; i++)
            {
                if (!FctIndicators.IsNull(_tab[i, EngineEvolution.ECART]))
                {
                    //Increase
                    ecart = Convert.ToDouble(_tab[i, EngineEvolution.ECART], fp);
                    if (ecart > 0)
                    {
                        u = FctIndicators.ConvertUnitValueToString(_tab[i, EngineEvolution.ECART], _webSession.Unit, fp);
                        if (!string.IsNullOrEmpty(u))
                        {
                            series.Points.AddXY(_tab[i, EngineEvolution.PRODUCT].ToString(), Math.Round(FctIndicators.ConvertToDouble(u.Trim(), fp)));
                            series.Points[compteur].ShowInLegend = true;

                            #region Reference or competitor ?
                            if (_tab[i, EngineEvolution.COMPETITOR] != null)
                            {
                                typeElt = Convert.ToInt32(_tab[i, EngineEvolution.COMPETITOR]);
                                if (typeElt == 3)
                                {
                                    _style.GetTag("EvolutionGraphColorLegendItemMixed").SetStyleDundas(ref colorTemp);
                                    series.Points[compteur].Color = colorTemp;
                                    hasMixed = true;
                                }
                                else if (typeElt == 2)
                                {
                                    _style.GetTag("EvolutionGraphColorLegendItemCompetitor").SetStyleDundas(ref colorTemp);
                                    series.Points[compteur].Color = colorTemp;
                                    hasComp = true;
                                }
                                else if (typeElt == 1)
                                {
                                    _style.GetTag("EvolutionGraphColorLegendItemReference").SetStyleDundas(ref colorTemp);
                                    series.Points[compteur].Color = colorTemp;
                                    hasRef = true;
                                }
                            }
                            #endregion

                            compteur++;
                        }
                    }
                }
                //Decrease
                if (!FctIndicators.IsNull(_tab[last, EngineEvolution.ECART]))
                {
                    ecart = Convert.ToDouble(_tab[last, EngineEvolution.ECART], fp);
                    if (ecart < 0)
                    {
                        u = FctIndicators.ConvertUnitValueToString(_tab[last, EngineEvolution.ECART], _webSession.Unit, fp);
                        if (!string.IsNullOrEmpty(u))
                        {
                            series.Points.AddXY(_tab[last, EngineEvolution.PRODUCT].ToString(), Math.Round(FctIndicators.ConvertToDouble(u.Trim(), fp)));
                            series.Points[compteur].ShowInLegend = true;
                            series.Points[compteur].CustomAttributes = "LabelStyle=top";

                            #region Reference or competitor ?
                            if (_tab[last, EngineEvolution.COMPETITOR] != null)
                            {
                                typeElt = Convert.ToInt32(_tab[last, EngineEvolution.COMPETITOR]);
                                if (typeElt == 3)
                                {
                                    _style.GetTag("EvolutionGraphColorLegendItemMixed").SetStyleDundas(ref colorTemp);
                                    series.Points[compteur].Color = colorTemp;
                                    hasMixed = true;
                                }
                                else if (typeElt == 2)
                                {
                                    _style.GetTag("EvolutionGraphColorLegendItemCompetitor").SetStyleDundas(ref colorTemp);
                                    series.Points[compteur].Color = colorTemp;
                                    hasComp = true;
                                }
                                else if (typeElt == 1)
                                {
                                    _style.GetTag("EvolutionGraphColorLegendItemReference").SetStyleDundas(ref colorTemp);
                                    series.Points[compteur].Color = colorTemp;
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
    }
}
