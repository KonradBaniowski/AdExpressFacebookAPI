#region Informations
// Auteur: D. Mussuma 
// Date de création: 07/09/2004 
// Date de modification: 07/09/2004 
// 29/11/2005 Par B.Masson > Mise en place de IDataSource pour Execution de la requête
#endregion

using System;
using System.Collections;
using System.Data;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TablesDBConstantes=TNS.AdExpress.Constantes.DB.Tables;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Exceptions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.DataAccess.Selections.Products{
	/// <summary>
	/// Description résumée de RecapProductClassificationListDataAccess.
	/// </summary>
	public class RecapProductClassificationListDataAccess{

		/// <summary>
		/// Donne une liste de Famille, classe, groupe,variété en fonction des droits du client.
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Liste d'éléments de la nomenclature</returns>
		public static DataSet RecapGetDataForSectorAnalysis(WebSession webSession){
			
			#region Construction de la requête
			string sql="";
            string shema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.recap01).Label;
			sql+=" select "+TablesDBConstantes.SECTOR_PREFIXE+".id_sector,"+TablesDBConstantes.SECTOR_PREFIXE+".sector,"+TablesDBConstantes.SUBSECTOR_PREFIXE+".id_subsector,"+TablesDBConstantes.SUBSECTOR_PREFIXE+".subsector,"+TablesDBConstantes.GROUP_PREFIXE+".id_group_,"+TablesDBConstantes.GROUP_PREFIXE+".group_,"+TablesDBConstantes.SEGMENT_PREFIXE+".id_segment,"+TablesDBConstantes.SEGMENT_PREFIXE+".segment";
            sql += " from " + shema + ".sector " + TablesDBConstantes.SECTOR_PREFIXE 
                + ", " + shema + ".subsector " + TablesDBConstantes.SUBSECTOR_PREFIXE
                + ", " + shema + ".group_ " + TablesDBConstantes.GROUP_PREFIXE
                + ", " + shema + ".segment " + TablesDBConstantes.SEGMENT_PREFIXE + "";
			sql+=" Where";
			// Langue
			sql+=" "+TablesDBConstantes.SECTOR_PREFIXE+".id_language="+webSession.DataLanguage.ToString();
            sql += " and " + TablesDBConstantes.SUBSECTOR_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
            sql += " and " + TablesDBConstantes.GROUP_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
            sql += " and " + TablesDBConstantes.SEGMENT_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString();
			// Activation
			sql+=" and "+TablesDBConstantes.SECTOR_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql+=" and "+TablesDBConstantes.SUBSECTOR_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql+=" and "+TablesDBConstantes.GROUP_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql+=" and "+TablesDBConstantes.SEGMENT_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			
			// jointure
			sql+=" and "+TablesDBConstantes.SECTOR_PREFIXE+".id_sector="+TablesDBConstantes.SUBSECTOR_PREFIXE+".id_sector";
			sql+=" and "+TablesDBConstantes.SUBSECTOR_PREFIXE+".id_subsector="+TablesDBConstantes.GROUP_PREFIXE+".id_subsector";
			sql+=" and "+TablesDBConstantes.GROUP_PREFIXE+".id_group_="+TablesDBConstantes.SEGMENT_PREFIXE+".id_group_";

			#region Nomenclature Produit (droits)
			//Droits en accès
			sql+=SQLGenerator.getClassificationCustomerProductRight(webSession,true);
			#endregion
			
			// Tri
			sql+=" order by "+TablesDBConstantes.SECTOR_PREFIXE+".sector,"+TablesDBConstantes.SUBSECTOR_PREFIXE+".subsector,"+TablesDBConstantes.GROUP_PREFIXE+".group_,"+TablesDBConstantes.SEGMENT_PREFIXE+".segment";

			#endregion

			#region Execution de la requête
			try{
                IDataSource source = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis, WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].NlsSort); 
				return(source.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw (new RecapProductClassificationListDataAccessException("Impossible de charger une liste de Famille, classe, groupe,variété en fonction des droits du client",err));
			}
			#endregion

		}
	}
}
