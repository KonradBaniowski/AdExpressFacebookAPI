//#region Informations
//// Auteur: G. Facon 
//// Date de création: 09/11/2004 
//// Date de modification:09/11/2004 
//// 4/01/2005 correction page division par 1000
//// 27/06/2005 K.Shehzad modifications for GAD in case of Gr. d'agence/agence/Advertiser
////	12/08/2005	A.Dadouch	Nom de fonctions
////	12/10/2005		D. V. Mussuma	Gestion des semaines comparatives	
////	28/10/2005	D. V. Mussuma	centralisation des unités	

//#endregion

//#region Using
//using System;
//using System.Data;
//using System.Collections;
//using System.Collections.Generic;
//using System.Windows.Forms;
//using TNS.AdExpress.Web.Core.Sessions;
//using TNS.AdExpress.Web.DataAccess.Results;
//using TNS.FrameWork.Date;
//using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
//using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
//using DynamicResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results.DynamicAnalysis;
//using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
//using WebConstantes=TNS.AdExpress.Constantes.Web;
//using CustomerConstantes=TNS.AdExpress.Constantes.Customer;
//using WebFunctions=TNS.AdExpress.Web.Functions;
//using WebExceptions=TNS.AdExpress.Web.Exceptions;
//using WebCommon=TNS.AdExpress.Web.Common;
//using WebCore=TNS.AdExpress.Web.Core;
//using ClassificationConstantes=TNS.AdExpress.Constantes.Classification;
//using ClassificationDB=TNS.AdExpress.DataAccess.Classification;
//using TNS.AdExpress.Domain.Web.Navigation;
//using FrameWorkDates=TNS.FrameWork.Date;
//using RulesConstantes=TNS.AdExpress.Web.Rules.Results;
//using TNS.AdExpress.Domain.Translation;
//using WebResultUI=TNS.FrameWork.WebResultUI;
//using TNS.FrameWork.WebResultUI;
//using TNS.AdExpress.Web.Core.Result;
//using TNS.AdExpress.Domain.Level;
//#endregion

//namespace TNS.AdExpress.Web.Rules.Results{
//    /// <summary>
//    /// Traitements métiers des analyses dynamiques
//    /// </summary>
//    public class DynamicRules{

//        #region Portefeuille
//        /// <summary>
//        /// Obtient le tableau de résultat du portefeuille d'une analyse dynamique
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="beginningPeriod">Date de début</param>
//        /// <param name="endPeriod">Date de fin</param>
//        /// <returns>Tableau de résultat</returns>
//        public static ResultTable GetPortofolio(WebSession webSession, string beginningPeriod, string endPeriod){

//            #region Variables
//            int nbUnivers=FrameWorkResultConstantes.DynamicAnalysis.NB_UNIVERSES_TEST;
//            #endregion

//            #region Tableaux d'index
//            WebCommon.Results.SelectionGroup[] groupMediaTotalIndex=new WebCommon.Results.SelectionGroup[6];
//            WebCommon.Results.SelectionSubGroup[] subGroupMediaTotalIndex = new WebCommon.Results.SelectionSubGroup[1000];
//            Hashtable mediaIndex=new Hashtable();
//            Hashtable mediaEvolIndex=new Hashtable();
//            string mediaListForLabelSearch="";
//            int maxIndex=0;
//            #endregion

//            #region Chargement du tableau
//            long nbLineInNewTable=0;
//            object[,] tabData = GetPreformatedTable(webSession, groupMediaTotalIndex, subGroupMediaTotalIndex, mediaIndex, mediaEvolIndex, ref maxIndex, ref nbLineInNewTable, beginningPeriod, endPeriod, ref mediaListForLabelSearch);
//            #endregion

//            #region Aucunes données
//            // Aucune données
//            if(tabData==null){
//                return null;
//            }
//            #endregion

//            #region Déclaration du tableau de résultat
//            long nbCol=tabData.GetLength(0);
//            long nbLineInFormatedTable=nbLineInNewTable;
//            object[,] tabResult=new object[nbCol,nbLineInFormatedTable];
//            #endregion

//            #region Traitement des données
//            // Pas de traitement
//            #endregion

//            return(GetResultTable(webSession,tabData,nbLineInFormatedTable,groupMediaTotalIndex,subGroupMediaTotalIndex,mediaIndex,mediaEvolIndex,mediaListForLabelSearch,nbUnivers));
//        }
//        #endregion

//        #region Fidèles
//        /// <summary>
//        /// Obtient le tableau de résultat des fidèles d'une analyse dynamique
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="beginningPeriod">Date de début</param>
//        /// <param name="endPeriod">Date de fin</param>
//        /// <returns>Tableau de résultat</returns>
//        internal static ResultTable GetLoyal(WebSession webSession, string beginningPeriod, string endPeriod){

//            #region Variables
//            int positionUnivers=1;
//            long currentLine;
//            long currentColumn;
//            long currentLineInTabResult;
//            int nbUnivers=FrameWorkResultConstantes.DynamicAnalysis.NB_UNIVERSES_TEST;
//            #endregion

//            #region Tableaux d'index
//            WebCommon.Results.SelectionGroup[] groupMediaTotalIndex=new WebCommon.Results.SelectionGroup[6];
//            WebCommon.Results.SelectionSubGroup[] subGroupMediaTotalIndex = new WebCommon.Results.SelectionSubGroup[1000];
//            Hashtable mediaIndex=new Hashtable();
//            Hashtable mediaEvolIndex=new Hashtable();
//            string mediaListForLabelSearch="";
//            int maxIndex=0;
//            //InitIndexAndValues(webSession,groupMediaTotalIndex,mediaIndex,mediaEvolIndex,ref mediaListForLabelSearch,ref maxIndex);
//            #endregion

//            #region Chargement du tableau
//            long nbLineInNewTable=0;
//            //object[,] tabData=GetPreformatedTable(webSession,groupMediaTotalIndex,mediaIndex,mediaEvolIndex,maxIndex,ref nbLineInNewTable,beginningPeriod,endPeriod);
//            object[,] tabData = GetPreformatedTable(webSession, groupMediaTotalIndex, subGroupMediaTotalIndex, mediaIndex, mediaEvolIndex, ref maxIndex, ref nbLineInNewTable, beginningPeriod, endPeriod, ref mediaListForLabelSearch);
//            #endregion

//            #region Aucunes données
//            // Aucune données
//            if(tabData==null){
//                return null;
//            }
//            #endregion

//            #region Déclaration du tableau de résultat
//            long nbCol=tabData.GetLength(0);
//            long nbLineInFormatedTable=nbLineInNewTable;
//            object[,] tabResult=new object[nbCol,nbLineInFormatedTable];
//            #endregion

		
//            #region Traitement des données
//            // On cherche les lignes qui ont tous les totaux non null(!=0)
//            bool allSubTotalNotNull=true;
//            currentLineInTabResult=-1;
//            for(currentLine=0;currentLine<nbLineInFormatedTable;currentLine++){
//                allSubTotalNotNull=true;
//                positionUnivers=1;
//                while(allSubTotalNotNull && positionUnivers<nbUnivers-1){
//                    if((double)tabData[groupMediaTotalIndex[positionUnivers].IndexInResultTable,currentLine]==0.0)allSubTotalNotNull=false;
//                    positionUnivers++;
//                }
//                if(allSubTotalNotNull){
//                    currentLineInTabResult++;
//                    for(currentColumn=0;currentColumn<nbCol;currentColumn++){
//                        tabResult[currentColumn,currentLineInTabResult]=tabData[currentColumn,currentLine];
//                    }
//                }
//            }
//            long nbLineInTabResult=currentLineInTabResult+1;
//            #endregion

//            return (GetResultTable(webSession, tabResult, nbLineInTabResult, groupMediaTotalIndex, subGroupMediaTotalIndex, mediaIndex, mediaEvolIndex, mediaListForLabelSearch, nbUnivers));
//        }
//        #endregion

//        #region Fidèles en baisse
//        /// <summary>
//        /// Obtient le tableau de résultat des fidèles en baisses d'une analyse dynamique
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="beginningPeriod">Date de début</param>
//        /// <param name="endPeriod">Date de fin</param>
//        /// <returns>Tableau de résultat</returns>
//        internal static ResultTable GetLoyalDecline(WebSession webSession, string beginningPeriod, string endPeriod){

//            #region Variables
//            int positionUnivers=1;
//            long currentLine;
//            long currentColumn;
//            long currentLineInTabResult;
//            int nbUnivers=FrameWorkResultConstantes.DynamicAnalysis.NB_UNIVERSES_TEST;
//            #endregion

//            #region Tableaux d'index
//            WebCommon.Results.SelectionGroup[] groupMediaTotalIndex=new WebCommon.Results.SelectionGroup[6];
//            WebCommon.Results.SelectionSubGroup[] subGroupMediaTotalIndex = new WebCommon.Results.SelectionSubGroup[1000];
//            Hashtable mediaIndex=new Hashtable();
//            Hashtable mediaEvolIndex=new Hashtable();
//            string mediaListForLabelSearch="";
//            int maxIndex=0;
//            //InitIndexAndValues(webSession,groupMediaTotalIndex,mediaIndex,mediaEvolIndex,ref mediaListForLabelSearch,ref maxIndex);
//            #endregion

//            #region Chargement du tableau
//            long nbLineInNewTable=0;
//            //object[,] tabData=GetPreformatedTable(webSession,groupMediaTotalIndex,mediaIndex,mediaEvolIndex,maxIndex,ref nbLineInNewTable,beginningPeriod,endPeriod);
//            object[,] tabData = GetPreformatedTable(webSession, groupMediaTotalIndex, subGroupMediaTotalIndex, mediaIndex, mediaEvolIndex, ref maxIndex, ref nbLineInNewTable, beginningPeriod, endPeriod, ref mediaListForLabelSearch);
//            #endregion

//            #region Aucunes données
//            // Aucune données
//            if(tabData==null){
//                return null;
//            }
//            #endregion

//            #region Déclaration du tableau de résultat
//            long nbCol=tabData.GetLength(0);
//            long nbLineInFormatedTable=nbLineInNewTable;
//            object[,] tabResult=new object[nbCol,nbLineInFormatedTable];
//            #endregion

//            #region Traitement des données
//            // On cherche les lignes qui ont tous les totaux non null(!=0)
//            bool allSubTotalNotNull=true;
//            currentLineInTabResult=-1;
//            for(currentLine=0;currentLine<nbLineInFormatedTable;currentLine++){
//                allSubTotalNotNull=true;
//                positionUnivers=1;
//                while(allSubTotalNotNull && positionUnivers<nbUnivers-1){
//                    if((double)tabData[groupMediaTotalIndex[positionUnivers].IndexInResultTable,currentLine]==0.0)allSubTotalNotNull=false;
//                    positionUnivers++;
//                }
//                if(allSubTotalNotNull && (double)tabData[groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION].IndexInResultTable,currentLine]<(double)tabData[groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N1_UNIVERSE_POSITION].IndexInResultTable,currentLine]){
//                    currentLineInTabResult++;
//                    for(currentColumn=0;currentColumn<nbCol;currentColumn++){
//                        tabResult[currentColumn,currentLineInTabResult]=tabData[currentColumn,currentLine];
//                    }
//                }
//            }
//            long nbLineInTabResult=currentLineInTabResult+1;
//            #endregion

//            return (GetResultTable(webSession, tabResult, nbLineInTabResult, groupMediaTotalIndex, subGroupMediaTotalIndex, mediaIndex, mediaEvolIndex, mediaListForLabelSearch, nbUnivers));
//        }

//        #endregion

//        #region Fidèles en développement
//        /// <summary>
//        /// Obtient le tableau de résultat des fidèles en hausse d'une analyse dynamique
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="beginningPeriod">Date de début</param>
//        /// <param name="endPeriod">Date de fin</param>
//        /// <returns>Tableau de résultat</returns>
//        internal static ResultTable GetLoyalRise(WebSession webSession, string beginningPeriod, string endPeriod){

//            #region Variables
//            int positionUnivers=1;
//            long currentLine;
//            long currentColumn;
//            long currentLineInTabResult;
//            int nbUnivers=FrameWorkResultConstantes.DynamicAnalysis.NB_UNIVERSES_TEST;
//            #endregion

//            #region Tableaux d'index
//            WebCommon.Results.SelectionGroup[] groupMediaTotalIndex=new WebCommon.Results.SelectionGroup[6];
//            WebCommon.Results.SelectionSubGroup[] subGroupMediaTotalIndex = new WebCommon.Results.SelectionSubGroup[1000];
//            Hashtable mediaIndex=new Hashtable();
//            Hashtable mediaEvolIndex=new Hashtable();
//            string mediaListForLabelSearch="";
//            int maxIndex=0;
//            //InitIndexAndValues(webSession,groupMediaTotalIndex,mediaIndex,mediaEvolIndex,ref mediaListForLabelSearch,ref maxIndex);
//            #endregion

//            #region Chargement du tableau
//            long nbLineInNewTable=0;
//            //object[,] tabData=GetPreformatedTable(webSession,groupMediaTotalIndex,mediaIndex,mediaEvolIndex,maxIndex,ref nbLineInNewTable,beginningPeriod,endPeriod);
//            object[,] tabData = GetPreformatedTable(webSession, groupMediaTotalIndex, subGroupMediaTotalIndex, mediaIndex, mediaEvolIndex, ref maxIndex, ref nbLineInNewTable, beginningPeriod, endPeriod, ref mediaListForLabelSearch);
//            #endregion

//            #region Aucunes données
//            // Aucune données
//            if(tabData==null){
//                return null;
//            }
//            #endregion

//            #region Déclaration du tableau de résultat
//            long nbCol=tabData.GetLength(0);
//            long nbLineInFormatedTable=nbLineInNewTable;
//            object[,] tabResult=new object[nbCol,nbLineInFormatedTable];
//            #endregion

//            #region Traitement des données
//            // On cherche les lignes qui ont tous les totaux non null(!=0)
//            bool allSubTotalNotNull=true;
//            currentLineInTabResult=-1;
//            for(currentLine=0;currentLine<nbLineInFormatedTable;currentLine++){
//                allSubTotalNotNull=true;
//                positionUnivers=1;
//                while(allSubTotalNotNull && positionUnivers<nbUnivers-1){
//                    if((double)tabData[groupMediaTotalIndex[positionUnivers].IndexInResultTable,currentLine]==0.0)allSubTotalNotNull=false;
//                    positionUnivers++;
//                }
//                if(allSubTotalNotNull && (double)tabData[groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION].IndexInResultTable,currentLine]>(double)tabData[groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N1_UNIVERSE_POSITION].IndexInResultTable,currentLine]){
//                    currentLineInTabResult++;
//                    for(currentColumn=0;currentColumn<nbCol;currentColumn++){
//                        tabResult[currentColumn,currentLineInTabResult]=tabData[currentColumn,currentLine];
//                    }
//                }
//            }
//            long nbLineInTabResult=currentLineInTabResult+1;
//            #endregion

//            return (GetResultTable(webSession, tabResult, nbLineInTabResult, groupMediaTotalIndex, subGroupMediaTotalIndex, mediaIndex, mediaEvolIndex, mediaListForLabelSearch, nbUnivers));
//        }
//        #endregion

//        #region Gagnés
//        /// <summary>
//        /// Obtient le tableau de résultat des gagnés d'une analyse dynamique
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="beginningPeriod">Date de début</param>
//        /// <param name="endPeriod">Date de fin</param>
//        /// <returns>Tableau de résultat</returns>
//        internal static ResultTable GetWon(WebSession webSession, string beginningPeriod, string endPeriod){

//            #region Variables
//            int positionUnivers=1;
//            long currentLine;
//            long currentColumn;
//            long currentLineInTabResult;
//            int nbUnivers=FrameWorkResultConstantes.DynamicAnalysis.NB_UNIVERSES_TEST;
//            #endregion

//            #region Tableaux d'index
//            WebCommon.Results.SelectionGroup[] groupMediaTotalIndex=new WebCommon.Results.SelectionGroup[6];
//            WebCommon.Results.SelectionSubGroup[] subGroupMediaTotalIndex = new WebCommon.Results.SelectionSubGroup[1000];
//            Hashtable mediaIndex=new Hashtable();
//            Hashtable mediaEvolIndex=new Hashtable();
//            string mediaListForLabelSearch="";
//            int maxIndex=0;
//            //InitIndexAndValues(webSession,groupMediaTotalIndex,mediaIndex,mediaEvolIndex,ref mediaListForLabelSearch,ref maxIndex);
//            #endregion

//            #region Chargement du tableau
//            long nbLineInNewTable=0;
//            //object[,] tabData=GetPreformatedTable(webSession,groupMediaTotalIndex,mediaIndex,mediaEvolIndex,maxIndex,ref nbLineInNewTable,beginningPeriod,endPeriod);
//            object[,] tabData = GetPreformatedTable(webSession, groupMediaTotalIndex, subGroupMediaTotalIndex, mediaIndex, mediaEvolIndex, ref maxIndex, ref nbLineInNewTable, beginningPeriod, endPeriod, ref mediaListForLabelSearch);
//            #endregion

//            #region Aucunes données
//            // Aucune données
//            if(tabData==null){
//                return null;
//            }
//            #endregion

//            #region Déclaration du tableau de résultat
//            long nbCol=tabData.GetLength(0);
//            long nbLineInFormatedTable=nbLineInNewTable;
//            object[,] tabResult=new object[nbCol,nbLineInFormatedTable];
//            #endregion

//            #region Traitement des données
//            bool allsubTotalNotNull=true;
//            currentLineInTabResult=-1;
//            for(currentLine=0;currentLine<nbLineInFormatedTable;currentLine++){
//                positionUnivers=1;
//                allsubTotalNotNull=true;
//                // On cherche les lignes qui on des unités différentes de 0(null) dans le premier sous total
//                if((double)tabData[groupMediaTotalIndex[1].IndexInResultTable,currentLine]!=0.0){
//                    positionUnivers++;
//                    while(allsubTotalNotNull && positionUnivers<nbUnivers-1){
//                        if((double)tabData[groupMediaTotalIndex[positionUnivers].IndexInResultTable,currentLine]!=0.0)allsubTotalNotNull=false;
//                        positionUnivers++;
//                    }
//                    // et tous les sous totaux de concurrent à 0(null)
//                    if(allsubTotalNotNull){
//                        currentLineInTabResult++;
//                        for(currentColumn=0;currentColumn<nbCol;currentColumn++){
//                            tabResult[currentColumn,currentLineInTabResult]=tabData[currentColumn,currentLine];
//                        }
//                    }
				
//                }
//            }
//            long nbLineInTabResult=currentLineInTabResult+1;
			
//            #endregion

//            return (GetResultTable(webSession, tabResult, nbLineInTabResult, groupMediaTotalIndex, subGroupMediaTotalIndex, mediaIndex, mediaEvolIndex, mediaListForLabelSearch, nbUnivers));
		
//        }
//        #endregion

//        #region Perdus
//        /// <summary>
//        /// Obtient le tableau de résultat des perdu d'une analyse dynamique
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="beginningPeriod">Date de début</param>
//        /// <param name="endPeriod">Date de fin</param>
//        /// <returns>Tableau de résultat</returns>
//        internal static ResultTable GetLost(WebSession webSession, string beginningPeriod, string endPeriod){

//            #region Variables
//            int positionUnivers=1;
//            long currentLine;
//            long currentColumn;
//            long currentLineInTabResult;
//            int nbUnivers=FrameWorkResultConstantes.DynamicAnalysis.NB_UNIVERSES_TEST;
//            #endregion

//            #region Tableaux d'index
//            WebCommon.Results.SelectionGroup[] groupMediaTotalIndex=new WebCommon.Results.SelectionGroup[6];
//            WebCommon.Results.SelectionSubGroup[] subGroupMediaTotalIndex = new WebCommon.Results.SelectionSubGroup[1000];
//            Hashtable mediaIndex=new Hashtable();
//            Hashtable mediaEvolIndex=new Hashtable();
//            string mediaListForLabelSearch="";
//            int maxIndex=0;
//            //InitIndexAndValues(webSession,groupMediaTotalIndex,mediaIndex,mediaEvolIndex,ref mediaListForLabelSearch,ref maxIndex);
//            #endregion

//            #region Chargement du tableau
//            long nbLineInNewTable=0;
//            //object[,] tabData=GetPreformatedTable(webSession,groupMediaTotalIndex,mediaIndex,mediaEvolIndex,maxIndex,ref nbLineInNewTable,beginningPeriod,endPeriod);
//            object[,] tabData = GetPreformatedTable(webSession, groupMediaTotalIndex, subGroupMediaTotalIndex, mediaIndex, mediaEvolIndex, ref maxIndex, ref nbLineInNewTable, beginningPeriod, endPeriod, ref mediaListForLabelSearch);
//            #endregion

//            #region Aucunes données
//            // Aucune données
//            if(tabData==null){
//                return null;
//            }
//            #endregion

//            #region Déclaration du tableau de résultat
//            long nbCol=tabData.GetLength(0);
//            long nbLineInFormatedTable=nbLineInNewTable;
//            object[,] tabResult=new object[nbCol,nbLineInFormatedTable];
//            #endregion

//            #region Traitement des données
//            bool subTotalNotNull=true;
//            currentLineInTabResult=-1;
//            for(currentLine=0;currentLine<nbLineInFormatedTable;currentLine++){
//                positionUnivers=1;
//                subTotalNotNull=false;
//                // On cherche les lignes qui on des unités à 0(null) dans le premier sous total
//                if((double)tabData[groupMediaTotalIndex[1].IndexInResultTable,currentLine]==0.0){
//                    positionUnivers++;
//                    while(!subTotalNotNull && positionUnivers<nbUnivers-1){
//                        if((double)tabData[groupMediaTotalIndex[positionUnivers].IndexInResultTable,currentLine]!=0.0)subTotalNotNull=true;
//                        positionUnivers++;
//                    }
//                    //au moins un sous total de concurrent différent à 0(null)
//                    if(subTotalNotNull){
//                        currentLineInTabResult++;
//                        for(currentColumn=0;currentColumn<nbCol;currentColumn++){
//                            tabResult[currentColumn,currentLineInTabResult]=tabData[currentColumn,currentLine];
//                        }
//                    }
				
//                }
//            }
//            long nbLineInTabResult=currentLineInTabResult+1;
			
//            #endregion

//            return (GetResultTable(webSession, tabResult, nbLineInTabResult, groupMediaTotalIndex, subGroupMediaTotalIndex, mediaIndex, mediaEvolIndex, mediaListForLabelSearch, nbUnivers));
		
//        }
//        #endregion

//        #region Synthèse
//        /// <summary>
//        /// Obtient une présentation synthétique, sous forme de tableau, du nombre d’éléments produits Fidèles
//        /// , Fidèles en baisse, Fidèles en développement, Gagnés, Perdus pour le module Analyse dynamique.
//        /// </summary>
//        /// <remarks>
//        /// Utilise les méthodes :
//        ///		- DynamicDataAccess.GetSynthesisData(webSession,vehicleName, beginningPeriod, endPeriod,beginningPeriodN1, endPeriodN1) : obtient les données à traiter.
//        ///		- GetProductActivity(object[,] tabResult,DataTable dt,int indexLineProduct,string expression,string filterN,string filterN1) : Obtient l'activité publicitaire d'un produit.	
//        /// </remarks>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="beginningPeriodDA">Date de début</param>
//        /// <param name="endPeriodDA">Date de fin</param>		
//        /// <returns>Tableau de résultats</returns>
//        internal static ResultTable GetSynthesis(WebSession webSession,string beginningPeriodDA, string endPeriodDA){
			
//            #region variables
//            long nbLine;			
//            ArrayList advertisers = null;
//            ArrayList products=null;
//            ArrayList brands = null;
//            ArrayList sectors=null;
//            ArrayList subsectors = null;
//            ArrayList groups=null;
//            ArrayList agencyGroups = null;
//            ArrayList agency=null;
//            Int64 advertiserLineIndex=0;
//            Int64 brandLineIndex=0;
//            Int64 productLineIndex=0;
//            Int64 sectorLineIndex=0;
//            Int64 subsectorLineIndex=0;
//            Int64 groupLineIndex=0;
//            Int64 agencyGroupLineIndex=0;
//            Int64 agencyLineIndex=0;

//            string expression="sum(unit)";
//            string filterN="";
//            string filterN1="";
//            DataTable dt = null;
			
//            #endregion
			
//            #region Sélection du vehicle
//            string vehicleSelection=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerConstantes.Right.type.vehicleAccess);
//            DBClassificationConstantes.Vehicles.names vehicleName=(DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
//            if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw(new WebExceptions.DynamicRulesException("La sélection de médias est incorrecte"));
//            #endregion

//            #region Calcul des périodes
//            WebCore.CustomerPeriod customerPeriod = webSession.CustomerPeriodSelected;

//            string beginningPeriodN1DA = customerPeriod.ComparativeStartDate;
//            string endPeriodN1DA = customerPeriod.ComparativeEndDate;
//            DateTime PeriodBeginningDate = new DateTime(int.Parse(customerPeriod.StartDate.Substring(0,4)),int.Parse(customerPeriod.StartDate.Substring(4,2)),int.Parse(customerPeriod.StartDate.Substring(6,2)));
//            DateTime PeriodEndDate = new DateTime(int.Parse(customerPeriod.EndDate.Substring(0, 4)), int.Parse(customerPeriod.EndDate.Substring(4, 2)), int.Parse(customerPeriod.EndDate.Substring(6, 2))); ;

//            DateTime PeriodBeginningDateN1DA = new DateTime(int.Parse(customerPeriod.ComparativeStartDate.Substring(0, 4)), int.Parse(customerPeriod.ComparativeStartDate.Substring(4, 2)), int.Parse(customerPeriod.ComparativeStartDate.Substring(6, 2)));
//            DateTime PeriodEndDateN1DA = new DateTime(int.Parse(customerPeriod.ComparativeEndDate.Substring(0, 4)), int.Parse(customerPeriod.ComparativeEndDate.Substring(4, 2)), int.Parse(customerPeriod.ComparativeEndDate.Substring(6, 2))); ;

//            #region Old code
//            //string beginningPeriodN1DA = WebFunctions.Dates.GetPreviousYearDate(beginningPeriodDA,webSession.DetailPeriod);
//            //string endPeriodN1DA = WebFunctions.Dates.GetPreviousYearDate(endPeriodDA,webSession.DetailPeriod);
//            //DateTime PeriodBeginningDate = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//            //DateTime PeriodEndDate = WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate,webSession.PeriodType);
//            #endregion

//            string PeriodDateN=PeriodBeginningDate.ToString("dd/MM/yyyy")+"-"+PeriodEndDate.Date.ToString("dd/MM/yyyy");

//            #region Old code
//            //string PeriodDateN1=PeriodBeginningDate.Date.AddYears(-1).ToString("dd/MM/yyyy")+"-"+PeriodEndDate.Date.AddYears(-1).ToString("dd/MM/yyyy");
//            #endregion

//            string PeriodDateN1 = PeriodBeginningDateN1DA.ToString("dd/MM/yyyy") + "-" + PeriodEndDateN1DA.ToString("dd/MM/yyyy");

//            // Gestion des semaines
//            #region Old code
//            //if(webSession.DetailPeriod==WebConstantes.CustomerSessions.Period.DisplayLevel.weekly){			
//            //    string beginningPeriodN1 = WebFunctions.Dates.GetPreviousYearDate(webSession.PeriodBeginningDate,WebConstantes.CustomerSessions.Period.DisplayLevel.weekly);
//            //    string endPeriodEndN1 = WebFunctions.Dates.GetPreviousYearDate(webSession.PeriodEndDate,WebConstantes.CustomerSessions.Period.DisplayLevel.weekly);
				
//            //    TNS.FrameWork.Date.AtomicPeriodWeek testWeek=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(endPeriodEndN1.Substring(0,4)),1);

//            //    TNS.FrameWork.Date.AtomicPeriodWeek testWeekBeginPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(beginningPeriodN1.Substring(0,4)),1);
				
//            //    TNS.FrameWork.Date.AtomicPeriodWeek endPeriod;
//            //    TNS.FrameWork.Date.AtomicPeriodWeek firstPeriod;
//            //    if(testWeek.NumberWeekInYear<int.Parse(endPeriodEndN1.Substring(4,2))){
//            //        endPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(endPeriodEndN1.Substring(0,4)),testWeek.NumberWeekInYear);
//            //    }
//            //    else{
//            //        endPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(endPeriodEndN1.Substring(0,4)),int.Parse(endPeriodEndN1.Substring(4,2)));
//            //    }

//            //    if(testWeekBeginPeriod.NumberWeekInYear<int.Parse(beginningPeriodN1.Substring(4,2))){
//            //        firstPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek((int.Parse(beginningPeriodN1.Substring(0,4))+1),01);
//            //    }
//            //    else{
//            //        firstPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(beginningPeriodN1.Substring(0,4)),int.Parse(beginningPeriodN1.Substring(4,2)));
					
//            //    }

//            //    if(int.Parse(endPeriodEndN1.Substring(4,2))==53 &&  int.Parse(beginningPeriodN1.Substring(4,2))==53){
//            //        PeriodDateN1=GestionWeb.GetWebWord(1530,webSession.SiteLanguage);
//            //    }else{				
//            //        PeriodDateN1=firstPeriod.FirstDay.Date.ToString("dd/MM/yyyy")+"-"+endPeriod.LastDay.Date.ToString("dd/MM/yyyy");				
//            //    }
//            //}
//            #endregion

//            #endregion

//            #region Aucune données
//            if (PeriodBeginningDate > DateTime.Now) {
//                return null;
//            }
//            #endregion

//            #region Chargement des données à partir de la base

//            try {
//                //dt = DynamicDataAccess.GetSynthesisData(webSession,vehicleName, beginningPeriodDA, endPeriodDA,beginningPeriodN1DA, endPeriodN1DA);
//                dt = DynamicDataAccess.GetGenericSynthesisData(webSession, vehicleName); 
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.DynamicRulesException("Impossible de charger les données de l'analyse dynamique",err));
//            }			
//            #endregion
	
//            //if(dt!=null && !dt.Equals(System.DBNull.Value) && dt.Rows.Count>0){

//            #region Identifiant du texte des unités
//            Int64 unitId=webSession.GetUnitLabelId();
//            CellUnitFactory cellUnitFactory=webSession.GetCellUnitFactory();
//            #endregion
				
				
//            #region Création des headers
//            nbLine=5;
//            if(webSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE)!=null)nbLine++;
//            if(webSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_MEDIA_AGENCY)!=null)nbLine+=2;

//            // Ajout de la colonne Produit
//            Headers headers=new Headers();
//            headers.Root.Add(new WebResultUI.Header(GestionWeb.GetWebWord(1164,webSession.SiteLanguage),DynamicResultConstantes.LEVEL_ID));

//            #region Fidèle
//            WebResultUI.HeaderGroup fidele=new WebResultUI.HeaderGroup(GestionWeb.GetWebWord(1241,webSession.SiteLanguage),DynamicResultConstantes.LOYAL_HEADER_ID);
//            fidele.Add(new WebResultUI.Header(true,GestionWeb.GetWebWord(1852,webSession.SiteLanguage),DynamicResultConstantes.ITEM_NUMBER_HEADER_ID));
//            WebResultUI.Header unitFidele=new WebResultUI.Header(true,GestionWeb.GetWebWord(unitId,webSession.SiteLanguage),DynamicResultConstantes.UNIT_HEADER_ID);
//            unitFidele.Add(new WebResultUI.Header(true,PeriodDateN,DynamicResultConstantes.N_UNIVERSE_POSITION));
//            unitFidele.Add(new WebResultUI.Header(true,PeriodDateN1,DynamicResultConstantes.N1_UNIVERSE_POSITION));
//            fidele.Add(unitFidele);
//            headers.Root.Add(fidele);
//            #endregion

//            #region Fidèle en baisse
//            WebResultUI.HeaderGroup fideleDecline=new WebResultUI.HeaderGroup(GestionWeb.GetWebWord(1242,webSession.SiteLanguage),DynamicResultConstantes.LOYAL_DECLINE_HEADER_ID);
//            fideleDecline.Add(new WebResultUI.Header(true,GestionWeb.GetWebWord(1852,webSession.SiteLanguage),DynamicResultConstantes.ITEM_NUMBER_HEADER_ID));
//            WebResultUI.Header unitFideleDecline=new WebResultUI.Header(true,GestionWeb.GetWebWord(unitId,webSession.SiteLanguage),DynamicResultConstantes.UNIT_HEADER_ID);
//            unitFideleDecline.Add(new WebResultUI.Header(true,PeriodDateN,DynamicResultConstantes.N_UNIVERSE_POSITION));
//            unitFideleDecline.Add(new WebResultUI.Header(true,PeriodDateN1,DynamicResultConstantes.N1_UNIVERSE_POSITION));
//            fideleDecline.Add(unitFideleDecline);
//            headers.Root.Add(fideleDecline);
//            #endregion

//            #region Fidèle en hausse
//            WebResultUI.HeaderGroup fideleRise=new WebResultUI.HeaderGroup(GestionWeb.GetWebWord(1243,webSession.SiteLanguage),DynamicResultConstantes.LOYAL_RISE_HEADER_ID);
//            fideleRise.Add(new WebResultUI.Header(true,GestionWeb.GetWebWord(1852,webSession.SiteLanguage),DynamicResultConstantes.ITEM_NUMBER_HEADER_ID));
//            WebResultUI.Header unitFideleRise=new WebResultUI.Header(true,GestionWeb.GetWebWord(unitId,webSession.SiteLanguage),DynamicResultConstantes.UNIT_HEADER_ID);
//            unitFideleRise.Add(new WebResultUI.Header(true,PeriodDateN,DynamicResultConstantes.N_UNIVERSE_POSITION));
//            unitFideleRise.Add(new WebResultUI.Header(true,PeriodDateN1,DynamicResultConstantes.N1_UNIVERSE_POSITION));
//            fideleRise.Add(unitFideleRise);
//            headers.Root.Add(fideleRise);
//            #endregion

//            #region Gagnés
//            WebResultUI.HeaderGroup won=new WebResultUI.HeaderGroup(GestionWeb.GetWebWord(1244,webSession.SiteLanguage),DynamicResultConstantes.WON_HEADER_ID);
//            won.Add(new WebResultUI.Header(true,GestionWeb.GetWebWord(1852,webSession.SiteLanguage),DynamicResultConstantes.ITEM_NUMBER_HEADER_ID));
//            WebResultUI.Header unitWon=new WebResultUI.Header(true,GestionWeb.GetWebWord(unitId,webSession.SiteLanguage),DynamicResultConstantes.UNIT_HEADER_ID);
//            unitWon.Add(new WebResultUI.Header(true,PeriodDateN,DynamicResultConstantes.N_UNIVERSE_POSITION));
//            unitWon.Add(new WebResultUI.Header(true,PeriodDateN1,DynamicResultConstantes.N1_UNIVERSE_POSITION));
//            won.Add(unitWon);
//            headers.Root.Add(won);
//            #endregion

//            #region Perdus
//            WebResultUI.HeaderGroup lost=new WebResultUI.HeaderGroup(GestionWeb.GetWebWord(1245,webSession.SiteLanguage),DynamicResultConstantes.LOST_HEADER_ID);
//            lost.Add(new WebResultUI.Header(true,GestionWeb.GetWebWord(1852,webSession.SiteLanguage),DynamicResultConstantes.ITEM_NUMBER_HEADER_ID));
//            WebResultUI.Header unitLost=new WebResultUI.Header(true,GestionWeb.GetWebWord(unitId,webSession.SiteLanguage),DynamicResultConstantes.UNIT_HEADER_ID);
//            unitLost.Add(new WebResultUI.Header(true,PeriodDateN,DynamicResultConstantes.N_UNIVERSE_POSITION));
//            unitLost.Add(new WebResultUI.Header(true,PeriodDateN1,DynamicResultConstantes.N1_UNIVERSE_POSITION));
//            lost.Add(unitLost);
//            headers.Root.Add(lost);
//            #endregion

//            #endregion

//            ResultTable resultTable=new ResultTable(nbLine,headers);
//            Int64 nbCol=resultTable.ColumnsNumber-2;

//            advertisers = new ArrayList();
//            products=new ArrayList();
//            brands = new ArrayList();
//            sectors=new ArrayList();
//            subsectors = new ArrayList();
//            groups=new ArrayList();
//            agencyGroups =new ArrayList();
//            agency=new ArrayList();

//            #region Initialisation des lignes
//            Int64 levelLabelColIndex=resultTable.GetHeadersIndexInResultTable(DynamicResultConstantes.LEVEL_ID.ToString());
//            advertiserLineIndex=resultTable.AddNewLine(LineType.level1);
//            resultTable[advertiserLineIndex,levelLabelColIndex]=new CellLabel(GestionWeb.GetWebWord(1146,webSession.SiteLanguage));
//            if(webSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE)!=null){
//                brandLineIndex=resultTable.AddNewLine(LineType.level1);
//                resultTable[brandLineIndex,levelLabelColIndex]=new CellLabel(GestionWeb.GetWebWord(1149,webSession.SiteLanguage));
//            }
//            productLineIndex=resultTable.AddNewLine(LineType.level1);
//            resultTable[productLineIndex,levelLabelColIndex]=new CellLabel(GestionWeb.GetWebWord(1164,webSession.SiteLanguage));
//            sectorLineIndex=resultTable.AddNewLine(LineType.level1);
//            resultTable[sectorLineIndex,levelLabelColIndex]=new CellLabel(GestionWeb.GetWebWord(1847,webSession.SiteLanguage));
//            subsectorLineIndex=resultTable.AddNewLine(LineType.level1);
//            resultTable[subsectorLineIndex,levelLabelColIndex]=new CellLabel(GestionWeb.GetWebWord(1848,webSession.SiteLanguage));
//            groupLineIndex=resultTable.AddNewLine(LineType.level1);
//            resultTable[groupLineIndex,levelLabelColIndex]=new CellLabel(GestionWeb.GetWebWord(1849,webSession.SiteLanguage));
//            // Groupe d'Agence && Agence
//            if(webSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_MEDIA_AGENCY)!=null){
//                agencyGroupLineIndex=resultTable.AddNewLine(LineType.level1);
//                resultTable[agencyGroupLineIndex,levelLabelColIndex]=new CellLabel(GestionWeb.GetWebWord(1850,webSession.SiteLanguage));
//                agencyLineIndex=resultTable.AddNewLine(LineType.level1);
//                resultTable[agencyLineIndex,levelLabelColIndex]=new CellLabel(GestionWeb.GetWebWord(1851,webSession.SiteLanguage));
//            }
//            #endregion

//            #region Initialisation des lignes
//            Int64 _loyalNumberColonneIndex=resultTable.GetHeadersIndexInResultTable(DynamicResultConstantes.LOYAL_HEADER_ID+"-"+DynamicResultConstantes.ITEM_NUMBER_HEADER_ID);
//            Int64 _loyalDeclineNumberColonneIndex=resultTable.GetHeadersIndexInResultTable(DynamicResultConstantes.LOYAL_DECLINE_HEADER_ID+"-"+DynamicResultConstantes.ITEM_NUMBER_HEADER_ID);
//            Int64 _loyalRiseNumberColonneIndex=resultTable.GetHeadersIndexInResultTable(DynamicResultConstantes.LOYAL_RISE_HEADER_ID+"-"+DynamicResultConstantes.ITEM_NUMBER_HEADER_ID);
//            Int64 _wonNumberColonneIndex=resultTable.GetHeadersIndexInResultTable(DynamicResultConstantes.WON_HEADER_ID+"-"+DynamicResultConstantes.ITEM_NUMBER_HEADER_ID);
//            Int64 _lostNumberColonneIndex=resultTable.GetHeadersIndexInResultTable(DynamicResultConstantes.LOST_HEADER_ID+"-"+DynamicResultConstantes.ITEM_NUMBER_HEADER_ID);

//            #region Initialisation des Nombres
//            for(int i=0;i<nbLine;i++){
//                resultTable[i,_loyalNumberColonneIndex]=new CellNumber(0.0);
//                resultTable[i,_loyalDeclineNumberColonneIndex]=new CellNumber(0.0);
//                resultTable[i,_loyalRiseNumberColonneIndex]=new CellNumber(0.0);
//                resultTable[i,_wonNumberColonneIndex]=new CellNumber(0.0);
//                resultTable[i,_lostNumberColonneIndex]=new CellNumber(0.0);
//            }
//            for(long i=0;i<nbLine;i++){
//                for(long j=_loyalNumberColonneIndex+1;j<_loyalDeclineNumberColonneIndex;j++){
//                    resultTable[i,j]=cellUnitFactory.Get(0.0);
//                }
//                for(long j=_loyalDeclineNumberColonneIndex+1;j<_loyalRiseNumberColonneIndex;j++){
//                    resultTable[i,j]=cellUnitFactory.Get(0.0);
//                }
//                for(long j=_loyalRiseNumberColonneIndex+1;j<_wonNumberColonneIndex;j++){
//                    resultTable[i,j]=cellUnitFactory.Get(0.0);
//                }
//                for(long j=_wonNumberColonneIndex+1;j<_lostNumberColonneIndex;j++){
//                    resultTable[i,j]=cellUnitFactory.Get(0.0);
//                }
//                for(long j=_lostNumberColonneIndex+1;j<=nbCol;j++){
//                    resultTable[i,j]=cellUnitFactory.Get(0.0);
//                }
//            }
//            #endregion

//            #endregion
////
////				#region Traitement des données
								
//                foreach(DataRow currentRow in dt.Rows){	

////					//Activité publicitaire Annonceurs
//                    if(currentRow["id_advertiser"]!=null && currentRow["id_advertiser"]!=System.DBNull.Value && !advertisers.Contains(currentRow["id_advertiser"].ToString())){			
//                        filterN="id_advertiser="+currentRow["id_advertiser"].ToString()+" AND ((date_num>="+beginningPeriodDA+" AND date_num<="+endPeriodDA +") or (date_num>="+beginningPeriodDA.Substring(0,6)+" AND date_num<="+endPeriodDA.Substring(0,6) +"))";
//                        filterN1 = "id_advertiser=" + currentRow["id_advertiser"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))";
//                        GetProductActivity(resultTable,dt,advertiserLineIndex,expression,filterN,filterN1);
//                        advertisers.Add(currentRow["id_advertiser"].ToString());
//                    }

//                    if(webSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE)!=null){//droits marques
//                        //Activité publicitaire marques
//                        if(currentRow["id_brand"]!=null && currentRow["id_brand"]!=System.DBNull.Value && !brands.Contains(currentRow["id_brand"].ToString())){
//                            filterN = "id_brand=" + currentRow["id_brand"].ToString() + " AND ((date_num>=" + beginningPeriodDA + " AND date_num<=" + endPeriodDA + ") or (date_num>=" + beginningPeriodDA.Substring(0, 6) + " AND date_num<=" + endPeriodDA.Substring(0, 6) + "))";
//                            filterN1 = "id_brand=" + currentRow["id_brand"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))"; 
//                            GetProductActivity(resultTable,dt,brandLineIndex,expression,filterN,filterN1);
//                            brands.Add(currentRow["id_brand"].ToString());
//                        }
//                    }

//                    //Activité publicitaire produits
//                    if(currentRow["id_product"]!=null && currentRow["id_product"]!=System.DBNull.Value && !products.Contains(currentRow["id_product"].ToString())){
//                        filterN = "id_product=" + currentRow["id_product"].ToString() + " AND ((date_num>=" + beginningPeriodDA + " AND date_num<=" + endPeriodDA + ") or (date_num>=" + beginningPeriodDA.Substring(0, 6) + " AND date_num<=" + endPeriodDA.Substring(0, 6) + "))";
//                        filterN1 = "id_product=" + currentRow["id_product"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))";  
//                        GetProductActivity(resultTable,dt,productLineIndex,expression,filterN,filterN1);
//                        products.Add(currentRow["id_product"].ToString());
//                    }
				
//                    //Activité publicitaire Famille
//                    if(currentRow["id_sector"]!=null && currentRow["id_sector"]!=System.DBNull.Value && !sectors.Contains(currentRow["id_sector"].ToString())){
//                        filterN = "id_sector=" + currentRow["id_sector"].ToString() + " AND ((date_num>=" + beginningPeriodDA + " AND date_num<=" + endPeriodDA + ") or (date_num>=" + beginningPeriodDA.Substring(0, 6) + " AND date_num<=" + endPeriodDA.Substring(0, 6) + "))";
//                        filterN1 = "id_sector=" + currentRow["id_sector"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))";  
//                        GetProductActivity(resultTable,dt,sectorLineIndex,expression,filterN,filterN1);
//                        sectors.Add(currentRow["id_sector"].ToString());
//                    }
//                    //Activité publicitaire Classe
//                    if(currentRow["id_subsector"]!=null && currentRow["id_subsector"]!=System.DBNull.Value && !subsectors.Contains(currentRow["id_subsector"].ToString())){
//                        filterN = "id_subsector=" + currentRow["id_subsector"].ToString() + " AND ((date_num>=" + beginningPeriodDA + " AND date_num<=" + endPeriodDA + ") or (date_num>=" + beginningPeriodDA.Substring(0, 6) + " AND date_num<=" + endPeriodDA.Substring(0, 6) + "))"; ;
//                        filterN1 = "id_subsector=" + currentRow["id_subsector"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))"; 
//                        GetProductActivity(resultTable,dt,subsectorLineIndex,expression,filterN,filterN1);
//                        subsectors.Add(currentRow["id_subsector"].ToString());
//                    }
//                    //Activité publicitaire Groupes
//                    if(currentRow["id_group_"]!=null && currentRow["id_group_"]!=System.DBNull.Value && !groups.Contains(currentRow["id_group_"].ToString())){
//                        filterN = "id_group_=" + currentRow["id_group_"].ToString() + " AND ((date_num>=" + beginningPeriodDA + " AND date_num<=" + endPeriodDA + ") or (date_num>=" + beginningPeriodDA.Substring(0, 6) + " AND date_num<=" + endPeriodDA.Substring(0, 6) + "))";
//                        filterN1 = "id_group_=" + currentRow["id_group_"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))";  
//                        GetProductActivity(resultTable,dt,groupLineIndex,expression,filterN,filterN1);
//                        groups.Add(currentRow["id_group_"].ToString());
//                    }
					
//                    if(webSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_MEDIA_AGENCY)!=null){//droits agences média					
//                        //activité publicitaire Groupes d'agences
//                        if(currentRow["ID_GROUP_ADVERTISING_AGENCY"]!=null && currentRow["ID_GROUP_ADVERTISING_AGENCY"]!=System.DBNull.Value &&  !agencyGroups.Contains(currentRow["ID_GROUP_ADVERTISING_AGENCY"].ToString())){
//                            filterN = "ID_GROUP_ADVERTISING_AGENCY=" + currentRow["ID_GROUP_ADVERTISING_AGENCY"].ToString() + " AND ((date_num>=" + beginningPeriodDA + " AND date_num<=" + endPeriodDA + ") or (date_num>=" + beginningPeriodDA.Substring(0, 6) + " AND date_num<=" + endPeriodDA.Substring(0, 6) + "))";
//                            filterN1 = "ID_GROUP_ADVERTISING_AGENCY=" + currentRow["ID_GROUP_ADVERTISING_AGENCY"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))"; 
//                            GetProductActivity(resultTable,dt,agencyGroupLineIndex,expression,filterN,filterN1);
//                            agencyGroups.Add(currentRow["ID_GROUP_ADVERTISING_AGENCY"].ToString());
//                        }
				
//                        //activité publicitaire agence
//                        if(currentRow["ID_ADVERTISING_AGENCY"]!=null && currentRow["ID_ADVERTISING_AGENCY"]!=System.DBNull.Value &&  !agency.Contains(currentRow["ID_ADVERTISING_AGENCY"].ToString())){
//                            filterN = "ID_ADVERTISING_AGENCY=" + currentRow["ID_ADVERTISING_AGENCY"].ToString() + " AND ((date_num>=" + beginningPeriodDA + " AND date_num<=" + endPeriodDA + ") or (date_num>=" + beginningPeriodDA.Substring(0, 6) + " AND date_num<=" + endPeriodDA.Substring(0, 6) + "))";
//                            filterN1 = "ID_ADVERTISING_AGENCY=" + currentRow["ID_ADVERTISING_AGENCY"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))";  
//                            GetProductActivity(resultTable,dt,agencyLineIndex,expression,filterN,filterN1);
//                            agency.Add(currentRow["ID_ADVERTISING_AGENCY"].ToString());
//                        }						
//                    }
//                }
//            return(resultTable);
//        }
//        #endregion

//        #region Méthodes internes

//        #region Initialisation des indexes
//        /// <summary>
//        /// Initialisation des tableaux d'indexes et valeurs sur les groupes de séléection et médias
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="groupMediaTotalIndex">(out) Tableau d'indexes des groupes de sélection</param>
//        /// <param name="subGroupMediaTotalIndex">Liste des index des sous groupes de sélection</param>
//        /// <param name="mediaIndex">(out Tableau d'indexes des médias</param>
//        /// <param name="mediaListForLabelSearch">(out)Liste des codes des médias</param>
//        /// <param name="maxIndex">(out)Index des colonnes maximum</param>
//        /// <param name="mediaEvolIndex">Tableaux d'index pour les évols</param>
//        /// <param name="dtMedia">Liste des média avec le niveau de détail colonne correspondant</param>
//        internal static void InitIndexAndValues(WebSession webSession, WebCommon.Results.SelectionGroup[] groupMediaTotalIndex, WebCommon.Results.SelectionSubGroup[] subGroupMediaTotalIndex, Hashtable mediaIndex, Hashtable mediaEvolIndex, ref string mediaListForLabelSearch, ref int maxIndex, DataTable dtMedia) {
			
//            #region Variables
//            string tmp="";
//            string[] mediaList;
//            int positionSubGroup = 2;
//            int subGroupCount = 0;
//            Hashtable mediaSubGroupNId = new Hashtable();
//            Hashtable mediaSubGroupN1Id = new Hashtable();
//            Hashtable mediaSubGroupEvolId = new Hashtable();
//            ArrayList columnDetailLevelList = new ArrayList();
//            #endregion

//            #region Initialisation des variables
//            maxIndex=FrameWorkResultConstantes.DynamicAnalysis.FIRST_MEDIA_INDEX;
//            #endregion

//            tmp=webSession.GetSelection((TreeNode) webSession.CompetitorUniversMedia[FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION],CustomerConstantes.Right.type.mediaAccess);
//            mediaList=tmp.Split(',');
            

//            // Chargement de la liste du niveau de détail colonne
//            for (int i = 1; i <= 3; i++) {

//                subGroupCount = 0;
//                mediaListForLabelSearch = "";
//                columnDetailLevelList = new ArrayList();

//                foreach (string media in mediaList) {
//                    foreach (DataRow row in dtMedia.Rows) {
//                        if (media.Equals(row["id_media"].ToString())) {
//                            if (!columnDetailLevelList.Contains(row["columnDetailLevel"].ToString())) {
//                                columnDetailLevelList.Add(row["columnDetailLevel"].ToString());
//                                subGroupMediaTotalIndex[positionSubGroup] = new WebCommon.Results.SelectionSubGroup(positionSubGroup);
//                                subGroupMediaTotalIndex[positionSubGroup].DataBaseId = int.Parse(row["columnDetailLevel"].ToString());
//                                switch (i) {
//                                    case 1: 
//                                        subGroupMediaTotalIndex[positionSubGroup].ParentId = FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION;
//                                        mediaSubGroupNId[Int64.Parse(media)] = positionSubGroup;
//                                        break;
//                                    case 2: 
//                                        subGroupMediaTotalIndex[positionSubGroup].ParentId = FrameWorkResultConstantes.DynamicAnalysis.N1_UNIVERSE_POSITION;
//                                        mediaSubGroupN1Id[Int64.Parse(media)] = positionSubGroup;
//                                        break;
//                                    case 3: 
//                                        subGroupMediaTotalIndex[positionSubGroup].ParentId = FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION;
//                                        mediaSubGroupEvolId[Int64.Parse(media)] = positionSubGroup;
//                                        break;
//                                }
//                                subGroupMediaTotalIndex[positionSubGroup].SetItemsNumber = 0;
//                                subGroupMediaTotalIndex[positionSubGroup].IndexInResultTable = 0;
//                                positionSubGroup++;
//                                subGroupCount++;
//                                mediaListForLabelSearch += row["columnDetailLevel"].ToString() + ",";
//                            }
//                            else {
//                                foreach (WebCommon.Results.SelectionSubGroup subGroup in subGroupMediaTotalIndex)
//                                    if (subGroup != null) {
//                                        if (subGroup.DataBaseId == int.Parse(row["columnDetailLevel"].ToString())) {
//                                            if (subGroup.Count == 0)
//                                                subGroup.SetItemsNumber = 2;
//                                            else
//                                                subGroup.SetItemsNumber = subGroup.Count + 1;
//                                            switch (i) {
//                                                case 1:
//                                                    mediaSubGroupNId[Int64.Parse(media)] = subGroup.Id;
//                                                    break;
//                                                case 2:
//                                                    mediaSubGroupN1Id[Int64.Parse(media)] = subGroup.Id;
//                                                    break;
//                                                case 3:
//                                                    mediaSubGroupEvolId[Int64.Parse(media)] = subGroup.Id;
//                                                    break;
//                                            }
//                                        }
//                                    }
//                            }
//                        }
//                    }
//                }
//            }

//            #region Année N
//            // Définition du groupe
//            groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION]=new WebCommon.Results.SelectionGroup(FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION);
//            // Le groupe contient plus de 1 éléments
//            if (subGroupCount > 1) {
//                groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION].IndexInResultTable=maxIndex;
//                groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION].SetItemsNumber = subGroupCount;
//                // Changement pourcentage
//                maxIndex++;
//                //nbSubTotal++;
//            }
//            else{
//                groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION].IndexInResultTable=maxIndex;
//                groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION].SetItemsNumber=0;
//            }
//            // Indexes des média (support)
//            foreach(string currentMedia in mediaList){
//                mediaIndex[Int64.Parse(currentMedia)] = new WebCommon.Results.GroupItemForTableResult(Int64.Parse(currentMedia), (int)mediaSubGroupNId[Int64.Parse(currentMedia)], maxIndex);
//            }
//            // Pour les sous Groupes
//            foreach (WebCommon.Results.SelectionSubGroup subGroup in subGroupMediaTotalIndex) {
//                if (subGroup != null) {
//                    if (subGroup.IndexInResultTable == 0 && subGroup.ParentId == FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION) {
//                        subGroup.IndexInResultTable = maxIndex;
//                        maxIndex++;
//                    }
//                }
//            }
//            #endregion

//            #region Année N -1
//            // Définition du groupe
//            groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N1_UNIVERSE_POSITION]=new WebCommon.Results.SelectionGroup(FrameWorkResultConstantes.DynamicAnalysis.N1_UNIVERSE_POSITION);
//            // Le groupe contient plus de 1 éléments
//            if (subGroupCount > 1) {
//                groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N1_UNIVERSE_POSITION].IndexInResultTable=maxIndex;
//                groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N1_UNIVERSE_POSITION].SetItemsNumber = subGroupCount;
//                maxIndex++;
//                //nbSubTotal++;
//            }
//            else{
//                groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N1_UNIVERSE_POSITION].IndexInResultTable=maxIndex;
//                groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N1_UNIVERSE_POSITION].SetItemsNumber=0;
//            }
//            // Indexes des média (support)
//            foreach(string currentMedia in mediaList){
//                mediaIndex[-1 * Int64.Parse(currentMedia)] = new WebCommon.Results.GroupItemForTableResult(-1 * Int64.Parse(currentMedia), (int)mediaSubGroupN1Id[Int64.Parse(currentMedia)], maxIndex);
//            }
//            // Pour les sous Groupes
//            foreach (WebCommon.Results.SelectionSubGroup subGroup in subGroupMediaTotalIndex) {
//                if (subGroup != null) {
//                    if (subGroup.IndexInResultTable == 0 && subGroup.ParentId == FrameWorkResultConstantes.DynamicAnalysis.N1_UNIVERSE_POSITION) {
//                        subGroup.IndexInResultTable = maxIndex;
//                        maxIndex++;
//                    }
//                }
//            }
//            #endregion

//            #region Evol
//            // Définition du groupe
//            groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION]=new WebCommon.Results.SelectionGroup(FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION);
//            // Le groupe contient plus de 1 éléments
//            if (subGroupCount > 1){
//                groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION].IndexInResultTable=maxIndex;
//                groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION].SetItemsNumber = subGroupCount;
//                maxIndex++;
//                //nbSubTotal++;
//            }
//            else{
//                groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION].IndexInResultTable=maxIndex;
//                groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION].SetItemsNumber=0;
//            }
//            // Indexes des média (support)
//            foreach(string currentMedia in mediaList){
//                mediaEvolIndex[Int64.Parse(currentMedia)] = new WebCommon.Results.GroupItemForTableResult(Int64.Parse(currentMedia), (int)mediaSubGroupEvolId[Int64.Parse(currentMedia)], maxIndex);
//            }
//            // Pour les sous Groupes
//            foreach (WebCommon.Results.SelectionSubGroup subGroup in subGroupMediaTotalIndex) {
//                if (subGroup != null) {
//                    if (subGroup.IndexInResultTable == 0 && subGroup.ParentId == FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION) {
//                        subGroup.IndexInResultTable = maxIndex;
//                        maxIndex++;
//                    }
//                }
//            }
//            #endregion



//            mediaListForLabelSearch=mediaListForLabelSearch.Substring(0,mediaListForLabelSearch.Length-1);
//        }

//        #endregion

//        #region Tableau Préformaté
//        /// <summary>
//        /// Obtient le tableau de résultat préformaté pour une alerte Concurrentielle
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="groupMediaTotalIndex">Liste des index des groupes de sélection</param>
//        /// <param name="subGroupMediaTotalIndex">Liste des index des sous groupes de sélection</param>
//        /// <param name="mediaIndex">Liste des index des médias</param>
//        /// <param name="nbCol">Nombre de colonnes du tableau de résultat</param>
//        /// <param name="nbLineInNewTable">(out) Nombre de ligne du tableau de résultat</param>
//        /// <param name="beginningPeriod">Date de début</param>
//        /// <param name="endPeriod">Date de fin</param>
//        /// <param name="mediaEvolIndex">Liste des index des evols médias</param>
//        /// <param name="mediaListForLabelSearch">(out)Liste des codes des médias</param>
//        /// <returns>Tableau de résultat</returns>
//        private static object[,] GetPreformatedTable(WebSession webSession, WebCommon.Results.SelectionGroup[] groupMediaTotalIndex, WebCommon.Results.SelectionSubGroup[] subGroupMediaTotalIndex, Hashtable mediaIndex, Hashtable mediaEvolIndex, ref int nbCol, ref long nbLineInNewTable, string beginningPeriod, string endPeriod, ref string mediaListForLabelSearch) {
			
//            #region Variables
//            Int64 idMedia=-1;
//            double unit;
//            long oldIdL1=-1;
//            long oldIdL2=-1;
//            long oldIdL3=-1;
//            Int64 currentLine=-1;
//            int k;
//            bool changeLine=false;
//            #endregion
			
//            #region Formattage des dates

//            Int64 beginningDate=Int64.Parse(beginningPeriod);
//            Int64 endPeriodDate=Int64.Parse(endPeriod);
//            DateTime startDate = new DateTime(int.Parse(beginningPeriod.Substring(0, 4)), int.Parse(beginningPeriod.Substring(4, 2)), int.Parse(beginningPeriod.Substring(6, 2)));
//            // Date de l'année de comparaison
//            #region Old version
////			string beginningPeriodN1=(Int64.Parse(beginningPeriod.Substring(0,4))-1).ToString()+beginningPeriod.Substring(4,2);
////			string endPeriodEndN1=(Int64.Parse(endPeriod.Substring(0,4))-1).ToString()+endPeriod.Substring(4,2);
//            #endregion

//            #region Old code
////          string beginningPeriodN1 = WebFunctions.Dates.GetPreviousYearDate(beginningPeriod,WebConstantes.CustomerSessions.Period.DisplayLevel.weekly);
////			string endPeriodEndN1 = WebFunctions.Dates.GetPreviousYearDate(endPeriod,WebConstantes.CustomerSessions.Period.DisplayLevel.weekly);
			
//            //string beginningPeriodN1 = WebFunctions.Dates.GetPreviousYearDate(beginningPeriod,webSession.DetailPeriod);
//            //string endPeriodEndN1 = WebFunctions.Dates.GetPreviousYearDate(endPeriod,webSession.DetailPeriod);
//            #endregion
            
//            #endregion

//            #region Aucune données
//            if (startDate>DateTime.Now) {
//                return null;
//            }
//            #endregion

//            #region Sélection du vehicle
//            string vehicleSelection=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerConstantes.Right.type.vehicleAccess);
//            DBClassificationConstantes.Vehicles.names vehicleName=(DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
//            if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw(new WebExceptions.DynamicRulesException("La sélection de médias est incorrecte"));
//            #endregion

//            #region Chargement des données à partir de la base	
//            DataSet ds;
//            DataSet dsMedia = null;
//            Module currentModuleDescription=ModulesList.GetModule(webSession.CurrentModule);
//            try{
//                //ds=DynamicDataAccess.GetData(webSession,vehicleName, beginningPeriod, endPeriod,beginningPeriodN1, endPeriodEndN1);
//                ds = DynamicDataAccess.GetGenericData(webSession, vehicleName);
//                dsMedia = DynamicDataAccess.GetMediaColumnDetailLevelList(webSession);
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.DynamicRulesException("Impossible de charger les données de l'analyse dynamique",err));
//            }
//            DataTable dt=ds.Tables[0];
//            DataTable dtMedia = dsMedia.Tables[0];
//            #endregion

//            #region Aucune données
//            if (dt.Rows.Count == 0) {
//                return null;
//            }
//            #endregion

//            #region Tableaux d'index
//            InitIndexAndValues(webSession, groupMediaTotalIndex, subGroupMediaTotalIndex, mediaIndex, mediaEvolIndex, ref mediaListForLabelSearch, ref nbCol, dtMedia);
//            #endregion
			
//            #region Déclaration du tableau de résultat
//            long nbline=dt.Rows.Count;
//            object[,] tabResult=new object[nbCol,dt.Rows.Count];
//            #endregion

//            #region Tableau de résultat

//            foreach(DataRow currentRow in dt.Rows){
//                idMedia=(Int64)currentRow["id_media"];
//                if(webSession.GenericProductDetailLevel.GetIdValue(currentRow,1)>0 && webSession.GenericProductDetailLevel.GetIdValue(currentRow,1)!=oldIdL1)changeLine=true;
//                if(!changeLine && webSession.GenericProductDetailLevel.GetIdValue(currentRow,2)>0 && webSession.GenericProductDetailLevel.GetIdValue(currentRow,2)!=oldIdL2)changeLine=true;
//                if(!changeLine && webSession.GenericProductDetailLevel.GetIdValue(currentRow,3)>0 && webSession.GenericProductDetailLevel.GetIdValue(currentRow,3)!=oldIdL3)changeLine=true;


//                #region On change de ligne
//                if(changeLine){

//                    currentLine++;
//                    // Ecriture de L1 ?
//                    if(webSession.GenericProductDetailLevel.GetIdValue(currentRow,1)>0){
//                        oldIdL1=webSession.GenericProductDetailLevel.GetIdValue(currentRow,1);
//                        tabResult[FrameWorkResultConstantes.DynamicAnalysis.IDL1_INDEX,currentLine]=oldIdL1;
//                        tabResult[FrameWorkResultConstantes.DynamicAnalysis.LABELL1_INDEX,currentLine]=webSession.GenericProductDetailLevel.GetLabelValue(currentRow,1);
//                    }
//                    // Ecriture de L2 ?
//                    if(webSession.GenericProductDetailLevel.GetIdValue(currentRow,2)>0){
//                        oldIdL2=webSession.GenericProductDetailLevel.GetIdValue(currentRow,2);
//                        tabResult[FrameWorkResultConstantes.DynamicAnalysis.IDL2_INDEX,currentLine]=oldIdL2;
//                        tabResult[FrameWorkResultConstantes.DynamicAnalysis.LABELL2_INDEX,currentLine]=webSession.GenericProductDetailLevel.GetLabelValue(currentRow,2);
//                    }
//                    // Ecriture de L3 ?
//                    if(webSession.GenericProductDetailLevel.GetIdValue(currentRow,3)>0){
//                        oldIdL3=webSession.GenericProductDetailLevel.GetIdValue(currentRow,3);
//                        tabResult[FrameWorkResultConstantes.DynamicAnalysis.IDL3_INDEX,currentLine]=oldIdL3;
//                        tabResult[FrameWorkResultConstantes.DynamicAnalysis.LABELL3_INDEX,currentLine]=webSession.GenericProductDetailLevel.GetLabelValue(currentRow,3);
//                    }
//                    // Totaux, sous Totaux et médias à 0
//                    for(k=FrameWorkResultConstantes.DynamicAnalysis.FIRST_MEDIA_INDEX;k<nbCol;k++){
//                        tabResult[k,currentLine]=(double) 0.0;
//                    }
					
//                    try{
//                        if(currentRow["id_address"]!=null)tabResult[FrameWorkResultConstantes.DynamicAnalysis.ADDRESS_COLUMN_INDEX,currentLine]=Int64.Parse(currentRow["id_address"].ToString());
//                    }
//                    catch(Exception){
					
//                    }
//                    changeLine=false;
//                }
//                #endregion

//                unit=double.Parse(currentRow["unit"].ToString());				

				
//                if(IsComparativeDateLine(Int64.Parse(currentRow["date_num"].ToString()),beginningDate,endPeriodDate)){
//                    idMedia=-1*idMedia;
//                }
//                // Ecriture du résultat du média
//                tabResult[(long)subGroupMediaTotalIndex[((WebCommon.Results.GroupItem)mediaIndex[idMedia]).GroupNumber].IndexInResultTable, currentLine] = (double)tabResult[(long)subGroupMediaTotalIndex[((WebCommon.Results.GroupItemForTableResult)mediaIndex[idMedia]).GroupNumber].IndexInResultTable, currentLine] + unit;

//                // Ecriture du résultat du sous total (somme)
//                if (groupMediaTotalIndex[(long)subGroupMediaTotalIndex[((WebCommon.Results.GroupItem)mediaIndex[idMedia]).GroupNumber].ParentId].Count > 1) {
//                    tabResult[(long)groupMediaTotalIndex[(long)subGroupMediaTotalIndex[((WebCommon.Results.GroupItem)mediaIndex[idMedia]).GroupNumber].ParentId].IndexInResultTable, currentLine] = (double)tabResult[(long)groupMediaTotalIndex[(long)subGroupMediaTotalIndex[((WebCommon.Results.GroupItem)mediaIndex[idMedia]).GroupNumber].ParentId].IndexInResultTable, currentLine] + unit;
//                }

//            }
		
//            #endregion

//            #region Debug: voir le tableau			
//            #if(DEBUG)
////						int i,j;
////						string HTML="<html><table><tr>";
////						for(i=0;i<=currentLine;i++){
////							for(j=0;j<nbCol;j++){
////								if(tabResult[j,i]!=null)HTML+="<td>"+tabResult[j,i].ToString()+"</td>";
////								else HTML+="<td>&nbsp;</td>";
////							}
////							HTML+="</tr><tr>";
////						}
////						HTML+="</tr></table></html>";
//            #endif
//            #endregion

//            nbLineInNewTable=currentLine+1;
//            return(tabResult);
//        }
//        #endregion

//        #region Formattage d'un tableau de résultat
//        /// <summary>
//        /// Création du ResutTable
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="tabData">Tableau contenant les données</param>
//        /// <param name="nbLineInTabData">Nombre de ligne du tableau</param>
//        /// <param name="groupMediaTotalIndex">Groupe de media</param>
//        /// <param name="subGroupMediaTotalIndex">Liste des sous groupes de sélection</param>
//        /// <param name="mediaIndex">Index des medias dans le tableau</param>
//        /// <param name="mediaEvolIndex">Index des evols des media</param>
//        /// <param name="mediaListForLabelSearch">Liste des identifiants des media</param>
//        /// <param name="nbUnivers">Nombre d'univers</param>
//        /// <returns>Résultat</returns>
//        private static WebResultUI.ResultTable GetResultTable(WebSession webSession, object[,] tabData, long nbLineInTabData, WebCommon.Results.SelectionGroup[] groupMediaTotalIndex, WebCommon.Results.SelectionSubGroup[] subGroupMediaTotalIndex, Hashtable mediaIndex, Hashtable mediaEvolIndex, string mediaListForLabelSearch, int nbUnivers) {
			
//            #region Variables
//            //Int64 currentMediaN1;
//            //long currentIdMedia;
//            //Int64 idMedia;
//            string[] mediaList;
//            Int64 oldIdL1=-1;
//            Int64 oldIdL2=-1;
//            Int64 oldIdL3=-1;
//            long currentLine;
//            long currentLineInTabResult;
//            long k;
//            ArrayList l2Indexes=new ArrayList();
//            ArrayList l1Indexes=new ArrayList();
//            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)webSession.GenericColumnDetailLevel.Levels[0];
//            #endregion 

//            #region Aucune données
//            if(nbLineInTabData == 0){
//                return null;
//            }
//            #endregion

//            #region Calcul des PDM ?
//            bool computePDM=false;
//            if(webSession.Percentage) computePDM=true;
//            #endregion

//            #region Affiche le Gad ?
//            bool showGad=false;
//            int advertiserColumnIndex=webSession.GenericProductDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser);
//            if(advertiserColumnIndex>0){
//                showGad=true;
//            }
//            #endregion

//            // Chargement des libellés de colonnes
//            ClassificationDB.MediaBranch.PartialMediaListDataAccess mediaLabelList = null;
//            ClassificationDB.MediaBranch.PartialCategoryListDataAccess categoryLabelList = null;
//            ClassificationDB.MediaBranch.PartialMediaSellerListDataAccess mediaSellerLabelList = null;
//            ClassificationDB.MediaBranch.PartialTitleListDataAccess titleLabelList = null;
//            ClassificationDB.MediaBranch.PartialInterestCenterListDataAccess interestCenterLabelList = null;

//            switch (columnDetailLevel.Id) {

//                case DetailLevelItemInformation.Levels.media:
//                    mediaLabelList = new ClassificationDB.MediaBranch.PartialMediaListDataAccess(mediaListForLabelSearch, webSession.SiteLanguage, webSession.Source);
//                    break;
//                case DetailLevelItemInformation.Levels.category:
//                    categoryLabelList = new ClassificationDB.MediaBranch.PartialCategoryListDataAccess(mediaListForLabelSearch,webSession.SiteLanguage,webSession.Source);
//                    break;
//                case DetailLevelItemInformation.Levels.mediaSeller:
//                    mediaSellerLabelList = new ClassificationDB.MediaBranch.PartialMediaSellerListDataAccess(mediaListForLabelSearch,webSession.SiteLanguage,webSession.Source);
//                    break;
//                case DetailLevelItemInformation.Levels.title:
//                    titleLabelList = new ClassificationDB.MediaBranch.PartialTitleListDataAccess(mediaListForLabelSearch,webSession.SiteLanguage,webSession.Source);
//                    break;
//                case DetailLevelItemInformation.Levels.interestCenter:
//                    interestCenterLabelList = new ClassificationDB.MediaBranch.PartialInterestCenterListDataAccess(mediaListForLabelSearch,webSession.SiteLanguage,webSession.Source);
//                    break;

//            }

//            // Nombre d'éléments dans un groupe
//            mediaList=mediaListForLabelSearch.Split(',');

//            #region Calcul des périodes
//            WebCore.CustomerPeriod customerPeriod = webSession.CustomerPeriodSelected;

//            #region Old code
//            //DateTime PeriodBeginningDate = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//            //DateTime PeriodEndDate = WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate,webSession.PeriodType);
//            //string PeriodDateN=PeriodBeginningDate.ToString("dd/MM/yyyy")+"-"+PeriodEndDate.Date.ToString("dd/MM/yyyy");
//            //string PeriodDateN1=PeriodBeginningDate.Date.AddYears(-1).ToString("dd/MM/yyyy")+"-"+PeriodEndDate.Date.AddYears(-1).ToString("dd/MM/yyyy");
//            #endregion

//            DateTime PeriodBeginningDate = new DateTime(int.Parse(customerPeriod.StartDate.Substring(0, 4)), int.Parse(customerPeriod.StartDate.Substring(4, 2)), int.Parse(customerPeriod.StartDate.Substring(6, 2)));
//            DateTime PeriodEndDate = new DateTime(int.Parse(customerPeriod.EndDate.Substring(0, 4)), int.Parse(customerPeriod.EndDate.Substring(4, 2)), int.Parse(customerPeriod.EndDate.Substring(6, 2))); ;

//            DateTime PeriodBeginningDateN1DA = new DateTime(int.Parse(customerPeriod.ComparativeStartDate.Substring(0, 4)), int.Parse(customerPeriod.ComparativeStartDate.Substring(4, 2)), int.Parse(customerPeriod.ComparativeStartDate.Substring(6, 2)));
//            DateTime PeriodEndDateN1DA = new DateTime(int.Parse(customerPeriod.ComparativeEndDate.Substring(0, 4)), int.Parse(customerPeriod.ComparativeEndDate.Substring(4, 2)), int.Parse(customerPeriod.ComparativeEndDate.Substring(6, 2))); ;

//            string PeriodDateN = PeriodBeginningDate.ToString("dd/MM/yyyy") + "-" + PeriodEndDate.Date.ToString("dd/MM/yyyy");

//            string PeriodDateN1 = PeriodBeginningDateN1DA.ToString("dd/MM/yyyy") + "-" + PeriodEndDateN1DA.ToString("dd/MM/yyyy");


//            // Gestion des semaines
//            #region Old code
//            //if(webSession.DetailPeriod==WebConstantes.CustomerSessions.Period.DisplayLevel.weekly){			
//            //    string beginningPeriodN1 = WebFunctions.Dates.GetPreviousYearDate(webSession.PeriodBeginningDate,WebConstantes.CustomerSessions.Period.DisplayLevel.weekly);
//            //    string endPeriodEndN1 = WebFunctions.Dates.GetPreviousYearDate(webSession.PeriodEndDate,WebConstantes.CustomerSessions.Period.DisplayLevel.weekly);
				
//            //    TNS.FrameWork.Date.AtomicPeriodWeek testWeek=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(endPeriodEndN1.Substring(0,4)),1);

//            //    TNS.FrameWork.Date.AtomicPeriodWeek testWeekBeginPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(beginningPeriodN1.Substring(0,4)),1);
				
//            //    TNS.FrameWork.Date.AtomicPeriodWeek endPeriod;
//            //    TNS.FrameWork.Date.AtomicPeriodWeek firstPeriod;
//            //    if(testWeek.NumberWeekInYear<int.Parse(endPeriodEndN1.Substring(4,2))){
//            //        endPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(endPeriodEndN1.Substring(0,4)),testWeek.NumberWeekInYear);
//            //    }
//            //    else{
//            //        endPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(endPeriodEndN1.Substring(0,4)),int.Parse(endPeriodEndN1.Substring(4,2)));
//            //    }

//            //    if(testWeekBeginPeriod.NumberWeekInYear<int.Parse(beginningPeriodN1.Substring(4,2))){
//            //        firstPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek((int.Parse(beginningPeriodN1.Substring(0,4))+1),01);
//            //    }
//            //    else{
//            //        firstPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(beginningPeriodN1.Substring(0,4)),int.Parse(beginningPeriodN1.Substring(4,2)));
					
//            //    }

//            //    if(int.Parse(endPeriodEndN1.Substring(4,2))==53 &&  int.Parse(beginningPeriodN1.Substring(4,2))==53){
//            //        PeriodDateN1=GestionWeb.GetWebWord(1530,webSession.SiteLanguage);
//            //    }else{				
//            //        PeriodDateN1=firstPeriod.FirstDay.Date.ToString("dd/MM/yyyy")+"-"+endPeriod.LastDay.Date.ToString("dd/MM/yyyy");				
//            //    }
//            //}
//            #endregion

//            #endregion

//            #region Headers
//            // Ajout de la colonne Produit
//            Headers headers=new Headers();
//            headers.Root.Add(new WebResultUI.Header(true,GestionWeb.GetWebWord(1164,webSession.SiteLanguage),DynamicResultConstantes.LEVEL_ID));
//            // Ajout plan media ?
//            bool showMediaSchedule=false;
//            if(webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser)||
//                webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.product)||
//                webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.brand)||
//                webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.holdingCompany)||
//                webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.sector)||
//                webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.subSector)||
//                webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.group)
//                ){
//                headers.Root.Add(new HeaderMediaSchedule(false,GestionWeb.GetWebWord(150,webSession.SiteLanguage),DynamicResultConstantes.MEDIA_SCHEDULE_ID));
//                showMediaSchedule=true;
//            }

//            mediaList=mediaListForLabelSearch.Split(',');
//            bool addSubTotal=false;
//            if(mediaList.Length>1)addSubTotal=true;
			
//            #region Ajout Année N
//            WebResultUI.HeaderGroup yearN=new WebResultUI.HeaderGroup(PeriodDateN,true,FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION);
//            // Ajout sous total
//            if(addSubTotal)yearN.AddSubTotal(true,GestionWeb.GetWebWord(1102,webSession.SiteLanguage),FrameWorkResultConstantes.DynamicAnalysis.SUBTOTAL_ID);
//            // Media
//            foreach (WebCommon.Results.SelectionSubGroup subGroup in subGroupMediaTotalIndex) {
//                if (subGroup != null) {
//                    if (subGroup.ParentId == FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION)
//                        switch (columnDetailLevel.Id) {

//                            case DetailLevelItemInformation.Levels.media:
//                                yearN.Add(new WebResultUI.Header(true, mediaLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
//                                break;
//                            case DetailLevelItemInformation.Levels.category:
//                                yearN.Add(new WebResultUI.Header(true, categoryLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
//                                break;
//                            case DetailLevelItemInformation.Levels.mediaSeller:
//                                yearN.Add(new WebResultUI.Header(true, mediaSellerLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
//                                break;
//                            case DetailLevelItemInformation.Levels.title:
//                                yearN.Add(new WebResultUI.Header(true, titleLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
//                                break;
//                            case DetailLevelItemInformation.Levels.interestCenter:
//                                yearN.Add(new WebResultUI.Header(true, interestCenterLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
//                                break;

//                        }
//                }
//            }
//            headers.Root.Add(yearN);
//            #endregion

//            #region  Ajout Année N-1
//            WebResultUI.HeaderGroup yearN1=new WebResultUI.HeaderGroup(PeriodDateN1,true,FrameWorkResultConstantes.DynamicAnalysis.N1_UNIVERSE_POSITION);
//            // Ajout sous total
//            if(addSubTotal)yearN1.AddSubTotal(true,GestionWeb.GetWebWord(1102,webSession.SiteLanguage),FrameWorkResultConstantes.DynamicAnalysis.SUBTOTAL_ID);
//            // Media
//            foreach (WebCommon.Results.SelectionSubGroup subGroup in subGroupMediaTotalIndex) {
//                if (subGroup != null) {
//                    if (subGroup.ParentId == FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION)
//                        switch (columnDetailLevel.Id) {

//                            case DetailLevelItemInformation.Levels.media:
//                                yearN1.Add(new WebResultUI.Header(true, mediaLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
//                                break;
//                            case DetailLevelItemInformation.Levels.category:
//                                yearN1.Add(new WebResultUI.Header(true, categoryLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
//                                break;
//                            case DetailLevelItemInformation.Levels.mediaSeller:
//                                yearN1.Add(new WebResultUI.Header(true, mediaSellerLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
//                                break;
//                            case DetailLevelItemInformation.Levels.title:
//                                yearN1.Add(new WebResultUI.Header(true, titleLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
//                                break;
//                            case DetailLevelItemInformation.Levels.interestCenter:
//                                yearN1.Add(new WebResultUI.Header(true, interestCenterLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
//                                break;

//                        }
//                }
//            }
//            headers.Root.Add(yearN1);
//            #endregion

//            #region  Ajout Année Evol
//            WebResultUI.HeaderGroup evol=new WebResultUI.HeaderGroup(GestionWeb.GetWebWord(1212,webSession.SiteLanguage),true,FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION);
//            // Ajout sous total
//            if(addSubTotal)evol.AddSubTotal(true,GestionWeb.GetWebWord(1102,webSession.SiteLanguage),FrameWorkResultConstantes.DynamicAnalysis.SUBTOTAL_ID);
//            // Media
//            foreach (WebCommon.Results.SelectionSubGroup subGroup in subGroupMediaTotalIndex) {
//                if (subGroup != null) {
//                    if (subGroup.ParentId == FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION)
//                        switch (columnDetailLevel.Id) {

//                            case DetailLevelItemInformation.Levels.media:
//                                evol.Add(new WebResultUI.Header(true, mediaLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
//                                break;
//                            case DetailLevelItemInformation.Levels.category:
//                                evol.Add(new WebResultUI.Header(true, categoryLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
//                                break;
//                            case DetailLevelItemInformation.Levels.mediaSeller:
//                                evol.Add(new WebResultUI.Header(true, mediaSellerLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
//                                break;
//                            case DetailLevelItemInformation.Levels.title:
//                                evol.Add(new WebResultUI.Header(true, titleLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
//                                break;
//                            case DetailLevelItemInformation.Levels.interestCenter:
//                                evol.Add(new WebResultUI.Header(true, interestCenterLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
//                                break;

//                        }
//                }
//            }
//            headers.Root.Add(evol);
//            //resultTable.NewHeaders=headers;
			

//            #endregion

//            #endregion

//            #region Sélection du vehicle
//            string vehicleSelection = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerConstantes.Right.type.vehicleAccess);
//            DBClassificationConstantes.Vehicles.names vehicleName = (DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
//            if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new WebExceptions.DynamicRulesException("La sélection de médias est incorrecte"));
//            #endregion


//            #region Déclaration du tableau de résultat
//            //long nbCol=(mediaList.Length*3)+1;
//            //if(mediaList.Length>1)nbCol+=3;
//            long nbLine=GetNbLineFromPreformatedTableToResultTable(tabData)+1;//FrameWorkResultConstantes.DynamicAnalysis.FIRST_LINE_RESULT_INDEX;
//            if (DBClassificationConstantes.Vehicles.names.press == vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress == vehicleName) nbLine = nbLine + 1;////Pour ligne nombre de parutions
//            WebResultUI.ResultTable resultTable=new WebResultUI.ResultTable(nbLine,headers);
//            long nbCol=resultTable.ColumnsNumber-2;
//            long NStartColIndex=-1;
//            if(addSubTotal)NStartColIndex=resultTable.GetHeadersIndexInResultTable(DynamicResultConstantes.N_UNIVERSE_POSITION+"-"+DynamicResultConstantes.SUBTOTAL_ID);
//            else NStartColIndex=resultTable.GetHeadersIndexInResultTable(DynamicResultConstantes.N_UNIVERSE_POSITION+"-"+mediaList[0]);
//            long N1StartColIndex=-1;
//            if(addSubTotal)N1StartColIndex=resultTable.GetHeadersIndexInResultTable(DynamicResultConstantes.N1_UNIVERSE_POSITION+"-"+DynamicResultConstantes.SUBTOTAL_ID);
//            else N1StartColIndex=resultTable.GetHeadersIndexInResultTable(DynamicResultConstantes.N1_UNIVERSE_POSITION+"-"+mediaList[0]);
//            long levelLabelColIndex=resultTable.GetHeadersIndexInResultTable(DynamicResultConstantes.LEVEL_ID.ToString());
//            long mediaScheduleColIndex=resultTable.GetHeadersIndexInResultTable(DynamicResultConstantes.MEDIA_SCHEDULE_ID.ToString());
//            long EvolStartColIndex;
//            if(mediaList.Length>1)
//                EvolStartColIndex=resultTable.GetHeadersIndexInResultTable(DynamicResultConstantes.EVOL_UNIVERSE_POSITION+"-"+DynamicResultConstantes.SUBTOTAL_ID);
//            else
//                EvolStartColIndex=resultTable.GetHeadersIndexInResultTable(DynamicResultConstantes.EVOL_UNIVERSE_POSITION.ToString())+1;
//            long startDataColIndex=levelLabelColIndex+1;
//            if(showMediaSchedule)startDataColIndex=mediaScheduleColIndex+1;
//            #endregion


//            #region Sélection de l'unité
//            CellUnitFactory cellUnitFactory=webSession.GetCellUnitFactory();
//            #endregion

//            #region Ligne du Total
//            currentLineInTabResult= resultTable.AddNewLine(TNS.FrameWork.WebResultUI.LineType.total);
//            //Libellé du total
//            resultTable[currentLineInTabResult,levelLabelColIndex]=new CellLevel(-1,GestionWeb.GetWebWord(805,webSession.SiteLanguage),0,currentLineInTabResult);
//            CellLevel currentCellLevel0=(CellLevel)resultTable[currentLineInTabResult,levelLabelColIndex];
//            if(showMediaSchedule)resultTable[currentLineInTabResult,mediaScheduleColIndex]= new CellMediaScheduleLink(currentCellLevel0,webSession);
//            // Unité
//            if(computePDM)resultTable[currentLineInTabResult,NStartColIndex]=new CellPDM(0.0,null);
//            else resultTable[currentLineInTabResult,NStartColIndex]=cellUnitFactory.Get(0.0);
//            for(k=NStartColIndex+1;k<N1StartColIndex;k++){
//                if(computePDM)resultTable[currentLineInTabResult,k]=new CellPDM(0.0,(CellUnit)resultTable[currentLineInTabResult,NStartColIndex]);
//                else resultTable[currentLineInTabResult,k]=cellUnitFactory.Get(0.0);
//            }
//            if(computePDM)resultTable[currentLineInTabResult,N1StartColIndex]=new CellPDM(0.0,null);
//            else resultTable[currentLineInTabResult,N1StartColIndex]=cellUnitFactory.Get(0.0);
//            for(k=N1StartColIndex+1;k<EvolStartColIndex;k++){
//                if(computePDM)resultTable[currentLineInTabResult,k]=new CellPDM(0.0,(CellUnit)resultTable[currentLineInTabResult,N1StartColIndex]);
//                else resultTable[currentLineInTabResult,k]=cellUnitFactory.Get(0.0);
//            }
//            #region Evol Total
//            if(mediaList.Length>1)resultTable[currentLineInTabResult,EvolStartColIndex]=new CellEvol(resultTable[currentLineInTabResult,NStartColIndex],resultTable[currentLineInTabResult,N1StartColIndex]);
//            foreach(string currentMedia in mediaList){
//                resultTable[currentLineInTabResult,resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}",FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION,currentMedia))]=new CellEvol(resultTable[currentLineInTabResult,resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}",FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION,currentMedia))],resultTable[currentLineInTabResult,resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}",FrameWorkResultConstantes.DynamicAnalysis.N1_UNIVERSE_POSITION,currentMedia))]);
//            }			
//            #endregion

//            #endregion

//            #region Ligne nombre de parutions

		
//            if (DBClassificationConstantes.Vehicles.names.press == vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress == vehicleName) {
//                //Nombre de parution par média
//                Dictionary<string, double> nbParutionByMedia = GetNbParutionsByMedia(webSession);

//                currentLineInTabResult = resultTable.AddNewLine(TNS.FrameWork.WebResultUI.LineType.nbParution);
//                //Libellé du Nombre parutions
//                resultTable[currentLineInTabResult, levelLabelColIndex] = new CellLevel(-1, GestionWeb.GetWebWord(2460, webSession.SiteLanguage), 0, currentLineInTabResult);
//                CellLevel currentCellParution = (CellLevel)resultTable[currentLineInTabResult, levelLabelColIndex];
//                if (showMediaSchedule) resultTable[currentLineInTabResult, mediaScheduleColIndex] = new CellMediaScheduleLink(currentCellParution, webSession);
//                resultTable[currentLineInTabResult, NStartColIndex] = new CellNumber(0.0);
//                for (k = NStartColIndex + 1; k < N1StartColIndex; k++) {
//                    resultTable[currentLineInTabResult, k] = new CellNumber(0.0);
//                }
//                resultTable[currentLineInTabResult, N1StartColIndex] = new CellNumber(0.0);
//                for (k = N1StartColIndex + 1; k < EvolStartColIndex; k++) {
//                    resultTable[currentLineInTabResult, k] = new CellNumber(0.0);
//                }
//                #region Evol Nb parutions
//                if (mediaList.Length > 1) resultTable[currentLineInTabResult, EvolStartColIndex] = new CellEvol(resultTable[currentLineInTabResult, NStartColIndex], resultTable[currentLineInTabResult, N1StartColIndex]);
//                foreach (string currentMedia in mediaList) {
//                    resultTable[currentLineInTabResult, resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}", FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION, currentMedia))] = new CellEvol(resultTable[currentLineInTabResult, resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}", FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION, currentMedia))], resultTable[currentLineInTabResult, resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}", FrameWorkResultConstantes.DynamicAnalysis.N1_UNIVERSE_POSITION, currentMedia))]);
//                }
//                #endregion

//                //Insertion du nombre de parution pour period N et N-1			
//                if (nbParutionByMedia != null && nbParutionByMedia.Count > 0) {
//                    foreach (KeyValuePair<string, double> kpv in nbParutionByMedia) {
//                        if (resultTable.HeadersIndexInResultTable.ContainsKey(kpv.Key)) {
//                            TNS.FrameWork.WebResultUI.Header header = (TNS.FrameWork.WebResultUI.Header)resultTable.HeadersIndexInResultTable[kpv.Key];
//                            resultTable[currentLineInTabResult, header.IndexInResultTable] = new CellNumber(nbParutionByMedia[kpv.Key]);
//                        }
//                    }
//                }
//            }
//            #endregion

//            #region Tableau de résultat
//            oldIdL1=-1;
//            oldIdL2=-1;
//            oldIdL3=-1;
//            AdExpressCellLevel currentCellLevel1=null;
//            AdExpressCellLevel currentCellLevel2=null;
//            AdExpressCellLevel currentCellLevel3=null;
//            long currentL1Index=-1;
//            long currentL2Index=-1;
//            long currentL3Index=-1;
//            long nbColInTabData=tabData.GetLength(0);
//            currentLineInTabResult=FrameWorkResultConstantes.DynamicAnalysis.FIRST_LINE_RESULT_INDEX-1;
//            for(currentLine=0;currentLine<nbLineInTabData;currentLine++){

//                #region On change de niveau L1
//                if(tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL1_INDEX,currentLine]!=null && (Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL1_INDEX,currentLine]!=oldIdL1){
//                    currentLineInTabResult=resultTable.AddNewLine(LineType.level1);
					

//                    #region Totaux et sous Totaux à 0 et media
//                    // Unité
//                    // Unité
//                    if(computePDM)resultTable[currentLineInTabResult,NStartColIndex]=new CellPDM(0.0,null);
//                    else resultTable[currentLineInTabResult,NStartColIndex]=cellUnitFactory.Get(0.0);
//                    for(k=NStartColIndex+1;k<N1StartColIndex;k++){
//                        if(computePDM)resultTable[currentLineInTabResult,k]=new CellPDM(0.0,(CellUnit)resultTable[currentLineInTabResult,NStartColIndex]);
//                        else resultTable[currentLineInTabResult,k]=cellUnitFactory.Get(0.0);
//                    }
//                    if(computePDM)resultTable[currentLineInTabResult,N1StartColIndex]=new CellPDM(0.0,null);
//                    else resultTable[currentLineInTabResult,N1StartColIndex]=cellUnitFactory.Get(0.0);
//                    for(k=N1StartColIndex+1;k<EvolStartColIndex;k++){
//                        if(computePDM)resultTable[currentLineInTabResult,k]=new CellPDM(0.0,(CellUnit)resultTable[currentLineInTabResult,N1StartColIndex]);
//                        else resultTable[currentLineInTabResult,k]=cellUnitFactory.Get(0.0);
//                    }
//                    #endregion
	

//                    #region Evol L1
//                    if(mediaList.Length>1)resultTable[currentLineInTabResult,EvolStartColIndex]=new CellEvol(resultTable[currentLineInTabResult,NStartColIndex],resultTable[currentLineInTabResult,N1StartColIndex]);
//                    foreach(string currentMedia in mediaList){
//                        resultTable[currentLineInTabResult,resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}",FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION,currentMedia))]=new CellEvol(resultTable[currentLineInTabResult,resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}",FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION,currentMedia))],resultTable[currentLineInTabResult,resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}",FrameWorkResultConstantes.DynamicAnalysis.N1_UNIVERSE_POSITION,currentMedia))]);
//                    }
//                    #endregion

//                    oldIdL1=(Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL1_INDEX,currentLine];
//                    resultTable[currentLineInTabResult,levelLabelColIndex]= new AdExpressCellLevel((Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL1_INDEX,currentLine],(string)tabData[FrameWorkResultConstantes.DynamicAnalysis.LABELL1_INDEX,currentLine],currentCellLevel0,1,currentLineInTabResult,webSession);		
//                    currentCellLevel1=(AdExpressCellLevel)resultTable[currentLineInTabResult,levelLabelColIndex];
//                    if(showMediaSchedule)resultTable[currentLineInTabResult,mediaScheduleColIndex]= new CellMediaScheduleLink(currentCellLevel1,webSession);		
//                    currentL1Index=currentLineInTabResult;
//                    oldIdL2=oldIdL3=-1;

//                    #region GAD
//                    if(showGad && webSession.GenericProductDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser)==1){
//                        if(tabData[FrameWorkResultConstantes.DynamicAnalysis.ADDRESS_COLUMN_INDEX,currentLine]!=null){
//                            ((CellLevel)resultTable[currentLineInTabResult,levelLabelColIndex]).AddressId=(Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.ADDRESS_COLUMN_INDEX,currentLine];
//                        }
//                    }
//                    #endregion
//                }
//                #endregion

//                #region On change de niveau L2
//                if(tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL2_INDEX,currentLine]!=null && (Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL2_INDEX,currentLine]!=oldIdL2){
//                    currentLineInTabResult=resultTable.AddNewLine(LineType.level2);

//                    #region Totaux et sous Totaux à 0 et media
//                    // Unité
//                    // Unité
//                    if(computePDM)resultTable[currentLineInTabResult,NStartColIndex]=new CellPDM(0.0,null);
//                    else resultTable[currentLineInTabResult,NStartColIndex]=cellUnitFactory.Get(0.0);
//                    for(k=NStartColIndex+1;k<N1StartColIndex;k++){
//                        if(computePDM)resultTable[currentLineInTabResult,k]=new CellPDM(0.0,(CellUnit)resultTable[currentLineInTabResult,NStartColIndex]);
//                        else resultTable[currentLineInTabResult,k]=cellUnitFactory.Get(0.0);
//                    }
//                    if(computePDM)resultTable[currentLineInTabResult,N1StartColIndex]=new CellPDM(0.0,null);
//                    else resultTable[currentLineInTabResult,N1StartColIndex]=cellUnitFactory.Get(0.0);
//                    for(k=N1StartColIndex+1;k<EvolStartColIndex;k++){
//                        if(computePDM)resultTable[currentLineInTabResult,k]=new CellPDM(0.0,(CellUnit)resultTable[currentLineInTabResult,N1StartColIndex]);
//                        else resultTable[currentLineInTabResult,k]=cellUnitFactory.Get(0.0);
//                    }
//                    // Evolution
//                    //resultTable[currentLineInTabResult,EvolStartColIndex]=new CellEvol(0.0);
//                    #endregion

//                    #region Evol L2
//                    if(mediaList.Length>1)resultTable[currentLineInTabResult,EvolStartColIndex]=new CellEvol(resultTable[currentLineInTabResult,NStartColIndex],resultTable[currentLineInTabResult,N1StartColIndex]);
//                    foreach(string currentMedia in mediaList){
//                        resultTable[currentLineInTabResult,resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}",FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION,currentMedia))]=new CellEvol(resultTable[currentLineInTabResult,resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}",FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION,currentMedia))],resultTable[currentLineInTabResult,resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}",FrameWorkResultConstantes.DynamicAnalysis.N1_UNIVERSE_POSITION,currentMedia))]);
//                    }			
//                    #endregion

//                    oldIdL2=(Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL2_INDEX,currentLine];
//                    resultTable[currentLineInTabResult,levelLabelColIndex]= new AdExpressCellLevel((Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL2_INDEX,currentLine],(string)tabData[FrameWorkResultConstantes.DynamicAnalysis.LABELL2_INDEX,currentLine],currentCellLevel1,2,currentLineInTabResult,webSession);		
//                    currentCellLevel2=(AdExpressCellLevel)resultTable[currentLineInTabResult,levelLabelColIndex];
//                    if(showMediaSchedule)resultTable[currentLineInTabResult,mediaScheduleColIndex]= new CellMediaScheduleLink(currentCellLevel2,webSession);
//                    currentL2Index=currentLineInTabResult;
//                    oldIdL3=-1;

//                    #region GAD
//                    if(showGad && webSession.GenericProductDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser)==2){
//                        if(tabData[FrameWorkResultConstantes.DynamicAnalysis.ADDRESS_COLUMN_INDEX,currentLine]!=null){
//                            ((CellLevel)resultTable[currentLineInTabResult,levelLabelColIndex]).AddressId=(Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.ADDRESS_COLUMN_INDEX,currentLine];
//                        }
//                    }
//                    #endregion
//                }
//                #endregion

//                #region On change de niveau L3
//                if(tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL3_INDEX,currentLine]!=null && (Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL3_INDEX,currentLine]!=oldIdL3){
//                    currentLineInTabResult=resultTable.AddNewLine(LineType.level3);				

//                    #region Totaux et sous Totaux à 0 et media
//                    // Unité
//                    if(computePDM)resultTable[currentLineInTabResult,NStartColIndex]=new CellPDM(0.0,null);
//                    else resultTable[currentLineInTabResult,NStartColIndex]=cellUnitFactory.Get(0.0);
//                    for(k=NStartColIndex+1;k<N1StartColIndex;k++){
//                        if(computePDM)resultTable[currentLineInTabResult,k]=new CellPDM(0.0,(CellUnit)resultTable[currentLineInTabResult,NStartColIndex]);
//                        else resultTable[currentLineInTabResult,k]=cellUnitFactory.Get(0.0);
//                    }
//                    if(computePDM)resultTable[currentLineInTabResult,N1StartColIndex]=new CellPDM(0.0,null);
//                    else resultTable[currentLineInTabResult,N1StartColIndex]=cellUnitFactory.Get(0.0);
//                    for(k=N1StartColIndex+1;k<EvolStartColIndex;k++){
//                        if(computePDM)resultTable[currentLineInTabResult,k]=new CellPDM(0.0,(CellUnit)resultTable[currentLineInTabResult,N1StartColIndex]);
//                        else resultTable[currentLineInTabResult,k]=cellUnitFactory.Get(0.0);
//                    }
//                    // Evolution
////					resultTable[currentLineInTabResult,EvolStartColIndex]=new CellEvol(0.0);
//                    #endregion

//                    #region Evol L3
//                    if(mediaList.Length>1)resultTable[currentLineInTabResult,EvolStartColIndex]=new CellEvol(resultTable[currentLineInTabResult,NStartColIndex],resultTable[currentLineInTabResult,N1StartColIndex]);
//                    foreach(string currentMedia in mediaList){
//                        resultTable[currentLineInTabResult,resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}",FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION,currentMedia))]=new CellEvol(resultTable[currentLineInTabResult,resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}",FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION,currentMedia))],resultTable[currentLineInTabResult,resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}",FrameWorkResultConstantes.DynamicAnalysis.N1_UNIVERSE_POSITION,currentMedia))]);
//                    }			
//                    #endregion

//                    oldIdL3=(Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL3_INDEX,currentLine];
//                    resultTable[currentLineInTabResult,levelLabelColIndex]= new AdExpressCellLevel((Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL3_INDEX,currentLine],(string)tabData[FrameWorkResultConstantes.DynamicAnalysis.LABELL3_INDEX,currentLine],currentCellLevel2,3,currentLineInTabResult,webSession);		
//                    currentCellLevel3=(AdExpressCellLevel)resultTable[currentLineInTabResult,levelLabelColIndex];
//                    if(showMediaSchedule)resultTable[currentLineInTabResult,mediaScheduleColIndex]= new CellMediaScheduleLink(currentCellLevel3,webSession);
//                    currentL3Index=currentLineInTabResult;

//                    #region GAD
//                    if(showGad && webSession.GenericProductDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser)==3){
//                        if(tabData[FrameWorkResultConstantes.DynamicAnalysis.ADDRESS_COLUMN_INDEX,currentLine]!=null){
//                            ((CellLevel)resultTable[currentLineInTabResult,levelLabelColIndex]).AddressId=(Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.ADDRESS_COLUMN_INDEX,currentLine];
//                        }
//                    }
//                    #endregion
//                }
//                #endregion
			
//                // On copy la ligne et on l'ajoute aux totaux
//                for(k=FrameWorkResultConstantes.DynamicAnalysis.FIRST_MEDIA_INDEX;k<nbColInTabData;k++){
//                    resultTable.AffectValueAndAddToHierarchy(levelLabelColIndex,currentLineInTabResult,startDataColIndex+k-(long.Parse((FrameWorkResultConstantes.DynamicAnalysis.FIRST_MEDIA_INDEX).ToString())),(double)tabData[k,currentLine]);

//                }

//            }
//            #endregion

//            return(resultTable);
//        }
		
//        /// <summary>
//        /// Formattage d'un tableau de résultat à partir d'un tableau de données
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="tabData">Table de données</param>
//        /// <param name="nbLineInTabData">Nombre de ligne dans le tableau</param>
//        /// <param name="groupMediaTotalIndex">Liste des groupes de sélection</param>
//        /// <param name="mediaIndex">Liste des Média</param>
//        /// <param name="mediaListForLabelSearch">Liste des médias</param>
//        /// <param name="nbUnivers">Nombre d'univers</param>
//        /// <param name="mediaEvolIndex">Index pour les évol</param>
//        /// <returns>Tableau de résultat</returns>
//        private static object[,] GetResultTableOld(WebSession webSession,object[,] tabData,long nbLineInTabData,WebCommon.Results.SelectionGroup[] groupMediaTotalIndex,Hashtable mediaIndex,Hashtable mediaEvolIndex,string mediaListForLabelSearch,int nbUnivers){

//            #region Variables
//            Int64 currentMediaN1;
//            Int64 idMedia;
//            string[] mediaList;
//            Int64 oldIdL1=-1;
//            Int64 oldIdL2=-1;
//            Int64 oldIdL3=-1;
//            long currentLine;
//            long currentLineInTabResult;
//            long k;
//            ArrayList l2Indexes=new ArrayList();
//            ArrayList l1Indexes=new ArrayList();
//            #endregion

//            #region Calcul des PDM ?
//            bool computePDM=false;
//            if(webSession.Percentage) computePDM=true;
//            #endregion

//            #region Affiche le Gad ?
//            bool showGad=false;
//            int advertiserColumnIndex=GetAdvertiserColumnIndex(webSession);
//            if(advertiserColumnIndex>=0){
//                showGad=true;
//            }
//            #endregion

//            #region Déclaration du tableau de résultat
//            long nbCol=tabData.GetLength(0);
//            long nbLine=GetNbLineFromPreformatedTableToResultTable(tabData)+FrameWorkResultConstantes.DynamicAnalysis.FIRST_LINE_RESULT_INDEX;
//            object[,] tabResult=new object[nbCol,nbLine];
//            #endregion

//            #region Positionnement des groupes et supports
//            // Ecriture des groupes de sélections
//            for(k=1;k<nbUnivers;k++){
//                if(groupMediaTotalIndex[k].Count>1)
//                    tabResult[groupMediaTotalIndex[k].IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.TOTAL_GROUP_LINE_INDEX]=groupMediaTotalIndex[k].Id;
//                tabResult[groupMediaTotalIndex[k].IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.MEDIA_GROUP_LINE_INDEX]=groupMediaTotalIndex[k].Id;				
//            }



//            // Ecriture des groupes de sélection d'appartenance des médias
//            mediaList=mediaListForLabelSearch.Split(',');
//            foreach(string currentMedia in mediaList){
//                idMedia=Int64.Parse(currentMedia);
//                tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[idMedia]).IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.MEDIA_GROUP_LINE_INDEX]=((WebCommon.Results.GroupItemForTableResult)mediaIndex[idMedia]).GroupNumber;
//                // Evol
//                tabResult[((WebCommon.Results.GroupItemForTableResult)mediaEvolIndex[idMedia]).IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.MEDIA_GROUP_LINE_INDEX]=((WebCommon.Results.GroupItemForTableResult)mediaEvolIndex[idMedia]).GroupNumber;
//                idMedia=-1*idMedia;
//                tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[idMedia]).IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.MEDIA_GROUP_LINE_INDEX]=((WebCommon.Results.GroupItemForTableResult)mediaIndex[idMedia]).GroupNumber;
				
//            }
//            #endregion
			
//            #region Libellé des colonnes
//            // Chargement des libellés de colonnes
//            ClassificationDB.MediaBranch.PartialMediaListDataAccess mediaLabelList=new ClassificationDB.MediaBranch.PartialMediaListDataAccess(mediaListForLabelSearch,webSession.SiteLanguage,webSession.Source);
//            // Ecriture des médias en colonnes
//            mediaList=mediaListForLabelSearch.Split(',');
//            foreach(string currentMedia in mediaList){
//                tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[Int64.Parse(currentMedia)]).IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.MEDIA_LINE_LABEL_INDEX]=mediaLabelList[Int64.Parse(currentMedia)];
//                tabResult[((WebCommon.Results.GroupItemForTableResult)mediaEvolIndex[Int64.Parse(currentMedia)]).IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.MEDIA_LINE_LABEL_INDEX]=mediaLabelList[Int64.Parse(currentMedia)];
//                tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[-1*Int64.Parse(currentMedia)]).IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.MEDIA_LINE_LABEL_INDEX]=mediaLabelList[Int64.Parse(currentMedia)];
//            }
//            // Ecriture des sous totaux
//            for(k=1;k<nbUnivers;k++){
//                if(groupMediaTotalIndex[k].Count>1)
//                    tabResult[groupMediaTotalIndex[k].IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.MEDIA_LINE_LABEL_INDEX]=GestionWeb.GetWebWord(1102,webSession.SiteLanguage);
//            }

//            #endregion

//            #region Conversion de mediaList
//            Int64[] mediaListLong=new Int64[mediaList.GetLength(0)];
//            k=0;
//            foreach(string currentMedia in mediaList){
//                mediaListLong[k]=Int64.Parse(currentMedia);
//                k++;
//            }
//            #endregion

//            #region Ligne du Total
//            tabResult[FrameWorkResultConstantes.DynamicAnalysis.IDL1_INDEX,FrameWorkResultConstantes.DynamicAnalysis.TOTAL_LINE_INDEX]=FrameWorkResultConstantes.DynamicAnalysis.TOTAL_IDENTIFICATION;
//            tabResult[FrameWorkResultConstantes.DynamicAnalysis.LABELL1_INDEX,FrameWorkResultConstantes.DynamicAnalysis.TOTAL_LINE_INDEX]=GestionWeb.GetWebWord(805,webSession.SiteLanguage);
//            // Initialisation de la ligne
//            for(k=FrameWorkResultConstantes.DynamicAnalysis.FIRST_MEDIA_INDEX;k<nbCol;k++){
//                tabResult[k,FrameWorkResultConstantes.DynamicAnalysis.TOTAL_LINE_INDEX]=(double) 0.0;
//            }
//            #endregion

//            #region Tableau de résultat

//            oldIdL1=-1;
//            oldIdL2=-1;
//            oldIdL3=-1;
//            long currentL1Index=-1;
//            long currentL2Index=-1;
//            long currentL3Index=-1;
//            currentLineInTabResult=FrameWorkResultConstantes.DynamicAnalysis.FIRST_LINE_RESULT_INDEX-1;
//            for(currentLine=0;currentLine<nbLineInTabData;currentLine++){

//                #region On change de niveau L1
//                if(tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL1_INDEX,currentLine]!=null && (Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL1_INDEX,currentLine]!=oldIdL1){
//                    currentLineInTabResult++;				

//                    #region GAD
//                    if(showGad && advertiserColumnIndex==FrameWorkResultConstantes.DynamicAnalysis.IDL1_INDEX){
//                        if(tabData[FrameWorkResultConstantes.DynamicAnalysis.ADDRESS_COLUMN_INDEX,currentLine]!=null){
//                            tabResult[FrameWorkResultConstantes.DynamicAnalysis.ADDRESS_COLUMN_INDEX,currentLineInTabResult]=tabData[FrameWorkResultConstantes.DynamicAnalysis.ADDRESS_COLUMN_INDEX,currentLine];
//                        }
//                    }
//                    #endregion

//                    #region Calcul des PDMS
//                    if(computePDM && currentL1Index>0){
//                        // PDMS des Médias
//                        foreach(long currentMedia in mediaListLong){
//                            if(groupMediaTotalIndex[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).GroupNumber].Count>1){
//                                tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).IndexInResultTable,currentL1Index]=(double)tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).IndexInResultTable,currentL1Index]/(double)tabResult[groupMediaTotalIndex[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).GroupNumber].IndexInResultTable,currentL1Index]*100;
//                                currentMediaN1=-1*currentMedia;
//                                tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMediaN1]).IndexInResultTable,currentL1Index]=(double)tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMediaN1]).IndexInResultTable,currentL1Index]/(double)tabResult[groupMediaTotalIndex[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMediaN1]).GroupNumber].IndexInResultTable,currentL1Index]*100;
//                            }
//                        }
//                        // PDMS des groupes de sélections
//                        for(k=1;k<nbUnivers;k++){
//                            //if(groupMediaTotalIndex[k].Count>1)
//                            tabResult[groupMediaTotalIndex[k].IndexInResultTable,currentL1Index]=100;
//                        }
//                        // PDM du Total
//                        //tabResult[FrameWorkResultConstantes.DynamicAnalysis.TOTAL_INDEX,currentL1Index]=100.0;
//                    }
//                    #endregion				

//                    #region Evol L1
//                    if(currentL1Index>0){						
//                        if(groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION].Count>1){
//                            tabResult[(long)groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION].IndexInResultTable,currentL1Index]=((double)(tabResult[(long)groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION].IndexInResultTable,currentL1Index])/(double)(tabResult[(long)groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N1_UNIVERSE_POSITION].IndexInResultTable,currentL1Index])-1)*100;
//                        }						
//                        foreach(WebCommon.Results.GroupItem item in mediaEvolIndex.Values){
//                            if(tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[-(item.ID)]).IndexInResultTable,currentL1Index]!=null){								
//                                tabResult[((WebCommon.Results.GroupItemForTableResult)mediaEvolIndex[item.ID]).IndexInResultTable,currentL1Index]=(((double) tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[item.ID]).IndexInResultTable,currentL1Index]/(double) tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[-(item.ID)]).IndexInResultTable,currentL1Index])-1)*100;
									
//                            }
//                            else{
//                                tabResult[((WebCommon.Results.GroupItemForTableResult)mediaEvolIndex[item.ID]).IndexInResultTable,currentL1Index]=(double)0;
//                            }						
//                        }			
//                    }
//                    #endregion

//                    // Totaux et sous Totaux à 0 et media
//                    for(k=FrameWorkResultConstantes.DynamicAnalysis.FIRST_MEDIA_INDEX;k<nbCol;k++){
//                        tabResult[k,currentLineInTabResult]=(double) 0.0;
//                    }

//                    oldIdL1=(Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL1_INDEX,currentLine];
//                    tabResult[FrameWorkResultConstantes.DynamicAnalysis.IDL1_INDEX,currentLineInTabResult]=oldIdL1;
//                    tabResult[FrameWorkResultConstantes.DynamicAnalysis.LABELL1_INDEX,currentLineInTabResult]=tabData[FrameWorkResultConstantes.DynamicAnalysis.LABELL1_INDEX,currentLine];		
//                    currentL1Index=currentLineInTabResult;
//                    oldIdL2=oldIdL3=-1;
//                }
//                #endregion

//                #region On change de niveau L2
//                if(tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL2_INDEX,currentLine]!=null && (Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL2_INDEX,currentLine]!=oldIdL2){				
		
//                    currentLineInTabResult++;				

//                    #region GAD
//                    if(showGad && advertiserColumnIndex==FrameWorkResultConstantes.DynamicAnalysis.IDL2_INDEX){
//                        if(tabData[FrameWorkResultConstantes.DynamicAnalysis.ADDRESS_COLUMN_INDEX,currentLine]!=null){
//                            tabResult[FrameWorkResultConstantes.DynamicAnalysis.ADDRESS_COLUMN_INDEX,currentLineInTabResult]=tabData[FrameWorkResultConstantes.DynamicAnalysis.ADDRESS_COLUMN_INDEX,currentLine];
//                        }
//                    }
//                    #endregion

//                    #region Calcul des PDMS
//                    if(computePDM && currentL2Index>0){
//                        // PDMS des Médias
//                        foreach(long currentMedia in mediaListLong){
//                            if(groupMediaTotalIndex[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).GroupNumber].Count>1){
//                                tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).IndexInResultTable,currentL2Index]=(double)tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).IndexInResultTable,currentL2Index]/(double)tabResult[groupMediaTotalIndex[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).GroupNumber].IndexInResultTable,currentL2Index]*100;
//                                currentMediaN1=-1*currentMedia;
//                                tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMediaN1]).IndexInResultTable,currentL2Index]=(double)tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMediaN1]).IndexInResultTable,currentL2Index]/(double)tabResult[groupMediaTotalIndex[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMediaN1]).GroupNumber].IndexInResultTable,currentL2Index]*100;
//                            }
//                        }
//                        // PDMS des groupes de sélections
//                        for(k=1;k<nbUnivers;k++)
//                        {
//                            //if(groupMediaTotalIndex[k].Count>1)
//                            tabResult[groupMediaTotalIndex[k].IndexInResultTable,currentL2Index]=100;
//                        }
//                        // PDM du Total
//                        //tabResult[FrameWorkResultConstantes.DynamicAnalysis.TOTAL_INDEX,currentL2Index]=100.0;
//                    }

//                    #endregion

//                    #region Evol L2
//                    if(currentL2Index>0){						
//                        if(groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION].Count>1){
//                            tabResult[(long)groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION].IndexInResultTable,currentL2Index]=((double)(tabResult[(long)groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION].IndexInResultTable,currentL2Index])/(double)(tabResult[(long)groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N1_UNIVERSE_POSITION].IndexInResultTable,currentL2Index])-1)*100;
//                        }						
//                        foreach(WebCommon.Results.GroupItem item in mediaEvolIndex.Values){							
//                            if(tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[-(item.ID)]).IndexInResultTable,currentL2Index]!=null){								
//                                tabResult[((WebCommon.Results.GroupItemForTableResult)mediaEvolIndex[item.ID]).IndexInResultTable,currentL2Index]=(((double) tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[item.ID]).IndexInResultTable,currentL2Index]/(double) tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[-(item.ID)]).IndexInResultTable,currentL2Index])-1)*100;
									
//                            }
//                            else{
//                                tabResult[((WebCommon.Results.GroupItemForTableResult)mediaEvolIndex[item.ID]).IndexInResultTable,currentL2Index]=(double)0;
//                            }						
//                        }			
//                    }
//                    #endregion

//                    // Totaux et sous Totaux à 0 et media
//                    for(k=FrameWorkResultConstantes.DynamicAnalysis.FIRST_MEDIA_INDEX;k<nbCol;k++){
//                        tabResult[k,currentLineInTabResult]=(double) 0.0;
//                    }

//                    oldIdL2=(Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL2_INDEX,currentLine];
//                    tabResult[FrameWorkResultConstantes.DynamicAnalysis.IDL2_INDEX,currentLineInTabResult]=oldIdL2;
//                    tabResult[FrameWorkResultConstantes.DynamicAnalysis.LABELL2_INDEX,currentLineInTabResult]=tabData[FrameWorkResultConstantes.DynamicAnalysis.LABELL2_INDEX,currentLine];		
//                    currentL2Index=currentLineInTabResult;
//                    oldIdL3=-1;
//                }
//                #endregion
			
//                #region On change de niveau L3
//                if(tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL3_INDEX,currentLine]!=null && (Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL3_INDEX,currentLine]!=oldIdL3){
//                    currentLineInTabResult++;

//                    #region GAD
//                    if(showGad && advertiserColumnIndex==FrameWorkResultConstantes.DynamicAnalysis.IDL3_INDEX){
//                        if(tabData[FrameWorkResultConstantes.DynamicAnalysis.ADDRESS_COLUMN_INDEX,currentLine]!=null){
//                            tabResult[FrameWorkResultConstantes.DynamicAnalysis.ADDRESS_COLUMN_INDEX,currentLineInTabResult]=tabData[FrameWorkResultConstantes.DynamicAnalysis.ADDRESS_COLUMN_INDEX,currentLine];
//                        }
//                    }
//                    #endregion

//                    #region Calcul des PDMS
//                    if(computePDM && currentL3Index>0){
//                        // PDMS des Médias
//                        foreach(long currentMedia in mediaListLong){
//                            if(groupMediaTotalIndex[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).GroupNumber].Count>1){
//                                tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).IndexInResultTable,currentL3Index]=(double)tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).IndexInResultTable,currentL3Index]/(double)tabResult[groupMediaTotalIndex[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).GroupNumber].IndexInResultTable,currentL3Index]*100;
//                                currentMediaN1=-1*currentMedia;
//                                tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMediaN1]).IndexInResultTable,currentL3Index]=(double)tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMediaN1]).IndexInResultTable,currentL3Index]/(double)tabResult[groupMediaTotalIndex[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMediaN1]).GroupNumber].IndexInResultTable,currentL3Index]*100;
//                            }
//                        }
//                        // PDMS des groupes de sélections
//                        for(k=1;k<nbUnivers;k++){
//                            //if(groupMediaTotalIndex[k].Count>1)
//                            tabResult[groupMediaTotalIndex[k].IndexInResultTable,currentL3Index]=100;
//                        }
//                        // PDM du Total
//                        //tabResult[FrameWorkResultConstantes.DynamicAnalysis.TOTAL_INDEX,currentL3Index]=100.0;
//                    }
//                    #endregion

//                    #region Evol L3
//                    if(currentL3Index>0){						
//                        if(groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION].Count>1){
//                            tabResult[(long)groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION].IndexInResultTable,currentL3Index]=((double)(tabResult[(long)groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION].IndexInResultTable,currentL3Index])/(double)(tabResult[(long)groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N1_UNIVERSE_POSITION].IndexInResultTable,currentL3Index])-1)*100;
//                        }						
//                        foreach(WebCommon.Results.GroupItem item in mediaEvolIndex.Values){							
//                            if(tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[-(item.ID)]).IndexInResultTable,currentL3Index]!=null){								
//                                tabResult[((WebCommon.Results.GroupItemForTableResult)mediaEvolIndex[item.ID]).IndexInResultTable,currentL3Index]=(((double) tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[item.ID]).IndexInResultTable,currentL3Index]/(double) tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[-(item.ID)]).IndexInResultTable,currentL3Index])-1)*100;
//                            }
//                            else{
//                                tabResult[((WebCommon.Results.GroupItemForTableResult)mediaEvolIndex[item.ID]).IndexInResultTable,currentL3Index]=(double)0;
//                            }						
//                        }			
//                    }
//                    #endregion

//                    // Totaux et sous Totaux à 0 et media
//                    for(k=FrameWorkResultConstantes.DynamicAnalysis.FIRST_MEDIA_INDEX;k<nbCol;k++){
//                        tabResult[k,currentLineInTabResult]=(double) 0.0;
//                    }

//                    oldIdL3=(Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL3_INDEX,currentLine];
//                    tabResult[FrameWorkResultConstantes.DynamicAnalysis.IDL3_INDEX,currentLineInTabResult]=oldIdL3;
//                    tabResult[FrameWorkResultConstantes.DynamicAnalysis.LABELL3_INDEX,currentLineInTabResult]=tabData[FrameWorkResultConstantes.DynamicAnalysis.LABELL3_INDEX,currentLine];		
//                    currentL3Index=currentLineInTabResult;
//                }
//                #endregion

//                // On copy la ligne et on l'ajoute aux totaux
//                for(k=FrameWorkResultConstantes.DynamicAnalysis.FIRST_MEDIA_INDEX;k<nbCol;k++){
//                    tabResult[k,currentLineInTabResult]=tabData[k,currentLine];
//                    // Ecriture de L1
//                    if(currentL1Index>=0 && currentL2Index>=0){
//                        tabResult[k,currentL1Index]=(double)tabResult[k,currentL1Index]+(double)tabData[k,currentLine];
//                    }
//                    // Ecriture de L2
//                    if(currentL2Index>=0 && currentL3Index>=0){
//                        tabResult[k,currentL2Index]=(double)tabResult[k,currentL2Index]+(double)tabData[k,currentLine];
//                    }
//                    // Ecriture du total
//                    tabResult[k,FrameWorkResultConstantes.DynamicAnalysis.TOTAL_LINE_INDEX]=double.Parse(tabResult[k,FrameWorkResultConstantes.DynamicAnalysis.TOTAL_LINE_INDEX].ToString())+double.Parse(tabData[k,currentLine].ToString());


//                }
//            }		

//            #region PDM de FIN
//            if(computePDM){

//                #region Calcul des PDMS L3
//                if(computePDM && currentL3Index>0){
//                    // PDMS des Médias
//                    foreach(long currentMedia in mediaListLong){
//                        if(groupMediaTotalIndex[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).GroupNumber].Count>1){
//                            tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).IndexInResultTable,currentL3Index]=(double)tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).IndexInResultTable,currentL3Index]/(double)tabResult[groupMediaTotalIndex[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).GroupNumber].IndexInResultTable,currentL3Index]*100;
//                            currentMediaN1=-1*currentMedia;
//                            tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMediaN1]).IndexInResultTable,currentL3Index]=(double)tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMediaN1]).IndexInResultTable,currentL3Index]/(double)tabResult[groupMediaTotalIndex[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMediaN1]).GroupNumber].IndexInResultTable,currentL3Index]*100;
//                        }
//                    }
//                    // PDMS des groupes de sélections
//                    for(k=1;k<nbUnivers;k++){
//                        //if(groupMediaTotalIndex[k].Count>1)
//                        tabResult[groupMediaTotalIndex[k].IndexInResultTable,currentL3Index]=100;
//                    }
//                    // PDM du Total
//                    //tabResult[FrameWorkResultConstantes.DynamicAnalysis.TOTAL_INDEX,currentL3Index]=100.0;
//                }
//                #endregion
				
//                #region Calcul des PDMS L2
//                if(computePDM && currentL2Index>0){
//                    // PDMS des Médias
//                    foreach(long currentMedia in mediaListLong){
//                        if(groupMediaTotalIndex[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).GroupNumber].Count>1){
//                            tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).IndexInResultTable,currentL2Index]=(double)tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).IndexInResultTable,currentL2Index]/(double)tabResult[groupMediaTotalIndex[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).GroupNumber].IndexInResultTable,currentL2Index]*100;
//                            currentMediaN1=-1*currentMedia;
//                            tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMediaN1]).IndexInResultTable,currentL2Index]=(double)tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMediaN1]).IndexInResultTable,currentL2Index]/(double)tabResult[groupMediaTotalIndex[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMediaN1]).GroupNumber].IndexInResultTable,currentL2Index]*100;
//                        }
//                    }
////					// PDMS des groupes de sélections
//                    for(k=1;k<nbUnivers;k++){
//                        //if(groupMediaTotalIndex[k].Count>1)
//                        tabResult[groupMediaTotalIndex[k].IndexInResultTable,currentL2Index]=100;
//                    }
////					// PDM du Total
////					tabResult[FrameWorkResultConstantes.DynamicAnalysis.TOTAL_INDEX,currentL2Index]=100.0;
//                }

//                #endregion

//                #region Calcul des PDMS L1
//                if(computePDM && currentL1Index>0){
//                    // PDMS des Médias
//                    foreach(long currentMedia in mediaListLong){
//                        if(groupMediaTotalIndex[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).GroupNumber].Count>1){
//                            tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).IndexInResultTable,currentL1Index]=(double)tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).IndexInResultTable,currentL1Index]/(double)tabResult[groupMediaTotalIndex[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).GroupNumber].IndexInResultTable,currentL1Index]*100;
//                            currentMediaN1=-1*currentMedia;
//                            tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMediaN1]).IndexInResultTable,currentL1Index]=(double)tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMediaN1]).IndexInResultTable,currentL1Index]/(double)tabResult[groupMediaTotalIndex[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMediaN1]).GroupNumber].IndexInResultTable,currentL1Index]*100;
//                        }
//                    }
////					// PDMS des groupes de sélections
//                    for(k=1;k<nbUnivers;k++){
//                        //if(groupMediaTotalIndex[k].Count>1)
//                        tabResult[groupMediaTotalIndex[k].IndexInResultTable,currentL1Index]=100;
//                    }
////					// PDM du Total
////					tabResult[FrameWorkResultConstantes.DynamicAnalysis.TOTAL_INDEX,currentL1Index]=100.0;
//                }

//                #endregion

//                #region Calcul des PDMS Total
//                if(computePDM){
//                    // PDMS des Médias
//                    foreach(long currentMedia in mediaListLong){
//                        if(groupMediaTotalIndex[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).GroupNumber].Count>1){
//                            tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.TOTAL_LINE_INDEX]=(double)tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.TOTAL_LINE_INDEX]/(double)tabResult[groupMediaTotalIndex[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMedia]).GroupNumber].IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.TOTAL_LINE_INDEX]*100;
//                            currentMediaN1=-1*currentMedia;
//                            tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMediaN1]).IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.TOTAL_LINE_INDEX]=(double)tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMediaN1]).IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.TOTAL_LINE_INDEX]/(double)tabResult[groupMediaTotalIndex[((WebCommon.Results.GroupItemForTableResult)mediaIndex[currentMediaN1]).GroupNumber].IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.TOTAL_LINE_INDEX]*100;
//                        }
//                    }
//                    // PDMS des groupes de sélections
//                    for(k=1;k<nbUnivers;k++){
//                        //if(groupMediaTotalIndex[k].Count>1)
//                        tabResult[groupMediaTotalIndex[k].IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.TOTAL_LINE_INDEX]=100;
//                    }
////					// PDM du Total
////					tabResult[FrameWorkResultConstantes.DynamicAnalysis.TOTAL_INDEX,FrameWorkResultConstantes.DynamicAnalysis.TOTAL_LINE_INDEX]=100.0;
//                }

//                #endregion

//            }
//            #endregion

//            #region Evol L1		
//            if(currentL1Index>0){		
//                if(groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION].Count>0){
//                    tabResult[(long)groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION].IndexInResultTable,currentL1Index]=((double)(tabResult[(long)groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION].IndexInResultTable,currentL1Index])/(double)(tabResult[(long)groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N1_UNIVERSE_POSITION].IndexInResultTable,currentL1Index])-1)*100;
//                }						
//                foreach(WebCommon.Results.GroupItem item in mediaEvolIndex.Values){							
//                    if(tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[-(item.ID)]).IndexInResultTable,currentL1Index]!=null){								
//                        tabResult[((WebCommon.Results.GroupItemForTableResult)mediaEvolIndex[item.ID]).IndexInResultTable,currentL1Index]=(((double) tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[item.ID]).IndexInResultTable,currentL1Index]/(double) tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[-(item.ID)]).IndexInResultTable,currentL1Index])-1)*100;
//                    }
//                    else{
//                        tabResult[((WebCommon.Results.GroupItemForTableResult)mediaEvolIndex[item.ID]).IndexInResultTable,currentL1Index]=(double)0;
//                    }						
//                }			
//            }
//            #endregion

//            #region Evol L2
//            if(currentL2Index>0){					
//                if(groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION].Count>0){
//                    tabResult[(long)groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION].IndexInResultTable,currentL2Index]=((double)(tabResult[(long)groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION].IndexInResultTable,currentL2Index])/(double)(tabResult[(long)groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N1_UNIVERSE_POSITION].IndexInResultTable,currentL2Index])-1)*100;
//                }						
//                foreach(WebCommon.Results.GroupItem item in mediaEvolIndex.Values){							
//                    if(tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[-(item.ID)]).IndexInResultTable,currentL2Index]!=null){								
//                        tabResult[((WebCommon.Results.GroupItemForTableResult)mediaEvolIndex[item.ID]).IndexInResultTable,currentL2Index]=(((double) tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[item.ID]).IndexInResultTable,currentL2Index]/(double) tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[-(item.ID)]).IndexInResultTable,currentL2Index])-1)*100;
//                    }
//                    else{
//                        tabResult[((WebCommon.Results.GroupItemForTableResult)mediaEvolIndex[item.ID]).IndexInResultTable,currentL2Index]=(double)0;
//                    }						
//                }			
//            }
//            #endregion

//            #region Evol L3	
//            if(currentL3Index>0){				
//                if(groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION].Count>0){
//                    tabResult[(long)groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION].IndexInResultTable,currentL3Index]=((double)(tabResult[(long)groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION].IndexInResultTable,currentL3Index])/(double)(tabResult[(long)groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N1_UNIVERSE_POSITION].IndexInResultTable,currentL3Index])-1)*100;
//                }					
//                foreach(WebCommon.Results.GroupItem item in mediaEvolIndex.Values){							
//                    if(tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[-(item.ID)]).IndexInResultTable,currentL3Index]!=null){								
//                        tabResult[((WebCommon.Results.GroupItemForTableResult)mediaEvolIndex[item.ID]).IndexInResultTable,currentL3Index]=(((double) tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[item.ID]).IndexInResultTable,currentL3Index]/(double) tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[-(item.ID)]).IndexInResultTable,currentL3Index])-1)*100;
//                    }
//                    else{
//                        tabResult[((WebCommon.Results.GroupItemForTableResult)mediaEvolIndex[item.ID]).IndexInResultTable,currentL3Index]=(double)0;
//                    }						
//                }		
//            }
//            #endregion

//            #region Evol Total								
//            if(groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION].Count>0){
//                tabResult[(long)groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.EVOL_UNIVERSE_POSITION].IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.TOTAL_LINE_INDEX]=((double)(tabResult[(long)groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N_UNIVERSE_POSITION].IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.TOTAL_LINE_INDEX])/(double)(tabResult[(long)groupMediaTotalIndex[FrameWorkResultConstantes.DynamicAnalysis.N1_UNIVERSE_POSITION].IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.TOTAL_LINE_INDEX])-1)*100;
//            }					
//            foreach(WebCommon.Results.GroupItem item in mediaEvolIndex.Values){							
//                if(tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[-(item.ID)]).IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.TOTAL_LINE_INDEX]!=null){
//                    tabResult[((WebCommon.Results.GroupItemForTableResult)mediaEvolIndex[item.ID]).IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.TOTAL_LINE_INDEX]=(((double) tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[item.ID]).IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.TOTAL_LINE_INDEX]/(double) tabResult[((WebCommon.Results.GroupItemForTableResult)mediaIndex[-(item.ID)]).IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.TOTAL_LINE_INDEX])-1)*100;
//                }
//                else{
//                    tabResult[((WebCommon.Results.GroupItemForTableResult)mediaEvolIndex[item.ID]).IndexInResultTable,FrameWorkResultConstantes.DynamicAnalysis.TOTAL_LINE_INDEX]=(double)0;
//                }						
//            }			
		
//            #endregion
						
//            #endregion

//            #region Debug: voir le tableau			
//            #if(DEBUG)
////						int i,j;
////						string HTML="<html><table><tr>";
////						for(i=0;i<=currentLineInTabResult;i++){
////							for(j=0;j<nbCol;j++){
////								if(tabResult[j,i]!=null)HTML+="<td>"+tabResult[j,i].ToString()+"</td>";
////								else HTML+="<td>&nbsp;</td>";
////							}
////							HTML+="</tr><tr>";
////						}
////						HTML+="</tr></table></html>";
//            #endif
//            #endregion

//            return(tabResult);
//        }
//        #endregion

//        #region Calcul du nombre de ligne d'un tableau préformaté
//        /// <summary>
//        /// Obtient le nombre de ligne du tableau de résultat à partir d'un tableau préformaté
//        /// </summary>
//        /// <param name="tabData">Tableau préformaté</param>
//        /// <returns>Nombre de ligne du tableau de résultat</returns>
//        private static long GetNbLineFromPreformatedTableToResultTable(object[,] tabData){

//            #region Variables
//            long nbLine=0;
//            long k;
//            Int64 oldIdL1=-1;
//            Int64 oldIdL2=-1;
//            Int64 oldIdL3=-1;
//            #endregion

//            for(k=0;k<tabData.GetLength(1);k++){
//                // Somme des L1
//                if(tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL1_INDEX,k]!=null && (Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL1_INDEX,k]!=oldIdL1){
//                    oldIdL1=(Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL1_INDEX,k];
//                    nbLine++;
//                    oldIdL3=oldIdL2=-1;
//                }
//                // Somme des L2
//                if(tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL2_INDEX,k]!=null && (Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL2_INDEX,k]!=oldIdL2){
//                    oldIdL2=(Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL2_INDEX,k];
//                    nbLine++;
//                    oldIdL3=-1;
//                }
//                // Somme des L3
//                if(tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL3_INDEX,k]!=null && (Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL3_INDEX,k]!=oldIdL3){
//                    oldIdL3=(Int64)tabData[FrameWorkResultConstantes.DynamicAnalysis.IDL3_INDEX,k];
//                    nbLine++;
//                }
//            }
//            return(nbLine);
//        }
//        #endregion

//        #region Identifiant des éléments de la nomenclature produit
////		/// <summary>
////		/// Obtient l'identifiant du détail période de niveau 1
////		/// </summary>
////		/// <param name="webSession">Session du client</param>
////		/// <param name="dr">Ligne de la table de résultat</param>
////		/// <returns>Identifiant</returns>
////		private static Int64 getL1Id(WebSession webSession,DataRow dr){
////			try{
////				switch(webSession.PreformatedProductDetail){
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sector:
////						return(Int64.Parse(dr["id_sector"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsector:
////						return(Int64.Parse(dr["id_sector"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
////						return((Int64)dr["id_sector"]);
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser:
////						return(Int64.Parse(dr["id_sector"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorProduct:
////						return(Int64.Parse(dr["id_sector"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser:
////						return(Int64.Parse(dr["id_advertiser"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand:
////						return(Int64.Parse(dr["id_advertiser"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct:
////						return(Int64.Parse(dr["id_advertiser"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
////						return(Int64.Parse(dr["id_advertiser"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group:
////						return(Int64.Parse(dr["id_group_"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand:
////						return(Int64.Parse(dr["id_group_"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupProduct:
////						return(Int64.Parse(dr["id_group_"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
////						return(Int64.Parse(dr["id_group_"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
////						return(Int64.Parse(dr["ID_GROUP_ADVERTISING_AGENCY"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
////						return(Int64.Parse(dr["ID_ADVERTISING_AGENCY"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
////						return(Int64.Parse(dr["ID_ADVERTISING_AGENCY"].ToString()));
////					default:
////						return(-1);
////				}
////			}
////			catch(Exception){
////				return(-1);
////			}
////		}
////
////		/// <summary>
////		/// Obtient l'identifiant du détail période de niveau 2
////		/// </summary>
////		/// <param name="webSession">Session du client</param>
////		/// <param name="dr">Ligne de la table de résultat</param>
////		/// <returns>Identifiant</returns>
////		private static Int64 getL2Id(WebSession webSession,DataRow dr){
////			try{
////				switch(webSession.PreformatedProductDetail){
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsector:
////						return(Int64.Parse(dr["id_subsector"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorProduct:
////						return(Int64.Parse(dr["id_product"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
////						return(Int64.Parse(dr["id_advertiser"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser:
////						return(Int64.Parse(dr["id_advertiser"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand:
////						return(Int64.Parse(dr["id_brand"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct:
////						return(Int64.Parse(dr["id_product"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
////						return(Int64.Parse(dr["id_brand"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand:
////						return(Int64.Parse(dr["id_brand"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupProduct:
////						return(Int64.Parse(dr["id_product"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
////						return(Int64.Parse(dr["id_brand"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
////						return(Int64.Parse(dr["ID_ADVERTISING_AGENCY"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
////						return(Int64.Parse(dr["ID_ADVERTISER"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
////						return(Int64.Parse(dr["id_product"].ToString()));
////					default:
////						return(-1);
////				}
////			}
////			catch(Exception){
////				return(-1);
////			}
////		}
////
////		/// <summary>
////		/// Obtient l'identifiant du détail période de niveau 3
////		/// </summary>
////		/// <param name="webSession">Session du client</param>
////		/// <param name="dr">Ligne de la table de résultat</param>
////		/// <returns>Identifiant</returns>
////		private static Int64 getL3Id(WebSession webSession,DataRow dr){
////			try{
////				switch(webSession.PreformatedProductDetail){
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
////						return(Int64.Parse(dr["id_group_"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
////						return(Int64.Parse(dr["id_product"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
////						return(Int64.Parse(dr["id_product"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
////						return(Int64.Parse(dr["id_product"].ToString()));
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
////						return(-1);
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
////						return(-1);
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
////						return(-1);
////					default:
////						return(-1);
////				}
////			}
////			catch(Exception){
////				return(-1);
////			}
////		}
//        #endregion

//        #region Libellés des éléments de la nomenclature produit
////		/// <summary>
////		/// Obtient le libellé du détail période de niveau 1
////		/// </summary>
////		/// <param name="webSession">Session du client</param>
////		/// <param name="dr">Ligne de la table de résultat</param>
////		/// <returns>Identifiant</returns>
////		private static string getL1Label(WebSession webSession,DataRow dr){
////			try{
////				switch(webSession.PreformatedProductDetail){
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sector:
////						return(dr["sector"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsector:
////						return(dr["sector"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorProduct:
////						return(dr["sector"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
////						return(dr["sector"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser:
////						return(dr["sector"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser:
////						return(dr["advertiser"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand:
////						return(dr["advertiser"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct:
////						return(dr["advertiser"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
////						return(dr["advertiser"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group:
////						return(dr["group_"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand:
////						return(dr["group_"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupProduct:
////						return(dr["group_"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
////						return(dr["group_"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
////						return(dr["GROUP_ADVERTISING_AGENCY"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
////						return(dr["ADVERTISING_AGENCY"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
////						return(dr["ADVERTISING_AGENCY"].ToString());
////					default:
////						return("!!");
////				}
////			}
////			catch(Exception){
////				return("!!");
////			}
////		}
////
////		/// <summary>
////		/// Obtient le libellé du détail période de niveau 2
////		/// </summary>
////		/// <param name="webSession">Session du client</param>
////		/// <param name="dr">Ligne de la table de résultat</param>
////		/// <returns>Identifiant</returns>
////		private static string getL2Label(WebSession webSession,DataRow dr){
////			try{
////				switch(webSession.PreformatedProductDetail){
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsector:
////						return(dr["subsector"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorProduct:
////						return(dr["product"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
////						return(dr["advertiser"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser:
////						return(dr["advertiser"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand:
////						return(dr["brand"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct:
////						return(dr["product"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
////						return(dr["brand"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand:
////						return(dr["brand"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupProduct:
////						return(dr["product"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
////						return(dr["brand"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
////						return(dr["ADVERTISING_AGENCY"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
////						return(dr["advertiser"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
////						return(dr["product"].ToString());
////					default:
////						return("!!");
////				}
////			}
////			catch(Exception){
////				return("!!");
////			}
////		}
////
////		/// <summary>
////		/// Obtient le libellé du détail période de niveau 3
////		/// </summary>
////		/// <param name="webSession">Session du client</param>
////		/// <param name="dr">Ligne de la table de résultat</param>
////		/// <returns>Identifiant</returns>
////		private static string getL3Label(WebSession webSession,DataRow dr){
////			try{
////				switch(webSession.PreformatedProductDetail){
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
////						return(dr["group_"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
////						return(dr["product"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
////						return(dr["product"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
////						return(dr["product"].ToString());
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
////						return("!!");
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
////						return("!!");
////					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
////						return("!!");
////					default:
////						return("!!");
////				}
////			}
////			catch(Exception){
////				return("!!");
////			}
////		}
//        #endregion

//        #region Colonne du détail annonceur
//        /// <summary>
//        /// Détermine la colonne du détail annonceur
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Index de la colonne</returns>
//        internal static int GetAdvertiserColumnIndex(WebSession webSession){
//            switch(webSession.PreformatedProductDetail){
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
//                    return(FrameWorkResultConstantes.DynamicAnalysis.IDL1_INDEX);
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiser:					
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiser:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserBrand:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserProduct:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserProduct:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserBrand:
//                    return(FrameWorkResultConstantes.DynamicAnalysis.IDL2_INDEX);
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser:
//                    return(FrameWorkResultConstantes.DynamicAnalysis.IDL3_INDEX);
//                default:
//                    return(-1);
//            }
//        }

//        #endregion

//        #region Indique si une date numérique appartient à la période comparative ou non
//        /// <summary>
//        /// Indique si une date numérique appartient à la période comparative ou non
//        /// </summary>
//        /// <param name="dateToCompare">Date étudiée</param>
//        /// <param name="beginningDate">Date de début</param>
//        /// <param name="endDate">Date de fin</param>
//        /// <returns>True si la date appartient à la période comparative false sinon</returns>
//        private static bool IsComparativeDateLine(Int64 dateToCompare,Int64 beginningDate, Int64 endDate){
            
//            if (dateToCompare.ToString().Length == 6) {
//                beginningDate = beginningDate / 100;
//                endDate = endDate / 100;
//            }

//            if(dateToCompare>=beginningDate && dateToCompare<=endDate) return(false);
//            return(true);
//        }
//        #endregion

//        #region Obtient l'activité publicitaire d'un produit
//        /// <summary>
//        /// Obtient l'activité publicitaire d'un produit
//        /// </summary>
//        /// <param name="tabResult">Tableau de résultats</param>
//        /// <param name="dt">table de données</param>
//        /// <param name="indexLineProduct">index ligne produit</param>
//        /// <param name="expression">expression de calcul</param>
//        /// <param name="filterN">filtre année N</param>
//        /// <param name="filterN1">filtre année N-1</param>
//        private static void GetProductActivity(ResultTable tabResult,DataTable dt,long indexLineProduct,string expression,string filterN,string filterN1){
//            object unitValueN=System.DBNull.Value;
//            object unitValueN1=System.DBNull.Value;
//            Int64 loyalNumberColonneIndex=tabResult.GetHeadersIndexInResultTable(DynamicResultConstantes.LOYAL_HEADER_ID+"-"+DynamicResultConstantes.ITEM_NUMBER_HEADER_ID);
//            Int64 loyalDeclineNumberColonneIndex=tabResult.GetHeadersIndexInResultTable(DynamicResultConstantes.LOYAL_DECLINE_HEADER_ID+"-"+DynamicResultConstantes.ITEM_NUMBER_HEADER_ID);
//            Int64 loyalRiseNumberColonneIndex=tabResult.GetHeadersIndexInResultTable(DynamicResultConstantes.LOYAL_RISE_HEADER_ID+"-"+DynamicResultConstantes.ITEM_NUMBER_HEADER_ID);
//            Int64 wonNumberColonneIndex=tabResult.GetHeadersIndexInResultTable(DynamicResultConstantes.WON_HEADER_ID+"-"+DynamicResultConstantes.ITEM_NUMBER_HEADER_ID);
//            Int64 lostNumberColonneIndex=tabResult.GetHeadersIndexInResultTable(DynamicResultConstantes.LOST_HEADER_ID+"-"+DynamicResultConstantes.ITEM_NUMBER_HEADER_ID);

//            unitValueN = dt.Compute(expression,filterN);
//            unitValueN1 = dt.Compute(expression,filterN1);

//            #region Fidèles	
						
//            if(unitValueN!=System.DBNull.Value && !unitValueN.ToString().Equals("") && unitValueN1!=System.DBNull.Value && !unitValueN1.ToString().Equals("")){
//                //Nombre 
//                ((CellUnit)tabResult[indexLineProduct,loyalNumberColonneIndex]).Value+=1;
//                //Unité N
//                ((CellUnit)tabResult[indexLineProduct,loyalNumberColonneIndex+1]).Value+=double.Parse(unitValueN.ToString());	
//                //Unité N-1
//                ((CellUnit)tabResult[indexLineProduct,loyalNumberColonneIndex+2]).Value+=double.Parse(unitValueN1.ToString());

//            }
//            #endregion

//            #region Fidèle en baisse
			
//            if(unitValueN!=System.DBNull.Value && !unitValueN.ToString().Equals("") && unitValueN1!=System.DBNull.Value && !unitValueN1.ToString().Equals("") && double.Parse(unitValueN.ToString()) < double.Parse(unitValueN1.ToString())){
//                //Nombre 
//                ((CellUnit)tabResult[indexLineProduct,loyalDeclineNumberColonneIndex]).Value+=1;
//                //Unité N
//                ((CellUnit)tabResult[indexLineProduct,loyalDeclineNumberColonneIndex+1]).Value+=double.Parse(unitValueN.ToString());	
//                //Unité N-1
//                ((CellUnit)tabResult[indexLineProduct,loyalDeclineNumberColonneIndex+2]).Value+=double.Parse(unitValueN1.ToString());

//            }
//            #endregion
			
//            #region Fidèle en  développement
//            if(unitValueN!=System.DBNull.Value && !unitValueN.ToString().Equals("") && unitValueN1!=System.DBNull.Value && !unitValueN1.ToString().Equals("") && double.Parse(unitValueN.ToString()) > double.Parse(unitValueN1.ToString())){
//                //Nombre 
//                ((CellUnit)tabResult[indexLineProduct,loyalRiseNumberColonneIndex]).Value+=1;
//                //Unité N
//                ((CellUnit)tabResult[indexLineProduct,loyalRiseNumberColonneIndex+1]).Value+=double.Parse(unitValueN.ToString());	
//                //Unité N-1
//                ((CellUnit)tabResult[indexLineProduct,loyalRiseNumberColonneIndex+2]).Value+=double.Parse(unitValueN1.ToString());

//            }
//            #endregion
		
//            #region Gagnés
//            if(unitValueN!=System.DBNull.Value && !unitValueN.ToString().Equals("") && (unitValueN1==System.DBNull.Value || unitValueN1.ToString().Equals("")) ){
//                //Nombre 
//                ((CellUnit)tabResult[indexLineProduct,wonNumberColonneIndex]).Value+=1;
//                //Unité N
//                ((CellUnit)tabResult[indexLineProduct,wonNumberColonneIndex+1]).Value+=double.Parse(unitValueN.ToString());	
//            }
//            #endregion

//            #region Perdus
//            if(unitValueN1!=System.DBNull.Value && !unitValueN1.ToString().Equals("") && (unitValueN==System.DBNull.Value || unitValueN.ToString().Equals("")) ){
//                //Nombre 
//                ((CellUnit)tabResult[indexLineProduct,lostNumberColonneIndex]).Value+=1;	
//                //Unité N-1
//                ((CellUnit)tabResult[indexLineProduct,lostNumberColonneIndex+2]).Value+=double.Parse(unitValueN1.ToString());
//            }
//            #endregion

//        }
//        #endregion

//        #region GetNbParutionsByMedia

//        private static Dictionary<string, double> GetNbParutionsByMedia(WebSession webSession) {

//            #region Variables
//            Dictionary<string, double> res = new Dictionary<string, double>();
//            double nbParutionsCounter = 0;
//            Int64 oldColumnDetailLevel = -1;
//            Int64 oldYearParution = -1;
//            bool start = true;
//            string oldKey = "";
//            #endregion

//            #region Sélection du vehicle
//            string vehicleSelection = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerConstantes.Right.type.vehicleAccess);
//            DBClassificationConstantes.Vehicles.names vehicleName = (DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
//            if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new WebExceptions.DynamicRulesException("La sélection de médias est incorrecte"));
//            #endregion

//            #region Chargement des données à partir de la base
//            DataSet ds;
//            try {
//                ds = DynamicDataAccess.GetNbParutionData(webSession, vehicleName);
//            }
//            catch (System.Exception err) {
//                throw (new WebExceptions.DynamicRulesException("Impossible de charger les données pour le nombre de parution", err));
//            }
//            DataTable dt = ds.Tables[0];
//            #endregion

//            if (dt != null && dt.Rows.Count > 0) {
//                foreach (DataRow dr in dt.Rows) {
//                    if (!oldKey.Equals(dr["yearParution"].ToString() + "-" + dr["columnDetailLevel"].ToString()) && !start) {
//                        res.Add(oldKey, nbParutionsCounter);
//                        nbParutionsCounter = 0;
//                    }
//                    nbParutionsCounter += double.Parse(dr["NbParution"].ToString());
//                    start = false;
//                    oldKey = dr["yearParution"].ToString() + "-" + dr["columnDetailLevel"].ToString();
//                }
//                res.Add(oldKey, nbParutionsCounter);
//            }

//            return res;

//        }

//        #endregion

//        #endregion

//    }
//}
