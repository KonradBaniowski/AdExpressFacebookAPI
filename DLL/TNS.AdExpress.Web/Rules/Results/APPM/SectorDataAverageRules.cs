#region Informations
// Auteur: Y. R'kaina 
// Date de création: 17/01/2007 
#endregion

using System;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Date;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using SQLFunctions=TNS.AdExpress.Web.DataAccess;
using WebFnc = TNS.AdExpress.Web.Functions;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.WebResultUI;
using APPMConstantes=TNS.AdExpress.Constantes.FrameWork.Results.APPM;
using WebConstantes=TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.Rules.Results.APPM{
	/// <summary>
	/// Provides formatting rules for Sector Data Average
	/// </summary>
	public class SectorDataAverageRules{
	
		#region Constantes
		/// <summary>
		/// Nombre de lignes dans le tableau
		/// </summary>
		private const int LINES_NUMBER = 12;
		/// <summary>
		/// Nombre de colonnes dans le tableau
		/// </summary>
		private const int COLUMNS_NUMBER = 4;
		#endregion

		#region Sector Data synthesis 
		/// <summary>
		/// Formats the date for sector Data synthesis 
		/// </summary>
		/// <param name="webSession">clients session</param>
		/// <param name="dateBegin">Beginning Date</param>
		/// <param name="dateEnd">Ending Date</param>
		/// <param name="baseTarget">Base target</param>
		/// <param name="additionalTarget">supplementary target</param>
		/// <returns>HTML string for the synthesis table</returns>		
		public static ResultTable GetAverageFormattedTable(WebSession webSession,int dateBegin,int dateEnd,Int64 baseTarget,Int64 additionalTarget){
			
			#region variables
			DataTable synthesisTable=null;			
			bool start=true;
			string startDate=string.Empty;
			string targetSelected=string.Empty;
			string targetBase=string.Empty;
			string endDate=string.Empty;
			double avgBudget=0, minBudget=0, maxBudget=0;
			double avgInsertions=0, minInsertions=0, maxInsertions=0;
			double avgNbMedia=0, minNbMedia=0, maxNbMedia=0;
			double baseTargetGRP=0, avgBaseTargetGRP=0, minBaseTargetGRP=0, maxBaseTargetGRP=0;
			double baseTargetCost=0, avgBaseTargetCost=0, minBaseTargetCost=0, maxBaseTargetCost=0;
			double additionalTargetGRP=0, avgAdditionalTargetGRP=0, minAdditionalTargetGRP=0, maxAdditionalTargetGRP=0;
			double GRPAffinities=0, avgGRPAffinities=0, minGRPAffinities=0, maxGRPAffinities=0;
			double CGRPAffinities=0, avgCGRPAffinities=0, minCGRPAffinities=0, maxCGRPAffinities=0;
			double additionalTargetCost=0, avgAdditionalTargetCost=0, minAdditionalTargetCost=0, maxAdditionalTargetCost=0;
			double avgPages=0, minPages=0, maxPages=0;
//			double avgNbWeek=0, minNbWeek=0, maxNbWeek=0;
//			double avgGRPWeek=0, minGRPWeek=0, maxGRPWeek=0;
			ResultTable resultTable=null;
			#endregion			

			try{

				#region get Data
				synthesisTable=TNS.AdExpress.Web.DataAccess.Results.APPM.SectorDataAverageDataAccess.GetData(webSession,dateBegin,dateEnd,baseTarget,additionalTarget).Tables[0];

				#region formatage des dates
//				DateTime dateDebut, dateFin;
//				bool periodTypeMonth;
//				
//				switch(webSession.PeriodType){
//					case WebConstantes.CustomerSessions.Period.Type.dateToDateWeek:
//					case WebConstantes.CustomerSessions.Period.Type.LastLoadedWeek:
//					case WebConstantes.CustomerSessions.Period.Type.nLastWeek:
//					case WebConstantes.CustomerSessions.Period.Type.previousWeek:
//						periodTypeMonth=false;break;
//					default :
//						periodTypeMonth=true;break;
//				}
//					
//				if(periodTypeMonth){
//					dateDebut = new DateTime(int.Parse((dateBegin.ToString()).Substring(0,4)),int.Parse((dateBegin.ToString()).Substring(4,2)),1);
//					dateFin = new DateTime(int.Parse((dateEnd.ToString()).Substring(0,4)),int.Parse((dateEnd.ToString()).Substring(4,2)),1);
//					dateFin = dateFin.AddMonths(1);
//					dateFin = dateFin.AddDays(-1);
//					AtomicPeriodWeek beginWeek = new AtomicPeriodWeek(dateDebut);
//					AtomicPeriodWeek endWeek = new AtomicPeriodWeek(dateFin);
//					dateBegin = (beginWeek.Year*100)+beginWeek.Week;
//					dateEnd = (endWeek.Year*100)+endWeek.Week;
//				}
				#endregion

//              presenceDuration=TNS.AdExpress.Web.DataAccess.Results.APPM.SectorDataAverageDataAccess.GetPresenceDuration(webSession,dateBegin,dateEnd,baseTarget,additionalTarget).Tables[0];
				#endregion

				if(synthesisTable!=null && synthesisTable.Rows.Count>0){

					#region Création des headers
					// Ajout de la colonne Produit
					Headers headers=new Headers();
					headers.Root.Add(new Header(GestionWeb.GetWebWord(1054,webSession.SiteLanguage),APPMConstantes.FIRST_COLUMN_INDEX-1));
					headers.Root.Add(new Header(GestionWeb.GetWebWord(2081,webSession.SiteLanguage),APPMConstantes.AVERAGE_COLUMN_INDEX-1));
					headers.Root.Add(new Header(GestionWeb.GetWebWord(2096,webSession.SiteLanguage),APPMConstantes.MIN_COLUMN_INDEX-1));
					headers.Root.Add(new Header(GestionWeb.GetWebWord(2097,webSession.SiteLanguage),APPMConstantes.MAX_COLUMN_INDEX-1));
					#endregion

					#region Crétaion de la table resultTable
					long nbLines=LINES_NUMBER;
					long nbCol;
					long lineIndex=0;
					resultTable = new ResultTable(nbLines,headers);
					nbCol = resultTable.DataColumnsNumber;
					#endregion
					
					#region date formatting
					//Getting the date in the format yyyyMMdd
					int dateBeginning = int.Parse(WebFnc.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
					int dateEnding = int.Parse(WebFnc.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
					//Formatting the date in the format e.g 12/12/2005
					if(dateBeginning.ToString().Length>0)
						startDate = WebFnc.Dates.dateToString(new DateTime(int.Parse(dateBeginning.ToString().Substring(0,4)),int.Parse(dateBeginning.ToString().Substring(4,2)),int.Parse(dateBeginning.ToString().Substring(6,2))),webSession.SiteLanguage);						
					if(dateEnding.ToString().Length>0)
						endDate = WebFnc.Dates.dateToString(new DateTime(int.Parse(dateEnding.ToString().Substring(0,4)),int.Parse(dateEnding.ToString().Substring(4,2)),int.Parse(dateEnding.ToString().Substring(6,2))),webSession.SiteLanguage);												
					#endregion															
									
					#region construction of synthesis hashtable
				
					#region traversing the table
					foreach(DataRow dr in synthesisTable.Rows){
						//Values for the base target
						if(Convert.ToInt64(dr["id_target"])==baseTarget){

							#region Nombre de GRP (Moyenne, Min, Max)
							baseTargetGRP=Convert.ToDouble(dr["totalgrp"].ToString());
							avgBaseTargetGRP+=Convert.ToDouble(dr["totalgrp"].ToString());
							
							if(start)
								minBaseTargetGRP=Convert.ToDouble(dr["totalgrp"].ToString());
                            else if(minBaseTargetGRP>Convert.ToDouble(dr["totalgrp"].ToString()))
								minBaseTargetGRP=Convert.ToDouble(dr["totalgrp"].ToString());

							if(maxBaseTargetGRP<Convert.ToDouble(dr["totalgrp"].ToString()))
								maxBaseTargetGRP=Convert.ToDouble(dr["totalgrp"].ToString());
							#endregion

							#region Budget (Moyenne, Min, Max)
							avgBudget+=Convert.ToDouble(dr["euros"].ToString());

							if(start)
								minBudget=Convert.ToDouble(dr["euros"].ToString());
							else if(minBudget>Convert.ToDouble(dr["euros"].ToString()))
								minBudget=Convert.ToDouble(dr["euros"].ToString());

							if(maxBudget<Convert.ToDouble(dr["euros"].ToString()))
								maxBudget=Convert.ToDouble(dr["euros"].ToString());
							#endregion

							#region Nombre de titres (Moyenne, Min, Max)
							avgNbMedia+=Convert.ToDouble(dr["nbMedia"].ToString());

							if(start)
								minNbMedia=Convert.ToDouble(dr["nbMedia"].ToString());
							else if(minNbMedia>Convert.ToDouble(dr["nbMedia"].ToString()))
								minNbMedia=Convert.ToDouble(dr["nbMedia"].ToString());

							if(maxNbMedia<Convert.ToDouble(dr["nbMedia"].ToString()))
								maxNbMedia=Convert.ToDouble(dr["nbMedia"].ToString());
							#endregion

							#region Nombre d'insertions (Moyenne, Min, Max)
							avgInsertions+=Convert.ToDouble(dr["insertions"].ToString());

							if(start)
								minInsertions=Convert.ToDouble(dr["insertions"].ToString());
							else if(minInsertions>Convert.ToDouble(dr["insertions"].ToString()))
								minInsertions=Convert.ToDouble(dr["insertions"].ToString());

							if(maxInsertions<Convert.ToDouble(dr["insertions"].ToString()))
								maxInsertions=Convert.ToDouble(dr["insertions"].ToString());
							#endregion

							#region Nombre de pages (Moyenne, Min, Max)
							avgPages+=Convert.ToDouble(dr["pages"]);

							if(start)
								minPages=Convert.ToDouble(dr["pages"]);
							else if(minPages>Convert.ToDouble(dr["pages"]))
								minPages=Convert.ToDouble(dr["pages"]);

							if(maxPages<Convert.ToDouble(dr["pages"]))
								maxPages=Convert.ToDouble(dr["pages"]);
							#endregion

							#region Coût GRP (Moyenne, Min, Max)
							if(Convert.ToDouble(dr["totalgrp"].ToString())>0)
								baseTargetCost=Convert.ToDouble(dr["euros"].ToString())/Convert.ToDouble(dr["totalgrp"].ToString());
							else
								baseTargetCost=0;

							avgBaseTargetCost+=baseTargetCost;

							if(start)
								minBaseTargetCost=baseTargetCost;
							else if(minBaseTargetCost>baseTargetCost)
								minBaseTargetCost=baseTargetCost;

                            if(maxBaseTargetCost<baseTargetCost)
								maxBaseTargetCost=baseTargetCost;
							#endregion

							targetBase=dr["target"].ToString();
						}
						//values for the supplementary target
						else{

							#region Nombre de GRP (Moyenne, Min, Max)
							additionalTargetGRP=Convert.ToDouble(dr["totalgrp"].ToString());                                  
							avgAdditionalTargetGRP+=Convert.ToDouble(dr["totalgrp"].ToString());
							
							if(start)
								minAdditionalTargetGRP=Convert.ToDouble(dr["totalgrp"].ToString());
							else if(minAdditionalTargetGRP>Convert.ToDouble(dr["totalgrp"].ToString()))
								minAdditionalTargetGRP=Convert.ToDouble(dr["totalgrp"].ToString());

							if(maxAdditionalTargetGRP<Convert.ToDouble(dr["totalgrp"].ToString()))
								maxAdditionalTargetGRP=Convert.ToDouble(dr["totalgrp"].ToString());
							#endregion

							#region Coût GRP (Moyenne, Min, Max)
							if(Convert.ToDouble(dr["totalgrp"].ToString())>0)
								additionalTargetCost=Convert.ToDouble(dr["euros"].ToString())/Convert.ToDouble(dr["totalgrp"].ToString());
							else
								additionalTargetCost=0;

							avgAdditionalTargetCost+=additionalTargetCost;

							if(start)
								minAdditionalTargetCost=additionalTargetCost;
							if(minAdditionalTargetCost>additionalTargetCost)
								minAdditionalTargetCost=additionalTargetCost;

							if(maxAdditionalTargetCost<additionalTargetCost)
								maxAdditionalTargetCost=additionalTargetCost;
							#endregion

							#region Affinities GRP vs Ensemble 15 ans et plus
							if(baseTargetGRP>0)
								GRPAffinities=(additionalTargetGRP/baseTargetGRP)*100;
							else
								GRPAffinities=0;

							avgGRPAffinities+=GRPAffinities;

							if(start)
								minGRPAffinities=GRPAffinities;
							else if(minGRPAffinities>GRPAffinities)
								minGRPAffinities=GRPAffinities;

							if(maxGRPAffinities<GRPAffinities)
								maxGRPAffinities=GRPAffinities;
							#endregion

							#region Affinities coût GRP vc Ensemble 15 ans et plus
							if(baseTargetCost>0)
								CGRPAffinities=(additionalTargetCost/baseTargetCost)*100;
							else
								CGRPAffinities=0;

							avgCGRPAffinities+=CGRPAffinities;

							if(start)
								minCGRPAffinities=CGRPAffinities;
							if(minCGRPAffinities>CGRPAffinities)
								minCGRPAffinities=CGRPAffinities;

							if(maxCGRPAffinities<CGRPAffinities)
								maxCGRPAffinities=CGRPAffinities;
							#endregion

							targetSelected=dr["target"].ToString();
							start=false;
						}
					}					
					#endregion

					#region Calcul des moyennes
					int nbRows=synthesisTable.Rows.Count/2;

					avgBaseTargetGRP = avgBaseTargetGRP/nbRows;
					avgBudget = avgBudget/nbRows;
					avgNbMedia = avgNbMedia/nbRows;
					avgInsertions = avgInsertions/nbRows;
					avgPages = avgPages/nbRows;
					avgBaseTargetCost = avgBaseTargetCost/nbRows;
					avgAdditionalTargetGRP = avgAdditionalTargetGRP/nbRows;
					avgAdditionalTargetCost = avgAdditionalTargetCost/nbRows;
					avgGRPAffinities = avgGRPAffinities/nbRows;
					avgCGRPAffinities = avgCGRPAffinities/nbRows;
					#endregion

					#region Budget
					lineIndex = resultTable.AddNewLine(LineType.level1);
					resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(1712,webSession.SiteLanguage)+" : ");
					//budget (moyenne)
					resultTable[lineIndex,APPMConstantes.AVERAGE_COLUMN_INDEX]=new CellEuro(avgBudget);
					//budget (min)
					resultTable[lineIndex,APPMConstantes.MIN_COLUMN_INDEX]=new CellEuro(minBudget);
					//budget (max)
					resultTable[lineIndex,APPMConstantes.MAX_COLUMN_INDEX]=new CellEuro(maxBudget);
					#endregion

					#region Supports
					lineIndex = resultTable.AddNewLine(LineType.level2);
					resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(2108,webSession.SiteLanguage)+" : ");
					//Nombre de supports (moyenne)
					resultTable[lineIndex,APPMConstantes.AVERAGE_COLUMN_INDEX]=new CellNumber(avgNbMedia);
					//Nombre de supports (min)
					resultTable[lineIndex,APPMConstantes.MIN_COLUMN_INDEX]=new CellNumber(minNbMedia);
					//Nombre de supports (max)
					resultTable[lineIndex,APPMConstantes.MAX_COLUMN_INDEX]=new CellNumber(maxNbMedia);
					#endregion

					#region Insertions
					lineIndex = resultTable.AddNewLine(LineType.level1);
					resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(1398,webSession.SiteLanguage)+" : ");
					//nombre d'insertions (moyenne)
					resultTable[lineIndex,APPMConstantes.AVERAGE_COLUMN_INDEX]=new CellInsertion(avgInsertions);
					//nombre d'insertions (min)
					resultTable[lineIndex,APPMConstantes.MIN_COLUMN_INDEX]=new CellInsertion(minInsertions);
					//nombre d'insertions (max)
					resultTable[lineIndex,APPMConstantes.MAX_COLUMN_INDEX]=new CellInsertion(maxInsertions);
					#endregion

					#region Pages
					lineIndex = resultTable.AddNewLine(LineType.level2);
					resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(1385,webSession.SiteLanguage)+" : ");
					//nombre de pages utilisés (moyenne)
					avgPages=Math.Round(avgPages,3);
					resultTable[lineIndex,APPMConstantes.AVERAGE_COLUMN_INDEX]=new CellPage(avgPages);
					//nombre de pages utilisés (min)
					minPages=Math.Round(minPages,3);
					resultTable[lineIndex,APPMConstantes.MIN_COLUMN_INDEX]=new CellPage(minPages);
					//nombre de pages utilisés (max)
					maxPages=Math.Round(maxPages,3);
					resultTable[lineIndex,APPMConstantes.MAX_COLUMN_INDEX]=new CellPage(maxPages);
					#endregion

					#region Ligne blanche
					lineIndex = resultTable.AddNewLine(LineType.level3);
					resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel("");
					resultTable[lineIndex,APPMConstantes.AVERAGE_COLUMN_INDEX]=new CellLabel("");
					resultTable[lineIndex,APPMConstantes.MIN_COLUMN_INDEX]=new CellLabel("");
					resultTable[lineIndex,APPMConstantes.MAX_COLUMN_INDEX]=new CellLabel("");
					#endregion

					#region Durée de présence & nombre de GRP/semaine active sur cible
//					if(presenceDuration!=null && presenceDuration.Rows.Count>0){
//
//						DataRow presenceData = presenceDuration.Rows[0];
//
//						#region Durée de présence (nombre de semaine actives)
//						avgNbWeek = Convert.ToDouble(presenceData["avgNbWeek"].ToString());
//						minNbWeek = Convert.ToDouble(presenceData["minNbWeek"].ToString());
//						maxNbWeek = Convert.ToDouble(presenceData["maxNbWeek"].ToString());
//						lineIndex = resultTable.AddNewLine(LineType.level1);
//						resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(2083,webSession.SiteLanguage)+" : ");
//						resultTable[lineIndex,APPMConstantes.AVERAGE_COLUMN_INDEX]=new CellNumber(avgNbWeek);
//						resultTable[lineIndex,APPMConstantes.MIN_COLUMN_INDEX]=new CellNumber(minNbWeek);
//						resultTable[lineIndex,APPMConstantes.MAX_COLUMN_INDEX]=new CellNumber(maxNbWeek);
//						#endregion
//
//						#region nombre de GRP/semaine active sur cible
//						avgGRPWeek = Convert.ToDouble(presenceData["avgGRP_Week"].ToString());	
//						minGRPWeek = Convert.ToDouble(presenceData["minGRP_Week"].ToString());	
//						maxGRPWeek = Convert.ToDouble(presenceData["maxGRP_Week"].ToString());	
//						lineIndex = resultTable.AddNewLine(LineType.level2);
//						resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(2084,webSession.SiteLanguage)+" : ");
//						resultTable[lineIndex,APPMConstantes.AVERAGE_COLUMN_INDEX]=new CellNumber(avgGRPWeek);
//						resultTable[lineIndex,APPMConstantes.MIN_COLUMN_INDEX]=new CellNumber(minGRPWeek);
//						resultTable[lineIndex,APPMConstantes.MAX_COLUMN_INDEX]=new CellNumber(maxGRPWeek);
//						#endregion
//
//					}
					#endregion

					#region GRP (cible selectionnée)
					lineIndex = resultTable.AddNewLine(LineType.level1);
					resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(1673,webSession.SiteLanguage)+" : ");
					//nombre de GRP(cible selectionnée) (moyenne)
					avgAdditionalTargetGRP=Math.Round(avgAdditionalTargetGRP,3);
					resultTable[lineIndex,APPMConstantes.AVERAGE_COLUMN_INDEX]=new CellGRP(avgAdditionalTargetGRP);
					//nombre de GRP(cible selectionnée) (min)
					minAdditionalTargetGRP=Math.Round(minAdditionalTargetGRP,3);
					resultTable[lineIndex,APPMConstantes.MIN_COLUMN_INDEX]=new CellGRP(minAdditionalTargetGRP);
					//nombre de GRP(cible selectionnée) (max)
					maxAdditionalTargetGRP=Math.Round(maxAdditionalTargetGRP,3);
					resultTable[lineIndex,APPMConstantes.MAX_COLUMN_INDEX]=new CellGRP(maxAdditionalTargetGRP);
					#endregion

					#region GRP (cible 15 ans et +)
					lineIndex = resultTable.AddNewLine(LineType.level2);
					resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(1673,webSession.SiteLanguage) + " " +  targetBase + " : ");
					//nombre de GRP(cible 15 ans et +) (moyenne)
					resultTable[lineIndex,APPMConstantes.AVERAGE_COLUMN_INDEX]=new CellGRP(Math.Round(avgBaseTargetGRP, 3));
					//nombre de GRP(cible 15 ans et +) (min)
					resultTable[lineIndex,APPMConstantes.MIN_COLUMN_INDEX]=new CellGRP(Math.Round(minBaseTargetGRP, 3));
					//nombre de GRP(cible 15 ans et +) (max)
					maxAdditionalTargetGRP=Math.Round(maxAdditionalTargetGRP,3);
					resultTable[lineIndex,APPMConstantes.MAX_COLUMN_INDEX]=new CellGRP(Math.Round(maxBaseTargetGRP, 3));
					#endregion

					#region Affinité GRP vs cible 15 ans et +
                    //Affinité GRP vs cible 15 ans à + (moyenne)
					lineIndex = resultTable.AddNewLine(LineType.level1);
					resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(2076,webSession.SiteLanguage)+" vs "+targetBase+" : ");
					resultTable[lineIndex,APPMConstantes.AVERAGE_COLUMN_INDEX]=new CellAffinity(avgGRPAffinities);
					//Affinité GRP vs cible 15 ans à + (min) & (max)
					resultTable[lineIndex,APPMConstantes.MIN_COLUMN_INDEX]=new CellAffinity(minGRPAffinities);
					resultTable[lineIndex,APPMConstantes.MAX_COLUMN_INDEX]=new CellAffinity(maxGRPAffinities);
					#endregion

					#region Coût GRP(cible selectionnée)
					// Coût GRP(cible selectionnée) (moyenne)
					lineIndex = resultTable.AddNewLine(LineType.level2);
					resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(1675,webSession.SiteLanguage)+" : ");
					resultTable[lineIndex,APPMConstantes.AVERAGE_COLUMN_INDEX]=new CellCGRP(avgAdditionalTargetCost);
					// Coût GRP(cible selectionnée) (min) & (max)
					resultTable[lineIndex,APPMConstantes.MIN_COLUMN_INDEX]=new CellCGRP(minAdditionalTargetCost);
					resultTable[lineIndex,APPMConstantes.MAX_COLUMN_INDEX]=new CellCGRP(maxAdditionalTargetCost);
					#endregion

					#region Coût GRP(cible 15 et +)
					// Coût GRP(cible 15 et +) (moyenne)
					lineIndex = resultTable.AddNewLine(LineType.level1);
					resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(1675,webSession.SiteLanguage) + " " +  targetBase + " : ");
					resultTable[lineIndex,APPMConstantes.AVERAGE_COLUMN_INDEX]=new CellCGRP(avgBaseTargetCost);
					// Coût GRP(cible 15 et +) (min) & (max)
					resultTable[lineIndex,APPMConstantes.MIN_COLUMN_INDEX]=new CellCGRP(minBaseTargetCost);
					resultTable[lineIndex,APPMConstantes.MAX_COLUMN_INDEX]=new CellCGRP(maxBaseTargetCost);
					#endregion	

					#region Affinité coût GRP vs cible 15 ans et +
					//Affinité coût GRP vs cible 15 ans à + (moyenne)
					lineIndex = resultTable.AddNewLine(LineType.level2);
					resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(2077,webSession.SiteLanguage)+" vs "+targetBase+" : ");
					resultTable[lineIndex,APPMConstantes.AVERAGE_COLUMN_INDEX]=new CellAffinity(avgCGRPAffinities);
					//Affinité coût GRP vs cible 15 ans à + (min) & (max)
					resultTable[lineIndex,APPMConstantes.MIN_COLUMN_INDEX]=new CellAffinity(minCGRPAffinities);
					resultTable[lineIndex,APPMConstantes.MAX_COLUMN_INDEX]=new CellAffinity(maxCGRPAffinities);
					#endregion

					#endregion

				}
			}
			catch(System.Exception err){
				throw(new WebExceptions.SectorDataAverageRulesException("Error while formatting the data for Sector Data Average ",err));
			}

			return resultTable;
		}
		#endregion

	}
}
