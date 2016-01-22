using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Anubis.Hotep.Common;
using System.Globalization;
using FctIndicators = TNS.AdExpress.Anubis.Hotep.Russia.Functions;
using FrameWorkConstantes = TNS.AdExpress.Constantes.FrameWork;
using TNS.AdExpressI.ProductClassIndicators.Russia.Engines;
using Dundas.Charting.WinControl;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;
namespace TNS.AdExpress.Anubis.Hotep.Russia.UI
{
    public  class UIPalmaresGraph : TNS.AdExpress.Anubis.Hotep.UI.UIPalmaresGraph
    {
        #region Constructeur
        public UIPalmaresGraph(WebSession webSession, IDataSource dataSource, HotepConfig config, object[,] tab, TNS.FrameWork.WebTheme.Style style)
            : base( webSession,  dataSource,  config, tab, style)
        {            
        }
        #endregion

        #region Palmares
        /// <summary>
        /// Graphiques Palmares
        /// </summary>
        public virtual void BuildPalmares(FrameWorkConstantes.Results.PalmaresRecap.ElementType tableType)
        {

            AdExpressCultureInfo fp = WebApplicationParameters.AllowedLanguages[_webSession.DataLanguage].CultureInfo;
            UnitInformation selectedCurrency = _webSession.GetSelectedUnit();

            #region Init Chart
            Color colorTemp = Color.Black;
            bool referenceElement = false;
            bool competitorElement = false;
            bool mixedElement = false;
            // There is at least one element
            bool oneProductExist = false;

            Series series = new Series("Palmares");
            this.Series.Add(series);
            ChartArea chartArea = new ChartArea();
            this.ChartAreas.Add(chartArea);
            string strChartArea = this.Series["Palmares"].ChartArea;
            #endregion

            #region Chart Design
            _style.GetTag("PalmaresGraphSize").SetStyleDundas(this);
            this.BackGradientType = GradientType.TopBottom;
            this.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);
            _style.GetTag("PalmaresGraphBackColor").SetStyleDundas(ref colorTemp);
            this.ChartAreas[strChartArea].BackColor = colorTemp;
          //  this.DataManipulator.Sort(PointsSortOrder.Descending, series);
            _style.GetTag("PalmaresGraphLineEnCircle").SetStyleDundas(this);
            #endregion

            #region Title
            Title title = null;
            String strTitle = "";
            if (tableType == FrameWorkConstantes.Results.PalmaresRecap.ElementType.advertiser)
            {
                if (_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal)
                    strTitle = GestionWeb.GetWebWord(1184, _webSession.SiteLanguage) + " (" + GestionWeb.GetWebWord(1188, _webSession.SiteLanguage) + ")";
                else if (_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal)
                    strTitle = GestionWeb.GetWebWord(1184, _webSession.SiteLanguage) + " (" + GestionWeb.GetWebWord(1189, _webSession.SiteLanguage) + ")";
                else if (_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.marketTotal)
                    strTitle = GestionWeb.GetWebWord(1184, _webSession.SiteLanguage) + " (" + GestionWeb.GetWebWord(1316, _webSession.SiteLanguage) + ")";
                title = new Title("" + strTitle + "");
            }
            else
            {
                if (_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal)
                    strTitle = GestionWeb.GetWebWord(1169, _webSession.SiteLanguage) + " (" + GestionWeb.GetWebWord(1188, _webSession.SiteLanguage) + ")";
                else if (_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal)
                    strTitle = GestionWeb.GetWebWord(1169, _webSession.SiteLanguage) + " (" + GestionWeb.GetWebWord(1189, _webSession.SiteLanguage) + ")";
                else if (_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.marketTotal)
                    strTitle = GestionWeb.GetWebWord(1169, _webSession.SiteLanguage) + " (" + GestionWeb.GetWebWord(1316, _webSession.SiteLanguage) + ")";
                title = new Title("" + strTitle + "");
            }

            this.Titles.Add(title);
            _style.GetTag("PalmaresGraphTitleFont").SetStyleDundas(this.Titles[0]);
            #endregion

            #region Series
            series.Type = SeriesChartType.Column;
            series.ShowLabelAsValue = true;
            series.XValueType = ChartValueTypes.String;
            series.YValueType = ChartValueTypes.Double;
            _style.GetTag("PalmaresGraphColorSerie").SetStyleDundas(ref colorTemp);
            series.Color = colorTemp;
            series.Enabled = true;
            _style.GetTag("PalmaresGraphTitleFontSerie").SetStyleDundas(series);
            series.FontAngle = 45;
            #endregion

            #region Data building
            DataBuilding(series, fp, ref  oneProductExist, ref  mixedElement, ref  competitorElement, ref  referenceElement, ref  colorTemp);
            #endregion

            #region Légendes
            if (tableType == FrameWorkConstantes.Results.PalmaresRecap.ElementType.advertiser)
            {
                series.LegendText = "" + GestionWeb.GetWebWord(1106, _webSession.SiteLanguage) + "";
            }
            else
            {
                series.LegendText = "" + GestionWeb.GetWebWord(1200, _webSession.SiteLanguage) + "";
            }
            LegendItem legendItemReference = new LegendItem();
            legendItemReference.BorderWidth = 0;
            _style.GetTag("PalmaresGraphColorLegendItemReference").SetStyleDundas(ref colorTemp);
            legendItemReference.Color = colorTemp;
            if (referenceElement)
            {
                if (tableType == FrameWorkConstantes.Results.PalmaresRecap.ElementType.advertiser)
                {
                    legendItemReference.Name = GestionWeb.GetWebWord(1201, _webSession.SiteLanguage);
                    this.Legends["Default"].CustomItems.Add(legendItemReference);
                }
                else
                {
                    legendItemReference.Name = GestionWeb.GetWebWord(1203, _webSession.SiteLanguage);
                    this.Legends["Default"].CustomItems.Add(legendItemReference);

                }
            }
            LegendItem legendItemCompetitor = new LegendItem();
            legendItemCompetitor.BorderWidth = 0;
            _style.GetTag("PalmaresGraphColorLegendItemCompetitor").SetStyleDundas(ref colorTemp);
            legendItemCompetitor.Color = colorTemp;

            if (competitorElement)
            {
                if (tableType == FrameWorkConstantes.Results.PalmaresRecap.ElementType.advertiser)
                {
                    legendItemCompetitor.Name = GestionWeb.GetWebWord(1202, _webSession.SiteLanguage);
                    this.Legends["Default"].CustomItems.Add(legendItemCompetitor);
                }
                else
                {
                    legendItemCompetitor.Name = GestionWeb.GetWebWord(1204, _webSession.SiteLanguage);
                    this.Legends["Default"].CustomItems.Add(legendItemCompetitor);
                }
            }
            if (mixedElement)
            {
                LegendItem legendItemMixed = new LegendItem();
                legendItemMixed.BorderWidth = 0;
                _style.GetTag("PalmaresGraphColorLegendItemMixed").SetStyleDundas(ref colorTemp);
                legendItemMixed.Color = colorTemp;
                legendItemMixed.Name = GestionWeb.GetWebWord(2561, _webSession.SiteLanguage);
                this.Legends["Default"].CustomItems.Add(legendItemMixed);
            }
            #endregion

            #region Axe des X
            this.ChartAreas[strChartArea].AxisX.LabelStyle.Enabled = true;
            this.ChartAreas[strChartArea].AxisX.LabelsAutoFit = false;
            _style.GetTag("PalmaresGraphLabelFontAxisX").SetStyleDundas(this.ChartAreas[strChartArea].AxisX.LabelStyle);
            this.ChartAreas[strChartArea].AxisX.MajorGrid.LineWidth = 0;
            this.ChartAreas[strChartArea].AxisX.Interval = 1;
            this.ChartAreas[strChartArea].AxisX.LabelStyle.FontAngle = 35;
            #endregion

            #region Axe des Y
            SetAxeY(selectedCurrency, strChartArea, fp);
            #endregion

          

        }
        #endregion

        /// <summary>
        /// Dat building
        /// </summary>     
        /// <param name="series">series</param>
        /// <param name="fp">IFormatProvider</param>
        /// <param name="oneProductExist">test if one Product Exist</param>
        /// <param name="mixedElement">test if mixe dElement</param>
        /// <param name="competitorElement">test if competitor Element</param>
        /// <param name="referenceElement">test if reference Element</param>
        protected override void DataBuilding(Series series, AdExpressCultureInfo fp, ref bool oneProductExist, ref bool mixedElement, ref bool competitorElement, ref bool referenceElement, ref Color colorTemp)
        {
          
            for (int i = 1; i < _tab.GetLongLength(0) && i < 11; i++)
            {
                if (!FctIndicators.IsNull(_tab[i, EngineTop.TOTAL_N]))
                {
                    string u = FctIndicators.ConvertUnitValueToString(_tab[i, EngineTop.TOTAL_N], _webSession.Unit, fp);
                    if (!string.IsNullOrEmpty(u))
                    {
                        oneProductExist = true;

                        series.Points.AddXY(_tab[i, EngineTop.PRODUCT], FctIndicators.ConvertToDouble(u.Trim(), fp));

                        series.Points[i - 1].ShowInLegend = true;

                        if (!FctIndicators.IsNull(_tab[i, EngineTop.COMPETITOR]))
                        {
                            // Mixed in yellow
                            if ((int)_tab[i, EngineTop.COMPETITOR] == 3)
                            {
                                _style.GetTag("PalmaresGraphColorLegendItemMixed").SetStyleDundas(ref colorTemp);
                                series.Points[i - 1].Color = colorTemp;
                                mixedElement = true;
                            }
                            // Competitor in red
                            else if ((int)_tab[i, EngineTop.COMPETITOR] == 2)
                            {
                                _style.GetTag("PalmaresGraphColorLegendItemCompetitor").SetStyleDundas(ref colorTemp);
                                series.Points[i - 1].Color = colorTemp;
                                competitorElement = true;
                            }
                            // Reference in green
                            else if ((int)_tab[i, EngineTop.COMPETITOR] == 1)
                            {
                                _style.GetTag("PalmaresGraphColorLegendItemReference").SetStyleDundas(ref colorTemp);
                                series.Points[i - 1].Color = colorTemp;
                                referenceElement = true;
                            }
                        }

                    }
                }
            }
            if (!oneProductExist)
                this.Visible = false;
        }


        /// <summary>
        /// Set Axe Y
        /// </summary>        
        /// <param name="selectedCurrency">selected Currency</param>
        /// <param name="strChartArea">string Chart Area</param>
        protected override void SetAxeY(UnitInformation selectedCurrency, string strChartArea, IFormatProvider fp)
        {
            this.ChartAreas[strChartArea].AxisY.Enabled = AxisEnabled.True;
            this.ChartAreas[strChartArea].AxisY.LabelStyle.Enabled = true;
            this.ChartAreas[strChartArea].AxisY.LabelsAutoFit = false;

            _style.GetTag("PalmaresGraphLabelFontAxisY").SetStyleDundas(this.ChartAreas[strChartArea].AxisY.LabelStyle);
            this.ChartAreas[strChartArea].AxisY.Title = "" + GestionWeb.GetWebWord(1246, _webSession.SiteLanguage) + " (" + selectedCurrency.GetUnitSignWebText(_webSession.DataLanguage) + ")";
            _style.GetTag("PalmaresGraphTitleFontAxisY").SetStyleDundas(this.ChartAreas[strChartArea].AxisY);

            if (!FctIndicators.IsNull(_tab[0, EngineTop.TOTAL_N]))
            {
                string u = FctIndicators.ConvertUnitValueToString(_tab[0, EngineTop.TOTAL_N], _webSession.Unit, fp);
                if (!string.IsNullOrEmpty(u))
                {

                    double uu = Convert.ToDouble(u.Trim(), fp);
                    if (uu > 0)
                    {
                        this.ChartAreas[strChartArea].AxisY.Maximum = uu;
                        this.ChartAreas[strChartArea].AxisY2.Maximum = uu;
                    }
                    else
                    {
                        this.ChartAreas[strChartArea].AxisY.Maximum = (double)0.0;
                        this.ChartAreas[strChartArea].AxisY2.Maximum = (double)0.0;
                    }
                }
                else
                {
                    this.ChartAreas[strChartArea].AxisY.Maximum = (double)0.0;
                    this.ChartAreas[strChartArea].AxisY2.Maximum = (double)0.0;
                }
            }
            else
            {
                this.ChartAreas[strChartArea].AxisY.Maximum = (double)0.0;
                this.ChartAreas[strChartArea].AxisY2.Maximum = (double)0.0;
            }
            this.ChartAreas[strChartArea].AxisY.MajorGrid.LineWidth = 0;

            #region Axe des Y2
            this.ChartAreas[strChartArea].AxisY2.Enabled = AxisEnabled.True;
            this.ChartAreas[strChartArea].AxisY2.LabelsAutoFit = false;
            _style.GetTag("PalmaresGraphLabelFontAxisY").SetStyleDundas(this.ChartAreas[strChartArea].AxisY2.LabelStyle);
            _style.GetTag("PalmaresGraphTitleFontAxisY").SetStyleDundas(this.ChartAreas[strChartArea].AxisY2);

            #endregion
        }

    }
}
