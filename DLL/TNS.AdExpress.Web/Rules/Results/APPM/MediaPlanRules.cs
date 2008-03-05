#region Information
//Authors: K.Shehzad, D.Mussuma
//Date of Creation: 24/08/2005
//Date of modification:
//	29/08/2005 G. Facon	Refonte total du rules
#endregion

using System;
using System.Data;
using System.Collections;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.Date;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Translation;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using DataAccessResults=TNS.AdExpress.Web.DataAccess.Results;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using SQLFunctions=TNS.AdExpress.Web.DataAccess;
using TNS.AdExpress.Web.BusinessFacade.Selections.Medias;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Core;

namespace TNS.AdExpress.Web.Rules.Results.APPM{
	/// <summary>
	/// This class formats the data for Media Plan APPM
	/// </summary>
	public class MediaPlanRules{
		/// <summary>
		/// Formats the date for Media Plan Table 
		/// </summary>
		/// <param name="webSession">clients session</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="dateBegin">Beginning Date</param>
		/// <param name="dateEnd">Ending Date</param>
		/// <param name="baseTarget">Base target</param>
		/// <param name="additionalTarget">supplementary target</param>
		/// <param name="detailPeriod">period detail : monthly or weekly or daily</param>
		/// <returns>Formatted table for the PDV Analysis table</returns>		
		public static object[,] GetData(WebSession webSession,IDataSource dataSource,int dateBegin,int dateEnd,Int64 baseTarget,Int64 additionalTarget,WebConstantes.CustomerSessions.Period.DisplayLevel detailPeriod){
			
			#region Variables
			object[,] tab=null;
			DataTable planMediaData=null;
//			DataTable totalPublications=null;
//			DataTable mediaTable=null;
//			DataRow[] singleMediaPublications=null;
			//Int64 oldIdVehicle=0;
			Int64 oldIdCategory=0;
			Int64 oldIdMedia=0;
//			Int64 currentVehicleIndex=2;
			Int64 currentCategoryIndex=0;
			Int64 currentLineIndex=1;			
//			bool forceEntry=true;
//			AtomicPeriodWeek weekDate=null;
			//double budget=0.0;
//			double grp=0;
//			double pages=0;
			int currentDate=0;
			int oldCurrentDate=0;
			int oldDate=0;
//			Int64 i;
			int numberOflineToAdd=0;
			int nbCol=0;
			int nbline=0;					
			currentDate = 0;
			oldCurrentDate = 0;
			int nbItemsToExtend;
			string idsMedia=string.Empty;
			int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.MediaPlanAPPM.FIRST_PEDIOD_INDEX;
			int TOTAL_LINE_INDEX=FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX;
			
			
			#endregion

			#region test
//			 detailPeriod=WebConstantes.CustomerSessions.Period.DisplayLevel.dayly;
			#endregion

			#region Get data
			//getting the reference univers data
			planMediaData=DataAccessResults.APPM.MediaPlanDataAccess.Get(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget).Tables[0];				
			#endregion

			#region Getting Media ids and with the help of it getting all the publications of the selected medias
//			mediaTable=SQLFunctions.Functions.SelectDistinct("medias",planMediaData,"id_media");
//			foreach(DataRow dr in mediaTable.Rows){
//				idsMedia+=dr["id_media"].ToString()+",";				
//			}
//			totalPublications=TNS.AdExpress.Web.DataAccess.Results.APPM.MediaPlanDataAccess.GetAllPublications(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget,idsMedia.Remove(idsMedia.Length-1,1)).Tables[0];
			#endregion

			try{
				if(planMediaData!=null && planMediaData.Rows.Count>0){

					#region Counting the number of vehicles and categories
					//Faire un select sur le dataset pour avoir le nombre de catégories
					int nbCategory=0;
					foreach(DataRow currentRow in planMediaData.Rows){
//						if(oldIdVehicle!=(Int64)currentRow["id_vehicle"]){
//							nbVehicle++;
//							oldIdVehicle=(Int64)currentRow["id_vehicle"];
//						}
						if(oldIdCategory!=(Int64)currentRow["id_category"]){
							nbCategory++;
							oldIdCategory=(Int64)currentRow["id_category"];
						}
					}
					// There is no data
					//if(nbVehicle==0)return(new object[0,0]);
					//oldIdVehicle=0;
					oldIdCategory=0;
					#endregion
					
					#region Creating the table of month or weeks or days
					string tmpDate;
					ArrayList periodItemsList=new ArrayList();
				
					//Weeks table
					if(detailPeriod==WebConstantes.CustomerSessions.Period.DisplayLevel.weekly){
						AtomicPeriodWeek currentWeek=new AtomicPeriodWeek(int.Parse(webSession.PeriodBeginningDate.Substring(0,4)),int.Parse(webSession.PeriodBeginningDate.Substring(4,2)));
						AtomicPeriodWeek endWeek=new AtomicPeriodWeek(int.Parse(webSession.PeriodEndDate.Substring(0,4)),int.Parse(webSession.PeriodEndDate.Substring(4,2)));
						endWeek.Increment();
						while(!(currentWeek.Week==endWeek.Week && currentWeek.Year==endWeek.Year)){
							tmpDate=currentWeek.Year.ToString();
							if(currentWeek.Week.ToString().Length<2)tmpDate+="0"+currentWeek.Week.ToString();
							else tmpDate+=currentWeek.Week.ToString();
							periodItemsList.Add(tmpDate);
							currentWeek.Increment();
						}

					}
					//Month table
					if(detailPeriod==WebConstantes.CustomerSessions.Period.DisplayLevel.monthly){
						DateTime dateCurrent=new DateTime(int.Parse(webSession.PeriodBeginningDate.Substring(0,4)),int.Parse(webSession.PeriodBeginningDate.Substring(4,2)),1);
						DateTime endDate=new DateTime(int.Parse(webSession.PeriodEndDate.Substring(0,4)),int.Parse(webSession.PeriodEndDate.Substring(4,2)),1);
						endDate=endDate.AddMonths(1);						
						while(!(dateCurrent.Month==endDate.Month && dateCurrent.Year==endDate.Year)){
							tmpDate=dateCurrent.Year.ToString();
							if(dateCurrent.Month.ToString().Length<2)tmpDate+="0"+dateCurrent.Month.ToString();
							else tmpDate+=dateCurrent.Month.ToString();
							periodItemsList.Add(tmpDate);
							dateCurrent=dateCurrent.AddMonths(1);
						}
					}
				
					//Days table
					if(detailPeriod==WebConstantes.CustomerSessions.Period.DisplayLevel.dayly){
						DateTime currentDateTime =  WebFunctions.Dates.getPeriodBeginningDate(dateBegin.ToString(),Constantes.Web.CustomerSessions.Period.Type.nLastDays);
						DateTime endDate = WebFunctions.Dates.getPeriodEndDate(dateEnd.ToString(),Constantes.Web.CustomerSessions.Period.Type.nLastDays);
						while(currentDateTime<=endDate){
							tmpDate=currentDateTime.Year.ToString();
							if(currentDateTime.Month.ToString().Length<2)tmpDate+="0"+currentDateTime.Month.ToString();
							else tmpDate+=currentDateTime.Month.ToString();
							if(currentDateTime.Day.ToString().Length<2)tmpDate+="0"+currentDateTime.Day.ToString();
							else tmpDate+=currentDateTime.Day.ToString();
							periodItemsList.Add(tmpDate);
							currentDateTime = currentDateTime.AddDays(1);
						}
					}
					
					#endregion

					#region Table declarations
					// Nombre de colonne
					nbCol=periodItemsList.Count+FrameWorkResultConstantes.MediaPlanAPPM.FIRST_PEDIOD_INDEX;
					// Nombre de lignes
					nbline=planMediaData.Rows.Count+2+nbCategory; //Avant +2
					// Tableau de résultat
					tab=new object[nbline,nbCol];
					// Tableau des indexes des categories
					//Int64[] tabCategoryIndex=new Int64[nbCategory+1];
					#endregion

					#region Labelling the colunms
					while(currentDate<periodItemsList.Count){
						tab[0,FIRST_PERIOD_INDEX+currentDate]=(string)periodItemsList[currentDate];
						currentDate++;
					}					
					#endregion

					#region Initialisation de la ligne totale
					// On initialise toutes les dates a absent
					tab[TOTAL_LINE_INDEX,FrameWorkResultConstantes.MediaPlanAPPM.ID_TOTAL_COUMN_INDEX]=(Int64)FrameWorkResultConstantes.MediaPlanAPPM.ID_TOTAL_STRING;
					tab[TOTAL_LINE_INDEX,FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_COUMN_INDEX]="Total"; // à Modifier dans l'UI pour mettre GestionWeb
					for(int k=FIRST_PERIOD_INDEX;k<nbCol;k++){
						tab[TOTAL_LINE_INDEX,k]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.absent;
					}
					#endregion

					#region Construction of the resultant table
		
					foreach(DataRow currentRow in planMediaData.Rows){

						#region Nouvelle Category
						if(oldIdCategory!=(Int64)currentRow["id_category"]){
							currentLineIndex++;
							tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.ID_CATEGORY_COUMN_INDEX]=(Int64)currentRow["id_category"];
							tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.CATEGORY_COLUMN_INDEX]=currentRow["category"].ToString();
							//fin MAJ
							currentCategoryIndex=currentLineIndex;
							oldIdCategory=(Int64)currentRow["id_category"];
							numberOflineToAdd++;
							tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.MEDIA_COLUMN_INDEX]=null;
							tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.ID_MEDIA_COLUMN_INDEX]=null;
							// On initialise toutes les dates a absent
							for(int k=FIRST_PERIOD_INDEX;k<nbCol;k++){
								tab[currentCategoryIndex,k]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.absent;
							}
						}
						#endregion

						#region Nouveau Media
						if(oldIdMedia!=(Int64)currentRow["id_media"]){
							currentLineIndex++;
							tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.ID_MEDIA_COLUMN_INDEX]=(Int64)currentRow["id_media"];
							tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.MEDIA_COLUMN_INDEX]=currentRow["media"].ToString();
							tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.PERIODICITY_COLUMN_INDEX]=(Int64)currentRow["id_periodicity"];									
							//fin MAJ
							oldIdMedia=(Int64)currentRow["id_media"];
							currentDate=0;
							tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.CATEGORY_COLUMN_INDEX]=null;
						}
						else{
							#region Même mois
							if(oldDate==GetFormattedDate(currentRow["publication_date"].ToString(),detailPeriod)){
								currentDate--; //PB si 2 parutions le même mois
							}
							#endregion
						}
						#endregion

						// On se place sur le bon mois
						// En même temps on remplit les case vide par absent
//						if(oldIdMedia==6169){
//							string tireote="e";
//						}
						while(int.Parse(periodItemsList[currentDate].ToString())!=GetFormattedDate(currentRow["publication_date"].ToString(),detailPeriod)){ //(int.Parse(currentRow["publication_date"].ToString().Substring(0,6)))){
							if(tab[currentLineIndex,FIRST_PERIOD_INDEX+currentDate]==null)
								tab[currentLineIndex,FIRST_PERIOD_INDEX+currentDate]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.absent;
							currentDate++;
						}
						//valeur pour le media
						tab[currentLineIndex,FIRST_PERIOD_INDEX+currentDate]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present;
						//valeur pour la catégorie
						tab[currentCategoryIndex,FIRST_PERIOD_INDEX+currentDate]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present;
						//valeur pour le total
						tab[TOTAL_LINE_INDEX,FIRST_PERIOD_INDEX+currentDate]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present;
						
						
						currentDate++;
						oldCurrentDate=currentDate;
						oldDate=GetFormattedDate(currentRow["publication_date"].ToString(),detailPeriod);
						// On ajoute la périodicité(cas détail par mois)
						nbItemsToExtend=GetNbItemsToExtend(oldIdMedia,int.Parse(currentRow["publication_date"].ToString()),int.Parse(currentRow["id_periodicity"].ToString()),detailPeriod);
//						i=0;
						while(oldCurrentDate<periodItemsList.Count && nbItemsToExtend>0){
							tab[currentLineIndex,FIRST_PERIOD_INDEX+oldCurrentDate]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
							if((FrameWorkConstantes.Results.MediaPlan.graphicItemType)tab[currentCategoryIndex,FIRST_PERIOD_INDEX+oldCurrentDate]!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)
								tab[currentCategoryIndex,FIRST_PERIOD_INDEX+oldCurrentDate]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
							if((FrameWorkConstantes.Results.MediaPlan.graphicItemType)tab[TOTAL_LINE_INDEX,FIRST_PERIOD_INDEX+oldCurrentDate]!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)
								tab[TOTAL_LINE_INDEX,FIRST_PERIOD_INDEX+oldCurrentDate]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
							nbItemsToExtend--;
							oldCurrentDate++;
						}
						// A mettre dans le changement de media ??????? 
						while(oldCurrentDate<periodItemsList.Count){
							tab[currentLineIndex,FIRST_PERIOD_INDEX+oldCurrentDate]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.absent;
							oldCurrentDate++;
						}
	
					}
					#endregion

					#region Treating the periodicity	
//					for(i=1;i<nbline;i++)
//					{
//						if(tab[i,0]!=null)if(tab[i,0].GetType()==typeof(FrameWorkConstantes.MemoryArrayEnd)) break;
//						// C'est une ligne Vehicle
//						if(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.VEHICLE_COLUMN_INDEX]!=null)currentVehicleIndex=i;
//						// C'est une ligne Category
//						if(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.CATEGORY_COLUMN_INDEX]!=null)currentCategoryIndex=i;
//						// C'est une ligne Media
//						if(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.MEDIA_COLUMN_INDEX]!=null)
//						{
//							for(int j=FIRST_PERIOD_INDEX;j<nbCol;j++)
//							{
//								if((bool)tab[i,j]==true)
//								{
//									tab[i,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present;
//									tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present;
//									tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present;
//									//tab[FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present;
//									if(webSession.DetailPeriod==Constantes.Web.CustomerSessions.Period.DisplayLevel.monthly)
//									{
//										switch(int.Parse(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.PERIODICITY_COLUMN_INDEX].ToString()))
//										{
//												#region Trimestirel
//											case (int)DBClassificationConstantes.Periodicity.type.trimestriel:
//												j++;
//												if(j<nbCol)
//												{
//													tab[i,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//													//if(tab[FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//													if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//													if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//												}
//												j++;
//												if(j<nbCol)
//												{
//													tab[i,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//													//if(tab[FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//													if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//													if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//												}
//												break;
//												#endregion
//
//												#region Bimestriel
//											case (int)DBClassificationConstantes.Periodicity.type.bimestriel:
//												j++;
//												if(j<nbCol)
//												{
//													tab[i,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//													//if(tab[FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//													if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//													if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//												}
//												break;
//												#endregion
//
//											default:
//												break;
//										}
//									}
//									else
//									{
//										switch(int.Parse(tab[i,FrameWorkConstantes.Results.MediaPlan.PERIODICITY_COLUMN_INDEX].ToString()))
//										{
//												#region Mensuel
//											case (int)DBClassificationConstantes.Periodicity.type.mensuel:
//												j++;
//												if(j<nbCol)
//												{
//													weekDate=new AtomicPeriodWeek(int.Parse(tab[0,j].ToString().Substring(0,4)),int.Parse(tab[0,j].ToString().Substring(4,2)));
//													forceEntry=true;
//													while((weekDate.FirstDay.Day<weekDate.LastDay.Day && !((int)weekDate.FirstDay.DayOfWeek ==1 && weekDate.FirstDay.Day==1)) || forceEntry)
//													{
//														forceEntry=false;
//														if(j<nbCol)
//														{
//															tab[i,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//															if(tab[FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//															if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//															if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//														}
//														j++;
//														if(j<nbCol) weekDate=new AtomicPeriodWeek(int.Parse(tab[0,j].ToString().Substring(0,4)),int.Parse(tab[0,j].ToString().Substring(4,2)));
//														else break;
//													}
//													if(j>FIRST_PERIOD_INDEX)j--;
//												}
//												break;
//												#endregion
//
//												#region Bimensuel
//											case (int)DBClassificationConstantes.Periodicity.type.bimensuel:
//												j++;
//												if(j<nbCol)
//												{
//													tab[i,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//													if(tab[FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//													if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//													if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//												}
//												break;
//												#endregion
//
//												#region trimestriel
//											case (int)DBClassificationConstantes.Periodicity.type.trimestriel:
//												j++;
//												if(j<nbCol)
//												{
//													weekDate=new AtomicPeriodWeek(int.Parse(tab[0,j].ToString().Substring(0,4)),int.Parse(tab[0,j].ToString().Substring(4,2)));
//													for(int l=0;l<3;l++)
//													{
//														forceEntry=true;
//														while((weekDate.FirstDay.Day<weekDate.LastDay.Day && !((int)weekDate.FirstDay.DayOfWeek ==1 && weekDate.FirstDay.Day==1)) || forceEntry)
//														{
//															forceEntry=false;
//															if(j<nbCol)
//															{
//																tab[i,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//																if(tab[FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//																if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//																if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//															}
//															j++;
//															if(j<nbCol) weekDate=new AtomicPeriodWeek(int.Parse(tab[0,j].ToString().Substring(0,4)),int.Parse(tab[0,j].ToString().Substring(4,2)));
//															else break;
//														}
//													}
//													if(j>FIRST_PERIOD_INDEX)j--;
//												}
//												break;
//												#endregion
//
//												#region Bimestriel
//											case (int)DBClassificationConstantes.Periodicity.type.bimestriel:
//												j++;
//												if(j<nbCol)
//												{
//													weekDate=new AtomicPeriodWeek(int.Parse(tab[0,j].ToString().Substring(0,4)),int.Parse(tab[0,j].ToString().Substring(4,2)));
//													for(int l=0;l<2;l++)
//													{
//														forceEntry=true;
//														while((weekDate.FirstDay.Day<weekDate.LastDay.Day && !((int)weekDate.FirstDay.DayOfWeek ==1 && weekDate.FirstDay.Day==1)) || forceEntry)
//														{
//															forceEntry=false;
//															if(j<nbCol)
//															{
//																tab[i,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//																if(tab[FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//																if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//																if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//															}
//															j++;
//															if(j<nbCol) weekDate=new AtomicPeriodWeek(int.Parse(tab[0,j].ToString().Substring(0,4)),int.Parse(tab[0,j].ToString().Substring(4,2)));
//															else break;
//														}
//													}
//													if(j>FIRST_PERIOD_INDEX)j--;
//												}
//												break;
//												#endregion
//
//											default:
//												break;
//										}
//									}
//								}
//								else
//								{
//									tab[i,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.absent;
//								}
//							}
//						}
//					}
					#endregion
				}
			}		
			catch(System.Exception err){
				throw(new WebExceptions.APPMMediaPlanRulesExcpetion("Error while formatting the data Media Plan in APPM ",err));
			}

			#region Debug
			#if(DEBUG)
			/*
			int j;
			string HTML="<html><table><tr>";
			try{
				for(i=0;i<nbline;i++){
					for(j=0;j<nbCol;j++){
						if(tab[i,j]!=null)HTML+="<td>"+tab[i,j].ToString()+"</td>";
						else HTML+="<td>&nbsp;</td>";
					}
					HTML+="</tr><tr>";
				}
			}
			catch(System.Exception err){
				string g="3";
			}
			HTML+="</tr></table></html>";
			*/
			#endif
			#endregion

			return (tab);
		}
		
		
		#region Plan Media with Version
		/// <summary>
		/// Formats the date for Media Plan Table 
		/// </summary>
		/// <param name="webSession">clients session</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="dateBegin">Beginning Date</param>
		/// <param name="dateEnd">Ending Date</param>
		/// <param name="baseTarget">Base target</param>
		/// <param name="additionalTarget">supplementary target</param>
		/// <param name="detailPeriod">period detail : monthly or weekly or daily</param>
		/// <returns>Formatted table for the PDV Analysis table</returns>		
		public static object[,] GetFormattedTable(WebSession webSession,IDataSource dataSource,int dateBegin,int dateEnd,Int64 baseTarget,Int64 additionalTarget,WebConstantes.CustomerSessions.Period.DisplayLevel detailPeriod){
			
			#region Variables
			object[,] tab=null;	
			DataTable planMediaData=null;

			Int64 currentLineIndex=0;				
			int currentDate=0;
			int oldCurrentDate=0;
			int oldDate=0;
			int nbL1=0;
			int nbL2=0;
			int nbL3=0;
			int nbL4=0;
			Int64 oldIdL1=0;
			Int64 oldIdL2=0;
			Int64 oldIdL3=0;
			Int64 oldIdL4=0;
            //Int64 i;
//			Int64 currentTotalIndex=1;
			Int64 currentL1Index=2;
			Int64 currentL2Index=0;
			Int64 currentL3Index=1;
			Int64 currentL4Index=1;			
			int numberOflineToAdd=0;
			int k=0;//,mpi,nbDays,nbMonth = 0;
			int nbCol=0;
			int nbline=0;							
			int nbItemsToExtend;
			string idsMedia=string.Empty;
			int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.MediaPlanAPPM.FIRST_PEDIOD_COLUMN_INDEX;
//			int TOTAL_LINE_INDEX=FrameWorkResultConstantes.MediaPlanAPPM.TOTAL_LINE_INDEX;	
			bool forceL2=false;
			bool forceL3=false;
			bool forceL4=false;		
			#endregion
			
			if(webSession.CustomerLogin.GetFlag(TNS.AdExpress.Constantes.DB.Flags.ID_SLOGAN_ACCESS_FLAG)!=null){//Droits versions
				#region Get data
				//getting the reference univers data
				planMediaData=DataAccessResults.APPM.MediaPlanDataAccess.GetDataWithVersions(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget).Tables[0];				
				#endregion

				try{
					if(planMediaData!=null && planMediaData.Rows.Count>0){

						#region Counting the number of element by media level
						// On compte le nombre d'éléments par niveau Pour construire la tableau final
						int nbLevels=webSession.GenericMediaDetailLevel.GetNbLevels;
						foreach(DataRow currentRow in planMediaData.Rows){
							if(nbLevels>=1 && oldIdL1!=GetLevelId(webSession,currentRow,1)){
								forceL2=true;
								nbL1++;
								oldIdL1=GetLevelId(webSession,currentRow,1);
							}
							if(nbLevels>=2 && (oldIdL2!=GetLevelId(webSession,currentRow,2)||forceL2)){
								forceL3=true;
								forceL2=false;
								nbL2++;
								oldIdL2=GetLevelId(webSession,currentRow,2);
							}
							if(nbLevels>=3 && (oldIdL3!=GetLevelId(webSession,currentRow,3)||forceL3)){
								forceL4=true;
								forceL3=false;
								nbL3++;
								oldIdL3=GetLevelId(webSession,currentRow,3);
							}
							if(nbLevels>=4 && (oldIdL4!=GetLevelId(webSession,currentRow,4)||forceL4)){
								forceL4=false;
								nbL4++;
								oldIdL4=GetLevelId(webSession,currentRow,4);
							}
						}
						forceL2=forceL3=forceL4=false;
						oldIdL1=oldIdL2=oldIdL3=oldIdL4=-1;
						#endregion

						// Il n'y a pas de données
						if(nbL1==0){					
							return(new object[0,0]);
						}

						#region Creating the table of month or weeks or days

						string tmpDate;
						string tmpMonthDate;
						AtomicPeriodWeek tempWeekDate;
						ArrayList periodItemsList=new ArrayList(),periodMonthItemsList=new ArrayList(),periodWeekItemsList=new ArrayList();
				

						//Days table
						if(detailPeriod==WebConstantes.CustomerSessions.Period.DisplayLevel.dayly){
							DateTime currentDateTime =  WebFunctions.Dates.getPeriodBeginningDate(dateBegin.ToString(),Constantes.Web.CustomerSessions.Period.Type.nLastDays);
							DateTime endDate = WebFunctions.Dates.getPeriodEndDate(dateEnd.ToString(),Constantes.Web.CustomerSessions.Period.Type.nLastDays);
							while(currentDateTime<=endDate){
								tmpDate=tmpMonthDate=currentDateTime.Year.ToString();
								if(currentDateTime.Month.ToString().Length<2){
									tmpDate+="0"+currentDateTime.Month.ToString();
									tmpMonthDate=tmpDate;
								}
								else{ 
									tmpDate+=currentDateTime.Month.ToString();
									tmpMonthDate=tmpDate;
								}
								if(currentDateTime.Day.ToString().Length<2)tmpDate+="0"+currentDateTime.Day.ToString();
								else tmpDate+=currentDateTime.Day.ToString();

							
								periodItemsList.Add(tmpDate);
								periodMonthItemsList.Add(tmpMonthDate);
								tempWeekDate = new AtomicPeriodWeek(currentDateTime);
								periodWeekItemsList.Add(tempWeekDate.Week.ToString());
								currentDateTime = currentDateTime.AddDays(1);
							}
						}
					
						#endregion

						#region Table declarations
						// Nombre de colonnes
						nbCol=periodItemsList.Count+FIRST_PERIOD_INDEX;
						// Nombre de lignes
						nbline=nbL1+nbL2+nbL3+nbL4+3;
						// Tableau de résultat
						tab=new object[nbline,nbCol];
					
						#endregion
		
						#region Labelling the columns

						//Labelling months and weeks and days columns
					
						while(currentDate<periodItemsList.Count){

							tab[0,currentDate+FIRST_PERIOD_INDEX]=(string)periodMonthItemsList[currentDate];//months labels
												
							tab[1,currentDate+FIRST_PERIOD_INDEX]=(string)periodWeekItemsList[currentDate];//week labels
						
							tab[2,currentDate+FIRST_PERIOD_INDEX]=(string)periodItemsList[currentDate];//days labels
							currentDate++;
						}
						currentLineIndex=2;
						#endregion
				
						foreach(DataRow currentRow in planMediaData.Rows) {

							#region Nouveau Niveau L1
							if(nbLevels>=1 && oldIdL1!=GetLevelId(webSession,currentRow,1)){
								// Le Prochain niveau L2 doit être différent
								forceL2=true;														

								currentLineIndex++;
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L1_COLUMN_INDEX]=GetLevelLabel(webSession,currentRow,1);
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L1_ID_COLUMN_INDEX]=GetLevelId(webSession,currentRow,1);						
								if(nbLevels<=1) tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.PERIODICITY_COLUMN_INDEX]=currentRow["id_periodicity"].ToString();
						
								//fin MAJ
								currentL1Index=currentLineIndex;
								oldIdL1=GetLevelId(webSession,currentRow,1);
								currentDate=0;
								numberOflineToAdd++;
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L2_COLUMN_INDEX]=null;
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L2_ID_COLUMN_INDEX]=null;
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L3_COLUMN_INDEX]=null;
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L3_ID_COLUMN_INDEX]=null;
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L4_COLUMN_INDEX]=null;
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L4_ID_COLUMN_INDEX]=null;
								// On initialise toutes les dates a absent
								for( k=FIRST_PERIOD_INDEX;k<nbCol;k++){
									tab[currentL1Index,k]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.absent;
								}

							}
							#endregion

							#region Nouveau Niveau L2
							if(nbLevels>=2 && (oldIdL2!=GetLevelId(webSession,currentRow,2) || forceL2)){
								// Le Prochain niveau L3 doit être différent
								forceL3=true;
								forceL2=false;
														
								currentLineIndex++;
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L2_COLUMN_INDEX]=GetLevelLabel(webSession,currentRow,2);
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L2_ID_COLUMN_INDEX]=GetLevelId(webSession,currentRow,2);							
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L1_ID_COLUMN_INDEX]=oldIdL1;
								if(nbLevels<=2) tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.PERIODICITY_COLUMN_INDEX]=currentRow["id_periodicity"].ToString();
						
								//fin MAJ
								currentL2Index=currentLineIndex;
								oldIdL2=GetLevelId(webSession,currentRow,2);
								currentDate=0;
								numberOflineToAdd++;
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L1_COLUMN_INDEX]=null;
				
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L3_COLUMN_INDEX]=null;
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L3_ID_COLUMN_INDEX]=null;
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L4_COLUMN_INDEX]=null;
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L4_ID_COLUMN_INDEX]=null;
								// On initialise toutes les dates a absent
								for( k=FIRST_PERIOD_INDEX;k<nbCol;k++){
									tab[currentL2Index,k]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.absent;
								}
							}
							#endregion

							#region Nouveau Niveau L3
							if(nbLevels>=3 && (oldIdL3!=GetLevelId(webSession,currentRow,3)|| forceL3)){
								// Le Prochain niveau L4 doit être différent
								forceL4=true;
								forceL3=false;
							

								currentLineIndex++;
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L3_COLUMN_INDEX]=GetLevelLabel(webSession,currentRow,3);
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L3_ID_COLUMN_INDEX]=GetLevelId(webSession,currentRow,3);						
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L1_ID_COLUMN_INDEX]=oldIdL1;
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L2_ID_COLUMN_INDEX]=oldIdL2;
								if(nbLevels<=3) tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.PERIODICITY_COLUMN_INDEX]=currentRow["id_periodicity"].ToString();
						
								//fin MAJ
								currentL3Index=currentLineIndex;
								oldIdL3=GetLevelId(webSession,currentRow,3);
								currentDate=0;
								numberOflineToAdd++;
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L1_COLUMN_INDEX]=null;
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L2_COLUMN_INDEX]=null;
				
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L4_COLUMN_INDEX]=null;
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L4_ID_COLUMN_INDEX]=null;
								// On initialise toutes les dates a absent
								for( k=FIRST_PERIOD_INDEX;k<nbCol;k++){
									tab[currentL3Index,k]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.absent;
								}
							}
							#endregion

							#region Nouveau Niveau L4
							if(nbLevels>=4 && (oldIdL4!=GetLevelId(webSession,currentRow,4)|| forceL4)){
								forceL4=false;
								currentLineIndex++;
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L4_COLUMN_INDEX]=GetLevelLabel(webSession,currentRow,4);
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L4_ID_COLUMN_INDEX]=GetLevelId(webSession,currentRow,4);							
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L1_ID_COLUMN_INDEX]=oldIdL1;
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L2_ID_COLUMN_INDEX]=oldIdL2;
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L3_ID_COLUMN_INDEX]=oldIdL3;
								if(nbLevels<=4) tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.PERIODICITY_COLUMN_INDEX]=currentRow["id_periodicity"].ToString();
							
								//fin MAJ
								currentL4Index=currentLineIndex;
								oldIdL4=GetLevelId(webSession,currentRow,4);
								currentDate=0;
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L1_COLUMN_INDEX]=null;
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L2_COLUMN_INDEX]=null;
								tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L3_COLUMN_INDEX]=null;
								// On initialise toutes les dates a absent
								for( k=FIRST_PERIOD_INDEX;k<nbCol;k++){
									tab[currentL4Index,k]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.absent;
								}
							}
							#endregion

							#region Traitement des présences dans les dates
							// On se place sur le bon mois
							// En même temps on remplit les case vide par absent					
							while(int.Parse(periodItemsList[currentDate].ToString())!=GetFormattedDate(currentRow["publication_date"].ToString(),detailPeriod)){
								if(tab[currentLineIndex,FIRST_PERIOD_INDEX+currentDate]==null)
									tab[currentLineIndex,FIRST_PERIOD_INDEX+currentDate]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.absent;
								currentDate++;
							}
							#endregion

							//Règle: Activités pour le Niveau L1,l2,l3 (VEHICLE par défaut) ne sont pas montré dans ce calendrier =>graphicItemType = absent
							tab[currentL1Index,FIRST_PERIOD_INDEX+currentDate]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.absent;
							tab[currentL2Index,FIRST_PERIOD_INDEX+currentDate]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.absent;
							tab[currentL3Index,FIRST_PERIOD_INDEX+currentDate]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.absent;
							//Activité L4 ( par défaut version )
							if(tab[currentLineIndex,FrameWorkResultConstantes.MediaPlanAPPM.L4_COLUMN_INDEX]!=null)
								tab[currentLineIndex,FIRST_PERIOD_INDEX+currentDate]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present;
						
							currentDate++;
							oldCurrentDate=currentDate;
							oldDate=GetFormattedDate(currentRow["publication_date"].ToString(),detailPeriod);

							// On ajoute la périodicité(cas détail par mois)
							nbItemsToExtend=GetNbItemsToExtend(oldIdL3,int.Parse(currentRow["publication_date"].ToString()),int.Parse(currentRow["id_periodicity"].ToString()),detailPeriod); //Avec oldIdL3<==>oldIdMedia
						
							while(oldCurrentDate<periodItemsList.Count && nbItemsToExtend>0){
								tab[currentLineIndex,FIRST_PERIOD_INDEX+oldCurrentDate]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;							
								nbItemsToExtend--;
								oldCurrentDate++;
							}

							// A mettre dans le changement de media ??????? 
							while(oldCurrentDate<periodItemsList.Count){
								tab[currentLineIndex,FIRST_PERIOD_INDEX+oldCurrentDate]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.absent;
								oldCurrentDate++;
							}
						}

					
					
					
					}
				}		
				catch(System.Exception err){
					throw(new WebExceptions.APPMMediaPlanRulesExcpetion("Error while formatting the data Media Plan with Versions in APPM ",err));
				}
			}
			return tab;
		}
		#endregion
		
		#region Private Methods

		/// <summary>
		/// Obtient la date formatée en fonction du type de détail
		/// </summary>
		/// <param name="dayDate">date à convertir (YYYYMMDD)</param>
		/// <param name="detailPeriodType">type de détail date</param>
		/// <returns>Date formatée</returns>
		private static int GetFormattedDate(string dayDate,WebConstantes.CustomerSessions.Period.DisplayLevel detailPeriodType){
			switch(detailPeriodType){
				#region Mois
				case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
					return(int.Parse(dayDate.Substring(0,6)));
				#endregion
				#region Semaine
				case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
					AtomicPeriodWeek week=new AtomicPeriodWeek(new DateTime(int.Parse(dayDate.Substring(0,4)),int.Parse(dayDate.Substring(4,2)),int.Parse(dayDate.Substring(6,2))));
					if(week.Week.ToString().Length==1)
						return(int.Parse(week.Year.ToString()+"0"+week.Week.ToString()));
					return(int.Parse(week.Year.ToString()+week.Week.ToString()));
				#endregion
				#region Jour
				case WebConstantes.CustomerSessions.Period.DisplayLevel.dayly:
					return(int.Parse(dayDate));
				#endregion
				default:
					throw(new WebExceptions.APPMMediaPlanRulesExcpetion("Le type de détail date n'est pas correct"));
			}
		}

		/// <summary>
		/// Obtient le nombre de mois à étendre
		/// </summary>
		/// <param name="idMedia">Identifiant du media</param>
		/// <param name="parution">date de parution</param>
		/// <param name="periodicity">périodicité</param>
		/// <returns>Nombre de mois à étendre</returns>
		/// <param name="detailPeriodType">type de détail date</param>
		private static int GetNbItemsToExtend(Int64 idMedia,int parution,int periodicity,WebConstantes.CustomerSessions.Period.DisplayLevel detailPeriodType){
			int nbItems=0,nbItemsTmp=0;
			// On obtient la parution suivante:
			int nextParution=MediaPublicationDatesSystem.GetNextPublicationDate(idMedia,parution);
			if(nextParution==0){
				#region Cas pas d'autre parution
				// Dans le cas où l'on a pas une parution on crée une date fictive
				// En fonction de la périodicité
				switch(periodicity){

					#region Annuel
					case (int)DBClassificationConstantes.Periodicity.type.annuel:
						nextParution=int.Parse(DateString.DateTimeToYYYYMMDD(DateTime.Now.AddYears(1)));
					break;
					#endregion

					#region Bimensuel
					case (int)DBClassificationConstantes.Periodicity.type.bimensuel:
						nextParution=int.Parse(DateString.DateTimeToYYYYMMDD(DateTime.Now.AddDays(15)));
					break;
					#endregion

					#region Bimestriel
					case (int)DBClassificationConstantes.Periodicity.type.bimestriel:
						nextParution=int.Parse(DateString.DateTimeToYYYYMMDD(DateTime.Now.AddMonths(2)));
					break;
					#endregion

					#region Hebdomadaire
					case (int)DBClassificationConstantes.Periodicity.type.hebdomadaire:
						nextParution=int.Parse(DateString.DateTimeToYYYYMMDD(DateTime.Now.AddDays(7)));
					break;
					#endregion

					#region Mensuel
					case (int)DBClassificationConstantes.Periodicity.type.mensuel:
						nextParution=int.Parse(DateString.DateTimeToYYYYMMDD(DateTime.Now.AddMonths(1)));
					break;
					#endregion

					#region Semestriel
					case (int)DBClassificationConstantes.Periodicity.type.semestriel:
						nextParution=int.Parse(DateString.DateTimeToYYYYMMDD(DateTime.Now.AddMonths(6)));
					break;
					#endregion

					#region Trimestriel
					case (int)DBClassificationConstantes.Periodicity.type.trimestriel:
						nextParution=int.Parse(DateString.DateTimeToYYYYMMDD(DateTime.Now.AddMonths(3)));
					break;
					#endregion

					#region Autre
					case (int)DBClassificationConstantes.Periodicity.type.cinqParAn:
					case (int)DBClassificationConstantes.Periodicity.type.huitParAn:
					case (int)DBClassificationConstantes.Periodicity.type.septParAn:
					case (int)DBClassificationConstantes.Periodicity.type.bizarre:
					case (int)DBClassificationConstantes.Periodicity.type.heure:
					case (int)DBClassificationConstantes.Periodicity.type.horsSerie:
					case (int)DBClassificationConstantes.Periodicity.type.indetermine:
					case (int)DBClassificationConstantes.Periodicity.type.minute:
					case (int)DBClassificationConstantes.Periodicity.type.ponctuel:
					case (int)DBClassificationConstantes.Periodicity.type.quotidienne:
					case (int)DBClassificationConstantes.Periodicity.type.seconde:
					case (int)DBClassificationConstantes.Periodicity.type.supplement:
					case (int)DBClassificationConstantes.Periodicity.type.test:
						nextParution=parution;
					break;
					#endregion
				}
				#endregion
			}
			#region on retire un jour à la date de parution
			if(nextParution>parution){
				string[] date=DateString.YYYYMMDDToDD_MM_YYYY(nextParution.ToString(),33).Split('/');
				nextParution=int.Parse(DateString.DateTimeToYYYYMMDD(new DateTime(int.Parse(date[2]),int.Parse(date[1]),int.Parse(date[0])).AddDays(-1)));
			}
			#endregion

			#region Calcul du nombre
			int formattedParution=GetFormattedDate(parution.ToString(),detailPeriodType);
			int formattedNextParution=GetFormattedDate(nextParution.ToString(),detailPeriodType);
			nbItemsTmp=formattedNextParution-formattedParution;
			switch(detailPeriodType){

					#region Mois
				case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
					if(nbItemsTmp>11){
						// On change d'année, on somme les différences de chaque année 
						int endYear=int.Parse(parution.ToString().Substring(0,4)+"12");
						int startYear=int.Parse(nextParution.ToString().Substring(0,4)+"00");
						nbItems=endYear-formattedParution+formattedNextParution-startYear;
					}
					else{
						// Dans la même année
						nbItems=nbItemsTmp;
					}
					break;
					#endregion

					#region Semaine
				case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
					
					// On détermine le nombre de semaine dans l'année
					int nbWeekInYear=new AtomicPeriodWeek(new DateTime(int.Parse(formattedParution.ToString().Substring(0,4)),1,1)).NumberWeekInYear;
					
					//if(nbItemsTmp>(nbWeekInYear-1)){ //Old Version
					if(int.Parse(parution.ToString().Substring(0,4))!=int.Parse(nextParution.ToString().Substring(0,4))){
						// On chage d'année, on somme les différences de chaque année 
						int endYear=int.Parse(parution.ToString().Substring(0,4)+nbWeekInYear.ToString());
						int startYear=int.Parse(nextParution.ToString().Substring(0,4)+"00");
						nbItems=endYear-formattedParution+formattedNextParution-startYear;
					}
					else{
						// Dans la même année
						nbItems=nbItemsTmp;
					}


					break;
					#endregion

					#region Jour
				case WebConstantes.CustomerSessions.Period.DisplayLevel.dayly:
					DateTime parutionDate =  new DateTime(int.Parse(formattedParution.ToString().Substring(0,4)),int.Parse(formattedParution.ToString().Substring(4,2)),int.Parse(formattedParution.ToString().Substring(6,2)));
					DateTime nextParutionDate =  new DateTime(int.Parse(formattedNextParution.ToString().Substring(0,4)),int.Parse(formattedNextParution.ToString().Substring(4,2)),int.Parse(formattedNextParution.ToString().Substring(6,2)));
					nbItems = nextParutionDate.Subtract(parutionDate).Days;;
					break;
					#endregion
			
			}
			
			#endregion


			return(nbItems);
			
		
		}
		
		/// <summary>
		/// Obtient l'identifiant du niveau
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dr">ligne de données</param>
		/// <param name="level">Niveau de détail</param>
		/// <returns>identifiant du niveau</returns>
		private static Int64 GetLevelId(WebSession webSession, DataRow dr,int level){
			return(Int64.Parse(dr[webSession.GenericMediaDetailLevel.GetColumnNameLevelId(level)].ToString()));
		}

		/// <summary>
		/// Obtient le Libellé du niveau
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dr">ligne de données</param>
		/// <param name="level">Niveau de détail</param>
		/// <returns>Libellé du niveau</returns>
		private static string GetLevelLabel(WebSession webSession, DataRow dr,int level){
			return(dr[webSession.GenericMediaDetailLevel.GetColumnNameLevelLabel(level)].ToString());
		}
		
		
		#endregion
	}
}
