#region Informations
// Auteur:
// Date de création:
// Date de modification: 
//	22/08/2005	G. Facon		Solution temporaire pour les IDataSource
//	10/11/2005	D. V. Mussuma	Utilisation de IDataSource depuis WebSession
//	25/11/2005	B.Masson		webSession.Source
#endregion

#region Using
using System;
using System.Text;
using System.Data;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Functions;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using CstProject = TNS.AdExpress.Constantes.Project;
using TNS.FrameWork.DB.Common;
#endregion

namespace TNS.AdExpress.Web.DataAccess.Results{
	/// <summary>
	///  Extrait le détail des insertions publicitaires dans un support, une catégorie, un média correspondant
	/// à la sélection utilisateur (nomenclatures produits et média, période) présente dans une session pour les médias
	/// concurentiel
	/// </summary>
	public class CompetitorMediaInsertionsCreationDataAccess{				

		#region Méthodes publiques (Nouvelle version)
		/// <summary>
		/// Extrait le détail des insertions publicitaires dans un support, une catégorie, un média correspondant
		/// à la sélection utilisateur (nomenclatures produits et média, période) présente dans une session:
		///		Extraction de la table attaquée et des champs de sélection à partir du vehicle
		///		Construction et exécution de la requête
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="idVehicle">Identifiant du vehicle</param>
		/// <param name="idCategory">Identifiant Catégorie</param>
		/// <param name="idMedia">Identifiant du media</param>
		/// <param name="dateBegin">Date de début</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <param name="idAdvertiser">Identifiant annonceur</param>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
		/// Lancée quand on ne sait pas quelle table attaquer, quels champs sélectionner ou quand une erreur Orace s'est produite
		/// </exception>
		/// <returns>DataSet contenant le résultat de la requête</returns>
		/// <remarks>
		/// Utilise les méthodes:
		///		private static string GetTable(DBClassificationConstantes.Vehicles.names idVehicle)
		///		private static string GetFields(DBClassificationConstantes.Vehicles.names idVehicle)
		///		private static string GetOrder(DBClassificationConstantes.Vehicles.names idVehicle)
		/// </remarks>
		public static DataSet GetData(WebSession webSession, Int64 idVehicle, Int64 idCategory, Int64 idMedia, Int64 idAdvertiser, int dateBegin, int dateEnd) {

			#region Variables
			string tableName = "";
			string fields = "";
			//string levelProduct="";
			string list = "";
			bool premier = true;
			bool premierUnivers = true;
			int positionUnivers = 0;
			StringBuilder sql = new StringBuilder(500);
			#endregion

			try {
				tableName = GetTable((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));
				fields = GetFields((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));
			}
			catch (System.Exception ex) {
				throw new MediaCreationDataAccessException("GetData::Impossible d'extrait le détail des insertions publicitaires", ex);
			}
			sql.Append("select " + GetFields((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()),true) + " from ( ");
            int nbUnivers = 0;
			while (webSession.PrincipalProductUniverses.ContainsKey(positionUnivers) && webSession.PrincipalProductUniverses[positionUnivers] != null) {
                if (positionUnivers == (idAdvertiser - 1) || idAdvertiser < 0)
                {
                    if (nbUnivers > 0)
                    {
                        sql.Append(" UNION ");
                    }

                    #region Construction de la requête

                    // Sélection de la nomenclature Support
                    sql.Append("select " + fields);
                    // Tables
                    sql.Append(" from " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".media md, ");
                    sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.SEGMENT + " sg, ");
                    sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.GROUP_ + " gp, ");
                    sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + ".advertiser ad, ");
                    sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + ".product pr, ");
                    sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + ".category ct, ");
                    sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + ".vehicle ve, ");
                    sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + tableName + " wp ");
                    if ((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()) == DBClassificationConstantes.Vehicles.names.outdoor)
                    {
                        sql.Append("," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".agglomeration ag ");

                    }
                    // Tables additionneles si le vehicle considere est la presse
                    // A changer pour inter si le nom de la table est différent
                    if (tableName.CompareTo(DBConstantes.Tables.DATA_PRESS) == 0)
                    {
                        sql.Append(", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.COLOR + " co, ");
                        sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.LOCATION + " lo, ");
                        sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.DATA_LOCATION + "  dl, ");
                        sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.FORMAT + " fo, ");
                        sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.APPLICATION_MEDIA + " am ");
                    }
                    // Conditions de jointure
                    sql.Append(" Where md.id_media=wp.id_media ");
                    sql.Append(" and pr.id_product=wp.id_product ");
                    sql.Append(" and ad.id_advertiser=wp.id_advertiser ");
                    sql.Append(" and sg.id_segment=wp.id_segment ");
                    sql.Append(" and gp.id_group_=wp.id_group_ ");
                    sql.Append(" and ct.id_category=wp.id_category");
                    sql.Append(" and ve.id_vehicle=wp.id_vehicle");
                    // A changer pour inter si le nom de la table est différent
                    if (tableName.CompareTo(DBConstantes.Tables.DATA_PRESS) == 0)
                    {
                        sql.Append(" and (am.id_media(+) = wp.id_media ");
                        //	sql.Append(" and am.id_language_data_i(+) = wp.id_language_data_i ");
                        sql.Append(" and am.date_debut(+) = wp.date_media_num ");
                        sql.Append(" and am.id_project(+) = " + CstProject.ADEXPRESS_ID + ") ");
                        sql.Append(" and co.id_color (+)=wp.id_color ");
                        sql.Append(" and lo.id_location (+)=dl.id_location ");
                        sql.Append(" and dl.id_media (+)=wp.id_media ");
                        sql.Append(" and dl.date_media_num (+)=wp.date_media_num ");
                        sql.Append(" and dl.id_advertisement (+)=wp.id_advertisement ");
                        sql.Append(" and co.id_color (+)=wp.id_color ");
                        sql.Append(" and fo.id_format (+)=wp.id_format ");
                    }
                    if ((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()) == DBClassificationConstantes.Vehicles.names.outdoor)
                    {
                        sql.Append(" and ag.id_agglomeration (+)= wp.id_agglomeration ");
                        sql.Append(" and ag.id_language (+)= " + webSession.SiteLanguage.ToString());
                        sql.Append(" and ag.activation (+)<" + DBConstantes.ActivationValues.UNACTIVATED);
                    }
                    //langages
                    sql.Append(" and md.id_language=" + webSession.SiteLanguage.ToString());
                    sql.Append(" and sg.id_language=" + webSession.SiteLanguage.ToString());
                    sql.Append(" and gp.id_language=" + webSession.SiteLanguage.ToString());
                    sql.Append(" and ad.id_language=" + webSession.SiteLanguage.ToString());
                    sql.Append(" and pr.id_language=" + webSession.SiteLanguage.ToString());
                    sql.Append(" and ct.id_language=" + webSession.SiteLanguage.ToString());
                    sql.Append(" and ve.id_language=" + webSession.SiteLanguage.ToString());
                    // A changer pour inter si le nom de la table est différent
                    if (tableName.CompareTo(DBConstantes.Tables.DATA_PRESS) == 0)
                    {
                        sql.Append(" and co.id_language (+)=" + webSession.SiteLanguage.ToString());
                        sql.Append(" and lo.id_language (+)=" + webSession.SiteLanguage.ToString());
                        sql.Append(" and fo.id_language (+)=" + webSession.SiteLanguage.ToString());
                    }
                    // Période
                    sql.Append(" and wp.date_media_num>=" + dateBegin);
                    sql.Append(" and wp.date_media_num<=" + dateEnd);
                    // Gestion des sélections et des droits

                    #region Nomenclature Produit (droits)
                    premier = true;
                    //Droits en accès
                    sql.Append(SQLGenerator.getAnalyseCustomerProductRight(webSession, "wp", true));
                    // Produit à exclure en radio
                    sql.Append(SQLGenerator.getAdExpressProductUniverseCondition(TNS.AdExpress.Constantes.Web.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, "wp", true, false));
                    // Produit à exclure en radio
                    #endregion

                    #region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et sélection)

                    #region Sélection
                    // Sélection de Produits
                    if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0
                        && webSession.PrincipalProductUniverses[positionUnivers] != null)
                        sql.Append(webSession.PrincipalProductUniverses[positionUnivers].GetSqlConditions("wp", true));

                    #endregion

                    #endregion

                    #region Nomenclature Media (droits et sélection)

                    #region Droits
                    sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(webSession, "wp", true));
                    #endregion

                    #region Sélection
                    list = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess);
                    if (list.Length > 0)
                    {
                        sql.Append(" and ((wp.id_category<>35)) ");
                        if (idMedia > -1) sql.Append(" and ((wp.id_media=" + idMedia.ToString() + ")) ");
                        else if (idCategory > 1) sql.Append(" and ((wp.id_category=" + idCategory.ToString() + ")) ");
                        else sql.Append(" and ((wp.id_vehicle=" + idVehicle.ToString() + ")) ");
                    }
                    #endregion

                    #endregion

                    // Ordre
                    //sql.Append("Order by " + GetOrder((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())));

                    //Goup by
                    sql.Append("Group by " + fields + " ");
                    #endregion

                    nbUnivers++;
                }
				positionUnivers++;
			}
			// Ordre
			sql.Append(" ) Order by " + GetOrder((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()),true));
			#region Execution de la requête
			try {
				return webSession.Source.Fill(sql.ToString());
			}
			catch (System.Exception err) {
				throw (new MediaCreationDataAccessException("Impossible de charger les données de l'alerte concurrentielle" + sql, err));
			}
			#endregion

		}
		#endregion

		#region Méthodes privées
		/// <summary>
		/// Donne la table à utiliser pour le vehicle indiqué
		/// </summary>
		/// <param name="idVehicle">Identifiant du vehicle</param>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
		/// Lancée quand le cas du vehicle spécifié n'est pas traité
		/// </exception>
		/// <returns>Chaine contenant le nom de la table correspondante</returns>
		private static string GetTable(DBClassificationConstantes.Vehicles.names idVehicle){
			switch(idVehicle){
				case DBClassificationConstantes.Vehicles.names.press:
					return DBConstantes.Tables.DATA_PRESS;
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					return DBConstantes.Tables.DATA_PRESS_INTER;
				case DBClassificationConstantes.Vehicles.names.radio:
					return DBConstantes.Tables.DATA_RADIO;
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					return DBConstantes.Tables.DATA_TV;
				case DBClassificationConstantes.Vehicles.names.outdoor:
					return DBConstantes.Tables.DATA_OUTDOOR;
				default:
					throw new Exceptions.MediaCreationDataAccessException("GetTable(DBClassificationConstantes.Vehicles.value idMedia)-->Le cas de ce média n'est pas gérer. Pas de table correspondante.");
			}
		}

		/// <summary>
		/// Donne les champs à utiliser pour le vehicle indiqué
		/// </summary>
		/// <param name="idVehicle">Identifiant du vehicle</param>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
		/// Lancée quand le cas du vehicle spécifié n'est pas traité
		/// </exception>
		/// <returns>Chaine contenant les champs sélectionnés</returns>
		private static string GetFields(DBClassificationConstantes.Vehicles.names idVehicle) {
			return GetFields(idVehicle, false);
		}

		/// <summary>
		/// Donne les champs à utiliser pour le vehicle indiqué
		/// </summary>
		/// <param name="idVehicle">Identifiant du vehicle</param>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
		/// Lancée quand le cas du vehicle spécifié n'est pas traité
		/// </exception>
		/// <returns>Chaine contenant les champs sélectionnés</returns>
		private static string GetFields(DBClassificationConstantes.Vehicles.names idVehicle,bool withoutPrefix){
			string prefix = (withoutPrefix) ? "" : "wp.";
            string prefix1 = (withoutPrefix) ? "" : "am.";
            string prefix2 = (withoutPrefix) ? "" : "ag.";
            switch (idVehicle)
            {
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					return "media"
						+ ", "+prefix+"date_media_Num"
						+", "+prefix+"media_paging"
						+", group_"
						+", advertiser"
						+", product"
						+", format"
						+", "+prefix+"area_page"
						+", color"
						+", "+prefix+"expenditure_euro"
						+", location"
						+", "+prefix+"visual "
						+", "+prefix+"id_advertisement"
                        + ", " + prefix1 + "disponibility_visual"
                        + ", " + prefix1 + "activation"
						+", category"
						+", vehicle"
						+", "+prefix+"id_media"
						+", "+prefix+"date_cover_num";

				case DBClassificationConstantes.Vehicles.names.radio:
					return "media"
						+", "+prefix+"date_media_num"
						+", "+prefix+"id_top_diffusion"
						+", "+prefix+"associated_file"
						+", advertiser"
						+", product"
						+", group_"
						+", "+prefix+"duration"
						+", "+prefix+"rank"
						+", "+prefix+"duration_commercial_break"
						+", "+prefix+"number_spot_com_break"
						+", "+prefix+"rank_wap"
						+", "+prefix+"duration_com_break_wap"
						+", "+prefix+"number_spot_com_break_wap"
						+", "+prefix+"expenditure_euro"
						+", "+prefix+"id_cobranding_advertiser"
						+", category"
						+", vehicle"
						+", "+prefix+"id_slogan";
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					return  "media"
						+", "+prefix+"date_media_num"
						+", "+prefix+"top_diffusion"
						+", "+prefix+"associated_file"
						+", advertiser"
						+", product"
						+", group_"
						+", "+prefix+"duration"
						+", "+prefix+"id_rank"
						+", "+prefix+"duration_commercial_break"
						+", "+prefix+"number_message_commercial_brea"
						+", "+prefix+"expenditure_euro"
						+", "+prefix+"id_commercial_break"
						+", category"
						+", vehicle";
				case DBClassificationConstantes.Vehicles.names.outdoor:
					return "media"
						+", "+prefix+"date_media_num"						
						+", advertiser"
						+", product"
						+", group_"
						+", "+prefix+"number_board"
						+", "+prefix+"type_board"
						+", "+prefix+"type_sale"
						+", "+prefix+"poster_network"
                        + ", " + prefix2 + "agglomeration"											
						+", "+prefix+"expenditure_euro"
						+ ", "+prefix+"associated_file"
						+", category"
						+", vehicle";
					
				default:
					throw new Exceptions.MediaCreationDataAccessException("GetTable(DBClassificationConstantes.Vehicles.value idMedia)-->Le cas de ce média n'est pas gérer. Pas de table correspondante.");
			}
		}

		/// <summary>
		/// Donne l'ordre de tri des enregistrements extraits
		/// </summary>
		/// <param name="idVehicle">Identifiant du vehicle</param>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
		/// Lancée quand le cas du vehicle spécifié n'est pas traité
		/// </exception>
		/// <returns>Chaine contenant les champs de tris</returns>
		private static string GetOrder(DBClassificationConstantes.Vehicles.names idVehicle) {
			return GetOrder(idVehicle,false);
		}

		/// <summary>
		/// Donne l'ordre de tri des enregistrements extraits
		/// </summary>
		/// <param name="idVehicle">Identifiant du vehicle</param>
		/// <param name="withoutPrefix">avec prefixe</param>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
		/// Lancée quand le cas du vehicle spécifié n'est pas traité
		/// </exception>
		/// <returns>Chaine contenant les champs de tris</returns>
		private static string GetOrder(DBClassificationConstantes.Vehicles.names idVehicle, bool withoutPrefix) {
			string prefix = (withoutPrefix) ? "" : "wp.";
			switch(idVehicle){
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					return "category"
						+", media"
						+", "+prefix+"date_media_Num"
						+", "+prefix+"id_advertisement"
						+", "+prefix+"media_paging"
						+", location";
				case DBClassificationConstantes.Vehicles.names.radio:
					return "category"
						+", media"
						+", "+prefix+"date_media_num"
						+", "+prefix+"id_top_diffusion"
						+", "+prefix+"id_cobranding_advertiser";
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					return  "category"
						+", media"
						+", "+prefix+"date_media_num"
						+", "+prefix+"id_commercial_break"
						+", "+prefix+"id_rank";
				case DBClassificationConstantes.Vehicles.names.outdoor:

					return  "category"
						+", media"
						+", "+prefix+"date_media_num"
						+", "+prefix+"number_board";
				default:
					throw new Exceptions.MediaCreationDataAccessException("GetTable(DBClassificationConstantes.Vehicles.value idMedia)-->Le cas de ce média n'est pas gérer. Pas de table correspondante.");
			}
		}

		#endregion

	}
}
