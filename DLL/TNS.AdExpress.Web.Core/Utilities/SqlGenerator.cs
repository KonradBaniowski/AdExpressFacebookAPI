#region Informations
// Auteur: G. Facon 
// Date de cr�ation: 17/05/2004 
// Date de modification: 
//		29/10/2004  (G. Facon ajout univers)
//		27/04/2005  (K. Shehzad additions for "produits d�taill�s par")
//      Modifications for Outdoor K.Shehzad
/*
 *      22/07/2008 - G Ragneau - Move from TNS.AdExpress.Web.Core.Utilities
 * 
 *  
 */

#endregion

using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

using TNS.AdExpress.Web.Core.Sessions;
using CustomerRightConstante = TNS.AdExpress.Constantes.Customer.Right;
using CstPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using CstCustomerSessions = TNS.AdExpress.Constantes.Web.CustomerSessions;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using ClassificationConstantes = TNS.AdExpress.Constantes.Classification;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.DB.Common;
using FWKConstantes = TNS.AdExpress.Constantes.FrameWork;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Exceptions;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Classification;
using WebNavigation = TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain;

namespace TNS.AdExpress.Web.Core.Utilities
{
    /// <summary>
    /// G�n�rateur de code SQL
    /// </summary>
    public partial class SQLGenerator {

        #region In Clause Method
        /// <summary>
        /// In Clause Method
        /// </summary>
        /// <param name="session">User session (useless)</param>
        /// <param name="label">Field</param>
        /// <param name="inClauseItems">Items
        /// <example>"3,9,6"</example>
        /// </param>
        /// <returns>In clause SQL code</returns>
        [Obsolete("No need for user session")]
        public static string GetInClauseMagicMethod(WebSession session, string label, string inClauseItems)
        {
            return GetInClauseMagicMethod(label, inClauseItems, true);
        }
        /// <summary>
        /// In Clause Method
        /// </summary>
        /// <param name="label">Field</param>
        /// <param name="inClauseItems">Items
        /// <example>"3,9,6"</example>
        /// </param>
        /// <returns>In clause SQL code</returns>
        public static string GetInClauseMagicMethod(string label, string inClauseItems)
        {
            return GetInClauseMagicMethod(label, inClauseItems, true);
        }
        /// <summary>
        /// In Clause Method
        /// </summary>
        /// <param name="label">Field</param>
        /// <param name="inClauseItems">Items
        /// <example>"3,9,6"</example>
        /// </param>
        /// <param name="include">True if elements are included (in), false either (not in)</param>
        /// <returns>In clause SQL code</returns>
        public static string GetInClauseMagicMethod(string label, string inClauseItems, bool include)
        {

            string str = string.Empty;
            if (inClauseItems.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                string[] strs = inClauseItems.Split(',');
                int i = 0;
                sb.Append("(");
                while (i < strs.Length)
                {
                    if (i > 0)
                    {
                        sb.Append((include)?" or ":" and ");
                    }
                    sb.AppendFormat(" {1} {2} ({0}) ", String.Join(",", strs, i, Math.Min(strs.Length - i, 500)), label, (include)?" in ":" not in ");
                    i += 500;
                }
                sb.Append(")");
                return sb.ToString();
            }

            return str;
        }
        #endregion

        #region Droits client

        #region Supports

        /// <summary>
        /// G�n�re les droits clients M�dia pour les plan media AdNetTrack.
        /// Cette fonction est � utiliser si une m�me table contient tous les identifiants de la nomenclature support.
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="tablePrefixe">Pr�fixe de la table qui contient les donn�es</param>
        /// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string GetAdNetTrackMediaRight(WebSession webSession, string tablePrefixe, bool beginByAnd)
        {

            string sql = "";
            bool premier = true;
            // le bloc doit il commencer par AND
            // Vehicle
            //			if(webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess).Length>0){
            //				if(beginByAnd) sql+=" and";
            //				sql+=" (("+tablePrefixe+".id_vehicle in ("+webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess)+") ";
            //				premier=false;
            //			}
            if (beginByAnd) sql += " and";
            sql += " ((" + tablePrefixe + ".id_vehicle in (" + VehiclesInformation.EnumToDatabaseId(DBClassificationConstantes.Vehicles.names.adnettrack).ToString() + " )";
            premier = false;

            // Category
            if (webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length > 0)
            {
                if (!premier) sql += " or";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " ((";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(tablePrefixe + ".id_category", webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess], true) + " ";
                premier = false;
            }
            // Region
            if (webSession.CustomerLogin[CustomerRightConstante.type.regionAccess].Length > 0)
            {
                if (!premier) sql += " or";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " ((";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(tablePrefixe + ".id_region", webSession.CustomerLogin[CustomerRightConstante.type.regionAccess], true) + " ";
                premier = false;
            }
            // Media
            if (webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess].Length > 0)
            {
                if (!premier) sql += " or";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " ((";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(tablePrefixe + ".id_media", webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess], true) + " ";
                premier = false;
            }
            if (!premier) sql += " )";

            // Droits en exclusion
            // Vehicle
            //			if(webSession.CustomerLogin[CustomerRightConstante.type.vehicleException).Length>0){
            //				if(!premier) sql+=" and";
            //				else{
            //					if(beginByAnd) sql+=" and";
            //					sql+=" (";
            //				}
            //				sql+=" "+tablePrefixe+".id_vehicle not in ("+webSession.CustomerLogin[CustomerRightConstante.type.vehicleException)+") ";
            //				premier=false;
            //			}
            // Category
            if (webSession.CustomerLogin[CustomerRightConstante.type.categoryException].Length > 0)
            {
                if (!premier) sql += " and";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " (";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(tablePrefixe + ".id_category", webSession.CustomerLogin[CustomerRightConstante.type.categoryException], false) + " ";
                premier = false;
            }
            // region
            if (webSession.CustomerLogin[CustomerRightConstante.type.regionException].Length > 0)
            {
                if (!premier) sql += " and";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " (";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(tablePrefixe + ".id_region", webSession.CustomerLogin[CustomerRightConstante.type.regionException], false) + " ";
                premier = false;
            }
            // Media
            if (webSession.CustomerLogin[CustomerRightConstante.type.mediaException].Length > 0)
            {
                if (!premier) sql += " and";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " (";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(tablePrefixe + ".id_media", webSession.CustomerLogin[CustomerRightConstante.type.mediaException], false) + " ";
                premier = false;
            }
            if (!premier) sql += " )";
            return (sql);

        }

        /// <summary>
        /// G�n�re les droits clients M�dia.
        /// Cette fonction est � utiliser si une m�me table contient tous les identifiants de la nomenclature support.
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="tablePrefixe">Pr�fixe de la table qui contient les donn�es</param>
        /// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
        /// <param name="replaceIds">Function  replace ids</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string getAnalyseCustomerMediaRight(WebSession webSession, string tablePrefixe
            , bool beginByAnd,Func<CustomerRightConstante.type ,string,string>  replaceIds =null)
        {

            string sql = "";
            bool premier = true;
            // le bloc doit il commencer par AND
            // Vehicle
            if (webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess].Length > 0)
            {
                if (beginByAnd) sql += " and";
                
                sql += replaceIds != null
                           ? string.Format(" (( " + SQLGenerator.GetInClauseMagicMethod(tablePrefixe + ".id_vehicle"
                                                                                        , replaceIds(CustomerRightConstante.type.vehicleAccess, webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess])
                                                                                        , true) + " ")
                           : string.Format(" ((" + SQLGenerator.GetInClauseMagicMethod(tablePrefixe + ".id_vehicle"
                                                                                        ,webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess]
                                                                                        , true) + " ");
                premier = false;
            }
            // Category
            if (webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length > 0)
            {
                if (!premier) sql += " or";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " ((";
                }
                
                sql += replaceIds != null
                           ? string.Format(" " + SQLGenerator.GetInClauseMagicMethod(tablePrefixe + ".id_category"
                                                    , replaceIds(CustomerRightConstante.type.categoryAccess, webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess])
                                                    ,true) + " ")
                           : string.Format(" " + SQLGenerator.GetInClauseMagicMethod(tablePrefixe + ".id_category"
                                                    , webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess]
                                                    , true) + " ");
                premier = false;
            }
            // Region
            if (webSession.CustomerLogin[CustomerRightConstante.type.regionAccess].Length > 0)
            {
                if (!premier) sql += " or";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " ((";
                }
                
                sql += replaceIds != null
                          ? string.Format(" " + SQLGenerator.GetInClauseMagicMethod(tablePrefixe + ".id_region"
                                                    , replaceIds(CustomerRightConstante.type.regionAccess, webSession.CustomerLogin[CustomerRightConstante.type.regionAccess])
                                                    , true) + " ")
                          : string.Format(" " + SQLGenerator.GetInClauseMagicMethod(tablePrefixe + ".id_region"
                                                    , webSession.CustomerLogin[CustomerRightConstante.type.regionAccess]
                                                    ,true) + " ");
                premier = false;
            }
            // Media
            if (webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess].Length > 0)
            {
                if (!premier) sql += " or";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " ((";
                }
                
                sql += replaceIds != null
                          ? string.Format(" " + SQLGenerator.GetInClauseMagicMethod(tablePrefixe + ".id_media"
                                                    , replaceIds(CustomerRightConstante.type.mediaAccess, webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess])
                                                    , true) + " ")
                          : string.Format(" " + SQLGenerator.GetInClauseMagicMethod(tablePrefixe + ".id_media"
                                                    , webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess]
                                                    ,true) + " ");
                premier = false;
            }
            if (!premier) sql += " )";

            // Droits en exclusion
            // Vehicle
            if (webSession.CustomerLogin[CustomerRightConstante.type.vehicleException].Length > 0)
            {
                if (!premier) sql += " and";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " (";
                }
                
                sql += replaceIds != null
                           ? string.Format(" " + SQLGenerator.GetInClauseMagicMethod(tablePrefixe + ".id_vehicle"
                                                    , replaceIds(CustomerRightConstante.type.vehicleException, webSession.CustomerLogin[ CustomerRightConstante.type.vehicleException])
                                                    ,false) + " ")
                           : string.Format(" " + SQLGenerator.GetInClauseMagicMethod(tablePrefixe + ".id_vehicle"
                                                    , webSession.CustomerLogin[CustomerRightConstante.type.vehicleException]
                                                    ,false) + " ");
                premier = false;
            }
            // Category
            if (webSession.CustomerLogin[CustomerRightConstante.type.categoryException].Length > 0)
            {
                if (!premier) sql += " and";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " (";
                }
                
                sql += replaceIds != null
                          ? string.Format(" " + SQLGenerator.GetInClauseMagicMethod(tablePrefixe + ".id_category"
                                                    , replaceIds(CustomerRightConstante.type.categoryException, webSession.CustomerLogin[ CustomerRightConstante.type.categoryException])
                                                    , false) + " ")
                          : string.Format(" " + SQLGenerator.GetInClauseMagicMethod(tablePrefixe + ".id_category"
                                                    , webSession.CustomerLogin[CustomerRightConstante.type.categoryException]
                                                    ,false) + " ");
                premier = false;
            }
            // Region
            if (webSession.CustomerLogin[CustomerRightConstante.type.regionException].Length > 0)
            {
                if (!premier) sql += " and";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " (";
                }
                
                sql += replaceIds != null
                         ? string.Format(" " + SQLGenerator.GetInClauseMagicMethod(tablePrefixe + ".id_region"
                                                , replaceIds(CustomerRightConstante.type.regionException, webSession.CustomerLogin[CustomerRightConstante.type.regionException])
                                                , false) + " ")
                         : string.Format(" " + SQLGenerator.GetInClauseMagicMethod(tablePrefixe + ".id_region"
                                                , webSession.CustomerLogin[CustomerRightConstante.type.regionException]
                                                , false) + " ");
               
                premier = false;
            }
            // Media
            if (webSession.CustomerLogin[CustomerRightConstante.type.mediaException].Length > 0)
            {
                if (!premier) sql += " and";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " (";
                }
                
                sql += replaceIds != null
                        ? string.Format(" " + SQLGenerator.GetInClauseMagicMethod(tablePrefixe + ".id_media"
                                                , replaceIds(CustomerRightConstante.type.mediaException, webSession.CustomerLogin[CustomerRightConstante.type.mediaException])
                                                , false) + " ")
                        : string.Format(" " + SQLGenerator.GetInClauseMagicMethod(tablePrefixe + ".id_media"
                                                , webSession.CustomerLogin[CustomerRightConstante.type.mediaException]
                                                , false) + " ");  
                premier = false;
            }
            if (!premier) sql += " )";
            return (sql);

        }


        #endregion

        #region Produits

        /// <summary>
        /// G�n�re les droits clients Produit.
        /// Cette fonction est � utiliser si une m�me table contient tous les identifiants de la nomenclature produit.
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="tablePrefixe">Pr�fixe de la table qui contient les donn�es</param>
        /// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string getAnalyseCustomerProductRight(WebSession webSession, string tablePrefixe, bool beginByAnd)
        {
            return (getClassificationCustomerProductRight(webSession, tablePrefixe, tablePrefixe, tablePrefixe, tablePrefixe, beginByAnd));
        }

        /// <summary>
        /// G�n�re les droits clients Produit.
        /// Cette fonction est � utiliser si la nomenclature est � plat dans la requ�te.
        /// Les noms des tables sont:
        ///    - sector: sc
        ///    - subsector: ss
        ///    - group_:gr
        ///    - segment: sg
        /// Les pr�fixes sont d�finis dans TNS.AdExpress.Constantes.DB.Tables
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string getClassificationCustomerProductRight(WebSession webSession, bool beginByAnd)
        {
            return (getClassificationCustomerProductRight(webSession, DBConstantes.Tables.SECTOR_PREFIXE, DBConstantes.Tables.SUBSECTOR_PREFIXE, DBConstantes.Tables.GROUP_PREFIXE, DBConstantes.Tables.SEGMENT_PREFIXE, beginByAnd));
        }


        /// <summary>
        /// G�n�re les droits clients Produit.
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="sectorPrefixe">Pr�fixe de la table sector</param>
        /// <param name="subsectorPrefixe">Pr�fixe de la table subsector</param>
        /// <param name="groupPrefixe">Pr�fixe de la table group_</param>
        /// <param name="segmentPrefixe">Pr�fixe de la table segment</param>
        /// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string getClassificationCustomerProductRight(WebSession webSession, string sectorPrefixe, string subsectorPrefixe, string groupPrefixe, string segmentPrefixe, bool beginByAnd)
        {
			string sql = "";
			bool premier = true;
			// Sector (Famille)
			if (webSession.CustomerLogin[CustomerRightConstante.type.sectorAccess].Length > 0) {
				if (beginByAnd) sql += " and";
				sql += " ((" + GetInClauseMagicMethod(sectorPrefixe + ".id_sector", webSession.CustomerLogin[CustomerRightConstante.type.sectorAccess], true);
				premier = false;
			}
			// SubSector (Classe)
			if (webSession.CustomerLogin[CustomerRightConstante.type.subSectorAccess].Length > 0) {
				if (!premier) sql += " or";
				else {
					if (beginByAnd) sql += " and";
					sql += " ((";
				}
				sql += "  " + GetInClauseMagicMethod(subsectorPrefixe + ".id_subsector", webSession.CustomerLogin[CustomerRightConstante.type.subSectorAccess], true);
				premier = false;
			}
			// Group (Groupe)
			if (webSession.CustomerLogin[CustomerRightConstante.type.groupAccess].Length > 0) {
				if (!premier) sql += " or";
				else {
					if (beginByAnd) sql += " and";
					sql += " ((";
				}
				sql += "  " + GetInClauseMagicMethod(groupPrefixe + ".id_group_", webSession.CustomerLogin[CustomerRightConstante.type.groupAccess], true);
				premier = false;
			}
			// Segment (Vari�t�)
			if (webSession.CustomerLogin[CustomerRightConstante.type.segmentAccess].Length > 0) {
				if (!premier) sql += " or";
				else {
					if (beginByAnd) sql += " and";
					sql += " ((";
				}
				sql += "  " + GetInClauseMagicMethod(segmentPrefixe + ".id_segment", webSession.CustomerLogin[CustomerRightConstante.type.segmentAccess], true);
				premier = false;
			}			
			if (!premier) sql += " )";
			// Droits en exclusion
			// Sector (Famille)
			if (webSession.CustomerLogin[CustomerRightConstante.type.sectorException].Length > 0) {
				if (!premier) sql += " and";
				else {
					if (beginByAnd) sql += " and";
					sql += " (";
				}
				sql += "  " + GetInClauseMagicMethod(sectorPrefixe + ".id_sector", webSession.CustomerLogin[CustomerRightConstante.type.sectorException], false);
				premier = false;
			}
			// SubSector (Classe)
			if (webSession.CustomerLogin[CustomerRightConstante.type.subSectorException].Length > 0) {
				if (!premier) sql += " and";
				else {
					if (beginByAnd) sql += " and";
					sql += " (";
				}
				sql += "  " + GetInClauseMagicMethod(subsectorPrefixe + ".id_subsector", webSession.CustomerLogin[CustomerRightConstante.type.subSectorException], false);
				premier = false;
			}
			// Group (Groupe)
			if (webSession.CustomerLogin[CustomerRightConstante.type.groupException].Length > 0) {
				if (!premier) sql += " and";
				else {
					if (beginByAnd) sql += " and";
					sql += " (";
				}
				sql += "  " + GetInClauseMagicMethod(groupPrefixe + ".id_group_", webSession.CustomerLogin[CustomerRightConstante.type.groupException], false);
				premier = false;
			}
			// Segment (Vari�t�)
			if (webSession.CustomerLogin[CustomerRightConstante.type.segmentException].Length > 0) {
				if (!premier) sql += " and";
				else {
					if (beginByAnd) sql += " and";
					sql += " (";
				}
				sql += "  " + GetInClauseMagicMethod(segmentPrefixe + ".id_segment", webSession.CustomerLogin[CustomerRightConstante.type.segmentException], false);
				premier = false;
			}			
			if (!premier) sql += " )";
			return (sql);
        }

        /// <summary>
        /// G�n�re les droits clients Produit ( familles ) .
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="sectorPrefixe">Pr�fixe de la table sector</param>		
        /// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string getClassificationCustomerProductRight(WebSession webSession, string sectorPrefixe, bool beginByAnd)
        {
            string sql = "";
            bool premier = true;
			// Sector (Famille)
			if (webSession.CustomerLogin[CustomerRightConstante.type.sectorAccess].Length > 0) {
				if (beginByAnd) sql += " and";
				sql += " ((" + GetInClauseMagicMethod(sectorPrefixe + ".id_sector", webSession.CustomerLogin[CustomerRightConstante.type.sectorAccess], true);
				premier = false;
			}
            if (!premier) sql += " )";
            // Droits en exclusion           
			// Sector (Famille)
			if (webSession.CustomerLogin[CustomerRightConstante.type.sectorException].Length > 0) {
				if (!premier) sql += " and";
				else {
					if (beginByAnd) sql += " and";
					sql += " (";
				}
				sql += "  " + GetInClauseMagicMethod(sectorPrefixe + ".id_sector", webSession.CustomerLogin[CustomerRightConstante.type.sectorException], false);
				premier = false;
			}
            if (!premier) sql += " )";
            return (sql);
		}


        /// <summary>
        /// G�n�re les droits clients Produit.
        /// Cette fonction est � utiliser si une m�me table contient tous les identifiants de la nomenclature produit.
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="tablePrefixe">Pr�fixe de la table qui contient les donn�es</param>
        /// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string GetClassificationCustomerProductRight(WebSession webSession, string tablePrefixe, bool beginByAnd, string productRightBranches)
        {
            return (GetClassificationCustomerProductRight(webSession, tablePrefixe, tablePrefixe, tablePrefixe, tablePrefixe, tablePrefixe,beginByAnd,productRightBranches));
        }

		

		/// <summary>
		/// G�n�re les droits clients Produit dont les droits annonceurs.
		/// </summary>
		/// <remarks>Appliqu� � ce jour seulement pour les Recap.</remarks>
		/// <param name="webSession">Session du client</param>
		/// <param name="sectorPrefixe">Pr�fixe de la table sector</param>
		/// <param name="subsectorPrefixe">Pr�fixe de la table subsector</param>
		/// <param name="groupPrefixe">Pr�fixe de la table group_</param>
		/// <param name="segmentPrefixe">Pr�fixe de la table segment</param>
		/// <param name="advertiserPrefixe">Pr�fixe de la table annonceur</param>
		/// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
		/// <returns>Code SQL g�n�r�</returns>
        public static string GetClassificationCustomerProductRight(WebSession webSession, string sectorPrefixe, string subsectorPrefixe, string groupPrefixe, string segmentPrefixe, string advertiserPrefixe, bool beginByAnd, string productRightBranches)
        {
			string sql = "";
			bool premier = true;

            if (!string.IsNullOrEmpty(productRightBranches) && productRightBranches.IndexOf("product") >= 0)
            {

                // Sector (Famille)
                if (webSession.CustomerLogin[CustomerRightConstante.type.sectorAccess].Length > 0)
                {
                    if (beginByAnd) sql += " and";
                    sql += " ((" + GetInClauseMagicMethod(sectorPrefixe + ".id_sector", webSession.CustomerLogin[CustomerRightConstante.type.sectorAccess], true);
                    premier = false;
                }
                // SubSector (Classe)
                if (webSession.CustomerLogin[CustomerRightConstante.type.subSectorAccess].Length > 0)
                {
                    if (!premier) sql += " or";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " ((";
                    }
                    sql += "  " + GetInClauseMagicMethod(subsectorPrefixe + ".id_subsector", webSession.CustomerLogin[CustomerRightConstante.type.subSectorAccess], true);
                    premier = false;
                }
                // Group (Groupe)
                if (webSession.CustomerLogin[CustomerRightConstante.type.groupAccess].Length > 0)
                {
                    if (!premier) sql += " or";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " ((";
                    }
                    sql += "  " + GetInClauseMagicMethod(groupPrefixe + ".id_group_", webSession.CustomerLogin[CustomerRightConstante.type.groupAccess], true);
                    premier = false;
                }
                // Segment (Vari�t�)
                if (webSession.CustomerLogin[CustomerRightConstante.type.segmentAccess].Length > 0)
                {
                    if (!premier) sql += " or";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " ((";
                    }
                    sql += "  " + GetInClauseMagicMethod(segmentPrefixe + ".id_segment", webSession.CustomerLogin[CustomerRightConstante.type.segmentAccess], true);
                    premier = false;
                }

            }

            if (!string.IsNullOrEmpty(productRightBranches) && productRightBranches.IndexOf("advertiser") >= 0)
            {
                // Advertiser (Annonceur)
                if (webSession.CustomerLogin[CustomerRightConstante.type.advertiserAccess].Length > 0)
                {
                    if (!premier) sql += " or";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " ((";
                    }
                    sql += "  " + GetInClauseMagicMethod(advertiserPrefixe + ".id_advertiser", webSession.CustomerLogin[CustomerRightConstante.type.advertiserAccess], true);
                    premier = false;
                }
            }

			if (!premier) sql += " )";


			// Droits en exclusion

            if (!string.IsNullOrEmpty(productRightBranches) && productRightBranches.IndexOf("product") >= 0)
            {
                // Sector (Famille)
                if (webSession.CustomerLogin[CustomerRightConstante.type.sectorException].Length > 0)
                {
                    if (!premier) sql += " and";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " (";
                    }
                    sql += "  " + GetInClauseMagicMethod(sectorPrefixe + ".id_sector", webSession.CustomerLogin[CustomerRightConstante.type.sectorException], false);
                    premier = false;
                }
                // SubSector (Classe)
                if (webSession.CustomerLogin[CustomerRightConstante.type.subSectorException].Length > 0)
                {
                    if (!premier) sql += " and";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " (";
                    }
                    sql += "  " + GetInClauseMagicMethod(subsectorPrefixe + ".id_subsector", webSession.CustomerLogin[CustomerRightConstante.type.subSectorException], false);
                    premier = false;
                }
                // Group (Groupe)
                if (webSession.CustomerLogin[CustomerRightConstante.type.groupException].Length > 0)
                {
                    if (!premier) sql += " and";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " (";
                    }
                    sql += "  " + GetInClauseMagicMethod(groupPrefixe + ".id_group_", webSession.CustomerLogin[CustomerRightConstante.type.groupException], false);
                    premier = false;
                }
                // Segment (Vari�t�)
                if (webSession.CustomerLogin[CustomerRightConstante.type.segmentException].Length > 0)
                {
                    if (!premier) sql += " and";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " (";
                    }
                    sql += "  " + GetInClauseMagicMethod(segmentPrefixe + ".id_segment", webSession.CustomerLogin[CustomerRightConstante.type.segmentException], false);
                    premier = false;
                }
            }

            if (!string.IsNullOrEmpty(productRightBranches) && productRightBranches.IndexOf("advertiser") >= 0)
            {
                // Advertiser (Annonceur)
                if (webSession.CustomerLogin[CustomerRightConstante.type.advertiserException].Length > 0)
                {
                    if (!premier) sql += " and";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " (";
                    }
                    sql += "  " + GetInClauseMagicMethod(advertiserPrefixe + ".id_advertiser", webSession.CustomerLogin[CustomerRightConstante.type.advertiserException], false);
                    premier = false;
                }
            }
			if (!premier) sql += " )";
			return (sql);
		}

		/// <summary>
		/// G�n�re les droits clients annonceurs.
		/// </summary>
		/// <remarks>Appliqu� � ce jour seulement pour les Recap.</remarks>
		/// <param name="webSession">Session du client</param>
		/// <param name="advertiserPrefixe">Pr�fixe de la table annonceur</param>		
		/// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
		/// <returns>Code SQL g�n�r�</returns>
		public static string GetClassificationCustomerAdvertiserRight(WebSession webSession, string advertiserPrefixe, bool beginByAnd) {
			string sql = "";
			bool premier = true;
			// Advertiser (Annonceur)
			if (webSession.CustomerLogin[CustomerRightConstante.type.advertiserAccess].Length > 0) {
				if (beginByAnd) sql += " and";
				sql += " ((" + GetInClauseMagicMethod(advertiserPrefixe + ".id_advertiser", webSession.CustomerLogin[CustomerRightConstante.type.advertiserAccess], true);
				premier = false;
			}						
			if (!premier) sql += " )";
			// Droits en exclusion
			//  Advertiser (Annonceur)
			if (webSession.CustomerLogin[CustomerRightConstante.type.advertiserException].Length > 0) {
				if (!premier) sql += " and";
				else {
					if (beginByAnd) sql += " and";
					sql += " (";
				}
				sql += "  " + GetInClauseMagicMethod(advertiserPrefixe + ".id_advertiser", webSession.CustomerLogin[CustomerRightConstante.type.advertiserException], false);
				premier = false;
			}		
			if (!premier) sql += " )";
			return (sql);
		}
		


        #region GetProductRights

        /// <summary>
        /// Get product classification branch rights
        /// </summary>
        /// <param name="prefix">prefix</param> 
        /// <param name="beginByAnd">True if sql clause start with "AND"</param>
        /// <param name="session">Client session</param>
        /// <returns>string sql</returns>
        public static string GetProductRights(WebSession session, string prefix, bool beginByAnd)
        {
            return GetProductRights(session, prefix, prefix, prefix, prefix, beginByAnd);
        }

        /// <summary>
        /// Get vehicle classification branch rights
        /// </summary>
        /// <param name="sectorTable">Sector Table description</param>
        /// <param name="subSectorTable">Sub sector Table description</param>
        /// <param name="groupTable">group Table description</param>
        /// <param name="segmentTable">Segement Table description</param>
        /// <param name="session">Client session</param>
        /// <param name="beginByAnd">True if sql clause start with "AND"</param>
        /// <returns>sql rights string</returns>
        public static string GetProductRights(WebSession session, Table sectorTable, Table subSectorTable, Table groupTable, Table segmentTable, bool beginByAnd)
        {
            return GetProductRights(session, sectorTable.Prefix, subSectorTable.Prefix, groupTable.Prefix, segmentTable.Prefix, beginByAnd);
        }

        /// <summary>
        /// Get vehicle classification branch rights.
        /// </summary>
        /// <param name="sectorTablePrefix">sector Table Prefix</param>
        /// <param name="subSectorTablePrefix">sub sector Table Prefix</param>
        /// <param name="groupTablePrefix">group Table Prefix</param>
        /// <param name="segmentTablePrefix">segment Table Prefix</param>
        /// <param name="session">Client session</param>
        /// <param name="beginByAnd">True if sql clause start with "AND"</param>
        /// <returns>string sql</returns>
        public static string GetProductRights(WebSession session, string sectorTablePrefix, string subSectorTablePrefix, string groupTablePrefix,string segmentTablePrefix, bool beginByAnd)
        {
            string sql = "";

            bool fisrt = true;
            //Get product rights filter data
            Dictionary<TNS.AdExpress.Constantes.Customer.Right.type, string> rights = session.CustomerDataFilters.ProductsRights;
            string prefix = string.Empty;

            if (rights != null)
            {

                // Get the sector authorized for the current customer
                if (rights.ContainsKey(CustomerRightConstante.type.sectorAccess) && rights[CustomerRightConstante.type.sectorAccess].Length > 0)
                {
                    if (beginByAnd) sql += " and";
                    prefix = (sectorTablePrefix != null && sectorTablePrefix.Length > 0) ? sectorTablePrefix + "." : "";
                    sql += " ((" + GetInClauseMagicMethod(prefix + "id_sector", rights[CustomerRightConstante.type.sectorAccess], true) + " ";
                    fisrt = false;
                }
                // Get the sub sector authorized for the current customer
                if (rights.ContainsKey(CustomerRightConstante.type.subSectorAccess) && rights[CustomerRightConstante.type.subSectorAccess].Length > 0)
                {
                    if (!fisrt) sql += " or";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " ((";
                    }
                    prefix = (subSectorTablePrefix != null && subSectorTablePrefix.Length > 0) ? subSectorTablePrefix + "." : "";
                    sql += " " +  GetInClauseMagicMethod(prefix + "id_subsector", rights[CustomerRightConstante.type.subSectorAccess], true) + " ";
                    fisrt = false;
                }
                // Get the group authorized for the current customer                
                if (rights.ContainsKey(CustomerRightConstante.type.groupAccess) && rights[CustomerRightConstante.type.groupAccess].Length > 0)
                {
                    if (!fisrt) sql += " or";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " ((";
                    }
                    prefix = (groupTablePrefix != null && groupTablePrefix.Length > 0) ? groupTablePrefix + "." : "";
                    sql += " " + GetInClauseMagicMethod(prefix + "id_group_", rights[CustomerRightConstante.type.groupAccess], true) + " ";
                    fisrt = false;
                }
                // Get the segment authorized for the current customer                
                if (rights.ContainsKey(CustomerRightConstante.type.segmentAccess) && rights[CustomerRightConstante.type.segmentAccess].Length > 0)
                {
                    if (!fisrt) sql += " or";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " ((";
                    }
                    prefix = (segmentTablePrefix != null && segmentTablePrefix.Length > 0) ? segmentTablePrefix + "." : "";
                    sql += " " + GetInClauseMagicMethod(prefix + "id_segment", rights[CustomerRightConstante.type.segmentAccess], true) + " ";
                    fisrt = false;
                }
                if (!fisrt) sql += " )";

                // Get the sector not authorized for the current customer
                if (rights.ContainsKey(CustomerRightConstante.type.sectorException) && rights[CustomerRightConstante.type.sectorException].Length > 0)
                {
                    if (!fisrt) sql += " and";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " (";
                    }
                    prefix = (sectorTablePrefix != null && sectorTablePrefix.Length > 0) ? sectorTablePrefix + "." : "";
                    sql += " " + GetInClauseMagicMethod(prefix + " id_sector", rights[CustomerRightConstante.type.sectorException], false) + " ";
                    fisrt = false;
                }
                // Get the sub sector not authorized for the current customer
                if (rights.ContainsKey(CustomerRightConstante.type.subSectorException) && rights[CustomerRightConstante.type.subSectorException].Length > 0)
                {
                    if (!fisrt) sql += " and";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " (";
                    }
                    prefix = (subSectorTablePrefix != null && subSectorTablePrefix.Length > 0) ? subSectorTablePrefix + "." : "";
                    sql += " " + GetInClauseMagicMethod(prefix + "id_subsector", rights[CustomerRightConstante.type.subSectorException], false) + " ";
                    fisrt = false;
                }
                // Get the group not authorized for the current customer
                if (rights.ContainsKey(CustomerRightConstante.type.groupException) && rights[CustomerRightConstante.type.groupException].Length > 0)
                {
                    if (!fisrt) sql += " and";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " (";
                    }
                    prefix = (groupTablePrefix != null && groupTablePrefix.Length > 0) ? groupTablePrefix + "." : "";
                    sql += " " + GetInClauseMagicMethod(prefix + "id_group_", rights[CustomerRightConstante.type.groupException], false) + " ";
                    fisrt = false;
                }
                // Get the segment not authorized for the current customer
                if (rights.ContainsKey(CustomerRightConstante.type.segmentException) && rights[CustomerRightConstante.type.segmentException].Length > 0)
                {
                    if (!fisrt) sql += " and";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " (";
                    }
                    prefix = (segmentTablePrefix != null && segmentTablePrefix.Length > 0) ? segmentTablePrefix + "." : "";
                    sql += " " + GetInClauseMagicMethod(prefix + "id_segment", rights[CustomerRightConstante.type.segmentException], false) + " ";
                    fisrt = false;
                }
                if (!fisrt) sql += " )";
            }
            return sql;
        }
        #endregion

		#endregion

		#region media ||plurimedia
		/// <summary>
        /// G�n�re les droits clients Produit.
        /// Cette fonction est � utiliser si une m�me table contient tous les identifiants de la nomenclature produit.
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="tablePrefixe">Pr�fixe de la table qui contient les donn�es</param>
        /// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string getClassificationCustomerRecapMediaRight(WebSession webSession, string tablePrefixe, bool beginByAnd)
        {
            return (getClassificationCustomerRecapMediaRight(webSession, tablePrefixe, tablePrefixe, tablePrefixe, beginByAnd));
        }


        /// <summary>
        /// G�n�re les droits clients Media.
        /// Cette fonction est � utiliser si la nomenclature est � plat dans la requ�te.				
        /// Les pr�fixes sont d�finis dans TNS.AdExpress.Constantes.DB.Tables
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string getClassificationCustomerRecapMediaRight(WebSession webSession, bool beginByAnd)
        {
            return (getClassificationCustomerRecapMediaRight(webSession, DBConstantes.Tables.VEHICLE_PREFIXE, DBConstantes.Tables.CATEGORY_PREFIXE, DBConstantes.Tables.MEDIA_PREFIXE, beginByAnd));
        }


        /// <summary>
        /// G�n�re les droits media clients .
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="vehiclePrefixe">Pr�fixe de la table vehicle (media)</param>
        /// <param name="categoryPrefixe">Pr�fixe de la table category</param>
        /// <param name="mediaPrefixe">Pr�fixe de la table media (support)</param>		
        /// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string getClassificationCustomerRecapMediaRight(WebSession webSession, string vehiclePrefixe, string categoryPrefixe, string mediaPrefixe, bool beginByAnd)
        {
            string sql = "";
            bool premier = true;
            // vehicle (media)
            if (webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess].Length > 0)
            {
                if (beginByAnd) sql += " and";
                sql += " ((" + SQLGenerator.GetInClauseMagicMethod(vehiclePrefixe + ".id_vehicle", webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess], true) + " ";
                premier = false;
            }
            // category (Categorie)
            if (webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length > 0)
            {
                if (!premier) sql += " or";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " ((";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(categoryPrefixe + ".id_category", webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess], true) + " ";
                premier = false;
            }
            // media (support)
            if (webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess].Length > 0)
            {
                if (!premier) sql += " or";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " ((";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(mediaPrefixe + ".id_media", webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess], true) + " ";
                premier = false;
            }

            if (!premier) sql += " )";
            // Droits en exclusion
            // vehicle (media)
            if (webSession.CustomerLogin[CustomerRightConstante.type.vehicleException].Length > 0)
            {
                if (!premier) sql += " and";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " (";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(vehiclePrefixe + ".id_vehicle", webSession.CustomerLogin[CustomerRightConstante.type.vehicleException], false) + " ";
                premier = false;
            }
            // category (Categorie)
            if (webSession.CustomerLogin[CustomerRightConstante.type.categoryException].Length > 0)
            {
                if (!premier) sql += " and";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " (";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(categoryPrefixe + ".id_category", webSession.CustomerLogin[CustomerRightConstante.type.categoryException], false) + " ";
                premier = false;
            }
            // media (support)
            if (webSession.CustomerLogin[CustomerRightConstante.type.mediaException].Length > 0)
            {
                if (!premier) sql += " and";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " (";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(mediaPrefixe + ".id_media", webSession.CustomerLogin[CustomerRightConstante.type.mediaException], false) + " ";
                premier = false;
            }

            if (!premier) sql += " )";
            return (sql);
        }

        /// <summary>
        /// G�n�re la condition contenant la liste des identifiants vehicles accessibles
        /// </summary>
        /// <remarks>Un vehicle est accessible si au moins un vehicle une categorie au un support est ouvert</remarks>
        /// <param name="webSession">Session du client</param>
        /// <param name="tablePrefixe">Pr�fixe de la table contenant toute la nomenclature</param>
        /// <param name="beginByAnd">La condition doit elle commencer par And</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string getAccessVehicleList(WebSession webSession, string tablePrefixe, bool beginByAnd)
        {
            string sql = "";
			 string idVehicleVMC="";

			
            string idVehicleListString = webSession.CustomerLogin[TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccessForRecap];
			//Force VMC rigths
			if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE){
				idVehicleVMC = TNS.AdExpress.Domain.Lists.GetIdList(WebConstantes.GroupList.ID.media, WebConstantes.GroupList.Type.forceVmcRightsInRecap);
				if (idVehicleVMC != null && idVehicleVMC.Length > 0) {
					idVehicleListString = (idVehicleListString != null && idVehicleListString.Length > 0) ? idVehicleListString + "," + idVehicleVMC : idVehicleVMC;
				}
			}
            if (idVehicleListString.Length > 0)
            {
                if (beginByAnd) sql += " and ";
                sql += SQLGenerator.GetInClauseMagicMethod(tablePrefixe + ".id_vehicle", idVehicleListString, true) + " ";
            }
            return (sql);
        }


        /// <summary>
        /// G�n�re les droits media clients .
        /// </summary>
        /// <param name="webSession">Session du client</param>		
        /// <param name="categoryPrefixe">Pr�fixe de la table interest_center</param>
        /// <param name="mediaPrefixe">Pr�fixe de la table media (support)</param>		
        /// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string getClassificationCustomerMediaRight(WebSession webSession, string categoryPrefixe, string mediaPrefixe, bool beginByAnd)
        {
            string sql = "";
            bool premier = true;
            // category(cat�gorie)
            if (webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length > 0)
            {
                if (!premier) sql += " or";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " ((";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(categoryPrefixe + ".id_category", webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess], true) + " ";
                premier = false;
            }
            // media (support)
            if (webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess].Length > 0)
            {
                if (!premier) sql += " or";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " ((";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(mediaPrefixe + ".id_media", webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess], true) + " ";
                premier = false;
            }

            if (!premier) sql += " )";
            // Droits en exclusion			
            // cat�gorie
            if (webSession.CustomerLogin[CustomerRightConstante.type.categoryException].Length > 0)
            {
                if (!premier) sql += " and";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " (";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(categoryPrefixe + ".id_category", webSession.CustomerLogin[CustomerRightConstante.type.categoryException], false) + " ";
                premier = false;
            }
            // media (support)
            if (webSession.CustomerLogin[CustomerRightConstante.type.mediaException].Length > 0)
            {
                if (!premier) sql += " and";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " (";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(mediaPrefixe + ".id_media", webSession.CustomerLogin[CustomerRightConstante.type.mediaException], false) + " ";
                premier = false;
            }

            if (!premier) sql += " )";
            return (sql);
        }

        /// <summary>
        /// G�n�re les droits media clients .
        /// </summary>
        /// <param name="webSession">Session du client</param>		
        /// <param name="categoryPrefixe">Pr�fixe de la table category</param>	
        /// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string getClassificationCustomerMediaRight(WebSession webSession, string categoryPrefixe, bool beginByAnd)
        {
            string sql = "";
            bool premier = true;
            // Cat�gorie
            if (webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length > 0)
            {
                if (!premier) sql += " or";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " ((";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(categoryPrefixe + ".id_category", webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess], true) + " ";
                premier = false;
            }
            if (!premier) sql += " )";
            // Droits en exclusion			
            // Cat�gorie 
            if (webSession.CustomerLogin[CustomerRightConstante.type.categoryException].Length > 0)
            {
                if (!premier) sql += " and";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " (";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(categoryPrefixe + ".id_category", webSession.CustomerLogin[CustomerRightConstante.type.categoryException], false) + " ";
                premier = false;
            }
            if (!premier) sql += " )";
            return (sql);
        }


		/// <summary>
		/// Force les droits VMC dans les recap
		/// </summary>
		/// <remarks>Used only for recap modules</remarks>
		/// <param name="webSession">Session du client</param>
		/// <param name="tablePrefixe">Pr�fixe de la table contenant toute la nomenclature</param>
		/// <param name="beginByAnd">La condition doit elle commencer par And</param>
		/// <returns>Code SQL g�n�r�</returns>
		public static string ForceVmcRightsInRecap(WebSession webSession, string tablePrefixe, bool beginByAnd) {
			string sql = "";			
			if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE) {
				string idVehicle = TNS.AdExpress.Domain.Lists.GetIdList(WebConstantes.GroupList.ID.media, WebConstantes.GroupList.Type.forceVmcRightsInRecap);
				if (idVehicle != null && idVehicle.Length > 0) {
					sql += " and " + tablePrefixe + ".id_vehicle in (" + idVehicle + ") ";
				}
			}
			return (sql);

		}


        #region GetMediaRights

        /// <summary>
        /// Get vehicle classification brand rights
        /// </summary>
        /// <param name="prefix">prefix</param> 
        /// <param name="beginByAnd">True if sql clause start with "AND"</param>
        /// <returns>string sql</returns>
        public static string GetMediaRights(WebSession session, string prefix, bool beginByAnd)
        {
            return GetMediaRights(session,prefix, prefix, prefix, beginByAnd);
        }

        /// <summary>
        /// Get vehicle classification brand rights
        /// </summary>
        /// <param name="categoryTable">Sub media Table description</param>
        /// <param name="mediaTable">vehicle Table description</param>
        /// <param name="vehicleTable">media Table description</param>
        /// <param name="beginByAnd">True if sql clause start with "AND"</param>
        /// <returns>sql rights string</returns>
        public static string GetMediaRights(WebSession session, Table vehicleTable, Table categoryTable, Table mediaTable, bool beginByAnd)
        {
            return GetMediaRights(session,vehicleTable.Prefix, categoryTable.Prefix, mediaTable.Prefix, beginByAnd);
        }

        /// <summary>
        /// Get vehicle classification brand rights.
        /// </summary>
        /// <param name="categoryTable">sub media Table Prefix</param>
        /// <param name="mediaTable">vehicle Table Prefix</param>
        /// <param name="vehicleTable">media Table Prefix</param>
        /// <param name="beginByAnd">True if sql clause start with "AND"</param>
        /// <returns>string sql</returns>
        public static string GetMediaRights(WebSession session, string vehicleTablePrefix, string categoryTablePrefix, string mediaTablePrefix, bool beginByAnd)
        {
            string sql = "";

            bool fisrt = true;
            //Get Media rights filter data
            Dictionary<TNS.AdExpress.Constantes.Customer.Right.type, string> rights = session.CustomerDataFilters.MediaRights;

            if (rights != null)
            {

                // Get the medias authorized for the current customer
                if (rights.ContainsKey(CustomerRightConstante.type.vehicleAccess) && rights[CustomerRightConstante.type.vehicleAccess].Length > 0)
                {
                    if (beginByAnd) sql += " and";
                    sql += " ((" + SQLGenerator.GetInClauseMagicMethod(((vehicleTablePrefix != null && vehicleTablePrefix.Length > 0) ? vehicleTablePrefix + "." : "") + "id_vehicle", rights[CustomerRightConstante.type.vehicleAccess], true) + " ";
                    fisrt = false;
                }
                // Get the sub medias authorized for the current customer
                if (rights.ContainsKey(CustomerRightConstante.type.categoryAccess) && rights[CustomerRightConstante.type.categoryAccess].Length > 0)
                {
                    if (!fisrt) sql += " or";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " ((";
                    }
                    sql += " " + SQLGenerator.GetInClauseMagicMethod(((categoryTablePrefix != null && categoryTablePrefix.Length > 0) ? categoryTablePrefix + "." : "") + "id_category", rights[CustomerRightConstante.type.categoryAccess], true) + " ";
                    fisrt = false;
                }
                // Get the vehicles authorized for the current customer                
                if (rights.ContainsKey(CustomerRightConstante.type.mediaAccess) && rights[CustomerRightConstante.type.mediaAccess].Length > 0)
                {
                    if (!fisrt) sql += " or";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " ((";
                    }
                    sql += " " + SQLGenerator.GetInClauseMagicMethod(((mediaTablePrefix != null && mediaTablePrefix.Length > 0) ? mediaTablePrefix + "." : "") + "id_media", rights[CustomerRightConstante.type.mediaAccess], true) + " ";
                    fisrt = false;
                }
                if (!fisrt) sql += " )";

                // Get the medias not authorized for the current customer
                if (rights.ContainsKey(CustomerRightConstante.type.vehicleException) && rights[CustomerRightConstante.type.vehicleException].Length > 0)
                {
                    if (!fisrt) sql += " and";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " (";
                    }
                    sql += " " + SQLGenerator.GetInClauseMagicMethod(((vehicleTablePrefix != null && vehicleTablePrefix.Length > 0) ? vehicleTablePrefix + "." : "") + "id_vehicle", rights[CustomerRightConstante.type.vehicleException], false) + " ";
                    fisrt = false;
                }
                // Get the sub medias not authorized for the current customer
                if (rights.ContainsKey(CustomerRightConstante.type.categoryException) && rights[CustomerRightConstante.type.categoryException].Length > 0)
                {
                    if (!fisrt) sql += " and";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " (";
                    }
                    sql += " " + SQLGenerator.GetInClauseMagicMethod(((categoryTablePrefix != null && categoryTablePrefix.Length > 0) ? categoryTablePrefix + "." : "") + "id_category", rights[CustomerRightConstante.type.categoryException], false) + " ";
                    fisrt = false;
                }
                // Get the vehicles not authorized for the current customer
                if (rights.ContainsKey(CustomerRightConstante.type.mediaException) && rights[CustomerRightConstante.type.mediaException].Length > 0)
                {
                    if (!fisrt) sql += " and";
                    else
                    {
                        if (beginByAnd) sql += " and";
                        sql += " (";
                    }
                    sql += " " + SQLGenerator.GetInClauseMagicMethod(((mediaTablePrefix != null && mediaTablePrefix.Length > 0) ? mediaTablePrefix + "." : "") + "id_media", rights[CustomerRightConstante.type.mediaException], false) + " ";
                    fisrt = false;
                }
                if (!fisrt) sql += " )";
            }
            return sql;
        }
        #endregion

        #endregion

        #region Nomenclature support tronqu�e � Vehicle > Category
        /// <summary>
        /// G�n�re les droits clients support
        /// Cette fonction est � utiliser si une m�me table contient tous les identifiants de la nomenclature
        /// support limit�e aux niveaux vehicle > category
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="tablePrefixe">Pr�fixe de la table qui contient les donn�es</param>
        /// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string getShortMediaRight(WebSession webSession, string tablePrefixe, bool beginByAnd)
        {
            return (getShortMediaRight(webSession, tablePrefixe, tablePrefixe, tablePrefixe, beginByAnd));
        }


        /// <summary>
        /// G�n�re les droits clients Media limit�s aux niveaux vehicle > category.
        /// Cette fonction est � utiliser si la nomenclature est � plat dans la requ�te.				
        /// Les pr�fixes sont d�finis dans TNS.AdExpress.Constantes.DB.Tables
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string getShortMediaRight(WebSession webSession, bool beginByAnd)
        {
            return (getShortMediaRight(webSession, DBConstantes.Tables.VEHICLE_PREFIXE, DBConstantes.Tables.CATEGORY_PREFIXE, DBConstantes.Tables.MEDIA_PREFIXE, beginByAnd));
        }


        /// <summary>
        /// G�n�re les droits media clients limit�s aux niveaux vehicle > category.
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="vehiclePrefixe">Pr�fixe de la table vehicle (media)</param>
        /// <param name="categoryPrefixe">Pr�fixe de la table category</param>
        /// <param name="mediaPrefixe">Pr�fixe de la table media (support)</param>		
        /// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string getShortMediaRight(WebSession webSession, string vehiclePrefixe, string categoryPrefixe, string mediaPrefixe, bool beginByAnd)
        {
            string sql = "";
            bool premier = true;
            // vehicle (media)
            if (webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess].Length > 0)
            {
                if (beginByAnd) sql += " and";
                sql += " ((" + SQLGenerator.GetInClauseMagicMethod(vehiclePrefixe + ".id_vehicle", webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess], true) + " ";
                premier = false;
            }
            // category (Categorie)
            if (webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length > 0)
            {
                if (!premier) sql += " or";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " ((";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(categoryPrefixe + ".id_category", webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess], true) + " ";
                premier = false;
            }

            if (!premier) sql += " )";
            // Droits en exclusion
            // vehicle (media)
            if (webSession.CustomerLogin[CustomerRightConstante.type.vehicleException].Length > 0)
            {
                if (!premier) sql += " and";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " (";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(vehiclePrefixe + ".id_vehicle", webSession.CustomerLogin[CustomerRightConstante.type.vehicleException], false) + " ";
                premier = false;
            }
            // category (Categorie)
            if (webSession.CustomerLogin[CustomerRightConstante.type.categoryException].Length > 0)
            {
                if (!premier) sql += " and";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " (";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(categoryPrefixe + ".id_category", webSession.CustomerLogin[CustomerRightConstante.type.categoryException], false) + " ";
                premier = false;
            }

            if (!premier) sql += " )";
            return (sql);
        }

        #endregion

        #region Annonceurs

        /// <summary>
        /// G�n�re les droits clients Annonceurs.
        /// Cette fonction est � utiliser si la nomenclature annonceurs est � plat.
        /// Les pr�fixes de tables utilis�es sont ceux d�finis dans les constantes 
        /// TNS.AdExpress.Constantes.DB.Tables
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string getAnalyseCustomerAdvertiserRight(WebSession webSession, bool beginByAnd)
        {
            return getAnalyseCustomerAdvertiserRight(webSession, DBConstantes.Tables.HOLDING_PREFIXE, DBConstantes.Tables.ADVERTISER_PREFIXE, beginByAnd);
        }


        /// <summary>
        /// G�n�re les droits clients Annonceurs.
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="advertiserPrefixe">Pr�fixe de la table contenant le niveau annonceur</param>
        /// <param name="holdingPrefixe">Pr�fixe de la table contenant le niveau hoding</param>
        /// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string getAnalyseCustomerAdvertiserRight(WebSession webSession, string holdingPrefixe, string advertiserPrefixe, bool beginByAnd)
        {
            string sql = "";
            bool premier = true;
            // le bloc doit il commencer par AND		
            // Holding Company (Groupe d'annonceurs)
            if (webSession.CustomerLogin[CustomerRightConstante.type.holdingCompanyAccess].Length > 0)
            {
                if (beginByAnd) sql += " and";
                sql += " ((" + GetInClauseMagicMethod(holdingPrefixe + ".id_holding_company", webSession.CustomerLogin[CustomerRightConstante.type.holdingCompanyAccess], true) + " ";
                premier = false;
            }
            // Advertiser (Annonceur)
            if (webSession.CustomerLogin[CustomerRightConstante.type.advertiserAccess].Length > 0)
            {
                if (!premier) sql += " or";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " ((";
                }
                sql += " " + GetInClauseMagicMethod(advertiserPrefixe + ".id_advertiser", webSession.CustomerLogin[CustomerRightConstante.type.advertiserAccess], true) + " ";
                premier = false;
            }
            if (!premier) sql += " )";
            // Droits en exclusion
            // Holding Company (Groupe de soci�t�s)
            if (webSession.CustomerLogin[CustomerRightConstante.type.holdingCompanyException].Length > 0)
            {
                if (!premier) sql += " and";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " (";
                }
                sql += " " + GetInClauseMagicMethod(holdingPrefixe + ".id_holding_company", webSession.CustomerLogin[CustomerRightConstante.type.holdingCompanyException], false) + " ";
                premier = false;
            }
            // Advertiser (Annonceur)
            if (webSession.CustomerLogin[CustomerRightConstante.type.advertiserException].Length > 0)
            {
                if (!premier) sql += " and";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " (";
                }
                sql += " " + GetInClauseMagicMethod(advertiserPrefixe + ".id_advertiser", webSession.CustomerLogin[CustomerRightConstante.type.advertiserException], false) + " ";
                premier = false;
            }
            if (!premier) sql += " )";
            return (sql);
        }


        /// <summary>
        /// G�n�re les droits clients Annonceurs.
        /// Cette fonction est � utiliser si une m�me table contient tous les identifiants de la nomenclature annonceurs.
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="tablePrefixe">Pr�fixe de la table qui contient les donn�es</param>
        /// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string getAnalyseCustomerAdvertiserRight(WebSession webSession, string tablePrefixe, bool beginByAnd)
        {
            return getAnalyseCustomerAdvertiserRight(webSession, tablePrefixe, tablePrefixe, beginByAnd);
        }



        #endregion

        #endregion

        #region S�lections client

        /// <summary>
        /// !!!! Non D�velopp�, ne pas utiliser !!!!
        /// G�n�re le code SQL correpondant � la s�lection du client
        /// Cette fonction est � utiliser si une m�me table contient tous les identifiants de la nomenclature support.
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="tablePrefixe">Pr�fixe de la table qui contient les donn�es</param>
        /// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string getAnalyseCustomerMediaSelection(WebSession webSession, string tablePrefixe, bool beginByAnd)
        {
            string sql = "";
            //bool premier=true;
            // le bloc doit il commencer par AND
            if (beginByAnd) sql += " and";
            return (sql);
        }
        #region S�lection m�dia
        /// <summary>
        /// s�lection  m�dia pour analyse sectorielle
        /// </summary>
        /// <param name="CategoryAccessList">cat�gories en acc�s</param>
        /// <param name="MediaAccessList">supports en acc�s</param>
        /// <param name="beginbyand">vrai si commence par "and"</param>
        /// <returns>s�lection m�dia</returns>
        public static string GetRecapMediaSelection(string CategoryAccessList, string MediaAccessList, bool beginbyand)
        {
            return GetRecapMediaSelection(CategoryAccessList, MediaAccessList, "", beginbyand);
        }

        /// <summary>
        /// s�lection  m�dia pour analyse sectorielle
        /// </summary>
        /// <param name="CategoryAccessList">cat�gories en acc�s</param>
        /// <param name="MediaAccessList">supports en acc�s</param>
        /// <param name="indexSubQuery">index de la sous requ�te</param>
        /// <param name="beginbyand">vrai si commence par "and"</param>
        /// <returns>s�lection m�dia</returns>
        public static string GetRecapMediaSelection(string CategoryAccessList, string MediaAccessList, string indexSubQuery, bool beginbyand)
        {
            string sql = "";
            // Category				
            if (CheckedText.IsNotEmpty(CategoryAccessList))
            {
                if (CheckedText.IsNotEmpty(MediaAccessList))
                {
                    if (beginbyand)
                        sql += " and ( ";
                    else sql += "  ( ";
                }
                else if (beginbyand) sql += " and  ";
                sql += "  " + SQLGenerator.GetInClauseMagicMethod(DBConstantes.Tables.RECAP_PREFIXE + indexSubQuery + ".id_category", CategoryAccessList, true) + " ";
            }
            // Media			
            if (CheckedText.IsNotEmpty(MediaAccessList))
            {
                if (CheckedText.IsNotEmpty(CategoryAccessList)) sql += " or ";
                else if (beginbyand) sql += " and  ";
                sql += "  " + SQLGenerator.GetInClauseMagicMethod(DBConstantes.Tables.RECAP_PREFIXE + indexSubQuery + ".id_media", MediaAccessList, true) + " ";
                if (CheckedText.IsNotEmpty(CategoryAccessList)) sql += " ) ";
            }
            return sql;
        }

        #endregion

        #region S�lection produits
        /// <summary>
        /// G�n�re les s�lections produits clients .
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <param name="holdingCompanyPrefixe">prefixe groupe de soci�t�s</param>
        /// <param name="advertiserPrefixe">prefixe annonceur</param>
        /// <param name="branPrefixe">prefixe marque</param>
        /// <param name="productPrefixe">prefixe produit</param>
        /// <param name="sectorPrefixe">prefixe famille</param>
        /// <param name="subsectorPrefixe">prefixe classe</param>
        /// <param name="groupPrefixe">prefixe groupe</param>
        /// <param name="beginByAnd">vrai si la requete commence par "And"</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string GetAnalyseCustomerProductSelection(WebSession webSession, string holdingCompanyPrefixe, string advertiserPrefixe, string branPrefixe, string productPrefixe, string sectorPrefixe, string subsectorPrefixe, string groupPrefixe, bool beginByAnd)
        {
            string list = "";
            string sql = "";
            bool premier = true;

            #region S�lection
            // S�lection en acc�s
            premier = true;
            // HoldingCompany
            list = webSession.GetSelection(webSession.CurrentUniversAdvertiser, CustomerRightConstante.type.holdingCompanyAccess);
            if (list.Length > 0)
            {
                if (beginByAnd) sql += " and ";
                sql += "  (( " + holdingCompanyPrefixe + ".id_holding_company in (" + list + ") ";
                premier = false;
            }
            // Advertiser
            list = webSession.GetSelection(webSession.CurrentUniversAdvertiser, CustomerRightConstante.type.advertiserAccess);
            if (list.Length > 0)
            {
                if (!premier) sql += " or ";
                else
                {
                    if (beginByAnd) sql += " and ";
                    sql += "  (( ";
                }
                sql += advertiserPrefixe + ".id_advertiser in (" + list + ") ";
                premier = false;
            }
            // Marque
            list = webSession.GetSelection(webSession.CurrentUniversAdvertiser, CustomerRightConstante.type.brandAccess);
            if (list.Length > 0)
            {
                if (!premier) sql += " or ";
                else
                {
                    if (beginByAnd) sql += " and ";
                    sql += "  (( ";
                }
                sql += branPrefixe + ".id_brand in (" + list + ") ";
                premier = false;
            }
            // Product
            list = webSession.GetSelection(webSession.CurrentUniversAdvertiser, CustomerRightConstante.type.productAccess);
            if (list.Length > 0)
            {
                if (!premier) sql += " or ";
                else
                {
                    if (beginByAnd) sql += " and ";
                    sql += "  (( ";
                }
                sql += productPrefixe + ".id_product in (" + list + ") ";
                premier = false;
            }
            // Sector
            list = webSession.GetSelection(webSession.CurrentUniversAdvertiser, CustomerRightConstante.type.sectorAccess);
            if (list.Length > 0)
            {
                sql += " and (( " + sectorPrefixe + ".id_sector in (" + list + ") ";
                premier = false;
            }
            // SubSector
            list = webSession.GetSelection(webSession.CurrentUniversAdvertiser, CustomerRightConstante.type.subSectorAccess);
            if (list.Length > 0)
            {
                if (!premier) sql += " or ";
                else
                {
                    if (beginByAnd) sql += " and ";
                    sql += "  (( ";
                }
                sql += subsectorPrefixe + ".id_subsector in (" + list + ") ";
                premier = false;
            }
            // Group
            list = webSession.GetSelection(webSession.CurrentUniversAdvertiser, CustomerRightConstante.type.groupAccess);
            if (list.Length > 0)
            {
                if (!premier) sql += " or ";
                else
                {
                    if (beginByAnd) sql += " and ";
                    sql += "  (( ";
                }
                sql += groupPrefixe + ".id_group_ in (" + list + ") ";
                premier = false;
            }

            if (!premier) sql += " )";

            // S�lection en Exception
            // HoldingCompany
            list = webSession.GetSelection(webSession.CurrentUniversAdvertiser, CustomerRightConstante.type.holdingCompanyException);
            if (list.Length > 0)
            {
                if (premier)
                {
                    if (beginByAnd) sql += " and ";
                    sql += "  ( ";
                }
                else sql += " and ";
                sql += holdingCompanyPrefixe + ".id_holding_company not in (" + list + ") ";
                premier = false;
            }
            // Advertiser
            list = webSession.GetSelection(webSession.CurrentUniversAdvertiser, CustomerRightConstante.type.advertiserException);
            if (list.Length > 0)
            {
                if (premier)
                {
                    if (beginByAnd) sql += " and ";
                    sql += "  ( ";
                }
                else sql += " and ";
                sql += advertiserPrefixe + ".id_advertiser not in (" + list + ") ";
                premier = false;
            }
            // Marque
            list = webSession.GetSelection(webSession.CurrentUniversAdvertiser, CustomerRightConstante.type.brandException);
            if (list.Length > 0)
            {
                if (premier)
                {
                    if (beginByAnd) sql += " and ";
                    sql += "  ( ";
                }
                else sql += " and ";
                sql += branPrefixe + ".id_brand not in (" + list + ") ";
                premier = false;
            }
            // Product
            list = webSession.GetSelection(webSession.CurrentUniversAdvertiser, CustomerRightConstante.type.productException);
            if (list.Length > 0)
            {
                if (premier)
                {
                    if (beginByAnd) sql += " and ";
                    sql += "  ( ";
                }
                else sql += " and ";
                sql += productPrefixe + ".id_product not in (" + list + ") ";
                premier = false;
            }
            // Sector
            list = webSession.GetSelection(webSession.CurrentUniversAdvertiser, CustomerRightConstante.type.sectorException);
            if (list.Length > 0)
            {
                if (premier)
                {
                    if (beginByAnd) sql += " and ";
                    sql += "  ( ";
                }
                else sql += " and ";
                sql += sectorPrefixe + ".id_sector not in (" + list + ") ";
                premier = false;
            }
            // SubSector
            list = webSession.GetSelection(webSession.CurrentUniversAdvertiser, CustomerRightConstante.type.subSectorException);
            if (list.Length > 0)
            {
                if (premier)
                {
                    if (beginByAnd) sql += " and ";
                    sql += "  ( ";
                }
                else sql += " and ";
                sql += subsectorPrefixe + ".id_subsector not in (" + list + ") ";
                premier = false;
            }
            // Group
            list = webSession.GetSelection(webSession.CurrentUniversAdvertiser, CustomerRightConstante.type.groupException);
            if (list.Length > 0)
            {
                if (premier)
                {
                    if (beginByAnd) sql += " and ";
                    sql += "  ( ";
                }
                else sql += " and ";
                sql += groupPrefixe + ".id_group_ not in (" + list + ") ";
                premier = false;
            }
            if (!premier) sql += " )";
            #endregion

            return (sql);
        }
        /// <summary>
        /// G�n�re les s�lections produits clients .
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <param name="advertiserPrefixe">prefixe annonceur</param>
        /// <param name="branPrefixe">prefixe marque</param>
        /// <param name="productPrefixe">prefixe produit</param>
        /// <param name="beginByAnd">vrai si la requete commence par "And"</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string GetAnalyseCustomerProductSelection(WebSession webSession, string advertiserPrefixe, string branPrefixe, string productPrefixe, bool beginByAnd)
        {
            return GetAnalyseCustomerProductSelection(webSession, "", advertiserPrefixe, branPrefixe, productPrefixe, "", "", "", beginByAnd);
        }

        #region liste des familles pour une s�lection de produits dans les Recap
        /// <summary>
        /// Retourne la liste des familles pour une s�lection de produits dans les Recap.
        /// une vari�t� ou un groupe s�lectionn�
        /// </summary>
        /// <param name="recapTableName">table recap</param>
        /// <param name="GroupAccessList">liste des groupes</param>
        /// <param name="SegmentAccessList">liste des vari�t�s</param>		
        /// <returns>liste des familles</returns>
        public static string GetSectorList(string recapTableName, string GroupAccessList, string SegmentAccessList)
        {
            string sql = "";
            DataSet ds = new DataSet();
            string sectorList = "";

            #region selection familles  pour total famille
            //S�lection produit pour le calcul du total famille								

            #region s�lection des familles

            sql += " select distinct id_sector ";
            sql += " from " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.recap01).Label + "." + recapTableName + " " + DBConstantes.Tables.RECAP_PREFIXE + " ";
            if (CheckedText.IsNotEmpty(GroupAccessList))
            {
                sql += " where id_group_ in (" + GroupAccessList + ") ";
            }
            if (CheckedText.IsNotEmpty(SegmentAccessList) && CheckedText.IsNotEmpty(GroupAccessList))
            {
                sql += " or id_segment in  (" + SegmentAccessList + ")";
            }
            else if (CheckedText.IsNotEmpty(SegmentAccessList))
            {
                sql += " where id_segment in (" + SegmentAccessList + ") ";
            }
            #endregion

            #region Execution de la requ�te
            IDataSource dataSource = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis);
            try
            {
                ds = dataSource.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new SQLGeneratorException("Impossible de charger les donn�es pour start�gie media: " + sql, err));
            }
            #endregion

            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow currentRow in ds.Tables[0].Rows)
                {
                    if (ds.Tables[0].Columns.Contains("id_sector"))
                    {
                        sectorList += currentRow["id_sector"].ToString() + ",";
                    }
                }
                if (CheckedText.IsNotEmpty(sectorList))
                {
                    sectorList = sectorList.ToString().Substring(0, sectorList.Length - 1);
                }
            }


            #endregion

            return sectorList;

        }
        #endregion

        #region s�lection produit pour indicateurs
        /// <summary>
        /// S�lection produit pour analyse sectorielles
        /// </summary>
        /// <param name="comparisonCriterion">crit�re de comparaison (total univers, famille ou march�)</param>
        /// <param name="recapTableName">nom table de donn�es</param>
        /// <param name="indexSubQuery">index de la sous requ�te</param>
        /// <param name="GroupAccessList">groupes en acc�s</param>
        /// <param name="GroupExceptionList">groupes en exceptions</param>
        /// <param name="SegmentAccessList">vari�t�s en acc�s</param>
        /// <param name="SegmentExceptionList">vari�t�s en exception</param>
        /// <param name="AdvertiserAccessList">annonceurs en acc�s</param>
        /// <param name="RefenceOrCompetitorAdvertiser">vrai si annonceurs concurrents ou concurrents</param>
        /// <param name="beginByAnd">Vrai si requ�tre commence par " and "</param>
        /// <returns>s�lection produit</returns>
        public static string GetRecapProductSelection(TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion comparisonCriterion, string recapTableName, string indexSubQuery, string GroupAccessList, string GroupExceptionList, string SegmentAccessList, string SegmentExceptionList, string AdvertiserAccessList, bool RefenceOrCompetitorAdvertiser, bool beginByAnd)
        {
            string sql = "";
            string totalSector = "";
            bool premier = true;

            //S�lections des familles pour le total famille
            if (comparisonCriterion == WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal)
            {
                totalSector = GetSectorList(recapTableName, GroupAccessList, SegmentAccessList);
                if (CheckedText.IsNotEmpty(totalSector))
                {
                    if (beginByAnd) sql += " and ";
                    sql += "  " + DBConstantes.Tables.RECAP_PREFIXE + indexSubQuery + ".id_sector in (" + totalSector + ") ";
                    beginByAnd = true;
                }
            }

            //S�lection produit pour le calcul du total univers
            if (comparisonCriterion == WebConstantes.CustomerSessions.ComparisonCriterion.universTotal)
            {
                #region S�lection de Produits (groupes et/ou vari�t�s
                // S�lection en acc�s
                premier = true;
                // group				
                if (CheckedText.IsNotEmpty(GroupAccessList))
                {
                    if (beginByAnd) sql += " and ";
                    //					sql+=" and (("+DBConstantes.Tables.RECAP_PREFIXE+indexSubQuery+".id_group_ in ("+GroupAccessList+") ";
                    sql += "  ((" + DBConstantes.Tables.RECAP_PREFIXE + indexSubQuery + ".id_group_ in (" + GroupAccessList + ") ";
                    premier = false;
                    beginByAnd = true;
                }
                // Segment				
                if (CheckedText.IsNotEmpty(SegmentAccessList))
                {
                    if (!premier) sql += " or";
                    else
                    {
                        if (beginByAnd) sql += " and ";
                        //						sql+=" and ((";
                        sql += "  ((";
                    }
                    sql += " " + DBConstantes.Tables.RECAP_PREFIXE + indexSubQuery + ".id_segment in (" + SegmentAccessList + ") ";
                    premier = false;
                    beginByAnd = true;
                }

                if (!premier) sql += " )";

                // S�lection en Exception
                // group				
                if (CheckedText.IsNotEmpty(GroupExceptionList))
                {
                    if (premier)
                    {
                        if (beginByAnd) sql += " and ";
                        sql += "  (";
                        //sql+=" and (";
                    }
                    else
                    {
                        if (beginByAnd) sql += " and ";
                        //						sql+=" and";
                    }
                    sql += " " + DBConstantes.Tables.RECAP_PREFIXE + indexSubQuery + ".id_group_ not in (" + GroupExceptionList + ") ";
                    premier = false;
                    beginByAnd = true;
                }
                // segment en Exception				
                if (CheckedText.IsNotEmpty(SegmentExceptionList))
                {
                    if (premier)
                    {
                        if (beginByAnd) sql += " and ";
                        sql += "  (";
                        //sql+=" and (";
                    }
                    else
                    {
                        if (beginByAnd) sql += " and ";
                        //						sql+=" and";
                    }
                    sql += " " + DBConstantes.Tables.RECAP_PREFIXE + indexSubQuery + ".id_segment not in (" + SegmentExceptionList + ") ";
                    premier = false;
                    beginByAnd = true;
                }
                if (!premier) sql += " )";

                #endregion
            }
            //annonceur de r�f�rences ou concurrents s�lectionn�s
            if (RefenceOrCompetitorAdvertiser && CheckedText.IsNotEmpty(AdvertiserAccessList))
            {
                if (beginByAnd) sql += " and ";
                //				sql+=" and "+DBConstantes.Tables.ADVERTISER_PREFIXE+indexSubQuery+".id_advertiser in ("+AdvertiserAccessList+") ";
                sql += "  " + DBConstantes.Tables.ADVERTISER_PREFIXE + indexSubQuery + ".id_advertiser in (" + AdvertiserAccessList + ") ";
            }
            return sql;
        }
        #endregion

        #endregion

        #region S�lection Emissions
        /// <summary>
        /// Obtient la s�lection �mission client
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="progamPrefixe">Pr�fixe emisssion</param>
        /// <param name="progamTypePrefixe">Pr�fixe Genre d'�mission</param>
        /// <param name="beginByAnd">vrai si la requete commence par "And"</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string GetCustomerProgramSelection(WebSession webSession, string progamPrefixe, string progamTypePrefixe, bool beginByAnd)
        {
            string list = "";
            string sql = "";
            bool premier = true;

            premier = true;
            // Emissions
            list = webSession.GetSelection(webSession.CurrentUniversProgramType, CustomerRightConstante.type.programAccess);
            if (list.Length > 0)
            {
                if (beginByAnd) sql += " and ";
                sql += "  (( " + progamPrefixe + ".id_program in (" + list + ") ";
                premier = false;
            }

            // Genre d'�misions
            list = webSession.GetSelection(webSession.CurrentUniversProgramType, CustomerRightConstante.type.programTypeAccess);
            if (list.Length > 0)
            {
                if (!premier) sql += " or ";
                else
                {
                    if (beginByAnd) sql += " and ";
                    sql += "  (( ";
                }
                sql += progamTypePrefixe + ".id_program_type in (" + list + ") ";
                premier = false;
            }

            if (!premier) sql += " )";

            // S�lection en Exception
            // Emissions
            list = webSession.GetSelection(webSession.CurrentUniversProgramType, CustomerRightConstante.type.programException);
            if (list.Length > 0)
            {
                if (premier)
                {
                    if (beginByAnd) sql += " and ";
                    sql += "  ( ";
                }
                else sql += " and ";
                sql += progamPrefixe + ".id_program not in (" + list + ") ";
                premier = false;
            }

            // Genre d'�misions
            list = webSession.GetSelection(webSession.CurrentUniversProgramType, CustomerRightConstante.type.programTypeException);
            if (list.Length > 0)
            {
                if (premier)
                {
                    if (beginByAnd) sql += " and ";
                    sql += "  ( ";
                }
                else sql += " and ";
                sql += progamTypePrefixe + ".id_program_type not in (" + list + ") ";
                premier = false;
            }

            if (!premier) sql += " )";


            return (sql);
        }
        #endregion

        #region Formesde parrainage
        /// <summary>
        /// Obtient la s�lection  client forme de parrainage
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="sponsorshipFormPrefixe">Pr�fixe Forme de parrainage</param>
        /// <param name="beginByAnd">vrai si la requete commence par "And"</param>
        /// <returns>Code SQL g�n�r�</returns>
        public static string GetCustomerSponsorshipFormSelection(WebSession webSession, string sponsorshipFormPrefixe, bool beginByAnd)
        {
            string list = "";
            string sql = "";
            bool premier = true;

            // S�lection en Acc�s
            // Forme de parrainage
            list = webSession.GetSelection(webSession.CurrentUniversSponsorshipForm, CustomerRightConstante.type.sponsorshipFormAccess);
            if (list.Length > 0)
            {
                if (beginByAnd) sql += " and ";
                sql += "  (( " + sponsorshipFormPrefixe + ".id_form_sponsorship in (" + list + ") ";
                premier = false;
            }


            if (!premier) sql += " )";

            // S�lection en Exception
            // Forme de parrainage
            list = webSession.GetSelection(webSession.CurrentUniversSponsorshipForm, CustomerRightConstante.type.sponsorshipFormException);
            if (list.Length > 0)
            {
                if (premier)
                {
                    if (beginByAnd) sql += " and ";
                    sql += "  ( ";
                }
                else sql += " and ";
                sql += sponsorshipFormPrefixe + ".id_form_sponsorship not in (" + list + ") ";
                premier = false;
            }


            if (!premier) sql += " )";


            return (sql);
        }
        #endregion

        #endregion

        #region Univers Media AdExpress

        /// <summary>
        /// Donne la condition SQL pour int�grer la notion d'univers Adexpress
        /// </summary>
        /// <param name="webSession">Session du client AdExpress</param>
        /// <param name="beginByAnd">La condition doit elle commencer par And</param>
        /// <returns>Condition</returns>
        /// <remarks>
        /// Cette m�thode doit �tre utilis�e que si la nomenclature support n'est pas contenue
        /// dans une m�me table (M�dia, cat�gorie, support).
        /// De plus, elle utilise les pr�fixes d�termin�s dans les constantes
        /// </remarks>
        public static string getAdExpressUniverseCondition(WebSession webSession, bool beginByAnd)
        {
            try
            {
                return (getAdExpressUniverseCondition(webSession, DBConstantes.Tables.VEHICLE_PREFIXE, DBConstantes.Tables.CATEGORY_PREFIXE, DBConstantes.Tables.MEDIA_PREFIXE, beginByAnd));
            }
            catch (System.Exception e)
            {
                throw (e);
            }
        }

        /// <summary>
        /// Donne la condition SQL pour int�grer la notion d'univers Adexpress
        /// </summary>
        /// <param name="webSession">Session du client AdExpress</param>
        /// <param name="vehicleTablePrefixe">Pr�fixe de la table vehicle</param>
        /// <param name="categoryTablePrefixe">Pr�fixe de la table vehicle</param>
        /// <param name="mediaTablePrefixe">Pr�fixe de la table m�dia</param>
        /// <param name="beginByAnd">La condition doit elle commencer par And</param>
        /// <returns>Condition</returns>
        /// <remarks>
        /// Cette m�thode ne peut �tre utilis�e que si toute la nomenclature support est contenue
        /// dans une m�me table (M�dia, cat�gorie, support).
        /// </remarks>
        public static string getAdExpressUniverseCondition(WebSession webSession, string vehicleTablePrefixe, string categoryTablePrefixe, string mediaTablePrefixe, bool beginByAnd)
        {
            try
            {
                switch (webSession.CurrentModule)
                {

                    case WebConstantes.Module.Name.ALERTE_CONCURENTIELLE:
                        return (getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.COMPETITIVE_ALERT_LIST_ID, vehicleTablePrefixe, categoryTablePrefixe, mediaTablePrefixe, beginByAnd));

                    case WebConstantes.Module.Name.ALERTE_PLAN_MEDIA:
                        return (getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.MEDIA_SCHEDULE_ALERT_LIST_ID, vehicleTablePrefixe, categoryTablePrefixe, mediaTablePrefixe, beginByAnd));

                    case WebConstantes.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE:
                        return (getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.COMPARATIVE_MEDIA_SCHEDULE_ALERT_LIST_ID, vehicleTablePrefixe, categoryTablePrefixe, mediaTablePrefixe, beginByAnd));

                    case WebConstantes.Module.Name.ALERTE_PORTEFEUILLE:
                        return (getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.VEHICLE_PORTOFOLIO_ALERT_LIST_ID, vehicleTablePrefixe, categoryTablePrefixe, mediaTablePrefixe, beginByAnd));

                    case WebConstantes.Module.Name.ALERTE_POTENTIELS:
                        return (getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.PROSPECTING_ALERT_LIST_ID, vehicleTablePrefixe, categoryTablePrefixe, mediaTablePrefixe, beginByAnd));

                    case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
                        return (getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.COMPETITIVE_REPORTS_LIST_ID, vehicleTablePrefixe, categoryTablePrefixe, mediaTablePrefixe, beginByAnd));

                    case WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS:
                    case WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES:
                        return (getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID, vehicleTablePrefixe, categoryTablePrefixe, mediaTablePrefixe, beginByAnd));

                    case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
                        return (getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.TREND_REPORTS_LIST_ID, vehicleTablePrefixe, categoryTablePrefixe, mediaTablePrefixe, beginByAnd));

                    case WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA:
                        return (getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.MEDIA_SCHEDULE_ANALYSIS_LIST_ID, vehicleTablePrefixe, categoryTablePrefixe, mediaTablePrefixe, beginByAnd));

                    case WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA_CONCURENTIELLE:
                        return (getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.COMPARATIVE_MEDIA_SCHEDULE_ANALYSIS_LIST_ID, vehicleTablePrefixe, categoryTablePrefixe, mediaTablePrefixe, beginByAnd));

                    case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                        return (getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.VEHICLE_PORTOFOLIO_LIST_ID, vehicleTablePrefixe, categoryTablePrefixe, mediaTablePrefixe, beginByAnd));

                    case WebConstantes.Module.Name.ANALYSE_POTENTIELS:
                        return (getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.PROSPECTING_REPORTS_LIST_ID, vehicleTablePrefixe, categoryTablePrefixe, mediaTablePrefixe, beginByAnd));

                    case WebConstantes.Module.Name.TENDACES:
                        return (getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.TRENDS_LIST_ID, vehicleTablePrefixe, categoryTablePrefixe, mediaTablePrefixe, beginByAnd));

                    default:
                        return (getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.MEDIA_DEFAULT_LIST_ID, vehicleTablePrefixe, categoryTablePrefixe, mediaTablePrefixe, beginByAnd));

                }
            }
            catch (System.Exception e)
            {
                throw (e);
            }
        }

        /// <summary>
        /// Donne la condition SQL pour int�grer la notion d'univers Adexpress
        /// </summary>
        /// <param name="idMediaItemsList">Identifiant de l'univers m�dia AdExpress</param>
        /// <param name="tablePrefixe">Pr�fixe de la table contenant toute la nomenclature</param>
        /// <param name="beginByAnd">La condition doit elle commencer par And</param>
        /// <returns>Condition</returns>
        /// <remarks>
        /// Cette m�thode ne peut �tre utilis�e que si toute la nomenclature support est contenue
        /// dans une m�me table (M�dia, cat�gorie, support).
        /// </remarks>
        public static string getAdExpressUniverseCondition(int idMediaItemsList, string tablePrefixe, bool beginByAnd)
        {
            try
            {
                return (getAdExpressUniverseCondition(idMediaItemsList, tablePrefixe, tablePrefixe, tablePrefixe, beginByAnd));
            }
            catch (System.Exception e)
            {
                throw (e);
            }
        }

        /// <summary>
        /// Donne la condition SQL pour int�grer la notion d'univers Adexpress
        /// </summary>
        /// <param name="idMediaItemsList">Identifiant de l'univers m�dia AdExpress</param>
        /// <param name="tablePrefixe">Pr�fixe de la table contenant toute la nomenclature</param>
        /// <param name="beginByAnd">La condition doit elle commencer par And</param>
        /// <param name="webSession">Session client</param>
        /// <returns>Condition</returns>
        /// <remarks>
        /// Cette m�thode ne peut �tre utilis�e que si toute la nomenclature support est contenue
        /// dans une m�me table (M�dia, cat�gorie, support).
        /// </remarks>
        public static string getAdExpressUniverseCondition(WebSession webSession, int idMediaItemsList, string tablePrefixe, bool beginByAnd)
        {
            try
            {
                return (getAdExpressUniverseCondition(webSession, idMediaItemsList, tablePrefixe, tablePrefixe, tablePrefixe, beginByAnd));
            }
            catch (System.Exception e)
            {
                throw (e);
            }
        }

        /// <summary>
        /// Donne la condition SQL pour int�grer la notion d'univers Adexpress
        /// </summary>
        /// <param name="idMediaItemsList">Identifiant de l'univers m�dia AdExpress</param>
        /// <param name="beginByAnd">La condition doit elle commencer par And</param>
        /// <param name="withPrefixe">Request with tables prefixe</param>
        /// <returns>Condition</returns>
        /// <remarks>
        /// Cette m�thode doit �tre utilis�e que si la nomenclature support n'est pas contenue
        /// dans une m�me table (M�dia, cat�gorie, support).
        /// </remarks>
        public static string getAdExpressUniverseCondition(int idMediaItemsList, bool beginByAnd, bool withPrefixe) {
            try {
                if(withPrefixe)
                    return (getAdExpressUniverseCondition(idMediaItemsList, DBConstantes.Tables.VEHICLE_PREFIXE, DBConstantes.Tables.CATEGORY_PREFIXE, DBConstantes.Tables.MEDIA_PREFIXE, beginByAnd));
                else
                    return (getAdExpressUniverseCondition(idMediaItemsList, "", "", "", beginByAnd));
            }
            catch (System.Exception e) {
                throw (e);
            }
        }

        /// <summary>
        /// Donne la condition SQL pour int�grer la notion d'univers Adexpress
        /// </summary>
        /// <param name="idMediaItemsList">Identifiant de l'univers m�dia AdExpress</param>
        /// <param name="beginByAnd">La condition doit elle commencer par And</param>
        /// <returns>Condition</returns>
        /// <remarks>
        /// Cette m�thode doit �tre utilis�e que si la nomenclature support n'est pas contenue
        /// dans une m�me table (M�dia, cat�gorie, support).
        /// De plus, elle utilise les pr�fixes d�termin�s dans les constantes
        /// </remarks>
        public static string getAdExpressUniverseCondition(int idMediaItemsList, bool beginByAnd)
        {
            try
            {
                return (getAdExpressUniverseCondition(idMediaItemsList, DBConstantes.Tables.VEHICLE_PREFIXE, DBConstantes.Tables.CATEGORY_PREFIXE, DBConstantes.Tables.MEDIA_PREFIXE, beginByAnd));
            }
            catch (System.Exception e)
            {
                throw (e);
            }
        }

        /// <summary>
        /// Donne la condition SQL pour int�grer la notion d'univers Adexpress
        /// </summary>
        /// <param name="webSession">Session client</param>
        /// <param name="idMediaItemsList">Identifiant de l'univers m�dia AdExpress</param>
        /// <param name="beginByAnd">La condition doit elle commencer par And</param>
        /// <returns>Condition</returns>
        /// <remarks>
        /// Cette m�thode doit �tre utilis�e que si la nomenclature support n'est pas contenue
        /// dans une m�me table (M�dia, cat�gorie, support).
        /// De plus, elle utilise les pr�fixes d�termin�s dans les constantes
        /// </remarks>
        public static string getAdExpressUniverseCondition(WebSession webSession, int idMediaItemsList, bool beginByAnd)
        {
            try
            {
                return (getAdExpressUniverseCondition(webSession, idMediaItemsList, DBConstantes.Tables.VEHICLE_PREFIXE, DBConstantes.Tables.CATEGORY_PREFIXE, DBConstantes.Tables.MEDIA_PREFIXE, beginByAnd));
            }
            catch (System.Exception e)
            {
                throw (e);
            }
        }

        /// <summary>
        /// Donne la condition SQL pour int�grer la notion d'univers Adexpress
        /// </summary>
        /// <param name="idMediaItemsList">Identifiant de l'univers m�dia AdExpress</param>
        /// <param name="vehicleTablePrefixe">Pr�fixe de la table vehicle</param>
        /// <param name="categoryTablePrefixe">Pr�fixe de la table vehicle</param>
        /// <param name="mediaTablePrefixe">Pr�fixe de la table m�dia</param>
        /// <param name="beginByAnd">La condition doit elle commencer par And</param>
        /// <returns>Condition</returns>
        /// <remarks>
        /// Cette m�thode doit �tre utilis�e que si la nomenclature support n'est pas contenue
        /// dans une m�me table (M�dia, cat�gorie, support).
        /// </remarks>
        public static string getAdExpressUniverseCondition(int idMediaItemsList, string vehicleTablePrefixe, string categoryTablePrefixe, string mediaTablePrefixe, bool beginByAnd)
        {
            try
            {
                string sql = " ";
                if (beginByAnd) sql += "And ";
                MediaItemsList adexpressMediaItemsList = Media.GetItemsList(idMediaItemsList);
                if (adexpressMediaItemsList.VehicleList.Length > 0) sql += (vehicleTablePrefixe.Length>0 ?  vehicleTablePrefixe+"." : "") + "id_vehicle in(" + adexpressMediaItemsList.VehicleList + ") ";
                if (adexpressMediaItemsList.CategoryList.Length > 0) sql += (categoryTablePrefixe.Length>0 ? categoryTablePrefixe+"." : "") + "id_category in(" + adexpressMediaItemsList.CategoryList + ") ";
                if (adexpressMediaItemsList.MediaList.Length > 0) sql += (mediaTablePrefixe.Length>0 ? mediaTablePrefixe+"." : "") + "id_media in(" + adexpressMediaItemsList.MediaList + ") ";

                if (sql.Length < 6) return ("");
                return (sql);
            }
            catch (System.Exception e)
            {
                throw (new SQLGeneratorException("Impossible de d�terminer les conditions d'univers AdExpress: ", e));
            }
        }

        /// <summary>
        /// Donne la condition SQL pour int�grer la notion d'univers Adexpress
        /// </summary>
        /// <param name="webSession">Session client</param>
        /// <param name="idMediaItemsList">Identifiant de l'univers m�dia AdExpress</param>
        /// <param name="vehicleTablePrefixe">Pr�fixe de la table vehicle</param>
        /// <param name="categoryTablePrefixe">Pr�fixe de la table vehicle</param>
        /// <param name="mediaTablePrefixe">Pr�fixe de la table m�dia</param>
        /// <param name="beginByAnd">La condition doit elle commencer par And</param>
        /// <returns>Condition</returns>
        /// <remarks>
        /// Cette m�thode doit �tre utilis�e que si la nomenclature support n'est pas contenue
        /// dans une m�me table (M�dia, cat�gorie, support).
        /// </remarks>
        internal static string getAdExpressUniverseCondition(WebSession webSession, int idMediaItemsList, string vehicleTablePrefixe, string categoryTablePrefixe, string mediaTablePrefixe, bool beginByAnd)
        {
            try
            {
                string sql = " ";
                if (beginByAnd) sql += "And ";
                MediaItemsList adexpressMediaItemsList = Media.GetItemsList(idMediaItemsList);
                if (adexpressMediaItemsList.VehicleList.Length > 0) sql += vehicleTablePrefixe + ".id_vehicle in(" + adexpressMediaItemsList.VehicleList + ") ";

                if (adexpressMediaItemsList.CategoryList.Length > 0)
                {
                    if (WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID == idMediaItemsList
                        && webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SPONSORSHIP_TV_ACCESS_FLAG))
                    {
                        //Rajout de la cat�gorie parrainage tv pour les recap
                        sql += categoryTablePrefixe + ".id_category in(" + adexpressMediaItemsList.CategoryList + ",68) ";
                    }
                    else sql += categoryTablePrefixe + ".id_category in(" + adexpressMediaItemsList.CategoryList + ") ";
                }
                if (adexpressMediaItemsList.MediaList.Length > 0) sql += mediaTablePrefixe + ".id_media in(" + adexpressMediaItemsList.MediaList + ") ";
                if (sql.Length < 6) return ("");
                return (sql);
            }
            catch (System.Exception e)
            {
                throw (new SQLGeneratorException("Impossible de d�terminer les conditions d'univers AdExpress: " , e));
            }
        }
        #endregion

        #region Univers Support AdExpress
		/// <summary>
		/// Get excluded products
		/// </summary>
		/// <param name="prefix">prefix</param>
		/// <param name="idList">Baal Id list</param>
		/// <returns></returns>
		public static string GetExcludeProducts(int idList,string prefix) {
			// Exclude product 
			string sql = "";
			ProductItemsList prList = null; ;
			if (Product.Contains(idList) && (prList = Product.GetItemsList(idList)) != null)
				sql = prList.GetExcludeItemsSql(true, prefix);
			return sql;
		}

		/// <summary>
		/// Get media Universe
		/// </summary>
		/// <param name="webSession">Web Session</param>
		/// <param name="prefix">Prefix</param>
		/// <param name="startWithAnd">start with And</param>
		/// <returns>string sql</returns>
		public static string GetResultMediaUniverse(WebSession webSession,string prefix) {		
			return GetResultMediaUniverse(webSession, prefix,true);
		}
		/// <summary>
		/// Get media Universe
		/// </summary>
		/// <param name="webSession">Web Session</param>
		/// <param name="prefix">Prefix</param>
		/// <param name="startWithAnd">start with And</param>
		/// <returns>string sql</returns>
		public static string GetResultMediaUniverse(WebSession webSession, string prefix,bool startWithAnd) {
			string sql = "";
			WebNavigation.Module module = webSession.CustomerLogin.GetModule(webSession.CurrentModule);
			sql = module.GetResultPageInformation(webSession.CurrentTab).GetAllowedMediaUniverseSql(prefix, startWithAnd);
			return sql;
		}

        /// <summary>
        /// Donne la condition SQL pour int�grer la notion d'univers Adexpress
        /// </summary>
        /// <param name="idProductItemsList">Identifiant de l'univers m�dia AdExpress</param>
        /// <param name="tablePrefixe">Pr�fixe de la table contenant toute la nomenclature</param>
        /// <param name="beginByAnd">La condition doit elle commencer par And</param>
        /// <param name="access">si true condition en acc�s sinon en exclusion</param>
        /// <returns>Condition</returns>
        /// <remarks>
        /// Cette m�thode ne peut �tre utilis�e que si toute la nomenclature support est contenue
        /// dans une m�me table (famille, classe ,group, produit ).
        /// </remarks>
        public static string GetAdExpressProductUniverseCondition(int idProductItemsList, string tablePrefixe, bool beginByAnd, bool access)
        {
            try
            {
                return (getAdExpressProductUniverseCondition(idProductItemsList, tablePrefixe, tablePrefixe, tablePrefixe, tablePrefixe, tablePrefixe, beginByAnd, access));
            }
            catch (System.Exception e)
            {
                throw (e);
            }
        }

        /// <summary>
        /// Donne la condition SQL pour int�grer la notion d'univers Adexpress
        /// </summary>
        /// <param name="idProductItemsList">Identifiant de l'univers Produit AdExpress</param>
        /// <param name="beginByAnd">La condition doit elle commencer par And</param>
        /// <param name="access">si true condition en acc�s sinon en exclusion</param>
        /// <returns>Condition</returns>
        /// <remarks>
        /// Cette m�thode doit �tre utilis�e que si la nomenclature support n'est pas contenue
        /// dans une m�me table (famille, classe ,group, produit ).
        /// De plus, elle utilise les pr�fixes d�termin�s dans les constantes
        /// </remarks>
        internal static string getAdExpressProductUniverseCondition(int idProductItemsList, bool beginByAnd, bool access)
        {
            try
            {
                return (getAdExpressProductUniverseCondition(idProductItemsList, DBConstantes.Tables.SECTOR_PREFIXE, DBConstantes.Tables.SUBSECTOR_PREFIXE, DBConstantes.Tables.GROUP_PREFIXE, DBConstantes.Tables.SEGMENT_PREFIXE, DBConstantes.Tables.PRODUCT_PREFIXE, beginByAnd, access));
            }
            catch (System.Exception e)
            {
                throw (e);
            }
        }


        /// <summary>
        /// Donne la condition SQL pour int�grer la notion d'univers Adexpress
        /// </summary>
        /// <param name="idProductItemsList">Identifiant de l'univers produit AdExpress</param>
        /// <param name="sectorTablePrefixe">Pr�fixe de la table sector</param>
        /// <param name="subSectorTablePrefixe">Pr�fixe de la table subSector</param>
        /// <param name="groupTablePrefixe">Pr�fixe de la table group</param>
        /// <param name="segmentTablePrefixe">Pr�fixe de la table segment </param>
        /// <param name="productTablePrefixe">Pr�fixe de la table produit</param>			
        /// <param name="beginByAnd">La condition doit elle commencer par And</param>
        /// <param name="access">si true condition en acc�s sinon en exclusion</param>
        /// <returns>Condition</returns>
        /// <remarks>
        /// Cette m�thode doit �tre utilis�e que si la nomenclature support n'est pas contenue
        /// dans une m�me table (famille, classe ,group, produit ).
        /// </remarks>
        internal static string getAdExpressProductUniverseCondition(int idProductItemsList, string sectorTablePrefixe, string subSectorTablePrefixe, string groupTablePrefixe, string segmentTablePrefixe, string productTablePrefixe, bool beginByAnd, bool access)
        {
            try
            {
                string sql = " ";

				ProductItemsList prList = null; ;
				if (Product.Contains(idProductItemsList) && (prList = Product.GetItemsList(idProductItemsList)) != null) {
					string condition = "";

					if (access) {
						condition = "in";
					}
					else {
						condition = " not in";
					}

					if (beginByAnd) sql += "And ";
					ProductItemsList adexpressProductItemsList = Product.GetItemsList(idProductItemsList);
					if (adexpressProductItemsList.GetSectorItemsList.Length > 0) sql += sectorTablePrefixe + ".id_sector " + condition + " (" + adexpressProductItemsList.GetSectorItemsList + ") ";
					if (adexpressProductItemsList.GetSubSectorItemsList.Length > 0) sql += subSectorTablePrefixe + ".id_subSector " + condition + " (" + adexpressProductItemsList.GetSubSectorItemsList + ") ";
					if (adexpressProductItemsList.GetGroupItemsList.Length > 0) sql += groupTablePrefixe + ".id_group_ " + condition + "(" + adexpressProductItemsList.GetGroupItemsList + ") ";
					if (adexpressProductItemsList.GetSegmentItemsList.Length > 0) sql += segmentTablePrefixe + ".id_segment " + condition + "(" + adexpressProductItemsList.GetSegmentItemsList + ") ";
					if (adexpressProductItemsList.GetProductItemsList.Length > 0) sql += productTablePrefixe + ".id_product " + condition + "(" + adexpressProductItemsList.GetProductItemsList + ") ";
					if (sql.Length < 6) return ("");
				}
                return (sql);
            }
            catch (System.Exception e)
            {
                throw (new SQLGeneratorException("Impossible de d�terminer les conditions d'univers AdExpress: " , e));
            }
        }


        #endregion

        #region D�terminer la table,champs,... � traiter

        #region champ � utiliser pour la s�lection d'unit�
        /// <summary>
        /// D�termine le nom du champ � utiliser pour l'unit�
        /// 
        /// Nouvelle version: 28/11/2006
        /// </summary>
        /// <remarks>
        /// A utiliser pour les modules d'alerte et d'analyse
        /// </remarks>
        /// <param name="webSession">Session du client</param>
        /// <returns>Nom du champ � utiliser pour la s�lection de dates</returns>
        [Obsolete("Use GetUnitFieldName with paramter type !!!")]
        public static string GetUnitFieldName(WebSession webSession)
        {
            WebConstantes.Module.Type moduleType = (ModulesList.GetModule(webSession.CurrentModule)).ModuleType;
            switch (moduleType)
            {
                case WebConstantes.Module.Type.tvSponsorship:
                case WebConstantes.Module.Type.alert:
                    try {
                        return webSession.GetSelectedUnit().DatabaseField;
                    }
                    catch {
                        throw new SQLGeneratorException("Not managed unit (Alert Module)");
                    }
                case WebConstantes.Module.Type.analysis:
                    try {
                        return webSession.GetSelectedUnit().DatabaseMultimediaField;
                    }
                    catch {
                        throw (new SQLGeneratorException("Not managed unit (Analysis Module)"));
                    }
                default:
                    throw (new SQLGeneratorException("The type of module is not managed for the selection of unit"));
            }
        }
        /// <summary>
        /// D�termine le nom du champ � utiliser pour l'unit�
        /// 
        /// Nouvelle version: 25/10/2007
        /// </summary>
        /// <remarks>
        /// A utiliser pour diff�rencier entre le type des tables (DATA_VEHICLE ou WEB_PLAN)
        /// </remarks>
        /// <param name="webSession">Session du client</param>
        /// <param name="type">Type de la table</param>
        /// <returns>Nom du champ � utiliser pour la s�lection de dates</returns>
        public static string GetUnitFieldName(WebSession webSession, DBConstantes.TableType.Type type)
        {
            switch (type)
            {
                case DBConstantes.TableType.Type.dataVehicle4M:
                case DBConstantes.TableType.Type.dataVehicle:
                    try {
                        return webSession.GetSelectedUnit().DatabaseField;
                    }
                    catch {
                        throw new SQLGeneratorException("Not managed unit (Alert Module)");
                    }
                case DBConstantes.TableType.Type.webPlan:
                    try {
                        return webSession.GetSelectedUnit().DatabaseMultimediaField;
                    }
                    catch {
                        throw (new SQLGeneratorException("Not managed unit (Analysis Module)"));
                    }                    
                default:
                    throw (new SQLGeneratorException("The type of module is not managed for the selection of unit"));
            }
        }

        /// <summary>
        /// D�termine le nom du champ � utiliser pour l'unit�
        /// 
        /// Nouvelle version: 25/10/2007
        /// </summary>
        /// <remarks>
        /// A utiliser pour diff�rencier entre le type des tables (DATA_VEHICLE ou WEB_PLAN)
        /// </remarks>
        /// <param name="webSession">Session du client</param>
        /// <param name="type">Type de la table</param>
        /// <returns>Nom du champ � utiliser pour la s�lection de dates</returns>
        public static string GetUnitFieldNameWithAlias(WebSession webSession, DBConstantes.TableType.Type type) {
            switch (type) {
                case DBConstantes.TableType.Type.dataVehicle4M:
                case DBConstantes.TableType.Type.dataVehicle:
                    try {
                        UnitInformation unitInformation = webSession.GetSelectedUnit();
                        return unitInformation.DatabaseField + " as " + unitInformation.Id.ToString();
                    }
                    catch {
                        throw new SQLGeneratorException("Not managed unit (Alert Module)");
                    }
                case DBConstantes.TableType.Type.webPlan:
                    try {
                        UnitInformation unitInformation = webSession.GetSelectedUnit();
                        return unitInformation.DatabaseMultimediaField + " as " + unitInformation.Id.ToString();
                    }
                    catch {
                        throw (new SQLGeneratorException("Not managed unit (Analysis Module)"));
                    }
                default:
                    throw (new SQLGeneratorException("The type of module is not managed for the selection of unit"));
            }
        }

        /// <summary>
        /// D�termine le nom du champ � utiliser pour l'unit�
        /// 
        /// Nouvelle version: 25/10/2007
        /// </summary>
        /// <remarks>
        /// A utiliser pour diff�rencier entre le type des tables (DATA_VEHICLE ou WEB_PLAN)
        /// </remarks>
        /// <param name="webSession">Session du client</param>
        /// <param name="type">Type de la table</param>
        /// <returns>Nom du champ � utiliser pour la s�lection de dates</returns>
        public static string GetUnitFieldNameSumWithAlias(WebSession webSession, DBConstantes.TableType.Type type) {
            return GetUnitFieldNameSumWithAlias(webSession, type, string.Empty);
        }

        /// <summary>
        /// D�termine le nom du champ � utiliser pour l'unit�
        /// 
        /// Nouvelle version: 25/10/2007
        /// </summary>
        /// <remarks>
        /// A utiliser pour diff�rencier entre le type des tables (DATA_VEHICLE ou WEB_PLAN)
        /// </remarks>
        /// <param name="webSession">Session du client</param>
        /// <param name="type">Type de la table</param>
        /// <param name="prefixe">Prefixe of field</param>
        /// <returns>Nom du champ � utiliser pour la s�lection de dates</returns>
        public static string GetUnitFieldNameSumWithAlias(WebSession webSession, DBConstantes.TableType.Type type, string prefixe)
        {
            StringBuilder sql = new StringBuilder();
            if (prefixe != null && prefixe.Length > 0)
                prefixe += ".";
            else
                prefixe = "";
            switch (type)
            {
                case DBConstantes.TableType.Type.dataVehicle4M:
                case DBConstantes.TableType.Type.dataVehicle:
                    try
                    {
                        UnitInformation unitInformation = webSession.GetSelectedUnit();
                        if (unitInformation.Id != WebConstantes.CustomerSessions.Unit.versionNb)
                        {
                            sql.AppendFormat("sum({0}{1}) as {2}", prefixe, unitInformation.DatabaseField, unitInformation.Id.ToString());
                        }
                        else
                        {
                            sql.AppendFormat("{0}{1} as {2}", prefixe, unitInformation.DatabaseField, unitInformation.Id.ToString());
                        }
                        return sql.ToString();
                    }
                    catch
                    {
                        throw new SQLGeneratorException("Not managed unit (Alert Module)");
                    }
                case DBConstantes.TableType.Type.webPlan:
                    try
                    {
                        UnitInformation unitInformation = webSession.GetSelectedUnit();
                        if (unitInformation.Id != WebConstantes.CustomerSessions.Unit.versionNb)
                        {
                            sql.AppendFormat("sum({0}{1}) as {2}", prefixe, unitInformation.DatabaseMultimediaField, unitInformation.Id.ToString());
                        }
                        else
                        {
                            sql.AppendFormat("{0}{1} as {2}", prefixe, unitInformation.DatabaseMultimediaField, unitInformation.Id.ToString());
                        }
                        return sql.ToString();
                    }
                    catch
                    {
                        throw (new SQLGeneratorException("Not managed unit (Analysis Module)"));
                    }
                default:
                    throw (new SQLGeneratorException("The type of module is not managed for the selection of unit"));
            }
        }

        /// <summary>
        /// D�termine le nom du champ � utiliser pour l'unit�
        /// 
        /// Nouvelle version: 25/10/2007
        /// </summary>
        /// <remarks>
        /// A utiliser pour diff�rencier entre le type des tables (DATA_VEHICLE ou WEB_PLAN)
        /// </remarks>
        /// <param name="webSession">Session du client</param>
        /// <param name="type">Type de la table</param>
        /// <returns>Nom du champ � utiliser pour la s�lection de dates</returns>
        public static string GetUnitFieldNameSumUnionWithAlias(WebSession webSession) {
            try {
                StringBuilder sql = new StringBuilder();
                UnitInformation u = webSession.GetSelectedUnit();
                if (u.Id != WebConstantes.CustomerSessions.Unit.versionNb)
                {
                    sql.AppendFormat("sum({0}) as {0}", u.Id.ToString());
                }
                else
                {
                    sql.AppendFormat("{0} as {0}", u.Id.ToString());
                }
                return sql.ToString();
            }
            catch {
                throw new SQLGeneratorException("Not managed unit (Alert Module)");
            }
        }

        /// <summary>
        /// D�termine le nom du champ � utiliser pour l'unit�
        /// 
        /// Nouvelle version: 25/10/2007
        /// </summary>
        /// <remarks>
        /// A utiliser pour diff�rencier entre le type des tables (DATA_VEHICLE ou WEB_PLAN)
        /// </remarks>
        /// <param name="webSession">Session du client</param>
        /// <param name="type">Type de la table</param>
        /// <returns>Nom du champ � utiliser pour la s�lection de dates</returns>
        public static string GetUnitAlias(WebSession webSession) {
            try {
                return webSession.GetSelectedUnit().Id.ToString();
            }
            catch {
                throw new SQLGeneratorException("Not managed unit (Alert Module)");
            }
        }

        /// <summary>
        /// D�termine le nom du champ � utiliser pour l'unit�
        /// 
        /// Nouvelle version: 25/10/2007
        /// </summary>
        /// <remarks>
        /// A utiliser pour diff�rencier entre le type des tables (DATA_VEHICLE ou WEB_PLAN)
        /// </remarks>
        /// <param name="webSession">Session du client</param>
        /// <param name="type">Type de la table</param>
        /// <returns>Nom du champ � utiliser pour la s�lection de dates</returns>
        public static string GetUnitAliasSum(WebSession webSession) {
            try {
                return "sum(" + webSession.GetSelectedUnit().Id.ToString() + ")";
            }
            catch {
                throw new SQLGeneratorException("Not managed unit (Alert Module)");
            }
        }
        #endregion

        #region champ � utiliser pour la s�lection de dates

        /// <summary>
        /// D�termine le nom du champ � utiliser pour la s�lection de dates
        /// 
        /// Nouvelle version: 28/11/2006
        /// </summary>
        /// <remarks>
        /// A utiliser pour les modules d'alerte et d'analyse
        /// </remarks>
        /// <param name="webSession">Session du client</param>
        /// <returns>Nom du champ � utiliser pour la s�lection de dates</returns>
        public static string GetDateFieldName(WebSession webSession)
        {
            WebConstantes.Module.Type moduleType = (ModulesList.GetModule(webSession.CurrentModule)).ModuleType;
            switch (moduleType)
            {
                case WebConstantes.Module.Type.tvSponsorship:
                case WebConstantes.Module.Type.alert:
                    return (DBConstantes.Fields.DATE_MEDIA_NUM);
                case WebConstantes.Module.Type.analysis:
                    switch (webSession.DetailPeriod)
                    {
                        case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
                            return (DBConstantes.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD);
                        case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
                            return (DBConstantes.Fields.WEB_PLAN_MEDIA_WEEK_DATE_FIELD);
                        default:
                            throw (new SQLGeneratorException("Le d�tails p�riode s�lectionn� est incorrect pour le choix du champ"));
                    }
                default:
                    throw (new SQLGeneratorException("Le d�tails p�riode s�lectionn� est incorrect pour le choix du champ"));
            }
        }

        /// <summary>
        /// D�termine le nom du champ � utiliser pour la s�lection de dates
        /// 
        /// Nouvelle version: 28/11/2006
        /// </summary>
        /// <remarks>
        /// A utiliser pour les modules d'alerte et d'analyse
        /// </remarks>
        /// <param name="moduleType">Type de module du client</param>
        /// <param name="detailPeriod">Type de d�tail de la p�riode</param>
        /// <returns>Nom du champ � utiliser pour la s�lection de dates</returns>
        public static string GetDateFieldName(WebConstantes.Module.Type moduleType, WebConstantes.CustomerSessions.Period.DisplayLevel detailPeriod)
        {

            switch (moduleType)
            {
                case WebConstantes.Module.Type.tvSponsorship:
                case WebConstantes.Module.Type.alert:
                    return (DBConstantes.Fields.DATE_MEDIA_NUM);
                case WebConstantes.Module.Type.analysis:
                    switch (detailPeriod)
                    {
                        case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
                            return (DBConstantes.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD);
                        case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
                            return (DBConstantes.Fields.WEB_PLAN_MEDIA_WEEK_DATE_FIELD);
                        default:
                            throw (new SQLGeneratorException("Le d�tails p�riode s�lectionn� est incorrect pour le choix du champ"));
                    }
                    break;
                default:
                    throw (new SQLGeneratorException("Le d�tails p�riode s�lectionn� est incorrect pour le choix du champ"));
            }
        }

        /// <summary>
        /// Get Field to use for date
        /// </summary>
        /// <param name="period">Type of period</param>
        /// <returns>Date Filed Name matchnig the type of period</returns>
        public static string GetDateFieldName(CstPeriod.PeriodBreakdownType period) {
            switch (period) {
                case CstPeriod.PeriodBreakdownType.month:
                    return (DBConstantes.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD);
                case CstPeriod.PeriodBreakdownType.week:
                    return (DBConstantes.Fields.WEB_PLAN_MEDIA_WEEK_DATE_FIELD);
                case CstPeriod.PeriodBreakdownType.data:
                case CstPeriod.PeriodBreakdownType.data_4m:
                    return (DBConstantes.Fields.DATE_MEDIA_NUM);
                default:
                    throw (new SQLGeneratorException("Selected detail period is uncorrect. Unable to determine date field to use."));
            }
        }
        #endregion

        #region Recap (Analyses sectorielles)
        /// <summary>
        /// D�termine la table � traiter pour une Analyse sectorielle en fonction du vehicle
        /// </summary>
        /// <param name="vehicleType">Vehicle</param>
        /// <returns>Nom de la table</returns>
        public static string getVehicleTableNameForSectorAnalysisResult(ClassificationConstantes.DB.Vehicles.names vehicleType, bool isRetailerSelected)
        {
            switch (vehicleType)
            {
				case ClassificationConstantes.DB.Vehicles.names.cinema:
                    return WebApplicationParameters.GetDataTable(TableIds.recapCinema, isRetailerSelected).Sql;
				case ClassificationConstantes.DB.Vehicles.names.radio:
                    return WebApplicationParameters.GetDataTable(TableIds.recapRadio, isRetailerSelected).Sql;
				case ClassificationConstantes.DB.Vehicles.names.tv:
                    return WebApplicationParameters.GetDataTable(TableIds.recapTv, isRetailerSelected).Sql;
                case ClassificationConstantes.DB.Vehicles.names.newspaper:
                    return WebApplicationParameters.GetDataTable(TableIds.recapNewspaper, isRetailerSelected).Sql;
                case ClassificationConstantes.DB.Vehicles.names.magazine:
                    return WebApplicationParameters.GetDataTable(TableIds.recapMagazine, isRetailerSelected).Sql;
				case ClassificationConstantes.DB.Vehicles.names.press:
                    return WebApplicationParameters.GetDataTable(TableIds.recapPress, isRetailerSelected).Sql;
				case ClassificationConstantes.DB.Vehicles.names.outdoor:
                    return WebApplicationParameters.GetDataTable(TableIds.recapOutDoor, isRetailerSelected).Sql;
                case ClassificationConstantes.DB.Vehicles.names.instore:
                    return WebApplicationParameters.GetDataTable(TableIds.recapInStore, isRetailerSelected).Sql;
                case ClassificationConstantes.DB.Vehicles.names.indoor:
                    return WebApplicationParameters.GetDataTable(TableIds.recapInDoor, isRetailerSelected).Sql;
                case DBClassificationConstantes.Vehicles.names.adnettrack:
                case DBClassificationConstantes.Vehicles.names.czinternet:
                case ClassificationConstantes.DB.Vehicles.names.internet:
                    return WebApplicationParameters.GetDataTable(TableIds.recapInternet, isRetailerSelected).Sql;
				case ClassificationConstantes.DB.Vehicles.names.plurimedia:
                    return WebApplicationParameters.GetDataTable(TableIds.recapPluri, isRetailerSelected).Sql;
				case ClassificationConstantes.DB.Vehicles.names.mediasTactics:
                    return WebApplicationParameters.GetDataTable(TableIds.recapTactic, isRetailerSelected).Sql;
				case ClassificationConstantes.DB.Vehicles.names.mobileTelephony:
                    return WebApplicationParameters.GetDataTable(TableIds.recapMobileTel, isRetailerSelected).Sql;
				case ClassificationConstantes.DB.Vehicles.names.emailing:
                    return WebApplicationParameters.GetDataTable(TableIds.recapEmailing, isRetailerSelected).Sql;
				case ClassificationConstantes.DB.Vehicles.names.directMarketing:
                    return WebApplicationParameters.GetDataTable(TableIds.recapDirectMarketing, isRetailerSelected).Sql;
                case ClassificationConstantes.DB.Vehicles.names.mms:
                    return WebApplicationParameters.GetDataTable(TableIds.recapMms, isRetailerSelected).Sql;
                case ClassificationConstantes.DB.Vehicles.names.search:
                    return WebApplicationParameters.GetDataTable(TableIds.recapSearch, isRetailerSelected).Sql;
				default:
					throw (new SQLGeneratorException("Impossible de d�terminer la table recap � traiter."));
            }
        }

        /// <summary>
        /// D�termine la table media/segment � traiter pour une Analyse sectorielle en fonction du vehicle  
        /// </summary>
        /// <param name="vehicleType">Vehicle</param>
        /// <returns>Nom de la table</returns>
        public static string getVehicleTableNameForSectorAnalysisResultSegmentLevel(ClassificationConstantes.DB.Vehicles.names vehicleType, bool isRetailerSelected)
        {
            switch (vehicleType)
            {
                case ClassificationConstantes.DB.Vehicles.names.cinema:
                    return WebApplicationParameters.GetDataTable(TableIds.recapCinemaSegment, isRetailerSelected).Sql;                    
                case ClassificationConstantes.DB.Vehicles.names.radio:
                    return WebApplicationParameters.GetDataTable(TableIds.recapRadioSegment, isRetailerSelected).Sql;                    
                case ClassificationConstantes.DB.Vehicles.names.tv:
                    return WebApplicationParameters.GetDataTable(TableIds.recapTvSegment, isRetailerSelected).Sql;                    
                case ClassificationConstantes.DB.Vehicles.names.press:
                    return WebApplicationParameters.GetDataTable(TableIds.recapPressSegment, isRetailerSelected).Sql;
                case ClassificationConstantes.DB.Vehicles.names.newspaper:
                    return WebApplicationParameters.GetDataTable(TableIds.recapNewspaperSegment, isRetailerSelected).Sql;
                case ClassificationConstantes.DB.Vehicles.names.magazine:
                    return WebApplicationParameters.GetDataTable(TableIds.recapMagazineSegment, isRetailerSelected).Sql; 
                case ClassificationConstantes.DB.Vehicles.names.outdoor:
                    return WebApplicationParameters.GetDataTable(TableIds.recapOutDoorSegment, isRetailerSelected).Sql;
                case ClassificationConstantes.DB.Vehicles.names.indoor:
                    return WebApplicationParameters.GetDataTable(TableIds.recapInDoorSegment, isRetailerSelected).Sql;
                case ClassificationConstantes.DB.Vehicles.names.instore:
                    return WebApplicationParameters.GetDataTable(TableIds.recapInStoreSegment, isRetailerSelected).Sql;
                case DBClassificationConstantes.Vehicles.names.czinternet:
                case ClassificationConstantes.DB.Vehicles.names.internet:
                    return WebApplicationParameters.GetDataTable(TableIds.recapInternetSegment, isRetailerSelected).Sql;                    
                case ClassificationConstantes.DB.Vehicles.names.plurimedia:
                    return WebApplicationParameters.GetDataTable(TableIds.recapPluriSegment, isRetailerSelected).Sql;                    
                case ClassificationConstantes.DB.Vehicles.names.mediasTactics:
                    return WebApplicationParameters.GetDataTable(TableIds.recapTacticSegment, isRetailerSelected).Sql;                    
                case ClassificationConstantes.DB.Vehicles.names.mobileTelephony:
                    return WebApplicationParameters.GetDataTable(TableIds.recapMobileTelSegment, isRetailerSelected).Sql;                    
                case ClassificationConstantes.DB.Vehicles.names.emailing:
                    return WebApplicationParameters.GetDataTable(TableIds.recapEmailingSegment, isRetailerSelected).Sql;
				case ClassificationConstantes.DB.Vehicles.names.directMarketing:
                    return WebApplicationParameters.GetDataTable(TableIds.recapDirectMarketingSegment, isRetailerSelected).Sql;
                case ClassificationConstantes.DB.Vehicles.names.mms:
                    return WebApplicationParameters.GetDataTable(TableIds.recapMmsSegment, isRetailerSelected).Sql;
                case ClassificationConstantes.DB.Vehicles.names.search:
                    return WebApplicationParameters.GetDataTable(TableIds.recapSearchSegment, isRetailerSelected).Sql;
                default:
                    throw (new SQLGeneratorException("Impossible de d�terminer la table recap � traiter."));
            }
        }


        #endregion

        #region Gad

        /// <summary>
        /// D�termine la table contenant les adresses Gad des annonceurs
        /// </summary>
        /// <exception cref="TNS.AdExpress.Web.Exceptions.SQLGeneratorException">Le niveau de d�tail produit demand� ne g�re pas les donn�es du gad</exception>
        /// <param name="webSession">Session du client</param>
        /// <returns>Nom de la table</returns>
        public static string GetTablesForGad(WebSession webSession)
        {
            return (DBConstantes.Tables.GAD);
        }

        /// <summary>
        /// D�termine le champ addresse du gad
        /// </summary>
        /// <returns>Champ addresse du gad</returns>
        public static string GetFieldsAddressForGad()
        {
            return (GetFieldsAddressForGad(WebApplicationParameters.DataBaseDescription.GetTable(TableIds.gad).Prefix));
        }

        /// <summary>
        /// D�termine la table contenant les adresses Gad des annonceurs
        /// </summary>
        /// <param name="prefixe">Pr�fixe � utiliser pour la table du GAD</param>
        /// <returns>Champ addresse du gad</returns>
        public static string GetFieldsAddressForGad(string prefixe)
        {
            return (prefixe.Length > 0) ? prefixe + ".id_address" : " id_address";
        }

        /// <summary>
        /// D�termine les jointures � utiliser pour avoir l'adresse du gad pour un annonceur
        /// </summary>
        /// <param name="prefixeData">Pr�fixe � utiliser pour la de r�sultat</param>
        /// <returns>Jointure</returns>
        public static string GetJointForGad(string prefixeData)
        {
            return (GetJointForGad(WebApplicationParameters.DataBaseDescription.GetTable(TableIds.gad).Prefix, prefixeData));

        }

        /// <summary>
        /// D�termine les jointures � utiliser pour avoir l'adresse du gad pour un annonceur
        /// </summary>
        /// <param name="prefixeGAD">Pr�fixe � utiliser pour la table du GAD</param>
        /// <param name="prefixeData">Pr�fixe � utiliser pour la de r�sultat</param>
        /// <returns>Jointure</returns>
        public static string GetJointForGad(string prefixeGAD, string prefixeData)
        {
            return (" " + prefixeGAD + ".id_advertiser(+)=" + prefixeData + ".id_advertiser ");

        }
        #endregion

        #region Analyses
        /// <summary>
        /// Indique la table � utilis�e pour la requ�te
        /// </summary>
        /// <param name="periodType">Type de p�riode</param>
        /// <returns>La table correspondant au type de p�riode</returns>
        public static string getTableNameForAnalysisResult(WebConstantes.CustomerSessions.Period.DisplayLevel periodType)
        {
            switch (periodType)
            {
                case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
                    return (DBConstantes.Tables.WEB_PLAN_MEDIA_MONTH);
                case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
                    return (DBConstantes.Tables.WEB_PLAN_MEDIA_WEEK);
                default:
                    throw (new SQLGeneratorException("Le d�tails p�riode s�lectionn� est incorrect pour le choix de la table"));
            }
        }

        /// <summary>
        /// Indique le champ � utilis�e pour la date dans la requ�te
        /// </summary>
        /// <param name="periodType">Type de p�riode</param>
        /// <returns>Le champ correspondant au type de p�riode</returns>
        public static string getDateFieldNameForAnalysisResult(WebConstantes.CustomerSessions.Period.DisplayLevel periodType)
        {
            switch (periodType)
            {
                case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
                    return (DBConstantes.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD);
                case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
                    return (DBConstantes.Fields.WEB_PLAN_MEDIA_WEEK_DATE_FIELD);
                default:
                    throw (new SQLGeneratorException("Le d�tails p�riode s�lectionn� est incorrect pour le choix du champ"));
            }
        }

        /// <summary>
        /// Indique le champ � utilis�e pour l'unit� dans la requ�te
        /// </summary>
        /// <param name="unit">Type d'unit�</param>
        /// <returns>Le champ correspondant au type d'unit�</returns>
        internal static string getUnitFieldNameForAnalysisResult(WebConstantes.CustomerSessions.Unit unit)
        {
            try {
                return UnitsInformation.Get(unit).DatabaseMultimediaField;
            }
            catch {
                throw (new SQLGeneratorException("Details unit chosen am wrong for the choice of the field"));
            }
        }

        #endregion

        #region Tableaux de bord

        /// <summary>
        /// Indique la table � utilis�e pour la requ�te des donn�es aggr�g�es
        /// </summary>
        /// <param name="periodType">Type de p�riode</param>
        /// <returns>La table correspondant au type de p�riode</returns>
        internal static string getTableNameForAggregatedData(WebConstantes.CustomerSessions.Period.DisplayLevel periodType)
        {
            switch (periodType)
            {
                case WebConstantes.CustomerSessions.Period.DisplayLevel.yearly:
                case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
                    return (DBConstantes.Tables.WEB_PLAN_MEDIA_MONTH);
                case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
                    return (DBConstantes.Tables.WEB_PLAN_MEDIA_WEEK);
                default:
                    throw (new SQLGeneratorException("Le d�tails p�riode s�lectionn� est incorrect pour le choix de la table"));
            }
        }

        /// <summary>
        /// Indique la table � utilis�e pour la requ�te
        /// </summary>
        /// <param name="periodType">Type de p�riode</param>
        /// <returns>La table correspondant au type de p�riode</returns>
        public static string getTableNameForDashBoardResult(WebConstantes.CustomerSessions.Period.DisplayLevel periodType)
        {
            //	return (getTableNameForAnalysisResult(periodType));
            switch (periodType)
            {
                case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
                    return (DBConstantes.Tables.WEB_PLAN_MEDIA_MONTH);
                case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
                case WebConstantes.CustomerSessions.Period.DisplayLevel.yearly:
                    return (DBConstantes.Tables.WEB_PLAN_MEDIA_WEEK);
                default:
                    throw (new SQLGeneratorException("Le d�tails p�riode s�lectionn� est incorrect pour le choix de la table"));
            }
        }

        #region Tendances
        /// <summary>
        /// 
        /// </summary>
        /// <param name="periodType"></param>
        /// <returns></returns>
        public static string GetTableNameForTendency(WebConstantes.CustomerSessions.Period.DisplayLevel periodType, bool isRetailerSelected)
        {
            switch (periodType)
            {
                case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
                case WebConstantes.CustomerSessions.Period.DisplayLevel.yearly:
                    return (WebApplicationParameters.GetSqlDataTableLabelWithPrefix(TableIds.tendencyMonth, isRetailerSelected));
                case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
                    return (WebApplicationParameters.GetSqlDataTableLabelWithPrefix(TableIds.tendencyWeek, isRetailerSelected));
                default:
                    throw (new SQLGeneratorException("Le d�tails p�riode s�lectionn� est incorrect pour le choix de la table"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="periodType"></param>
        /// <returns></returns>
        public static Table GetTrendTableInformation(WebConstantes.CustomerSessions.Period.DisplayLevel periodType, bool isRetailerSelected)
        {
            switch (periodType)
            {
                case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
                case WebConstantes.CustomerSessions.Period.DisplayLevel.yearly:
                    return (WebApplicationParameters.GetDataTable(TableIds.tendencyMonth, isRetailerSelected));
                case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
                    return (WebApplicationParameters.GetDataTable(TableIds.tendencyWeek, isRetailerSelected));
                default:
                    throw (new SQLGeneratorException("Le d�tails p�riode s�lectionn� est incorrect pour le choix de la table"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="periodType"></param>
        /// <returns></returns>
        public static Table GetTrendTableInformation(WebConstantes.CustomerSessions.Period.DisplayLevel periodType)
        {
            switch (periodType)
            {
                case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
                case WebConstantes.CustomerSessions.Period.DisplayLevel.yearly:
                    return (WebApplicationParameters.DataBaseDescription.GetTable(TableIds.tendencyMonth));
                case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
                    return (WebApplicationParameters.DataBaseDescription.GetTable(TableIds.tendencyWeek));
                default:
                    throw (new SQLGeneratorException("Le d�tails p�riode s�lectionn� est incorrect pour le choix de la table"));
            }
        }

        /// <summary>
        /// Get table description for trends total according to the period selected by the client
        /// </summary>
        /// <param name="periodType">Period type selected</param>
        /// <returns>Tablz description</returns>
        public static Table GetTrendTotalTableInformtation(WebConstantes.CustomerSessions.Period.DisplayLevel periodType, bool isRetailerSelected) {
            switch(periodType) {
                case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
                case WebConstantes.CustomerSessions.Period.DisplayLevel.yearly:
                    return (WebApplicationParameters.GetDataTable(TableIds.totalTendencyMonth, isRetailerSelected));
                case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
                    return (WebApplicationParameters.GetDataTable(TableIds.totalTendencyWeek, isRetailerSelected));
                default:
                    throw (new SQLGeneratorException("Le d�tails p�riode s�lectionn� est incorrect pour le choix de la table"));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="periodType"></param>
        /// <returns></returns>
        [Obsolete("This method is obsolete; use GetTrendTotalTableInformtation")]
        public static string GetTotalTableNameForTendency(WebConstantes.CustomerSessions.Period.DisplayLevel periodType, bool isRetailerSelected)
        {
            switch (periodType)
            {
                case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
                case WebConstantes.CustomerSessions.Period.DisplayLevel.yearly:
                    return (WebApplicationParameters.GetSqlDataTableLabelWithPrefix(TableIds.totalTendencyMonth, isRetailerSelected));
                case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
                    return (WebApplicationParameters.GetSqlDataTableLabelWithPrefix(TableIds.totalTendencyWeek, isRetailerSelected));
                default:
                    throw (new SQLGeneratorException("Le d�tails p�riode s�lectionn� est incorrect pour le choix de la table"));
            }
        }

        /// <summary>
        /// Liste des champs pour le module tendance
        /// </summary>
        /// <param name="vehicleName">Nom du m�dia</param>
        /// <returns></returns>
        public static string getMediaFieldsForTendencies(DBClassificationConstantes.Vehicles.names vehicleName)
        {
            switch (vehicleName)
            {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    return "" + DBConstantes.Tables.WEB_PLAN_PREFIXE + ".id_category," + DBConstantes.Tables.CATEGORY_PREFIXE + ".category"
                        + " ," + DBConstantes.Tables.TITLE_PREFIXE + ".id_title as id_media," + DBConstantes.Tables.TITLE_PREFIXE + ".title as media";
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.others:
                    return "" + DBConstantes.Tables.WEB_PLAN_PREFIXE + ".id_category," + DBConstantes.Tables.CATEGORY_PREFIXE + ".category"
                        + " ," + DBConstantes.Tables.MEDIA_PREFIXE + ".id_media as id_media," + DBConstantes.Tables.MEDIA_PREFIXE + ".media as media";
                default:
                    throw (new SQLGeneratorException("Impossible de d�terminer la table � traiter"));
            }
        }


        /// <summary>
        /// Tables pour le module tendance
        /// </summary>
        /// <param name="vehicleName">Nom du vehicle</param>
        /// <returns>liste des tables</returns>
        public static string getTableForTendencies(DBClassificationConstantes.Vehicles.names vehicleName)
        {
            switch (vehicleName)
            {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    return " " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".media " + DBConstantes.Tables.MEDIA_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".title " + DBConstantes.Tables.TITLE_PREFIXE + ""
                            + " ," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".category " + DBConstantes.Tables.CATEGORY_PREFIXE + "";
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.others:
                    return " " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".media " + DBConstantes.Tables.MEDIA_PREFIXE + ""
                            + " ," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".category " + DBConstantes.Tables.CATEGORY_PREFIXE + "";
                default:
                    throw (new SQLGeneratorException("Impossible de d�terminer la table � traiter"));
            }
        }

        /// <summary>
        /// Choix p�riode
        /// </summary>
        /// <param name="periodType">type de semaine (hebdomadaire,mensuel)</param>
        /// <param name="firstPeriodN">d�but de p�riode de l'ann�e N</param>
        /// <param name="endPeriodN">fin de p�riode de l'ann�e N</param>
        /// <param name="firstPeriodN1">d�but de p�riode de l'ann�e N-1</param>
        /// <param name="endPeriodN1">fin de p�riode de l'ann�e N-1</param>
        /// <returns>Conditions sur la p�riode</returns>
        public static string getPeriodForTendencies(WebConstantes.CustomerSessions.Period.DisplayLevel periodType, string firstPeriodN, string endPeriodN, string firstPeriodN1, string endPeriodN1)
        {
            switch (periodType)
            {
                case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
                    return " and (( MONTH_MEDIA_NUM>=" + firstPeriodN + " and  MONTH_MEDIA_NUM<=" + endPeriodN + " ) or ( MONTH_MEDIA_NUM>=" + firstPeriodN1 + " "
                        + "and  MONTH_MEDIA_NUM<=" + endPeriodN1 + "  ) )";
                case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
                case WebConstantes.CustomerSessions.Period.DisplayLevel.yearly:
                    return " and (( WEEK_MEDIA_NUM>=" + firstPeriodN + " and  WEEK_MEDIA_NUM<=" + endPeriodN + " ) or ( WEEK_MEDIA_NUM>=" + firstPeriodN1 + " "
                        + "and  WEEK_MEDIA_NUM<=" + endPeriodN1 + "  ) )";
                default:
                    throw (new SQLGeneratorException("Le d�tails p�riode s�lectionn� est incorrect pour le choix de la table"));
            }
        }

        /// <summary>
        /// Jointure pour le module
        /// </summary>
        /// <param name="webSession">Session client</param>
        /// <param name="vehicleName">Nom du vehicle</param>
        /// <returns></returns>
        public static string getJointForTendencies(WebSession webSession, DBClassificationConstantes.Vehicles.names vehicleName)
        {
            switch (vehicleName)
            {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    return " and " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_title=" + DBConstantes.Tables.TITLE_PREFIXE + ".id_title"
                            + " and " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_media=" + DBConstantes.Tables.WEB_PLAN_PREFIXE + ".id_media"
                            + " and " + DBConstantes.Tables.CATEGORY_PREFIXE + ".id_category=" + DBConstantes.Tables.WEB_PLAN_PREFIXE + ".id_category"
                            + " and " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_language=" + webSession.DataLanguage
                            + " and " + DBConstantes.Tables.TITLE_PREFIXE + ".id_language=" + webSession.DataLanguage
                            + " and " + DBConstantes.Tables.CATEGORY_PREFIXE + ".id_language=" + webSession.DataLanguage
                            + " and " + DBConstantes.Tables.MEDIA_PREFIXE + ".activation<" + DBConstantes.ActivationValues.UNACTIVATED
                            + " and " + DBConstantes.Tables.TITLE_PREFIXE + ".activation<" + DBConstantes.ActivationValues.UNACTIVATED
                            + " and " + DBConstantes.Tables.CATEGORY_PREFIXE + ".activation<" + DBConstantes.ActivationValues.UNACTIVATED;

                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                    return " and " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_media=" + DBConstantes.Tables.WEB_PLAN_PREFIXE + ".id_media"
                        + " and " + DBConstantes.Tables.CATEGORY_PREFIXE + ".id_category=" + DBConstantes.Tables.WEB_PLAN_PREFIXE + ".id_category"
                        + " and " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_language=" + webSession.DataLanguage
                        + " and " + DBConstantes.Tables.CATEGORY_PREFIXE + ".id_language=" + webSession.DataLanguage
                        + " and " + DBConstantes.Tables.MEDIA_PREFIXE + ".activation<" + DBConstantes.ActivationValues.UNACTIVATED
                        + " and " + DBConstantes.Tables.CATEGORY_PREFIXE + ".activation<" + DBConstantes.ActivationValues.UNACTIVATED;
                case DBClassificationConstantes.Vehicles.names.others:
                    return " and " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_media=" + DBConstantes.Tables.WEB_PLAN_PREFIXE + ".id_media"
                        + " and " + DBConstantes.Tables.CATEGORY_PREFIXE + ".id_category=" + DBConstantes.Tables.WEB_PLAN_PREFIXE + ".id_category"
                        + " and " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_language=" + webSession.DataLanguage
                        + " and " + DBConstantes.Tables.CATEGORY_PREFIXE + ".id_language=" + webSession.DataLanguage
                        + " and " + DBConstantes.Tables.MEDIA_PREFIXE + ".activation<" + DBConstantes.ActivationValues.UNACTIVATED
                        + " and " + DBConstantes.Tables.CATEGORY_PREFIXE + ".activation<" + DBConstantes.ActivationValues.UNACTIVATED
                        + " and " + DBConstantes.Tables.CATEGORY_PREFIXE + ".id_category=" + DBClassificationConstantes.panEuro.PAN_EURO_CATEGORY + "";

                default:
                    throw (new SQLGeneratorException("Impossible de d�terminer la table � traiter"));
            }

        }

        /// <summary>
        /// Group by pour le module Tendance
        /// </summary>
        /// <param name="vehicleName">Nom du Vehicle</param>
        /// <returns></returns>
        public static string getGroupByForTendencies(DBClassificationConstantes.Vehicles.names vehicleName)
        {
            switch (vehicleName)
            {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    return " group by " + DBConstantes.Tables.TITLE_PREFIXE + ".id_title,title "
                            + " ,wp.id_category "
                            + " ,category";
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.others:
                    return " group by " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_media,media "
                        + " ,wp.id_category "
                        + " ,category";
                default:
                    throw (new SQLGeneratorException("Impossible de d�terminer la table � traiter"));
            }
        }

        /// <summary>
        /// Order by pour le module tendance
        /// </summary>
        /// <param name="vehicleName"></param>
        /// <returns></returns>
        public static string getOrderByForTendencies(DBClassificationConstantes.Vehicles.names vehicleName)
        {
            switch (vehicleName)
            {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    return " order by category, title";
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.others:
                    return " order by category, media";
                default:
                    throw (new SQLGeneratorException("Impossible de d�terminer la table � traiter"));
            }
        }

        #endregion
        #endregion
        
        #region Analyse portefeuille

        /// <summary>
        /// Fournit les champs pour avoir le max et le min des dates
        /// </summary>
        /// <param name="periodType">Type de p�riode</param>
        /// <returns>les champs pour avoir le max et le min des dates</returns>
        public static string getMaxDateForPortofolio(WebConstantes.CustomerSessions.Period.DisplayLevel periodType)
        {
            switch (periodType)
            {
                case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
                    return "max(month_media_num) as lastDate,min(month_media_num) as firstDate";
                case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
                    return "max(week_media_num) as lastDate,min(week_media_num) as firstDate";
                default:
                    throw (new SQLGeneratorException("Le d�tails p�riode s�lectionn� est incorrect pour le choix de la table"));
            }
        }

        #endregion

        #region D�tails support
        /// <summary>
        /// D�termine la table � traiter pour avoir des donn�es d�sagr�g�es en fonction du vehicle
        /// </summary>
        /// <param name="vehicleName">Vehicle</param>
        /// <param name="moduleType">Type de module</param>
        /// <returns>Nom de la table</returns>
        public static string GetVehicleTableSQLForDetailResult(DBClassificationConstantes.Vehicles.names vehicleName, WebConstantes.Module.Type moduleType, bool isRetailerSelected)
        {
            try
            {
                switch (moduleType)
                {
                    case WebConstantes.Module.Type.alert:
                        return (GetVehicleTableSQLForAlertDetailResult(vehicleName, isRetailerSelected));
                    case WebConstantes.Module.Type.analysis:
                        return (GetVehicleTableSQLForZoomDetailResult(vehicleName, isRetailerSelected));
                    case WebConstantes.Module.Type.tvSponsorship:
                        return (GetVehicleTableSQLForSponsorshipResult(isRetailerSelected));

                    default:
                        throw (new SQLGeneratorException("Impossible de d�terminer le type du module"));
                }
            }
            catch (SQLGeneratorException fe)
            {
                throw (fe);
            }
        }


        /// <summary>
        /// D�termine la table � traiter pour un Zoom en fonction du vehicle
        /// </summary>
        /// <param name="vehicleName">Vehicle</param>
        /// <returns>Nom de la table</returns>
        internal static string GetVehicleTableSQLForZoomDetailResult(DBClassificationConstantes.Vehicles.names vehicleName, bool isRetailerSelected)
        {
            switch (vehicleName)
            {
                case DBClassificationConstantes.Vehicles.names.press:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataPress, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.newspaper:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataNewspaper, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.magazine:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataMagazine, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataPressInter, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.radio:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataRadio, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.others:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataTv, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.outdoor:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataOutDoor, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.indoor:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataInDoor, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.instore:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataInStore, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.adnettrack:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataAdNetTrack, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.czinternet:
                case DBClassificationConstantes.Vehicles.names.internet:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataInternet, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataMarketingDirect, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.mailValo:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataMail, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.cinema:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataCinema, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.evaliantMobile:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataEvaliantMobile, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.mms:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataMms, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.search:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataSearch, isRetailerSelected).SqlWithPrefix);
                default:
                    throw (new SQLGeneratorException("Impossible de d�terminer la table � traiter"));
            }
        }

        /// <summary>
        /// D�termine la table � traiter pour le parrainage TV
        /// </summary>
        /// <returns>Nom de la table</returns>
        internal static string GetVehicleTableSQLForSponsorshipResult(bool isRetailerSelected)
        {
            return (WebApplicationParameters.GetDataTable(TableIds.dataSponsorship, isRetailerSelected).SqlWithPrefix);
        }


        /// <summary>
        /// D�termine la table � traiter pour une Alerte en fonction du vehicle
        /// </summary>
        /// <param name="vehicleName">Vehicle</param>
        /// <returns>Nom de la table</returns>
        internal static string GetVehicleTableSQLForAlertDetailResult(DBClassificationConstantes.Vehicles.names vehicleName, bool isRetailerSelected)
        {
            switch (vehicleName)
            {
                case DBClassificationConstantes.Vehicles.names.newspaper:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataNewspaperAlert, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.magazine:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataMagazineAlert, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.press:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataPressAlert, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataPressInterAlert, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.radio:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataRadioAlert, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.others:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataTvAlert, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.outdoor:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataOutDoorAlert, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.indoor:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataInDoorAlert, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.instore:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataInStoreAlert, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.adnettrack:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataAdNetTrackAlert, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.czinternet:
                case DBClassificationConstantes.Vehicles.names.internet:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataInternetAlert, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataMarketingDirectAlert, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.mailValo:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataMailAlert, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.cinema:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataCinemaAlert, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.evaliantMobile:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataEvaliantMobileAlert, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.mms:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataMmsAlert, isRetailerSelected).SqlWithPrefix);
                case DBClassificationConstantes.Vehicles.names.search:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataSearchAlert, isRetailerSelected).SqlWithPrefix);
                default:
                    throw (new SQLGeneratorException("Impossible de d�terminer la table � traiter"));
            }
        }

        /// <summary>
        /// D�termine la table � traiter pour avoir des donn�es d�sagr�g�es en fonction du vehicle
        /// </summary>
        /// <param name="vehicleName">Vehicle</param>
        /// <param name="moduleType">Type de module</param>
        /// <returns>Nom de la table</returns>
        public static string GetVehicleTableNameForDetailResult(DBClassificationConstantes.Vehicles.names vehicleName, WebConstantes.Module.Type moduleType, bool isRetailerSelected)
        {
            try
            {
                switch (moduleType)
                {
                    case WebConstantes.Module.Type.alert:
                        return (GetVehicleTableNameForAlertDetailResult(vehicleName, isRetailerSelected));
                    case WebConstantes.Module.Type.analysis:
                        return (GetVehicleTableNameForZoomDetailResult(vehicleName, isRetailerSelected));
                    case WebConstantes.Module.Type.tvSponsorship:
                        return (GetVehicleTableNameForSponsorshipResult(isRetailerSelected));

                    default:
                        throw (new SQLGeneratorException("Impossible de d�terminer le type du module"));
                }
            }
            catch (SQLGeneratorException fe)
            {
                throw (fe);
            }
        }



        /// <summary>
        /// D�termine la table � traiter pour un Zoom en fonction du vehicle
        /// </summary>
        /// <param name="vehicleName">Vehicle</param>
        /// <returns>Nom de la table</returns>
        internal static string GetVehicleTableNameForZoomDetailResult(DBClassificationConstantes.Vehicles.names vehicleName, bool isRetailerSelected)
        {
            switch (vehicleName)
            {
                case DBClassificationConstantes.Vehicles.names.press:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataPress, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.newspaper:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataNewspaper, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.magazine:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataMagazine, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataPressInter, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.radio:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataRadio, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.others:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataTv, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.outdoor:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataOutDoor, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.indoor:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataInDoor, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.instore:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataInStore, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.adnettrack:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataAdNetTrack, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.czinternet:
                case DBClassificationConstantes.Vehicles.names.internet:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataInternet, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataMarketingDirect, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.mailValo:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataMail, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.cinema:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataCinema, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.evaliantMobile:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataEvaliantMobile, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.mms:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataMms, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.search:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataSearch, isRetailerSelected).Label);
                default:
                    throw (new SQLGeneratorException("Impossible de d�terminer la table � traiter"));
            }
        }

        /// <summary>
        /// D�termine la table � traiter pour le parrainage TV
        /// </summary>
        /// <returns>Nom de la table</returns>
        internal static string GetVehicleTableNameForSponsorshipResult(bool isRetailerSelected)
        {
            return (WebApplicationParameters.GetDataTable(TableIds.dataSponsorship, isRetailerSelected).Label);
        }


        /// <summary>
        /// D�termine la table � traiter pour une Alerte en fonction du vehicle
        /// </summary>
        /// <param name="vehicleName">Vehicle</param>
        /// <returns>Nom de la table</returns>
        public static string GetVehicleTableNameForAlertDetailResult(DBClassificationConstantes.Vehicles.names vehicleName, bool isRetailerSelected)
        {
            switch (vehicleName)
            {
                case DBClassificationConstantes.Vehicles.names.newspaper:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataNewspaperAlert, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.magazine:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataMagazineAlert, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.press:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataPressAlert, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataPressInterAlert, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.radio:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataRadioAlert, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.others:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataTvAlert, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.outdoor:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataOutDoorAlert, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.indoor:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataInDoorAlert, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.instore:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataInStoreAlert, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.adnettrack:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataAdNetTrackAlert, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.czinternet:
                case DBClassificationConstantes.Vehicles.names.internet:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataInternetAlert, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataMarketingDirectAlert, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.mailValo:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataMailAlert, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.cinema:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataCinemaAlert, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.evaliantMobile:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataEvaliantMobileAlert, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.mms:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataMmsAlert, isRetailerSelected).Label);
                case DBClassificationConstantes.Vehicles.names.search:
                    return (WebApplicationParameters.GetDataTable(TableIds.dataSearchAlert, isRetailerSelected).Label);
                default:
                    throw (new SQLGeneratorException("Impossible de d�terminer la table � traiter"));
            }
        }

        /// <summary>
        /// Get data table to use in queries
        /// </summary>
        /// <param name="period">Type of period</param>
        /// <param name="vehicleId">Vehicle Id</param>
        /// <returns>Table matching the vehicle and the type of period</returns>
        public static string GetDataTableName(CstPeriod.PeriodBreakdownType period, Int64 vehicleId, bool isRetailerSelected) {
            switch (period) {
                case CstPeriod.PeriodBreakdownType.month:
                    return WebApplicationParameters.GetDataTable(TableIds.monthData, isRetailerSelected).SqlWithPrefix;
                case CstPeriod.PeriodBreakdownType.week:
                    return WebApplicationParameters.GetDataTable(TableIds.weekData, isRetailerSelected).SqlWithPrefix;
                case CstPeriod.PeriodBreakdownType.data_4m:
                    switch (VehiclesInformation.DatabaseIdToEnum(vehicleId)) {
                        case DBClassificationConstantes.Vehicles.names.newspaper:
                            return WebApplicationParameters.GetDataTable(TableIds.dataNewspaperAlert, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.magazine:
                            return WebApplicationParameters.GetDataTable(TableIds.dataMagazineAlert, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.press:
                            return WebApplicationParameters.GetDataTable(TableIds.dataPressAlert, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.internationalPress:
                            return WebApplicationParameters.GetDataTable(TableIds.dataPressInterAlert, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.radio:
                            return WebApplicationParameters.GetDataTable(TableIds.dataRadioAlert, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.tv:
                        case DBClassificationConstantes.Vehicles.names.tvGeneral:
                        case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                        case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                        case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                        case DBClassificationConstantes.Vehicles.names.others:
                            return WebApplicationParameters.GetDataTable(TableIds.dataTvAlert, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.outdoor:
                            return WebApplicationParameters.GetDataTable(TableIds.dataOutDoorAlert, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.indoor:
                            return WebApplicationParameters.GetDataTable(TableIds.dataInDoorAlert, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.instore:
                            return WebApplicationParameters.GetDataTable(TableIds.dataInStoreAlert, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.adnettrack:
                            return WebApplicationParameters.GetDataTable(TableIds.dataAdNetTrackAlert, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.czinternet:
                        case DBClassificationConstantes.Vehicles.names.internet:
                            return WebApplicationParameters.GetDataTable(TableIds.dataInternetAlert, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.directMarketing:
                            return WebApplicationParameters.GetDataTable(TableIds.dataMarketingDirectAlert, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.mailValo:
                            return WebApplicationParameters.GetDataTable(TableIds.dataMailAlert, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.cinema:
                            return WebApplicationParameters.GetDataTable(TableIds.dataCinemaAlert, isRetailerSelected).SqlWithPrefix;
						case DBClassificationConstantes.Vehicles.names.evaliantMobile:
                            return WebApplicationParameters.GetDataTable(TableIds.dataEvaliantMobileAlert, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.mms:
                            return WebApplicationParameters.GetDataTable(TableIds.dataMmsAlert, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.search:
                            return WebApplicationParameters.GetDataTable(TableIds.dataSearchAlert, isRetailerSelected).SqlWithPrefix;
                        default:
                            throw (new SQLGeneratorException("Unable to determine table to use."));
                    }
                case CstPeriod.PeriodBreakdownType.data:
                    switch(VehiclesInformation.DatabaseIdToEnum(vehicleId)) {
                        case DBClassificationConstantes.Vehicles.names.newspaper:
                            return WebApplicationParameters.GetDataTable(TableIds.dataNewspaper, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.magazine:
                            return WebApplicationParameters.GetDataTable(TableIds.dataMagazine, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.press:
                            return WebApplicationParameters.GetDataTable(TableIds.dataPress, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.internationalPress:
                            return WebApplicationParameters.GetDataTable(TableIds.dataPressInter, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.radio:
                            return WebApplicationParameters.GetDataTable(TableIds.dataRadio, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.tv:
                        case DBClassificationConstantes.Vehicles.names.tvGeneral:
                        case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                        case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                        case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                        case DBClassificationConstantes.Vehicles.names.others:
                            return WebApplicationParameters.GetDataTable(TableIds.dataTv, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.outdoor:
                            return WebApplicationParameters.GetDataTable(TableIds.dataOutDoor, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.indoor:
                            return WebApplicationParameters.GetDataTable(TableIds.dataInDoor, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.instore:
                            return WebApplicationParameters.GetDataTable(TableIds.dataInStore, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.adnettrack:
                            return WebApplicationParameters.GetDataTable(TableIds.dataAdNetTrack, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.czinternet:
                        case DBClassificationConstantes.Vehicles.names.internet:
                            return WebApplicationParameters.GetDataTable(TableIds.dataInternet, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.directMarketing:
                            return WebApplicationParameters.GetDataTable(TableIds.dataMarketingDirect, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.mailValo:
                            return WebApplicationParameters.GetDataTable(TableIds.dataMail, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.cinema:
                            return WebApplicationParameters.GetDataTable(TableIds.dataCinema, isRetailerSelected).SqlWithPrefix;
						case DBClassificationConstantes.Vehicles.names.evaliantMobile:
                            return WebApplicationParameters.GetDataTable(TableIds.dataEvaliantMobile, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.mms:
                            return WebApplicationParameters.GetDataTable(TableIds.dataMms, isRetailerSelected).SqlWithPrefix;
                        case DBClassificationConstantes.Vehicles.names.search:
                            return WebApplicationParameters.GetDataTable(TableIds.dataSearch, isRetailerSelected).SqlWithPrefix;
                        default:
                            throw (new SQLGeneratorException("Unable to determine the table to use"));
                    }
                default:
                    throw (new SQLGeneratorException("The detail selected is not a correct one to to choos of the tablme"));
            }
        }
        #endregion

        #region D�tail Produit

        #region Champs pour le d�tail produit
        /// <summary>
        /// Donne les champs � utilis� pour la colonne produit du tableau
        /// </summary>
        /// <param name="preformatedProductDetail">Niveau de d�tail produit</param>
        /// <param name="dataTablePrefixe">Pr�fixe de la table des donn�es</param>
        /// <returns>Champs</returns>
        internal static string GetIdFieldsForProductDetail(WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails preformatedProductDetail, string dataTablePrefixe)
        {
            switch (preformatedProductDetail)
            {
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
                    return (dataTablePrefixe + ".id_sector," + dataTablePrefixe + ".id_subsector," + dataTablePrefixe + ".id_group_");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
                    return (dataTablePrefixe + ".id_advertiser," + dataTablePrefixe + ".id_brand," + dataTablePrefixe + ".id_product");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
                    return (DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ID_GROUP_ADVERTISING_AGENCY," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ID_ADVERTISING_AGENCY");
                default:
                    throw (new SQLGeneratorException("Impossible d'initialiser les champs produits de la requ�tes"));
            }
        }

        /// <summary>
        /// Donne les champs � utilis� pour la colonne produit du tableau
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="dataTablePrefixe">Pr�fixe de la table des donn�es</param>
        /// <returns>Champs</returns>

        internal static string getFieldsForProductDetail(WebSession webSession, string dataTablePrefixe)
        {
            switch (webSession.PreformatedProductDetail)
            {
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sector:
                    return (dataTablePrefixe + ".id_sector," + DBConstantes.Tables.SECTOR_PREFIXE + ".sector");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
                    return (dataTablePrefixe + ".id_sector," + DBConstantes.Tables.SECTOR_PREFIXE + ".sector," + dataTablePrefixe + ".id_subsector," + DBConstantes.Tables.SUBSECTOR_PREFIXE + ".subsector, " + dataTablePrefixe + ".id_group_," + DBConstantes.Tables.GROUP_PREFIXE + ".group_");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsector:
                    return (dataTablePrefixe + ".id_sector," + DBConstantes.Tables.SECTOR_PREFIXE + ".sector," + dataTablePrefixe + ".id_subsector," + DBConstantes.Tables.SUBSECTOR_PREFIXE + ".subsector");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
                    return (dataTablePrefixe + ".id_sector," + DBConstantes.Tables.SECTOR_PREFIXE + ".sector," + dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser, " + dataTablePrefixe + ".id_product," + DBConstantes.Tables.PRODUCT_PREFIXE + ".product");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser:
                    return (dataTablePrefixe + ".id_sector," + DBConstantes.Tables.SECTOR_PREFIXE + ".sector," + dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorProduct:
                    return (dataTablePrefixe + ".id_sector," + DBConstantes.Tables.SECTOR_PREFIXE + ".sector," + dataTablePrefixe + ".id_product," + DBConstantes.Tables.PRODUCT_PREFIXE + ".product");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product:
                    return (dataTablePrefixe + ".id_product," + DBConstantes.Tables.PRODUCT_PREFIXE + ".product");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser:
                    return (dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand:
                    return (dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser," + dataTablePrefixe + ".id_brand," + DBConstantes.Tables.BRAND_PREFIXE + ".brand");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct:
                    return (dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser," + dataTablePrefixe + ".id_product," + DBConstantes.Tables.PRODUCT_PREFIXE + ".product");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
                    return (dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser," + dataTablePrefixe + ".id_brand," + DBConstantes.Tables.BRAND_PREFIXE + ".brand," + dataTablePrefixe + ".id_product," + DBConstantes.Tables.PRODUCT_PREFIXE + ".product");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group:
                    return (dataTablePrefixe + ".id_group_," + DBConstantes.Tables.GROUP_PREFIXE + ".group_");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand:
                    return (dataTablePrefixe + ".id_group_," + DBConstantes.Tables.GROUP_PREFIXE + ".group_," + dataTablePrefixe + ".id_brand," + DBConstantes.Tables.BRAND_PREFIXE + ".brand");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupProduct:
                    return (dataTablePrefixe + ".id_group_," + DBConstantes.Tables.GROUP_PREFIXE + ".group_," + dataTablePrefixe + ".id_product," + DBConstantes.Tables.PRODUCT_PREFIXE + ".product");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiser:
                    return (dataTablePrefixe + ".id_group_," + DBConstantes.Tables.GROUP_PREFIXE + ".group_," + dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser");

                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
                    return (dataTablePrefixe + ".id_group_," + DBConstantes.Tables.GROUP_PREFIXE + ".group_," + dataTablePrefixe + ".id_brand," + DBConstantes.Tables.BRAND_PREFIXE + ".brand," + dataTablePrefixe + ".id_product," + DBConstantes.Tables.PRODUCT_PREFIXE + ".product");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
                    return (DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ID_GROUP_ADVERTISING_AGENCY," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".GROUP_ADVERTISING_AGENCY, " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ID_ADVERTISING_AGENCY," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ADVERTISING_AGENCY");

                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser:
                    return (DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ID_GROUP_ADVERTISING_AGENCY," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".GROUP_ADVERTISING_AGENCY, " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ID_ADVERTISING_AGENCY," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ADVERTISING_AGENCY," + dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct:
                    return (DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ID_GROUP_ADVERTISING_AGENCY," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".GROUP_ADVERTISING_AGENCY, " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ID_ADVERTISING_AGENCY," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ADVERTISING_AGENCY," + dataTablePrefixe + ".id_product," + DBConstantes.Tables.PRODUCT_PREFIXE + ".product");

                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
                    return (DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ID_ADVERTISING_AGENCY," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".advertising_agency, " + dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
                    return (DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ID_ADVERTISING_AGENCY," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ADVERTISING_AGENCY, " + dataTablePrefixe + ".id_product," + DBConstantes.Tables.PRODUCT_PREFIXE + ".product");
                //additions
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompany:
                    return (dataTablePrefixe + ".id_holding_company," + DBConstantes.Tables.HOLDING_PREFIXE + ".holding_company");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiser:
                    return (dataTablePrefixe + ".id_holding_company," + DBConstantes.Tables.HOLDING_PREFIXE + ".holding_company," + dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserBrand:
                    return (dataTablePrefixe + ".id_holding_company," + DBConstantes.Tables.HOLDING_PREFIXE + ".holding_company," + dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser," + dataTablePrefixe + ".id_brand," + DBConstantes.Tables.BRAND_PREFIXE + ".brand");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserProduct:
                    return (dataTablePrefixe + ".id_holding_company," + DBConstantes.Tables.HOLDING_PREFIXE + ".holding_company," + dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser," + dataTablePrefixe + ".id_product," + DBConstantes.Tables.PRODUCT_PREFIXE + ".product");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser:
                    return (dataTablePrefixe + ".id_segment," + DBConstantes.Tables.SEGMENT_PREFIXE + ".segment," + dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentProduct:
                    return (dataTablePrefixe + ".id_segment," + DBConstantes.Tables.SEGMENT_PREFIXE + ".segment," + dataTablePrefixe + ".id_product," + DBConstantes.Tables.PRODUCT_PREFIXE + ".product");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentBrand:
                    return (dataTablePrefixe + ".id_segment," + DBConstantes.Tables.SEGMENT_PREFIXE + ".segment," + dataTablePrefixe + ".id_brand," + DBConstantes.Tables.BRAND_PREFIXE + ".brand");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserProduct:
                    return (dataTablePrefixe + ".id_segment," + DBConstantes.Tables.SEGMENT_PREFIXE + ".segment," + dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser," + dataTablePrefixe + ".id_product," + DBConstantes.Tables.PRODUCT_PREFIXE + ".product");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserBrand:
                    return (dataTablePrefixe + ".id_segment," + DBConstantes.Tables.SEGMENT_PREFIXE + ".segment," + dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser," + dataTablePrefixe + ".id_brand," + DBConstantes.Tables.BRAND_PREFIXE + ".brand");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorHoldingCompanyAdvertiser:
                    return (dataTablePrefixe + ".id_sector," + DBConstantes.Tables.SECTOR_PREFIXE + ".sector," + dataTablePrefixe + ".id_holding_company," + DBConstantes.Tables.HOLDING_PREFIXE + ".holding_company," + dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser");
                default:
                    throw (new SQLGeneratorException("Impossible d'initialiser les champs produits de la requ�tes"));
            }
        }
        #endregion

        #region Tables pour le d�tail produits
        /// <summary>
        /// Donne les champs � utiliser pour la colonne produit du tableau
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <returns>Tables</returns>
        internal static string getTablesForProductDetail(WebSession webSession)
        {
            switch (webSession.PreformatedProductDetail)
            {
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sector:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".sector " + DBConstantes.Tables.SECTOR_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".sector " + DBConstantes.Tables.SECTOR_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".subsector " + DBConstantes.Tables.SUBSECTOR_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".group_ " + DBConstantes.Tables.GROUP_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsector:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".sector " + DBConstantes.Tables.SECTOR_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".subsector " + DBConstantes.Tables.SUBSECTOR_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".sector " + DBConstantes.Tables.SECTOR_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".advertiser " + DBConstantes.Tables.ADVERTISER_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".sector " + DBConstantes.Tables.SECTOR_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".advertiser " + DBConstantes.Tables.ADVERTISER_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".product " + DBConstantes.Tables.PRODUCT_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorProduct:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".sector " + DBConstantes.Tables.SECTOR_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".product " + DBConstantes.Tables.PRODUCT_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".product " + DBConstantes.Tables.PRODUCT_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".advertiser " + DBConstantes.Tables.ADVERTISER_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".advertiser " + DBConstantes.Tables.ADVERTISER_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".brand " + DBConstantes.Tables.BRAND_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".advertiser " + DBConstantes.Tables.ADVERTISER_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".product " + DBConstantes.Tables.PRODUCT_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".advertiser " + DBConstantes.Tables.ADVERTISER_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".brand " + DBConstantes.Tables.BRAND_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".product " + DBConstantes.Tables.PRODUCT_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".group_ " + DBConstantes.Tables.GROUP_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".group_ " + DBConstantes.Tables.GROUP_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".brand " + DBConstantes.Tables.BRAND_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupProduct:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".group_ " + DBConstantes.Tables.GROUP_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".product " + DBConstantes.Tables.PRODUCT_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".group_ " + DBConstantes.Tables.GROUP_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".brand " + DBConstantes.Tables.BRAND_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".product " + DBConstantes.Tables.PRODUCT_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + webSession.MediaAgencyFileYear + " " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + "");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".advertiser " + DBConstantes.Tables.ADVERTISER_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + webSession.MediaAgencyFileYear + " " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + "");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".product " + DBConstantes.Tables.PRODUCT_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + webSession.MediaAgencyFileYear + " " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + "");
                //additions
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + webSession.MediaAgencyFileYear + " " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".product " + DBConstantes.Tables.PRODUCT_PREFIXE + "");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + webSession.MediaAgencyFileYear + " " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".advertiser " + DBConstantes.Tables.ADVERTISER_PREFIXE + "");

                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompany:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".holding_company " + DBConstantes.Tables.HOLDING_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiser:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".holding_company " + DBConstantes.Tables.HOLDING_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".advertiser " + DBConstantes.Tables.ADVERTISER_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserBrand:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".holding_company " + DBConstantes.Tables.HOLDING_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".advertiser " + DBConstantes.Tables.ADVERTISER_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".brand " + DBConstantes.Tables.BRAND_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserProduct:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".holding_company " + DBConstantes.Tables.HOLDING_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".advertiser " + DBConstantes.Tables.ADVERTISER_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".product " + DBConstantes.Tables.PRODUCT_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".segment " + DBConstantes.Tables.SEGMENT_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".advertiser " + DBConstantes.Tables.ADVERTISER_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentBrand:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".segment " + DBConstantes.Tables.SEGMENT_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".brand " + DBConstantes.Tables.BRAND_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentProduct:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".segment " + DBConstantes.Tables.SEGMENT_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".product " + DBConstantes.Tables.PRODUCT_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserBrand:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".segment " + DBConstantes.Tables.SEGMENT_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".advertiser " + DBConstantes.Tables.ADVERTISER_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".brand " + DBConstantes.Tables.BRAND_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserProduct:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".segment " + DBConstantes.Tables.SEGMENT_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".advertiser " + DBConstantes.Tables.ADVERTISER_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".product " + DBConstantes.Tables.PRODUCT_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiser:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".group_ " + DBConstantes.Tables.GROUP_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".advertiser " + DBConstantes.Tables.ADVERTISER_PREFIXE);
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorHoldingCompanyAdvertiser:
                    return (DBConstantes.Schema.ADEXPRESS_SCHEMA + ".sector " + DBConstantes.Tables.SECTOR_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".holding_company " + DBConstantes.Tables.HOLDING_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".advertiser " + DBConstantes.Tables.ADVERTISER_PREFIXE);

                default:
                    throw (new SQLGeneratorException("Impossible d'initialiser les tables produits de la requ�tes"));
            }
        }
        #endregion

        #region Jointure pour les d�tails Produits
        /// <summary>
        /// Donne les champs � utilis� pour la colonne produit du tableau
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="dataTablePrefixe">Pr�fixe de la table des donn�es</param>
        /// <param name="vehicleName">Type de vehicle</param>
        /// <returns>Jointures</returns>
        internal static string getJointAndLanguageForProductDetail(WebSession webSession, string dataTablePrefixe, DBClassificationConstantes.Vehicles.names vehicleName)
        {
            string joint = "";
            switch (webSession.PreformatedProductDetail)
            {
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sector:
                    // Sector
                    joint += " and " + DBConstantes.Tables.SECTOR_PREFIXE + ".id_sector=" + dataTablePrefixe + ".id_sector";
                    joint += " and " + DBConstantes.Tables.SECTOR_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
                    // Sector
                    joint += " and " + DBConstantes.Tables.SECTOR_PREFIXE + ".id_sector=" + dataTablePrefixe + ".id_sector";
                    joint += " and " + DBConstantes.Tables.SECTOR_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    //SubSector
                    joint += " and " + DBConstantes.Tables.SUBSECTOR_PREFIXE + ".id_subsector=" + dataTablePrefixe + ".id_subsector";
                    joint += " and " + DBConstantes.Tables.SUBSECTOR_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    //Group_
                    joint += " and " + DBConstantes.Tables.GROUP_PREFIXE + ".id_group_=" + dataTablePrefixe + ".id_group_";
                    joint += " and " + DBConstantes.Tables.GROUP_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsector:
                    // Sector
                    joint += " and " + DBConstantes.Tables.SECTOR_PREFIXE + ".id_sector=" + dataTablePrefixe + ".id_sector";
                    joint += " and " + DBConstantes.Tables.SECTOR_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    //SubSector
                    joint += " and " + DBConstantes.Tables.SUBSECTOR_PREFIXE + ".id_subsector=" + dataTablePrefixe + ".id_subsector";
                    joint += " and " + DBConstantes.Tables.SUBSECTOR_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
                    // Sector
                    joint += " and " + DBConstantes.Tables.SECTOR_PREFIXE + ".id_sector=" + dataTablePrefixe + ".id_sector";
                    joint += " and " + DBConstantes.Tables.SECTOR_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    //Annonceur
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_advertiser=" + dataTablePrefixe + ".id_advertiser";
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    //Produit
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_product=" + dataTablePrefixe + ".id_product";
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser:
                    //Sector
                    joint += " and " + DBConstantes.Tables.SECTOR_PREFIXE + ".id_sector=" + dataTablePrefixe + ".id_sector";
                    joint += " and " + DBConstantes.Tables.SECTOR_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    //Annonceur
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_advertiser=" + dataTablePrefixe + ".id_advertiser";
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorProduct:
                    //Sector
                    joint += " and " + DBConstantes.Tables.SECTOR_PREFIXE + ".id_sector=" + dataTablePrefixe + ".id_sector";
                    joint += " and " + DBConstantes.Tables.SECTOR_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    //Product
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_product=" + dataTablePrefixe + ".id_product";
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product:
                    // Product
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_product=" + dataTablePrefixe + ".id_product";
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser:
                    // Advertiser
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_advertiser=" + dataTablePrefixe + ".id_advertiser";
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand:
                    // Advertiser
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_advertiser=" + dataTablePrefixe + ".id_advertiser";
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    // Brand
                    joint += " and " + DBConstantes.Tables.BRAND_PREFIXE + ".id_brand=" + dataTablePrefixe + ".id_brand";
                    joint += " and " + DBConstantes.Tables.BRAND_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct:
                    // Advertiser
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_advertiser=" + dataTablePrefixe + ".id_advertiser";
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    //Product
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_product=" + dataTablePrefixe + ".id_product";
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
                    // Advertiser
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_advertiser=" + dataTablePrefixe + ".id_advertiser";
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    // Brand
                    joint += " and " + DBConstantes.Tables.BRAND_PREFIXE + ".id_brand=" + dataTablePrefixe + ".id_brand";
                    joint += " and " + DBConstantes.Tables.BRAND_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    //Product
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_product=" + dataTablePrefixe + ".id_product";
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group:
                    //Group_
                    joint += " and " + DBConstantes.Tables.GROUP_PREFIXE + ".id_group_=" + dataTablePrefixe + ".id_group_";
                    joint += " and " + DBConstantes.Tables.GROUP_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand:
                    //Group_
                    joint += " and " + DBConstantes.Tables.GROUP_PREFIXE + ".id_group_=" + dataTablePrefixe + ".id_group_";
                    joint += " and " + DBConstantes.Tables.GROUP_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    // Brand
                    joint += " and " + DBConstantes.Tables.BRAND_PREFIXE + ".id_brand=" + dataTablePrefixe + ".id_brand";
                    joint += " and " + DBConstantes.Tables.BRAND_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupProduct:
                    //Group_
                    joint += " and " + DBConstantes.Tables.GROUP_PREFIXE + ".id_group_=" + dataTablePrefixe + ".id_group_";
                    joint += " and " + DBConstantes.Tables.GROUP_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    //Product
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_product=" + dataTablePrefixe + ".id_product";
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiser:
                    //Group_
                    joint += " and " + DBConstantes.Tables.GROUP_PREFIXE + ".id_group_=" + dataTablePrefixe + ".id_group_";
                    joint += " and " + DBConstantes.Tables.GROUP_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    // Advertiser
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_advertiser=" + dataTablePrefixe + ".id_advertiser";
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
                    //Group_
                    joint += " and " + DBConstantes.Tables.GROUP_PREFIXE + ".id_group_=" + dataTablePrefixe + ".id_group_";
                    joint += " and " + DBConstantes.Tables.GROUP_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    // Brand
                    joint += " and " + DBConstantes.Tables.BRAND_PREFIXE + ".id_brand=" + dataTablePrefixe + ".id_brand";
                    joint += " and " + DBConstantes.Tables.BRAND_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    //Product
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_product=" + dataTablePrefixe + ".id_product";
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
                    // Product
                    joint += " and " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_product=" + dataTablePrefixe + ".id_product";
                    joint += " and " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    // Vehicle
                    joint += " and " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_vehicle=" + vehicleName.GetHashCode();
                    break;

                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser:
                    // Product
                    joint += " and " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_product=" + dataTablePrefixe + ".id_product";
                    joint += " and " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString(); 
                    // Vechicle
                    joint += " and " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_vehicle=" + vehicleName.GetHashCode();
                    // Advertiser
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_advertiser=" + dataTablePrefixe + ".id_advertiser";
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct:
                    // Product
                    joint += " and " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_product=" + dataTablePrefixe + ".id_product";
                    joint += " and " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    // Vechicle
                    joint += " and " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_vehicle=" + vehicleName.GetHashCode();
                    //Product
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_product=" + dataTablePrefixe + ".id_product";
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;

                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
                    // Product
                    joint += " and " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_product=" + dataTablePrefixe + ".id_product";
                    joint += " and " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    // Advertiser
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_advertiser=" + dataTablePrefixe + ".id_advertiser";
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    // Vehicle
                    joint += " and " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_vehicle=" + vehicleName.GetHashCode();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
                    //Product
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_product=" + dataTablePrefixe + ".id_product";
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    // Product
                    joint += " and " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_product=" + dataTablePrefixe + ".id_product";
                    joint += " and " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    // Vehicle
                    joint += " and " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_vehicle=" + vehicleName.GetHashCode();
                    break;

                //additions
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompany:
                    // holdingCompany
                    joint += " and " + DBConstantes.Tables.HOLDING_PREFIXE + ".id_holding_company=" + dataTablePrefixe + ".id_holding_company";
                    joint += " and " + DBConstantes.Tables.HOLDING_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiser:
                    //holdingCompany
                    joint += " and " + DBConstantes.Tables.HOLDING_PREFIXE + ".id_holding_company=" + dataTablePrefixe + ".id_holding_company";
                    joint += " and " + DBConstantes.Tables.HOLDING_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    // Advertiser
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_advertiser=" + dataTablePrefixe + ".id_advertiser";
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserBrand:
                    //holdingCompany
                    joint += " and " + DBConstantes.Tables.HOLDING_PREFIXE + ".id_holding_company=" + dataTablePrefixe + ".id_holding_company";
                    joint += " and " + DBConstantes.Tables.HOLDING_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    // Advertiser
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_advertiser=" + dataTablePrefixe + ".id_advertiser";
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    // Brand
                    joint += " and " + DBConstantes.Tables.BRAND_PREFIXE + ".id_brand=" + dataTablePrefixe + ".id_brand";
                    joint += " and " + DBConstantes.Tables.BRAND_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserProduct:
                    //holdingCompany
                    joint += " and " + DBConstantes.Tables.HOLDING_PREFIXE + ".id_holding_company=" + dataTablePrefixe + ".id_holding_company";
                    joint += " and " + DBConstantes.Tables.HOLDING_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    // Advertiser
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_advertiser=" + dataTablePrefixe + ".id_advertiser";
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    //Product
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_product=" + dataTablePrefixe + ".id_product";
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser:
                    //segment
                    joint += " and " + DBConstantes.Tables.SEGMENT_PREFIXE + ".id_segment=" + dataTablePrefixe + ".id_segment";
                    joint += " and " + DBConstantes.Tables.SEGMENT_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    // Advertiser
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_advertiser=" + dataTablePrefixe + ".id_advertiser";
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentBrand:
                    //segment
                    joint += " and " + DBConstantes.Tables.SEGMENT_PREFIXE + ".id_segment=" + dataTablePrefixe + ".id_segment";
                    joint += " and " + DBConstantes.Tables.SEGMENT_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    // Brand
                    joint += " and " + DBConstantes.Tables.BRAND_PREFIXE + ".id_brand=" + dataTablePrefixe + ".id_brand";
                    joint += " and " + DBConstantes.Tables.BRAND_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentProduct:
                    //segment
                    joint += " and " + DBConstantes.Tables.SEGMENT_PREFIXE + ".id_segment=" + dataTablePrefixe + ".id_segment";
                    joint += " and " + DBConstantes.Tables.SEGMENT_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    //Product
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_product=" + dataTablePrefixe + ".id_product";
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserBrand:
                    //segment
                    joint += " and " + DBConstantes.Tables.SEGMENT_PREFIXE + ".id_segment=" + dataTablePrefixe + ".id_segment";
                    joint += " and " + DBConstantes.Tables.SEGMENT_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    // Advertiser
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_advertiser=" + dataTablePrefixe + ".id_advertiser";
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    // Brand
                    joint += " and " + DBConstantes.Tables.BRAND_PREFIXE + ".id_brand=" + dataTablePrefixe + ".id_brand";
                    joint += " and " + DBConstantes.Tables.BRAND_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserProduct:
                    //segment
                    joint += " and " + DBConstantes.Tables.SEGMENT_PREFIXE + ".id_segment=" + dataTablePrefixe + ".id_segment";
                    joint += " and " + DBConstantes.Tables.SEGMENT_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    // Advertiser
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_advertiser=" + dataTablePrefixe + ".id_advertiser";
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    //Product
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_product=" + dataTablePrefixe + ".id_product";
                    joint += " and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorHoldingCompanyAdvertiser:
                    // Sector
                    joint += " and " + DBConstantes.Tables.SECTOR_PREFIXE + ".id_sector=" + dataTablePrefixe + ".id_sector";
                    joint += " and " + DBConstantes.Tables.SECTOR_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    //holdingCompany
                    joint += " and " + DBConstantes.Tables.HOLDING_PREFIXE + ".id_holding_company=" + dataTablePrefixe + ".id_holding_company";
                    joint += " and " + DBConstantes.Tables.HOLDING_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    // Advertiser
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_advertiser=" + dataTablePrefixe + ".id_advertiser";
                    joint += " and " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
                    break;
                default:
                    throw (new SQLGeneratorException("Impossible d'initialiser les champs produits de la requ�tes"));
            }
            return (joint);
        }
        #endregion

        #region order by pour les d�tails Produits
        /// <summary>
        /// Retourne la liste des champs pour le order by
        /// </summary>
        /// <param name="webSession">session client</param>
        /// <param name="dataTablePrefixe">pr�fixe de la table des donn�es</param>
        /// <returns></returns>
        internal static string getOrderByForProductDetail(WebSession webSession, string dataTablePrefixe)
        {
            switch (webSession.PreformatedProductDetail)
            {
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sector:
                    return (DBConstantes.Tables.SECTOR_PREFIXE + ".sector, " + dataTablePrefixe + ".id_sector");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
                    return (DBConstantes.Tables.SECTOR_PREFIXE + ".sector, " + dataTablePrefixe + ".id_sector," + DBConstantes.Tables.SUBSECTOR_PREFIXE + ".subsector," + dataTablePrefixe + ".id_subsector," + DBConstantes.Tables.GROUP_PREFIXE + ".group_," + dataTablePrefixe + ".id_group_");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsector:
                    return (DBConstantes.Tables.SECTOR_PREFIXE + ".sector," + dataTablePrefixe + ".id_sector," + DBConstantes.Tables.SUBSECTOR_PREFIXE + ".subsector");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
                    return (DBConstantes.Tables.SECTOR_PREFIXE + ".sector," + dataTablePrefixe + ".id_sector," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser," + dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.PRODUCT_PREFIXE + ".product," + dataTablePrefixe + ".id_product");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser:
                    return (DBConstantes.Tables.SECTOR_PREFIXE + ".sector," + dataTablePrefixe + ".id_sector," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser," + dataTablePrefixe + ".id_advertiser");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorProduct:
                    return (DBConstantes.Tables.SECTOR_PREFIXE + ".sector," + dataTablePrefixe + ".id_sector," + DBConstantes.Tables.PRODUCT_PREFIXE + ".product," + dataTablePrefixe + ".id_product");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product:
                    return (DBConstantes.Tables.PRODUCT_PREFIXE + ".product," + dataTablePrefixe + ".id_product");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser:
                    return (DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser," + dataTablePrefixe + ".id_advertiser");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand:
                    return (DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser," + dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.BRAND_PREFIXE + ".brand," + dataTablePrefixe + ".id_brand");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct:
                    return (DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser," + dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.PRODUCT_PREFIXE + ".product," + dataTablePrefixe + ".id_product");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
                    return (DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser," + dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.BRAND_PREFIXE + ".brand," + dataTablePrefixe + ".id_brand," + DBConstantes.Tables.PRODUCT_PREFIXE + ".product," + dataTablePrefixe + ".id_product");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group:
                    return (DBConstantes.Tables.GROUP_PREFIXE + ".group_," + dataTablePrefixe + ".id_group_");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand:
                    return (DBConstantes.Tables.GROUP_PREFIXE + ".group_," + dataTablePrefixe + ".id_group_," + DBConstantes.Tables.BRAND_PREFIXE + ".brand," + dataTablePrefixe + ".id_brand");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupProduct:
                    return (DBConstantes.Tables.GROUP_PREFIXE + ".group_," + dataTablePrefixe + ".id_group_," + DBConstantes.Tables.PRODUCT_PREFIXE + ".product," + dataTablePrefixe + ".id_product");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiser:
                    return (DBConstantes.Tables.GROUP_PREFIXE + ".group_," + dataTablePrefixe + ".id_group_," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser," + dataTablePrefixe + ".id_advertiser");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
                    return (DBConstantes.Tables.GROUP_PREFIXE + ".group_," + dataTablePrefixe + ".id_group_," + DBConstantes.Tables.BRAND_PREFIXE + ".brand," + dataTablePrefixe + ".id_brand," + DBConstantes.Tables.PRODUCT_PREFIXE + ".product," + dataTablePrefixe + ".id_product");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
                    return (DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".GROUP_ADVERTISING_AGENCY," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_group_advertising_agency," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ADVERTISING_AGENCY," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_advertising_agency");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser:
                    return (DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".GROUP_ADVERTISING_AGENCY," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_group_advertising_agency," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ADVERTISING_AGENCY," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_advertising_agency, " + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser," + dataTablePrefixe + ".id_advertiser");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct:
                    return (DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".GROUP_ADVERTISING_AGENCY," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_group_advertising_agency," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ADVERTISING_AGENCY," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_advertising_agency, " + DBConstantes.Tables.PRODUCT_PREFIXE + ".product," + dataTablePrefixe + ".id_product");

                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
                    return (DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".advertising_agency," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_advertising_agency," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser," + dataTablePrefixe + ".id_advertiser");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
                    return (DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ADVERTISING_AGENCY," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_advertising_agency," + DBConstantes.Tables.PRODUCT_PREFIXE + ".product," + dataTablePrefixe + ".id_product");

                //additions
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompany:
                    return (DBConstantes.Tables.HOLDING_PREFIXE + ".holding_company, " + dataTablePrefixe + ".id_holding_company");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiser:
                    return (DBConstantes.Tables.HOLDING_PREFIXE + ".holding_company, " + dataTablePrefixe + ".id_holding_company," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser," + dataTablePrefixe + ".id_advertiser");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserBrand:
                    return (DBConstantes.Tables.HOLDING_PREFIXE + ".holding_company, " + dataTablePrefixe + ".id_holding_company," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser," + dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.BRAND_PREFIXE + ".brand," + dataTablePrefixe + ".id_brand");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserProduct:
                    return (DBConstantes.Tables.HOLDING_PREFIXE + ".holding_company, " + dataTablePrefixe + ".id_holding_company," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser," + dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.PRODUCT_PREFIXE + ".product," + dataTablePrefixe + ".id_product");

                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser:
                    return (DBConstantes.Tables.SEGMENT_PREFIXE + ".segment," + dataTablePrefixe + ".id_segment," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser," + dataTablePrefixe + ".id_advertiser");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentBrand:
                    return (DBConstantes.Tables.SEGMENT_PREFIXE + ".segment," + dataTablePrefixe + ".id_segment," + DBConstantes.Tables.BRAND_PREFIXE + ".brand," + dataTablePrefixe + ".id_brand");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentProduct:
                    return (DBConstantes.Tables.SEGMENT_PREFIXE + ".segment," + dataTablePrefixe + ".id_segment," + DBConstantes.Tables.PRODUCT_PREFIXE + ".product," + dataTablePrefixe + ".id_product");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserBrand:
                    return (DBConstantes.Tables.SEGMENT_PREFIXE + ".segment," + dataTablePrefixe + ".id_segment," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser," + dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.BRAND_PREFIXE + ".brand," + dataTablePrefixe + ".id_brand");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserProduct:
                    return (DBConstantes.Tables.SEGMENT_PREFIXE + ".segment," + dataTablePrefixe + ".id_segment," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser," + dataTablePrefixe + ".id_advertiser," + DBConstantes.Tables.PRODUCT_PREFIXE + ".product," + dataTablePrefixe + ".id_product");
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorHoldingCompanyAdvertiser:
                    return (DBConstantes.Tables.SECTOR_PREFIXE + ".sector," + dataTablePrefixe + ".id_sector," + DBConstantes.Tables.HOLDING_PREFIXE + ".holding_company," + dataTablePrefixe + ".id_holding_company," + DBConstantes.Tables.ADVERTISER_PREFIXE + ".advertiser," + dataTablePrefixe + ".id_advertiser");

                default:
                    throw (new SQLGeneratorException("Impossible d'initialiser les champs produits de la requ�tes"));
            }
        }
        #endregion

        #endregion

        #region Get DataBase Table
        /// <summary>
        /// Get Table to treat detailed data
        /// </summary>
        /// <param name="vehicleName">Vehicle</param>
        /// <param name="moduleType">Type de module</param>
        /// <returns>Nom de la table</returns>
        public static Table GetDataTable(VehicleInformation v, WebConstantes.Module.Type moduleType, bool isRetailerSelected)
        {
            try
            {
                switch (moduleType)
                {
                    case WebConstantes.Module.Type.alert:
                        return GetAlertTable(v, isRetailerSelected);
                    case WebConstantes.Module.Type.analysis:
                        return GetAnalysisTable(v, isRetailerSelected);
                    case WebConstantes.Module.Type.tvSponsorship:
                        return WebApplicationParameters.GetDataTable(TableIds.dataSponsorship, isRetailerSelected);

                    default:
                        throw (new SQLGeneratorException("Unvalid module type."));
                }
            }
            catch (SQLGeneratorException fe)
            {
                throw (fe);
            }
        }
        /// <summary>
        /// Get Alert Table
        /// </summary>
        /// <param name="v">Vehicle</param>
        /// <returns>Object Table</returns>
        internal static Table GetAlertTable(VehicleInformation v, bool isRetailerSelected) {
            switch (v.Id) {
                case DBClassificationConstantes.Vehicles.names.press:
                    return WebApplicationParameters.GetDataTable(TableIds.dataPressAlert, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.newspaper:
                    return WebApplicationParameters.GetDataTable(TableIds.dataNewspaperAlert, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.magazine:
                    return WebApplicationParameters.GetDataTable(TableIds.dataMagazineAlert, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    return WebApplicationParameters.GetDataTable(TableIds.dataPressInterAlert, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.radio:
                    return WebApplicationParameters.GetDataTable(TableIds.dataRadioAlert, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    return WebApplicationParameters.GetDataTable(TableIds.dataTvAlert, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.outdoor:
                    return WebApplicationParameters.GetDataTable(TableIds.dataOutDoorAlert, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.indoor:
                    return WebApplicationParameters.GetDataTable(TableIds.dataInDoorAlert, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.instore:
                    return WebApplicationParameters.GetDataTable(TableIds.dataInStoreAlert, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.adnettrack:
                    return WebApplicationParameters.GetDataTable(TableIds.dataAdNetTrackAlert, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.czinternet:
                case DBClassificationConstantes.Vehicles.names.internet:
                    return WebApplicationParameters.GetDataTable(TableIds.dataInternetAlert, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    return WebApplicationParameters.GetDataTable(TableIds.dataMarketingDirectAlert, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.mailValo:
                    return WebApplicationParameters.GetDataTable(TableIds.dataMailAlert, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.cinema:
                    return WebApplicationParameters.GetDataTable(TableIds.dataCinemaAlert, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.evaliantMobile:
                    return WebApplicationParameters.GetDataTable(TableIds.dataEvaliantMobileAlert, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.mms:
                    return WebApplicationParameters.GetDataTable(TableIds.dataMmsAlert, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.search:
                    return WebApplicationParameters.GetDataTable(TableIds.dataSearchAlert, isRetailerSelected);
                default:
                    throw new SQLGeneratorException("Unknown vehicle.");
            }
        }
        /// <summary>
        /// Get Analysis Table
        /// </summary>
        /// <param name="v">Vehicle</param>
        /// <returns>Object Table</returns>
        internal static Table GetAnalysisTable(VehicleInformation v, bool isRetailerSelected)
        {
            switch (v.Id)
            {
                case DBClassificationConstantes.Vehicles.names.press:
                    return WebApplicationParameters.GetDataTable(TableIds.dataPress, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.newspaper:
                    return WebApplicationParameters.GetDataTable(TableIds.dataNewspaper, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.magazine:
                    return WebApplicationParameters.GetDataTable(TableIds.dataMagazine, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    return WebApplicationParameters.GetDataTable(TableIds.dataPressInter, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.radio:
                    return WebApplicationParameters.GetDataTable(TableIds.dataRadio, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    return WebApplicationParameters.GetDataTable(TableIds.dataTv, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.outdoor:
                    return WebApplicationParameters.GetDataTable(TableIds.dataOutDoor, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.indoor:
                    return WebApplicationParameters.GetDataTable(TableIds.dataInDoor, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.instore:
                    return WebApplicationParameters.GetDataTable(TableIds.dataInStore, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.adnettrack:
                    return WebApplicationParameters.GetDataTable(TableIds.dataAdNetTrack, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.czinternet:
                case DBClassificationConstantes.Vehicles.names.internet:
                    return WebApplicationParameters.GetDataTable(TableIds.dataInternet, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    return WebApplicationParameters.GetDataTable(TableIds.dataMarketingDirect, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.mailValo:
                    return WebApplicationParameters.GetDataTable(TableIds.dataMail, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.cinema:
                    return WebApplicationParameters.GetDataTable(TableIds.dataCinema, isRetailerSelected);
				case DBClassificationConstantes.Vehicles.names.evaliantMobile:
                    return WebApplicationParameters.GetDataTable(TableIds.dataEvaliantMobile, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.mms:
                    return WebApplicationParameters.GetDataTable(TableIds.dataMms, isRetailerSelected);
                case DBClassificationConstantes.Vehicles.names.search:
                    return WebApplicationParameters.GetDataTable(TableIds.dataSearch, isRetailerSelected);
                default:
                    throw new SQLGeneratorException("Unknown vehicle.");
            }
        }
        #endregion

        #endregion

        #region Unit�
        /// <summary>
        /// Obtient le champ � utiliser en fonction de l'unit� des tables d�sagr�g�es
        /// </summary>
        /// <param name="unit">Unit�</param>
        /// <returns>Nom du champ</returns>
        /// <exception cref="SQLGeneratorException">
        /// Lanc�e au cas ou l'unit� consid�r�e n'est pas un cas trait�
        /// </exception>
        [Obsolete("La m�thode est obsolete; Utilisez GetUnitFieldsName qui peut g�rer les modules d'alerte et d'analyse")]
        public static string getUnitField(WebConstantes.CustomerSessions.Unit unit) {

            try {
                return UnitsInformation.List[unit].DatabaseField;
            }
            catch{
                throw new SQLGeneratorException("getField(TNS.AdExpress.Constantes.Web.CustomerSessions.Unit unit)-->Le cas de cette unit� n'est pas g�rer. Pas de champ correspondante.");
            }

        }

        /// <summary>
        /// Obtient le champ � utiliser en fonction de l'unit� des tables agr�g�es
        /// </summary>
        /// <param name="unit">Unit�</param>
        /// <returns>Nom du champ</returns>
        /// <exception cref="SQLGeneratorException">
        /// Lanc�e au cas ou l'unit� consid�r�e n'est pas un cas trait�
        /// </exception>
        [Obsolete("La m�thode est obsolete; Utilisez GetUnitFieldsName qui peut g�rer les modules d'alerte et d'analyse")]
        public static string getTotalUnitField(WebConstantes.CustomerSessions.Unit unit) {

            try {
                return UnitsInformation.List[unit].DatabaseMultimediaField;
            }
            catch{
                throw new SQLGeneratorException("getTotalUnitField(WebConstantes.CustomerSessions.Unit unit)-->Le cas de cette unit� n'est pas g�rer. Pas de champ correspondante.");
            }

        }


        /// <summary>
        /// Obtient les champs unit�s � utiliser en fonction du media
        /// </summary>
        ///<param name="webSession">Web session</param>
        /// <param name="type">Type de la table</param>
        ///<param name="dataTablePrefixe">pr�fixe de la table des donn�es</param>
        /// <returns>NOms des champs coorespondant aux unit�s s�lectionn�es</returns>
        public static string GetUnitFieldsName(WebSession webSession, DBConstantes.TableType.Type type, string dataTablePrefixe) {
            List<UnitInformation> unitsList = webSession.GetValidUnitForResult();
            StringBuilder sqlUnit = new StringBuilder();
            if (dataTablePrefixe != null && dataTablePrefixe.Length > 0)
                dataTablePrefixe += ".";
            else
                dataTablePrefixe = "";
            for (int i = 0; i < unitsList.Count; i++) {
                if (i > 0) sqlUnit.Append(",");
                switch (type) {
                    case DBConstantes.TableType.Type.dataVehicle:
                    case DBConstantes.TableType.Type.dataVehicle4M:
                        sqlUnit.AppendFormat("sum({0}{1}) as {2}", dataTablePrefixe, unitsList[i].DatabaseField, unitsList[i].Id.ToString());
                        break;
                    case DBConstantes.TableType.Type.webPlan:
                        sqlUnit.AppendFormat("sum({0}{1}) as {2}", dataTablePrefixe, unitsList[i].DatabaseMultimediaField, unitsList[i].Id.ToString());
                        break;
                }
            }
            return sqlUnit.ToString();
        }

        /// <summary>
        /// Get unit field to use in query
        /// </summary>
        ///<param name="webSession">Web session</param>
        /// <param name="vehicleId">Vehicle id</param>
        /// <param name="periodType">Period type</param>
        /// <returns>Unit field name</returns>
        public static string GetUnitFieldName(WebSession webSession, Int64 vehicleId, CstPeriod.PeriodBreakdownType periodType) {
            switch (periodType) {
                case CstPeriod.PeriodBreakdownType.week:
                case CstPeriod.PeriodBreakdownType.month:

                    try {
                        return webSession.GetSelectedUnit().DatabaseMultimediaField;
                    }
                    catch {
                        throw (new SQLGeneratorException("Selected unit detail is uncorrect. Unable to determine unit field."));
                    }

                case CstPeriod.PeriodBreakdownType.data:
                case CstPeriod.PeriodBreakdownType.data_4m:

                    try {
                            CstCustomerSessions.Unit unit = VehiclesInformation.Get(vehicleId).GetUnitFromBaseId(webSession.Unit);
                            return UnitsInformation.List[unit].DatabaseField;
                    }
                    catch {
                        throw (new SQLGeneratorException("Selected unit detail is uncorrect. Unable to determine unit field."));
                    }

                default:
                    throw (new SQLGeneratorException("Selected period detail is uncorrect. Unable to determine unit field."));

            }
        }

        /// <summary>
        /// Obtient les champs unit�s � utiliser en fonction du media
        /// </summary>
        /// <param name="webSession">Web Session</param>
        /// <param name="type">Type de la table</param>
        /// <returns>NOms des champs coorespondant aux unit�s s�lectionn�es</returns>
        public static string GetUnitFieldsNameForPortofolio(WebSession webSession, DBConstantes.TableType.Type type){
            return GetUnitFieldsNameForPortofolio(webSession, type, string.Empty);
        }

        /// <summary>
        /// Obtient les champs unit�s � utiliser en fonction du media
        /// </summary>
        /// <param name="webSession">Web Session</param>
        /// <param name="type">Type de la table</param>
        /// <returns>NOms des champs coorespondant aux unit�s s�lectionn�es</returns>
        public static string GetUnitFieldsNameForPortofolio(WebSession webSession, DBConstantes.TableType.Type type, string prefixe) {
            try {
                List<UnitInformation> unitsList = webSession.GetValidUnitForResult();
                string sqlUnit = "";
                bool first = true;

                foreach (UnitInformation currentUnit in unitsList) {
                    if(currentUnit.Id != CstCustomerSessions.Unit.versionNb) {
                        switch(type) {
                            case DBConstantes.TableType.Type.dataVehicle:
                            case DBConstantes.TableType.Type.dataVehicle4M:
                                if(!first) sqlUnit += ", ";
                                else first = false;
                                sqlUnit += currentUnit.GetSQLDetailledSum(prefixe);
                                break;
                            case DBConstantes.TableType.Type.webPlan:
                                if(!first) sqlUnit += ", ";
                                else first = false;
                                sqlUnit += currentUnit.GetSQLSum(prefixe);
                                break;
                        }
                    }   
                }
                return sqlUnit;
            }
            catch {
                throw new SQLGeneratorException("GetUnitFieldsNameForPortofolio(WebSession webSession,DBClassificationConstantes.Vehicles.names vehicleName, DBConstantes.TableType.Type type)-->The type of module is not managed for the selection of unit.");
            }
        }


        /// <summary>
        /// Obtient les champs unit�s � utiliser en fonction du media
        /// </summary>
        /// <param name="webSession">Web Session</param>
        /// <returns>NOms des champs coorespondant aux unit�s s�lectionn�es</returns>
        public static string GetUnitFieldsNameUnionForPortofolio(WebSession webSession)
        {
            try
            {
                List<UnitInformation> unitsList = webSession.GetValidUnitForResult();
                string sqlUnit = "";
                bool first = true;

                foreach (UnitInformation currentUnit in unitsList)
                {
                    if (currentUnit.Id != CstCustomerSessions.Unit.versionNb)
                    {
                        if (!first) sqlUnit += ", ";
                        else first = false;
                        sqlUnit += currentUnit.GetSQLUnionSum();
                    }
                }
                return sqlUnit;
            }
            catch
            {
                throw new SQLGeneratorException("GetUnitFieldsNameUnionForPortofolio(WebSession webSession,DBClassificationConstantes.Vehicles.names vehicleName, DBConstantes.TableType.Type type)-->The type of module is not managed for the selection of unit.");
            }
        }

        /// <summary>
        /// Obtient les champs unit�s � utiliser en fonction du media pour les tendances
        /// </summary>
        /// <param name="vehicleName">type du m�dia</param>
        /// <param name="dataTablePrefixe">pr�fixe de la table des donn�es</param>
        /// <param name="dataTotalTablePrefixe">pr�fixe de la table des donn�es</param>
        /// <returns>NOms des champs coorespondant aux unit�s s�lectionn�es</returns>
        public static string GetUnitFieldsForTendency(DBClassificationConstantes.Vehicles.names vehicleName, string dataTablePrefixe, string dataTotalTablePrefixe)
        {

            string fields = "";

            switch (vehicleName)
            {
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:    
                case DBClassificationConstantes.Vehicles.names.others:
                    fields = dataTablePrefixe + ".EXPENDITURE_CUR, ";
                    fields += dataTablePrefixe + ".EXPENDITURE_PREV, ";
                    fields += dataTablePrefixe + ".EXPENDITURE_EVOL, ";
                    fields += dataTablePrefixe + ".INSERTION_CUR, ";
                    fields += dataTablePrefixe + ".INSERTION_PREV, ";
                    fields += dataTablePrefixe + ".INSERTION_EVOL, ";
                    fields += dataTablePrefixe + ".DURATION_CUR, ";
                    fields += dataTablePrefixe + ".DURATION_PREV, ";
                    fields += dataTablePrefixe + ".DURATION_EVOL, ";
                    fields += dataTotalTablePrefixe + ".EXPENDITURE_CUR as SUB_EXPENDITURE_CUR, ";
                    fields += dataTotalTablePrefixe + ".EXPENDITURE_PREV as SUB_EXPENDITURE_PREV, ";
                    fields += dataTotalTablePrefixe + ".EXPENDITURE_EVOL as SUB_EXPENDITURE_EVOL, ";
                    fields += dataTotalTablePrefixe + ".INSERTION_CUR as SUB_INSERTION_CUR, ";
                    fields += dataTotalTablePrefixe + ".INSERTION_PREV as SUB_INSERTION_PREV, ";
                    fields += dataTotalTablePrefixe + ".INSERTION_EVOL as SUB_INSERTION_EVOL, ";
                    fields += dataTotalTablePrefixe + ".DURATION_CUR as SUB_DURATION_CUR, ";
                    fields += dataTotalTablePrefixe + ".DURATION_PREV as SUB_DURATION_PREV, ";
                    fields += dataTotalTablePrefixe + ".DURATION_EVOL as SUB_DURATION_EVOL ";
                    return (fields);
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.press:
                    fields = " sum(" + dataTablePrefixe + ".MMC_CUR) as MMC_CUR, ";
                    fields += " sum(" + dataTablePrefixe + ".MMC_PREV) as MMC_PREV, ";
                    fields += " decode(sum(" + dataTablePrefixe + ".MMC_PREV),0,100,round(((sum(" + dataTablePrefixe + ".MMC_CUR)-sum(" + dataTablePrefixe + ".MMC_PREV))/sum(" + dataTablePrefixe + ".MMC_PREV))*100,'2')) as MMC_EVOL, ";
                    //					fields+=" round(((sum("+dataTablePrefixe+".MMC_CUR)-sum("+dataTablePrefixe+".MMC_PREV))/sum("+dataTablePrefixe+".MMC_PREV))*100,'2') as MMC_EVOL, ";
                    //fields+=" sum("+dataTablePrefixe+".MMC_EVOL) as MMC_EVOL, ";
                    fields += " sum(" + dataTablePrefixe + ".PAGE_CUR) as PAGE_CUR, ";
                    fields += " sum(" + dataTablePrefixe + ".PAGE_PREV) as PAGE_PREV , ";
                    fields += " decode(sum(" + dataTablePrefixe + ".PAGE_PREV),0,100,round(((sum(" + dataTablePrefixe + ".PAGE_CUR)-sum(" + dataTablePrefixe + ".PAGE_PREV))/sum(" + dataTablePrefixe + ".PAGE_PREV))*100,'2')) as PAGE_EVOL, ";
                    //					fields+=" round(((sum("+dataTablePrefixe+".PAGE_CUR)-sum("+dataTablePrefixe+".PAGE_PREV))/sum("+dataTablePrefixe+".PAGE_PREV))*100,'2') as PAGE_EVOL, ";
                    //fields+=" sum("+dataTablePrefixe+".PAGE_EVOL) as PAGE_EVOL, ";
                    fields += " sum(" + dataTablePrefixe + ".EXPENDITURE_CUR) as EXPENDITURE_CUR, ";
                    fields += " sum(" + dataTablePrefixe + ".EXPENDITURE_PREV) as EXPENDITURE_PREV, ";
                    fields += " decode(sum(" + dataTablePrefixe + ".EXPENDITURE_PREV),0,100,round(((sum(" + dataTablePrefixe + ".EXPENDITURE_CUR)-sum(" + dataTablePrefixe + ".EXPENDITURE_PREV))/sum(" + dataTablePrefixe + ".EXPENDITURE_PREV))*100,'2')) as EXPENDITURE_EVOL, ";
                    //					fields+=" round(((sum("+dataTablePrefixe+".EXPENDITURE_CUR)-sum("+dataTablePrefixe+".EXPENDITURE_PREV))/sum("+dataTablePrefixe+".EXPENDITURE_PREV))*100,'2') as EXPENDITURE_EVOL, ";
                    //fields+=" sum("+dataTablePrefixe+".EXPENDITURE_EVOL) as EXPENDITURE_EVOL, ";
                    fields += " sum(" + dataTablePrefixe + ".INSERTION_CUR) as INSERTION_CUR, ";
                    fields += " sum(" + dataTablePrefixe + ".INSERTION_PREV) as INSERTION_PREV, ";
                    fields += " decode(sum(" + dataTablePrefixe + ".INSERTION_PREV),0,100,round(((sum(" + dataTablePrefixe + ".INSERTION_CUR)-sum(" + dataTablePrefixe + ".INSERTION_PREV))/sum(" + dataTablePrefixe + ".INSERTION_PREV))*100,'2')) as INSERTION_EVOL, ";
                    //					fields+=" round(((sum("+dataTablePrefixe+".INSERTION_CUR)-sum("+dataTablePrefixe+".INSERTION_PREV))/sum("+dataTablePrefixe+".INSERTION_PREV))*100,'2') as INSERTION_EVOL, ";
                    //fields+=" sum("+dataTablePrefixe+".INSERTION_EVOL) as INSERTION_EVOL, ";
                    fields += " sum(" + dataTablePrefixe + ".DURATION_CUR) as DURATION_CUR, ";
                    fields += " sum(" + dataTablePrefixe + ".DURATION_PREV) as DURATION_PREV, ";
                    fields += " decode(sum(" + dataTablePrefixe + ".DURATION_PREV),0,100,round(((sum(" + dataTablePrefixe + ".DURATION_CUR)-sum(" + dataTablePrefixe + ".DURATION_PREV))/sum(" + dataTablePrefixe + ".DURATION_PREV))*100,'2')) as DURATION_EVOL, ";
                    //					fields+=" round(((sum("+dataTablePrefixe+".DURATION_CUR)-sum("+dataTablePrefixe+".DURATION_PREV))/sum("+dataTablePrefixe+".DURATION_PREV))*100,'2') as DURATION_EVOL, ";
                    //fields+=" sum("+dataTablePrefixe+".DURATION_EVOL) as DURATION_EVOL, ";
                    fields += " (sum(" + dataTotalTablePrefixe + ".MMC_CUR)/count(" + DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".MEDIA)) as SUB_MMC_CUR, ";
                    fields += " sum(" + dataTotalTablePrefixe + ".MMC_PREV)/count(" + DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".MEDIA)) as SUB_MMC_PREV, ";
                    fields += " sum(" + dataTotalTablePrefixe + ".MMC_EVOL)/count(" + DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".MEDIA)) as SUB_MMC_EVOL, ";
                    fields += " sum(" + dataTotalTablePrefixe + ".PAGE_CUR)/count(" + DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".MEDIA)) as SUB_PAGE_CUR, ";
                    fields += " sum(" + dataTotalTablePrefixe + ".PAGE_PREV)/count(" + DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".MEDIA)) as SUB_PAGE_PREV, ";
                    fields += " sum(" + dataTotalTablePrefixe + ".PAGE_EVOL)/count(" + DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".MEDIA)) as SUB_PAGE_EVOL, ";
                    fields += " sum(" + dataTotalTablePrefixe + ".EXPENDITURE_CUR)/count(" + DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".MEDIA)) as SUB_EXPENDITURE_CUR, ";
                    fields += " sum(" + dataTotalTablePrefixe + ".EXPENDITURE_PREV)/count(" + DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".MEDIA)) as SUB_EXPENDITURE_PREV, ";
                    fields += " sum(" + dataTotalTablePrefixe + ".EXPENDITURE_EVOL)/count(" + DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".MEDIA)) as SUB_EXPENDITURE_EVOL, ";
                    fields += " sum(" + dataTotalTablePrefixe + ".INSERTION_CUR)/count(" + DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".MEDIA)) as SUB_INSERTION_CUR, ";
                    fields += " sum(" + dataTotalTablePrefixe + ".INSERTION_PREV)/count(" + DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".MEDIA)) as SUB_INSERTION_PREV, ";
                    fields += " sum(" + dataTotalTablePrefixe + ".INSERTION_EVOL)/count(" + DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".MEDIA)) as SUB_INSERTION_EVOL, ";
                    fields += " sum(" + dataTotalTablePrefixe + ".DURATION_CUR)/count(" + DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".MEDIA)) as SUB_DURATION_CUR, ";
                    fields += " sum(" + dataTotalTablePrefixe + ".DURATION_PREV)/count(" + DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".MEDIA)) as SUB_DURATION_PREV, ";
                    fields += " sum(" + dataTotalTablePrefixe + ".DURATION_EVOL)/count(" + DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".MEDIA)) as SUB_DURATION_EVOL ";
                    return (fields);
                case DBClassificationConstantes.Vehicles.names.outdoor:
                case DBClassificationConstantes.Vehicles.names.indoor:
                case DBClassificationConstantes.Vehicles.names.instore:
                    fields = dataTablePrefixe + ".EXPENDITURE_CUR, ";
                    fields += dataTablePrefixe + ".EXPENDITURE_PREV, ";
                    fields += dataTablePrefixe + ".EXPENDITURE_EVOL, ";
                    fields += dataTablePrefixe + ".INSERTION_CUR, ";
                    fields += dataTablePrefixe + ".INSERTION_PREV, ";
                    fields += dataTablePrefixe + ".INSERTION_EVOL, ";
                    fields += dataTotalTablePrefixe + ".EXPENDITURE_CUR as SUB_EXPENDITURE_CUR, ";
                    fields += dataTotalTablePrefixe + ".EXPENDITURE_PREV as SUB_EXPENDITURE_PREV, ";
                    fields += dataTotalTablePrefixe + ".EXPENDITURE_EVOL as SUB_EXPENDITURE_EVOL, ";
                    fields += dataTotalTablePrefixe + ".INSERTION_CUR as SUB_INSERTION_CUR, ";
                    fields += dataTotalTablePrefixe + ".INSERTION_PREV as SUB_INSERTION_PREV, ";
                    fields += dataTotalTablePrefixe + ".INSERTION_EVOL as SUB_INSERTION_EVOL ";
                    return (fields);
                default:
                    throw new SQLGeneratorException("getUnitFields(ClassificationConstantes.Vehicle.type vehicleType)-->Le cas de cette m�dia n'est pas g�rer. Pas de champs correspondant.");
            }
        }
        #endregion

        #region Encarts
        /// <summary>
        /// Obtient les encarts pour le media presse
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="dataTablePrefixe">Pr�fixe de la table des donn�es</param>
        /// <returns>Jointures</returns>
        public static string GetJointForInsertDetail(WebSession webSession, string dataTablePrefixe) {
			
			Module currentModuleDescription = ModulesList.GetModule(webSession.CurrentModule);
            string insertMediaList = string.Empty;
            if (VehiclesInformation.Contains(DBClassificationConstantes.Vehicles.names.press))
                insertMediaList += VehiclesInformation.Get(DBClassificationConstantes.Vehicles.names.press).DatabaseId;
            if (VehiclesInformation.Contains(DBClassificationConstantes.Vehicles.names.internationalPress))
            {
                if (insertMediaList.Length > 0) insertMediaList += ",";
                insertMediaList += VehiclesInformation.Get(DBClassificationConstantes.Vehicles.names.internationalPress).DatabaseId;
            }
            if (VehiclesInformation.Contains(DBClassificationConstantes.Vehicles.names.newspaper))
            {
                if (insertMediaList.Length > 0) insertMediaList += ",";
                insertMediaList += VehiclesInformation.Get(DBClassificationConstantes.Vehicles.names.newspaper).DatabaseId;
            }
            if (VehiclesInformation.Contains(DBClassificationConstantes.Vehicles.names.magazine))
            {
                if (insertMediaList.Length > 0) insertMediaList += ",";
                insertMediaList += VehiclesInformation.Get(DBClassificationConstantes.Vehicles.names.magazine).DatabaseId;
            }


			switch (webSession.Insert) {
				case WebConstantes.CustomerSessions.Insert.total:
					return "";
				case WebConstantes.CustomerSessions.Insert.withOutInsert:
					return string.Format(" and  ( " + dataTablePrefixe + ".id_inset=0 or " + dataTablePrefixe + ".id_inset is null )");
                    return string.Format(" and (({0}.id_vehicle not in({1})) or ({0}.id_vehicle in({1}) and ( {0}.id_inset=0 or {0}.id_inset is null ) ))", dataTablePrefixe, insertMediaList);
                case WebConstantes.CustomerSessions.Insert.insert:
                    string fieldsList = webSession.CustomerDataFilters.InsetTypesAsString; //Lists.GetIdList(WebConstantes.GroupList.ID.inset);
                    if (fieldsList != null && fieldsList.Length > 0)
                    {
                        return string.Format(" and (({0}.id_vehicle not in({2})) or ({0}.id_vehicle in({2}) and {0}.id_inset in ({1})))", dataTablePrefixe, fieldsList, insertMediaList);
                    }
                    else return "";
				default:
					throw new SQLGeneratorException("getJointForInsertDetail(WebSession webSession,string dataTablePrefixe)--> Impossible to retreive inset type..");
			}
		}

		//TODO : Fonction a supprimer des que G.R aura archiver module presents/absent
		/// <summary>
		/// Obtient les encarts pour le media presse
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dataTablePrefixe">Pr�fixe de la table des donn�es</param>
		/// <param name="type">Type de la table</param>
		/// <returns>Jointures</returns>
		public static string GetJointForInsertDetail(WebSession webSession, string dataTablePrefixe, DBConstantes.TableType.Type type) {
			return GetJointForInsertDetail( webSession,  dataTablePrefixe);
		}
        #endregion

        #region Niveau Produit
        /// <summary>
        /// Retourne une condition sur le niveau de d�tail produit
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="tablePrefixe">Pr�fixe de la table</param>
        /// <param name="beginByAnd">d�bute par and</param>
        /// <returns>Code SQL G�n�r�</returns>
        public static string getLevelProduct(WebSession webSession, string tablePrefixe, bool beginByAnd)
        {

            string sql = "";

            if (webSession.ProductDetailLevel != null)
            {
                if (beginByAnd) sql += " and";

                switch (webSession.ProductDetailLevel.LevelProduct)
                {
                    case (ClassificationConstantes.Level.type.product):
                        sql += " " + tablePrefixe + ".id_product in (" + webSession.GetSelection(webSession.ProductDetailLevel.ListElement, CustomerRightConstante.type.productAccess) + ")";
                        break;
                    case (ClassificationConstantes.Level.type.segment):
                        sql += " " + tablePrefixe + ".id_segment in (" + webSession.GetSelection(webSession.ProductDetailLevel.ListElement, CustomerRightConstante.type.segmentAccess) + ")";
                        break;
                    case (ClassificationConstantes.Level.type.group):
                        sql += " " + tablePrefixe + ".id_group_ in (" + webSession.GetSelection(webSession.ProductDetailLevel.ListElement, CustomerRightConstante.type.groupAccess) + ") ";
                        break;
                    case (ClassificationConstantes.Level.type.subsector):
                        sql += " " + tablePrefixe + ".id_subSector in (" + webSession.GetSelection(webSession.ProductDetailLevel.ListElement, CustomerRightConstante.type.subSectorAccess) + ")";
                        break;
                    case (ClassificationConstantes.Level.type.sector):
                        sql += " " + tablePrefixe + ".id_sector in (" + webSession.GetSelection(webSession.ProductDetailLevel.ListElement, CustomerRightConstante.type.sectorAccess) + ")";
                        break;
                    case (ClassificationConstantes.Level.type.advertiser):
                        sql += " " + tablePrefixe + ".id_advertiser in (" + webSession.GetSelection(webSession.ProductDetailLevel.ListElement, CustomerRightConstante.type.advertiserAccess) + ")";
                        break;
                    case (ClassificationConstantes.Level.type.holding_company):
                        sql += " " + tablePrefixe + ".id_holding_company in (" + webSession.GetSelection(webSession.ProductDetailLevel.ListElement, CustomerRightConstante.type.holdingCompanyAccess) + ")";
                        break;
                    case (ClassificationConstantes.Level.type.brand):
                        sql += " " + tablePrefixe + ".id_brand in (" + webSession.GetSelection(webSession.ProductDetailLevel.ListElement, CustomerRightConstante.type.brandAccess) + ")";
                        break;
                }
            }

            return (sql);
        }

        #endregion

        #region Niveau Media

        /// <summary>
        /// Retourne une condition sur le niveau de d�tail m�dia
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="tablePrefixe">Pr�fixe de la table</param>
        /// <param name="beginByAnd">d�bute par and</param>
        /// <returns>Code SQL G�n�r�</returns>
        public static string getLevelMedia(WebSession webSession, string tablePrefixe, bool beginByAnd)
        {

            string sql = "";

            if (webSession.MediaDetailLevel != null)
            {
                if (beginByAnd) sql += " and";

                switch (webSession.MediaDetailLevel.LevelMedia)
                {
                    case (ClassificationConstantes.Level.type.media):
                        sql += " " + tablePrefixe + ".id_media in (" + webSession.GetSelection(webSession.MediaDetailLevel.ListElement, CustomerRightConstante.type.mediaAccess) + ")";
                        break;
                    case (ClassificationConstantes.Level.type.category):
                        sql += " " + tablePrefixe + ".id_category in (" + webSession.GetSelection(webSession.MediaDetailLevel.ListElement, CustomerRightConstante.type.categoryAccess) + ")";
                        break;
                    case (ClassificationConstantes.Level.type.region):
                        sql += " " + tablePrefixe + ".id_region in (" + webSession.GetSelection(webSession.MediaDetailLevel.ListElement, CustomerRightConstante.type.regionAccess) + ")";
                        break;
                    case (ClassificationConstantes.Level.type.vehicle):
                        sql += " " + tablePrefixe + ".id_vehicle in (" + webSession.GetSelection(webSession.MediaDetailLevel.ListElement, CustomerRightConstante.type.vehicleAccess) + ") ";
                        break;
                }
            }

            return (sql);
        }

        /// <summary>
        /// Obtient le libell� du m�dia de niveau 2
        /// </summary>
        /// <param name="webSession">session client</param>
        /// <returns></returns>
        public static string GetLevel2MediaFields(WebSession webSession)
        {

            switch (webSession.PreformatedMediaDetail)
            {
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
                    return DBConstantes.Tables.CATEGORY;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
                    return DBConstantes.Tables.INTEREST_CENTER;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
                    return DBConstantes.Tables.MEDIA;
                default:
                    return "";
            }

        }

        /// <summary>
        /// Obtient le libell� du m�dia de niveau 2
        /// </summary>
        /// <param name="webSession">session client</param>
        /// <returns></returns>
        public static string GetLevel2IdMediaFields(WebSession webSession)
        {

            switch (webSession.PreformatedMediaDetail)
            {
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
                    return "id_category";
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
                    return "id_interest_center";
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
                    return "id_media";
                default:
                    return "";
            }

        }
        #endregion

        /// <summary>
        /// For the Sale_Type value of outdoor
        /// </summary>
        /// <param name="saleType">SaleType</param>
        /// <param name="siteLanguage">Site language</param>
        /// <returns>value of saleType in string </returns>
        public static string SaleTypeOutdoor(string saleType, int siteLanguage)
        {
            switch (saleType)
            {
                case DBConstantes.TypeSaleOutdoor.MIXTE:
                    return (GestionWeb.GetWebWord(305, siteLanguage));
                case DBConstantes.TypeSaleOutdoor.LOCALE:
                    return (GestionWeb.GetWebWord(1625, siteLanguage));
                case DBConstantes.TypeSaleOutdoor.NATIONALE:
                    return (GestionWeb.GetWebWord(1624, siteLanguage));
                case DBConstantes.TypeSaleOutdoor.REGIONALE:
                    return (GestionWeb.GetWebWord(1623, siteLanguage));
                default:
                    return "";
            }
        }

    }

}
