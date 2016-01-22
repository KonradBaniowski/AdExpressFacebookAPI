#region Informations
// Auteur: A.DADOUCH
// Date de création: 05/08/2005 
// Modified by: K.Shehzad
// Date of Modification: 12/08/2005  (changing the Exception usage)

#endregion

using System;
using System.Data;
using System.Collections;
using System.Drawing; 
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Translation;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using TNS.AdExpress.Web.Core.Sessions;
using CustomerCst=TNS.AdExpress.Constantes.Customer;
using Dundas.Charting.WebControl;
using APPMUIs = TNS.AdExpress.Web.UI.Results.APPM;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TblFormatCst = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;

namespace TNS.AdExpress.Web.UI.Results.APPM{ 
	/// <summary>
	/// Classe pour construire les graphiques des périodicité plan
	/// </summary>
	public class AnalyseFamilyInterestPlanChartUI{

		#region constructeur
		/// <summary>
		/// constructeur
		/// </summary>
		public AnalyseFamilyInterestPlanChartUI(){		
		}
		#endregion

		/// <summary>
		/// Graphiques Famille d'interêts
		/// </summary>
		/// <param name="appmChart">Objet Webcontrol</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="dataSource">dataSource pour la creation de Dataset </param>
		/// <param name="dateBegin">date du debut</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="baseTarget">cible de base</param>
		/// <param name="additionalTarget">cible seléctionnée</param>
		/// <param name="typeFlash">Si flash true</param>
        public static void InterestFamilyPlanChart(APPMChartUI appmChart, WebSession webSession, TNS.FrameWork.DB.Common.IDataSource dataSource, int dateBegin, int dateEnd, Int64 baseTarget, Int64 additionalTarget, bool typeFlash)
        {
		
			#region variable
			DataTable InterestFamilyPlanData;
			ChartArea chartArea=new ChartArea();
			//Séries de valeurs pour chaque tranche du graphique
			double[]  yUnitValues = null;
			string[]  xUnitValues  = null;
			double[]  yUnitValuesSelected = null;
			string[]  xUnitValuesSelected  = null;
			double maxScale=0;

			#endregion

			#region Constantes
			//couleurs des tranches du graphique
			#region Couleur du camembert
			Color[] pieColors={
								  Color.FromArgb(100,72,131),
								  Color.FromArgb(177,163,193),
								  Color.FromArgb(208,200,218),
								  Color.FromArgb(255,153,204),
								  Color.FromArgb(204,255,255),
								  Color.FromArgb(204,204,255),
								  Color.FromArgb(255,204,255),
								  Color.FromArgb(255,255,204),
								  Color.FromArgb(204,255,204),
								  Color.FromArgb(255,204,204),
								  Color.FromArgb(204,102,153),
								  Color.FromArgb(153,255,255),
								  Color.FromArgb(153,153,255),
								  Color.FromArgb(255,153,255),
								  Color.FromArgb(255,255,153),
								  Color.FromArgb(153,255,153),
								  Color.FromArgb(255,153,153),
								  Color.FromArgb(153,51,102),
								  Color.FromArgb(102,255,255),
								  Color.FromArgb(102,102,255),
								  Color.FromArgb(255,102,255),
								  Color.FromArgb(255,255,102),
								  Color.FromArgb(102,255,102),
								  Color.FromArgb(255,102,102),
								  Color.FromArgb(102,0,51),
								  Color.FromArgb(51,255,255),
								  Color.FromArgb(51,51,255),
								  Color.FromArgb(255,51,255),
								  Color.FromArgb(255,255,51),
								  Color.FromArgb(51,255,51),
								  Color.FromArgb(255,51,51)
							  };
			#endregion

			Color[] barColors={ 
								  Color.FromArgb(255,223,222),
								   
			};
			#endregion

			try {
			
				#region Paramétrage des dates
				//				int dateBegin = int.Parse(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
				//				int dateEnd = int.Parse(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
				#endregion

				#region targets
				//base target
                Int64 idBaseTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmBaseTargetAccess));
				//additional target
                Int64 idAdditionalTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmTargetAccess));									
				#endregion

				#region Wave
                Int64 idWave=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMWave,CustomerCst.Right.type.aepmWaveAccess));									
				#endregion

				#region Données
				//				try{
				InterestFamilyPlanData=TNS.AdExpress.Web.Rules.Results.APPM.AnalyseFamilyInterestPlanRules.InterestFamilyPlan( webSession, dataSource,idWave,dateBegin, dateEnd,idBaseTarget,idAdditionalTarget);
				//					if(InterestFamilyPlanData==null || InterestFamilyPlanData.Rows.Count==0 )throw(new WebExceptions.InterestFamilyPlanUIException());
				//					}
				//				catch(System.Exception){
				//								//	throw(new WebExceptions.APPMBusinessFacadeException("pas de données+: "+ee.Message));
				//				return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage));
				//				}
				#endregion				
				
				#region Création et définition du graphique pour la cible de base
				if(InterestFamilyPlanData!=null && InterestFamilyPlanData.Rows.Count>0)
					getSeriesDataBase(InterestFamilyPlanData,ref xUnitValues,ref yUnitValues);				
				//Création du graphique	des uités(euros, grp, insertion, page) cible de base
				ChartArea chartAreaUnit=null;
				Series serieInterestFamily=new Series();

				if(InterestFamilyPlanData!=null && InterestFamilyPlanData.Rows.Count>0){																	
					//Conteneur graphique pour unité de cible de base
					chartAreaUnit=new ChartArea();
					#region dimension 1er Camembert
					//Alignement
					if (webSession.Unit == WebConstantes.CustomerSessions.Unit.grp){
						chartAreaUnit.AlignOrientation = AreaAlignOrientation.Vertical;
						chartAreaUnit.Position.X=2;
						if(InterestFamilyPlanData.Rows.Count<=8){
							chartAreaUnit.Position.Y=4;
							chartAreaUnit.Position.Height=30;
						}
						else{
							chartAreaUnit.Position.Y=4;
							chartAreaUnit.Position.Height=20;
						}
						chartAreaUnit.Position.Width=96;
					}else{
						chartAreaUnit.AlignOrientation = AreaAlignOrientation.Vertical;
						chartAreaUnit.Position.X=2;
						if(InterestFamilyPlanData.Rows.Count<=8){
							chartAreaUnit.Position.Y=4;
							chartAreaUnit.Position.Height=40;
						}
						else{
							chartAreaUnit.Position.Y=4;
							chartAreaUnit.Position.Height=30;
						}
						chartAreaUnit.Position.Width=96;
					}
					#endregion
					appmChart.ChartAreas.Add(chartAreaUnit);
					//Charger les séries de valeurs 
					string unitName="";
					string chartAreaName="";
					string chartAreaAdditionalName="";
					string chartAreaCgrpName="";

					#region sélection par rappot à l'unité choisit
                    unitName = GestionWeb.GetWebWord(webSession.GetSelectedUnit().WebTextId, webSession.SiteLanguage);
					#region Titres du graphiques
					//Titre graphique cible de base
					if (webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
						chartAreaName+=GestionWeb.GetWebWord(1736,webSession.SiteLanguage);
						chartAreaName+=" ";
						chartAreaName+= unitName ;
						chartAreaName+=" ("+InterestFamilyPlanData.Rows[0]["baseTarget"]+") " ;
						chartAreaName+=GestionWeb.GetWebWord(1737,webSession.SiteLanguage);
						//Titre graphique cible selectionnée
						chartAreaAdditionalName+=GestionWeb.GetWebWord(1736,webSession.SiteLanguage);
						chartAreaAdditionalName+=" ";
						chartAreaAdditionalName+=unitName ;
						chartAreaAdditionalName+=" ("+InterestFamilyPlanData.Rows[0]["additionalTarget"]+") " ;
						chartAreaAdditionalName+=GestionWeb.GetWebWord(1737,webSession.SiteLanguage);
					}else{
						chartAreaName+=GestionWeb.GetWebWord(1736,webSession.SiteLanguage);
						chartAreaName+=" ";
						chartAreaName+= unitName ;			
						chartAreaName+=" "+GestionWeb.GetWebWord(1737,webSession.SiteLanguage);
					}
					//Titre graphique du CGRP
					chartAreaCgrpName+=GestionWeb.GetWebWord(1685,webSession.SiteLanguage);
					chartAreaCgrpName+=" "+GestionWeb.GetWebWord(1737,webSession.SiteLanguage)+" :"; 
					chartAreaCgrpName+=InterestFamilyPlanData.Rows[0]["baseTarget"];
					chartAreaCgrpName+=" "+GestionWeb.GetWebWord(1739,webSession.SiteLanguage)+" "; 
					chartAreaCgrpName+=InterestFamilyPlanData.Rows[0]["additionalTarget"] ;
					#endregion
					#endregion

					serieInterestFamily=setSeriesInterestFamily(InterestFamilyPlanData,chartAreaUnit,serieInterestFamily,xUnitValues,yUnitValues,pieColors,chartAreaName,typeFlash);												
					#endregion									
					serieInterestFamily.ShowInLegend=false;
					#region legend chart Area
					//					ChartArea chartAreaLegend=new ChartArea();
					//					chartAreaLegend.Name="legendArea";
					//					//Alignement
					//					chartAreaLegend.AlignOrientation = AreaAlignOrientation.Vertical;
					//					chartAreaLegend.Position.X=5;
					//					chartAreaLegend.Position.Y=30;
					//					chartAreaLegend.Position.Width=80;
					//					chartAreaLegend.Position.Height=4;	
					//					appmChart.ChartAreas.Add(chartAreaLegend);
					//					appmChart.Legends["Default"].DockToChartArea = chartAreaLegend.Name;
					//					appmChart.Legends["Default"].InsideChartArea = "legendArea";
					//					appmChart.Legends["Default"].Enabled =false;
					//					appmChart.Legends["Default"].LegendStyle = LegendStyle.Table;
					//					appmChart.Legends["Default"].Docking = LegendDocking.Bottom;
					//					appmChart.Legends["Default"].Alignment = StringAlignment.Center;
	
					#endregion

					#region Création et définition du graphique pour la cible selectionnée
					getSeriesDataAdditional(InterestFamilyPlanData,ref xUnitValues,ref yUnitValues);
					//Création du graphique	pour unité
					ChartArea chartAreaUnitadditional=null;
					Series serieInterestFamilyadditional=new Series();	
					//Conteneur graphique pour unité
					chartAreaUnitadditional=new ChartArea();

					#region legend2
					Legend secondLegend = new Legend("Second");
					appmChart.Legends.Add(secondLegend);
					serieInterestFamilyadditional.Legend = "Second";
					secondLegend.DockToChartArea = chartAreaUnitadditional.Name;
					secondLegend.DockInsideChartArea = true;	
					secondLegend.BorderWidth=2;
					secondLegend.Enabled=false;
					#endregion

					//Alignement
					if (webSession.Unit == WebConstantes.CustomerSessions.Unit.grp){
						chartAreaUnitadditional.AlignOrientation = AreaAlignOrientation.Vertical;
						chartAreaUnitadditional.Position.X=2;
						if(InterestFamilyPlanData.Rows.Count<=8){
							chartAreaUnitadditional.Position.Y=37;
							chartAreaUnitadditional.Position.Height=30;
						}
						else{
							chartAreaUnitadditional.Position.Y=27;
							chartAreaUnitadditional.Position.Height=20;
						}
										
						chartAreaUnitadditional.Position.Width=96;
						appmChart.ChartAreas.Add(chartAreaUnitadditional);
						//Charger les séries de valeurs 
						serieInterestFamilyadditional=setSeriesInterestFamily(InterestFamilyPlanData,chartAreaUnitadditional,serieInterestFamilyadditional,xUnitValues,yUnitValues,pieColors,chartAreaAdditionalName,typeFlash);												
					}
						#endregion
 
					#region Création et définition du graphique pour CGRP
					getSeriesDataCgrp(InterestFamilyPlanData,ref xUnitValues,ref yUnitValues,ref xUnitValuesSelected,ref yUnitValuesSelected,ref maxScale);
					//Création du graphique	pour unité
					ChartArea chartAreaCgrp=null;
					Series serieInterestFamilyCgrpBase=new Series();
					Series serieInterestFamilyCgrpSelected=new Series();
					
					//legende du graphe bar
					ChartArea chartAreaCgrpLegend=new ChartArea();
					chartAreaCgrpLegend.Name="legendCgrpArea";
					//Allignement
					if (webSession.Unit == WebConstantes.CustomerSessions.Unit.grp){
						chartAreaCgrpLegend.AlignOrientation = AreaAlignOrientation.Vertical;
						chartAreaCgrpLegend.Position.X=5;
						if(InterestFamilyPlanData.Rows.Count<=8){
							chartAreaCgrpLegend.Position.Y=97;
							chartAreaCgrpLegend.Position.Height=2;
						}
						else{
							chartAreaCgrpLegend.Position.Y=96;
							chartAreaCgrpLegend.Position.Height=2;
						}
						chartAreaCgrpLegend.Position.Width=80;
					}else{
						chartAreaCgrpLegend.AlignOrientation = AreaAlignOrientation.Vertical;
						chartAreaCgrpLegend.Position.X=5;
						if(InterestFamilyPlanData.Rows.Count<=8){
							chartAreaCgrpLegend.Position.Y=96;
							chartAreaCgrpLegend.Position.Height=2;
						}
						else{
							chartAreaCgrpLegend.Position.Y=92;
							chartAreaCgrpLegend.Position.Height=4;
						}
						chartAreaCgrpLegend.Position.Width=80;
					}
					appmChart.ChartAreas.Add(chartAreaCgrpLegend);

					#region legend3
					serieInterestFamilyCgrpBase.ShowInLegend=false;
					Legend thirdLegend = new Legend("Third");
					LegendItem legendItemReference = new LegendItem();
					legendItemReference.Name = GestionWeb.GetWebWord(1685,webSession.SiteLanguage);
					legendItemReference.Name+= " ("+InterestFamilyPlanData.Rows[0]["baseTarget"]+") " ;
					legendItemReference.Style = LegendImageStyle.Rectangle;
					legendItemReference.ShadowOffset = 1;
					legendItemReference.Color =Color.FromArgb(255,215,215);
					thirdLegend.CustomItems.Add(legendItemReference);

					LegendItem legendItemReference2 = new LegendItem();
					legendItemReference2.Name = GestionWeb.GetWebWord(1685,webSession.SiteLanguage);
					legendItemReference2.Name+= " ("+InterestFamilyPlanData.Rows[0]["additionalTarget"]+") " ;
					legendItemReference2.Style = LegendImageStyle.Rectangle;
					legendItemReference2.ShadowOffset = 1;
					legendItemReference2.Color = Color.FromArgb(255,215,215);
					thirdLegend.CustomItems.Add(legendItemReference2);
					appmChart.Legends.Add(thirdLegend);
					thirdLegend.DockToChartArea = chartAreaCgrpLegend.Name;
					thirdLegend.Font=new Font("Arial", (float)8);
					thirdLegend.Enabled =true;
					thirdLegend.InsideChartArea = "legendCgrpArea";
					thirdLegend.LegendStyle = LegendStyle.Row;
					thirdLegend.Docking = LegendDocking.Bottom;
					thirdLegend.Alignment = StringAlignment.Center;
					#endregion

					#region legend4
					Legend fourthLegend = new Legend("Fourth");
					appmChart.Legends.Add(fourthLegend);					
					serieInterestFamilyCgrpSelected.Legend = "Fourth";
					fourthLegend.DockToChartArea = chartAreaCgrpLegend.Name;					
					//fourthLegend.DockInsideChartArea = true;	
					fourthLegend.InsideChartArea = "legendCgrpArea";
					legendItemReference.BorderWidth=1;
					legendItemReference.Color = Color.FromArgb(148,121,181);
					legendItemReference.Name=GestionWeb.GetWebWord(1685,webSession.SiteLanguage);
					legendItemReference.Name+= " ("+InterestFamilyPlanData.Rows[0]["baseTarget"]+") " ;
					fourthLegend.Enabled =false;
					fourthLegend.CustomItems.Add(legendItemReference);				
					fourthLegend.Font=new Font("Arial", (float)8);
					fourthLegend.LegendStyle = LegendStyle.Row;
					fourthLegend.Docking = LegendDocking.Bottom;
					fourthLegend.Alignment = StringAlignment.Center;					
					#endregion

					//Conteneur graphique pour unité
					chartAreaCgrp=new ChartArea();
					//Alignement
					#region dimension histogramme
					if (webSession.Unit == WebConstantes.CustomerSessions.Unit.grp){
						chartAreaCgrp.AlignOrientation = AreaAlignOrientation.Vertical;
						chartAreaCgrp.Position.X=2;
						if(InterestFamilyPlanData.Rows.Count<=8){
							chartAreaCgrp.Position.Y=68;	
							chartAreaCgrp.Position.Height=30;
						}
						else{
							chartAreaCgrp.Position.Y=48;	
							chartAreaCgrp.Position.Height=48;
						}
						chartAreaCgrp.Position.Width=80;
					}else{
						chartAreaCgrp.AlignOrientation = AreaAlignOrientation.Vertical;
						chartAreaCgrp.Position.X=2;
						if(InterestFamilyPlanData.Rows.Count<=8){
							chartAreaCgrp.Position.Y=48;	
							chartAreaCgrp.Position.Height=40;
						}
						else{
							chartAreaCgrp.Position.Y=40;	
							chartAreaCgrp.Position.Height=55;
						}
						chartAreaCgrp.Position.Width=80;
					}
					#endregion
					appmChart.ChartAreas.Add(chartAreaCgrp);
					//Charger les séries de valeurs 
					serieInterestFamilyCgrpBase=setSeriesBarInterestFamily(InterestFamilyPlanData,chartAreaCgrp,serieInterestFamilyCgrpBase,xUnitValues,yUnitValues, Color.FromArgb(148,121,181),chartAreaCgrpName,maxScale,typeFlash);												
					serieInterestFamilyCgrpSelected=setSeriesBarInterestFamily(InterestFamilyPlanData,chartAreaCgrp,serieInterestFamilyCgrpSelected,xUnitValuesSelected,yUnitValuesSelected,Color.FromArgb(255,215,215),chartAreaCgrpName,maxScale,typeFlash);												
					
					#endregion

					//initialisation du control
					if (webSession.Unit == WebConstantes.CustomerSessions.Unit.grp){
						InitializeComponent(InterestFamilyPlanData,appmChart,chartAreaUnit,chartAreaUnitadditional,chartAreaCgrp,typeFlash);

						if(InterestFamilyPlanData!=null && InterestFamilyPlanData.Rows.Count>0){
							appmChart.Series.Add(serieInterestFamily);
							appmChart.Series.Add(serieInterestFamilyadditional);
							appmChart.Series.Add(serieInterestFamilyCgrpBase);	
							appmChart.Series.Add(serieInterestFamilyCgrpSelected);	
						}	
					}else{
						InitializeComponent(InterestFamilyPlanData ,appmChart,chartAreaUnit,chartAreaCgrp,typeFlash);

						if(InterestFamilyPlanData!=null && InterestFamilyPlanData.Rows.Count>0){
							appmChart.Series.Add(serieInterestFamily);
							appmChart.Series.Add(serieInterestFamilyCgrpBase);	
							appmChart.Series.Add(serieInterestFamilyCgrpSelected);	
						}	
					}
				}

				

			}
			catch(System.Exception err){				
				throw(new WebExceptions.AnalyseFamilyInterestPlanChartUIException("Erreur dans l'affichage des graphiques des données des AnalyseFamilyInterestPlanChart ",err));
			}
		}


		#region méthodes privées
		/// <summary>
		/// Obtient les séries de valeurs des unités pour la cible de base à afficher graphiquement
		/// </summary>
		/// <param name="dt">table de données</param>
		/// <param name="xValues">libellés du graphique</param>
		/// <param name="yValues">valeurs pour graphique</param>
		private static void getSeriesDataBase(DataTable dt,ref string[] xValues,ref double[] yValues){
	
			#region Variable
			//string ventilationType="distributionBase";
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
		private static void getSeriesDataAdditional(DataTable dt,ref string[] xValues,ref double[] yValues){
	
			#region Variable
			//string ventilationType="distributionSelected";
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
		/// Obtient les séries de valeurs des unités pour la cible sélectionnée à afficher graphiquement
		/// </summary>
		/// <param name="dt">table de données</param>
		/// <param name="xValuesBase">libellés du graphique</param>
		/// <param name="yValuesBase">valeurs pour graphique</param>
		/// <param name="xValuesSelected">Liebllés sélectionnée</param>
		/// <param name="yValuesSelected">Valeur sélectionnée</param>
		/// <param name="maxScale">Echelle de l'axe X du graphe</param>
		private static void getSeriesDataCgrp(DataTable dt,ref string[] xValuesBase,ref double[] yValuesBase,ref string[] xValuesSelected,ref double[] yValuesSelected,ref double maxScale){
	
			#region Variable
			//string ventilationTypeBase="cgrpBase";
			//string ventilationTypeSelected="cgrpSelected";
			int x=0;
			int y=0;
			#endregion
			
			#region Les séries  cgrp
			xValuesBase = new string[dt.Rows.Count-2];				
			yValuesBase = new double[dt.Rows.Count-2];
			xValuesSelected = new string[dt.Rows.Count-2];				
			yValuesSelected= new double[dt.Rows.Count-2];
	
			for(int i=0; i<dt.Rows.Count-1;i++){
				if(dt.Rows[i]["cgrpBase"].ToString()==""){
					continue;
				}else{
					xValuesBase[x]=dt.Rows[i]["InterestFamily"].ToString();
					//yValuesBase[y]=double.Parse(dt.Rows[i]["cgrpDistributionBase"].ToString());
					yValuesBase[y]=double.Parse(dt.Rows[i]["cgrpBase"].ToString());
					xValuesSelected[x]=dt.Rows[i]["InterestFamily"].ToString();
					//yValuesSelected[y]=double.Parse(dt.Rows[i]["cgrpDistributionSelected"].ToString());
					yValuesSelected[y]=double.Parse(dt.Rows[i]["cgrpSelected"].ToString());
					x++;
					y++;
				}
			}
			for(int i=0;i<yValuesBase.Length;i++){					
				if(maxScale<yValuesBase[i])
					maxScale=yValuesBase[i];
			}

			for(int i=0;i<yValuesSelected.Length;i++){					
				if(maxScale<yValuesSelected[i])
					maxScale=yValuesSelected[i];
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
		/// <param name="typeFlash">sortie flash</param>
		/// <param name="chartAreaName">Nom du conteneur de l'image</param>
		/// <returns>séries de valeurs</returns>
		private static  Dundas.Charting.WebControl.Series setSeriesInterestFamily(DataTable dt ,ChartArea chartArea,Dundas.Charting.WebControl.Series series,string[] xValues,double[] yValues,Color[] barColors,string chartAreaName,bool typeFlash){
			#region  Création graphique
			if(xValues!=null && yValues!=null){
				
				#region Création et définition du graphique
				//Création du graphique							
				
				//Type de graphique
				series.Type=SeriesChartType.Pie;
				//Chart1.ImageType = ChartImageType.Png;
				series.SmartLabels.Enabled = true;				
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
		/// <param name="typeFlash">sortie flash</param>
		/// <returns>séries de valeurs</returns>
		private static  Dundas.Charting.WebControl.Series setSeriesBarInterestFamily(DataTable dt ,ChartArea chartArea,Dundas.Charting.WebControl.Series series,string[] xValues,double[] yValues,System.Drawing.Color barColor,string chartAreaName,double maxScale,bool typeFlash){
			#region  Création graphique
			if(xValues!=null && yValues!=null){
				
				#region Création et définition du graphique
				//Création du graphique							
				
				//Type de graphique
				series.Type= SeriesChartType.Bar;
				//series.SmartLabels.Enabled = true;	
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
		}


		/// <summary>
		/// Initialise les styles du webcontrol pour média radio et télé
		/// </summary>
		/// <param name="dt">tableau de données</param>
		/// <param name="appmChart">Objet Webcontrol</param>
		/// <param name="chartAreaUnit">conteneur de l'image répartition unité</param>
		/// <param name="chartAreaUnitadditional">conteneur de l'image répartition pour cible selectionnée</param>
		/// <param name="chartAreaCgrp">conteneur de l'image répartition pour Cgrp</param>
		/// <param name="typeFlash">sortie flash</param>
		private static void InitializeComponent(DataTable dt ,APPMChartUI appmChart,ChartArea chartAreaUnit,ChartArea chartAreaUnitadditional,ChartArea chartAreaCgrp,bool typeFlash) {			
			
			#region Animation Flash
			//Animation flash
			if(typeFlash ){
				appmChart.ImageType=ChartImageType.Flash;
				appmChart.AnimationTheme = AnimationTheme.GrowingTogether;
				appmChart.AnimationDuration =0.3;
				appmChart.RepeatAnimation = false;
			}
			else{
				appmChart.ImageType=ChartImageType.Jpeg;
			}
			#endregion

			#region Chart
			appmChart.Width=new Unit("900px");
						if(dt.Rows.Count<6){
							appmChart.Height=new Unit("1000px");
						}
						else if(dt.Rows.Count>=6 && dt.Rows.Count<16 ){
							appmChart.Height=new Unit("1200px");
						}
						 if(dt.Rows.Count>=16){
			appmChart.Height=new Unit("1500px");
						}
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
						if(dt.Rows.Count<=8){
							appmChart.Titles[1].Position.Y = 36;
						}else
							appmChart.Titles[1].Position.Y = 26;
			
			appmChart.Titles[1].Font=new Font("Arial", (float)10);
			appmChart.Titles[1].Color=Color.FromArgb(100,72,131);
			appmChart.Titles[1].DockToChartArea=chartAreaUnitadditional.Name;
	
			//titre unité pour CGRP

			appmChart.Titles.Add(chartAreaCgrp.Name);
			appmChart.Titles[2].DockInsideChartArea=true;
			appmChart.Titles[2].Position.Auto = false;
			appmChart.Titles[2].Position.X = 50;
						if(dt.Rows.Count<=8){
							appmChart.Titles[2].Position.Y = 67;
						}else
							appmChart.Titles[2].Position.Y = 48;
			
			appmChart.Titles[2].Font=new Font("Arial", (float)10);
			appmChart.Titles[2].Color=Color.FromArgb(100,72,131);
			appmChart.Titles[2].DockToChartArea=chartAreaCgrp.Name;
			#endregion
		}

		/// <summary>
		/// Initialise les styles du webcontrol pour média radio et télé
		/// </summary>
		/// <param name="dt">tableau de données</param>
		/// <param name="appmChart">Objet Webcontrol</param>
		/// <param name="chartAreaUnit">conteneur de l'image répartition unité</param>
		/// <param name="chartAreaCgrp">conteneur de l'image répartition pour Cgrp</param>
		/// <param name="typeFlash">sortie flash</param>
		private static void InitializeComponent(DataTable dt ,APPMChartUI appmChart, ChartArea chartAreaUnit,ChartArea chartAreaCgrp,bool typeFlash) {			
			#region Animation Flash
			//Animation flash
			if(typeFlash ){
				appmChart.ImageType=ChartImageType.Flash;
				appmChart.AnimationTheme = AnimationTheme.GrowingTogether;
				appmChart.AnimationDuration =0.3;
				appmChart.RepeatAnimation = false;
			}
			else{
				appmChart.ImageType=ChartImageType.Jpeg;				
			}
			#endregion

			#region Chart
			appmChart.Width=new Unit("900px");
			if(dt.Rows.Count<6){
				appmChart.Height=new Unit("900px");
			}
			else if(dt.Rows.Count>=6 && dt.Rows.Count<16 ){
				appmChart.Height=new Unit("1100px");
			}
			if(dt.Rows.Count>=16){
				appmChart.Height=new Unit("1400px");
			}
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
				
			//titre unité pour CGRP

			appmChart.Titles.Add(chartAreaCgrp.Name);
			appmChart.Titles[1].DockInsideChartArea=true;
			appmChart.Titles[1].Position.Auto = false;
			appmChart.Titles[1].Position.X = 50;
			if(dt.Rows.Count<=8){
				appmChart.Titles[1].Position.Y = 46;
			}else
				appmChart.Titles[1].Position.Y = 36;
			
			appmChart.Titles[1].Font=new Font("Arial", (float)10);
			appmChart.Titles[1].Color=Color.FromArgb(100,72,131);
			appmChart.Titles[1].DockToChartArea=chartAreaCgrp.Name;
			#endregion
		}

		#endregion

	}
}

