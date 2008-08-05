#region Information
// Auteur: 
// Créé le: 
// Modifiée le:
//	22/08/2005	G. Facon		Solution temporaire pour les IDataSource
//	27/10/2005	D. V. Mussuma   Récupération des noms des tables et des unités via SQLGenerator
//	10/11/2005	D. V. Mussuma	Utilisation de IDataSource depuis WebSession
//	25/11/2005	B.Masson		webSession.Source
#endregion

#region Using
using System;
using System.Data;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Functions;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using TNS.FrameWork.DB.Common;
#endregion

namespace TNS.AdExpress.Web.DataAccess.Results{
	/// <summary>
	/// Classe d'accès à la base de données pour les plans média concurrentiels
	/// sur un historique long
	/// </summary>
	public class CompetitorMediaPlanAnalysisDataAccess {		

		/// <summary>
		/// Classe qui charge les données d'un plan media concurrentiel en analyse
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Données</returns>
		public static DataSet GetData(WebSession webSession) {

			#region Variables
			string list = "";
			bool premier = true;
			string tableName = SQLGenerator.getTableNameForAnalysisResult(webSession.DetailPeriod);
			string dateFieldName = SQLGenerator.getDateFieldNameForAnalysisResult(webSession.DetailPeriod);
			string unitFieldName = SQLGenerator.getTotalUnitField(webSession.Unit);
			int positionUnivers = 0;
			const string DATA_TABLE_PREFIXE = "wp";
			#endregion

			#region Construction de la requête

			string sql = "";
			// Sélection de la nomenclature Support		
			sql += "select grp,grpUnivers ,id_vehicle, vehicle, id_category, category, id_media, media,id_periodicity, date_num, unit";
			sql += " from (";

			#region While Condition pour chaque groupe d'univers
			while (webSession.PrincipalProductUniverses.ContainsKey(positionUnivers) && webSession.PrincipalProductUniverses[positionUnivers] != null) {
				if (positionUnivers > 0) {
					sql += " UNION ";
				}
				// Nom de l'univers
				//sql += " select '" + ((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).NameCompetitorAdvertiser + "' as grp , ";
				sql += " select '" + webSession.PrincipalProductUniverses[positionUnivers].Label + "' as grp , ";
				sql += " " + positionUnivers + " as grpUnivers ,wp.id_vehicle, vehicle, wp.id_category, category, wp.id_media, media, wp.id_periodicity";
				// Sélection de la date
				sql += ", " + dateFieldName + " as date_num";				
				sql += ", sum(" + unitFieldName + ") as unit";
				// Tables
				sql += " from " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".vehicle vh, " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".category ct, " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".media md, " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + tableName + " wp";
				// Jointure
				sql += " where vh.id_vehicle=wp.id_vehicle";
				sql += " and ct.id_category=wp.id_category";
				sql += " and md.id_media = wp.id_media ";
				sql += " and vh.id_language = " + webSession.DataLanguage.ToString() + " ";
				sql += " and ct.id_language = " + webSession.DataLanguage.ToString() + " ";
				sql += " and md.id_language = " + webSession.DataLanguage.ToString() + " ";
				// Activation
				sql += " and vh.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
				sql += " and ct.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
				sql += " and md.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;


				sql += " and " + dateFieldName + " >= " + webSession.PeriodBeginningDate + " ";
				sql += " and " + dateFieldName + " <= " + webSession.PeriodEndDate + " ";

				#region Nomenclature Produit (droits)
				premier = true;
				//Droits en accès
				sql += SQLGenerator.getAnalyseCustomerProductRight(webSession, "wp", true);
				// Produit à exclure en radio
				sql += SQLGenerator.GetAdExpressProductUniverseCondition(TNS.AdExpress.Constantes.Web.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, "wp", true, false);
				#endregion

				#region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et sélection)

				#region Ancienne selection produit
				//// Sélection en accès
				//premier = true;
				//// HoldingCompany
				//list = webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser, CustomerRightConstante.type.holdingCompanyAccess);
				//if (list.Length > 0) {
				//    sql += " and ((wp.id_holding_company in (" + list + ") ";
				//    premier = false;
				//}
				//// Advertiser
				//list = webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser, CustomerRightConstante.type.advertiserAccess);
				//if (list.Length > 0) {
				//    if (!premier) sql += " or";
				//    else sql += " and ((";
				//    sql += " wp.id_advertiser in (" + list + ") ";
				//    premier = false;
				//}
				//// Marque
				//list = webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser, CustomerRightConstante.type.brandAccess);
				//if (list.Length > 0) {
				//    if (!premier) sql += " or";
				//    else sql += " and ((";
				//    sql += " wp.id_brand in (" + list + ") ";
				//    premier = false;
				//}
				//// Product
				//list = webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser, CustomerRightConstante.type.productAccess);
				//if (list.Length > 0) {
				//    if (!premier) sql += " or";
				//    else sql += " and ((";
				//    sql += " wp.id_product in (" + list + ") ";
				//    premier = false;
				//}
				//// Sector
				//list = webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser, CustomerRightConstante.type.sectorAccess);
				//if (list.Length > 0) {
				//    sql += " and ((wp.id_sector in (" + list + ") ";
				//    premier = false;
				//}
				//// subsector
				//list = webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser, CustomerRightConstante.type.subSectorAccess);
				//if (list.Length > 0) {
				//    if (!premier) sql += " or";
				//    else sql += " and ((";
				//    sql += " wp.id_subsector in (" + list + ") ";
				//    premier = false;
				//}
				//// Group
				//list = webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser, CustomerRightConstante.type.groupAccess);
				//if (list.Length > 0) {
				//    if (!premier) sql += " or";
				//    else sql += " and ((";
				//    sql += " wp.id_group_ in (" + list + ") ";
				//    premier = false;
				//}
				//if (!premier) sql += " )";

				//// Sélection en Exception
				//// HoldingCompany
				//list = webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser, CustomerRightConstante.type.holdingCompanyException);
				//if (list.Length > 0) {
				//    if (premier) sql += " and (";
				//    else sql += " and";
				//    sql += " wp.id_holding_company not in (" + list + ") ";
				//    premier = false;
				//}
				//// Advertiser
				//list = webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser, CustomerRightConstante.type.advertiserException);
				//if (list.Length > 0) {
				//    if (premier) sql += " and (";
				//    else sql += " and";
				//    sql += " wp.id_advertiser not in (" + list + ") ";
				//    premier = false;
				//}
				//// Marque
				//list = webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser, CustomerRightConstante.type.brandException);
				//if (list.Length > 0) {
				//    if (premier) sql += " and (";
				//    else sql += " and";
				//    sql += " wp.id_brand not in (" + list + ") ";
				//    premier = false;
				//}
				//// Product
				//list = webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser, CustomerRightConstante.type.productException);
				//if (list.Length > 0) {
				//    if (premier) sql += " and (";
				//    else sql += " and";
				//    sql += " wp.id_product not in (" + list + ") ";
				//    premier = false;
				//}
				//// Sector
				//list = webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser, CustomerRightConstante.type.sectorException);
				//if (list.Length > 0) {
				//    if (premier) sql += " and (";
				//    else sql += " and";
				//    sql += " wp.id_sector not in (" + list + ") ";
				//    premier = false;
				//}
				//// SubSector
				//list = webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser, CustomerRightConstante.type.subSectorException);
				//if (list.Length > 0) {
				//    if (premier) sql += " and (";
				//    else sql += " and";
				//    sql += " wp.id_subsector not in (" + list + ") ";
				//    premier = false;
				//}
				//// group
				//list = webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser, CustomerRightConstante.type.groupException);
				//if (list.Length > 0) {
				//    if (premier) sql += " and (";
				//    else sql += " and";
				//    sql += " wp.id_group_ not in (" + list + ") ";
				//    premier = false;
				//}

				//if (!premier) sql += " )";
				#endregion

				// Sélection de Produits
				if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0
					&& webSession.PrincipalProductUniverses[positionUnivers] != null)
					sql += webSession.PrincipalProductUniverses[positionUnivers].GetSqlConditions(DATA_TABLE_PREFIXE, true);

				#endregion

				#region Nomenclature Media (droits et sélection)

				#region Droits
				sql += SQLGenerator.getAnalyseCustomerMediaRight(webSession, "wp", true);
				#endregion

				#region Sélection
				list = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess);
				if (list.Length > 0) sql += " and ((wp.id_vehicle in (" + list + "))) ";
				#endregion

				#endregion

				sql += " group by wp.id_vehicle, vehicle, wp.id_category, category, wp.id_media, media, ";
				sql += " wp.id_periodicity, " + dateFieldName + " ";

				positionUnivers++;

			}
			#endregion

			sql += " )";
			sql += " order by id_vehicle, vehicle, category, media, grpUnivers, date_num";

			#endregion

			#region Execution de la requête
			try {
				return webSession.Source.Fill(sql.ToString());
			}
			catch (System.Exception err) {
				throw (new CompetitorMediaPlanAnalysisDataAccessException("Impossible de charger les données d'un plan media concurrentiel" + sql, err));
			}
			#endregion

		}

	}
}
