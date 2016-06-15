using System;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using Oracle.DataAccess.Client;

using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using CustomerRightConstante = TNS.AdExpress.Constantes.Customer.Right;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using CstProject = TNS.AdExpress.Constantes.Project;
using TNS.FrameWork.DB.Common;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DbTables = TNS.AdExpress.Constantes.DB.Tables;
using ConstantesCustomer = TNS.AdExpress.Constantes.Customer;
using ConstantesFrameWork = TNS.AdExpress.Constantes.FrameWork;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Web.Helper.Exceptions;

namespace TNS.AdExpress.Web.Helper.DataAccess
{
    /// <summary>
    /// Extrait le détail des insertions publicitaires dans un support, une catégorie, un média correspondant
    /// à la sélection utilisateur (nomenclatures produits et média, période) présente dans une session
    /// </summary>
    public class MediaCreationDataAccess
    {

        #region Méthodes publiques

        #region Détail insertion sans colonne génériques

        #region GetAdNetTrackData
        /// <summary>
        /// Extrait le détail des insertions publicitaires dans un média de niveau 1, 2,3,4 correspondant
        /// à la sélection utilisateur (nomenclatures produits et média, période) présente dans une session:
        ///		Extraction de la table attaquée et des champs de sélection à partir du vehicle
        ///		Construction et exécution de la requête
        ///		
        /// </summary>
        /// <param name="webSession">Session du client</param>		
        /// <param name="dateBegin">Date de début de l'étude</param>
        /// <param name="dateEnd">Date de fin de l'étude</param>
        /// <param name="mediaList">list des détails médias </param>
        /// <param name="idVehicle">Identifiant du média (Vehicle) sélectionné par le client</param>
        /// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
        /// Lancée quand on ne sait pas quelle table attaquer, quels champs sélectionner ou quand une erreur Oracle s'est produite
        /// </exception>
        /// <returns>DataSet contenant le résultat de la requête</returns>
        /// <remarks>
        /// Utilise les méthodes:
        ///		- <code>private static string GetFields(DBClassificationConstantes.Vehicles.names idVehicle,WebSession webSesssion,bool isMediaDetail)</code> : obtient les champs de la requête.
        ///		- <code>private static string GetTables(DBClassificationConstantes.Vehicles.names idVehicle,WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail)</code> : obtient les tables de données pour la requête
        ///		- <code>private static void GetJoinConditions(WebSession webSession,StringBuilder sql,DBClassificationConstantes.Vehicles.names idVehicle,string dataTablePrefixe,bool beginByAnd)</code> : obtient les jointures de la requête
        ///		- <code> public static string TNS.AdExpress.Web.DataAccess.GetInsertionsOrder(DBClassificationConstantes.Vehicles.names idVehicle,WebSession webSesssion,bool isMediaDetail,string prefixeMediaPlanTable)</code> : obtient liodre de tri des résultats
        /// </remarks>
        public static DataSet GetAdNetTrackData(WebSession webSession, ListDictionary mediaList, int dateBegin, int dateEnd, Int64 idVehicle)
        {

            #region Variables
            StringBuilder sql = new StringBuilder(5000);
            //			string currentIdMedia="";
            #endregion

            sql = GetRequest(webSession, mediaList, dateBegin, dateEnd, idVehicle);

            #region Execution de la requête
            try
            {
                return webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new MediaCreationDataAccessException("Impossible de charger pour les insertions: " + sql, err));
            }
            #endregion
        }
        #endregion

        #region GetMDData
        /// <summary>
        /// Extrait les versions du media marketing direct détaillées par catégorie/support/semaine
        ///		Extraction de la table attaquée et des champs de sélection à partir du vehicle
        ///		Construction et exécution de la requête
        ///		
        /// </summary>
        /// <param name="webSession">Session du client</param>		
        /// <param name="dateBegin">Date de début de l'étude</param>
        /// <param name="dateEnd">Date de fin de l'étude</param>
        /// <param name="mediaList">list des détails médias </param>
        /// <param name="idVehicle">Identifiant du média (Vehicle) sélectionné par le client</param>
        /// <param name="export">True s'il s'agit d'un export</param>
        /// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
        /// Lancée quand on ne sait pas quelle table attaquer, quels champs sélectionner ou quand une erreur Oracle s'est produite
        /// </exception>
        /// <returns>DataSet contenant le résultat de la requête</returns>
        /// <remarks>
        /// Utilise les méthodes:
        ///		- <code>private static string GetFields(DBClassificationConstantes.Vehicles.names idVehicle,WebSession webSesssion,bool isMediaDetail)</code> : obtient les champs de la requête.
        ///		- <code>private static string GetTables(DBClassificationConstantes.Vehicles.names idVehicle,WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail)</code> : obtient les tables de données pour la requête
        ///		- <code>private static void GetJoinConditions(WebSession webSession,StringBuilder sql,DBClassificationConstantes.Vehicles.names idVehicle,string dataTablePrefixe,bool beginByAnd)</code> : obtient les jointures de la requête
        ///		- <code> public static string TNS.AdExpress.Web.DataAccess.GetInsertionsOrder(DBClassificationConstantes.Vehicles.names idVehicle,WebSession webSesssion,bool isMediaDetail,string prefixeMediaPlanTable)</code> : obtient liodre de tri des résultats
        /// </remarks>
        public static DataSet GetMDData(WebSession webSession, ListDictionary mediaList, int dateBegin, int dateEnd, Int64 idVehicle, bool export)
        {

            #region Variables
            StringBuilder sql = new StringBuilder(5000);
            #endregion

            sql = GetRequest(webSession, mediaList, dateBegin, dateEnd, idVehicle, export);

            #region Execution de la requête
            try
            {
                return webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new MediaCreationDataAccessException("Impossible de charger pour les insertions: " + sql, err));
            }
            #endregion
        }
        #endregion

        #region GetRequest
        /// <summary>
        /// Construit la requête sql pour la méthode GetAdNetTrackData
        /// </summary>
        /// <param name="webSession">Session du client</param>		
        /// <param name="dateBegin">Date de début de l'étude</param>
        /// <param name="dateEnd">Date de fin de l'étude</param>
        /// <param name="mediaList">list des détails médias </param>
        /// <param name="idVehicle">Identifiant du média (Vehicle) sélectionné par le client</param>
        /// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
        /// Lancée quand on ne sait pas quelle table attaquer, quels champs sélectionner ou quand une erreur Oracle s'est produite
        /// </exception>
        /// <returns>string contenant la requête</returns>
        public static StringBuilder GetRequest(WebSession webSession, ListDictionary mediaList, int dateBegin, int dateEnd, Int64 idVehicle)
        {
            return (GetRequest(webSession, mediaList, dateBegin, dateEnd, idVehicle, false));
        }


        /// <summary>
        /// Construit la requête sql pour la méthode GetAdNetTrackData
        /// </summary>
        /// <param name="webSession">Session du client</param>		
        /// <param name="dateBegin">Date de début de l'étude</param>
        /// <param name="dateEnd">Date de fin de l'étude</param>
        /// <param name="mediaList">list des détails médias </param>
        /// <param name="idVehicle">Identifiant du média (Vehicle) sélectionné par le client</param>
        /// <param name="export">True si la requête renvoie les données pour un export</param>
        /// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
        /// Lancée quand on ne sait pas quelle table attaquer, quels champs sélectionner ou quand une erreur Oracle s'est produite
        /// </exception>
        /// <returns>DataSet contenant le résultat de la requête</returns>
        /// <remarks>
        /// Utilise les méthodes:
        ///		- <code>private static string GetFields(DBClassificationConstantes.Vehicles.names idVehicle,WebSession webSesssion,bool isMediaDetail)</code> : obtient les champs de la requête.
        ///		- <code>private static string GetTables(DBClassificationConstantes.Vehicles.names idVehicle,WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail)</code> : obtient les tables de données pour la requête
        ///		- <code>private static void GetJoinConditions(WebSession webSession,StringBuilder sql,DBClassificationConstantes.Vehicles.names idVehicle,string dataTablePrefixe,bool beginByAnd)</code> : obtient les jointures de la requête
        ///		- <code> public static string TNS.AdExpress.Web.DataAccess.GetInsertionsOrder(DBClassificationConstantes.Vehicles.names idVehicle,WebSession webSesssion,bool isMediaDetail,string prefixeMediaPlanTable)</code> : obtient liodre de tri des résultats
        /// </remarks>
        public static StringBuilder GetRequest(WebSession webSession, ListDictionary mediaList, int dateBegin, int dateEnd, Int64 idVehicle, bool export)
        {

            #region Variables
            string fields = "";
            string list = "";
            bool premier = true;
            StringBuilder sql = new StringBuilder(5000);
            #endregion

            try
            {

                #region Construction de la requête
                fields = GetFields(VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())), mediaList, webSession, DbTables.WEB_PLAN_PREFIXE);

                // Sélection de la nomenclature Support
                sql.Append("select " + fields);

                // Tables // TODO ADNETTRACK ??
                sql.Append(" from ");
                sql.Append(GetTables(VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())), mediaList, webSession.PreformatedMediaDetail, webSession, dateBegin, dateEnd));

                // Conditions de jointure
                sql.Append(" Where ");
                GetJoinConditions(webSession, sql, VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())), mediaList, DbTables.WEB_PLAN_PREFIXE, false);

                // Période
                sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + ".date_media_num>=" + dateBegin);
                sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + ".date_media_num<=" + dateEnd);

                //Affiner univers version
                if (webSession.CurrentModule == WebConstantes.Module.Name.ALERTE_PLAN_MEDIA && webSession.SloganIdList != null && webSession.SloganIdList.Length > 0)
                {
                    sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + ".id_slogan in ( " + webSession.SloganIdList + " )  ");
                }

                // Gestion des sélections et des droits
                #region Nomenclature Produit (droits)
                premier = true;
                //Droits en accès
                sql.Append(Core.Utilities.SQLGenerator.getAnalyseCustomerProductRight(webSession, DbTables.WEB_PLAN_PREFIXE, true));

                #endregion

                // Niveau de produit
                sql.Append(Core.Utilities.SQLGenerator.getLevelProduct(webSession, DbTables.WEB_PLAN_PREFIXE, true));
                // Produit à exclure en radio
                //sql.Append(SQLGenerator.GetAdExpressProductUniverseCondition(TNS.AdExpress.Constantes.Web.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, DbTables.WEB_PLAN_PREFIXE, true, false));
                sql.Append(Core.Utilities.SQLGenerator.GetExcludeProducts(TNS.AdExpress.Constantes.Web.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, DbTables.WEB_PLAN_PREFIXE));

                #region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et sélection)

                // Sélection de Produits
                if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
                    sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DbTables.WEB_PLAN_PREFIXE, true));

                #endregion

                #region Nomenclature Media (droits et sélection)

                #region Droits
                // On ne tient pas compte des droits vehicle pour les plans media AdNetTrack
                if (VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())) == DBClassificationConstantes.Vehicles.names.adnettrack)
                    sql.Append(Core.Utilities.SQLGenerator.GetAdNetTrackMediaRight(webSession, DbTables.WEB_PLAN_PREFIXE, true));
                else
                    sql.Append(Core.Utilities.SQLGenerator.getAnalyseCustomerMediaRight(webSession, DbTables.WEB_PLAN_PREFIXE, true));

                //Droit detail spot à spot TNT
                if (VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())) == DBClassificationConstantes.Vehicles.names.tv
                    && !webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_DETAIL_DIGITAL_TV_ACCESS_FLAG))
                    sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + ".id_category != " + DBConstantes.Category.ID_DIGITAL_TV + "  ");

                #endregion

                #region Sélection média client

                //Obtient la sélection des média en fonction du niveau de détail média

                if (mediaList != null && mediaList.Count > 0)
                {

                    IEnumerator myEnumerator = mediaList.GetEnumerator();
                    foreach (DictionaryEntry de in mediaList)
                    {

                        if (de.Value != null && de.Key != null && long.Parse(de.Value.ToString()) > -1)
                        {

                            if (de.Key.ToString().Equals(DBConstantes.Fields.ID_SLOGAN) && de.Value.ToString().Equals("0") && webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG))
                                sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + "." + de.Key.ToString() + " is null  "); //accroche ==0
                            else if (de.Key.ToString().Equals(DBConstantes.Fields.ID_VEHICLE) && long.Parse(de.Value.ToString()) == VehiclesInformation.EnumToDatabaseId(DBClassificationConstantes.Vehicles.names.internet))
                            {
                                //Remplace identifiant internet par celui adnettrack
                                sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + "." + de.Key.ToString() + "=" + VehiclesInformation.EnumToDatabaseId(DBClassificationConstantes.Vehicles.names.adnettrack) + "  ");
                            }
                            else
                            {
                                if (!de.Key.ToString().Equals(DBConstantes.Fields.ID_SLOGAN)
                                    || (de.Key.ToString().Equals(DBConstantes.Fields.ID_SLOGAN) && !de.Value.ToString().Equals("0") && webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG)))
                                    sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + "." + de.Key.ToString() + "=" + de.Value.ToString() + "  ");
                            }

                        }
                    }
                }


                #endregion

                #endregion

                // Clause marketing Direct
                if (long.Parse(idVehicle.ToString()) == VehiclesInformation.EnumToDatabaseId(DBClassificationConstantes.Vehicles.names.directMarketing))
                {
                    sql.Insert(0, getMDSpecificField(VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())), mediaList, webSession, DbTables.WEB_PLAN_PREFIXE, false, export));
                    sql.Append(GetMDGroupByFields(VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())), mediaList, webSession, DbTables.WEB_PLAN_PREFIXE, true));
                    sql.Append(GetMDOrderByFields(VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())), mediaList, webSession, DbTables.WEB_PLAN_PREFIXE));
                    sql.Append(getMDSpecificGroupBy(VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())), mediaList, webSession, DbTables.WEB_PLAN_PREFIXE, false, export));
                }
                else
                    // Ordre
                    sql.Append(" order by advertiser,product ");
                #endregion

            }
            catch (System.Exception err)
            {
                throw (new MediaCreationDataAccessException("Impossible de construire la requête", err));
            }

            return sql;
        }
        #endregion

        #endregion

        #region Détail insertion avec colonnes génériques
        /// <summary>
		/// Extrait le détail des insertions publicitaires dans un média de niveau 1, 2,3,4 correspondant
		/// à la sélection utilisateur (nomenclatures produits et média, période) présente dans une session:
		///		Extraction de la table attaquée et des champs de sélection à partir du vehicle
		///		Construction et exécution de la requête
		///		
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="mediaList">Liste des médias</param>
		/// <param name="dateBegin">Date de début</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <param name="idVehicle">Identifiant du média</param>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
		/// Lancée quand on ne sait pas quelle table attaquer, quels champs sélectionner ou quand une erreur Oracle s'est produite
		/// </exception>
		/// <returns>DataSet contenant la liste des insertions par média</returns>		
		public static DataSet GetData(WebSession webSession, ListDictionary mediaList, int dateBegin, int dateEnd, Int64 idVehicle)
        {

            #region Variables
            string list = "";
            bool premier = true;
            StringBuilder sql = new StringBuilder(5000);
            ArrayList detailLevelList = new ArrayList();
            bool hasDetailLevelSelect = false;
            bool hasDetailLevelGroupBy = false;
            bool hasDetailLevelOrder = false;
            string selectedMediaList = "";
            #endregion

            try
            {

                //Select
                sql.Append(" select ");
                GetSqlFields(VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())), sql, webSession, ref hasDetailLevelSelect, detailLevelList);

                //Tables
                GetSqlTables(VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())), webSession, sql, detailLevelList, dateBegin, dateEnd);

                // Conditions de jointure
                sql.Append(" Where ");

                // Période
                sql.Append("  " + DbTables.WEB_PLAN_PREFIXE + ".date_media_num>=" + dateBegin);
                sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + ".date_media_num<=" + dateEnd);

                //Affiner univers version
                if (webSession.CurrentModule == WebConstantes.Module.Name.ALERTE_PLAN_MEDIA && webSession.SloganIdList != null && webSession.SloganIdList.Length > 0)
                {
                    sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + ".id_slogan in ( " + webSession.SloganIdList + " )  ");

                }
                if (webSession.GenericInsertionColumns.GetSqlJoins(webSession.DataLanguage, DbTables.WEB_PLAN_PREFIXE, detailLevelList).Length > 0)
                    sql.Append("  " + webSession.GenericInsertionColumns.GetSqlJoins(webSession.DataLanguage, DbTables.WEB_PLAN_PREFIXE, detailLevelList));
                if (webSession.DetailLevel != null)
                    sql.Append("  " + webSession.DetailLevel.GetSqlJoins(webSession.DataLanguage, DbTables.WEB_PLAN_PREFIXE));
                sql.Append("  " + webSession.GenericInsertionColumns.GetSqlContraintJoins());



                // Gestion des sélections et des droits
                #region Nomenclature Produit (droits)
                premier = true;
                //Droits en accès
                sql.Append(Core.Utilities.SQLGenerator.getAnalyseCustomerProductRight(webSession, DbTables.WEB_PLAN_PREFIXE, true));

                #endregion

                // Niveau de produit
                sql.Append(Core.Utilities.SQLGenerator.getLevelProduct(webSession, DbTables.WEB_PLAN_PREFIXE, true));

                if (Core.Utilities.Modules.IsSponsorShipTVModule(webSession))
                    //Media Universe
                    sql.Append(Core.Utilities.SQLGenerator.GetResultMediaUniverse(webSession, DbTables.WEB_PLAN_PREFIXE));
                else
                    sql.Append(Core.Utilities.SQLGenerator.GetExcludeProducts(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, DbTables.WEB_PLAN_PREFIXE));


                #region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et sélection) 

                // Sélection de Produits
                if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
                    sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DbTables.WEB_PLAN_PREFIXE, true));
                #endregion

                #region Nomenclature Media (droits et sélection)

                #region Droits
                sql.Append(Core.Utilities.SQLGenerator.getAnalyseCustomerMediaRight(webSession, DbTables.WEB_PLAN_PREFIXE, true));
                //Droit detail spot à spot TNT
                if (VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())) == DBClassificationConstantes.Vehicles.names.tv
                    && !webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_DETAIL_DIGITAL_TV_ACCESS_FLAG))
                    sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + ".id_category != " + DBConstantes.Category.ID_DIGITAL_TV + "  ");
                #endregion

                #region Sélection média client
                //obtient la sélection des média en fonction du niveau de détail média
                if (mediaList != null && mediaList.Count > 0)
                {
                    IEnumerator myEnumerator = mediaList.GetEnumerator();
                    foreach (DictionaryEntry de in mediaList)
                    {
                        if (de.Value != null && de.Key != null && long.Parse(de.Value.ToString()) > -1)
                        {

                            if (de.Key.ToString().Equals(DBConstantes.Fields.ID_SLOGAN) && de.Value.ToString().Equals("0") && webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG))
                                sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + "." + de.Key.ToString() + " is null  "); //accroche ==0
                            else
                            {
                                if (!de.Key.ToString().Equals(DBConstantes.Fields.ID_SLOGAN)
                                    || (de.Key.ToString().Equals(DBConstantes.Fields.ID_SLOGAN) && !de.Value.ToString().Equals("0") && webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG)))
                                    sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + "." + de.Key.ToString() + "=" + de.Value.ToString() + "  ");
                            }

                        }
                    }
                }
                sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + ".id_category<>35  ");// Pas d'affichage de TV NAT thématiques
                sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + ".id_vehicle=" + idVehicle.ToString());
                if (webSession.SloganIdZoom > -1)
                {
                    sql.AppendFormat(" and wp.id_slogan={0}", webSession.SloganIdZoom);
                }
                //univers supports séléctionné	pour Parrainage
                if (webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES
                    || webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS)
                {

                    if (webSession.CurrentUniversMedia != null && webSession.CurrentUniversMedia.Nodes.Count > 0)
                    {
                        selectedMediaList += webSession.GetSelection(webSession.CurrentUniversMedia, CustomerRightConstante.type.mediaAccess) + ",";
                    }
                    if (selectedMediaList.Length > 0) sql.Append(" and  " + DbTables.WEB_PLAN_PREFIXE + ".id_media in (" + selectedMediaList.Substring(0, selectedMediaList.Length - 1) + ") ");
                }
                #endregion

                #endregion

                #region Nomenclature Emission
                if (webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES
                    || webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS)
                {
                    //Sélection des émissions
                    sql.Append(Core.Utilities.SQLGenerator.GetCustomerProgramSelection(webSession, DbTables.WEB_PLAN_PREFIXE, DbTables.WEB_PLAN_PREFIXE, true));

                    //sélection des formes de parrainages
                    sql.Append(Core.Utilities.SQLGenerator.GetCustomerSponsorshipFormSelection(webSession, DbTables.WEB_PLAN_PREFIXE, true));
                }
                #endregion

                //Group by
                GetSqlGroupByFields(webSession, sql, VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())), ref hasDetailLevelGroupBy, detailLevelList);

                //Order by 
                GetSqlOrderFields(webSession, sql, VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())), ref hasDetailLevelOrder, detailLevelList);

            }
            catch (System.Exception err)
            {
                throw (new MediaCreationDataAccessException("Impossible de construire la requête", err));
            }


            #region Execution de la requête
            try
            {
                return webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new MediaCreationDataAccessException("Impossible de charger pour les insertions: " + sql, err));
            }
            #endregion
        }
        #endregion

        #region identification des média (vehicles) à traiter

        /// <summary>
        /// Obtient le média (vehicle) correspondant à la catégorie et/ou support sélectionné.
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <param name="idCategory">Identifiant catégorie</param>
        /// <param name="idMedia">Identifiant support</param>
        /// <returns>Média (vehicle)</returns>
        internal static DataSet GetIdsVehicle(WebSession webSession, Int64 idCategory, Int64 idMedia)
        {

            StringBuilder sql = new StringBuilder(500);


            #region Construction de la requête

            sql.Append(" select distinct " + DbTables.VEHICLE_PREFIXE + ".id_vehicle,vehicle from ");
            sql.Append(" " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.VEHICLE + " " + DbTables.VEHICLE_PREFIXE);
            sql.Append(", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.CATEGORY + " " + DbTables.CATEGORY_PREFIXE);

            if (idMedia > -1)
            {
                sql.Append(" , " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".basic_media " + DbTables.BASIC_MEDIA_PREFIXE);
                sql.Append(" , " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".media " + DbTables.MEDIA_PREFIXE);
            }

            sql.Append(" Where ");

            //Langages
            sql.Append("  " + DbTables.VEHICLE_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString());
            sql.Append(" and  " + DbTables.CATEGORY_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString());
            if (idMedia > -1)
            {
                sql.Append(" and " + DbTables.MEDIA_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString());
                sql.Append(" and " + DbTables.BASIC_MEDIA_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString());
            }

            //Jointures
            sql.Append("  and " + DbTables.VEHICLE_PREFIXE + ".id_vehicle =" + DbTables.CATEGORY_PREFIXE + ".id_vehicle");
            if (idMedia > -1)
            {
                sql.Append("  and " + DbTables.BASIC_MEDIA_PREFIXE + ".id_basic_media =" + DbTables.MEDIA_PREFIXE + ".id_basic_media");
                sql.Append("  and " + DbTables.BASIC_MEDIA_PREFIXE + ".id_category =" + DbTables.CATEGORY_PREFIXE + ".id_category");
                sql.Append("  and " + DbTables.MEDIA_PREFIXE + ".id_media= " + idMedia);
            }
            if (idCategory > -1) sql.Append(" and " + DbTables.CATEGORY_PREFIXE + ".id_category =" + idCategory);


            //Activation
            sql.Append(" and " + DbTables.VEHICLE_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
            sql.Append(" and " + DbTables.CATEGORY_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
            if (idMedia > -1)
            {
                sql.Append(" and " + DbTables.MEDIA_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
                sql.Append(" and " + DbTables.BASIC_MEDIA_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
            }

            sql.Append(" order by vehicle");
            #endregion

            #region Execution de la requête
            try
            {
                return webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new MediaCreationDataAccessException("Impossible d'identifier le média : " + sql, err));
            }
            #endregion

        }


        /// <summary>
        /// Obtient le média (vehicle) correspondant aux accroches.
        /// </summary>
        /// <param name="webSession">session du client</param>
        ///<param name="idSlogan">identifiant accroche</param>	
        ///<param name="idVehicle">Identifiant du média</param>
        ///<param name="tableName">Table de données</param>
        ///<param name="dateBegin">date de début</param>
        ///<param name="dateEnd">date de fin</param>
        /// <returns>Identifiant Média (vehicle)</returns>
        internal static DataSet GetIdsVehicle(WebSession webSession, Int64 idSlogan, string tableName, string dateBegin, string dateEnd, string idVehicle)
        {
            StringBuilder sql = new StringBuilder(500);
            string list = "";
            bool premier = true;
            //			int positionUnivers=1;
            //			string mediaList="";

            #region Construction de la requête
            //select
            sql.Append(" select count(ROWNUM) from ");

            sql.Append(" " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + tableName + " " + DbTables.WEB_PLAN_PREFIXE);


            sql.Append(" Where ");


            // Période
            sql.Append("  " + DbTables.WEB_PLAN_PREFIXE + ".date_media_num>=" + dateBegin);
            sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + ".date_media_num<=" + dateEnd);

            sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + ".id_category<>35  ");// Pas d'affichage de TV NAT thématiques
            sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + ".id_vehicle=" + idVehicle + "  ");

            #region Sélection média client	
            if (webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG))
            {
                if (idSlogan == 0)
                {
                    sql.Append(" and  " + DbTables.WEB_PLAN_PREFIXE + ".id_slogan is null ");

                }
                else sql.Append(" and  " + DbTables.WEB_PLAN_PREFIXE + ".id_slogan=" + idSlogan);
            }

            #endregion

            #region Nomenclature Produit (droits)
            premier = true;
            //Droits en accès
            sql.Append(Core.Utilities.SQLGenerator.getAnalyseCustomerProductRight(webSession, DbTables.WEB_PLAN_PREFIXE, true));

            #endregion

            // Niveau de produit
            sql.Append(Core.Utilities.SQLGenerator.getLevelProduct(webSession, DbTables.WEB_PLAN_PREFIXE, true));


            //Catégorie exclusive du parrainage TV
            if (Core.Utilities.Modules.IsSponsorShipTVModule(webSession))
                //Media Universe
                sql.Append(Core.Utilities.SQLGenerator.GetResultMediaUniverse(webSession, DbTables.WEB_PLAN_PREFIXE));
            else
                sql.Append(Core.Utilities.SQLGenerator.GetExcludeProducts(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, DbTables.WEB_PLAN_PREFIXE));


            #region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et sélection) 

            // Sélection de Produits
            if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
                sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DbTables.WEB_PLAN_PREFIXE, true));


            #endregion

            #region Nomenclature Media (droits et sélection)

            #region Droits
            // On ne tient pas compte des droits vehicle pour les plans media AdNetTrack
            if (VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())) == DBClassificationConstantes.Vehicles.names.adnettrack)
                sql.Append(Core.Utilities.SQLGenerator.GetAdNetTrackMediaRight(webSession, DbTables.WEB_PLAN_PREFIXE, true));
            else
                sql.Append(Core.Utilities.SQLGenerator.getAnalyseCustomerMediaRight(webSession, DbTables.WEB_PLAN_PREFIXE, true));
            #endregion

            #endregion



            #endregion

            #region Execution de la requête
            try
            {
                return webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new MediaCreationDataAccessException("Impossible d'identifier le média : " + sql, err));
            }
            #endregion


        }


        /// <summary>
        /// Obtient le média (vehicle) correspondant au niveau de détail sélectionné
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <param name="mediaImpactedList">Liste des médias impactés</param>		
        ///<param name="idVehicle">identifiant du média</param>
        ///<param name="tableName">Table de données</param>
        ///<param name="dateBegin">date de début</param>
        ///<param name="dateEnd">date de fin</param>
        /// <returns>Identifiant Média (vehicle)</returns>
        internal static DataSet GetIdsVehicle(WebSession webSession, ListDictionary mediaImpactedList, string tableName, string dateBegin, string dateEnd, string idVehicle)
        {
            StringBuilder sql = new StringBuilder(500);
            string list = "";
            bool premier = true;

            #region Construction de la requête
            //select
            sql.Append(" select count(ROWNUM) from ");

            sql.Append(" " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + tableName + " " + DbTables.WEB_PLAN_PREFIXE);


            sql.Append(" Where ");

            // Période
            sql.Append("  " + DbTables.WEB_PLAN_PREFIXE + ".date_media_num>=" + dateBegin);
            sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + ".date_media_num<=" + dateEnd);

            #region Sélection média client
            if (mediaImpactedList != null && mediaImpactedList.Count > 0)
            {
                //obtient la sélection des média en fonction du niveau de détail média
                IEnumerator myEnumerator = mediaImpactedList.GetEnumerator();
                foreach (DictionaryEntry de in mediaImpactedList)
                {
                    if (de.Key != null && de.Value != null && long.Parse(de.Value.ToString()) > -1 && de.Key != null && de.Key.ToString().Length > 0)
                    {
                        if (de.Key.ToString().Equals(DBConstantes.Fields.ID_VEHICLE) && long.Parse(de.Value.ToString()) == VehiclesInformation.EnumToDatabaseId(DBClassificationConstantes.Vehicles.names.internet)
                            )
                        {
                            //Remplace identifiant internet par celui adnettrack
                            sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + "." + de.Key.ToString() + "=" + VehiclesInformation.EnumToDatabaseId(DBClassificationConstantes.Vehicles.names.adnettrack) + "  ");
                        }
                        else
                            sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + "." + de.Key.ToString() + "=" + de.Value.ToString() + "  ");

                    }
                }
            }
            sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + ".id_category<>35  ");// Pas d'affichage de TV NAT thématiques
            sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + ".id_vehicle=" + idVehicle.ToString());

            #endregion

            #region Nomenclature Produit (droits)
            premier = true;
            //Droits en accès
            sql.Append(Core.Utilities.SQLGenerator.getAnalyseCustomerProductRight(webSession, DbTables.WEB_PLAN_PREFIXE, true));

            #endregion

            // Niveau de produit
            sql.Append(Core.Utilities.SQLGenerator.getLevelProduct(webSession, DbTables.WEB_PLAN_PREFIXE, true));

            //Catégorie exclusive du parrainage TV
            if (Core.Utilities.Modules.IsSponsorShipTVModule(webSession))
                //Media Universe
                sql.Append(Core.Utilities.SQLGenerator.GetResultMediaUniverse(webSession, DbTables.WEB_PLAN_PREFIXE));
            else
                sql.Append(Core.Utilities.SQLGenerator.GetExcludeProducts(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, DbTables.WEB_PLAN_PREFIXE));


            #region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et sélection) 

            // Sélection de Produits
            if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
                sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DbTables.WEB_PLAN_PREFIXE, true));

            #endregion

            #region Nomenclature Media (droits et sélection)

            #region Droits
            // On ne tient pas compte des droits vehicle pour les plans media AdNetTrack
            if (VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())) == DBClassificationConstantes.Vehicles.names.adnettrack)
                sql.Append(Core.Utilities.SQLGenerator.GetAdNetTrackMediaRight(webSession, DbTables.WEB_PLAN_PREFIXE, true));
            else
                sql.Append(Core.Utilities.SQLGenerator.getAnalyseCustomerMediaRight(webSession, DbTables.WEB_PLAN_PREFIXE, true));
            #endregion


            #endregion


            #endregion

            #region Execution de la requête
            try
            {
                return webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new MediaCreationDataAccessException("Impossible d'identifier le média : " + sql, err));
            }
            #endregion


        }

        /// <summary>
        /// Obtient le média (vehicle) correspondant aux accroches.
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="idSlogan">Identifiant accroche</param>	
        /// <returns>Identifiant Média (vehicle)</returns>
        internal static DataSet GetIdsVehicle(WebSession webSession, Int64 idSlogan)
        {
            StringBuilder sql = new StringBuilder(500);

            #region Construction de la requête

            sql.Append(" select " + DbTables.SLOGAN_PREFIXE + ".id_vehicle,vehicle from ");
            sql.Append(" " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.VEHICLE + " " + DbTables.VEHICLE_PREFIXE);
            sql.Append("," + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DbTables.SLOGAN + " " + DbTables.SLOGAN_PREFIXE);
            sql.Append(" Where ");
            sql.Append("  " + DbTables.SLOGAN_PREFIXE + ".id_slogan= " + idSlogan);
            sql.Append("  and " + DbTables.VEHICLE_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString());
            sql.Append("  and " + DbTables.VEHICLE_PREFIXE + ".id_vehicle =" + DbTables.SLOGAN_PREFIXE + ".id_vehicle");
            sql.Append(" and " + DbTables.VEHICLE_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
            sql.Append("  and  " + DbTables.SLOGAN_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
            sql.Append(" order by vehicle ");
            #endregion

            #region Execution de la requête
            try
            {
                return webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new MediaCreationDataAccessException("Impossible d'identifier le média : " + sql, err));
            }
            #endregion
        }

        #endregion

        #endregion

        #region Méthodes privées

        #region Get Fields

        /// <summary>
        /// Obetient les champs de la requête
        /// </summary>
        /// <param name="webSession"></param>
        /// <returns></returns>
        private static string GetSqlFields(WebSession webSession)
        {
            string sql = "";
            IList tempDetailLevelSqlFields = null, tempInsertionColumnsSqlFields = null;

            if (webSession.DetailLevel != null && webSession.DetailLevel.GetSqlFields().Length > 0)
            {
                tempDetailLevelSqlFields = webSession.DetailLevel.GetSqlFields().Split(',');
                sql = "  " + webSession.DetailLevel.GetSqlFields();
            }
            if (webSession.GenericInsertionColumns != null && webSession.GenericInsertionColumns.GetSqlFields().Length > 0)
            {

                if (tempDetailLevelSqlFields != null && tempDetailLevelSqlFields.Count > 0)
                {
                    tempInsertionColumnsSqlFields = webSession.GenericInsertionColumns.GetSqlFields().Split(',');
                    for (int i = 0; i < tempInsertionColumnsSqlFields.Count; i++)
                    {

                        if (!tempDetailLevelSqlFields.Contains(tempInsertionColumnsSqlFields[i].ToString()))
                            sql += " ," + tempInsertionColumnsSqlFields[i].ToString();
                    }
                }
                else sql = webSession.GenericInsertionColumns.GetSqlFields();
            }

            return sql;
        }

        /// <summary>
        /// Donne les champs à traiter pour le détail des insertions.
        /// </summary>
        /// <param name="idVehicle">Identifiant du média</param>
        /// <param name="mediaList">list des détails médias </param>
        /// <param name="webSesssion">Session  du client</param>
        /// <param name="prefixeMediaPlanTable">prfixe table média (vehicle)</param>
        /// <returns>Chaine contenant les champs à traiter</returns>
        private static string GetFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable)
        {
            string sql = "";
            bool showProduct = webSesssion.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);


            switch (idVehicle)
            {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.internationalPress:


                    sql = "  " + prefixeMediaPlanTable + ".date_media_Num"
                        + ", " + prefixeMediaPlanTable + ".media_paging"
                        + ", group_"
                        + ", advertiser";
                    sql += (showProduct) ? ", product" : "";
                    sql += ", format"
                        + ", " + prefixeMediaPlanTable + "." + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].DatabaseField + " as " + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString()
                        + ", color"
                        + ", " + prefixeMediaPlanTable + "." + UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField + " as " + UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()
                        + ", location"
                        + ", " + prefixeMediaPlanTable + ".visual "
                        + ", " + prefixeMediaPlanTable + ".id_advertisement"
                        + ", " + DbTables.APPLICATION_MEDIA_PREFIXE + ".disponibility_visual"
                        + ", " + DbTables.APPLICATION_MEDIA_PREFIXE + ".activation";
                    //						+", "+prefixeMediaPlanTable+".id_slogan";
                    sql += " ," + GetMediaFields(webSesssion, webSesssion.PreformatedMediaDetail, prefixeMediaPlanTable);
                    sql += ", " + prefixeMediaPlanTable + ".date_cover_Num";
                    return sql;

                case DBClassificationConstantes.Vehicles.names.radio:


                    sql = " " + prefixeMediaPlanTable + ".date_media_num"
                    + ", " + prefixeMediaPlanTable + ".id_top_diffusion"
                    + ", " + prefixeMediaPlanTable + ".associated_file"
                    + ", advertiser";
                    sql += (showProduct) ? ", product" : "";
                    sql += ", group_"
                        + ", " + prefixeMediaPlanTable + "." + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.duration].DatabaseField + " as " + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.duration].Id.ToString()
                        + ", " + prefixeMediaPlanTable + ".rank"
                        + ", " + prefixeMediaPlanTable + ".duration_commercial_break"
                        + ", " + prefixeMediaPlanTable + ".number_spot_com_break"
                        + ", " + prefixeMediaPlanTable + ".rank_wap"
                        + ", " + prefixeMediaPlanTable + ".duration_com_break_wap"
                        + ", " + prefixeMediaPlanTable + ".number_spot_com_break_wap"
                        + ", " + prefixeMediaPlanTable + "." + UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField + " as " + UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()
                        + ", " + prefixeMediaPlanTable + ".id_cobranding_advertiser";
                    sql += " ," + GetMediaFields(webSesssion, webSesssion.PreformatedMediaDetail, prefixeMediaPlanTable);
                    return sql;

                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:


                    sql = " " + prefixeMediaPlanTable + ".date_media_num"
                    + ", " + prefixeMediaPlanTable + ".top_diffusion"
                    + ", " + prefixeMediaPlanTable + ".associated_file"
                    + ", advertiser";
                    sql += (showProduct) ? ", product" : "";
                    sql += ", group_"
                        + ", " + prefixeMediaPlanTable + "." + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.duration].DatabaseField + " as " + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.duration].Id.ToString()
                        + ", " + prefixeMediaPlanTable + ".id_rank"
                        + ", " + prefixeMediaPlanTable + ".duration_commercial_break"
                        + ", " + prefixeMediaPlanTable + ".number_message_commercial_brea"
                        + ", " + prefixeMediaPlanTable + "." + UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField + " as " + UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()
                        + ", " + prefixeMediaPlanTable + ".id_commercial_break";
                    sql += " ," + GetMediaFields(webSesssion, webSesssion.PreformatedMediaDetail, prefixeMediaPlanTable);
                    return sql;

                case DBClassificationConstantes.Vehicles.names.outdoor:
                case DBClassificationConstantes.Vehicles.names.instore:

                    sql = " " + prefixeMediaPlanTable + ".date_media_num"
                    + ", advertiser";
                    sql += (showProduct) ? ", product" : "";
                    sql += ", group_"
                        + ", " + prefixeMediaPlanTable + "." + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.numberBoard].DatabaseField + " as " + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.numberBoard].Id.ToString()
                        + ", " + prefixeMediaPlanTable + ".type_board"
                        + ", " + prefixeMediaPlanTable + ".type_sale"
                        + ", " + prefixeMediaPlanTable + ".poster_network"
                        + ", " + DbTables.AGGLOMERATION_PREFIXE + ".agglomeration"
                        + ", " + prefixeMediaPlanTable + "." + UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField + " as " + UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString();
                    sql += " ," + GetMediaFields(webSesssion, webSesssion.PreformatedMediaDetail, prefixeMediaPlanTable);
                    return sql;
                case DBClassificationConstantes.Vehicles.names.adnettrack:
                    sql = " distinct " + prefixeMediaPlanTable + ".hashcode,"
                      + " " + prefixeMediaPlanTable + ".ASSOCIATED_FILE,"
                    + " " + prefixeMediaPlanTable + ".dimension,"
                    + " " + prefixeMediaPlanTable + ".format,"
                    + " " + prefixeMediaPlanTable + ".url";
                    sql += (showProduct) ? ", product" : "";
                    sql += "," + prefixeMediaPlanTable + ".id_product,advertiser," + prefixeMediaPlanTable + ".id_advertiser";


                    return sql;

                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    sql = "  " + DbTables.CATEGORY_PREFIXE + ".id_category, category"
                       + ", " + DbTables.MEDIA_PREFIXE + ".id_media, media"
                       + ", " + prefixeMediaPlanTable + ".date_media_num"
                       + ", " + DbTables.ADVERTISER_PREFIXE + ".id_advertiser, advertiser";
                    sql += (showProduct) ? ", " + DbTables.PRODUCT_PREFIXE + ".id_product, product" : "";
                    sql += ", " + DbTables.GROUP_PREFIXE + ".id_group_, group_"
                    + ", " + prefixeMediaPlanTable + ".weight"
                    + ", " + prefixeMediaPlanTable + ".associated_file"
                    + ", sum(" + prefixeMediaPlanTable + "." + UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField + ") as " + UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()
                    + ", sum(" + prefixeMediaPlanTable + "." + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.volume].DatabaseField + ") as " + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.volume].Id.ToString()
                    + ", " + prefixeMediaPlanTable + ".id_slogan";

                    sql += GetMDFields(idVehicle, mediaList, webSesssion, prefixeMediaPlanTable, true);
                    return sql;

                default:
                    throw new Exceptions.MediaCreationDataAccessException("GetFields(DBClassificationConstantes.Vehicles.names idVehicle,WebSession webSesssion) :: Le cas de ce média n'est pas gérer. Pas de table correspondante.");
            }
        }

        /// <summary>
		/// Donne les champs à traiter pour les créations du Marketing Direct.
		/// </summary>
		/// <param name="idVehicle">Identifiant du média</param>
        /// <param name="mediaList">list des détails médias </param>
		/// <param name="webSesssion">Session  du client</param>
		/// <param name="prefixeMediaPlanTable">prfixe table média (vehicle)</param>
        /// <param name="withPrefix">Ajouter des préfixes ou non</param>
		/// <returns>Chaine contenant les champs à traiter</returns>
        private static string GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable, bool withPrefix)
        {

            string sql = "";

            if (mediaList["id_media"] != null && mediaList["id_media"].ToString() != "-1")
            {

                switch (mediaList["id_media"].ToString())
                {

                    case DBConstantes.Media.PUBLICITE_NON_ADRESSEE:
                        sql = ",  format, "
                            + (withPrefix ? DbTables.MAIL_FORMAT_PREFIXE + "." : "") + "mail_format, "
                            + "mail_type";
                        break;
                    case DBConstantes.Media.COURRIER_ADRESSE_GENERAL:
                        sql = ", " + (withPrefix ? prefixeMediaPlanTable + ".mail_format as " : "") + "wp_mail_format,"
                            + " " + (withPrefix ? prefixeMediaPlanTable + ".object_count," : "object_count")
                            + " " + (withPrefix ? DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content," : "")
                            + " " + (withPrefix ? DbTables.MAIL_CONTENT_PREFIXE + ".mail_content" : "");
                        break;
                    case DBConstantes.Media.COURRIER_ADRESSE_PRESSE:
                        sql = ", " + (withPrefix ? prefixeMediaPlanTable + ".object_count," : "object_count")
                            + " " + (withPrefix ? DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content," : "")
                            + " " + (withPrefix ? DbTables.MAIL_CONTENT_PREFIXE + ".mail_content" : "");
                        break;
                    case DBConstantes.Media.COURRIER_ADRESSE_GESTION:
                        sql = ", " + (withPrefix ? prefixeMediaPlanTable + "." : "") + "object_count,"
                            + " mailing_rapidity" + (withPrefix ? "," : "")
                            + " " + (withPrefix ? DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content," : "")
                            + " " + (withPrefix ? DbTables.MAIL_CONTENT_PREFIXE + ".mail_content" : "");
                        break;
                    default:
                        throw new Exceptions.MediaCreationDataAccessException("GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable) : Ce support ne correspond pas à un support du MD.");
                }

            }
            else if (mediaList["id_category"] != null && mediaList["id_category"].ToString() != "-1")
            {

                switch (mediaList["id_category"].ToString())
                {

                    case DBConstantes.Category.PUBLICITE_NON_ADRESSEE:
                        sql = ",  format, "
                            + (withPrefix ? DbTables.MAIL_FORMAT_PREFIXE + "." : "") + "mail_format, "
                            + "mail_type";
                        break;
                    case DBConstantes.Category.COURRIER_ADRESSE:
                        sql = ", " + (withPrefix ? prefixeMediaPlanTable + ".mail_format as " : "") + "wp_mail_format,"
                            + " " + (withPrefix ? prefixeMediaPlanTable + "." : "") + "object_count,"
                            + " mailing_rapidity" + (withPrefix ? "," : "")
                            + " " + (withPrefix ? DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content," : "")
                            + " " + (withPrefix ? DbTables.MAIL_CONTENT_PREFIXE + ".mail_content" : "");
                        break;
                    default:
                        throw new Exceptions.MediaCreationDataAccessException("GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable) : Cette catégorie ne correspond pas à une catégorie du MD.");
                }

            }
            else
            {

                sql = ",  format, "
                    + (withPrefix ? DbTables.MAIL_FORMAT_PREFIXE + "." : "") + "mail_format, "
                    + "mail_type,"
                    + (withPrefix ? prefixeMediaPlanTable + ".mail_format as " : "") + "wp_mail_format,"
                    + " " + (withPrefix ? prefixeMediaPlanTable + "." : "") + "object_count,"
                    + " mailing_rapidity" + (withPrefix ? "," : "")
                    + " " + (withPrefix ? DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content," : "")
                    + " " + (withPrefix ? DbTables.MAIL_CONTENT_PREFIXE + ".mail_content" : "");

            }

            return sql;

        }

        /// <summary>
        /// Donne les champs à traiter pour le détail des insertions.
        /// </summary>
        /// <param name="idVehicle">Identifiant du média</param>
        /// <param name="webSesssion">Session  du client</param>
        /// <param name="hasDetailLevelSelect">Indique si des niveaux de détaillés ont été sélectionnés</param>
        /// <returns>Chaine contenant les champs à traiter</returns>
        private static void GetSqlFields(DBClassificationConstantes.Vehicles.names idVehicle, StringBuilder sql, WebSession webSession, ref bool hasDetailLevelSelect, ArrayList detailLevelList)
        {


            if (webSession.DetailLevel != null && webSession.DetailLevel.Levels != null && webSession.DetailLevel.Levels.Count > 0)
            {
                //detailLevelList = new ArrayList();
                foreach (DetailLevelItemInformation detailLevelItemInformation in webSession.DetailLevel.Levels)
                {
                    detailLevelList.Add(detailLevelItemInformation.Id.GetHashCode());
                }
                if (webSession.DetailLevel.GetSqlFields().Length > 0)
                {
                    sql.Append(" " + webSession.DetailLevel.GetSqlFields());
                    hasDetailLevelSelect = true;
                }

            }

            //Ajoute des champs spécifiques à la Presse 
            if ((idVehicle == DBClassificationConstantes.Vehicles.names.press
                || idVehicle == DBClassificationConstantes.Vehicles.names.internationalPress
                || idVehicle == DBClassificationConstantes.Vehicles.names.newspaper
                || idVehicle == DBClassificationConstantes.Vehicles.names.magazine
                )
                && AddPressSpecificField(webSession, DbTables.WEB_PLAN_PREFIXE).Length > 0)
            {
                sql.Append("," + AddPressSpecificField(webSession, DbTables.WEB_PLAN_PREFIXE));
            }


            if (webSession.GenericInsertionColumns.GetSqlFields(detailLevelList).Length > 0)
            {
                if (hasDetailLevelSelect) sql.Append(" , ");
                sql.Append(" " + webSession.GenericInsertionColumns.GetSqlFields(detailLevelList));
            }

            if (webSession.GenericInsertionColumns.GetSqlConstraintFields().Length > 0)
                sql.Append(" , " + webSession.GenericInsertionColumns.GetSqlConstraintFields());//Champs pour la gestion des contraintes métiers

            AddSloganField(webSession, sql, idVehicle);

            if (webSession.GenericInsertionColumns.GetSqlFields(detailLevelList).Length > 0 && (idVehicle == DBClassificationConstantes.Vehicles.names.tv) && !webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.category) && !webSession.DetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.category))
            {
                sql.Append(" , " + DbTables.WEB_PLAN_PREFIXE + ".id_category");
            }
        }

        /// <summary>
        /// Obtient les champs correspondants au détail media demandé par le client.
        /// Les champs demandées corespondent aux libellés des niveaux de média sélectionnés
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="preformatedMediaDetail">Niveau du détail media demandé</param>
        /// <param name="prefixeMediaPlanTable">prefixe table plan média</param>
        /// <returns>Chaîne contenant les champs</returns>
        public static string GetMediaFields(WebSession webSession, WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail, string prefixeMediaPlanTable)
        {


            if (webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG))
                return (" vehicle," + prefixeMediaPlanTable + ".id_category,category," + prefixeMediaPlanTable + ".id_media,media ,interest_center,media_seller," + prefixeMediaPlanTable + ".id_slogan");
            else return (" vehicle," + prefixeMediaPlanTable + ".id_category,category," + prefixeMediaPlanTable + ".id_media,media,interest_center,media_seller ");

        }

        #endregion

        #region GetTables
        /// <summary>
        ///Obtient les tables correspondants au détail media demandée par le client. 
        /// </summary>
        /// <param name="idVehicle">Identifiant du média</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="sql">Chaine sql</param>
        /// <param name="detailLevelList">Liste des niveaux de détail</param>
        private static void GetSqlTables(DBClassificationConstantes.Vehicles.names idVehicle, WebSession webSession, StringBuilder sql, ArrayList detailLevelList, int dateBegin, int dateEnd)
        {

            string tableName = "";
            Module currentModuleDescription = ModulesList.GetModule(webSession.CurrentModule);
            if (Core.Utilities.Modules.IsSponsorShipTVModule(webSession))
                tableName = DBConstantes.Tables.DATA_SPONSORSHIP;
            else
            {
                if (Core.Utilities.Dates.Is4M(dateBegin))
                    tableName = Core.Utilities.SQLGenerator.GetVehicleTableNameForDetailResult(idVehicle, WebConstantes.Module.Type.alert, webSession.IsSelectRetailerDisplay);
                else
                    tableName = Core.Utilities.SQLGenerator.GetVehicleTableNameForDetailResult(idVehicle, WebConstantes.Module.Type.analysis, webSession.IsSelectRetailerDisplay);
            }

            sql.Append(" from ");
            sql.Append(" " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + tableName + " " + DbTables.WEB_PLAN_PREFIXE);
            if (webSession.DetailLevel != null && webSession.DetailLevel.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA).Length > 0)
                sql.Append(" , " + webSession.DetailLevel.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA));
            if (webSession.GenericInsertionColumns.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA, detailLevelList).Length > 0)
            {
                sql.Append(" ," + webSession.GenericInsertionColumns.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA, detailLevelList));
            }
            if (webSession.GenericInsertionColumns.GetSqlConstraintTables(DBConstantes.Schema.ADEXPRESS_SCHEMA).Length > 0)
                sql.Append(" , " + webSession.GenericInsertionColumns.GetSqlConstraintTables(DBConstantes.Schema.ADEXPRESS_SCHEMA));//Tables pour la gestion des contraintes métiers

        }
        /// <summary>
        ///Obtient les tables correspondants au détail media demandée par le client. 
        /// </summary>
        /// <param name="idVehicle">Identifiant du média (vehicle)</param>
        /// <param name="mediaList">list des détails médias </param>
        /// <param name="preformatedMediaDetail">Niveau de détail média</param>
        /// <param name="webSession">Session du client</param>
        /// <returns>Chaîne contenant les tables</returns>
        private static string GetTables(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail, WebSession webSession, int dateBegin, int dateEnd)
        {
            string sql = "";
            string tableName = "";
            Module currentModuleDescription = ModulesList.GetModule(webSession.CurrentModule);
            if (currentModuleDescription.ModuleType == WebConstantes.Module.Type.tvSponsorship)
                tableName = Core.Utilities.SQLGenerator.GetVehicleTableNameForDetailResult(idVehicle, WebConstantes.Module.Type.tvSponsorship, webSession.IsSelectRetailerDisplay);
            else if (Core.Utilities.Dates.Is4M(dateBegin))
                tableName = Core.Utilities.SQLGenerator.GetVehicleTableNameForDetailResult(idVehicle, WebConstantes.Module.Type.alert, webSession.IsSelectRetailerDisplay);
            else
                tableName = Core.Utilities.SQLGenerator.GetVehicleTableNameForDetailResult(idVehicle, WebConstantes.Module.Type.analysis, webSession.IsSelectRetailerDisplay);
            bool showProduct = webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);


            sql += DBConstantes.Schema.ADEXPRESS_SCHEMA + ".advertiser   " + DbTables.ADVERTISER_PREFIXE;
            if (showProduct) sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".product   " + DbTables.PRODUCT_PREFIXE;

            sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + tableName + " " + DbTables.WEB_PLAN_PREFIXE;

            switch (idVehicle)
            {
                case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                    sql += GetMediaTables(preformatedMediaDetail);
                    sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.GROUP_ + "  " + DbTables.GROUP_PREFIXE;
                    break;
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                    sql += GetMediaTables(preformatedMediaDetail);
                    sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.GROUP_ + "  " + DbTables.GROUP_PREFIXE;
                    break;
                case DBClassificationConstantes.Vehicles.names.outdoor:
                case DBClassificationConstantes.Vehicles.names.instore:
                    sql += GetMediaTables(preformatedMediaDetail);
                    sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.GROUP_ + "  " + DbTables.GROUP_PREFIXE;
                    sql += "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".agglomeration  " + DbTables.AGGLOMERATION_PREFIXE;
                    break;
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.press:
                    sql += GetMediaTables(preformatedMediaDetail);
                    sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.GROUP_ + "  " + DbTables.GROUP_PREFIXE;
                    sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.COLOR + " " + DbTables.COLOR_PREFIXE;
                    sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.LOCATION + " " + DbTables.LOCATION_PREFIXE;
                    sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.DATA_LOCATION + " " + DbTables.DATA_LOCATION_PREFIXE;
                    sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.FORMAT + " " + DbTables.FORMAT_PREFIXE;
                    sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.APPLICATION_MEDIA + " " + DbTables.APPLICATION_MEDIA_PREFIXE;
                    break;
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.GROUP_ + "  " + DbTables.GROUP_PREFIXE;
                    sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.CATEGORY + "  " + DbTables.CATEGORY_PREFIXE;
                    sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.MEDIA + "  " + DbTables.MEDIA_PREFIXE;
                    sql += GetMDTables(idVehicle, mediaList, preformatedMediaDetail, webSession);
                    break;
            }

            return sql;
        }

        /// <summary>
        ///Obtient les tables correspondants au détail media demandée par le client. 
        /// </summary>
        /// <param name="idVehicle">Identifiant du média (vehicle)</param>
        /// <param name="mediaList">list des détails médias </param>
        /// <param name="preformatedMediaDetail">Niveau de détail média</param>
        /// <param name="webSession">Session du client</param>
        /// <returns>Chaîne contenant les tables</returns>
        private static string GetMDTables(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail, WebSession webSession)
        {

            string sql = "", cond = "";

            if ((mediaList["id_media"] != null && mediaList["id_media"].ToString() != "-1") || (mediaList["id_category"] != null && mediaList["id_category"].ToString() != "-1"))
            {

                if (mediaList["id_media"] != null && mediaList["id_media"].ToString() != "-1")
                    cond = mediaList["id_media"].ToString();
                else
                    cond = mediaList["id_category"].ToString();

                switch (cond)
                {

                    case DBConstantes.Category.PUBLICITE_NON_ADRESSEE:
                    case DBConstantes.Media.PUBLICITE_NON_ADRESSEE:
                        sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.FORMAT + "  " + DbTables.FORMAT_PREFIXE;
                        sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.MAIL_FORMAT + "  " + DbTables.MAIL_FORMAT_PREFIXE;
                        sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.MAIL_TYPE + "  " + DbTables.MAIL_TYPE_PREFIXE;
                        break;
                    case DBConstantes.Category.COURRIER_ADRESSE:
                    case DBConstantes.Media.COURRIER_ADRESSE_GESTION:
                        sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.MAILING_RAPIDITY + "  " + DbTables.MAILING_RAPIDITY_PREFIXE;
                        sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.MAIL_CONTENT + "  " + DbTables.MAIL_CONTENT_PREFIXE;
                        sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.DATA_MAIL_CONTENT + "  " + DbTables.DATA_MAIL_CONTENT_PREFIXE;
                        break;
                    case DBConstantes.Media.COURRIER_ADRESSE_PRESSE:
                    case DBConstantes.Media.COURRIER_ADRESSE_GENERAL:
                        sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.MAIL_CONTENT + "  " + DbTables.MAIL_CONTENT_PREFIXE;
                        sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.DATA_MAIL_CONTENT + "  " + DbTables.DATA_MAIL_CONTENT_PREFIXE;
                        break;
                    default:
                        throw new Exceptions.MediaCreationDataAccessException("GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable) : Le support ou la catégorie ne correspondent pas à un support ou une catégorie du MD.");
                }

            }
            else
            {

                sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.FORMAT + "  " + DbTables.FORMAT_PREFIXE;
                sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.MAIL_FORMAT + "  " + DbTables.MAIL_FORMAT_PREFIXE;
                sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.MAIL_TYPE + "  " + DbTables.MAIL_TYPE_PREFIXE;
                sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.MAILING_RAPIDITY + "  " + DbTables.MAILING_RAPIDITY_PREFIXE;
                sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.MAIL_CONTENT + "  " + DbTables.MAIL_CONTENT_PREFIXE;
                sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.DATA_MAIL_CONTENT + "  " + DbTables.DATA_MAIL_CONTENT_PREFIXE;
            }

            return sql;

        }

        /// <summary>
        /// Obtient les tables médias correspondants au détail media demandée par le client. 
        /// </summary>
        /// <param name="preformatedMediaDetail">Niveau du détail media demandé</param>
        /// <returns>Chaîne contenant les tables médias</returns>
        public static string GetMediaTables(WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail)
        {
            string sql = "";

            //Vehicles							
            sql += "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".vehicle " + DbTables.VEHICLE_PREFIXE;

            //Categories
            sql += "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".category " + DbTables.CATEGORY_PREFIXE;


            // Media							
            sql += "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".media " + DbTables.MEDIA_PREFIXE;

            // Interest center
            sql += "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".interest_center " + DbTables.INTEREST_CENTER_PREFIXE;


            // Régie
            sql += "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".media_seller " + DbTables.MEDIA_SELLER_PREFIXE;


            return (sql);
        }

        /// <summary>
        /// Obetient les tables de la requête
        /// </summary>
        /// <param name="webSession">Session client</param>
        /// <returns>Tables</returns>
        private static string GetSqlTables(WebSession webSession)
        {
            string sql = "";
            IList tempDetailLevelSqlTables = null, tempInsertionColumnsSqlTables = null;

            if (webSession.DetailLevel != null && webSession.DetailLevel.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA).Length > 0)
            {
                tempDetailLevelSqlTables = webSession.DetailLevel.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA).Split(',');
                sql = "  " + webSession.DetailLevel.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA);
            }
            if (webSession.GenericInsertionColumns != null && webSession.GenericInsertionColumns.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA).Length > 0)
            {

                if (tempDetailLevelSqlTables != null && tempDetailLevelSqlTables.Count > 0)
                {
                    tempInsertionColumnsSqlTables = webSession.GenericInsertionColumns.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA).Split(',');
                    for (int i = 0; i < tempInsertionColumnsSqlTables.Count; i++)
                    {

                        if (!tempDetailLevelSqlTables.Contains(tempInsertionColumnsSqlTables[i].ToString()))
                            sql += " ," + tempInsertionColumnsSqlTables[i].ToString();
                    }
                }
                else sql = webSession.GenericInsertionColumns.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA);
            }

            return sql;
        }
        #endregion

        #region Jointures

        /// <summary>
        /// Obtient les jointures à utiliser lors d'un détail media
        /// </summary>
        /// <param name="webSession">Session client</param>
        /// <param name="sql">requete sql</param>
        /// <param name="idVehicle">Identifiant média (vehicle)</param>
        /// <param name="mediaList">list des détails médias </param>
        /// <param name="dataTablePrefixe">prefixe table média</param>
        /// <param name="beginByAnd">Vrai si la condition doit commencée par And</param>
        /// <returns>requete sql</returns>
        private static void GetJoinConditions(WebSession webSession, StringBuilder sql, DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, string dataTablePrefixe, bool beginByAnd)
        {
            bool showProduct = webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
            if (beginByAnd) sql.Append(" and ");
            //produit
            if (showProduct)
            {
                sql.Append(" " + DbTables.PRODUCT_PREFIXE + ".id_product=" + dataTablePrefixe + ".id_product ");
                sql.Append(" and " + DbTables.PRODUCT_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString());
                sql.Append(" and " + DbTables.PRODUCT_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
            }
            // Annonceur
            if (showProduct) sql.Append(" and ");
            sql.Append("  " + DbTables.ADVERTISER_PREFIXE + ".id_advertiser=" + dataTablePrefixe + ".id_advertiser ");
            sql.Append(" and " + DbTables.ADVERTISER_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString());
            sql.Append(" and " + DbTables.ADVERTISER_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);

            switch (idVehicle)
            {

                case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                    // Groupe
                    sql.Append(" and " + DbTables.GROUP_PREFIXE + ".id_group_=" + dataTablePrefixe + ".id_group_ ");
                    sql.Append(" and " + DbTables.GROUP_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString());
                    sql.Append(" and " + DbTables.GROUP_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);

                    sql.Append(GetMediaJoinConditions(webSession, dataTablePrefixe, true));
                    break;
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                    sql.Append(GetMediaJoinConditions(webSession, dataTablePrefixe, true));
                    // Groupe
                    sql.Append(" and " + DbTables.GROUP_PREFIXE + ".id_group_=" + dataTablePrefixe + ".id_group_ ");
                    sql.Append(" and " + DbTables.GROUP_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString());
                    sql.Append(" and " + DbTables.GROUP_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
                    break;
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    sql.Append(GetMediaJoinConditions(webSession, dataTablePrefixe, true));
                    // Groupe
                    sql.Append(" and " + DbTables.GROUP_PREFIXE + ".id_group_=" + dataTablePrefixe + ".id_group_ ");
                    sql.Append(" and " + DbTables.GROUP_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString());
                    sql.Append(" and " + DbTables.GROUP_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);

                    sql.Append(" and (" + DbTables.APPLICATION_MEDIA_PREFIXE + ".id_media(+) = " + dataTablePrefixe + ".id_media ");
                    sql.Append(" and " + DbTables.APPLICATION_MEDIA_PREFIXE + ".date_debut(+) = " + dataTablePrefixe + ".date_media_num ");
                    sql.Append(" and " + DbTables.APPLICATION_MEDIA_PREFIXE + ".id_project(+) = " + CstProject.ADEXPRESS_ID + ") ");

                    sql.Append(" and " + DbTables.LOCATION_PREFIXE + ".id_location (+)=" + DbTables.DATA_LOCATION_PREFIXE + ".id_location ");
                    sql.Append(" and " + DbTables.LOCATION_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());

                    sql.Append(" and " + DbTables.DATA_LOCATION_PREFIXE + ".id_advertisement (+)=" + dataTablePrefixe + ".id_advertisement ");
                    sql.Append(" and " + DbTables.DATA_LOCATION_PREFIXE + ".id_media (+)=" + dataTablePrefixe + ".id_media ");
                    sql.Append(" and " + DbTables.DATA_LOCATION_PREFIXE + ".date_media_num (+)=" + dataTablePrefixe + ".date_media_num ");


                    sql.Append(" and " + DbTables.COLOR_PREFIXE + ".id_color (+)=" + dataTablePrefixe + ".id_color ");
                    sql.Append(" and " + DbTables.COLOR_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());

                    sql.Append(" and " + DbTables.FORMAT_PREFIXE + ".id_format (+)=" + dataTablePrefixe + ".id_format ");
                    sql.Append(" and " + DbTables.FORMAT_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
                    break;
                case DBClassificationConstantes.Vehicles.names.outdoor:
                case DBClassificationConstantes.Vehicles.names.instore:
                    sql.Append(GetMediaJoinConditions(webSession, dataTablePrefixe, true));
                    // Groupe
                    sql.Append(" and " + DbTables.GROUP_PREFIXE + ".id_group_=" + dataTablePrefixe + ".id_group_ ");
                    sql.Append(" and " + DbTables.GROUP_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString());
                    sql.Append(" and " + DbTables.GROUP_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);

                    sql.Append(" and " + DbTables.AGGLOMERATION_PREFIXE + ".id_agglomeration (+)=" + dataTablePrefixe + ".id_agglomeration ");
                    sql.Append(" and " + DbTables.AGGLOMERATION_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
                    sql.Append(" and " + DbTables.AGGLOMERATION_PREFIXE + ".activation (+)< " + DBConstantes.ActivationValues.UNACTIVATED);
                    break;
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    // Groupe
                    sql.Append(" and " + DbTables.GROUP_PREFIXE + ".id_group_=" + dataTablePrefixe + ".id_group_ ");
                    sql.Append(" and " + DbTables.GROUP_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString());
                    sql.Append(" and " + DbTables.GROUP_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
                    // category
                    sql.Append(" and " + DbTables.CATEGORY_PREFIXE + ".id_category=" + dataTablePrefixe + ".id_category ");
                    sql.Append(" and " + DbTables.CATEGORY_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString());
                    sql.Append(" and " + DbTables.CATEGORY_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
                    // Media
                    sql.Append(" and " + DbTables.MEDIA_PREFIXE + ".id_media=" + dataTablePrefixe + ".id_media ");
                    sql.Append(" and " + DbTables.MEDIA_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString());
                    sql.Append(" and " + DbTables.MEDIA_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
                    GetMDJoinConditions(webSession, mediaList, sql, idVehicle, dataTablePrefixe, beginByAnd);
                    break;

            }
        }

        /// <summary>
		/// Obtient les jointures à utiliser lors d'un détail media
		/// </summary>
		/// <param name="webSession">Session client</param>
        /// <param name="mediaList">list des détails médias </param>
		/// <param name="sql">requete sql</param>
		/// <param name="idVehicle">Identifiant média (vehicle)</param>
		/// <param name="dataTablePrefixe">prefixe table média</param>
		/// <param name="beginByAnd">Vrai si la condition doit commencée par And</param>
		/// <returns>requete sql</returns>
        private static void GetMDJoinConditions(WebSession webSession, ListDictionary mediaList, StringBuilder sql, DBClassificationConstantes.Vehicles.names idVehicle, string dataTablePrefixe, bool beginByAnd)
        {

            string cond = "";

            if ((mediaList["id_media"] != null && mediaList["id_media"].ToString() != "-1") || (mediaList["id_category"] != null && mediaList["id_category"].ToString() != "-1"))
            {

                if (mediaList["id_media"] != null && mediaList["id_media"].ToString() != "-1")
                    cond = mediaList["id_media"].ToString();
                else
                    cond = mediaList["id_category"].ToString();

                switch (cond)
                {

                    case DBConstantes.Category.PUBLICITE_NON_ADRESSEE:
                    case DBConstantes.Media.PUBLICITE_NON_ADRESSEE:
                        sql.Append(" and " + DbTables.FORMAT_PREFIXE + ".id_format (+)=" + dataTablePrefixe + ".id_format ");
                        sql.Append(" and " + DbTables.FORMAT_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
                        sql.Append(" and " + DbTables.MAIL_FORMAT_PREFIXE + ".id_mail_format (+)=" + dataTablePrefixe + ".id_mail_format ");
                        sql.Append(" and " + DbTables.MAIL_FORMAT_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
                        sql.Append(" and " + DbTables.MAIL_TYPE_PREFIXE + ".id_mail_type (+)=" + dataTablePrefixe + ".id_mail_type ");
                        sql.Append(" and " + DbTables.MAIL_TYPE_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
                        break;
                    case DBConstantes.Category.COURRIER_ADRESSE:
                    case DBConstantes.Media.COURRIER_ADRESSE_GESTION:
                        sql.Append(" and " + DbTables.MAILING_RAPIDITY_PREFIXE + ".id_mailing_rapidity (+)=" + dataTablePrefixe + ".id_mailing_rapidity ");
                        sql.Append(" and " + DbTables.MAILING_RAPIDITY_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
                        sql.Append(" and " + dataTablePrefixe + ".id_media =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_media (+) ");
                        sql.Append(" and " + dataTablePrefixe + ".date_media_num =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".date_media_num (+) ");
                        sql.Append(" and " + dataTablePrefixe + ".id_cobranding_advertiser =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_cobranding_advertiser (+) ");
                        sql.Append(" and " + dataTablePrefixe + ".id_data_marketing_direct_panel =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_data_marketing_direct_panel (+) ");
                        sql.Append(" and " + DbTables.MAIL_CONTENT_PREFIXE + ".id_mail_content (+) =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content ");
                        sql.Append(" and " + DbTables.MAIL_CONTENT_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
                        break;
                    case DBConstantes.Media.COURRIER_ADRESSE_PRESSE:
                    case DBConstantes.Media.COURRIER_ADRESSE_GENERAL:
                        sql.Append(" and " + dataTablePrefixe + ".id_media =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_media (+) ");
                        sql.Append(" and " + dataTablePrefixe + ".date_media_num =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".date_media_num (+) ");
                        sql.Append(" and " + dataTablePrefixe + ".id_cobranding_advertiser =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_cobranding_advertiser (+) ");
                        sql.Append(" and " + dataTablePrefixe + ".id_data_marketing_direct_panel =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_data_marketing_direct_panel (+) ");
                        sql.Append(" and " + DbTables.MAIL_CONTENT_PREFIXE + ".id_mail_content (+) =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content ");
                        sql.Append(" and " + DbTables.MAIL_CONTENT_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
                        break;
                    default:
                        throw new Exceptions.MediaCreationDataAccessException("GetMDJoinConditions(WebSession webSession, StringBuilder sql, DBClassificationConstantes.Vehicles.names idVehicle, string dataTablePrefixe, bool beginByAnd) : Le support ou la catégorie ne correspondent pas à un support ou une catégorie du MD.");
                }

            }
            else
            {

                sql.Append(" and " + DbTables.FORMAT_PREFIXE + ".id_format (+)=" + dataTablePrefixe + ".id_format ");
                sql.Append(" and " + DbTables.FORMAT_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
                sql.Append(" and " + DbTables.MAIL_FORMAT_PREFIXE + ".id_mail_format (+)=" + dataTablePrefixe + ".id_mail_format ");
                sql.Append(" and " + DbTables.MAIL_FORMAT_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
                sql.Append(" and " + DbTables.MAIL_TYPE_PREFIXE + ".id_mail_type (+)=" + dataTablePrefixe + ".id_mail_type ");
                sql.Append(" and " + DbTables.MAIL_TYPE_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
                sql.Append(" and " + DbTables.MAILING_RAPIDITY_PREFIXE + ".id_mailing_rapidity (+)=" + dataTablePrefixe + ".id_mailing_rapidity ");
                sql.Append(" and " + DbTables.MAILING_RAPIDITY_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
                sql.Append(" and " + dataTablePrefixe + ".id_media =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_media (+) ");
                sql.Append(" and " + dataTablePrefixe + ".date_media_num =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".date_media_num (+) ");
                sql.Append(" and " + dataTablePrefixe + ".id_cobranding_advertiser =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_cobranding_advertiser (+) ");
                sql.Append(" and " + dataTablePrefixe + ".id_data_marketing_direct_panel =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_data_marketing_direct_panel (+) ");
                sql.Append(" and " + DbTables.MAIL_CONTENT_PREFIXE + ".id_mail_content (+) =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content ");
                sql.Append(" and " + DbTables.MAIL_CONTENT_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
            }


        }

        /// <summary>
        /// Obtient les jointures média à utiliser lors d'un détail media
        /// </summary>
        /// <param name="webSession">Session client</param>
        /// <param name="dataTablePrefixe">Prefixe de la table de résultat</param>
        /// <param name="beginByAnd">Vrai si la condition doit commencée par And</param>
        /// <returns>Chaîne contenant les tables</returns>
        private static string GetMediaJoinConditions(WebSession webSession, string dataTablePrefixe, bool beginByAnd)
        {
            string tmp = "";

            //Vehicles			
            if (beginByAnd) tmp += " and ";
            tmp += "  " + DbTables.VEHICLE_PREFIXE + ".id_language=" + webSession.DataLanguage;
            tmp += " and " + DbTables.VEHICLE_PREFIXE + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
            tmp += " and " + DbTables.VEHICLE_PREFIXE + ".id_vehicle=" + dataTablePrefixe + ".id_vehicle ";


            //Categories
            tmp += " and " + DbTables.CATEGORY_PREFIXE + ".id_language=" + webSession.DataLanguage;
            tmp += " and " + DbTables.CATEGORY_PREFIXE + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
            tmp += " and " + DbTables.CATEGORY_PREFIXE + ".id_category=" + dataTablePrefixe + ".id_category ";

            // Media			
            tmp += " and " + DbTables.MEDIA_PREFIXE + ".id_language=" + webSession.DataLanguage;
            tmp += " and " + DbTables.MEDIA_PREFIXE + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
            tmp += " and " + DbTables.MEDIA_PREFIXE + ".id_media=" + dataTablePrefixe + ".id_media ";


            // Interest center

            tmp += " and " + DbTables.INTEREST_CENTER_PREFIXE + ".id_language=" + webSession.DataLanguage;
            tmp += " and " + DbTables.INTEREST_CENTER_PREFIXE + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
            tmp += " and " + DbTables.INTEREST_CENTER_PREFIXE + ".id_interest_center=" + dataTablePrefixe + ".id_interest_center ";


            // Régie
            tmp += " and " + DbTables.MEDIA_SELLER_PREFIXE + ".id_language=" + webSession.DataLanguage;
            tmp += " and " + DbTables.MEDIA_SELLER_PREFIXE + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
            tmp += " and " + DbTables.MEDIA_SELLER_PREFIXE + ".id_media_seller=" + dataTablePrefixe + ".id_media_seller ";



            //			if(tmp.Length==0)throw(new Exceptions.SQLGeneratorException("Le détail support demandé n'est pas valide"));
            //			if(!beginByAnd)tmp=tmp.Substring(4,tmp.Length-4);
            return (tmp);
        }
        #endregion

        #region Champ spécifique à rajouter 
        /// <summary>
        /// Obtient un champ de données spécifique pour la presse
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="wpPrefixe">prefixe table</param>
        /// <returns>Champ</returns>
        private static string AddPressSpecificField(WebSession webSession, string wpPrefixe)
        {

            string sql = "";
            ArrayList detailLevelList = new ArrayList();
            bool hasIdMedia = false;
            bool hasDateMediaNum = false;
            bool hasLocation = false;

            if (webSession.DetailLevel != null)
            {
                foreach (DetailLevelItemInformation detailLevelItemInformation in webSession.DetailLevel.Levels)
                {
                    if (detailLevelItemInformation.DataBaseIdField != null && detailLevelItemInformation.DataBaseIdField.Trim().Equals("id_media"))
                        hasIdMedia = true;
                    if (detailLevelItemInformation.DataBaseField != null && detailLevelItemInformation.DataBaseField.Trim().Equals("date_media_num"))
                        hasDateMediaNum = true;
                }
            }



            if (!hasIdMedia || !hasDateMediaNum || !hasLocation && webSession.GenericInsertionColumns != null)
            {
                foreach (GenericColumnItemInformation genericColumnItemInformation in webSession.GenericInsertionColumns.Columns)
                {
                    if (genericColumnItemInformation.DataBaseIdField != null && genericColumnItemInformation.DataBaseIdField.Trim().Equals("id_media"))
                        hasIdMedia = true;
                    if (genericColumnItemInformation.DataBaseField != null && genericColumnItemInformation.DataBaseField.Trim().Equals("date_media_num"))
                        hasDateMediaNum = true;
                    if ((GenericColumnItemInformation.Columns)genericColumnItemInformation.Id == GenericColumnItemInformation.Columns.location)
                        hasLocation = true;
                }

            }
            if ((webSession.DetailLevel != null && webSession.DetailLevel.Levels != null && webSession.DetailLevel.Levels.Count > 0)
                || (webSession.GenericInsertionColumns != null && webSession.GenericInsertionColumns.Columns != null && webSession.GenericInsertionColumns.Columns.Count > 0)

                )
            {
                if (!hasIdMedia)
                    sql = wpPrefixe + ".id_media";
                if (!hasDateMediaNum)
                {
                    if (sql.Length > 0) sql += ",";
                    sql += wpPrefixe + ".date_media_num";
                }
                if (!hasLocation)
                {
                }

            }
            if (sql.Length > 0) sql += ",";
            sql += wpPrefixe + ".date_cover_num";

            return sql;


        }

        /// <summary>
        /// Ajoute le champ slogan si nécessaire
        /// </summary>
        /// <param name="webSession">Session client</param>
        /// <param name="sql">Chaine de caractères</param>
        /// <param name="idVehicle">Identifiant du média</param>
        private static void AddSloganField(WebSession webSession, StringBuilder sql, DBClassificationConstantes.Vehicles.names idVehicle)
        {
            //Ajoute l'identifiant (uniquement pour la radio) de la version qui sera nécessaire pour construire le chemin du fichier audio de la radio
            if (webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG) && idVehicle == DBClassificationConstantes.Vehicles.names.radio &&
                !webSession.DetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.slogan) && !webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.slogan)
                ) sql.Append(" ," + DbTables.WEB_PLAN_PREFIXE + ".id_slogan ");
        }

        /// <summary>
        /// Donne les champs à traiter pour les créations du Marketing Direct.
        /// </summary>
        /// <param name="idVehicle">Identifiant du média</param>
        /// <param name="mediaList">list des détails médias </param>
        /// <param name="webSesssion">Session  du client</param>
        /// <param name="prefixeMediaPlanTable">prfixe table média (vehicle)</param>
        /// <returns>Chaine contenant les champs à traiter</returns>
        private static string getMDSpecificField(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable, bool withPrefix, bool export)
        {

            string sql = string.Empty;
            string cond = string.Empty;
            bool showProduct = webSesssion.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);

            sql = "  select id_category, category"
                + ", id_media, media"
                + (!export ? ", date_media_num" : ", min(date_media_num) as date_media_num")
                + ", id_advertiser, advertiser";
            if (showProduct) sql += ", id_product, product";
            sql += ", id_group_, group_"
                + ", weight"
                + ", associated_file"
                + (!export ? ", " + UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString() : ", sum(" + UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField + ")"
                + " as " + UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString())
                + (!export ? ", " + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.volume].DatabaseField : ", sum(" + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.volume].DatabaseField + ")"
                + " as " + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.volume].Id.ToString())
                + ", id_slogan";
            sql += GetMDFields(idVehicle, mediaList, webSesssion, prefixeMediaPlanTable, withPrefix);

            if ((mediaList["id_media"] != null && mediaList["id_media"].ToString() != "-1") || (mediaList["id_category"] != null && mediaList["id_category"].ToString() != "-1"))
            {

                if (mediaList["id_media"] != null && mediaList["id_media"].ToString() != "-1")
                    cond = mediaList["id_media"].ToString();
                else
                    cond = mediaList["id_category"].ToString();
            }

            if (cond != DBConstantes.Media.PUBLICITE_NON_ADRESSEE && cond != DBConstantes.Category.PUBLICITE_NON_ADRESSEE)
            {

                sql += ",max(decode(MAIL_CONTENT,'" + DBConstantes.MailContent.LETTRE_ACCOMP_PERSONALIS + "',MAIL_CONTENT)) as item1,"
                 + "max(decode(MAIL_CONTENT,'" + DBConstantes.MailContent.ENV_RETOUR_PRE_IMPRIMEE + "',MAIL_CONTENT)) as item2,"
                 + "max(decode(MAIL_CONTENT,'" + DBConstantes.MailContent.ENV_RETOUR_A_TIMBRER + "',MAIL_CONTENT)) as item3,"
                 + "max(decode(MAIL_CONTENT,'" + DBConstantes.MailContent.COUPON_DE_REDUCTION + "',MAIL_CONTENT)) as item4,"
                 + "max(decode(MAIL_CONTENT,'" + DBConstantes.MailContent.ECHANTILLON + "',MAIL_CONTENT)) as item5,"
                 + "max(decode(MAIL_CONTENT,'" + DBConstantes.MailContent.BON_DE_COMMANDE + "',MAIL_CONTENT)) as item6,"
                 + "max(decode(MAIL_CONTENT,'" + DBConstantes.MailContent.JEUX_CONCOUR + "',MAIL_CONTENT)) as item7,"
                 + "max(decode(MAIL_CONTENT,'" + DBConstantes.MailContent.CATALOGUE_BROCHURE + "',MAIL_CONTENT)) as item8,"
                 + "max(decode(MAIL_CONTENT,'" + DBConstantes.MailContent.CADEAU + "',MAIL_CONTENT)) as item9,"
                 + "max(decode(MAIL_CONTENT,'" + DBConstantes.MailContent.ACCELERATEUR_REPONSE + "',MAIL_CONTENT)) as item10";
            }
            sql += " from (";
            return sql;

        }

        /// <summary>
        /// Donne les champs à traiter pour les créations du Marketing Direct.
        /// </summary>
        /// <param name="idVehicle">Identifiant du média</param>
        /// <param name="mediaList">list des détails médias </param>
        /// <param name="webSesssion">Session  du client</param>
        /// <param name="prefixeMediaPlanTable">prfixe table média (vehicle)</param>
        /// <returns>Chaine contenant les champs à traiter</returns>
        private static string getMDSpecificGroupBy(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable, bool withPrefix, bool export)
        {

            string sql = string.Empty;
            bool showProduct = webSesssion.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);

            sql = " ) Group by id_category, category"
                + ", id_media, media"
                + (!export ? ", date_media_num" : "")
                + ", id_advertiser, advertiser";

            if (showProduct) sql += ", id_product, product";
            sql += ", id_group_, group_"
                + ", weight"
                + ", associated_file"
                + (!export ? ", " + UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString() : "")

                + (!export ? ", " + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.volume].Id.ToString() : "")
                + ", id_slogan";
            sql += GetMDGroupByFields(idVehicle, mediaList, webSesssion, prefixeMediaPlanTable, withPrefix);
            return sql;

        }
        #endregion

        #region Get Group By
        /// <summary>
        /// Regroupe les données de la requête
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="sql">Chaine sql</param>
        /// <param name="idVehicle">Identifiant du média</param>
        /// <param name="hasDetailLevelGroupBy">Indique s'il faut regrouper les niveaux de détail</param>
        /// <param name="detailLevelList">Liste des niveaux de détail</param>
        private static void GetSqlGroupByFields(WebSession webSession, StringBuilder sql, DBClassificationConstantes.Vehicles.names idVehicle, ref bool hasDetailLevelGroupBy, ArrayList detailLevelList)
        {

            sql.Append("  group by");
            if (webSession.DetailLevel != null && webSession.DetailLevel.GetSqlGroupByFields().Length > 0)
            {
                sql.Append("  " + webSession.DetailLevel.GetSqlGroupByFields());
                hasDetailLevelGroupBy = true;
            }
            if (webSession.GenericInsertionColumns.GetSqlGroupByFields(detailLevelList).Length > 0)
            {
                if (hasDetailLevelGroupBy) sql.Append(",");
                sql.Append("  " + webSession.GenericInsertionColumns.GetSqlGroupByFields(detailLevelList));
            }
            if (webSession.GenericInsertionColumns != null && webSession.GenericInsertionColumns.GetSqlConstraintGroupByFields() != null && webSession.GenericInsertionColumns.GetSqlConstraintGroupByFields().Length > 0)
            {
                if (webSession.GenericInsertionColumns.GetSqlGroupByFields().Length > 0) sql.Append("  , ");
                sql.Append("  " + webSession.GenericInsertionColumns.GetSqlConstraintGroupByFields());//contraintre regroupement des données
            }

            if ((idVehicle == DBClassificationConstantes.Vehicles.names.press
                || idVehicle == DBClassificationConstantes.Vehicles.names.internationalPress
                ) && AddPressSpecificField(webSession, DbTables.WEB_PLAN_PREFIXE).Length > 0)
            {
                sql.Append("," + AddPressSpecificField(webSession, DbTables.WEB_PLAN_PREFIXE));
            }

            AddSloganField(webSession, sql, idVehicle);

            if (webSession.GenericInsertionColumns.GetSqlFields(detailLevelList).Length > 0 && (idVehicle == DBClassificationConstantes.Vehicles.names.tv) && !webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.category) && !webSession.DetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.category))
            {
                sql.Append(" , " + DbTables.WEB_PLAN_PREFIXE + ".id_category");
            }

        }

        /// <summary>
        /// Donne les champs à traiter pour les créations du Marketing Direct.
        /// </summary>
        /// <param name="idVehicle">Identifiant du média</param>
        /// <param name="mediaList">list des détails médias </param>
        /// <param name="webSesssion">Session  du client</param>
        /// <param name="prefixeMediaPlanTable">prfixe table média (vehicle)</param>
        /// <returns>Chaine contenant les champs à traiter</returns>
        private static string GetMDGroupByFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable, bool withPrefix)
        {
            bool showProduct = webSesssion.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);

            string sql = (withPrefix ? "group by " + DbTables.CATEGORY_PREFIXE + ".id_category, category, " + DbTables.MEDIA_PREFIXE + ".id_media, media, " + prefixeMediaPlanTable + ".date_media_num, " + DbTables.ADVERTISER_PREFIXE + ".id_advertiser, advertiser" : "");
            sql += ((withPrefix && showProduct) ? " , " + DbTables.PRODUCT_PREFIXE + ".id_product, product " : "");
            sql += (withPrefix ? " , " + DbTables.GROUP_PREFIXE + ".id_group_, group_, " + prefixeMediaPlanTable + ".weight, " + prefixeMediaPlanTable + ".associated_file," + prefixeMediaPlanTable + ".id_slogan" : "");

            if (mediaList["id_media"] != null && mediaList["id_media"].ToString() != "-1")
            {

                switch (mediaList["id_media"].ToString())
                {

                    case DBConstantes.Media.PUBLICITE_NON_ADRESSEE:
                        sql += ",  format, "
                            + (withPrefix ? DbTables.MAIL_FORMAT_PREFIXE + "." : "") + "mail_format, "
                            + "mail_type";
                        break;
                    case DBConstantes.Media.COURRIER_ADRESSE_GENERAL:
                        sql += ", " + (withPrefix ? prefixeMediaPlanTable + "." : "wp_") + "mail_format,"
                            + " " + (withPrefix ? prefixeMediaPlanTable + ".object_count," : "object_count")
                            + " " + (withPrefix ? DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content," : "")
                            + " " + (withPrefix ? DbTables.MAIL_CONTENT_PREFIXE + ".mail_content" : "");
                        break;
                    case DBConstantes.Media.COURRIER_ADRESSE_PRESSE:
                        sql += ", " + (withPrefix ? prefixeMediaPlanTable + ".object_count," : "object_count")
                            + " " + (withPrefix ? DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content," : "")
                            + " " + (withPrefix ? DbTables.MAIL_CONTENT_PREFIXE + ".mail_content" : "");
                        break;
                    case DBConstantes.Media.COURRIER_ADRESSE_GESTION:
                        sql += ", " + (withPrefix ? prefixeMediaPlanTable + "." : "") + "object_count,"
                            + " mailing_rapidity" + (withPrefix ? "," : "")
                            + " " + (withPrefix ? DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content," : "")
                            + " " + (withPrefix ? DbTables.MAIL_CONTENT_PREFIXE + ".mail_content" : "");
                        break;
                    default:
                        throw new Exceptions.MediaCreationDataAccessException("GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable) : Ce support ne correspond pas à un support du MD.");
                }

            }
            else if (mediaList["id_category"] != null && mediaList["id_category"].ToString() != "-1")
            {

                switch (mediaList["id_category"].ToString())
                {

                    case DBConstantes.Category.PUBLICITE_NON_ADRESSEE:
                        sql += ",  format, "
                            + (withPrefix ? DbTables.MAIL_FORMAT_PREFIXE + "." : "") + "mail_format, "
                            + "mail_type";
                        break;
                    case DBConstantes.Category.COURRIER_ADRESSE:
                        sql += ", " + (withPrefix ? prefixeMediaPlanTable + "." : "wp_") + "mail_format,"
                            + " " + (withPrefix ? prefixeMediaPlanTable + "." : "") + "object_count,"
                            + " mailing_rapidity" + (withPrefix ? "," : "")
                            + " " + (withPrefix ? DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content," : "")
                            + " " + (withPrefix ? DbTables.MAIL_CONTENT_PREFIXE + ".mail_content" : "");
                        break;
                    default:
                        throw new Exceptions.MediaCreationDataAccessException("GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable) : Cette catégorie ne correspond pas à une catégorie du MD.");
                }

            }
            else
            {

                sql += ",  format, "
                    + (withPrefix ? DbTables.MAIL_FORMAT_PREFIXE + "." : "") + "mail_format, "
                    + "mail_type,"
                    + (withPrefix ? prefixeMediaPlanTable + "." : "wp_") + "mail_format,"
                    + " " + (withPrefix ? prefixeMediaPlanTable + "." : "") + "object_count,"
                    + " mailing_rapidity" + (withPrefix ? "," : "")
                    + " " + (withPrefix ? DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content," : "")
                    + " " + (withPrefix ? DbTables.MAIL_CONTENT_PREFIXE + ".mail_content" : "");

            }

            return sql;

        }
        #endregion

        #region Get Sql order fields

        /// <summary>
        /// Obtient les chaines pour ordonner les données
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="sql">Chaine sql</param>
        /// <param name="idVehicle">Idnetifiant du média</param>
        /// <param name="hasDetailLevelOrder">Indique s'il faut ordonner les données</param>
        /// <param name="detailLevelList">Liste des niveaux de détail</param>
        private static void GetSqlOrderFields(WebSession webSession, StringBuilder sql, DBClassificationConstantes.Vehicles.names idVehicle, ref bool hasDetailLevelOrder, ArrayList detailLevelList)
        {
            sql.Append("  order by ");
            if (webSession.DetailLevel != null && webSession.DetailLevel.GetSqlOrderFields().Length > 0)
            {
                sql.Append("  " + webSession.DetailLevel.GetSqlOrderFields());
                hasDetailLevelOrder = true;
            }
            if (webSession.GenericInsertionColumns.GetSqlOrderFields(detailLevelList).Length > 0)
            {
                if (hasDetailLevelOrder) sql.Append(",");
                sql.Append("  " + webSession.GenericInsertionColumns.GetSqlOrderFields(detailLevelList));
            }
            if (webSession.GenericInsertionColumns != null && webSession.GenericInsertionColumns.GetSqlConstraintOrderFields() != null && webSession.GenericInsertionColumns.GetSqlConstraintOrderFields().Length > 0)
            {
                if (webSession.GenericInsertionColumns.GetSqlOrderFields().Length > 0) sql.Append("  , ");
                sql.Append("  " + webSession.GenericInsertionColumns.GetSqlConstraintOrderFields());//contraintre ordre des données
            }

            if ((idVehicle == DBClassificationConstantes.Vehicles.names.press
                || idVehicle == DBClassificationConstantes.Vehicles.names.internationalPress
                ) && AddPressSpecificField(webSession, DbTables.WEB_PLAN_PREFIXE).Length > 0)
            {
                sql.Append("," + AddPressSpecificField(webSession, DbTables.WEB_PLAN_PREFIXE));
            }

            AddSloganField(webSession, sql, idVehicle);
        }

        /// <summary>
        /// Donne les champs à traiter pour les créations du Marketing Direct.
        /// </summary>
        /// <param name="idVehicle">Identifiant du média</param>
        /// <param name="mediaList">list des détails médias </param>
        /// <param name="webSesssion">Session  du client</param>
        /// <param name="prefixeMediaPlanTable">prfixe table média (vehicle)</param>
        /// <returns>Chaine contenant les champs à traiter</returns>
        private static string GetMDOrderByFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable)
        {
            bool showProduct = webSesssion.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);

            string sql = " Order by " + DbTables.CATEGORY_PREFIXE + ".id_category, category, " + DbTables.MEDIA_PREFIXE + ".id_media, media, " + prefixeMediaPlanTable + ".date_media_num, " + DbTables.ADVERTISER_PREFIXE + ".id_advertiser, advertiser";
            if (showProduct) sql += ", " + DbTables.PRODUCT_PREFIXE + ".id_product, product";
            sql += ", " + DbTables.GROUP_PREFIXE + ".id_group_, group_," + prefixeMediaPlanTable + ".associated_file," + prefixeMediaPlanTable + ".id_slogan";

            if (mediaList["id_media"] != null && mediaList["id_media"].ToString() != "-1")
            {

                switch (mediaList["id_media"].ToString())
                {

                    case DBConstantes.Media.PUBLICITE_NON_ADRESSEE:
                        sql += ",  format, "
                            + DbTables.MAIL_FORMAT_PREFIXE + ".mail_format, "
                            + "mail_type";
                        break;
                    case DBConstantes.Media.COURRIER_ADRESSE_GENERAL:
                        sql += ", " + prefixeMediaPlanTable + ".mail_format,"
                            + " " + prefixeMediaPlanTable + ".object_count,"
                            + " " + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content,"
                            + " " + DbTables.MAIL_CONTENT_PREFIXE + ".mail_content";
                        break;
                    case DBConstantes.Media.COURRIER_ADRESSE_PRESSE:
                        sql += ", " + prefixeMediaPlanTable + ".object_count,"
                            + " " + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content,"
                            + " " + DbTables.MAIL_CONTENT_PREFIXE + ".mail_content";
                        break;
                    case DBConstantes.Media.COURRIER_ADRESSE_GESTION:
                        sql += ", " + prefixeMediaPlanTable + ".object_count,"
                            + " mailing_rapidity,"
                            + " " + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content,"
                            + " " + DbTables.MAIL_CONTENT_PREFIXE + ".mail_content";
                        break;
                    default:
                        throw new Exceptions.MediaCreationDataAccessException("GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable) : Ce support ne correspond pas à un support du MD.");
                }

            }
            else if (mediaList["id_category"] != null && mediaList["id_category"].ToString() != "-1")
            {

                switch (mediaList["id_category"].ToString())
                {

                    case DBConstantes.Category.PUBLICITE_NON_ADRESSEE:
                        sql = ",  format, "
                            + DbTables.MAIL_FORMAT_PREFIXE + ".mail_format, "
                            + "mail_type";
                        break;
                    case DBConstantes.Category.COURRIER_ADRESSE:
                        sql += ", " + prefixeMediaPlanTable + ".mail_format,"
                            + " " + prefixeMediaPlanTable + ".object_count,"
                            + " mailing_rapidity,"
                            + " " + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content,"
                            + " " + DbTables.MAIL_CONTENT_PREFIXE + ".mail_content";
                        break;
                    default:
                        throw new Exceptions.MediaCreationDataAccessException("GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable) : Cette catégorie ne correspond pas à une catégorie du MD.");
                }

            }
            else
            {

                sql += ",  format, "
                    + DbTables.MAIL_FORMAT_PREFIXE + ".mail_format, "
                    + "mail_type,"
                    + prefixeMediaPlanTable + ".mail_format,"
                    + " " + prefixeMediaPlanTable + ".object_count,"
                    + " mailing_rapidity,"
                    + " " + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content,"
                    + " " + DbTables.MAIL_CONTENT_PREFIXE + ".mail_content";

            }

            return sql;

        }
        #endregion

        #endregion

    }
}
