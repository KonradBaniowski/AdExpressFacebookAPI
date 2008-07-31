#region Informations
// Auteur: ?
// Date de création: ? 
// Date de modification:

#endregion

using System;
using System.Data;
using System.Collections;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.FrameWork.Date;
using TNS.AdExpress.Web.Exceptions;
using CstDB=TNS.AdExpress.Constantes.DB;
using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebFunctions=TNS.AdExpress.Web.Functions;
using VehicleClassification=TNS.AdExpress.Constantes.Classification;

namespace TNS.AdExpress.Web.Rules.Results{
	/// <summary>
	///Class métier de traitement des données issues de la base pour une analyse plan media concurentiel
	/// </summary>
	public class CompetitorMediaPlanAlertRules{						

		/// <summary>
		/// Fonction qui formate un DataSet en Calendrier d'actions sur une période
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="period">Periode d'étude</param>
		/// <returns>Tableau Formaté</returns>
		/// <remarks>
		/// Utilise les méthodes:
		///		public static object[,] TNS.AdExpress.Web.Rules.Results.CompetitorMediaPlanAlertRules.getFormattedTable(WebSession webSession, string beginningPeriod, string endPeriod)
		/// </remarks>
		public static object[,] GetFormattedTable(WebSession webSession, string period){
			// Création du DataSet de résultats
			return GetFormattedTable(webSession, period, period);
		}

		#region Ancienne version
//        /// <summary>
//        /// Fonction qui formate un DataSet en Calendrier d'actions à partie d'une date de debut et d'une date de fin:
//        ///		Calcul des dates de début et de fin de calendrier
//        ///		Extraction des données de la BD
//        ///		Extraction du nombre de vehicle et de category des données renvoyées
//        ///		Création d'un tableau repertoriant tous les jours
//        ///		Initialisation des tableaux tab(résultat), tabCategoryIndex(index des lignes catégorie pour les pdm et les totaux), tabVehicleIndex(index des lignes média pour les pdm et les totaux)
//        ///		Construction des libellé des périodes dans tab
//        ///		Remplissage du tableau avec les chiffres (totaux) pour les supports et calculs des totaux vehicle et categorie(1 ligne = 1 vehicle OU 1catégorie OU 1 support)
//        ///		Calcul des PDM vehicle, support et catégorie et total
//        ///		Calcul de la périodicité
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="beginningPeriod">Date de début</param>
//        /// <param name="endPeriod">Date de fin</param>
//        /// <returns>Tableau Formaté</returns>
//        /// <remarks>
//        /// Utilise les méthodes:
//        ///		public static DateTime TNS.AdExpress.Web.Functions.Dates.getPeriodBeginningDate(string period, TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type periodType)
//        ///		public static DateTime TNS.AdExpress.Web.Functions.Dates.getPeriodEndDate(string period, TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type periodType)
//        ///		public static DataSet TNS.AdExpress.Web.DataAccess.Results.CompetitorMediaPlanAlertDataAccess.getPluriMediaDataset(WebSession webSession, string beginningDate, string endDate)
//        /// </remarks>
//        public static object[,] GetFormattedTable(WebSession webSession, string beginningPeriod, string endPeriod){
		
//            #region Formattage des dates sur 8 chiffres
//            string periodBeginning = WebFunctions.Dates.getPeriodBeginningDate(beginningPeriod, webSession.PeriodType).ToString("yyyyMMdd");
//            string periodEnd = WebFunctions.Dates.getPeriodEndDate(endPeriod, webSession.PeriodType).ToString("yyyyMMdd");
//            #endregion

//            #region Chargement des données à partir de la base				
//            DataSet ds;
//            try{
//                ds=CompetitorMediaPlanAlertDataAccess.GetPluriMediaDataset(webSession, periodBeginning, periodEnd);
//            }
//            catch(System.Exception err){
//                throw(new CompetitorMediaPlanAlertRulesException("Impossible d'obtenir les données à partir de la base de données",err));
//            }
//            #endregion
	
//            #region Variables
//            DataTable dt=ds.Tables[0];
//            Int64 oldIdVehicle=0;
//            Int64 oldIdCategory=0;
//            Int64 oldIdMedia=0;
//            Int64 oldIdAdvertiser=0;
//            Int64 currentTotalIndex=1;
//            Int64 currentCategoryPDMIndex=0;
//            Int64 currentVehiclePDMIndex=0;
//            Int64 currentMediaPDMIndex=0;
//            int nbDays=0;
//            int nbMonth=0;
//            int nbline;
//            double unit=0.0;
//            int currentDate=0;
//            int oldCurrentDate=0;
//            Int64 i;
//            int numberOflineToAdd=0;
//            //string tmp="";
//            Int64 k=1;

//            //MAJ GR : Colonnes totaux par année si nécessaire
//            //FIRST_PERIOD_INDEX a remplacé FrameWorkResultConstantes.MediaPlan.FIRST_PEDIOD_INDEX dans toute la fonction
//            Hashtable YEARS_INDEX = new Hashtable();
//            int c = 0;
//            currentDate = int.Parse(periodBeginning.Substring(0,4));
//            oldCurrentDate = int.Parse(endPeriod.Substring(0,4));
//            int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.CompetitorMediaPlanAlert.FIRST_PEDIOD_INDEX;
//            if (currentDate!=oldCurrentDate) {
//                for(c=currentDate; c<=oldCurrentDate; c++) {
//                    YEARS_INDEX.Add(c,FIRST_PERIOD_INDEX);
//                    FIRST_PERIOD_INDEX++;
//                }
//            }
//            currentDate = 0;
//            oldCurrentDate = 0;
//            //fin MAJ

//            #endregion

//            #region Compte le nombre de vehicle de category et de média
//            int nbVehicle=0;
//            int nbCategory=0;
//            int nbMedia=0;
//            // Univers pour le total
//            Hashtable nbAdervertiser=new Hashtable();
//            foreach(DataRow currentRow in dt.Rows){
//                if(oldIdVehicle!=Int64.Parse(currentRow["id_vehicle"].ToString())){
//                    nbVehicle++;
//                    oldIdVehicle=Int64.Parse(currentRow["id_vehicle"].ToString());
//                }
//                if(oldIdCategory!=Int64.Parse(currentRow["id_category"].ToString())){
//                    nbCategory++;
//                    oldIdCategory=Int64.Parse(currentRow["id_category"].ToString());
//                }

//                if(oldIdMedia!=Int64.Parse(currentRow["id_media"].ToString())){
//                    nbMedia++;
//                    oldIdMedia=Int64.Parse(currentRow["id_media"].ToString());
//                }

//                if(oldIdAdvertiser!=Int64.Parse(currentRow["grpunivers"].ToString())){
//                    switch(Int64.Parse(currentRow["id_vehicle"].ToString())){
//                        case (Int64)DBClassificationConstantes.Vehicles.names.internationalPress:
//                            if(nbAdervertiser[Int64.Parse(currentRow["grpunivers"].ToString())]==null){
//                                nbAdervertiser.Add(Int64.Parse(currentRow["grpunivers"].ToString()),currentRow["grp"].ToString());
//                            }
//                            break;
//                        case (Int64)DBClassificationConstantes.Vehicles.names.press:
//                            if(nbAdervertiser[Int64.Parse(currentRow["grpunivers"].ToString())]==null){
//                                nbAdervertiser.Add(Int64.Parse(currentRow["grpunivers"].ToString()),currentRow["grp"].ToString());
//                            }
//                            break;
//                        case (Int64)DBClassificationConstantes.Vehicles.names.radio:
//                            if(nbAdervertiser[Int64.Parse(currentRow["grpunivers"].ToString())]==null){
//                                nbAdervertiser.Add(Int64.Parse(currentRow["grpunivers"].ToString()),currentRow["grp"].ToString());
//                            }
//                            break;
//                        case (Int64)DBClassificationConstantes.Vehicles.names.others:
//                        case (Int64)DBClassificationConstantes.Vehicles.names.tv:
//                            if(nbAdervertiser[Int64.Parse(currentRow["grpunivers"].ToString())]==null){
//                                nbAdervertiser.Add(Int64.Parse(currentRow["grpunivers"].ToString()),currentRow["grp"].ToString());
//                            }
//                            break;
//                        default:
//                            break;
		
//                    }
//                    oldIdAdvertiser=Int64.Parse(currentRow["grpunivers"].ToString());
//                }
//            }


//            for (k = 1; k <= 5; k++) {
//                if (nbAdervertiser[k] == null) {
//                    if (webSession.CompetitorUniversAdvertiser[(int)k] != null) {
//                        nbAdervertiser.Add(k, ((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[(int)k]).NameCompetitorAdvertiser);
//                    }
//                }
//            }			

			
//            // Il n'y a pas de données
//            if(nbVehicle==0)return(new object[0,0]);


//            oldIdVehicle=0;
//            oldIdCategory=0;
//            oldIdMedia=0;
//            #endregion

//            #region Création du tableau des jours
//            ArrayList periodItemsList=new ArrayList();
//            DateTime currentDateTime =  WebFunctions.Dates.getPeriodBeginningDate(beginningPeriod, webSession.PeriodType);
//            DateTime endDate = WebFunctions.Dates.getPeriodEndDate(endPeriod, webSession.PeriodType);
//            while(currentDateTime<=endDate){
//                periodItemsList.Add(currentDateTime);
//                currentDateTime = currentDateTime.AddDays(1);
//            }
//            #endregion

//            #region Déclaration des tableaux
//            // Nombre de colonne
//            int nbCol=periodItemsList.Count+FrameWorkConstantes.Results.CompetitorMediaPlanAlert.FIRST_PEDIOD_INDEX+YEARS_INDEX.Count;
//            // Tableau de résultat
//            object[,] tab=new object[dt.Rows.Count+2+nbVehicle+nbCategory+nbMedia+nbCategory*nbAdervertiser.Count+nbVehicle*nbAdervertiser.Count+nbAdervertiser.Count,nbCol];
//            // Tableau des indexes des categories
//            Int64[] tabCategoryIndex=new Int64[nbCategory+1];
//            // Tableau des indexes des vehicles
//            Int64[] tabVehicleIndex=new Int64[nbVehicle+1];
//            // Tableau des indexes des medias
//            Int64[] tabMediaIndex=new Int64[nbMedia+1];
//            #endregion

//            #region Libellé des colonnes
//            currentDate = 0;
//            while(currentDate<periodItemsList.Count){
//                tab[0,currentDate+FIRST_PERIOD_INDEX]= (DateTime)periodItemsList[currentDate];
//                currentDate++;
//            }
//            //MAJ GR
//            foreach(object o in YEARS_INDEX.Keys) {
//                tab[0,int.Parse(YEARS_INDEX[o].ToString())] = o.ToString();
//            }
//            //fin MAJ
//            tab[currentTotalIndex,0]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_STRING;
//            tab[currentTotalIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]=(double)0.0;
//            #endregion

//            //MAJ GR : initialisation totaux années
//            foreach(object o in YEARS_INDEX.Keys) {
//                tab[currentTotalIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
//            }
//            //fin MAJ			

			
//            Int64 j = 1;
//            for (j = 1; j <= nbAdervertiser.Count; j++) {
//                tab[currentTotalIndex + j, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] = (double)0.0;
//                tab[currentTotalIndex + j, 0] = nbAdervertiser[j].ToString();
//                //MAJ GR : initialisation totaux années
//                foreach (object o in YEARS_INDEX.Keys) {
//                    tab[currentTotalIndex + j, int.Parse(YEARS_INDEX[o].ToString())] = (double)0.0;
//                }
//                //fin MAJ			
//            }
			
		

//            Int64 currentVehicleIndex=2+nbAdervertiser.Count;
//            Int64 currentCategoryIndex=3+nbAdervertiser.Count;
//            Int64 currentLineIndex=1+nbAdervertiser.Count;
//            Int64 currentMediaIndex=4+nbAdervertiser.Count;

//            #region Construction du tableau de résultat

//            foreach(DataRow currentRow in dt.Rows){
//                // Nouveau Vehicle
//                if(oldIdVehicle!=Int64.Parse(currentRow["id_vehicle"].ToString())){
//                    // Calcul des PDM
//                    if(oldIdVehicle!=0){
//                        for(i=0;i<currentCategoryPDMIndex;i++){
//                            tab[tabCategoryIndex[i],FrameWorkConstantes.Results.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]=(double)tab[tabCategoryIndex[i],FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]/(double)tab[currentVehicleIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]*100.0;
//                        }
//                        tabCategoryIndex=new Int64[nbCategory+1];
//                        currentCategoryPDMIndex=0;
//                        //Calcul des PDM des groupes du vehicles précédent
//                        for(k=1;k<=nbAdervertiser.Count;k++){
//                            tab[currentVehicleIndex+k,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]=(double)tab[currentVehicleIndex+k,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]/(double)tab[currentVehicleIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]*100.0;
						
//                        }
//                    }
//                    oldIdVehicle=Int64.Parse(currentRow["id_vehicle"].ToString());
//                    for(k=1;k<=nbAdervertiser.Count;k++){
//                        if ((oldIdVehicle!=(Int64)VehicleClassification.DB.Vehicles.names.outdoor) ||(oldIdVehicle==(Int64)VehicleClassification.DB.Vehicles.names.outdoor && webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG)!=null))
//                            tab[currentLineIndex+k+1,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]=Int64.Parse(currentRow["id_vehicle"].ToString());	
//                        else
//                            tab[currentLineIndex+k+1,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]=null;

				
//                    }
//                    // Préparation des PDM des vehicles
//                    tabVehicleIndex[currentVehiclePDMIndex]=currentLineIndex+1;
//                    currentVehiclePDMIndex++;

//                    currentLineIndex++;
//                    tab[currentLineIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX]=currentRow["vehicle"].ToString();
//                    tab[currentLineIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]=(double)0.0;
//                    //MAJ GR : initialisation totaux années
//                    foreach(object o in YEARS_INDEX.Keys) {
//                        tab[currentLineIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
//                    }
//                    //fin MAJ
//                    currentVehicleIndex=currentLineIndex;
					
//                    numberOflineToAdd++;
//                    tab[currentLineIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.MEDIA_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.PERIODICITY_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX]=null;					

//                    if ((oldIdVehicle!=(Int64)VehicleClassification.DB.Vehicles.names.outdoor) ||(oldIdVehicle==(Int64)VehicleClassification.DB.Vehicles.names.outdoor && webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG)!=null)){
//                        tab[currentLineIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]=Int64.Parse(currentRow["id_vehicle"].ToString());
//                        tab[currentLineIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]=Int64.Parse(currentRow["id_category"].ToString());
//                        tab[currentLineIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX]=Int64.Parse(currentRow["id_media"].ToString());
//                    }

//                    // Totaux Annonceur au niveau de la Vezhicle
//                    for(k=1;k<=nbAdervertiser.Count;k++){
//                        tab[currentLineIndex+k,FrameWorkResultConstantes.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX]=nbAdervertiser[k].ToString();
//                        tab[currentLineIndex+k,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]=(double)0.0;
//                        tab[currentLineIndex+k,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]=null;
//                        tab[currentLineIndex+k,FrameWorkResultConstantes.CompetitorMediaPlanAlert.MEDIA_COLUMN_INDEX]=null;
//                        tab[currentLineIndex+k,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX]=null;
//                        //MAJ GR : initialisation totaux années
//                        foreach(object o in YEARS_INDEX.Keys) {
//                            tab[currentLineIndex+k,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
//                        }
//                        //fin MAJ						
//                    }
//                    currentLineIndex=currentLineIndex+nbAdervertiser.Count;
			
//                }
//                // Nouvelle Category
//                if(oldIdCategory!=Int64.Parse(currentRow["id_category"].ToString())){
//                    //Calcul des PDM de la categorie précedente
//                    if(oldIdCategory!=0){
//                        for(i=0;i<currentMediaPDMIndex;i++){
//                            tab[tabMediaIndex[i],FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]=(double)tab[tabMediaIndex[i],FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]/(double)tab[currentCategoryIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]*100.0;
//                        }
//                        tabMediaIndex=new Int64[nbMedia+1];
//                        currentMediaPDMIndex=0;
//                        //Calcul des PDM des groupes de la catégorie précédente
//                        for(k=1;k<=nbAdervertiser.Count;k++){
//                            tab[currentCategoryIndex+k,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]=(double)tab[currentCategoryIndex+k,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]/(double)tab[currentCategoryIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]*100.0;
//                        }
//                    }
//                    // Préparation des PDM des Categories
//                    tabCategoryIndex[currentCategoryPDMIndex]=currentLineIndex+1;
//                    currentCategoryPDMIndex++;

//                    currentLineIndex++;
//                    tab[currentLineIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]=currentRow["category"].ToString();
//                    tab[currentLineIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]=(double)0.0;
//                    //MAJ GR : initialisation totaux années
//                    foreach(object o in YEARS_INDEX.Keys) {
//                        tab[currentLineIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
//                    }
//                    //fin MAJ
//                    currentCategoryIndex=currentLineIndex;
//                    oldIdCategory=Int64.Parse(currentRow["id_category"].ToString());
//                    numberOflineToAdd++;
//                    tab[currentLineIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.MEDIA_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.PERIODICITY_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX]=null;

					
//                    if ((oldIdVehicle!=(Int64)VehicleClassification.DB.Vehicles.names.outdoor && oldIdCategory!=35) || (oldIdVehicle==(Int64)VehicleClassification.DB.Vehicles.names.outdoor && webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG)!=null)){
//                        tab[currentLineIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]=Int64.Parse(currentRow["id_vehicle"].ToString());
//                        tab[currentLineIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]=Int64.Parse(currentRow["id_category"].ToString());
//                        tab[currentLineIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX]=Int64.Parse(currentRow["id_media"].ToString());
//                    }
//                    else{
//                        tab[currentLineIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]=null;
//                        tab[currentLineIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]=null;
//                        tab[currentLineIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX]=null;
//                    }
//                    // Totaux Annonceur au niveau de la categorie
//                    for(j=1;j<=nbAdervertiser.Count;j++){
//                        tab[currentLineIndex+j,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]=nbAdervertiser[j].ToString();
//                        tab[currentLineIndex+j,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]=(double)0.0;
//                        tab[currentLineIndex+j,FrameWorkResultConstantes.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX]=null;
//                        tab[currentLineIndex+j,FrameWorkResultConstantes.CompetitorMediaPlanAlert.MEDIA_COLUMN_INDEX]=null;
//                        tab[currentLineIndex+j,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX]=null;
//                        //MAJ GR : initialisation totaux années
//                        foreach(object o in YEARS_INDEX.Keys) {
//                            tab[currentLineIndex+j,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
//                        }
//                        //fin MAJ												
//                    }
					
				
//                    currentLineIndex=currentLineIndex+nbAdervertiser.Count;
			
//                }
//                // Nouveau Media
//                if(oldIdMedia!=Int64.Parse(currentRow["id_media"].ToString())){
//                    if(oldIdMedia!=0){
//                        for(i=currentMediaIndex+1;i<=currentLineIndex;i++){
//                            tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]=(double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]/(double)tab[currentMediaIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]*100.0;
//                        }
//                    }
//                    // Préparation des PDM des médias
//                    tabMediaIndex[currentMediaPDMIndex]=currentLineIndex+1;
//                    currentMediaPDMIndex++;

//                    currentLineIndex++;
//                    tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.MEDIA_COLUMN_INDEX]=currentRow["media"].ToString();
//                    tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]=(double)0.0;
//                    foreach(object o in YEARS_INDEX.Keys) {
//                        tab[currentLineIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
//                    }
//                    //fin MAJ
//                    currentMediaIndex=currentLineIndex;
//                    oldIdMedia=Int64.Parse(currentRow["id_media"].ToString());
//                    oldIdAdvertiser=0;
//                    numberOflineToAdd++;
//                    tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX]=null;
				
//                    tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]=Int64.Parse(currentRow["id_category"].ToString());
//                    // Chaine thématique
//                    if ((oldIdVehicle!=(Int64)VehicleClassification.DB.Vehicles.names.outdoor && oldIdCategory!=35) || (oldIdVehicle==(Int64)VehicleClassification.DB.Vehicles.names.outdoor && webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG)!=null)){
//                        tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX]=Int64.Parse(currentRow["id_media"].ToString());	
//                    }
//                    else{
//                        tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX]=null;
//                    }
				
//                    tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]=Int64.Parse(currentRow["id_vehicle"].ToString());
				
				
			
//                }

//                // Advertiser
//                if(oldIdAdvertiser!=Int64.Parse(currentRow["grpunivers"].ToString())){	
//                    currentLineIndex++;
//                    tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX]=currentRow["grp"].ToString();
//                    //Périodicité Hebdomadaire pour OutDoor
//                    if(currentRow["id_vehicle"].ToString().Equals(DBClassificationConstantes.Vehicles.names.outdoor.GetHashCode().ToString())
//                        && currentRow["id_periodicity"].ToString().Equals(DBClassificationConstantes.Periodicity.type.indetermine.GetHashCode().ToString())						
//                        ){
//                        tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PERIODICITY_COLUMN_INDEX]=DBClassificationConstantes.Periodicity.type.hebdomadaire.GetHashCode().ToString();
//                    }else
//                        tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PERIODICITY_COLUMN_INDEX]=currentRow["id_periodicity"].ToString();
//                    tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]=(double)0.0;
//                    tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_ADVERTISER_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_ADVERTISER_COLUMN_INDEX]=Int64.Parse(currentRow["grpunivers"].ToString());

//                    tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]=Int64.Parse(currentRow["id_vehicle"].ToString());
//                    tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]=Int64.Parse(currentRow["id_category"].ToString());
//                    //MAJ GR : initialisation totaux années
//                    foreach(object o in YEARS_INDEX.Keys) {
//                        tab[currentLineIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
//                    }
//                    //fin MAJ

//                    //changement oldIdMedia->oldIdAdvertiser
//                    if ((oldIdVehicle!=(Int64)VehicleClassification.DB.Vehicles.names.outdoor && oldIdCategory!=35) || (oldIdVehicle==(Int64)VehicleClassification.DB.Vehicles.names.outdoor && webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG)!=null)){
//                        tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX]=Int64.Parse(currentRow["id_media"].ToString());
//                    }
//                    else{
//                        tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX]=null;
//                    }
				
				
//                    oldIdAdvertiser=Int64.Parse(currentRow["grpunivers"].ToString());
//                    currentDate=0;
//                    tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.MEDIA_COLUMN_INDEX]=null;
//                }

//                try{

//                    while(((DateTime)periodItemsList[currentDate]).ToString("yyyyMMdd")!=currentRow["date_num"].ToString()){
//                        tab[currentLineIndex,FIRST_PERIOD_INDEX+currentDate]=false;
//                        currentDate++;
//                    }
//                }
//                catch(System.Exception e){
//                    Console.Write(e.Message);
//                }
//                tab[currentLineIndex,FIRST_PERIOD_INDEX+currentDate]=true;				
							
//                // Période totaux Annonceur
//                tab[currentTotalIndex+Int64.Parse(currentRow["grpunivers"].ToString()),FIRST_PERIOD_INDEX+currentDate]=true;
//                tab[currentMediaIndex,FIRST_PERIOD_INDEX+currentDate]=true;
//                tab[currentCategoryIndex+Int64.Parse(currentRow["grpunivers"].ToString()),FIRST_PERIOD_INDEX+currentDate]=true;
//                tab[currentVehicleIndex+Int64.Parse(currentRow["grpunivers"].ToString()),FIRST_PERIOD_INDEX+currentDate]=true;
			
//                unit=double.Parse(currentRow["unit"].ToString());
//                tab[currentLineIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]=(double)tab[currentLineIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]+unit;
//                tab[currentMediaIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]=(double)tab[currentMediaIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]+unit;
//                tab[currentCategoryIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]=(double)tab[currentCategoryIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]+unit;
//                tab[currentVehicleIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]=(double)tab[currentVehicleIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]+unit;
//                tab[currentTotalIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]=(double)tab[currentTotalIndex,FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]+unit;
//                //MAJ GR : totaux par année
//                c = int.Parse(currentRow["date_num"].ToString().Substring(0,4));
//                if (YEARS_INDEX.Count>0) {
//                    c = int.Parse(YEARS_INDEX[c].ToString());
//                    tab[currentLineIndex,c]=(double)tab[currentLineIndex,c]+unit;
//                    tab[currentMediaIndex,c]=(double)tab[currentMediaIndex,c]+unit;
//                    tab[currentCategoryIndex,c]=(double)tab[currentCategoryIndex,c]+unit;
//                    tab[currentVehicleIndex,c]=(double)tab[currentVehicleIndex,c]+unit;
//                    tab[currentTotalIndex,c]=(double)tab[currentTotalIndex,c]+unit;
//                }
//                //fin MAJ

//                currentDate++;
//                oldCurrentDate=currentDate;
//                while(oldCurrentDate<periodItemsList.Count){
//                    tab[currentLineIndex,FIRST_PERIOD_INDEX+oldCurrentDate]=false;
//                    oldCurrentDate++;
//                }

//                //Essai totaux + création
//                tab[currentCategoryIndex+Int64.Parse(currentRow["grpunivers"].ToString()),FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]=(double)tab[currentCategoryIndex+Int64.Parse(currentRow["grpunivers"].ToString()),FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]+unit;
//                tab[currentCategoryIndex+Int64.Parse(currentRow["grpunivers"].ToString()),FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_ADVERTISER_COLUMN_INDEX]=Int64.Parse(currentRow["grpunivers"].ToString());
//                //MAJ GR : totaux par année
//                c = int.Parse(currentRow["date_num"].ToString().Substring(0,4));
//                if (YEARS_INDEX.Count>0) {
//                    c = int.Parse(YEARS_INDEX[c].ToString());
//                    tab[currentCategoryIndex+Int64.Parse(currentRow["grpunivers"].ToString()),c]=(double)tab[currentCategoryIndex+Int64.Parse(currentRow["grpunivers"].ToString()),c]+unit;
//                    tab[currentVehicleIndex+Int64.Parse(currentRow["grpunivers"].ToString()),c]=(double)tab[currentVehicleIndex+Int64.Parse(currentRow["grpunivers"].ToString()),c]+unit;
//                    tab[currentTotalIndex+Int64.Parse(currentRow["grpunivers"].ToString()),c]=(double)tab[currentTotalIndex+Int64.Parse(currentRow["grpunivers"].ToString()),c]+unit;
//                }
//                //fin MAJ

//                if ((oldIdVehicle!=(Int64)VehicleClassification.DB.Vehicles.names.outdoor && oldIdCategory!=35) || (oldIdVehicle==(Int64)VehicleClassification.DB.Vehicles.names.outdoor && webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG)!=null)){
//                    tab[currentCategoryIndex+Int64.Parse(currentRow["grpunivers"].ToString()),FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]=Int64.Parse(currentRow["id_category"].ToString());
//                }
//                else{
//                    tab[currentCategoryIndex+Int64.Parse(currentRow["grpunivers"].ToString()),FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]=null;
//                }
//                tab[currentCategoryIndex+Int64.Parse(currentRow["grpunivers"].ToString()),FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]=Int64.Parse(currentRow["id_vehicle"].ToString());
			
//                tab[currentVehicleIndex+Int64.Parse(currentRow["grpunivers"].ToString()),FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]=(double)tab[currentVehicleIndex+Int64.Parse(currentRow["grpunivers"].ToString()),FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]+unit;
//                tab[currentVehicleIndex+Int64.Parse(currentRow["grpunivers"].ToString()),FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_ADVERTISER_COLUMN_INDEX]=Int64.Parse(currentRow["grpunivers"].ToString());
			
//                tab[currentTotalIndex+Int64.Parse(currentRow["grpunivers"].ToString()),FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]=(double)tab[currentTotalIndex+Int64.Parse(currentRow["grpunivers"].ToString()),FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]+unit;
//            }
//            #endregion

//            #region Calcul des PDM de FIN
//            if(nbVehicle>0){				
		
//                // PDM des média
//                if(oldIdCategory!=0){
//                    for(i=currentCategoryIndex+1;i<=currentLineIndex;i++){
//                        tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]=(double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]/(double)tab[currentCategoryIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]*100.0;
//                    }
//                }
//                // PDM des catégories
//                for(i=0;i<currentCategoryPDMIndex;i++){
//                    tab[tabCategoryIndex[i],FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]=(double)tab[tabCategoryIndex[i],FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]/(double)tab[currentVehicleIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]*100.0;
//                }
//                // PDM des vehicles
//                for(i=0;i<currentVehiclePDMIndex;i++){
//                    tab[tabVehicleIndex[i],FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]=(double)tab[tabVehicleIndex[i],FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]/(double)tab[currentTotalIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]*100.0;
//                }
//                for(k=1;k<=nbAdervertiser.Count;k++){
//                    tab[currentVehicleIndex+k,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]=(double)tab[currentVehicleIndex+k,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]/(double)tab[currentVehicleIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]*100.0;
//                }

//                // PDM du Total
//                tab[currentTotalIndex,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]=(double)100.0;
//                //Calcul des PDM des groupes du total
//                for(k=1;k<=nbAdervertiser.Count;k++){
//                    tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX+k,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]=(double)tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX+k,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]/(double)tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]*100.0;
//                }
		
//            }
//            #endregion

//            #region Ecriture de fin de tableau
//            nbCol=tab.GetLength(1);
//            nbline=tab.GetLength(0);
//            if(currentLineIndex+1<nbline)
//                tab[currentLineIndex+1,0]=new TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd();
//            #endregion

//            ArrayList alVehicle=new ArrayList();
//            ArrayList alCategory=new ArrayList();

//            #region traitement de la périodicité	
//            for(i=1;i<nbline;i++){
//                if(tab[i,0]!=null)if(tab[i,0].GetType()==typeof(FrameWorkConstantes.MemoryArrayEnd)) break;
//                // C'est une ligne Vehicle
//                //if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX]!=null)currentVehicleIndex=i;
//                // C'est une ligne Vehicle
//                if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX]!=null){
//                    //	currentVehicleIndex=i;
//                    alVehicle.Clear();
//                    for(k=0;k<=nbAdervertiser.Count;k++){
//                        alVehicle.Add((int)(i-(nbAdervertiser.Count-k)));
//                    }
//                }

//                // C'est une ligne Category
//                //if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]!=null)currentCategoryIndex=i;
//                // C'est une ligne Category
//                if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]!=null){
//                    //currentCategoryIndex=i;
//                    alCategory.Clear();
//                    for(k=0;k<=nbAdervertiser.Count;k++){
//                        alCategory.Add((int)(i-(nbAdervertiser.Count-k)));
//                    }
//                }

//                // C'est une ligne Média
//                if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.MEDIA_COLUMN_INDEX]!=null)currentMediaIndex=i;

//                // C'est une ligne univers annonceurs
//                if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX]!=null){
//                    for( j=FIRST_PERIOD_INDEX;j<nbCol;j++){
//                        if((bool)tab[i,j]==true){
//                            tab[i,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.present;
//                            // Categorie
//                            //tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.present;
//                            tab[(int)alCategory[0],(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.present;
//                            for(k=1;k<=nbAdervertiser.Count;k++){
							
//                                if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
//                                    tab[(int)alCategory[(int)k],(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.present;
//                                }	
							
//                            }	
						
//                            // Vehicle
//                            //tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.present;
//                            tab[(int)alVehicle[0],(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.present;
//                            for(k=1;k<=nbAdervertiser.Count;k++){
//                                if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
//                                    tab[(int)alVehicle[(int)k],(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.present;
//                                }
//                            }
//                            // Total
//                            //tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.present;
//                            tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.present;
//                            for(k=1;k<=nbAdervertiser.Count;k++){
//                                if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
//                                    tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX+k,(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.present;
//                                }	
//                            }

//                            tab[currentMediaIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.present;
						
//                            switch(int.Parse(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PERIODICITY_COLUMN_INDEX].ToString())){
//                                case (int)DBClassificationConstantes.Periodicity.type.mensuel:
//                                    j++;
//                                    while (j<nbCol && int.Parse(((DateTime)tab[FrameWorkConstantes.Results.CompetitorMediaPlanAlert.PERIOD_LINE_INDEX,j]).ToString("dd"))!=1){
//                                        tab[i,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                        // Total
//                                        if(tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                        for(k=1;k<=nbAdervertiser.Count;k++){
//                                            if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
//                                                if(tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX+k,(int)j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX+k,(int)j].GetType()==typeof(bool)){
//                                                    tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX+k,(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                                }
//                                            }
//                                        }
//                                        // Vehicle
//                                        //if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                        if(tab[(int)alVehicle[0],j]==null || tab[(int)alVehicle[0],j].GetType()==typeof(bool))tab[(int)alVehicle[0],j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                        for(k=1;k<=nbAdervertiser.Count;k++){
//                                            if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
//                                                if(tab[(int)alVehicle[(int)k],j]==null || tab[(int)alVehicle[(int)k],j].GetType()==typeof(bool)){
//                                                    tab[(int)alVehicle[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                                }
//                                            }	
//                                        }	
//                                        // Categorie
//                                        //if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;										
//                                        if(tab[(int)alCategory[0],j]==null || tab[(int)alCategory[0],j].GetType()==typeof(bool))tab[(int)alCategory[0],j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                        for(k=1;k<=nbAdervertiser.Count;k++){
//                                            if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
//                                                if(tab[(int)alCategory[(int)k],j]==null || tab[(int)alCategory[(int)k],j].GetType()==typeof(bool)){
//                                                    tab[(int)alCategory[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                                }
//                                            }	
										
//                                        }
//                                        // Media
//                                        if(tab[currentMediaIndex,j]==null || tab[currentMediaIndex,j].GetType()==typeof(bool))tab[currentMediaIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                        j++;
//                                    }
//                                    j--;
//                                    break;
//                                case (int)DBClassificationConstantes.Periodicity.type.bimestriel:
//                                    j++;
//                                    nbMonth=0;
//                                    while(j<nbCol && nbMonth<DBClassificationConstantes.Periodicity.MONTH_NUMBER_IN_BIMONTHLY){
//                                        while (j<nbCol && int.Parse(((DateTime)tab[FrameWorkConstantes.Results.CompetitorMediaPlanAlert.PERIOD_LINE_INDEX,j]).ToString("dd"))!=1){
//                                            tab[i,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                            // Total
//                                            if(tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                            for(k=1;k<=nbAdervertiser.Count;k++){
//                                                if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
//                                                    if(tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX+k,(int)j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX+k,(int)j].GetType()==typeof(bool)){
//                                                        tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX+k,(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                                    }
//                                                }
//                                            }
//                                            // Vehicle
//                                            //if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                            if(tab[(int)alVehicle[0],j]==null || tab[(int)alVehicle[0],j].GetType()==typeof(bool))tab[(int)alVehicle[0],j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                            for(k=1;k<=nbAdervertiser.Count;k++){
//                                                if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
//                                                    if(tab[(int)alVehicle[(int)k],j]==null || tab[(int)alVehicle[(int)k],j].GetType()==typeof(bool)){
//                                                        tab[(int)alVehicle[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                                    }
//                                                }
//                                            }
//                                            // Categorie
//                                            //if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;										
//                                            if(tab[(int)alCategory[0],j]==null || tab[(int)alCategory[0],j].GetType()==typeof(bool))tab[(int)alCategory[0],j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                            for(k=1;k<=nbAdervertiser.Count;k++){
//                                                if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
//                                                    if(tab[(int)alCategory[(int)k],j]==null || tab[(int)alCategory[(int)k],j].GetType()==typeof(bool)){
//                                                        tab[(int)alCategory[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                                    }
//                                                }	
//                                            }
//                                            // Media
//                                            if(tab[currentMediaIndex,j]==null || tab[currentMediaIndex,j].GetType()==typeof(bool))tab[currentMediaIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                            j++;
//                                        }
//                                        nbMonth++;
//                                    }
//                                    j--;
//                                    break;
//                                case (int)DBClassificationConstantes.Periodicity.type.trimestriel:
//                                    j++;
//                                    nbMonth=0;
//                                    while(nbMonth<DBClassificationConstantes.Periodicity.MONTH_NUMBER_IN_QUATERLY){
//                                        while (j<nbCol && int.Parse(((DateTime)tab[FrameWorkConstantes.Results.CompetitorMediaPlanAlert.PERIOD_LINE_INDEX,j]).ToString("dd"))!=1){
//                                            tab[i,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                            // Total
//                                            if(tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                            for(k=1;k<=nbAdervertiser.Count;k++){
//                                                if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
//                                                    if(tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX+k,(int)j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX+k,(int)j].GetType()==typeof(bool)){
//                                                        tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX+k,(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                                    }
//                                                }	
//                                            }											
//                                            // Vehicle
//                                            //if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                            if(tab[(int)alVehicle[0],j]==null || tab[(int)alVehicle[0],j].GetType()==typeof(bool))tab[(int)alVehicle[0],j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                            for(k=1;k<=nbAdervertiser.Count;k++){
//                                                if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
//                                                    if(tab[(int)alVehicle[(int)k],j]==null || tab[(int)alVehicle[(int)k],j].GetType()==typeof(bool)){
//                                                        tab[(int)alVehicle[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                                    }
//                                                }
//                                            }
//                                            // Categorie
//                                            //if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;										
//                                            if(tab[(int)alCategory[0],j]==null || tab[(int)alCategory[0],j].GetType()==typeof(bool))tab[(int)alCategory[0],j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                            for(k=1;k<=nbAdervertiser.Count;k++){
//                                                if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
//                                                    if(tab[(int)alCategory[(int)k],j]==null || tab[(int)alCategory[(int)k],j].GetType()==typeof(bool)){
//                                                        tab[(int)alCategory[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                                    }
//                                                }	
//                                            }
//                                            // Média
//                                            if(tab[currentMediaIndex,j]==null || tab[currentMediaIndex,j].GetType()==typeof(bool))tab[currentMediaIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                            j++;
//                                        }
//                                        nbMonth++;
//                                    }
//                                    j--;
//                                    break;
//                                case (int)DBClassificationConstantes.Periodicity.type.hebdomadaire:
//                                    j++;
//                                    nbDays=0;
//                                    while(j<nbCol && nbDays<DBClassificationConstantes.Periodicity.DAY_NUMBER_IN_WEEK-1){
//                                        tab[i,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                        // Total
//                                        if(tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                        for(k=1;k<=nbAdervertiser.Count;k++){
//                                            if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
//                                                if(tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX+k,(int)j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX+k,(int)j].GetType()==typeof(bool)){
//                                                    tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX+k,(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                                }							
//                                            }
//                                        }
//                                        // Vehicle
//                                        //if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                        if(tab[(int)alVehicle[0],j]==null || tab[(int)alVehicle[0],j].GetType()==typeof(bool))tab[(int)alVehicle[0],j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                        for(k=1;k<=nbAdervertiser.Count;k++){
//                                            if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
//                                                if(tab[(int)alVehicle[(int)k],j]==null || tab[(int)alVehicle[(int)k],j].GetType()==typeof(bool)){
//                                                    tab[(int)alVehicle[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                                }
//                                            }	
//                                        }
//                                        // Categorie
//                                        //if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;										
//                                        if(tab[(int)alCategory[0],j]==null || tab[(int)alCategory[0],j].GetType()==typeof(bool))tab[(int)alCategory[0],j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                        for(k=1;k<=nbAdervertiser.Count;k++){
//                                            if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
//                                                if(tab[(int)alCategory[(int)k],j]==null || tab[(int)alCategory[(int)k],j].GetType()==typeof(bool)){
//                                                    tab[(int)alCategory[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                                }
//                                            }	
//                                        }
//                                        // Media
//                                        if(tab[currentMediaIndex,j]==null || tab[currentMediaIndex,j].GetType()==typeof(bool))tab[currentMediaIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                        nbDays++;
//                                        j++;
//                                    }
//                                    j--;
//                                    break;
//                                case (int)DBClassificationConstantes.Periodicity.type.bimensuel:
//                                    j++;
//                                    nbDays=0;
//                                    while(j<nbCol && nbDays<DBClassificationConstantes.Periodicity.DAY_NUMBER_IN_BIWEEKLY-1){
//                                        tab[i,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                        // Total
//                                        if(tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX,j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX,j].GetType()==typeof(bool))tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                        for(k=1;k<=nbAdervertiser.Count;k++){
//                                            if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
//                                                if(tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX+k,(int)j]==null || tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX+k,(int)j].GetType()==typeof(bool)){
//                                                    tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX+k,(int)j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                                }
//                                            }	
//                                        }
//                                        // Vehicle
//                                        //if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                        if(tab[(int)alVehicle[0],j]==null || tab[(int)alVehicle[0],j].GetType()==typeof(bool))tab[(int)alVehicle[0],j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                        for(k=1;k<=nbAdervertiser.Count;k++){
//                                            if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
//                                                if(tab[(int)alVehicle[(int)k],j]==null || tab[(int)alVehicle[(int)k],j].GetType()==typeof(bool)){
//                                                    tab[(int)alVehicle[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                                }
//                                            }
//                                        }
//                                        // Categorie
//                                        //if(tab[currentCategoryIndex,j]==null || tab[currentCategoryIndex,j].GetType()==typeof(bool))tab[currentCategoryIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;										
//                                        if(tab[(int)alCategory[0],j]==null || tab[(int)alCategory[0],j].GetType()==typeof(bool))tab[(int)alCategory[0],j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                        for(k=1;k<=nbAdervertiser.Count;k++){
//                                            if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString()==nbAdervertiser[k].ToString()){
//                                                if(tab[(int)alCategory[(int)k],j]==null || tab[(int)alCategory[(int)k],j].GetType()==typeof(bool)){
//                                                    tab[(int)alCategory[(int)k],j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                                }
//                                            }
//                                        }
//                                        // Media
//                                        if(tab[currentMediaIndex,j]==null || tab[currentMediaIndex,j].GetType()==typeof(bool))tab[currentMediaIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
//                                        nbDays++;
//                                        j++;
//                                    }
//                                    j--;
//                                    break;
//                                default:
//                                    break;
//                            }
//                        }
//                        else{
//                            tab[i,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.absent;
//                        }
//                    }
//                }
//            }
//            #endregion


//            ds.Dispose();
//            #region Debug: Voir le tableau
////			StringBuilder HTML=new StringBuilder();
////			HTML.Append("<html><table><tr>");
////			for(i=0;i<nbline;i++){
////				for(j=0;j<nbCol;j++){
////					if(tab[i,j]!=null)HTML.Append("<td>"+tab[i,j].ToString()+"</td>");
////					else HTML.Append("<td>&nbsp;</td>");
////				}
////				HTML.Append("</tr><tr>");
////			}
////			HTML.Append("</tr></table></html>");
//            #endregion		
	
//            return(tab);


//        }
		#endregion

		#region Nouvelle version
		/// <summary>
		/// Fonction qui formate un DataSet en Calendrier d'actions à partie d'une date de debut et d'une date de fin:
		///		Calcul des dates de début et de fin de calendrier
		///		Extraction des données de la BD
		///		Extraction du nombre de vehicle et de category des données renvoyées
		///		Création d'un tableau repertoriant tous les jours
		///		Initialisation des tableaux tab(résultat), tabCategoryIndex(index des lignes catégorie pour les pdm et les totaux), tabVehicleIndex(index des lignes média pour les pdm et les totaux)
		///		Construction des libellé des périodes dans tab
		///		Remplissage du tableau avec les chiffres (totaux) pour les supports et calculs des totaux vehicle et categorie(1 ligne = 1 vehicle OU 1catégorie OU 1 support)
		///		Calcul des PDM vehicle, support et catégorie et total
		///		Calcul de la périodicité
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="beginningPeriod">Date de début</param>
		/// <param name="endPeriod">Date de fin</param>
		/// <returns>Tableau Formaté</returns>
		/// <remarks>
		/// Utilise les méthodes:
		///		public static DateTime TNS.AdExpress.Web.Functions.Dates.getPeriodBeginningDate(string period, TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type periodType)
		///		public static DateTime TNS.AdExpress.Web.Functions.Dates.getPeriodEndDate(string period, TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type periodType)
		///		public static DataSet TNS.AdExpress.Web.DataAccess.Results.CompetitorMediaPlanAlertDataAccess.getPluriMediaDataset(WebSession webSession, string beginningDate, string endDate)
		/// </remarks>
		public static object[,] GetFormattedTable(WebSession webSession, string beginningPeriod, string endPeriod) {

			#region Formattage des dates sur 8 chiffres
			string periodBeginning = WebFunctions.Dates.getPeriodBeginningDate(beginningPeriod, webSession.PeriodType).ToString("yyyyMMdd");
			string periodEnd = WebFunctions.Dates.getPeriodEndDate(endPeriod, webSession.PeriodType).ToString("yyyyMMdd");
			#endregion

			#region Chargement des données à partir de la base
			DataSet ds;
			try {
				ds = CompetitorMediaPlanAlertDataAccess.GetPluriMediaDataset(webSession, periodBeginning, periodEnd);
			}
			catch (System.Exception err) {
				throw (new CompetitorMediaPlanAlertRulesException("Impossible d'obtenir les données à partir de la base de données", err));
			}
			#endregion

			#region Variables
			DataTable dt = ds.Tables[0];
			Int64 oldIdVehicle = 0;
			Int64 oldIdCategory = 0;
			Int64 oldIdMedia = 0;
			Int64 oldIdAdvertiser = -1;	//0 MAJ DM		
			Int64 currentTotalIndex = 1;
			Int64 currentCategoryPDMIndex = 0;
			Int64 currentVehiclePDMIndex = 0;
			Int64 currentMediaPDMIndex = 0;
			int nbDays = 0;
			int nbMonth = 0;
			int nbline;
			double unit = 0.0;
			int currentDate = 0;
			int oldCurrentDate = 0;
			Int64 i;
			int numberOflineToAdd = 0;
			//string tmp="";
			Int64 k = 1;

			//MAJ GR : Colonnes totaux par année si nécessaire
			//FIRST_PERIOD_INDEX a remplacé FrameWorkResultConstantes.MediaPlan.FIRST_PEDIOD_INDEX dans toute la fonction
			Hashtable YEARS_INDEX = new Hashtable();
			int c = 0;
			currentDate = int.Parse(periodBeginning.Substring(0, 4));
			oldCurrentDate = int.Parse(endPeriod.Substring(0, 4));
			int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.CompetitorMediaPlanAlert.FIRST_PEDIOD_INDEX;
			if (currentDate != oldCurrentDate) {
				for (c = currentDate; c <= oldCurrentDate; c++) {
					YEARS_INDEX.Add(c, FIRST_PERIOD_INDEX);
					FIRST_PERIOD_INDEX++;
				}
			}
			currentDate = 0;
			oldCurrentDate = 0;
			//fin MAJ

			#endregion

			#region Compte le nombre de vehicle de category et de média
			int nbVehicle = 0;
			int nbCategory = 0;
			int nbMedia = 0;
			// Univers pour le total
			Hashtable nbAdervertiser = new Hashtable();
			foreach (DataRow currentRow in dt.Rows) {
				if (oldIdVehicle != Int64.Parse(currentRow["id_vehicle"].ToString())) {
					nbVehicle++;
					oldIdVehicle = Int64.Parse(currentRow["id_vehicle"].ToString());
				}
				if (oldIdCategory != Int64.Parse(currentRow["id_category"].ToString())) {
					nbCategory++;
					oldIdCategory = Int64.Parse(currentRow["id_category"].ToString());
				}

				if (oldIdMedia != Int64.Parse(currentRow["id_media"].ToString())) {
					nbMedia++;
					oldIdMedia = Int64.Parse(currentRow["id_media"].ToString());
				}

				if (oldIdAdvertiser != Int64.Parse(currentRow["grpunivers"].ToString())) {
					switch (Int64.Parse(currentRow["id_vehicle"].ToString())) {
						case (Int64)DBClassificationConstantes.Vehicles.names.internationalPress:
							if (nbAdervertiser[Int64.Parse(currentRow["grpunivers"].ToString())] == null) {
								nbAdervertiser.Add(Int64.Parse(currentRow["grpunivers"].ToString()), currentRow["grp"].ToString());
							}
							break;
						case (Int64)DBClassificationConstantes.Vehicles.names.press:
							if (nbAdervertiser[Int64.Parse(currentRow["grpunivers"].ToString())] == null) {
								nbAdervertiser.Add(Int64.Parse(currentRow["grpunivers"].ToString()), currentRow["grp"].ToString());
							}
							break;
						case (Int64)DBClassificationConstantes.Vehicles.names.radio:
							if (nbAdervertiser[Int64.Parse(currentRow["grpunivers"].ToString())] == null) {
								nbAdervertiser.Add(Int64.Parse(currentRow["grpunivers"].ToString()), currentRow["grp"].ToString());
							}
							break;
						case (Int64)DBClassificationConstantes.Vehicles.names.others:
						case (Int64)DBClassificationConstantes.Vehicles.names.tv:
							if (nbAdervertiser[Int64.Parse(currentRow["grpunivers"].ToString())] == null) {
								nbAdervertiser.Add(Int64.Parse(currentRow["grpunivers"].ToString()), currentRow["grp"].ToString());
							}
							break;
						default:
							break;

					}
					oldIdAdvertiser = Int64.Parse(currentRow["grpunivers"].ToString());
				}
			}


			//for (k = 1; k <= 5; k++) {
			//    if (nbAdervertiser[k] == null) {
			//        if (webSession.CompetitorUniversAdvertiser[(int)k] != null) {
			//            nbAdervertiser.Add(k, ((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[(int)k]).NameCompetitorAdvertiser);
			//        }
			//    }
			//}
			for (k = 0; k < 5; k++) {
				if (nbAdervertiser[k] == null) {
					if (webSession.PrincipalProductUniverses.ContainsKey((int)k)  && webSession.PrincipalProductUniverses[(int)k] != null) {
						nbAdervertiser.Add(k, webSession.PrincipalProductUniverses[(int)k].Label);
					}
				}
			}


			// Il n'y a pas de données
			if (nbVehicle == 0) return (new object[0, 0]);


			oldIdVehicle = 0;
			oldIdCategory = 0;
			oldIdMedia = 0;
			#endregion

			#region Création du tableau des jours
			ArrayList periodItemsList = new ArrayList();
			DateTime currentDateTime = WebFunctions.Dates.getPeriodBeginningDate(beginningPeriod, webSession.PeriodType);
			DateTime endDate = WebFunctions.Dates.getPeriodEndDate(endPeriod, webSession.PeriodType);
			while (currentDateTime <= endDate) {
				periodItemsList.Add(currentDateTime);
				currentDateTime = currentDateTime.AddDays(1);
			}
			#endregion

			#region Déclaration des tableaux
			// Nombre de colonne
			int nbCol = periodItemsList.Count + FrameWorkConstantes.Results.CompetitorMediaPlanAlert.FIRST_PEDIOD_INDEX + YEARS_INDEX.Count;
			// Tableau de résultat
			object[,] tab = new object[dt.Rows.Count + 2 + nbVehicle + nbCategory + nbMedia + nbCategory * nbAdervertiser.Count + nbVehicle * nbAdervertiser.Count + nbAdervertiser.Count, nbCol];
			// Tableau des indexes des categories
			Int64[] tabCategoryIndex = new Int64[nbCategory + 1];
			// Tableau des indexes des vehicles
			Int64[] tabVehicleIndex = new Int64[nbVehicle + 1];
			// Tableau des indexes des medias
			Int64[] tabMediaIndex = new Int64[nbMedia + 1];
			#endregion

			#region Libellé des colonnes
			currentDate = 0;
			while (currentDate < periodItemsList.Count) {
				tab[0, currentDate + FIRST_PERIOD_INDEX] = (DateTime)periodItemsList[currentDate];
				currentDate++;
			}
			//MAJ GR
			foreach (object o in YEARS_INDEX.Keys) {
				tab[0, int.Parse(YEARS_INDEX[o].ToString())] = o.ToString();
			}
			//fin MAJ
			tab[currentTotalIndex, 0] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_STRING;
			tab[currentTotalIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] = (double)0.0;
			#endregion

			//MAJ GR : initialisation totaux années
			foreach (object o in YEARS_INDEX.Keys) {
				tab[currentTotalIndex, int.Parse(YEARS_INDEX[o].ToString())] = (double)0.0;
			}
			//fin MAJ			


			//Int64 j = 1;
			//for (j = 1; j <= nbAdervertiser.Count; j++) {
			//    tab[currentTotalIndex + j, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] = (double)0.0;
			//    tab[currentTotalIndex + j, 0] = nbAdervertiser[j].ToString();
			//    //MAJ GR : initialisation totaux années
			//    foreach (object o in YEARS_INDEX.Keys) {
			//        tab[currentTotalIndex + j, int.Parse(YEARS_INDEX[o].ToString())] = (double)0.0;
			//    }
			//    //fin MAJ			
			//}

			Int64 j = 1;
			for (j = 1; j <= nbAdervertiser.Count; j++) {
				tab[currentTotalIndex + j, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] = (double)0.0;
				tab[currentTotalIndex + j, 0] = nbAdervertiser[j-1].ToString();
				//MAJ GR : initialisation totaux années
				foreach (object o in YEARS_INDEX.Keys) {
					tab[currentTotalIndex + j, int.Parse(YEARS_INDEX[o].ToString())] = (double)0.0;
				}
				//fin MAJ			
			}

			Int64 currentVehicleIndex = 2 + nbAdervertiser.Count;
			Int64 currentCategoryIndex = 3 + nbAdervertiser.Count;
			Int64 currentLineIndex = 1 + nbAdervertiser.Count;
			Int64 currentMediaIndex = 4 + nbAdervertiser.Count;

			#region Construction du tableau de résultat

			foreach (DataRow currentRow in dt.Rows) {
				// Nouveau Vehicle
				if (oldIdVehicle != Int64.Parse(currentRow["id_vehicle"].ToString())) {
					// Calcul des PDM
					if (oldIdVehicle != 0) {
						for (i = 0; i < currentCategoryPDMIndex; i++) {
							tab[tabCategoryIndex[i], FrameWorkConstantes.Results.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX] = (double)tab[tabCategoryIndex[i], FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] / (double)tab[currentVehicleIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] * 100.0;
						}
						tabCategoryIndex = new Int64[nbCategory + 1];
						currentCategoryPDMIndex = 0;
						//Calcul des PDM des groupes du vehicles précédent
						for (k = 1; k <= nbAdervertiser.Count; k++) {
							tab[currentVehicleIndex + k, FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX] = (double)tab[currentVehicleIndex + k, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] / (double)tab[currentVehicleIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] * 100.0;

						}
					}
					oldIdVehicle = Int64.Parse(currentRow["id_vehicle"].ToString());
					for (k = 1; k <= nbAdervertiser.Count; k++) {
						if ((oldIdVehicle != (Int64)VehicleClassification.DB.Vehicles.names.outdoor) || (oldIdVehicle == (Int64)VehicleClassification.DB.Vehicles.names.outdoor && webSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG)))
							tab[currentLineIndex + k + 1, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] = Int64.Parse(currentRow["id_vehicle"].ToString());
						else
							tab[currentLineIndex + k + 1, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] = null;


					}
					// Préparation des PDM des vehicles
					tabVehicleIndex[currentVehiclePDMIndex] = currentLineIndex + 1;
					currentVehiclePDMIndex++;

					currentLineIndex++;
					tab[currentLineIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX] = currentRow["vehicle"].ToString();
					tab[currentLineIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] = (double)0.0;
					//MAJ GR : initialisation totaux années
					foreach (object o in YEARS_INDEX.Keys) {
						tab[currentLineIndex, int.Parse(YEARS_INDEX[o].ToString())] = (double)0.0;
					}
					//fin MAJ
					currentVehicleIndex = currentLineIndex;

					numberOflineToAdd++;
					tab[currentLineIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX] = null;
					tab[currentLineIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.MEDIA_COLUMN_INDEX] = null;
					tab[currentLineIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX] = null;
					tab[currentLineIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.PERIODICITY_COLUMN_INDEX] = null;
					tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX] = null;

					if ((oldIdVehicle != (Int64)VehicleClassification.DB.Vehicles.names.outdoor) || (oldIdVehicle == (Int64)VehicleClassification.DB.Vehicles.names.outdoor && webSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG))) {
						tab[currentLineIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] = Int64.Parse(currentRow["id_vehicle"].ToString());
						tab[currentLineIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX] = Int64.Parse(currentRow["id_category"].ToString());
						tab[currentLineIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX] = Int64.Parse(currentRow["id_media"].ToString());
					}

					// Totaux Annonceur au niveau de la Vezhicle
					for (k = 1; k <= nbAdervertiser.Count; k++) {
						tab[currentLineIndex + k, FrameWorkResultConstantes.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX] = nbAdervertiser[k-1].ToString();
						tab[currentLineIndex + k, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] = (double)0.0;
						tab[currentLineIndex + k, FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX] = null;
						tab[currentLineIndex + k, FrameWorkResultConstantes.CompetitorMediaPlanAlert.MEDIA_COLUMN_INDEX] = null;
						tab[currentLineIndex + k, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX] = null;
						//MAJ GR : initialisation totaux années
						foreach (object o in YEARS_INDEX.Keys) {
							tab[currentLineIndex + k, int.Parse(YEARS_INDEX[o].ToString())] = (double)0.0;
						}
						//fin MAJ						
					}
					currentLineIndex = currentLineIndex + nbAdervertiser.Count;

				}
				// Nouvelle Category
				if (oldIdCategory != Int64.Parse(currentRow["id_category"].ToString())) {
					//Calcul des PDM de la categorie précedente
					if (oldIdCategory != 0) {
						for (i = 0; i < currentMediaPDMIndex; i++) {
							tab[tabMediaIndex[i], FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX] = (double)tab[tabMediaIndex[i], FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] / (double)tab[currentCategoryIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] * 100.0;
						}
						tabMediaIndex = new Int64[nbMedia + 1];
						currentMediaPDMIndex = 0;
						//Calcul des PDM des groupes de la catégorie précédente
						for (k = 1; k <= nbAdervertiser.Count; k++) {
							tab[currentCategoryIndex + k, FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX] = (double)tab[currentCategoryIndex + k, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] / (double)tab[currentCategoryIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] * 100.0;
						}
					}
					// Préparation des PDM des Categories
					tabCategoryIndex[currentCategoryPDMIndex] = currentLineIndex + 1;
					currentCategoryPDMIndex++;

					currentLineIndex++;
					tab[currentLineIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX] = currentRow["category"].ToString();
					tab[currentLineIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] = (double)0.0;
					//MAJ GR : initialisation totaux années
					foreach (object o in YEARS_INDEX.Keys) {
						tab[currentLineIndex, int.Parse(YEARS_INDEX[o].ToString())] = (double)0.0;
					}
					//fin MAJ
					currentCategoryIndex = currentLineIndex;
					oldIdCategory = Int64.Parse(currentRow["id_category"].ToString());
					numberOflineToAdd++;
					tab[currentLineIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX] = null;
					tab[currentLineIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.MEDIA_COLUMN_INDEX] = null;
					tab[currentLineIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX] = null;
					tab[currentLineIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.PERIODICITY_COLUMN_INDEX] = null;
					tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX] = null;


					if ((oldIdVehicle != (Int64)VehicleClassification.DB.Vehicles.names.outdoor && oldIdCategory != 35) || (oldIdVehicle == (Int64)VehicleClassification.DB.Vehicles.names.outdoor && webSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG))) {
						tab[currentLineIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] = Int64.Parse(currentRow["id_vehicle"].ToString());
						tab[currentLineIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX] = Int64.Parse(currentRow["id_category"].ToString());
						tab[currentLineIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX] = Int64.Parse(currentRow["id_media"].ToString());
					}
					else {
						tab[currentLineIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] = null;
						tab[currentLineIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX] = null;
						tab[currentLineIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX] = null;
					}
					// Totaux Annonceur au niveau de la categorie
					for (j = 1; j <= nbAdervertiser.Count; j++) {
						//tab[currentLineIndex + j, FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX] = nbAdervertiser[j].ToString();
						tab[currentLineIndex + j, FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX] = nbAdervertiser[j-1].ToString();
						tab[currentLineIndex + j, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] = (double)0.0;
						tab[currentLineIndex + j, FrameWorkResultConstantes.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX] = null;
						tab[currentLineIndex + j, FrameWorkResultConstantes.CompetitorMediaPlanAlert.MEDIA_COLUMN_INDEX] = null;
						tab[currentLineIndex + j, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX] = null;
						//MAJ GR : initialisation totaux années
						foreach (object o in YEARS_INDEX.Keys) {
							tab[currentLineIndex + j, int.Parse(YEARS_INDEX[o].ToString())] = (double)0.0;
						}
						//fin MAJ												
					}


					currentLineIndex = currentLineIndex + nbAdervertiser.Count;

				}
				// Nouveau Media
				if (oldIdMedia != Int64.Parse(currentRow["id_media"].ToString())) {
					if (oldIdMedia != 0) {
						for (i = currentMediaIndex + 1; i <= currentLineIndex; i++) {
							tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX] = (double)tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] / (double)tab[currentMediaIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] * 100.0;
						}
					}
					// Préparation des PDM des médias
					tabMediaIndex[currentMediaPDMIndex] = currentLineIndex + 1;
					currentMediaPDMIndex++;

					currentLineIndex++;
					tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.MEDIA_COLUMN_INDEX] = currentRow["media"].ToString();
					tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] = (double)0.0;
					foreach (object o in YEARS_INDEX.Keys) {
						tab[currentLineIndex, int.Parse(YEARS_INDEX[o].ToString())] = (double)0.0;
					}
					//fin MAJ
					currentMediaIndex = currentLineIndex;
					oldIdMedia = Int64.Parse(currentRow["id_media"].ToString());
					oldIdAdvertiser = -1;// MAJ DM
					numberOflineToAdd++;
					tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX] = null;
					tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX] = null;
					tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX] = null;

					tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX] = Int64.Parse(currentRow["id_category"].ToString());
					// Chaine thématique
					if ((oldIdVehicle != (Int64)VehicleClassification.DB.Vehicles.names.outdoor && oldIdCategory != 35) || (oldIdVehicle == (Int64)VehicleClassification.DB.Vehicles.names.outdoor && webSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG))) {
						tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX] = Int64.Parse(currentRow["id_media"].ToString());
					}
					else {
						tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX] = null;
					}

					tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] = Int64.Parse(currentRow["id_vehicle"].ToString());



				}

				// Advertiser
				if (oldIdAdvertiser != Int64.Parse(currentRow["grpunivers"].ToString())) {
					currentLineIndex++;
					tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX] = currentRow["grp"].ToString();
					//Périodicité Hebdomadaire pour OutDoor
					if (currentRow["id_vehicle"].ToString().Equals(DBClassificationConstantes.Vehicles.names.outdoor.GetHashCode().ToString())
						&& currentRow["id_periodicity"].ToString().Equals(DBClassificationConstantes.Periodicity.type.indetermine.GetHashCode().ToString())
						) {
						tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.PERIODICITY_COLUMN_INDEX] = DBClassificationConstantes.Periodicity.type.hebdomadaire.GetHashCode().ToString();
					}
					else
						tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.PERIODICITY_COLUMN_INDEX] = currentRow["id_periodicity"].ToString();
					tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] = (double)0.0;
					tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_ADVERTISER_COLUMN_INDEX] = null;
					tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_ADVERTISER_COLUMN_INDEX] = Int64.Parse(currentRow["grpunivers"].ToString());

					tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] = Int64.Parse(currentRow["id_vehicle"].ToString());
					tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX] = Int64.Parse(currentRow["id_category"].ToString());
					//MAJ GR : initialisation totaux années
					foreach (object o in YEARS_INDEX.Keys) {
						tab[currentLineIndex, int.Parse(YEARS_INDEX[o].ToString())] = (double)0.0;
					}
					//fin MAJ

					//changement oldIdMedia->oldIdAdvertiser
					if ((oldIdVehicle != (Int64)VehicleClassification.DB.Vehicles.names.outdoor && oldIdCategory != 35) || (oldIdVehicle == (Int64)VehicleClassification.DB.Vehicles.names.outdoor && webSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG))) {
						tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX] = Int64.Parse(currentRow["id_media"].ToString());
					}
					else {
						tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX] = null;
					}


					oldIdAdvertiser = Int64.Parse(currentRow["grpunivers"].ToString());
					currentDate = 0;
					tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX] = null;
					tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX] = null;
					tab[currentLineIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.MEDIA_COLUMN_INDEX] = null;
				}

				try {
					//currentDate = 0;// MAJ DM 25/11/07
					while (((DateTime)periodItemsList[currentDate]).ToString("yyyyMMdd") != currentRow["date_num"].ToString()) {
						tab[currentLineIndex, FIRST_PERIOD_INDEX + currentDate] = false;
						currentDate++;
					}
				}
				catch (System.Exception e) {
					Console.Write(e.Message);
				}
				tab[currentLineIndex, FIRST_PERIOD_INDEX + currentDate] = true;

				// Période totaux Annonceur
				//tab[currentTotalIndex + Int64.Parse(currentRow["grpunivers"].ToString()), FIRST_PERIOD_INDEX + currentDate] = true;
				//tab[currentMediaIndex, FIRST_PERIOD_INDEX + currentDate] = true;
				//tab[currentCategoryIndex + Int64.Parse(currentRow["grpunivers"].ToString()), FIRST_PERIOD_INDEX + currentDate] = true;
				//tab[currentVehicleIndex + Int64.Parse(currentRow["grpunivers"].ToString()), FIRST_PERIOD_INDEX + currentDate] = true;
				tab[currentTotalIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, FIRST_PERIOD_INDEX + currentDate] = true;
				tab[currentMediaIndex, FIRST_PERIOD_INDEX + currentDate] = true;
				tab[currentCategoryIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, FIRST_PERIOD_INDEX + currentDate] = true;
				tab[currentVehicleIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, FIRST_PERIOD_INDEX + currentDate] = true;


				unit = double.Parse(currentRow["unit"].ToString());
				tab[currentLineIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] = (double)tab[currentLineIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] + unit;
				tab[currentMediaIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] = (double)tab[currentMediaIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] + unit;
				tab[currentCategoryIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] = (double)tab[currentCategoryIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] + unit;
				tab[currentVehicleIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] = (double)tab[currentVehicleIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] + unit;
				tab[currentTotalIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] = (double)tab[currentTotalIndex, FrameWorkConstantes.Results.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] + unit;
				//MAJ GR : totaux par année
				c = int.Parse(currentRow["date_num"].ToString().Substring(0, 4));
				if (YEARS_INDEX.Count > 0) {
					c = int.Parse(YEARS_INDEX[c].ToString());
					tab[currentLineIndex, c] = (double)tab[currentLineIndex, c] + unit;
					tab[currentMediaIndex, c] = (double)tab[currentMediaIndex, c] + unit;
					tab[currentCategoryIndex, c] = (double)tab[currentCategoryIndex, c] + unit;
					tab[currentVehicleIndex, c] = (double)tab[currentVehicleIndex, c] + unit;
					tab[currentTotalIndex, c] = (double)tab[currentTotalIndex, c] + unit;
				}
				//fin MAJ

				currentDate++;
				oldCurrentDate = currentDate;
				while (oldCurrentDate < periodItemsList.Count) {
					tab[currentLineIndex, FIRST_PERIOD_INDEX + oldCurrentDate] = false;
					oldCurrentDate++;
				}

				//Essai totaux + création
				//tab[currentCategoryIndex + Int64.Parse(currentRow["grpunivers"].ToString()), FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] = (double)tab[currentCategoryIndex + Int64.Parse(currentRow["grpunivers"].ToString()), FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] + unit;
				//tab[currentCategoryIndex + Int64.Parse(currentRow["grpunivers"].ToString()), FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_ADVERTISER_COLUMN_INDEX] = Int64.Parse(currentRow["grpunivers"].ToString());
				tab[currentCategoryIndex + Int64.Parse(currentRow["grpunivers"].ToString())+1, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] = (double)tab[currentCategoryIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] + unit;
				tab[currentCategoryIndex + Int64.Parse(currentRow["grpunivers"].ToString())+1, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_ADVERTISER_COLUMN_INDEX] = Int64.Parse(currentRow["grpunivers"].ToString()) + 1;
				//MAJ GR : totaux par année
				c = int.Parse(currentRow["date_num"].ToString().Substring(0, 4));
				if (YEARS_INDEX.Count > 0) {
					c = int.Parse(YEARS_INDEX[c].ToString());
					#region ancienne version
					//tab[currentCategoryIndex + Int64.Parse(currentRow["grpunivers"].ToString()), c] = (double)tab[currentCategoryIndex + Int64.Parse(currentRow["grpunivers"].ToString()), c] + unit;
					//tab[currentVehicleIndex + Int64.Parse(currentRow["grpunivers"].ToString()), c] = (double)tab[currentVehicleIndex + Int64.Parse(currentRow["grpunivers"].ToString()), c] + unit;
					//tab[currentTotalIndex + Int64.Parse(currentRow["grpunivers"].ToString()), c] = (double)tab[currentTotalIndex + Int64.Parse(currentRow["grpunivers"].ToString()), c] + unit;
					#endregion
					tab[currentCategoryIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, c] = (double)tab[currentCategoryIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, c] + unit;
					tab[currentVehicleIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, c] = (double)tab[currentVehicleIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, c] + unit;
					tab[currentTotalIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, c] = (double)tab[currentTotalIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, c] + unit;
				}
				//fin MAJ
				#region ancienne version
				//if ((oldIdVehicle != (Int64)VehicleClassification.DB.Vehicles.names.outdoor && oldIdCategory != 35) || (oldIdVehicle == (Int64)VehicleClassification.DB.Vehicles.names.outdoor && webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG) != null)) {
				//    tab[currentCategoryIndex + Int64.Parse(currentRow["grpunivers"].ToString()), FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX] = Int64.Parse(currentRow["id_category"].ToString());
				//}
				//else {
				//    tab[currentCategoryIndex + Int64.Parse(currentRow["grpunivers"].ToString()), FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX] = null;
				//}
				//tab[currentCategoryIndex + Int64.Parse(currentRow["grpunivers"].ToString()), FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] = Int64.Parse(currentRow["id_vehicle"].ToString());

				//tab[currentVehicleIndex + Int64.Parse(currentRow["grpunivers"].ToString()), FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] = (double)tab[currentVehicleIndex + Int64.Parse(currentRow["grpunivers"].ToString()), FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] + unit;
				//tab[currentVehicleIndex + Int64.Parse(currentRow["grpunivers"].ToString()), FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_ADVERTISER_COLUMN_INDEX] = Int64.Parse(currentRow["grpunivers"].ToString());

				//tab[currentTotalIndex + Int64.Parse(currentRow["grpunivers"].ToString()), FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] = (double)tab[currentTotalIndex + Int64.Parse(currentRow["grpunivers"].ToString()), FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] + unit;
				#endregion
				if ((oldIdVehicle != (Int64)VehicleClassification.DB.Vehicles.names.outdoor && oldIdCategory != 35) || (oldIdVehicle == (Int64)VehicleClassification.DB.Vehicles.names.outdoor && webSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG) )) {
					tab[currentCategoryIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX] = Int64.Parse(currentRow["id_category"].ToString());
				}
				else {
					tab[currentCategoryIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX] = null;
				}
				tab[currentCategoryIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] = Int64.Parse(currentRow["id_vehicle"].ToString());

				tab[currentVehicleIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] = (double)tab[currentVehicleIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] + unit;
				tab[currentVehicleIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_ADVERTISER_COLUMN_INDEX] = Int64.Parse(currentRow["grpunivers"].ToString()) + 1;

				tab[currentTotalIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] = (double)tab[currentTotalIndex + Int64.Parse(currentRow["grpunivers"].ToString()) + 1, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] + unit;
			}
			#endregion

			#region Calcul des PDM de FIN
			if (nbVehicle > 0) {

				// PDM des média
				if (oldIdCategory != 0) {
					for (i = currentCategoryIndex + 1; i <= currentLineIndex; i++) {
						tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX] = (double)tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] / (double)tab[currentCategoryIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] * 100.0;
					}
				}
				// PDM des catégories
				for (i = 0; i < currentCategoryPDMIndex; i++) {
					tab[tabCategoryIndex[i], FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX] = (double)tab[tabCategoryIndex[i], FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] / (double)tab[currentVehicleIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] * 100.0;
				}
				// PDM des vehicles
				for (i = 0; i < currentVehiclePDMIndex; i++) {
					tab[tabVehicleIndex[i], FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX] = (double)tab[tabVehicleIndex[i], FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] / (double)tab[currentTotalIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] * 100.0;
				}
				for (k = 1; k <= nbAdervertiser.Count; k++) {
					tab[currentVehicleIndex + k, FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX] = (double)tab[currentVehicleIndex + k, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] / (double)tab[currentVehicleIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] * 100.0;
				}

				// PDM du Total
				tab[currentTotalIndex, FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX] = (double)100.0;
				//Calcul des PDM des groupes du total
				for (k = 1; k <= nbAdervertiser.Count; k++) {
					tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX + k, FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX] = (double)tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX + k, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] / (double)tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX, FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX] * 100.0;
				}

			}
			#endregion

			#region Ecriture de fin de tableau
			nbCol = tab.GetLength(1);
			nbline = tab.GetLength(0);
			if (currentLineIndex + 1 < nbline)
				tab[currentLineIndex + 1, 0] = new TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd();
			#endregion

			ArrayList alVehicle = new ArrayList();
			ArrayList alCategory = new ArrayList();

			#region traitement de la périodicité
			for (i = 1; i < nbline; i++) {
				if (tab[i, 0] != null) if (tab[i, 0].GetType() == typeof(FrameWorkConstantes.MemoryArrayEnd)) break;
				// C'est une ligne Vehicle
				//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX]!=null)currentVehicleIndex=i;
				// C'est une ligne Vehicle
				if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX] != null) {
					//	currentVehicleIndex=i;
					alVehicle.Clear();
					for (k = 0; k <= nbAdervertiser.Count; k++) {
						alVehicle.Add((int)(i - (nbAdervertiser.Count - k)));
					}
				}

				// C'est une ligne Category
				//if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]!=null)currentCategoryIndex=i;
				// C'est une ligne Category
				if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX] != null) {
					//currentCategoryIndex=i;
					alCategory.Clear();
					for (k = 0; k <= nbAdervertiser.Count; k++) {
						alCategory.Add((int)(i - (nbAdervertiser.Count - k)));
					}
				}

				// C'est une ligne Média
				if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.MEDIA_COLUMN_INDEX] != null) currentMediaIndex = i;

				// C'est une ligne univers annonceurs
				if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX] != null) {
					for (j = FIRST_PERIOD_INDEX; j < nbCol; j++) {
						if ((bool)tab[i, j] == true) {
							tab[i, j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.present;
							// Categorie							
							tab[(int)alCategory[0], (int)j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.present;
							for (k = 1; k <= nbAdervertiser.Count; k++) {

								//if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k].ToString()) {
								//    tab[(int)alCategory[(int)k], (int)j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.present;
								//}
								if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
									tab[(int)alCategory[(int)k], (int)j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.present;
								}
							}

							// Vehicle
							tab[(int)alVehicle[0], (int)j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.present;
							for (k = 1; k <= nbAdervertiser.Count; k++) {
								//if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k].ToString()) {
								//    tab[(int)alVehicle[(int)k], (int)j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.present;
								//}
								if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
									tab[(int)alVehicle[(int)k], (int)j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.present;
								}
							}
							// Total
							tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX, j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.present;
							for (k = 1; k <= nbAdervertiser.Count; k++) {
								//if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k].ToString()) {
								//    tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX + k, (int)j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.present;
								//}
								if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
									tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX + k, (int)j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.present;
								}
							}

							tab[currentMediaIndex, j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.present;

							switch (int.Parse(tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.PERIODICITY_COLUMN_INDEX].ToString())) {
								case (int)DBClassificationConstantes.Periodicity.type.mensuel:
									j++;
									while (j < nbCol && int.Parse(((DateTime)tab[FrameWorkConstantes.Results.CompetitorMediaPlanAlert.PERIOD_LINE_INDEX, j]).ToString("dd")) != 1) {
										tab[i, j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
										// Total
										if (tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX, j] == null || tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX, j].GetType() == typeof(bool)) tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX, j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
										for (k = 1; k <= nbAdervertiser.Count; k++) {
											//if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k].ToString()) {
											//    if (tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX + k, (int)j] == null || tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX + k, (int)j].GetType() == typeof(bool)) {
											//        tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX + k, (int)j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
											//    }
											//}
											if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
												if (tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX + k, (int)j] == null || tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX + k, (int)j].GetType() == typeof(bool)) {
													tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX + k, (int)j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
												}
											}
										}
										// Vehicle
										//if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
										if (tab[(int)alVehicle[0], j] == null || tab[(int)alVehicle[0], j].GetType() == typeof(bool)) tab[(int)alVehicle[0], j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
										for (k = 1; k <= nbAdervertiser.Count; k++) {
											//if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k].ToString()) {
											//    if (tab[(int)alVehicle[(int)k], j] == null || tab[(int)alVehicle[(int)k], j].GetType() == typeof(bool)) {
											//        tab[(int)alVehicle[(int)k], j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
											//    }
											//}
											if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
												if (tab[(int)alVehicle[(int)k], j] == null || tab[(int)alVehicle[(int)k], j].GetType() == typeof(bool)) {
													tab[(int)alVehicle[(int)k], j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
												}
											}
										}
										// Categorie
										if (tab[(int)alCategory[0], j] == null || tab[(int)alCategory[0], j].GetType() == typeof(bool)) tab[(int)alCategory[0], j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
										for (k = 1; k <= nbAdervertiser.Count; k++) {
											//if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k].ToString()) {
											//    if (tab[(int)alCategory[(int)k], j] == null || tab[(int)alCategory[(int)k], j].GetType() == typeof(bool)) {
											//        tab[(int)alCategory[(int)k], j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
											//    }
											//}
											if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
												if (tab[(int)alCategory[(int)k], j] == null || tab[(int)alCategory[(int)k], j].GetType() == typeof(bool)) {
													tab[(int)alCategory[(int)k], j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
												}
											}

										}
										// Media
										if (tab[currentMediaIndex, j] == null || tab[currentMediaIndex, j].GetType() == typeof(bool)) tab[currentMediaIndex, j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
										j++;
									}
									j--;
									break;
								case (int)DBClassificationConstantes.Periodicity.type.bimestriel:
									j++;
									nbMonth = 0;
									while (j < nbCol && nbMonth < DBClassificationConstantes.Periodicity.MONTH_NUMBER_IN_BIMONTHLY) {
										while (j < nbCol && int.Parse(((DateTime)tab[FrameWorkConstantes.Results.CompetitorMediaPlanAlert.PERIOD_LINE_INDEX, j]).ToString("dd")) != 1) {
											tab[i, j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
											// Total
											if (tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX, j] == null || tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX, j].GetType() == typeof(bool)) tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX, j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
											for (k = 1; k <= nbAdervertiser.Count; k++) {
												//if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k].ToString()) {
												if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
													if (tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX + k, (int)j] == null || tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX + k, (int)j].GetType() == typeof(bool)) {
														tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX + k, (int)j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
													}
												}
											}
											// Vehicle
											if (tab[(int)alVehicle[0], j] == null || tab[(int)alVehicle[0], j].GetType() == typeof(bool)) tab[(int)alVehicle[0], j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
											for (k = 1; k <= nbAdervertiser.Count; k++) {
												//if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k].ToString()) {
												if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
													if (tab[(int)alVehicle[(int)k], j] == null || tab[(int)alVehicle[(int)k], j].GetType() == typeof(bool)) {
														tab[(int)alVehicle[(int)k], j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
													}
												}
											}
											// Categorie
											if (tab[(int)alCategory[0], j] == null || tab[(int)alCategory[0], j].GetType() == typeof(bool)) tab[(int)alCategory[0], j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
											for (k = 1; k <= nbAdervertiser.Count; k++) {
												//if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k].ToString()) {
												if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
													if (tab[(int)alCategory[(int)k], j] == null || tab[(int)alCategory[(int)k], j].GetType() == typeof(bool)) {
														tab[(int)alCategory[(int)k], j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
													}
												}
											}
											// Media
											if (tab[currentMediaIndex, j] == null || tab[currentMediaIndex, j].GetType() == typeof(bool)) tab[currentMediaIndex, j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
											j++;
										}
										nbMonth++;
									}
									j--;
									break;
								case (int)DBClassificationConstantes.Periodicity.type.trimestriel:
									j++;
									nbMonth = 0;
									while (nbMonth < DBClassificationConstantes.Periodicity.MONTH_NUMBER_IN_QUATERLY) {
										while (j < nbCol && int.Parse(((DateTime)tab[FrameWorkConstantes.Results.CompetitorMediaPlanAlert.PERIOD_LINE_INDEX, j]).ToString("dd")) != 1) {
											tab[i, j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
											// Total
											if (tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX, j] == null || tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX, j].GetType() == typeof(bool)) tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX, j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
											for (k = 1; k <= nbAdervertiser.Count; k++) {
												//if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k].ToString()) {
												if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
													if (tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX + k, (int)j] == null || tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX + k, (int)j].GetType() == typeof(bool)) {
														tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX + k, (int)j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
													}
												}
											}
											// Vehicle
											if (tab[(int)alVehicle[0], j] == null || tab[(int)alVehicle[0], j].GetType() == typeof(bool)) tab[(int)alVehicle[0], j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
											for (k = 1; k <= nbAdervertiser.Count; k++) {
												//if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k].ToString()) {
												if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
													if (tab[(int)alVehicle[(int)k], j] == null || tab[(int)alVehicle[(int)k], j].GetType() == typeof(bool)) {
														tab[(int)alVehicle[(int)k], j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
													}
												}
											}
											// Categorie
											if (tab[(int)alCategory[0], j] == null || tab[(int)alCategory[0], j].GetType() == typeof(bool)) tab[(int)alCategory[0], j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
											for (k = 1; k <= nbAdervertiser.Count; k++) {
												//if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k].ToString()) {
												if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
													if (tab[(int)alCategory[(int)k], j] == null || tab[(int)alCategory[(int)k], j].GetType() == typeof(bool)) {
														tab[(int)alCategory[(int)k], j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
													}
												}
											}
											// Média
											if (tab[currentMediaIndex, j] == null || tab[currentMediaIndex, j].GetType() == typeof(bool)) tab[currentMediaIndex, j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
											j++;
										}
										nbMonth++;
									}
									j--;
									break;
								case (int)DBClassificationConstantes.Periodicity.type.hebdomadaire:
									j++;
									nbDays = 0;
									while (j < nbCol && nbDays < DBClassificationConstantes.Periodicity.DAY_NUMBER_IN_WEEK - 1) {
										tab[i, j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
										// Total
										if (tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX, j] == null || tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX, j].GetType() == typeof(bool)) tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX, j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
										for (k = 1; k <= nbAdervertiser.Count; k++) {
											//if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k].ToString()) {
											if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
												if (tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX + k, (int)j] == null || tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX + k, (int)j].GetType() == typeof(bool)) {
													tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX + k, (int)j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
												}
											}
										}
										// Vehicle
										//if(tab[currentVehicleIndex,j]==null || tab[currentVehicleIndex,j].GetType()==typeof(bool))tab[currentVehicleIndex,j]=FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
										if (tab[(int)alVehicle[0], j] == null || tab[(int)alVehicle[0], j].GetType() == typeof(bool)) tab[(int)alVehicle[0], j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
										for (k = 1; k <= nbAdervertiser.Count; k++) {
											//if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k].ToString()) {
											if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
												if (tab[(int)alVehicle[(int)k], j] == null || tab[(int)alVehicle[(int)k], j].GetType() == typeof(bool)) {
													tab[(int)alVehicle[(int)k], j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
												}
											}
										}
										// Categorie
										if (tab[(int)alCategory[0], j] == null || tab[(int)alCategory[0], j].GetType() == typeof(bool)) tab[(int)alCategory[0], j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
										for (k = 1; k <= nbAdervertiser.Count; k++) {
											//if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k].ToString()) {
											if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
												if (tab[(int)alCategory[(int)k], j] == null || tab[(int)alCategory[(int)k], j].GetType() == typeof(bool)) {
													tab[(int)alCategory[(int)k], j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
												}
											}
										}
										// Media
										if (tab[currentMediaIndex, j] == null || tab[currentMediaIndex, j].GetType() == typeof(bool)) tab[currentMediaIndex, j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
										nbDays++;
										j++;
									}
									j--;
									break;
								case (int)DBClassificationConstantes.Periodicity.type.bimensuel:
									j++;
									nbDays = 0;
									while (j < nbCol && nbDays < DBClassificationConstantes.Periodicity.DAY_NUMBER_IN_BIWEEKLY - 1) {
										tab[i, j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
										// Total
										if (tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX, j] == null || tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX, j].GetType() == typeof(bool)) tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX, j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
										for (k = 1; k <= nbAdervertiser.Count; k++) {
											//if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k].ToString()) {
											if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
												if (tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX + k, (int)j] == null || tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX + k, (int)j].GetType() == typeof(bool)) {
													tab[FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_LINE_INDEX + k, (int)j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
												}
											}
										}
										// Vehicle
										if (tab[(int)alVehicle[0], j] == null || tab[(int)alVehicle[0], j].GetType() == typeof(bool)) tab[(int)alVehicle[0], j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
										for (k = 1; k <= nbAdervertiser.Count; k++) {
											//if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k].ToString()) {
											if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
												if (tab[(int)alVehicle[(int)k], j] == null || tab[(int)alVehicle[(int)k], j].GetType() == typeof(bool)) {
													tab[(int)alVehicle[(int)k], j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
												}
											}
										}
										// Categorie
										if (tab[(int)alCategory[0], j] == null || tab[(int)alCategory[0], j].GetType() == typeof(bool)) tab[(int)alCategory[0], j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
										for (k = 1; k <= nbAdervertiser.Count; k++) {
											//if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k].ToString()) {
											if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX].ToString() == nbAdervertiser[k-1].ToString()) {
												if (tab[(int)alCategory[(int)k], j] == null || tab[(int)alCategory[(int)k], j].GetType() == typeof(bool)) {
													tab[(int)alCategory[(int)k], j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
												}
											}
										}
										// Media
										if (tab[currentMediaIndex, j] == null || tab[currentMediaIndex, j].GetType() == typeof(bool)) tab[currentMediaIndex, j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.extended;
										nbDays++;
										j++;
									}
									j--;
									break;
								default:
									break;
							}
						}
						else {
							tab[i, j] = FrameWorkConstantes.Results.CompetitorMediaPlanAlert.graphicItemType.absent;
						}
					}
				}
			}
			#endregion


			ds.Dispose();
			#region Debug: Voir le tableau
			//			StringBuilder HTML=new StringBuilder();
			//			HTML.Append("<html><table><tr>");
			//			for(i=0;i<nbline;i++){
			//				for(j=0;j<nbCol;j++){
			//					if(tab[i,j]!=null)HTML.Append("<td>"+tab[i,j].ToString()+"</td>");
			//					else HTML.Append("<td>&nbsp;</td>");
			//				}
			//				HTML.Append("</tr><tr>");
			//			}
			//			HTML.Append("</tr></table></html>");
			#endregion

			return (tab);


		}
		#endregion
	}
}
