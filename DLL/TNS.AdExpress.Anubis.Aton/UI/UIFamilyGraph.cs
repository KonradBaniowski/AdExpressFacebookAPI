#region Information
// Author : Y. R'kaina
// Creation : 09/02/2007
// Modifications :
#endregion

using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;

using WebConstantes=TNS.AdExpress.Constantes.Web;
using CstUI = TNS.AdExpress.Constantes.Web.UI;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using WebFunctions=TNS.AdExpress.Web.Functions;

using TNS.AdExpress.Anubis.Aton.Common;
using TNS.AdExpress.Anubis.Aton.Exceptions;
using TNS.AdExpress.Web.Rules.Results.APPM;

using Dundas.Charting.WinControl;
using TNS.FrameWork.DB.Common;
using System.Collections.Generic;

namespace TNS.AdExpress.Anubis.Aton.UI{
	/// <summary>
	/// Description résumée de UIFamilyGraph.
	/// </summary>
	public class UIFamilyGraph:Chart{

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
		/// APPM configuration
		/// </summary>
		private AtonConfig _config = null;
		/// <summary>
		/// Data reserved to the calcul of PDVs
		/// </summary>
		private DataTable _dtGraphicsData = null;
        /// <summary>
        /// Style
        /// </summary>
        private TNS.FrameWork.WebTheme.Style _style = null;
        /// <summary>
        /// Pie ColorS
        /// </summary>
        private List<Color> _newPieColors = null;
		#endregion

		#region Constructeur
        public UIFamilyGraph(WebSession webSession, IDataSource dataSource, AtonConfig config, DataTable dtGraphicsData, TNS.FrameWork.WebTheme.Style style)
            : base() {
			_webSession = webSession;
			_dataSource = dataSource;
			_config = config;
			_dtGraphicsData = dtGraphicsData;
            _style = style;

            _newPieColors = ((TNS.FrameWork.WebTheme.Colors)_style.GetTag("FamilyGraphNewPieColors32")).ColorList;
		}
		#endregion

		#region SetDesignMode
		/// <summary>		
		/// Définit les données au moment du design du contrôle. 	
		/// </summary>
		public void SetDesignMode(){
		
			#region variable
			ChartArea chartArea=new ChartArea();
			//Séries de valeurs pour chaque tranche du graphique
			double[]  yUnitValues = null;
			string[]  xUnitValues  = null;
			#endregion

			try{
					
				#region Initialisation
				float areaUnitPositionHeight=0, areaUnitPositionY=0;
				
				areaUnitPositionY=12;
				if(_dtGraphicsData.Rows.Count<=8){
					areaUnitPositionHeight=70;
				}
				else
					areaUnitPositionHeight=80;
				#endregion
				
				#region Création et définition du graphique pour la cible de base
				if(_dtGraphicsData!=null && _dtGraphicsData.Rows.Count>0)
					getSeriesDataBase(_dtGraphicsData,ref xUnitValues,ref yUnitValues);	
			
				//Création du graphique	des uités(euros, grp, insertion, page) cible de base
				ChartArea chartAreaUnit=null;
				Series serieInterestFamily=new Series();

				if(_dtGraphicsData!=null && _dtGraphicsData.Rows.Count>0){
					//Conteneur graphique pour unité de cible de base
					chartAreaUnit=new ChartArea();

					#region dimension du Camembert
					//Alignement
					chartAreaUnit.AlignOrientation = AreaAlignOrientation.Vertical;
					chartAreaUnit.Position.X=2;
					if(_dtGraphicsData.Rows.Count<=8){
						chartAreaUnit.Position.Y=areaUnitPositionY;
						chartAreaUnit.Position.Height=areaUnitPositionHeight;
					}
					else{
						chartAreaUnit.Position.Y=areaUnitPositionY;
						chartAreaUnit.Position.Height=areaUnitPositionHeight;
					}
					chartAreaUnit.Position.Width=96;
					#endregion

					this.ChartAreas.Add(chartAreaUnit);
					//Charger les séries de valeurs 
					string unitName="";
					string chartAreaName="";

					#region sélection par rappot à l'unité choisit
					switch (_webSession.Unit){
						case WebConstantes.CustomerSessions.Unit.euro:  
							unitName= GestionWeb.GetWebWord(2110,_webSession.SiteLanguage);
							break;
						case WebConstantes.CustomerSessions.Unit.kEuro:  
							unitName= GestionWeb.GetWebWord(2111,_webSession.SiteLanguage);
							break;
						case WebConstantes.CustomerSessions.Unit.grp:  
							unitName= GestionWeb.GetWebWord(1679,_webSession.SiteLanguage);						
							break;
						case WebConstantes.CustomerSessions.Unit.insertion:  
							unitName= GestionWeb.GetWebWord(940,_webSession.SiteLanguage);
							break;
						case WebConstantes.CustomerSessions.Unit.pages:  
							unitName= GestionWeb.GetWebWord(566,_webSession.SiteLanguage);
							break;
						default : break;
					}

					#region Titres du graphiques
					//Titre graphique cible de base
					if (_webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
						chartAreaName+=GestionWeb.GetWebWord(1736,_webSession.SiteLanguage);
						chartAreaName+=" ";
						chartAreaName+= unitName ;
						chartAreaName+=" ("+_dtGraphicsData.Rows[0]["baseTarget"]+") " ;
						chartAreaName+=GestionWeb.GetWebWord(1737,_webSession.SiteLanguage);
					}
					else{
						chartAreaName+=GestionWeb.GetWebWord(1736,_webSession.SiteLanguage);
						chartAreaName+=" ";
						chartAreaName+= unitName ;			
						chartAreaName+=" "+GestionWeb.GetWebWord(1737,_webSession.SiteLanguage);
					}
					#endregion

					#endregion
					
					serieInterestFamily=setSeriesInterestFamily(this,_dtGraphicsData,chartAreaUnit,serieInterestFamily,xUnitValues,yUnitValues,_newPieColors,chartAreaName);												

					serieInterestFamily.ShowInLegend=false;				

					#region initialisation du control pour le module Données de cadrage
					InitializeComponent(_dtGraphicsData ,this,chartAreaUnit);

					if(_dtGraphicsData!=null && _dtGraphicsData.Rows.Count>0){
						this.Series.Add(serieInterestFamily);
					}	
					#endregion

				}
				else{
					this.Titles.Add(GestionWeb.GetWebWord(2106,_webSession.SiteLanguage));
					this.Titles[0].Font=new Font("Arial", (float)8,System.Drawing.FontStyle.Bold);
					this.Titles[0].Color=Color.FromArgb(100,72,131);
					this.Width=250;
					this.Height=25;
				}
				#endregion
			}
			catch(System.Exception err){
				throw(new AtonPdfException("Erreur dans l'affichage du graphique FamilyInterestPlanChart ",err));
			}
		}
		#endregion

		#region SetGRPDesignMode
		/// <summary>		
		/// Définit les données au moment du design du contrôle. 	
		/// </summary>
		public void SetGRPDesignMode(){
		
			#region variable
			ChartArea chartArea=new ChartArea();
			//Séries de valeurs pour chaque tranche du graphique
			double[]  yUnitValues = null;
			string[]  xUnitValues  = null;
			#endregion

			try{
					
				#region Initialisation
				float areaUnitadditionalPositionHeight=0, areaUnitadditionalPositionY=0;
				
				areaUnitadditionalPositionY=12;
				if(_dtGraphicsData.Rows.Count<=8){
					areaUnitadditionalPositionHeight=70;
				}
				else
					areaUnitadditionalPositionHeight=80;
				#endregion
				
				#region Création et définition du graphique pour la cible
				if(_dtGraphicsData!=null && _dtGraphicsData.Rows.Count>0)
					GetSeriesDataAdditional(_dtGraphicsData,ref xUnitValues,ref yUnitValues);
			
				//Création du graphique	des uités(euros, grp, insertion, page) cible de base
				ChartArea chartAreaUnitadditional=null;
				Series serieInterestFamilyadditional=new Series();

				if(_dtGraphicsData!=null && _dtGraphicsData.Rows.Count>0){
					//Conteneur graphique pour unité de cible de base
					chartAreaUnitadditional=new ChartArea();

					#region dimension 1er Camembert
					//Alignement
					chartAreaUnitadditional.AlignOrientation = AreaAlignOrientation.Vertical;
					chartAreaUnitadditional.Position.X=2;
					if(_dtGraphicsData.Rows.Count<=8){
						chartAreaUnitadditional.Position.Y=areaUnitadditionalPositionY;
						chartAreaUnitadditional.Position.Height=areaUnitadditionalPositionHeight;
					}
					else{
						chartAreaUnitadditional.Position.Y=areaUnitadditionalPositionY;
						chartAreaUnitadditional.Position.Height=areaUnitadditionalPositionHeight;
					}
					chartAreaUnitadditional.Position.Width=96;
					#endregion

					this.ChartAreas.Add(chartAreaUnitadditional);
					//Charger les séries de valeurs 
					string unitName="";
					string chartAreaAdditionalName="";

					unitName= GestionWeb.GetWebWord(1679,_webSession.SiteLanguage);						

					#region Titres du graphiques
					//Titre graphique cible selectionnée
					chartAreaAdditionalName+=GestionWeb.GetWebWord(1736,_webSession.SiteLanguage);
					chartAreaAdditionalName+=" ";
					chartAreaAdditionalName+=unitName ;
					chartAreaAdditionalName+=" ("+_dtGraphicsData.Rows[0]["additionalTarget"]+") " ;
					chartAreaAdditionalName+=GestionWeb.GetWebWord(1737,_webSession.SiteLanguage);
					#endregion

                    serieInterestFamilyadditional = setSeriesInterestFamily(this, _dtGraphicsData, chartAreaUnitadditional, serieInterestFamilyadditional, xUnitValues, yUnitValues, _newPieColors, chartAreaAdditionalName);												

					serieInterestFamilyadditional.ShowInLegend=false;				

					#region initialisation du control pour le module Données de cadrage
					InitializeComponent(_dtGraphicsData ,this,chartAreaUnitadditional);

					if(_dtGraphicsData!=null && _dtGraphicsData.Rows.Count>0){
						this.Series.Add(serieInterestFamilyadditional);
					}	
					#endregion

				}
				else{
					this.Titles.Add(GestionWeb.GetWebWord(2106,_webSession.SiteLanguage));
                    _style.GetTag("FamilyGraphDefaultFont").SetStyleDundas(this.Titles[0]);
					this.Width=250;
					this.Height=25;
				}
				#endregion

			}
			catch(System.Exception err){
				throw(new AtonPdfException("Erreur dans l'affichage du graphique FamilyInterestPlanChart ",err));
			}
		}
		#endregion

		#region Méthodes privées
		/// <summary>
		/// Obtient les séries de valeurs des unités pour la cible de base à afficher graphiquement
		/// </summary>
		/// <param name="dt">table de données</param>
		/// <param name="xValues">libellés du graphique</param>
		/// <param name="yValues">valeurs pour graphique</param>
		private static void getSeriesDataBase(DataTable dt,ref string[] xValues,ref double[] yValues){
	
			#region Variable
			int x=0; 
			int y=0;
			#endregion
			
			#region Les séries  d'unité à afficher graphiquement
			xValues = new string[dt.Rows.Count];				
			yValues = new double[dt.Rows.Count];
			string[] zValues = new string[dt.Rows.Count];				
			for(int i=1; i<dt.Rows.Count-1;i++){
				xValues[x]=dt.Rows[i]["InterestFamily"].ToString();
				yValues[y]=double.Parse(dt.Rows[i]["distributionBase"].ToString());
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
		private static void GetSeriesDataAdditional(DataTable dt,ref string[] xValues,ref double[] yValues){
	
			#region Variable
			int x=0;
			int y=0;
			#endregion
			
			#region Les séries  d'unité à afficher graphiquement
			xValues = new string[dt.Rows.Count];				
			yValues = new double[dt.Rows.Count];			
			for(int i=1; i<dt.Rows.Count-1;i++){
				xValues[x]=dt.Rows[i]["InterestFamily"].ToString();
				yValues[y]=double.Parse(dt.Rows[i]["distributionSelected"].ToString());
				x++;
				y++;
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
		private static  Dundas.Charting.WinControl.Series setSeriesInterestFamily(Chart appmChart,DataTable dt ,ChartArea chartArea,Dundas.Charting.WinControl.Series series,string[] xValues,double[] yValues,List<Color> barColors,string chartAreaName){
	
			#region  Création graphique
			if(xValues!=null && yValues!=null){
				
				#region Création et définition du graphique
				//Création du graphique							
				
				//Type de graphique
				series.Type=SeriesChartType.Pie;
				series.XValueType=ChartValueTypes.String;
				series.YValueType=ChartValueTypes.Double;	
						
				series.Enabled=true;
																
				chartArea.Area3DStyle.Enable3D = true; 
				chartArea.Name=chartAreaName;
				series.ChartArea=chartArea.Name;
				series.Points.DataBindXY(xValues,yValues);
				
				appmChart.DataManipulator.Sort(PointsSortOrder.Descending,series);
				
				#region Définition des couleurs
				//couleur du graphique
                for (int k = 0; k < dt.Rows.Count && k < barColors.Count; k++) {
					series.Points[k].Color=barColors[k];
				}
				#endregion

				#region Légende
				series["LabelStyle"]="Outside";
				series.LegendToolTip = "#PERCENT";
				series["PieLineColor"]="Black";
			
				#endregion	

				series["LabelStyle"]="Outside";
				series.Label="#PERCENT : #VALX ";
				series.ToolTip = "#VALX";
				for (int i=0;i<xValues.Length;i++){
					series.Points[i]["Exploded"] = "true";
				}

				series.LegendText="#VALX";
				#endregion	
			}
			#endregion 

			return series;
        }

        #region SetSeriesBarInterestFamily (Non utilisé)
        /*
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
		private static  Dundas.Charting.WinControl.Series SetSeriesBarInterestFamily(DataTable dt ,ChartArea chartArea,Dundas.Charting.WinControl.Series series,string[] xValues,double[] yValues,System.Drawing.Color barColor,string chartAreaName,double maxScale){
			
			#region  Création graphique
			if(xValues!=null && yValues!=null){
				
				#region Création et définition du graphique
				//Création du graphique							
				
				//Type de graphique
				series.Type= SeriesChartType.Bar;
				series.XValueType=ChartValueTypes.String;
				series.YValueType=ChartValueTypes.Double;								
				series.Enabled=true;
																
				chartArea.Area3DStyle.Enable3D = false; 
				chartArea.BackColor =Color.FromArgb(222,207,231);
				chartArea.Name=chartAreaName;
				series.ChartArea=chartArea.Name;
				chartArea.AxisY.Maximum= maxScale+1000;
				chartArea.AxisX.Maximum=dt.Rows.Count;
				series.Points.DataBindXY(xValues,yValues);
				chartArea.AxisX.LabelStyle.Font = new Font("Arial", 8);
				chartArea.AxisY.LabelStyle.Font = new Font("Arial", 8);

				chartArea.AxisX.Interval=1;
				chartArea.AxisX.Margin=true;
			
				chartArea.AxisX.LabelStyle.ShowEndLabels = true;
	
				#region Définition des couleurs
				//couleur du graphique
				series.Color= barColor;
				#endregion

				#region Légende
				series["LabelStyle"]="Outside";
				series["PointWidth"] = "1.0";
				series.ToolTip = "#VALX : #VALY";
				series["PieLineColor"]="Black";
				#endregion

				series.Label="#VALY"; 
				#endregion	
			}
			#endregion 

			return series;
		}*/
        #endregion

        #region InitializeComponent (Non utilisé)
        /*
        /// <summary>
		/// Initialise les styles du webcontrol pour média radio et télé
		/// </summary>
		/// <param name="dt">tableau de données</param>
		/// <param name="appmChart">Objet Webcontrol</param>
		/// <param name="chartAreaUnit">conteneur de l'image répartition unité</param>
		/// <param name="chartAreaUnitadditional">conteneur de l'image répartition pour cible selectionnée</param>
		private static void InitializeComponent(DataTable dt ,Chart appmChart,ChartArea chartAreaUnit,ChartArea chartAreaUnitadditional){			
						
			#region Initialisation
			int height = 0;
			float positionY = 0;
			
			//Height
			if(dt.Rows.Count<6){
				height=700;
			}
			else if(dt.Rows.Count>=6 && dt.Rows.Count<16 ){
				height=800;
			}
			if(dt.Rows.Count>=16){
				height=900;
			}
			//Position Y
			if(dt.Rows.Count<=8){
				positionY = 56;
			}
			else
				positionY = 52;
			#endregion

			#region Chart
			appmChart.Width=900;
			appmChart.Height=height;
			appmChart.BackGradientType = GradientType.TopBottom;
			appmChart.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);											
			appmChart.BorderStyle=ChartDashStyle.Solid;
			appmChart.BorderLineColor=Color.FromArgb(99,73,132);
			appmChart.BorderLineWidth=2;
			appmChart.Legend.Enabled=true;
			#endregion	

			#region Titre
			//titre unité de base
			appmChart.Titles.Add(chartAreaUnit.Name);
			appmChart.Titles[0].DockInsideChartArea=true;
			appmChart.Titles[0].Position.Auto = false;
			appmChart.Titles[0].Position.X = 50;
			appmChart.Titles[0].Position.Y = 3;
			appmChart.Titles[0].Font=new Font("Arial", (float)10);
			appmChart.Titles[0].Color=Color.FromArgb(100,72,131);
			appmChart.Titles[0].DockToChartArea=chartAreaUnit.Name;	
			
			//titre unité pour la cible selectionnée
			appmChart.Titles.Add(chartAreaUnitadditional.Name);
			appmChart.Titles[1].DockInsideChartArea=true;
			appmChart.Titles[1].Position.Auto = false;
			appmChart.Titles[1].Position.X = 50;
			appmChart.Titles[1].Position.Y = positionY;
			appmChart.Titles[1].Font=new Font("Arial", (float)10);
			appmChart.Titles[1].Color=Color.FromArgb(100,72,131);
			appmChart.Titles[1].DockToChartArea=chartAreaUnitadditional.Name;
			#endregion
		}
        */
        #endregion

		/// <summary>
		/// Initialise les styles du webcontrol pour média radio et télé
		/// </summary>
		/// <param name="dt">tableau de données</param>
		/// <param name="appmChart">Objet Webcontrol</param>
		/// <param name="chartAreaUnit">conteneur de l'image répartition unité</param>
		private void InitializeComponent(DataTable dt ,Chart appmChart, ChartArea chartAreaUnit){			

			#region Initialisation
			string TagName = string.Empty;
			float positionY=0;
			
			positionY=8;
			if(dt.Rows.Count<6){
                TagName = "FamilyGraphSizeLittle";
			}
			else if(dt.Rows.Count>=6 && dt.Rows.Count<16 ){
                TagName = "FamilyGraphSizeMedium";
			}
			if(dt.Rows.Count>=16){
                TagName = "FamilyGraphSizeBig";
			}
			#endregion

			#region Chart
            _style.GetTag(TagName).SetStyleDundas(this);
			appmChart.BackGradientType = GradientType.TopBottom;
            _style.GetTag("FamilyGraphLineEnCircle").SetStyleDundas(this);
			appmChart.Legend.Enabled=true;
			#endregion	

			#region Titre
			//titre unité de base
			
			appmChart.Titles.Add(chartAreaUnit.Name);
			appmChart.Titles[0].DockInsideChartArea=true;
			appmChart.Titles[0].Position.Auto = false;
			appmChart.Titles[0].Position.X = 50;
			appmChart.Titles[0].Position.Y = positionY;
            _style.GetTag("FamilyGraphTitleFont").SetStyleDundas(appmChart.Titles[0]);
			appmChart.Titles[0].DockToChartArea=chartAreaUnit.Name;	
			#endregion
		}

		#endregion

	}
}
