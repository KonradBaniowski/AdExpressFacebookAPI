#region Info
/*
 * Author           : G RAGNEAU 
 * Date             : 20/08/2007
 * Modifications    :
 *      Author - Date - Description
 * 
 *  
 */
#endregion


using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data;
using System.Text;

using CustCst = TNS.AdExpress.Constantes.Customer;
using DBClassifCst = TNS.AdExpress.Constantes.Classification.DB;
using DBCst = TNS.AdExpress.Constantes.DB;
using WebCst = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpress.Web.DataAccess.Results {

    /// <summary>
    /// Get Data for creatives
    /// </summary>
    public class CreativesDataAccess {


		#region GetData (Nouvelle version)
		/// <summary>
        /// Get Creatives Data
        /// </summary>
        /// <param name="session">Web Session</param>
        /// <param name="vehicle">Vehicle Id</param>
        /// <param name="filters">Filters to consider</param>
        /// <param name="fromDate">Period beginning</param>
        /// <param name="toDate">Period End</param>
        /// <param name="universId">Competitor Univers Id</param>
        /// <returns>DataSet containing creatives Data</returns>
        public static DataSet GetData(WebSession session, DBClassifCst.Vehicles.names vehicle, string filters, int fromDate, int toDate, int universId, Int64 moduleId) {

            #region Variables
            StringBuilder sql = new StringBuilder();
            string table = string.Empty;
            string dataTable = string.Empty;

			int posUnivers = 0;
            #endregion

            try {

                universId--;
				
				if (session.CurrentModule == WebCst.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE
					|| session.CurrentModule == WebCst.Module.Name.ANALYSE_PLAN_MEDIA_CONCURENTIELLE) {
										
					sql.Append("select ");
					GetFields(sql, vehicle,true);
					sql.Append(" from ( ");
                    int nbUnivers = 0;
					while (session.PrincipalProductUniverses.ContainsKey(posUnivers) && session.PrincipalProductUniverses[posUnivers] != null) {
                        if (universId < 0 || universId == posUnivers)
                        {
                            if (nbUnivers > 0)
                            {
                                sql.Append(" UNION ");
                            }
                            sql.Append(GetOneUniverseData(session, vehicle, filters, fromDate, toDate, posUnivers, moduleId));
                            
                            nbUnivers++;
                        }
                        posUnivers++;
					}
					//order by 
					sql.Append(" ) order by version");					
				}
				else {
					sql.Append(GetOneUniverseData( session, vehicle, filters, fromDate, toDate, universId, moduleId));
				}
	
               
                return session.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new MediaCreationDataAccessException("GetData::Impossible de charger les données des créations publicitaires " + sql.ToString(), err));
            }

        }

		/// <summary>
		/// Get Creatives Data
		/// </summary>
		/// <param name="session">Web Session</param>
		/// <param name="vehicle">Vehicle Id</param>
		/// <param name="filters">Filters to consider</param>
		/// <param name="fromDate">Period beginning</param>
		/// <param name="toDate">Period End</param>
		/// <param name="universId">Competitor Univers Id</param>
		/// <returns>Requete sql</returns>
		public static string GetOneUniverseData(WebSession session, DBClassifCst.Vehicles.names vehicle, string filters, int fromDate, int toDate, int universId, Int64 moduleId) {

			#region Variables
			StringBuilder sql = new StringBuilder();
			string table = string.Empty;
			string dataTable = string.Empty;
			#endregion

            try {

                Module module = ModulesList.GetModule(session.CurrentModule);
                if(vehicle != DBClassifCst.Vehicles.names.internet) {
                    dataTable = SQLGenerator.GetVehicleTableNameForDetailResult(vehicle, module.ModuleType);
                }
                else {
                    dataTable = GetInternetTable(module.ModuleType);
                }
                sql.Append("select ");
                GetFields(sql, vehicle);
                sql.Append(" from ");
                GetTables(sql, vehicle, session, dataTable);
                sql.Append(" where ");
                GetJoins(sql, vehicle, session);
                sql.Append(" and ");
                GetUniversFilters(sql, session, fromDate, toDate, vehicle.GetHashCode(), universId, moduleId, filters, module);

                //group by
                sql.Append(" group by ");
                GetGroupBy(sql, vehicle);

                if(vehicle == DBClassifCst.Vehicles.names.directMarketing) {
                    SetMDRequest(sql);
                }

                return sql.ToString();
            }
            catch(System.Exception err) {
                throw (new MediaCreationDataAccessException("GetOneUniverseData::Impossible de construire la requete " + sql.ToString(), err));
            }

		}
        #endregion

        #region CheckData
        /// <summary>
        /// Return list of vehicle referenced in the user univers
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="filters">User filters</param>
        /// <param name="fromDate">User Period beginning</param>
        /// <param name="toDate">User Period End</param>
        /// <param name="universId">User Univers Selection</param>
        /// <param name="moduleId">User Current Module</param>
        /// <param name="vehicles">Vehicles to check</param>
        /// <returns>List of vehicles present</returns>
        public static List<int> GetPresentVehicles(WebSession session, string filters, int fromDate, int toDate, int universId, Int64 moduleId, List<int> vehicles) {

            #region Variables
            StringBuilder sql = new StringBuilder();
            List<int> found = new List<int>();
            DataSet ds = null;
            #endregion

            try {

                Module module = ModulesList.GetModule(session.CurrentModule);
                bool first = true;

                universId--;
                string dataTable;
                foreach (int i in vehicles) {
                    if(DBClassifCst.Vehicles.names.internet != (DBClassifCst.Vehicles.names)i) {
                        dataTable = SQLGenerator.GetVehicleTableNameForDetailResult((DBClassifCst.Vehicles.names)i, module.ModuleType);
                    }
                    else {
                        dataTable = GetInternetTable(module.ModuleType);
                    }

                    if (session.CurrentModule == WebCst.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE
                        || session.CurrentModule == WebCst.Module.Name.ANALYSE_PLAN_MEDIA_CONCURENTIELLE)
                    {
                        int posUnivers = 0;
                        while (session.PrincipalProductUniverses.ContainsKey(posUnivers) && session.PrincipalProductUniverses[posUnivers] != null)
                        {

                            if (universId < 0 || universId == posUnivers)
                            {

                                if (!first)
                                    sql.Append(" UNION ");
                                else
                                    first = false;
                                sql.Append(" select id_vehicle from ");
                                sql.AppendFormat(" {0}.{1} wp ", DBCst.Schema.ADEXPRESS_SCHEMA, dataTable);
                                sql.Append(" where ");
                                GetUniversFilters(sql, session, fromDate, toDate, i, universId, moduleId, filters, module);
                                sql.AppendFormat(" and rownum < 2 ");
                            }
                            posUnivers++;

                        }
                    }
                    else
                    {
                        if (!first)
                            sql.Append(" UNION ");
                        else
                            first = false;
                        sql.Append(" select id_vehicle from ");
                        sql.AppendFormat(" {0}.{1} wp ", dataTable);
                        sql.Append(" where ");
                        GetUniversFilters(sql, session, fromDate, toDate, i, universId, moduleId, filters, module);
                        sql.AppendFormat(" and rownum < 2 ");
                    }

                }

                ds = session.Source.Fill(sql.ToString());
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0) {

                    foreach (DataRow row in ds.Tables[0].Rows) {
                        found.Add(Convert.ToInt32(row["id_vehicle"]));
                    }

                }

            }
            catch (System.Exception err) {
                throw (new MediaCreationDataAccessException("GetData::Impossible de vérifier la présence de données pour les créations publicitaires " + sql.ToString(), err));
            }

            return found;
        }
        #endregion

        #region GetInternetTable
        protected static string GetInternetTable(WebCst.Module.Type moduleType) {
            switch(moduleType) {
                case WebCst.Module.Type.alert:
                    return (WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataInternetVersionAlert).Label);
                    break;
                case WebCst.Module.Type.analysis:
                    return (WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataInternetVersion).Label);
                    break;
                default:
                    throw new ArgumentException("Type of module is not supported");
                    break;

            }
        }
        #endregion

        #region GetFields
        /// <summary>
        /// Build select clause
        /// </summary>
        /// <param name="sql">Output</param>
        /// <param name="vehicle">Vehicle considered</param>
		private static void GetFields(StringBuilder sql, TNS.AdExpress.Constantes.Classification.DB.Vehicles.names vehicle) {
			 GetFields( sql,vehicle,false) ;
		}

        /// <summary>
        /// Build select clause
        /// </summary>
        /// <param name="sql">Output</param>
		/// <param name="withOutPrefix">With out prefixe</param>
        /// <param name="vehicle">Vehicle considered</param>
        private static void GetFields(StringBuilder sql, TNS.AdExpress.Constantes.Classification.DB.Vehicles.names vehicle, bool withOutPrefix)
        {
            string prefix = (withOutPrefix) ? "" : "wp.";

            if (withOutPrefix)
                sql.Append(" idadvertiser ,advertiser,id_address, idgroup  , groupe, idproduct, product");
            else
                sql.AppendFormat(" {0}id_advertiser as idadvertiser, advertiser as advertiser, gd.id_address, {0}id_group_ as idgroup, group_ as groupe, {0}id_product as idproduct, product as product", prefix);

            if (vehicle != DBClassifCst.Vehicles.names.adnettrack)
            {
                if (withOutPrefix)
                    sql.AppendFormat(", {0}", UnitsInformation.List[WebCst.CustomerSessions.Unit.euro].Id.ToString());
                else
                    sql.AppendFormat(", sum({0}{1}) as {2}"
                        , prefix
                        , UnitsInformation.List[WebCst.CustomerSessions.Unit.euro].DatabaseField
                        , UnitsInformation.List[WebCst.CustomerSessions.Unit.euro].Id.ToString());

            }

            switch (vehicle)
            {
                case DBClassifCst.Vehicles.names.press:
                case DBClassifCst.Vehicles.names.internationalPress:
                    if (withOutPrefix)
                        sql.AppendFormat(",  version,  volume, {0}, nbsupport, visuel ", UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString());
                    else
                        sql.AppendFormat(", {0}id_slogan as version, sum({0}{1})/1000 as volume, sum({0}{2}) as {3}, count(distinct {0}id_media) as nbsupport, max({0}id_media || '/' || {0}date_cover_num || '/imagette/' || replace({0}visual, ',' , ',' || {0}id_media || '/' || {0}date_cover_num || '/imagette/') ) as visuel "
                            , prefix
                            , UnitsInformation.List[WebCst.CustomerSessions.Unit.pages].DatabaseField
                            , UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].DatabaseField
                            , UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString());
                    break;

                case DBClassifCst.Vehicles.names.radio:
                case DBClassifCst.Vehicles.names.tv:
                case DBClassifCst.Vehicles.names.others:
                    if (withOutPrefix)
                        sql.AppendFormat(", version,  {0},  {1},  nbsupport,  visuel "
                            , UnitsInformation.List[WebCst.CustomerSessions.Unit.duration].Id.ToString()
                            , UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString());
                    else
                        sql.AppendFormat(", {0}id_slogan as version, sum({0}{1}) as {2}, sum({0}{3}) as {4}, count(distinct {0}id_media) as nbsupport, max({0}associated_file) as visuel "
                            , prefix
                            , UnitsInformation.List[WebCst.CustomerSessions.Unit.duration].DatabaseField
                            , UnitsInformation.List[WebCst.CustomerSessions.Unit.duration].Id.ToString()
                            , UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].DatabaseField
                            , UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString());
                    break;

                case DBClassifCst.Vehicles.names.outdoor:
                    if (withOutPrefix)
                        sql.AppendFormat(", version,  {0},  nbsupport,  visuel ", UnitsInformation.List[WebCst.CustomerSessions.Unit.numberBoard].Id.ToString());
                    else 
                        sql.AppendFormat(", {0}id_slogan as version, sum({0}{1}) as {2}, count(distinct {0}id_media) as nbsupport, max({0}associated_file) as visuel "
                            , prefix
                            , UnitsInformation.List[WebCst.CustomerSessions.Unit.numberBoard].DatabaseField
                            , UnitsInformation.List[WebCst.CustomerSessions.Unit.numberBoard].Id.ToString());
                    break;

                case DBClassifCst.Vehicles.names.adnettrack:
                    if (withOutPrefix)
                        sql.Append(", dProduct, idAdvertiser,  version,  dimension,  format,  url, visuel ");
                    else 
                        sql.AppendFormat(", {0}id_product as idProduct, {0}id_advertiser as idAdvertiser, {0}hashcode as version, {0}dimension as dimension, {0}format as format, {0}url as url, max({0}associated_file) as visuel ", prefix);
                    break;

                case DBClassifCst.Vehicles.names.directMarketing:
                    if (withOutPrefix)
                    {
                        sql.Append(",  version, media as media, weight,  volume, nbobjet,  visuel ");
                        sql.Append(",  format,  mail_format, mail_type,  standard,  rapidity, id_mail_content, mail_content");
                    }
                    else
                    {
                        sql.AppendFormat(", {0}id_slogan as version, media as media, {0}weight as weight, sum({0}volume) as volume, {0}object_count as nbobjet, max({0}associated_file) as visuel ", prefix);
                        sql.AppendFormat(", fr.format as format, mf.mail_format as mail_format, mt.mail_type as mail_type, {0}mail_format as standard, mr.mailing_rapidity as rapidity, mc.id_mail_content as id_mail_content, mc.mail_content as mail_content", prefix);
                    }
                    break;

                default:
                    throw new Exceptions.MediaCreationDataAccessException("Le cas de ce média n'est pas gérer.");
            }

        }
        #endregion

        #region GetTables
        /// <summary>
        /// Generate from clause
        /// </summary>
        /// <param name="sql">Output</param>
        /// <param name="vehicle">Vehicle</param>
        /// <param name="session">Web Session</param>
        /// <param name="dataTable">Table utilisée</param>
        private static void GetTables(StringBuilder sql, DBClassifCst.Vehicles.names vehicle, WebSession session, string dataTable) {
            
            sql.AppendFormat(" {0}.{1} wp", DBCst.Schema.ADEXPRESS_SCHEMA, dataTable);
            sql.AppendFormat(", {0}.{1} ad, {0}.{2} gp, {0}.{3} pr, {0}.{4} gd"
                , DBCst.Schema.ADEXPRESS_SCHEMA
                , DBCst.Tables.ADVERTISER
                , DBCst.Tables.GROUP_
                , DBCst.Tables.PRODUCT
                , DBCst.Tables.GAD);

            switch (vehicle) {
                case DBClassifCst.Vehicles.names.directMarketing:
                    sql.AppendFormat(", {0}.{1} md, {0}.{2} fr, {0}.{3} mf, {0}.{4} mt, {0}.{5} mr, {0}.{6} mc, {0}.{7} dmc"
                        , DBCst.Schema.ADEXPRESS_SCHEMA
                        , DBCst.Tables.MEDIA
                        , DBCst.Tables.FORMAT
                        , DBCst.Tables.MAIL_FORMAT
                        , DBCst.Tables.MAIL_TYPE
                        , DBCst.Tables.MAILING_RAPIDITY
                        , DBCst.Tables.MAIL_CONTENT
                        , DBCst.Tables.DATA_MAIL_CONTENT);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region GetJoins
        /// <summary>
        /// Get Jointure clauses
        /// </summary>
        /// <param name="sql">Output</param>
        /// <param name="vehicle">Vehicle considered</param>
        /// <param name="session">Web Sessions</param>
        private static void GetJoins(StringBuilder sql, DBClassifCst.Vehicles.names vehicle, WebSession session) {

            //produit
            sql.Append(" pr.id_product=wp.id_product ");
			sql.AppendFormat(" and pr.id_language={0}", session.DataLanguage);
            sql.AppendFormat(" and pr.activation < {0}", DBCst.ActivationValues.UNACTIVATED);

            // Annonceur
            sql.Append(" and ad.id_advertiser=wp.id_advertiser ");
			sql.AppendFormat(" and ad.id_language={0}", session.DataLanguage);
            sql.AppendFormat(" and ad.activation < {0}", DBCst.ActivationValues.UNACTIVATED);

            // Groupe
            sql.Append(" and gp.id_group_ = wp.id_group_ ");
			sql.AppendFormat(" and gp.id_language = {0}", session.DataLanguage);
            sql.AppendFormat(" and gp.activation < {0}", DBCst.ActivationValues.UNACTIVATED);

            //GAD
            sql.Append(" and gd.id_advertiser(+)=wp.id_advertiser ");

            if (vehicle == DBClassifCst.Vehicles.names.directMarketing) {
                // Media
                sql.Append(" and md.id_media = wp.id_media ");
				sql.AppendFormat(" and md.id_language = {0}", session.DataLanguage);
                sql.AppendFormat(" and md.activation < {0}", DBCst.ActivationValues.UNACTIVATED);
                //format
                sql.Append(" and fr.id_format (+)= wp.id_format ");
				sql.AppendFormat(" and fr.id_language (+)= {0}", session.DataLanguage);
                //mail_format
                sql.Append(" and mf.id_mail_format (+)= wp.id_mail_format ");
				sql.AppendFormat(" and mf.id_language (+)= {0}", session.DataLanguage);
                //mail_type
                sql.Append(" and mt.id_mail_type (+)= wp.id_mail_type ");
				sql.AppendFormat(" and mt.id_language (+)= {0}", session.DataLanguage);
                //mailing rapidity
                sql.Append(" and mr.id_mailing_rapidity (+)= wp.id_mailing_rapidity ");
				sql.AppendFormat(" and mr.id_language (+)= {0}", session.DataLanguage);
                //mail content
                sql.Append(" and wp.id_media = dmc.id_media (+) ");
                sql.Append(" and wp.date_media_num = dmc.date_media_num (+) ");
                sql.Append(" and wp.id_cobranding_advertiser = dmc.id_cobranding_advertiser (+) ");
                sql.Append(" and wp.id_data_marketing_direct_panel = dmc.id_data_marketing_direct_panel (+) ");
                sql.Append(" and mc.id_mail_content (+) = dmc.id_mail_content ");
				sql.AppendFormat(" and mc.id_language (+)= {0}", session.DataLanguage);

            }
        }
        #endregion

        #region GetUniversFilters
        private static void GetUniversFilters(StringBuilder sql, WebSession session, int fromDate, int toDate, int vehicle, int universId, Int64 moduleId, string filters, Module module) {

            // Période
            if (session.PeriodType == WebCst.CustomerSessions.Period.Type.dateToDate)
            {
                int begin = Convert.ToInt32(session.PeriodBeginningDate);
                if (begin > fromDate)
                {
                    sql.AppendFormat(" wp.date_media_num >= {0}", begin);
                }
                else
                {
                    sql.AppendFormat(" wp.date_media_num >= {0}", fromDate);
                }
                int end = Convert.ToInt32(session.PeriodEndDate);
                if (end < toDate)
                {
                    sql.AppendFormat(" and wp.date_media_num <= {0}", end);
                }
                else
                {
                    sql.AppendFormat(" and wp.date_media_num <= {0}", toDate);
                }
            }
            else
            {
                sql.AppendFormat(" wp.date_media_num >= {0}", fromDate);
                sql.AppendFormat(" and wp.date_media_num <= {0}", toDate);
            }

            #region Approche annonceur

            #region Univers de versions
            if (session.CurrentModule == WebCst.Module.Name.ALERTE_PLAN_MEDIA && session.SloganIdList != null && session.SloganIdList.Length > 0) {
                sql.AppendFormat(" and wp.id_slogan in ( {0} ) ", session.SloganIdList);
            }
            if (vehicle != DBClassifCst.Vehicles.names.adnettrack.GetHashCode())
                sql.AppendFormat(" and wp.id_slogan is not null ");
            #endregion

          

			//Sélection Nomenclature produit
            if (session.PrincipalProductUniverses != null && session.PrincipalProductUniverses.Count > 0)
            {
                if (universId < 0)
                {
                    sql.Append(session.PrincipalProductUniverses[0].GetSqlConditions("wp", true));
                }
                else
                {
                    sql.Append(session.PrincipalProductUniverses[universId].GetSqlConditions("wp", true));
                }
            }


            #endregion

            #region Approche Media

            #region Sélection de Médias
            string listMediaAccess = string.Empty;
            if (moduleId == WebCst.Module.Name.ALERTE_PORTEFEUILLE ||
                moduleId == WebCst.Module.Name.ANALYSE_PORTEFEUILLE)
            {
                listMediaAccess += session.GetSelection((TreeNode)session.ReferenceUniversMedia, CustCst.Right.type.mediaAccess) + ",";
            }
            if (moduleId == WebCst.Module.Name.ALERTE_CONCURENTIELLE ||
                moduleId == WebCst.Module.Name.ALERTE_POTENTIELS ||
                moduleId == WebCst.Module.Name.ANALYSE_CONCURENTIELLE ||
                moduleId == WebCst.Module.Name.ANALYSE_POTENTIELS)
            {
                int positionUnivers = 1;
                while (session.CompetitorUniversMedia[positionUnivers] != null)
                {
                    listMediaAccess += session.GetSelection((TreeNode)session.CompetitorUniversMedia[positionUnivers], CustCst.Right.type.mediaAccess) + ",";
                    positionUnivers++;
                }


            }
            if (listMediaAccess.Length > 0)
            {
                sql.AppendFormat(" and ((wp.id_media in ({0}))) ", listMediaAccess.Substring(0, listMediaAccess.Length - 1));
            }
            #endregion

            #endregion

            #region Droits

            #region Droits Nomenclature Produit
            sql.Append(SQLGenerator.getAnalyseCustomerProductRight(session, "wp", true));
            #endregion

            #region Droits Nomenclature Supports
            // On ne tient pas compte des droits vehicle pour les plans media AdNetTrack
            if (vehicle == VehiclesInformation.EnumToDatabaseId(DBClassifCst.Vehicles.names.adnettrack))
                sql.Append(SQLGenerator.GetAdNetTrackMediaRight(session, "wp", true));
            else
                sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(session, "wp", true));
            #endregion

            #endregion

            #region Univers support du parrainage
            if (module.Id == WebCst.Module.Name.ANALYSE_DES_PROGRAMMES
                || module.Id == WebCst.Module.Name.ANALYSE_DES_DISPOSITIFS) {
                string tmp = string.Empty;
                if (session.CurrentUniversMedia != null && session.CurrentUniversMedia.Nodes.Count > 0) {
                    tmp = session.GetSelection(session.CurrentUniversMedia, CustCst.Right.type.mediaAccess);
                }
                if (tmp.Length > 0)
                    sql.AppendFormat(" and  wp.id_media in ({0}) ", tmp);
                //Nomenclature Emission
                //Sélection des émissions
                sql.Append(SQLGenerator.GetCustomerProgramSelection(session, "wp", "wp", true));
                //sélection des formes de parrainages
                sql.Append(SQLGenerator.GetCustomerSponsorshipFormSelection(session, "wp", true));
            }
            #endregion

            #region règles générales
            // Filtre Niveau Nomenclature produits
            switch (moduleId) {
                case WebCst.Module.Name.ALERTE_PLAN_MEDIA:
                case WebCst.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE:
                case WebCst.Module.Name.ANALYSE_PLAN_MEDIA:
                case WebCst.Module.Name.ANALYSE_PLAN_MEDIA_CONCURENTIELLE:
                //case WebCst.Module.Name.ANALYSE_CONCURENTIELLE:
                case WebCst.Module.Name.ANALYSE_DYNAMIQUE:
                case WebCst.Module.Name.ANALYSE_PORTEFEUILLE:
                case WebCst.Module.Name.ANALYSE_POTENTIELS:
                    sql.Append(SQLGenerator.getLevelProduct(session, "wp", true));
                    break;
            }


            // Produit à exclure en radio
            if (Modules.IsSponsorShipTVModule(session))
                sql.Append(SQLGenerator.getAdExpressUniverseCondition(WebCst.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID, "wp", "wp", "wp", true));
            else
                sql.Append(SQLGenerator.GetAdExpressProductUniverseCondition(WebCst.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, "wp", true, false));
            sql.Append(" and wp.id_category<>35  ");// Pas d'affichage de TV NAT thématiques
            #endregion

            #region Filtres
            if (filters != null && filters.Length > 0) {
                GenericDetailLevel detailLevels = null;
                switch (moduleId) {
                    case WebCst.Module.Name.ALERTE_CONCURENTIELLE:
                    case WebCst.Module.Name.ALERTE_PORTEFEUILLE:
                    case WebCst.Module.Name.ALERTE_POTENTIELS:
                    case WebCst.Module.Name.ANALYSE_CONCURENTIELLE:
                        detailLevels = session.GenericProductDetailLevel;
                        break;
                    case WebCst.Module.Name.ALERTE_PLAN_MEDIA:
                    case WebCst.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE:
                    case WebCst.Module.Name.ANALYSE_PLAN_MEDIA:
                    case WebCst.Module.Name.ANALYSE_PLAN_MEDIA_CONCURENTIELLE:
                    
                    case WebCst.Module.Name.ANALYSE_DYNAMIQUE:
                    case WebCst.Module.Name.ANALYSE_PORTEFEUILLE:
                    case WebCst.Module.Name.ANALYSE_POTENTIELS:
                    case WebCst.Module.Name.ANALYSE_DES_DISPOSITIFS:
                    case WebCst.Module.Name.ANALYSE_DES_PROGRAMMES:
                        detailLevels = session.GenericMediaDetailLevel;
                        break;
                }
                sql.Append(GetFiltersClause(session, detailLevels, filters, vehicle));
                sql.AppendFormat(CheckZeroVersion(detailLevels, VehiclesInformation.DatabaseIdToEnum(vehicle), filters));
            }
            if (session.SloganIdZoom > -1)
            {
                sql.AppendFormat(" and wp.id_slogan={0}", session.SloganIdZoom);
            }
            #endregion

            sql.AppendFormat(" and wp.id_vehicle={0}", vehicle);

        }
        #endregion

        #region GetGroupby

		 /// <summary>
        /// Build group by clause
        /// </summary>
        /// <param name="sql">Output</param>
        /// <param name="vehicle">Vehicle considered</param>
		private static void GetGroupBy(StringBuilder sql, TNS.AdExpress.Constantes.Classification.DB.Vehicles.names vehicle) {
			 GetGroupBy(sql,  vehicle, false);
		}
        /// <summary>
        /// Build group by clause
        /// </summary>
        /// <param name="sql">Output</param>
        /// <param name="vehicle">Vehicle considered</param>
        private static void GetGroupBy(StringBuilder sql, TNS.AdExpress.Constantes.Classification.DB.Vehicles.names vehicle, bool withOutPrefix)
        {

            string prefix = (withOutPrefix) ? "" : "wp.";

            sql.AppendFormat(" {0}id_advertiser, advertiser, gd.id_address, {0}id_group_ , group_ , {0}id_product , product ", prefix);

            switch (vehicle)
            {
                case DBClassifCst.Vehicles.names.press:
                case DBClassifCst.Vehicles.names.internationalPress:
                    sql.AppendFormat(", {0}id_slogan ", prefix);
                    break;

                case DBClassifCst.Vehicles.names.radio:
                    sql.AppendFormat(", {0}id_slogan ", prefix);
                    break;
                case DBClassifCst.Vehicles.names.tv:
                case DBClassifCst.Vehicles.names.others:
                    sql.AppendFormat(", {0}id_slogan ", prefix);
                    break;

                case DBClassifCst.Vehicles.names.outdoor:
                    sql.AppendFormat(", {0}id_slogan ", prefix);
                    break;

                case DBClassifCst.Vehicles.names.adnettrack:
                    sql.AppendFormat(", {0}id_product, {0}id_advertiser, {0}hashcode, {0}dimension, {0}format, {0}url ", prefix);
                    break;

                case DBClassifCst.Vehicles.names.directMarketing:
                    sql.AppendFormat(", {0}id_slogan, media, {0}weight, {0}object_count ", prefix);
                    sql.AppendFormat(", fr.format, mf.mail_format, mt.mail_type, {0}mail_format, mr.mailing_rapidity, mc.id_mail_content, mc.mail_content ", prefix);
                    break;

                default:
                    throw new Exceptions.MediaCreationDataAccessException("Le cas de ce média n'est pas gérer.");
            }

        }
        #endregion

        #region GetSelection
        /// <summary>
        /// Generate selection in current environment
        /// </summary>
        /// <param name="sql">Output</param>
        /// <param name="session">Web Session</param>
        private static void GetSelection(StringBuilder sql, WebSession session) {

			string dataTablePrefixe = "wp";

			
			bool isException = false;
			bool premier = true;

			Dictionary<CustCst.Right.type, string> rightTypes = new Dictionary<CustCst.Right.type, string>();
			rightTypes.Add(CustCst.Right.type.holdingCompanyAccess, "wp.id_holding_company");
			rightTypes.Add(CustCst.Right.type.advertiserAccess, "wp.id_advertiser");
			rightTypes.Add(CustCst.Right.type.brandAccess, "wp.id_brand");
			rightTypes.Add(CustCst.Right.type.productAccess, "wp.id_product");
			rightTypes.Add(CustCst.Right.type.sectorAccess, "wp.id_sector");
			rightTypes.Add(CustCst.Right.type.subSectorAccess, "wp.id_subsector");
			rightTypes.Add(CustCst.Right.type.groupAccess, "wp.id_group_");
			rightTypes.Add(CustCst.Right.type.segmentAccess, "wp.id_segment");
			rightTypes.Add(CustCst.Right.type.holdingCompanyException, "wp.id_holding_company");
			rightTypes.Add(CustCst.Right.type.advertiserException, "wp.id_advertiser");
			rightTypes.Add(CustCst.Right.type.brandException, "wp.id_brand");
			rightTypes.Add(CustCst.Right.type.productException, "wp.id_product");
			rightTypes.Add(CustCst.Right.type.sectorException, "wp.id_sector");
			rightTypes.Add(CustCst.Right.type.subSectorException, "wp.id_subsector");
			rightTypes.Add(CustCst.Right.type.groupException, "wp.id_group_");
			rightTypes.Add(CustCst.Right.type.segmentException, "wp.id_segment");

			string list;
			foreach (CustCst.Right.type type in rightTypes.Keys) {

				list = string.Empty;

				if (type.ToString().IndexOf("Exception") > 0) {
					if (!premier)
						sql.Append(")");
					premier = true;
					isException = true;
				}


				list = session.GetSelection(session.CurrentUniversAdvertiser, type);

				if (list.Length > 0) {
					if (premier) {
						sql.Append(" and (");
						premier = false;
					}
					else {
						sql.AppendFormat(" {0} ",
							isException ? "and" : "or"
						 );
					}

					sql.AppendFormat(" {0} {1} in ({2})"
						, rightTypes[type]
						, isException ? "not" : string.Empty
						, list);
				}

			}

			if (!premier)
				sql.Append(")");
			

		}

        /// <summary>
        /// Generate selection in competitor environnement
        /// </summary>
        /// <param name="sql">Output</param>
        /// <param name="session">Session</param>
        /// <param name="universId">Univers Id Number</param>
        private static void GetCompetitorSelection(StringBuilder sql, WebSession session, int universId) {


            int posUnivers = 1;
            bool isException = false;
            bool premier = true;

            Dictionary<CustCst.Right.type, string> rightTypes = new Dictionary<CustCst.Right.type, string>();
            rightTypes.Add(CustCst.Right.type.holdingCompanyAccess, "wp.id_holding_company");
            rightTypes.Add(CustCst.Right.type.advertiserAccess, "wp.id_advertiser");
            rightTypes.Add(CustCst.Right.type.brandAccess, "wp.id_brand");
            rightTypes.Add(CustCst.Right.type.productAccess, "wp.id_product");
            rightTypes.Add(CustCst.Right.type.holdingCompanyException, "wp.id_holding_company");
            rightTypes.Add(CustCst.Right.type.advertiserException, "wp.id_advertiser");
            rightTypes.Add(CustCst.Right.type.brandException, "wp.id_brand");
            rightTypes.Add(CustCst.Right.type.productException, "wp.id_product");
            
            StringBuilder list;
            foreach (CustCst.Right.type type in rightTypes.Keys) {

                posUnivers = 1;
                list = new StringBuilder();

                if (type.ToString().IndexOf("Exception") > 0) {
                    if (!premier)
                        sql.Append(")");
                    premier = true;
                    isException = true;
                }

                while (session.CompetitorUniversAdvertiser[posUnivers] != null) {

                    if (posUnivers == universId || universId == -1) {

                        int i = list.Length;
                        list.Append(session.GetSelection(((CompetitorAdvertiser)session.CompetitorUniversAdvertiser[posUnivers]).TreeCompetitorAdvertiser, type));
                        if (list.Length > i)
                            list.Append(",");
                    }
                    posUnivers++;
                }

                if (list.Length > 0) {
                    list.Length--;
                    if (premier)
                        sql.Append(" and (");
                    else
                        sql.AppendFormat(" {0} ",
                            isException ? "and" : "or"
                         );

                    sql.AppendFormat(" {0} {1} in ({2})"
                        , rightTypes[type]
                        , isException?"not":string.Empty
                        , list.ToString());
                    premier = false;
                }

            }

            if (!premier)
                sql.Append(")");

        }
        #endregion

        #region SetMDRequest
        /// <summary>
        /// Set MD specific request parameters
        /// </summary>
        /// <param name="sql">Output</param>
        private static void SetMDRequest(StringBuilder sql) {
            StringBuilder md = new StringBuilder();
            md.Append("select idadvertiser, advertiser, id_address, idgroup, groupe, idproduct, product");
            md.AppendFormat(", {0}", UnitsInformation.List[WebCst.CustomerSessions.Unit.euro].Id.ToString());
            md.Append(", version, media, weight, volume, nbobjet, visuel ");
            md.Append(", format, mail_format, mail_type, standard, rapidity"); 
            md.AppendFormat(",max(decode(id_mail_content,{0},MAIL_CONTENT || ', ')) || ", DBCst.MailContent.ID_LETTRE_ACCOMP_PERSONALIS);
            md.AppendFormat("max(decode(id_mail_content,{0},MAIL_CONTENT || ', ')) || ", DBCst.MailContent.ID_ENV_RETOUR_PRE_IMPRIMEE);
            md.AppendFormat("max(decode(id_mail_content,{0},MAIL_CONTENT || ', ')) || ", DBCst.MailContent.ID_ENV_RETOUR_A_TIMBRER);
            md.AppendFormat("max(decode(id_mail_content,{0},MAIL_CONTENT || ', ')) || ", DBCst.MailContent.ID_COUPON_DE_REDUCTION);
            md.AppendFormat("max(decode(id_mail_content,{0},MAIL_CONTENT || ', ')) || ", DBCst.MailContent.ID_ECHANTILLON);
            md.AppendFormat("max(decode(id_mail_content,{0},MAIL_CONTENT || ', ')) || ", DBCst.MailContent.ID_BON_DE_COMMANDE);
            md.AppendFormat("max(decode(id_mail_content,{0},MAIL_CONTENT || ', ')) || ", DBCst.MailContent.ID_JEUX_CONCOUR);
            md.AppendFormat("max(decode(id_mail_content,{0},MAIL_CONTENT || ', ')) || ", DBCst.MailContent.ID_CATALOGUE_BROCHURE);
            md.AppendFormat("max(decode(id_mail_content,{0},MAIL_CONTENT || ', ')) || ", DBCst.MailContent.ID_CADEAU);
            md.AppendFormat("max(decode(id_mail_content,{0},MAIL_CONTENT)) as mail_content", DBCst.MailContent.ID_ACCELERATEUR_REPONSE);
            md.Append(" from (");


            sql.Insert(0, md.ToString());

            sql.Append(") group by idadvertiser, advertiser, id_address, idgroup, groupe, idproduct, product");
            sql.AppendFormat(", {0}", UnitsInformation.List[WebCst.CustomerSessions.Unit.euro].Id.ToString());
            sql.Append(", version, media, weight, volume, nbobjet, visuel ");
            sql.Append(", format, mail_format, mail_type, standard, rapidity");


        }
        #endregion

        #region versions 0
        /// <summary>
        /// Test si on demande une version égale à zero
        /// </summary>
        /// <param name="detailLevels">Detail Levels Selected<</param>
        /// <param name="vehicle">Vehicle</param>
        /// <returns>SQL</returns>
        private static string CheckZeroVersion(GenericDetailLevel detailLevels,DBClassifCst.Vehicles.names vehicle,string filters) {
            int id = 0;
            string[] ids = filters.Split(',');
            int rank=detailLevels.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan);
            if(rank!=0) {
                id = Convert.ToInt32(ids[rank-1]);
                if(id==0 && vehicle!=DBClassifCst.Vehicles.names.adnettrack) return (" and wp.id_slogan is null ");
            }
        return ("");
        }
        #endregion

        #region GetFilters
        /// <summary>
        /// Generate filters clause
        /// </summary>
        /// <param name="session">Web Session</param>
        /// <param name="detailLevels">Detail Levels Selected</param>
        /// <param name="filters">Filters Ids</param>
        /// <param name="vehicle">Vehicle Id</param>
        /// <returns>Filters clause</returns>
        private static string GetFiltersClause(WebSession session, GenericDetailLevel detailLevels, string filters, int vehicle) {

            StringBuilder str = new StringBuilder();
            string[] ids = filters.Split(',');

            DetailLevelItemInformation level = null;
            int id = 0;
            for (int i = 0; i < ids.Length && i < detailLevels.Levels.Count; i++) {
                id = Convert.ToInt32(ids[i]);
                level = (DetailLevelItemInformation)detailLevels.Levels[i];
                if (id > 0) {
					if (level.DataBaseIdField == DBCst.Fields.ID_VEHICLE && id == VehiclesInformation.EnumToDatabaseId(DBClassifCst.Vehicles.names.internet))
                        id = DBClassifCst.Vehicles.names.adnettrack.GetHashCode();
                    str.AppendFormat(" and wp.{0} = {1}", level.DataBaseIdField, id);
                }
                if (id == 0 && level.Id == DetailLevelItemInformation.Levels.slogan && vehicle != VehiclesInformation.EnumToDatabaseId(DBClassifCst.Vehicles.names.adnettrack)) {
                    str.AppendFormat(" and wp.{0} = {1}",level.DataBaseIdField,id);
                }
                
            }

            return str.ToString();
        }
        #endregion
    }

}
