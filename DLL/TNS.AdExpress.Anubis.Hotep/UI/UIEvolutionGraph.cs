#region Information
//Author : Y. Rkaina 
//Creation : 18/07/2006
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
using TNS.AdExpressI.ProductClassIndicators.Engines;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Anubis.Hotep.UI
{
	/// <summary>
	/// Description résumée de UIEvolutionGraph.
	/// </summary>
	public class UIEvolutionGraph : Chart{
		
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
        public UIEvolutionGraph(WebSession webSession, IDataSource dataSource, HotepConfig config, object[,] tab, TNS.AdExpress.Domain.Theme.Style style)
            : base() {
            _webSession = webSession;
            _dataSource = dataSource;
            _config = config;
            _tab = tab;
            _style = style;
        }
		#endregion
		
		#region Evolution
		/// <summary>
		/// Graphiques Evolution
		/// </summary>
		internal void BuildEvolution(FrameWorkConstantes.Results.EvolutionRecap.ElementType tableType) {

            #region Series Init
            Color colorTemp = Color.Black;
            Series series = new Series("Evolution");
            this.Series.Add(series);
			ChartArea chartArea=new ChartArea();
            this.ChartAreas.Add(chartArea);
            string strChartArea = this.Series["Evolution"].ChartArea;
			#endregion

            long last = _tab.GetLongLength(0) - 1;

            #region Chart Design
            _style.GetTag("EvolutionGraphSize").SetStyleDundas(this);
			this.BackGradientType = GradientType.TopBottom;
			this.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);
            _style.GetTag("EvolutionGraphBackColor").SetStyleDundas(ref colorTemp);
            this.ChartAreas[strChartArea].BackColor = colorTemp;
            _style.GetTag("EvolutionGraphLineEnCircle").SetStyleDundas(this);
			#endregion

			#region Titre
			Title title;
			if(tableType==FrameWorkConstantes.Results.EvolutionRecap.ElementType.advertiser){
				title = new Title(GestionWeb.GetWebWord(1215,_webSession.SiteLanguage));
			}
			else{
				title = new Title(GestionWeb.GetWebWord(1216,_webSession.SiteLanguage));
			}
			this.Titles.Add(title);
            _style.GetTag("EvolutionGraphTitleFont").SetStyleDundas(this.Titles[0]);
			#endregion	

			#region Series
			series.Type = SeriesChartType.Column;
			series.ShowLabelAsValue=true;
			series.XValueType=ChartValueTypes.String;
			series.YValueType=ChartValueTypes.Double;
            _style.GetTag("EvolutionGraphColorSerie").SetStyleDundas(ref colorTemp);
            series.Color = colorTemp;
			series.Enabled=true;
            _style.GetTag("EvolutionGraphTitleFontSerie").SetStyleDundas(series);
			series.FontAngle=90;
			series["LabelStyle"] = "TOP";
			#endregion			

            #region Series building
            double ecart = 0;
            int typeElt = 0;
            int compteur = 0;
            bool hasComp = false;
            bool hasRef = false;
            for (int i = 0; i < _tab.GetLongLength(0) && i < 10; i++) {
                ecart = Convert.ToDouble(_tab[i, EngineEvolution.ECART]);
                if (ecart > 0) {
                    series.Points.AddXY(_tab[i, EngineEvolution.PRODUCT].ToString(), Math.Round(FctUtilities.Units.ConvertUnitValue(ecart, _webSession.Unit)));
                    series.Points[compteur].ShowInLegend = true;

                    #region Reference or competitor ?
                    if (_tab[i, EngineEvolution.COMPETITOR] != null) {
                        typeElt = Convert.ToInt32(_tab[i, EngineEvolution.COMPETITOR]);
                        if (typeElt == 2) {
                            _style.GetTag("EvolutionGraphColorLegendItemCompetitor").SetStyleDundas(ref colorTemp);
                            series.Points[compteur].Color = colorTemp;
                            hasComp = true;
                        }
                        else if (typeElt == 1) {
                            _style.GetTag("EvolutionGraphColorLegendItemReference").SetStyleDundas(ref colorTemp);
                            series.Points[compteur].Color = colorTemp;
                            hasRef = true;
                        }
                    }
                    #endregion

                    compteur++;
                }
                ecart = Convert.ToDouble(_tab[last, EngineEvolution.ECART]);
                if (ecart < 0) {
                    series.Points.AddXY(_tab[last, EngineEvolution.PRODUCT].ToString(), Math.Round(FctUtilities.Units.ConvertUnitValue(ecart, _webSession.Unit)));
                    series.Points[compteur].ShowInLegend = true;
                    series.Points[compteur].CustomAttributes = "LabelStyle=top";

                    #region Reference or competitor ?
                    if (_tab[last, EngineEvolution.COMPETITOR] != null) {
                        typeElt = Convert.ToInt32(_tab[last, EngineEvolution.COMPETITOR]);
                        if (typeElt == 2) {
                            _style.GetTag("EvolutionGraphColorLegendItemCompetitor").SetStyleDundas(ref colorTemp);
                            series.Points[compteur].Color = colorTemp;
                            hasComp = true;
                        }
                        else if (typeElt == 1) {
                            _style.GetTag("EvolutionGraphColorLegendItemReference").SetStyleDundas(ref colorTemp);
                            series.Points[compteur].Color = colorTemp;
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
            if (tableType==FrameWorkConstantes.Results.EvolutionRecap.ElementType.advertiser){
				series.LegendText=""+GestionWeb.GetWebWord(1106,_webSession.SiteLanguage)+"";
			}
			else{
				series.LegendText=""+GestionWeb.GetWebWord(1200,_webSession.SiteLanguage)+"";
			}
			LegendItem legendItemReference = new LegendItem();			
			legendItemReference.BorderWidth=0;
            _style.GetTag("EvolutionGraphColorLegendItemReference").SetStyleDundas(ref colorTemp);
            legendItemReference.Color = colorTemp;
            if (hasRef) {
				if(tableType==FrameWorkConstantes.Results.EvolutionRecap.ElementType.advertiser){
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
            _style.GetTag("EvolutionGraphColorLegendItemCompetitor").SetStyleDundas(ref colorTemp);
            legendItemCompetitor.Color = colorTemp;

            if (hasComp) {
				if(tableType==FrameWorkConstantes.Results.EvolutionRecap.ElementType.advertiser){
					legendItemCompetitor.Name=GestionWeb.GetWebWord(1202,_webSession.SiteLanguage);
					this.Legends["Default"].CustomItems.Add(legendItemCompetitor);
				}
				else{
					legendItemCompetitor.Name=GestionWeb.GetWebWord(1204,_webSession.SiteLanguage);
					this.Legends["Default"].CustomItems.Add(legendItemCompetitor);
				}
			}
			#endregion
			
			this.DataManipulator.Sort(PointsSortOrder.Descending,series);
			
			#region Axe des X
			this.ChartAreas[strChartArea].AxisX.LabelStyle.Enabled = true;
			this.ChartAreas[strChartArea].AxisX.LabelsAutoFit = false;
            _style.GetTag("EvolutionGraphLabelFontAxisX").SetStyleDundas(this.ChartAreas[strChartArea].AxisX.LabelStyle);
			this.ChartAreas[strChartArea].AxisX.MajorGrid.LineWidth=0;
			this.ChartAreas[strChartArea].AxisX.Interval=1;				
			this.ChartAreas[strChartArea].AxisX.LabelStyle.FontAngle = 35;			
			#endregion

			#region Axe des Y
			this.ChartAreas[strChartArea].AxisY.Enabled=AxisEnabled.True;
			this.ChartAreas[strChartArea].AxisY.LabelStyle.Enabled=true;
			this.ChartAreas[strChartArea].AxisY.LabelsAutoFit=false;
            _style.GetTag("EvolutionGraphLabelFontAxisY").SetStyleDundas(this.ChartAreas[strChartArea].AxisY.LabelStyle);
            _style.GetTag("EvolutionGraphTitleFontAxisY").SetStyleDundas(this.ChartAreas[strChartArea].AxisY);
			this.ChartAreas[strChartArea].AxisY.MajorGrid.LineWidth=0;
			#endregion

			#region Axe des Y2
			this.ChartAreas[strChartArea].AxisY2.Enabled=AxisEnabled.True;
			this.ChartAreas[strChartArea].AxisY2.LabelStyle.Enabled=true;
			this.ChartAreas[strChartArea].AxisY2.LabelsAutoFit=false;
            _style.GetTag("EvolutionGraphLabelFontAxisY2").SetStyleDundas(this.ChartAreas[strChartArea].AxisY2.LabelStyle);
            _style.GetTag("EvolutionGraphTitleFontAxisY2").SetStyleDundas(this.ChartAreas[strChartArea].AxisY2);
			this.ChartAreas[strChartArea].AxisY2.Title=""+GestionWeb.GetWebWord(1217,_webSession.SiteLanguage)+"";
			#endregion					

		}
		#endregion
		


		
	}
}
