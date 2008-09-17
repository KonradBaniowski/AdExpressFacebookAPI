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


namespace TNS.AdExpress.Anubis.Hotep.UI
{
	/// <summary>
	/// Description résumée de UISeasonalityGraph.
	/// </summary>
	public class UISeasonalityGraph : Chart
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
		public UISeasonalityGraph(WebSession webSession,IDataSource dataSource, HotepConfig config,object[,] tab):base(){
		_webSession = webSession;
		_dataSource = dataSource;
		_config = config;
		_tab = tab;
		}
		#endregion
		
		#region Seasonality
		/// <summary>
		/// Graphiques Seasonality
		/// </summary>
		internal void BuildSeasonality(){

			#region Variables
			Series serieUnivers = new Series("Seasonality");
			Series serieSectorMarket = new Series();
			Series serieMediumMonth = new Series();

			ChartArea chartArea=new ChartArea();

			bool referenceElement=false;
			bool competitorElement=false;
			
			int compteur=1;
			// Nombre de mois
			int nbMonth=0;
			int i=0;
		
			double number=0;
			int oldMonth=-1;
			


			double mediumMonth=0;

			Hashtable advertiserTotal = new Hashtable();
			Hashtable advertiserSerie=new Hashtable();
			#endregion

			this.Series.Add(serieUnivers);
			this.Series.Add(serieSectorMarket);
			this.Series.Add(serieMediumMonth);

			this.ChartAreas.Add(chartArea);
			string strChartArea = this.Series["Seasonality"].ChartArea;

			compteur=0;
		
			long valueAdvertiser=0;
			if(_tab!=null){
				//advertiserNumber=tabAdvertiser.GetLength(0)/(tabUniverse.GetLength(0)-1);
				for(i=0;i<_tab.GetLength(0);i++){
					if(advertiserTotal[long.Parse(_tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())]==null){
						advertiserTotal.Add(long.Parse(_tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString()),long.Parse(_tab[i,FrameWorkConstantes.Results.Seasonality.INVEST_COLUMN_INDEX].ToString()));
						if(long.Parse(_tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())!=FrameWorkConstantes.Results.Seasonality.ID_TOTAL_UNIVERSE_COLUMN_INDEX
							&& long.Parse(_tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())!=FrameWorkConstantes.Results.Seasonality.ID_TOTAL_MARKET_COLUMN_INDEX
							&& long.Parse(_tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())!=FrameWorkConstantes.Results.Seasonality.ID_TOTAL_SECTOR_COLUMN_INDEX
							){
							advertiserSerie.Add(long.Parse(_tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString()),new Series());
							this.Series.Add(((Series)advertiserSerie[long.Parse( _tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())]));
						}
					}
					else{
						valueAdvertiser=(long)advertiserTotal[long.Parse( _tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())];
						advertiserTotal[long.Parse(_tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())]=valueAdvertiser+long.Parse(_tab[i,FrameWorkConstantes.Results.Seasonality.INVEST_COLUMN_INDEX].ToString());
					}
					if(oldMonth!=int.Parse(_tab[i,FrameWorkConstantes.Results.Seasonality.ID_MONTH_COLUMN_INDEX].ToString())){
						nbMonth++;
						oldMonth=int.Parse(_tab[i,FrameWorkConstantes.Results.Seasonality.ID_MONTH_COLUMN_INDEX].ToString());
					}

				}
			}

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
			title = new Title(GestionWeb.GetWebWord(1139,_webSession.SiteLanguage));
			title.Font = new Font("Arial", (float)14);
			this.Titles.Add(title);
			#endregion	

			#region Series
			serieUnivers.Type = SeriesChartType.Line;
			serieUnivers.ShowLabelAsValue=true;
			serieUnivers.XValueType=ChartValueTypes.String;
			serieUnivers.YValueType=ChartValueTypes.Double;
			serieUnivers.Enabled=true;
			serieUnivers.Font=new Font("Arial", (float)8);
			serieUnivers["LabelStyle"] = "Top";
			
			
			serieSectorMarket.Type = SeriesChartType.Line;
			serieSectorMarket.ShowLabelAsValue=true;
			serieSectorMarket.XValueType=ChartValueTypes.String;
			serieSectorMarket.YValueType=ChartValueTypes.Double;
			serieSectorMarket.Enabled=true;
			serieSectorMarket.Font=new Font("Arial", (float)8);
			serieSectorMarket["LabelStyle"] = "Bottom";
							
			serieMediumMonth.Type = SeriesChartType.Line;
			serieMediumMonth.ShowLabelAsValue=false;
			serieMediumMonth.XValueType=ChartValueTypes.String;
			serieMediumMonth.YValueType=ChartValueTypes.Double;
			serieMediumMonth.Enabled=true;
			serieMediumMonth.Font=new Font("Arial", (float)8);
			serieMediumMonth.LabelToolTip= GestionWeb.GetWebWord(1233,_webSession.SiteLanguage);

			//serieUnivers.FontAngle=90;
			
			#endregion			

			
			DateTime PeriodBeginningDate = WebFunctions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);
			
			compteur=-1;
			oldMonth=-1;

			if(_tab!=null){
				// Calcul des totaux pour les annonceurs
				for(i=0;i<_tab.GetLength(0);i++){

					if(oldMonth!=int.Parse(_tab[i,FrameWorkConstantes.Results.Seasonality.ID_MONTH_COLUMN_INDEX].ToString())){
						compteur++;
						oldMonth=int.Parse(_tab[i,FrameWorkConstantes.Results.Seasonality.ID_MONTH_COLUMN_INDEX].ToString());
					}
					
				
					if(long.Parse(_tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())==FrameWorkConstantes.Results.Seasonality.ID_TOTAL_UNIVERSE_COLUMN_INDEX){
						if(double.Parse(advertiserTotal[long.Parse( _tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())].ToString())>0){
							number= double.Parse(_tab[i,FrameWorkConstantes.Results.Seasonality.INVEST_COLUMN_INDEX].ToString())/double.Parse(advertiserTotal[long.Parse( _tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())].ToString()) *100;
							serieUnivers.Points.AddXY(TNS.FrameWork.Date.MonthString.Get(PeriodBeginningDate.AddMonths(compteur).Month,_webSession.SiteLanguage,0),double.Parse(number.ToString("0.00")));
						}
					}
					else if(long.Parse(_tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())==FrameWorkConstantes.Results.Seasonality.ID_TOTAL_SECTOR_COLUMN_INDEX
						|| long.Parse(_tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())==FrameWorkConstantes.Results.Seasonality.ID_TOTAL_MARKET_COLUMN_INDEX
						
						){
						if(double.Parse(advertiserTotal[long.Parse( _tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())].ToString())>0){
							number= double.Parse(_tab[i,FrameWorkConstantes.Results.Seasonality.INVEST_COLUMN_INDEX].ToString())/double.Parse(advertiserTotal[long.Parse( _tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())].ToString()) *100;
							serieSectorMarket.Points.AddXY(TNS.FrameWork.Date.MonthString.Get(PeriodBeginningDate.AddMonths(compteur).Month,_webSession.SiteLanguage,0),double.Parse(number.ToString("0.00")));
						
							number=(double)100/nbMonth;
							serieMediumMonth.Points.AddXY(TNS.FrameWork.Date.MonthString.Get(PeriodBeginningDate.AddMonths(compteur).Month,_webSession.SiteLanguage,0),double.Parse(number.ToString("0.00")));
							mediumMonth=double.Parse(number.ToString("0.00"));
						}							
					}
					else{
						Series s = (Series)advertiserSerie[long.Parse( _tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())];
						s.Type = SeriesChartType.Line;
						s.ShowLabelAsValue=true;
						s.XValueType=ChartValueTypes.String;
						s.YValueType=ChartValueTypes.Double;					
						s.Enabled=true;
						s.Font=new Font("Arial", (float)8);
						s.LegendText=_tab[i,FrameWorkConstantes.Results.Seasonality.LABEL_ELEMENT_COLUMN_INDEX].ToString();
						s.LabelToolTip = _tab[i,FrameWorkConstantes.Results.Seasonality.LABEL_ELEMENT_COLUMN_INDEX].ToString();


						if(double.Parse(advertiserTotal[long.Parse( _tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())].ToString())>0){
							number= double.Parse(_tab[i,FrameWorkConstantes.Results.Seasonality.INVEST_COLUMN_INDEX].ToString())/double.Parse(advertiserTotal[long.Parse( _tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())].ToString()) *100;
							((Series)advertiserSerie[long.Parse( _tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())]).Points.AddXY(TNS.FrameWork.Date.MonthString.Get(PeriodBeginningDate.AddMonths(compteur).Month,_webSession.SiteLanguage,0),double.Parse(number.ToString("0.00")));
							
						}					
					}
				}
			}


			#region Légendes
			//univers
			serieUnivers.LegendText=GestionWeb.GetWebWord(1188,_webSession.SiteLanguage);
			serieUnivers.LabelToolTip = WebFunctions.Text.SuppressAccent(GestionWeb.GetWebWord(1188,_webSession.SiteLanguage));
			// Marché
			if(_webSession.ComparaisonCriterion==TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.marketTotal){
				serieSectorMarket.LegendText=GestionWeb.GetWebWord(1316,_webSession.SiteLanguage);
				serieSectorMarket.LabelToolTip = WebFunctions.Text.SuppressAccent(GestionWeb.GetWebWord(1316,_webSession.SiteLanguage));

			}
				// Famille
			else{
				serieSectorMarket.LegendText=GestionWeb.GetWebWord(1189,_webSession.SiteLanguage);
				serieSectorMarket.LabelToolTip = WebFunctions.Text.SuppressAccent(GestionWeb.GetWebWord(1189,_webSession.SiteLanguage));
			}
			//Mois Moyen théorique
			serieMediumMonth.LegendText=GestionWeb.GetWebWord(1233,_webSession.SiteLanguage)+" "+mediumMonth.ToString()+" %";

			LegendItem legendItemReference = new LegendItem();			
			legendItemReference.BorderWidth=0;
			legendItemReference.Color=Color.FromArgb(222,255,222);
			if(referenceElement){								
				legendItemReference.Name=GestionWeb.GetWebWord(1203,_webSession.SiteLanguage);
				this.Legends["Default"].CustomItems.Add(legendItemReference);			
			}			
			LegendItem legendItemCompetitor = new LegendItem();
			legendItemCompetitor.BorderWidth=0;			
			legendItemCompetitor.Color=Color.FromArgb(255,223,222);

			if(competitorElement){
				
				legendItemCompetitor.Name=GestionWeb.GetWebWord(1202,_webSession.SiteLanguage);
				this.Legends["Default"].CustomItems.Add(legendItemCompetitor);				
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
			//	this.ChartAreas[strChartArea].AxisY.Title=""+GestionWeb.GetWebWord(1217,webSession.SiteLanguage)+"";
			this.ChartAreas[strChartArea].AxisY.TitleFont=new Font("Arial", (float)10);
			//	this.ChartAreas[strChartArea].AxisY.Maximum=100;
			this.ChartAreas[strChartArea].AxisY.MajorGrid.LineWidth=0;
			
			#endregion

			#region Axe des Y2

			this.ChartAreas[strChartArea].AxisY2.Enabled=AxisEnabled.True;
			this.ChartAreas[strChartArea].AxisY2.LabelStyle.Enabled=true;
			this.ChartAreas[strChartArea].AxisY2.LabelsAutoFit=false;
			
			this.ChartAreas[strChartArea].AxisY2.LabelStyle.Font=new Font("Arial", (float)10);
			this.ChartAreas[strChartArea].AxisY2.TitleFont=new Font("Arial", (float)10);
			//	this.ChartAreas[strChartArea].AxisY2.Maximum=100;
			this.ChartAreas[strChartArea].AxisY2.Title=""+GestionWeb.GetWebWord(1236,_webSession.SiteLanguage)+"";

			#endregion							

		}
		#endregion

		
	}
}
