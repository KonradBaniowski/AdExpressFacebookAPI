using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Anubis.Hotep.Common;
using Dundas.Charting.WinControl;
using FrameWorkConstantes = TNS.AdExpress.Constantes.FrameWork;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.FrameWork;
using TNS.FrameWork.Date;
using TNS.AdExpressI.ProductClassIndicators.Engines;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Web;
using System.Globalization;
using FctIndicators = TNS.AdExpress.Anubis.Hotep.Russia.Functions;

namespace TNS.AdExpress.Anubis.Hotep.Russia.UI
{
    public class UISeasonalityGraph : TNS.AdExpress.Anubis.Hotep.UI.UISeasonalityGraph
    {
        #region Constructeur      
        public UISeasonalityGraph(WebSession webSession, IDataSource dataSource, HotepConfig config, object[,] tab, TNS.FrameWork.WebTheme.Style style)
            : base( webSession, dataSource,  config,  tab, style)
        {           
        }
        #endregion

        #region Seasonality
        /// <summary>
        /// Graphiques Seasonality
        /// </summary>
        public override void BuildSeasonality()
        {
            #region Variables
            bool referenceElement = false;
            bool competitorElement = false;
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Localization);
            Color colorTemp = Color.Black;
            #endregion

            #region Series Init
            Series serieUnivers = new Series("Seasonality");
            this.Series.Add(serieUnivers);
            Series serieSectorMarket = new Series();
            this.Series.Add(serieSectorMarket);
            Series serieMediumMonth = new Series();
            this.Series.Add(serieMediumMonth);

            ChartArea chartArea = new ChartArea();
            this.ChartAreas.Add(chartArea);
            string strChartArea = this.Series["Seasonality"].ChartArea;
            #endregion

            #region Advertiser totals
            Dictionary<long, double> advertiserTotal = new Dictionary<long, double>();
            Dictionary<long, Series> advertiserSerie = new Dictionary<long, Series>();

            //int cMonth = 0;
            //int oldMonth = -1;
            int nbMonth = 0;

            SetAdvertisSeries(advertiserTotal, advertiserSerie, ref nbMonth);
            #endregion

            #region Chart
            _style.GetTag("SeasonalityGraphSize").SetStyleDundas(this);
            this.BackGradientType = GradientType.TopBottom;
            _style.GetTag("SeasonalityGraphBackColor").SetStyleDundas(ref colorTemp);
            this.ChartAreas[strChartArea].BackColor = colorTemp;
            _style.GetTag("SeasonalityGraphLineEnCircle").SetStyleDundas(this);
            #endregion

            #region Titre
            Title title = new Title(GestionWeb.GetWebWord(1139, _webSession.SiteLanguage));
            this.Titles.Add(title);
            _style.GetTag("SeasonalityGraphTitleFont").SetStyleDundas(this.Titles[0]);
            #endregion

            #region Series Design
            serieUnivers.Type = SeriesChartType.Line;
            serieUnivers.ShowLabelAsValue = true;
            serieUnivers.XValueType = ChartValueTypes.String;
            serieUnivers.YValueType = ChartValueTypes.Double;
            serieUnivers.Enabled = true;
            _style.GetTag("SeasonalityGraphTitleFontSerieUnivers").SetStyleDundas(serieUnivers);
            serieUnivers["LabelStyle"] = "Top";

            serieSectorMarket.Type = SeriesChartType.Line;
            serieSectorMarket.ShowLabelAsValue = true;
            serieSectorMarket.XValueType = ChartValueTypes.String;
            serieSectorMarket.YValueType = ChartValueTypes.Double;
            serieSectorMarket.Enabled = true;
            _style.GetTag("SeasonalityGraphTitleFontSerieSectorMarket").SetStyleDundas(serieSectorMarket);
            serieSectorMarket["LabelStyle"] = "Bottom";

            serieMediumMonth.Type = SeriesChartType.Line;
            serieMediumMonth.ShowLabelAsValue = false;
            serieMediumMonth.XValueType = ChartValueTypes.String;
            serieMediumMonth.YValueType = ChartValueTypes.Double;
            serieMediumMonth.Enabled = true;
            _style.GetTag("SeasonalityGraphTitleFontSerieMediumMonth").SetStyleDundas(serieMediumMonth);
            serieMediumMonth.LabelToolTip = GestionWeb.GetWebWord(1233, _webSession.SiteLanguage);
            #endregion

            #region Series Data
            DateTime periodBegin = FctUtilities.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);
            double mediumMonth = 0;

            SetSeriesData(serieUnivers, serieSectorMarket, serieMediumMonth, advertiserTotal, advertiserSerie, periodBegin, ref mediumMonth, ref nbMonth);
            #endregion

            #region Légendes
            SetLegendsLabels(serieUnivers, serieSectorMarket);

            //Mois Moyen théorique
            serieMediumMonth.LegendText = GestionWeb.GetWebWord(1233, _webSession.SiteLanguage) + " " + mediumMonth.ToString() + " %";

            LegendItem legendItemReference = new LegendItem();
            legendItemReference.BorderWidth = 0;
            _style.GetTag("SeasonalityGraphColorLegendItemReference").SetStyleDundas(ref colorTemp);
            legendItemReference.Color = colorTemp;

            if (referenceElement)
            {
                legendItemReference.Name = GestionWeb.GetWebWord(1203, _webSession.SiteLanguage);
                this.Legends["Default"].CustomItems.Add(legendItemReference);
            }
            LegendItem legendItemCompetitor = new LegendItem();
            legendItemCompetitor.BorderWidth = 0;
            _style.GetTag("SeasonalityGraphColorLegendItemCompetitor").SetStyleDundas(ref colorTemp);
            legendItemCompetitor.Color = colorTemp;

            if (competitorElement)
            {

                legendItemCompetitor.Name = GestionWeb.GetWebWord(1202, _webSession.SiteLanguage);
                this.Legends["Default"].CustomItems.Add(legendItemCompetitor);
            }
            #endregion

            #region X axe
            this.ChartAreas[strChartArea].AxisX.LabelStyle.Enabled = true;
            this.ChartAreas[strChartArea].AxisX.LabelsAutoFit = false;
            _style.GetTag("SeasonalityGraphLabelFontAxisX").SetStyleDundas(this.ChartAreas[strChartArea].AxisX.LabelStyle);
            this.ChartAreas[strChartArea].AxisX.MajorGrid.LineWidth = 0;
            this.ChartAreas[strChartArea].AxisX.Interval = 1;
            this.ChartAreas[strChartArea].AxisX.LabelStyle.FontAngle = 35;
            #endregion

            #region Y axe
            this.ChartAreas[strChartArea].AxisY.Enabled = AxisEnabled.True;
            this.ChartAreas[strChartArea].AxisY.LabelStyle.Enabled = true;
            this.ChartAreas[strChartArea].AxisY.LabelsAutoFit = false;
            _style.GetTag("SeasonalityGraphLabelFontAxisY").SetStyleDundas(this.ChartAreas[strChartArea].AxisY.LabelStyle);
            _style.GetTag("SeasonalityGraphTitleFontAxisY").SetStyleDundas(this.ChartAreas[strChartArea].AxisY);
            this.ChartAreas[strChartArea].AxisY.MajorGrid.LineWidth = 0;
            #endregion

            #region Y2 axe
            this.ChartAreas[strChartArea].AxisY2.Enabled = AxisEnabled.True;
            this.ChartAreas[strChartArea].AxisY2.LabelStyle.Enabled = true;
            this.ChartAreas[strChartArea].AxisY2.LabelsAutoFit = false;
            _style.GetTag("SeasonalityGraphLabelFontAxisY2").SetStyleDundas(this.ChartAreas[strChartArea].AxisY2.LabelStyle);
            _style.GetTag("SeasonalityGraphTitleFontAxisY2").SetStyleDundas(this.ChartAreas[strChartArea].AxisY2);
            this.ChartAreas[strChartArea].AxisY2.Title = "" + GestionWeb.GetWebWord(1236, _webSession.SiteLanguage) + "";
            #endregion

          
        }
        #endregion

        #region SetAdvertisSeries
        /// <summary>
        /// Set Advertisers Series
        /// </summary>
        /// <param name="advertiserTotal">advertiser Total values</param>
        /// <param name="advertiserSerie">advertiser Serie</param>
        /// <param name="nbMonth">Number of monthes</param>
        protected override void SetAdvertisSeries(Dictionary<long, double> advertiserTotal, Dictionary<long, Series> advertiserSerie, ref int nbMonth)
        {
            Int64 idElement = 0;
            int cMonth = 0;
            int oldMonth = -1;

            if (_tab != null)
            {
                for (int i = 0; i < _tab.GetLength(0); i++)
                {
                    idElement = Convert.ToInt64(_tab[i, EngineSeasonality.ID_ELEMENT_COLUMN_INDEX]);
                    if (idElement != 0)
                    {
                        if (!advertiserTotal.ContainsKey(idElement))
                        {
                            advertiserTotal.Add(idElement, Convert.ToDouble(_tab[i, EngineSeasonality.INVEST_COLUMN_INDEX], WebApplicationParameters.AllowedLanguages[_webSession.DataLanguage].CultureInfo));
                            if (idElement != EngineSeasonality.ID_TOTAL_UNIVERSE_COLUMN_INDEX && idElement != EngineSeasonality.ID_TOTAL_MARKET_COLUMN_INDEX && idElement != EngineSeasonality.ID_TOTAL_SECTOR_COLUMN_INDEX)
                            {
                                advertiserSerie.Add(idElement, new Series());
                                this.Series.Add(advertiserSerie[idElement]);
                            }
                        }
                        else
                        {
                            advertiserTotal[idElement] = advertiserTotal[idElement] + Convert.ToDouble(_tab[i, EngineSeasonality.INVEST_COLUMN_INDEX], WebApplicationParameters.AllowedLanguages[_webSession.DataLanguage].CultureInfo);
                        }
                    }
                    cMonth = Convert.ToInt32(_tab[i, EngineSeasonality.ID_MONTH_COLUMN_INDEX]);
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
        /// <param name="serieUnivers">serie Univers</param>
        /// <param name="serieSectorMarket">serie Sector Market</param>
        /// <param name="serieMediumMonth">serie Medium Month</param>
        /// <param name="advertiserTotal">advertiser Total</param>
        /// <param name="advertiserSerie">advertiser Serie</param>
        /// <param name="periodBegin">period Beginning</param>
        /// <param name="mediumMonth">medium Month</param>
        /// <param name="nbMonth">Nb Month</param>
        protected override void SetSeriesData(Series serieUnivers, Series serieSectorMarket, Series serieMediumMonth, Dictionary<long, double> advertiserTotal, Dictionary<long, Series> advertiserSerie, DateTime periodBegin, ref  double mediumMonth, ref int nbMonth)
        {
            #region Series Data

            int compteur = -1;
            int oldMonth = -1;
            int cMonth = -1;
            double invest = 0;
            string month = string.Empty;
            Int64 idElement = 0;
            CultureInfo cInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Localization);
            AdExpressCultureInfo fp = WebApplicationParameters.AllowedLanguages[_webSession.DataLanguage].CultureInfo;

            if (_tab != null)
            {
                // Calcul des totaux pour les annonceurs
                for (int i = 0; i < _tab.GetLength(0); i++)
                {
                    if (!FctIndicators.IsNull(_tab[i, EngineSeasonality.INVEST_COLUMN_INDEX]))
                    {
                        cMonth = Convert.ToInt32(_tab[i, EngineSeasonality.ID_MONTH_COLUMN_INDEX]);
                        if (oldMonth != cMonth)
                        {
                            compteur++;
                            oldMonth = cMonth;
                        }
                        idElement = Convert.ToInt64(_tab[i, EngineSeasonality.ID_ELEMENT_COLUMN_INDEX]);
                        invest = Convert.ToDouble(_tab[i, EngineSeasonality.INVEST_COLUMN_INDEX], fp);
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
                                s.XValueType = ChartValueTypes.String;
                                s.YValueType = ChartValueTypes.Double;
                                s.Enabled = true;
                                _style.GetTag("SeasonalityGraphTitleFontSerieAdvertiser").SetStyleDundas(s);
                                s.LegendText = _tab[i, EngineSeasonality.LABEL_ELEMENT_COLUMN_INDEX].ToString();
                                s.Enabled = true;
                                _style.GetTag("SeasonalityGraphTitleFontSerieAdvertiser").SetStyleDundas(s);
                                s.LabelToolTip = _tab[i, EngineSeasonality.LABEL_ELEMENT_COLUMN_INDEX].ToString();

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
            AdExpressCultureInfo fp = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].CultureInfo;

            // Univers
            serieUnivers.LegendText = GestionWeb.GetWebWord(1188, _webSession.SiteLanguage).ToString(fp);
            serieUnivers.LabelToolTip = GestionWeb.GetWebWord(1188, _webSession.SiteLanguage).ToString(fp);
            // Market
            if (_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.marketTotal)
            {
                serieSectorMarket.LegendText = GestionWeb.GetWebWord(1316, _webSession.SiteLanguage).ToString(fp);
                serieSectorMarket.LabelToolTip = GestionWeb.GetWebWord(1316, _webSession.SiteLanguage).ToString(fp);

            }
            // Famille
            else
            {
                serieSectorMarket.LegendText = GestionWeb.GetWebWord(1189, _webSession.SiteLanguage).ToString(fp);
                serieSectorMarket.LabelToolTip = GestionWeb.GetWebWord(1189, _webSession.SiteLanguage).ToString(fp);
            }
        }
        #endregion

    }
}
