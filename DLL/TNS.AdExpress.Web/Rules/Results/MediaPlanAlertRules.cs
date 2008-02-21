#region Informations
// Auteur: G. Facon 
// Date de cr�ation: 03/06/2004 
// Date de modification: 07/06/2004 
// Date de modification: 29/04/2005 par GR
//	12/08/2005	A.Dadouch	Nom de fonctions	
#endregion

using System;
using System.Data;
using System.Collections;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.FrameWork.Date;
using TNS.AdExpress.Constantes.Classification;

using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebFunctions=TNS.AdExpress.Web.Functions;
using CstDB=TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Web.Rules.Results{

	/// <summary>
	/// Class m�tier de traitement des donn�es issues de la base pour une analyse plan media
	/// </summary>
	public class MediaPlanAlertRules{		

		/// <summary>
		/// Fonction qui formate un DataSet en Calendrier d'actions sur une p�riode
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="period">Periode d'�tude</param>
		/// <returns>Tableau Format�</returns>
		/// <remarks>
		/// Utilise les m�thodes:
		///		public static object[,] TNS.AdExpress.Web.Rules.Results.MediaPlanAlertRules.getFormattedTable(WebSession webSession, string beginningPeriod, string endPeriod)
		/// </remarks>
		public static object[,] GetFormattedTable(WebSession webSession, string period){
			// Cr�ation du DataSet de r�sultats
			return GetFormattedTable(webSession, period, period);
		}

		/// <summary>
		/// Fonction qui formate un DataSet en Calendrier d'actions � partie d'une date de debut et d'une date de fin:
		///		Calcul des dates de d�but et de fin de calendrier
		///		Extraction des donn�es de la BD
		///		Extraction du nombre de vbehicle et de category des donn�es renvoy�es
		///		Cr�ation d'un tableau repertoriant tous les jours
		///		Initialisation des tableaux tab(r�sultat), tabCategoryIndex(index des lignes cat�gorie pour les pdm et les totaux), tabVehicleIndex(index des lignes m�dia pour les pdm et les totaux)
		///		Construction des libell� des p�riodes dans tab
		///		Remplissage du tableau avec les chiffres (totaux) pour les supports et calculs des totaux vehicle et categorie(1 ligne = 1 vehicle OU 1cat�gorie OU 1 support)
		///		Calcul des PDM vehicle, support et cat�gorie et total
		///		Calcul de la p�riodicit�
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="beginningPeriod">Date de d�but</param>
		/// <param name="endPeriod">Date de fin</param>
		/// <returns>Tableau Format�</returns>
		/// <remarks>
		/// Utilise les m�thodes:
		///		public static DateTime TNS.AdExpress.Web.Functions.Dates.getPeriodBeginningDate(string period, TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type periodType)
		///		public static DateTime TNS.AdExpress.Web.Functions.Dates.getPeriodEndDate(string period, TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type periodType)
		///		public static DataSet TNS.AdExpress.Web.DataAccess.Results.MediaPlanAlertDataAccess.getPluriMediaDataset(WebSession webSession, string beginningDate, string endDate)
		/// </remarks>
		public static object[,] GetFormattedTable(WebSession webSession, string beginningPeriod, string endPeriod){
			
			#region Formattage des dates sur 8 chiffres
			string periodBeginning = WebFunctions.Dates.getPeriodBeginningDate(beginningPeriod, webSession.PeriodType).ToString("yyyyMMdd");
			string periodEnd = WebFunctions.Dates.getPeriodEndDate(endPeriod, webSession.PeriodType).ToString("yyyyMMdd");
			#endregion

			#region Chargement des donn�es � partir de la base				
			DataSet ds;
			ds=MediaPlanAlertDataAccess.GetPluriMediaDataset(webSession, periodBeginning, periodEnd);
			#endregion
		
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
			int nbDays=0;
			int nbMonth=0;
			int nbline;
			double unit=0.0;
			int currentDate=0;
			int oldCurrentDate=0;
			Int64 i;
			int numberOflineToAdd=0;
			int k = 0;
			#endregion

			#region Ann�e
			//MAJ GR : Colonnes totaux par ann�e si n�cessaire
			//FIRST_PERIOD_INDEX a remplac� FrameWorkResultConstantes.MediaPlan.FIRST_PEDIOD_INDEX dans toute la fonction
			Hashtable YEARS_INDEX = new Hashtable();
			currentDate = int.Parse(periodBeginning.Substring(0,4));
			oldCurrentDate = int.Parse(periodEnd.Substring(0,4));
			int FIRST_PERIOD_INDEX = FrameWorkConstantes.Results.MediaPlanAlert.FIRST_PEDIOD_INDEX;
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

			//string tmp="";
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
			// Il n'y a pas de donn�es
			if(nbVehicle==0)return(new object[0,0]);

			oldIdVehicle=0;
			oldIdCategory=0;
			#endregion

			#region Cr�ation du tableau des jours
			ArrayList periodItemsList=new ArrayList();
			DateTime currentDateTime =  WebFunctions.Dates.getPeriodBeginningDate(beginningPeriod, webSession.PeriodType);
			DateTime endDate = WebFunctions.Dates.getPeriodEndDate(endPeriod, webSession.PeriodType);
			while(currentDateTime<=endDate){
				periodItemsList.Add(currentDateTime);
				currentDateTime = currentDateTime.AddDays(1);
			}
			#endregion

			#region D�claration des tableaux
			// Nombre de colonne
			int nbCol=periodItemsList.Count+FrameWorkConstantes.Results.MediaPlanAlert.FIRST_PEDIOD_INDEX+YEARS_INDEX.Count;
			// Tableau de r�sultat
			object[,] tab=new object[dt.Rows.Count+2+nbVehicle+nbCategory,nbCol];
			// Tableau des indexes des categories
			Int64[] tabCategoryIndex=new Int64[nbCategory+1];
			// Tableau des indexes des vehicles
			Int64[] tabVehicleIndex=new Int64[nbVehicle+1];
			#endregion

			#region Libell� des colonnes
			currentDate = 0;
			while(currentDate<periodItemsList.Count){
				tab[0,currentDate+FIRST_PERIOD_INDEX]= (DateTime)periodItemsList[currentDate];
				currentDate++;
			}
			//MAJ GR : entetes de tableau=ann�e
			foreach(object o in YEARS_INDEX.Keys)
			{
				tab[0,int.Parse(YEARS_INDEX[o].ToString())] = o.ToString();
			}
			#endregion


			#region initialisation totaux ann�es
			foreach(object o in YEARS_INDEX.Keys)
			{
				tab[currentLineIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
				tab[currentTotalIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
			}
			//fin MAJ
			tab[currentTotalIndex,0]=FrameWorkConstantes.Results.MediaPlanAlert.TOTAL_STRING;
			tab[currentTotalIndex,FrameWorkConstantes.Results.MediaPlanAlert.TOTAL_COLUMN_INDEX]=(double)0.0;
			#endregion

			#region Construction du tableau de r�sultat

			foreach(DataRow currentRow in dt.Rows)
			{
				// Nouveau Vehicle
				if(oldIdVehicle!=(Int64)currentRow["id_vehicle"]){
					// Calcul des PDM
					if(oldIdVehicle!=0){
						for(i=0;i<currentCategoryPDMIndex;i++){
							tab[tabCategoryIndex[i],FrameWorkConstantes.Results.MediaPlanAlert.PDM_COLUMN_INDEX]=(double)tab[tabCategoryIndex[i],FrameWorkConstantes.Results.MediaPlanAlert.TOTAL_COLUMN_INDEX]/(double)tab[currentVehicleIndex,FrameWorkConstantes.Results.MediaPlanAlert.TOTAL_COLUMN_INDEX]*100.0;
						}
						tabCategoryIndex=new Int64[nbCategory+1];
						currentCategoryPDMIndex=0;
					}
					// Pr�paration des PDM des vehicles
					tabVehicleIndex[currentVehiclePDMIndex]=currentLineIndex+1;
					currentVehiclePDMIndex++;

					currentLineIndex++;
					tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.VEHICLE_COLUMN_INDEX]=currentRow["vehicle"].ToString();
					tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.TOTAL_COLUMN_INDEX]=(double)0.0;
					//MAJ GR : initialisation totaux ann�es
					foreach(object o in YEARS_INDEX.Keys) {
						tab[currentLineIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
					}
					//fin MAJ
					currentVehicleIndex=currentLineIndex;
					oldIdVehicle=(Int64)currentRow["id_vehicle"];
					numberOflineToAdd++;
					tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.CATEGORY_COLUMN_INDEX]=null;
					tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.MEDIA_COLUMN_INDEX]=null;
					tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX]=null;
					tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.PERIODICITY_COLUMN_INDEX]=null;										

					tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]=(Int64)currentRow["id_vehicle"];
					tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX]=(Int64)currentRow["id_category"];
					tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX]=(Int64)currentRow["id_media"];
					if(oldIdVehicle==(Int64)Branch.type.mediaOutdoor){
						if (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG)!=null )
							tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]=(Int64)currentRow["id_vehicle"];
						else 
							tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]=null;
					}
				}
				
				// Nouvelle Category
				if(oldIdCategory!=(Int64)currentRow["id_category"]){
					//Calcul des PDM de la categorie pr�cedente
					if(oldIdCategory!=0){
						for(i=currentCategoryIndex+1;i<=currentLineIndex;i++){
							tab[i,FrameWorkConstantes.Results.MediaPlanAlert.PDM_COLUMN_INDEX]=(double)tab[i,FrameWorkConstantes.Results.MediaPlanAlert.TOTAL_COLUMN_INDEX]/(double)tab[currentCategoryIndex,FrameWorkConstantes.Results.MediaPlanAlert.TOTAL_COLUMN_INDEX]*100.0;
						}
					}
					// Pr�paration des PDM des Categories
					tabCategoryIndex[currentCategoryPDMIndex]=currentLineIndex+1;
					currentCategoryPDMIndex++;

					currentLineIndex++;
					tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.CATEGORY_COLUMN_INDEX]=currentRow["category"].ToString();
					tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.TOTAL_COLUMN_INDEX]=(double)0.0;
					//MAJ GR : initialisation totaux ann�es
					foreach(object o in YEARS_INDEX.Keys)
					{
						tab[currentLineIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
					}
					//fin MAJ
					currentCategoryIndex=currentLineIndex;
					oldIdCategory=(Int64)currentRow["id_category"];
					numberOflineToAdd++;
					tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.VEHICLE_COLUMN_INDEX]=null;
					tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.MEDIA_COLUMN_INDEX]=null;
					tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX]=null;
					tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.PERIODICITY_COLUMN_INDEX]=null;

					tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]=(Int64)currentRow["id_vehicle"];
					if ((oldIdCategory!=35 && oldIdVehicle!=(Int64)Branch.type.mediaOutdoor) || (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG)!=null && oldIdVehicle==(Int64)Branch.type.mediaOutdoor))
						tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX]=(Int64)currentRow["id_category"];
					else
						tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX]=null;
						tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX]=(Int64)currentRow["id_media"];;

				}
				// Nouveau Media
				if(oldIdMedia!=(Int64)currentRow["id_media"]){
					currentLineIndex++;
					tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.MEDIA_COLUMN_INDEX]=currentRow["media"].ToString();
					tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.TOTAL_COLUMN_INDEX]=(double)0.0;
					//MAJ GR : initialisation totaux ann�es
					foreach(object o in YEARS_INDEX.Keys)
					{
						tab[currentLineIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
					}
					//fin MAJ

					oldIdMedia=(Int64)currentRow["id_media"];
					currentDate=0;
					tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.VEHICLE_COLUMN_INDEX]=null;
					tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.CATEGORY_COLUMN_INDEX]=null;

					tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]=currentRow["id_vehicle"].ToString();
					tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX]=currentRow["id_category"].ToString();

					//chaine th�matique??
					if ((oldIdCategory!=35 && oldIdVehicle!=(Int64)Branch.type.mediaOutdoor) || (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG)!=null && oldIdVehicle==(Int64)Branch.type.mediaOutdoor))
						tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX]=currentRow["id_media"].ToString();
					else
						tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX]=null;
					//P�riodicit� Hebdomadaire pour OutDoor
					if(currentRow["id_vehicle"].ToString().Equals(DBClassificationConstantes.Vehicles.names.outdoor.GetHashCode().ToString())
						&& currentRow["id_periodicity"].ToString().Equals(DBClassificationConstantes.Periodicity.type.indetermine.GetHashCode().ToString())						
						){
						tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.PERIODICITY_COLUMN_INDEX]=DBClassificationConstantes.Periodicity.type.hebdomadaire.GetHashCode().ToString();
					}else
					tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.PERIODICITY_COLUMN_INDEX]=currentRow["id_periodicity"].ToString();
				}
				try{

					while(((DateTime)periodItemsList[currentDate]).ToString("yyyyMMdd")!=currentRow["date_num"].ToString()){
						tab[currentLineIndex,FIRST_PERIOD_INDEX+currentDate]=false;
						currentDate++;
					}
				}
				catch(System.Exception e){
					Console.Write(e.Message);
				}
				tab[currentLineIndex,FIRST_PERIOD_INDEX+currentDate]=true;
				//				tab[currentVehicleIndex,FrameWorkConstantes.Results.ZoomMediaPlan.FIRST_PEDIOD_INDEX+currentDate]=true;
				//				tab[currentCategoryIndex,FrameWorkConstantes.Results.ZoomMediaPlan.FIRST_PEDIOD_INDEX+currentDate]=true;
				//				tab[currentTotalIndex,FrameWorkConstantes.Results.ZoomMediaPlan.FIRST_PEDIOD_INDEX+currentDate]=true;
				unit=double.Parse(currentRow["unit"].ToString());
				tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.TOTAL_COLUMN_INDEX]=(double)tab[currentLineIndex,FrameWorkConstantes.Results.MediaPlanAlert.TOTAL_COLUMN_INDEX]+unit;
				tab[currentCategoryIndex,FrameWorkConstantes.Results.MediaPlanAlert.TOTAL_COLUMN_INDEX]=(double)tab[currentCategoryIndex,FrameWorkConstantes.Results.MediaPlanAlert.TOTAL_COLUMN_INDEX]+unit;
				tab[currentVehicleIndex,FrameWorkConstantes.Results.MediaPlanAlert.TOTAL_COLUMN_INDEX]=(double)tab[currentVehicleIndex,FrameWorkConstantes.Results.MediaPlanAlert.TOTAL_COLUMN_INDEX]+unit;
				tab[currentTotalIndex,FrameWorkConstantes.Results.MediaPlanAlert.TOTAL_COLUMN_INDEX]=(double)tab[currentTotalIndex,FrameWorkConstantes.Results.MediaPlanAlert.TOTAL_COLUMN_INDEX]+unit;
				//MAJ GR : totaux par ann�e
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
				// PDM des m�dia
				if(oldIdCategory!=0){
					for(i=currentCategoryIndex+1;i<=currentLineIndex;i++){
						tab[i,FrameWorkConstantes.Results.MediaPlanAlert.PDM_COLUMN_INDEX]=(double)tab[i,FrameWorkConstantes.Results.MediaPlanAlert.TOTAL_COLUMN_INDEX]/(double)tab[currentCategoryIndex,FrameWorkConstantes.Results.MediaPlanAlert.TOTAL_COLUMN_INDEX]*100.0;
					}
				}
				// PDM des cat�gories
				for(i=0;i<currentCategoryPDMIndex;i++){
					tab[tabCategoryIndex[i],FrameWorkConstantes.Results.MediaPlanAlert.PDM_COLUMN_INDEX]=(double)tab[tabCategoryIndex[i],FrameWorkConstantes.Results.MediaPlanAlert.TOTAL_COLUMN_INDEX]/(double)tab[currentVehicleIndex,FrameWorkConstantes.Results.MediaPlanAlert.TOTAL_COLUMN_INDEX]*100.0;
				}
				// PDM des vehicles
				for(i=0;i<currentVehiclePDMIndex;i++){
					tab[tabVehicleIndex[i],FrameWorkConstantes.Results.MediaPlanAlert.PDM_COLUMN_INDEX]=(double)tab[tabVehicleIndex[i],FrameWorkConstantes.Results.MediaPlanAlert.TOTAL_COLUMN_INDEX]/(double)tab[currentTotalIndex,FrameWorkConstantes.Results.MediaPlanAlert.TOTAL_COLUMN_INDEX]*100.0;
				}
				// PDM du Total
				tab[currentTotalIndex,FrameWorkConstantes.Results.MediaPlanAlert.PDM_COLUMN_INDEX]=(double)100.0;
			}
			#endregion

			#region Ecriture de fin de tableau
			nbCol=tab.GetLength(1);
			nbline=tab.GetLength(0);
			if(currentLineIndex+1<nbline)
				tab[currentLineIndex+1,0]=new TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd();
			#endregion

			#region traitement de la p�riodicit�	
			for(i=1;i<nbline;i++){
				if(tab[i,0]!=null)if(tab[i,0].GetType()==typeof(FrameWorkConstantes.MemoryArrayEnd)) break;
				// C'est une ligne Vehicle
				if(tab[i,FrameWorkResultConstantes.MediaPlanAlert.VEHICLE_COLUMN_INDEX]!=null)currentVehicleIndex=i;
				// C'est une ligne Category
				if(tab[i,FrameWorkResultConstantes.MediaPlanAlert.CATEGORY_COLUMN_INDEX]!=null)currentCategoryIndex=i;
				// C'est une ligne Media
				if(tab[i,FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX]!=null){
					for(int j=FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX;j<nbCol;j++){
						if((bool)tab[i,j]==true){
							tab[i,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.present;
							tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.present;
							tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.present;
							tab[FrameWorkResultConstantes.MediaPlanAlert.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.present;
							switch(int.Parse(tab[i,FrameWorkResultConstantes.MediaPlanAlert.PERIODICITY_COLUMN_INDEX].ToString())){
								case (int)DBClassificationConstantes.Periodicity.type.mensuel:
									j++;
									while (j<nbCol && int.Parse(((DateTime)tab[FrameWorkConstantes.Results.MediaPlanAlert.PERIOD_LINE_INDEX,j]).ToString("dd"))!=1){
										tab[i,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.extended;
										if(tab[FrameWorkResultConstantes.MediaPlanAlert.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.MediaPlanAlert.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.MediaPlanAlert.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.extended;
										if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.extended;
										if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.extended;										
										j++;
									}
									j--;
									break;
								case (int)DBClassificationConstantes.Periodicity.type.bimestriel:
									j++;
									nbMonth=0;
									while(j<nbCol && nbMonth<DBClassificationConstantes.Periodicity.MONTH_NUMBER_IN_BIMONTHLY){
										while (j<nbCol && int.Parse(((DateTime)tab[FrameWorkConstantes.Results.MediaPlanAlert.PERIOD_LINE_INDEX,j]).ToString("dd"))!=1){
											tab[i,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.extended;
											if(tab[FrameWorkResultConstantes.MediaPlanAlert.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.MediaPlanAlert.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.MediaPlanAlert.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.extended;
											if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.extended;
											if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.extended;										
											j++;
										}
										nbMonth++;
									}
									j--;
									break;
								case (int)DBClassificationConstantes.Periodicity.type.trimestriel:
									j++;
									nbMonth=0;
									while(nbMonth<DBClassificationConstantes.Periodicity.MONTH_NUMBER_IN_QUATERLY){
										while (j<nbCol && int.Parse(((DateTime)tab[FrameWorkConstantes.Results.MediaPlanAlert.PERIOD_LINE_INDEX,j]).ToString("dd"))!=1){
											tab[i,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.extended;
											if(tab[FrameWorkResultConstantes.MediaPlanAlert.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.MediaPlanAlert.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.MediaPlanAlert.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.extended;
											if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.extended;
											if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.extended;										
											j++;
										}
										nbMonth++;
									}
									j--;
									break;
								case (int)DBClassificationConstantes.Periodicity.type.hebdomadaire:
									j++;
									nbDays=0;
									while(j<nbCol && nbDays<DBClassificationConstantes.Periodicity.DAY_NUMBER_IN_WEEK-1){
										tab[i,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.extended;
										if(tab[FrameWorkResultConstantes.MediaPlanAlert.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.MediaPlanAlert.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.MediaPlanAlert.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.extended;
										if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))
											tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.extended;
										if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))
											tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.extended;										
										nbDays++;
										j++;
									}
									j--;
									break;									
								case (int)DBClassificationConstantes.Periodicity.type.bimensuel:
									j++;
									nbDays=0;
									while(j<nbCol && nbDays<DBClassificationConstantes.Periodicity.DAY_NUMBER_IN_BIWEEKLY-1){
										tab[i,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.extended;
										if(tab[FrameWorkResultConstantes.MediaPlanAlert.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.MediaPlanAlert.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.MediaPlanAlert.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.extended;
										if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.extended;
										if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.extended;										
										nbDays++;
										j++;
									}
									j--;
									break;
								default:
									break;
							}
						}
						else{
							tab[i,j]=FrameWorkConstantes.Results.MediaPlanAlert.graphicItemType.absent;
						}
					}
				}
			}
			#endregion

			ds.Dispose();

			#region Debug: Voir le tableau
			string HTML1="<html><table><tr>";
			for(int z=0;z<=currentLineIndex;z++){
				for(int r=0;r<nbCol;r++){
					if(tab[z,r]!=null)HTML1+="<td>"+tab[z,r].ToString()+"</td>";
					else HTML1+="<td>&nbsp;</td>";
				}
				HTML1+="</tr><tr>";
			}
			HTML1+="</tr></table></html>";
			#endregion

			return(tab);
			}
		
		
	}
}