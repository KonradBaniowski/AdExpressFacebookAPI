using System;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using FctUtilities = TNS.AdExpress.Web.Core.Utilities;

using Dundas.Charting.WebControl;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.ProductClassIndicators.DAL;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.FrameWork.Date;
using TNS.AdExpressI.ProductClassIndicators.Russia.Engines;
using System.Drawing;
using FctIndicators = TNS.AdExpressI.ProductClassIndicators.Russia.Functions;

namespace TNS.AdExpressI.ProductClassIndicators.Russia.Charts
{

    [ToolboxData("<{0}:ChartSeasonality runat=server></{0}:ChartSeasonality>")]
    public class ChartSeasonality : TNS.AdExpressI.ProductClassIndicators.Charts.ChartSeasonality
    {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User sessions</param>
        /// <param name="dalLayer">Data Access Layer</param>
        /// <param name="bigSize">Large graph?</param>
        public ChartSeasonality(WebSession session, IProductClassIndicatorsDAL dalLayer, bool bigSize)
            : base(session, dalLayer, bigSize)
        {
        }
        #endregion

        #region Init Engine
        /// <summary>
        /// Init Engine
        /// </summary>
        override protected void InitEngine() {
            _engine = new EngineSeasonality(this._session, this._dalLayer);
        }
        #endregion

        #region SetAdvertisSeries
        protected override void SetAdvertisSeries(object[,] tab, Dictionary<long, double> advertiserTotal, Dictionary<long, Series> advertiserSerie, ref int nbMonth)
        {
            Int64 idElement = 0;
            int cMonth = 0;
            int oldMonth = -1;

            if (tab != null)
            {
                for (int i = 0; i < tab.GetLength(0); i++)
                {
                    idElement = Convert.ToInt64(tab[i, EngineSeasonality.ID_ELEMENT_COLUMN_INDEX]);
                    if (idElement != 0)
                    {
                        if (!advertiserTotal.ContainsKey(idElement))
                        {
                            advertiserTotal.Add(idElement, Convert.ToDouble(tab[i, EngineSeasonality.INVEST_COLUMN_INDEX], WebApplicationParameters.AllowedLanguages[_session.DataLanguage].CultureInfo));
                            if (idElement != EngineSeasonality.ID_TOTAL_UNIVERSE_COLUMN_INDEX && idElement != EngineSeasonality.ID_TOTAL_MARKET_COLUMN_INDEX && idElement != EngineSeasonality.ID_TOTAL_SECTOR_COLUMN_INDEX)
                            {
                                advertiserSerie.Add(idElement, new Series());
                                this.Series.Add(advertiserSerie[idElement]);
                            }
                        }
                        else
                        {
                            advertiserTotal[idElement] = advertiserTotal[idElement] + Convert.ToDouble(tab[i, EngineSeasonality.INVEST_COLUMN_INDEX], WebApplicationParameters.AllowedLanguages[_session.DataLanguage].CultureInfo);
                        }
                    }
                    cMonth = Convert.ToInt32(tab[i, EngineSeasonality.ID_MONTH_COLUMN_INDEX]);
                    if (cMonth != 0 && oldMonth != cMonth)
                    {
                        nbMonth++;
                        oldMonth = cMonth;
                    }

                }
            }

        }
        #endregion
        #region Set Series Data
        /// <summary>
        /// Set Series Data
        /// </summary>
        /// <param name="tab">table</param>
        /// <param name="serieUnivers">serie Univers</param>
        /// <param name="serieSectorMarket">serie Sector Market</param>
        /// <param name="serieMediumMonth">serie Medium Month</param>
        /// <param name="advertiserTotal">advertiser Total</param>
        /// <param name="advertiserSerie">advertiser Serie</param>
        /// <param name="periodBegin">period Beginning</param>
        /// <param name="mediumMonth">medium Month</param>
        /// <param name="nbMonth">Nb Month</param>
        protected override void SetSeriesData(object[,] tab, Series serieUnivers, Series serieSectorMarket, Series serieMediumMonth, Dictionary<long, double> advertiserTotal, Dictionary<long, Series> advertiserSerie, DateTime periodBegin, ref  double mediumMonth, ref int nbMonth)
        {
            #region Series Data

            int compteur = -1;
            int oldMonth = -1;
            int cMonth = -1;
            double invest = 0;
            string month = string.Empty;
            Int64 idElement = 0;
            CultureInfo cInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);
            AdExpressCultureInfo fp = WebApplicationParameters.AllowedLanguages[_session.DataLanguage].CultureInfo;

            if (tab != null)
            {
                // Calcul des totaux pour les annonceurs
                for (int i = 0; i < tab.GetLength(0); i++)
                {
                    if (!FctIndicators.IsNull(tab[i, EngineSeasonality.INVEST_COLUMN_INDEX]))
                    {
                        cMonth = Convert.ToInt32(tab[i, EngineSeasonality.ID_MONTH_COLUMN_INDEX]);
                        if (oldMonth != cMonth)
                        {
                            compteur++;
                            oldMonth = cMonth;
                        }
                        idElement = Convert.ToInt64(tab[i, EngineSeasonality.ID_ELEMENT_COLUMN_INDEX]);
                        invest = Convert.ToDouble(tab[i, EngineSeasonality.INVEST_COLUMN_INDEX], fp);
                        month = MonthString.GetCharacters(periodBegin.AddMonths(compteur).Month, cInfo, 0);
                        if (idElement != 0)
                        {
                            if (idElement == EngineSeasonality.ID_TOTAL_UNIVERSE_COLUMN_INDEX)
                            {
                                if (advertiserTotal[idElement] > 0)
                                {
                                    serieUnivers.Points.AddXY(month, Math.Round(invest / advertiserTotal[idElement] * 100, 2));
                                }
                            }
                            else if (idElement == EngineSeasonality.ID_TOTAL_SECTOR_COLUMN_INDEX || idElement == EngineSeasonality.ID_TOTAL_MARKET_COLUMN_INDEX)
                            {
                                if (advertiserTotal[idElement] > 0)
                                {
                                    serieSectorMarket.Points.AddXY(month, Math.Round(invest / advertiserTotal[idElement] * 100, 2));
                                    serieMediumMonth.Points.AddXY(month, mediumMonth = Math.Round((double)100 / nbMonth, 2));
                                }
                            }
                            else
                            {
                                Series s = advertiserSerie[idElement];
                                s.Type = SeriesChartType.Line;
                                s.ShowLabelAsValue = true;
                                s.XValueType = Dundas.Charting.WebControl.ChartValueTypes.String;
                                s.YValueType = Dundas.Charting.WebControl.ChartValueTypes.Double;
                                s.Enabled = true;
                                s.Font = new Font("Arial", (float)8);
                                s.LegendText = tab[i, EngineSeasonality.LABEL_ELEMENT_COLUMN_INDEX].ToString();
                                s.SmartLabels.Enabled = true;
                                s.SmartLabels.CalloutLineStyle = ChartDashStyle.Dot;
                                s.SmartLabels.MinMovingDistance = 3;
                                s.LabelToolTip = tab[i, EngineSeasonality.LABEL_ELEMENT_COLUMN_INDEX].ToString();

                                if (advertiserTotal[idElement] > 0)
                                {
                                    advertiserSerie[idElement].Points.AddXY(month, Math.Round(invest / advertiserTotal[idElement] * 100, 2));

                                }
                            }
                        }
                    }
                }
            }
            #endregion
        }
        #endregion

        #region SetLegendsLabels
        /// <summary>
        ///Set Legends Labels 
        /// </summary>
        /// <param name="serieUnivers">serie Univers</param>
        /// <param name="serieSectorMarket">serie Sector Market</param>
        protected override void SetLegendsLabels(Series serieUnivers, Series serieSectorMarket)
        {
            AdExpressCultureInfo fp = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;

            // Univers
            serieUnivers.LegendText = GestionWeb.GetWebWord(1188, _session.SiteLanguage).ToString(fp);
            serieUnivers.LabelToolTip = GestionWeb.GetWebWord(1188, _session.SiteLanguage).ToString(fp);
            // Market
            if (_session.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.marketTotal)
            {
                serieSectorMarket.LegendText = GestionWeb.GetWebWord(1316, _session.SiteLanguage).ToString(fp);
                serieSectorMarket.LabelToolTip = GestionWeb.GetWebWord(1316, _session.SiteLanguage).ToString(fp);

            }
            // Famille
            else
            {
                serieSectorMarket.LegendText = GestionWeb.GetWebWord(1189, _session.SiteLanguage).ToString(fp);
                serieSectorMarket.LabelToolTip = GestionWeb.GetWebWord(1189, _session.SiteLanguage).ToString(fp);
            }
        }
        #endregion

    }

}
