#region Info
/*
 * Author :     G Ragneau
 * Created on : 07/08/2008
 * History:
 *      Date - Author - Description
 *      29/07/2008 - G Ragneau - Moved from TNS.AdExpress.Web
 * 
 * 
 * */
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;

using Dundas.Charting.WebControl;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.ProductClassIndicators.DAL;
using System.Drawing;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpressI.ProductClassIndicators.Engines;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpressI.ProductClassIndicators.Charts
{

    [ToolboxData("<{0}:ChartTop runat=server></{0}:ChartTop>")]
    public class ChartEvolution : ChartProductClassIndicator
    {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User sessions</param>
        /// <param name="dalLayer">Data Access Layer</param>
        public ChartEvolution(WebSession session, IProductClassIndicatorsDAL dalLayer, CstResult.MotherRecap.ElementType classifLevel)
            : base(session, dalLayer)
        {
            this._classifLevel = classifLevel;
        }
        #endregion

        #region OnPreRender
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;

            #region Series Init
            Series series = new Series("Evolution");
            ChartArea chartArea = new ChartArea();
            this.Series.Add(series);
            this.ChartAreas.Add(chartArea);
            string strChartArea = this.Series["Evolution"].ChartArea;
            #endregion

            #region Get Data
            EngineEvolution engine = new EngineEvolution(this._session, this._dalLayer);
            object[,] tab = engine.GetData(this._classifLevel); 
            long last = tab.GetLongLength(0) - 1;

            if (tab.GetLongLength(0) == 0)
            {
                this.Visible = false;
                return;
            }
            #endregion

            #region Chart Design
            this.Width = new Unit("850px");
            this.Height = new Unit("500px");
            this.ChartAreas[strChartArea].BackColor = (Color)_colorConverter.ConvertFrom(_chartAreasBackColor);
            #endregion

            #region Titre
            Title title;
            if (_classifLevel == CstResult.MotherRecap.ElementType.advertiser)
            {
                title = new Title(GestionWeb.GetWebWord(1215, _session.SiteLanguage));
            }
            else
            {
                title = new Title(GestionWeb.GetWebWord(1216, _session.SiteLanguage));
            }
            title.Font = new Font("Arial", (float)14);
            this.Titles.Add(title);
            #endregion

            #region Series
            series.Type = SeriesChartType.Column;
            series.ShowLabelAsValue = true;
            series.XValueType = Dundas.Charting.WebControl.ChartValueTypes.String;
            series.YValueType = Dundas.Charting.WebControl.ChartValueTypes.Double;
            series.Color = (Color)_colorConverter.ConvertFrom(_seriesColor);
            series.Enabled = true;
            series.Font = new Font("Arial", (float)10);
            series.FontAngle = 90;
            #endregion

            #region Series building
            double ecart = 0;
            string sEcart = string.Empty;
            int typeElt = 0;
            int compteur = 0;
            bool hasComp = false;
            bool hasRef = false;
            for (int i = 0; i < tab.GetLongLength(0) && i < 10; i++)
            {
                ecart = Convert.ToDouble(tab[i, EngineEvolution.ECART]);
                sEcart = FctUtilities.Units.ConvertUnitValueToString(ecart, _session.Unit, fp).Trim();
                if (ecart > 0)
                {
                    series.Points.AddXY(tab[i, EngineEvolution.PRODUCT].ToString(), Convert.ToDouble(sEcart.Replace(" ", string.Empty), fp));
                    series.Points[compteur].ShowInLegend = true;

                    #region Reference or competitor ?
                    if (tab[i, EngineEvolution.COMPETITOR] != null)
                    {
                        typeElt = Convert.ToInt32(tab[i, EngineEvolution.COMPETITOR]);
                        if (typeElt == 2)
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
                ecart = Convert.ToDouble(tab[last, EngineEvolution.ECART]);
                sEcart = FctUtilities.Units.ConvertUnitValueToString(ecart, _session.Unit, fp).Trim();

                if (ecart < 0)
                {
                    series.Points.AddXY(tab[last, EngineEvolution.PRODUCT].ToString(), Convert.ToDouble(sEcart.Replace(" ", string.Empty), fp));
                    series.Points[compteur].ShowInLegend = true;
                    series.Points[compteur].CustomAttributes = "LabelStyle=top";

                    #region Reference or competitor ?
                    if (tab[last, EngineEvolution.COMPETITOR] != null)
                    {
                        typeElt = Convert.ToInt32(tab[last, EngineEvolution.COMPETITOR]);
                        if (typeElt == 2)
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

                last--;

            }
            #endregion

            #region Legends
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
            if (hasRef)
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

            if (hasComp)
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

            this.DataManipulator.Sort(PointsSortOrder.Descending, series);

            #region Axe des X
            this.ChartAreas[strChartArea].AxisX.LabelStyle.Enabled = true;
            this.ChartAreas[strChartArea].AxisX.LabelsAutoFit = false;
            this.ChartAreas[strChartArea].AxisX.LabelStyle.Font = new Font("Arial", (float)8);
            this.ChartAreas[strChartArea].AxisX.MajorGrid.LineWidth = 0;
            this.ChartAreas[strChartArea].AxisX.Interval = 1;
            this.ChartAreas[strChartArea].AxisX.LabelStyle.FontAngle = 35;
            #endregion

            #region Axe des Y
            this.ChartAreas[strChartArea].AxisY.Enabled = AxisEnabled.True;
            this.ChartAreas[strChartArea].AxisY.LabelStyle.Enabled = true;
            this.ChartAreas[strChartArea].AxisY.LabelsAutoFit = false;
            this.ChartAreas[strChartArea].AxisY.LabelStyle.Font = new Font("Arial", (float)10);
            this.ChartAreas[strChartArea].AxisY.TitleFont = new Font("Arial", (float)10);
            this.ChartAreas[strChartArea].AxisY.MajorGrid.LineWidth = 0;
            #endregion

            #region Axe des Y2
            this.ChartAreas[strChartArea].AxisY2.Enabled = AxisEnabled.True;
            this.ChartAreas[strChartArea].AxisY2.LabelStyle.Enabled = true;
            this.ChartAreas[strChartArea].AxisY2.LabelsAutoFit = false;
            this.ChartAreas[strChartArea].AxisY2.LabelStyle.Font = new Font("Arial", (float)10);
            this.ChartAreas[strChartArea].AxisY2.TitleFont = new Font("Arial", (float)10);
            this.ChartAreas[strChartArea].AxisY2.Title = GestionWeb.GetWebWord(1217, _session.SiteLanguage);

            #endregion					
		
        }
        #endregion

    }

}
