#region Informations
// Auteur: A.DADOUCH
// Date de cr�ation: 15/07/2005 
// Date of Modification: 12/08/2005
// By: K. Shehzad (try catch and putting function names in Capital)
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
	/// Classe pour construire les graphiques des p�riodicit� plan
	/// </summary>
	public class PeriodicityPlanChartUI{

		#region constructeur
		/// <summary>
		/// constructeur
		/// </summary>
		public PeriodicityPlanChartUI(){		
		}
		#endregion

		/// <summary>
		/// Graphiques p�riodicity plan
		/// </summary>
		/// <param name="appmChart">Objet Webcontrol</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="dataSource">dataSource pour la creation de Dataset </param>
		/// <param name="dateBegin">date du debut</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="baseTarget">cible de base</param>
		/// <param name="additionalTarget">cible sel�ctionn�e</param>
		/// <param name="typeFlash">Si flash true</param>
        public static void PeriodicityPlanChart(APPMChartUI appmChart, WebSession webSession, TNS.FrameWork.DB.Common.IDataSource dataSource, int dateBegin, int dateEnd, Int64 baseTarget, Int64 additionalTarget, bool typeFlash)
        {
		
			#region variable
			DataTable periodicityPlanData;
			ChartArea chartArea=new ChartArea();
			//S�ries de valeurs pour chaque tranche du graphique
			double[]  yUnitValues = null;
			string[]  xUnitValues  = null;
			double[]  yUnitValuesSelected = null;
			string[]  xUnitValuesSelected  = null;
			double maxScale=0;
			#endregion

			#region Constantes
			//couleurs des tranches du graphique
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

			Color[] barColors={ 
								  Color.FromArgb(255,223,222),
								   
							  };
			#endregion

			try {

				#region targets
//				//base target
                Int64 idBaseTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmBaseTargetAccess));
				//additional target
                Int64 idAdditionalTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmTargetAccess));									
				#endregion

				#region Wave
                Int64 idWave=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMWave,CustomerCst.Right.type.aepmWaveAccess));									
				#endregion

				#region Donn�es
				//				try{
				periodicityPlanData=TNS.AdExpress.Web.Rules.Results.APPM.PeriodicityPlanRules.PeriodicityPlan( webSession, dataSource,idWave,dateBegin, dateEnd,idBaseTarget,idAdditionalTarget);
				//					if(periodicityPlanData==null || periodicityPlanData.Rows.Count==0 )throw(new WebExceptions.PeriodicityPlanUIException());
				//					}
				//				catch(System.Exception){
				//								//	throw(new WebExceptions.APPMBusinessFacadeException("pas de donn�es+: "+ee.Message));
				//				return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage));
				//				}
				#endregion
				
				#region Cr�ation et d�finition du graphique pour la cible de base
				if(periodicityPlanData!=null && periodicityPlanData.Rows.Count>0)
					GetSeriesDataBase(periodicityPlanData,ref xUnitValues,ref yUnitValues);				
				//Cr�ation du graphique	des uit�s(euros, grp, insertion, page) cible de base
				ChartArea chartAreaUnit=null;
				Series seriePeriodicity=new Series();	
				if(periodicityPlanData!=null && periodicityPlanData.Rows.Count>0){																	
					//Conteneur graphique pour unit� de cible de base
					chartAreaUnit=new ChartArea();
					//Alignement
					if (webSession.Unit == WebConstantes.CustomerSessions.Unit.grp){
						chartAreaUnit.AlignOrientation = AreaAlignOrientation.Vertical;
						chartAreaUnit.Position.X=5;
						chartAreaUnit.Position.Y=2;
						chartAreaUnit.Position.Width=80;
						chartAreaUnit.Position.Height=32;	
					}else{
						chartAreaUnit.AlignOrientation = AreaAlignOrientation.Vertical;
						chartAreaUnit.Position.X=5;
						chartAreaUnit.Position.Y=2;
						chartAreaUnit.Position.Width=80;
						chartAreaUnit.Position.Height=38;	
					}
					//chartAreaUnit.Name="chartAreaUnit";								
					appmChart.ChartAreas.Add(chartAreaUnit);
					//Charger les s�ries de valeurs 
					string unitName="";
					string chartAreaName="";
					string chartAreaAdditionalName="";
					string chartAreaCgrpName="";
					#region s�lection par rappot � l'unit� choisit
					switch (webSession.Unit){
						case WebConstantes.CustomerSessions.Unit.euro:  
							unitName= GestionWeb.GetWebWord(1669,webSession.SiteLanguage);
							break;
						case WebConstantes.CustomerSessions.Unit.kEuro :  
							unitName= GestionWeb.GetWebWord(1790,webSession.SiteLanguage);
							break;
						case WebConstantes.CustomerSessions.Unit.grp:  
							unitName= GestionWeb.GetWebWord(1679,webSession.SiteLanguage);						
							break;
						case WebConstantes.CustomerSessions.Unit.insertion:  
							unitName= GestionWeb.GetWebWord(940,webSession.SiteLanguage);
							break;
						case WebConstantes.CustomerSessions.Unit.pages:  
							unitName= GestionWeb.GetWebWord(566,webSession.SiteLanguage);
							break;
						default : break;
					}
					#region Titres des graphiques
					if (webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
						//Titre graphique cible de base
						chartAreaName+=GestionWeb.GetWebWord(1736,webSession.SiteLanguage);
						chartAreaName+=" ";
						chartAreaName+= unitName ;
						chartAreaName+=" ("+periodicityPlanData.Rows[0]["baseTarget"]+") " ;
						chartAreaName+=GestionWeb.GetWebWord(1738,webSession.SiteLanguage);
						//Titre graphique cible selectionn�e
						chartAreaAdditionalName+=GestionWeb.GetWebWord(1736,webSession.SiteLanguage);
						chartAreaAdditionalName+=" ";
						chartAreaAdditionalName+=unitName ;
						chartAreaAdditionalName+=" ("+periodicityPlanData.Rows[0]["additionalTarget"]+") " ;
						chartAreaAdditionalName+=GestionWeb.GetWebWord(1738,webSession.SiteLanguage);
					}else{
						chartAreaName+=GestionWeb.GetWebWord(1736,webSession.SiteLanguage);
						chartAreaName+=" ";
						chartAreaName+= unitName ;
						chartAreaName+=" "+GestionWeb.GetWebWord(1738,webSession.SiteLanguage);					
					}
						//Titre graphique du CGRP
						chartAreaCgrpName+=GestionWeb.GetWebWord(1685,webSession.SiteLanguage);
						chartAreaCgrpName+=" "+GestionWeb.GetWebWord(1738,webSession.SiteLanguage)+" :"; 
						chartAreaCgrpName+=periodicityPlanData.Rows[0]["baseTarget"];
						chartAreaCgrpName+=" "+GestionWeb.GetWebWord(1739,webSession.SiteLanguage)+" "; 
						chartAreaCgrpName+=periodicityPlanData.Rows[0]["additionalTarget"] ;
					 
					#endregion

					#endregion
					seriePeriodicity=SetSeriesPeriodicity(periodicityPlanData,chartAreaUnit,seriePeriodicity,xUnitValues,yUnitValues,pieColors,chartAreaName,typeFlash);												
					#endregion												

					#region legend chart Area
					ChartArea chartAreaLegend=new ChartArea();
					chartAreaLegend.Name="legendArea";
					//Alignement
					if (webSession.Unit == WebConstantes.CustomerSessions.Unit.grp){
						chartAreaLegend.AlignOrientation = AreaAlignOrientation.Vertical;
						chartAreaLegend.Position.X=5;
						chartAreaLegend.Position.Y=32;
						chartAreaLegend.Position.Width=80;
						chartAreaLegend.Position.Height=6;	
					}else{
						chartAreaLegend.AlignOrientation = AreaAlignOrientation.Vertical;
						chartAreaLegend.Position.X=5;
						chartAreaLegend.Position.Y=36;
						chartAreaLegend.Position.Width=80;
						chartAreaLegend.Position.Height=8;					
					}
					appmChart.ChartAreas.Add(chartAreaLegend);
					appmChart.Legends["Default"].DockToChartArea = chartAreaLegend.Name;
					//appmChart.Legends["Default"].DockInsideChartArea = true;
					appmChart.Legends["Default"].InsideChartArea = "legendArea";
					appmChart.Legends["Default"].Enabled =false;
					appmChart.Legends["Default"].LegendStyle = LegendStyle.Table;
					appmChart.Legends["Default"].Docking = LegendDocking.Bottom;
					appmChart.Legends["Default"].Alignment = StringAlignment.Center;
	
					#endregion
			
					#region Cr�ation et d�finition du graphique pour la cible selectionn�e
					//Cr�ation du graphique	pour unit�
					ChartArea chartAreaUnitadditional=null;
					Series seriePeriodicityadditional=new Series();	
					//Conteneur graphique pour unit�
					chartAreaUnitadditional=new ChartArea();

					if (webSession.Unit == WebConstantes.CustomerSessions.Unit.grp){
						GetSeriesDataAdditional(periodicityPlanData,ref xUnitValues,ref yUnitValues);

						#region legend2
						Legend secondLegend = new Legend("Second");
						appmChart.Legends.Add(secondLegend);
						seriePeriodicityadditional.Legend = "Second";
						secondLegend.DockToChartArea = chartAreaUnitadditional.Name;
						secondLegend.DockInsideChartArea = true;	
						secondLegend.BorderWidth=2;
						secondLegend.Enabled=false;
						#endregion

						//Alignement
						chartAreaUnitadditional.AlignOrientation = AreaAlignOrientation.Vertical;
						chartAreaUnitadditional.Position.X=5;
						chartAreaUnitadditional.Position.Y=36;
						chartAreaUnitadditional.Position.Width=80;
						chartAreaUnitadditional.Position.Height=32;
						appmChart.ChartAreas.Add(chartAreaUnitadditional);
						//Charger les s�ries de valeurs 
						seriePeriodicityadditional=SetSeriesPeriodicity(periodicityPlanData,chartAreaUnitadditional,seriePeriodicityadditional,xUnitValues,yUnitValues,pieColors,chartAreaAdditionalName,typeFlash);												
					}
					#endregion
								
					#region Cr�ation et d�finition du graphique pour CGRP
					GetSeriesDataCgrp(periodicityPlanData,ref xUnitValues,ref yUnitValues,ref xUnitValuesSelected,ref yUnitValuesSelected,ref maxScale);
					//Cr�ation du graphique	pour unit�
					ChartArea chartAreaCgrp=null;
					Series seriePeriodicityCgrpBase=new Series();
					Series seriePeriodicityCgrpSelected=new Series();

					chartAreaCgrp=new ChartArea();
					seriePeriodicityCgrpBase.ShowInLegend=false;
					seriePeriodicityCgrpSelected.ShowInLegend=false;

					//legende du graphe bar
					ChartArea chartAreaCgrpLegend=new ChartArea();
					chartAreaCgrpLegend.Name="legendCgrpArea";
					//Allignement
					if (webSession.Unit == WebConstantes.CustomerSessions.Unit.grp){
						chartAreaCgrpLegend.AlignOrientation = AreaAlignOrientation.Vertical;
						chartAreaCgrpLegend.Position.X=5;
						chartAreaCgrpLegend.Position.Y=97;
						chartAreaCgrpLegend.Position.Width=80;
						chartAreaCgrpLegend.Position.Height=2;	
					}else{
						chartAreaCgrpLegend.AlignOrientation = AreaAlignOrientation.Vertical;
						chartAreaCgrpLegend.Position.X=5;
						chartAreaCgrpLegend.Position.Y=90;
						chartAreaCgrpLegend.Position.Width=80;
						chartAreaCgrpLegend.Position.Height=5;
					}
					
					appmChart.ChartAreas.Add(chartAreaCgrpLegend);

					#region legend3
					Legend thirdLegend = new Legend("Third");
					LegendItem legendItemReference = new LegendItem();
					legendItemReference.Name = GestionWeb.GetWebWord(1685,webSession.SiteLanguage);
					legendItemReference.Name+= " ("+periodicityPlanData.Rows[0]["baseTarget"]+") " ;
					legendItemReference.Style = LegendImageStyle.Rectangle;
					legendItemReference.ShadowOffset = 1;
					legendItemReference.Color =Color.FromArgb(255,215,215);
					thirdLegend.CustomItems.Add(legendItemReference);

					LegendItem legendItemReference2 = new LegendItem();
					legendItemReference2.Name = GestionWeb.GetWebWord(1685,webSession.SiteLanguage);
					legendItemReference2.Name+= " ("+periodicityPlanData.Rows[0]["additionalTarget"]+") " ;
					legendItemReference2.Style = LegendImageStyle.Rectangle;
					legendItemReference2.ShadowOffset = 1;
					legendItemReference2.Color = Color.FromArgb(255,215,215);
					thirdLegend.CustomItems.Add(legendItemReference2);
					appmChart.Legends.Add(thirdLegend);
					//serieInterestFamilyCgrpBase.Legend = "Third";
					thirdLegend.DockToChartArea = chartAreaCgrpLegend.Name;
					//thirdLegend.DockInsideChartArea = true;	
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
					seriePeriodicityCgrpSelected.Legend = "Fourth";
					fourthLegend.DockToChartArea = chartAreaCgrpLegend.Name;					
					//fourthLegend.DockInsideChartArea = true;	
					fourthLegend.InsideChartArea = "legendCgrpArea";
					legendItemReference.BorderWidth=1;
					legendItemReference.Color = Color.FromArgb(148,121,181);
					legendItemReference.Name=GestionWeb.GetWebWord(1685,webSession.SiteLanguage);
					legendItemReference.Name+= " ("+periodicityPlanData.Rows[0]["baseTarget"]+") " ;
					fourthLegend.Enabled =false;
					fourthLegend.CustomItems.Add(legendItemReference);				
					fourthLegend.Font=new Font("Arial", (float)8);
					fourthLegend.LegendStyle = LegendStyle.Row;
					fourthLegend.Docking = LegendDocking.Bottom;
					fourthLegend.Alignment = StringAlignment.Center;					
					#endregion

					//Conteneur graphique pour unit�
					
					//Alignement
					if (webSession.Unit == WebConstantes.CustomerSessions.Unit.grp){
						chartAreaCgrp.AlignOrientation = AreaAlignOrientation.Vertical;	
						chartAreaCgrp.Position.X=5;
						chartAreaCgrp.Position.Y=69;
						chartAreaCgrp.Position.Width=80;
						chartAreaCgrp.Position.Height=30;
					}
					else{
						chartAreaCgrp.AlignOrientation = AreaAlignOrientation.Vertical;	
						chartAreaCgrp.Position.X=5;
						chartAreaCgrp.Position.Y=52;
						chartAreaCgrp.Position.Width=80;
						chartAreaCgrp.Position.Height=38;
					}
					//chartAreaCgrp.Name="chartAreaCgrp";								
					appmChart.ChartAreas.Add(chartAreaCgrp);
					//Charger les s�ries de valeurs 
					seriePeriodicityCgrpBase=SetSeriesBarPeriodicity(periodicityPlanData,chartAreaCgrp,seriePeriodicityCgrpBase,xUnitValues,yUnitValues,Color.FromArgb(148,121,181),chartAreaCgrpName,maxScale,false);												
					seriePeriodicityCgrpSelected=SetSeriesBarPeriodicity(periodicityPlanData,chartAreaCgrp,seriePeriodicityCgrpSelected,xUnitValuesSelected,yUnitValuesSelected, Color.FromArgb(255,215,215),chartAreaCgrpName,maxScale,false);												
					
					#endregion

					//initialisation du control
					if (webSession.Unit == WebConstantes.CustomerSessions.Unit.grp){
						InitializeComponentGrp(appmChart,chartAreaUnit,chartAreaUnitadditional,chartAreaCgrp,typeFlash);

						if(periodicityPlanData!=null && periodicityPlanData.Rows.Count>0){
							appmChart.Series.Add(seriePeriodicity);
							appmChart.Series.Add(seriePeriodicityadditional);
							appmChart.Series.Add(seriePeriodicityCgrpBase);	
							appmChart.Series.Add(seriePeriodicityCgrpSelected);	
						}					
					}else{
						InitializeComponent(appmChart,chartAreaUnit,chartAreaCgrp,typeFlash);
						if(periodicityPlanData!=null && periodicityPlanData.Rows.Count>0){
							appmChart.Series.Add(seriePeriodicity);
							appmChart.Series.Add(seriePeriodicityCgrpBase);	
							appmChart.Series.Add(seriePeriodicityCgrpSelected);	
						}
					}
				}
			}
			catch(System.Exception err){				
				throw(new WebExceptions.PeriodicityPlanChartUIException("Erreur dans l'affichage des graphiques des donn�es des p�riodivit�s du plan ",err));
			}
		}


		#region m�thodes priv�es

		/// <summary>
		/// Obtient les s�ries de valeurs des unit�s pour la cible de base � afficher graphiquement
		/// </summary>
		/// <param name="dt">table de donn�es</param>
		/// <param name="xValues">libell�s du graphique</param>
		/// <param name="yValues">valeurs pour graphique</param>
		private static void GetSeriesDataBase(DataTable dt,ref string[] xValues,ref double[] yValues){
	
			#region Variable
			string ventilationType="periodicity";
			int x=0; 
			int y=0;
			#endregion
			
			#region Les s�ries  d'unit� � afficher graphiquement
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
		/// Obtient les s�ries de valeurs des unit�s pour la cible s�lectionn�e � afficher graphiquement
		/// </summary>
		/// <param name="dt">table de donn�es</param>
		/// <param name="xValues">libell�s du graphique</param>
		/// <param name="yValues">valeurs pour graphique</param>
		private static void GetSeriesDataAdditional(DataTable dt,ref string[] xValues,ref double[] yValues){
	
			#region Variable
			string ventilationType="periodicity";
			int x=0;
			int y=0;
			#endregion
			
			#region Les s�ries  d'unit� � afficher graphiquement
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
		/// Obtient les s�ries de valeurs des unit�s pour la cible s�lectionn�e � afficher graphiquement
		/// </summary>
		/// <param name="dt">table de donn�es</param>
		/// <param name="xValuesBase">libell�s du graphique</param>
		/// <param name="yValuesBase">valeurs pour graphique</param>
		/// <param name="xValuesSelected">Liebll�s s�lectionn�e</param>
		/// <param name="yValuesSelected">Valeur s�lectionn�e</param>
		/// <param name="maxScale">Echel maximum</param>
		private static void GetSeriesDataCgrp(DataTable dt,ref string[] xValuesBase,ref double[] yValuesBase,ref string[] xValuesSelected,ref double[] yValuesSelected,ref double maxScale){
	
			#region Variable
			//string ventilationTypeBase="cgrpBase";
			//string ventilationTypeSelected="cgrpSelected";
			int x=0;
			int y=0;
			#endregion
			
			#region Les s�ries  cgrp
			xValuesBase = new string[dt.Rows.Count-2];				
			yValuesBase = new double[dt.Rows.Count-2];
			xValuesSelected = new string[dt.Rows.Count-2];				
			yValuesSelected= new double[dt.Rows.Count-2];
	
			for(int i=0; i<dt.Rows.Count-1;i++){
				if(dt.Rows[i]["cgrpBase"].ToString()==""){
					continue;
				}else{
					xValuesBase[x]=dt.Rows[i]["periodicity"].ToString();
					//yValuesBase[y]=double.Parse(dt.Rows[i]["cgrpDistributionBase"].ToString());
					yValuesBase[y]=double.Parse(dt.Rows[i]["cgrpBase"].ToString());
					xValuesSelected[x]=dt.Rows[i]["periodicity"].ToString();
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
		/// Cr�tion du graphique unit�(grp,euro,isertion,page) 
		/// </summary>
		/// <param name="dt">tableau de r�sultats</param>
		/// <param name="chartArea">contenant objet graphique</param>
		/// <param name="series">s�ries de valeurs</param>
		/// <param name="xValues">s�ries de libell�s</param>
		/// <param name="yValues">s�ries de valeurs</param>
		/// <param name="barColors">couleurs du graphique</param>
		/// <param name="typeFlash">sortie flash</param>
		/// <param name="chartAreaName">Nom du conteneur de l'image</param>
		/// <returns>s�ries de valeurs</returns>
		private static  Dundas.Charting.WebControl.Series SetSeriesPeriodicity(DataTable dt ,ChartArea chartArea,Dundas.Charting.WebControl.Series series,string[] xValues,double[] yValues,Color[] barColors,string chartAreaName,bool typeFlash){
			#region  Cr�ation graphique
			if(xValues!=null && yValues!=null){
								
				#region Cr�ation et d�finition du graphique
				//Cr�ation du graphique							
				
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
				
				#region D�finition des couleurs
				//couleur du graphique
				for(int k=0;k<dt.Rows.Count && k<11;k++){
					series.Points[k].Color=barColors[k];
				}
				#endregion

				#region L�gende
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

		/// <summary>
		/// Cr�tion du graphique unit� Cgrp (histogramme)
		/// </summary>
		/// <param name="dt">tableau de r�sultats</param>
		/// <param name="chartArea">contenant objet graphique</param>
		/// <param name="series">s�ries de valeurs</param>
		/// <param name="xValues">s�ries de libell�s</param>
		/// <param name="yValues">s�ries de valeurs</param>
		///<param name="barColor">couleurs du graphique</param>
		/// <param name="chartAreaName">Nom du conteneur de l'image</param>
		/// <param name="maxScale">echelle maximum</param>
		/// <param name="typeFlash">sortie flash</param>
		/// <returns>s�ries de valeurs</returns>
		private static  Dundas.Charting.WebControl.Series SetSeriesBarPeriodicity(DataTable dt ,ChartArea chartArea,Dundas.Charting.WebControl.Series series,string[] xValues,double[] yValues,System.Drawing.Color barColor,string chartAreaName,double maxScale,bool typeFlash){
			#region  Cr�ation graphique
			if(xValues!=null && yValues!=null){
								
				#region Cr�ation et d�finition du graphique
				//Cr�ation du graphique							
				
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
				//chartArea.AxisX.Maximum= dt.Rows.Count+1;
				series.Points.DataBindXY(xValues,yValues);
				chartArea.AxisX.LabelStyle.Font = new Font("Arial", 8);
				chartArea.AxisY.LabelStyle.Font = new Font("Arial", 8);
				
				#region D�finition des couleurs
				//couleur du graphique
				series.Color= barColor;
				#endregion
 
				#region L�gende
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


		/// <summary>
		/// Initialise les styles du webcontrol pour m�dia radio et t�l�
		/// </summary>
		/// <param name="appmChart">Objet Webcontrol</param>
		/// <param name="chartAreaUnit">conteneur de l'image r�partition unit�</param>
		/// <param name="chartAreaCgrp">conteneur de l'image r�partition pour Cgrp</param>
		/// <param name="typeFlash">sortie flash</param>
		private static void InitializeComponent(APPMChartUI appmChart, ChartArea chartAreaUnit,ChartArea chartAreaCgrp,bool typeFlash) {			
			#region Animation Flash
			//Animation flash
			if(typeFlash){
				appmChart.ImageType=ChartImageType.Flash;
				appmChart.AnimationTheme = AnimationTheme.GrowingTogether;
				appmChart.AnimationDuration = 0.4;
				appmChart.RepeatAnimation = false;
			}
			else{
				appmChart.ImageType=ChartImageType.Jpeg;
			}
			#endregion

			#region Chart
			appmChart.Width=new Unit("700px");
			appmChart.Height=new Unit("800px");
			appmChart.BackGradientType = GradientType.TopBottom;
			appmChart.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);											
			appmChart.BorderStyle=ChartDashStyle.Solid;
			appmChart.BorderLineColor=Color.FromArgb(99,73,132);
			appmChart.BorderLineWidth=2;
			appmChart.Legend.Enabled=true;
			#endregion	

			#region Titre
			//titre unit� de base
			
			appmChart.Titles.Add(chartAreaUnit.Name);
			appmChart.Titles[0].DockInsideChartArea=true;
//			appmChart.Titles[0].Position.Auto = false;
//			appmChart.Titles[0].Position.X = 50;
//			appmChart.Titles[0].Position.Y = 2;
			appmChart.Titles[0].Font=new Font("Arial", (float)10);
			appmChart.Titles[0].Color=Color.FromArgb(100,72,131);
			appmChart.Titles[0].DockToChartArea=chartAreaUnit.Name;	
			
			//titre unit� pour CGRP

			appmChart.Titles.Add(chartAreaCgrp.Name);
			appmChart.Titles[1].DockInsideChartArea=true;
			appmChart.Titles[1].Position.Auto = false;
			appmChart.Titles[1].Position.X = 45;
			appmChart.Titles[1].Position.Y = 50;
			appmChart.Titles[1].Font=new Font("Arial", (float)10);
			appmChart.Titles[1].Color=Color.FromArgb(100,72,131);
			appmChart.Titles[1].DockToChartArea=chartAreaCgrp.Name;
			#endregion
		}

		/// <summary>
		/// Initialise les styles du webcontrol pour m�dia radio et t�l�
		/// </summary>
		/// <param name="appmChart">Objet Webcontrol</param>
		/// <param name="chartAreaUnit">conteneur de l'image r�partition unit�</param>
		/// <param name="chartAreaUnitadditional">conteneur de l'image r�partition pour cible selectionn�e</param>
		/// <param name="chartAreaCgrp">conteneur de l'image r�partition pour Cgrp</param>
		/// <param name="typeFlash">sortie flash</param>
		private static void InitializeComponentGrp(APPMChartUI appmChart, ChartArea chartAreaUnit,ChartArea chartAreaUnitadditional,ChartArea chartAreaCgrp,bool typeFlash) {			
			#region Animation Flash
			//Animation flash
			if(typeFlash){
				appmChart.ImageType=ChartImageType.Flash;
				appmChart.AnimationTheme = AnimationTheme.GrowingTogether;
				appmChart.AnimationDuration = 0.4;
				appmChart.RepeatAnimation = false;
			}
			else{
				appmChart.ImageType=ChartImageType.Jpeg;
			}
			#endregion

			#region Chart
			appmChart.Width=new Unit("700px");
			appmChart.Height=new Unit("1100px");
			appmChart.BackGradientType = GradientType.TopBottom;
			appmChart.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);											
			appmChart.BorderStyle=ChartDashStyle.Solid;
			appmChart.BorderLineColor=Color.FromArgb(99,73,132);
			appmChart.BorderLineWidth=2;
			appmChart.Legend.Enabled=true;
			#endregion	

			#region Titre
			//titre unit� de base
			
			appmChart.Titles.Add(chartAreaUnit.Name);
			appmChart.Titles[0].DockInsideChartArea=true;
			//			appmChart.Titles[0].Position.Auto = false;
			//			appmChart.Titles[0].Position.X = 50;
			//			appmChart.Titles[0].Position.Y = 2;
			appmChart.Titles[0].Font=new Font("Arial", (float)10);
			appmChart.Titles[0].Color=Color.FromArgb(100,72,131);
			appmChart.Titles[0].DockToChartArea=chartAreaUnit.Name;	
		
				//titre unit� pour la cible selectionn�e
		
				appmChart.Titles.Add(chartAreaUnitadditional.Name);
				appmChart.Titles[1].DockInsideChartArea=true;
				appmChart.Titles[1].Position.Auto = false;
				appmChart.Titles[1].Position.X = 50;
				appmChart.Titles[1].Position.Y = 41;
				appmChart.Titles[1].Font=new Font("Arial", (float)10);
				appmChart.Titles[1].Color=Color.FromArgb(100,72,131);
				appmChart.Titles[1].DockToChartArea=chartAreaUnitadditional.Name;

			//titre unit� pour CGRP

			appmChart.Titles.Add(chartAreaCgrp.Name);
			appmChart.Titles[2].DockInsideChartArea=true;
			appmChart.Titles[2].Position.Auto = false;
			appmChart.Titles[2].Position.X = 50;
			appmChart.Titles[2].Position.Y = 67;
			appmChart.Titles[2].Font=new Font("Arial", (float)10);
			appmChart.Titles[2].Color=Color.FromArgb(100,72,131);
			appmChart.Titles[2].DockToChartArea=chartAreaCgrp.Name;
			#endregion
		}

		#endregion

	}
}
