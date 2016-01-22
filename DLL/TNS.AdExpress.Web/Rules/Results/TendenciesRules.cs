//#region Informations
//// Auteur: A. Obermeyer 
//// Date de création: 07/02/2005 
//// Date de modification
//#endregion

//using System;
//using System.Data;
//using TNS.AdExpress.Web.Core.Sessions;
//using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
//using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;

//namespace TNS.AdExpress.Web.Rules.Results{

//    /// <summary>
//    /// Description résumée de TendenciesRules.
//    /// </summary>
//    public class TendenciesRules{
		
//        /// <summary>
//        /// Fournit le tableau pour le module tendance
//        /// </summary>
//        /// <param name="webSession">Session Client</param>
//        /// <param name="vehicleName">Nom du Vehicle</param>
//        /// <returns>tableau</returns>
//        public static object[,] GetTendenciesFormattedTable(WebSession webSession,DBClassificationConstantes.Vehicles.names vehicleName){
			
//            #region Variables
//            Int64 idOldCategory=-1;
//            Int64 idOldMedia=-1;
//            //Int64 idCategory=-1;
//            int nbCategory=0;
//            int nbMedia=0;
//            int start=-1;
//            int startMedia=-1;
//            // Nombre de colonnes
//            int nbCol=0;
//            decimal temp=0;
			
//            int currentLineCategory=0;
//            int currentLineMedia=0;
//            int currentLine=1;
//            int k=0;	
//            #endregion
		
//            int firstPeriod=int.Parse(webSession.PeriodBeginningDate);
//            int endPeriod=int.Parse(webSession.PeriodEndDate);
				
//            DataSet ds;

//            ds=TNS.AdExpress.Web.DataAccess.Results.TendenciesDataAccess.GetData(webSession,vehicleName);
//            DataTable dt=ds.Tables[0];

//            #region Nombre de Catégorie
//            // Compte le nombre de catégorie
//            foreach(DataRow currentRow in dt.Rows){
//                if(idOldCategory!=(Int64)currentRow["id_category"]){
//                    nbCategory++;
//                    idOldCategory=(Int64)currentRow["id_category"];
//                }
//                if(idOldMedia!=(Int64)currentRow["id_media"]){					
//                    nbMedia++;
//                    idOldMedia=(Int64)currentRow["id_media"];

//                }
//            }
//            #endregion

//            int[] indexCategory=new int[nbCategory];
//            int idCategory=0;

//            #region Déclaration du tableau
//            switch(vehicleName){
//                case DBClassificationConstantes.Vehicles.names.press:
//                case DBClassificationConstantes.Vehicles.names.internationalPress:
//                    nbCol=FrameWorkConstantes.Tendencies.NBRE_COLUMN_PRESS;
//                    break;
//                case DBClassificationConstantes.Vehicles.names.radio:					
//                case DBClassificationConstantes.Vehicles.names.tv:
//                case DBClassificationConstantes.Vehicles.names.others:	
//                    nbCol=FrameWorkConstantes.Tendencies.NBRE_COLUMN_RADIO_TV;
//                    break;
//                default:
//                    break;
//            }

//            object[,] tab=new object[nbMedia+1+nbCategory,nbCol];
//            #endregion

//            idOldCategory=-1;

//            #region Parcours du tableau
//            // Parcours du tableau

//            InitializeTable(ref tab,FrameWorkConstantes.Tendencies.TOTAL_LINE,vehicleName);
//            for(k=0;k<dt.Rows.Count;k++){
							

//                // Total
//                if( firstPeriod<=(int)dt.Rows[k]["period"] && endPeriod>=(int)dt.Rows[k]["period"]){
					
//                    #region Investissement N
//                    if(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INVEST_N]!=null)
//                        temp=(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INVEST_N];
//                    else
//                        temp=0;

//                    tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INVEST_N]=(decimal)dt.Rows[k]["euro"]+temp;
//                    #endregion

//                    #region Pages N/duree N
					
//                    switch(vehicleName){
//                        case DBClassificationConstantes.Vehicles.names.press:
//                        case DBClassificationConstantes.Vehicles.names.internationalPress:
//                            if(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.SURFACE_N]!=null)
//                                temp=(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.SURFACE_N];
//                            else
//                                temp=0;

//                            tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.SURFACE_N]=(decimal)dt.Rows[k]["pages"]+temp;
//                            break;
//                        case DBClassificationConstantes.Vehicles.names.radio:					
//                        case DBClassificationConstantes.Vehicles.names.tv:
//                        case DBClassificationConstantes.Vehicles.names.others:	
//                            if(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.DURATION_N]!=null)
//                                temp=(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.DURATION_N];
//                            else
//                                temp=0;
//                            tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.DURATION_N]=(decimal)dt.Rows[k]["duration"]+temp;
//                            break;
//                        default:
//                            break;
//                    }


				
//                    #endregion

//                    #region Insertion N
//                    if(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INSERTION_N]!=null)
//                        temp=(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INSERTION_N];
//                    else
//                        temp=0;

//                    tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INSERTION_N]=(decimal)dt.Rows[k]["insertion"]+temp;
//                    #endregion
//                }
//                else{

//                    #region Investissement N-1
//                    if(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INVEST_N1]!=null)
//                        temp=(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INVEST_N1];
//                    else
//                        temp=0;

//                    tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INVEST_N1]=(decimal)dt.Rows[k]["euro"]+temp;
//                    #endregion

//                    #region Pages N-1 /duree
			
//                    switch(vehicleName){
//                        case DBClassificationConstantes.Vehicles.names.press:
//                        case DBClassificationConstantes.Vehicles.names.internationalPress:
//                            if(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.SURFACE_N1]!=null)
//                                temp=(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.SURFACE_N1];
//                            else
//                                temp=0;

//                            tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.SURFACE_N1]=(decimal)dt.Rows[k]["pages"]+temp;
//                            break;
//                        case DBClassificationConstantes.Vehicles.names.radio:					
//                        case DBClassificationConstantes.Vehicles.names.tv:
//                        case DBClassificationConstantes.Vehicles.names.others:
//                            if(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.DURATION_N1]!=null)
//                                temp=(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.DURATION_N1];
//                            else
//                                temp=0;

//                            tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.DURATION_N1]=(decimal)dt.Rows[k]["duration"]+temp;
//                            break;
//                        default:
//                            break;
//                    }				
//                    #endregion

//                    #region Insertion N-1
//                    if(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INSERTION_N1]!=null)
//                        temp=(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INSERTION_N1];
//                    else
//                        temp=0;

//                    tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INSERTION_N1]=(decimal)dt.Rows[k]["insertion"]+temp;
//                    #endregion
//                }

//                // Category
//                if(idOldCategory!=(Int64)dt.Rows[k]["id_category"]){
//                    // Evolution
//                    if(start==0){
						 
////						if(webSession.PDM){
////							fillPdmForTendencies(ref tab,currentLineCategory,currentLine,vehicleName);						
////						}

//                        // Investissement
//                        Evol(ref tab,currentLineCategory,FrameWorkConstantes.Tendencies.INVEST_N,FrameWorkConstantes.Tendencies.INVEST_N1,FrameWorkConstantes.Tendencies.EVOL_INVEST);
//                        // Insertion
//                        Evol(ref tab,currentLineCategory,FrameWorkConstantes.Tendencies.INSERTION_N,FrameWorkConstantes.Tendencies.INSERTION_N1,FrameWorkConstantes.Tendencies.EVOL_INSERTION);
						
					
//                        switch(vehicleName){
//                            case DBClassificationConstantes.Vehicles.names.press:
//                            case DBClassificationConstantes.Vehicles.names.internationalPress:
//                                // Surface
//                                Evol(ref tab,currentLineCategory,FrameWorkConstantes.Tendencies.SURFACE_N,FrameWorkConstantes.Tendencies.SURFACE_N1,FrameWorkConstantes.Tendencies.EVOL_SURFACE);	
//                                break;
//                            case DBClassificationConstantes.Vehicles.names.radio:					
//                            case DBClassificationConstantes.Vehicles.names.tv:
//                            case DBClassificationConstantes.Vehicles.names.others:	
//                                // Duree
//                                Evol(ref tab,currentLineCategory,FrameWorkConstantes.Tendencies.DURATION_N,FrameWorkConstantes.Tendencies.DURATION_N1,FrameWorkConstantes.Tendencies.EVOL_DURATION);		
//                                break;
//                            default:
//                                break;
//                        }					
//                    }
//                    indexCategory[idCategory]=currentLine;
//                    idCategory++;
//                    currentLineCategory=currentLine;
//                    tab[currentLineCategory,FrameWorkConstantes.Tendencies.CATEGORY_INDEX]=dt.Rows[k]["category"];
//                    InitializeTable(ref tab,currentLine,vehicleName);
//                    currentLine++;
//                    idOldCategory=(Int64)dt.Rows[k]["id_category"];
//                    start=0;
//                }

				
//                // Support
//                if(idOldMedia!=(Int64)dt.Rows[k]["id_media"]){
//                    // Evolution
//                    if(startMedia==0){		

//                        // Investissement
//                        Evol(ref tab,currentLineMedia,FrameWorkConstantes.Tendencies.INVEST_N,FrameWorkConstantes.Tendencies.INVEST_N1,FrameWorkConstantes.Tendencies.EVOL_INVEST);
//                        // Insertion
//                        Evol(ref tab,currentLineMedia,FrameWorkConstantes.Tendencies.INSERTION_N,FrameWorkConstantes.Tendencies.INSERTION_N1,FrameWorkConstantes.Tendencies.EVOL_INSERTION);
						
//                        switch(vehicleName){
//                            case DBClassificationConstantes.Vehicles.names.press:
//                            case DBClassificationConstantes.Vehicles.names.internationalPress:
//                                // Surface
//                                Evol(ref tab,currentLineMedia,FrameWorkConstantes.Tendencies.SURFACE_N,FrameWorkConstantes.Tendencies.SURFACE_N1,FrameWorkConstantes.Tendencies.EVOL_SURFACE);
//                                break;
//                            case DBClassificationConstantes.Vehicles.names.radio:					
//                            case DBClassificationConstantes.Vehicles.names.tv:
//                            case DBClassificationConstantes.Vehicles.names.others:	
//                                // Duree
//                                Evol(ref tab,currentLineMedia,FrameWorkConstantes.Tendencies.DURATION_N,FrameWorkConstantes.Tendencies.DURATION_N1,FrameWorkConstantes.Tendencies.EVOL_DURATION);
//                                break;
//                            default:
//                                break;
//                        }		
					
//                    }
//                    currentLineMedia=currentLine;				
//                    tab[currentLineMedia,FrameWorkConstantes.Tendencies.MEDIA_INDEX]=dt.Rows[k]["media"];
//                    InitializeTable(ref tab,currentLine,vehicleName);
//                    currentLine++;
//                    idOldMedia=(Int64)dt.Rows[k]["id_media"];
//                    startMedia=0;
//                }

//                #region Category
//                //Category
//                if( firstPeriod<=(int)dt.Rows[k]["period"] && endPeriod>=(int)dt.Rows[k]["period"]){
					
//                    #region Investissement
//                    if(tab[currentLineCategory,FrameWorkConstantes.Tendencies.INVEST_N]!=null)
//                        temp=(decimal)tab[currentLineCategory,FrameWorkConstantes.Tendencies.INVEST_N];
//                    else
//                        temp=0;
						
//                    tab[currentLineCategory,FrameWorkConstantes.Tendencies.INVEST_N]=(decimal)dt.Rows[k]["euro"]+temp;
//                    #endregion

//                    #region Pages /duree
//                    switch(vehicleName){
//                        case DBClassificationConstantes.Vehicles.names.press:
//                        case DBClassificationConstantes.Vehicles.names.internationalPress:
//                            if(tab[currentLineCategory,FrameWorkConstantes.Tendencies.SURFACE_N]!=null)
//                                temp=(decimal)tab[currentLineCategory,FrameWorkConstantes.Tendencies.SURFACE_N];
//                            else
//                                temp=0;
						
//                            tab[currentLineCategory,FrameWorkConstantes.Tendencies.SURFACE_N]=(decimal)dt.Rows[k]["pages"]+temp;
							
//                            break;
//                        case DBClassificationConstantes.Vehicles.names.radio:					
//                        case DBClassificationConstantes.Vehicles.names.tv:
//                        case DBClassificationConstantes.Vehicles.names.others:	
								
//                            if(tab[currentLineCategory,FrameWorkConstantes.Tendencies.DURATION_N]!=null)
//                                temp=(decimal)tab[currentLineCategory,FrameWorkConstantes.Tendencies.DURATION_N];
//                            else
//                                temp=0;
						
//                            tab[currentLineCategory,FrameWorkConstantes.Tendencies.DURATION_N]=(decimal)dt.Rows[k]["duration"]+temp;
//                            break;
//                        default:
//                            break;
//                    }					
//                    #endregion

//                    #region Insertion	
//                    if(tab[currentLineCategory,FrameWorkConstantes.Tendencies.INSERTION_N]!=null)
//                        temp=(decimal)tab[currentLineCategory,FrameWorkConstantes.Tendencies.INSERTION_N];
//                    else
//                        temp=0;
						
//                    tab[currentLineCategory,FrameWorkConstantes.Tendencies.INSERTION_N]=(decimal)dt.Rows[k]["insertion"]+temp;
//                    #endregion

//                }
//                else{
					
//                    #region Investissement N-1
//                    if(tab[currentLineCategory,FrameWorkConstantes.Tendencies.INVEST_N1]!=null)
//                        temp=(decimal)tab[currentLineCategory,FrameWorkConstantes.Tendencies.INVEST_N1];
//                    else
//                        temp=0;

//                    tab[currentLineCategory,FrameWorkConstantes.Tendencies.INVEST_N1]=(decimal)dt.Rows[k]["euro"]+temp;
//                    #endregion

//                    #region Pages N-1 / duree
					
//                    switch(vehicleName){
//                        case DBClassificationConstantes.Vehicles.names.press:
//                        case DBClassificationConstantes.Vehicles.names.internationalPress:
//                            if(tab[currentLineCategory,FrameWorkConstantes.Tendencies.SURFACE_N1]!=null)
//                                temp=(decimal)tab[currentLineCategory,FrameWorkConstantes.Tendencies.SURFACE_N1];
//                            else
//                                temp=0;

//                            tab[currentLineCategory,FrameWorkConstantes.Tendencies.SURFACE_N1]=(decimal)dt.Rows[k]["pages"]+temp;	
//                            break;
//                        case DBClassificationConstantes.Vehicles.names.radio:					
//                        case DBClassificationConstantes.Vehicles.names.tv:
//                        case DBClassificationConstantes.Vehicles.names.others:									
//                            if(tab[currentLineCategory,FrameWorkConstantes.Tendencies.DURATION_N1]!=null)
//                                temp=(decimal)tab[currentLineCategory,FrameWorkConstantes.Tendencies.DURATION_N1];
//                            else
//                                temp=0;

//                            tab[currentLineCategory,FrameWorkConstantes.Tendencies.DURATION_N1]=(decimal)dt.Rows[k]["duration"]+temp;
//                            break;
//                        default:
//                            break;
//                    }					
//                    #endregion

//                    #region Insertion N-1
//                    if(tab[currentLineCategory,FrameWorkConstantes.Tendencies.INSERTION_N1]!=null)
//                        temp=(decimal)tab[currentLineCategory,FrameWorkConstantes.Tendencies.INSERTION_N1];
//                    else
//                        temp=0;

//                    tab[currentLineCategory,FrameWorkConstantes.Tendencies.INSERTION_N1]=(decimal)dt.Rows[k]["insertion"]+temp;
//                    #endregion
//                }
//                #endregion

//                #region Media
//                //Media
//                if( firstPeriod<=(int)dt.Rows[k]["period"] && endPeriod>=(int)(int)dt.Rows[k]["period"]){
					
//                    #region Investissement N
//                    if(tab[currentLineMedia,FrameWorkConstantes.Tendencies.INVEST_N]!=null)
//                        temp=(decimal)tab[currentLineMedia,FrameWorkConstantes.Tendencies.INVEST_N];
//                    else 
//                        temp=0;
					
//                    tab[currentLineMedia,FrameWorkConstantes.Tendencies.INVEST_N]=(decimal)dt.Rows[k]["euro"]+temp;
//                    #endregion

//                    #region Pages N / Duree
				
					
//                    switch(vehicleName){
//                        case DBClassificationConstantes.Vehicles.names.press:
//                        case DBClassificationConstantes.Vehicles.names.internationalPress:
//                            if(tab[currentLineMedia,FrameWorkConstantes.Tendencies.SURFACE_N]!=null)
//                                temp=(decimal)tab[currentLineMedia,FrameWorkConstantes.Tendencies.SURFACE_N];
//                            else 
//                                temp=0;
					
//                            tab[currentLineMedia,FrameWorkConstantes.Tendencies.SURFACE_N]=(decimal)dt.Rows[k]["pages"]+temp;
							
//                            break;
//                        case DBClassificationConstantes.Vehicles.names.radio:					
//                        case DBClassificationConstantes.Vehicles.names.tv:
//                        case DBClassificationConstantes.Vehicles.names.others:								
//                            if(tab[currentLineMedia,FrameWorkConstantes.Tendencies.DURATION_N]!=null)
//                                temp=(decimal)tab[currentLineMedia,FrameWorkConstantes.Tendencies.DURATION_N];
//                            else 
//                                temp=0;
					
//                            tab[currentLineMedia,FrameWorkConstantes.Tendencies.DURATION_N]=(decimal)dt.Rows[k]["duration"]+temp;
//                            break;
//                        default:
//                            break;
//                    }	

					
//                    #endregion

//                    #region Insertion N
//                    if(tab[currentLineMedia,FrameWorkConstantes.Tendencies.INSERTION_N]!=null)
//                        temp=(decimal)tab[currentLineMedia,FrameWorkConstantes.Tendencies.INSERTION_N];
//                    else 
//                        temp=0;
					
//                    tab[currentLineMedia,FrameWorkConstantes.Tendencies.INSERTION_N]=(decimal)dt.Rows[k]["insertion"]+temp;
//                    #endregion

//                }
//                else{

//                    #region Investissement N-1
//                    if(tab[currentLineMedia,FrameWorkConstantes.Tendencies.INVEST_N1]!=null)
//                        temp=(decimal)tab[currentLineMedia,FrameWorkConstantes.Tendencies.INVEST_N1];
//                    else
//                        temp=0;
//                    tab[currentLineMedia,FrameWorkConstantes.Tendencies.INVEST_N1]=(decimal)dt.Rows[k]["euro"]+temp;
//                    #endregion

//                    #region Pages N-1 / Duree
					
//                    switch(vehicleName){
//                        case DBClassificationConstantes.Vehicles.names.press:
//                        case DBClassificationConstantes.Vehicles.names.internationalPress:
//                            if(tab[currentLineMedia,FrameWorkConstantes.Tendencies.SURFACE_N1]!=null)
//                                temp=(decimal)tab[currentLineMedia,FrameWorkConstantes.Tendencies.SURFACE_N1];
//                            else
//                                temp=0;
//                            tab[currentLineMedia,FrameWorkConstantes.Tendencies.SURFACE_N1]=(decimal)dt.Rows[k]["pages"]+temp;
//                            break;
//                        case DBClassificationConstantes.Vehicles.names.radio:					
//                        case DBClassificationConstantes.Vehicles.names.tv:
//                        case DBClassificationConstantes.Vehicles.names.others:									
//                            if(tab[currentLineMedia,FrameWorkConstantes.Tendencies.DURATION_N1]!=null)
//                                temp=(decimal)tab[currentLineMedia,FrameWorkConstantes.Tendencies.DURATION_N1];
//                            else
//                                temp=0;
//                            tab[currentLineMedia,FrameWorkConstantes.Tendencies.SURFACE_N1]=(decimal)dt.Rows[k]["duration"]+temp;
//                            break;
//                        default:
//                            break;
//                    }		
					
					
//                    #endregion

//                    #region Insertion N-1
//                    if(tab[currentLineMedia,FrameWorkConstantes.Tendencies.INSERTION_N1]!=null)
//                        temp=(decimal)tab[currentLineMedia,FrameWorkConstantes.Tendencies.INSERTION_N1];
//                    else
//                        temp=0;
//                    tab[currentLineMedia,FrameWorkConstantes.Tendencies.INSERTION_N1]=(decimal)dt.Rows[k]["insertion"]+temp;
//                    #endregion
//                }
//                #endregion

//            }
//            #endregion
			

//            #region Dernière ligne			
			
//            // Support
//            // Investissement
//            Evol(ref tab,currentLineMedia,FrameWorkConstantes.Tendencies.INVEST_N,FrameWorkConstantes.Tendencies.INVEST_N1,FrameWorkConstantes.Tendencies.EVOL_INVEST);
//            // Insertion
//            Evol(ref tab,currentLineMedia,FrameWorkConstantes.Tendencies.INSERTION_N,FrameWorkConstantes.Tendencies.INSERTION_N1,FrameWorkConstantes.Tendencies.EVOL_INSERTION);
			

//            //Category
//            // Investissement
//            Evol(ref tab,currentLineCategory,FrameWorkConstantes.Tendencies.INVEST_N,FrameWorkConstantes.Tendencies.INVEST_N1,FrameWorkConstantes.Tendencies.EVOL_INVEST);
//            // Insertion
//            Evol(ref tab,currentLineCategory,FrameWorkConstantes.Tendencies.INSERTION_N,FrameWorkConstantes.Tendencies.INSERTION_N1,FrameWorkConstantes.Tendencies.EVOL_INSERTION);
			

//            //Evolution pour la ligne total
//            // Investissement
//            Evol(ref tab,FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INVEST_N,FrameWorkConstantes.Tendencies.INVEST_N1,FrameWorkConstantes.Tendencies.EVOL_INVEST);
//            // Insertion
//            Evol(ref tab,FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INSERTION_N,FrameWorkConstantes.Tendencies.INSERTION_N1,FrameWorkConstantes.Tendencies.EVOL_INSERTION);
			
		
//            switch(vehicleName){
//                case DBClassificationConstantes.Vehicles.names.press:
//                case DBClassificationConstantes.Vehicles.names.internationalPress:
//                    // Surface
//                    Evol(ref tab,currentLineMedia,FrameWorkConstantes.Tendencies.SURFACE_N,FrameWorkConstantes.Tendencies.SURFACE_N1,FrameWorkConstantes.Tendencies.EVOL_SURFACE);
//                    // Surface
//                    Evol(ref tab,currentLineCategory,FrameWorkConstantes.Tendencies.SURFACE_N,FrameWorkConstantes.Tendencies.SURFACE_N1,FrameWorkConstantes.Tendencies.EVOL_SURFACE);
//                    // Surface
//                    Evol(ref tab,FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.SURFACE_N,FrameWorkConstantes.Tendencies.SURFACE_N1,FrameWorkConstantes.Tendencies.EVOL_SURFACE);
//                    break;
//                case DBClassificationConstantes.Vehicles.names.radio:					
//                case DBClassificationConstantes.Vehicles.names.tv:
//                case DBClassificationConstantes.Vehicles.names.others:									
//                    // duree
//                    Evol(ref tab,currentLineMedia,FrameWorkConstantes.Tendencies.DURATION_N,FrameWorkConstantes.Tendencies.DURATION_N1,FrameWorkConstantes.Tendencies.EVOL_DURATION);
//                    // duree
//                    Evol(ref tab,currentLineCategory,FrameWorkConstantes.Tendencies.DURATION_N,FrameWorkConstantes.Tendencies.DURATION_N1,FrameWorkConstantes.Tendencies.EVOL_DURATION);
//                    // duree
//                    Evol(ref tab,FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.DURATION_N,FrameWorkConstantes.Tendencies.DURATION_N1,FrameWorkConstantes.Tendencies.EVOL_DURATION);
//                    break;
//                default:
//                    break;
//            }		

//            if(webSession.PDM){
//                FillPdmTotal(ref tab,currentLine,vehicleName);
					
//            }

//            #endregion 

//            return tab;
//        }

//        #region Méthodes Internes
		
//        /// <summary>
//        /// Calcul des pdm pour les medias
//        /// </summary>
//        /// <param name="tab">tableau</param>
//        /// <param name="categoryLine">line de la categorie</param>
//        /// <param name="endLine">dernière ligne</param>
//        /// <param name="vehicleName">Nom du vehicle</param>
//        private static void FillPdmForTendencies(ref object[,] tab,int categoryLine,int endLine,DBClassificationConstantes.Vehicles.names vehicleName){
		
//            for(int i=categoryLine+1;i<endLine;i++){
//                // Investissement
//                if((decimal)tab[i,FrameWorkConstantes.Tendencies.INVEST_N]!=0){
//                    tab[i,FrameWorkConstantes.Tendencies.INVEST_N]=(decimal)tab[i,FrameWorkConstantes.Tendencies.INVEST_N]/(decimal)tab[categoryLine,FrameWorkConstantes.Tendencies.INVEST_N]*100;
//                }
//                if((decimal)tab[i,FrameWorkConstantes.Tendencies.INVEST_N1]!=0){
//                    tab[i,FrameWorkConstantes.Tendencies.INVEST_N1]=(decimal)tab[i,FrameWorkConstantes.Tendencies.INVEST_N1]/(decimal)tab[categoryLine,FrameWorkConstantes.Tendencies.INVEST_N1]*100;
//                }				
//                Evol(ref tab,i,FrameWorkConstantes.Tendencies.INVEST_N,FrameWorkConstantes.Tendencies.INVEST_N1,FrameWorkConstantes.Tendencies.EVOL_INVEST);
				
//                // Insertion
//                if((decimal)tab[i,FrameWorkConstantes.Tendencies.INSERTION_N]!=0){
//                    tab[i,FrameWorkConstantes.Tendencies.INSERTION_N]=(decimal)tab[i,FrameWorkConstantes.Tendencies.INSERTION_N]/(decimal)tab[categoryLine,FrameWorkConstantes.Tendencies.INSERTION_N]*100;
//                }
//                if((decimal)tab[i,FrameWorkConstantes.Tendencies.INSERTION_N1]!=0){
//                    tab[i,FrameWorkConstantes.Tendencies.INSERTION_N1]=(decimal)tab[i,FrameWorkConstantes.Tendencies.INSERTION_N1]/(decimal)tab[categoryLine,FrameWorkConstantes.Tendencies.INSERTION_N1]*100;
//                }
//                Evol(ref tab,i,FrameWorkConstantes.Tendencies.INSERTION_N,FrameWorkConstantes.Tendencies.INSERTION_N1,FrameWorkConstantes.Tendencies.EVOL_INSERTION);			
			

//                switch(vehicleName){
//                    case DBClassificationConstantes.Vehicles.names.press:
//                    case DBClassificationConstantes.Vehicles.names.internationalPress:
//                        // Surface
//                        if((decimal)tab[i,FrameWorkConstantes.Tendencies.SURFACE_N]!=0){
//                            tab[i,FrameWorkConstantes.Tendencies.SURFACE_N]=(decimal)tab[i,FrameWorkConstantes.Tendencies.SURFACE_N]/(decimal)tab[categoryLine,FrameWorkConstantes.Tendencies.SURFACE_N]*100;
//                        }
//                        if((decimal)tab[i,FrameWorkConstantes.Tendencies.SURFACE_N1]!=0){
//                            tab[i,FrameWorkConstantes.Tendencies.SURFACE_N1]=(decimal)tab[i,FrameWorkConstantes.Tendencies.SURFACE_N1]/(decimal)tab[categoryLine,FrameWorkConstantes.Tendencies.SURFACE_N1]*100;
//                        }
//                        Evol(ref tab,i,FrameWorkConstantes.Tendencies.SURFACE_N,FrameWorkConstantes.Tendencies.SURFACE_N1,FrameWorkConstantes.Tendencies.EVOL_SURFACE);
//                        break;
//                    case DBClassificationConstantes.Vehicles.names.radio:					
//                    case DBClassificationConstantes.Vehicles.names.tv:
//                    case DBClassificationConstantes.Vehicles.names.others:									
//                        // Duree
//                        if((decimal)tab[i,FrameWorkConstantes.Tendencies.DURATION_N]!=0){
//                            tab[i,FrameWorkConstantes.Tendencies.DURATION_N]=(decimal)tab[i,FrameWorkConstantes.Tendencies.DURATION_N]/(decimal)tab[categoryLine,FrameWorkConstantes.Tendencies.DURATION_N]*100;
//                        }
//                        if((decimal)tab[i,FrameWorkConstantes.Tendencies.DURATION_N1]!=0){
//                            tab[i,FrameWorkConstantes.Tendencies.DURATION_N1]=(decimal)tab[i,FrameWorkConstantes.Tendencies.DURATION_N1]/(decimal)tab[categoryLine,FrameWorkConstantes.Tendencies.DURATION_N1]*100;
//                        }
//                        Evol(ref tab,i,FrameWorkConstantes.Tendencies.DURATION_N,FrameWorkConstantes.Tendencies.DURATION_N1,FrameWorkConstantes.Tendencies.EVOL_DURATION);
//                        break;
//                    default:
//                        break;
//                }		
//            }
			
//        }

//        /// <summary>
//        /// Calcul des PDM pour les categories
//        /// </summary>
//        /// <param name="tab">tableau</param>
//        /// <param name="indexCategory">index de la category</param>
//        /// <param name="vehicleName">Nom du vehicle</param>
//        private static void FillPdmCategoryForTendencies(ref object[,] tab,int[] indexCategory,DBClassificationConstantes.Vehicles.names vehicleName){
		
//            for(int i=0;i<indexCategory.Length;i++){
//                // Investissement
//                if((decimal)tab[indexCategory[i],FrameWorkConstantes.Tendencies.INVEST_N]!=0){
//                    tab[indexCategory[i],FrameWorkConstantes.Tendencies.INVEST_N]=(decimal)tab[indexCategory[i],FrameWorkConstantes.Tendencies.INVEST_N]/(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INVEST_N]*100;
//                }
//                if((decimal)tab[indexCategory[i],FrameWorkConstantes.Tendencies.INVEST_N1]!=0){
//                    tab[indexCategory[i],FrameWorkConstantes.Tendencies.INVEST_N1]=(decimal)tab[indexCategory[i],FrameWorkConstantes.Tendencies.INVEST_N1]/(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INVEST_N1]*100;
//                }				
//                Evol(ref tab,indexCategory[i],FrameWorkConstantes.Tendencies.INVEST_N,FrameWorkConstantes.Tendencies.INVEST_N1,FrameWorkConstantes.Tendencies.EVOL_INVEST);
				
//                // Insertion
//                if((decimal)tab[indexCategory[i],FrameWorkConstantes.Tendencies.INSERTION_N]!=0){
//                    tab[indexCategory[i],FrameWorkConstantes.Tendencies.INSERTION_N]=(decimal)tab[indexCategory[i],FrameWorkConstantes.Tendencies.INSERTION_N]/(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INSERTION_N]*100;
//                }
//                if((decimal)tab[indexCategory[i],FrameWorkConstantes.Tendencies.INSERTION_N1]!=0){
//                    tab[indexCategory[i],FrameWorkConstantes.Tendencies.INSERTION_N1]=(decimal)tab[indexCategory[i],FrameWorkConstantes.Tendencies.INSERTION_N1]/(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INSERTION_N1]*100;
//                }
//                Evol(ref tab,indexCategory[i],FrameWorkConstantes.Tendencies.INSERTION_N,FrameWorkConstantes.Tendencies.INSERTION_N1,FrameWorkConstantes.Tendencies.EVOL_INSERTION);			
			

//                switch(vehicleName){
//                    case DBClassificationConstantes.Vehicles.names.press:
//                    case DBClassificationConstantes.Vehicles.names.internationalPress:
//                        // Surface
//                        if((decimal)tab[indexCategory[i],FrameWorkConstantes.Tendencies.SURFACE_N]!=0){
//                            tab[indexCategory[i],FrameWorkConstantes.Tendencies.SURFACE_N]=(decimal)tab[indexCategory[i],FrameWorkConstantes.Tendencies.SURFACE_N]/(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.SURFACE_N]*100;
//                        }
//                        if((decimal)tab[indexCategory[i],FrameWorkConstantes.Tendencies.SURFACE_N1]!=0){
//                            tab[indexCategory[i],FrameWorkConstantes.Tendencies.SURFACE_N1]=(decimal)tab[indexCategory[i],FrameWorkConstantes.Tendencies.SURFACE_N1]/(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.SURFACE_N1]*100;
//                        }
//                        Evol(ref tab,indexCategory[i],FrameWorkConstantes.Tendencies.SURFACE_N,FrameWorkConstantes.Tendencies.SURFACE_N1,FrameWorkConstantes.Tendencies.EVOL_SURFACE);
//                        break;
//                    case DBClassificationConstantes.Vehicles.names.radio:					
//                    case DBClassificationConstantes.Vehicles.names.tv:
//                    case DBClassificationConstantes.Vehicles.names.others:									
//                        // Duree
//                        if((decimal)tab[indexCategory[i],FrameWorkConstantes.Tendencies.DURATION_N]!=0){
//                            tab[indexCategory[i],FrameWorkConstantes.Tendencies.DURATION_N]=(decimal)tab[indexCategory[i],FrameWorkConstantes.Tendencies.DURATION_N]/(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.DURATION_N]*100;
//                        }
//                        if((decimal)tab[indexCategory[i],FrameWorkConstantes.Tendencies.DURATION_N1]!=0){
//                            tab[indexCategory[i],FrameWorkConstantes.Tendencies.DURATION_N1]=(decimal)tab[indexCategory[i],FrameWorkConstantes.Tendencies.DURATION_N1]/(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.DURATION_N1]*100;
//                        }
//                        Evol(ref tab,indexCategory[i],FrameWorkConstantes.Tendencies.DURATION_N,FrameWorkConstantes.Tendencies.DURATION_N1,FrameWorkConstantes.Tendencies.EVOL_DURATION);
//                        break;
//                    default:
//                        break;
//                }		
//            }
			
//        }


//        /// <summary>
//        /// Calcul des pdm pour les medias la base 100 étant le total média
//        /// </summary>
//        /// <param name="tab">tableau</param>
//        /// <param name="endLine">dernière ligne</param>
//        /// <param name="vehicleName">Nom du vehicle</param>
//        private static void FillPdmTotal(ref object[,] tab,int endLine,DBClassificationConstantes.Vehicles.names vehicleName){
		
//            for(int i=FrameWorkConstantes.Tendencies.TOTAL_LINE+1;i<endLine;i++){
//                // Investissement
//                if((decimal)tab[i,FrameWorkConstantes.Tendencies.INVEST_N]!=0){
//                    tab[i,FrameWorkConstantes.Tendencies.INVEST_N]=(decimal)tab[i,FrameWorkConstantes.Tendencies.INVEST_N]/(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INVEST_N]*100;
//                }
//                if((decimal)tab[i,FrameWorkConstantes.Tendencies.INVEST_N1]!=0){
//                    tab[i,FrameWorkConstantes.Tendencies.INVEST_N1]=(decimal)tab[i,FrameWorkConstantes.Tendencies.INVEST_N1]/(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INVEST_N1]*100;
//                }				
//                Evol(ref tab,i,FrameWorkConstantes.Tendencies.INVEST_N,FrameWorkConstantes.Tendencies.INVEST_N1,FrameWorkConstantes.Tendencies.EVOL_INVEST);
				
//                // Insertion
//                if((decimal)tab[i,FrameWorkConstantes.Tendencies.INSERTION_N]!=0){
//                    tab[i,FrameWorkConstantes.Tendencies.INSERTION_N]=(decimal)tab[i,FrameWorkConstantes.Tendencies.INSERTION_N]/(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INSERTION_N]*100;
//                }
//                if((decimal)tab[i,FrameWorkConstantes.Tendencies.INSERTION_N1]!=0){
//                    tab[i,FrameWorkConstantes.Tendencies.INSERTION_N1]=(decimal)tab[i,FrameWorkConstantes.Tendencies.INSERTION_N1]/(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INSERTION_N1]*100;
//                }
//                Evol(ref tab,i,FrameWorkConstantes.Tendencies.INSERTION_N,FrameWorkConstantes.Tendencies.INSERTION_N1,FrameWorkConstantes.Tendencies.EVOL_INSERTION);			
			

//                switch(vehicleName){
//                    case DBClassificationConstantes.Vehicles.names.press:
//                    case DBClassificationConstantes.Vehicles.names.internationalPress:
//                        // Surface
//                        if((decimal)tab[i,FrameWorkConstantes.Tendencies.SURFACE_N]!=0){
//                            tab[i,FrameWorkConstantes.Tendencies.SURFACE_N]=(decimal)tab[i,FrameWorkConstantes.Tendencies.SURFACE_N]/(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.SURFACE_N]*100;
//                        }
//                        if((decimal)tab[i,FrameWorkConstantes.Tendencies.SURFACE_N1]!=0){
//                            tab[i,FrameWorkConstantes.Tendencies.SURFACE_N1]=(decimal)tab[i,FrameWorkConstantes.Tendencies.SURFACE_N1]/(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.SURFACE_N1]*100;
//                        }
//                        Evol(ref tab,i,FrameWorkConstantes.Tendencies.SURFACE_N,FrameWorkConstantes.Tendencies.SURFACE_N1,FrameWorkConstantes.Tendencies.EVOL_SURFACE);
//                        break;
//                    case DBClassificationConstantes.Vehicles.names.radio:					
//                    case DBClassificationConstantes.Vehicles.names.tv:
//                    case DBClassificationConstantes.Vehicles.names.others:									
//                        // Duree
//                        if((decimal)tab[i,FrameWorkConstantes.Tendencies.DURATION_N]!=0){
//                            tab[i,FrameWorkConstantes.Tendencies.DURATION_N]=(decimal)tab[i,FrameWorkConstantes.Tendencies.DURATION_N]/(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.DURATION_N]*100;
//                        }
//                        if((decimal)tab[i,FrameWorkConstantes.Tendencies.DURATION_N1]!=0){
//                            tab[i,FrameWorkConstantes.Tendencies.DURATION_N1]=(decimal)tab[i,FrameWorkConstantes.Tendencies.DURATION_N1]/(decimal)tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.DURATION_N1]*100;
//                        }
//                        Evol(ref tab,i,FrameWorkConstantes.Tendencies.DURATION_N,FrameWorkConstantes.Tendencies.DURATION_N1,FrameWorkConstantes.Tendencies.EVOL_DURATION);
//                        break;
//                    default:
//                        break;
//                }		
//            }			
//        }


//        /// <summary>
//        /// Calcul de l'évolution
//        /// </summary>
//        /// <param name="tab">Tableau</param>
//        /// <param name="line">ligne à traiter</param>
//        /// <param name="N">Index de la colonne de l'année N</param>
//        /// <param name="N1">Index de la colonne de l'année N-1</param>
//        /// <param name="Evol">Index de la colonne Evolution</param>
//        private static void Evol(ref object[,] tab,int line,int N,int N1,int Evol){
			
//            decimal ecart=0;
//            if(tab[line,N]==null){
//                tab[line,N]=(decimal)0;
//            }
//            if(tab[line,N1]==null){
//                tab[line,N1]=(decimal)0;
//            }

//            if((decimal)tab[line,N]!=0 && (decimal)tab[line,N1]==0){
//                tab[line,Evol]=null;			
//            }
//            else if((decimal)tab[line,N1]!=0 && (decimal)tab[line,N]==0){
//                tab[line,Evol]=null;
//            }
//            else{
//                ecart=((decimal)tab[line,N]-(decimal)tab[line,N1]);
//                if((decimal)tab[line,N]-ecart!=0){
//                    tab[line,Evol]=ecart/((decimal)tab[line,N]-ecart)*100;
//                }
//            }
		
//        }
		
//        /// <summary>
//        /// Initialisation de la table
//        /// </summary>
//        /// <param name="tab">table</param>
//        /// <param name="currentLine">Ligne à traiter</param>
//        /// <param name="vehicleName">Nom du vehicle</param>
//        private static void InitializeTable(ref object[,] tab,int currentLine,DBClassificationConstantes.Vehicles.names vehicleName){
			
//            tab[currentLine,FrameWorkConstantes.Tendencies.INVEST_N]=(decimal)0;
//            tab[currentLine,FrameWorkConstantes.Tendencies.INVEST_N1]=(decimal)0;
//            tab[currentLine,FrameWorkConstantes.Tendencies.EVOL_INVEST]=(decimal)0;
//            tab[currentLine,FrameWorkConstantes.Tendencies.INSERTION_N]=(decimal)0;
//            tab[currentLine,FrameWorkConstantes.Tendencies.INSERTION_N1]=(decimal)0;
//            tab[currentLine,FrameWorkConstantes.Tendencies.EVOL_INSERTION]=(decimal)0;


//            switch(vehicleName){
//                case DBClassificationConstantes.Vehicles.names.press:
//                case DBClassificationConstantes.Vehicles.names.internationalPress:
//                    tab[currentLine,FrameWorkConstantes.Tendencies.SURFACE_N]=(decimal)0;
//                    tab[currentLine,FrameWorkConstantes.Tendencies.SURFACE_N1]=(decimal)0;
//                    tab[currentLine,FrameWorkConstantes.Tendencies.EVOL_SURFACE]=(decimal)0;
//                    break;
//                case DBClassificationConstantes.Vehicles.names.radio:					
//                case DBClassificationConstantes.Vehicles.names.tv:
//                case DBClassificationConstantes.Vehicles.names.others:	
//                    tab[currentLine,FrameWorkConstantes.Tendencies.DURATION_N]=(decimal)0;
//                    tab[currentLine,FrameWorkConstantes.Tendencies.DURATION_N1]=(decimal)0;
//                    tab[currentLine,FrameWorkConstantes.Tendencies.EVOL_DURATION]=(decimal)0;
//                    break;
//                default:
//                    break;
//            }		
//        }

//        #endregion

//    }
//}
