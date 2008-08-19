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
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;

using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using CstComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstMediaStrategy = TNS.AdExpress.Constantes.FrameWork.Results.MediaStrategy;
using CstUnit = TNS.AdExpress.Constantes.Web.CustomerSessions.Unit;
using CstDbClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstPreformatedDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstInvestmentType = TNS.AdExpress.Constantes.FrameWork.Results.MediaStrategy.InvestmentType;
using CstRight = TNS.AdExpress.Constantes.Customer.Right;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;


using TNS.AdExpress.Classification;
using TNS.Classification.Universe;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpressI.ProductClassIndicators.DAL;
using TNS.AdExpressI.ProductClassIndicators.Exceptions;
using System.Collections;
using TNS.FrameWork.Date;

namespace TNS.AdExpressI.ProductClassIndicators.Engines
{
    /// <summary>
    /// Engine to build a Top report or to provide computed data for top report or indicator
    /// </summary>
    public class EngineMediaStrategy:Engine
    {

        #region Constantes

        #region Constantes index colonnes
        /// <summary>
        /// COLONNE PDM
        /// </summary>
        public const int PDM_COLUMN_INDEX = 22;
        /// <summary>
        /// COLONNE EVOLUTION
        /// </summary>
        public const int EVOL_COLUMN_INDEX = 23;
        /// <summary>
        /// COLONNE ID PREMIER ANNONCEUR
        /// </summary>
        public const int ID_FIRST_ADVERT_COLUMN_INDEX = 24;
        /// <summary>
        /// COLONNE LIBELLE PREMIER ANNONCEUR
        /// </summary>
        public const int LABEL_FIRST_ADVERT_COLUMN_INDEX = 25;
        /// <summary>
        /// COLONNE INVESTISSSEMENT  PREMIER ANNONCEUR
        /// </summary>
        public const int INVEST_FIRST_ADVERT_COLUMN_INDEX = 26;
        /// <summary>
        /// COLONNE ID PREMIERE REFENCE
        /// </summary>
        public const int ID_FIRST_REF_COLUMN_INDEX = 27;
        /// <summary>
        /// COLONNE LIBELLE PREMIER ANNONCEUR
        /// </summary>
        public const int LABEL_FIRST_REF_COLUMN_INDEX = 28;
        /// <summary>
        /// COLONNE INVESTISSSEMENT  PREMIER ANNONCEUR
        /// </summary>
        public const int INVEST_FIRST_REF_COLUMN_INDEX = 29;
        /// <summary>
        /// NOMBRE MAXIMAL DE COLONNES POUR LE TABLEAU
        /// </summary>
        public const int NB_MAX_COLUMNS = 30;
        /// <summary>
        /// NOMBRE MAXIMAL DE COLONNES POUR LE TABLEAU (sortie graphique)
        /// </summary>
        public const int NB_CHART_MAX_COLUMNS = 22;
        #endregion

        #region Constantes index colonnes
        /// <summary>
        /// COLONNE  INVESTISSEMENT ANNONCEUR DE REFERENCE OU CONCURRENT ANNEE N
        /// </summary>
        public const int REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX = 0;
        /// <summary>
        /// COLONNE ID  Vehicle
        /// </summary>
        public const int ID_VEHICLE_COLUMN_INDEX = 1;
        /// <summary>
        /// COLONNE LIBELLE Vehicle
        /// </summary>
        public const int LABEL_VEHICLE_COLUMN_INDEX = 2;
        /// <summary>
        /// COLONNE ID CATEGORIE
        /// </summary>
        public const int ID_CATEGORY_COLUMN_INDEX = 3;
        /// <summary>
        /// COLONNE LIBELLE CATEGORIE
        /// </summary>
        public const int LABEL_CATEGORY_COLUMN_INDEX = 4;
        /// <summary>
        /// COLONNE ID MEDIA
        /// </summary>
        public const int ID_MEDIA_COLUMN_INDEX = 5;
        /// <summary>
        /// COLONNE LIBELLE MEDIA
        /// </summary>
        public const int LABEL_MEDIA_COLUMN_INDEX = 6;
        /// <summary>
        /// COLONNE ID ANNONCEURS DE REFERENCE OU CONCURRENTS
        /// </summary>
        public const int ID_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX = 7;
        /// <summary>
        /// OLONNE LIBELLE ANNONCEURS DE REFERENCE OU CONCURRENTS
        /// </summary>
        public const int LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX = 8;
        /// <summary>
        /// COLONNE MEDIA (VEHICLE) INVESTISSMENT TOTAL UNIVERS
        /// </summary>
        public const int TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX = 9;
        /// <summary>
        /// COLONNE MEDIA (VEHICLE) INVESTISSEMENT TOTAL FAMILLE
        /// </summary>
        public const int TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX = 10;
        /// <summary>
        /// COLONNE MEDIA (VEHICLE) INVESTISSEMENT TOTAL MARCHE
        /// </summary>
        public const int TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX = 11;
        /// <summary>
        /// COLONNE CATEGORIE INVESTISSMENT TOTAL UNIVERS
        /// </summary>
        public const int TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX = 12;
        /// <summary>
        /// COLONNE CATEGORIE INVESTISSEMENT TOTAL FAMILLE
        /// </summary>
        public const int TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX = 13;
        /// <summary>
        /// COLONNE CATEGORIE INVESTISSEMENT TOTAL MARCHE
        /// </summary>
        public const int TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX = 14;
        /// <summary>
        /// COLONNE CATEGORIE INVESTISSMENT TOTAL UNIVERS
        /// </summary>
        public const int TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX = 15;
        /// <summary>
        /// COLONNE CATEGORIE INVESTISSEMENT TOTAL FAMILLE
        /// </summary>
        public const int TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX = 16;
        /// <summary>
        /// COLONNE CATEGORIE INVESTISSEMENT TOTAL MARCHE
        /// </summary>
        public const int TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX = 17;
        /// <summary>
        /// COLONNE INVESTISSMENT TOTAL UNIVERS
        /// </summary>
        public const int TOTAL_UNIV_INVEST_COLUMN_INDEX = 18;
        /// <summary>
        /// COLONNE INVESTISSEMENT TOTAL FAMILLE
        /// </summary>
        public const int TOTAL_SECTOR_INVEST_COLUMN_INDEX = 19;
        /// <summary>
        /// COLONNE  INVESTISSEMENT TOTAL MARCHE
        /// </summary>
        public const int TOTAL_MARKET_INVEST_COLUMN_INDEX = 20;
        /// <summary>
        /// COLONNE  INVESTISSEMENT ANNONCEUR DE REFERENCE OU CONCURRENT ANNEE N POUR TOTAL 		
        /// </summary>
        public const int TOTAL_REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX = 21;

        #endregion

        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="dalLayer">Data Access Layer</param>
        public EngineMediaStrategy(WebSession session, IProductClassIndicatorsDAL dalLayer)
            : base(session, dalLayer)
        { 
        }
        #endregion

        #region GetResult
        /// <summary>
        /// Cr�e le code HTML pour afficher le tableau de la r�partition m�dia sur le total de la p�riode 
        /// contenant les �l�ments ci-apr�s :
        /// en ligne :
        /// -le total famille (en option uniquement en s�lection de groupe/vari�t�s) ou le
        /// total march� (en option)
        /// -les �l�ments de r�f�rences
        /// -les �l�ments concurrents 
        /// en colonne :
        /// -Les investissements de la p�riode N
        /// -une PDM (part de march� ) exprimant la r�partition media en %
        /// -une �volution N vs N-1 en % (uniquement dans le cas d'une �tude comparative)
        /// -le 1er annonceur en Keuros uniquement  pour les lignes total produits �ventuels
        /// -la 1ere r�f�rence en Keuros uniquement  pour les lignes total produits �ventuels
        /// Sur la dimension support le tableau est d�clin� de la fa�on suivante :
        /// -si plusieurs media, le tableua sera d�clin� par media
        /// -si un seul media, le tableau sera d�clin� par media, cat�gorie et supports
        /// </summary>				
        /// <param name="page">Page qui affiche les stat�gies m�dia</param>
        /// <param name="tab">tableau des r�sultats</param>	
        /// <param name="webSession">Session du client</param>
        /// <param name="excel">bool�en pour sortie html ou excel</param>	
        /// <returns>Code HTML</returns>		
        public override StringBuilder GetResult()
        {

            #region GetData
            object[,] tab = this.GetTableData();
            #endregion

            System.Text.StringBuilder t = new System.Text.StringBuilder(10000);

            #region Pas de donn�es � afficher
            if (tab.GetLength(0) == 0)
            {
                return t.AppendFormat("<div align=\"center\" class=\"txtViolet11Bold\">{0}</div>", GestionWeb.GetWebWord(177, _session.SiteLanguage));
            }
            #endregion

            #region Constantes
            string cssHeader = "p2";
            string cssL1Label = "pmtotal";
            string cssL1Nb = "pmtotalnb";
            string cssL2Label = "asl5";
            //string cssL2nb = (_excel) ? "p142xls" : "asl5 asl5nb";
            string cssL2nb = "asl5nb";
            string cssL3Label = "asl5b";
            //string cssL3Nb = (_excel) ? "asl5bxls" : "asl5bnb"; ;
            string cssL3Nb = "asl5bnb"; ;
            string cssRefLabel = "p15";
            string cssRefNb = "p151";
            string cssCompLabel = (_excel) ? "p142" : "p14";
            string cssCompNb = (_excel) ? "p143" : "p141";
            #endregion

            //Table begin
            t.Append("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" >");

            #region headers
            t.Append("<tr>");
            t.Append("<td nowrap  colspan=2 class=\"p2\">");
            t.Append("&nbsp;");
            t.Append("</td>");
            t.Append("<!--<td nowrap  class=\"p2\">");
            t.Append("&nbsp;");
            t.Append("</td>-->");
            //Separator
            if (!_excel)
            {
                t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
            }
            //Media label
            t.AppendFormat("<td nowrap class=\"{0}\">{1} {2}</td>", cssHeader, GestionWeb.GetWebWord(1246, _session.SiteLanguage), _periodEnd.Year);

            //Advertiser or product total
            t.AppendFormat("<td nowrap class=\"{0}\" >{1}</td>", cssHeader, GestionWeb.GetWebWord(806, _session.SiteLanguage));
            //Evol
            if (_session.ComparativeStudy)
            {
                t.AppendFormat("<td nowrap class=\"{0}\">{1} {2}/{3}</td>", cssHeader, GestionWeb.GetWebWord(1168, _session.SiteLanguage), _periodEnd.Year, (_periodEnd.Year - 1));
            }
            //Separator
            if (!_excel)
            {
                t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
            }
            //First Advertiser
            t.AppendFormat("<td nowrap class=\"{0}\">{1}</td>", cssHeader, GestionWeb.GetWebWord(1154, _session.SiteLanguage));
            //Investments
            t.AppendFormat("<td nowrap class=\"{0}\">{1} {2}</td>", cssHeader, GestionWeb.GetWebWord(1246, _session.SiteLanguage), _periodEnd.Year);
            //Separator
            if (!_excel)
            {
                t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
            }
            //First product
            t.AppendFormat("<td nowrap class=\"{0}\">{1}</td>", cssHeader, GestionWeb.GetWebWord(1155, _session.SiteLanguage));
            //Investments
            t.AppendFormat("<td nowrap class=\"{0}\">{1} {2}</td>", cssHeader, GestionWeb.GetWebWord(1246, _session.SiteLanguage), _periodEnd.Year);
            t.Append("</tr>");
            #endregion

            #region Build table
            object oLevelLabel = null;
            object oLabel = null;
            object oTotal = null;
            object oPDM = null;
            object oEvol = null;
            object oAdvLabel = null;
            object oAdvInvest = null;
            object oRefLabel = null;
            object oRefInvest = null;
            string cssLabel = string.Empty;
            string cssNb = string.Empty;

            for (int i = 0; i < tab.GetLength(0); i++)
            {

                #region TOTAL MARKET OR SECTOR (IF PLURI)
                if (tab[i, TOTAL_MARKET_INVEST_COLUMN_INDEX] != null || tab[i, TOTAL_SECTOR_INVEST_COLUMN_INDEX] != null)
                {
                    if ((oTotal = tab[i, TOTAL_MARKET_INVEST_COLUMN_INDEX]) != null)
                    {
                        oLabel = GestionWeb.GetWebWord(1190, _session.SiteLanguage);
                    }
                    else
                    {
                        oLabel = GestionWeb.GetWebWord(1189, _session.SiteLanguage);
                        oTotal = tab[i, TOTAL_SECTOR_INVEST_COLUMN_INDEX];
                    }

                    cssLabel = cssL1Label;
                    cssNb = cssL1Nb;

                    oLevelLabel = GestionWeb.GetWebWord(210, _session.SiteLanguage);
                    oPDM        = tab[i, PDM_COLUMN_INDEX];
                    oEvol       = tab[i, EVOL_COLUMN_INDEX];
                    oAdvLabel = tab[i, LABEL_FIRST_ADVERT_COLUMN_INDEX];
                    oAdvInvest = tab[i, INVEST_FIRST_ADVERT_COLUMN_INDEX];
                    oRefLabel = tab[i, LABEL_FIRST_REF_COLUMN_INDEX];
                    oRefInvest = tab[i, INVEST_FIRST_REF_COLUMN_INDEX];

                    AppendLine(cssLabel, cssNb, t, oLevelLabel, oLabel, oTotal, oPDM, oEvol, oAdvLabel, oAdvInvest, oRefLabel, oRefInvest);
                }
                #endregion

                #region TOTAL MARKET OR SECTOR BY VEHICLE 
                if (tab[i, TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX] != null || tab[i, TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX] != null)
                {
                    if ((oTotal = tab[i, TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX]) != null)
                    {
                        oLabel = GestionWeb.GetWebWord(1190, _session.SiteLanguage);
                    }
                    else
                    {
                        oLabel = GestionWeb.GetWebWord(1189, _session.SiteLanguage);
                        oTotal = tab[i, TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX];
                    }

                    cssLabel = cssL1Label;
                    cssNb = cssL1Nb;

                    oLevelLabel = tab[i, LABEL_VEHICLE_COLUMN_INDEX];
                    oPDM = tab[i, PDM_COLUMN_INDEX];
                    oEvol = tab[i, EVOL_COLUMN_INDEX];
                    oAdvLabel = tab[i, LABEL_FIRST_ADVERT_COLUMN_INDEX];
                    oAdvInvest = tab[i, INVEST_FIRST_ADVERT_COLUMN_INDEX];
                    oRefLabel = tab[i, LABEL_FIRST_REF_COLUMN_INDEX];
                    oRefInvest = tab[i, INVEST_FIRST_REF_COLUMN_INDEX];

                    AppendLine(cssLabel, cssNb, t, oLevelLabel, oLabel, oTotal, oPDM, oEvol, oAdvLabel, oAdvInvest, oRefLabel, oRefInvest);

                }
                #endregion

                #region UNIVERS TOTAL
                if (tab[i, TOTAL_UNIV_INVEST_COLUMN_INDEX] != null || tab[i, TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX] != null)
                {
                    if (tab[i, TOTAL_UNIV_INVEST_COLUMN_INDEX] != null)
                    {
                        oTotal = tab[i, TOTAL_UNIV_INVEST_COLUMN_INDEX];
                    }
                    else
                    {
                        oTotal = tab[i, TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX];
                    }

                    cssLabel = cssL1Label;
                    cssNb = cssL1Nb;

                    oLevelLabel = null;
                    oLabel = GestionWeb.GetWebWord(1188, _session.SiteLanguage);
                    oPDM = tab[i, PDM_COLUMN_INDEX];
                    oEvol = tab[i, EVOL_COLUMN_INDEX];
                    oAdvLabel = tab[i, LABEL_FIRST_ADVERT_COLUMN_INDEX];
                    oAdvInvest = tab[i, INVEST_FIRST_ADVERT_COLUMN_INDEX];
                    oRefLabel = tab[i, LABEL_FIRST_REF_COLUMN_INDEX];
                    oRefInvest = tab[i, INVEST_FIRST_REF_COLUMN_INDEX];

                    AppendLine(cssLabel, cssNb, t, oLevelLabel, oLabel, oTotal, oPDM, oEvol, oAdvLabel, oAdvInvest, oRefLabel, oRefInvest);

                }
                #endregion

                #region Total cat
                if (TreatCategory())
                {
                    cssLabel = cssL2Label;
                    cssNb = cssL2nb;

                    #region Market or Sector total
                    if (tab[i, TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX] != null || tab[i, TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX] != null)
                    {

                        if ((oTotal = tab[i, TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX]) != null)
                        {
                            oLabel = GestionWeb.GetWebWord(1190, _session.SiteLanguage);
                        }
                        else
                        {
                            oLabel = GestionWeb.GetWebWord(1189, _session.SiteLanguage);
                            oTotal = tab[i, TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX];
                        }

                        oLevelLabel = tab[i, LABEL_CATEGORY_COLUMN_INDEX];
                        oPDM = tab[i, PDM_COLUMN_INDEX];
                        oEvol = tab[i, EVOL_COLUMN_INDEX];
                        oAdvLabel = tab[i, LABEL_FIRST_ADVERT_COLUMN_INDEX];
                        oAdvInvest = tab[i, INVEST_FIRST_ADVERT_COLUMN_INDEX];
                        oRefLabel = tab[i, LABEL_FIRST_REF_COLUMN_INDEX];
                        oRefInvest = tab[i, INVEST_FIRST_REF_COLUMN_INDEX];

                        AppendLine(cssLabel, cssNb, t, oLevelLabel, oLabel, oTotal, oPDM, oEvol, oAdvLabel, oAdvInvest, oRefLabel, oRefInvest);

                    }
                    #endregion

                    #region Univers total
                    // lignes total univers pour chaque cat�gorie s�lectionn�e
                    if (tab[i, TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX] != null)
                    {

                        oLevelLabel = null;
                        oLabel = GestionWeb.GetWebWord(1188, _session.SiteLanguage);
                        oTotal = tab[i, TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX];
                        oPDM = tab[i, PDM_COLUMN_INDEX];
                        oEvol = tab[i, EVOL_COLUMN_INDEX];
                        oAdvLabel = tab[i, LABEL_FIRST_ADVERT_COLUMN_INDEX];
                        oAdvInvest = tab[i, INVEST_FIRST_ADVERT_COLUMN_INDEX];
                        oRefLabel = tab[i, LABEL_FIRST_REF_COLUMN_INDEX];
                        oRefInvest = tab[i, INVEST_FIRST_REF_COLUMN_INDEX];

                        AppendLine(cssLabel, cssNb, t, oLevelLabel, oLabel, oTotal, oPDM, oEvol, oAdvLabel, oAdvInvest, oRefLabel, oRefInvest);

                    }
                    #endregion

                }
                #endregion

                #region Media
                if (TreatMedia())
                {

                    cssLabel = cssL3Label;
                    cssNb = cssL3Nb;

                    #region Market or sector total
                    if (tab[i, TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX] != null || tab[i, TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX] != null)
                    {
                        if ((oTotal = tab[i, TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX]) != null)
                        {
                            oLabel = GestionWeb.GetWebWord(1190, _session.SiteLanguage);
                        }
                        else
                        {
                            oLabel = GestionWeb.GetWebWord(1189, _session.SiteLanguage);
                            oTotal = tab[i, TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX];
                        }

                        oLevelLabel = tab[i, LABEL_MEDIA_COLUMN_INDEX];
                        oPDM = tab[i, PDM_COLUMN_INDEX];
                        oEvol = tab[i, EVOL_COLUMN_INDEX];
                        oAdvLabel = tab[i, LABEL_FIRST_ADVERT_COLUMN_INDEX];
                        oAdvInvest = tab[i, INVEST_FIRST_ADVERT_COLUMN_INDEX];
                        oRefLabel = tab[i, LABEL_FIRST_REF_COLUMN_INDEX];
                        oRefInvest = tab[i, INVEST_FIRST_REF_COLUMN_INDEX];

                        AppendLine(cssLabel, cssNb, t, oLevelLabel, oLabel, oTotal, oPDM, oEvol, oAdvLabel, oAdvInvest, oRefLabel, oRefInvest);

                    }
                    #endregion

                    #region Univers total
                    if (tab[i, TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX] != null)
                    {
                        oLevelLabel = null;
                        oLabel = GestionWeb.GetWebWord(1188, _session.SiteLanguage);
                        oTotal = tab[i, TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX];
                        oPDM = tab[i, PDM_COLUMN_INDEX];
                        oEvol = tab[i, EVOL_COLUMN_INDEX];
                        oAdvLabel = tab[i, LABEL_FIRST_ADVERT_COLUMN_INDEX];
                        oAdvInvest = tab[i, INVEST_FIRST_ADVERT_COLUMN_INDEX];
                        oRefLabel = tab[i, LABEL_FIRST_REF_COLUMN_INDEX];
                        oRefInvest = tab[i, INVEST_FIRST_REF_COLUMN_INDEX];

                        AppendLine(cssLabel, cssNb, t, oLevelLabel, oLabel, oTotal, oPDM, oEvol, oAdvLabel, oAdvInvest, oRefLabel, oRefInvest);

                    }
                    #endregion

                }
                #endregion

                #region Reference and competitor elements
                if ((tab[i, REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] != null || tab[i, TOTAL_REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] != null)
                    &&(
                        (tab[i, ID_CATEGORY_COLUMN_INDEX] == null && tab[i, ID_MEDIA_COLUMN_INDEX] == null)
                        || (tab[i, ID_CATEGORY_COLUMN_INDEX] != null && tab[i, ID_MEDIA_COLUMN_INDEX] == null && ((_session.PreformatedMediaDetail == CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory) || (_session.PreformatedMediaDetail == CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia)))
                        || (tab[i, ID_CATEGORY_COLUMN_INDEX] != null && tab[i, ID_MEDIA_COLUMN_INDEX] != null && (_session.PreformatedMediaDetail == CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia))
                        )
                    )
                    {

                        if (tab[i, ID_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null){
                            if (_referenceIDS.Contains(Convert.ToInt64(tab[i, ID_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX])))
                            {
                                cssLabel = cssRefLabel;
                                cssNb = cssRefNb;

                            }
                            else if (_competitorIDS.Contains(Convert.ToInt64(tab[i, ID_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX])))
                            {
                                cssLabel = cssCompLabel;
                                cssNb = cssCompNb;
                            }
                        }
                        oLevelLabel = null;
                        oLabel = tab[i, LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX];
                        if (tab[i, TOTAL_REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] != null)
                        {
                            oTotal = tab[i, TOTAL_REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX];
                        }
                        else
                        {
                            oTotal = tab[i, REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX];
                        }
                        oPDM = tab[i, PDM_COLUMN_INDEX];
                        oEvol = tab[i, EVOL_COLUMN_INDEX];

                        AppendLine(cssLabel, cssNb, t, oLevelLabel, oLabel, oTotal, oPDM, oEvol, null, null, null, null);

                }
                #endregion

            }
            #endregion

            //End Table
            t.Append("</table>");

            return t;

        }

        #region AppendLine
        protected void AppendLine(string cssLabel, string cssNb, System.Text.StringBuilder t, object oLevelLabel, object oLabel, object oTotal, object oPDM, object oEvol, object oAdvLabel, object oAdvInvest, object oRefLabel, object oRefInvest)
        {
            t.Append("<tr>");
            if (oLevelLabel != null)
            {
                t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssLabel, oLevelLabel);
            }
            else
            {
                t.AppendFormat("<td class=\"{0}\" nowrap>&nbsp;</td>", cssLabel);
            }
            t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssLabel, oLabel);
            //Separator
            if (!_excel)
            {
                t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
            }
            if (oTotal != null)
            {
                t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, FctUtilities.Units.ConvertUnitValueToString(oTotal, _session.Unit));
            }
            else
            {
                t.AppendFormat("<td class=\"{0}\" nowrap>&nbsp;</td>", cssNb);
            }
            //PDM
            if (oPDM != null)
            {
                t.AppendFormat("<td class=\"{0}\" nowrap>{1}%</td>", cssNb, FctUtilities.Units.ConvertUnitValueAndPdmToString(oPDM, _session.Unit, true));
            }
            else
            {
                t.AppendFormat("<td class=\"{0}\" nowrap>&nbsp;</td>", cssNb);
            }
            //Evol
            if (_session.ComparativeStudy)
            {
                if (oEvol != null)
                {
                    double dEvol = Convert.ToDouble(oEvol);
                    string image = string.Empty;
                    if (!_excel)
                    {
                        if (dEvol > 0)
                        {
                            image = " <img src=\"/I/g.gif\"/>";
                        }
                        else if (dEvol < 0)
                        {
                            image = " <img src=\"/I/r.gif\"/>";
                        }
                        else
                        {
                            image = " <img src=\"/I/o.gif\"/>";
                        }
                    }
                    t.AppendFormat("<td class=\"{0}\" nowrap>{1}%{2}</td>", cssNb, FctUtilities.Units.ConvertUnitValueAndPdmToString(dEvol, _session.Unit, true), image);
                }
                else
                {
                    t.AppendFormat("<td class=\"{0}\" nowrap>&nbsp;</td>", cssNb);
                }
            }
            //Separator
            if (!_excel)
            {
                t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
            }
            //First Advertiser
            if (oAdvLabel != null)
            {
                t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssLabel, oAdvLabel);
            }
            else
            {
                t.AppendFormat("<td class=\"{0}\" nowrap>&nbsp;</td>", cssLabel);
            }
            if (oAdvInvest != null)
            {
                t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, FctUtilities.Units.ConvertUnitValueToString(oAdvInvest, _session.Unit));
            }
            else
            {
                t.AppendFormat("<td class=\"{0}\" nowrap>&nbsp;</td>", cssNb);
            }
            //Separator
            if (!_excel)
            {
                t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
            }
            //First Advertiser
            if (oRefLabel != null)
            {
                t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssLabel, oRefLabel);
            }
            else
            {
                t.AppendFormat("<td class=\"{0}\" nowrap>&nbsp;</td>", cssLabel);
            }
            if (oRefInvest != null)
            {
                t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, FctUtilities.Units.ConvertUnitValueToString(oRefInvest, _session.Unit));
            }
            else
            {
                t.AppendFormat("<td class=\"{0}\" nowrap>&nbsp;</td>", cssNb);
            }
            t.Append("</tr>");
        }
        #endregion

        #endregion

        #region GetTableData
        /// <summary>
        /// La r�partition m�dia sur le total de la p�riode dans un tableau
        /// contenant les �l�ments ci-apr�s :
        /// en ligne :
        /// -le total famille (en option uniquement en s�lection de groupe/vari�t�s) ou le
        /// total march� (en option)
        /// -les �l�ments de r�f�rences
        /// -les �l�ments concurrents 
        /// en colonne :
        /// -Les investissements de la p�riode N
        /// -une PDM (part de march� ) exprimant la r�partition media en %
        /// -une �volution N vs N-1 en % (uniquement dans le cas d'une �tude comparative)
        /// -le 1er annonceur en Keuros uniquement  pour les lignes total produits �ventuels
        /// -la 1ere r�f�rence en Keuros uniquement  pour les lignes total produits �ventuels
        /// Sur la dimension support le tableau est d�clin� de la fa�on suivante :
        /// -si plusieurs media, le tableau sera d�clin� par media
        /// -si un seul media, le tableau sera d�clin� par media, cat�gorie et supports
        /// </summary>
        /// <returns>tableau de r�sultats</returns>
        public object[,] GetTableData()
        {

            #region variables
            CstComparaisonCriterion comparisonCriterion = this._session.ComparaisonCriterion;
            //donn�es 1ere �l�ment plurim�dia
            #region variables tempon
            //	string tempInvest="";
            string tempEvol = "";
            string tempPDM = "";
            #endregion

            #region variables pour calculer les PDM (part de march�)
            //liste des investissments total pour l'ensemble de l'�tude
            Hashtable hPdmTotAdvert = new Hashtable();
            //liste des investissments par m�dia (vehicle)
            Hashtable hPdmVehAdvert = new Hashtable();
            //liste des investissments par cat�gorie
            Hashtable hPdmCatAdvert = new Hashtable();
            //			//PDM d'un media (vehicle)
            //			double PdmVehicle = (double)0.0;
            //			//PDM d'un support
            //			double PdmMedia  = (double)0.0;
            //			//PDM d'une cat�gorie
            //			double	PdmCategory = (double)0.0;
            #endregion

            #region variables pour les dimensions du tableau de r�sultats
            // tableau de resultats
            object[,] tab = null;
            //nombre maximal de lignes
            //int nbMaxLines = 0;
            //Index d'une ligne du tableau
            int indexTabRow = 0;
            //bool�en incr�mentation d'une ligne
            bool increment = false;
            #endregion

            #region Variables annonceurs de r�f�rences ou concurrents
            //identifiant  annonceurs de r�f�rences ou concurrents
            string idAdvertiser = "";
            //nombre  annonceurs de r�f�rences ou concurrents
            //int nbAdvertiser=0;
            //Y a t'il des annonceurs de r�f�rences ou concurrents
            bool hasAdvertiser = false;
            //Investissement d'un annonceur sur la p�riode N
            //string AdvertiserInvest="";
            //Investissement d'un annonceur sur la p�riode N-1
            //string AdvertiserInvest_N1="";
            //Investissement total des annonceurs sur la p�riode N
            string AdvertiserTotalInvest = "";
            //Investissement total des annonceurs sur la p�riode N-1
            string AdvertiserTotalInvest_N1 = "";
            //Investissement d'un annonceur par cat�gorie sur la p�riode N
            string AdvertiserInvestByCat = "";
            //Investissement d'un annonceur par cat�gorie sur la p�riode N-1
            string AdvertiserInvestByCat_N1 = "";
            //investissement pour une cat�gorie sur p�riode N
            //double CatInvest=(double)0.0;
            //investissement pour une cat�gorie sur p�riode N-1
            //double CatInvest_N1=(double)0.0;
            //Investissement d'un annonceur par m�dia (vehicle) sur la p�riode N
            string AdvertiserInvestByVeh = "";
            //Investissement d'un annonceur par m�dia (vehicle) sur la p�riode N-1
            string AdvertiserInvestByVeh_N1 = "";
            //Evolution N/N-1 pour total annonceurs
            //double AdvertiserTotalEvolution = (double)0.0;
            //Evolution N/N-1 pour un annonceur
            //double AdvertiserEvolution = (double)0.0;
            //collection des annonceurs d�j� trait�s
            ArrayList OldIdAdvertiserArr = new ArrayList();
            //collection des annonceurs d�j� trait�s pour le total univers
            ArrayList inTotUnivAdvertAlreadyUsedArr = new ArrayList();
            //collection des annonceurs d�j� trait�s pour un m�dia (vehicle)
            ArrayList inAdvertVehicleAlreadyUsedArr = new ArrayList();
            //collection des annonceurs d�j� trait�s pour une cxat�gorie
            ArrayList inAdvertCategoryAlreadyUsedArr = new ArrayList();
            //collection des annonceurs d�j� trait�s pour un support (media)
            ArrayList inAdvertMediaAlreadyUsedArr = new ArrayList();
            //			string OldIdAdvertiser="";				
            //			string OldIdCatForAdvert="0";
            //			string containt="";
            #endregion

            #region Variables des supports
            //depart de la boucle 
            int start = 0;
            //identifiant d'un m�dia (vehicle)
            string idVehicle = "";
            //identifiant m�dia pr�c�dent
            string OldIdVehicle = "0";
            //lib�ll� d'un m�dia
            string Vehicle = "";
            //nombre de m�dia (vehicle)
            //int nbVehicle=0;
            //identifiant d'une cat�gorie
            string idCategory = "";
            //identifiant pr�c�dent d'une cat�gorie
            string OldIdCategory = "0";
            //collection identifiant pr�c�dentes cat�gories
            ArrayList OldIdCategoryArr = new ArrayList();
            //lib�ll� d'une cat�gorie
            string Category = "";
            //nombre de cat�gories
            //int nbCategory=0;
            //identifiant d'un support
            string idMedia = "";
            //lib�ll� d'un support
            string Media = "";
            //identifiant du pr�c�dent support
            string OldIdMedia = "0";
            //collection identifiant pr�c�dents supports
            ArrayList OldIdMediaArr = new ArrayList();
            //nombre de supports				
            //double TotalUnivVehicleEvolution =(double)0.0;						
            #endregion

            #region construction des listes de produits, media, annonceurs s�lectionn�s
            RecapUniversSelection recapUniversSelection = new RecapUniversSelection(_session);
            string AdvertiserAccessList = recapUniversSelection.AdvertiserAccessList;
            string CompetitorAdvertiserAccessList = recapUniversSelection.CompetitorAdvertiserAccessList;
            string CategoryAccessList = recapUniversSelection.CategoryAccessList;
            string MediaAccessList = recapUniversSelection.MediaAccessList;
            string VehicleAccessList = recapUniversSelection.VehicleAccessList;
            #endregion

            #region variable pour total univers
            //Y a t'il des donn�es pour calculer les valeurs du total univers
            bool hasTotalUniv = false;
            //Collecton des m�dia d�j� trait�
            ArrayList inTotUnivVehicleAlreadyUsedArr = new ArrayList();
            //Collecton des cat�gories d�j� trait�
            ArrayList inTotUnivCategoryAlreadyUsedArr = new ArrayList();
            //Collecton des supports d�j� trait�
            ArrayList inTotUnivMediaAlreadyUsedArr = new ArrayList();
            //investissement premier annonceur	
            //	string  maxFirstAdvertInvest = "";
            //investissement premiere r�f�rence	
            //	string  maxFirstProdInvest = "";
            //investissment total univers par support p�riode N
            string TotalUnivMediaInvest = "";
            //investissment total univers par support p�riode N-1
            string TotalUnivMediaInvest_N1 = "";
            //investissment total univers par m�dia p�riode N
            string TotalUnivVehicleInvest = "";
            //investissment total univers par m�dia p�riode N-1
            string TotalUnivVehicleInvest_N1 = "";
            //investissment total univers par cat�gorie p�riode N
            string TotalUnivCategoryInvest = "";
            //investissment total univers par cat�gorie p�riode N
            string TotalUnivCategoryInvest_N1 = "";
            //investissment total univers p�riode N
            string TotalUnivInvest = "";
            //investissment total univers p�riode N-1
            string TotalUnivInvest_N1 = "";
            //Evolution N/N-1 total univers
            //double TotalUnivEvolution = (double)0.0;
            //Evolution N/N-1 par support
            //double TotalUnivMediaEvolution = (double)0.0;	
            //Evolution N/N-1 par cat�gorie
            //double TotalUnivCategoryEvolution = (double)0.0;
            //groupe de donn�es pour univers
            DataSet dsTotalUniverse = null;
            //table de donn�es pour univers
            DataTable dtTotalUniverse = null;
            #region ancienne version
            //groupe de donn�es pour premiere r�f�rence univers
            //DataSet dsTotalUniverseFirstProduct = null;
            //table de donn�es pour premiere r�f�rence univers
            //DataTable dtTotalUniverseFirstProduct = null;
            //groupe de donn�es pour premier annonceur univers
            //DataSet dsTotalUniverseFirstAvertiser = null;	
            //table de donn�e pour premier annonceur univers
            //DataTable dtTotalUniverseFirstAvertiser = null;	
            #endregion
            #region variables 1er �l�ment par m�dia pour univers
            //table de donn�es pour 1ere r�f�rence par m�dia (vehicle) pour total univers
            DataTable dtUniverse1stProductByVeh = null;
            //table de donn�es pour 1er annonceur par m�dia (vehicle) pour total univers
            DataTable dtUniverse1stAdvertiserByVeh = null;
            //table de donn�es pour 1ere r�f�rence par cat�gorie pour total univers
            DataTable dtUniverse1stProductByCat = null;
            //table de donn�es pour 1er annonceur par cat�gorie pour total univers
            DataTable dtUniverse1stAdvertiserByCat = null;
            //table de donn�es pour 1ere r�f�rence par support pour total univers
            DataTable dtUniverse1stProductByMed = null;
            //table de donn�es pour 1er annonceur par support pour total univers
            DataTable dtUniverse1stAdvertiserByMed = null;
            #endregion
            #endregion

            #region variable pour total march� ou famille
            // y a t'il des donn�es pour le total march�
            bool hasTotalMarketOrSector = false;
            //investissemnt total march� par support p�riode N
            string TotalMarketOrSectorMediaInvest = "";
            //investissemnt total march� par support p�riode N-1
            string TotalMarketOrSectorMediaInvest_N1 = "";
            //investissemnt total cat�gorie par cat�gorie p�riode N
            string TotalMarketOrSectorCategoryInvest = "";
            //investissemnt total march� par cat�gorie p�riode N-1
            string TotalMarketOrSectorCategoryInvest_N1 = "";
            //investissemnt total march� par m�dia p�riode N
            string TotalMarketOrSectorVehicleInvest = "";
            //investissemnt total march� par m�dia p�riode N-1
            string TotalMarketOrSectorVehicleInvest_N1 = "";
            //investissemnt total march�  p�riode N
            string TotalMarketOrSectorInvest = "";
            //investissemnt total march�  p�riode N-1
            string TotalMarketOrSectorInvest_N1 = "";
            //identifiant plurim�dia
            ArrayList pluriArr = new ArrayList();
            //identifiant des m�dias d�j� trait�s
            ArrayList inTotMarketOrSectorVehicleAlreadyUsedArr = new ArrayList();
            //identifiant des cat�gories d�j� trait�s
            ArrayList inTotMarketOrSectorCategoryAlreadyUsedArr = new ArrayList();
            //identifiant des supports d�j� trait�s
            ArrayList inTotMarketOrSectorMediaAlreadyUsedArr = new ArrayList();
            //Evolution total march� par support
            //double TotalMarketOrSectorMediaEvolution = (double)0.0;
            //Evolution total march� par m�dia
            //double TotalMarketOrSectorVehicleEvolution = (double)0.0;
            //Evolution total march� par cat�gorie
            //double TotalMarketOrSectorCategoryEvolution = (double)0.0;
            //Evolution total march�
            //double TotalMarketOrSectorEvolution = (double)0.0;
            //Groupe de donn�es pour total march�
            DataSet dsTotalMarketOrSector = null;
            //table de donn�es pour total march�
            DataTable dtTotalMarketOrSector = null;
            #region ancienne version
            //Groupe de donn�es premiere r�f�rence total march�
            //DataSet dsTotalMarketOrSectorFirstProduct = null;
            //table de donn�es premiere r�f�rence total march�
            //DataTable dtTotalMarketOrSectorFirstProduct = null;
            //Goupe de donn�es premier annonceur total march�
            //DataSet dsTotalMarketOrSectorFirstAdvertiser = null;
            //table de donn�es premier annonceur total march�
            //DataTable dtTotalMarketOrSectorFirstAdvertiser = null;
            #endregion
            #region variables 1er �l�ment par m�dia pour march� ou famille
            //table de donn�es pour 1ere r�f�rence par m�dia (vehicle) pour total march� ou famille
            DataTable dtMarketOrSector1stProductByVeh = null;
            //table de donn�es pour 1er annonceur par m�dia (vehicle) pour  total march� ou famille
            DataTable dtMarketOrSector1stAdvertiserByVeh = null;
            //table de donn�es pour 1ere r�f�rence par cat�gorie pour  total march� ou famille
            DataTable dtMarketOrSector1stProductByCat = null;
            //table de donn�es pour 1er annonceur par cat�gorie pour  total march� ou famille
            DataTable dtMarketOrSector1stAdvertiserByCat = null;
            //table de donn�es pour 1ere r�f�rence par support pour  total march� ou famille
            DataTable dtMarketOrSector1stProductByMed = null;
            //table de donn�es pour 1er annonceur par support pour  total march� ou famille
            DataTable dtMarketOrSector1stAdvertiserByMed = null;
            #endregion
            #endregion

            #region variable pour annonceur de r�f�rences ou concurrent
            //chaine pour colonne � r�cuperer dans collection de DataRow annonceurs
            string strExpr = "";
            //filtre pour colonne � r�cuperer dans collection de DataRow annonceurs
            string strSort = "";
            //DataRow annonceurs
            DataRow[] foundRows = null;
            //Groupe de donn�es annonceurs de r�f�rence ou concurrents
            DataSet dsAdvertiser = null;
            //Table de donn�es annonceurs de r�f�rences ou concurrents
            DataTable dtAdvertiser = null;
            #endregion

            #endregion
            try
            {
                #region Chargement des donn�es

                #region chargement des donn�es pour les annonceurs de r�f�rences et/ou concurrents
                /* Chargement des donn�es pour les annonceurs de r�f�rences et,ou concurrents
				 * A partir de ces donn�es on peut calculer l'investissement,l'evolution,le PDM,
				 * pour chaque annonceur et par niveau support
				 */
                if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserAccessList))
                {
                    dsAdvertiser = _dalLayer.GetMediaStrategyTblData(CstResult.MotherRecap.ElementType.advertiser, CstComparaisonCriterion.universTotal, true);
                    if (dsAdvertiser != null && dsAdvertiser.Tables[0] != null && dsAdvertiser.Tables[0].Rows.Count > 0)
                    {
                        dtAdvertiser = dsAdvertiser.Tables[0];
                    }
                }
                #endregion

                #region chargement des donn�es totaux univers
                /*Chargement des donn�es pour les totaux univers
					* dsTotalUniverse : permet de r�cuperer pour chaque niveau support
					* l'investissement,l'evolution,le PDM
					*/
                dsTotalUniverse = _dalLayer.GetMediaStrategyTblData(CstResult.MotherRecap.ElementType.advertiser, CstComparaisonCriterion.universTotal, false);
                if (dsTotalUniverse != null && dsTotalUniverse.Tables[0] != null && dsTotalUniverse.Tables[0].Rows.Count > 0)
                {
                    dtTotalUniverse = dsTotalUniverse.Tables[0];
                }
                /*Chargement des donn�es pour les totaux univers
                    * dsTotalUniverseFirstProduct : permet de r�cuperer pour chaque niveau support
                    * l'investissement et le libell� de la premi�re r�f�rence
                    */
                CstResult.MediaStrategy.MediaLevel mediaLevel = CstResult.MediaStrategy.MediaLevel.vehicleLevel;
                mediaLevel = SwitchMedia();
                Get1stElmtDataTbleByMedia(ref dtUniverse1stProductByVeh, ref dtUniverse1stAdvertiserByVeh, ref dtUniverse1stProductByCat, ref dtUniverse1stAdvertiserByCat, ref dtUniverse1stProductByMed, ref dtUniverse1stAdvertiserByMed, CstComparaisonCriterion.universTotal, mediaLevel);
                #endregion

                #region chargement des donn�es totaux march�s ou famille(s)
                /* Chargement des donn�es pour les annonceurs de r�f�rences et,ou concurrents
				 * A partir de ces donn�es on peut calculer l'investissement,l'evolution,le PDM,
				 * pour chaque annonceur et par niveau support
				 */
                if (comparisonCriterion == CstComparaisonCriterion.sectorTotal)
                    dsTotalMarketOrSector = _dalLayer.GetMediaStrategyTblData(CstResult.MotherRecap.ElementType.advertiser, CstComparaisonCriterion.sectorTotal, false);
                else if (comparisonCriterion == CstComparaisonCriterion.marketTotal)
                    dsTotalMarketOrSector = _dalLayer.GetMediaStrategyTblData(CstResult.MotherRecap.ElementType.advertiser, CstComparaisonCriterion.marketTotal, false);
                if (dsTotalMarketOrSector != null && dsTotalMarketOrSector.Tables[0] != null && dsTotalMarketOrSector.Tables[0].Rows.Count > 0)
                {
                    dtTotalMarketOrSector = dsTotalMarketOrSector.Tables[0];
                }
                /*Chargement des donn�es pour les totaux univers
                    * dsTotalMarketOrSectorFirstProduct : permet de r�cuperer pour chaque niveau support
                    * l'investissement et le lib�ll� de la premi�re r�f�rence
                    */
                Get1stElmtDataTbleByMedia(ref dtMarketOrSector1stProductByVeh, ref dtMarketOrSector1stAdvertiserByVeh, ref dtMarketOrSector1stProductByCat, ref dtMarketOrSector1stAdvertiserByCat, ref dtMarketOrSector1stProductByMed, ref dtMarketOrSector1stAdvertiserByMed, comparisonCriterion, mediaLevel);
                #endregion

                #endregion

                #region  Construction du tableau de r�sultats

                #region Instanciation du tableau de r�sultats
                /*On instancie le tableau de r�sultats pour strat�gie m�dia
				 */
                //cr�ation du tableau 				
                tab = TabInstance(dtTotalMarketOrSector, dtAdvertiser, VehicleAccessList, NB_MAX_COLUMNS);
                // Il n'y a pas de donn�es
                if (tab == null) return (new object[0, 0]);
                #endregion

                #region Remplissage chaque ligne m�dia dans table
                /* Chaque ligne du tableau contient toutes les donn�es d'un annonceur de r�f�rence ou concurrent
				 * (Investissement,Evolution,PDM,lib�ll�) ou d'un total support (media ou cat�gorie ou support)
				 * avec ses param�tres (Investissement,Evolution,PDM,lib�ll�, 1er annonceur et son investissement, idem pour 1er r�f�rence)
				*/
                if (dtTotalMarketOrSector != null && dtTotalMarketOrSector.Rows.Count > 0)
                {
                    /*Si utilisateur a s�lectionn� PLURIMEDIA on calcule investissement total march� ou famille et univers, et
                     * les param�tres associ�s (PDM,evolution,1er annonceurs et r�f�rences)
                     */
                    if (FctUtilities.CheckedText.IsNotEmpty(VehicleAccessList) && CstDbClassif.Vehicles.names.plurimedia == (CstDbClassif.Vehicles.names)int.Parse(VehicleAccessList))
                    {
                        #region ligne total march� ou famille pour PLURIMEDIA
                        //Traitement ligne total march� ou famille pour Plurimedia : Investissment,Evolution,PDM,1er nnonceur et son investissment, premi�re r�f�rence et son investissment
                        if (dtTotalMarketOrSector.Columns.Contains("total_N") && !hasTotalMarketOrSector)
                        {
                            ComputeInvestPdmEvol(dtTotalMarketOrSector, ref TotalMarketOrSectorInvest, ref TotalMarketOrSectorInvest, ref TotalMarketOrSectorInvest_N1, ref tempPDM, ref tempEvol, "Sum(total_N)", "Sum(total_N1)", "", _session.ComparativeStudy);
                            // Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� et plurim�dia 
                            if (FctUtilities.CheckedText.IsNotEmpty(TotalMarketOrSectorInvest.Trim()))
                            {
                                tab = FillTabInvestPdmEvol(tab, indexTabRow, VehicleAccessList, GestionWeb.GetWebWord(210, _session.SiteLanguage), true, TotalMarketOrSectorInvest, "100", tempEvol, _session.ComparaisonCriterion, CstResult.MediaStrategy.InvestmentType.total, CstPreformatedDetail.PreformatedMediaDetails.vehicle);
                                increment = true;
                            }
                            tab = FillTabFisrtElmt(tab, comparisonCriterion, TotalMarketOrSectorInvest, ref indexTabRow, increment);
                            increment = false;
                            hasTotalMarketOrSector = true;
                        }
                        #endregion

                        #region ligne univers pour PLURIMEDIA
                        //Traitement ligne total univers pour Plurimedia : Investissment,Evolution,PDM,1er nnonceur et son investissment, premi�re r�f�rence et son investissment
                        if (dtTotalUniverse.Columns.Contains("total_N") && !hasTotalUniv)
                        {
                            ComputeInvestPdmEvol(dtTotalUniverse, ref TotalUnivInvest, ref TotalUnivInvest, ref TotalUnivInvest_N1, ref tempPDM, ref tempEvol, "Sum(total_N)", "Sum(total_N1)", "", _session.ComparativeStudy);
                            // Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et plurim�dia 
                            if (FctUtilities.CheckedText.IsNotEmpty(TotalUnivInvest.Trim()))
                            {
                                tab = FillTabInvestPdmEvol(tab, indexTabRow, VehicleAccessList, GestionWeb.GetWebWord(210, _session.SiteLanguage), true, TotalUnivInvest, "100", tempEvol, CstComparaisonCriterion.universTotal, CstResult.MediaStrategy.InvestmentType.total, CstPreformatedDetail.PreformatedMediaDetails.vehicle);
                                increment = true;
                            }
                            tab = FillTabFisrtElmt(tab, CstComparaisonCriterion.universTotal, TotalUnivInvest, ref indexTabRow, increment);
                            increment = false;//ligne suivante
                            hasTotalUniv = true;
                        }
                        #endregion

                        #region lignes  annonceurs de r�f�rences ou concurrents	pour s�lection PLURIMEDIA
                        //Pour les annonceurs de r�f�rence ou concurrents s�lectionn�s par PLURIMEDIA : Investissment,Evolution,PDM
                        if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserAccessList) && dtAdvertiser != null && dtAdvertiser.Rows.Count > 0 && !hasAdvertiser)
                        {
                            //Pour chaque ligne  TOTAL annonceur de r�f�rence ou concurrent on r�cup�re les donn�es							
                            FillAdvertisers(tab, dtAdvertiser, dtTotalMarketOrSector, "Sum(total_N)", "Sum(total_N1)", "", inTotUnivAdvertAlreadyUsedArr, ref hPdmTotAdvert, ref hPdmTotAdvert, ref AdvertiserTotalInvest, ref AdvertiserTotalInvest_N1, ref indexTabRow, VehicleAccessList, idVehicle, Vehicle, true, ref hasAdvertiser);
                        }
                        #endregion
                    }
                    //Pour chaque ligne  media du total univers on r�cup�re les donn�es										
                    foreach (DataRow currentRow in dtTotalMarketOrSector.Rows)
                    {

                        #region lignes univers et march� ou famille par m�dia (vehicle)
                        /*Si utilisateur a s�lectionn� un MEDIA (vehicle) on calcule investissement total march� ou famille et univers, et
					 * les param�tres associ�s (PDM,evolution,1er annonceurs et r�f�rences)
					 */
                        #region ligne total march� ou famille par m�dia (vehicle)
                        //Colonne m�dia (vehicle)
                        if (dtTotalMarketOrSector.Columns.Contains("id_vehicle") && dtTotalMarketOrSector.Columns.Contains("vehicle"))
                        {
                            idVehicle = currentRow["id_vehicle"].ToString();
                            Vehicle = currentRow["vehicle"].ToString();
                            if (!inTotMarketOrSectorVehicleAlreadyUsedArr.Contains(idVehicle))
                            {
                                if (dtTotalMarketOrSector.Columns.Contains("total_N"))
                                {
                                    //investissement total univers pour m�dia (vehicle)									
                                    if (CstDbClassif.Vehicles.names.plurimedia == (CstDbClassif.Vehicles.names)int.Parse(VehicleAccessList))
                                        ComputeInvestPdmEvol(dtTotalMarketOrSector, ref TotalMarketOrSectorInvest, ref TotalMarketOrSectorVehicleInvest, ref TotalMarketOrSectorVehicleInvest_N1, ref tempPDM, ref tempEvol, "Sum(total_N)", "Sum(total_N1)", "id_vehicle = " + idVehicle + "", _session.ComparativeStudy);
                                    else ComputeInvestPdmEvol(dtTotalMarketOrSector, ref TotalMarketOrSectorVehicleInvest, ref TotalMarketOrSectorVehicleInvest, ref TotalMarketOrSectorVehicleInvest_N1, ref tempPDM, ref tempEvol, "Sum(total_N)", "Sum(total_N1)", "id_vehicle = " + idVehicle + "", _session.ComparativeStudy);
                                    // Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et media (vehicle) 
                                    if (FctUtilities.CheckedText.IsNotEmpty(TotalMarketOrSectorVehicleInvest.Trim()))
                                    {
                                        tab = FillTabInvestPdmEvol(tab, indexTabRow, idVehicle, Vehicle, false, TotalMarketOrSectorVehicleInvest, tempPDM, tempEvol, _session.ComparaisonCriterion, CstResult.MediaStrategy.InvestmentType.total, CstPreformatedDetail.PreformatedMediaDetails.vehicle);
                                        increment = true;
                                    }
                                }
                                tab = FillTabFisrtElmt(tab, ref TotalMarketOrSectorVehicleInvest, dtMarketOrSector1stAdvertiserByVeh, dtMarketOrSector1stProductByVeh, "id_vehicle", idVehicle, ref inTotMarketOrSectorVehicleAlreadyUsedArr, ref indexTabRow, increment, false);
                                increment = false;
                            }
                        }
                        #endregion

                        #region ligne total univers par m�dia (vehicle)
                        if (dtTotalUniverse != null && dtTotalUniverse.Columns.Contains("id_vehicle") && dtTotalUniverse.Columns.Contains("vehicle"))
                        {
                            idVehicle = currentRow["id_vehicle"].ToString();
                            Vehicle = currentRow["vehicle"].ToString();
                            if (!inTotUnivVehicleAlreadyUsedArr.Contains(idVehicle))
                            {
                                if (dtTotalUniverse.Columns.Contains("total_N"))
                                {
                                    //investissement total univers pour m�dia (vehicle)									
                                    if (CstDbClassif.Vehicles.names.plurimedia == (CstDbClassif.Vehicles.names)int.Parse(VehicleAccessList))
                                        ComputeInvestPdmEvol(dtTotalUniverse, ref TotalUnivInvest, ref TotalUnivVehicleInvest, ref TotalUnivVehicleInvest_N1, ref tempPDM, ref tempEvol, "Sum(total_N)", "Sum(total_N1)", "id_vehicle = " + idVehicle + "", _session.ComparativeStudy);
                                    else ComputeInvestPdmEvol(dtTotalUniverse, ref TotalUnivVehicleInvest, ref TotalUnivVehicleInvest, ref TotalUnivVehicleInvest_N1, ref tempPDM, ref tempEvol, "Sum(total_N)", "Sum(total_N1)", "id_vehicle = " + idVehicle + "", _session.ComparativeStudy);
                                    // Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et media (vehicle) 
                                    if (FctUtilities.CheckedText.IsNotEmpty(TotalUnivVehicleInvest.Trim()))
                                    {
                                        tab = FillTabInvestPdmEvol(tab, indexTabRow, idVehicle, Vehicle, false, TotalUnivVehicleInvest, tempPDM, tempEvol, CstComparaisonCriterion.universTotal, CstResult.MediaStrategy.InvestmentType.total, CstPreformatedDetail.PreformatedMediaDetails.vehicle);
                                        increment = true;
                                    }
                                }
                                tab = FillTabFisrtElmt(tab, ref TotalUnivVehicleInvest, dtUniverse1stAdvertiserByVeh, dtUniverse1stProductByVeh, "id_vehicle", idVehicle, ref inTotUnivVehicleAlreadyUsedArr, ref indexTabRow, increment, false);
                                increment = false;

                        #endregion

                                if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserAccessList))
                                {
                                    #region Investissement,PDM,EVOL pour annonceur de r�f�rence ou concurrent Niveau m�dia (vehicle)
                                    //Pour les annonceurs de r�f�rence ou concurrents s�lectionn�s par MEDIA (vehicle) : Investissment,Evolution,PDM
                                    if (dtAdvertiser != null && dtAdvertiser.Rows.Count > 0 && dtAdvertiser.Columns.Contains("id_vehicle") && dtAdvertiser.Columns.Contains("id_advertiser"))
                                    {
                                        //Pour chaque ligne  annonceur de r�f�rence ou concurrent on r�cup�re les donn�es
                                        if ((int.Parse(idVehicle) != int.Parse(OldIdVehicle)))
                                        {
                                            inAdvertVehicleAlreadyUsedArr = null;
                                            inAdvertVehicleAlreadyUsedArr = new ArrayList();
                                            hPdmVehAdvert = null;
                                            hPdmVehAdvert = new Hashtable();
                                            strSort = " id_vehicle=" + idVehicle;
                                            FillAdvertisers(tab, dtAdvertiser, dtTotalMarketOrSector, "Sum(total_N)", "Sum(total_N1)", strSort, inAdvertVehicleAlreadyUsedArr, ref hPdmVehAdvert, ref hPdmTotAdvert, ref AdvertiserInvestByVeh, ref AdvertiserInvestByVeh_N1, ref indexTabRow, VehicleAccessList, idVehicle, Vehicle, false, ref hasAdvertiser);
                                        }
                                    }


                                    #endregion
                                }
                            }
                        }
                        #endregion

                        #region lignes univers et march� ou famille par cat�gorie
                        /*Si utilisateur a s�lectionn� des cat�gories on calcule investissement total march� ou famille et univers, et
					 * les param�tres associ�s (PDM,evolution,1er annonceurs et r�f�rences)
					 */
                        if (TreatCategory())
                        {
                            #region ligne total march� ou famille par cat�gorie

                            if (dtTotalMarketOrSector.Columns.Contains("id_category") && dtTotalMarketOrSector.Columns.Contains("category"))
                            {
                                idCategory = currentRow["id_category"].ToString();
                                Category = currentRow["category"].ToString();
                                //On vide la liste des cat�gories anciennement trait�s d�s qu'on change de m�dia (vehicle)
                                if (start == 1 && (int.Parse(idVehicle) != int.Parse(OldIdVehicle)) && inTotMarketOrSectorCategoryAlreadyUsedArr != null && inTotMarketOrSectorCategoryAlreadyUsedArr.Count > 0)
                                {
                                    inTotMarketOrSectorCategoryAlreadyUsedArr = null;
                                    inTotMarketOrSectorCategoryAlreadyUsedArr = new ArrayList();
                                }
                                if (!inTotMarketOrSectorCategoryAlreadyUsedArr.Contains(idCategory))
                                {
                                    if (dtTotalMarketOrSector.Columns.Contains("total_N"))
                                    {
                                        //investissement total march� ou famille par cat�gorie										
                                        ComputeInvestPdmEvol(dtTotalMarketOrSector, ref TotalMarketOrSectorVehicleInvest, ref TotalMarketOrSectorCategoryInvest, ref TotalMarketOrSectorCategoryInvest_N1, ref tempPDM, ref tempEvol, "Sum(total_N)", "Sum(total_N1)", "id_vehicle=" + idVehicle + " AND id_category = " + idCategory, _session.ComparativeStudy);
                                        // Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et cat�gorie
                                        if (FctUtilities.CheckedText.IsNotEmpty(TotalMarketOrSectorCategoryInvest.Trim()))
                                        {
                                            tab = FillTabInvestPdmEvol(tab, indexTabRow, "", "", idCategory, Category, "", "", "", "", false, TotalMarketOrSectorCategoryInvest, tempPDM, tempEvol, _session.ComparaisonCriterion, CstResult.MediaStrategy.InvestmentType.total, CstPreformatedDetail.PreformatedMediaDetails.vehicleCategory);
                                            increment = true;
                                        }
                                    }
                                    tab = FillTabFisrtElmt(tab, ref TotalMarketOrSectorCategoryInvest, dtMarketOrSector1stAdvertiserByCat, dtMarketOrSector1stProductByCat, "id_category", idCategory, ref inTotMarketOrSectorCategoryAlreadyUsedArr, ref indexTabRow, increment, false);
                                    increment = false;
                                }
                            }

                            #endregion

                            #region ligne total univers par cat�gorie
                            if (dtTotalUniverse != null && dtTotalUniverse.Columns.Contains("id_category") && dtTotalUniverse.Columns.Contains("category"))
                            {
                                idCategory = currentRow["id_category"].ToString();
                                Category = currentRow["category"].ToString();
                                //On vide la liste des cat�gories trait�s lorsqu'on change de m�dia (vehicle)
                                if (start == 1 && (int.Parse(idVehicle) != int.Parse(OldIdVehicle)) && inTotUnivCategoryAlreadyUsedArr != null && inTotUnivCategoryAlreadyUsedArr.Count > 0)
                                {
                                    inTotUnivCategoryAlreadyUsedArr = null;
                                    inTotUnivCategoryAlreadyUsedArr = new ArrayList();
                                }
                                //pour chaque cat�gorie distincte
                                if (!inTotUnivCategoryAlreadyUsedArr.Contains(idCategory))
                                {
                                    if (dtTotalUniverse.Columns.Contains("total_N"))
                                    {
                                        //investissement total univers par cat�gorie										
                                        ComputeInvestPdmEvol(dtTotalUniverse, ref TotalUnivVehicleInvest, ref TotalUnivCategoryInvest, ref TotalUnivCategoryInvest_N1, ref tempPDM, ref tempEvol, "Sum(total_N)", "Sum(total_N1)", "id_vehicle=" + idVehicle + " AND id_category = " + idCategory, _session.ComparativeStudy);
                                        // Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et cat�gorie 
                                        if (FctUtilities.CheckedText.IsNotEmpty(TotalUnivCategoryInvest.Trim()))
                                        {
                                            tab = FillTabInvestPdmEvol(tab, indexTabRow, "", "", idCategory, Category, "", "", "", "", false, TotalUnivCategoryInvest, tempPDM, tempEvol, CstComparaisonCriterion.universTotal, CstResult.MediaStrategy.InvestmentType.total, CstPreformatedDetail.PreformatedMediaDetails.vehicleCategory);
                                            increment = true;
                                        }
                                    }
                                    tab = FillTabFisrtElmt(tab, ref TotalUnivCategoryInvest, dtUniverse1stAdvertiserByCat, dtUniverse1stProductByCat, "id_category", idCategory, ref inTotUnivCategoryAlreadyUsedArr, ref indexTabRow, increment, false);
                                    increment = false;
                                    if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserAccessList))
                                    {
                                        #region Investissement,PDM,EVOL pour annonceur de r�f�rence ou concurrent par Niveau cat�gorie
                                        //Pour les annonceurs de r�f�rence ou concurrents s�lectionn�s par cat�gorie : Investissment,Evolution,PDM
                                        if (dtAdvertiser != null && dtAdvertiser.Rows.Count > 0 && dtAdvertiser.Columns.Contains("id_vehicle") && dtAdvertiser.Columns.Contains("id_category") && dtAdvertiser.Columns.Contains("id_advertiser"))
                                        {
                                            //Pour chaque ligne  annonceur de r�f�rence ou concurrent on r�cup�re les donn�es
                                            if (dtTotalUniverse.Columns.Contains("id_media"))
                                            {
                                                //si le niveau support est le plus bas alors vider la collection des annonceurs anciennement trait�s
                                                if (start == 1 && ((int.Parse(idCategory) != int.Parse(OldIdCategory)) || (int.Parse(idMedia) != int.Parse(OldIdMedia))))
                                                {
                                                    inAdvertCategoryAlreadyUsedArr = null;
                                                    inAdvertCategoryAlreadyUsedArr = new ArrayList();

                                                }
                                            }
                                            else
                                            {
                                                //si le niveau cat�gorie est le plus bas alors vider la collection des cat�gories anciennement trait�s
                                                if (start == 1 && ((int.Parse(idVehicle) != int.Parse(OldIdVehicle)) || (int.Parse(idCategory) != int.Parse(OldIdCategory))))
                                                {
                                                    inAdvertCategoryAlreadyUsedArr = null;
                                                    inAdvertCategoryAlreadyUsedArr = new ArrayList();
                                                }
                                            }
                                            //si on change de cat�gorie ou support la liste des annonceurs d�j� trait�s et leur investissment est vid�e.
                                            if (start == 1 && ((int.Parse(idCategory) != int.Parse(OldIdCategory)) || (int.Parse(idMedia) != int.Parse(OldIdMedia))))
                                            {
                                                hPdmCatAdvert = null;
                                                hPdmCatAdvert = new Hashtable();
                                            }
                                            if ((int.Parse(idCategory) != int.Parse(OldIdCategory)) || (int.Parse(idVehicle) != int.Parse(OldIdVehicle)))
                                            {
                                                //Pour les annonceurs de r�f�rence ou concurrents s�lectionn�s par cat�gorie : Investissment,Evolution,PDM

                                                foreach (DataRow currentAdvertRow in dtAdvertiser.Rows)
                                                {
                                                    #region ligne annonceurs de r�f�rences ou concurrents par  cat�gorie
                                                    idAdvertiser = currentAdvertRow["id_advertiser"].ToString();
                                                    if (dtAdvertiser.Columns.Contains("total_N"))
                                                    {
                                                        if (!inAdvertCategoryAlreadyUsedArr.Contains(idAdvertiser))
                                                        {
                                                            #region  support est le niveau le plus bas
                                                            if (dtTotalUniverse.Columns.Contains("id_media"))
                                                            {
                                                                strExpr = "id_vehicle = " + idVehicle + " AND id_category = " + idCategory + " AND id_advertiser=" + idAdvertiser;
                                                                strSort = "id_media ASC";
                                                                tab = FillAdvertisers(tab, dtAdvertiser, dtTotalMarketOrSector, strExpr, strSort, ref hPdmVehAdvert, ref hPdmCatAdvert, ref indexTabRow, idVehicle, Vehicle, idCategory, Category, idMedia, Media, idAdvertiser, CstPreformatedDetail.PreformatedMediaDetails.vehicleCategory);
                                                            }
                                                            #endregion
                                                            if (!inAdvertCategoryAlreadyUsedArr.Contains(idAdvertiser)) inAdvertCategoryAlreadyUsedArr.Add(idAdvertiser);//annonceurs trait�s doivent �tre distincts au niveau cat�gorie
                                                        }
                                                        if (!inAdvertCategoryAlreadyUsedArr.Contains(idCategory) && !dtTotalUniverse.Columns.Contains("id_media"))
                                                        {
                                                            #region  cat�gorie est le niveau le plus bas
                                                            strExpr = "id_vehicle = " + idVehicle + " AND id_category = " + idCategory;
                                                            strSort = "id_category ASC";
                                                            foundRows = dtAdvertiser.Select(strExpr, strSort, DataViewRowState.OriginalRows);
                                                            tab = FillAdvertisers(tab, dtTotalMarketOrSector, foundRows, ref AdvertiserInvestByCat, ref AdvertiserInvestByCat_N1, ref hPdmVehAdvert, ref hPdmCatAdvert, ref indexTabRow, idVehicle, Vehicle, idCategory, Category, "", "", CstPreformatedDetail.PreformatedMediaDetails.vehicleCategory, false);
                                                            #endregion
                                                            if (!inAdvertCategoryAlreadyUsedArr.Contains(idCategory)) inAdvertCategoryAlreadyUsedArr.Add(idCategory);//cat�gories trait�s doivent �tre distincts 
                                                        }
                                                    }
                                                    #endregion
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    if (!inTotUnivCategoryAlreadyUsedArr.Contains(idCategory)) inTotUnivCategoryAlreadyUsedArr.Add(idCategory);

                                }
                            }

                            #endregion
                        }
                        #endregion

                        #region lignes univers et march� ou famille par supports (media)
                        /*Si utilisateur a s�lectionn� de(s) support(s) on calcule investissement total march� ou famille et univers, et
					 * les param�tres associ�s (PDM,evolution,1er annonceurs et r�f�rences)
					 */
                        if (TreatMedia())
                        {
                            #region ligne total march� ou famille par support (media)
                            if (dtTotalMarketOrSector.Columns.Contains("id_media") && dtTotalMarketOrSector.Columns.Contains("media"))
                            {
                                idMedia = currentRow["id_media"].ToString();
                                Media = currentRow["media"].ToString();
                                //On vide la liste des supports anciennement trait�s lorsqu'on change de cat�gorie
                                if (start == 1 && (int.Parse(idCategory) != int.Parse(OldIdCategory)) && inTotMarketOrSectorMediaAlreadyUsedArr != null && inTotMarketOrSectorMediaAlreadyUsedArr.Count > 0)
                                {
                                    inTotMarketOrSectorMediaAlreadyUsedArr = null;
                                    inTotMarketOrSectorMediaAlreadyUsedArr = new ArrayList();
                                }
                                if (!inTotMarketOrSectorMediaAlreadyUsedArr.Contains(idMedia))
                                {
                                    if (dtTotalMarketOrSector.Columns.Contains("total_N"))
                                    {
                                        //investissement total univers par support (media)									
                                        ComputeInvestPdmEvol(dtTotalMarketOrSector, ref TotalMarketOrSectorCategoryInvest, ref TotalMarketOrSectorMediaInvest, ref TotalMarketOrSectorMediaInvest_N1, ref tempPDM, ref tempEvol, "Sum(total_N)", "Sum(total_N1)", "id_vehicle=" + idVehicle + " AND id_category=" + idCategory + " AND id_media = " + idMedia, _session.ComparativeStudy);
                                        // Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et support 
                                        if (FctUtilities.CheckedText.IsNotEmpty(TotalMarketOrSectorMediaInvest.ToString().Trim()))
                                        {
                                            tab = FillTabInvestPdmEvol(tab, indexTabRow, "", "", "", "", idMedia, Media, "", "", false, TotalMarketOrSectorMediaInvest, tempPDM, tempEvol, _session.ComparaisonCriterion, CstResult.MediaStrategy.InvestmentType.total, CstPreformatedDetail.PreformatedMediaDetails.vehicleCategoryMedia);
                                            increment = true;
                                        }
                                    }
                                    tab = FillTabFisrtElmt(tab, ref TotalMarketOrSectorMediaInvest, dtMarketOrSector1stAdvertiserByMed, dtMarketOrSector1stProductByMed, "id_media", idMedia, ref inTotMarketOrSectorMediaAlreadyUsedArr, ref indexTabRow, increment, false);
                                    increment = false;
                                }
                            }

                            #endregion

                            #region ligne total univers par support (media)
                            if (dtTotalUniverse != null && dtTotalUniverse.Columns.Contains("id_media") && dtTotalUniverse.Columns.Contains("media"))
                            {
                                idMedia = currentRow["id_media"].ToString();
                                Media = currentRow["media"].ToString();
                                //On vide la liste des supports anciennement trat�s d�s qu'on change de cat�gorie
                                if (start == 1 && (int.Parse(idCategory) != int.Parse(OldIdCategory)) && inTotUnivMediaAlreadyUsedArr != null && inTotUnivMediaAlreadyUsedArr.Count > 0)
                                {
                                    inTotUnivMediaAlreadyUsedArr = null;
                                    inTotUnivMediaAlreadyUsedArr = new ArrayList();
                                }
                                if (!inTotUnivMediaAlreadyUsedArr.Contains(idMedia))
                                {
                                    if (dtTotalUniverse.Columns.Contains("total_N"))
                                    {
                                        //investissement total univers support (media)										
                                        ComputeInvestPdmEvol(dtTotalUniverse, ref TotalUnivCategoryInvest, ref TotalUnivMediaInvest, ref TotalUnivMediaInvest_N1, ref tempPDM, ref tempEvol, "Sum(total_N)", "Sum(total_N1)", "id_vehicle=" + idVehicle + " AND id_category=" + idCategory + " AND id_media =" + idMedia, _session.ComparativeStudy);
                                        // Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et support 
                                        if (FctUtilities.CheckedText.IsNotEmpty(TotalUnivMediaInvest.ToString().Trim()))
                                        {
                                            tab = FillTabInvestPdmEvol(tab, indexTabRow, "", "", "", "", idMedia, Media, "", "", false, TotalUnivMediaInvest, tempPDM, tempEvol, CstComparaisonCriterion.universTotal, CstResult.MediaStrategy.InvestmentType.total, CstPreformatedDetail.PreformatedMediaDetails.vehicleCategoryMedia);
                                            increment = true;
                                        }
                                    }
                                    tab = FillTabFisrtElmt(tab, ref TotalUnivMediaInvest, dtUniverse1stAdvertiserByMed, dtUniverse1stProductByMed, "id_media", idMedia, ref inTotUnivMediaAlreadyUsedArr, ref indexTabRow, increment, false);
                                    increment = false;
                                    if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserAccessList))
                                    {
                                        #region Investissement,PDM,EVOL pour annonceur de r�f�rence ou concurrent par Niveau support (media)
                                        if (dtAdvertiser != null && dtAdvertiser.Rows.Count > 0 && dtAdvertiser.Columns.Contains("id_vehicle") && dtAdvertiser.Columns.Contains("id_category") && dtAdvertiser.Columns.Contains("id_media") && dtAdvertiser.Columns.Contains("id_advertiser"))
                                        {
                                            //On vide la liste des supports anciennement trat�s d�s qu'on change de cat�gorie
                                            if (start == 1 && (int.Parse(idCategory) != int.Parse(OldIdCategory)))
                                            {
                                                inAdvertMediaAlreadyUsedArr = null;
                                                inAdvertMediaAlreadyUsedArr = new ArrayList();
                                            }
                                            foreach (DataRow currentAdvertRow in dtAdvertiser.Rows)
                                            {

                                                #region ligne annonceurs de r�f�rences ou concurrents par  supports
                                                //Pour chaque annonceur distinct concurrent ou r�f�rence																						
                                                idAdvertiser = currentAdvertRow["id_advertiser"].ToString();
                                                if (FctUtilities.CheckedText.IsNotEmpty(idMedia) && !inAdvertMediaAlreadyUsedArr.Contains(idMedia))
                                                {
                                                    if (dtAdvertiser.Columns.Contains("total_N"))
                                                    {
                                                        strExpr = "id_vehicle = " + idVehicle + " AND id_category = " + idCategory + " AND id_media=" + idMedia;
                                                        strSort = "id_media ASC";
                                                        foundRows = dtAdvertiser.Select(strExpr, strSort, DataViewRowState.OriginalRows);
                                                        tab = FillAdvertisers(tab, dtTotalMarketOrSector, foundRows, ref AdvertiserInvestByCat, ref AdvertiserInvestByCat_N1, ref hPdmCatAdvert, ref hPdmCatAdvert, ref indexTabRow, idVehicle, Vehicle, idCategory, Category, idMedia, Media, CstPreformatedDetail.PreformatedMediaDetails.vehicleCategoryMedia, false);
                                                        if (!inAdvertMediaAlreadyUsedArr.Contains(idMedia)) inAdvertMediaAlreadyUsedArr.Add(idMedia);//Traitement unique de support											
                                                    }
                                                }

                                            }
                                        }
                                                #endregion

                                        #endregion
                                    }
                                    if (!inTotUnivMediaAlreadyUsedArr.Contains(idMedia)) inTotUnivMediaAlreadyUsedArr.Add(idMedia);

                                }
                            }
                            #endregion
                        }
                        #endregion

                        OldIdVehicle = idVehicle;
                        OldIdCategory = idCategory;
                        OldIdMedia = idMedia;
                        start = 1;
                    }
                }
                #endregion

                #endregion

            }
            catch (Exception ex)
            {
                throw new ProductClassIndicatorsException("GetFormattedTable(WebSession webSession,TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion comparisonCriterion )::Impossible de traiter les donn�es pour la strat�gir m�dia.", ex);
            }

            return tab;
        }
        #endregion

        #region GetChartData
        /// <summary>
        /// La r�partition m�dia sur le total de la p�riode dans un tableau
        /// contenant les �l�ments ci-apr�s :
        /// en ligne :
        /// -le total famille (en option uniquement en s�lection de groupe/vari�t�s) ou le
        /// total march� (en option)
        /// -les �l�ments de r�f�rences
        /// -les �l�ments concurrents 
        /// en colonne :
        /// -Les investissements de la p�riode N		
        /// Sur la dimension support le tableau est d�clin� de la fa�on suivante :
        /// -si plusieurs media, le tableau sera d�clin� par media
        /// -si un seul media, le tableau sera d�clin� par media, cat�gorie et supports
        /// </summary>
        /// <remarks>Cette m�thode est utilis�e pour la pr�sentation graphique des r�sultats.</remarks>
        /// <returns>tableau de r�sultats</returns>
        public object[,] GetChartData()
        {

            #region variables
            CstComparaisonCriterion comparisonCriterion = _session.ComparaisonCriterion;

            #region variables tempon
            string tempEvol = "";
            string tempPDM = "";
            #endregion

            #region variables pour les dimensions du tableau de r�sultats
            // tableau de resultats
            object[,] tab = null;
            //Index d'une ligne du tableau
            int indexTabRow = 0;
            //bool�en incr�mentation d'une ligne
            bool increment = false;
            #endregion

            #region Variables annonceurs de r�f�rences ou concurrents
            //identifiant  annonceurs de r�f�rences ou concurrents
            string idAdvertiser = "";
            //Y a t'il des annonceurs de r�f�rences ou concurrents
            bool hasAdvertiser = false;
            //Investissement d'un annonceur sur la p�riode N
            string AdvertiserInvest = "";
            //Investissement total des annonceurs sur la p�riode N
            string AdvertiserTotalInvest = "";
            //Investissement d'un annonceur par cat�gorie sur la p�riode N
            string AdvertiserInvestByCat = "";
            //investissement pour une cat�gorie sur p�riode N
            double CatInvest = (double)0.0;
            //investissement pour une cat�gorie sur p�riode N-1
            double CatInvest_N1 = (double)0.0;
            //Investissement d'un annonceur par m�dia (vehicle) sur la p�riode N
            string AdvertiserInvestByVeh = "";
            //collection des annonceurs d�j� trait�s
            ArrayList OldIdAdvertiserArr = new ArrayList();
            //collection des annonceurs d�j� trait�s pour le total univers
            ArrayList inTotUnivAdvertAlreadyUsedArr = new ArrayList();
            //collection des annonceurs d�j� trait�s pour un m�dia (vehicle)
            ArrayList inAdvertVehicleAlreadyUsedArr = new ArrayList();
            //collection des annonceurs d�j� trait�s pour une cxat�gorie
            ArrayList inAdvertCategoryAlreadyUsedArr = new ArrayList();
            //collection des annonceurs d�j� trait�s pour un support (media)
            ArrayList inAdvertMediaAlreadyUsedArr = new ArrayList();
            #endregion

            #region Variables des supports
            //depart de la boucle 
            int start = 0;
            //identifiant d'un m�dia (vehicle)
            string idVehicle = "";
            //identifiant m�dia pr�c�dent
            string OldIdVehicle = "0";
            //lib�ll� d'un m�dia
            string Vehicle = "";
            //identifiant d'une cat�gorie
            string idCategory = "";
            //identifiant pr�c�dent d'une cat�gorie
            string OldIdCategory = "0";
            //collection identifiant pr�c�dentes cat�gories
            ArrayList OldIdCategoryArr = new ArrayList();
            //lib�ll� d'une cat�gorie
            string Category = "";
            //identifiant d'un support
            string idMedia = "";
            //lib�ll� d'un support
            string Media = "";
            //identifiant du pr�c�dent support
            string OldIdMedia = "0";
            //collection identifiant pr�c�dents supports
            ArrayList OldIdMediaArr = new ArrayList();
            #endregion

            #region construction des listes de produits, media, annonceurs s�lectionn�s
            RecapUniversSelection recapUniversSelection = new RecapUniversSelection(_session);
            string AdvertiserAccessList = recapUniversSelection.AdvertiserAccessList;
            string CompetitorAdvertiserAccessList = recapUniversSelection.CompetitorAdvertiserAccessList;
            string CategoryAccessList = recapUniversSelection.CategoryAccessList;
            string MediaAccessList = recapUniversSelection.MediaAccessList;
            string VehicleAccessList = recapUniversSelection.VehicleAccessList;
            #endregion

            #region variable pour total univers
            //Y a t'il des donn�es pour calculer les valeurs du total univers
            bool hasTotalUniv = false;
            //Collecton des m�dia d�j� trait�
            ArrayList inTotUnivVehicleAlreadyUsedArr = new ArrayList();
            //Collecton des cat�gories d�j� trait�
            ArrayList inTotUnivCategoryAlreadyUsedArr = new ArrayList();
            //Collecton des supports d�j� trait�
            ArrayList inTotUnivMediaAlreadyUsedArr = new ArrayList();
            //investissment total univers par support p�riode N
            string TotalUnivMediaInvest = "";
            //investissment total univers par m�dia p�riode N
            string TotalUnivVehicleInvest = "";
            //investissment total univers par cat�gorie p�riode N
            string TotalUnivCategoryInvest = "";
            //investissment total univers p�riode N
            string TotalUnivInvest = "";
            //groupe de donn�es pour univers
            DataSet dsTotalUniverse = null;
            //table de donn�es pour univers
            DataTable dtTotalUniverse = null;
            #endregion

            #region variable pour total march� ou famille
            // y a t'il des donn�es pour le total march�
            bool hasTotalMarketOrSector = false;
            //investissemnt total march� par support p�riode N
            string TotalMarketOrSectorMediaInvest = "";
            //investissemnt total cat�gorie par cat�gorie p�riode N
            string TotalMarketOrSectorCategoryInvest = "";
            //investissemnt total march� par m�dia p�riode N
            string TotalMarketOrSectorVehicleInvest = "";
            //investissemnt total march�  p�riode N
            string TotalMarketOrSectorInvest = "";
            //identifiant des m�dias d�j� trait�s
            ArrayList inTotMarketOrSectorVehicleAlreadyUsedArr = new ArrayList();
            //identifiant des cat�gories d�j� trait�s
            ArrayList inTotMarketOrSectorCategoryAlreadyUsedArr = new ArrayList();
            //identifiant des supports d�j� trait�s
            ArrayList inTotMarketOrSectorMediaAlreadyUsedArr = new ArrayList();
            //Groupe de donn�es pour total march�
            DataSet dsTotalMarketOrSector = null;
            //table de donn�es pour total march�
            DataTable dtTotalMarketOrSector = null;
            #endregion

            #region variable pour annonceur de r�f�rences ou concurrent
            //chaine pour colonne � r�cuperer dans collection de DataRow annonceurs
            string strExpr = "";
            //filtre pour colonne � r�cuperer dans collection de DataRow annonceurs
            string strSort = "";
            //DataRow annonceurs
            DataRow[] foundRows = null;
            //Groupe de donn�es annonceurs de r�f�rence ou concurrents
            DataSet dsAdvertiser = null;
            //Table de donn�es annonceurs de r�f�rences ou concurrents
            DataTable dtAdvertiser = null;
            #endregion

            #endregion

            #region Chargement des donn�es

            #region chargement des donn�es pour les annonceurs de r�f�rences et/ou concurrents
            /* Chargement des donn�es pour les annonceurs de r�f�rences et,ou concurrents
			 * A partir de ces donn�es on peut calculer l'investissement,l'evolution,le PDM,
			 * pour chaque annonceur et par niveau support
			 */
            if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserAccessList))
            {
                dsAdvertiser = _dalLayer.GetMediaStrategyTblData(CstResult.MotherRecap.ElementType.advertiser, CstComparaisonCriterion.universTotal, true);
                if (dsAdvertiser != null && dsAdvertiser.Tables[0] != null && dsAdvertiser.Tables[0].Rows.Count > 0)
                {
                    dtAdvertiser = dsAdvertiser.Tables[0];
                }
            }
            #endregion

            #region chargement des donn�es totaux univers
            /*Chargement des donn�es pour les totaux univers
				* dsTotalUniverse : permet de r�cuperer pour chaque niveau support
				* l'investissement,l'evolution,le PDM
				*/
            dsTotalUniverse = _dalLayer.GetMediaStrategyTblData(CstResult.MotherRecap.ElementType.advertiser, CstComparaisonCriterion.universTotal, false);
            if (dsTotalUniverse != null && dsTotalUniverse.Tables[0] != null && dsTotalUniverse.Tables[0].Rows.Count > 0)
            {
                dtTotalUniverse = dsTotalUniverse.Tables[0];
            }
            #endregion

            #region chargement des donn�es totaux march�s ou famille(s)
            /* Chargement des donn�es pour les annonceurs de r�f�rences et,ou concurrents
			 * A partir de ces donn�es on peut calculer l'investissement,l'evolution,le PDM,
			 * pour chaque annonceur et par niveau support
			 */
            if (comparisonCriterion == CstComparaisonCriterion.sectorTotal)
                dsTotalMarketOrSector = _dalLayer.GetMediaStrategyTblData(CstResult.MotherRecap.ElementType.advertiser, CstComparaisonCriterion.sectorTotal, false);
            else if (comparisonCriterion == CstComparaisonCriterion.marketTotal)
                dsTotalMarketOrSector = _dalLayer.GetMediaStrategyTblData(CstResult.MotherRecap.ElementType.advertiser, CstComparaisonCriterion.marketTotal, false);
            if (dsTotalMarketOrSector != null && dsTotalMarketOrSector.Tables[0] != null && dsTotalMarketOrSector.Tables[0].Rows.Count > 0)
            {
                dtTotalMarketOrSector = dsTotalMarketOrSector.Tables[0];
            }
            #endregion

            #endregion

            #region  construction du tableau de r�sultats

            #region instanciation du tableau de r�sultats
            /*On instancie le tableau de r�sultats pour strat�gie m�dia
			 */
            //cr�ation du tableau 			
            tab = TabInstance(dtTotalMarketOrSector, dtAdvertiser, VehicleAccessList, NB_CHART_MAX_COLUMNS);

            // Il n'y a pas de donn�es
            if (tab == null) return (new object[0, 0]);
            #endregion

            #region Remplissage chaque ligne m�dia dans table
            /* Chaque ligne du tableau contient toutes les donn�es d'un annonceur de r�f�rence ou concurrent
			 * (Investissement,Evolution,PDM,lib�ll�) ou d'un total support (media ou cat�gorie ou support)
			 * avec ses param�tres (Investissement,Evolution,PDM,lib�ll�, 1er annonceur et son investissement, idem pour 1er r�f�rence)
			*/
            if (dtTotalMarketOrSector != null && dtTotalMarketOrSector.Rows.Count > 0)
            {
                /*Si utilisateur a s�lectionn� PLURIMEDIA on calcule investissement total march� ou famille et univers, et
                 * les param�tres associ�s (PDM,evolution,1er annonceurs et r�f�rences)
                 */
                if (FctUtilities.CheckedText.IsNotEmpty(VehicleAccessList) && CstDbClassif.Vehicles.names.plurimedia == (CstDbClassif.Vehicles.names)int.Parse(VehicleAccessList))
                {
                    #region ligne total march� ou famille pour PLURIMEDIA
                    //Traitement ligne total march� ou famille pour Plurimedia : Investissment,Evolution,PDM,1er nnonceur et son investissment, premi�re r�f�rence et son investissment
                    if (dtTotalMarketOrSector.Columns.Contains("total_N") && !hasTotalMarketOrSector)
                    {
                        //investissement total 										
                        TotalMarketOrSectorInvest = dtTotalMarketOrSector.Compute("Sum(total_N)", "").ToString();
                        if (FctUtilities.CheckedText.IsNotEmpty(TotalMarketOrSectorInvest.Trim()))
                            //							TotalMarketOrSectorInvest = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(TotalMarketOrSectorInvest)/(double)1000));																																									
                            // Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� et plurim�dia 
                            if (FctUtilities.CheckedText.IsNotEmpty(TotalMarketOrSectorInvest.Trim()))
                            {
                                tab = FillTabInvestPdmEvol(tab, indexTabRow, VehicleAccessList, GestionWeb.GetWebWord(210, _session.SiteLanguage), "", "", "", "", "", "", true, TotalMarketOrSectorInvest, "", tempEvol, _session.ComparaisonCriterion, CstResult.MediaStrategy.InvestmentType.total, CstPreformatedDetail.PreformatedMediaDetails.vehicle);
                                increment = true;
                            }
                        if (increment) indexTabRow++; //ligne suivante
                        hasTotalMarketOrSector = true;
                        increment = false;
                    }
                    #endregion

                    #region ligne univers pour PLURIMEDIA
                    //Traitement ligne total univers pour Plurimedia : Investissment,Evolution,PDM,1er nnonceur et son investissment, premi�re r�f�rence et son investissment
                    if (dtTotalUniverse.Columns.Contains("total_N") && !hasTotalUniv)
                    {
                        //investissement total 	univers plurim�dia					
                        if (dtTotalUniverse.Columns.Contains("total_N"))
                            TotalUnivInvest = dtTotalUniverse.Compute("Sum(total_N)", "").ToString();
                        if (FctUtilities.CheckedText.IsNotEmpty(TotalUnivInvest.Trim()))
                            //							TotalUnivInvest = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(TotalUnivInvest)/(double)1000));																						
                            // Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et plurim�dia 
                            if (FctUtilities.CheckedText.IsNotEmpty(TotalUnivInvest.Trim()))
                            {
                                tab = FillTabInvestPdmEvol(tab, indexTabRow, VehicleAccessList, GestionWeb.GetWebWord(210, _session.SiteLanguage), "", "", "", "", "", "", true, TotalUnivInvest, "", tempEvol, CstComparaisonCriterion.universTotal, CstResult.MediaStrategy.InvestmentType.total, CstPreformatedDetail.PreformatedMediaDetails.vehicle);
                                increment = true;
                            }
                        if (increment) indexTabRow++;
                        increment = false;
                        //ligne suivante
                        hasTotalUniv = true;
                    }
                    #endregion

                    #region lignes  annonceurs de r�f�rences ou concurrents	pour s�lection PLURIMEDIA
                    //Pour les annonceurs de r�f�rence ou concurrents s�lectionn�s par PLURIMEDIA : Investissment,Evolution,PDM
                    if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserAccessList) && dtAdvertiser != null && dtAdvertiser.Rows.Count > 0 && !hasAdvertiser)
                    {
                        //Pour chaque ligne  TOTAL annonceur de r�f�rence ou concurrent on r�cup�re les donn�es
                        foreach (DataRow currentAdvertRow in dtAdvertiser.Rows)
                        {
                            if (dtAdvertiser.Columns.Contains("id_advertiser") && dtAdvertiser.Columns.Contains("total_N"))
                            {
                                idAdvertiser = currentAdvertRow["id_advertiser"].ToString();
                                if (!inTotUnivAdvertAlreadyUsedArr.Contains(idAdvertiser))
                                {
                                    //investissement TOTAL annonceur de r�f�rence ou concurrent 
                                    AdvertiserTotalInvest = dtAdvertiser.Compute("Sum(total_N)", "id_advertiser = " + idAdvertiser + "").ToString();
                                    if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserTotalInvest.Trim()))
                                    {
                                        //										AdvertiserTotalInvest = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(AdvertiserTotalInvest)/(double)1000));																														
                                        //Insertion des donn�es dans la ligne courante pour un annonceur
                                        if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserTotalInvest.Trim()))
                                            tab = FillTabInvestPdmEvol(tab, indexTabRow, "", "", "", "", "", "", currentAdvertRow["id_advertiser"].ToString(), currentAdvertRow["advertiser"].ToString(), true, AdvertiserTotalInvest, "", tempEvol, CstComparaisonCriterion.universTotal, CstResult.MediaStrategy.InvestmentType.advertiser, CstPreformatedDetail.PreformatedMediaDetails.vehicle);
                                        indexTabRow++;
                                        if (!inTotUnivAdvertAlreadyUsedArr.Contains(idAdvertiser)) inTotUnivAdvertAlreadyUsedArr.Add(idAdvertiser);
                                    }
                                }
                            }
                            hasAdvertiser = true;
                        }
                    }
                    #endregion
                }

                //Pour chaque ligne  media du total univers on r�cup�re les donn�es				
                foreach (DataRow currentRow in dtTotalMarketOrSector.Rows)
                {

                    #region lignes univers et march� ou famille par m�dia (vehicle)
                    /*Si utilisateur a s�lectionn� un MEDIA (vehicle) on calcule investissement total march� ou famille et univers, et
					* les param�tres associ�s (PDM,evolution,1er annonceurs et r�f�rences)
					*/
                    #region ligne total march� ou famille par m�dia (vehicle)
                    //colonne m�dia (vehicle)
                    if (dtTotalMarketOrSector.Columns.Contains("id_vehicle") && dtTotalMarketOrSector.Columns.Contains("vehicle"))
                    {
                        idVehicle = currentRow["id_vehicle"].ToString();
                        Vehicle = currentRow["vehicle"].ToString();
                        if (!inTotMarketOrSectorVehicleAlreadyUsedArr.Contains(idVehicle))
                        {
                            if (dtTotalMarketOrSector.Columns.Contains("total_N"))
                            {
                                //investissement total univers pour m�dia (vehicle)
                                TotalMarketOrSectorVehicleInvest = dtTotalMarketOrSector.Compute("Sum(total_N)", "id_vehicle = " + idVehicle + "").ToString();
                                if (FctUtilities.CheckedText.IsNotEmpty(TotalMarketOrSectorVehicleInvest.ToString().Trim()))
                                {
                                    //									TotalMarketOrSectorVehicleInvest=String.Format("{0:### ### ### ### ##0.##}",(double.Parse(TotalMarketOrSectorVehicleInvest)/(double)1000));																																
                                    // Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et media (vehicle) 								
                                    tab = FillTabInvestPdmEvol(tab, indexTabRow, idVehicle, Vehicle, "", "", "", "", "", "", false, TotalMarketOrSectorVehicleInvest, tempPDM, tempEvol, _session.ComparaisonCriterion, CstResult.MediaStrategy.InvestmentType.total, CstPreformatedDetail.PreformatedMediaDetails.vehicle);
                                    increment = true;
                                }
                            }
                            if (!inTotMarketOrSectorVehicleAlreadyUsedArr.Contains(idVehicle)) inTotMarketOrSectorVehicleAlreadyUsedArr.Add(idVehicle);
                            if (increment) indexTabRow++; //ligne suivante
                            increment = false;
                        }
                    }
                    #endregion

                    #region ligne total univers par m�dia (vehicle)
                    if (dtTotalUniverse != null && dtTotalUniverse.Columns.Contains("id_vehicle") && dtTotalUniverse.Columns.Contains("vehicle"))
                    {
                        idVehicle = currentRow["id_vehicle"].ToString();
                        Vehicle = currentRow["vehicle"].ToString();
                        if (!inTotUnivVehicleAlreadyUsedArr.Contains(idVehicle))
                        {
                            if (dtTotalUniverse.Columns.Contains("total_N"))
                            {
                                //investissement total univers pour m�dia (vehicle)
                                TotalUnivVehicleInvest = dtTotalUniverse.Compute("Sum(total_N)", "id_vehicle = " + idVehicle + "").ToString();
                                if (FctUtilities.CheckedText.IsNotEmpty(TotalUnivVehicleInvest.ToString().Trim()))
                                {
                                    //									TotalUnivVehicleInvest =  String.Format("{0:### ### ### ### ##0.##}",(double.Parse(TotalUnivVehicleInvest)/(double)1000));																									
                                    // Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et media (vehicle) 								
                                    tab = FillTabInvestPdmEvol(tab, indexTabRow, idVehicle, Vehicle, "", "", "", "", "", "", false, TotalUnivVehicleInvest, tempPDM, tempEvol, CstComparaisonCriterion.universTotal, CstResult.MediaStrategy.InvestmentType.total, CstPreformatedDetail.PreformatedMediaDetails.vehicle);
                                    increment = true;
                                }
                            }
                            if (!inTotUnivVehicleAlreadyUsedArr.Contains(idVehicle)) inTotUnivVehicleAlreadyUsedArr.Add(idVehicle);
                            if (increment) indexTabRow++; //ligne suivante
                            increment = false;

                    #endregion

                            if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserAccessList))
                            {
                                #region Investissement pour annonceur de r�f�rence ou concurrent Niveau m�dia (vehicle)
                                //Pour les annonceurs de r�f�rence ou concurrents s�lectionn�s par MEDIA (vehicle) : Investissment
                                if (dtAdvertiser != null && dtAdvertiser.Rows.Count > 0 && dtAdvertiser.Columns.Contains("id_vehicle") && dtAdvertiser.Columns.Contains("id_advertiser"))
                                {
                                    //Pour chaque ligne  annonceur de r�f�rence ou concurrent on r�cup�re les donn�es
                                    if ((int.Parse(idVehicle) != int.Parse(OldIdVehicle)))
                                    {
                                        inAdvertVehicleAlreadyUsedArr = null;
                                        inAdvertVehicleAlreadyUsedArr = new ArrayList();
                                        foreach (DataRow currentAdvertRow in dtAdvertiser.Rows)
                                        {
                                            //Pour les annonceurs de r�f�rence ou concurrents s�lectionn�s par media (vehicle) : Investissment,Evolution,PDM
                                            #region ligne annonceurs de r�f�rences ou concurrents par m�dia (vehicle)
                                            //colonne m�dia (vehicle)																						
                                            idAdvertiser = currentAdvertRow["id_advertiser"].ToString();
                                            if (!inAdvertVehicleAlreadyUsedArr.Contains(idAdvertiser))
                                            {
                                                if (dtAdvertiser.Columns.Contains("total_N"))
                                                {
                                                    //investissement annonceur de r�f�rence ou concurrent  par m�dia (vehicle)
                                                    AdvertiserInvestByVeh = dtAdvertiser.Compute("Sum(total_N)", "id_advertiser = " + idAdvertiser + " AND id_vehicle=" + idVehicle).ToString();
                                                    //													if(FctUtilities.CheckedText.IsNotEmpty(AdvertiserInvestByVeh.Trim())){
                                                    //														AdvertiserInvestByVeh = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(AdvertiserInvestByVeh)/(double)1000));														
                                                    //													}													
                                                    //Insertion des donn�es dans la ligne courante pour un annonceur et par m�dia (vehicle)
                                                    if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserInvestByVeh.Trim()))
                                                        tab = FillTabInvestPdmEvol(tab, indexTabRow, idVehicle, Vehicle, "", "", "", "", currentAdvertRow["id_advertiser"].ToString(), currentAdvertRow["advertiser"].ToString(), false, AdvertiserInvestByVeh, tempPDM, tempEvol, CstComparaisonCriterion.universTotal, CstResult.MediaStrategy.InvestmentType.advertiser, CstPreformatedDetail.PreformatedMediaDetails.vehicle);
                                                    if (!inAdvertVehicleAlreadyUsedArr.Contains(idAdvertiser)) inAdvertVehicleAlreadyUsedArr.Add(idAdvertiser);
                                                    indexTabRow++;
                                                }

                                            }
                                        }
                                    }
                                }
                                            #endregion

                                #endregion
                            }
                        }
                    }
                    #endregion

                    #region lignes univers et march� ou famille par cat�gorie
                    /*Si utilisateur a s�lectionn� des cat�gories on calcule investissement total march� ou famille et univers, et
				 * les param�tres associ�s (PDM,evolution,1er annonceurs et r�f�rences)
				 */
                    #region ligne total march� ou famille par cat�gorie
                    if (dtTotalMarketOrSector.Columns.Contains("id_category") && dtTotalMarketOrSector.Columns.Contains("category"))
                    {
                        idCategory = currentRow["id_category"].ToString();
                        Category = currentRow["category"].ToString();
                        //On vide la liste des cat�gories anciennement trait�s d�s qu'on change de m�dia (vehicle)
                        if (start == 1 && (int.Parse(idVehicle) != int.Parse(OldIdVehicle)) && inTotMarketOrSectorCategoryAlreadyUsedArr != null && inTotMarketOrSectorCategoryAlreadyUsedArr.Count > 0)
                        {
                            inTotMarketOrSectorCategoryAlreadyUsedArr = null;
                            inTotMarketOrSectorCategoryAlreadyUsedArr = new ArrayList();
                        }
                        if (!inTotMarketOrSectorCategoryAlreadyUsedArr.Contains(idCategory))
                        {
                            if (dtTotalMarketOrSector.Columns.Contains("total_N"))
                            {
                                //investissement total march� ou famille par cat�gorie
                                TotalMarketOrSectorCategoryInvest = dtTotalMarketOrSector.Compute("Sum(total_N)", "id_vehicle=" + idVehicle + " AND id_category = " + idCategory).ToString();
                                if (FctUtilities.CheckedText.IsNotEmpty(TotalMarketOrSectorCategoryInvest.ToString().Trim()))
                                {
                                    //									TotalMarketOrSectorCategoryInvest = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(TotalMarketOrSectorCategoryInvest)/(double)1000));
                                    increment = true;
                                }
                                // Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et cat�gorie
                                if (FctUtilities.CheckedText.IsNotEmpty(TotalMarketOrSectorCategoryInvest.Trim()))
                                    tab = FillTabInvestPdmEvol(tab, indexTabRow, "", "", idCategory, Category, "", "", "", "", false, TotalMarketOrSectorCategoryInvest, tempPDM, tempEvol, _session.ComparaisonCriterion, CstResult.MediaStrategy.InvestmentType.total, CstPreformatedDetail.PreformatedMediaDetails.vehicleCategory);
                            }
                            if (!inTotMarketOrSectorCategoryAlreadyUsedArr.Contains(idCategory)) inTotMarketOrSectorCategoryAlreadyUsedArr.Add(idCategory);//Les cat�gories trait�es doivent �tre distincts
                            if (increment) indexTabRow++; //ligne suivante
                            increment = false;
                        }
                    }

                    #endregion

                    #region ligne total univers par cat�gorie
                    if (dtTotalUniverse != null && dtTotalUniverse.Columns.Contains("id_category") && dtTotalUniverse.Columns.Contains("category"))
                    {
                        idCategory = currentRow["id_category"].ToString();
                        Category = currentRow["category"].ToString();
                        //On vide la liste des cat�gories trait�s lorsqu'on change de m�dia (vehicle)
                        if (start == 1 && (int.Parse(idVehicle) != int.Parse(OldIdVehicle)) && inTotUnivCategoryAlreadyUsedArr != null && inTotUnivCategoryAlreadyUsedArr.Count > 0)
                        {
                            inTotUnivCategoryAlreadyUsedArr = null;
                            inTotUnivCategoryAlreadyUsedArr = new ArrayList();
                        }
                        //pour chaque cat�gorie distincte
                        if (!inTotUnivCategoryAlreadyUsedArr.Contains(idCategory))
                        {
                            if (dtTotalUniverse.Columns.Contains("total_N"))
                            {
                                //investissement total univers par cat�gorie
                                TotalUnivCategoryInvest = dtTotalUniverse.Compute("Sum(total_N)", "id_vehicle=" + idVehicle + " AND id_category = " + idCategory).ToString();
                                //								if(FctUtilities.CheckedText.IsNotEmpty(TotalUnivCategoryInvest.ToString().Trim()))
                                //									TotalUnivCategoryInvest = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(TotalUnivCategoryInvest)/(double)1000));																
                                // Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et cat�gorie 
                                if (FctUtilities.CheckedText.IsNotEmpty(TotalUnivCategoryInvest.Trim()))
                                {
                                    tab = FillTabInvestPdmEvol(tab, indexTabRow, "", "", idCategory, Category, "", "", "", "", false, TotalUnivCategoryInvest, tempPDM, tempEvol, CstComparaisonCriterion.universTotal, CstResult.MediaStrategy.InvestmentType.total, CstPreformatedDetail.PreformatedMediaDetails.vehicleCategory);
                                    increment = true;
                                }
                            }
                            if (increment) indexTabRow++;
                            increment = false;
                            if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserAccessList))
                            {
                                #region Investissement,PDM,EVOL pour annonceur de r�f�rence ou concurrent par Niveau cat�gorie
                                //Pour les annonceurs de r�f�rence ou concurrents s�lectionn�s par cat�gorie : Investissment,Evolution,PDM
                                if (dtAdvertiser != null && dtAdvertiser.Rows.Count > 0 && dtAdvertiser.Columns.Contains("id_vehicle") && dtAdvertiser.Columns.Contains("id_category") && dtAdvertiser.Columns.Contains("id_advertiser"))
                                {
                                    //Pour chaque ligne  annonceur de r�f�rence ou concurrent on r�cup�re les donn�es
                                    if (dtTotalUniverse.Columns.Contains("id_media"))
                                    {
                                        //si le niveau support est le plus bas alors vider la collection des annonceurs anciennement trait�s
                                        if (start == 1 && ((int.Parse(idCategory) != int.Parse(OldIdCategory)) || (int.Parse(idMedia) != int.Parse(OldIdMedia))))
                                        {
                                            inAdvertCategoryAlreadyUsedArr = null;
                                            inAdvertCategoryAlreadyUsedArr = new ArrayList();
                                        }
                                    }
                                    else
                                    {
                                        //si le niveau cat�gorie est le plus bas alors vider la collection des cat�gories anciennement trait�s
                                        if (start == 1 && ((int.Parse(idVehicle) != int.Parse(OldIdVehicle)) || (int.Parse(idCategory) != int.Parse(OldIdCategory))))
                                        {
                                            inAdvertCategoryAlreadyUsedArr = null;
                                            inAdvertCategoryAlreadyUsedArr = new ArrayList();
                                        }
                                    }
                                    if ((int.Parse(idCategory) != int.Parse(OldIdCategory)) || (int.Parse(idVehicle) != int.Parse(OldIdVehicle)))
                                    {
                                        //Pour les annonceurs de r�f�rence ou concurrents s�lectionn�s par cat�gorie : Investissment,Evolution,PDM

                                        foreach (DataRow currentAdvertRow in dtAdvertiser.Rows)
                                        {
                                            #region ligne annonceurs de r�f�rences ou concurrents par  cat�gorie
                                            idAdvertiser = currentAdvertRow["id_advertiser"].ToString();
                                            if (dtAdvertiser.Columns.Contains("total_N"))
                                            {
                                                if (!inAdvertCategoryAlreadyUsedArr.Contains(idAdvertiser))
                                                {
                                                    #region  support est le niveau le plus bas
                                                    if (dtTotalUniverse.Columns.Contains("id_media"))
                                                    {
                                                        strExpr = "id_vehicle = " + idVehicle + " AND id_category = " + idCategory + " AND id_advertiser=" + idAdvertiser;
                                                        strSort = "id_media ASC";
                                                        foundRows = dtAdvertiser.Select(strExpr, strSort, DataViewRowState.OriginalRows);
                                                        if (foundRows != null && foundRows.Length > 0)
                                                        {
                                                            CatInvest = (double)0.0;
                                                            CatInvest_N1 = (double)0.0;
                                                            //investissement annonceur de r�f�rence ou concurrent  par cat�gorie sur p�riode N
                                                            for (int n = 0; n < foundRows.Length; n++)
                                                            {
                                                                CatInvest += double.Parse(foundRows[n]["total_N"].ToString());
                                                                if (_session.ComparativeStudy) CatInvest_N1 += double.Parse(foundRows[n]["total_N1"].ToString());
                                                            }
                                                            AdvertiserInvestByCat = CatInvest.ToString();
                                                            //															if(FctUtilities.CheckedText.IsNotEmpty(AdvertiserInvestByCat.Trim())){
                                                            //																AdvertiserInvestByCat = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(AdvertiserInvestByCat)/(double)1000));																
                                                            //															}																														
                                                            //Insertion des donn�es dans la ligne courante pour un annonceur et par cat�gorie
                                                            if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserInvestByCat.Trim()))
                                                                tab = FillTabInvestPdmEvol(tab, indexTabRow, idVehicle, Vehicle, idCategory, Category, "", "", foundRows[0]["id_advertiser"].ToString(), foundRows[0]["advertiser"].ToString(), false, AdvertiserInvestByCat, tempPDM, tempEvol, CstComparaisonCriterion.universTotal, CstResult.MediaStrategy.InvestmentType.advertiser, CstPreformatedDetail.PreformatedMediaDetails.vehicleCategory);
                                                            indexTabRow++;
                                                        }
                                                    }
                                                    #endregion
                                                    if (!inAdvertCategoryAlreadyUsedArr.Contains(idAdvertiser)) inAdvertCategoryAlreadyUsedArr.Add(idAdvertiser);//annonceurs trait�s doivent �tre distincts au niveau cat�gorie
                                                }
                                                if (!inAdvertCategoryAlreadyUsedArr.Contains(idCategory) && !dtTotalUniverse.Columns.Contains("id_media"))
                                                {
                                                    #region  cat�gorie est le niveau le plus bas
                                                    strExpr = "id_vehicle = " + idVehicle + " AND id_category = " + idCategory;
                                                    strSort = "id_category ASC";
                                                    foundRows = dtAdvertiser.Select(strExpr, strSort, DataViewRowState.OriginalRows);
                                                    if (foundRows != null && foundRows.Length > 0)
                                                    {
                                                        for (int n = 0; n < foundRows.Length; n++)
                                                        {
                                                            //investissement annonceur de r�f�rence ou concurrent  par cat�gorie
                                                            //															if(FctUtilities.CheckedText.IsNotEmpty(foundRows[n]["total_N"].ToString()))
                                                            //																AdvertiserInvestByCat = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(foundRows[n]["total_N"].ToString())/(double)1000));																																													
                                                            //Insertion des donn�es dans la ligne courante pour un annonceur et par cat�gorie
                                                            if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserInvestByCat.Trim()))
                                                                tab = FillTabInvestPdmEvol(tab, indexTabRow, idVehicle, Vehicle, idCategory, Category, "", "", foundRows[n]["id_advertiser"].ToString(), foundRows[n]["advertiser"].ToString(), false, AdvertiserInvestByCat, tempPDM, tempEvol, _session.ComparaisonCriterion, CstResult.MediaStrategy.InvestmentType.advertiser, CstPreformatedDetail.PreformatedMediaDetails.vehicleCategory);
                                                            indexTabRow++;
                                                        }
                                                    }
                                                    #endregion
                                                    if (!inAdvertCategoryAlreadyUsedArr.Contains(idCategory)) inAdvertCategoryAlreadyUsedArr.Add(idCategory);//acat�gories trait�s doivent �tre distincts 
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                }
                                #endregion
                            }
                            if (!inTotUnivCategoryAlreadyUsedArr.Contains(idCategory)) inTotUnivCategoryAlreadyUsedArr.Add(idCategory);
                        }
                    }

                    #endregion

                    #endregion

                    #region lignes univers et march� ou famille par supports (media)
                    /*Si utilisateur a s�lectionn� de(s) support(s) on calcule investissement total march� ou famille et univers, et
				 * les param�tres associ�s (PDM,evolution,1er annonceurs et r�f�rences)
				 */
                    #region ligne total march� ou famille par support (media)
                    if (dtTotalMarketOrSector.Columns.Contains("id_media") && dtTotalMarketOrSector.Columns.Contains("media"))
                    {
                        idMedia = currentRow["id_media"].ToString();
                        Media = currentRow["media"].ToString();
                        //On vide la liste des supports anciennement trait�s lorsqu'on change de cat�gorie
                        if (start == 1 && (int.Parse(idCategory) != int.Parse(OldIdCategory)) && inTotMarketOrSectorMediaAlreadyUsedArr != null && inTotMarketOrSectorMediaAlreadyUsedArr.Count > 0)
                        {
                            inTotMarketOrSectorMediaAlreadyUsedArr = null;
                            inTotMarketOrSectorMediaAlreadyUsedArr = new ArrayList();
                        }
                        if (!inTotMarketOrSectorMediaAlreadyUsedArr.Contains(idMedia))
                        {
                            if (dtTotalMarketOrSector.Columns.Contains("total_N"))
                            {
                                //investissement total univers par support (media)								
                                TotalMarketOrSectorMediaInvest = dtTotalMarketOrSector.Compute("Sum(total_N)", "id_vehicle=" + idVehicle + " AND id_category=" + idCategory + " AND id_media = " + idMedia).ToString();
                                if (FctUtilities.CheckedText.IsNotEmpty(TotalMarketOrSectorMediaInvest.ToString().Trim()))
                                {
                                    //									TotalMarketOrSectorMediaInvest=String.Format("{0:### ### ### ### ##0.##}",(double.Parse(TotalMarketOrSectorMediaInvest)/(double)1000));
                                    increment = true;
                                }
                                // Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et support 
                                if (FctUtilities.CheckedText.IsNotEmpty(TotalMarketOrSectorMediaInvest.ToString().Trim()))
                                    tab = FillTabInvestPdmEvol(tab, indexTabRow, "", "", "", "", idMedia, Media, "", "", false, TotalMarketOrSectorMediaInvest, tempPDM, tempEvol, _session.ComparaisonCriterion, CstResult.MediaStrategy.InvestmentType.total, CstPreformatedDetail.PreformatedMediaDetails.vehicleCategoryMedia);
                            }
                            if (!inTotMarketOrSectorMediaAlreadyUsedArr.Contains(idMedia)) inTotMarketOrSectorMediaAlreadyUsedArr.Add(idMedia);//On traite des m�dia distincts
                            if (increment) indexTabRow++; //ligne suivante
                            increment = false;
                        }
                    }
                    #endregion

                    #region ligne total univers par support (media)
                    if (dtTotalUniverse != null && dtTotalUniverse.Columns.Contains("id_media") && dtTotalUniverse.Columns.Contains("media"))
                    {
                        idMedia = currentRow["id_media"].ToString();
                        Media = currentRow["media"].ToString();
                        //On vide la liste des supports anciennement trat�s d�s qu'on change de cat�gorie
                        if (start == 1 && (int.Parse(idCategory) != int.Parse(OldIdCategory)) && inTotUnivMediaAlreadyUsedArr != null && inTotUnivMediaAlreadyUsedArr.Count > 0)
                        {
                            inTotUnivMediaAlreadyUsedArr = null;
                            inTotUnivMediaAlreadyUsedArr = new ArrayList();
                        }
                        if (!inTotUnivMediaAlreadyUsedArr.Contains(idMedia))
                        {
                            if (dtTotalUniverse.Columns.Contains("total_N"))
                            {
                                //investissement total univers support (media)
                                TotalUnivMediaInvest = dtTotalUniverse.Compute("Sum(total_N)", "id_vehicle=" + idVehicle + " AND id_category=" + idCategory + " AND id_media =" + idMedia).ToString();
                                //								if(FctUtilities.CheckedText.IsNotEmpty(TotalUnivMediaInvest.ToString().Trim()))
                                //									TotalUnivMediaInvest=String.Format("{0:### ### ### ### ##0.##}",(double.Parse(TotalUnivMediaInvest)/(double)1000));																
                                // Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et support 
                                if (FctUtilities.CheckedText.IsNotEmpty(TotalUnivMediaInvest.ToString().Trim()))
                                {
                                    tab = FillTabInvestPdmEvol(tab, indexTabRow, "", "", "", "", idMedia, Media, "", "", false, TotalUnivMediaInvest, tempPDM, tempEvol, CstComparaisonCriterion.universTotal, CstResult.MediaStrategy.InvestmentType.total, CstPreformatedDetail.PreformatedMediaDetails.vehicleCategoryMedia);
                                    increment = true;
                                }
                            }
                            if (increment) indexTabRow++;
                            increment = false;

                            if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserAccessList))
                            {
                                #region Investissement,PDM,EVOL pour annonceur de r�f�rence ou concurrent par Niveau support (media)
                                if (dtAdvertiser != null && dtAdvertiser.Rows.Count > 0 && dtAdvertiser.Columns.Contains("id_vehicle") && dtAdvertiser.Columns.Contains("id_category") && dtAdvertiser.Columns.Contains("id_media") && dtAdvertiser.Columns.Contains("id_advertiser"))
                                {
                                    //On vide la liste des supports anciennement trat�s d�s qu'on change de cat�gorie
                                    if (start == 1 && (int.Parse(idCategory) != int.Parse(OldIdCategory)))
                                    {
                                        inAdvertMediaAlreadyUsedArr = null;
                                        inAdvertMediaAlreadyUsedArr = new ArrayList();
                                    }
                                    foreach (DataRow currentAdvertRow in dtAdvertiser.Rows)
                                    {
                                        #region ligne annonceurs de r�f�rences ou concurrents par  supports
                                        //Pour chaque annonceur distinct concurrent ou r�f�rence																						
                                        idAdvertiser = currentAdvertRow["id_advertiser"].ToString();
                                        if (FctUtilities.CheckedText.IsNotEmpty(idMedia) && !inAdvertMediaAlreadyUsedArr.Contains(idMedia))
                                        {
                                            if (dtAdvertiser.Columns.Contains("total_N"))
                                            {
                                                strExpr = "id_vehicle = " + idVehicle + " AND id_category = " + idCategory + " AND id_media=" + idMedia;
                                                strSort = "id_media ASC";
                                                foundRows = dtAdvertiser.Select(strExpr, strSort, DataViewRowState.OriginalRows);
                                                if (foundRows != null && foundRows.Length > 0)
                                                {
                                                    for (int i = 0; i < foundRows.Length; i++)
                                                    {//														
                                                        //investissement annonceur de r�f�rence ou concurrent  par support												
                                                        AdvertiserInvest = foundRows[i]["total_N"].ToString();
                                                        //														if(FctUtilities.CheckedText.IsNotEmpty(AdvertiserInvest.Trim()))
                                                        //															AdvertiserInvest=String.Format("{0:### ### ### ### ##0.##}",(double.Parse(AdvertiserInvest)/(double)1000));																												
                                                        //Insertion des donn�es dans la ligne courante pour un annonceur et par support
                                                        if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserInvestByVeh.Trim()))
                                                            tab = FillTabInvestPdmEvol(tab, indexTabRow, idVehicle, Vehicle, idCategory, Category, idMedia, Media, foundRows[i]["id_advertiser"].ToString(), foundRows[i]["advertiser"].ToString(), false, AdvertiserInvest, tempPDM, tempEvol, CstComparaisonCriterion.universTotal, CstResult.MediaStrategy.InvestmentType.advertiser, CstPreformatedDetail.PreformatedMediaDetails.vehicleCategoryMedia);
                                                        indexTabRow++;
                                                    }
                                                }
                                                if (!inAdvertMediaAlreadyUsedArr.Contains(idMedia)) inAdvertMediaAlreadyUsedArr.Add(idMedia);//Traitement unique de support											
                                            }
                                        }

                                    }
                                }
                                        #endregion
                                #endregion
                            }
                            if (!inTotUnivMediaAlreadyUsedArr.Contains(idMedia)) inTotUnivMediaAlreadyUsedArr.Add(idMedia);
                        }
                    }
                    #endregion

                    #endregion

                    OldIdVehicle = idVehicle;
                    OldIdCategory = idCategory;
                    OldIdMedia = idMedia;
                    start = 1;
                }
            }
            #endregion

            #endregion

            return tab;
        }
        #endregion

        #region M�thodes internes

        #region Annonceurs de r�f�rence ou concurrents
        /// <summary>
        /// Obtient les annonceurs de r�f�rence ou concurrents au niveau d'une cat�gorie ou d'un m�dia(support).
        /// </summary>
        /// <param name="webSession"></param>
        /// <param name="tab">session du client</param>
        /// <param name="dtTotalMarketOrSector">tableau de donn�es annonceurs total</param>
        /// <param name="foundRows">liste d'annonceurs</param>
        /// <param name="AdvertiserInvest">investissement par annonceur</param>
        /// <param name="AdvertiserInvest_N1">investissement par annonceur ann�e pr�c�dente</param>
        /// <param name="hPdmParentAdvert">PDM �l�ment parent</param>
        /// <param name="hPdmChildAdvert">PDM �l�ment enfant</param>
        /// <param name="indexTabRow">index ligne tableau</param>
        /// <param name="idVehicle">Identifiant m�dia</param>
        /// <param name="Vehicle">Libell� m�dia</param>
        /// <param name="idCategory">identifiant cat�gorie</param>
        /// <param name="Category">Libell� cat�gorie</param>
        /// <param name="idMedia">identifiant support</param>
        /// <param name="Media">Libell� support</param>
        /// <param name="preformatedMediaDetails">niveau de d�tail m�dia</param>
        /// <param name="isPluri">vrai si plurim�dia</param>
        /// <returns>Tableau de r�sultats</returns>
        protected object[,] FillAdvertisers(object[,] tab, DataTable dtTotalMarketOrSector, DataRow[] foundRows, ref string AdvertiserInvest, ref string AdvertiserInvest_N1, ref Hashtable hPdmParentAdvert, ref Hashtable hPdmChildAdvert, ref int indexTabRow, string idVehicle, string Vehicle, string idCategory, string Category, string idMedia, string Media, CstPreformatedDetail.PreformatedMediaDetails preformatedMediaDetails, bool isPluri)
        {
            string tempEvol = "";
            string tempPDM = "";
            double PdmVehicle = 0;
            double AdvertiserEvolution = 0;

            for (int n = 0; n < foundRows.Length; n++)
            {
                //investissement annonceur de r�f�rence ou concurrent  par m�dia
                //				if(FctUtilities.CheckedText.IsNotEmpty(foundRows[n]["total_N"].ToString()))
                //					AdvertiserInvest = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(foundRows[n]["total_N"].ToString())/(double)1000));															
                //PDM annonceur de r�f�rence ou concurrent  par m�dia																
                if (hPdmParentAdvert != null && foundRows[n]["id_advertiser"] != null && hPdmParentAdvert[foundRows[n]["id_advertiser"].ToString()] != null && FctUtilities.CheckedText.IsNotEmpty(AdvertiserInvest)
                    && FctUtilities.CheckedText.IsNotEmpty(foundRows[n]["id_advertiser"].ToString().Trim()) && double.Parse(hPdmParentAdvert[foundRows[n]["id_advertiser"].ToString()].ToString().Trim()) > (double)0.0)
                {
                    PdmVehicle = (double.Parse(AdvertiserInvest.ToString().Trim()) * (double)100.0) / double.Parse(hPdmParentAdvert[foundRows[n]["id_advertiser"].ToString()].ToString().Trim());
                    tempPDM = PdmVehicle.ToString();
                }
                else tempPDM = "";
                //Evolution par m�dia anne� N par rapport N-1= ((N-(N-1))*100)/N-1 																
                if (_session.ComparativeStudy && dtTotalMarketOrSector.Columns.Contains("total_N") && dtTotalMarketOrSector.Columns.Contains("total_N1"))
                {
                    AdvertiserEvolution = (double)0.0;
                    //investissement p�riode N-1 annonceur de r�f�rence ou concurrent  par m�dia
                    //					if(FctUtilities.CheckedText.IsNotEmpty(foundRows[n]["total_N1"].ToString()))
                    //						AdvertiserInvest_N1 = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(foundRows[n]["total_N1"].ToString())/(double)1000));
                    if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserInvest_N1) && FctUtilities.CheckedText.IsNotEmpty(AdvertiserInvest) && double.Parse(AdvertiserInvest_N1) > (double)0.0)
                    {
                        AdvertiserEvolution = ((double.Parse(AdvertiserInvest) - double.Parse(AdvertiserInvest_N1)) * (double)100.0) / double.Parse(AdvertiserInvest_N1);
                        tempEvol = AdvertiserEvolution.ToString();
                    }
                    else tempEvol = "";
                }
                else tempEvol = "";
                //Insertion des donn�es dans la ligne courante pour un annonceur et par m�dia
                if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserInvest.Trim()))
                    tab = FillTabInvestPdmEvol(tab, indexTabRow, idVehicle, Vehicle, idCategory, Category, idMedia, Media, foundRows[n]["id_advertiser"].ToString(), foundRows[n]["advertiser"].ToString(), isPluri, AdvertiserInvest, tempPDM, tempEvol, _session.ComparaisonCriterion, CstResult.MediaStrategy.InvestmentType.advertiser, preformatedMediaDetails);
                if (CstPreformatedDetail.PreformatedMediaDetails.vehicleCategoryMedia != preformatedMediaDetails && hPdmChildAdvert[foundRows[n]["id_advertiser"].ToString()] == null)
                    hPdmChildAdvert.Add(foundRows[n]["id_advertiser"].ToString().Trim(), AdvertiserInvest);
                indexTabRow++;
            }
            return tab;
        }

        /// <summary>
        /// Obtient les annonceurs de r�f�rence ou concurrents au niveau d'une cat�gorie ou d'un m�dia(support).
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <param name="tab">tableau de r�sultats</param>
        /// <param name="dtAdvertiser">tableau de donn�es annonceurs</param>
        /// <param name="dtTotalMarketOrSector">tableau de donn�es annonceurs total</param>
        /// <param name="expression">expression de s�lection</param>
        /// <param name="filter">expression de filtrage</param>
        /// <param name="hPdmVehAdvert">PDM annonceur par m�dia (vehicle)</param>
        /// <param name="hPdmCatAdvert">PDM annonceur par cat�gorie</param>
        /// <param name="indexTabRow">index ligne tableau</param>
        /// <param name="idVehicle">ID m�dia (vehicle)</param>
        /// <param name="Vehicle">Libell� m�dia (vehicle)</param>
        /// <param name="idCategory">ID cat�gorie </param>
        /// <param name="Category">libell� cat�gorie</param>
        /// <param name="idMedia">ID support</param>
        /// <param name="Media">libell� support</param>
        /// <param name="idAdvertiser">ID annonceur</param>
        /// <param name="preformatedMediaDetails">niveau de d�tail m�dia</param>
        /// <returns>tableau de r�sultats</returns>
        protected object[,] FillAdvertisers(object[,] tab, DataTable dtAdvertiser, DataTable dtTotalMarketOrSector, string expression, string filter, ref Hashtable hPdmVehAdvert, ref Hashtable hPdmCatAdvert, ref int indexTabRow, string idVehicle, string Vehicle, string idCategory, string Category, string idMedia, string Media, string idAdvertiser, CstPreformatedDetail.PreformatedMediaDetails preformatedMediaDetails)
        {

            double CatInvest = 0;
            double CatInvest_N1 = 0;
            string tempEvol = "";
            string tempPDM = "";
            double AdvertiserEvolution = 0;
            string AdvertiserInvestByCat = "";
            string AdvertiserInvestByCat_N1 = "";
            double PdmVehicle = 0;

            DataRow[] foundRows = dtAdvertiser.Select(expression, filter, DataViewRowState.OriginalRows);
            if (foundRows != null && foundRows.Length > 0)
            {
                CatInvest = (double)0.0;
                CatInvest_N1 = (double)0.0;
                //investissement annonceur de r�f�rence ou concurrent  par cat�gorie sur p�riode N
                for (int n = 0; n < foundRows.Length; n++)
                {
                    CatInvest += double.Parse(foundRows[n]["total_N"].ToString());
                    if (_session.ComparativeStudy) CatInvest_N1 += double.Parse(foundRows[n]["total_N1"].ToString());
                }
                AdvertiserInvestByCat = CatInvest.ToString();
                //				if(FctUtilities.CheckedText.IsNotEmpty(AdvertiserInvestByCat.Trim())){
                //					AdvertiserInvestByCat = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(AdvertiserInvestByCat)/(double)1000));																
                //				}
                if (hPdmVehAdvert != null && hPdmVehAdvert[idAdvertiser] != null && FctUtilities.CheckedText.IsNotEmpty(AdvertiserInvestByCat) && FctUtilities.CheckedText.IsNotEmpty(hPdmVehAdvert[idAdvertiser].ToString().Trim()) && double.Parse(hPdmVehAdvert[idAdvertiser].ToString().Trim()) > (double)0.0)
                {
                    PdmVehicle = (double.Parse(AdvertiserInvestByCat.ToString()) * (double)100.0) / double.Parse(hPdmVehAdvert[idAdvertiser].ToString().Trim());
                    tempPDM = PdmVehicle.ToString();
                }
                else tempPDM = "";
                //Evolution par cat�gorie anne� N par rapport N-1 = ((N-(N-1))*100)/N-1 	
                if (_session.ComparativeStudy && dtTotalMarketOrSector.Columns.Contains("total_N") && dtTotalMarketOrSector.Columns.Contains("total_N1"))
                {
                    AdvertiserEvolution = (double)0.0;
                    //investissement p�riode N-1 annonceur de r�f�rence ou concurrent  par cat�gorie																
                    AdvertiserInvestByCat_N1 = CatInvest_N1.ToString();
                    if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserInvestByCat_N1) && FctUtilities.CheckedText.IsNotEmpty(AdvertiserInvestByCat))
                    {
                        //						AdvertiserInvestByCat_N1 = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(AdvertiserInvestByCat_N1)/(double)1000));
                        if (double.Parse(AdvertiserInvestByCat_N1) > (double)0.0)
                        {
                            AdvertiserEvolution = ((double.Parse(AdvertiserInvestByCat) - double.Parse(AdvertiserInvestByCat_N1)) * (double)100.0) / double.Parse(AdvertiserInvestByCat_N1.Trim());
                            tempEvol = AdvertiserEvolution.ToString();
                        }
                        else tempEvol = "";
                    }
                    else tempEvol = "";
                }
                else tempEvol = "";
                //Insertion des donn�es dans la ligne courante pour un annonceur et par cat�gorie
                if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserInvestByCat.Trim()))
                    tab = FillTabInvestPdmEvol(tab, indexTabRow, idVehicle, Vehicle, idCategory, Category, idMedia, Media, foundRows[0]["id_advertiser"].ToString(), foundRows[0]["advertiser"].ToString(), false, AdvertiserInvestByCat, tempPDM, tempEvol, CstComparaisonCriterion.universTotal, CstResult.MediaStrategy.InvestmentType.advertiser, preformatedMediaDetails);
                hPdmCatAdvert.Add(idAdvertiser, AdvertiserInvestByCat);
                indexTabRow++;
            }


            return tab;
        }

        /// <summary>
        /// Obtient les annonceurs de r�f�rence ou concurrents au niveau d'un vehicle (m�dia) ou plurim�dia.
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <param name="tab">tableau de r�sultats</param>
        /// <param name="dtAdvertiser">tableau de donn�es annonceurs</param>
        /// <param name="dtTotalMarketOrSector">tableau de donn�es annonceurs total</param>
        /// <param name="expression">expression de s�lection</param>
        /// <param name="expressionPreviuousYear">expression de s�lection ann�e pr�c�dente</param>
        /// <param name="filter">filtre de donn�es</param>
        /// <param name="inAdvertVehicleAlreadyUsedArr">m�dia d�j� trait�s</param>
        /// <param name="hPdmVehAdvert">PDM m�dia (vehicle)</param>
        /// <param name="hPdmTotAdvert">PDM total</param>
        /// <param name="AdvertiserInvestByVeh">investissement annonceur par vehicle(media)</param>
        /// <param name="AdvertiserInvestByVeh_N1">investissement annonceur par vehicle(media) ann�e pr�c�dente</param>
        /// <param name="indexTabRow">index ligne du tableau</param>
        /// <param name="VehicleAccessList">liste des m�dia</param>
        /// <param name="idVehicle">identifiant des m�dia</param>
        /// <param name="Vehicle">libell� du vehicle</param>
        /// <param name="isPluri">vrai si plurim�dia</param>
        /// <param name="hasAdvertiser">vrai si poss�de des annonceurs</param>
        /// <returns>tableau de r�sultats</returns>
        protected object[,] FillAdvertisers(object[,] tab, DataTable dtAdvertiser, DataTable dtTotalMarketOrSector, string expression, string expressionPreviuousYear, string filter, ArrayList inAdvertVehicleAlreadyUsedArr, ref Hashtable hPdmVehAdvert, ref Hashtable hPdmTotAdvert, ref string AdvertiserInvestByVeh, ref string AdvertiserInvestByVeh_N1, ref int indexTabRow, string VehicleAccessList, string idVehicle, string Vehicle, bool isPluri, ref bool hasAdvertiser)
        {

            string idAdvertiser = "";
            double AdvertiserEvolution = 0;
            double PdmVehicle = 0;
            string tempEvol = "";
            string tempPDM = "";
            string localFilter = "";


            foreach (DataRow currentAdvertRow in dtAdvertiser.Rows)
            {
                //Pour les annonceurs de r�f�rence ou concurrents s�lectionn�s par media (vehicle) : Investissment,Evolution,PDM

                //colonne m�dia (vehicle)																						
                idAdvertiser = currentAdvertRow["id_advertiser"].ToString();
                if (!inAdvertVehicleAlreadyUsedArr.Contains(idAdvertiser))
                {
                    if (dtAdvertiser.Columns.Contains("total_N"))
                    {
                        if (filter.Length > 0) localFilter = filter + " AND id_advertiser=" + idAdvertiser;
                        else localFilter = " id_advertiser=" + idAdvertiser;
                        //investissement annonceur de r�f�rence ou concurrent  par m�dia (vehicle)
                        AdvertiserInvestByVeh = dtAdvertiser.Compute(expression, localFilter).ToString();
                        //						if(FctUtilities.CheckedText.IsNotEmpty(AdvertiserInvestByVeh.Trim())){
                        //							AdvertiserInvestByVeh = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(AdvertiserInvestByVeh)/(double)1000));														
                        //						}
                        //PDM annonceur de r�f�rence ou concurrent  par m�dia (vehicle)												
                        if (!isPluri && CstDbClassif.Vehicles.names.plurimedia == (CstDbClassif.Vehicles.names)int.Parse(VehicleAccessList))
                        {
                            if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserInvestByVeh) && hPdmTotAdvert != null && hPdmTotAdvert[idAdvertiser] != null && FctUtilities.CheckedText.IsNotEmpty(hPdmTotAdvert[idAdvertiser].ToString().Trim()) && double.Parse(hPdmTotAdvert[idAdvertiser].ToString().Trim()) > (double)0.0)
                            {
                                PdmVehicle = (double.Parse(AdvertiserInvestByVeh.ToString()) * (double)100.0) / double.Parse(hPdmTotAdvert[idAdvertiser].ToString().Trim());
                                tempPDM = PdmVehicle.ToString();
                            }
                            else tempPDM = "";
                        }
                        else tab[indexTabRow, PDM_COLUMN_INDEX] = "100";
                        //Evolution pour m�dia (vehicle) anne� N par rapport N-1	= ((N-(N-1))*100)/N-1 	
                        if (_session.ComparativeStudy && dtTotalMarketOrSector.Columns.Contains("total_N") && dtTotalMarketOrSector.Columns.Contains("total_N1"))
                        {
                            AdvertiserEvolution = (double)0.0;
                            //investissement p�riode N-1 annonceur de r�f�rence ou concurrent  par m�dia (vehicle)
                            AdvertiserInvestByVeh_N1 = dtAdvertiser.Compute(expressionPreviuousYear, localFilter).ToString();
                            if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserInvestByVeh_N1) && FctUtilities.CheckedText.IsNotEmpty(AdvertiserInvestByVeh))
                            {
                                //								AdvertiserInvestByVeh_N1 = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(AdvertiserInvestByVeh_N1)/(double)1000));
                                if (double.Parse(AdvertiserInvestByVeh_N1) > (double)0.0)
                                {
                                    AdvertiserEvolution = ((double.Parse(AdvertiserInvestByVeh) - double.Parse(AdvertiserInvestByVeh_N1)) * (double)100.0) / double.Parse(AdvertiserInvestByVeh_N1);
                                    tempEvol = AdvertiserEvolution.ToString();
                                }
                                else tempEvol = "";
                            }
                            else tempEvol = "";
                        }
                        else tempEvol = "";
                        //Insertion des donn�es dans la ligne courante pour un annonceur et par m�dia (vehicle)
                        if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserInvestByVeh.Trim()))
                            tab = FillTabInvestPdmEvol(tab, indexTabRow, idVehicle, Vehicle, "", "", "", "", currentAdvertRow["id_advertiser"].ToString(), currentAdvertRow["advertiser"].ToString(), false, AdvertiserInvestByVeh, tempPDM, tempEvol, CstComparaisonCriterion.universTotal, CstResult.MediaStrategy.InvestmentType.advertiser, CstPreformatedDetail.PreformatedMediaDetails.vehicle);
                        if (hPdmVehAdvert[idAdvertiser] == null) hPdmVehAdvert.Add(idAdvertiser, AdvertiserInvestByVeh);
                        if (!inAdvertVehicleAlreadyUsedArr.Contains(idAdvertiser)) inAdvertVehicleAlreadyUsedArr.Add(idAdvertiser);
                        indexTabRow++;
                    }

                }
                localFilter = "";
                if (isPluri) hasAdvertiser = true;
            }

            return tab;
        }
        #endregion

        #region Investissement,PDM,Evolution

        /// <summary>
        /// Calcule les investissements,PDM et l'evolution pour chaque m�dia.
        /// </summary>
        /// <param name="dt">Table de donn�es</param>
        /// <param name="totalInvestParent">Investissement du m�dia parent</param>
        /// <param name="totalInvestChild">Investissement du m�dia fils</param>
        /// <param name="totalInvestChildPreviousYear">Investissement du m�dia fils ann�e pr�c�dente</param>
        /// <param name="mediaPDM">PDM m�dia</param>	
        /// <param name="evolution">evolution</param>
        /// <param name="expression">expression selection lignes tables de donn�es</param>
        /// <param name="expressionPreviuousYear">expression selection lignes tables de donn�es ann�e pr�c�dente</param>
        /// <param name="filter">expression filtrage des donn�es</param>	
        /// <param name="comparativeStudy">vrai si �tude comparative</param>
        protected void ComputeInvestPdmEvol(DataTable dt, ref string totalInvestParent, ref string totalInvestChild, ref string totalInvestChildPreviousYear, ref string mediaPDM, ref string evolution, string expression, string expressionPreviuousYear, string filter, bool comparativeStudy)
        {

            double tempPDM = 0;
            double tempEvol = 0;

            //Investissement total univers pour m�dia (vehicle)
            totalInvestChild = dt.Compute(expression, filter).ToString();
            //			if(FctUtilities.CheckedText.IsNotEmpty(totalInvestChild.ToString().Trim())){
            //				totalInvestChild=String.Format("{0:### ### ### ### ##0.##}",(double.Parse(totalInvestChild)/(double)1000));
            //			}

            //PDM pour m�dia 				
            if (FctUtilities.CheckedText.IsNotEmpty(totalInvestParent) && FctUtilities.CheckedText.IsNotEmpty(totalInvestChild.ToString()))
            {
                tempPDM = (double)0.0;
                if (FctUtilities.CheckedText.IsNotEmpty(totalInvestParent) && double.Parse(totalInvestParent) > (double)0.0)
                {
                    tempPDM = (double.Parse(totalInvestChild.ToString()) * (double)100.0) / double.Parse(totalInvestParent);
                    mediaPDM = tempPDM.ToString();
                }
                else mediaPDM = "";
            }
            else mediaPDM = "";

            //Evolution pour m�dia 
            if (comparativeStudy && dt.Columns.Contains("total_N") && dt.Columns.Contains("total_N1"))
            {
                tempEvol = (double)0.0;
                totalInvestChildPreviousYear = dt.Compute(expressionPreviuousYear, filter).ToString();
                //				if(FctUtilities.CheckedText.IsNotEmpty(totalInvestChildPreviousYear))totalInvestChildPreviousYear =String.Format("{0:### ### ### ### ##0.##}",(double.Parse(totalInvestChildPreviousYear)/(double)1000));
                //anne� N par rapport N-1 = ((N-(N-1))*100)/N-1 
                if (FctUtilities.CheckedText.IsNotEmpty(totalInvestChildPreviousYear) && FctUtilities.CheckedText.IsNotEmpty(totalInvestChild) && double.Parse(totalInvestChildPreviousYear) > (double)0.0)
                {
                    tempEvol = ((double.Parse(totalInvestChild) - double.Parse(totalInvestChildPreviousYear)) * (double)100.0) / double.Parse(totalInvestChildPreviousYear);
                    evolution = tempEvol.ToString();
                }
            }
        }


        /// <summary>
        /// Remplit chaque ligne du tableau de r�sultats avec
        /// Investissement pour un total univers ou march� ou famille, ou un annonceur de r�f�rence ou concurrent,
        /// par m�dia ou cat�gorie ou support
        /// </summary>
        /// <param name="webSession">session client</param>
        /// <param name="tab">tableau de r�sultats</param>
        /// <param name="indexTabRow">index ligne du tableau</param>
        /// <param name="idVehicle">identifiant m�dia</param>
        /// <param name="Vehicle">lib�ll� m�dia</param>		
        /// <param name="plurimedia">vrai si s�lection plurim�dia</param>
        /// <param name="Invest">investissement en Keuros soit pour un total univers ou march� ou famille, 
        /// ou un annonceur de r�f�rence ou concurrent</param>
        /// <param name="PDM">Part de march�</param>
        /// <param name="Evolution">Evolution p�riode N par rapport � N-1</param>
        /// <param name="comparisonCriterion">total march� ou famille ou univers</param>
        /// <param name="investmentType">investissement total famille ou march� ou univers, ou pour un annonceur de r�f�rence ou concurrent</param>
        /// <param name="preformatedMediaDetail">niveau de d�tail m�dia</param>
        /// <returns>tableau de r�sultats</returns>		
        protected object[,] FillTabInvestPdmEvol(object[,] tab, int indexTabRow, string idVehicle, string Vehicle, bool plurimedia, string Invest, string PDM, string Evolution, CstComparaisonCriterion comparisonCriterion, CstResult.MediaStrategy.InvestmentType investmentType, CstPreformatedDetail.PreformatedMediaDetails preformatedMediaDetail)
        {
            return FillTabInvestPdmEvol(tab, indexTabRow, idVehicle, Vehicle, "", "", "", "", "", "", plurimedia, Invest, PDM, Evolution, comparisonCriterion, investmentType, preformatedMediaDetail);
        }

        /// <summary>
        /// Remplit chaque ligne du tableau de r�sultats avec
        /// Investissement pour un total univers ou march� ou famille, ou un annonceur de r�f�rence ou concurrent,
        /// par m�dia ou cat�gorie ou support
        /// </summary>
        /// <param name="webSession">session client</param>
        /// <param name="tab">tableau de r�sultats</param>
        /// <param name="indexTabRow">index ligne du tableau</param>
        /// <param name="idVehicle">identifiant m�dia</param>
        /// <param name="Vehicle">lib�ll� m�dia</param>
        /// <param name="idCategory">identifiant cat�gorie</param>
        /// <param name="Category">lib�ll� cat�gorie</param>
        /// <param name="idMedia">identifiant support</param>
        /// <param name="Media">lib�ll� support</param>
        /// <param name="idRefOrCompetAdvertiser">identifiant annonceur de r�f�rence ou concurrent</param>
        /// <param name="RefOrCompetAdvertiser">lib�ll� annonceur de r�f�rence ou concurrent</param>
        /// <param name="plurimedia">vrai si s�lection plurim�dia</param>
        /// <param name="Invest">investissement en Keuros soit pour un total univers ou march� ou famille, 
        /// ou un annonceur de r�f�rence ou concurrent</param>
        /// <param name="PDM">Part de march�</param>
        /// <param name="Evolution">Evolution p�riode N par rapport � N-1</param>
        /// <param name="comparisonCriterion">total march� ou famille ou univers</param>
        /// <param name="investmentType">investissement total famille ou march� ou univers, ou pour un annonceur de r�f�rence ou concurrent</param>
        /// <param name="preformatedMediaDetail">niveau de d�tail m�dia</param>
        /// <returns>tableau de r�sultats</returns>		
        protected object[,] FillTabInvestPdmEvol(object[,] tab, int indexTabRow, string idVehicle, string Vehicle, string idCategory, string Category, string idMedia, string Media, string idRefOrCompetAdvertiser, string RefOrCompetAdvertiser, bool plurimedia, string Invest, string PDM, string Evolution, TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion comparisonCriterion, CstResult.MediaStrategy.InvestmentType investmentType, CstPreformatedDetail.PreformatedMediaDetails preformatedMediaDetail)
        {
            /*Remplit chaqe colonne de la ligne courante du tableau*/

            //colonne ID media 
            if (FctUtilities.CheckedText.IsNotEmpty(idVehicle.Trim())) tab[indexTabRow, ID_VEHICLE_COLUMN_INDEX] = idVehicle;
            //colonne lib�ll� media 
            if (FctUtilities.CheckedText.IsNotEmpty(Vehicle.Trim())) tab[indexTabRow, LABEL_VEHICLE_COLUMN_INDEX] = Vehicle;
            //colonne ID categorie 
            if (FctUtilities.CheckedText.IsNotEmpty(idCategory.Trim())) tab[indexTabRow, ID_CATEGORY_COLUMN_INDEX] = idCategory;
            //colonne lib�ll� cat�gorie 
            if (FctUtilities.CheckedText.IsNotEmpty(Category.Trim())) tab[indexTabRow, LABEL_CATEGORY_COLUMN_INDEX] = Category;
            //colonne ID support
            if (FctUtilities.CheckedText.IsNotEmpty(idMedia.Trim())) tab[indexTabRow, ID_MEDIA_COLUMN_INDEX] = idMedia;
            //colonne lib�ll� support 
            if (FctUtilities.CheckedText.IsNotEmpty(Media.Trim())) tab[indexTabRow, LABEL_MEDIA_COLUMN_INDEX] = Media;
            //PDM 
            if (FctUtilities.CheckedText.IsNotEmpty(PDM.Trim())) tab[indexTabRow, PDM_COLUMN_INDEX] = PDM;
            //EVOLUTION p�riode N/N-1
            if (FctUtilities.CheckedText.IsNotEmpty(Evolution.Trim())) tab[indexTabRow, EVOL_COLUMN_INDEX] = Evolution;

            if (investmentType == CstResult.MediaStrategy.InvestmentType.total)
            {
                //investissement ann�e N pour total univers ou march� ou famille
                if (FctUtilities.CheckedText.IsNotEmpty(Invest.Trim()))
                {
                    //Remplit les investissement totaux pour la famille ou l'ensemble du march�
                    if (comparisonCriterion == CstComparaisonCriterion.marketTotal || comparisonCriterion == CstComparaisonCriterion.sectorTotal)
                    {
                        switch (preformatedMediaDetail)
                        {
                            case CstPreformatedDetail.PreformatedMediaDetails.vehicle:
                                if (comparisonCriterion == CstComparaisonCriterion.marketTotal)
                                {
                                    if (plurimedia) tab[indexTabRow, TOTAL_MARKET_INVEST_COLUMN_INDEX] = Invest;
                                    else tab[indexTabRow, TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX] = Invest;
                                }
                                else
                                {
                                    if (plurimedia) tab[indexTabRow, TOTAL_SECTOR_INVEST_COLUMN_INDEX] = Invest;
                                    else tab[indexTabRow, TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX] = Invest;
                                }
                                break;
                            case CstPreformatedDetail.PreformatedMediaDetails.vehicleCategory:
                                if (_session.ComparaisonCriterion == CstComparaisonCriterion.marketTotal)
                                    tab[indexTabRow, TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX] = Invest;
                                else tab[indexTabRow, TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX] = Invest;
                                break;
                            case CstPreformatedDetail.PreformatedMediaDetails.vehicleCategoryMedia:
                                if (_session.ComparaisonCriterion == CstComparaisonCriterion.marketTotal)
                                    tab[indexTabRow, TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX] = Invest;
                                else tab[indexTabRow, TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX] = Invest;
                                break;
                            default:
                                throw (new ProductClassIndicatorsException(GestionWeb.GetWebWord(1237, _session.SiteLanguage)));
                        }
                        //Remplit les investissement totaux pour l'univers
                    }
                    else if (comparisonCriterion == CstComparaisonCriterion.universTotal)
                    {
                        switch (preformatedMediaDetail)
                        {
                            case CstPreformatedDetail.PreformatedMediaDetails.vehicle:
                                if (plurimedia)
                                {
                                    tab[indexTabRow, TOTAL_UNIV_INVEST_COLUMN_INDEX] = Invest;
                                }
                                else tab[indexTabRow, TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX] = Invest;
                                break;
                            case CstPreformatedDetail.PreformatedMediaDetails.vehicleCategory:
                                tab[indexTabRow, TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX] = Invest;
                                break;
                            case CstPreformatedDetail.PreformatedMediaDetails.vehicleCategoryMedia:
                                tab[indexTabRow, TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX] = Invest;
                                break;
                            default:
                                throw (new ProductClassIndicatorsException(GestionWeb.GetWebWord(1237, _session.SiteLanguage)));
                        }
                    }
                }
            }
            else if (investmentType == CstResult.MediaStrategy.InvestmentType.advertiser)
            {
                //investissement ann�e N pour annonceur de r�f�rence ou concurrent
                if (FctUtilities.CheckedText.IsNotEmpty(Invest.ToString().Trim()))
                {
                    if (FctUtilities.CheckedText.IsNotEmpty(RefOrCompetAdvertiser.Trim()))
                        tab[indexTabRow, LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] = RefOrCompetAdvertiser.ToString();
                    if (FctUtilities.CheckedText.IsNotEmpty(idRefOrCompetAdvertiser.ToString().Trim()))
                        tab[indexTabRow, ID_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] = idRefOrCompetAdvertiser.ToString();
                    if (plurimedia)
                    {
                        tab[indexTabRow, TOTAL_REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] = Invest;
                    }
                    else tab[indexTabRow, REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] = Invest;
                }
            }

            return tab;
        }
        #endregion

        #region Traitement premier annonceur ou r�f�rence
        /// <summary>
        /// Remplit chaque ligne du tableau de r�sultats avec
        /// Investissement les donn�es du premier annonceur ou r�f�rence de chauqe m�dia.
        /// </summary>
        /// <param name="webSession">session</param>
        /// <param name="tab">tableau de r�sultats</param>
        /// <param name="comparisonCriterion">crit�re de comparaison</param>
        /// <param name="TotalMarketOrSectorInvest">investissement total</param>
        /// <param name="indexTabRow">index des lignes du tableau r�sultas</param>
        /// <param name="increment">vrai si change de ligne</param>
        /// <returns>tableau de r�sultats</returns>
        protected object[,] FillTabFisrtElmt(object[,] tab, CstComparaisonCriterion comparisonCriterion, string TotalMarketOrSectorInvest, ref int indexTabRow, bool increment)
        {

            //Remplit la ligne courante avec le lib�ll� et l'investissment du premier annonceur,	pour le total march� et plurim�dia 					
            DataSet dsPluri = _dalLayer.GetMediaStrategyTopsData(CstResult.MotherRecap.ElementType.advertiser, comparisonCriterion, CstResult.MediaStrategy.MediaLevel.vehicleLevel,true);
            if (dsPluri != null && dsPluri.Tables[0] != null && dsPluri.Tables[0].Rows.Count == 1 && FctUtilities.CheckedText.IsNotEmpty(TotalMarketOrSectorInvest) && (double.Parse(TotalMarketOrSectorInvest) > (double)0.0))
                tab = FillTabFisrtElmt(tab, indexTabRow, CstResult.MotherRecap.ElementType.advertiser, "", "", dsPluri.Tables[0], true);
            //Remplit la ligne courante avec le lib�ll� et l'investissment de la  premiere r�f�rence,	pour le total march� et plurim�dia 						
            dsPluri = _dalLayer.GetMediaStrategyTopsData(CstResult.MotherRecap.ElementType.product, comparisonCriterion, CstResult.MediaStrategy.MediaLevel.vehicleLevel,true);
            if (dsPluri != null && dsPluri.Tables[0] != null && dsPluri.Tables[0].Rows.Count == 1 && FctUtilities.CheckedText.IsNotEmpty(TotalMarketOrSectorInvest) && (double.Parse(TotalMarketOrSectorInvest) > (double)0.0))
                tab = FillTabFisrtElmt(tab, indexTabRow, CstResult.MotherRecap.ElementType.product, "", "", dsPluri.Tables[0], true);
            if (increment) indexTabRow++; //ligne suivante

            return tab;
        }

        /// <summary>
        /// Remplit chaque ligne du tableau de r�sultats avec
        /// Investissement les donn�es du premier annonceur ou r�f�rence de chauqe m�dia.
        /// </summary>	
        /// <param name="tab">tableau de r�sultats</param>
        /// <param name="totalInvestChild">Investissement du m�dia fils</param>		
        /// <param name="dt1stAdvertiser">Table de donn�es premier annonceur</param>
        /// <param name="dt1stProduct">Table de donn�es premiere r�f�rence</param>
        /// <param name="idElement">identifiant du m�dia</param>
        /// <param name="LabelElement">libell� du m�dia</param>
        /// <param name="inMediaAlreadyUsedArr">M�dias d�j� trait�s</param>
        /// <param name="indexTabRow">index des lignes du tableau r�sultas</param>
        /// <param name="increment">vrai si la ligne doit �tre incr�ment�e</param>
        /// <param name="isPluri">vrai si plurim�dia</param>
        /// <returns>tableau de r�sultats</returns>
        protected object[,] FillTabFisrtElmt(object[,] tab, ref string totalInvestChild, DataTable dt1stAdvertiser, DataTable dt1stProduct, string LabelElement, string idElement, ref ArrayList inMediaAlreadyUsedArr, ref int indexTabRow, bool increment, bool isPluri)
        {
            //Remplit la ligne courante avec le lib�ll� et l'investissment du 1er annonceur, pour le total, et par m�dia 						
            if (FctUtilities.CheckedText.IsNotEmpty(totalInvestChild) && (double.Parse(totalInvestChild) > (double)0.0) && dt1stAdvertiser != null && dt1stAdvertiser.Rows.Count > 0)
                tab = FillTabFisrtElmt(tab, indexTabRow, CstResult.MotherRecap.ElementType.advertiser, LabelElement, idElement, dt1stAdvertiser, isPluri);
            //Remplit la ligne courante avec le lib�ll� et l'investissment du 1ere r�f�rence, pour le total, et par m�dia 							
            if (FctUtilities.CheckedText.IsNotEmpty(totalInvestChild) && (double.Parse(totalInvestChild) > (double)0.0) && dt1stProduct != null && dt1stProduct.Rows.Count > 0)
                tab = FillTabFisrtElmt(tab, indexTabRow, CstResult.MotherRecap.ElementType.product, LabelElement, idElement, dt1stProduct, isPluri);

            if (!inMediaAlreadyUsedArr.Contains(idElement)) inMediaAlreadyUsedArr.Add(idElement);
            if (increment) indexTabRow++; //ligne suivante

            return tab;
        }

        /// <summary>
        /// Remplit chaque ligne du tableau de r�sultats concernant les totaux
        /// univers ou famille ou march� par les lib�ll�s des annonceurs de r�f�rence
        /// ou concurrentsainsi que leurs investissements en Keuros.
        /// </summary>
        /// <param name="tab">tableau de r�sultats</param>
        /// <param name="indexTabRow">index ligne du tableau</param>		
        /// <param name="elementType">r�f�rence ou annonceur</param>
        /// <param name="dtFirstElmt">table de donn�es</param>
        /// <param name="idMedia">identifiant du m�dia (m�dia ou cat�gorie ou support)</param>
        /// <param name="idMediaName">libell� du m�dia</param>
        /// <param name="pluri">bool�en pour pr�ciser s'il s'agit d'une s�lection mono ou plurim�dia</param>
        /// <returns>tableau de r�sultats</returns>
        protected object[,] FillTabFisrtElmt(object[,] tab, int indexTabRow, CstResult.Novelty.ElementType elementType, string idMediaName, string idMedia, DataTable dtFirstElmt, bool pluri)
        {
            #region variables locales
            string strExpr = "";
            string strSort = "";
            DataRow[] foundRows = null;
            string tempInvest = "";
            #endregion
            //Remplit la ligne courante avec le lib�ll� et l'investissment du 1er annonceur ou r�f�rence, pour le total march� ou famille ou univers, et par m�dia (m�dia ou cat�gorie ou support)
            if (dtFirstElmt != null && dtFirstElmt.Rows.Count > 0 && dtFirstElmt.Columns.Contains("total_N"))
            {
                //Plurimedia
                if (pluri) foundRows = dtFirstElmt.Select();
                else
                {
                    //MonoMedia
                    strExpr = " " + idMediaName + "=" + idMedia;
                    strSort = "total_N  ASC";
                    foundRows = dtFirstElmt.Select(strExpr, strSort, DataViewRowState.OriginalRows);
                }
                if (foundRows != null && foundRows.Length > 0 && foundRows[0] != null)
                {
                    //					tempInvest = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(foundRows[0]["total_N"].ToString())/(double)1000));
                    tempInvest = foundRows[0]["total_N"].ToString();
                    //Insertion des donn�es dans la ligne courante
                    if (FctUtilities.CheckedText.IsNotEmpty(tempInvest.ToString().Trim()))
                    {
                        if (dtFirstElmt.Columns.Contains("advertiser"))
                        {
                            if (pluri) tab = FillTabFirstElmt(tab, indexTabRow, "", foundRows[0]["advertiser"].ToString(), tempInvest, CstResult.MotherRecap.ElementType.advertiser);
                            else tab = FillTabFirstElmt(tab, indexTabRow, foundRows[0]["id_advertiser"].ToString(), foundRows[0]["advertiser"].ToString(), tempInvest, CstResult.MotherRecap.ElementType.advertiser);
                        }
                        else if (dtFirstElmt.Columns.Contains("product"))
                        {
                            if (pluri) tab = FillTabFirstElmt(tab, indexTabRow, "", foundRows[0]["product"].ToString(), tempInvest, CstResult.MotherRecap.ElementType.product);
                            else tab = FillTabFirstElmt(tab, indexTabRow, foundRows[0]["id_product"].ToString(), foundRows[0]["product"].ToString(), tempInvest, CstResult.MotherRecap.ElementType.product);
                        }
                    }
                }
            }
            return tab;
        }

        /// <summary>
        /// Remplit chaque ligne du tableau de r�sultats concernant les totaux
        /// univers ou famille ou march� par les lib�ll�s des annonceurs de r�f�rence
        /// ou concurrentsainsi que leurs investissements en Keuros.
        /// </summary>
        /// <param name="tab">tableau de r�sultats</param>
        /// <param name="indexTabRow">index ligne du tableau</param>
        /// <param name="idElement">identifiant annonceur de r�f�rence ou concurrent</param>
        /// <param name="LabelElement">lib�ll� annonceur de r�f�rence ou concurrent</param>
        /// <param name="InvestElement">investissement annonceur de r�f�rence ou concurrent</param>
        /// <param name="elementType">r�f�rence ou annonceur</param>
        /// <returns>tableau de r�sultats</returns>
        protected object[,] FillTabFirstElmt(object[,] tab, int indexTabRow, string idElement, string LabelElement, string InvestElement, CstResult.Novelty.ElementType elementType)
        {
            if (CstResult.MotherRecap.ElementType.advertiser == elementType)
            {
                //ID premier annonceur
                if (FctUtilities.CheckedText.IsNotEmpty(idElement.Trim()))
                    tab[indexTabRow, ID_FIRST_ADVERT_COLUMN_INDEX] = idElement;
                //Lib�ll� premier annonceur
                if (FctUtilities.CheckedText.IsNotEmpty(LabelElement.Trim()))
                    tab[indexTabRow, LABEL_FIRST_ADVERT_COLUMN_INDEX] = LabelElement;
                //investissement premiere annonceur
                if (FctUtilities.CheckedText.IsNotEmpty(InvestElement.Trim()))
                    tab[indexTabRow, INVEST_FIRST_ADVERT_COLUMN_INDEX] = InvestElement;
            }
            else if (CstResult.MotherRecap.ElementType.product == elementType)
            {
                //ID premier r�f�rence
                if (FctUtilities.CheckedText.IsNotEmpty(idElement.Trim()))
                    tab[indexTabRow, ID_FIRST_REF_COLUMN_INDEX] = idElement;
                //Lib�ll� premier r�f�rence
                if (FctUtilities.CheckedText.IsNotEmpty(LabelElement.Trim()))
                    tab[indexTabRow, LABEL_FIRST_REF_COLUMN_INDEX] = LabelElement;
                //investissement premiere r�f�rence
                if (FctUtilities.CheckedText.IsNotEmpty(InvestElement.Trim()))
                    tab[indexTabRow, INVEST_FIRST_REF_COLUMN_INDEX] = InvestElement;
            }
            else
            {
                throw (new ProductClassIndicatorsException("Impossible d'identifier le type d'�l�ments (produits ou annonceurs) � afficher."));
            }

            return tab;
        }



        /// <summary>
        /// R�cup�re les tables de donn�es pour les 1ers annonceurs ou r�f�rences par m�dia
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <param name="dt1stProductByVeh">table de donn�es 1ere r�f�rence par m�dia </param>
        /// <param name="dt1stAdvertiserByVeh">table de donn�es 1er annonceur par m�dia</param>
        /// <param name="dt1stProductByCat">table de donn�es 1ere r�f�rence par cat�gorie</param>
        /// <param name="dt1stAdvertiserByCat">table de donn�es 1er annonceur par cat�gorie</param>
        /// <param name="dt1stProductByMed">table de donn�es 1ere r�f�rence par support</param>
        /// <param name="dt1stAdvertiserByMed">table de donn�es 1ere r�f�rence par annonceur</param>
        /// <param name="comparisonCriterion">crit�re de comparaisonn (univers,march�,famille)</param>
        /// <param name="mediaLevel">niveau m�dia</param>
        protected void Get1stElmtDataTbleByMedia(ref DataTable dt1stProductByVeh, ref DataTable dt1stAdvertiserByVeh, ref DataTable dt1stProductByCat, ref DataTable dt1stAdvertiserByCat, ref DataTable dt1stProductByMed, ref DataTable dt1stAdvertiserByMed, CstComparaisonCriterion comparisonCriterion, CstResult.MediaStrategy.MediaLevel mediaLevel)
        {

            #region variables locales
            //groupe de donn�es pour premiere r�f�rence 
            DataSet dsTotalFirstProduct = null;
            //groupe de donn�es pour premier annonceur 
            DataSet dsTotalFirstAdvertiser = null;
            #endregion

            //Donn�es pour premi�re r�f�rence par m�dia (vehicle)
            dsTotalFirstProduct = _dalLayer.GetMediaStrategyTopsData(CstResult.MotherRecap.ElementType.product, comparisonCriterion, CstResult.MediaStrategy.MediaLevel.vehicleLevel, false);
            if (dsTotalFirstProduct != null && dsTotalFirstProduct.Tables[0] != null && dsTotalFirstProduct.Tables[0].Rows.Count > 0)
                dt1stProductByVeh = dsTotalFirstProduct.Tables[0];

            //Donn�es pour premier annonceur par m�dia (vehicle)
            dsTotalFirstAdvertiser = _dalLayer.GetMediaStrategyTopsData(CstResult.MotherRecap.ElementType.advertiser, comparisonCriterion, CstResult.MediaStrategy.MediaLevel.vehicleLevel, false);
            if (dsTotalFirstAdvertiser != null && dsTotalFirstAdvertiser.Tables[0] != null && dsTotalFirstAdvertiser.Tables[0].Rows.Count > 0)
                dt1stAdvertiserByVeh = dsTotalFirstAdvertiser.Tables[0];
            if ((CstResult.MediaStrategy.MediaLevel.categoryLevel == mediaLevel) || (CstResult.MediaStrategy.MediaLevel.mediaLevel == mediaLevel))
            {
                //Donn�es pour premi�re r�f�rence par m�dia (cat�gorie)
                dsTotalFirstProduct = _dalLayer.GetMediaStrategyTopsData(CstResult.MotherRecap.ElementType.product, comparisonCriterion, CstResult.MediaStrategy.MediaLevel.categoryLevel, false);
                if (dsTotalFirstProduct != null && dsTotalFirstProduct.Tables[0] != null && dsTotalFirstProduct.Tables[0].Rows.Count > 0)
                    dt1stProductByCat = dsTotalFirstProduct.Tables[0];

                //Donn�es pour premier annonceur par m�dia (cat�gorie)
                dsTotalFirstAdvertiser = _dalLayer.GetMediaStrategyTopsData(CstResult.MotherRecap.ElementType.advertiser, comparisonCriterion, CstResult.MediaStrategy.MediaLevel.categoryLevel, false);
                if (dsTotalFirstAdvertiser != null && dsTotalFirstAdvertiser.Tables[0] != null && dsTotalFirstAdvertiser.Tables[0].Rows.Count > 0)
                    dt1stAdvertiserByCat = dsTotalFirstAdvertiser.Tables[0];
            }
            if (CstResult.MediaStrategy.MediaLevel.mediaLevel == mediaLevel)
            {
                //Donn�es pour premi�re r�f�rence par support
                dsTotalFirstProduct = _dalLayer.GetMediaStrategyTopsData(CstResult.MotherRecap.ElementType.product, comparisonCriterion, CstResult.MediaStrategy.MediaLevel.mediaLevel, false);
                if (dsTotalFirstProduct != null && dsTotalFirstProduct.Tables[0] != null && dsTotalFirstProduct.Tables[0].Rows.Count > 0)
                    dt1stProductByMed = dsTotalFirstProduct.Tables[0];

                //Donn�es pour premier annonceur par support
                dsTotalFirstAdvertiser = _dalLayer.GetMediaStrategyTopsData(CstResult.MotherRecap.ElementType.advertiser, comparisonCriterion, CstResult.MediaStrategy.MediaLevel.mediaLevel, false);
                if (dsTotalFirstAdvertiser != null && dsTotalFirstAdvertiser.Tables[0] != null && dsTotalFirstAdvertiser.Tables[0].Rows.Count > 0)
                    dt1stAdvertiserByMed = dsTotalFirstAdvertiser.Tables[0];
            }
        }
        #endregion

        #region Initialisation tableau r�sultats
        /// <summary>
        /// Initialise le tableau de r�sultas des r�partion m�dia en investissments.
        /// </summary>
        /// <param name="webSession">session du client </param>
        /// <param name="dtTotal">table de donn�es total famille ou march�</param>
        /// <param name="dtAdvertiser">table de donn�es annonceurs de r�f�rences ou concurrents</param>		
        /// <param name="VehicleAccessList">m�dia en acc�s</param>
        /// <param name="NB_MAX_COLUMNS">nombre max de colonne dans tableau</param>
        /// <returns>instance tableau de r�sultats</returns>
        protected object[,] TabInstance(System.Data.DataTable dtTotal, System.Data.DataTable dtAdvertiser, string VehicleAccessList, int NB_MAX_COLUMNS)
        {

            #region variables
            //tableau de r�sultats
            object[,] tab = null;
            //identifiant m�dia pr�c�dent
            string OldIdVehicle = "0";
            //nombre de m�dia (vehicle)
            int nbVehicle = 0;
            //collection identifiant pr�c�dentes cat�gories
            ArrayList OldIdCategoryArr = new ArrayList();
            //nombre de cat�gories
            int nbCategory = 0;
            //collection identifiant pr�c�dents supports
            ArrayList OldIdMediaArr = new ArrayList();
            //nombre de supports
            int nbMedia = 0;
            //nombre  annonceurs de r�f�rences ou concurrents
            int nbAdvertiser = 0;
            //collection des annonceurs d�j� trait�s
            ArrayList OldIdAdvertiserArr = new ArrayList();
            //nombre maximal de lignes
            int nbMaxLines = 0;
            #endregion

            #region instanciation du tableau de r�sultats
            /*On instancie le tableau de r�sultats pour strat�gie m�dia
			 */
            if (dtTotal != null && dtTotal.Rows.Count > 0)
            {
                //calcule nombre maximal de lignes
                switch (_session.PreformatedMediaDetail)
                {
                    //s�lection plurimedia
                    case CstPreformatedDetail.PreformatedMediaDetails.vehicle:
                        //nombre de m�dia (vehicle)
                        foreach (DataRow curRow in dtTotal.Rows)
                        {
                            if (dtTotal.Columns.Contains("id_vehicle") && (int.Parse(OldIdVehicle) != int.Parse(curRow["id_vehicle"].ToString())))
                            {
                                nbVehicle++;
                            }
                            OldIdVehicle = curRow["id_vehicle"].ToString();
                        }
                        if (dtAdvertiser != null && dtAdvertiser.Rows.Count > 0)
                        {
                            //nombre d'annonceurs
                            foreach (DataRow currRow in dtAdvertiser.Rows)
                            {
                                if (dtAdvertiser.Columns.Contains("id_advertiser") && !OldIdAdvertiserArr.Contains(currRow["id_advertiser"].ToString()))
                                {
                                    nbAdvertiser++;
                                    OldIdAdvertiserArr.Add(currRow["id_advertiser"].ToString());
                                }
                            }
                        }
                        //calcule du nombre maximal de lignes
                        if (FctUtilities.CheckedText.IsNotEmpty(VehicleAccessList) && CstDbClassif.Vehicles.names.plurimedia == (CstDbClassif.Vehicles.names)int.Parse(VehicleAccessList))
                            nbMaxLines = 2 + nbAdvertiser + (2 + nbAdvertiser) * nbVehicle;
                        else nbMaxLines = (2 + nbAdvertiser) * nbVehicle;
                        break;
                    //s�lection monomedia et cat�gories
                    case CstPreformatedDetail.PreformatedMediaDetails.vehicleCategory:
                        //nombre de m�dia (vehicle)
                        foreach (DataRow curRow in dtTotal.Rows)
                        {
                            if (dtTotal.Columns.Contains("id_vehicle") && (int.Parse(OldIdVehicle) != int.Parse(curRow["id_vehicle"].ToString())))
                            {
                                nbVehicle++;
                            }
                            //nombre de cat�gories
                            if (dtTotal.Columns.Contains("id_category") && !OldIdCategoryArr.Contains(curRow["id_category"].ToString()))
                            {
                                nbCategory++;
                                OldIdCategoryArr.Add(curRow["id_category"].ToString());
                            }
                            OldIdVehicle = curRow["id_vehicle"].ToString();
                        }
                        if (dtAdvertiser != null && dtAdvertiser.Rows.Count > 0)
                        {
                            //nombre d'annonceurs
                            foreach (DataRow currRow in dtAdvertiser.Rows)
                            {
                                if (dtAdvertiser.Columns.Contains("id_advertiser") && !OldIdAdvertiserArr.Contains(currRow["id_advertiser"].ToString()))
                                {
                                    nbAdvertiser++;
                                    OldIdAdvertiserArr.Add(currRow["id_advertiser"].ToString());
                                }
                            }
                        }
                        //calcule du nombre maximal de lignes
                        if (FctUtilities.CheckedText.IsNotEmpty(VehicleAccessList) && CstDbClassif.Vehicles.names.plurimedia == (CstDbClassif.Vehicles.names)int.Parse(VehicleAccessList))
                            nbMaxLines = 2 + nbAdvertiser + ((2 + nbAdvertiser) * nbCategory) * nbVehicle;
                        else nbMaxLines = 2 + nbAdvertiser + (2 + nbAdvertiser) * nbCategory;
                        break;
                    //s�lection monomedia et cat�gories et supports
                    case CstPreformatedDetail.PreformatedMediaDetails.vehicleCategoryMedia:
                        //nombre de m�dia (vehicle)
                        foreach (DataRow curRow in dtTotal.Rows)
                        {
                            if (dtTotal.Columns.Contains("id_vehicle") && (int.Parse(OldIdVehicle) != int.Parse(curRow["id_vehicle"].ToString())))
                            {
                                nbVehicle++;
                            }
                            //nombre de cat�gories
                            if (dtTotal.Columns.Contains("id_category") && !OldIdCategoryArr.Contains(curRow["id_category"].ToString()))
                            {
                                nbCategory++;
                                OldIdCategoryArr.Add(curRow["id_category"].ToString());
                            }
                            //nombre de supports
                            if (dtTotal.Columns.Contains("id_media") && !OldIdMediaArr.Contains(curRow["id_media"].ToString()))
                            {
                                nbMedia++;
                                OldIdMediaArr.Add(curRow["id_media"].ToString());
                            }
                            OldIdVehicle = curRow["id_vehicle"].ToString();
                        }
                        if (dtAdvertiser != null && dtAdvertiser.Rows.Count > 0)
                        {
                            //nombre d'annonceurs
                            foreach (DataRow currRow in dtAdvertiser.Rows)
                            {
                                if (dtAdvertiser.Columns.Contains("id_advertiser") && !OldIdAdvertiserArr.Contains(currRow["id_advertiser"].ToString()))
                                {
                                    nbAdvertiser++;
                                    OldIdAdvertiserArr.Add(currRow["id_advertiser"].ToString());
                                }
                            }
                        }
                        //calcule du nombre maximal de lignes
                        if (FctUtilities.CheckedText.IsNotEmpty(VehicleAccessList) && CstDbClassif.Vehicles.names.plurimedia == (CstDbClassif.Vehicles.names)int.Parse(VehicleAccessList))
                            nbMaxLines = 2 + nbAdvertiser + (2 + nbAdvertiser + (2 + nbAdvertiser + (2 + nbAdvertiser) * nbMedia) * nbCategory) * nbVehicle;
                        else nbMaxLines = 2 + nbAdvertiser + (2 + nbAdvertiser + (2 + nbAdvertiser) * nbMedia) * nbCategory;
                        break;
                    default:
                        throw (new ProductClassIndicatorsException(GestionWeb.GetWebWord(1237, _session.SiteLanguage)));
                }
                OldIdVehicle = "0";
            }
            //cr�ation du tableau 
            tab = new object[nbMaxLines, NB_MAX_COLUMNS];
            #endregion

            return tab;
        }
        #endregion

        #region Niveau M�dia s�lectionn�

        #region D�termine le niveau support s�lectionn�
        /// <summary>
        /// D�termine le niveau M�dia (M�dia ou cat�gorie ou support)
        /// </summary>
        /// <param name="webSession">session client</param>		
        /// <returns>niveau m�dia</returns>
        protected CstResult.MediaStrategy.MediaLevel SwitchMedia()
        {
            switch (_session.PreformatedMediaDetail)
            {
                case CstPreformatedDetail.PreformatedMediaDetails.vehicleCategoryMedia:
                    return CstResult.MediaStrategy.MediaLevel.mediaLevel;
                case CstPreformatedDetail.PreformatedMediaDetails.vehicleCategory:
                    return CstResult.MediaStrategy.MediaLevel.categoryLevel;
                case CstPreformatedDetail.PreformatedMediaDetails.vehicle:
                    return CstResult.MediaStrategy.MediaLevel.vehicleLevel;
                default:
                    throw (new ProductClassIndicatorsException(GestionWeb.GetWebWord(1237, _session.SiteLanguage)));
            }
        }
        /// <summary>
        /// Un traitement peut �tre affectu� si s�lection m�dia est au niveau cat�gorie
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <returns>vrai si niveau cat�gorie</returns>
        protected bool TreatCategory()
        {
            switch (_session.PreformatedMediaDetail)
            {
                case CstPreformatedDetail.PreformatedMediaDetails.vehicleCategoryMedia:
                case CstPreformatedDetail.PreformatedMediaDetails.vehicleCategory:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// Un traitement peut �tre affectu� si on la session contient le niveau support
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <returns>vrai si niveau cat�gorie</returns>
        protected bool TreatMedia()
        {
            switch (_session.PreformatedMediaDetail)
            {
                case CstPreformatedDetail.PreformatedMediaDetails.vehicleCategoryMedia:
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #endregion

        #endregion
    
    }

    #region S�lection Univers Recap
    /// <summary>
    /// Univers s�lectionn� m�dia et produit s�lectionn� par le client dans l'annalyse sectorielle.
    /// </summary>
    public class RecapUniversSelection
    {
        /// <summary>
        /// Session client
        /// </summary>
        private WebSession _webSession;
        /// <summary>
        /// Annonceurs en acc�s
        /// </summary>
        private string _advertiserAccessList = "";
        /// <summary>
        /// Annonceurs en concurrents
        /// </summary>
        private string _competitorAdvertiserAccessList = "";
        /// <summary>
        /// Vari�t�s en acc�s
        /// </summary>
        string _segmentAccessList = "";
        /// <summary>
        /// Vari�t�s en exception
        /// </summary>
        private string _segmentExceptionList = "";
        /// <summary>
        /// Groupes en acc�s
        /// </summary>
        private string _groupAccessList = "";
        /// <summary>
        /// Groupes en exception
        /// </summary>
        private string _groupExceptionList = "";
        /// <summary>
        /// Cat�gories en acc�s
        /// </summary>
        private string _categoryAccessList = "";
        /// <summary>
        /// Supports en acc�s
        /// </summary>
        private string _mediaAccessList = "";
        /// <summary>
        /// M�dias en acc�s
        /// </summary>
        private string _vehicleAccessList = "";

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="webSession">Session client</param>
        public RecapUniversSelection(WebSession webSession)
        {

            _webSession = webSession;
            List<NomenclatureElementsGroup> nElmtGr = null;
            NomenclatureElementsGroup nomenclatureElementsGroup = null;
            string tempListAsString = "";

            if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(0))
            {
                nomenclatureElementsGroup = webSession.SecondaryProductUniverses[0].GetGroup(0);
                if (nomenclatureElementsGroup != null && nomenclatureElementsGroup.Count() > 0)
                {
                    tempListAsString = nomenclatureElementsGroup.GetAsString(TNSClassificationLevels.ADVERTISER);
                    if (tempListAsString != null && tempListAsString.Length > 0) _advertiserAccessList = tempListAsString;
                }
                nomenclatureElementsGroup = null;
                tempListAsString = "";
            }
            //_advertiserAccessList = _webSession.GetSelection(_webSession.ReferenceUniversAdvertiser,CustomerRightConstante.type.advertiserAccess);
            if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(1))
            {
                nomenclatureElementsGroup = webSession.SecondaryProductUniverses[1].GetGroup(0);
                if (nomenclatureElementsGroup != null && nomenclatureElementsGroup.Count() > 0)
                {
                    tempListAsString = nomenclatureElementsGroup.GetAsString(TNSClassificationLevels.ADVERTISER);
                    if (tempListAsString != null && tempListAsString.Length > 0) _competitorAdvertiserAccessList = tempListAsString;
                }
                tempListAsString = "";
            }
            //if(_webSession.CompetitorUniversAdvertiser[0]!=null)
            //    _competitorAdvertiserAccessList = _webSession.GetSelection((TreeNode)_webSession.CompetitorUniversAdvertiser[0],CustomerRightConstante.type.advertiserAccess);		
            if (FctUtilities.CheckedText.IsNotEmpty(_competitorAdvertiserAccessList))
            {
                if (FctUtilities.CheckedText.IsNotEmpty(AdvertiserAccessList)) _advertiserAccessList += "," + CompetitorAdvertiserAccessList;
                else _advertiserAccessList = _competitorAdvertiserAccessList;
            }


            if (webSession.PrincipalProductUniverses.Count > 0)
            {
                //Recuperation des �l�ments s�lectionn�s en inclusion
                nElmtGr = webSession.PrincipalProductUniverses[0].GetIncludes();
                if (nElmtGr != null && nElmtGr.Count > 0)
                {
                    tempListAsString = nElmtGr[0].GetAsString(TNSClassificationLevels.GROUP_);
                    if (tempListAsString != null && tempListAsString.Length > 0) _groupAccessList = tempListAsString;
                    tempListAsString = nElmtGr[0].GetAsString(TNSClassificationLevels.SEGMENT);
                    if (tempListAsString != null && tempListAsString.Length > 0) _segmentAccessList = tempListAsString;
                }
                //Recuperation des �l�ments s�lectionn�s exclusion
                nElmtGr = webSession.PrincipalProductUniverses[0].GetExludes();
                if (nElmtGr != null && nElmtGr.Count > 0)
                {
                    tempListAsString = nElmtGr[0].GetAsString(TNSClassificationLevels.GROUP_);
                    if (tempListAsString != null && tempListAsString.Length > 0) _groupExceptionList = tempListAsString;
                    tempListAsString = nElmtGr[0].GetAsString(TNSClassificationLevels.SEGMENT);
                    if (tempListAsString != null && tempListAsString.Length > 0) _segmentExceptionList = tempListAsString;
                }
            }

            _categoryAccessList = _webSession.GetSelection(_webSession.CurrentUniversMedia, CstRight.type.categoryAccess);
            _mediaAccessList = _webSession.GetSelection(_webSession.CurrentUniversMedia, CstRight.type.mediaAccess);
            _vehicleAccessList = ((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
        }

        /// <summary>
        /// Annonceurs en acc�s
        /// </summary>
        internal string AdvertiserAccessList
        {
            get { return _advertiserAccessList; }
        }

        /// <summary>
        /// Annonceurs  concurrents
        /// </summary>
        internal string CompetitorAdvertiserAccessList
        {
            get { return _competitorAdvertiserAccessList; }
        }

        /// <summary>
        /// Vari�t�s en acc�s
        /// </summary>
        internal string SegmentAccessList
        {
            get { return _segmentAccessList; }
        }
        /// <summary>
        /// Vari�t�s en exception
        /// </summary>
        internal string SegmentExceptionList
        {
            get { return _segmentExceptionList; }
        }
        /// <summary>
        /// groupes en acc�s
        /// </summary>
        internal string GroupAccessList
        {
            get { return _groupAccessList; }
        }
        /// <summary>
        /// Groupes en exception
        /// </summary>
        internal string GroupExceptionList
        {
            get { return _groupExceptionList; }
        }
        /// <summary>
        /// Cat�gories en acc�s
        /// </summary>
        internal string CategoryAccessList
        {
            get { return _categoryAccessList; }
        }
        /// <summary>
        /// Supports en acc�s
        /// </summary>
        internal string MediaAccessList
        {
            get { return _mediaAccessList; }
        }
        /// <summary>
        /// M�dias en acc�s
        /// </summary>
        internal string VehicleAccessList
        {
            get { return _vehicleAccessList; }
        }


    }
    #endregion

}