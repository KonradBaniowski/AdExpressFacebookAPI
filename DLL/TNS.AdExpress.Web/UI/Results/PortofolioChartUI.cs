#region Informations
// Auteur: D. V. MUSSUMA
// Date de création: 20/12/2004
// Date de modification: 
//	21/12/2004	D. V. MUSSUMA 
//	12/08/2005	G. Facon	Nom de variables
#endregion

using System;
using Dundas.Charting.WebControl;
using System.Web.UI.WebControls;
using System.Drawing;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Constantes.FrameWork.Results;
using System.Data;
using TNS.AdExpress.Web.Core.Sessions;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Translation;
using System.Web.UI;
using System.IO;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.UI.Results{
	/// <summary>
	/// Graphiques Structure de l'alerte d'un portefeuille
	/// </summary>
	public class PortofolioChartUI:Chart{
		
        #region Variables
        /// <summary>
        /// Pie colors list
        /// </summary>
        private string _pieColors = string.Empty;
        /// <summary>
        /// Pie line color
        /// </summary>
        private string _pieLineColor = string.Empty;
        /// <summary>
        /// Default border line color
        /// </summary>
        private string _defaultBorderLineColor = string.Empty;
        /// <summary>
        /// Chart border line color
        /// </summary>
        private string _chartBorderLineColor = string.Empty;
        /// <summary>
        /// Title color
        /// </summary>
        private string _titleColor = string.Empty;
        #endregion

        #region Accessors
        /// <summary>
        /// Get or Set Pie colors list
        /// </summary>
        public string PieColors {
            get { return _pieColors; }
            set { _pieColors = value; }
        }
        /// <summary>
        /// Get or Set Pie line color
        /// </summary>
        public string PieLineColor {
            get { return _pieLineColor; }
            set { _pieLineColor = value; }
        }
        /// <summary>
        /// Get or Set Default border line color
        /// </summary>
        public string DefaultBorderLineColor {
            get { return _defaultBorderLineColor; }
            set { _defaultBorderLineColor = value; }
        }
        /// <summary>
        /// Get or Set Chart border line color
        /// </summary>
        public string ChartBorderLineColor {
            get { return _chartBorderLineColor; }
            set { _chartBorderLineColor = value; }
        }
        /// <summary>
        /// Get or Set Title color
        /// </summary>
        public string TitleColor {
            get { return _titleColor; }
            set { _titleColor = value; }
        }
        #endregion

        /// <summary>
		/// Graphiques Structure de alerte d'un portefeuille
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="typeFlash">sortie flash</param>
		public void StructureChart(WebSession webSession,bool typeFlash){
			
			#region variables
			object[,] tab=null;			
			DataTable dtFormat;
			DataTable dtColor;
			DataTable dtInsert;
			DataTable dtLocation;
			//Séries de valeurs pour chaque tranche du graphique
			double[]  yFormatValues = null;
			string[]  xFormatValues = null;
			double[]  yColorValues = null;
			string[]  xColorValues = null;
			double[]  yInsertValues = null;
			string[]  xInsertValues = null;
			double[]  yLocationValues = null;
			string[]  xLocationValues = null;

			double[]  yEurosValues = null;
			string[]  xEurosValues = null;
			double[]  ySpotValues = null;
			string[]  xSpotValues = null;
			double[]  yDurationValues = null;
			string[]  xDurationValues = null;
			#endregion

            #region couleurs des tranches du graphique
            string[] pieColorsList = _pieColors.Split(',');
            Color[] pieColors = new Color[12];
            int indexPieColors = 0;
            ColorConverter ColorConverter = new ColorConverter();
            
            foreach (string pieColorsString in pieColorsList) {
                pieColors.SetValue((Color)ColorConverter.ConvertFrom(pieColorsString), indexPieColors++);
            }

            //Color[] pieColors={
            //    Color.FromArgb(100,72,131),
            //    Color.FromArgb(177,163,193),
            //    Color.FromArgb(208,200,218),
            //    Color.FromArgb(225,224,218),
            //    Color.FromArgb(255,215,215),
            //    Color.FromArgb(255,240,240),
            //    Color.FromArgb(202,255,202),
            //    Color.FromArgb(255,5,182),
            //    Color.FromArgb(157,152,133),
            //    Color.FromArgb(241,241,241),
            //    Color.FromArgb(77,150,75),
            //    Color.FromArgb(0,0,0)
            //};
			#endregion

			#region Paramétrage des dates
			int dateBegin = int.Parse(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
			int dateEnd = int.Parse(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
			#endregion	


			//id Média
			string idVehicle=webSession.GetSelection(webSession.SelectionUniversMedia,TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
			switch((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())){
				case DBClassificationConstantes.Vehicles.names.press :
				case DBClassificationConstantes.Vehicles.names.internationalPress :
					//données pour presse
					PortofolioRules.GetStructPressTab(webSession,dateBegin,dateEnd,out dtFormat,out dtColor,out dtLocation,out dtInsert);
					if(dtFormat==null && dtColor==null && dtInsert==null && dtLocation==null){
						//Pas de données à afficher
						this.Visible=false;
					}else{
						//Récupère les séries de valeurs pour le graphique
						//format
						if(dtFormat!=null && dtFormat.Rows.Count>0)
							GetPressSeriesData(dtFormat,ref xFormatValues,ref yFormatValues,PortofolioStructure.Ventilation.format);
						//couleur
						if(dtColor!=null && dtColor.Rows.Count>0)
							GetPressSeriesData(dtColor,ref xColorValues,ref yColorValues,PortofolioStructure.Ventilation.color);
						//Emplacements
						if(dtLocation!=null && dtLocation.Rows.Count>0)
							GetPressSeriesData(dtLocation,ref xLocationValues,ref yLocationValues,PortofolioStructure.Ventilation.location);
						//Encarts
						if(dtInsert!=null && dtInsert.Rows.Count>0)
							GetPressSeriesData(dtInsert,ref xInsertValues,ref yInsertValues,PortofolioStructure.Ventilation.insert);
																	
							#region Création et définition du graphique
							//Création du graphique	
						ChartArea chartAreaFormat=null;
						Series serieFormat=new Series();	
						if(dtFormat!=null && dtFormat.Rows.Count>0){																	
							//Conteneur graphique pour format
							 chartAreaFormat=new ChartArea();
							//Alignement
							chartAreaFormat.AlignOrientation = AreaAlignOrientation.Vertical;
							chartAreaFormat.Position.X=7;							
							chartAreaFormat.Position.Width=80;

							if(dtInsert!=null && dtInsert.Rows.Count>0){
								chartAreaFormat.Position.Y = (typeFlash) ? 3 : 8;
								chartAreaFormat.Position.Height=22;	
							}else{
								chartAreaFormat.Position.Y = (typeFlash) ? 3 : 8;
								chartAreaFormat.Position.Height=31;	
							}							

							this.ChartAreas.Add(chartAreaFormat);
							//Charger les séries de valeurs pour format
							serieFormat=SetSeriesForPress(dtFormat,chartAreaFormat,serieFormat,xFormatValues,yFormatValues,pieColors,GestionWeb.GetWebWord(1420,webSession.SiteLanguage),typeFlash,_pieLineColor);												
						}
							//Conteneur graphique pour couleur
						ChartArea chartAreaColor=null;
						Series serieColor=new Series();	
						if(dtColor!=null && dtColor.Rows.Count>0){
							 chartAreaColor=new ChartArea();
							//Alignement
							chartAreaColor.AlignOrientation = AreaAlignOrientation.Vertical;						
							chartAreaColor.Position.X=7;
							chartAreaColor.Position.Width=80;
							if(dtInsert!=null && dtInsert.Rows.Count>0){
								chartAreaColor.Position.Y = (typeFlash) ? 27 : 32;								
								chartAreaColor.Position.Height=23;	
							}else{
								chartAreaColor.Position.Y = (typeFlash) ? 36 : 41;
								chartAreaColor.Position.Height=31;	
							}					
							this.ChartAreas.Add(chartAreaColor);
							//Charger les séries de valeurs pour couleur
							serieColor=SetSeriesForPress(dtColor,chartAreaColor,serieColor,xColorValues,yColorValues,pieColors,GestionWeb.GetWebWord(1438,webSession.SiteLanguage),typeFlash,_pieLineColor);																					
						}
							//Conteneur graphique pour emplacements
						ChartArea chartAreaLocation=null;
						Series serieLocation=new Series();
						if(dtLocation!=null && dtLocation.Rows.Count>0){
							chartAreaLocation=new ChartArea();
							//Alignement
							chartAreaLocation.AlignOrientation = AreaAlignOrientation.Vertical;							
							chartAreaLocation.Position.X=1;
							chartAreaLocation.Position.Width=95;
							if(dtInsert!=null && dtInsert.Rows.Count>0){
								chartAreaLocation.Position.Y = (typeFlash) ? 52 : 57;								
								chartAreaLocation.Position.Height=23;
							}else{
								chartAreaLocation.Position.Y = (typeFlash) ? 69 : 74;
								chartAreaLocation.Position.Height=30;	
							}							
							this.ChartAreas.Add(chartAreaLocation);
							//Charger les séries de valeurs pour emplacements
							serieLocation=SetSeriesForPress(dtLocation,chartAreaLocation,serieLocation,xLocationValues,yLocationValues,pieColors,GestionWeb.GetWebWord(1439,webSession.SiteLanguage),typeFlash,_pieLineColor);						
							
						}

						//Conteneur graphique pour encarts
						ChartArea chartAreaInsert=null;
						Series serieInsert=new Series();
						if(dtInsert!=null && dtInsert.Rows.Count>0){
								 chartAreaInsert=new ChartArea();
								//Alignement
								chartAreaInsert.AlignOrientation = AreaAlignOrientation.Vertical;							
								chartAreaInsert.Position.X = 7;
							chartAreaInsert.Position.Y = (typeFlash) ? 77 : 78;
								chartAreaInsert.Position.Width = 80;								
								chartAreaInsert.Position.Height = 22;							
								this.ChartAreas.Add(chartAreaInsert);
								//Charger les séries de valeurs pour encarts
								serieInsert=SetSeriesForPress(dtInsert,chartAreaInsert,serieInsert,xInsertValues,yInsertValues,pieColors,GestionWeb.GetWebWord(1440,webSession.SiteLanguage),typeFlash,_pieLineColor);																																
						}	
							//initialisation du control
							InitializeComponentForPress(chartAreaFormat,chartAreaColor,chartAreaLocation,chartAreaInsert,typeFlash,webSession,_defaultBorderLineColor,_chartBorderLineColor,_titleColor);
								
							//ajout des séries de valeurs
						if(dtFormat!=null && dtFormat.Rows.Count>0)
							this.Series.Add(serieFormat);
						if(dtColor!=null && dtColor.Rows.Count>0)
							this.Series.Add(serieColor);
						if(dtLocation!=null && dtLocation.Rows.Count>0)
							this.Series.Add(serieLocation);
						if(dtInsert!=null && dtInsert.Rows.Count>0)
							this.Series.Add(serieInsert);
							
							
															
							#endregion												
					}
					break;
					case DBClassificationConstantes.Vehicles.names.radio :
					//données pour radio
						tab = PortofolioRules.GetStructRadioTab(webSession,dateBegin,dateEnd);
						if(tab!=null && tab.GetLength(0)>0){
							GetEuroSeriesData(tab,ref xEurosValues,ref yEurosValues);
							GetSpotSeriesData(tab,ref xSpotValues,ref ySpotValues);
							GetDurationSeriesData(tab,ref xDurationValues,ref yDurationValues);
							#region  Création du graphique 
							if(xEurosValues!=null && yEurosValues!=null && xSpotValues!=null && ySpotValues!=null && xDurationValues!=null && yDurationValues!=null){								
								#region Création et définition du graphique
								//Création du graphique																								
								//Conteneur graphique pour Euros
								ChartArea chartAreaEuros=new ChartArea();
								Series serieEuros=new Series();								
								this.ChartAreas.Add(chartAreaEuros);								
								//Conteneur graphique pour Spot
								ChartArea chartAreaSpot=new ChartArea();
								Series serieSpot=new Series();								
								this.ChartAreas.Add(chartAreaSpot);
								//Conteneur graphique pour durée
								ChartArea chartAreaDuration=new ChartArea();
								Series serieDuration=new Series();								
								this.ChartAreas.Add(chartAreaDuration);
								
								
								//Charger les séries de valeurs pour euros 
								serieEuros=SetSeries(tab,chartAreaEuros,serieEuros,xEurosValues,yEurosValues,pieColors,GestionWeb.GetWebWord(1423,webSession.SiteLanguage),typeFlash,_pieLineColor);							
								
								//Charger les séries de valeurs pour spot
								serieSpot=SetSeries(tab,chartAreaSpot,serieSpot,xSpotValues,ySpotValues,pieColors,GestionWeb.GetWebWord(869,webSession.SiteLanguage),typeFlash,_pieLineColor);							
								
								//Charger les séries de valeurs pour durée
								serieDuration=SetSeries(tab,chartAreaDuration,serieDuration,xDurationValues,yDurationValues,pieColors,GestionWeb.GetWebWord(280,webSession.SiteLanguage),typeFlash,_pieLineColor);															
								//initialisation du control
								InitializeComponent(chartAreaEuros,chartAreaSpot,chartAreaDuration,typeFlash,webSession,_defaultBorderLineColor,_chartBorderLineColor,_titleColor);
								
								//ajout des séries de valeurs
								this.Series.Add(serieEuros);
								this.Series.Add(serieSpot);
								this.Series.Add(serieDuration);
															
								#endregion
							}
							#endregion
						}else{
							this.Visible=false;
						}
					break;
					case DBClassificationConstantes.Vehicles.names.tv :
					case DBClassificationConstantes.Vehicles.names.others:
					//données pour tv
					tab = PortofolioRules.GetStructTVTab(webSession,dateBegin,dateEnd);
						if(tab!=null && tab.GetLength(0)>0){
							GetEuroSeriesData(tab,ref xEurosValues,ref yEurosValues);
							GetSpotSeriesData(tab,ref xSpotValues,ref ySpotValues);
							GetDurationSeriesData(tab,ref xDurationValues,ref yDurationValues);
							#region  Création du graphique 
							if(xEurosValues!=null && yEurosValues!=null && xSpotValues!=null && ySpotValues!=null && xDurationValues!=null && yDurationValues!=null){								
								#region Création et définition du graphique
								//Création du graphique																								
								//Conteneur graphique pour Euros
								ChartArea chartAreaEuros=new ChartArea();
								Series serieEuros=new Series();								
								this.ChartAreas.Add(chartAreaEuros);								
								//Conteneur graphique pour Spot
								ChartArea chartAreaSpot=new ChartArea();
								Series serieSpot=new Series();								
								this.ChartAreas.Add(chartAreaSpot);
								//Conteneur graphique pour durée
								ChartArea chartAreaDuration=new ChartArea();
								Series serieDuration=new Series();								
								this.ChartAreas.Add(chartAreaDuration);
																
								//Charger les séries de valeurs pour euros 
								serieEuros=SetSeries(tab,chartAreaEuros,serieEuros,xEurosValues,yEurosValues,pieColors,GestionWeb.GetWebWord(1423,webSession.SiteLanguage),typeFlash,_pieLineColor);							
								
								//Charger les séries de valeurs pour spot
								serieSpot=SetSeries(tab,chartAreaSpot,serieSpot,xSpotValues,ySpotValues,pieColors,GestionWeb.GetWebWord(869,webSession.SiteLanguage),typeFlash,_pieLineColor);							
								
								//Charger les séries de valeurs pour durée
								serieDuration=SetSeries(tab,chartAreaDuration,serieDuration,xDurationValues,yDurationValues,pieColors,GestionWeb.GetWebWord(280,webSession.SiteLanguage),typeFlash,_pieLineColor);							

								//initialisation du control
								InitializeComponent(chartAreaEuros,chartAreaSpot,chartAreaDuration,typeFlash,webSession,_defaultBorderLineColor,_chartBorderLineColor,_titleColor);
								
								//ajout des séries de valeurs
								this.Series.Add(serieEuros);
								this.Series.Add(serieSpot);
								this.Series.Add(serieDuration);
															
								#endregion
							}
							#endregion							
						}else{
							this.Visible=false;
						}
					break;
				default :				
					throw new Exceptions.PortofolioUIException("Le cas de ce média n'est pas gérer.");
			}

			
		}

		#region méthodes privées
		/// <summary>
		/// Obtient les séries de valeurs à afficher graphiquement
		/// </summary>
		/// <param name="dt">table de données</param>
		/// <param name="xValues">libellés du graphique</param>
		/// <param name="yValues">valeurs pour graphique</param>
		/// <param name="ventilation">format , couleur,emplacements,encarts</param>
		private static void GetPressSeriesData(DataTable dt,ref string[] xValues,ref double[] yValues,PortofolioStructure.Ventilation ventilation){
			
			xValues = new string[dt.Rows.Count];
			int x=0;
			yValues = new double[dt.Rows.Count];
			int y=0;

			string ventilationType="";
			switch(ventilation){
				case PortofolioStructure.Ventilation.color :
					ventilationType="color";
					break;
				case PortofolioStructure.Ventilation.format :
					ventilationType="format";
					break;
				case PortofolioStructure.Ventilation.insert :
					ventilationType="inset";
					break;
				case PortofolioStructure.Ventilation.location :
					ventilationType="location";
					break;
				default :				
					throw new Exceptions.PortofolioSystemException("GetVentilationLines(WebSession webSession,DataTable dt,int labelcode,string classCss,PortofolioStructure.Ventilation ventilation)-->Le type de ventilation ne peut pas être déterminé.");
			}
			//crétions de la séries de valeurs pour la construction du graphique
			foreach(DataRow dr in dt.Rows){
				if(dr[ventilationType]!=null && dr["insertion"]!=null){
					xValues[x]=dr[ventilationType].ToString();
					yValues[y]=double.Parse(dr["insertion"].ToString());
					x++;
					y++;
				}
			}
		}
		/// <summary>
		/// Série d'euros
		/// </summary>
		/// <param name="tab">tableau de données</param>
		/// <param name="xEurosValues">liste libellés euros</param>
		/// <param name="yEurosValues">liste valeurs euros</param>
		private static void GetEuroSeriesData(object[,] tab,ref string[] xEurosValues,ref double[] yEurosValues){
			xEurosValues = new string[tab.GetLength(0)-1];
			yEurosValues = new double[tab.GetLength(0)-1];
			for(int i=1;i<tab.GetLength(0);i++){
				if(tab[i,PortofolioStructure.EUROS_COLUMN_INDEX]!=null && WebFunctions.CheckedText.IsStringEmpty(tab[i,PortofolioStructure.EUROS_COLUMN_INDEX].ToString().Trim())){
					if(tab[i,PortofolioStructure.MEDIA_HOURS_COLUMN_INDEX]!=null )xEurosValues[i-1]=tab[i,PortofolioStructure.MEDIA_HOURS_COLUMN_INDEX].ToString();
					yEurosValues[i-1]=double.Parse(tab[i,PortofolioStructure.EUROS_COLUMN_INDEX].ToString());
				}				
			}
		}
		/// <summary>
		/// Série spot
		/// </summary>
		/// <param name="tab">tableau de données</param>
		/// <param name="xSpotValues">liste libellés spot</param>
		/// <param name="ySpotValues">liste valeurs spot</param>
		private static void GetSpotSeriesData(object[,] tab,ref string[] xSpotValues,ref double[] ySpotValues){
			xSpotValues = new string[tab.GetLength(0)-1];
			ySpotValues = new double[tab.GetLength(0)-1];
			for(int i=1;i<tab.GetLength(0);i++){
				if(tab[i,PortofolioStructure.SPOT_COLUMN_INDEX]!=null && WebFunctions.CheckedText.IsStringEmpty(tab[i,PortofolioStructure.SPOT_COLUMN_INDEX].ToString().Trim())){
					if(tab[i,PortofolioStructure.MEDIA_HOURS_COLUMN_INDEX]!=null)xSpotValues[i-1]=tab[i,PortofolioStructure.MEDIA_HOURS_COLUMN_INDEX].ToString();
					ySpotValues[i-1]=double.Parse(tab[i,PortofolioStructure.SPOT_COLUMN_INDEX].ToString());
				}
			}
		}
		/// <summary>
		/// Série durées
		/// </summary>
		/// <param name="tab">tableau de données</param>
		/// <param name="xDurationValues">liste libellés durées</param>
		/// <param name="yDurationValues">liste valeurs durées</param>
		private static void GetDurationSeriesData(object[,] tab,ref string[] xDurationValues,ref double[] yDurationValues){
			xDurationValues = new string[tab.GetLength(0)-1];
			yDurationValues = new double[tab.GetLength(0)-1];
			for(int i=1;i<tab.GetLength(0);i++){
				if(tab[i,PortofolioStructure.DURATION_COLUMN_INDEX]!=null && WebFunctions.CheckedText.IsStringEmpty(tab[i,PortofolioStructure.DURATION_COLUMN_INDEX].ToString().Trim())){
					if(tab[i,PortofolioStructure.MEDIA_HOURS_COLUMN_INDEX]!=null)xDurationValues[i-1]=tab[i,PortofolioStructure.MEDIA_HOURS_COLUMN_INDEX].ToString();
					yDurationValues[i-1]=double.Parse(tab[i,PortofolioStructure.DURATION_COLUMN_INDEX].ToString());
				}
			}
		}
		/// <summary>
		/// Crétion du graphique euros,spot ou  durée pour média radio et télé
		/// </summary>
		/// <param name="tab">tableau de résultats</param>
		/// <param name="chartArea">contenant objet graphique</param>
		/// <param name="series">séries de valeurs</param>
		/// <param name="xValues">séries de libellés</param>
		/// <param name="yValues">séries de valeurs</param>
		/// <param name="pieColors">couleurs du graphique</param>
		/// <param name="typeFlash">sortie flash</param>
		/// <param name="chartAreaName">nom du conteneur de l'image</param>
        /// <param name="pieLineColor">Pie line color</param>
		/// <returns>séries de valeurs</returns>
		private static  Dundas.Charting.WebControl.Series SetSeries(object[,] tab ,ChartArea chartArea,Dundas.Charting.WebControl.Series series,string[] xValues,double[] yValues,Color[] pieColors,string chartAreaName,bool typeFlash, string pieLineColor){
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
				for(int k=0;k<tab.GetLength(0)-1;k++){
					series.Points[k].Color=pieColors[k];
				}
				#endregion

				#region Légende
				series["LabelStyle"]="Outside";
				series.LegendToolTip = "#PERCENT";
				series.ToolTip = "#PERCENT : #VALX ";
                //series["PieLineColor"]="Black";
                series["PieLineColor"] = pieLineColor;
				#endregion				
				series.Label="#PERCENT : #VALX";
				series["3DLabelLineSize"]="30";
				
																	
				#endregion				

			}
			#endregion

			return series;
		}

		/// <summary>
		/// Création du graphique euros,spot ou  durée pour média presse 
		/// </summary>
		/// <param name="dt">tableau de résultats</param>
		/// <param name="chartArea">contenant objet graphique</param>
		/// <param name="series">séries de valeurs</param>
		/// <param name="xValues">séries de libellés</param>
		/// <param name="yValues">séries de valeurs</param>
		/// <param name="pieColors">couleurs du graphique</param>
		/// <param name="typeFlash">sortie flash</param>
		/// <param name="chartAreaName">Nom du conteneur de l'image</param>
        /// <param name="pieLineColor">Pie line color</param>
		/// <returns>séries de valeurs</returns>
		private static  Dundas.Charting.WebControl.Series SetSeriesForPress(DataTable dt ,ChartArea chartArea,Dundas.Charting.WebControl.Series series,string[] xValues,double[] yValues,Color[] pieColors,string chartAreaName,bool typeFlash, string pieLineColor){
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
				for(int k=0;k<dt.Rows.Count && k<11;k++){
					series.Points[k].Color=pieColors[k];
				}
				#endregion

				#region Légende
				series["LabelStyle"]="Outside";
				series.LegendToolTip = "#PERCENT";
				series.ToolTip = "#PERCENT : #VALX";
                //series["PieLineColor"] = "Black";
                series["PieLineColor"] = pieLineColor;
				//series.LabelFormat=
				#endregion			

			
					series.Label="#PERCENT : #VALX";
				
																	
				#endregion				

			}
			#endregion

			return series;
		}
		/// <summary>
		/// Initialise les styles du webcontrol pour média radio et télé
		/// </summary>
		/// <param name="chartAreaEuros">conteneur de l'image répartition euros</param>
		/// <param name="chartAreaSpot">conteneur de l'image répartition spot</param>
		/// <param name="chartAreaDuration">conteneur de l'image répartition durée</param>
		/// <param name="typeFlash">sortie flash</param>
		/// <param name="webSession">Session client</param>
        /// <param name="defaultBorderLineColor">Default chart border color</param>
        /// <param name="borderLineColor">Chart border color</param>
        /// <param name="title">Title color</param>
        private void InitializeComponent(ChartArea chartAreaEuros, ChartArea chartAreaSpot, ChartArea chartAreaDuration, bool typeFlash, WebSession webSession, string defaultBorderLineColor, string borderLineColor, string titleColor) {
			
            #region Animation Flash
			//Animation flash
			if(typeFlash){
				this.ImageType=ChartImageType.Flash;
				this.AnimationTheme = AnimationTheme.MovingFromTop;
				this.AnimationDuration = 3;
//				this.AnimationTheme = AnimationTheme.GrowingTogether;
//				this.AnimationDuration =0.3;
				this.RepeatAnimation = false;
			}
			else{
				this.ImageType=ChartImageType.Jpeg;
				this.BackImage = WebConstantes.Images.LOGO_TNS_2;
				this.BackImageAlign = ChartImageAlign.TopLeft;
				this.BackImageMode = ChartImageWrapMode.Unscaled;
			}
			#endregion

            #region Color Converter
            ColorConverter ColorConverter = new ColorConverter();
            #endregion

            #region Chart
            this.Width=new Unit("700px");
			this.Height=new Unit("800px");
			this.BackGradientType = GradientType.TopBottom;
            //this.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);											
            this.BorderLineColor = (Color)ColorConverter.ConvertFrom(defaultBorderLineColor);											
			this.BorderStyle=ChartDashStyle.Solid;
            //this.BorderLineColor=Color.FromArgb(99,73,132);
            this.BorderLineColor = (Color)ColorConverter.ConvertFrom(borderLineColor);											
			this.BorderLineWidth=2;
			this.Legend.Enabled=false;
			#endregion	

			#region Titre
			//titre euros
			this.Titles.Add(chartAreaEuros.Name);
			this.Titles[0].DockInsideChartArea=true;
			this.Titles[0].Position.Auto = false;
			this.Titles[0].Position.X = 50;
			this.Titles[0].Position.Y = (typeFlash) ? 2 : 7;
			this.Titles[0].Font=new Font("Arial", (float)13);
            //this.Titles[0].Color=Color.FromArgb(100,72,131);
            this.Titles[0].Color = (Color)ColorConverter.ConvertFrom(titleColor);
			this.Titles[0].DockToChartArea=chartAreaEuros.Name;
			if (!typeFlash) {
				chartAreaEuros.Position.X = 22;
				chartAreaEuros.Position.Width = 60;
				chartAreaEuros.Position.Y = 8;
				chartAreaEuros.Position.Height = 22;
			}
			//titre spot
			this.Titles.Add(chartAreaSpot.Name);
			this.Titles[1].DockInsideChartArea=true;
			this.Titles[1].Position.Auto = false;
			this.Titles[1].Position.X = 50;
			this.Titles[1].Position.Y = (typeFlash) ? 34 : 38;
			this.Titles[1].Font=new Font("Arial", (float)13);
            //this.Titles[1].Color=Color.FromArgb(100,72,131);
            this.Titles[1].Color = (Color)ColorConverter.ConvertFrom(titleColor);
			this.Titles[1].DockToChartArea=chartAreaSpot.Name;
			if (!typeFlash) {
				chartAreaSpot.Position.X = 22;
				chartAreaSpot.Position.Width = 60;
				chartAreaSpot.Position.Y = 40;
				chartAreaSpot.Position.Height = 22;
			}
			//titre durée
			this.Titles.Add(chartAreaDuration.Name);
			this.Titles[2].DockInsideChartArea=true;
			this.Titles[2].Position.Auto = false;
			this.Titles[2].Position.X = 50;
			this.Titles[2].Position.Y = 66;
			this.Titles[2].Font=new Font("Arial", (float)13);
            //this.Titles[2].Color=Color.FromArgb(100,72,131);
            this.Titles[2].Color = (Color)ColorConverter.ConvertFrom(titleColor);
			this.Titles[2].DockToChartArea=chartAreaDuration.Name;
			if (!typeFlash) {
				chartAreaDuration.Position.X = 22;
				chartAreaDuration.Position.Width = 60;
				chartAreaDuration.Position.Y = 68;
				chartAreaDuration.Position.Height = 22;
			}
			#endregion

			if (!typeFlash) {
				//CopyRight
				Title title = new Title("" + GestionWeb.GetWebWord(2266, webSession.SiteLanguage) + "");
				title.Font = new Font("Arial", (float)8);
				title.DockInsideChartArea = false;
				title.Docking = Docking.Bottom;
				this.Titles.Add(title);
			}
			
		}
		/// <summary>
		/// Initialise les styles du webcontrol pour média presse
		/// </summary>
		/// <param name="chartAreaFormat">conteneur de l'image répartition format</param>
		/// <param name="chartAreaColor">conteneur de l'image répartition couleur</param>
		/// <param name="chartAreaLocation">conteneur de l'image répartition emplacement</param>
		/// <param name="chartAreaInsert">conteneur de l'image répartition encart</param>
		/// <param name="typeFlash"></param>
		/// <param name="webSession">Session client</param>
        /// <param name="defaultBorderLineColor">Default chart border color</param>
        /// <param name="borderLineColor">Chart border color</param>
        /// <param name="titleColor">Title color</param>
        private void InitializeComponentForPress(ChartArea chartAreaFormat, ChartArea chartAreaColor, ChartArea chartAreaLocation, ChartArea chartAreaInsert, bool typeFlash, WebSession webSession, string defaultBorderLineColor, string borderLineColor, string titleColor) {			
			
            #region Animation Flash
			//Animation flash
			if(typeFlash){
				this.ImageType=ChartImageType.Flash;
				this.AnimationTheme = AnimationTheme.MovingFromTop;
//				this.AnimationTheme = AnimationTheme.GrowingTogether;
//				this.AnimationDuration =0.3;
				this.AnimationDuration = 3;
				this.RepeatAnimation = false;					
			}
			else{
				this.ImageType=ChartImageType.Jpeg;
				this.BackImage = WebConstantes.Images.LOGO_TNS_2;
				this.BackImageAlign = ChartImageAlign.TopLeft;
				this.BackImageMode = ChartImageWrapMode.Unscaled;				
			}
			#endregion

            #region Color Converter
            ColorConverter ColorConverter = new ColorConverter();
            #endregion

			#region Chart
			if(chartAreaInsert!=null){
				this.Width=new Unit("800px");
				this.Height=new Unit("1000px");
			}else{
				this.Width=new Unit("750px");
				this.Height=new Unit("900px");
			}
			this.BackGradientType = GradientType.TopBottom;
            //this.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);
            this.BorderLineColor = (Color)ColorConverter.ConvertFrom(defaultBorderLineColor);
			this.BorderStyle=ChartDashStyle.Solid;
            //this.BorderLineColor=Color.FromArgb(99,73,132);
            this.BorderLineColor = (Color)ColorConverter.ConvertFrom(borderLineColor);
			this.BorderLineWidth=2;
			this.Legend.Enabled=false;			
			#endregion	

			#region Titre
			//titre format
			if(chartAreaFormat!=null){
				this.Titles.Add(chartAreaFormat.Name);
				this.Titles[0].DockInsideChartArea=true;
				if(chartAreaInsert!=null){
					this.Titles[0].Position.Auto = false;
					this.Titles[0].Position.X = 46;
					this.Titles[0].Position.Y = (typeFlash) ? 2 : 7;
				}else{
					this.Titles[0].Position.Auto = false;
					this.Titles[0].Position.X =46;
					this.Titles[0].Position.Y = (typeFlash) ? 2 : 7;
				}
				
				this.Titles[0].Font=new Font("Arial", (float)13);
                //this.Titles[0].Color=Color.FromArgb(100,72,131);
                this.Titles[0].Color = (Color)ColorConverter.ConvertFrom(titleColor);
				this.Titles[0].DockToChartArea=chartAreaFormat.Name;
			}
			//titre couleur
			if(chartAreaColor!=null){
				this.Titles.Add(chartAreaColor.Name);
				this.Titles[1].DockInsideChartArea=true;
				if(chartAreaInsert!=null){
					this.Titles[1].Position.Auto = false;
					this.Titles[1].Position.X = 46;
					this.Titles[1].Position.Y = (typeFlash) ? 26 : 31; 
				}else{
					this.Titles[1].Position.Auto = false;
					this.Titles[1].Position.X = 46;
					this.Titles[1].Position.Y = (typeFlash) ? 35 : 40; 
				}
				
				this.Titles[1].Font=new Font("Arial", (float)13);
                //this.Titles[1].Color=Color.FromArgb(100,72,131);
                this.Titles[1].Color = (Color)ColorConverter.ConvertFrom(titleColor);
				this.Titles[1].DockToChartArea=chartAreaColor.Name;
			}
			//titre emplacements
			if(chartAreaLocation!=null){
				this.Titles.Add(chartAreaLocation.Name);
				this.Titles[2].DockInsideChartArea=true;
				if(chartAreaInsert!=null){
					this.Titles[2].Position.Auto = false;
					this.Titles[2].Position.X = 46;
					this.Titles[2].Position.Y = (typeFlash) ? 51 : 56;
				}else{
					this.Titles[2].Position.Auto = false;
					this.Titles[2].Position.X = 46;
					this.Titles[2].Position.Y = (typeFlash) ? 68 : 73;
				}
				this.Titles[2].Font=new Font("Arial", (float)13);
                //this.Titles[2].Color=Color.FromArgb(100,72,131);
                this.Titles[2].Color = (Color)ColorConverter.ConvertFrom(titleColor);
				this.Titles[2].DockToChartArea=chartAreaLocation.Name;
			}
			//titre encarts
			if(chartAreaInsert!=null){
				this.Titles.Add(chartAreaInsert.Name);
				this.Titles[3].DockInsideChartArea=true;

				this.Titles[3].Position.Auto = false;
				this.Titles[3].Position.X =46;
				this.Titles[3].Position.Y = (typeFlash) ? 76 : 78;
				

				this.Titles[3].Font=new Font("Arial", (float)13);
                //this.Titles[3].Color=Color.FromArgb(100,72,131);
                this.Titles[3].Color = (Color)ColorConverter.ConvertFrom(titleColor);
				this.Titles[3].DockToChartArea=chartAreaInsert.Name;
			}
			if (!typeFlash) {
				//CopyRight
				Title title = new Title("" + GestionWeb.GetWebWord(2266, webSession.SiteLanguage) + "");
				title.Font = new Font("Arial", (float)8);
				title.DockInsideChartArea = false;
				title.Docking = Docking.Bottom;
				this.Titles.Add(title);
			}

			#endregion
			
		}
		#endregion

		#region Render
		/// <summary>
		/// Overrided to add "param" tags for contextual menu managment
		/// </summary>
		/// <param name="writer">Writer</param>
		protected override void Render(HtmlTextWriter writer) {

			HtmlTextWriter txt = new HtmlTextWriter(new StringWriter());
			base.Render(txt);
			int i = -1;
			if((i = txt.InnerWriter.ToString().IndexOf("<PARAM name=\"movie\""))>-1){
				writer.Write(txt.InnerWriter.ToString().Insert(i, "\r\n<PARAM name=\"wmode\" value=\"transparent\">\r\n<PARAM name=\"menu\" value=\"false\">\r\n"));
			}
			else{
				writer.Write(txt.InnerWriter.ToString());
			}
		}
		#endregion
	}
}
