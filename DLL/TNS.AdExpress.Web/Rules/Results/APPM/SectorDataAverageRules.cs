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
using TNS.AdExpress.Domain.Units;

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
			string grpFormat = UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.grp).StringFormat;
            string afinityFormat = "{0:max0}", cGrpFormat = "{0:max0}", numberFormat = "{0:max0}";
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
						startDate = WebFnc.Dates.DateToString(new DateTime(int.Parse(dateBeginning.ToString().Substring(0,4)),int.Parse(dateBeginning.ToString().Substring(4,2)),int.Parse(dateBeginning.ToString().Substring(6,2))),webSession.SiteLanguage);						
					if(dateEnding.ToString().Length>0)
						endDate = WebFnc.Dates.DateToString(new DateTime(int.Parse(dateEnding.ToString().Substring(0,4)),int.Parse(dateEnding.ToString().Substring(4,2)),int.Parse(dateEnding.ToString().Substring(6,2))),webSession.SiteLanguage);												
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
							avgBudget+=Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()].ToString());

							if(start)
								minBudget=Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()].ToString());
							else if(minBudget>Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()].ToString()))
								minBudget=Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()].ToString());

							if(maxBudget<Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()].ToString()))
								maxBudget=Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()].ToString());
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
							avgInsertions+=Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString()].ToString());

							if(start)
								minInsertions=Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString()].ToString());
							else if(minInsertions>Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString()].ToString()))
								minInsertions=Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString()].ToString());

							if(maxInsertions<Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString()].ToString()))
								maxInsertions=Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString()].ToString());
							#endregion

							#region Nombre de pages (Moyenne, Min, Max)
							avgPages+=Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString()]);

							if(start)
								minPages=Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString()]);
							else if(minPages>Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString()]))
								minPages=Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString()]);

							if(maxPages<Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString()]))
								maxPages=Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString()]);
							#endregion

							#region Coût GRP (Moyenne, Min, Max)
							if(Convert.ToDouble(dr["totalgrp"].ToString())>0)
								baseTargetCost=Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()].ToString())/Convert.ToDouble(dr["totalgrp"].ToString());
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
								additionalTargetCost=Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()].ToString())/Convert.ToDouble(dr["totalgrp"].ToString());
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
					CellEuro cE = new CellEuro(avgBudget);
					cE.StringFormat = UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.euro).StringFormat;
					resultTable[lineIndex, APPMConstantes.AVERAGE_COLUMN_INDEX] = cE;
					//budget (min)
					CellEuro cE1 = new CellEuro(minBudget);
					cE1.StringFormat = UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.euro).StringFormat;
					resultTable[lineIndex, APPMConstantes.MIN_COLUMN_INDEX] = cE1;
					//budget (max)
					CellEuro cE2 = new CellEuro(maxBudget);
					cE2.StringFormat = UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.euro).StringFormat;
					resultTable[lineIndex, APPMConstantes.MAX_COLUMN_INDEX] = cE2;
					#endregion

					#region Supports
					lineIndex = resultTable.AddNewLine(LineType.level2);
					resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(2108,webSession.SiteLanguage)+" : ");
					//Nombre de supports (moyenne)
					CellNumber cN = new CellNumber(avgNbMedia);
					cN.StringFormat = numberFormat;
					resultTable[lineIndex, APPMConstantes.AVERAGE_COLUMN_INDEX] = cN;
					//Nombre de supports (min)
					CellNumber cN1 = new CellNumber(minNbMedia);
					cN1.StringFormat = numberFormat;
					resultTable[lineIndex, APPMConstantes.MIN_COLUMN_INDEX] = cN1;
					//Nombre de supports (max)
					CellNumber cN2 = new CellNumber(maxNbMedia);
					cN2.StringFormat = numberFormat;
					resultTable[lineIndex, APPMConstantes.MAX_COLUMN_INDEX] = cN2;
					#endregion

					string insertionFormat = UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.insertion).StringFormat;
					#region Insertions
					lineIndex = resultTable.AddNewLine(LineType.level1);
					resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(1398,webSession.SiteLanguage)+" : ");
					//nombre d'insertions (moyenne)
					CellInsertion cI = new CellInsertion(avgInsertions);
					cI.StringFormat = insertionFormat;
					resultTable[lineIndex, APPMConstantes.AVERAGE_COLUMN_INDEX] = cI;
					//nombre d'insertions (min)
					CellInsertion cI1 = new CellInsertion(minInsertions);
					cI1.StringFormat = insertionFormat;
					resultTable[lineIndex, APPMConstantes.MIN_COLUMN_INDEX] = cI1;
					//nombre d'insertions (max)
					CellInsertion cI2 = new CellInsertion(maxInsertions);
					cI2.StringFormat = insertionFormat;
					resultTable[lineIndex, APPMConstantes.MAX_COLUMN_INDEX] = cI2;
					#endregion

					string pageFormat = UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.pages).StringFormat;
					#region Pages
					lineIndex = resultTable.AddNewLine(LineType.level2);
					resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(1385,webSession.SiteLanguage)+" : ");
					//nombre de pages utilisés (moyenne)
					avgPages=Math.Round(avgPages,3);
					CellPage cP = new CellPage(avgPages);
					cP.StringFormat = pageFormat;
					resultTable[lineIndex, APPMConstantes.AVERAGE_COLUMN_INDEX] = cP;
					//nombre de pages utilisés (min)
					minPages=Math.Round(minPages,3);
					CellPage cP1 = new CellPage(minPages);
					cP1.StringFormat = pageFormat;
					resultTable[lineIndex, APPMConstantes.MIN_COLUMN_INDEX] = cP1;
					//nombre de pages utilisés (max)
					maxPages=Math.Round(maxPages,3);
					CellPage cP2 = new CellPage(maxPages);
					cP2.StringFormat = pageFormat;
					resultTable[lineIndex, APPMConstantes.MAX_COLUMN_INDEX] = cP2;
					#endregion

					#region Ligne blanche
					lineIndex = resultTable.AddNewLine(LineType.level3);
					resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel("");
					resultTable[lineIndex,APPMConstantes.AVERAGE_COLUMN_INDEX]=new CellLabel("");
					resultTable[lineIndex,APPMConstantes.MIN_COLUMN_INDEX]=new CellLabel("");
					resultTable[lineIndex,APPMConstantes.MAX_COLUMN_INDEX]=new CellLabel("");
					#endregion

					
					#region GRP (cible selectionnée)
					lineIndex = resultTable.AddNewLine(LineType.level1);
					resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(1673,webSession.SiteLanguage)+" : ");
					//nombre de GRP(cible selectionnée) (moyenne)
					avgAdditionalTargetGRP=Math.Round(avgAdditionalTargetGRP,3);
					CellGRP cGrp = new CellGRP(avgAdditionalTargetGRP);
					cGrp.StringFormat = grpFormat;
					resultTable[lineIndex, APPMConstantes.AVERAGE_COLUMN_INDEX] = cGrp;
					//nombre de GRP(cible selectionnée) (min)
					minAdditionalTargetGRP=Math.Round(minAdditionalTargetGRP,3);
					CellGRP cGrp1 = new CellGRP(minAdditionalTargetGRP);
					cGrp1.StringFormat = grpFormat;
					resultTable[lineIndex, APPMConstantes.MIN_COLUMN_INDEX] = cGrp1;
					//nombre de GRP(cible selectionnée) (max)
					maxAdditionalTargetGRP=Math.Round(maxAdditionalTargetGRP,3);
					CellGRP cGrp2 = new CellGRP(maxAdditionalTargetGRP);
					cGrp2.StringFormat = grpFormat;
					resultTable[lineIndex, APPMConstantes.MAX_COLUMN_INDEX] = cGrp2;
					#endregion

					#region GRP (cible 15 ans et +)
					lineIndex = resultTable.AddNewLine(LineType.level2);
					resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(1673,webSession.SiteLanguage) + " " +  targetBase + " : ");
					//nombre de GRP(cible 15 ans et +) (moyenne)
					CellGRP cGrp3 = new CellGRP(Math.Round(avgBaseTargetGRP, 3));
					cGrp3.StringFormat = grpFormat;
					resultTable[lineIndex, APPMConstantes.AVERAGE_COLUMN_INDEX] = cGrp3;
					//nombre de GRP(cible 15 ans et +) (min)
					CellGRP cGrp4 = new CellGRP(Math.Round(minBaseTargetGRP, 3));
					cGrp4.StringFormat = grpFormat;
					resultTable[lineIndex, APPMConstantes.MIN_COLUMN_INDEX] = cGrp4;
					//nombre de GRP(cible 15 ans et +) (max)
					maxAdditionalTargetGRP=Math.Round(maxAdditionalTargetGRP,3);
					CellGRP cGrp5 = new CellGRP(Math.Round(maxBaseTargetGRP, 3));
					cGrp5.StringFormat = grpFormat;
					resultTable[lineIndex, APPMConstantes.MAX_COLUMN_INDEX] = cGrp5;
					#endregion

					#region Affinité GRP vs cible 15 ans et +
                    //Affinité GRP vs cible 15 ans à + (moyenne)
					lineIndex = resultTable.AddNewLine(LineType.level1);
					resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(2076,webSession.SiteLanguage)+" vs "+targetBase+" : ");
					CellAffinity cAf = new CellAffinity(avgGRPAffinities);
					cAf.StringFormat = afinityFormat;
					resultTable[lineIndex, APPMConstantes.AVERAGE_COLUMN_INDEX] = cAf;
					//Affinité GRP vs cible 15 ans à + (min) & (max)
					CellAffinity cAf1 = new CellAffinity(minGRPAffinities);
					cAf1.StringFormat = afinityFormat;
					resultTable[lineIndex, APPMConstantes.MIN_COLUMN_INDEX] = cAf1;
					CellAffinity cAf2 = new CellAffinity(maxGRPAffinities);
					cAf2.StringFormat = afinityFormat;
					resultTable[lineIndex, APPMConstantes.MAX_COLUMN_INDEX] = cAf2;
					#endregion

					#region Coût GRP(cible selectionnée)
					// Coût GRP(cible selectionnée) (moyenne)
					lineIndex = resultTable.AddNewLine(LineType.level2);
					resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(1675,webSession.SiteLanguage)+" : ");
					CellCGRP cCgrp = new CellCGRP(avgAdditionalTargetCost);
					cCgrp.StringFormat = cGrpFormat;
					resultTable[lineIndex, APPMConstantes.AVERAGE_COLUMN_INDEX] = cCgrp;
					// Coût GRP(cible selectionnée) (min) & (max)
					CellCGRP cCgrp1 = new CellCGRP(minAdditionalTargetCost);
					cCgrp1.StringFormat = cGrpFormat;
					resultTable[lineIndex, APPMConstantes.MIN_COLUMN_INDEX] = cCgrp1;
					CellCGRP cCgrp2 = new CellCGRP(maxAdditionalTargetCost);
					cCgrp2.StringFormat = cGrpFormat;
					resultTable[lineIndex, APPMConstantes.MAX_COLUMN_INDEX] = cCgrp2;
					#endregion

					#region Coût GRP(cible 15 et +)
					// Coût GRP(cible 15 et +) (moyenne)
					lineIndex = resultTable.AddNewLine(LineType.level1);
					resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(1675,webSession.SiteLanguage) + " " +  targetBase + " : ");
					CellCGRP cCgrp3 = new CellCGRP(avgBaseTargetCost);
					cCgrp3.StringFormat = cGrpFormat;
					resultTable[lineIndex, APPMConstantes.AVERAGE_COLUMN_INDEX] = cCgrp3;
					// Coût GRP(cible 15 et +) (min) & (max)
					CellCGRP cCgrp4 = new CellCGRP(minBaseTargetCost);
					cCgrp4.StringFormat = cGrpFormat;
					resultTable[lineIndex,APPMConstantes.MIN_COLUMN_INDEX]= cCgrp4;
					CellCGRP cCgrp5 = new CellCGRP(maxBaseTargetCost);
					cCgrp5.StringFormat = cGrpFormat;
					resultTable[lineIndex, APPMConstantes.MAX_COLUMN_INDEX] = cCgrp5;
					#endregion	

					#region Affinité coût GRP vs cible 15 ans et +
					//Affinité coût GRP vs cible 15 ans à + (moyenne)
					lineIndex = resultTable.AddNewLine(LineType.level2);
					resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(2077,webSession.SiteLanguage)+" vs "+targetBase+" : ");
					CellAffinity cAf3 = new CellAffinity(avgCGRPAffinities);
					cAf3.StringFormat = afinityFormat;
					resultTable[lineIndex, APPMConstantes.AVERAGE_COLUMN_INDEX] = cAf3;
					//Affinité coût GRP vs cible 15 ans à + (min) & (max)
					CellAffinity cAf4 = new CellAffinity(minCGRPAffinities);
					cAf4.StringFormat = afinityFormat;
					resultTable[lineIndex, APPMConstantes.MIN_COLUMN_INDEX] = cAf4;
					CellAffinity cAf5 = new CellAffinity(maxCGRPAffinities);
					cAf5.StringFormat = afinityFormat;
					resultTable[lineIndex, APPMConstantes.MAX_COLUMN_INDEX] = cAf5;
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
