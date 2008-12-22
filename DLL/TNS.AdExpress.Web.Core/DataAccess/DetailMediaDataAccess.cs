using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Oracle.DataAccess.Client;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using VehicleClassificationCst = TNS.AdExpress.Constantes.Classification.DB.Vehicles.names;
using CustomerRightConstante = TNS.AdExpress.Constantes.Customer.Right;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Constantes.Classification.DB;

using WebNavigation = TNS.AdExpress.Domain.Web.Navigation;

namespace TNS.AdExpress.Web.Core.DataAccess {
    /// <summary>
    /// Class used to get the detail media list for one vehicle
    /// </summary>
    public class DetailMediaDataAccess {

        #region Detail Media DataAccess

        #region DetailMediaListDataAccess
        /// <summary>
        /// Retourne la liste d'un detail Media avec les supports d'un utilisateur
        /// </summary>
        /// <param name="webSession">Session</param>
        /// <param name="detailLevel">Detail Media</param>
        /// <returns>Dataset avec 4 colonnes : idDetailMedia, detailMedia, id_media, media</returns>
        public static DataSet DetailMediaListDataAccess(WebSession webSession, DetailLevelItemInformation detailLevel) {

            #region Variables
            OracleConnection connection = (OracleConnection)webSession.Source.GetSource();
            bool premier = true;
            DataSet dsListAdvertiser = null;
            StringBuilder sql = new StringBuilder(500);
            string activeMediaList = string.Empty;
            #endregion

            #region Requête
			try {
				sql.Append("Select distinct " + detailLevel.DataBaseIdField + "," + detailLevel.DataBaseField + ", id_media , media, activation");
				sql.Append(" from " + WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allMedia).Sql + webSession.DataLanguage + " ");
				sql.Append(" where");

				// Vehicle
				sql.Append(" id_vehicle=" + ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID + "");

                //Liste des supports actifs pour Internet
                if (((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == VehiclesInformation.EnumToDatabaseId(Vehicles.names.internet)) {
                    activeMediaList = TNS.AdExpress.Web.Core.ActiveMediaList.GetActiveMediaList(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID);
                    if(activeMediaList.Length>0)
                        sql.Append(" and id_media in (" + activeMediaList + ")");
                }

				//Condition universe media in access
				WebNavigation.Module module = webSession.CustomerLogin.GetModule(webSession.CurrentModule);
				if (module != null && module.ModuleType == WebConstantes.Module.Type.tvSponsorship)
					sql.Append(module.GetAllowedMediaUniverseSqlWithOutPrefix(true));

				#region le bloc doit il commencer par AND
				premier = true;
				bool beginByAnd = true;

				#region Vehicle
				if (webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess].Length > 0) {
					if (beginByAnd) sql.Append(" and");
					sql.Append(" ((id_vehicle in (" + webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess] + ") ");
					premier = false;
				}
				#endregion

				#region Category
				if (webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length > 0) {
					if (!premier) sql.Append(" or");
					else {
						if (beginByAnd) sql.Append(" and");
						sql.Append("((");
					}
					sql.Append(" id_category in (" + webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess] + ") ");
					premier = false;
				}
				#endregion

				#region Media
				if (webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess].Length > 0) {
					if (!premier) sql.Append(" or");
					else {
						if (beginByAnd) sql.Append(" and");
						sql.Append(" ((");
					}
					sql.Append(" id_media in (" + webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess] + ") ");
					premier = false;
				}
				#endregion

				#endregion

				if (!premier) sql.Append(" )");

				#region Droits en exclusion

				#region Vehicle
				if (webSession.CustomerLogin[CustomerRightConstante.type.vehicleException].Length > 0) {
					if (!premier) sql.Append(" and");
					else {
						if (beginByAnd) sql.Append(" and");
						sql.Append(" (");
					}
					sql.Append(" ");
					sql.Append("id_vehicle not in (" + webSession.CustomerLogin[CustomerRightConstante.type.vehicleException] + ") ");
					premier = false;
				}
				#endregion

				#region Category
				if (webSession.CustomerLogin[CustomerRightConstante.type.categoryException].Length > 0) {
					if (!premier) sql.Append(" and");
					else {
						if (beginByAnd) sql.Append(" and");
						sql.Append(" (");
					}
					sql.Append(" id_category not in (" + webSession.CustomerLogin[CustomerRightConstante.type.categoryException] + ") ");
					premier = false;
				}
				#endregion

				#region Media
				if (webSession.CustomerLogin[CustomerRightConstante.type.mediaException].Length > 0) {
					if (!premier) sql.Append(" and");
					else {
						if (beginByAnd) sql.Append(" and");
						sql.Append(" (");
					}
					sql.Append(" id_media not in (" + webSession.CustomerLogin[CustomerRightConstante.type.mediaException] + ") ");
					premier = false;
				}
				#endregion

				#endregion

				if (!premier) sql.Append(" )");

				sql.Append(" order by " + detailLevel.DataBaseField + " , media ");
			}
			catch (System.Exception err) {
				throw (new Exceptions.DetailMediaDataAccessException("Impossible to build sql query for detail media ", err));
			}
            #endregion

            #region Execution de la requête
            try {
                dsListAdvertiser = webSession.Source.Fill(sql.ToString());
                dsListAdvertiser.Tables[0].TableName = "dsListAdvertiser";
            }
            catch (System.Exception err) {
                throw (new Exceptions.DetailMediaDataAccessException("Impossible de charger la liste des centres d'intérêts avec les supports d'un utilisateur", err));
            }
            #endregion

            return dsListAdvertiser;
        }

		/// <summary>
		/// Retourne la liste d'un detail Media avec les supports d'un utilisateur
		/// </summary>
		/// <param name="webSession">Session</param>
		/// <param name="detailLevel">Detail Media</param>
		/// <returns>Dataset avec 4 colonnes : idDetailMedia, detailMedia, id_media, media</returns>
		public static DataSet DetailMediaListDataAccess(WebSession webSession, GenericDetailLevel genericDetailLevel){
			return DetailMediaListDataAccess(webSession, genericDetailLevel,"");
		}

		/// <summary>
		/// Retourne la liste d'un detail Media avec les supports d'un utilisateur
		/// </summary>
		/// <param name="webSession">Session</param>
		/// <param name="detailLevel">Detail Media</param>
		/// <param name="listMedia">Liste de supports</param>
		/// <returns>Dataset avec 4 colonnes : idDetailMedia, detailMedia, id_media, media</returns>
		public static DataSet DetailMediaListDataAccess(WebSession webSession, GenericDetailLevel genericDetailLevel,string listMedia) {
			#region Variables
			OracleConnection connection = (OracleConnection)webSession.Source.GetSource();
			bool premier = true;
			DataSet dsListAdvertiser = null;
			StringBuilder sql = new StringBuilder(500);
			string activeMediaList = string.Empty;
			#endregion

			#region Requête
			try {
				string fields = genericDetailLevel.GetSqlFieldsWithoutTablePrefix();
				string orderFields = genericDetailLevel.GetSqlOrderFieldsWithoutTablePrefix();

				//sql.Append(" select * from   ( ");

				//sql.Append("Select distinct " + detailLevel.DataBaseIdField + "," + detailLevel.DataBaseField + ", id_media , media, activation");
				sql.Append("Select distinct " + fields + ", activation");
				sql.Append(" from " + WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allMedia).Sql + webSession.DataLanguage + " ");
				sql.Append(" where");

				// Vehicle
				sql.Append(" id_vehicle=" + ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID + "");

				//Liste des supports actifs pour Internet
				if (((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == VehiclesInformation.EnumToDatabaseId(Vehicles.names.internet)) {
					activeMediaList = TNS.AdExpress.Web.Core.ActiveMediaList.GetActiveMediaList(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID);
					if (activeMediaList.Length > 0)
						sql.Append(" and id_media in (" + activeMediaList + ")");
				}
				//Listes supports en cours
				if (listMedia!=null && listMedia.Length > 0) {
					sql.Append(" and id_media in (" + listMedia + ") ");
				}
				//Condition universe media in access
				WebNavigation.Module module = webSession.CustomerLogin.GetModule(webSession.CurrentModule);
				if (module != null && module.ModuleType == WebConstantes.Module.Type.tvSponsorship)
					sql.Append(module.GetAllowedMediaUniverseSqlWithOutPrefix(true));

				#region le bloc doit il commencer par AND
				premier = true;
				bool beginByAnd = true;

				#region Vehicle
				if (webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess].Length > 0) {
					if (beginByAnd) sql.Append(" and");
					sql.Append(" ((id_vehicle in (" + webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess] + ") ");
					premier = false;
				}
				#endregion

				#region Category
				if (webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length > 0) {
					if (!premier) sql.Append(" or");
					else {
						if (beginByAnd) sql.Append(" and");
						sql.Append("((");
					}
					sql.Append(" id_category in (" + webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess] + ") ");
					premier = false;
				}
				#endregion

				#region Media
				if (webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess].Length > 0) {
					if (!premier) sql.Append(" or");
					else {
						if (beginByAnd) sql.Append(" and");
						sql.Append(" ((");
					}
					sql.Append(" id_media in (" + webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess] + ") ");
					premier = false;
				}
				#endregion

				#endregion

				if (!premier) sql.Append(" )");

				#region Droits en exclusion

				#region Vehicle
				if (webSession.CustomerLogin[CustomerRightConstante.type.vehicleException].Length > 0) {
					if (!premier) sql.Append(" and");
					else {
						if (beginByAnd) sql.Append(" and");
						sql.Append(" (");
					}
					sql.Append(" ");
					sql.Append("id_vehicle not in (" + webSession.CustomerLogin[CustomerRightConstante.type.vehicleException] + ") ");
					premier = false;
				}
				#endregion

				#region Category
				if (webSession.CustomerLogin[CustomerRightConstante.type.categoryException].Length > 0) {
					if (!premier) sql.Append(" and");
					else {
						if (beginByAnd) sql.Append(" and");
						sql.Append(" (");
					}
					sql.Append(" id_category not in (" + webSession.CustomerLogin[CustomerRightConstante.type.categoryException] + ") ");
					premier = false;
				}
				#endregion

				#region Media
				if (webSession.CustomerLogin[CustomerRightConstante.type.mediaException].Length > 0) {
					if (!premier) sql.Append(" and");
					else {
						if (beginByAnd) sql.Append(" and");
						sql.Append(" (");
					}
					sql.Append(" id_media not in (" + webSession.CustomerLogin[CustomerRightConstante.type.mediaException] + ") ");
					premier = false;
				}
				#endregion

				#endregion

				if (!premier) sql.Append(" )");				

				sql.Append(" order by " + orderFields);

				//sql.Append(") where rownum<1390 ");
			}
			catch (System.Exception err) {
				throw (new Exceptions.DetailMediaDataAccessException("Impossible to build sql query for detail media ", err));
			}
			#endregion

			#region Execution de la requête
			try {
				dsListAdvertiser = webSession.Source.Fill(sql.ToString());
				dsListAdvertiser.Tables[0].TableName = "dsListAdvertiser";
			}
			catch (System.Exception err) {
				throw (new Exceptions.DetailMediaDataAccessException("Impossible de charger la liste des centres d'intérêts avec les supports d'un utilisateur", err));
			}
			#endregion

			return dsListAdvertiser;
		}
        #endregion

        #region keyWordDetailMediaListDataAccess
        /// <summary>
        /// Retourne la liste des detailMedia avec les supports d'un utilisateur ayant une partie du mot keyWord 
        /// </summary>
        /// <param name="webSession">Session</param>
        /// <param name="keyWord">Mot clef</param>
        /// <param name="listMedia">Liste des supports (media) déjà sélectionnés</param>
        /// <param name="detailLevel">DetailMedia</param>
        /// <returns>Données</returns>
        public static DataSet keyWordDetailMediaListDataAccess(WebSession webSession, string keyWord, string listMedia, DetailLevelItemInformation detailLevel) {

            #region Variables
            OracleConnection connection = (OracleConnection)webSession.Source.GetSource();
            bool premier = true;
            DataSet dsListAdvertiser = null;
            StringBuilder sql = new StringBuilder(500);
            string activeMediaList = string.Empty;
            #endregion

            #region Requête
            sql.Append("Select distinct " + detailLevel.DataBaseIdField + ", " + detailLevel.DataBaseField + " , id_media , media, activation");
            sql.Append(" from " + WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allMedia).Sql + webSession.DataLanguage + " ");
            sql.Append(" where");

            #region Mot clé et médias déjà sélectionnés
            sql.Append("  (" + detailLevel.DataBaseField + " like UPPER('%" + keyWord + "%') ");
            sql.Append(" or media like UPPER('%" + keyWord + "%') ");
            if (listMedia.Length > 0) {
                sql.Append(" or id_media in (" + listMedia + ") ");
            }
            sql.Append(" ) ");
            #endregion

            // Vehicle
            sql.Append(" and id_vehicle=" + ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID + "");

            //Liste des supports actifs pour Internet
            if (((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == VehiclesInformation.EnumToDatabaseId(Vehicles.names.internet)) {
                activeMediaList = TNS.AdExpress.Web.Core.ActiveMediaList.GetActiveMediaList(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID);
                if(activeMediaList.Length>0)
                    sql.Append(" and id_media in (" + activeMediaList + ")");
            }

			//Condition universe media in access
			WebNavigation.Module module = webSession.CustomerLogin.GetModule(webSession.CurrentModule);
			if (module != null && module.ModuleType == WebConstantes.Module.Type.tvSponsorship)
				sql.Append(module.GetAllowedMediaUniverseSqlWithOutPrefix(true));

            #region le bloc doit il commencer par AND

            premier = true;
            bool beginByAnd = true;

            #region Vehicle
            if (webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess].Length > 0) {
                if (beginByAnd) sql.Append(" and");
                sql.Append(" ((id_vehicle in (" + webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess] + ") ");
                premier = false;
            }
            #endregion

            #region Category
            if (webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length > 0) {
                if (!premier) sql.Append(" or");
                else {
                    if (beginByAnd) sql.Append(" and");
                    sql.Append("((");
                }
                sql.Append(" id_category in (" + webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess] + ") ");
                premier = false;
            }
            #endregion

            #region Media
            if (webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess].Length > 0) {
                if (!premier) sql.Append(" or");
                else {
                    if (beginByAnd) sql.Append(" and");
                    sql.Append(" ((");
                }
                sql.Append(" id_media in (" + webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess] + ") ");
                premier = false;
            }
            #endregion

            #endregion

            if (!premier) sql.Append(" )");

            #region Droits en exclusion

            #region vehicle
            if (webSession.CustomerLogin[CustomerRightConstante.type.vehicleException].Length > 0) {
                if (!premier) sql.Append(" and");
                else {
                    if (beginByAnd) sql.Append(" and");
                    sql.Append(" (");
                }
                sql.Append(" ");
                sql.Append("id_vehicle not in (" + webSession.CustomerLogin[CustomerRightConstante.type.vehicleException] + ") ");
                premier = false;
            }
            #endregion

            #region Category
            if (webSession.CustomerLogin[CustomerRightConstante.type.categoryException].Length > 0) {
                if (!premier) sql.Append(" and");
                else {
                    if (beginByAnd) sql.Append(" and");
                    sql.Append(" (");
                }
                sql.Append(" id_category not in (" + webSession.CustomerLogin[CustomerRightConstante.type.categoryException] + ") ");
                premier = false;
            }
            #endregion

            #region Media
            if (webSession.CustomerLogin[CustomerRightConstante.type.mediaException].Length > 0) {
                if (!premier) sql.Append(" and");
                else {
                    if (beginByAnd) sql.Append(" and");
                    sql.Append(" (");
                }
                sql.Append(" id_media not in (" + webSession.CustomerLogin[CustomerRightConstante.type.mediaException] + ") ");
                premier = false;
            }
            #endregion

            #endregion

            if (!premier) sql.Append(" )");

            sql.Append(" order by " + detailLevel.DataBaseField + " , media ");
            #endregion

            #region Execution de la requête
            try {
                dsListAdvertiser = webSession.Source.Fill(sql.ToString());
                dsListAdvertiser.Tables[0].TableName = "dsListMedia";
            }
            catch (System.Exception err) {
                throw (new Exceptions.DetailMediaDataAccessException("Impossible de charger la liste des centres d'intérêts avec les supports d'un utilisateur ayant une partie du mot keyWord", err));
            }
            #endregion

            return dsListAdvertiser;
        }

		/// <summary>
		/// Retourne la liste des detailMedia avec les supports d'un utilisateur ayant une partie du mot keyWord 
		/// </summary>
		/// <param name="webSession">Session</param>
		/// <param name="keyWord">Mot clef</param>
		/// <param name="listMedia">Liste des supports (media) déjà sélectionnés</param>
		/// <param name="genericDetailLevel">genericDetailLevel</param>
		/// <returns>Données</returns>
		public static DataSet keyWordDetailMediaListDataAccess(WebSession webSession, string keyWord, string listMedia,  GenericDetailLevel genericDetailLevel) {

			#region Variables
			OracleConnection connection = (OracleConnection)webSession.Source.GetSource();
			bool premier = true;
			DataSet dsListAdvertiser = null;
			StringBuilder sql = new StringBuilder(500);
			string activeMediaList = string.Empty;
			#endregion

			#region Requête

			string fields = genericDetailLevel.GetSqlFieldsWithoutTablePrefix();
			string orderFields = genericDetailLevel.GetSqlOrderFieldsWithoutTablePrefix();

			//sql.Append("Select distinct " + detailLevel.DataBaseIdField + ", " + detailLevel.DataBaseField + " , id_media , media, activation");
			sql.Append("Select distinct " + fields +", activation");
			sql.Append(" from " + WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allMedia).Sql + webSession.DataLanguage + " ");
			sql.Append(" where");

			#region Mot clé et médias déjà sélectionnés
			int j = 0;
			foreach (DetailLevelItemInformation currentLevel in genericDetailLevel.Levels) {
				if (j > 0) sql.Append(" or ");
				else sql.Append("  (");
				sql.Append("  " + currentLevel.DataBaseField + " like UPPER('%" + keyWord + "%') ");
				j++;
			}
			
			if (listMedia.Length > 0) {
				sql.Append(" or id_media in (" + listMedia + ") ");
			}
			sql.Append(" ) ");
			#endregion

			// Vehicle
			sql.Append(" and id_vehicle=" + ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID + "");

			//Liste des supports actifs pour Internet
			if (((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == VehiclesInformation.EnumToDatabaseId(Vehicles.names.internet)) {
				activeMediaList = TNS.AdExpress.Web.Core.ActiveMediaList.GetActiveMediaList(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID);
				if (activeMediaList.Length > 0)
					sql.Append(" and id_media in (" + activeMediaList + ")");
			}

			//Condition universe media in access
			WebNavigation.Module module = webSession.CustomerLogin.GetModule(webSession.CurrentModule);
			if (module != null && module.ModuleType == WebConstantes.Module.Type.tvSponsorship)
				sql.Append(module.GetAllowedMediaUniverseSqlWithOutPrefix(true));

			#region le bloc doit il commencer par AND

			premier = true;
			bool beginByAnd = true;

			#region Vehicle
			if (webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess].Length > 0) {
				if (beginByAnd) sql.Append(" and");
				sql.Append(" ((id_vehicle in (" + webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess] + ") ");
				premier = false;
			}
			#endregion

			#region Category
			if (webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length > 0) {
				if (!premier) sql.Append(" or");
				else {
					if (beginByAnd) sql.Append(" and");
					sql.Append("((");
				}
				sql.Append(" id_category in (" + webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess] + ") ");
				premier = false;
			}
			#endregion

			#region Media
			if (webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess].Length > 0) {
				if (!premier) sql.Append(" or");
				else {
					if (beginByAnd) sql.Append(" and");
					sql.Append(" ((");
				}
				sql.Append(" id_media in (" + webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess] + ") ");
				premier = false;
			}
			#endregion

			#endregion

			if (!premier) sql.Append(" )");

			#region Droits en exclusion

			#region vehicle
			if (webSession.CustomerLogin[CustomerRightConstante.type.vehicleException].Length > 0) {
				if (!premier) sql.Append(" and");
				else {
					if (beginByAnd) sql.Append(" and");
					sql.Append(" (");
				}
				sql.Append(" ");
				sql.Append("id_vehicle not in (" + webSession.CustomerLogin[CustomerRightConstante.type.vehicleException] + ") ");
				premier = false;
			}
			#endregion

			#region Category
			if (webSession.CustomerLogin[CustomerRightConstante.type.categoryException].Length > 0) {
				if (!premier) sql.Append(" and");
				else {
					if (beginByAnd) sql.Append(" and");
					sql.Append(" (");
				}
				sql.Append(" id_category not in (" + webSession.CustomerLogin[CustomerRightConstante.type.categoryException] + ") ");
				premier = false;
			}
			#endregion

			#region Media
			if (webSession.CustomerLogin[CustomerRightConstante.type.mediaException].Length > 0) {
				if (!premier) sql.Append(" and");
				else {
					if (beginByAnd) sql.Append(" and");
					sql.Append(" (");
				}
				sql.Append(" id_media not in (" + webSession.CustomerLogin[CustomerRightConstante.type.mediaException] + ") ");
				premier = false;
			}
			#endregion

			#endregion

			if (!premier) sql.Append(" )");

			sql.Append(" order by " + orderFields );
			#endregion

			#region Execution de la requête
			try {
				dsListAdvertiser = webSession.Source.Fill(sql.ToString());
				dsListAdvertiser.Tables[0].TableName = "dsListMedia";
			}
			catch (System.Exception err) {
				throw (new Exceptions.DetailMediaDataAccessException("Impossible de charger la liste des centres d'intérêts avec les supports d'un utilisateur ayant une partie du mot keyWord", err));
			}
			#endregion

			return dsListAdvertiser;
		}
        #endregion

        #endregion

    }
}
