#region Info
/*
 * Author :     G Ragneau
 * Created on : 29/07/2008
 * History:
 *      Date - Author - Description
 *      29/07/2008 - G Ragneau - Moved from TNS.AdExpress.Web
 * 
 * 
 * Auteur: A. Obermeyer 
 * Date de création : 21/10/2004 
 * Date de modification : 21/10/2004 
 *      12/08/2005		G. Facon		Nom de fonction et suppression des propriétés
 *      24/10/2005	D. V. Mussuma	Intégration unité Keuros
 * */
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dundas.Charting.WebControl;

using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;

using TNS.AdExpress.Domain.Translation;
using TNS.AdExpressI.ProductClassIndicators.Engines;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.ProductClassIndicators.DAL;


namespace TNS.AdExpressI.ProductClassIndicators.Charts
{
    /// <summary>
    /// Product Class Indicator "Top"
    /// </summary>
    [ToolboxData("<{0}:ChartTop runat=server></{0}:ChartTop>")]
    public class ChartTop : ChartProductClassIndicator
    {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User sessions</param>
        /// <param name="dalLayer">Data Access layer</param>
        /// <param name="classifLevel">Classification Detail (product or advertiser)</param>
        public ChartTop(WebSession session, IProductClassIndicatorsDAL dalLayer, CstResult.MotherRecap.ElementType classifLevel):base(session, dalLayer)
        {
            this._classifLevel = classifLevel;
        }
        #endregion

        /// <summary>
        /// Design and build seris
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            #region Init Chart
            bool referenceElement = false;
            bool competitorElement = false;
            // There is at least one element
            bool oneProductExist = false;

            Series series = new Series("Palmares");
            this.Series.Add(series);
            ChartArea chartArea = new ChartArea();
            this.ChartAreas.Add(chartArea);
            string strChartArea = this.Series["Palmares"].ChartArea;
            #endregion

            EngineTop engine = new EngineTop(this._session, this._dalLayer);
            object[,] tab = engine.GetData(TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.typeYearSelected.currentYear, this._classifLevel); // TNS.AdExpress.Web.Rules.Results.IndicatorPalmaresRules.GetFormattedTable(_session, typeYear, tableType);

            if (tab.GetLongLength(0) == 1 || Convert.ToDouble(tab[0, EngineTop.TOTAL_N]) == 0)
            {
                this.Visible = false;
            }

            #region Chart Design
            this.Width = new Unit("750px");
            this.Height = new Unit("750px");
            this.ChartAreas[strChartArea].BackColor = (Color)_colorConverter.ConvertFrom(_chartAreasBackColor);
            this.DataManipulator.Sort(PointsSortOrder.Descending, series);
            #endregion

            #region Title
            Title title;
            if (_classifLevel == CstResult.MotherRecap.ElementType.advertiser)
            {
                title = new Title("" + GestionWeb.GetWebWord(1184, _session.SiteLanguage) + "");
            }
            else
            {
                title = new Title("" + GestionWeb.GetWebWord(1169, _session.SiteLanguage) + "");
            }
            title.Font = new Font("Arial", (float)14);
            this.Titles.Add(title);
            #endregion

            #region Series
            series.Type = SeriesChartType.Column;
            series.ShowLabelAsValue = true;
            series.XValueType = ChartValueTypes.String;
            series.YValueType = ChartValueTypes.Double;
            series.Color = (Color)_colorConverter.ConvertFrom(_seriesColor);
            series.Enabled = true;
            series.Font = new Font("Arial", (float)10);
            series.FontAngle = 45;
            #endregion

            #region Data building
            for (int i = 1; i < tab.GetLongLength(0) && i < 11; i++)
            {
                double d = Convert.ToDouble(tab[i, EngineTop.TOTAL_N]);
                string u = FctUtilities.Units.ConvertUnitValueToString(d, _session.Unit);
                if ( d != 0 && FctUtilities.CheckedText.IsNotEmpty(u) )
                {
                    oneProductExist = true;

                    series.Points.AddXY(tab[i, EngineTop.PRODUCT], Convert.ToDouble(u));

                    series.Points[i - 1].ShowInLegend = true;
                    // Competitor in red
                    if (tab[i, EngineTop.COMPETITOR] != null)
                    {
                        if ((int)tab[i, EngineTop.COMPETITOR] == 2)
                        {
                            series.Points[i - 1].Color = (Color)_colorConverter.ConvertFrom(_competitorSerieColor);
                            competitorElement = true;
                        }
                        // Reference in green
                        else if ((int)tab[i, EngineTop.COMPETITOR] == 1)
                        {
                            series.Points[i - 1].Color = (Color)_colorConverter.ConvertFrom(_referenceSerieColor);
                            referenceElement = true;
                        }
                    }
                }
            }
            if (!oneProductExist)
                this.Visible = false;
            #endregion

            #region Legend
            if (_classifLevel == CstResult.MotherRecap.ElementType.advertiser)
            {
                series.LegendText = GestionWeb.GetWebWord(1106, _session.SiteLanguage);
            }
            else
            {
                series.LegendText = GestionWeb.GetWebWord(1200, _session.SiteLanguage);
            }
            LegendItem legendItemReference = new LegendItem();
            legendItemReference.BorderWidth = 0;
            legendItemReference.Color = (Color)_colorConverter.ConvertFrom(_legendItemReferenceColor);
            if (referenceElement)
            {
                if (_classifLevel == CstResult.MotherRecap.ElementType.advertiser)
                {
                    legendItemReference.Name = GestionWeb.GetWebWord(1201, _session.SiteLanguage);
                    this.Legends["Default"].CustomItems.Add(legendItemReference);
                }
                else
                {
                    legendItemReference.Name = GestionWeb.GetWebWord(1203, _session.SiteLanguage);
                    this.Legends["Default"].CustomItems.Add(legendItemReference);

                }
            }
            LegendItem legendItemCompetitor = new LegendItem();
            legendItemCompetitor.BorderWidth = 0;
            legendItemCompetitor.Color = (Color)_colorConverter.ConvertFrom(_legendItemCompetitorColor);

            if (competitorElement)
            {
                if (_classifLevel == CstResult.MotherRecap.ElementType.advertiser)
                {
                    legendItemCompetitor.Name = GestionWeb.GetWebWord(1202, _session.SiteLanguage);
                    this.Legends["Default"].CustomItems.Add(legendItemCompetitor);
                }
                else
                {
                    legendItemCompetitor.Name = GestionWeb.GetWebWord(1204, _session.SiteLanguage);
                    this.Legends["Default"].CustomItems.Add(legendItemCompetitor);
                }
            }
            #endregion

            #region Axe X
            this.ChartAreas[strChartArea].AxisX.LabelStyle.Enabled = true;
            this.ChartAreas[strChartArea].AxisX.LabelsAutoFit = false;
            this.ChartAreas[strChartArea].AxisX.LabelStyle.Font = new Font("Arial", (float)8);
            this.ChartAreas[strChartArea].AxisX.MajorGrid.LineWidth = 0;
            this.ChartAreas[strChartArea].AxisX.Interval = 1;
            this.ChartAreas[strChartArea].AxisX.LabelStyle.FontAngle = 35;
            #endregion

            #region Axe Y
            this.ChartAreas[strChartArea].AxisY.Enabled = AxisEnabled.True;
            this.ChartAreas[strChartArea].AxisY.LabelStyle.Enabled = true;
            this.ChartAreas[strChartArea].AxisY.LabelsAutoFit = false;

            this.ChartAreas[strChartArea].AxisY.LabelStyle.Font = new Font("Arial", (float)10);
            this.ChartAreas[strChartArea].AxisY.Title = "" + GestionWeb.GetWebWord(1206, _session.SiteLanguage) + "";
            this.ChartAreas[strChartArea].AxisY.TitleFont = new Font("Arial", (float)10);
            double dd = Convert.ToDouble(tab[0, EngineTop.TOTAL_N]);
            double uu = Convert.ToDouble(FctUtilities.Units.ConvertUnitValueToString(dd, _session.Unit));
            if (uu > 0)
            {
                this.ChartAreas[strChartArea].AxisY.Maximum = uu;
            }
            else
            {
                this.ChartAreas[strChartArea].AxisY.Maximum = (double)0.0;
            }
            this.ChartAreas[strChartArea].AxisY.MajorGrid.LineWidth = 0;
            #endregion

            #region Axe Y2
            this.ChartAreas[strChartArea].AxisY2.Enabled = AxisEnabled.True;
            this.ChartAreas[strChartArea].AxisY2.LabelStyle.Enabled = true;
            this.ChartAreas[strChartArea].AxisY2.LabelsAutoFit = false;

            this.ChartAreas[strChartArea].AxisY2.LabelStyle.Font = new Font("Arial", (float)10);
            this.ChartAreas[strChartArea].AxisY2.TitleFont = new Font("Arial", (float)10);
            this.ChartAreas[strChartArea].AxisY2.Maximum = 100;
            this.ChartAreas[strChartArea].AxisY2.Title = GestionWeb.GetWebWord(1205, _session.SiteLanguage);
            #endregion	
			
			
        }

    }

}
