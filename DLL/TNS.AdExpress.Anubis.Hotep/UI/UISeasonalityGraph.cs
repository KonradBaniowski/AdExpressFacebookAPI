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
        /// <summary>
        /// Style
        /// </summary>
        private TNS.AdExpress.Domain.Theme.Style _style = null;
		#endregion

		#region Constructeur
        public UISeasonalityGraph(WebSession webSession, IDataSource dataSource, HotepConfig config, object[,] tab, TNS.AdExpress.Domain.Theme.Style style)
            : base() {
            _webSession = webSession;
            _dataSource = dataSource;
            _config = config;
            _tab = tab;
            _style = style;
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
            Color colorTemp = Color.Black;
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
            _style.GetTag("SeasonalityGraphSize").SetStyleDundas(this);
			this.BackGradientType = GradientType.TopBottom;
            _style.GetTag("SeasonalityGraphBackColor").SetStyleDundas(ref colorTemp);
            this.ChartAreas[strChartArea].BackColor = colorTemp;
            _style.GetTag("SeasonalityGraphLineEnCircle").SetStyleDundas(this);
			#endregion

			#region Titre
			Title title = new Title(GestionWeb.GetWebWord(1139,_webSession.SiteLanguage));
			this.Titles.Add(title);
            _style.GetTag("SeasonalityGraphTitleFont").SetStyleDundas(this.Titles[0]);
			#endregion	

            #region Series Design
            serieUnivers.Type = SeriesChartType.Line;
			serieUnivers.ShowLabelAsValue=true;
			serieUnivers.XValueType=ChartValueTypes.String;
			serieUnivers.YValueType=ChartValueTypes.Double;
			serieUnivers.Enabled=true;
            _style.GetTag("SeasonalityGraphTitleFontSerieUnivers").SetStyleDundas(serieUnivers);
			serieUnivers["LabelStyle"] = "Top";
			
			serieSectorMarket.Type = SeriesChartType.Line;
			serieSectorMarket.ShowLabelAsValue=true;
			serieSectorMarket.XValueType=ChartValueTypes.String;
			serieSectorMarket.YValueType=ChartValueTypes.Double;
			serieSectorMarket.Enabled=true;
            _style.GetTag("SeasonalityGraphTitleFontSerieSectorMarket").SetStyleDundas(serieSectorMarket);
			serieSectorMarket["LabelStyle"] = "Bottom";
							
			serieMediumMonth.Type = SeriesChartType.Line;
			serieMediumMonth.ShowLabelAsValue=false;
			serieMediumMonth.XValueType=ChartValueTypes.String;
			serieMediumMonth.YValueType=ChartValueTypes.Double;
			serieMediumMonth.Enabled=true;
            _style.GetTag("SeasonalityGraphTitleFontSerieMediumMonth").SetStyleDundas(serieMediumMonth);
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
                            _style.GetTag("SeasonalityGraphTitleFontSerieAdvertiser").SetStyleDundas(s);
                            s.LegendText = _tab[i, EngineSeasonality.LABEL_ELEMENT_COLUMN_INDEX].ToString();
                            s.Enabled = true;
                            _style.GetTag("SeasonalityGraphTitleFontSerieAdvertiser").SetStyleDundas(s);
                            s.LabelToolTip = _tab[i, EngineSeasonality.LABEL_ELEMENT_COLUMN_INDEX].ToString();

                            if (advertiserTotal[idElement] > 0) {
                                advertiserSerie[idElement].Points.AddXY(month, Math.Round(invest / advertiserTotal[idElement] * 100, 2));

                            }
                        }
                    }
                }
            }
            #endregion

			#region Légendes
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
			//Mois Moyen théorique
			serieMediumMonth.LegendText=GestionWeb.GetWebWord(1233,_webSession.SiteLanguage)+" "+mediumMonth.ToString()+" %";

			LegendItem legendItemReference = new LegendItem();			
			legendItemReference.BorderWidth=0;
            _style.GetTag("SeasonalityGraphColorLegendItemReference").SetStyleDundas(ref colorTemp);
            legendItemReference.Color = colorTemp;

			if(referenceElement){								
				legendItemReference.Name=GestionWeb.GetWebWord(1203,_webSession.SiteLanguage);
				this.Legends["Default"].CustomItems.Add(legendItemReference);			
			}			
			LegendItem legendItemCompetitor = new LegendItem();
			legendItemCompetitor.BorderWidth=0;
            _style.GetTag("SeasonalityGraphColorLegendItemCompetitor").SetStyleDundas(ref colorTemp);
            legendItemCompetitor.Color = colorTemp;

			if(competitorElement){
				
				legendItemCompetitor.Name=GestionWeb.GetWebWord(1202,_webSession.SiteLanguage);
				this.Legends["Default"].CustomItems.Add(legendItemCompetitor);				
			}
			#endregion

            #region X axe
            this.ChartAreas[strChartArea].AxisX.LabelStyle.Enabled = true;
			this.ChartAreas[strChartArea].AxisX.LabelsAutoFit = false;
            _style.GetTag("SeasonalityGraphLabelFontAxisX").SetStyleDundas(this.ChartAreas[strChartArea].AxisX.LabelStyle);
			this.ChartAreas[strChartArea].AxisX.MajorGrid.LineWidth=0;
			this.ChartAreas[strChartArea].AxisX.Interval=1;				
			this.ChartAreas[strChartArea].AxisX.LabelStyle.FontAngle = 35;
			#endregion

            #region Y axe
            this.ChartAreas[strChartArea].AxisY.Enabled=AxisEnabled.True;
			this.ChartAreas[strChartArea].AxisY.LabelStyle.Enabled=true;
			this.ChartAreas[strChartArea].AxisY.LabelsAutoFit=false;
            _style.GetTag("SeasonalityGraphLabelFontAxisY").SetStyleDundas(this.ChartAreas[strChartArea].AxisY.LabelStyle);
            _style.GetTag("SeasonalityGraphTitleFontAxisY").SetStyleDundas(this.ChartAreas[strChartArea].AxisY);
			this.ChartAreas[strChartArea].AxisY.MajorGrid.LineWidth=0;
			#endregion

            #region Y2 axe
            this.ChartAreas[strChartArea].AxisY2.Enabled=AxisEnabled.True;
			this.ChartAreas[strChartArea].AxisY2.LabelStyle.Enabled=true;
			this.ChartAreas[strChartArea].AxisY2.LabelsAutoFit=false;
            _style.GetTag("SeasonalityGraphLabelFontAxisY2").SetStyleDundas(this.ChartAreas[strChartArea].AxisY2.LabelStyle);
            _style.GetTag("SeasonalityGraphTitleFontAxisY2").SetStyleDundas(this.ChartAreas[strChartArea].AxisY2);
			this.ChartAreas[strChartArea].AxisY2.Title=""+GestionWeb.GetWebWord(1236,_webSession.SiteLanguage)+"";
			#endregion							

		}
		#endregion

		
	}
}
