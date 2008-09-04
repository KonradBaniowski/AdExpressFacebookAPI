//#region Informations
//// Auteur: A. Obermeyer 
//// Date de création : 21/10/2004 
//// Date de modification : 21/10/2004 
////	12/08/2005	A.Dadouch	Nom de fonctions	
////	25/10/2005	D. V. Mussuma	Intégration unité Keuros	
//#endregion

//using System;
//using System.Data;
//using System.Collections;
//using System.Windows.Forms;

//using TNS.AdExpress.Web.Core.Sessions;
//using TNS.AdExpress.Web.DataAccess.Results;
//using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
//using WebConstantes = TNS.AdExpress.Constantes.Web;
//using TNS.Classification.Universe;
//using TNS.AdExpress.Classification;

//namespace TNS.AdExpress.Web.Rules.Results {
//    /// <summary>
//    /// Indicateur Evolution
//    /// </summary>
//    public class IndicatorEvolutionRules{

//        #region Constructeur
//        /// <summary>
//        /// Constructeur
//        /// </summary>
//        public IndicatorEvolutionRules(){
			
//        }
//        #endregion

//        /// <summary>
//        /// Retourne l'objet contenant les infos pour un tableau evolution
//        /// </summary>
//        /// <param name="webSession">session du client</param>
//        /// <param name="tableType">type de tableau</param>
//        /// <returns></returns>
//        public static object[,] GetFormattedTable(WebSession webSession,FrameWorkConstantes.Results.EvolutionRecap.ElementType tableType ){
			
			
//            #region Variables
//            long i=0;
//            string [] tabListCompetitorAdvertiser=null;
//            string [] tabListReferenceAdvertiser=null;
//            bool competitorElement;
//            NomenclatureElementsGroup nomenclatureElementsGroup = null;
//            string tempListAsString = "";

//            #endregion
			
//            #region Chargement des données à partir de la base	
//            DataSet ds;
//            ds=IndicatorDataAccess.GetEvolutionData(webSession,tableType);
//            DataTable dt=ds.Tables[0];			
//            #endregion

//            //if(webSession.CompetitorUniversAdvertiser[0]!=null){
//            //    tabListCompetitorAdvertiser=webSession.GetSelection((TreeNode)webSession.CompetitorUniversAdvertiser[0],TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess).Split(',');			
//            //}
//            //tabListReferenceAdvertiser=webSession.GetSelection(webSession.ReferenceUniversAdvertiser,TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess).Split(',');

//            if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(1)) {
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
//            #region Déclaration du tableau de résultat
//            long nbline=dt.Rows.Count;
//            object[,] tabResult=new object[nbline,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR+1];
//            #endregion

//            foreach(DataRow currentRow in dt.Rows){
			
//                #region Nom
//                if(tableType==FrameWorkConstantes.Results.EvolutionRecap.ElementType.product){
//                    tabResult[i,FrameWorkConstantes.Results.EvolutionRecap.ID_PRODUCT_INDEX]=double.Parse(currentRow["id_product"].ToString());
//                    tabResult[i,FrameWorkConstantes.Results.EvolutionRecap.PRODUCT]=currentRow["product"].ToString();

//                }else if(tableType==FrameWorkConstantes.Results.EvolutionRecap.ElementType.advertiser){
//                    tabResult[i,FrameWorkConstantes.Results.EvolutionRecap.ID_PRODUCT_INDEX]=double.Parse(currentRow["id_advertiser"].ToString());
//                    tabResult[i,FrameWorkConstantes.Results.EvolutionRecap.PRODUCT]=currentRow["advertiser"].ToString();
//                }
//                #endregion

//                #region Total				
////				tabResult[i,FrameWorkConstantes.Results.EvolutionRecap.TOTAL_N]=double.Parse(currentRow["total_N"].ToString())/1000;				
//                tabResult[i,FrameWorkConstantes.Results.EvolutionRecap.TOTAL_N]=double.Parse(currentRow["total_N"].ToString());
//                #endregion


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
//                    tabResult[i,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]=1;
//                }
//                else{
//                    tabResult[i,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]=0;
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
//                    tabResult[i,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]=2;
//                }

//                #endregion

				

//                #region Ecart
////				tabResult[i,FrameWorkConstantes.Results.EvolutionRecap.ECART]=double.Parse(currentRow["Ecart"].ToString())/1000;	
//                tabResult[i,FrameWorkConstantes.Results.EvolutionRecap.ECART]=double.Parse(currentRow["Ecart"].ToString());	
//                #endregion

//                #region Evolution

//                tabResult[i,FrameWorkConstantes.Results.EvolutionRecap.EVOLUTION]=(double)tabResult[i,FrameWorkConstantes.Results.EvolutionRecap.ECART]/((double)tabResult[i,FrameWorkConstantes.Results.EvolutionRecap.TOTAL_N]-(double)tabResult[i,FrameWorkConstantes.Results.EvolutionRecap.ECART])*100;

//                #endregion


//                i++;
//            }


//            return tabResult;
//        }


//    }
//}
