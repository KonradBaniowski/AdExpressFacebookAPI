#region Information
//Author : Y. Rkaina 
//Creation : 17/07/2006
#endregion

using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

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
using TNS.FrameWork.Date;
using TNS.AdExpressI.ProductClassIndicators.Engines;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Web;


namespace TNS.AdExpress.Anubis.Hotep.UI
{
	/// <summary>
	/// Description r�sum�e de UISeasonalityGraph.
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
		/// Tableau d'objets qui contient les r�sultats
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
			bool referenceElement=false;
			bool competitorElement=false;
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Localization); 
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
            Int64 idElement = 0;
            int cMonth = 0;
            int oldMonth = -1;
            int nbMonth = 0;

            if (_tab != null) {
                for (int i = 0; i < _tab.GetLength(0); i++) {
                    idElement = Convert.ToInt64(_tab[i, EngineSeasonality.ID_ELEMENT_COLUMN_INDEX]);
                    if (idElement != 0) {
                        if (!advertiserTotal.ContainsKey(idElement)) {
                            advertiserTotal.Add(idElement, Convert.ToDouble(_tab[i, EngineSeasonality.INVEST_COLUMN_INDEX]));
                            if (idElement != EngineSeasonality.ID_TOTAL_UNIVERSE_COLUMN_INDEX && idElement != EngineSeasonality.ID_TOTAL_MARKET_COLUMN_INDEX && idElement != EngineSeasonality.ID_TOTAL_SECTOR_COLUMN_INDEX) {
                                advertiserSerie.Add(idElement, new Series());
                                this.Series.Add(advertiserSerie[idElement]);
                            }
                        }
                        else {
                            advertiserTotal[idElement] = advertiserTotal[idElement] + Convert.ToDouble(_tab[i, EngineSeasonality.INVEST_COLUMN_INDEX]);
                        }
                    }
                    cMonth = Convert.ToInt32(_tab[i, EngineSeasonality.ID_MONTH_COLUMN_INDEX]);
                    if (cMonth != 0 && oldMonth != cMonth) {
                        nbMonth++;
                        oldMonth = cMonth;
                    }

                }
            }
            #endregion

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
			Title title = new Title(GestionWeb.GetWebWord(1139,_webSession.SiteLanguage));
			title.Font = new Font("Arial", (float)14);
			this.Titles.Add(title);
			#endregion	

            #region Series Design
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
			#endregion			

            #region Series Data
            DateTime periodBegin = FctUtilities.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);

            int compteur = -1;
            oldMonth = -1;
            cMonth = -1;
            double invest = 0;
            string month = string.Empty;
            double mediumMonth = 0;

            if (_tab != null) {
                // Calcul des totaux pour les annonceurs
                for (int i = 0; i < _tab.GetLength(0); i++) {
                    cMonth = Convert.ToInt32(_tab[i, EngineSeasonality.ID_MONTH_COLUMN_INDEX]);
                    if (oldMonth != cMonth) {
                        compteur++;
                        oldMonth = cMonth;
                    }
                    idElement = Convert.ToInt64(_tab[i, EngineSeasonality.ID_ELEMENT_COLUMN_INDEX]);
                    invest = Convert.ToDouble(_tab[i, EngineSeasonality.INVEST_COLUMN_INDEX]);
                    month = MonthString.GetCharacters(periodBegin.AddMonths(compteur).Month,cultureInfo, 0);
                    if (idElement != 0) {
                        if (idElement == EngineSeasonality.ID_TOTAL_UNIVERSE_COLUMN_INDEX) {
                            if (advertiserTotal[idElement] > 0) {
                                serieUnivers.Points.AddXY(month, Math.Round(invest / advertiserTotal[idElement] * 100, 2));
                            }
                        }
                        else if (idElement == EngineSeasonality.ID_TOTAL_SECTOR_COLUMN_INDEX || idElement == EngineSeasonality.ID_TOTAL_MARKET_COLUMN_INDEX) {
                            if (advertiserTotal[idElement] > 0) {
                                serieSectorMarket.Points.AddXY(month, Math.Round(invest / advertiserTotal[idElement] * 100, 2));
                                serieMediumMonth.Points.AddXY(month, mediumMonth = Math.Round((double)100 / nbMonth, 2));
                            }
                        }
                        else {
                            Series s = advertiserSerie[idElement];
                            s.Type = SeriesChartType.Line;
                            s.ShowLabelAsValue = true;
                            s.XValueType = ChartValueTypes.String;
                            s.YValueType = ChartValueTypes.Double;
                            s.Enabled = true;
                            s.Font = new Font("Arial", (float)8);
                            s.LegendText = _tab[i, EngineSeasonality.LABEL_ELEMENT_COLUMN_INDEX].ToString();
                            s.Enabled = true;
                            s.Font = new Font("Arial", (float)8);
                            s.LabelToolTip = _tab[i, EngineSeasonality.LABEL_ELEMENT_COLUMN_INDEX].ToString();

                            if (advertiserTotal[idElement] > 0) {
                                advertiserSerie[idElement].Points.AddXY(month, Math.Round(invest / advertiserTotal[idElement] * 100, 2));

                            }
                        }
                    }
                }
            }
            #endregion

			#region L�gendes
			//univers
			serieUnivers.LegendText=GestionWeb.GetWebWord(1188,_webSession.SiteLanguage);
			serieUnivers.LabelToolTip = WebFunctions.Text.SuppressAccent(GestionWeb.GetWebWord(1188,_webSession.SiteLanguage));
            // Market
			if(_webSession.ComparaisonCriterion==TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.marketTotal){
				serieSectorMarket.LegendText=GestionWeb.GetWebWord(1316,_webSession.SiteLanguage);
				serieSectorMarket.LabelToolTip = WebFunctions.Text.SuppressAccent(GestionWeb.GetWebWord(1316,_webSession.SiteLanguage));
			}
			// Famille
			else{
				serieSectorMarket.LegendText=GestionWeb.GetWebWord(1189,_webSession.SiteLanguage);
				serieSectorMarket.LabelToolTip = WebFunctions.Text.SuppressAccent(GestionWeb.GetWebWord(1189,_webSession.SiteLanguage));
			}
			//Mois Moyen th�orique
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

            #region X axe
            this.ChartAreas[strChartArea].AxisX.LabelStyle.Enabled = true;
			this.ChartAreas[strChartArea].AxisX.LabelsAutoFit = false;
			this.ChartAreas[strChartArea].AxisX.LabelStyle.Font=new Font("Arial", (float)8);
			this.ChartAreas[strChartArea].AxisX.MajorGrid.LineWidth=0;
			this.ChartAreas[strChartArea].AxisX.Interval=1;				
			this.ChartAreas[strChartArea].AxisX.LabelStyle.FontAngle = 35;
			#endregion

            #region Y axe
            this.ChartAreas[strChartArea].AxisY.Enabled=AxisEnabled.True;
			this.ChartAreas[strChartArea].AxisY.LabelStyle.Enabled=true;
			this.ChartAreas[strChartArea].AxisY.LabelsAutoFit=false;			
			this.ChartAreas[strChartArea].AxisY.LabelStyle.Font=new Font("Arial", (float)10);
			this.ChartAreas[strChartArea].AxisY.TitleFont=new Font("Arial", (float)10);
			this.ChartAreas[strChartArea].AxisY.MajorGrid.LineWidth=0;
			#endregion

            #region Y2 axe
            this.ChartAreas[strChartArea].AxisY2.Enabled=AxisEnabled.True;
			this.ChartAreas[strChartArea].AxisY2.LabelStyle.Enabled=true;
			this.ChartAreas[strChartArea].AxisY2.LabelsAutoFit=false;
			this.ChartAreas[strChartArea].AxisY2.LabelStyle.Font=new Font("Arial", (float)10);
			this.ChartAreas[strChartArea].AxisY2.TitleFont=new Font("Arial", (float)10);
			this.ChartAreas[strChartArea].AxisY2.Title=""+GestionWeb.GetWebWord(1236,_webSession.SiteLanguage)+"";
			#endregion							

		}
		#endregion

		
	}
}
