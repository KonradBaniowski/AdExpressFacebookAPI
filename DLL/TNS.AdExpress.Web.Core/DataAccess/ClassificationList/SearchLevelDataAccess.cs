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
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.Classification.Universe;
using TNS.AdExpress.Web.Core.Utilities;

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
        public static DataSet GetItems(string table, string wordToSearch, WebSession webSession, string dBSchema, Dimension dimension) {

            #region Tables initilization
			View oView = null;
            string classificationRight = "";            
			bool useView = true;

			try {
				oView = GetView(webSession, dimension);
				classificationRight = GetRights(webSession, dimension);
				useView = CanUseView(webSession, dimension, classificationRight);
			}
			catch (System.Exception err) {
				throw (new CoreExceptions.SearchLevelDataAccessException("Impossible to get view names or rights", err));
			}
            #endregion

			#region Construction de la requête
			string sql = "select distinct wp.id_" + table.ToString() + " as id_item, wp." + table.ToString() + " as item ";
			sql += GetFromClause(webSession,oView,table,dBSchema,useView);

			//Word to search			
			wordToSearch = wordToSearch.ToUpper().Replace("'", " ");
			wordToSearch = wordToSearch.ToUpper().Replace("*", "%");
			wordToSearch = "'" + wordToSearch.Trim() + "%'";

			sql += " where wp." + table.ToString() + " like " + wordToSearch + "";
			sql += GetJointClause(webSession, oView, table, dimension, classificationRight, useView);
			sql += " order by " + table.ToString();
			#endregion

			#region Excution de la requête
			try {
				return GetSource(webSession).Fill(sql);
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
		/// <param name="dimension">dimension</param>
		/// <param name="dBSchema"> DB Schema</param>
		/// <returns>Le nombre d'élément de la nomenclature trouvé</returns>
        public static DataSet GetOneLevelItems(string table, string idList, WebSession webSession, string dBSchema, Dimension dimension) {
			
            #region Tables initilization
            View oView = null;
            string classificationRight = "";
			bool useView = true;

			try {
				oView = GetView(webSession, dimension);
				classificationRight = GetRights(webSession, dimension);
				useView = CanUseView(webSession, dimension, classificationRight);
			}
			catch (System.Exception err) {
				throw (new CoreExceptions.SearchLevelDataAccessException("Impossible to get view names or rights", err));
			}
            #endregion

			#region Construction de la requête
			string sql = "select distinct wp.id_" + table.ToString() + " as id_item, wp." + table.ToString() + " as item ";
			sql += GetFromClause(webSession, oView, table,dBSchema, useView);
			sql += " where wp.id_" + table.ToString() + " in (" + idList + ")";
			sql += GetJointClause(webSession, oView, table, dimension, classificationRight, useView);
			sql += " order by " + table.ToString();
			#endregion

			#region Excution de la requête
			try {
				return GetSource(webSession).Fill(sql);
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
        public static DataSet GetSelectedItems(string table, string idList, WebSession webSession, string dBSchema, Dimension dimension) {

            #region Tables initilization
			View oView = null;
            string classificationRight = "";
			bool useView = true;

			try {
				oView = GetView(webSession, dimension);
				classificationRight = GetRights(webSession, dimension);
				useView = CanUseView(webSession, dimension, classificationRight);
			}
			catch (System.Exception err) {
				throw (new CoreExceptions.SearchLevelDataAccessException("Impossible to get view names or rights", err));
			}
            #endregion

			#region Construction de la requête
			string sql = "select distinct wp.id_" + table.ToString() + " as id_item, wp." + table.ToString() + " as item ";
			sql += GetFromClause(webSession, oView, table,dBSchema, useView);
			sql += " where wp.id_" + table.ToString() + " in (" + idList + ")";
			sql += GetJointClause(webSession, oView, table, dimension, classificationRight, useView);
			sql += " order by " + table.ToString();
			#endregion

			#region Excution de la requête
			try {
				return GetSource(webSession).Fill(sql);
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
		public static DataSet GetItems(string table, string selectedItemIds, string selectedItemTableName, WebSession webSession, string dBSchema, Dimension dimension) {

            #region Tables initilization
			View oView = null;
            string classificationRight = "";

            try {				
				oView = GetView(webSession, dimension);
				classificationRight = GetRights(webSession, dimension);
			}
            catch(System.Exception err) {
                throw (new CoreExceptions.SearchLevelDataAccessException("Impossible to get view names or rights",err));
            }
            #endregion

			#region Construction de la requête
			string sql = "select distinct wp.id_" + table.ToString() + " as id_item, wp." + table.ToString() + " as item ";
            sql += " from " + oView.Sql + webSession.DataLanguage.ToString() + " wp"; 
			sql += " where wp.id_" + selectedItemTableName + " in ( " + selectedItemIds + " ) ";

			#region Application des droits
            if(classificationRight != null && classificationRight.Length > 0) {
                sql += classificationRight;
			}
			#endregion

            switch(webSession.CurrentModule) {
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA:
                    if(dimension == Dimension.media)
                        sql += " and wp.id_vehicle in (" + webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess) + ") ";
                    break;
            }

			sql += " order by " + table.ToString();
			#endregion

			#region Excution de la requête
			try {
				return GetSource(webSession).Fill(sql);
			}
			catch (System.Exception e) {
				throw new CoreExceptions.SearchLevelDataAccessException(e.Message, e);
			}
			#endregion

		}
		#endregion

		#region GetItems
		/// <summary>
		/// Obtient la liste d'éléments contenant le terme contenu dans wordToSearch dans le niveau de nomenclature table
		/// </summary>
		/// <param name="table">Table de la nomenclature</param>
		/// <param name="wordToSearch">Terme recherché</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="dBSchema"> DB Schema</param>
		/// <returns>Le nombre d'élément de la nomenclature trouvé</returns>
		public static DataSet GetRecapItems(string table, WebSession webSession, string dBSchema,int universeLevelId, Dimension dimension) {

			#region Tables Variables
			View oView = null;
			string classificationRight = "";
			bool useView = false;
			
			#endregion

			#region Construction de la requête
			string sql = "select distinct wp.id_" + table.ToString() + " as id_item, wp." + table.ToString() + " as item ";
			sql += " from " + dBSchema + "." + table + " wp ";		

			sql += " where ";
			sql += "  wp.id_language = " + webSession.DataLanguage;
			sql += GetAccessRights(webSession, universeLevelId, "id_" + table, true);
			sql += " and wp.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql += " order by " + table.ToString();
			#endregion

			#region Excution de la requête
			try {
				return GetSource(webSession).Fill(sql);
			}
			catch (System.Exception e) {
				throw new CoreExceptions.SearchLevelDataAccessException(e.Message, e);
			}
			#endregion
		}

		
		#endregion

		#region Droits client

        #region Supports
        /// <summary>
		/// Génère les droits clients Media.
		/// Cette fonction est à utiliser si une même table contient tous les identifiants de la nomenclature support.
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="tablePrefixe">Préfixe de la table qui contient les données</param>
		/// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
		/// <returns>Code SQL généré</returns>
        public static string getCustomerMediaRight(WebSession webSession, string tablePrefixe, bool beginByAnd) {
            return getClassificationCustomerMediaRight(webSession, tablePrefixe, tablePrefixe, tablePrefixe, beginByAnd);
        }

        /// <summary>
        /// Génère les droits media clients .
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="vehiclePrefixe">Préfixe de la table vehicle (media)</param>
        /// <param name="categoryPrefixe">Préfixe de la table category</param>
        /// <param name="mediaPrefixe">Préfixe de la table media (support)</param>		
        /// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
        /// <returns>Code SQL généré</returns>  
        public static string getClassificationCustomerMediaRight(WebSession webSession, string vehiclePrefixe, string categoryPrefixe, string mediaPrefixe, bool beginByAnd) {
            string sql = "";
            bool premier = true;
            // vehicle (media)
            if(webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess].Length > 0) {
                if(beginByAnd) sql += " and";
                sql += " ((" + SQLGenerator.GetInClauseMagicMethod(vehiclePrefixe + ".id_vehicle", webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess], true) + " ";
                premier = false;
            }
            // category (Categorie)
            if(webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length > 0) {
                if(!premier) sql += " or";
                else {
                    if(beginByAnd) sql += " and";
                    sql += " ((";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(categoryPrefixe + ".id_category", webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess], true) + " ";
                premier = false;
            }
            // media (support)
            if(webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess].Length > 0) {
                if(!premier) sql += " or";
                else {
                    if(beginByAnd) sql += " and";
                    sql += " ((";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(mediaPrefixe + ".id_media", webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess], true) + " ";
                premier = false;
            }

            if(!premier) sql += " )";
            // Droits en exclusion
            // vehicle (media)
            if(webSession.CustomerLogin[CustomerRightConstante.type.vehicleException].Length > 0) {
                if(!premier) sql += " and";
                else {
                    if(beginByAnd) sql += " and";
                    sql += " (";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(vehiclePrefixe + ".id_vehicle", webSession.CustomerLogin[CustomerRightConstante.type.vehicleException], false) + " ";
                premier = false;
            }
            // category (Categorie)
            if(webSession.CustomerLogin[CustomerRightConstante.type.categoryException].Length > 0) {
                if(!premier) sql += " and";
                else {
                    if(beginByAnd) sql += " and";
                    sql += " (";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(categoryPrefixe + ".id_category", webSession.CustomerLogin[CustomerRightConstante.type.categoryException], false) + " ";
                premier = false;
            }
            // media (support)
            if(webSession.CustomerLogin[CustomerRightConstante.type.mediaException].Length > 0) {
                if(!premier) sql += " and";
                else {
                    if(beginByAnd) sql += " and";
                    sql += " (";
                }
                sql += " " + SQLGenerator.GetInClauseMagicMethod(mediaPrefixe + ".id_media", webSession.CustomerLogin[CustomerRightConstante.type.mediaException], false) + " ";
                premier = false;
            }

            if(!premier) sql += " )";
            return (sql);
        }

        #endregion

        #region Produits
        /// <summary>
		/// Génère les droits clients Produit.
		/// Cette fonction est à utiliser si une même table contient tous les identifiants de la nomenclature produit.
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="tablePrefixe">Préfixe de la table qui contient les données</param>
		/// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
		/// <returns>Code SQL généré</returns>
		public static string getCustomerProductRight(WebSession webSession, string tablePrefixe, bool beginByAnd) {           

            TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(webSession.CurrentModule);
            string productRightsBranches = (module != null) ? module.ProductRightBranches : "";
            return (Utilities.SQLGenerator.GetClassificationCustomerProductRight(webSession, tablePrefixe, beginByAnd, productRightsBranches));

		}		

		#endregion

		#endregion

		#region GetView
		/// <summary>
		/// Get classification View 
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="oView">View</param>
		/// <param name="classificationRight">Sql rights</param>
		protected static View GetView(WebSession webSession, Dimension dimension) {
			
			switch (webSession.CurrentModule) {
				case TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR:
				case TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE:
					switch (dimension) {
						case Dimension.product:
							return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allRecapProduct);							
						case Dimension.media:
							return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allRecapMedia);
						default:
							throw (new CoreExceptions.SearchLevelDataAccessException("Unknown nomenclature dimension"));
					}
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_PROGRAMMES:
                    switch (dimension)
                    {
                        case Dimension.product:
                            return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allProduct);
                        case Dimension.media:
                            return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allProgram);
                        case Dimension.advertisingAgency:
                            return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allAdvAgency);
                        default:
                            throw (new CoreExceptions.SearchLevelDataAccessException("Unknown nomenclature dimension"));
                    }
                case TNS.AdExpress.Constantes.Web.Module.Name.HEALTH:
                    switch (dimension)
                    {
                        case Dimension.product:
                            return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allHealthProduct);
                        case Dimension.media:
                            return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.target);
                        case Dimension.advertisingAgency:
                            return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allAdvAgency);
                        default:
                            throw (new CoreExceptions.SearchLevelDataAccessException("Unknown nomenclature dimension"));
                    }
                default :
					switch (dimension) {
						case Dimension.product:
							return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allProduct);							
						case Dimension.media:
                            return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allMedia);
                        case Dimension.advertisingAgency:
                            return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allAdvAgency);
						default:
							throw (new CoreExceptions.SearchLevelDataAccessException("Unknown nomenclature dimension"));
					}					
			}

		}
		#endregion

		#region GetRights
		/// <summary>
		/// Get classification rights
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="oView">View</param>
		/// <param name="classificationRight">Sql rights</param>
		protected static string GetRights(WebSession webSession, Dimension dimension) {
			string classificationRight = "";
			switch (dimension) {
				case Dimension.product:
					classificationRight = getCustomerProductRight(webSession, "wp", true);
					break;
				case Dimension.media:
					classificationRight = getCustomerMediaRight(webSession, "wp", true);
					break;
                case Dimension.advertisingAgency:
                    return string.Empty;
				default:
					throw (new CoreExceptions.SearchLevelDataAccessException("Unknown nomenclature dimension"));
			}

			return classificationRight;
		}

		#region GetItemsInAccess
		/// <summary>
		/// Get classification access rights
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="universeLevelId">universe levekl Id</param>
		 /// <param name="label">Field</param>
		/// <param name="include">True if elements are included (in), false either (not in)</param>
		/// <returns>ID list of nomenclature items </returns>
		public static string GetAccessRights(WebSession webSession,long universeLevelId,string label, bool include) {
			string accessItems = "";
			switch (universeLevelId) {
				case TNS.Classification.Universe.TNSClassificationLevels.ADVERTISER:
					accessItems = Utilities.SQLGenerator.GetInClauseMagicMethod(label, webSession.CustomerLogin[CustomerRightConstante.type.advertiserAccess], include); break;
				case TNS.Classification.Universe.TNSClassificationLevels.SECTOR:
					accessItems = Utilities.SQLGenerator.GetInClauseMagicMethod(label, webSession.CustomerLogin[CustomerRightConstante.type.sectorAccess], include); break;
				case TNS.Classification.Universe.TNSClassificationLevels.SUB_SECTOR:
					accessItems = Utilities.SQLGenerator.GetInClauseMagicMethod(label, webSession.CustomerLogin[CustomerRightConstante.type.subSectorAccess], include); break;
				case TNS.Classification.Universe.TNSClassificationLevels.GROUP_:
					accessItems = Utilities.SQLGenerator.GetInClauseMagicMethod(label, webSession.CustomerLogin[CustomerRightConstante.type.groupAccess], include); break;
				case TNS.Classification.Universe.TNSClassificationLevels.SEGMENT:
					accessItems = Utilities.SQLGenerator.GetInClauseMagicMethod(label, webSession.CustomerLogin[CustomerRightConstante.type.segmentAccess], include); break;
                case TNS.Classification.Universe.TNSClassificationLevels.BRAND: 
                    	accessItems = Utilities.SQLGenerator.GetInClauseMagicMethod(label, webSession.CustomerLogin[CustomerRightConstante.type.brandAccess], include); break;
                case TNS.Classification.Universe.TNSClassificationLevels.PRODUCT:
                    accessItems = Utilities.SQLGenerator.GetInClauseMagicMethod(label, webSession.CustomerLogin[CustomerRightConstante.type.productAccess], include); break;
                default:
					throw (new CoreExceptions.SearchLevelDataAccessException("Identifiant de niveau de nomenclature inconnu"));
			}
			return !string.IsNullOrEmpty(accessItems) ? " and " + accessItems : accessItems;
		}

		#endregion

		#endregion

		#region GetSource
		/// <summary>
		/// Get data source
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		protected static IDataSource GetSource(WebSession webSession) {
			switch (webSession.CurrentModule) {
				case TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR:
				case TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE:
					return WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis, WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].NlsSort);
				default:
					return webSession.Source;				
			}
		}
		#endregion

		#region GetFromClause
		/// <summary>
		/// Get sql from clause
		/// </summary>
		/// <param name="webSession">User session</param>
		/// <param name="oView">View</param>
		/// <param name="table">Table</param>
		/// <param name="useView">determine if must use View</param>
		/// <returns>sql from clause string</returns>
		protected static string GetFromClause(WebSession webSession, View oView, string table, string dBSchema, bool useView) {			
			if (useView)
				return " from " + oView.Sql + webSession.DataLanguage.ToString() + " wp ";
			else return " from " + dBSchema + "."+ table + " wp ";
		}
		#endregion

		#region GetJointClause
		/// <summary>
		/// Get sql Joint clause
		/// </summary>
		/// <param name="webSession">User session</param>
		/// <param name="oView">View</param>
		/// <param name="table">Table</param>
		/// <param name="classificationRight">Classification rights</param>
		/// <param name="dimension">dimension media or product</param>
		/// <param name="useView">determine if must use View</param>
		/// <returns>sql from clause string</returns>
		protected static string GetJointClause(WebSession webSession, View oView, string table, Dimension dimension,string classificationRight,bool useView) {			
			string sql="";

			if (useView) {

				#region Application des droits
				if (classificationRight != null && classificationRight.Length > 0) {
					sql += classificationRight;
				}
				#endregion

				switch (webSession.CurrentModule) {
					case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA:
						if (dimension == Dimension.media)
							sql += " and wp.id_vehicle in (" + webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess) + ") ";
						break;
				}
			}
			else {
				sql += " and wp.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED; 
				sql += " and wp.id_language = " + webSession.DataLanguage;
			}

			return sql;
		}
		#endregion

		#region CanUseView
		/// <summary>
		/// Determine if View table must be used in query
		/// </summary>
		/// <param name="webSession">User Session</param>
		/// <param name="dimension">dimension media or product</param>
		/// <param name="classificationRight">classification rights</param>
		/// <returns></returns>
		protected static bool CanUseView(WebSession webSession, Dimension dimension, string classificationRight) {
			if ((classificationRight != null && classificationRight.Length > 0)
			|| (dimension == Dimension.media && webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA))
			 return true;
			 return false;
		}
		#endregion

		

	}
}
