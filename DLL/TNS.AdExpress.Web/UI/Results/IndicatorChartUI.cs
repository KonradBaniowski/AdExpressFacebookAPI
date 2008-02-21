#region Informations
// Auteur: A. Obermeyer 
// Date de création : 21/10/2004 
// Date de modification : 21/10/2004 
//		12/08/2005		G. Facon		Nom de fonction et suppression des propriétés
//	24/10/2005	D. V. Mussuma	Intégration unité Keuros	
#endregion

using System;
using System.Data;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.AdExpress.Web.Core.Translation;
using Dundas.Charting.WebControl;
using TNS.AdExpress.Web.Core.Sessions;
using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TblFormatCst = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using TNS.FrameWork;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.UI.Results{
	/// <summary>
	/// Classe pour construire les graphiques
	/// </summary>
	public class IndicatorChartUI:Chart{
		
		#region Constantes
		/// <summary>
		/// Hauteur d'un graphique stratégie média
		/// </summary>
		const int MEDIA_STRATEGY_HEIGHT_GRAPHIC=300;
		#endregion

		#region Variables
//		/// <summary>
//		/// Variables Sessions
//		/// </summary>
//		protected WebSession _webSession;
//		/// <summary>
//		/// liste
//		/// </summary>
//		protected System.Web.UI.WebControls.RadioButtonList _listBox;
//		/// <summary>
//		/// Type d'année
//		/// </summary>
//		protected FrameWorkConstantes.Results.PalmaresRecap.typeYearSelected _typeYear;
//		/// <summary>
//		/// Type annonceur ou produit
//		/// </summary>
//		protected FrameWorkConstantes.Results.PalmaresRecap.ElementType _tableType;

		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public IndicatorChartUI():base(){			            
		}		
		#endregion

		#region Palmares
		/// <summary>
		/// Histogramme pour palmares
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="typeYear">Type d'année</param>
		/// <param name="tableType">Type de table</param>
		/// <param name="flashType">si oui flash sinon image jpeg</param>		
		public void PalmaresBar(WebSession webSession,FrameWorkConstantes.Results.PalmaresRecap.typeYearSelected typeYear,FrameWorkConstantes.Results.PalmaresRecap.ElementType tableType,bool flashType ){
			
			#region Variables
			Series series = new Series("Palmares");
			ChartArea chartArea=new ChartArea();			
			bool referenceElement=false;
			bool competitorElement=false;
			// Il y a au moins un élément
			bool oneProductExist=false;
			#endregion

			#region Animation Flash
			if(flashType){
				this.ImageType=ChartImageType.Flash;
				this.AnimationTheme = AnimationTheme.GrowingTogether;
				this.AnimationDuration = 5;
				this.RepeatAnimation = false;
			}else{
				this.ImageType=ChartImageType.Jpeg;
				
			}
			
			
			#endregion

			#region Test Ajout Logo
			
			#endregion

			this.Series.Add(series);
			this.ChartAreas.Add(chartArea);

			string strChartArea = this.Series["Palmares"].ChartArea;
			object[,] tab=TNS.AdExpress.Web.Rules.Results.IndicatorPalmaresRules.GetFormattedTable(webSession,typeYear,tableType);
			
			if(tab.GetLongLength(0)==1 || double.Parse(tab[0,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N].ToString())==0){
				this.Visible=false;
			}

			#region Chart
			this.Width=new Unit("750px");
			this.Height=new Unit("750px");
			this.BackGradientType = GradientType.TopBottom;
			this.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);
			this.ChartAreas[strChartArea].BackColor=Color.FromArgb(222,207,231);
			this.DataManipulator.Sort(PointsSortOrder.Descending,series);
			this.BorderStyle=ChartDashStyle.Solid;
			this.BorderLineColor=Color.FromArgb(99,73,132);
			this.BorderLineWidth=2;
			#endregion

			#region Titre
			Title title;
			if(tableType==FrameWorkConstantes.Results.PalmaresRecap.ElementType.advertiser){
				 title = new Title(""+GestionWeb.GetWebWord(1184,webSession.SiteLanguage)+"");
			}else{
				 title = new Title(""+GestionWeb.GetWebWord(1169,webSession.SiteLanguage)+"");
			}
			title.Font = new Font("Arial", (float)14);
			this.Titles.Add(title);
			#endregion	

			#region Copyright & Logo
			AddHeader(webSession, !flashType);			
			#endregion

			#region Series
			series.Type = SeriesChartType.Column;
			series.ShowLabelAsValue=true;
			series.XValueType=Dundas.Charting.WebControl.ChartValueTypes.String;
			series.YValueType=Dundas.Charting.WebControl.ChartValueTypes.Double;
			series.Color= Color.FromArgb(148,121,181);
			series.Enabled=true;
			series.Font=new Font("Arial", (float)10);
			series.FontAngle=45;
			
			#endregion			
			
			#region Parcours de tab
			for(int i=1;i<tab.GetLongLength(0) && i<11 ;i++){
				
				if(tab[i,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N].ToString()!="0" 
					&& WebFunctions.CheckedText.IsStringEmpty(WebFunctions.Units.ConvertUnitValueToString(tab[0,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N].ToString(),webSession.Unit)) ){
					oneProductExist=true;
					
//					series.Points.AddXY(tab[i,FrameWorkConstantes.Results.PalmaresRecap.PRODUCT].ToString(),(int)(double)tab[i,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N]);
					series.Points.AddXY(tab[i,FrameWorkConstantes.Results.PalmaresRecap.PRODUCT].ToString(),(int)double.Parse(WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N].ToString(),webSession.Unit)));					
				
					series.Points[i-1].ShowInLegend=true;
					// Coloration des concurrents en rouge
					if(tab[i,FrameWorkConstantes.Results.PalmaresRecap.COMPETITOR]!=null && (int)tab[i,FrameWorkConstantes.Results.PalmaresRecap.COMPETITOR]==2){
						series.Points[i-1].Color=Color.FromArgb(255,223,222);
						competitorElement=true;
					}
						// Coloration des références en vert
					else if(tab[i,FrameWorkConstantes.Results.PalmaresRecap.COMPETITOR]!=null && (int)tab[i,FrameWorkConstantes.Results.PalmaresRecap.COMPETITOR]==1){
						series.Points[i-1].Color=Color.FromArgb(222,255,222);	
						referenceElement=true;
					}	
				}
			}
			if(!oneProductExist)
			this.Visible=false;
			#endregion

			#region Légendes
			if(tableType==FrameWorkConstantes.Results.PalmaresRecap.ElementType.advertiser){
				series.LegendText=""+GestionWeb.GetWebWord(1106,webSession.SiteLanguage)+"";
			}
			else{
				series.LegendText=""+GestionWeb.GetWebWord(1200,webSession.SiteLanguage)+"";
			}
			LegendItem legendItemReference = new LegendItem();			
			legendItemReference.BorderWidth=0;
			legendItemReference.Color=Color.FromArgb(222,255,222);
			if(referenceElement){
				if(tableType==FrameWorkConstantes.Results.PalmaresRecap.ElementType.advertiser){
					legendItemReference.Name=GestionWeb.GetWebWord(1201,webSession.SiteLanguage);
					this.Legends["Default"].CustomItems.Add(legendItemReference);
				}else{					
					legendItemReference.Name=GestionWeb.GetWebWord(1203,webSession.SiteLanguage);
					this.Legends["Default"].CustomItems.Add(legendItemReference);
					
				}
			}			
			LegendItem legendItemCompetitor = new LegendItem();
			legendItemCompetitor.BorderWidth=0;			
			legendItemCompetitor.Color=Color.FromArgb(255,223,222);

			if(competitorElement){
				if(tableType==FrameWorkConstantes.Results.PalmaresRecap.ElementType.advertiser){
					legendItemCompetitor.Name=GestionWeb.GetWebWord(1202,webSession.SiteLanguage);
					this.Legends["Default"].CustomItems.Add(legendItemCompetitor);
				}
				else{
					legendItemCompetitor.Name=GestionWeb.GetWebWord(1204,webSession.SiteLanguage);
					this.Legends["Default"].CustomItems.Add(legendItemCompetitor);
				}
			}
			#endregion
		
			#region Axe des X
			this.ChartAreas[strChartArea].AxisX.LabelStyle.Enabled = true;
			this.ChartAreas[strChartArea].AxisX.LabelsAutoFit = false;
			this.ChartAreas[strChartArea].AxisX.LabelStyle.Font=new Font("Arial", (float)8);
			this.ChartAreas[strChartArea].AxisX.MajorGrid.LineWidth=0;
			this.ChartAreas[strChartArea].AxisX.Interval=1;				
			this.ChartAreas[strChartArea].AxisX.LabelStyle.FontAngle = 35;


			#endregion

			#region Axe des Y

			this.ChartAreas[strChartArea].AxisY.Enabled=AxisEnabled.True;
			this.ChartAreas[strChartArea].AxisY.LabelStyle.Enabled=true;
			this.ChartAreas[strChartArea].AxisY.LabelsAutoFit=false;			

			this.ChartAreas[strChartArea].AxisY.LabelStyle.Font=new Font("Arial", (float)10);
			this.ChartAreas[strChartArea].AxisY.Title=""+GestionWeb.GetWebWord(1206,webSession.SiteLanguage)+"";
			this.ChartAreas[strChartArea].AxisY.TitleFont=new Font("Arial", (float)10);
			if(WebFunctions.CheckedText.IsStringEmpty(WebFunctions.Units.ConvertUnitValueToString(tab[0,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N].ToString(),webSession.Unit)))
//			this.ChartAreas[strChartArea].AxisY.Maximum=double.Parse(tab[0,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N].ToString());
			this.ChartAreas[strChartArea].AxisY.Maximum=double.Parse(WebFunctions.Units.ConvertUnitValueToString(tab[0,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N].ToString(),webSession.Unit));
			else this.ChartAreas[strChartArea].AxisY.Maximum = (double)0.0;
			this.ChartAreas[strChartArea].AxisY.MajorGrid.LineWidth=0;
			
			#endregion

			#region Axe des Y2

			this.ChartAreas[strChartArea].AxisY2.Enabled=AxisEnabled.True;
			this.ChartAreas[strChartArea].AxisY2.LabelStyle.Enabled=true;
			this.ChartAreas[strChartArea].AxisY2.LabelsAutoFit=false;
			
			this.ChartAreas[strChartArea].AxisY2.LabelStyle.Font=new Font("Arial", (float)10);
			this.ChartAreas[strChartArea].AxisY2.TitleFont=new Font("Arial", (float)10);
			this.ChartAreas[strChartArea].AxisY2.Maximum=100;
			this.ChartAreas[strChartArea].AxisY2.Title=""+GestionWeb.GetWebWord(1205,webSession.SiteLanguage)+"";

			#endregion	
			
			
			
		}
		#endregion

		#region Evolution

		/// <summary>
		/// Affiche un histogramme
		/// </summary>
		/// <param name="webSession">Session</param>
		/// <param name="tableType">Annonceurs ou référence</param>
		/// <param name="typeFlash">si oui flash sinon image jpeg</param>
		public void EvolutionBar(WebSession webSession,FrameWorkConstantes.Results.EvolutionRecap.ElementType tableType,bool typeFlash ){
			
			#region Variables
			Series series = new Series("Evolution");
			ChartArea chartArea=new ChartArea();
			bool referenceElement=false;
			bool competitorElement=false;
			long last;
			int compteur=0;
			#endregion

			#region Animation Flash
			if(typeFlash){
				this.ImageType=ChartImageType.Flash;
				this.AnimationTheme = AnimationTheme.GrowingTogether;
				this.AnimationDuration = 5;
				this.RepeatAnimation = false;
			}
			else{
				this.ImageType=ChartImageType.Jpeg;
			}
			#endregion

			this.Series.Add(series);
			this.ChartAreas.Add(chartArea);
			string strChartArea = this.Series["Evolution"].ChartArea;
			object[,] tab=TNS.AdExpress.Web.Rules.Results.IndicatorEvolutionRules.GetFormattedTable(webSession,tableType);
			last=tab.GetLongLength(0)-1;

			if(tab.GetLongLength(0)==0){
				this.Visible=false;
			}

			#region Chart
			this.Width=new Unit("850px");
			this.Height=new Unit("500px");
			this.BackGradientType = GradientType.TopBottom;
			this.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);
			this.ChartAreas[strChartArea].BackColor=Color.FromArgb(222,207,231);			
			this.BorderStyle=ChartDashStyle.Solid;
			this.BorderLineColor=Color.FromArgb(99,73,132);
			this.BorderLineWidth=2;
			#endregion

			#region Titre
			Title title;
			if(tableType==FrameWorkConstantes.Results.EvolutionRecap.ElementType.advertiser){
				title = new Title(GestionWeb.GetWebWord(1215,webSession.SiteLanguage));
			}else{
				title = new Title(GestionWeb.GetWebWord(1216,webSession.SiteLanguage));
			}
			title.Font = new Font("Arial", (float)14);
			this.Titles.Add(title);
			#endregion	

			#region Copyright & Logo
			AddHeader(webSession, !typeFlash);
			#endregion
					
			#region Series
			series.Type = SeriesChartType.Column;
			series.ShowLabelAsValue=true;
			series.XValueType=Dundas.Charting.WebControl.ChartValueTypes.String;
			series.YValueType=Dundas.Charting.WebControl.ChartValueTypes.Double;
			series.Color= Color.FromArgb(148,121,181);
			series.Enabled=true;
			series.Font=new Font("Arial", (float)10);
			series.FontAngle=90;
			
			#endregion			
			
			#region Parcours de tab
			for(int i=0;i<tab.GetLongLength(0) && i<10 ;i++){
			
				if(WebFunctions.CheckedText.IsStringEmpty(WebFunctions.Units.ConvertUnitValueToString(tab[0,FrameWorkConstantes.Results.EvolutionRecap.ECART].ToString(),webSession.Unit)) 
						  && double.Parse(tab[i,FrameWorkConstantes.Results.EvolutionRecap.ECART].ToString())>0){	
				
//					series.Points.AddXY(tab[i,FrameWorkConstantes.Results.EvolutionRecap.PRODUCT].ToString(),(int)(double)tab[i,FrameWorkConstantes.Results.EvolutionRecap.ECART]);
					series.Points.AddXY(tab[i,FrameWorkConstantes.Results.EvolutionRecap.PRODUCT].ToString(),(int)double.Parse(WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkConstantes.Results.EvolutionRecap.ECART].ToString(),webSession.Unit)));
					series.Points[compteur].ShowInLegend=true;
					// Coloration des concurrents en rouge
					if(tab[i,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]!=null && (int)tab[i,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]==2){
						series.Points[compteur].Color=Color.FromArgb(255,223,222);
						competitorElement=true;
					}
						// Coloration des références en vert
					else if(tab[i,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]!=null && (int)tab[i,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]==1){
						series.Points[compteur].Color=Color.FromArgb(222,255,222);	
						referenceElement=true;
					}	
					
					compteur++;
				}

				if( WebFunctions.CheckedText.IsStringEmpty(WebFunctions.Units.ConvertUnitValueToString(tab[0,FrameWorkConstantes.Results.EvolutionRecap.ECART].ToString(),webSession.Unit)) 
					&& double.Parse(tab[last,FrameWorkConstantes.Results.EvolutionRecap.ECART].ToString())<0){					
						series.Points.AddXY(tab[last,FrameWorkConstantes.Results.EvolutionRecap.PRODUCT].ToString(),(int)double.Parse(WebFunctions.Units.ConvertUnitValueToString(tab[last,FrameWorkConstantes.Results.EvolutionRecap.ECART].ToString(),webSession.Unit).Replace(" ","").Trim()));
					
					series.Points[compteur].ShowInLegend=true;
					series.Points[compteur].CustomAttributes="LabelStyle=top";
					

					// Coloration des concurrents en rouge
					if(tab[last,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]!=null && (int)tab[last,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]==2){
						series.Points[compteur].Color=Color.FromArgb(255,223,222);
						competitorElement=true;
					}
						// Coloration des références en vert
					else if(tab[last,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]!=null && (int)tab[last,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]==1){
						series.Points[compteur].Color=Color.FromArgb(222,255,222);	
						referenceElement=true;
					}
	
					compteur++;
				}
				
				last--;

			}
			#endregion

			#region Légendes
			if(tableType==FrameWorkConstantes.Results.EvolutionRecap.ElementType.advertiser){
				series.LegendText=""+GestionWeb.GetWebWord(1106,webSession.SiteLanguage)+"";
			}
			else{
				series.LegendText=""+GestionWeb.GetWebWord(1200,webSession.SiteLanguage)+"";
			}
			LegendItem legendItemReference = new LegendItem();			
			legendItemReference.BorderWidth=0;
			legendItemReference.Color=Color.FromArgb(222,255,222);
			if(referenceElement){
				if(tableType==FrameWorkConstantes.Results.EvolutionRecap.ElementType.advertiser){
					legendItemReference.Name=GestionWeb.GetWebWord(1201,webSession.SiteLanguage);
					this.Legends["Default"].CustomItems.Add(legendItemReference);
				}else{					
					legendItemReference.Name=GestionWeb.GetWebWord(1203,webSession.SiteLanguage);
					this.Legends["Default"].CustomItems.Add(legendItemReference);
					
				}
			}			
			LegendItem legendItemCompetitor = new LegendItem();
			legendItemCompetitor.BorderWidth=0;			
			legendItemCompetitor.Color=Color.FromArgb(255,223,222);

			if(competitorElement){
				if(tableType==FrameWorkConstantes.Results.EvolutionRecap.ElementType.advertiser){
					legendItemCompetitor.Name=GestionWeb.GetWebWord(1202,webSession.SiteLanguage);
					this.Legends["Default"].CustomItems.Add(legendItemCompetitor);
				}
				else{
					legendItemCompetitor.Name=GestionWeb.GetWebWord(1204,webSession.SiteLanguage);
					this.Legends["Default"].CustomItems.Add(legendItemCompetitor);
				}
			}
			#endregion
		
			this.DataManipulator.Sort(PointsSortOrder.Descending,series);

			#region Axe des X
			this.ChartAreas[strChartArea].AxisX.LabelStyle.Enabled = true;
			this.ChartAreas[strChartArea].AxisX.LabelsAutoFit = false;
			this.ChartAreas[strChartArea].AxisX.LabelStyle.Font=new Font("Arial", (float)8);
			this.ChartAreas[strChartArea].AxisX.MajorGrid.LineWidth=0;
			this.ChartAreas[strChartArea].AxisX.Interval=1;				
			this.ChartAreas[strChartArea].AxisX.LabelStyle.FontAngle = 35;			

			#endregion

			#region Axe des Y

			this.ChartAreas[strChartArea].AxisY.Enabled=AxisEnabled.True;
			this.ChartAreas[strChartArea].AxisY.LabelStyle.Enabled=true;
			this.ChartAreas[strChartArea].AxisY.LabelsAutoFit=false;			

			this.ChartAreas[strChartArea].AxisY.LabelStyle.Font=new Font("Arial", (float)10);
			//	this.ChartAreas[strChartArea].AxisY.Title=""+GestionWeb.GetWebWord(1217,webSession.SiteLanguage)+"";
			this.ChartAreas[strChartArea].AxisY.TitleFont=new Font("Arial", (float)10);
			//	this.ChartAreas[strChartArea].AxisY.Maximum=double.Parse(tab[0,FrameWorkConstantes.Results.EvolutionRecap.TOTAL_N].ToString());
			this.ChartAreas[strChartArea].AxisY.MajorGrid.LineWidth=0;
			
			#endregion

			#region Axe des Y2

			this.ChartAreas[strChartArea].AxisY2.Enabled=AxisEnabled.True;
			this.ChartAreas[strChartArea].AxisY2.LabelStyle.Enabled=true;
			this.ChartAreas[strChartArea].AxisY2.LabelsAutoFit=false;
			
			this.ChartAreas[strChartArea].AxisY2.LabelStyle.Font=new Font("Arial", (float)10);
			this.ChartAreas[strChartArea].AxisY2.TitleFont=new Font("Arial", (float)10);
			//this.ChartAreas[strChartArea].AxisY2.Maximum=100;
			this.ChartAreas[strChartArea].AxisY2.Title=""+GestionWeb.GetWebWord(1217,webSession.SiteLanguage)+"";

			#endregion					
		
		
		}

		#endregion

		#region Saisonnalité
		/// <summary>
		/// Saisonnalité
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="bigFormat">True si grand format</param>
		/// <param name="typeFlash">si oui flash sinon image jpeg</param>
		public void SeasonalityLine(WebSession webSession,bool bigFormat,bool typeFlash){
			
			#region Variables
			Series serieUnivers = new Series("Seasonality");
			Series serieSectorMarket = new Series();
			Series serieMediumMonth = new Series();

			ChartArea chartArea=new ChartArea();
			bool referenceElement=false;
			bool competitorElement=false;
			
			int compteur=1;
			// Nombre de mois
			int nbMonth=0;
			int i=0;
		
			double number=0;
			int oldMonth=-1;
			


			double mediumMonth=0;

			Hashtable advertiserTotal = new Hashtable();
			Hashtable advertiserSerie=new Hashtable();

			#endregion

			#region Animation Flash
			if(typeFlash){
				this.ImageType=ChartImageType.Flash;
				this.AnimationTheme = AnimationTheme.Fading;
				this.AnimationDuration = 6;
				this.RepeatAnimation = false;
			}
			else{
				this.ImageType=ChartImageType.Jpeg;
			}
			#endregion

			this.Series.Add(serieUnivers);
			this.Series.Add(serieSectorMarket);
			this.Series.Add(serieMediumMonth);

			this.ChartAreas.Add(chartArea);
			string strChartArea = this.Series["Seasonality"].ChartArea;
					
	 		//object[] tab=TNS.AdExpress.Web.Rules.Results.IndicatorSeasonalityRules.getFormattedTable(webSession);
			
			object[,] tab=TNS.AdExpress.Web.Rules.Results.IndicatorSeasonalityRules.GetChartFormattedTable(webSession);			
			

			if(tab.Length==0){
				this.Visible=false;
			}
			

			compteur=0;
		
			long valueAdvertiser=0;
			if(tab!=null){
				//advertiserNumber=tabAdvertiser.GetLength(0)/(tabUniverse.GetLength(0)-1);
				for(i=0;i<tab.GetLength(0);i++){
					if(advertiserTotal[long.Parse(tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())]==null){
						advertiserTotal.Add(long.Parse(tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString()),long.Parse(tab[i,FrameWorkConstantes.Results.Seasonality.INVEST_COLUMN_INDEX].ToString()));
						if(long.Parse(tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())!=FrameWorkConstantes.Results.Seasonality.ID_TOTAL_UNIVERSE_COLUMN_INDEX
							&& long.Parse(tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())!=FrameWorkConstantes.Results.Seasonality.ID_TOTAL_MARKET_COLUMN_INDEX
							&& long.Parse(tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())!=FrameWorkConstantes.Results.Seasonality.ID_TOTAL_SECTOR_COLUMN_INDEX
							){
							advertiserSerie.Add(long.Parse(tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString()),new Series());
							this.Series.Add(((Series)advertiserSerie[long.Parse( tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())]));
						}
					}
					else{
						valueAdvertiser=(long)advertiserTotal[long.Parse( tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())];
						advertiserTotal[long.Parse(tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())]=valueAdvertiser+long.Parse(tab[i,FrameWorkConstantes.Results.Seasonality.INVEST_COLUMN_INDEX].ToString());
					}
					if(oldMonth!=int.Parse(tab[i,FrameWorkConstantes.Results.Seasonality.ID_MONTH_COLUMN_INDEX].ToString())){
						nbMonth++;
						oldMonth=int.Parse(tab[i,FrameWorkConstantes.Results.Seasonality.ID_MONTH_COLUMN_INDEX].ToString());
					}

				}
			}

			

			//last=tab.GetLongLength(0)-1;

			#region Chart
			if(!bigFormat) {
				this.Width=new Unit("850px");
				this.Height=new Unit("500px");
			}
			else{
				this.Width=new Unit("1150px");
				this.Height=new Unit("700px");
			}
			this.BackGradientType = GradientType.TopBottom;
			this.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);
			this.ChartAreas[strChartArea].BackColor=Color.FromArgb(222,207,231);			
			this.BorderStyle=ChartDashStyle.Solid;
			this.BorderLineColor=Color.FromArgb(99,73,132);
			this.BorderLineWidth=2;
			#endregion

			#region Titre
			Title title;
			title = new Title(GestionWeb.GetWebWord(1139,webSession.SiteLanguage));
			title.Font = new Font("Arial", (float)14);
			this.Titles.Add(title);
			#endregion	

			#region Copyright & Logo
			AddHeader(webSession, !typeFlash);
			#endregion
					
			#region Series
			serieUnivers.Type = SeriesChartType.Line;
			serieUnivers.ShowLabelAsValue=true;
			serieUnivers.XValueType=Dundas.Charting.WebControl.ChartValueTypes.String;
			serieUnivers.YValueType=Dundas.Charting.WebControl.ChartValueTypes.Double;
			serieUnivers.Enabled=true;
			serieUnivers.Font=new Font("Arial", (float)8);
			serieUnivers["LabelStyle"] = "Right";
			serieUnivers.SmartLabels.Enabled = true;
			serieUnivers.SmartLabels.CalloutLineStyle = ChartDashStyle.Dot;
			serieUnivers.SmartLabels.MinMovingDistance = 3;

			serieSectorMarket.Type = SeriesChartType.Line;
			serieSectorMarket.ShowLabelAsValue=true;
			serieSectorMarket.XValueType=Dundas.Charting.WebControl.ChartValueTypes.String;
			serieSectorMarket.YValueType=Dundas.Charting.WebControl.ChartValueTypes.Double;
			serieSectorMarket.Enabled=true;
			serieSectorMarket.Font=new Font("Arial", (float)8);
			serieSectorMarket["LabelStyle"] = "Right";
			serieSectorMarket.SmartLabels.Enabled = true;
			serieSectorMarket.SmartLabels.CalloutLineStyle = ChartDashStyle.Dot;
			serieSectorMarket.SmartLabels.MinMovingDistance = 3;
							
			serieMediumMonth.Type = SeriesChartType.Line;
			serieMediumMonth.ShowLabelAsValue=false;
			serieMediumMonth.XValueType=Dundas.Charting.WebControl.ChartValueTypes.String;
			serieMediumMonth.YValueType=Dundas.Charting.WebControl.ChartValueTypes.Double;
			serieMediumMonth.Enabled=true;
			serieMediumMonth.Font=new Font("Arial", (float)8);
			serieMediumMonth.SmartLabels.Enabled = true;
			serieMediumMonth.SmartLabels.CalloutLineStyle = ChartDashStyle.Dot;
			serieMediumMonth.SmartLabels.MinMovingDistance = 3;
			serieMediumMonth.LabelToolTip= GestionWeb.GetWebWord(1233,webSession.SiteLanguage);




			//serieUnivers.FontAngle=90;
			
			#endregion			
			

			DateTime PeriodBeginningDate = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
			
			compteur=-1;
			oldMonth=-1;

			if(tab!=null){
			// Calcul des totaux pour les annonceurs
				for(i=0;i<tab.GetLength(0);i++){

					if(oldMonth!=int.Parse(tab[i,FrameWorkConstantes.Results.Seasonality.ID_MONTH_COLUMN_INDEX].ToString())){
						compteur++;
						oldMonth=int.Parse(tab[i,FrameWorkConstantes.Results.Seasonality.ID_MONTH_COLUMN_INDEX].ToString());
					}
					
				
					if(long.Parse(tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())==FrameWorkConstantes.Results.Seasonality.ID_TOTAL_UNIVERSE_COLUMN_INDEX){
						if(double.Parse(advertiserTotal[long.Parse( tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())].ToString())>0){
							number= double.Parse(tab[i,FrameWorkConstantes.Results.Seasonality.INVEST_COLUMN_INDEX].ToString())/double.Parse(advertiserTotal[long.Parse( tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())].ToString()) *100;
							serieUnivers.Points.AddXY(TNS.FrameWork.Date.MonthString.Get(PeriodBeginningDate.AddMonths(compteur).Month,webSession.SiteLanguage,0),double.Parse(number.ToString("0.00")));
						}
					}else if(long.Parse(tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())==FrameWorkConstantes.Results.Seasonality.ID_TOTAL_SECTOR_COLUMN_INDEX
						|| long.Parse(tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())==FrameWorkConstantes.Results.Seasonality.ID_TOTAL_MARKET_COLUMN_INDEX
						
						){
						if(double.Parse(advertiserTotal[long.Parse( tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())].ToString())>0){
							number= double.Parse(tab[i,FrameWorkConstantes.Results.Seasonality.INVEST_COLUMN_INDEX].ToString())/double.Parse(advertiserTotal[long.Parse( tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())].ToString()) *100;
							serieSectorMarket.Points.AddXY(TNS.FrameWork.Date.MonthString.Get(PeriodBeginningDate.AddMonths(compteur).Month,webSession.SiteLanguage,0),double.Parse(number.ToString("0.00")));
						
							number=(double)100/nbMonth;
							serieMediumMonth.Points.AddXY(TNS.FrameWork.Date.MonthString.Get(PeriodBeginningDate.AddMonths(compteur).Month,webSession.SiteLanguage,0),double.Parse(number.ToString("0.00")));
							mediumMonth=double.Parse(number.ToString("0.00"));
						}							
					}
					else{
						Series s = (Series)advertiserSerie[long.Parse( tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())];
						s.Type = SeriesChartType.Line;
						s.ShowLabelAsValue=true;
						s.XValueType=Dundas.Charting.WebControl.ChartValueTypes.String;
						s.YValueType=Dundas.Charting.WebControl.ChartValueTypes.Double;					
						s.Enabled=true;
						s.Font=new Font("Arial", (float)8);
						s.LegendText=tab[i,FrameWorkConstantes.Results.Seasonality.LABEL_ELEMENT_COLUMN_INDEX].ToString();
						s.SmartLabels.Enabled = true;
						s.SmartLabels.CalloutLineStyle = ChartDashStyle.Dot;
						s.SmartLabels.MinMovingDistance = 3;
						s.LabelToolTip = tab[i,FrameWorkConstantes.Results.Seasonality.LABEL_ELEMENT_COLUMN_INDEX].ToString();


						if(double.Parse(advertiserTotal[long.Parse( tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())].ToString())>0){
							number= double.Parse(tab[i,FrameWorkConstantes.Results.Seasonality.INVEST_COLUMN_INDEX].ToString())/double.Parse(advertiserTotal[long.Parse( tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())].ToString()) *100;
							((Series)advertiserSerie[long.Parse( tab[i,FrameWorkConstantes.Results.Seasonality.ID_ELEMENT_COLUMN_INDEX].ToString())]).Points.AddXY(TNS.FrameWork.Date.MonthString.Get(PeriodBeginningDate.AddMonths(compteur).Month,webSession.SiteLanguage,0),double.Parse(number.ToString("0.00")));
							
						}					
					}

					
				
				}
			}

			#region Légendes
			//univers
			serieUnivers.LegendText=GestionWeb.GetWebWord(1188,webSession.SiteLanguage);
			serieUnivers.LabelToolTip = WebFunctions.Text.SuppressAccent(GestionWeb.GetWebWord(1188,webSession.SiteLanguage));
			// Marché
			if(webSession.ComparaisonCriterion==TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.marketTotal){
				serieSectorMarket.LegendText=GestionWeb.GetWebWord(1316,webSession.SiteLanguage);
				serieSectorMarket.LabelToolTip = WebFunctions.Text.SuppressAccent(GestionWeb.GetWebWord(1316,webSession.SiteLanguage));

			}
			// Famille
			else{
				serieSectorMarket.LegendText=GestionWeb.GetWebWord(1189,webSession.SiteLanguage);
				serieSectorMarket.LabelToolTip = WebFunctions.Text.SuppressAccent(GestionWeb.GetWebWord(1189,webSession.SiteLanguage));
			}
			//Mois Moyen théorique
			serieMediumMonth.LegendText=GestionWeb.GetWebWord(1233,webSession.SiteLanguage)+" "+mediumMonth.ToString()+" %";

			LegendItem legendItemReference = new LegendItem();			
			legendItemReference.BorderWidth=0;
			legendItemReference.Color=Color.FromArgb(222,255,222);
			if(referenceElement){								
					legendItemReference.Name=GestionWeb.GetWebWord(1203,webSession.SiteLanguage);
					this.Legends["Default"].CustomItems.Add(legendItemReference);			
			}			
			LegendItem legendItemCompetitor = new LegendItem();
			legendItemCompetitor.BorderWidth=0;			
			legendItemCompetitor.Color=Color.FromArgb(255,223,222);

			if(competitorElement){
				
					legendItemCompetitor.Name=GestionWeb.GetWebWord(1202,webSession.SiteLanguage);
					this.Legends["Default"].CustomItems.Add(legendItemCompetitor);				
			}


			#endregion
					
			#region Axe des X
			this.ChartAreas[strChartArea].AxisX.LabelStyle.Enabled = true;
			this.ChartAreas[strChartArea].AxisX.LabelsAutoFit = false;
			this.ChartAreas[strChartArea].AxisX.LabelStyle.Font=new Font("Arial", (float)8);
			this.ChartAreas[strChartArea].AxisX.MajorGrid.LineWidth=0;
			this.ChartAreas[strChartArea].AxisX.Interval=1;				
			this.ChartAreas[strChartArea].AxisX.LabelStyle.FontAngle = 35;


			#endregion

			#region Axe des Y

			this.ChartAreas[strChartArea].AxisY.Enabled=AxisEnabled.True;
			this.ChartAreas[strChartArea].AxisY.LabelStyle.Enabled=true;
			this.ChartAreas[strChartArea].AxisY.LabelsAutoFit=false;			

			this.ChartAreas[strChartArea].AxisY.LabelStyle.Font=new Font("Arial", (float)10);
			//	this.ChartAreas[strChartArea].AxisY.Title=""+GestionWeb.GetWebWord(1217,webSession.SiteLanguage)+"";
			this.ChartAreas[strChartArea].AxisY.TitleFont=new Font("Arial", (float)10);
		//	this.ChartAreas[strChartArea].AxisY.Maximum=100;
			this.ChartAreas[strChartArea].AxisY.MajorGrid.LineWidth=0;
			
			#endregion

			#region Axe des Y2

			this.ChartAreas[strChartArea].AxisY2.Enabled=AxisEnabled.True;
			this.ChartAreas[strChartArea].AxisY2.LabelStyle.Enabled=true;
			this.ChartAreas[strChartArea].AxisY2.LabelsAutoFit=false;
			
			this.ChartAreas[strChartArea].AxisY2.LabelStyle.Font=new Font("Arial", (float)10);
			this.ChartAreas[strChartArea].AxisY2.TitleFont=new Font("Arial", (float)10);
		//	this.ChartAreas[strChartArea].AxisY2.Maximum=100;
			this.ChartAreas[strChartArea].AxisY2.Title=""+GestionWeb.GetWebWord(1236,webSession.SiteLanguage)+"";

			#endregion							

		
		}
		#endregion

		#region Stratégie Media

		/// <summary>
		/// Graphiques Stratégie Média
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="typeFlash">Si flash true</param>
		public void MediaStrategyBar(WebSession webSession,bool typeFlash){
			object [,] tab=TNS.AdExpress.Web.Rules.Results.IndicatorMediaStrategyRules.GetChartFormattedTable(webSession,webSession.ComparaisonCriterion);

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
			#endregion
		
			#region Variables
			//ChartArea chartArea=new ChartArea();
			// Il y a au moins un élément
			//bool oneProductExist=false;
			#endregion

			#region Animation Flash
			if(typeFlash){
				//this.ImageType=ChartImageType.Svg;
				this.ImageType=ChartImageType.Flash;
				this.AnimationTheme = AnimationTheme.MovingFromTop;
				this.AnimationDuration = 3;
				this.RepeatAnimation = false;
			}
			else{
				this.ImageType=ChartImageType.Jpeg;
			}
			#endregion
	
			//this.ChartAreas.Add(chartArea);
			
			#region Niveau de détail
			int MEDIA_LEVEL_NUMBER;
			switch(webSession.PreformatedMediaDetail){
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
			this.Width = new Unit("850px");
			this.Height = new Unit("850px");
			this.BackGradientType = GradientType.TopBottom;
			this.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);
			//	this.ChartAreas[strChartArea].BackColor=Color.FromArgb(222,207,231);			
			this.BorderStyle=ChartDashStyle.Solid;
			this.BorderLineColor=Color.FromArgb(99,73,132);
			this.BorderLineWidth=2;
			this.Legend.Enabled=false;
			
			#endregion					
			
			#region Parcours de tab
			
			#region Variables
			double totalUniversValue=0;
			double totalSectorValue=0;
			double totalMarketValue=0;
			double oldValueRefCompetitor=0;
			double elementValue;
			int i,j=0;
			Hashtable listSeriesMedia=new Hashtable();
			Hashtable listSeriesName=new Hashtable();
			Hashtable listSeriesMediaRefCompetitor=new Hashtable();
			Hashtable listTableRefCompetitor=new Hashtable();		
			DataTable tableUnivers=new DataTable();
			DataTable tableSectorMarket=new DataTable();
			#endregion

			// Définition des colonnes
			tableUnivers.Columns.Add("Name");
			tableUnivers.Columns.Add("Position",typeof(double));
			tableSectorMarket.Columns.Add("Name");
			tableSectorMarket.Columns.Add("Position",typeof(double));

			
			// Serie Univers
			listSeriesMedia.Add(GestionWeb.GetWebWord(1780,webSession.SiteLanguage),new Series());
			listSeriesName.Add(0,GestionWeb.GetWebWord(1780,webSession.SiteLanguage));
			// Serie Famille
			if(webSession.ComparaisonCriterion==TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal){
				listSeriesMedia.Add(GestionWeb.GetWebWord(1189,webSession.SiteLanguage),new Series());
				listSeriesName.Add(1,GestionWeb.GetWebWord(1189,webSession.SiteLanguage));
				// Serie Marché
			}
			else{
				listSeriesMedia.Add(GestionWeb.GetWebWord(1316,webSession.SiteLanguage),new Series());
				listSeriesName.Add(1,GestionWeb.GetWebWord(1316,webSession.SiteLanguage));
			}


			// Création des séries (une série par média) que l'on place dans la hashTable listSeriesMedia
			for(i=1;i<tab.GetLongLength(0);i++){			

				//	HashTable avec comme clé le libéllé de l'annonceur référence ou concurrent et comme valeur le total
				if(tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX]!=null){
					if(listSeriesMediaRefCompetitor[tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]==null){
						listSeriesMediaRefCompetitor.Add(tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString(),new double());
					}

					if(listTableRefCompetitor[tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]==null){
						DataTable tableCompetitorRef=new DataTable();
						tableCompetitorRef.Columns.Add("Name");
						tableCompetitorRef.Columns.Add("Position",typeof(double));
						listTableRefCompetitor.Add(tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString(),tableCompetitorRef);

					}

					if(listSeriesMedia[tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]==null){
						listSeriesMedia.Add(tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString(),new Series());
					}
				}
			}	
	

			#region Totaux
			// Création des totaux
			if(MEDIA_LEVEL_NUMBER ==2 || MEDIA_LEVEL_NUMBER==3){
				for(i=1;i<tab.GetLongLength(0);i++){			
					for( j=0;j<FrameWorkConstantes.Results.MediaStrategy.NB_MAX_COLUMNS;j++){
						switch(j){

								#region support
								// Total Univers
							case FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX:
								if(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==3){									
									totalUniversValue+=double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX].ToString());
								}							
								break;
								// Total Famille
							case FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX:
								if(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==3){
									totalSectorValue+=double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX].ToString());
								}
								break;
								// Total Marché
							case FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX:
								if(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==3){
									totalMarketValue+=double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX].ToString());
								}
								break;
								#endregion

							case FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX:
								if(tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]!=null 
									&& tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_MEDIA_COLUMN_INDEX]!=null
									&&  tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX]!=null
									&& MEDIA_LEVEL_NUMBER==3){
									oldValueRefCompetitor=(double)listSeriesMediaRefCompetitor[tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()];
									listSeriesMediaRefCompetitor[tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]=oldValueRefCompetitor+double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX].ToString());
								}
								else if(tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]!=null 
									&& tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_CATEGORY_COLUMN_INDEX]!=null
									&&  tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX]!=null
									&& tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_MEDIA_COLUMN_INDEX]==null
									&& MEDIA_LEVEL_NUMBER==2){
									oldValueRefCompetitor=(double)listSeriesMediaRefCompetitor[tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()];
									listSeriesMediaRefCompetitor[tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]=oldValueRefCompetitor+double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX].ToString());
								}
								break;


								#region Category

								// Total Univers	
							case FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX:
								if(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==2){
									totalUniversValue+=double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX].ToString());
								}
								break;
								// Total Famille
							case FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX:
								if(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==2){
									totalSectorValue+=double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX].ToString());
								}
								break;
								// Total Marché	
							case FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX:
						
								if(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==2){
									totalMarketValue+=double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX].ToString());
								}

								break;
							


								#endregion														


							default:
								break;
						}
					}
				}
			}

			#region PluriMedia
			if(MEDIA_LEVEL_NUMBER==1){
				for(i=0;i<tab.GetLongLength(0);i++){			
					for( j=0;j<FrameWorkConstantes.Results.MediaStrategy.NB_MAX_COLUMNS;j++){
						switch(j){			
						
								// Total Univers
							case FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_INVEST_COLUMN_INDEX:
								if(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_INVEST_COLUMN_INDEX]!=null ){
									totalUniversValue+=double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_INVEST_COLUMN_INDEX].ToString());
								}							
								break;
								// Total Famille
							case FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_INVEST_COLUMN_INDEX:
								if(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_INVEST_COLUMN_INDEX]!=null ){
									totalSectorValue+=double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_INVEST_COLUMN_INDEX].ToString());
								}
								break;
								// Total Marché
							case FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_INVEST_COLUMN_INDEX:
								if(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_INVEST_COLUMN_INDEX]!=null){
									totalMarketValue+=double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_INVEST_COLUMN_INDEX].ToString());
								}
								break;

							case FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX:
								if(tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]!=null 
									&&  tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX]!=null
									&&  tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_VEHICLE_COLUMN_INDEX]!=null){
									oldValueRefCompetitor=(double)listSeriesMediaRefCompetitor[tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()];
									listSeriesMediaRefCompetitor[tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]=oldValueRefCompetitor+double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX].ToString());
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
			
			// Parcours de l'arbre pour la création des histogrammes
			for( i=1;i<tab.GetLongLength(0);i++){			
				for( j=0;j<FrameWorkConstantes.Results.MediaStrategy.NB_MAX_COLUMNS;j++){
					switch(j){

						#region Media
						//Univers niveau Media 
						case FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX:
							if(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==3){								
								
								if(totalUniversValue!=0){
									elementValue=double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX].ToString())/totalUniversValue*100;
									DataRow row=tableUnivers.NewRow();
									row["Name"]=tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_MEDIA_COLUMN_INDEX].ToString();
									row["Position"]=double.Parse(elementValue.ToString("0.00"));
									tableUnivers.Rows.Add(row);
								}
									
								j=j+6;
							}
														
							break;
						// Famille niveau media
						case FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX:
							if(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==3){
								
								if(totalSectorValue!=0){
									elementValue=double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX].ToString())/totalSectorValue*100;
									DataRow row1=tableSectorMarket.NewRow();
									row1["Name"]=tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_MEDIA_COLUMN_INDEX].ToString();
									row1["Position"]=double.Parse(elementValue.ToString("0.00"));
									tableSectorMarket.Rows.Add(row1);

								}
								j=j+5;
							}							
							break;

						// Marché niveau Media
						case FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX:
							if(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==3){
									
									
								if(totalMarketValue!=0){							
									elementValue=double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX].ToString())/totalMarketValue*100;
									DataRow row1=tableSectorMarket.NewRow();
									row1["Name"]=tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_MEDIA_COLUMN_INDEX].ToString();
									row1["Position"]=double.Parse(elementValue.ToString("0.00"));
									tableSectorMarket.Rows.Add(row1);
								}
								j=j+4;
							}
							break;
						#endregion

						case FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX:
							if(tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]!=null 
								&& tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_MEDIA_COLUMN_INDEX]!=null
								&&  tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX]!=null
								&& MEDIA_LEVEL_NUMBER==3){
								
								if((double)listSeriesMediaRefCompetitor[tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]!=0){
									elementValue=double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX].ToString())/(double)listSeriesMediaRefCompetitor[tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]*100;
									DataRow row1=((DataTable)listTableRefCompetitor[tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]).NewRow();
									row1["Name"]=tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_MEDIA_COLUMN_INDEX].ToString();
									row1["Position"]=double.Parse(elementValue.ToString("0.00"));
									((DataTable)listTableRefCompetitor[tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]).Rows.Add(row1);
								}
								
								j=j+12;
							}
							else	if(tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]!=null 
								&& tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_CATEGORY_COLUMN_INDEX]!=null
								&& tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_MEDIA_COLUMN_INDEX]==null
								//&& tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_VEHICLE_COLUMN_INDEX]==null
								&&  tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX]!=null
								&& MEDIA_LEVEL_NUMBER==2){
								
								if((double)listSeriesMediaRefCompetitor[tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]!=0){
									elementValue=double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX].ToString())/(double)listSeriesMediaRefCompetitor[tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]*100;
									DataRow row1=((DataTable)listTableRefCompetitor[tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]).NewRow();
									row1["Name"]=tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_CATEGORY_COLUMN_INDEX].ToString();
									row1["Position"]=double.Parse(elementValue.ToString("0.00"));
									((DataTable)listTableRefCompetitor[tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]).Rows.Add(row1);
								}
								
								j=j+12;
							}
							else if(tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]!=null 
								&&  tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX]!=null
								&&  tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_VEHICLE_COLUMN_INDEX]!=null
								&& MEDIA_LEVEL_NUMBER==1){
							
								if((double)listSeriesMediaRefCompetitor[tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]!=0){
									elementValue=double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX].ToString())/(double)listSeriesMediaRefCompetitor[tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]*100;
									DataRow row1=((DataTable)listTableRefCompetitor[tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]).NewRow();
									row1["Name"]=tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_VEHICLE_COLUMN_INDEX].ToString();
									row1["Position"]=double.Parse(elementValue.ToString("0.00"));
									((DataTable)listTableRefCompetitor[tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]).Rows.Add(row1);

								}
							
								j=j+12;							
							}
							break;							
						
						#region Categorie 
						// Univers niveau categorie
						case FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX:
							if(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==2){								
								
								if(totalUniversValue!=0){
									elementValue=double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX].ToString())/totalUniversValue*100;
									DataRow row=tableUnivers.NewRow();
									row["Name"]=tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_CATEGORY_COLUMN_INDEX].ToString();
									row["Position"]=double.Parse(elementValue.ToString("0.00"));
									tableUnivers.Rows.Add(row);
								}
								
							}							
							break;
						// Famille niveau category
						case FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX:
							if(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==2){
								if(totalSectorValue!=0){
									elementValue=double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX].ToString())/totalSectorValue*100;
									DataRow row1=tableSectorMarket.NewRow();
									row1["Name"]=tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_CATEGORY_COLUMN_INDEX].ToString();
									row1["Position"]=double.Parse(elementValue.ToString("0.00"));
									tableSectorMarket.Rows.Add(row1);
								}
								
							}							
							break;
						
						// Marché niveau categorie
						case FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX:
							if(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==2){
								
								if(totalMarketValue!=0){
									elementValue=double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX].ToString())/totalMarketValue*100;
									DataRow row1=tableSectorMarket.NewRow();
									row1["Name"]=tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_CATEGORY_COLUMN_INDEX].ToString();
									row1["Position"]=double.Parse(elementValue.ToString("0.00"));
									tableSectorMarket.Rows.Add(row1);
								}
							}				

							break;
						
						#endregion

						#region PluriMedia
						//Univers  
						case FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX:
							if(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX]!=null && i>1){								
								if(totalUniversValue!=0){	
									elementValue=double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX].ToString())/totalUniversValue*100;
									DataRow row=tableUnivers.NewRow();
									row["Name"]=tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_VEHICLE_COLUMN_INDEX].ToString();
									row["Position"]=double.Parse(elementValue.ToString("0.00"));
									tableUnivers.Rows.Add(row);	
								}
							}														
							break;
							// Famille 
						case FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX:
							if(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX]!=null && i>1){
								if(totalSectorValue!=0){
									elementValue=double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX].ToString())/totalSectorValue*100;
									DataRow row1=tableSectorMarket.NewRow();
									row1["Name"]=tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_VEHICLE_COLUMN_INDEX].ToString();
									row1["Position"]=double.Parse(elementValue.ToString("0.00"));
									tableSectorMarket.Rows.Add(row1);								
								}
							}							
							break;

							// Marché 
						case FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX:
							if(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX]!=null && i>1){
								if(totalMarketValue!=0){
									elementValue=double.Parse(tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX].ToString())/totalMarketValue*100;
									DataRow row1=tableSectorMarket.NewRow();
									row1["Name"]=tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_VEHICLE_COLUMN_INDEX].ToString();
									row1["Position"]=double.Parse(elementValue.ToString("0.00"));
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
		string strSort = "Position  DESC";
		DataRow[] foundRows=null;
		foundRows=tableUnivers.Select("",strSort);
		
		

		DataRow[] foundRowsSectorMarket=null;
		foundRowsSectorMarket=tableSectorMarket.Select("",strSort);
			double[]  yValues=new double[foundRows.Length];
			string[]    xValues=new string[foundRows.Length];
			#region ancienne version (modifié par Dédé)
//			double[] yValuesSectorMarket=new double[foundRows.Length];
//			string[]  xValuesSectorMarket=new string[foundRows.Length];
			#endregion
			double[] yValuesSectorMarket=new double[foundRowsSectorMarket.Length];
			string[]  xValuesSectorMarket=new string[foundRowsSectorMarket.Length];
			double otherUniversValue=0;
			double otherSectorMarketValue=0;

			if( MEDIA_LEVEL_NUMBER!=1 ){
				for(i=0;i<5 && i<foundRows.Length;i++){
					xValues[i]=(string)foundRows[i]["Name"];
					yValues[i]=double.Parse(foundRows[i]["Position"].ToString());
					otherUniversValue+=double.Parse(foundRows[i]["Position"].ToString());
				}
				if(foundRows.Length>NBRE_MEDIA){				
					xValues[i]=GestionWeb.GetWebWord(647,webSession.SiteLanguage);
					yValues[i]=100-otherUniversValue;			
				}

				for(i=0;i<5 && i<foundRowsSectorMarket.Length ;i++){
					xValuesSectorMarket[i]=(string)foundRowsSectorMarket[i]["Name"];
					yValuesSectorMarket[i]=double.Parse(foundRowsSectorMarket[i]["Position"].ToString());
					otherSectorMarketValue+=double.Parse(foundRowsSectorMarket[i]["Position"].ToString());
				}
				#region ancienne version (modifié par Dédé)
//				if(foundRows.Length>NBRE_MEDIA){
				#endregion
				if(foundRowsSectorMarket.Length>NBRE_MEDIA){
					xValuesSectorMarket[i]=GestionWeb.GetWebWord(647,webSession.SiteLanguage);
					yValuesSectorMarket[i]=100-otherSectorMarketValue;			
				}
			}
			// Cas PluriMedia
			else {
				for(i=0; i<foundRows.Length;i++){
					xValues[i]=(string)foundRows[i]["Name"];
					yValues[i]=double.Parse(foundRows[i]["Position"].ToString());
					otherUniversValue+=double.Parse(foundRows[i]["Position"].ToString());
				}				

				for(i=0; i<foundRowsSectorMarket.Length ;i++){
					xValuesSectorMarket[i]=(string)foundRowsSectorMarket[i]["Name"];
					yValuesSectorMarket[i]=double.Parse(foundRowsSectorMarket[i]["Position"].ToString());
					otherSectorMarketValue+=double.Parse(foundRowsSectorMarket[i]["Position"].ToString());
				}			
			}
				
			double[]  yVal=new double[foundRows.Length];
			string[]    xVal=new string[foundRows.Length];
			double otherCompetitorRefValue=0;
			j=2;

			foreach(string name in listSeriesMedia.Keys){
				
				if(name==GestionWeb.GetWebWord(1780,webSession.SiteLanguage)){ 
					if(xValues!=null && xValues.Length>0 && xValues[0]!=null)
					((Series)listSeriesMedia[GestionWeb.GetWebWord(1780,webSession.SiteLanguage)]).Points.DataBindXY(xValues,yValues);
				}
				else if(webSession.ComparaisonCriterion==TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal && name==GestionWeb.GetWebWord(1189,webSession.SiteLanguage)){
					if(xValuesSectorMarket!=null && xValuesSectorMarket.Length>0 && xValuesSectorMarket[0]!=null)
					((Series)listSeriesMedia[GestionWeb.GetWebWord(1189,webSession.SiteLanguage)]).Points.DataBindXY(xValuesSectorMarket,yValuesSectorMarket);
				}else if(name==GestionWeb.GetWebWord(1316,webSession.SiteLanguage)){
					if(xValuesSectorMarket!=null  && xValuesSectorMarket.Length>0 && xValuesSectorMarket[0]!=null)
					((Series)listSeriesMedia[GestionWeb.GetWebWord(1316,webSession.SiteLanguage)]).Points.DataBindXY(xValuesSectorMarket,yValuesSectorMarket);
				}
				else{
					DataRow[] foundRowsCompetitorRef=null;
					foundRowsCompetitorRef=((DataTable)listTableRefCompetitor[name]).Select("",strSort);
					otherCompetitorRefValue=0;
					
					yVal=new double[foundRowsCompetitorRef.Length];
					xVal=new string[foundRowsCompetitorRef.Length];
					if( MEDIA_LEVEL_NUMBER!=1 ){
						for(i=0;i<foundRowsCompetitorRef.Length && i<NBRE_MEDIA ;i++){
							
							
							xVal[i]=(string)foundRowsCompetitorRef[i]["Name"];
							yVal[i]=double.Parse(foundRowsCompetitorRef[i]["Position"].ToString());
							
							otherCompetitorRefValue+=double.Parse(foundRowsCompetitorRef[i]["Position"].ToString());
				
						}
						if(foundRowsCompetitorRef.Length>NBRE_MEDIA){				
							xVal[i]="Autres";
							yVal[i]=100-otherCompetitorRefValue;			
						}
					}
					// PluriMedia
					else{
						for(i=0;i<foundRowsCompetitorRef.Length ;i++){
							xVal[i]=(string)foundRowsCompetitorRef[i]["Name"];
							yVal[i]=double.Parse(foundRowsCompetitorRef[i]["Position"].ToString());
							
						}
					}
					if(xVal.Length>0 && xVal[0]!=null)
						((Series)listSeriesMedia[name]).Points.DataBindXY(xVal,yVal);


					listSeriesName.Add(j,name);
					j++;
				}
			
			}
			//float yPosition=0.0F;
			#region Affichage des graphiques

			i=0;
			decimal dec = 0;
			int pCount = 0;
			int nbLeftElements = 0, nbRigthElements = 1;
			if (listSeriesMedia != null && listSeriesMedia.Count > 0) {
				for (int pc = 0; pc < listSeriesMedia.Count; pc++) {
					if (((Series)listSeriesMedia[(string)listSeriesName[pc]]).Points.Count > 0)
						pCount++;
				}
			}
			dec = pCount / (decimal)2;
			

			for(j=0;j<listSeriesMedia.Count;j++){
				if(((Series)listSeriesMedia[(string)listSeriesName[j]]).Points.Count>0){
					
										
					#region Type de Graphique
					((Series)listSeriesMedia[(string)listSeriesName[j]]).Type= SeriesChartType.Pie;
					#endregion
				
					#region Définition des couleurs
					for(int k=0;k<6&&k<((Series)listSeriesMedia[(string)listSeriesName[j]]).Points.Count;k++){
						((Series)listSeriesMedia[(string)listSeriesName[j]]).Points[k].Color=pieColors[k];
					}
					#endregion
				
					#region Légende
					((Series)listSeriesMedia[(string)listSeriesName[j]])["LabelStyle"]="Outside";
					((Series)listSeriesMedia[(string)listSeriesName[j]]).LegendToolTip = "#PERCENT";
					((Series)listSeriesMedia[(string)listSeriesName[j]]).ToolTip = " "+(string)listSeriesName[j]+" \n #VALX : #PERCENT";
					((Series)listSeriesMedia[(string)listSeriesName[j]])["PieLineColor"]="Black";
					((Series)listSeriesMedia[(string)listSeriesName[j]]).Font= new Font("Arial", (float)7);
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
					this.Titles[i].DockInsideChartArea=true;
					this.Titles[i].Position.Auto = true;
					//this.Titles[i].Position.X = (i % 2 == 0) ? 45 : 90;//this.Titles[i].Position.X=45;
					//this.Titles[i].Position.Y = (i % 2 == 0) ? (3 + (((96 / (float)Math.Ceiling(dec)) * nbLeftElements) + 1)) : (3 + (((96 / (float)Math.Ceiling(dec)) * (nbRigthElements - 1)) + 1));
					this.Titles[i].Font = (pCount < 15) ? new Font("Arial", (float)12) : new Font("Arial", (float)8); //this.Titles[i].Font=new Font("Arial", (float)13);
					this.Titles[i].Color=Color.FromArgb(100,72,131);
				    this.Titles[i].DockToChartArea=chartArea2.Name;					
					#endregion

					#region Type image
					((Series)listSeriesMedia[(string)listSeriesName[j]]).SmartLabels.Enabled = true;
					((Series)listSeriesMedia[(string)listSeriesName[j]]).Label = "#VALX \n #PERCENT";
					((Series)listSeriesMedia[(string)listSeriesName[j]])["3DLabelLineSize"] = "50"; //((Series)listSeriesMedia[(string)listSeriesName[j]])["3DLabelLineSize"] = "50"; 
					if(pCount>4)((Series)listSeriesMedia[(string)listSeriesName[j]])["MinimumRelativePieSize"] = "50";//70
					else ((Series)listSeriesMedia[(string)listSeriesName[j]])["MinimumRelativePieSize"] = "45";//45	
						
					#endregion

					if (j == 0 && !typeFlash) {
						chartArea2.BackImage = "/Images/common/logo_Tns_2.gif";//WebConstantes.Images.LOGO_TNS;
						chartArea2.BackImageAlign = ChartImageAlign.TopLeft;
						chartArea2.BackImageMode = ChartImageWrapMode.Unscaled;							
					}
					#region Positionnement du graphique



					if (pCount > 4) {
						chartArea2.Position.Auto = false;
						chartArea2.Position.X = (i % 2 == 0) ? 2 : 52;
						chartArea2.Position.Y = (i % 2 == 0) ? (3 + (((96 / (float)Math.Ceiling(dec)) * nbLeftElements) + 1)) : (3 + (((96 / (float)Math.Ceiling(dec)) * (nbRigthElements - 1)) + 1));
						chartArea2.Position.Width = (pCount > 15) ? 47 : 43;
						chartArea2.Position.Height = ((96 / (float)Math.Ceiling(dec)) - 1);
						chartArea2.Area3DStyle.PointDepth = (pCount > 10) ?  10 : 40;

					}
					else {
						chartArea2.Position.X = 4;
						chartArea2.Position.Y = 3 + (((96 / pCount) * i) + 1);
						chartArea2.Position.Width = 80;
						chartArea2.Position.Height = (96 / pCount) - 1;
						chartArea2.Area3DStyle.PointDepth = 45;

					}
					chartArea2.Area3DStyle.Enable3D = true;
					chartArea2.Area3DStyle.XAngle = 20;


					#endregion


					if (i % 2 == 0) nbLeftElements++;
					else nbRigthElements++;
					i++;
					
					
					//Ajout des dans la série

		

					this.Series.Add(((Series)listSeriesMedia[(string)listSeriesName[j]]));	
					

					//yPosition+=chartArea2.Position.Height;
				}
			}

			#region Dimensionnement de l'image
			////// Taille d'un graphique * Nombre de graphique
			if (pCount > 4) {
				this.Height = (pCount > 12) ? ((pCount > 20) ? new Unit("3000") : new Unit("2500")) : new Unit("1100");
				this.Width = (pCount > 12) ? new Unit("1500") : new Unit("1100");
			}	
					
			#endregion

			#region Copyright & Logo
			if (!typeFlash) {				
				Title title = new Title("" + GestionWeb.GetWebWord(2266, webSession.SiteLanguage) + "");
				title.Font = new Font("Arial", (float)8);
				title.DockInsideChartArea = false;
				title.Docking = Docking.Bottom;
				this.Titles.Add(title);
			}
			#endregion
			#endregion
	
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

		#region Add Logo
		/// <summary>
		/// Ajoute en-tête
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="isNotFlashType">type image</param>
		private void AddHeader(WebSession webSession, bool isNotFlashType) {
			if (isNotFlashType) {
				Title title = new Title("" + GestionWeb.GetWebWord(2266, webSession.SiteLanguage) + "");
				title.Font = new Font("Arial", (float)8);
				title.DockInsideChartArea = false;
				title.Docking = Docking.Bottom;
				this.Titles.Add(title);

				this.BackImage = WebConstantes.Images.LOGO_TNS_2;
				this.BackImageAlign = ChartImageAlign.TopLeft;
				this.BackImageMode = ChartImageWrapMode.Unscaled;
				
			}
		}
		#endregion

	}
}
