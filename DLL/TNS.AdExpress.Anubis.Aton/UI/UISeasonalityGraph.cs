#region Information
// Author : Y. R'kaina
// Creation : 09/02/2007
// Modifications :
#endregion

using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;

using TNS.AdExpress.Common;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Rules.Results.APPM;
using TNS.AdExpress.Anubis.Aton.Common;
using TNS.AdExpress.Anubis.Aton.Exceptions;
using TNS.FrameWork.DB.Common;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using CstUI = TNS.AdExpress.Constantes.Web.UI;
using WebFunctions=TNS.AdExpress.Web.Functions;

using Dundas.Charting.WinControl;

namespace TNS.AdExpress.Anubis.Aton.UI{
	/// <summary>
	/// Description r�sum�e de UISeasonalityGraph.
	/// </summary>
	public class UISeasonalityGraph:Chart{

		#region Constantes
		/// <summary>
		/// La position de Chart Area (Horizontal)
		/// </summary>
		const int CHART_AREA_POSITION_X=1;
		/// <summary>
		/// La position de Chart Area Unit (Vertical)
		/// </summary>
		const int UNIT_CHART_AREA_POSITION_Y=14;
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
		const int CHART_AREA_POSITION_HEIGHT=85;
		/// <summary>
		/// La position des libell�s par rapport � l'axe des X
		/// </summary>
		const int SERIES_ANGLE=-90;
		/// <summary>
		/// La taille des chiffres
		/// </summary>
		const int FONT_SIZE=8;
		/// <summary>
		/// La largeur de l'image
		/// </summary>
		const int CHART_WIDTH=600;
		/// <summary>
		/// La taille de l'image
		/// </summary>
		const int CHART_HEIGHT=500;
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
		const int TITLE_FONT_SIZE=10;
		#endregion

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
		#endregion

		#region Variables
		/// <summary>
		/// couleurs des tranches du graphique
		/// </summary>
		Color[] pieColors={
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
		#endregion

		#region Constructeur
		public UISeasonalityGraph(WebSession webSession,IDataSource dataSource, AtonConfig config, DataTable dtGraphicsData):base()
		{
			_webSession = webSession;
			_dataSource = dataSource;
			_config = config;
			_dtGraphicsData = dtGraphicsData;
		}
		#endregion

		#region Impl�mentation des m�thodes

		#region SetDesignModeUnitGraph
		/// <summary>		
		/// D�finit les donn�es au moment du design du contr�le. 	
		/// </summary>
		public void SetDesignModeUnitGraph(){
			
			#region variable
			ChartArea chartArea=new ChartArea();
			//S�ries de valeurs pour chaque tranche du graphique
			double[]  yUnitValues = null;
			string[]  xUnitValues  = null;
			double[]  y1UnitValues = null;
			int chartWidth=0, fontSize=0;
			double rowCount=0, div=8;
			#endregion

			try{
			
				if(_dtGraphicsData!=null && _dtGraphicsData.Rows.Count>0){

					#region Cr�ation et d�finition du graphique pour les unit�s
								
					#region Initialisation Chart Width
					rowCount=_dtGraphicsData.Rows.Count/div;
					rowCount=Math.Ceiling(rowCount);
					if(_dtGraphicsData.Rows.Count<=40)
						chartWidth = CHART_WIDTH + (int)(50 * rowCount);
					else 
						chartWidth = 900;
					#endregion

					#region Initialisation font Size
					if(_dtGraphicsData.Rows.Count<=16)
						fontSize=FONT_SIZE+1;
					else
						fontSize=FONT_SIZE;
					#endregion

					#region Get Series Data
					if (_webSession.Unit == WebConstantes.CustomerSessions.Unit.grp)
						GetSeriesDataDistribution(_webSession,_dtGraphicsData,ref xUnitValues,ref yUnitValues,ref y1UnitValues);
					else
						GetSeriesDataBase(_webSession,_dtGraphicsData,ref xUnitValues,ref yUnitValues,ref y1UnitValues);
					#endregion

					//Cr�ation du graphique	des uit�s(euros, grp, insertion, page) cible de base
					ChartArea chartAreaUnit=null;
					Series serieSeasonality=new Series();	
					
					#region Cr�ation de chart area & Alignement
					//Conteneur graphique pour unit� de cible de base
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

					#region s�lection par rappot � l'unit� choisit
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
					#endregion
				
					if (_webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
						chartAreaName+=GestionWeb.GetWebWord(1736,_webSession.SiteLanguage);
						chartAreaName+=" ";
						chartAreaName+=unitName ;
						chartAreaName+=" ("+_dtGraphicsData.Rows[0]["additionalTarget"]+") " ;
						chartAreaName+=GestionWeb.GetWebWord(2112,_webSession.SiteLanguage);
					}
					else{
						chartAreaName+=GestionWeb.GetWebWord(1736,_webSession.SiteLanguage);
						chartAreaName+=" ";
						chartAreaName+= unitName ;
						chartAreaName+=" "+GestionWeb.GetWebWord(2112,_webSession.SiteLanguage);					
					} 
					#endregion

					serieSeasonality=SetSeriesSeasonality(_dtGraphicsData,chartAreaUnit,serieSeasonality,xUnitValues,yUnitValues,pieColors,chartAreaName,GestionWeb.GetWebWord(1795,_webSession.SiteLanguage),fontSize);												
					#endregion		

					#region initialisation du control
					InitializeComponent(this,chartAreaUnit,null,chartWidth,fontSize);
					if(_dtGraphicsData!=null && _dtGraphicsData.Rows.Count>0){
						this.Series.Add(serieSeasonality);
					}
					#endregion

				}
				else{
					this.Titles.Add(GestionWeb.GetWebWord(2106,_webSession.SiteLanguage));
					this.Titles[0].Font=new Font("Arial", (float)8,System.Drawing.FontStyle.Bold);
					this.Titles[0].Color=Color.FromArgb(100,72,131);
					this.Width=250;
					this.Height=20;
				}
				
			}
			catch(System.Exception err){
				throw(new AtonPdfException("Erreur dans l'affichage des graphiques des donn�es de saisonalit� dans le module donn�es de cadrage",err));
			}
		}
		#endregion

		#region SetDesignModeDistributionGraph
		/// <summary>		
		/// D�finit les donn�es au moment du design du contr�le. 	
		/// </summary>
		public void SetDesignModeDistributionGraph(){
			
			#region variable
			//S�ries de valeurs pour chaque tranche du graphique
			double[]  yUnitValues = null;
			string[]  xUnitValues  = null;
			double[]  y1UnitValues = null;
			int chartWidth=0, fontSize=0;
			double rowCount=0, div=8;
			#endregion

			try{
				if(_dtGraphicsData!=null && _dtGraphicsData.Rows.Count>0){

					#region Initialisation Chart Width
					rowCount=_dtGraphicsData.Rows.Count/div;
					rowCount=Math.Ceiling(rowCount);
					if(_dtGraphicsData.Rows.Count<=40)
						chartWidth = CHART_WIDTH + (int)(50 * rowCount);
					else 
						chartWidth = 900;
					#endregion

					#region Initialisation font Size
					if(_dtGraphicsData.Rows.Count<=16)
						fontSize=FONT_SIZE+1;
					else
						fontSize=FONT_SIZE;
					#endregion

					#region Get Series Data
					if (_webSession.Unit == WebConstantes.CustomerSessions.Unit.grp)
						GetSeriesDataDistribution(_webSession,_dtGraphicsData,ref xUnitValues,ref yUnitValues,ref y1UnitValues);
					else
						GetSeriesDataBase(_webSession,_dtGraphicsData,ref xUnitValues,ref yUnitValues,ref y1UnitValues);
					#endregion

					#region Titres des graphiques
					string chartAreaDistributionName="";
					chartAreaDistributionName=GestionWeb.GetWebWord(2105,_webSession.SiteLanguage);
					#endregion

					#region Cr�ation et d�finition du graphique pour le poids de chaque p�riode
					//Cr�ation du graphique	pour unit�
					ChartArea chartAreaDistribution=null;
					Series serieSeasonalityDistribution=new Series();	
					//Conteneur graphique pour unit�
					chartAreaDistribution=new ChartArea();
				
					//Alignement
					chartAreaDistribution.AlignOrientation = AreaAlignOrientation.Vertical;
					chartAreaDistribution.Position.X=CHART_AREA_POSITION_X;
					chartAreaDistribution.Position.Y=DISTRIBUTION_CHART_AREA_POSITION_Y;
					chartAreaDistribution.Position.Width=CHART_AREA_POSITION_WIDTH;
					chartAreaDistribution.Position.Height=CHART_AREA_POSITION_HEIGHT;
					this.ChartAreas.Add(chartAreaDistribution);
					//Charger les s�ries de valeurs 
					serieSeasonalityDistribution=SetSeriesSeasonalityDistribution(_dtGraphicsData,chartAreaDistribution,serieSeasonalityDistribution,xUnitValues,y1UnitValues,pieColors,chartAreaDistributionName,GestionWeb.GetWebWord(1743,_webSession.SiteLanguage),fontSize);
					#endregion

					#region initialisation du control pour le module Donn�es de cadrage
					InitializeComponentDistribution(this,chartAreaDistribution,chartWidth,fontSize);
					if(_dtGraphicsData!=null && _dtGraphicsData.Rows.Count>0){
						this.Series.Add(serieSeasonalityDistribution);
					}
					#endregion
				}
				else{
					this.Titles.Add(GestionWeb.GetWebWord(2106,_webSession.SiteLanguage));
					this.Titles[0].Font=new Font("Arial", (float)8,System.Drawing.FontStyle.Bold);
					this.Titles[0].Color=Color.FromArgb(100,72,131);
					this.Width=250;
					this.Height=20;
				}
			}
			catch(System.Exception err){
				throw(new AtonPdfException("Erreur dans l'affichage des graphiques des donn�es de saisonalit� dans le module donn�es de cadrage",err));
			}
		}
		#endregion

		#endregion

		#region m�thodes priv�es

		#region GetSeriesDataBase
		/// <summary>
		/// Obtient les s�ries de valeurs des unit�s pour la cible de base � afficher graphiquement
		/// </summary>
		/// <param name="webSession">WebSession</param>
		/// <param name="dt">table de donn�es</param>
		/// <param name="xValues">libell�s du graphique</param>
		/// <param name="yValues">valeurs pour graphique</param>
		/// <param name="y1Values">valeurs pour graphique</param>
		private static void GetSeriesDataBase(WebSession webSession,DataTable dt,ref string[] xValues,ref double[] yValues,ref double[] y1Values){
	
			#region Variable
			string ventilationType="seasonality", period="";
			#endregion
			
			#region Les s�ries  d'unit� � afficher graphiquement
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
				yValues[i]=double.Parse(dt.Rows[i]["unitBase"].ToString());
				y1Values[i]=double.Parse(dt.Rows[i]["distributionBase"].ToString());
			}
			#endregion
		}
		#endregion
		
		#region GetSeriesDataDistribution
		/// <summary>
		/// Obtient les s�ries de valeurs des unit�s pour la cible s�lectionn�e � afficher graphiquement
		/// </summary>
		/// <param name="webSession">WebSession</param>
		/// <param name="dt">table de donn�es</param>
		/// <param name="xValues">libell�s du graphique</param>
		/// <param name="yValues">valeurs pour graphique</param>
		/// <param name="y1Values">valeurs pour graphique</param>
		private static void GetSeriesDataDistribution(WebSession webSession,DataTable dt,ref string[] xValues,ref double[] yValues,ref double[] y1Values){
	
			#region Variable
			string ventilationType="seasonality", period="";
			#endregion
			
			#region Les s�ries  d'unit� � afficher graphiquement
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
				yValues[i]=double.Parse(dt.Rows[i]["unitSelected"].ToString());
				y1Values[i]=double.Parse(dt.Rows[i]["distributionSelected"].ToString());
			}
			#endregion
		}
		#endregion

		#region SetSeriesSeasonality
		/// <summary>
		/// Cr�tion du graphique unit�(grp,euro,isertion,page) 
		/// </summary>
		/// <param name="dt">tableau de r�sultats</param>
		/// <param name="chartArea">contenant objet graphique</param>
		/// <param name="series">s�ries de valeurs</param>
		/// <param name="xValues">s�ries de libell�s</param>
		/// <param name="yValues">s�ries de valeurs</param>
		/// <param name="barColors">couleurs du graphique</param>		
		/// <param name="chartAreaName">Nom du conteneur de l'image</param>
		/// <param name="legendText">Legende Texte</param>
		/// <param name="fontSize">Font size</param>
		/// <returns>s�ries de valeurs</returns>
		private static  Dundas.Charting.WinControl.Series SetSeriesSeasonality(DataTable dt ,ChartArea chartArea,Dundas.Charting.WinControl.Series series,string[] xValues,double[] yValues,Color[] barColors,string chartAreaName,string legendText,int fontSize){
			
			#region  Cr�ation graphique
			if(xValues!=null && yValues!=null){
								
				#region Cr�ation et d�finition du graphique
				//Type de graphique
				series.Type = SeriesChartType.Column;
				series.ShowLabelAsValue=true;
				series.XValueType=Dundas.Charting.WinControl.ChartValueTypes.String;
				series.YValueType=Dundas.Charting.WinControl.ChartValueTypes.Double;
				series.Color= Color.FromArgb(148,121,181);
				series.Enabled=true;
				series.Font=new Font("Arial", (float)fontSize);
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
		/// Cr�tion du graphique unit�(grp,euro,isertion,page) 
		/// </summary>
		/// <param name="dt">tableau de r�sultats</param>
		/// <param name="chartArea">contenant objet graphique</param>
		/// <param name="series">s�ries de valeurs</param>
		/// <param name="xValues">s�ries de libell�s</param>
		/// <param name="yValues">s�ries de valeurs</param>
		/// <param name="barColors">couleurs du graphique</param>		
		/// <param name="chartAreaName">Nom du conteneur de l'image</param>
		/// <param name="legendText">Legende Texte</param>
		/// <param name="fontSize">Font size</param>
		/// <returns>s�ries de valeurs</returns>
		private static  Dundas.Charting.WinControl.Series SetSeriesSeasonalityDistribution(DataTable dt ,ChartArea chartArea,Dundas.Charting.WinControl.Series series,string[] xValues,double[] yValues,Color[] barColors,string chartAreaName,string legendText,int fontSize){
			
			#region  Cr�ation graphique
			if(xValues!=null && yValues!=null){
								
				#region Cr�ation et d�finition du graphique
				//Type de graphique
				series.Type = SeriesChartType.Line;
				series.ShowLabelAsValue=true;
				series.XValueType=Dundas.Charting.WinControl.ChartValueTypes.String;
				series.YValueType=Dundas.Charting.WinControl.ChartValueTypes.Double;
				series.Enabled=true;
				series.Font=new Font("Arial", (float)fontSize);
				series["LabelStyle"] = "Right";
													
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
		/// Initialise les styles du webcontrol pour m�dia radio et t�l�
		/// </summary>
		/// <param name="sectorDataChart">Objet Webcontrol</param>
		/// <param name="chartAreaUnit">conteneur de l'image r�partition unit�</param>
		/// <param name="chartAreaDistribution">conteneur de l'image distribution</param>
		/// <param name="ImageType">sortie flash</param>
		/// <param name="chart_width">Taille de l'image</param>
		/// <param name="fontSize">Font size</param>
		private static void InitializeComponent(Chart sectorDataChart, ChartArea chartAreaUnit, ChartArea chartAreaDistribution,int chart_width,int fontSize){					

			#region Chart
			sectorDataChart.BackGradientType = GradientType.TopBottom;
			sectorDataChart.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);
			sectorDataChart.ChartAreas[chartAreaUnit.Name].BackColor=Color.FromArgb(222,207,231);		
			sectorDataChart.BorderStyle=ChartDashStyle.Solid;
			sectorDataChart.BorderLineColor=Color.FromArgb(99,73,132);
			sectorDataChart.BorderLineWidth=2;

			sectorDataChart.Width=chart_width;
			sectorDataChart.Height=CHART_HEIGHT;
			sectorDataChart.Legend.Enabled=false;

			#region Axe des X
			SetAxisX(sectorDataChart,chartAreaUnit.Name,fontSize);
			#endregion

			#region Axe des Y
			SetAxisY(sectorDataChart,chartAreaUnit.Name,fontSize);
			#endregion

			#region Axe des Y2
			SetAxisY2(sectorDataChart,chartAreaUnit.Name,fontSize);
			#endregion

			#endregion	

			#region Titre
			//titre unit� de base			
			sectorDataChart.Titles.Add(chartAreaUnit.Name);
			sectorDataChart.Titles[0].DockInsideChartArea=true;
			sectorDataChart.Titles[0].Position.X=TITLE_POSITION_X;
			sectorDataChart.Titles[0].Position.Y=UNIT_TITLE_POSITION_Y;
			sectorDataChart.Titles[0].Font=new Font("Arial", (float)TITLE_FONT_SIZE);
			sectorDataChart.Titles[0].Color=Color.FromArgb(100,72,131);
			sectorDataChart.Titles[0].DockToChartArea=chartAreaUnit.Name;
			#endregion
		}
		#endregion

		#region InitializeComponentDistribution
		/// <summary>
		/// Initialise les styles du webcontrol pour m�dia radio et t�l�
		/// </summary>
		/// <param name="sectorDataChart">Objet Webcontrol</param>
		/// <param name="chartAreaUnit">conteneur de l'image r�partition unit�</param>
		/// <param name="chartAreaDistribution">conteneur de l'image distribution</param>
		/// <param name="ImageType">sortie flash</param>
		/// <param name="chart_width">Taille de l'image</param>
		/// <param name="fontSize">Font size</param>
		private static void InitializeComponentDistribution(Chart sectorDataChart, ChartArea chartAreaDistribution,int chart_width,int fontSize){

			#region Chart
			sectorDataChart.BackGradientType = GradientType.TopBottom;
			sectorDataChart.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);
			sectorDataChart.ChartAreas[chartAreaDistribution.Name].BackColor=Color.FromArgb(222,207,231);
			sectorDataChart.BorderStyle=ChartDashStyle.Solid;
			sectorDataChart.BorderLineColor=Color.FromArgb(99,73,132);
			sectorDataChart.BorderLineWidth=2;

			sectorDataChart.Width=chart_width;
			sectorDataChart.Height=CHART_HEIGHT;
			sectorDataChart.Legend.Enabled=false;

			#region Axe des X
			SetAxisX(sectorDataChart,chartAreaDistribution.Name,fontSize);
			#endregion

			#region Axe des Y
			SetAxisY(sectorDataChart,chartAreaDistribution.Name,fontSize);
			#endregion

			#region Axe des Y2
			SetAxisY2(sectorDataChart,chartAreaDistribution.Name,fontSize);
			#endregion

			#endregion	

			#region Titre
			sectorDataChart.Titles.Add(chartAreaDistribution.Name);
			sectorDataChart.Titles[0].DockInsideChartArea=true;
			sectorDataChart.Titles[0].Position.X=TITLE_POSITION_X;
			sectorDataChart.Titles[0].Position.Y=UNIT_TITLE_POSITION_Y;
			sectorDataChart.Titles[0].Font=new Font("Arial", (float)TITLE_FONT_SIZE);
			sectorDataChart.Titles[0].Color=Color.FromArgb(100,72,131);
			sectorDataChart.Titles[0].DockToChartArea=chartAreaDistribution.Name;
			#endregion
		}
		#endregion

		#region Set AxisX
		/// <summary>
		/// Param�trages de l'axe des X
		/// </summary>
		/// <param name="sectorDataChart">Objet Webcontrol</param>
		/// <param name="chartAreaName">Le nom de la chart Area</param>
		/// <param name="fontSize">Font size</param>
		private static void SetAxisX(Chart sectorDataChart, string chartAreaName,int fontSize){
			sectorDataChart.ChartAreas[chartAreaName].AxisX.LabelStyle.Enabled = true;
			sectorDataChart.ChartAreas[chartAreaName].AxisX.LabelsAutoFit = false;
			sectorDataChart.ChartAreas[chartAreaName].AxisX.LabelStyle.Font=new Font("Arial", (float)fontSize);
			sectorDataChart.ChartAreas[chartAreaName].AxisX.MajorGrid.LineWidth=0;
			sectorDataChart.ChartAreas[chartAreaName].AxisX.Interval=1;				
			sectorDataChart.ChartAreas[chartAreaName].AxisX.LabelStyle.FontAngle = SERIES_ANGLE;
		}
		#endregion

		#region Set AxisY
		/// <summary>
		/// Param�trages de l'axe des y
		/// </summary>
		/// <param name="sectorDataChart">Objet Webcontrol</param>
		/// <param name="chartAreaName">Le nom de la chart Area</param>
		/// 		/// <param name="fontSize">Font size</param>
		private static void SetAxisY(Chart sectorDataChart, string chartAreaName,int fontSize){
			sectorDataChart.ChartAreas[chartAreaName].AxisY.Enabled=AxisEnabled.True;
			sectorDataChart.ChartAreas[chartAreaName].AxisY.LabelStyle.Enabled=true;
			sectorDataChart.ChartAreas[chartAreaName].AxisY.LabelsAutoFit=false;			
			sectorDataChart.ChartAreas[chartAreaName].AxisY.LabelStyle.Font=new Font("Arial", (float)fontSize);
			sectorDataChart.ChartAreas[chartAreaName].AxisY.TitleFont=new Font("Arial", (float)fontSize);
			sectorDataChart.ChartAreas[chartAreaName].AxisY.MajorGrid.LineWidth=0;
		}
		#endregion

		#region Set AxisY2
		/// <summary>
		/// Param�trages de l'axe des y2
		/// </summary>
		/// <param name="sectorDataChart">Objet Webcontrol</param>
		/// <param name="chartAreaName">Le nom de la chart Area</param>
		/// 		/// <param name="fontSize">Font size</param>
		private static void SetAxisY2(Chart sectorDataChart, string chartAreaName,int fontSize){
			sectorDataChart.ChartAreas[chartAreaName].AxisY2.Enabled=AxisEnabled.True;
			sectorDataChart.ChartAreas[chartAreaName].AxisY2.LabelStyle.Enabled=true;
			sectorDataChart.ChartAreas[chartAreaName].AxisY2.LabelsAutoFit=false;
			sectorDataChart.ChartAreas[chartAreaName].AxisY2.LabelStyle.Font=new Font("Arial", (float)fontSize);
			sectorDataChart.ChartAreas[chartAreaName].AxisY2.TitleFont=new Font("Arial", (float)fontSize);
		}
		#endregion

		#endregion

	}
}
