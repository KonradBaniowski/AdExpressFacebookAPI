#region Informations
// Auteur: D. V. Mussuma 
// Date de création: 30/09/2004  
// Date de modification: 24/11/2004 
//		10/05/2005	K.Shehzad	Chagement d'en tête Excel
//		12/08/2005	G. Facon	Nom de fonction
//	24/10/2005	D. V. Mussuma	Intégration unité Keuros	
#endregion

using System.Web;

using System;
using System.Collections;
using System.Web.UI;
using TNS.AdExpress.Constantes.FrameWork.Results;
using CstWeb = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Exceptions;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.FrameWork.Date;
using WebRules=TNS.AdExpress.Web.Rules.Results;
using DateFunctions = TNS.FrameWork.Date;
using ConstResults=TNS.AdExpress.Constantes.FrameWork.Results;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using System.Windows.Forms;
using System.Data;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
using TNS.FrameWork;
using TNS.Classification.Universe;
using TNS.AdExpress.Classification;


namespace TNS.AdExpress.Web.UI.Results{
	/// <summary>
	/// Interface Utilisateur d'un Indicateur de Saisonnalité dans analyse sectorielle
	/// Cette classe génère suivant le format de sortie le code pour afficher un tableau	
	/// </summary>
	public class IndicatorSeasonalityUI{	
			
		#region Sortie HTML (Web)

		#region UI saisonnalité
		/// <summary>
		/// Génère le code HTML sur le total support de l'univers,marché ou famille, ou sur les
		/// annonceurs de références ou concurrents :
		/// -les investissements de l'année N
		/// -une évolution N vs N-1 (uniquement dans le ca d'une étude comparative)
		/// -le nombre de référence
		/// -le budget moyen par référence
		/// -le 1er annonceur en Ke et SOV (uniquement pour les lignes total produits éventuels)
		/// -la 1ere référence en Ke et SOV (uniquement pour les lignes total produits éventuels)
		/// </summary>		
		/// <param name="webSession">Session du client</param>
		/// <param name="page">Page qui affiche le Plan Média</param>
		/// <param name="tabResult">tableau de résultats</param>
		/// <param name="excel">sortie des résultats sous tableur excel</param>
		/// <returns>Code HTML</returns>
		//public static string getIndicatorSeasonalityHtmlUI(Page page,object[,] tab,WebSession webSession){
		public static string GetIndicatorSeasonalityHtmlUI(Page page,object[] tabResult,WebSession webSession,bool excel){	
			
			#region variables
			int nbMonths=0;
			int TAB_MAX_COLUMN_INDEX=0;
			int TAB_MAX_LINE_INDEX=0;
			Int64 TOTAL_UNIVERS_LINE_INDEX=0;
			string adclasse;
			string adclasse2;
			string AdvertiserAccessList="";
			string[] AdvertiserSplitter={","};
			string CompetitorAdvertiserAccessList="";			
			ArrayList AdvertiserAccessListArr=null;
			ArrayList CompetitorAdvertiserAccessListArr=null;
			//ligne courante
			Int64 currentLine=1;								
			//mois 
			int currentmonth=0;
			//couleur 
			int color=1;												
			//nombre maximal de lignes
			Int64 maxLinesToTreat=0;										
			object[,] tab =null;	
			object[,] tabTotal = null;
			object[,] tabTotalUniverse=null;
			int rowspan=0;	
			int nbAdvertiser=0;
			DataTable dtTotalMarket=null;
			DataRow[] foundRows = null; 
			string strExpr;
			string strSort;
			string image="/I/p.gif";
			NomenclatureElementsGroup nomenclatureElementsGroup = null;
			string tempListAsString = "";
 			#endregion
			
			#region constantes
			const string competitorExcelStyle="p142";
			const string competitorExcelStyleNB="p143";
			#endregion
			
			#region Pas de données à afficher
			if(tabResult.GetLength(0)==0 ){
				return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
					+"</div>");
			}			
			#endregion
							
			#region construction du tableau de saisonnalité
			//Intialisation des variables
			tab = (object[,])tabResult[0];	
			tabTotal = (object[,])tabResult[1];
			tabTotalUniverse=(object[,])tabResult[2];
			if(tab!=null){
				TAB_MAX_COLUMN_INDEX=tab.GetLength(1);
				TAB_MAX_LINE_INDEX=tab.GetLength(0);
			}
			else if(tabTotal!=null && (tab==null || tab.GetLength(0)==0)){
				TAB_MAX_COLUMN_INDEX=tabTotal.GetLength(1);
				TAB_MAX_LINE_INDEX=tabTotal.GetLength(0);
			}

			#region identiant annonceurs référence et concurrents
			if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(0)) {
				nomenclatureElementsGroup = webSession.SecondaryProductUniverses[0].GetGroup(0);
				if (nomenclatureElementsGroup != null && nomenclatureElementsGroup.Count() > 0) {
					tempListAsString = nomenclatureElementsGroup.GetAsString(TNSClassificationLevels.ADVERTISER);
					if (tempListAsString != null && tempListAsString.Length > 0) AdvertiserAccessList = tempListAsString;
				}
			}
			//AdvertiserAccessList = webSession.GetSelection(webSession.ReferenceUniversAdvertiser,CustomerRightConstante.type.advertiserAccess);		
			//if(webSession.CompetitorUniversAdvertiser[0]!=null){
			//    CompetitorAdvertiserAccessList = webSession.GetSelection((TreeNode)webSession.CompetitorUniversAdvertiser[0],CustomerRightConstante.type.advertiserAccess);		
			//}
			if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(1)) {
				nomenclatureElementsGroup = webSession.SecondaryProductUniverses[1].GetGroup(0);
				if (nomenclatureElementsGroup != null && nomenclatureElementsGroup.Count() > 0) {
					tempListAsString = nomenclatureElementsGroup.GetAsString(TNSClassificationLevels.ADVERTISER);
					if (tempListAsString != null && tempListAsString.Length > 0) CompetitorAdvertiserAccessList = tempListAsString;
				}
			}

			#region recuperation éléments références et concurrents
			if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserAccessList)){				
				AdvertiserAccessListArr = new ArrayList(AdvertiserAccessList.Split(','));
			}
			if(WebFunctions.CheckedText.IsStringEmpty(CompetitorAdvertiserAccessList)){
				CompetitorAdvertiserAccessListArr = new ArrayList(CompetitorAdvertiserAccessList.Split(','));
			}
			
			#endregion
			#endregion

			#region Compte le nombre de mois d'etude			
			
			//Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
			//du dernier mois dispo en BDD
			//traitement de la notion de fréquence
			string absolutEndPeriod = WebFunctions.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
			if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
				throw new NoDataException();
			
			
			DateTime PeriodBeginningDate = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
			DateTime PeriodEndDate = WebFunctions.Dates.getPeriodEndDate(absolutEndPeriod, webSession.PeriodType);
			nbMonths =  (int)PeriodEndDate.Month - (int)PeriodBeginningDate.Month +1 ;
			currentmonth=(int)PeriodBeginningDate.Month;
			int month=PeriodBeginningDate.Month;
			//Nombre de lignes à traiter
			maxLinesToTreat=nbMonths;
			int maxLineToTreat=nbMonths;
			#endregion
			if(tab!=null){
				nbAdvertiser =(nbMonths>0)?(TAB_MAX_LINE_INDEX-1)/nbMonths : 0;
			}else if((tabTotal!=null || tabTotalUniverse!=null) && (tab==null || tab.GetLength(0)==0)){
				nbAdvertiser =  0;
			}
			//index ligne total univers			
			TOTAL_UNIVERS_LINE_INDEX=1;
			//index ligne total marché ou famille
			Int64 TOTAL_SECTOR_LINE_INDEX=1;	
			//rowspan
			if( ((tabTotal == null || tabTotal.GetLength(0)==0) || (tabResult==null || tabResult[3]==null && dtTotalMarket==null))&& (tabTotalUniverse==null || tabTotalUniverse.GetLength(0)==0))
			rowspan=nbAdvertiser+1;	
			else if ( (tabTotal == null ) && (tabResult!=null && tabResult[3]==null ) && (tabTotalUniverse!=null || tabTotalUniverse.GetLength(0)>0) )rowspan=nbAdvertiser+1;
			else if (((tabTotal != null && tabTotal.GetLength(0)>0 ) || (tabResult!=null || tabResult[3]!=null && (dtTotalMarket!=null && dtTotalMarket.Rows.Count>0)) ) && (tabTotalUniverse==null || tabTotalUniverse.GetLength(0)==0) )rowspan=nbAdvertiser+1;
			else rowspan=nbAdvertiser+2;

			System.Text.StringBuilder t=new System.Text.StringBuilder(50000);									
			//Debut tableau
			t.Append("<TABLE border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"p2\"><TR><TD>");			
			//Debut tableau mensuel
			t.Append("<TABLE border=\"0\" cellpadding=\"0\" cellspacing=\"0\">");

			#region ligne libellés
			//Ligne Libellés tableau
			t.Append("<tr ><td  colspan=2 class=\"p2\"></td><td nowrap class=\"p2\" >"+GestionWeb.GetWebWord(1156,webSession.SiteLanguage)+"  "+webSession.PeriodBeginningDate.Substring(0,4).ToString()+"</td>");
			//Colonne evolution (optionnelle)
			if(webSession.ComparativeStudy) t.Append("<td  nowrap  class=\"p2\">"+GestionWeb.GetWebWord(1168,webSession.SiteLanguage)+"&nbsp;"+webSession.PeriodBeginningDate.Substring(0,4).ToString()+"/"+(int.Parse(webSession.PeriodBeginningDate.Substring(0,4))-1).ToString()+"</td>");
			t.Append("<td  nowrap  class=\"p2\">"+GestionWeb.GetWebWord(1152,webSession.SiteLanguage)+"</td>");
			t.Append("<td  nowrap  class=\"p2\">"+GestionWeb.GetWebWord(1153,webSession.SiteLanguage)+"</td>");
			//Colonne separation 
			if(!excel){
				t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=1px></td>");
			}
			//Colonnes 1er annonceurs (optionnels)		
			t.Append("<td  nowrap  class=\"p2\" colspan=2>"+GestionWeb.GetWebWord(1154,webSession.SiteLanguage)+"</td>");				
		//	t.Append("<!--<td  nowrap  class=\"p2\"></td>-->");				
			t.Append("<td  nowrap  class=\"p2\">"+GestionWeb.GetWebWord(437,webSession.SiteLanguage)+"</td>");
			//Colonne separation 
			if(!excel){
				t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=1px></td>");
			}
			//Colonnes 1er références (optionnels)		
			t.Append("<td  nowrap  class=\"p2\" colspan=2>"+GestionWeb.GetWebWord(1155,webSession.SiteLanguage)+"</td>");				
		//	t.Append("<!--<td  nowrap  class=\"p2\"></td>-->");				
			t.Append("<td  nowrap  class=\"p2\">"+GestionWeb.GetWebWord(437,webSession.SiteLanguage)+"</td>");
			t.Append("</tr>");
			#endregion

			//Pour chaque mois de la période sélectionnée
			for(int m=1;m<=nbMonths;m++){	
				if(WebFunctions.CalculationType.IsSectorTotalCriterion(webSession.ComparaisonCriterion) && tabTotal!=null && !(tabTotal.GetLength(0)==0)){
					#region ligne total  famille
					 image="/I/p.gif";			
					//Ligne Total Famille 
					t.Append("<tr><td   class=\"p2\" nowrap rowspan="+rowspan+">"+DateFunctions.MonthString.GetHTMLCharacters(currentmonth,33,0).ToString()+"</td>");
					if(WebFunctions.CalculationType.IsMarketTotalCriterion(webSession.ComparaisonCriterion))t.Append("<td nowrap  class=\"pmtotal\" >"+GestionWeb.GetWebWord(1190,webSession.SiteLanguage)+"</td>");
					else t.Append("<td nowrap  class=\"pmtotal\" >"+GestionWeb.GetWebWord(1189,webSession.SiteLanguage)+"</td>");			
					if (tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX]!=null)
//						t.Append("<td nowrap  bgcolor=#B1A3C1 class=\"pmtotalnb\" >"+String.Format("{0:### ### ### ### ##0.## }",Decimal.Parse(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX].ToString())/(Decimal)1000)+"</td>");
						t.Append("<td nowrap  bgcolor=#B1A3C1 class=\"pmtotalnb\" >"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX].ToString(),webSession.Unit,false)+"</td>");
					else t.Append("<td nowrap bgcolor=#B1A3C1 class=\"pmtotalnb\" >-</td>");
					if(webSession.ComparativeStudy){
						if (tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX]!=null){
							if(Decimal.Parse(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX].ToString())>(Decimal)0.0)image="/I/g.gif";
							else if (Decimal.Parse(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX].ToString())<(Decimal)0.0)image="/I/r.gif";
							else if (Decimal.Parse(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX].ToString())==(Decimal)0.0)image="/I/o.gif";							
							if(!excel){
								t.Append("<td  nowrap  bgcolor=#B1A3C1 class=\"pmtotalnb\">"+Decimal.Parse(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+"% &nbsp;<img src="+image+"></td>");
							}else{
								t.Append("<td  nowrap  bgcolor=#B1A3C1 class=\"pmtotalnb\">"+Decimal.Parse(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+"%</td>");
							}
						}
						else t.Append("<td  nowrap bgcolor=#B1A3C1  class=\"pmtotalnb\">-</td>");
					}
					if(tabTotal[TOTAL_SECTOR_LINE_INDEX, ConstResults.Seasonality.REFERENCE_COLUMN_INDEX]!=null)
						t.Append("<td  nowrap class=\"pmtotalnb\">"+Decimal.Parse(tabTotal[TOTAL_SECTOR_LINE_INDEX, ConstResults.Seasonality.REFERENCE_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+"</td>");
					else t.Append("<td  nowrap class=\"pmtotalnb\">-</td>");					
					if(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.AVERAGE_BUDGET_COLUMN_INDEX]!=null)
//						t.Append("<td  nowrap class=\"pmtotalnb\" >"+String.Format("{0:### ### ### ### ##0.## }",Decimal.Parse(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.AVERAGE_BUDGET_COLUMN_INDEX].ToString())/(Decimal)1000)+"</td>");
						t.Append("<td  nowrap class=\"pmtotalnb\" >"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.AVERAGE_BUDGET_COLUMN_INDEX].ToString(),webSession.Unit,false)+"</td>");
					else t.Append("<td  nowrap class=\"pmtotalnb\" >-</td>");
					//Colonne separation 
					if(!excel){
						t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=1px></td>");		
					}
					//Colonnes 1er annonceur (optionnels)
					if(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.FIRST_ADVERTISER_COLUMN_INDEX]!=null && (Decimal.Parse(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX].ToString())/(Decimal)1000)>(Decimal)0.0)
					t.Append("<td nowrap class=\"pmtotal\" >"+tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.FIRST_ADVERTISER_COLUMN_INDEX].ToString()+"</td>");				
					else t.Append("<td nowrap class=\"pmtotal\" >-</td>");				
					if(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.FIRST_ADVERTISER_INVEST_COLUMN_INDEX]!=null && (Decimal.Parse(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX].ToString())/(Decimal)1000)>(Decimal)0.0)
//					t.Append("<td nowrap class=\"pmtotalnb\"  >"+String.Format("{0:### ### ### ### ##0.## }",Decimal.Parse(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.FIRST_ADVERTISER_INVEST_COLUMN_INDEX].ToString())/(Decimal)1000)+"</td>");				
						t.Append("<td nowrap class=\"pmtotalnb\"  >"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.FIRST_ADVERTISER_INVEST_COLUMN_INDEX].ToString(),webSession.Unit,false)+"</td>");				
					else t.Append("<td nowrap class=\"pmtotalnb\"  >-</td>");	
					if(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.FIRST_ADVERTISER_SOV_COLUMN_INDEX]!=null && (Decimal.Parse(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX].ToString())/(Decimal)1000)>(Decimal)0.0)
					t.Append("<td class=\"pmtotalnb\" >"+Decimal.Parse(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.FIRST_ADVERTISER_SOV_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+"%</td>");				
					else t.Append("<td class=\"pmtotalnb\" >-</td>");
					//Colonne separation 
					if(!excel){
						t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=1px></td>");		
					}
					//Colonnes 1ere référence (optionnels)		
					if(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.FIRST_PRODUCT_COLUMN_INDEX]!=null && (Decimal.Parse(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX].ToString())/(Decimal)1000)>(Decimal)0.0)
					t.Append("<td nowrap class=\"pmtotal\" >"+tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.FIRST_PRODUCT_COLUMN_INDEX].ToString()+"</td>");				
					else t.Append("<td nowrap class=\"pmtotal\" >-</td>");				
					if(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.FIRST_PRODUCT_INVEST_COLUMN_INDEX]!=null && (Decimal.Parse(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX].ToString())/(Decimal)1000)>(Decimal)0.0)
//					t.Append("<td nowrap class=\"pmtotalnb\" >"+String.Format("{0:### ### ### ### ##0.## }",Decimal.Parse(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.FIRST_PRODUCT_INVEST_COLUMN_INDEX].ToString())/(Decimal)1000)+"</td>");				
						t.Append("<td nowrap class=\"pmtotalnb\" >"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.FIRST_PRODUCT_INVEST_COLUMN_INDEX].ToString(),webSession.Unit,false)+"</td>");				
					else t.Append("<td nowrap class=\"pmtotalnb\" ></td>");				
					if(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.FIRST_PRODUCT_SOV_COLUMN_INDEX]!=null && (Decimal.Parse(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX].ToString())/(Decimal)1000)>(Decimal)0.0)
					t.Append("<td nowrap class=\"pmtotalnb\" >"+Decimal.Parse(tabTotal[TOTAL_SECTOR_LINE_INDEX,ConstResults.Seasonality.FIRST_PRODUCT_SOV_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+"%</td></tr>");
					else t.Append("<td nowrap class=\"pmtotalnb\" >-</td></tr>");			
				//	t.Append("\n");				
					#endregion
				}
				else if(tabResult!=null && tabResult[3]!=null && webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.marketTotal){
					dtTotalMarket =(DataTable)tabResult[3];
					if(dtTotalMarket!=null && dtTotalMarket.Rows.Count>0){
								
						strExpr = "MOIS = "+month;
						strSort = "MOIS ASC";
						foundRows = dtTotalMarket.Select( strExpr, strSort, DataViewRowState.OriginalRows);																	
						#region ligne total marché
						 image="/I/p.gif";		
						//Ligne Total  marché
						t.Append("<tr><td nowrap rowspan="+rowspan+" class=\"p2\"  >"+DateFunctions.MonthString.GetHTMLCharacters(currentmonth,33,0).ToString()+"</td>");
						t.Append("<td nowrap  class=\"pmtotal\" >"+GestionWeb.GetWebWord(1190,webSession.SiteLanguage)+"</td>");					
						if (foundRows!=null && foundRows.Length>0 && foundRows[0]!=null && foundRows[0]["TOTAL_N"]!=null )
//							t.Append("<td nowrap  bgcolor=#B1A3C1 class=\"pmtotalnb\" >"+String.Format("{0:### ### ### ### ##0.## }",Decimal.Parse(foundRows[0]["TOTAL_N"].ToString())/(Decimal)1000)+"</td>");
							t.Append("<td nowrap  bgcolor=#B1A3C1 class=\"pmtotalnb\" >"+WebFunctions.Units.ConvertUnitValueAndPdmToString(foundRows[0]["TOTAL_N"].ToString(),webSession.Unit,false)+"</td>");
						else t.Append("<td nowrap bgcolor=#B1A3C1 class=\"pmtotalnb\" >-</td>");
						if(webSession.ComparativeStudy){
							if (foundRows!=null && foundRows.Length>0 && foundRows[0]!=null && foundRows[0]["EVOL"]!=null){
								if(Decimal.Parse(foundRows[0]["EVOL"].ToString())>(Decimal)0.0)image="/I/g.gif";
								else if (Decimal.Parse(foundRows[0]["EVOL"].ToString())<(Decimal)0.0)image="/I/r.gif";
								else if (Decimal.Parse(foundRows[0]["EVOL"].ToString())==(Decimal)0.0)image="/I/o.gif";								
								if(!excel){
									t.Append("<td  nowrap  bgcolor=#B1A3C1 class=\"pmtotalnb\">"+Decimal.Parse(foundRows[0]["EVOL"].ToString()).ToString("### ### ### ### ##0.##")+"% &nbsp;<img src="+image+"></td>");
								}else{
									t.Append("<td  nowrap  bgcolor=#B1A3C1 class=\"pmtotalnb\">"+Decimal.Parse(foundRows[0]["EVOL"].ToString()).ToString("### ### ### ### ##0.##")+"%</td>");
								}
							}
							else t.Append("<td  nowrap bgcolor=#B1A3C1  class=\"pmtotalnb\">-</td>");
						}
						if(foundRows!=null && foundRows.Length>0 && foundRows[0]!=null && foundRows[0]["NBREF"]!=null)
							t.Append("<td  nowrap class=\"pmtotalnb\">"+Decimal.Parse(foundRows[0]["NBREF"].ToString()).ToString("### ### ### ### ##0.##")+"</td>");
						else t.Append("<td  nowrap class=\"pmtotalnb\">-</td>");					
						if(foundRows!=null && foundRows.Length>0 && foundRows[0]!=null && foundRows[0]["BUDGET_MOYEN"]!=null)
//							t.Append("<td  nowrap class=\"pmtotalnb\" >"+String.Format("{0:### ### ### ### ##0.## }",Decimal.Parse(foundRows[0]["BUDGET_MOYEN"].ToString())/(Decimal)1000)+"</td>");
							t.Append("<td  nowrap class=\"pmtotalnb\" >"+WebFunctions.Units.ConvertUnitValueAndPdmToString(foundRows[0]["BUDGET_MOYEN"].ToString(),webSession.Unit,false)+"</td>");
						else t.Append("<td  nowrap class=\"pmtotalnb\" >-</td>");
						//Colonne separation 
						if(!excel){
							t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=1px></td>");
						}
						//Colonnes 1er annonceur (optionnels)
						if(foundRows!=null && foundRows.Length>0 && foundRows[0]!=null && foundRows[0]["ADVERTISER"]!=null)t.Append("<td nowrap class=\"pmtotal\" >"+foundRows[0]["ADVERTISER"].ToString()+"</td>");				
						else t.Append("<td nowrap class=\"pmtotal\" >-</td>");				
						if(foundRows!=null && foundRows.Length>0 && foundRows[0]!=null && foundRows[0]["INVESTMENT_ADVERTISER"]!=null)
//							t.Append("<td nowrap class=\"pmtotalnb\"  >"+String.Format("{0:### ### ### ### ##0.## }",Decimal.Parse(foundRows[0]["INVESTMENT_ADVERTISER"].ToString())/(Decimal)1000)+"</td>");				
							t.Append("<td nowrap class=\"pmtotalnb\"  >"+WebFunctions.Units.ConvertUnitValueAndPdmToString(foundRows[0]["INVESTMENT_ADVERTISER"].ToString(),webSession.Unit,false)+"</td>");				
						else t.Append("<td nowrap class=\"pmtotalnb\"  >-</td>");
						if(foundRows!=null && foundRows.Length>0 && foundRows[0]!=null && foundRows[0]["SOV_FIRST_ADVERTISER"]!=null)t.Append("<td class=\"pmtotalnb\" >"+Decimal.Parse(foundRows[0]["SOV_FIRST_ADVERTISER"].ToString()).ToString("### ### ### ### ##0.##")+"%</td>");				
						else t.Append("<td class=\"pmtotalnb\" >-</td>");
						//Colonne separation 
						if(!excel){
							t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=1px></td>");
						}
						//Colonnes 1ere référence (optionnels)		
						if(foundRows!=null && foundRows.Length>0 && foundRows[0]!=null && foundRows[0]["PRODUCT"]!=null)t.Append("<td nowrap class=\"pmtotal\" >"+foundRows[0]["PRODUCT"].ToString()+"</td>");				
						else t.Append("<td nowrap class=\"pmtotal\" >-</td>");				
						if(foundRows!=null && foundRows.Length>0 && foundRows[0]!=null && foundRows[0]["INVESTMENT_PRODUCT"]!=null)
//							t.Append("<td nowrap class=\"pmtotalnb\" >"+String.Format("{0:### ### ### ### ##0.## }",Decimal.Parse(foundRows[0]["INVESTMENT_PRODUCT"].ToString())/(Decimal)1000)+"</td>");				
							t.Append("<td nowrap class=\"pmtotalnb\" >"+WebFunctions.Units.ConvertUnitValueAndPdmToString(foundRows[0]["INVESTMENT_PRODUCT"].ToString(),webSession.Unit,false)+"</td>");				
						else t.Append("<td nowrap class=\"pmtotalnb\" ></td>");				
						if(foundRows!=null && foundRows.Length>0 && foundRows[0]!=null && foundRows[0]["SOV_FIRST_PRODUCT"]!=null)
						t.Append("<td nowrap class=\"pmtotalnb\" >"+Decimal.Parse(foundRows[0]["SOV_FIRST_PRODUCT"].ToString()).ToString("### ### ### ### ##0.##")+"%</td></tr>");
						else t.Append("<td nowrap class=\"pmtotalnb\" >-</td></tr>");			
					//	t.Append("\n");	
					}
					#endregion
				}
					#region ligne total  Univers
					//Ligne Total univers	
					if(tabTotalUniverse!=null && !(tabTotalUniverse.GetLength(0)==0)){	
						 image="/I/p.gif";	
						t.Append("<tr>");
						if( (tabTotal == null || tabTotal.GetLength(0)==0) &&  (tabResult==null || tabResult[3]==null || foundRows==null) )
							t.Append("<td  nowrap rowspan="+rowspan+"  class=\"p2\">"+DateFunctions.MonthString.GetHTMLCharacters(currentmonth,33,0).ToString()+"</td>");										
						t.Append("<td nowrap class=\"pmtotal\"  >"+GestionWeb.GetWebWord(1188,webSession.SiteLanguage)+"</td>");			
//						t.Append("<td nowrap  class=\"pmtotalnb\">"+String.Format("{0:### ### ### ### ##0.## }",Decimal.Parse(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX].ToString())/(Decimal)1000)+"</td>");
						t.Append("<td nowrap  class=\"pmtotalnb\">"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX].ToString(),webSession.Unit,false)+"</td>");
						if(webSession.ComparativeStudy) {
							if(Decimal.Parse(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX].ToString())>(Decimal)0.0)image="/I/g.gif";
							else if (Decimal.Parse(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX].ToString())<(Decimal)0.0)image="/I/r.gif";
							else if (Decimal.Parse(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX].ToString())==(Decimal)0.0)image="/I/o.gif";							
							if(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX]!=null){
								if(Decimal.Parse(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX].ToString())>(Decimal)0.0)image="/I/g.gif";
								else if (Decimal.Parse(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX].ToString())<(Decimal)0.0)image="/I/r.gif";
								else if (Decimal.Parse(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX].ToString())==(Decimal)0.0)image="/I/o.gif";							
								if(!excel){
									t.Append("<td nowrap  class=\"pmtotalnb\">"+Decimal.Parse(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+"% &nbsp;<img src="+image+"></td>");
								}else{
									t.Append("<td nowrap  class=\"pmtotalnb\">"+Decimal.Parse(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+"%</td>");
								}
							}
							else t.Append("<td nowrap  class=\"pmtotalnb\">-</td>");
						}
						t.Append("<td nowrap  class=\"pmtotalnb\">"+tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.REFERENCE_COLUMN_INDEX].ToString()+"</td>");
//						t.Append("<td nowrap  class=\"pmtotalnb\">"+String.Format("{0:### ### ### ### ##0.## }",Decimal.Parse(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.AVERAGE_BUDGET_COLUMN_INDEX].ToString())/(Decimal)1000)+"</td>");
						t.Append("<td nowrap  class=\"pmtotalnb\">"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.AVERAGE_BUDGET_COLUMN_INDEX].ToString(),webSession.Unit,false)+"</td>");
						
						//Colonne separation 
						if(!excel){
							t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=1px></td>");
						}
						//Colonnes 1er annonceur (optionnels)		
						if(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.FIRST_ADVERTISER_COLUMN_INDEX]!=null && tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX]!=null && (Decimal.Parse(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX].ToString())/(Decimal)1000)>(Decimal)0.0)
						t.Append("<td nowrap  class=\"pmtotal\">"+tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.FIRST_ADVERTISER_COLUMN_INDEX].ToString()+"</td>");				
						else t.Append("<td nowrap  class=\"pmtotal\">-</td>");				
						if(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.FIRST_ADVERTISER_INVEST_COLUMN_INDEX]!=null && tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX]!=null && (Decimal.Parse(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX].ToString())/(Decimal)1000)>(Decimal)0.0)
//						t.Append("<td nowrap  class=\"pmtotalnb\">"+String.Format("{0:### ### ### ### ##0.## }",Decimal.Parse(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.FIRST_ADVERTISER_INVEST_COLUMN_INDEX].ToString())/(Decimal)1000)+"</td>");				
							t.Append("<td nowrap  class=\"pmtotalnb\">"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.FIRST_ADVERTISER_INVEST_COLUMN_INDEX].ToString(),webSession.Unit,false)+"</td>");				
						else t.Append("<td nowrap  class=\"pmtotalnb\">-</td>");				 
						if(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.FIRST_ADVERTISER_SOV_COLUMN_INDEX]!=null && tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX]!=null && (Decimal.Parse(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX].ToString())/(Decimal)1000)>(Decimal)0.0)
						t.Append("<td nowrap  class=\"pmtotalnb\">"+Decimal.Parse(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.FIRST_ADVERTISER_SOV_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+"%</td>");
						else t.Append("<td nowrap  class=\"pmtotalnb\">-</td>");
						//Colonne separation 
						if(!excel){
							t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=1px></td>");
						}
						//Colonnes 1ere référence (optionnels)		
						if(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.FIRST_PRODUCT_COLUMN_INDEX]!=null && tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX]!=null && (Decimal.Parse(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX].ToString())/(Decimal)1000)>(Decimal)0.0)
						t.Append("<td nowrap  class=\"pmtotal\">"+tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.FIRST_PRODUCT_COLUMN_INDEX].ToString()+"</td>");				
						else t.Append("<td nowrap  class=\"pmtotal\">-</td>");				
						if(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.FIRST_PRODUCT_INVEST_COLUMN_INDEX]!=null && tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX]!=null && (Decimal.Parse(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX].ToString())/(Decimal)1000)>(Decimal)0.0)
//						t.Append("<td nowrap  class=\"pmtotalnb\">"+String.Format("{0:### ### ### ### ##0.## }",Decimal.Parse(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.FIRST_PRODUCT_INVEST_COLUMN_INDEX].ToString())/(Decimal)1000)+"</td>");				
						t.Append("<td nowrap  class=\"pmtotalnb\">"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.FIRST_PRODUCT_INVEST_COLUMN_INDEX].ToString(),webSession.Unit,false)+"</td>");				
						else t.Append("<td nowrap  class=\"pmtotalnb\">-</td>");				
						if(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.FIRST_PRODUCT_SOV_COLUMN_INDEX]!=null && tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX]!=null && (Decimal.Parse(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX].ToString())/(Decimal)1000)>(Decimal)0.0)
						t.Append("<td nowrap  class=\"pmtotalnb\">"+Decimal.Parse(tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX,ConstResults.Seasonality.FIRST_PRODUCT_SOV_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+"%</td></tr>");
						else t.Append("<td nowrap  class=\"pmtotalnb\">-</td></tr>");
						//t.Append("\n");
					}
					#endregion
					
					#region lignes annonceurs 	
					if(tab!=null && !(tab.GetLength(0)==0)){					
					for(Int64 i=currentLine;i<tab.GetLength(0);i+=nbMonths){												
						adclasse2="p7";
						adclasse="p9";

						#region personnalisation éléments de référence et concurrents
						if(tab[i,ConstResults.Seasonality.ID_ADVERTISER_COLUMN_INDEX] != null && AdvertiserAccessListArr!=null && AdvertiserAccessListArr.Contains(tab[i,ConstResults.Seasonality.ID_ADVERTISER_COLUMN_INDEX].ToString())){
							adclasse2="p15"; 
							adclasse="p151";

						}
						if(tab[i,ConstResults.Seasonality.ID_ADVERTISER_COLUMN_INDEX] != null && CompetitorAdvertiserAccessListArr!=null && CompetitorAdvertiserAccessListArr.Contains(tab[i,ConstResults.Seasonality.ID_ADVERTISER_COLUMN_INDEX].ToString())){
								
							if(excel){
								adclasse2=competitorExcelStyle; 
								adclasse=competitorExcelStyleNB;
							}
							else{
								adclasse2="p14"; 
								adclasse="p141";
							}

						}
						#endregion
						//Ligne annnonceur
						if(i==m && tab[i,ConstResults.Seasonality.ADVERTISER_COLUMN_INDEX]!=null )
							t.Append("<tr><td nowrap class="+adclasse2+">"+tab[i,ConstResults.Seasonality.ADVERTISER_COLUMN_INDEX].ToString()+"</td>");			
						else if(tab[i,ConstResults.Seasonality.ADVERTISER_COLUMN_INDEX]!=null) t.Append("<tr><!--<td rowspan="+rowspan+"></td>--><td nowrap class="+adclasse2+">"+tab[i,ConstResults.Seasonality.ADVERTISER_COLUMN_INDEX].ToString()+"</td>");			
						else t.Append("<tr><td nowrap class="+adclasse2+">-</td>");			
						if(tab[i,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX]!=null)t.Append("<td nowrap  class="+adclasse+">"+String.Format("{0:### ### ### ### ##0.## }",Decimal.Parse(tab[i,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX].ToString())/(Decimal)1000)+"</td>");
						else t.Append("<td nowrap  class="+adclasse+">-</td>");
						if(webSession.ComparativeStudy){
							if(tab[i,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX]!=null){
								 image="/I/p.gif";
								if(Decimal.Parse(tab[i,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX].ToString())>(Decimal)0.0)image="/I/g.gif";
								else if (Decimal.Parse(tab[i,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX].ToString())<(Decimal)0.0)image="/I/r.gif";
								else if (Decimal.Parse(tab[i,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX].ToString())==(Decimal)0.0)image="/I/o.gif";													
								if(!excel){
									t.Append("<td nowrap  class="+adclasse+">"+Decimal.Parse(tab[i,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+"% &nbsp;<img src="+image+"></td>");
								}
								else{
									t.Append("<td nowrap  class="+adclasse+">"+Decimal.Parse(tab[i,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+"%</td>");
								}

								}
									else t.Append("<td nowrap  class="+adclasse+">-</td>");
						}
						if(tab[i,ConstResults.Seasonality.REFERENCE_COLUMN_INDEX]!=null)t.Append("<td nowrap  class="+adclasse+">"+tab[i,ConstResults.Seasonality.REFERENCE_COLUMN_INDEX].ToString()+"</td>");				
						else t.Append("<td nowrap  class="+adclasse+">-</td>");
						if(tab[i,ConstResults.Seasonality.AVERAGE_BUDGET_COLUMN_INDEX]!=null)
//							t.Append("<td nowrap  class="+adclasse+" >"+String.Format("{0:### ### ### ### ##0.## }",Decimal.Parse(tab[i,ConstResults.Seasonality.AVERAGE_BUDGET_COLUMN_INDEX].ToString())/(Decimal)1000)+"</td>");
							t.Append("<td nowrap  class="+adclasse+" >"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.Seasonality.AVERAGE_BUDGET_COLUMN_INDEX].ToString(),webSession.Unit,false)+"</td>");							
						else t.Append("<td nowrap  class="+adclasse+">-</td>");
						//Colonne separation 
						if(!excel){
							t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=1px></td>");
						}
						#region éléments non personnalisés
						//Colonnes 1er annonceur (optionnels)		
						t.Append("<td bgcolor=\"#FFFFFF\" nowrap  class="+adclasse+">-</td>");				
						t.Append("<td bgcolor=\"#FFFFFF\"  nowrap  class="+adclasse+">-</td>");				
						t.Append("<td bgcolor=\"#FFFFFF\"  nowrap  class="+adclasse+">-</td>");
						//Colonne separation 
						if(!excel){
							t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=1px></td>");
						}
						//Colonnes 1ere référence (optionnels)		
						t.Append("<td  bgcolor=\"#FFFFFF\" nowrap  class="+adclasse+">-</td>");				
						t.Append("<td  bgcolor=\"#FFFFFF\" nowrap  class="+adclasse+">-</td>");				
						t.Append("<td  bgcolor=\"#FFFFFF\"  nowrap   class="+adclasse+">-</td>");
						#endregion
						t.Append("</tr>");
						color++;				
					}
				}
				#endregion				
				
				//incrémentation Tableau pour mois suivant
				currentLine=m+1;
				currentmonth++;			
				TOTAL_UNIVERS_LINE_INDEX++;
				TOTAL_SECTOR_LINE_INDEX++;
				 month++;
			}
			//Fin tableau mensuel
			t.Append("</TABLE>");	
			//Fin tableau
			t.Append("</TD></TR></TABLE>");

			return(t.ToString());
		}
		#endregion

		#endregion
		
		#endregion

		#region Sortie Excel
		/// <summary>
		/// Génère le code HTML (pour une sortie sous excel) sur le total support de l'univers,marché ou famille, ou sur les
		/// annonceurs de références ou concurrents :
		/// -les investissements de l'année N
		/// -une évolution N vs N-1 (uniquement dans le ca d'une étude comparative)
		/// -le nombre de référence
		/// -le budget moyen par référence
		/// -le 1er annonceur en Ke et SOV (uniquement pour les lignes total produits éventuels)
		/// -la 1ere référence en Ke et SOV (uniquement pour les lignes total produits éventuels)
		/// </summary>
		/// <param name="page">Page qui affiche le Plan Média</param>
		/// <param name="tabResult">tableau de résultats</param>
		/// <param name="webSession">Session du client</param>
		/// <returns>code HTML adapté pour une sortie sous excel</returns>
		public static string GetIndicatorSeasonalityExcelUI(Page page,object[] tabResult,WebSession webSession){
		
			System.Text.StringBuilder t = new System.Text.StringBuilder(5000);

			#region Rappel des paramètres
			// Paramètres du tableau
            t.Append(ExcelFunction.GetLogo(webSession));
			t.Append(ExcelFunction.GetExcelHeader(webSession,false,true,false,GestionWeb.GetWebWord(1308,webSession.SiteLanguage)));
			#endregion

			t.Append(GetIndicatorSeasonalityHtmlUI(page,tabResult,webSession,true));
			t.Append(ExcelFunction.GetFooter(webSession));
			return Convertion.ToHtmlString(t.ToString());
		
		}

		#endregion				
	}
}
