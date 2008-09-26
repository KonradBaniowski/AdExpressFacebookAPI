#region Information
//Author : Y. Rkaina 
//Creation : 19/07/2006
#endregion

using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Collections;
using System.Collections.Generic;

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
using TblFormatCst = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstComparisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion;
using TNS.AdExpressI.ProductClassIndicators.Engines;

namespace TNS.AdExpress.Anubis.Hotep.UI
{
	/// <summary>
	/// Description résumée de UIMediaStrategyGraph.
	/// </summary>
	public class UIMediaStrategyGraph :  Chart{ 
	
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
		public UIMediaStrategyGraph(WebSession webSession,IDataSource dataSource, HotepConfig config,object[,] tab):base(){
		_webSession = webSession;
		_dataSource = dataSource;
		_config = config;
		_tab = tab;
		}
		#endregion
		
		#region MediaStrategy
		/// <summary>
		/// Graphiques Media Strategy
		/// </summary>
		internal void BuildMediaStrategy(){
			
			#region Constantes
			Color[] pieColors={
								  Color.FromArgb(100,72,131),
								  Color.FromArgb(177,163,193),
								  Color.FromArgb(208,200,218),
								  Color.FromArgb(225,224,218),
								  Color.FromArgb(255,215,215),
								  Color.FromArgb(255,240,240),
								  Color.FromArgb(202,255,202)};

			const int NBRE_MEDIA=5;
			/// <summary>
			/// Hauteur d'un graphique stratégie média
			/// </summary>
			const int MEDIA_STRATEGY_HEIGHT_GRAPHIC=300;
			#endregion
					
			#region Niveau de détail
			int MEDIA_LEVEL_NUMBER;
			switch(_webSession.PreformatedMediaDetail){
				case TblFormatCst.PreformatedMediaDetails.vehicle:
					MEDIA_LEVEL_NUMBER = 1;
					break;
				case TblFormatCst.PreformatedMediaDetails.vehicleCategory:
					MEDIA_LEVEL_NUMBER = 2;
					break;
				default:
					MEDIA_LEVEL_NUMBER = 3;
					break;
			}
			#endregion

			#region Chart
            ChartArea chartArea = new ChartArea();
            this.ChartAreas.Add(chartArea);
			this.Size = new Size(800,500);
			this.BackGradientType = GradientType.TopBottom;
			this.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);
			this.BorderStyle=ChartDashStyle.Solid;
			this.BorderLineColor=Color.FromArgb(99,73,132);
			this.BorderLineWidth=2;
			this.Legend.Enabled=false;
			#endregion		

			#region Parcours de tab
			
			#region Variables
            Dictionary<string, Series> listSeriesMedia = new Dictionary<string, Series>();
            Dictionary<int, string> listSeriesName = new Dictionary<int, string>();
            Dictionary<string, double> listSeriesMediaRefCompetitor = new Dictionary<string, double>();
            Dictionary<string, DataTable> listTableRefCompetitor = new Dictionary<string, DataTable>();
            bool universTotalVerif = false;
			#endregion

            if (_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal) {
                _webSession.ComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal;
                universTotalVerif = true;
            }

            #region Create Series
            // Serie Univers
			listSeriesMedia.Add(GestionWeb.GetWebWord(1780,_webSession.SiteLanguage),new Series());
			listSeriesName.Add(0,GestionWeb.GetWebWord(1780,_webSession.SiteLanguage));
			// Serie Famille
			if(_webSession.ComparaisonCriterion==TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal){
				listSeriesMedia.Add(GestionWeb.GetWebWord(1189,_webSession.SiteLanguage),new Series());
				listSeriesName.Add(1,GestionWeb.GetWebWord(1189,_webSession.SiteLanguage));
		    // Serie Marché
			}
			else{
				listSeriesMedia.Add(GestionWeb.GetWebWord(1316,_webSession.SiteLanguage),new Series());
				listSeriesName.Add(1,GestionWeb.GetWebWord(1316,_webSession.SiteLanguage));
			}

            // Create series (one per media)
            for (int i = 1; i < _tab.GetLongLength(0); i++) {

                //	Dictionary with advertiser label as key and total as value
                if (_tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null) {
                    if (!listSeriesMediaRefCompetitor.ContainsKey(_tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString())) {
                        listSeriesMediaRefCompetitor.Add(_tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString(), new double());
                    }

                    if (!listTableRefCompetitor.ContainsKey(_tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString())) {
                        DataTable tableCompetitorRef = new DataTable();
                        tableCompetitorRef.Columns.Add("Name");
                        tableCompetitorRef.Columns.Add("Position", typeof(double));
                        listTableRefCompetitor.Add(_tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString(), tableCompetitorRef);

                    }

                    if (!listSeriesMedia.ContainsKey(_tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString())) {
                        listSeriesMedia.Add(_tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString(), new Series());
                    }
                }
            }
            #endregion

            #region Totals
            double totalUniversValue = 0;
            double totalSectorValue = 0;
            double totalMarketValue = 0;

            #region Once Media
            if (MEDIA_LEVEL_NUMBER == 2 || MEDIA_LEVEL_NUMBER == 3) {
                for (int i = 1; i < _tab.GetLongLength(0); i++) {
                    for (int j = 0; j < EngineMediaStrategy.NB_MAX_COLUMNS; j++) {
                        switch (j) {

                            #region support
                            // Univers Total
                            case EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX:
                                if (_tab[i, EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 3) {
                                    totalUniversValue += Convert.ToDouble(_tab[i, EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            // Sector Total
                            case EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX:
                                if (_tab[i, EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 3) {
                                    totalSectorValue += Convert.ToDouble(_tab[i, EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            // Market Total
                            case EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX:
                                if (_tab[i, EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 3) {
                                    totalMarketValue += Convert.ToDouble(_tab[i, EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            #endregion

                            #region Advertisers
                            case EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX:
                                if (_tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] != null
                                    && _tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX] != null
                                    && _tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null
                                    && MEDIA_LEVEL_NUMBER == 3) {
                                    listSeriesMediaRefCompetitor[_tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] += Convert.ToDouble(_tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]);
                                }
                                else if (_tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] != null
                                    && _tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX] != null
                                    && _tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null
                                    && _tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX] == null
                                    && MEDIA_LEVEL_NUMBER == 2) {
                                    listSeriesMediaRefCompetitor[_tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] += Convert.ToDouble(_tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            #endregion

                            #region Category
                            // Univers Total	
                            case EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX:
                                if (_tab[i, EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 2) {
                                    totalUniversValue += Convert.ToDouble(_tab[i, EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            // Sector Total
                            case EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX:
                                if (_tab[i, EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 2) {
                                    totalSectorValue += Convert.ToDouble(_tab[i, EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            // Market Total
                            case EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX:

                                if (_tab[i, EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 2) {
                                    totalMarketValue += Convert.ToDouble(_tab[i, EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX]);
                                }

                                break;
                            #endregion

                            default:
                                break;
                        }
                    }
                }
            }
            #endregion

            #region PluriMedia
            if (MEDIA_LEVEL_NUMBER == 1) {
                for (int i = 0; i < _tab.GetLongLength(0); i++) {
                    for (int j = 0; j < EngineMediaStrategy.NB_MAX_COLUMNS; j++) {
                        switch (j) {
                            case EngineMediaStrategy.TOTAL_UNIV_INVEST_COLUMN_INDEX:
                                if (_tab[i, EngineMediaStrategy.TOTAL_UNIV_INVEST_COLUMN_INDEX] != null) {
                                    totalUniversValue += Convert.ToDouble(_tab[i, EngineMediaStrategy.TOTAL_UNIV_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            case EngineMediaStrategy.TOTAL_SECTOR_INVEST_COLUMN_INDEX:
                                if (_tab[i, EngineMediaStrategy.TOTAL_SECTOR_INVEST_COLUMN_INDEX] != null) {
                                    totalSectorValue += Convert.ToDouble(_tab[i, EngineMediaStrategy.TOTAL_SECTOR_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            case EngineMediaStrategy.TOTAL_MARKET_INVEST_COLUMN_INDEX:
                                if (_tab[i, EngineMediaStrategy.TOTAL_MARKET_INVEST_COLUMN_INDEX] != null) {
                                    totalMarketValue += Convert.ToDouble(_tab[i, EngineMediaStrategy.TOTAL_MARKET_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            case EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX:
                                if (_tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] != null
                                    && _tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null
                                    && _tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX] != null) {
                                    listSeriesMediaRefCompetitor[_tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] += Convert.ToDouble(_tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]);
                                }

                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            #endregion

            #endregion

            #region Table
            double elementValue;
            DataTable tableUnivers = new DataTable();
            DataTable tableSectorMarket = new DataTable();
            // Define columns
            tableUnivers.Columns.Add("Name");
            tableUnivers.Columns.Add("Position", typeof(double));
            tableSectorMarket.Columns.Add("Name");
            tableSectorMarket.Columns.Add("Position", typeof(double));

            for (int i = 1; i < _tab.GetLongLength(0); i++) {
                for (int j = 0; j < EngineMediaStrategy.NB_MAX_COLUMNS; j++) {
                    switch (j) {

                        #region Media
                        case EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX:
                            if (_tab[i, EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 3) {
                                if (totalUniversValue != 0) {
                                    elementValue = Convert.ToDouble(_tab[i, EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX]) / totalUniversValue * 100;
                                    DataRow row = tableUnivers.NewRow();
                                    row["Name"] = _tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX];
                                    row["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableUnivers.Rows.Add(row);
                                }
                                j = j + 6;
                            }

                            break;
                        case EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX:
                            if (_tab[i, EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 3) {
                                if (totalSectorValue != 0) {
                                    elementValue = Convert.ToDouble(_tab[i, EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX]) / totalSectorValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = _tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableSectorMarket.Rows.Add(row1);
                                }
                                j = j + 5;
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX:
                            if (_tab[i, EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 3) {
                                if (totalMarketValue != 0) {
                                    elementValue = Convert.ToDouble(_tab[i, EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX]) / totalMarketValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = _tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableSectorMarket.Rows.Add(row1);
                                }
                                j = j + 4;
                            }
                            break;
                        #endregion

                        #region Advertisers
                        case EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX:
                            if (_tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] != null
                                && _tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX] != null
                                && _tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null
                                && MEDIA_LEVEL_NUMBER == 3) {

                                if (listSeriesMediaRefCompetitor[_tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] != 0) {
                                    elementValue = Convert.ToDouble(_tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]) / listSeriesMediaRefCompetitor[_tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] * 100;
                                    DataRow row1 = listTableRefCompetitor[_tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].NewRow();
                                    row1["Name"] = _tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    listTableRefCompetitor[_tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].Rows.Add(row1);
                                }

                                j = j + 12;
                            }
                            else if (_tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] != null
                                && _tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX] != null
                                && _tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX] == null
                                && _tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null
                                && MEDIA_LEVEL_NUMBER == 2) {

                                if (listSeriesMediaRefCompetitor[_tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] != 0) {
                                    elementValue = Convert.ToDouble(_tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]) / listSeriesMediaRefCompetitor[_tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] * 100;
                                    DataRow row1 = listTableRefCompetitor[_tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].NewRow();
                                    row1["Name"] = _tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    listTableRefCompetitor[_tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].Rows.Add(row1);
                                }
                                j = j + 12;
                            }
                            else if (_tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] != null
                        && _tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null
                        && _tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX] != null
                             && MEDIA_LEVEL_NUMBER == 1) {
                                if (listSeriesMediaRefCompetitor[_tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] != 0) {
                                    elementValue = Convert.ToDouble(_tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]) / listSeriesMediaRefCompetitor[_tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] * 100;
                                    DataRow row1 = listTableRefCompetitor[_tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].NewRow();
                                    row1["Name"] = _tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    listTableRefCompetitor[_tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].Rows.Add(row1);
                                }
                                j = j + 12;
                            }
                            break;
                        #endregion

                        #region Categorie
                        case EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX:
                            if (_tab[i, EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 2) {
                                if (totalUniversValue != 0) {
                                    elementValue = Convert.ToDouble(_tab[i, EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX]) / totalUniversValue * 100;
                                    DataRow row = tableUnivers.NewRow();
                                    row["Name"] = _tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX];
                                    row["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableUnivers.Rows.Add(row);
                                }
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX:
                            if (_tab[i, EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 2) {
                                if (totalSectorValue != 0) {
                                    elementValue = Convert.ToDouble(_tab[i, EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX]) / totalSectorValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = _tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableSectorMarket.Rows.Add(row1);
                                }
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX:
                            if (_tab[i, EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 2) {
                                if (totalMarketValue != 0) {
                                    elementValue = Convert.ToDouble(_tab[i, EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX]) / totalMarketValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = _tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableSectorMarket.Rows.Add(row1);
                                }
                            }
                            break;
                        #endregion

                        #region PluriMedia
                        case EngineMediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX:
                            if (_tab[i, EngineMediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX] != null && i > 1) {
                                if (totalUniversValue != 0) {
                                    elementValue = Convert.ToDouble(_tab[i, EngineMediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX]) / totalUniversValue * 100;
                                    DataRow row = tableUnivers.NewRow();
                                    row["Name"] = _tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX];
                                    row["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableUnivers.Rows.Add(row);
                                }
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX:
                            if (_tab[i, EngineMediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX] != null && i > 1) {
                                if (totalSectorValue != 0) {
                                    elementValue = Convert.ToDouble(_tab[i, EngineMediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX]) / totalSectorValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = _tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableSectorMarket.Rows.Add(row1);
                                }
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX:
                            if (_tab[i, EngineMediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX] != null && i > 1) {
                                if (totalMarketValue != 0) {
                                    elementValue = Convert.ToDouble(_tab[i, EngineMediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX]) / totalMarketValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = _tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableSectorMarket.Rows.Add(row1);
                                }
                            }
                            break;
                        #endregion

                        default:
                            break;
                    }
                }
            }
            #endregion

            #region Init Series
            string strSort = "Position  DESC";
            DataRow[] foundRows = null;
            foundRows = tableUnivers.Select("", strSort);
            DataRow[] foundRowsSectorMarket = null;
            foundRowsSectorMarket = tableSectorMarket.Select("", strSort);
            double[] yValues = new double[foundRows.Length];
            string[] xValues = new string[foundRows.Length];
            double[] yValuesSectorMarket = new double[foundRowsSectorMarket.Length];
            string[] xValuesSectorMarket = new string[foundRowsSectorMarket.Length];
            double otherUniversValue = 0;
            double otherSectorMarketValue = 0;
            int index = 0;

            if (MEDIA_LEVEL_NUMBER != 1) {
                for (int i = 0; i < 5 && i < foundRows.Length; i++) {
                    xValues[i] = foundRows[i]["Name"].ToString();
                    yValues[i] = Convert.ToDouble(foundRows[i]["Position"]);
                    otherUniversValue += Convert.ToDouble(foundRows[i]["Position"]);
                    index = i + 1;
                }
                if (foundRows.Length > NBRE_MEDIA) {
                    xValues[index] = GestionWeb.GetWebWord(647, _webSession.SiteLanguage);
                    yValues[index] = 100 - otherUniversValue;
                }

                for (int i = 0; i < 5 && i < foundRowsSectorMarket.Length; i++) {
                    xValuesSectorMarket[i] = foundRowsSectorMarket[i]["Name"].ToString();
                    yValuesSectorMarket[i] = Convert.ToDouble(foundRowsSectorMarket[i]["Position"]);
                    otherSectorMarketValue += Convert.ToDouble(foundRowsSectorMarket[i]["Position"]);
                    index = i + 1;
                }
                if (foundRowsSectorMarket.Length > NBRE_MEDIA) {
                    xValuesSectorMarket[index] = GestionWeb.GetWebWord(647, _webSession.SiteLanguage);
                    yValuesSectorMarket[index] = 100 - otherSectorMarketValue;
                }
            }
            // Cas PluriMedia
            else {
                for (int i = 0; i < foundRows.Length; i++) {
                    xValues[i] = foundRows[i]["Name"].ToString();
                    yValues[i] = Convert.ToDouble(foundRows[i]["Position"]);
                    otherUniversValue += Convert.ToDouble(foundRows[i]["Position"]);
                }

                for (int i = 0; i < foundRowsSectorMarket.Length; i++) {
                    xValuesSectorMarket[i] = foundRowsSectorMarket[i]["Name"].ToString();
                    yValuesSectorMarket[i] = Convert.ToDouble(foundRowsSectorMarket[i]["Position"]);
                    otherSectorMarketValue += Convert.ToDouble(foundRowsSectorMarket[i]["Position"]);
                }
            }

            double[] yVal = new double[foundRows.Length];
            string[] xVal = new string[foundRows.Length];
            double otherCompetitorRefValue = 0;
            int k = 2;

            foreach (string name in listSeriesMedia.Keys) {

                if (name == GestionWeb.GetWebWord(1780, _webSession.SiteLanguage)) {
                    if (xValues != null && xValues.Length > 0 && xValues[0] != null)
                        listSeriesMedia[GestionWeb.GetWebWord(1780, _webSession.SiteLanguage)].Points.DataBindXY(xValues, yValues);
                }
                else if (_webSession.ComparaisonCriterion == CstComparisonCriterion.sectorTotal && name == GestionWeb.GetWebWord(1189, _webSession.SiteLanguage)) {
                    if (xValuesSectorMarket != null && xValuesSectorMarket.Length > 0 && xValuesSectorMarket[0] != null)
                        listSeriesMedia[GestionWeb.GetWebWord(1189, _webSession.SiteLanguage)].Points.DataBindXY(xValuesSectorMarket, yValuesSectorMarket);
                }
                else if (name == GestionWeb.GetWebWord(1316, _webSession.SiteLanguage)) {
                    if (xValuesSectorMarket != null && xValuesSectorMarket.Length > 0 && xValuesSectorMarket[0] != null)
                        listSeriesMedia[GestionWeb.GetWebWord(1316, _webSession.SiteLanguage)].Points.DataBindXY(xValuesSectorMarket, yValuesSectorMarket);
                }
                else {
                    DataRow[] foundRowsCompetitorRef = null;
                    foundRowsCompetitorRef = ((DataTable)listTableRefCompetitor[name]).Select("", strSort);
                    otherCompetitorRefValue = 0;

                    yVal = new double[foundRowsCompetitorRef.Length];
                    xVal = new string[foundRowsCompetitorRef.Length];
                    if (MEDIA_LEVEL_NUMBER != 1) {
                        for (int i = 0; i < foundRowsCompetitorRef.Length && i < NBRE_MEDIA; i++) {


                            xVal[i] = foundRowsCompetitorRef[i]["Name"].ToString();
                            yVal[i] = Convert.ToDouble(foundRowsCompetitorRef[i]["Position"]);

                            otherCompetitorRefValue += Convert.ToDouble(foundRowsCompetitorRef[i]["Position"]);
                            index = i + 1;
                        }
                        if (foundRowsCompetitorRef.Length > NBRE_MEDIA) {
                            xVal[index] = "Autres";
                            yVal[index] = 100 - otherCompetitorRefValue;
                        }
                    }
                    // PluriMedia
                    else {
                        for (int i = 0; i < foundRowsCompetitorRef.Length; i++) {
                            xVal[i] = foundRowsCompetitorRef[i]["Name"].ToString();
                            yVal[i] = Convert.ToDouble(foundRowsCompetitorRef[i]["Position"]);

                        }
                    }
                    if (xVal.Length > 0 && xVal[0] != null)
                        listSeriesMedia[name].Points.DataBindXY(xVal, yVal);


                    listSeriesName.Add(k, name);
                    k++;
                }

            }
            #endregion	            

			float yPosition=0.0F;

			#region Affichage des graphiques
			int iterator=0;
			for(int j=0;j<listSeriesMedia.Count;j++){
				if(((Series)listSeriesMedia[(string)listSeriesName[j]]).Points.Count>0){
										
					#region Type de Graphique
					((Series)listSeriesMedia[(string)listSeriesName[j]]).Type= SeriesChartType.Pie;
					#endregion
				
					#region Définition des couleurs
					for(k=0;k<6&&k<((Series)listSeriesMedia[(string)listSeriesName[j]]).Points.Count;k++){
						((Series)listSeriesMedia[(string)listSeriesName[j]]).Points[k].Color=pieColors[k];
					}
					#endregion
				
					#region Légende
					((Series)listSeriesMedia[(string)listSeriesName[j]])["LabelStyle"]="Outside";
					((Series)listSeriesMedia[(string)listSeriesName[j]]).LegendToolTip = "#PERCENT";
					((Series)listSeriesMedia[(string)listSeriesName[j]]).ToolTip = " "+(string)listSeriesName[j]+" \n #VALX : #PERCENT";
					((Series)listSeriesMedia[(string)listSeriesName[j]])["PieLineColor"]="Black";
					#endregion

					#region Création et définition du graphique
					ChartArea chartArea2=new ChartArea();
					this.ChartAreas.Add(chartArea2);
					chartArea2.Area3DStyle.Enable3D = true; 
					chartArea2.Name=(string)listSeriesName[j];
					((Series)listSeriesMedia[(string)listSeriesName[j]]).ChartArea=chartArea2.Name;
					#endregion

					#region Titre
					this.Titles.Add(chartArea2.Name);
                    this.Titles[iterator].DockInsideChartArea = true;
                    this.Titles[iterator].Position.Auto = false;
                    this.Titles[iterator].Position.X = 45;
                    this.Titles[iterator].Position.Y = 3 + ((96 / listSeriesMedia.Count) * iterator);
                    this.Titles[iterator].Font = new Font("Arial", (float)13);
                    this.Titles[iterator].Color = Color.FromArgb(100, 72, 131);
                    this.Titles[iterator].DockToChartArea = chartArea2.Name;
					#endregion

					#region Type image
					((Series)listSeriesMedia[(string)listSeriesName[j]]).Label="#PERCENT : #VALX";
					((Series)listSeriesMedia[(string)listSeriesName[j]])["3DLabelLineSize"]="50";
					#endregion
				
					#region Positionnement du graphique
					chartArea2.Position.Width = 80;
                    chartArea2.Position.Y = 3 + (((96 / listSeriesMedia.Count) * iterator) + 1);
					chartArea2.Position.Height = (96/listSeriesMedia.Count)-1;
					chartArea2.Position.X=4;
					#endregion

                    iterator++;				

					#region Ajout des dans la série
					this.Series.Add(((Series)listSeriesMedia[(string)listSeriesName[j]]));	
					#endregion

					yPosition+=chartArea2.Position.Height;
				}
			}

			#region Dimensionnement de l'image
			// Taille d'un graphique * Nombre de graphique
			double imgLength=(MEDIA_STRATEGY_HEIGHT_GRAPHIC*listSeriesMedia.Count);
			#endregion

			#endregion

            if(universTotalVerif)
                _webSession.ComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal;
	
			#endregion			

		}
		#endregion

	}
}
