#region Information
//Author : Y. Rkaina 
//Creation : 17/07/2006
#endregion

using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Collections;

//using TNS.AdExpress.Common;

using TNS.AdExpress.Constantes.Web;
using CstUI = TNS.AdExpress.Constantes.Web.UI;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;

using TNS.AdExpress.Anubis.Hotep.Common;
using TNS.AdExpress.Anubis.Hotep.Exceptions;

using Dundas.Charting.WinControl;
using TNS.FrameWork.DB.Common;
using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.FrameWork;
using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpressI.ProductClassIndicators.Engines;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Anubis.Hotep.UI
{
	/// <summary>
	/// Description résumée de UIPalmaresGraph.
	/// </summary>
	public class UIPalmaresGraph : Chart
	{
		
		#region Attributes
		/// <summary>
		/// User Session
		/// </summary>
		private WebSession _webSession = null;
		/// <summary>
		/// Data Source
		/// </summary>
		private IDataSource _dataSource = null;
		/// <summary>
		/// Hotep configuration
		/// </summary>
		private HotepConfig _config = null;
		/// <summary>
		/// Tableau d'objets qui contient les résultats
		/// </summary>
		private object[,] _tab=null;
        /// <summary>
        /// Style
        /// </summary>
        private TNS.AdExpress.Domain.Theme.Style _style = null;
		#endregion

		#region Constructeur
        public UIPalmaresGraph(WebSession webSession, IDataSource dataSource, HotepConfig config, object[,] tab, TNS.AdExpress.Domain.Theme.Style style)
            : base() {
            _webSession = webSession;
            _dataSource = dataSource;
            _config = config;
            _tab = tab;
            _style = style;
        }
		#endregion
		
		#region Palmares
		/// <summary>
		/// Graphiques Palmares
		/// </summary>
		internal void BuildPalmares(FrameWorkConstantes.Results.PalmaresRecap.ElementType tableType) {

            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].CultureInfo;

            #region Init Chart
            Color colorTemp = Color.Black;
            bool referenceElement = false;
            bool competitorElement = false;
            // There is at least one element
            bool oneProductExist = false;

			Series series = new Series("Palmares");
            this.Series.Add(series);
			ChartArea chartArea=new ChartArea();
            this.ChartAreas.Add(chartArea);
            string strChartArea = this.Series["Palmares"].ChartArea;
			#endregion

            #region Chart Design
            _style.GetTag("PalmaresGraphSize").SetStyleDundas(this);
			this.BackGradientType = GradientType.TopBottom;
			this.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);
            _style.GetTag("PalmaresGraphBackColor").SetStyleDundas(ref colorTemp);
            this.ChartAreas[strChartArea].BackColor = colorTemp;
			this.DataManipulator.Sort(PointsSortOrder.Descending,series);
            _style.GetTag("PalmaresGraphLineEnCircle").SetStyleDundas(this);
			#endregion

            #region Title
            Title title=null;
			String strTitle="";
			if(tableType==FrameWorkConstantes.Results.PalmaresRecap.ElementType.advertiser){
				//title = new Title(""+GestionWeb.GetWebWord(1184,_webSession.SiteLanguage)+"");
				if(_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal)
					strTitle = GestionWeb.GetWebWord(1184,_webSession.SiteLanguage)+ " (" + GestionWeb.GetWebWord(1188,_webSession.SiteLanguage) + ")";
				else if(_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal)
						 strTitle = GestionWeb.GetWebWord(1184,_webSession.SiteLanguage) + " (" + GestionWeb.GetWebWord(1189,_webSession.SiteLanguage)+ ")";
					 else if(_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.marketTotal)
							strTitle = GestionWeb.GetWebWord(1184,_webSession.SiteLanguage) + " (" + GestionWeb.GetWebWord(1316,_webSession.SiteLanguage)+ ")";
				title = new Title(""+strTitle+"");
			}
			else{
				//title = new Title(""+GestionWeb.GetWebWord(1169,_webSession.SiteLanguage)+"");
				if(_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal)
					strTitle = GestionWeb.GetWebWord(1169,_webSession.SiteLanguage)+ " (" + GestionWeb.GetWebWord(1188,_webSession.SiteLanguage) + ")";
				else if(_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal)
					strTitle = GestionWeb.GetWebWord(1169,_webSession.SiteLanguage) + " (" + GestionWeb.GetWebWord(1189,_webSession.SiteLanguage)+ ")";
					else if(_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.marketTotal)
							strTitle = GestionWeb.GetWebWord(1169,_webSession.SiteLanguage) + " (" + GestionWeb.GetWebWord(1316,_webSession.SiteLanguage)+ ")";
				title = new Title(""+strTitle+"");
			}
            
			this.Titles.Add(title);
            _style.GetTag("PalmaresGraphTitleFont").SetStyleDundas(this.Titles[0]);
			#endregion	

			#region Series
			series.Type = SeriesChartType.Column;
			series.ShowLabelAsValue=true;
			series.XValueType=ChartValueTypes.String;
			series.YValueType=ChartValueTypes.Double;
            _style.GetTag("PalmaresGraphColorSerie").SetStyleDundas(ref colorTemp);
            series.Color = colorTemp;
			series.Enabled=true;
            _style.GetTag("PalmaresGraphTitleFontSerie").SetStyleDundas(series);
			series.FontAngle=45;
			#endregion			

            #region Data building
            for (int i = 1; i < _tab.GetLongLength(0) && i < 11; i++) {
                double d = Convert.ToDouble(_tab[i, EngineTop.TOTAL_N]);
                string u = FctUtilities.Units.ConvertUnitValueToString(d, _webSession.Unit, fp);
                if (d != 0 && FctUtilities.CheckedText.IsNotEmpty(u)) {
                    oneProductExist = true;

                    series.Points.AddXY(_tab[i, EngineTop.PRODUCT].ToString(), Math.Round(FctUtilities.Units.ConvertUnitValue(d, _webSession.Unit)));

                    series.Points[i - 1].ShowInLegend = true;
                    // Competitor in red
                    if (_tab[i, EngineTop.COMPETITOR] != null) {
                        if ((int)_tab[i, EngineTop.COMPETITOR] == 2) {
                            _style.GetTag("PalmaresGraphColorLegendItemCompetitor").SetStyleDundas(ref colorTemp);
                            series.Points[i - 1].Color = colorTemp;
                            competitorElement = true;
                        }
                        // Reference in green
                        else if ((int)_tab[i, EngineTop.COMPETITOR] == 1) {
                            _style.GetTag("PalmaresGraphColorLegendItemReference").SetStyleDundas(ref colorTemp);
                            series.Points[i - 1].Color = colorTemp;	
                            referenceElement = true;
                        }
                    }
                }
            }
            if (!oneProductExist)
                this.Visible = false;
            #endregion

			#region Légendes
			if(tableType==FrameWorkConstantes.Results.PalmaresRecap.ElementType.advertiser){
				series.LegendText=""+GestionWeb.GetWebWord(1106,_webSession.SiteLanguage)+"";
			}
			else{
				series.LegendText=""+GestionWeb.GetWebWord(1200,_webSession.SiteLanguage)+"";
			}
			LegendItem legendItemReference = new LegendItem();			
			legendItemReference.BorderWidth=0;
            _style.GetTag("PalmaresGraphColorLegendItemReference").SetStyleDundas(ref colorTemp);
            legendItemReference.Color = colorTemp;
			if(referenceElement){
				if(tableType==FrameWorkConstantes.Results.PalmaresRecap.ElementType.advertiser){
					legendItemReference.Name=GestionWeb.GetWebWord(1201,_webSession.SiteLanguage);
					this.Legends["Default"].CustomItems.Add(legendItemReference);
				}
				else{					
					legendItemReference.Name=GestionWeb.GetWebWord(1203,_webSession.SiteLanguage);
					this.Legends["Default"].CustomItems.Add(legendItemReference);
					
				}
			}			
			LegendItem legendItemCompetitor = new LegendItem();
			legendItemCompetitor.BorderWidth=0;
            _style.GetTag("PalmaresGraphColorLegendItemCompetitor").SetStyleDundas(ref colorTemp);
            legendItemCompetitor.Color = colorTemp;

			if(competitorElement){
				if(tableType==FrameWorkConstantes.Results.PalmaresRecap.ElementType.advertiser){
					legendItemCompetitor.Name=GestionWeb.GetWebWord(1202,_webSession.SiteLanguage);
					this.Legends["Default"].CustomItems.Add(legendItemCompetitor);
				}
				else{
					legendItemCompetitor.Name=GestionWeb.GetWebWord(1204,_webSession.SiteLanguage);
					this.Legends["Default"].CustomItems.Add(legendItemCompetitor);
				}
			}
			#endregion

			#region Axe des X
			this.ChartAreas[strChartArea].AxisX.LabelStyle.Enabled = true;
			this.ChartAreas[strChartArea].AxisX.LabelsAutoFit = false;
            _style.GetTag("PalmaresGraphLabelFontAxisX").SetStyleDundas(this.ChartAreas[strChartArea].AxisX.LabelStyle);
			this.ChartAreas[strChartArea].AxisX.MajorGrid.LineWidth=0;
			this.ChartAreas[strChartArea].AxisX.Interval=1;				
			this.ChartAreas[strChartArea].AxisX.LabelStyle.FontAngle = 35;
			#endregion

			#region Axe des Y
			this.ChartAreas[strChartArea].AxisY.Enabled=AxisEnabled.True;
			this.ChartAreas[strChartArea].AxisY.LabelStyle.Enabled=true;
			this.ChartAreas[strChartArea].AxisY.LabelsAutoFit=false;
            _style.GetTag("PalmaresGraphLabelFontAxisY").SetStyleDundas(this.ChartAreas[strChartArea].AxisY.LabelStyle);
			this.ChartAreas[strChartArea].AxisY.Title=""+GestionWeb.GetWebWord(1206,_webSession.SiteLanguage)+"";
            _style.GetTag("PalmaresGraphTitleFontAxisY").SetStyleDundas(this.ChartAreas[strChartArea].AxisY);
            double dd = Convert.ToDouble(_tab[0, EngineTop.TOTAL_N]);
            double uu = FctUtilities.Units.ConvertUnitValue(dd, _webSession.Unit);
            if (uu > 0)
                this.ChartAreas[strChartArea].AxisY.Maximum = uu;
            else
                this.ChartAreas[strChartArea].AxisY.Maximum = (double)0.0;
			this.ChartAreas[strChartArea].AxisY.MajorGrid.LineWidth=0;
			#endregion

			#region Axe des Y2
			this.ChartAreas[strChartArea].AxisY2.Enabled=AxisEnabled.True;
			this.ChartAreas[strChartArea].AxisY2.LabelStyle.Enabled=true;
			this.ChartAreas[strChartArea].AxisY2.LabelsAutoFit=false;
            _style.GetTag("PalmaresGraphLabelFontAxisY2").SetStyleDundas(this.ChartAreas[strChartArea].AxisY2.LabelStyle);
            _style.GetTag("PalmaresGraphTitleFontAxisY2").SetStyleDundas(this.ChartAreas[strChartArea].AxisY2);
			this.ChartAreas[strChartArea].AxisY2.Maximum=100;
			this.ChartAreas[strChartArea].AxisY2.Title=""+GestionWeb.GetWebWord(1205,_webSession.SiteLanguage)+"";
			#endregion					

		}
		#endregion

	}
}
