#region Info
/*
 * Author :     G Ragneau
 * Created on : 04/08/2008
 * History:
 *      Date - Author - Description
 *      04/08/2008 - G Ragneau - Moved from TNS.AdExpress.Web
 * 
 * 
 * */
#endregion

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Data;
using System.Text;

using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using CstComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstUnit = TNS.AdExpress.Constantes.Web.CustomerSessions.Unit;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using DBConstantes = TNS.AdExpress.Constantes.DB;


using TNS.AdExpress.Classification;
using TNS.Classification.Universe;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpressI.ProductClassIndicators.DAL;
using TNS.AdExpressI.ProductClassIndicators.Exceptions;
using System.Collections;
using TNS.FrameWork.Date;

namespace TNS.AdExpressI.ProductClassIndicators.Engines
{
    /// <summary>
    /// Engine to build a Top report or to provide computed data for top report or indicator
    /// </summary>
    public class EngineSeasonality:Engine
    {

        #region Constants

        #region constantes présentation tableau
        public const int NB_MAX_COLUMN = 17;
        /// <summary>
        ///Colonne des MOIS (libellés)
        /// </summary>
        public const Int64 MONTH_COLUMN_INDEX = 0;
        /// <summary>
        ///Colonne des MOIS (identifiant)
        /// </summary>
        public const Int64 VALUE_MONTH_COLUMN_INDEX = 1;
        /// <summary>
        /// Colonne des MOIS Comparatifs(libellés)
        /// </summary>
        public const Int64 COMPAR_MONTH_COLUMN_INDEX = 2;
        /// <summary>
        /// Colonne des MOIS Comparatifs(identifiant)
        /// </summary>
        public const Int64 COMPAR_VALUE_MONTH_COLUMN_INDEX = 3;
        /// <summary>
        /// Colonne Libelle annonceur
        /// </summary>
        public const Int64 ADVERTISER_COLUMN_INDEX = 4;
        /// <summary>
        /// Colonne id annonceur
        /// </summary>
        public const Int64 ID_ADVERTISER_COLUMN_INDEX = 5;
        /// <summary>
        /// Index colonne des investissements
        /// /// </summary>
        public const Int64 INVESTMENT_COLUMN_INDEX = 6;
        /// <summary>
        /// Index colonne des investissements (option etude comparative)
        /// </summary>
        public const Int64 COMPAR_INVESTMENT_COLUMN_INDEX = 7;
        /// <summary>
        /// Colonne evolution 
        /// </summary>
        public const Int64 EVOLUTION_COLUMN_INDEX = 8;
        /// <summary>
        /// Colonne nombre de références
        /// </summary>
        public const Int64 REFERENCE_COLUMN_INDEX = 9;
        /// <summary>
        /// Colonne budget moyen
        /// </summary>
        public const Int64 AVERAGE_BUDGET_COLUMN_INDEX = 10;
        /// <summary>										
        /// Colonne 1er Annonceur			
        /// </summary>
        public const Int64 FIRST_ADVERTISER_COLUMN_INDEX = 11;
        /// <summary>
        /// Colonne investissement 1er Annonceur			
        /// </summary>
        public const Int64 FIRST_ADVERTISER_INVEST_COLUMN_INDEX = 12;
        /// <summary>
        /// Colonne SOV 1er Annonceur			
        /// </summary>
        public const Int64 FIRST_ADVERTISER_SOV_COLUMN_INDEX = 13;
        /// <summary>
        /// Colonne 1er référence			
        /// </summary>
        public const Int64 FIRST_PRODUCT_COLUMN_INDEX = 14;
        /// <summary>
        /// Colonne investissement 1er référence			
        /// </summary>
        public const Int64 FIRST_PRODUCT_INVEST_COLUMN_INDEX = 15;
        /// <summary>
        /// Colonne SOV 1er référence			
        /// </summary>
        public const Int64 FIRST_PRODUCT_SOV_COLUMN_INDEX = 16;
        #endregion

        #region constantes présentation graphique
        /// <summary>
        /// Colonne ID mois 
        /// </summary>
        /// <remarks>pour présentation tableau</remarks>
        public const Int64 ID_MONTH_COLUMN_INDEX = 0;
        /// <summary>
        /// Colonne ID ELement (annonceur de référence ou concurrents, ou total )
        /// </summary>
        /// <remarks>pour présentation tableau</remarks>
        public const Int64 ID_ELEMENT_COLUMN_INDEX = 1;
        /// <summary>
        /// Colonne Libellé Elément (annonceur de référence ou concurrents, ou total )
        /// </summary>
        /// <remarks>pour présentation tableau</remarks>
        public const Int64 LABEL_ELEMENT_COLUMN_INDEX = 2;
        /// <summary>
        /// Colonne Investissement
        /// </summary>
        /// <remarks>pour présentation tableau</remarks>
        public const Int64 INVEST_COLUMN_INDEX = 3;
        /// <summary>
        /// Colonne Id Total Marché
        /// </summary>
        /// <remarks>pour présentation tableau</remarks>
        public const Int64 ID_TOTAL_MARKET_COLUMN_INDEX = -1;
        /// <summary>
        /// Colonne Id Total famille
        /// </summary>
        /// <remarks>pour présentation tableau</remarks>
        public const Int64 ID_TOTAL_SECTOR_COLUMN_INDEX = -2;
        /// <summary>
        /// Colonne Id Total famille
        /// </summary>
        /// <remarks>pour présentation tableau</remarks>
        public const Int64 ID_TOTAL_UNIVERSE_COLUMN_INDEX = -3;
        /// <summary>
        /// Colonne nombre colonnes
        /// </summary>
        /// <remarks>pour présentation tableau</remarks>
        public const Int64 NB_MAX_COLUMNS_COLUMN_INDEX = 4;
        #endregion

        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="dalLayer">Data Access Layer</param>
        public EngineSeasonality(WebSession session, IProductClassIndicatorsDAL dalLayer) : base(session, dalLayer) { 
        }
        #endregion

        #region GetResult
        /// <summary>
        /// Get Seasonality indicator as a table
        /// </summary>
        /// <returns>StringBuilder with HTML code</returns>
        public override StringBuilder GetResult()
        {
            StringBuilder str = new StringBuilder(50000);
			bool showProduct = _session.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
			CultureInfo cInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);

            #region CSS Styles
            string cssTotalLabel = "pmtotal";
            string cssTotalNb = "pmtotalnb";
            string cssAdvLabel = "p7";
            string cssAdvNb = "p9";
            string cssRefLabel = "p15";
            string cssRefNb = "p151";
            string cssCompLabel = (_excel) ? "p142" : "p14";
            string cssCompNb = (_excel) ? "p143" : "p141";
            #endregion

            #region GetData
            object[] tabResult = this.GetTableData();
            #endregion

            #region No data
            if (tabResult.GetLength(0) == 0 ){
				return str.AppendFormat("<div align=\"center\" class=\"txtViolet11Bold\">{0}</div>", GestionWeb.GetWebWord(177,_session.SiteLanguage));
			}			
			#endregion
							
			#region construction du tableau de saisonnalité
			//Init var tables
            object[,] tab = (object[,])tabResult[0];
            object[,] tabTotal = (object[,])tabResult[1];
            object[,] tabTotalUniverse = (object[,])tabResult[2];
            int TAB_MAX_COLUMN_INDEX = 0;
            int TAB_MAX_LINE_INDEX = 0;
			if( tab != null ){
				TAB_MAX_COLUMN_INDEX=tab.GetLength(1);
				TAB_MAX_LINE_INDEX=tab.GetLength(0);
			}
			else if( tabTotal!=null && (tab==null || tab.GetLength(0)==0) ){
				TAB_MAX_COLUMN_INDEX=tabTotal.GetLength(1);
				TAB_MAX_LINE_INDEX=tabTotal.GetLength(0);
			}
            #endregion

			#region Get monthes 
			int nbMonths =  _periodEnd.Month - _periodBegin.Month + 1 ;
			int currentmonth = _periodBegin.Month;
			int month = _periodBegin.Month;
			//Nombre de lignes à traiter
			int maxLineToTreat = nbMonths;
			#endregion

            #region Indexes
            DataTable dtTotalMarket = null;
            int nbAdvertiser = 0;
            if (tab!=null)
            {
				nbAdvertiser =(nbMonths>0)?(TAB_MAX_LINE_INDEX-1)/nbMonths : 0;
			}
            else if((tabTotal!=null || tabTotalUniverse!=null) && (tab==null || tab.GetLength(0)==0))
            {
				nbAdvertiser =  0;
			}
			//Total Univers line
			Int64 TOTAL_UNIVERS_LINE_INDEX = 1;
			//Market or sector total lineindex ligne total marché ou famille
			Int64 TOTAL_SECTOR_LINE_INDEX = 1;	
			//Rowspan
            int rowspan = 0;
			if( ((tabTotal == null || tabTotal.GetLength(0)==0) || (tabResult==null || tabResult[3]==null && dtTotalMarket==null))&& (tabTotalUniverse==null || tabTotalUniverse.GetLength(0)==0)){
			    rowspan = nbAdvertiser+1;	
            }
			else if ( (tabTotal == null ) && (tabResult!=null && tabResult[3]==null ) && (tabTotalUniverse!=null || tabTotalUniverse.GetLength(0)>0) ){
                rowspan = nbAdvertiser + 1;
            }
			else if (((tabTotal != null && tabTotal.GetLength(0)>0 ) || (tabResult!=null || tabResult[3]!=null && (dtTotalMarket!=null && dtTotalMarket.Rows.Count>0)) ) && (tabTotalUniverse==null || tabTotalUniverse.GetLength(0)==0) )
            {
                rowspan = nbAdvertiser + 1;
            }
			else{
                rowspan = nbAdvertiser + 2;
            }
            #endregion

			str.Append("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"p2\"><tr><td>");
			//Monthly table
			str.Append("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">");

			#region headers
			str.AppendFormat("<tr ><td colspan=2 class=\"p2\"></td><td nowrap class=\"p2\">{0} {1}</td>", GestionWeb.GetWebWord(1156,_session.SiteLanguage), _periodBegin.Year);
			//Evol (optionnelle)
			if(_session.ComparativeStudy){
                str.AppendFormat("<td nowrap class=\"p2\">{0} {1}/{2}</td>", GestionWeb.GetWebWord(1168,_session.SiteLanguage), _periodBegin.Year, _periodBegin.AddYears(-1).Year);
            }
			str.AppendFormat("<td nowrap class=\"p2\">{0}</td>", GestionWeb.GetWebWord(1152,_session.SiteLanguage));
			str.AppendFormat("<td nowrap class=\"p2\">{0}</td>", GestionWeb.GetWebWord(1153,_session.SiteLanguage));
			//Separator
			if(!_excel){
                str.Append("<td class=\"violetBackGround columnSeparator\"><img width=1px></td>");
			}
			//First advertiser (optionnels)		
			str.AppendFormat("<td nowrap class=\"p2\" colSpan=\"2\">{0}</td>", GestionWeb.GetWebWord(1154,_session.SiteLanguage));
			str.AppendFormat("<td nowrap class=\"p2\">{0}</td>", GestionWeb.GetWebWord(437,_session.SiteLanguage));

			if (showProduct) {
				//Separator
				if (!_excel) {
					str.Append("<td class=\"violetBackGround columnSeparator\"><img width=1px></td>");
				}
				//1er références (optionnels)		
				str.AppendFormat("<td nowrap class=\"p2\" colSpan=\"2\">{0}</td>", GestionWeb.GetWebWord(1155, _session.SiteLanguage));
				str.AppendFormat("<td nowrap class=\"p2\">{0}</td>", GestionWeb.GetWebWord(437, _session.SiteLanguage));
			}
			str.Append("</tr>");
			#endregion

            #region Monthes
            //For each selected monthes
            object invest;
            object evol;
            object refNb;
            object avgInvest;
            object advLabel;
            object advInvest;
            object advSOV;
            object refLabel=null;
            object refInvest=null;
            object refSOV=null;
            DataRow[] foundRows;
            string cssLabel = cssTotalLabel;
            string cssNb = cssTotalNb;
            Int64 currentLine=1;								
            for (int m = 1; m <= nbMonths; m++)
            {

                cssLabel = cssTotalLabel;
                cssNb = cssTotalNb;
				//str.AppendFormat("<tr><td class=\"p2\" nowrap rowspan={0}>{1}</td>", rowspan, MonthString.GetHTMLCharacters(currentmonth, _session.SiteLanguage, 0));
				str.AppendFormat("<tr><td class=\"p2\" nowrap rowspan={0}>{1}</td>", rowspan, MonthString.GetCharacters(currentmonth, cInfo, 0));
				
                #region Total sector
                if (_session.ComparaisonCriterion == CstComparaisonCriterion.sectorTotal && tabTotal != null && !(tabTotal.GetLength(0) == 0))
                {
                    invest = tabTotal[TOTAL_SECTOR_LINE_INDEX, INVESTMENT_COLUMN_INDEX];
                    evol = tabTotal[TOTAL_SECTOR_LINE_INDEX, EVOLUTION_COLUMN_INDEX];
                    refNb = tabTotal[TOTAL_SECTOR_LINE_INDEX, REFERENCE_COLUMN_INDEX];
                    avgInvest = tabTotal[TOTAL_SECTOR_LINE_INDEX, AVERAGE_BUDGET_COLUMN_INDEX];
                    advLabel = tabTotal[TOTAL_SECTOR_LINE_INDEX, FIRST_ADVERTISER_COLUMN_INDEX];
                    advInvest = tabTotal[TOTAL_SECTOR_LINE_INDEX, FIRST_ADVERTISER_INVEST_COLUMN_INDEX];
                    advSOV = tabTotal[TOTAL_SECTOR_LINE_INDEX, FIRST_ADVERTISER_SOV_COLUMN_INDEX];
					if (showProduct) {
						refLabel = tabTotal[TOTAL_SECTOR_LINE_INDEX, FIRST_PRODUCT_COLUMN_INDEX];
						refInvest = tabTotal[TOTAL_SECTOR_LINE_INDEX, FIRST_PRODUCT_INVEST_COLUMN_INDEX];
						refSOV = tabTotal[TOTAL_SECTOR_LINE_INDEX, FIRST_PRODUCT_SOV_COLUMN_INDEX];
					}
                    AppendLine(str, GestionWeb.GetWebWord(1189, _session.SiteLanguage), cssLabel, cssNb, invest, evol, refNb, avgInvest, advLabel, advInvest, advSOV, refLabel, refInvest, refSOV);

                    str.Append("</tr>");

                }
                #endregion

                #region Total Market
                else if (_session.ComparaisonCriterion == CstComparaisonCriterion.marketTotal && tabResult != null && tabResult[3] != null)
                {
                    dtTotalMarket = (DataTable)tabResult[3];
                    if (dtTotalMarket != null && dtTotalMarket.Rows.Count > 0)
                    {

                        foundRows = dtTotalMarket.Select(string.Format("MOIS = {0}", month), "MOIS ASC", DataViewRowState.OriginalRows);
                        if (foundRows != null && foundRows.Length > 0 && foundRows[0] != null)
                        {
                            invest = foundRows[0]["TOTAL_N"];
                            evol = null;
                            if (foundRows[0].Table.Columns.Contains("EVOL"))
                            {
                                evol = foundRows[0]["EVOL"];
                            }
                            refNb = foundRows[0]["NBREF"];
                            avgInvest = foundRows[0]["BUDGET_MOYEN"];
                            advLabel = foundRows[0]["ADVERTISER"];
                            advInvest = foundRows[0]["INVESTMENT_ADVERTISER"];
                            advSOV = foundRows[0]["SOV_FIRST_ADVERTISER"];
							if (showProduct) {
								refLabel = foundRows[0]["PRODUCT"];
								refInvest = foundRows[0]["INVESTMENT_PRODUCT"];
								refSOV = foundRows[0]["SOV_FIRST_PRODUCT"];
							}

                            AppendLine(str, GestionWeb.GetWebWord(1190, _session.SiteLanguage), cssLabel, cssNb, invest, evol, refNb, avgInvest, advLabel, advInvest, advSOV, refLabel, refInvest, refSOV);

                            str.Append("</tr>");

                        }
                    }
                }
                #endregion

                #region Total univers
                //Total univers	
                if (tabTotalUniverse != null && !(tabTotalUniverse.GetLength(0) == 0))
                {
                    str.Append("<tr>");

                    invest = tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX, INVESTMENT_COLUMN_INDEX];
                    evol = tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX, EVOLUTION_COLUMN_INDEX];
                    refNb = tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX, REFERENCE_COLUMN_INDEX];
                    avgInvest = tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX, AVERAGE_BUDGET_COLUMN_INDEX];
                    advLabel = tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX, FIRST_ADVERTISER_COLUMN_INDEX];
                    advInvest = tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX, FIRST_ADVERTISER_INVEST_COLUMN_INDEX];
                    advSOV = tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX, FIRST_ADVERTISER_SOV_COLUMN_INDEX];
					if (showProduct) {
						refLabel = tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX, FIRST_PRODUCT_COLUMN_INDEX];
						refInvest = tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX, FIRST_PRODUCT_INVEST_COLUMN_INDEX];
						refSOV = tabTotalUniverse[TOTAL_UNIVERS_LINE_INDEX, FIRST_PRODUCT_SOV_COLUMN_INDEX];
					}
                    AppendLine(str, GestionWeb.GetWebWord(1188, _session.SiteLanguage), cssLabel, cssNb, invest, evol, refNb, avgInvest, advLabel, advInvest, advSOV, refLabel, refInvest, refSOV);

                    str.Append("</tr>");

                }
                #endregion

                #region Advertisers
                if (tab != null && !(tab.GetLength(0) == 0))
                {
                    for (Int64 i = currentLine; i < tab.GetLength(0); i += nbMonths)
                    {
                        cssLabel = cssAdvLabel;
                        cssNb = cssAdvNb;

                        #region Reference or competitor?
                        if (tab[i, ID_ADVERTISER_COLUMN_INDEX] != null)
                        {
                            Int64 idAdv = Convert.ToInt64(tab[i, ID_ADVERTISER_COLUMN_INDEX]);
                            if (_referenceIDS.Contains(idAdv))
                            {
                                cssLabel = cssRefLabel;
                                cssNb = cssRefNb;
                            }
                            else if (_competitorIDS.Contains(idAdv))
                            {
                                cssLabel = cssCompLabel;
                                cssNb = cssCompNb;
                            }
                        }
                        #endregion

                        str.Append("<tr>");

                        invest = tab[i, INVESTMENT_COLUMN_INDEX];
                        evol = tab[i, EVOLUTION_COLUMN_INDEX];
                        refNb = tab[i, REFERENCE_COLUMN_INDEX];
                        avgInvest = tab[i, AVERAGE_BUDGET_COLUMN_INDEX];
                        advLabel = null;
                        advInvest = null;
                        advSOV = null;
                        refLabel = null;
                        refInvest = null;
                        refSOV = null;

                        AppendLine(str, tab[i, ADVERTISER_COLUMN_INDEX].ToString(), cssLabel, cssNb, invest, evol, refNb, avgInvest, advLabel, advInvest, advSOV, refLabel, refInvest, refSOV);

                        str.Append("</tr>");

                    }
                }
                #endregion

                //Next monh
                currentLine = m + 1;
                currentmonth++;
                TOTAL_UNIVERS_LINE_INDEX++;
                TOTAL_SECTOR_LINE_INDEX++;
                month++;
            }
			//End mensual table
			str.Append("</table>");	
            #endregion

            str.Append("</td></tr></table>");

            return str;
        }
        #endregion

        #region GetTableData
        /// <summary>
        /// Get data to build  seasonality report as a table
        /// Give : 
        ///     -Investments on N
        ///     -Evol n vs N-1 (only if comparative study)
        ///     -Reference number
        ///     -Average of investment per reference
        ///     -First advertiser in k€ and SOV (only for eventual advertiser lines)
        ///     -First reference in k€ and SOV (only for eventual product lines)
        /// For univers, market or sector and on potential references and competitors advertisers
        /// </summary>
        /// <returns>Table with a mensual compareason N vs N-1</returns>
        /// <returns></returns>
        public object[] GetTableData()
        {

            object[] tabResult = new object[4];
			bool showProduct = _session.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);

            #region Calculs pour le total support(univers,marché ou famille) et , les annonceurs et des références

            #region Initialisation tableau de résultats	pour les annonceurs et des références
            /*
			*Initialisation du tableau ("tab") qui contiendra (1 annonceur par ligne) la liste des annonceurs
			de références ou concurrents et leurs investissements mensuels,Evolution par rapport au mois 
			de la pérode N-1,nombre de références et le budget moyen 
			*/
            //Groupe de données pour annonceurs de références ou concurrents
            DataSet ds = _dalLayer.GetSeasonalityTblData(true, true);
            DataTable dt = null;
            object[] oTotalAdvert = null;
            object[,] tab = null;
            if (ds != null) dt = ds.Tables[0];
            if ((_referenceIDS.Count > 0 || _competitorIDS.Count > 0) && dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    oTotalAdvert = GetGlobalParams(dt, true,showProduct);
                    //initialisation du tableau de résultats pour annonceurs de références ou concurrents
                    tab = (object[,])oTotalAdvert[2];
                }
            }
            #endregion

            #region tableau de résultats pour le total univers
            /*
			*Initialisation et insertion de données, pour tableau ("tabTotalUniv") de résultats du total univers.
			Chaque ligne contient : l'investissements mensuel,evolution par rapport au mois 
			de la pérode N-1,nombre de références et le budget moyen ,1er annonceur avec son
			investissement et SOV,1 ere référence avec son investissement et SOV.
			*/
            //Groupe de données pour total univers 		
            DataSet dsTotalUniverse = _dalLayer.GetSeasonalityTblData(false, true);
            object[] oTotalUniv = null;
            object[,] tabTotalUniv = null;
            //insertion des données dans le tableau
            if (dsTotalUniverse != null && dsTotalUniverse.Tables[0] != null && dsTotalUniverse.Tables[0].Rows.Count > 0)
            {
                oTotalUniv = GetGlobalParams(dsTotalUniverse.Tables[0], false,showProduct);
                tabTotalUniv = (object[,])oTotalUniv[2];
            }
            #endregion

            #region tableau de résultats pour le total famille ou marché
            /*
			*Pour le total marché chaque ligne de la tablea de données recupérée correspond
			à l'investissement mensuel,evolution par rapport au mois 
			de la pérode N-1,nombre de références et le budget moyen ,1er annonceur avec son
			investissement et SOV,1 ere référence avec son investissement et SOV.
			Remarque : Traitement distinct du total famille pour optimiser les temps de réponses
			par utilisation de procédures stockées.
			*/
            DataSet dsTotal = null;
            if (_session.ComparaisonCriterion == CstComparaisonCriterion.marketTotal)
            {
                dsTotal = _dalLayer.GetSeasonalityTotal(); //Total marché
            }
            else
            {
                dsTotal = _dalLayer.GetSeasonalityTblData(false, false); //Total famille
            }
            /*
            *Initialisation et insertion de données, pour tableau ("tabTotal") de résultats du total famille.
            Chaque ligne contient : l'investissements mensuel,evolution par rapport au mois 
            de la pérode N-1,nombre de références et le budget moyen ,1er annonceur avec son
            investissement et SOV,1 ere référence avec son investissement et SOV.
            */
            object[,] tabTotal = null;
            object[] oTotal = null;
            if (_session.ComparaisonCriterion == CstComparaisonCriterion.sectorTotal)
            {
                if (dsTotal != null && dsTotal.Tables[0] != null && (dsTotal.Tables[0].Rows.Count > 0))
                {
                    oTotal = GetGlobalParams(dsTotal.Tables[0], false,showProduct);
                    if (oTotal != null)
                    {
                        tabTotal = (object[,])oTotal[2];
                    }
                }
                else
                {
                        dsTotal = null;
                }
            }
            #endregion

            // Il n'y a pas de données
            if (tab == null && tabTotal == null && tabTotalUniv == null && dsTotal == null) return (new object[0]);
            #endregion

            #region Construction du tableau de résultats pour les annonceurs de références ou concurrents sélectionnés (optionnel)
            /*Pour annonceurs de références ou concurrents chaque ligne du tableau contiendra ( 1 ligne par annonceur et par mois)
				Les investissements mensuels, evolution, nb de références,budget moyen.				
			*/
            //On recupere le nombre d'annonceurs (pour sélection annonceurs concurrents ou références)
            int nbAdvertiser = 0;
            if (oTotalAdvert != null && oTotalAdvert[0] != null && dt != null && dt.Rows.Count > 0 && (_referenceIDS.Count > 0 || _competitorIDS.Count > 0)) 
                nbAdvertiser = int.Parse(oTotalAdvert[0].ToString());

            //Nombre de lignes à traiter (pour sélection annonceurs concurrents ou références)
            int nbMonths = (int)_periodEnd.Month - (int)_periodBegin.Month + 1;
            Int64 maxLinesToTreat = nbMonths;

            //Nombre total de lignes (pour sélection annonceurs concurrents ou références)
            Int64 nbTabLines = (oTotalAdvert != null) ? int.Parse(oTotalAdvert[3].ToString()) : 0;

            //Pour les anonceurs (références ou concurrents) sélectionnés (optionnel)		
            if (dt != null && tab != null)
            {
                #region remplissage du tab et calculs investissements

                #region libellés tab
                // Libellés du tableaux 				
                tab[0, MONTH_COLUMN_INDEX] = "LIBELLE Mois";
                tab[0, VALUE_MONTH_COLUMN_INDEX] = "ID Mois";
                tab[0, COMPAR_MONTH_COLUMN_INDEX] = "ID Mois Concurrent";
                tab[0, COMPAR_VALUE_MONTH_COLUMN_INDEX] = "LIBELLE Mois Concurrent";
                tab[0, ID_ADVERTISER_COLUMN_INDEX] = "id Annonceur";
                tab[0, ADVERTISER_COLUMN_INDEX] = "Libelle Annonceur";
                tab[0, INVESTMENT_COLUMN_INDEX] = "Investissement N";
                tab[0, COMPAR_INVESTMENT_COLUMN_INDEX] = "Investissement N-1";
                tab[0, EVOLUTION_COLUMN_INDEX] = "Evol N/N-1";
                tab[0, REFERENCE_COLUMN_INDEX] = GestionWeb.GetWebWord(1152, _session.SiteLanguage);
                tab[0, AVERAGE_BUDGET_COLUMN_INDEX] = GestionWeb.GetWebWord(1153, _session.SiteLanguage);
                tab[0, FIRST_ADVERTISER_COLUMN_INDEX] = GestionWeb.GetWebWord(1154, _session.SiteLanguage);
                tab[0, FIRST_ADVERTISER_INVEST_COLUMN_INDEX] = "FirstAdInvest";
                tab[0, FIRST_ADVERTISER_SOV_COLUMN_INDEX] = GestionWeb.GetWebWord(437, _session.SiteLanguage);
				if (showProduct) {
					tab[0, FIRST_PRODUCT_COLUMN_INDEX] = GestionWeb.GetWebWord(1155, _session.SiteLanguage);
					tab[0, FIRST_PRODUCT_INVEST_COLUMN_INDEX] = "FirstRefInvest";
					tab[0, FIRST_PRODUCT_SOV_COLUMN_INDEX] = GestionWeb.GetWebWord(437, _session.SiteLanguage);
				}
                #endregion

                #region calcul avec droits produits
                //ligne courante
                Int64 currentLine = 1;
                //On remplit la colonne des libéllés de mois
                tab = FillMonthIndexColum(tab, nbTabLines, _periodEnd, MONTH_COLUMN_INDEX, VALUE_MONTH_COLUMN_INDEX);
                if (_session.ComparativeStudy) 
                    tab = FillMonthIndexColum(tab, nbTabLines, _periodEnd.AddYears(-1), COMPAR_MONTH_COLUMN_INDEX, COMPAR_VALUE_MONTH_COLUMN_INDEX);

                #region Pour chaque ligne de données
                
                Decimal currentMonthInvest = (Decimal)0.0;//Investissement du mois en cours de traitement
                string currentMonth = "";//Mois courant
                int tempTotalRef = 0;// investissement temporaire total des références
                ArrayList tempAdvertiser = new ArrayList();//liste temporaire des annonceurs
                Int64 oldIdAdvertiser = 0;//ID ancien annonceur                
                Decimal currentComparMonthInvest = (Decimal)0.0;//Investissement du mois N-1               
                string currentComparMonth = ""; //Mois courant période N-1
                Int64 oldCurrentLine = 0;//index ancienne ligne du tableau de résultats
                Int64 oldMaxLinesToTreat = 0;//ancien nomre de lignes total à traiter
                Int64 Start = 0;//debut boucle
                int n = 0;
                foreach (DataRow currentRow in dt.Rows)
                {
                    //On passe aux lignes dans le tab correspondant au prochain annonceur
                    if ((_referenceIDS.Count > 0 || _competitorIDS.Count > 0) && oldIdAdvertiser != (Int64)currentRow["id_advertiser"] && Start == 1)
                    {
                        currentLine += nbMonths;
                        maxLinesToTreat += nbMonths;
                    }

                    #region remplissage du tableau par annonceur
                    //Pour un annonceur
                    n = _periodBegin.Month;
                    for (Int64 i = currentLine; i <= maxLinesToTreat; i++)
                    {
                        //id et Libelle annonceur 
                        if ((_referenceIDS.Count > 0 || _competitorIDS.Count > 0) && oldIdAdvertiser != (Int64)currentRow["id_advertiser"] && !tempAdvertiser.Contains(currentRow["id_advertiser"].ToString()))
                        {
                            tab[i, ADVERTISER_COLUMN_INDEX] = currentRow["advertiser"].ToString();
                            tab[i, ID_ADVERTISER_COLUMN_INDEX] = currentRow["id_advertiser"].ToString();
                            //Mois en cours
                            currentMonth = FctUtilities.Dates.GetMonthAlias(n, _iYearID, 3, _session);
                            //Calcul des investissements de chaque mois sélectionné sur l'année N (Nouvelle version)
                            tab[i, INVESTMENT_COLUMN_INDEX] = currentMonthInvest = Decimal.Parse(dt.Compute("sum(" + currentMonth + ")", " id_advertiser=" + currentRow["id_advertiser"].ToString()).ToString());
                            //Calcul nombre de références mensuelles par annonceur															
                            tab[i, REFERENCE_COLUMN_INDEX] = tempTotalRef = int.Parse(dt.Compute("count(" + currentMonth + ")", "" + currentMonth.ToString() + " >0 AND id_advertiser=" + currentRow["id_advertiser"].ToString()).ToString());
                            //Calcul du budget moyen = budget/nombre de références																													
                            tab[i, AVERAGE_BUDGET_COLUMN_INDEX] = (tempTotalRef > 0 && (Decimal.Parse(currentMonthInvest.ToString()) >= (Decimal)0.0)) ? (Decimal.Parse(currentMonthInvest.ToString()) / Decimal.Parse(tempTotalRef.ToString())) : Decimal.Parse(currentMonthInvest.ToString());
                        }
                        //Calcul de l'évolution par rapport à année N-1 
                        if (_session.ComparativeStudy && oldIdAdvertiser != (Int64)currentRow["id_advertiser"] && !tempAdvertiser.Contains(currentRow["id_advertiser"].ToString()))
                        {
                            currentComparMonth = tab[i, COMPAR_MONTH_COLUMN_INDEX].ToString();
                            //Calcul des investissements de chaque mois sélectionné sur l'année N -1												
                            if (FctUtilities.CheckedText.IsNotEmpty(currentComparMonth))
                                tab[i, COMPAR_INVESTMENT_COLUMN_INDEX] = currentComparMonthInvest = Decimal.Parse(dt.Compute("sum(" + currentComparMonth + ")", " id_advertiser=" + currentRow["id_advertiser"].ToString()).ToString());
                            //Calcul de l'evolution anneé N par rapport N-1	= ((N-(N-1))*100)/N-1 																					
                            tab[i, EVOLUTION_COLUMN_INDEX] = (tab[i, INVESTMENT_COLUMN_INDEX] != null && tab[i, COMPAR_INVESTMENT_COLUMN_INDEX] != null &&
                                Decimal.Parse(tab[i, COMPAR_INVESTMENT_COLUMN_INDEX].ToString()) != (Decimal)0.0) ? (((Decimal.Parse(tab[i, INVESTMENT_COLUMN_INDEX].ToString()) - Decimal.Parse(tab[i, COMPAR_INVESTMENT_COLUMN_INDEX].ToString())) * (Decimal)100.0) / Decimal.Parse(tab[i, COMPAR_INVESTMENT_COLUMN_INDEX].ToString())) : (Decimal)0.0;
                        }
                        n++;
                    }
                    #endregion

                    //Traitement distinct des annonceurs
                    if (!tempAdvertiser.Contains(currentRow["id_advertiser"].ToString()))
                    {
                        tempAdvertiser.Add(currentRow["id_advertiser"].ToString());
                    }
                    Start = 1;
                    oldCurrentLine = currentLine;
                    oldMaxLinesToTreat = maxLinesToTreat;
                    tempTotalRef = 0;
                    oldIdAdvertiser = (Int64)currentRow["id_advertiser"];
                }
                #endregion

                tempTotalRef = 0;

                #endregion

                #endregion
            }
            #endregion

            if (tab != null) tabResult[0] = tab;
            if (tabTotal != null) tabResult[1] = tabTotal;
            if (tabTotalUniv != null) tabResult[2] = tabTotalUniv;
            if (dsTotal != null && _session.ComparaisonCriterion == CstComparaisonCriterion.marketTotal) tabResult[3] = dsTotal.Tables[0];


            return tabResult;

        }
        #endregion

        #region GetChartData
        /// <summary>
        /// Get investments of year N and market/sector/univers totals
        /// </summary>
        /// <remarks>Used to build seasonality graph.</remarks>
        /// <returns>Month by month investments</returns>
        public object[,] GetChartData()
        {

            #region Load data
            //Get investments of reference / competitors advertisers
            DataSet ds = null;
            DataTable dt = null;
            if (_referenceIDS.Count+_competitorIDS.Count > 0){
                ds = _dalLayer.GetSeasonalityGraphData(true, true);
            }
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0) dt = ds.Tables[0];
            //Get univers total
            DataSet dsTotalUniverse = _dalLayer.GetSeasonalityGraphData(false, true);
            DataTable dtTotalUniverse = null;
            if (dsTotalUniverse != null && dsTotalUniverse.Tables[0] != null && dsTotalUniverse.Tables[0].Rows.Count > 0) dtTotalUniverse = dsTotalUniverse.Tables[0];
            //Get market or sector total
            DataSet dsTotal = _dalLayer.GetSeasonalityGraphData(false, false);
            DataTable dtTotal = null;
            if (dsTotal != null && dsTotal.Tables[0] != null && dsTotal.Tables[0].Rows.Count > 0) dtTotal = dsTotal.Tables[0];
            #endregion

            #region Indexes
            int nbMaxRow = 0;
            //Max number of line = Sum(each datatable.rows.count * number of monthes)
            if (dt != null && dt.Rows.Count > 0) nbMaxRow += dt.Rows.Count * (dt.Columns.Count - 2);
            if (dtTotalUniverse != null && dtTotalUniverse.Rows.Count == 1)
            {
                foreach (DataColumn dtc in dtTotalUniverse.Columns)
                {
                    if(dtTotalUniverse.Rows[0][dtc] != System.DBNull.Value){
                        nbMaxRow += dtTotalUniverse.Rows.Count * (dtTotalUniverse.Columns.Count);
                        break;
                    }
                }
                if (nbMaxRow < 1)
                {
                    dtTotalUniverse = null; 
                }
            }
            if (dtTotal != null && dtTotal.Rows.Count == 1)
            {
                foreach (DataColumn dtc in dtTotal.Columns)
                {
                    if (dtTotal.Rows[0][dtc] != System.DBNull.Value)
                    {
                        nbMaxRow += dtTotal.Rows.Count * (dtTotal.Columns.Count);
                        break;
                    }
                }
            }

            //Init tab
            object[,] tab = null;
            if (nbMaxRow > 0) tab = new object[nbMaxRow, NB_MAX_COLUMNS_COLUMN_INDEX];
            #endregion

            // No data
            if (tab == null) return (new object[0, 0]);

            #region Result Table
            //For each selected month
            string monthAlias = string.Empty;
            int line = 0;
            for (int i = _periodBegin.Month; i <= _periodEnd.Month; i++)
            {
                monthAlias = FctUtilities.Dates.GetMonthAlias(i, _iYearID, 3, _session).Trim();
                //Market or sector total
                if (dtTotal != null && dtTotal.Rows.Count == 1 && dtTotal.Columns.Contains(monthAlias))
                {
                    //Month
                    tab[line, ID_MONTH_COLUMN_INDEX] = i;
                    if (_session.ComparaisonCriterion == CstComparaisonCriterion.marketTotal)
                    {
                        //ID market total
                        tab[line, ID_ELEMENT_COLUMN_INDEX] = ID_TOTAL_MARKET_COLUMN_INDEX;
                        //Label
                        tab[line, LABEL_ELEMENT_COLUMN_INDEX] = GestionWeb.GetWebWord(1190, _session.SiteLanguage);
                    }
                    else if (_session.ComparaisonCriterion == CstComparaisonCriterion.sectorTotal)
                    {
                        //ID sector total
                        tab[line, ID_ELEMENT_COLUMN_INDEX] = ID_TOTAL_SECTOR_COLUMN_INDEX;
                        //libéllé total famille
                        tab[line, LABEL_ELEMENT_COLUMN_INDEX] = GestionWeb.GetWebWord(1189, _session.SiteLanguage);
                    }
                    //investments
                    tab[line, INVEST_COLUMN_INDEX] = dtTotal.Rows[0][monthAlias];
                    line++;
                }
                //Univers total
                if (dtTotalUniverse != null && dtTotalUniverse.Rows.Count == 1 && dtTotalUniverse.Columns.Contains(monthAlias))
                {
                    //MonthInsertion ID Mois étudié								
                    tab[line, ID_MONTH_COLUMN_INDEX] = i;
                    //ID univers total
                    tab[line, ID_ELEMENT_COLUMN_INDEX] = ID_TOTAL_UNIVERSE_COLUMN_INDEX;
                    //label univers total
                    tab[line, LABEL_ELEMENT_COLUMN_INDEX] = GestionWeb.GetWebWord(1188, _session.SiteLanguage);
                    //investments
                    tab[line, INVEST_COLUMN_INDEX] = dtTotalUniverse.Rows[0][monthAlias];
                    line++;
                }
                //ID advertisers
                if (dt != null && dt.Rows.Count > 0 && dt.Columns.Contains(monthAlias) && dt.Columns.Contains("id_advertiser") && dt.Columns.Contains("advertiser"))
                {
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        //Insertion ID Mois étudié								
                        tab[line, ID_MONTH_COLUMN_INDEX] = i;
                        //ID annonceur
                        tab[line, ID_ELEMENT_COLUMN_INDEX] = dt.Rows[j]["id_advertiser"];
                        //libéllé annonceur
                        tab[line, LABEL_ELEMENT_COLUMN_INDEX] = dt.Rows[j]["advertiser"];
                        //investissment mensuel
                        tab[line, INVEST_COLUMN_INDEX] = dt.Rows[j][monthAlias];
                        line++;
                    }
                }
            }
            #endregion

            return tab;
        }
        #endregion

        #region Internal methods

        #region  Calculs totaux univers ou marché ou famille
        /// <summary>
        /// Retourne le nombre d'annonceurs ou références , le 1er annonceur ou référence et son investissement
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="isAdvertCalculation">Est ce que le calcule concerne les annonceurs</param>
		/// <param name="showProduct">True if show product</param>
        /// <returns> object[] avec nombre d'annonceurs , le 1er annonceur et son investissement</returns>
        protected object[] GetGlobalParams(DataTable dt, bool isAdvertCalculation,bool showProduct)
        {

            #region Variables annonceurs
            Decimal TempFirstAdInvest = (Decimal)0.0;
            string TempFirstAdInvestName = "";
            object TempForAdSov = "";
            object TempForRefSov = "";
            string currentMonth = "";
            ArrayList TempAdvertiserIds = new ArrayList();
            int MonthsInterval = (_periodEnd.Month - _periodBegin.Month) + 1;
            string[,] AdValues = null;
            string[,] RefValues = null;
            object[] AdObject = new object[5];
            if (MonthsInterval > 0)
                AdValues = new string[MonthsInterval, 3];
            RefValues = new string[MonthsInterval, 3];
            int start = 0;
            Int64 nbTabLines = 0;
            Int64 oldIdAdvertiser = 0;
            int indexLine = 0;
            int t = 0;
            #endregion

            #region variables références
            ArrayList IdProductComputedList = new ArrayList();
            Decimal TempFirstRefInvest = (Decimal)0.0;
            string TempFirstRefInvestName = "";
            int startRef = 0;
            int indexLineRef = 0;
            int nbProduct = 0;
            int nbAdvertiser = 0;
            Int64 oldIdProduct = 0;
            Int64 TotStartIndex = 0;
            object[,] tab = null;
            int[] TotNbRefByMonth = new int[MonthsInterval];
            #endregion

            #region Calcul investissement, 1er référence, 1er annonceur
            //Calcul nombre annonceurs, 1er annonceur et son investissement
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow currentRow in dt.Rows)
                {

                    #region annonceur
                    if (dt.Columns.Contains("id_advertiser") && oldIdAdvertiser != (Int64)currentRow["id_advertiser"])
                    {
                        //nombre d'annonceurs
                        nbAdvertiser++;
                        //Investissement annonceur courant
                        if (!isAdvertCalculation)
                        {
                            TempFirstAdInvestName = currentRow["advertiser"].ToString();
                            for (int s = (int)_periodBegin.Month; s <= (int)_periodEnd.Month; s++)
                            {
                                currentMonth = FctUtilities.Dates.GetMonthAlias(s, _iYearID, 3, _session);
                                if (FctUtilities.CheckedText.IsNotEmpty(currentMonth))
                                    TempFirstAdInvest = Decimal.Parse(dt.Compute("Sum(" + currentMonth + ")", "id_advertiser = " + currentRow["id_advertiser"].ToString() + "").ToString());
                                if (start == 0)
                                {
                                    AdValues[indexLine, 0] = s.ToString();
                                    AdValues[indexLine, 1] = TempFirstAdInvest.ToString();
                                    AdValues[indexLine, 2] = TempFirstAdInvestName;
                                }
                                else if (Decimal.Parse(AdValues[indexLine, 1].ToString()) < TempFirstAdInvest)
                                {
                                    AdValues[indexLine, 0] = s.ToString();
                                    AdValues[indexLine, 1] = TempFirstAdInvest.ToString();
                                    AdValues[indexLine, 2] = TempFirstAdInvestName;
                                }
                                indexLine++;
                            }
                            indexLine = 0;
                            TempFirstAdInvest = (Decimal)0.0;
                        }
                    }
                    start = 1;
                    oldIdAdvertiser = (Int64)currentRow["id_advertiser"];
                    #endregion

                    #region produit
                    if ( showProduct && oldIdProduct != (Int64)currentRow["id_product"] && !IdProductComputedList.Contains(Int64.Parse(currentRow["id_product"].ToString())) && start == 1)
                    {
                        //nombre de produits
                        nbProduct++;
                        //Investissement produit courant
                        if (!isAdvertCalculation)
                        {
                            TempFirstRefInvestName = currentRow["product"].ToString();
                            for (int p = (int)_periodBegin.Month; p <= (int)_periodEnd.Month; p++)
                            {
                                currentMonth = FctUtilities.Dates.GetMonthAlias(p, _iYearID, 3, _session);
                                if (FctUtilities.CheckedText.IsNotEmpty(currentMonth))
                                    TempFirstRefInvest = Decimal.Parse(dt.Compute("Sum(" + currentMonth + ")", "id_product = " + currentRow["id_product"].ToString() + "").ToString());
                                if (startRef == 0)
                                {
                                    RefValues[indexLineRef, 0] = p.ToString();
                                    RefValues[indexLineRef, 1] = TempFirstRefInvest.ToString();
                                    RefValues[indexLineRef, 2] = TempFirstRefInvestName.ToString();
                                }
                                else if (Decimal.Parse(RefValues[indexLineRef, 1].ToString()) < TempFirstRefInvest)
                                {
                                    RefValues[indexLineRef, 0] = p.ToString();
                                    RefValues[indexLineRef, 1] = TempFirstRefInvest.ToString();
                                    RefValues[indexLineRef, 2] = TempFirstRefInvestName.ToString();
                                }
                                indexLineRef++;
                            }
                            indexLineRef = 0;
                            TempFirstRefInvest = (Decimal)0.0;
                        }
                        IdProductComputedList.Add(Int64.Parse(currentRow["id_product"].ToString()));
                    }
                    startRef = 1;
                    oldIdProduct = (Int64)currentRow["id_product"];
                    #endregion

                }
            }

            #region totaux nombre de références
            try
            {
                if (!isAdvertCalculation)
                {
                    t = 0;
                    for (int n = (int)_periodBegin.Month; n <= (int)_periodEnd.Month; n++)
                    {
                        currentMonth = FctUtilities.Dates.GetMonthAlias(n, _iYearID, 3, _session);
                        TotNbRefByMonth[t] = int.Parse(dt.Compute("count(" + currentMonth + ")", "" + currentMonth.ToString() + " >0 ").ToString());
                        t++;
                    }
                }
            }
            catch (Exception nbRefErr)
            {
                throw (new ProductClassIndicatorsException("Impossible de déterminer le nombre de références pour chaque mois :", nbRefErr));
            }
            #endregion

            //Calcul nombre de référence par mois (seulement pour total univers ou famille ou marché)

            TotStartIndex = (Int64)1;
            //Nombre total de lignes
            if (isAdvertCalculation)
                nbTabLines = (nbAdvertiser > 0) ? ((Int64)nbAdvertiser * (Int64)MonthsInterval) + 1 : MonthsInterval + 1;
            else nbTabLines = (Int64)MonthsInterval + 1;
            // Tableau de résultat 	
            if (tab == null) tab = new object[nbTabLines, NB_MAX_COLUMN];
            if (!isAdvertCalculation)
            {
                tab = GlobalCompute(dt, tab, TotStartIndex, nbProduct, TotNbRefByMonth);
            }
            //On rempli le tableau avec 1er annonceur, 1ere référence, SOV par mois
            if (!isAdvertCalculation && tab != null)
            {
                indexLine = 0;
                t = 0;
                for (int m = (int)_periodBegin.Month; m <= (int)_periodEnd.Month; m++)
                {
                    //nolmbre de références par mois
                    if (TotNbRefByMonth != null) tab[TotStartIndex, REFERENCE_COLUMN_INDEX] = TotNbRefByMonth[t].ToString();
                    if (AdValues != null && AdValues.Length > 0)
                    {
                        //1er annonceur			
                        if (AdValues[indexLine, 2] != null) tab[TotStartIndex, FIRST_ADVERTISER_COLUMN_INDEX] = AdValues[indexLine, 2].ToString();
                        if (AdValues[indexLine, 1] != null)
                        {
                            //investissement 1er annonceur				
                            tab[TotStartIndex, FIRST_ADVERTISER_INVEST_COLUMN_INDEX] = AdValues[indexLine, 1].ToString();
                            //SOV 1er annonceur				
                            TempForAdSov = tab[TotStartIndex, INVESTMENT_COLUMN_INDEX];
                            tab[TotStartIndex, FIRST_ADVERTISER_SOV_COLUMN_INDEX] =
                                (TempForAdSov != null && Decimal.Parse(tab[TotStartIndex, INVESTMENT_COLUMN_INDEX].ToString()) > (Decimal)0.0) ? (Decimal.Parse(AdValues[indexLine, 1].ToString()) * (Decimal)100.0) / Decimal.Parse(tab[TotStartIndex, INVESTMENT_COLUMN_INDEX].ToString()) : (Decimal)0.0;
                        }
                    }
                    if (showProduct && RefValues != null && RefValues.Length > 0)
                    {
                        //1ere référence
                        if (RefValues[indexLine, 2] != null) tab[TotStartIndex, FIRST_PRODUCT_COLUMN_INDEX] = RefValues[indexLine, 2].ToString();
                        if (RefValues[indexLine, 2] != null)
                        {
                            //investissement 1ere référence
                            tab[TotStartIndex, FIRST_PRODUCT_INVEST_COLUMN_INDEX] = RefValues[indexLine, 1].ToString();
                            //SOV 1ere  référence = ((N-(N-1))*100)/N-1
                            TempForRefSov = tab[TotStartIndex, INVESTMENT_COLUMN_INDEX];
                            tab[TotStartIndex, FIRST_PRODUCT_SOV_COLUMN_INDEX] = (TempForRefSov != null && Decimal.Parse(tab[TotStartIndex, INVESTMENT_COLUMN_INDEX].ToString()) > (Decimal)0.0) ? (Decimal.Parse(RefValues[indexLine, 1].ToString()) * (Decimal)100.0) / Decimal.Parse(tab[TotStartIndex, INVESTMENT_COLUMN_INDEX].ToString()) : (Decimal)0.0;
                        }
                    }
                    TotStartIndex++;
                    indexLine++;
                    t++;
                }
            }
            #endregion

            AdObject[0] = nbAdvertiser;
            AdObject[1] = nbProduct;
            AdObject[2] = tab;
            AdObject[3] = nbTabLines;
            AdObject[4] = TotNbRefByMonth;

            return AdObject;
        }
        #endregion

        #region calcul totaux investissement, evolution, nb référence, budget moyen
        /// <summary>
        /// calcule totaux investissement, evolution, nb référence, budget moyen
        /// </summary>
        /// <param name="dt">Datatable source de données</param>
        /// <param name="tab">tableau objet à 2 dimension</param>
        /// <param name="StartIndex">index debut tableau</param>
        /// <param name="TotalRef">nombre total de références</param>
        /// <param name="TotNbRefByMonth">nombre total de références par mois</param>
        /// <returns>tableau (resultats) objet à 2 dimension</returns>
        protected object[,] GlobalCompute(DataTable dt, object[,] tab, Int64 StartIndex, Int64 TotalRef, int[] TotNbRefByMonth)
        {

            #region calcul total univers

            #region variables
            Decimal TotalInvestByMonth = (Decimal)0.0;
            Decimal TotalComparativeInvestByMonth = (Decimal)0.0;
            Decimal Evolution = (Decimal)0.0;
            Decimal BudgetAverage = (Decimal)0.0;
            //Int64 TotStartIndex=TOTAL_LINE_START_INDEX;
            Int64 TotStartIndex = StartIndex;
            int t = 0;
            #endregion

            //Calcul des investissements Pour année N
            for (int s = (int)_periodBegin.Month; s <= (int)_periodEnd.Month; s++)
            {
                TotalInvestByMonth = TotalInvestmentByMonth(s, FctUtilities.Dates.yearID(_periodBegin, _session), dt);
                tab[TotStartIndex, INVESTMENT_COLUMN_INDEX] = TotalInvestByMonth;
                try
                {
                    //Calcul du nombre de références 
                    tab[TotStartIndex, REFERENCE_COLUMN_INDEX] = TotalRef;
                    //Calcul du budget moyen par mois sélectionné
                    if (TotNbRefByMonth != null && TotNbRefByMonth.Length > 0)
                    {
                        BudgetAverage = (Decimal.Parse(TotNbRefByMonth[t].ToString()) > (Decimal)0.0) ? TotalInvestByMonth / Decimal.Parse(TotNbRefByMonth[t].ToString()) : TotalInvestByMonth;
                        tab[TotStartIndex, AVERAGE_BUDGET_COLUMN_INDEX] = BudgetAverage;
                    }
                    else tab[TotStartIndex, AVERAGE_BUDGET_COLUMN_INDEX] = TotalInvestByMonth;
                }
                catch (Exception nbRefErr)
                {
                    throw (new ProductClassIndicatorsException("Impossible de déterminer le budget moyen de chaque mois :", nbRefErr));
                }
                //Calcul des investissements Pour année N-1 si étude comparative
                if (_session.ComparativeStudy)
                {
                    TotalComparativeInvestByMonth = TotalInvestmentByMonth(s, FctUtilities.Dates.yearID(_periodBegin.AddYears(-1), _session), dt);
                    tab[TotStartIndex, COMPAR_INVESTMENT_COLUMN_INDEX] = TotalComparativeInvestByMonth;
                    //Calcul de l'evolution anneé N par rapport N-1	= ((N-(N-1))*100)/N-1 
                    Evolution = (TotalComparativeInvestByMonth > (Decimal)0.0) ? ((TotalInvestByMonth - TotalComparativeInvestByMonth) * (Decimal)100.0) / TotalComparativeInvestByMonth : (Decimal)0.0;
                    tab[TotStartIndex, EVOLUTION_COLUMN_INDEX] = Evolution;
                }
                TotStartIndex++;
                t++;
            }
            return tab;
            #endregion
        
        }
        #endregion

        #region Calcul total investissement par mois
        /// <summary>
        /// Calcul investissement total par mois
        /// </summary>
        /// <param name="monthID">ID mois</param>
        /// <param name="YearID">ID année etudiée</param>
        /// <param name="dt">source de données</param>
        /// <param name="webSession">Session client</param>
        /// <returns>investissement total par mois</returns>
        protected Decimal TotalInvestmentByMonth(int monthID, int YearID, DataTable dt)
        {
            string currentMonth = "";
            Decimal Invest = (Decimal)0.0;
            currentMonth = FctUtilities.Dates.GetMonthAlias(monthID, YearID, 3, _session);
            if (FctUtilities.CheckedText.IsNotEmpty(currentMonth))
                Invest = Decimal.Parse(dt.Compute("Sum(" + currentMonth + ")", "").ToString());
            return Invest;
        }
        #endregion

        #region FillMonthIndexColum
        /// <summary>
        /// Rempli le tableau d'objets avec les libéllés des mois sélectionnés
        /// </summary>
        /// <param name="tab">tableau bidimensionnelle d'object</param>
        /// <param name="nbTabLines">nombre maximale de lignes</param>
        /// <param name="MONTH_COLUMN_INDEX">index de colonne pour le libéllé d'un mois</param>
        /// <param name="VALUE_MONTH_COLUMN_INDEX">index de colonne pour l'identifiant d'un mois</param>
        /// <param name="PeriodEndDate">fin de la période étudiée</param>
        /// <returns>tabelau objet 2 dimensions</returns>
        protected object[,] FillMonthIndexColum(object[,] tab, Int64 nbTabLines, DateTime PeriodEndDate, Int64 MONTH_COLUMN_INDEX, Int64 VALUE_MONTH_COLUMN_INDEX)
        {

            int year = 0;
            int tempMonth = _periodBegin.Month;
            int downLoadDate = _session.DownLoadDate;
            if (DateTime.Now.Year > downLoadDate)
            {
                if (PeriodEndDate.Year == DateTime.Now.Year - 1) year = 0;
                else if (PeriodEndDate.Year == DateTime.Now.Year - 2) year = 1;
                else if (PeriodEndDate.Year == DateTime.Now.Year - 3) year = 2;
            }
            else
            {
                if (PeriodEndDate.Year == DateTime.Now.Year - 1) year = 1;
                else if (PeriodEndDate.Year == DateTime.Now.Year - 2) year = 2;
            }
            for (int l = 1; l < nbTabLines; l++)
            {
                tab[l, MONTH_COLUMN_INDEX] = FctUtilities.Dates.GetMonthAlias(tempMonth, year, 3, _session);
                tab[l, VALUE_MONTH_COLUMN_INDEX] = tempMonth;
                if (tempMonth == _periodEnd.Month) tempMonth = _periodBegin.Month - 1;
                tempMonth++;
            }
            return tab;
        }
        #endregion

        #region AppendLine
        /// <summary>
        /// Append cells to the current line
        /// </summary>
        /// <param name="str">Object to fill</param>
        /// <param name="lineLabel">Label of the line</param>
        /// <param name="cssLabel">Style of text cells</param>
        /// <param name="cssNb">Style of number cells</param>
        /// <param name="invest">Investments object</param>
        /// <param name="evol">Evolution object</param>
        /// <param name="refNb">Number of reference</param>
        /// <param name="avgInvest">Average of investments</param>
        /// <param name="advLabel">First Advertiser Label</param>
        /// <param name="advInvest">First Advertiser Investments</param>
        /// <param name="advSOV">First Advertiser Share of voice</param>
        /// <param name="refLabel">First Product Label</param>
        /// <param name="refInvest">First Product Investments</param>
        /// <param name="refSOV">First Product Share of voice</param>
        protected void AppendLine(StringBuilder str, string lineLabel, string cssLabel, string cssNb, object invest, object evol, object refNb, object avgInvest, object advLabel, object advInvest, object advSOV, object refLabel, object refInvest, object refSOV)
        {
			bool showProduct = _session.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
            string img = "/I/p.gif";
            str.AppendFormat("<td nowrap class=\"{1}\">{0}</td>", lineLabel, cssLabel);
            if (invest != null)
            {
                str.AppendFormat("<td nowrap  class=\"{0}{2}\">{1}</td>", (_excel ? "" : "violetBackGroundV3 "), FctUtilities.Units.ConvertUnitValueAndPdmToString(invest, _session.Unit, false), cssNb);
            }
            else
            {
                str.AppendFormat("<td nowrap  class=\"{0}{2}\">-</td>", (_excel ? "" : "violetBackGroundV3 "), cssNb);
            }
            if (_session.ComparativeStudy)
            {

                if (evol != null)
                {
                    Decimal d = Convert.ToDecimal(evol);
                    if (d > 0)
                    {
                        img = "/I/g.gif";
                    }
                    else if (d < 0)
                    {
                        img = "/I/r.gif";
                    }
                    else
                    {
                        img = "/I/o.gif";
                    }
                    if (!_excel)
                    {
                        str.AppendFormat("<td nowrap class=\"violetBackGroundV3 {2}\">{0}%&nbsp;<img src={1}></td>", FctUtilities.Units.ConvertUnitValueAndPdmToString(d, _session.Unit, true), img, cssNb);
                    }
                    else
                    {
                        str.AppendFormat("<td nowrap class=\"{1}\">{0}%</td>", FctUtilities.Units.ConvertUnitValueAndPdmToString(d, _session.Unit, true), cssNb);
                    }
                }
                else
                {
                    str.AppendFormat("<td  nowrap class=\"{0}{1}\">-</td>", (_excel ? "" : "violetBackGroundV3 "), cssNb);
                }

            }
            if (refNb != null)
            {
                str.AppendFormat("<td nowrap class=\"{1}\">{0}</td>", FctUtilities.Units.ConvertUnitValueToString(refNb, CstUnit.euro), cssNb);
            }
            else
            {
                str.AppendFormat("<td nowrap class=\"{0}\">-</td>", cssNb);
            }
            if (avgInvest != null)
            {
                str.AppendFormat("<td nowrap class=\"{1}\">{0}</td>", FctUtilities.Units.ConvertUnitValueToString(avgInvest, _session.Unit), cssNb);
            }
            else
            {
                str.AppendFormat("<td nowrap class=\"{0}\">-</td>", cssNb);
            }
            //Separator
            if (!_excel)
            {
                str.Append("<td class=\"violetBackGround columnSeparator\"><img width=1px></td>");
            }
            if ((Convert.ToDecimal(advInvest) / (Decimal)1000) > 0)
            {
                //1er advertiser (optionnels)
                if (advLabel != null)
                {
                    str.AppendFormat("<td nowrap class=\"{1}\">{0}</td>", advLabel, cssLabel);
                }
                if (advInvest != null)
                {
                    str.AppendFormat("<td nowrap class=\"{1}\">{0}</td>", FctUtilities.Units.ConvertUnitValueToString(advInvest, _session.Unit), cssNb);
                }
                if (advSOV != null)
                {
                    str.AppendFormat("<td class=\"{1}\">{0}%</td>", FctUtilities.Units.ConvertUnitValueAndPdmToString(advSOV, _session.Unit, true), cssNb);
                }
				if (showProduct) {
					//Separator
					if (!_excel) {
						str.Append("<td class=\"violetBackGround columnSeparator\"><img width=1px></td>");
					}
					//1st refrrence (optionnels)	
					if (refLabel != null) {
						str.AppendFormat("<td nowrap class=\"{1}\">{0}</td>", refLabel, cssLabel);
					}
					if (refInvest != null) {
						str.AppendFormat("<td nowrap class=\"{1}\">{0}</td>", FctUtilities.Units.ConvertUnitValueToString(refInvest, _session.Unit), cssNb);
					}
					if (refSOV != null) {
						str.AppendFormat("<td class=\"{1}\">{0}%</td>", FctUtilities.Units.ConvertUnitValueAndPdmToString(refSOV, _session.Unit, true), cssNb);
					}
				}
            }
            else
            {
                str.AppendFormat("<td class=\"{0}\">-</td><td class=\"{0}\">-</td><td class=\"{0}\">-</td>", cssNb);
                //Separator
                if (!_excel)
                {
                    str.Append("<td class=\"violetBackGround columnSeparator\"><img width=1px></td>");
                }
                str.AppendFormat("<td class=\"{0}\">-</td><td class=\"{0}\">-</td><td class=\"{0}\">-</td>", cssNb);
            }

        }
        #endregion

        #endregion
    
    }
}