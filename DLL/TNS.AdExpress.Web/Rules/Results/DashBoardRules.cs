#region Informations
// Auteur: D. V. Mussuma 
// Date de création: 24/03/2005 
// Date de modification:
//  -  D. V. Mussuma  12/08/2005 : modification de la generation d'excetpion : passage de l'exception en paramètre.
//  -  D. V. Mussuma  26/06/2006 : ajout du tableau media X produits.

#endregion

using System;
using System.Collections;
using System.Data;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Results;
using CstWeb = TNS.AdExpress.Constantes.Web;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Exceptions;
using ClassificationCst=TNS.AdExpress.Constantes.Classification;
using CstResults=TNS.AdExpress.Constantes.FrameWork.Results;
using CstPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel;
using TNS.AdExpress.Web.Core.Translation;
using TNS.FrameWork.Date;
using WebModule=TNS.AdExpress.Constantes.Web.Module;
using TNS.FrameWork.WebResultUI;


namespace TNS.AdExpress.Web.Rules.Results
{
	/// <summary>
	/// Classe métier du module tableaux de bord 
	/// </summary>
	public class DashBoardRules
	{
	
		#region		GetDataTable
		/// <summary>
		/// Génère un tableau de bord en fonction de la sélection effectuée par le client. 
		/// </summary>
		/// <param name="webSession">Session utilisateur</param>		
		/// <remarks>
		/// Utilise les méthodes:
		///		- DashBoardDataAccess.getData(webSession,yearN,yearN1) : obtient les données à traiter
		///		- BuildDataTable(ds,webSession,vehicleType) : construit le tableau de bord <code>object[,] tab</code>	
		/// </remarks>
		/// <returns>Tableau contenant les données à livrer structurées suivant le tableau préformaté
		/// sélectionné</returns>
		public static object[,] GetDataTable(WebSession webSession){
			#region variables
			//tableau de résultats
			object[,] tab = null;
			//années sélectionnées
			int yearN1=int.Parse(webSession.PeriodBeginningDate.Substring(0,4))-1;
			int yearN = int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
			#endregion
			try{
				#region type de média
				//identification du Média  sélectionné
				string Vehicle = ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
				ClassificationCst.DB.Vehicles.names vehicleType = (ClassificationCst.DB.Vehicles.names)int.Parse(Vehicle);
				#endregion
			
				//Chargement des données du tableau de bord
				DataSet ds=DashBoardDataAccess.getData(webSession,yearN,yearN1);
				//Construction du tableau de résultats				
			   tab = BuildDataTable(ds,webSession,vehicleType);																																																	
			}
			catch(Exception e){
				throw new DashBoardRulesException("getDataTable(WebSession webSession): Impossible de générer le tableau de résultats",e);
			}
			//tableau de résultats
			if(tab==null)return(new object[0,0]);

			return tab;
		}
		#endregion 

		#region GetDataTable For Generic UI
//		/// <summary>
//		/// Génère un tableau de bord en fonction de la sélection effectuée par le client. 
//		/// </summary>
//		/// <param name="webSession">Session utilisateur</param>		
//		/// <remarks>
//		/// Utilise les méthodes:
//		///		- DashBoardDataAccess.getData(webSession,yearN,yearN1) : obtient les données à traiter
//		///		- BuildDataTable(ds,webSession,vehicleType) : construit le tableau de bord <code>object[,] tab</code>	
//		/// </remarks>
//		/// <returns>Tableau contenant les données à livrer structurées suivant le tableau préformaté
//		/// sélectionné</returns>
//		public static ResultTable GetDataTableForGenericUI(WebSession webSession)
//		{
//			#region variables
//			//tableau de résultats
//			ResultTable tab = null;
//			//années sélectionnées
//			int yearN1=int.Parse(webSession.PeriodBeginningDate.Substring(0,4))-1;
//			int yearN = int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
//			#endregion
//			try
//			{
//				#region type de média
//				//identification du Média  sélectionné
//				string Vehicle = ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
//				ClassificationCst.DB.Vehicles.names vehicleType = (ClassificationCst.DB.Vehicles.names)int.Parse(Vehicle);
//				#endregion
//			
//				//Chargement des données du tableau de bord
//				DataSet ds=DashBoardDataAccess.getData(webSession,yearN,yearN1);
//				//Construction du tableau de résultats				
//				tab = BuildDataTableForGenericUI(ds,webSession,vehicleType);																																																	
//			}
//			catch(Exception e)
//			{
//				throw new DashBoardRulesException("getDataTable(WebSession webSession): Impossible de générer le tableau de résultats",e);
//			}
//			//tableau de résultats
//			if(tab==null)return(new ResultTable(0,0));
//
//			return tab;
//		}
		#endregion 
		
		#region Méthodes privées

		#region Tableaux de résulats 
		/// <summary>
		/// Construit un tableau structuré de type 1 à 12 en fonction des détails sélectionnés.		
		/// Etapes:
		/// 	- Vérification de la présence de données dans dsData
		///		- Construction des constantes nécessaires au traitement des données:		
		///			- index de la première colonne à contenir des données quantitatives
		///			- tableau des index de nomenclature contenant pour chaque niveau de nomenclature des triplets
		///			(index colonne de la nom dans dsData, ligne du niveau dans le tableau de resultat, Identifiant du niveau)
		/// </summary>
		/// <param name="dsData">DataSet issue de la couche BDD. Il contient déjà les données nécessaires à 
		/// l'édition du tableau en fonction des niveaux de détails préformatés et du tableau considéré</param>
		/// <param name="webSession">Session utilisateur</param>
		/// <param name="vehicleType">type du média</param>
		/// <returns>Un tableau structurant les données de dsData pour un tableau de type 1 ou 2</returns>
		private static object[,] BuildDataTable(DataSet dsData, WebSession webSession,ClassificationCst.DB.Vehicles.names vehicleType)
		{
			#region variables
			object[,] tab = null;
//			ArrayList sectorIdList =null;
//			if(webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector )
//			sectorIdList = new ArrayList();
			Hashtable sectorHashtable =null;
			if(webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector )
				sectorHashtable = new Hashtable();
			#endregion

			#region Construction du tableau de résultats
			if(dsData!=null && dsData.Tables[0].Rows.Count>0){
				//Dimensionne le tableau
				tab = GetSizedTable(dsData,webSession,vehicleType);
				//Insertion des libellés du tableau
//				tab = SetTabLabel(webSession,tab,vehicleType,dsData.Tables[0],sectorIdList);
				tab = SetTabLabel(webSession,tab,vehicleType,dsData.Tables[0],sectorHashtable);
				if(tab!=null){
					//Tableau formatté
//					tab = GetFormattedTable(webSession,dsData,tab,vehicleType,sectorIdList);
					tab = GetFormattedTable(webSession,dsData,tab,vehicleType,sectorHashtable);																																																													
				}				
			}
			#endregion

			return tab;
		}
		#endregion				

		#region Tableaux de résulats For Generic UI
//		/// <summary>
//		/// Construit un tableau structuré de type 1 à 12 en fonction des détails sélectionnés.		
//		/// Etapes:
//		/// 	- Vérification de la présence de données dans dsData
//		///		- Construction des constantes nécessaires au traitement des données:		
//		///			- index de la première colonne à contenir des données quantitatives
//		///			- tableau des index de nomenclature contenant pour chaque niveau de nomenclature des triplets
//		///			(index colonne de la nom dans dsData, ligne du niveau dans le tableau de resultat, Identifiant du niveau)
//		/// </summary>
//		/// <param name="dsData">DataSet issue de la couche BDD. Il contient déjà les données nécessaires à 
//		/// l'édition du tableau en fonction des niveaux de détails préformatés et du tableau considéré</param>
//		/// <param name="webSession">Session utilisateur</param>
//		/// <param name="vehicleType">type du média</param>
//		/// <returns>Un tableau structurant les données de dsData pour un tableau de type 1 ou 2</returns>
//		private static ResultTable BuildDataTableForGenericUI(DataSet dsData, WebSession webSession,ClassificationCst.DB.Vehicles.names vehicleType)
//		{
//			#region variables
//			ResultTable tab = null;
//			//			ArrayList sectorIdList =null;
//			//			if(webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector )
//			//			sectorIdList = new ArrayList();
//			Hashtable sectorHashtable =null;
//			if(webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector )
//				sectorHashtable = new Hashtable();
//			#endregion
//
//			#region Construction du tableau de résultats
//			if(dsData!=null && dsData.Tables[0].Rows.Count>0)
//			{
//				//Dimensionne le tableau
//				tab = GetSizedTableForGenericUI(dsData,webSession,vehicleType);
//				//Insertion des libellés du tableau
//				//				tab = SetTabLabel(webSession,tab,vehicleType,dsData.Tables[0],sectorIdList);
//				tab = SetTabLabelForGenericUI(webSession,tab,vehicleType,dsData.Tables[0],sectorHashtable);
//				if(tab!=null)
//				{
//					//Tableau formatté
//					//					tab = GetFormattedTable(webSession,dsData,tab,vehicleType,sectorIdList);
//					tab = GetFormattedTableForGenericUI(webSession,dsData,tab,vehicleType,sectorHashtable);																																																													
//				}				
//			}
//			#endregion
//
//			return tab;
//		}
		#endregion				

		#region Insertion et traitement de données dans les tableaux de résulats

		#region Obtient le tableau de résultats
		/// <summary>
		///	Obtient le tableau de résultats en fonction de la sélection utilisateur
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dsData">table de données</param>
		/// <param name="tab">tableau de résultats à remplir</param>
		/// <param name="vehicleType">type du média sélectionné</param>
		/// <param name="sectorHashtable">Liste identifiant des familles à afficher en colonne pour tableau Media\Famille</param>
		/// <returns>tableau de résultats</returns>
		private static object[,] GetFormattedTable(WebSession webSession,DataSet dsData,object[,] tab,ClassificationCst.DB.Vehicles.names vehicleType,Hashtable sectorHashtable)
		{	
			#region variables
			int[] coordCellTab=new int[1];;
			int col=0;
			int row=0;
			int oldRowL1=0;
			int oldRowL2=0;
			int oldRowL3=0;
			ArrayList oldIdL2= new ArrayList();
			ArrayList oldIdL3= new ArrayList();
			int yearN1=int.Parse(webSession.PeriodBeginningDate.Substring(0,4))-1;	
			if(!webSession.DetailPeriodBeginningDate.Equals("0") && WebFunctions.CheckedText.IsStringEmpty(webSession.DetailPeriodBeginningDate)) 
				yearN1=int.Parse(webSession.DetailPeriodBeginningDate.Substring(0,4))-1;			
			
			string criteriaPeriodN = "period>="+webSession.PeriodBeginningDate+" AND period<="+webSession.PeriodEndDate;
			string criteriaPeriodN1 ="";		
			CstWeb.CustomerSessions.Unit unit = webSession.Unit;
			string sumUnit = "sum("+unit.ToString()+")";
			string PeriodBeginningDate = "";
			string PeriodEndDate ="";
			string PeriodBeginningDateN1 = "";
			string PeriodEndDateN1 ="";
			string vehicleId = ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
			string vehicleName = ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).Text.ToString();
//			AtomicPeriodWeek currentPeriod,beginPreviousPeriod,endPreviousPeriod;
			int year;
			if(!webSession.DetailPeriodBeginningDate.Equals("0") && WebFunctions.CheckedText.IsStringEmpty(webSession.DetailPeriodBeginningDate))
				year = int.Parse(webSession.DetailPeriodBeginningDate.Substring(0,4));
			else
				 year = int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
			#endregion			

			//Choix du type de tableau de résultats
			switch(webSession.PreformatedTable){
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units :					
					#region Total unités
					//Total investissement en euro période N,N-1
					coordCellTab[0] = oldRowL1 = row = 1;
					col = CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS;					
					//Total unités
					
					tab = SetTabLinesLabel(tab,vehicleId,vehicleName,row,CstResults.DashBoard.ID_ELMT_L1_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L1_COLUMN_INDEX);
					if(webSession.ComparativeStudy)tab = SetTabLinesLabel(tab,vehicleId,vehicleName,row+1,CstResults.DashBoard.ID_ELMT_L1_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L1_COLUMN_INDEX);
					CriteriaPeriod(webSession,ref PeriodBeginningDate,ref PeriodEndDate,year,false);
					criteriaPeriodN = "period>="+PeriodBeginningDate+" AND period<="+PeriodEndDate;						
					if(webSession.ComparativeStudy){
						CriteriaPeriod(webSession,ref PeriodBeginningDateN1,ref PeriodEndDateN1,year,true);
						criteriaPeriodN1 ="period>="+PeriodBeginningDateN1+" AND period<="+PeriodEndDateN1;			 											
					}
					tab = FillTabUnits(webSession,dsData.Tables[0],ref tab,ref row,ref col,criteriaPeriodN,criteriaPeriodN1,coordCellTab,true,vehicleType);
										
					#endregion

					#region Traitement des centres d'intérêts et/ou supports					
						
					foreach(DataRow dr in  dsData.Tables[0].Rows){	
													
						#region Traitement sélection nomenclature niveau 2 (centres d'intérêt ou support)
						//Traitement des centres d'intérêts et/ou supports
						if(oldIdL2!=null  && (GetL2Id(webSession).Length>0) && !oldIdL2.Contains(dr[GetL2Id(webSession)].ToString()) 
							&& dsData.Tables[0].Columns.Contains(GetL2Id(webSession))){
							//Unité euro
							oldRowL2 = row = row+1;
							tab = SetTabLinesLabel(tab,dr[GetL2Id(webSession)].ToString(),dr[GetL2Label(webSession)].ToString(),row,CstResults.DashBoard.ID_ELMT_L2_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L2_COLUMN_INDEX);
							if(webSession.ComparativeStudy)tab = SetTabLinesLabel(tab,dr[GetL2Id(webSession)].ToString(),dr[GetL2Label(webSession)].ToString(),row+1,CstResults.DashBoard.ID_ELMT_L2_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L2_COLUMN_INDEX);
							//Critères de calcul	
							criteriaPeriodN = "period>="+PeriodBeginningDate+" AND period<="+PeriodEndDate;	
							criteriaPeriodN +=" AND "+GetL2Id(webSession)+"="+ dr[GetL2Id(webSession)].ToString();	
							if(webSession.ComparativeStudy){
								criteriaPeriodN1 ="period>="+PeriodBeginningDateN1+" AND period<="+PeriodEndDateN1;
								criteriaPeriodN1+=" AND "+GetL2Id(webSession)+"="+ dr[GetL2Id(webSession)].ToString();			 													
							}
							//Traitement unités
							tab = FillTabUnits(webSession,dsData.Tables[0],ref tab,ref row,ref col,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType);
							oldIdL2.Add(dr[GetL2Id(webSession)].ToString());
						}
						#endregion																

						#region Traitement sélection nomenclature niveau 3
						//Traitement des supports
						if(oldIdL3!=null  && (GetL3Id(webSession).Length>0) && !oldIdL3.Contains(dr[GetL3Id(webSession)].ToString()) 
							&& dsData.Tables[0].Columns.Contains(GetL3Id(webSession))){
							coordCellTab[0] = oldRowL2;
							oldRowL3 = row = row+1;
							tab = SetTabLinesLabel(tab,dr[GetL3Id(webSession)].ToString(),dr[GetL3Label(webSession)].ToString(),row,CstResults.DashBoard.ID_ELMT_L3_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L3_COLUMN_INDEX);
							if(webSession.ComparativeStudy)tab = SetTabLinesLabel(tab,dr[GetL3Id(webSession)].ToString(),dr[GetL3Label(webSession)].ToString(),row+1,CstResults.DashBoard.ID_ELMT_L3_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L3_COLUMN_INDEX);
							//Critères de calcul
							criteriaPeriodN = "period>="+PeriodBeginningDate+" AND period<="+PeriodEndDate;	
							criteriaPeriodN +=" AND "+GetL2Id(webSession)+"="+ dr[GetL2Id(webSession)].ToString()+" AND "+GetL3Id(webSession)+"="+dr[GetL3Id(webSession)].ToString();
							if(webSession.ComparativeStudy){
								criteriaPeriodN1 ="period>="+PeriodBeginningDateN1+" AND period<="+PeriodEndDateN1;
								criteriaPeriodN1+=" AND "+GetL2Id(webSession)+"="+ dr[GetL2Id(webSession)].ToString()+" AND "+GetL3Id(webSession)+"="+dr[GetL3Id(webSession)].ToString();
							}
							//Traitement unités
							tab = FillTabUnits(webSession,dsData.Tables[0],ref tab,ref row,ref col,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType);
							oldIdL3.Add(dr[GetL3Id(webSession)].ToString());
						}
						#endregion
					}
				
					#endregion
					//Calcul du PDM ou PDV
					if(webSession.PDM || webSession.PDV)
						tab=ComputeTabPDMOrPDV(webSession,ref tab,1,CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS);
					break;
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual :											
					//Total unité sélectionnée sur période N,N-1
					coordCellTab[0] = oldRowL1 = row = 1;
					col = CstResults.DashBoard.TOTAL_COLUMN_INDEX;						
					if(webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual){
						
						tab = SetTabLinesLabel(tab,vehicleId,vehicleName,row,CstResults.DashBoard.ID_ELMT_L1_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L1_COLUMN_INDEX);
						if(webSession.ComparativeStudy)tab = SetTabLinesLabel(tab,vehicleId,vehicleName,row+1,CstResults.DashBoard.ID_ELMT_L1_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L1_COLUMN_INDEX);
					}
					else{ 
						tab = SetTabLinesLabel(tab,"-1",GestionWeb.GetWebWord(1582,webSession.SiteLanguage),row,CstResults.DashBoard.ID_ELMT_L1_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L1_COLUMN_INDEX);
						if(webSession.ComparativeStudy)tab = SetTabLinesLabel(tab,"-1",GestionWeb.GetWebWord(1582,webSession.SiteLanguage),row+1,CstResults.DashBoard.ID_ELMT_L1_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L1_COLUMN_INDEX);
					}
					tab = FillTabUnitByPeriod(webSession,dsData.Tables[0],ref tab,ref row,ref col,"","",coordCellTab,true,vehicleType,yearN1,year);
					// unité sélectionnée sur période N,N-1
					foreach(DataRow dr in  dsData.Tables[0].Rows){
						
						#region Traitement sélection nomenclature niveau 2 (centres d'intérêt ou support ou famille)
						//Traitement des centres d'intérêts et/ou supports
						if(oldIdL2!=null  && (GetL2Id(webSession).Length>0) && !oldIdL2.Contains(dr[GetL2Id(webSession)].ToString()) 
							&& dsData.Tables[0].Columns.Contains(GetL2Id(webSession))){							
							oldRowL2 = row = row+1;
							//Critères de calcul
							criteriaPeriodN = criteriaPeriodN1 =" AND "+GetL2Id(webSession)+"="+ dr[GetL2Id(webSession)].ToString();														
							tab = SetTabLinesLabel(tab,dr[GetL2Id(webSession)].ToString(),dr[GetL2Label(webSession)].ToString(),row,CstResults.DashBoard.ID_ELMT_L2_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L2_COLUMN_INDEX);
							if(webSession.ComparativeStudy)tab = SetTabLinesLabel(tab,dr[GetL2Id(webSession)].ToString(),dr[GetL2Label(webSession)].ToString(),row+1,CstResults.DashBoard.ID_ELMT_L2_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L2_COLUMN_INDEX);							
							//Traitement unités
							tab = FillTabUnitByPeriod(webSession,dsData.Tables[0],ref tab,ref row,ref col,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType,yearN1,year);														
							
							oldIdL2.Add(dr[GetL2Id(webSession)].ToString());
						}
						#endregion	
						
						#region Traitement sélection nomenclature niveau 3
						//Traitement des supports
						if(webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual){
							if(oldIdL3!=null  && (GetL3Id(webSession).Length>0) && !oldIdL3.Contains(dr[GetL3Id(webSession)].ToString()) 
								&& dsData.Tables[0].Columns.Contains(GetL3Id(webSession))){
								coordCellTab[0] = oldRowL2;
								oldRowL3 = row = row+1;
								//Critères de calcul
								criteriaPeriodN = criteriaPeriodN1 = " AND "+GetL2Id(webSession)+"="+ dr[GetL2Id(webSession)].ToString()+" AND "+GetL3Id(webSession)+"="+dr[GetL3Id(webSession)].ToString();							 								
								tab = SetTabLinesLabel(tab,dr[GetL3Id(webSession)].ToString(),dr[GetL3Label(webSession)].ToString(),row,CstResults.DashBoard.ID_ELMT_L3_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L3_COLUMN_INDEX);
								if(webSession.ComparativeStudy)tab = SetTabLinesLabel(tab,dr[GetL3Id(webSession)].ToString(),dr[GetL3Label(webSession)].ToString(),row+1,CstResults.DashBoard.ID_ELMT_L3_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L3_COLUMN_INDEX);								
								//Traitement unités
								tab = FillTabUnitByPeriod(webSession,dsData.Tables[0],ref tab,ref row,ref col,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType,yearN1,year);							
								oldIdL3.Add(dr[GetL3Id(webSession)].ToString());
							}
						}
						#endregion
					}
					//Calcul du PDM ou PDV
					if(webSession.PDM || webSession.PDV)
						tab=ComputeTabPDMOrPDV(webSession,ref tab,1,CstResults.DashBoard.TOTAL_COLUMN_INDEX);
					//répartition en Pourcentage
					if(webSession.Percentage)
						tab=ComputePercentage(webSession,ref tab,1,CstResults.DashBoard.TOTAL_COLUMN_INDEX);
					break;																			
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format :					
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format :						
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay :					
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice :																									
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:																										
					
					tab =  BuildDataTableRepartition(webSession,dsData.Tables[0],ref tab,ref row,ref col,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType,yearN1);
					//Calcul du PDM ou PDV
					if(webSession.PDM || webSession.PDV)
						tab=ComputeTabPDMOrPDV(webSession,ref tab,1,CstResults.DashBoard.TOTAL_COLUMN_INDEX);
					//répartition en Pourcentage
					if(webSession.Percentage)
						tab=ComputePercentage(webSession,ref tab,1,CstResults.DashBoard.TOTAL_COLUMN_INDEX);
					break;	
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector :						
//					if(sectorIdList!=null && sectorIdList.Count>0){
					if(sectorHashtable!=null && sectorHashtable.Count>0){
//						tab = BuildDataTableType_Media_X_Sector(webSession,dsData.Tables[0],ref tab,ref row,ref col,coordCellTab,sectorIdList,year);	
						tab = BuildDataTableType_Media_X_Sector(webSession,dsData.Tables[0],ref tab,ref row,ref col,coordCellTab,sectorHashtable,year);	
						//Calcul du PDM 
						if(webSession.PDM)
							tab=ComputeTabPDMOrPDV(webSession,ref tab,1,CstResults.DashBoard.TOTAL_COLUMN_INDEX);
						//répartition en Pourcentage
						if(webSession.Percentage)
							tab=ComputePercentage(webSession,ref tab,1,CstResults.DashBoard.TOTAL_COLUMN_INDEX);
					}
					break;
				default : 
					throw new DashBoardRulesException("GetFormattedTable(WebSession webSession,DataSet dsData,object[,] tab,ClassificationCst.DB.Vehicles.names vehicleType)--> Impossible d'identifier le tableau de bord à traiter.");
			}
			return tab;
		}
		#endregion 

		#region Obtient le tableau de résultats For Generic UI
//		/// <summary>
//		///	Obtient le tableau de résultats en fonction de la sélection utilisateur
//		/// </summary>
//		/// <param name="webSession">session du client</param>
//		/// <param name="dsData">table de données</param>
//		/// <param name="tab">tableau de résultats à remplir</param>
//		/// <param name="vehicleType">type du média sélectionné</param>
//		/// <param name="sectorHashtable">Liste identifiant des familles à afficher en colonne pour tableau Media\Famille</param>
//		/// <returns>tableau de résultats</returns>
//		private static ResultTable GetFormattedTableForGenericUI(WebSession webSession,DataSet dsData,ResultTable tab,ClassificationCst.DB.Vehicles.names vehicleType,Hashtable sectorHashtable)
//		{	
//			#region variables
//			int[] coordCellTab=new int[1];;
//			int col=0;
//			int row=0;
//			int oldRowL1=0;
//			int oldRowL2=0;
//			int oldRowL3=0;
//			ArrayList oldIdL2= new ArrayList();
//			ArrayList oldIdL3= new ArrayList();
//			int yearN1=int.Parse(webSession.PeriodBeginningDate.Substring(0,4))-1;	
//			if(!webSession.DetailPeriodBeginningDate.Equals("0") && WebFunctions.CheckedText.IsStringEmpty(webSession.DetailPeriodBeginningDate)) 
//				yearN1=int.Parse(webSession.DetailPeriodBeginningDate.Substring(0,4))-1;			
//			
//			string criteriaPeriodN = "period>="+webSession.PeriodBeginningDate+" AND period<="+webSession.PeriodEndDate;
//			string criteriaPeriodN1 ="";		
//			CstWeb.CustomerSessions.Unit unit = webSession.Unit;
//			string sumUnit = "sum("+unit.ToString()+")";
//			string PeriodBeginningDate = "";
//			string PeriodEndDate ="";
//			string PeriodBeginningDateN1 = "";
//			string PeriodEndDateN1 ="";
//			string vehicleId = ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
//			string vehicleName = ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).Text.ToString();
//			//			AtomicPeriodWeek currentPeriod,beginPreviousPeriod,endPreviousPeriod;
//			int year;
//			if(!webSession.DetailPeriodBeginningDate.Equals("0") && WebFunctions.CheckedText.IsStringEmpty(webSession.DetailPeriodBeginningDate))
//				year = int.Parse(webSession.DetailPeriodBeginningDate.Substring(0,4));
//			else
//				year = int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
//			#endregion			
//
//			//Choix du type de tableau de résultats
//			switch(webSession.PreformatedTable)
//			{
//				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units :					
//					#region Total unités
//					//Total investissement en euro période N,N-1
//					coordCellTab[0] = oldRowL1 = row = 1;
//					col = CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS;					
//					//Total unités
//					tab.AddNewLine(LineType.level1);
//					tab = SetTabLinesLabelForGenericUI(tab,vehicleId,vehicleName,row,CstResults.DashBoard.ID_ELMT_L1_COLUMN_INDEX+1,CstResults.DashBoard.LABEL_ELMT_L1_COLUMN_INDEX+1);
//					if(webSession.ComparativeStudy)tab = SetTabLinesLabelForGenericUI(tab,vehicleId,vehicleName,row+1,CstResults.DashBoard.ID_ELMT_L1_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L1_COLUMN_INDEX);
//					CriteriaPeriod(webSession,ref PeriodBeginningDate,ref PeriodEndDate,year,false);
//					criteriaPeriodN = "period>="+PeriodBeginningDate+" AND period<="+PeriodEndDate;						
//					if(webSession.ComparativeStudy)
//					{
//						CriteriaPeriod(webSession,ref PeriodBeginningDateN1,ref PeriodEndDateN1,year,true);
//						criteriaPeriodN1 ="period>="+PeriodBeginningDateN1+" AND period<="+PeriodEndDateN1;			 											
//					}
//					tab.AddNewLine(LineType.level1);
//					col++;
//					tab = FillTabUnitsForGenericUI(webSession,dsData.Tables[0],ref tab,ref row,ref col,criteriaPeriodN,criteriaPeriodN1,coordCellTab,true,vehicleType);
//										
//					#endregion
//
//					#region Traitement des centres d'intérêts et/ou supports					
//						
//					foreach(DataRow dr in  dsData.Tables[0].Rows)
//					{	
//													
//						#region Traitement sélection nomenclature niveau 2 (centres d'intérêt ou support)
//						//Traitement des centres d'intérêts et/ou supports
//						if(oldIdL2!=null  && (GetL2Id(webSession).Length>0) && !oldIdL2.Contains(dr[GetL2Id(webSession)].ToString()) 
//							&& dsData.Tables[0].Columns.Contains(GetL2Id(webSession)))
//						{
//							//Unité euro
//							oldRowL2 = row = row+1;
//							tab.AddNewLine(LineType.level1);
//							tab = SetTabLinesLabelForGenericUI(tab,dr[GetL2Id(webSession)].ToString(),dr[GetL2Label(webSession)].ToString(),row,CstResults.DashBoard.ID_ELMT_L2_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L2_COLUMN_INDEX);
//							if(webSession.ComparativeStudy)tab = SetTabLinesLabelForGenericUI(tab,dr[GetL2Id(webSession)].ToString(),dr[GetL2Label(webSession)].ToString(),row+1,CstResults.DashBoard.ID_ELMT_L2_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L2_COLUMN_INDEX);
//							//Critères de calcul	
//							criteriaPeriodN = "period>="+PeriodBeginningDate+" AND period<="+PeriodEndDate;	
//							criteriaPeriodN +=" AND "+GetL2Id(webSession)+"="+ dr[GetL2Id(webSession)].ToString();	
//							if(webSession.ComparativeStudy)
//							{
//								criteriaPeriodN1 ="period>="+PeriodBeginningDateN1+" AND period<="+PeriodEndDateN1;
//								criteriaPeriodN1+=" AND "+GetL2Id(webSession)+"="+ dr[GetL2Id(webSession)].ToString();			 													
//							}
//							//Traitement unités
//							tab.AddNewLine(LineType.level1);
//							tab = FillTabUnitsForGenericUI(webSession,dsData.Tables[0],ref tab,ref row,ref col,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType);
//							oldIdL2.Add(dr[GetL2Id(webSession)].ToString());
//						}
//						#endregion																
//
//						#region Traitement sélection nomenclature niveau 3
//						//Traitement des supports
//						if(oldIdL3!=null  && (GetL3Id(webSession).Length>0) && !oldIdL3.Contains(dr[GetL3Id(webSession)].ToString()) 
//							&& dsData.Tables[0].Columns.Contains(GetL3Id(webSession)))
//						{
//							coordCellTab[0] = oldRowL2;
//							oldRowL3 = row = row+1;
//							tab.AddNewLine(LineType.level1);
//							tab = SetTabLinesLabelForGenericUI(tab,dr[GetL3Id(webSession)].ToString(),dr[GetL3Label(webSession)].ToString(),row,CstResults.DashBoard.ID_ELMT_L3_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L3_COLUMN_INDEX);
//							if(webSession.ComparativeStudy)tab = SetTabLinesLabelForGenericUI(tab,dr[GetL3Id(webSession)].ToString(),dr[GetL3Label(webSession)].ToString(),row+1,CstResults.DashBoard.ID_ELMT_L3_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L3_COLUMN_INDEX);
//							//Critères de calcul
//							criteriaPeriodN = "period>="+PeriodBeginningDate+" AND period<="+PeriodEndDate;	
//							criteriaPeriodN +=" AND "+GetL2Id(webSession)+"="+ dr[GetL2Id(webSession)].ToString()+" AND "+GetL3Id(webSession)+"="+dr[GetL3Id(webSession)].ToString();
//							if(webSession.ComparativeStudy)
//							{
//								criteriaPeriodN1 ="period>="+PeriodBeginningDateN1+" AND period<="+PeriodEndDateN1;
//								criteriaPeriodN1+=" AND "+GetL2Id(webSession)+"="+ dr[GetL2Id(webSession)].ToString()+" AND "+GetL3Id(webSession)+"="+dr[GetL3Id(webSession)].ToString();
//							}
//							//Traitement unités
//							tab.AddNewLine(LineType.level1);
//							tab = FillTabUnitsForGenericUI(webSession,dsData.Tables[0],ref tab,ref row,ref col,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType);
//							oldIdL3.Add(dr[GetL3Id(webSession)].ToString());
//						}
//						#endregion
//					}
//				
//					#endregion
//					//Calcul du PDM ou PDV
////					if(webSession.PDM || webSession.PDV)
////						tab=ComputeTabPDMOrPDV(webSession,ref tab,1,CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS);
//					break;
//																		
//				
//				default : 
//					throw new DashBoardRulesException("GetFormattedTable(WebSession webSession,DataSet dsData,object[,] tab,ClassificationCst.DB.Vehicles.names vehicleType)--> Impossible d'identifier le tableau de bord à traiter.");
//			}
//			return tab;
//		}
		#endregion
		
		#region Traitement des colonnes des tableau de résultats

		#region Construit les tables répartition ( tableaux 4 à 12)
		/// <summary>
		/// Remplit des valeurs dans les cellules du tableau de résultats
		/// pour le type répartition sélectionné : tableaux 4 à 12
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dt">table de données</param>
		/// <param name="tab">tableau de résultats</param>
		/// <param name="row">index ligne tableau de résultats</param>
		/// <param name="col">index colonne tableau de résultats</param>
		/// <param name="criteriaPeriodN">critère de sélection sur table de données période N</param>
		/// <param name="criteriaPeriodN1">critère de sélection sur table de données période N-1</param>
		/// <param name="coordCellTab">coordonnées  de la dernière cellule traitée</param>
		/// <param name="isTotalLine">vrai si ligne totale</param>	
		/// <param name="vehicleType">type de média</param>
		/// <param name="yearN1">année précédente</param>
		/// <returns>tableau de résultats</returns>
		private static object[,]  BuildDataTableRepartition(WebSession webSession, DataTable dt,ref object[,] tab,ref int row,ref int col,string criteriaPeriodN,string criteriaPeriodN1, int[] coordCellTab,bool isTotalLine,ClassificationCst.DB.Vehicles.names vehicleType,int yearN1)
		{						

			switch(webSession.PreformatedTable){
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format :				
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format :		
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay :				
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice :																							
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:					
					tab=GetDataTableType_4_5_7_8_10_11(webSession,dt,ref tab,ref row,ref col,coordCellTab,isTotalLine,vehicleType,yearN1);
					break;
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format :										
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice :
					tab=GetDataTableType_6_9_12(webSession,dt,ref tab,ref row,ref col,coordCellTab,isTotalLine,vehicleType,yearN1);
					break;										
				default : 
					throw new DashBoardRulesException("BuildDataTableRepartition(WebSession webSession, DataTable dt,ref object[,] tab,ref int row,ref int col,string criteriaPeriodN,string criteriaPeriodN1, int[] coordCellTab,bool isTotalLine,ClassificationCst.DB.Vehicles.names vehicleType,int yearN1) --> Impossible d'identifier le tableau de bord à traiter.");
			}
			return tab;
		}			

		#region tableaux Media ou familles X repartition 4,5,7,8,10,11
		/// <summary>
		/// Construit lese tableaux de bord Media ou familles X Repartition 
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dt">table de données</param>
		/// <param name="tab">tableau de résultats</param>
		/// <param name="row">index ligne tableau de résultats</param>
		/// <param name="col">index colonne tableau de résultats</param>
		/// <param name="coordCellTab">coordonnées  de la dernière cellule traitée</param>
		/// <param name="isTotalLine">vrai si ligne totale</param>	
		/// <param name="vehicleType">type de média</param>
		/// <param name="yearN1">année précédente</param>
		/// <returns>tableau de résultats</returns>
		private static object[,] GetDataTableType_4_5_7_8_10_11(WebSession webSession, DataTable dt,ref object[,] tab,ref int row,ref int col, int[] coordCellTab,bool isTotalLine,ClassificationCst.DB.Vehicles.names vehicleType,int yearN1){
			#region variables
			string vehicleId = ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
			string vehicleName = ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).Text.ToString();		
			CstWeb.CustomerSessions.Unit unit = webSession.Unit;
			string sumUnit = "sum("+unit.ToString()+")";
			string operation="";							
			int oldRowL1=0;
			int oldRowL2=0;
			int oldRowL3=0;		
			ArrayList oldIdL2= new ArrayList();
			ArrayList oldIdL3= new ArrayList();
			string criteriaPeriodN =" ";	
			string criteriaPeriodN1 =" ";
			coordCellTab[0] = oldRowL1 = row = 1;
			col = CstResults.DashBoard.TOTAL_COLUMN_INDEX;
			#endregion
			
			operation = sumUnit;			
				
			//Total unité sélectionnée sur période N,N-1									
			if(webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format
				|| webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay 
				|| webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice
				){
				tab = SetTabLinesLabel(tab,vehicleId,vehicleName,row,CstResults.DashBoard.ID_ELMT_L1_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L1_COLUMN_INDEX);
				if(webSession.ComparativeStudy)
					SetTabLinesLabel(tab,vehicleId,vehicleName,row+1,CstResults.DashBoard.ID_ELMT_L1_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L1_COLUMN_INDEX);
			}
			else if(webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format 						
				|| webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay 																											
				|| webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice					
				){
				tab = SetTabLinesLabel(tab,"-1",GestionWeb.GetWebWord(1582,webSession.SiteLanguage),row,CstResults.DashBoard.ID_ELMT_L1_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L1_COLUMN_INDEX);						
					if(webSession.ComparativeStudy)
						tab = SetTabLinesLabel(tab,"-1",GestionWeb.GetWebWord(1582,webSession.SiteLanguage),row+1,CstResults.DashBoard.ID_ELMT_L1_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L1_COLUMN_INDEX);						
			}
				//Critères de calcul
			if(!webSession.DetailPeriodBeginningDate.Equals("0") && WebFunctions.CheckedText.IsStringEmpty(webSession.DetailPeriodBeginningDate)) {
				criteriaPeriodN =" period="+webSession.DetailPeriodBeginningDate.Substring(0,4);
				yearN1 = int.Parse(webSession.DetailPeriodBeginningDate.Substring(0,4))-1;
				if(webSession.ComparativeStudy)criteriaPeriodN1 =" period="+yearN1.ToString();
			}else{
				criteriaPeriodN =" period="+webSession.PeriodBeginningDate.Substring(0,4);	
				if(webSession.ComparativeStudy)criteriaPeriodN1 =" period="+yearN1.ToString();
			}
			switch(webSession.PreformatedTable){
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format :				
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format :		
					tab=BuildDataTableType_4_5_6(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType);					
					break;	
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay :				
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay :		
					tab=BuildDataTableType_7_8_9(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType);					
					break;
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice :																							
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:					
					tab = BuildDataTableType_10_11_12(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType);					
					break;
				default : 
					throw new DashBoardRulesException("GetDataTableType_4_5_7_8_10_11(WebSession webSession, DataTable dt,ref object[,] tab,ref int row,ref int col, int[] coordCellTab,bool isTotalLine,ClassificationCst.DB.Vehicles.names vehicleType,int yearN1) : Impossible d'identifier le tableau de bord à traiter.");
			}								
			// unité sélectionnée sur période N,N-1
			foreach(DataRow dr in  dt.Rows){
						
				#region traitement sélection nomenclature niveau 2 (centres d'intérêt ou support ou famille)
				//Traitement des centres d'intérêts et/ou supports ou famille
				if(oldIdL2!=null  && (GetL2Id(webSession).Length>0) && !oldIdL2.Contains(dr[GetL2Id(webSession)].ToString()) 
					&& dt.Columns.Contains(GetL2Id(webSession))){							
					oldRowL2 = row = row+1;
					tab = SetTabLinesLabel(tab,dr[GetL2Id(webSession)].ToString(),dr[GetL2Label(webSession)].ToString(),row,CstResults.DashBoard.ID_ELMT_L2_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L2_COLUMN_INDEX);
					if(webSession.ComparativeStudy)tab = SetTabLinesLabel(tab,dr[GetL2Id(webSession)].ToString(),dr[GetL2Label(webSession)].ToString(),row+1,CstResults.DashBoard.ID_ELMT_L2_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L2_COLUMN_INDEX);
					//Critères de calcul
					criteriaPeriodN ="  "+GetL2Id(webSession)+"="+ dr[GetL2Id(webSession)].ToString()+" AND period="+webSession.PeriodBeginningDate.Substring(0,4);	
					if(webSession.ComparativeStudy)criteriaPeriodN1 ="  "+GetL2Id(webSession)+"="+ dr[GetL2Id(webSession)].ToString()+" AND period="+yearN1.ToString();							
					//Traitement unités							
					switch(webSession.PreformatedTable){
						case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format :				
						case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format :		
							tab=BuildDataTableType_4_5_6(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType);					
							break;	
						case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay :				
						case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay :		
							tab=BuildDataTableType_7_8_9(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType);					
							break;
						case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice :																							
						case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:					
							tab = BuildDataTableType_10_11_12(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType);					
							break;
						default : 
							throw new DashBoardRulesException("GetDataTableType_4_5_7_8_10_11(WebSession webSession, DataTable dt,ref object[,] tab,ref int row,ref int col, int[] coordCellTab,bool isTotalLine,ClassificationCst.DB.Vehicles.names vehicleType,int yearN1) : Impossible d'identifier le tableau de bord à traiter.");
					}	
					oldIdL2.Add(dr[GetL2Id(webSession)].ToString());
				}
				#endregion	
						
				#region Traitement sélection nomenclature niveau 3
				//Traitement des supports
				if(webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format
					|| webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice
					|| webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay){
					if(oldIdL3!=null  && (GetL3Id(webSession).Length>0) && !oldIdL3.Contains(dr[GetL3Id(webSession)].ToString()) 
						&& dt.Columns.Contains(GetL3Id(webSession))){
						coordCellTab[0] = oldRowL2;
						oldRowL3 = row = row+1;
						tab = SetTabLinesLabel(tab,dr[GetL3Id(webSession)].ToString(),dr[GetL3Label(webSession)].ToString(),row,CstResults.DashBoard.ID_ELMT_L3_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L3_COLUMN_INDEX);
						if(webSession.ComparativeStudy)
							tab = SetTabLinesLabel(tab,dr[GetL3Id(webSession)].ToString(),dr[GetL3Label(webSession)].ToString(),row+1,CstResults.DashBoard.ID_ELMT_L3_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L3_COLUMN_INDEX);
						//Critères de calcul
						criteriaPeriodN = "  "+GetL2Id(webSession)+"="+ dr[GetL2Id(webSession)].ToString()+" AND "+GetL3Id(webSession)+"="+dr[GetL3Id(webSession)].ToString()+" AND period="+webSession.PeriodBeginningDate.Substring(0,4);	
						if(webSession.ComparativeStudy)criteriaPeriodN1 = "  "+GetL2Id(webSession)+"="+ dr[GetL2Id(webSession)].ToString()+" AND "+GetL3Id(webSession)+"="+dr[GetL3Id(webSession)].ToString()+" AND period="+yearN1.ToString();							 
						//Traitement unités
						switch(webSession.PreformatedTable){
							case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format :				
							case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format :		
								tab=BuildDataTableType_4_5_6(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType);					
								break;	
							case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay :				
							case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay :		
								tab=BuildDataTableType_7_8_9(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType);					
								break;
							case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice :																							
							case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:					
								tab = BuildDataTableType_10_11_12(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType);					
								break;
							default : 
								throw new DashBoardRulesException("GetDataTableType_4_5_7_8_10_11(WebSession webSession, DataTable dt,ref object[,] tab,ref int row,ref int col, int[] coordCellTab,bool isTotalLine,ClassificationCst.DB.Vehicles.names vehicleType,int yearN1) : Impossible d'identifier le tableau de bord à traiter.");
						}	
						oldIdL3.Add(dr[GetL3Id(webSession)].ToString());
					}
				}
				#endregion
			}
			return tab;
		}
		#endregion		

		#region tableau 13 -  ( rajouté 21/06/06)
		///<author>D. Mussuma</author>
		///  <since>22/06/2006</since>	
		/// <summary>
		/// Construit le tableau de résultats Media par famille de produits.
		/// </summary>				
		/// <param name="webSession">session du client</param>
		/// <param name="dt">table de données</param>
		/// <param name="tab">tableau de résultats</param>
		/// <param name="row">index ligne tableau de résultats</param>
		/// <param name="col">index colonne tableau de résultats</param>
		/// <param name="coordCellTab">Coordonnées d'un cellule du tableau de résultats</param>		
		/// <param name="sectorHashtable">Liste identifiants des familles</param>
		/// <param name="year">année N</param>
		/// <returns>tableau de résultats</returns>
//		private static object[,] BuildDataTableType_Media_X_Sector(WebSession webSession, DataTable dt,ref object[,] tab,ref int row,ref int col, int[] coordCellTab,ArrayList sectorIdList,int year){
		private static object[,] BuildDataTableType_Media_X_Sector(WebSession webSession, DataTable dt,ref object[,] tab,ref int row,ref int col, int[] coordCellTab,Hashtable sectorHashtable,int year){	
			#region variables
			CstWeb.CustomerSessions.Unit unit = webSession.Unit;
			string sumUnit = "";			
			int oldRowL1=0;
			int oldRowL2=0;
			int oldRowL3=0;		
//			ArrayList oldIdL2= new ArrayList();
//			ArrayList oldIdL3= new ArrayList();
			Int64 oldIdL3= -1,oldIdL2= -1;
			
			bool firstL2=true,firstL3=true;
			bool incrementRow=false;
//			string PeriodBeginningDate = "";
//			string PeriodEndDate ="";
//			string PeriodBeginningDateN1 = "";
//			string PeriodEndDateN1 ="";
			string criteriaPeriodN =" ";	
			string criteriaPeriodN1 =" ";
			string criteriaN="", criteriaN1="";
			coordCellTab[0] = oldRowL1 = row = 1;
			int yearN1=0;
			bool ContainsL2=false,ContainsL3=false;
		
			#endregion
				
			//Unité traitée
			sumUnit="sum("+unit.ToString()+")";
			
			//Critères de type période
//			CriteriaPeriod(webSession,ref PeriodBeginningDate,ref PeriodEndDate,year,false);
//			criteriaPeriodN = "period>="+PeriodBeginningDate+" AND period<="+PeriodEndDate;						
//			if(webSession.ComparativeStudy){
//				CriteriaPeriod(webSession,ref PeriodBeginningDateN1,ref PeriodEndDateN1,year,true);
//				criteriaPeriodN1 ="period>="+PeriodBeginningDateN1+" AND period<="+PeriodEndDateN1;			 											
//			}
			criteriaPeriodN=" period="+year.ToString();
			if(webSession.ComparativeStudy){
				yearN1=year-1;
				criteriaPeriodN1=" period="+yearN1.ToString();
			}

			//Idenbtifiant et nom du média sélectionné
			string vehicleId = ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
			string vehicleName = ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).Text.ToString();
			
			#region Insertion de valeurs dans ligne(s) total familles et total média
			//Total unité sélectionnée sur période N,N-1	
			tab = SetTabLinesLabel(tab,vehicleId,vehicleName,row,CstResults.DashBoard.ID_ELMT_L1_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L1_COLUMN_INDEX);
			if(webSession.ComparativeStudy)
				SetTabLinesLabel(tab,vehicleId,vehicleName,row+1,CstResults.DashBoard.ID_ELMT_L1_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L1_COLUMN_INDEX);
			criteriaN = criteriaPeriodN;
			criteriaN1 = criteriaPeriodN1;
//			tab = FillLineTableType_Media_X_Sector(webSession,dt,ref tab,ref row,ref col,sumUnit,criteriaN,criteriaN1,coordCellTab,true,sectorIdList);
			tab = FillLineTableType_Media_X_Sector(webSession,dt,ref tab,ref row,ref col,sumUnit,criteriaN,criteriaN1,coordCellTab,true,sectorHashtable);
			#endregion 

			//Insertions des valeurs dans chaque de chauqe niveau de la nomenclature support sélectionnée sur période N,N-1
			ContainsL2 = dt.Columns.Contains(GetL2Id(webSession));
			ContainsL3 = dt.Columns.Contains(GetL3Id(webSession));
			foreach(DataRow dr in  dt.Rows){
						
				#region Traitement sélection nomenclature niveau 2 (centres d'intérêt ou support)
				#region ancienne version
				//				//Traitement des centres d'intérêts et/ou supports 
				//				if(oldIdL2!=null  && (GetL2Id(webSession).Length>0) && !oldIdL2.Contains(dr[GetL2Id(webSession)].ToString()) 
				//					&& dt.Columns.Contains(GetL2Id(webSession))){							
				//					oldRowL2 = row = row+1;
				//					tab = SetTabLinesLabel(tab,dr[GetL2Id(webSession)].ToString(),dr[GetL2Label(webSession)].ToString(),row,CstResults.DashBoard.ID_ELMT_L2_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L2_COLUMN_INDEX);
				//					if(webSession.ComparativeStudy)tab = SetTabLinesLabel(tab,dr[GetL2Id(webSession)].ToString(),dr[GetL2Label(webSession)].ToString(),row+1,CstResults.DashBoard.ID_ELMT_L2_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L2_COLUMN_INDEX);
				//					
				//					//Critères de calcul
				//					criteriaN ="  "+GetL2Id(webSession)+"="+ dr[GetL2Id(webSession)].ToString()+" AND "+criteriaPeriodN;
				//					if(webSession.ComparativeStudy)criteriaN1 ="  "+GetL2Id(webSession)+"="+ dr[GetL2Id(webSession)].ToString()+" AND "+criteriaPeriodN1;						
				//					
				//					//Traitement unités							
				////					tab = FillLineTableType_Media_X_Sector(webSession,dt,ref tab,ref row,ref col,sumUnit,criteriaN,criteriaN1,coordCellTab,false,sectorIdList);
				//					tab = FillLineTableType_Media_X_Sector(webSession,dt,ref tab,ref row,ref col,sumUnit,criteriaN,criteriaN1,coordCellTab,false,sectorHashtable);
				//					oldIdL2.Add(dr[GetL2Id(webSession)].ToString());
				//				}
				#endregion

				//Traitement des centres d'intérêts ou supports 
				if((GetL2Id(webSession).Length>0) && dr[GetL2Id(webSession)]!=null  && ContainsL2 ){	
					
					if(oldIdL2!=Int64.Parse(dr[GetL2Id(webSession)].ToString())){
						if(!firstL2) {

							if(ContainsL3){
								//Calcul éventuel des PDM, Evol,Pourcentage pour élément niveau 3
								for(int k = CstResults.DashBoard.TOTAL_COLUMN_INDEX;k<= CstResults.DashBoard.TOTAL_COLUMN_INDEX+sectorHashtable.Count;k++){													
									if(k==CstResults.DashBoard.TOTAL_COLUMN_INDEX+sectorHashtable.Count)incrementRow=true;
									coordCellTab = InsertValue(webSession,ref tab,ref oldRowL3,ref k,coordCellTab,false,incrementRow);
									firstL3=true;
									oldIdL3=-1;
								}
								incrementRow=false;
							}

							//Calcul éventuel des PDM, Evol,Pourcentage pour élément niveau 2
							for(int k = CstResults.DashBoard.TOTAL_COLUMN_INDEX;k<= CstResults.DashBoard.TOTAL_COLUMN_INDEX+sectorHashtable.Count;k++){	
								if(k==CstResults.DashBoard.TOTAL_COLUMN_INDEX+sectorHashtable.Count)incrementRow=true;
								coordCellTab = InsertValue(webSession,ref tab,ref oldRowL2,ref k,coordCellTab,false,incrementRow);
							}
							incrementRow=false;
						}
					
						if(firstL2)oldRowL2 = row = row+1; 
						else oldRowL2 = row = GetCurrentRow(webSession,row);//	on change de ligne
						tab = SetTabLinesLabel(tab,dr[GetL2Id(webSession)].ToString(),dr[GetL2Label(webSession)].ToString(),row,CstResults.DashBoard.ID_ELMT_L2_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L2_COLUMN_INDEX);
						if(webSession.ComparativeStudy)tab = SetTabLinesLabel(tab,dr[GetL2Id(webSession)].ToString(),dr[GetL2Label(webSession)].ToString(),row+1,CstResults.DashBoard.ID_ELMT_L2_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L2_COLUMN_INDEX);
						firstL2=false;
					}
										
					//Calcul investissment année N et par famille
					FillCellTableType_Media_X_Sector(tab,oldRowL2,dr,year,sectorHashtable,webSession.Unit.ToString());					

					//Calcul investissment année N-1 et par famille
					if(webSession.ComparativeStudy)
						FillCellTableType_Media_X_Sector(tab,oldRowL2+1,dr,yearN1,sectorHashtable,webSession.Unit.ToString());	
				
					
					oldIdL2=Int64.Parse(dr[GetL2Id(webSession)].ToString());
					firstL2=false;
				}

				
				#endregion	
						
				#region Traitement sélection nomenclature niveau 3
				//Traitement des supports	
				#region ancienne version
				//				if(oldIdL3!=null  && (GetL3Id(webSession).Length>0) && !oldIdL3.Contains(dr[GetL3Id(webSession)].ToString()) 
				//					&& dt.Columns.Contains(GetL3Id(webSession))){
				//					coordCellTab[0] = oldRowL2;
				//					oldRowL3 = row = row+1;
				//					tab = SetTabLinesLabel(tab,dr[GetL3Id(webSession)].ToString(),dr[GetL3Label(webSession)].ToString(),row,CstResults.DashBoard.ID_ELMT_L3_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L3_COLUMN_INDEX);
				//					if(webSession.ComparativeStudy)
				//						tab = SetTabLinesLabel(tab,dr[GetL3Id(webSession)].ToString(),dr[GetL3Label(webSession)].ToString(),row+1,CstResults.DashBoard.ID_ELMT_L3_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L3_COLUMN_INDEX);
				//					//Critères de calcul
				//					criteriaN = "  "+GetL2Id(webSession)+"="+ dr[GetL2Id(webSession)].ToString()+" AND "+GetL3Id(webSession)+"="+dr[GetL3Id(webSession)].ToString()+" AND "+criteriaPeriodN;
				//					if(webSession.ComparativeStudy)criteriaN1 = "  "+GetL2Id(webSession)+"="+ dr[GetL2Id(webSession)].ToString()+" AND "+GetL3Id(webSession)+"="+dr[GetL3Id(webSession)].ToString()+" AND "+criteriaPeriodN1;
				//						
				//					//Traitement unités							
				////					tab = FillLineTableType_Media_X_Sector(webSession,dt,ref tab,ref row,ref col,sumUnit,criteriaN,criteriaN1,coordCellTab,false,sectorIdList);
				//					tab = FillLineTableType_Media_X_Sector(webSession,dt,ref tab,ref row,ref col,sumUnit,criteriaN,criteriaN1,coordCellTab,false,sectorHashtable);
				//					oldIdL3.Add(dr[GetL3Id(webSession)].ToString());
				//				}
				#endregion
				if( (GetL3Id(webSession).Length>0) &&  dr[GetL3Id(webSession)]!=null &&  ContainsL3 ){

					if(oldIdL3!=Int64.Parse(dr[GetL3Id(webSession)].ToString())){
						
						if(!firstL3) {
							//Calcul éventuel des PDM, Evol,Pourcentage
							for(int k = CstResults.DashBoard.TOTAL_COLUMN_INDEX;k<= CstResults.DashBoard.TOTAL_COLUMN_INDEX+sectorHashtable.Count;k++){														
								if(k==CstResults.DashBoard.TOTAL_COLUMN_INDEX+sectorHashtable.Count)incrementRow=true;
								coordCellTab = InsertValue(webSession,ref tab,ref oldRowL3,ref k,coordCellTab,false,incrementRow);
							}
							incrementRow=false;
						}

					
						coordCellTab[0] = oldRowL2;
//						if(firstL3 && firstL2)oldRowL3 = row = row+1;
//						else 
							oldRowL3 = row = GetCurrentRow(webSession,row);
						
						tab = SetTabLinesLabel(tab,dr[GetL3Id(webSession)].ToString(),dr[GetL3Label(webSession)].ToString(),row,CstResults.DashBoard.ID_ELMT_L3_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L3_COLUMN_INDEX);
						if(webSession.ComparativeStudy)
							tab = SetTabLinesLabel(tab,dr[GetL3Id(webSession)].ToString(),dr[GetL3Label(webSession)].ToString(),row+1,CstResults.DashBoard.ID_ELMT_L3_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L3_COLUMN_INDEX);
						
						firstL3=false;
					}

					//Calcul investissment année N et par famille
					FillCellTableType_Media_X_Sector(tab,oldRowL3,dr,year,sectorHashtable,webSession.Unit.ToString());					

					//Calcul investissment année N-1 et par famille
					if(webSession.ComparativeStudy)
					FillCellTableType_Media_X_Sector(tab,oldRowL3+1,dr,yearN1,sectorHashtable,webSession.Unit.ToString());	
					
					oldIdL3=Int64.Parse(dr[GetL3Id(webSession)].ToString());
					firstL3=false;
									
				}
				#endregion
			}

			//Dernière ligne niveau 2
			//Calcul éventuel des PDM, Evol,Pourcentage
			if(oldIdL2!=-1){
				for(int k = CstResults.DashBoard.TOTAL_COLUMN_INDEX;k<= CstResults.DashBoard.TOTAL_COLUMN_INDEX+sectorHashtable.Count;k++){										
					if(k==CstResults.DashBoard.TOTAL_COLUMN_INDEX+sectorHashtable.Count)incrementRow=true;
					coordCellTab = InsertValue(webSession,ref tab,ref oldRowL2,ref k,coordCellTab,false,incrementRow);
				}
					incrementRow=false;
			}

			//Dernière ligne niveau 3
			//Calcul éventuel des PDM, Evol,Pourcentage
			if(oldIdL3!=-1){
				for(int k = CstResults.DashBoard.TOTAL_COLUMN_INDEX;k<= CstResults.DashBoard.TOTAL_COLUMN_INDEX+sectorHashtable.Count;k++){										
					if(k==CstResults.DashBoard.TOTAL_COLUMN_INDEX+sectorHashtable.Count)incrementRow=true;
					coordCellTab = InsertValue(webSession,ref tab,ref oldRowL3,ref k,coordCellTab,false,incrementRow);
				}
				incrementRow=false;
			}

			return tab;
		}
		
		///<author>D. Mussuma</author>
		///  <since>22/06/2006</since>		
		/// <summary>
		/// Remplit une ligne du tableau de résultats avec en ligne les données concernant un média sur une période définie.
		/// </summary>	
		/// <param name="webSession">session du client</param>
		/// <param name="dt">table de données</param>
		/// <param name="tab">tableau de résultats</param>
		/// <param name="row">index ligne tableau de résultats</param>
		/// <param name="col">index colonne tableau de résultats</param>
		///<param name="operation">Operation Sql</param>
		/// <param name="criteriaN">Critère de sélection période N</param>
		/// <param name="criteriaN1">Critère de sélection période N-1</param>
		/// <param name="coordCellTab">coordonnées  de la dernière cellule traitée</param>
		/// <param name="isTotalLine">vrai si ligne totale</param>	
		/// <param name="sectorHashtable">Liste identifiants des familles associés à leur index de colonne dans le tableau de résultats</param>
		/// <returns>tableau de résultats</returns>
		private static object[,] FillLineTableType_Media_X_Sector(WebSession webSession, DataTable dt,ref object[,] tab,ref int row,ref int col,string operation,string criteriaN,string criteriaN1, int[] coordCellTab,bool isTotalLine,Hashtable sectorHashtable){
			#region ancienne version
//			bool incrementRow=false;
//			string tempCriteriaN="",tempCriteriaN1="";
//			//Total famille
//			col = CstResults.DashBoard.TOTAL_COLUMN_INDEX;		
//			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaN,criteriaN1,coordCellTab,isTotalLine,false);					
//			
//			//Pour chaque famille
//			for(int i=1;i<=sectorIdList.Count;i++){
//				col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+i;					
//				tempCriteriaN= criteriaN+" AND id_sector = "+sectorIdList[i-1].ToString();
//				tempCriteriaN1= criteriaN1+"  AND  id_sector = "+sectorIdList[i-1].ToString();
//				if(i==sectorIdList.Count)incrementRow=true;
//				coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,tempCriteriaN,tempCriteriaN1,coordCellTab,isTotalLine,incrementRow);					
//			}
			#endregion
			int i=0;
			bool incrementRow=false;
			string tempCriteriaN="",tempCriteriaN1="";
			//Total famille
			col = CstResults.DashBoard.TOTAL_COLUMN_INDEX;		
			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaN,criteriaN1,coordCellTab,isTotalLine,false);					
			
			//Pour chaque famille
			IEnumerator myEnumerator = sectorHashtable.GetEnumerator();				
			foreach (DictionaryEntry de in sectorHashtable ){
				i++;
				if(de.Value!=null && de.Key!=null ){
					col = int.Parse(de.Value.ToString());					
					tempCriteriaN= criteriaN+" AND id_sector = "+de.Key.ToString();
					tempCriteriaN1= criteriaN1+"  AND  id_sector = "+de.Key.ToString();
					if(i==sectorHashtable.Count)incrementRow=true;
					coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,tempCriteriaN,tempCriteriaN1,coordCellTab,isTotalLine,incrementRow);					
				}

			}

			return tab;
		}

		/// <summary>
		/// Remplit une cellule d'un tableau de type media par famille
		/// </summary>
		/// <param name="tab">tableau de résultats</param>
		/// <param name="row">index ligne du tableau</param>
		/// <param name="dr">ligne de données </param>
		/// <param name="year">année</param>
		/// <param name="sectorHashtable">liste des identifiants de familles associés à leur indexcolonne dans le tableau de résulats</param>
		/// <param name="unitLabel">libellé unité courant</param>
		private static void FillCellTableType_Media_X_Sector(object[,] tab, int row, DataRow dr,int year, Hashtable sectorHashtable,string unitLabel){
			int col= -1;
			if(sectorHashtable!=null && dr!=null && dr["id_sector"]!=System.DBNull.Value){
				col = int.Parse(sectorHashtable[dr["id_sector"].ToString()].ToString());

				if(dr["period"]!=System.DBNull.Value && dr["period"].ToString().Equals(year.ToString())){

					//Remplit les investissments d'une famille sur une période donnée et pour un média
					if( tab[row,col]==null && dr[unitLabel]!=System.DBNull.Value)
						tab[row,col]= double.Parse(dr[unitLabel].ToString());
					else if(dr[unitLabel]!=System.DBNull.Value)tab[row,col] = double.Parse(tab[row,col].ToString())+ double.Parse(dr[unitLabel].ToString());

					//Remplit les investissments pour total familles sur une période donnée et pour un média
					if( tab[row,CstResults.DashBoard.TOTAL_COLUMN_INDEX]==null && dr[unitLabel]!=System.DBNull.Value)
						tab[row,CstResults.DashBoard.TOTAL_COLUMN_INDEX]= double.Parse(dr[unitLabel].ToString());
					else if(dr[unitLabel]!=System.DBNull.Value)tab[row,CstResults.DashBoard.TOTAL_COLUMN_INDEX] = double.Parse(tab[row,CstResults.DashBoard.TOTAL_COLUMN_INDEX].ToString())+ double.Parse(dr[unitLabel].ToString());
				}
			}			

		}

		#endregion 
		
		#region tableaux 4,5,6
		/// <summary>
		/// Remplit des valeurs dans les cellules du tableau de résultats
		/// pour le type répartition format : tableaux 4 , 5 et 6
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dt">table de données</param>
		/// <param name="tab">tableau de résultats</param>
		/// <param name="row">index ligne tableau de résultats</param>
		/// <param name="col">index colonne tableau de résultats</param>
		/// <param name="operation">operation a effectuer</param>
		/// <param name="criteriaN">critère de sélection sur table de données période N</param>
		/// <param name="criteriaN1">critère de sélection sur table de données période N-1</param>
		/// <param name="coordCellTab">coordonnées  de la dernière cellule traitée</param>
		/// <param name="isTotalLine">vrai si ligne totale</param>	
		/// <param name="vehicleType">Type de média</param>
		/// <returns>tableau de résultats</returns>
		private static object[,] BuildDataTableType_4_5_6(WebSession webSession, DataTable dt,ref object[,] tab,ref int row,ref int col,string operation,string criteriaN,string criteriaN1, int[] coordCellTab,bool isTotalLine,ClassificationCst.DB.Vehicles.names vehicleType){
			string criteriaPeriodN = "";
			string criteriaPeriodN1 = "";
			string criteriaFormat="";

			//Total format
			col = CstResults.DashBoard.TOTAL_COLUMN_INDEX;
			criteriaPeriodN = criteriaN;
			criteriaPeriodN1 = criteriaN1;
			
			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);					
		
			//Format 1 à 9 sec
			col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+1;
			criteriaFormat= " repartition="+CstWeb.Repartition.Format.Spot_1_9.GetHashCode().ToString();		
			criteriaPeriodN= criteriaFormat+" AND "+criteriaN;
			criteriaPeriodN1= criteriaFormat+" AND "+criteriaN1;
			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);					
			//Format 10 sec
			col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+2;
			criteriaFormat= " repartition="+CstWeb.Repartition.Format.Spot_10.GetHashCode().ToString();		
			criteriaPeriodN= criteriaFormat+" AND "+criteriaN;
			criteriaPeriodN1= criteriaFormat+" AND "+criteriaN1;
			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);					
			//Format 11 à 19 sec
			col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+3;
			criteriaFormat= " repartition="+CstWeb.Repartition.Format.Spot_11_19.GetHashCode().ToString();		
			criteriaPeriodN= criteriaFormat+" AND "+criteriaN;
			criteriaPeriodN1= criteriaFormat+" AND "+criteriaN1;
			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);
			//Format 20 sec
			col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+4;
			criteriaFormat= " repartition="+CstWeb.Repartition.Format.Spot_20.GetHashCode().ToString();		
			criteriaPeriodN= criteriaFormat+" AND "+criteriaN;
			criteriaPeriodN1= criteriaFormat+" AND "+criteriaN1;
			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);							
			//Format 21 à 29 sec
			col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+5;
			criteriaFormat = " repartition="+CstWeb.Repartition.Format.Spot_21_29.GetHashCode().ToString();		
			criteriaPeriodN= criteriaFormat+" AND "+criteriaN;
			criteriaPeriodN1= criteriaFormat+" AND "+criteriaN1;
			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);	
			//Format 30 sec
			col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+6;
			criteriaFormat = " repartition="+CstWeb.Repartition.Format.Spot_30.GetHashCode().ToString();		
			criteriaPeriodN= criteriaFormat+" AND "+criteriaN;
			criteriaPeriodN1= criteriaFormat+" AND "+criteriaN1;
			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);	
			//Format 31 à 45 sec
			col = CstResults.DashBoard.TOTAL_COLUMN_INDEX+7;
			criteriaFormat = " repartition="+CstWeb.Repartition.Format.Spot_31_45.GetHashCode().ToString();		
			criteriaPeriodN= criteriaFormat+" AND "+criteriaN;
			criteriaPeriodN1= criteriaFormat+" AND "+criteriaN1;
			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);
			//Format plus de  45 sec
			col = CstResults.DashBoard.TOTAL_COLUMN_INDEX+8;
			criteriaFormat = " repartition="+CstWeb.Repartition.Format.Spot_45.GetHashCode().ToString();		
			criteriaPeriodN= criteriaFormat+" AND "+criteriaN;
			criteriaPeriodN1= criteriaFormat+" AND "+criteriaN1;
			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,true);																													
			
			return tab;
		}

		#endregion	

		#region tableaux 7,8,9
		/// <summary>
		/// Remplit des valeurs dans les cellules du tableau de résultats
		/// pour le type répartition format : tableaux 7 , 8 et 9
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dt">table de données</param>
		/// <param name="tab">tableau de résultats</param>
		/// <param name="row">index ligne tableau de résultats</param>
		/// <param name="col">index colonne tableau de résultats</param>
		/// <param name="criteriaN">critère de sélection sur table de données période N</param>
		/// <param name="criteriaN1">critère de sélection sur table de données période N-1</param>
		/// <param name="coordCellTab">coordonnées  de la dernière cellule traitée</param>
		/// <param name="isTotalLine">vrai si ligne totale</param>	
		/// <param name="operation">operation à effectuer sur les données</param>
		/// <param name="vehicleType">type du média sélectionné</param>
		/// <returns>tableau de résultats</returns>
		private static object[,] BuildDataTableType_7_8_9(WebSession webSession, DataTable dt,ref object[,] tab,ref int row,ref int col,string operation,string criteriaN,string criteriaN1, int[] coordCellTab,bool isTotalLine,ClassificationCst.DB.Vehicles.names vehicleType){
			string criteriaPeriodN = "";
			string criteriaPeriodN1 = "";
			string criteriaNamedDay="";
			
			//semaine
			col = CstResults.DashBoard.TOTAL_COLUMN_INDEX;
			criteriaPeriodN = criteriaN;
			criteriaPeriodN1 = criteriaN1;

			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);					
		
			//Semaine 5 jour
			col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+1;
			criteriaNamedDay= " repartition>="+CstWeb.Repartition.namedDay.Monday.GetHashCode().ToString()
				+ "	AND	repartition<="+CstWeb.Repartition.namedDay.Friday.GetHashCode().ToString();
			criteriaPeriodN= criteriaNamedDay+" AND "+criteriaN;
			criteriaPeriodN1= criteriaNamedDay+" AND "+criteriaN1;
			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);					
			
			//lundi
			col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+2;
			criteriaNamedDay= " repartition="+CstWeb.Repartition.namedDay.Monday.GetHashCode().ToString();		
			criteriaPeriodN=criteriaNamedDay+" AND "+criteriaN;
			criteriaPeriodN1= criteriaNamedDay+" AND "+criteriaN1;
			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);					
			
			//mardi
			col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+3;
			criteriaNamedDay= " repartition="+CstWeb.Repartition.namedDay.Tuesday.GetHashCode().ToString();		
			criteriaPeriodN= criteriaNamedDay+" AND "+criteriaN;
			criteriaPeriodN1= criteriaNamedDay+" AND "+criteriaN1;
			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);
			
			//mercredi
			col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+4;
			criteriaNamedDay= " repartition="+CstWeb.Repartition.namedDay.Wednesdays.GetHashCode().ToString();		
			criteriaPeriodN= criteriaNamedDay+" AND "+criteriaN;
			criteriaPeriodN1= criteriaNamedDay+" AND "+criteriaN1;
			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);							
			
			//jeudi
			col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+5;
			criteriaNamedDay= " repartition="+CstWeb.Repartition.namedDay.Thursday.GetHashCode().ToString();
			criteriaPeriodN= criteriaNamedDay+" AND "+criteriaN;
			criteriaPeriodN1= criteriaNamedDay+" AND "+criteriaN1;
			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);	
			
			//vendredi
			col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+6;
			criteriaNamedDay= " repartition="+CstWeb.Repartition.namedDay.Friday.GetHashCode().ToString();
			criteriaPeriodN= criteriaNamedDay+" AND "+criteriaN;
			criteriaPeriodN1= criteriaNamedDay+" AND "+criteriaN1;
			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);	
			
			//Week end
			col = CstResults.DashBoard.TOTAL_COLUMN_INDEX+7;
			criteriaNamedDay= " repartition>="+CstWeb.Repartition.namedDay.Saturday.GetHashCode().ToString()
				+ " AND repartition<="+CstWeb.Repartition.namedDay.Sunday.GetHashCode().ToString();
			criteriaPeriodN= criteriaNamedDay+" AND "+criteriaN;
			criteriaPeriodN1= criteriaNamedDay+" AND "+criteriaN1;
			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);
			
			//samedi
			col = CstResults.DashBoard.TOTAL_COLUMN_INDEX+8;
			criteriaNamedDay= " repartition="+CstWeb.Repartition.namedDay.Saturday.GetHashCode().ToString();
			criteriaPeriodN= criteriaNamedDay+" AND "+criteriaN;
			criteriaPeriodN1= criteriaNamedDay+" AND "+criteriaN1;
			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);																													
			
			//Dimanche
			col = CstResults.DashBoard.TOTAL_COLUMN_INDEX+9;
			criteriaNamedDay= " repartition="+CstWeb.Repartition.namedDay.Sunday.GetHashCode().ToString();
			criteriaPeriodN= criteriaNamedDay+" AND "+criteriaN;
			criteriaPeriodN1= criteriaNamedDay+" AND "+criteriaN1;
			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,true);																													
	
			return tab;
		}

		#endregion

		#region tableaux 10,11,12
		/// <summary>
		/// Remplit des valeurs dans les cellules du tableau de résultats
		/// pour le type répartition format : tableaux 10 , 11 et 12
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dt">table de données</param>
		/// <param name="tab">tableau de résultats</param>
		/// <param name="row">index ligne tableau de résultats</param>
		/// <param name="col">index colonne tableau de résultats</param>
		/// <param name="criteriaN">critère de sélection sur table de données période N</param>
		/// <param name="criteriaN1">critère de sélection sur table de données période N-1</param>
		/// <param name="coordCellTab">coordonnées  de la dernière cellule traitée</param>
		/// <param name="isTotalLine">vrai si ligne totale</param>	
		/// <param name="operation">operation à effectuer sur les données</param>
		/// <param name="vehicleType">type du média sélectionné</param>
		/// <returns>tableau de résultats</returns>
		private static object[,] BuildDataTableType_10_11_12(WebSession webSession, DataTable dt,ref object[,] tab,ref int row,ref int col,string operation,string criteriaN,string criteriaN1, int[] coordCellTab,bool isTotalLine,ClassificationCst.DB.Vehicles.names vehicleType){
			string criteriaPeriodN = "";
			string criteriaPeriodN1 = "";
			string criteriaTimeSlice="";
			switch(vehicleType){					
				
				case ClassificationCst.DB.Vehicles.names.tv:
				case ClassificationCst.DB.Vehicles.names.others:
					#region tranches horaires tv
					//Total
					col = CstResults.DashBoard.TOTAL_COLUMN_INDEX;
					criteriaPeriodN = criteriaN;
					criteriaPeriodN1 = criteriaN1;

					coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);					
		
					//7h à 12 h 
					col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+1;
					criteriaTimeSlice= " repartition="+CstWeb.Repartition.timeInterval.Slice_7h_12h.GetHashCode().ToString();						
					criteriaPeriodN= criteriaTimeSlice+" AND "+criteriaN;
					criteriaPeriodN1= criteriaTimeSlice+" AND "+criteriaN1;
					coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);					
			
					//12h à 14h
					col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+2;
					criteriaTimeSlice= " repartition="+CstWeb.Repartition.timeInterval.Slice_12h_14h.GetHashCode().ToString();						
					criteriaPeriodN=criteriaTimeSlice+" AND "+criteriaN;
					criteriaPeriodN1= criteriaTimeSlice+" AND "+criteriaN1;
					coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);					
			
					//14h à 17h
					col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+3;
					criteriaTimeSlice= " repartition="+CstWeb.Repartition.timeInterval.Slice_14h_17h.GetHashCode().ToString();						
					criteriaPeriodN= criteriaTimeSlice+" AND "+criteriaN;
					criteriaPeriodN1= criteriaTimeSlice+" AND "+criteriaN1;
					coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);
			
					//17h à 19h
					col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+4;					
					criteriaTimeSlice= " repartition="+CstWeb.Repartition.timeInterval.Slice_17h_19h.GetHashCode().ToString();						
					criteriaPeriodN= criteriaTimeSlice+" AND "+criteriaN;
					criteriaPeriodN1= criteriaTimeSlice+" AND "+criteriaN1;
					coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);
			
					//19h à 22h
					col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+5;
					criteriaTimeSlice= " repartition="+CstWeb.Repartition.timeInterval.Slice_19h_22h.GetHashCode().ToString();						
					criteriaPeriodN= criteriaTimeSlice+" AND "+criteriaN;
					criteriaPeriodN1= criteriaTimeSlice+" AND "+criteriaN1;
					coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);
			
					//22h à 24h
					col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+6;
					criteriaTimeSlice= " repartition="+CstWeb.Repartition.timeInterval.Slice_22h_24h.GetHashCode().ToString();						
					criteriaPeriodN= criteriaTimeSlice+" AND "+criteriaN;
					criteriaPeriodN1= criteriaTimeSlice+" AND "+criteriaN1;
					coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);
					
					//24h à 7h
					col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+7;
					criteriaTimeSlice= " repartition="+CstWeb.Repartition.timeInterval.Slice_24h_7h.GetHashCode().ToString();						
					criteriaPeriodN= criteriaTimeSlice+" AND "+criteriaN;
					criteriaPeriodN1= criteriaTimeSlice+" AND "+criteriaN1;
					coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,true);

					break;
					#endregion
	
				case ClassificationCst.DB.Vehicles.names.radio:
					#region tranche horaires radio
					//Total
					col = CstResults.DashBoard.TOTAL_COLUMN_INDEX;
					criteriaPeriodN = criteriaN;
					criteriaPeriodN1 = criteriaN1;

					//Vérifie la présence de données pour la ligne en cours
					//					if(!HasValues(webSession,dt,criteriaN,operation))
					//						tab = SetRowNull(tab,row);
					//					if(!HasValues(webSession,dt,criteriaN1,operation) && webSession.ComparativeStudy)
					//						tab = SetRowNull(tab,row+1);

					coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);					
		
					//5h à 6h59 
					col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+1;
					criteriaTimeSlice= " repartition="+CstWeb.Repartition.timeInterval.Slice_5h_6h59.GetHashCode().ToString();						
					criteriaPeriodN= criteriaTimeSlice+" AND "+criteriaN;
					criteriaPeriodN1= criteriaTimeSlice+" AND "+criteriaN1;
					coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);					
			
					//7h à 8h59
					col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+2;
					criteriaTimeSlice= " repartition="+CstWeb.Repartition.timeInterval.Slice_7h_8h59.GetHashCode().ToString();						
					criteriaPeriodN=criteriaTimeSlice+" AND "+criteriaN;
					criteriaPeriodN1= criteriaTimeSlice+" AND "+criteriaN1;
					coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);					
			
					//9h à 12h59
					col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+3;
					criteriaTimeSlice= " repartition="+CstWeb.Repartition.timeInterval.Slice_9h_12h59.GetHashCode().ToString();						
					criteriaPeriodN= criteriaTimeSlice+" AND "+criteriaN;
					criteriaPeriodN1= criteriaTimeSlice+" AND "+criteriaN1;
					coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);
			
					//13h à 18h59
					col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+4;					
					criteriaTimeSlice= " repartition="+CstWeb.Repartition.timeInterval.Slice_13h_18h59.GetHashCode().ToString();						
					criteriaPeriodN= criteriaTimeSlice+" AND "+criteriaN;
					criteriaPeriodN1= criteriaTimeSlice+" AND "+criteriaN1;
					coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);
			
					//19h à 24h
					col =  CstResults.DashBoard.TOTAL_COLUMN_INDEX+5;
					criteriaTimeSlice= " repartition="+CstWeb.Repartition.timeInterval.Slice_19h_24h.GetHashCode().ToString();						
					criteriaPeriodN= criteriaTimeSlice+" AND "+criteriaN;
					criteriaPeriodN1= criteriaTimeSlice+" AND "+criteriaN1;
					coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,true);							
					break;
					#endregion
				default:
					throw(new DashBoardDataAccessException("BuildDataTableType_10_11_12(WebSession webSession, DataTable dt,ref object[,] tab,ref int row,ref int col,string operation,string criteriaN,string criteriaN1, int[] coordCellTab,bool isTotalLine,ClassificationCst.DB.Vehicles.names vehicleType) : Impossible de déterminer le type de média à traiter."));
			}
			
			
			return tab;
		}

		#endregion

		#region tableaux unité X repartition 6,9,12
		/// <summary>
		/// Construit lese tableaux de bord Unité X Repartition 
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dt">table de données</param>
		/// <param name="tab">tableau de résultats</param>
		/// <param name="row">index ligne tableau de résultats</param>
		/// <param name="col">index colonne tableau de résultats</param>	
		/// <param name="coordCellTab">coordonnées  de la dernière cellule traitée</param>
		/// <param name="isTotalLine">vrai si ligne totale</param>	
		/// <param name="vehicleType">type de média</param>
		/// <param name="yearN1">année précédente</param>
		/// <returns>tableau de résultats</returns>
		private static object[,] GetDataTableType_6_9_12(WebSession webSession, DataTable dt,ref object[,] tab,ref int row,ref int col, int[] coordCellTab,bool isTotalLine,ClassificationCst.DB.Vehicles.names vehicleType,int yearN1){
			string criteriaPeriodN="";
			string criteriaPeriodN1 ="";
			string operation="";
			
			row = 1;								
			//unité euro	
			tab = SetTabLinesLabel(tab,"1",GestionWeb.GetWebWord(1423,webSession.SiteLanguage),row,CstResults.DashBoard.ID_ELMT_L2_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L2_COLUMN_INDEX);
			if(webSession.ComparativeStudy)tab = SetTabLinesLabel(tab,"1",GestionWeb.GetWebWord(1423,webSession.SiteLanguage),row+1,CstResults.DashBoard.ID_ELMT_L2_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L2_COLUMN_INDEX);
			operation="sum(euro)";
			if(!webSession.DetailPeriodBeginningDate.Equals("0") && WebFunctions.CheckedText.IsStringEmpty(webSession.DetailPeriodBeginningDate)) {
				criteriaPeriodN =" period="+webSession.DetailPeriodBeginningDate.Substring(0,4);
				yearN1 = int.Parse(webSession.DetailPeriodBeginningDate.Substring(0,4))-1;
				if(webSession.ComparativeStudy)criteriaPeriodN1 =" period="+yearN1.ToString();
			}else{
				criteriaPeriodN =" period="+webSession.PeriodBeginningDate.Substring(0,4);	
				if(webSession.ComparativeStudy)criteriaPeriodN1 =" period="+yearN1.ToString();
			}
			
			switch(webSession.PreformatedTable){
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format :
					tab=BuildDataTableType_4_5_6(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType);					
					break;	
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay :
					tab=BuildDataTableType_7_8_9(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType);					
					break;
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice :
					tab = BuildDataTableType_10_11_12(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType);					
					break;
				default : 
					throw new DashBoardRulesException("GetDataTableType_6_9_12(WebSession webSession, DataTable dt,ref object[,] tab,ref int row,ref int col, int[] coordCellTab,bool isTotalLine,ClassificationCst.DB.Vehicles.names vehicleType,int yearN1) : Impossible d'identifier le tableau de bord à traiter.");
			}
			//unité durée
			row=row+1;
			tab = SetTabLinesLabel(tab,"2",GestionWeb.GetWebWord(1435,webSession.SiteLanguage),row,CstResults.DashBoard.ID_ELMT_L2_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L2_COLUMN_INDEX);
			if(webSession.ComparativeStudy)tab = SetTabLinesLabel(tab,"2",GestionWeb.GetWebWord(1435,webSession.SiteLanguage),row+1,CstResults.DashBoard.ID_ELMT_L2_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L2_COLUMN_INDEX);
			operation="sum(duration)";
			criteriaPeriodN ="  period="+webSession.PeriodBeginningDate.Substring(0,4);	
			criteriaPeriodN1 =" period="+yearN1.ToString();										
			switch(webSession.PreformatedTable){
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format :
					tab=BuildDataTableType_4_5_6(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType);					
					break;	
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay :
					tab=BuildDataTableType_7_8_9(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType);					
					break;
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice :
					tab = BuildDataTableType_10_11_12(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType);					
					break;
				default : 
					throw new DashBoardRulesException("GetDataTableType_6_9_12(WebSession webSession, DataTable dt,ref object[,] tab,ref int row,ref int col, int[] coordCellTab,bool isTotalLine,ClassificationCst.DB.Vehicles.names vehicleType,int yearN1) : Impossible d'identifier le tableau de bord à traiter.");
			}

			//unité insertion
			row=row+1;
			tab = SetTabLinesLabel(tab,"3",GestionWeb.GetWebWord(939,webSession.SiteLanguage),row,CstResults.DashBoard.ID_ELMT_L2_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L2_COLUMN_INDEX);					
			if(webSession.ComparativeStudy)tab = SetTabLinesLabel(tab,"3",GestionWeb.GetWebWord(939,webSession.SiteLanguage),row+1,CstResults.DashBoard.ID_ELMT_L2_COLUMN_INDEX,CstResults.DashBoard.LABEL_ELMT_L2_COLUMN_INDEX);
			operation="sum(insertion)";
			criteriaPeriodN ="  period="+webSession.PeriodBeginningDate.Substring(0,4);	
			if(webSession.ComparativeStudy)criteriaPeriodN1 =" period="+yearN1.ToString();										
			switch(webSession.PreformatedTable){
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format :
					tab=BuildDataTableType_4_5_6(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType);					
					break;	
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay :
					tab=BuildDataTableType_7_8_9(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType);					
					break;
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice :
					tab = BuildDataTableType_10_11_12(webSession,dt,ref tab,ref row,ref col,operation,criteriaPeriodN,criteriaPeriodN1,coordCellTab,false,vehicleType);					
					break;
				default : 
					throw new DashBoardRulesException("GetDataTableType_6_9_12(WebSession webSession, DataTable dt,ref object[,] tab,ref int row,ref int col, int[] coordCellTab,bool isTotalLine,ClassificationCst.DB.Vehicles.names vehicleType,int yearN1) : Impossible d'identifier le tableau de bord à traiter.");
			}
			return tab;
		}
		#endregion

		#region Traitement des colonnes unités

		/// <summary>
		/// Remplit des valeurs dans les cellules du tableau de résultats
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dt">table de données</param>
		/// <param name="tab">tableau de résultats</param>
		/// <param name="row">index ligne tableau de résultats</param>
		/// <param name="col">index colonne tableau de résultats</param>		
		/// <param name="criteriaPeriodN">critère de sélection sur table de données période N</param>
		/// <param name="criteriaPeriodN1">critère de sélection sur table de données période N-1</param>
		/// <param name="coordCellTab">coordonnées  de la dernière cellule traitée</param>
		/// <returns>coordonnées des index de la dernière cellule traitée</returns>
		/// <param name="isTotalLine">vrai si ligne totale</param>	
		/// <param name="vehicleType">type de média</param>
		/// <returns>tableau de résultats</returns>
		private static object[,] FillTabUnits(WebSession webSession, DataTable dt,ref object[,] tab,ref int row,ref int col,string criteriaPeriodN,string criteriaPeriodN1, int[] coordCellTab,bool isTotalLine,ClassificationCst.DB.Vehicles.names vehicleType){

			//Total euro
			col = CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS;
			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,"Sum(euro)",criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);					

			if(ClassificationCst.DB.Vehicles.names.press==vehicleType){
				//Total Mm/Col sur période N,N-1
				col = CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS+1;
				coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,"Sum(mmPerCol)",criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);				

				//Total  pages période N,N-1
				col = CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS+2;
				coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,"Sum(pages)",criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);
				//Total  insertion période N,N-1
				col = CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS+3;
				coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,"Sum(insertion)",criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,true);
				//Vérifie la présence de données pour les lignes traitées
							
			}else{							
				//Total  durée période N,N-1	
				col = CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS+1;
				coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,"Sum(duration)",criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);
				col = CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS+2;
				//Total insertion/spot période N-1
				coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,"Sum(insertion)",criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,true);
			}		

			return tab;
		}
		#endregion

		#region Traitement des colonnes unités ForGenericUI

//		/// <summary>
//		/// Remplit des valeurs dans les cellules du tableau de résultats
//		/// </summary>
//		/// <param name="webSession">session du client</param>
//		/// <param name="dt">table de données</param>
//		/// <param name="tab">tableau de résultats</param>
//		/// <param name="row">index ligne tableau de résultats</param>
//		/// <param name="col">index colonne tableau de résultats</param>		
//		/// <param name="criteriaPeriodN">critère de sélection sur table de données période N</param>
//		/// <param name="criteriaPeriodN1">critère de sélection sur table de données période N-1</param>
//		/// <param name="coordCellTab">coordonnées  de la dernière cellule traitée</param>
//		/// <returns>coordonnées des index de la dernière cellule traitée</returns>
//		/// <param name="isTotalLine">vrai si ligne totale</param>	
//		/// <param name="vehicleType">type de média</param>
//		/// <returns>tableau de résultats</returns>
//		private static ResultTable FillTabUnitsForGenericUI(WebSession webSession, DataTable dt,ref ResultTable tab,ref int row,ref int col,string criteriaPeriodN,string criteriaPeriodN1, int[] coordCellTab,bool isTotalLine,ClassificationCst.DB.Vehicles.names vehicleType)
//		{
//
//			//Total euro
//			col = CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS;
//			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,"Sum(euro)",criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);					
//
//			if(ClassificationCst.DB.Vehicles.names.press==vehicleType)
//			{
//				//Total Mm/Col sur période N,N-1
//				col = CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS+1;
//				coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,"Sum(mmPerCol)",criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);				
//
//				//Total  pages période N,N-1
//				col = CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS+2;
//				coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,"Sum(pages)",criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);
//				//Total  insertion période N,N-1
//				col = CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS+3;
//				coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,"Sum(insertion)",criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,true);
//				//Vérifie la présence de données pour les lignes traitées
//							
//			}
//			else
//			{							
//				//Total  durée période N,N-1	
//				col = CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS+1;
//				coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,"Sum(duration)",criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);
//				col = CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS+2;
//				//Total insertion/spot période N-1
//				coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,"Sum(insertion)",criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,true);
//			}		
//
//			return tab;
//		}
		#endregion

		#endregion

		#region Traitement des colonnes périodes (mois  ou semaines)
		/// <summary>
		/// Remplit des valeurs dans les cellules du tableau de résultats 
		/// pour chauqe colonne correspondant à 1 mois ou une semaine
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dt">table de données</param>
		/// <param name="tab">tableau de résultats</param>
		/// <param name="row">index ligne</param>
		/// <param name="col">index colonne </param>
		/// <param name="criteriaN">critère période N</param>
		/// <param name="criteriaN1">critère période N1</param>
		/// <param name="coordCellTab">coordonnées de la cellule de l'élément parent</param>
		/// <param name="isTotalLine">vrai si ligne totale</param>
		/// <param name="vehicleType">type du média</param>
		/// <param name="yearN1">année précédente</param>
		/// <param name="year">année courante</param>
		/// <returns>tableau de résultats</returns>
		private static object[,] FillTabUnitByPeriod(WebSession webSession, DataTable dt,ref object[,] tab,ref int row,ref int col,string criteriaN,string criteriaN1, int[] coordCellTab,bool isTotalLine,ClassificationCst.DB.Vehicles.names vehicleType,int yearN1,int year)
		{
			
			string detailDate="";
			AtomicPeriodWeek week;
			AtomicPeriodWeek weekN1;
			CstWeb.CustomerSessions.Unit unit = webSession.Unit;
			string sumUnit = "sum("+unit.ToString()+")";
			string criteriaPeriodN = "";
			string criteriaPeriodN1 = "";
			string PeriodBeginningDate = "";
			string PeriodEndDate ="";
			bool incrementRow=false;
			int j=1;

			//Traitement Colonne Total
			col = CstResults.DashBoard.TOTAL_COLUMN_INDEX;
			//Critères de calcul			
			CriteriaPeriod(webSession,ref PeriodBeginningDate,ref PeriodEndDate,year,false);
			criteriaPeriodN = "period>="+PeriodBeginningDate+" AND period<="+PeriodEndDate;									
				CriteriaPeriod(webSession,ref PeriodBeginningDate,ref PeriodEndDate,year,true);
				criteriaPeriodN1 ="period>="+PeriodBeginningDate+" AND period<="+PeriodEndDate;			 														
			if(DashBoardDataAccess.IsCrossRepartitionType(webSession)){
				criteriaPeriodN+=criteriaN;
				criteriaPeriodN1+=criteriaN1;	
			}

			if(criteriaN.Length>0)criteriaPeriodN = criteriaPeriodN+"  "+criteriaN;
			if(criteriaN1.Length>0)criteriaPeriodN1 = criteriaPeriodN1+"  "+criteriaN1;				

			//Calcul effectué
			coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,sumUnit,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,false);					
			
			//Traitement de chaque colonne hebdomadaire ou mensuel
			if(webSession.DetailPeriod==CstPeriodDetail.monthly){
				
				//libellés mois				
				for(int i=int.Parse(webSession.PeriodBeginningDate.Substring(4,2));i<=int.Parse(webSession.PeriodEndDate.Substring(4,2));i++){					
					//Critères de calcul pour chaque mois à afficher
					if(DashBoardDataAccess.IsCrossRepartitionType(webSession)){
						detailDate = (i.ToString().Length>1) ? "period>="+webSession.PeriodBeginningDate.Substring(0,4)+i.ToString()+"01 AND period<="+webSession.PeriodBeginningDate.Substring(0,4)+i.ToString()+"31"
							: "period>="+webSession.PeriodBeginningDate.Substring(0,4)+"0"+i.ToString()+"01 AND "+"period<="+webSession.PeriodBeginningDate.Substring(0,4)+"0"+i.ToString()+"31";
						criteriaPeriodN = detailDate+criteriaN;
						if(webSession.ComparativeStudy){
							detailDate = (i.ToString().Length>1) ? "period>="+yearN1.ToString()+i.ToString()+"01 AND period<="+yearN1.ToString()+i.ToString()+"31"
								: "period>="+yearN1.ToString()+"0"+i.ToString()+"01 AND "+"period<="+yearN1.ToString()+"0"+i.ToString()+"31";							
							criteriaPeriodN1 = detailDate+criteriaN1;
						}
					}
					else {detailDate = (i.ToString().Length>1) ? webSession.PeriodBeginningDate.Substring(0,4)+i.ToString():webSession.PeriodBeginningDate.Substring(0,4)+"0"+i.ToString();
						criteriaPeriodN = "period="+detailDate+criteriaN;
						if(webSession.ComparativeStudy){
							detailDate = (i.ToString().Length>1) ? yearN1+i.ToString():yearN1+"0"+i.ToString();
							criteriaPeriodN1 = "period="+detailDate+criteriaN1;
						}
					}					
					col = CstResults.DashBoard.TOTAL_COLUMN_INDEX+j;
					//Calcul effectué
					if(i==int.Parse(webSession.PeriodEndDate.Substring(4,2)))incrementRow=true;
					coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,sumUnit,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,incrementRow);																	
					j++;
				}
			}else if(webSession.DetailPeriod==CstPeriodDetail.weekly){
				//libellés semaines				
				week = new AtomicPeriodWeek(int.Parse(webSession.PeriodBeginningDate.Substring(0,4)),int.Parse(webSession.PeriodBeginningDate.Substring(4,2)));	
				weekN1 = new AtomicPeriodWeek(int.Parse(webSession.PeriodEndDate.Substring(0,4)),int.Parse(webSession.PeriodEndDate.Substring(4,2)));									
				for(int i=int.Parse(webSession.PeriodBeginningDate.Substring(4,2));i<=int.Parse(webSession.PeriodEndDate.Substring(4,2));i++){																		
					//Critères de calcul pour chaque semaine à afficher
					week = new AtomicPeriodWeek(int.Parse(webSession.PeriodBeginningDate.Substring(0,4)),i);
					if(DashBoardDataAccess.IsCrossRepartitionType(webSession)){
						detailDate ="period>="+week.FirstDay.Year.ToString()+(week.FirstDay.Month.ToString().Length>1?week.FirstDay.Month.ToString():"0"+week.FirstDay.Month.ToString())
							+(week.FirstDay.Day.ToString().Length>1?week.FirstDay.Day.ToString():"0"+week.FirstDay.Day.ToString())+" AND period<="+week.LastDay.Year.ToString()
							+(week.LastDay.Month.ToString().Length>1 ? week.LastDay.Month.ToString(): "0"+week.LastDay.Month.ToString())
							+(week.LastDay.Day.ToString().Length>1? week.LastDay.Day.ToString() : "0"+week.LastDay.Day.ToString() );
						criteriaPeriodN = detailDate+criteriaN;
						if(webSession.ComparativeStudy){
							try{
								week.SubWeek(52);
								weekN1 = week;
								detailDate ="period>="+weekN1.FirstDay.Year.ToString()+(weekN1.FirstDay.Month.ToString().Length>1?weekN1.FirstDay.Month.ToString():"0"+weekN1.FirstDay.Month.ToString())
									+(weekN1.FirstDay.Day.ToString().Length>1?weekN1.FirstDay.Day.ToString():"0"+weekN1.FirstDay.Day.ToString())+" AND period<="+weekN1.LastDay.Year.ToString()
									+(weekN1.LastDay.Month.ToString().Length>1? weekN1.LastDay.Month.ToString() : "0"+weekN1.LastDay.Month.ToString())
									+(weekN1.LastDay.Day.ToString().Length>1? weekN1.LastDay.Day.ToString(): "0"+weekN1.LastDay.Day.ToString() );
								criteriaPeriodN1 = detailDate+criteriaN1;
							}catch(Exception){
								criteriaPeriodN1 = "period=99999999";//Pas de semaine correspondante pour période N-1								
							}
						}							
					}else{
						detailDate = (i.ToString().Length>1) ? webSession.PeriodBeginningDate.Substring(0,4)+i.ToString():webSession.PeriodBeginningDate.Substring(0,4)+"0"+i.ToString();
						criteriaPeriodN = "period="+detailDate+criteriaN;
						if(webSession.ComparativeStudy){
							week.SubWeek(52);
							detailDate = week.Year.ToString()+ (week.Week.ToString().Length>1 ? week.Week.ToString() : "0"+week.Week.ToString()); //! Avérifier
							criteriaPeriodN1 = "period="+detailDate+criteriaN1;
						}
					}
					col = CstResults.DashBoard.TOTAL_COLUMN_INDEX+j;
					//Calcul effectué
					if(i==int.Parse(webSession.PeriodEndDate.Substring(4,2)))incrementRow=true;
					coordCellTab = InsertValue(webSession,dt,ref tab,ref row,ref col,sumUnit,criteriaPeriodN,criteriaPeriodN1,coordCellTab,isTotalLine,incrementRow);																	
					j++;
				}
			}			
			return tab;
		}
		#endregion

		#endregion

		#region insertion d'une valeur dans tableau
		/// <summary>
		/// Insert des valeurs dans les cellules du tableau de résultats
		/// </summary>
		/// <param name="webSession">session du client</param>		
		/// <param name="tab">tableau de résultats</param>
		/// <param name="row">index ligne tableau de résultats</param>
		/// <param name="col">index colonne tableau de résultats</param>
		/// <param name="coordCellTab">coordonnées de la cellule de l'élément parent</param>
		/// <param name="isTotalLine">vrai si ligne totale</param>
		/// <param name="incrementRow">change de ligne</param>		
		/// <returns>coordonnées des index de la fdernière cellule traitée</returns>
		private static int[] InsertValue(WebSession webSession, ref object[,] tab,ref int row,ref int col,int[] coordCellTab,bool isTotalLine,bool incrementRow){
			return  InsertValue(webSession,null,ref tab,ref row,ref col,"","","",coordCellTab,isTotalLine,incrementRow);
		}

		/// <summary>
		/// Insert des valeurs dans les cellules du tableau de résultats
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dt">table de données</param>
		/// <param name="tab">tableau de résultats</param>
		/// <param name="row">index ligne tableau de résultats</param>
		/// <param name="col">index colonne tableau de résultats</param>
		/// <param name="operation">operation sql sur table de données</param>
		/// <param name="criteriaPeriodN">critère de sélection sur table de données période N</param>
		/// <param name="criteriaPeriodN1">critère de sélection sur table de données période N-1</param>
		/// <param name="coordCellTab">coordonnées de la cellule de l'élément parent</param>
		/// <param name="isTotalLine">vrai si ligne totale</param>
		/// <param name="incrementRow">change de ligne</param>		
		/// <returns>coordonnées des index de la dernière cellule traitée</returns>
		private static int[] InsertValue(WebSession webSession, DataTable dt,ref object[,] tab,ref int row,ref int col,string operation,string criteriaPeriodN,string criteriaPeriodN1,int[] coordCellTab,bool isTotalLine,bool incrementRow){
			int indexRow=0;
			int evolIndexRow=0;
			int yearN1=int.Parse(webSession.PeriodBeginningDate.Substring(0,4))-1;
			//int colTotal=0;			

			#region Calcul unité période N et/ou période N -1
			if(dt!=null){
				// unité période N
				try{
					tab[row,col]=double.Parse(dt.Compute(operation,criteriaPeriodN).ToString());				
				
				}catch(Exception){
					tab[row,col]=null;
				}
			}
			//changement de ligne
			if(incrementRow)								
				indexRow=row;
			
			// unité période N-1
			if(webSession.ComparativeStudy){
				if(dt!=null){
					try{					
						tab[row+1,col]=double.Parse(dt.Compute(operation,criteriaPeriodN1).ToString());	
					
					}catch(Exception){					
						tab[row+1,col]=null;
					}	
				}
			}	
			if(incrementRow){ 
				#region libellés périodes
				//libellés des périodes
				
							
				if(IsDetailPeriod(webSession)){
					//libellé période N	
					tab = SetTabLinesPeriodLabel(webSession,tab,row,CstResults.DashBoard.PERIOD_N_COLUMN_INDEX,int.Parse(webSession.DetailPeriodBeginningDate.Substring(0,4)),false);
					//libellé période N	- 1	
					if(webSession.ComparativeStudy){												
						tab = SetTabLinesPeriodLabel(webSession,tab,row+1,CstResults.DashBoard.PERIOD_N1_COLUMN_INDEX,int.Parse(webSession.DetailPeriodBeginningDate.Substring(0,4)),true);								
						//Change de ligne
						indexRow=indexRow+1;
					}
				}else{
					//libellé période N	
					tab = SetTabLinesPeriodLabel(webSession,tab,row,CstResults.DashBoard.PERIOD_N_COLUMN_INDEX,int.Parse(webSession.PeriodBeginningDate.Substring(0,4)),false);
					//libellé période N	- 1	
					if(webSession.ComparativeStudy)	{											
						tab = SetTabLinesPeriodLabel(webSession,tab,row+1,CstResults.DashBoard.PERIOD_N1_COLUMN_INDEX,int.Parse(webSession.PeriodBeginningDate.Substring(0,4)),true);																							
						//Change de ligne
						indexRow=indexRow+1;
					}
				}
				
				#endregion				
			}
			
			#endregion

			#region libellés PDM ou PDV
			
			//libelés PDM élément enfant (niveau 2 ou niveau 3)
			if(webSession.PDM || webSession.PDV){
				if(incrementRow){ 
					if(webSession.ComparativeStudy && coordCellTab!=null && coordCellTab.Length>0 && tab!=null){
						//PDM ou PDV période N				
						if(webSession.PDM ) //&& tab[row,colTotal]!=null && tab[row,colTotal]!=System.DBNull.Value)
							tab[row+2,CstResults.DashBoard.PDM_COLUMN_INDEX.GetHashCode()]= GestionWeb.GetWebWord(806,webSession.SiteLanguage)+" "+webSession.PeriodBeginningDate.Substring(0,4);
						else if(webSession.PDV ) //&& tab[row,colTotal]!=null && tab[row,colTotal]!=System.DBNull.Value) 
							tab[row+2,CstResults.DashBoard.PDV_COLUMN_INDEX]= GestionWeb.GetWebWord(1167,webSession.SiteLanguage)+" "+webSession.PeriodBeginningDate.Substring(0,4);
						//PDM ou PDV période N - 1				
						if(webSession.PDM ) //&& tab[row+1,colTotal]!=null && tab[row+1,colTotal]!=System.DBNull.Value)
							tab[row+3,CstResults.DashBoard.PDM_COLUMN_INDEX]= GestionWeb.GetWebWord(806,webSession.SiteLanguage)+" "+yearN1;
						else if(webSession.PDV ) //&& tab[row+1,colTotal]!=null && tab[row+1,colTotal]!=System.DBNull.Value)
							tab[row+3,CstResults.DashBoard.PDV_COLUMN_INDEX]= GestionWeb.GetWebWord(1167,webSession.SiteLanguage)+" "+yearN1;
						if(incrementRow) indexRow=indexRow+2;
					}else if(coordCellTab!=null && coordCellTab.Length>0 ){					
						
						if(webSession.PDM ) //&& tab[row,colTotal]!=null && tab[row,colTotal]!=System.DBNull.Value)
							tab[row+1,CstResults.DashBoard.PDM_COLUMN_INDEX]= GestionWeb.GetWebWord(806,webSession.SiteLanguage)+" "+webSession.PeriodBeginningDate.Substring(0,4);
						else if(webSession.PDV) // && tab[row,colTotal]!=null && tab[row,colTotal]!=System.DBNull.Value)
							tab[row+1,CstResults.DashBoard.PDV_COLUMN_INDEX]= GestionWeb.GetWebWord(1167,webSession.SiteLanguage)+" "+webSession.PeriodBeginningDate.Substring(0,4);
						if(incrementRow) indexRow=indexRow+1;
					}
				}
			}
			#endregion

			#region Evolution
			//Evolution
			if(webSession.Evolution){				
					//Evolution anneé N par rapport N-1	= ((N-(N-1))*100)/N-1
					try{
						if(webSession.PDM || webSession.PDV)evolIndexRow=row+4;
						else evolIndexRow = row+2;
						if(tab[row,col]!=null && tab[row,col]!=System.DBNull.Value && WebFunctions.CheckedText.IsStringEmpty(tab[row,col].ToString()) && (tab[row+1,col]==null || tab[row+1,col] ==System.DBNull.Value)){
							tab[evolIndexRow,col]=double.PositiveInfinity;
						}
						else if( tab[row+1,col] !=null && tab[row+1,col] !=System.DBNull.Value && WebFunctions.CheckedText.IsStringEmpty(tab[row+1,col].ToString()) && (tab[row,col]==null || tab[row,col]==System.DBNull.Value))
						tab[evolIndexRow,col]=(double)-100;
						else if(tab[row,col]!=null && tab[row,col]!=System.DBNull.Value && tab[row+1,col] !=null && tab[row+1,col] !=System.DBNull.Value && WebFunctions.CheckedText.IsStringEmpty(tab[row,col].ToString()) 
							&& WebFunctions.CheckedText.IsStringEmpty(tab[row+1,col].ToString()) && double.Parse(tab[row+1,col].ToString())>(double)0){					
							tab[evolIndexRow,col] = ((double.Parse(tab[row,col].ToString())-double.Parse(tab[row+1,col].ToString()))*(double)100)/double.Parse(tab[row+1,col].ToString());																						
						}
						tab[evolIndexRow,CstResults.DashBoard.EVOL_COLUMN_INDEX] = GestionWeb.GetWebWord(1168,webSession.SiteLanguage);
					}catch(Exception){					
						tab[evolIndexRow,col]=null;
					}					
					if(incrementRow) indexRow=indexRow+1;				
			}
			#endregion

			if(incrementRow) row=indexRow;
			return coordCellTab;
		}
		#endregion

		#region insertion d'une valeur dans tableau For Generic UI
//		/// <summary>
//		/// Insert des valeurs dans les cellules du tableau de résultats
//		/// </summary>
//		/// <param name="webSession">session du client</param>		
//		/// <param name="tab">tableau de résultats</param>
//		/// <param name="row">index ligne tableau de résultats</param>
//		/// <param name="col">index colonne tableau de résultats</param>
//		/// <param name="coordCellTab">coordonnées de la cellule de l'élément parent</param>
//		/// <param name="isTotalLine">vrai si ligne totale</param>
//		/// <param name="incrementRow">change de ligne</param>		
//		/// <returns>coordonnées des index de la fdernière cellule traitée</returns>
//		private static int[] InsertValueForGenericUI(WebSession webSession, ref ResultTable tab,ref int row,ref int col,int[] coordCellTab,bool isTotalLine,bool incrementRow)
//		{
//			return  InsertValue(webSession,null,ref tab,ref row,ref col,"","","",coordCellTab,isTotalLine,incrementRow);
//		}
//
//		/// <summary>
//		/// Insert des valeurs dans les cellules du tableau de résultats
//		/// </summary>
//		/// <param name="webSession">session du client</param>
//		/// <param name="dt">table de données</param>
//		/// <param name="tab">tableau de résultats</param>
//		/// <param name="row">index ligne tableau de résultats</param>
//		/// <param name="col">index colonne tableau de résultats</param>
//		/// <param name="operation">operation sql sur table de données</param>
//		/// <param name="criteriaPeriodN">critère de sélection sur table de données période N</param>
//		/// <param name="criteriaPeriodN1">critère de sélection sur table de données période N-1</param>
//		/// <param name="coordCellTab">coordonnées de la cellule de l'élément parent</param>
//		/// <param name="isTotalLine">vrai si ligne totale</param>
//		/// <param name="incrementRow">change de ligne</param>		
//		/// <returns>coordonnées des index de la dernière cellule traitée</returns>
//		private static int[] InsertValue(WebSession webSession, DataTable dt,ref ResultTable tab,ref int row,ref int col,string operation,string criteriaPeriodN,string criteriaPeriodN1,int[] coordCellTab,bool isTotalLine,bool incrementRow)
//		{
//			int indexRow=0;
//			int evolIndexRow=0;
//			int yearN1=int.Parse(webSession.PeriodBeginningDate.Substring(0,4))-1;
//			//int colTotal=0;			
//
//			#region Calcul unité période N et/ou période N -1
//			if(dt!=null)
//			{
//				// unité période N
//				try
//				{
//					//tab[row,col]=double.Parse(dt.Compute(operation,criteriaPeriodN).ToString());				
//					tab[row,col]=new CellLevel(row,dt.Compute(operation,criteriaPeriodN).ToString(),1);
//				
//				}
//				catch(Exception)
//				{
//					tab[row,col]=null;
//				}
//			}
//			//changement de ligne
//			if(incrementRow)								
//				indexRow=row;
//			
//			// unité période N-1
//			if(webSession.ComparativeStudy)
//			{
//				if(dt!=null)
//				{
//					try
//					{					
//						//tab[row+1,col]=double.Parse(dt.Compute(operation,criteriaPeriodN1).ToString());	
//						tab[row+1,col]=new CellLevel(row+1,dt.Compute(operation,criteriaPeriodN1).ToString(),1);
//					
//					}
//					catch(Exception)
//					{					
//						tab[row+1,col]=null;
//					}	
//				}
//			}	
//			if(incrementRow)
//			{ 
//				#region libellés périodes
//				//libellés des périodes
//				
//							
//				if(IsDetailPeriod(webSession))
//				{
//					//libellé période N	
//					tab = SetTabLinesPeriodLabelForGenericUI(webSession,tab,row,CstResults.DashBoard.PERIOD_N_COLUMN_INDEX,int.Parse(webSession.DetailPeriodBeginningDate.Substring(0,4)),false);
//					//libellé période N	- 1	
//					if(webSession.ComparativeStudy)
//					{												
//						tab = SetTabLinesPeriodLabelForGenericUI(webSession,tab,row+1,CstResults.DashBoard.PERIOD_N1_COLUMN_INDEX,int.Parse(webSession.DetailPeriodBeginningDate.Substring(0,4)),true);								
//						//Change de ligne
//						indexRow=indexRow+1;
//					}
//				}
//				else
//				{
//					//libellé période N	
//					tab = SetTabLinesPeriodLabelForGenericUI(webSession,tab,row,CstResults.DashBoard.PERIOD_N_COLUMN_INDEX,int.Parse(webSession.PeriodBeginningDate.Substring(0,4)),false);
//					//libellé période N	- 1	
//					if(webSession.ComparativeStudy)	
//					{											
//						tab = SetTabLinesPeriodLabelForGenericUI(webSession,tab,row+1,CstResults.DashBoard.PERIOD_N1_COLUMN_INDEX,int.Parse(webSession.PeriodBeginningDate.Substring(0,4)),true);																							
//						//Change de ligne
//						indexRow=indexRow+1;
//					}
//				}
//				
//				#endregion				
//			}
//			
//			#endregion
//
//			#region libellés PDM ou PDV
//			
//			//libelés PDM élément enfant (niveau 2 ou niveau 3)
////			if(webSession.PDM || webSession.PDV)
////			{
////				if(incrementRow)
////				{ 
////					if(webSession.ComparativeStudy && coordCellTab!=null && coordCellTab.Length>0 && tab!=null)
////					{
////						//PDM ou PDV période N				
////						if(webSession.PDM ) //&& tab[row,colTotal]!=null && tab[row,colTotal]!=System.DBNull.Value)
////							tab[row+2,CstResults.DashBoard.PDM_COLUMN_INDEX.GetHashCode()]= GestionWeb.GetWebWord(806,webSession.SiteLanguage)+" "+webSession.PeriodBeginningDate.Substring(0,4);
////						else if(webSession.PDV ) //&& tab[row,colTotal]!=null && tab[row,colTotal]!=System.DBNull.Value) 
////							tab[row+2,CstResults.DashBoard.PDV_COLUMN_INDEX]= GestionWeb.GetWebWord(1167,webSession.SiteLanguage)+" "+webSession.PeriodBeginningDate.Substring(0,4);
////						//PDM ou PDV période N - 1				
////						if(webSession.PDM ) //&& tab[row+1,colTotal]!=null && tab[row+1,colTotal]!=System.DBNull.Value)
////							tab[row+3,CstResults.DashBoard.PDM_COLUMN_INDEX]= GestionWeb.GetWebWord(806,webSession.SiteLanguage)+" "+yearN1;
////						else if(webSession.PDV ) //&& tab[row+1,colTotal]!=null && tab[row+1,colTotal]!=System.DBNull.Value)
////							tab[row+3,CstResults.DashBoard.PDV_COLUMN_INDEX]= GestionWeb.GetWebWord(1167,webSession.SiteLanguage)+" "+yearN1;
////						if(incrementRow) indexRow=indexRow+2;
////					}
////					else if(coordCellTab!=null && coordCellTab.Length>0 )
////					{					
////						
////						if(webSession.PDM ) //&& tab[row,colTotal]!=null && tab[row,colTotal]!=System.DBNull.Value)
////							tab[row+1,CstResults.DashBoard.PDM_COLUMN_INDEX]= GestionWeb.GetWebWord(806,webSession.SiteLanguage)+" "+webSession.PeriodBeginningDate.Substring(0,4);
////						else if(webSession.PDV) // && tab[row,colTotal]!=null && tab[row,colTotal]!=System.DBNull.Value)
////							tab[row+1,CstResults.DashBoard.PDV_COLUMN_INDEX]= GestionWeb.GetWebWord(1167,webSession.SiteLanguage)+" "+webSession.PeriodBeginningDate.Substring(0,4);
////						if(incrementRow) indexRow=indexRow+1;
////					}
////				}
////			}
//			#endregion
//
//			#region Evolution
//			//Evolution
////			if(webSession.Evolution)
////			{				
////				//Evolution anneé N par rapport N-1	= ((N-(N-1))*100)/N-1
////				try
////				{
////					if(webSession.PDM || webSession.PDV)evolIndexRow=row+4;
////					else evolIndexRow = row+2;
////					if(tab[row,col]!=null && tab[row,col]!=System.DBNull.Value && WebFunctions.CheckedText.IsStringEmpty(tab[row,col].ToString()) && (tab[row+1,col]==null || tab[row+1,col] ==System.DBNull.Value))
////					{
////						tab[evolIndexRow,col]=double.PositiveInfinity;
////					}
////					else if( tab[row+1,col] !=null && tab[row+1,col] !=System.DBNull.Value && WebFunctions.CheckedText.IsStringEmpty(tab[row+1,col].ToString()) && (tab[row,col]==null || tab[row,col]==System.DBNull.Value))
////						tab[evolIndexRow,col]=(double)-100;
////					else if(tab[row,col]!=null && tab[row,col]!=System.DBNull.Value && tab[row+1,col] !=null && tab[row+1,col] !=System.DBNull.Value && WebFunctions.CheckedText.IsStringEmpty(tab[row,col].ToString()) 
////						&& WebFunctions.CheckedText.IsStringEmpty(tab[row+1,col].ToString()) && double.Parse(tab[row+1,col].ToString())>(double)0)
////					{					
////						tab[evolIndexRow,col] = ((double.Parse(tab[row,col].ToString())-double.Parse(tab[row+1,col].ToString()))*(double)100)/double.Parse(tab[row+1,col].ToString());																						
////					}
////					tab[evolIndexRow,CstResults.DashBoard.EVOL_COLUMN_INDEX] = GestionWeb.GetWebWord(1168,webSession.SiteLanguage);
////				}
////				catch(Exception)
////				{					
////					tab[evolIndexRow,col]=null;
////				}					
////				if(incrementRow) indexRow=indexRow+1;				
////			}
//			#endregion
//
//			if(incrementRow) row=indexRow;
//			return coordCellTab;
//		}
		#endregion
		
		#region Insertion Libellés période dans ligne  du tableau de résultats
		/// <summary>
		///Insert libellés période dans ligne du tableau de résultats 
		/// </summary>
		/// <param name="webSession">session du client </param>
		/// <param name="tab">tableau de résultats</param>
		/// <param name="row">index ligne tableau de résultats</param>
		/// <param name="col">index colonne tableau de résultats</param>
		/// <param name="year">année</param>
		/// <param name="isPreviousYear">vrai si année précédente</param>
		/// <returns>tableau de résultats</returns>
		private static object[,] SetTabLinesPeriodLabel(WebSession webSession,object[,] tab,int row,int col,int year,bool isPreviousYear)
		{
			//libellé période N	
			AtomicPeriodWeek weekBeginningDate;
			AtomicPeriodWeek weekEndDate;
			switch(webSession.DetailPeriod ){					
				case CstPeriodDetail.monthly :	
					if(isPreviousYear)year--;
					if(IsDetailPeriod(webSession)){
						tab[row,col]=MonthString.Get(int.Parse(webSession.DetailPeriodBeginningDate.Substring(4,2)),webSession.SiteLanguage,0)+" "+year.ToString();
					}else{
						if(webSession.PeriodBeginningDate.Substring(4,2).Equals(webSession.PeriodEndDate.Substring(4,2))){						
							tab[row,col]=MonthString.Get(int.Parse(webSession.PeriodBeginningDate.Substring(4,2)),webSession.SiteLanguage,0)+" "+year.ToString();						
						}else{							
							tab[row,col]=MonthString.Get(int.Parse(webSession.PeriodBeginningDate.Substring(4,2)),webSession.SiteLanguage,0)+" "+year.ToString()+" - "
								+MonthString.Get(int.Parse(webSession.PeriodEndDate.Substring(4,2)),webSession.SiteLanguage,0)+" "+year.ToString();													
						}
					}					
					break;
				case CstPeriodDetail.weekly :
					if(IsDetailPeriod(webSession)){
						weekBeginningDate=new AtomicPeriodWeek(year,int.Parse(webSession.DetailPeriodBeginningDate.Substring(4,2)));	
						weekEndDate=new AtomicPeriodWeek(year,int.Parse(webSession.DetailPeriodBeginningDate.Substring(4,2)));							
						if(isPreviousYear){
							weekBeginningDate.SubWeek(52);
							weekEndDate.SubWeek(52);
						}
						tab[row,col]=(weekBeginningDate.FirstDay.Day.ToString().Length==1 ? "0"+weekBeginningDate.FirstDay.Day.ToString() : weekBeginningDate.FirstDay.Day.ToString())
							+"/"+(weekBeginningDate.FirstDay.Month.ToString().Length==1 ? "0"+weekBeginningDate.FirstDay.Month.ToString() : weekBeginningDate.FirstDay.Month.ToString())
							+"/"+ weekBeginningDate.FirstDay.Year.ToString()										
							+" - " + (weekEndDate.LastDay.Day.ToString().Length==1 ? "0"+weekEndDate.LastDay.Day.ToString() : weekEndDate.LastDay.Day.ToString() )
							+"/"+ (weekEndDate.LastDay.Month.ToString().Length==1 ? "0"+weekEndDate.LastDay.Month.ToString() : weekEndDate.LastDay.Month.ToString() )
							+"/"+weekEndDate.LastDay.Year.ToString();
					}else{
						weekBeginningDate=new AtomicPeriodWeek(year,int.Parse(webSession.PeriodBeginningDate.Substring(4,2)));	
						weekEndDate=new AtomicPeriodWeek(year,int.Parse(webSession.PeriodEndDate.Substring(4,2)));												
						if(isPreviousYear){
							weekBeginningDate.SubWeek(52);
							weekEndDate.SubWeek(52);
						}
						tab[row,col]=(weekBeginningDate.FirstDay.Day.ToString().Length==1 ? "0"+weekBeginningDate.FirstDay.Day.ToString() : weekBeginningDate.FirstDay.Day.ToString())
							+"/"+(weekBeginningDate.FirstDay.Month.ToString().Length==1 ? "0"+weekBeginningDate.FirstDay.Month.ToString() : weekBeginningDate.FirstDay.Month.ToString())
							+"/"+ weekBeginningDate.FirstDay.Year.ToString()										
							+" - " + (weekEndDate.LastDay.Day.ToString().Length==1 ? "0"+weekEndDate.LastDay.Day.ToString() : weekEndDate.LastDay.Day.ToString() )
							+"/"+ (weekEndDate.LastDay.Month.ToString().Length==1 ? "0"+weekEndDate.LastDay.Month.ToString() : weekEndDate.LastDay.Month.ToString() )
							+"/"+weekEndDate.LastDay.Year.ToString();
					}
					break;
			}	

			return tab;
		}

		/// <summary>
		/// Verifie qu'une periode doit être detaillé
		/// </summary>
		/// <param name="webSession">client</param>
		/// <returns>vraie si la periode est sélectionnée et faux sinon</returns>
		private static bool IsDetailPeriod(WebSession webSession){		 	
			return!(webSession.DetailPeriodBeginningDate.Equals("") || webSession.DetailPeriodBeginningDate.Equals("0"));				
		}
		#endregion

		#region Insertion Libellés période dans ligne  du tableau de résultats For Generic UI
//		/// <summary>
//		///Insert libellés période dans ligne du tableau de résultats 
//		/// </summary>
//		/// <param name="webSession">session du client </param>
//		/// <param name="tab">tableau de résultats</param>
//		/// <param name="row">index ligne tableau de résultats</param>
//		/// <param name="col">index colonne tableau de résultats</param>
//		/// <param name="year">année</param>
//		/// <param name="isPreviousYear">vrai si année précédente</param>
//		/// <returns>tableau de résultats</returns>
//		private static ResultTable SetTabLinesPeriodLabelForGenericUI(WebSession webSession,ResultTable tab,int row,int col,int year,bool isPreviousYear)
//		{
//			//libellé période N	
//			AtomicPeriodWeek weekBeginningDate;
//			AtomicPeriodWeek weekEndDate;
//			switch(webSession.DetailPeriod )
//			{					
//				case CstPeriodDetail.monthly :	
//					if(isPreviousYear)year--;
//					if(IsDetailPeriod(webSession))
//					{
//						//tab[row,col]=MonthString.Get(int.Parse(webSession.DetailPeriodBeginningDate.Substring(4,2)),webSession.SiteLanguage,0)+" "+year.ToString();
//						tab[row,col]=new CellLevel(row,MonthString.Get(int.Parse(webSession.DetailPeriodBeginningDate.Substring(4,2)),webSession.SiteLanguage,0)+" "+year.ToString(),1);
//					}
//					else
//					{
//						if(webSession.PeriodBeginningDate.Substring(4,2).Equals(webSession.PeriodEndDate.Substring(4,2)))
//						{						
//							//tab[row,col]=MonthString.Get(int.Parse(webSession.PeriodBeginningDate.Substring(4,2)),webSession.SiteLanguage,0)+" "+year.ToString();						
//							tab[row,col]=new CellLevel(row,MonthString.Get(int.Parse(webSession.PeriodBeginningDate.Substring(4,2)),webSession.SiteLanguage,0)+" "+year.ToString(),1);
//						}
//						else
//						{							
////							tab[row,col]=MonthString.Get(int.Parse(webSession.PeriodBeginningDate.Substring(4,2)),webSession.SiteLanguage,0)+" "+year.ToString()+" - "
////								+MonthString.Get(int.Parse(webSession.PeriodEndDate.Substring(4,2)),webSession.SiteLanguage,0)+" "+year.ToString();													
//							tab[row,col]=new CellLevel(row,MonthString.Get(int.Parse(webSession.PeriodBeginningDate.Substring(4,2)),webSession.SiteLanguage,0)+" "+year.ToString()+" - "+MonthString.Get(int.Parse(webSession.PeriodEndDate.Substring(4,2)),webSession.SiteLanguage,0)+" "+year.ToString(),1);
//						}
//					}					
//					break;
//				case CstPeriodDetail.weekly :
//					if(IsDetailPeriod(webSession))
//					{
//						weekBeginningDate=new AtomicPeriodWeek(year,int.Parse(webSession.DetailPeriodBeginningDate.Substring(4,2)));	
//						weekEndDate=new AtomicPeriodWeek(year,int.Parse(webSession.DetailPeriodBeginningDate.Substring(4,2)));							
//						if(isPreviousYear)
//						{
//							weekBeginningDate.SubWeek(52);
//							weekEndDate.SubWeek(52);
//						}
////						tab[row,col]=(weekBeginningDate.FirstDay.Day.ToString().Length==1 ? "0"+weekBeginningDate.FirstDay.Day.ToString() : weekBeginningDate.FirstDay.Day.ToString())
////							+"/"+(weekBeginningDate.FirstDay.Month.ToString().Length==1 ? "0"+weekBeginningDate.FirstDay.Month.ToString() : weekBeginningDate.FirstDay.Month.ToString())
////							+"/"+ weekBeginningDate.FirstDay.Year.ToString()										
////							+" - " + (weekEndDate.LastDay.Day.ToString().Length==1 ? "0"+weekEndDate.LastDay.Day.ToString() : weekEndDate.LastDay.Day.ToString() )
////							+"/"+ (weekEndDate.LastDay.Month.ToString().Length==1 ? "0"+weekEndDate.LastDay.Month.ToString() : weekEndDate.LastDay.Month.ToString() )
////							+"/"+weekEndDate.LastDay.Year.ToString();
//						tab[row,col]=new CellLevel(row,(weekBeginningDate.FirstDay.Day.ToString().Length==1 ? "0"+weekBeginningDate.FirstDay.Day.ToString() : weekBeginningDate.FirstDay.Day.ToString())+"/"+(weekBeginningDate.FirstDay.Month.ToString().Length==1 ? "0"+weekBeginningDate.FirstDay.Month.ToString() : weekBeginningDate.FirstDay.Month.ToString())+"/"+ weekBeginningDate.FirstDay.Year.ToString()+" - " + (weekEndDate.LastDay.Day.ToString().Length==1 ? "0"+weekEndDate.LastDay.Day.ToString() : weekEndDate.LastDay.Day.ToString() )+"/"+ (weekEndDate.LastDay.Month.ToString().Length==1 ? "0"+weekEndDate.LastDay.Month.ToString() : weekEndDate.LastDay.Month.ToString() )+"/"+weekEndDate.LastDay.Year.ToString(),1);
//					}
//					else
//					{
//						weekBeginningDate=new AtomicPeriodWeek(year,int.Parse(webSession.PeriodBeginningDate.Substring(4,2)));	
//						weekEndDate=new AtomicPeriodWeek(year,int.Parse(webSession.PeriodEndDate.Substring(4,2)));												
//						if(isPreviousYear)
//						{
//							weekBeginningDate.SubWeek(52);
//							weekEndDate.SubWeek(52);
//						}
////						tab[row,col]=(weekBeginningDate.FirstDay.Day.ToString().Length==1 ? "0"+weekBeginningDate.FirstDay.Day.ToString() : weekBeginningDate.FirstDay.Day.ToString())
////							+"/"+(weekBeginningDate.FirstDay.Month.ToString().Length==1 ? "0"+weekBeginningDate.FirstDay.Month.ToString() : weekBeginningDate.FirstDay.Month.ToString())
////							+"/"+ weekBeginningDate.FirstDay.Year.ToString()										
////							+" - " + (weekEndDate.LastDay.Day.ToString().Length==1 ? "0"+weekEndDate.LastDay.Day.ToString() : weekEndDate.LastDay.Day.ToString() )
////							+"/"+ (weekEndDate.LastDay.Month.ToString().Length==1 ? "0"+weekEndDate.LastDay.Month.ToString() : weekEndDate.LastDay.Month.ToString() )
////							+"/"+weekEndDate.LastDay.Year.ToString();
//						tab[row,col]=new CellLevel(row,(weekBeginningDate.FirstDay.Day.ToString().Length==1 ? "0"+weekBeginningDate.FirstDay.Day.ToString() : weekBeginningDate.FirstDay.Day.ToString())+"/"+(weekBeginningDate.FirstDay.Month.ToString().Length==1 ? "0"+weekBeginningDate.FirstDay.Month.ToString() : weekBeginningDate.FirstDay.Month.ToString())+"/"+ weekBeginningDate.FirstDay.Year.ToString()+" - " + (weekEndDate.LastDay.Day.ToString().Length==1 ? "0"+weekEndDate.LastDay.Day.ToString() : weekEndDate.LastDay.Day.ToString() )+"/"+ (weekEndDate.LastDay.Month.ToString().Length==1 ? "0"+weekEndDate.LastDay.Month.ToString() : weekEndDate.LastDay.Month.ToString() )+"/"+weekEndDate.LastDay.Year.ToString(),1);
//					}
//					break;
//			}	
//
//			return tab;
//		}

		#endregion

		#region Calcul PDM ou PDV
		/// <summary>
		/// Calcul du PDM  ou PDV pour le tableau de résultats
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="tab">tableau de résultats</param>
		/// <param name="row">index ligne tableau de résultats</param>
		/// <param name="col">index colonne tableau de résultats</param>
		/// <returns>tableau de résultats</returns>
		private static object[,] ComputeTabPDMOrPDV(WebSession webSession,ref object[,] tab,int row,int col)
		{
			
			Int64 oldIdL1=-2;
			Int64 oldIdL1N1=-2;
			double valueL1=0;
			double valueL1N1=0;
			double valueL2=0;
			double valueL2N1=0;		
			int oldL1Row=0;
			int oldL1RowN1=0;
			int startCol=col;

			if(tab!=null){
				//Pour chaque colonne  du tableau 
				for(int i=startCol;i<tab.GetLength(1);i++){
					 oldIdL1=-2;
					 oldIdL1N1=-2;
					 valueL1N1=0;
					valueL2=0;
					 valueL2N1=0;					
					 oldL1Row=0;
					 oldL1RowN1=0;
					//Pour chaque ligne du tableau
					for(int j=row;j<tab.GetLength(0);j++){
						//Niveau 1
						if(tab[j,CstResults.DashBoard.ID_ELMT_L1_COLUMN_INDEX]!=null){
							try{
								//PDM ou PDV période N
								if(Int64.Parse(tab[j,CstResults.DashBoard.ID_ELMT_L1_COLUMN_INDEX].ToString())!=oldIdL1 && tab[j,CstResults.DashBoard.PERIOD_N_COLUMN_INDEX]!=null){
									oldIdL1 = Int64.Parse(tab[j,CstResults.DashBoard.ID_ELMT_L1_COLUMN_INDEX].ToString());
									valueL1 = (double)0;
									oldL1Row = j;
									if(tab[j,CstResults.DashBoard.PERIOD_N_COLUMN_INDEX]!=null && tab[j,i]!=null)
										valueL1 = double.Parse(tab[j,i].ToString());																		
									if(tab[j,CstResults.DashBoard.PERIOD_N_COLUMN_INDEX]!=null && tab[j,i]!=null)	{				
										if(webSession.ComparativeStudy)tab[j+2,i] = (double)100;
										else tab[j+1,i] = (double)100;
									}									
								}
								//PDM ou PDV période N-1
								if(Int64.Parse(tab[j,CstResults.DashBoard.ID_ELMT_L1_COLUMN_INDEX].ToString())!=oldIdL1N1 && tab[j,CstResults.DashBoard.PERIOD_N1_COLUMN_INDEX]!=null && webSession.ComparativeStudy ){
									oldIdL1N1 = Int64.Parse(tab[j,CstResults.DashBoard.ID_ELMT_L1_COLUMN_INDEX].ToString());
									valueL1N1 = (double)0;
									oldL1RowN1 = j;
									if(tab[j,CstResults.DashBoard.PERIOD_N1_COLUMN_INDEX]!=null && tab[j,i]!=null)
										valueL1N1 = double.Parse(tab[j,i].ToString());																																		
										//PDM ou PDV période N-1
										if(tab[j,CstResults.DashBoard.PERIOD_N1_COLUMN_INDEX]!=null && tab[j,i]!=null && valueL1N1>0)					
											tab[j+2,i] = (double)100;
									
								}
								
							}catch(Exception ex){
								throw new DashBoardRulesException("ComputeTabPDMOrPDV(WebSession webSession,ref object[,] tab,int row,int col) :  Impossible de calculer le PDM ou PDV "+ex.Message);
							}
						}
						//PDM ou PDV Niveau 2
						if(tab[j,CstResults.DashBoard.ID_ELMT_L2_COLUMN_INDEX]!=null){
							try{																	
								//PDM ou PDV période N
								if(tab[j,CstResults.DashBoard.PERIOD_N_COLUMN_INDEX]!=null && tab[j,i]!=null && valueL1>0 )	{				
									if(webSession.ComparativeStudy)tab[j+2,i] = (double.Parse(tab[j,i].ToString())*(double)100)/valueL1;
									else tab[j+1,i] = (double.Parse(tab[j,i].ToString())*(double)100)/valueL1;										
									valueL2 = double.Parse(tab[j,i].ToString());
								}									
								//PDM ou PDV période N-1									
								if(webSession.ComparativeStudy && tab[j,CstResults.DashBoard.PERIOD_N1_COLUMN_INDEX]!=null && tab[j,i]!=null && valueL1N1>0){																	
									tab[j+2,i] = (double.Parse(tab[j,i].ToString())*(double)100)/valueL1N1;									
									valueL2N1 = double.Parse(tab[j,i].ToString());
								}
							}catch(Exception ex1){
								throw new DashBoardRulesException("ComputeTabPDMOrPDV(WebSession webSession,ref object[,] tab,int row,int col) :  Impossible de calculer le PDM ou PDV "+ex1.Message);
							}
						}
						//PDM ou PDV niveau 3
						if(tab[j,CstResults.DashBoard.ID_ELMT_L3_COLUMN_INDEX]!=null){														
							try{
								//PDM ou PDV période N
								if(tab[j,CstResults.DashBoard.PERIOD_N_COLUMN_INDEX]!=null && tab[j,i]!=null && valueL2>0 )	{				
									if(webSession.ComparativeStudy)tab[j+2,i] = (double.Parse(tab[j,i].ToString())*(double)100)/valueL2;
									else tab[j+1,i] = (double.Parse(tab[j,i].ToString())*(double)100)/valueL2;
								}
								//PDM ou PDV période N-1
								if(webSession.ComparativeStudy && tab[j,CstResults.DashBoard.PERIOD_N1_COLUMN_INDEX]!=null && tab[j,i]!=null && valueL2N1>0){									
										tab[j+2,i] = (double.Parse(tab[j,i].ToString())*(double)100)/valueL2N1;
								}	
							}catch(Exception ex2){
								throw new DashBoardRulesException("ComputeTabPDMOrPDV(WebSession webSession,ref object[,] tab,int row,int col) :  Impossible de calculer le PDM ou PDV "+ex2.Message);
							}								
						}																	
					}					
				}						
			}	
			return tab;
		}	

		#endregion

		#region Pourcentage de répartition
		/// <summary>
		/// Calcul le pourcentage de répartition par rapport au total
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="tab">tableau de résultats</param>
		/// <param name="row">index ligne tableau de résultats</param>
		/// <param name="col">index colonne</param>
		private static object[,] ComputePercentage(WebSession webSession,ref object[,] tab,int row,int col){
			int startCol=col;
			
			//Pour chaque ligne du tableau
			for(int j=row;j<tab.GetLength(0);j++){
				//Pour chaque colonne répartition du tableau on calcule le %
				for(int i=startCol+1;i<tab.GetLength(1);i++){
					if(tab[j,CstResults.DashBoard.ID_ELMT_L1_COLUMN_INDEX]!=null || tab[j,CstResults.DashBoard.ID_ELMT_L2_COLUMN_INDEX]!=null
						|| tab[j,CstResults.DashBoard.ID_ELMT_L3_COLUMN_INDEX]!=null){
						if(tab[j,i]!=null && tab[j,startCol]!=null && double.Parse(tab[j,startCol].ToString())>(double)0){
							tab[j,i] = (double.Parse(tab[j,i].ToString())*(double)100)/double.Parse(tab[j,startCol].ToString());
							tab[j,CstResults.DashBoard.REPARTITION_PERCENT]=true;
						}
					}
				}
				if(tab[j,CstResults.DashBoard.ID_ELMT_L1_COLUMN_INDEX]!=null || tab[j,CstResults.DashBoard.ID_ELMT_L2_COLUMN_INDEX]!=null
					|| tab[j,CstResults.DashBoard.ID_ELMT_L3_COLUMN_INDEX]!=null)
				if(tab[j,startCol]!=null)tab[j,startCol]=(double)100;
			}	
		
			return tab;
		}
		#endregion

		#region  Champs niveau de nomenclature 
		/// <summary>
		/// Retourne Identifiant De l'élément de nomenclature média ou produit sélectionné de niveau 2
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <returns>élément sélectionné</returns>
		private static string	GetL2Id(WebSession webSession){
			string elementLabel="";
			switch(webSession.PreformatedTable){
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units :				
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format :	
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay:
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice :	
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector :
					if(webSession.PreformatedMediaDetail==CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia						
						|| webSession.PreformatedMediaDetail==CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter){
						elementLabel="id_interest_center";
					}else if(webSession.PreformatedMediaDetail==CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia){						
						elementLabel="id_media";
					}	
					break;
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice :
						
					elementLabel="id_sector";
					break;
			}
			return  elementLabel;
		}
		/// <summary>
		/// Retourne Identifiant l'élément de nomenclature média ou produit sélectionné de niveau 2
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <returns>élément sélectionné</returns>
		private static string	GetL2Label(WebSession webSession){
			string elementLabel="";
			switch(webSession.PreformatedTable){
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units :				
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format :	
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay:
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice :	
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector :
					if(webSession.PreformatedMediaDetail==CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia						
					|| webSession.PreformatedMediaDetail==CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter){
						elementLabel="interest_center";
					}else if(webSession.PreformatedMediaDetail==CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia){						
						elementLabel="media";
					}	
					break;
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice :						
					elementLabel="sector";
					break;
			}
			return  elementLabel;
		}
		/// <summary>
		/// Retourne idebtifiant de l'élément de nomenclature média ou produit sélectionné de niveau 3
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <returns>élément sélectionné</returns>
		private static string	GetL3Id(WebSession webSession){
			string elementLabel="";
			switch(webSession.PreformatedTable){
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units :				
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format :	
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay:
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector :
					if(webSession.PreformatedMediaDetail==CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia){				
						elementLabel="id_media";
					}	
					break;				
			}
			return  elementLabel;
		} 
		/// <summary>
		/// Retourne l'élément de nomenclature média ou produit sélectionné de niveau 3
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <returns>élément sélectionné</returns>
		private static string	GetL3Label(WebSession webSession){
			string elementLabel="";
			switch(webSession.PreformatedTable){
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units :				
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format :	
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay:
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector :
					if(webSession.PreformatedMediaDetail==CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia){				
						elementLabel="media";
					}	
					break;				
			}
			return  elementLabel;
		}
 		
		/// <summary>
		/// Retourne l'élément de nomenclature média ou produit sélectionné de niveau 3
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <returns>élément sélectionné</returns>
		private static bool	IsLevel3(WebSession webSession){
			bool islevel=false;
			switch(webSession.PreformatedTable){
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units :				
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format :	
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay:
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector :
					if(webSession.PreformatedMediaDetail==CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia){				
						islevel=true;
						return islevel;
					}	
					break;				
			}
			return  islevel;
		}
 		
		#endregion
		
		#region Vérifie si tableau affiche lignes des totaux
		/// <summary>
		/// Vérifie si le tableau doit afficher des lignes avec le total des éléments
		/// de la nomenclatture média ou produit sélectionnée
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <returns>vrai si doit afficher les lignes des totaux</returns>
		private static bool HasTotalLines(WebSession webSession){
			switch(webSession.PreformatedTable){																								
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format :															
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay :							
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice :
					return false;																										
				default : 
					return true;
			}
		}
		#endregion

		#endregion
	
		#region Obtention d'un tableau dimensionné
		/// <summary>
		/// Obtient un tableau dimensionné
		/// </summary>
		/// <param name="dsData">groupe de données</param>
		/// <param name="webSession">session du client</param>
		/// <param name="vehicleType">type de média</param>
		/// <returns>tableau dimensionné</returns>
		private static object[,] GetSizedTable(DataSet dsData, WebSession webSession,ClassificationCst.DB.Vehicles.names vehicleType){
			object[,] tab=null;
			int columnsCount=0;
			int rowsCount=0;

			if(dsData!=null && dsData.Tables[0].Rows.Count>0){
				rowsCount = RowsCount(webSession,dsData,vehicleType);
				switch(webSession.PreformatedTable){
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units :
						columnsCount=UnitsCount(webSession,vehicleType)+CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS;	
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual :
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual :						
						columnsCount = CountMonthOrWeek(webSession)+CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS;
						break;																			
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format :
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format :	
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format :
						columnsCount = CountRepartition(CstWeb.Repartition.repartitionCode.Format,vehicleType)+CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS;
						break;
				
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay :
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay :
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay :
						columnsCount = CountRepartition(CstWeb.Repartition.repartitionCode.namedDay,vehicleType)+CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS;
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice :
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice :																									
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:
						columnsCount = CountRepartition(CstWeb.Repartition.repartitionCode.timeInterval,vehicleType)+CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS;						
						break;		
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector :	
						columnsCount = CountSector(webSession,dsData)+CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS;						
						break;
					default : 
						throw new DashBoardRulesException("getTablesName(WebSession webSession) --> Impossible d'identifier le tableau de bord à traiter.");
				}
				//dimensionnement du tableau
				if(rowsCount>0 && columnsCount>0)
					tab = new object[rowsCount,columnsCount];
			}

			return tab;
			
		}
		/// <summary>
		/// Donne le nombre de colonnes unités du tableau
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="vehicleType">type de média</param>
		/// <returns>nombre de colonnes média du tableau</returns>
		private static int UnitsCount(WebSession webSession,ClassificationCst.DB.Vehicles.names vehicleType){
			switch(vehicleType){					
				case ClassificationCst.DB.Vehicles.names.radio:
				case ClassificationCst.DB.Vehicles.names.tv:
				case ClassificationCst.DB.Vehicles.names.others:
					return 3;
				case ClassificationCst.DB.Vehicles.names.press:
					return 4;
				default:
					throw(new DashBoardDataAccessException("CountUnits(WebSession webSession,ClassificationCst.DB.Vehicles.names vehicleType) : Impossible de déterminer le type de média à traiter."));
			}
		}
		/// <summary>
		/// Donne le nombre de lignes  du tableau
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dsData">groupe de données</param>
		///  <param name="vehicleType">type de média</param>
		/// <returns>nombre de lignes  du tableau</returns>
		private static int RowsCount(WebSession webSession,DataSet dsData,ClassificationCst.DB.Vehicles.names vehicleType){
			
			#region variables					
			int countInterestCenter=0;						
			int countMedia=0;					
			int countSector=0;			
			#endregion

			int rowsCount=0;
			if(dsData!=null && dsData.Tables[0].Rows.Count>0){
				switch(webSession.PreformatedTable){
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units :																	
						//Nombre de média
						CountMedia(webSession,dsData,ref countInterestCenter,ref countMedia);						
						#region Nombre de lignes dans le tableau 
						//Nombre de lignes dans le tableau 
						if(countInterestCenter>0 || countMedia>0){
							//Rajoute les lignes du total univers
							rowsCount+=1; //ligne libellés des colonnes
							rowsCount+=RowsCount(webSession,webSession.Evolution,webSession.PDM,false,1);							
							//Rajoute les lignes centres d'intérêts
							if(countInterestCenter>0)
								rowsCount+=RowsCount(webSession,webSession.Evolution,webSession.PDM,false,countInterestCenter);																	
							//Rajoute les lignes média
							if(countMedia>0)
								rowsCount+=RowsCount(webSession,webSession.Evolution,webSession.PDM,false,countMedia);																																
						}	
						#endregion				
						break;	
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual :												
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format :
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay :
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice :
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector :
						//Nombre de média
						CountMedia(webSession,dsData,ref countInterestCenter,ref countMedia);						
						#region Nombre de lignes dans le tableau 
						//Nombre de lignes dans le tableau 
						if(countInterestCenter>0 || countMedia>0){
							//Rajoute les lignes du total univers
							rowsCount+=1; //ligne libellés des colonnes
							rowsCount+=RowsCount(webSession,webSession.Evolution,webSession.PDM,false,1);							
							//Rajoute les lignes centres d'intérêts
							if(countInterestCenter>0)
								rowsCount+=RowsCount(webSession,webSession.Evolution,webSession.PDM,false,countInterestCenter);																	
							//Rajoute les lignes média
							if(countMedia>0)
								rowsCount+=RowsCount(webSession,webSession.Evolution,webSession.PDM,false,countMedia);																																
						}	
						#endregion				
						break;						
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual :
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format :
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay :
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:	
						//Rajoute les lignes du total univers
						rowsCount+=1; //ligne libellés des colonnes
						rowsCount+=RowsCount(webSession,webSession.Evolution,false,webSession.PDV,1);
						//Nombre de familles
//						countSector+=CountSector(webSession,dsData,ref countSector);
						countSector=CountSector(webSession,dsData);
						//Rajoute les lignes familles
						if(countSector>0)
							rowsCount+=RowsCount(webSession,webSession.Evolution,false,webSession.PDV,countSector);																																							
						break;																
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format :					
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay :													
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice :
						rowsCount+=1; //ligne libellés des colonnes
						if(webSession.ComparativeStudy)
							rowsCount+=(webSession.Evolution)?UnitsCount(webSession,vehicleType)*3:UnitsCount(webSession,vehicleType)*2;														
						else
							rowsCount+=UnitsCount(webSession,vehicleType);																								
						break;																
					default : 
						throw new DashBoardRulesException("RowsCount(WebSession webSession): Impossible d'identifier le tableau de bord à traiter.");
				}
			}
			return rowsCount;
		}

		/// <summary>
		/// Nombre de lignes  du tableau en fonction des options d'analyse
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="Evolution">vrai si evolution </param>
		/// <param name="PDM">vrai si PDM</param>
		/// <param name="PDV">vrai si PDV</param>	
		/// <param name="countElement">nombre d'élément média </param>
		/// <returns> Nombre de lignes  du tableau en fonction des options d'analyse</returns>
		private static int RowsCount(WebSession webSession,bool Evolution,bool PDM,bool PDV,int countElement){
			int	rowsCount=0;
			rowsCount+=(webSession.ComparativeStudy)?countElement*2 :countElement*1;//lignes année N et N-1							
			if(PDM)rowsCount+=(webSession.ComparativeStudy)?countElement*2 : countElement*1;//lignes PDM					
			if(PDV)rowsCount+=(webSession.ComparativeStudy)?countElement*2 : countElement*1;//lignes PDV				
			if(rowsCount>0 && Evolution)rowsCount+=countElement;//lignes evolution

			return rowsCount;
		}

		/// <summary>
		/// Compte le nombre de média
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dsData">groupe de données</param>
		/// <param name="countInterestCenter">nombre de centre d'intérêts</param>
		/// <param name="countMedia">nombre de support</param>					
		private static void CountMedia(WebSession webSession, DataSet dsData ,ref int countInterestCenter,ref int countMedia){			
			Int64 oldIdInterestCenter=0;
			Int64 oldIdMedia=0;
			foreach(DataRow dr in dsData.Tables[0].Rows){
				
				switch(webSession.PreformatedMediaDetail){
						// selection du niveau media/support
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
						if(dsData.Tables[0].Columns.Contains("id_media")){							
							//Nombre de média
							if(Int64.Parse(dr["id_media"].ToString())!=oldIdMedia)
								countMedia++;
							oldIdMedia = Int64.Parse(dr["id_media"].ToString());
						}
						break;
						// selection du niveau media/centre d'interet
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
						if(dsData.Tables[0].Columns.Contains("id_interest_center")){							
							//Nombre de centres d'intérêts
							if(Int64.Parse(dr["id_interest_center"].ToString())!=oldIdInterestCenter)
								countInterestCenter++;
							oldIdInterestCenter = Int64.Parse(dr["id_interest_center"].ToString());
						}
						break;
						// selection du niveau media/centre d'interet/support
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
						if(dsData.Tables[0].Columns.Contains("id_interest_center")){							
							//Nombre de centres d'intérêts
							if(Int64.Parse(dr["id_interest_center"].ToString())!=oldIdInterestCenter)
								countInterestCenter++;
							oldIdInterestCenter = Int64.Parse(dr["id_interest_center"].ToString());
						}
						if(dsData.Tables[0].Columns.Contains("id_media")){							
							//Nombre de média
							if(Int64.Parse(dr["id_media"].ToString())!=oldIdMedia)
								countMedia++;
							oldIdMedia = Int64.Parse(dr["id_media"].ToString());
						}
						break;
					
				}
			}			
		}

		/// <summary>
		/// Compte le nombre de familles
		/// </summary>
		/// <param name="dsData">groupe de données</param>
		/// <param name="webSession">session du client</param>	
		/// <returns>nombre de familles</returns>
//		private static int CountSector(WebSession webSession,DataSet dsData ,ref int countSector){
		private static int CountSector(WebSession webSession,DataSet dsData){
			int countSector=0;
			ArrayList oldIdSectorArr = new ArrayList();
			
			switch(webSession.PreformatedTable){
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector :
					foreach(DataRow dr in dsData.Tables[0].Rows){							
						if(dsData.Tables[0].Columns.Contains("id_sector")){																			
							if(!oldIdSectorArr.Contains(Int64.Parse(dr["id_sector"].ToString()))){
								//Nombre de familles de produits							
								countSector++;
								oldIdSectorArr.Add(Int64.Parse(dr["id_sector"].ToString()));
							}
						}
					}
					break;
			}
			return countSector;
		}

		/// <summary>
		/// Compte le nombre de mois ou de semaine à afficher
		/// </summary>
		/// <param name="webSession">session du cleint</param>
		/// <returns>nombre de mois ou de semaine à afficher</returns>
		private static int CountMonthOrWeek(WebSession webSession){				
			switch(webSession.DetailPeriod ){					
				case CstPeriodDetail.monthly :	
				case CstPeriodDetail.weekly :
					if(!webSession.DetailPeriodBeginningDate.Equals("0") && WebFunctions.CheckedText.IsStringEmpty(webSession.DetailPeriodBeginningDate)) {
						return (int.Parse(webSession.DetailPeriodEndDate.Substring(4,2))-int.Parse(webSession.DetailPeriodBeginningDate.Substring(4,2)))+1;													
					}else return (int.Parse(webSession.PeriodEndDate.Substring(4,2))-int.Parse(webSession.PeriodBeginningDate.Substring(4,2)))+1;													
				default:
					throw(new DashBoardRulesException(" CountMonthOrWeek(WebSession webSession) : Impossible de déterminer le type de période."));
			}
		}
		/// <summary>
		/// Compte le nombre de Colonne pour un type de répatition
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="vehicleType">type de média</param>
		/// <returns>nombre de Colonne pour un type de répatition</returns>
		private static int CountRepartition(WebSession webSession,ClassificationCst.DB.Vehicles.names vehicleType){
			int columnsCount=0;
			switch(webSession.PreformatedTable){																					
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format :	
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format :
					columnsCount = CountRepartition(CstWeb.Repartition.repartitionCode.Format,vehicleType);
					break;
				
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay :
					columnsCount = CountRepartition(CstWeb.Repartition.repartitionCode.namedDay,vehicleType);
					break;
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice :																									
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:
					columnsCount = CountRepartition(CstWeb.Repartition.repartitionCode.timeInterval,vehicleType);						
					break;																
				default : 
					throw new DashBoardRulesException(" CountRepartition(WebSession webSession,ClassificationCst.DB.Vehicles.names vehicleType) --> Impossible d'identifier le tableau de bord à traiter.");
			}
			return columnsCount;
		}

		/// <summary>
		/// Compte le nombre de Colonne pour un type de répatition
		/// </summary>
		/// <param name="repartitionCode">type de répartition</param>
		/// <param name="vehicleType">type de média</param>
		/// <returns>nombre de répatition</returns>
		private static int CountRepartition(CstWeb.Repartition.repartitionCode repartitionCode,ClassificationCst.DB.Vehicles.names vehicleType){
			switch(repartitionCode){
				case CstWeb.Repartition.repartitionCode.namedDay :
					return CstResults.DashBoard.NB_TOTAL_NAMED_DAY_COLUMNS;
				case CstWeb.Repartition.repartitionCode.Format :
					return CstResults.DashBoard.NB_TOTAL_FORMAT_COLUMNS;
				case CstWeb.Repartition.repartitionCode.timeInterval :
					if(ClassificationCst.DB.Vehicles.names.radio==vehicleType)
						return CstResults.DashBoard.NB_TOTAL_RADIO_TIMESLICE_COLUMNS;
					else if(ClassificationCst.DB.Vehicles.names.tv==vehicleType ||  ClassificationCst.DB.Vehicles.names.others==vehicleType)
						return CstResults.DashBoard.NB_TOTAL_TV_TIMESLICE_COLUMNS;
					else throw(new DashBoardRulesException("CountRepartition(CstWeb.Repartition.repartitionCode repartitionCode,ClassificationCst.DB.Vehicles.names vehicleType) : Impossible de déterminer le type de média."));
				default:
					throw(new DashBoardRulesException(" CountRepartition(CstWeb.Repartition.repartitionCode repartitionCode,ClassificationCst.DB.Vehicles.names vehicleType) : Impossible de déterminer le type de répartition."));
			}
		}

		#endregion 

		#region Obtention d'un tableau dimensionné For Generic UI
//		/// <summary>
//		/// Obtient un tableau dimensionné
//		/// </summary>
//		/// <param name="dsData">groupe de données</param>
//		/// <param name="webSession">session du client</param>
//		/// <param name="vehicleType">type de média</param>
//		/// <returns>tableau dimensionné</returns>
//		private static ResultTable GetSizedTableForGenericUI(DataSet dsData, WebSession webSession,ClassificationCst.DB.Vehicles.names vehicleType)
//		{
//			ResultTable tab=null;
//			int columnsCount=0;
//			int rowsCount=0;
//
//			if(dsData!=null && dsData.Tables[0].Rows.Count>0)
//			{
//				rowsCount = RowsCount(webSession,dsData,vehicleType);
//				switch(webSession.PreformatedTable)
//				{
//					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units :
//						columnsCount=UnitsCount(webSession,vehicleType)+CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS;	
//						break;
//					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual :
//					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual :						
//						columnsCount = CountMonthOrWeek(webSession)+CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS;
//						break;																			
//					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format :
//					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format :	
//					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format :
//						columnsCount = CountRepartition(CstWeb.Repartition.repartitionCode.Format,vehicleType)+CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS;
//						break;
//				
//					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay :
//					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay :
//					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay :
//						columnsCount = CountRepartition(CstWeb.Repartition.repartitionCode.namedDay,vehicleType)+CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS;
//						break;
//					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice :
//					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice :																									
//					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:
//						columnsCount = CountRepartition(CstWeb.Repartition.repartitionCode.timeInterval,vehicleType)+CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS;						
//						break;		
//					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector :	
//						columnsCount = CountSector(webSession,dsData)+CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS;						
//						break;
//					default : 
//						throw new DashBoardRulesException("getTablesName(WebSession webSession) --> Impossible d'identifier le tableau de bord à traiter.");
//				}
//				//dimensionnement du tableau
//				if(rowsCount>0 && columnsCount>0)
//					tab = new ResultTable(rowsCount,columnsCount);
//			}
//
//			return tab;
//		}
//		
		#endregion 

		#region Libellés du tableau
		/// <summary>
		/// Insert les libellés du tableau
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="tab">tableau </param>
		/// <param name="vehicleType">type de média</param>
		/// <param name="dt">Table de données résultats</param>
		/// <param name="sectorHashTable">Liste identifiant des familles à afficher en colonne pour tableau Media\Famille</param>
		/// <returns>tableau de résultats</returns>
		private static object[,] SetTabLabel(WebSession webSession,object[,] tab,ClassificationCst.DB.Vehicles.names vehicleType,DataTable dt,Hashtable sectorHashTable)
		{
			#region variables
			AtomicPeriodWeek week;
			//DateTime dateBegin;
			//DateTime dateEnd;
			string dateString;
			int j=0;
			
			#endregion

			switch(webSession.PreformatedTable){
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units :
					#region Unités
					if(ClassificationCst.DB.Vehicles.names.press==vehicleType){
						//libellés unités : Euros,Mm/Col,pages,Isertions
						tab[0,CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS] = GestionWeb.GetWebWord(1423,webSession.SiteLanguage);
						tab[0,CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS+1] = GestionWeb.GetWebWord(1424,webSession.SiteLanguage);
						tab[0,CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS+2] = GestionWeb.GetWebWord(943,webSession.SiteLanguage);
						tab[0,CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS+3] = GestionWeb.GetWebWord(940,webSession.SiteLanguage);
					}else{
						//libellés unités : Euros,Durée,Spots
						tab[0,CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS] = GestionWeb.GetWebWord(1423,webSession.SiteLanguage);
						tab[0,CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS+1] = GestionWeb.GetWebWord(1435,webSession.SiteLanguage);						
						tab[0,CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS+2] = GestionWeb.GetWebWord(939,webSession.SiteLanguage);
					}
					#endregion
					break;
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual :										
					#region période
					if(webSession.DetailPeriod==CstPeriodDetail.monthly){
						//libellés mois
						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX] =  GestionWeb.GetWebWord(1401,webSession.SiteLanguage);
						j++;
						if(!webSession.DetailPeriodBeginningDate.Equals("0") && WebFunctions.CheckedText.IsStringEmpty(webSession.DetailPeriodBeginningDate)){
							for(int i=int.Parse(webSession.DetailPeriodBeginningDate.Substring(4,2));i<=int.Parse(webSession.DetailPeriodEndDate.Substring(4,2));i++){
								tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+j] = MonthString.Get(i,webSession.SiteLanguage,0);
								j++;
							}
						}else{							
							for(int i=int.Parse(webSession.PeriodBeginningDate.Substring(4,2));i<=int.Parse(webSession.PeriodEndDate.Substring(4,2));i++){
								tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+j] = MonthString.Get(i,webSession.SiteLanguage,0);
								j++;
							}
						}
					}else if(webSession.DetailPeriod==CstPeriodDetail.weekly){
						//libellés semaines
						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX] =  GestionWeb.GetWebWord(1401,webSession.SiteLanguage);
						j++;
						if(!webSession.DetailPeriodBeginningDate.Equals("0") && WebFunctions.CheckedText.IsStringEmpty(webSession.DetailPeriodBeginningDate)) {
							for(int i=int.Parse(webSession.DetailPeriodBeginningDate.Substring(4,2));i<=int.Parse(webSession.DetailPeriodEndDate.Substring(4,2));i++){							
								week=new AtomicPeriodWeek(int.Parse(webSession.DetailPeriodBeginningDate.Substring(0,4)),i);														
								dateString=week.FirstDay.Day.ToString()+"/"+week.FirstDay.Month.ToString()+"/"+week.FirstDay.Year.ToString()+" - "+week.LastDay.Day.ToString()+"/"+week.LastDay.Month.ToString()+"/"+week.LastDay.Year.ToString();
								tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+j]=dateString;
								j++;
							}
						}else{
							for(int i=int.Parse(webSession.PeriodBeginningDate.Substring(4,2));i<=int.Parse(webSession.PeriodEndDate.Substring(4,2));i++){							
								week=new AtomicPeriodWeek(int.Parse(webSession.PeriodBeginningDate.Substring(0,4)),i);														
								dateString=week.FirstDay.Day.ToString()+"/"+week.FirstDay.Month.ToString()+"/"+week.FirstDay.Year.ToString()+" - "+week.LastDay.Day.ToString()+"/"+week.LastDay.Month.ToString()+"/"+week.LastDay.Year.ToString();
								tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+j]=dateString;
								j++;
							}
						}
					}									
					#endregion
					break;																			
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format :	
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format :	
					//libellés format
					tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX] =  GestionWeb.GetWebWord(1401,webSession.SiteLanguage);
					for(int i=0;i<=7;i++){
						j++;
						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+j] =  GestionWeb.GetWebWord(1545+i,webSession.SiteLanguage);											
					}
					break;				
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay :	
					//libellés jour nommés
					tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX] =  GestionWeb.GetWebWord(848,webSession.SiteLanguage);
					for(int i=0;i<=5;i++){
						j++;
						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+j] =  GestionWeb.GetWebWord(1553+i,webSession.SiteLanguage);											
					}
					//week end
					j++;
					tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+j] =  GestionWeb.GetWebWord(1561,webSession.SiteLanguage);
					//Samedi
					j++;
					tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+j] =  GestionWeb.GetWebWord(1559,webSession.SiteLanguage);											
					//Dimanche
					j++;
					tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+j] =  GestionWeb.GetWebWord(1560,webSession.SiteLanguage);												
					break;
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice :																									
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:
					//libellés tranche horaire
					tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX] =  GestionWeb.GetWebWord(1401,webSession.SiteLanguage);
					if(ClassificationCst.DB.Vehicles.names.radio==vehicleType){
						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+1] = GestionWeb.GetWebWord(1562,webSession.SiteLanguage);
						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+2] = GestionWeb.GetWebWord(1563,webSession.SiteLanguage);						
						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+3] = GestionWeb.GetWebWord(1565,webSession.SiteLanguage);
						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+4] = GestionWeb.GetWebWord(1567,webSession.SiteLanguage);
						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+5] = GestionWeb.GetWebWord(1571,webSession.SiteLanguage);
					}else if(ClassificationCst.DB.Vehicles.names.tv==vehicleType || ClassificationCst.DB.Vehicles.names.others==vehicleType){												
						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+1] = GestionWeb.GetWebWord(1564,webSession.SiteLanguage);
						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+2] = GestionWeb.GetWebWord(1566,webSession.SiteLanguage);						
						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+3] = GestionWeb.GetWebWord(1568,webSession.SiteLanguage);
						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+4] = GestionWeb.GetWebWord(1569,webSession.SiteLanguage);
						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+5] = GestionWeb.GetWebWord(1570,webSession.SiteLanguage);
						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+6] = GestionWeb.GetWebWord(1572,webSession.SiteLanguage);
						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+7] = GestionWeb.GetWebWord(1573,webSession.SiteLanguage);
					}					
					break;	
				
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector :
					//libellés familles en colonne					
					tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX] =  GestionWeb.GetWebWord(1401,webSession.SiteLanguage);
					j++;
					string strExpr;
					string strSort;
					
					strExpr = "";					
					strSort = "sector ASC";
				
					DataRow[] foundRows = dt.Select( strExpr, strSort, DataViewRowState.OriginalRows );

					foreach(DataRow dr in  foundRows){

						if(!sectorHashTable.Contains(dr["id_sector"].ToString())){
							tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+j] = dr["sector"].ToString();
							sectorHashTable.Add(dr["id_sector"].ToString(),CstResults.DashBoard.TOTAL_COLUMN_INDEX+j);
							j++;							
						}
				
					}
					
					break;
				default : 
					throw new DashBoardRulesException("SetTabLabel(WebSession webSession,object[,] tab,ClassificationCst.DB.Vehicles.names vehicleType,DataTable dt) : Impossible d'identifier le tableau de bord à traiter.");
			}
			return tab;

		}
		/// <summary>
		/// Insert identifiant et libellé d'un l'élément
		/// déans une cellule du tableau de résultats
		/// </summary>
		/// <param name="tab"> tableau de résultats</param>
		/// <param name="id"> identifiant</param>
		/// <param name="name">libellé d'un l'élément</param>
		/// <param name="row">index ligne tableau de résultats</param>	
		/// <param name="colId">identifiant colonne</param>	
		/// <param name="colName">libellé colonne</param>
		/// <returns> tableau de résultats</returns>
		private static object[,] SetTabLinesLabel(object[,] tab,string id,string name,int row,int colId,int colName){
			tab[row,colId]=id;
			tab[row,colName]=name;
			return tab;
		}
		#endregion

		#region Libellés du tableau for Generic UI
//		/// <summary>
//		/// Insert les libellés du tableau
//		/// </summary>
//		/// <param name="webSession">session du client</param>
//		/// <param name="tab">tableau </param>
//		/// <param name="vehicleType">type de média</param>
//		/// <param name="dt">Table de données résultats</param>
//		/// <param name="sectorHashTable">Liste identifiant des familles à afficher en colonne pour tableau Media\Famille</param>
//		/// <returns>tableau de résultats</returns>
//		private static ResultTable SetTabLabelForGenericUI(WebSession webSession,ResultTable tab,ClassificationCst.DB.Vehicles.names vehicleType,DataTable dt,Hashtable sectorHashTable)
//		{
//			#region variables
//			AtomicPeriodWeek week;
//			//DateTime dateBegin;
//			//DateTime dateEnd;
//			string dateString;
//			long level1CurrentIndex=0;
//			int j=0;
//			
//			#endregion
//
//			switch(webSession.PreformatedTable)
//			{
//				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units :
//					#region Unités
//					level1CurrentIndex=tab.AddNewLine(LineType.level1);
//					if(ClassificationCst.DB.Vehicles.names.press==vehicleType)
//					{
//						//libellés unités : Euros,Mm/Col,pages,Isertions
//						tab[0,CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS] = new CellLevel(0,GestionWeb.GetWebWord(1423,webSession.SiteLanguage),0);
//						tab[0,CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS+1] = new CellLevel(0,GestionWeb.GetWebWord(1424,webSession.SiteLanguage),0);
//						tab[0,CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS+2] = new CellLevel(0,GestionWeb.GetWebWord(943,webSession.SiteLanguage),0);
//						tab[0,CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS+3] = new CellLevel(0,GestionWeb.GetWebWord(940,webSession.SiteLanguage),0);
//					}
//					else
//					{
//						//libellés unités : Euros,Durée,Spots
//						tab[0,CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS] = new CellLevel(0,GestionWeb.GetWebWord(1423,webSession.SiteLanguage),0);
//						tab[0,CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS+1] = new CellLevel(0,GestionWeb.GetWebWord(1435,webSession.SiteLanguage),0);						
//						tab[0,CstResults.DashBoard.NB_TOTAL_CONST_COLUMNS+2] = new CellLevel(0,GestionWeb.GetWebWord(939,webSession.SiteLanguage),0);
//					}
//					#endregion
//					break;
//				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual :
//				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual :										
//					#region période
//					if(webSession.DetailPeriod==CstPeriodDetail.monthly)
//					{
//						//libellés mois
//						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX] = new CellLevel(0,GestionWeb.GetWebWord(1401,webSession.SiteLanguage),0);
//						j++;
//						if(!webSession.DetailPeriodBeginningDate.Equals("0") && WebFunctions.CheckedText.IsStringEmpty(webSession.DetailPeriodBeginningDate))
//						{
//							for(int i=int.Parse(webSession.DetailPeriodBeginningDate.Substring(4,2));i<=int.Parse(webSession.DetailPeriodEndDate.Substring(4,2));i++)
//							{
//								tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+j] = new CellLevel(0,MonthString.Get(i,webSession.SiteLanguage,0),0);
//								j++;
//							}
//						}
//						else
//						{							
//							for(int i=int.Parse(webSession.PeriodBeginningDate.Substring(4,2));i<=int.Parse(webSession.PeriodEndDate.Substring(4,2));i++)
//							{
//								tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+j] = new CellLevel(0,MonthString.Get(i,webSession.SiteLanguage,0),0);
//								j++;
//							}
//						}
//					}
//					else if(webSession.DetailPeriod==CstPeriodDetail.weekly)
//					{
//						//libellés semaines
//						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX] = new CellLevel(0,GestionWeb.GetWebWord(1401,webSession.SiteLanguage),0);
//						j++;
//						if(!webSession.DetailPeriodBeginningDate.Equals("0") && WebFunctions.CheckedText.IsStringEmpty(webSession.DetailPeriodBeginningDate)) 
//						{
//							for(int i=int.Parse(webSession.DetailPeriodBeginningDate.Substring(4,2));i<=int.Parse(webSession.DetailPeriodEndDate.Substring(4,2));i++)
//							{							
//								week=new AtomicPeriodWeek(int.Parse(webSession.DetailPeriodBeginningDate.Substring(0,4)),i);														
//								dateString=week.FirstDay.Day.ToString()+"/"+week.FirstDay.Month.ToString()+"/"+week.FirstDay.Year.ToString()+" - "+week.LastDay.Day.ToString()+"/"+week.LastDay.Month.ToString()+"/"+week.LastDay.Year.ToString();
//								tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+j]=new CellLevel(0,dateString,0);
//								j++;
//							}
//						}
//						else
//						{
//							for(int i=int.Parse(webSession.PeriodBeginningDate.Substring(4,2));i<=int.Parse(webSession.PeriodEndDate.Substring(4,2));i++)
//							{							
//								week=new AtomicPeriodWeek(int.Parse(webSession.PeriodBeginningDate.Substring(0,4)),i);														
//								dateString=week.FirstDay.Day.ToString()+"/"+week.FirstDay.Month.ToString()+"/"+week.FirstDay.Year.ToString()+" - "+week.LastDay.Day.ToString()+"/"+week.LastDay.Month.ToString()+"/"+week.LastDay.Year.ToString();
//								tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+j]=new CellLevel(0,dateString,0);
//								j++;
//							}
//						}
//					}									
//					#endregion
//					break;																			
//				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format :
//				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format :	
//				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format :	
//					//libellés format
//					tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX] = new CellLevel(0,GestionWeb.GetWebWord(1401,webSession.SiteLanguage),0);
//					for(int i=0;i<=7;i++)
//					{
//						j++;
//						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+j] = new CellLevel(0,GestionWeb.GetWebWord(1545+i,webSession.SiteLanguage),0);											
//					}
//					break;				
//				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay :
//				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay :
//				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay :	
//					//libellés jour nommés
//					tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX] = new CellLevel(0,GestionWeb.GetWebWord(848,webSession.SiteLanguage),0);
//					for(int i=0;i<=5;i++)
//					{
//						j++;
//						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+j] = new CellLevel(0,GestionWeb.GetWebWord(1553+i,webSession.SiteLanguage),0);											
//					}
//					//week end
//					j++;
//					tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+j] = new CellLevel(0,GestionWeb.GetWebWord(1561,webSession.SiteLanguage),0);
//					//Samedi
//					j++;
//					tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+j] = new CellLevel(0,GestionWeb.GetWebWord(1559,webSession.SiteLanguage),0);											
//					//Dimanche
//					j++;
//					tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+j] = new CellLevel(0,GestionWeb.GetWebWord(1560,webSession.SiteLanguage),0);												
//					break;
//				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice :
//				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice :																									
//				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:
//					//libellés tranche horaire
//					tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX] = new CellLevel(0,GestionWeb.GetWebWord(1401,webSession.SiteLanguage),0);
//					if(ClassificationCst.DB.Vehicles.names.radio==vehicleType)
//					{
//						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+1] = new CellLevel(0,GestionWeb.GetWebWord(1562,webSession.SiteLanguage),0);
//						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+2] = new CellLevel(0,GestionWeb.GetWebWord(1563,webSession.SiteLanguage),0);						
//						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+3] = new CellLevel(0,GestionWeb.GetWebWord(1565,webSession.SiteLanguage),0);
//						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+4] = new CellLevel(0,GestionWeb.GetWebWord(1567,webSession.SiteLanguage),0);
//						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+5] = new CellLevel(0,GestionWeb.GetWebWord(1571,webSession.SiteLanguage),0);
//					}
//					else if(ClassificationCst.DB.Vehicles.names.tv==vehicleType || ClassificationCst.DB.Vehicles.names.others==vehicleType)
//					{												
//						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+1] = new CellLevel(0,GestionWeb.GetWebWord(1564,webSession.SiteLanguage),0);
//						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+2] = new CellLevel(0,GestionWeb.GetWebWord(1566,webSession.SiteLanguage),0);						
//						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+3] = new CellLevel(0,GestionWeb.GetWebWord(1568,webSession.SiteLanguage),0);
//						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+4] = new CellLevel(0,GestionWeb.GetWebWord(1569,webSession.SiteLanguage),0);
//						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+5] = new CellLevel(0,GestionWeb.GetWebWord(1570,webSession.SiteLanguage),0);
//						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+6] = new CellLevel(0,GestionWeb.GetWebWord(1572,webSession.SiteLanguage),0);
//						tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+7] = new CellLevel(0,GestionWeb.GetWebWord(1573,webSession.SiteLanguage),0);
//					}					
//					break;	
//				
//				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector :
//					//libellés familles en colonne					
//					tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX] = new CellLevel(0,GestionWeb.GetWebWord(1401,webSession.SiteLanguage),0);
//					j++;
//					string strExpr;
//					string strSort;
//					
//					strExpr = "";					
//					strSort = "sector ASC";
//				
//					DataRow[] foundRows = dt.Select( strExpr, strSort, DataViewRowState.OriginalRows );
//
//					foreach(DataRow dr in  foundRows)
//					{
//
//						if(!sectorHashTable.Contains(dr["id_sector"].ToString()))
//						{
//							tab[0,CstResults.DashBoard.TOTAL_COLUMN_INDEX+j] = new CellLevel(0,dr["sector"].ToString(),0);
//							sectorHashTable.Add(dr["id_sector"].ToString(),CstResults.DashBoard.TOTAL_COLUMN_INDEX+j);
//							j++;							
//						}
//				
//					}
//					
//					break;
//				default : 
//					throw new DashBoardRulesException("SetTabLabel(WebSession webSession,object[,] tab,ClassificationCst.DB.Vehicles.names vehicleType,DataTable dt) : Impossible d'identifier le tableau de bord à traiter.");
//			}
//			return tab;
//
//		}
//		/// <summary>
//		/// Insert identifiant et libellé d'un l'élément
//		/// déans une cellule du tableau de résultats
//		/// </summary>
//		/// <param name="tab"> tableau de résultats</param>
//		/// <param name="id"> identifiant</param>
//		/// <param name="name">libellé d'un l'élément</param>
//		/// <param name="row">index ligne tableau de résultats</param>	
//		/// <param name="colId">identifiant colonne</param>	
//		/// <param name="colName">libellé colonne</param>
//		/// <returns> tableau de résultats</returns>
//		private static ResultTable SetTabLinesLabelForGenericUI(ResultTable tab,string id,string name,int row,int colId,int colName)
//		{
//			tab[row,colId]=new CellLevel(row,id.ToString(),1);
//			tab[row,colName]=new CellLevel(row,name,1);
//			return tab;
//		}
		#endregion

		#region Formattage Période sélectionné
		/// <summary>
		/// Formatte les dates sélectionnés sous forme YYYYMMDD
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="PeriodBeginningDate">période de début</param>
		/// <param name="PeriodEndDate">période de fin</param>
		/// <param name="isPreviousYear">vrai si année précédente</param>		
		/// <param name="year">année sélectionné</param>
		private static void CriteriaPeriod(WebSession webSession ,ref string PeriodBeginningDate,ref string PeriodEndDate,int year,bool isPreviousYear)
		{
				AtomicPeriodWeek PeriodBeginningweek;	
				AtomicPeriodWeek PeriodEndweek;
				switch(webSession.DetailPeriod ){					
					case CstPeriodDetail.monthly :
						//période mensuelle
						if(isPreviousYear)year = year-1;
						if(!webSession.DetailPeriodBeginningDate.Equals("0") && WebFunctions.CheckedText.IsStringEmpty(webSession.DetailPeriodBeginningDate)) {
							if(DashBoardDataAccess.IsCrossRepartitionType(webSession)){
								PeriodBeginningDate = year.ToString()+ webSession.DetailPeriodBeginningDate.Substring(4,2)+"01";
								PeriodEndDate = year.ToString()+ webSession.DetailPeriodEndDate.Substring(4,2)+"31";
							}else{
								PeriodBeginningDate = year.ToString()+ webSession.DetailPeriodBeginningDate.Substring(4,2);
								PeriodEndDate = year.ToString()+ webSession.DetailPeriodEndDate.Substring(4,2);
							}
						}else{
							if(DashBoardDataAccess.IsCrossRepartitionType(webSession)){
								PeriodBeginningDate = year.ToString()+ webSession.PeriodBeginningDate.Substring(4,2)+"01";
								PeriodEndDate = year.ToString()+ webSession.PeriodEndDate.Substring(4,2)+"31";
							}else{
								PeriodBeginningDate = year.ToString()+ webSession.PeriodBeginningDate.Substring(4,2);
								PeriodEndDate = year.ToString()+ webSession.PeriodEndDate.Substring(4,2);
							}
						}
						break;
					case CstPeriodDetail.weekly :	
						//période hebdomadaire
						if(!webSession.DetailPeriodBeginningDate.Equals("0") && WebFunctions.CheckedText.IsStringEmpty(webSession.DetailPeriodBeginningDate)){ 							
								PeriodBeginningweek=new AtomicPeriodWeek(year,int.Parse(webSession.DetailPeriodBeginningDate.Substring(4,2)));
								if(isPreviousYear)PeriodBeginningweek.SubWeek(52);							
						}
						else{							
							PeriodBeginningweek=new AtomicPeriodWeek(year,int.Parse(webSession.PeriodBeginningDate.Substring(4,2)));							
							if(isPreviousYear) PeriodBeginningweek.SubWeek(52);
						}
						if(DashBoardDataAccess.IsCrossRepartitionType(webSession)){
							PeriodBeginningDate = PeriodBeginningweek.FirstDay.Year+(PeriodBeginningweek.FirstDay.Month.ToString().Length>1?PeriodBeginningweek.FirstDay.Month.ToString():"0"+PeriodBeginningweek.FirstDay.Month.ToString())
								+(PeriodBeginningweek.FirstDay.Day.ToString().Length>1?PeriodBeginningweek.FirstDay.Day.ToString():"0"+PeriodBeginningweek.FirstDay.Day.ToString() );					
						}else PeriodBeginningDate = PeriodBeginningweek.Year+(PeriodBeginningweek.Week.ToString().Length>1?PeriodBeginningweek.Week.ToString():"0"+PeriodBeginningweek.Week.ToString());														
						
						if(!webSession.DetailPeriodBeginningDate.Equals("0") && WebFunctions.CheckedText.IsStringEmpty(webSession.DetailPeriodBeginningDate)){ 
							PeriodEndweek=new AtomicPeriodWeek(year,int.Parse(webSession.DetailPeriodEndDate.Substring(4,2)));	
							if(isPreviousYear)PeriodEndweek.SubWeek(52);
						}else {
							PeriodEndweek=new AtomicPeriodWeek(year,int.Parse(webSession.PeriodEndDate.Substring(4,2)));	
							if(isPreviousYear)PeriodEndweek.SubWeek(52);
						}
						if(DashBoardDataAccess.IsCrossRepartitionType(webSession)){
							PeriodEndDate = PeriodEndweek.LastDay.Year.ToString()+(PeriodEndweek.LastDay.Month.ToString().Length>1?PeriodEndweek.LastDay.Month.ToString(): "0"+PeriodEndweek.LastDay.Month.ToString())
								+(PeriodEndweek.LastDay.Day.ToString().Length>1? PeriodEndweek.LastDay.Day.ToString(): "0"+PeriodEndweek.LastDay.Day.ToString());					
						}else PeriodEndDate = PeriodEndweek.Year.ToString()+(PeriodEndweek.Week.ToString().Length>1?PeriodEndweek.Week.ToString(): "0"+PeriodEndweek.Week.ToString());
						break;
					default:
						throw(new DashBoardRulesException(" CriteriaPeriod(WebSession webSession ,ref string PeriodBeginningDate,ref string PeriodEndDate,int year) : Impossible de déterminer le type de période."));
				}
		}
		#endregion
		
		private static int GetCurrentRow(WebSession webSession, int row){
			
			//Incrément période
			row=row+1;
			if(webSession.ComparativeStudy)	{
				row=row+1;
			}

			//Incrémente numero ligne pour PDM ou PDV
			if(webSession.PDM || webSession.PDV){			
				if(webSession.ComparativeStudy ){
					row=row+2;
				}else {											
					row=row+1;
				}			
			}
					
			//Evolution
			if(webSession.Evolution){				
				row=row+1;			
			}
			
			return row;
		}
		#endregion		
	
	}
}
