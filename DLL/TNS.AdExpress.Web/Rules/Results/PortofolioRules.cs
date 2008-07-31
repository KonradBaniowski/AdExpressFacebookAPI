#region Informations
// Auteur: D. V. Mussuma 
// Date de création: 06/12/2004 
// Date de modification:06/12/2004 
//		K.Shehzad Date de modification:29/06/2005 Bug fixed in detail support
//		K.Shehzad Date de modification:29/06/2005 Bug fixed in detail support
//		G.Ragneau - 08/12/2006 - Prise en compte des niveaux de détails génériques
#endregion

#region Namespace
using System;
using System.Reflection;
using System.Data;
using System.Collections;
using System.Windows.Forms;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.FrameWork.Date;
using TNS.AdExpress.Web.Exceptions;

using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using CustomerConstantes=TNS.AdExpress.Constantes.Customer;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using WebCommon=TNS.AdExpress.Web.Common;
using WebCore=TNS.AdExpress.Web.Core;
using WebDataAccess = TNS.AdExpress.Web.DataAccess;
using ClassificationConstantes=TNS.AdExpress.Constantes.Classification;
using ClassificationDB=TNS.AdExpress.DataAccess.Classification;
using TNS.AdExpress.Domain.Translation;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using TNS.AdExpress.Domain.Translation;
using RulesConstantes=TNS.AdExpress.Web.Rules.Results;
using TNS.FrameWork.WebResultUI;
using DateFmk = TNS.FrameWork.Date;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
#endregion


namespace TNS.AdExpress.Web.Rules.Results{
	/// <summary>
	/// Obtient les résultats pour le portefeuille d'un support.
	/// - getFormattedTable(WebSession webSession) donne le détail d'un portefeuille.
	/// - getStructRadioTab(WebSession webSession,int dateBegin,int dateEnd) donne la structure du portefeuille pour la presse.
	/// -  getStructTVTab(WebSession webSession,int dateBegin,int dateEnd) donne la structure du portefeuille pour la télévision.
	/// - getStructPressTab(WebSession webSession, int dateBegin,int dateEnd,out DataTable dtFormat, out DataTable dtColor, out DataTable dtLocation,out DataTable dtInsert)
	/// donne la structure du portefeuille pour la presse.
	/// - getFormattedTableDetailMedia(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd) donne le détail d'un support.
	/// </summary>
	public class PortofolioRules {

		#region Constantes
		const long TOTAL_LINE_INDEX=0;
		const long DETAILED_PORTOFOLIO_EURO_COLUMN_INDEX=2;
		const long DETAILED_PORTOFOLIO_INSERTION_COLUMN_INDEX=3;
		const long DETAILED_PORTOFOLIO_DURATION_COLUMN_INDEX=4;
		const long DETAILED_PORTOFOLIO_MMC_COLUMN_INDEX=4;
		const long DETAILED_PORTOFOLIO_PAGE_COLUMN_INDEX=5;	
		#endregion

		#region Détail du portefeuille
		/// <summary>
		/// Obtient le tableau contenant l'ensemble des résultats
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Tableau de résultats</returns>
		public static ResultTable GetResultTable(WebSession webSession){

			#region Constantes
			const int PROD_COL = 1164;
            const int INSERTIONS_LIST_COL = 2245;
			const int CREATIVES_COL = 1994;
			const int PM_COL = 751;
			const int EUROS_COL = 1423;
			const int MM_COL = 1424;
			const int SPOTS_COL = 939;
			const int INSERTIONS_COL = 940;
			const int PAGE_COL =943;
			const int PAN_COL = 1604;
			const int DURATION_COL = 1435;
            const int VOLUME = 2216;
			#endregion

			#region Variables
			ResultTable tab=null;
			DataSet ds  =null;
			DataTable dt =null;
			CellUnitFactory[] cellFactories;
			AdExpressCellLevel[] cellLevels;
			LineType[] lineTypes = new LineType[5]{LineType.total,LineType.level1,LineType.level2,LineType.level3,LineType.level4};
			string[] columnsName;
			DBClassificationConstantes.Vehicles.names vehicle;
			TNS.FrameWork.WebResultUI.Headers headers;
			string periodBeginning;
			string periodEnd;
			int iCurLine=0;
			int iNbLine=0;
			int iNbCol=0;
			int iNbLevels=0;
			int insertions = 0;
            int creatives =0;
			#endregion

			#region Formattage des dates
			periodBeginning = GetDateBegin(webSession);
			periodEnd = GetDateEnd(webSession);
			#endregion

			#region Sélection du vehicle
			string vehicleSelection=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerConstantes.Right.type.vehicleAccess);
			DBClassificationConstantes.Vehicles.names vehicleName=(DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
			if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw(new WebExceptions.CompetitorRulesException("La sélection de médias est incorrecte"));
			vehicle = (DBClassificationConstantes.Vehicles.names)Convert.ToInt32(vehicleSelection);
			#endregion
			
			TNS.AdExpress.Domain.Web.Navigation.Module currentModuleDescription=(TNS.AdExpress.Domain.Web.Navigation.Module)ModulesList.GetModule(webSession.CurrentModule);

			#region Chargement des données
			switch(webSession.CurrentModule){
				case WebConstantes.Module.Name.ALERTE_PORTEFEUILLE:								
					ds=PortofolioDataAccess.GetData(webSession,vehicleName,currentModuleDescription.ModuleType,periodBeginning,periodEnd);
					if(ds!=null && ds.Tables[0]!=null && ds.Tables[0].Rows.Count>0){
						dt=ds.Tables[0];
					}
					break;
				case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:				
					//ds=PortofolioAnalysisDataAccess.GetData(webSession,vehicleName,periodBeginning,periodEnd);
                    ds = PortofolioAnalysisDataAccess.GetGenericData(webSession, vehicleName);
					if(ds!=null && ds.Tables[0]!=null && ds.Tables[0].Rows.Count>0 ){
						dt=ds.Tables[0];
					}
					break;
			}
			#endregion

			if(dt!=null && dt.Rows!=null && dt.Rows.Count>0){

				#region Nombre de lignes du tableau du tableau
				iNbLine = 0;
				if(dt!=null && dt.Rows!=null && dt.Rows.Count>0)
					iNbLine=GetPortefolioSize(webSession,dt);
				#endregion

				#region Initialisation du tableau de résultats
				headers = new TNS.FrameWork.WebResultUI.Headers();
				headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(PROD_COL,webSession.SiteLanguage), PROD_COL));
				foreach(DetailLevelItemInformation item in webSession.GenericProductDetailLevel.Levels){
					if (ModulesList.GetModule(webSession.CurrentModule).ModuleType==WebConstantes.Module.Type.alert &&
						(item.Id.Equals(DetailLevelItemInformation.Levels.advertiser)
						|| item.Id.Equals(DetailLevelItemInformation.Levels.product))) {
						insertions = 1;
                        creatives=1;
						break;
					}
				}
                if(creatives>0 && webSession.CustomerLogin.CustormerFlagAccess(TNS.AdExpress.Constantes.DB.Flags.ID_SLOGAN_ACCESS_FLAG)) {
                    headers.Root.Add(new HeaderCreative(false,GestionWeb.GetWebWord(CREATIVES_COL,webSession.SiteLanguage),CREATIVES_COL));
                }
                else creatives=0;
				if (insertions>0){
                    headers.Root.Add(new HeaderCreative(false,GestionWeb.GetWebWord(INSERTIONS_LIST_COL,webSession.SiteLanguage),INSERTIONS_LIST_COL));
				}
				headers.Root.Add(new HeaderMediaSchedule(false, GestionWeb.GetWebWord(PM_COL,webSession.SiteLanguage), PM_COL));
				headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(EUROS_COL,webSession.SiteLanguage), EUROS_COL));
				switch(vehicle){
					case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press:
					case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress:
						headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(MM_COL,webSession.SiteLanguage), MM_COL));
						headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(PAGE_COL,webSession.SiteLanguage), PAGE_COL));
						headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(INSERTIONS_COL,webSession.SiteLanguage), INSERTIONS_COL));
                        iNbCol = 6+creatives+insertions;
						cellFactories = new CellUnitFactory[iNbCol];
						columnsName = new string[iNbCol];
                        columnsName[3+creatives+insertions] = "mmpercol";
                        columnsName[4+creatives+insertions] = "pages";
                        columnsName[5+creatives+insertions] = "insertion";
                        cellFactories[3+creatives+insertions] = new CellUnitFactory(new CellMMC(0.0));
                        cellFactories[4+creatives+insertions] = new CellUnitFactory(new CellPage(0.0));
                        cellFactories[5+creatives+insertions] = new CellUnitFactory(new CellInsertion(0.0));
						break;
                    case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.directMarketing:
                        if (webSession.CustomerLogin.CustormerFlagAccess(TNS.AdExpress.Constantes.DB.Flags.ID_VOLUME_MARKETING_DIRECT)) {
                            headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(VOLUME, webSession.SiteLanguage), VOLUME));
                            iNbCol = 4 + creatives+insertions;
                            cellFactories = new CellUnitFactory[iNbCol];
                            columnsName = new string[iNbCol];
                            columnsName[3 + creatives+insertions] = "volume";
                            cellFactories[3 + creatives+insertions] = new CellUnitFactory(new CellVolume(0.0));
                        }
                        else {
                            iNbCol = 3 + creatives+insertions;
                            cellFactories = new CellUnitFactory[iNbCol];
                            columnsName = new string[iNbCol];
                        }
                        break;
					case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radio:
					case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.others:
					case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tv:
						headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(DURATION_COL,webSession.SiteLanguage), DURATION_COL));
						headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(SPOTS_COL,webSession.SiteLanguage), SPOTS_COL));
                        iNbCol = 5+creatives+insertions;
						columnsName = new string[iNbCol];
                        columnsName[3+creatives+insertions] = "duration";
                        columnsName[4+creatives+insertions] = "insertion";
						cellFactories = new CellUnitFactory[iNbCol];
                        cellFactories[3+creatives+insertions] = new CellUnitFactory(new CellDuration(0.0));
                        cellFactories[4+creatives+insertions] = new CellUnitFactory(new CellNumber(0.0));
						break;
					case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.outdoor:
						headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(PAN_COL,webSession.SiteLanguage), PAN_COL));
                        iNbCol = 4+creatives+insertions;
						columnsName = new string[iNbCol];
                        columnsName[3+creatives+insertions] = "insertion";
						cellFactories = new CellUnitFactory[iNbCol];
                        cellFactories[3+creatives+insertions] = new CellUnitFactory(new CellNumber(0.0));
						break;
					case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internet:
                        iNbCol = 3+creatives+insertions;
						columnsName = new string[iNbCol];					
						cellFactories = new CellUnitFactory[iNbCol];					
						break;
					default:
						throw new WebExceptions.CompetitorRulesException("Média non traité.");
				}
				cellFactories[0] = null;
				cellFactories[1] = null;
                if(creatives>0) columnsName[1+creatives] = null;
                if(insertions>0) columnsName[1+creatives+insertions] = null;
                columnsName[2+creatives+insertions] = "euro";
                cellFactories[2+creatives+insertions] = new CellUnitFactory(new CellEuro(0.0));
				tab = new ResultTable(iNbLine, headers);
				#endregion
			
				#region Traitement du tableau de résultats
			
				#region Intialisation des totaux
				iNbLevels = webSession.GenericProductDetailLevel.GetNbLevels;
				cellLevels = new AdExpressCellLevel[iNbLevels+1];
				tab.AddNewLine(LineType.total);
				tab[iCurLine, 1] = cellLevels[0] = new AdExpressCellLevel(0, GestionWeb.GetWebWord(805,webSession.SiteLanguage), 0, iCurLine, webSession);
				//Creatives
                if(creatives > 0) tab[iCurLine,1+creatives] = new CellOneLevelCreativesLink((AdExpressCellLevel)tab[iCurLine,1],webSession,webSession.GenericProductDetailLevel);
                if(insertions > 0) tab[iCurLine,1+creatives+insertions] = new CellOneLevelInsertionsLink((AdExpressCellLevel)tab[iCurLine,1],webSession,webSession.GenericProductDetailLevel); 
				
                tab[iCurLine,2+creatives+insertions] = new CellMediaScheduleLink(cellLevels[0],webSession);
				AffectPortefolioLine(cellFactories, columnsName, null, tab, iCurLine, false);
				#endregion

				int i = 1;
				long dCurLevel=0;
				foreach(DataRow row in dt.Rows){
					//pour chaque niveau
					for(i=1; i <= iNbLevels; i++){
						//nouveau niveau i
						dCurLevel = webSession.GenericProductDetailLevel.GetIdValue(row, i);
						if(dCurLevel > 0 && (cellLevels[i]==null || dCurLevel!=cellLevels[i].Id)){
							for (int j = i+1; j < cellLevels.Length; j++){
								cellLevels[j] = null;
							}
							iCurLine++;
							tab.AddNewLine(lineTypes[i]);
							tab[iCurLine, 1] = cellLevels[i] = new AdExpressCellLevel(dCurLevel, webSession.GenericProductDetailLevel.GetLabelValue(row, i), cellLevels[i-1], i, iCurLine, webSession); 
							if (row.Table.Columns.Contains("id_address") && row["id_address"]!=System.DBNull.Value){
								cellLevels[i].AddressId = Convert.ToInt64(row["id_address"]);
							}
							//Creatives
                            if(creatives>0) tab[iCurLine,1+creatives] = new CellOneLevelCreativesLink((AdExpressCellLevel)tab[iCurLine,1],webSession,webSession.GenericProductDetailLevel);
                            if(insertions > 0) tab[iCurLine,1+creatives+insertions] = new CellOneLevelInsertionsLink((AdExpressCellLevel)tab[iCurLine,1],webSession,webSession.GenericProductDetailLevel);
                            tab[iCurLine,2+creatives+insertions] = new CellMediaScheduleLink((AdExpressCellLevel)tab[iCurLine,1],webSession); 
							//feuille ou niveau parent?
							if (i != iNbLevels){
								AffectPortefolioLine(cellFactories, columnsName, null, tab, iCurLine, false);
							}
							else{
								AffectPortefolioLine(cellFactories, columnsName, row, tab, iCurLine, true);
							}
						}
					}
				}

			
				#endregion
			}
			return tab;
		}

		private static void AffectPortefolioLine(CellUnitFactory[] tCellFactories, string[] columnsName, DataRow dr, ResultTable oTab, int iLineIndex, bool isLeaf){

			for (int i = 0; i < tCellFactories.Length; i++){
				if (tCellFactories[i] != null){
					oTab[iLineIndex, i+1] = tCellFactories[i].Get(0.0);
					if (dr != null){
						if (isLeaf){
							oTab.AffectValueAndAddToHierarchy(1, iLineIndex, i+1, Convert.ToDouble(dr[columnsName[i]]));
						}
					}
				}
			}

		}


		#region private static int GetPortefolioSize(WebSession webSession, DataTable dt)
		/// <summary>
		/// Calcul la taille du tableau de résultats
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dt">table de données</param>
		/// <returns>nombre de ligne du tableau de résultats</returns>
		private static int GetPortefolioSize(WebSession webSession, DataTable dt){

			Int64 OldL1Id=0;
			Int64 cL1Id=0;
			Int64 nbL1Id=0;
			Int64 OldL2Id=0;
			Int64 cL2Id=0;
			Int64 nbL2Id=0;
			Int64 OldL3Id=0;
			Int64 cL3Id=0;
			Int64 nbL3Id=0;
			
			Int64 nbLine=0;
		
			if(dt!=null && dt.Rows.Count>0){
				foreach(DataRow dr in dt.Rows){
					cL1Id = webSession.GenericProductDetailLevel.GetIdValue(dr,1);
					if( cL1Id > 0 && cL1Id!=OldL1Id ){
						nbL1Id++;
						OldL1Id=cL1Id;
						OldL2Id=OldL3Id=-1;
					}
					cL2Id = webSession.GenericProductDetailLevel.GetIdValue(dr,2);
					if( cL2Id>0 && OldL2Id!=cL2Id ){						
						nbL2Id++;
						OldL2Id=cL2Id;
						OldL3Id=-1;
					}
					cL3Id = webSession.GenericProductDetailLevel.GetIdValue(dr,3);
					if( cL3Id>0 && OldL3Id!=cL3Id){
						nbL3Id++;
						OldL3Id=cL3Id;
					}
				}
				
			}
			if((nbL1Id>0) || (nbL2Id>0) || (nbL3Id>0)){
				nbLine=nbL1Id+nbL2Id+nbL3Id+1;
			}
			return (int)nbLine;
		}
		#endregion
		
		#endregion 

		#region Calendrier d'actions
		/// <summary>
		/// Obtient le tableau contenant l'ensemble des résultats
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Tableau de résultats</returns>
		public static ResultTable GetCalendar(WebSession webSession){

			#region Constantes
			const int PROD_COL = 1164;
			const int PM_COL = 751;
			const int TOTAL_COL = 1401;
			const int POURCENTAGE_COL = 1236;
			#endregion

			#region Variables
			ResultTable tab=null;
			DataSet ds  =null;
			DataTable dt =null;
			CellUnitFactory cellFactory;
			AdExpressCellLevel[] cellLevels;
			LineType[] lineTypes = new LineType[5]{LineType.total,LineType.level1,LineType.level2,LineType.level3,LineType.level4};
			DBClassificationConstantes.Vehicles.names vehicle;
			Headers headers;
			string periodBeginning;
			string periodEnd;
			int iCurLine=0;
			int iNbLine=0;
			int iNbLevels=0;
			ArrayList parutions = new ArrayList();
			#endregion

			#region Formattage des dates
			periodBeginning = GetDateBegin(webSession);
			periodEnd = GetDateEnd(webSession);
			#endregion

			#region Sélection du vehicle
			string vehicleSelection=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerConstantes.Right.type.vehicleAccess);
			DBClassificationConstantes.Vehicles.names vehicleName=(DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
			if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw(new WebExceptions.CompetitorRulesException("La sélection de médias est incorrecte"));
			vehicle = (DBClassificationConstantes.Vehicles.names)Convert.ToInt32(vehicleSelection);
			#endregion

			#region Chargement des données
            TNS.AdExpress.Domain.Web.Navigation.Module currentModuleDescription = ModulesList.GetModule(webSession.CurrentModule);
			ds = PortofolioDataAccess.GetDataCalendar(webSession, vehicleName, currentModuleDescription.ModuleType, periodBeginning, periodEnd);
			if(ds!=null && ds.Tables[0]!=null){
				dt=ds.Tables[0];
			}
			#endregion

			#region Nombre de lignes du tableau du tableau
			iNbLine = 0;
			if(dt!=null && dt.Rows!=null && dt.Rows.Count>0)
				iNbLine=GetCalendarSize(webSession, dt, parutions);
			#endregion

			#region Initialisation du tableau de résultats
			headers = new Headers();
			headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(PROD_COL,webSession.SiteLanguage), PROD_COL));
			headers.Root.Add(new HeaderMediaSchedule(false, GestionWeb.GetWebWord(PM_COL,webSession.SiteLanguage), PM_COL));
			headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(TOTAL_COL,webSession.SiteLanguage), TOTAL_COL));
			headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(POURCENTAGE_COL,webSession.SiteLanguage), POURCENTAGE_COL));

			//une colonne par date de parution
			parutions.Sort();
			foreach(Int32 parution  in parutions){
				headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, DateFmk.DateString.YYYYMMDDToDD_MM_YYYY(parution.ToString(), webSession.SiteLanguage), (long)parution));
			}
			if (!webSession.Percentage){
				switch(webSession.Unit){
					case WebConstantes.CustomerSessions.Unit.duration:
						cellFactory = new CellUnitFactory(new CellDuration(0.0));
						break;
					case WebConstantes.CustomerSessions.Unit.euro:
						cellFactory = new CellUnitFactory(new CellEuro(0.0));
						break;
					case WebConstantes.CustomerSessions.Unit.kEuro:
						cellFactory = new CellUnitFactory(new CellKEuro(0.0));
						break;
					case WebConstantes.CustomerSessions.Unit.insertion:
						cellFactory = new CellUnitFactory(new CellInsertion(0.0));
						break;
					case WebConstantes.CustomerSessions.Unit.pages:
						cellFactory = new CellUnitFactory(new CellPage(0.0));
						break;
					case WebConstantes.CustomerSessions.Unit.mmPerCol:
						cellFactory = new CellUnitFactory(new CellMMC(0.0));
						break;
					default:
						cellFactory = new CellUnitFactory(new CellNumber(0.0));
						break;
				}
			}
			else{
				cellFactory = new CellUnitFactory(new CellPDM(0.0));
			}
			tab = new ResultTable(iNbLine, headers);
			#endregion
			
			#region Traitement du tableau de résultats
			if(dt!=null && dt.Rows!=null && dt.Rows.Count>0){

				int i = 1;

				#region Intialisation des totaux
				iNbLevels = webSession.GenericProductDetailLevel.GetNbLevels;
				cellLevels = new AdExpressCellLevel[iNbLevels+1];
				tab.AddNewLine(LineType.total);
				tab[iCurLine, 1] = cellLevels[0] = new AdExpressCellLevel(0, GestionWeb.GetWebWord(805,webSession.SiteLanguage), 0, iCurLine, webSession);
				tab[iCurLine, 2] = new CellMediaScheduleLink(cellLevels[0], webSession);
				if (!webSession.Percentage){
					tab[iCurLine, 3] = cellFactory.Get(0.0);
				}
				else{
					tab[iCurLine, 3] = new CellPDM(0.0, null);
				}
				tab[iCurLine, 4] = new CellPercent(0.0, null);
				for(i = 5; i < 5+parutions.Count; i++){
					if (!webSession.Percentage){
						tab[iCurLine, i] = cellFactory.Get(0.0);
					}
					else{
						tab[iCurLine, i] = new CellPDM(0.0, (CellUnit)tab[iCurLine, 3]);
					}
				}
				#endregion

				i = 1;
				long dCurLevel=0;
				DetailLevelItemInformation.Levels level;
				long lCol = -1;
				double valu = 0.0;
				foreach(DataRow row in dt.Rows){
					//pour chaque niveau
					for(i=1; i <= iNbLevels; i++){
						//nouveau niveau i
						dCurLevel = webSession.GenericProductDetailLevel.GetIdValue(row, i);
						if(dCurLevel > 0 && (cellLevels[i]==null || dCurLevel!=cellLevels[i].Id)){
							iCurLine++;
							tab.AddNewLine(lineTypes[i]);
							tab[iCurLine, 1] = cellLevels[i] = new AdExpressCellLevel(dCurLevel, webSession.GenericProductDetailLevel.GetLabelValue(row, i), cellLevels[i-1], i, iCurLine, webSession); 
							if (row.Table.Columns.Contains("id_address") && row["id_address"]!=System.DBNull.Value){
								cellLevels[i].AddressId = Convert.ToInt64(row["id_address"]);
							}
							level = webSession.GenericProductDetailLevel.GetDetailLevelItemInformation(i);
							//PM
							if (level!=DetailLevelItemInformation.Levels.agency && level!=DetailLevelItemInformation.Levels.groupMediaAgency){
								tab[iCurLine, 2] = new CellMediaScheduleLink((AdExpressCellLevel)tab[iCurLine, 1], webSession); 
							}
							else{
								tab[iCurLine, 2] = new CellEmpty(); 
							}
							//total
							if (!webSession.Percentage){
								tab[iCurLine, 3] = cellFactory.Get(0.0);
							}
							else{
								tab[iCurLine, 3] = new CellPDM(0.0, null);;
							}
							//pourcentage
							tab[iCurLine, 4] = new CellPercent(0.0, (CellUnit)tab[cellLevels[i-1].LineIndexInResultTable, 4]);
							//initialisation des autres colonnes
							for (int j =5; j < 5+parutions.Count; j++){
								if (!webSession.Percentage){
									tab[iCurLine, j] = cellFactory.Get(0.0);
								}
								else{
									tab[iCurLine, j] = new CellPDM(0.0, (CellUnit)tab[iCurLine, 3]);
								}
							}
						}
						//feuille ou niveau parent?
						if (i == iNbLevels){
							lCol = tab.GetHeadersIndexInResultTable(row["date_media_num"].ToString());
							valu = Convert.ToDouble(row["unit"]);
							tab.AffectValueAndAddToHierarchy(1, iCurLine, lCol, valu);
							tab.AffectValueAndAddToHierarchy(1, iCurLine, 4, valu);
							tab.AffectValueAndAddToHierarchy(1, iCurLine, 3, valu);
						}
					}
				}

			}
			#endregion

			return tab;
		}

		#region private static int GetCalendarSize(WebSession webSession, DataTable dt)
		/// <summary>
		/// Calcul la taille du tableau de résultats d'un calendrier d'action
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dt">table de données</param>
		/// <returns>nombre de ligne du tableau de résultats</returns>
        /// <param name="parutions">Parutions</param>
		private static int GetCalendarSize(WebSession webSession, DataTable dt, ArrayList parutions){

			Int64 OldL1Id=0;
			Int64 cL1Id=0;
			Int64 nbL1Id=0;
			Int64 OldL2Id=0;
			Int64 cL2Id=0;
			Int64 nbL2Id=0;
			Int64 OldL3Id=0;
			Int64 cL3Id=0;
			Int64 nbL3Id=0;
			
			Int64 nbLine=0;
		
			if(dt!=null && dt.Rows.Count>0){
				foreach(DataRow dr in dt.Rows){
					cL1Id = webSession.GenericProductDetailLevel.GetIdValue(dr,1);
					if( cL1Id > 0 && cL1Id!=OldL1Id ){
						nbL1Id++;
						OldL1Id=cL1Id;
						OldL2Id=OldL3Id=-1;
					}
					cL2Id = webSession.GenericProductDetailLevel.GetIdValue(dr,2);
					if( cL2Id>0 && OldL2Id!=cL2Id ){						
						nbL2Id++;
						OldL2Id=cL2Id;
						OldL3Id=-1;
					}
					cL3Id = webSession.GenericProductDetailLevel.GetIdValue(dr,3);
					if( cL3Id>0 && OldL3Id!=cL3Id){
						nbL3Id++;
						OldL3Id=cL3Id;
					}
					if(!parutions.Contains(dr["date_media_num"])) {
						parutions.Add(dr["date_media_num"]);
					}
				}
				
			}
			if((nbL1Id>0) || (nbL2Id>0) || (nbL3Id>0)){
				nbLine=nbL1Id+nbL2Id+nbL3Id+1;
			}
			return (int)nbLine;
		}
		#endregion
		
		#endregion 
				
		#region Détail support
		/// <summary>
		/// Créer un tableau avec pour chaque jour de la semaine
		/// l'investissement du support et le nombre de spot
		/// </summary>
		/// <param name="webSession">Session Client</param>
		/// <param name="idVehicle">identifiant du vehicle</param>
		/// <param name="idMedia">identifiant média</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <returns>tableau avec pour chaque jour de la semaine
		/// l'investissement du support et le nombre de spot</returns>
		public static int[,] GetFormattedTableDetailMedia(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd){
		
			#region Variables
			int[,] tab=null;			
			DataTable dt =null;	
			DateTime dayDT;
			int currentLine=-1;
			int oldEcranCode=-1;
			int ecranCode;			
			#endregion
		
//			dt=TNS.AdExpress.Web.DataAccess.Results.PortofolioDataAccess.GetCommercialBreakForTvRadio(webSession,idVehicle,idMedia,dateBegin,dateEnd).Tables[0];
			dt=TNS.AdExpress.Web.DataAccess.Results.PortofolioDetailMediaDataAccess.GetCommercialBreakForTvRadio(webSession,idVehicle,idMedia,dateBegin,dateEnd).Tables[0];

			#region Initialisation du tableau
			tab= new int[dt.Rows.Count,FrameWorkResultConstantes.PortofolioDetailMedia.TOTAL_INDEX];			
			#endregion

			#region Parcours du tableau
			foreach(DataRow row in dt.Rows){
				ecranCode=int.Parse(row["code_ecran"].ToString());
				dayDT=new DateTime(int.Parse(row["date_media_num"].ToString().Substring(0,4)),int.Parse(row["date_media_num"].ToString().Substring(4,2)),int.Parse(row["date_media_num"].ToString().Substring(6,2)));
				if(ecranCode!=oldEcranCode){
					currentLine++;
					oldEcranCode=ecranCode;					
					tab[currentLine,FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN]=int.Parse(row["code_ecran"].ToString());										
				}
				switch(dayDT.DayOfWeek.ToString()){
						
					case "Monday":
						tab[currentLine,FrameWorkConstantes.Results.PortofolioDetailMedia.MONDAY_VALUE]+=int.Parse(row["value"].ToString());
						tab[currentLine,FrameWorkConstantes.Results.PortofolioDetailMedia.MONDAY_INSERTION]+=int.Parse(row["insertion"].ToString());
						break;
					case "Tuesday":
						tab[currentLine,FrameWorkConstantes.Results.PortofolioDetailMedia.TUESDAY_VALUE]+=int.Parse(row["value"].ToString());
						tab[currentLine,FrameWorkConstantes.Results.PortofolioDetailMedia.TUESDAY_INSERTION]+=int.Parse(row["insertion"].ToString());
						break;
					case "Wednesday":
						tab[currentLine,FrameWorkConstantes.Results.PortofolioDetailMedia.WEDNESDAY_VALUE]+=int.Parse(row["value"].ToString());
						tab[currentLine,FrameWorkConstantes.Results.PortofolioDetailMedia.WEDNESDAY_INSERTION]+=int.Parse(row["insertion"].ToString());
						break;
					case "Thursday":
						tab[currentLine,FrameWorkConstantes.Results.PortofolioDetailMedia.THURSDAY_VALUE]+=int.Parse(row["value"].ToString());
						tab[currentLine,FrameWorkConstantes.Results.PortofolioDetailMedia.THURSDAY_INSERTION]+=int.Parse(row["insertion"].ToString());
						break;
					case "Friday":
						tab[currentLine,FrameWorkConstantes.Results.PortofolioDetailMedia.FRIDAY_VALUE]+=int.Parse(row["value"].ToString());
						tab[currentLine,FrameWorkConstantes.Results.PortofolioDetailMedia.FRIDAY_INSERTION]+=int.Parse(row["insertion"].ToString());
						break;
					case "Saturday":
						tab[currentLine,FrameWorkConstantes.Results.PortofolioDetailMedia.SATURDAY_VALUE]+=int.Parse(row["value"].ToString());
						tab[currentLine,FrameWorkConstantes.Results.PortofolioDetailMedia.SATURDAY_INSERTION]+=int.Parse(row["insertion"].ToString());
						break;
					case "Sunday":
						tab[currentLine,FrameWorkConstantes.Results.PortofolioDetailMedia.SUNDAY_VALUE]+=int.Parse(row["value"].ToString());
						tab[currentLine,FrameWorkConstantes.Results.PortofolioDetailMedia.SUNDAY_INSERTION]+=int.Parse(row["insertion"].ToString());
						break;					
				}				
			}
			//if condition added to fix the bug in detail support when we select the single date
			if(currentLine+1<dt.Rows.Count)
			tab[currentLine+1,FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN]=FrameWorkConstantes.Results.PortofolioDetailMedia.END_ARRAY;
			#endregion

			return tab;		
		}
		#endregion		

        #region GetPortofolioDetailMediaResultTable (Generation du tableResult pour l'affichage du résultat de la page 'PortofolioDetailMediaPopUp'
        /// <summary>
		/// Accès à la construction du tableau du détail média pour le module portefeuille
		/// </summary>		
		/// <param name="webSession">Session du client</param>
        /// <param name="dayOfWeek">jour de la semaine</param>
        /// <param name="adBreak">Code écran</param>
		/// <returns>Code HTML du tableau du parrainage </returns>
        public static ResultTable GetPortofolioDetailMediaResultTable(WebSession webSession, string dayOfWeek, string adBreak) {

            #region Variables
            ResultTable tab = null;
            DataSet ds;
            DataTable dt = null;
            Headers headers;
            ArrayList columnItemList;
            int iCurLine = 0;
            int iNbLine = 0;
            Assembly assembly;
            Type type;
            bool allPeriod = false;
			//string mediaAgencyYear = "";
            Int64 idMedia = ((LevelInformation)webSession.ReferenceUniversMedia.FirstNode.Tag).ID;
            bool isDigitalTV = DataAccess.Results.PortofolioDetailMediaDataAccess.IsMediaBelongToCategory(webSession, idMedia, TNS.AdExpress.Constantes.DB.Category.ID_DIGITAL_TV, webSession.SiteLanguage);
            #endregion

            #region Niveau de détail produit (Generic)
            // Initialisation à produit
            ArrayList levels = new ArrayList();
            levels.Add(10);
            webSession.GenericProductDetailLevel = new GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
            #endregion

            #region Niveau de colonnes (Generic)
            columnItemList = PortofolioDetailMediaColumnsInformation.GetDefaultMediaDetailColumns(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID);

            ArrayList columnIdList = new ArrayList();
            foreach (GenericColumnItemInformation Column in columnItemList)
                columnIdList.Add((int)Column.Id);

            webSession.GenericInsertionColumns = new GenericColumns(columnIdList);
            webSession.Save();
            #endregion

            #region Obtention du vehicle
            string vehicleSelection = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerConstantes.Right.type.vehicleAccess);
            if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new WebExceptions.CompetitorRulesException("La sélection de médias est incorrecte"));
            DBClassificationConstantes.Vehicles.names vehicle = (DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
            #endregion

            #region On récupère la dernière année des agences médias

			//DataTable dtAgency = TNS.AdExpress.Web.DataAccess.Results.MediaAgencyDataAccess.GetListYear(webSession).Tables[0];

			//if (dtAgency != null && dtAgency.Rows.Count > 0) {
			//    //On récupère la dernière année des agences médias
			//    mediaAgencyYear = dtAgency.Rows[0]["year"].ToString();
			//}
            #endregion

            #region Chargement des données
            try {
                if (adBreak.Length > 0 || dayOfWeek.Length > 0) {
                    ds = DataAccess.Results.PortofolioDetailMediaDataAccess.GetGenericDetailMedia(webSession, vehicle.GetHashCode(), idMedia,  webSession.PeriodBeginningDate, webSession.PeriodEndDate, adBreak, allPeriod);
                }
                else {
                    allPeriod = true;
                    ds = DataAccess.Results.PortofolioDetailMediaDataAccess.GetGenericDetailMedia(webSession, vehicle.GetHashCode(), idMedia, webSession.PeriodBeginningDate, webSession.PeriodEndDate, adBreak, allPeriod);
                }
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0) {
                    dt = ds.Tables[0];
                }
            }
            catch (System.Exception err) { 
                throw (new PortofolioDetailMediaException("Erreur lors de l'extraction des données pour le détail media du portefeuille",err));
            }
            #endregion

            #region Cas de la Presse et de la Presse Internatioanl
            try {
                if (vehicle == DBClassificationConstantes.Vehicles.names.press || vehicle == DBClassificationConstantes.Vehicles.names.internationalPress)
                    SetDataTable(dt, dayOfWeek, allPeriod);
            }
            catch (System.Exception err) {
                throw (new PortofolioDetailMediaException("Erreur lors de la suppression des lignes du dataTable (cas de la presse) pour le détail media du portefeuille", err));
            }
            #endregion

            if (dt != null && dt.Rows != null && dt.Rows.Count > 0) {

                #region Gestion des droits
                // Affichage Création
                bool showCreative = webSession.CustomerLogin.ShowCreatives(vehicle);
                // Affichage Agences Medias
                bool showMediaAgency = false;
                if (webSession.CustomerLogin.CustormerFlagAccess(TNS.AdExpress.Constantes.DB.Flags.ID_MEDIA_AGENCY) && dt.Columns.Contains("advertising_agency")) {
                    showMediaAgency = true;
                }
                //Affichage date Diffusion
                bool showDate = true;
                if (!allPeriod && (vehicle == DBClassificationConstantes.Vehicles.names.press || vehicle == DBClassificationConstantes.Vehicles.names.internationalPress))
                    showDate = false;
                #endregion
                
                #region Nombre de lignes du tableau du tableau
                iNbLine = dt.Rows.Count;
                #endregion

                #region Initialisation du tableau de résultats
                try {
                    headers = new Headers();
                    columnItemList = PortofolioDetailMediaColumnsInformation.GetDefaultMediaDetailColumns(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID);

                    foreach (GenericColumnItemInformation Column in columnItemList) {

                        switch (Column.Id) {
                            case GenericColumnItemInformation.Columns.associatedFile://Visuel radio/tv
                            case GenericColumnItemInformation.Columns.visual://Visuel presse
                                if (showCreative)
                                    headers.Root.Add(new HeaderCreative(false, GestionWeb.GetWebWord(Column.WebTextId, webSession.SiteLanguage), Column.WebTextId));
                                break;
                            case GenericColumnItemInformation.Columns.agenceMedia://Agence Media
                                if (showMediaAgency)
                                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(Column.WebTextId, webSession.SiteLanguage), Column.WebTextId));
                                break;
                            case GenericColumnItemInformation.Columns.planMedia://Plan media
                                headers.Root.Add(new HeaderMediaSchedule(false, GestionWeb.GetWebWord(Column.WebTextId, webSession.SiteLanguage), Column.WebTextId));
                                break;
                            case GenericColumnItemInformation.Columns.dateDiffusion:
                            case GenericColumnItemInformation.Columns.dateParution:
                                if (showDate)
                                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(Column.WebTextId, webSession.SiteLanguage), Column.WebTextId));
                                break;
                            case GenericColumnItemInformation.Columns.topDiffusion:
                                if (!isDigitalTV)
                                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(Column.WebTextId, webSession.SiteLanguage), Column.WebTextId));
                                break;
                            default:
                                if(Column.Visible)
                                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(Column.WebTextId, webSession.SiteLanguage), Column.WebTextId));
                                break;
                        }

                    }

                    tab = new ResultTable(iNbLine, headers);
                }
                catch (System.Exception err) { 
                    throw (new PortofolioDetailMediaException("Erreur au moment de l'initialisation des headers pour le detail media du portefeuille",err));
                }
                #endregion

                #region Generation du tableau
                string[] files;
                string listVisual = "";
                long iCurColumn=0;
                Cell curCell = null;
                string date = "";
                string dateMediaNum = "";
                DateTime dateMedia;

                try {

                    // Chargement de l'assemblage
                    assembly = Assembly.Load(@"TNS.FrameWork.WebResultUI");

                    foreach (DataRow row in dt.Rows) {

                        #region Initialisation de dateMediaNum
                        switch (vehicle) {
                            case DBClassificationConstantes.Vehicles.names.press:
                            case DBClassificationConstantes.Vehicles.names.internationalPress:
                                dateMediaNum = row["date_media_num"].ToString();
                                break;
                            case DBClassificationConstantes.Vehicles.names.tv:
                            case DBClassificationConstantes.Vehicles.names.radio:
                            case DBClassificationConstantes.Vehicles.names.others:
                                dateMedia = new DateTime(int.Parse(row["date_media_num"].ToString().Substring(0, 4)), int.Parse(row["date_media_num"].ToString().Substring(4, 2)), int.Parse(row["date_media_num"].ToString().Substring(6, 2)));
                                dateMediaNum = dateMedia.DayOfWeek.ToString();
                                break;
                        }
                        #endregion

                        if (dayOfWeek == dateMediaNum || allPeriod) {

                            tab.AddNewLine(LineType.level1);
                            iCurColumn = 1;

                            foreach (GenericColumnItemInformation Column in columnItemList) {
                                switch (Column.Id) {
                                    case GenericColumnItemInformation.Columns.visual://Visuel presse
                                        if (showCreative) {
                                            if (row[Column.DataBaseField].ToString().Length > 0) {
                                                // Création
                                                files = row[Column.DataBaseField].ToString().Split(',');
                                                foreach (string str in files) {
                                                    listVisual += "/ImagesPresse/" + idMedia + "/" + row["date_cover_num"] + "/" + str + ",";
                                                }
                                                if (listVisual.Length > 0) {
                                                    listVisual = listVisual.Substring(0, listVisual.Length - 1);
                                                }
                                                tab[iCurLine, iCurColumn++] = new CellPressCreativeLink(listVisual, webSession);
                                                listVisual = "";
                                            }
                                            else
                                                tab[iCurLine, iCurColumn++] = new CellPressCreativeLink("", webSession);
                                        }
                                        break;
                                    case GenericColumnItemInformation.Columns.associatedFile://Visuel radio/tv
                                        if (showCreative) {
                                            switch (vehicle) {
                                                case DBClassificationConstantes.Vehicles.names.radio:
                                                    tab[iCurLine, iCurColumn++] = new CellRadioCreativeLink(row[Column.DataBaseField].ToString(), webSession);
                                                    break;
                                                case DBClassificationConstantes.Vehicles.names.tv:
                                                case DBClassificationConstantes.Vehicles.names.others:
                                                    if (row[Column.DataBaseField].ToString().Length > 0)
                                                        tab[iCurLine, iCurColumn++] = new CellTvCreativeLink(Convert.ToInt64(row[Column.DataBaseField]), webSession, vehicle.GetHashCode());
                                                    else
                                                        tab[iCurLine, iCurColumn++] = new CellTvCreativeLink(-1, webSession, vehicle.GetHashCode());

                                                    break;
                                            }
                                        }
                                        break;
                                    case GenericColumnItemInformation.Columns.agenceMedia://Agence Media
                                        if (showMediaAgency) {
                                            tab[iCurLine, iCurColumn++] = new CellLabel(row["advertising_agency"].ToString());
                                        }
                                        break;
                                    case GenericColumnItemInformation.Columns.planMedia://Plan media
                                        tab[iCurLine, iCurColumn++] = new CellInsertionMediaScheduleLink(webSession, Convert.ToInt64(row["id_product"]), 1);
                                        break;
                                    case GenericColumnItemInformation.Columns.dateParution://Date Parution and Date diffusion
                                    case GenericColumnItemInformation.Columns.dateDiffusion:
                                        if (showDate) {
                                            type = assembly.GetType(Column.CellType);
                                            curCell = (Cell)type.InvokeMember("GetInstance", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, null);
                                            date = row[Column.DataBaseField].ToString();
                                            if (date.Length > 0)
                                                curCell.SetCellValue((object)new DateTime(int.Parse(date.Substring(0, 4)), int.Parse(date.Substring(4, 2)), int.Parse(date.Substring(6, 2))));
                                            else
                                                curCell.SetCellValue(null);
                                            tab[iCurLine, iCurColumn++] = curCell;
                                        }
                                        break;
                                    case GenericColumnItemInformation.Columns.topDiffusion:
                                    case GenericColumnItemInformation.Columns.idTopDiffusion:
                                        if (!isDigitalTV)
                                        {
                                            if (row[Column.DataBaseField].ToString().Length > 0)
                                                tab[iCurLine, iCurColumn++] = new TNS.FrameWork.WebResultUI.CellAiredTime(Convert.ToDouble(row[Column.DataBaseField]));
                                            else
                                                tab[iCurLine, iCurColumn++] = new TNS.FrameWork.WebResultUI.CellAiredTime(0);
                                        }
                                        break;
                                    default:
                                        if (Column.Visible) {
                                            type = assembly.GetType(Column.CellType);
                                            curCell = (Cell)type.InvokeMember("GetInstance", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, null);
                                            curCell.SetCellValue(row[Column.DataBaseField]);
                                            tab[iCurLine, iCurColumn++] = curCell;
                                        }
                                        break;
                                }
                            }
                            iCurLine++;
                        }

                    }
                }
                catch(System.Exception err){
                    throw (new PortofolioDetailMediaException("Erreur lors de la création du tableResult pour le détail Media du portefeuille", err));
                }
                #endregion

            }

            return tab;
        }
        #endregion

        #region  Structure

        #region Tableau de résultats pour radio
        /// <summary>
		/// Obtient le tableau de donnée des informations synthétiques (structure)
		/// pour la radio
		/// </summary>
		/// <param name="webSession">session du client</param>		
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <returns>tableau de données</returns>
		public static object[,] GetStructRadioTab(WebSession webSession,int dateBegin,int dateEnd){
			
			#region Variables
			object[,] tab = null;
			double totalEuros=0;
			double totalSpot=0;
			double totalDuration=0;
			DataSet ds5_7=null;
			DataSet ds7_9=null;
			DataSet ds9_13=null;
			DataSet ds13_19=null;
			DataSet ds19_24=null;
			int nbLine=1;
			int lineIndex=1;
			#endregion

			#region Construction du tab
			//Radio	
			//5h-7h
			ds5_7 = PortofolioDataAccess.GetTvOrRadioStructData(webSession,dateBegin ,dateEnd,50000,70000);	
			if(ds5_7!=null && ds5_7.Tables[0].Rows.Count==1 && IsRowNull(ds5_7))nbLine++;			
			//7h-9h
			ds7_9 = PortofolioDataAccess.GetTvOrRadioStructData(webSession,dateBegin,dateEnd,70000,90000);
			if(ds7_9!=null && ds7_9.Tables[0].Rows.Count==1 && IsRowNull(ds7_9))nbLine++;
			
			//9h-13h
			ds9_13 = PortofolioDataAccess.GetTvOrRadioStructData(webSession,dateBegin,dateEnd,90000,130000);
			if(ds9_13!=null && ds9_13.Tables[0].Rows.Count==1 && IsRowNull(ds9_13))nbLine++;
			
			//13h-19h			
			ds13_19 = PortofolioDataAccess.GetTvOrRadioStructData(webSession,dateBegin,dateEnd,130000,190000);
			if(ds13_19!=null && ds13_19.Tables[0].Rows.Count==1 && IsRowNull(ds13_19))nbLine++;
			//19h-24h
			 ds19_24 = PortofolioDataAccess.GetTvOrRadioStructData(webSession,dateBegin,dateEnd,190000,240000);
			if(ds19_24!=null && ds9_13.Tables[0].Rows.Count==1 && IsRowNull(ds19_24))nbLine++;
			
			if(nbLine>1){
				tab = new object[nbLine,FrameWorkResultConstantes.PortofolioStructure.RADIO_TV_NB_MAX_COLUMNS];
						
				if(ds5_7!=null && ds5_7.Tables[0].Rows.Count==1 && IsRowNull(ds5_7)){
					FillTabEuroSpotDuration(ref tab,ds5_7,lineIndex,ref totalEuros,ref totalSpot,ref totalDuration,"5H - 7H");				
					lineIndex++;
				}
				if(ds7_9!=null && ds7_9.Tables[0].Rows.Count==1 && IsRowNull(ds7_9)){
					FillTabEuroSpotDuration(ref tab, ds7_9,lineIndex,ref totalEuros,ref totalSpot,ref totalDuration,"7H - 9H");
					lineIndex++;
				}
				if(ds9_13!=null && ds9_13.Tables[0].Rows.Count==1 && IsRowNull(ds9_13)){
					FillTabEuroSpotDuration(ref tab, ds9_13,lineIndex,ref totalEuros,ref totalSpot,ref totalDuration,"9H - 13H");
					lineIndex++;
				}
				if(ds13_19!=null && ds13_19.Tables[0].Rows.Count==1){
					FillTabEuroSpotDuration(ref tab,ds13_19,lineIndex,ref totalEuros,ref totalSpot,ref totalDuration,"13H - 19H");
					lineIndex++;
				}
				if(ds19_24!=null && ds9_13.Tables[0].Rows.Count==1 && IsRowNull(ds19_24)){
					FillTabEuroSpotDuration(ref tab,ds19_24,lineIndex,ref totalEuros,ref totalSpot,ref totalDuration,"19H - 24H");
					lineIndex++;
				}
			
			
				//total
				if(tab!=null && tab.GetLength(0)!=0){
					tab[0,FrameWorkResultConstantes.PortofolioStructure.MEDIA_HOURS_COLUMN_INDEX]=GestionWeb.GetWebWord(1401,webSession.SiteLanguage).ToString();
					tab[0,FrameWorkResultConstantes.PortofolioStructure.EUROS_COLUMN_INDEX]=totalEuros.ToString();
					tab[0,FrameWorkResultConstantes.PortofolioStructure.SPOT_COLUMN_INDEX]=totalSpot.ToString();
					tab[0,FrameWorkResultConstantes.PortofolioStructure.DURATION_COLUMN_INDEX]=totalDuration.ToString();
				}
			}
					
			
			#endregion

			return tab;
		}
		#endregion

		#region Tableau de résultats pour télé
		/// <summary>
		/// Obtient le tableau de donnée des informations synthétiques (structure)
		/// pour la TV
		/// </summary>
		/// <param name="webSession">session du client</param>		
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <returns>tableau de données</returns>
		public static object[,] GetStructTVTab(WebSession webSession,int dateBegin,int dateEnd){
			
			#region Variables
			object[,] tab = null;
			double totalEuros=0;
			double totalSpot=0;
			double totalDuration=0;
			DataSet ds7_9=null;
			DataSet ds12_14=null;
			DataSet ds14_17=null;
			DataSet ds17_19=null;
			DataSet ds19_22=null;
			DataSet ds22_24=null;
			DataSet ds0_7=null;
			int nbLine=1;
			int lineIndex=1;
			#endregion
			
			#region Construction du tab
			//Radio	
			//7h-12h
			ds7_9 = PortofolioDataAccess.GetTvOrRadioStructData(webSession,dateBegin,dateEnd,70000,120000);				
			if(ds7_9!=null && ds7_9.Tables[0].Rows.Count==1 && IsRowNull(ds7_9))nbLine++;				
			//12h-14h
			ds12_14= PortofolioDataAccess.GetTvOrRadioStructData(webSession,dateBegin,dateEnd,120000,140000);
			if(ds12_14!=null && ds12_14.Tables[0].Rows.Count==1 && IsRowNull(ds12_14))nbLine++;			
			//14h-17h
			ds14_17 = PortofolioDataAccess.GetTvOrRadioStructData(webSession,dateBegin,dateEnd,140000,170000);
			if(ds14_17!=null && ds14_17.Tables[0].Rows.Count==1 && IsRowNull(ds14_17))nbLine++;			
			//17h-19h
			ds17_19 = PortofolioDataAccess.GetTvOrRadioStructData(webSession,dateBegin,dateEnd,170000,190000);
			if(ds17_19!=null && ds17_19.Tables[0].Rows.Count==1 && IsRowNull(ds17_19))nbLine++;			
			//19h-22h
			ds19_22 = PortofolioDataAccess.GetTvOrRadioStructData(webSession,dateBegin,dateEnd,190000,220000);
			if(ds19_22!=null && ds19_22.Tables[0].Rows.Count==1 && IsRowNull(ds19_22))nbLine++;
			//22h-24h
			ds22_24 = PortofolioDataAccess.GetTvOrRadioStructData(webSession,dateBegin,dateEnd,220000,240000);
			if(ds22_24!=null && ds22_24.Tables[0].Rows.Count==1 && IsRowNull(ds22_24))nbLine++;
			//0h-7h
			ds0_7 = PortofolioDataAccess.GetTvOrRadioStructData(webSession,dateBegin,dateEnd,0,70000);
			if(ds0_7!=null && ds0_7.Tables[0].Rows.Count==1 && IsRowNull(ds0_7))nbLine++;
			
			if(nbLine>1){
				tab = new object[nbLine,FrameWorkResultConstantes.PortofolioStructure.RADIO_TV_NB_MAX_COLUMNS];

				if(ds7_9!=null && ds7_9.Tables[0].Rows.Count==1 && IsRowNull(ds7_9)){
					FillTabEuroSpotDuration(ref tab,ds7_9,lineIndex,ref totalEuros,ref totalSpot,ref totalDuration," 7 H - 12 H ");
					lineIndex++;
				}
				if(ds12_14!=null && ds12_14.Tables[0].Rows.Count==1 && IsRowNull(ds12_14)){
					FillTabEuroSpotDuration(ref tab,ds12_14,lineIndex,ref totalEuros,ref totalSpot,ref totalDuration," 12 H - 14 H");
					lineIndex++;
				}
				if(ds14_17!=null && ds14_17.Tables[0].Rows.Count==1 && IsRowNull(ds14_17)){
					FillTabEuroSpotDuration(ref tab,ds14_17,lineIndex,ref totalEuros,ref totalSpot,ref totalDuration," 14 H - 17 H");
					lineIndex++;
				}
				if(ds17_19!=null && ds17_19.Tables[0].Rows.Count==1 && IsRowNull(ds17_19)){
					FillTabEuroSpotDuration(ref tab,ds17_19,lineIndex,ref totalEuros,ref totalSpot,ref totalDuration," 17 H - 19 H");
					lineIndex++;
				}
				if(ds19_22!=null && ds19_22.Tables[0].Rows.Count==1 && IsRowNull(ds19_22)){
					FillTabEuroSpotDuration(ref tab,ds19_22,lineIndex,ref totalEuros,ref totalSpot,ref totalDuration," 19 H - 22 H ");
					lineIndex++;
				}
				if(ds22_24!=null && ds22_24.Tables[0].Rows.Count==1 && IsRowNull(ds22_24)){
					FillTabEuroSpotDuration(ref tab,ds22_24,lineIndex,ref totalEuros,ref totalSpot,ref totalDuration," 22 H - 24 H ");
					lineIndex++;
				}
				if(ds0_7!=null && ds0_7.Tables[0].Rows.Count==1 && IsRowNull(ds0_7)){
					FillTabEuroSpotDuration(ref tab,ds0_7,lineIndex,ref totalEuros,ref totalSpot,ref totalDuration," 0 H - 7 H ");
					lineIndex++;
				}
			
				//total
				if(tab!=null && tab.GetLength(0)!=0){
					tab[0,FrameWorkResultConstantes.PortofolioStructure.MEDIA_HOURS_COLUMN_INDEX]=GestionWeb.GetWebWord(1401,webSession.SiteLanguage).ToString();
					tab[0,FrameWorkResultConstantes.PortofolioStructure.EUROS_COLUMN_INDEX]=totalEuros.ToString();
					tab[0,FrameWorkResultConstantes.PortofolioStructure.SPOT_COLUMN_INDEX]=totalSpot.ToString();
					tab[0,FrameWorkResultConstantes.PortofolioStructure.DURATION_COLUMN_INDEX]=totalDuration.ToString();
				}
			}
								
			#endregion

			return tab;
		}
		#endregion

		#region Tableau de résultats pour la presse
		/// <summary>
		///Obtient le tableau de donnée des informations synthétiques (structure)
		///pour la presse 
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="dtFormat">table de données pour le format</param>
		/// <param name="dtColor">table de données pour la couleur</param>
		/// <param name="dtLocation">table de données pour les emplacements</param>
		/// <param name="dtInsert">table de données pour les encarts</param>
		public static void GetStructPressTab(WebSession webSession, int dateBegin,int dateEnd,out DataTable dtFormat, out DataTable dtColor, out DataTable dtLocation,out DataTable dtInsert){									
			
			#region Variables
			DataSet ds=null;
			dtFormat=null;
			dtColor=null;
			dtInsert =null;
			dtLocation=null;
			#endregion

			#region Construction du tableau de résultats
			//Presse
			//Format
			ds = PortofolioDataAccess.GetPressStructData(webSession,dateBegin,dateEnd,FrameWorkResultConstantes.PortofolioStructure.Ventilation.format);
			if(ds!=null && ds.Tables[0]!=null && ds.Tables[0].Rows.Count>0)
				dtFormat = ds.Tables[0];
			//Couleur
			ds = PortofolioDataAccess.GetPressStructData(webSession,dateBegin,dateEnd,FrameWorkResultConstantes.PortofolioStructure.Ventilation.color);
			if(ds!=null && ds.Tables[0]!=null && ds.Tables[0].Rows.Count>0)
				dtColor = ds.Tables[0];
			//Emplacements
			ds = PortofolioDataAccess.GetPressStructData(webSession,dateBegin,dateEnd,FrameWorkResultConstantes.PortofolioStructure.Ventilation.location);
			if(ds!=null && ds.Tables[0]!=null && ds.Tables[0].Rows.Count>0)
				dtLocation = ds.Tables[0];
			//Encarts
			ds = PortofolioDataAccess.GetPressStructData(webSession,dateBegin,dateEnd,FrameWorkResultConstantes.PortofolioStructure.Ventilation.insert);
			if(ds!=null && ds.Tables[0]!=null && ds.Tables[0].Rows.Count>0)
				dtInsert = ds.Tables[0];	
			#endregion

		}
		/// <summary>
		/// Vérifie qu'une datarow est vide
		/// </summary>
		/// <param name="ds">dataset</param>
		/// <returns>vrai si non vide</returns>
		private static bool IsRowNull(DataSet ds){
			if(ds!=null && ds.Tables[0].Rows.Count>0){
				foreach(DataRow dr in ds.Tables[0].Rows){
					return(dr["euros"]!=System.DBNull.Value && dr["spot"]!=System.DBNull.Value && dr["duration"]!=System.DBNull.Value );					
				}								
			}
			 return false;
		}
		#endregion
				
		#endregion		

		#region Euros, spot et durée pour structure
		/// <summary>
		/// Remplit le tableau de résultats avec euros,spot et durée.
		/// </summary>
		/// <param name="tab">tableau de résultats</param>
		/// <param name="ds">groupe de données</param>
		/// <param name="lineIndex">line courante du tableau de résultat</param>
		/// <param name="totalEuros">total euros</param>
		/// <param name="totalSpot">total spot</param>
		/// <param name="totalDuration">total durée</param>
		/// <param name="timeSpan">timeSpan</param>
		private static void FillTabEuroSpotDuration(ref object[,] tab,DataSet ds,int lineIndex,ref double totalEuros, ref double totalSpot,ref double totalDuration,string timeSpan){
			DataRow dr=null;
			if(tab!=null && tab.GetLength(0)>0 && ds!=null && ds.Tables[0]!=null && ds.Tables[0].Rows.Count==1){
				dr=ds.Tables[0].Rows[0];
				//euros
				if(dr["euros"]!=System.DBNull.Value){
					tab[lineIndex,FrameWorkResultConstantes.PortofolioStructure.EUROS_COLUMN_INDEX]=dr["euros"].ToString();
					totalEuros+=double.Parse(dr["euros"].ToString());
				}
				//spot
				if(dr["spot"]!=System.DBNull.Value){
					tab[lineIndex,FrameWorkResultConstantes.PortofolioStructure.SPOT_COLUMN_INDEX]=dr["spot"].ToString();
					totalSpot+=double.Parse(dr["spot"].ToString());
				}
				//durée
				if(dr["duration"]!=System.DBNull.Value){
					tab[lineIndex,FrameWorkResultConstantes.PortofolioStructure.DURATION_COLUMN_INDEX]=dr["duration"].ToString();
					totalDuration+=double.Parse(dr["duration"].ToString());
				}
				//intervalle horaire
				if(((dr["euros"]!=System.DBNull.Value) || (dr["spot"]!=System.DBNull.Value) || (dr["duration"]!=System.DBNull.Value)) && WebFunctions.CheckedText.IsStringEmpty(timeSpan.ToString().Trim()))
					tab[lineIndex,FrameWorkResultConstantes.PortofolioStructure.MEDIA_HOURS_COLUMN_INDEX]=timeSpan.ToString();
			}
		}
		#endregion

		#region Formatage des dates
		/// <summary>
		/// Obtient la date de début de l'analyse en fonction du module
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Date de début</returns>
		internal static string GetDateBegin(WebSession webSession){
			switch(webSession.CurrentModule){
				case WebConstantes.Module.Name.ALERTE_PORTEFEUILLE:			
					return(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
				case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:			
					return(webSession.PeriodBeginningDate);
			}
			return(null);
		}

		/// <summary>
		/// Obtient la date de fin de l'analyse en fonction du module
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Date de fin</returns>
		internal static string GetDateEnd(WebSession webSession){
			switch(webSession.CurrentModule){
				case WebConstantes.Module.Name.ALERTE_PORTEFEUILLE:		
					return(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
				case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
					return(webSession.PeriodEndDate);
			}
			return(null);
		}
		#endregion

        #region DataTable pour la Presse et la Presse interntional
        /// <summary>
        /// Adapte le tadaTable pour la Presse et la Presse interntional
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns>DataTable</returns>
        public static void SetDataTable(DataTable dt, string dayOfWeek, bool allPeriod) {

            #region Variables
            Int64 idOldLine = -1;
            Int64 idLine = -1;
            DataRow oldRow = null;
            int iLine = 0;
            ArrayList indexLines = new ArrayList();
            #endregion

            if (dt != null && dt.Rows != null && dt.Rows.Count > 0) {

                #region Parcours du tableau
                foreach (DataRow row in dt.Rows) {

                    if (dayOfWeek == row["date_media_num"].ToString() || allPeriod) {

                        idLine = (long)row["id_advertisement"];

                        if (idLine != idOldLine) {
                            idOldLine = idLine;
                            oldRow = row;
                        }
                        else {
                            if(oldRow["location"].ToString().Length>0 && row["location"].ToString().Length>0)
                                oldRow["location"] = oldRow["location"].ToString() + "-" + row["location"].ToString();
                            else if (oldRow["location"].ToString().Length == 0 && row["location"].ToString().Length > 0)
                                oldRow["location"] = row["location"].ToString();
                            indexLines.Add(iLine);
                        }
                    }

                    iLine++;
                }
                #endregion

                indexLines.Reverse();
                //suppression des lignes
                foreach (int index in indexLines)
                    dt.Rows.Remove(dt.Rows[index]);

            }
        }
        #endregion

    }
}
