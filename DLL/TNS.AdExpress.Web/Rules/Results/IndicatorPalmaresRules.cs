//#region Informations
//// Auteur: A. Obermeyer 
//// Date de création : 11/10/2004 
//// Date de modification : 11/10/2004 
////	12/08/2005	A.Dadouch	Nom de fonctions
////	24/10/2005	D. V. Mussuma	Intégration unité Keuros	
//#endregion

//using System;
//using System.Data;
//using System.Collections;
//using System.Windows.Forms;

//using TNS.AdExpress.Web;
//using TNS.AdExpress.Web.Core.Sessions;
//using TNS.AdExpress.Web.DataAccess.Results;
//using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
//using WebConstantes = TNS.AdExpress.Constantes.Web;
//using WebFunctions=TNS.AdExpress.Web.Functions;
//using TNS.Classification.Universe;
//using TNS.AdExpress.Classification;

//namespace TNS.AdExpress.Web.Rules.Results{

//    /// <summary>
//    /// Utiliser pour le palmares 
//    /// </summary>
//    public class IndicatorPalmaresRules{

//        #region Constructeur
//        /// <summary>
//        /// Constructeur
//        /// </summary>
//        public IndicatorPalmaresRules(){
//        }
//        #endregion

//        /// <summary>
//        /// Retourne l'objet contenant les infos pour un tableau palmares
//        /// </summary>
//        /// <param name="webSession"></param>
//        /// <param name="typeYear"></param>
//        /// <param name="tableType"></param>
//        /// <returns></returns>
//        public static object[,] GetFormattedTable(WebSession webSession,FrameWorkConstantes.Results.PalmaresRecap.typeYearSelected typeYear,FrameWorkConstantes.Results.PalmaresRecap.ElementType tableType ){
			

//            long i=1;
//            int compteur=0;
//            long nbLineTabResult=0;
//            long rank=1;
//            long oldRank=0;
//            double totalUniverse=0.00;
//            double cumul_Sov=0.00;
//            string tempListAsString = "";
//            string [] tabListCompetitorAdvertiser=null;
//            string [] tabListReferenceAdvertiser=null;
//            NomenclatureElementsGroup nomenclatureElementsGroup = null;
//            bool competitorElement;
			

//            #region Chargement des données à partir de la base	
//            DataSet ds;
//            ds=IndicatorDataAccess.GetPalmaresData(webSession,typeYear,tableType);
//            DataTable dt=ds.Tables[0];			
//            #endregion
			
//            #region Déclaration du tableau de résultat
//            long nbline=dt.Rows.Count;
//            object[,] tabResult=new object[nbline+1,FrameWorkConstantes.Results.PalmaresRecap.COMPETITOR+1];
//            if(nbline+1<11){
//                nbLineTabResult=nbline+1;
//            }
//            else{
//                nbLineTabResult=11;
//            }
//            object[,] finalTabResult=new object[nbLineTabResult,FrameWorkConstantes.Results.PalmaresRecap.COMPETITOR+1];
//            #endregion


		
//            //if(webSession.CompetitorUniversAdvertiser[0]!=null){
//            if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(1)) {
//                //tabListCompetitorAdvertiser=webSession.GetSelection((TreeNode)webSession.CompetitorUniversAdvertiser[0],TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess).Split(',');			
//                nomenclatureElementsGroup = webSession.SecondaryProductUniverses[1].GetGroup(0);
//                if (nomenclatureElementsGroup != null && nomenclatureElementsGroup.Count() > 0) {
//                    tempListAsString = nomenclatureElementsGroup.GetAsString(TNSClassificationLevels.ADVERTISER);
//                    if (tempListAsString != null && tempListAsString.Length > 0) tabListCompetitorAdvertiser = tempListAsString.Split(',');
//                }
//            }
//            if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(0)) {
//                nomenclatureElementsGroup = webSession.SecondaryProductUniverses[0].GetGroup(0);
//                if (nomenclatureElementsGroup != null && nomenclatureElementsGroup.Count() > 0) {
//                    tempListAsString = nomenclatureElementsGroup.GetAsString(TNSClassificationLevels.ADVERTISER);
//                    if (tempListAsString != null && tempListAsString.Length > 0) tabListReferenceAdvertiser = tempListAsString.Split(',');
//                }
//            }
//            //tabListReferenceAdvertiser=webSession.GetSelection(webSession.ReferenceUniversAdvertiser,TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess).Split(',');

//            foreach(DataRow currentRow in dt.Rows){
			
//                if(tableType==FrameWorkConstantes.Results.PalmaresRecap.ElementType.product){
//                    tabResult[i,FrameWorkConstantes.Results.PalmaresRecap.ID_PRODUCT_INDEX]=double.Parse(currentRow["id_product"].ToString());
//                    tabResult[i,FrameWorkConstantes.Results.PalmaresRecap.PRODUCT]=currentRow["product"].ToString();

//                }else if(tableType==FrameWorkConstantes.Results.PalmaresRecap.ElementType.advertiser){
//                    tabResult[i,FrameWorkConstantes.Results.PalmaresRecap.ID_PRODUCT_INDEX]=double.Parse(currentRow["id_advertiser"].ToString());
//                    tabResult[i,FrameWorkConstantes.Results.PalmaresRecap.PRODUCT]=currentRow["advertiser"].ToString();
//                }


//                #region Element Références ?

//                competitorElement=false;
//                if(tabListReferenceAdvertiser!=null){
//                    foreach(string currentValue in tabListReferenceAdvertiser ){
//                        if(currentValue==currentRow["id_advertiser"].ToString()){
//                            competitorElement=true;
//                            break;
//                        }		
//                    }
//                }
//                if(competitorElement){
//                    tabResult[i,FrameWorkConstantes.Results.PalmaresRecap.COMPETITOR]=1;
//                }
//                else{
//                    tabResult[i,FrameWorkConstantes.Results.PalmaresRecap.COMPETITOR]=0;
//                }

//                #endregion

//                #region Element concurent ?
				
//                competitorElement=false;
//                if(tabListCompetitorAdvertiser!=null){
//                    foreach(string currentValue in tabListCompetitorAdvertiser ){
//                        if(currentValue==currentRow["id_advertiser"].ToString()){
//                            competitorElement=true;
//                            break;
//                        }		
//                    }
//                }
//                if(competitorElement){
//                    tabResult[i,FrameWorkConstantes.Results.PalmaresRecap.COMPETITOR]=2;
//                }

//                #endregion

//                if(typeYear==FrameWorkConstantes.Results.PalmaresRecap.typeYearSelected.currentYear){
////					tabResult[i,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N]=double.Parse(currentRow["total_N"].ToString())/1000;
//                    tabResult[i,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N]=double.Parse(currentRow["total_N"].ToString());
					
//                }
//                else{
//                    //					tabResult[i,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N]=double.Parse(currentRow["total_N1"].ToString())/1000;						
//                    tabResult[i,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N]=double.Parse(currentRow["total_N1"].ToString());
//                }
//                if(webSession.ComparativeStudy && typeYear==FrameWorkConstantes.Results.PalmaresRecap.typeYearSelected.currentYear){
//                    tabResult[i,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N1]=double.Parse(currentRow["total_N1"].ToString());
//                }
//                tabResult[i,FrameWorkConstantes.Results.PalmaresRecap.RANK]=rank;
//                if(typeYear==FrameWorkConstantes.Results.PalmaresRecap.typeYearSelected.currentYear){
//                    totalUniverse+=double.Parse(currentRow["total_N"].ToString());
//                }
//                else{
//                    totalUniverse+=double.Parse(currentRow["total_N1"].ToString());
//                }
//                i++;
//                rank++;

//            }			

//            if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.universTotal){
//                //				tabResult[0,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N]=totalUniverse/1000;
//                tabResult[0,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N]=totalUniverse;
//            }
//            else if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.marketTotal
//                || webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal
//                ){			
//                //				tabResult[0,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N]=TNS.AdExpress.Web.DataAccess.Results.IndicatorDataAccess.getTotalForPeriod(webSession,typeYear)/1000;
//                tabResult[0,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N]=double.Parse(DataAccess.Results.IndicatorDataAccess.getTotalForPeriod(webSession,typeYear).ToString());
//            }
			
//            tabResult[0,FrameWorkConstantes.Results.PalmaresRecap.SOV]=(double)100.00;
//            tabResult[0,FrameWorkConstantes.Results.PalmaresRecap.CUMUL_SOV]=(double)100.00;

//            for(i=1;i<nbline+1;i++){
				
//                if(webSession.ComparativeStudy && typeYear==FrameWorkConstantes.Results.PalmaresRecap.typeYearSelected.currentYear){
//                    oldRank=1;
//                    for(compteur=1;compteur<nbline;compteur++){
//                        if((double)tabResult[i,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N1]<(double)tabResult[compteur,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N1]
							
	
							
//                            ){
//                            oldRank=oldRank+1;
//                        }					
//                    }
					
//                    if((double)tabResult[i,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N1]!=0) {
//                        tabResult[i,FrameWorkConstantes.Results.PalmaresRecap.PROGRESS_RANK]=(double)oldRank-double.Parse(tabResult[i,FrameWorkConstantes.Results.PalmaresRecap.RANK].ToString());
//                    }
//                    else{
//                        tabResult[i,FrameWorkConstantes.Results.PalmaresRecap.PROGRESS_RANK]=null;
//                    }
//                }

//                tabResult[i,FrameWorkConstantes.Results.PalmaresRecap.SOV]=double.Parse(tabResult[i,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N].ToString())/double.Parse(tabResult[0,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N].ToString())*100;

//                cumul_Sov+=(double)tabResult[i,FrameWorkConstantes.Results.PalmaresRecap.SOV];
//                tabResult[i,FrameWorkConstantes.Results.PalmaresRecap.CUMUL_SOV]=cumul_Sov;
//            }

//            tabResult=GetTable(tabResult,finalTabResult);

//            return tabResult;
		
//        }
		
//        /// <summary>
//        /// fournit un tableau avec les 10 première lignes 
//        /// </summary>
//        /// <param name="firstTab">tableau avec toutes les lignes</param>
//        /// <param name="endTab">tableau avec 10 lignes</param>
//        /// <returns></returns>
//        protected static object [,] GetTable(object [,] firstTab,object [,] endTab ){
			
//            for(int i=0;i<11 & i<firstTab.GetLength(0); i++){
			
//                for(int j=0; j<firstTab.GetLength(1);j++){
//                    endTab[i,j]=firstTab[i,j];
				
//                }
			
//            }
//            return endTab;
		
//    }

//    }
//}
