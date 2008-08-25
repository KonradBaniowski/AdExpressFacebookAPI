//#region Information
//// Auteur: D�d� Mussuma
//// Cr�� le: 18/04/2006
//// Modifi�e le: 
//#endregion

//using System;
//using System.Data;

//using TNS.AdExpress.Web;
//using TNS.AdExpress.Web.Core.Sessions;
//using TNS.AdExpress.Domain.Translation;
//using WebConstantes = TNS.AdExpress.Constantes.Web;
//using FrameWorkConstantes = TNS.AdExpress.Constantes.FrameWork;
//using WebFunctions=TNS.AdExpress.Web.Functions;
//using WebExceptions=TNS.AdExpress.Web.Exceptions;
//using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
//namespace TNS.AdExpress.Web.Rules.Results
//{
//    /// <summary>
//    /// Traite les donn�es synth�tiques des indicateurs sectorielles.
//    /// </summary>
//    public class IndicatorSynthesisRules
//    {	
//        /// <summary>
//        /// Traite les donn�es de synth�se indicateurs.
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>tableau  de r�sultats des synth�ses indicatuers.</returns>
//        public static object[,] GetFormattedTable(WebSession webSession)
//        {			
//            object[,] tab = null;					

//            try{				

//                #region Chargement ders donn�es
//                //Donn�es Budget total s�lection
//                DataTable dtUniversTotal = DataAccess.Results.IndicatorSynthesisDataAccess.GetInvestmentData(webSession,WebConstantes.CustomerSessions.ComparisonCriterion.universTotal);
			
//                //Donn�es Budget total famille
//                DataTable dtSectorTotal = DataAccess.Results.IndicatorSynthesisDataAccess.GetInvestmentData(webSession,WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal);
			
//                //Donn�es Budget total march�
//                DataTable dtMarketTotal = DataAccess.Results.IndicatorSynthesisDataAccess.GetInvestmentData(webSession,WebConstantes.CustomerSessions.ComparisonCriterion.marketTotal);
			
//                //Donn�es nombre de produits actifs
//                DataTable dtNbRef = DataAccess.Results.IndicatorSynthesisDataAccess.GetProductNumberData(webSession,WebConstantes.CustomerSessions.ComparisonCriterion.universTotal,"id_product");
			
//                //Donn�es nombre d'annonceurs actifs
//                DataTable dtNbAdvert = DataAccess.Results.IndicatorSynthesisDataAccess.GetProductNumberData(webSession,WebConstantes.CustomerSessions.ComparisonCriterion.universTotal,"id_advertiser");
//                #endregion

//                #region Traitement des donn�es
//                if(hasData(dtUniversTotal,dtSectorTotal,dtMarketTotal,dtNbRef,dtNbAdvert)){
//                    //Tableau de donn�es
//                    tab = new object[10,5];

//                    #region Libell�s du tableau
//                    //Libell�s tableau			
//                    tab[0,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N_COLUMN_INDEX]=FctUtilities.Dates.getPeriodLabel(webSession,WebConstantes.CustomerSessions.Period.Type.currentYear);
				
//                    if(webSession.ComparativeStudy) {
//                        tab[0,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N1_COLUMN_INDEX]=FctUtilities.Dates.getPeriodLabel(webSession,WebConstantes.CustomerSessions.Period.Type.previousYear);									
//                        tab[0,FrameWorkConstantes.Results.SynthesisRecap.EVOLUTION_COLUMN_INDEX]=GestionWeb.GetWebWord(1207,webSession.SiteLanguage);					
//                        tab[0,FrameWorkConstantes.Results.SynthesisRecap.ECART_COLUMN_INDEX]=GestionWeb.GetWebWord(1213,webSession.SiteLanguage);
//                    }				
//                    tab[1,0]=GestionWeb.GetWebWord(1900,webSession.SiteLanguage);
//                    tab[2,0]=GestionWeb.GetWebWord(1901,webSession.SiteLanguage);
//                    tab[3,0]=GestionWeb.GetWebWord(1903,webSession.SiteLanguage);
//                    tab[4,0]=GestionWeb.GetWebWord(1902,webSession.SiteLanguage);
//                    tab[5,0]=GestionWeb.GetWebWord(1904,webSession.SiteLanguage);
//                    tab[6,0]=GestionWeb.GetWebWord(1905,webSession.SiteLanguage);
//                    tab[7,0]=GestionWeb.GetWebWord(1907,webSession.SiteLanguage);
//                    tab[8,0]=GestionWeb.GetWebWord(1906,webSession.SiteLanguage);
//                    tab[9,0]=GestionWeb.GetWebWord(1908,webSession.SiteLanguage);
//                    #endregion

//                    #region Calcul des donn�es
//                    //Budget Keuros Total s�lection
//                    ComputeBudget(tab,dtUniversTotal,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_UNIV_INVEST_LINE_INDEX,webSession.ComparativeStudy);

//                    //Budget Keuros Total famille					
//                    ComputeBudget(tab,dtSectorTotal,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_SECTOR_INVEST_LINE_INDEX,webSession.ComparativeStudy);

//                    //PDV s�lection vs Total famille
//                    ComputePDV(tab,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_UNIV_INVEST_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_SECTOR_INVEST_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.PDV_UNIV_TOTAL_SECTOR_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N_COLUMN_INDEX);//Ann�e N

//                    if(webSession.ComparativeStudy)
//                    ComputePDV(tab,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_UNIV_INVEST_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_SECTOR_INVEST_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.PDV_UNIV_TOTAL_SECTOR_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N1_COLUMN_INDEX);//Ann�e N-1
					
//                    //Budget Keuros Total march�					
//                    ComputeBudget(tab,dtMarketTotal,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_MARKET_INVEST_LINE_INDEX,webSession.ComparativeStudy);

//                    //PDV s�lection vs Total march�				
//                    ComputePDV(tab,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_UNIV_INVEST_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_MARKET_INVEST_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.PDV_UNIV_TOTAL_MARKET_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N_COLUMN_INDEX);//Ann�e N

//                    if(webSession.ComparativeStudy)
//                        ComputePDV(tab,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_UNIV_INVEST_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_MARKET_INVEST_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.PDV_UNIV_TOTAL_MARKET_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N1_COLUMN_INDEX);//Ann�e N-1
					 
//                    //Nb d'annonceurs actifs total s�lection
//                    if(!dtNbAdvert.Equals(System.DBNull.Value) && dtNbAdvert.Rows.Count>0){
//                        tab[FrameWorkConstantes.Results.SynthesisRecap.NB_ADVERTISER_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N_COLUMN_INDEX]=dtNbAdvert.Rows[0]["nbProduct"].ToString();//Ann�e N
//                        if(webSession.ComparativeStudy){
//                            tab[FrameWorkConstantes.Results.SynthesisRecap.NB_ADVERTISER_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N1_COLUMN_INDEX]=dtNbAdvert.Rows[1]["nbProduct"].ToString();//Ann�e N-1
//                            //Evolution et ecartanne� N par rapport N-1 
//                            ComputeEvolAndEcart(tab,FrameWorkConstantes.Results.SynthesisRecap.NB_ADVERTISER_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N_COLUMN_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N1_COLUMN_INDEX,FrameWorkConstantes.Results.SynthesisRecap.EVOLUTION_COLUMN_INDEX,FrameWorkConstantes.Results.SynthesisRecap.ECART_COLUMN_INDEX);
//                        }
//                    }

//                    //Budget moyen par annonceurs total s�lection
//                    ComputeAverageBudget(tab,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_UNIV_INVEST_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.NB_ADVERTISER_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.AVERAGE_INVEST_BY_ADVERTISER_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N_COLUMN_INDEX);//Ann�e N

//                    if(webSession.ComparativeStudy ){												
//                        ComputeAverageBudget(tab,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_UNIV_INVEST_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.NB_ADVERTISER_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.AVERAGE_INVEST_BY_ADVERTISER_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N1_COLUMN_INDEX);//Ann�e N-1
//                        //Evolution et ecartanne� N par rapport N-1 
//                        ComputeEvolAndEcart(tab,FrameWorkConstantes.Results.SynthesisRecap.AVERAGE_INVEST_BY_ADVERTISER_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N_COLUMN_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N1_COLUMN_INDEX,FrameWorkConstantes.Results.SynthesisRecap.EVOLUTION_COLUMN_INDEX,FrameWorkConstantes.Results.SynthesisRecap.ECART_COLUMN_INDEX);
//                    }

//                    //Nb de produits actifs total s�lection
//                    if(!dtNbRef.Equals(System.DBNull.Value) && dtNbRef.Rows.Count>0){
//                        tab[FrameWorkConstantes.Results.SynthesisRecap.NB_PRODUCT_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N_COLUMN_INDEX]=dtNbRef.Rows[0]["nbProduct"].ToString();//Ann�e N
//                        if(webSession.ComparativeStudy){
//                            tab[FrameWorkConstantes.Results.SynthesisRecap.NB_PRODUCT_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N1_COLUMN_INDEX]=dtNbRef.Rows[1]["nbProduct"].ToString();//Ann�e N-1
//                            //Evolution et ecartanne� N par rapport N-1 
//                            ComputeEvolAndEcart(tab,FrameWorkConstantes.Results.SynthesisRecap.NB_PRODUCT_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N_COLUMN_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N1_COLUMN_INDEX,FrameWorkConstantes.Results.SynthesisRecap.EVOLUTION_COLUMN_INDEX,FrameWorkConstantes.Results.SynthesisRecap.ECART_COLUMN_INDEX);
//                        }
//                    }

//                    //Budget moyen par produit total s�lection
//                    ComputeAverageBudget(tab,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_UNIV_INVEST_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.NB_PRODUCT_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.AVERAGE_INVEST_BY_PRODUCT_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N_COLUMN_INDEX);//Ann�e N

//                    if(webSession.ComparativeStudy ){												
//                        ComputeAverageBudget(tab,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_UNIV_INVEST_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.NB_PRODUCT_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.AVERAGE_INVEST_BY_PRODUCT_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N1_COLUMN_INDEX);//Ann�e N-1
//                        //Evolution et ecartanne� N par rapport N-1 
//                        ComputeEvolAndEcart(tab,FrameWorkConstantes.Results.SynthesisRecap.AVERAGE_INVEST_BY_PRODUCT_LINE_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N_COLUMN_INDEX,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N1_COLUMN_INDEX,FrameWorkConstantes.Results.SynthesisRecap.EVOLUTION_COLUMN_INDEX,FrameWorkConstantes.Results.SynthesisRecap.ECART_COLUMN_INDEX);
//                    }


//                    #endregion
//                }		
//                #endregion

//            }catch(Exception ex){
//                throw new WebExceptions.IndicatorSynthesisRulesException(" Impossible de r�aliser la synth�se des indicateurs.",ex);
//            }

//            return tab;
//        }


//        #region M�thodes internes
//        /// <summary>
//        /// Calcule le budget en fonction de l'univers
//        /// </summary>
//        /// <param name="tab">tableau de r�sultats</param>
//        /// <param name="dt">table d donn�es</param>
//        /// <param name="lineIndex">index ligne du tableau</param>
//        /// <param name="comparativeStudy">Vrai si �tude comparative</param>
//        private static void ComputeBudget(object[,] tab,DataTable dt,int lineIndex,bool comparativeStudy){
//            double tempEvol=0;
//            if(!dt.Equals(System.DBNull.Value) && dt.Rows.Count>0){
//                tab[lineIndex,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N_COLUMN_INDEX]=dt.Rows[0]["total_N"].ToString();//Ann�e N
//                if(comparativeStudy){
//                    tab[lineIndex,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N1_COLUMN_INDEX]=dt.Rows[0]["total_N1"].ToString();//Ann�e N-1
//                    if(dt.Rows[0]["evol"].Equals(System.DBNull.Value) ) {
//                        tempEvol=((double.Parse(tab[lineIndex,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N_COLUMN_INDEX].ToString())-double.Parse(tab[lineIndex,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N1_COLUMN_INDEX].ToString()))*(double)100.0)/double.Parse(tab[lineIndex,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N1_COLUMN_INDEX].ToString());																
//                        tab[lineIndex,FrameWorkConstantes.Results.SynthesisRecap.EVOLUTION_COLUMN_INDEX] =tempEvol;
//                    }else tab[lineIndex,FrameWorkConstantes.Results.SynthesisRecap.EVOLUTION_COLUMN_INDEX]=dt.Rows[0]["evol"].ToString();//evolution
//                    tab[lineIndex,FrameWorkConstantes.Results.SynthesisRecap.ECART_COLUMN_INDEX]=dt.Rows[0]["ecart"].ToString();//ecart
//                }
//            }
//        }
		
//        /// <summary>
//        /// Calcule le budget Moyen en fonction de l'univers par produit ou annonceur.
//        /// </summary>
//        /// <param name="tab">tableau de r�sultats</param>
//        /// <param name="totalInvestLineIndex">index ligne total investissement</param>
//        /// <param name="nbAdvertLineIndex">index ligne nombre d'�l�ments</param>
//        /// <param name="averageInvestLineIndex">index ligne budget moyen</param>
//        /// <param name="columnLineIndex">index colonne courante</param>
//        private static void ComputeAverageBudget(object[,] tab,int totalInvestLineIndex,int nbAdvertLineIndex,int averageInvestLineIndex,int columnLineIndex){
//            double tempBudgetMoyen=0;
//            if(tab[totalInvestLineIndex,columnLineIndex]!=null
//                && tab[nbAdvertLineIndex,columnLineIndex]!=null
//                && double.Parse(tab[nbAdvertLineIndex,columnLineIndex].ToString())>0){
//                tempBudgetMoyen  = double.Parse(tab[totalInvestLineIndex,columnLineIndex].ToString())/double.Parse(tab[nbAdvertLineIndex,columnLineIndex].ToString());
//                tab[averageInvestLineIndex,columnLineIndex]=tempBudgetMoyen;
//            }
//        }
//        /// <summary>
//        /// Calcule du PDV
//        /// </summary>
//        /// <param name="tab">tableau de r�sultats</param>
//        /// <param name="childLineIndex">index line sous total</param>
//        /// <param name="parentLineIndex">index line total</param>
//        /// <param name="pdvLineIndex">index line pdv</param>
//        /// <param name="columnIndex">index colonne</param>
//        private static void ComputePDV(object[,] tab,int childLineIndex,int parentLineIndex,int pdvLineIndex,int columnIndex){
//            double tempPDV=0;
//            if(tab[childLineIndex,columnIndex]!=null 
//                && tab[parentLineIndex,columnIndex]!=null
//                && double.Parse(tab[parentLineIndex,columnIndex].ToString())>0){
//                tempPDV = (double.Parse(tab[childLineIndex,columnIndex].ToString())*(double)100.0)/double.Parse(tab[parentLineIndex,columnIndex].ToString());
//                tab[pdvLineIndex,columnIndex]=tempPDV.ToString();
//            }
//        }
		
//        /// <summary>
//        /// Calcule Evolution et ecart  ann�e N par rapport � ann�e N-1
//        /// </summary>
//        /// <param name="tab">tableau de r�sultats</param>
//        /// <param name="lineIndex">index ligne courante</param>
//        /// <param name="total_N_Column">index colonne ann�e N</param>
//        /// <param name="total_N1_Column">index colonne ann�e N-1</param>
//        /// <param name="evolColumn">index colonne evolution</param>
//        /// <param name="ecartColumn">index colonne ecart</param>
//        private static void ComputeEvolAndEcart(object[,] tab,int lineIndex,int total_N_Column,int total_N1_Column,int evolColumn,int ecartColumn){
//            //Evolution anne� N par rapport N-1 = ((N-(N-1))*100)/N-1 
//            double tempEvol =(double)0.0;
//            double tempEcart = (double)0.0;
//            if(tab[lineIndex,total_N_Column]!=null && tab[lineIndex,total_N1_Column]!=null){									
//                tempEvol=((double.Parse(tab[lineIndex,total_N_Column].ToString())-double.Parse(tab[lineIndex,total_N1_Column].ToString()))*(double)100.0)/double.Parse(tab[lineIndex,total_N1_Column].ToString());																
//                tab[lineIndex,evolColumn]=tempEvol;//evolution
//            }
			
//            //Ecart anne� N - rapport N-1				
//            if(tab[lineIndex,total_N_Column]!=null 
//                && tab[lineIndex,total_N1_Column]!=null 
//                ){																	
//                tempEcart  = double.Parse(tab[lineIndex,total_N_Column].ToString()) - double.Parse(tab[lineIndex,total_N1_Column].ToString());
//                tab[lineIndex,ecartColumn]=tempEcart.ToString();//ecart
//            }
//        }

//        /// <summary>
//        /// V�rifie qu'il ya des donn�es � traiter.
//        /// </summary>
//        /// <param name="dtUniversTotal">Donn�es total s�lection</param>
//        /// <param name="dtSectorTotal">Donn�es total famille</param>
//        /// <param name="dtMarketTotal">Donn�es total march�</param>
//        /// <param name="dtNbRef">Donn�es nombre de r�f�rences</param>
//        /// <param name="dtNbAdvert">Donn�es nombre d'annonceurs</param>
//        /// <returns>Vrai si pr�senece de donn�es.</returns>
//        private static bool hasData(DataTable dtUniversTotal,DataTable dtSectorTotal,DataTable dtMarketTotal,DataTable dtNbRef,DataTable dtNbAdvert){
//            if((!dtUniversTotal.Equals(System.DBNull.Value) && dtUniversTotal.Rows.Count>0)
//                || (!dtSectorTotal.Equals(System.DBNull.Value) && dtSectorTotal.Rows.Count>0)
//                || (!dtMarketTotal.Equals(System.DBNull.Value) && dtMarketTotal.Rows.Count>0)
//                || (!dtNbRef.Equals(System.DBNull.Value) && dtNbRef.Rows.Count>0)
//                || (!dtNbAdvert.Equals(System.DBNull.Value) && dtNbAdvert.Rows.Count>0)
//                )return true;
//            else return false;
			
//        }
//        #endregion
//    }
//}
