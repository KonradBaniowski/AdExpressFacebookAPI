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
using TNS.AdExpressI.ProductClassIndicators.Engines;
using System.Drawing;

namespace TNS.AdExpressI.ProductClassIndicators.Charts
{

    [ToolboxData("<{0}:ChartSeasonality runat=server></{0}:ChartSeasonality>")]
    public class ChartSeasonality : ChartProductClassIndicator
    {

        #region Variables
        /// <summary>
        /// Engine Seasonality
        /// </summary>
        protected EngineSeasonality _engine;
        #endregion

        #region Attributes
        /// <summary>
        /// Big Size
        /// </summary>
        protected bool _bigFormat = false;
        #endregion

        #region Accessors
        /// <summary>
        /// Get/Set Big Size option
        /// </summary>
        public bool BigFormat{
            get { return _bigFormat; }
            set { _bigFormat = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User sessions</param>
        /// <param name="dalLayer">Data Access Layer</param>
        /// <param name="bigSize">Large graph?</param>
        public ChartSeasonality(WebSession session, IProductClassIndicatorsDAL dalLayer, bool bigSize)
            : base(session, dalLayer)
        {
            _bigFormat = bigSize;
            InitEngine();
        }
        #endregion

        #region OnPrerender
        /// <summary>
        /// On Prerender
        /// </summary>
        /// <param name="e">Argument</param>
        protected override void OnPreRender(EventArgs e)
        {
            Int64 idElement = 0;
            //int cMonth = 0;
            //int oldMonth = -1;
            int nbMonth = 0;
            base.OnPreRender(e);

            #region Get Data
            object[,] tab = _engine.GetChartData();
            if (tab==null || tab.Length == 0)
            {
                this.Visible = false;
                return;
            }
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

            SetAdvertisSeries(tab, advertiserTotal, advertiserSerie,ref nbMonth);
            #endregion

            #region Chart Size
            if (!_bigFormat)
            {
                this.Width = new Unit("" + WebApplicationParameters.CustomStyles.ChartSeasonalityWidth + "px");
                this.Height = new Unit("" + WebApplicationParameters.CustomStyles.ChartSeasonalityHeight + "px");
            }
            else
            {
                this.Width = new Unit("" + WebApplicationParameters.CustomStyles.ChartSeasonalityBigWidth + "px");
                this.Height = new Unit("" + WebApplicationParameters.CustomStyles.ChartSeasonalityBigHeight + "px");
            }
            this.ChartAreas[strChartArea].BackColor = (Color)_colorConverter.ConvertFrom(_chartAreasBackColor);
            #endregion

            #region Titre
            Title title = new Title(GestionWeb.GetWebWord(1139, _session.SiteLanguage));
            title.Font = new Font("Arial", (float)14);
            this.Titles.Add(title);
            #endregion

            #region Series Design
            serieUnivers.Type = SeriesChartType.Line;
            serieUnivers.ShowLabelAsValue = true;
            serieUnivers.XValueType = Dundas.Charting.WebControl.ChartValueTypes.String;
            serieUnivers.YValueType = Dundas.Charting.WebControl.ChartValueTypes.Double;
            serieUnivers.Enabled = true;
            serieUnivers.Font = new Font("Arial", (float)8);
            serieUnivers["LabelStyle"] = "Right";
            serieUnivers.SmartLabels.Enabled = true;
            serieUnivers.SmartLabels.CalloutLineStyle = ChartDashStyle.Dot;
            serieUnivers.SmartLabels.MinMovingDistance = 3;

            serieSectorMarket.Type = SeriesChartType.Line;
            serieSectorMarket.ShowLabelAsValue = true;
            serieSectorMarket.XValueType = Dundas.Charting.WebControl.ChartValueTypes.String;
            serieSectorMarket.YValueType = Dundas.Charting.WebControl.ChartValueTypes.Double;
            serieSectorMarket.Enabled = true;
            serieSectorMarket.Font = new Font("Arial", (float)8);
            serieSectorMarket["LabelStyle"] = "Right";
            serieSectorMarket.SmartLabels.Enabled = true;
            serieSectorMarket.SmartLabels.CalloutLineStyle = ChartDashStyle.Dot;
            serieSectorMarket.SmartLabels.MinMovingDistance = 3;

            serieMediumMonth.Type = SeriesChartType.Line;
            serieMediumMonth.ShowLabelAsValue = false;
            serieMediumMonth.XValueType = Dundas.Charting.WebControl.ChartValueTypes.String;
            serieMediumMonth.YValueType = Dundas.Charting.WebControl.ChartValueTypes.Double;
            serieMediumMonth.Enabled = true;
            serieMediumMonth.Font = new Font("Arial", (float)8);
            serieMediumMonth.SmartLabels.Enabled = true;
            serieMediumMonth.SmartLabels.CalloutLineStyle = ChartDashStyle.Dot;
            serieMediumMonth.SmartLabels.MinMovingDistance = 3;
            serieMediumMonth.LabelToolTip = GestionWeb.GetWebWord(1233, _session.SiteLanguage);
            #endregion

            #region Series Data
            DateTime periodBegin = FctUtilities.Dates.GetPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType);
            double mediumMonth = 0;
            

            SetSeriesData(tab, serieUnivers, serieSectorMarket, serieMediumMonth, advertiserTotal, advertiserSerie, periodBegin, ref mediumMonth,ref nbMonth);
            #endregion

            #region Legends
            SetLegendsLabels(serieUnivers, serieSectorMarket);
            //Mois Moyen théorique
            serieMediumMonth.LegendText = string.Format("{0} {1} %", GestionWeb.GetWebWord(1233, _session.SiteLanguage), mediumMonth);

            #region size legend
            int sizeLegend = serieUnivers.LegendText.Length;
            if(sizeLegend<serieSectorMarket.LegendText.Length)
                sizeLegend = serieSectorMarket.LegendText.Length;
            if (sizeLegend < serieMediumMonth.LegendText.Length)
                sizeLegend = serieMediumMonth.LegendText.Length;

            this.Legends[strChartArea].TextWrapThreshold = sizeLegend;

            #endregion

            #endregion

            #region X axe
            this.ChartAreas[strChartArea].AxisX.LabelStyle.Enabled = true;
            this.ChartAreas[strChartArea].AxisX.LabelsAutoFit = false;
            this.ChartAreas[strChartArea].AxisX.LabelStyle.Font = new Font("Arial", (float)8);
            this.ChartAreas[strChartArea].AxisX.MajorGrid.LineWidth = 0;
            this.ChartAreas[strChartArea].AxisX.Interval = 1;
            this.ChartAreas[strChartArea].AxisX.LabelStyle.FontAngle = 35;
            #endregion

            #region Y axe
            this.ChartAreas[strChartArea].AxisY.Enabled = AxisEnabled.True;
            this.ChartAreas[strChartArea].AxisY.LabelStyle.Enabled = true;
            this.ChartAreas[strChartArea].AxisY.LabelsAutoFit = false;
            this.ChartAreas[strChartArea].AxisY.LabelStyle.Font = new Font("Arial", (float)10);
            this.ChartAreas[strChartArea].AxisY.TitleFont = new Font("Arial", (float)10);
            this.ChartAreas[strChartArea].AxisY.MajorGrid.LineWidth = 0;
            #endregion

            #region Y2 axe
            this.ChartAreas[strChartArea].AxisY2.Enabled = AxisEnabled.True;
            this.ChartAreas[strChartArea].AxisY2.LabelStyle.Enabled = true;
            this.ChartAreas[strChartArea].AxisY2.LabelsAutoFit = false;
            this.ChartAreas[strChartArea].AxisY2.LabelStyle.Font = new Font("Arial", (float)10);
            this.ChartAreas[strChartArea].AxisY2.TitleFont = new Font("Arial", (float)10);
            this.ChartAreas[strChartArea].AxisY2.Title = "" + GestionWeb.GetWebWord(1236, _session.SiteLanguage) + "";
            #endregion							

        }
        #endregion

        #region Init Engine
        /// <summary>
        /// Init Engine
        /// </summary>
        virtual protected void InitEngine() {
            _engine = new EngineSeasonality(this._session, this._dalLayer);
        }
        #endregion

        #region SetAdvertisSeries
        protected virtual void SetAdvertisSeries(object[,] tab, Dictionary<long, double> advertiserTotal, Dictionary<long, Series> advertiserSerie, ref int nbMonth)
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
        protected virtual void SetSeriesData(object[,] tab, Series serieUnivers, Series serieSectorMarket, Series serieMediumMonth, Dictionary<long, double> advertiserTotal, Dictionary<long, Series> advertiserSerie, DateTime periodBegin, ref  double mediumMonth, ref int nbMonth)
        {
            #region Series Data

            int compteur = -1;
            int oldMonth = -1;
            int cMonth = -1;
            double invest = 0;
            string month = string.Empty;           
            Int64 idElement = 0;
            CultureInfo cInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);

            if (tab != null)
            {
                // Calcul des totaux pour les annonceurs
                for (int i = 0; i < tab.GetLength(0); i++)
                {
                    cMonth = Convert.ToInt32(tab[i, EngineSeasonality.ID_MONTH_COLUMN_INDEX]);
                    if (oldMonth != cMonth)
                    {
                        compteur++;
                        oldMonth = cMonth;
                    }
                    idElement = Convert.ToInt64(tab[i, EngineSeasonality.ID_ELEMENT_COLUMN_INDEX]);
                    invest = Convert.ToDouble(tab[i, EngineSeasonality.INVEST_COLUMN_INDEX]);
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
            #endregion
        }
        #endregion

        #region SetLegendsLabels
        /// <summary>
        ///Set Legends Labels 
        /// </summary>
        /// <param name="serieUnivers">serie Univers</param>
        /// <param name="serieSectorMarket">serie Sector Market</param>
        protected virtual void SetLegendsLabels(Series serieUnivers,Series serieSectorMarket)
        {
            // Univers
            serieUnivers.LegendText = GestionWeb.GetWebWord(1188, _session.SiteLanguage);
            serieUnivers.LabelToolTip = FctUtilities.Text.SuppressAccent(GestionWeb.GetWebWord(1188, _session.SiteLanguage));
            // Market
            if (_session.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.marketTotal)
            {
                serieSectorMarket.LegendText = GestionWeb.GetWebWord(1316, _session.SiteLanguage);
                serieSectorMarket.LabelToolTip = FctUtilities.Text.SuppressAccent(GestionWeb.GetWebWord(1316, _session.SiteLanguage));

            }
            // Famille
            else
            {
                serieSectorMarket.LegendText = GestionWeb.GetWebWord(1189, _session.SiteLanguage);
                serieSectorMarket.LabelToolTip = FctUtilities.Text.SuppressAccent(GestionWeb.GetWebWord(1189, _session.SiteLanguage));
            }
        }
        #endregion
    }

}
