#region Informations
// Auteur: G. Facon 
// Date de création: 03/06/2004 
// Date de modification: 06/07/2004 
// Date de modification: 28/04/2005 par GR
//	12/08/2005	A.Dadouch	Nom de fonctions	
#endregion

using System;
using System.Data;
using System.Collections;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.Date;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using WebException=TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Functions;


namespace TNS.AdExpress.Web.Rules.Results{

	/// <summary>
	/// Class métier de traitement des données issues de la base pour une analyse plan media.
	/// </summary>
	public class MediaPlanAnalysisRules{

		#region Plan media sans choix du détail media
		/// <summary>
		/// Fonction qui formate un DataSet en un tableau qui servira à faire un Calendrier d'actions
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Tableau Formaté</returns>
		public static object[,] GetFormattedTable(WebSession webSession){
			// Création du DataSet de résultats
			DataSet ds=MediaPlanAnalysisDataAccess.GetData(webSession);

			#region Variables
			DataTable dt=ds.Tables[0];
			Int64 oldIdVehicle=0;
			Int64 oldIdCategory=0;
			Int64 oldIdMedia=0;
			Int64 currentTotalIndex=1;
			Int64 currentVehicleIndex=2;
			Int64 currentCategoryIndex=0;
			Int64 currentLineIndex=1;
			Int64 currentCategoryPDMIndex=0;
			Int64 currentVehiclePDMIndex=0;
			bool forceEntry=true;
			AtomicPeriodWeek weekDate=null;
			double unit=0.0;
			int currentDate=0;
			int oldCurrentDate=0;
			Int64 i;
			int numberOflineToAdd=0;
			int nbCol=0;
			int nbline=0;
			int k = 0;

			//MAJ GR : Colonnes totaux par année si nécessaire
			//FIRST_PERIOD_INDEX a remplacé FrameWorkResultConstantes.MediaPlan.FIRST_PEDIOD_INDEX dans toute la fonction
			Hashtable YEARS_INDEX = new Hashtable();
			currentDate = int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
			oldCurrentDate = int.Parse(webSession.PeriodEndDate.Substring(0,4));
			int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.MediaPlan.FIRST_PEDIOD_INDEX;
			if (currentDate!=oldCurrentDate)
			{
				for(k=currentDate; k<=oldCurrentDate; k++)
				{
					YEARS_INDEX.Add(k,FIRST_PERIOD_INDEX);
					FIRST_PERIOD_INDEX++;
				}
			}
			currentDate = 0;
			oldCurrentDate = 0;
			//fin MAJ
			#endregion

			#region Compte le nombre de vehicle et category
			int nbVehicle=0;
			int nbCategory=0;
			foreach(DataRow currentRow in dt.Rows){
				if(oldIdVehicle!=(Int64)currentRow["id_vehicle"]){
					nbVehicle++;
					oldIdVehicle=(Int64)currentRow["id_vehicle"];
				}
				if(oldIdCategory!=(Int64)currentRow["id_category"]){
					nbCategory++;
					oldIdCategory=(Int64)currentRow["id_category"];
				}
			}

			// Il n'y a pas de données
			if(nbVehicle==0)return(new object[0,0]);


			oldIdVehicle=0;
			oldIdCategory=0;
			#endregion

			#region Création du tableau des mois ou semaine
			string tmpDate;
			ArrayList periodItemsList=new ArrayList();
			if(webSession.DetailPeriod==WebConstantes.CustomerSessions.Period.DisplayLevel.weekly){
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
			if(webSession.DetailPeriod==WebConstantes.CustomerSessions.Period.DisplayLevel.monthly){
				DateTime dateCurrent=new DateTime(int.Parse(webSession.PeriodBeginningDate.Substring(0,4)),int.Parse(webSession.PeriodBeginningDate.Substring(4,2)),1);
				DateTime dateEnd=new DateTime(int.Parse(webSession.PeriodEndDate.Substring(0,4)),int.Parse(webSession.PeriodEndDate.Substring(4,2)),1);
				dateEnd=dateEnd.AddMonths(1);
				while(!(dateCurrent.Month==dateEnd.Month && dateCurrent.Year==dateEnd.Year)){
					tmpDate=dateCurrent.Year.ToString();
					if(dateCurrent.Month.ToString().Length<2)tmpDate+="0"+dateCurrent.Month.ToString();
					else tmpDate+=dateCurrent.Month.ToString();
					periodItemsList.Add(tmpDate);
					dateCurrent=dateCurrent.AddMonths(1);
				}
			}
			#endregion

			#region Déclaration des tableaux
			// Nombre de colonne
			nbCol=periodItemsList.Count+FrameWorkResultConstantes.MediaPlan.FIRST_PEDIOD_INDEX+YEARS_INDEX.Count;
			// Tableau de résultat
			object[,] tab=new object[dt.Rows.Count+2+nbVehicle+nbCategory,nbCol];
			// Tableau des indexes des categories
			Int64[] tabCategoryIndex=new Int64[nbCategory+1];
			// Tableau des indexes des vehicles
			Int64[] tabVehicleIndex=new Int64[nbVehicle+1];
			#endregion

			#region Libellé des colonnes
			while(currentDate<periodItemsList.Count){
				tab[0,currentDate+FIRST_PERIOD_INDEX]=(string)periodItemsList[currentDate];
				currentDate++;
			}
			//MAJ GR
			foreach(object o in YEARS_INDEX.Keys)
			{
				tab[0,int.Parse(YEARS_INDEX[o].ToString())] = o.ToString();
			}
			//fin MAJ
			#endregion

			tab[currentTotalIndex,0]=FrameWorkResultConstantes.MediaPlan.TOTAL_STRING;
			tab[currentTotalIndex,FrameWorkResultConstantes.MediaPlan.TOTAL_COLUMN_INDEX]=(double)0.0;
			//MAJ GR : initialisation totaux années
			foreach(object o in YEARS_INDEX.Keys)
			{
				tab[currentLineIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
			}
			//fin MAJ
			#region Construction du tableau de résultat
		
			foreach(DataRow currentRow in dt.Rows)
			{
				// Nouveau Vehicle
				if(oldIdVehicle!=(Int64)currentRow["id_vehicle"]){
					// Calcul des PDM
					if(oldIdVehicle!=0){
						for(i=0;i<currentCategoryPDMIndex;i++){
							tab[tabCategoryIndex[i],FrameWorkResultConstantes.MediaPlan.PDM_COLUMN_INDEX]=(double)tab[tabCategoryIndex[i],FrameWorkResultConstantes.MediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[currentVehicleIndex,FrameWorkResultConstantes.MediaPlan.TOTAL_COLUMN_INDEX]*100.0;
						}
						tabCategoryIndex=new Int64[nbCategory+1];
						currentCategoryPDMIndex=0;
					}
					// Préparation des PDM des vehicles
					tabVehicleIndex[currentVehiclePDMIndex]=currentLineIndex+1;
					currentVehiclePDMIndex++;

					currentLineIndex++;
					tab[currentLineIndex,FrameWorkResultConstantes.MediaPlan.VEHICLE_COLUMN_INDEX]=currentRow["vehicle"].ToString();
					tab[currentLineIndex,FrameWorkResultConstantes.MediaPlan.TOTAL_COLUMN_INDEX]=(double)0.0;
					//MAJ GR : initialisation totaux années
					foreach(object o in YEARS_INDEX.Keys)
					{
						tab[currentLineIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
					}
					//fin MAJ
					currentVehicleIndex=currentLineIndex;
					oldIdVehicle=(Int64)currentRow["id_vehicle"];
					numberOflineToAdd++;
					tab[currentLineIndex,FrameWorkResultConstantes.MediaPlan.CATEGORY_COLUMN_INDEX]=null;
					tab[currentLineIndex,FrameWorkResultConstantes.MediaPlan.MEDIA_COLUMN_INDEX]=null;
				}
				// Nouvelle Category
				if(oldIdCategory!=(Int64)currentRow["id_category"]){
					//Calcul des PDM de la categorie précedente
					if(oldIdCategory!=0){
						for(i=currentCategoryIndex+1;i<=currentLineIndex;i++){
							tab[i,FrameWorkResultConstantes.MediaPlan.PDM_COLUMN_INDEX]=(double)tab[i,FrameWorkResultConstantes.MediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[currentCategoryIndex,FrameWorkResultConstantes.MediaPlan.TOTAL_COLUMN_INDEX]*100.0;
						}
					}
					// Préparation des PDM des Categories
					tabCategoryIndex[currentCategoryPDMIndex]=currentLineIndex+1;
					currentCategoryPDMIndex++;

					currentLineIndex++;
					tab[currentLineIndex,FrameWorkResultConstantes.MediaPlan.CATEGORY_COLUMN_INDEX]=currentRow["category"].ToString();
					tab[currentLineIndex,FrameWorkResultConstantes.MediaPlan.TOTAL_COLUMN_INDEX]=(double)0.0;
					//MAJ GR : initialisation totaux années
					foreach(object o in YEARS_INDEX.Keys)
					{
						tab[currentLineIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
					}
					//fin MAJ
					currentCategoryIndex=currentLineIndex;
					oldIdCategory=(Int64)currentRow["id_category"];
					numberOflineToAdd++;
					tab[currentLineIndex,FrameWorkResultConstantes.MediaPlan.VEHICLE_COLUMN_INDEX]=null;
					tab[currentLineIndex,FrameWorkResultConstantes.MediaPlan.MEDIA_COLUMN_INDEX]=null;
				}
				// Nouveau Media
				if(oldIdMedia!=(Int64)currentRow["id_media"]){
					currentLineIndex++;
					tab[currentLineIndex,FrameWorkResultConstantes.MediaPlan.MEDIA_COLUMN_INDEX]=currentRow["media"].ToString();
					tab[currentLineIndex,FrameWorkResultConstantes.MediaPlan.PERIODICITY_COLUMN_INDEX]=currentRow["id_periodicity"].ToString();
					tab[currentLineIndex,FrameWorkResultConstantes.MediaPlan.TOTAL_COLUMN_INDEX]=(double)0.0;
					//MAJ GR : initialisation totaux années
					foreach(object o in YEARS_INDEX.Keys)
					{
						tab[currentLineIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
					}
					//fin MAJ
					oldIdMedia=(Int64)currentRow["id_media"];
					currentDate=0;
					tab[currentLineIndex,FrameWorkResultConstantes.MediaPlan.VEHICLE_COLUMN_INDEX]=null;
					tab[currentLineIndex,FrameWorkResultConstantes.MediaPlan.CATEGORY_COLUMN_INDEX]=null;
				}
				try{
					while(Int64.Parse(periodItemsList[currentDate].ToString())!=(Int64.Parse(currentRow["date_num"].ToString()))){
						tab[currentLineIndex,FIRST_PERIOD_INDEX+currentDate]=false;
						currentDate++;
					}
				}
				catch(System.Exception e){
					Console.Write(e.Message);
				}
				tab[currentLineIndex,FIRST_PERIOD_INDEX+currentDate]=true;
				//tab[currentVehicleIndex,MediaPlan.FIRST_PEDIOD_INDEX+currentDate]=true;
				//tab[currentCategoryIndex,MediaPlan.FIRST_PEDIOD_INDEX+currentDate]=true;
				//tab[currentTotalIndex,MediaPlan.FIRST_PEDIOD_INDEX+currentDate]=true;
				unit=double.Parse(currentRow[SQLGenerator.GetUnitAlias(webSession)].ToString());
				tab[currentLineIndex,FrameWorkResultConstantes.MediaPlan.TOTAL_COLUMN_INDEX]=(double)tab[currentLineIndex,FrameWorkResultConstantes.MediaPlan.TOTAL_COLUMN_INDEX]+unit;
				tab[currentCategoryIndex,FrameWorkResultConstantes.MediaPlan.TOTAL_COLUMN_INDEX]=(double)tab[currentCategoryIndex,FrameWorkResultConstantes.MediaPlan.TOTAL_COLUMN_INDEX]+unit;
				tab[currentVehicleIndex,FrameWorkResultConstantes.MediaPlan.TOTAL_COLUMN_INDEX]=(double)tab[currentVehicleIndex,FrameWorkResultConstantes.MediaPlan.TOTAL_COLUMN_INDEX]+unit;
				tab[currentTotalIndex,FrameWorkResultConstantes.MediaPlan.TOTAL_COLUMN_INDEX]=(double)tab[currentTotalIndex,FrameWorkResultConstantes.MediaPlan.TOTAL_COLUMN_INDEX]+unit;
				//MAJ GR : totaux par année
				k = int.Parse(currentRow["date_num"].ToString().Substring(0,4));
				if (YEARS_INDEX.Count>0)
				{
					k = int.Parse(YEARS_INDEX[k].ToString());
					tab[currentLineIndex,k]=(double)tab[currentLineIndex,k]+unit;
					tab[currentCategoryIndex,k]=(double)tab[currentCategoryIndex,k]+unit;
					tab[currentVehicleIndex,k]=(double)tab[currentVehicleIndex,k]+unit;
					tab[currentTotalIndex,k]=(double)tab[currentTotalIndex,k]+unit;
				}
				//fin MAJ
				currentDate++;
				oldCurrentDate=currentDate;
				while(oldCurrentDate<periodItemsList.Count){
					tab[currentLineIndex,FIRST_PERIOD_INDEX+oldCurrentDate]=false;
					oldCurrentDate++;
				}
			}
			#endregion

			#region Calcul des PDM de FIN
			if(nbVehicle>0){
				// PDM des média
				if(oldIdCategory!=0){
					for(i=currentCategoryIndex+1;i<=currentLineIndex;i++){
						tab[i,FrameWorkResultConstantes.MediaPlan.PDM_COLUMN_INDEX]=(double)tab[i,FrameWorkResultConstantes.MediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[currentCategoryIndex,FrameWorkResultConstantes.MediaPlan.TOTAL_COLUMN_INDEX]*100.0;
					}
				}
				// PDM des catégories
				for(i=0;i<currentCategoryPDMIndex;i++){
					tab[tabCategoryIndex[i],FrameWorkResultConstantes.MediaPlan.PDM_COLUMN_INDEX]=(double)tab[tabCategoryIndex[i],FrameWorkResultConstantes.MediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[currentVehicleIndex,FrameWorkResultConstantes.MediaPlan.TOTAL_COLUMN_INDEX]*100.0;
				}
				// PDM des vehicles
				for(i=0;i<currentVehiclePDMIndex;i++){
					tab[tabVehicleIndex[i],FrameWorkResultConstantes.MediaPlan.PDM_COLUMN_INDEX]=(double)tab[tabVehicleIndex[i],FrameWorkResultConstantes.MediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[currentTotalIndex,FrameWorkResultConstantes.MediaPlan.TOTAL_COLUMN_INDEX]*100.0;
				}
				// PDM du Total
				tab[currentTotalIndex,FrameWorkResultConstantes.MediaPlan.PDM_COLUMN_INDEX]=(double)100.0;
			}
			#endregion

			
			#region Ecriture de fin de tableau
			nbCol=tab.GetLength(1);
			nbline=tab.GetLength(0);
			if(currentLineIndex+1<nbline)
				tab[currentLineIndex+1,0]=new FrameWorkConstantes.MemoryArrayEnd();
			#endregion

			#region traitement de la périodicité	
			for(i=1;i<nbline;i++){
				if(tab[i,0]!=null)if(tab[i,0].GetType()==typeof(FrameWorkConstantes.MemoryArrayEnd)) break;
				// C'est une ligne Vehicle
				if(tab[i,FrameWorkResultConstantes.MediaPlan.VEHICLE_COLUMN_INDEX]!=null)currentVehicleIndex=i;
				// C'est une ligne Category
				if(tab[i,FrameWorkResultConstantes.MediaPlan.CATEGORY_COLUMN_INDEX]!=null)currentCategoryIndex=i;
				// C'est une ligne Media
				if(tab[i,FrameWorkResultConstantes.MediaPlan.MEDIA_COLUMN_INDEX]!=null){
					for(int j=FIRST_PERIOD_INDEX;j<nbCol;j++){
						if((bool)tab[i,j]==true){
							tab[i,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present;
							tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present;
							tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present;
							tab[FrameWorkResultConstantes.MediaPlan.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present;
							if(webSession.DetailPeriod==Constantes.Web.CustomerSessions.Period.DisplayLevel.monthly){
								switch(int.Parse(tab[i,FrameWorkResultConstantes.MediaPlan.PERIODICITY_COLUMN_INDEX].ToString())){
									#region Trimestirel
									case (int)DBClassificationConstantes.Periodicity.type.trimestriel:
										j++;
										if(j<nbCol){
											tab[i,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
											if(tab[FrameWorkResultConstantes.MediaPlan.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.MediaPlan.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.MediaPlan.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
											if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
											if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
										}
										j++;
										if(j<nbCol){
											tab[i,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
											if(tab[FrameWorkResultConstantes.MediaPlan.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.MediaPlan.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.MediaPlan.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
											if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
											if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
										}
										break;
									#endregion

									#region Bimestriel
									case (int)DBClassificationConstantes.Periodicity.type.bimestriel:
										j++;
										if(j<nbCol){
											tab[i,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
											if(tab[FrameWorkResultConstantes.MediaPlan.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.MediaPlan.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.MediaPlan.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
											if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
											if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
										}
										break;
									#endregion

									default:
										break;
								}
							}
							else{
								switch(int.Parse(tab[i,FrameWorkConstantes.Results.MediaPlan.PERIODICITY_COLUMN_INDEX].ToString())){
									#region Mensuel
									case (int)DBClassificationConstantes.Periodicity.type.mensuel:
										j++;
										if(j<nbCol){
											weekDate=new AtomicPeriodWeek(int.Parse(tab[0,j].ToString().Substring(0,4)),int.Parse(tab[0,j].ToString().Substring(4,2)));
											forceEntry=true;
											while((weekDate.FirstDay.Day<weekDate.LastDay.Day && !((int)weekDate.FirstDay.DayOfWeek ==1 && weekDate.FirstDay.Day==1)) || forceEntry){
												forceEntry=false;
												if(j<nbCol){
													tab[i,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
													if(tab[FrameWorkResultConstantes.MediaPlan.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.MediaPlan.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.MediaPlan.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
													if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
													if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
												}
												j++;
												if(j<nbCol) weekDate=new AtomicPeriodWeek(int.Parse(tab[0,j].ToString().Substring(0,4)),int.Parse(tab[0,j].ToString().Substring(4,2)));
												else break;
											}
											if(j>FIRST_PERIOD_INDEX)j--;
										}
										break;
									#endregion

									#region Bimensuel
									case (int)DBClassificationConstantes.Periodicity.type.bimensuel:
										j++;
										if(j<nbCol){
											tab[i,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
											if(tab[FrameWorkResultConstantes.MediaPlan.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.MediaPlan.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.MediaPlan.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
											if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
											if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
										}
										break;
									#endregion

									#region trimestriel
									case (int)DBClassificationConstantes.Periodicity.type.trimestriel:
										j++;
										if(j<nbCol){
											weekDate=new AtomicPeriodWeek(int.Parse(tab[0,j].ToString().Substring(0,4)),int.Parse(tab[0,j].ToString().Substring(4,2)));
											for(int l=0;l<3;l++){
												forceEntry=true;
												while((weekDate.FirstDay.Day<weekDate.LastDay.Day && !((int)weekDate.FirstDay.DayOfWeek ==1 && weekDate.FirstDay.Day==1)) || forceEntry){
													forceEntry=false;
													if(j<nbCol){
														tab[i,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
														if(tab[FrameWorkResultConstantes.MediaPlan.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.MediaPlan.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.MediaPlan.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
														if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
														if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
													}
													j++;
													if(j<nbCol) weekDate=new AtomicPeriodWeek(int.Parse(tab[0,j].ToString().Substring(0,4)),int.Parse(tab[0,j].ToString().Substring(4,2)));
													else break;
												}
											}
											if(j>FIRST_PERIOD_INDEX)j--;
										}
										break;
									#endregion

									#region Bimestriel
									case (int)DBClassificationConstantes.Periodicity.type.bimestriel:
										j++;
										if(j<nbCol){
											weekDate=new AtomicPeriodWeek(int.Parse(tab[0,j].ToString().Substring(0,4)),int.Parse(tab[0,j].ToString().Substring(4,2)));
											for(int l=0;l<2;l++){
												forceEntry=true;
												while((weekDate.FirstDay.Day<weekDate.LastDay.Day && !((int)weekDate.FirstDay.DayOfWeek ==1 && weekDate.FirstDay.Day==1)) || forceEntry){
													forceEntry=false;
													if(j<nbCol){
														tab[i,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
														if(tab[FrameWorkResultConstantes.MediaPlan.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.MediaPlan.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.MediaPlan.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
														if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
														if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
													}
													j++;
													if(j<nbCol) weekDate=new AtomicPeriodWeek(int.Parse(tab[0,j].ToString().Substring(0,4)),int.Parse(tab[0,j].ToString().Substring(4,2)));
													else break;
												}
											}
											if(j>FIRST_PERIOD_INDEX)j--;
										}
										break;
									#endregion

									default:
										break;
								}
							}
						}
						else{
							tab[i,j]=FrameWorkConstantes.Results.MediaPlan.graphicItemType.absent;
						}
					}
				}
			}
			#endregion

			ds.Dispose();

			return(tab);
		}
		#endregion
		
	}
}