//#region Informations
//// Auteur: G. Facon 
//// Date de création: 28/09/2004 
//// Date de modification:28/09/2004 
////	12/08/2005	A.Dadouch	Nom de fonctions
////	01/12/2006	G. Facon	Résultats génériques et niveaux génériques	
//#endregion

//#region Using
//using System;
//using System.Data;
//using System.Collections;
//using System.Windows.Forms;
//using TNS.AdExpress.Web.Core.Sessions;
//using WebCommon=TNS.AdExpress.Web.Common;
//using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
//using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
//using WebExceptions=TNS.AdExpress.Web.Exceptions;
//using TNS.FrameWork.WebResultUI;
//using ComptitorResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results.CompetitorAlert;
//using CustomerConstantes=TNS.AdExpress.Constantes.Customer;
//#endregion

//namespace TNS.AdExpress.Web.Rules.Results{
//    /// <summary>
//    /// Classe métier des modules potentiels
//    /// </summary>
//    /// <remarks>
//    /// Classe s'appuie sur la classe metier des modules concurrentiels
//    /// </remarks>
//    public class MarketShareRules{

//        #region PorteFeuille

//        /// <summary>
//        /// Obtient le tableau de résultat du portefeuille d'une alerte Concurrentielle
//        /// </summary>
//        /// <param name="webSession">Session du client</param>		
//        /// <returns>Tableau de résultat</returns>
//        internal static ResultTable GetPortofolioFormattedTable(WebSession webSession){
//            try{
//                return(CompetitorRules.GetData(webSession));
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.MarketShareRulesException("Impossible d'appliquer les régles métiers d'un module potentiels",err));
//            }
//        }

//        #endregion

//        #region Forces
		
//        /// <summary>
//        /// Obtient le tableau de résultat du portefeuille d'une alerte Concurrentielle
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="computeStrenghs">Calcule les forces</param>
//        /// <returns>Tableau de résultat</returns>
//        internal static ResultTable GetStrenghsOrOpportunitiesFormattedTable(WebSession webSession,bool computeStrenghs){

//            #region Variables
//            long i;
//            WebCommon.Results.SelectionSubGroup[] subGroupMediaTotalIndex = new WebCommon.Results.SelectionSubGroup[100];
//            #endregion

//            #region Chargement du tableau de résultat
//            ResultTable tabData=null;
//            try{
//                tabData = CompetitorRules.GetData(webSession, ref subGroupMediaTotalIndex);
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.MarketShareRulesException("Impossible d'obtenir le tableau préformaté: ",err));
//            }
//            #endregion

//            if(tabData==null){
//                return null;
//            }

//            #region Indexes de comparaison
//            long comparaisonIndexInTabResult ;
//            if(tabData.HeadersIndexInResultTable.ContainsKey((ComptitorResultConstantes.START_ID_GROUP+1)+"-"+ComptitorResultConstantes.SUB_TOTAL_HEADER_ID)){
//                comparaisonIndexInTabResult = tabData.GetHeadersIndexInResultTable((ComptitorResultConstantes.START_ID_GROUP+1)+"-"+ComptitorResultConstantes.SUB_TOTAL_HEADER_ID);
//            }
//            else{
//                //comparaisonIndexInTabResult = tabData.GetHeadersIndexInResultTable((ComptitorResultConstantes.START_ID_GROUP+1)+"-"+webSession.GetSelection((TreeNode) webSession.CompetitorUniversMedia[1],CustomerConstantes.Right.type.mediaAccess));
//                comparaisonIndexInTabResult = tabData.GetHeadersIndexInResultTable((ComptitorResultConstantes.START_ID_GROUP + 1) + "-" + subGroupMediaTotalIndex[2].DataBaseId.ToString());
//            }
//            long levelLabelColIndex=tabData.GetHeadersIndexInResultTable(ComptitorResultConstantes.LEVEL_HEADER_ID.ToString());
//            #endregion

//            #region Détermine le niveau de détail Produit
//            int NbLevels=webSession.GenericProductDetailLevel.GetNbLevels;
//            #endregion

//            #region Traitement des données
//            try{
//                // Supprime les lignes qui ont un total support de références média inférieur
//                // au total univers des supports de référence.
//                long[] nbLevelToShow={0,0,0,0};
//                CellLevel curLevel = null;
//                for(i=tabData.LinesNumber-1;i>=0;i--){
//                    curLevel = (CellLevel)tabData[i,levelLabelColIndex];
//                    if(curLevel.Level==NbLevels){
//                        if(tabData[i,comparaisonIndexInTabResult]!=null && ((CellUnit)tabData[i,comparaisonIndexInTabResult]).Value!=double.NaN && 
//                            ( (computeStrenghs &&((CellUnit)tabData[i,comparaisonIndexInTabResult]).Value<((CellUnit)tabData[0,comparaisonIndexInTabResult]).Value)||
//                            (!computeStrenghs &&((CellUnit)tabData[i,comparaisonIndexInTabResult]).Value>((CellUnit)tabData[0,comparaisonIndexInTabResult]).Value))){
//                            tabData.SetLineStart(new LineHide(tabData.GetLineStart(i).LineType),i);
//                        }
//                        else{
//                            nbLevelToShow[NbLevels]++;
//                        }
//                    }
//                    else{
//                        if (nbLevelToShow[curLevel.Level+1] > 0){
//                            nbLevelToShow[curLevel.Level]++;
//                        }
//                        else{
//                            tabData.SetLineStart(new LineHide(tabData.GetLineStart(i).LineType),i);
//                        }
//                        for (int j = curLevel.Level+1; j < nbLevelToShow.Length; j++){
//                            nbLevelToShow[j] = 0;
//                        }
//                    }
//                }
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.MarketShareRulesException("Impossible de construire le tableau de résultat d'un module potentiels: ",err));
//            }
//            #endregion

//            return(tabData);
//            //throw(new System.NotImplementedException());
//        }
//        #endregion

//        #region Potentiels

//        /// <summary>
//        /// Obtient le tableau de résultat du portefeuille d'une alerte Concurrentielle
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Tableau de résultat</returns>
//        internal static object[,] GetOpportunitiesFormattedTable(WebSession webSession){
		
////			#region Variables
////			long i;
////			long oldL1=-1;
////			long oldL2=-1;
////			//long oldL3=-1;
////			//long currentL1=-1;
////			//long currentL2=-1;
////			//long currentL3=-1;
////			//bool findL3=false;
////			//bool findL2=false;
////			//bool findL1=false;
////			//bool L1=true;
////			bool L2=false;
////			bool L3=false;
////			long k;
////
////			long nbrNotLineL1=0;
////			long nbrNotLineL2=0;
////			bool lastL2=false;
////			bool lastL3=false;
////			#endregion
////
////			#region Tableaux d'index
////			WebCommon.Results.SelectionGroup[] groupMediaTotalIndex=new WebCommon.Results.SelectionGroup[6];
////			int nbUnivers=0;
////			Hashtable mediaIndex=new Hashtable();
////			string mediaListForLabelSearch="";
////			int maxIndex=0;
////			try{
////				CompetitorRules.InitIndexAndValues(webSession,groupMediaTotalIndex,ref nbUnivers,mediaIndex,ref mediaListForLabelSearch,ref maxIndex);
////			}
////			catch(System.Exception err){
////				throw(new WebExceptions.MarketShareRulesException("Impossible de définir les indexes d'un module potentiels: ",err));
////			}
////			#endregion
////
////			#region Chargement du tableau de résultat
////			object[,] tabData=null;
////			try{
////				tabData=CompetitorRules.GetData(webSession);
////			}
////			catch(System.Exception err){
////				throw(new WebExceptions.MarketShareRulesException("Impossible d'obtenir le tableau préformaté: ",err));
////			}
////			#endregion
////
////			if(tabData==null){
////				return null;
////			}
////
////			#region Indexes de comparaison
////			int comparaisonIndexInTabResult=groupMediaTotalIndex[1].IndexInResultTable;
////			#endregion
////
////			#region Détermine le niveau de détail Produit
////			int columnTestIndex=-1;
////			oldL1=FrameWorkResultConstantes.CompetitorAlert.FIRST_LINE_RESULT_INDEX;
////			i=FrameWorkResultConstantes.CompetitorAlert.FIRST_LINE_RESULT_INDEX+1;
////			try{
////				while(oldL1==FrameWorkResultConstantes.CompetitorAlert.FIRST_LINE_RESULT_INDEX && oldL1<tabData.GetLongLength(1)
////					&& i<tabData.GetLongLength(1)){
////					if(tabData[FrameWorkResultConstantes.CompetitorAlert.IDL3_INDEX,i]!=null) L3=true;
////					if(tabData[FrameWorkResultConstantes.CompetitorAlert.IDL2_INDEX,i]!=null) L2=true;
////					if(tabData[FrameWorkResultConstantes.CompetitorAlert.IDL1_INDEX,i]!=null) oldL1=i;
////					i++;
////				}
////				if(L3)columnTestIndex=FrameWorkResultConstantes.CompetitorAlert.IDL3_INDEX;
////				else if(L2)columnTestIndex=FrameWorkResultConstantes.CompetitorAlert.IDL2_INDEX;
////				else columnTestIndex=FrameWorkResultConstantes.CompetitorAlert.IDL1_INDEX;
////			}
////			catch(System.Exception err){
////				throw(new WebExceptions.MarketShareRulesException("Impossible de détermine le niveau de détail Produit: ",err));
////			}
////			#endregion
////
////			#region Traitement des données
////			// Supprime les lignes qui ont un total support de références média inférieur
////			// au total univers des supports de référence.
////			try{
////				for(i=FrameWorkResultConstantes.CompetitorAlert.FIRST_LINE_RESULT_INDEX;i<tabData.GetLongLength(1);i++){
////					if(tabData[columnTestIndex,i]!=null){
////						if(tabData[comparaisonIndexInTabResult,i]!=null && (double)tabData[comparaisonIndexInTabResult,i]!=double.NaN && (double)tabData[comparaisonIndexInTabResult,i]>(double)tabData[comparaisonIndexInTabResult,FrameWorkResultConstantes.CompetitorAlert.TOTAL_LINE_INDEX]
////						
////							){
////							for(k=0;k<FrameWorkResultConstantes.CompetitorAlert.TOTAL_INDEX;k++){
////								tabData[k,i]=new FrameWorkConstantes.MemoryArrayNotShowLine();
////							}
////						}
////					}
////				}
////				// On retraite le tableau pour en lever les niveau hierarchiques suppérieurs qui non pas d'enfant
////				for(i=FrameWorkResultConstantes.CompetitorAlert.FIRST_LINE_RESULT_INDEX;i<tabData.GetLongLength(1);i++){
////			
////					#region Niveau L1
////					if(tabData[FrameWorkResultConstantes.CompetitorAlert.IDL1_INDEX,i]!=null && L2 && tabData[FrameWorkResultConstantes.CompetitorAlert.IDL1_INDEX,i].GetType()!=typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
////						if(oldL1+1+nbrNotLineL1==i){
////							for(k=0;k<FrameWorkResultConstantes.CompetitorAlert.TOTAL_INDEX;k++){
////								tabData[k,oldL1]=new FrameWorkConstantes.MemoryArrayNotShowLine();
////							}
////						}
////						oldL1=i;
////						nbrNotLineL1=0;
////						nbrNotLineL2++;
////					
////					}else if(tabData[FrameWorkResultConstantes.CompetitorAlert.IDL1_INDEX,i]!=null && L2 && !L3 && tabData[FrameWorkResultConstantes.CompetitorAlert.IDL1_INDEX,i].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
////						nbrNotLineL1++;
////					}
////					#endregion
////
////					#region Niveau L2
////					if(tabData[FrameWorkResultConstantes.CompetitorAlert.IDL2_INDEX,i]!=null && L3 && tabData[FrameWorkResultConstantes.CompetitorAlert.IDL2_INDEX,i].GetType()!=typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
////						if(oldL2+1+nbrNotLineL2==i){
////							for(k=0;k<FrameWorkResultConstantes.CompetitorAlert.TOTAL_INDEX;k++){
////								tabData[k,oldL2]=new FrameWorkConstantes.MemoryArrayNotShowLine();
////							}
////						}
////						oldL2=i;
////						nbrNotLineL2=0;
////						nbrNotLineL1++;
////					}else if(tabData[FrameWorkResultConstantes.CompetitorAlert.IDL2_INDEX,i]!=null && L3 && tabData[FrameWorkResultConstantes.CompetitorAlert.IDL2_INDEX,i].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){	
////						nbrNotLineL1++;
////					}
////					if(tabData[FrameWorkResultConstantes.CompetitorAlert.IDL3_INDEX,i]!=null && L3 && tabData[FrameWorkResultConstantes.CompetitorAlert.IDL3_INDEX,i].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
////						nbrNotLineL2++;
////					}
////					#endregion
////				}
////
////				#region cas de la dernière ligne
////			
////				#region Niveau L1
////				if(L2){
////					lastL2=false;
////					for(i=oldL1;i<tabData.GetLongLength(1)&& !lastL2;i++){	 
////						if(tabData[FrameWorkResultConstantes.CompetitorAlert.IDL2_INDEX,i]!=null && tabData[FrameWorkResultConstantes.CompetitorAlert.IDL2_INDEX,i].GetType()!=typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
////							lastL2=true;
////						}
////					}
////					if(!lastL2){
////						for(k=0;k<FrameWorkResultConstantes.CompetitorAlert.TOTAL_INDEX;k++){
////							tabData[k,oldL1]=new FrameWorkConstantes.MemoryArrayNotShowLine();
////						}
////					}
////				}
////				#endregion
////
////				#region Niveau L2
////				if(L3){
////					lastL3=false;
////					for(i=oldL2;i<tabData.GetLongLength(1)&&!lastL3;i++){
////						if(tabData[FrameWorkResultConstantes.CompetitorAlert.IDL3_INDEX,i]!=null && tabData[FrameWorkResultConstantes.CompetitorAlert.IDL3_INDEX,i].GetType()!=typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
////							//&& !Object.ReferenceEquals(tabData[FrameWorkResultConstantes.CompetitorAlert.IDL3_INDEX,i].GetType(),typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine))
////							lastL3=true;
////						}
////					}
////					if(!lastL3){
////						for(k=0;k<FrameWorkResultConstantes.CompetitorAlert.TOTAL_INDEX;k++){
////							tabData[k,oldL2]=new FrameWorkConstantes.MemoryArrayNotShowLine();
////						}
////					}
////				}
////				#endregion
////
////				#endregion
////			}
////			catch(System.Exception err){
////				throw(new WebExceptions.MarketShareRulesException("Impossible de construire le tableau de résultat d'un module potentiels: ",err));
////			}
////			#endregion
////						
////			return(tabData);
//            throw(new System.NotImplementedException());
//        }
//        #endregion

//    }
//}
