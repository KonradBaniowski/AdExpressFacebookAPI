#region Information
// Auteur : A.Obermeyer
// Création : 
// Modification:
//	12/08/2005	A.Dadouch	Nom de fonctions		
#endregion


using System;
using System.Data;
using System.Collections;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.FrameWork.Date;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using VehicleClassification=TNS.AdExpress.Constantes.Classification;
using TNS.AdExpress.Web.Exceptions;

namespace TNS.AdExpress.Web.Rules.Results{
	/// <summary>
	/// Class métier de traitement des données issues de la base pour une analyse plan media concurentiel.
	/// </summary>
	public class CompetitorMediaPlanAnalysisRules{

		/// <summary>
		/// Fonction qui formate un DataSet en un tableau qui servira à faire un Calendrier d'actions
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Tableau Formaté</returns>
		public static object[,] GetFormattedTable(WebSession webSession){
			// Création du DataSet de résultats
			DataSet ds=null;
			try{
				ds=CompetitorMediaPlanAnalysisDataAccess.GetData(webSession);
			}
			catch(System.Exception err){
				throw(new CompetitorMediaPlanAnalysisRulesException("Impossible de charger les données pour le plan media concurrentiel",err));
			}

			#region Variables
			DataTable dt=ds.Tables[0];
			Int64 oldIdVehicle=0;
			Int64 oldIdCategory=0;
			Int64 oldIdMedia=0;
			Int64 oldIdAdvertiser=-1;//0 MAJ DM

			Int64 currentTotalIndex=1;
			
			
			Int64 currentCategoryPDMIndex=0;
			Int64 currentVehiclePDMIndex=0;
			Int64 currentMediaPDMIndex=0;
			bool forceEntry=true;
			AtomicPeriodWeek weekDate=null;
			double unit=0.0;
			int currentDate=0;
			int oldCurrentDate=0;
			Int64 i;
			int numberOflineToAdd=0;
			int nbCol=0;
			int nbline=0;
			Int64 k=1;

			//MAJ GR : Colonnes totaux par année si nécessaire
			//FIRST_PERIOD_INDEX a remplacé FrameWorkResultConstantes.MediaPlan.FIRST_PEDIOD_INDEX dans toute la fonction
			Hashtable YEARS_INDEX = new Hashtable();
			int c = 0;
			currentDate = int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
			oldCurrentDate = int.Parse(webSession.PeriodEndDate.Substring(0,4));
			int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.CompetitorMediaPlan.FIRST_PEDIOD_INDEX;
			if (currentDate!=oldCurrentDate)
			{
				for(c=currentDate; c<=oldCurrentDate; c++)
				{
					YEARS_INDEX.Add(c,FIRST_PERIOD_INDEX);
					FIRST_PERIOD_INDEX++;
				}
			}
			currentDate = 0;
			oldCurrentDate = 0;
			//fin MAJ

			#endregion

			#region Compte le nombre de vehicle de category et de média
			int nbVehicle=0;
			int nbCategory=0;
			int nbMedia=0;		 
			// Univers pour le total
			Hashtable nbAdervertiser=new Hashtable();
			foreach(DataRow currentRow in dt.Rows){
				if(oldIdVehicle!=Int64.Parse(currentRow["id_vehicle"].ToString())){
					nbVehicle++;
					oldIdVehicle=Int64.Parse(currentRow["id_vehicle"].ToString());
				}
				if(oldIdCategory!=Int64.Parse(currentRow["id_category"].ToString())){
					nbCategory++;
					oldIdCategory=Int64.Parse(currentRow["id_category"].ToString());
				}

				if(oldIdMedia!=Int64.Parse(currentRow["id_media"].ToString())){
					nbMedia++;
					oldIdMedia=Int64.Parse(currentRow["id_media"].ToString());
				}

				if(oldIdAdvertiser!=Int64.Parse(currentRow["grpunivers"].ToString())){
					switch(Int64.Parse(currentRow["id_vehicle"].ToString())){
						case (Int64)DBClassificationConstantes.Vehicles.names.internationalPress:
							if(nbAdervertiser[Int64.Parse(currentRow["grpunivers"].ToString())]==null){
								nbAdervertiser.Add(Int64.Parse(currentRow["grpunivers"].ToString()),currentRow["grp"].ToString());
							}
							break;
						case (Int64)DBClassificationConstantes.Vehicles.names.press:
							if(nbAdervertiser[Int64.Parse(currentRow["grpunivers"].ToString())]==null){
								nbAdervertiser.Add(Int64.Parse(currentRow["grpunivers"].ToString()),currentRow["grp"].ToString());
							}
							break;
						case (Int64)DBClassificationConstantes.Vehicles.names.radio:
							if(nbAdervertiser[Int64.Parse(currentRow["grpunivers"].ToString())]==null){
								nbAdervertiser.Add(Int64.Parse(currentRow["grpunivers"].ToString()),currentRow["grp"].ToString());
							}
							break;
						case (Int64)DBClassificationConstantes.Vehicles.names.others:
						case (Int64)DBClassificationConstantes.Vehicles.names.tv:
							if(nbAdervertiser[Int64.Parse(currentRow["grpunivers"].ToString())]==null){
								nbAdervertiser.Add(Int64.Parse(currentRow["grpunivers"].ToString()),currentRow["grp"].ToString());
							}
							break;
						default:
							break;
			
					}
					oldIdAdvertiser=Int64.Parse(currentRow["grpunivers"].ToString());
				}
			}			

			//for(k=1;k<=5;k++){
			//    if(nbAdervertiser[k]==null){
			//        if(webSession.CompetitorUniversAdvertiser[(int)k]!=null){
			//            nbAdervertiser.Add(k,((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[(int)k]).NameCompetitorAdvertiser);
			//        }
			//    }
			//}
			for (k = 0; k < 5; k++) {
				if (nbAdervertiser[k] == null) {
					if (webSession.PrincipalProductUniverses.ContainsKey((int)k) && webSession.PrincipalProductUniverses[(int)k] != null) {
						nbAdervertiser.Add(k, webSession.PrincipalProductUniverses[(int)k].Label);
					}
				}
			}


			// Il n'y a pas de données
			if(nbVehicle==0)return(new object[0,0]);


			oldIdVehicle=0;
			oldIdCategory=0;
			oldIdMedia=0;
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
			nbCol=periodItemsList.Count+FrameWorkResultConstantes.CompetitorMediaPlan.FIRST_PEDIOD_INDEX+YEARS_INDEX.Count;
			// Tableau de résultat
			object[,] tab=new object[dt.Rows.Count+2+nbVehicle+nbCategory+nbMedia+nbCategory*nbAdervertiser.Count+nbVehicle*nbAdervertiser.Count+nbAdervertiser.Count,nbCol];
			// Tableau des indexes des categories
			Int64[] tabCategoryIndex=new Int64[nbCategory+1];
			// Tableau des indexes des vehicles
			Int64[] tabVehicleIndex=new Int64[nbVehicle+1];
			// Tableau des indexes des medias
			Int64[] tabMediaIndex=new Int64[nbMedia+1];
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

			//tab[currentTotalIndex,0]=FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_STRING;
			tab[currentTotalIndex, FrameWorkResultConstantes.CompetitorMediaPlan.VEHICLE_COLUMN_INDEX] = FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_STRING;
			tab[currentTotalIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]=(double)0.0;
			//MAJ GR : initialisation totaux années
			foreach(object o in YEARS_INDEX.Keys){
				tab[currentTotalIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
			}
			//fin MAJ			
			
			Int64 j=1;
			for(j=1;j<=nbAdervertiser.Count;j++){
				tab[currentTotalIndex+j,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]=(double)0.0;
				//tab[currentTotalIndex+j,0]=nbAdervertiser[j].ToString();
				//tab[currentTotalIndex + j, 0] = nbAdervertiser[j-1].ToString();//MAJ DM
				tab[currentTotalIndex + j, FrameWorkResultConstantes.CompetitorMediaPlan.VEHICLE_COLUMN_INDEX] = nbAdervertiser[j - 1].ToString();//MAJ DM 
				//MAJ GR : initialisation totaux années
				foreach(object o in YEARS_INDEX.Keys){
					tab[currentTotalIndex+j,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
				}
				//fin MAJ			
			}
			
			Int64 currentVehicleIndex=2+nbAdervertiser.Count;
			Int64 currentCategoryIndex=3+nbAdervertiser.Count;
			Int64 currentLineIndex=1+nbAdervertiser.Count;
			Int64 currentMediaIndex=4+nbAdervertiser.Count;

			#region Construction du tableau de résultat
			foreach(DataRow currentRow in dt.Rows){
								
				// Nouveau Vehicle
				if(oldIdVehicle!=Int64.Parse(currentRow["id_vehicle"].ToString())){
					// Calcul des PDM des catégories du vehicle précédent
					if(oldIdVehicle!=0){
						for(i=0;i<currentCategoryPDMIndex;i++){
							tab[tabCategoryIndex[i],FrameWorkResultConstantes.CompetitorMediaPlan.PDM_COLUMN_INDEX]=(double)tab[tabCategoryIndex[i],FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[currentVehicleIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]*100.0;
						}
						tabCategoryIndex=new Int64[nbCategory+1];
						currentCategoryPDMIndex=0;
						//Calcul des PDM des groupes du vehicles précédent
						for(k=1;k<=nbAdervertiser.Count;k++){
							tab[currentVehicleIndex+k,FrameWorkResultConstantes.CompetitorMediaPlan.PDM_COLUMN_INDEX]=(double)tab[currentVehicleIndex+k,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[currentVehicleIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]*100.0;
						}
					}	
					// Préparation des PDM des vehicles
					tabVehicleIndex[currentVehiclePDMIndex]=currentLineIndex+1;
					currentVehiclePDMIndex++;

					currentLineIndex++;
					tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlan.ID_VEHICLE_COLUMN_INDEX] = currentRow["id_vehicle"].ToString();//Ajouté par DM 31012008
					tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlan.VEHICLE_COLUMN_INDEX]=currentRow["vehicle"].ToString();
					tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]=(double)0.0;
					//MAJ GR : initialisation totaux années
					foreach(object o in YEARS_INDEX.Keys)
					{
						tab[currentLineIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
					}
					//fin MAJ
					currentVehicleIndex=currentLineIndex;
					oldIdVehicle=Int64.Parse(currentRow["id_vehicle"].ToString());
					numberOflineToAdd++;
					tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlan.CATEGORY_COLUMN_INDEX]=null;
					tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlan.MEDIA_COLUMN_INDEX]=null;
					tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX]=null;
				
				
				
					
					// Totaux Annonceur au niveau de la categorie
					for(k=1;k<=nbAdervertiser.Count;k++){
						//tab[currentLineIndex+k,FrameWorkResultConstantes.CompetitorMediaPlan.VEHICLE_COLUMN_INDEX]=nbAdervertiser[k].ToString();
						tab[currentLineIndex + k, FrameWorkResultConstantes.CompetitorMediaPlan.ID_VEHICLE_COLUMN_INDEX] = currentRow["id_vehicle"].ToString();//Ajouté par DM 31012008
						tab[currentLineIndex + k, FrameWorkResultConstantes.CompetitorMediaPlan.VEHICLE_COLUMN_INDEX] = nbAdervertiser[k-1].ToString();//MAJ DM
						tab[currentLineIndex+k,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]=(double)0.0;
						tab[currentLineIndex+k,FrameWorkResultConstantes.CompetitorMediaPlan.CATEGORY_COLUMN_INDEX]=null;
						tab[currentLineIndex+k,FrameWorkResultConstantes.CompetitorMediaPlan.MEDIA_COLUMN_INDEX]=null;
						tab[currentLineIndex+k,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX]=null;
						//MAJ GR : initialisation totaux années
						foreach(object o in YEARS_INDEX.Keys)
						{
							tab[currentLineIndex+k,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
						}
						//fin MAJ						
					}
					currentLineIndex=currentLineIndex+nbAdervertiser.Count;
					
				
				}
				// Nouvelle Category
				if(oldIdCategory!=Int64.Parse(currentRow["id_category"].ToString())){
					//Calcul des PDM de la categorie précedente
					if(oldIdCategory!=0){
						for(i=0;i<currentMediaPDMIndex;i++){
							tab[tabMediaIndex[i],FrameWorkResultConstantes.CompetitorMediaPlan.PDM_COLUMN_INDEX]=(double)tab[tabMediaIndex[i],FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[currentCategoryIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]*100.0;
						}
						tabMediaIndex=new Int64[nbMedia+1];
						currentMediaPDMIndex=0;
						//Calcul des PDM des groupes de la catégorie précédente
						for(k=1;k<=nbAdervertiser.Count;k++){
							tab[currentCategoryIndex+k,FrameWorkResultConstantes.CompetitorMediaPlan.PDM_COLUMN_INDEX]=(double)tab[currentCategoryIndex+k,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[currentCategoryIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]*100.0;
						}
					}
					// Préparation des PDM des Categories
					tabCategoryIndex[currentCategoryPDMIndex]=currentLineIndex+1;
					currentCategoryPDMIndex++;

					currentLineIndex++;
					tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlan.CATEGORY_COLUMN_INDEX]=currentRow["category"].ToString();
					tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]=(double)0.0;
					//MAJ GR : initialisation totaux années
					foreach(object o in YEARS_INDEX.Keys)
					{
						tab[currentLineIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
					}
					//fin MAJ
					currentCategoryIndex=currentLineIndex;
					oldIdCategory=Int64.Parse(currentRow["id_category"].ToString());
					numberOflineToAdd++;
					tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlan.VEHICLE_COLUMN_INDEX]=null;
					tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlan.MEDIA_COLUMN_INDEX]=null;
					tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX]=null;
				
				
				
					 j=1;
					// Totaux Annonceur au niveau de la categorie
					for(j=1;j<=nbAdervertiser.Count;j++){
						//tab[currentLineIndex+j,FrameWorkResultConstantes.CompetitorMediaPlan.CATEGORY_COLUMN_INDEX]=nbAdervertiser[j].ToString();
						tab[currentLineIndex + j, FrameWorkResultConstantes.CompetitorMediaPlan.CATEGORY_COLUMN_INDEX] = nbAdervertiser[j-1].ToString();//MAJ DM
						tab[currentLineIndex+j,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]=(double)0.0;
						tab[currentLineIndex+j,FrameWorkResultConstantes.CompetitorMediaPlan.VEHICLE_COLUMN_INDEX]=null;
						tab[currentLineIndex+j,FrameWorkResultConstantes.CompetitorMediaPlan.MEDIA_COLUMN_INDEX]=null;
						tab[currentLineIndex+j,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX]=null;
						//MAJ GR : initialisation totaux années
						foreach(object o in YEARS_INDEX.Keys)
						{
							tab[currentLineIndex+j,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
						}
						//fin MAJ

					}
					currentLineIndex=currentLineIndex+nbAdervertiser.Count;
					
				
				
				}
				// Nouveau Media
				if(oldIdMedia!=Int64.Parse(currentRow["id_media"].ToString())){
					if(oldIdMedia!=0){
						for(i=currentMediaIndex+1;i<=currentLineIndex;i++){
							tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.PDM_COLUMN_INDEX]=(double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[currentMediaIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]*100.0;
						}
						//Calcul des PDM des groupes du média précédente
//						for(k=1;k<=nbAdervertiser.Count;k++){
//							tab[currentMediaIndex+k,FrameWorkResultConstantes.CompetitorMediaPlan.PDM_COLUMN_INDEX]=(double)tab[currentMediaIndex+k,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[currentCategoryIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]*100.0;
//						}
					}
					// Préparation des PDM des médias
					tabMediaIndex[currentMediaPDMIndex]=currentLineIndex+1;
					currentMediaPDMIndex++;

					currentLineIndex++;
					tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlan.MEDIA_COLUMN_INDEX]=currentRow["media"].ToString();
					tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]=(double)0.0;
					//MAJ GR : initialisation totaux années
					foreach(object o in YEARS_INDEX.Keys)
					{
						tab[currentLineIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
					}
					//fin MAJ

					currentMediaIndex=currentLineIndex;
					oldIdMedia=Int64.Parse(currentRow["id_media"].ToString());
					oldIdAdvertiser=-1;//0 MAJ DM
					numberOflineToAdd++;
					tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlan.VEHICLE_COLUMN_INDEX]=null;
					tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlan.CATEGORY_COLUMN_INDEX]=null;
					tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX]=null;
				
				
					
				
				}
				// Advertiser	
				if(oldIdAdvertiser!=Int64.Parse(currentRow["grpunivers"].ToString())){	
					currentLineIndex++;
					tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX]=currentRow["grp"].ToString();
					tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlan.PERIODICITY_COLUMN_INDEX]=currentRow["id_periodicity"].ToString();
					tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]=(double)0.0;
					//MAJ GR : initialisation totaux années
					foreach(object o in YEARS_INDEX.Keys)
					{
						tab[currentLineIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
					}
					//fin MAJ

					//changement oldIdMedia->oldIdAdvertiser
					
					
					
					oldIdAdvertiser=Int64.Parse(currentRow["grpunivers"].ToString());
					currentDate=0;
					tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlan.VEHICLE_COLUMN_INDEX]=null;
					tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlan.CATEGORY_COLUMN_INDEX]=null;
					tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlan.MEDIA_COLUMN_INDEX]=null;
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
				//				if(currentLineIndex>319)break;
				//				if(currentLineIndex>=tab.GetLength(0)){
				//					string h="dd";
				//				}
				//				if(currentDate>tab.GetLength(1)){
				//					string h="dd";
				//				}
				
				//tab[currentTotalIndex+Int64.Parse(currentRow["grpunivers"].ToString()),FrameWorkResultConstantes.CompetitorMediaPlan.PDM_COLUMN_INDEX]=(double)tab[currentTotalIndex+Int64.Parse(currentRow["grpunivers"].ToString()),FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]*100/(double)tab[currentTotalIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX];
				//Essai PDM
				//tab[currentVehicleIndex+Int64.Parse(currentRow["grpunivers"].ToString()),FrameWorkResultConstantes.CompetitorMediaPlan.PDM_COLUMN_INDEX]=(double)tab[currentVehicleIndex+Int64.Parse(currentRow["grpunivers"].ToString()),FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]*100/(double)tab[currentVehicleIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX];

				
				

				tab[currentLineIndex,FIRST_PERIOD_INDEX+currentDate]=true;
				tab[currentMediaIndex,FIRST_PERIOD_INDEX+currentDate]=true;
				tab[currentVehicleIndex,FIRST_PERIOD_INDEX+currentDate]=true;
				tab[currentCategoryIndex,FIRST_PERIOD_INDEX+currentDate]=true;
				tab[currentTotalIndex,FIRST_PERIOD_INDEX+currentDate]=true;
				
				// Période totaux Annonceur
				//tab[currentTotalIndex+Int64.Parse(currentRow["grpunivers"].ToString()),FIRST_PERIOD_INDEX+currentDate]=true;
				//tab[currentCategoryIndex+Int64.Parse(currentRow["grpunivers"].ToString()),FIRST_PERIOD_INDEX+currentDate]=true;
				//tab[currentVehicleIndex+Int64.Parse(currentRow["grpunivers"].ToString()),FIRST_PERIOD_INDEX+currentDate]=true;
				tab[currentTotalIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, FIRST_PERIOD_INDEX + currentDate] = true;
				tab[currentCategoryIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, FIRST_PERIOD_INDEX + currentDate] = true;
				tab[currentVehicleIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, FIRST_PERIOD_INDEX + currentDate] = true;


				unit=double.Parse(currentRow["unit"].ToString());
				tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]=(double)tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]+unit;
				tab[currentMediaIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]=(double)tab[currentMediaIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]+unit;
				tab[currentCategoryIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]=(double)tab[currentCategoryIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]+unit;
				tab[currentVehicleIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]=(double)tab[currentVehicleIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]+unit;
				tab[currentTotalIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]=(double)tab[currentTotalIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]+unit;
				//MAJ GR : totaux par année
				c = int.Parse(currentRow["date_num"].ToString().Substring(0,4));
				if (YEARS_INDEX.Count>0)
				{
					c = int.Parse(YEARS_INDEX[c].ToString());
					tab[currentLineIndex,c]=(double)tab[currentLineIndex,c]+unit;
					tab[currentMediaIndex,c]=(double)tab[currentMediaIndex,c]+unit;
					tab[currentCategoryIndex,c]=(double)tab[currentCategoryIndex,c]+unit;
					tab[currentVehicleIndex,c]=(double)tab[currentVehicleIndex,c]+unit;
					tab[currentTotalIndex,c]=(double)tab[currentTotalIndex,c]+unit;
				}
				//fin MAJ
				currentDate++;
				oldCurrentDate=currentDate;
				while(oldCurrentDate<periodItemsList.Count){
					tab[currentLineIndex,FIRST_PERIOD_INDEX+oldCurrentDate]=false;
					oldCurrentDate++;
				}

				//Essai totaux
				tab[currentCategoryIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX] = (double)tab[currentCategoryIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX] + unit;
				tab[currentVehicleIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX] = (double)tab[currentVehicleIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX] + unit;
				tab[currentTotalIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX] = (double)tab[currentTotalIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX] + unit;
				//MAJ GR : totaux par année
				c = int.Parse(currentRow["date_num"].ToString().Substring(0,4));
				if (YEARS_INDEX.Count>0)
				{
					c = int.Parse(YEARS_INDEX[c].ToString());
					tab[currentCategoryIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, c] = (double)tab[currentCategoryIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, c] + unit;
					tab[currentVehicleIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, c] = (double)tab[currentVehicleIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, c] + unit;
					tab[currentTotalIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, c] = (double)tab[currentTotalIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, c] + unit;
				}
				//fin MAJ

			}
			#endregion

			#region Calcul des PDM de FIN
			if(nbVehicle>0){
				// PDM des média
				if(oldIdCategory!=0){
					for(i=currentCategoryIndex+1;i<=currentLineIndex;i++){
						tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.PDM_COLUMN_INDEX]=(double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[currentCategoryIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]*100.0;
					}
				}
				// PDM des catégories
				for(i=0;i<currentCategoryPDMIndex;i++){
					tab[tabCategoryIndex[i],FrameWorkResultConstantes.CompetitorMediaPlan.PDM_COLUMN_INDEX]=(double)tab[tabCategoryIndex[i],FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[currentVehicleIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]*100.0;
				}
				// PDM des vehicles
				for(i=0;i<currentVehiclePDMIndex;i++){
					tab[tabVehicleIndex[i],FrameWorkResultConstantes.CompetitorMediaPlan.PDM_COLUMN_INDEX]=(double)tab[tabVehicleIndex[i],FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[currentTotalIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]*100.0;
				}
				for(k=1;k<=nbAdervertiser.Count;k++){
					tab[currentVehicleIndex+k,FrameWorkResultConstantes.CompetitorMediaPlan.PDM_COLUMN_INDEX]=(double)tab[currentVehicleIndex+k,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[currentVehicleIndex,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]*100.0;
				}

				// PDM du Total
				tab[currentTotalIndex,FrameWorkResultConstantes.CompetitorMediaPlan.PDM_COLUMN_INDEX]=(double)100.0;
				//Calcul des PDM des groupes du total
				for(k=1;k<=nbAdervertiser.Count;k++){
					tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,FrameWorkResultConstantes.CompetitorMediaPlan.PDM_COLUMN_INDEX]=(double)tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX,FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_COLUMN_INDEX]*100.0;
				}
			}
			#endregion
			
			#region Ecriture de fin de tableau
			nbCol=tab.GetLength(1);
			nbline=tab.GetLength(0);
			if(currentLineIndex+1<nbline)
				tab[currentLineIndex+1,0]=new FrameWorkConstantes.MemoryArrayEnd();
			#endregion

			ArrayList alVehicle=new ArrayList();
			ArrayList alCategory=new ArrayList();

			#region traitement de la périodicité	
			for(i=1;i<nbline;i++){
				if(tab[i,0]!=null)if(tab[i,0].GetType()==typeof(FrameWorkConstantes.MemoryArrayEnd)) break;
				// C'est une ligne Vehicle
				if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.VEHICLE_COLUMN_INDEX]!=null){
				//	currentVehicleIndex=i;
					alVehicle.Clear();
					for(k=0;k<=nbAdervertiser.Count;k++){
						alVehicle.Add((int)(i-(nbAdervertiser.Count-k)));
					}
				}
			
				
				// C'est une ligne Category
				if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.CATEGORY_COLUMN_INDEX]!=null){
					//currentCategoryIndex=i;
					alCategory.Clear();
					for(k=0;k<=nbAdervertiser.Count;k++){
						alCategory.Add((int)(i-(nbAdervertiser.Count-k)));
					}
				}
				
				// C'est une ligne Média
				if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.MEDIA_COLUMN_INDEX]!=null)currentMediaIndex=i;
				// C'est une ligne univers (annonceurs)
				if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX]!=null){
					for( j=FIRST_PERIOD_INDEX;j<nbCol;j++){
						if((bool)tab[i,j]==true){
							#region if
							tab[i,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.present;
							//tab[currentCategoryIndex-nbAdervertiser.Count,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.present;
							// Vehicle
							tab[(int)alVehicle[0],(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.present;
							for(k=1;k<=nbAdervertiser.Count;k++){
								//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
								if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
									tab[(int)alVehicle[(int)k],(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.present;
								}							
							}							
							// Category
							tab[(int)alCategory[0],(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.present;
							for(k=1;k<=nbAdervertiser.Count;k++){
								//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
								if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
									tab[(int)alCategory[(int)k],(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.present;
								}							
							}					
							tab[currentMediaIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.present;
							// Total
							tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.present;
							for(k=1;k<=nbAdervertiser.Count;k++){
								//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
								if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
									tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.present;
								}							
							}
							if(webSession.DetailPeriod==Constantes.Web.CustomerSessions.Period.DisplayLevel.monthly){
								switch(int.Parse(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.PERIODICITY_COLUMN_INDEX].ToString())){
										#region Trimestriel
									case (int)DBClassificationConstantes.Periodicity.type.trimestriel:
										j++;
										if(j<nbCol){
											tab[i,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
											//Total
											if(tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
											for(k=1;k<=nbAdervertiser.Count;k++){
												//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
												if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
													if(tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,(int)j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,(int)j].GetType()==typeof(bool)){
														tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
													}
												}							
											}
											// Vehicle
											if(tab[(int)alVehicle[0],j]==null || tab[(int)alVehicle[0],j].GetType()==typeof(bool))tab[(int)alVehicle[0],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
											for(k=1;k<=nbAdervertiser.Count;k++){
												//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
												if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
													if(tab[(int)alVehicle[(int)k],j]==null || tab[(int)alVehicle[(int)k],j].GetType()==typeof(bool)){
														tab[(int)alVehicle[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
													}
												}							
											}									
											// Categorie
											if(tab[(int)alCategory[0],j]==null || tab[(int)alCategory[0],j].GetType()==typeof(bool))tab[(int)alCategory[0],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
											for(k=1;k<=nbAdervertiser.Count;k++){
												//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
												if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
													if(tab[(int)alCategory[(int)k],j]==null || tab[(int)alCategory[(int)k],j].GetType()==typeof(bool)){
														tab[(int)alCategory[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
													}
												}							
											}											
											// Media
											if(tab[currentMediaIndex,j]==null || tab[currentMediaIndex,j].GetType()==typeof(bool))tab[currentMediaIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;

										}
										j++;
										if(j<nbCol){
											tab[i,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
											// Total
											if(tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
											for(k=1;k<=nbAdervertiser.Count;k++){
												//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
												if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
													if(tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,(int)j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,(int)j].GetType()==typeof(bool)){
														tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
													}
												}							
											}
											// Vehicle
											if(tab[(int)alVehicle[0],j]==null || tab[(int)alVehicle[0],j].GetType()==typeof(bool))tab[(int)alVehicle[0],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
											for(k=1;k<=nbAdervertiser.Count;k++){
												//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
												if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
													if(tab[(int)alVehicle[(int)k],j]==null || tab[(int)alVehicle[(int)k],j].GetType()==typeof(bool)){
														tab[(int)alVehicle[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
													}
												}							
											}	
											// Categorie
											if(tab[(int)alCategory[0],j]==null || tab[(int)alCategory[0],j].GetType()==typeof(bool))tab[(int)alCategory[0],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
											for(k=1;k<=nbAdervertiser.Count;k++){
												//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
												if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
													if(tab[(int)alCategory[(int)k],j]==null || tab[(int)alCategory[(int)k],j].GetType()==typeof(bool)){
														tab[(int)alCategory[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
													}
												}							
											}
											// Media
											if(tab[currentMediaIndex,j]==null || tab[currentMediaIndex,j].GetType()==typeof(bool))tab[currentMediaIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
										}
										break;
										#endregion

										#region Bimestriel
									case (int)DBClassificationConstantes.Periodicity.type.bimestriel:
										j++;
										if(j<nbCol){
											tab[i,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
											// Total
											if(tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
											for(k=1;k<=nbAdervertiser.Count;k++){
												//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
												if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
													if(tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,(int)j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,(int)j].GetType()==typeof(bool)){
														tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
													}
												}							
											}
											// Vehicle
											if(tab[(int)alVehicle[0],j]==null || tab[(int)alVehicle[0],j].GetType()==typeof(bool))tab[(int)alVehicle[0],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
											for(k=1;k<=nbAdervertiser.Count;k++){
												//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
												if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
													if(tab[(int)alVehicle[(int)k],j]==null || tab[(int)alVehicle[(int)k],j].GetType()==typeof(bool)){
														tab[(int)alVehicle[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
													}
												}							
											}	
											// Categorie
											if(tab[(int)alCategory[0],(int)j]==null || tab[(int)alCategory[0],(int)j].GetType()==typeof(bool))tab[(int)alCategory[0],(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
											for(k=1;k<=nbAdervertiser.Count;k++){
												//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
												if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
													if(tab[(int)alCategory[(int)k],j]==null || tab[(int)alCategory[(int)k],j].GetType()==typeof(bool)){
														tab[(int)alCategory[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
													}
												}							
											}
											// Media
											if(tab[currentMediaIndex,j]==null || tab[currentMediaIndex,j].GetType()==typeof(bool))tab[currentMediaIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
										}
										break;
										#endregion

									default:
										break;
								}
							}
							else{
								switch(int.Parse(tab[i,FrameWorkConstantes.Results.CompetitorMediaPlan.PERIODICITY_COLUMN_INDEX].ToString())){
										#region Mensuel
									case (int)DBClassificationConstantes.Periodicity.type.mensuel:
										j++;
										if(j<nbCol){
											weekDate=new AtomicPeriodWeek(int.Parse(tab[0,j].ToString().Substring(0,4)),int.Parse(tab[0,j].ToString().Substring(4,2)));
											forceEntry=true;
											while((weekDate.FirstDay.Day<weekDate.LastDay.Day && !((int)weekDate.FirstDay.DayOfWeek ==1 && weekDate.FirstDay.Day==1)) || forceEntry){
												forceEntry=false;
												if(j<nbCol){
													tab[i,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
													// Total
													if(tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
													for(k=1;k<=nbAdervertiser.Count;k++){
														//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
														if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
															if(tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,(int)j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,(int)j].GetType()==typeof(bool)){
																tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
															}
														}							
													}
													// Vehicle
													if(tab[(int)alVehicle[0],j]==null || tab[(int)alVehicle[0],j].GetType()==typeof(bool))tab[(int)alVehicle[0],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
													for(k=1;k<=nbAdervertiser.Count;k++){
														//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
														if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
															if(tab[(int)alVehicle[(int)k],j]==null || tab[(int)alVehicle[(int)k],j].GetType()==typeof(bool)){
																tab[(int)alVehicle[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
															}
														}							
													}	
													// Categorie
													if(tab[(int)alCategory[0],(int)j]==null || tab[(int)alCategory[0],(int)j].GetType()==typeof(bool))tab[(int)alCategory[0],(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
													for(k=1;k<=nbAdervertiser.Count;k++){
														//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
														if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
															if(tab[(int)alCategory[(int)k],j]==null || tab[(int)alCategory[(int)k],j].GetType()==typeof(bool)){
																tab[(int)alCategory[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
															}
														}							
													}
													// Media
													if(tab[currentMediaIndex,j]==null || tab[currentMediaIndex,j].GetType()==typeof(bool))tab[currentMediaIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
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
											tab[i,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
											// Total
											if(tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
											for(k=1;k<=nbAdervertiser.Count;k++){
												//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
												if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
													if(tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,(int)j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,(int)j].GetType()==typeof(bool)){
														tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
													}
												}							
											}
											// Vehicle
											if(tab[(int)alVehicle[0],j]==null || tab[(int)alVehicle[0],j].GetType()==typeof(bool))tab[(int)alVehicle[0],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
											for(k=1;k<=nbAdervertiser.Count;k++){
												//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
												if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
													if(tab[(int)alVehicle[(int)k],j]==null || tab[(int)alVehicle[(int)k],j].GetType()==typeof(bool)){
														tab[(int)alVehicle[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
													}
												}							
											}
											// Categorie
											if(tab[(int)alCategory[0],j]==null || tab[(int)alCategory[0],j].GetType()==typeof(bool))tab[(int)alCategory[0],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
											for(k=1;k<=nbAdervertiser.Count;k++){
												//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
												if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
													if(tab[(int)alCategory[(int)k],j]==null || tab[(int)alCategory[(int)k],j].GetType()==typeof(bool)){
														tab[(int)alCategory[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
													}
												}							
											}
											// Media
											if(tab[currentMediaIndex,j]==null || tab[currentMediaIndex,j].GetType()==typeof(bool))tab[currentMediaIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
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
														tab[i,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
														// Total
														if(tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
														for(k=1;k<=nbAdervertiser.Count;k++){
															//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
															if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
																if(tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,(int)j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,(int)j].GetType()==typeof(bool)){
																	tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
																}
															}							
														}
														// Vehicle
														if(tab[(int)alVehicle[0],j]==null || tab[(int)alVehicle[0],j].GetType()==typeof(bool))tab[(int)alVehicle[0],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
														for(k=1;k<=nbAdervertiser.Count;k++){
															//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
															if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
																if(tab[(int)alVehicle[(int)k],j]==null || tab[(int)alVehicle[(int)k],j].GetType()==typeof(bool)){
																	tab[(int)alVehicle[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
																}
															}							
														}	
														// Categorie
														if(tab[(int)alCategory[0],j]==null || tab[(int)alCategory[0],j].GetType()==typeof(bool))tab[(int)alCategory[0],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
														for(k=1;k<=nbAdervertiser.Count;k++){
															//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
															if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
																if(tab[(int)alCategory[(int)k],j]==null || tab[(int)alCategory[(int)k],j].GetType()==typeof(bool)){
																	tab[(int)alCategory[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
																}
															}							
														}
														// Media
														if(tab[currentMediaIndex,j]==null || tab[currentMediaIndex,j].GetType()==typeof(bool))tab[currentMediaIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
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
														tab[i,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
														// Total
														if(tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
														for(k=1;k<=nbAdervertiser.Count;k++){
															//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
															if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
																if(tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,(int)j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,(int)j].GetType()==typeof(bool)){
																	tab[FrameWorkResultConstantes.CompetitorMediaPlan.TOTAL_LINE_INDEX+k,(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
																}
															}
														}
														// Vehicle
														if(tab[(int)alVehicle[0],j]==null || tab[(int)alVehicle[0],j].GetType()==typeof(bool))tab[(int)alVehicle[0],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
														for(k=1;k<=nbAdervertiser.Count;k++){
															//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
															if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
																if(tab[(int)alVehicle[(int)k],j]==null || tab[(int)alVehicle[(int)k],j].GetType()==typeof(bool)){
																	tab[(int)alVehicle[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
																}
															}							
														}	
														// Categorie
														if(tab[(int)alCategory[0],j]==null || tab[(int)alCategory[0],j].GetType()==typeof(bool))tab[(int)alCategory[0],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
														for(k=1;k<=nbAdervertiser.Count;k++){
															//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
															if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
																if(tab[(int)alCategory[(int)k],j]==null || tab[(int)alCategory[(int)k],j].GetType()==typeof(bool)){
																	tab[(int)alCategory[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
																}
															}							
														}
														// Media
														if(tab[currentMediaIndex,j]==null || tab[currentMediaIndex,j].GetType()==typeof(bool))tab[currentMediaIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.extended;
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
							#endregion
						}
						else{
							tab[i,j]=FrameWorkConstantes.Results.CompetitorMediaPlan.graphicItemType.absent;
						}
					}
					
				}
			}
			#endregion

			ds.Dispose();
			return(tab);
		}
	}
}
