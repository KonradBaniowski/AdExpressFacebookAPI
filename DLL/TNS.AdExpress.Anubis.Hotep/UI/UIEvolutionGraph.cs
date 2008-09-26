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
		
		#endregion
		
		#region Constructeur
		public UIEvolutionGraph(WebSession webSession,IDataSource dataSource, HotepConfig config,object[,] tab):base(){
		_webSession = webSession;
		_dataSource = dataSource;
		_config = config;
		_tab = tab;
		}
		#endregion
		
		#region Evolution
		/// <summary>
		/// Graphiques Evolution
		/// </summary>
		internal void BuildEvolution(FrameWorkConstantes.Results.EvolutionRecap.ElementType tableType) {

            #region Series Init
            Series series = new Series("Evolution");
            this.Series.Add(series);
			ChartArea chartArea=new ChartArea();
            this.ChartAreas.Add(chartArea);
            string strChartArea = this.Series["Evolution"].ChartArea;
			#endregion

            long last = _tab.GetLongLength(0) - 1;

            #region Chart Design
            this.Size = new Size(800,500);
			this.BackGradientType = GradientType.TopBottom;
			this.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);
			this.ChartAreas[strChartArea].BackColor=Color.FromArgb(222,207,231);			
			this.BorderStyle=ChartDashStyle.Solid;
			this.BorderLineColor=Color.FromArgb(99,73,132);
			this.BorderLineWidth=2;
			#endregion

			#region Titre
			Title title;
			if(tableType==FrameWorkConstantes.Results.EvolutionRecap.ElementType.advertiser){
				title = new Title(GestionWeb.GetWebWord(1215,_webSession.SiteLanguage));
			}
			else{
				title = new Title(GestionWeb.GetWebWord(1216,_webSession.SiteLanguage));
			}
			title.Font = new Font("Arial", (float)14);
			this.Titles.Add(title);
			#endregion	

			#region Series
			series.Type = SeriesChartType.Column;
			series.ShowLabelAsValue=true;
			series.XValueType=ChartValueTypes.String;
			series.YValueType=ChartValueTypes.Double;
			series.Color= Color.FromArgb(148,121,181);
			series.Enabled=true;
			series.Font=new Font("Arial", (float)10);
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
                    series.Points.AddXY(_tab[i, EngineEvolution.PRODUCT].ToString(), Convert.ToDouble(FctUtilities.Units.ConvertUnitValueToString(ecart, _webSession.Unit)));
                    series.Points[compteur].ShowInLegend = true;

                    #region Reference or competitor ?
                    if (_tab[i, EngineEvolution.COMPETITOR] != null) {
                        typeElt = Convert.ToInt32(_tab[i, EngineEvolution.COMPETITOR]);
                        if (typeElt == 2) {
                            series.Points[compteur].Color = Color.FromArgb(255, 223, 222);
                            hasComp = true;
                        }
                        else if (typeElt == 1) {
                            series.Points[compteur].Color = Color.FromArgb(222, 255, 222);
                            hasRef = true;
                        }
                    }
                    #endregion

                    compteur++;
                }
                ecart = Convert.ToDouble(_tab[last, EngineEvolution.ECART]);
                if (ecart < 0) {
                    series.Points.AddXY(_tab[last, EngineEvolution.PRODUCT].ToString(), Convert.ToDouble(FctUtilities.Units.ConvertUnitValueToString(ecart, _webSession.Unit).Replace(" ", string.Empty)));
                    series.Points[compteur].ShowInLegend = true;
                    series.Points[compteur].CustomAttributes = "LabelStyle=top";

                    #region Reference or competitor ?
                    if (_tab[last, EngineEvolution.COMPETITOR] != null) {
                        typeElt = Convert.ToInt32(_tab[last, EngineEvolution.COMPETITOR]);
                        if (typeElt == 2) {
                            series.Points[compteur].Color = Color.FromArgb(255, 223, 222);
                            hasComp = true;
                        }
                        else if (typeElt == 1) {
                            series.Points[compteur].Color = Color.FromArgb(222, 255, 222);
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
			legendItemReference.Color=Color.FromArgb(222,255,222);
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
			legendItemCompetitor.Color=Color.FromArgb(255,223,222);

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
			this.ChartAreas[strChartArea].AxisX.LabelStyle.Font=new Font("Arial", (float)8);
			this.ChartAreas[strChartArea].AxisX.MajorGrid.LineWidth=0;
			this.ChartAreas[strChartArea].AxisX.Interval=1;				
			this.ChartAreas[strChartArea].AxisX.LabelStyle.FontAngle = 35;			
			#endregion

			#region Axe des Y
			this.ChartAreas[strChartArea].AxisY.Enabled=AxisEnabled.True;
			this.ChartAreas[strChartArea].AxisY.LabelStyle.Enabled=true;
			this.ChartAreas[strChartArea].AxisY.LabelsAutoFit=false;			
			this.ChartAreas[strChartArea].AxisY.LabelStyle.Font=new Font("Arial", (float)10);
			this.ChartAreas[strChartArea].AxisY.TitleFont=new Font("Arial", (float)10);
			this.ChartAreas[strChartArea].AxisY.MajorGrid.LineWidth=0;
			#endregion

			#region Axe des Y2
			this.ChartAreas[strChartArea].AxisY2.Enabled=AxisEnabled.True;
			this.ChartAreas[strChartArea].AxisY2.LabelStyle.Enabled=true;
			this.ChartAreas[strChartArea].AxisY2.LabelsAutoFit=false;
			this.ChartAreas[strChartArea].AxisY2.LabelStyle.Font=new Font("Arial", (float)10);
			this.ChartAreas[strChartArea].AxisY2.TitleFont=new Font("Arial", (float)10);
			this.ChartAreas[strChartArea].AxisY2.Title=""+GestionWeb.GetWebWord(1217,_webSession.SiteLanguage)+"";
			#endregion					

		}
		#endregion
		


		
	}
}
