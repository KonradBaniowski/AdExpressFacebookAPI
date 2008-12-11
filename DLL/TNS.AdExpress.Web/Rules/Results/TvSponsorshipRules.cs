#region Informations
// Auteur: D. Mussuma 
// Date de cr�ation: 01/12/2006 
// Date de modification:
//      21/08/2007  G. Facon        Ajout Colonne version et insertion
#endregion

using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Globalization;

using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.DataAccess.Results;
using WebCommon=TNS.AdExpress.Web.Common;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebException=TNS.AdExpress.Web.Exceptions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using CustomerConstantes=TNS.AdExpress.Constantes.Customer;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using ClassificationDB=TNS.AdExpress.DataAccess.Classification;

using TNS.FrameWork;
using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.Date;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Units;

namespace TNS.AdExpress.Web.Rules.Results
{
	/// <summary>
	/// Classe des r�gles m�tiers du Parrainage t�l�
	/// </summary>
	public class TvSponsorshipRules {

		#region GetData
		/// <summary>
		/// Obtient le tableau contenant l'ensemble des r�sultats du parrainage t�l�
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="beginningPeriod">Date de d�but</param>
		/// <param name="endPeriod">Date de fin</param>
		/// <returns>Tableau de r�sultat</returns>
		public static ResultTable GetData(WebSession webSession, string beginningPeriod, string endPeriod){ 
			
			#region Variables
			
			//			long currentLine;
			//			long currentColumn;
			//			long currentLineInTabResult;
//			long nbLineInTabResult=-1;			
			#endregion

			#region Tableaux d'index
			long nbCol=0;
			Hashtable dimensionColIndex=new Hashtable();
			string dimensionListForLabelSearch="";
			int maxIndex=0;
			InitIndexAndValues(webSession,dimensionColIndex,ref nbCol,beginningPeriod,endPeriod,ref maxIndex,ref dimensionListForLabelSearch);
			#endregion

			#region Chargement du tableau
			long nbLineInNewTable=0;
			object[,] tabData=GetPreformatedTable(webSession,dimensionColIndex,maxIndex,ref nbLineInNewTable, beginningPeriod, endPeriod);
			#endregion

			if(tabData==null){
				return null;
			}

			return (GetResultTable(webSession,tabData,nbLineInNewTable,dimensionColIndex,dimensionListForLabelSearch));
		}
		#endregion

		#region M�thodes internes

		#region Tableau Pr�format�
		/// <summary>
		/// Obtient le tableau de r�sultat pr�format� pour le parrainage t�l�
		/// </summary>
		/// <param name="webSession">Session du client</param>	
		/// <param name="dimensionColIndex">Liste des index des dimensions en colonnes du tableau</param>
		/// <param name="nbCol">Nombre de colonnes du tableau de r�sultat</param>
		/// <param name="nbLineInNewTable">(out) Nombre de ligne du tableau de r�sultat</param>
		/// <param name="periodBeginning">Date de d�but</param>
		/// <param name="periodEnd">Date de fin</param>
		/// <returns>Tableau de r�sultat</returns>
		private static object[,] GetPreformatedTable(WebSession webSession,Hashtable dimensionColIndex,long nbCol,ref long nbLineInNewTable,string periodBeginning,string  periodEnd){
			
			#region Variables
			DataSet ds=null;
			#endregion
			

			#region Chargement des donn�es � partir de la base	
									
			ds=TvSponsorshipDataAccess.GetData(webSession, periodBeginning, periodEnd);					
			DataTable dt=ds.Tables[0];

			if(dt.Rows.Count==0){
				return null;
			}

			#endregion

			#region D�claration du tableau de r�sultat
			long nbline=dt.Rows.Count;
			object[,] tabResult=new object[nbline,nbCol];
			#endregion

			#region Tableau de r�sultat

			SetMediaDetailLevelValues(webSession,dt,tabResult,ref nbLineInNewTable,dimensionColIndex,nbCol);
			#endregion


			return(tabResult);
		}
		#endregion

		#region Formattage d'un tableau de r�sultat
		/// <summary>
		/// Formattage d'un tableau de r�sultat � partir d'un tableau de donn�es
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="tabData">Table de donn�es</param>
		/// <param name="nbLineInTabData">Nombre de ligne dans le tableau</param>
        /// <param name="dimensionColIndex">Column Index of dimension</param>
        /// <param name="dimensionListForLabelSearch">Dimension List for Label Searching</param>
		/// <returns>Tableau de r�sultat</returns>
		private static ResultTable GetResultTable(WebSession webSession,object[,] tabData,long nbLineInTabData,Hashtable dimensionColIndex,string dimensionListForLabelSearch){
			
			#region Variables		
			Int64 oldIdL1=-1;
			Int64 oldIdL2=-1;
			Int64 oldIdL3=-1;
			Int64 oldIdL4=-1;
			long currentLine;
			long currentLineInTabResult;			
			long k;
			ResultTable resultTable = null;
			CellUnitFactory cellUnitFactory =null,cellKeuroFactory = null,cellEuroFactory = null,cellDurationFactory = null,cellInsertionFactory = null;
            CellUnit cellKeuro = null, cellEuro=null, cellDuration = null, cellInsertion = null;
			#endregion

			#region Calcul des pourcentages
			bool computePercentage=false;
			if(webSession.PercentageAlignment!=WebConstantes.Percentage.Alignment.none)
				computePercentage = true;

			#endregion

            #region Vahicle Informations
            string vehicleSelection = webSession.GetSelection(webSession.SelectionUniversMedia,CustomerConstantes.Right.type.vehicleAccess);
            if(vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new System.Exception("Media Selection is not valid"));
            VehicleInformation vehicleInformation=VehiclesInformation.Get(Int64.Parse(vehicleSelection));
            Vehicles.names vehicle =vehicleInformation.Id;
            #endregion


            #region Headers

            Headers headers=new Headers();
			// Ajout de la colonne des libell�s des Autres dimensions
			if(webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES )
				headers.Root.Add(new Header(true,GestionWeb.GetWebWord(1164,webSession.SiteLanguage),FrameWorkResultConstantes.TvSponsorship.LEVEL_HEADER_ID));
			else if	(webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS )
				headers.Root.Add(new Header(true,GestionWeb.GetWebWord(804,webSession.SiteLanguage),FrameWorkResultConstantes.TvSponsorship.LEVEL_HEADER_ID));
			long startDataColIndex=1;
			long startDataColIndexInit=1;

			// Ajout Cr�ation 
			bool showCreative=false;
			//A v�rifier Cr�ation o� version
            if (vehicleInformation.ShowCreations &&
                (webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG) &&
                (webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES &&
                (webSession.GenericMediaDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser) ||
                webSession.GenericMediaDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.product)) ||
                webSession.CurrentModule != WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES))
                ) {
                headers.Root.Add(new HeaderCreative(false, GestionWeb.GetWebWord(1994, webSession.SiteLanguage), FrameWorkResultConstantes.TvSponsorship.CREATIVE_HEADER_ID));
                showCreative = true;
                startDataColIndex++;
                startDataColIndexInit++;
            }
            bool showInsertions = false;
            if(vehicleInformation.ShowInsertions &&
                ((webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES &&
                (webSession.GenericMediaDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser) ||
                webSession.GenericMediaDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.product))) ||
                webSession.CurrentModule != WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES
                )){
                headers.Root.Add(new HeaderInsertions(false, GestionWeb.GetWebWord(2245, webSession.SiteLanguage), FrameWorkResultConstantes.TvSponsorship.INSERTIONS_HEADER_ID));
                startDataColIndex++;
                startDataColIndexInit++;
                showInsertions = true;
            }

			//Colonne total 			
			bool showTotal=false;
			if(webSession.PreformatedTable!=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units ){
				startDataColIndexInit++;
				showTotal=true;
				headers.Root.Add(new Header(true,GestionWeb.GetWebWord(805,webSession.SiteLanguage),FrameWorkResultConstantes.TvSponsorship.TOTAL_HEADER_ID));
			}
		
			// Chargement des libell�s de colonnes
			SetColumnsLabels(webSession,headers,dimensionColIndex,dimensionListForLabelSearch);
			#endregion

			#region D�claration du tableau de r�sultat
			
			long nbLine=GetNbLineFromPreformatedTableToResultTable(tabData)+1;
		
			 resultTable=new ResultTable(nbLine,headers);
			long nbCol=resultTable.ColumnsNumber-2;
			long totalColIndex=-1;
			if(showTotal)totalColIndex=resultTable.GetHeadersIndexInResultTable(FrameWorkResultConstantes.TvSponsorship.TOTAL_HEADER_ID.ToString());
			long levelLabelColIndex=resultTable.GetHeadersIndexInResultTable(FrameWorkResultConstantes.TvSponsorship.LEVEL_HEADER_ID.ToString());
			long creativeColIndex=resultTable.GetHeadersIndexInResultTable(FrameWorkResultConstantes.TvSponsorship.CREATIVE_HEADER_ID.ToString());
            long insertionsColIndex = resultTable.GetHeadersIndexInResultTable(FrameWorkResultConstantes.TvSponsorship.INSERTIONS_HEADER_ID.ToString());
			#endregion

			#region S�lection de l'unit�
			switch(webSession.PreformatedTable){
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Media :
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Period :
				cellUnitFactory = webSession.GetCellUnitFactory();
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units :
                    cellKeuro = new CellKEuro(0.0);
                    cellKeuro.StringFormat = UnitsInformation.Get(CustomerSessions.Unit.kEuro).StringFormat;
                    cellEuro = new CellEuro(0.0);
                    cellEuro.StringFormat = UnitsInformation.Get(CustomerSessions.Unit.euro).StringFormat;
                    cellDuration = new CellDuration(0.0);
                    cellDuration.StringFormat = UnitsInformation.Get(CustomerSessions.Unit.duration).StringFormat;
                    cellInsertion = new CellInsertion(0.0);
                    cellInsertion.StringFormat = UnitsInformation.Get(CustomerSessions.Unit.insertion).StringFormat;

                    cellKeuroFactory = new CellUnitFactory(cellKeuro);
                    cellEuroFactory = new CellUnitFactory(cellEuro);
                    cellDurationFactory = new CellUnitFactory(cellDuration);
                    cellInsertionFactory = new CellUnitFactory(cellInsertion);
					break;
			}
			#endregion

			#region Total

			TNS.AdExpress.Domain.Level.GenericDetailLevel genericDetailLevel = webSession.GenericMediaDetailLevel;
			
			long nbColInTabData=tabData.GetLength(1);
			startDataColIndex++;
			startDataColIndexInit++;
			currentLineInTabResult = resultTable.AddNewLine(TNS.FrameWork.WebResultUI.LineType.total);
			//Libell� du total
			resultTable[currentLineInTabResult,levelLabelColIndex]=new CellLevel(-1,GestionWeb.GetWebWord(805,webSession.SiteLanguage),0,currentLineInTabResult);
			CellLevel currentCellLevel0=(CellLevel)resultTable[currentLineInTabResult,levelLabelColIndex];
			if(showCreative)resultTable[currentLineInTabResult,creativeColIndex]= new CellSponsorshipCreativesLink(currentCellLevel0,webSession,genericDetailLevel);
            if (showInsertions) resultTable[currentLineInTabResult, insertionsColIndex] = new CellSponsorshipInsertionsLink(currentCellLevel0, webSession, genericDetailLevel);
			if(webSession.PreformatedTable!=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units ){//Pas de colonnes total pour le tableau Autres dimensions X unit�s 
				if(showTotal && !computePercentage)resultTable[currentLineInTabResult,totalColIndex]=cellUnitFactory.Get(0.0);
				if(showTotal && computePercentage){					
						resultTable[currentLineInTabResult,totalColIndex]=new CellPercent(0.0,null);					
				}
			}
			
			for(k=startDataColIndexInit;k<=nbCol;k++){
				if(computePercentage){
					if(webSession.PercentageAlignment==WebConstantes.Percentage.Alignment.horizontal)
						resultTable[currentLineInTabResult,k]=new CellPercent(0.0,(CellUnit)resultTable[currentLineInTabResult,totalColIndex]);
					else if(webSession.PercentageAlignment==WebConstantes.Percentage.Alignment.vertical){
						resultTable[currentLineInTabResult,k] = new CellPercent(0.0,null);						
					}

				}
				else{
					if(webSession.PreformatedTable!=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units )
						resultTable[currentLineInTabResult,k]=cellUnitFactory.Get(0.0);
					else if (webSession.PreformatedTable==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units ){
						
						//Keuro
						resultTable[currentLineInTabResult,k]=cellKeuroFactory.Get(0.0);

						//Euro
						k++;
						resultTable[currentLineInTabResult,k]=cellEuroFactory.Get(0.0);

						//Dur�e
						k++;
						resultTable[currentLineInTabResult,k]=cellDurationFactory.Get(0.0);

						//Spots
						k++;
						resultTable[currentLineInTabResult,k]=cellInsertionFactory.Get(0.0);

						break;
					}
				}
			}

			#endregion

			
			#region Tableau de r�sultat
			oldIdL1 = -1;
			oldIdL2 = -1;
			oldIdL3 = -1;
			oldIdL4 = -1;
			AdExpressCellLevel currentCellLevel1=null;
			AdExpressCellLevel currentCellLevel2=null;
			AdExpressCellLevel currentCellLevel3=null;
			AdExpressCellLevel currentCellLevel4=null;
			long currentL1Index=-1;
			long currentL2Index=-1;
			long currentL3Index=-1;
			long currentL4Index=-1;

			for(currentLine=0;currentLine<nbLineInTabData;currentLine++){

				#region On change de niveau L1
				if(tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.IDL1_INDEX]!=null && (Int64)tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.IDL1_INDEX]!=oldIdL1){
					currentLineInTabResult =  resultTable.AddNewLine(LineType.level1);
					
					#region Totaux et autres dimensions en colonnnes � 0 
					if(webSession.PreformatedTable!=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units ){//Pas de colonnes total pour le tableau Autres dimensions X unit�s 
						if(showTotal && !computePercentage)resultTable[currentLineInTabResult,totalColIndex]=cellUnitFactory.Get(0.0);
						if(showTotal && computePercentage){
							if(webSession.PercentageAlignment==WebConstantes.Percentage.Alignment.horizontal)
							resultTable[currentLineInTabResult,totalColIndex]=new CellPercent(0.0,null);
							else if(webSession.PercentageAlignment==WebConstantes.Percentage.Alignment.vertical)
								resultTable[currentLineInTabResult,totalColIndex]=new CellPercent(0.0,(CellUnit)resultTable[currentCellLevel0.LineIndexInResultTable,totalColIndex]);

						}
					}
					for(k=startDataColIndexInit;k<=nbCol;k++){
						if(computePercentage){
							if(webSession.PercentageAlignment==WebConstantes.Percentage.Alignment.horizontal)
								resultTable[currentLineInTabResult,k]=new CellPercent(0.0,(CellUnit)resultTable[currentLineInTabResult,totalColIndex]);
							else if(webSession.PercentageAlignment==WebConstantes.Percentage.Alignment.vertical)
								resultTable[currentLineInTabResult,k]=new CellPercent(0.0,(CellUnit)resultTable[currentCellLevel0.LineIndexInResultTable,k]);

						}
						else{
							if(webSession.PreformatedTable!=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units )
								resultTable[currentLineInTabResult,k]=cellUnitFactory.Get(0.0);
							else if (webSession.PreformatedTable==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units ){
								
								//Keuro
								resultTable[currentLineInTabResult,k]=cellKeuroFactory.Get(0.0);

								//Euro
								k++;
								resultTable[currentLineInTabResult,k]=cellEuroFactory.Get(0.0);

								//Dur�e
								k++;
								resultTable[currentLineInTabResult,k]=cellDurationFactory.Get(0.0);

								//Spots
								k++;
								resultTable[currentLineInTabResult,k]=cellInsertionFactory.Get(0.0);
								break;
							}
						}
					}
					#endregion

					oldIdL1=(Int64)tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.IDL1_INDEX];
					resultTable[currentLineInTabResult,levelLabelColIndex] = new AdExpressCellLevel(Int64.Parse(tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.IDL1_INDEX].ToString()),tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.LABELL1_INDEX].ToString(),currentCellLevel0,1,currentLineInTabResult,webSession,webSession.GenericMediaDetailLevel);		
					currentCellLevel1=(AdExpressCellLevel)resultTable[currentLineInTabResult,levelLabelColIndex];
					if(showCreative)resultTable[currentLineInTabResult,creativeColIndex] = new CellSponsorshipCreativesLink(currentCellLevel1,webSession,genericDetailLevel);
                    if(showInsertions) resultTable[currentLineInTabResult, insertionsColIndex] = new CellSponsorshipInsertionsLink(currentCellLevel1, webSession, genericDetailLevel);
					currentL1Index=currentLineInTabResult;
					oldIdL2 = oldIdL3 = oldIdL4 = -1;

					#region GAD
					if(webSession.GenericMediaDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser)==1){
						if(tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.ADDRESS_COLUMN_INDEX]!=null){
							((CellLevel)resultTable[currentLineInTabResult,levelLabelColIndex]).AddressId=Int64.Parse(tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.ADDRESS_COLUMN_INDEX].ToString());
						}
					}
					#endregion

					
				}
				#endregion

				#region On change de niveau L2
				if(tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.IDL2_INDEX]!=null && (Int64)tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.IDL2_INDEX]!=oldIdL2){
					currentLineInTabResult = resultTable.AddNewLine(LineType.level2);

					#region Totaux et autres dimensions en colonnnes � 0 
					if(webSession.PreformatedTable!=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units ){//Pas de colonnes total pour le tableau Autres dimensions X unit�s 
						if(showTotal && !computePercentage)resultTable[currentLineInTabResult,totalColIndex]=cellUnitFactory.Get(0.0);
						if(showTotal && computePercentage){
							if(webSession.PercentageAlignment==WebConstantes.Percentage.Alignment.horizontal)
								resultTable[currentLineInTabResult,totalColIndex]=new CellPercent(0.0,null);
							else if(webSession.PercentageAlignment==WebConstantes.Percentage.Alignment.vertical)
								resultTable[currentLineInTabResult,totalColIndex]=new CellPercent(0.0,(CellUnit)resultTable[currentCellLevel1.LineIndexInResultTable,totalColIndex]);
						}
					}
					for(k=startDataColIndexInit;k<=nbCol;k++){
						if(computePercentage){
							if(webSession.PercentageAlignment==WebConstantes.Percentage.Alignment.horizontal)
								resultTable[currentLineInTabResult,k]=new CellPercent(0.0,(CellUnit)resultTable[currentLineInTabResult,totalColIndex]);
							else if(webSession.PercentageAlignment==WebConstantes.Percentage.Alignment.vertical)
								resultTable[currentLineInTabResult,k]=new CellPercent(0.0,(CellUnit)resultTable[currentCellLevel1.LineIndexInResultTable,k]);
						}
						else{
							if(webSession.PreformatedTable!=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units )
								resultTable[currentLineInTabResult,k]=cellUnitFactory.Get(0.0);
							else if (webSession.PreformatedTable==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units ){
								
								//Keuro
								resultTable[currentLineInTabResult,k]=cellKeuroFactory.Get(0.0);

								//Euro
								k++;
								resultTable[currentLineInTabResult,k]=cellEuroFactory.Get(0.0);

								//Dur�e
								k++;
								resultTable[currentLineInTabResult,k]=cellDurationFactory.Get(0.0);

								//Spots
								k++;
								resultTable[currentLineInTabResult,k]=cellInsertionFactory.Get(0.0);
								break;
							}
						}
					}
					#endregion

					oldIdL2=(Int64)tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.IDL2_INDEX];
					resultTable[currentLineInTabResult,levelLabelColIndex]= new AdExpressCellLevel(Int64.Parse(tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.IDL2_INDEX].ToString()),tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.LABELL2_INDEX].ToString(),currentCellLevel1,2,currentLineInTabResult,webSession,webSession.GenericMediaDetailLevel);		
					currentCellLevel2=(AdExpressCellLevel)resultTable[currentLineInTabResult,levelLabelColIndex];
					if(showCreative)resultTable[currentLineInTabResult,creativeColIndex]= new CellSponsorshipCreativesLink(currentCellLevel2,webSession,genericDetailLevel);
                    if(showInsertions)resultTable[currentLineInTabResult, insertionsColIndex] = new CellSponsorshipInsertionsLink(currentCellLevel2, webSession, genericDetailLevel);
					currentL2Index=currentLineInTabResult;
					oldIdL3 = oldIdL4 = -1;

					#region GAD
					if(webSession.GenericMediaDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser)==2){
						if(tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.ADDRESS_COLUMN_INDEX]!=null){
							((CellLevel)resultTable[currentLineInTabResult,levelLabelColIndex]).AddressId=Int64.Parse(tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.ADDRESS_COLUMN_INDEX].ToString());
						}
					}
					#endregion
				}
				#endregion

				#region On change de niveau L3
				if(tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.IDL3_INDEX]!=null && (Int64)tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.IDL3_INDEX]!=oldIdL3){
					currentLineInTabResult=resultTable.AddNewLine(LineType.level3);				

					#region Totaux et autres dimensions en colonnnes � 0 
					if(webSession.PreformatedTable!=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units ){//Pas de colonnes total pour le tableau Autres dimensions X unit�s 
						if(showTotal && !computePercentage)resultTable[currentLineInTabResult,totalColIndex]=cellUnitFactory.Get(0.0);
						if(showTotal && computePercentage){
							if(webSession.PercentageAlignment==WebConstantes.Percentage.Alignment.horizontal)
								resultTable[currentLineInTabResult,totalColIndex]=new CellPercent(0.0,null);
							else if(webSession.PercentageAlignment==WebConstantes.Percentage.Alignment.vertical)
								resultTable[currentLineInTabResult,totalColIndex]=new CellPercent(0.0,(CellUnit)resultTable[currentCellLevel2.LineIndexInResultTable,totalColIndex]);
						}
					}
					for(k=startDataColIndexInit;k<=nbCol;k++){
						if(computePercentage){
							if(webSession.PercentageAlignment==WebConstantes.Percentage.Alignment.horizontal)
								resultTable[currentLineInTabResult,k]=new CellPercent(0.0,(CellUnit)resultTable[currentLineInTabResult,totalColIndex]);
							else if(webSession.PercentageAlignment==WebConstantes.Percentage.Alignment.vertical)
								resultTable[currentLineInTabResult,k]=new CellPercent(0.0,(CellUnit)resultTable[currentCellLevel2.LineIndexInResultTable,k]);
						}
						else{
							if(webSession.PreformatedTable!=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units )
								resultTable[currentLineInTabResult,k]=cellUnitFactory.Get(0.0);
							else if (webSession.PreformatedTable==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units ){
								
								//Keuro
								resultTable[currentLineInTabResult,k]=cellKeuroFactory.Get(0.0);

								//Euro
								k++;
								resultTable[currentLineInTabResult,k]=cellEuroFactory.Get(0.0);

								//Dur�e
								k++;
								resultTable[currentLineInTabResult,k]=cellDurationFactory.Get(0.0);

								//Spots
								k++;
								resultTable[currentLineInTabResult,k]=cellInsertionFactory.Get(0.0);
								break;
							}
						}
					}			
					#endregion

					oldIdL3=(Int64)tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.IDL3_INDEX];
					resultTable[currentLineInTabResult,levelLabelColIndex]= new AdExpressCellLevel(Int64.Parse(tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.IDL3_INDEX].ToString()),(string)tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.LABELL3_INDEX].ToString(),currentCellLevel2,3,currentLineInTabResult,webSession,webSession.GenericMediaDetailLevel);		
					currentCellLevel3=(AdExpressCellLevel)resultTable[currentLineInTabResult,levelLabelColIndex];
                    if (showCreative) resultTable[currentLineInTabResult, creativeColIndex] = new CellSponsorshipCreativesLink(currentCellLevel3, webSession, genericDetailLevel);
                    if (showInsertions) resultTable[currentLineInTabResult, insertionsColIndex] = new CellSponsorshipInsertionsLink(currentCellLevel3, webSession, genericDetailLevel);					
					currentL3Index=currentLineInTabResult;
					oldIdL4 = -1;

					#region GAD
					if(webSession.GenericMediaDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser)==3){
						if(tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.ADDRESS_COLUMN_INDEX]!=null){
							((CellLevel)resultTable[currentLineInTabResult,levelLabelColIndex]).AddressId=Int64.Parse(tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.ADDRESS_COLUMN_INDEX].ToString());
						}
					}
					#endregion
				
				}
				#endregion

				#region On change de niveau L4
				if(tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.IDL4_INDEX]!=null && (Int64)tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.IDL4_INDEX]!=oldIdL4){
					currentLineInTabResult=resultTable.AddNewLine(LineType.level4);				

					#region Totaux et autres dimensions en colonnnes � 0 
					if(webSession.PreformatedTable!=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units ){//Pas de colonnes total pour le tableau Autres dimensions X unit�s 
						if(showTotal && !computePercentage)resultTable[currentLineInTabResult,totalColIndex]=cellUnitFactory.Get(0.0);
						if(showTotal && computePercentage){
							if(webSession.PercentageAlignment==WebConstantes.Percentage.Alignment.horizontal)
								resultTable[currentLineInTabResult,totalColIndex]=new CellPercent(0.0,null);
							else if(webSession.PercentageAlignment==WebConstantes.Percentage.Alignment.vertical)
								resultTable[currentLineInTabResult,totalColIndex]=new CellPercent(0.0,(CellUnit)resultTable[currentCellLevel3.LineIndexInResultTable,totalColIndex]);
						}
					}
					for(k=startDataColIndexInit;k<=nbCol;k++){
						if(computePercentage){
							if(webSession.PercentageAlignment==WebConstantes.Percentage.Alignment.horizontal)
								resultTable[currentLineInTabResult,k]=new CellPercent(0.0,(CellUnit)resultTable[currentLineInTabResult,totalColIndex]);
							else if(webSession.PercentageAlignment==WebConstantes.Percentage.Alignment.vertical)
								resultTable[currentLineInTabResult,k]=new CellPercent(0.0,(CellUnit)resultTable[currentCellLevel3.LineIndexInResultTable,k]);
						}
						else{
							if(webSession.PreformatedTable!=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units )
								resultTable[currentLineInTabResult,k]=cellUnitFactory.Get(0.0);
							else if (webSession.PreformatedTable==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units ){
								
								//Keuro
								resultTable[currentLineInTabResult,k]=cellKeuroFactory.Get(0.0);

								//Euro
								k++;
								resultTable[currentLineInTabResult,k]=cellEuroFactory.Get(0.0);

								//Dur�e
								k++;
								resultTable[currentLineInTabResult,k]=cellDurationFactory.Get(0.0);

								//Spots
								k++;
								resultTable[currentLineInTabResult,k]=cellInsertionFactory.Get(0.0);
								break;
							}
						}
					}			
					#endregion

					oldIdL4=(Int64)tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.IDL4_INDEX];
					resultTable[currentLineInTabResult,levelLabelColIndex]= new AdExpressCellLevel(Int64.Parse(tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.IDL4_INDEX].ToString()),tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.LABELL4_INDEX].ToString(),currentCellLevel3,4,currentLineInTabResult,webSession,webSession.GenericMediaDetailLevel);		
					currentCellLevel4=(AdExpressCellLevel)resultTable[currentLineInTabResult,levelLabelColIndex];
                    if (showCreative) resultTable[currentLineInTabResult, creativeColIndex] = new CellSponsorshipCreativesLink(currentCellLevel4, webSession, genericDetailLevel);
                    if (showInsertions) resultTable[currentLineInTabResult, insertionsColIndex] = new CellSponsorshipInsertionsLink(currentCellLevel4, webSession, genericDetailLevel);
					currentL4Index=currentLineInTabResult;
					oldIdL4 = -1;

					#region GAD
					if(webSession.GenericMediaDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser)==4){
						if(tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.ADDRESS_COLUMN_INDEX]!=null){
							((CellLevel)resultTable[currentLineInTabResult,levelLabelColIndex]).AddressId=Int64.Parse(tabData[currentLine,FrameWorkResultConstantes.TvSponsorship.ADDRESS_COLUMN_INDEX].ToString());
						}
					}
					#endregion
				
				}
				#endregion

				// On rajoute les valeurs aux cellules de la ligne	
				long startCol;
				if (webSession.PreformatedTable==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units )
				startCol = FrameWorkResultConstantes.TvSponsorship.FIRST_RESULT_ITEM_COLUMN_INDEX;
				else startCol = FrameWorkResultConstantes.TvSponsorship.TOTAL_COLUMN_INDEX;

				for(k=startCol;k<nbColInTabData;k++){
						if (webSession.PreformatedTable==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units )		
						resultTable.AffectValueAndAddToHierarchy(levelLabelColIndex,currentLineInTabResult,startDataColIndex+k-(long.Parse((FrameWorkResultConstantes.TvSponsorship.FIRST_RESULT_ITEM_COLUMN_INDEX).ToString())),(double)tabData[currentLine,k]);					
					else resultTable.AffectValueAndAddToHierarchy(levelLabelColIndex,currentLineInTabResult,startDataColIndex+k-(long.Parse((FrameWorkResultConstantes.TvSponsorship.TOTAL_COLUMN_INDEX).ToString())),(double)tabData[currentLine,k]);					
				}

			}
			#endregion
	
			return resultTable;
		}
		#endregion

		#region Calcul du nombre de ligne d'un tableau pr�format�
		/// <summary>
		/// Obtient le nombre de ligne du tableau de r�sultat � partir d'un tableau pr�format�
		/// </summary>
		/// <param name="tabData">Tableau pr�format�</param>
		/// <returns>Nombre de ligne du tableau de r�sultat</returns>
		private static long GetNbLineFromPreformatedTableToResultTable(object[,] tabData){

			#region Variables
			long nbLine=0;
			long k;
			Int64 oldIdL1=-1;
			Int64 oldIdL2=-1;
			Int64 oldIdL3=-1;
			Int64 oldIdL4=-1;
			#endregion

			for(k=0;k<tabData.GetLength(0);k++){
				// Somme des L1
				if(tabData[k,FrameWorkResultConstantes.TvSponsorship.IDL1_INDEX]!=null && Int64.Parse(tabData[k,FrameWorkResultConstantes.TvSponsorship.IDL1_INDEX].ToString())!=oldIdL1){
					oldIdL1=Int64.Parse(tabData[k,FrameWorkResultConstantes.TvSponsorship.IDL1_INDEX].ToString());
					oldIdL4=oldIdL3=oldIdL2=-1;
					nbLine++;
				}

				// Somme des L2
				if(tabData[k,FrameWorkResultConstantes.TvSponsorship.IDL2_INDEX]!=null && Int64.Parse(tabData[k,FrameWorkResultConstantes.TvSponsorship.IDL2_INDEX].ToString())!=oldIdL2){
					oldIdL2=Int64.Parse(tabData[k,FrameWorkResultConstantes.TvSponsorship.IDL2_INDEX].ToString());
					oldIdL3=-1;
					oldIdL4=-1;
					nbLine++;
				}

				// Somme des L3
				if(tabData[k,FrameWorkResultConstantes.TvSponsorship.IDL3_INDEX]!=null && Int64.Parse(tabData[k,FrameWorkResultConstantes.TvSponsorship.IDL3_INDEX].ToString())!=oldIdL3){
					oldIdL3=Int64.Parse(tabData[k,FrameWorkResultConstantes.TvSponsorship.IDL3_INDEX].ToString());
					oldIdL4=-1;
					nbLine++;
				}

				// Somme des L4
				if(tabData[k,FrameWorkResultConstantes.TvSponsorship.IDL4_INDEX]!=null && Int64.Parse(tabData[k,FrameWorkResultConstantes.TvSponsorship.IDL4_INDEX].ToString())!=oldIdL4){
					oldIdL4=Int64.Parse(tabData[k,FrameWorkResultConstantes.TvSponsorship.IDL4_INDEX].ToString());
					nbLine++;
				}

			}
			return(nbLine);
		}
		#endregion

		#region Initialisation des indexes
		/// <summary>
		/// Initialisation des tableaux d'indexes et valeurs sur les groupes de s�l�ection et m�dias
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="beginningPeriod">Date de d�but</param>
		/// <param name="nbCol">Nombre de colonnes du tableau</param>
		/// <param name="endPeriod">Date de fin</param>
		/// <param name="maxIndex">Index max des colonnex du tableaux</param>
		/// <param name="dimensionColIndex">(out Tableau d'indexes des dimensions en colonne</param>
		/// <param name="dimensionListForLabelSearch">Liste des libell�s de colonnes</param>	
		internal static void  InitIndexAndValues(WebSession webSession, Hashtable dimensionColIndex,ref long nbCol, string beginningPeriod, string endPeriod,ref int maxIndex,ref string dimensionListForLabelSearch){
		
		
			switch(webSession.PreformatedTable){

				case TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Media :
					InitMediaTableIndex(webSession,dimensionColIndex,ref nbCol,beginningPeriod,endPeriod,ref maxIndex,ref dimensionListForLabelSearch);
					break;

				case TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Period :
					if(	webSession.DetailPeriod==WebConstantes.CustomerSessions.Period.DisplayLevel.weekly
						|| webSession.DetailPeriod==WebConstantes.CustomerSessions.Period.DisplayLevel.monthly){
						beginningPeriod = webSession.PeriodBeginningDate;
						endPeriod = webSession.PeriodEndDate;
					}
					InitDatesTableIndex(webSession,dimensionColIndex,ref nbCol,beginningPeriod,endPeriod,ref maxIndex,ref dimensionListForLabelSearch);
					break;

				case TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units :
					InitUnitsTableIndex(webSession,dimensionColIndex,ref nbCol,ref maxIndex,ref dimensionListForLabelSearch);
					break;
			}
		}

		/// <summary>
		/// Construit le tableau des index des unit�s
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dimensionColIndex">Tableau des index</param>		
		/// <param name="maxIndex">Index max des colonnex du tableaux</param>
		/// <param name="nbCol">Nombre de colonnes</param>
        /// <param name="unitsListForLabelSearch">  units list for Label search</param>
		internal static void InitUnitsTableIndex(WebSession webSession, Hashtable dimensionColIndex,ref long nbCol,ref int maxIndex,ref string unitsListForLabelSearch){
				maxIndex = FrameWorkResultConstantes.TvSponsorship.FIRST_RESULT_ITEM_COLUMN_INDEX;
			int positionUnivers=1;

			//Keuros		
			dimensionColIndex[Constantes.Web.CustomerSessions.Unit.kEuro.GetHashCode()] = new GroupItemForTableResult(Constantes.Web.CustomerSessions.Unit.kEuro.GetHashCode(),positionUnivers,maxIndex);
			unitsListForLabelSearch=Constantes.Web.CustomerSessions.Unit.kEuro.GetHashCode().ToString();
			maxIndex++;
			

			//euro			
			dimensionColIndex[Constantes.Web.CustomerSessions.Unit.euro.GetHashCode()] = new GroupItemForTableResult(Constantes.Web.CustomerSessions.Unit.euro.GetHashCode(),positionUnivers,maxIndex);			
			unitsListForLabelSearch +=","+ Constantes.Web.CustomerSessions.Unit.euro.GetHashCode().ToString();
			maxIndex++;
			
			
			//Dur�e		
			dimensionColIndex[Constantes.Web.CustomerSessions.Unit.duration.GetHashCode()] = new GroupItemForTableResult(Constantes.Web.CustomerSessions.Unit.duration.GetHashCode(),positionUnivers,maxIndex);			
			unitsListForLabelSearch +=","+ Constantes.Web.CustomerSessions.Unit.duration.GetHashCode().ToString();
			maxIndex++;
			
			//Spot ( car parrainage Tv )			
			dimensionColIndex[Constantes.Web.CustomerSessions.Unit.spot.GetHashCode()] = new GroupItemForTableResult(Constantes.Web.CustomerSessions.Unit.spot.GetHashCode(),positionUnivers,maxIndex);			
			unitsListForLabelSearch +=","+ Constantes.Web.CustomerSessions.Unit.spot.GetHashCode().ToString();
			maxIndex++;

			if(dimensionColIndex!=null && dimensionColIndex.Count>0)nbCol = maxIndex;
		}

		/// <summary>
		/// Construit le tableau des index des dates
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="periodItemsIndex">Tableau des index</param>
		/// <param name="beginUserDate">Date de d�but</param>
		/// <param name="endUserDate">Date de fin</param>
		/// <param name="nbCol">Nombre de colonnes</param>
		/// <param name="maxIndex">Index max des colonnex du tableaux</param>
        /// <param name="datesListForLabelSearch">Dates list for label search</param>
		internal static void InitDatesTableIndex(WebSession webSession, Hashtable periodItemsIndex,ref long nbCol, string beginUserDate, string endUserDate,ref int maxIndex,ref string datesListForLabelSearch){
			
			#region Cr�ation du tableau des mois ou semaine
			string tmpDate;
			int positionUnivers=1;
			maxIndex = FrameWorkResultConstantes.TvSponsorship.FIRST_RESULT_ITEM_COLUMN_INDEX;
			
			switch(webSession.DetailPeriod){

				case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
					AtomicPeriodWeek currentWeek=new AtomicPeriodWeek(int.Parse(beginUserDate.Substring(0,4)),int.Parse(beginUserDate.Substring(4,2)));
					AtomicPeriodWeek endWeek=new AtomicPeriodWeek(int.Parse(endUserDate.Substring(0,4)),int.Parse(endUserDate.Substring(4,2)));
					endWeek.Increment();
					while(!(currentWeek.Week==endWeek.Week && currentWeek.Year==endWeek.Year)){
						
						tmpDate=currentWeek.Year.ToString();
						if(currentWeek.Week.ToString().Length<2)tmpDate+="0"+currentWeek.Week.ToString();
						else tmpDate+=currentWeek.Week.ToString();											
						datesListForLabelSearch += tmpDate+",";
						periodItemsIndex[Int64.Parse(tmpDate)]=new GroupItemForTableResult(Int64.Parse(tmpDate),positionUnivers,maxIndex);
						currentWeek.Increment();												
						maxIndex++;
					}	
					if(datesListForLabelSearch!=null && datesListForLabelSearch.Length>0)datesListForLabelSearch = datesListForLabelSearch.Substring(0,datesListForLabelSearch.Length-1);
					break;
				case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
					DateTime dateCurrent=new DateTime(int.Parse(beginUserDate.Substring(0,4)),int.Parse(beginUserDate.Substring(4,2)),1);
					DateTime dateEnd=new DateTime(int.Parse(endUserDate.Substring(0,4)),int.Parse(endUserDate.Substring(4,2)),1);
					dateEnd=dateEnd.AddMonths(1);
					while(!(dateCurrent.Month==dateEnd.Month && dateCurrent.Year==dateEnd.Year)){
						
						tmpDate=dateCurrent.Year.ToString();
						if(dateCurrent.Month.ToString().Length<2)tmpDate+="0"+dateCurrent.Month.ToString();
						else tmpDate+=dateCurrent.Month.ToString();						
						datesListForLabelSearch += tmpDate+",";
							periodItemsIndex[Int64.Parse(tmpDate)]=new GroupItemForTableResult(Int64.Parse(tmpDate),positionUnivers,maxIndex);
						dateCurrent=dateCurrent.AddMonths(1);										
						maxIndex++;
					}
					if(datesListForLabelSearch!=null && datesListForLabelSearch.Length>0)datesListForLabelSearch = datesListForLabelSearch.Substring(0,datesListForLabelSearch.Length-1);
					break;
				case WebConstantes.CustomerSessions.Period.DisplayLevel.dayly:
					DateTime currentDateTime =  DateString.YYYYMMDDToDateTime(beginUserDate);
					DateTime endDate = DateString.YYYYMMDDToDateTime(endUserDate);
					while(currentDateTime<=endDate){																		
						datesListForLabelSearch += DateString.DateTimeToYYYYMMDD(currentDateTime)+",";
						periodItemsIndex[Int64.Parse(DateString.DateTimeToYYYYMMDD(currentDateTime))]=new GroupItemForTableResult(Int64.Parse(DateString.DateTimeToYYYYMMDD(currentDateTime)),positionUnivers,maxIndex);
						currentDateTime = currentDateTime.AddDays(1);												
						maxIndex++;
					}
					if(datesListForLabelSearch!=null && datesListForLabelSearch.Length>0)datesListForLabelSearch = datesListForLabelSearch.Substring(0,datesListForLabelSearch.Length-1);
					break;
				default:
					throw(new WebException.TvSponsorshipRulesException("Impossible de construire le tableau d'index des dates"));
			}
			#endregion

			if(periodItemsIndex!=null && periodItemsIndex.Count>0)nbCol = maxIndex;
		}

		/// <summary>
		/// Initialise le tableau des index supports
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="mediaIndex">Tableau des index supports</param>
		/// <param name="nbCol">Nombre de colonnes</param>
		/// <param name="beginningPeriod">Date de d�but</param>
		/// <param name="endPeriod">Date de fin</param>
		/// <param name="maxIndex">Index max des colonnex du tableaux</param>
		/// <param name="mediaListForLabelSearch">List des m�idas pour la recherche</param>
		internal static void  InitMediaTableIndex(WebSession webSession, Hashtable mediaIndex,ref long nbCol, string beginningPeriod, string endPeriod,ref int maxIndex,ref string mediaListForLabelSearch){
			
			#region Variables
			string tmp="";
			//			int positionUnivers=1;
			string[] mediaList;
			maxIndex = FrameWorkResultConstantes.TvSponsorship.FIRST_RESULT_ITEM_COLUMN_INDEX;
			#endregion

			//			if(webSession.CompetitorUniversMedia.Count>0){

			if(webSession.CurrentUniversMedia!=null && webSession.CurrentUniversMedia.Nodes.Count>0){
				// Chargement de la liste de support (m�dia)
				tmp=webSession.GetSelection(webSession.CurrentUniversMedia,CustomerConstantes.Right.type.mediaAccess);
				mediaListForLabelSearch+=tmp+",";
				if(mediaListForLabelSearch!=null && mediaListForLabelSearch.Length>0)mediaListForLabelSearch = mediaListForLabelSearch.Substring(0,mediaListForLabelSearch.Length-1);
				mediaList=tmp.Split(',');
				// Indexes des m�dia (support)
				foreach(string currentMedia in mediaList){						
					mediaIndex[Int64.Parse(currentMedia)]=new GroupItemForTableResult(Int64.Parse(currentMedia),1,maxIndex);
					maxIndex++;						
				}									
			}
				//		}	
			else{
				if(webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS ){
					DataSet ds=TvSponsorshipDataAccess.GetMediaData(webSession,beginningPeriod,endPeriod);	
					
					if(ds!=null && ds.Tables[0].Rows.Count>0){
						
						foreach(DataRow dr in ds.Tables[0].Rows){		
							mediaListForLabelSearch+=dr["id_media"].ToString()+",";
							mediaIndex[Int64.Parse(dr["id_media"].ToString())]=new GroupItemForTableResult(Int64.Parse(dr["id_media"].ToString()),1,maxIndex);
							maxIndex++;							
						}
						if(mediaListForLabelSearch!=null && mediaListForLabelSearch.Length>0)mediaListForLabelSearch = mediaListForLabelSearch.Substring(0,mediaListForLabelSearch.Length-1);
					}
				}

			}
			if(mediaIndex!=null && mediaIndex.Count>0)nbCol = maxIndex;

		}
		#endregion		

		#region Insert les valeurs des niveaux de d�tail m�dia
		/// <summary>
		/// Calcule les r�sultats pour les niveaux de d�tail m�dia
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dt">Table de donn�es</param>
		/// <param name="tabResult">Table de r�sultats</param>
		/// <param name="dimensionColIndex">Tableau d'indexes</param>
		/// <param name="nbCol">Nombre de colonnes</param>
		/// <param name="nbLineInNewTable">Nombre de lignes dans le tableau de r�sultats</param>
		private static void SetMediaDetailLevelValues(WebSession webSession,DataTable dt,object[,] tabResult,ref long nbLineInNewTable,Hashtable dimensionColIndex,long nbCol){
			
			#region Variables
//			double unit;
			long oldIdL1=-1;
			long oldIdL2=-1;
			long oldIdL3=-1;
			long oldIdL4=-1;
			Int64 currentLine=-1;
			int k;
			bool changeLine=false;
		
			#endregion

			#region Tableau de r�sultat


			
			foreach(DataRow currentRow in dt.Rows){
				
				if(webSession.GenericMediaDetailLevel.GetIdValue(currentRow,1)>=0 && webSession.GenericMediaDetailLevel.GetIdValue(currentRow,1)!=oldIdL1)changeLine=true;
				if(!changeLine && webSession.GenericMediaDetailLevel.GetIdValue(currentRow,2)>=0 && webSession.GenericMediaDetailLevel.GetIdValue(currentRow,2)!=oldIdL2)changeLine=true;
				if(!changeLine && webSession.GenericMediaDetailLevel.GetIdValue(currentRow,3)>=0 && webSession.GenericMediaDetailLevel.GetIdValue(currentRow,3)!=oldIdL3)changeLine=true;
				if(!changeLine && webSession.GenericMediaDetailLevel.GetIdValue(currentRow,4)>=0 && webSession.GenericMediaDetailLevel.GetIdValue(currentRow,4)!=oldIdL4)changeLine=true;

				#region On change de ligne
				if(changeLine){
					currentLine++;

					// identifiants de L1 
					if(webSession.GenericMediaDetailLevel.GetIdValue(currentRow,1)>=0){
						oldIdL1=webSession.GenericMediaDetailLevel.GetIdValue(currentRow,1);
						tabResult[currentLine,FrameWorkResultConstantes.TvSponsorship.IDL1_INDEX]=oldIdL1;
						tabResult[currentLine,FrameWorkResultConstantes.TvSponsorship.LABELL1_INDEX]=webSession.GenericMediaDetailLevel.GetLabelValue(currentRow,1);
					}

					// identifiants de L2 
					if(webSession.GenericMediaDetailLevel.GetIdValue(currentRow,2)>=0){
						oldIdL2=webSession.GenericMediaDetailLevel.GetIdValue(currentRow,2);
						tabResult[currentLine,FrameWorkResultConstantes.TvSponsorship.IDL2_INDEX]=oldIdL2;
						tabResult[currentLine,FrameWorkResultConstantes.TvSponsorship.LABELL2_INDEX]=webSession.GenericMediaDetailLevel.GetLabelValue(currentRow,2);
					}

					// identifiants de L3 
					if(webSession.GenericMediaDetailLevel.GetIdValue(currentRow,3)>=0){
						oldIdL3=webSession.GenericMediaDetailLevel.GetIdValue(currentRow,3);
						tabResult[currentLine,FrameWorkResultConstantes.TvSponsorship.IDL3_INDEX]=oldIdL3;
						tabResult[currentLine,FrameWorkResultConstantes.TvSponsorship.LABELL3_INDEX]=webSession.GenericMediaDetailLevel.GetLabelValue(currentRow,3);
					}

					// identifiants de L4 
					if(webSession.GenericMediaDetailLevel.GetIdValue(currentRow,4)>=0){
						oldIdL4=webSession.GenericMediaDetailLevel.GetIdValue(currentRow,4);
						tabResult[currentLine,FrameWorkResultConstantes.TvSponsorship.IDL4_INDEX]=oldIdL4;
						tabResult[currentLine,FrameWorkResultConstantes.TvSponsorship.LABELL4_INDEX]=webSession.GenericMediaDetailLevel.GetLabelValue(currentRow,4);
					}

					// Totaux  et dimensions en colonnes � 0
					for(k=FrameWorkResultConstantes.TvSponsorship.TOTAL_COLUMN_INDEX;k<nbCol;k++){
						tabResult[currentLine,k]=(double) 0.0;
					}
						
					if(webSession.GenericMediaDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser)){
						try{
							if(currentRow["id_address"]!=null)tabResult[currentLine,FrameWorkResultConstantes.TvSponsorship.ADDRESS_COLUMN_INDEX]=Int64.Parse(currentRow["id_address"].ToString());
						}catch(Exception){
						
						}

					}

					changeLine=false;
				}
				#endregion

				
				//Insertion valeurs dans cellule
				SetTableCellValue(webSession,tabResult,currentRow,currentLine,dimensionColIndex);
				
			}
			#endregion

			nbLineInNewTable=currentLine+1;
		}
		
		#endregion

		#region Insert des valeurs dans les cellules du tableau
		/// <summary>
		/// Insert des valeurs dans les celules du tableau de r�sulats
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="tabResult">Tableau de r�sultats</param>
		/// <param name="currentRow">Ligne courante dans table de donn�es (DataTable)</param>
		/// <param name="currentLine">Ligne courante dans tableau de r�sultats </param>
		/// <param name="dimensionColIndex"></param>
		private static void SetTableCellValue(WebSession webSession ,object[,] tabResult,DataRow currentRow,long currentLine,Hashtable dimensionColIndex){
			double unit;

			switch(webSession.PreformatedTable){

				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Media :
					
					unit=double.Parse(currentRow[WebFunctions.SQLGenerator.GetUnitAlias(webSession)].ToString());				
					
					// Ecriture du r�sultat des dimensions en colonnes
					if(dimensionColIndex!=null && dimensionColIndex.Count>0)
					tabResult[currentLine,((GroupItemForTableResult)dimensionColIndex[Int64.Parse(currentRow["id_media"].ToString())]).IndexInResultTable]=unit;								
				
					// Ecriture du r�sultat du total (somme)
					tabResult[currentLine,FrameWorkResultConstantes.TvSponsorship.TOTAL_COLUMN_INDEX]=(double)tabResult[currentLine,FrameWorkResultConstantes.TvSponsorship.TOTAL_COLUMN_INDEX]+unit;
					break;

				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Period :
                    unit = double.Parse(currentRow[WebFunctions.SQLGenerator.GetUnitAlias(webSession)].ToString());				

					// Ecriture du r�sultat des dimensions en colonnes
					if(dimensionColIndex!=null && dimensionColIndex.Count>0)
					tabResult[currentLine,((GroupItemForTableResult)dimensionColIndex[Int64.Parse(currentRow["date_num"].ToString())]).IndexInResultTable]=unit;								
				
					// Ecriture du r�sultat du total (somme)
					tabResult[currentLine,FrameWorkResultConstantes.TvSponsorship.TOTAL_COLUMN_INDEX]=(double)tabResult[currentLine,FrameWorkResultConstantes.TvSponsorship.TOTAL_COLUMN_INDEX]+unit;				
					break;

				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units :
														
					// Ecriture du r�sultat des keuros en colonnes
					unit=double.Parse(currentRow[UnitsInformation.List[CustomerSessions.Unit.euro].Id.ToString()].ToString());
                    tabResult[currentLine, ((GroupItemForTableResult)dimensionColIndex[UnitsInformation.List[CustomerSessions.Unit.kEuro].Id.GetHashCode()]).IndexInResultTable] = unit;
					
					// Ecriture du r�sultat des euro en colonnes
                    unit = double.Parse(currentRow[UnitsInformation.List[CustomerSessions.Unit.euro].Id.ToString()].ToString());
					if(dimensionColIndex!=null && dimensionColIndex.Count>0)
                        tabResult[currentLine, ((GroupItemForTableResult)dimensionColIndex[UnitsInformation.List[CustomerSessions.Unit.euro].Id.GetHashCode()]).IndexInResultTable] = unit;								
					
					// Ecriture du r�sultat des dur�e en colonnes
                unit = double.Parse(currentRow[UnitsInformation.List[CustomerSessions.Unit.duration].Id.ToString()].ToString());
					if(dimensionColIndex!=null && dimensionColIndex.Count>0)
                        tabResult[currentLine, ((GroupItemForTableResult)dimensionColIndex[UnitsInformation.List[CustomerSessions.Unit.duration].Id.GetHashCode()]).IndexInResultTable] = unit;	
					
					// Ecriture du r�sultat des spots en colonnes
                unit = double.Parse(currentRow[UnitsInformation.List[CustomerSessions.Unit.insertion].Id.ToString()].ToString());
					if(dimensionColIndex!=null && dimensionColIndex.Count>0)
                        tabResult[currentLine, ((GroupItemForTableResult)dimensionColIndex[UnitsInformation.List[CustomerSessions.Unit.spot].Id.GetHashCode()]).IndexInResultTable] = unit;								
							
					break;
			}
		}
		#endregion

		#region Chargements des libell�s de colonnes
		/// <summary>
		/// D�finit les lib�ll�s des ent�tes de colonnes
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="headers">ent�tes</param>
		/// <param name="dimensionColIndex">Tableau des indexes des dimensions en colonnes</param>
		/// <param name="dimensionListForLabelSearch">Identifiants des �l�ments  en colonnes pour la rechercher de libell�s</param>
		private static void SetColumnsLabels(WebSession webSession, Headers headers,Hashtable dimensionColIndex,string dimensionListForLabelSearch){
			string[] dimendionIdList=null;
			HeaderGroup headerGroupTmp=null;
			int m = 1;
			int i = 0;
			AtomicPeriodWeek week;
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
			switch(webSession.PreformatedTable){

				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Media :
				
					//Libell�s supports 
					headerGroupTmp=new HeaderGroup(GestionWeb.GetWebWord(804,webSession.SiteLanguage),true,FrameWorkResultConstantes.TvSponsorship.START_ID_GROUP+m);//TODO v�rifier index de d�part															

					if(dimensionListForLabelSearch!=null && dimensionListForLabelSearch.Length>0){
						dimendionIdList=dimensionListForLabelSearch.Split(',');
						ClassificationDB.MediaBranch.PartialMediaListDataAccess mediaLabelList = new ClassificationDB.MediaBranch.PartialMediaListDataAccess(dimensionListForLabelSearch,webSession.DataLanguage,webSession.Source);
						
						foreach(string currentMedia in dimendionIdList) {							
							headerGroupTmp.Add(new Header(true,mediaLabelList[Int64.Parse(currentMedia)],Int64.Parse(currentMedia)));							
						}
						headers.Root.Add(headerGroupTmp);
					}										
					break;

				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Period :
					//Libell�s p�riodes
					headerGroupTmp=new HeaderGroup(GestionWeb.GetWebWord(1755,webSession.SiteLanguage),true,FrameWorkResultConstantes.TvSponsorship.START_ID_GROUP+m);//TODO v�rifier index de d�part

					if(dimensionListForLabelSearch!=null && dimensionListForLabelSearch.Length>0){
						dimendionIdList=dimensionListForLabelSearch.Split(',');
						string dateLabel = null;

						foreach(string currentDate in dimendionIdList) {

							switch(webSession.DetailPeriod){

								case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
									dateLabel = DateString.dateTimeToDD_MM_YYYY(WebFunctions.Dates.getPeriodBeginningDate(currentDate, webSession.PeriodType), webSession.SiteLanguage)
									+ "-" + DateString.dateTimeToDD_MM_YYYY(WebFunctions.Dates.getPeriodEndDate(currentDate, webSession.PeriodType), webSession.SiteLanguage);
									break;
								case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
									i++;
									if (i == 1 || i == dimendionIdList.GetLongLength(0)) {//Si premier ou dernier mois de l'univers
										dateLabel = MonthString.GetCharacters(int.Parse(currentDate.Substring(4, 2)), cultureInfo, 9) + " " + currentDate.Substring(0, 4);
										if (webSession.PeriodType == WebConstantes.CustomerSessions.Period.Type.dateToDate) {
											dateLabel += (i == 1) ? "<br>" + DateString.YYYYMMDDToDD_MM_YYYY(webSession.PeriodBeginningDate, webSession.SiteLanguage) : "<br>" + DateString.YYYYMMDDToDD_MM_YYYY(webSession.PeriodEndDate, webSession.SiteLanguage);
											if (dimendionIdList.GetLongLength(0) == 1 && !webSession.PeriodBeginningDate.Equals(webSession.PeriodEndDate))
												dateLabel = dateLabel + " - " + DateString.YYYYMMDDToDD_MM_YYYY(webSession.PeriodEndDate, webSession.SiteLanguage);
										}
									}
									else dateLabel = MonthString.GetCharacters(int.Parse(currentDate.Substring(4, 2)), cultureInfo, 9) + " " + currentDate.Substring(0, 4);
									break;
								case WebConstantes.CustomerSessions.Period.DisplayLevel.dayly:
									dateLabel = DateString.YYYYMMDDToDD_MM_YYYY(currentDate,webSession.SiteLanguage);//TODO v�rifier la forme du libell� date retourn� par la fonction DateString.YYYYMMDDToDD_MM_YYYY
									break;
							}

							if(dateLabel!=null)headerGroupTmp.Add(new Header(true,dateLabel,Int64.Parse(currentDate)));	
							dateLabel = null;
						}
						headers.Root.Add(headerGroupTmp);
					}
					break;

				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units :
					//Libell�s unit�s
					headerGroupTmp=new HeaderGroup(GestionWeb.GetWebWord(2061,webSession.SiteLanguage),true,FrameWorkResultConstantes.TvSponsorship.START_ID_GROUP+m);//TODO v�rifier index de d�part
					
					if(dimensionListForLabelSearch!=null && dimensionListForLabelSearch.Length>0){
						dimendionIdList=dimensionListForLabelSearch.Split(',');
						string unitLabel = null;
						Constantes.Web.CustomerSessions.Unit unit;

						foreach(string currentUnit in dimendionIdList) {
							unit = (Constantes.Web.CustomerSessions.Unit)int.Parse(currentUnit);
							
							switch(unit){
								case Constantes.Web.CustomerSessions.Unit.euro :
									unitLabel  = Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[CustomerSessions.Unit.euro].WebTextId,webSession.SiteLanguage));
									break;
								case Constantes.Web.CustomerSessions.Unit.kEuro :
                                    unitLabel = Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[CustomerSessions.Unit.kEuro].WebTextId, webSession.SiteLanguage));
									break;
								case Constantes.Web.CustomerSessions.Unit.duration :
                                    unitLabel = Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[CustomerSessions.Unit.duration].WebTextId, webSession.SiteLanguage));
									break;
								case Constantes.Web.CustomerSessions.Unit.spot :
                                    unitLabel = Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[CustomerSessions.Unit.spot].WebTextId, webSession.SiteLanguage));
									break;
							}

							if(unitLabel!=null)headerGroupTmp.Add(new Header(true,unitLabel,Int64.Parse(currentUnit)));	
							unitLabel = null;
						}
						headers.Root.Add(headerGroupTmp);
					}
					break;
			}
		}
		#endregion

		#endregion
	}
}
