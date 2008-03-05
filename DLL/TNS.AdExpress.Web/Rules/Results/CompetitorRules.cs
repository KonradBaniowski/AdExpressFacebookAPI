#region Informations
// Auteur: G. Facon 
// Date de création: 16/09/2004 
// Date de modification:
//		22/11/2004: G Facon			Refonte de la structure pour intégrer l'analyse
//		20/01/2005: G Facon			Modification de la règle de calcul des PDMS
//      27/04/2005: K. Shehzad		Modification pour ajouter des choses dans "produits détaillés par" dropDownList
//      18/05/2005: K. Shehzad		Modification pour ajouter des choses dans "produits détaillés par" dropDownList
//      22/06/2005: K. Shehzad		Modification pour ajouter des choses dans "produits détaillés par" dropDownList
//      27/10/2005: D. V. Mussuma	unités pages divisé par 1000 dans "UI" non plus dans "Rules" (centralisation des unités)
//      19/09/2006: D. V. Mussuma	Ajout du niveau de détail produit
//		24/11/2006	G. Facon		Intégration des niveaux génériques et webResultUI
//		29/11/2006	G. Facon		Modification des synthèse pour gérer les resultTables
//      21/08/2007  G. Facon        Ajout Colonne version et insertion
#endregion

#region Using
using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.FrameWork.Date;

using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using CustomerConstantes=TNS.AdExpress.Constantes.Customer;
using WebFunctions=TNS.AdExpress.Web.Functions;	
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using WebCommon=TNS.AdExpress.Web.Common;
using WebCore=TNS.AdExpress.Web.Core;
using ClassificationConstantes=TNS.AdExpress.Constantes.Classification;
using ClassificationDB=TNS.AdExpress.Classification.DataAccess;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Translation;
using WebResultUI=TNS.FrameWork.WebResultUI;
using TNS.FrameWork.WebResultUI;
using ComptitorResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results.CompetitorAlert;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Domain.Level;
#endregion



namespace TNS.AdExpress.Web.Rules.Results{
	/// <summary>
	/// Traitements métiers des alertes concurrentielles
	/// </summary>
	public class CompetitorRules{

		#region Constantes
		/// <summary>
		/// Nombre de colonnes à ajouter aux tableaux de résultat
		/// </summary>
		/// <remarks>Début de ligne et fin de ligne</remarks>
		private const int NB_COLUMNS_TO_ADD=2;
		const long LEVEL_COLUMN_INDEX=1;
		const long START_COLUMN_INDEX=2;
		#endregion

		#region GetData
        /// <summary>
		/// Obtient le tableau contenant l'ensemble des résultats
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Tableau de résultats</returns>
        public static WebResultUI.ResultTable GetData(WebSession webSession) {
            WebCommon.Results.SelectionSubGroup[] subGroupMediaTotalIndex = new WebCommon.Results.SelectionSubGroup[1];
            return GetData(webSession, ref subGroupMediaTotalIndex);
        }

		/// <summary>
		/// Obtient le tableau contenant l'ensemble des résultats
		/// </summary>
		/// <param name="webSession">Session du client</param>
        /// <param name="subGroupColumnTotalIndex">Liste des sous groupes</param>
		/// <returns>Tableau de résultats</returns>
        public static WebResultUI.ResultTable GetData(WebSession webSession, ref WebCommon.Results.SelectionSubGroup[] subGroupColumnTotalIndex) {

			#region Variables
			int positionUnivers=1;
			long currentLine;
			long currentColumn;
			long currentLineInTabResult;
			long nbLineInTabResult=-1;
			bool allSubTotalNotNull=true;
			bool subTotalNotNull=true;
			#endregion

			#region Tableaux d'index
			WebCommon.Results.SelectionGroup[] groupMediaTotalIndex=new WebCommon.Results.SelectionGroup[11];
			WebCommon.Results.SelectionSubGroup[] subGroupMediaTotalIndex=new WebCommon.Results.SelectionSubGroup[1000];
			int nbUnivers=0;
			Hashtable mediaIndex=new Hashtable();
			string mediaListForLabelSearch="";
			int maxIndex=0;
			#endregion

			#region Chargement du tableau
			long nbLineInNewTable=0;
            object[,] tabData = GetPreformatedTable(webSession, groupMediaTotalIndex, subGroupMediaTotalIndex, mediaIndex, ref maxIndex, ref nbLineInNewTable, ref nbUnivers, ref mediaListForLabelSearch);
			#endregion
				
			if(tabData==null){
				return null;
			}
            
			#region Déclaration du tableau de résultat
			long nbCol=tabData.GetLength(0);
			long nbLineInFormatedTable=nbLineInNewTable;
			object[,] tabResult=new object[nbCol,nbLineInFormatedTable];
			#endregion
			
			#region Traitement des données
			currentLineInTabResult=-1;


			#region Partie Concurentielle
			if(webSession.CurrentModule==WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE ||
				webSession.CurrentModule==WebConstantes.Module.Name.ALERTE_CONCURENTIELLE 	){
				switch(webSession.CurrentTab){

					#region Portefeuille
					case TNS.AdExpress.Constantes.FrameWork.Results.CompetitorMarketShare.PORTEFEUILLE:
						// Pas de traitement
						tabResult=tabData;
						nbLineInTabResult=nbLineInFormatedTable;
						break;
					#endregion

					#region Absent
					case TNS.AdExpress.Constantes.FrameWork.Results.CompetitorMarketShare.ABSENT:
						for(currentLine=0;currentLine<nbLineInFormatedTable;currentLine++){
							positionUnivers=1;
							subTotalNotNull=false;
							// On cherche les lignes qui on des unités à 0(null) dans le premier sous total
							if((double)tabData[groupMediaTotalIndex[1].IndexInResultTable,currentLine]==0.0){
								positionUnivers++;
								while(!subTotalNotNull && positionUnivers<nbUnivers){
									if((double)tabData[groupMediaTotalIndex[positionUnivers].IndexInResultTable,currentLine]!=0.0)subTotalNotNull=true;
									positionUnivers++;
								}
								//au moins un sous total de concurrent différent à 0(null)
								if(subTotalNotNull){
									currentLineInTabResult++;
									for(currentColumn=0;currentColumn<nbCol;currentColumn++){
										tabResult[currentColumn,currentLineInTabResult]=tabData[currentColumn,currentLine];
									}
								}
				
							}
						}
						nbLineInTabResult=currentLineInTabResult+1;
						break;
					#endregion
				
					#region Exclusif
					case TNS.AdExpress.Constantes.FrameWork.Results.CompetitorMarketShare.EXCLUSIF:
						for(currentLine=0;currentLine<nbLineInFormatedTable;currentLine++){
							positionUnivers=1;
							allSubTotalNotNull=true;
							// On cherche les lignes qui on des unités différentes de 0(null) dans le premier sous total
							if((double)tabData[groupMediaTotalIndex[1].IndexInResultTable,currentLine]!=0.0){
								positionUnivers++;
								while(allSubTotalNotNull && positionUnivers<nbUnivers){
									if((double)tabData[groupMediaTotalIndex[positionUnivers].IndexInResultTable,currentLine]!=0.0)allSubTotalNotNull=false;
									positionUnivers++;
								}
								// et tous les sous totaux de concurrent à 0(null)
								if(allSubTotalNotNull){
									currentLineInTabResult++;
									for(currentColumn=0;currentColumn<nbCol;currentColumn++){
										tabResult[currentColumn,currentLineInTabResult]=tabData[currentColumn,currentLine];
									}
								}
				
							}
						}
						nbLineInTabResult=currentLineInTabResult+1;
						break;
					#endregion

					#region Commun
					case TNS.AdExpress.Constantes.FrameWork.Results.CompetitorMarketShare.COMMON:
						for(currentLine=0;currentLine<nbLineInFormatedTable;currentLine++){
							allSubTotalNotNull=true;
							positionUnivers=1;
							while(allSubTotalNotNull && positionUnivers<nbUnivers){
								if((double)tabData[groupMediaTotalIndex[positionUnivers].IndexInResultTable,currentLine]==0.0)allSubTotalNotNull=false;
								positionUnivers++;
							}
							if(allSubTotalNotNull){
								currentLineInTabResult++;
								for(currentColumn=0;currentColumn<nbCol;currentColumn++){
									tabResult[currentColumn,currentLineInTabResult]=tabData[currentColumn,currentLine];
								}
							}
						}
						nbLineInTabResult=currentLineInTabResult+1;
						break;
					#endregion

                    #region Forces ou Potentiels
                    case TNS.AdExpress.Constantes.FrameWork.Results.CompetitorMarketShare.FORCES:
                    case TNS.AdExpress.Constantes.FrameWork.Results.CompetitorMarketShare.POTENTIELS:
                        tabResult = tabData;
                        nbLineInTabResult = nbLineInFormatedTable;
                        subGroupColumnTotalIndex = subGroupMediaTotalIndex;
                        break;
                    #endregion
                }
			}
			#endregion

			#region Partie Potentiels
			if(webSession.CurrentModule==WebConstantes.Module.Name.ANALYSE_POTENTIELS ||
				webSession.CurrentModule==WebConstantes.Module.Name.ALERTE_POTENTIELS 	){
				
				// Code où l'on supprime les lignes	
				/*
				for(currentLine=0;currentLine<nbLineInFormatedTable;currentLine++){
					allSubTotalNotNull=true;
					positionUnivers=1;
					while(allSubTotalNotNull && positionUnivers<nbUnivers){
						if((double)tabData[groupMediaTotalIndex[positionUnivers].IndexInResultTable,currentLine]==0.0)allSubTotalNotNull=false;
						positionUnivers++;
					}
					if(allSubTotalNotNull){
						currentLineInTabResult++;
						for(currentColumn=0;currentColumn<nbCol;currentColumn++){
							tabResult[currentColumn,currentLineInTabResult]=tabData[currentColumn,currentLine];
						}
					}
				}*/
				// Test Mettre toutes lignes
				// Pas de traitement
				tabResult=tabData;
				nbLineInTabResult=nbLineInFormatedTable;
                subGroupColumnTotalIndex = subGroupMediaTotalIndex;

			
			}
			#endregion


			#endregion

			#region Debug: Voir le tableau
//						int i,j;
//						string HTML="<html><table><tr>";
//						for(i=0;i<=currentLineInTabResult;i++){
//							for(j=0;j<nbCol;j++){
//								if(tabResult[j,i]!=null)HTML+="<td>"+tabResult[j,i].ToString()+"</td>";
//								else HTML+="<td>&nbsp;</td>";
//							}
//							HTML+="</tr><tr>";
//						}
//						HTML+="</tr></table></html>";
			#endregion

			return(GetResultTable(webSession,tabResult,nbLineInTabResult,groupMediaTotalIndex, subGroupMediaTotalIndex,mediaIndex,mediaListForLabelSearch,nbUnivers));
		}
		#endregion
		
		#region Synthèse
		/// <summary>
		/// Obtient le tableau contenant la synthèse du nombre d’éléments produits Communs, Absents, Exclusifs. 
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Tableau de résultats</returns>
		public static ResultTable GetSynthesisData(WebSession webSession){
			
			#region Variables
			int positionUnivers=1;
			long nbLine=8;
			Int64 advertiserLineIndex=0;
			Int64 brandLineIndex=0;
			Int64 productLineIndex=0;
			Int64 sectorLineIndex=0;
			Int64 subsectorLineIndex=0;
			Int64 groupLineIndex=0;
			Int64 agencyGroupLineIndex=0;
			Int64 agencyLineIndex=0;
			ArrayList advertisers = null;
			ArrayList products=null;
			ArrayList brands = null;
			ArrayList sectors=null;
			ArrayList subsectors = null;
			ArrayList groups=null;
			ArrayList agencyGroups = null;
			ArrayList agency=null;
			DataTable dt = null;
            //object[,] tabResult = null;
			IList referenceUniversMedia = null;
			IList competitorUniversMedia = null;
			string mediaList="";
			string expression="";
			string sort="id_media asc";
			
			#endregion
			
			#region Formattage des dates 
			string periodBeginning = GetDateBegin(webSession);
			string periodEnd = GetDateEnd(webSession);
			#endregion

			#region Sélection du vehicle
			string vehicleSelection=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerConstantes.Right.type.vehicleAccess);
			DBClassificationConstantes.Vehicles.names vehicleName=(DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
			if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw(new WebExceptions.CompetitorRulesException("La sélection de médias est incorrecte"));
			#endregion
			
			Module currentModuleDescription=ModulesList.GetModule(webSession.CurrentModule);

			#region Chargement des données
			//dt = CompetitorDataAccess.GetSynthesisData(webSession,vehicleName,periodBeginning,periodEnd);
            dt = CompetitorDataAccess.GetGenericSynthesisData(webSession, vehicleName);
			
//			switch(webSession.CurrentModule){
//				case WebConstantes.Module.Name.ALERTE_CONCURENTIELLE:
//					dt = CompetitorAlertDataAccess.GetSynthesisData(webSession,vehicleName,currentModuleDescription.ModuleType,periodBeginning,periodEnd);
//					break;
//				case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
//					dt = CompetitorDataAccess.GetSynthesisData(webSession,vehicleName,periodBeginning,periodEnd);
//					break;				
//			}

			
			#endregion

			#region Identifiant du texte des unités
			Int64 unitId=webSession.GetUnitLabelId();
			CellUnitFactory cellUnitFactory=webSession.GetCellUnitFactory();
			#endregion

			#region Création des headers
			nbLine=5;
			if(webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE]!=null)nbLine++;
			if(webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_MEDIA_AGENCY]!=null)nbLine+=2;

			// Ajout de la colonne Produit
			Headers headers=new Headers();
			headers.Root.Add(new WebResultUI.Header(GestionWeb.GetWebWord(1164,webSession.SiteLanguage),ComptitorResultConstantes.LEVEL_HEADER_ID));

			#region Communs
			WebResultUI.HeaderGroup present=new WebResultUI.HeaderGroup(GestionWeb.GetWebWord(1127,webSession.SiteLanguage),ComptitorResultConstantes.PRESENT_HEADER_ID);
			present.Add(new WebResultUI.Header(true,GestionWeb.GetWebWord(1852,webSession.SiteLanguage),ComptitorResultConstantes.ITEM_NUMBER_HEADER_ID));
			WebResultUI.Header unitPresent=new WebResultUI.Header(GestionWeb.GetWebWord(unitId,webSession.SiteLanguage),ComptitorResultConstantes.UNIT_HEADER_ID);
			unitPresent.Add(new WebResultUI.Header(true,GestionWeb.GetWebWord(1365,webSession.SiteLanguage),ComptitorResultConstantes.REFERENCE_MEDIA_HEADER_ID));
			unitPresent.Add(new WebResultUI.Header(true,GestionWeb.GetWebWord(1366,webSession.SiteLanguage),ComptitorResultConstantes.COMPETITOR_MEDIA_HEADER_ID));
			present.Add(unitPresent);
			headers.Root.Add(present);
			#endregion

			#region Absents
			WebResultUI.HeaderGroup absent=new WebResultUI.HeaderGroup(GestionWeb.GetWebWord(1126,webSession.SiteLanguage),ComptitorResultConstantes.ABSENT_HEADER_ID);
			absent.Add(new WebResultUI.Header(true,GestionWeb.GetWebWord(1852,webSession.SiteLanguage),ComptitorResultConstantes.ITEM_NUMBER_HEADER_ID));
			WebResultUI.Header unitAbsent=new WebResultUI.Header(GestionWeb.GetWebWord(unitId,webSession.SiteLanguage),ComptitorResultConstantes.UNIT_HEADER_ID);
			unitAbsent.Add(new WebResultUI.Header(true,GestionWeb.GetWebWord(1365,webSession.SiteLanguage),ComptitorResultConstantes.REFERENCE_MEDIA_HEADER_ID));
			unitAbsent.Add(new WebResultUI.Header(true,GestionWeb.GetWebWord(1366,webSession.SiteLanguage),ComptitorResultConstantes.COMPETITOR_MEDIA_HEADER_ID));
			absent.Add(unitAbsent);
			headers.Root.Add(absent);
			#endregion

			#region Exclusifs
			WebResultUI.HeaderGroup exclusive=new WebResultUI.HeaderGroup(GestionWeb.GetWebWord(1128,webSession.SiteLanguage),ComptitorResultConstantes.EXCLUSIVE_HEADER_ID);
			exclusive.Add(new WebResultUI.Header(true,GestionWeb.GetWebWord(1852,webSession.SiteLanguage),ComptitorResultConstantes.ITEM_NUMBER_HEADER_ID));
			WebResultUI.Header unitExclusive=new WebResultUI.Header(GestionWeb.GetWebWord(unitId,webSession.SiteLanguage),ComptitorResultConstantes.UNIT_HEADER_ID);
			unitExclusive.Add(new WebResultUI.Header(true,GestionWeb.GetWebWord(1365,webSession.SiteLanguage),ComptitorResultConstantes.REFERENCE_MEDIA_HEADER_ID));
			unitExclusive.Add(new WebResultUI.Header(true,GestionWeb.GetWebWord(1366,webSession.SiteLanguage),ComptitorResultConstantes.COMPETITOR_MEDIA_HEADER_ID));
			exclusive.Add(unitExclusive);
			headers.Root.Add(exclusive);
			#endregion

			#endregion
			
			#region Création du tableau
			ResultTable resultTable=new ResultTable(nbLine,headers);
			Int64 nbCol=resultTable.ColumnsNumber-2;
			#endregion
			
			#region Initialisation des lignes
			Int64 levelLabelColIndex=resultTable.GetHeadersIndexInResultTable(ComptitorResultConstantes.LEVEL_HEADER_ID.ToString());
			advertiserLineIndex=resultTable.AddNewLine(LineType.level1);
			resultTable[advertiserLineIndex,levelLabelColIndex]=new CellLabel(GestionWeb.GetWebWord(1146,webSession.SiteLanguage));
			if(webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE]!=null){
				brandLineIndex=resultTable.AddNewLine(LineType.level1);
				resultTable[brandLineIndex,levelLabelColIndex]=new CellLabel(GestionWeb.GetWebWord(1149,webSession.SiteLanguage));
			}
			productLineIndex=resultTable.AddNewLine(LineType.level1);
			resultTable[productLineIndex,levelLabelColIndex]=new CellLabel(GestionWeb.GetWebWord(1164,webSession.SiteLanguage));
			sectorLineIndex=resultTable.AddNewLine(LineType.level1);
			resultTable[sectorLineIndex,levelLabelColIndex]=new CellLabel(GestionWeb.GetWebWord(1847,webSession.SiteLanguage));
			subsectorLineIndex=resultTable.AddNewLine(LineType.level1);
			resultTable[subsectorLineIndex,levelLabelColIndex]=new CellLabel(GestionWeb.GetWebWord(1848,webSession.SiteLanguage));
			groupLineIndex=resultTable.AddNewLine(LineType.level1);
			resultTable[groupLineIndex,levelLabelColIndex]=new CellLabel(GestionWeb.GetWebWord(1849,webSession.SiteLanguage));
			// Groupe d'Agence && Agence
			if(webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_MEDIA_AGENCY]!=null){
				agencyGroupLineIndex=resultTable.AddNewLine(LineType.level1);
				resultTable[agencyGroupLineIndex,levelLabelColIndex]=new CellLabel(GestionWeb.GetWebWord(1850,webSession.SiteLanguage));
				agencyLineIndex=resultTable.AddNewLine(LineType.level1);
				resultTable[agencyLineIndex,levelLabelColIndex]=new CellLabel(GestionWeb.GetWebWord(1851,webSession.SiteLanguage));
			}

			Int64 presentNumberColumnIndex = resultTable.GetHeadersIndexInResultTable(ComptitorResultConstantes.PRESENT_HEADER_ID+"-"+ComptitorResultConstantes.ITEM_NUMBER_HEADER_ID);
			Int64 absentNumberColumnIndex = resultTable.GetHeadersIndexInResultTable(ComptitorResultConstantes.ABSENT_HEADER_ID+"-"+ComptitorResultConstantes.ITEM_NUMBER_HEADER_ID);
			Int64 exclusiveNumberColumnIndex = resultTable.GetHeadersIndexInResultTable(ComptitorResultConstantes.EXCLUSIVE_HEADER_ID+"-"+ComptitorResultConstantes.ITEM_NUMBER_HEADER_ID);

			#region Initialisation des Nombres
			for(int i=0;i<nbLine;i++){
				resultTable[i,presentNumberColumnIndex]=new CellNumber(0.0);
				resultTable[i,absentNumberColumnIndex]=new CellNumber(0.0);
				resultTable[i,exclusiveNumberColumnIndex]=new CellNumber(0.0);
			}
			for(long i=0;i<nbLine;i++){
				for(long j=presentNumberColumnIndex+1;j<absentNumberColumnIndex;j++){
					resultTable[i,j]=cellUnitFactory.Get(0.0);
				}
				for(long j=absentNumberColumnIndex+1;j<exclusiveNumberColumnIndex;j++){
					resultTable[i,j]=cellUnitFactory.Get(0.0);
				}
				for(long j=exclusiveNumberColumnIndex+1;j<=nbCol;j++){
					resultTable[i,j]=cellUnitFactory.Get(0.0);
				}
			}
			#endregion

			#endregion
			
			
			if(dt!=null && !dt.Equals(System.DBNull.Value) && dt.Rows.Count>0){
				
				#region Sélection de Médias
				//Liste des supports de référence
				if(webSession.CompetitorUniversMedia[positionUnivers]!=null){
					mediaList=webSession.GetSelection((TreeNode) webSession.CompetitorUniversMedia[positionUnivers],CustomerConstantes.Right.type.mediaAccess);
					if(mediaList!=null && mediaList.Length>0){
						referenceUniversMedia = mediaList.Split(',');						
						positionUnivers++;
					}
					mediaList="";
				}
				//Liste des supports concurrents
				if(referenceUniversMedia!=null && referenceUniversMedia.Count>0){
					while(webSession.CompetitorUniversMedia[positionUnivers]!=null){
						mediaList+=webSession.GetSelection((TreeNode) webSession.CompetitorUniversMedia[positionUnivers],CustomerConstantes.Right.type.mediaAccess)+",";
						positionUnivers++;
					}
					if (mediaList.Length>0)competitorUniversMedia =  mediaList.Substring(0,mediaList.Length-1).Split(',');	
				}else return null;

				#endregion

				if(referenceUniversMedia!=null && referenceUniversMedia.Count>0 && competitorUniversMedia!=null && competitorUniversMedia.Count>0){
					
					advertisers = new ArrayList();
					products=new ArrayList();
					brands = new ArrayList();
					sectors=new ArrayList();
					subsectors = new ArrayList();
					groups=new ArrayList();
					agencyGroups =new ArrayList();
					agency=new ArrayList();

				
					#region Traitement des données
					//Activités publicitaire Annonceurs,marques,produits
					foreach(DataRow currentRow in dt.Rows){		
		
						//Activité publicitaire Annonceurs
						if(!advertisers.Contains(currentRow["id_advertiser"].ToString())){			
							expression="id_advertiser="+currentRow["id_advertiser"].ToString(); 						
							GetProductActivity(resultTable,dt,advertiserLineIndex,expression,sort,referenceUniversMedia,competitorUniversMedia);
							advertisers.Add(currentRow["id_advertiser"].ToString());
						}
						if(webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE]!=null){
							//Activité publicitaire marques
							if(currentRow["id_brand"]!=null && currentRow["id_brand"]!=System.DBNull.Value && !brands.Contains(currentRow["id_brand"].ToString())){					
								expression="id_brand="+currentRow["id_brand"].ToString(); 							
								GetProductActivity(resultTable,dt,brandLineIndex,expression,sort,referenceUniversMedia,competitorUniversMedia);
								brands.Add(currentRow["id_brand"].ToString());
							}
						}

						//Activité publicitaire produits
						if(currentRow["id_product"]!=null && currentRow["id_product"]!=System.DBNull.Value && !products.Contains(currentRow["id_product"].ToString())){					
							expression="id_product="+currentRow["id_product"].ToString(); 						
							GetProductActivity(resultTable,dt,productLineIndex,expression,sort,referenceUniversMedia,competitorUniversMedia);
							products.Add(currentRow["id_product"].ToString());
						}				
				
						//Activité publicitaire Famille
						if(currentRow["id_sector"]!=null && currentRow["id_sector"]!=System.DBNull.Value && !sectors.Contains(currentRow["id_sector"].ToString())){					
							expression="id_sector="+currentRow["id_sector"].ToString(); 					
							GetProductActivity(resultTable,dt,sectorLineIndex,expression,sort,referenceUniversMedia,competitorUniversMedia);
							sectors.Add(currentRow["id_sector"].ToString());
						}
						//Activité publicitaire Classe
						if(currentRow["id_subsector"]!=null && currentRow["id_subsector"]!=System.DBNull.Value && !subsectors.Contains(currentRow["id_subsector"].ToString())){					
							expression="id_subsector="+currentRow["id_subsector"].ToString(); 						
							GetProductActivity(resultTable,dt,subsectorLineIndex, expression,sort,referenceUniversMedia,competitorUniversMedia);
							subsectors.Add(currentRow["id_subsector"].ToString());
						}
						//Activité publicitaire Groupes
						if(currentRow["id_group_"]!=null && currentRow["id_group_"]!=System.DBNull.Value && !groups.Contains(currentRow["id_group_"].ToString())){					
							expression="id_group_="+currentRow["id_group_"].ToString(); 						
							GetProductActivity(resultTable,dt,groupLineIndex,expression,sort,referenceUniversMedia,competitorUniversMedia);
							groups.Add(currentRow["id_group_"].ToString());
						}
				
			
						if(webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_MEDIA_AGENCY]!=null){
							//activité publicitaire Groupes d'agences
							if(currentRow["ID_GROUP_ADVERTISING_AGENCY"]!=null && currentRow["ID_GROUP_ADVERTISING_AGENCY"]!=System.DBNull.Value && !agencyGroups.Contains(currentRow["ID_GROUP_ADVERTISING_AGENCY"].ToString())){					
								expression="ID_GROUP_ADVERTISING_AGENCY="+currentRow["ID_GROUP_ADVERTISING_AGENCY"].ToString(); 					
								GetProductActivity(resultTable,dt,agencyGroupLineIndex,expression,sort,referenceUniversMedia,competitorUniversMedia);
								agencyGroups.Add(currentRow["ID_GROUP_ADVERTISING_AGENCY"].ToString());
							}
				
							//activité publicitaire agence
							if(currentRow["ID_ADVERTISING_AGENCY"]!=null && currentRow["ID_ADVERTISING_AGENCY"]!=System.DBNull.Value && !agency.Contains(currentRow["ID_ADVERTISING_AGENCY"].ToString())){					
								expression="ID_ADVERTISING_AGENCY="+currentRow["ID_ADVERTISING_AGENCY"].ToString(); 						
								GetProductActivity(resultTable,dt,agencyLineIndex,expression,sort,referenceUniversMedia,competitorUniversMedia);
								agency.Add(currentRow["ID_ADVERTISING_AGENCY"].ToString());
							}
						}
					}
				
					#endregion

				}else return null;
			}else return null;
			

			return resultTable;
		}
		#endregion

		#region Méthodes internes

		#region Initialisation des indexes
		/// <summary>
		/// Initialisation des tableaux d'indexes et valeurs sur les groupes de séléection et médias
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="groupMediaTotalIndex">(out) Tableau d'indexes des groupes de sélection</param>
        /// <param name="subGroupMediaTotalIndex">Liste des index des sous groupes de sélection</param>
		/// <param name="nbUnivers">(out)Nombre d'univers</param>
        /// <param name="mediaIndex">(out Tableau d'indexes des médias</param>
		/// <param name="mediaListForLabelSearch">(out)Liste des codes des médias</param>
		/// <param name="maxIndex">(out)Index des colonnes maximum</param>
        /// <param name="dtMedia">Liste des média avec le niveau de détail colonne correspondant</param>
        internal static void InitIndexAndValues(WebSession webSession, WebCommon.Results.SelectionGroup[] groupMediaTotalIndex, WebCommon.Results.SelectionSubGroup[] subGroupMediaTotalIndex, ref int nbUnivers, Hashtable mediaIndex, ref string mediaListForLabelSearch, ref int maxIndex, DataTable dtMedia) {
			
			#region Variables
			string tmp="";
			int positionUnivers=1;
            int positionSubGroup=2;
            int subGroupCount = 0;
			string[] mediaList;
            Hashtable mediaSubGroupId = new Hashtable();
            ArrayList columnDetailLevelList = new ArrayList();
			#endregion

			#region Initialisation des variables
			maxIndex=FrameWorkResultConstantes.CompetitorAlert.FIRST_MEDIA_INDEX;
			#endregion
			
			// On suppose que les supports sont triés en order croissant par sous groupe
			while(webSession.CompetitorUniversMedia[positionUnivers]!=null){
				// Chargement de la liste de support (média)
				tmp=webSession.GetSelection((TreeNode) webSession.CompetitorUniversMedia[positionUnivers],CustomerConstantes.Right.type.mediaAccess);
				mediaList=tmp.Split(',');
                columnDetailLevelList = new ArrayList();

                // Chargement de la liste du niveau de détail colonne
                foreach (string media in mediaList) {
                    foreach (DataRow row in dtMedia.Rows) {
                        if (media.Equals(row["id_media"].ToString())) {
                            if (!columnDetailLevelList.Contains(row["columnDetailLevel"].ToString())) {
                                columnDetailLevelList.Add(row["columnDetailLevel"].ToString());
                                subGroupMediaTotalIndex[positionSubGroup] = new WebCommon.Results.SelectionSubGroup(positionSubGroup);
                                subGroupMediaTotalIndex[positionSubGroup].DataBaseId = int.Parse(row["columnDetailLevel"].ToString());
                                subGroupMediaTotalIndex[positionSubGroup].ParentId = positionUnivers;
                                subGroupMediaTotalIndex[positionSubGroup].SetItemsNumber = 0;
                                subGroupMediaTotalIndex[positionSubGroup].IndexInResultTable=0;
                                mediaSubGroupId[Int64.Parse(media)] = positionSubGroup;
                                positionSubGroup++;
                                subGroupCount++;
                                mediaListForLabelSearch += row["columnDetailLevel"].ToString() + ",";
                            }
                            else { 
                                foreach(WebCommon.Results.SelectionSubGroup subGroup in subGroupMediaTotalIndex)
                                    if (subGroup != null) {
                                        if (subGroup.DataBaseId == int.Parse(row["columnDetailLevel"].ToString())) {
                                            if (subGroup.Count == 0)
                                                subGroup.SetItemsNumber = 2;
                                            else
                                                subGroup.SetItemsNumber = subGroup.Count + 1;
                                            mediaSubGroupId[Int64.Parse(media)] = subGroup.Id;
                                        }
                                    }
                            }
                        }
                    }
                }

                // Définition du groupe
                groupMediaTotalIndex[positionUnivers] = new WebCommon.Results.SelectionGroup(positionUnivers);

                // Le groupe contient plus de 1 éléments
                if (subGroupCount > 1) {
                    groupMediaTotalIndex[positionUnivers].IndexInResultTable = maxIndex;
                    groupMediaTotalIndex[positionUnivers].SetItemsNumber = subGroupCount;
                    maxIndex++;
                    //nbSubTotal++;
                }
                else {
                    groupMediaTotalIndex[positionUnivers].IndexInResultTable = maxIndex;
                    groupMediaTotalIndex[positionUnivers].SetItemsNumber = 0;
                }

                // Pour les sous Groupes
                foreach (WebCommon.Results.SelectionSubGroup subGroup in subGroupMediaTotalIndex) {
                    if (subGroup != null) {
                        if (subGroup.IndexInResultTable == 0) {
                            subGroup.IndexInResultTable = maxIndex;
                            maxIndex++;
                        }
                    }
                }

                // Indexes des média (support)
                foreach (string currentMedia in mediaList) {
                    mediaIndex[Int64.Parse(currentMedia)] = new WebCommon.Results.GroupItemForTableResult(Int64.Parse(currentMedia), (int)mediaSubGroupId[Int64.Parse(currentMedia)], maxIndex);
                }

                positionUnivers++;
                subGroupCount = 0;
			}
			
            nbUnivers=positionUnivers--;
			mediaListForLabelSearch=mediaListForLabelSearch.Substring(0,mediaListForLabelSearch.Length-1);
		}
		#endregion

		#region Formattage d'un tableau de résultat
		/// <summary>
		/// Formattage d'un tableau de résultat à partir d'un tableau de données
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="tabData">Table de données</param>
		/// <param name="nbLineInTabData">Nombre de ligne dans le tableau</param>
		/// <param name="groupMediaTotalIndex">Liste des groupes de sélection</param>
        /// <param name="subGroupMediaTotalIndex">Liste des sous groupes de sélection</param>
		/// <param name="mediaIndex">Liste des Média</param>
		/// <param name="mediaListForLabelSearch">Liste des médias</param>
		/// <param name="nbUnivers">Nombre d'univers</param>
		/// <returns>Tableau de résultat</returns>
		private static ResultTable GetResultTable(WebSession webSession,object[,] tabData,long nbLineInTabData,WebCommon.Results.SelectionGroup[] groupMediaTotalIndex, WebCommon.Results.SelectionSubGroup[] subGroupMediaTotalIndex, Hashtable mediaIndex,string mediaListForLabelSearch,int nbUnivers){

			#region Variables
			long decal=0;
			string[] mediaList;
			Int64 oldIdL1=-1;
			Int64 oldIdL2=-1;
			Int64 oldIdL3=-1;
            //long pdmIndex=0;
			long currentLine;
			long currentLineInTabResult;
			long k;
			ArrayList l2Indexes=new ArrayList();
			ArrayList l1Indexes=new ArrayList();
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)webSession.GenericColumnDetailLevel.Levels[0];
			#endregion 

			#region Aucune données
			if(nbLineInTabData == 0){
				return null;
			}
			#endregion
            
			#region Calcul des PDM ?
			bool computePDM=false;
			if(webSession.Percentage) computePDM=true;
			#endregion

            #region Obtention du vehicle
            string vehicleSelection=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerConstantes.Right.type.vehicleAccess);
            if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw(new WebExceptions.CompetitorRulesException("La sélection de médias est incorrecte"));
			DBClassificationConstantes.Vehicles.names vehicle=(DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
			
            #endregion

			#region Headers
			// Ajout de la colonne Produit
			Headers headers=new Headers();
			headers.Root.Add(new WebResultUI.Header(true,GestionWeb.GetWebWord(1164,webSession.SiteLanguage),ComptitorResultConstantes.LEVEL_HEADER_ID));
			long startDataColIndex=1;
			long startDataColIndexInit=1;

			
            // Ajout Création ?
			bool showCreative=false;
            //A vérifier Création où version
			if(  webSession.CustomerLogin.FlagsList[DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG]!=null &&
				(webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser)||
				webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.product))){
				headers.Root.Add(new HeaderCreative(false,GestionWeb.GetWebWord(1994,webSession.SiteLanguage),ComptitorResultConstantes.CREATIVE_HEADER_ID));
				showCreative=true;
				startDataColIndex++;
				startDataColIndexInit++;
			}

            // Ajout Insertions ?
            bool showInsertions = false;
            if ((webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser) ||
                webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.product))) {
                headers.Root.Add(new HeaderInsertions(false, GestionWeb.GetWebWord(2245, webSession.SiteLanguage), ComptitorResultConstantes.INSERTION_HEADER_ID));
                showInsertions = true;
                startDataColIndex++;
                startDataColIndexInit++;
            }

			// Ajout plan media ?
			bool showMediaSchedule=false;
			if(webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser)||
				webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.product)||
				webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.brand)||
				webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.holdingCompany)||
				webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.sector)||
				webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.subSector)||
				webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.group)
				){
				headers.Root.Add(new HeaderMediaSchedule(false,GestionWeb.GetWebWord(150,webSession.SiteLanguage),ComptitorResultConstantes.MEDIA_SCHEDULE_HEADER_ID));
				showMediaSchedule=true;
				startDataColIndex++;
				startDataColIndexInit++;
			}


			//Colonne total s'il ya plusieurs groupes:
			mediaList=mediaListForLabelSearch.Split(',');
			bool showTotal=false;
			if(webSession.CompetitorUniversMedia.Count>1 || mediaList.Length>1){
				startDataColIndexInit++;
				showTotal=true;
				headers.Root.Add(new WebResultUI.Header(true,GestionWeb.GetWebWord(805,webSession.SiteLanguage),ComptitorResultConstantes.TOTAL_HEADER_ID));
			}
			// Chargement des libellés de colonnes
            ClassificationDB.MediaBranch.PartialMediaListDataAccess mediaLabelList = null;
            ClassificationDB.MediaBranch.PartialCategoryListDataAccess categoryLabelList = null;
            ClassificationDB.MediaBranch.PartialMediaSellerListDataAccess mediaSellerLabelList = null;
            ClassificationDB.MediaBranch.PartialTitleListDataAccess titleLabelList = null;
            ClassificationDB.MediaBranch.PartialInterestCenterListDataAccess interestCenterLabelList = null;
            
            switch (columnDetailLevel.Id) {

                case DetailLevelItemInformation.Levels.media:
                    mediaLabelList = new ClassificationDB.MediaBranch.PartialMediaListDataAccess(mediaListForLabelSearch, webSession.SiteLanguage, webSession.CustomerLogin.Connection);
                    break;
                case DetailLevelItemInformation.Levels.category:
                    categoryLabelList = new ClassificationDB.MediaBranch.PartialCategoryListDataAccess(mediaListForLabelSearch, webSession.SiteLanguage, webSession.CustomerLogin.Connection);
                    break;
                case DetailLevelItemInformation.Levels.mediaSeller:
                    mediaSellerLabelList = new ClassificationDB.MediaBranch.PartialMediaSellerListDataAccess(mediaListForLabelSearch, webSession.SiteLanguage, webSession.CustomerLogin.Connection);
                    break;
                case DetailLevelItemInformation.Levels.title:
                    titleLabelList = new ClassificationDB.MediaBranch.PartialTitleListDataAccess(mediaListForLabelSearch, webSession.SiteLanguage, webSession.CustomerLogin.Connection);
                    break;
                case DetailLevelItemInformation.Levels.interestCenter:
                    interestCenterLabelList = new TNS.AdExpress.Classification.DataAccess.MediaBranch.PartialInterestCenterListDataAccess(mediaListForLabelSearch, webSession.SiteLanguage, webSession.CustomerLogin.Connection);
                    break;

            }

			HeaderGroup headerGroupTmp=null;
			for(int m=1;m<groupMediaTotalIndex.GetLength(0);m++){
				if(groupMediaTotalIndex[m]!=null){
					//Supports de référence ou concurents 1365
					if(m==1)headerGroupTmp=new WebResultUI.HeaderGroup(GestionWeb.GetWebWord(1365,webSession.SiteLanguage),true,ComptitorResultConstantes.START_ID_GROUP+m);
					else headerGroupTmp=new WebResultUI.HeaderGroup(GestionWeb.GetWebWord(1366,webSession.SiteLanguage)+" "+(m-1).ToString(),true,ComptitorResultConstantes.START_ID_GROUP+m);
					if(groupMediaTotalIndex[m].Count>1 && webSession.CompetitorUniversMedia.Count>1)headerGroupTmp.AddSubTotal(true,GestionWeb.GetWebWord(1102,webSession.SiteLanguage),ComptitorResultConstantes.SUB_TOTAL_HEADER_ID);

                    foreach (WebCommon.Results.SelectionSubGroup subGroup in subGroupMediaTotalIndex) {
                        if (subGroup != null) {
                            if(subGroup.ParentId == m)
                                switch (columnDetailLevel.Id) {

                                    case DetailLevelItemInformation.Levels.media:
                                        headerGroupTmp.Add(new WebResultUI.Header(true, mediaLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                        break;
                                    case DetailLevelItemInformation.Levels.category:
                                        headerGroupTmp.Add(new WebResultUI.Header(true, categoryLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                        break;
                                    case DetailLevelItemInformation.Levels.mediaSeller:
                                        headerGroupTmp.Add(new WebResultUI.Header(true, mediaSellerLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                        break;
                                    case DetailLevelItemInformation.Levels.title:
                                        headerGroupTmp.Add(new WebResultUI.Header(true, titleLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                        break;
                                    case DetailLevelItemInformation.Levels.interestCenter:
                                        headerGroupTmp.Add(new WebResultUI.Header(true, interestCenterLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                        break;

                                }
                        }
                    }

					headers.Root.Add(headerGroupTmp);
				}
			}
			#endregion

			#region Déclaration du tableau de résultat
			
			long nbLine=GetNbLineFromPreformatedTableToResultTable(tabData)+1;//FrameWorkResultConstantes.DynamicAnalysis.FIRST_LINE_RESULT_INDEX;
			//long nbLine=1;
			WebResultUI.ResultTable resultTable=new WebResultUI.ResultTable(nbLine,headers);
			long nbCol=resultTable.ColumnsNumber-2;
			long totalColIndex=-1;
			if(showTotal)totalColIndex=resultTable.GetHeadersIndexInResultTable(ComptitorResultConstantes.TOTAL_HEADER_ID.ToString());
			long levelLabelColIndex=resultTable.GetHeadersIndexInResultTable(ComptitorResultConstantes.LEVEL_HEADER_ID.ToString());
			long mediaScheduleColIndex=resultTable.GetHeadersIndexInResultTable(ComptitorResultConstantes.MEDIA_SCHEDULE_HEADER_ID.ToString());
			long creativeColIndex=resultTable.GetHeadersIndexInResultTable(ComptitorResultConstantes.CREATIVE_HEADER_ID.ToString());
            long insertionsColIndex = resultTable.GetHeadersIndexInResultTable(ComptitorResultConstantes.INSERTION_HEADER_ID.ToString());
			#endregion

			#region Sélection de l'unité
			CellUnitFactory cellUnitFactory=webSession.GetCellUnitFactory();
			#endregion

			#region Total
			long nbColInTabData=tabData.GetLength(0);
			Hashtable subTotalWithOneMediaIndexes=GetSubTotalWithOneMediaIndexes(webSession,nbColInTabData,resultTable,groupMediaTotalIndex);
			//Hashtable subTotalIndexes=GetSubTotalIndexes(resultTable,groupMediaTotalIndex);
			startDataColIndex++;
			startDataColIndexInit++;
			currentLineInTabResult= resultTable.AddNewLine(TNS.FrameWork.WebResultUI.LineType.total);
			//Libellé du total
			resultTable[currentLineInTabResult,levelLabelColIndex]=new CellLevel(-1,GestionWeb.GetWebWord(805,webSession.SiteLanguage),0,currentLineInTabResult);
			CellLevel currentCellLevel0=(CellLevel)resultTable[currentLineInTabResult,levelLabelColIndex];
            if (showCreative)resultTable[currentLineInTabResult, creativeColIndex] = new CellOneLevelCreativesLink(currentCellLevel0, webSession, webSession.GenericProductDetailLevel);
            if(showInsertions) resultTable[currentLineInTabResult,insertionsColIndex] = new CellOneLevelInsertionsLink(currentCellLevel0,webSession,webSession.GenericProductDetailLevel);
			if(showMediaSchedule)resultTable[currentLineInTabResult,mediaScheduleColIndex]= new CellMediaScheduleLink(currentCellLevel0,webSession);
			if(showTotal && !computePDM)resultTable[currentLineInTabResult,totalColIndex]=cellUnitFactory.Get(0.0);
			if(showTotal && computePDM)resultTable[currentLineInTabResult,totalColIndex]=new CellPDM(0.0,null); 
			for(k=startDataColIndexInit;k<=nbCol;k++){
				if(computePDM)resultTable[currentLineInTabResult,k]=new CellPDM(0.0,(CellUnit)resultTable[currentLineInTabResult,totalColIndex]);
				else resultTable[currentLineInTabResult,k]=cellUnitFactory.Get(0.0);
			}
			#endregion

			#region Tableau de résultat
			oldIdL1=-1;
			oldIdL2=-1;
			oldIdL3=-1;
			AdExpressCellLevel currentCellLevel1=null;
			AdExpressCellLevel currentCellLevel2=null;
			AdExpressCellLevel currentCellLevel3=null;
			long currentL1Index=-1;
			long currentL2Index=-1;
			long currentL3Index=-1;
			
			for(currentLine=0;currentLine<nbLineInTabData;currentLine++){

				#region On change de niveau L1
				if(tabData[ComptitorResultConstantes.IDL1_INDEX,currentLine]!=null && (Int64)tabData[ComptitorResultConstantes.IDL1_INDEX,currentLine]!=oldIdL1){
					currentLineInTabResult=resultTable.AddNewLine(LineType.level1);
					
					#region Totaux et sous Totaux à 0 et media
					if(showTotal && !computePDM)resultTable[currentLineInTabResult,totalColIndex]=cellUnitFactory.Get(0.0);
					if(showTotal && computePDM)resultTable[currentLineInTabResult,totalColIndex]=new CellPDM(0.0,null);
					for(k=startDataColIndexInit;k<=nbCol;k++){
						if(computePDM)resultTable[currentLineInTabResult,k]=new CellPDM(0.0,(CellUnit)resultTable[currentLineInTabResult,totalColIndex]);
						else resultTable[currentLineInTabResult,k]=cellUnitFactory.Get(0.0);
					}
					#endregion

					oldIdL1=(Int64)tabData[ComptitorResultConstantes.IDL1_INDEX,currentLine];
					resultTable[currentLineInTabResult,levelLabelColIndex]= new AdExpressCellLevel((Int64)tabData[ComptitorResultConstantes.IDL1_INDEX,currentLine],(string)tabData[ComptitorResultConstantes.LABELL1_INDEX,currentLine],currentCellLevel0,1,currentLineInTabResult,webSession);		
					currentCellLevel1=(AdExpressCellLevel)resultTable[currentLineInTabResult,levelLabelColIndex];
                    if(showCreative) resultTable[currentLineInTabResult,creativeColIndex] = new CellOneLevelCreativesLink(currentCellLevel1,webSession,webSession.GenericProductDetailLevel);
                    if(showInsertions) resultTable[currentLineInTabResult,insertionsColIndex] = new CellOneLevelInsertionsLink(currentCellLevel1,webSession,webSession.GenericProductDetailLevel);
					if(showMediaSchedule)resultTable[currentLineInTabResult,mediaScheduleColIndex]= new CellMediaScheduleLink(currentCellLevel1,webSession);
					currentL1Index=currentLineInTabResult;
					oldIdL2=oldIdL3=-1;

					#region GAD
					if(webSession.GenericProductDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser)==1){
						if(tabData[ComptitorResultConstantes.ADDRESS_COLUMN_INDEX,currentLine]!=null){
							((CellLevel)resultTable[currentLineInTabResult,levelLabelColIndex]).AddressId=(Int64)tabData[ComptitorResultConstantes.ADDRESS_COLUMN_INDEX,currentLine];
						}
					}
					#endregion
				}
				#endregion

				#region On change de niveau L2
				if(tabData[ComptitorResultConstantes.IDL2_INDEX,currentLine]!=null && (Int64)tabData[ComptitorResultConstantes.IDL2_INDEX,currentLine]!=oldIdL2){
					currentLineInTabResult=resultTable.AddNewLine(LineType.level2);

					#region Totaux et sous Totaux à 0 et media
					if(showTotal && !computePDM)resultTable[currentLineInTabResult,totalColIndex]=cellUnitFactory.Get(0.0);
					if(showTotal && computePDM)resultTable[currentLineInTabResult,totalColIndex]=new CellPDM(0.0,null);
					for(k=startDataColIndexInit;k<=nbCol;k++){
						if(computePDM)resultTable[currentLineInTabResult,k]=new CellPDM(0.0,(CellUnit)resultTable[currentLineInTabResult,totalColIndex]);
						else resultTable[currentLineInTabResult,k]=cellUnitFactory.Get(0.0);
					}
					#endregion

					oldIdL2=(Int64)tabData[ComptitorResultConstantes.IDL2_INDEX,currentLine];
					resultTable[currentLineInTabResult,levelLabelColIndex]= new AdExpressCellLevel((Int64)tabData[ComptitorResultConstantes.IDL2_INDEX,currentLine],(string)tabData[ComptitorResultConstantes.LABELL2_INDEX,currentLine],currentCellLevel1,2,currentLineInTabResult,webSession);		
					currentCellLevel2=(AdExpressCellLevel)resultTable[currentLineInTabResult,levelLabelColIndex];
                    if(showCreative) resultTable[currentLineInTabResult,creativeColIndex] = new CellOneLevelCreativesLink(currentCellLevel2,webSession,webSession.GenericProductDetailLevel);
                    if(showInsertions) resultTable[currentLineInTabResult,insertionsColIndex] = new CellOneLevelInsertionsLink(currentCellLevel2,webSession,webSession.GenericProductDetailLevel);
					if(showMediaSchedule)resultTable[currentLineInTabResult,mediaScheduleColIndex]= new CellMediaScheduleLink(currentCellLevel2,webSession);
					currentL2Index=currentLineInTabResult;
					oldIdL3=-1;

					#region GAD
					if(webSession.GenericProductDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser)==2){
						if(tabData[ComptitorResultConstantes.ADDRESS_COLUMN_INDEX,currentLine]!=null){
							((CellLevel)resultTable[currentLineInTabResult,levelLabelColIndex]).AddressId=(Int64)tabData[ComptitorResultConstantes.ADDRESS_COLUMN_INDEX,currentLine];
						}
					}
					#endregion
				}
				#endregion

				#region On change de niveau L3
				if(tabData[ComptitorResultConstantes.IDL3_INDEX,currentLine]!=null && (Int64)tabData[ComptitorResultConstantes.IDL3_INDEX,currentLine]!=oldIdL3){
					currentLineInTabResult=resultTable.AddNewLine(LineType.level3);				

					#region Totaux et sous Totaux à 0 et media
					if(showTotal && !computePDM)resultTable[currentLineInTabResult,totalColIndex]=cellUnitFactory.Get(0.0);
					if(showTotal && computePDM)resultTable[currentLineInTabResult,totalColIndex]=new CellPDM(0.0,null);
					for(k=startDataColIndexInit;k<=nbCol;k++){
						if(computePDM)resultTable[currentLineInTabResult,k]=new CellPDM(0.0,(CellUnit)resultTable[currentLineInTabResult,totalColIndex]);
						else resultTable[currentLineInTabResult,k]=cellUnitFactory.Get(0.0);
					}				
					#endregion

					oldIdL3=(Int64)tabData[ComptitorResultConstantes.IDL3_INDEX,currentLine];
					resultTable[currentLineInTabResult,levelLabelColIndex]= new AdExpressCellLevel((Int64)tabData[ComptitorResultConstantes.IDL3_INDEX,currentLine],(string)tabData[ComptitorResultConstantes.LABELL3_INDEX,currentLine],currentCellLevel2,3,currentLineInTabResult,webSession);		
					currentCellLevel3=(AdExpressCellLevel)resultTable[currentLineInTabResult,levelLabelColIndex];
                    if(showCreative) resultTable[currentLineInTabResult,creativeColIndex] = new CellOneLevelCreativesLink(currentCellLevel3,webSession,webSession.GenericProductDetailLevel);
                    if(showInsertions) resultTable[currentLineInTabResult,insertionsColIndex] = new CellOneLevelInsertionsLink(currentCellLevel3,webSession,webSession.GenericProductDetailLevel);
					if(showMediaSchedule)resultTable[currentLineInTabResult,mediaScheduleColIndex]= new CellMediaScheduleLink(currentCellLevel3,webSession);
					currentL3Index=currentLineInTabResult;

					#region GAD
					if(webSession.GenericProductDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser)==3){
						if(tabData[ComptitorResultConstantes.ADDRESS_COLUMN_INDEX,currentLine]!=null){
							((CellLevel)resultTable[currentLineInTabResult,levelLabelColIndex]).AddressId=(Int64)tabData[ComptitorResultConstantes.ADDRESS_COLUMN_INDEX,currentLine];
						}
					}
					#endregion
				}
				#endregion
			
				// On copy la ligne et on l'ajoute aux totaux
				decal=0;
				for(k=FrameWorkResultConstantes.CompetitorAlert.TOTAL_INDEX;k<nbColInTabData;k++){
					//On affiche pas la cellule si:
					// Dans data c'est un sous total et que le groupe n'a qu'un seul éléments
					if(!subTotalWithOneMediaIndexes.ContainsKey(k))
						resultTable.AffectValueAndAddToHierarchy(levelLabelColIndex,currentLineInTabResult,startDataColIndex-decal+k-(long.Parse((FrameWorkResultConstantes.CompetitorAlert.TOTAL_INDEX).ToString())),(double)tabData[k,currentLine]);
					else
						decal++;
				}

			}
			#endregion

			return(resultTable);
		}
		
		
		#endregion

		#region Calcul du nombre de ligne d'un tableau préformaté
		/// <summary>
		/// Obtient le nombre de ligne du tableau de résultat à partir d'un tableau préformaté
		/// </summary>
		/// <param name="tabData">Tableau préformaté</param>
		/// <returns>Nombre de ligne du tableau de résultat</returns>
		private static long GetNbLineFromPreformatedTableToResultTable(object[,] tabData){

			#region Variables
			long nbLine=0;
			long k;
			Int64 oldIdL1=-1;
			Int64 oldIdL2=-1;
			Int64 oldIdL3=-1;
			#endregion

			for(k=0;k<tabData.GetLength(1);k++){
				// Somme des L1
				if(tabData[FrameWorkResultConstantes.CompetitorAlert.IDL1_INDEX,k]!=null && (Int64)tabData[FrameWorkResultConstantes.CompetitorAlert.IDL1_INDEX,k]!=oldIdL1){
					oldIdL1=(Int64)tabData[FrameWorkResultConstantes.CompetitorAlert.IDL1_INDEX,k];
					oldIdL3=oldIdL2=-1;
					nbLine++;
				}

				// Somme des L2
				if(tabData[FrameWorkResultConstantes.CompetitorAlert.IDL2_INDEX,k]!=null && (Int64)tabData[FrameWorkResultConstantes.CompetitorAlert.IDL2_INDEX,k]!=oldIdL2){
					oldIdL2=(Int64)tabData[FrameWorkResultConstantes.CompetitorAlert.IDL2_INDEX,k];
					oldIdL3=-1;
					nbLine++;
				}

				// Somme des L3
				if(tabData[FrameWorkResultConstantes.CompetitorAlert.IDL3_INDEX,k]!=null && (Int64)tabData[FrameWorkResultConstantes.CompetitorAlert.IDL3_INDEX,k]!=oldIdL3){
					oldIdL3=(Int64)tabData[FrameWorkResultConstantes.CompetitorAlert.IDL3_INDEX,k];
					nbLine++;
				}

			}
			return(nbLine);
		}
		#endregion

		#region Tableau Préformaté
		/// <summary>
		/// Obtient le tableau de résultat préformaté pour une alerte Concurrentielle
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="groupMediaTotalIndex">Liste des index des groupes de sélection</param>
        /// <param name="subGroupMediaTotalIndex">Liste des index des sous groupes de sélection</param>
		/// <param name="mediaIndex">Liste des index des médias</param>
		/// <param name="nbCol">Nombre de colonnes du tableau de résultat</param>
		/// <param name="nbLineInNewTable">(out) Nombre de ligne du tableau de résultat</param>
        /// <param name="nbUnivers">(out)Nombre d'univers</param>
        /// <param name="mediaListForLabelSearch">(out)Liste des codes des médias</param>
		/// <returns>Tableau de résultat</returns>
        private static object[,] GetPreformatedTable(WebSession webSession, WebCommon.Results.SelectionGroup[] groupMediaTotalIndex, WebCommon.Results.SelectionSubGroup[] subGroupMediaTotalIndex, Hashtable mediaIndex, ref int nbCol, ref long nbLineInNewTable, ref int nbUnivers, ref string mediaListForLabelSearch) {
			
			#region Variables
			double unit;
			long oldIdL1=-1;
			long oldIdL2=-1;
			long oldIdL3=-1;
			Int64 currentLine=-1;
			int k;
			bool changeLine=false;
			#endregion
			
			#region Formattage des dates 
			string periodBeginning = GetDateBegin(webSession);
			string periodEnd = GetDateEnd(webSession);
			#endregion

			#region Sélection du vehicle
			string vehicleSelection=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerConstantes.Right.type.vehicleAccess);
			DBClassificationConstantes.Vehicles.names vehicleName=(DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
			if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw(new WebExceptions.CompetitorRulesException("La sélection de médias est incorrecte"));
			#endregion

			#region Chargement des données à partir de la base	
			DataSet ds=null;
            DataSet dsMedia = null;
//			Module currentModuleDescription=ModulesList.GetModule(webSession.CurrentModule);
			
			#region Choix de l'accès des données en fonction du module
			//ds=CompetitorDataAccess.GetData(webSession,vehicleName, periodBeginning, periodEnd);
            ds = CompetitorDataAccess.GetGenericData(webSession, vehicleName);
            dsMedia = CompetitorDataAccess.GetMediaColumnDetailLevelList(webSession);
			#endregion
			
			DataTable dt=ds.Tables[0];
            DataTable dtMedia = dsMedia.Tables[0];

            if (dt.Rows.Count == 0) {
                return null;
            }

            #region Tableaux d'index
            InitIndexAndValues(webSession, groupMediaTotalIndex, subGroupMediaTotalIndex, ref nbUnivers, mediaIndex, ref mediaListForLabelSearch, ref nbCol, dtMedia);
            #endregion

			#endregion
			
			#region Déclaration du tableau de résultat
			long nbline=dt.Rows.Count;
			object[,] tabResult=new object[nbCol,dt.Rows.Count];
			#endregion

			#region Tableau de résultat



			foreach(DataRow currentRow in dt.Rows){
				//idMedia=(Int64)currentRow["id_media"];
				if(webSession.GenericProductDetailLevel.GetIdValue(currentRow,1)>0 && webSession.GenericProductDetailLevel.GetIdValue(currentRow,1)!=oldIdL1)changeLine=true;
				if(!changeLine && webSession.GenericProductDetailLevel.GetIdValue(currentRow,2)>0 && webSession.GenericProductDetailLevel.GetIdValue(currentRow,2)!=oldIdL2)changeLine=true;
				if(!changeLine && webSession.GenericProductDetailLevel.GetIdValue(currentRow,3)>0 && webSession.GenericProductDetailLevel.GetIdValue(currentRow,3)!=oldIdL3)changeLine=true;

				#region On change de ligne
				if(changeLine){
					currentLine++;
					// Ecriture de L1 ?
					if(webSession.GenericProductDetailLevel.GetIdValue(currentRow,1)>0){
						oldIdL1=webSession.GenericProductDetailLevel.GetIdValue(currentRow,1);
						tabResult[FrameWorkResultConstantes.CompetitorAlert.IDL1_INDEX,currentLine]=oldIdL1;
						tabResult[FrameWorkResultConstantes.CompetitorAlert.LABELL1_INDEX,currentLine]=webSession.GenericProductDetailLevel.GetLabelValue(currentRow,1);
					}
					// Ecriture de L2 ?
					if(webSession.GenericProductDetailLevel.GetIdValue(currentRow,2)>0){
						oldIdL2=webSession.GenericProductDetailLevel.GetIdValue(currentRow,2);
						tabResult[FrameWorkResultConstantes.CompetitorAlert.IDL2_INDEX,currentLine]=oldIdL2;
						tabResult[FrameWorkResultConstantes.CompetitorAlert.LABELL2_INDEX,currentLine]=webSession.GenericProductDetailLevel.GetLabelValue(currentRow,2);
					}
					// Ecriture de L3 ?
					if(webSession.GenericProductDetailLevel.GetIdValue(currentRow,3)>0){
						oldIdL3=webSession.GenericProductDetailLevel.GetIdValue(currentRow,3);
						tabResult[FrameWorkResultConstantes.CompetitorAlert.IDL3_INDEX,currentLine]=oldIdL3;
						tabResult[FrameWorkResultConstantes.CompetitorAlert.LABELL3_INDEX,currentLine]=webSession.GenericProductDetailLevel.GetLabelValue(currentRow,3);
					}
					// Totaux, sous Totaux et médias à 0
					for(k=FrameWorkResultConstantes.CompetitorAlert.TOTAL_INDEX;k<nbCol;k++){
						tabResult[k,currentLine]=(double) 0.0;
					}
					
					try{
						if(currentRow["id_address"]!=null)tabResult[FrameWorkResultConstantes.CompetitorAlert.ADDRESS_COLUMN_INDEX,currentLine]=Int64.Parse(currentRow["id_address"].ToString());
					}catch(Exception){
					
					}

					changeLine=false;
				}
				#endregion


				
				unit=double.Parse(currentRow["unit"].ToString());
				// Si l'unité est une page,on divise l'unité par 1000
//				if(webSession.Unit==WebConstantes.CustomerSessions.Unit.pages){					
////					unit=unit/1000;	
//					unit=unit;	
//				}

				// Ecriture du résultat du média
                tabResult[(long)subGroupMediaTotalIndex[((WebCommon.Results.GroupItem)mediaIndex[(Int64)currentRow["id_media"]]).GroupNumber].IndexInResultTable, currentLine] = (double)tabResult[(long)subGroupMediaTotalIndex[((WebCommon.Results.GroupItemForTableResult)mediaIndex[(Int64)currentRow["id_media"]]).GroupNumber].IndexInResultTable, currentLine] + unit;
                
                // Ecriture du résultat du sous total (somme)
                if (groupMediaTotalIndex[(long)subGroupMediaTotalIndex[((WebCommon.Results.GroupItem)mediaIndex[(Int64)currentRow["id_media"]]).GroupNumber].ParentId].Count > 1) {
                    tabResult[(long)groupMediaTotalIndex[(long)subGroupMediaTotalIndex[((WebCommon.Results.GroupItem)mediaIndex[(Int64)currentRow["id_media"]]).GroupNumber].ParentId].IndexInResultTable, currentLine] = (double)tabResult[(long)groupMediaTotalIndex[(long)subGroupMediaTotalIndex[((WebCommon.Results.GroupItem)mediaIndex[(Int64)currentRow["id_media"]]).GroupNumber].ParentId].IndexInResultTable, currentLine] + unit;
                }

                // Ecriture du résultat du total (somme)
				tabResult[FrameWorkResultConstantes.CompetitorAlert.TOTAL_INDEX,currentLine]=(double)tabResult[FrameWorkResultConstantes.CompetitorAlert.TOTAL_INDEX,currentLine]+unit;
			}
			#endregion
			
			#region Debug: Voir le tableau 
//			int i,j;
//			string HTML="<html><table><tr>";
//			for(i=0;i<=currentLine;i++){
//				for(j=0;j<nbCol;j++){
//					if(tabResult[j,i]!=null)HTML+="<td>"+tabResult[j,i].ToString()+"</td>";
//					else HTML+="<td>&nbsp;</td>";
//				}
//				HTML+="</tr><tr>";
//			}
//			HTML+="</tr></table></html>";
			#endregion

			nbLineInNewTable=currentLine+1;
			return(tabResult);
		}
		#endregion

		#region Identifiant des éléments de la nomenclature produit
		/// <summary>
		/// Obtient l'identifiant du détail période de niveau 1
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dr">Ligne de la table de résultat</param>
		/// <returns>Identifiant</returns>
		internal static Int64 GetL1Id(WebSession webSession,DataRow dr)
		{
			try{
				switch(webSession.PreformatedProductDetail){
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sector:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsector:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
						case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorHoldingCompanyAdvertiser:
						return(Int64.Parse(dr["id_sector"].ToString()));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product:					
						return(Int64.Parse(dr["id_product"].ToString()));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
						return(Int64.Parse(dr["id_advertiser"].ToString()));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
						return(Int64.Parse(dr["id_group_"].ToString()));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
						return(Int64.Parse(dr["ID_GROUP_ADVERTISING_AGENCY"].ToString()));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
						return(Int64.Parse(dr["ID_ADVERTISING_AGENCY"].ToString()));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct:
						return(Int64.Parse(dr["ID_GROUP_ADVERTISING_AGENCY"].ToString()));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompany:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserBrand:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserProduct:
						return(Int64.Parse(dr["id_holding_company"].ToString()));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentBrand:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserBrand:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserProduct:
						return(Int64.Parse(dr["id_segment"].ToString()));														
					default:
						return(-1);
				}
			}
			catch(Exception){
				return(-1);
			}
		}

		/// <summary>
		/// Obtient l'identifiant du détail période de niveau 2
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dr">Ligne de la table de résultat</param>
		/// <returns>Identifiant</returns>
		internal static Int64 GetL2Id(WebSession webSession,DataRow dr){
			try{
				switch(webSession.PreformatedProductDetail){
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsector:
						return(Int64.Parse(dr["id_subsector"].ToString()));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorProduct:
						return(Int64.Parse(dr["id_product"].ToString()));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserBrand:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserBrand:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
						return(Int64.Parse(dr["id_advertiser"].ToString()));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentBrand:
						return(Int64.Parse(dr["id_brand"].ToString()));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentProduct:
						return(Int64.Parse(dr["id_product"].ToString()));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct:
						return(Int64.Parse(dr["ID_ADVERTISING_AGENCY"].ToString()));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorHoldingCompanyAdvertiser:
						return(Int64.Parse(dr["id_holding_company"].ToString()));
					
					default:
						return(-1);
				}
			}
			catch(Exception){
				return(-1);
			}
		}

		/// <summary>
		/// Obtient l'identifiant du détail période de niveau 3
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dr">Ligne de la table de résultat</param>
		/// <returns>Identifiant</returns>
		internal static Int64 GetL3Id(WebSession webSession,DataRow dr){
			try{
				switch(webSession.PreformatedProductDetail){
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
						return(Int64.Parse(dr["id_group_"].ToString()));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct:
						return(Int64.Parse(dr["id_product"].ToString()));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorHoldingCompanyAdvertiser:
						return(Int64.Parse(dr["id_advertiser"].ToString()));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserBrand:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserBrand:
						return(Int64.Parse(dr["id_brand"].ToString()));
					default:
						return(-1);
				}
			}
			catch(Exception){
				return(-1);
			}
		}
		#endregion

		#region Libellés des éléments de la nomenclature produit
		/// <summary>
		/// Obtient le libellé du détail période de niveau 1
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dr">Ligne de la table de résultat</param>
		/// <returns>Identifiant</returns>
		internal static string GetL1Label(WebSession webSession,DataRow dr){
			try{
				switch(webSession.PreformatedProductDetail){
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sector:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorHoldingCompanyAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsector:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
						return(dr["sector"].ToString());	
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product:
						return(dr["product"].ToString());	
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
						return(dr["advertiser"].ToString());
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
						return(dr["group_"].ToString());
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct:
						return(dr["GROUP_ADVERTISING_AGENCY"].ToString());
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
						return(dr["ADVERTISING_AGENCY"].ToString());
						//additions
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompany:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiser:						
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserBrand:						
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserProduct:
						return(dr["holding_company"].ToString());
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser:						
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentBrand:						
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserBrand:						
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserProduct:		
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentProduct:
						return(dr["segment"].ToString());
					

					default:
						return("!!");
				}
			}
			catch(Exception){
				return("!!");
			}
		}

		/// <summary>
		/// Obtient le libellé du détail période de niveau 2
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dr">Ligne de la table de résultat</param>
		/// <returns>Identifiant</returns>
		internal static string GetL2Label(WebSession webSession,DataRow dr){
			try{
				switch(webSession.PreformatedProductDetail){
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsector:
						return(dr["subsector"].ToString());
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorProduct:
						return(dr["product"].ToString());
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserBrand:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserBrand:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserProduct:
						return(dr["advertiser"].ToString());
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentBrand:
						return(dr["brand"].ToString());
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentProduct:
						return(dr["product"].ToString());
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct:
						return(dr["ADVERTISING_AGENCY"].ToString());
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorHoldingCompanyAdvertiser:
						return(dr["holding_company"].ToString());
					
					default:
						return("!!");
				}
			}
			catch(Exception){
				return("!!");
			}
		}

		/// <summary>
		/// Obtient le libellé du détail période de niveau 3
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dr">Ligne de la table de résultat</param>
		/// <returns>Identifiant</returns>
		internal static string GetL3Label(WebSession webSession,DataRow dr){
			try{
				switch(webSession.PreformatedProductDetail){
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
						return(dr["group_"].ToString());
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserProduct:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserProduct:
						return(dr["product"].ToString());
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser:
						case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorHoldingCompanyAdvertiser:
						return(dr["advertiser"].ToString());
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserBrand:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserBrand:
						return(dr["brand"].ToString());
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
						return("!!");
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
						return("!!");
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
						return("!!");
					default:
						return("!!");
				}
			}
			catch(Exception){
				return("!!");
			}
		}
		#endregion

		#region Colonne du détail annonceur
		/// <summary>
		/// Détermine la colonne du détail annonceur
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Index de la colonne</returns>
		internal static int GetAdvertiserColumnIndex(WebSession webSession){
			switch(webSession.PreformatedProductDetail){
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
					return(FrameWorkResultConstantes.CompetitorAlert.IDL1_INDEX);
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiser:					
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiser:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserBrand:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserProduct:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserProduct:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserBrand:
					return(FrameWorkResultConstantes.CompetitorAlert.IDL2_INDEX);
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorHoldingCompanyAdvertiser:		
					return(FrameWorkResultConstantes.CompetitorAlert.IDL3_INDEX);
				default:
					return(-1);
			}
		}

		#endregion

		#region Formatage des dates
		/// <summary>
		/// Obtient la date de début de l'analyse en fonction du module
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Date de début</returns>
		private static string GetDateBegin(WebSession webSession){
			switch(webSession.CurrentModule){
				case WebConstantes.Module.Name.ALERTE_CONCURENTIELLE:
				case WebConstantes.Module.Name.ALERTE_POTENTIELS:
					return(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
				case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
				case WebConstantes.Module.Name.ANALYSE_POTENTIELS:
					return(webSession.PeriodBeginningDate);
			}
			return(null);
		}

		/// <summary>
		/// Obtient la date de fin de l'analyse en fonction du module
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Date de fin</returns>
		private static string GetDateEnd(WebSession webSession){
			switch(webSession.CurrentModule){
				case WebConstantes.Module.Name.ALERTE_CONCURENTIELLE:
				case WebConstantes.Module.Name.ALERTE_POTENTIELS:
					return(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
				case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
				case WebConstantes.Module.Name.ANALYSE_POTENTIELS:
					return(webSession.PeriodEndDate);
			}
			return(null);
		}
		#endregion

		#region Obtient l'activité publicitaire d'un produit
		/// <summary>
		/// Obtient l'activité publicitaire d'un produit
		/// </summary>
		/// <param name="tabResult">Tableau de résultats</param>
		/// <param name="dt">table de données</param>
		/// <param name="indexLineProduct">index ligne produit</param>
		/// <param name="filterExpression">expression de filtre</param>
		/// <param name="sort">expression de tri</param>
		/// <param name="referenceUniversMedia">liste supports de référence</param>
		/// <param name="competitorUniversMedia">liste supports conccurents</param>	
		private static void GetProductActivity(ResultTable tabResult,DataTable dt,long indexLineProduct,string filterExpression,string sort,IList referenceUniversMedia,IList competitorUniversMedia){
			double unitReferenceMedia = 0;
			double unitCompetitorMedia = 0;
			Int64 presentNumberColumnIndex = tabResult.GetHeadersIndexInResultTable(ComptitorResultConstantes.PRESENT_HEADER_ID+"-"+ComptitorResultConstantes.ITEM_NUMBER_HEADER_ID);
			Int64 absentNumberColumnIndex = tabResult.GetHeadersIndexInResultTable(ComptitorResultConstantes.ABSENT_HEADER_ID+"-"+ComptitorResultConstantes.ITEM_NUMBER_HEADER_ID);
			Int64 exclusiveNumberColumnIndex = tabResult.GetHeadersIndexInResultTable(ComptitorResultConstantes.EXCLUSIVE_HEADER_ID+"-"+ComptitorResultConstantes.ITEM_NUMBER_HEADER_ID);

			Int64 presentReferenceColumnIndex = tabResult.GetHeadersIndexInResultTable(ComptitorResultConstantes.PRESENT_HEADER_ID+"-"+ComptitorResultConstantes.UNIT_HEADER_ID+"-"+ComptitorResultConstantes.REFERENCE_MEDIA_HEADER_ID);
			Int64 absentReferenceColumnIndex = tabResult.GetHeadersIndexInResultTable(ComptitorResultConstantes.ABSENT_HEADER_ID+"-"+ComptitorResultConstantes.UNIT_HEADER_ID+"-"+ComptitorResultConstantes.REFERENCE_MEDIA_HEADER_ID);
			Int64 exclusiveReferenceColumnIndex = tabResult.GetHeadersIndexInResultTable(ComptitorResultConstantes.EXCLUSIVE_HEADER_ID+"-"+ComptitorResultConstantes.UNIT_HEADER_ID+"-"+ComptitorResultConstantes.REFERENCE_MEDIA_HEADER_ID);

			Int64 presentCompetitorColumnIndex = tabResult.GetHeadersIndexInResultTable(ComptitorResultConstantes.PRESENT_HEADER_ID+"-"+ComptitorResultConstantes.UNIT_HEADER_ID+"-"+ComptitorResultConstantes.COMPETITOR_MEDIA_HEADER_ID);
			Int64 absentCompetitorColumnIndex = tabResult.GetHeadersIndexInResultTable(ComptitorResultConstantes.ABSENT_HEADER_ID+"-"+ComptitorResultConstantes.UNIT_HEADER_ID+"-"+ComptitorResultConstantes.COMPETITOR_MEDIA_HEADER_ID);
			Int64 exclusiveCompetitorColumnIndex = tabResult.GetHeadersIndexInResultTable(ComptitorResultConstantes.EXCLUSIVE_HEADER_ID+"-"+ComptitorResultConstantes.UNIT_HEADER_ID+"-"+ComptitorResultConstantes.COMPETITOR_MEDIA_HEADER_ID);

			 DataRow[] foundRows = dt.Select(filterExpression,sort);

			if(foundRows!=null && !foundRows.Equals(System.DBNull.Value) && foundRows.Length>0){
				for(int i=0;i<foundRows.Length;i++){
					//Unité supports de référence
					if(foundRows[i]["id_media"]!=null && foundRows[i]["id_media"]!=System.DBNull.Value && referenceUniversMedia!=null && 
						foundRows[i]["unit"]!=null && foundRows[i]["unit"]!=System.DBNull.Value && referenceUniversMedia.Contains(foundRows[i]["id_media"].ToString()) ){
						unitReferenceMedia+=double.Parse(foundRows[i]["unit"].ToString());
					}
					//Unité supports concurrents
					if(foundRows[i]["id_media"]!=null && foundRows[i]["id_media"]!=System.DBNull.Value && competitorUniversMedia!=null && 
						foundRows[i]["unit"]!=null && foundRows[i]["unit"]!=System.DBNull.Value && competitorUniversMedia.Contains(foundRows[i]["id_media"].ToString()) ){
						unitCompetitorMedia+=double.Parse(foundRows[i]["unit"].ToString());
					}
				}
			}

			#region Communs
						
			if(unitReferenceMedia>0 && unitCompetitorMedia>0){
				//Nombre 
				((CellUnit)tabResult[indexLineProduct,presentNumberColumnIndex]).Value+=1;

				//supports de référence communs
				((CellUnit)tabResult[indexLineProduct,presentReferenceColumnIndex]).Value+=unitReferenceMedia;
				
				//supports concurrents communs
				((CellUnit)tabResult[indexLineProduct,presentCompetitorColumnIndex]).Value+=unitCompetitorMedia;

			}
			#endregion

			#region Absents
						
			if(unitReferenceMedia==0 && unitCompetitorMedia>0){
				//Nombre 
				((CellUnit)tabResult[indexLineProduct,absentNumberColumnIndex]).Value+=1;
				
				//supports concurrents Absents
				((CellUnit)tabResult[indexLineProduct,absentCompetitorColumnIndex]).Value+=unitCompetitorMedia;

			}
			#endregion
			
			#region Exclusifs
						
			if(unitReferenceMedia>0 && unitCompetitorMedia==0){
				//Nombre 
				((CellUnit)tabResult[indexLineProduct,exclusiveNumberColumnIndex]).Value+=1;

				//supports de référence exclusifs
				((CellUnit)tabResult[indexLineProduct,exclusiveReferenceColumnIndex]).Value+=unitReferenceMedia;
			}
			#endregion

		}
		#endregion

		#region Liste des indexes de colonnes qui ont un seul support ou si 1 seul groupe
		/// <summary>
		/// Liste des indexes de colonnes qui ont un seul support ou si 1 seul groupe
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="nbColInTabData">Nombre de colonnes dans la table de données source</param>
		/// <param name="resultTable">Tableau de résultat</param>
		/// <param name="groupMediaTotalIndex">Liste des groupes de supports</param>
		/// <returns>Indexes de colonnes qui ont un seul support</returns>
		private static Hashtable GetSubTotalWithOneMediaIndexes(WebSession webSession,long nbColInTabData,ResultTable resultTable, WebCommon.Results.SelectionGroup[] groupMediaTotalIndex){
			Hashtable subTotalWithOneMediaIndexes=new Hashtable();
			for(long k=FrameWorkResultConstantes.CompetitorAlert.TOTAL_INDEX;k<nbColInTabData;k++){
				for(int m=1;m<groupMediaTotalIndex.GetLength(0);m++){
					if(groupMediaTotalIndex[m]!=null){
						if(groupMediaTotalIndex[m].IndexInResultTable==k && 
						  (groupMediaTotalIndex[m].Count==1||webSession.CompetitorUniversMedia.Count==1))
								subTotalWithOneMediaIndexes.Add(k,true);
					}
				}
			}
			return(subTotalWithOneMediaIndexes);
		}
		#endregion

		#endregion
		
	}
}

