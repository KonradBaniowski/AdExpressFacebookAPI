#region Information
//Author : Y. Rkaina 
//Creation : 18/07/2006
#endregion

using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Collections;

using TNS.AdExpress.Common;

using TNS.AdExpress.Constantes.Web;
using CstUI = TNS.AdExpress.Constantes.Web.UI;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;

using TNS.AdExpress.Anubis.Hotep.Common;
using TNS.AdExpress.Anubis.Hotep.Exceptions;

using Dundas.Charting.WinControl;
using TNS.FrameWork.DB.Common;
using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.FrameWork;

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
		internal void BuildEvolution(FrameWorkConstantes.Results.EvolutionRecap.ElementType tableType){

			#region Variables
			Series series = new Series("Evolution");
			ChartArea chartArea=new ChartArea();
			bool referenceElement=false;
			bool competitorElement=false;
			long last;
			int compteur=0;
			#endregion

			this.Series.Add(series);
			this.ChartAreas.Add(chartArea);
			string strChartArea = this.Series["Evolution"].ChartArea;
			last=_tab.GetLongLength(0)-1;
			
			#region Chart
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
	
			#region Parcours de tab
			for(int i=0;i<_tab.GetLongLength(0) && i<10 ;i++){
			
				if(WebFunctions.CheckedText.IsStringEmpty(WebFunctions.Units.ConvertUnitValueToString(_tab[0,FrameWorkConstantes.Results.EvolutionRecap.ECART].ToString(),_webSession.Unit)) 
					&& double.Parse(_tab[i,FrameWorkConstantes.Results.EvolutionRecap.ECART].ToString())>0){	
				
					//					series.Points.AddXY(tab[i,FrameWorkConstantes.Results.EvolutionRecap.PRODUCT].ToString(),(int)(double)tab[i,FrameWorkConstantes.Results.EvolutionRecap.ECART]);
					series.Points.AddXY(_tab[i,FrameWorkConstantes.Results.EvolutionRecap.PRODUCT].ToString(),(int)double.Parse(WebFunctions.Units.ConvertUnitValueToString(_tab[i,FrameWorkConstantes.Results.EvolutionRecap.ECART].ToString(),_webSession.Unit)));
					series.Points[compteur].ShowInLegend=true;
					// Coloration des concurrents en rouge
					if(_tab[i,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]!=null && (int)_tab[i,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]==2){
						series.Points[compteur].Color=Color.FromArgb(255,223,222);
						competitorElement=true;
					}
						// Coloration des références en vert
					else if(_tab[i,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]!=null && (int)_tab[i,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]==1){
						series.Points[compteur].Color=Color.FromArgb(222,255,222);	
						referenceElement=true;
					}	
					
					compteur++;
				}

				if( WebFunctions.CheckedText.IsStringEmpty(WebFunctions.Units.ConvertUnitValueToString(_tab[0,FrameWorkConstantes.Results.EvolutionRecap.ECART].ToString(),_webSession.Unit)) 
					&& double.Parse(_tab[last,FrameWorkConstantes.Results.EvolutionRecap.ECART].ToString())<0){					
					series.Points.AddXY(_tab[last,FrameWorkConstantes.Results.EvolutionRecap.PRODUCT].ToString(),(int)double.Parse(WebFunctions.Units.ConvertUnitValueToString(_tab[last,FrameWorkConstantes.Results.EvolutionRecap.ECART].ToString(),_webSession.Unit).Replace(" ","").Trim()));
					
					series.Points[compteur].ShowInLegend=true;
					series.Points[compteur].CustomAttributes="LabelStyle=top";
					

					// Coloration des concurrents en rouge
					if(_tab[last,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]!=null && (int)_tab[last,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]==2){
						series.Points[compteur].Color=Color.FromArgb(255,223,222);
						competitorElement=true;
					}
						// Coloration des références en vert
					else if(_tab[last,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]!=null && (int)_tab[last,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]==1){
						series.Points[compteur].Color=Color.FromArgb(222,255,222);	
						referenceElement=true;
					}
	
					compteur++;
				}
				
				last--;

			}
			#endregion
			
			#region Légendes
			if(tableType==FrameWorkConstantes.Results.EvolutionRecap.ElementType.advertiser){
				series.LegendText=""+GestionWeb.GetWebWord(1106,_webSession.SiteLanguage)+"";
			}
			else{
				series.LegendText=""+GestionWeb.GetWebWord(1200,_webSession.SiteLanguage)+"";
			}
			LegendItem legendItemReference = new LegendItem();			
			legendItemReference.BorderWidth=0;
			legendItemReference.Color=Color.FromArgb(222,255,222);
			if(referenceElement){
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

			if(competitorElement){
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
			//	this.ChartAreas[strChartArea].AxisY.Title=""+GestionWeb.GetWebWord(1217,webSession.SiteLanguage)+"";
			this.ChartAreas[strChartArea].AxisY.TitleFont=new Font("Arial", (float)10);
			//	this.ChartAreas[strChartArea].AxisY.Maximum=double.Parse(tab[0,FrameWorkConstantes.Results.EvolutionRecap.TOTAL_N].ToString());
			this.ChartAreas[strChartArea].AxisY.MajorGrid.LineWidth=0;
			
			#endregion

			#region Axe des Y2

			this.ChartAreas[strChartArea].AxisY2.Enabled=AxisEnabled.True;
			this.ChartAreas[strChartArea].AxisY2.LabelStyle.Enabled=true;
			this.ChartAreas[strChartArea].AxisY2.LabelsAutoFit=false;
			
			this.ChartAreas[strChartArea].AxisY2.LabelStyle.Font=new Font("Arial", (float)10);
			this.ChartAreas[strChartArea].AxisY2.TitleFont=new Font("Arial", (float)10);
			//this.ChartAreas[strChartArea].AxisY2.Maximum=100;
			this.ChartAreas[strChartArea].AxisY2.Title=""+GestionWeb.GetWebWord(1217,_webSession.SiteLanguage)+"";

			#endregion					
			

		}
		#endregion
		


		
	}
}
