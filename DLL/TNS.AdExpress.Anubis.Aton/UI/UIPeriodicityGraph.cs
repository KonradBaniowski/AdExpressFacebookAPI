#region Information
// Author : Y. R'kaina
// Creation : 12/02/2007
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
	/// Description résumée de UIPeriodicityGraph.
	/// </summary>
	public class UIPeriodicityGraph:Chart{

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
        private static TNS.FrameWork.WebTheme.Style _style = null;
        /// <summary>
        /// Pie ColorS
        /// </summary>
        private List<Color> _newPieColors = null;
		#endregion

		#region Constructeur
        public UIPeriodicityGraph(WebSession webSession, IDataSource dataSource, AtonConfig config, DataTable dtGraphicsData, TNS.FrameWork.WebTheme.Style style)
            : base() {
			_webSession = webSession;
			_dataSource = dataSource;
			_config = config;
			_dtGraphicsData = dtGraphicsData;
            _style = style;

            _newPieColors = ((TNS.FrameWork.WebTheme.Colors)_style.GetTag("PeriodicityGraphNewPieColors32")).ColorList;
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

			#region Constantes
			//couleurs des tranches du graphique
            List<Color> pieColors = ((TNS.FrameWork.WebTheme.Colors)_style.GetTag("PeriodicityGraphPieColors12")).ColorList;
            List<Color> barColors = ((TNS.FrameWork.WebTheme.Colors)_style.GetTag("PeriodicityGraphBarColors")).ColorList;
			#endregion

			#region Initialisation
			float areaUnitPositionHeight=0, areaLegendPositionY=0, areaLegendPositionHeight=0, areaUnitadditionalPositionY=0, areaUnitadditionalPositionHeight=0;	
			
			if (_webSession.Unit == WebConstantes.CustomerSessions.Unit.grp){
				areaUnitPositionHeight = 43;
				areaLegendPositionY = 39;
				areaLegendPositionHeight = 10;
				areaUnitadditionalPositionY=55;
				areaUnitadditionalPositionHeight=43;
			}
			else{
				areaUnitPositionHeight = 68;
				areaLegendPositionY = 66;
				areaLegendPositionHeight = 16;
			}
			#endregion

			try{
								
				#region Création et définition du graphique pour la cible de base
				if(_dtGraphicsData!=null && _dtGraphicsData.Rows.Count>0)
					GetSeriesDataBase(_dtGraphicsData,ref xUnitValues,ref yUnitValues);				
				//Création du graphique	des uités(euros, grp, insertion, page) cible de base
				ChartArea chartAreaUnit=null;
				Series seriePeriodicity=new Series();	
				if(_dtGraphicsData!=null && _dtGraphicsData.Rows.Count>0){																	
					//Conteneur graphique pour unité de cible de base
					chartAreaUnit=new ChartArea();
					//Alignement
					if (_webSession.Unit == WebConstantes.CustomerSessions.Unit.grp){
						chartAreaUnit.AlignOrientation = AreaAlignOrientation.Vertical;
						chartAreaUnit.Position.X=5;
						chartAreaUnit.Position.Y=2;
						chartAreaUnit.Position.Width=80;
						chartAreaUnit.Position.Height=areaUnitPositionHeight;	
					}
					else{
						chartAreaUnit.AlignOrientation = AreaAlignOrientation.Vertical;
						chartAreaUnit.Position.X=5;
						chartAreaUnit.Position.Y=2;
						chartAreaUnit.Position.Width=80;
						chartAreaUnit.Position.Height=areaUnitPositionHeight;	
					}
					//chartAreaUnit.Name="chartAreaUnit";								
					this.ChartAreas.Add(chartAreaUnit);
					//Charger les séries de valeurs 
					string unitName="";
					string chartAreaName="";
					string chartAreaAdditionalName="";

					#region sélection par rappot à l'unité choisit
					switch (_webSession.Unit){
						case WebConstantes.CustomerSessions.Unit.euro:  
							unitName= GestionWeb.GetWebWord(2110,_webSession.SiteLanguage);
							break;
						case WebConstantes.CustomerSessions.Unit.kEuro :  
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

					#region Titres des graphiques
					if (_webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
						//Titre graphique cible de base
						chartAreaName+=GestionWeb.GetWebWord(1736,_webSession.SiteLanguage);
						chartAreaName+=" ";
						chartAreaName+= unitName ;
						chartAreaName+=" ("+_dtGraphicsData.Rows[0]["baseTarget"]+") " ;
						chartAreaName+=GestionWeb.GetWebWord(1738,_webSession.SiteLanguage);
						//Titre graphique cible selectionnée
						chartAreaAdditionalName+=GestionWeb.GetWebWord(1736,_webSession.SiteLanguage);
						chartAreaAdditionalName+=" ";
						chartAreaAdditionalName+=unitName ;
						chartAreaAdditionalName+=" ("+_dtGraphicsData.Rows[0]["additionalTarget"]+") " ;
						chartAreaAdditionalName+=GestionWeb.GetWebWord(1738,_webSession.SiteLanguage);
					}
					else{
						chartAreaName+=GestionWeb.GetWebWord(1736,_webSession.SiteLanguage);
						chartAreaName+=" ";
						chartAreaName+= unitName ;
						chartAreaName+=" "+GestionWeb.GetWebWord(1738,_webSession.SiteLanguage);					
					}
					#endregion

					#endregion

                    seriePeriodicity = SetSeriesPeriodicity(_dtGraphicsData, chartAreaUnit, seriePeriodicity, xUnitValues, yUnitValues, _newPieColors, chartAreaName);												
					#endregion												
 
					#region legend chart Area
					ChartArea chartAreaLegend=new ChartArea();
					chartAreaLegend.Name="legendArea";
					//Alignement
					if (_webSession.Unit == WebConstantes.CustomerSessions.Unit.grp){
						chartAreaLegend.AlignOrientation = AreaAlignOrientation.Vertical;
						chartAreaLegend.Position.X=5;
						chartAreaLegend.Position.Y=areaLegendPositionY;
						chartAreaLegend.Position.Width=80;
						chartAreaLegend.Position.Height=areaLegendPositionHeight;	
					}
					else{
						chartAreaLegend.AlignOrientation = AreaAlignOrientation.Vertical;
						chartAreaLegend.Position.X=5;
						chartAreaLegend.Position.Y=areaLegendPositionY;
						chartAreaLegend.Position.Width=80;
						chartAreaLegend.Position.Height=areaLegendPositionHeight;					
					}
					this.ChartAreas.Add(chartAreaLegend);
					this.Legends["Default"].DockToChartArea = chartAreaLegend.Name;
					this.Legends["Default"].InsideChartArea = "legendArea";
					this.Legends["Default"].Enabled =false;
					this.Legends["Default"].LegendStyle = LegendStyle.Table;
					this.Legends["Default"].Docking = LegendDocking.Bottom;
					this.Legends["Default"].Alignment = StringAlignment.Center;
	
					#endregion
			
					#region Création et définition du graphique pour la cible selectionnée
					//Création du graphique	pour unité
					ChartArea chartAreaUnitadditional=null;
					Series seriePeriodicityadditional=new Series();	
					//Conteneur graphique pour unité
					chartAreaUnitadditional=new ChartArea();

					if (_webSession.Unit == WebConstantes.CustomerSessions.Unit.grp){
						GetSeriesDataAdditional(_dtGraphicsData,ref xUnitValues,ref yUnitValues);

						#region legend2
						Legend secondLegend = new Legend("Second");
						this.Legends.Add(secondLegend);
						seriePeriodicityadditional.Legend = "Second";
						secondLegend.DockToChartArea = chartAreaUnitadditional.Name;
						secondLegend.DockInsideChartArea = true;	
						secondLegend.BorderWidth=2;
						secondLegend.Enabled=false;
						#endregion

						//Alignement
						chartAreaUnitadditional.AlignOrientation = AreaAlignOrientation.Vertical;
						chartAreaUnitadditional.Position.X=5;
						chartAreaUnitadditional.Position.Y=areaUnitadditionalPositionY;
						chartAreaUnitadditional.Position.Width=80;
						chartAreaUnitadditional.Position.Height=areaUnitadditionalPositionHeight;
						this.ChartAreas.Add(chartAreaUnitadditional);
						//Charger les séries de valeurs 
                        seriePeriodicityadditional = SetSeriesPeriodicity(_dtGraphicsData, chartAreaUnitadditional, seriePeriodicityadditional, xUnitValues, yUnitValues, _newPieColors, chartAreaAdditionalName);												
					}
					#endregion
					
					#region initialisation du control pour le module Données de cadrage
					if (_webSession.Unit == WebConstantes.CustomerSessions.Unit.grp){
						InitializeComponentGrp(this,chartAreaUnit,chartAreaUnitadditional);

						if(_dtGraphicsData!=null && _dtGraphicsData.Rows.Count>0){
							this.Series.Add(seriePeriodicity);
							this.Series.Add(seriePeriodicityadditional);
						}					
					}
					else{
						InitializeComponent(this,chartAreaUnit);
						if(_dtGraphicsData!=null && _dtGraphicsData.Rows.Count>0){
							this.Series.Add(seriePeriodicity);
						}
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
			}
			catch(System.Exception err){				
				throw(new AtonPdfException("Erreur dans l'affichage du graphique périodicités",err));
			}
		}
		#endregion

		#region méthodes privées
		/// <summary>
		/// Obtient les séries de valeurs des unités pour la cible de base à afficher graphiquement
		/// </summary>
		/// <param name="dt">table de données</param>
		/// <param name="xValues">libellés du graphique</param>
		/// <param name="yValues">valeurs pour graphique</param>
		private static void GetSeriesDataBase(DataTable dt,ref string[] xValues,ref double[] yValues){
	
			#region Variable
			string ventilationType="periodicity";
			int x=0; 
			int y=0;
			#endregion
			
			#region Les séries  d'unité à afficher graphiquement
			xValues = new string[dt.Rows.Count];				
			yValues = new double[dt.Rows.Count];			
			for(int i=1; i<dt.Rows.Count-1;i++){
				xValues[x]=dt.Rows[i][ventilationType].ToString();
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
			string ventilationType="periodicity";
			int x=0;
			int y=0;
			#endregion
			
			#region Les séries  d'unité à afficher graphiquement
			xValues = new string[dt.Rows.Count];				
			yValues = new double[dt.Rows.Count];			
			for(int i=1; i<dt.Rows.Count-1;i++){
				xValues[x]=dt.Rows[i][ventilationType].ToString();
				yValues[y]=double.Parse(dt.Rows[i]["distributionSelected"].ToString());
				x++;
				y++;
			}
			#endregion
		}

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
		/// <returns>séries de valeurs</returns>
		private static  Dundas.Charting.WinControl.Series SetSeriesPeriodicity(DataTable dt ,ChartArea chartArea,Dundas.Charting.WinControl.Series series,string[] xValues,double[] yValues,List<Color> barColors,string chartAreaName){

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
				series.ToolTip = "#VALX";
				#endregion	

				for (int i=0;i<xValues.Length;i++){
					series.Points[i]["Exploded"] = "true";
				}

				series["LabelStyle"]="Outside";
				series.Label="#PERCENT";
				series.LegendText="#VALX";
				#endregion	
			}
			#endregion 

			return series;
        }

        #region SetSeriesBarPeriodicity (Non utilisé)
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
		/// <param name="chartAreaName">Nom du conteneur de l'image</param>
		/// <param name="maxScale">echelle maximum</param>
		/// <returns>séries de valeurs</returns>
		private static  Dundas.Charting.WinControl.Series SetSeriesBarPeriodicity(DataTable dt ,ChartArea chartArea,Dundas.Charting.WinControl.Series series,string[] xValues,double[] yValues,System.Drawing.Color barColor,string chartAreaName,double maxScale){

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
				series.Points.DataBindXY(xValues,yValues);
				chartArea.AxisX.LabelStyle.Font = new Font("Arial", 8);
				chartArea.AxisY.LabelStyle.Font = new Font("Arial", 8);
				
				#region Définition des couleurs
				//couleur du graphique
				series.Color= barColor;
				#endregion
 
				#region Légende
				series["LabelStyle"]="Outside";
				series.LegendToolTip = "#PERCENT";
				series.ToolTip = "#VALX : #VALY";
				series["PieLineColor"]="Black";
				#endregion

				series.Label="#VALY";
				#endregion	
			}
			#endregion 

			return series;
        }
        */
        #endregion


        /// <summary>
		/// Initialise les styles du webcontrol pour média radio et télé
		/// </summary>
		/// <param name="appmChart">Objet Webcontrol</param>
		/// <param name="chartAreaUnit">conteneur de l'image répartition unité</param>
		/// <param name="appmImageType">sortie flash</param>
		private static void InitializeComponent(Chart appmChart, ChartArea chartAreaUnit){

			#region Chart
			appmChart.Width=700;
			appmChart.Height=400;
			appmChart.BackGradientType = GradientType.TopBottom;
            _style.GetTag("PeriodicityGraphLineEnCircle").SetStyleDundas(appmChart);			
			appmChart.Legend.Enabled=true;
			#endregion	

			#region Titre
			//titre unité de base			
			appmChart.Titles.Add(chartAreaUnit.Name);
			appmChart.Titles[0].DockInsideChartArea=true;			
			appmChart.Titles[0].Font=new Font("Arial", (float)10);
			appmChart.Titles[0].Color=Color.FromArgb(100,72,131);
			appmChart.Titles[0].DockToChartArea=chartAreaUnit.Name;	
			#endregion
		}

		/// <summary>
		/// Initialise les styles du webcontrol pour média radio et télé
		/// </summary>
		/// <param name="appmChart">Objet Webcontrol</param>
		/// <param name="chartAreaUnit">conteneur de l'image répartition unité</param>
		/// <param name="chartAreaUnitadditional">conteneur de l'image répartition pour cible selectionnée</param>
		private static void InitializeComponentGrp(Chart appmChart, ChartArea chartAreaUnit,ChartArea chartAreaUnitadditional){			
			
			#region Initialisation
			float positionY = 0;
			positionY=59;
			#endregion

			#region Chart
			appmChart.Width=700;
			appmChart.Height=700;
			appmChart.BackGradientType = GradientType.TopBottom;
            _style.GetTag("PeriodicityGraphLineEnCircle").SetStyleDundas(appmChart);
			appmChart.Legend.Enabled=true;
			#endregion

			#region Titre
			//titre unité de base			
			appmChart.Titles.Add(chartAreaUnit.Name);
			appmChart.Titles[0].DockInsideChartArea=true;			
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
		#endregion

	}
}
