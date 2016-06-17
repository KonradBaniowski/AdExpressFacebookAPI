//#region Informations
//// Auteur: G. Facon 
//// Date de création: 01/12/2005 (Basé sur MediaPlanAnalysisRules
//// Date de modification: 06/07/2004 
//// Date de modification: 
//#endregion

//using System;
//using System.Data;
//using System.Collections;

//using TNS.AdExpress.Web.Core.Sessions;
//using TNS.AdExpress.Web.DataAccess.Results;
//using TNS.AdExpress.Constantes.Web;
//using TNS.FrameWork.Date;
//using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
//using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
//using WebConstantes=TNS.AdExpress.Constantes.Web;
//using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
//using WebException=TNS.AdExpress.Web.Exceptions;
//using TNS.AdExpress.Web.Core;
//using WebFunctions=TNS.AdExpress.Web.Functions;

//namespace TNS.AdExpress.Web.Rules.Results{
//    /// <summary>
//    /// Gestion metier des plans media
//    /// </summary>
//    public class MediaPlanRules{


//        /// <summary>
//        /// Fonction qui formate un DataSet en un tableau qui servira à faire un Calendrier d'actions
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Tableau résultat</returns>
//        public static object[,] GetFormattedTableWithMediaDetailLevel(WebSession webSession){
//            string periodBeginning;
//            string periodEnd;

//            if(webSession.DetailPeriod==WebConstantes.CustomerSessions.Period.DisplayLevel.dayly && webSession.PeriodBeginningDate.Length<8){
//                periodBeginning = WebFunctions.Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd");
//                periodEnd = WebFunctions.Dates.GetPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd");
//            }
//            else{
//                periodBeginning=webSession.PeriodBeginningDate;
//                periodEnd=webSession.PeriodEndDate;
			
//            }
//            return(GetFormattedTableWithMediaDetailLevel(webSession,periodBeginning,periodEnd,false));
		
//        }

//        /// <summary>
//        /// Fonction qui formate un DataSet en un tableau qui servira à faire un Calendrier d'actions
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="zoomDate">Zoom</param>
//        /// <returns>Tableau résultat</returns>
//        public static object[,] GetFormattedTableWithMediaDetailLevelForZoom(WebSession webSession,string zoomDate){

//            #region Formattage des dates sur 8 chiffres
//            string periodBeginning = WebFunctions.Dates.GetPeriodBeginningDate(zoomDate, webSession.PeriodType).ToString("yyyyMMdd");
//            string periodEnd = WebFunctions.Dates.GetPeriodEndDate(zoomDate, webSession.PeriodType).ToString("yyyyMMdd");
//            #endregion

//            return(GetFormattedTableWithMediaDetailLevel(webSession,periodBeginning,periodEnd,true));
		
//        }


//        /// <summary>
//        /// Fonction qui formate un DataSet en un tableau qui servira à faire un Calendrier d'actions
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="beginUserDate">Date utilisateur de début</param>
//        /// <param name="endUserDate">Date utilisateur de fin</param>
//        /// <param name="zoom">Mode Zoom</param>
//        /// <returns>Tableau résultat</returns>
//        public static object[,] GetFormattedTableWithMediaDetailLevel(WebSession webSession,string beginUserDate,string endUserDate,bool zoom){
			
//            DataSet ds=null;

//            #region Création du DataSet de résultats
//            WebConstantes.CustomerSessions.Period.DisplayLevel oldDisplayPeriod=webSession.DetailPeriod;
//            if(zoom){
//                // On force affichage jour (cas zoom)
//                webSession.DetailPeriod=WebConstantes.CustomerSessions.Period.DisplayLevel.dayly;
//                ds=MediaPlanDataAccess.GetPluriMediaDataSetWithMediaDetailLevel(webSession,beginUserDate,endUserDate);
//            }
//            else{
//                // On rappatrie les données à partir de la base de données
//                // Selon le type de détail période les requêtes sont différentes
//                // En analyse on se base sur les WebPlan
//                // En alerte In se base sur les tables désagrégés en 4M
//                switch(webSession.DetailPeriod){
//                    case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
//                    case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
//                        //ds=MediaPlanAnalysisDataAccess.GetDataWithMediaDetailLevel(webSession);
//                        ds=MediaPlanDataAccess.GetDataWithMediaDetailLevel(webSession);
//                        break;
//                    case WebConstantes.CustomerSessions.Period.DisplayLevel.dayly:
//                        ds=MediaPlanDataAccess.GetPluriMediaDataSetWithMediaDetailLevel(webSession,beginUserDate,endUserDate);
//                        break;
//                    default:
//                        throw( new WebException.MediaPlanRulesException("Détail période invalide"));
//                }
//            }
//            #endregion

//            #region Variables
//            if(ds==null || ds.Tables.Count==0 || ds.Tables[0]==null){
//                if(zoom)webSession.DetailPeriod=oldDisplayPeriod;
//                return(new object[0,0]);
//            }
//            DataTable dt=ds.Tables[0];
//            int nbL1=0;
//            int nbL2=0;
//            int nbL3=0;
//            int nbL4=0;
//            Int64 oldIdL1=0;
//            Int64 oldIdL2=0;
//            Int64 oldIdL3=0;
//            Int64 oldIdL4=0;
//            Int64 currentLineIndex=1;
//            Int64 currentTotalIndex=1;
//            Int64 currentL1Index=2;
//            Int64 currentL2Index=0;
//            Int64 currentL3Index=1;
//            Int64 currentL4Index=1;
//            Int64 currentL3PDMIndex=0;
//            Int64 currentL2PDMIndex=0;
//            Int64 currentL1PDMIndex=0;
//            bool forceEntry=true;
//            AtomicPeriodWeek weekDate=null;
//            double unit=0.0;
//            int currentDate=0;
//            int oldCurrentDate=0;
//            Int64 i;
//            int numberOflineToAdd=0;
//            int nbCol=0;
//            int nbline=0;
//            int k,mpi,nbDays,nbMonth = 0;
//            bool forceL2=false;
//            bool forceL3=false;
//            bool forceL4=false;
//            #endregion


//            #region Année
//            //MAJ GR : Colonnes totaux par année si nécessaire
//            //FIRST_PERIOD_INDEX a remplacé FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX dans toute la fonction
//            Hashtable YEARS_INDEX = new Hashtable();
//            currentDate = int.Parse(beginUserDate.Substring(0,4));
//            oldCurrentDate = int.Parse(endUserDate.Substring(0,4));
//            int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX;
//            if (currentDate!=oldCurrentDate) {
//                for(k=currentDate; k<=oldCurrentDate; k++) {
//                    YEARS_INDEX.Add(k,FIRST_PERIOD_INDEX);
//                    FIRST_PERIOD_INDEX++;
//                }
//            }
//            currentDate = 0;
//            oldCurrentDate = 0;
//            //fin MAJ
//            #endregion

//            #region Compte le nombre d'éléments pour chaque niveau media
//            // On compte le nombre d'éléments par niveau Pour construire la tableau final
//            int nbLevels=GetNbLevel(webSession);
//            foreach(DataRow currentRow in dt.Rows){
//                if(nbLevels>=1 && oldIdL1!=GetL1Id(webSession,currentRow)){
//                    forceL2=true;
//                    nbL1++;
//                    oldIdL1=GetL1Id(webSession,currentRow);
//                }
//                if(nbLevels>=2 && (oldIdL2!=GetL2Id(webSession,currentRow)||forceL2)){
//                    forceL3=true;
//                    forceL2=false;
//                    nbL2++;
//                    oldIdL2=GetL2Id(webSession,currentRow);
//                }
//                if(nbLevels>=3 && (oldIdL3!=GetL3Id(webSession,currentRow)||forceL3)){
//                    forceL4=true;
//                    forceL3=false;
//                    nbL3++;
//                    oldIdL3=GetL3Id(webSession,currentRow);
//                }
//                if(nbLevels>=4 && (oldIdL4!=GetL4Id(webSession,currentRow)||forceL4)){
//                    forceL4=false;
//                    nbL4++;
//                    oldIdL4=GetL4Id(webSession,currentRow);
//                }
//            }
//            forceL2=forceL3=forceL4=false;
//            oldIdL1=oldIdL2=oldIdL3=oldIdL4=-1;
//            #endregion

//            // Il n'y a pas de données
//            if(nbL1==0){
//                if(zoom)webSession.DetailPeriod=oldDisplayPeriod;
//                return(new object[0,0]);
//            }

//            #region Création du tableau des mois ou semaine
//            string tmpDate;
//            ArrayList periodItemsList=new ArrayList();
//            switch(webSession.DetailPeriod){
//                case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
//                    AtomicPeriodWeek currentWeek=new AtomicPeriodWeek(int.Parse(beginUserDate.Substring(0,4)),int.Parse(beginUserDate.Substring(4,2)));
//                    AtomicPeriodWeek endWeek=new AtomicPeriodWeek(int.Parse(endUserDate.Substring(0,4)),int.Parse(endUserDate.Substring(4,2)));
//                    endWeek.Increment();
//                    while(!(currentWeek.Week==endWeek.Week && currentWeek.Year==endWeek.Year)){
//                        tmpDate=currentWeek.Year.ToString();
//                        if(currentWeek.Week.ToString().Length<2)tmpDate+="0"+currentWeek.Week.ToString();
//                        else tmpDate+=currentWeek.Week.ToString();
//                        periodItemsList.Add(tmpDate);
//                        currentWeek.Increment();
//                    }
//                    break;
//                case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
//                    DateTime dateCurrent=new DateTime(int.Parse(beginUserDate.Substring(0,4)),int.Parse(beginUserDate.Substring(4,2)),1);
//                    DateTime dateEnd=new DateTime(int.Parse(endUserDate.Substring(0,4)),int.Parse(endUserDate.Substring(4,2)),1);
//                    dateEnd=dateEnd.AddMonths(1);
//                    while(!(dateCurrent.Month==dateEnd.Month && dateCurrent.Year==dateEnd.Year)){
//                        tmpDate=dateCurrent.Year.ToString();
//                        if(dateCurrent.Month.ToString().Length<2)tmpDate+="0"+dateCurrent.Month.ToString();
//                        else tmpDate+=dateCurrent.Month.ToString();
//                        periodItemsList.Add(tmpDate);
//                        dateCurrent=dateCurrent.AddMonths(1);
//                    }
//                    break;
//                case WebConstantes.CustomerSessions.Period.DisplayLevel.dayly:
//                        DateTime currentDateTime =  DateString.YYYYMMDDToDateTime(beginUserDate);
//                        DateTime endDate = DateString.YYYYMMDDToDateTime(endUserDate);
//                        while(currentDateTime<=endDate){
//                            periodItemsList.Add(DateString.DateTimeToYYYYMMDD(currentDateTime));
//                            currentDateTime = currentDateTime.AddDays(1);
//                        }
//                    break;
//                default:
//                    throw(new WebException.MediaPlanRulesException("Impossible de construire le tableau des date"));
//            }
//            #endregion

//            #region Déclaration des tableaux
//            // Nombre de colonnes
//            nbCol=periodItemsList.Count+FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX+YEARS_INDEX.Count;
//            // Nombre de lignes
//            nbline=nbL1+nbL2+nbL3+nbL4+3;
//            // Tableau de résultat
//            object[,] tab=new object[nbline,nbCol];
//            // Tableau des indexes du niveau L3
//            Int64[] tabL3Index=new Int64[nbL3+1];
//            // Tableau des indexes du niveau L2
//            Int64[] tabL2Index=new Int64[nbL2+1];
//            // Tableau des indexes des niveau L1
//            Int64[] tabL1Index=new Int64[nbL1+1];
//            #endregion
			
//            #region Libellé des colonnes
//            while(currentDate<periodItemsList.Count){
//                tab[0,currentDate+FIRST_PERIOD_INDEX]=(string)periodItemsList[currentDate];
//                currentDate++;
//            }
//            foreach(object o in YEARS_INDEX.Keys) {
//                tab[0,int.Parse(YEARS_INDEX[o].ToString())] = o.ToString();
//            }
//            #endregion

//            #region Initialisation des totaux
//            tab[currentTotalIndex,0]=FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_STRING;
//            tab[currentTotalIndex,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]=(double)0.0;
//            foreach(object o in YEARS_INDEX.Keys) {
//                tab[currentLineIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
//            }
//            #endregion

//            #region Construction du tableau de résultat 

//            #region Création des MediaPlanItem pour le total
//            for(mpi=FIRST_PERIOD_INDEX;mpi<nbCol;mpi++){
//                tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,mpi]=new MediaPlanItem((long)-1);	
//            }
//            #endregion
		
//            foreach(DataRow currentRow in dt.Rows) {

//                #region Nouveau Niveau L1
//                if(nbLevels>=1 && oldIdL1!=GetL1Id(webSession,currentRow)){
//                    // Le Prochain niveau L2 doit être différent
//                    forceL2=true;
//                    // Calcul des PDM du niveau L2
//                    if(oldIdL1!=-1){
//                        for(i=0;i<currentL2PDMIndex;i++){
//                            tab[tabL2Index[i],FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]=(double)tab[tabL2Index[i],FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[currentL1Index,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]*100.0;
//                        }
//                        tabL2Index=new Int64[nbL2+1];
//                        currentL2PDMIndex=0;
//                    }
//                    // Préparation des PDM des vehicles
//                    tabL1Index[currentL1PDMIndex]=currentLineIndex+1;
//                    currentL1PDMIndex++;

//                    currentLineIndex++;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L1_COLUMN_INDEX]=GetL1Label(webSession,currentRow);
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX]=GetL1Id(webSession,currentRow);
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]=(double)0.0;
//                    if(nbLevels<=1) tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.PERIODICITY_COLUMN_INDEX]=currentRow["id_periodicity"].ToString();
//                    //MAJ GR : initialisation totaux années
//                    foreach(object o in YEARS_INDEX.Keys) {
//                        tab[currentLineIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
//                    }
//                    //fin MAJ
//                    currentL1Index=currentLineIndex;
//                    oldIdL1=GetL1Id(webSession,currentRow);
//                    currentDate=0;
//                    numberOflineToAdd++;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L4_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L4_ID_COLUMN_INDEX]=null;
//                    // Création des MediaPlanItem
//                    for(mpi=FIRST_PERIOD_INDEX;mpi<nbCol;mpi++){
//                        tab[currentLineIndex,mpi]=new MediaPlanItem((long)-1);	
//                    }

//                }
//                #endregion

//                #region Nouveau Niveau L2
//                if(nbLevels>=2 && (oldIdL2!=GetL2Id(webSession,currentRow) || forceL2)){
//                    // Le Prochain niveau L3 doit être différent
//                    forceL3=true;
//                    forceL2=false;
//                    //Calcul des PDM du niveau L3
//                    if(oldIdL2!=-1){
//                        for(i=0;i<currentL3PDMIndex;i++){
//                            tab[tabL3Index[i],FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]=(double)tab[tabL3Index[i],FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[currentL2Index,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]*100.0;
//                        }
//                        tabL3Index=new Int64[nbL3+1];
//                        currentL3PDMIndex=0;

////						for(i=currentL2Index+1;i<=currentLineIndex;i++){
////							tab[i,FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]=(double)tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[currentL2Index,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]*100.0;
////						}
//                    }
//                    // Préparation des PDM des L2
//                    tabL2Index[currentL2PDMIndex]=currentLineIndex+1;
//                    currentL2PDMIndex++;

//                    currentLineIndex++;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX]=GetL2Label(webSession,currentRow);
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]=GetL2Id(webSession,currentRow);
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]=(double)0.0;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX]=oldIdL1;
//                    if(nbLevels<=2) tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.PERIODICITY_COLUMN_INDEX]=currentRow["id_periodicity"].ToString();
//                    //MAJ GR : initialisation totaux années
//                    foreach(object o in YEARS_INDEX.Keys) {
//                        tab[currentLineIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
//                    }
//                    //fin MAJ
//                    currentL2Index=currentLineIndex;
//                    oldIdL2=GetL2Id(webSession,currentRow);
//                    currentDate=0;
//                    numberOflineToAdd++;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L1_COLUMN_INDEX]=null;
					
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L4_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L4_ID_COLUMN_INDEX]=null;
//                    // Création des MediaPlanItem
//                    for(mpi=FIRST_PERIOD_INDEX;mpi<nbCol;mpi++){
//                        tab[currentLineIndex,mpi]=new MediaPlanItem((long)-1);	
//                    }
//                }
//                #endregion

//                #region Nouveau Niveau L3
//                if(nbLevels>=3 && (oldIdL3!=GetL3Id(webSession,currentRow)|| forceL3)){
//                    // Le Prochain niveau L4 doit être différent
//                    forceL4=true;
//                    forceL3=false;
//                    //Calcul des PDM niveau L4
//                    if(oldIdL3!=-1){
//                        for(i=currentL3Index+1;i<=currentLineIndex;i++){
//                            tab[i,FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]=(double)tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[currentL3Index,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]*100.0;
//                        }
//                    }
//                    // Préparation des PDM des L3
//                    tabL3Index[currentL3PDMIndex]=currentLineIndex+1;
//                    currentL3PDMIndex++;

//                    currentLineIndex++;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX]=GetL3Label(webSession,currentRow);
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX]=GetL3Id(webSession,currentRow);
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]=(double)0.0;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX]=oldIdL1;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]=oldIdL2;
//                    if(nbLevels<=3) tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.PERIODICITY_COLUMN_INDEX]=currentRow["id_periodicity"].ToString();
//                    //MAJ GR : initialisation totaux années
//                    foreach(object o in YEARS_INDEX.Keys) {
//                        tab[currentLineIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
//                    }
//                    //fin MAJ
//                    currentL3Index=currentLineIndex;
//                    oldIdL3=GetL3Id(webSession,currentRow);
//                    currentDate=0;
//                    numberOflineToAdd++;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L1_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX]=null;
					
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L4_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L4_ID_COLUMN_INDEX]=null;
//                    // Création des MediaPlanItem
//                    for(mpi=FIRST_PERIOD_INDEX;mpi<nbCol;mpi++){
//                        tab[currentLineIndex,mpi]=new MediaPlanItem((long)-1);	
//                    }
//                }
//                #endregion

//                #region Nouveau Niveau L4
//                if(nbLevels>=4 && (oldIdL4!=GetL4Id(webSession,currentRow)|| forceL4)){
//                    forceL4=false;
//                    currentLineIndex++;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L4_COLUMN_INDEX]=GetL4Label(webSession,currentRow);
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L4_ID_COLUMN_INDEX]=GetL4Id(webSession,currentRow);
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]=(double)0.0;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX]=oldIdL1;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]=oldIdL2;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX]=oldIdL3;
//                    if(nbLevels<=4) tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.PERIODICITY_COLUMN_INDEX]=currentRow["id_periodicity"].ToString();
//                    //MAJ GR : initialisation totaux années
//                    foreach(object o in YEARS_INDEX.Keys) {
//                        tab[currentLineIndex,int.Parse(YEARS_INDEX[o].ToString())]=(double)0.0;
//                    }
//                    //fin MAJ
//                    currentL4Index=currentLineIndex;
//                    oldIdL4=GetL4Id(webSession,currentRow);
//                    currentDate=0;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L1_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX]=null;
//                    tab[currentLineIndex,FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX]=null;
//                    // Création des MediaPlanItem
//                    for(mpi=FIRST_PERIOD_INDEX;mpi<nbCol;mpi++){
//                        tab[currentLineIndex,mpi]=new MediaPlanItem((long)-1);	
//                    }
//                }
//                #endregion

//                #region Traitement des présences dans les dates
//                try{
//                    while(Int64.Parse(periodItemsList[currentDate].ToString())!=(Int64.Parse(currentRow["date_num"].ToString()))){
//                        //tab[currentLineIndex,FIRST_PERIOD_INDEX+currentDate]=false;
//                        tab[currentLineIndex,FIRST_PERIOD_INDEX+currentDate]= new MediaPlanItem((long)-1);
//                        currentDate++;
//                    }
//                }
//                catch(System.Exception e){
//                    Console.Write(e.Message);
//                }
//                //tab[currentLineIndex,FIRST_PERIOD_INDEX+currentDate]=true;
//                // On met la périodicité
//                unit = double.Parse(currentRow[WebFunctions.SQLGenerator.GetUnitAlias(webSession)].ToString());
//                tab[currentLineIndex,FIRST_PERIOD_INDEX+currentDate]=new MediaPlanItem(Int64.Parse(currentRow["id_periodicity"].ToString()));
//                //((MediaPlanItem)tab[currentLineIndex,FIRST_PERIOD_INDEX+currentDate]).Unit+=unit;
				
//                if(nbLevels>=4){
//                    tab[currentL4Index,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]=(double)tab[currentL4Index,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]+unit;
//                    ((MediaPlanItem)tab[currentL4Index,FIRST_PERIOD_INDEX+currentDate]).Unit+=unit;
//                }
//                if(nbLevels>=3){
//                    tab[currentL3Index,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]=(double)tab[currentL3Index,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]+unit;
//                    ((MediaPlanItem)tab[currentL3Index,FIRST_PERIOD_INDEX+currentDate]).Unit+=unit;
//                }
//                if(nbLevels>=2){
//                    tab[currentL2Index,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]=(double)tab[currentL2Index,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]+unit;
//                    ((MediaPlanItem)tab[currentL2Index,FIRST_PERIOD_INDEX+currentDate]).Unit+=unit;
//                }
//                if(nbLevels>=1){
//                    tab[currentL1Index,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]=(double)tab[currentL1Index,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]+unit;
//                    ((MediaPlanItem)tab[currentL1Index,FIRST_PERIOD_INDEX+currentDate]).Unit+=unit;
//                }
//                tab[currentTotalIndex,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]=(double)tab[currentTotalIndex,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]+unit;
//                ((MediaPlanItem)tab[currentTotalIndex,FIRST_PERIOD_INDEX+currentDate]).Unit+=unit;
				
//                //MAJ GR : totaux par année
//                k = int.Parse(currentRow["date_num"].ToString().Substring(0,4));
//                if (YEARS_INDEX.Count>0) {
//                    k = int.Parse(YEARS_INDEX[k].ToString());
//                    if(nbLevels>=4)tab[currentL4Index,k]=(double)tab[currentL4Index,k]+unit;
//                    if(nbLevels>=3)tab[currentL3Index,k]=(double)tab[currentL3Index,k]+unit;
//                    if(nbLevels>=2)tab[currentL2Index,k]=(double)tab[currentL2Index,k]+unit;
//                    if(nbLevels>=1)tab[currentL1Index,k]=(double)tab[currentL1Index,k]+unit;
//                    tab[currentTotalIndex,k]=(double)tab[currentTotalIndex,k]+unit;
//                }
//                //fin MAJ
//                currentDate++;
//                oldCurrentDate=currentDate;
//                // On peut suremement enlever cette partie à tester
//                while(oldCurrentDate<periodItemsList.Count){
//                    //tab[currentLineIndex,FIRST_PERIOD_INDEX+oldCurrentDate]=false;
//                    tab[currentLineIndex,FIRST_PERIOD_INDEX+currentDate]= new MediaPlanItem((long)-1);
//                    oldCurrentDate++;
//                }
//                //
//                #endregion
//            }
//            #endregion

//            #region Calcul des PDM de FIN
//            if(nbL1>0){
//                // PDM niveau L4
//                if(nbLevels>=4){
//                    for(i=currentL3Index+1;i<=currentLineIndex;i++){
//                        tab[i,FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]=(double)tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[currentL3Index,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]*100.0;
//                    }
//                }
//                // PDM niveau L3
//                if(nbLevels>=3){
//                    for(i=0;i<currentL3PDMIndex;i++){
//                        tab[tabL3Index[i],FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]=(double)tab[tabL3Index[i],FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[currentL2Index,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]*100.0;
//                    }
//                }
//                // PDM niveau L2
//                if(nbLevels>=2){
//                    for(i=0;i<currentL2PDMIndex;i++){
//                        tab[tabL2Index[i],FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]=(double)tab[tabL2Index[i],FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[currentL1Index,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]*100.0;
//                    }
//                }
//                // PDM niveau L1
//                if(nbLevels>=1){
//                    for(i=0;i<currentL1PDMIndex;i++){
//                        tab[tabL1Index[i],FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]=(double)tab[tabL1Index[i],FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]/(double)tab[currentTotalIndex,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]*100.0;
//                    }
//                }
//                // PDM du Total
//                tab[currentTotalIndex,FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]=(double)100.0;
//            }
//            #endregion


//            #region Debug: Voir le tableau
////#if(DEBUG)
////			System.Text.StringBuilder HTML= new System.Text.StringBuilder(2000);
////			HTML.Append("<html><table><tr>");
////			for(int z=0;z<=currentLineIndex;z++){
////				for(int r=0;r<nbCol;r++){
////					if(tab[z,r]!=null)HTML.Append("<td>"+tab[z,r].ToString()+"</td>");
////					else HTML.Append("<td>&nbsp;</td>");
////				}
////				HTML.Append("</tr><tr>");
////			}
////			HTML.Append("</tr></table></html>");
////			Console.WriteLine(HTML.ToString());
////#endif
//            #endregion


//            #region traitement de la périodicité
//            try{
//                for(i=1;i<nbline;i++){
//                    if(tab[i,0]!=null)if(tab[i,0].GetType()==typeof(FrameWorkConstantes.MemoryArrayEnd)) break;
//                    // C'est une ligne Niveau 1
//                    if(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.L1_COLUMN_INDEX]!=null)currentL1Index=i;
//                    // C'est une ligne Niveau 2
//                    if(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX]!=null)currentL2Index=i;
//                    // C'est une ligne Niveau 3
//                    if(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX]!=null)currentL3Index=i;
//                    // C'est une ligne Niveau 4
//                    if(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.L4_COLUMN_INDEX]!=null)currentL4Index=i;
//                    // On est sur le niveau le plus bas:
//                    if((nbLevels==1 && currentL1Index==i)||(nbLevels==2 && currentL2Index==i)||(nbLevels==3 && currentL3Index==i)||(nbLevels==4 && currentL4Index==i)){
//                        for(int j=FIRST_PERIOD_INDEX;j<nbCol;j++){
//                            if(((MediaPlanItem)tab[i,j]).PeriodicityId>=0){
//                                ((MediaPlanItem)tab[i,j]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present;
//                                if(nbLevels>3)((MediaPlanItem)tab[currentL3Index,j]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present;
//                                if(nbLevels>2)((MediaPlanItem)tab[currentL2Index,j]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present;
//                                if(nbLevels>1)((MediaPlanItem)tab[currentL1Index,j]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present;
//                                if(tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,j]==null)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,j]=new MediaPlanItem();
//                                ((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,j]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present;

							
//                                switch(webSession.DetailPeriod){

//                                        #region Plan media détaillé par mois
//                                    case Constantes.Web.CustomerSessions.Period.DisplayLevel.monthly:
//                                    switch(((MediaPlanItem)tab[i,j]).PeriodicityId){
//                                            #region Trimestirel
//                                        case (int)DBClassificationConstantes.Periodicity.type.trimestriel:
//                                            k=j+1; // On se place sur le suivant
//                                            if(k<nbCol){
//                                                ((MediaPlanItem)tab[i,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(nbLevels>1 && ((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(nbLevels>2 && ((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(nbLevels>3 && ((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                            }
//                                            k++;
//                                            if(k<nbCol){
//                                                ((MediaPlanItem)tab[i,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(nbLevels>1 && ((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(nbLevels>2 && ((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(nbLevels>3 && ((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                            }
//                                            break;
//                                            #endregion

//                                            #region Bimestriel
//                                        case (int)DBClassificationConstantes.Periodicity.type.bimestriel:
//                                            k=j+1; // On se place sur le suivant
//                                            if(k<nbCol){
//                                                ((MediaPlanItem)tab[i,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(nbLevels>1 && ((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(nbLevels>2 && ((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(nbLevels>3 && ((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                            }
//                                            break;
//                                            #endregion

//                                        default:
//                                            break;
//                                    }
//                                        break;
//                                        #endregion

//                                        #region Plan media détaillé par semaine
//                                    case Constantes.Web.CustomerSessions.Period.DisplayLevel.weekly:
//                                    switch(((MediaPlanItem)tab[i,j]).PeriodicityId){
//                                            #region Mensuel
//                                        case (int)DBClassificationConstantes.Periodicity.type.mensuel:
//                                            k=j+1; // On se place sur le suivant
//                                            if(k<nbCol){
//                                                weekDate=new AtomicPeriodWeek(int.Parse(tab[0,k].ToString().Substring(0,4)),int.Parse(tab[0,k].ToString().Substring(4,2)));
//                                                forceEntry=true;
//                                                while((weekDate.FirstDay.Day<weekDate.LastDay.Day && !((int)weekDate.FirstDay.DayOfWeek ==1 && weekDate.FirstDay.Day==1)) || forceEntry){
//                                                    forceEntry=false;
//                                                    if(k<nbCol){
//                                                        ((MediaPlanItem)tab[i,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                        if(((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                        if(nbLevels>1 && ((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                        if(nbLevels>2 && ((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                        if(nbLevels>3 && ((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                    }
//                                                    k++;
//                                                    if(k<nbCol) weekDate=new AtomicPeriodWeek(int.Parse(tab[0,k].ToString().Substring(0,4)),int.Parse(tab[0,k].ToString().Substring(4,2)));
//                                                    else break;
//                                                }
//                                                //if(j>FIRST_PERIOD_INDEX)j--;
//                                            }
//                                            break;
//                                            #endregion

//                                            #region Bimensuel
//                                        case (int)DBClassificationConstantes.Periodicity.type.bimensuel:
//                                            k=j+1; // On se place sur le suivant
//                                            if(k<nbCol){
//                                                ((MediaPlanItem)tab[i,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,j]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(nbLevels>1 && ((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(nbLevels>2 && ((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(nbLevels>3 && ((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                            }
//                                            break;
//                                            #endregion

//                                            #region trimestriel
//                                        case (int)DBClassificationConstantes.Periodicity.type.trimestriel:
//                                            k=j+1; // On se place sur le suivant
//                                            if(k<nbCol){
//                                                weekDate=new AtomicPeriodWeek(int.Parse(tab[0,k].ToString().Substring(0,4)),int.Parse(tab[0,k].ToString().Substring(4,2)));
//                                                for(int l=0;l<3;l++){
//                                                    forceEntry=true;
//                                                    while((weekDate.FirstDay.Day<weekDate.LastDay.Day && !((int)weekDate.FirstDay.DayOfWeek ==1 && weekDate.FirstDay.Day==1)) || forceEntry){
//                                                        forceEntry=false;
//                                                        if(k<nbCol){
//                                                            ((MediaPlanItem)tab[i,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                            if(((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                            if(nbLevels>1 && ((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                            if(nbLevels>2 && ((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                            if(nbLevels>3 && ((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                        }
//                                                        k++;
//                                                        if(k<nbCol) weekDate=new AtomicPeriodWeek(int.Parse(tab[0,k].ToString().Substring(0,4)),int.Parse(tab[0,k].ToString().Substring(4,2)));
//                                                        else break;
//                                                    }
//                                                }
//                                                //if(j>FIRST_PERIOD_INDEX)j--;
//                                            }
//                                            break;
//                                            #endregion

//                                            #region Bimestriel
//                                        case (int)DBClassificationConstantes.Periodicity.type.bimestriel:
//                                            k=j+1; // On se place sur le suivant
//                                            if(k<nbCol){
//                                                weekDate=new AtomicPeriodWeek(int.Parse(tab[0,k].ToString().Substring(0,4)),int.Parse(tab[0,k].ToString().Substring(4,2)));
//                                                for(int l=0;l<2;l++){
//                                                    forceEntry=true;
//                                                    while((weekDate.FirstDay.Day<weekDate.LastDay.Day && !((int)weekDate.FirstDay.DayOfWeek ==1 && weekDate.FirstDay.Day==1)) || forceEntry){
//                                                        forceEntry=false;
//                                                        if(k<nbCol){
//                                                            ((MediaPlanItem)tab[i,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                            if(((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                            if(nbLevels>1 && ((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                            if(nbLevels>2 && ((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                            if(nbLevels>3 && ((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                        }
//                                                        k++;
//                                                        if(k<nbCol) weekDate=new AtomicPeriodWeek(int.Parse(tab[0,k].ToString().Substring(0,4)),int.Parse(tab[0,k].ToString().Substring(4,2)));
//                                                        else break;
//                                                    }
//                                                }
//                                            }
//                                            break;
//                                            #endregion

//                                        default:
//                                            break;
//                                    }
//                                        break;
//                                        #endregion

//                                        #region Plan media détaillé par jour
//                                    case Constantes.Web.CustomerSessions.Period.DisplayLevel.dayly:
//                                    switch(((MediaPlanItem)tab[i,j]).PeriodicityId){
//                                        case (int)DBClassificationConstantes.Periodicity.type.mensuel:
//                                            k=j+1; // On se place sur le suivant
//                                            while (k<nbCol && int.Parse(((string)tab[FrameWorkConstantes.Results.MediaPlanAlert.PERIOD_LINE_INDEX,k]).Substring(6,2))!=1){
//                                                ((MediaPlanItem)tab[i,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(nbLevels>1 && ((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(nbLevels>2 && ((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(nbLevels>3 && ((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                k++;
//                                            }
//                                            break;
//                                        case (int)DBClassificationConstantes.Periodicity.type.bimestriel:
//                                            k=j+1; // On se place sur le suivant
//                                            nbMonth=0;
//                                            while(k<nbCol && nbMonth<DBClassificationConstantes.Periodicity.MONTH_NUMBER_IN_BIMONTHLY){
//                                                while (k<nbCol && int.Parse(((string)tab[FrameWorkConstantes.Results.MediaPlanAlert.PERIOD_LINE_INDEX,k]).Substring(6,2))!=1){
//                                                    ((MediaPlanItem)tab[i,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                    if(((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                    if(nbLevels>1 && ((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                    if(nbLevels>2 && ((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                    if(nbLevels>3 && ((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                    k++;
//                                                }
//                                                nbMonth++;
//                                            }
//                                            break;
//                                        case (int)DBClassificationConstantes.Periodicity.type.trimestriel:
//                                            k=j+1; // On se place sur le suivant
//                                            nbMonth=0;
//                                            while(k<nbCol && nbMonth<DBClassificationConstantes.Periodicity.MONTH_NUMBER_IN_QUATERLY){
//                                                while (k<nbCol && int.Parse(((string)tab[FrameWorkConstantes.Results.MediaPlanAlert.PERIOD_LINE_INDEX,k]).Substring(6,2))!=1){
//                                                    ((MediaPlanItem)tab[i,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                    if(((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                    if(nbLevels>1 && ((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                    if(nbLevels>2 && ((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                    if(nbLevels>3 && ((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                    k++;
//                                                }
//                                                nbMonth++;
//                                            }
//                                            break;
//                                        case (int)DBClassificationConstantes.Periodicity.type.hebdomadaire:
//                                            k=j+1; // On se place sur le suivant
//                                            nbDays=0;
//                                            while(k<nbCol && nbDays<DBClassificationConstantes.Periodicity.DAY_NUMBER_IN_WEEK-1){
//                                                ((MediaPlanItem)tab[i,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(nbLevels>1 && ((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(nbLevels>2 && ((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(nbLevels>3 && ((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                k++;
//                                                nbDays++;
//                                            }
//                                            break;
//                                        case (int)DBClassificationConstantes.Periodicity.type.bimensuel:
//                                            k=j+1; // On se place sur le suivant
//                                            nbDays=0;
//                                            while(k<nbCol && nbDays<DBClassificationConstantes.Periodicity.DAY_NUMBER_IN_BIWEEKLY-1){
//                                                ((MediaPlanItem)tab[i,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_LINE_INDEX,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(nbLevels>1 && ((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL1Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(nbLevels>2 && ((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL2Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                if(nbLevels>3 && ((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType!=FrameWorkConstantes.Results.MediaPlan.graphicItemType.present)((MediaPlanItem)tab[currentL3Index,k]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.extended;
//                                                k++;
//                                                nbDays++;
//                                            }
//                                            break;
//                                    }
//                                        break;
//                                        #endregion
//                                }
//                            }
//                            else{
//                                //if(((MediaPlanItem)tab[i,j]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.absent)
//                                //	((MediaPlanItem)tab[i,j]).GraphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.absent;
//                            }
//                        }
//                    }
//                }
//                #endregion
//            }
//            catch(System.Exception err){
//                throw(new WebException.MediaPlanRulesException("Erreur lors de l'extention de périodicité",err));
//            }

			
//            #region Ecriture de fin de tableau
//            nbCol=tab.GetLength(1);
//            nbline=tab.GetLength(0);
//            if(currentLineIndex+1<nbline)
//                tab[currentLineIndex+1,0]=new FrameWorkConstantes.MemoryArrayEnd();
//            #endregion

//            ds.Dispose();

//            #region Debug: Voir le tableau
////#if(DEBUG)
////			string HTML1="<html><table><tr>";
////			for(int z=0;z<=currentLineIndex;z++){
////				for(int r=0;r<nbCol;r++){
////					if(tab[z,r]!=null)HTML1+="<td>"+tab[z,r].ToString()+"</td>";
////					else HTML1+="<td>&nbsp;</td>";
////				}
////				HTML1+="</tr><tr>";
////			}
////			HTML1+="</tr></table></html>";
////#endif
//            #endregion

//            if(zoom)webSession.DetailPeriod=oldDisplayPeriod;
//            return(tab);
//        }

//        #region Méthodes Internal
		
//        #region Nombre de niveaux support
//        /// <summary>
//        /// Obtient le nombre d'élément dans le détail media
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Identifiant</returns>
//        internal static int GetNbLevel(WebSession webSession){
//            switch(webSession.PreformatedMediaDetail){
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle:
//                    return(1);
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSeller:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenter:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMedia:
//                    return(2);
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerMedia:
//                    return(3);
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleCategoryMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleInterestCenterMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMediaSellerMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerVehicleMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerInterestCenterMedia:
//                    return(4);
//                default:
//                    throw(new WebException.MediaPlanAnalysisRulesException("Le détail support demandé n'est pas valide"));
//            }
//        }

//        #endregion

//        #region Identifiant des éléments de la nomenclature produit
//        /// <summary>
//        /// Obtient l'identifiant du détail media de niveau 1
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="dr">Ligne de la table de résultat</param>
//        /// <returns>Identifiant</returns>
//        internal static Int64 GetL1Id(WebSession webSession,DataRow dr){
//            switch(webSession.PreformatedMediaDetail){
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSeller:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMediaSlogan:
//                    return(Int64.Parse(dr["id_vehicle"].ToString()));
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenter:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMediaSlogan:
//                    return(Int64.Parse(dr["id_media_seller"].ToString()));
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleCategoryMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleInterestCenterMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMediaSellerMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerVehicleMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerInterestCenterMedia:
//                    return(Int64.Parse(dr["id_slogan"].ToString()));
//                default:
//                    throw(new WebException.MediaPlanAnalysisRulesException("Le détail support demandé n'est pas valide pour le niveau 1"));
//            }
//        }

//        /// <summary>
//        /// Obtient l'identifiant du détail media de niveau 2
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="dr">Ligne de la table de résultat</param>
//        /// <returns>Identifiant</returns>
//        internal static Int64 GetL2Id(WebSession webSession,DataRow dr){
//            switch(webSession.PreformatedMediaDetail){
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleCategoryMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleInterestCenterMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMediaSellerMedia:
//                    return(Int64.Parse(dr["id_vehicle"].ToString()));
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan:
//                    return(Int64.Parse(dr["id_category"].ToString()));
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMedia:
//                    return(Int64.Parse(dr["id_media"].ToString()));
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenter:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMediaSlogan:
//                    return(Int64.Parse(dr["id_interest_center"].ToString()));
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSeller:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerVehicleMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerInterestCenterMedia:
//                    return(Int64.Parse(dr["id_media_seller"].ToString()));
//                default:
//                    throw(new WebException.MediaPlanAnalysisRulesException("Le détail support demandé n'est pas valide pour le niveau 2"));
//            }
//        }

//        /// <summary>
//        /// Obtient l'identifiant du détail media de niveau 3
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="dr">Ligne de la table de résultat</param>
//        /// <returns>Identifiant</returns>
//        internal static Int64 GetL3Id(WebSession webSession,DataRow dr){
//            switch(webSession.PreformatedMediaDetail){
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerVehicleMedia:
//                    return(Int64.Parse(dr["id_vehicle"].ToString()));
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleCategoryMedia:
//                    return(Int64.Parse(dr["id_category"].ToString()));
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerMedia:
//                    return(Int64.Parse(dr["id_media"].ToString()));
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleInterestCenterMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerInterestCenterMedia:
//                    return(Int64.Parse(dr["id_interest_center"].ToString()));
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMediaSellerMedia:
//                    return(Int64.Parse(dr["id_media_seller"].ToString()));
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMediaSlogan:
//                    return(Int64.Parse(dr["id_slogan"].ToString()));
//                default:
//                    throw(new WebException.MediaPlanAnalysisRulesException("Le détail support demandé n'est pas valide"));
//            }
//        }

//        /// <summary>
//        /// Obtient l'identifiant du détail media de niveau 4
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="dr">Ligne de la table de résultat</param>
//        /// <returns>Identifiant</returns>
//        internal static Int64 GetL4Id(WebSession webSession,DataRow dr){
//            switch(webSession.PreformatedMediaDetail){
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerVehicleMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleCategoryMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleInterestCenterMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerInterestCenterMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMediaSellerMedia:
//                    return(Int64.Parse(dr["id_media"].ToString()));
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMediaSlogan:
//                    return(Int64.Parse(dr["id_slogan"].ToString()));
//                default:
//                    throw(new WebException.MediaPlanAnalysisRulesException("Le détail support demandé n'est pas valide"));
//            }
//        }

//        #endregion

//        #region Libellés des éléments de la nomenclature media
//        /// <summary>
//        /// Obtient le libellé du détail media de niveau 1
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="dr">Ligne de la table de résultat</param>
//        /// <returns>Identifiant</returns>
//        internal static string GetL1Label(WebSession webSession,DataRow dr){
//            switch(webSession.PreformatedMediaDetail){
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSeller:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMediaSlogan:
//                    return(dr["vehicle"].ToString());
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenter:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMediaSlogan:
//                    return(dr["media_seller"].ToString());
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleCategoryMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleInterestCenterMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMediaSellerMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerVehicleMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerInterestCenterMedia:
//                    return(dr["slogan"].ToString());
//                default:
//                    throw(new WebException.MediaPlanAnalysisRulesException("Le détail support demandé n'est pas valide"));
//            }
//        }

//        /// <summary>
//        /// Obtient le libellé du détail media de niveau 2
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="dr">Ligne de la table de résultat</param>
//        /// <returns>Identifiant</returns>
//        internal static string GetL2Label(WebSession webSession,DataRow dr){
//            switch(webSession.PreformatedMediaDetail){
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleCategoryMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleInterestCenterMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMediaSellerMedia:
//                    return(dr["vehicle"].ToString());
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan:
//                    return(dr["category"].ToString());
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMedia:
//                    return(dr["media"].ToString());
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenter:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMediaSlogan:
//                    return(dr["interest_center"].ToString());
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSeller:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerVehicleMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerInterestCenterMedia:
//                    return(dr["media_seller"].ToString());
//                default:
//                    throw(new WebException.MediaPlanAnalysisRulesException("Le détail support demandé n'est pas valide"));
//            }
//        }

//        /// <summary>
//        /// Obtient le libellé du détail media de niveau 3
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="dr">Ligne de la table de résultat</param>
//        /// <returns>Identifiant</returns>
//        internal static string GetL3Label(WebSession webSession,DataRow dr){
//            switch(webSession.PreformatedMediaDetail){
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerVehicleMedia:
//                    return(dr["vehicle"].ToString());
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleCategoryMedia:
//                    return(dr["category"].ToString());
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerMedia:
//                    return(dr["media"].ToString());
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleInterestCenterMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerInterestCenterMedia:
//                    return(dr["interest_center"].ToString());
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMediaSellerMedia:
//                    return(dr["media_seller"].ToString());
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMediaSlogan:
//                    return(dr["slogan"].ToString());
//                default:
//                    throw(new WebException.MediaPlanAnalysisRulesException("Le détail support demandé n'est pas valide"));
//            }
//        }

//        /// <summary>
//        /// Obtient le libellé du détail media de niveau 4
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="dr">Ligne de la table de résultat</param>
//        /// <returns>Identifiant</returns>
//        internal static string GetL4Label(WebSession webSession,DataRow dr){
//            switch(webSession.PreformatedMediaDetail){
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerVehicleMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleCategoryMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleInterestCenterMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerInterestCenterMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMediaSellerMedia:
//                    return(dr["media"].ToString());
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMediaSlogan:
//                    return(dr["slogan"].ToString());
//                default:
//                    throw(new WebException.MediaPlanAnalysisRulesException("Le détail support demandé n'est pas valide"));
//            }
//        }

//        #endregion

//        #endregion
//    }
//}

