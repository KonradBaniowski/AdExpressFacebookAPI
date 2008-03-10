#region Informations
// Auteur: D. Mussuma
// Date de création: 12/11/2007
// Date de modification: 
#endregion
using System;
using System.Data;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.DB.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
//using TNS.AdExpress.Web.Functions;
using System.Collections.Generic;
using CoreExceptions = TNS.AdExpress.Web.Core.Exceptions;
using CustomerRightConstante = TNS.AdExpress.Constantes.Customer.Right;

namespace TNS.AdExpress.Web.Core.DataAccess.ClassificationList {
	/// <summary>
	/// Description résumée de SearchLevelDataAccess
	/// </summary>
	public class SearchLevelDataAccess {

		#region GetItems
		/// <summary>
		/// Obtient la liste d'éléments contenant le terme contenu dans wordToSearch dans le niveau de nomenclature table
		/// </summary>
		/// <param name="table">Table de la nomenclature</param>
		/// <param name="wordToSearch">Terme recherché</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="dBSchema"> DB Schema</param>
		/// <returns>Le nombre d'élément de la nomenclature trouvé</returns>
		public static DataSet GetItems(string table, string wordToSearch, WebSession webSession,string dBSchema) {
			
			string productRight = getCustomerProductRight(webSession, "wp", true);
			string productRightSql = "";

			//Get all product rights
			if (productRight != null && productRight.Length > 0) {
				productRightSql  = " select id_" + table.ToString() +" from ";
				productRightSql += dBSchema + ".ALL_PRODUCT_" + webSession.SiteLanguage.ToString() + " wp";
				productRightSql += " Where 0=0 " + productRight;
			}

			#region Construction de la requête
			string sql = "select distinct wp.id_" + table.ToString() + " as id_item, wp." + table.ToString() + " as item ";
			sql += " from " + dBSchema + "." + table + " wp";
			
			if (productRightSql != null && productRightSql.Length > 0) {
				sql += ",( " + productRightSql + " ) wp2 ";
			}

			//Word to search			
			wordToSearch = wordToSearch.ToUpper().Replace("'", " ");
			wordToSearch = wordToSearch.ToUpper().Replace("*", "%");
			wordToSearch = "'" + wordToSearch.Trim() + "%'";

			sql += " where wp." + table.ToString() + " like " + wordToSearch + "";
			sql += " and wp.id_language=" + webSession.SiteLanguage;
			sql += " and wp.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;

			if (productRightSql != null && productRightSql.Length > 0) {
				sql += " and  wp.id_" + table.ToString() + " =  wp2.id_" + table.ToString();
				sql += " and rownum <= 1000 ";
			}			

			sql += " order by " + table.ToString();
			#endregion

			#region Excution de la requête
			try {
				return webSession.Source.Fill(sql);
			}
			catch (System.Exception e) {
				throw new CoreExceptions.SearchLevelDataAccessException(e.Message, e);
			}
			#endregion
		}

		/// <summary>
		/// Obtient la liste d'éléments contenant les identifiants contenu dans idList dans le niveau de nomenclature table
		/// </summary>
		/// <param name="table">Table de la nomenclature</param>
		/// <param name="idList">Liste d'identifiant d'éléments</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="dBSchema"> DB Schema</param>
		/// <returns>Le nombre d'élément de la nomenclature trouvé</returns>
		public static DataSet GetOneLevelItems(string table, string idList, WebSession webSession, string dBSchema) {
			string sqlRights = "";

			#region Construction de la requête
			string sql = "select distinct wp.id_" + table.ToString() + " as id_item, wp." + table.ToString() + " as item ";
			sql += " from " + dBSchema + ".ALL_PRODUCT_" + webSession.SiteLanguage.ToString() + " wp ";
			sql += " where wp.id_" + table.ToString() + " in (" + idList + ")";
			
			#region Application des droits produits
			sqlRights = getCustomerProductRight(webSession, "wp", true);
			if (sqlRights != null && sqlRights.Length > 0) {
				sql += sqlRights;
			}
			#endregion

			sql += " order by " + table.ToString();
			#endregion

			#region Excution de la requête
			try {
				return webSession.Source.Fill(sql);
			}
			catch (System.Exception e) {
				throw new CoreExceptions.SearchLevelDataAccessException(e.Message, e);
			}
			#endregion
		}

		/// <summary>
		/// Obtient la liste d'éléments contenant les identifiants contenu dans idList dans le niveau de nomenclature table
		/// </summary>
		/// <param name="table">Table de la nomenclature</param>
		/// <param name="idList">Liste d'identifiant d'éléments</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="dBSchema"> DB Schema</param>
		/// <returns>Le nombre d'élément de la nomenclature trouvé</returns>
		public static DataSet GetSelectedItems(string table, string idList, WebSession webSession, string dBSchema) {
			string sqlRights = "";

			#region Construction de la requête
			string sql = "select distinct wp.id_" + table.ToString() + " as id_item, wp." + table.ToString() + " as item ";
			sql += " from " + dBSchema + ".ALL_PRODUCT_" + webSession.SiteLanguage.ToString() + " wp";
			sql += " where wp.id_" + table.ToString() + " in (" + idList + ")";

			#region Application des droits produits
			sqlRights = getCustomerProductRight(webSession, "wp", true); 
			if (sqlRights != null && sqlRights.Length > 0) {
				sql += sqlRights;
			}
			#endregion
			sql += " order by " + table.ToString();
			#endregion

			#region Excution de la requête
			try {
				return webSession.Source.Fill(sql);
			}
			catch (System.Exception e) {
				throw new CoreExceptions.SearchLevelDataAccessException(e.Message, e);
			}
			#endregion
		}
		#endregion

		#region GetItemsHierarchy
		/// <summary>
		/// Obtient la liste d'éléments associés à ID de l'élément sélectionné dans le Niveau de la nomenclature
		/// </summary>
		/// <param name="table">Table de la nomenclature</param>
		/// <param name="selectedItemIds">ID des éléments sélectionnés dans le Niveau de la nomenclature en cours</param>
		/// <param name="webSession">Session client</param>
		/// <param name="dBSchema"> DB Schema</param>
		/// <param name="selectedItemTableName">selected Item TableName</param>
		/// <returns>listes d'éléments </returns>
		public static DataSet GetItems(string table, string selectedItemIds, string selectedItemTableName, WebSession webSession, string dBSchema) {
			string sqlRights = "";
			
			#region Construction de la requête
			string sql = "select distinct wp.id_" + table.ToString() + " as id_item, wp." + table.ToString() + " as item ";
			sql += " from " + dBSchema + ".ALL_PRODUCT_" + webSession.SiteLanguage.ToString() + " wp"; 
			sql += " where wp.id_" + selectedItemTableName + " in ( " + selectedItemIds + " ) ";

			#region Application des droits produits
			sqlRights = getCustomerProductRight(webSession, "wp", true); 
			if (sqlRights != null && sqlRights.Length > 0) {
				sql += sqlRights;
			}
			#endregion

			sql += " order by " + table.ToString();
			#endregion

			#region Excution de la requête
			try {
				return webSession.Source.Fill(sql);
			}
			catch (System.Exception e) {
				throw new CoreExceptions.SearchLevelDataAccessException(e.Message, e);
			}
			#endregion
		}
		#endregion

		#region Droits client
		/// <summary>
		/// Génère les droits clients Produit.
		/// Cette fonction est à utiliser si une même table contient tous les identifiants de la nomenclature produit.
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="tablePrefixe">Préfixe de la table qui contient les données</param>
		/// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
		/// <returns>Code SQL généré</returns>
		public static string getCustomerProductRight(WebSession webSession, string tablePrefixe, bool beginByAnd) {
			return (getClassificationCustomerProductRight(webSession, tablePrefixe, tablePrefixe, tablePrefixe, tablePrefixe,tablePrefixe,tablePrefixe,tablePrefixe,tablePrefixe, beginByAnd));
		}

		/// <summary>
		/// Génère les droits clients Produit.
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="sectorPrefixe">Préfixe de la table sector</param>
		/// <param name="subsectorPrefixe">Préfixe de la table subsector</param>
		/// <param name="groupPrefixe">Préfixe de la table group_</param>
		/// <param name="segmentPrefixe">Préfixe de la table segment</param>
		/// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
		/// <returns>Code SQL généré</returns>
		public static string getClassificationCustomerProductRight(WebSession webSession, string sectorPrefixe, string subsectorPrefixe, string groupPrefixe, string segmentPrefixe,string holdingCompanyPrefixe,string advertiserPrefixe, string brandPrefixe,string productPrefixe,bool beginByAnd) {
			string sql = "";
			bool premier = true;
			// Sector (Famille)
			if (webSession.CustomerLogin[CustomerRightConstante.type.sectorAccess].Length > 0) {
				if (beginByAnd) sql += " and";
				sql += " ((" + sectorPrefixe + ".id_sector in (" + webSession.CustomerLogin[CustomerRightConstante.type.sectorAccess] + ") ";
				premier = false;
			}
			// SubSector (Classe)
			if (webSession.CustomerLogin[CustomerRightConstante.type.subSectorAccess].Length > 0) {
				if (!premier) sql += " or";
				else {
					if (beginByAnd) sql += " and";
					sql += " ((";
				}
				sql += " " + subsectorPrefixe + ".id_subsector in (" + webSession.CustomerLogin[CustomerRightConstante.type.subSectorAccess] + ") ";
				premier = false;
			}
			// Group (Groupe)
			if (webSession.CustomerLogin[CustomerRightConstante.type.groupAccess].Length > 0) {
				if (!premier) sql += " or";
				else {
					if (beginByAnd) sql += " and";
					sql += " ((";
				}
				sql += " " + groupPrefixe + ".id_group_ in (" + webSession.CustomerLogin[CustomerRightConstante.type.groupAccess] + ") ";
				premier = false;
			}
			// Segment (Variété)
			if (webSession.CustomerLogin[CustomerRightConstante.type.segmentAccess].Length > 0) {
				if (!premier) sql += " or";
				else {
					if (beginByAnd) sql += " and";
					sql += " ((";
				}
				sql += " " + segmentPrefixe + ".id_segment in (" + webSession.CustomerLogin[CustomerRightConstante.type.segmentAccess] + ") ";
				premier = false;
			}
			// Holding Company 
			if (webSession.CustomerLogin[CustomerRightConstante.type.holdingCompanyAccess].Length > 0) {
				if (!premier) sql += " or";
				else {
					if (beginByAnd) sql += " and";
					sql += " ((";
				}
				sql += " " + holdingCompanyPrefixe + ".id_holding_company in (" + webSession.CustomerLogin[CustomerRightConstante.type.holdingCompanyAccess] + ") ";
				premier = false;
			}
			// Advertiser
			if (webSession.CustomerLogin[CustomerRightConstante.type.advertiserAccess].Length > 0) {
				if (!premier) sql += " or";
				else {
					if (beginByAnd) sql += " and";
					sql += " ((";
				}
				sql += " " + advertiserPrefixe + ".id_advertiser in (" + webSession.CustomerLogin[CustomerRightConstante.type.advertiserAccess] + ") ";
				premier = false;
			}
			// Brand
			if (webSession.CustomerLogin[CustomerRightConstante.type.brandAccess].Length > 0) {
				if (!premier) sql += " or";
				else {
					if (beginByAnd) sql += " and";
					sql += " ((";
				}
				sql += " " + brandPrefixe + ".id_brand in (" + webSession.CustomerLogin[CustomerRightConstante.type.brandAccess] + ") ";
				premier = false;
			}
			// Product
			if (webSession.CustomerLogin[CustomerRightConstante.type.productAccess].Length > 0) {
				if (!premier) sql += " or";
				else {
					if (beginByAnd) sql += " and";
					sql += " ((";
				}
				sql += " " + productPrefixe + ".id_product in (" + webSession.CustomerLogin[CustomerRightConstante.type.productAccess] + ") ";
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
				sql += " " + sectorPrefixe + ".id_sector not in (" + webSession.CustomerLogin[CustomerRightConstante.type.sectorException] + ") ";
				premier = false;
			}
			// SubSector (Classe)
			if (webSession.CustomerLogin[CustomerRightConstante.type.subSectorException].Length > 0) {
				if (!premier) sql += " and";
				else {
					if (beginByAnd) sql += " and";
					sql += " (";
				}
				sql += " " + subsectorPrefixe + ".id_subsector not in (" + webSession.CustomerLogin[CustomerRightConstante.type.subSectorException] + ") ";
				premier = false;
			}
			// Group (Groupe)
			if (webSession.CustomerLogin[CustomerRightConstante.type.groupException].Length > 0) {
				if (!premier) sql += " and";
				else {
					if (beginByAnd) sql += " and";
					sql += " (";
				}
				sql += " " + groupPrefixe + ".id_group_  not in (" + webSession.CustomerLogin[CustomerRightConstante.type.groupException] + ") ";
				premier = false;
			}
			// Segment (Variété)
			if (webSession.CustomerLogin[CustomerRightConstante.type.segmentException].Length > 0) {
				if (!premier) sql += " and";
				else {
					if (beginByAnd) sql += " and";
					sql += " (";
				}
				sql += " " + segmentPrefixe + ".id_segment  not in (" + webSession.CustomerLogin[CustomerRightConstante.type.segmentException] + ") ";
				premier = false;
			}
			// Holding Company
			if (webSession.CustomerLogin[CustomerRightConstante.type.holdingCompanyException].Length > 0) {
				if (!premier) sql += " and";
				else {
					if (beginByAnd) sql += " and";
					sql += " (";
				}
				sql += " " + holdingCompanyPrefixe + ".id_holding_company  not in (" + webSession.CustomerLogin[CustomerRightConstante.type.holdingCompanyException] + ") ";
				premier = false;
			}
			// Advertiser
			if (webSession.CustomerLogin[CustomerRightConstante.type.advertiserException].Length > 0) {
				if (!premier) sql += " and";
				else {
					if (beginByAnd) sql += " and";
					sql += " (";
				}
				sql += " " + advertiserPrefixe + ".id_advertiser  not in (" + webSession.CustomerLogin[CustomerRightConstante.type.advertiserException] + ") ";
				premier = false;
			}
			// Brand
			if (webSession.CustomerLogin[CustomerRightConstante.type.brandException].Length > 0) {
				if (!premier) sql += " and";
				else {
					if (beginByAnd) sql += " and";
					sql += " (";
				}
				sql += " " + brandPrefixe + ".id_brand  not in (" + webSession.CustomerLogin[CustomerRightConstante.type.brandException] + ") ";
				premier = false;
			}
			// Product
			if (webSession.CustomerLogin[CustomerRightConstante.type.productException].Length > 0) {
				if (!premier) sql += " and";
				else {
					if (beginByAnd) sql += " and";
					sql += " (";
				}
				sql += " " + productPrefixe + ".id_product  not in (" + webSession.CustomerLogin[CustomerRightConstante.type.productException] + ") ";
				premier = false;
			}
			if (!premier) sql += " )";
			return (sql);
		}

		#endregion
	}
}
