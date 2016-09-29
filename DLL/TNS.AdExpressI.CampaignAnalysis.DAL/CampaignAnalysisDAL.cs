﻿using System;
using System.Data;
using System.Text;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using DbTables = TNS.AdExpress.Constantes.DB.Tables;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using WebNavigation = TNS.AdExpress.Domain.Web.Navigation;
using WebModule = TNS.AdExpress.Constantes.Web.Module;
using CustormerConstantes = TNS.AdExpress.Constantes.Customer;

namespace TNS.AdExpressI.CampaignAnalysis.DAL
{
    public class CampaignAnalysisDAL
    {
        #region GetData
        /// <summary>
        /// Charge les données pour créer étudier le parrainage télé
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="beginningDate">Date de début</param>
        /// <param name="endDate">Date de fin</param>
        /// <returns>Données du  parrainage télé</returns>
        public static DataSet GetData(WebSession webSession, string beginningDate, string endDate)
        {

            #region variables
            StringBuilder sql = new StringBuilder(5000);
            string detailLevelTableName = null;
            string orderFieldName = null, orderFieldNameWithoutTablePrefix = null;
            string groupByFieldName = null, groupByFieldNameWithoutTablePrefix = null;
            string tableName = null;
            string unitFieldNameSumWithAlias = null;
            //			string mediaFieldName = null;
            string mediaJoinCondition = null;
            string detailLevelFieldName = null;
            string detailLevelFieldNameWithoutTablePrefix = null;
            string dateFieldName = null;
            //			int positionUnivers=1;
            string mediaList = "";
            string dataTableNameForGad = "";
            string dataFieldsForGad = "";
            string dataFieldsForGadWithoutTablePrefix = "";
            string dataJointForGad = "";
            #endregion

            #region Construction de la requête
            try
            {
                //Obtient la table de parrainage
                tableName = DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DbTables.DATA_SPONSORSHIP;

                // Obtient les tables de la nomenclature
                detailLevelTableName = GetSqlTables(webSession);

                if (detailLevelTableName.Length > 0) detailLevelTableName += ",";

                // Obtient le champs des dates
                dateFieldName = SQLGenerator.GetDateFieldName(webSession);

                // Obtinet le(s) champ(s) de(s) unité(s) sélectioné(s)
                if (webSession.PreformatedTable != WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units)
                    unitFieldNameSumWithAlias = SQLGenerator.GetUnitFieldNameSumWithAlias(webSession, TNS.AdExpress.Constantes.DB.TableType.Type.dataVehicle4M);

                // Obtient les champs de la nomenclature
                detailLevelFieldName = GetSqlFields(webSession);
                detailLevelFieldNameWithoutTablePrefix = GetSqlFieldsWithoutTablePrefix(webSession);

                // Obtient l'ordre des champs
                orderFieldName = GetSqlOrderFields(webSession);
                orderFieldNameWithoutTablePrefix = GetSqlOrderFieldsWithoutTablePrefix(webSession);

                // obtient la clause group by
                groupByFieldName = GetSqlGroupByFields(webSession);
                groupByFieldNameWithoutTablePrefix = GetSqlGroupByFieldsWithoutTablePrefix(webSession);

                // Obtient les jointures pour la nomenclature
                mediaJoinCondition = GetSqlJoins(webSession);

                if (webSession.GenericMediaDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser))
                {
                    try
                    {
                        dataTableNameForGad = ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.GAD + " " + DBConstantes.Tables.GAD_PREFIXE;
                        dataFieldsForGad = ", " + SQLGenerator.GetFieldsAddressForGad();
                        dataFieldsForGadWithoutTablePrefix = " , " + SQLGenerator.GetFieldsAddressForGad("");
                        dataJointForGad = " and " + SQLGenerator.GetJointForGad(DbTables.DATA_SPONSORSHIP_PREFIXE);
                    }
                    catch (Exception ex) {; }
                }

            }
            catch (Exception e)
            {
                throw (new Exception("Impossible d'initialiser les paramètres de la requêtes" + e.Message));
            }



            // Sélection de la nomenclature 

            if (detailLevelFieldNameWithoutTablePrefix.Length > 0)
                sql.Append("  select /*+ opt_param('_optimizer_use_feedback', 'false') */ " + detailLevelFieldNameWithoutTablePrefix + dataFieldsForGadWithoutTablePrefix + " ,date_num, " + SQLGenerator.GetUnitFieldNameSumUnionWithAlias(webSession) + " from (  ");
            sql.Append(" select  " + detailLevelFieldName + dataFieldsForGad + " ,");


            // Sélection de la date
            if (webSession.PreformatedTable == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Period)
            {
                if (detailLevelFieldNameWithoutTablePrefix.Length > 0)
                {
                    if (webSession.DetailPeriod == WebConstantes.CustomerSessions.Period.DisplayLevel.weekly)
                        sql.Append(" to_char(to_date(" + dateFieldName + ",'YYYYMMDD'),'IYYYIW') as date_num,");//Conversion au format YYYYWW
                    else if (webSession.DetailPeriod == WebConstantes.CustomerSessions.Period.DisplayLevel.monthly)
                        sql.Append(" substr(to_char(" + dateFieldName + "),0,6) as date_num,");//Conversion au format YYYYMM

                }
                else
                    sql.Append(" " + dateFieldName + " as date_num,");
            }

            // Sélection de l'unité
            if (webSession.PreformatedTable == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units)
            {

                sql.AppendFormat(" sum({0}) as {1} ,"
                    , UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField
                    , UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString());
                sql.AppendFormat(" sum({0}) as {1} ,"
                    , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.duration].DatabaseField
                    , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.duration].Id.ToString());
                sql.AppendFormat(" sum({0}) as {1} "
                    , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].DatabaseField
                    , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString());

            }
            else
            {
                sql.AppendFormat(" {0} ", unitFieldNameSumWithAlias);
            }

            // Tables
            sql.Append(" from " + detailLevelTableName + tableName + " " + DbTables.DATA_SPONSORSHIP_PREFIXE + " " + dataTableNameForGad + " ");

            //Conditions media
            sql.Append(" where 0=0 " + mediaJoinCondition + " ");

            // Période
            sql.Append(" and " + dateFieldName + ">=" + beginningDate);
            sql.Append(" and " + dateFieldName + "<=" + endDate);

            //Gad
            sql.Append(" " + dataJointForGad);

            // Gestion des sélections et des droits

            #region Nomenclature Produit (droits)
            //Droits produits
            TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(webSession.CurrentModule);
            sql.Append(SQLGenerator.GetClassificationCustomerProductRight(webSession, DbTables.DATA_SPONSORSHIP_PREFIXE, true, module.ProductRightBranches));
            #endregion

            #region Nomenclature Produit (Niveau de détail)  
            // Niveau de produit
            sql.Append(SQLGenerator.getLevelProduct(webSession, DbTables.DATA_SPONSORSHIP_PREFIXE, true));
            #endregion

            #region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et sélection) 

            #region Sélection
            if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
                sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DbTables.DATA_SPONSORSHIP_PREFIXE, true));
            #endregion

            #endregion

            #region Nomenclature Media (droits et sélection)

            #region Droits
            sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(webSession, DbTables.DATA_SPONSORSHIP_PREFIXE, true));
            #endregion

            //Catégorie exclusive du parrainage TV
            sql.Append(GetMediaUniverse(webSession));

            #region Sélection
            //univers supports séléctionné	
            if (webSession.CurrentUniversMedia != null && webSession.CurrentUniversMedia.Nodes.Count > 0)
            {
                mediaList += webSession.GetSelection(webSession.CurrentUniversMedia, CustormerConstantes.Right.type.mediaAccess) + ",";
            }
            if (mediaList.Length > 0) sql.Append(" and  " + DbTables.DATA_SPONSORSHIP_PREFIXE + ".id_media in (" + mediaList.Substring(0, mediaList.Length - 1) + ") ");
            #endregion

            #endregion

            #region Nomenclature Emission

            //Sélection des émissions
            sql.Append(SQLGenerator.GetCustomerProgramSelection(webSession, DbTables.DATA_SPONSORSHIP_PREFIXE, DbTables.DATA_SPONSORSHIP_PREFIXE, true));

            //sélection des formes de parrainages
            sql.Append(SQLGenerator.GetCustomerSponsorshipFormSelection(webSession, DbTables.DATA_SPONSORSHIP_PREFIXE, true));

            #endregion

            // Ordre
            sql.Append(" Group by " + groupByFieldName + dataFieldsForGad + " ");

            // et la date
            if (webSession.PreformatedTable == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Period)
                sql.Append(", " + dateFieldName + " ");

            // Ordre
            if (UseOrder(webSession))
            {

                sql.Append(" Order by " + orderFieldName + " ");

                // et la date
                if (webSession.PreformatedTable == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Period)
                    sql.Append(", " + dateFieldName + " ");

            }


            if (detailLevelFieldNameWithoutTablePrefix.Length > 0) sql.Append("  ) ");

            if (groupByFieldNameWithoutTablePrefix.Length > 0)
            {

                sql.Append("  Group by " + groupByFieldNameWithoutTablePrefix + dataFieldsForGadWithoutTablePrefix + ",date_num ");
            }

            if (UseOrder(webSession))
            {
                if (orderFieldNameWithoutTablePrefix.Length > 0)
                    sql.Append("  Order by  " + orderFieldNameWithoutTablePrefix + ",date_num ");
            }
            #endregion

            #region Execution de la requête
            try
            {
                return webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new Exception("Impossible de charger les données du parrainage télé " + sql.ToString(), err));
            }

            #endregion
        }
        #endregion

        #region GetMediaData
        /// <summary>
        /// Charge les données supportspour créer étudier le parrainage télé
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="beginningDate">Date de début</param>
        /// <param name="endDate">Date de fin</param>
        /// <returns>Données du  parrainage télé</returns>
        public static DataSet GetMediaData(WebSession webSession, string beginningDate, string endDate)
        {

            #region variables
            StringBuilder sql = new StringBuilder(5000);
            string tableName = null;
            string dateFieldName = null;
            //			int positionUnivers=1;
            string mediaList = "";
            #endregion

            #region Construction de la requête
            try
            {
                //Obtient la table de parrainage
                tableName = DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DbTables.DATA_SPONSORSHIP;

                // Obtient le champs des dates
                dateFieldName = SQLGenerator.GetDateFieldName(webSession);

            }
            catch (Exception e)
            {
                throw (new Exception("Impossible d'initialiser les paramètres de la requêtes" + e.Message));
            }

            // Sélection de la nomenclature Support
            sql.Append(" select distinct " + DbTables.DATA_SPONSORSHIP_PREFIXE + ".id_media,media ");

            // Tables
            sql.Append(" from " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DbTables.MEDIA + " " + DbTables.MEDIA_PREFIXE + "," + tableName + " " + DbTables.DATA_SPONSORSHIP_PREFIXE + " ");

            //Conditions media
            sql.Append(" where ");

            sql.Append("  " + DbTables.MEDIA_PREFIXE + ".id_media = " + DbTables.DATA_SPONSORSHIP_PREFIXE + ".id_media");
            sql.Append(" and " + DbTables.MEDIA_PREFIXE + ".id_language =" + webSession.DataLanguage);
            sql.Append(" and " + DbTables.MEDIA_PREFIXE + ".activation < " + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);

            // Période
            sql.Append(" and " + dateFieldName + ">=" + beginningDate);
            sql.Append(" and " + dateFieldName + "<=" + endDate);

            // Gestion des sélections et des droits

            #region Nomenclature Produit (droits)
            //Droits produits
            TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(webSession.CurrentModule);
            sql.Append(SQLGenerator.GetClassificationCustomerProductRight(webSession, DbTables.DATA_SPONSORSHIP_PREFIXE, true, module.ProductRightBranches));

            #endregion

            #region Nomenclature Produit (Niveau de détail)  
            // Niveau de produit
            sql.Append(SQLGenerator.getLevelProduct(webSession, DbTables.DATA_SPONSORSHIP_PREFIXE, true));
            #endregion

            #region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et sélection) 

            #region Sélection

            sql.Append(SQLGenerator.GetAnalyseCustomerProductSelection(webSession, DbTables.DATA_SPONSORSHIP_PREFIXE, DbTables.DATA_SPONSORSHIP_PREFIXE, DbTables.DATA_SPONSORSHIP_PREFIXE, DbTables.DATA_SPONSORSHIP_PREFIXE, DbTables.DATA_SPONSORSHIP_PREFIXE, DbTables.DATA_SPONSORSHIP_PREFIXE, DbTables.DATA_SPONSORSHIP_PREFIXE, true));
            #endregion

            #endregion

            #region Nomenclature Media (droits et sélection)

            #region Droits
            sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(webSession, DbTables.DATA_SPONSORSHIP_PREFIXE, true));
            #endregion

            #region Sélection

            //univers supports séléctionné	
            if (webSession.CurrentUniversMedia != null && webSession.CurrentUniversMedia.Nodes.Count > 0)
            {
                mediaList += webSession.GetSelection(webSession.CurrentUniversMedia, CustormerConstantes.Right.type.mediaAccess) + ",";
            }
            if (mediaList.Length > 0) sql.Append(" and  " + DbTables.DATA_SPONSORSHIP_PREFIXE + ".id_media in (" + mediaList.Substring(0, mediaList.Length - 1) + ") ");

            #endregion

            //Catégorie exclusive du parrainage TV					
            sql.Append(GetMediaUniverse(webSession));
            #endregion

            #region Nomenclature Emission

            //Sélection des émissions
            sql.Append(SQLGenerator.GetCustomerProgramSelection(webSession, DbTables.DATA_SPONSORSHIP_PREFIXE, DbTables.DATA_SPONSORSHIP_PREFIXE, true));

            //sélection des formes de parrainages
            sql.Append(SQLGenerator.GetCustomerSponsorshipFormSelection(webSession, DbTables.DATA_SPONSORSHIP_PREFIXE, true));

            #endregion

            // Ordre		
            sql.Append(" Order by media ");
            #endregion

            #region Execution de la requête
            try
            {
                return webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new Exception("Impossible de charger la liste des supports  " + sql.ToString(), err));
            }

            #endregion
        }
        #endregion

        #region Private Methods

        #region GetSqlTables
        private static string GetSqlTables(WebSession webSession)
        {
            string detailLevelTableName = "";
            detailLevelTableName = webSession.GenericMediaDetailLevel.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA);
            if (webSession.PreformatedTable == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Media
                && !webSession.GenericMediaDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.media))
            {
                if (detailLevelTableName.Length > 0) detailLevelTableName += ",";
                detailLevelTableName += DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DbTables.MEDIA + " " + DbTables.MEDIA_PREFIXE;

            }

            return detailLevelTableName;
        }
        #endregion

        #region GetSqlFields
        private static string GetSqlFields(WebSession webSession)
        {
            string detailLevelFieldName = "";
            detailLevelFieldName = webSession.GenericMediaDetailLevel.GetSqlFields();
            if (webSession.PreformatedTable == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Media
                && !webSession.GenericMediaDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.media))
            {
                if (detailLevelFieldName.Length > 0) detailLevelFieldName += ",";
                detailLevelFieldName += DbTables.DATA_SPONSORSHIP_PREFIXE + ".id_media," + DbTables.MEDIA_PREFIXE + ".media";
            }

            return detailLevelFieldName;
        }
        #endregion

        #region GetSqlFieldsWithoutTablePrefix
        private static string GetSqlFieldsWithoutTablePrefix(WebSession webSession)
        {
            string detailLevelFieldName = "";
            if (webSession.PreformatedTable == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Period
                && webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.dayly)
            {
                detailLevelFieldName = webSession.GenericMediaDetailLevel.GetSqlFieldsWithoutTablePrefix();

            }
            return detailLevelFieldName;
        }
        #endregion

        #region GetSqlOrderFields
        private static string GetSqlOrderFields(WebSession webSession)
        {
            string orderFieldName = "";
            orderFieldName = webSession.GenericMediaDetailLevel.GetSqlOrderFields();
            if (orderFieldName.Length > 0)
            {
                if (webSession.PreformatedTable == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Media
                    && !webSession.GenericMediaDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.media))
                {
                    if (orderFieldName.Length > 0) orderFieldName += ",";
                    orderFieldName += DbTables.MEDIA_PREFIXE + ".media," + DbTables.DATA_SPONSORSHIP_PREFIXE + ".id_media";
                }
            }
            return orderFieldName;
        }
        #endregion

        #region GetSqlOrderFieldsWithoutTablePrefix
        private static string GetSqlOrderFieldsWithoutTablePrefix(WebSession webSession)
        {
            string orderFieldName = "";
            if (webSession.PreformatedTable == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Period
                && webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.dayly)
            {
                orderFieldName = webSession.GenericMediaDetailLevel.GetSqlOrderFieldsWithoutTablePrefix();

            }
            return orderFieldName;
        }
        #endregion

        #region GetSqlGroupByFields
        private static string GetSqlGroupByFields(WebSession webSession)
        {
            string groupByFieldName = "";
            groupByFieldName = webSession.GenericMediaDetailLevel.GetSqlGroupByFields();
            if (groupByFieldName.Length > 0)
            {
                if (webSession.PreformatedTable == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Media
                    && !webSession.GenericMediaDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.media))
                {
                    if (groupByFieldName.Length > 0) groupByFieldName += ",";
                    groupByFieldName += DbTables.DATA_SPONSORSHIP_PREFIXE + ".id_media," + DbTables.MEDIA_PREFIXE + ".media";
                }
            }

            return groupByFieldName;
        }
        #endregion

        #region GetSqlGroupByFieldsWithoutTablePrefix
        private static string GetSqlGroupByFieldsWithoutTablePrefix(WebSession webSession)
        {
            string groupByFieldName = "";
            if (webSession.PreformatedTable == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Period
                && webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.dayly)
            {
                groupByFieldName = webSession.GenericMediaDetailLevel.GetSqlGroupByFieldsWithoutTablePrefix();

            }
            return groupByFieldName;
        }
        #endregion

        #region GetSqlJoins
        private static string GetSqlJoins(WebSession webSession)
        {
            string mediaJoinCondition = "";
            mediaJoinCondition = webSession.GenericMediaDetailLevel.GetSqlJoins(webSession.DataLanguage, DbTables.DATA_SPONSORSHIP_PREFIXE);
            if (mediaJoinCondition.Length > 0)
            {
                if (webSession.PreformatedTable == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Media
                    && !webSession.GenericMediaDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.media))
                {
                    mediaJoinCondition += " and " + DbTables.MEDIA_PREFIXE + ".id_media=" + DbTables.DATA_SPONSORSHIP_PREFIXE + ".id_media";
                    mediaJoinCondition += " and " + DbTables.MEDIA_PREFIXE + ".id_language=" + webSession.DataLanguage;
                    mediaJoinCondition += " and " + DbTables.MEDIA_PREFIXE + ".activation<" + DBConstantes.ActivationValues.UNACTIVATED;
                }
            }

            return mediaJoinCondition;
        }
        #endregion

        #region GetMediaUniverse
        protected static string GetMediaUniverse(WebSession webSession)
        {
            string sql = "";
            if (IsSponsorShipTVModule(webSession))
            {
                WebNavigation.Module module = webSession.CustomerLogin.GetModule(webSession.CurrentModule);
                sql = module.GetResultPageInformation(webSession.CurrentTab).GetAllowedMediaUniverseSql(DbTables.DATA_SPONSORSHIP_PREFIXE, true);
            }
            return sql;
        }
        #endregion

        #region IsSponsorShipTVModule
        public static bool IsSponsorShipTVModule(WebSession webSession)
        {
            WebNavigation.Module currentModuleDescription = WebNavigation.ModulesList.GetModule(webSession.CurrentModule);
            if (currentModuleDescription.ModuleType == WebModule.Type.tvSponsorship)
                return true;
            return false;
        }
        #endregion

        #region UseOrder
        private static bool UseOrder(WebSession webSession)
        {
            switch (webSession.DetailPeriod)
            {
                case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
                case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
                    return (true);
                case WebConstantes.CustomerSessions.Period.DisplayLevel.dayly:
                    return (false);
                default:
                    throw (new Exception("Le détails période sélectionné est incorrect pour le choix de la table"));
            }
        }
        #endregion

        #endregion

    }
}
