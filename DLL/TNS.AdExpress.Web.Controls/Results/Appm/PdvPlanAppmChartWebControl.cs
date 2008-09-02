#region Informations
// Auteur: D. Mussuma 
// Date de création: 28/07/2006
// Date de modification:
#endregion

using System;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Rules.Results.APPM;
using TNS.AdExpress.Constantes.Customer; 
using WebConstantes = TNS.AdExpress.Constantes.Web;

using TNS.FrameWork.DB.Common;
using Dundas.Charting.WebControl;
using TNS.FrameWork;
using TNS.AdExpress.Domain.Units;

namespace TNS.AdExpress.Web.Controls.Results.Appm {
    /// <summary>
    /// Sert de classe  pour des contrôles graphiques d'analyse des parts de voix de l'APPM.
    /// </summary>
    [DefaultProperty("Text"),
    ToolboxData("<{0}:PdvPlanAppmChartWebControl runat=server></{0}:PdvPlanAppmChartWebControl>")]
    public class PdvPlanAppmChartWebControl : BaseAppmChartWebControl {

        #region Variables
        /// <summary>
        /// Valeur hexadecimale des couleurs des portions de camembert separé par le caractere ','
        /// </summary>
        private string _strPieColors = string.Empty;
        /// <summary>
        /// Famille de police du texte lorsqu'il n'y a aucun resultat
        /// </summary>
        private string _textNoResultFontFamily = "Arial";
        /// <summary>
        /// Taille de police du texte lorsqu'il n'y a aucun resultat
        /// </summary>
        private string _textNoResultFontSize = "8";
        /// <summary>
        /// Couleur de police du texte lorsqu'il n'y a aucun resultat
        /// </summary>
        private string _textNoResultFontColor = "#644883";
        /// <summary>
        /// Couleur de police du Camembert Euros
        /// </summary>
        private static string _camembertEurosFontColor = "#644883";
        /// <summary>
        /// Taille de police du Camembert Euros
        /// </summary>
        private static string _camembertEurosFontSize = "12";
        /// <summary>
        /// Famille de police du Camembert Euros
        /// </summary>
        private static string _camembertEurosFontFamily = "Arial";
        /// <summary>
        /// Couleur de police du Camembert Pages
        /// </summary>
        private static string _camembertPagesFontColor = "#644883";
        /// <summary>
        /// Taille de police du Camembert Pages
        /// </summary>
        private static string _camembertPagesFontSize = "12";
        /// <summary>
        /// Famille de police du Camembert Pages
        /// </summary>
        private static string _camembertPagesFontFamily = "Arial";
        /// <summary>
        /// Couleur de police du Camembert Insertions
        /// </summary>
        private static string _camembertInsertionsFontColor = "#644883";
        /// <summary>
        /// Taille de police du Camembert Insertions
        /// </summary>
        private static string _camembertInsertionsFontSize = "12";
        /// <summary>
        /// Famille de police du Camembert Insertions
        /// </summary>
        private static string _camembertInsertionsFontFamily = "Arial";
        /// <summary>
        /// Couleur de police du Camembert GRP
        /// </summary>     
        private static string _camembertGRPFontColor = "#644883";
        /// <summary>
        /// Taille de police du Camembert GRP
        /// </summary>
        private static string _camembertGRPFontSize = "12";
        /// <summary>
        /// Famille de police du Camembert GRP
        /// </summary>
        private static string _camembertGRPFontFamily = "Arial";
        /// <summary>
        /// Taille de la bordure du controle global
        /// </summary>
        private static string _controlBorderSize = "2";
        /// <summary>
        /// Couleur de la bordure du controle global
        /// </summary>
        private static string _controlBorderColor = "#634984";
        #endregion

        #region Assesseur
        /// <summary>
        /// Obtient ou definis la Valeur hexadecimale des couleurs des portions de camembert separé par le caractere ','
        /// </summary>
        public string PieColors {
            get { return _strPieColors; }
            set { _strPieColors = value; }
        }
        /// <summary>
        /// Obtient ou definis la Famille de police du texte lorsqu'il n'y a aucun resultat
        /// </summary>
        public string TextNoResultFontFamily {
            get { return _textNoResultFontFamily; }
            set { _textNoResultFontFamily = value; }
        }
        /// <summary>
        /// Obtient ou definis la Taille de police du texte lorsqu'il n'y a aucun resultat
        /// </summary>
        public string TextNoResultFontSize {
            get { return _textNoResultFontSize; }
            set { _textNoResultFontSize = value; }
        }
        /// <summary>
        /// Obtient ou definis la Couleur de police du texte lorsqu'il n'y a aucun resultat 
        /// </summary>
        public string TextNoResultFontColor {
            get { return _textNoResultFontColor; }
            set { _textNoResultFontColor = value; }
        }
        /// <summary>
        /// Obtient ou definis la Couleur de police du Camembert Euros
        /// </summary>
        public string CamembertEurosFontColor {
            get { return _camembertEurosFontColor; }
            set { _camembertEurosFontColor = value; }
        }
        /// <summary>
        /// Obtient ou definis la Taille de police du Camembert Euros
        /// </summary>
        public string CamembertEurosFontSize {
            get { return _camembertEurosFontSize; }
            set { _camembertEurosFontSize = value; }
        }
        /// <summary>
        /// Obtient ou definis la Famille de police du Camembert Euros
        /// </summary>
        public string CamembertEurosFontFamily {
            get { return _camembertEurosFontFamily; }
            set { _camembertEurosFontFamily = value; }
        }
        /// <summary>
        /// Obtient ou definis la Couleur de police du Camembert Pages
        /// </summary>
        public string CamembertPagesFontColor {
            get { return _camembertPagesFontColor; }
            set { _camembertPagesFontColor = value; }
        }
        /// <summary>
        /// Obtient ou definis la Taille de police du Camembert Pages
        /// </summary>
        public string CamembertPagesFontSize {
            get { return _camembertPagesFontSize; }
            set { _camembertPagesFontSize = value; }
        }
        /// <summary>
        /// Obtient ou definis la Famille de police du Camembert Pages
        /// </summary>
        public string CamembertPagesFontFamily {
            get { return _camembertPagesFontFamily; }
            set { _camembertPagesFontFamily = value; }
        }
        /// <summary>
        /// Obtient ou definis la Couleur de police du Camembert Insertions
        /// </summary>
        public string CamembertInsertionsFontColor {
            get { return _camembertInsertionsFontColor; }
            set { _camembertInsertionsFontColor = value; }
        }
        /// <summary>
        /// Obtient ou definis la Taille de police du Camembert Insertions
        /// </summary>
        public string CamembertInsertionsFontSize {
            get { return _camembertInsertionsFontSize; }
            set { _camembertInsertionsFontSize = value; }
        }
        /// <summary>
        /// Obtient ou definis la Famille de police du Camembert Insertions
        /// </summary>
        public string CamembertInsertionsFontFamily {
            get { return _camembertInsertionsFontFamily; }
            set { _camembertInsertionsFontFamily = value; }
        }
        /// <summary>
        /// Obtient ou definis la Couleur de police du Camembert GRP
        /// </summary>
        public string CamembertGRPFontColor {
            get { return _camembertGRPFontColor; }
            set { _camembertGRPFontColor = value; }
        }
        /// <summary>
        /// Obtient ou definis la Taille de police du Camembert GRP
        /// </summary>
        public string CamembertGRPFontSize {
            get { return _camembertGRPFontSize; }
            set { _camembertGRPFontSize = value; }
        }
        /// <summary>
        /// Obtient ou definis la Famille de police du Camembert GRP
        /// </summary>
        public string CamembertGRPFontFamily {
            get { return _camembertGRPFontFamily; }
            set { _camembertGRPFontFamily = value; }
        }
        /// <summary>
        /// Obtient ou definis la Taille de la bordure du controle global
        /// </summary>
        public string ControlBorderSize {
            get { return _controlBorderSize; }
            set { _controlBorderSize = value; }
        }
        /// <summary>
        /// Obtient ou definis la Couleur de la bordure du controle global
        /// </summary>
        public string ControlBorderColor {
            get { return _controlBorderColor; }
            set { _controlBorderColor = value; }
        }
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="dataSource">Source de données</param>
        /// <param name="appmImageType">Type de l'image Appm (jpg, flash...)</param>
        public PdvPlanAppmChartWebControl(WebSession webSession, TNS.FrameWork.DB.Common.IDataSource dataSource, ChartImageType appmImageType)
            : base(webSession, dataSource, appmImageType) {
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="dataSource">Source de données</param>
        /// <param name="appmImageType">Type de l'image Appm (jpg, flash...)</param>
        /// <param name="skinId">Nom du skin</param>
        public PdvPlanAppmChartWebControl(WebSession webSession, TNS.FrameWork.DB.Common.IDataSource dataSource, ChartImageType appmImageType, string skinId)
            : base(webSession, dataSource, appmImageType) {
            this.SkinID = skinId;
        }
        #endregion

        #region Implémentation des méthodes abstraites
        /// <summary>		
        /// Définit les données au moment du design du contrôle. 	
        /// </summary>
        public override void SetDesignMode() {

            #region variables
            double[] yUnitValues = null;
            string[] xUnitValues = null;
            string chartAreaName = string.Empty;
            #endregion

            #region Chargement des données
            DataTable PDVPlanData = PDVPlanRules.GetData(this._customerWebSession, this._dataSource, this._dateBegin, this._dateEnd, this._idBaseTarget, this._idAdditionalTarget, true);
            DataTable PDVGraphicsData = PDVPlanRules.GetGraphicsData(this._customerWebSession, this._dataSource, this._dateBegin, this._dateEnd, this._idAdditionalTarget);
            #endregion

            #region Couleurs
            ColorConverter cc = new ColorConverter();

            #region No Result
            Color textNoResultFontColor = (Color)cc.ConvertFromString(_textNoResultFontColor);
            #endregion

            #region colors of the pie chart
            Color[] pieColors = null;
            if (_strPieColors != null && _strPieColors.Length != 0) {
                string[] colorTemp = _strPieColors.Split(",".ToCharArray());
                pieColors = new Color[colorTemp.Length];
                for (int i = 0; i < colorTemp.Length; i++) {
                    pieColors[i] = (Color)cc.ConvertFromString(colorTemp[i]);
                }
            }
            else {

                pieColors = WebConstantes.UI.UI.newPieColors;
            }
            #endregion

            #endregion

            #region Construction du  graphique

            if (PDVPlanData != null && PDVPlanData.Rows.Count > 0 && PDVGraphicsData != null && PDVGraphicsData.Rows.Count > 0) {
                yUnitValues = new double[PDVGraphicsData.Rows.Count + 1];
                xUnitValues = new string[PDVGraphicsData.Rows.Count + 1];

                #region euros
                //Creates chart area and series for euros
                GetSeriesData(PDVGraphicsData, PDVPlanData, xUnitValues, yUnitValues, "euros");
                chartAreaName = GestionWeb.GetWebWord(1423, this._customerWebSession.SiteLanguage);
                ChartArea chartAreaEuro = new ChartArea();
                Series euroSeries = new Series();
                //Allignments of the chart
                chartAreaEuro.AlignOrientation = AreaAlignOrientation.Vertical;
                chartAreaEuro.Position.X = 2;
                chartAreaEuro.Position.Y = 2;
                chartAreaEuro.Position.Width = 90;
                chartAreaEuro.Position.Height = 23;
                this.ChartAreas.Add(chartAreaEuro);
                SeriesPDVPlan(PDVGraphicsData, chartAreaEuro, euroSeries, xUnitValues, yUnitValues, pieColors, chartAreaName);
                #endregion

                #region pages
                //Creates chart area and series for pages
                GetSeriesData(PDVGraphicsData, PDVPlanData, xUnitValues, yUnitValues, "pages");
                chartAreaName = GestionWeb.GetWebWord(UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].WebTextId, _customerWebSession.SiteLanguage);
                ChartArea chartAreaPages = new ChartArea();
                Series pageSeries = new Series();
                //Allignments of the chart
                chartAreaPages.AlignOrientation = AreaAlignOrientation.Vertical;
                chartAreaPages.Position.X = 2;
                chartAreaPages.Position.Y = 27;
                chartAreaPages.Position.Width = 90;
                chartAreaPages.Position.Height = 23;
                this.ChartAreas.Add(chartAreaPages);
                SeriesPDVPlan(PDVGraphicsData, chartAreaPages, pageSeries, xUnitValues, yUnitValues, pieColors, chartAreaName);
                #endregion

                #region insertions
                //Creates chart area and series for insertions
                GetSeriesData(PDVGraphicsData, PDVPlanData, xUnitValues, yUnitValues, "insertions");
                chartAreaName = Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].WebTextId, _customerWebSession.SiteLanguage));
                ChartArea chartAreaInsertions = new ChartArea();
                Series insertionSeries = new Series();
                //Allignments of the chart
                chartAreaInsertions.AlignOrientation = AreaAlignOrientation.Vertical;
                chartAreaInsertions.Position.X = 2;
                chartAreaInsertions.Position.Y = 52;
                chartAreaInsertions.Position.Width = 90;
                chartAreaInsertions.Position.Height = 23;
                this.ChartAreas.Add(chartAreaInsertions);
                SeriesPDVPlan(PDVGraphicsData, chartAreaInsertions, insertionSeries, xUnitValues, yUnitValues, pieColors, chartAreaName);
                #endregion

                #region GRP
                //Creates chart area and series for GRP
                GetSeriesData(PDVGraphicsData, PDVPlanData, xUnitValues, yUnitValues, "GRP");
                chartAreaName = GestionWeb.GetWebWord(1679, this._customerWebSession.SiteLanguage);
                ChartArea chartAreaGRP = new ChartArea();
                Series grpSeries = new Series();
                //Allignments of the chart
                chartAreaGRP.AlignOrientation = AreaAlignOrientation.Vertical;
                chartAreaGRP.Position.X = 2;
                chartAreaGRP.Position.Y = 77;
                chartAreaGRP.Position.Width = 90;
                chartAreaGRP.Position.Height = 22;
                this.ChartAreas.Add(chartAreaGRP);
                SeriesPDVPlan(PDVGraphicsData, chartAreaGRP, grpSeries, xUnitValues, yUnitValues, pieColors, chartAreaName);
                #endregion

                #region Initializing chart and adding series to it
                InitializeComponents(this, chartAreaEuro, chartAreaPages, chartAreaInsertions, chartAreaGRP, this._imageType);
                this.Series.Add(euroSeries);
                this.Series.Add(pageSeries);
                this.Series.Add(insertionSeries);
                this.Series.Add(grpSeries);
                #endregion

                AddCopyRight(this._customerWebSession, (_imageType == ChartImageType.Jpeg));

            }
            else {
                this.Titles.Add(GestionWeb.GetWebWord(2106, this._customerWebSession.SiteLanguage));
                //this.Titles[0].Font=new Font("Arial", (float)8,System.Drawing.FontStyle.Bold);
                //this.Titles[0].Color=Color.FromArgb(100,72,131);
                this.Titles[0].Font = new Font(_textNoResultFontFamily, (float)Convert.ToDouble(_textNoResultFontSize), System.Drawing.FontStyle.Bold);
                this.Titles[0].Color = textNoResultFontColor;
                this.Width = 250;
                this.Height = 20;
            }
            #endregion

        }
        #endregion

        #region Render
        /// <summary> 
        /// Génère ce contrôle dans le paramètre de sortie spécifié.
        /// </summary>
        /// <param name="output"> Le writer HTML vers lequel écrire </param>
        protected override void Render(HtmlTextWriter output) {
            //On cree le design ici pour pouvoir appliquer le skin a ce controle, 
            //Si le design est appelé avant, les membres du skin ne sont pas appliqué a ce controle
            SetDesignMode();
            base.Render(output);
        }
        #endregion

        #region Méthodes privées

        #region Construction des Series de valeurs
        /// <summary>
        /// Construct series for the PDV Plan Charts 
        /// </summary>
        /// <param name="dt">DataTable containing the data for constructing chart</param>
        /// <param name="chartArea">contenant objet graphique</param>
        /// <param name="series">Reference to the series object</param>
        /// <param name="xValues">xValues for the series</param>
        /// <param name="yValues">yValues for the series</param>
        /// <param name="pieColors">colors of the pie chart</param>
        /// <param name="chartAreaName">Name of the chart Area</param>
        private static void SeriesPDVPlan(DataTable dt, ChartArea chartArea, Series series, string[] xValues, double[] yValues, Color[] pieColors, string chartAreaName) {

            if (xValues != null && yValues != null) {
                #region Creation and definitions of the series, labels , legends etc
                //Graphics Type
                series.Type = SeriesChartType.Pie;
                series.SmartLabels.Enabled = true;
                series.XValueType = ChartValueTypes.String;
                series.YValueType = ChartValueTypes.Long;
                series.Enabled = true;
                chartArea.Area3DStyle.Enable3D = true;
                chartArea.Name = chartAreaName;
                series.ChartArea = chartArea.Name;
                //Binding series values
                series.Points.DataBindXY(xValues, yValues);

                #region Defining Colour
                for (int k = 0; k < dt.Rows.Count + 1 && k < pieColors.Length; k++) {
                    series.Points[k].Color = pieColors[k];
                }
                #endregion

                #region Labels and Tooltips
                series["LabelStyle"] = "Outside";
                series.Label = "#VALX \n #PERCENT ";
                series.ToolTip = "#PERCENT #VALX";
                series.Font = new Font("Arial", (float)7);

                #endregion

                #endregion
            }



        }
        #endregion

        #region Initialize Chart
        /// <summary>
        /// This method is used to set the styles for the chart
        /// </summary>
        /// <param name="appmChart">Reference to the chart Object</param>
        /// <param name="chartAreaEuros">Chart Area for the Euro Pie</param>
        /// <param name="chartAreaPages">Chart Area for the Pages Pie</param>
        /// <param name="chartAreaInsertions">Chart Area for the Insertions Pie</param>
        /// <param name="chartAreaGRP">Chart Area for the GRP Pie</param>
        /// <param name="appmImageType">type de l'image</param>
        private static void InitializeComponents(BaseAppmChartWebControl appmChart, ChartArea chartAreaEuros, ChartArea chartAreaPages, ChartArea chartAreaInsertions, ChartArea chartAreaGRP, ChartImageType appmImageType) {
            #region type image

            appmChart.ImageType = appmImageType;
            if (appmImageType == ChartImageType.Flash) {
                appmChart.AnimationTheme = AnimationTheme.MovingFromTop;
                appmChart.AnimationDuration = 3;
                appmChart.RepeatAnimation = false;
            }

            #endregion

            #region Initialisation des couleurs
            ColorConverter cc = new ColorConverter();

            #region No Result
            Color controlBorderColor = (Color)cc.ConvertFromString(_controlBorderColor);
            #endregion

            #region Camembert
            Color camembertEurosFontColor = (Color)cc.ConvertFromString(_camembertEurosFontColor);
            Color camembertPagesFontColor = (Color)cc.ConvertFromString(_camembertPagesFontColor);
            Color camembertInsertionsFontColor = (Color)cc.ConvertFromString(_camembertInsertionsFontColor);
            Color camembertGRPFontColor = (Color)cc.ConvertFromString(_camembertGRPFontColor);
            #endregion

            #endregion

            #region Chart
            //setting dimensions and properties of the chart
            appmChart.Width = new Unit("850");
            appmChart.Height = new Unit("1000");
            appmChart.BackGradientType = GradientType.TopBottom;
            appmChart.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);
            appmChart.BorderStyle = ChartDashStyle.Solid;
            //appmChart.BorderLineColor=Color.FromArgb(99,73,132);
            appmChart.BorderLineWidth = 2;
            appmChart.BorderLineColor = controlBorderColor;
            appmChart.BorderLineWidth = Convert.ToInt32(_controlBorderSize);
            appmChart.Legend.Enabled = false;
            #endregion

            #region Titles
            //Euros			
            appmChart.Titles.Add(chartAreaEuros.Name);
            appmChart.Titles[0].DockInsideChartArea = true;
            appmChart.Titles[0].DockInsideChartArea = true;
            appmChart.Titles[0].Position.Auto = false;
            appmChart.Titles[0].Position.X = 47;
            appmChart.Titles[0].Position.Y = 2;
            //appmChart.Titles[0].Font = new Font("Arial", (float)12);
            //appmChart.Titles[0].Color = Color.FromArgb(100, 72, 131);
            appmChart.Titles[0].Font = new Font(_camembertEurosFontFamily, (float)Convert.ToDouble(_camembertEurosFontSize));
            appmChart.Titles[0].Color = camembertEurosFontColor;
            appmChart.Titles[0].DockToChartArea = chartAreaEuros.Name;

            //pages
            appmChart.Titles.Add(chartAreaPages.Name);
            appmChart.Titles[1].DockInsideChartArea = true;
            appmChart.Titles[1].DockInsideChartArea = true;
            appmChart.Titles[1].Position.Auto = false;
            appmChart.Titles[1].Position.X = 47;
            appmChart.Titles[1].Position.Y = 26;
            //appmChart.Titles[1].Font = new Font("Arial", (float)12);
            //appmChart.Titles[1].Color = Color.FromArgb(100, 72, 131);
            appmChart.Titles[1].Font = new Font(_camembertEurosFontFamily, (float)Convert.ToDouble(_camembertInsertionsFontSize));
            appmChart.Titles[1].Color = camembertPagesFontColor;
            appmChart.Titles[1].DockToChartArea = chartAreaPages.Name;

            //Insertions
            appmChart.Titles.Add(chartAreaInsertions.Name);
            appmChart.Titles[2].DockInsideChartArea = true;
            appmChart.Titles[2].DockInsideChartArea = true;
            appmChart.Titles[2].Position.Auto = false;
            appmChart.Titles[2].Position.X = 47;
            appmChart.Titles[2].Position.Y = 51;
            //appmChart.Titles[2].Font = new Font("Arial", (float)12);
            //appmChart.Titles[2].Color = Color.FromArgb(100, 72, 131);
            appmChart.Titles[2].Font = new Font(_camembertEurosFontFamily, (float)Convert.ToDouble(_camembertInsertionsFontSize));
            appmChart.Titles[2].Color = camembertInsertionsFontColor;
            appmChart.Titles[2].DockToChartArea = chartAreaInsertions.Name;

            //GRP
            appmChart.Titles.Add(chartAreaGRP.Name);
            appmChart.Titles[3].DockInsideChartArea = true;
            appmChart.Titles[3].DockInsideChartArea = true;
            appmChart.Titles[3].Position.Auto = false;
            appmChart.Titles[3].Position.X = 47;
            appmChart.Titles[3].Position.Y = 76;
            //appmChart.Titles[3].Font = new Font("Arial", (float)12);
            //appmChart.Titles[3].Color = Color.FromArgb(100, 72, 131);
            appmChart.Titles[3].Font = new Font(_camembertEurosFontFamily, (float)Convert.ToDouble(_camembertEurosFontSize));
            appmChart.Titles[3].Color = camembertGRPFontColor;
            appmChart.Titles[3].DockToChartArea = chartAreaGRP.Name;
            #endregion
        }
            #endregion

        #region Gets data for the Series
        /// <summary>
        /// This method populates the xValues and Yvalues arrays which are then used to construct the series
        /// </summary>
        /// <param name="graphicsTable">Table that contains the data for reference univers</param>
        /// <param name="totalTable">Table that contains the data for the total univers</param>
        /// <param name="xValues">An array that carries XValues for the series</param>
        /// <param name="yValues">An array that carries YValues for the series</param>
        /// <param name="colunm">colunm whose value is required for example euros, pages etc</param>
        private static void GetSeriesData(DataTable graphicsTable, DataTable totalTable, string[] xValues, double[] yValues, string colunm) {
            int x = 1;
            int y = 1;

            //values for the rest of the chart series  
            double rest = Convert.ToDouble(totalTable.Rows[0][colunm]) - Convert.ToDouble(totalTable.Rows[1][colunm]);
            xValues[0] = "";
            yValues[0] = rest;
            //Values for the series
            foreach (DataRow dr in graphicsTable.Rows) {
                xValues[x] = dr["elements"].ToString();  //":"+                       //+" ["+dr["elementType"]+"] ";
                yValues[y] = double.Parse(dr[colunm].ToString());
                x++;
                y++;
            }

        }
        #endregion

        #endregion
    }
}
