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

namespace TNS.AdExpress.Web.Core.DataAccess {
    /// <summary>
    /// Class used to get the detail media list for one vehicle
    /// </summary>
    public class DetailMediaDataAccess {

        #region Detail Media DataAccess (New)

        #region DetailMediaListDataAccess
        ///// <summary>
        ///// Retourne la liste d'un detail Media avec les supports d'un utilisateur
        ///// </summary>
        ///// <param name="webSession">Session</param>
        ///// <param name="detailLevel">Detail Media</param>
        ///// <returns>Dataset avec 4 colonnes : idDetailMedia, detailMedia, id_media, media</returns>
        //public static DataSet DetailMediaListDataAccess(WebSession webSession, DetailLevelItemInformation detailLevel) {

        //    #region Variables
        //    OracleConnection connection = (OracleConnection)webSession.Source.GetSource();
        //    bool premier = true;
        //    DataSet dsListAdvertiser = null;
        //    StringBuilder sql = new StringBuilder(500);
        //    #endregion

        //    #region Requête
        //    sql.Append("Select distinct " + detailLevel.DataBaseIdField + "," + detailLevel.DataBaseField + ", id_media , media");
        //    sql.Append(" from " + WebApplicationParameters.DataBaseDescription.GetSqlViewLabelWithPrefix(ViewIds.allMedia) + " ");
        //    sql.Append(" where");

        //    #region Langue
        //    sql.Append(" id_language=" + webSession.DataLanguage.ToString());
        //    #endregion

        //    #region Activation
        //    //sql.Append(" and activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
        //    #endregion

        //    // Vehicle
        //    sql.Append(" and id_vehicle=" + ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID + "");

        //    //Liste des supports actifs pour Internet
        //    if (((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == (long)VehicleClassificationCst.internet)
        //        sql.Append(" and id_media in (" + TNS.AdExpress.Web.Core.ActiveMediaList.GetActiveMediaList(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID) + ")");

        //    //Condition univers des médias AdExpress en accès
        //    Module currentModuleDescription = ModulesList.GetModule(webSession.CurrentModule);
        //    if (currentModuleDescription.ModuleType == WebConstantes.Module.Type.tvSponsorship)
        //        sql.Append(Utilities.SQLGenerator.getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID, true));

        //    #region le bloc doit il commencer par AND
        //    premier = true;
        //    bool beginByAnd = true;

        //    #region Vehicle
        //    if (webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess].Length > 0) {
        //        if (beginByAnd) sql.Append(" and");
        //        sql.Append(" ((id_vehicle in (" + webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess] + ") ");
        //        premier = false;
        //    }
        //    #endregion

        //    #region Category
        //    if (webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length > 0) {
        //        if (!premier) sql.Append(" or");
        //        else {
        //            if (beginByAnd) sql.Append(" and");
        //            sql.Append("((");
        //        }
        //        sql.Append(" id_category in (" + webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess] + ") ");
        //        premier = false;
        //    }
        //    #endregion

        //    #region Media
        //    if (webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess].Length > 0) {
        //        if (!premier) sql.Append(" or");
        //        else {
        //            if (beginByAnd) sql.Append(" and");
        //            sql.Append(" ((");
        //        }
        //        sql.Append(" id_media in (" + webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess] + ") ");
        //        premier = false;
        //    }
        //    #endregion

        //    #endregion

        //    if (!premier) sql.Append(" )");

        //    #region Droits en exclusion

        //    #region Vehicle
        //    if (webSession.CustomerLogin[CustomerRightConstante.type.vehicleException].Length > 0) {
        //        if (!premier) sql.Append(" and");
        //        else {
        //            if (beginByAnd) sql.Append(" and");
        //            sql.Append(" (");
        //        }
        //        sql.Append(" ");
        //        sql.Append("id_vehicle not in (" + webSession.CustomerLogin[CustomerRightConstante.type.vehicleException] + ") ");
        //        premier = false;
        //    }
        //    #endregion

        //    #region Category
        //    if (webSession.CustomerLogin[CustomerRightConstante.type.categoryException].Length > 0) {
        //        if (!premier) sql.Append(" and");
        //        else {
        //            if (beginByAnd) sql.Append(" and");
        //            sql.Append(" (");
        //        }
        //        sql.Append(" id_category not in (" + webSession.CustomerLogin[CustomerRightConstante.type.categoryException] + ") ");
        //        premier = false;
        //    }
        //    #endregion

        //    #region Media
        //    if (webSession.CustomerLogin[CustomerRightConstante.type.mediaException].Length > 0) {
        //        if (!premier) sql.Append(" and");
        //        else {
        //            if (beginByAnd) sql.Append(" and");
        //            sql.Append(" (");
        //        }
        //        sql.Append(" id_media not in (" + webSession.CustomerLogin[CustomerRightConstante.type.mediaException] + ") ");
        //        premier = false;
        //    }
        //    #endregion

        //    #endregion

        //    if (!premier) sql.Append(" )");

        //    sql.Append(" order by " + detailLevel.DataBaseField + " , media ");
        //    #endregion

        //    #region Execution de la requête
        //    try {
        //        dsListAdvertiser = webSession.Source.Fill(sql.ToString());
        //        dsListAdvertiser.Tables[0].TableName = "dsListAdvertiser";
        //    }
        //    catch (System.Exception err) {
        //        throw (new Exceptions.DetailMediaDataAccessException("Impossible de charger la liste des centres d'intérêts avec les supports d'un utilisateur", err));
        //    }
        //    #endregion

        //    return dsListAdvertiser;
        //}
        #endregion

        #region keyWordDetailMediaListDataAccess
        ///// <summary>
        ///// Retourne la liste des detailMedia avec les supports d'un utilisateur ayant une partie du mot keyWord 
        ///// </summary>
        ///// <param name="webSession">Session</param>
        ///// <param name="keyWord">Mot clef</param>
        ///// <param name="listMedia">Liste des supports (media) déjà sélectionnés</param>
        ///// <param name="detailLevel">DetailMedia</param>
        ///// <returns>Données</returns>
        //public static DataSet keyWordDetailMediaListDataAccess(WebSession webSession, string keyWord, string listMedia, DetailLevelItemInformation detailLevel) {

        //    #region Variables
        //    OracleConnection connection = (OracleConnection)webSession.Source.GetSource();
        //    bool premier = true;
        //    DataSet dsListAdvertiser = null;
        //    StringBuilder sql = new StringBuilder(500);
        //    #endregion

        //    #region Requête
        //    sql.Append("Select distinct " + detailLevel.DataBaseIdField + ", " + detailLevel.DataBaseField + " , id_media , media");
        //    sql.Append(" from " + WebApplicationParameters.DataBaseDescription.GetSqlViewLabelWithPrefix(ViewIds.allMedia) + " ");
        //    sql.Append(" where");

        //    #region Langue
        //    sql.Append(" id_language=" + webSession.DataLanguage.ToString());
        //    #endregion

        //    #region Activation
        //    //sql.Append(" and activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
        //    #endregion

        //    #region Mot clé et médias déjà sélectionnés
        //    sql.Append("  and (" + detailLevel.DataBaseField + " like UPPER('%" + keyWord + "%') ");
        //    sql.Append(" or media like UPPER('%" + keyWord + "%') ");
        //    if (listMedia.Length > 0) {
        //        sql.Append(" or id_media in (" + listMedia + ") ");
        //    }
        //    sql.Append(" ) ");
        //    #endregion

        //    // Vehicle
        //    sql.Append(" and id_vehicle=" + ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID + "");

        //    //Liste des supports actifs pour Internet
        //    if (((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == (long)VehicleClassificationCst.internet)
        //        sql.Append(" and id_media in (" + TNS.AdExpress.Web.Core.ActiveMediaList.GetActiveMediaList(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID) + ")");

        //    //Condition univers des médias AdExpress en accès
        //    Module currentModuleDescription = ModulesList.GetModule(webSession.CurrentModule);
        //    if (currentModuleDescription.ModuleType == WebConstantes.Module.Type.tvSponsorship)
        //        sql.Append(Utilities.SQLGenerator.getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID, true));


        //    #region le bloc doit il commencer par AND
            
        //    premier = true;
        //    bool beginByAnd = true;

        //    #region Vehicle
        //    if (webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess].Length > 0) {
        //        if (beginByAnd) sql.Append(" and");
        //        sql.Append(" ((id_vehicle in (" + webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess] + ") ");
        //        premier = false;
        //    }
        //    #endregion

        //    #region Category
        //    if (webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length > 0) {
        //        if (!premier) sql.Append(" or");
        //        else {
        //            if (beginByAnd) sql.Append(" and");
        //            sql.Append("((");
        //        }
        //        sql.Append(" id_category in (" + webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess] + ") ");
        //        premier = false;
        //    }
        //    #endregion

        //    #region Media
        //    if (webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess].Length > 0) {
        //        if (!premier) sql.Append(" or");
        //        else {
        //            if (beginByAnd) sql.Append(" and");
        //            sql.Append(" ((");
        //        }
        //        sql.Append(" id_media in (" + webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess] + ") ");
        //        premier = false;
        //    }
        //    #endregion
            
        //    #endregion

        //    if (!premier) sql.Append(" )");

        //    #region Droits en exclusion

        //    #region vehicle
        //    if (webSession.CustomerLogin[CustomerRightConstante.type.vehicleException].Length > 0) {
        //        if (!premier) sql.Append(" and");
        //        else {
        //            if (beginByAnd) sql.Append(" and");
        //            sql.Append(" (");
        //        }
        //        sql.Append(" ");
        //        sql.Append("id_vehicle not in (" + webSession.CustomerLogin[CustomerRightConstante.type.vehicleException] + ") ");
        //        premier = false;
        //    }
        //    #endregion

        //    #region Category
        //    if (webSession.CustomerLogin[CustomerRightConstante.type.categoryException].Length > 0) {
        //        if (!premier) sql.Append(" and");
        //        else {
        //            if (beginByAnd) sql.Append(" and");
        //            sql.Append(" (");
        //        }
        //        sql.Append(" id_category not in (" + webSession.CustomerLogin[CustomerRightConstante.type.categoryException] + ") ");
        //        premier = false;
        //    }
        //    #endregion

        //    #region Media
        //    if (webSession.CustomerLogin[CustomerRightConstante.type.mediaException].Length > 0) {
        //        if (!premier) sql.Append(" and");
        //        else {
        //            if (beginByAnd) sql.Append(" and");
        //            sql.Append(" (");
        //        }
        //        sql.Append(" id_media not in (" + webSession.CustomerLogin[CustomerRightConstante.type.mediaException] + ") ");
        //        premier = false;
        //    }
        //    #endregion

        //    #endregion

        //    if (!premier) sql.Append(" )");

        //    sql.Append(" order by " + detailLevel.DataBaseField + " , media ");
        //    #endregion

        //    #region Execution de la requête
        //    try {
        //        dsListAdvertiser = webSession.Source.Fill(sql.ToString());
        //        dsListAdvertiser.Tables[0].TableName = "dsListMedia";
        //    }
        //    catch (System.Exception err) {
        //        throw (new Exceptions.DetailMediaDataAccessException("Impossible de charger la liste des centres d'intérêts avec les supports d'un utilisateur ayant une partie du mot keyWord", err));
        //    }
        //    #endregion

        //    return dsListAdvertiser;
        //}
        #endregion

        #endregion

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
            #endregion

            #region Requête
            sql.Append("Select distinct " + detailLevel.DataBaseTableNamePrefix + "." + detailLevel.DataBaseIdField + "," + detailLevel.DataBaseTableNamePrefix + "." + detailLevel.DataBaseField + ", " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_media , " + DBConstantes.Tables.MEDIA_PREFIXE + ".media");
            sql.Append(" from " + WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.vehicle) + "," + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label + "." + detailLevel.DataBaseTableName + " " + detailLevel.DataBaseTableNamePrefix + "," + WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.media) + " , " + WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.category) + ", " + WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.basicMedia) + " ");
            sql.Append(" where");

            #region Langue
            sql.Append(" " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle).Prefix + ".id_language=" + webSession.DataLanguage.ToString());
            sql.Append(" and " + detailLevel.DataBaseTableNamePrefix + ".id_language=" + webSession.DataLanguage.ToString());
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_language=" + webSession.DataLanguage.ToString());
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix + ".id_language=" + webSession.DataLanguage.ToString());
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_language=" + webSession.DataLanguage.ToString());
            #endregion

            #region Activation
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle).Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.Append(" and " + detailLevel.DataBaseTableNamePrefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            #endregion

            #region Jointure
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle).Prefix + ".id_vehicle=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_vehicle");
            sql.Append(" and " + detailLevel.DataBaseTableNamePrefix + "." + detailLevel.DataBaseIdField + "=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + "." + detailLevel.DataBaseIdField);
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_category=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix + ".id_category");
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix + ".id_basic_media=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_basic_media");
            #endregion

            // Vehicle
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle).Prefix + ".id_vehicle=" + ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID + "");

            //Liste des supports actifs pour Internet
            if (((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == (long)VehicleClassificationCst.internet)
                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media in (" + TNS.AdExpress.Web.Core.ActiveMediaList.GetActiveMediaList(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID) + ")");

            //Condition univers des médias AdExpress en accès
            Module currentModuleDescription = ModulesList.GetModule(webSession.CurrentModule);
            if (currentModuleDescription.ModuleType == WebConstantes.Module.Type.tvSponsorship)
                sql.Append(Utilities.SQLGenerator.getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID, true));

            #region le bloc doit il commencer par AND
            premier = true;
            bool beginByAnd = true;

            #region Vehicle
            if (webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess].Length > 0) {
                if (beginByAnd) sql.Append(" and");
                sql.Append(" ((" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle).Prefix + ".id_vehicle in (" + webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess] + ") ");
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
                sql.Append(" " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_category in (" + webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess] + ") ");
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
                sql.Append(" " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media in (" + webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess] + ") ");
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
                sql.Append(WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle).Prefix + ".id_vehicle not in (" + webSession.CustomerLogin[CustomerRightConstante.type.vehicleException] + ") ");
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
                sql.Append(" " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_category not in (" + webSession.CustomerLogin[CustomerRightConstante.type.categoryException] + ") ");
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
                sql.Append(" " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media not in (" + webSession.CustomerLogin[CustomerRightConstante.type.mediaException] + ") ");
                premier = false;
            }
            #endregion

            #endregion

            if (!premier) sql.Append(" )");

            sql.Append(" order by " + detailLevel.DataBaseTableNamePrefix + "." + detailLevel.DataBaseField + " , " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".media ");
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
            #endregion

            #region Requête
            sql.Append("Select distinct " + detailLevel.DataBaseTableNamePrefix + "." + detailLevel.DataBaseIdField + "," + detailLevel.DataBaseTableNamePrefix + "." + detailLevel.DataBaseField + " , " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_media , " + DBConstantes.Tables.MEDIA_PREFIXE + ".media");
            sql.Append(" from " + WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.vehicle) + "," + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label + "." + detailLevel.DataBaseTableName + " " + detailLevel.DataBaseTableNamePrefix + "," + WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.media) + " , " + WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.category) + ", " + WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.basicMedia) + " ");
            sql.Append(" where");

            #region Langue
            sql.Append(" " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle).Prefix + ".id_language=" + webSession.DataLanguage.ToString());
            sql.Append(" and " + detailLevel.DataBaseTableNamePrefix + ".id_language=" + webSession.DataLanguage.ToString());
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_language=" + webSession.DataLanguage.ToString());
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix + ".id_language=" + webSession.DataLanguage.ToString());
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_language=" + webSession.DataLanguage.ToString());
            #endregion

            #region Activation
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle).Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.Append(" and " + detailLevel.DataBaseTableNamePrefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            #endregion

            #region Jointure
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle).Prefix + ".id_vehicle=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_vehicle");
            sql.Append(" and " + detailLevel.DataBaseTableNamePrefix + "." + detailLevel.DataBaseIdField + "=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + "." + detailLevel.DataBaseIdField + "");
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_category=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix + ".id_category");
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix + ".id_basic_media=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_basic_media");
            #endregion

            #region Mot clé et médias déjà sélectionnés
            sql.Append("  and (" + detailLevel.DataBaseField + " like UPPER('%" + keyWord + "%') ");
            sql.Append(" or media like UPPER('%" + keyWord + "%') ");
            if (listMedia.Length > 0) {
                sql.Append(" or " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media in (" + listMedia + ") ");
            }
            sql.Append(" ) ");
            #endregion

            // Vehicle
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_vehicle=" + ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID + "");

            //Liste des supports actifs pour Internet
            if (((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == (long)VehicleClassificationCst.internet)
                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media in (" + TNS.AdExpress.Web.Core.ActiveMediaList.GetActiveMediaList(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID) + ")");

            //Condition univers des médias AdExpress en accès
            Module currentModuleDescription = ModulesList.GetModule(webSession.CurrentModule);
            if (currentModuleDescription.ModuleType == WebConstantes.Module.Type.tvSponsorship)
                sql.Append(Utilities.SQLGenerator.getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID, true));


            #region le bloc doit il commencer par AND
            premier = true;
            bool beginByAnd = true;

            #region Vehicle
            if (webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess].Length > 0) {
                if (beginByAnd) sql.Append(" and");
                sql.Append(" ((" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle).Prefix + ".id_vehicle in (" + webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess] + ") ");
                premier = false;
            }
            #endregion

            #region Category
            //			if(webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess).Length>0)
            //			{
            //				if(beginByAnd) sql.Append(" and");
            //				sql.Append(" (("+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category in ("+webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess)+") ");
            //				premier=false;
            //			}
            if (webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length > 0) {
                if (!premier) sql.Append(" or");
                else {
                    if (beginByAnd) sql.Append(" and");
                    sql.Append("((");
                }
                sql.Append(" " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_category in (" + webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess] + ") ");
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
                sql.Append(" " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media in (" + webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess] + ") ");
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
                sql.Append(WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle).Prefix + ".id_vehicle not in (" + webSession.CustomerLogin[CustomerRightConstante.type.vehicleException] + ") ");
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
                sql.Append(" " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_category not in (" + webSession.CustomerLogin[CustomerRightConstante.type.categoryException] + ") ");
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
                sql.Append(" " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media not in (" + webSession.CustomerLogin[CustomerRightConstante.type.mediaException] + ") ");
                premier = false;
            }
            #endregion

            #endregion

            if (!premier) sql.Append(" )");

            sql.Append(" order by " + detailLevel.DataBaseTableNamePrefix + "." + detailLevel.DataBaseField + " , " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".media ");
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
