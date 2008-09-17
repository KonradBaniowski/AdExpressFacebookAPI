#region Information
//Author : Y. Rkaina 
//Creation : 17/07/2006
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
using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;


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
		
		#endregion

		#region Constructeur
		public UIPalmaresGraph(WebSession webSession,IDataSource dataSource, HotepConfig config,object[,] tab):base(){
		_webSession = webSession;
		_dataSource = dataSource;
		_config = config;
		_tab = tab;
		}
		#endregion
		
		#region Palmares
		/// <summary>
		/// Graphiques Palmares
		/// </summary>
		internal void BuildPalmares(FrameWorkConstantes.Results.PalmaresRecap.ElementType tableType){
		
			#region Variables
			Series series = new Series("Palmares");
			ChartArea chartArea=new ChartArea();			
			bool referenceElement=false;
			bool competitorElement=false;
			// Il y a au moins un élément
			bool oneProductExist=false;
			#endregion
		
			this.Series.Add(series);
			this.ChartAreas.Add(chartArea);

			string strChartArea = this.Series["Palmares"].ChartArea;

			#region Chart
			this.Size = new Size(800,500);
			this.BackGradientType = GradientType.TopBottom;
			this.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);
			this.ChartAreas[strChartArea].BackColor=Color.FromArgb(222,207,231);
			this.DataManipulator.Sort(PointsSortOrder.Descending,series);
			this.BorderStyle=ChartDashStyle.Solid;
			this.BorderLineColor=Color.FromArgb(99,73,132);
			this.BorderLineWidth=2;
			#endregion

			#region Titre
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
			series.FontAngle=45;
			
			#endregion			

			#region Parcours de tab
			for(int i=1;i<_tab.GetLongLength(0) && i<11 ;i++){
				
				if(_tab[i,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N].ToString()!="0" 
					&& WebFunctions.CheckedText.IsStringEmpty(WebFunctions.Units.ConvertUnitValueToString(_tab[0,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N].ToString(),_webSession.Unit)) ){
					oneProductExist=true;
					
					//					series.Points.AddXY(tab[i,FrameWorkConstantes.Results.PalmaresRecap.PRODUCT].ToString(),(int)(double)tab[i,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N]);
					series.Points.AddXY(_tab[i,FrameWorkConstantes.Results.PalmaresRecap.PRODUCT].ToString(),(int)double.Parse(WebFunctions.Units.ConvertUnitValueToString(_tab[i,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N].ToString(),_webSession.Unit)));					
				
					series.Points[i-1].ShowInLegend=true;
					// Coloration des concurrents en rouge
					if(_tab[i,FrameWorkConstantes.Results.PalmaresRecap.COMPETITOR]!=null && (int)_tab[i,FrameWorkConstantes.Results.PalmaresRecap.COMPETITOR]==2){
						series.Points[i-1].Color=Color.FromArgb(255,223,222);
						competitorElement=true;
					}
						// Coloration des références en vert
					else if(_tab[i,FrameWorkConstantes.Results.PalmaresRecap.COMPETITOR]!=null && (int)_tab[i,FrameWorkConstantes.Results.PalmaresRecap.COMPETITOR]==1){
						series.Points[i-1].Color=Color.FromArgb(222,255,222);	
						referenceElement=true;
					}	
				}
			}
			if(!oneProductExist)
				this.Visible=false;
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
			legendItemReference.Color=Color.FromArgb(222,255,222);
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
			legendItemCompetitor.Color=Color.FromArgb(255,223,222);

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
			this.ChartAreas[strChartArea].AxisY.Title=""+GestionWeb.GetWebWord(1206,_webSession.SiteLanguage)+"";
			this.ChartAreas[strChartArea].AxisY.TitleFont=new Font("Arial", (float)10);
			if(WebFunctions.CheckedText.IsStringEmpty(WebFunctions.Units.ConvertUnitValueToString(_tab[0,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N].ToString(),_webSession.Unit)))
				//			this.ChartAreas[strChartArea].AxisY.Maximum=double.Parse(tab[0,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N].ToString());
				this.ChartAreas[strChartArea].AxisY.Maximum=double.Parse(WebFunctions.Units.ConvertUnitValueToString(_tab[0,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N].ToString(),_webSession.Unit));
			else this.ChartAreas[strChartArea].AxisY.Maximum = (double)0.0;
			this.ChartAreas[strChartArea].AxisY.MajorGrid.LineWidth=0;
			
			#endregion

			#region Axe des Y2

			this.ChartAreas[strChartArea].AxisY2.Enabled=AxisEnabled.True;
			this.ChartAreas[strChartArea].AxisY2.LabelStyle.Enabled=true;
			this.ChartAreas[strChartArea].AxisY2.LabelsAutoFit=false;
			
			this.ChartAreas[strChartArea].AxisY2.LabelStyle.Font=new Font("Arial", (float)10);
			this.ChartAreas[strChartArea].AxisY2.TitleFont=new Font("Arial", (float)10);
			this.ChartAreas[strChartArea].AxisY2.Maximum=100;
			this.ChartAreas[strChartArea].AxisY2.Title=""+GestionWeb.GetWebWord(1205,_webSession.SiteLanguage)+"";

			#endregion					

		}
		#endregion

	}
}
