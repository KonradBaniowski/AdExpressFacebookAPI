#region Informations
// Auteur: G. Ragneau
// Date de création: 16/09/2004 
// Date de modification: 16/09/2004 
// 25/11/2005 Par B.Masson & G.Facon > webSession.Source
// 16/011/2006 Par D. V. Mussuma> requête sur les tables du schéma recap01
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Oracle.DataAccess.Client;
using System.Text;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Rules.Customer;
using DBCst=TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Exceptions;
using TNS.FrameWork.DB.Common;
using TNS.Classification.Universe;
using TNS.AdExpress.Classification;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.DataAccess.Selections.Products{
	/// <summary>
	/// Class d'accès aux annonceurs dans le cadre des recap
	/// </summary>
	public class RecapAdvertiserDataAccess{
		/// <summary>
		/// Donne une liste des annonceurs accessibles par un login ormi les annonceurs présents dans une 
		/// liste d'exceptions.
		/// Les annonceurs ont tous des références dans la sélection produits de la session
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="advetiserException">liste d'identifiants annonceurs séparés par une virgule à ne pas rapatrier</param>
		/// <returns>Dataset contenant des couples (id_advertiser, advertiser)</returns>
		public static DataSet GetAvertiserData(WebSession webSession, string advetiserException){
			StringBuilder sql = new StringBuilder(1000);
			string groupAccess = "", segmentAccess = "", segmentException="";
			List<NomenclatureElementsGroup> groups = null;
			AdExpressUniverse adExpressUniverse = null;

			if (webSession.PrincipalProductUniverses.Count > 0) adExpressUniverse = webSession.PrincipalProductUniverses[0];

			if (adExpressUniverse != null && adExpressUniverse.Count()>0) {
				groups = adExpressUniverse.GetExludes();
				if (groups != null && groups.Count > 0) {
					segmentException = groups[0].GetAsString(TNSClassificationLevels.SEGMENT);
				}
				groups = adExpressUniverse.GetIncludes();
				if (groups != null && groups.Count > 0) {
					groupAccess = groups[0].GetAsString(TNSClassificationLevels.GROUP_);
					segmentAccess = groups[0].GetAsString(TNSClassificationLevels.SEGMENT);
				}
			}

			#region Construction de la requête

			#region select
			sql.Append("select distinct " + DBCst.Tables.ADVERTISER_PREFIXE + ".id_advertiser");
			sql.Append(" ," + DBCst.Tables.ADVERTISER_PREFIXE + ".advertiser");
			#endregion

			#region from
			sql.Append(" from " + DBCst.Schema.RECAP_SCHEMA + ".advertiser " + DBCst.Tables.ADVERTISER_PREFIXE);
			sql.Append(" ," + DBCst.Schema.RECAP_SCHEMA + ".product " + DBCst.Tables.PRODUCT_PREFIXE);
			sql.Append(" ," + DBCst.Schema.RECAP_SCHEMA + ".segment " + DBCst.Tables.SEGMENT_PREFIXE);
			sql.Append(" ," + DBCst.Schema.RECAP_SCHEMA + ".group_ " + DBCst.Tables.GROUP_PREFIXE);
			sql.Append(" ," + DBCst.Schema.RECAP_SCHEMA + ".holding_company " + DBCst.Tables.HOLDING_PREFIXE);

			#endregion

			#region conditions

			#region jointures
			sql.Append(" where " + DBCst.Tables.HOLDING_PREFIXE + ".id_holding_company=" + DBCst.Tables.ADVERTISER_PREFIXE + ".id_holding_company");
			sql.Append(" and " + DBCst.Tables.ADVERTISER_PREFIXE + ".id_advertiser=" + DBCst.Tables.PRODUCT_PREFIXE + ".id_advertiser");
			sql.Append(" and " + DBCst.Tables.PRODUCT_PREFIXE + ".id_segment=" + DBCst.Tables.SEGMENT_PREFIXE + ".id_segment");
			sql.Append(" and " + DBCst.Tables.SEGMENT_PREFIXE + ".id_group_=" + DBCst.Tables.GROUP_PREFIXE + ".id_group_");
			#endregion

			#region sélection
			string str = "";
			bool or = false;
			//inclusion des groupes
			//if ((str = webSession.GetSelection(webSession.CurrentUniversProduct, TNS.AdExpress.Constantes.Customer.Right.type.groupAccess)).Length>0){
			if (groupAccess != null && groupAccess.Length > 0) {
				sql.Append(" and ((" + DBCst.Tables.GROUP_PREFIXE + ".id_group_ in (" + groupAccess + "))");
				or=true;
			}
			//incusion des variétés
			//if ((str = webSession.GetSelection(webSession.CurrentUniversProduct, TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess)).Length>0){
			if (segmentAccess != null && segmentAccess.Length > 0){
				if (or) sql.Append(" or ");
				else sql.Append(" and (");
				sql.Append("(" + DBCst.Tables.SEGMENT_PREFIXE + ".id_segment in ("+ segmentAccess + "))");
				or = true;
			}
			if (or)sql.Append(")");
			//exclusion des variétés			
			//if ((str = webSession.GetSelection(webSession.CurrentUniversProduct, TNS.AdExpress.Constantes.Customer.Right.type.segmentException)).Length>0)
			if(segmentException !=null && segmentException.Length>0)
				sql.Append(" and " + DBCst.Tables.SEGMENT_PREFIXE + ".id_segment not in (" + segmentException + ")");
			//exclusion des éléménents de la liste exceptions
			if (advetiserException.Length>0){
				sql.Append(" and " + DBCst.Tables.ADVERTISER_PREFIXE + ".id_advertiser not in (" + advetiserException + ")");
			}
			#endregion

			#region droits (Pas de droit sur les annonceurs pour le moment)
			//nomenclature annonceurs
			//sql.Append(SQLGenerator.getAnalyseCustomerAdvertiserRight(webSession, true));
			#endregion

			#region langue
			sql.Append(" and " + DBCst.Tables.ADVERTISER_PREFIXE + ".id_language=" + webSession.SiteLanguage);
			sql.Append(" and " + DBCst.Tables.PRODUCT_PREFIXE + ".id_language=" + webSession.SiteLanguage);
			sql.Append(" and " + DBCst.Tables.SEGMENT_PREFIXE + ".id_language=" + webSession.SiteLanguage);
			sql.Append(" and " + DBCst.Tables.GROUP_PREFIXE + ".id_language=" + webSession.SiteLanguage);
			sql.Append(" and " + DBCst.Tables.HOLDING_PREFIXE + ".id_language=" + webSession.SiteLanguage);
			#endregion

			#region Activation
			sql.Append(" and " + DBCst.Tables.ADVERTISER_PREFIXE + ".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and " + DBCst.Tables.PRODUCT_PREFIXE + ".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and " + DBCst.Tables.SEGMENT_PREFIXE + ".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and " + DBCst.Tables.GROUP_PREFIXE + ".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and " + DBCst.Tables.HOLDING_PREFIXE + ".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			
			#endregion
			
			#endregion
			
			#region Tri
			sql.Append(" order by " +  DBCst.Tables.ADVERTISER_PREFIXE + ".advertiser");
			#endregion

			#endregion
			
			#region Execution de la requête
            IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
			try{
//				return(webSession.Source.Fill(sql.ToString()));
				return(dataSource.Fill(sql.ToString()));
			}
			catch(System.Exception err) {
				throw(new RecapAdvertiserDataAccessException("Impossible d'obtenir une liste des annonceurs accessibles par un login ormi les annonceurs présents dans une liste d'exceptions",err));
			}
			#endregion

		}
	}
}
