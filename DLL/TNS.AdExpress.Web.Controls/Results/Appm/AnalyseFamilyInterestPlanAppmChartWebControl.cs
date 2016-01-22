#region Informations
// Auteur: D. Mussuma
// Date de création: 31/07/2006 
// Date of Modification: 
// 
#endregion

using System;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.FrameWork.DB.Common;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TblFormatCst = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;

using TNS.AdExpress.Web.Rules.Results.APPM;
using Dundas.Charting.WebControl;
using TNS.FrameWork;
using TNS.AdExpress.Domain.Units;


namespace TNS.AdExpress.Web.Controls.Results.Appm {
    /// <summary>
    /// Description résumée de AnalyseFamilyInterestPlanAppmChartWebControl.
    /// </summary>
    [DefaultProperty("Text"),
        ToolboxData("<{0}:AnalyseFamilyInterestPlanAppmChartWebControl runat=server></{0}:AnalyseFamilyInterestPlanAppmChartWebControl>")]
    public class AnalyseFamilyInterestPlanAppmChartWebControl : BaseAppmChartWebControl {

        #region Variables
        /// <summary>
        /// Valeur hexadecimale des couleurs des portions de camembert separé par le caractere ','
        /// </summary>
        private string _strPieColors = string.Empty;
        /// <summary>
        /// Couleur du titre du premier camembert
        /// </summary>
        private static string _camembertTitleTextFontColor1 = "#644883";
        /// <summary>
        /// Taille du titre du premier camembert
        /// </summary>
        private static string _camembertTitleTextFontSize1 = "10";
        /// <summary>
        /// Famille de police du titre du premier camembert
        /// </summary>
        private static string _camembertTitleTextFontFamily1 = "Arial";
        /// <summary>
        /// Couleur du titre du second camembert
        /// </summary>
        private static string _camembertTitleTextFontColor2 = "#644883";
        /// <summary>
        /// Taille du titre du second camembert
        /// </summary>
        private static string _camembertTitleTextFontSize2 = "10";
        /// <summary>
        /// Famille de police du titre du second camembert
        /// </summary>
        private static string _camembertTitleTextFontFamily2 = "Arial";
        /// <summary>
        /// Couleur du titre de l'histogramme
        /// </summary>
        private static string _histogrammeTitleTextFontColor = "#644883";
        /// <summary>
        /// Taille du titre de l'histogramme
        /// </summary>
        private static string _histogrammeTitleTextFontSize = "10";
        /// <summary>
        /// Famille de police de l'histogramme
        /// </summary>
        private static string _histogrammeTitleTextFontFamily = "Arial";
        /// <summary>
        /// Couleur du texte de la legende de l'histogramme
        /// </summary>
        private string _histogrammeLegendTextFontColor = "#000000";
        /// <summary>
        /// Taille du texte de la legende de l'histogramme
        /// </summary>
        private string _histogrammeLegendTextFontSize = "8";
        /// <summary>
        /// Famille de police du texte de la legende de l'histogramme
        /// </summary>
        private string _histogrammeLegendTextFontFamily = "Arial";
        /// <summary>
        /// Couleur de la premiere famille de presse
        /// </summary>
        private string _histogrammeColorFamille1 = "#9479B5";
        /// <summary>
        /// Couleur de la deuxieme famille de presse
        /// </summary>
        private string _histogrammeColorFamille2 = "#FFD7D7";
        /// <summary>
        /// Couleur de la troisieme famille de presse
        /// </summary>
        private string _histogrammeColorFamille3 = "#FFD7D7";
        /// <summary>
        /// Couleur de fond de l'histogramme
        /// </summary>
        private static string _histogrammeBackgroundColor = "#DECFE7";
        /// <summary>
        /// Famille de police du texte des unités en ordonnee sur l'histogramme
        /// </summary>
        private static string _histogrammeOrdonneeTextFontFamily = "Arial";
        /// <summary>
        /// Taille de police du texte des unités en ordonnee sur l'histogramme
        /// </summary>
        private static string _histogrammeOrdonneeTextFontSize = "8";
        /// <summary>
        /// Famille de police du texte des unités en Abscisse sur l'histogramme
        /// </summary>
        private static string _histogrammeAbscisseTextFontFamily = "Arial";
        /// <summary>
        /// Taille de police du texte des unités en Abscisse sur l'histogramme
        /// </summary>
        private static string _histogrammeAbscisseTextFontSize = "8";
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
        /// Obtient ou definis la Couleur du titre du premier camembert
        /// </summary>
        public string CamembertTitleTextFontColor1 {
            get { return _camembertTitleTextFontColor1; }
            set { _camembertTitleTextFontColor1 = value; }
        }
        /// <summary>
        /// Obtient ou definis la Taille du titre du premier camembert
        /// </summary>
        public string CamembertTitleTextFontSize1 {
            get { return _camembertTitleTextFontSize1; }
            set { _camembertTitleTextFontSize1 = value; }
        }
        /// <summary>
        /// Obtient ou definis la Famille de police du titre du premier camembert
        /// </summary>
        public string CamembertTitleTextFontFamily1 {
            get { return _camembertTitleTextFontFamily1; }
            set { _camembertTitleTextFontFamily1 = value; }
        }
        /// <summary>
        /// Obtient ou definis la Couleur du titre du second camembert
        /// </summary>
        public string CamembertTitleTextFontColor2 {
            get { return _camembertTitleTextFontColor2; }
            set { _camembertTitleTextFontColor2 = value; }
        }
        /// <summary>
        /// Obtient ou definis la Taille du titre du second camembert
        /// </summary>
        public string CamembertTitleTextFontSize2 {
            get { return _camembertTitleTextFontSize2; }
            set { _camembertTitleTextFontSize2 = value; }
        }
        /// <summary>
        /// Famille de police du titre du second camembert
        /// </summary>
        public string CamembertTitleTextFontFamily2 {
            get { return _camembertTitleTextFontFamily2; }
            set { _camembertTitleTextFontFamily2 = value; }
        }
        /// <summary>
        /// Obtient ou definis la Couleur du titre de l'histogramme
        /// </summary>
        public string HistogrammeTitleTextFontColor {
            get { return _histogrammeTitleTextFontColor; }
            set { _histogrammeTitleTextFontColor = value; }
        }
        /// <summary>
        /// Obtient ou definis la Taille du titre de l'histogramme
        /// </summary>
        public string HistogrammeTitleTextFontSize {
            get { return _histogrammeTitleTextFontSize; }
            set { _histogrammeTitleTextFontSize = value; }
        }
        /// <summary>
        /// Obtient ou definis la Famille de police de l'histogramme
        /// </summary>
        public string HistogrammeTitleTextFontFamily {
            get { return _histogrammeTitleTextFontFamily; }
            set { _histogrammeTitleTextFontFamily = value; }
        }
        /// <summary>
        /// Obtient ou definis la Couleur du texte de la legende de l'histogramme
        /// </summary>
        public string HistogrammeLegendTextFontColor {
            get { return _histogrammeLegendTextFontColor; }
            set { _histogrammeLegendTextFontColor = value; }
        }
        /// <summary>
        /// Obtient ou definis la Taille du texte de la legende de l'histogramme
        /// </summary>
        public string HistogrammeLegendTextFontSize {
            get { return _histogrammeLegendTextFontSize; }
            set { _histogrammeLegendTextFontSize = value; }
        }
        /// <summary>
        /// Obtient ou definis la Famille de police du texte de la legende de l'histogramme
        /// </summary>
        public string HistogrammeLegendTextFontFamily {
            get { return _histogrammeLegendTextFontFamily; }
            set { _histogrammeLegendTextFontFamily = value; }
        }
        /// <summary>
        /// Obtient ou definis la Couleur de la premiere famille de presse
        /// </summary>
        public string HistogrammeColorFamille1 {
            get { return _histogrammeColorFamille1; }
            set { _histogrammeColorFamille1 = value; }
        }
        /// <summary>
        /// Obtient ou definis la Couleur de la deuxieme famille de presse
        /// </summary>
        public string HistogrammeColorFamille2 {
            get { return _histogrammeColorFamille2; }
            set { _histogrammeColorFamille2 = value; }
        }
        /// <summary>
        /// Obtient ou definis la Couleur de la troisieme famille de presse
        /// </summary>
        public string HistogrammeColorFamille3 {
            get { return _histogrammeColorFamille3; }
            set { _histogrammeColorFamille3 = value; }
        }
        /// <summary>
        /// Obtient ou definis la Couleur de fond de l'histogramme
        /// </summary>
        public string HistogrammeBackgroundColor {
            get { return _histogrammeBackgroundColor; }
            set { _histogrammeBackgroundColor = value; }
        }
        /// <summary>
        /// Obtient ou definis la Famille de police du texte des unités en ordonnee sur l'histogramme
        /// </summary>
        public string HistogrammeOrdonneeTextFontFamily {
            get { return _histogrammeOrdonneeTextFontFamily; }
            set { _histogrammeOrdonneeTextFontFamily = value; }
        }
        /// <summary>
        /// Obtient ou definis la Taille de police du texte des unités en ordonnee sur l'histogramme
        /// </summary>
        public string HistogrammeOrdonneeTextFontSize {
            get { return _histogrammeOrdonneeTextFontSize; }
            set { _histogrammeOrdonneeTextFontSize = value; }
        }
        /// <summary>
        /// Obtient ou definis la Famille de police du texte des unités en abscisse sur l'histogramme
        /// </summary>
        public string HistogrammeAbscisseTextFontFamily {
            get { return _histogrammeAbscisseTextFontFamily; }
            set { _histogrammeAbscisseTextFontFamily = value; }
        }
        /// <summary>
        /// Obtient ou definis la Taille de police du texte des unités en abscisse sur l'histogramme
        /// </summary>
        public string HistogrammeAbscisseTextFontSize {
            get { return _histogrammeAbscisseTextFontSize; }
            set { _histogrammeAbscisseTextFontSize = value; }
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
        public AnalyseFamilyInterestPlanAppmChartWebControl(WebSession webSession, TNS.FrameWork.DB.Common.IDataSource dataSource, ChartImageType appmImageType)
            : base(webSession, dataSource, appmImageType) {
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="dataSource">Source de données</param>
        /// <param name="appmImageType">Type de l'image Appm (jpg, flash...)</param>
        /// <param name="skinId">Nom du skin</param>
        public AnalyseFamilyInterestPlanAppmChartWebControl(WebSession webSession, TNS.FrameWork.DB.Common.IDataSource dataSource, ChartImageType appmImageType, string skinId)
            : base(webSession, dataSource, appmImageType) {
            this.SkinID = skinId;
        }
        #endregion

        /// <summary>		
        /// Définit les données au moment du design du contrôle. 	
        /// </summary>
        public override void SetDesignMode() {

            #region variable
            DataTable InterestFamilyPlanData;
            ChartArea chartArea = new ChartArea();
            //Séries de valeurs pour chaque tranche du graphique
            double[] yUnitValues = null;
            string[] xUnitValues = null;
            double[] yUnitValuesSelected = null;
            string[] xUnitValuesSelected = null;
            double maxScale = 0;

            #endregion

            #region Initialisation des couleurs des graphiques
            ColorConverter cc = new ColorConverter();
            #region No Result
            Color textNoResultFontColor = (Color)cc.ConvertFromString(_textNoResultFontColor);
            #endregion
            //couleurs des tranches du graphique
            #region Camembert
            #region Couleur du camembert
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

            #region Histogramme
            Color histogrammeTitleTextFontColor = (Color)cc.ConvertFromString(_histogrammeTitleTextFontColor);
            Color histogrammeLegendTextFontColor = (Color)cc.ConvertFromString(_histogrammeLegendTextFontColor);
            Color histogrammeColorFamille1 = (Color)cc.ConvertFromString(_histogrammeColorFamille1);
            Color histogrammeColorFamille2 = (Color)cc.ConvertFromString(_histogrammeColorFamille2);
            Color histogrammeColorFamille3 = (Color)cc.ConvertFromString(_histogrammeColorFamille3);
            Color histogrammeBackgroundColor = (Color)cc.ConvertFromString(_histogrammeBackgroundColor);
            #endregion
            #endregion

            try {

                // Données				
                InterestFamilyPlanData = TNS.AdExpress.Web.Rules.Results.APPM.AnalyseFamilyInterestPlanRules.InterestFamilyPlan(this._customerWebSession, this._dataSource, this._idWave, this._dateBegin, this._dateEnd, this._idBaseTarget, this._idAdditionalTarget);

                #region Initialisation
                float areaUnitPositionHeight = 0, areaUnitadditionalPositionHeight = 0, areaUnitadditionalPositionY = 0, areaUnitPositionY = 0;
                if (this._customerWebSession.CurrentModule == WebConstantes.Module.Name.BILAN_CAMPAGNE) {
                    areaUnitPositionY = 4;
                    if (this._customerWebSession.Unit == WebConstantes.CustomerSessions.Unit.grp) {
                        if (InterestFamilyPlanData.Rows.Count <= 8) {
                            areaUnitPositionHeight = 30;
                            areaUnitadditionalPositionHeight = 30;
                            areaUnitadditionalPositionY = 37;
                        }
                        else {
                            areaUnitPositionHeight = 20;
                            areaUnitadditionalPositionHeight = 20;
                            areaUnitadditionalPositionY = 27;
                        }
                    }
                    else {
                        if (InterestFamilyPlanData.Rows.Count <= 8) {
                            areaUnitPositionHeight = 40;
                        }
                        else
                            areaUnitPositionHeight = 30;
                    }
                }
                else {
                    if (this._customerWebSession.Unit == WebConstantes.CustomerSessions.Unit.grp) {
                        areaUnitPositionY = 4;
                        if (InterestFamilyPlanData.Rows.Count <= 8) {
                            areaUnitPositionHeight = 40;
                            areaUnitadditionalPositionHeight = 40;
                            areaUnitadditionalPositionY = 58;
                        }
                        else {
                            areaUnitPositionHeight = 40;
                            areaUnitadditionalPositionHeight = 40;
                            areaUnitadditionalPositionY = 54;
                        }
                    }
                    else {
                        areaUnitPositionY = 12;
                        if (InterestFamilyPlanData.Rows.Count <= 8) {
                            areaUnitPositionHeight = 70;
                        }
                        else
                            areaUnitPositionHeight = 80;
                    }
                }
                #endregion

                #region Création et définition du graphique pour la cible de base
                if (InterestFamilyPlanData != null && InterestFamilyPlanData.Rows.Count > 0)
                    getSeriesDataBase(InterestFamilyPlanData, ref xUnitValues, ref yUnitValues);

                //Création du graphique	des uités(euros, grp, insertion, page) cible de base
                ChartArea chartAreaUnit = null;
                Series serieInterestFamily = new Series();

                if (InterestFamilyPlanData != null && InterestFamilyPlanData.Rows.Count > 0) {
                    //Conteneur graphique pour unité de cible de base
                    chartAreaUnit = new ChartArea();
                    #region dimension 1er Camembert
                    //Alignement
                    if (this._customerWebSession.Unit == WebConstantes.CustomerSessions.Unit.grp) {
                        chartAreaUnit.AlignOrientation = AreaAlignOrientation.Vertical;
                        chartAreaUnit.Position.X = 2;
                        if (InterestFamilyPlanData.Rows.Count <= 8) {
                            chartAreaUnit.Position.Y = areaUnitPositionY;
                            chartAreaUnit.Position.Height = areaUnitPositionHeight;
                        }
                        else {
                            chartAreaUnit.Position.Y = areaUnitPositionY;
                            chartAreaUnit.Position.Height = areaUnitPositionHeight;
                        }
                        chartAreaUnit.Position.Width = 96;
                    }
                    else {
                        chartAreaUnit.AlignOrientation = AreaAlignOrientation.Vertical;
                        chartAreaUnit.Position.X = 2;
                        if (InterestFamilyPlanData.Rows.Count <= 8) {
                            chartAreaUnit.Position.Y = areaUnitPositionY;
                            chartAreaUnit.Position.Height = areaUnitPositionHeight;
                        }
                        else {
                            chartAreaUnit.Position.Y = areaUnitPositionY;
                            chartAreaUnit.Position.Height = areaUnitPositionHeight;
                        }
                        chartAreaUnit.Position.Width = 96;
                    }
                    #endregion
                    this.ChartAreas.Add(chartAreaUnit);
                    //Charger les séries de valeurs 
                    string unitName = "";
                    string chartAreaName = "";
                    string chartAreaAdditionalName = "";
                    string chartAreaCgrpName = "";

                    #region sélection par rappot à l'unité choisit
                    switch (this._customerWebSession.Unit) {
                        case WebConstantes.CustomerSessions.Unit.euro:
                            unitName = GestionWeb.GetWebWord(2110, this._customerWebSession.SiteLanguage);
                            break;
                        case WebConstantes.CustomerSessions.Unit.kEuro:
                            unitName = GestionWeb.GetWebWord(2111, this._customerWebSession.SiteLanguage);
                            break;
                        case WebConstantes.CustomerSessions.Unit.grp:
                            unitName = Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].WebTextId, _customerWebSession.SiteLanguage));
                            break;
                        case WebConstantes.CustomerSessions.Unit.insertion:
                            unitName = Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].WebTextId, _customerWebSession.SiteLanguage));
                            break;
                        case WebConstantes.CustomerSessions.Unit.pages:
                            unitName = Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].WebTextId, _customerWebSession.SiteLanguage));
                            break;
                        default: break;
                    }
                    #region Titres du graphiques
                    //Titre graphique cible de base
                    if (this._customerWebSession.Unit == WebConstantes.CustomerSessions.Unit.grp) {
                        chartAreaName += GestionWeb.GetWebWord(1736, this._customerWebSession.SiteLanguage);
                        chartAreaName += " ";
                        chartAreaName += unitName;
                        chartAreaName += " (" + InterestFamilyPlanData.Rows[0]["baseTarget"] + ") ";
                        chartAreaName += GestionWeb.GetWebWord(1737, this._customerWebSession.SiteLanguage);
                        //Titre graphique cible selectionnée
                        chartAreaAdditionalName += GestionWeb.GetWebWord(1736, this._customerWebSession.SiteLanguage);
                        chartAreaAdditionalName += " ";
                        chartAreaAdditionalName += unitName;
                        chartAreaAdditionalName += " (" + InterestFamilyPlanData.Rows[0]["additionalTarget"] + ") ";
                        chartAreaAdditionalName += GestionWeb.GetWebWord(1737, this._customerWebSession.SiteLanguage);
                    }
                    else {
                        chartAreaName += GestionWeb.GetWebWord(1736, this._customerWebSession.SiteLanguage);
                        chartAreaName += " ";
                        chartAreaName += unitName;
                        chartAreaName += " " + GestionWeb.GetWebWord(1737, this._customerWebSession.SiteLanguage);
                    }
                    //Titre graphique du CGRP
                    chartAreaCgrpName += GestionWeb.GetWebWord(1685, this._customerWebSession.SiteLanguage);
                    chartAreaCgrpName += " " + GestionWeb.GetWebWord(1737, this._customerWebSession.SiteLanguage) + " :";
                    chartAreaCgrpName += InterestFamilyPlanData.Rows[0]["baseTarget"];
                    chartAreaCgrpName += " " + GestionWeb.GetWebWord(1739, this._customerWebSession.SiteLanguage) + " ";
                    chartAreaCgrpName += InterestFamilyPlanData.Rows[0]["additionalTarget"];
                    #endregion
                    #endregion


                    //serieInterestFamily=setSeriesInterestFamily(this,InterestFamilyPlanData,chartAreaUnit,serieInterestFamily,xUnitValues,yUnitValues,WebConstantes.UI.UI.newPieColors,chartAreaName);												
                    serieInterestFamily = setSeriesInterestFamily(this, InterestFamilyPlanData, chartAreaUnit, serieInterestFamily, xUnitValues, yUnitValues, pieColors, chartAreaName);
                #endregion

                    serieInterestFamily.ShowInLegend = false;

                    #region Création et définition du graphique pour la cible selectionnée
                    GetSeriesDataAdditional(InterestFamilyPlanData, ref xUnitValues, ref yUnitValues);
                    //Création du graphique	pour unité
                    ChartArea chartAreaUnitadditional = null;
                    Series serieInterestFamilyadditional = new Series();
                    //Conteneur graphique pour unité
                    chartAreaUnitadditional = new ChartArea();

                    #region legend2
                    Legend secondLegend = new Legend("Second");
                    this.Legends.Add(secondLegend);
                    serieInterestFamilyadditional.Legend = "Second";
                    secondLegend.DockToChartArea = chartAreaUnitadditional.Name;
                    secondLegend.DockInsideChartArea = true;
                    secondLegend.BorderWidth = 2;
                    secondLegend.Enabled = false;
                    #endregion

                    //Alignement
                    if (this._customerWebSession.Unit == WebConstantes.CustomerSessions.Unit.grp) {
                        chartAreaUnitadditional.AlignOrientation = AreaAlignOrientation.Vertical;
                        chartAreaUnitadditional.Position.X = 2;
                        if (InterestFamilyPlanData.Rows.Count <= 8) {
                            chartAreaUnitadditional.Position.Y = areaUnitadditionalPositionY;
                            chartAreaUnitadditional.Position.Height = areaUnitadditionalPositionHeight;
                        }
                        else {
                            chartAreaUnitadditional.Position.Y = areaUnitadditionalPositionY;
                            chartAreaUnitadditional.Position.Height = areaUnitadditionalPositionHeight;
                        }

                        chartAreaUnitadditional.Position.Width = 96;
                        this.ChartAreas.Add(chartAreaUnitadditional);
                        //Charger les séries de valeurs 
                        //serieInterestFamilyadditional=setSeriesInterestFamily(this,InterestFamilyPlanData,chartAreaUnitadditional,serieInterestFamilyadditional,xUnitValues,yUnitValues,WebConstantes.UI.UI.newPieColors,chartAreaAdditionalName);												
                        serieInterestFamilyadditional = setSeriesInterestFamily(this, InterestFamilyPlanData, chartAreaUnitadditional, serieInterestFamilyadditional, xUnitValues, yUnitValues, pieColors, chartAreaAdditionalName);
                    }
                    #endregion

                    if (this._customerWebSession.CurrentModule == WebConstantes.Module.Name.BILAN_CAMPAGNE) {

                        #region Création et définition du graphique pour CGRP
                        getSeriesDataCgrp(InterestFamilyPlanData, ref xUnitValues, ref yUnitValues, ref xUnitValuesSelected, ref yUnitValuesSelected, ref maxScale);
                        //Création du graphique	pour unité
                        ChartArea chartAreaCgrp = null;
                        Series serieInterestFamilyCgrpBase = new Series();
                        Series serieInterestFamilyCgrpSelected = new Series();

                        //legende du graphe bar
                        ChartArea chartAreaCgrpLegend = new ChartArea();
                        chartAreaCgrpLegend.Name = "legendCgrpArea";
                        //Allignement
                        if (this._customerWebSession.Unit == WebConstantes.CustomerSessions.Unit.grp) {
                            chartAreaCgrpLegend.AlignOrientation = AreaAlignOrientation.Vertical;
                            chartAreaCgrpLegend.Position.X = 5;
                            if (InterestFamilyPlanData.Rows.Count <= 8) {
                                chartAreaCgrpLegend.Position.Y = 97;
                                chartAreaCgrpLegend.Position.Height = 2;
                            }
                            else {
                                chartAreaCgrpLegend.Position.Y = 96;
                                chartAreaCgrpLegend.Position.Height = 2;
                            }
                            chartAreaCgrpLegend.Position.Width = 80;
                        }
                        else {
                            chartAreaCgrpLegend.AlignOrientation = AreaAlignOrientation.Vertical;
                            chartAreaCgrpLegend.Position.X = 5;
                            if (InterestFamilyPlanData.Rows.Count <= 8) {
                                chartAreaCgrpLegend.Position.Y = 96;
                                chartAreaCgrpLegend.Position.Height = 2;
                            }
                            else {
                                chartAreaCgrpLegend.Position.Y = 92;
                                chartAreaCgrpLegend.Position.Height = 4;
                            }
                            chartAreaCgrpLegend.Position.Width = 80;
                        }
                        this.ChartAreas.Add(chartAreaCgrpLegend);

                        #region legend3
                        serieInterestFamilyCgrpBase.ShowInLegend = false;
                        Legend thirdLegend = new Legend("Third");
                        LegendItem legendItemReference = new LegendItem();
                        legendItemReference.Name = GestionWeb.GetWebWord(1685, this._customerWebSession.SiteLanguage);
                        legendItemReference.Name += " (" + InterestFamilyPlanData.Rows[0]["baseTarget"] + ") ";
                        legendItemReference.Style = LegendImageStyle.Rectangle;
                        legendItemReference.ShadowOffset = 1;
                        legendItemReference.Color = histogrammeColorFamille3;
                        thirdLegend.CustomItems.Add(legendItemReference);

                        LegendItem legendItemReference2 = new LegendItem();
                        legendItemReference2.Name = GestionWeb.GetWebWord(1685, this._customerWebSession.SiteLanguage);
                        legendItemReference2.Name += " (" + InterestFamilyPlanData.Rows[0]["additionalTarget"] + ") ";
                        legendItemReference2.Style = LegendImageStyle.Rectangle;
                        legendItemReference2.ShadowOffset = 1;
                        legendItemReference2.Color = histogrammeColorFamille2;
                        thirdLegend.CustomItems.Add(legendItemReference2);
                        this.Legends.Add(thirdLegend);
                        thirdLegend.DockToChartArea = chartAreaCgrpLegend.Name;
                        //thirdLegend.Font = new Font("Arial", (float)8);
                        thirdLegend.Font = new Font(_histogrammeLegendTextFontFamily, (float)Convert.ToDouble(_histogrammeLegendTextFontSize));
                        thirdLegend.Enabled = true;
                        thirdLegend.InsideChartArea = "legendCgrpArea";
                        thirdLegend.LegendStyle = LegendStyle.Row;
                        thirdLegend.Docking = LegendDocking.Bottom;
                        thirdLegend.Alignment = StringAlignment.Center;
                        thirdLegend.FontColor = histogrammeLegendTextFontColor;
                        #endregion

                        #region legend4
                        Legend fourthLegend = new Legend("Fourth");
                        this.Legends.Add(fourthLegend);
                        serieInterestFamilyCgrpSelected.Legend = "Fourth";
                        fourthLegend.DockToChartArea = chartAreaCgrpLegend.Name;
                        //fourthLegend.DockInsideChartArea = true;	
                        fourthLegend.InsideChartArea = "legendCgrpArea";
                        legendItemReference.BorderWidth = 1;
                        legendItemReference.Color = histogrammeColorFamille1;
                        legendItemReference.Name = GestionWeb.GetWebWord(1685, this._customerWebSession.SiteLanguage);
                        legendItemReference.Name += " (" + InterestFamilyPlanData.Rows[0]["baseTarget"] + ") ";
                        fourthLegend.Enabled = false;
                        fourthLegend.CustomItems.Add(legendItemReference);
                        fourthLegend.Font = new Font("Arial", (float)8);
                        fourthLegend.LegendStyle = LegendStyle.Row;
                        fourthLegend.Docking = LegendDocking.Bottom;
                        fourthLegend.Alignment = StringAlignment.Center;
                        #endregion

                        //Conteneur graphique pour unité
                        chartAreaCgrp = new ChartArea();
                        //Alignement
                        #region dimension histogramme
                        if (this._customerWebSession.Unit == WebConstantes.CustomerSessions.Unit.grp) {
                            chartAreaCgrp.AlignOrientation = AreaAlignOrientation.Vertical;
                            chartAreaCgrp.Position.X = 2;
                            if (InterestFamilyPlanData.Rows.Count <= 8) {
                                chartAreaCgrp.Position.Y = 68;
                                chartAreaCgrp.Position.Height = 30;
                            }
                            else {
                                chartAreaCgrp.Position.Y = 48;
                                chartAreaCgrp.Position.Height = 48;
                            }
                            chartAreaCgrp.Position.Width = 80;
                        }
                        else {
                            chartAreaCgrp.AlignOrientation = AreaAlignOrientation.Vertical;
                            chartAreaCgrp.Position.X = 2;
                            if (InterestFamilyPlanData.Rows.Count <= 8) {
                                chartAreaCgrp.Position.Y = 48;
                                chartAreaCgrp.Position.Height = 40;
                            }
                            else {
                                chartAreaCgrp.Position.Y = 40;
                                chartAreaCgrp.Position.Height = 55;
                            }
                            chartAreaCgrp.Position.Width = 80;
                        }
                        #endregion
                        this.ChartAreas.Add(chartAreaCgrp);
                        //Charger les séries de valeurs 
                        serieInterestFamilyCgrpBase = SetSeriesBarInterestFamily(InterestFamilyPlanData, chartAreaCgrp, serieInterestFamilyCgrpBase, xUnitValues, yUnitValues, histogrammeColorFamille1, chartAreaCgrpName, maxScale);
                        serieInterestFamilyCgrpSelected = SetSeriesBarInterestFamily(InterestFamilyPlanData, chartAreaCgrp, serieInterestFamilyCgrpSelected, xUnitValuesSelected, yUnitValuesSelected, histogrammeColorFamille2, chartAreaCgrpName, maxScale);

                        #endregion

                        #region initialisation du control pour le module Chronopresse
                        if (this._customerWebSession.Unit == WebConstantes.CustomerSessions.Unit.grp) {
                            InitializeComponent(InterestFamilyPlanData, this, chartAreaUnit, chartAreaUnitadditional, chartAreaCgrp, this._imageType);

                            if (InterestFamilyPlanData != null && InterestFamilyPlanData.Rows.Count > 0) {
                                this.Series.Add(serieInterestFamily);
                                this.Series.Add(serieInterestFamilyadditional);
                                this.Series.Add(serieInterestFamilyCgrpBase);
                                this.Series.Add(serieInterestFamilyCgrpSelected);
                            }
                        }
                        else {
                            InitializeComponent(InterestFamilyPlanData, this, chartAreaUnit, chartAreaCgrp, this._imageType);

                            if (InterestFamilyPlanData != null && InterestFamilyPlanData.Rows.Count > 0) {
                                this.Series.Add(serieInterestFamily);
                                this.Series.Add(serieInterestFamilyCgrpBase);
                                this.Series.Add(serieInterestFamilyCgrpSelected);
                            }
                        }
                        #endregion

                    }
                    else {

                        #region initialisation du control pour le module Données de cadrage
                        if (this._customerWebSession.Unit == WebConstantes.CustomerSessions.Unit.grp) {
                            InitializeComponent(InterestFamilyPlanData, this, chartAreaUnit, chartAreaUnitadditional, null, this._imageType);

                            if (InterestFamilyPlanData != null && InterestFamilyPlanData.Rows.Count > 0) {
                                this.Series.Add(serieInterestFamily);
                                this.Series.Add(serieInterestFamilyadditional);
                            }
                        }
                        else {
                            InitializeComponent(InterestFamilyPlanData, this, chartAreaUnit, null, this._imageType);

                            if (InterestFamilyPlanData != null && InterestFamilyPlanData.Rows.Count > 0) {
                                this.Series.Add(serieInterestFamily);
                            }
                        }
                        #endregion

                    }

                    AddCopyRight(this._customerWebSession, (_imageType == ChartImageType.Jpeg));
                }
                else {
                    this.Titles.Add(GestionWeb.GetWebWord(2106, this._customerWebSession.SiteLanguage));
                    this.Titles[0].Font = new Font(_textNoResultFontFamily, (float)Convert.ToDouble(_textNoResultFontSize), System.Drawing.FontStyle.Bold);
                    this.Titles[0].Color = textNoResultFontColor;
                    this.Width = 250;
                    this.Height = 25;
                }

            }
            catch (System.Exception err) {
                throw (new WebExceptions.AnalyseFamilyInterestPlanChartUIException("Erreur dans l'affichage des graphiques des données des AnalyseFamilyInterestPlanChart ", err));
            }
        }


        #region Méthodes privées
        /// <summary>
        /// Obtient les séries de valeurs des unités pour la cible de base à afficher graphiquement
        /// </summary>
        /// <param name="dt">table de données</param>
        /// <param name="xValues">libellés du graphique</param>
        /// <param name="yValues">valeurs pour graphique</param>
        private static void getSeriesDataBase(DataTable dt, ref string[] xValues, ref double[] yValues) {

            #region Variable
            //string ventilationType="distributionBase";
            int x = 0;
            int y = 0;
            #endregion

            #region Les séries  d'unité à afficher graphiquement
            xValues = new string[dt.Rows.Count];
            yValues = new double[dt.Rows.Count];
            string[] zValues = new string[dt.Rows.Count];
            for (int i = 1; i < dt.Rows.Count - 1; i++) {
                xValues[x] = dt.Rows[i]["InterestFamily"].ToString();
                yValues[y] = double.Parse(dt.Rows[i]["distributionBase"].ToString());
                x++;
                y++;
            }
            #endregion
        }

        /// <summary>
        /// Obtient les séries de valeurs des unités pour la cible sélectionnée à afficher graphiquement
        /// </summary>
        /// <param name="dt">table de données</param>
        /// <param name="xValues">libellés du graphique</param>
        /// <param name="yValues">valeurs pour graphique</param>
        private static void GetSeriesDataAdditional(DataTable dt, ref string[] xValues, ref double[] yValues) {

            #region Variable
            //string ventilationType="distributionSelected";
            int x = 0;
            int y = 0;
            #endregion

            #region Les séries  d'unité à afficher graphiquement
            xValues = new string[dt.Rows.Count];
            yValues = new double[dt.Rows.Count];
            for (int i = 1; i < dt.Rows.Count - 1; i++) {
                xValues[x] = dt.Rows[i]["InterestFamily"].ToString();
                yValues[y] = double.Parse(dt.Rows[i]["distributionSelected"].ToString());
                x++;
                y++;
            }
            #endregion
        }

        /// <summary>
        /// Obtient les séries de valeurs des unités pour la cible sélectionnée à afficher graphiquement
        /// </summary>
        /// <param name="dt">table de données</param>
        /// <param name="xValuesBase">libellés du graphique</param>
        /// <param name="yValuesBase">valeurs pour graphique</param>
        /// <param name="xValuesSelected">Liebllés sélectionnée</param>
        /// <param name="yValuesSelected">Valeur sélectionnée</param>
        /// <param name="maxScale">Echelle de l'axe X du graphe</param>
        private static void getSeriesDataCgrp(DataTable dt, ref string[] xValuesBase, ref double[] yValuesBase, ref string[] xValuesSelected, ref double[] yValuesSelected, ref double maxScale) {

            #region Variable
            //string ventilationTypeBase="cgrpBase";
            //string ventilationTypeSelected="cgrpSelected";
            int x = 0;
            int y = 0;
            #endregion

            #region Les séries  cgrp
            xValuesBase = new string[dt.Rows.Count - 2];
            yValuesBase = new double[dt.Rows.Count - 2];
            xValuesSelected = new string[dt.Rows.Count - 2];
            yValuesSelected = new double[dt.Rows.Count - 2];

            for (int i = 0; i < dt.Rows.Count - 1; i++) {
                if (dt.Rows[i]["cgrpBase"].ToString() == "") {
                    continue;
                }
                else {
                    xValuesBase[x] = dt.Rows[i]["InterestFamily"].ToString();
                    //yValuesBase[y]=double.Parse(dt.Rows[i]["cgrpDistributionBase"].ToString());
                    yValuesBase[y] = double.Parse(dt.Rows[i]["cgrpBase"].ToString());
                    xValuesSelected[x] = dt.Rows[i]["InterestFamily"].ToString();
                    //yValuesSelected[y]=double.Parse(dt.Rows[i]["cgrpDistributionSelected"].ToString());
                    yValuesSelected[y] = double.Parse(dt.Rows[i]["cgrpSelected"].ToString());
                    x++;
                    y++;
                }
            }
            for (int i = 0; i < yValuesBase.Length; i++) {
                if (maxScale < yValuesBase[i])
                    maxScale = yValuesBase[i];
            }

            for (int i = 0; i < yValuesSelected.Length; i++) {
                if (maxScale < yValuesSelected[i])
                    maxScale = yValuesSelected[i];
            }
            #endregion
        }


        /// <summary>
        /// Crétion du graphique unité(grp,euro,isertion,page) 
        /// </summary>
        /// <param name="appmChart">Chart</param>
        /// <param name="dt">tableau de résultats</param>
        /// <param name="chartArea">contenant objet graphique</param>
        /// <param name="series">séries de valeurs</param>
        /// <param name="xValues">séries de libellés</param>
        /// <param name="yValues">séries de valeurs</param>
        /// <param name="barColors">couleurs du graphique</param>
        /// <param name="chartAreaName">Nom du conteneur de l'image</param>
        /// <returns>séries de valeurs</returns>
        private static Dundas.Charting.WebControl.Series setSeriesInterestFamily(BaseAppmChartWebControl appmChart, DataTable dt, ChartArea chartArea, Dundas.Charting.WebControl.Series series, string[] xValues, double[] yValues, Color[] barColors, string chartAreaName) {
            #region  Création graphique
            if (xValues != null && yValues != null) {

                #region Création et définition du graphique
                //Création du graphique							

                //Type de graphique
                series.Type = SeriesChartType.Pie;
                //Chart1.ImageType = ChartImageType.Png;
                series.SmartLabels.Enabled = true;
                series.XValueType = ChartValueTypes.String;
                series.YValueType = ChartValueTypes.Double;

                series.Enabled = true;

                chartArea.Area3DStyle.Enable3D = true;
                chartArea.Name = chartAreaName;
                series.ChartArea = chartArea.Name;
                series.Points.DataBindXY(xValues, yValues);

                appmChart.DataManipulator.Sort(PointsSortOrder.Descending, series);

                #region Définition des couleurs
                //couleur du graphique
                for (int k = 0; k < dt.Rows.Count && k < barColors.Length; k++) {
                    series.Points[k].Color = barColors[k];
                }
                #endregion

                #region Légende
                series["LabelStyle"] = "Outside";
                series.LegendToolTip = "#PERCENT";
                series["PieLineColor"] = "Black";

                #endregion
                series["LabelStyle"] = "Outside";
                series.Label = "#PERCENT : #VALX ";
                series.ToolTip = "#VALX";
                for (int i = 0; i < xValues.Length; i++) {
                    series.Points[i]["Exploded"] = "true";
                }

                series.LegendText = "#VALX";
                #endregion
            }
            #endregion

            return series;
        }

        /// <summary>
        /// Crétion du graphique unité Cgrp (histogramme)
        /// </summary>
        /// <param name="dt">tableau de résultats</param>
        /// <param name="chartArea">contenant objet graphique</param>
        /// <param name="series">séries de valeurs</param>
        /// <param name="xValues">séries de libellés</param>
        /// <param name="yValues">séries de valeurs</param>
        ///<param name="barColor">couleurs du graphique</param>
        ///<param name="chartAreaName">Nom du conteneur de l'image</param>
        ///<param name="maxScale">echelle maximum</param>
        /// <returns>séries de valeurs</returns>
        private static Dundas.Charting.WebControl.Series SetSeriesBarInterestFamily(DataTable dt, ChartArea chartArea, Dundas.Charting.WebControl.Series series, string[] xValues, double[] yValues, System.Drawing.Color barColor, string chartAreaName, double maxScale) {
            #region  Création graphique
            if (xValues != null && yValues != null) {

                #region Couleur du controle
                ColorConverter cc = new ColorConverter();
                Color histogrammeBackgroundColor = (Color)cc.ConvertFromString(_histogrammeBackgroundColor);
                #endregion

                #region Création et définition du graphique
                //Création du graphique							

                //Type de graphique
                series.Type = SeriesChartType.Bar;
                //series.SmartLabels.Enabled = true;	
                series.XValueType = ChartValueTypes.String;
                series.YValueType = ChartValueTypes.Double;
                series.Enabled = true;

                chartArea.Area3DStyle.Enable3D = false;
                //chartArea.BackColor = Color.FromArgb(222, 207, 231);
                chartArea.BackColor = histogrammeBackgroundColor;
                chartArea.Name = chartAreaName;
                series.ChartArea = chartArea.Name;
                chartArea.AxisY.Maximum = maxScale + 1000;
                chartArea.AxisX.Maximum = dt.Rows.Count;
                series.Points.DataBindXY(xValues, yValues);
                chartArea.AxisX.LabelStyle.Font = new Font(_histogrammeAbscisseTextFontFamily, (float)Convert.ToDouble(_histogrammeAbscisseTextFontSize));
                chartArea.AxisY.LabelStyle.Font = new Font(_histogrammeOrdonneeTextFontFamily, (float)Convert.ToDouble(_histogrammeOrdonneeTextFontSize));

                chartArea.AxisX.Interval = 1;
                chartArea.AxisX.Margin = true;

                chartArea.AxisX.LabelStyle.ShowEndLabels = true;

                #region Définition des couleurs
                //couleur du graphique
                series.Color = barColor;
                #endregion

                #region Légende
                series["LabelStyle"] = "Outside";
                series["PointWidth"] = "1.0";
                series.ToolTip = "#VALX : #VALY";
                series["PieLineColor"] = "Black";
                #endregion
                series.Label = "#VALY";
                #endregion
            }
            #endregion

            return series;
        }


        /// <summary>
        /// Initialise les styles du webcontrol pour média radio et télé
        /// </summary>
        /// <param name="dt">tableau de données</param>
        /// <param name="appmChart">Objet Webcontrol</param>
        /// <param name="chartAreaUnit">conteneur de l'image répartition unité</param>
        /// <param name="chartAreaUnitadditional">conteneur de l'image répartition pour cible selectionnée</param>
        /// <param name="chartAreaCgrp">conteneur de l'image répartition pour Cgrp</param>
        /// <param name="appmImageType">Type sortie de l'image</param>
        private static void InitializeComponent(DataTable dt, BaseAppmChartWebControl appmChart, ChartArea chartAreaUnit, ChartArea chartAreaUnitadditional, ChartArea chartAreaCgrp, ChartImageType appmImageType) {

            ColorConverter cc = new ColorConverter();
            Color camembertTitleTextFontColor1 = (Color)cc.ConvertFromString(_camembertTitleTextFontColor1);
            Color camembertTitleTextFontColor2 = (Color)cc.ConvertFromString(_camembertTitleTextFontColor2);
            Color histogrammeTitleTextFontColor = (Color)cc.ConvertFromString(_histogrammeTitleTextFontColor);
            Color controlBorderColor = (Color)cc.ConvertFromString(_controlBorderColor);

            //Type image
            appmChart.ImageType = appmImageType;
            if (appmImageType == ChartImageType.Flash) {
                appmChart.AnimationTheme = AnimationTheme.GrowingTogether;
                appmChart.AnimationDuration = 0.3;
                appmChart.RepeatAnimation = false;
            }

            #region Initialisation
            int height = 0;
            float positionY = 0;
            if (chartAreaCgrp != null) {
                //Height
                if (dt.Rows.Count < 6) {
                    height = 1000;
                }
                else if (dt.Rows.Count >= 6 && dt.Rows.Count < 16) {
                    height = 1200;
                }
                if (dt.Rows.Count >= 16) {
                    height = 1500;
                }
                //Position Y
                if (dt.Rows.Count <= 8) {
                    positionY = 36;
                }
                else
                    positionY = 26;
            }
            else {
                //Height
                if (dt.Rows.Count < 6) {
                    height = 700;
                }
                else if (dt.Rows.Count >= 6 && dt.Rows.Count < 16) {
                    height = 800;
                }
                if (dt.Rows.Count >= 16) {
                    height = 900;
                }
                //Position Y
                if (dt.Rows.Count <= 8) {
                    positionY = 56;
                }
                else
                    positionY = 52;
            }
            #endregion

            #region Chart
            appmChart.Width = new Unit("900px");
            appmChart.Height = new Unit("" + height + "px");
            appmChart.BackGradientType = GradientType.TopBottom;
            appmChart.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);
            appmChart.BorderStyle = ChartDashStyle.Solid;
            //appmChart.BorderLineColor = Color.FromArgb(99, 73, 132);
            appmChart.BorderLineColor = controlBorderColor;
            appmChart.BorderLineWidth = Convert.ToInt32(_controlBorderSize);

            appmChart.Legend.Enabled = true;
            #endregion

            #region Titre
            //titre unité de base
            appmChart.Titles.Add(chartAreaUnit.Name);
            appmChart.Titles[0].DockInsideChartArea = true;
            appmChart.Titles[0].Position.Auto = false;
            appmChart.Titles[0].Position.X = 50;
            appmChart.Titles[0].Position.Y = 3;
            appmChart.Titles[0].Font = new Font(_camembertTitleTextFontFamily1, (float)Convert.ToDouble(_camembertTitleTextFontSize1));
            //appmChart.Titles[0].Color = Color.FromArgb(100, 72, 131);
            appmChart.Titles[0].Color = camembertTitleTextFontColor1;
            appmChart.Titles[0].DockToChartArea = chartAreaUnit.Name;

            //titre unité pour la cible selectionnée
            appmChart.Titles.Add(chartAreaUnitadditional.Name);
            appmChart.Titles[1].DockInsideChartArea = true;
            appmChart.Titles[1].Position.Auto = false;
            appmChart.Titles[1].Position.X = 50;
            appmChart.Titles[1].Position.Y = positionY;
            //appmChart.Titles[1].Font = new Font("Arial", (float)10);
            appmChart.Titles[1].Font = new Font(_camembertTitleTextFontFamily2, (float)Convert.ToDouble(_camembertTitleTextFontSize2));
            //appmChart.Titles[1].Color = Color.FromArgb(100, 72, 131);
            appmChart.Titles[1].Color = camembertTitleTextFontColor2;
            appmChart.Titles[1].DockToChartArea = chartAreaUnitadditional.Name;

            //titre unité pour CGRP
            if (chartAreaCgrp != null) {
                appmChart.Titles.Add(chartAreaCgrp.Name);
                appmChart.Titles[2].DockInsideChartArea = true;
                appmChart.Titles[2].Position.Auto = false;
                appmChart.Titles[2].Position.X = 50;
                if (dt.Rows.Count <= 8) {
                    appmChart.Titles[2].Position.Y = 67;
                }
                else
                    appmChart.Titles[2].Position.Y = 48;

                appmChart.Titles[2].Font = new Font(_histogrammeTitleTextFontFamily, (float)Convert.ToDouble(_histogrammeTitleTextFontSize));
                //appmChart.Titles[2].Color = Color.FromArgb(100, 72, 131);
                appmChart.Titles[2].Color = histogrammeTitleTextFontColor;
                appmChart.Titles[2].DockToChartArea = chartAreaCgrp.Name;
            }
            #endregion
        }

        /// <summary>
        /// Initialise les styles du webcontrol pour média radio et télé
        /// </summary>
        /// <param name="dt">tableau de données</param>
        /// <param name="appmChart">Objet Webcontrol</param>
        /// <param name="chartAreaUnit">conteneur de l'image répartition unité</param>
        /// <param name="chartAreaCgrp">conteneur de l'image répartition pour Cgrp</param>
        /// <param name="appmImageType">Type sortie de l'image</param>
        private static void InitializeComponent(DataTable dt, BaseAppmChartWebControl appmChart, ChartArea chartAreaUnit, ChartArea chartAreaCgrp, ChartImageType appmImageType) {
            ColorConverter cc = new ColorConverter();
            Color camembertTitleTextFontColor1 = (Color)cc.ConvertFromString(_camembertTitleTextFontColor1);
            Color histogrammeTitleTextFontColor = (Color)cc.ConvertFromString(_histogrammeTitleTextFontColor);
            Color controlBorderColor = (Color)cc.ConvertFromString(_controlBorderColor);
            //Type image
            appmChart.ImageType = appmImageType;
            if (appmImageType == ChartImageType.Flash) {
                appmChart.AnimationTheme = AnimationTheme.GrowingTogether;
                appmChart.AnimationDuration = 0.3;
                appmChart.RepeatAnimation = false;
            }

            #region Initialisation
            int height = 0;
            float positionY = 0;
            if (chartAreaCgrp != null) {
                positionY = 3;
                if (dt.Rows.Count < 6) {
                    height = 900;
                }
                else if (dt.Rows.Count >= 6 && dt.Rows.Count < 16) {
                    height = 1100;
                }
                if (dt.Rows.Count >= 16) {
                    height = 1400;
                }
            }
            else {
                positionY = 8;
                if (dt.Rows.Count < 6) {
                    height = 350;
                }
                else if (dt.Rows.Count >= 6 && dt.Rows.Count < 16) {
                    height = 400;
                }
                if (dt.Rows.Count >= 16) {
                    height = 550;
                }
            }
            #endregion

            #region Chart
            appmChart.Width = new Unit("900px");
            appmChart.Height = new Unit("" + height + "px");
            appmChart.BackGradientType = GradientType.TopBottom;
            appmChart.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);
            appmChart.BorderStyle = ChartDashStyle.Solid;
            //appmChart.BorderLineColor = Color.FromArgb(99, 73, 132);
            //appmChart.BorderLineWidth = 2;
            appmChart.BorderLineColor = controlBorderColor;
            appmChart.BorderLineWidth = Convert.ToInt32(_controlBorderSize);

            appmChart.Legend.Enabled = true;
            #endregion

            #region Titre
            //titre unité de base

            appmChart.Titles.Add(chartAreaUnit.Name);
            appmChart.Titles[0].DockInsideChartArea = true;
            appmChart.Titles[0].Position.Auto = false;
            appmChart.Titles[0].Position.X = 50;
            appmChart.Titles[0].Position.Y = positionY;
            //appmChart.Titles[0].Font = new Font("Arial", (float)10);
            //appmChart.Titles[0].Color = Color.FromArgb(100, 72, 131);
            appmChart.Titles[0].Font = new Font(_camembertTitleTextFontFamily1, (float)Convert.ToDouble(_camembertTitleTextFontSize1));
            appmChart.Titles[0].Color = camembertTitleTextFontColor1;
            appmChart.Titles[0].DockToChartArea = chartAreaUnit.Name;

            //titre unité pour CGRP
            if (chartAreaCgrp != null) {
                appmChart.Titles.Add(chartAreaCgrp.Name);
                appmChart.Titles[1].DockInsideChartArea = true;
                appmChart.Titles[1].Position.Auto = false;
                appmChart.Titles[1].Position.X = 50;
                if (dt.Rows.Count <= 8) {
                    appmChart.Titles[1].Position.Y = 46;
                }
                else
                    appmChart.Titles[1].Position.Y = 36;

                //appmChart.Titles[1].Font = new Font("Arial", (float)10);
                //appmChart.Titles[1].Color = Color.FromArgb(100, 72, 131);
                appmChart.Titles[1].Font = new Font(_histogrammeTitleTextFontFamily, (float)Convert.ToDouble(_histogrammeTitleTextFontSize));
                appmChart.Titles[1].Color = histogrammeTitleTextFontColor;
                appmChart.Titles[1].DockToChartArea = chartAreaCgrp.Name;
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
    }
}
