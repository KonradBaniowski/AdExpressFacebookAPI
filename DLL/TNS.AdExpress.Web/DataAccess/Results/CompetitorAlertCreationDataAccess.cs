#region Informations
// Auteur: A. Obermeyer
// Date de création: 24/09/2004
// Date de modification: 
//	19/05/2005	K. Shehzad		new product detail levels
//	06/07/2005	K. Shehzad		Addition of Agglomeration colunm for Outdoor creations 
//	23/08/2005	G. Facon		Solution temporaire pour les IDataSource
//	10/11/2005	D. V. Mussuma	Utilisation de IDataSource depuis WebSession
//	25/11/2005	B.Masson		webSession.Source
//	19/09/2006	D. V. Mussuma	Ajout du niveau de détail produit
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
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using CstProject = TNS.AdExpress.Constantes.Project;
using ResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results.CompetitorAlert;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Classification;
#endregion

namespace TNS.AdExpress.Web.DataAccess.Results{

	/// <summary>
	/// Base de données Affichier les création
	/// </summary>
	public class CompetitorAlertCreationDataAccess{

		#region Méthodes publiques
		/// <summary>
		/// Extrait le détail des insertions publicitaires dans un support, une catégorie, un média correspondant
		/// à la sélection utilisateur (nomenclatures produits et média, période) présente dans une session:
		///		Extraction de la table attaquée et des champs de sélection à partir du vehicle
		///		Construction et exécution de la requête
		///		
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="idVehicle">Identifiant du vehicle</param>
		/// <param name="dateBegin">Date de début</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <param name="idElement">Identifiant de l'élément sélectioné?</param>
		/// <param name="level">Niveau de idElement?</param>
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
		public static DataSet GetData(WebSession webSession,Int64 idVehicle,Int64 idElement,int level,int dateBegin,int dateEnd){
			string tableName="";
			string fields = "";
			string element="";
			string list="";
			bool premier = true;
			string listMediaAccess="";
			int positionUnivers=1;
            Module currentModuleDescription = ModulesList.GetModule(webSession.CurrentModule);

			#region Sélection de Médias
            if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ALERTE_PORTEFEUILLE ||
                webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE)
            {
                listMediaAccess += webSession.GetSelection((TreeNode)webSession.ReferenceUniversMedia, CustomerRightConstante.type.mediaAccess) + ",";
            }
            if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE)
            {
                while (webSession.CompetitorUniversMedia[positionUnivers] != null)
                {
                    listMediaAccess += webSession.GetSelection((TreeNode)webSession.CompetitorUniversMedia[positionUnivers], CustomerRightConstante.type.mediaAccess) + ",";
                    positionUnivers++;
                }
            }
            if (listMediaAccess.Length > 0) listMediaAccess = listMediaAccess.Substring(0, listMediaAccess.Length - 1);

			#endregion

			try{
				//tableName = GetTable((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()),webSession.CurrentModule);
                if(currentModuleDescription.ModuleType == WebConstantes.Module.Type.tvSponsorship)
                    tableName = SQLGenerator.GetVehicleTableNameForDetailResult(VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())), WebConstantes.Module.Type.tvSponsorship, webSession.IsSelectRetailerDisplay);
                else if (Dates.Is4M(dateBegin))
                    tableName = SQLGenerator.GetVehicleTableNameForDetailResult(VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())), WebConstantes.Module.Type.alert, webSession.IsSelectRetailerDisplay);
                else
                    tableName = SQLGenerator.GetVehicleTableNameForDetailResult(VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())), WebConstantes.Module.Type.analysis, webSession.IsSelectRetailerDisplay);
				fields = GetFields(VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())));
				element=GetIdElement(webSession,idElement,level,idVehicle);
			}
			catch(System.Exception ex){
				throw new MediaCreationDataAccessException("GetData::Impossible d'extrait le détail des insertions publicitaires ",ex);
			}

			#region Données des insertions

			#region Construction de la requête
			StringBuilder sql=new StringBuilder(500);
			// Sélection de la nomenclature Support
			sql.Append("select distinct " + fields);
			// Tables
			sql.Append(" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".media md, ");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.SEGMENT+" sg, ");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.GROUP_+" gp, ");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA+".advertiser ad, ");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA+".product pr, ");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA+".category ct, ");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA+".vehicle ve, ");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName+" wp, ");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA+".product_group_adver_agency pga ");
			if(VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString()))==DBClassificationConstantes.Vehicles.names.outdoor
                || VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())) == DBClassificationConstantes.Vehicles.names.instore)
			{
				sql.Append("," +DBConstantes.Schema.ADEXPRESS_SCHEMA+".agglomeration ag ");
			
			}
			// Tables additionneles si le vehicle considere est la presse et presse inter
			// A changer pour inter si le nom de la table est différent
			if (tableName.CompareTo(DBConstantes.Tables.ALERT_DATA_PRESS)==0
                || tableName.CompareTo(DBConstantes.Tables.DATA_PRESS) == 0) {
				sql.Append(", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.COLOR+" co, ");
				sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.LOCATION+" lo, ");
				sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.DATA_LOCATION+"  dl, ");
				sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.FORMAT+" fo, ");
				sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.APPLICATION_MEDIA+" am ");
			}
			// Conditions de jointure
			sql.Append(" Where md.id_media=wp.id_media ");
			sql.Append(" and pr.id_product=wp.id_product ");
			sql.Append(" and ad.id_advertiser=wp.id_advertiser ");
			sql.Append(" and sg.id_segment=wp.id_segment ");
			sql.Append(" and gp.id_group_=wp.id_group_ ");
			sql.Append(" and ct.id_category=wp.id_category");
			sql.Append(" and ve.id_vehicle=wp.id_vehicle");
			sql.Append(" and pga.id_product(+)=wp.id_product");
			// A changer pour inter si le nom de la table est différent
			if (tableName.CompareTo(DBConstantes.Tables.ALERT_DATA_PRESS)==0
                || tableName.CompareTo(DBConstantes.Tables.DATA_PRESS) == 0) {
				sql.Append(" and (am.id_media(+) = wp.id_media ");
				//sql.Append(" and am.id_language_data_i(+) = wp.id_language_data_i ");
                sql.Append(" and am.date_debut(+) = wp.date_media_num ");
				sql.Append(" and am.id_project(+) = "+ CstProject.ADEXPRESS_ID +") ");
				sql.Append(" and lo.id_location (+)=dl.id_location ");
				sql.Append(" and lo.id_language (+)="+webSession.DataLanguage.ToString());

				sql.Append(" and dl.id_media (+)=wp.id_media ");
                sql.Append(" and dl.date_media_num (+)=wp.date_media_num ");
				sql.Append(" and dl.id_advertisement (+)=wp.id_advertisement ");
				sql.Append(" and co.id_color (+)=wp.id_color ");
				sql.Append(" and fo.id_format (+)=wp.id_format ");
			}
			if(VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString()))==DBClassificationConstantes.Vehicles.names.outdoor
                || VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())) == DBClassificationConstantes.Vehicles.names.instore)
			{
				sql.Append(" and ag.id_agglomeration (+)= wp.id_agglomeration ");
				sql.Append(" and ag.id_language (+)= "+webSession.DataLanguage.ToString());
				sql.Append(" and ag.activation (+)<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			}
			//langages
			sql.Append(" and md.id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and sg.id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and gp.id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and ad.id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and pr.id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and ct.id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and ve.id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and pga.id_language(+)="+webSession.DataLanguage.ToString());
			sql.Append("");
			
			//activation
			sql.Append(" and md.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and sg.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and gp.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and ad.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and pr.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and ct.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and ve.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			
			// A changer pour inter si le nom de la table est différent
			if (tableName.CompareTo(DBConstantes.Tables.DATA_PRESS)==0){
				sql.Append(" and co.id_language (+)="+webSession.DataLanguage.ToString());
				sql.Append(" and lo.id_language (+)="+webSession.DataLanguage.ToString());
				sql.Append(" and fo.id_language (+)="+webSession.DataLanguage.ToString());
			//activation   
				sql.Append(" and co.activation <"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
				sql.Append(" and fo.activation <"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			}
			// Période
			sql.Append(" and wp.date_media_num>="+dateBegin);
			sql.Append(" and wp.date_media_num<="+dateEnd);
			// Gestion des sélections et des droits

			#region Nomenclature Produit (droits)
			premier=true;
			//Droits en accès
			sql.Append(SQLGenerator.getAnalyseCustomerProductRight(webSession,"wp",true));
			//produits à exclure dans les alertes et analyse (radio)
			sql.Append(SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,"wp",true,false));
			#endregion

			// Niveau de media
			sql.Append(SQLGenerator.getLevelMedia(webSession,"wp",true));

			#region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et sélection) 

            #region Sélection
            // Sélection de Produits
            if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
                sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions("wp", true));
            #endregion            

			#endregion

			#region Nomenclature Media (droits et sélection)

			#region Droits
			sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(webSession,"wp",true));

            
            //Droit detail spot à spot TNT
            if (VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())) == DBClassificationConstantes.Vehicles.names.tv
                && !webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_DETAIL_DIGITAL_TV_ACCESS_FLAG))
                sql.Append(" and wp.id_category != " + DBConstantes.Category.ID_DIGITAL_TV + "  ");

			#endregion

			#region Sélection
			list=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerRightConstante.type.vehicleAccess);
            if (webSession.SloganIdZoom > -1)
            {
                sql.AppendFormat(" and wp.id_slogan={0}", webSession.SloganIdZoom);
            }

			sql.Append(" and ((wp.id_category<>35)) ");
			if(listMediaAccess.Length>0){
				sql.Append(" and ((wp.id_media in ("+listMediaAccess+"))) ");
			}
				sql.Append(element);
				sql.Append(" and ((wp.id_vehicle="+idVehicle.ToString()+")) ");
			
			#endregion

			#endregion
	
			// Ordre
			sql.Append("Order by " + GetOrder(VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString()))));
			#endregion

			#region Execution de la requête
			try{
				return webSession.Source.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw(new MediaCreationDataAccessException ("Impossible de charger les données pour afficher les créations:"+sql.ToString()+" ",err));
			}
			#endregion

			#endregion
		}


		#endregion

		#region Méthodes privées
		/// <summary>
		/// Donne la table à utiliser pour le vehicle indiqué
		/// </summary>
		/// <param name="idVehicle">Identifiant du vehicle</param>
		/// <param name="currentModule">Module courant</param>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
		/// Lancée quand le cas du vehicle spécifié n'est pas traité
		/// </exception>
		/// <returns>Chaine contenant le nom de la table correspondante</returns>
        //private static string GetTable(DBClassificationConstantes.Vehicles.names idVehicle,long currentModule){
        //    switch(idVehicle){
        //        case DBClassificationConstantes.Vehicles.names.press:
        //            return DBConstantes.Tables.ALERT_DATA_PRESS;
        //        case DBClassificationConstantes.Vehicles.names.internationalPress:
        //            return DBConstantes.Tables.ALERT_DATA_PRESS_INTER;
        //        case DBClassificationConstantes.Vehicles.names.radio:
        //            return DBConstantes.Tables.ALERT_DATA_RADIO;
        //        case DBClassificationConstantes.Vehicles.names.tv:
        //        case DBClassificationConstantes.Vehicles.names.others:
        //            if(currentModule == WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES.GetHashCode())
        //            return DBConstantes.Tables.DATA_SPONSORSHIP;
        //            else return DBConstantes.Tables.ALERT_DATA_TV;
        //        case DBClassificationConstantes.Vehicles.names.outdoor:
        //        case DBClassificationConstantes.Vehicles.names.instore:
        //            return DBConstantes.Tables.ALERT_DATA_OUTDOOR;
        //        default:
        //            throw new Exceptions.MediaCreationDataAccessException("GetTable(DBClassificationConstantes.Vehicles.value idMedia)-->Le cas de ce média n'est pas gérer. Pas de table correspondante.");
        //    }
        //}


		/// <summary>
		/// Fournit l'identifiant de l'élément sélectionné
		/// </summary>
		/// <returns>Condition sur l'élément sélectionné</returns>
		private static string GetIdElement(WebSession webSession, Int64 idElement,int level,Int64 idVehicle){
			
			string sql="";

			try{
				switch(webSession.CurrentModule){
					case WebConstantes.Module.Name.ALERTE_CONCURENTIELLE:
					case WebConstantes.Module.Name.ALERTE_PORTEFEUILLE:
                    case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
                        sql = string.Format(" and ((wp.{0} = {1}))", webSession.GenericProductDetailLevel.GetColumnNameLevelId(level), idElement);
						break;
					default:
						switch(webSession.PreformatedProductDetail){
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sector:
								sql=" and ((wp.id_sector="+idElement.ToString()+")) ";
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
								if(level==ResultConstantes.IDL1_INDEX)sql=" and ((wp.id_sector="+idElement.ToString()+")) ";
								else if (level==ResultConstantes.IDL2_INDEX)sql=" and ((wp.id_subsector="+idElement.ToString()+")) ";
								else sql=" and ((wp.id_group_="+idElement.ToString()+")) ";
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
								if(level==ResultConstantes.IDL1_INDEX)sql=" and ((wp.id_sector="+idElement.ToString()+")) ";
								else if (level==ResultConstantes.IDL2_INDEX)sql=" and ((wp.id_advertiser="+idElement.ToString()+")) ";
								else sql=" and ((wp.id_product="+idElement.ToString()+")) ";
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsector:
								if(level==ResultConstantes.IDL1_INDEX)sql=" and ((wp.id_sector="+idElement.ToString()+")) ";
								else sql=" and ((wp.id_subsector="+idElement.ToString()+")) ";						
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser:
								if(level==ResultConstantes.IDL1_INDEX) sql=" and ((wp.id_sector="+idElement.ToString()+")) ";
								else sql=" and ((wp.id_advertiser="+idElement.ToString()+")) ";
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorProduct:
								if(level==ResultConstantes.IDL1_INDEX) sql=" and ((wp.id_sector="+idElement.ToString()+")) ";
								else sql=" and ((wp.id_product="+idElement.ToString()+")) ";
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product:
								sql=" and ((wp.id_product="+idElement.ToString()+")) ";
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser:
								sql=" and ((wp.id_advertiser="+idElement.ToString()+")) ";
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand:
								if(level==ResultConstantes.IDL1_INDEX)sql=" and ((wp.id_advertiser="+idElement.ToString()+")) ";
								else sql=" and ((wp.id_brand="+idElement.ToString()+")) ";
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct:
								if(level==ResultConstantes.IDL1_INDEX) sql=" and ((wp.id_advertiser="+idElement.ToString()+")) ";
								else sql=" and ((wp.id_product="+idElement.ToString()+")) ";
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
								if(level==ResultConstantes.IDL1_INDEX)sql=" and ((wp.id_advertiser="+idElement.ToString()+")) ";
								else if(level==ResultConstantes.IDL2_INDEX)sql=" and ((wp.id_brand="+idElement.ToString()+")) ";
								else sql=" and ((wp.id_product="+idElement.ToString()+")) ";
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group:
								sql=" and ((wp.id_group_="+idElement.ToString()+")) ";
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand:
								if(level==ResultConstantes.IDL1_INDEX)sql=" and ((wp.id_group_="+idElement.ToString()+")) ";
								else sql=" and ((wp.id_brand="+idElement.ToString()+")) ";
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupProduct:
								if(level==ResultConstantes.IDL1_INDEX)sql=" and ((wp.id_group_="+idElement.ToString()+")) ";
								else sql=" and ((wp.id_product="+idElement.ToString()+")) ";
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiser:
								if(level==ResultConstantes.IDL1_INDEX)sql=" and ((wp.id_group_="+idElement.ToString()+")) ";
								else sql=" and ((wp.id_advertiser="+idElement.ToString()+")) ";
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
								if(level==ResultConstantes.IDL1_INDEX)sql=" and ((wp.id_group_="+idElement.ToString()+")) ";
								else if(level==ResultConstantes.IDL2_INDEX)sql=" and ((wp.id_brand="+idElement.ToString()+")) ";
								else sql=" and ((wp.id_product="+idElement.ToString()+")) ";
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
								if(level==ResultConstantes.IDL1_INDEX){
									sql=" and (("+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".ID_GROUP_ADVERTISING_AGENCY="+idElement.ToString()+")) ";
									sql+=" and (("+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_vehicle="+idVehicle+"))";
								}
								else {
									sql=" and (("+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".ID_ADVERTISING_AGENCY="+idElement.ToString()+")) ";
									sql+=" and (("+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_vehicle="+idVehicle+")) ";

								}						
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser:
								if(level==ResultConstantes.IDL1_INDEX) {
									sql=" and (("+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".ID_GROUP_ADVERTISING_AGENCY="+idElement.ToString()+")) ";
									sql+=" and (("+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_vehicle="+idVehicle+"))";
								}
								else if(level==ResultConstantes.IDL2_INDEX) {
									sql=" and (("+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".ID_ADVERTISING_AGENCY="+idElement.ToString()+")) ";
									sql+=" and (("+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_vehicle="+idVehicle+")) ";

								}
								else sql=" and ((wp.id_advertiser="+idElement.ToString()+")) ";
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct:
								if(level==ResultConstantes.IDL1_INDEX) {
									sql=" and (("+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".ID_GROUP_ADVERTISING_AGENCY="+idElement.ToString()+")) ";
									sql+=" and (("+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_vehicle="+idVehicle+"))";
								}
								else if(level==ResultConstantes.IDL2_INDEX) {
									sql=" and (("+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".ID_ADVERTISING_AGENCY="+idElement.ToString()+")) ";
									sql+=" and (("+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_vehicle="+idVehicle+")) ";

								}
								else sql=" and ((wp.id_product="+idElement.ToString()+")) ";
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
								if(level==ResultConstantes.IDL1_INDEX){
									sql=" and (("+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".ID_ADVERTISING_AGENCY="+idElement.ToString()+")) ";
									sql+=" and (("+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_vehicle="+idVehicle+")) ";
								}
								else sql=" and ((wp.id_advertiser="+idElement.ToString()+")) ";
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
								if(level==ResultConstantes.IDL1_INDEX){
									sql=" and (("+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".ID_ADVERTISING_AGENCY="+idElement.ToString()+")) ";
									sql+=" and (("+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_vehicle="+idVehicle+")) ";
								}
								else sql=" and ((wp.id_product="+idElement.ToString()+")) ";
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompany:
								sql=" and ((wp.id_holding_company="+idElement.ToString()+")) ";						
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiser:
								if(level==ResultConstantes.IDL1_INDEX)sql=" and ((wp.id_holding_company="+idElement.ToString()+")) ";
								else sql=" and ((wp.id_advertiser="+idElement.ToString()+")) ";						
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserProduct:
								if(level==ResultConstantes.IDL1_INDEX)sql=" and ((wp.id_holding_company="+idElement.ToString()+")) ";
								else if(level==ResultConstantes.IDL2_INDEX)sql=" and ((wp.id_advertiser="+idElement.ToString()+")) ";
								else sql=" and ((wp.id_product="+idElement.ToString()+")) ";
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserBrand:
								if(level==ResultConstantes.IDL1_INDEX)sql=" and ((wp.id_holding_company="+idElement.ToString()+")) ";
								else if(level==ResultConstantes.IDL2_INDEX)sql=" and ((wp.id_advertiser="+idElement.ToString()+")) ";
								else sql=" and ((wp.id_brand="+idElement.ToString()+")) ";
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser:
								if(level==ResultConstantes.IDL1_INDEX)sql=" and ((wp.id_segment="+idElement.ToString()+")) ";
								else sql=" and ((wp.id_advertiser="+idElement.ToString()+")) ";						
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentBrand:
								if(level==ResultConstantes.IDL1_INDEX)sql=" and ((wp.id_segment="+idElement.ToString()+")) ";
								else sql=" and ((wp.id_brand="+idElement.ToString()+")) ";					
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentProduct:
								if(level==ResultConstantes.IDL1_INDEX)sql=" and ((wp.id_segment="+idElement.ToString()+")) ";
								else sql=" and ((wp.id_product="+idElement.ToString()+")) ";						
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserBrand:
								if(level==ResultConstantes.IDL1_INDEX)sql=" and ((wp.id_segment="+idElement.ToString()+")) ";
								else if(level==ResultConstantes.IDL2_INDEX)sql=" and ((wp.id_advertiser="+idElement.ToString()+")) ";
								else sql=" and ((wp.id_brand="+idElement.ToString()+")) ";					
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserProduct:
								if(level==ResultConstantes.IDL1_INDEX)sql=" and ((wp.id_segment="+idElement.ToString()+")) ";
								else if(level==ResultConstantes.IDL2_INDEX)sql=" and ((wp.id_advertiser="+idElement.ToString()+")) ";
								else sql=" and ((wp.id_product="+idElement.ToString()+")) ";
								break;
							case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorHoldingCompanyAdvertiser:
								if(level==ResultConstantes.IDL1_INDEX)sql=" and ((wp.id_sector="+idElement.ToString()+")) ";
								else if(level==ResultConstantes.IDL2_INDEX)sql=" and ((wp.id_holding_company="+idElement.ToString()+")) ";
								else sql=" and ((wp.id_advertiser="+idElement.ToString()+")) ";
								break;
							default:
								sql="";
								break;
						}
						break;
				}
			}
			catch(Exception){
				sql="";
			}
			return sql;
		}
		
		/// <summary>
		/// Donne les champs à utiliser pour le vehicle indiqué
		/// </summary>
		/// <param name="idVehicle">Identifiant du vehicle</param>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
		/// Lancée quand le cas du vehicle spécifié n'est pas traité
		/// </exception>
		/// <returns>Chaine contenant les champs sélectionnés</returns>
		private static string GetFields(DBClassificationConstantes.Vehicles.names idVehicle){
			switch(idVehicle){
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					return "media"
						+", wp.date_media_Num"
						+", wp.media_paging"
						+", group_"
						+", advertiser"
						+", product"
						+", format"
                        + ", wp." + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].DatabaseField + " as " + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString()
						+", color"
                        + ", wp." + UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField + " as " + UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()
						+", location"
						+", wp.visual "
						+", wp.id_advertisement"
						+", am.disponibility_visual"
						+", am.activation"
						+", category"
						+", vehicle"
						+", wp.id_media"
                        +", wp.date_cover_num";

                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:    
					return "media"
						+", wp.date_media_num"
						+", wp.id_top_diffusion"
						+", wp.associated_file"
						+", advertiser"
						+", product"
						+", group_"
                        + ", wp." + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.duration].DatabaseField + " as " + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.duration].Id.ToString()
						+", wp.rank"
						+", wp.duration_commercial_break"
						+", wp.number_spot_com_break"
						+", wp.rank_wap"
						+", wp.duration_com_break_wap"
						+", wp.number_spot_com_break_wap"
                        + ", wp." + UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField + " as " + UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()
						+", wp.id_cobranding_advertiser"
						+", category"
						+", vehicle"
						+", id_slogan";
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.others:	
					return  "media"
						+", wp.date_media_num"
						+", wp.top_diffusion"
						+", wp.associated_file"
						+", advertiser"
						+", product"
						+", group_"
                        + ", wp." + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.duration].DatabaseField + " as " + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.duration].Id.ToString()
						+", wp.id_rank"
						+", wp.duration_commercial_break"
						+", wp.number_message_commercial_brea"
                        + ", wp." + UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField + " as " + UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()
						+", wp.id_commercial_break"
						+", category"
						+", vehicle"
                        +", wp.id_category";
				case DBClassificationConstantes.Vehicles.names.outdoor :
                case DBClassificationConstantes.Vehicles.names.instore:
					return  "media"
						+", wp.date_media_num"											
						+", advertiser"
						+", product"
						+", group_"
                        + ", wp." + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.numberBoard].DatabaseField + " as " + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.numberBoard].Id.ToString()
						+", wp.type_board"
						+", wp.type_sale"
						+", wp.poster_network"
						+", ag.agglomeration"
                        + ", wp." + UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField + " as " + UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()
						+", category"
						+", vehicle"
						+ ", wp.associated_file";
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
		private static string GetOrder(DBClassificationConstantes.Vehicles.names idVehicle){
			switch(idVehicle){
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					return "category"
						+", media"
						+", wp.date_media_Num"
						+", wp.id_advertisement"
						+", wp.media_paging"
						+", location";
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:    
					return "category"
						+", media"
						+", wp.date_media_num"
						+", wp.id_top_diffusion"
						+", wp.id_cobranding_advertiser";
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.others:	
					return  "category"
						+", media"
						+", wp.date_media_num"
						+", wp.id_commercial_break"
						+", wp.id_rank";
				case DBClassificationConstantes.Vehicles.names.outdoor:
                case DBClassificationConstantes.Vehicles.names.instore:
					return  "category"
						+", media"
						+", wp.date_media_num"
                        + ", wp." + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.numberBoard].DatabaseField + " as " + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.numberBoard].Id.ToString()
						+", wp.type_board";
				default:
					throw new Exceptions.MediaCreationDataAccessException("GetTable(DBClassificationConstantes.Vehicles.value idMedia)-->Le cas de ce média n'est pas gérer. Pas de table correspondante.");
			}
		}

		#endregion

	}
}
