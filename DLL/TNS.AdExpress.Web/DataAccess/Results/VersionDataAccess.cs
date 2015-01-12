#region Informations
/* Auteur: G. Ragneau
 * Creation : 16/07/2006
 * Modification:
 *		Author - Date - Description
 * 
 * */
#endregion

using System;
using System.Collections;
using System.Data;
using Oracle.DataAccess.Client;

//using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Constantes.DB;
using DBCst = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Constantes.Web;
using CustomerCst=TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Exceptions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Units;


namespace TNS.AdExpress.Web.DataAccess.Results{
	/// <summary>
	/// VersionDataAccess provides access to the versions
	/// </summary>
	public class VersionDataAccess{

		#region Get Sample of visual for an press version
//		/// <summary>
//		/// Get a sample of visual for each version of a list of version
//		/// </summary>
//		/// <param name="versions">List of verions</param>
//		/// <param name="webSession">Customer Session (for datasource)</param>
//		/// <returns>DataSet containing records as (id,visual,idmedia,datenum)</returns>
//		public static DataSet GetPressVersions(ICollection versions, WebSession webSession){
//			return GetPressVersions(versions, webSession, Constantes.DB.Tables.ALERT_DATA_PRESS, Constantes.DB.Schema.ADEXPRESS_SCHEMA);
//		}

        		/// <summary>
		/// Get a sample of visual for each version of a list of version
		/// </summary>
		/// <param name="versions">List of verions</param>
		/// <param name="webSession">Customer Session (for datasource)</param>
		/// <param name="vehicle">vehicle</param>
		/// <returns>DataSet containing records as (id,visual,idmedia,datenum)</returns>
        public static DataSet GetVersions(ICollection versions, WebSession webSession, DBCst.Vehicles.names vehicle) {
            return GetVersions(versions, webSession, vehicle, null);
        }

		/// <summary>
		/// Get a sample of visual for each version of a list of version
		/// </summary>
		/// <param name="versions">List of verions</param>
		/// <param name="webSession">Customer Session (for datasource)</param>
		/// <param name="vehicle">vehicle</param>
        /// <param name="period">Période utilisée</param>
		/// <returns>DataSet containing records as (id,visual,idmedia,datenum)</returns>
		public static DataSet GetVersions(ICollection versions, WebSession webSession,DBCst.Vehicles.names vehicle, MediaSchedulePeriod period){
			string creationFileFieldName ="associated_file";
            DateTime today = DateTime.Now.Date;
            DateTime begin;
            if(period != null)
                begin = WebFunctions.Dates.getPeriodBeginningDate(period.Begin.ToString("yyyyMMdd"), webSession.PeriodType);
            else
                begin = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);


            string tableSuffix = string.Empty;
            if (begin >= today.AddDays(1 - today.Day).AddMonths(-3)) {
                tableSuffix = "_4M";
            }

			switch(vehicle){

				case DBCst.Vehicles.names.radio:
					return GetVersions(versions, webSession, string.Format("{0}{1}",Constantes.DB.Tables.DATA_RADIO,tableSuffix), Constantes.DB.Schema.ADEXPRESS_SCHEMA,creationFileFieldName,period);

				case DBCst.Vehicles.names.tv:
					return GetVersions(versions, webSession, string.Format("{0}{1}",Constantes.DB.Tables.DATA_TV,tableSuffix), Constantes.DB.Schema.ADEXPRESS_SCHEMA,creationFileFieldName,period);

                case DBCst.Vehicles.names.directMarketing:
                    return GetVersions(versions, webSession, string.Format("{0}{1}",Constantes.DB.Tables.DATA_MARKETING_DIRECT,tableSuffix), Constantes.DB.Schema.ADEXPRESS_SCHEMA, creationFileFieldName,period);

                case DBCst.Vehicles.names.outdoor:
                    return GetVersions(versions, webSession, string.Format("{0}{1}",Constantes.DB.Tables.DATA_OUTDOOR,tableSuffix), Constantes.DB.Schema.ADEXPRESS_SCHEMA, creationFileFieldName,period);

                case DBCst.Vehicles.names.instore:
                    return GetVersions(versions, webSession, string.Format("{0}{1}", Constantes.DB.Tables.DATA_INSTORE, tableSuffix), Constantes.DB.Schema.ADEXPRESS_SCHEMA, creationFileFieldName, period);

				case DBCst.Vehicles.names.press:
					return GetPressVersions(versions, webSession, string.Format("{0}{1}",Constantes.DB.Tables.DATA_PRESS,tableSuffix), Constantes.DB.Schema.ADEXPRESS_SCHEMA,period);
				
				case DBCst.Vehicles.names.internationalPress:
					return GetPressVersions(versions, webSession, string.Format("{0}{1}",Constantes.DB.Tables.DATA_PRESS_INTER,tableSuffix), Constantes.DB.Schema.ADEXPRESS_SCHEMA,period);

				default :
						throw new VersionDataAccessException(" Impossible d'indentifier le media (vehicle) à traiter.");
			}
		}

        /// <summary>
        /// Get a sample of visual for each version of a list of version
        /// </summary>
        /// <param name="versions">List of verions</param>
        /// <param name="webSession">Customer Session (for datasource)</param>
        /// <param name="vehicle">vehicle</param>
        /// <param name="period">Période utilisée</param>
        /// <returns>DataSet containing records as (id,visual,idmedia,datenum)</returns>
        public static DataSet GetVersionsAllMedia(ICollection versions, WebSession webSession, DBCst.Vehicles.names vehicle, MediaSchedulePeriod period) {
            string creationFileFieldName = "associated_file";
            DateTime today = DateTime.Now.Date;
            DateTime begin;
            if (period != null)
                begin = WebFunctions.Dates.getPeriodBeginningDate(period.Begin.ToString("yyyyMMdd"), webSession.PeriodType);
            else
                begin = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);


            string tableSuffix = string.Empty;
            if (begin >= today.AddDays(1 - today.Day).AddMonths(-3)) {
                tableSuffix = "_4M";
            }

            switch (vehicle) {

                case DBCst.Vehicles.names.press:
                    return GetPressVersionsAllMedia(versions, webSession, string.Format("{0}{1}", Constantes.DB.Tables.DATA_PRESS, tableSuffix), Constantes.DB.Schema.ADEXPRESS_SCHEMA, period);

                default:
                    throw new VersionDataAccessException(" Impossible d'indentifier le media (vehicle) à traiter.");
            }
        }
		#endregion

		#region Get Detail of visual for an press version
		/// <summary>
		/// Get a sample of visual for each version of a list of version
		/// </summary>
		/// <param name="versions">List of verions</param>
		/// <param name="webSession">Customer Session (for datasource)</param>
		/// <param name="vehicle">vehicle</param>
		/// <returns>DataSet containing records as (id,visual,idmedia,datenum)</returns>
		public static DataSet GetPressVersionsDetails(ICollection versions, WebSession webSession,DBCst.Vehicles.names vehicle)
		{
            DateTime today = DateTime.Now.Date;
            DateTime begin = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);

            string tableSuffix = string.Empty;
            if (begin >= today.AddDays(1 - today.Day).AddMonths(-3))
            {
                tableSuffix = "_4M";
            }

			switch(vehicle){
				case DBCst.Vehicles.names.tv:
					return GetPressVersionsDetails(versions, webSession, string.Format("{0}{1}",Constantes.DB.Tables.DATA_TV, tableSuffix), Constantes.DB.Schema.ADEXPRESS_SCHEMA,"dt");
				case DBCst.Vehicles.names.radio:
					return GetPressVersionsDetails(versions, webSession, string.Format("{0}{1}",Constantes.DB.Tables.DATA_RADIO, tableSuffix), Constantes.DB.Schema.ADEXPRESS_SCHEMA,"dr");
				case DBCst.Vehicles.names.press:
					return GetPressVersionsDetails(versions, webSession, string.Format("{0}{1}",Constantes.DB.Tables.DATA_PRESS, tableSuffix), Constantes.DB.Schema.ADEXPRESS_SCHEMA,Constantes.DB.Tables.DATA_PRESS_PREFIXE);
				case DBCst.Vehicles.names.internationalPress:
					return GetPressVersionsDetails(versions, webSession, string.Format("{0}{1}",Constantes.DB.Tables.DATA_PRESS_INTER, tableSuffix), Constantes.DB.Schema.ADEXPRESS_SCHEMA,Constantes.DB.Tables.DATA_PRESS_INTER_PREFIXE);
                case DBCst.Vehicles.names.outdoor:
                    return GetOutdoorVersionsDetails(versions, webSession, string.Format("{0}{1}", Constantes.DB.Tables.DATA_OUTDOOR, tableSuffix), Constantes.DB.Schema.ADEXPRESS_SCHEMA, "do");
                case DBCst.Vehicles.names.instore:
                    return GetOutdoorVersionsDetails(versions, webSession, string.Format("{0}{1}", Constantes.DB.Tables.DATA_INSTORE, tableSuffix), Constantes.DB.Schema.ADEXPRESS_SCHEMA, "do");

				default :
					throw new VersionDataAccessException(" Impossible d'indentifier le media (vehicle) à traiter.");
			}
		}
		#endregion

		#region Get Sample of visual for an appm version
		/// <summary>
		/// Get a sample of visual for each version of a list of version
		/// </summary>
		/// <param name="versions">List of verions</param>
		/// <param name="webSession">Customer Session (for datasource)</param>
		/// <returns>DataSet containing records as (id,visual,idmedia,datenum)</returns>
		public static DataSet GetAPPMVersions(ICollection versions, WebSession webSession)
		{ 
			return GetAPPMVersions(versions, webSession, Constantes.DB.Tables.DATA_PRESS_APPM, Constantes.DB.Schema.APPM_SCHEMA,Constantes.DB.Tables.DATA_PRESS_APPM_PREFIXE);
		}
        /// <summary>
        /// Get a sample of visual for each version of a list of version with all media
        /// </summary>
        /// <param name="versions">List of verions</param>
        /// <param name="webSession">Customer Session (for datasource)</param>
        /// <returns>DataSet containing records as (id,idmedia)</returns>
        public static DataSet GetAPPMVersionsAllMedia(ICollection versions, WebSession webSession) {
            return GetAPPMVersionsAllMedia(versions, webSession, Constantes.DB.Tables.DATA_PRESS_APPM, Constantes.DB.Schema.APPM_SCHEMA, Constantes.DB.Tables.DATA_PRESS_APPM_PREFIXE);
        }
		#endregion

		#region Get version  detail
		
		/// <summary>
		/// Obtient le détail produit d'une  version
		/// </summary>
		/// <param name="idVersion">Idenbtifiant de la version</param>
		/// <param name="siteLanguage">Langue du site</param>
		/// <param name="dataSource">Source de données</param>
		/// <returns>Libellés version,produit,groupe,annonceur</returns>
		public static DataSet GetVersion(string idVersion, string siteLanguage,IDataSource dataSource){
			 System.Text.StringBuilder query = new System.Text.StringBuilder(1000);

			 query.Append ("  SELECT id_slogan,ap.product,ap.group_,apa.advertiser ");
			 query.Append ("  FROM "+Constantes.DB.Schema.ADEXPRESS_SCHEMA+".slogan  sl, "+Constantes.DB.Schema.ADEXPRESS_SCHEMA+".ALL_PRODUCT ap , "+Constantes.DB.Schema.ADEXPRESS_SCHEMA+".ALL_PRODUCT_ADVERTISER  apa ");				
			 query.Append ("  WHERE sl.id_slogan="+idVersion);  
			 query.Append ("  and sl.id_product = ap.id_product and apa.id_product = sl.id_product ");
			 query.Append ("  and ap.id_language="+siteLanguage);
			try {
				return dataSource.Fill(query.ToString());
			}
			catch (System.Exception exc) {
				throw new VersionDataAccessException(exc.Message, exc);
			}

		}

		#endregion

		#region Private Methods

		#region Vehicle Versions
		/// <summary>
		/// Get a sample of creations for each version of a list of version for tv or radio
		/// </summary>
		/// <param name="versions">List of verions</param>
		/// <param name="webSession">Customer Session (for datasource)</param>
		/// <param name="dbSchema">Database schema</param>
		/// <param name="dbTbl">Database table</param>
		/// <param name="creationFileFieldName">file creation database field name</param>
		/// <returns>DataSet containing records as (id,visual,idmedia,datenum)</returns>
        private static DataSet GetVersions(ICollection versions, WebSession webSession, string dbTbl, string dbSchema, string creationFileFieldName, MediaSchedulePeriod period) {
			string ids = string.Empty;			
			string query = "";
			string versionCondition="";
			int nbVersion=0;
			bool first=true;
            string dateBegin = "", dateEnd = "";

            #region gestion des dates
            if (period != null) {
                dateBegin = period.Begin.ToString("yyyyMMdd");
                dateEnd = period.End.ToString("yyyyMMdd");
            }
            #endregion

            #region ancienne version
            //			foreach(Object o in versions){
//				ids += o.ToString() + ",";
//			}

//			if (ids.Length > 0){
//				ids = ids.Substring(0, ids.Length-1);
//			}
//			else{
//				return null;
//			}
			#endregion

			#region nouvelle version avec max 999 éléments par requete
			foreach(Object o in versions){

				ids += o.ToString() + ",";
				nbVersion++;

				if(nbVersion==999){//Limitation à 999 versions par condition
					if (ids.Length > 0){
						ids = ids.Substring(0, ids.Length-1);
					}
					if(!first)versionCondition+= " or ";
					versionCondition+= " id_slogan in ( "+ids+" ) ";
					first=false;
					ids=string.Empty;
					nbVersion=0;
				}
				
			}

			if (ids.Length > 0 || versionCondition.Length>0){
				if (ids.Length > 0 ){
					ids = ids.Substring(0, ids.Length-1);
					if(!first)versionCondition+= " or ";
					versionCondition+= " id_slogan in ( "+ids+" ) ";
				}
			}
			else{
				return null;
			}
			#endregion

            query = "SELECT id_slogan as id, " + creationFileFieldName + " as visual, date_media_num as datenum, id_media as idmedia, advertiser as annonceur, product as produit, sector as famille, subsector as classe, group_ as groupe, segment as variete "
               + " FROM ("
               + " SELECT id_slogan , " + creationFileFieldName + ", date_media_num , id_media, dp4.id_advertiser, dp4.id_product, pr.product, ad.advertiser, sc.sector, sb.subsector, gr.group_, sg.segment , ROW_NUMBER()"
               + " OVER ("
               + " PARTITION BY id_slogan ORDER BY id_slogan DESC"
               + " ) Oneligne "
               + " FROM " + dbSchema + "." + dbTbl + " dp4,  " + dbSchema + ".advertiser ad" + ", " + dbSchema + ".product pr" + ", " + dbSchema + ".sector sc"
               + ", " + dbSchema + ".subsector sb" + ", " + dbSchema + ".group_ gr" + ", " + dbSchema + ".segment sg"
               + " WHERE pr.id_product = dp4.id_product and pr.id_language="+webSession.DataLanguage
               + " and ad.id_advertiser = dp4.id_advertiser and ad.id_language="+webSession.DataLanguage
               + " and sc.id_sector = dp4.id_sector and sc.id_language="+webSession.DataLanguage
               + " and sb.id_subsector = dp4.id_subsector and sb.id_language="+webSession.DataLanguage
               + " and gr.id_group_ = dp4.id_group_ and gr.id_language="+webSession.DataLanguage
               + " and sg.id_segment = dp4.id_segment and sg.id_language="+webSession.DataLanguage;

            if (period != null)
                query += "  and date_media_num>=" + dateBegin + " and  date_media_num<=" + dateEnd + "  ";

            query += " )"
				+ " WHERE Oneligne <= 1"
				 #region ancienne version
//				+ " and id_slogan in ("
//				+  ids
				 #endregion
				+ " and  ("
				+  versionCondition
				+ ")";
			
			try{
				return webSession.Source.Fill(query);
			}
			catch (System.Exception exc){
				throw new VersionDataAccessException(exc.Message, exc);
			}

		}
		#endregion

		#region Press Versions
		/// <summary>
		/// Get a sample of visual for each version of a list of version
		/// </summary>
		/// <param name="versions">List of verions</param>
		/// <param name="webSession">Customer Session (for datasource)</param>
		/// <param name="dbSchema">Database schema</param>
		/// <param name="dbTbl">Database table</param>
        /// <param name="period">Période utilisée</param>
		/// <returns>DataSet containing records as (id,visual,idmedia,datenum)</returns>
        private static DataSet GetPressVersions(ICollection versions, WebSession webSession, string dbTbl, string dbSchema, MediaSchedulePeriod period) {

			string ids = string.Empty;			
			string query = "";
			string versionCondition="";
			int nbVersion=0;
			bool first=true;
            string dateBegin = "", dateEnd = "";

            #region gestion des dates
            if (period != null) {
                dateBegin = period.Begin.ToString("yyyyMMdd");
                dateEnd = period.End.ToString("yyyyMMdd");
            }
            #endregion
		
			#region ancienne version
			//			foreach(Object o in versions){
			//				ids += o.ToString() + ",";
			//			}

			//			if (ids.Length > 0){
			//				ids = ids.Substring(0, ids.Length-1);
			//			}
			//			else{
			//				return null;
			//			}
			#endregion

			#region nouvelle version avec max 999 éléments par requete
			foreach(Object o in versions){

				ids += o.ToString() + ",";
				nbVersion++;

				if(nbVersion==999){//Limitation à 999 versions par condition
					if (ids.Length > 0){
						ids = ids.Substring(0, ids.Length-1);
					}
					if(!first)versionCondition+= " or ";
					versionCondition+= " id_slogan in ( "+ids+" ) ";
					first=false;
					ids=string.Empty;
					nbVersion=0;
				}
				
			}

			if (ids.Length > 0 || versionCondition.Length>0){
				if (ids.Length > 0 ){
					ids = ids.Substring(0, ids.Length-1);
					if(!first)versionCondition+= " or ";
					versionCondition+= " id_slogan in ( "+ids+" ) ";
				}
			}
			else{
				return null;
			}
			#endregion

            query = "SELECT id_slogan as id, visual as visual, date_cover_num as dateCover, date_media_num as dateKiosque, id_media as idmedia, advertiser as annonceur, product as produit, sector as famille, subsector as classe, group_ as groupe, segment as variete"
               + " FROM ("
               + " SELECT id_slogan , visual, date_cover_num, date_media_num , id_media, dp4.id_advertiser, dp4.id_product, pr.product, ad.advertiser, sc.sector, sb.subsector, gr.group_, sg.segment, ROW_NUMBER()"
               + " OVER ("
               + " PARTITION BY id_slogan ORDER BY id_slogan DESC"
               + " ) Oneligne "
               + " FROM " + dbSchema + "." + dbTbl + " dp4, " + dbSchema + ".advertiser ad" + ", " + dbSchema + ".product pr" + ", " + dbSchema + ".sector sc"
               + ", " + dbSchema + ".subsector sb" + ", " + dbSchema + ".group_ gr" + ", " + dbSchema + ".segment sg"
               + " WHERE pr.id_product = dp4.id_product and pr.id_language="+webSession.DataLanguage
               + " and ad.id_advertiser = dp4.id_advertiser and ad.id_language="+webSession.DataLanguage
               + " and sc.id_sector = dp4.id_sector and sc.id_language="+webSession.DataLanguage
               + " and sb.id_subsector = dp4.id_subsector and sb.id_language="+webSession.DataLanguage
               + " and gr.id_group_ = dp4.id_group_ and gr.id_language="+webSession.DataLanguage
               + " and sg.id_segment = dp4.id_segment and sg.id_language="+webSession.DataLanguage;

             if (period != null)
                query += "  and date_media_num>=" + dateBegin + " and  date_media_num<=" + dateEnd + "  ";

			query +=  " )"
				+ " WHERE Oneligne <= 1"
				#region ancienne version
				//				+ " and id_slogan in ("
				//				+  ids
				#endregion
				+ " and  ("
				+  versionCondition
				+ ")";

			try{
				return webSession.Source.Fill(query);
			}
			catch (System.Exception exc){
				throw new VersionDataAccessException(exc.Message, exc);
			}

		}
		#endregion

        #region Press Versions All Media
        /// <summary>
        /// Get a sample of visual for each version of a list of version
        /// </summary>
        /// <param name="versions">List of verions</param>
        /// <param name="webSession">Customer Session (for datasource)</param>
        /// <param name="dbSchema">Database schema</param>
        /// <param name="dbTbl">Database table</param>
        /// <param name="period">Période utilisée</param>
        /// <returns>DataSet containing records as (id,visual,idmedia,datenum)</returns>
        private static DataSet GetPressVersionsAllMedia(ICollection versions, WebSession webSession, string dbTbl, string dbSchema, MediaSchedulePeriod period) {

            string ids = string.Empty;
            string query = "";
            string versionCondition = "";
            int nbVersion = 0;
            bool first = true;
            string dateBegin = "", dateEnd = "";

            #region gestion des dates
            if (period != null) {
                dateBegin = period.Begin.ToString("yyyyMMdd");
                dateEnd = period.End.ToString("yyyyMMdd");
            }
            #endregion

            #region nouvelle version avec max 999 éléments par requete
            foreach (Object o in versions) {

                ids += o.ToString() + ",";
                nbVersion++;

                if (nbVersion == 999) {//Limitation à 999 versions par condition
                    if (ids.Length > 0) {
                        ids = ids.Substring(0, ids.Length - 1);
                    }
                    if (!first) versionCondition += " or ";
                    versionCondition += " id_slogan in ( " + ids + " ) ";
                    first = false;
                    ids = string.Empty;
                    nbVersion = 0;
                }

            }

            if (ids.Length > 0 || versionCondition.Length > 0) {
                if (ids.Length > 0) {
                    ids = ids.Substring(0, ids.Length - 1);
                    if (!first) versionCondition += " or ";
                    versionCondition += " id_slogan in ( " + ids + " ) ";
                }
            }
            else {
                return null;
            }
            #endregion

            query = " SELECT distinct id_slogan, id_media "
               + " FROM ("
               + " SELECT id_slogan, id_media "
               + " FROM " + dbSchema + "." + dbTbl + " dp4, " + dbSchema + ".advertiser ad" + ", " + dbSchema + ".product pr" + ", " + dbSchema + ".sector sc"
               + ", " + dbSchema + ".subsector sb" + ", " + dbSchema + ".group_ gr" + ", " + dbSchema + ".segment sg"
               + " WHERE pr.id_product = dp4.id_product and pr.id_language=" + webSession.DataLanguage
               + " and ad.id_advertiser = dp4.id_advertiser and ad.id_language=" + webSession.DataLanguage
               + " and sc.id_sector = dp4.id_sector and sc.id_language=" + webSession.DataLanguage
               + " and sb.id_subsector = dp4.id_subsector and sb.id_language=" + webSession.DataLanguage
               + " and gr.id_group_ = dp4.id_group_ and gr.id_language=" + webSession.DataLanguage
               + " and sg.id_segment = dp4.id_segment and sg.id_language=" + webSession.DataLanguage;

            if (period != null)
                query += "  and date_media_num>=" + dateBegin + " and  date_media_num<=" + dateEnd + "  ";

            query += " )"
                + " WHERE "
                + " ("
                + versionCondition
                + ")"
                + " order by id_slogan ";

            try {
                return webSession.Source.Fill(query);
            }
            catch (System.Exception exc) {
                throw new VersionDataAccessException(exc.Message, exc);
            }

        }
        #endregion

		#region Press Versions Details
		/// <summary>
		/// Get a detail of visual for each version of a list of version
		/// </summary>
		/// <param name="versions">List of verions</param>
		/// <param name="webSession">Customer Session (for datasource)</param>
		/// <param name="dbSchema">Database schema</param>
		/// <param name="dbTbl">Database table</param>
		/// <param name="tablePrefixe">Table Data Press prefixe</param>
		/// <returns>DataSet containing records as (id,visual,idmedia,datenum)</returns>
		private static DataSet GetPressVersionsDetails(ICollection versions, WebSession webSession, string dbTbl, string dbSchema,string tablePrefixe){

			string ids = string.Empty;
			string query = "";
			string versionCondition="";
			int nbVersion=0;
			bool first=true;
		
			#region ancienne version
			//			foreach(Object o in versions){
			//				ids += o.ToString() + ",";
			//			}

			//			if (ids.Length > 0){
			//				ids = ids.Substring(0, ids.Length-1);
			//			}
			//			else{
			//				return null;
			//			}
			#endregion

			#region nouvelle version avec max 999 éléments par requete
			foreach(Object o in versions){

				ids += o.ToString() + ",";
				nbVersion++;

				if(nbVersion==999){//Limitation à 999 versions par condition
					if (ids.Length > 0){
						ids = ids.Substring(0, ids.Length-1);
					}
					if(!first)versionCondition+= " or ";
					versionCondition+= " id_slogan in ( "+ids+" ) ";
					first=false;
					ids=string.Empty;
					nbVersion=0;
				}
				
			}

			if (ids.Length > 0 || versionCondition.Length>0){
				if (ids.Length > 0 ){
					ids = ids.Substring(0, ids.Length-1);
					if(!first)versionCondition+= " or ";
					versionCondition+= " id_slogan in ( "+ids+" ) ";
				}
			}
			else{
				return null;
			}
			#endregion

			#region Dates Parameters
			//Formatting date to be used in the query
			string dateBegin = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd");
			string dateEnd = WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd");
			#endregion

            query = "SELECT id_slogan as id, min(date_media_num) as datenum, count(distinct ID_MEDIA) as nbsupports"
                + ", sum(" + UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField + ") as " + UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString() 
                + ", sum(" + UnitsInformation.List[CustomerSessions.Unit.insertion].DatabaseField + ") as " + UnitsInformation.List[CustomerSessions.Unit.insertion].Id.ToString()
				+ " FROM " + dbSchema + "." + dbTbl + " " +tablePrefixe
				+ " WHERE "
				+"  date_media_num>="+dateBegin+" and  date_media_num<="+dateEnd+"  " 
				#region ancienne version
				//				+ " and id_slogan in ("
				//				+  ids
				#endregion
				+ " and  ("
				+  versionCondition
				+ ")  "				
				//Product Right
				+ WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,tablePrefixe,true)
				//media rights
				+"  "+ WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,tablePrefixe, true)
				+ "group by  ID_SLOGAN";

			try {
				return webSession.Source.Fill(query);
			}
			catch (System.Exception exc) {
				throw new VersionDataAccessException(exc.Message, exc);
			}

		}
		#endregion

		#region APPM Press Versions
		/// <summary>
		/// Get a sample of visual for each version of a list of version
		/// </summary>
		/// <param name="versions">List of verions</param>
		/// <param name="webSession">Customer Session (for datasource)</param>
		/// <param name="dbSchema">Database schema</param>
		/// <param name="dbTbl">Database table</param>
		/// <param name="tablePrefixe">Table APPM prefixe</param>
		/// <returns>DataSet containing records as (id,visual,idmedia,datenum)</returns>
		private static DataSet GetAPPMVersions(ICollection versions, WebSession webSession, string dbTbl, string dbSchema,string tablePrefixe)
		{

			string ids = string.Empty;			
			string query = "";
			string versionCondition="";
			int nbVersion=0;
			bool first=true;
		
			#region ancienne version
			//			foreach(Object o in versions){
			//				ids += o.ToString() + ",";
			//			}

			//			if (ids.Length > 0){
			//				ids = ids.Substring(0, ids.Length-1);
			//			}
			//			else{
			//				return null;
			//			}
			#endregion

			#region nouvelle version avec max 999 éléments par requete
			foreach(Object o in versions){

				ids += o.ToString() + ",";
				nbVersion++;

				if(nbVersion==999){//Limitation à 999 versions par condition
					if (ids.Length > 0){
						ids = ids.Substring(0, ids.Length-1);
					}
					if(!first)versionCondition+= " or ";
					versionCondition+= " id_slogan in ( "+ids+" ) ";
					first=false;
					ids=string.Empty;
					nbVersion=0;
				}
				
			}

			if (ids.Length > 0 || versionCondition.Length>0){
				if (ids.Length > 0 ){
					ids = ids.Substring(0, ids.Length-1);
					if(!first)versionCondition+= " or ";
					versionCondition+= " id_slogan in ( "+ids+" ) ";
				}
			}
			else{
				return null;
			}
			#endregion


			#region Dates Parameters
			//Formatting date to be used in the query
			string dateBegin = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd");
			string dateEnd = WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd");
			#endregion

			//additional target
            Int64 idAdditionalTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmTargetAccess));

            query = "SELECT id_slogan as id, visual as visual,date_media_num as dateKiosque,date_cover_num as dateCover, id_media as idmedia"
				+ " FROM ("
				+ " SELECT id_slogan , visual, date_media_num ,date_cover_num, id_media, ROW_NUMBER()"
				+ " OVER ("
				+ " PARTITION BY id_slogan ORDER BY id_slogan , date_media_num ASC"
				+ " ) Oneligne " 
				+ " FROM " + dbSchema + "." + dbTbl+"  "+tablePrefixe
				+" , "+ dbSchema+"."+Constantes.DB.Tables.TARGET_MEDIA_ASSIGNEMNT+" "+Constantes.DB.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE
				+ " WHERE " + Constantes.DB.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ".id_media_secodip = id_media "
		
				// Period selected								
				+" and  date_media_num>="+dateBegin+" and  date_media_num<="+dateEnd+"  "
						
				//Product Right
				+ WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,tablePrefixe,true)

				// Products to exclude
				+" "+WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,tablePrefixe,true,false)			

				//product selection
                + "  " + ((webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0) ? webSession.PrincipalProductUniverses[0].GetSqlConditions(tablePrefixe, true) : "")
				
				//on one target
				+" and "+Constantes.DB.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_target in("+idAdditionalTarget+") "
 			
				//outside encart
				+" and "+tablePrefixe+".id_inset is null "

				//media rights
				+"  "+ WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,tablePrefixe, true)

				+ " )"
				+ " WHERE Oneligne <= 1"
				#region ancienne version
				//				+ " and id_slogan in ("
				//				+  ids
				#endregion
				+ " and  ("
				+  versionCondition
				+ ")";

				
			try{
				return webSession.Source.Fill(query);
			}
			catch (System.Exception exc){
				throw new VersionDataAccessException(exc.Message, exc);
			}

		}
		#endregion

        #region APPM Press Versions with all media
        /// <summary>
        /// Get a sample of visual for each version of a list of version with all media
        /// </summary>
        /// <param name="versions">List of verions</param>
        /// <param name="webSession">Customer Session (for datasource)</param>
        /// <param name="dbSchema">Database schema</param>
        /// <param name="dbTbl">Database table</param>
        /// <param name="tablePrefixe">Table APPM prefixe</param>
        /// <returns>DataSet containing records as (id,visual,idmedia,datenum)</returns>
        private static DataSet GetAPPMVersionsAllMedia(ICollection versions, WebSession webSession, string dbTbl, string dbSchema, string tablePrefixe) {

            string ids = string.Empty;
            string query = "";
            string versionCondition = "";
            int nbVersion = 0;
            bool first = true;

            #region nouvelle version avec max 999 éléments par requete
            foreach (Object o in versions) {

                ids += o.ToString() + ",";
                nbVersion++;

                if (nbVersion == 999) {//Limitation à 999 versions par condition
                    if (ids.Length > 0) {
                        ids = ids.Substring(0, ids.Length - 1);
                    }
                    if (!first) versionCondition += " or ";
                    versionCondition += " id_slogan in ( " + ids + " ) ";
                    first = false;
                    ids = string.Empty;
                    nbVersion = 0;
                }

            }

            if (ids.Length > 0 || versionCondition.Length > 0) {
                if (ids.Length > 0) {
                    ids = ids.Substring(0, ids.Length - 1);
                    if (!first) versionCondition += " or ";
                    versionCondition += " id_slogan in ( " + ids + " ) ";
                }
            }
            else {
                return null;
            }
            #endregion


            #region Dates Parameters
            //Formatting date to be used in the query
            string dateBegin = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd");
            string dateEnd = WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd");
            #endregion

            //additional target
            Int64 idAdditionalTarget = Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget, CustomerCst.Right.type.aepmTargetAccess));

            query = " SELECT distinct id_slogan, id_media"
                + " FROM ("
                + " SELECT id_slogan, id_media"
                + " FROM " + dbSchema + "." + dbTbl + "  " + tablePrefixe
                + " , " + dbSchema + "." + Constantes.DB.Tables.TARGET_MEDIA_ASSIGNEMNT + " " + Constantes.DB.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE
                + " WHERE " + Constantes.DB.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ".id_media_secodip = id_media "

                // Period selected								
                + " and  date_media_num>=" + dateBegin + " and  date_media_num<=" + dateEnd + "  "

                //Product Right
                + WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession, tablePrefixe, true)

                // Products to exclude
                + " " + WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, tablePrefixe, true, false)

                //product selection
                + "  " + ((webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0) ? webSession.PrincipalProductUniverses[0].GetSqlConditions(tablePrefixe, true) : "")

                //on one target
                + " and " + Constantes.DB.Tables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ".id_target in(" + idAdditionalTarget + ") "

                //outside encart
                + " and " + tablePrefixe + ".id_inset is null "

                //media rights
                + "  " + WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession, tablePrefixe, true)

                + " )"
                + " WHERE "
                + " ("
                + versionCondition
                + ")"
                + " order by id_slogan ";


            try {
                return webSession.Source.Fill(query);
            }
            catch (System.Exception exc) {
                throw new VersionDataAccessException(exc.Message, exc);
            }

        }
        #endregion

        #region Outdoor Versions Details
        /// <summary>
        /// Get a detail of visual for each version of a list of version
        /// </summary>
        /// <param name="versions">List of verions</param>
        /// <param name="webSession">Customer Session (for datasource)</param>
        /// <param name="dbSchema">Database schema</param>
        /// <param name="dbTbl">Database table</param>
        /// <param name="tablePrefixe">Table Data Press prefixe</param>
        /// <returns>DataSet containing records as (id,visual,idmedia,datenum)</returns>
        private static DataSet GetOutdoorVersionsDetails(ICollection versions, WebSession webSession, string dbTbl, string dbSchema, string tablePrefixe) {

            string ids = string.Empty;
            string query = "";
            string versionCondition = "";
            int nbVersion = 0;
            bool first = true;

            #region nouvelle version avec max 999 éléments par requete
            foreach (Object o in versions) {

                ids += o.ToString() + ",";
                nbVersion++;

                if (nbVersion == 999) {//Limitation à 999 versions par condition
                    if (ids.Length > 0) {
                        ids = ids.Substring(0, ids.Length - 1);
                    }
                    if (!first) versionCondition += " or ";
                    versionCondition += " id_slogan in ( " + ids + " ) ";
                    first = false;
                    ids = string.Empty;
                    nbVersion = 0;
                }

            }

            if (ids.Length > 0 || versionCondition.Length > 0) {
                if (ids.Length > 0) {
                    ids = ids.Substring(0, ids.Length - 1);
                    if (!first) versionCondition += " or ";
                    versionCondition += " id_slogan in ( " + ids + " ) ";
                }
            }
            else {
                return null;
            }
            #endregion

            #region Dates Parameters
            //Formatting date to be used in the query
            string dateBegin = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd");
            string dateEnd = WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd");
            #endregion

            query = "SELECT id_slogan as id, count(distinct id_media) as nbsupports"
               + ", sum(" + UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField + ") as " + UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()
               + ", sum(" + UnitsInformation.List[CustomerSessions.Unit.numberBoard].DatabaseField + ") as " + UnitsInformation.List[CustomerSessions.Unit.numberBoard].Id.ToString()
               + " FROM " + dbSchema + "." + dbTbl + " " + tablePrefixe
               + " WHERE "
               + "  date_media_num>=" + dateBegin + " and  date_media_num<=" + dateEnd + "  "
               + " and  ("
               + versionCondition
               + ")  "
                //Product Right
               + WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession, tablePrefixe, true)
                //media rights
               + "  " + WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession, tablePrefixe, true)
               + "group by  ID_SLOGAN";

            try {
                return webSession.Source.Fill(query);
            }
            catch (System.Exception exc) {
                throw new VersionDataAccessException(exc.Message, exc);
            }

        }
        #endregion

		#endregion

	}
}
