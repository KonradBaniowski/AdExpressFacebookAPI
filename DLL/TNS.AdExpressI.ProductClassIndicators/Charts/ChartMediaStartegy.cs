using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.ProductClassIndicators.DAL;
using TNS.AdExpressI.ProductClassIndicators.Engines;
using System.Drawing;

using CstComparisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion;
using CstPreformatedDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstDbClassif = TNS.AdExpress.Constantes.Classification.DB;

using TNS.AdExpress.Domain.Translation;
using Dundas.Charting.WebControl;
using System.Data;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;

using System.Web.UI.WebControls;

namespace TNS.AdExpressI.ProductClassIndicators.Charts
{

    [ToolboxData("<{0}:ChartMediaStartegy runat=server></{0}:ChartMediaStartegy>")]
    public class ChartMediaStartegy : TNS.AdExpressI.ProductClassIndicators.Charts.ChartProductClassIndicator
    {

        #region Variables
        /// <summary>
        /// Engine Media Strategy
        /// </summary>
        protected EngineMediaStrategy _engine;

        #endregion

        #region Constants
        protected const int NBRE_MEDIA = 5;
        #endregion


        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User sessions</param>
        /// <param name="dalLayer">Data Access Layer</param>
        public ChartMediaStartegy(WebSession session, IProductClassIndicatorsDAL dalLayer)
            : base(session, dalLayer)
        {
            InitEngine();
        }
        #endregion

        #region OnPreRender
        protected override void OnPreRender(EventArgs e)
        {
            bool withPluriByCategory = (_session.PreformatedMediaDetail == CstPreformatedDetail.PreformatedMediaDetails.vehicleCategory
                && CstDbClassif.Vehicles.names.plurimedia == VehiclesInformation.DatabaseIdToEnum(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID));
            #region Animation Params
            if (_chartType != ChartImageType.Flash)
            {
                this.ImageType = _chartType;
            }
            else
            {
                this.ImageType = ChartImageType.Flash;
                this.AnimationTheme = AnimationTheme.GrowingAndFading;
                this.AnimationDuration = 6;
                this.RepeatAnimation = false;
            }
            #endregion

            #region Rendering Params
            this.BackGradientType = GradientType.TopBottom;
            this.BorderStyle = ChartDashStyle.Solid;
            this.BorderLineColor = (Color)_colorConverter.ConvertFrom(_chartBorderLineColor);
            this.BorderLineWidth = 2;
            #endregion


            #region Get Data
            object[,] tab = _engine.GetChartData();
            if (tab == null || tab.Length == 0)
            {
                this.Visible = false;
                return;
            }
            #endregion

            #region couleurs des tranches du graphique
            string[] pieColorsList = _pieColors.Split(',');
            Color[] pieColors = new Color[7];
            int indexPieColors = 0;

            foreach (string pieColorsString in pieColorsList)
            {
                pieColors.SetValue((Color)_colorConverter.ConvertFrom(pieColorsString), indexPieColors++);
            }
            #endregion

            #region Niveau de détail
            int MEDIA_LEVEL_NUMBER = GetMediaLevelNumber();
            #endregion

            #region Chart Design
            this.Width = new Unit(""+WebApplicationParameters.CustomStyles.ChartMediaStrategyWidth+"px");
            this.Height = new Unit("" + WebApplicationParameters.CustomStyles.ChartMediaStrategyHeight + "px");
            this.Legend.Enabled = false;
            #endregion

            #region Parcours de tab

            #region Variables
            Dictionary<string, Series> listSeriesMedia = new Dictionary<string, Series>();
            Dictionary<int, string> listSeriesName = new Dictionary<int, string>();
            Dictionary<string, double> listSeriesMediaRefCompetitor = new Dictionary<string, double>();
            Dictionary<string, DataTable> listTableRefCompetitor = new Dictionary<string, DataTable>();
            #endregion

            #region Create Series
            // Univers Serie
            listSeriesMedia.Add(GestionWeb.GetWebWord(1780, _session.SiteLanguage), new Series());
            listSeriesName.Add(0, GestionWeb.GetWebWord(1780, _session.SiteLanguage));
            // Market or Sector Serie
            if (_session.ComparaisonCriterion == CstComparisonCriterion.sectorTotal)
            {
                listSeriesMedia.Add(GestionWeb.GetWebWord(1189, _session.SiteLanguage), new Series());
                listSeriesName.Add(1, GestionWeb.GetWebWord(1189, _session.SiteLanguage));
            }
            else
            {
                listSeriesMedia.Add(GestionWeb.GetWebWord(1316, _session.SiteLanguage), new Series());
                listSeriesName.Add(1, GestionWeb.GetWebWord(1316, _session.SiteLanguage));
            }


            // Create series (one per media)
            CreatesSeries(tab, listSeriesMediaRefCompetitor, listTableRefCompetitor, listSeriesMedia);
            #endregion

            #region Totals
            double totalUniversValue = 0;
            double totalSectorValue = 0;
            double totalMarketValue = 0;

            ComputeTotals(tab, listSeriesMediaRefCompetitor, ref  totalUniversValue, ref  totalSectorValue, ref  totalMarketValue, MEDIA_LEVEL_NUMBER);
            #endregion

            #region Table
            DataTable tableUnivers = new DataTable();
            DataTable tableSectorMarket = new DataTable();
            FillTable(tab, listSeriesMediaRefCompetitor, listTableRefCompetitor, tableUnivers, tableSectorMarket, ref  totalUniversValue, ref totalSectorValue, ref  totalMarketValue, MEDIA_LEVEL_NUMBER, withPluriByCategory);
            #endregion

            #endregion



            //Init series
            int index = 0;
            string strSort = "Position  DESC";
            DataRow[] foundRows = null;
            foundRows = tableUnivers.Select("", strSort);
            DataRow[] foundRowsSectorMarket = null;
            foundRowsSectorMarket = tableSectorMarket.Select("", strSort);
            double[] yValues = new double[foundRows.Length];
            string[] xValues = new string[foundRows.Length];
            double[] yValuesSectorMarket = new double[foundRowsSectorMarket.Length];
            string[] xValuesSectorMarket = new string[foundRowsSectorMarket.Length];
            InitSeries(tableUnivers, tableSectorMarket, yValues, xValues, yValuesSectorMarket, xValuesSectorMarket, listSeriesMedia, listTableRefCompetitor, listSeriesName, MEDIA_LEVEL_NUMBER);

            #region Design charts
            index = 0;
            decimal dec = 0;
            int pCount = 0;
            int nbLeftElements = 0, nbRigthElements = 1;
            if (listSeriesMedia != null && listSeriesMedia.Count > 0)
            {
                for (int pc = 0; pc < listSeriesMedia.Count; pc++)
                {
                    if (listSeriesMedia[listSeriesName[pc]].Points.Count > 0)
                        pCount++;
                }
            }
            dec = pCount / (decimal)2;


            for (int j = 0; j < listSeriesMedia.Count; j++)
            {
                if (listSeriesMedia[listSeriesName[j]].Points.Count > 0)
                {

                    #region Type of chart
                    listSeriesMedia[listSeriesName[j]].Type = SeriesChartType.Pie;
                    #endregion

                    #region Colors
                    for (int l = 0; l < 6 && l < listSeriesMedia[listSeriesName[j]].Points.Count; l++)
                    {
                        listSeriesMedia[listSeriesName[j]].Points[l].Color = pieColors[l];
                    }
                    #endregion

                    #region Lengend
                    listSeriesMedia[listSeriesName[j]]["LabelStyle"] = "Outside";
                    listSeriesMedia[listSeriesName[j]].LegendToolTip = "#PERCENT";
                    listSeriesMedia[listSeriesName[j]].ToolTip = " " + listSeriesName[j] + " \n #VALX : #PERCENT";
                    listSeriesMedia[listSeriesName[j]]["PieLineColor"] = _pieLineColor;
                    listSeriesMedia[listSeriesName[j]].Font = new Font("Arial", (float)7);
                    #endregion

                    #region Create Chart
                    ChartArea chartArea2 = new ChartArea();
                    this.ChartAreas.Add(chartArea2);
                    chartArea2.Area3DStyle.Enable3D = true;
                    chartArea2.Name = listSeriesName[j];
                    listSeriesMedia[listSeriesName[j]].ChartArea = chartArea2.Name;
                    #endregion

                    #region Title
                    this.Titles.Add(chartArea2.Name);
                    this.Titles[index].DockInsideChartArea = true;
                    this.Titles[index].Position.Auto = true;
                    this.Titles[index].Font = (pCount < 15) ? new Font("Arial", (float)12) : new Font("Arial", (float)8);
                    this.Titles[index].Color = (Color)_colorConverter.ConvertFrom(_titleColor);
                    this.Titles[index].DockToChartArea = chartArea2.Name;
                    #endregion

                    #region Type image
                    listSeriesMedia[listSeriesName[j]].SmartLabels.Enabled = true;
                    listSeriesMedia[listSeriesName[j]].Label = "#VALX \n #PERCENT";
                    listSeriesMedia[listSeriesName[j]]["3DLabelLineSize"] = "50";
                    if (pCount > 4) listSeriesMedia[listSeriesName[j]]["MinimumRelativePieSize"] = "50";//70
                    else listSeriesMedia[listSeriesName[j]]["MinimumRelativePieSize"] = "45";//45	
                    if (j == 0 && _chartType != ChartImageType.Flash)
                    {
                        this.BackImage = string.Format("/App_themes/{0}{1}", WebApplicationParameters.Themes[_session.SiteLanguage].Name, TNS.AdExpress.Constantes.Web.Images.LOGO_TNS_2);
                        this.BackImageAlign = ChartImageAlign.BottomRight;
                        this.BackImageMode = ChartImageWrapMode.Unscaled;

                    }
                    #endregion

                    #region Positionnement du graphique
                    if (pCount > 4)
                    {
                        chartArea2.Position.Auto = false;
                        chartArea2.Position.X = (index % 2 == 0) ? 2 : 52;
                        chartArea2.Position.Y = (index % 2 == 0) ? (3 + (((96 / (float)Math.Ceiling(dec)) * nbLeftElements) + 1)) : (3 + (((96 / (float)Math.Ceiling(dec)) * (nbRigthElements - 1)) + 1));
                        chartArea2.Position.Width = (pCount > 15) ? 47 : 43;
                        chartArea2.Position.Height = ((96 / (float)Math.Ceiling(dec)) - 1);
                        chartArea2.Area3DStyle.PointDepth = (pCount > 10) ? 10 : 40;
                    }
                    else
                    {
                        chartArea2.Position.Auto = true;
                        chartArea2.Area3DStyle.PointDepth = 45;
                    }
                    chartArea2.Area3DStyle.Enable3D = true;
                    chartArea2.Area3DStyle.XAngle = 20;
                    #endregion

                    if (index % 2 == 0) nbLeftElements++;
                    else nbRigthElements++;
                    index++;

                    this.Series.Add(listSeriesMedia[listSeriesName[j]]);
                }
            }

            #region Dimensionnement de l'image
            ////// Taille d'un graphique * Nombre de graphique
            if (pCount > 4)
            {
                this.Height = (pCount > 12) ? ((pCount > 20) ? new Unit("3000") : new Unit("2500")) : new Unit("1100");
                this.Width = (pCount > 12) ? new Unit("1500") : new Unit("1100");
            }
            else if (pCount < 2)
            {
                this.Height = new Unit("600");
            }
            #endregion

            #region Copyright & Logo
            if (this._chartType != ChartImageType.Flash)
            {
                Title title = new Title("" + GestionWeb.GetWebWord(2848, _session.SiteLanguage) + " "
                                           + WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CompanyNameTexts.GetCompanyShortName(_session.SiteLanguage) + " "
                                           + GestionWeb.GetWebWord(2849, _session.SiteLanguage) + "");
                title.Font = new Font("Arial", (float)8);
                title.DockInsideChartArea = false;
                title.Docking = Docking.Bottom;
                this.Titles.Add(title);
            }
            #endregion

            #endregion

        }
        #endregion

        #region Init Engine
        /// <summary>
        /// Init Engine
        /// </summary>
        virtual protected void InitEngine()
        {
            _engine = new EngineMediaStrategy(this._session, this._dalLayer);
        }
        #endregion


        /// <summary>
        /// Get Media Level Number
        /// </summary>
        /// <returns>Media Level Number</returns>
        protected virtual int GetMediaLevelNumber()
        {
            switch (_session.PreformatedMediaDetail)
            {
                case CstPreformatedDetail.PreformatedMediaDetails.vehicle:
                    return 1;
                case CstPreformatedDetail.PreformatedMediaDetails.vehicleRegion:
                    return 2;
                default:
                    return 3;
            }
        }

        #region ComputeTotals
        /// <summary>
        /// Compute totals values
        /// </summary>
        /// <param name="tab"></param>
        /// <param name="listSeriesMediaRefCompetitor"></param>
        /// <param name="totalUniversValue"></param>
        /// <param name="totalSectorValue"></param>
        /// <param name="totalMarketValue"></param>
        /// <param name="MEDIA_LEVEL_NUMBER"></param>
        protected virtual void ComputeTotals(object[,] tab, Dictionary<string, double> listSeriesMediaRefCompetitor, ref double totalUniversValue, ref double totalSectorValue, ref double totalMarketValue, int MEDIA_LEVEL_NUMBER)
        {
            #region Once Media
            if (MEDIA_LEVEL_NUMBER == 2 || MEDIA_LEVEL_NUMBER == 3)
            {
                for (int i = 1; i < tab.GetLongLength(0); i++)
                {
                    for (int j = 0; j < EngineMediaStrategy.NB_MAX_COLUMNS; j++)
                    {
                        switch (j)
                        {

                            #region support
                            // Univers Total
                            case EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX:
                                if (tab[i, EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 3)
                                {
                                    totalUniversValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            // Sector Total
                            case EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX:
                                if (tab[i, EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 3)
                                {
                                    totalSectorValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            // Market Total
                            case EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX:
                                if (tab[i, EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 3)
                                {
                                    totalMarketValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            #endregion

                            #region Advertisers
                            case EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX:
                                if (tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] != null
                                    && tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX] != null
                                    && tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null
                                    && MEDIA_LEVEL_NUMBER == 3)
                                {
                                    listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] += Convert.ToDouble(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]);
                                }
                                else if (tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] != null
                                    && tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX] != null
                                    && tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null
                                    && tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX] == null
                                    && MEDIA_LEVEL_NUMBER == 2)
                                {
                                    listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] += Convert.ToDouble(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            #endregion

                            #region Category
                            // Univers Total	
                            case EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX:
                                if (tab[i, EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 2)
                                {
                                    totalUniversValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            // Sector Total
                            case EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX:
                                if (tab[i, EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 2)
                                {
                                    totalSectorValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            // Market Total
                            case EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX:

                                if (tab[i, EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 2)
                                {
                                    totalMarketValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX]);
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
            if (MEDIA_LEVEL_NUMBER == 1)
            {
                for (int i = 0; i < tab.GetLongLength(0); i++)
                {
                    for (int j = 0; j < EngineMediaStrategy.NB_MAX_COLUMNS; j++)
                    {
                        switch (j)
                        {
                            case EngineMediaStrategy.TOTAL_UNIV_INVEST_COLUMN_INDEX:
                                if (tab[i, EngineMediaStrategy.TOTAL_UNIV_INVEST_COLUMN_INDEX] != null)
                                {
                                    totalUniversValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            case EngineMediaStrategy.TOTAL_SECTOR_INVEST_COLUMN_INDEX:
                                if (tab[i, EngineMediaStrategy.TOTAL_SECTOR_INVEST_COLUMN_INDEX] != null)
                                {
                                    totalSectorValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            case EngineMediaStrategy.TOTAL_MARKET_INVEST_COLUMN_INDEX:
                                if (tab[i, EngineMediaStrategy.TOTAL_MARKET_INVEST_COLUMN_INDEX] != null)
                                {
                                    totalMarketValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            case EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX:
                                if (tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] != null
                                    && tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null
                                    && tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX] != null)
                                {
                                    listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] += Convert.ToDouble(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]);
                                }

                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            #endregion
        }
        #endregion

        #region Fill Table
        protected virtual void FillTable(object[,] tab, Dictionary<string, double> listSeriesMediaRefCompetitor, Dictionary<string, DataTable> listTableRefCompetitor, DataTable tableUnivers, DataTable tableSectorMarket, ref double totalUniversValue, ref double totalSectorValue, ref double totalMarketValue, int MEDIA_LEVEL_NUMBER, bool withPluriByCategory)
        {
            #region Table
            double elementValue;
            //DataTable tableUnivers = new DataTable();
            //DataTable tableSectorMarket = new DataTable();
            // Define columns
            tableUnivers.Columns.Add("Name");
            tableUnivers.Columns.Add("Position", typeof(double));
            tableSectorMarket.Columns.Add("Name");
            tableSectorMarket.Columns.Add("Position", typeof(double));

            for (int i = 1; i < tab.GetLongLength(0); i++)
            {
                for (int j = 0; j < EngineMediaStrategy.NB_MAX_COLUMNS; j++)
                {
                    switch (j)
                    {

                        #region Media
                        case EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX:
                            if (tab[i, EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 3)
                            {
                                if (totalUniversValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX]) / totalUniversValue * 100;
                                    DataRow row = tableUnivers.NewRow();
                                    row["Name"] = tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX];
                                    row["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableUnivers.Rows.Add(row);
                                }
                                j = j + 6;
                            }

                            break;
                        case EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX:
                            if (tab[i, EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 3)
                            {
                                if (totalSectorValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX]) / totalSectorValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableSectorMarket.Rows.Add(row1);
                                }
                                j = j + 5;
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX:
                            if (tab[i, EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 3)
                            {
                                if (totalMarketValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX]) / totalMarketValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableSectorMarket.Rows.Add(row1);
                                }
                                j = j + 4;
                            }
                            break;
                        #endregion

                        #region Advertisers
                        case EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX:
                            if (tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] != null
                                && tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX] != null
                                && tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null
                                && MEDIA_LEVEL_NUMBER == 3)
                            {

                                if (listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]) / listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] * 100;
                                    DataRow row1 = listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].Rows.Add(row1);
                                }

                                j = j + 12;
                            }
                            else if (tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] != null
                                && tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX] != null
                                && tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX] == null
                                && tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null
                                && MEDIA_LEVEL_NUMBER == 2)
                            {

                                if (listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]) / listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] * 100;
                                    DataRow row1 = listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].Rows.Add(row1);
                                }
                                j = j + 12;
                            }
                            else if (tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] != null
                             && tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null
                             && tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX] != null
                             && MEDIA_LEVEL_NUMBER == 1)
                            {
                                if (listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]) / listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] * 100;
                                    DataRow row1 = listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].Rows.Add(row1);
                                }
                                j = j + 12;
                            }
                            break;
                        #endregion

                        #region Categorie
                        case EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX:
                            if (tab[i, EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 2)
                            {
                                if (totalUniversValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX]) / totalUniversValue * 100;
                                    DataRow row = tableUnivers.NewRow();
                                    row["Name"] = tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX];
                                    row["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableUnivers.Rows.Add(row);
                                }
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX:
                            if (tab[i, EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 2)
                            {
                                if (totalSectorValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX]) / totalSectorValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableSectorMarket.Rows.Add(row1);
                                }
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX:
                            if (tab[i, EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 2)
                            {
                                if (totalMarketValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX]) / totalMarketValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableSectorMarket.Rows.Add(row1);
                                }
                            }
                            break;
                        #endregion

                        #region PluriMedia
                        case EngineMediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX:
                            if (tab[i, EngineMediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX] != null && i > 1 && !withPluriByCategory)
                            {
                                if (totalUniversValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX]) / totalUniversValue * 100;
                                    DataRow row = tableUnivers.NewRow();
                                    row["Name"] = tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX];
                                    row["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableUnivers.Rows.Add(row);
                                }
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX:
                            if (tab[i, EngineMediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX] != null && i > 1 && !withPluriByCategory)
                            {
                                if (totalSectorValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX]) / totalSectorValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableSectorMarket.Rows.Add(row1);
                                }
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX:
                            if (tab[i, EngineMediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX] != null && i > 1 && !withPluriByCategory)
                            {
                                if (totalMarketValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX]) / totalMarketValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX];
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
        }
        #endregion

        #region  Init Series
        protected virtual void InitSeries(DataTable tableUnivers, DataTable tableSectorMarket, double[] yValues, string[] xValues, double[] yValuesSectorMarket, string[] xValuesSectorMarket, Dictionary<string, Series> listSeriesMedia, Dictionary<string, DataTable> listTableRefCompetitor, Dictionary<int, string> listSeriesName, int MEDIA_LEVEL_NUMBER)
        {
            #region Init Series
            string strSort = "Position  DESC";
            DataRow[] foundRows = null;
            foundRows = tableUnivers.Select("", strSort);
            DataRow[] foundRowsSectorMarket = null;
            foundRowsSectorMarket = tableSectorMarket.Select("", strSort);
            //double[] yValues = new double[foundRows.Length];
            //string[] xValues = new string[foundRows.Length];
            //double[] yValuesSectorMarket = new double[foundRowsSectorMarket.Length];
            //string[] xValuesSectorMarket = new string[foundRowsSectorMarket.Length];
            double otherUniversValue = 0;
            double otherSectorMarketValue = 0;
            int index = 0;

            if (MEDIA_LEVEL_NUMBER != 1)
            {
                for (int i = 0; i < NBRE_MEDIA && i < foundRows.Length; i++)
                {
                    xValues[i] = foundRows[i]["Name"].ToString();
                    yValues[i] = Convert.ToDouble(foundRows[i]["Position"]);
                    otherUniversValue += Convert.ToDouble(foundRows[i]["Position"]);
                    index = i + 1;
                }
                if (foundRows.Length > NBRE_MEDIA)
                {
                    xValues[index] = GestionWeb.GetWebWord(2566, _session.SiteLanguage);
                    yValues[index] = 100 - otherUniversValue;
                }

                for (int i = 0; i < NBRE_MEDIA && i < foundRowsSectorMarket.Length; i++)
                {
                    xValuesSectorMarket[i] = foundRowsSectorMarket[i]["Name"].ToString();
                    yValuesSectorMarket[i] = Convert.ToDouble(foundRowsSectorMarket[i]["Position"]);
                    otherSectorMarketValue += Convert.ToDouble(foundRowsSectorMarket[i]["Position"]);
                    index = i + 1;
                }
                if (foundRowsSectorMarket.Length > NBRE_MEDIA)
                {
                    xValuesSectorMarket[index] = GestionWeb.GetWebWord(2566, _session.SiteLanguage);
                    yValuesSectorMarket[index] = 100 - otherSectorMarketValue;
                }
            }
            // Cas PluriMedia
            else
            {
                for (int i = 0; i < foundRows.Length; i++)
                {
                    xValues[i] = foundRows[i]["Name"].ToString();
                    yValues[i] = Convert.ToDouble(foundRows[i]["Position"]);
                    otherUniversValue += Convert.ToDouble(foundRows[i]["Position"]);
                }

                for (int i = 0; i < foundRowsSectorMarket.Length; i++)
                {
                    xValuesSectorMarket[i] = foundRowsSectorMarket[i]["Name"].ToString();
                    yValuesSectorMarket[i] = Convert.ToDouble(foundRowsSectorMarket[i]["Position"]);
                    otherSectorMarketValue += Convert.ToDouble(foundRowsSectorMarket[i]["Position"]);
                }
            }

            double[] yVal = new double[foundRows.Length];
            string[] xVal = new string[foundRows.Length];
            double otherCompetitorRefValue = 0;
            int k = 2;

            foreach (string name in listSeriesMedia.Keys)
            {

                if (name == GestionWeb.GetWebWord(1780, _session.SiteLanguage))
                {
                    if (xValues != null && xValues.Length > 0 && xValues[0] != null)
                        listSeriesMedia[GestionWeb.GetWebWord(1780, _session.SiteLanguage)].Points.DataBindXY(xValues, yValues);
                }
                else if (_session.ComparaisonCriterion == CstComparisonCriterion.sectorTotal && name == GestionWeb.GetWebWord(1189, _session.SiteLanguage))
                {
                    if (xValuesSectorMarket != null && xValuesSectorMarket.Length > 0 && xValuesSectorMarket[0] != null)
                        listSeriesMedia[GestionWeb.GetWebWord(1189, _session.SiteLanguage)].Points.DataBindXY(xValuesSectorMarket, yValuesSectorMarket);
                }
                else if (name == GestionWeb.GetWebWord(1316, _session.SiteLanguage))
                {
                    if (xValuesSectorMarket != null && xValuesSectorMarket.Length > 0 && xValuesSectorMarket[0] != null)
                        listSeriesMedia[GestionWeb.GetWebWord(1316, _session.SiteLanguage)].Points.DataBindXY(xValuesSectorMarket, yValuesSectorMarket);
                }
                else
                {
                    DataRow[] foundRowsCompetitorRef = null;
                    foundRowsCompetitorRef = ((DataTable)listTableRefCompetitor[name]).Select("", strSort);
                    otherCompetitorRefValue = 0;

                    yVal = new double[foundRowsCompetitorRef.Length];
                    xVal = new string[foundRowsCompetitorRef.Length];
                    if (MEDIA_LEVEL_NUMBER != 1)
                    {
                        for (int i = 0; i < foundRowsCompetitorRef.Length && i < NBRE_MEDIA; i++)
                        {


                            xVal[i] = foundRowsCompetitorRef[i]["Name"].ToString();
                            yVal[i] = Convert.ToDouble(foundRowsCompetitorRef[i]["Position"]);

                            otherCompetitorRefValue += Convert.ToDouble(foundRowsCompetitorRef[i]["Position"]);
                            index = i + 1;
                        }
                        if (foundRowsCompetitorRef.Length > NBRE_MEDIA)
                        {
                            xVal[index] = GestionWeb.GetWebWord(2566, _session.DataLanguage);
                            yVal[index] = 100 - otherCompetitorRefValue;
                        }
                    }
                    // PluriMedia
                    else
                    {
                        for (int i = 0; i < foundRowsCompetitorRef.Length; i++)
                        {
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
        }
        #endregion

        protected virtual void CreatesSeries(object[,] tab, Dictionary<string, double> listSeriesMediaRefCompetitor, Dictionary<string, DataTable> listTableRefCompetitor, Dictionary<string, Series> listSeriesMedia)
        {
            // Create series (one per media)
            for (int i = 1; i < tab.GetLongLength(0); i++)
            {

                //	Dictionary with advertiser label as key and total as value
                if (tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null)
                {
                    if (!listSeriesMediaRefCompetitor.ContainsKey(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()))
                    {
                        listSeriesMediaRefCompetitor.Add(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString(), new double());
                    }

                    if (!listTableRefCompetitor.ContainsKey(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()))
                    {
                        DataTable tableCompetitorRef = new DataTable();
                        tableCompetitorRef.Columns.Add("Name");
                        tableCompetitorRef.Columns.Add("Position", typeof(double));
                        listTableRefCompetitor.Add(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString(), tableCompetitorRef);

                    }

                    if (!listSeriesMedia.ContainsKey(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()))
                    {
                        listSeriesMedia.Add(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString(), new Series());
                    }
                }
            }
        }
    }

}
