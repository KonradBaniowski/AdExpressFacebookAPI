#region Informations
// Auteur: Y. R'kaina
// Date de création: 26/01/2007 
// Date of Modification: 
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
using TNS.FrameWork;

using TNS.AdExpress.Web.Rules.Results.APPM;
using Dundas.Charting.WebControl;
using TNS.AdExpress.Domain.Units;

namespace TNS.AdExpress.Web.Controls.Results.Appm{
	/// <summary>
	/// Description résumée de SeasonalityPlanSectorDataChartWebControl.
	/// </summary>
	[ToolboxData("<{0}:SeasonalityPlanSectorDataChartWebControl runat=server></{0}:SeasonalityPlanSectorDataChartWebControl>")]
	public class SeasonalityPlanSectorDataChartWebControl : BaseAppmChartWebControl {

        #region variable
        /// <summary>
        /// Valeur hexadecimale des couleurs separé par le caractere ','
        /// </summary>
        private string _strPieColors = string.Empty;
        /// <summary>
        /// Famille de police du texte des unités en Abscisse
        /// </summary>
        private static string _abscisseTextFontFamily = "Arial";
        /// <summary>
        /// Famille de police du texte des unités en ordonnee Y
        /// </summary>
        private static string _ordonnee1TextFontFamily = "Arial";
        /// <summary>
        /// Famille de police du texte des unités en ordonnee Y2
        /// </summary>
        private static string _ordonnee2TextFontFamily = "Arial";

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
        /// Couleur de la famille de presse
        /// </summary>
        private static string _histogrammeColorFamille1 = "#9479B5";
        /// <summary>
        /// Couleur du texte de la legende de la famille de presse
        /// </summary>
        private static string _histogrammeFamille1LegendTextFontSize = "8";
        /// <summary>
        /// Couleur du texte de la legende de la famille de presse
        /// </summary>
        private static string _histogrammeFamille1LegendTextFontFamily = "Arial";
        /// <summary>
        /// Couleur de fond de l'histogramme
        /// </summary>
        private static string _histogrammeBackgroundColor = "#DECFE7";
        /// <summary>
        /// Taille de police du texte des unités en ordonnee sur l'histogramme
        /// </summary>
        private static string _histogrammeOrdonneeTextFontSize = "8";
        /// <summary>
        /// Taille de police du texte des unités en Abscisse sur l'histogramme
        /// </summary>
        private static string _histogrammeAbscisseTextFontSize = "8";

        /// <summary>
        /// Couleur du titre de le graph
        /// </summary>
        private static string _graphTitleTextFontColor = "#644883";
        /// <summary>
        /// Taille du titre de le graph
        /// </summary>
        private static string _graphTitleTextFontSize = "10";
        /// <summary>
        /// Famille de police de le graph
        /// </summary>
        private static string _graphTitleTextFontFamily = "Arial";
        /// <summary>
        /// Couleur de la famille de presse
        /// </summary>
        private static string _graphColorFamille1 = "#9479B5";
        /// <summary>
        /// Couleur du texte de la legende de la famille de presse
        /// </summary>
        private static string _graphFamille1LegendTextFontSize = "8";
        /// <summary>
        /// Couleur du texte de la legende de la famille de presse
        /// </summary>
        private static string _graphFamille1LegendTextFontFamily = "Arial";
        /// <summary>
        /// Couleur de fond de le graph
        /// </summary>
        private static string _graphBackgroundColor = "#DECFE7";
        /// <summary>
        /// Taille de police du texte des unités en ordonnee sur le graph
        /// </summary>
        private static string _graphOrdonneeTextFontSize = "8";
        /// <summary>
        /// Taille de police du texte des unités en Abscisse sur le graph
        /// </summary>
        private static string _graphAbscisseTextFontSize = "8";

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

        #region Assessor
        /// <summary>
        /// Obtient ou definis la Valeur hexadecimale des couleurs separé par le caractere ','
        /// </summary>
        public string PieColors {
            get { return _strPieColors; }
            set { _strPieColors = value; }
        }
        /// <summary>
        /// Obtient ou definis la Famille de police du texte des unités en abscisse
        /// </summary>
        public string AbscisseTextFontFamily {
            get { return _abscisseTextFontFamily; }
            set { _abscisseTextFontFamily = value; }
        }
        /// <summary>
        /// Obtient ou definis la Famille de police du texte des unités en ordonnee sur le graph
        /// </summary>
        public string Ordonnee1TextFontFamily {
            get { return _ordonnee1TextFontFamily; }
            set { _ordonnee1TextFontFamily = value; }
        }
        /// <summary>
        /// Obtient ou definis la Famille de police du texte des unités en ordonnee sur le graph
        /// </summary>
        public string Ordonnee2TextFontFamily {
            get { return _ordonnee2TextFontFamily; }
            set { _ordonnee2TextFontFamily = value; }
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
        /// Obtient ou definis la Couleur de la premiere famille de presse
        /// </summary>
        public string HistogrammeColorFamille1 {
            get { return _histogrammeColorFamille1; }
            set { _histogrammeColorFamille1 = value; }
        }
        /// <summary>
        /// Obtient ou definis la Taille du texte de la legende de la famille de presse
        /// </summary>
        public string HistogrammeFamille1LegendTextFontSize {
            get { return _histogrammeFamille1LegendTextFontSize; }
            set { _histogrammeFamille1LegendTextFontSize = value; }
        }
        /// <summary>
        /// Obtient ou definis la Famille de police du texte de la legende de la famille de presse
        /// </summary>
        public string HistogrammeFamille1LegendTextFontFamily {
            get { return _histogrammeFamille1LegendTextFontFamily; }
            set { _histogrammeFamille1LegendTextFontFamily = value; }
        }
        /// <summary>
        /// Obtient ou definis la Couleur de fond de l'histogramme
        /// </summary>
        public string HistogrammeBackgroundColor {
            get { return _histogrammeBackgroundColor; }
            set { _histogrammeBackgroundColor = value; }
        }
        /// <summary>
        /// Obtient ou definis la Taille de police du texte des unités en ordonnee sur l'histogramme
        /// </summary>
        public string HistogrammeOrdonneeTextFontSize {
            get { return _histogrammeOrdonneeTextFontSize; }
            set { _histogrammeOrdonneeTextFontSize = value; }
        }
        /// <summary>
        /// Obtient ou definis la Taille de police du texte des unités en abscisse sur l'histogramme
        /// </summary>
        public string HistogrammeAbscisseTextFontSize {
            get { return _histogrammeAbscisseTextFontSize; }
            set { _histogrammeAbscisseTextFontSize = value; }
        }

        /// <summary>
        /// Obtient ou definis la Couleur du titre de le graph
        /// </summary>
        public string GraphTitleTextFontColor {
            get { return _graphTitleTextFontColor; }
            set { _graphTitleTextFontColor = value; }
        }
        /// <summary>
        /// Obtient ou definis la Taille du titre de le graph
        /// </summary>
        public string GraphTitleTextFontSize {
            get { return _graphTitleTextFontSize; }
            set { _graphTitleTextFontSize = value; }
        }
        /// <summary>
        /// Obtient ou definis la Famille de police de le graph
        /// </summary>
        public string GraphTitleTextFontFamily {
            get { return _graphTitleTextFontFamily; }
            set { _graphTitleTextFontFamily = value; }
        }
        /// <summary>
        /// Obtient ou definis la Couleur de la premiere famille de presse
        /// </summary>
        public string GraphColorFamille1 {
            get { return _graphColorFamille1; }
            set { _graphColorFamille1 = value; }
        }
        /// <summary>
        /// Obtient ou definis la Taille du texte de la legende de la famille de presse
        /// </summary>
        public string GraphFamille1LegendTextFontSize {
            get { return _graphFamille1LegendTextFontSize; }
            set { _graphFamille1LegendTextFontSize = value; }
        }
        /// <summary>
        /// Obtient ou definis la Famille de police du texte de la legende de la famille de presse
        /// </summary>
        public string GraphFamille1LegendTextFontFamily {
            get { return _graphFamille1LegendTextFontFamily; }
            set { _graphFamille1LegendTextFontFamily = value; }
        }
        /// <summary>
        /// Obtient ou definis la Couleur de fond de le graph
        /// </summary>
        public string GraphBackgroundColor {
            get { return _graphBackgroundColor; }
            set { _graphBackgroundColor = value; }
        }
        /// <summary>
        /// Obtient ou definis la Taille de police du texte des unités en ordonnee sur le graph
        /// </summary>
        public string GraphOrdonneeTextFontSize {
            get { return _graphOrdonneeTextFontSize; }
            set { _graphOrdonneeTextFontSize = value; }
        }
        /// <summary>
        /// Obtient ou definis la Taille de police du texte des unités en abscisse sur le graph
        /// </summary>
        public string GraphAbscisseTextFontSize {
            get { return _graphAbscisseTextFontSize; }
            set { _graphAbscisseTextFontSize = value; }
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

        #region Constantes
        /// <summary>
		/// La position de Chart Area (Horizontal)
		/// </summary>
		const int CHART_AREA_POSITION_X=1;
		/// <summary>
		/// La position de Chart Area Unit (Vertical)
		/// </summary>
		const int UNIT_CHART_AREA_POSITION_Y=7;
		/// <summary>
		/// La position de Chart Area Destribution (Vertical)
		/// </summary>
		const int DISTRIBUTION_CHART_AREA_POSITION_Y=60;
		/// <summary>
		/// La largeur de Chart Area
		/// </summary>
		const int CHART_AREA_POSITION_WIDTH=100;
		/// <summary>
		/// La taille de Chart Area
		/// </summary>
		const int CHART_AREA_POSITION_HEIGHT=45;
		/// <summary>
		/// La position des libellés par rapport à l'axe des X
		/// </summary>
		const int SERIES_ANGLE=-90;
		/// <summary>
		/// La largeur de l'image
		/// </summary>
		const int CHART_WIDTH=600;
		/// <summary>
		/// La taille de l'image
		/// </summary>
		const int CHART_HEIGHT=1000;
		/// <summary>
		/// La position horizontale du titre
		/// </summary>
		const int TITLE_POSITION_X=50;
		/// <summary>
		/// La position verticale du titre
		/// </summary>
		const int UNIT_TITLE_POSITION_Y=2;
		/// <summary>
		/// La position verticale du titre
		/// </summary>
		const int DISTRIBUTION_TITLE_POSITION_Y=54;
		/// <summary>
		/// La taille des titres
		/// </summary>
		//const int TITLE_FONT_SIZE=10;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dataSource">Source de données</param>
		/// <param name="ImageType">Type de l'image (jpg, flash...)</param>
        public SeasonalityPlanSectorDataChartWebControl(WebSession webSession, TNS.FrameWork.DB.Common.IDataSource dataSource, ChartImageType ImageType)
            : base(webSession, dataSource, ImageType)
        {
		}
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="dataSource">Source de données</param>
        /// <param name="ImageType">Type de l'image (jpg, flash...)</param>
        /// <param name="skinID">Nom du skin du controle</param>
        public SeasonalityPlanSectorDataChartWebControl(WebSession webSession, TNS.FrameWork.DB.Common.IDataSource dataSource, ChartImageType ImageType, string skinID)
            : base(webSession, dataSource, ImageType) {
            this.SkinID = skinID;
        }
		#endregion

		#region Implémentation des méthodes abstraites 
		/// <summary>		
		/// Définit les données au moment du design du contrôle. 	
		/// </summary>
		public override void SetDesignMode(){
			
			#region variable
			DataTable seasonalityPlanData;
			ChartArea chartArea=new ChartArea();
			//Séries de valeurs pour chaque tranche du graphique
			double[]  yUnitValues = null;
			string[]  xUnitValues  = null;
			double[]  y1UnitValues = null;
			int chartWidth=0, fontSize=0;
			double rowCount=0, div=8;
			#endregion


            #region Initialisation des couleurs des graphiques
            ColorConverter cc = new ColorConverter();

            #region couleurs du graphique
            //couleurs des tranches du graphique
            Color[] pieColors = null;
            if (_strPieColors != null && _strPieColors.Length != 0) {
                string[] colorTemp = _strPieColors.Split(",".ToCharArray());
                pieColors = new Color[colorTemp.Length];
                for (int i = 0; i < colorTemp.Length; i++) {
                    pieColors[i] = (Color)cc.ConvertFromString(colorTemp[i]);
                }
            }
            else {
                Color[] colorTemp = {
								  Color.FromArgb(100,72,131),
								  Color.FromArgb(177,163,193),
								  Color.FromArgb(208,200,218),
								  Color.FromArgb(225,224,218),
								  Color.FromArgb(255,215,215),
								  Color.FromArgb(255,240,240),
								  Color.FromArgb(202,255,202),
								  Color.FromArgb(255,5,182),
								  Color.FromArgb(157,152,133),
								  Color.FromArgb(241,241,241),
								  Color.FromArgb(77,150,75),
								  Color.FromArgb(0,0,0)
							  };
                pieColors = colorTemp;
            }
            #endregion

            #region No Result
            Color textNoResultFontColor = (Color)cc.ConvertFromString(_textNoResultFontColor);
            #endregion

            #endregion

			try{

				
				if (this._customerWebSession.DetailPeriod == WebConstantes.CustomerSessions.Period.DisplayLevel.weekly){
					this.Titles.Add(GestionWeb.GetWebWord(2125,this._customerWebSession.SiteLanguage));
					//this.Titles[0].Font=new Font("Arial", (float)8,System.Drawing.FontStyle.Bold);
					//this.Titles[0].Color=Color.FromArgb(100,72,131);
                    this.Titles[0].Font = new Font(_textNoResultFontFamily, (float)Convert.ToDouble(_textNoResultFontSize), System.Drawing.FontStyle.Bold);
                    this.Titles[0].Color=textNoResultFontColor;
					this.Width=250;
					this.Height=30;
				}
				else{
					//Données				
					seasonalityPlanData=SectorDataSeasonalityRules.GetSeasonalityPreformatedData(this._customerWebSession,this._dataSource,this._idWave,this._dateBegin, this._dateEnd,this._idBaseTarget,this._idAdditionalTarget);
				
					if(seasonalityPlanData!=null && seasonalityPlanData.Rows.Count>0){

						  #region Création et définition du graphique pour les unités
								
						  #region Initialisation Chart Width
						  rowCount=seasonalityPlanData.Rows.Count/div;
						  rowCount=Math.Ceiling(rowCount);
						  if(seasonalityPlanData.Rows.Count<=40)
							  chartWidth = CHART_WIDTH + (int)(50 * rowCount);
						  else 
							  chartWidth = 900;
						  #endregion

						  #region Initialisation font Size
						  if(seasonalityPlanData.Rows.Count<=16)
							  fontSize=Convert.ToInt32(_histogrammeFamille1LegendTextFontSize)+1;
						  else
                              fontSize = Convert.ToInt32(_histogrammeFamille1LegendTextFontSize);
						  #endregion

						  #region Get Series Data
						  if (this._customerWebSession.Unit == WebConstantes.CustomerSessions.Unit.grp)
							  GetSeriesDataDistribution(this._customerWebSession,seasonalityPlanData,ref xUnitValues,ref yUnitValues,ref y1UnitValues);
						  else
							  GetSeriesDataBase(this._customerWebSession,seasonalityPlanData,ref xUnitValues,ref yUnitValues,ref y1UnitValues);
						  #endregion

						  //Création du graphique	des uités(euros, grp, insertion, page) cible de base
						  ChartArea chartAreaUnit=null;
						  Series serieSeasonality=new Series();	
					
						  #region Création de chart area & Alignement
						  //Conteneur graphique pour unité de cible de base
						  chartAreaUnit=new ChartArea();
				
						  //Alignement
						  chartAreaUnit.AlignOrientation = AreaAlignOrientation.Vertical;
						  chartAreaUnit.Position.X=CHART_AREA_POSITION_X;
						  chartAreaUnit.Position.Y=UNIT_CHART_AREA_POSITION_Y;
						  chartAreaUnit.Position.Width=CHART_AREA_POSITION_WIDTH;
						  chartAreaUnit.Position.Height=CHART_AREA_POSITION_HEIGHT;	
						  this.ChartAreas.Add(chartAreaUnit);
						  #endregion

						  #region Titres des graphiques
						  string unitName="";
						  string chartAreaName="";
						  string chartAreaDistributionName="";

						  #region sélection par rappot à l'unité choisit
						  switch (this._customerWebSession.Unit)
						  {
							  case WebConstantes.CustomerSessions.Unit.euro:  
								  unitName= GestionWeb.GetWebWord(2110,this._customerWebSession.SiteLanguage);
								  break;
							  case WebConstantes.CustomerSessions.Unit.kEuro :  
								  unitName= GestionWeb.GetWebWord(2111,this._customerWebSession.SiteLanguage);
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
                              case WebConstantes.CustomerSessions.Unit.numberPost:
                                  unitName = Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[WebConstantes.CustomerSessions.Unit.numberPost].WebTextId, _customerWebSession.SiteLanguage));
                                  break;							
						  }
						  #endregion
				
						  if (this._customerWebSession.Unit==WebConstantes.CustomerSessions.Unit.grp)
						  {
							  chartAreaName+=GestionWeb.GetWebWord(1736,this._customerWebSession.SiteLanguage);
							  chartAreaName+=" ";
							  chartAreaName+=unitName ;
							  chartAreaName+=" ("+seasonalityPlanData.Rows[0]["additionalTarget"]+") " ;
							  chartAreaName+=GestionWeb.GetWebWord(2112,this._customerWebSession.SiteLanguage);
						  }
						  else
						  {
							  chartAreaName+=GestionWeb.GetWebWord(1736,this._customerWebSession.SiteLanguage);
							  chartAreaName+=" ";
							  chartAreaName+= unitName ;
							  chartAreaName+=" "+GestionWeb.GetWebWord(2112,this._customerWebSession.SiteLanguage);					
						  } 
						  chartAreaDistributionName=GestionWeb.GetWebWord(2105,this._customerWebSession.SiteLanguage);
						  #endregion

						  serieSeasonality=SetSeriesSeasonality(seasonalityPlanData,chartAreaUnit,serieSeasonality,xUnitValues,yUnitValues,pieColors,chartAreaName,GestionWeb.GetWebWord(1795,this._customerWebSession.SiteLanguage),fontSize);												
						  #endregion		

						  #region Création et définition du graphique pour le poids de chaque période
						  //Création du graphique	pour unité
						  ChartArea chartAreaDistribution=null;
						  Series serieSeasonalityDistribution=new Series();	
						  //Conteneur graphique pour unité
						  chartAreaDistribution=new ChartArea();
				
						  //Alignement
						  chartAreaDistribution.AlignOrientation = AreaAlignOrientation.Vertical;
						  chartAreaDistribution.Position.X=CHART_AREA_POSITION_X;
						  chartAreaDistribution.Position.Y=DISTRIBUTION_CHART_AREA_POSITION_Y;
						  chartAreaDistribution.Position.Width=CHART_AREA_POSITION_WIDTH;
						  chartAreaDistribution.Position.Height=CHART_AREA_POSITION_HEIGHT;
						  this.ChartAreas.Add(chartAreaDistribution);
						  //Charger les séries de valeurs 
						  serieSeasonalityDistribution=SetSeriesSeasonalityDistribution(seasonalityPlanData,chartAreaDistribution,serieSeasonalityDistribution,xUnitValues,y1UnitValues,pieColors,chartAreaDistributionName,GestionWeb.GetWebWord(1743,this._customerWebSession.SiteLanguage),fontSize);
						  #endregion

						  #region initialisation du control pour le module Données de cadrage
						  InitializeComponent(this,chartAreaUnit,chartAreaDistribution,this._imageType,chartWidth,fontSize);
						  if(seasonalityPlanData!=null && seasonalityPlanData.Rows.Count>0)
						  {
							  this.Series.Add(serieSeasonality);
							  this.Series.Add(serieSeasonalityDistribution);
						  }
						  #endregion

						  AddCopyRight(this._customerWebSession, (_imageType == ChartImageType.Jpeg));
					 }
					 else{
						  this.Titles.Add(GestionWeb.GetWebWord(2106,this._customerWebSession.SiteLanguage));
						  //this.Titles[0].Font=new Font("Arial", (float)8,System.Drawing.FontStyle.Bold);
						  //this.Titles[0].Color=Color.FromArgb(100,72,131);
                          this.Titles[0].Font = new Font(_textNoResultFontFamily, (float)Convert.ToDouble(_textNoResultFontSize), System.Drawing.FontStyle.Bold);
                          this.Titles[0].Color = textNoResultFontColor;
						  this.Width=250;
						  this.Height=20;
					 }
				}
			}
			catch(System.Exception err){
				throw(new WebExceptions.SeasonalityPlanChartUIException("Erreur dans l'affichage des graphiques des données de saisonalité dans le module données de cadrage",err));
			}
		}
		#endregion

		#region méthodes privées

		#region GetSeriesDataBase
		/// <summary>
		/// Obtient les séries de valeurs des unités pour la cible de base à afficher graphiquement
		/// </summary>
		/// <param name="webSession">WebSession</param>
		/// <param name="dt">table de données</param>
		/// <param name="xValues">libellés du graphique</param>
		/// <param name="yValues">valeurs pour graphique</param>
		/// <param name="y1Values">valeurs pour graphique</param>
		private static void GetSeriesDataBase(WebSession webSession,DataTable dt,ref string[] xValues,ref double[] yValues,ref double[] y1Values){
	
			#region Variable
			string ventilationType="seasonality", period="";
			#endregion
			
			#region Les séries  d'unité à afficher graphiquement
			xValues = new string[dt.Rows.Count];				
			yValues = new double[dt.Rows.Count];
			y1Values = new double[dt.Rows.Count];

			for(int i=0; i<dt.Rows.Count;i++){
				if (webSession.DetailPeriod == WebConstantes.CustomerSessions.Period.DisplayLevel.weekly){
					period=dt.Rows[i][ventilationType].ToString();
					xValues[i]=period.Substring(4,2) + " (" + period.Substring(0,4)+")";
				}
				else
					xValues[i]=WebFunctions.Dates.getPeriodTxt(webSession,dt.Rows[i][ventilationType].ToString());
				
				if(webSession.Unit==WebConstantes.CustomerSessions.Unit.kEuro)
					yValues[i]=Math.Round(double.Parse(dt.Rows[i]["unitBase"].ToString()));
				else
					yValues[i]=double.Parse(dt.Rows[i]["unitBase"].ToString());
				y1Values[i]=double.Parse(dt.Rows[i]["distributionBase"].ToString());
			}
			#endregion
		}
		#endregion
		
		#region GetSeriesDataDistribution
		/// <summary>
		/// Obtient les séries de valeurs des unités pour la cible sélectionnée à afficher graphiquement
		/// </summary>
		/// <param name="webSession">WebSession</param>
		/// <param name="dt">table de données</param>
		/// <param name="xValues">libellés du graphique</param>
		/// <param name="yValues">valeurs pour graphique</param>
		/// <param name="y1Values">valeurs pour graphique</param>
		private static void GetSeriesDataDistribution(WebSession webSession,DataTable dt,ref string[] xValues,ref double[] yValues,ref double[] y1Values){
	
			#region Variable
			string ventilationType="seasonality", period="";
			#endregion
			
			#region Les séries  d'unité à afficher graphiquement
			xValues = new string[dt.Rows.Count];				
			yValues = new double[dt.Rows.Count];
			y1Values = new double[dt.Rows.Count];

			for(int i=0; i<dt.Rows.Count;i++){
				if (webSession.DetailPeriod == WebConstantes.CustomerSessions.Period.DisplayLevel.weekly){
					period=dt.Rows[i][ventilationType].ToString();
					xValues[i]=period.Substring(4,2) + " (" + period.Substring(0,4)+")";
				}
				else
					xValues[i]=WebFunctions.Dates.getPeriodTxt(webSession,dt.Rows[i][ventilationType].ToString());
			
				if(webSession.Unit==WebConstantes.CustomerSessions.Unit.kEuro)
					yValues[i]=Math.Round(double.Parse(dt.Rows[i]["unitSelected"].ToString()));
				else
					yValues[i]=double.Parse(dt.Rows[i]["unitSelected"].ToString());
				y1Values[i]=double.Parse(dt.Rows[i]["distributionSelected"].ToString());
			}
			#endregion
		}
		#endregion

		#region SetSeriesSeasonality
		/// <summary>
		/// Crétion du graphique unité(grp,euro,isertion,page) 
		/// </summary>
		/// <param name="dt">tableau de résultats</param>
		/// <param name="chartArea">contenant objet graphique</param>
		/// <param name="series">séries de valeurs</param>
		/// <param name="xValues">séries de libellés</param>
		/// <param name="yValues">séries de valeurs</param>
		/// <param name="barColors">couleurs du graphique</param>		
		/// <param name="chartAreaName">Nom du conteneur de l'image</param>
		/// <param name="legendText">Legende Texte</param>
		/// <param name="fontSize">Font size</param>
		/// <returns>séries de valeurs</returns>
		private static  Dundas.Charting.WebControl.Series SetSeriesSeasonality(DataTable dt ,ChartArea chartArea,Dundas.Charting.WebControl.Series series,string[] xValues,double[] yValues,Color[] barColors,string chartAreaName,string legendText,int fontSize){
			
			#region  Création graphique
            ColorConverter cc = new ColorConverter();
            Color histogrammeColorFamille1 = (Color)cc.ConvertFromString(_histogrammeColorFamille1);

			if(xValues!=null && yValues!=null){
								
				#region Création et définition du graphique
				//Type de graphique
				series.Type = SeriesChartType.Column;
				series.ShowLabelAsValue=true;
				series.XValueType=Dundas.Charting.WebControl.ChartValueTypes.String;
				series.YValueType=Dundas.Charting.WebControl.ChartValueTypes.Double;
				//series.Color= Color.FromArgb(148,121,181);
                series.Color = histogrammeColorFamille1;
				series.Enabled=true;
				//series.Font=new Font("Arial", (float)fontSize);
                series.Font = new Font(_histogrammeFamille1LegendTextFontFamily, (float)fontSize);
				series.FontAngle=SERIES_ANGLE;
				series["LabelStyle"] = "Top";
																
				chartArea.Name=chartAreaName;
				series.ChartArea=chartArea.Name;
				series.Points.DataBindXY(xValues,yValues);
				#endregion	

			}
			#endregion 

			return series;
		}
		#endregion

		#region SetSeriesSeasonalityDistribution
		/// <summary>
		/// Crétion du graphique unité(grp,euro,isertion,page) 
		/// </summary>
		/// <param name="dt">tableau de résultats</param>
		/// <param name="chartArea">contenant objet graphique</param>
		/// <param name="series">séries de valeurs</param>
		/// <param name="xValues">séries de libellés</param>
		/// <param name="yValues">séries de valeurs</param>
		/// <param name="barColors">couleurs du graphique</param>		
		/// <param name="chartAreaName">Nom du conteneur de l'image</param>
		/// <param name="legendText">Legende Texte</param>
		/// <param name="fontSize">Font size</param>
		/// <returns>séries de valeurs</returns>
		private static  Dundas.Charting.WebControl.Series SetSeriesSeasonalityDistribution(DataTable dt ,ChartArea chartArea,Dundas.Charting.WebControl.Series series,string[] xValues,double[] yValues,Color[] barColors,string chartAreaName,string legendText,int fontSize){
			
			#region  Création graphique
            ColorConverter cc = new ColorConverter();
            Color graphColorFamille1 = (Color)cc.ConvertFromString(_graphColorFamille1);

			if(xValues!=null && yValues!=null){
								
				#region Création et définition du graphique
				//Type de graphique
				series.Type = SeriesChartType.Line;
				series.ShowLabelAsValue=true;
				series.XValueType=Dundas.Charting.WebControl.ChartValueTypes.String;
				series.YValueType=Dundas.Charting.WebControl.ChartValueTypes.Double;
                series.Color = graphColorFamille1;
				series.Enabled=true;
				//series.Font=new Font("Arial", (float)fontSize);
                series.Font = new Font(_graphFamille1LegendTextFontFamily, (float)fontSize);
				series["LabelStyle"] = "Right";
				series.SmartLabels.Enabled = true;
				series.SmartLabels.CalloutLineStyle = ChartDashStyle.Dot;
				series.SmartLabels.MinMovingDistance = 3;
															
				chartArea.Name=chartAreaName;
				series.ChartArea=chartArea.Name;
				series.Points.DataBindXY(xValues,yValues);
				#endregion	

			}
			#endregion 

			return series;
		}
		#endregion

		#region InitializeComponent
		/// <summary>
		/// Initialise les styles du webcontrol pour média radio et télé
		/// </summary>
		/// <param name="sectorDataChart">Objet Webcontrol</param>
		/// <param name="chartAreaUnit">conteneur de l'image répartition unité</param>
		/// <param name="chartAreaDistribution">conteneur de l'image distribution</param>
		/// <param name="ImageType">sortie flash</param>
		/// <param name="chart_width">Taille de l'image</param>
		/// <param name="fontSize">Font size</param>
		private static void InitializeComponent(BaseAppmChartWebControl sectorDataChart, ChartArea chartAreaUnit, ChartArea chartAreaDistribution,ChartImageType ImageType,int chart_width,int fontSize){

            ColorConverter cc = new ColorConverter();
            Color controlBorderColor = (Color)cc.ConvertFromString(_controlBorderColor);
            Color histogrammeBackgroundColor = (Color)cc.ConvertFromString(_histogrammeBackgroundColor);
            Color graphBackgroundColor = (Color)cc.ConvertFromString(_graphBackgroundColor);
            Color histogrammeTitleTextFontColor = (Color)cc.ConvertFromString(_histogrammeTitleTextFontColor);
            Color graphTitleTextFontColor = (Color)cc.ConvertFromString(_graphTitleTextFontColor);

			//Type image
			sectorDataChart.ImageType=ImageType;
			if(ImageType==ChartImageType.Flash){
				sectorDataChart.AnimationTheme =AnimationTheme.GrowingTogether;
				sectorDataChart.AnimationDuration =  0.6;
				sectorDataChart.RepeatAnimation = false;
			}	

			#region Chart
			sectorDataChart.BackGradientType = GradientType.TopBottom;
			//sectorDataChart.ChartAreas[chartAreaUnit.Name].BackColor=Color.FromArgb(222,207,231);		
			//sectorDataChart.ChartAreas[chartAreaDistribution.Name].BackColor=Color.FromArgb(222,207,231);
            sectorDataChart.ChartAreas[chartAreaUnit.Name].BackColor = histogrammeBackgroundColor;
            sectorDataChart.ChartAreas[chartAreaDistribution.Name].BackColor=graphBackgroundColor;
			sectorDataChart.BorderStyle=ChartDashStyle.Solid;
			//sectorDataChart.BorderLineColor=Color.FromArgb(99,73,132);
			//sectorDataChart.BorderLineWidth=2;
            sectorDataChart.BorderLineColor = controlBorderColor;
            sectorDataChart.BorderLineWidth=Convert.ToInt32(_controlBorderSize);

			sectorDataChart.Width=new Unit(""+chart_width+"px");
			sectorDataChart.Height=new Unit(""+CHART_HEIGHT+"px");
			sectorDataChart.Legend.Enabled=false;

			#region Axe des X
			SetAxisX(sectorDataChart,chartAreaUnit.Name,Convert.ToInt32(_histogrammeAbscisseTextFontSize));
            SetAxisX(sectorDataChart, chartAreaDistribution.Name, Convert.ToInt32(_graphAbscisseTextFontSize));
			#endregion

			#region Axe des Y
            SetAxisY(sectorDataChart, chartAreaUnit.Name, Convert.ToInt32(_histogrammeOrdonneeTextFontSize));
            SetAxisY(sectorDataChart, chartAreaDistribution.Name, Convert.ToInt32(_graphOrdonneeTextFontSize));
			#endregion

			#region Axe des Y2
            SetAxisY2(sectorDataChart, chartAreaUnit.Name, Convert.ToInt32(_histogrammeOrdonneeTextFontSize));
            SetAxisY2(sectorDataChart, chartAreaDistribution.Name, Convert.ToInt32(_graphOrdonneeTextFontSize));
			#endregion

			#endregion	

			#region Titre
			//titre unité de base			
			sectorDataChart.Titles.Add(chartAreaUnit.Name);
			sectorDataChart.Titles[0].DockInsideChartArea=true;
			sectorDataChart.Titles[0].Position.X=TITLE_POSITION_X;
			sectorDataChart.Titles[0].Position.Y=UNIT_TITLE_POSITION_Y;
			//sectorDataChart.Titles[0].Font=new Font("Arial", (float)TITLE_FONT_SIZE);
			//sectorDataChart.Titles[0].Color=Color.FromArgb(100,72,131);
            sectorDataChart.Titles[0].Font = new Font(_histogrammeTitleTextFontFamily, (float)Convert.ToDouble(_histogrammeTitleTextFontSize));
            sectorDataChart.Titles[0].Color = histogrammeTitleTextFontColor;
			sectorDataChart.Titles[0].DockToChartArea=chartAreaUnit.Name;

			sectorDataChart.Titles.Add(chartAreaDistribution.Name);
			sectorDataChart.Titles[1].DockInsideChartArea=true;
			sectorDataChart.Titles[1].Position.Auto = false;
			sectorDataChart.Titles[1].Position.X = TITLE_POSITION_X;
			sectorDataChart.Titles[1].Position.Y = DISTRIBUTION_TITLE_POSITION_Y;
			//sectorDataChart.Titles[1].Font=new Font("Arial", (float)TITLE_FONT_SIZE);
			//sectorDataChart.Titles[1].Color=Color.FromArgb(100,72,131);
            sectorDataChart.Titles[1].Font = new Font(_graphTitleTextFontFamily, (float)Convert.ToDouble(_graphTitleTextFontSize));
            sectorDataChart.Titles[1].Color = graphTitleTextFontColor;
			sectorDataChart.Titles[1].DockToChartArea=chartAreaDistribution.Name;
			#endregion
		}
		#endregion

		#region Set AxisX
		/// <summary>
		/// Paramétrages de l'axe des X
		/// </summary>
		/// <param name="sectorDataChart">Objet Webcontrol</param>
		/// <param name="chartAreaName">Le nom de la chart Area</param>
		/// <param name="fontSize">Font size</param>
		private static void SetAxisX(BaseAppmChartWebControl sectorDataChart, string chartAreaName,int fontSize){
			sectorDataChart.ChartAreas[chartAreaName].AxisX.LabelStyle.Enabled = true;
			sectorDataChart.ChartAreas[chartAreaName].AxisX.LabelsAutoFit = false;
			//sectorDataChart.ChartAreas[chartAreaName].AxisX.LabelStyle.Font=new Font("Arial", (float)fontSize);
            sectorDataChart.ChartAreas[chartAreaName].AxisX.LabelStyle.Font = new Font(_abscisseTextFontFamily, (float)fontSize);
			sectorDataChart.ChartAreas[chartAreaName].AxisX.MajorGrid.LineWidth=0;
			sectorDataChart.ChartAreas[chartAreaName].AxisX.Interval=1;				
			sectorDataChart.ChartAreas[chartAreaName].AxisX.LabelStyle.FontAngle = SERIES_ANGLE;
		}
		#endregion

		#region Set AxisY
		/// <summary>
		/// Paramétrages de l'axe des y
		/// </summary>
		/// <param name="sectorDataChart">Objet Webcontrol</param>
		/// <param name="chartAreaName">Le nom de la chart Area</param>
		/// 		/// <param name="fontSize">Font size</param>
		private static void SetAxisY(BaseAppmChartWebControl sectorDataChart, string chartAreaName,int fontSize){
			sectorDataChart.ChartAreas[chartAreaName].AxisY.Enabled=AxisEnabled.True;
			sectorDataChart.ChartAreas[chartAreaName].AxisY.LabelStyle.Enabled=true;
			sectorDataChart.ChartAreas[chartAreaName].AxisY.LabelsAutoFit=false;			
			sectorDataChart.ChartAreas[chartAreaName].AxisY.LabelStyle.Font=new Font(_ordonnee1TextFontFamily, (float)fontSize);
			sectorDataChart.ChartAreas[chartAreaName].AxisY.TitleFont=new Font("Arial", (float)fontSize);
			sectorDataChart.ChartAreas[chartAreaName].AxisY.MajorGrid.LineWidth=0;
		}
		#endregion

		#region Set AxisY2
		/// <summary>
		/// Paramétrages de l'axe des y2
		/// </summary>
		/// <param name="sectorDataChart">Objet Webcontrol</param>
		/// <param name="chartAreaName">Le nom de la chart Area</param>
		/// 		/// <param name="fontSize">Font size</param>
		private static void SetAxisY2(BaseAppmChartWebControl sectorDataChart, string chartAreaName,int fontSize){
			sectorDataChart.ChartAreas[chartAreaName].AxisY2.Enabled=AxisEnabled.True;
			sectorDataChart.ChartAreas[chartAreaName].AxisY2.LabelStyle.Enabled=true;
			sectorDataChart.ChartAreas[chartAreaName].AxisY2.LabelsAutoFit=false;
			sectorDataChart.ChartAreas[chartAreaName].AxisY2.LabelStyle.Font=new Font(_ordonnee2TextFontFamily, (float)fontSize);
			sectorDataChart.ChartAreas[chartAreaName].AxisY2.TitleFont=new Font("Arial", (float)fontSize);
		}
		#endregion

		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output)
		{
            //On cree le design ici pour pouvoir appliquer le skin a ce controle, 
            //Si le design est appelé avant, les membres du skin ne sont pas appliqué a ce controle
            SetDesignMode();
			base.Render(output);
		}
		#endregion

	}
}
